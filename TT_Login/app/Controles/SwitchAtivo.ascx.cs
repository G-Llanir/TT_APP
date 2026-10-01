using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TT_Flow.App.Controles
{
    public partial class SwitchAtivo : System.Web.UI.UserControl
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarScriptSwitch();
        }
       
        public void Definir (string sSituacao, string sTitulo, string sBloquear)
        {
            hddSwitch.Value = sSituacao;
            lblTitulo.Text = sTitulo != ""? sTitulo : "Ativo";
            hddSwitchBloquear.Value = sBloquear != "" ? sBloquear : "N";
        }

        public string Recuperar ()
        {            
            return hddSwitch.Value;
        }

        public void MudarTitulo (string sTitulo)
        {
            lblTitulo.Text = sTitulo;
        }

        public void BloquearEdicao (bool ativo)
        {
            if (ativo)
            {
                hddSwitchBloquear.Value = "S";
            }
            else 
            {
                hddSwitchBloquear.Value = "N";
            }
        }

        private void RegistrarScriptSwitch()
        {            
            string idSwitchClientID = this.ClientID + "_idSwitch"; 
            string hddSwitchClientID = hddSwitch.ClientID; 

            StringBuilder script = new StringBuilder();
            script.AppendLine("(function (idSwitchClientID, hddSwitchClientID) {");
            script.AppendLine("    document.addEventListener('DOMContentLoaded', function () {");
            script.AppendLine("        var idSwitch = document.getElementById(idSwitchClientID);");
            script.AppendLine("        var hddSwitch = document.getElementById(hddSwitchClientID);");
            script.AppendLine("        console.log('hddSwitch element:', hddSwitch);");
            script.AppendLine("        idSwitch.addEventListener('change', function () {");
            script.AppendLine("            if (idSwitch.checked) {");
            script.AppendLine("                hddSwitch.value = 'S';");
            script.AppendLine("            } else {");
            script.AppendLine("                hddSwitch.value = 'N';");
            script.AppendLine("            }");
            script.AppendLine("            console.log(hddSwitch.value);");
            script.AppendLine("        });");
            script.AppendLine("    });");
            script.AppendLine("})('" + idSwitchClientID + "', '" + hddSwitchClientID + "');");

            ScriptManager.RegisterStartupScript(this, this.GetType(), idSwitchClientID + "_switchScript", script.ToString(), true);
        }
    }
}