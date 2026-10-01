//using iTextSharp.tool.xml.html.head;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using static TT.FrameWork.BD;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using GRID = TT.FrameWork.Grid;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.Adm.Faturamento
{
    public partial class GerenciadorNFe : System.Web.UI.Page
    {
        string sTituloPagina = "Gerenciador NF-e";
        string sPagina_NovoRegistro = "app/Paginas/Adm/Faturamento/GerenciadorNFe.aspx";

        List<string> itensSelecionados = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Administracao.NFe.Consultar, true);

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscCodigoEmpresa", false, "Todas as Empresas", "0");
                Pesquisar();
            }
            //GeraPDF();

            List<int> indexesToIgnore = new List<int> { };
            List<int> indexesParaIniciarOcultado = new List<int> { 4, 8 };            

            Grid.BotoesOcultarColunaComFiltro(placeholderButtons, dtgvConsulta, this, indexesToIgnore, indexesParaIniciarOcultado);

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
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametros);

                if (BD.ValidarDataSet(ds))
                {
                    pnResultado.Visible = true;
                    hddsCaminho_UniNFe.Value = ds.Tables[0].Rows[0]["sCaminho_UniNFe"].ToString();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", GRID.DataBindComScriptData(dtgvConsulta, ds.Tables[0], 2, "desc", "false", "''"), true);
                }
                else
                    MensagemPagina.MostraMensagem_Erro("Não foram encontrados registros de NF-e!");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.ToString());
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
       =>
           Pesquisar();




        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton cmdDownloadDANFE = e.Row.FindControl("cmdDownloadDANFE") as LinkButton;
                LinkButton btnXML = e.Row.FindControl("btnXML") as LinkButton;
                //LinkButton btnGerarPDF = e.Row.FindControl("btnGerarPDF") as LinkButton;
                //LinkButton btnGerarTXT = e.Row.FindControl("btnGerarTXT") as LinkButton;
                //LinkButton lnkEnvio_UpLoad = e.Row.FindControl("lnkEnvio_UpLoad") as LinkButton;
                string chaveNFe = dtgvConsulta.DataKeys[e.Row.RowIndex].Values["chNFe"].ToString();
                string sLinkPDF = dtgvConsulta.DataKeys[e.Row.RowIndex].Values["sLinkPDF"].ToString();
                string sXML_Autorizado = dtgvConsulta.DataKeys[e.Row.RowIndex].Values["sXML_Autorizado"].ToString();
                //btnGerarTXT.Visible = false;
                //lnkEnvio_UpLoad.Visible = false;
                //btnGerarPDF.Visible = false;


                if (sXML_Autorizado != "S")
                {
                    cmdDownloadDANFE.Visible = false;
                }

                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }

        protected void cmdDownloadDANFE_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            string idXML = dtgvConsulta.DataKeys[row.RowIndex].Values["idXML"].ToString();
            Funcoes_NFe.Download.PDF(Page, idXML);
            Pesquisar();
        }

        protected void cmdDownloadXML_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            string idXML = dtgvConsulta.DataKeys[row.RowIndex].Values["idXML"].ToString();
            Funcoes_NFe.Download.XML(Page, idXML);
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

            Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
            DataTable dtArquivo;
            vParametrosItem = new Dictionary<string, string>
                                    {
                                        {"@sFuncao",                    "CONSULTAR_DETALHE" },
                                        {"@idArquivo",                  idArquivo}
                                    };

            dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

            foreach (DataRow item in dtArquivo.Rows)
            {
                try
                {
                    sNomeArquivo = item["sNomeArquivo"].ToString();
                    bObjArquivo = (byte[])item["vbArquivo"];
                }
                catch
                {

                }

                TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
                lObjFile.Close();
                lObjFile.Dispose();

                FUNCOES.DownloadArquivo(Page, sNomeArquivo);
                //Arquivo = Server.MapPath("~/Download/" + sNomeArquivo);
            }
            Pesquisar();
        }

        protected void dtgvConsulta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Upload_Arquivo")
            {
                string idXML = e.CommandArgument.ToString();
                hddIdLinha.Value = idXML;
                Pesquisar();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_AbrirModalUploadArquivos", "$('#UploadArquivos_Modal').modal('show');", true );
            }
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
                    {
                        throw new Exception("Apenas arquivos .txt são permitidos.");
                    }
                    string conteudoTxt = string.Empty;
                    using (StreamReader reader = new StreamReader(fu_EnviarArquivo.PostedFile.InputStream, Encoding.UTF8))
                    {
                        conteudoTxt = reader.ReadToEnd();
                    }
                    string[] linhas = conteudoTxt.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string linha in linhas)
                    {
                        if (linha.StartsWith("2") && linha.Length >= 56)
                        {
                            link = GerarLinkImpressao(linha);
                        }
                    }

                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "Upload_PDF");
                    vParametros.Add("@sLinkPDF", link);
                    vParametros.Add("@idXML", hddIdLinha.Value);
                    dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametros);
                }
                //string idArquivo = "0";
                //Byte[] lObjArquivo = null;
                //TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                //lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(fu_EnviarArquivo.FileName, fu_EnviarArquivo.PostedFile.InputStream);

                //SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", TT.FrameWork.BD.StringDeConexao);

                //DataSet tabela = new DataSet();
                //da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
                //da.SelectCommand.CommandType = CommandType.StoredProcedure;
                //SqlCommand lObjCommand = new SqlCommand();

                //da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR";
                //da.SelectCommand.Parameters.Add("@idTipoArquivo", SqlDbType.Int).Value = 10009;
                //da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = 0;
                //da.SelectCommand.Parameters.Add("@sNomeArquivo", SqlDbType.VarChar).Value = fu_EnviarArquivo.FileName;
                //da.SelectCommand.Parameters.Add("@sDscArquivo", SqlDbType.VarChar).Value = "";
                //da.SelectCommand.Parameters.Add("@sObservacao", SqlDbType.VarChar).Value = "";
                //da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = lObjArquivo;
                //da.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.Int).Value = IDENTITY.Variaveis.idUsuario();
                //da.SelectCommand.Parameters.Add("@dtExpiracaoDoc", SqlDbType.VarChar).Value = "";
                //da.SelectCommand.Parameters.Add("@dtRegistroDoc", SqlDbType.VarChar).Value = "";

                //try
                //{
                //    da.Fill(tabela);
                //    idArquivo = RETORNO.DATASET(tabela, 0, "idArquivo");
                //}
                //catch (Exception ex)
                //{
                //    throw new Exception(string.Format("Erro BD-DS: {0}", ex.Message));
                //}

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