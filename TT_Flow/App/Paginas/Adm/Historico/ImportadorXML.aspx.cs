using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.IO;
using System.Web;
using TT_Flow.App.Controles;

namespace TT_Flow.App.Paginas.Adm.Historico
{
    public partial class ImportadorXML : System.Web.UI.Page
    {
        string sTituloPagina = "Importação de XML";
        string sPagina_NovoRegistro = "app/Paginas/Adm/Historico/ImportadorXML_Detalhe.aspx";

        List<string> itensSelecionados = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-ImportaçãodeXML.pdf";
            FUNCOES.ValidaPermissao(Permissao.Administracao.Historico.ImportaçãodeXML.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Administracao.Historico.ImportaçãodeXML.incluir);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                PopularCombos();
                itensSelecionados.Clear();
                Pesquisar("");
            }

            txtPesquisa.Focus();
        }

        protected void Pesquisar(string sPesquisa)
        {
            try
            {
                pnResultado.Visible = false;
                
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR" },
                    { "@sPesquisa", sPesquisa },
                    { "@idTipoArquivo", "10010" },
                    { "@idStatus", ddlTipo.SelectedValue },
                    { "@idParceiro_Dest", ddlCliente.SelectedValue },
                    { "@idParceiro_Emit", ddlEmpresa.SelectedValue },
                    { "@dtInicio", txtdtInicio.Text },
                    { "@dtFinal", txtdtFinal.Text }
                };

                DataSet dsArquivos = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametros);

                if (dsArquivos.Tables[0].Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, dsArquivos.Tables[0], 0, "DESC", "false", "false"), true);
                    pnResultado.Visible = true;
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Nenhum XML Localizado");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.ToString());
            }
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlEmpresa, "sp_Select 'Flow_Empresa'", "idParceiro", "sDscEmpresa", false, "Todas as Empresas", "0");
            FUNCOES.Popula_Combo(ddlCliente, "sp_Select 'Flow_Clientes_Pedido', " + IDENTITY.Variaveis.idParceiro(), "idCliente", "sRazaoSocial", false, "Todos os Clientes", "0");
        }

        protected void cmdXMLDownload_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string arquivoId = btn.CommandArgument;
            string sNomeArquivo = "";
            byte[] bObjArquivo = null;

            Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
            DataTable dtArquivo;
            vParametrosItem = new Dictionary<string, string>
            {
                {"@sFuncao",                    "CONSULTAR_DETALHE" },
                {"@idArquivo",                  arquivoId}
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

                FUNCOES.DirecionaPagina_NovaAba(Page, "/Download/" + sNomeArquivo);
            }
            Pesquisar("");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar(txtPesquisa.Text);
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina("App/Paginas/Adm/Historico/ImportadorXML_Detalhe.aspx?Novo=S");
        }

        protected void btnPDF_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string arquivoId = btn.CommandArgument;
            string sNomeArquivo = "";
            byte[] bObjArquivo = null;

            Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
            DataTable dtArquivo;
            vParametrosItem = new Dictionary<string, string>
            {
                {"@sFuncao",                    "CONSULTAR_DETALHE" },
                {"@idArquivo",                  arquivoId}
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

                FUNCOES.DirecionaPagina_NovaAba(Page, "/Download/" + sNomeArquivo);
            }
            Pesquisar("");
        }
    }
}