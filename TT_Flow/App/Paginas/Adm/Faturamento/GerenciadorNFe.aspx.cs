using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using static TT.FrameWork.BD;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Grid;

namespace TT_Flow.App.Paginas.Adm.Faturamento
{
    public partial class GerenciadorNFe : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidaPermissao(Permissao.Administracao.NFe.Consultar, true);

            if (!IsPostBack)
            {
                Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscCodigoEmpresa", false, "Todas as Empresas", "0");
                Pesquisar();
            }
        }

        protected void Pesquisar()
        {
            try
            {
                pnResultado.Visible = false;

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR" },
                    { "@sPesquisa", txtPesquisa.Text },
                    { "@dtInicial", txtDataInicial.Text },
                    { "@dtFinal", txtDataFinal.Text },
                    { "@sStatus", ddlsStatus.SelectedValue },
                    { "@idEmpresa", ddlidEmpresa.SelectedValue },
                    { "@idOrigem", ddlidOrigem.SelectedValue },
                    { "@sTipoXML", "2" }
                };
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametros);
                DataTable tb = ds.Tables[0];

                if (ValidarDataSet(ds))
                {
                    pnResultado.Visible = true;
                    hddsCaminho_UniNFe.Value = tb.Rows[0]["sCaminho_UniNFe"].ToString();
                    var nColunasDatas = new int[1] { 4 };
                    ScriptManager.RegisterStartupScript(Page, GetType(), "DataTables", DataBindComScriptData(dtgvConsulta, tb, 2, nColunasDatas, "desc", ltOcultarColunas, new List<int> { -1 }, new List<int> { 4, 8 }), true);
                
                }
                else MensagemPagina.MostraMensagem_Erro("Não foram encontrados registros de NF-e!");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.ToString());
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton cmdDownloadDANFE = e.Row.FindControl("cmdDownloadDANFE") as LinkButton;
                LinkButton btnXML = e.Row.FindControl("btnXML") as LinkButton;
                string chaveNFe = dtgvConsulta.DataKeys[e.Row.RowIndex].Values["chNFe"].ToString();
                string sLinkPDF = dtgvConsulta.DataKeys[e.Row.RowIndex].Values["sLinkPDF"].ToString();
                string sXML_Autorizado = dtgvConsulta.DataKeys[e.Row.RowIndex].Values["sXML_Autorizado"].ToString();

                //if (sXML_Autorizado != "S")
                //    cmdDownloadDANFE.Visible = false;

                if (e.Row.Cells[1].Text.Contains("NFS"))
                {
                    cmdDownloadDANFE.Text = "DANFSe";
                    cmdDownloadDANFE.SetAttribute("data-arquivo", DataBinder.Eval(e.Row.DataItem, "idArquivo").ToString());
                }

                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }

        protected void cmdDownloadDANFE_Click(object sender, EventArgs e)
        {
            Funcoes_NFe.Download.PDF(Page, dtgvConsulta.DataKeys[((sender as LinkButton).NamingContainer as GridViewRow).RowIndex].Values["idXML"].ToString());
            Pesquisar();
        }

        protected void cmdDownloadXML_Click(object sender, EventArgs e)
        {
            Funcoes_NFe.Download.XML(Page, dtgvConsulta.DataKeys[((sender as LinkButton).NamingContainer as GridViewRow).RowIndex].Values["idXML"].ToString());
            Pesquisar();
        }

        protected void btnGerarPDF_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            int index = row.RowIndex;
            string sLinkPDF = dtgvConsulta.DataKeys[index]["sLinkPDF"].ToString();
            string script = $"window.open('{sLinkPDF}', '_blank');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenPDF", script, true);
            Pesquisar();
        }

        protected void btnGerarTXT_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            int index = row.RowIndex;
            string idArquivo = dtgvConsulta.DataKeys[index]["idArquivo"].ToString();

            string sNomeArquivo = "";
            byte[] bObjArquivo = null;

            Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idArquivo", idArquivo }
            };
            DataTable dtArquivo = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

            foreach (DataRow item in dtArquivo.Rows)
            {
                try
                {
                    sNomeArquivo = item["sNomeArquivo"].ToString();
                    bObjArquivo = (byte[])item["vbArquivo"];
                }
                catch { }

                FileStream lObjFile = new Arquivo().TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
                lObjFile.Close();
                lObjFile.Dispose();

                DownloadArquivo(Page, sNomeArquivo);
            }

            Pesquisar();
        }

        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {
            try
            {
                string link = "";
                if (fu_EnviarArquivo.HasFile)
                {
                    string extensao = Path.GetExtension(fu_EnviarArquivo.FileName).ToLower();
                    if (extensao != ".txt")
                        throw new Exception("Apenas arquivos .txt são permitidos.");

                    string conteudoTxt = string.Empty;
                    using (StreamReader reader = new StreamReader(fu_EnviarArquivo.PostedFile.InputStream, Encoding.UTF8))
                    {
                        conteudoTxt = reader.ReadToEnd();
                    }

                    string[] linhas = conteudoTxt.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string linha in linhas)
                    {
                        if (linha.StartsWith("2") && linha.Length >= 56)
                            link = GerarLinkImpressao(linha);
                    }

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "Upload_PDF" },
                        { "@sLinkPDF", link },
                        { "@idXML", hddIdLinha.Value }
                    };
                    ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametros);
                }

                Pesquisar();
            }
            catch (Exception ex)
            {
                Pesquisar();
                MensagemPagina.MostraMensagem_Erro(ex.ToString());

            }
        }

        private string GerarLinkImpressao(string registroTipo6)
        {
            string ccm = registroTipo6.Substring(1, 8);

            // Número da NFS-e (5 dígitos)
            string numeroNfse = registroTipo6.Substring(26, 5); // posição correta

            // Código de verificação (8 caracteres)
            string codigoVerificacao = registroTipo6.Substring(31, 8); // posição correta

            return $"https://nfe.prefeitura.sp.gov.br/contribuinte/notaprint.aspx?ccm={ccm}&nf={numeroNfse}&cod={codigoVerificacao}";
        }
    }
}
