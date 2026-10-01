using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static TT.FrameWork.BD;
using Identity = TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class Seguro : System.Web.UI.Page
    {
        string sTituloPagina = "Seguro";
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores";

        public List<cls_Seguro> bs_Seguro
        {
            get
            {
                if (ViewState["bs_Seguro"] == null)
                {
                    ViewState["bs_Seguro"] = new List<cls_Seguro>();
                }
                return (List<cls_Seguro>)ViewState["bs_Seguro"];
            }
            set
            {
                ViewState["bs_Seguro"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                manual.sNomeArquivo = "Manual_Seguro.pdf";

                Funcoes.ValidaPermissao(Permissao.RRHH.Seguro.Consultar, true);
                if (!Funcoes.ValidaPermissao(Permissao.RRHH.Seguro.Incluir))
                {
                    btnNovoSeguro.Visible = false;
                }

                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;

                AlterarVisualizacao_Edicao(false);
                
                Pesquisar();                
            }
        }

        private void Pesquisar()
        {           

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Consultar_Seguro" },
                { "@sPesquisa", txtPesquisa.Text.Trim() }
            };
            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros, false);            

            if (tb.Rows.Count > 0)
            {
                div_gvConsulta.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "asc"), true);
            }
            else
            {
                div_gvConsulta.Visible = false;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado");
            }
        }

        private void AlterarVisualizacao_Edicao(bool bHabilitar)
        {
            pnSeguroConsulta.Visible = !bHabilitar;
            pnSeguroDetalhe.Visible = bHabilitar;

            if (hddidSeguro.Value != "0")
                div_IncluirPlanosSeguro.Visible = bHabilitar;
            else
                div_IncluirPlanosSeguro.Visible = !bHabilitar;
        }

        private void SeguroDetalhe(string id)
        {
            try
            {
                DataSet ds;
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE_SEGURO" },
                    { "@idSeguro", id }
                };
                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out string sErro))
                {
                    txtIdSeguro.Text = id;
                    ddlsParceiroSeguro.SelectedValue = Retorno.DATASET(ds, 0, "idParceiro");
                    txtsCNPJ.Text = Retorno.DATASET(ds, 0, "sCNPJ");
                    txtsEndereco.Text = Retorno.DATASET(ds, 0, "sEndereco");
                    txtnApoliceVG.Text = Retorno.DATASET(ds, 0, "nApoliceVG");
                    txtnApoliceAPC.Text = Retorno.DATASET(ds, 0, "nApoliceAPC");
                    txtsTelefoneApoio.Text= Retorno.DATASET(ds, 0, "sTelefoneApoio");

                    AlterarVisualizacao_Edicao(true);

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        bs_Seguro.Clear();
                        foreach (DataRow row in ds.Tables[1].Rows)
                        {
                            cls_Seguro objItem = new cls_Seguro();
                            objItem.sFuncao = "Sem_Alteracao";
                            objItem.idPlanoSeguro= Convert.ToInt32(row["idPlanoSeguro"]);
                            objItem.sTipoPlano = row["sTipoPlano"].ToString();
                            objItem.nValorNatural = Convert.ToDecimal(row["nValorNatural"]);
                            objItem.nValorAcidental = Convert.ToDecimal(row["nValorAcidental"]);
                            objItem.nValorInvalidez = Convert.ToDecimal(row["nValorInvalidez"]);
                            objItem.nVG = Convert.ToDecimal(row["nVG"]);
                            objItem.nAPC = Convert.ToDecimal(row["nAPC"]);
                            objItem.sTemRelatorio = row["sTemRelatorio"].ToString();

                            bs_Seguro.Add(objItem);
                        }

                        gvPlanoSeguro.DataSource = bs_Seguro;
                        gvPlanoSeguro.DataBind();
                    }

                    if (!Funcoes.ValidaPermissao(Permissao.RRHH.Seguro.Editar))
                    {
                        btnSalvar.Visible = false;
                    }
                }
                else
                {
                    throw new Exception("BD: " + sErro.ToString());
                }
            }
            catch (Exception e)
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro(e.Message);
            }

            RegistraScript();
        }

        private void SalvarPlanoSeguro()
        {
            foreach (var item in bs_Seguro)
            {
                if (item.sFuncao != "Sem_Alteracao")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", item.sFuncao },
                        { "@idPlanoSeguro", item.idPlanoSeguro.ToString() },
                        { "@idSeguro", hddidSeguro.Value },
                        { "@sTipoPlano", item.sTipoPlano },
                        { "@nValorNatural", item.nValorNatural.ToString().Replace(",",".") },
                        { "@nValorAcidental", item.nValorAcidental.ToString().Replace(",",".") },
                        { "@nValorInvalidez", item.nValorInvalidez.ToString().Replace(",",".") },
                        { "@nVG", item.nVG.ToString().Replace(",",".") },
                        { "@nAPC", item.nAPC.ToString().Replace(",",".") },
                        { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                    };
                    DataTable dt = BD.ExecutarDataTable(sProcedure, vParametros);
                    
                }
            }
        }

        private void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("     $('[id*=txtsCNPJ]').mask('00.000.000/0000-00');");
            sb.AppendLine("     $('[id*=txtnCredito]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnValorNatural]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnValorAcidental]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnValorInvalidez]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnVGSeguro]').mask('000.000.000.000.000,00000', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnAPCSeguro]').mask('000.000.000.000.000,00000', { reverse: true });");
            sb.AppendLine("     $('[id*=txtsTelefoneApoio]').mask('(00) 0000-00009', {");
            sb.AppendLine("         placeholder: '(00) 0000-0000',");
            sb.AppendLine("         clearIfNotMatch: false,");
            sb.AppendLine("         onKeyPress: function (value, e, field, options) {");
            sb.AppendLine("             if (value.replace(/\\D/g, '').length === 11) {");
            sb.AppendLine("                 field.mask('(00) 00000-0000', options);");
            sb.AppendLine("             } else {");
            sb.AppendLine("                 field.mask('(00) 0000-00009', options);");
            sb.AppendLine("             }");
            sb.AppendLine("         }");
            sb.AppendLine("    });");
            sb.AppendLine("});");


            sb.AppendLine("</script>");


            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptExtrato", sb.ToString(), false);
        }

        private void LimpaCampos(int nTipo)
        {
            if (nTipo == 1)
            {
                txtIdSeguro.Text = "";
                ddlsParceiroSeguro.SelectedValue = "0";
                txtsEndereco.Text = "";
                txtsCNPJ.Text = "";
                txtsTelefoneApoio.Text = "";
                txtnApoliceVG.Text = "";
                txtnApoliceAPC.Text = "";
            }
            else
            {
                txtsTipoPlano.Text = "";
                txtnValorNatural.Text = "";
                txtnValorAcidental.Text = "";
                txtnValorInvalidez.Text = "";
                txtnVGSeguro.Text = "";
                txtnAPCSeguro.Text = "";
            }
        }

        private bool ValidarDados(int nTipo)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (nTipo == 1)
            {
                if (ddlsParceiroSeguro.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Parceiro";
                }

                if (txtnApoliceVG.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Número da Apólice V.G.";
                }
                
                if (txtnApoliceAPC.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Número da Apólice APC";
                }

                if (txtsTelefoneApoio.Text.Length < 8)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Telefone de Apoio da seguradora";
                }            
            }
            else
            {
                if (txtsTipoPlano.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Tipo de Plano";
                }

                if (txtnValorNatural.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Valor Natural";
                }
                
                if (txtnValorAcidental.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Valor Acidental";
                }
                
                if (txtnValorInvalidez.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Valor Invalidez";
                } 
                
                if (txtnVGSeguro.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Valor V.G.";
                } 
                
                if (txtnAPCSeguro.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Valor APC";
                } 
                
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaDetalhe.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        private void PopulaCombo()
        {
            Funcoes.Popula_Combo(ddlsParceiroSeguro, "sp_Select 'Flow_Clientes'", "idCliente", "sRazaoSocial", false, "Selecione o Parceiro", "0");
        }

        protected void lbSeguroDetalhe_Command(object sender, CommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            hddidSeguro.Value = id;
            PopulaCombo();
            SeguroDetalhe(id);
        }

        protected void btnNovoSeguro_Click(object sender, EventArgs e)
        {
            hddidSeguro.Value = "0";
            AlterarVisualizacao_Edicao(true);
            LimpaCampos(1);
            PopulaCombo();
            txtIdSeguro.Text = "Novo";
            div_dadosSeguro.Visible = false;
            btnSalvar.Visible = false;
        }

        protected void ddlsSeguroParceiro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlsParceiroSeguro.SelectedValue != "0")
            {
                div_dadosSeguro.Visible = true;
                div_IncluirPlanosSeguro.Visible = true;

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "Consultar_Parceiro_Seguro" },                         
                        { "@idParceiro", ddlsParceiroSeguro.SelectedValue }
                    };
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out string sErro))
                {
                    txtsCNPJ.Text = Retorno.DATASET(ds, 0, "sCNPJ");
                    txtsEndereco.Text = Retorno.DATASET(ds, 0, "sEndereco");
                }

                txtIdSeguro.Text = "Novo";
                btnSalvar.Visible = true;
                RegistraScript();
            }
            else
            {
                txtIdSeguro.Text = "Novo";
                div_dadosSeguro.Visible = false;
                div_IncluirPlanosSeguro.Visible = false;
                btnSalvar.Visible = false;
            }
        }

        protected void btnIncluirPlano_Click(object sender, EventArgs e)
        {
            if (ddlsParceiroSeguro.SelectedValue != "0")
            {                
                if (ValidarDados(2))
                {
                    if (hddidPlanoSeguro.Value == "0")
                    {
                        cls_Seguro objItem = new cls_Seguro
                        {
                            sFuncao = "Salvar_Seguro_Plano",
                            idPlanoSeguro = 0,
                            sTipoPlano = txtsTipoPlano.Text,
                            nValorNatural = Convert.ToDecimal(txtnValorNatural.Text.Replace(".","")),
                            nValorAcidental = Convert.ToDecimal(txtnValorAcidental.Text.Replace(".","")),
                            nValorInvalidez = Convert.ToDecimal(txtnValorInvalidez.Text.Replace(".","")),
                            nVG = Convert.ToDecimal(txtnVGSeguro.Text.Replace(".","")),
                            nAPC = Convert.ToDecimal(txtnAPCSeguro.Text.Replace(".",""))
                        };

                        bs_Seguro.Add(objItem);
                    }
                    else
                    {
                        var item = bs_Seguro.FirstOrDefault(x => x.idPlanoSeguro.ToString() == hddidPlanoSeguro.Value);
                        item.sFuncao = "Editar_Seguro_Plano";
                        item.sTipoPlano = txtsTipoPlano.Text;
                        item.nValorNatural = Convert.ToDecimal(txtnValorNatural.Text);
                        item.nValorAcidental = Convert.ToDecimal(txtnValorAcidental.Text);
                        item.nValorInvalidez = Convert.ToDecimal(txtnValorInvalidez.Text);
                        item.nVG = Convert.ToDecimal(txtnVGSeguro.Text);
                        item.nAPC = Convert.ToDecimal(txtnAPCSeguro.Text);

                        hddidPlanoSeguro.Value = "0";
                        btnIncluirPlano.Text = "Incluir";
                    }

                    gvPlanoSeguro.DataSource = bs_Seguro.Where(x => x.sFuncao.ToString() != "Excluir_Plano_Seguro");
                    gvPlanoSeguro.DataBind();

                    LimpaCampos(2);
                }           

            }
            else
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro("Selecione um Parceiro");
            }

            RegistraScript();
        }        

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados(1))
            {
                try
                {
                    DataSet ds;
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "Salvar_Seguro" },
                        { "@idSeguro", hddidSeguro.Value},
                        { "@idParceiro", ddlsParceiroSeguro.SelectedValue },
                        { "@nApoliceVG", txtnApoliceVG.Text },
                        { "@nApoliceAPC", txtnApoliceAPC.Text },
                        { "@sTelefoneApoio", txtsTelefoneApoio.Text },                        
                        { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                    };
                    ds = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(ds, out string sErro))
                    {
                        hddidSeguro.Value = Retorno.DATASET(ds, 0, "idSeguro");

                        if (gvPlanoSeguro.Rows.Count > 0)
                        {
                            SalvarPlanoSeguro();
                        }

                        MensagemPaginaDetalhe.MostraMensagem_Sucesso("Seguro gravado com sucesso");
                        SeguroDetalhe(hddidSeguro.Value);
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPaginaDetalhe.MostraMensagem_Erro(ex.Message);
                }
            }
            else
            {
                RegistraScript();
            }
        }        

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            AlterarVisualizacao_Edicao(false);
            Pesquisar();
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void gvPlanoSeguro_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Editar")
                {
                    int idPlanoSeguro = Convert.ToInt32(e.CommandArgument);                    
                    var item = bs_Seguro.FirstOrDefault(x => x.idPlanoSeguro == idPlanoSeguro);

                    if (item != null)
                    {   
                        txtsTipoPlano.Text = item.sTipoPlano;
                        txtnValorNatural.Text = item.nValorNatural.ToString("N2") ;
                        txtnValorAcidental.Text = item.nValorAcidental.ToString("N2");
                        txtnValorInvalidez.Text = item.nValorInvalidez.ToString("N2");
                        txtnVGSeguro.Text = item.nVG.ToString("N5");
                        txtnAPCSeguro.Text = item.nAPC.ToString("N5");

                        hddidPlanoSeguro.Value = idPlanoSeguro.ToString();

                        btnIncluirPlano.Text = "Salvar";
                        //bs_Seguro.Remove(item);                        
                    }
                    else
                        throw new Exception("Plano não encontrado!");
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro("Houve um erro na tentativa de Editar!<br />Erro ao Editar: " + ex.Message, true);
            }

            gvPlanoSeguro.DataSource = bs_Seguro.Where(x => x.sFuncao.ToString() != "Excluir_Plano_Seguro");
            gvPlanoSeguro.DataBind();

            RegistraScript();
        }

        protected void gvPlanoSeguro_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
                string sTemRelatorio = gvPlanoSeguro.DataKeys[e.Row.RowIndex]["sTemRelatorio"].ToString();
                LinkButton lnkExcluir = (LinkButton)e.Row.FindControl("lnkExcluir");

                if (sTemRelatorio == "S")
                    lnkExcluir.Visible = false;
                
            }
        }

        protected void gvPlanoSeguro_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idPlanoSeguro = Convert.ToInt32(gvPlanoSeguro.DataKeys[e.RowIndex].Value);

            var planoSeguro = bs_Seguro.FirstOrDefault(x => x.idPlanoSeguro == idPlanoSeguro);
            if (planoSeguro != null)
            {
                planoSeguro.sFuncao = "Excluir_Plano_Seguro";
                gvPlanoSeguro.DataSource = bs_Seguro.Where(x => x.sFuncao.ToString() != "Excluir_Plano_Seguro");
                gvPlanoSeguro.DataBind();
            }
        }
    }

    [Serializable]
    public class cls_Seguro
    {
        public string sFuncao { get; set; }
        public int idPlanoSeguro { get; set; }
        public string sTipoPlano { get; set; }
        public decimal nValorNatural { get; set; }
        public decimal nValorAcidental { get; set; }
        public decimal nValorInvalidez { get; set; }
        public decimal nVG { get; set; }
        public decimal nAPC { get; set; }
        public string sTemRelatorio { get; set; }
    }
}