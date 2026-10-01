using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using TT.FrameWork;
using System.Web.UI.WebControls;
using System.IO;

namespace TT_Flow.App.Paginas.Adm.Manutencao
{
    public partial class Pesquisa_NFe : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Administracao.NFe.Consultar, true);

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlTipoNFe, "sp_Select 'tbl_Flow_XML_NFe_TipoObjeto'", "idTipoObjeto", "sDscTipoObjeto", false, "Selecione um Tipo", "0");
                Pesquisar();
            }
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sPesquisa", txtPesquisa.Text },
                { "@idTipo", ddlTipoNFe.SelectedValue },
                { "@dtInicial", txtDataInicial.Text },
                { "@dtFinal", txtDataFinal.Text }
            };
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametros);

            if (BD.ValidarDataSet(ds))
            {
                pnResultado.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, ds.Tables[0], 1, "desc", "false", "''"), true);
            }
            else
                MensagemPagina.MostraMensagem_Erro("Não foram encontrados registros de NF-e!");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();

        protected void btnXML_Click(object sender, EventArgs e)
        =>
            BaixaXML(sender);

        void BaixaXML(object sender)
        {
            var row = (sender as LinkButton).NamingContainer as GridViewRow;

            Dictionary<string, string> vParametrosXML = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idXML", row.Cells[0].Text }
            };
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosXML);

            if (BD.ValidarDataSet(ds))
            {
                try
                {
                    string nomeArquivo = string.Format("{0}.xml", RETORNO.DATASET(ds, 0, "chNFe"));

                    File.WriteAllText(Server.MapPath("~/Download/" + nomeArquivo), RETORNO.DATASET(ds, 0, "sXML"));

                    FUNCOES.DownloadArquivo(Page, nomeArquivo);
                }
                catch
                {
                    MensagemPagina.MostraMensagem_Erro("Houve um erro no documento XML!");
                }
            }
            else
                MensagemPagina.MostraMensagem_Erro("Não foi encontrado documento XML!");

            Pesquisar();
        }
    }
}