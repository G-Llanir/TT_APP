using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Grid;
using static TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Adm.Fiscal
{
    public partial class RegrasICMS : Page
    {
        #region | Propriedades

        string sTituloPagina = "Regras ICMS";
        string sProcedure = "sp_Manipula_tbl_Flow_Fiscal_Regras";

        public List<cls_RegrasICMS> bs_RegrasICMS
        {
            get
            {
                if (ViewState["bs_RegrasICMS"] == null)
                    ViewState["bs_RegrasICMS"] = new List<cls_RegrasICMS>();
                return (List<cls_RegrasICMS>)ViewState["bs_RegrasICMS"];
            }
            set => ViewState["bs_RegrasICMS"] = value;
        }

        [Serializable]
        public class cls_RegrasICMS
        {
            public int idLinha { get; set; }
            public int idRegraICMS { get; set; }
            public string sUFOrigem { get; set; }
            public string sUFDestino { get; set; }
            public string sDscUFOrigem { get; set; }
            public string sDscUFDestino { get; set; }
            public decimal nICMS { get; set; }
            public decimal nICMS_DIFAL { get; set; }
            public decimal nICMS_Nacional { get; set; }
            public decimal nICMS_Importado { get; set; }
            public decimal nIBS_UF { get; set; }
            public decimal nRedBC { get; set; }
        }

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidaPermissao(Permissao.Administracao.Fiscal.RegrasICMS.Consultar, true);
            btnNovoICMS.Visible = ValidaPermissao(Permissao.Administracao.Fiscal.RegrasICMS.Incluir);

            if (!IsPostBack)
            {
                PopularCombos();
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                lblTituloPagina.Text = sTituloPagina;

                ModoEdicao(false);
                Pesquisar();
            }

            if (Request.QueryString["action"] == "export")
            {
                if (Session["Relatorio_RegraICMS"] != null)
                {
                    ExportarConsultaSQLparaXLS(ExecutarDataSet(sProcedure, (Dictionary<string, string>)Session["Relatorio_RegraICMS"]), "Relatorio_RegraICMS");
                    MensagemPagina.MostraMensagem_Sucesso("Relatório Gerado com sucesso!");
                }
            }
        }

        private void Pesquisar()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Consultar_RegrasICMS" },
                { "@sUFOrigem", ddlsOrigemPesquisa.SelectedValue },
                { "@sUFDestino", ddlsDestinoPesquisa.SelectedValue }
            };
            DataTable tb = ExecutarDataTable(sProcedure, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                div_gvConsulta.Visible = true;
                Session["Relatorio_RegraICMS"] = SalvarParametrosParaExcel();
                ScriptManager.RegisterStartupScript(Page, GetType(), "DataTables", DataBindComScript(dtgvConsulta, tb, 0, "asc"), true);
            }
            else
            {
                div_gvConsulta.Visible = false;
                MensagemPagina.MostraMensagem_Erro("Nenhum Registro Encontrado");
            }
        }

        #endregion

        #region | Utils

        private void PopularCombos()
        {
            ddlsOrigemPesquisa.Popula_Combo("sp_Select 'UF'", "sEstado", "sDscEstado", false, "Todos os Estados de Origem", "");
            ddlsDestinoPesquisa.Popula_Combo("sp_Select 'UF'", "sEstado", "sDscEstado", false, "Todos os Estados de Destino", "");
            ddlidOrigem.Popula_Combo("sp_Select 'UF'", "sEstado", "sDscEstado", false, "Selecione a Origem", "0");
            ddlidDestino.Popula_Combo("sp_Select 'UF'", "sEstado", "sDscEstado", false, "Todos os Destinos", "0");
        }

        private void LimpaCampos()
        {
            ddlidOrigem.SelectedValue = "0";
            ddlidDestino.SelectedValue = "0";
        }

        private void ModoEdicao(bool bEdicao)
        {
            pnICMSConsulta.Visible = !bEdicao;
            pnICMSDetalhe.Visible = bEdicao;
        }

        private void SalvarRegrasICMS()
        {
            foreach (var item in bs_RegrasICMS)
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Salvar_RegrasICMS" },
                    { "@idRegraICMS", item.idRegraICMS.ToString() },
                    { "@sUFOrigem", item.sUFOrigem },
                    { "@sUFDestino", item.sUFDestino },
                    { "@nICMS", item.nICMS.DecimalToDecimalString() },
                    { "@nICMS_DIFAL", item.nICMS_DIFAL.DecimalToDecimalString() },
                    { "@nICMS_Nacional", item.nICMS_Nacional.DecimalToDecimalString() },
                    { "@nICMS_Importado", item.nICMS_Importado.DecimalToDecimalString() },
                    { "@nIBS_UF", item.nIBS_UF.DecimalToDecimalString() },
                    { "@nRedBC", item.nRedBC.DecimalToDecimalString() },
                    { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                };
                DataSet ds = ExecutarDataSet(sProcedure, vParametros, false);

                if (ValidarDataSet(ds, out string sErro))
                    hddidRegraICMS.Value = DATASET(ds, "idRegraICMS");
                else
                    throw new Exception("BD: " + sErro.ToString());
            }
        }

        private void RegraICMSDetalhe()
        {
            ModoEdicao(true);
            bs_RegrasICMS.Clear();

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Consultar_RegrasICMS" },
                    { "@idRegraICMS", hddidRegraICMS.Value }
                };
                DataSet ds = ExecutarDataSet(sProcedure, vParametros);

                if (ValidarDataSet(ds, out string sErro))
                {
                    bs_RegrasICMS.Add(new cls_RegrasICMS
                    {
                        idLinha = bs_RegrasICMS.Count + 1,
                        idRegraICMS = Convert.ToInt32(hddidRegraICMS.Value),
                        sUFOrigem = DATASET(ds, "sUFOrigem"),
                        sUFDestino = DATASET(ds, "sUFDestino"),
                        sDscUFOrigem = DATASET(ds, "sDscUFOrigem"),
                        sDscUFDestino = DATASET(ds, "sDscUFDestino"),
                        nICMS = Convert.ToDecimal(DATASET(ds, "nICMS")),
                        nICMS_DIFAL = Convert.ToDecimal(DATASET(ds, "nICMS_DIFAL")),
                        nICMS_Nacional = Convert.ToDecimal(DATASET(ds, "nICMS_Nacional")),
                        nICMS_Importado = Convert.ToDecimal(DATASET(ds, "nICMS_Importado")),
                        nIBS_UF = Convert.ToDecimal(DATASET(ds, "nIBS_UF")),
                        nRedBC = Convert.ToDecimal(DATASET(ds, "nRedBC"))
                    });

                    PainelAtualizacao.Visible = true;
                    PainelAtualizacao.Personalizar(string.Format("Última atualização em <b>{0}</b> por <b>{1}</b>", DATASET(ds, "dtAtualizacao"), DATASET(ds, "sDscUsuarioAtualizacao")));

                    txtIdRegraICMS.Text = hddidRegraICMS.Value;

                    div_novaRegra.Visible = false;

                    gvRegrasICMS_Novo.DataSource = bs_RegrasICMS;
                    gvRegrasICMS_Novo.DataBind();
                }
                else throw new Exception("BD: " + sErro.ToString());
            }
            catch (Exception e)
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro(e.Message);
            }
        }        

        private Dictionary<string, string> SalvarParametrosParaExcel()
        {
            return new Dictionary<string, string>
            {
                { "@sFuncao", "Consultar_Relatorio_Excel" },
                { "@sUFOrigem", ddlsOrigemPesquisa.SelectedValue },
                { "@sUFDestino", ddlsDestinoPesquisa.SelectedValue }
            };
        }

        public static void ExportarConsultaSQLparaXLS(DataSet ds, string sNomeArquivoSemExten)
        {
            string sNomeArquivoComExtensao = sNomeArquivoSemExten + CarimboDataHora() + ".xls";
            var response = HttpContext.Current.Response;

            string logoBase64 = "";
            string caminhoLogo = HttpContext.Current.Server.MapPath("~/img/LogoTT.png");
            if (File.Exists(caminhoLogo))
            {
                byte[] imageBytes = File.ReadAllBytes(caminhoLogo);
                string base64String = Convert.ToBase64String(imageBytes);
                logoBase64 = "data:image/png;base64," + base64String;
            }

            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.AddHeader("content-disposition", "attachment; filename=" + sNomeArquivoComExtensao);
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = "application/vnd.ms-excel";
            response.ContentEncoding = Encoding.UTF8;

            StringBuilder lSbExcel = new StringBuilder();

            lSbExcel.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n");
            lSbExcel.Append("<style type=\"text/css\">\r\n");
            lSbExcel.Append("body { font-family: Arial, Helvetica, sans-serif; color: #333; }\r\n");
            lSbExcel.Append(".report-table { border-collapse: collapse; width: 100%; font-size: 12px; }\r\n");

            // --- ESTILO AQUI ---
            lSbExcel.Append(".report-table th, .report-table td { font-family: Arial, Helvetica, sans-serif; border: 1px solid #999999; padding: 12px 15px; text-align: left; vertical-align: middle; }\r\n");

            lSbExcel.Append(".report-table th { background-color: #009a22; color: #ffffff; font-size: 13px; font-weight: bold; text-transform: uppercase; }\r\n");
            lSbExcel.Append(".report-table tr.alt-row td { background-color: #f2f2f2; }\r\n");
            lSbExcel.Append(".header-container { text-align: center; margin-bottom: 25px; }\r\n");
            lSbExcel.Append(".logo { max-height: 60px; margin-bottom: 15px; }\r\n");
            lSbExcel.Append(".report-title { font-family: Arial, Helvetica, sans-serif; color: #024e0a; font-size: 24px; font-weight: bold; margin: 0; }\r\n");
            lSbExcel.Append(".report-subtitle { font-family: Arial, Helvetica, sans-serif; font-size: 14px; text-align: center; color: #666; margin-top: 5px; }\r\n");
            lSbExcel.Append("hr.separator { border: 0; height: 2px; background-color: #009a22; margin-top: 25px; }\r\n");
            lSbExcel.Append("</style>\r\n\r\n");

            // --- Montagem do Cabeçalho ---
            lSbExcel.Append("<div class='header-container'>");
            if (!string.IsNullOrEmpty(logoBase64))
                lSbExcel.AppendFormat("<img src='{0}' class='logo' />", logoBase64);

            lSbExcel.Append("<div class='report-title'>Relatório Regras ICMS</div>");
            lSbExcel.AppendFormat("<div class='report-subtitle'>Gerado em: {0}</div>", DateTime.Now.ToString("dd/MM/yyyy 'às' HH:mm:ss"));
            lSbExcel.Append("</div>");
            lSbExcel.Append("<hr class='separator' />");

            // --- Montagem da Tabela de Dados ---
            lSbExcel.Append("<table class=\"report-table\">\r\n");
            lSbExcel.Append("<thead>\r\n");
            lSbExcel.Append("<tr>\r\n");
            foreach (DataColumn Coluna in ds.Tables[0].Columns)
            {
                lSbExcel.AppendFormat("\t<th>{0}</th>\r\n", Coluna.ColumnName);
            }
            lSbExcel.Append("</tr>\r\n");
            lSbExcel.Append("</thead>\r\n");
            lSbExcel.Append("<tbody>\r\n");

            int rowIndex = 0;
            foreach (DataRow Linha in ds.Tables[0].Rows)
            {
                string rowClass = (rowIndex % 2 != 0) ? "class='alt-row'" : "";
                lSbExcel.AppendFormat("<tr {0}>\r\n", rowClass);
                foreach (DataColumn Coluna in ds.Tables[0].Columns)
                {
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(Linha[Coluna].ToString().Trim()));
                }
                lSbExcel.Append("</tr>\r\n");
                rowIndex++;
            }

            lSbExcel.Append("</tbody>\r\n");
            lSbExcel.Append("</table>\r\n");

            // --- Finalização da Resposta ---
            response.Write(lSbExcel.ToString());
            response.Flush();
            response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        #endregion

        #region | Eventos

        protected void lbRegrasIMCS_Command(object sender, CommandEventArgs e)
        {
            hddidRegraICMS.Value = e.CommandArgument.ToString();
            RegraICMSDetalhe();
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void btnNovoICMS_Click(object sender, EventArgs e)
        {
            ModoEdicao(true);
            LimpaCampos();

            bs_RegrasICMS.Clear();

            div_gvRegrasICMS_Novo.Visible = false;
            PainelAtualizacao.Visible = false;
            div_novaRegra.Visible = true;  
            
            txtIdRegraICMS.Text = "Novo";
        }

        protected void btnIncluir_Click(object sender, EventArgs e)
        {
            if (ddlidOrigem.SelectedValue != "0")
            {
                if (bs_RegrasICMS.Count != 0 || bs_RegrasICMS.Any(x => x.sUFDestino.ToString() == ddlidDestino.SelectedValue))
                    MensagemPaginaDetalhe.MostraMensagem_Erro("UF Destino já consta na lista");
                else
                {
                    if (ddlidDestino.SelectedValue != "0")
                    {
                        bs_RegrasICMS.Add(new cls_RegrasICMS
                        {
                            idLinha = bs_RegrasICMS.Count + 1,
                            sUFOrigem = ddlidOrigem.SelectedValue,
                            sUFDestino = ddlidDestino.SelectedValue,
                            sDscUFOrigem = ddlidOrigem.SelectedItem.ToString(),
                            sDscUFDestino = ddlidDestino.SelectedItem.ToString(),
                            nICMS = 0,
                            nICMS_DIFAL = 0,
                            nICMS_Nacional = 0,
                            nICMS_Importado = 0,
                            nIBS_UF = 0,
                            nRedBC = 0
                        });
                    }
                    else
                    {
                        foreach (ListItem item in ddlidDestino.Items)
                        {
                            if (item.Value == "0")
                                continue;

                            bs_RegrasICMS.Add(new cls_RegrasICMS
                            {
                                idLinha = bs_RegrasICMS.Count + 1,
                                sUFOrigem = ddlidOrigem.SelectedValue,
                                sUFDestino = item.Value,
                                sDscUFOrigem = ddlidOrigem.SelectedItem.ToString(),
                                sDscUFDestino = item.Text,
                                nICMS = 0,
                                nICMS_DIFAL = 0,
                                nICMS_Nacional = 0,
                                nICMS_Importado = 0,
                                nIBS_UF = 0,
                                nRedBC = 0
                            });
                        }
                    }

                    LimpaCampos();
                    div_gvRegrasICMS_Novo.Visible = true;

                    gvRegrasICMS_Novo.DataSource = bs_RegrasICMS;
                    gvRegrasICMS_Novo.DataBind();                   
                }

            }
            else MensagemPaginaDetalhe.MostraMensagem_Erro("Selecione uma Origem");
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow row in gvRegrasICMS_Novo.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        int idLinha = Convert.ToInt32(gvRegrasICMS_Novo.DataKeys[row.RowIndex].Value);
                        var txtICMS = (TextBox_Padrao)row.FindControl("txtnICMS");
                        var txtICMS_DIFAL = (TextBox_Padrao)row.FindControl("txtnICMS_DIFAL");
                        var txtICMS_Nacional = (TextBox_Padrao)row.FindControl("txtnICMS_Nacional");
                        var txtICMS_Importado = (TextBox_Padrao)row.FindControl("txtnICMS_Importado");
                        var txtIBS_UF = (TextBox_Padrao)row.FindControl("txtnIBS_UF");
                        var txtRedBC = (TextBox_Padrao)row.FindControl("txtnRedBC");

                        var item = bs_RegrasICMS.FirstOrDefault(x => x.idLinha == idLinha);                        
                        item.nICMS = txtICMS.Text == "" ? 0 : Convert.ToDecimal(txtICMS.Text);
                        item.nICMS_DIFAL = txtICMS_DIFAL.Text == "" ? 0 : Convert.ToDecimal(txtICMS_DIFAL.Text);
                        item.nICMS_Nacional = txtICMS_Nacional.Text == "" ? 0 : Convert.ToDecimal(txtICMS_Nacional.Text);
                        item.nICMS_Importado = txtICMS_Importado.Text == "" ? 0 : Convert.ToDecimal(txtICMS_Importado.Text);
                        item.nIBS_UF = txtIBS_UF.Text == "" ? 0 : Convert.ToDecimal(txtIBS_UF.Text);
                        item.nRedBC = txtRedBC.Text == "" ? 0 : Convert.ToDecimal(txtRedBC.Text);
                    }
                }

                SalvarRegrasICMS();

                if (bs_RegrasICMS.Count > 1)
                {
                    ModoEdicao(false);
                    Pesquisar();
                    MensagemPagina.MostraMensagem_Sucesso("Registros salvos com sucesso");
                }
                else
                {
                    RegraICMSDetalhe();
                    MensagemPaginaDetalhe.MostraMensagem_Sucesso("Registro salvo com sucesso");
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro("Erro ao salvar: " + ex.ToString());
            }
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            bs_RegrasICMS.Clear();
            ModoEdicao(false);
            Pesquisar();
        }

        #endregion
    }
}