namespace TT_Flow.App.Controles
{
    public partial class PainelAtualizacao : System.Web.UI.UserControl
    {
        #region | Métodos publicos

        private string _sLabel = "Última atualização em";

        public string TextoLabel
        {
            get { return _sLabel; }
            set { _sLabel = value; }
        }

        #endregion

        #region | Inicialização dos Componentes
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
            lblTexto.Text = TextoLabel;
        }

        public void Atualizar(string dtAtualizacao, string sDscUsuario)
        {
            lbldtAtualizacao.Text = dtAtualizacao;
            lblsDscUsuarioAtualizacao.Text = sDscUsuario;
            lblTexto.Text = TextoLabel;
        }

        public void Personalizar(string sTexto)
        {
            TextoLabel = sTexto;
            lblTexto.Text = TextoLabel;
            lbldtAtualizacao.Text = string.Empty;
            lblsDscUsuarioAtualizacao.Text = string.Empty;
            lblPor.Text = string.Empty;
        }
    }
}