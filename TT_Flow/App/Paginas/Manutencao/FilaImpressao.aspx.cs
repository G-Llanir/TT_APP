using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using TT.FrameWork;
using System.Web.UI.WebControls;
using static TT.FrameWork.BD;
using static Permissao;
using NPOI.SS.Formula.Functions;
using System.Globalization;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class FilaImpressao : Page
    {
        string sTituloPagina = "Fila de Impressão";
        string sPagina_NovoRegistro = "";
        string sProcedure = "sp_Manipula_tbl_Flow_WMS_OPI_Etiqueta";
        string sRealocar = "S";
        string sCancelar = "N";

        #region | Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.FilaImpressao.Consultar, true);
            cmdCancelarFilas.Visible = FUNCOES.ValidaPermissao(Permissao.FilaImpressao.CancelarFilasChk);
            cmdRecolocarFilas.Visible = FUNCOES.ValidaPermissao(Permissao.FilaImpressao.RecolocarFila);
            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                Pesquisar("0");
            }

            txtPesquisa.Focus();
        }
        #endregion

        #region | Eventos
        protected void cmdPesquisar_Click(object sender, EventArgs e)
         => Pesquisar("0");
        protected void cmdCancelarFilas_Click(object sender, EventArgs e)
        {
            Atualizar_Filas(sRealocar);
            Pesquisar("0");
        }
        protected void cmdRecolocarFilas_Click(object sender, EventArgs e)
        {
            Atualizar_Filas(sCancelar);
            Pesquisar("0");
        }
        protected void CancelarImpressao_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string idEtiqueta = btn.CommandArgument;

            if (idEtiqueta != "0")
                Atualizar_Fila(idEtiqueta, sRealocar);
        }
        protected void RecolocarImpressao_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string idEtiqueta = btn.CommandArgument;

            if (idEtiqueta != "0")
                Atualizar_Fila(idEtiqueta, sCancelar);
        }
        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            bool podeCancelarFila = FUNCOES.ValidaPermissao(Permissao.FilaImpressao.CancelarFila);
            bool podeRecolocarFila = FUNCOES.ValidaPermissao(Permissao.FilaImpressao.RecolocarFila);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //--- AÇÃO / PERMISSÕES -------------------------------------------------
                LinkButton btnDesativar = (LinkButton)e.Row.FindControl("cmdCancelarImpressao");
                LinkButton btnRecolocar = (LinkButton)e.Row.FindControl("cmdRecolocarImpressao");

                if (btnDesativar != null)
                    btnDesativar.Visible = podeCancelarFila;
                else if (btnRecolocar != null)
                    btnRecolocar.Visible = podeRecolocarFila;

                //--- COR DA LINHA ------------------------------------------------------
                string cancelado = DataBinder.Eval(e.Row.DataItem, "sCancelado")?.ToString();

                if (string.Equals(cancelado, "Sim", StringComparison.OrdinalIgnoreCase))
                {
                    e.Row.CssClass += " danger";
                }
                else
                {
                    e.Row.CssClass += " success";
                }
            }

            if (e.Row.RowType == DataControlRowType.Header)
            {
                CheckBox chkAll = (CheckBox)e.Row.FindControl("chkSelectAll");
                if (chkAll != null)
                {
                    chkAll.AutoPostBack = true;
                    chkAll.CheckedChanged += new EventHandler(chkSelectAll_CheckedChanged);
                }
            }

            //--- ESCONDER / REEXIBIR A COLUNA DE AÇÃO ---------------------------------
            if (!podeCancelarFila && !podeRecolocarFila)
                Funcoes.EsconderColunas(dtgvConsulta, "Ação");
            else
                Funcoes.ReexibirColunas(dtgvConsulta, "Ação");
        }
        private (int start, int length) GetCurrentPageInfo()
        {
            int start = int.TryParse(hddPageStart.Value, out var s) ? s : 0;
            int length = int.TryParse(hddPageLength.Value, out var l) ? l : 50; 
            return (start, length);
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkAll = (CheckBox)sender;

                // sua pesquisa / re-bind normal
                Pesquisar(hddidEtiqueta.Value);

            var (start, len) = GetCurrentPageInfo();

            for (int i = start; i < start + len && i < dtgvConsulta.Rows.Count; i++)
            {
                CheckBox chk = (CheckBox)dtgvConsulta.Rows[i].FindControl("chkOpcao");
                if (chk != null) chk.Checked = chkAll.Checked;
            }
        }
        #endregion

        #region | Funções de Pesquisar
        protected void Pesquisar(string idEtiqueta)
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR-IMPRESSAO-FILA" },
                { "@sDscEtiqueta", txtPesquisa.Text.Trim() },
                { "@sFiltroFila", ddlsFiltroFila.SelectedValue.ToString() },
                { "@dtImpressao", DateTimeParaBD(txtDtImpressao) },
            };

            if (idEtiqueta != "0" || string.IsNullOrEmpty(idEtiqueta))
            {
                vParametros.Add("@idEtiqueta", idEtiqueta);

                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros, false);

                hddidEtiqueta.Value = idEtiqueta;
            }

            if (vParametros.ContainsKey("@idEtiqueta"))
            {
                vParametros.Remove("@idEtiqueta");
            }

            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros, false);
            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 0, new int[2] { 4,7 }, "asc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");

        }
        #endregion

        #region | Funções
        void Atualizar_Fila(string idEtiqueta, string cancelado)
        {
            try
            {
                if (string.IsNullOrEmpty(hddidEtiqueta.Value) || hddidEtiqueta.Value == "0")
                {
                    if (idEtiqueta == "0" || string.IsNullOrEmpty(idEtiqueta))
                        idEtiqueta = "0";
                }
                else
                {
                    if (idEtiqueta == "0" || string.IsNullOrEmpty(idEtiqueta))
                        idEtiqueta = hddidEtiqueta.Value;
                }


                Dictionary<string, string> vParametros = new Dictionary<string, string>
               {
                { "@sFuncao", "ATUALIZAR-FILA-IMPRESSAO" },
                { "@idEtiqueta", idEtiqueta },
                { "@sCancelado", cancelado},
                { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                };
                DataSet dsSalvar;
                dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros, false);

                if (BD.ValidarDataSet(dsSalvar))
                {
                    if (cancelado == "S")
                    {
                        MensagemPagina.MostraMensagem_Sucesso("Foi desativado da fila com sucesso!");
                        
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Sucesso("Foi reativado na fila com sucesso!");
                    }
                    Pesquisar("0");
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao salvar!");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.ToString());
            }
        }
        void Atualizar_Filas(string cancelado)
        {
            try
            {
                bool algumaMarcada = false;

                foreach (GridViewRow row in dtgvConsulta.Rows)
                {
                    CheckBox chk = row.FindControl("chkOpcao") as CheckBox;

                    if (chk != null && chk.Checked)
                    {
                        string idEtiqueta = dtgvConsulta.DataKeys[row.RowIndex].Value.ToString();

                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "ATUALIZAR-FILA-IMPRESSAO" },
                    { "@idEtiqueta", idEtiqueta },
                    { "@sCancelado", cancelado },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                };

                        DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros, false);

                        if (!BD.ValidarDataSet(dsSalvar))
                        {
                            MensagemPagina.MostraMensagem_Erro($"Erro ao salvar etiqueta ID {idEtiqueta}.");
                            return;
                        }

                        algumaMarcada = true;
                    }
                }

                if (algumaMarcada)
                {
                    if (cancelado == "S")
                    {
                        MensagemPagina.MostraMensagem_Sucesso("Foi desativado da fila com sucesso!");

                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Sucesso("Foi reativado na fila com sucesso!");
                    }
                    Pesquisar("0");
                }
                else
                {
                    MensagemPagina.MostraMensagem_Aviso("Nenhuma fila marcada para atualização.");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.ToString());
            }
        }
        string DateTimeParaBD(TextBox txt)
        {
            if (txt.Text == "")
            {
                return txt.Text.ToString();
            }
            else
            {
                DateTime dt = DateTime.Parse(txt.Text.ToString(), CultureInfo.InvariantCulture);
                return dt.ToString();
            }
        }
        #endregion

        #region | Validações

        #endregion
    }
}