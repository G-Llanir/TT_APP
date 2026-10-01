using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static TT.FrameWork.BD;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Colaborador
{
    public partial class SiteMaster : MasterPage
    {
        StringBuilder AreaColaborador_Menu = new StringBuilder();
        readonly string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Area";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (HttpContext.Current.Session["AreaColaborador_Menu"] != null) AreaColaborador_Menu.AppendLine(HttpContext.Current.Session["AreaColaborador_Menu"].ToString());
            }
            catch { }

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request["sChave"])) Usuario.Login_Sessao(Request["sChave"], Request.Url.AbsoluteUri.Split('?')[0]);

                ValidarSessao(true);

                try
                {
                    if (AreaColaborador_Menu.Length < 1) Gera_MenuDinamico();
                    else
                    {
                        if (AreaColaborador_Menu.Replace("\\r\\n", "").ToString().Trim().Length > 0) ltrMaster.Text = AreaColaborador_Menu.ToString();
                        else throw new Exception("Não há recursos disponíveis para seu usuário");
                    }
                }
                catch (Exception ex)
                {
                    ltrMaster.Text = string.Format("<h2><b>{0}</b></h2>", ex.Message);
                }

                AtualizarParametrosSistema();
                Pesquisar();

                Label lblTituloPagina = (Label)MainContent.FindControl("lblTituloPagina");
                if (lblTituloPagina != null) litComplemento.Text = " - " + lblTituloPagina.Text + " - ";
                else litComplemento.Text = " - ";
            }
            else ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Page);

            ltrNome_TT.Text = "Tec and Tec";
            ltrNome_Sistema.Text = "Área do Colaborador";

            RegistraScript();
        }

        protected void Pesquisar()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR-USUARIO" },
                { "@idUsuarioIntegrado", Variaveis.idUsuario() }
            };
            DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

            if (ValidarDataSet(ds))
            {
                if (string.IsNullOrEmpty(RETORNO.DATASET(ds, "vbAssinatura")) && !Request.RawUrl.Contains("Colaborador_Dados")) DirecionaPagina("Aplicativo/Paginas/Colaboradores/Colaborador_Dados.aspx?msg=1");

                divMarcaDagua.Visible = bDev;

                ltrNome.Text = RETORNO.DATASET(ds, "sDscUsuario");

                PesquisarColaborador(RETORNO.DATASET(ds, "idColaborador"));
                carregaimgColaborador(RETORNO.DATASET(ds, "idColaborador"));
            }
            else DirecionaPagina("Aplicativo/PermissaoNegada.aspx?UsuarioVinculado=0");
        }

        protected void PesquisarColaborador(string idColaborador)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR-DADOS" },
                { "@idColaborador", idColaborador }
            };
            DataSet ds = ExecutarDataSet(sProcedure, vParametros);

            if (ValidarDataSet(ds)) ltrEmail.Text = RETORNO.DATASET(ds, 0, "sEmail");
        }

        private void Gera_MenuDinamico()
        {
            AreaColaborador_Menu.Clear();
            AreaColaborador_Menu.AppendLine(ExecutarDataTable(sProcedure, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_MENU_DINAMICO" }, { "@idUsuario", Variaveis.idUsuario() } }).Rows[0][0].ToString());
            HttpContext.Current.Session["AreaColaborador_Menu"] = AreaColaborador_Menu.ToString();

            ltrMaster.Text = AreaColaborador_Menu.ToString();

            AtualizarParametrosSistema();
        }

        protected void lnkRecarregaMenu_Click(object sender, EventArgs e) { ValidarSessao(); AtualizarParametrosSistema(); Gera_MenuDinamico(); }

        public static void AtualizarParametrosSistema()
        {
            SqlDataReader dr = ExecutarDataReader("sp_Flow_ConsultaDadosSistema '" + FrameWork.Identity.Variaveis.idUsuario() + "'");

            if (dr != null)
            {
                while (dr.Read())
                {
                    FrameWork.Identity.sVersao = dr["sVersao"].ToString();
                    FrameWork.Identity.nQtdMensagens = dr["nQtdMensagens"].ToString();
                    FrameWork.Identity.nQtdMensagens_NaoLidas = dr["nQtdMensagens_NaoLidas"].ToString();
                    FrameWork.Identity.nQtdMensagens_Lidas = dr["nQtdMensagens_Lidas"].ToString();

                    HttpContext.Current.Session["sPermissao"] = dr["sPermissao"].ToString();
                }

                dr.Close();
            }
        }

        protected void carregaimgColaborador(string idObjeto)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_IMAGEM" },
                { "@idTipoArquivo", "40" },
                { "@idObjeto", idObjeto }
            };
            DataTable dtPesquisa = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dtPesquisa.Rows.Count > 0)
            {
                DataRow imgBd = dtPesquisa.Rows[0];
                string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);
                imgColaborador.ImageUrl = string.IsNullOrEmpty(imgUrl) || string.IsNullOrEmpty(Convert.ToBase64String((byte[])imgBd["vbArquivo"])) ? "~/Aplicativo/img/defaultUser.png" : imgUrl;
                imgColaborador.Visible = true;
            }
            else imgColaborador.ImageUrl = "~/Aplicativo/img/defaultUser.png";
        }

        #region | Script

        protected void RegistraScript()
        {
            Scripts.Aplica_TooltipPersonalizado(Page, "tooltip_top", "top", true);
            Scripts.Aplica_TooltipPersonalizado(Page, "tooltip_right", "right", true);
            Scripts.Aplica_TooltipPersonalizado(Page, "tooltip_bottom", "bottom", true);
            Scripts.Aplica_TooltipPersonalizado(Page, "tooltip_left", "left", true);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("function MudarBotao_Consulta(botao) { $(botao).val('Buscando...'); }");
            sb.AppendLine("");
            sb.AppendLine("function initializeChosen() { $('.Caixa_Selecao').chosen({ width: '100%' }); }");
            sb.AppendLine("");
            sb.AppendLine("Sys.WebForms.PageRequestManager.getInstance().add_endRequest(initializeChosen);");
            sb.AppendLine("");
            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("");
            sb.AppendLine("     var prm = Sys.WebForms.PageRequestManager.getInstance();");
            sb.AppendLine("     var loadingTimer;");
            sb.AppendLine("");
            sb.AppendLine("     prm.add_beginRequest(function () {");
            sb.AppendLine("         loadingTimer = setTimeout(function () {");
            sb.AppendLine("             $('[id*=div_Loading]').show();");
            sb.AppendLine("             $('[id*=div_img]').show();");
            sb.AppendLine("         }, 500);");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     prm.add_endRequest(function () {");
            sb.AppendLine("         clearTimeout(loadingTimer);");
            sb.AppendLine("         $('[id*=div_Loading]').hide();");
            sb.AppendLine("         $('[id*=div_img]').hide();");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $(window).on('beforeunload', function () {");
            sb.AppendLine("         loadingTimer = setTimeout(function () {");
            sb.AppendLine("             $('[id*=div_Loading]').show();");
            sb.AppendLine("             $('[id*=div_img]').show();");
            sb.AppendLine("         }, 500);");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('.cmdMenuPaginas').on('click', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $('#menuPaginas').toggleClass('menuPaginas_Ativo');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('.lnkPaginas').on('click', function () {");
            sb.AppendLine("         $(this).find('a').get(0).click();");
            sb.AppendLine("     });");
            sb.AppendLine("     $('.lnkPaginas a').on('click', function (e) {");
            sb.AppendLine("        e.stopPropagation();");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('.mascara-int').mask('999.999.999', { reverse: true });");
            sb.AppendLine("     $('.mascara-decimal').mask('999.999.999,99', { reverse: true });");
            sb.AppendLine("     $('.mascara-decimal-4').mask('999.999.999,9999', { reverse: true });");
            sb.AppendLine("");
            sb.AppendLine("     initializeChosen();");
            sb.AppendLine("     getGeolocation();");
            sb.AppendLine("     setInterval(getGeolocation, 60000);"); // Atualiza a localização a cada 1 minuto
            sb.AppendLine("");
            sb.AppendLine("     setInterval(function() {");
            sb.AppendLine("         $.ajax({");
            sb.AppendLine("             url: '/API/WebMethods.asmx/Timer_Atualizar',");
            sb.AppendLine("             type: 'POST',");
            sb.AppendLine("             contentType: 'application/json; charset=utf-8',");
            sb.AppendLine("             data: '{}',");
            sb.AppendLine("             dataType: 'json',");
            sb.AppendLine("             success: function (response) { console.debug('Timer_Atualizar'); },");
            sb.AppendLine("             error: function (xhr) { console.warn('Erro no Timer_Atualizar:', xhr.responseText); }");
            sb.AppendLine("         });");
            sb.AppendLine("     }, 300000);");
            sb.AppendLine("");
            sb.AppendLine("     $('.form-range').each(function () {");
            sb.AppendLine("         const $range = $(this);");
            sb.AppendLine("         const min = $range.attr('min') || 0;");
            sb.AppendLine("         const max = $range.attr('max') || 100;");
            sb.AppendLine("         const value = $range.val() || min;");
            sb.AppendLine("         const step = $range.attr('step') || 1;");
            sb.AppendLine("         const progress = (value - min) * 100 / (max - min);");
            sb.AppendLine("         $range.css({");
            sb.AppendLine("             '--min': min,");
            sb.AppendLine("             '--max': max,");
            sb.AppendLine("             '--value': value,");
            sb.AppendLine("             '--step': step,");
            sb.AppendLine("             '--progress': `${progress}%`");
            sb.AppendLine("         });");
            sb.AppendLine("     });");
            sb.AppendLine("     $('.form-range').on('input change', function () {");
            sb.AppendLine("         const $this = $(this);");
            sb.AppendLine("         $this.css({ '--value': this.value || $this.val(), '--progress': `${((this.value || $this.val()) - ($this.attr('min') || 0)) * 100 / (($this.attr('max') || 100) - ($this.attr('min') || 0))}%` });");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("});");
            sb.AppendLine("");
            sb.AppendLine("function getGeolocation() {");
            sb.AppendLine("     if (navigator.geolocation && $('[id*=hddLatitude]').length && $('[id*=hddLongitude]').length) {");
            sb.AppendLine("         navigator.geolocation.getCurrentPosition(sendPositionToServer, showError);");
            sb.AppendLine("     }");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("function sendPositionToServer(position) {");
            sb.AppendLine("     $('[id*=hddLatitude]').val(position.coords.latitude);");
            sb.AppendLine("     $('[id*=hddLongitude]').val(position.coords.longitude);");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("function showError(error) {");
            sb.AppendLine("     switch (error.code) {");
            sb.AppendLine("         case error.PERMISSION_DENIED:");
            sb.AppendLine("             console.log('Usuário negou a solicitação de geolocalização!');");
            sb.AppendLine("             break;");
            sb.AppendLine("         case error.POSITION_UNAVAILABLE:");
            sb.AppendLine("             console.log('Informação da localização não está disponível!');");
            sb.AppendLine("             break;");
            sb.AppendLine("         case error.TIMEOUT:");
            sb.AppendLine("             console.log('A solicitação para obter localização expirou!');");
            sb.AppendLine("             break;");
            sb.AppendLine("         case error.UNKNOWN_ERROR:");
            sb.AppendLine("             console.log('Ocorreu um erro desconhecido!');");
            sb.AppendLine("             break;");
            sb.AppendLine("     }");
            sb.AppendLine("}");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScript_Master", sb.ToString(), true);
        }

        #endregion
    }
}