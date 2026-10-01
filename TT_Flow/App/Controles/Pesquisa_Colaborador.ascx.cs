using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;


namespace TT_Flow.App.Controles
{
    public partial class Pesquisa_Colaborador : System.Web.UI.UserControl
    {
        public bool Enabled
        {
            get { return txtColaborador_CPF.Enabled && txtColaborador_Nome.Enabled; }
            set
            {
                txtColaborador_CPF.Enabled = value;
                txtColaborador_Nome.Enabled = value;
            }
        }


        public int idColaborador { get => ObterIDColaborador(); set => DefinirColaborador(value); }
        public string SCPF { get => txtColaborador_CPF.Text; set => txtColaborador_CPF.Text = value; }
        public string SDscColaborador { get => txtColaborador_Nome.Text; set => txtColaborador_Nome.Text = value; }
       
        public string sErro { get; set; }
        public delegate void CPF_x_DscColaborador(object sender, EventArgs e);
        public event CPF_x_DscColaborador EventoPesquisaColaborador;



        protected void Page_Load(object sender, EventArgs e)
        {
            RegistrarScriptFormatarCpf();

            if (!IsPostBack)
            {
               
            }
            RegistrarScriptPesquisarItens();

        }

        public void LimparCamposColaborador()
        {
            txtColaborador_CPF.Text = string.Empty;
            txtColaborador_Nome.Text = string.Empty;
        }

        private void RegistrarScriptFormatarCpf()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("function formatarDocumento(valor) {");
            sb.AppendLine("    valor = valor.replace(/\\D/g, '');"); 
            sb.AppendLine("    if (valor.length > 11) {");
            sb.AppendLine("        valor = valor.substring(0, 11);"); 
            sb.AppendLine("    }");
            sb.AppendLine("    if (valor.length <= 11) {"); // CPF
            sb.AppendLine("        valor = valor.replace(/(\\d{3})(\\d)/, '$1.$2');");
            sb.AppendLine("        valor = valor.replace(/(\\d{3})(\\d)/, '$1.$2');");
            sb.AppendLine("        valor = valor.replace(/(\\d{3})(\\d{1,2})$/, '$1-$2');");
            sb.AppendLine("    }");
            sb.AppendLine("    return valor;");
            sb.AppendLine("}");

            sb.AppendLine("$(document).ready(function() {");
            sb.AppendLine("    $('#" + txtColaborador_CPF.ClientID + "').keyup(function() {");
            sb.AppendLine("        this.value = formatarDocumento(this.value);");
            sb.AppendLine("    });");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "scriptFormatarCpf", sb.ToString(), true);
        }

        protected int ObterIDColaborador()
        {
            if (hddidColaborador.Value != "")
            {
                return int.Parse(hddidColaborador.Value);
            }
            else
                return 0;
        }
        protected void DefinirColaborador(int idColaborador)
        {
            string sErro = "";
            SqlDataReader sdr = null;
            try
            {
                if (idColaborador == 0)
                {
                    return;
                }

                string sqlQuery = $"EXEC sp_Manipula_tbl_Flow_Colaboradores @sFuncao = 'Consulta_Controle', @idColaborador={idColaborador}";
                sdr = BD.ExecutarDataReader(sqlQuery);

                if (sdr.Read()) 
                {
                    txtColaborador_CPF.Text = sdr["sCPF"].ToString();
                    txtColaborador_Nome.Text = sdr["sDscColaborador"].ToString();
                    hddidColaborador.Value = sdr["idColaborador"].ToString();
                }
                else
                {
                    sErro = $"Colaborador (ID: {idColaborador}) Não localizado!";
                }
            }
            catch (Exception ex)
            {
                sErro = $"BD: Erro ao Consultar Colaborador: {ex.Message}";
            }
            finally
            {
                if (sdr != null)
                    sdr.Close();
            }

            if (!string.IsNullOrEmpty(sErro))
            {
                MensagemPagina.MostraMensagem_Erro(sErro);
            }
        }

        protected void txtColaborador_Nome_TextChanged(object sender, EventArgs e)
        {
            if(txtColaborador_Nome.Text.Length < 1)
            {
                txtColaborador_CPF.Text = "";   
            }

            if (string.IsNullOrEmpty(txtColaborador_CPF.Text))
            {
                SqlDataReader sdr = null;
                try
                {
                    string query = "EXEC sp_Manipula_tbl_Flow_Colaboradores @sFuncao = 'Consulta_Controle', @sDscColaborador='" + txtColaborador_Nome.Text.Replace("'", "''") + "'";
                    sdr = BD.ExecutarDataReader(query);

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            txtColaborador_CPF.Text = sdr["sCPF"].ToString();
                            hddidColaborador.Value = sdr["idColaborador"].ToString();
                            txtColaborador_Nome.Text = sdr["sDscColaborador"].ToString();
                        }
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro("Colaborador não localizado.");
                    }
                }
                catch (Exception ex)
                {
                    string sErro = "Erro ao consultar colaborador: " + ex.Message;
                    MensagemPagina.MostraMensagem_Erro(sErro);
                }
                finally
                {
                    if (sdr != null && !sdr.IsClosed)
                        sdr.Close();
                }
            }

            OnControleEvento();
        }




        protected void Colaborador_CPF_TextChanged(object sender, EventArgs e)
        {
            if (txtColaborador_CPF.Text.Length < 1)
            {
                txtColaborador_Nome.Text = "";
            }

            RegistrarScriptFormatarCpf();
            if (sender is TextBox txb)
            {
                string id = txb.ID;

                if (!string.IsNullOrEmpty(txtColaborador_Nome.Text) && id == txtColaborador_Nome.ID)
                    return;
            }

            string cpfComPontuacao = txtColaborador_CPF.Text; // CPF mantém sua pontuação

            if (string.IsNullOrEmpty(cpfComPontuacao))
                return;

            SqlDataReader sdr = null;
            try
            {
                string query = $"EXEC sp_Manipula_tbl_Flow_Colaboradores @sFuncao = 'Flow_Colaboradores_Avaliacao', @sCPF='{cpfComPontuacao}'";
                sdr = BD.ExecutarDataReader(query);

                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        txtColaborador_Nome.Text = sdr["sDscColaborador"].ToString();
                        hddidColaborador.Value = sdr["idColaborador"].ToString();
                    }
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Documento não localizado");
                }
            }
            catch (Exception ex)
            {
                string sErro = "Erro ao consultar colaborador: " + ex.Message;
                MensagemPagina.MostraMensagem_Erro(sErro);
            }
            finally
            {
                if (sdr != null)
                    sdr.Close();
            }

            OnControleEvento();
        }
        private string RemoverPontuacao(string texto)
        {
            return new string(texto.Where(char.IsDigit).ToArray());
        }


        public void RegistrarScriptPesquisarItens()
        {

            string _txtColaborador_Nome = txtColaborador_Nome.ClientID;
            string _txtColaborador_CPF = txtColaborador_CPF.ClientID;
            string _hddidColaborador = hddidColaborador.ClientID;

            List<string> list = new List<string>
            {
                _txtColaborador_Nome,
                _hddidColaborador,
                _txtColaborador_CPF
            };

            ScriptsPagina(_txtColaborador_Nome, "ScriptCompletar", list, this.Page);
        }
        public void LimparCampos()
        {
            txtColaborador_CPF.Text = "";
            txtColaborador_Nome.Text = "";
            hddidColaborador.Value = "";


        }

        public void ConfigurarControles(bool bAtivo)
        {
            txtColaborador_CPF.ReadOnly = !bAtivo;
            txtColaborador_Nome.ReadOnly = !bAtivo;
        }

        public string ScriptsPagina(string dgPagina, string tipoScript, List<string> elementos, Page pg)
        {
            string script = "";
            Dictionary<string, string> vParametrosAjax = new Dictionary<string, string>();
            vParametrosAjax.Add("sDscColaborador", "JSON.stringify(request.term)");

            if (tipoScript == "ScriptCompletar")
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("$v192(function() {");
                sb.Append("$v192(\"#" + dgPagina + "\").autocomplete({");
                sb.Append("source: function(request, response) {");
                sb.Append("$v192.ajax({");
                sb.Append("url:'/API/Pagina_Ajax.aspx/GetColaboradores',");
                sb.Append("data: JSON.stringify({");

                foreach (KeyValuePair<string, string> item in vParametrosAjax)
                {
                    sb.Append("'" + item.Key + "': " + item.Value + ", ");
                }

                if (vParametrosAjax.Count > 0)
                {
                    sb.Length -= 2;
                }

                sb.Append("}),");
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
                    sb.Append(elementoID + ": item.split('|')[" + index++ + "],");
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append("};");
                sb.Append("}));");
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

                sb.Append("$('#" + txtColaborador_Nome.ClientID + "').focus();");
                sb.Append("},");
                sb.Append("minLength: 3");
                sb.Append("});");
                sb.Append("});");

                ScriptManager.RegisterStartupScript(pg, pg.GetType(), "js_PesquisaColaborador" + Guid.NewGuid(), sb.ToString(), true);
            }

            return script;
        }

        protected virtual void OnControleEvento()
        {
            EventoPesquisaColaborador?.Invoke(this, EventArgs.Empty);
        }
    }
}