using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using static TT.FrameWork.BD;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Controles
{
    public partial class Pesquisa_Parceiros : System.Web.UI.UserControl
    {
        public bool Enabled
        {
            get { return txtParceiro_sCPF_CNPJ.Enabled && txtParceiro_sRazaoSocial.Enabled; }
            set
            {
                txtParceiro_sCPF_CNPJ.Enabled = value;
                txtParceiro_sRazaoSocial.Enabled = value;
            }
        }

        /// <summary>
        /// SsTipoParceiro é o campo que deve ser utilizado para configurar o filtro dos parceiros, de acordo com a tabela tbl_Flow_Clientes_Tipo_Parceiro
        /// </summary>
        public int idParceiro { get => ObterIDCLient(); set => DefinirParceiro(value); }
        public string SCnpj_CPF { get => txtParceiro_sCPF_CNPJ.Text; set => txtParceiro_sCPF_CNPJ.Text = value; }
        public string SRazaoSocial { get => txtParceiro_sRazaoSocial.Text; set => txtParceiro_sRazaoSocial.Text = value; }

        //Thiago Rodrigues - 20/08/2024
        public string SEmail { get; set; }
        public string STelefone { get; set; }
        public string SRgIE { get; set; }

        public string SsTipoParceiro { get => hddClientes_sTipoParceiro.Value; set => hddClientes_sTipoParceiro.Value = value; }
        public string sErro { get; set; }
        public delegate void CNPJ_CPF_x_RazaoSocialEvent(object sender, EventArgs e);
        public event CNPJ_CPF_x_RazaoSocialEvent EventoPesquisaParceiro;

        //Thiago Rodrigues - 23/09/2024
        public event EventHandler ParceiroAlterado;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegistrarScriptFormatarCpfCnpj();

                txtParceiro_sCPF_CNPJ.MaxLength = 18;
            }
            RegistrarScriptPesquisarItens();
        }

        //public void Focus()
        //{
        //    txtParceiro_sCPF_CNPJ.Focus();
        //}
        public void LimparCamposParceiro()
        {
            txtParceiro_sCPF_CNPJ.Text = string.Empty;
            txtParceiro_sRazaoSocial.Text = string.Empty;
        }

        private void RegistrarScriptFormatarCpfCnpj()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("function formatarCpfCnpj(valor) {");
            sb.AppendLine("    valor = valor.replace(/\\D/g, '');");
            sb.AppendLine("    if (valor.length <= 11) {");
            sb.AppendLine("        valor = valor.replace(/(\\d{3})(\\d)/, '$1.$2');");
            sb.AppendLine("        valor = valor.replace(/(\\d{3})(\\d)/, '$1.$2');");
            sb.AppendLine("        valor = valor.replace(/(\\d{3})(\\d{1,2})$/, '$1-$2');");
            sb.AppendLine("    } else {");
            sb.AppendLine("        valor = valor.replace(/^(\\d{2})(\\d)/, '$1.$2');");
            sb.AppendLine("        valor = valor.replace(/^(\\d{2})\\.(\\d{3})(\\d)/, '$1.$2.$3');");
            sb.AppendLine("        valor = valor.replace(/\\.(\\d{3})(\\d)/, '.$1/$2');");
            sb.AppendLine("        valor = valor.replace(/(\\d{4})(\\d)/, '$1-$2');");
            sb.AppendLine("    }");
            sb.AppendLine("    return valor;");
            sb.AppendLine("}");

            sb.AppendLine("$(document).ready(function() {");
            sb.AppendLine("    $('#" + txtParceiro_sCPF_CNPJ.ClientID + "').keyup(function() {");
            sb.AppendLine("        this.value = formatarCpfCnpj(this.value);");
            sb.AppendLine("    });");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "scriptFormatarCpfCnpj", sb.ToString(), true);
        }
        protected int ObterIDCLient()
        {
            if (hddClientes_idParceiro.Value != "")
            {
                return int.Parse(hddClientes_idParceiro.Value);
            }
            else
                return 0;
        }

        public void ModificaTamanhoCampos(int nDivCnpj, int nDivRazao)
        {
            divCnpjCpf.Attributes["class"] = $"col-lg-{nDivCnpj}";
            divRazao.Attributes["class"] = $"col-lg-{nDivRazao}";
        }
        protected void DefinirParceiro(int idParceiro)
        {
            string sErro = "";
            SqlDataReader sdr = null;
            try
            {
                if (idParceiro == 0)
                {
                    return;
                }

                sdr = BD.ExecutarDataReader("EXEC sp_Manipula_tbl_Flow_Clientes @sFuncao = 'CONSULTAR', @idParceiro=" + idParceiro + ",@sSituacao='T'");
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        txtParceiro_sCPF_CNPJ.Text = sdr["sCPF_CNPJ"].ToString();
                        txtParceiro_sRazaoSocial.Text = sdr["sRazaoSocial"].ToString();
                        hddClientes_idParceiro.Value = sdr["idParceiro"].ToString();
                    }
                }
                else
                {
                    sErro = string.Format("Parceiro (ID: {0}) Não localizado!", idParceiro.ToString());
                }
            }
            catch
            {
                sErro = "BD: Erro ao Consultar Parceiro!";
                return;
            }
            finally
            {
                if (sdr != null)
                    sdr.Close();
            }

            if (sErro != "")
            {
                MensagemPagina.MostraMensagem_Erro(sErro);
            }
        }
        //Thiago Rodrigues - 20/08/2024
        //Trazer Informações de Contato do Cliente.
        public void GetDetalhesParceiros(string idParceiro)
        {
            var sErro = "";
            if (idParceiro != "0")
            {
                DataSet dsPesquisa;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                vParametros.Add("@idCliente", idParceiro);
                dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    SEmail = RETORNO.DATASET(dsPesquisa, 0, "sEmail");
                    STelefone = RETORNO.DATASET(dsPesquisa, 0, "sTelefone");
                    SRgIE = RETORNO.DATASET(dsPesquisa, 0, "sRG_IE");
                }
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum dado foi Encontrado.", false);
                return;
            }
        }
        public delegate void MeuDelegate(string mensagem);
        public event MeuDelegate MeuEvento;

        //Thiago Rodrigues - 23/08/2024
        protected void OnParceiroAlterado(EventArgs e)
        {
            ParceiroAlterado?.Invoke(this, e); // Invoca o evento quando o parceiro for alterado
        }
        protected void txtParceiro_sRazaoSocial_TextChanged(object sender, EventArgs e)
        {
            //Thiago Rodrigues - 20/08/2024
            GetDetalhesParceiros(hddClientes_idParceiro.Value);
            MeuEvento?.Invoke("Clique no botão do UserControl.");
            //Thiago Rodrigues - 23/08/2024
            OnParceiroAlterado(EventArgs.Empty); // Dispara o evento

            if (string.IsNullOrEmpty(txtParceiro_sCPF_CNPJ.Text))
            {

                txtParceiro_sCPF_CNPJ.Text = "";
                string sIdTipoParceiro = "";
                try
                {
                    sIdTipoParceiro = hddClientes_sTipoParceiro.Value.ToString();
                }
                catch
                {
                    sIdTipoParceiro = "0;";
                }

                SqlDataReader sdr = BD.ExecutarDataReader("EXEC sp_Manipula_tbl_Flow_Clientes @sFuncao = 'CONSULTAR', @sRazaoSocial='" + txtParceiro_sRazaoSocial.Text + "'" + ",@sidTipoParceiro='" + sIdTipoParceiro + "'");

                try
                {
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            txtParceiro_sCPF_CNPJ.Text = sdr["sCPF_CNPJ"].ToString();
                            hddClientes_idParceiro.Value = sdr["idParceiro"].ToString();


                        }
                    }
                    else
                    {

                        MensagemPagina.MostraMensagem_Erro("Razão Social não localizado");
                    }
                }
                catch
                {
                    sErro = "Não há dados";
                    return;
                }
                finally
                {
                    if (sdr != null)
                        sdr.Close();
                }
            }
            OnControleEvento();

        }
        private string RemoverPontuacao(string texto)
        {
            return new string(texto.Where(char.IsDigit).ToArray());
        }

        protected void txtParceiro_sCPF_CNPJ_TextChanged(object sender, EventArgs e)
        {
            //Thiago Rodrigues - 20/08/2024
            GetDetalhesParceiros(hddClientes_idParceiro.Value);
            MeuEvento?.Invoke("Clique no botão do UserControl.");
            //Thiago Rodrigues - 23/08/2024
            OnParceiroAlterado(EventArgs.Empty); // Dispara o evento

            if (sender is TextBox)
            {
                TextBox txb = (TextBox)sender;
                string id = txb.ID;
                if (!string.IsNullOrEmpty(txtParceiro_sRazaoSocial.Text) && id == txtParceiro_sRazaoSocial.ID.ToString())
                    return;
            }
            txtParceiro_sRazaoSocial.Text = "";
            string sIdTipoParceiro = "";
            string cnpjCpfSemPontuacao = RemoverPontuacao(txtParceiro_sCPF_CNPJ.Text);
            try
            {
                sIdTipoParceiro = hddClientes_sTipoParceiro.Value.ToString();
            }
            catch
            {
                sIdTipoParceiro = "0;";
            }


            SqlDataReader sdr = BD.ExecutarDataReader("EXEC sp_Manipula_tbl_Flow_Clientes @sFuncao = 'Flow_Parceiros', @sCPF_CNPJ='" + cnpjCpfSemPontuacao + "', @sidTipoParceiro='" + sIdTipoParceiro + "'");


            try
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        txtParceiro_sRazaoSocial.Text = sdr["sRazaoSocial"].ToString();
                        hddClientes_idParceiro.Value = sdr["idParceiro"].ToString();
                    }
                }
                else
                {

                    MensagemPagina.MostraMensagem_Erro("CNPJ não localizado");
                }
            }
            catch
            {
                sErro = "Não há dados";
                return;
            }
            finally
            {
                if (sdr != null)
                    sdr.Close();
            }
            OnControleEvento();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AtivaCampos()
        {
            txtParceiro_sCPF_CNPJ.Attributes.Remove("disabled");
            txtParceiro_sRazaoSocial.Attributes.Remove("disabled");
        }
        public void DesativaCampos()
        {
            txtParceiro_sCPF_CNPJ.Attributes.Add("disabled", "disabled");
            txtParceiro_sRazaoSocial.Attributes.Add("disabled", "disabled");
        }
        public void RegistrarScriptPesquisarItens()
        {

            string _txtnParceiros_sRazaoSocial = txtParceiro_sRazaoSocial.ClientID;
            string _txtnParceiro_sCPF_CNPJ = txtParceiro_sCPF_CNPJ.ClientID;
            string _hddClientes_idParceiro = hddClientes_idParceiro.ClientID;
            string _hddClientes_sNomeFantasia = hddClientes_sNomeFantasia.ClientID;
            string _hddClientes_sidTipoParceiro = hddClientes_sTipoParceiro.ClientID;

            List<string> list = new List<string>
            {
                _txtnParceiros_sRazaoSocial,
                _hddClientes_idParceiro,
                _txtnParceiro_sCPF_CNPJ
            };

            ScriptsPagina(_txtnParceiros_sRazaoSocial, "ScriptCompletar", list, this.Page);
        }
        public void LimparCampos()
        {
            txtParceiro_sCPF_CNPJ.Text = "";
            txtParceiro_sRazaoSocial.Text = "";
            hddClientes_idParceiro.Value = "";
            hddClientes_sNomeFantasia.Value = "";
            hddClientes_sTipoParceiro.Value = "";
        }

        public void ConfigurarControles(bool bAtivo)
        {
            txtParceiro_sCPF_CNPJ.ReadOnly = !bAtivo;
            txtParceiro_sRazaoSocial.ReadOnly = !bAtivo;
        }

        //thiago - 20/08/2024
        public void SetCampos(string cnpjCpf, string razao)
        {
            txtParceiro_sCPF_CNPJ.Text = cnpjCpf;
            txtParceiro_sRazaoSocial.Text = razao;
        }
        public string ScriptsPagina(string dgPagina, string tipoScript, List<string> elementos, Page pg)
        {
            string script = "";
            Dictionary<string, string> vParametrosAjax = new Dictionary<string, string>();
            vParametrosAjax.Add("sRazaoSocial", "JSON.stringify(request.term)");
            vParametrosAjax.Add("sidTipoParceiro", "$('#" + hddClientes_sTipoParceiro.ClientID + "').val()");
            //vParametrosAjax.Add("idCliente", "$('[id*=ddlTipoProduto]').val() || 'S'");

            if (tipoScript == "ScriptCompletar")
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("$v192(function() {");
                sb.Append("$v192(\"#" + dgPagina + "\").autocomplete({");
                sb.Append("source: function(request, response) {");
                sb.Append("$v192.ajax({");
                sb.Append("url:'/API/Pagina_Ajax.aspx/GetClientesParceiros',");
                sb.Append("data: JSON.stringify({");

                foreach (KeyValuePair<string, string> item in vParametrosAjax)
                {
                    sb.Append("'" + item.Key + "': " + item.Value + ", ");
                }

                if (vParametrosAjax.Count > 0)
                {
                    sb.Length -= 2; // Remove a vírgula extra
                }

                sb.Append("}),"); // Adicione uma vírgula após a chave 'data'

                sb.Append("dataType: \"json\",");
                sb.Append("type: \"POST\",");
                sb.Append("contentType: \"application/json; charset=utf-8\",");
                sb.Append("success: function(data) {");
                sb.Append("response($v192.map(data.d, function(item) {");
                sb.Append("return {");

                sb.Append("label: item.split('|')[0],");

                int index = 0;
                foreach (string elementoID in elementos)
                {
                    sb.Append(elementoID + ": item.split('|')[" + (index) + "],");
                    index++;
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append("};");
                sb.Append("}));"); // Adicione parênteses de fechamento para a função 'map'
                sb.Append("},");
                sb.Append("error: function(response) {");
                sb.Append("alert(response.responseText);");
                sb.Append("},");
                sb.Append("failure: function(response) {");
                sb.Append("alert(response.responseText);");
                sb.Append("}");
                sb.Append("});");
                sb.Append("},");
                sb.Append("select: function(e, i) {");

                foreach (string elementoID in elementos)
                {
                    sb.Append("$(\"#" + elementoID + "\").val(i.item." + elementoID + ");");
                }

                sb.Append("$('#" + txtParceiro_sRazaoSocial.ClientID + "').focus();");
                sb.Append("},");
                sb.Append("minLength: 3");
                sb.Append("});");
                sb.Append("});");

                ScriptManager.RegisterStartupScript(pg, pg.GetType(), "js_PesquisaParceiros" + Guid.NewGuid(), sb.ToString(), true);
            }
            else
            {
                //string codigoJavaScript = System.IO.Path.Combine("~/App/JS/TabelaConsulta.js");
                //string scriptPagina = File.ReadAllText(codigoJavaScript);
                //ScriptManager.RegisterStartupScript(pg, pg.GetType(), "js_ScriptPagina_" + tipoScript, scriptPagina, true);
            }

            return script;
        }
        protected virtual void OnControleEvento()
        {
            EventoPesquisaParceiro?.Invoke(this, EventArgs.Empty);
        }
    }
}