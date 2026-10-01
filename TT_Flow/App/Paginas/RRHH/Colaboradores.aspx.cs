using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using TT.FrameWork;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using TT_Flow.FrameWork;
using System.IO;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class Colaboradores : Page
    {
        private static string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores";
        private static string sTituloPagina = "Colaboradores";
        private static object dt = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Incluir, false);
            cmdEPIs.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Entrega_EPI.Consultar_Controle_de_EPIs, false);

            if (!IsPostBack)
            {
                if (Request.QueryString["action"] == "export")
                {  
                    if (Session["RelatorioColaboradores_Parametros"] != null)
                    {
                        string sNomeArquivo = "Relatorio_Colaboradores";

                        Dictionary<string, string> vParametros = (Dictionary<string, string>)Session["RelatorioColaboradores_Parametros"];
                        DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                        ExportarConsultaSQLparaXLS(ds, sNomeArquivo);                                     
                    }

                    return;
                }

                pnResultado.Visible = false;

                FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos", "0");
                FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");
                FUNCOES.Popula_Combo(ddlidCargo, "sp_Select 'Flow_Colaboradores_Cargos'", "idCargo", "sDscCargo", false, "Todos os Cargos", "0");
                FUNCOES.Popula_Combo(ddlidTipoContrato, "sp_Select 'tbl_Flow_Colaboradores_TipoContrato'", "idTipoContrato", "sDscTipoContrato", false, "Todas os Tipos de Contratos", "0");
                FUNCOES.Popula_Combo(ddlsCBO, "sp_Select 'Flow_Colaboradores_FuncaoCarteira_sCBO'", "sCBO", "sCBO", false, "Todos os CBO", "0");
                FUNCOES.Popula_Combo(ddlidSupervisorDireto, "sp_Select 'RRHH_SUPERVISORES'", "idColaborador", "sDscColaborador", false, "Todos os Supervisores", "0");

                FUNCOES.Popula_Combo(ddlidFuncao, "sp_Manipula_tbl_Flow_Colaboradores_FuncaoCarteira @sFuncao='Flow-Funcoes'", "idFuncao", "sDscFuncao", false, "Todas Funções", "0");

                PopularCombo_GHE("0");
                if (Request["idGHE"] != null)
                {
                    ddlidGHE.SelectedValue = Convert.ToInt32(Request["idGHE"].ToString()).ToString();
                    lblSubTituloPagina.Text = " GHE " + ddlidGHE.SelectedItem.Text;
                    DIV_Filtro.Visible = false;
                }
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;

                Pesquisar();
            }
            else
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, dt, 1, new int[1] { 6 }, "asc", "false", "''"), true);

            RegistraScript();
        }
        protected void PopularCombo_GHE(string idEmpresa)
        {
            FUNCOES.Popula_Combo(ddlidGHE, "sp_Manipula_tbl_Flow_Colaboradores_GHE 'SELECT_GHE', @sidEmpresa=" + idEmpresa, "idGHE", "sDscGHE", false, "Todos os GHEs", "0");
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idDepartamento", ddlidDepartamento.SelectedValue },
                { "@sSituacao", ddlsSituacao.SelectedValue },
                { "@idEmpresa", ddlidEmpresa.SelectedValue },
                { "@idCargo", ddlidCargo.SelectedValue },
                { "@idTipoContrato", ddlidTipoContrato.SelectedValue },
                { "@sPesquisa", txtPesquisa.Text.Trim() },
                { "@sCBO", ddlsCBO.Text },
                { "@idGHE", ddlidGHE.SelectedValue },
                { "@idSupervisorDireto", ddlidSupervisorDireto.SelectedValue },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                { "@idFuncao", ddlidFuncao.SelectedValue } //Thiago Rodrigues - 22/10/2025
            };

            if (Request["idFuncao"] != null)
            {
                vParametros.Add("@idFuncao", Request["idFuncao"].ToString());
                DIV_Filtro.Visible = false;
            }

            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros);

            Session["RelatorioColaboradores_Parametros"] = SalvarParametrosParaExcel();

            if (tb.Rows.Count > 0)
            {
                pnResultado.Visible = true;

                dt = tb;

                string idColaborador = ddlColaboradores.SelectedValue;
                ddlColaboradores.Items.Clear();
                ddlColaboradores.Items.Add(new ListItem("Selecione um Colaborador", "0"));

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, 1, new int[1] { 6 }, "asc", "false", "''"), true);

                if (ddlColaboradores.Items.Contains(ddlColaboradores.Items.FindByValue(idColaborador)))
                    ddlColaboradores.SelectedValue = idColaborador;
                else
                    ddlColaboradores.SelectedIndex = 0;
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum registro Localizado");
        }

        protected void RegistraScript()
        {
            FUNCOES.Scripts.FocusScript(Page, txtPesquisa.ClientID);
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page, "tooltip", "right");

            StringBuilder sb = new StringBuilder();

            sb.Append("     function MudarBotao() {\r\n");
            sb.Append("         $('#cphCorpo_cmdPesquisar').val('Pesquisando...');\r\n");
            sb.Append("     }\r\n\r\n");

            sb.Append("     var cardTimer = { };\r\n");
            sb.Append("     function mostraCard(element, idProduto, tabela) {\r\n");
            sb.Append("         cardTimer[idProduto + '_' + tabela] = setTimeout(function() {\r\n");
            sb.Append("             $.ajax({\r\n");
            sb.Append("                 url: \"/API/Pagina_Ajax.aspx/GetProdutoDetalhes\",\r\n");
            sb.Append("                 data: JSON.stringify({ idProduto: idProduto }),\r\n");
            sb.Append("                 type: 'POST',\r\n");
            sb.Append("                 dataType: 'json',\r\n");
            sb.Append("                 contentType: 'application/json; charset=utf-8',\r\n");
            sb.Append("                 success: function(response) {\r\n");
            sb.Append("                     var produto = JSON.parse(response.d);\r\n");
            sb.Append("                     var cardProduto = `\r\n");
            sb.Append("                         <div class=\"card\">\r\n");
            sb.Append("                             <div class=\"card-body d-flex\">\r\n");
            sb.Append("                                 <div class=\"flex-shrink-0\" style=\"min-inline-size: fit-content;\">\r\n");
            sb.Append("                                     ${produto.imagem? `<img src = \"${produto.imagem}\" alt=\"Imagem do Produto\" class=\"img-fluid img-thumbnail\" style=\"width: 100px; height: auto;\" />` : ''}\r\n");
            sb.Append("                                 </div>\r\n");
            sb.Append("                                 <div class=\"flex-grow-1 d-flex flex-column ms-3\">\r\n");
            sb.Append("                                     <div class=\"d-flex\">\r\n");
            sb.Append("                                         ${produto.sCategoriaVendas? `<div class=\"card-text me-3\"> <strong>Categoria Vendas: </strong>${produto.sCategoriaVendas\r\n}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sFabricante ? `<div class= \"card-text me-3\"> <strong > Fabricante: </strong >${ produto.sFabricante}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sTipo ? `<div class= \"card-text me-3\"> <strong > Tipo: </strong >${ produto.sTipo}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sGrupo ? `<div class= \"card-text me-3\"> <strong > Grupo: </strong >${ produto.sGrupo}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sFamilia ? `<div class= \"card-text me-3\"> <strong > Família: </strong >${ produto.sFamilia}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sPaisOrigem ? `<div class= \"card-text me-3\"> <strong > Origem: </strong >${ produto.sPaisOrigem}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sLocalArmazenamento ? `<div class= \"card-text me-3\"> <strong > Local Armazenamento: </strong >${ produto.sLocalArmazenamento}</div>` : ''}\r\n");
            sb.Append("                                     </div>\r\n");
            sb.Append("                                 </div>\r\n");
            sb.Append("                             </div>\r\n");
            sb.Append("                         </div>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                     var cardId = idProduto + '_' + tabela;\r\n");
            sb.Append("                     var card = document.getElementById(cardId);\r\n");
            sb.Append("                     card.innerHTML = cardProduto;\r\n");
            sb.Append("                     var rect = element.getBoundingClientRect();\r\n");
            sb.Append("                     var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;\r\n");
            sb.Append("                     var scrollLeft = document.documentElement.scrollLeft || document.body.scrollLeft;\r\n");
            sb.Append("                     hideAllCards();\r\n");
            sb.Append("                     card.style.top = (rect.top + scrollTop - 10) + 'px';\r\n");
            sb.Append("                     card.style.left = (rect.right + scrollLeft + element.offsetWidth + 10) + 'px';\r\n");
            sb.Append("                     card.style.display = 'block';\r\n");
            sb.Append("                 },\r\n");
            sb.Append("                 error: function(error) {\r\n");
            sb.Append("                     console.error(\"Erro ao obter os detalhes do produto:\", error);\r\n");
            sb.Append("                 }\r\n");
            sb.Append("             });\r\n");
            sb.Append("         }, 300);\r\n");
            sb.Append("     }\r\n\r\n");
            sb.Append("     function escondeCard(idProduto, tabela) {\r\n");
            sb.Append("         var cardId = idProduto + '_' + tabela;\r\n");
            sb.Append("         var card = document.getElementById(cardId);\r\n");
            sb.Append("         clearTimeout(cardTimer[idProduto + '_' + tabela]);\r\n");
            sb.Append("         card.style.display = 'none';\r\n");
            sb.Append("     }\r\n\r\n");
            sb.Append("     function hideAllCards() {\r\n");
            sb.Append("         var cards = document.querySelectorAll('.product-card');\r\n");
            sb.Append("         cards.forEach(function(card) {\r\n");
            sb.Append("             card.style.display = 'none';\r\n");
            sb.Append("         });\r\n");
            sb.Append("     }\r\n\r\n");
            sb.Append("     function openModal(idProduto) {\r\n");
            sb.Append("         $.ajax({\r\n");
            sb.Append("             url: \"/API/Pagina_Ajax.aspx/GetProdutoDetalhes\",\r\n");
            sb.Append("             data: JSON.stringify({ idProduto: idProduto }),\r\n");
            sb.Append("             type: 'POST',\r\n");
            sb.Append("             dataType: 'json',\r\n");
            sb.Append("             contentType: 'application/json; charset=utf-8',\r\n");
            sb.Append("             success: function(response) {\r\n");
            sb.Append("                 var produto = JSON.parse(response.d);\r\n");
            sb.Append("                 var tituloProduto = `\r\n");
            sb.Append("                     <button type = \"button\" class= \"close\" data - dismiss = \"modal\" aria - label = \"Close\">\r\n");
            sb.Append("                         <span aria - hidden = \"true\" > &times;</span>\r\n");
            sb.Append("                     </button>\r\n");
            sb.Append("                     <h5 class= \"modal-title\" id = \"detailsModalLabel\" > ${ produto.sCodigo} - ${ produto.sDsc}</h5>\r\n");
            sb.Append("                 `;\r\n");
            sb.Append("                 var modalInfo = document.getElementById('modalInfo');\r\n");
            sb.Append("                 modalInfo.innerHTML = tituloProduto;\r\n");
            sb.Append("                 var imagem = '';\r\n");
            sb.Append("                 if (produto.imagem) {\r\n");
            sb.Append("                     imagem += `\r\n");
            sb.Append("                         <div style = \"text-align: center; margin-bottom: 20px;\">\r\n");
            sb.Append("                             <img src = \"${produto.imagem}\" alt = \"Imagem do Produto\" class= \"img-fluid\" style = \"width: 300px; height: auto;\" />\r\n");
            sb.Append("                         </div>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 var tabelaProduto = '<table class=\"table table-bordered\">';\r\n");
            sb.Append("                 if (produto.sCategoriaVendas) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Categoria Vendas </th>\r\n");
            sb.Append("                             <td>${ produto.sCategoriaVendas}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sTipo) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Tipo </th>\r\n");
            sb.Append("                             <td>${ produto.sTipo}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sGrupo) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Grupo </th>\r\n");
            sb.Append("                             <td>${ produto.sGrupo}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sFabricante) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Fabricante </th>\r\n");
            sb.Append("                             <td>${ produto.sFabricante}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sLocalArmazenamento) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Local Armazenamento </th>\r\n");
            sb.Append("                             <td>${ produto.sLocalArmazenamento}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sFamilia) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Família </th>\r\n");
            sb.Append("                             <td>${ produto.sFamilia}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sCodigoCEST) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> CEST </th>\r\n");
            sb.Append("                             <td>${ produto.sCodigoCEST}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sCodigoNCM) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> NCM </th>\r\n");
            sb.Append("                             <td>${ produto.sCodigoNCM}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sPaisOrigem) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Origem </th>\r\n");
            sb.Append("                             <td>${ produto.sPaisOrigem}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 tabelaProduto += `</table >`;\r\n");
            sb.Append("                 var modalBody = document.getElementById('modalBody');\r\n");
            sb.Append("                 modalBody.innerHTML = imagem + tabelaProduto;\r\n");
            sb.Append("                 $('#produtoDetalheModal').modal('show');\r\n");
            sb.Append("             },\r\n");
            sb.Append("             error: function(error) {\r\n");
            sb.Append("                 console.error(\"Erro ao obter os detalhes do produto:\", error);\r\n");
            sb.Append("             }\r\n");
            sb.Append("         });\r\n");
            sb.Append("     }\r\n");
            sb.Append("     function openProductDetail(idItem) {\r\n");
            sb.Append("         var url = '/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id=' + idItem;\r\n");
            sb.Append("         window.open(url, '_blank');\r\n");
            sb.Append("         return false;\r\n");
            sb.Append("     }\r\n");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RegistraScript", sb.ToString(), true);
        }

        #region | Eventos

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdEPIs_Click(object sender, EventArgs e) { ddlColaboradores_SelectedIndexChanged(null, null); FUNCOES.Scripts.AbrirModal(Page, "modalEPI"); }

        protected void ddlColaboradores_SelectedIndexChanged(object sender, EventArgs e)
        {
			div_espaco_modal.Visible = true;								
            if (ddlColaboradores.SelectedIndex == 0)
            {
                div_gvEPIs.Visible = false;
                gvEPIs.DataSource = null;
                gvEPIs.DataBind();
            }
            else
            {
                cmdConferencia.NavigateUrl = $"~/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id=0&idColaborador={ddlColaboradores.SelectedValue}";

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_EPI_X_COLABORADOR" },
                    { "@idColaborador", ddlColaboradores.SelectedValue }
                };
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores", vParametros);

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTable_EPI", Grid.DataBindComScriptData(gvEPIs, ds.Tables[0], 1, new int[1] { 5 }, "asc", "false", "''"), true);


                if (gvEPIs.Rows.Count > 0)
                {
                    div_espaco_modal.Visible = false;
                    div_gvEPIs.Visible = true;

                    string dt = RETORNO.DATASET(ds, 1, 0, "dtUltimaConferencia");
                    string sdsc = RETORNO.DATASET(ds, 1, 0, "sDscUsuarioConferencia");
                    PainelAtualizacao.Personalizar($"Última <b>Conferência de EPIs</b> realizada em <b>{dt}</b> por <b>{sdsc}</b>");
                    if (string.IsNullOrEmpty(dt) || string.IsNullOrEmpty(sdsc)) PainelAtualizacao.Visible = false;
                }
                else
                {
                    div_gvEPIs.Visible = false;
                    MensagemPagina_ModalEPI.MostraMensagem_Erro("Não foram localizados EPIs na Função do Colaborador selecionado!");
                }
            }

            FUNCOES.Scripts.RemoverBackdrop_Modal(Page);
            FUNCOES.Scripts.AbrirModal(Page, "modalEPI");
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && !ddlColaboradores.Items.Contains(ddlColaboradores.Items.FindByValue((e.Row.Cells[0].Controls[0] as HyperLink).Text)))
                ddlColaboradores.Items.Add(new ListItem($"{(e.Row.Cells[1].Controls[0] as HyperLink).Text} 🡺 {(e.Row.Cells[3].Controls[0] as HyperLink).Text}", (e.Row.Cells[0].Controls[0] as HyperLink).Text));
        }

        protected void gvEPIs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[2].Text = e.Row.Cells[2].Text == "0" ? string.Empty : e.Row.Cells[2].Text;
                e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
            }
        }

        #endregion

        protected void ddlidEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularCombo_GHE(ddlidEmpresa.SelectedValue);
        }

        private Dictionary<string,string> SalvarParametrosParaExcel()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Consultar_Relatorio_Excel" },
                { "@idDepartamento", ddlidDepartamento.SelectedValue },
                { "@sSituacao", ddlsSituacao.SelectedValue },
                { "@idEmpresa", ddlidEmpresa.SelectedValue },
                { "@idCargo", ddlidCargo.SelectedValue },
                { "@idTipoContrato", ddlidTipoContrato.SelectedValue },
                { "@sPesquisa", txtPesquisa.Text.Trim() },
                { "@sCBO", ddlsCBO.Text },
                { "@idGHE", ddlidGHE.SelectedValue },
                { "@idSupervisorDireto", ddlidSupervisorDireto.SelectedValue },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };

            return vParametros;
        }

        public static void ExportarConsultaSQLparaXLS(DataSet ds, string sNomeArquivoSemExten)
        {
            string sNomeArquivoComExtensao = sNomeArquivoSemExten + FUNCOES.CarimboDataHora() + ".xls";
            var server = HttpContext.Current.Server;
            var response = HttpContext.Current.Response;

            string logoBase64 = "";
            string caminhoLogo = server.MapPath("~/img/LogoTT.png");
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
            response.ContentEncoding = System.Text.Encoding.UTF8;

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
            {
                lSbExcel.AppendFormat("<img src='{0}' class='logo' />", logoBase64);
            }
            lSbExcel.Append("<div class='report-title'>Relatório de Colaboradores</div>");
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
    }
}