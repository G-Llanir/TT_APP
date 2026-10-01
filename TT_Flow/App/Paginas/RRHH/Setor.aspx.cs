using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using IDENTITY = TT.FrameWork.Identity;
using TT.FrameWork;
using System.Web.UI.WebControls;
using static TT.FrameWork.BD;
using static Permissao;
using TT_Flow.FrameWork;
using System.Linq;

namespace TT_Flow.App.Paginas.RRHH
{



    public partial class Setor : Page
    {
        string sTituloPagina = "Setor";
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Setor";



        #region | Contrutores

        public List<cls_Setor> lstSetor
        {
            get
            {
                if (ViewState["lstSetor"] == null)
                {
                    ViewState["lstSetor"] = new List<cls_Setor>();
                }
                return (List<cls_Setor>)ViewState["lstSetor"];
            }
            set
            {
                ViewState["cls_Setor"] = value;
            }
        }
        #endregion


        #region | Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.Setor.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Setor.Incluir);
            cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Setor.Incluir);

            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                Pesquisar();
                txtPesquisa.Focus();
            }


            RegistraScript("");

            var requestTarget = this.Request["__EVENTTARGET"];
            var requestArgs = this.Request["__EVENTARGUMENT"];


            if (requestTarget == "funcao_SALVAR")
            {
                Salvar();
            }

        }
        #endregion

        #region | Eventos
        protected void cmdPesquisar_Click(object sender, EventArgs e)
         => Pesquisar();
        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            LimparCampos();
            AbrirModal_Click();
            FUNCOES.Scripts.FocusScript(Page, txtsDscSetor.ClientID);
        }

       
        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }
        #endregion

        #region | Funções de Pesquisar
        protected void Pesquisar()
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscSetor", txtPesquisa.Text.Trim() }
            };

         
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Setor", vParametros, false);
            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 0, new int[1] { 4 }, "asc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");

        }

        protected void Salvar()
        {
            gvSetor_SalvarGRID();

            if (ValidarDados())
            {
                try
                {
                    string[] viSetor = hddidSetor.Value.Split(',');
                    string idSetor = viSetor[0].ToString();



                    cls_Setor Principal = new cls_Setor();
                    Principal.idSetor = Convert.ToInt32(idSetor);
                    Principal.sDscSetor = txtsDscSetor.Text;
                    Principal.sFuncao = "SALVAR";
                    idSetor = Salvar_BD(Principal, "0");

                    foreach (cls_Setor Secundario in lstSetor)
                    {
                        Salvar_BD(Secundario, idSetor);
                    }

                    Pesquisar_Setor(idSetor);
                    Pesquisar();
                    MensagemPaginaModalDetalhe.MostraMensagem_Sucesso("Setor salvo com sucesso!");

                }
                catch (Exception ex)
                {

                }

            }


        }

        private string Salvar_BD(cls_Setor DADOS, string idSetorPai)
        {
            string sidSetor = "0";
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", DADOS.sFuncao },
                { "@idSetor", DADOS.idSetor.ToString()},
                { "@idEmpresa", DADOS.idEmpresa.ToString()},
                { "@nOrdem", DADOS.nOrdem.ToString() },
                { "@sDscSetor", DADOS.sDscSetor },
                { "@idSetorPai", idSetorPai },
                { "@idUsusarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };

            DataSet dsBD = ExecutarDataSet(sProcedure, vParametros);

            if (ValidarDataSet(dsBD))
            {
                sidSetor = BD.Retorno.DATASET(dsBD, "idSetor");
            }

            return sidSetor;

        }
        private bool ValidarDados()
        {
            string sMensagemErro = "";
            bool bRetorno = true;


            if (Validacoes.ValidarTexto(txtsDscSetor))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um nome válido para o Setor";
            }

            if (lstSetor.Count > 0)
            {
                foreach (var Linha in lstSetor)
                {
                    if (Linha.sFuncao != "EXCLUIR")
                    {
                        if (Linha.sDscSetor == "")
                        {
                            sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um nome válido para o Setor Secundário";
                            break;
                        }
                    }

                }

            }
             


            if (sMensagemErro != "")
            {
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                bRetorno = false;
            }

            return bRetorno;

        }
        void Pesquisar_Setor(string idSetor)
        {
            LimparCampos();
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idSetor", idSetor }
            };

            DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Setor", vParametros);

            txtidSetor.Text = BD.Retorno.DATASET(dsPesquisa, "idSetor");
            hddidSetor.Value = BD.Retorno.DATASET(dsPesquisa, "idSetor");
            txtsDscSetor.Text = BD.Retorno.DATASET(dsPesquisa, "sDscSetor");
            lbltituloModal.Text = txtsDscSetor.Text;
            PainelAtualizacao.Visible = true;
            PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));
            PopularLista_Setor_Filho(dsPesquisa);

            AbrirModal_Click();
            FUNCOES.Scripts.FocusScript(Page, txtsDscSetor.ClientID);

        }

        protected void dtgvConsulta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {

                string idSetor = dtgvConsulta.DataKeys[Convert.ToInt32(e.CommandArgument.ToString())].Value.ToString();
                Pesquisar_Setor(idSetor);
            }
            catch (Exception ex)
            {

                MensagemPagina.MostraMensagem_Erro("Erro ao exibir dados: " + ex.Message.ToString());
            }
        }

        void PopularLista_Setor_Filho(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[1].Rows)
            {
                lstSetor_Adicionar(
                                  Convert.ToInt32(row["idSetor"].ToString())
                                 , Convert.ToInt32(row["nOrdem"].ToString())
                                 , Convert.ToInt32(RETORNO.DATASET(dsPesquisa, "idSetorPai"))
                                 , Convert.ToInt32(row["idUsuarioAtualizacao"].ToString())
                                 , row["sDscSetor"].ToString()
                                 , "CONSULTA_DETALHE"
                    );

            }
            gvSetor_Popular();
        }

        void lstSetor_Adicionar(int idSetor, int nOrdem, int idSetorPai, int idUsuarioAtualizacao, string sDscSetor, string sFuncao)
        {
            if (nOrdem == 0)
            {
                if (lstSetor.Count > 0)
                {
                    nOrdem = lstSetor.Where(d => d.nOrdem != 998 && d.nOrdem != 999).Max(d => d.nOrdem) + 1;
                }
                else
                {
                    nOrdem = 1;
                }
            }


            cls_Setor lstAdicionar = new cls_Setor();
            lstAdicionar.idRegistro = lstSetor.Count() + 1; 
            lstAdicionar.idSetor = idSetor;
            lstAdicionar.nOrdem = nOrdem;
            lstAdicionar.idSetorPai = idSetorPai;
            lstAdicionar.idUsuarioAtualizacao = idUsuarioAtualizacao;
            lstAdicionar.sDscSetor = sDscSetor;
            lstAdicionar.sFuncao = sFuncao;
            lstSetor.Add(lstAdicionar);
        }
        
        void gvSetor_Popular()
        {
            gvSetor.DataSource = lstSetor.Where(c => c.sFuncao.ToString() != "EXCLUIR").OrderBy(x => x.nOrdem).ToList();
            gvSetor.DataBind();
        }

        void gvSetor_SalvarGRID()
        {
            foreach (GridViewRow item in gvSetor.Rows)
            {
                if (item.RowType == DataControlRowType.DataRow)
                {
                    int I = lstSetor.FindIndex(x => x.idRegistro.Equals(Convert.ToInt32(gvSetor.DataKeys[item.RowIndex]["idRegistro"].ToString())));

                    int nOrdem = Convert.ToInt32((item.FindControl("txtnOrdem") as TextBox).Text);
                    if (lstSetor[I].nOrdem != nOrdem)
                    {
                        lstSetor[I].nOrdem = nOrdem;
                        lstSetor[I].sFuncao = "SALVAR";
                    }

                    string sDscSetor = (item.FindControl("txtsDscSetor") as TextBox).Text;

                    if (lstSetor[I].sDscSetor != sDscSetor)
                    {
                        lstSetor[I].sDscSetor = sDscSetor;
                        lstSetor[I].sFuncao = "SALVAR";
                    }
                }
            }
        }

        void LimparCampos()
        {
            txtidSetor.Text = "Novo";
            hddidSetor.Value = "0";
            txtsDscSetor.Text = "";
            lbltituloModal.Text = "Novo Setor";
            PainelAtualizacao.Visible = false;
            lstSetor.Clear();
            gvSetor_Popular();

        }

        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$v192(function() {");

            sb.Append("$v192(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("zIndex: 10000,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$('[id*=txtnOrdem]').mask('0000', { reverse: false });");

            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }

        #endregion

        protected void AbrirModal_Click()
        {
            FecharModal("modalForm");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalForm').modal('show');", true);
        }
        protected void FecharModal(string modalId)
        {
            string script = $@"
         $('#{modalId}').modal('hide');
         $('.modal-backdrop').remove();
         $('body').removeClass('modal-open');
        ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal_" + modalId, script, true);
        }

        protected void cmdIncluirSetor_Click(object sender, EventArgs e)
        {
            gvSetor_SalvarGRID();
            lstSetor_Adicionar(0, 0, 0, 0, "", "SALVAR");
            gvSetor_Popular();
        }

        protected void gvSetor_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            gvSetor_SalvarGRID();
            int index = lstSetor.FindIndex(x => x.idRegistro.Equals(Convert.ToInt32(gvSetor.DataKeys[e.RowIndex]["idRegistro"].ToString())));
            lstSetor[index].sFuncao = "EXCLUIR";
            gvSetor_Popular();
        }

        protected void gvSetor_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {

        }

       
    }
}