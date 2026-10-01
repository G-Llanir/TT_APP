using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using BD = TT.FrameWork.BD;
using TT.FrameWork;

namespace TT_Hub.App
{

    
    public partial class Dashboard : System.Web.UI.Page
    {
        int TAB_Quadrados = 0;
        int TAB_UltimasOcorrencias = 1;
        int TAB_TIposDeEquipamento = 2;
        int TAB_ResumoEventos = 3;
        int TAB_NovoDashBoard = 4;
        int TAB_ClientesSelecao = 5;
        int TAB_MonitoramentoPING = 6;
     
        protected void Page_Load(object sender, EventArgs e)
        {
            //Funcoes.ValidaPermissao(1, true);

            if (!IsPostBack)
            {
                // BreadCrumb.NivelPagina = 0;
                // BreadCrumb.TitulodaPagina = "DashBoard";
                AtualizarDashBoard(IDENTITY.Variaveis.idUsuario(), IDENTITY.Variaveis.idCliente());
            }

        }

        void AtualizarDashBoard(string idUsuario, string idGabinete)
        {
            DataSet dsDashBoard;

            string sSql = "sp_HUB_DashBoard";

            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@idUsuario", idUsuario.ToString());
     
            dsDashBoard = BD.ExecutarDataSet(sSql, vParametros);
            if (BD.ValidarDataSet(dsDashBoard))
            {
                //Atualiza o Listbox de Seleção de Cliente
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                lstidCliente.Items.Clear();
                for (int i = 0; i < dsDashBoard.Tables[TAB_ClientesSelecao].Rows.Count; i++)
                {
                    lstidCliente.Items.Add(new System.Web.UI.WebControls.ListItem(BD.Retorno.DATASET(dsDashBoard, TAB_ClientesSelecao, i, "sDScCliente"), BD.Retorno.DATASET(dsDashBoard, TAB_ClientesSelecao, i, "idCliente")));
                    if (BD.Retorno.DATASET(dsDashBoard, TAB_ClientesSelecao, i, "sCheck") == "S")
                    {
                        lstidCliente.Items[i].Selected = true;
                    }
                }

                    


                //Tabela 1 - 
                //lblQuantidadeOK.Text        = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["QuantidadeOK"]);
                //lblQuantidadeAlarme.Text    = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["QuantidadedeAlarme"]);
                //lblQuantidadeFalhas.Text    = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["QuantidadedeFalhas"]);
                //lblQuantidadeDescarga.Text  = string.Format("{0:n0}", dsDashBoard.Tables[TAB_Quadrados].Rows[0]["QuantidadedeDescarga"]);


                //if (lblQuantidadeOK.Text != "0")
                //{
                //    lnkOK.NavigateUrl = sLinkOcorrencia + "OK";
                //}
                //if (lblQuantidadeAlarme.Text != "0")
                //{
                //    lnkAlarme.NavigateUrl = sLinkOcorrencia + "Alarme";
                //}
                //if (lblQuantidadeFalhas.Text != "0")
                //{
                //    lnkFalha.NavigateUrl = sLinkOcorrencia + "Falha";
                //}
                //if (lblQuantidadeDescarga.Text != "0")
                //{
                //    lnkDescarga.NavigateUrl = sLinkOcorrencia + "Descarga";
                //}


                rptBotoes.DataSource = dsDashBoard.Tables[TAB_NovoDashBoard];
                rptBotoes.DataBind();

                gvUltimasOcorrencias.DataSource = dsDashBoard.Tables[TAB_UltimasOcorrencias];
                gvUltimasOcorrencias.DataBind();

                gvMonitoramento.DataSource = dsDashBoard.Tables[TAB_MonitoramentoPING];
                gvMonitoramento.DataBind();

                div_Resumo.Attributes.CssStyle.Clear();

                if (dsDashBoard.Tables[TAB_MonitoramentoPING].Rows.Count == 0 )
                {
                    div_Resumo.Attributes.Add("class", "col-lg-12");
                    div_Monitoramento.Visible = false;
                }
                else
                {
                    div_Resumo.Attributes.Add("class", "col-lg-8");
                    div_Monitoramento.Visible = true;   
                }

                //Grafico 1 
                sb.Append("$(function() {");

                ////         sb.Append("Morris.Line({");
                ////         sb.Append("element: 'GraficoLigacoes',");
                ////         sb.Append("data: [{");

                ////         for (int nLinhasGraficos = 0; nLinhasGraficos < dsDashBoard.Tables[2].Rows.Count; nLinhasGraficos++)
                ////         {
                ////             sb.Append("Período: '" + dsDashBoard.Tables[2].Rows[nLinhasGraficos]["Data"].ToString() + "',");
                ////             sb.Append("Recebidas: " + dsDashBoard.Tables[2].Rows[nLinhasGraficos]["QtdLigacoesEntrada"].ToString() + ",");
                ////             sb.Append("Efetuadas: " + dsDashBoard.Tables[2].Rows[nLinhasGraficos]["QtdLigacoesEfetuadas"].ToString());

                ////             if (nLinhasGraficos < dsDashBoard.Tables[2].Rows.Count -1 )
                ////             { 
                ////                 sb.Append(" }, {");
                ////             }
                ////         }

                ////         sb.Append("}],");
                ////         sb.Append("xkey: 'Período',");
                ////         sb.Append("xlabel: 'hour',");
                ////         sb.Append("ykeys: ['Recebidas', 'Efetuadas'],");
                ////         sb.Append("labels: ['Ligações Recebidas', 'Ligações Efetuadas'],");
                ////         sb.Append("pointSize: 2,");
                ////         sb.Append("hideHover: 'auto',");
                ////         sb.Append("resize: true");
                ////         sb.Append("});");



                sb.Append("Morris.Bar({");
                sb.Append("element: 'GraficoEquipamentos',");
                sb.Append("data: [");

                for (int nLinhasGraficoDistribuicao = 0; nLinhasGraficoDistribuicao < dsDashBoard.Tables[TAB_TIposDeEquipamento].Rows.Count; nLinhasGraficoDistribuicao++)
                {
                    sb.Append("{Equipamento: '" + BD.Retorno.DATASET(dsDashBoard, TAB_TIposDeEquipamento, nLinhasGraficoDistribuicao, "sDscTipoEquipamento") + "',");
                    sb.Append("	Total: " + BD.Retorno.DATASET(dsDashBoard, TAB_TIposDeEquipamento, nLinhasGraficoDistribuicao, "nTotal") + "}");

                    if (nLinhasGraficoDistribuicao < dsDashBoard.Tables[TAB_TIposDeEquipamento].Rows.Count - 1)
                    {
                        sb.Append(", ");
                    }

                }
                sb.Append("],");
                sb.Append("xkey: 'Equipamento',");
                sb.Append("ykeys: ['Total'],");
                sb.Append("labels: ['Total'],");
                sb.Append("barRatio: 0.3,");
                sb.Append("xLabelAngle: 45,");
                //sb.Append("gridTextSize: 10, grid: true, axes: true , resize: false,stacked: true,");
                sb.Append("hideHover: 'auto'");

                sb.Append("});");





                sb.Append("Morris.Donut({");
                sb.Append("element: 'ResumoEventos',");
                sb.Append("data: [");

                for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[TAB_ResumoEventos].Rows.Count; nLinhasOrigem++)
                {
                    sb.Append("{label: '" + dsDashBoard.Tables[TAB_ResumoEventos].Rows[nLinhasOrigem]["sDscTipoStatus"].ToString() + "',");
                    sb.Append("	value: '" + dsDashBoard.Tables[TAB_ResumoEventos].Rows[nLinhasOrigem]["nTotal"].ToString() + "'}");
                    if (nLinhasOrigem < dsDashBoard.Tables[TAB_ResumoEventos].Rows.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append("],");
                sb.Append("resize: true");
                sb.Append("});");

                

                sb.Append(" });");

                
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AddShowModalScript", sb.ToString(), true);






                //sb.Append("Morris.Donut({");
                //sb.Append("element: 'Grafico_Origem',");
                //sb.Append("data: [");

                //for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[TAB_Origem_Cadastro].Rows.Count; nLinhasOrigem++)
                //{
                //    sb.Append("{label: '" + dsDashBoard.Tables[TAB_Origem_Cadastro].Rows[nLinhasOrigem]["sDscOrigemCadastro"].ToString() + "',");
                //    sb.Append("	value: '" + dsDashBoard.Tables[TAB_Origem_Cadastro].Rows[nLinhasOrigem]["Total"].ToString() + "'}");
                //    if (nLinhasOrigem < dsDashBoard.Tables[TAB_Origem_Cadastro].Rows.Count - 1)
                //    {
                //        sb.Append(", ");
                //    }
                //}
                //sb.Append("],");
                //sb.Append("resize: true");
                //sb.Append("});");




                ////Final do Gráfico


                //GVPendenteFB.DataSource = dsDashBoard.Tables[TAB_Cadastro_Facebook];
                //GVPendenteFB.DataBind();
            }
        }

        protected void btnTest0_Click(object sender, EventArgs e)
        {

        }


        protected void Button1_Click(object sender, EventArgs e)
        {
            
        }

        protected void gvUltimasOcorrencias_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();

                //string sHTMLProgress = "";
                //Literal _ltrprogress = new Literal();
                //_ltrprogress = (Literal)e.Row.FindControl("ltrprogress");
                //sHTMLProgress = "<div id=\"progress\" class=\"bar " + DataBinder.Eval(e.Row.DataItem, "sClasse").ToString() + "\"";
                //sHTMLProgress += " style=\"width: " + DataBinder.Eval(e.Row.DataItem, "sPercentualTempoDecorrido").ToString() + "\"></div>";
                //_ltrprogress.Text = sHTMLProgress;

                //LinkButton lb = e.Row.FindControl("lnkStatusChamado") as LinkButton;
                //if (lb != null) ScriptManager.GetCurrent(this).RegisterAsyncPostBackControl(lb);


            }
        }

        protected void timer_Atualizar_Tick(object sender, EventArgs e)
        {
            AtualizarDashBoard(IDENTITY.Variaveis.idUsuario(), IDENTITY.Variaveis.idCliente());
        }

        protected void lstidCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
        
        }

        protected void cmdAtualizar_Click(object sender, EventArgs e)
        {
            string sidCliente_HUB = "";
            string idUsuario = IDENTITY.Variaveis.idUsuario();
            
            foreach (ListItem item in lstidCliente.Items)
            {
                if (item.Selected)
                {
                    sidCliente_HUB += item.Value + "|";
                }
            }

            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "SALVAR_Usuarios_x_Clientes");
            vParametros.Add("@idUsuario", idUsuario);
            vParametros.Add("@sidCliente_HUB", sidCliente_HUB);

            BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", vParametros);
            AtualizarDashBoard(idUsuario, "0");

        }

        protected void gvMonitoramento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[3].Text != "0")
                {
                    e.Row.Cells[3].CssClass = "danger";
                    e.Row.Cells[1].CssClass = "warning";

                    // e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
                }
                else
                {
                    e.Row.Cells[2].CssClass = "success";
                    e.Row.Cells[1].CssClass = "success";
                    //e.Row.Cells[0].CssClass = "success";
                }
            }
        }
    }
}