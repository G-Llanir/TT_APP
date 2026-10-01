using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using GRID = TT.FrameWork.Grid;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class Colaboradores_Detalhe : Page
    {
        #region | Construtores

        #region | Propriedades

        string sTituloPagina = "Colaborador";
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores";
        string sProcedureColaboradoresArea = "sp_Manipula_tbl_Flow_Colaboradores_Area";
        bool isSalvarStatus = false;
        int idExclusao;

        int nTabela_Principal = 0;
        int nTabela_NR = 1;
        int nTabela_EPI = 2;
        int nTabela_Ocorrencias = 3;
        int nTabela_Dependentes = 4;
        int nTabela_Equipamentos = 7;
        int nTabela_VT = 8;
        int nTabela_Roupas = 9;
        int nTabela_Avaliacao = 10;
        int nTabela_ArquivoMorto = 11;
        int nTabela_EntregaEPI = 12;
        int nTabela_PlanoSaude = 13;
        int nTabela_Ferias = 14;
        static int nTempoEntradaPlanoSaude = 0;


        #endregion

        #region | Classes

        public List<cls_NR_Itens> bs_NR_Itens
        {

            get
            {
                if (ViewState["bs_NR_Itens"] == null)
                {
                    ViewState["bs_NR_Itens"] = new List<cls_NR_Itens>();
                }
                return (List<cls_NR_Itens>)ViewState["bs_NR_Itens"];
            }

            set
            {
                ViewState["bs_NR_Itens"] = value;
            }

        }

        public List<cls_EPI_Itens> bs_EPI_Itens
        {
            get
            {
                if (ViewState["bs_EPI_Itens"] == null)
                {
                    ViewState["bs_EPI_Itens"] = new List<cls_EPI_Itens>();
                }
                return (List<cls_EPI_Itens>)ViewState["bs_EPI_Itens"];
            }

            set
            {
                ViewState["bs_EPI_Itens"] = value;
            }

        }

        public List<cls_Ausencia> bs_Ausencia
        {
            get
            {
                if (ViewState["bs_Ausencia"] == null)
                {
                    ViewState["bs_Ausencia"] = new List<cls_Ausencia>();
                }
                return (List<cls_Ausencia>)ViewState["bs_Ausencia"];
            }

            set
            {
                ViewState["bs_Ausencia"] = value;
            }

        }

        public List<cls_Credito> bs_Credito
        {
            get
            {
                if (ViewState["bs_Credito"] == null)
                {
                    ViewState["bs_Credito"] = new List<cls_Credito>();
                }
                return (List<cls_Credito>)ViewState["bs_Credito"];
            }

            set
            {
                ViewState["bs_Credito"] = value;
            }

        }

        public List<cls_Evento> bs_Evento
        {
            get
            {
                if (ViewState["bs_Evento"] == null)
                {
                    ViewState["bs_Evento"] = new List<cls_Evento>();
                }
                return (List<cls_Evento>)ViewState["bs_Evento"];
            }

            set
            {
                ViewState["bs_Evento"] = value;
            }

        }

        public List<cls_Ocorrencias> bs_Ocorrencias
        {
            get
            {
                if (ViewState["bs_Ocorrencias"] == null)
                {
                    ViewState["bs_Ocorrencias"] = new List<cls_Ocorrencias>();
                }
                return (List<cls_Ocorrencias>)ViewState["bs_Ocorrencias"];
            }

            set
            {
                ViewState["bs_Ocorrencias"] = value;
            }

        }

        public List<cls_Equipamentos> bs_Equipamentos
        {
            get
            {
                if (ViewState["bs_Equipamentos"] == null)
                {
                    ViewState["bs_Equipamentos"] = new List<cls_Equipamentos>();
                }
                return (List<cls_Equipamentos>)ViewState["bs_Equipamentos"];
            }

            set
            {
                ViewState["bs_Equipamentos"] = value;
            }

        }

        public List<cls_Dependentes> bs_Dependentes
        {
            get
            {
                if (ViewState["bs_Dependentes"] == null)
                {
                    ViewState["bs_Dependentes"] = new List<cls_Dependentes>();
                }
                return (List<cls_Dependentes>)ViewState["bs_Dependentes"];
            }

            set
            {
                ViewState["bs_Dependentes"] = value;
            }

        }

        public List<cls_Elogios> bs_Elogios
        {
            get
            {
                if (ViewState["bs_Elogios"] == null)
                {
                    ViewState["bs_Elogios"] = new List<cls_Elogios>();
                }
                return (List<cls_Elogios>)ViewState["bs_Elogios"];
            }

            set
            {
                ViewState["bs_Elogios"] = value;
            }

        }

        public List<cls_Conversas> bs_Conversas
        {
            get
            {
                if (ViewState["bs_Conversas"] == null)
                {
                    ViewState["bs_Conversas"] = new List<cls_Conversas>();
                }
                return (List<cls_Conversas>)ViewState["bs_Conversas"];
            }

            set
            {
                ViewState["bs_Conversas"] = value;
            }

        }

        public List<cls_VT> bs_VT
        {
            get
            {
                if (ViewState["bs_VT"] == null)
                {
                    ViewState["bs_VT"] = new List<cls_VT>();
                }
                return (List<cls_VT>)ViewState["bs_VT"];
            }

            set
            {
                ViewState["bs_VT"] = value;
            }

        }

        public List<cls_Roupas> bs_Roupas
        {
            get
            {
                if (ViewState["bs_Roupas"] == null)
                {
                    ViewState["bs_Roupas"] = new List<cls_Roupas>();
                }
                return (List<cls_Roupas>)ViewState["bs_Roupas"];
            }

            set
            {
                ViewState["bs_Roupas"] = value;
            }

        }

        public List<cls_Avaliacao> bs_Avaliacao
        {
            get
            {
                if (ViewState["bs_Avaliacao"] == null)
                {
                    ViewState["bs_Avaliacao"] = new List<cls_Avaliacao>();
                }
                return (List<cls_Avaliacao>)ViewState["bs_Avaliacao"];
            }

            set
            {
                ViewState["bs_Avaliacao"] = value;
            }

        }

        public List<cls_ArquivoMorto> bs_ArquivoMorto
        {
            get
            {
                if (ViewState["bs_ArquivoMorto"] == null)
                {
                    ViewState["bs_ArquivoMorto"] = new List<cls_ArquivoMorto>();
                }
                return (List<cls_ArquivoMorto>)ViewState["bs_ArquivoMorto"];
            }

            set
            {
                ViewState["bs_ArquivoMorto"] = value;
            }

        }

        public List<cls_EntregaEPI> bs_EntregasEPI
        {
            get
            {
                if (ViewState["bs_EntregasEPI"] == null)
                {
                    ViewState["bs_EntregasEPI"] = new List<cls_EntregaEPI>();
                }
                return (List<cls_EntregaEPI>)ViewState["bs_EntregasEPI"];
            }
            set
            {
                ViewState["bs_EntregasEPI"] = value;
            }

        }

        public List<cls_PlanoSaude_Itens> bs_PlanoSaude
        {
            get
            {
                if (ViewState["bs_PlanoSaude"] == null)
                {
                    ViewState["bs_PlanoSaude"] = new List<cls_PlanoSaude_Itens>();
                }
                return (List<cls_PlanoSaude_Itens>)ViewState["bs_PlanoSaude"];
            }
            set
            {
                ViewState["bs_PlanoSaude"] = value;
            }

        }

        private enum eBloco
        {
            NR = 1,
            VT = 2,
            Conversas = 3,
            Roupas = 4,
            Atestado = 5,
            EPI = 6,
            Evento = 7,
            Equipamento = 8,
            Ocorrencias = 9,
            Dependentes = 10,
            Avaliacao = 11,
            Credito = 12,
            ArquivoMorto = 13,
            PlanoSaude = 14
        }

        #endregion

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {


            var tipo = ddlConversas_idTipoEvento.SelectedValue;
            var meio = ddlConversas_idMeio.SelectedValue;
            var acao = ddlConversas_idAcao.SelectedValue;

            if (!IsPostBack)
            {
                PopularCombos();

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.Consultar, true);

                    Pesquisar(Request["id"].ToString(), false);

                    string sTipo = Request.QueryString["sTipo"];
                    if (sTipo != null)
                    {
                        StringBuilder script = new StringBuilder();
                        script.AppendLine("<script>");
                        script.AppendLine("$(window).on('load', function() {");

                        if (sTipo == "NR")
                        {
                            Div_nr.Visible = true;
                            script.AppendLine("$('#SSTT-tab').tab('show');");
                            script.AppendLine($"$('#{Div_nr.ClientID}').get(0).scrollIntoView({{ behavior: 'smooth' }});");

                        }
                        else if (sTipo == "EPI")
                        {
                            Div_EPI.Visible = true;
                            script.AppendLine("$('#SSTT-tab').tab('show');");
                            script.AppendLine("$('.composicaoLinha').each(function() {");
                            script.AppendLine("    var btn = $(this);");
                            script.AppendLine("    var divId = btn.data('div-id');");
                            script.AppendLine("    var targetDiv = $('#' + divId);");
                            script.AppendLine("    if (targetDiv.length > 0 && targetDiv.css('display') === 'none') {");
                            script.AppendLine("        btn.click();");
                            script.AppendLine("        targetDiv.show();");
                            script.AppendLine("        btn.removeClass('fa-plus').addClass('fa-minus');");
                            script.AppendLine("    }");
                            script.AppendLine("});");
                            script.AppendLine($"$('#{Div_EPI.ClientID}').get(0).scrollIntoView({{ behavior: 'smooth' }});");

                        }
                        else if (sTipo == "EPI_Antigo")
                        {
                            div_EPIs_Antigos.Visible = true;
                            script.AppendLine("$('#SSTT-tab').tab('show');");
                            script.AppendLine($"document.getElementById('{cmdConsulta_EPIsAntigos.ClientID}').click();");
                            script.AppendLine($"$('#{div_EPIs_Antigos.ClientID}').get(0).scrollIntoView({{ behavior: 'smooth' }});");
                        }
                        else if (sTipo == "DOC")
                        {
                            script.AppendLine("$('#documentos-tab').tab('show');");
                        }
                        else if (sTipo == "PS")
                        {
                            Div_PlanoSaude.Visible = true;
                            script.AppendLine("$('#beneficios-tab').tab('show');");
                            script.AppendLine($"$('#{Div_PlanoSaude.ClientID}').get(0).scrollIntoView({{ behavior: 'smooth' }});");
                        }

                        script.AppendLine("});");
                        script.AppendLine("</script>");

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ativaAbaEExpande", script.ToString(), false);
                    }
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.Incluir, true);
                    FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesConversas.Incluir, true);
                    Pesquisar("0", true);
                }

                ddlidTipoContrato_SelectedIndexChanged(null, null);
                ddlsLimiteContrato_SelectedIndexChanged(null, null);
                ddlsSituacao_SelectedIndexChanged(null, null);
                ddlsDependentesConvenio_SelectedIndexChanged(null, null);
                ddlsPensaoDependente_SelectedIndexChanged(null, null);
                ddlVT_sOpcaoVT_SelectedIndexChanged(null, null);
                ddlidFuncao_SelectedIndexChanged(null, null);

                if (ViewState["Funcao"] != null)
                {
                    int funcao = Convert.ToInt32(ViewState["Funcao"]);
                    ddlidFuncao.SelectedValue = funcao.ToString();
                    PopularDadosFuncao(funcao);
                }
            }
            else
            {
                var requestTarget = Page.Request["__EVENTTARGET"];
                var requestArgs = Page.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                else if (requestTarget == "funcao_SALVAR")
                {
                    Salvar_Colaborador();
                    //Response.Redirect(Request.RawUrl); //08/09/2026 - Retirei pois não exibia mensagem de Erro 
                }

                else if (requestTarget == "funcao_Editar")
                    Pesquisar(hddidColaborador.Value, true);

                //------------------Higor Maestrello 18-06-2024------------------------------
                if (requestTarget == "dialog_CheckArquivo")
                {
                    if (hddChecked.Value != "")
                        DownloadArquivoCheck(hddChecked.Value);
                    else
                    {
                        MensagemPagina1.MostraMensagem_Erro("Selecione um Arquivo !");
                        MensagemPagina1.Focus();
                    }
                }
                //--------------------------------------------------------------------------
            }

            Popular_TipoBeneficio();
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page, "tooltip");
            RegistraScript("");
        }

        protected void Pesquisar(string idColaborador, bool bEdicao)
        {
            PopularCombos();
            DIV_Salvar.Visible = true;
            Principal_cmdEditar.Visible = false;

            Div_atestado.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesAbaOcorrencias.Bloco_Ausencia);
            Div_Evento.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesAbaOcorrencias.Bloco_Eventos);
            Div_Veiculos.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesAbaOcorrencias.Bloco_Veiculos);

            DateTime dtAtual = DateTime.Now;

            try
            {
                LimpaClasse();

                if (idColaborador != "0")
                {
                    btnCredito_Salvar.Visible = false;                    
                    Div_Beneficios.Visible = true;
                    //aba_Avaliacao.Visible = true;
                    //aba_Beneficios.Visible = true;
                    //aba_ArquivoMorto.Visible = true;

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idColaborador", idColaborador }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidColaborador.Value = RETORNO.DATASET(dsPesquisa, 0, "idColaborador");

                        // Bloco Beneficios

                        //TipoBeneficio_Popular(RETORNO.DATASET(dsPesquisa, 0, "sTipoBeneficio"));

                        // Bloco Dados do Colaborador
                        txtsDscColaborador.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscColaborador");
                        txtsNomeSocial.Text = RETORNO.DATASET(dsPesquisa, 0, "sNomeSocial");
                        ddlidEmpresa.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEmpresa");
                        ddlidTipoContrato.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipoContrato");
                        ViewState["idTipoContrato"] = Convert.ToInt32(RETORNO.DATASET(dsPesquisa, 0, "idTipoContrato"));

                        ddlidDepartamento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idDepartamento");
                        ddlidCargo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCargo");
                        ddlidSupervisorDireto.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idSupervisorDireto");

                        PopularCombo_Funcao();
                        ddlidFuncao.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idFuncao");
                        PopularCombo_Funcao();
                        ViewState["Funcao"] = ddlidFuncao.SelectedValue;
                        txtsEmail.Text = RETORNO.DATASET(dsPesquisa, 0, "sEmail");

                        if (RETORNO.DATASET(dsPesquisa, 0, "dtInicioContrato").ToString() != "")
                        {
                            var dtValidade = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtInicioContrato").ToString());
                            txtdtInicioContrato.Text = dtValidade.ToString(@"yyyy/MM/dd").Replace('/', '-');
                        }

                        ddlsLimiteContrato.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sLimiteContrato");
                        txtsTerminoContrato.Text = RETORNO.DATASET(dsPesquisa, 0, "sTerminoContrato");
                        txtsTelCelular.Text = RETORNO.DATASET(dsPesquisa, 0, "sTelCelular");
                        txtsRamal.Text = RETORNO.DATASET(dsPesquisa, 0, "sRamal");
                        ddlsDependentesConvenio.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sDependentesConvenio");
                        ddlsSituacao.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sSituacao");
                        txtdtDesligamento.Text = RETORNO.DATASET(dsPesquisa, 0, "dtDesligamento");
                        ddlsTipoCTPS.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sTipoCTPS");
                        sCBO.Text = RETORNO.DATASET(dsPesquisa, 0, "sCBO");
                        txtdtPrevPlanoSaude.Text = RETORNO.DATASET(dsPesquisa, 0, "dtPrevPlanoSaude");
                        txtRegistro.Text = RETORNO.DATASET(dsPesquisa, 0, "sRegistro");

                        // Bloco Dados Pessoais
                        txtEndereco_sCEP.Text = RETORNO.DATASET(dsPesquisa, 0, "sCEP");
                        txtEndereco_sLogradouro.Text = RETORNO.DATASET(dsPesquisa, 0, "sLogradouro");
                        txtEndereco_sNumero.Text = RETORNO.DATASET(dsPesquisa, 0, "sNumero");
                        txtEndereco_sComplemento.Text = RETORNO.DATASET(dsPesquisa, 0, "sComplemento");
                        txtEndereco_sBairro.Text = RETORNO.DATASET(dsPesquisa, 0, "sBairro");
                        txtEndereco_sCidade.Text = RETORNO.DATASET(dsPesquisa, 0, "sCidade");
                        ddlEndereco_sUF.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sUF");
                        txtdtNascimento.Text = RETORNO.DATASET(dsPesquisa, 0, "dtNascimento");
                        txtsTelResidencial.Text = RETORNO.DATASET(dsPesquisa, 0, "sTelResidencial");
                        txtsCelularPessoal.Text = RETORNO.DATASET(dsPesquisa, 0, "sCelularPessoal");
                        txtsEmailPessoal.Text = RETORNO.DATASET(dsPesquisa, 0, "sEmailPessoal");
                        txtsNomeMae.Text = RETORNO.DATASET(dsPesquisa, 0, "sNomeMae");
                        ddlsEstadoCivil.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEstadoCivil");
                        ddlsSexo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sSexo");
                        ddlsPolitico.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sPolitico");

                        txtsCPF.Text = RETORNO.DATASET(dsPesquisa, 0, "sCPF").ToString();
                        ViewState["sCPF"] = txtsCPF.Text;

                        txtsRG.Text = RETORNO.DATASET(dsPesquisa, 0, "sRG").ToString();
                        ViewState["sRG"] = txtsRG.Text;

                        txtsPIS.Text = RETORNO.DATASET(dsPesquisa, 0, "sPIS").ToString();
                        ViewState["sPIS"] = txtsPIS.Text;

                        txtsIE.Text = RETORNO.DATASET(dsPesquisa, 0, "sIE").ToString();
                        ViewState["sIE"] = txtsIE.Text;

                        txtsIM.Text = RETORNO.DATASET(dsPesquisa, 0, "sIM").ToString();
                        ViewState["sIM"] = txtsIM.Text;

                        txtsNIRE.Text = RETORNO.DATASET(dsPesquisa, 0, "sNIRE").ToString();
                        ViewState["sNIRE"] = txtsNIRE.Text;

                        txtdtRegistro.Text = RETORNO.DATASET(dsPesquisa, 0, "dtRegistro").ToString();
                        ViewState["dtRegistro"] = txtdtRegistro.Text;

                        ddlsEscolaridade.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEscolaridade");
                        txtsCNH.Text = RETORNO.DATASET(dsPesquisa, 0, "sCNH");
                        txtdtVencCNH.Text = RETORNO.DATASET(dsPesquisa, 0, "dtVencCNH");
                        TipoCNH_Popular(RETORNO.DATASET(dsPesquisa, 0, "sTipoCNH"));
                        ddlsTipoConta.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sTipoConta");
                        txtsBanco.Text = RETORNO.DATASET(dsPesquisa, 0, "sBanco");
                        txtsAgenciaBancaria.Text = RETORNO.DATASET(dsPesquisa, 0, "sAgenciaBancaria");
                        txtsContaBancaria.Text = RETORNO.DATASET(dsPesquisa, 0, "sContaBancaria");
                        ddlsRelatorioGastos.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sRelatorioGastos");

                        txtsCNPJ.Text = RETORNO.DATASET(dsPesquisa, 0, "sCNPJ");
                        ViewState["sCNPJ"] = txtsCNPJ.Text;

                        txtsObservacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacao");

                        // Saúde
                        ddlidTipoSanguineo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipoSanguineo");
                        txtsContatoEmergencia.Text = RETORNO.DATASET(dsPesquisa, 0, "sContatoEmergencia");
                        txtsDscContatoEmergencia.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscContatoEmergencia");

                        // Bloco VR
                        txtsVR.Text = RETORNO.DATASET(dsPesquisa, 0, "sVR");
                        txtnValorDiaVR.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorDiaVR");
                        ddlsVRDefinitivo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sVRDefinitivo");

                        // Bloco Plano Saúde
                        ddlTemPlanoSaude.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sTemPlanoSaude");
                        txtdtCarenciaConvenio.Text = RETORNO.DATASET(dsPesquisa, 0, "dtCarenciaConvenio");
                        nTempoEntradaPlanoSaude = Convert.ToInt32(RETORNO.DATASET(dsPesquisa, 0, "nTempoEntradaPlanoSaude"));

                        if (ddlTemPlanoSaude.SelectedValue == "S")
                        {
                            div_btnIncluirPlanoSaude.Visible = true;
                        }
                        else if (ddlTemPlanoSaude.SelectedValue == "N")
                        {
                            div_arquivoPlanoSaude.Visible = true;
                            div_btnIncluirPlanoSaude.Visible = false;
                        }

                        hddidArquivoPlanoSaude.Value = RETORNO.DATASET(dsPesquisa, 0, "idArquivoPlanoSaude");

                        // Bloco Outros
                        txtsObsBeneficios.Text = RETORNO.DATASET(dsPesquisa, 0, "sObsBeneficios");

                        // Bloco Seguro de Vida
                        ddlSeguroVida.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sSeguroVida");

                        if (ddlSeguroVida.SelectedValue == "S")
                        {
                            div_tipoSeguro.Visible = true;
                            ddlsTipoSeguro.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idPlanoSeguro");
                            txtdtInicioSeguro.Text = RETORNO.DATASET(dsPesquisa, 0, "dtInicioSeguro");
                        }

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, nTabela_Principal, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, nTabela_Principal, "sDscUsuarioAtualizacao"));
                        lblTituloPagina.Text = string.Format("Colaborador {0}", RETORNO.DATASET(dsPesquisa, nTabela_Principal, "sDscColaborador"));
                        BreadCrumb.TitulodaPagina = string.Format("Colaborador {0}", RETORNO.DATASET(dsPesquisa, nTabela_Principal, "sDscColaborador"));

                        lblTituloSalvar.Text = "Confirma a Alteração da " + lblTituloPagina.Text + "?";
                        lblTituloEdiar.Text = "Deseja editar o Colaborador " + lblTituloPagina.Text + "?";

                        carregaimgColaborador(hddidColaborador.Value, false);
                        carregaimgColaborador(hddidColaborador.Value, true);

                        //txtdtInicioAtestado_Pesquisa.Text = new DateTime(dtAtual.Year, dtAtual.Month, 1).ToString();
                        //txtdtFinalAtestado_Pesquisa.Text = new DateTime(dtAtual.Year, dtAtual.Month, DateTime.DaysInMonth(dtAtual.Year, dtAtual.Month)).ToString();
                        txtdtInicioAtestado_Pesquisa.Text = dtAtual.AddYears(-1).ToString("dd/MM/yyyy");
                        txtdtFinalAtestado_Pesquisa.Text = dtAtual.ToString("dd/MM/yyyy");

                        txtdtdtInicioAfastamento_Pesquisa.Text = new DateTime(dtAtual.Year, dtAtual.Month, 1).ToString();
                        txtdtdtFinalAfastamento_Pesquisa.Text = new DateTime(dtAtual.Year, dtAtual.Month, DateTime.DaysInMonth(dtAtual.Year, dtAtual.Month)).ToString();

                        Popular_dtgEvento(idColaborador);
                        dtg_Ausencia_Popular(idColaborador);
                        Popular_dtgOcorrencias(dsPesquisa);
                        Popular_dtgDependentes(dsPesquisa);
                        Popular_dtgEquipamentos(dsPesquisa);
                        Popular_dtgVT(dsPesquisa);
                        Roupas_Popular(dsPesquisa);
                        Popular_dtgAvaliacao(dsPesquisa);
                        Popular_dtgEPI(dsPesquisa);
                        Popular_dtgNR(dsPesquisa);
                        Popular_gvArquivoMorto(dsPesquisa);
                        Popula_EntregasEPI(dsPesquisa.Tables[nTabela_EntregaEPI]);
                        Popular_gv_PlanoSaude(dsPesquisa);
                        PopularControleFerias(dsPesquisa.Tables[nTabela_Ferias]);

                        Div_Beneficios.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.ExibirAbaBeneficios);
                        aba_Beneficios.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.ExibirAbaBeneficios);
                        aba_Avaliacao.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.ExibirAbaAvaliacao);
    

                        if (!bEdicao)
                        {
                            Popular_Aba_Documentos(idColaborador);
                            Popular_Aba_DocumentosSSTT(idColaborador);
                            Popular_Aba_Ocorrencias(idColaborador);
                            Popular_aba_Beneficios(idColaborador);
                            pesquisarConversas(idColaborador, false);

                            Principal_cmdEditar.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Alterar);
                            Principal_cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Alterar);
                            aba_Documentos.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.ExibirDocumentos_RH);
                            aba_SSTT.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.SSTT);
                            aba_Evento.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Ocorrencias);
                            DIV_Documentos_Evento.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.ExibirDocumentos_Ocorrencias);
                            DIV_DadosPessoais.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.ExibirDadosPessoais);
                            Div_Documentos.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.ExibirDadosPessoais);
                            aba_ArquivoMorto.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.AbaArquivoMorto);
                        }
                    }
                    else
                        throw new Exception(sErro);
                }
                else
                {
                    BreadCrumb.TitulodaPagina = string.Format("Novo {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    lblTituloSalvar.Text = "Confirma a Inclusão do Colaborador?";
                    PainelAtualizacao.Visible = false;
                    Div_Beneficios.Visible = false;
                    aba_Avaliacao.Visible = false;
                    aba_Beneficios.Visible = false;
                    aba_ArquivoMorto.Visible = false;
                    Principal_cmdSalvar.Text = "Incluir";
                    txtsDscColaborador.Focus();
                    LimpaCampos();
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            RegistraScript("");
        }

        #endregion

        #region | Combos

        void PopularCombosConversas()
        {
            FUNCOES.Popula_Combo(ddlConversas_idTipoEvento, "sp_Select 'tbl_Flow_Colaboradores_Conversas_TipoEvento'", "idTipoEvento", "sDscTipoEvento", false, "Selecione a Solicitação", "0");
            FUNCOES.Popula_Combo(ddlConversas_idMeio, "sp_Select 'tbl_Flow_Colaboradores_Conversas_Meio'", "idMeio", "sDscMeio", false, "Selecione um Meio", "0");
            FUNCOES.Popula_Combo(ddlConversas_idAcao, "sp_Select 'tbl_Flow_Colaboradores_Conversas_Acao'", "idAcao", "sDscAcao", false, "Selecione uma Ação", "0");

        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Selecione o Departamento ", "0");

            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");

            FUNCOES.Popula_Combo(ddlidTipoNr, "sp_Select 'Flow_Colaboradores_NR'", "idTipoNr", "sDscNR", false, "Selecione o NR ", "0");
            ddlidTipoNr.Items.Add(new ListItem("Outros", "99"));

            FUNCOES.Popula_Combo(ddlidTipoSanguineo, "sp_Select 'Flow_Colaboradores_TipoSanguineo'", "idTipoSanguineo", "sDscTipoSanguineo", false, "Selecione o Tipo", "0");

            FUNCOES.Popula_Combo(ddlidCargo, "sp_Select 'Flow_Colaboradores_Cargos'", "idCargo", "sDscCargo", false, "Selecione o Cargo", "0");

            FUNCOES.Popula_Combo(ddlidPlanoSaude, "sp_Select 'Flow_Colaboradores_PlanoSaude'", "idPlanoSaude", "sDscPlanoSaude", false, "Selecione o Plano", "0");

            FUNCOES.Popula_Combo(ddlidGrauParentesco, "sp_Select 'Flow_Colaboradores_GrauParentesco'", "idGrauParentesco", "sDscGrauParentesco", false, "Grau de Parentesco", "0");

            FUNCOES.Popula_Combo(ddlEndereco_sUF, "sp_Select 'Flow_Estado'", "sEstado", "sEstado", false, "Selecione o Estado", "");

            FUNCOES.Popula_Combo(ddlidTipoContrato, "sp_Select 'tbl_Flow_Colaboradores_TipoContrato'", "idTipoContrato", "sDscTipoContrato", false, "Selecione o Tipo de Contrato", "0");

            FUNCOES.Popula_Combo(Roupas_ddlidTipoRoupa, "sp_Select 'tbl_Flow_Colaboradores_Roupas_Tipo'", "idTipoRoupa", "sDscTipoRoupa", false, "Selecione o Tipo da Roupa", "0");

            FUNCOES.Popula_Combo(Roupas_ddlidTamanho, "sp_Select 'tbl_Flow_Colaboradores_Tamanho'", "idTamanho", "sDscTamanho", false, "Selecione o Tamanho", "0");

            FUNCOES.Popula_Combo(ddlidVeiculoEquipamentos, "sp_Select 'Flow_Veiculos_Frota'", "idVeiculo", "sFrotaCompleto", false, "Selecione o Veiculo", "0");

            FUNCOES.Popula_Combo(Infracoes_ddlidTipoOcorrencia, "sp_Select 'tbl_Flow_Colaboradores_Ocorrencias_Tipo'", "idTipoOcorrencia", "sDscTipoOcorrencia", false, "Selecione o Tipo", "0");

            FUNCOES.Popula_Combo(ddlidVeiculoOcorrencia, "sp_Select 'Flow_Veiculos_Frota'", "idVeiculo", "sFrotaCompleto", false, "Selecione o Veiculo", "0");

            FUNCOES.Popula_Combo(ddlConversas_idAcao, "sp_Select 'tbl_Flow_Colaboradores_Conversas_Acao'", "idAcao", "sDscAcao", false, "Selecione uma Ação", "0");

            FUNCOES.Popula_Combo(ddlConversas_idMeio, "sp_Select 'tbl_Flow_Colaboradores_Conversas_Meio'", "idMeio", "sDscMeio", false, "Selecione um Meio", "0");

            FUNCOES.Popula_Combo(ddlConversas_idStatusConversa, "sp_Select 'tbl_Flow_Colaboradores_Conversas_Status'", "idStatusConversa", "sDscStatusConversa", false, "Selecione o Status", "0");

            FUNCOES.Popula_Combo(ddlConversas_idTipoEvento, "sp_Select 'tbl_Flow_Colaboradores_Conversas_TipoEvento'", "idTipoEvento", "sDscTipoEvento", false, "Selecione a Solicitação", "0");

            FUNCOES.Popula_Combo(ddlVT_idOpcaoVT, "sp_Select 'tbl_Flow_Colaboradores_VT_Opcao'", "idOpcaoVT", "sDscOpcaoVT", false);

            FUNCOES.Popula_Combo(ddlVT_idTipoVT, "sp_Select 'tbl_Flow_Colaboradores_VT_Tipo'", "idTipoVT", "sDscTipoVT", false, "Selecione o Tipo", "0");
            ddlVT_idTipoVT.Items.Add(new ListItem("Outros", "99"));

            FUNCOES.Popula_Combo(ddlVT_idVTDefinitivo, "sp_Select 'tbl_Flow_Colaboradores_VT_Definitivo'", "idVTDefinitivo", "sDscVTDefinitivo", false, "Selecione uma Opção", "0");

            FUNCOES.Popula_Combo(ddlidResponsavelVeiculo, "sp_Select 'tbl_Flow_Colaboradores_Ocorrencias_Responsavel'", "idResponsavelVeiculo", "sDscResponsavelVeiculo", false, "Selecione um Responsavel", "0");

            FUNCOES.Popula_Combo(ddlidStatusOcorrencia, "sp_Select 'tbl_Flow_Colaboradores_Ocorrencias_Status'", "idStatusOcorrencia", "sDscStatusOcorrencia", false, "Selecione uma Opção", "0");

            FUNCOES.Popula_Combo(ddlidTipoContaDepentente, "sp_Select 'tbl_Flow_Colaboradores_Conta_Dependente'", "idTipoContaDepentente", "sDscTipoContaDepentente", false, "Selecione uma Opção", "0");

            FUNCOES.Popula_Combo(ddlidTipoAvaliacao, "sp_Select 'tbl_Flow_Colaboradores_Avaliacao_Tipo'", "idTipoAvaliacao", "sDscAvaliacao", false, "Selecione uma Opção", "0");

            FUNCOES.Popula_Combo(ddlidSupervisorDireto, "sp_Select 'RRHH_SUPERVISORES'", "idColaborador", "sDscColaborador", false, "Selecione um Supervisor", "0");

            FUNCOES.Popula_Combo(ddlsEscolaridade, "sp_Select 'tbl_Flow_Colaboradores_Escolaridade'", "idEscolaridade", "sDscEscolaridade", false, "Selecione uma Escolaridade", "0");

            FUNCOES.Popula_Combo(ddlsTipoSeguro, sProcedure + " 'Consulta_Seguro_Plano'", "idPlanoSeguro", "sDscSeguradora", false, "Selecione um Plano do Seguro", "0");

            FUNCOES.Popula_Combo(ddlsEstadoCivil, "sp_Select 'tbl_Flow_Colaboradores_EstadoCivil'", "idEstadoCivil", "sDscEstadoCivil", false, "Selecione Estado Civil", "0");
        }

        #endregion

        #region | Utils

        #region | Limpa Campos

        void LimpaCampos()
        {
            hddidColaborador.Value = "0";

            ddlidDepartamento.SelectedValue = "0";
            ddlidEmpresa.SelectedValue = "0";
            ddlidTipoContrato.SelectedValue = "0";

            txtsDscColaborador.Text = "";
            ddlidCargo.SelectedValue = "0";
            txtsEmail.Text = "";
            txtsRamal.Text = "";
            txtsTelCelular.Text = "";
            txtsTelResidencial.Text = "";
            ddlsSituacao.SelectedValue = "S";
            txtdtDesligamento.Text = "";
            txtdtInicioContrato.Text = "";
            txtdtNascimento.Text = "";
            txtsObservacao.Text = "";
            txtsCelularPessoal.Text = "";
            txtsEmailPessoal.Text = "";
            ddlidFuncao.SelectedValue = "0";
            ddlsTipoCTPS.SelectedValue = "0";

            // Documentos
            txtsCPF.Text = "";
            txtsRG.Text = "";
            txtsCNH.Text = "";
            txtdtVencCNH.Text = "";
            txtsPIS.Text = "";
            txtsNIRE.Text = "";
            txtdtRegistro.Text = "";
            txtsTerminoContrato.Text = "";
            ddlsLimiteContrato.SelectedValue = "S";
            txtsCNPJ.Text = "";


            txtsBanco.Text = "";
            ddlsTipoConta.SelectedValue = "C";

            txtsContaBancaria.Text = "";
            txtsAgenciaBancaria.Text = "";
            txtsIE.Text = "";
            txtsIM.Text = "";

            // Beneficios
            ddlidPlanoSaude.SelectedValue = "0";

            // VR
            txtsVR.Text = "";
            ddlsVRDefinitivo.SelectedValue = "S";

            txtdtInclusaoConvenio.Text = "";
            txtdtCarenciaConvenio.Text = "";
            txtnValorPlano.Text = "";

            ddlsDependentesConvenio.SelectedValue = "N";

            // Saúde
            ddlidTipoSanguineo.SelectedValue = "0";
            txtsContatoEmergencia.Text = "";
            txtsDscContatoEmergencia.Text = "";

            // Dados Pessoais - Endereço
            txtEndereco_sCEP.Text = "";
            txtEndereco_sLogradouro.Text = "";
            txtEndereco_sNumero.Text = "";
            txtEndereco_sComplemento.Text = "";
            txtEndereco_sBairro.Text = "";
            txtEndereco_sCidade.Text = "";
            ddlEndereco_sUF.SelectedValue = "";

            imgColaborador.Visible = false;
            imgQr.Visible = false;

            aba_Beneficios.Visible = false;
            aba_Documentos.Visible = false;
            aba_Evento.Visible = false;
            aba_SSTT.Visible = false;
            aba_Conversas.Visible = false;
            aba_Avaliacao.Visible = false;
        }

        void LimpaClasse()
        {
            bs_NR_Itens.Clear();
            bs_EPI_Itens.Clear();
            bs_Ausencia.Clear();
            bs_Evento.Clear();
            bs_Ocorrencias.Clear();
            bs_Dependentes.Clear();
            bs_Conversas.Clear();
            bs_Elogios.Clear();
            bs_Equipamentos.Clear();
            bs_VT.Clear();
            bs_Roupas.Clear();
            bs_Avaliacao.Clear();
        }

        #endregion

        #region | Validação 

        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtsDscColaborador.Text.Length < 6)
                sMensagemErro = "Nome inválido, deve ter no mínimo 6 caracteres!";

            if (ddlidTipoContrato.SelectedItem.ToString() == "PJ" && txtsCNPJ.Text.Length < 18)
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += "CNPJ inválido";
            }

            if (txtsCPF.Text.Length < 14)
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += "CPF inválido";
            }

            if (ddlidDepartamento.SelectedValue == "0")
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += "Departamento inválido!";
            }

            if (ddlidCargo.SelectedValue == "0")
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += "Selecione um Cargo!";
            }

            if (!Validacoes.ValidarEmail(txtsEmail.Text, false))
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += "e-mail inválido!";
            }

            if (!Validacoes.ValidarEmail(txtsEmailPessoal.Text, false))
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += "e-mail pessoal inválido!";
            }

            if (ddlSeguroVida.SelectedValue == "S")
            {
                if (ddlsTipoSeguro.SelectedValue != "0" && !Validacoes.ValidarData(txtdtInicioSeguro))
                {
                    if (sMensagemErro != "")
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a data de Inclusão do Seguro de Vida!";
                }


            }



            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        #endregion

        #region | Imagem

        protected void carregaimgColaborador(string idObjeto, bool bCodigoQR)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_IMAGEM" },
                { "@idTipoArquivo", bCodigoQR ? "82" : "40" },
                { "@idObjeto", idObjeto }
            };
            DataTable dt = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dt.Rows.Count > 0)
            {
                DataRow imgBd = dt.Rows[0];
                string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);

                if (bCodigoQR)
                {
                    imgQr.ImageUrl = imgUrl;
                    imgQr.Visible = true;
                }
                else
                {
                    imgColaborador.ImageUrl = imgUrl;
                    imgColaborador.Visible = true;
                }
            }
        }

        #endregion

        #region | Outros

        void Modal_Abrir(string sModal)
        {
            FUNCOES.Scripts.FecharModal(Page, sModal);
            FUNCOES.Scripts.RemoverBackdrop_Modal(Page);
            FUNCOES.Scripts.AbrirModal(Page, sModal);
            RegistraScript("");
        }

        protected void Modal_Fechar(object sender, EventArgs e)
        {
            LinkButton cmdClick = sender as LinkButton;

            switch (cmdClick.ID.ToString())
            {
                case "cmdInfracao_Cancelar":
                    Modal_Fechar("modal_Infracoes");
                    break;
                case "cmdNR_Cancelar":
                    Modal_Fechar("modal_NR");
                    break;
                case "Ausencia_cmdCancelar":
                    Modal_Fechar("modal_Ausencia");
                    break;
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        void Modal_Fechar(string sModal)
        {
            FUNCOES.Scripts.FecharModal(Page, sModal);
            FUNCOES.Scripts.RemoverBackdrop_Modal(Page);
            RegistraScript("");
        }

        private void RecarregaDatatable()
        {
            EntregasEPI_DataBind();
            dtgNR_DataBind();
            gvArquivoMorto_DataBind();
            gv_PlanoSaude_DataBind();
        }

        public string NovaLinha(object id, string gridNome)
        {
            /* 
            * Passo a passo:
            * 1. Fecha a célula atual
            * 2. Fecha a linha Atual
            * 3. Cria uma nova linha com o ID e a classe <TR id='...' style='...'>
            * 4. Cria uma célula em branco: <TD></TD>
            * 5. Cria uma nova célula para conter o gridview
            ************************************************************/
            if (id != null && !string.IsNullOrEmpty(id.ToString()))
            {
                // Se houver um ID, retorna a nova linha com o ID e a classe
                return string.Format(@"</td></tr><tr id='tr{0}{1}' class='collapsed-row'>
                               <td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            }
            else
            {
                // Se não houver ID, retorna uma string vazia para que nada seja renderizado e o botão de colapso desapareça
                return string.Empty;
            }
        }

        #endregion

        #endregion

        #region | Salvar

        void Salvar_Colaborador()
        {
            if (ValidarDados())
            {
                try
                {
                    string[] vidColaborador = hddidColaborador.Value.Split(',');
                    string idColaborador = vidColaborador[0].ToString();

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        // Bloco Beneficios

                        { "@sFuncao", "SALVAR" },
                        { "@idColaborador", idColaborador },

                        // Bloco Dados do Colaborador
                        { "@sDscColaborador", txtsDscColaborador.Text },
                        { "@sNomeSocial", txtsNomeSocial.Text },
                        { "@idEmpresa", ddlidEmpresa.SelectedValue },
                        { "@idTipoContrato", ddlidTipoContrato.SelectedValue },
                        { "@idDepartamento", ddlidDepartamento.SelectedValue },
                        { "@idCargo", ddlidCargo.SelectedValue },
                        { "@idFuncao", ddlidFuncao.SelectedValue },
                        { "@sEmail", txtsEmail.Text.ToLower() },
                        { "@dtInicioContrato", txtdtInicioContrato.Text },
                        { "@sLimiteContrato", ddlsLimiteContrato.SelectedValue },
                        { "@sTerminoContrato", txtsTerminoContrato.Text },
                        { "@sTelCelular", txtsTelCelular.Text },
                        { "@sRamal", txtsRamal.Text },
                        { "@sDependentesConvenio", ddlsDependentesConvenio.SelectedValue },
                        { "@sSituacao", ddlsSituacao.SelectedValue },
                        { "@dtDesligamento", txtdtDesligamento.Text },
                        { "@sTipoCTPS", ddlsTipoCTPS.SelectedValue },
                        { "@dtPrevPlanoSaude", txtdtPrevPlanoSaude.Text },
                        { "@sRegistro", txtRegistro.Text },
                        { "@idSupervisorDireto", ddlidSupervisorDireto.SelectedValue },

                        // Bloco Dados Pessoais
                        { "@sCEP", txtEndereco_sCEP.Text },
                        { "@sLogradouro", txtEndereco_sLogradouro.Text },
                        { "@sNumero", txtEndereco_sNumero.Text },
                        { "@sComplemento", txtEndereco_sComplemento.Text },
                        { "@sBairro", txtEndereco_sBairro.Text },
                        { "@sCidade", txtEndereco_sCidade.Text },
                        { "@sUF", ddlEndereco_sUF.SelectedValue },
                        { "@dtNascimento", txtdtNascimento.Text },
                        { "@sTelResidencial", txtsTelResidencial.Text },
                        { "@sCelularPessoal", txtsCelularPessoal.Text },
                        { "@sEmailPessoal", txtsEmailPessoal.Text },
                        { "@idEstadoCivil", ddlsEstadoCivil.SelectedValue },
                        { "@sSexo", ddlsSexo.SelectedValue },
                        { "@sPolitico", ddlsPolitico.SelectedValue },

                        // Bloco Documentos
                        { "@sCPF", txtsCPF.Text },
                        { "@sRG", txtsRG.Text },
                        { "@sPIS", txtsPIS.Text },
                        { "@sIE", txtsIE.Text },
                        { "@sIM", txtsIM.Text },
                        { "@sNIRE", txtsNIRE.Text },
                        { "@dtRegistro", txtdtRegistro.Text },
                        { "@sCNH", txtsCNH.Text },
                        { "@dtVencCNH", txtdtVencCNH.Text },
                        { "@sTipoCNH", TipoCNH_Concatenar() },
                        { "@sTipoConta", ddlsTipoConta.SelectedValue },
                        { "@sBanco", txtsBanco.Text },
                        { "@sAgenciaBancaria", txtsAgenciaBancaria.Text },
                        { "@sContaBancaria", txtsContaBancaria.Text },
                        { "@sCNPJ", txtsCNPJ.Text },
                        { "@sObservacao", txtsObservacao.Text },
                        { "@sNomeMae", txtsNomeMae.Text },
                        { "@idEscolaridade", ddlsEscolaridade.SelectedValue },

                        // Saúde
                        { "@idTipoSanguineo", ddlidTipoSanguineo.SelectedValue },
                        { "@sContatoEmergencia", txtsContatoEmergencia.Text },
                        { "@sDscContatoEmergencia", txtsDscContatoEmergencia.Text },

                        // Bloco VR
                        { "@sVR", txtsVR.Text },
                        { "@nValorDiaVR", BD.Conversoes.Numerico(txtnValorDiaVR) },
                        { "@sVRDefinitivo", ddlsVRDefinitivo.SelectedValue },

                        { "sTemPlanoSaude", bs_PlanoSaude.Any(x => x.sFuncao != "EXCLUIR_Plano") ? "S" : ddlTemPlanoSaude.SelectedValue},

                        // Bloco Outros
                        { "@sObsBeneficios", txtsObsBeneficios.Text },

                        // Bloco Seguro de Vida
                        { "@sSeguroVida", ddlSeguroVida.SelectedValue },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                        { "@sRelatorioGastos", ddlsRelatorioGastos.SelectedValue }
                    };

                    if (ddlSeguroVida.SelectedValue == "S")
                    {
                        vParametros.Add("@idPlanoSeguro", ddlsTipoSeguro.SelectedValue);
                        vParametros.Add("@dtInicioSeguro", txtdtInicioSeguro.Text);
                    }

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        idColaborador = RETORNO.DATASET(dsSalvar, 0, "idColaborador");
                        if (Salvar_NR_Itens(idColaborador) && Salvar_Atestado(idColaborador) && Salvar_Ocorrencias(idColaborador) && Salvar_Dependentes(idColaborador) && Salvar_Equipamentos(idColaborador) &&
                            Salvar_VT(idColaborador) && Roupas_Salvar(idColaborador) && Salvar_Avaliacao(idColaborador) && Salvar_Credito(idColaborador) && Salvar_PlanoSaude_Itens(idColaborador))
                        {
                            Pesquisar(idColaborador, false);
                            MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                        }
                        else
                            throw new Exception("Erro ao Salvar!");
                    }
                    else
                        throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }

            }

            RegistraScript("");
        }

        #endregion

        #region | Dependentes 

        void Popular_dtgDependentes(DataSet dsPesquisa)
        {
            BtnSalvarDependente.Visible = false;

            foreach (DataRow row in dsPesquisa.Tables[nTabela_Dependentes].Rows)
            {
                cls_Dependentes objItem = new cls_Dependentes();

                objItem.idLinha = bs_Dependentes.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";
                objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                objItem.idRegistroDependentes = Convert.ToInt32(row["idRegistroDependentes"].ToString());
                objItem.idDependente = Convert.ToInt32(row["idDependente"].ToString());
                objItem.sNomeDependente = row["sNomeDependente"].ToString();
                objItem.sCPFDependente = row["sCPFDependente"].ToString();
                objItem.dtNascDependente = row["dtNascDependente"].ToString();
                objItem.sPensaoDependente = row["sPensaoDependente"].ToString();
                objItem.nValorPensaoDependente = BD.Conversoes.Numerico_Decimal(row["nValorPensaoDependente"].ToString());
                objItem.sObservacaoDependente = row["sObservacaoDependente"].ToString();
                objItem.sDscGrauParentesco = row["sDscGrauParentesco"].ToString();
                objItem.idGrauParentesco = Convert.ToInt32(row["idGrauParentesco"].ToString());
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                objItem.idTipoContaDepentente = Convert.ToInt32(row["idTipoContaDepentente"].ToString());
                objItem.sDscTipoContaDepentente = row["sDscTipoContaDepentente"].ToString();
                objItem.sBancoDependente = row["sBancoDependente"].ToString();
                objItem.sContaDependente = row["sContaDependente"].ToString();
                objItem.sAgencia = row["sAgencia"].ToString();

                bs_Dependentes.Add(objItem);
            }

            dtgDependentes_DataBind();
            Dependente_LimpaCampos();
        }

        protected void cmdIncluirDependentes_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (ValidarDados_Dependentes(ref sMensagem))
            {
                cls_Dependentes objItem = new cls_Dependentes();

                string[] vidColaborador = hddidColaborador.Value.Split(',');
                string idColaborador = vidColaborador[0].ToString();

                objItem.idLinha = bs_Dependentes.Count() + 1;
                objItem.sFuncao = "SALVAR_DEPENDENTE";
                objItem.idColaborador = Convert.ToInt32(idColaborador);
                objItem.sNomeDependente = txtsNomeDependente.Text;
                objItem.sCPFDependente = txtsCPFDependente.Text;
                objItem.dtNascDependente = txtdtNascDependente.Text;
                objItem.nValorPensaoDependente = BD.Conversoes.Numerico_Decimal(txtnValorPensaoDependente.Text);
                objItem.sObservacaoDependente = txtsObservacaoDependente.Text;
                objItem.sPensaoDependente = ddlsPensaoDependente.SelectedItem.ToString();
                objItem.sDscGrauParentesco = ddlidGrauParentesco.SelectedItem.ToString();
                objItem.idGrauParentesco = Convert.ToInt32(ddlidGrauParentesco.SelectedValue.ToString());
                objItem.idTipoContaDepentente = Convert.ToInt32(ddlidTipoContaDepentente.SelectedValue.ToString());
                objItem.sDscTipoContaDepentente = ddlidTipoContaDepentente.SelectedItem.ToString();
                objItem.sContaDependente = txtsContaDependente.Text;
                objItem.sBancoDependente = txtsBancoDependente.Text;
                objItem.sAgencia = txtsAgencia.Text;
                objItem.sNomeArquivo = "";

                bs_Dependentes.Add(objItem);

                dtgDependentes_DataBind();
                Dependente_LimpaCampos();
            }

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador");
        }

        bool Salvar_Dependentes(string idColaborador)
        {
            bool bRetorno = false;

            try
            {
                foreach (cls_Dependentes Dependente_Linha in bs_Dependentes)
                {
                    int idArquivo = Dependente_Linha.idArquivo;
                    if (Dependente_Linha.sNomeArquivo != "" && Dependente_Linha.idArquivo == 0)
                    {
                        cls_Arquivos Arquivo = new cls_Arquivos();
                        Arquivo.idTipoArquivo = 9999;
                        Arquivo.idObjeto = Dependente_Linha.idRegistroDependentes;
                        Arquivo.sNomeArquivo = Dependente_Linha.sNomeArquivo;
                        Arquivo.sDscArquivo = Dependente_Linha.sObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        Arquivo.vbArquivo = Dependente_Linha.objArquivo;
                        Arquivo.dtExpiracaoDoc = "";

                        Dependente_Linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (Dependente_Linha.sFuncao == "SEM ALTERAÇÃO")
                            Dependente_Linha.sFuncao = "SALVAR_DEPENDENTE";
                    }

                    if (Dependente_Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<string, string> vParametroDependentes_Incluir = new Dictionary<string, string>
                        {
                            ["@sFuncao"] = Dependente_Linha.sFuncao,
                            ["idRegistro"] = Dependente_Linha.idRegistroDependentes.ToString(),
                            ["idColaborador"] = idColaborador,
                            ["idDependente"] = Dependente_Linha.idDependente.ToString(),
                            ["sNomeDependente"] = Dependente_Linha.sNomeDependente.ToString(),
                            ["sCPFDependente"] = Dependente_Linha.sCPFDependente.ToString(),
                            ["dtNascDependente"] = Dependente_Linha.dtNascDependente.ToString(),
                            ["sPensaoDependente"] = Dependente_Linha.sPensaoDependente.ToString(),
                            ["nValorPensaoDependente"] = Dependente_Linha.nValorPensaoDependente.ToString().Replace(",", "."),
                            ["sObservacaoDependente"] = Dependente_Linha.sObservacaoDependente.ToString(),
                            ["idGrauParentesco"] = Dependente_Linha.idGrauParentesco.ToString(),
                            ["sBancoDependente"] = Dependente_Linha.sBancoDependente.ToString(),
                            ["sContaDependente"] = Dependente_Linha.sContaDependente.ToString(),
                            ["sAgencia"] = Dependente_Linha.sAgencia.ToString(),
                            ["idTipoContaDepentente"] = Dependente_Linha.idTipoContaDepentente.ToString(),
                            ["@idArquivo"] = Dependente_Linha.idArquivo.ToString()
                        };
                        BD.ExecutarDataSet(sProcedure, vParametroDependentes_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Bloco Dependentes - Erro: " + ex.Message);
            }

            return bRetorno;
        }

        protected void BtnSalvarDependente_Click1(object sender, EventArgs e)
        {
            int idLinha = Convert.ToInt32(hddDependente_idLinha.Value);
            int index = bs_Dependentes.FindIndex(x => x.idLinha.Equals(idLinha));
            decimal nValor1 = 0;
            decimal nValor2 = 0;

            bs_Dependentes[index].sNomeDependente = txtsNomeDependente.Text;
            bs_Dependentes[index].sCPFDependente = txtsCPFDependente.Text;
            bs_Dependentes[index].dtNascDependente = txtdtNascDependente.Text;
            bs_Dependentes[index].idGrauParentesco = Convert.ToInt32(ddlidGrauParentesco.SelectedValue);
            bs_Dependentes[index].sPensaoDependente = ddlsPensaoDependente.SelectedItem.ToString();

            decimal.TryParse(txtnValorPensaoDependente.Text.Replace(".", ","), out nValor2);
            bs_Dependentes[index].nValorPensaoDependente = nValor2;
            bs_Dependentes[index].sObservacaoDependente = txtsObservacaoDependente.Text;
            bs_Dependentes[index].idTipoContaDepentente = Convert.ToInt32(ddlidTipoContaDepentente.SelectedValue);
            bs_Dependentes[index].sBancoDependente = txtsBancoDependente.Text;
            bs_Dependentes[index].sContaDependente = txtsContaDependente.Text;
            bs_Dependentes[index].sAgencia = txtsAgencia.Text;
            bs_Dependentes[index].sFuncao = "SALVAR_DEPENDENTE";

            dtgDependentes_DataBind();
            Dependente_LimpaCampos();
            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador");
        }

        void Dependente_LimpaCampos()
        {
            txtsNomeDependente.Text = "";
            txtsCPFDependente.Text = "";
            txtdtNascDependente.Text = "";
            ddlsPensaoDependente.SelectedValue = "N";
            ddlidGrauParentesco.SelectedValue = "0";
            txtnValorPensaoDependente.Text = "";
            txtsObservacaoDependente.Text = "";

            ddlidTipoContaDepentente.SelectedValue = "0";
            txtsContaDependente.Text = "";
            txtsBancoDependente.Text = "";
            txtsAgencia.Text = "";

            cmdDependente.Visible = true;
            BtnSalvarDependente.Visible = false;
        }

        private bool ValidarDados_Dependentes(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtsNomeDependente.Text.Length < 6)
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += "Insira um nome para o Dependente de no mínimo 6 caracteres!";
            }

            if (ddlidGrauParentesco.SelectedValue == "0")
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += "Selecione o Grau de Parentesco!";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina_Dependentes.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void dtgDependentes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtgDependentes.Rows[e.RowIndex].Cells[0].Text);
            bs_Dependentes[bs_Dependentes.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_DEPENDENTE";
            dtgDependentes_DataBind();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador");
        }

        protected void dtgDependentes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 1;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkDependente_Download" && sNomeArquivo == "") || (lnk.ID == "lnkDependente_UpLoad" && sNomeArquivo != ""))
                        lnk.Visible = false;
                }
            }

            GRID.EsconderColunas(e, 0);
        }

        protected void dtgDependentes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "Dependentes";
            hddDependente_idLinha.Value = idLinha.ToString();

            if (e.CommandName == "Upload_Arquivo")
                AbrirModal_EnvioArquivo(eBloco.Dependentes, idLinha, bs_Dependentes[bs_Dependentes.FindIndex(x => x.idLinha.Equals(idLinha))].sNomeDependente);
            else if (e.CommandName == "Download_Arquivo")
                Efetuar_Download_Arquivo(eBloco.Dependentes, idLinha);
            else if (e.CommandName == "Editar")
            {
                cmdDependente.Visible = false;
                BtnSalvarDependente.Visible = true;

                var Dependentes = bs_Dependentes[bs_Dependentes.FindIndex(x => x.idLinha.Equals(idLinha))];

                txtsNomeDependente.Text = Dependentes.sNomeDependente;
                txtsCPFDependente.Text = Dependentes.sCPFDependente;
                txtdtNascDependente.Text = Dependentes.dtNascDependente;
                txtnValorPensaoDependente.Text = Dependentes.nValorPensaoDependente.ToString();

                if (Dependentes.sPensaoDependente == "Não")
                    ddlsPensaoDependente.SelectedValue = "N";
                else
                    ddlsPensaoDependente.SelectedValue = "S";

                ddlidGrauParentesco.SelectedValue = Dependentes.idGrauParentesco.ToString();
                ddlidTipoContaDepentente.SelectedValue = Dependentes.idTipoContaDepentente.ToString();
                txtsContaDependente.Text = Dependentes.sContaDependente;
                txtsBancoDependente.Text = Dependentes.sBancoDependente;
                txtsAgencia.Text = Dependentes.sAgencia;
                txtsObservacaoDependente.Text = Dependentes.sObservacaoDependente;

                txtsNomeDependente.Focus();
            }

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador");
        }

        void dtgDependentes_DataBind()
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_dtgDependentes_DataBind", GRID.DataBindComScriptData(dtgDependentes, bs_Dependentes.Where(c => c.sFuncao.ToString() != "EXCLUIR_DEPENDENTE").ToList(), 2, "asc", "false", "''"), true);
                RegistraScript("");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Dependentes: " + ex.Message);
            }
        }

        #endregion

        #region | Tipo CNH

        void TipoCNH_Popular(string sTipoCNH)
        {
            string[] vidTipoCNH = sTipoCNH.Split(';');

            for (int i = 0; i < vidTipoCNH.Count(); i++)
            {
                for (int contador = 0; contador <= cblsTipoCNH.Items.Count - 1; contador++)
                {
                    if (cblsTipoCNH.Items[contador].Value == vidTipoCNH[i].ToString())
                        cblsTipoCNH.Items[contador].Selected = true;
                }

            }
        }

        string TipoCNH_Concatenar()
        {
            string sRetornoConcatenado = "";

            for (int contador = 0; contador <= cblsTipoCNH.Items.Count - 1; contador++)
            {
                if (cblsTipoCNH.Items[contador].Selected)
                    sRetornoConcatenado += string.Concat(cblsTipoCNH.Items[contador].Value, ";");
            }

            return sRetornoConcatenado;
        }

        #endregion

        #region | Aba Colaborador

        void PopularCombo_Funcao()
        {
            string idFuncaoCarregada = ddlidFuncao.SelectedValue;
            FUNCOES.Popula_Combo(ddlidFuncao, string.Format("sp_Manipula_tbl_Flow_Colaboradores_FuncaoCarteira @sFuncao='Popular_Combo', @idEmpresa={0}, @idDepartamento={1}, @idTipoContrato={2}", ddlidEmpresa.SelectedValue, ddlidDepartamento.SelectedValue, ddlidTipoContrato.SelectedValue), "idFuncao", "sDscFuncao", false, "Selecione uma Função", "0");

            if (idFuncaoCarregada != "0")
            {
                if (Convert.ToInt32(ddlidTipoContrato.SelectedValue) != Convert.ToInt32(ViewState["idTipoContrato"]))
                    ddlidFuncao.SelectedValue = "0";
                else
                    ddlidFuncao.SelectedValue = idFuncaoCarregada;
            }
        }

        private void PopularDadosFuncao(int funcao)
        {
            if (funcao != 0)
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR" },
                        { "@idFuncao", funcao.ToString() }
                    };
                DataSet dsFuncao = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_FuncaoCarteira", vParametros);

                sCBO.Text = RETORNO.DATASET(dsFuncao, 0, "sCBO");
                txtsDscGHE.Text = RETORNO.DATASET(dsFuncao, 0, "sDscGHE");
                txtsDscGHESetor.Text = RETORNO.DATASET(dsFuncao, 0, "sDscGHE_Setor");

            }
        
        }

        #endregion

        #region | Aba Documentos 

        void Popular_Aba_Documentos(string idColaborador)
        {
            aba_Documentos.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.ExibirDocumentos_RH, false);
            frmDocumentos.Attributes.Add("src", string.Format("../RRHH/Documentos_RRHH.aspx?idObjeto={0}&sTipoObjeto={1}", idColaborador, "RRHH"));
        }

        void Popular_Aba_DocumentosSSTT(string idColaborador) => aba_SSTT.Visible = true;

        void Popular_Aba_Ocorrencias(string idColaborador)
        {
            frmEvento.Attributes.Add("src", string.Format("../RRHH/Documentos_RRHH.aspx?idObjeto={0}&sTipoObjeto={1}", idColaborador, "Ausencia"));
            aba_Evento.Visible = true;
        }

        void Popular_aba_Beneficios(string idColaborador)
        {
            frmEvento.Attributes.Add("src", string.Format("../RRHH/Documentos_RRHH.aspx?idObjeto={0}&sTipoObjeto={1}", idColaborador, "Beneficios"));
            aba_Evento.Visible = true;
        }

        #endregion

        #region | Aba SSTT

        #region | NR 

        void Popular_dtgNR(DataSet dsPesquisa)
        {
            if (dsPesquisa.Tables[nTabela_NR].Rows.Count > 0)
            {
                foreach (DataRow row in dsPesquisa.Tables[nTabela_NR].Rows)
                {
                    cls_NR_Itens objItem = new cls_NR_Itens();

                    objItem.idLinha = bs_NR_Itens.Count() + 1;
                    objItem.sFuncao = "SEM ALTERAÇÃO";
                    objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                    objItem.idRegistroNR = Convert.ToInt32(row["idRegistroNR"].ToString());
                    objItem.idTipoNr = Convert.ToInt32(row["idTipoNr"].ToString());
                    objItem.sDscNR = row["sDscNR"].ToString();
                    objItem.dtEmissaoNR = row["dtEmissaoNR"].ToString();
                    objItem.dtVencimentoNR = row["dtVencimentoNR"].ToString();
                    objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                    objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                    objItem.sObservacaoArquivo = row["sDscArquivo"].ToString();

                    bs_NR_Itens.Add(objItem);
                }
            }
            else
                btDownloadArquivo.Visible = false;

            dtgNR_DataBind();
            NR_LimpaCampos();
        }

        protected void cmdIncluirItem_Click(object sender, EventArgs e)
        {
            NR_LimpaCampos();
            Modal_Abrir("modal_NR");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");
        }

        bool Salvar_NR_Itens(string idColaborador)
        {
            bool bRetorno = false;

            try
            {
                foreach (var linha in bs_NR_Itens)
                {
                    int idArquivo = linha.idArquivo;

                    if (linha.sNomeArquivo != "" && linha.idArquivo == 0)
                    {
                        cls_Arquivos Arquivo = new cls_Arquivos();
                        Arquivo.idTipoArquivo = 9999;
                        Arquivo.idObjeto = linha.idRegistroNR;
                        Arquivo.sNomeArquivo = linha.sNomeArquivo;
                        Arquivo.sDscArquivo = linha.sObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        Arquivo.vbArquivo = linha.objArquivo;
                        Arquivo.dtExpiracaoDoc = linha.dtVencimentoNR;

                        linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (linha.sFuncao == "SEM ALTERAÇÃO")
                            linha.sFuncao = "SALVAR_NR";
                    }

                    if (linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<string, string> vParametroItens_Incluir = new Dictionary<string, string>
                        {
                            ["@sFuncao"] = linha.sFuncao,
                            ["@idRegistro"] = linha.idRegistroNR.ToString(),
                            ["@idColaborador"] = idColaborador,
                            ["@idTipoNr"] = linha.idTipoNr.ToString(),
                            ["@sDscNR"] = linha.sDscNR,
                            ["@dtEmissaoNR"] = linha.dtEmissaoNR,
                            ["@dtVencimentoNR"] = linha.dtVencimentoNR,
                            ["@sObservacao"] = linha.sObservacaoArquivo,
                            ["@idArquivo"] = linha.idArquivo.ToString(),
                            ["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario()
                        };
                        BD.ExecutarDataSet(sProcedure, vParametroItens_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Bloco NR - Erro: " + ex.Message);
            }

            return bRetorno;
        }

        protected void BtnSalvarNR_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_NR(ref sMensagem))
            {
                if (hddNR_idLinha.Value == "0")
                {
                    cls_NR_Itens objItem = new cls_NR_Itens();

                    string[] vidColaborador = hddidColaborador.Value.Split(',');
                    string idColaborador = vidColaborador[0].ToString();

                    objItem.idLinha = bs_NR_Itens.Count() + 1;
                    objItem.sFuncao = "SALVAR_NR";
                    objItem.idColaborador = Convert.ToInt32(idColaborador);
                    objItem.idTipoNr = Convert.ToInt32(ddlidTipoNr.SelectedValue);
                    objItem.sDscNR = ddlidTipoNr.SelectedValue == "99" ? txtTipoNrOutros.Text : ddlidTipoNr.SelectedItem.ToString();
                    objItem.dtEmissaoNR = txtdtEmissaoNR.Text.ToString();
                    objItem.dtVencimentoNR = txtdtVencimentoNR.Text.ToString();
                    objItem.sObservacaoArquivo = txtObservacaoNR.Text;
                    objItem.sNomeArquivo = "";

                    if (fu_EnviarArquivo_NR.HasFile)
                    {
                        Byte[] lObjArquivo = null;

                        try
                        {
                            Arquivo objArquivo = new Arquivo();
                            lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(fu_EnviarArquivo_NR.FileName, fu_EnviarArquivo_NR.PostedFile.InputStream);

                            objItem.idArquivo = 0;
                            objItem.sNomeArquivo = fu_EnviarArquivo_NR.FileName;
                            objItem.objArquivo = lObjArquivo;
                            objItem.sDscArquivo = fu_EnviarArquivo_NR.FileName;
                        }
                        catch { }
                    }

                    bs_NR_Itens.Add(objItem);

                }
                else
                {
                    int idLinha = Convert.ToInt32(hddNR_idLinha.Value);
                    int index = bs_NR_Itens.FindIndex(x => x.idLinha.Equals(idLinha));

                    bs_NR_Itens[index].idTipoNr = Convert.ToInt32(ddlidTipoNr.SelectedValue);
                    bs_NR_Itens[index].sDscNR = ddlidTipoNr.SelectedValue == "99" ? txtTipoNrOutros.Text : ddlidTipoNr.SelectedItem.ToString();
                    bs_NR_Itens[index].dtEmissaoNR = txtdtEmissaoNR.Text;
                    bs_NR_Itens[index].dtVencimentoNR = txtdtVencimentoNR.Text;
                    bs_NR_Itens[index].sObservacaoArquivo = txtObservacaoNR.Text;
                    bs_NR_Itens[index].sFuncao = "SALVAR_NR";

                    if (bs_NR_Itens[index].idArquivo != 0 && fu_EnviarArquivo_NR.HasFile && bs_NR_Itens[index].idRegistroNR > 0)
                    {
                        Dictionary<string, string> vParametroItens = new Dictionary<string, string>
                        {
                            ["@sFuncao"] = "Atualiza_Arquivo_NR",
                            ["@idRegistro"] = bs_NR_Itens[index].idRegistroNR.ToString(),
                            ["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario()
                        };
                        BD.ExecutarDataSet(sProcedure, vParametroItens);

                        Byte[] lObjArquivo = null;

                        try
                        {
                            Arquivo objArquivo = new Arquivo();
                            lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(fu_EnviarArquivo_NR.FileName, fu_EnviarArquivo_NR.PostedFile.InputStream);

                            bs_NR_Itens[index].idArquivo = 0;
                            bs_NR_Itens[index].sNomeArquivo = fu_EnviarArquivo_NR.FileName;
                            bs_NR_Itens[index].objArquivo = lObjArquivo;
                            bs_NR_Itens[index].sDscArquivo = fu_EnviarArquivo_NR.FileName;
                        }
                        catch { }
                    }
                }

                NR_LimpaCampos();
                Modal_Fechar("modal_NR");
                dtgNR_DataBind();
                RecarregaDatatable();
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");

            }
            else
            {
                MensagemPagina_ModalNR.MostraMensagem_Erro(sMensagem, false);
                Modal_Abrir("modal_NR");
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");

            }

        }

        void NR_LimpaCampos()
        {
            BtnSalvarNR.Text = "Incluir";

            if (hddNR_idLinha.Value != "0")
                BtnSalvarNR.Text = "Salvar";


            hddNR_idLinha.Value = "0";
            ddlidTipoNr.SelectedValue = "0";

            txtdtEmissaoNR.Text = "";
            txtdtVencimentoNR.Text = "";
            txtObservacaoNR.Text = "";
        }

        private bool ValidarDados_NR(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlidTipoNr.SelectedValue == "0")
                sMensagemErro += "Selecione um NR!";

            if (txtdtEmissaoNR.Text.Length < 6)
            {
                if (sMensagemErro != "")
                    sMensagemErro += "</br>";

                sMensagemErro += "Data Emissão inválida!";
            }

            if (txtdtVencimentoNR.Text.Length < 6)
            {
                if (sMensagemErro != "")
                    sMensagemErro += "</br>";

                sMensagemErro += "Data Vencimento inválida!";
            }

            string dt = Validacoes.ValidaDatas(txtdtEmissaoNR.Text, txtdtVencimentoNR.Text);
            sMensagemErro += string.IsNullOrEmpty(dt) ? string.Empty : "</br>" + dt;

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina_ModalNR.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void dtgNR_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtgNR.Rows[e.RowIndex].Cells[1].Text);
            bs_NR_Itens[bs_NR_Itens.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_NR";
            RecarregaDatatable();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");
        }

        protected void dtgNR_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 1;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //------------------Higor Maestrello 18-06-2024------------------------------

                e.Row.Attributes.Add("id", dtgNR.DataKeys[e.Row.RowIndex]["idArquivo"].ToString());

                string idArquivo = dtgNR.DataKeys[e.Row.RowIndex]["idArquivo"].ToString();
                if (idArquivo.Equals("0"))
                    (e.Row.Cells[0].FindControl("chkTarefas_Seleciona") as CheckBox).Visible = false;

                //---------------------------------------------------------------------------

                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkDownload" && sNomeArquivo == "") || (lnk.ID == "lnkUpLoad" && sNomeArquivo != ""))
                        lnk.Visible = false;
                }
            }

            GRID.EsconderColunas(e, 1);
        }

        protected void dtgNR_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "NR";
            hddNR_idLinha.Value = idLinha.ToString();

            if (e.CommandName == "Upload_Arquivo")
                AbrirModal_EnvioArquivo(eBloco.NR, idLinha, bs_NR_Itens[bs_NR_Itens.FindIndex(x => x.idLinha.Equals(idLinha))].sDscNR);
            else if (e.CommandName == "Download_Arquivo")
                Efetuar_Download_Arquivo(eBloco.NR, idLinha);
            else if (e.CommandName == "Editar")
            {
                var NR = bs_NR_Itens[bs_NR_Itens.FindIndex(x => x.idLinha.Equals(idLinha))];
                NR_LimpaCampos();
                ddlidTipoNr.SelectedValue = NR.idTipoNr.ToString();
                txtTipoNrOutros.Text = ddlidTipoNr.SelectedValue == "99" ? NR.sDscNR : "";
                txtdtEmissaoNR.Text = NR.dtEmissaoNR;
                txtdtVencimentoNR.Text = NR.dtVencimentoNR;
                txtObservacaoNR.Text = NR.sObservacaoArquivo;
                Modal_Abrir("modal_NR");
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");
            }
            else if (e.CommandName == "Download")
            {
                Efetuar_Download_Arquivo(eBloco.NR, idLinha);
                RecarregaDatatable();
            }

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");
        }

        void dtgNR_DataBind()
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_dtgNR_DataBind", GRID.DataBindComScriptData(dtgNR, bs_NR_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR_NR").ToList(), 1, new int[2] { 4, 5 }, "asc", "false", "''"), true);
                RegistraScript("");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Itens: " + ex.Message);
            }
        }

        //------------------ Higor Maestrello 18-06-2024 ------------------------------
        void DownloadArquivoCheck(string arquivoId)
        {
            Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
            {
                {"@sFuncao", "CONSULTAR_DETALHECHECK" },
                {"@idArquivoCheck", arquivoId}
            };
            DataTable dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

            string zipFileName = $"Arquivos_{DateTime.Now:yyyyMMddHHmmss}.zip";
            string zipFilePath = Server.MapPath($"~/Download/{zipFileName}");

            using (FileStream zipFile = new FileStream(zipFilePath, FileMode.Create))
            {
                using (ZipArchive zipArchive = new ZipArchive(zipFile, ZipArchiveMode.Create))
                {
                    foreach (DataRow item in dtArquivo.Rows)
                    {
                        using (Stream entryStream = zipArchive.CreateEntry(item["sNomeArquivo"].ToString().Replace(",", "")).Open())
                        {
                            byte[] arquivoBytes = (byte[])item["vbArquivo"];
                            entryStream.Write(arquivoBytes, 0, arquivoBytes.Length);
                        }
                    }
                }
            }

            FUNCOES.DownloadArquivo(Page, zipFileName);
            Pesquisar(hddidColaborador.Value, false);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "aba_SSTT", "$('#SSTT-tab').tab('show');", true);
        }
        //-----------------------------------------------------------------------------

        protected void ddlidTipoNr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidTipoNr.SelectedValue == "99")
            {
                div_TipoNrOutros.Visible = true;
                div_ddlTipoNr.Attributes.Add("class", "col-lg-3");
            }
            else
            {
                div_ddlTipoNr.Attributes.Add("class", "col-lg-6");
                div_TipoNrOutros.Visible = false;
            }
            //Modal_Abrir("modal_NR");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");
            txtdtEmissaoNR.Focus();
        }

        protected void txtdtEmissaoNR_TextChanged(object sender, EventArgs e)
        {
            if (ddlidTipoNr.SelectedValue != "0")
            {
                Dictionary<string, string> vParametroItens = new Dictionary<string, string>
                {
                    ["@sFuncao"] = "Consulta_Validade_NR",
                    ["@idColaborador"] = hddidColaborador.Value,
                    ["@idTipoNR"] = ddlidTipoNr.SelectedValue
                };
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametroItens);
                if (BD.ValidarDataSet(ds, out string sErro))
                {
                    int nValidade = Convert.ToInt32(RETORNO.DATASET(ds, 0, "nValidade"));
                    string sPeriodicidade = RETORNO.DATASET(ds, 0, "sPeriodicidade");

                    if (nValidade != 0)
                    {
                        if (DateTime.TryParseExact(txtdtEmissaoNR.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dtEmissao))
                        {
                            switch (sPeriodicidade)
                            {
                                case "A":
                                    dtEmissao = dtEmissao.AddYears(nValidade);
                                    break;
                                case "M":
                                    dtEmissao = dtEmissao.AddMonths(nValidade);
                                    break;
                            }

                            txtdtVencimentoNR.Text = dtEmissao.ToString("dd/MM/yyyy");
                            txtdtVencimentoNR.Focus();
                        }
                    }

                }
                else
                {
                    MensagemPagina_ModalNR.MostraMensagem_Erro(sErro);
                }

            }
            else
            {
                txtdtEmissaoNR.Text = "";
                MensagemPagina_ModalNR.MostraMensagem_Erro("Selecione um Tipo de NR");
            }

            //Modal_Abrir("modal_NR");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");
        }

        protected void cmdNR_Cancelar_Click(object sender, EventArgs e)
        {
            Modal_Fechar("modal_NR");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");
        }

        #endregion

        #region | EPI

        protected void Popula_EntregasEPI(DataTable dt)
        {
            bs_EntregasEPI.Clear();
            bs_EntregasEPI = dt.AsEnumerable()
                        .GroupBy(row => new
                        {
                            idEntregaEPI = Convert.ToInt32(row["idEntregaEPI"]),
                            sPeriodicidade = row["sPeriodicidade"].ToString(),
                            sTipoEntrega = row["sTipoEntrega"].ToString(),
                            sDscStatus = row["sDscStatus"].ToString(),
                            dtSolicitacao = row["dtSolicitacao"].ToString(),
                            dtEntrega = row["dtEntrega"].ToString(),
                            dtConfirmacao = row["dtConfirmacao"].ToString(),
                            idArquivo = Convert.ToInt32(row["idArquivo"])
                        }).Select(g => new cls_EntregaEPI
                        {
                            idEntregaEPI = g.Key.idEntregaEPI,
                            sPeriodicidade = g.Key.sPeriodicidade,
                            sTipoEntrega = g.Key.sTipoEntrega,
                            sDscStatus = g.Key.sDscStatus,
                            dtSolicitacao = g.Key.dtSolicitacao,
                            dtEntrega = g.Key.dtEntrega,
                            dtConfirmacao = g.Key.dtConfirmacao,
                            idArquivo = g.Key.idArquivo,
                            lsItensEPI = g.Select(row => new cls_ItemEPI
                            {
                                idItem = Convert.ToInt32(row["idItem"]),
                                nOrdem = Convert.ToInt32(row["nOrdem"]),
                                sCodigo = row["sCodigo"].ToString(),
                                sDscProduto = row["sDscProduto"].ToString(),
                                nQuantidadeEPI = Convert.ToInt32(row["nQuantidadeEPI"]),
                                nCA = Convert.ToInt32(row["nCA"]),
                                sPeriodo = row["sPeriodo"].ToString(),
                                dtVencimento = row["dtVencimento"].ToString(),
                                idEntregaEPI = Convert.ToInt32(row["idEntregaEPI"])
                            }).OrderBy(x => x.nOrdem).ToList()
                        }).OrderByDescending(e => e.dtEntrega_Date).ToList();

            var episMaisRecentes = bs_EntregasEPI
                .SelectMany(entrega => entrega.lsItensEPI.Select(item => new
                {
                    Item = item,
                    entrega.sPeriodicidade,
                    DataEntrega = entrega.dtEntrega_Date
                })).ToList().GroupBy(x => x.Item.idItem).Select(g => g.Where(x => x.sPeriodicidade == "S").OrderByDescending(x => x.DataEntrega).FirstOrDefault()?.Item).Where(x => x != null);

            foreach (var item in episMaisRecentes)
            {
                item.bMaisRecente = true;
            }

            EntregasEPI_DataBind();
        }

        void EntregasEPI_DataBind() { gv_EPI.DataSource = bs_EntregasEPI; gv_EPI.DataBind(); }

        protected void gv_EPI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "gvMainTh";
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = "gvMainTd";

                var gvItens = e.Row.FindControl("gv_EPI_itens") as GridView;
                var entrega = e.Row.DataItem as cls_EntregaEPI;

                if (entrega.idArquivo <= 0)
                    e.Row.FindControl("lnkEPI_Download").Visible = false;

                gvItens.DataSource = entrega.lsItensEPI;
                gvItens.DataBind();
            }
        }

        protected void gv_EPI_RowCommand(object sender, GridViewCommandEventArgs e) { Efetuar_Download_Arquivo(eBloco.EPI, Convert.ToInt32(e.CommandArgument), true); FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab"); }

        protected void gv_EPI_itens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = "gvChildRow";

                bool bMaisRecente = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "bMaisRecente") ?? false);

                if (bMaisRecente && DateTime.TryParseExact(DataBinder.Eval(e.Row.DataItem, "dtVencimento")?.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtVencimento))
                {
                    if (dtVencimento < DateTime.Today)
                        e.Row.CssClass = "danger";
                }
            }
        }

        #region | Antigos

        void Popular_dtgEPI(DataSet dsPesquisa)
        {
            div_EPIs_Antigos.Visible = false;

            foreach (DataRow row in dsPesquisa.Tables[nTabela_EPI].Rows)
            {
                cls_EPI_Itens objItem = new cls_EPI_Itens();

                objItem.idLinha = bs_EPI_Itens.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";
                objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                objItem.idRegistroEPI = Convert.ToInt32(row["idRegistroEPI"].ToString());
                objItem.idEPI = Convert.ToInt32(row["idEPI"].ToString());
                objItem.sEPI = row["sEPI"].ToString();
                objItem.sTamanhoEPI = row["sTamanhoEPI"].ToString();
                objItem.dtRecebimentoEPI = row["dtRecebimentoEPI"].ToString();
                objItem.dtVencimentoEPI = row["dtVencimentoEPI"].ToString();
                objItem.sObservacaoEPI = row["sObservacaoEPI"].ToString();
                objItem.sQuantidadeEPI = row["sQuantidadeEPI"].ToString();
                objItem.sCA = row["sCA"].ToString();
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());

                bs_EPI_Itens.Add(objItem);
            }

            dtgEPI_DataBind(false);
        }

        protected void dtgEPI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();
                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if (lnk.ID == "lnkEPI_Download" && sNomeArquivo == "")
                        lnk.Visible = false;
                }
            }

            GRID.EsconderColunas(e, 0);
        }

        protected void dtgEPI_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "EPI";
            hddEPI_idLinha.Value = idLinha.ToString();

            Efetuar_Download_Arquivo(eBloco.EPI, idLinha);

            RegistraScript("");

            RecarregaDatatable();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");
        }

        void dtgEPI_DataBind(bool bFechar)
        {
            if (bFechar)
            {
                dtgEPI.DataSource = null;
                dtgEPI.DataBind();
            }
            else
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_dtgEPI_DataBind", GRID.DataBindComScriptData(dtgEPI, bs_EPI_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR_EPI").ToList(), new int[2] { 3, 4 }, "asc", "false", "''"), true);
        }

        #endregion

        #endregion

        #region | Roupas

        void Roupas_Popular(DataSet dsPesquisa)
        {
            Roupas_BtnSalvar.Visible = false;

            foreach (DataRow row in dsPesquisa.Tables[nTabela_Roupas].Rows)
            {
                cls_Roupas objItem = new cls_Roupas();

                objItem.IdLinha = bs_Roupas.Count() + 1;
                objItem.SFuncao = "SEM ALTERAÇÃO";
                objItem.IdColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                objItem.IdRegistroRoupa = Convert.ToInt32(row["idRegistroRoupa"].ToString());
                objItem.IdTipoRoupa = Convert.ToInt32(row["idTipoRoupa"].ToString());
                objItem.SDscTipoRoupa = row["sDscTipoRoupa"].ToString();
                objItem.IdTamanho = Convert.ToInt32(row["idTamanho"].ToString());
                objItem.SDscTamanho = row["sDscTamanho"].ToString();
                objItem.SCorRoupa = row["sCorRoupa"].ToString();
                objItem.NQuantidade = Convert.ToInt32(row["nQuantidade"].ToString());
                objItem.DtEntregaRoupa = row["dtEntrega"].ToString();
                objItem.SObservacao = row["sObservacao"].ToString();
                objItem.SNomeArquivo = row["sNomeArquivo"].ToString();
                objItem.IdArquivo = Convert.ToInt32(row["idArquivo"].ToString());

                bs_Roupas.Add(objItem);
            }

            Roupas_GV_DataBind();
            Roupas_LimpaCampos();
        }

        protected void Roupas_cmdIncluir_Click(object sender, EventArgs e)
        {
            if (Roupas_ValidarDados())
            {
                cls_Roupas objItem = new cls_Roupas();

                string[] vidColaborador = hddidColaborador.Value.Split(',');
                string idColaborador = vidColaborador[0].ToString();

                objItem.IdLinha = bs_Roupas.Count() + 1;
                objItem.SFuncao = "SALVAR_ROUPAS";
                objItem.IdColaborador = Convert.ToInt32(idColaborador);
                objItem.IdTipoRoupa = Convert.ToInt32(Roupas_ddlidTipoRoupa.SelectedValue.ToString());
                objItem.SDscTipoRoupa = Roupas_ddlidTipoRoupa.SelectedItem.ToString();
                objItem.IdTamanho = Convert.ToInt32(Roupas_ddlidTamanho.SelectedValue.ToString());
                objItem.SDscTamanho = Roupas_ddlidTamanho.SelectedItem.ToString();
                objItem.SCorRoupa = Roupas_txtsCor.Text.ToString();
                objItem.NQuantidade = Convert.ToInt32(Roupas_txtQuantidade.Text.ToString());
                objItem.DtEntregaRoupa = Roupas_txtdtEntrega.Text.ToString();
                objItem.SObservacao = Roupas_txtsObservacao.Text.ToString();
                objItem.SNomeArquivo = "";

                bs_Roupas.Add(objItem);

                Roupas_GV_DataBind();
                Roupas_LimpaCampos();
            }

            RegistraScript("$('[id$=Roupas_txtsObservacao]').focus();");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");
        }

        bool Roupas_Salvar(string idColaborador)
        {
            bool bRetorno = false;

            try
            {
                foreach (var Roupas_Linha in bs_Roupas)
                {
                    int idArquivo = Roupas_Linha.IdArquivo;
                    if (Roupas_Linha.SNomeArquivo != "" && Roupas_Linha.IdArquivo == 0)
                    {
                        cls_Arquivos Arquivo = new cls_Arquivos();
                        Arquivo.idTipoArquivo = 9999;
                        Arquivo.idObjeto = Roupas_Linha.IdRegistroRoupa;
                        Arquivo.sNomeArquivo = Roupas_Linha.SNomeArquivo;
                        Arquivo.sDscArquivo = Roupas_Linha.SObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        Arquivo.vbArquivo = Roupas_Linha.ObjArquivo;
                        Arquivo.dtExpiracaoDoc = "";

                        Roupas_Linha.IdArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (Roupas_Linha.SFuncao == "SEM ALTERAÇÃO")
                            Roupas_Linha.SFuncao = "SALVAR_ROUPAS";
                    }

                    if (Roupas_Linha.SFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<string, string> vParametros_Roupas_Incluir = new Dictionary<string, string>
                        {
                            ["@sFuncao"] = Roupas_Linha.SFuncao,
                            ["@idRegistro"] = Roupas_Linha.IdRegistroRoupa.ToString(),
                            ["@idColaborador"] = idColaborador,
                            ["@idTipoRoupa"] = Roupas_Linha.IdTipoRoupa.ToString(),
                            ["@idTamanho"] = Roupas_Linha.IdTamanho.ToString(),
                            ["@sCorRoupa"] = Roupas_Linha.SCorRoupa.ToString(),
                            ["@nQuantidade"] = Roupas_Linha.NQuantidade.ToString(),
                            ["@DtEntregaRoupa"] = Roupas_Linha.DtEntregaRoupa.ToString(),
                            ["@sObservacao"] = Roupas_Linha.SObservacao.ToString(),
                            ["@idArquivo"] = Roupas_Linha.IdArquivo.ToString()
                        };
                        BD.ExecutarDataSet(sProcedure, vParametros_Roupas_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Bloco Roupas - Erro: " + ex.Message);
            }

            return bRetorno;
        }

        protected void Roupas_BtnSalvar_Click(object sender, EventArgs e)
        {
            int idLinha = Convert.ToInt32(hddRoupas_idLinha.Value);
            int index = bs_Roupas.FindIndex(x => x.IdLinha.Equals(idLinha));

            bs_Roupas[index].IdTipoRoupa = Convert.ToInt32(Roupas_ddlidTipoRoupa.SelectedValue);
            bs_Roupas[index].IdTamanho = Convert.ToInt32(Roupas_ddlidTamanho.SelectedValue);
            bs_Roupas[index].DtEntregaRoupa = Roupas_txtdtEntrega.Text;
            bs_Roupas[index].NQuantidade = Convert.ToInt32(Roupas_txtQuantidade.Text);
            bs_Roupas[index].SCorRoupa = Roupas_txtsCor.Text;
            bs_Roupas[index].SObservacao = Roupas_txtsObservacao.Text;
            bs_Roupas[index].DtEntregaRoupa = Roupas_txtdtEntrega.Text;
            bs_Roupas[index].SDscTamanho = Roupas_ddlidTamanho.SelectedItem.ToString();
            bs_Roupas[index].SDscTipoRoupa = Roupas_ddlidTipoRoupa.SelectedItem.ToString();
            bs_Roupas[index].SFuncao = "SALVAR_ROUPAS";

            Roupas_GV_DataBind();
            Roupas_LimpaCampos();
            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");
        }

        void Roupas_LimpaCampos()
        {
            Roupas_ddlidTipoRoupa.SelectedValue = "0";
            Roupas_ddlidTamanho.SelectedValue = "0";
            Roupas_txtdtEntrega.Text = "";
            Roupas_txtQuantidade.Text = "";
            Roupas_txtsCor.Text = "";
            Roupas_txtsObservacao.Text = "";
            hddRoupas_idLinha.Value = "";

            Roupas_BtnSalvar.Visible = false;
            Roupas_cmdIncluir.Visible = true;
        }

        private bool Roupas_ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (Roupas_ddlidTipoRoupa.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Tipo de Roupa";

            if (Roupas_ddlidTamanho.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tamanho";

            if (!Validacoes.ValidarNumerico(Roupas_txtQuantidade))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Quantidade Inválida";

            if (Validacoes.ValidarTexto(Roupas_txtsCor))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Cor Inválida";

            if (Validacoes.ValidarTexto(Roupas_txtdtEntrega))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a data de Entrega";

            if (sMensagemErro != "")
            {
                bRetorno = false;
                Roupas_Mensagem.MostraMensagem_Erro(sMensagemErro, true);
            }

            return bRetorno;
        }

        protected void Roupas_GV_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(Roupas_GV.Rows[e.RowIndex].Cells[0].Text);
            bs_Roupas[bs_Roupas.FindIndex(x => x.IdLinha.Equals(idLinha))].SFuncao = "ROUPA_EXCLUIR";
            Roupas_GV_DataBind();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");
        }

        protected void Roupas_GV_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkRoupas_Download" && sNomeArquivo == "") || (lnk.ID == "lnkRoupas_UpLoad" && sNomeArquivo != ""))
                        lnk.Visible = false;
                }

            }

            GRID.EsconderColunas(e, 0, 1);
        }

        protected void Roupas_GV_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "Roupas";
            hddRoupas_idLinha.Value = idLinha.ToString();

            if (e.CommandName == "Upload_Arquivo")
                AbrirModal_EnvioArquivo(eBloco.Roupas, idLinha, bs_Roupas[bs_Roupas.FindIndex(x => x.IdLinha.Equals(idLinha))].SObservacao);
            else if (e.CommandName == "Download_Arquivo")
                Efetuar_Download_Arquivo(eBloco.Roupas, idLinha);
            else if (e.CommandName == "Editar")
            {
                Roupas_cmdIncluir.Visible = false;
                Roupas_BtnSalvar.Visible = true;

                var Roupas = bs_Roupas[bs_Roupas.FindIndex(x => x.IdLinha.Equals(idLinha))];

                Roupas_ddlidTipoRoupa.SelectedValue = Roupas.IdTipoRoupa.ToString();
                Roupas_ddlidTamanho.SelectedValue = Roupas.IdTamanho.ToString();
                Roupas_txtdtEntrega.Text = Roupas.DtEntregaRoupa;
                Roupas_txtQuantidade.Text = Roupas.NQuantidade.ToString();
                Roupas_txtsCor.Text = Roupas.SCorRoupa;
                Roupas_txtsObservacao.Text = Roupas.SObservacao;

                Roupas_txtsObservacao.Focus();
            }

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab");
        }

        void Roupas_GV_DataBind()
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Roupas_GV_DataBind", GRID.DataBindComScriptData(Roupas_GV, bs_Roupas.Where(c => c.SFuncao.ToString() != "ROUPA_EXCLUIR").ToList(), 4, "asc", "false", "''"), true);
                RegistraScript("");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Roupas: " + ex.Message);
            }
        }

        #endregion

        #endregion

        #region | Aba Ocorrências

        #region | Ausência 


        void Ausencia_Validar_Horas()
        {
            Ausencia_div_Horas.Visible = false;
            Ausencia_div_HorasCalculadas.Visible = false;

            decimal horasAtestado = 0;
            if (Ausencia_ddlsTipoPeriodo.SelectedValue == "Dia")
            {
                Ausencia_div_HorasCalculadas.Visible = true;

                if (Validacoes.ValidarData(Ausencia_txtdtInicio) && Validacoes.ValidarData(Ausencia_txtdtRetorno))
                {
                    DateTime currentDay = Convert.ToDateTime(Ausencia_txtdtInicio.Text);
                    DateTime finalDay = Convert.ToDateTime(Ausencia_txtdtRetorno.Text);

                    while (currentDay <= finalDay)
                    {
                        if (currentDay.DayOfWeek >= DayOfWeek.Monday && currentDay.DayOfWeek <= DayOfWeek.Thursday)
                            horasAtestado += 9;
                        else if (currentDay.DayOfWeek == DayOfWeek.Friday)
                            horasAtestado += 8;

                        currentDay = currentDay.AddDays(1);
                    }
                    Ausencia_txtnQuantidadeHoras.Text = horasAtestado.ToString();
                }
            }
            else
            {
                Ausencia_div_Horas.Visible = true;

                if (Validacoes.ValidarData(Ausencia_txtdtInicio) && Validacoes.ValidarData(Ausencia_txtdtRetorno))
                {
                    TimeSpan diff = Convert.ToDateTime(Ausencia_txtdtRetorno.Text) - Convert.ToDateTime(Ausencia_txtdtInicio.Text);
                    horasAtestado = (decimal)diff.TotalHours;
                    //Thread.Sleep(1000);
                    Ausencia_txtnHoras.Text = Convert.ToDecimal(horasAtestado).ToString();
                }
            }


            

        }
        protected void Ausencia_txtdt_TextChanged(object sender, EventArgs e)
        {
            Ausencia_Validar_Horas();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");

            TextBox txt = (TextBox)sender;
            
            //if (txt.ID.ToString() =="Ausencia_txtdtInicio")
            //{
            //    Ausencia_txtdtInicio.Focus();
            //}
            //else if (txt.ID.ToString() == "Ausencia_txtdtRetorno")
            //{
            //    Ausencia_txtdtRetorno.Focus();
            //}
        }

        protected void Ausencia_ddlsPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {

            Ausencia_Validar_Horas();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
            Ausencia_txtdtInicio.Focus();
        }



        void dtg_Ausencia_Popular(string idColaborador)
        {
            bs_Ausencia.Clear();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE_ATESTADO" },
                { "@idColaborador", hddidColaborador.Value },
                { "@dtInicioAtestado_Pesquisa", txtdtInicioAtestado_Pesquisa.Text },
                { "@dtFinalAtestado_Pesquisa", txtdtFinalAtestado_Pesquisa.Text }
            };

            DataSet Ausencia_dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(Ausencia_dsPesquisa, out string sErro))
            {
                foreach (DataRow row in Ausencia_dsPesquisa.Tables[0].Rows)
                {
                    cls_Ausencia objItem = new cls_Ausencia();

                    objItem.idLinha = bs_Ausencia.Count() + 1;
                    objItem.sFuncao = "SEM ALTERAÇÃO";
                    objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                    objItem.idRegistroAtestado = Convert.ToInt32(row["idRegistroAtestado"].ToString());
                    objItem.idAtestado = Convert.ToInt32(row["idAtestado"].ToString());
                    objItem.sDscAtestado = row["sDscAtestado"].ToString();
                    objItem.nHorasAtestado = Convert.ToDouble(row["nHorasAtestado"].ToString());
                   // objItem.nQuantidadeHoras = BD.Conversoes.Numerico_Decimal(row["nQuantidadeHoras"].ToString());
                    objItem.dtInicioAtestado = Convert.ToDateTime(row["dtInicioAtestado"].ToString());
                    objItem.dtRetornoAtestado = Convert.ToDateTime(row["dtRetornoAtestado"].ToString());
                    objItem.sObservacaoAtestado = row["sObservacaoAtestado"].ToString();
                    objItem.sDescontoVT = row["sDescontoVT"].ToString();
                    objItem.sDescontoVR = row["sDescontoVR"].ToString();
                    objItem.sDescontoDSR = row["sDescontoDSR"].ToString();
                    objItem.sTipoAtestado = row["sTipoAtestado"].ToString();
                    objItem.sTipoAusencia = row["sTipoAusencia"].ToString();
                    objItem.sMotivoAtestado = row["sMotivoAtestado"].ToString();
                    objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                    objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                    objItem.sCID = row["sCID"].ToString();

                    bs_Ausencia.Add(objItem);
                }
            }

            dtg_Ausencia_DataBind();
            Ausencia_LimpaCampos();
        }

        protected void Ausencia_cmdIncluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (Ausencia_ValidarDados(ref sMensagem))
            {
                cls_Ausencia objItem = new cls_Ausencia();

                string[] vidColaborador = hddidColaborador.Value.Split(',');
                string idColaborador = vidColaborador[0].ToString();

                objItem.idLinha = bs_Ausencia.Count() + 1;
                objItem.sFuncao = "SALVAR_ATESTADO";
                objItem.idColaborador = Convert.ToInt32(idColaborador);
                objItem.sDscAtestado = Ausencia_txtsDscAtestado.Text;

                DateTime dtInicioAtestado = Convert.ToDateTime(Ausencia_txtdtInicio.Text);
                DateTime dtFinalAtestado = Convert.ToDateTime(Ausencia_txtdtRetorno.Text);

                objItem.sTipoAusencia = Ausencia_ddlsTipoAusencia.SelectedValue.ToString();
                objItem.sMotivoAtestado = Ausencia_ddlsMotivo.SelectedValue.ToString();

                objItem.sTipoAtestado = Ausencia_ddlsTipoPeriodo.SelectedValue.ToString();
                objItem.dtInicioAtestado = dtInicioAtestado;
                objItem.dtRetornoAtestado = dtFinalAtestado;
                objItem.sDescontoVT = Ausencia_cbDescontoVT.Checked ? "Sim" : "Não";
                objItem.sDescontoVR = Ausencia_cbDescontoVR.Checked ? "Sim" : "Não";
                objItem.sDescontoDSR = Ausencia_cbDescontoDSR.Checked ? "Sim" : "Não";
                objItem.sCID = Ausencia_txtsCID.Text;

                objItem.sObservacaoAtestado = Ausencia_txtsObservacao.Text;
                objItem.sNomeArquivo = "";

                Double nHoras = 0;
                if (Ausencia_ddlsTipoPeriodo.SelectedValue == "Dia")
                    nHoras = Convert.ToDouble(Ausencia_txtnQuantidadeHoras.Text);
                else
                    nHoras = Convert.ToDouble(Ausencia_txtnHoras.Text);

                objItem.nHorasAtestado = nHoras;

                bs_Ausencia.Add(objItem);
                Ausencia_LimpaCampos();

                dtg_Ausencia_DataBind();
                Modal_Fechar("modal_Ausencia");

            }
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
            RegistraScript("");

        }




        protected void Ausencia_cmdSalvar_Click(object sender, EventArgs e)
        {
            int idLinha = Convert.ToInt32(hddAusencia_idLinha.Value);
            int index = bs_Ausencia.FindIndex(x => x.idLinha.Equals(idLinha));
            string sMensagem = "";

            if (Ausencia_ValidarDados(ref sMensagem))
            {
                DateTime dtInicio = Convert.ToDateTime(Ausencia_txtdtInicio.Text);
                DateTime dtFinal = Convert.ToDateTime(Ausencia_txtdtRetorno.Text);

                bs_Ausencia[index].sFuncao = "SALVAR_ATESTADO";
                bs_Ausencia[index].sDscAtestado = Ausencia_txtsDscAtestado.Text;
                bs_Ausencia[index].dtInicioAtestado = dtInicio;
                bs_Ausencia[index].dtRetornoAtestado = dtFinal;
                bs_Ausencia[index].sObservacaoAtestado = Ausencia_txtsObservacao.Text;
                bs_Ausencia[index].sTipoAtestado = Ausencia_ddlsTipoPeriodo.SelectedValue.ToString();
                bs_Ausencia[index].sTipoAusencia = Ausencia_ddlsTipoAusencia.SelectedItem.ToString();
                bs_Ausencia[index].sMotivoAtestado = Ausencia_ddlsMotivo.SelectedItem.ToString();
                bs_Ausencia[index].sDescontoVT = Ausencia_cbDescontoVT.Checked ? "Sim" : "Não";
                bs_Ausencia[index].sDescontoVR = Ausencia_cbDescontoVR.Checked ? "Sim" : "Não";
                bs_Ausencia[index].sDescontoDSR = Ausencia_cbDescontoDSR.Checked ? "Sim" : "Não";
                bs_Ausencia[index].sCID = Ausencia_txtsCID.Text;

                Double nHoras = 0;
                if (Ausencia_ddlsTipoPeriodo.SelectedValue == "Dia")
                    nHoras = Convert.ToDouble(Ausencia_txtnQuantidadeHoras.Text);
                else
                    nHoras = Convert.ToDouble(Ausencia_txtnHoras.Text);

                bs_Ausencia[index].nHorasAtestado = nHoras;

                dtg_Ausencia_DataBind();
                RegistraScript("");
                Modal_Fechar("modal_Ausencia");
            }
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
            RegistraScript("");
        }

        void Ausencia_LimpaCampos()
        {
            Ausencia_cmdSalvar.Visible = false;
            Ausencia_cmdIncluir.Visible = false;
            Ausencia_ddlsTipoAusencia.SelectedValue = "";
            Ausencia_ddlsMotivo.SelectedValue = "";
            Ausencia_Validar_Horas();

            Ausencia_txtsDscAtestado.Text = "";
            Ausencia_txtdtInicio.Text = "";
            Ausencia_txtdtRetorno.Text = "";
            Ausencia_txtsObservacao.Text = "";
            Ausencia_ddlsTipoPeriodo.SelectedValue = "Dia";
            Ausencia_txtnQuantidadeHoras.Text = "";
            Ausencia_txtnHoras.Text = "";
            Ausencia_txtsCID.Text = "";
            Ausencia_cbDescontoVT.Checked = false;
            Ausencia_cbDescontoVR.Checked = false;
            Ausencia_cbDescontoDSR.Checked = false;
            Ausencia_ddlsTipoAusencia.Focus();
        }

        private bool Ausencia_ValidarDados(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";




            if (Ausencia_ddlsTipoAusencia.SelectedValue == "")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo";

            if (Ausencia_ddlsMotivo.SelectedValue == "")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Motivo";


            if (Ausencia_txtsDscAtestado.Text.Length < 6)
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += "Insira uma descrição maior que seis caracteres!";
            }

            if (Ausencia_ddlsTipoPeriodo.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Periodo";

            if (Ausencia_ddlsTipoPeriodo.SelectedValue == "Hora")
            {
                if (Ausencia_txtnHoras.Text.Length < 1)
                {
                    if (sMensagemErro != "")
                        sMensagemErro = sMensagemErro + "</br>";

                    sMensagemErro += "Insira Hora!";
                }
            }

            string sMsgValidacaoDatas = Validacoes.ValidaDatas_Hora(Ausencia_txtdtInicio .Text, Ausencia_txtdtRetorno.Text);

            if (sMsgValidacaoDatas != "")
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += sMsgValidacaoDatas;
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                Ausencia_MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            sMensagem = sMensagemErro;
            return bRetorno;
        }

        protected void dtg_Ausencia_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtg_Ausencia.Rows[e.RowIndex].Cells[0].Text);
            bs_Ausencia[bs_Ausencia.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_ATESTADO";
            dtg_Ausencia_DataBind();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        protected void dtgAtestado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString(); ;

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkAtestado_Download" && sNomeArquivo == "") || (lnk.ID == "lnkAtestado_UpLoad" && sNomeArquivo != ""))
                        lnk.Visible = false;
                }

                if (DataBinder.Eval(e.Row.DataItem, "sDescontoVT").ToString() == "Sim")
                {
                    e.Row.Cells[9].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[9].CssClass = "danger";
                }


                if (DataBinder.Eval(e.Row.DataItem, "sDescontoVR").ToString() == "Sim")
                {
                    e.Row.Cells[10].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[10].CssClass = "danger";
                }

                if (DataBinder.Eval(e.Row.DataItem, "sDescontoDSR").ToString() == "Sim")
                {
                    e.Row.Cells[11].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[11].CssClass = "danger";
                }


            }

            GRID.EsconderColunas(e, 0);
        }

        protected void dtg_Ausencia_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "Atestado";
            hddAusencia_idLinha.Value = idLinha.ToString();



            if (e.CommandName == "Upload_Arquivo")
                AbrirModal_EnvioArquivo(eBloco.Atestado, idLinha, bs_Ausencia[bs_Ausencia.FindIndex(x => x.idLinha.Equals(idLinha))].sDscAtestado);
            else if (e.CommandName == "Download_Arquivo")
                Efetuar_Download_Arquivo(eBloco.Atestado, idLinha);
            else if (e.CommandName == "Editar")
            {
                Ausencia_LimpaCampos();
                Ausencia_cmdIncluir.Visible = false;
                Ausencia_cmdSalvar.Visible = true;

                var Ausencia = bs_Ausencia[bs_Ausencia.FindIndex(x => x.idLinha.Equals(idLinha))];

                Ausencia_ddlsTipoAusencia.SelectedValue = Ausencia.sTipoAusencia;
                Ausencia_ddlsMotivo.SelectedValue = Ausencia.sMotivoAtestado;
                Ausencia_txtsDscAtestado.Text = Ausencia.sDscAtestado;
                Ausencia_ddlsTipoPeriodo.SelectedValue = Ausencia.sTipoAtestado.ToString();

                if (Ausencia.sTipoAtestado == "Dia")
                {
                    Ausencia_div_HorasCalculadas.Visible = true;
                    Ausencia_div_Horas.Visible = false;
                    Ausencia_txtnQuantidadeHoras.Text = Ausencia.nHorasAtestado.ToString();
                }
                else
                {
                    Ausencia_div_HorasCalculadas.Visible = false;
                    Ausencia_div_Horas.Visible = true;
                    Ausencia_txtnHoras.Text = Ausencia.nHorasAtestado.ToString();
                }

                Ausencia_txtdtInicio.Text = Ausencia.dtInicioAtestado.ToString("yyyy-MM-ddTHH:mm");
                Ausencia_txtdtRetorno.Text = Ausencia.dtRetornoAtestado.ToString("yyyy-MM-ddTHH:mm");
                Ausencia_cbDescontoVT.Checked = Ausencia.sDescontoVT.ToString().Substring(0, 1) == "S";
                Ausencia_cbDescontoVR.Checked = Ausencia.sDescontoVR.ToString().Substring(0, 1) == "S";
                Ausencia_cbDescontoDSR.Checked = Ausencia.sDescontoDSR.ToString().Substring(0, 1) == "S";
                Ausencia_txtsObservacao.Text = Ausencia.sObservacaoAtestado;
                Ausencia_txtsCID.Text = Ausencia.sCID;
                Ausencia_ddlsTipoAusencia.Focus();

                Modal_Abrir("modal_Ausencia");
            }

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        void dtg_Ausencia_DataBind()
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_dtgAusencia_DataBind", GRID.DataBindComScriptData(dtg_Ausencia, bs_Ausencia.Where(c => c.sFuncao.ToString() != "EXCLUIR_ATESTADO").ToList(), new int[2] { 3, 4 }, "asc", "false", "''"), true);

                GRID.SomarColunas(dtg_Ausencia, true, GRID.Formatação.Numero, 6);
                RegistraScript("");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Atestado: " + ex.Message);
            }
        }

        protected void cmdPesquisarAtestado_Click(object sender, EventArgs e) { dtg_Ausencia_Popular(hddidColaborador.Value); FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab"); }


















        bool Salvar_Atestado(string idColaborador)
        {
            bool bRetorno = false;

            try
            {
                foreach (var Atestado_Linha in bs_Ausencia)
                {
                    int idArquivo = Atestado_Linha.idArquivo;
                    if (Atestado_Linha.sNomeArquivo != "" && Atestado_Linha.idArquivo == 0)
                    {
                        cls_Arquivos Arquivo = new cls_Arquivos();
                        Arquivo.idTipoArquivo = 9999;
                        Arquivo.idObjeto = Atestado_Linha.idRegistroAtestado;
                        Arquivo.sNomeArquivo = Atestado_Linha.sNomeArquivo;
                        Arquivo.sDscArquivo = Atestado_Linha.sObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        Arquivo.vbArquivo = Atestado_Linha.objArquivo;
                        Arquivo.dtExpiracaoDoc = "";

                        Atestado_Linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (Atestado_Linha.sFuncao == "SEM ALTERAÇÃO")
                            Atestado_Linha.sFuncao = "SALVAR_ATESTADO";
                    }

                    if (Atestado_Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<string, string> vParametroAtestado_Incluir = new Dictionary<string, string>
                        {
                            ["@sFuncao"] = Atestado_Linha.sFuncao,
                            ["@idRegistro"] = Atestado_Linha.idRegistroAtestado.ToString(),
                            ["@idColaborador"] = idColaborador,
                            ["@idAtestado"] = Atestado_Linha.idAtestado.ToString(),
                            ["@sDscAtestado"] = Atestado_Linha.sDscAtestado
                        };


                        vParametroAtestado_Incluir["@dtInicioAtestado"] = Atestado_Linha.dtInicioAtestado.ToString();
                        vParametroAtestado_Incluir["@dtRetornoAtestado"] = Atestado_Linha.dtRetornoAtestado.ToString();
                        vParametroAtestado_Incluir["@sObservacaoAtestado"] = Atestado_Linha.sObservacaoAtestado;
                        vParametroAtestado_Incluir["@nHorasAtestado"] = Atestado_Linha.nHorasAtestado.ToString().Replace(",", ".");
                        vParametroAtestado_Incluir["@sTipoAtestado"] = Atestado_Linha.sTipoAtestado.ToString();
                        vParametroAtestado_Incluir["@sTipoAusencia"] = Atestado_Linha.sTipoAusencia ?? "t";  
                        vParametroAtestado_Incluir["@sMotivoAtestado"] = Atestado_Linha.sMotivoAtestado?? "t"; 
                        vParametroAtestado_Incluir["@sDescontoVT"] = Atestado_Linha.sDescontoVT.ToString();
                        vParametroAtestado_Incluir["@sDescontoVR"] = Atestado_Linha.sDescontoVR.ToString();
                        vParametroAtestado_Incluir["@sDescontoDSR"] = Atestado_Linha.sDescontoDSR?? "t"; 
                        vParametroAtestado_Incluir["@idArquivo"] = Atestado_Linha.idArquivo.ToString();

                        BD.ExecutarDataSet(sProcedure, vParametroAtestado_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Bloco Ausência - Erro: " + ex.Message);
            }

            return bRetorno;
        }

        
        #endregion

        #region | Evento 

        void Popular_dtgEvento(string idColaborador)
        {
            bs_Evento.Clear();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE_AFASTAMENTO" },
                { "@idColaborador", idColaborador },
                { "@dtInicioAfastamento_Pesquisa", Convert.ToDateTime(txtdtdtInicioAfastamento_Pesquisa.Text).ToString("dd/MM/yyyy") },
                { "@dtFinalAfastamento_Pesquisa", Convert.ToDateTime(txtdtdtFinalAfastamento_Pesquisa.Text).ToString("dd/MM/yyyy") },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_dtgEvento_DataBind", GRID.DataBindComScriptData(dtgEvento, dsPesquisa.Tables[0], new int[3] { 5, 10, 11 }, "asc", "false", "''"), true);
            }

        }

        protected void dtgEvento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idRegistroEvento = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                string url = "/App/Paginas/RRHH/OcorrenciasElogios_Detalhe.aspx?id=" + idRegistroEvento;
                string script = $"window.open('{url}', '_blank');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "openNewTab", script, true);
            }

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        protected void cmdPesquisarAfastamento_Click(object sender, EventArgs e) { Popular_dtgEvento(hddidColaborador.Value); FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab"); }

        #endregion

        #region | Veiculos / Infrações / Ocorrencias

        protected void ddlsTipoOcorrencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            div_VeiculoOcorrencia.Visible = true;
            div_sDscInfracao.Visible = true;
            div_Custo.Visible = true;
            div_dtOcorrencia.Visible = true;
            div_Responsavel.Visible = true;
            div_status.Visible = true;
            div_Potnos.Visible = true;
            div_dtInfração.Visible = true;
            div_dtIndicarCondutor.Visible = true;
            div_dtLimitePagamento.Visible = true;
            div_AIT.Visible = true;
            div_ObservacaoInfraca.Visible = true;

            switch (Infracoes_ddlidTipoOcorrencia.SelectedValue)
            {
                case "0":
                    div_VeiculoOcorrencia.Visible = false;
                    div_sDscInfracao.Visible = false;
                    div_Custo.Visible = false;
                    div_dtOcorrencia.Visible = false;
                    div_Responsavel.Visible = false;
                    div_status.Visible = false;
                    div_Potnos.Visible = false;
                    div_dtInfração.Visible = false;
                    div_dtIndicarCondutor.Visible = false;
                    div_dtLimitePagamento.Visible = false;
                    div_AIT.Visible = false;
                    div_ObservacaoInfraca.Visible = false;
                    Infracoes_MensagemPagina.MostraMensagem("Selecione um Tipo!", "info", false);
                    break;

                // Evento
                case "1":
                    div_status.Visible = false;
                    div_Potnos.Visible = false;
                    div_dtInfração.Visible = false;
                    div_dtIndicarCondutor.Visible = false;
                    div_dtLimitePagamento.Visible = false;
                    break;

                // Infração
                case "2":
                    div_dtIndicarCondutor.Visible = false;
                    break;

                // Acidente      
                case "3":
                    div_status.Visible = false;
                    div_Potnos.Visible = false;
                    div_dtInfração.Visible = false;
                    div_dtIndicarCondutor.Visible = false;
                    div_AIT.Visible = false;
                    break;

                // Locadora
                case "4":
                    div_dtIndicarCondutor.Visible = false;
                    div_AIT.Visible = false;
                    break;

                // Indicar Condutor
                case "5":
                    div_Custo.Visible = false;
                    div_dtInfração.Visible = false;
                    div_dtLimitePagamento.Visible = false;
                    break;
            }

            Modal_Abrir("modal_Infracoes");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        void Popular_dtgOcorrencias(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[nTabela_Ocorrencias].Rows)
            {
                cls_Ocorrencias objItem = new cls_Ocorrencias();

                objItem.idLinha = bs_Ocorrencias.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";
                objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                objItem.idRegistroOcorrencia = Convert.ToInt32(row["idRegistroOcorrencia"].ToString());
                objItem.idOcorrencia = Convert.ToInt32(row["idOcorrencia"].ToString());
                objItem.sDscInfracao = row["sDscInfracao"].ToString();
                objItem.dtOcorrencia = row["dtOcorrencia"].ToString();
                objItem.dtLimiteIndicarCondutor = row["dtLimiteIndicarCondutor"].ToString();
                objItem.dtLimitePagamento = row["dtLimitePagamento"].ToString();
                objItem.nValor = BD.Conversoes.Numerico_Decimal(row["nValor"].ToString());
                objItem.sPontos = row["sPontos"].ToString();
                objItem.sAIT = row["sAIT"].ToString();
                objItem.sObservacaoInfracao = row["sObservacaoInfracao"].ToString();
                objItem.dtInfracao = row["dtInfracao"].ToString();
                objItem.sVeiculoOcorrencia = row["sVeiculoOcorrencia"].ToString();
                objItem.sStatus = row["sStatus"].ToString();
                objItem.sResponsavel = row["sResponsavel"].ToString();
                objItem.sTipoOcorrencia = row["sTipoOcorrencia"].ToString();
                objItem.idVeiculoOcorrencia = Convert.ToInt32(row["idVeiculoOcorrencia"].ToString());
                objItem.idStatusOcorrencia = Convert.ToInt32(row["idStatusOcorrencia"].ToString());
                objItem.idResponsavelVeiculo = Convert.ToInt32(row["idResponsavelVeiculo"].ToString());
                objItem.idTipoOcorrencia = Convert.ToInt32(row["idTipoOcorrencia"].ToString());
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());

                bs_Ocorrencias.Add(objItem);
            }

            dtgOcorrencias_DataBind();
            Ocorrencias_LimpaCampos();
        }

        protected void cmdIncluirOcorrencias_Click(object sender, EventArgs e)
        {
            Ocorrencias_LimpaCampos();

            hddOcorrencias_idLinha.Value = "0";
            Infracoes_lblTitulo.Text = "Nova Infração";
            div_VeiculoOcorrencia.Visible = false;
            div_sDscInfracao.Visible = false;
            div_Custo.Visible = false;
            div_dtOcorrencia.Visible = false;
            div_Responsavel.Visible = false;
            div_status.Visible = false;
            div_Potnos.Visible = false;
            div_dtInfração.Visible = false;
            div_dtIndicarCondutor.Visible = false;
            div_dtLimitePagamento.Visible = false;
            div_AIT.Visible = false;
            div_ObservacaoInfraca.Visible = false;

            Modal_Abrir("modal_Infracoes");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }
        protected void cmdIncluirAusencia_Click(object sender, EventArgs e)
        {
            Ausencia_LimpaCampos();
            Ausencia_cmdIncluir.Visible = true;
            Modal_Abrir("modal_Ausencia");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        bool Salvar_Ocorrencias(string idColaborador)
        {
            bool bRetorno = false;

            try
            {
                foreach (var Ocorrencias_Linha in bs_Ocorrencias)
                {
                    int idArquivo = Ocorrencias_Linha.idArquivo;
                    if (Ocorrencias_Linha.sNomeArquivo != "" && Ocorrencias_Linha.idArquivo == 0)
                    {
                        cls_Arquivos Arquivo = new cls_Arquivos();
                        Arquivo.idTipoArquivo = 9999;
                        Arquivo.idObjeto = Ocorrencias_Linha.idRegistroOcorrencia;
                        Arquivo.sNomeArquivo = Ocorrencias_Linha.sNomeArquivo;
                        Arquivo.sDscArquivo = Ocorrencias_Linha.sObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        Arquivo.vbArquivo = Ocorrencias_Linha.objArquivo;
                        Arquivo.dtExpiracaoDoc = "";

                        Ocorrencias_Linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (Ocorrencias_Linha.sFuncao == "SEM ALTERAÇÃO")
                            Ocorrencias_Linha.sFuncao = "SALVAR_OCORRENCIAS";
                    }

                    if (Ocorrencias_Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<string, string> vParametroItensOcorrencias_Incluir = new Dictionary<string, string>
                        {
                            ["@sFuncao"] = Ocorrencias_Linha.sFuncao,
                            ["@idRegistro"] = Ocorrencias_Linha.idRegistroOcorrencia.ToString(),
                            ["@idColaborador"] = idColaborador,
                            ["@sDscInfracao"] = Ocorrencias_Linha.sDscInfracao,
                            ["@dtOcorrencia"] = Ocorrencias_Linha.dtOcorrencia,
                            ["@dtLimiteIndicarCondutor"] = Ocorrencias_Linha.dtLimiteIndicarCondutor,
                            ["@dtLimitePagamento"] = Ocorrencias_Linha.dtLimitePagamento,
                            ["nValor"] = Ocorrencias_Linha.nValor.ToString().Replace(",", "."),
                            ["@sPontos"] = Ocorrencias_Linha.sPontos,
                            ["@sAIT"] = Ocorrencias_Linha.sAIT,
                            ["@dtInfracao"] = Ocorrencias_Linha.dtInfracao,
                            ["@sObservacaoInfracao"] = Ocorrencias_Linha.sObservacaoInfracao,
                            ["@sVeiculoOcorrencia"] = Ocorrencias_Linha.sVeiculoOcorrencia.ToString(),
                            ["@sStatus"] = Ocorrencias_Linha.sStatus.ToString(),
                            ["@sResponsavel"] = Ocorrencias_Linha.sResponsavel.ToString(),
                            ["@sTipoOcorrencia"] = Ocorrencias_Linha.sTipoOcorrencia.ToString(),
                            ["@idVeiculoOcorrencia"] = Ocorrencias_Linha.idVeiculoOcorrencia.ToString(),
                            ["@idStatusOcorrencia"] = Ocorrencias_Linha.idStatusOcorrencia.ToString(),
                            ["@idResponsavelVeiculo"] = Ocorrencias_Linha.idResponsavelVeiculo.ToString(),
                            ["@idTipoOcorrencia"] = Ocorrencias_Linha.idTipoOcorrencia.ToString(),
                            ["@idArquivo"] = Ocorrencias_Linha.idArquivo.ToString()
                        };
                        BD.ExecutarDataSet(sProcedure, vParametroItensOcorrencias_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Bloco Ocorrencias - Erro: " + ex.Message);
            }

            return bRetorno;
        }

        protected void cmdInfracao_Salvar_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            int idLinha = Convert.ToInt32(hddOcorrencias_idLinha.Value);

            if (ValidarDados_Ocorrencias(ref sMensagem))
            {
                int index = 0;
                cls_Ocorrencias objItem = new cls_Ocorrencias();

                if (idLinha != 0)
                {
                    index = bs_Ocorrencias.FindIndex(x => x.idLinha.Equals(idLinha));
                    objItem = bs_Ocorrencias[index];
                }

                if (idLinha == 0)
                {
                    objItem.sNomeArquivo = "";
                    bs_Ocorrencias.Add(objItem);
                    index = bs_Ocorrencias.Count() - 1;
                    objItem.idLinha = bs_Ocorrencias.Max(item => item.idLinha) + 1;
                }

                DateTime.TryParse(txtdtOcorrencia.Text, out DateTime dtOcorrencia);
                DateTime.TryParse(txtdtInfracao.Text, out DateTime dtInfracao);
                DateTime.TryParse(txtdtLimiteIndicarCondutor.Text, out DateTime dtLimiteIndicarCondutor);
                DateTime.TryParse(txtdtLimitePagamento.Text, out DateTime dtLimitePagamento);

                objItem.sFuncao = "SALVAR_OCORRENCIAS";
                objItem.idColaborador = Convert.ToInt32(hddidColaborador.Value.Split(',')[0].ToString());
                objItem.sDscInfracao = txtsDscInfracao.Text;
                objItem.dtOcorrencia = string.IsNullOrEmpty(txtdtOcorrencia.Text) ? string.Empty : dtOcorrencia.ToString("dd/MM/yyyy");
                objItem.dtInfracao = string.IsNullOrEmpty(txtdtInfracao.Text) ? string.Empty : dtInfracao.ToString("dd/MM/yyyy");
                objItem.dtLimiteIndicarCondutor = string.IsNullOrEmpty(txtdtLimiteIndicarCondutor.Text) ? string.Empty : dtLimiteIndicarCondutor.ToString("dd/MM/yyyy");
                objItem.dtLimitePagamento = string.IsNullOrEmpty(txtdtLimitePagamento.Text) ? string.Empty : dtLimitePagamento.ToString("dd/MM/yyyy");
                objItem.nValor = BD.Conversoes.Numerico_Decimal(txtnValor.Text);
                objItem.sPontos = txtsPontos.Text;
                objItem.sAIT = txtsAIT.Text;
                objItem.sObservacaoInfracao = txtsObservacaoInfracao.Text;
                objItem.sVeiculoOcorrencia = ddlidVeiculoOcorrencia.SelectedItem.ToString();
                objItem.sStatus = ddlidStatusOcorrencia.SelectedValue == "0" ? "" : ddlidStatusOcorrencia.SelectedItem.ToString();
                objItem.sResponsavel = ddlidResponsavelVeiculo.SelectedItem.ToString();
                objItem.sTipoOcorrencia = Infracoes_ddlidTipoOcorrencia.SelectedItem.ToString();
                objItem.idVeiculoOcorrencia = Convert.ToInt32(ddlidVeiculoOcorrencia.SelectedValue.ToString());
                objItem.idStatusOcorrencia = Convert.ToInt32(ddlidStatusOcorrencia.SelectedValue.ToString());
                objItem.idResponsavelVeiculo = Convert.ToInt32(ddlidResponsavelVeiculo.SelectedValue.ToString());
                objItem.idTipoOcorrencia = Convert.ToInt32(Infracoes_ddlidTipoOcorrencia.SelectedValue.ToString());

                bs_Ocorrencias[index] = objItem;

                dtgOcorrencias_DataBind();
                Ocorrencias_LimpaCampos();
                Modal_Fechar("modal_Infracoes");
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        void Ocorrencias_LimpaCampos()
        {
            txtsDscInfracao.Text = "";
            ddlidVeiculoOcorrencia.SelectedValue = "0";
            txtdtOcorrencia.Text = "";
            txtdtLimiteIndicarCondutor.Text = "";
            txtdtLimitePagamento.Text = "";
            txtdtInfracao.Text = "";
            txtnValor.Text = "";
            txtsPontos.Text = "";
            ddlidStatusOcorrencia.SelectedValue = "0";
            txtsAIT.Text = "";
            ddlidResponsavelVeiculo.SelectedValue = "0";
            Infracoes_ddlidTipoOcorrencia.SelectedValue = "0";
            txtsObservacaoInfracao.Text = "";
        }

        private bool ValidarDados_Ocorrencias(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (Infracoes_ddlidTipoOcorrencia.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Tipo de Infração";
            else if (Infracoes_ddlidTipoOcorrencia.SelectedValue == "1") // Evento
            {
                if (txtnValor.Text == "")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um valor de custo válido";

                if (txtsAIT.Text == "")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um AIT válido";
            }
            else if (Infracoes_ddlidTipoOcorrencia.SelectedValue == "2") // Infração
            {
                if (txtnValor.Text == "")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um valor de custo válido";

                if (ddlidStatusOcorrencia.SelectedValue == "0")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Status";

                if (txtsPontos.Text == "")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira a quantidade de Pontos";

                if (!DateTime.TryParse(txtdtInfracao.Text, out DateTime dtInfracao))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Data de Infração válida";

                if (!DateTime.TryParse(txtdtLimitePagamento.Text, out DateTime dtLimitePagamento))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Data Limite de Pagamento válida";

                if (txtsAIT.Text == "")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um AIT válido";
            }
            else if (Infracoes_ddlidTipoOcorrencia.SelectedValue == "3") // Acidente
            {
                if (txtnValor.Text == "")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um valor de custo válido";

                if (!DateTime.TryParse(txtdtLimitePagamento.Text, out DateTime dtLimitePagamento))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Data Limite de Pagamento válida";
            }
            else if (Infracoes_ddlidTipoOcorrencia.SelectedValue == "4") // Locadora
            {
                if (ddlidStatusOcorrencia.SelectedValue == "0")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Status";

                if (txtsPontos.Text == "")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira a quantidade de Pontos";

                if (!DateTime.TryParse(txtdtInfracao.Text, out DateTime dtInfracao))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Data de Infração válida";

                if (!DateTime.TryParse(txtdtLimitePagamento.Text, out DateTime dtLimitePagamento))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Data Limite de Pagamento válida";
            }
            else if (Infracoes_ddlidTipoOcorrencia.SelectedValue == "5") // Indica Condutor
            {
                if (ddlidStatusOcorrencia.SelectedValue == "0")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Status";

                if (txtsPontos.Text == "")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira a quantidade de Pontos";

                if (!DateTime.TryParse(txtdtLimiteIndicarCondutor.Text, out DateTime dtLimiteIndicarCondutor))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Data de Infração válida";

                if (txtsAIT.Text == "")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um AIT válido";
            }

            if (!DateTime.TryParse(txtdtOcorrencia.Text, out DateTime dtOcorrencia))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Data de Ocorrência válida";

            if (ddlidResponsavelVeiculo.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Responsável";

            if (txtsDscInfracao.Text.Length < 6)
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += "Insira uma descrição de no mínimo 6 caracteres!";
            }

            if (ddlidVeiculoOcorrencia.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Veículo";

            if (sMensagemErro != "")
            {
                bRetorno = false;
                Infracoes_MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                Modal_Abrir("modal_Infracoes");
            }

            return bRetorno;
        }

        protected void dtgOcorrencias_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtgOcorrencias.Rows[e.RowIndex].Cells[0].Text);
            bs_Ocorrencias[bs_Ocorrencias.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_OCORRENCIAS";
            dtgOcorrencias_DataBind();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        protected void dtgOcorrencias_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkOcorrencia_Download" && sNomeArquivo == "") || (lnk.ID == "lnkOcorrencia_UpLoad" && sNomeArquivo != ""))
                        lnk.Visible = false;
                }
            }

            GRID.EsconderColunas(e, 0);
        }

        protected void dtgOcorrencias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "Ocorrencias";
            hddOcorrencias_idLinha.Value = idLinha.ToString();

            if (e.CommandName == "Upload_Arquivo")
                AbrirModal_EnvioArquivo(eBloco.Ocorrencias, idLinha, bs_Ocorrencias[bs_Ocorrencias.FindIndex(x => x.idLinha.Equals(idLinha))].sDscInfracao);
            else if (e.CommandName == "Download_Arquivo")
                Efetuar_Download_Arquivo(eBloco.Ocorrencias, idLinha);
            else if (e.CommandName == "Editar")
            {
                var Ocorrencias = bs_Ocorrencias[bs_Ocorrencias.FindIndex(x => x.idLinha.Equals(idLinha))];

                DateTime.TryParse(txtdtOcorrencia.Text, out DateTime dtOcorrencia);
                DateTime.TryParse(txtdtLimiteIndicarCondutor.Text, out DateTime dtLimiteIndicarCondutor);
                DateTime.TryParse(txtdtLimitePagamento.Text, out DateTime dtLimitePagamento);
                DateTime.TryParse(txtdtInfracao.Text, out DateTime dtInfracao);

                txtsDscInfracao.Text = Ocorrencias.sDscInfracao;
                txtdtOcorrencia.Text = string.IsNullOrEmpty(Ocorrencias.dtOcorrencia) ? string.Empty : dtOcorrencia.ToString("yyyy-MM-dd");
                txtdtLimiteIndicarCondutor.Text = string.IsNullOrEmpty(Ocorrencias.dtLimiteIndicarCondutor) ? string.Empty : dtLimiteIndicarCondutor.ToString("yyyy-MM-dd");
                txtdtLimitePagamento.Text = string.IsNullOrEmpty(Ocorrencias.dtLimitePagamento) ? string.Empty : dtLimitePagamento.ToString("yyyy-MM-dd");
                txtdtInfracao.Text = string.IsNullOrEmpty(Ocorrencias.dtInfracao) ? string.Empty : dtInfracao.ToString("yyyy-MM-dd");
                txtnValor.Text = Ocorrencias.nValor.ToString();
                txtsPontos.Text = Ocorrencias.sPontos;
                txtsAIT.Text = Ocorrencias.sAIT;
                txtsObservacaoInfracao.Text = Ocorrencias.sObservacaoInfracao;
                ddlidVeiculoOcorrencia.SelectedValue = Ocorrencias.idVeiculoOcorrencia.ToString();
                ddlidStatusOcorrencia.SelectedValue = Ocorrencias.idStatusOcorrencia.ToString();
                ddlidResponsavelVeiculo.SelectedValue = Ocorrencias.idResponsavelVeiculo.ToString();
                Infracoes_ddlidTipoOcorrencia.SelectedValue = Ocorrencias.idTipoOcorrencia.ToString();
                Infracoes_lblTitulo.Text = "Editar Infração";

                Modal_Abrir("modal_Infracoes");
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        void dtgOcorrencias_DataBind()
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_dtgOcorrencias_DataBind", GRID.DataBindComScriptData(dtgOcorrencias, bs_Ocorrencias.Where(c => c.sFuncao.ToString() != "EXCLUIR_OCORRENCIAS").ToList(), new int[4] { 1, 4, 5, 6 }, "asc", "false", "''"), true);

                GRID.SomarColunas(dtgOcorrencias, true, GRID.Formatação.Moeda, 8);
                RegistraScript("");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Ocorrências: " + ex.Message);
            }
        }

        #endregion

        #region | Creditos

        void Popular_dtgCredito(string idColaborador)
        {
            bs_Credito.Clear();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_CREDITO" },
                { "@idColaborador", hddidColaborador.Value }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedureColaboradoresArea, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
            {
                foreach (DataRow row in dsPesquisa.Tables[0].Rows)
                {
                    cls_Credito objItem = new cls_Credito();

                    objItem.idLinha = bs_Credito.Count() + 1;
                    objItem.sFuncao = "SEM ALTERAÇÃO";
                    objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                    objItem.idRegistro = Convert.ToInt32(row["idRegistro"].ToString());
                    objItem.sDscJustificativa = row["sDscJustificativa"].ToString();
                    objItem.nHorasCredito = Convert.ToDouble(row["nHorasCredito"].ToString());
                    objItem.dtInicioCredito = Convert.ToDateTime(row["dtInicioCredito"].ToString());
                    objItem.dtRetornoCredito = Convert.ToDateTime(row["dtRetornoCredito"].ToString());
                    objItem.sObservacaoCredito = row["sObservacaoCredito"].ToString();
                    objItem.sCreditaVR = row["sCreditaVR"].ToString();
                    objItem.sCreditaVT = row["sCreditaVT"].ToString();
                    objItem.nQuantidadeHoras = BD.Conversoes.Numerico_Decimal(row["nQuantidadeHoras"].ToString());
                    objItem.sTipoCredito = row["sTipoCredito"].ToString();
                    objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                    objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());

                    bs_Credito.Add(objItem);
                }
            }

            dtgCredito_DataBind();
            Credito_LimpaCampos();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        protected void cmdIncluirCredito_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (ValidarDados_Credito(ref sMensagem))
            {
                cls_Credito objItem = new cls_Credito();

                string[] vidColaborador = hddidColaborador.Value.Split(',');
                string idColaborador = vidColaborador[0].ToString();

                objItem.idLinha = bs_Credito.Count() + 1;
                objItem.sFuncao = "SALVAR_CREDITO";
                objItem.idColaborador = Convert.ToInt32(idColaborador);
                objItem.sDscJustificativa = sDscJustificativa.Text;
                DateTime dtInicioCredito = Convert.ToDateTime(txtCredito_dtInicioCredito.Text);
                DateTime dtRetornoCredito = Convert.ToDateTime(txtCredito_dtFinalCredito.Text);
                objItem.nHorasCredito = (dtRetornoCredito - dtInicioCredito).TotalHours;
                objItem.dtInicioCredito = dtInicioCredito;
                objItem.dtRetornoCredito = dtRetornoCredito;
                objItem.sObservacaoCredito = txtCredito_sObservacaoCredito.Text;
                objItem.sCreditaVT = ddlCreditaVT.SelectedValue;
                objItem.sCreditaVR = ddlCreditaVR.SelectedValue;
               // objItem.nQuantidadeHoras = BD.Conversoes.Numerico_Decimal(txtnQuantidadeHoras.Text);
                objItem.sTipoCredito = ddlsTipoCredito.SelectedValue.ToString();
                objItem.sNomeArquivo = "";

                bs_Credito.Add(objItem);

                dtgCredito_DataBind();
                Credito_LimpaCampos();
            }

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        bool Salvar_Credito(string idColaborador)
        {
            bool bRetorno = false;

            try
            {
                foreach (var Credito_Linha in bs_Credito)
                {
                    int idArquivo = Credito_Linha.idArquivo;
                    if (Credito_Linha.sNomeArquivo != "" && Credito_Linha.idArquivo == 0)
                    {
                        cls_Arquivos Arquivo = new cls_Arquivos();
                        Arquivo.idTipoArquivo = 7001;
                        Arquivo.idObjeto = Credito_Linha.idRegistro;
                        Arquivo.sNomeArquivo = Credito_Linha.sNomeArquivo;
                        Arquivo.sDscArquivo = Credito_Linha.sObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        Arquivo.vbArquivo = Credito_Linha.objArquivo;
                        Arquivo.dtExpiracaoDoc = "";

                        Credito_Linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (Credito_Linha.sFuncao == "SEM ALTERAÇÃO")
                            Credito_Linha.sFuncao = "SALVAR_CREDITO";
                    }

                    if (Credito_Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<string, string> vParametroCredito_Incluir = new Dictionary<string, string>
                        {
                            ["@sFuncao"] = Credito_Linha.sFuncao,
                            ["@idRegistro"] = Credito_Linha.idRegistro.ToString(),
                            ["@idColaborador"] = idColaborador,
                            ["@sDscJustificativa"] = Credito_Linha.sDscJustificativa,
                            ["@nHorasCredito"] = Credito_Linha.nHorasCredito.ToString().Replace(',', '.'),
                            ["@dtInicioCredito"] = Credito_Linha.dtInicioCredito.ToString(),
                            ["@dtRetornoCredito"] = Credito_Linha.dtRetornoCredito.ToString(),
                            ["@sObservacaoCredito"] = Credito_Linha.sObservacaoCredito,
                            ["@sCreditaVR"] = Credito_Linha.sCreditaVR,
                            ["@sCreditaVT"] = Credito_Linha.sCreditaVT,
                            ["@nQuantidadeHoras"] = Credito_Linha.nQuantidadeHoras.ToString().Replace(",", "."),
                            ["@sTipoCredito"] = Credito_Linha.sTipoCredito.ToString(),
                            ["@idArquivo"] = Credito_Linha.idArquivo.ToString()
                        };
                        BD.ExecutarDataSet(sProcedureColaboradoresArea, vParametroCredito_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Bloco Creditos - Erro: " + ex.Message);
            }

            return bRetorno;
        }

        protected void BtnSalvarCredito_Click(object sender, EventArgs e)
        {
            int idLinha = Convert.ToInt32(hddCredito_idLinha.Value);
            int index = bs_Credito.FindIndex(x => x.idLinha.Equals(idLinha));
            string sMensagem = "";

            if (ValidarDados_Credito(ref sMensagem))
            {
                DateTime dtInicioCredito = Convert.ToDateTime(txtCredito_dtInicioCredito.Text);
                DateTime dtRetornoCredito = Convert.ToDateTime(txtCredito_dtFinalCredito.Text);
                decimal nValor = 0;

                bs_Credito[index].sDscJustificativa = sDscJustificativa.Text;
                bs_Credito[index].nHorasCredito = (dtRetornoCredito - dtInicioCredito).TotalHours;
                bs_Credito[index].dtInicioCredito = dtInicioCredito;
                bs_Credito[index].dtRetornoCredito = dtRetornoCredito;
                bs_Credito[index].sObservacaoCredito = txtCredito_sObservacaoCredito.Text;
                bs_Credito[index].sCreditaVR = ddlCreditaVR.SelectedValue;
                bs_Credito[index].sCreditaVT = ddlCreditaVT.SelectedValue;

                //bs_Credito[index].nQuantidadeHoras = nValor;
                bs_Credito[index].sTipoCredito = ddlsTipoCredito.SelectedItem.ToString();
                bs_Credito[index].sFuncao = "SALVAR_CREDITO";

                dtgCredito_DataBind();
                Credito_LimpaCampos();
                RegistraScript("");
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        void Credito_LimpaCampos()
        {
            btnCredito_Salvar.Visible = false;
            btnCredito_Incluir.Visible = true;

            sDscJustificativa.Text = "";
            txtCredito_dtInicioCredito.Text = "";
            txtCredito_dtFinalCredito.Text = "";
            txtCredito_sObservacaoCredito.Text = "";
            ddlCreditaVR.SelectedValue = "0";
            ddlCreditaVT.SelectedValue = "0";
        }

        private bool ValidarDados_Credito(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (sDscJustificativa.Text.Length < 6)
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += "Insira uma descrição!";
            }

            if (string.IsNullOrEmpty(ddlCreditaVR.SelectedValue))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O Campo Creditar VR é obrigatório!";

            if (string.IsNullOrEmpty(ddlCreditaVT.SelectedValue))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O Campo Creditar VT é obrigatório!";

            string sMsgValidacaoDatas = Validacoes.ValidaDatas_Hora(txtCredito_dtInicioCredito.Text, txtCredito_dtFinalCredito.Text);

            if (sMsgValidacaoDatas != "")
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += sMsgValidacaoDatas;
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaCredito.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void dtgCredito_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtgCredito.Rows[e.RowIndex].Cells[0].Text);
            bs_Credito[bs_Credito.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_CREDITO";
            dtgCredito_DataBind();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        protected void dtgCredito_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkCredito_Download" && sNomeArquivo == "") || (lnk.ID == "lnkCredito_UpLoad" && sNomeArquivo != ""))
                        lnk.Visible = false;
                }

            }

            GRID.EsconderColunas(e, 0);
        }

        protected void dtgCredito_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "Credito";
            hddCredito_idLinha.Value = idLinha.ToString();

            if (e.CommandName == "Upload_Arquivo")
                AbrirModal_EnvioArquivo(eBloco.Credito, idLinha, bs_Credito[bs_Credito.FindIndex(x => x.idLinha.Equals(idLinha))].sDscJustificativa);
            else if (e.CommandName == "Download_Arquivo")
                Efetuar_Download_Arquivo(eBloco.Credito, idLinha);
            else if (e.CommandName == "Editar")
            {
                btnCredito_Incluir.Visible = false;
                btnCredito_Salvar.Visible = true;

                var Credito = bs_Credito[bs_Credito.FindIndex(x => x.idLinha.Equals(idLinha))];

                sDscJustificativa.Text = Credito.sDscJustificativa;
                txtCredito_dtInicioCredito.Text = string.Format("{0:yyyy-MM-dd}", Credito.dtInicioCredito);
                txtCredito_dtFinalCredito.Text = string.Format("{0:yyyy-MM-dd}", Credito.dtRetornoCredito);
                txtCredito_sObservacaoCredito.Text = Credito.sObservacaoCredito;
                ddlCreditaVR.SelectedValue = Credito.sCreditaVR;
                ddlCreditaVT.SelectedValue = Credito.sCreditaVT;
                ddlsTipoCredito.SelectedValue = Credito.sTipoCredito.ToString();

                sDscJustificativa.Focus();
            }

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab");
        }

        void dtgCredito_DataBind()
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_dtgCredito_DataBind", GRID.DataBindComScriptData(dtgCredito, bs_Credito.Where(c => c.sFuncao.ToString() != "EXCLUIR_CREDITO").ToList(), new int[2] { 3, 4 }, "asc", "false", "''"), true);

                GRID.SomarColunas(dtgCredito, true, GRID.Formatação.Numero, 2);
                RegistraScript("");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Creditos: " + ex.Message);
            }
        }

        protected void cmdPesquisarCredito_Click(object sender, EventArgs e) { Popular_dtgCredito(hddidColaborador.Value); FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Evento-tab"); }

        #endregion

        #endregion

        #region | Aba Benefícios

        #region | TipoBeneficio

        private void Popular_TipoBeneficio()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_Beneficio" },
                { "@idColaborador", hddidColaborador.Value }
            };

            try
            {

                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                {
                    if (dsPesquisa.Tables[0].Rows.Count > 0)
                    {
                        string sAusencia = RETORNO.DATASET(dsPesquisa, 0, "sAusencia_Completo");
                        string sOcorrencia = RETORNO.DATASET(dsPesquisa, 0, "sOcorrencia_Completo");

                        var row = dsPesquisa.Tables[0].Rows[0];
                        div_tipoBeneficio.Controls.Clear();

                        var h3 = new HtmlGenericControl("h3");
                        h3.Attributes["class"] = "tipoBeneficio-container";

                        var tipoBeneficioConfig = new Dictionary<string, (string sNome, string sCor)>
                    {
                        { "sVRDefinitivo", ("VR", "success") },
                        { "sOpcaoVT", ("VT", "warning") },
                        { "sPlanoSaude", ("Plano Saúde", "info") },
                        { "sPlanoOdonto", ("Plano Odontológico", "success") },
                        { "sSeguroVida", ("Seguro Vida", "primary") },
                        { "sDependentesConvenio", ("Dependente", "success") },
                        { "idVeiculo", ("Veículo", "warning") },
                        { "sAusencia", (sAusencia, "danger") },
                        { "sOcorrencia", (sOcorrencia, "danger") }
                    };

                        foreach (var config in tipoBeneficioConfig)
                        {
                            string sColuna = config.Key;
                            string sNomeExibicao = config.Value.sNome;
                            string sCor = config.Value.sCor;

                            if (row.Table.Columns.Contains(sColuna) && row[sColuna] != DBNull.Value && row[sColuna].ToString().Trim().ToUpper() == "S")
                            {
                                var span = new HtmlGenericControl("span");
                                span.Attributes["class"] = "status";
                                span.Attributes["title"] = sNomeExibicao;

                                var label = new Label();
                                label.Text = sNomeExibicao;
                                label.CssClass = $"label label-{sCor}";

                                if (config.Key == "sAusencia")
                                {
                                    span.Attributes["onclick"] = "$('#Evento-tab').tab('show');";
                                }

                                span.Controls.Add(label);
                                h3.Controls.Add(span);
                            }
                        }

                        div_tipoBeneficio.Controls.Add(h3);
                    }

                }

            }
            catch (Exception ex)
            {

                
            }
        }

        protected void ddlSeguroVida_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSeguroVida.SelectedValue == "S")
                div_tipoSeguro.Visible = true;
            else
                div_tipoSeguro.Visible = false;

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }
        #endregion

        #region | VT 

        void Popular_dtgVT(DataSet dsPesquisa)
        {
            BtnSalvarVT.Visible = false;

            foreach (DataRow row in dsPesquisa.Tables[nTabela_VT].Rows)
            {
                cls_VT objItem = new cls_VT();

                objItem.idLinha = bs_VT.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";
                objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                objItem.idRegistroVT = Convert.ToInt32(row["idRegistroVT"].ToString());
                objItem.sNumeroVT = row["sNumeroVT"].ToString();
                objItem.nValorVT = BD.Conversoes.Numerico_Decimal(row["nValorVT"].ToString());
                objItem.sDscMotivo = row["sDscMotivo"].ToString();
                objItem.dtVT = row["dtVT"].ToString();
                objItem.sVTDefinitivo = row["sVTDefinitivo"].ToString();
                objItem.sOpcaoVT = row["sOpcaoVT"].ToString();
                objItem.sTipoVt = row["sTipoVt"].ToString();
                objItem.idVTDefinitivo = Convert.ToInt32(row["idVTDefinitivo"].ToString());
                objItem.idOpcaoVT = Convert.ToInt32(row["idOpcaoVT"].ToString());
                objItem.idTipoVT = Convert.ToInt32(row["idTipoVT"].ToString());
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                objItem.sTipoVtOutros = row["sTipoVTOutros"].ToString();

                bs_VT.Add(objItem);
            }

            dtgVT_DataBind();
            LimpaCampos_VT();
        }

        protected void cmdVT_Incluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_VT(ref sMensagem))
            {
                cls_VT objItem = new cls_VT();

                string[] vidColaborador = hddidColaborador.Value.Split(',');
                string idColaborador = vidColaborador[0].ToString();

                objItem.idLinha = bs_VT.Count() + 1;
                objItem.sFuncao = "SALVAR_VT";
                objItem.idColaborador = Convert.ToInt32(idColaborador);
                objItem.sNumeroVT = txtVT_sNumeroVT.Text;
                objItem.nValorVT = BD.Conversoes.Numerico_Decimal(txtVT_nValorVT.Text);
                objItem.sDscMotivo = txtVT_sDscMotivo.Text;
                objItem.dtVT = txtVT_dtVT.Text;
                objItem.sTipoVt = ddlVT_idTipoVT.SelectedValue == "99" ? string.Format("Outros - {0}", txtVtTipoOutro.Text) : ddlVT_idTipoVT.SelectedValue == "0" ? string.Empty : ddlVT_idTipoVT.SelectedItem.ToString();
                objItem.sTipoVtOutros = ddlVT_idTipoVT.SelectedValue == "99" ? txtVtTipoOutro.Text : string.Empty;
                objItem.sVTDefinitivo = ddlVT_idVTDefinitivo.SelectedValue == "0" ? string.Empty : ddlVT_idVTDefinitivo.SelectedItem.ToString();
                objItem.sOpcaoVT = ddlVT_idOpcaoVT.SelectedItem.ToString();
                objItem.idVTDefinitivo = Convert.ToInt32(ddlVT_idVTDefinitivo.SelectedValue.ToString());
                objItem.idOpcaoVT = Convert.ToInt32(ddlVT_idOpcaoVT.SelectedValue.ToString());
                objItem.idTipoVT = Convert.ToInt32(ddlVT_idTipoVT.SelectedValue.ToString());
                objItem.sNomeArquivo = "";

                bs_VT.Add(objItem);

                dtgVT_DataBind();
                LimpaCampos_VT();
            }
            else
                MensagemVT.MostraMensagem_Erro(sMensagem, false);

            RegistraScript("$('[id$=ddlVT_sTipo]').focus();");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        bool Salvar_VT(string idColaborador)
        {
            bool bRetorno = false;

            try
            {
                foreach (var Linha in bs_VT)
                {
                    int idArquivo = Linha.idArquivo;
                    if (Linha.sNomeArquivo != "" && Linha.idArquivo == 0)
                    {
                        cls_Arquivos Arquivo = new cls_Arquivos();
                        Arquivo.idTipoArquivo = 9999;
                        Arquivo.idObjeto = Linha.idRegistroVT;
                        Arquivo.sNomeArquivo = Linha.sNomeArquivo;
                        Arquivo.sDscArquivo = Linha.sObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        Arquivo.vbArquivo = Linha.objArquivo;
                        Arquivo.dtExpiracaoDoc = "";

                        Linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (Linha.sFuncao == "SEM ALTERAÇÃO")
                            Linha.sFuncao = "SALVAR_VT";
                    }

                    if (Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<string, string> vParametroItensVT_Incluir = new Dictionary<string, string>
                        {
                            { "@sFuncao", Linha.sFuncao },
                            { "@idRegistro", Linha.idRegistroVT.ToString() },
                            { "@idColaborador", idColaborador },
                            { "@sNumeroVT", Linha.sNumeroVT },
                            { "@nValorVT", Linha.nValorVT.ToString().Replace(",", ".") },
                            { "@sDscMotivo", Linha.sDscMotivo },
                            { "@dtVT", Linha.dtVT.ToString() },
                            { "@sVTDefinitivo", Linha.sVTDefinitivo.ToString() },
                            { "@sOpcaoVT", Linha.sOpcaoVT.ToString() },
                            { "@sTipoVT", Linha.idTipoVT.ToString() == "99" ? "Outros" : Linha.sTipoVt.ToString() },
                            { "@idVTDefinitivo", Linha.idVTDefinitivo.ToString() },
                            { "@idOpcaoVT", Linha.idOpcaoVT.ToString() },
                            { "@idTipoVT", Linha.idTipoVT.ToString() },
                            { "@idArquivo", Linha.idArquivo.ToString() },
                            { "@sTipoVTOutros", Linha.sTipoVtOutros.ToString() }
                        };
                        BD.ExecutarDataSet(sProcedure, vParametroItensVT_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Bloco VT - Erro: " + ex.Message);
            }

            return bRetorno;
        }

        protected void BtnSalvarVT_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            int idLinha = Convert.ToInt32(hddVT_idLinha.Value);
            int index = bs_VT.FindIndex(x => x.idLinha.Equals(idLinha));

            if (ValidarDados_VT(ref sMensagem))
            {
                bs_VT[index].sNumeroVT = txtVT_sNumeroVT.Text;
                bs_VT[index].sDscMotivo = txtVT_sDscMotivo.Text;
                bs_VT[index].dtVT = txtVT_dtVT.Text;
                bs_VT[index].nValorVT = BD.Conversoes.Numerico_Decimal(txtVT_nValorVT.Text);
                bs_VT[index].idOpcaoVT = Convert.ToInt32(ddlVT_idOpcaoVT.SelectedValue);
                bs_VT[index].idTipoVT = Convert.ToInt32(ddlVT_idTipoVT.SelectedValue);
                bs_VT[index].idVTDefinitivo = Convert.ToInt32(ddlVT_idVTDefinitivo.SelectedValue);
                bs_VT[index].sTipoVt = ddlVT_idTipoVT.SelectedValue == "99" ? string.Format("Outros - {0}", txtVtTipoOutro.Text) : ddlVT_idTipoVT.SelectedValue == "0" ? string.Empty : ddlVT_idTipoVT.SelectedItem.ToString();
                bs_VT[index].sVTDefinitivo = ddlVT_idVTDefinitivo.SelectedValue == "0" ? string.Empty : ddlVT_idVTDefinitivo.SelectedItem.ToString();
                bs_VT[index].sOpcaoVT = ddlVT_idOpcaoVT.SelectedItem.ToString();
                bs_VT[index].sTipoVtOutros = ddlVT_idTipoVT.SelectedValue == "99" ? txtVtTipoOutro.Text : "";
                bs_VT[index].sFuncao = "SALVAR_VT";

                dtgVT_DataBind();
                LimpaCampos_VT();
            }
            else
                MensagemVT.MostraMensagem_Erro(sMensagem, false);

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        void LimpaCampos_VT()
        {
            Div_ddlVT_sVTDefinitivo.Visible = true;
            Div_ddlVT_sVTDefinitivo.Visible = true;
            Div_nValorVT.Visible = true;
            Div_txtVT_sNumeroVT.Visible = true;
            Div_ddlVT_sTipo.Visible = true;
            Div_txtVT_sDscMotivo.Visible = false;

            ddlVT_idOpcaoVT.SelectedValue = "1";
            ddlVT_idVTDefinitivo.SelectedValue = "0";
            ddlVT_idTipoVT.SelectedValue = "0";
            txtVT_sNumeroVT.Text = "";
            txtVT_nValorVT.Text = "";
            txtVT_sDscMotivo.Text = "";
            txtVT_dtVT.Text = "";
            hddVT_idLinha.Value = "";
            txtVtTipoOutro.Text = "";

            BtnSalvarVT.Visible = false;
            cmdVT_Incluir.Visible = true;
        }

        private bool ValidarDados_VT(ref string sMensagemErro)
        {
            string opcaoSelecionada = ddlVT_idOpcaoVT.SelectedValue;

            if (ddlVT_idOpcaoVT.SelectedValue == "")
                sMensagemErro += "Selecione uma Opção";

            if (opcaoSelecionada == "1")
            {
                if (ddlVT_idTipoVT.SelectedValue == "0")
                    sMensagemErro += "Selecione um tipo de VT";
                else if (ddlVT_idTipoVT.SelectedValue == "99")
                {
                    if (string.IsNullOrEmpty(txtVtTipoOutro.Text))
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe qual seria o outro Tipo de VT no campo \"Outros\"";
                }

                if (string.IsNullOrEmpty(txtVT_sNumeroVT.Text))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o número do Cartão";

                if (!Validacoes.ValidarMoeda(txtVT_nValorVT))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Valor do VT";
            }
            else
            {
                if (string.IsNullOrEmpty(txtVT_sDscMotivo.Text))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Motivo";
            }

            return sMensagemErro != "" ? false : true;
        }

        protected void dtgVT_RowDeleting1(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtgVT.Rows[e.RowIndex].Cells[0].Text);
            bs_VT[bs_VT.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_VT";
            dtgVT_DataBind();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        protected void dtgVT_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkVT_Download" && sNomeArquivo == "") || (lnk.ID == "lnkVT_UpLoad" && sNomeArquivo != ""))
                        lnk.Visible = false;
                }
            }

            GRID.EsconderColunas(e, 0, 1);
        }

        protected void dtgVT_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "VT";
            hddVT_idLinha.Value = idLinha.ToString();

            if (e.CommandName == "Upload_Arquivo")
                AbrirModal_EnvioArquivo(eBloco.VT, idLinha, bs_VT[bs_VT.FindIndex(x => x.idLinha.Equals(idLinha))].sTipoVt);
            else if (e.CommandName == "Download_Arquivo")
                Efetuar_Download_Arquivo(eBloco.VT, idLinha);
            else if (e.CommandName == "Editar")
            {
                cmdVT_Incluir.Visible = false;
                BtnSalvarVT.Visible = true;

                var VT = bs_VT[bs_VT.FindIndex(x => x.idLinha.Equals(idLinha))];

                txtVT_sNumeroVT.Text = VT.sNumeroVT;
                txtVT_sDscMotivo.Text = VT.sDscMotivo;
                txtVT_dtVT.Text = VT.dtVT;
                txtVT_nValorVT.Text = VT.nValorVT.ToString();
                ddlVT_idOpcaoVT.SelectedValue = VT.idOpcaoVT.ToString();
                ddlVT_idTipoVT.SelectedValue = VT.idTipoVT.ToString();
                ddlVT_idVTDefinitivo.SelectedValue = VT.idVTDefinitivo.ToString();
                txtVtTipoOutro.Text = VT.sTipoVtOutros.ToString();

                txtVT_sDscMotivo.Focus();
            }

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        void dtgVT_DataBind()
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_dtgVT_DataBind", GRID.DataBindComScriptData(dtgVT, bs_VT.Where(c => c.sFuncao.ToString() != "EXCLUIR_VT").ToList(), 5, "asc", "false", "''"), true);

                GRID.SomarColunas(dtgVT, true, GRID.Formatação.Moeda, 4);
                RegistraScript("");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de VT: " + ex.Message);
            }
        }

        protected void ddlVT_idTipoVT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlVT_idTipoVT.SelectedValue == "99")
                div_tipoVT_outros.Visible = true;
            else
                div_tipoVT_outros.Visible = false;

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        protected void ddlVT_sOpcaoVT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlVT_idOpcaoVT.SelectedValue == "1")
            {
                Div_ddlVT_sVTDefinitivo.Visible = true;
                Div_ddlVT_sVTDefinitivo.Visible = true;
                Div_nValorVT.Visible = true;
                Div_txtVT_sNumeroVT.Visible = true;
                Div_ddlVT_sTipo.Visible = true;
                Div_txtVT_sDscMotivo.Visible = false;
            }
            else
            {
                Div_ddlVT_sVTDefinitivo.Visible = false;
                Div_ddlVT_sVTDefinitivo.Visible = false;
                Div_nValorVT.Visible = false;
                Div_txtVT_sNumeroVT.Visible = false;
                Div_ddlVT_sTipo.Visible = false;
                Div_txtVT_sDscMotivo.Visible = true;
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        #endregion

        #region | Equipamentos 

        void Popular_dtgEquipamentos(DataSet dsPesquisa)
        {
            BtnSalvarEquipamento.Visible = false;

            foreach (DataRow row in dsPesquisa.Tables[nTabela_Equipamentos].Rows)
            {
                cls_Equipamentos objItem = new cls_Equipamentos();

                objItem.idLinha = bs_Equipamentos.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";
                objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                objItem.idRegistroEquipamento = Convert.ToInt32(row["idRegistroEquipamento"].ToString());
                objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                objItem.idEquipamento = Convert.ToInt32(row["idEquipamento"].ToString());
                objItem.sDcsTipoEquipamento = row["sDcsTipoEquipamento"].ToString();
                objItem.sModeloEquipamento = row["sModeloEquipamento"].ToString();
                objItem.sCodTTEquipamento = row["sCodTTEquipamento"].ToString();
                objItem.dtRecebimentoEquipamento = row["dtRecebimentoEquipamento"].ToString();
                objItem.dtDevolucaoEquipamento = row["dtDevolucaoEquipamento"].ToString();
                objItem.sObservacaoEquipamento = row["sObservacaoEquipamento"].ToString();
                objItem.idVeiculo = Convert.ToInt32(row["idVeiculo"].ToString());
                objItem.sVeiculoEquipamentos = row["sVeiculoEquipamentos"].ToString();
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());

                bs_Equipamentos.Add(objItem);
            }

            dtgEquipamentos_DataBind();
            Equipamentos_LimpaCampos();
        }

        protected void cmdIncluirEquipamentos_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (ValidarDados_Equipamentos(ref sMensagem))
            {
                cls_Equipamentos objItem = new cls_Equipamentos();

                string[] vidColaborador = hddidColaborador.Value.Split(',');
                string idColaborador = vidColaborador[0].ToString();

                objItem.idLinha = bs_Equipamentos.Count() + 1;
                objItem.sFuncao = "SALVAR_EQUIPAMENTOS";
                objItem.idColaborador = Convert.ToInt32(idColaborador);
                objItem.sDcsTipoEquipamento = txtsDcsTipoEquipamento.Text;
                objItem.sModeloEquipamento = txtsModeloEquipamento.Text;
                objItem.sCodTTEquipamento = txtsCodTTEquipamento.Text;
                objItem.dtRecebimentoEquipamento = txtdtRecebimentoEquipamento.Text;
                objItem.dtDevolucaoEquipamento = txtdtDevolucaoEquipamento.Text;
                objItem.sObservacaoEquipamento = txtsObservacaoEquipamento.Text;
                objItem.idVeiculo = Convert.ToInt32(ddlidVeiculoEquipamentos.SelectedValue);
                objItem.sVeiculoEquipamentos = ddlidVeiculoEquipamentos.SelectedItem.ToString();
                objItem.sNomeArquivo = "";

                bs_Equipamentos.Add(objItem);

                dtgEquipamentos_DataBind();
                Equipamentos_LimpaCampos();
            }
            else
                MensagemEquipamentos.MostraMensagem_Erro(sMensagem);

            RegistraScript("$('[id$=txtsDcsTipoEquipamento]').focus();");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        bool Salvar_Equipamentos(string idColaborador)
        {
            bool bRetorno = false;

            try
            {
                foreach (var Equipamentos_linha in bs_Equipamentos)
                {
                    if (idColaborador != "0")
                    {
                        int idArquivo = Equipamentos_linha.idArquivo;
                        if (Equipamentos_linha.sNomeArquivo != "" && Equipamentos_linha.idArquivo == 0)
                        {
                            cls_Arquivos Arquivo = new cls_Arquivos();
                            Arquivo.idTipoArquivo = 9999;
                            Arquivo.idObjeto = Equipamentos_linha.idRegistroEquipamento;
                            Arquivo.sNomeArquivo = Equipamentos_linha.sNomeArquivo;
                            Arquivo.sDscArquivo = Equipamentos_linha.sObservacaoArquivo;
                            Arquivo.sObservacao = "";
                            Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                            Arquivo.vbArquivo = Equipamentos_linha.objArquivo;
                            Arquivo.dtExpiracaoDoc = "";

                            Equipamentos_linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                            if (Equipamentos_linha.sFuncao == "SEM ALTERAÇÃO")
                                Equipamentos_linha.sFuncao = "SALVAR_EQUIPAMENTOS";
                        }

                        if (Equipamentos_linha.sFuncao != "SEM ALTERAÇÃO")
                        {
                            Dictionary<string, string> vParametroEquipamentos_Incluir = new Dictionary<string, string>
                            {
                                { "@sFuncao", "SALVAR_EQUIPAMENTOS" },
                                { "@idRegistro", Equipamentos_linha.idRegistroEquipamento.ToString() },
                                { "@idColaborador", idColaborador },
                                { "@idEquipamento", Equipamentos_linha.idEquipamento.ToString() },
                                { "@sDcsTipoEquipamento", Equipamentos_linha.sDcsTipoEquipamento },
                                { "@sModeloEquipamento", Equipamentos_linha.sModeloEquipamento },
                                { "@sCodTTEquipamento", Equipamentos_linha.sCodTTEquipamento },
                                { "@dtRecebimentoEquipamento", Equipamentos_linha.dtRecebimentoEquipamento },
                                { "@dtDevolucaoEquipamento", Equipamentos_linha.dtDevolucaoEquipamento },
                                { "@sObservacaoEquipamento", Equipamentos_linha.sObservacaoEquipamento },
                                { "@sVeiculoEquipamentos", Equipamentos_linha.sVeiculoEquipamentos },
                                { "@idVeiculo", Equipamentos_linha.idVeiculo.ToString() },
                                { "@idArquivo", Equipamentos_linha.idArquivo.ToString() }
                            };
                            DataSet dsEquipamentos_Incluir = BD.ExecutarDataSet(sProcedure, vParametroEquipamentos_Incluir);
                        }
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Bloco Equipamentos - Erro: " + ex.Message);
            }

            return bRetorno;
        }

        protected void BtnSalvarEquipamento_Click(object sender, EventArgs e)
        {
            int idLinha = Convert.ToInt32(hddEquipamento_idLinha.Value);
            int index = bs_Equipamentos.FindIndex(x => x.idLinha.Equals(idLinha));

            bs_Equipamentos[index].sDcsTipoEquipamento = txtsDcsTipoEquipamento.Text;
            bs_Equipamentos[index].idVeiculo = Convert.ToInt32(ddlidVeiculoEquipamentos.SelectedValue);
            bs_Equipamentos[index].sModeloEquipamento = txtsModeloEquipamento.Text;
            bs_Equipamentos[index].sCodTTEquipamento = txtsCodTTEquipamento.Text;
            bs_Equipamentos[index].dtRecebimentoEquipamento = txtdtRecebimentoEquipamento.Text;
            bs_Equipamentos[index].dtDevolucaoEquipamento = txtdtDevolucaoEquipamento.Text;
            bs_Equipamentos[index].sObservacaoEquipamento = txtsObservacaoEquipamento.Text;
            bs_Equipamentos[index].sFuncao = "SALVAR_EQUIPAMENTOS";

            dtgEquipamentos_DataBind();
            Equipamentos_LimpaCampos();
            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        void Equipamentos_LimpaCampos()
        {
            txtsDcsTipoEquipamento.Text = "";
            txtsModeloEquipamento.Text = "";
            txtsCodTTEquipamento.Text = "";
            txtdtRecebimentoEquipamento.Text = "";
            txtdtDevolucaoEquipamento.Text = "";
            txtsObservacaoEquipamento.Text = "";
            ddlidVeiculoEquipamentos.SelectedValue = "0";

            BtnSalvarEquipamento.Visible = false;
            cmdEquipamentos_Incluir.Visible = true;
        }

        private bool ValidarDados_Equipamentos(ref string sMensagemErro)
        {
            if (string.IsNullOrEmpty(txtsDcsTipoEquipamento.Text))
                sMensagemErro = "Informe a descrição do equipamento";

            if (!Validacoes.ValidarData(txtdtRecebimentoEquipamento))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma data válida de entrega.";

            return sMensagemErro != "" ? false : true;
        }

        protected void dtgEquipamentos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtgEquipamentos.Rows[e.RowIndex].Cells[0].Text);
            bs_Equipamentos[bs_Equipamentos.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_EQUIPAMENTOS";
            dtgEquipamentos_DataBind();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        protected void dtgEquipamentos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkEquipamento_Download" && sNomeArquivo == "") || (lnk.ID == "lnkEquipamento_UpLoad" && sNomeArquivo != ""))
                        lnk.Visible = false;
                }
            }

            GRID.EsconderColunas(e, 0);
        }

        protected void dtgEquipamentos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "Equipamento";
            hddEquipamento_idLinha.Value = idLinha.ToString();

            if (e.CommandName == "Upload_Arquivo")
                AbrirModal_EnvioArquivo(eBloco.Equipamento, idLinha, bs_Equipamentos[bs_Equipamentos.FindIndex(x => x.idLinha.Equals(idLinha))].sDcsTipoEquipamento);
            else if (e.CommandName == "Download_Arquivo")
                Efetuar_Download_Arquivo(eBloco.Equipamento, idLinha);
            else if (e.CommandName == "Editar")
            {
                cmdEquipamentos_Incluir.Visible = false;
                BtnSalvarEquipamento.Visible = true;

                var Equip = bs_Equipamentos[bs_Equipamentos.FindIndex(x => x.idLinha.Equals(idLinha))];

                ddlidVeiculoEquipamentos.SelectedValue = Equip.idVeiculo.ToString();
                txtsDcsTipoEquipamento.Text = Equip.sDcsTipoEquipamento;
                txtsModeloEquipamento.Text = Equip.sModeloEquipamento;
                txtsCodTTEquipamento.Text = Equip.sCodTTEquipamento;
                txtdtRecebimentoEquipamento.Text = Equip.dtRecebimentoEquipamento;
                txtdtDevolucaoEquipamento.Text = Equip.dtDevolucaoEquipamento;
                txtsObservacaoEquipamento.Text = Equip.sObservacaoEquipamento;

                txtsDcsTipoEquipamento.Focus();
            }

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        void dtgEquipamentos_DataBind() => ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_dtgEquipamentos_DataBind", GRID.DataBindComScriptData(dtgEquipamentos, bs_Equipamentos.ToList(), new int[2] { 3, 4 }, "asc", "false", "''"), true);

        #endregion

        #region | Plano de Saude

        void Popular_gv_PlanoSaude(DataSet dsPesquisa)
        {
            bs_PlanoSaude.Clear();
            if (dsPesquisa.Tables[nTabela_PlanoSaude].Rows.Count > 0 && ddlTemPlanoSaude.SelectedValue == "S")
            {
                foreach (DataRow row in dsPesquisa.Tables[nTabela_PlanoSaude].Rows)
                {
                    cls_PlanoSaude_Itens objItem = new cls_PlanoSaude_Itens();

                    objItem.idLinha = bs_PlanoSaude.Count() + 1;
                    objItem.sFuncao = "SEM ALTERAÇÃO";
                    objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                    objItem.idDependente = Convert.ToInt32(row["idRegistroDependentes"].ToString());
                    objItem.idPlanoSaude = Convert.ToInt32(row["idPlanoSaude"].ToString());
                    objItem.sDscColaborador = row["sDscColaborador"].ToString();
                    objItem.sNomeDependente = row["sNomeDependente"].ToString();
                    objItem.sDscPlanoSaude = row["sDscPlanoSaude"].ToString();
                    objItem.dtInclusao = row["dtInclusaoConvenio"].ToString();
                    objItem.dtFimCarencia = row["dtCarenciaConvenio"].ToString();
                    objItem.nValorPlano = Convert.ToDecimal(row["nValorPlano"]);
                    objItem.sDscBeneficiario = row["idRegistroDependentes"].ToString() == "0" ? row["sDscColaborador"].ToString() : row["sNomeDependente"].ToString();
                    objItem.sTipo = row["sTipo"].ToString();
                    objItem.sTipoBeneficiario = row["sTipoBeneficiario"].ToString();

                    bs_PlanoSaude.Add(objItem);
                }

                ddlTemPlanoSaude.Attributes.Add("disabled", "disabled");
            }
            else
            {
                btDownloadArquivo.Visible = true;
                ddlTemPlanoSaude.Attributes.Remove("disabled");
            }

            gv_PlanoSaude_DataBind();
        }

        private void gv_PlanoSaude_DataBind()
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_gvPlanoSaude_DataBind", GRID.DataBindComScriptData(gv_PlanoSaude, bs_PlanoSaude.Where(c => c.sFuncao.ToString() != "EXCLUIR_Plano").ToList(), 1, new int[2] { 4, 5 }, "asc", "false", "''"), true);
                Grid.SomarColunas(gv_PlanoSaude, true, Grid.Formatação.Moeda, 7);

                RegistraScript("");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Itens: " + ex.Message);
            }
        }

        private void LimpaCampos_PlanoSaude()
        {
            ddlidPlanoSaude.SelectedValue = "0";
            ddlidDependente.SelectedValue = "0";
            ddlsBeneficiario.SelectedValue = "0";
            txtdtInclusaoConvenio.Text = "";
            txtdtCarenciaConvenio.Text = "";
            txtnValorPlano.Text = "";
        }

        bool Salvar_PlanoSaude_Itens(string idColaborador)
        {
            bool bRetorno = false;

            try
            {
                if (ddlTemPlanoSaude.SelectedValue == "S")
                {
                    foreach (var linha in bs_PlanoSaude)
                    {
                        if (linha.sFuncao != "SEM ALTERAÇÃO")
                        {
                            Dictionary<string, string> vParametroItens_Incluir = new Dictionary<string, string>
                            {
                                ["@sFuncao"] = linha.sFuncao,
                                ["@idPlanoSaude"] = linha.idPlanoSaude.ToString(),
                                ["@idColaborador"] = idColaborador,
                                ["@idDependente"] = linha.idDependente.ToString(),
                                ["@dtInclusaoConvenio"] = Convert.ToDateTime(linha.dtInclusao).ToString("yyyy-MM-dd"),
                                ["@dtCarenciaConvenio"] = Convert.ToDateTime(linha.dtFimCarencia).ToString("yyyy-MM-dd"),
                                ["@nValorPlano"] = linha.nValorPlano.ToString().Replace(".", "").Replace(",", "."),
                                ["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario()
                            };
                            BD.ExecutarDataSet(sProcedure, vParametroItens_Incluir);
                        }
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Bloco Plano Saúde - Erro: " + ex.Message);
            }

            return bRetorno;
        }

        protected void ddlTemPlanoSaude_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTemPlanoSaude.SelectedValue == "S")
            {
                div_btnIncluirPlanoSaude.Visible = true;
                div_arquivoPlanoSaude.Visible = false;
            }
            else if (ddlTemPlanoSaude.SelectedValue == "N")
            {
                div_arquivoPlanoSaude.Visible = true;
                div_btnIncluirPlanoSaude.Visible = false;
            }
            else
            {
                div_btnIncluirPlanoSaude.Visible = false;
                div_arquivoPlanoSaude.Visible = false;
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_MantemAbs_Ativa", " $('#beneficios-tab').tab('show');", true);
        }

        protected void btnArquivoPlanoSaude_Click(object sender, EventArgs e)
        {
            if (hddidArquivoPlanoSaude.Value != "0")
            {
                Efetuar_Download_Arquivo(eBloco.PlanoSaude, 0, false);
            }
            else
            {
                AbrirModal_EnvioArquivo(eBloco.PlanoSaude, 0, "Carta de Desinteresse");
            }

        }

        protected void btnArquivoConversa_Click(object sender, EventArgs e)
        {
            if (hddidArquivoConversa.Value != "0" && hddidArquivoConversa.Value != "")
            {
                Efetuar_Download_Arquivo(eBloco.Conversas, int.Parse(hddConversa_idRegistroConvesa.Value), false);
            }
            else
            {
                AbrirModal_EnvioArquivo(eBloco.Conversas, int.Parse(hddConversa_idRegistroConvesa.Value), "Envio de Arquivos");
            }

        }

        protected void btnIncluirPlanoSaude_Click(object sender, EventArgs e)
        {
            LimpaCampos_PlanoSaude();
            div_ddlDependente.Visible = false;
            FUNCOES.Popula_Combo(ddlidDependente, "sp_Select 'Flow_Colaborador_Dependente', @idPesquisa=" + hddidColaborador.Value, "idRegistroDependentes", "sNomeDependente", false, "Selecione o Dependente", "0");
            RegistraScript("");
            Modal_Abrir("modal_PlanoSaude");
        }

        protected void gv_PlanoSaude_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        protected void gv_PlanoSaude_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(gv_PlanoSaude.Rows[e.RowIndex].Cells[0].Text);
            bs_PlanoSaude[bs_PlanoSaude.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_Plano";

            if (bs_PlanoSaude.All(x => x.sFuncao == "EXCLUIR_Plano"))
                ddlTemPlanoSaude.Attributes.Remove("disabled");

            RecarregaDatatable();
            MensagemPagina_PlanoSaude.MostraMensagem_Sucesso("Registro excluído! Para persistir clique em Salvar");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        protected void gv_PlanoSaude_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddPlanoSaude_idLinha.Value = idLinha.ToString();
            FUNCOES.Popula_Combo(ddlidDependente, "sp_Select 'Flow_Colaborador_Dependente', @idPesquisa=" + hddidColaborador.Value, "idRegistroDependentes", "sNomeDependente", false, "Selecione o Dependente", "0");
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (e.CommandName == "Editar")
            {
                var planoSaude = bs_PlanoSaude[bs_PlanoSaude.FindIndex(x => x.idLinha.Equals(idLinha))];
                ddlidPlanoSaude.SelectedValue = planoSaude.idPlanoSaude.ToString();
                ddlsBeneficiario.SelectedValue = planoSaude.idDependente.ToString() == "0" ? "0" : "1";
                ddlidDependente.SelectedValue = planoSaude.idDependente.ToString();
                txtdtInclusaoConvenio.Text = Convert.ToDateTime(planoSaude.dtInclusao).ToString("yyyy-MM-dd");
                txtdtCarenciaConvenio.Text = Convert.ToDateTime(planoSaude.dtFimCarencia).ToString("yyyy-MM-dd");
                txtnValorPlano.Text = planoSaude.nValorPlano.ToString("N2");
                ddlidPlanoSaude_SelectedIndexChanged(objSender, objEventArgs);
                Modal_Abrir("modal_PlanoSaude");
            }

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        protected void ddlsBeneficiario_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlsBeneficiario.SelectedValue == "0")
            {
                div_ddlDependente.Visible = false;
            }
            else
            {
                div_ddlDependente.Visible = true;
            }
        }

        protected void txtdtInclusaoConvenio_TextChanged(object sender, EventArgs e)
        {
            if (ddlidPlanoSaude.SelectedValue != "0")
            {
                int nCarencia = Convert.ToInt32(hddPlanoSaude_Carencia.Value);

                if (nCarencia != 0)
                {
                    DateTime dtInclusao = Convert.ToDateTime(txtdtInclusaoConvenio.Text);
                    dtInclusao = dtInclusao.AddDays(nCarencia);
                    txtdtCarenciaConvenio.Text = dtInclusao.ToString("dd/MM/yyyy");
                }

                FUNCOES.Scripts.FocusScript(Page, txtdtCarenciaConvenio.ClientID);
            }
            else
            {
                txtdtEmissaoNR.Text = "";
                MensagemPaginaPlanoSaude.MostraMensagem_Erro("Selecione um Plano de Saúde");
            }

            RegistraScript("");
            //Modal_Abrir("modal_PlanoSaude");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        protected void btnSalvarPlanoSaude_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_PlanoSaude(ref sMensagem))
            {
                if (hddPlanoSaude_idLinha.Value == "0")
                {
                    cls_PlanoSaude_Itens objItem = new cls_PlanoSaude_Itens();

                    string[] vidColaborador = hddidColaborador.Value.Split(',');
                    string idColaborador = vidColaborador[0].ToString();

                    objItem.idLinha = bs_PlanoSaude.Count() + 1;
                    objItem.sFuncao = "SALVAR_PlanoSaude";
                    objItem.idColaborador = Convert.ToInt32(idColaborador);
                    objItem.idPlanoSaude = Convert.ToInt32(ddlidPlanoSaude.SelectedValue);
                    objItem.idDependente = ddlsBeneficiario.SelectedValue == "1" ? Convert.ToInt32(ddlidDependente.SelectedValue) : 0;
                    objItem.sDscColaborador = txtsDscColaborador.Text.ToString();
                    objItem.sNomeDependente = ddlidDependente.SelectedItem.ToString();
                    objItem.sDscPlanoSaude = ddlidPlanoSaude.SelectedItem.ToString();
                    objItem.dtInclusao = Convert.ToDateTime(txtdtInclusaoConvenio.Text).ToString("dd/MM/yyyy");
                    objItem.dtFimCarencia = Convert.ToDateTime(txtdtCarenciaConvenio.Text).ToString("dd/MM/yyyy");
                    objItem.sDscBeneficiario = ddlidDependente.SelectedValue == "0" ? txtsDscColaborador.Text.ToString() : ddlidDependente.SelectedItem.ToString();
                    objItem.sTipo = hddPlanoSaude_sTipo.Value;
                    objItem.nValorPlano = Convert.ToDecimal(txtnValorPlano.Text);
                    objItem.sTipoBeneficiario = ddlidDependente.SelectedValue == "0" ? "Colaborador" : "Dependente";

                    bs_PlanoSaude.Add(objItem);
                    sMensagem = "Registro Incluído! Para persistir clique em Salvar";
                }
                else
                {
                    int idLinha = Convert.ToInt32(hddPlanoSaude_idLinha.Value);
                    int index = bs_PlanoSaude.FindIndex(x => x.idLinha.Equals(idLinha));

                    bs_PlanoSaude[index].idPlanoSaude = Convert.ToInt32(ddlidPlanoSaude.SelectedValue);
                    bs_PlanoSaude[index].idDependente = ddlsBeneficiario.SelectedValue == "1" ? Convert.ToInt32(ddlidDependente.SelectedValue) : 0;
                    bs_PlanoSaude[index].sDscColaborador = txtsDscColaborador.Text.ToString();
                    bs_PlanoSaude[index].sNomeDependente = ddlidDependente.SelectedItem.ToString();
                    bs_PlanoSaude[index].sDscPlanoSaude = ddlidPlanoSaude.SelectedItem.ToString();
                    bs_PlanoSaude[index].dtInclusao = Convert.ToDateTime(txtdtInclusaoConvenio.Text).ToString("dd/MM/yyyy");
                    bs_PlanoSaude[index].dtFimCarencia = Convert.ToDateTime(txtdtCarenciaConvenio.Text).ToString("dd/MM/yyyy");
                    bs_PlanoSaude[index].sFuncao = "SALVAR_PlanoSaude";
                    bs_PlanoSaude[index].sDscBeneficiario = ddlidDependente.SelectedValue == "0" ? txtsDscColaborador.Text.ToString() : ddlidDependente.SelectedItem.ToString();
                    bs_PlanoSaude[index].sTipo = hddPlanoSaude_sTipo.Value;
                    bs_PlanoSaude[index].nValorPlano = Convert.ToDecimal(txtnValorPlano.Text);
                    bs_PlanoSaude[index].sTipoBeneficiario = ddlidDependente.SelectedValue == "0" ? "Colaborador" : "Dependente";

                    hddPlanoSaude_idLinha.Value = "0";

                    sMensagem = "Registro Editado! Para persistir clique em Salvar";
                }

                Modal_Fechar("modal_PlanoSaude");
                gv_PlanoSaude_DataBind();
                RecarregaDatatable();
                MensagemPagina_PlanoSaude.MostraMensagem_Sucesso(sMensagem);
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
            }
            else
            {
                MensagemPaginaPlanoSaude.MostraMensagem_Erro(sMensagem, false);
                RegistraScript("");
                Modal_Abrir("modal_PlanoSaude");
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
            }
        }

        private bool ValidarDados_PlanoSaude(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlidPlanoSaude.SelectedValue == "0")
                sMensagemErro += "Selecione um Plano de Saúde";

            if (ddlsBeneficiario.SelectedValue == "1" && ddlidDependente.SelectedValue == "0")
            {
                if (sMensagemErro != "")
                    sMensagemErro += "</br>";

                sMensagemErro += "Selecione um Dependente";
            }

            if (txtdtInclusaoConvenio.Text == "")
            {
                if (sMensagemErro != "")
                    sMensagemErro += "</br>";

                sMensagemErro += "Insira uma Data de Inclusão";
            }

            if (txtdtCarenciaConvenio.Text == "")
            {
                if (sMensagemErro != "")
                    sMensagemErro += "</br>";

                sMensagemErro += "Insira uma Data de Fim de Carência";
            }

            if (txtnValorPlano.Text == "")
            {
                if (sMensagemErro != "")
                    sMensagemErro += "</br>";

                sMensagemErro += "Insira o Valor do Plano";
            }

            string dt = Validacoes.ValidaDatas(txtdtInclusaoConvenio.Text, txtdtCarenciaConvenio.Text);
            sMensagemErro += string.IsNullOrEmpty(dt) ? string.Empty : "</br>" + dt;

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPaginaPlanoSaude.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void lbCancelaPlanoSaude_Click(object sender, EventArgs e)
        {
            Modal_Fechar("modal_PlanoSaude");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        protected void ddlidPlanoSaude_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidPlanoSaude.SelectedValue != "0")
            {
                Dictionary<string, string> vParametroItens = new Dictionary<string, string>
                {
                    ["@sFuncao"] = "Consulta_PlanoSaude",
                    ["@idPlanoSaude"] = ddlidPlanoSaude.SelectedValue
                };
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametroItens);
                if (BD.ValidarDataSet(ds, out string sErro))
                {
                    hddPlanoSaude_Carencia.Value = RETORNO.DATASET(ds, 0, "nCarencia") != "" ? RETORNO.DATASET(ds, 0, "nCarencia") : "0";
                    txtnValorPlano.Text = RETORNO.DATASET(ds, 0, "nValor");
                    hddPlanoSaude_sTipo.Value = RETORNO.DATASET(ds, 0, "sTipo");
                }
            }
        }

        #endregion

        #region | Controle Férias

        private void PopularControleFerias(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ControleFerias_DataBind", GRID.DataBindComScriptData(dtgv_ControleFerias, dt, 0, new int[2] { 0, 1 }, "asc", "false", "''"), true);
            }
            else
            {
                MensagemPaginaControleFerias.MostraMensagem_Erro("Nenhum Registro Encontrado");
            }
        }

        protected void btnFiltroFerias_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_ControleFerias_Filtro" },
                { "@idColaborador", hddidColaborador.Value },
                { "@dtInicial", txtdtInicioFerias.Text },
                { "@dtFinal", txtdtFinalFerias.Text }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
            {
                PopularControleFerias(dsPesquisa.Tables[0]);
            }
            else
            {
                MensagemPaginaControleFerias.MostraMensagem_Erro("Erro Controle Férias: " + sErro);
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "beneficios-tab");
        }

        #endregion

        #endregion

        #region | Aba Conversas

        void pesquisarConversas(string idColaborador, bool edicao)
        {
            aba_Conversas.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesConversas.Consultar);

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR-CONVERSAS" },
                { "@idColaborador", idColaborador },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

            DIV_ASSUNTO.Visible = true;
            DIV_CONVERSA.Visible = false;
            DIV_NOVOASSUNTO.Visible = true;
            ddlConversas_idStatusConversa.Visible = false;
            Div_resolucao.Visible = false;
            txtConversas_dtResolucaoConversa.Visible = false;
            btnSalvarStatus.Visible = false;
            btnArquivoChat.Visible = false;

            btnExcluir.Visible = false;

            if (edicao == true)
            {
                DIV_ASSUNTO.Visible = false;
                DIV_CONVERSA.Visible = true;
                DIV_NOVOASSUNTO.Visible = false;

                div_dtProxConv.Visible = true;
                div_statusConv.Visible = true;
                ddlConversas_idStatusConversa.Visible = true;
                txtConversas_dtProximaConversa.Visible = true;
                btnExcluir.Visible = true;

                Div_resolucao.Visible = true;
                txtConversas_dtResolucaoConversa.ReadOnly = false;
                btnSalvarStatus.Visible = true;
                btnArquivoChat.Visible = true;
            }

            rptConversas.DataSource = ds;
                rptConversas.DataBind();
            if (bs_Conversas.Count > 0)
            {
                if (bs_Conversas[0].idRegistroConvesa != 0)
                {
                    lnkConversasEditar_Click(bs_Conversas[0].idRegistroConvesa.ToString());
                }
            }
        }

        protected void rptConversas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRow row = ((DataRowView)e.Item.DataItem).Row;

                Literal litDtProximaConversa = e.Item.FindControl("litDtProximaConversa") as Literal;

                HtmlGenericControl pDtProxConv = e.Item.FindControl("pDtProxConv") as HtmlGenericControl;
                HtmlGenericControl sCor = e.Item.FindControl("sCor") as HtmlGenericControl;

                sCor.Attributes["class"] = DataBinder.Eval(e.Item.DataItem, "sCor").ToString();

                if (string.IsNullOrEmpty(litDtProximaConversa?.Text))
                {
                    if (pDtProxConv != null)
                        pDtProxConv.Visible = false;
                }
            }
        }

        protected void lnkConversasEditar_Click(object sender, EventArgs e) { string idRegistroConvesa = (sender as LinkButton).CommandArgument; lnkConversasEditar_Click(idRegistroConvesa); }

        protected void lnkConversasEditar_Click(string idConversa)
        {
            string idRegistroConvesa = idConversa;

            if (idRegistroConvesa != "0")
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONVERSA-DETALHES" },
                    { "@idRegistroConvesa", idRegistroConvesa },
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                };
                DataSet dsEditar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

                DIV_ASSUNTO.Visible = false;
                DIV_CONVERSA.Visible = true;
                DIV_NOVOASSUNTO.Visible = false;
                btnSalvarStatus.Visible = true;
                btnArquivoChat.Visible = true;
                btnExcluir.Visible = true;
                div_dtProxConv.Visible = true;
                div_statusConv.Visible = true;
                txtConversas_sDscConversa.ReadOnly = true;

                ExibeCombos(false);

                ddlConversas_idStatusConversa.Visible = true;
                txtConversas_dtProximaConversa.Visible = true;

                if (BD.ValidarDataSet(dsEditar, out string sErro))
                {
                    hddidArquivoConversa.Value = RETORNO.DATASET(dsEditar, 0, "idArquivo");
                    if (hddidArquivoConversa.Value != "0" && hddidArquivoConversa.Value != "") {
                        btnArquivoChat.Text = "Baixar Arquivo";
                    }
                    else
                    {
                        btnArquivoChat.Text = "Adicionar Arquivo";
                    }

                    hddConversa_idRegistroConvesa.Value = idRegistroConvesa;
                    txtConversas_sDscConversa.Text = RETORNO.DATASET(dsEditar, 0, "sDscConversa");
                    lblAssunto.Text = RETORNO.DATASET(dsEditar, 0, "sDscConversa");
                    txtConversas_dtResolucaoConversa.Text = RETORNO.DATASET(dsEditar, 0, "dtResolucaoConversa");

                    if (RETORNO.DATASET(dsEditar, 0, "dtProximaConversa").ToString() == "")
                        txtConversas_dtProximaConversa.Text = null;
                    else
                    {
                        var dt = Convert.ToDateTime(RETORNO.DATASET(dsEditar, 0, "dtProximaConversa").ToString());
                        txtConversas_dtProximaConversa.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                    }

                    ddlConversas_idAcao.SelectedValue = RETORNO.DATASET(dsEditar, 0, "idAcao");
                    ddlConversas_idMeio.SelectedValue = RETORNO.DATASET(dsEditar, 0, "idMeio");
                    ddlConversas_idStatusConversa.SelectedValue = RETORNO.DATASET(dsEditar, 0, "idStatusConversa");
                    ddlConversas_idTipoEvento.SelectedValue = RETORNO.DATASET(dsEditar, 0, "idTipoEvento");

                    popularHistorico(idRegistroConvesa);
                }
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "conversas-tab");
        }

        protected void timer_Atualizar_Tick(object sender, EventArgs e) { pesquisarConversas(hddidColaborador.Value, false); FUNCOES.Scripts.Mantem_AbaAtiva(Page, "conversas-tab"); }

        void ExibeCombos(bool exibe)
        {
            if (exibe == false)
            {
                ddlConversas_idTipoEvento.Attributes.Add("disabled", "disabled");
                ddlConversas_idAcao.Attributes.Add("disabled", "disabled");
                ddlConversas_idMeio.Attributes.Add("disabled", "disabled");
            }
            else
            {
                ddlConversas_idTipoEvento.Attributes.Remove("disabled");
                ddlConversas_idAcao.Attributes.Remove("disabled");
                ddlConversas_idMeio.Attributes.Remove("disabled");
            }
        }

        protected void popularHistorico(string idRegistroConvesa)
        {
            btnSalvarStatus.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesConversas.Alterar);
            btnExcluir.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesConversas.Excluir);

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONVERSA-HISTORICO" },
                { "@idRegistroConvesa", idRegistroConvesa }
            };
            DataSet dsHistorico = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

            rptHistorico.DataSource = dsHistorico;
            rptHistorico.DataBind();
        }

        protected void cmdConversas_Cancelar_Click(object sender, EventArgs e) { pesquisarConversas(hddidColaborador.Value, false); FUNCOES.Scripts.Mantem_AbaAtiva(Page, "conversas-tab"); hddidArquivoConversa.Value = "0"; if (bs_Conversas.Count > 0) bs_Conversas.RemoveAt(bs_Conversas.Count - 1);
        }

        protected void cmdConversas_Novo_Click(object sender, EventArgs e)
        {
            div_dtProxConv.Visible = false;
            div_statusConv.Visible = false;
            txtConversas_dtResolucaoConversa.ReadOnly = true;
            ExibeCombos(true);
            txtConversas_sDscConversa.ReadOnly = false;

            DIV_ASSUNTO.Visible = false;
            DIV_CONVERSA.Visible = true;
            DIV_NOVOASSUNTO.Visible = false;
            Div_Inclusao.Visible = true;
            LimpaCampos_Conversas();

            rptHistorico.DataSource = null;
            rptHistorico.DataBind();

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "conversas-tab");
        }

        protected void cmdConversas_Salvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados_Conversas())
            {
                try
                {
                    var linha = new cls_Conversas();
                    cls_Arquivos arquivo = new cls_Arquivos();
                    if (bs_Conversas.Count != 0)
                    {
                    linha = bs_Conversas[0];

                    if (linha.sNomeArquivo != "" && linha.idArquivo == 0)
                    {
                        cls_Arquivos Arquivo = new cls_Arquivos();

                        Arquivo.idTipoArquivo = 9999;
                        Arquivo.idObjeto = int.Parse(hddConversa_idRegistroConvesa.Value);
                        Arquivo.sNomeArquivo = linha.sNomeArquivo;
                        Arquivo.sDscArquivo = linha.sObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        Arquivo.vbArquivo = linha.objArquivo;

                        linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));
                         hddidArquivoConversa.Value = linha.idArquivo.ToString();
                    }
                    }

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR-CONVERSA" },
                        { "@idRegistroConvesa", hddConversa_idRegistroConvesa.Value },
                        { "@idColaborador", hddidColaborador.Value },
                        { "@sDscConversa", txtConversas_sDscConversa.Text },
                        { "@idAcao", ddlConversas_idAcao.SelectedValue },
                        { "@idArquivo", linha.idArquivo.ToString() },
                        { "@idMeio", ddlConversas_idMeio.SelectedValue },
                        { "@idTipoEvento", ddlConversas_idTipoEvento.SelectedValue },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                    }; 

                    if (isSalvarStatus == false)
                        vParametros.Add("@sObservacaoConversa", txtConversas_sObservacaoConversa.Text);
                    else  if (linha.sNomeArquivo != "" && linha.idArquivo == 0) vParametros.Add("@sObservacaoConversa", "[Arquivo Enviado]");


                    // Edição
                    if (hddConversa_idRegistroConvesa.Value != "" && isSalvarStatus == true)
                    {
                        if (ddlConversas_idStatusConversa.SelectedValue == "3" || ddlConversas_idStatusConversa.SelectedValue == "5" || ddlConversas_idStatusConversa.SelectedValue == "6")
                        {
                            vParametros.Add("@dtResolucaoConversa", DateTime.Now.ToString());
                            txtConversas_dtResolucaoConversa.Visible = true;
                            txtConversas_dtResolucaoConversa.ReadOnly = true;
                        }
                        else
                            vParametros.Add("@dtResolucaoConversa", txtConversas_dtResolucaoConversa.Text);

                        if (idExclusao == 7)
                            vParametros.Add("@idStatusConversa", idExclusao.ToString());
                        else
                            vParametros.Add("@idStatusConversa", ddlConversas_idStatusConversa.SelectedValue);

                        if (txtConversas_dtProximaConversa.Text == "")
                            vParametros.Add("@dtProximaConversa", txtConversas_dtProximaConversa.Text);
                        else
                        {
                            DateTime dt = DateTime.Parse(txtConversas_dtProximaConversa.Text.ToString(), CultureInfo.InvariantCulture);
                            vParametros.Add("@dtProximaConversa", dt.ToString());
                            vParametros.Add("@sObservacaoConversa", $"data da próxima conversa marcada para {dt}");
                        }
                    }

                    DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        pesquisarConversas(hddidColaborador.Value, true);
                        if (idExclusao == 7)
                        {
                            MensagemPaginaConversas2.MostraMensagem_Sucesso("Excluído com Sucesso!");
                            DIV_ASSUNTO.Visible = true;
                            DIV_CONVERSA.Visible = false;
                            DIV_NOVOASSUNTO.Visible = true;
                            ddlConversas_idStatusConversa.Visible = false;
                            Div_resolucao.Visible = false;
                            txtConversas_dtResolucaoConversa.Visible = false;
                            btnSalvarStatus.Visible = false;
                            btnArquivoChat.Visible = false;
                            btnExcluir.Visible = false;
                        }
                        else
                            MensagemPaginaConversas.MostraMensagem_Sucesso("Mensagem Enviada!");

                        if (hddConversa_idRegistroConvesa.Value != "")
                            popularHistorico(hddConversa_idRegistroConvesa.Value);
                        else
                            pesquisarConversas(hddidColaborador.Value, false);

                        txtConversas_sObservacaoConversa.Text = "";
                    }
                    else
                        throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPaginaConversas.MostraMensagem_Erro(ex.Message);
                }

                RegistraScript("");
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "conversas-tab");
        }

        protected void cmdConversas_Excluir_Click(object sender, EventArgs e)
        {
            isSalvarStatus = true;
            idExclusao = 7;
            txtConversas_dtProximaConversa.Text = "";
            cmdConversas_Salvar_Click(sender, e);
        }

        protected void cmdConversas_SalvarDtStt_Click(object sender, EventArgs e) { 
            isSalvarStatus = true; 
            cmdConversas_Salvar_Click(sender, e);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "conversas-tab");
        }

        void LimpaCampos_Conversas()
        {
         
            txtConversas_sDscConversa.Text = "";
            txtConversas_dtResolucaoConversa.Text = "";
            txtConversas_sObservacaoConversa.Text = "";
            txtConversas_dtProximaConversa.Text = "";
            lblAssunto.Text = "";
            ddlConversas_idAcao.SelectedValue = "0";
            ddlConversas_idMeio.SelectedValue = "0";
            ddlConversas_idStatusConversa.SelectedValue = "0";
            ddlConversas_idTipoEvento.SelectedValue = "0";
            hddConversa_idRegistroConvesa.Value = "";
            hddidArquivoConversa.Value = "";
            if (bs_Conversas.Count > 0) bs_Conversas.RemoveAt(bs_Conversas.Count - 1);
            cmdConversas.Visible = true;
        }

        private bool ValidarDados_Conversas()
        {
            if (string.IsNullOrEmpty(txtConversas_sDscConversa.Text))
            {
                string sMensagemErro = "Informe um Assunto!";
                MensagemPaginaConversas.MostraMensagem_Erro(sMensagemErro);

                return false;
            }

            if (string.IsNullOrEmpty(txtConversas_sObservacaoConversa.Text) && isSalvarStatus != true)
            {
                string sMensagemErro = "É Preciso Digitar uma Mensagem!";
                MensagemPaginaConversas.MostraMensagem_Erro(sMensagemErro);

                return false;
            }

            if (string.IsNullOrEmpty(ddlConversas_idTipoEvento.SelectedValue) || ddlConversas_idTipoEvento.SelectedValue == "0")
            {
                string sMensagemErro = "Campo Tipo de Solicitação é obrigatório!";
                MensagemPaginaConversas.MostraMensagem_Erro(sMensagemErro);

                return false;
            }

            if (string.IsNullOrEmpty(ddlConversas_idMeio.SelectedValue) || ddlConversas_idMeio.SelectedValue == "0")
            {
                string sMensagemErro = "Campo Meio é obrigatório!";
                MensagemPaginaConversas.MostraMensagem_Erro(sMensagemErro);

                return false;
            }

            if (txtConversas_sDscConversa.MaxLength > 200)
            {
                string sMensagemErro = "Campo Assunto tem Limite de 200 Caracteres";
                MensagemPaginaConversas.MostraMensagem_Erro(sMensagemErro);

                return false;
            }

            if (DateTime.TryParse(txtConversas_dtProximaConversa.Text, out DateTime dataProximaConversa))
            {
                if (dataProximaConversa < DateTime.Now)
                {
                    string sMensagemErro = "Data Inválida!";
                    MensagemPaginaConversas.MostraMensagem_Erro(sMensagemErro);
                    return false;
                }
            }

            return true;
        }

        protected string GetLiClass(object idUsuarioItem)
        {
            var logado = IDENTITY.Variaveis.idUsuario();
            string idUsuarioAtualizacao = idUsuarioItem.ToString();

            if (logado != idUsuarioAtualizacao)
                return "right";
            else
                return "left";
        }

        protected string GetLiImagemColaborador(object vbImagem)
        {
            string sImagem = "http://placehold.it/50/55C1E7/fff";

            try
            {
                sImagem = Convert.ToBase64String((byte[])vbImagem);
                sImagem = "data:image/jpg;base64," + sImagem;
            }
            catch { }

            return sImagem;
        }

        protected string GetLiClassSmall(object idUsuarioItem)
        {
            var logado = IDENTITY.Variaveis.idUsuario();
            string idUsuarioAtualizacao = idUsuarioItem.ToString();

            if (logado != idUsuarioAtualizacao)
                return "pull-right";
            else
                return "pull-left";
        }

        protected string GetLiClassStrong(object idUsuarioItem)
        {
            var logado = IDENTITY.Variaveis.idUsuario();
            string idUsuarioAtualizacao = idUsuarioItem.ToString();

            if (logado != idUsuarioAtualizacao)
                return "pull-right";

            return string.Empty;
        }

        #endregion

        #region | Aba Avaliação

        void Popular_dtgAvaliacao(DataSet dsPesquisa)
        {
            BtnAvaliacao.Visible = false;

            foreach (DataRow row in dsPesquisa.Tables[nTabela_Avaliacao].Rows)
            {
                cls_Avaliacao objItem = new cls_Avaliacao();

                objItem.idLinha = bs_Avaliacao.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";
                objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                objItem.idRegistroAvaliacao = Convert.ToInt32(row["idRegistroAvaliacao"].ToString());
                objItem.idTipoAvaliacao = Convert.ToInt32(row["idTipoAvaliacao"].ToString());
                objItem.sDscAvaliacao = row["sDscAvaliacao"].ToString();
                objItem.dtAvaliacao = row["dtAvaliacao"].ToString();
                objItem.sObservacaoAvaliacao = row["sObservacaoAvaliacao"].ToString();
                objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();

                bs_Avaliacao.Add(objItem);
            }

            dtgAvaliacao_DataBind();
            Avaliacao_LimpaCampos();
        }

        protected void cmdAvaliacao_Incluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_Avaliacao(ref sMensagem))
            {
                cls_Avaliacao objItem = new cls_Avaliacao();

                string[] vidColaborador = hddidColaborador.Value.Split(',');
                string idColaborador = vidColaborador[0].ToString();

                objItem.idLinha = bs_Avaliacao.Count() + 1;
                objItem.sFuncao = "SALVAR_AVALIACAO";
                objItem.idColaborador = Convert.ToInt32(idColaborador);
                objItem.idTipoAvaliacao = Convert.ToInt32(ddlidTipoAvaliacao.SelectedValue.ToString());
                objItem.sDscAvaliacao = ddlidTipoAvaliacao.SelectedItem.ToString();
                objItem.dtAvaliacao = txtdtAvaliacao.Text;
                objItem.sObservacaoAvaliacao = txtsObservacaoAvaliacao.Text.ToString();
                objItem.sNomeArquivo = "";

                bs_Avaliacao.Add(objItem);
                dtgAvaliacao_DataBind();
                Avaliacao_LimpaCampos();
            }
            else
                MensagemPagina_Avaliacao.MostraMensagem_Erro(sMensagem, false);

            RegistraScript("$('[id$=ddlidTipoAvaliacao]').focus();");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "avaliacao-tab");
        }

        bool Salvar_Avaliacao(string idColaborador)
        {
            bool bRetorno = false;

            try
            {
                foreach (var linha in bs_Avaliacao)
                {
                    if (linha.sNomeArquivo != "" && linha.idArquivo == 0)
                    {
                        cls_Arquivos Arquivo = new cls_Arquivos();

                        Arquivo.idTipoArquivo = 9999;
                        Arquivo.idObjeto = linha.idRegistroAvaliacao;
                        Arquivo.sNomeArquivo = linha.sNomeArquivo;
                        Arquivo.sDscArquivo = linha.sObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        Arquivo.vbArquivo = linha.objArquivo;
                        Arquivo.dtExpiracaoDoc = linha.dtAvaliacao;

                        linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (linha.sFuncao == "SEM ALTERAÇÃO")
                            linha.sFuncao = "SALVAR_AVALIACAO";
                    }

                    if (linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<string, string> vParametroItens_Incluir = new Dictionary<string, string>
                        {
                            ["@sFuncao"] = linha.sFuncao,
                            ["@idRegistro"] = linha.idRegistroAvaliacao.ToString(),
                            ["@idColaborador"] = idColaborador,
                            ["@idTipoAvaliacao"] = linha.idTipoAvaliacao.ToString(),
                            ["@sDscAvaliacao"] = linha.sDscAvaliacao.ToString(),
                            ["@dtAvaliacao"] = linha.dtAvaliacao,
                            ["@sObservacaoAvaliacao"] = linha.sObservacaoAvaliacao,
                            ["@idArquivo"] = linha.idArquivo.ToString()
                        };
                        BD.ExecutarDataSet(sProcedure, vParametroItens_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Bloco Avaliacao - Erro: " + ex.Message);
            }

            return bRetorno;
        }

        protected void BtnAvaliacao_Click(object sender, EventArgs e)
        {
            int idLinha = Convert.ToInt32(hddAvalicao_idLinha.Value);
            int index = bs_Avaliacao.FindIndex(x => x.idLinha.Equals(idLinha));

            bs_Avaliacao[index].idTipoAvaliacao = Convert.ToInt32(ddlidTipoAvaliacao.SelectedValue);
            bs_Avaliacao[index].dtAvaliacao = txtdtAvaliacao.Text;
            bs_Avaliacao[index].sObservacaoAvaliacao = txtsObservacaoAvaliacao.Text;

            bs_Avaliacao[index].sFuncao = "SALVAR_AVALIACAO";

            dtgAvaliacao_DataBind();
            Avaliacao_LimpaCampos();
            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "avaliacao-tab");
        }

        void Avaliacao_LimpaCampos()
        {
            ddlidTipoAvaliacao.SelectedValue = "0";
            txtdtAvaliacao.Text = "";
            txtsObservacaoAvaliacao.Text = "";
            hddAvalicao_idLinha.Value = "";

            BtnAvaliacao.Visible = false;
            cmdAvaliacao.Visible = true;
        }

        private bool ValidarDados_Avaliacao(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlidTipoAvaliacao.SelectedValue == "0")
                sMensagemErro += "Selecione um Tipo!";

            if (txtdtAvaliacao.Text.Length < 6)
            {
                if (sMensagemErro != "")
                    sMensagemErro = sMensagemErro + "</br>";

                sMensagemErro += "Data inválida!";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina_Avaliacao.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void dtgAvaliacao_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtgAvaliacao.Rows[e.RowIndex].Cells[0].Text);
            bs_Avaliacao[bs_Avaliacao.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_AVALIACAO";
            dtgAvaliacao_DataBind();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "avaliacao-tab");
        }

        protected void dtgAvaliacao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();
                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkAvaliacao_Download" && sNomeArquivo == "") || (lnk.ID == "lnkAvaliacao_UpLoad" && sNomeArquivo != ""))
                        lnk.Visible = false;
                }
            }
            GRID.EsconderColunas(e, 0, 1);
        }

        protected void dtgAvaliacao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "Avaliacao";
            hddAvalicao_idLinha.Value = idLinha.ToString();

            if (e.CommandName == "Upload_Arquivo")
                AbrirModal_EnvioArquivo(eBloco.Avaliacao, idLinha, bs_Avaliacao[bs_Avaliacao.FindIndex(x => x.idLinha.Equals(idLinha))].sDscAvaliacao);
            else if (e.CommandName == "Download_Arquivo")
                Efetuar_Download_Arquivo(eBloco.Avaliacao, idLinha);
            else if (e.CommandName == "Editar")
            {
                cmdAvaliacao.Visible = false;
                BtnAvaliacao.Visible = true;

                var Avaliacao = bs_Avaliacao[bs_Avaliacao.FindIndex(x => x.idLinha.Equals(idLinha))];

                ddlidTipoAvaliacao.SelectedValue = Avaliacao.idTipoAvaliacao.ToString();
                txtdtAvaliacao.Text = Avaliacao.dtAvaliacao;
                txtsObservacaoAvaliacao.Text = Avaliacao.sObservacaoAvaliacao;

                ddlidTipoAvaliacao.Focus();
            }

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "avaliacao-tab");
        }

        void dtgAvaliacao_DataBind()
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_dtgAvaliacao_DataBind", GRID.DataBindComScriptData(dtgAvaliacao, bs_Avaliacao.Where(c => c.sFuncao.ToString() != "EXCLUIR_AVALIACAO").ToList(), 1, "asc", "false", "''"), true);
                RegistraScript("");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Avaliacao: " + ex.Message);
            }
        }

        #endregion

        #region | Aba Arquivo Morto

        void Popular_gvArquivoMorto(DataSet dsPesquisa)
        {
            if (dsPesquisa.Tables[nTabela_ArquivoMorto].Rows.Count > 0)
            {
                bs_ArquivoMorto.Clear();
                foreach (DataRow row in dsPesquisa.Tables[nTabela_ArquivoMorto].Rows)
                {
                    cls_ArquivoMorto objItem = new cls_ArquivoMorto();

                    objItem.idLinha = bs_ArquivoMorto.Count() + 1;
                    objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                    objItem.idUsuarioAtualizacao = Convert.ToInt32(row["idUsuarioAtualizacao"].ToString());
                    objItem.sDescricao = row["sDescricao"].ToString();
                    objItem.dtExclusao = row["dtExclusao"].ToString();
                    objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                    objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                    objItem.sDscUsuario = row["sDscUsuario"].ToString();

                    bs_ArquivoMorto.Add(objItem);
                }
            }

            gvArquivoMorto_DataBind();
        }

        private void gvArquivoMorto_DataBind()
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_gv_ArquivoMorto_DataBind", GRID.DataBindComScriptData(gv_ArquivoMorto, bs_ArquivoMorto, new int[1] { 3 }, "desc", "false", "''"), true);
                RegistraScript("");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Itens: " + ex.Message);
            }
        }

        protected void gv_ArquivoMorto_RowDataBound(object sender, GridViewRowEventArgs e) => GRID.EsconderColunas(e, 0);

        protected void gv_ArquivoMorto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "ArquivoMorto";

            if (e.CommandName == "Download_Arquivo")
                Efetuar_Download_Arquivo(eBloco.ArquivoMorto, idLinha);

            RegistraScript("");
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "arquivoMorto-tab");
        }

        #endregion

        #region | Eventos

        protected void txtdtInicioContrato_TextChanged(object sender, EventArgs e)
        {
            if (DateTime.TryParse(txtdtInicioContrato.Text, out DateTime dataInicioContrato))
            {
                DateTime dataPrevistaPlanoSaude = dataInicioContrato.AddMonths(nTempoEntradaPlanoSaude);
                txtdtPrevPlanoSaude.Text = dataPrevistaPlanoSaude.ToString("dd/MM/yyyy");
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador");
        }

        protected void ddlidTipoContrato_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidTipoContrato.SelectedItem.ToString() == "PJ")
            {
                lblsCPF.Text = "CPF";
                lblsFuncao.Text = "Descrição Atividade";

                DIV_sNIRE.Visible = true;
                DIV_dtRegistro.Visible = true;
                DIV_sIE.Visible = true;
                DIV_sIM.Visible = true;

                DIV_sCNPJ.Visible = true;

                Div_ddlsTipoCTPS.Visible = false;
                DIV_sPIS.Visible = false;
                DIV7.Visible = false;

                ddlsTipoCTPS.SelectedValue = "0";
                txtsPIS.Text = "";

                txtsCNPJ.Text = ViewState["sCNPJ"]?.ToString()?.ToString() ?? string.Empty;
                txtsNIRE.Text = ViewState["sNIRE"]?.ToString()?.ToString() ?? string.Empty;
                txtdtRegistro.Text = ViewState["dtRegistro"]?.ToString() ?? string.Empty;
                txtsIE.Text = ViewState["sIE"]?.ToString() ?? string.Empty;
                txtsIM.Text = ViewState["sIM"]?.ToString() ?? string.Empty;
            }
            else
            {
                lblsCPF.Text = "CPF";
                lblsFuncao.Text = "Função Carteira";

                DIV_sCNPJ.Visible = false;
                DIV_sNIRE.Visible = false;
                DIV_dtRegistro.Visible = false;
                DIV_sIE.Visible = false;
                DIV_sIM.Visible = false;

                ddlidFuncao.SelectedValue = "0";
                txtsNIRE.Text = "";
                txtdtRegistro.Text = "";
                txtsIE.Text = "";
                txtsIM.Text = "";

                DIV_sPIS.Visible = true;
                DIV7.Visible = true;
            }

            PopularCombo_Funcao();
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador");
        }

        protected void ddlidFuncao_SelectedIndexChanged(object sender, EventArgs e) { PopularDadosFuncao(Convert.ToInt32(ddlidFuncao.SelectedValue)); FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador"); }

        protected void ddlsLimiteContrato_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlsLimiteContrato.SelectedValue == "N")
            {
                DIV_sTerminoContrato.Visible = false;
                div_ddlsLimiteContrato.Attributes["class"] = "col-lg-4";
            }
            else
            {
                DIV_sTerminoContrato.Visible = true;
                div_ddlsLimiteContrato.Attributes["class"] = "col-lg-2";
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador");
        }

        protected void ddlsSituacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlsSituacao.SelectedValue == "S")
                DIV_dtDesligamento.Visible = false;
            else
                DIV_dtDesligamento.Visible = true;

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador");
        }

        protected void ddlsDependentesConvenio_SelectedIndexChanged(object sender, EventArgs e)
        {
            Div_Dependentes.Visible = false;

            if (ddlsDependentesConvenio.SelectedValue == "S")
                Div_Dependentes.Visible = true;

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador");
        }

        protected void ddlsPensaoDependente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlsPensaoDependente.SelectedValue == "N")
                Div_ValorPensaoDependente.Visible = false;
            else
                Div_ValorPensaoDependente.Visible = true;

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador");
        }

        protected void ddlidDepartamento_SelectedIndexChanged(object sender, EventArgs e) { PopularCombo_Funcao(); FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador"); }

        protected void txtEndereco_sCEP_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sCEP", txtEndereco_sCEP.Text.Replace("-", "") }
                };
                DataSet dsCEP = BD.ExecutarDataSet("sp_Consulta_CEP", vParametros);

                if (BD.ValidarDataSet(dsCEP))
                {
                    txtEndereco_sLogradouro.Text = RETORNO.DATASET(dsCEP, 0, "sLogradouro");
                    txtEndereco_sBairro.Text = RETORNO.DATASET(dsCEP, 0, "sBairro");
                    ddlEndereco_sUF.SelectedValue = RETORNO.DATASET(dsCEP, 0, "sUF");
                    txtEndereco_sCidade.Text = RETORNO.DATASET(dsCEP, 0, "sCidade");

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Endereco_Focus", "$('[id$=txtEndereco_sNumero]').focus();", true);
                }
                else
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Endereco_Focus", "$('[id$=txtEndereco_sLogradouro]').focus();", true);
            }
            catch
            {
                txtEndereco_sLogradouro.Text = "";
                txtEndereco_sBairro.Text = "";
                ddlEndereco_sUF.SelectedValue = "";
                txtEndereco_sCidade.Text = "";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "Endereco_Focus", "$('[id$=txtEndereco_sLogradouro]').focus();", true);
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador");
        }

        protected void ddlidEmpresa_SelectedIndexChanged(object sender, EventArgs e) { PopularCombo_Funcao(); FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_Colaborador"); }




        protected void cmdConsulta_EPIsAntigos_Click(object sender, EventArgs e) { div_EPIs_Antigos.Visible = true; dtgEPI_DataBind(false); FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab"); }

        protected void cmdFechar_EPIsAntigos_Click(object sender, EventArgs e) { div_EPIs_Antigos.Visible = false; dtgEPI_DataBind(true); FUNCOES.Scripts.Mantem_AbaAtiva(Page, "SSTT-tab"); }


        #endregion

        #region | Script

        void RegistraScript(string sFuncao)
        {
            StringBuilder sb = new StringBuilder();

            // Mensagens de Confirmação
            sb.Append("$v192(function() {");

            sb.Append("$v192(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=Principal_cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Editar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Editar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=Principal_cmdEditar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Editar').dialog('open');");
            sb.Append("});");

            sb.Append(@"var seletorDatePicker = `[id*=txtdtElogio], [id*=txtdtRecebimentoEquipamento], [id*=txtdtDevolucaoEquipamento], [id*=txtdtNascDependente], [id*=txtdtNascimento], 
                        [id*=txtsTerminoContrato], [id*=txtdtDesligamento], [id*=dtInicioEvento], [id*=dtRetornoEvento], [id*=txtdtEmissaoNR], [id*=txtdtVencimentoNR], [id*=txtsdtExpiracaoDoc], [id*=txtdtRecebimentoEPI], [id*=txtdtVenvimentoEPI],
                        [id*=txtdtVencCNH], [id*=Roupas_txtdtEntrega], [id*=txtdtEvento], [id*=txtdtInicioAfastamento], [id*=txtdtRetornoAfastamento], [id*=txtConversas_dtResolucaoConversa], [id*=txtConversas_dtEventoConversa], [id*=txtVT_dtVT],
                        [id*=txtConversas_dtRepostaConversa], [id*=txtdtAvaliacao], [id*=txtdtInicioAtestado_Pesquisa], [id*=txtdtFinalAtestado_Pesquisa], [id*=txtdtdtInicioAfastamento_Pesquisa], [id*=txtdtdtFinalAfastamento_Pesquisa],
                        [id*=txtdtPrevPlanoSaude]`;");
            sb.Append(@"$(seletorDatePicker).mask('99/99/9999');");
            sb.Append("$(seletorDatePicker).datepicker({ autoclose: true, format: 'dd/mm/yyyy', language: 'pt-BR' });");

            sb.Append("$('[id*=Roupas_txtdtQuantidade]').mask('999');");

            sb.Append("$('[id*=txtsTelCelular]').mask('(99) 99999-9999');");
            sb.Append("$('[id*=txtsTelResidencial]').mask('(99) 9999-9999');");
            sb.Append("$('[id*=txtsCelularPessoal]').mask('(99) 99999-9999');");
            sb.Append("$('[id*=txtVT_nValorVT]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorPlano]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorOcorrencia]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValor]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnHorasAtestado]').mask('99,99');");
            sb.Append("$('[id*=txtsCNPJ]').mask('99.999.999/9999-99');");
            sb.Append("$('[id*=txtsCPF]').mask('999.999.999-99');");
            sb.Append("$('[id*=txtsHorasAtestado]').mask('99999999999');");
            sb.Append("$('[id*=txtsCNH]').mask('99999999999');");
            sb.Append("$('[id*=txtsPIS]').mask('999.99999.99-9');");

            sb.Append("$('[id*=txtEndereco_sCEP]').mask('99999999');");
            sb.Append("$('[id*=txtdtInclusaoConvenio]').mask('00/00/0000');");
            sb.Append("$('[id*=txtdtCarenciaConvenio]').mask('00/00/0000');");

            //------------------Higor Maestrello 18-06-2024-----------------------------
            sb.Append("$('.Todos').change(function(){ var isChecked = $(this).find('input').is(':checked'); console.log(isChecked); $('.Individual').find('input').prop('checked', isChecked); });");

            sb.Append("$v192('.btDownload').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("var hddChecked = [];");
            sb.Append("$v192('.Individual').each(function() {");
            sb.Append("var checkboxId = $v192(this).closest('tr').attr('id');");
            sb.Append("if ($v192(this).find('input').is(':checked')) {");
            sb.Append("hddChecked.push(checkboxId);");
            sb.Append("}");
            sb.Append("});");
            sb.Append("if (hddChecked.length > 0) {");
            sb.Append("$('[id*=hddChecked]').val(hddChecked.join(','));");
            sb.Append("__doPostBack('dialog_CheckArquivo', '');");
            sb.Append("} else {");
            sb.Append("$('[id*=hddChecked]').val('');");
            sb.Append("__doPostBack('dialog_CheckArquivo', '');");
            sb.Append("}");
            sb.Append("});");
            //-----------------------------------------------------------------------------                    

            if (sFuncao != "")
                sb.Append(sFuncao);

            sb.Append("});");

            // Script para exibir a Composição dos Itens no método por Linhas
            sb.AppendLine("$('.composicaoLinha').click(function(e) {");
            sb.AppendLine("     e.preventDefault();");
            sb.AppendLine("     var icon = $(this);");
            sb.AppendLine("     var divId = $(this).data('div-id');");
            sb.AppendLine("     if (icon.attr('id') == 'btnToggle_Todos') {");
            sb.AppendLine("         var expandir = icon.hasClass('fa-plus');");
            sb.AppendLine("         icon.toggleClass('fa-minus fa-plus');");
            sb.AppendLine("         $('.composicaoLinha').not(this).each(function() {");
            sb.AppendLine("             var otherIcon = $(this);");
            sb.AppendLine("             var otherDivId = otherIcon.data('div-id');");
            sb.AppendLine("             if (expandir) {");
            sb.AppendLine("                 $('#' + otherDivId).show('slow');");
            sb.AppendLine("                 otherIcon.removeClass('fa fa-plus').addClass('fa fa-minus');");
            sb.AppendLine("             } else {");
            sb.AppendLine("                 $('#' + otherDivId).hide('slow');");
            sb.AppendLine("                 otherIcon.removeClass('fa fa-minus').addClass('fa fa-plus');");
            sb.AppendLine("             }");
            sb.AppendLine("         });");
            sb.AppendLine("     } else {");
            sb.AppendLine("         var current = $('#' + divId).css('display');");
            sb.AppendLine("         if (current == 'none') {");
            sb.AppendLine("             $('#' + divId).show('slow');");
            sb.AppendLine("             icon.removeClass('fa fa-plus').addClass('fa fa-minus');");
            sb.AppendLine("         } else {");
            sb.AppendLine("             $('#' + divId).hide('slow');");
            sb.AppendLine("             icon.removeClass('fa fa-minus').addClass('fa fa-plus');");
            sb.AppendLine("         }");
            sb.AppendLine("     }");
            sb.AppendLine("});");



            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }

        #endregion

        #region | Arquivos

        void AbrirModal_EnvioArquivo(eBloco bloco, int idLinha, string sTituloModal)
        {
            hddIdLinha.Value = idLinha.ToString();
            hddsBloco.Value = bloco.ToString();

            if (string.IsNullOrEmpty(sTituloModal))
                sTituloModal = bloco.ToString();

            if (bloco == eBloco.NR)
                div_observacaoEnvioArquivo.Visible = false;
            else
                div_observacaoEnvioArquivo.Visible = true;

            lblEnviarArquivos_Titulo.Text = sTituloModal;
            txtEnviarArquivo_sDscArquivo.Text = "";

            UpdTitulo_UploadArquivos_Modal.Update();
            UpdObs_UploadArquivos_Modal.Update();

             FUNCOES.Scripts.AbrirModal(Page, "UploadArquivos_Modal");
        }

        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {
            int index = 0;
            int.TryParse(hddIdLinha.Value, out int idLinha);
            eBloco bloco = (eBloco)Enum.Parse(typeof(eBloco), hddsBloco.Value);
            string sJSExecutar = "";

            if (fu_EnviarArquivo.HasFile)
            {
                Byte[] lObjArquivo = null;

                try
                {
                    Arquivo objArquivo = new Arquivo();
                    lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(fu_EnviarArquivo.FileName, fu_EnviarArquivo.PostedFile.InputStream);

                    switch (bloco)
                    {
                        case eBloco.NR:
                            index = bs_NR_Itens.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_NR_Itens[index].idArquivo = 0;
                            bs_NR_Itens[index].sNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_NR_Itens[index].objArquivo = lObjArquivo;
                            bs_NR_Itens[index].sDscArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            dtgNR_DataBind();
                            sJSExecutar = " $('#SSTT-tab').tab('show');";
                            break;

                        case eBloco.VT:
                            index = bs_VT.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_VT[index].idArquivo = 0;
                            bs_VT[index].sNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_VT[index].objArquivo = lObjArquivo;
                            bs_VT[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            dtgVT_DataBind();
                            sJSExecutar = " $('#beneficios-tab').tab('show');";
                            break;

                        case eBloco.Conversas:
                            //index = bs_Conversas.FindIndex(x => x.idLinha.Equals(idLinha));
                            index = idLinha;
                            if (bs_Conversas.Count == 0)
                            {
                              bs_Conversas.Add(new cls_Conversas());
                            }

                            bs_Conversas[0].idRegistroConvesa = idLinha;
                            bs_Conversas[0].idArquivo = 0;
                            bs_Conversas[0].sNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_Conversas[0].objArquivo = lObjArquivo;
                            bs_Conversas[0].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            sJSExecutar = " $('#conversas-tab').tab('show');";
                            break;

                        case eBloco.Roupas:
                            index = bs_Roupas.FindIndex(x => x.IdLinha.Equals(idLinha));
                            bs_Roupas[index].IdArquivo = 0;
                            bs_Roupas[index].SNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_Roupas[index].ObjArquivo = lObjArquivo;
                            bs_Roupas[index].SObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            Roupas_GV_DataBind();
                            sJSExecutar = " $('#SSTT-tab').tab('show');";
                            break;

                        case eBloco.Atestado:
                            index = bs_Ausencia.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_Ausencia[index].idArquivo = 0;
                            bs_Ausencia[index].sNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_Ausencia[index].objArquivo = lObjArquivo;
                            bs_Ausencia[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            dtg_Ausencia_DataBind();
                            sJSExecutar = " $('#Evento-tab').tab('show');";
                            break;

                        case eBloco.Credito:
                            index = bs_Credito.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_Credito[index].idArquivo = 0;
                            bs_Credito[index].sNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_Credito[index].objArquivo = lObjArquivo;
                            bs_Credito[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            dtgCredito_DataBind();
                            sJSExecutar = " $('#Evento-tab').tab('show');";
                            break;

                        case eBloco.Equipamento:
                            index = bs_Equipamentos.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_Equipamentos[index].idArquivo = 0;
                            bs_Equipamentos[index].sNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_Equipamentos[index].objArquivo = lObjArquivo;
                            bs_Equipamentos[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            dtgEquipamentos_DataBind();
                            sJSExecutar = " $('#beneficios-tab').tab('show');";
                            break;

                        case eBloco.Ocorrencias:
                            index = bs_Ocorrencias.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_Ocorrencias[index].idArquivo = 0;
                            bs_Ocorrencias[index].sNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_Ocorrencias[index].objArquivo = lObjArquivo;
                            bs_Ocorrencias[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            dtgOcorrencias_DataBind();
                            sJSExecutar = " $('#Evento-tab').tab('show');";
                            break;

                        case eBloco.Avaliacao:
                            index = bs_Avaliacao.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_Avaliacao[index].idArquivo = 0;
                            bs_Avaliacao[index].sNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_Avaliacao[index].objArquivo = lObjArquivo;
                            bs_Avaliacao[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            dtgAvaliacao_DataBind();
                            sJSExecutar = " $('#avaliacao-tab').tab('show');";
                            break;

                        case eBloco.Dependentes:
                            index = bs_Dependentes.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_Dependentes[index].idArquivo = 0;
                            bs_Dependentes[index].sNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_Dependentes[index].objArquivo = lObjArquivo;
                            bs_Dependentes[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            dtgDependentes_DataBind();
                            break;

                        case eBloco.PlanoSaude:

                            cls_Arquivos Arquivo = new cls_Arquivos();
                            Arquivo.idTipoArquivo = 9999;
                            Arquivo.idObjeto = Convert.ToInt32(hddidColaborador.Value);
                            Arquivo.sNomeArquivo = fu_EnviarArquivo.FileName;
                            Arquivo.sDscArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            Arquivo.sObservacao = "";
                            Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                            Arquivo.vbArquivo = lObjArquivo;
                            Arquivo.dtExpiracaoDoc = "";
                            Arquivo.dtRegistroDoc = DateTime.Today.ToString();

                            hddidArquivoPlanoSaude.Value = BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo");
                            DataTable dt = BD.ExecutarDataTable(sProcedure, new Dictionary<string, string> { { "@sFuncao", "SALVA_ARQUIVO_PlanoSaude" }, { "@idArquivoPlanoSaude", hddidArquivoPlanoSaude.Value }, { "@idColaborador", hddidColaborador.Value } }, false);
                            MensagemPagina_PlanoSaude.MostraMensagem_Sucesso("Arquivo Inserido com sucesso!");
                            sJSExecutar = " $('#beneficios-tab').tab('show');";
                            break;
                    }
                }
                catch { return; }
            }

            pesquisarConversas(hddidColaborador.Value, false);
            FUNCOES.Scripts.RemoverBackdrop_Modal(Page);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ModalUpload_MantemAbe_Ativa", sJSExecutar, true);
            }

        void Efetuar_Download_Arquivo(eBloco bloco, int idLinha, bool bEntregaEPI = false)
        {
            int index;
            int idArquivo = 0;
            string sNomeArquivo = "";
            byte[] bytes = null;
            string urlAtualPagina = Request.UrlReferrer.ToString().Replace(Request.RawUrl, "/Download/");

            if (bEntregaEPI) { idArquivo = idLinha; goto baixaArquivo; }

            switch (bloco)
            {
                case eBloco.NR:
                    index = bs_NR_Itens.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_NR_Itens[index].sNomeArquivo;
                    bytes = bs_NR_Itens[index].objArquivo;
                    idArquivo = bs_NR_Itens[index].idArquivo;
                    break;

                case eBloco.VT:
                    index = bs_VT.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_VT[index].sNomeArquivo;
                    bytes = bs_VT[index].objArquivo;
                    idArquivo = bs_VT[index].idArquivo;
                    break;

                case eBloco.Conversas:
                    index = idLinha;
                    idArquivo = int.Parse(hddidArquivoConversa.Value);
                    FUNCOES.Scripts.Mantem_AbaAtiva(Page, "conversas-tab");

                    break;

                case eBloco.Roupas:
                    index = bs_Roupas.FindIndex(x => x.IdLinha.Equals(idLinha));
                    sNomeArquivo = bs_Roupas[index].SNomeArquivo;
                    bytes = bs_Roupas[index].ObjArquivo;
                    idArquivo = bs_Roupas[index].IdArquivo;
                    break;

                case eBloco.Atestado:
                    index = bs_Ausencia.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_Ausencia[index].sNomeArquivo;
                    bytes = bs_Ausencia[index].objArquivo;
                    idArquivo = bs_Ausencia[index].idArquivo;
                    break;

                case eBloco.EPI:
                    index = bs_EPI_Itens.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_EPI_Itens[index].sNomeArquivo;
                    bytes = bs_EPI_Itens[index].objArquivo;
                    idArquivo = bs_EPI_Itens[index].idArquivo;
                    break;

                case eBloco.Evento:
                    index = bs_Evento.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_Evento[index].sNomeArquivo;
                    bytes = bs_Evento[index].objArquivo;
                    idArquivo = bs_Evento[index].idArquivo;
                    break;

                case eBloco.Equipamento:
                    index = bs_Equipamentos.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_Equipamentos[index].sNomeArquivo;
                    bytes = bs_Equipamentos[index].objArquivo;
                    idArquivo = bs_Equipamentos[index].idArquivo;
                    break;

                case eBloco.Ocorrencias:
                    index = bs_Ocorrencias.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_Ocorrencias[index].sNomeArquivo;
                    bytes = bs_Ocorrencias[index].objArquivo;
                    idArquivo = bs_Ocorrencias[index].idArquivo;
                    break;

                case eBloco.Dependentes:
                    index = bs_Dependentes.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_Dependentes[index].sNomeArquivo;
                    bytes = bs_Dependentes[index].objArquivo;
                    idArquivo = bs_Dependentes[index].idArquivo;
                    break;

                case eBloco.Avaliacao:
                    index = bs_Avaliacao.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_Avaliacao[index].sNomeArquivo;
                    bytes = bs_Avaliacao[index].objArquivo;
                    idArquivo = bs_Avaliacao[index].idArquivo;
                    break;

                case eBloco.ArquivoMorto:
                    index = bs_ArquivoMorto.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_ArquivoMorto[index].sNomeArquivo;
                    bytes = bs_ArquivoMorto[index].objArquivo;
                    idArquivo = bs_ArquivoMorto[index].idArquivo;
                    break;
                case eBloco.PlanoSaude:
                    idArquivo = Convert.ToInt32(hddidArquivoPlanoSaude.Value);
                    break;
            }

        baixaArquivo:
            if (bytes is null)
            {
                Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idArquivo", idArquivo.ToString() }
                };
                DataTable dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

                sNomeArquivo = dtArquivo.Rows[0]["sNomeArquivo"].ToString();
                bytes = dtArquivo.Rows[0]["vbArquivo"] as byte[];
            }
            File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);
            FUNCOES.DownloadArquivo(Page, sNomeArquivo);
        }



        #endregion

    }
}
