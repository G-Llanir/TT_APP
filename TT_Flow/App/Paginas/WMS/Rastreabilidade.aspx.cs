using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using TT_Hub.App.Paginas.RRHH;
using static Permissao.WMS;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.WMS
{
    public partial class Rastreabilidade : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Valida Permissão 
            FUNCOES.ValidaPermissao(Permissao.WMS.Rastreabilidade.Consultar, true);

            // Vínculo obrigatório do UserControl do Quagga com os inputs da tela
            LeitorQuaggaPesquisa.txtClient = txtLeitura.ClientID;
            LeitorQuaggaPesquisa.click = cmdPesquisar.ClientID;

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = "Rastreabilidade";

                // Reseta a sessão da câmera ao entrar na tela
                Session["CameraAbertaRastreabilidade"] = false;
                DivBipador.Visible = false;
            }
        }

        protected void cmdAbrirCamera_Click(object sender, EventArgs e)
        {
            bool abrir = Session["CameraAbertaRastreabilidade"] != null ? (bool)Session["CameraAbertaRastreabilidade"] : false;
            abrir = !abrir; // Alterna o status

            Session["CameraAbertaRastreabilidade"] = abrir;
            DivBipador.Visible = abrir;
            cmdAbrirCamera.Text = abrir ? "Fechar Câmera / Leitor" : "Abrir Câmera / Leitor";
            cmdAbrirCamera.CssClass = abrir ? "btn btn-lg btn-danger" : "btn btn-lg btn-success";

            if (abrir)
            {
                LeitorQuaggaPesquisa.AbrirCameraVariante();
                divCodigoManual.Style["Display"] = "none"; // Oculta o input manual se a câmera estiver na tela
            }
            else
            {
                LeitorQuaggaPesquisa.DesligarCamVariante();
                divCodigoManual.Style["Display"] = "block"; // Volta o input manual
            }

            updGeral.Update();
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            string termoBuscado = "";

            // Tenta pegar do Quagga
            if (!string.IsNullOrEmpty(LeitorQuaggaPesquisa.GetCodigoBarras(txtLeitura)))
            {
                termoBuscado = LeitorQuaggaPesquisa.GetCodigoBarras(txtLeitura).Trim();
            }
            // Senão pega da digitação manual
            else
            {
                termoBuscado = txtLeitura.Text.Trim();
            }

            // Se estiver vazio, ABORTA antes de apagar a Grid (Isso mata o problema de esconder a tela sem querer)
            if (string.IsNullOrEmpty(termoBuscado))
            {
                txtLeitura.Focus();
                return;
            }

            // Só esconde os resultados e limpa a tela DEPOIS de confirmar que tem pesquisa válida
            pnResultado.Visible = false;
            txtLeitura.Text = "";

            Dictionary<string, string> vParametros = new Dictionary<string, string>
    {
        { "@sBusca", termoBuscado }
    };

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_WMS_RastreabilidadeProduto", vParametros, false);

            if (tb != null && tb.Rows.Count > 0)
            {
                DataRow primeiraLinha = tb.Rows[0];

                lblDscProduto.Text = primeiraLinha["sDscProduto"].ToString();
                lblCodigoProduto.Text = primeiraLinha["sCodigoProduto"].ToString();
                lblFamilia.Text = primeiraLinha["sFamilia"].ToString();
                lblUnidade.Text = primeiraLinha["sUnidade"].ToString();

                lblCodigoBipado.Text = termoBuscado.ToUpper();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvMovimentacao, tb, 0, new int[1] { 0 }, "desc", "false", "''"), true);

                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro($"Nenhum produto ou histórico localizado para: <b>{termoBuscado}</b>");
            }

            // Foco de volta
            bool isCameraAberta = Session["CameraAbertaRastreabilidade"] != null && (bool)Session["CameraAbertaRastreabilidade"];
            if (!isCameraAberta)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "focusInput", "setTimeout(function() { document.getElementById('" + txtLeitura.ClientID + "').focus(); }, 200);", true);
            }

            updGeral.Update();
        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public static object[] BuscarProdutosRastreabilidadeAutocomplete(string termo)
        {
            List<object> lstProdutos = new List<object>();

            if (termo.Length >= 2)
            {
                try
                {
                    // Busca blindada via parâmetros e fazendo a pesquisa avançada
                    string query = @"
                SELECT TOP 30 
                    p.idItem, 
                    p.sCodigo, 
                    p.sDscProduto, 
                    p.sCodigoEAN 
                FROM tbl_Flow_Produtos p (NOLOCK)
                WHERE p.sCodigo LIKE '%' + @termo + '%' 
                   OR p.sDscProduto LIKE '%' + @termo + '%' 
                   OR p.sCodigoEAN LIKE '%' + @termo + '%'
                   OR EXISTS (
                       SELECT 1 FROM tbl_Flow_WMS_OPI_Etiqueta e (NOLOCK) 
                       WHERE e.idProduto = p.idItem AND e.sCodigoBarras = @termoLimpo
                   )
                ORDER BY p.sDscProduto
            ";

                    // Usando SqlConnection/SqlCommand purinho para passar o @termo com segurança,
                    // ou se quiser, pode usar seu BD.ExecutarDataReader formatando a string (mostro com SqlDataReader padrão)
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(TT.FrameWork.BD.StringDeConexao))
                    {
                        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@termo", termo);
                            cmd.Parameters.AddWithValue("@termoLimpo", termo.Trim());

                            conn.Open();
                            using (System.Data.SqlClient.SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    string label = $"{sdr["sCodigo"]} - {sdr["sDscProduto"]}";

                                    if (sdr["sCodigoEAN"] != DBNull.Value && !string.IsNullOrEmpty(sdr["sCodigoEAN"].ToString()))
                                    {
                                        label += $" (EAN: {sdr["sCodigoEAN"]})";
                                    }

                                    lstProdutos.Add(new
                                    {
                                        label = label,
                                        exibicao = "✅ " + label,
                                        value = sdr["idItem"].ToString(),
                                        busca = sdr["sCodigo"].ToString() // O que vai ser jogado no TextBox para pesquisar
                                    });
                                }
                            }
                        }
                    }

                    // Tratativa caso não ache nenhum item (Gera o feedback de erro vermelho)
                    if (lstProdutos.Count == 0)
                    {
                        lstProdutos.Add(new
                        {
                            label = "Produto não encontrado",
                            exibicao = "🚫 Produto não encontrado",
                            value = "VAZIO",
                            busca = ""
                        });
                    }
                }
                catch (Exception)
                {
                    // Ignora falhas silenciosas do autocomplete para não estourar na tela
                }
            }

            return lstProdutos.ToArray();
        }
    }
}