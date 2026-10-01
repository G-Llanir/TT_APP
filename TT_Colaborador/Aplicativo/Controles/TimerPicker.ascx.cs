using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TT_Flow.App.Controles
{
    public partial class TimerPicker : System.Web.UI.UserControl
    {
        public string LabelText = "";
        public string TimerPickerText = "";
        public bool ReadOnly = false;
        private string Horadata;

        public string Hora { get => txtbTimerPicker.Text; set => Horadata=value; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(ReadOnly)
            {
                //txtbTimerPicker.Visible = false;
                //lbl.Visible = false;

            }else
            {
                //lbl.Text = LabelText;
                //txtbTimerPicker.Text = TimerPickerText;

            }
            
        }
        public string testc ()
        {
            //string t = txtTimerPicker.Value.ToString();
            string x = txtbTimerPicker.Text.ToString();
            return "";
        }
    }
}