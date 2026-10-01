using System;
using System.Web.UI;

namespace TT_Flow.App.Controles
{
    public partial class UpdatePanel_Loading : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void DefinirUpdatePanel(UpdatePanel upd)
        {
            UpProgress.AssociatedUpdatePanelID = upd.ID;
        }
    }
}