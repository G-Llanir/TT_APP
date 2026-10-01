using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Text;
using System.Web.UI.WebControls;

namespace TT_Colaborador.Aplicativo.Paginas.EPI
{
    public partial class EntregaEPI : Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Entrega_EPI";

        #region | Funções Incialização do Form

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Entrega_de_EPI.Consultar, true, true);

            Pesquisar();

            if (IsPostBack)
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                if (requestTarget == "funcao_SAIR")
                    FUNCOES.DirecionaPagina("/Aplicativo/MenuColaborador.aspx");
                else if (requestTarget == "funcao_CONFIRMAR")
                    Confirmar();
            }

            RegistraScript();
        }

        #endregion

        #region | Metodos Banco de Dados

        protected void Pesquisar()
        {
            div_dados.Visible = false;
            PainelAtualizacao.Visible = false;
            div_entregas.Visible = true;
            lblTituloPagina.Text = "Entregas de EPI";

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idUsuarioLogado", IDENTITY.Variaveis.idUsuario() }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa))
            {
                rptEntregas.DataSource = dsPesquisa.Tables[0];
                rptEntregas.DataBind();
            }
            else
            {
                div_entregas.Visible = false;
                MensagemPagina_Entregas.MostraMensagem_Erro("Nenhum Registro Localizado!");
            }
        }

        void Pesquisar(string idEntregaEPI)
        {
            try
            {
                lblTituloPagina.Text = "Confirmar Entrega de EPI";

                LimpaCampos();

                if (idEntregaEPI != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>()
                    {
                        { "@idEntregaEPI", idEntregaEPI }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        div_dados.Visible = true;
                        div_entregas.Visible = false;
                        cmdConfirmar.Visible = FUNCOES.ValidaPermissao(Permissao.Entrega_de_EPI.Editar, false, true);
                        divFormArquivo.Visible = true;

                        hddidEntregaEPI.Value = RETORNO.DATASET(dsPesquisa, "idEntregaEPI");
                        txtStatus.Text = RETORNO.DATASET(dsPesquisa, "sDscStatus");
                        txtEntregaPontual.Text = RETORNO.DATASET(dsPesquisa, "sDscEntregaPontual");
                        txtObs.Text = RETORNO.DATASET(dsPesquisa, "sDscObservacao");

                        frmArquivos.Attributes.Add("src", $"~/Aplicativo/Paginas/Arquivos.aspx?idObjeto={hddidEntregaEPI.Value}&sTipoObjeto={"EntregaEPI"}");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                    }
                    else
                    {
                        MensagemPagina_Entregas.MostraMensagem_Erro("Erro: " + sErro);
                        PainelAtualizacao.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        void Confirmar()
        {
            try
            {
                string[] vidEntregaEPI = hddidEntregaEPI.Value.Split(',');
                string idEntregaEPI = vidEntregaEPI[0].ToString();

                Dictionary<string, string> vParametros = new Dictionary<string, string>()
                {
                    { "@sFuncao", "CONFIRMAR" },
                    { "@idEntregaEPI", idEntregaEPI },
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                };
                DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                Pesquisar();
                MensagemPagina_Entregas.MostraMensagem_Sucesso("Entrega de EPI confirmada com sucesso!");
            }
            catch (Exception ex)
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Houve um erro na tentativa de Confirmar a Entrega de EPI!<br />Erro ao Confirmar: " + ex.Message);
            }
        }

        #endregion

        #region | Limpar Campos

        void LimpaCampos()
        {
            txtStatus.Text = string.Empty;
            txtEntregaPontual.Text = string.Empty;
            txtObs.Text = string.Empty;
            PainelAtualizacao.Visible = false;
            cmdConfirmar.Text = "Confirmar";
        }

        #endregion

        #region | Eventos

        protected void fechar_click(object sender, EventArgs e) => Pesquisar();

        protected void rptEntregas_ItemCommand(object source, RepeaterCommandEventArgs e) => Pesquisar(e.CommandArgument.ToString());

        #endregion

        #region | Script

        void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("$(document).ready(function() {\r\n");

            sb.Append("     $('#dialog-Confirmar').dialog({\r\n");
            sb.Append("         resizable: false,\r\n");
            sb.Append("         height: 'auto',\r\n");
            sb.Append("         width: 400,\r\n");
            sb.Append("         modal: true,\r\n");
            sb.Append("         autoOpen: false,\r\n");
            sb.Append("         buttons: {\r\n");
            sb.Append("             'Sim': function() {\r\n");
            sb.Append("                 __doPostBack('funcao_CONFIRMAR', '');\r\n");
            sb.Append("                 $(this).dialog('close');\r\n");
            sb.Append("             },\r\n");
            sb.Append("             'Não': function() {\r\n");
            sb.Append("                 $(this).dialog('close');\r\n");
            sb.Append("             },\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n");
            sb.Append("     $('[id*=cmdConfirmar]').click(function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $('#dialog-Confirmar').dialog('open');\r\n");
            sb.Append("     });");

            sb.Append("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina_Modal", sb.ToString(), true);
        }

        #endregion
    }
}