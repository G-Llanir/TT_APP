using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using static TT.FrameWork.BD;
using Identity = TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Adm.Fiscal
{
    public partial class ServicoMunicipal : Page
    {
        string sTituloPagina = "Serviços Municipal";
        string sProcedure = "sp_Manipula_tbl_Flow_Servico_Municipal";

        public List<cls_Servicos> bs_Servico
        {
            get
            {
                if (ViewState["bs_Servico"] == null) ViewState["bs_Servico"] = new List<cls_Servicos>();
                return (List<cls_Servicos>)ViewState["bs_Servico"];
            }
            set
            {
                ViewState["bs_Servico"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Funcoes.ValidaPermissao(Permissao.Manutencao.ServicoMunicipal.Consultar, true);

                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;

                pnServicoConsulta.Visible = true;
                pnServicoDetalhe.Visible = false;
                div_IncluirServicos.Visible = false;

                Pesquisar();
                sAtivo.Definir("S", "Ativo", "");
            }           
        }

        private void Pesquisar()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sPesquisa", txtPesquisa.Text.Trim() },
                { "@sAtivo", ddlsAtivoo.SelectedValue }
            };
            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros);

            if (tb.Rows.Count > 0)
            {
                div_gvConsulta.Visible = true;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, 0, new int[1] { 6 }, "asc", "false", "''"), true);
            }
            else div_gvConsulta.Visible = false;
        }

        private void CodigoServicoDetalhe(string id)
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idServico", id }
                };
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(ds, out string sErro))
                {
                    txtidServico.Text = id;
                    txtsCodigo.Text = Retorno.DATASET(ds, 0, "sCodigo");
                    txtsDscServico.Text = Retorno.DATASET(ds, 0, "sDscServico");
                    txtnISS.Text = Retorno.DATASET(ds, 0, "nISS");
                    ddlidServicoFederal.SelectedValue = Retorno.DATASET(ds, 0, "idServicoFederal");
                    sAtivo.Definir(Retorno.DATASET(ds, 0, "sAtivo"), "Ativo", "");
                    pnServicoConsulta.Visible = false;
                    pnServicoDetalhe.Visible = true;
                    div_IncluirServicos.Visible = true;

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        bs_Servico.Clear();
                        foreach (DataRow row in ds.Tables[1].Rows)
                        {
                            cls_Servicos objItem = new cls_Servicos();
                            objItem.idServico = Convert.ToInt32(row["idServico"]);
                            objItem.sDscServico = row["sDscServico"].ToString();
                            objItem.sFuncao = "Sem_Alteracao";

                            bs_Servico.Add(objItem);
                        }

                        gvServicoVinculado.DataSource = bs_Servico;
                        gvServicoVinculado.DataBind();
                    }
                }
                else throw new Exception("BD: " + sErro.ToString());
            }
            catch (Exception e)
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro(e.Message);
            }

            RegistraScript();
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void btnNovoCodServico_Click(object sender, EventArgs e)
        {
            pnServicoConsulta.Visible = false;
            pnServicoDetalhe.Visible = true;
            div_IncluirServicos.Visible = false;

            LimpaCampos();
            PopulaCombo();

            txtidServico.Text = "Novo";
            txtidServico.ReadOnly = true;
            btnSalvar.Visible = true;
            hddidCodigoServico.Value = "0";
            RegistraScript();
        }

        protected void lbDetalheServico_Command(object sender, CommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            hddidCodigoServico.Value = id;
            PopulaCombo();
            CodigoServicoDetalhe(id);
        }

        private void LimpaCampos()
        {
            ddlidServicoFederal.SelectedValue = "0";
            txtsCodigo.Text = "";
            txtsDscServico.Text = "";
            txtnISS.Text = "0,00";
        }

        private void PopulaCombo()
        {
            Funcoes.Popula_Combo(ddlidServicoFederal, "sp_Select 'tbl_Flow_Codigo_Servico'", "idCodigoServico", "sDscServico", false, "Selecione o Serviço Federal", "0");
            Funcoes.Popula_Combo(ddlsServico, "sp_Select 'Flow_Consulta_Sevicos'", "idServico", "sDscServico", false, "Selecione o Serviço", "0");            
        }

        private void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("if ($('[id*=txtnISS]').length) $('[id*=txtnISS]').mask('009,99', { reverse: true });");
            sb.Append("if ($('[id*=txtsCodigo]').length) $('[id*=txtsCodigo]').mask('0099999999', { reverse: true });");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "scriptCodigoServicoFilho", sb.ToString(), true);
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                try
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@sCodigo", txtsCodigo.Text },
                        { "@sDscServico", txtsDscServico.Text },
                        { "@nISS", txtnISS.Text.Replace(",",".") },
                        { "@idServicoFederal", ddlidServicoFederal.SelectedValue },
                        { "@idServico", hddidCodigoServico.Value },
                        { "@sAtivo", sAtivo.Recuperar() },
                        { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                    };
                    DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(ds, out string sErro))
                    {                        
                        hddidCodigoServico.Value = Retorno.DATASET(ds, 0, "idServico");

                        if (gvServicoVinculado.Rows.Count > 0) SalvarServicosVinculados();

                        MensagemPaginaDetalhe.MostraMensagem_Sucesso("Serviço Municipal gravado com sucesso");
                        CodigoServicoDetalhe(hddidCodigoServico.Value);                        
                    }
                    else throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPaginaDetalhe.MostraMensagem_Erro(ex.Message);
                }
            }
            else RegistraScript();
        }

        private void SalvarServicosVinculados()
        {
            foreach (var item in bs_Servico)
            {
                if (item.sFuncao != "Sem_Alteracao")
                {
                    Dictionary<string, string> vParametrosDocumentacao = new Dictionary<string, string>
                    {
                        { "@sFuncao", item.sFuncao },
                        { "@idItem", item.idServico.ToString() },
                        { "@idServico", hddidCodigoServico.Value },
                        { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                    };
                    BD.ExecutarDataSet(sProcedure, vParametrosDocumentacao);
                }
            }
        }

        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlidServicoFederal.SelectedValue == "0") sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Serviço Federal";
            if (txtsCodigo.Text == "") sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Descreva o Código do Serviço";
            if (txtsDscServico.Text.Length < 5) sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Descrição do Serviço deve ter ao menos 5 caracteres";
            if (txtnISS.Text == "") sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O campo de ISS não pode ficar vazio! Insira um valor ou coloque 0,00";

            if (sMensagemErro != "") 
            {
                bRetorno = false;
                MensagemPaginaDetalhe.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            pnServicoConsulta.Visible = true;
            pnServicoDetalhe.Visible = false;
            Pesquisar();
        }

        protected void btnIncluirServico_Click(object sender, EventArgs e)
        {
            if (ddlsServico.SelectedValue != "0")
            {
                if(bs_Servico.Count == 0 || bs_Servico.Any(x => x.idServico.ToString() != ddlsServico.SelectedValue))
                {
                    cls_Servicos objItem = new cls_Servicos
                    {
                        idServico = Convert.ToInt32(ddlsServico.SelectedValue),
                        sDscServico = ddlsServico.SelectedItem.ToString(),
                        sFuncao = "Salvar_Servicos_Vinculados"
                    };

                    bs_Servico.Add(objItem);

                    gvServicoVinculado.DataSource = bs_Servico.Where(x => x.sFuncao.ToString() != "Excluir_Servico_Vinculado");
                    gvServicoVinculado.DataBind();                    
                }
                else MensagemPaginaDetalhe.MostraMensagem_Erro("Serviço selecionado já consta na lista");

                ddlsServico.SelectedValue = "0";
            }
            else MensagemPaginaDetalhe.MostraMensagem_Erro("Selecione um Serviço para vincular a este Serviço Municipal");
        }

        protected void gvServicoVinculado_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idServico = Convert.ToInt32(gvServicoVinculado.DataKeys[e.RowIndex].Value);

            var servico = bs_Servico.FirstOrDefault(x => x.idServico == idServico);
            if (servico != null)
            {
                servico.sFuncao = "Excluir_Servico_Vinculado";
                gvServicoVinculado.DataSource = bs_Servico.Where(x => x.sFuncao.ToString() != "Excluir_Servico_Vinculado");
                gvServicoVinculado.DataBind();
            }

        }
    }

    [Serializable]
    public class cls_Servicos
    {
        public string sFuncao{ get; set; }
        public int idServico { get; set; }
        public string sDscServico { get; set; }

    }
}