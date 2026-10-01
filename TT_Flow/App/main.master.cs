using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IDENTITY = TT_Flow.FrameWork.Identity;
using TT.FrameWork;
using static TT.FrameWork.BD;
using static TT.FrameWork.Identity;
using static TT.FrameWork.Funcoes;

namespace TT_Flow.App
{
    public partial class MainMaster : MasterPage
    {
        StringBuilder salvaMenu = new StringBuilder();
        public static string sProcedure = "sp_Flow_Valida_Recursos_x_Usuarios";

        #region | Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.LoadComplete += new EventHandler(Page_LoadComplete);

            try
            {
                if (HttpContext.Current.Session["SalvaMenu"] != null)
                    salvaMenu.AppendLine(HttpContext.Current.Session["SalvaMenu"].ToString());
            }
            catch { }

            if (!IsPostBack)
            {
                try
                {
                    ValidarSessao();

                    divMarcaDagua.Visible = bDev;
                    div_ddlBancos.Visible = ValidaPermissao(Permissao.AlterarBancos);

                    //SwitchTema.Definir("N", "Tema", "N");

                    PopulaCombo_Bancos(ddlBancos);

                    if (salvaMenu.Length < 1)
                        Gera_MenuDinamico();
                    else
                    {
                        if (salvaMenu.Length > 0)
                        {
                            AplicarMenuIAFallback();
                            ltrMain.Text = salvaMenu.ToString();
                        }
                        else throw new Exception("Não há recursos disponíveis para seu usuário!");
                    }

                    Dictionary<string, string> vParametro = new Dictionary<string, string>
                    {
                        { "@sFuncao", "Usuarios_x_Empresas" },
                        { "@idUsuario", Variaveis.idUsuario() }
                    };
                    DataSet Usuarios_x_Empresas = ExecutarDataSet("sp_Manipula_tbl_Flow_Empresas", vParametro);

                    if (Usuarios_x_Empresas.Tables[0].Rows.Count == 0) btnBrasil.Visible = false;
                    if (Usuarios_x_Empresas.Tables[1].Rows.Count == 0) btnEUA.Visible = false;
                }
                catch (Exception ex)
                {
                    ltrMain.Text = string.Format("<h2><b>{0}</b></h2>", ex.Message);
                }

                AtualizarParametrosSistema();
            }
            else ScriptManager.GetCurrent(Page)?.RegisterAsyncPostBackControl(Page);

            if (ValidaPermissao(Permissao.Usuarios.Editar_PaginaInicial))
            {
                DataTable dt = ExecutarDataTable("sp_Manipula_tbl_Usuarios", new Dictionary<string, string> { { "@sFuncao", "CONSULTA_PAGINA_INICIAL" }, { "@idUsuario", Variaveis.idUsuario() }, { "@sURL_Recurso", Page.Request.RawUrl } });

                int.TryParse(dt.Rows[0][0].ToString(), out int idPaginaInicial);
                int.TryParse(dt.Rows[0][1].ToString(), out int idRecurso);

                if (idRecurso > 0)
                {
                    cmdPaginaInicial.Visible = true;
                    iconPaginaInicial.Attributes["class"] = "fa fa-square-o";
                    cmdPaginaInicial.Attributes["title"] = "Definir Página Inicial";

                    if (idPaginaInicial == idRecurso) { iconPaginaInicial.Attributes["class"] = "fa fa-check-square-o"; cmdPaginaInicial.Attributes["title"] = "Página Inicial do Usuário"; }
                    else cmdPaginaInicial.Attributes.Add("data-id", idRecurso.ToString());
                }
                else cmdPaginaInicial.Visible = false;
            }

            //SwitchTema.sFunctionScript_Switch = "AlterarTema(idSwitch.checked);";
            //SwitchTema.sCorFundo_Sim = "gray";
            //SwitchTema.sCorFundo_Nao = "white";
            //SwitchTema.sCorTexto_Sim = "black";
            //SwitchTema.sCorTexto_Nao = "white";
            //SwitchTema.sSim = "☾";
            //SwitchTema.sNao = "💡";

            RegistraScript();
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (Variaveis.idEmpresa() == "Brasil")
            {
                PopulaComboEmpresa(1);
                imgBrasil.Attributes["class"] += " selected";
                imgEUA.Attributes["class"] = imgBrasil.Attributes["class"].Replace("selected", "");
            }
            else if (Variaveis.idEmpresa() == "EUA")
            {
                PopulaComboEmpresa(2);
                imgEUA.Attributes["class"] += " selected";
                imgBrasil.Attributes["class"] = imgBrasil.Attributes["class"].Replace("selected", "");
            }

            // Script para aplicar o Título da Aba no navegador
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Titulo_AbaNavegador", $@"var titulo = $('[id*=lblTituloPagina]'); if (titulo.length) titulo = titulo.text() + ' - '; else titulo = ''; document.title = `{sNomeSistema} - ${{titulo}}{sVersao}`;", true);
        }

        #endregion

        #region | Eventos

        protected void timer_main_Atualizar_Tick(object sender, EventArgs e) { ValidarSessao(); AtualizarParametrosSistema(); Gera_MenuDinamico(); }

        protected void cmdPesquisa_click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtsPesquisa.Text))
            {
                if (string.IsNullOrEmpty(hddUrlRecurso.Value))
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sDscRecurso", txtsPesquisa.Text.Trim() },
                        { "@idUsuario", Variaveis.idUsuario() },
                        { "@sSalvaLog", "S" }
                    };
                    DataSet ds = ExecutarDataSet(sProcedure, vParametros);
                    if (ValidarDataSet(ds, out _))
                    {
                        hddUrlRecurso.Value = Retorno.DATASET(ds, 0, "sURL");
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_NovaAba_MenuPesquisa", $"window.open('{hddUrlRecurso.Value}', '_blank');", true);
                    }
                }

                hddUrlRecurso.Value = "";
            }
        }

        protected void lnkRecarregaMenu_Click(object sender, EventArgs e) { Gera_MenuDinamico(); Scripts.RecarregaPagina(Page); }

        protected void ddlBancos_SelectedIndexChanged(object sender, EventArgs e) { Session["idBanco"] = ddlBancos.SelectedValue; Scripts.RecarregaPagina(Page); }

        protected void cmdPaginaInicial_Click(object sender, EventArgs e)
        {
            if (iconPaginaInicial.Attributes["class"].Contains("check")) return;
            else ExecutarDataTable("sp_Manipula_tbl_Usuarios", new Dictionary<string, string> { { "@sFuncao", "SALVAR_PAGINA_INICIAL" }, { "@idUsuario", Variaveis.idUsuario() }, { "@idDashboard", cmdPaginaInicial.Attributes["data-id"] } });

            Scripts.RecarregaPagina(Page);
        }

        #endregion

        #region | Utils

        private void Gera_MenuDinamico()
        {
            salvaMenu.Clear();
            salvaMenu.AppendLine(ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@idUsuario", Variaveis.idUsuario() } }, out string sSQL).Tables[0].Rows[0][0].ToString());
            AplicarMenuIAFallback();
            HttpContext.Current.Session["SalvaMenu"] = salvaMenu.ToString();

            ltrMain.Text = salvaMenu.ToString();

            AtualizarParametrosSistema();
        }

        private void AplicarMenuIAFallback()
        {
            string menuAtual = salvaMenu.ToString();
            StringBuilder itensIA = new StringBuilder();
            AdicionarItemMenuIA(itensIA, menuAtual, Permissao.IA.Consultar, "Assistente IA", "/App/Paginas/IA/Chat.aspx", "fa fa-comments-o");
            AdicionarItemMenuIA(itensIA, menuAtual, Permissao.IA.VisualizarArquivos, "IA - Arquivos", "/App/Paginas/IA/Arquivos.aspx", "fa fa-file-text-o");
            AdicionarItemMenuIA(itensIA, menuAtual, Permissao.IA.VisualizarAuditoria, "IA - Auditoria", "/App/Paginas/IA/Auditoria.aspx", "fa fa-shield");
            AdicionarItemMenuIA(itensIA, menuAtual, Permissao.IA.AdministrarFerramentas, "IA - Configuracao", "/App/Paginas/IA/Configuracao.aspx", "fa fa-cogs");

            if (itensIA.Length == 0)
                return;

            if (InserirItensNoSubmenuIA(menuAtual, itensIA.ToString()))
                return;

            string menuIA = MontarMenuIA(itensIA.ToString());
            if (!InserirMenuAntesLogout(menuAtual, menuIA))
                salvaMenu.Append(menuIA);
        }

        private void AdicionarItemMenuIA(StringBuilder itens, string menuAtual, int idPermissao, string titulo, string url, string icone)
        {
            if (!ValidaPermissao(idPermissao) || MenuContemUrl(menuAtual, url))
                return;

            itens.AppendFormat("    <li><a href=\"{0}\"><i class=\"{1}\"></i> {2}</a></li>{3}", url, icone, titulo, Environment.NewLine);
        }

        private bool InserirItensNoSubmenuIA(string menuAtual, string itens)
        {
            int indiceIA = menuAtual.IndexOf("Paginas/IA/", StringComparison.OrdinalIgnoreCase);
            if (indiceIA < 0)
                return false;

            int indiceFechamento = menuAtual.IndexOf("</ul>", indiceIA, StringComparison.OrdinalIgnoreCase);
            if (indiceFechamento < 0)
                return false;

            salvaMenu.Insert(indiceFechamento, itens);
            return true;
        }

        private static string MontarMenuIA(string itens)
        {
            StringBuilder menu = new StringBuilder();
            menu.AppendLine("<li>");
            menu.AppendLine("  <a href=\"#\"><i class=\"fa fa-comments-o\"></i> IA <span class=\"fa arrow\"></span></a>");
            menu.AppendLine("  <ul class=\"nav nav-second-level\">");
            menu.Append(itens);
            menu.AppendLine("  </ul>");
            menu.AppendLine("</li>");
            return menu.ToString();
        }

        private bool InserirMenuAntesLogout(string menuAtual, string menuIA)
        {
            int indiceLogout = LocalizarIndiceLogout(menuAtual);
            if (indiceLogout < 0)
                return false;

            int indiceItem = menuAtual.LastIndexOf("<li", indiceLogout, StringComparison.OrdinalIgnoreCase);
            if (indiceItem < 0)
                return false;

            salvaMenu.Insert(indiceItem, menuIA);
            return true;
        }

        private static int LocalizarIndiceLogout(string menuAtual)
        {
            string[] marcadoresLogout = { "Log Out", "Logout", "LogOut", "Log Out", "power-off", "Login.aspx?LogOut", "Sair" };
            foreach (string marcador in marcadoresLogout)
            {
                int indice = menuAtual.IndexOf(marcador, StringComparison.OrdinalIgnoreCase);
                if (indice >= 0)
                    return indice;
            }

            return -1;
        }

        private static bool MenuContemUrl(string menuAtual, string url)
        {
            string urlSemBarra = (url ?? string.Empty).TrimStart('/');
            return menuAtual.IndexOf(url ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   menuAtual.IndexOf(urlSemBarra, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected void AtualizarParametrosSistema()
        {
            SqlDataReader dr = ExecutarDataReader($"sp_Flow_ConsultaDadosSistema '{Variaveis.idUsuario()}'");

            if (dr != null)
            {
                while (dr.Read())
                {
                    if (dr["sSituacao"].ToString() == "S")
                    {
                        sNomeSistema = dr["sNomeSistema"].ToString();
                        sVersao = dr["sVersao"].ToString();
                        IDENTITY.nQtdMensagens = dr["nQtdMensagens"].ToString();
                        IDENTITY.nQtdMensagens_NaoLidas = dr["nQtdMensagens_NaoLidas"].ToString();
                        IDENTITY.nQtdMensagens_Lidas = dr["nQtdMensagens_Lidas"].ToString();
                        Session["sPermissao"] = dr["sPermissao"].ToString();
                    }
                    else
                    {
                        dr.Close();
                        DirecionaPagina("App/Login.aspx");
                    }
                }

                dr.Close();
            }
        }

        #endregion

        #region | Bandeiras

        protected void btnBrasil_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["idEmpresa"] = "Brasil";
            PopulaComboEmpresa(1);

            string url = Request.UrlReferrer.PathAndQuery.ToString().Substring(1, Request.UrlReferrer.PathAndQuery.ToString().Length - 1);
            DirecionaPagina(url);
        }

        protected void btnEUA_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["idEmpresa"] = "EUA";
            PopulaComboEmpresa(2);

            DirecionaPagina(Request.UrlReferrer.PathAndQuery.ToString().Substring(1, Request.UrlReferrer.PathAndQuery.ToString().Length - 1));
        }

        DropDownList GerarddlPesquisaGrid(int idEmpresa)
        {
            DropDownList ddlPesquisarGrid = new DropDownList();

            DataTable dtPesquisa = ExecutarDataTable2("sp_Select 'Flow_Empresa', @idPesquisa = " + idEmpresa + ", @idUsuario = " + Variaveis.idUsuario());

            foreach (DataRow row in dtPesquisa.Rows)
            {
                ddlPesquisarGrid.Items.Add(row["sDscEmpresa"].ToString());
                ddlPesquisarGrid.Items.Add(row["sDscEmpresaReduzida"].ToString());
            }

            return ddlPesquisarGrid;
        }

        void PopulaComboEmpresa(int idEmpresa)
        {
            DropDownList ddl = (DropDownList)FindControl_Recursivo(cphCorpo);
            DropDownList ddlPesquisaGrid = GerarddlPesquisaGrid(idEmpresa);
            GridView gridView = FindGridView_Recursivo(Page);

            if (ddl != null)
            {
                if (ddl.Visible == true)
                {
                    var Value = ddl.SelectedValue;

                    if (Value == "")
                        Popula_Combo(ddl, "sp_Select 'Flow_Empresa', @idPesquisa = " + idEmpresa + ", @idUsuario = " + Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
                    else
                        Popula_Combo(ddl, "sp_Select 'Flow_Empresa', @idPesquisa = " + idEmpresa + ", @idUsuario = " + Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, ddl.Items[0].Text, ddl.Items[0].Value);

                    try
                    {
                        ddl.SelectedValue = Value;
                    }
                    catch { }
                }

                if (ddl.Visible == true)
                {
                    List<string> selectedEmpresa = new List<string>();

                    foreach (ListItem item in ddl.Items)
                    {
                        if (item.Value != "0" && item.Value != "-1" && item.Value != "-2" && item.Value != "-3" && item.Value != "-4" && item.Value != "-5")
                            selectedEmpresa.Add(item.ToString());
                    }
                }

                if (ddl.Visible == true)
                {
                    if (gridView != null)
                    {
                        if (gridView.Visible == true)
                        {
                            string columnName = "Empresa";
                            int columnIndex = -1;

                            for (int i = 0; i < gridView.Columns.Count; i++)
                            {
                                if (gridView.Columns[i].HeaderText == columnName)
                                {
                                    columnIndex = i;
                                    break;
                                }
                            }

                            if (columnIndex != -1)
                            {
                                if (gridView.Columns[columnIndex].Visible == true)
                                {
                                    foreach (GridViewRow row in gridView.Rows)
                                    {
                                        DataControlField column = gridView.Columns[columnIndex];

                                        string sDcsEmpresa = "";
                                        if (column is BoundField)
                                            sDcsEmpresa = row.Cells[columnIndex].Text;
                                        else if (column is TemplateField)
                                            sDcsEmpresa = row.Cells[columnIndex].Text;
                                        else if (column is HyperLinkField)
                                            sDcsEmpresa = (row.Cells[columnIndex].Controls[0] as HyperLink).Text;

                                        bool isVisible = false;
                                        foreach (ListItem item in ddlPesquisaGrid.Items)
                                        {
                                            if (item.Text == sDcsEmpresa && item.Value != "0" && item.Value != "-1" && item.Value != "-2" && item.Value != "-3" && item.Value != "-4" && item.Value != "-5")
                                            {
                                                isVisible = true;
                                                break;
                                            }
                                        }

                                        row.Visible = isVisible;
                                    }

                                    int[] colunas = new int[gridView.Columns.Count];
                                    bool total = false;
                                    int formatacao = 1;

                                    if (gridView.FooterRow != null)
                                    {
                                        for (int i = 0; i < gridView.Columns.Count; i++)
                                        {
                                            string coluna = gridView.FooterRow.Cells[i].Text;

                                            if (!string.IsNullOrEmpty(coluna) && coluna != "&nbsp;" && decimal.TryParse(coluna.Replace("R$", "").Replace(",", ".").Trim(), out decimal valor))
                                            {
                                                colunas[i] = i;

                                                if (coluna.Contains(","))
                                                {
                                                    if (coluna.StartsWith("R"))
                                                        formatacao = 2;
                                                    else if (coluna.Split(',')[1].Length == 2)
                                                        formatacao = 4;
                                                    else if (coluna.Split(',')[1].Length == 3)
                                                        formatacao = 3;
                                                }
                                            }
                                            else if (coluna.Contains("Total"))
                                                total = true;
                                            else
                                                colunas[i] = -1;
                                        }
                                        int somar = colunas.Where(c => c != -1).Count();
                                        int coluna1 = 0;
                                        int coluna2 = 0;
                                        int coluna3 = 0;
                                        int coluna4 = 0;
                                        int coluna5 = 0;
                                        int coluna6 = 0;

                                        switch (somar)
                                        {
                                            case 1:
                                                coluna1 = colunas.Where(c => !c.Equals("-1")).First();
                                                Grid.SomarColunas(gridView, total, formatacao == 1 ? Grid.Formatação.Inteiro : formatacao == 2 ? Grid.Formatação.Moeda : formatacao == 3 ? Grid.Formatação.Peso : Grid.Formatação.Numero, coluna1);
                                                break;
                                            case 2:
                                                foreach (int i in colunas.Where(c => !c.Equals("-1")).ToList())
                                                {
                                                    if (coluna1 <= 0)
                                                        coluna1 = i;
                                                    else if (coluna2 <= 0)
                                                        coluna2 = i;
                                                }

                                                Grid.SomarColunas(gridView, total, formatacao == 1 ? Grid.Formatação.Inteiro : formatacao == 2 ? Grid.Formatação.Moeda : formatacao == 3 ? Grid.Formatação.Peso : Grid.Formatação.Numero, coluna1, coluna2);
                                                break;
                                            case 3:
                                                foreach (int i in colunas.Where(c => !c.Equals("-1")).ToList())
                                                {
                                                    if (coluna1 <= 0)
                                                        coluna1 = i;
                                                    else if (coluna2 <= 0)
                                                        coluna2 = i;
                                                    else if (coluna3 <= 0)
                                                        coluna3 = i;
                                                }

                                                Grid.SomarColunas(gridView, total, formatacao == 1 ? Grid.Formatação.Inteiro : formatacao == 2 ? Grid.Formatação.Moeda : formatacao == 3 ? Grid.Formatação.Peso : Grid.Formatação.Numero, coluna1, coluna2, coluna3);
                                                break;
                                            case 4:
                                                foreach (int i in colunas.Where(c => !c.Equals("-1")).ToList())
                                                {
                                                    if (coluna1 <= 0)
                                                        coluna1 = i;
                                                    else if (coluna2 <= 0)
                                                        coluna2 = i;
                                                    else if (coluna3 <= 0)
                                                        coluna3 = i;
                                                    else if (coluna4 <= 0)
                                                        coluna4 = i;
                                                }

                                                Grid.SomarColunas(gridView, total, formatacao == 1 ? Grid.Formatação.Inteiro : formatacao == 2 ? Grid.Formatação.Moeda : formatacao == 3 ? Grid.Formatação.Peso : Grid.Formatação.Numero, coluna1, coluna2, coluna3, coluna4);
                                                break;
                                            case 5:
                                                foreach (int i in colunas.Where(c => !c.Equals("-1")).ToList())
                                                {
                                                    if (coluna1 <= 0)
                                                        coluna1 = i;
                                                    else if (coluna2 <= 0)
                                                        coluna2 = i;
                                                    else if (coluna3 <= 0)
                                                        coluna3 = i;
                                                    else if (coluna4 <= 0)
                                                        coluna4 = i;
                                                    else if (coluna5 <= 0)
                                                        coluna5 = i;
                                                }

                                                Grid.SomarColunas(gridView, total, formatacao == 1 ? Grid.Formatação.Inteiro : formatacao == 2 ? Grid.Formatação.Moeda : formatacao == 3 ? Grid.Formatação.Peso : Grid.Formatação.Numero, coluna1, coluna2, coluna3, coluna4, coluna5);
                                                break;
                                            case 6:
                                                foreach (int i in colunas.Where(c => !c.Equals("-1")).ToList())
                                                {
                                                    if (coluna1 <= 0)
                                                        coluna1 = i;
                                                    else if (coluna2 <= 0)
                                                        coluna2 = i;
                                                    else if (coluna3 <= 0)
                                                        coluna3 = i;
                                                    else if (coluna4 <= 0)
                                                        coluna4 = i;
                                                    else if (coluna5 <= 0)
                                                        coluna5 = i;
                                                    else if (coluna6 <= 0)
                                                        coluna6 = i;
                                                }

                                                Grid.SomarColunas(gridView, total, formatacao == 1 ? Grid.Formatação.Inteiro : formatacao == 2 ? Grid.Formatação.Moeda : formatacao == 3 ? Grid.Formatação.Peso : Grid.Formatação.Numero, coluna1, coluna2, coluna3, coluna4, coluna5, coluna6);
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Control FindControl_Recursivo(Control root)
        {
            if (root.ID == "ddlEmpresa" || root.ID == "ddlidEmpresa" || root.ID == "ddlidEmpresas" || root.ID == "ddlEmpresas" || root.ID == "ddlempresas" || root.ID == "ddlempresa")
                return root;

            foreach (Control control in root.Controls)
            {
                Control foundControl = FindControl_Recursivo(control);

                if (foundControl != null)
                    return foundControl;
            }

            return null;
        }

        public static GridView FindGridView_Recursivo(Control root)
        {
            if (root is GridView)
                return (GridView)root;

            foreach (Control control in root.Controls)
            {
                GridView foundGridView = FindGridView_Recursivo(control);

                if (foundGridView != null)
                    return foundGridView;
            }

            return null;
        }

        #endregion

        #region | Script

        public void RegistraScript()
        {
            Dictionary<string, string> vParametrosAjax = new Dictionary<string, string> { { "sRecurso", "request.term" } };
            List<string> elementos = new List<string> { txtsPesquisa.ClientID, hddUrlRecurso.ClientID };

            StringBuilder sb = new StringBuilder();

            // Pesquisa - Menu
            {
                sb.AppendLine("$v192(function() {");
                sb.AppendLine("     $v192(\"#" + txtsPesquisa.ClientID + "\").addClass('custom-autocomplete').autocomplete({");
                sb.AppendLine("         source: function(request, response) {");
                sb.AppendLine("             $v192.ajax({");
                sb.AppendLine("                 url:'/API/Pagina_Ajax.aspx/GetRecurso',");
                sb.AppendLine("                 data: JSON.stringify({");

                foreach (KeyValuePair<string, string> item in vParametrosAjax) sb.AppendLine("'" + item.Key + "': " + item.Value + ", ");
                if (vParametrosAjax.Count > 0) sb.Length -= 2; // Remove a vírgula extra

                sb.AppendLine("                 }),");
                sb.AppendLine("                 dataType: \"json\",");
                sb.AppendLine("                 type: \"POST\",");
                sb.AppendLine("                 contentType: \"application/json; charset=utf-8\",");
                sb.AppendLine("                 success: function(data) {");
                sb.AppendLine("                     response($v192.map(data.d, function(item) {");
                sb.AppendLine("                         var parts = item.split('|');");
                sb.AppendLine("                         return {");
                sb.AppendLine("                             label: parts[0],");
                sb.AppendLine("                             url: parts[1],");

                int index = 0;
                foreach (string elementoID in elementos)
                {
                    sb.AppendLine(elementoID + ": item.split('|')[" + index + "],");
                    index++;
                }

                sb.Remove(sb.Length - 1, 1);

                sb.AppendLine("                         };");
                sb.AppendLine("                     }));"); // Adicione parênteses de fechamento para a função 'map'
                sb.AppendLine("                 },");
                sb.AppendLine("                 error: function(response) {");
                sb.AppendLine("                     alert(response.responseText);");
                sb.AppendLine("                 },");
                sb.AppendLine("                 failure: function(response) {");
                sb.AppendLine("                     alert(response.responseText);");
                sb.AppendLine("                 }");
                sb.AppendLine("             });");
                sb.AppendLine("         },");
                sb.AppendLine("         select: function(e, i) {");

                foreach (string elementoID in elementos) sb.AppendLine("$(\"#" + elementoID + "\").val(i.item." + elementoID + ");");

                sb.AppendLine("             var url = i.item.url;");
                sb.AppendLine("             if (url) window.open(url, '_blank');");
                sb.AppendLine("         },");
                sb.AppendLine("         minLength: 2");
                sb.AppendLine("     });");
                sb.AppendLine("});");
            }

            sb.AppendLine("");
            sb.AppendLine(File.ReadAllText(Server.MapPath("/App/JS/master.js")));

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScipt_Master", sb.ToString(), true);
        }

        #endregion
    }
}
