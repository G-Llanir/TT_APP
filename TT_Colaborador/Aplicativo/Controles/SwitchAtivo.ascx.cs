using System;
using System.Text;
using System.Web.UI;

namespace TT_Colaborador.Aplicativo.Controles
{
    public partial class SwitchAtivo : UserControl
    {
        private string _sNao;
        private string _sSim;
        public string sNao { get { return string.IsNullOrEmpty(_sNao) ? "\"Não\"" : _sNao; } set { _sNao = "\"" + value + "\""; } }
        public string sSim { get { return string.IsNullOrEmpty(_sSim) ? "\"Sim\"" : _sSim; } set { _sSim = "\"" + value + "\""; } }

        public string sPosicao_Nao = "45%";
        public string sPosicao_Sim = "170%";
        public string sTamanho_Switch = "60px";
        public string sCorFundo_Nao = "red";
        public string sCorFundo_Sim = "green";
        public string sCorTexto_Nao = "white";
        public string sCorTexto_Sim = "white";
        public string sPostBack_Switch = string.Empty;
        public string sFunctionScript_Switch = string.Empty;

        protected void Page_Load(object sender, EventArgs e) => RegistrarScriptSwitch();

        public void Definir(string sSituacao, string sTitulo, string sBloquear)
        {
            hddSwitch.Value = sSituacao;
            lblTitulo.InnerText = sTitulo;
            lblTitulo.Visible = !string.IsNullOrEmpty(lblTitulo.InnerText);
            hddSwitchBloquear.Value = sBloquear != "" ? sBloquear : "N";
        }

        public string Recuperar() => hddSwitch.Value;

        public void MudarTitulo(string sTitulo) { lblTitulo.InnerText = sTitulo; lblTitulo.Visible = !string.IsNullOrEmpty(lblTitulo.InnerText); }

        public void BloquearEdicao(bool ativo) => hddSwitchBloquear.Value = ativo ? "S" : "N";

        public void RegistrarScriptSwitch()
        {
            string idSwitchClientID = ClientID + "_idSwitch";
            string hddSwitchClientID = hddSwitch.ClientID;

            StringBuilder script = new StringBuilder();
            script.AppendLine("(function (idSwitchClientID, hddSwitchClientID) {");
            script.AppendLine("     Sys.Application.add_load(function () {");
            script.AppendLine("        var idSwitch = document.getElementById(idSwitchClientID);");
            script.AppendLine("        var hddSwitch = document.getElementById(hddSwitchClientID);");
            script.AppendLine("        if (idSwitch) {");
            script.AppendLine("            idSwitch.addEventListener('change', function () {");
            script.AppendLine("                if (idSwitch.checked) {");
            script.AppendLine("                    hddSwitch.value = 'S';");
            script.AppendLine("                } else {");
            script.AppendLine("                    hddSwitch.value = 'N';");
            script.AppendLine("                }");

            if (!string.IsNullOrEmpty(sFunctionScript_Switch)) script.AppendLine($"           {sFunctionScript_Switch}");
            if (!string.IsNullOrEmpty(sPostBack_Switch)) script.AppendLine($"            __doPostBack('{sPostBack_Switch}', '_blank');");

            script.AppendLine("            });");
            script.AppendLine("        }");
            script.AppendLine("    });");
            script.AppendLine("})('" + idSwitchClientID + "', '" + hddSwitchClientID + "');");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), $"{idSwitchClientID}_switchScript", script.ToString(), true);
        }
    }
}