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

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class EntregaEPI : Page
    {
        private static string sTituloPagina = "Entrega de EPI";
        private static object dt = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.Entrega_EPI.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Entrega_EPI.Incluir);
            cmdEPIs.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Entrega_EPI.Consultar_Controle_de_EPIs);

            if (!IsPostBack)
            {
                ddlSituacao.SelectedValue = "S";

                FUNCOES.Popula_Combo(ddlEPIs, "sp_Select 'Flow_Colaboradores_EntregaEPI_EPIs'", "idItem", "sCodigoComDescricao", false, "Todos os EPIs", "0");
                FUNCOES.Popula_Combo(ddlColaborador, "sp_Select 'Flow_Colaboradores'", "idColaborador", "sDscColaborador", false, "Todos os Colaboradores", "0");
                FUNCOES.Popula_Combo(ddlStatus, "sp_Select 'tbl_Flow_Colaboradores_Entrega_EPI_Status'", "idStatus", "sDscStatus", false, "Todos os Status", "0");
                FUNCOES.Popula_Combo(ddlTipo, "sp_Select 'tbl_Flow_Colaboradores_Entrega_EPI_Tipo'", "idTipo", "sDscTipo", false, "Todos os Tipos", "0");

                Pesquisar();

                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;

                FUNCOES.Scripts.FocusScript(Page, txtPesquisa.ClientID);
            }
            else ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, dt, 0, new int[4] { 0, 2, 3, 5 }, "desc", "false", "''"), true);

            RegistraScript();
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;

            DateTime.TryParse(txtdtInicial.Text, out DateTime dtInicial);
            DateTime.TryParse(txtdtFinal.Text, out DateTime dtFinal);

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscPesquisa", txtPesquisa.Text.Trim() },
                { "@idColaborador", ddlColaborador.SelectedValue },
                { "@idStatus", ddlStatus.SelectedValue },
                { "@idProduto", ddlEPIs.SelectedValue },
                { "@sFiltroData", ddlFiltraDatas.SelectedValue },
                { "@dtInicial", txtdtInicial.Text.Length > 0 ? dtInicial.ToString("dd/MM/yyyy") : "" },
                { "@dtFinal", txtdtFinal.Text.Length > 0 ? dtFinal.ToString("dd/MM/yyyy") : "" },
                { "@idTipoEntrega", ddlTipo.SelectedValue },
                { "@sAtivo", ddlSituacao.SelectedValue }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Entrega_EPI", vParametros);

            if (tb.Rows.Count > 0)
            {
                pnResultado.Visible = true;

                dt = tb;

                string idColaborador = ddlColaboradores.SelectedValue;
                ddlColaboradores.Items.Clear();
                ddlColaboradores.Items.Add(new ListItem("Selecione um Colaborador", "0"));

                dtgvConsulta.Columns[0].Visible = false;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, 0, new int[4] { 0, 2, 3, 5 }, "desc", "false", "''"), true);

                if (ddlColaboradores.Items.Contains(ddlColaboradores.Items.FindByValue(idColaborador)))
                    ddlColaboradores.SelectedValue = idColaborador;
                else
                    ddlColaboradores.SelectedIndex = 0;
            }
            else
            {
                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
            }
        }

        protected void RegistraScript()
        {
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page);

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

        protected void cmdPesquisar_Click(object sender, EventArgs e) { Pesquisar(); FUNCOES.Scripts.FocusScript(Page, txtPesquisa.ClientID); }

        protected void cmdEPIs_Click(object sender, EventArgs e) { ddlColaboradores_SelectedIndexChanged(null, null); FUNCOES.Scripts.AbrirModal(Page, "modalEPI"); }

        protected void ddlColaboradores_SelectedIndexChanged(object sender, EventArgs e)
        {
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
                    div_gvEPIs.Visible = true;
                    PainelAtualizacao.Personalizar($"Última <b>Conferência dos EPIs</b> realizada em <b>{RETORNO.DATASET(ds, 1, 0, "dtUltimaConferencia")}</b> por <b>{RETORNO.DATASET(ds, 1, 0, "sDscUsuarioConferencia")}</b>");
                }
                else
                {
                    div_gvEPIs.Visible = true;
                    MensagemPagina_ModalEPI.MostraMensagem_Erro("Não foram localizados EPIs na Função do Colaborador selecionado!");
                }
            }

            FUNCOES.Scripts.RemoverBackdrop_Modal(Page);
            FUNCOES.Scripts.AbrirModal(Page, "modalEPI");
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && !ddlColaboradores.Items.Contains(ddlColaboradores.Items.FindByValue(e.Row.Cells[e.Row.Cells.Count - 1].Text)))
                ddlColaboradores.Items.Add(new ListItem((e.Row.Cells[2].Controls[0] as HyperLink).Text, e.Row.Cells[e.Row.Cells.Count - 1].Text));
        }

        protected void gvEPIs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[2].Text = e.Row.Cells[2].Text == "0" ? string.Empty : e.Row.Cells[2].Text;
                e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
            }
        }

        protected void cmdRepeteDatas_Click(object sender, EventArgs e) { txtdtFinal.Text = txtdtInicial.Text; FUNCOES.Scripts.FocusScript(Page, cmdPesquisar.ClientID); }

        #endregion
    }
}