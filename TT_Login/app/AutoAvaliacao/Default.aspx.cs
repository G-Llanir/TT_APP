using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using System.Data.SqlClient;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using static TT.FrameWork.BD;
using System.Reflection.Emit;

namespace TT_Login.AutoAvaliacao
{
    public partial class Default : System.Web.UI.Page
    {
        int nPag_Identificacao = 0;
        int nPag_ConfirmacaoDados = 1;
        int nPag_Questao = 2;
        int nPag_Finalizacao = 3;
        int nPag_Erro = 5;

        public List<cls_Avaliacao_Perguntas_Ópcoes> lstPerguntas
        {
            get
            {
                if (ViewState["lstPerguntas"] == null)
                {
                    ViewState["lstPerguntas"] = new List<cls_Avaliacao_Perguntas_Ópcoes>();
                }
                return (List<cls_Avaliacao_Perguntas_Ópcoes>)ViewState["lstPerguntas"];
            }
            set
            {
                ViewState["lstPerguntas"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            lblTitulo.Text = Identity.Variaveis.sNomeSistema() + " - Autoavaliação (AAP)";
            if (!IsPostBack)
            {

                if (Request["SChave"] != null)
                {
                    hddsChave.Value = Request["SChave"].ToString();
                }

                if (Request["idr"] != null)
                {
                    hddidRegistro.Value = Request["idr"].ToString();
                }

                if (Request["sRP"] != null)
                {
                    hddsRP.Value = Request["sRP"].ToString();
                }

                if (Request["idU"] != null)
                {
                    hddidU.Value = Request["idU"].ToString();
                }

                if (hddidRegistro.Value != "")
                {
                    lblTitulo.Text = Identity.Variaveis.sNomeSistema() + " - " + hddsRP.Value;
                    DIV_Principal.Attributes["class"] = "col-lg-11";
                    cmdAvancar_2_Click(sender, e);
                }
                else
                {
                    mtv_Principal.ActiveViewIndex = nPag_Identificacao;
                    DIV_Principal.Attributes["class"] = "col-md-6 col-md-offset-3";
                    Funcoes.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Selecione o seu Departamento ", "0");
                    txtsCPF.Focus();
                }

            }
            var requestTarget = this.Request["__EVENTTARGET"];
            if (requestTarget == "funcao_LOCALIZACAO")
            {
                Session["Latitude"] = hddLatitude.Value;
                Session["Longitude"] = hddLongitude.Value;
                txtsCPF.Focus();
            }
        }

        protected void cmdAnonimo_Click(object sender, EventArgs e)
        {
            txtsNome.Text = "Não identificado";
            txtsNome.ReadOnly = true;
            mtv_Principal.ActiveViewIndex = nPag_ConfirmacaoDados;
        }

        protected void cmdIdentificar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtsCPF.Text))
            {
                SqlDataReader sdr = null;
                try
                {
                    string cpfComPontuacao = txtsCPF.Text;
                    string query = $"EXEC sp_Manipula_tbl_Flow_Colaboradores @sFuncao = 'Flow_Colaboradores_Avaliacao', @sCPF='{cpfComPontuacao}'";
                    sdr = BD.ExecutarDataReader(query);

                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            txtsNome.Text = sdr["sDscColaborador"].ToString();
                            ddlidDepartamento.SelectedValue = sdr["idDepartamento"].ToString();
                            hddidColaborador.Value = sdr["idColaborador"].ToString();
                            mtv_Principal.ActiveViewIndex = nPag_ConfirmacaoDados;

                            txtsNome.ReadOnly = true;
                            ddlidDepartamento.Attributes.Remove("disabled");
                            ddlidDepartamento.Attributes.Add("disabled", "disabled");
                        }
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro("Documento (CPF) não localizado");
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
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Informe um CPF válido!");
            }

        }

        private void RegistrarScriptFormatarCpf()
        {

            txtsCPF.Focus();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();



            sb.Append("function getGeolocation() {\r\n");
            sb.Append("     if (navigator.geolocation && $('hddLatitude').val().length <= 0 && $('hddLongitude').val().length <= 0) {\r\n");
            sb.Append("         navigator.geolocation.getCurrentPosition(sendPositionToServer, showError);\r\n");
            sb.Append("     }\r\n");
            sb.Append("}\r\n\r\n");

            sb.Append("function sendPositionToServer(position) {\r\n");
            sb.Append("     $('[id*=hddLatitude]').val(position.coords.latitude);\r\n");
            sb.Append("     $('[id*=hddLongitude]').val(position.coords.longitude);\r\n");
            sb.Append("     __doPostBack(\"funcao_LOCALIZACAO\", \"\");\r\n");
            sb.Append("}\r\n\r\n");

            sb.Append("function showError(error) {\r\n");
            sb.Append("     switch (error.code) {\r\n");
            sb.Append("         case error.PERMISSION_DENIED:\r\n");
            sb.Append("             console.log('Usuário negou a solicitação de geolocalização!');\r\n");
            sb.Append("             break;\r\n");
            sb.Append("         case error.POSITION_UNAVAILABLE:\r\n");
            sb.Append("             console.log('Informação da localização não está disponível!');\r\n");
            sb.Append("             break;\r\n");
            sb.Append("         case error.TIMEOUT:\r\n");
            sb.Append("             console.log('A solicitação para obter localização expirou!');\r\n");
            sb.Append("             break;\r\n");
            sb.Append("         case error.UNKNOWN_ERROR:\r\n");
            sb.Append("             console.log('Ocorreu um erro desconhecido!');\r\n");
            sb.Append("             break;\r\n");
            sb.Append("     }\r\n");
            sb.Append("}\r\n\r\n");

            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScript_Master" + Guid.NewGuid(), sb.ToString(), true);

        }

        protected void cmdAvancar_2_Click(object sender, EventArgs e)
        {

            string sErro = "";
            string sInformacoesCliente = "";
            string sIP = "";

            if (mtv_Principal.ActiveViewIndex == nPag_ConfirmacaoDados)
            {
                if (ddlidDepartamento.SelectedValue == "0")
                {
                    sErro += (sErro != "" ? "</br>" : "") + "Para continuar, é necessário informar seu departamento!";
                }
            }
            else if (mtv_Principal.ActiveViewIndex == nPag_Questao)
            {
                //Colocar as Validações

                if (DIV_sTipo_SIM_NAO.Visible && rb_Resposta.SelectedItem == null)
                {
                    sErro += (sErro != "" ? "</br>" : "") + "Por favor, selecione SIM ou NÃO!";
                }

                if (DIV_sTipo_Numeral.Visible && rb_Numeral.SelectedItem == null)
                {
                    sErro += (sErro != "" ? "</br>" : "") + "Por favor, selecione uma das opções";
                }

                if (DIV_sTipo_Unica.Visible && rb_OpcaoUnica.SelectedItem == null)
                {
                    sErro += (sErro != "" ? "</br>" : "") + "Por favor, selecione uma das opções";
                }

                if (DIV_sTipo_Multipla.Visible && chkMultipla.SelectedItem == null)
                {
                    sErro += (sErro != "" ? "</br>" : "") + "Por favor, selecione uma ou mais opções";
                }


                if (DIV_sTipo_TEXTO.Visible && txtsResposta.Text == "")
                {
                    sErro += (sErro != "" ? "</br>" : "") + "Por favor informe sua resposta!";
                }




                if (sErro == "")
                {
                    string sOpcoes = "";

                    if (hddsTipo.Value == "S")
                    {
                        sOpcoes = rb_Resposta.SelectedItem.Value.Replace("&nbsp;", "");
                    }

                    if (hddsTipo.Value == "N")
                    {
                        sOpcoes = rb_Numeral.SelectedItem.Text.Replace("&nbsp;", "");
                    }

                    if (hddsTipo.Value == "E")
                    {
                        sOpcoes = rb_OpcaoUnica.SelectedItem.Text.Replace("&nbsp;", "");
                    }

                    if (hddsTipo.Value == "M")
                    {
                        for (int i = 0; i < chkMultipla.Items.Count; i++)
                        {
                            if (chkMultipla.Items[i].Selected)
                            {
                                sOpcoes += chkMultipla.Items[i].Text.Replace("&nbsp;", "") + "|";
                            }
                        }
                    }
                    //Salvar Resposta


                    try
                    {
                        if (hddidRegistro.Value == "0")
                        {
                            System.Web.HttpBrowserCapabilities browser = System.Web.HttpContext.Current.Request.Browser;
                            sIP = Request.ServerVariables["REMOTE_ADDR"].ToString();
                            sInformacoesCliente = "Host:" + Request.ServerVariables["REMOTE_HOST"].ToString();
                            sInformacoesCliente += "|Navegador:" + browser.Browser;
                            sInformacoesCliente += "|Versão:" + browser.Version;
                            sInformacoesCliente += "|Rastreado:" + browser.Crawler;

                            if (browser.IsMobileDevice)
                            {
                                sInformacoesCliente += "|Mobile:Sim";
                                sInformacoesCliente += "|Fabricante:" + browser.MobileDeviceManufacturer.Replace("Unknown", "Não Reconhecido");
                                sInformacoesCliente += "|Modelo:" + browser.MobileDeviceModel.Replace("Unknown", "Não Reconhecido");
                            }
                            else
                            {
                                sInformacoesCliente += "|Mobile:Não";
                            }




                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }



                    DataSet dsAvaliacao;
                    try
                    {



                        Dictionary<String, String> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao",           "GRAVAR_RESPOSTA" },
                            { "@idColaborador",     hddidColaborador.Value },
                            { "@idDepartamento",    ddlidDepartamento.SelectedValue },
                            { "@idAvaliacao",       hddidAvaliacao.Value },
                            { "@idRegistro",        hddidRegistro.Value },
                            { "@sTipo",             hddsTipo.Value },
                            { "@idPergunta",        hddidPergunta.Value },
                            { "@sOpcoes",           sOpcoes },
                            { "@sResposta",         txtsResposta.Text },
                            { "@sLatitude",         hddLatitude.Value },
                            { "@sLongitude",        hddLongitude.Value },
                            { "@sIP",               sIP },
                            { "@sInformacoes_Cliente", sInformacoesCliente},
                            { "@sRespondidaPor",    hddsRP.Value },
                            { "@idUsuario",         hddidU.Value}

                        };
                        dsAvaliacao = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Avaliacao", vParametros);
                        if (BD.ValidarDataSet(dsAvaliacao, out sErro))
                        {
                            hddidRegistro.Value = BD.Retorno.DATASET(dsAvaliacao, 0, 0, "idRegistro");
                        }
                    }
                    catch (Exception ex)
                    {
                        sErro = "Erro ao gravar resposta: " + ex.Message;
                    }
                }
            }

            if (sErro == "")
            {
                DataSet dsAvaliacao;
                try
                {
                    Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao",           "APLICAR_AVALIACAO" },
                        { "@idColaborador",     hddidColaborador.Value },
                        { "@idDepartamento",    ddlidDepartamento.SelectedValue },
                        { "@idRegistro",        hddidRegistro.Value },
                        { "@sRespondidaPor",    hddsRP.Value },
                        { "@idUsuario",         hddidU.Value}

                    };
                    dsAvaliacao = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Avaliacao", vParametros);
                    if (BD.ValidarDataSet(dsAvaliacao, out sErro))
                    {
                        txtsResposta.Text = "";
                        if (dsAvaliacao.Tables[1].Rows.Count > 0)
                        {
                            string[] sOpcoes = BD.Retorno.DATASET(dsAvaliacao, 1, 0, "sOpcoes").Split('|');

                            lblTituloPergunta.Text = BD.Retorno.DATASET(dsAvaliacao, 1, 0, "sGrupo");
                            lblPergunta.Text = BD.Retorno.DATASET(dsAvaliacao, 1, 0, "sDscPergunta");
                            hddidPergunta.Value = BD.Retorno.DATASET(dsAvaliacao, 1, 0, "idPergunta");
                            hddidAvaliacao.Value = BD.Retorno.DATASET(dsAvaliacao, 0, 0, "idAvaliacao");
                            hddidRegistro.Value = BD.Retorno.DATASET(dsAvaliacao, 0, 0, "idRegistro");
                            hddidColaborador.Value = BD.Retorno.DATASET(dsAvaliacao, 0, 0, "idColaborador");
                            hddsTipo.Value = BD.Retorno.DATASET(dsAvaliacao, 1, 0, "sTipo");
                            hddsCaixadeObservacao.Value = BD.Retorno.DATASET(dsAvaliacao, 1, 0, "sCaixadeObservacao");


                            if (hddsTipo.Value == "N")
                            {
                                rb_Numeral.Items.Clear();
                                for (var i = Convert.ToInt32(sOpcoes[0]); i <= Convert.ToInt32(sOpcoes[1]); i++)
                                {
                                    rb_Numeral.Items.Add("&nbsp;" + (i).ToString() + "&nbsp;&nbsp;&nbsp;");
                                }

                            }

                            if (hddsTipo.Value == "E")
                            {
                                lstPerguntas.Clear();

                                rb_OpcaoUnica.Items.Clear();
                                hddsCaixadeObservacao.Value = "|";
                                for (var i = 0; i < sOpcoes.Count(); i++)
                                {
 
                                    if (sOpcoes[i].ToString() != "")
                                    {
                                        cls_Avaliacao_Perguntas_Ópcoes perguntas = new cls_Avaliacao_Perguntas_Ópcoes();
                                        string sValor = sOpcoes[i].Split(';')[0];
                                        perguntas.sValor = sOpcoes[i].Split(';')[0];
                                        perguntas.sCaixadeObservacao = sOpcoes[i].Split(';')[1];
                                        if (sOpcoes[i].Split(';').Length > 2)
                                            perguntas.sMensagemObservacao = sOpcoes[i].Split(';')[2];
                                        lstPerguntas.Add(perguntas);
                                        
                                        if (sOpcoes[i].Split(';')[1] == "S")
                                        {
                                            hddsCaixadeObservacao.Value += sValor + "|";
                                        }
                                        rb_OpcaoUnica.Items.Add("&nbsp;" + sValor);
                                    }
                                }

                            }

                            if (hddsTipo.Value == "M")
                            {
                                chkMultipla.Items.Clear();
                                hddsCaixadeObservacao.Value = "|";
                                for (var i = 0; i < sOpcoes.Count(); i++)
                                {
                                    if (sOpcoes[i].ToString() != "")
                                    {
                                        string sValor = sOpcoes[i].Split(';')[0];

                                        if (sOpcoes[i].Split(';')[1] == "S")
                                        {
                                            hddsCaixadeObservacao.Value += sValor + "|";
                                        }
                                        chkMultipla.Items.Add("&nbsp;" + sValor);
                                    }
                                }
                            }

                            //Ajustar Exibição dos Campos
                            AjustaCampos_Respostas(hddsTipo.Value, true);
                            mtv_Principal.ActiveViewIndex = nPag_Questao;
                        }
                        else
                        {
                            //Finaliza a Avaliação
                            Dictionary<String, String> vParametros_Finaliza = new Dictionary<string, string>
                            {
                                { "@sFuncao",           "FINALIZAR_AVALIACAO" },
                                { "@idRegistro",        hddidRegistro.Value },
                                { "@sRespondidaPor",    hddsRP.Value },
                                { "@idUsuario",         hddidU.Value}
                            };
                            dsAvaliacao = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Avaliacao", vParametros_Finaliza);
                            mtv_Principal.ActiveViewIndex = nPag_Finalizacao;
                            if (hddsRP.Value != "")
                            {
                                cmdFinalizar.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro(sErro);
                    }

                }
                catch (Exception ex)
                {
                    sErro = "Erro ao consultar colaborador: " + ex.Message;
                    MensagemPagina.MostraMensagem_Erro(sErro);
                }

            }
            else
            {
                MensagemPagina.MostraMensagem_Erro(sErro);
                AjustaCampos_Respostas(hddsTipo.Value, false);
            }
        }

        void AjustaCampos_CaixaJustificativa(bool sLimparCampos, bool bFocusCaixaTexto)
        {
            bool bExibirCaixaJustificativa = false;
            lblJustifique_Texto.Text = "Justifique";
            switch (hddsTipo.Value)
            {
                case "E":


                    for (var i = 0; i < lstPerguntas.Count(); i++)
                    {
                        if (lstPerguntas[i].sValor == rb_OpcaoUnica.SelectedValue.Replace("&nbsp;", ""))
                        {
                            if (lstPerguntas[i].sCaixadeObservacao == "S")
                            {
                                if (lstPerguntas[i].sMensagemObservacao != "")
                                    lblJustifique_Texto.Text = lstPerguntas[i].sMensagemObservacao;

                                bExibirCaixaJustificativa = true;
                                txtsResposta.Focus();
                            }

                        }
                    }
                    //if (hddsCaixadeObservacao.Value.Contains("|" + rb_OpcaoUnica.SelectedValue.Replace("&nbsp;", "") + "|"))
                    //{
                    //    bExibirCaixaJustificativa = true;
                    //    txtsResposta.Focus();
                    //}
                    break;

                case "M":
                    if (hddsCaixadeObservacao.Value.Contains(chkMultipla.SelectedValue.Replace("&nbsp;", "") + "|"))
                    {
                        bExibirCaixaJustificativa = true;
                        txtsResposta.Focus();
                    }
                    break;



                default:
                    if (hddsCaixadeObservacao.Value == "S")
                        bExibirCaixaJustificativa = true;
                    break;

            }



            lblJustifique.Visible = false;
            if (bExibirCaixaJustificativa)
            {
                if (hddsTipo.Value != "T")
                    lblJustifique.Visible = true;
                DIV_sTipo_TEXTO.Visible = true;
            }
            else
            {

                DIV_sTipo_TEXTO.Visible = false;
                txtsResposta.Text = "";
            }


            if (sLimparCampos)
                txtsResposta.Text = "";

            if (bFocusCaixaTexto)
                txtsResposta.Focus();
        }

        void AjustaCampos_Respostas(string sTipo, bool sLimparCampos)
        {
            DIV_sTipo_TEXTO.Visible = false;
            DIV_sTipo_SIM_NAO.Visible = false;
            DIV_sTipo_Numeral.Visible = false;
            DIV_sTipo_Unica.Visible = false;
            DIV_sTipo_Multipla.Visible = false;

            lblJustifique.Visible = false;
            switch (hddsTipo.Value)
            {
                case "T": //Caixa de TExto
                    DIV_sTipo_TEXTO.Visible = true;
                    txtsResposta.Text = "";
                    txtsResposta.Focus();
                    break;

                case "S": //Caixa de SIM ou Não
                    DIV_sTipo_SIM_NAO.Visible = true;
                    if (rb_Resposta.SelectedItem != null)
                        if (sLimparCampos)
                            rb_Resposta.ClearSelection();
                    break;

                case "N": //Numeral
                    DIV_sTipo_Numeral.Visible = true;
                    break;

                case "E": //Opção única
                    DIV_sTipo_Unica.Visible = true;
                    break;

                case "M":
                    DIV_sTipo_Multipla.Visible = true;
                    break;


            }
            AjustaCampos_CaixaJustificativa(sLimparCampos, false);

        }
        protected void cmdFinalizar_Click(object sender, EventArgs e)
        {
            Response.Redirect("https://www.tecandtec.com.br");
        }

        protected void rb_OpcaoUnica_SelectedIndexChanged(object sender, EventArgs e)
        {
            AjustaCampos_CaixaJustificativa(true, true);
        }

        protected void chkMultipla_SelectedIndexChanged(object sender, EventArgs e)
        {
            AjustaCampos_CaixaJustificativa(true, true);
        }

        protected void rb_Numeral_SelectedIndexChanged(object sender, EventArgs e)
        {
            AjustaCampos_CaixaJustificativa(true, true);
        }

        protected void rb_Resposta_SelectedIndexChanged(object sender, EventArgs e)
        {
            AjustaCampos_CaixaJustificativa(true, true);
        }




    }

    [Serializable]

    public class cls_Avaliacao_Perguntas_Ópcoes
    {

        #region | Construtor 
        public cls_Avaliacao_Perguntas_Ópcoes()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public cls_Avaliacao_Perguntas_Ópcoes
        (
             string sValor
            , string sCaixadeObservacao
            , string sMensagemObservacao
        )
        {
            _sValor = sValor;
            _sCaixadeObservacao = sCaixadeObservacao;
            _sMensagemObservacao = sMensagemObservacao;
        }

        #endregion

        #region | Membros Privados 


        private string _sValor;
        private string _sMensagemObservacao;
        private string _sCaixadeObservacao;

        #endregion

        public string sValor
        {
            get { return _sValor; }
            set { _sValor = value; }
        }
        public string sMensagemObservacao
        {
            get { return _sMensagemObservacao; }
            set { _sMensagemObservacao = value; }
        }
        public string sCaixadeObservacao
        {
            get { return _sCaixadeObservacao; }
            set { _sCaixadeObservacao = value; }
        }
    }
}
    