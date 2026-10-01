using System;
using System.Text;
using System.Web.UI;

namespace TT_Flow.App.Controles
{
    public partial class SwitchAtivo : UserControl
    {
        private string _sNao;
        private string _sSim;
        public string sNao { get { return string.IsNullOrEmpty(_sNao) ? "\"Não\"" : _sNao; } set { _sNao = $"\"{value}\""; } }
        public string sSim { get { return string.IsNullOrEmpty(_sSim) ? "\"Sim\"" : _sSim; } set { _sSim = $"\"{value}\""; } }

        public string sPosicao_Nao = "45%";
        public string sPosicao_Sim = "170%";
        public string sTamanho_Switch = "60px";
        public string sCorFundo_Nao = "red";
        public string sCorFundo_Sim = "green";
        public string sCorTexto_Nao = "white";
        public string sCorTexto_Sim = "white";
        public string sPostBack_Switch = string.Empty;
        public string sFunctionScript_Switch = string.Empty;

        protected override void OnPreRender(EventArgs e)
        {
            hddSwitch.Value = NormalizarSN(hddSwitch.Value);
            if (string.IsNullOrEmpty(hddSwitchBloquear.Value))
                hddSwitchBloquear.Value = "N";

            RegistrarScriptSwitch();

            base.OnPreRender(e);
        }

        public string CheckboxCheckedAttribute => NormalizarSN(hddSwitch.Value) == "S" ? @" checked=""checked""" : string.Empty;

        static string NormalizarSN(string valor) => (valor ?? string.Empty).Trim() == "S" ? "S" : "N";

        public void Definir(string sSituacao, string sTitulo, string sBloquear)
        {
            hddSwitch.Value = NormalizarSN(sSituacao);
            lblTitulo.InnerText = sTitulo != "" ? sTitulo : "Ativo";
            lblTitulo.Visible = !string.IsNullOrEmpty(lblTitulo.InnerText);
            hddSwitchBloquear.Value = sBloquear != "" ? sBloquear : "N";
        }

        public string Recuperar() => NormalizarSN(hddSwitch.Value);

        public void MudarTitulo(string sTitulo) { lblTitulo.InnerText = sTitulo; lblTitulo.Visible = !string.IsNullOrEmpty(lblTitulo.InnerText); }

        public void BloquearEdicao(bool ativo) => hddSwitchBloquear.Value = ativo ? "S" : "N";

        public void RegistrarScriptSwitch()
        {
            string idSwitchClientID = ClientID + "_idSwitch";
            string hddSwitchClientID = hddSwitch.ClientID;

            StringBuilder script = new StringBuilder();
            script.AppendLine("(function (idSwitchClientID, hddSwitchClientID) {");
            script.AppendLine("    function normalizarSN(valor) { return (valor || '').trim() === 'S' ? 'S' : 'N'; }");
            script.AppendLine("    function sincronizarSwitch() {");
            script.AppendLine("        var idSwitch = document.getElementById(idSwitchClientID);");
            script.AppendLine("        var hddSwitch = document.getElementById(hddSwitchClientID);");
            script.AppendLine("        if (!idSwitch || !hddSwitch) return;");
            script.AppendLine("        hddSwitch.value = normalizarSN(hddSwitch.value);");
            script.AppendLine("        idSwitch.checked = hddSwitch.value === 'S';");
            script.AppendLine("    }");
            script.AppendLine("    function aplicarSwitchNoHidden() {");
            script.AppendLine("        var idSwitch = document.getElementById(idSwitchClientID);");
            script.AppendLine("        var hddSwitch = document.getElementById(hddSwitchClientID);");
            script.AppendLine("        if (!idSwitch || !hddSwitch) return;");
            script.AppendLine("        hddSwitch.value = idSwitch.checked ? 'S' : 'N';");
            if (!string.IsNullOrEmpty(sFunctionScript_Switch)) script.AppendLine($"        {sFunctionScript_Switch}");
            if (!string.IsNullOrEmpty(sPostBack_Switch)) script.AppendLine($"        __doPostBack('{sPostBack_Switch}', '');");
            script.AppendLine("    }");
            script.AppendLine("    function initSwitch() {");
            script.AppendLine("        var idSwitch = document.getElementById(idSwitchClientID);");
            script.AppendLine("        var hddSwitch = document.getElementById(hddSwitchClientID);");
            script.AppendLine("        if (!idSwitch || !hddSwitch) return;");
            script.AppendLine("        sincronizarSwitch();");
            script.AppendLine("        if (!idSwitch.getAttribute('data-switch-init')) {");
            script.AppendLine("            idSwitch.setAttribute('data-switch-init', '1');");
            script.AppendLine("            idSwitch.addEventListener('change', aplicarSwitchNoHidden);");
            script.AppendLine("            var form = idSwitch.closest('form');");
            script.AppendLine("            if (form) form.addEventListener('submit', aplicarSwitchNoHidden);");
            script.AppendLine("        }");
            script.AppendLine("    }");
            script.AppendLine("    Sys.Application.add_load(initSwitch);");
            script.AppendLine("    if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {");
            script.AppendLine("        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(initSwitch);");
            script.AppendLine("    }");
            script.AppendLine("})('" + idSwitchClientID + "', '" + hddSwitchClientID + "');");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), $"{idSwitchClientID}_switchScript", script.ToString(), true);
        }
    }
}