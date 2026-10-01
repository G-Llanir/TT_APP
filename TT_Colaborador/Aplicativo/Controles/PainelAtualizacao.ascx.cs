using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TT_Colaborador.Aplicativo.Controles
{
    public partial class PainelAtualizacao : System.Web.UI.UserControl
    {
        #region |Métodos publicos

        private string _DataAtualizacao;
        private string _AtualizadoPor;
        private string _sLabel = "Última atualização em";

        public string DataAtualizacao
        {
            get { return _DataAtualizacao; }
            set { _DataAtualizacao = value; }
        }

        public string AtualizadoPor
        {
            get { return _AtualizadoPor; }
            set { _AtualizadoPor = value; }
        }
        public string TextoLabel
        {
            get { return _sLabel; }
            set { _sLabel = value; }
        }

        #endregion

        #region |Inicialização dos Componentes
        [System.Diagnostics.DebuggerStepThrough()]

        private void InitializeComponent()
        {
        }

        private void Page_Init(System.Object sender, System.EventArgs e)
        {
            InitializeComponent();
        }

        #endregion


        private void Page_Load(System.Object sender, System.EventArgs e)
        {

            // lbldtAtualizacao.Text = DataAtualizacao;
            //lblsDscUsuarioAtualizacao.Text = AtualizadoPor;
            lblTexto.Text = TextoLabel;
        }


        public PainelAtualizacao()
        {
            Load += Page_Load;
            Init += Page_Init;
        }

        public void Atualizar(string dtAtualizacao, string sDscUsuario)
        {
            lbldtAtualizacao.Text = dtAtualizacao;
            lblsDscUsuarioAtualizacao.Text = sDscUsuario;
            lblTexto.Text = TextoLabel;
        }


    }
}