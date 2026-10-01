using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using VALIDACOES = TT.FrameWork.Validacoes;
using TT.FrameWork;
using System.IO;
using TT_Flow.FrameWork;
using static Permissao;
using TT_Flow.App.Controles;


namespace TT_Flow.App.Paginas.OS
{
    public partial class OrdemServico_Detalhe : System.Web.UI.Page
    {
       string sTituloPagina = "Ordem de Serviço";
       string sProcedure = "sp_Manipula_tbl_Flow_OrdemServico";
       string ClienteAnterior = "";
       //string sPrefixoDepartamentoOS = "";
               
        #region | Classes

        public List<FrameWork.cls_OSColaboradores_Itens> bs_OSColaboradores_Itens
        {

            get
            {
                if (ViewState["bs_OSColaboradores_Itens"] == null)
                {
                    ViewState["bs_OSColaboradores_Itens"] = new List<FrameWork.cls_OSColaboradores_Itens>();
                }
                return (List<FrameWork.cls_OSColaboradores_Itens>)ViewState["bs_OSColaboradores_Itens"];
            }

            set
            {
                ViewState["bs_OSColaboradores_Itens"] = value;
            }

        }

        public List<FrameWork.cls_OSEPI_Itens> bs_OSEPI_Itens
        {

            get
            {
                if (ViewState["bs_OSEPI_Itens"] == null)
                {
                    ViewState["bs_OSEPI_Itens"] = new List<FrameWork.cls_OSEPI_Itens>();
                }
                return (List<FrameWork.cls_OSEPI_Itens>)ViewState["bs_OSEPI_Itens"];
            }

            set
            {
                ViewState["bs_OSEPI_Itens"] = value;
            }

        }

        public List<FrameWork.cls_OSAcomodacao_Itens> bs_OSAcomodacao_Itens
        {

            get
            {
                if (ViewState["bs_OSAcomodacao_Itens"] == null)
                {
                    ViewState["bs_OSAcomodacao_Itens"] = new List<FrameWork.cls_OSAcomodacao_Itens>();
                }
                return (List<FrameWork.cls_OSAcomodacao_Itens>)ViewState["bs_OSAcomodacao_Itens"];
            }

            set
            {
                ViewState["bs_OSAcomodacao_Itens"] = value;
            }

        }

        public List<FrameWork.cls_OSTransporte_Itens> bs_OSTransporte_Itens
        {

            get
            {
                if (ViewState["bs_OSTransporte_Itens"] == null)
                {
                    ViewState["bs_OSTransporte_Itens"] = new List<FrameWork.cls_OSTransporte_Itens>();
                }
                return (List<FrameWork.cls_OSTransporte_Itens>)ViewState["bs_OSTransporte_Itens"];
            }

            set
            {
                ViewState["bs_OSTransporte_Itens"] = value;
            }

        }

        #endregion

        #region | Inicialização

        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (!IsPostBack)
            {
                PopularCombos();
                                           
                if (Request["id"] != null)
                {
                     FUNCOES.ValidaPermissao(Permissao.OrdemServico.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.OrdemServico.Incluir, true);
                    Pesquisar("0", true);
                }
               
                ddlTipoOrdemServico_SelectedIndexChanged(objSender, objEventArgs);
                ddlidDepartamento_SelectedIndexChanged(objSender, objEventArgs);
                ddlCliente_SelectedIndexChanged(objSender, objEventArgs);
                ddlReferencia_SelectedIndexChanged(objSender, objEventArgs);
                ddlTipoTransporte_SelectedIndexChanged(objSender, objEventArgs);
            }
            //else
            //{
            //    var requestTarget = this.Request["__EVENTTARGET"];
            //    var requestArgs = this.Request["__EVENTARGUMENT"];

            //    if (requestTarget == "funcao_SAIR")
            //    {
            //        FUNCOES.DirecionaPagina("/app/dashboard.aspx");
            //    }
            //   else if (requestTarget == "funcao_SALVAR")
            //    {
            //        Salvar_OrdemServico();
            //    }
            //    else if (requestTarget == "funcao_Editar")
            //    {
            //        Pesquisar(hddidOrdemServico.Value, true);
            //    }
            //}


        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidTipoOrdemServico, "sp_Select 'Flow_OrdemServico_Tipo'", "idTipoOrdemServico", "sDscTipoOrdemServico", false, "Selecione um Tipo de Ordem de Servico", "0");
            if (ddlidCliente.SelectedValue == "")
            {
                FUNCOES.Popula_Combo(ddlidCliente, "sp_Select 'Flow_Clientes_Pedidos'", "idCliente", "sRazaoSocial", false, "Selecione um Cliente", "0");
            }
            FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos_OS'", "idDepartamento", "sDscDepartamento", false, "Selecione um Departamento", "0");
            FUNCOES.Popula_Combo(ddlidSolicitante, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Selecione o Solicitante", "0");
            FUNCOES.Popula_Combo(ddlColaborador, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Selecione o Colaborador", "0");
            FUNCOES.Popula_Combo(ddlEPI, "sp_Select 'Flow_OrdemServico_EPI'", "idEPI", "sEPI", false, "Selecione um EPI", "0");
            FUNCOES.Popula_Combo(ddlTipoAcomodacao, "sp_Select 'Flow_OrdemServico_Acomodacao'", "idTipoAcomodacao", "sDscTipoAcomodacao", false, "Selecione um Tipo de Acomodação", "0");
            FUNCOES.Popula_Combo(ddlTipoTransporte, "sp_Select 'Flow_OrdemServico_Transporte'", "idTipoTransporte", "sDscTipoTransporte", false, "Selecione um Tipo de Transporte", "0");

            FUNCOES.Popula_Combo(lstidJornadaTrabalho, "sp_Select 'tbl_Flow_OrdemServico_JornadaTrabalho'", "idJornadaTrabalho", "sDscJornadaTrabalho", false, "Selecione Jornada", "");
            FUNCOES.Popula_Combo(lstsExigencias, "sp_Select 'tbl_Flow_OrdemServico_Exigencias'", "idExigencias", "sDscExigencias", false, "Selecione Exigencias", "");
            FUNCOES.Popula_Combo(lstidTipoAtividadeExterna, "sp_Select 'tbl_Flow_OrdemServico_Atividade_Externa'", "idAtividade", "sDscAtividade", false, "Selecione Atividade", "");
            FUNCOES.Popula_Combo(lstidDocumentosExterna, "sp_Select 'tbl_Flow_OrdemServico_Documentos_Externa'", "idDocumentos", "sDscDocumentos", false, "Selecione Documentos", "");
            FUNCOES.Popula_Combo(lstidTipoAtividadeInterna, "sp_Select 'tbl_Flow_OrdemServico_Atividade_Interna'", "idAtividade", "sDscAtividade", false, "Selecione Atividade", "");
            FUNCOES.Popula_Combo(lstidDocumentosInterna, "sp_Select 'tbl_Flow_OrdemServico_Documentos_Interna'", "idDocumentos", "sDscDocumentos", false, "Selecione Documentos", "");

        }
     
        void LimpaCampos()
        {
            txtidOrdemServico.Text = "Nova";
            txtsNumeroOS.Text = "";
            ddlidTipoOrdemServico.Attributes.Add("enabled", "enabled");
            ddlidTipoOrdemServico.SelectedValue = "0";
            txtsCentroCusto.Text = "";
            txtdtOrdemServico.Text = "";
            ddlidCliente.SelectedValue = "0";
            ddlidCliente.Attributes.Add("enabled", "enabled");
            txtidPedido.Text = "";
            ddlsReferencia.SelectedValue = "0";
            ddlidEndereco.SelectedValue = "0";
            ddlidEndereco.Attributes.Add("enabled", "enabled");
            ddlidContato.SelectedValue = "0";
            ddlidDepartamento.Attributes.Add("enabled", "enabled");
            ddlidDepartamento.SelectedValue = "0";
            ddlidSolicitante.SelectedValue = "0";

            ddlColaborador.SelectedValue = "0";
            txtdtPrevisaoInicio.Text = "";
            txtdtPrevisaoTermino.Text = "";
            lstidJornadaTrabalho.SelectedValue = "";
            //cblidJornadaTrabalho.SelectedValue = "0";
            txtdtDataInicio.Text = "";
            txtdtDataTermino.Text = "";
            lstidTipoAtividadeExterna.SelectedValue = "";
            lstidTipoAtividadeInterna.SelectedValue = "";
            //cblidTipoAtividade.SelectedValue = "0";
            ddlsHoraExtra.Text = "N";
            txtsProduto.Text = "";
            txtdtHorarioInicio.Text = "";
            //txtdtHorarioInicio.Hora = "";
            //txtdtHorarioInicio.LabelText = "Horário de Início";
            txtdtHorarioTermino.Text = "";
            //txtdtHorarioTermino.Hora = "";
            //txtdtHorarioTermino.LabelText = "Horário de Término";
            lstsExigencias.SelectedValue = "";
            //cblsExigencias.SelectedValue = "0";
            ddlsPericulosidade.SelectedValue = "N";
            txtsDscMotivo.Text = "";
            txtsObservacao.Text = "";
            lstidDocumentosExterna.SelectedValue = "";
            lstidDocumentosInterna.SelectedValue = "";

            //cblidDocumentos.SelectedValue = "0";


            ddlEPI.SelectedValue = "0";
            txtnQuantidadeEPI.Text = "";

            ddlTipoAcomodacao.SelectedValue = "0";
            txtsNomeAcomodacao.Text = "";
            txtsEnderecoAcomodacao.Text = "";
            txtsNomeContatoAcomodacao.Text = "";
            txtsContatoAcomodacao.Text = "";

            ddlTipoTransporte.SelectedValue = "0";
            txtsMarca.Text = "";
            txtsModelo.Text = "";
            txtsPlaca.Text = "";
            txtsCor.Text = "";
            ddlsFrota.SelectedValue = "0";
            txtsItinerario.Text = "";

            
            hddidOrdemServico.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina; 

            DIV_CLIENTE.Visible = false;
            DIV_CONTATO.Visible = false;
            DIV_FROTA.Visible = false;
            DIV_ALUGADO.Visible = false;
            DIV_TRANSPORTE.Visible = false;
            DIV_INTERNO.Visible = false;
            DIV_EXTERNO.Visible = false;

            updPanel_Seguranca.Visible = false;
            updPanel_Acomodacao.Visible = false;
            updPanel_Transporte.Visible = false;

        }

        #endregion

        protected void Pesquisar(string idPesquisa, bool bEdicao)
        {
            string sErro = "";
            PopularCombos();
            //div_EnviarArquivos.Visible = false;
            aba_Procedimentos.Visible = false;
            aba_Historico.Visible = false;
            aba_Arquivos.Visible = false;
            cmdEditar.Visible = false;
            cmdAcao.Visible = false;
            lnkIniciarOrdemServico.Visible = false;
            lnkRejeitarOrdemServico.Visible = false;
            lnkFinalizarOrdemServico.Visible = false;
            cmdAlterarStatus.Visible = false;
            lblTituloStatus.Visible = false;

            try
            {
                LimpaCampos();
                hddidCliente.Value = "0";
                cmdSalvar.Text = "Salvar";
                
                if (idPesquisa != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idOrdemServico", idPesquisa);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
                    

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidOrdemServico.Value = RETORNO.DATASET(dsPesquisa, 0, "idOrdemServico");
                        txtsNumeroOS.Text = RETORNO.DATASET(dsPesquisa, 0, "sNumeroOS");
                        txtidOrdemServico.Text = RETORNO.DATASET(dsPesquisa, 0, "idOrdemServico");
                        ddlidTipoOrdemServico.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipoOrdemServico");

                                               
                        txtdtOrdemServico.Text = RETORNO.DATASET(dsPesquisa, 0, "dtOrdemServico");
                        txtsCentroCusto.Text = RETORNO.DATASET(dsPesquisa, 0, "sCentroCusto");
                        ddlidCliente.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCliente");
                        txtidPedido.Text = RETORNO.DATASET(dsPesquisa, 0, "idPedido");
                        ddlidEndereco.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEndereco");
                        ddlidContato.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idContato");
                        ddlidDepartamento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idDepartamento");
                        ddlidSolicitante.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idSolicitante");


                        txtdtDataInicio.Text = RETORNO.DATASET(dsPesquisa, 0, "dtDataInicio");
                        txtdtDataTermino.Text = RETORNO.DATASET(dsPesquisa, 0, "dtDataTermino");
                        ddlsHoraExtra.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sHoraExtra");
                        txtsProduto.Text = RETORNO.DATASET(dsPesquisa, 0, "sProduto");
                        txtdtHorarioInicio.Text = RETORNO.DATASET(dsPesquisa, 0, "dtHorarioInicio");
                        //txtdtHorarioInicio.Hora = RETORNO.DATASET(dsPesquisa, 0, "dtHorarioInicio");
                        txtdtHorarioTermino.Text = RETORNO.DATASET(dsPesquisa, 0, "dtHorarioTermino");
                        //txtdtHorarioTermino.Hora = RETORNO.DATASET(dsPesquisa, 0, "dtHorarioTermino");
                        //ddlsExigencias.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sExigencia");
                        ddlsPericulosidade.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sPericulosidade");
                        txtsDscMotivo.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscMotivo");
                        txtsObservacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacao");
                        lblTituloStatus.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscStatus");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        lblTituloPagina.Text = string.Format("Ordem de Serviço ID:{0} - N°:{1}", RETORNO.DATASET(dsPesquisa, 0, "idOrdemServico"), txtsNumeroOS.Text);
                        BreadCrumb.TitulodaPagina = string.Format("Ordem de Serviço {0}", RETORNO.DATASET(dsPesquisa, 0, "idOrdemServico"));
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.OrdemServico.Procedimentos.Alterar);

                        lblTituloStatus.CssClass = string.Format("label label-{0}", RETORNO.DATASET(dsPesquisa, 0, "sCor"));
                        caixaTitulo.Attributes["class"] = string.Format("well-lg label-{0}", RETORNO.DATASET(dsPesquisa, 0, "sCor"));

                        //lblTituloSalvar.Text = "Confirma a Alteração da OS " + lblTituloPagina.Text + "?";
                        //lblTituloEdiar.Text = "Deseja editar a OS " + lblTituloPagina.Text + "?";

                        Popular_dtgOSColaboradores(dsPesquisa);
                        Popular_dtgOSEPI(dsPesquisa);
                        Popular_dtgOSAcomodacao(dsPesquisa);
                        Popular_dtgOSTransporte(dsPesquisa);
                        Popular_JornadaTrabalho(RETORNO.DATASET(dsPesquisa, 0, "sidJornadaTrabalho"));
                        Popular_TipoAtividade(RETORNO.DATASET(dsPesquisa, 0, "sidAtividade"));
                        Popular_Exigencias(RETORNO.DATASET(dsPesquisa, 0, "sExigencias"));
                        Popular_Documentos(RETORNO.DATASET(dsPesquisa, 0, "sidDocumentos"));

                        AlterarEstadoControles(bEdicao);

                        if (!bEdicao)
                        {
                            switch (RETORNO.DATASET(dsPesquisa, "sAcao"))
                            {
                                case "Iniciar":
                                    cmdAcao.Visible = true;
                                    lnkIniciarOrdemServico.Visible = true;
                                    lnkRejeitarOrdemServico.Visible = true;
                                    cmdEditar.Visible = true;
                                    aba_Procedimentos.Visible = true;
                                    break;

                                case "Finalizar":
                                    cmdAcao.Visible = true;
                                    lnkFinalizarOrdemServico.Visible = true;
                                    lnkRejeitarOrdemServico.Visible = true;
                                    aba_Procedimentos.Visible = true;
                                    break;

                                case "Alterar Status":
                                    cmdEditar.Visible = true;
                                    cmdAlterarStatus.Visible = true;
                                    break;
                            }

                            
                            Popular_Aba_Arquivos(idPesquisa);
                            Popular_Aba_Historico(dsPesquisa);
                            
                            lblTituloStatus.Visible = true;

                            if (RETORNO.DATASET(dsPesquisa, 0, "sPermiteEdicao") == "S")
                            {
                                //cmdEditar.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                {
                    BreadCrumb.TitulodaPagina = "Incluir";
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    //lblTituloSalvar.Text = "Confirma a Inclusão da OS?";
                    cmdSalvar.Text = "Incluir";
                    ddlidSolicitante.SelectedValue = IDENTITY.Variaveis.idUsuario();
                    txtdtOrdemServico.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    ddlidDepartamento.Focus();
                }
                
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
            RegistraScript("");
        }
                
        private bool ValidarDados()
        {
            if (ddlidDepartamento.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um Departamento");
                ddlidDepartamento.Focus();
                return false;
            }

            if (ddlidTipoOrdemServico.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Informe um tipo de Ordem de Servico!");
                ddlidTipoOrdemServico.Focus();
                return false;
            }

            return true;
        }

        void Popular_Aba_Historico(DataSet ds)
        {
            gv_Historico.DataSource = ds.Tables[5];
            gv_Historico.DataBind();
            aba_Historico.Visible = true;
        }

        void Popular_Aba_Arquivos(string idPedido)
        {
            frmArquivos.Attributes.Add("src", string.Format("../Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idPedido, "OrdemServico"));
            aba_Arquivos.Visible = true;
        }
        
        void AlterarEstadoControles(bool bAtivo)
        {
            cmdSalvar.Visible = false;
            cmdEditar.Visible = false;

            ddlidSolicitante.Attributes.Remove("disabled");
            ddlidDepartamento.Attributes.Remove("disabled");
            ddlidTipoOrdemServico.Attributes.Remove("disabled");
            ddlidCliente.Attributes.Remove("disabled");
            ddlsReferencia.Attributes.Remove("disabled");
            ddlidEndereco.Attributes.Remove("disabled");
            ddlidContato.Attributes.Remove("disabled");
            //txtdtOrdemServico.ReadOnly = !bAtivo;
            //txtsCentroCusto.ReadOnly = !bAtivo;
            //txtsDscMotivo.ReadOnly = !bAtivo;
            //txtsObservacao.ReadOnly = !bAtivo;


            if (!bAtivo)
            {
                ddlidSolicitante.Attributes.Add("disabled", "disabled");
                ddlidDepartamento.Attributes.Add("disabled", "disabled");
                ddlidTipoOrdemServico.Attributes.Add("disabled", "disabled");
                ddlidCliente.Attributes.Add("disabled", "disabled");
                ddlsReferencia.Attributes.Add("disabled", "disabled");
                ddlidEndereco.Attributes.Add("disabled", "disabled");
                ddlidContato.Attributes.Add("disabled", "disabled");
                txtdtOrdemServico.ReadOnly = !bAtivo;
                txtsCentroCusto.ReadOnly = !bAtivo;
            }
            else
            {
                //cmdSalvar.Visible = true;
               
            }


        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (ValidarDados())
            {

                try
                {
                    string[] vidOrdemServico = hddidOrdemServico.Value.Split(',');
                    string idOrdemServico = vidOrdemServico[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idOrdemServico", idOrdemServico);
                    vParametros.Add("@sNumeroOS", txtsNumeroOS.Text);
                    vParametros.Add("@idTipoOrdemServico", ddlidTipoOrdemServico.SelectedValue);
                    vParametros.Add("@dtOrdemServico", txtdtOrdemServico.Text);
                    vParametros.Add("@sCentroCusto", txtsCentroCusto.Text);
                    vParametros.Add("@idCliente", ddlidCliente.SelectedValue);
                    vParametros.Add("@idPedido", txtidPedido.Text);
                    vParametros.Add("@idDepartamento", ddlidDepartamento.SelectedValue);
                    vParametros.Add("@idEndereco", ddlidEndereco.SelectedValue);
                    vParametros.Add("@idContato", ddlidContato.SelectedValue);
                    vParametros.Add("@idSolicitante", ddlidSolicitante.SelectedValue);

                    vParametros.Add("@sidJornadaTrabalho", Concatenar_JornadaTrabalho());
                    vParametros.Add("@dtDataInicio", txtdtDataInicio.Text);
                    vParametros.Add("@dtDataTermino", txtdtDataTermino.Text);
                    vParametros.Add("@sidAtividade", Concatenar_TipoAtividade());
                    vParametros.Add("@sHoraExtra", ddlsHoraExtra.SelectedValue);
                    vParametros.Add("@sProduto", txtsProduto.Text);
                    vParametros.Add("@dtHorarioInicio", txtdtHorarioInicio.Text);
                    vParametros.Add("@dtHorarioTermino", txtdtHorarioTermino.Text);
                    vParametros.Add("@sExigencias", Concatenar_Exigencias());
                    vParametros.Add("@sPericulosidade", ddlsPericulosidade.SelectedValue);
                    vParametros.Add("@sDscMotivo", txtsDscMotivo.Text);
                    vParametros.Add("@sObservacao", txtsObservacao.Text);
                    vParametros.Add("@sidDocumentos", Concatenar_Documentos());

                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idOrdemServico = RETORNO.DATASET(dsSalvar, 0, "idOrdemServico");
                        
                        if (Salvar_Colaboradores_Itens(idOrdemServico) && Salvar_EPI_Itens(idOrdemServico) && Salvar_Acomodacao_Itens(idOrdemServico) && Salvar_Transporte_Itens(idOrdemServico))
                        {
                            Pesquisar(idOrdemServico, false);
                            MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                            MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!  </br><a href='OrdemServico_Detalhe.aspx?id=0'>Clique aqui para incluir uma nova ordem de servico.</a>");
                        }

                        
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }

                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }

            }
            RegistraScript("");
        }

        bool Salvar_Colaboradores_Itens(string idOrdemServico)
        {
            bool bRetorno = false;

            try
            {
                DataSet dsOSColaboradores_Excluir;
                Dictionary<String, String> vParametroItens_Excluir = new Dictionary<string, string>();
                vParametroItens_Excluir.Add("@sFuncao", "EXCLUIR_COLABORADORES");
                vParametroItens_Excluir.Add("@idOrdemServico", idOrdemServico);
                dsOSColaboradores_Excluir = BD.ExecutarDataSet(sProcedure, vParametroItens_Excluir);

                foreach (GridViewRow item in dtgOSColaboradores.Rows)
                {
                    string idColaborador = item.Cells[0].Text;
                    string sDscColaborador = item.Cells[1].Text;
                    string dtPrevisaoInicio = item.Cells[2].Text;
                    string dtPrevisaoTermino = item.Cells[3].Text;


                    if (idOrdemServico != "0")
                    {
                        DataSet dsOSColaboradores_Incluir;
                        Dictionary<String, String> vParametroItens_Incluir = new Dictionary<string, string>();

                        vParametroItens_Incluir.Add("@sFuncao", "INSERIR_COLABORADORES");
                        vParametroItens_Incluir.Add("@idOrdemServico", idOrdemServico);
                        vParametroItens_Incluir.Add("@idColaborador", idColaborador);
                        vParametroItens_Incluir.Add("@sDscColaborador", Server.HtmlDecode(sDscColaborador));
                        vParametroItens_Incluir.Add("@dtPrevisaoInicio", Server.HtmlDecode(dtPrevisaoInicio));
                        vParametroItens_Incluir.Add("@dtPrevisaoTermino", Server.HtmlDecode(dtPrevisaoTermino));

                        dsOSColaboradores_Incluir = BD.ExecutarDataSet(sProcedure, vParametroItens_Incluir);
                    }

                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            return bRetorno;

        }

        bool Salvar_EPI_Itens(string idOrdemServico)
        {
            bool bRetorno = false;

            try
            {
                DataSet dsOSEPI_Excluir;
                Dictionary<String, String> vParametroItens_Excluir = new Dictionary<string, string>();
                vParametroItens_Excluir.Add("@sFuncao", "EXCLUIR_EPI");
                vParametroItens_Excluir.Add("@idOrdemServico", idOrdemServico);
                dsOSEPI_Excluir = BD.ExecutarDataSet(sProcedure, vParametroItens_Excluir);

                foreach (GridViewRow item in dtgOSEPI.Rows)
                {
                    string idEPI = item.Cells[0].Text;
                    string sDscEPI = item.Cells[1].Text;
                    string nQuantidadeEPI = item.Cells[2].Text;
                    
                    if (idOrdemServico != "0")
                    {
                        DataSet dsOSEPI_Incluir;
                        Dictionary<String, String> vParametroItens_Incluir = new Dictionary<string, string>();

                        vParametroItens_Incluir.Add("@sFuncao", "INSERIR_EPI");
                        vParametroItens_Incluir.Add("@idOrdemServico", idOrdemServico);
                        vParametroItens_Incluir.Add("@idEPI", idEPI);
                        vParametroItens_Incluir.Add("@sDscEPI", sDscEPI);
                        vParametroItens_Incluir.Add("@nQuantidadeEPI", nQuantidadeEPI);
                        

                        dsOSEPI_Incluir = BD.ExecutarDataSet(sProcedure, vParametroItens_Incluir);
                    }

                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro1: " + ex.Message);
            }

            return bRetorno;

        }

        bool Salvar_Acomodacao_Itens(string idOrdemServico)
        {
            bool bRetorno = false;

            try
            {
                DataSet dsOSAcomodacao_Excluir;
                Dictionary<String, String> vParametroItens_Excluir = new Dictionary<string, string>();
                vParametroItens_Excluir.Add("@sFuncao", "EXCLUIR_ACOMODACAO");
                vParametroItens_Excluir.Add("@idOrdemServico", idOrdemServico);
                dsOSAcomodacao_Excluir = BD.ExecutarDataSet(sProcedure, vParametroItens_Excluir);

                foreach (GridViewRow item in dtgOSAcomodacao.Rows)
                {
                    string idTipoAcomodacao = item.Cells[0].Text;
                    string sNomeAcomodacao = item.Cells[1].Text;
                    string sEnderecoAcomodacao = item.Cells[2].Text;
                    string sNomeContatoAcomodacao = item.Cells[3].Text;
                    string sContatoAcomodacao = item.Cells[4].Text;

                    if (idOrdemServico != "0")
                    {
                        DataSet dsOSEPI_Incluir;
                        Dictionary<String, String> vParametroItens_Incluir = new Dictionary<string, string>();

                        vParametroItens_Incluir.Add("@sFuncao", "INSERIR_ACOMODACAO");
                        vParametroItens_Incluir.Add("@idOrdemServico", idOrdemServico);
                        vParametroItens_Incluir.Add("@idTipoAcomodacao", idTipoAcomodacao);
                        vParametroItens_Incluir.Add("@sNomeAcomodacao", sNomeAcomodacao);
                        vParametroItens_Incluir.Add("@sEnderecoAcomodacao", sEnderecoAcomodacao);
                        vParametroItens_Incluir.Add("@sNomeContatoAcomodacao", sNomeContatoAcomodacao);
                        vParametroItens_Incluir.Add("@sContatoAcomodacao", sContatoAcomodacao);


                        dsOSEPI_Incluir = BD.ExecutarDataSet(sProcedure, vParametroItens_Incluir);
                    }

                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            return bRetorno;

        }

        bool Salvar_Transporte_Itens(string idOrdemServico)
        {
            bool bRetorno = false;

            try
            {
                DataSet dsOSTransporte_Excluir;
                Dictionary<String, String> vParametroItens_Excluir = new Dictionary<string, string>();
                vParametroItens_Excluir.Add("@sFuncao", "EXCLUIR_TRANSPORTE");
                vParametroItens_Excluir.Add("@idOrdemServico", idOrdemServico);
                dsOSTransporte_Excluir = BD.ExecutarDataSet(sProcedure, vParametroItens_Excluir);

                foreach (GridViewRow item in dtgOSTransporte.Rows)
                {
                    string idTipoTransporte = item.Cells[0].Text;
                    string sDscTipoTransporte = item.Cells[1].Text;
                    string sMarca = item.Cells[2].Text;
                    string sModelo = item.Cells[3].Text;
                    string sPlaca = item.Cells[4].Text;
                    string sCor = item.Cells[5].Text;
                    string sFrota = item.Cells[6].Text;
                    string sItinerario = item.Cells[7].Text;
                    
                    if (idOrdemServico != "0")
                    {
                        DataSet dsOSTransporte_Incluir;
                        Dictionary<String, String> vParametroItens_Incluir = new Dictionary<string, string>();

                        vParametroItens_Incluir.Add("@sFuncao", "INSERIR_TRANSPORTE");
                        vParametroItens_Incluir.Add("@idOrdemServico", idOrdemServico);
                        vParametroItens_Incluir.Add("@idTipoTransporte", idTipoTransporte);
                        vParametroItens_Incluir.Add("@sDscTipoTransporte", sDscTipoTransporte);
                        vParametroItens_Incluir.Add("@sMarca", sMarca);
                        vParametroItens_Incluir.Add("@sModelo", sModelo);
                        vParametroItens_Incluir.Add("@sPlaca", sPlaca);
                        vParametroItens_Incluir.Add("@sCor", sCor);
                        vParametroItens_Incluir.Add("@sFrota", sFrota);
                        vParametroItens_Incluir.Add("@sItinerario", sItinerario);


                        dsOSTransporte_Incluir = BD.ExecutarDataSet(sProcedure, vParametroItens_Incluir);
                    }

                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            return bRetorno;

        }

        protected void cmdIncluirColaborador_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_Colaboradores(ref sMensagem))
            {
                string[] vidOrdemServico = hddidOrdemServico.Value.Split(',');
                string idOrdemServico = vidOrdemServico[0].ToString();

                FrameWork.cls_OSColaboradores_Itens objItem = new FrameWork.cls_OSColaboradores_Itens();
                objItem.idOrdemServico = Convert.ToInt32(idOrdemServico);

                objItem.idColaborador = Convert.ToInt32(ddlColaborador.SelectedValue);
                objItem.sDscColaborador = ddlColaborador.SelectedItem.ToString();
                objItem.dtPrevisaoInicio = txtdtPrevisaoInicio.Text;
                objItem.dtPrevisaoTermino = txtdtPrevisaoTermino.Text;

                bs_OSColaboradores_Itens.Add(objItem);
                dtgOSColaboradores_DataBind();
                OSColaboradores_LimpaCampos();
            }
            else
            {
                //MensagemAcoes.MostraMensagem_Erro(sMensagem);
                //lblMensagem_Selecao.Text = sMensagem;
                //lblMensagem_Selecao.Visible = true;
            }
            RegistraScript("");
        }

        void OSColaboradores_LimpaCampos()
        {
            ddlColaborador.SelectedValue = "0";
            txtdtPrevisaoInicio.Text = "";
            txtdtPrevisaoTermino.Text = "";
        }

        protected void cmdIncluirEPI_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_EPI(ref sMensagem))
            {

                FrameWork.cls_OSEPI_Itens objItem = new FrameWork.cls_OSEPI_Itens();

                string[] vidOrdemServico = hddidOrdemServico.Value.Split(',');
                string idOrdemServico = vidOrdemServico[0].ToString();

                objItem.idOrdemServico = Convert.ToInt32(idOrdemServico);

                objItem.idEPI = ddlEPI.SelectedValue;
                objItem.sDscEPI = ddlEPI.SelectedItem.ToString();
                objItem.nQuantidadeEPI = txtnQuantidadeEPI.Text;
                

                bs_OSEPI_Itens.Add(objItem);
                dtgOSEPI_DataBind();

            }
            else
            {
                //MensagemAcoes.MostraMensagem_Erro(sMensagem);
                //lblMensagem_Selecao.Text = sMensagem;
                //lblMensagem_Selecao.Visible = true;
            }
            RegistraScript("");

        }

        protected void cmdIncluirAcomodacao_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_Acomodacao(ref sMensagem))
            {

                FrameWork.cls_OSAcomodacao_Itens objItem = new FrameWork.cls_OSAcomodacao_Itens();

                string[] vidOrdemServico = hddidOrdemServico.Value.Split(',');
                string idOrdemServico = vidOrdemServico[0].ToString();

                objItem.idOrdemServico = Convert.ToInt32(idOrdemServico);

                objItem.idTipoAcomodacao = ddlTipoAcomodacao.SelectedValue;
                objItem.sDscTipoAcomodacao = ddlTipoAcomodacao.SelectedItem.ToString();
                objItem.sNomeAcomodacao = txtsNomeAcomodacao.Text;
                objItem.sEnderecoAcomodacao = txtsEnderecoAcomodacao.Text;
                objItem.sNomeContatoAcomodacao = txtsNomeContatoAcomodacao.Text;
                objItem.sContatoAcomodacao = txtsContatoAcomodacao.Text;


                bs_OSAcomodacao_Itens.Add(objItem);
                dtgOSAcomodacao_DataBind();

            }
            else
            {
                //MensagemAcoes.MostraMensagem_Erro(sMensagem);
                //lblMensagem_Selecao.Text = sMensagem;
                //lblMensagem_Selecao.Visible = true;
            }
            RegistraScript("");

        }

        protected void cmdIncluirTransporte_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_Transporte(ref sMensagem))
            {

                FrameWork.cls_OSTransporte_Itens objItem = new FrameWork.cls_OSTransporte_Itens();

                string[] vidOrdemServico = hddidOrdemServico.Value.Split(',');
                string idOrdemServico = vidOrdemServico[0].ToString();

                objItem.idOrdemServico = Convert.ToInt32(idOrdemServico);

                objItem.idTipoTransporte = ddlTipoTransporte.SelectedValue;
                objItem.sDscTipoTransporte = ddlTipoTransporte.SelectedItem.ToString();
                objItem.sMarca = txtsMarca.Text;
                objItem.sModelo = txtsModelo.Text;
                objItem.sPlaca = txtsPlaca.Text;
                objItem.sCor = txtsCor.Text;
                objItem.sFrota = ddlsFrota.SelectedItem.ToString();
                objItem.sItinerario = txtsItinerario.Text;

                bs_OSTransporte_Itens.Add(objItem);
                dtgOSTransporte_DataBind();

            }
            else
            {
                //MensagemAcoes.MostraMensagem_Erro(sMensagem);
                //lblMensagem_Selecao.Text = sMensagem;
                //lblMensagem_Selecao.Visible = true;
            }
            RegistraScript("");

        }

        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (!txtdtOrdemServico.ReadOnly)
            {
                sb.Append("$(function() {$('[id*=txtdtOrdemServico]').datepicker({");
                sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");
            }
                                

            sb.Append("$(function() {$('[id*=txtdtPrevisaoInicio]').datepicker({");
            sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

            sb.Append("$(function() {$('[id*=txtdtPrevisaoTermino]').datepicker({");
            sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

            sb.Append("$(function() {$('[id*=txtdtDataInicio]').datepicker({");
            sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

            sb.Append("$(function() {$('[id*=txtdtDataTermino]').datepicker({");
            sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

            sb.Append("$('[id*=txtdtHorarioInicio]').mask('00:00');");
            sb.Append("$('[id*=txtdtHorarioTermino]').mask('00:00');");

            sb.Append("$(function () {$('[id*=lstidTipoAtividade]').multiselect({");
            sb.Append("buttonWidth: '195px',includeSelectAllOption: false, ");
            sb.Append("maxHeight: 300,dropRight: false,nSelectedText:' - Fluxos Selecionados!',");
            sb.Append("allSelectedText: 'Todos os Fluxos',enableFiltering: true});});");

            sb.Append("$(function () {$('[id*=lstidJornadaTrabalho]').multiselect({");
            sb.Append("buttonWidth: '195px',includeSelectAllOption: false, ");
            sb.Append("maxHeight: 300,dropRight: false,nSelectedText:' - Fluxos Selecionados!',");
            sb.Append("allSelectedText: 'Todos os Fluxos',enableFiltering: true});});");

            sb.Append("$(function () {$('[id*=lstsExigencias]').multiselect({");
            sb.Append("buttonWidth: '195px',includeSelectAllOption: false, ");
            sb.Append("maxHeight: 300,dropRight: false,nSelectedText:' - Fluxos Selecionados!',");
            sb.Append("allSelectedText: 'Todos os Fluxos',enableFiltering: true});});");

            sb.Append("$(function () {$('[id*=lstidDocumentos]').multiselect({");
            sb.Append("buttonWidth: '195px',includeSelectAllOption: false, ");
            sb.Append("maxHeight: 300,dropRight: false,nSelectedText:' - Fluxos Selecionados!',");
            sb.Append("allSelectedText: 'Todos os Fluxos',enableFiltering: true});});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
            OrdemServico_Acao_EsconderCaixa();

        }
        
        void Popular_dtgOSColaboradores(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[1].Rows)
            {
                FrameWork.cls_OSColaboradores_Itens objItem = new FrameWork.cls_OSColaboradores_Itens();
                objItem.idOrdemServico = Convert.ToInt32(row["idOrdemServico"].ToString());
                objItem.idColaborador = Convert.ToInt32(row["idColaborador"].ToString());
                objItem.sDscColaborador = row["sDscColaborador"].ToString();
                objItem.dtPrevisaoInicio = row["dtPrevisaoInicio"].ToString();
                objItem.dtPrevisaoTermino = row["dtPrevisaoTermino"].ToString();

                bs_OSColaboradores_Itens.Add(objItem);
            }
            dtgOSColaboradores_DataBind();

        }

        void Popular_dtgOSEPI(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[2].Rows)
            {
                FrameWork.cls_OSEPI_Itens objItem = new FrameWork.cls_OSEPI_Itens();
                objItem.idOrdemServico = Convert.ToInt32(row["idOrdemServico"].ToString());
                objItem.idEPI = row["idEPI"].ToString();
                objItem.sDscEPI = row["sDscEPI"].ToString();
                objItem.nQuantidadeEPI = row["nQuantidadeEPI"].ToString();
                bs_OSEPI_Itens.Add(objItem);
            }
            dtgOSEPI_DataBind();
        }

        void Popular_dtgOSAcomodacao(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[3].Rows)
            {
                FrameWork.cls_OSAcomodacao_Itens objItem = new FrameWork.cls_OSAcomodacao_Itens();
                objItem.idOrdemServico = Convert.ToInt32(row["idOrdemServico"].ToString());
                objItem.idTipoAcomodacao = row["idTipoAcomodacao"].ToString();
                //objItem.sDescricaoAcomodacao = row["sDescricaoAcomodacao"].ToString();
                objItem.sNomeAcomodacao = row["sNomeAcomodacao"].ToString();
                objItem.sEnderecoAcomodacao = row["sEnderecoAcomodacao"].ToString();
                objItem.sNomeContatoAcomodacao = row["sNomeContatoAcomodacao"].ToString();
                objItem.sContatoAcomodacao = row["sContatoAcomodacao"].ToString();
                bs_OSAcomodacao_Itens.Add(objItem);
            }
            dtgOSAcomodacao_DataBind();
        }

        void Popular_dtgOSTransporte(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[4].Rows)
            {
                FrameWork.cls_OSTransporte_Itens objItem = new FrameWork.cls_OSTransporte_Itens();
                objItem.idOrdemServico = Convert.ToInt32(row["idOrdemServico"].ToString());
                objItem.idTipoTransporte = row["idTipoTransporte"].ToString();
                objItem.sDscTipoTransporte = row["sDscTipoTransporte"].ToString();
                objItem.sMarca = row["sMarca"].ToString();
                objItem.sModelo = row["sModelo"].ToString();
                objItem.sPlaca = row["sPlaca"].ToString();
                objItem.sCor = row["sCor"].ToString();
                objItem.sFrota = row["sFrota"].ToString();
                objItem.sItinerario = row["sItinerario"].ToString();
                bs_OSTransporte_Itens.Add(objItem);
            }
            dtgOSTransporte_DataBind();
        }

        void Popular_JornadaTrabalho (string idJornadaTrabalho)
        {
            string[] vidJornadaTrabalho = idJornadaTrabalho.Split(';');

            for (int i = 0; i < vidJornadaTrabalho.Count(); i++)
            {

                for (int contador = 0; contador <= lstidJornadaTrabalho.Items.Count - 1; contador++)
                {
                    if (lstidJornadaTrabalho.Items[contador].Value == vidJornadaTrabalho[i].ToString())
                    {
                        lstidJornadaTrabalho.Items[contador].Selected = true;
                    }
                }

            }
        }

        void Popular_TipoAtividade(string idAtividade)
        {
            string[] vidAtividade = idAtividade.Split(';');

            if(ddlidTipoOrdemServico.SelectedValue == "1" || ddlidTipoOrdemServico.SelectedValue == "3" || ddlidTipoOrdemServico.SelectedValue == "5")
            {
                for (int i = 0; i < vidAtividade.Count(); i++)
                {
                    for (int contador = 0; contador <= lstidTipoAtividadeExterna.Items.Count - 1; contador++)
                    {
                        if (lstidTipoAtividadeExterna.Items[contador].Value == vidAtividade[i].ToString())
                        {
                            lstidTipoAtividadeExterna.Items[contador].Selected = true;
                        }
                    }

                }
            }

            else if (ddlidTipoOrdemServico.SelectedValue == "2" || ddlidTipoOrdemServico.SelectedValue == "4")
            {
                for (int i = 0; i < vidAtividade.Count(); i++)
                {
                    for (int contador = 0; contador <= lstidTipoAtividadeInterna.Items.Count - 1; contador++)
                    {
                        if (lstidTipoAtividadeInterna.Items[contador].Value == vidAtividade[i].ToString())
                        {
                            lstidTipoAtividadeInterna.Items[contador].Selected = true;
                        }
                    }
                }
            }
        }

        void Popular_Exigencias(string sExigencias)
        {
            string[] vsExigencias = sExigencias.Split(';');

            for (int i = 0; i < vsExigencias.Count(); i++)
            {

                for (int contador = 0; contador <= lstsExigencias.Items.Count - 1; contador++)
                {
                    if (lstsExigencias.Items[contador].Value == vsExigencias[i].ToString())
                    {
                        lstsExigencias.Items[contador].Selected = true;
                    }
                }

            }
        }

        void Popular_Documentos(string idDocumentos)
        {
            string[] vidDocumentos = idDocumentos.Split(';');

            if (ddlidTipoOrdemServico.SelectedValue == "1" || ddlidTipoOrdemServico.SelectedValue == "3" || ddlidTipoOrdemServico.SelectedValue == "5")

            {
                for (int i = 0; i < vidDocumentos.Count(); i++)
                {
                    for (int contador = 0; contador <= lstidDocumentosExterna.Items.Count - 1; contador++)
                    {
                        if (lstidDocumentosExterna.Items[contador].Value == vidDocumentos[i].ToString())
                        {
                            lstidDocumentosExterna.Items[contador].Selected = true;
                        }
                    }
                }
            }

            else if (ddlidTipoOrdemServico.SelectedValue == "2" || ddlidTipoOrdemServico.SelectedValue == "4")
            {
                for (int i = 0; i < vidDocumentos.Count(); i++)
                {
                    for (int contador = 0; contador <= lstidDocumentosInterna.Items.Count - 1; contador++)
                    {
                        if (lstidDocumentosInterna.Items[contador].Value == vidDocumentos[i].ToString())
                        {
                            lstidDocumentosInterna.Items[contador].Selected = true;
                        }
                    }
                }
            }
        }

        string Concatenar_TipoAtividade()
        {
            string sRetornoConcatenado = "";

            if (ddlidTipoOrdemServico.SelectedValue == "1" || ddlidTipoOrdemServico.SelectedValue == "3" || ddlidTipoOrdemServico.SelectedValue == "5")
            {
                for (int contador = 0; contador <= lstidTipoAtividadeExterna.Items.Count - 1; contador++)
                {
                    if (lstidTipoAtividadeExterna.Items[contador].Selected)
                    {
                        sRetornoConcatenado += string.Concat(lstidTipoAtividadeExterna.Items[contador].Value, ";");
                    }
                }
            }

            else if (ddlidTipoOrdemServico.SelectedValue == "2" || ddlidTipoOrdemServico.SelectedValue == "4")
            {
                for (int contador = 0; contador <= lstidTipoAtividadeInterna.Items.Count - 1; contador++)
                {
                    if (lstidTipoAtividadeInterna.Items[contador].Selected)
                    {
                        sRetornoConcatenado += string.Concat(lstidTipoAtividadeInterna.Items[contador].Value, ";");
                    }
                }
            }

            return sRetornoConcatenado;
        }

        string Concatenar_JornadaTrabalho()
        {
            string sRetornoConcatenado = "";


            for (int contador = 0; contador <= lstidJornadaTrabalho.Items.Count - 1; contador++)
            {
                if (lstidJornadaTrabalho.Items[contador].Selected)
                {
                    sRetornoConcatenado += string.Concat(lstidJornadaTrabalho.Items[contador].Value, ";");
                }

            }

            return sRetornoConcatenado;
        }

        string Concatenar_Exigencias()
        {
            string sRetornoConcatenado = "";


            for (int contador = 0; contador <= lstsExigencias.Items.Count - 1; contador++)
            {
                if (lstsExigencias.Items[contador].Selected)
                {
                    sRetornoConcatenado += string.Concat(lstsExigencias.Items[contador].Value, ";");
                }

            }

            return sRetornoConcatenado;
        }

        string Concatenar_Documentos()
        {
            string sRetornoConcatenado = "";

            if (ddlidTipoOrdemServico.SelectedValue == "1" || ddlidTipoOrdemServico.SelectedValue == "3" || ddlidTipoOrdemServico.SelectedValue == "5")
            {
                for (int contador = 0; contador <= lstidDocumentosExterna.Items.Count - 1; contador++)
                {
                    if (lstidDocumentosExterna.Items[contador].Selected)
                    {
                        sRetornoConcatenado += string.Concat(lstidDocumentosExterna.Items[contador].Value, ";");
                    }

                }
            }
            else if (ddlidTipoOrdemServico.SelectedValue == "2" || ddlidTipoOrdemServico.SelectedValue == "4")
            {
                for (int contador = 0; contador <= lstidDocumentosInterna.Items.Count - 1; contador++)
                {
                    if (lstidDocumentosInterna.Items[contador].Selected)
                    {
                        sRetornoConcatenado += string.Concat(lstidDocumentosInterna.Items[contador].Value, ";");
                    }

                }
            }

                

            return sRetornoConcatenado;
        }

        void dtgOSColaboradores_DataBind()
        {
            dtgOSColaboradores.DataSource = bs_OSColaboradores_Itens;
            dtgOSColaboradores.DataBind();
        }

        void dtgOSEPI_DataBind()
        {
            dtgOSEPI.DataSource = bs_OSEPI_Itens;
            dtgOSEPI.DataBind();
        }

        void dtgOSAcomodacao_DataBind()
        {
            dtgOSAcomodacao.DataSource = bs_OSAcomodacao_Itens;
            dtgOSAcomodacao.DataBind();
        }

        void dtgOSTransporte_DataBind()
        {
            dtgOSTransporte.DataSource = bs_OSTransporte_Itens;
            dtgOSTransporte.DataBind();
        }

       protected void dtgOSColaboradores_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        protected void dtgOSEPI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        protected void dtgOSAcomodacao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        protected void dtgOSTransporte_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        protected void dtgOSColaboradores_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            bs_OSColaboradores_Itens.RemoveAt(index);
            dtgOSColaboradores_DataBind();
        }

        protected void dtgOSEPI_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            bs_OSEPI_Itens.RemoveAt(index);
            dtgOSEPI_DataBind();
        }

        protected void dtgOSAcomodacao_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            bs_OSAcomodacao_Itens.RemoveAt(index);
            dtgOSAcomodacao_DataBind();
        }

        protected void dtgOSTransporte_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            bs_OSTransporte_Itens.RemoveAt(index);
            dtgOSTransporte_DataBind();
        }

        protected void ddlTipoOrdemServico_SelectedIndexChanged(object sender, EventArgs e)
        {

            DIV_CLIENTE.Visible = false;
            DIV_CONTATO.Visible = false;
            DIV_INTERNO.Visible = false;
            DIV_EXTERNO.Visible = false;
            DIV_LST_EXTERNA.Visible = false;
            DIV_LST_INTERNA.Visible = false;
            updPanel_Seguranca.Visible = false;
            updPanel_Acomodacao.Visible = false;
            updPanel_Transporte.Visible = false;

            if (ddlidTipoOrdemServico.SelectedValue == "1") // Externa
            {
                DIV_CLIENTE.Visible = true;
                DIV_CONTATO.Visible = true;
                DIV_EXTERNO.Visible = true;
                DIV_LST_EXTERNA.Visible = true;
                updPanel_Seguranca.Visible = true;
                updPanel_Acomodacao.Visible = true;
                updPanel_Transporte.Visible = true;
                
            }
            else if (ddlidTipoOrdemServico.SelectedValue == "2") // Interna
            {
                DIV_INTERNO.Visible = true;
                DIV_LST_INTERNA.Visible = true;
                updPanel_Seguranca.Visible = true;
               
            }
            else if (ddlidTipoOrdemServico.SelectedValue == "3") // Melhoria
            {
                DIV_LST_EXTERNA.Visible = true;
                updPanel_Seguranca.Visible = true;
               
            }
            else if (ddlidTipoOrdemServico.SelectedValue == "4") // TI
            {
                DIV_LST_INTERNA.Visible = true;
            }
            else if (ddlidTipoOrdemServico.SelectedValue == "5") // Conservação
            {
                DIV_LST_EXTERNA.Visible = true;
                updPanel_Seguranca.Visible = true;
                
            }
            RegistraScript("");
        }

        protected void ddlidDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(hddidOrdemServico.Value == "0")
            
            {

                string sErro = "";
               
                try
                {
                    DataSet dsOS_Numero;
                    Dictionary<String, String> vParametroOS_Numero = new Dictionary<string, string>();
                    vParametroOS_Numero.Add("@sFuncao", "CONSULTAR_NUMERO");
                    vParametroOS_Numero.Add("@idPesquisa", ddlidDepartamento.SelectedValue);
                    dsOS_Numero = BD.ExecutarDataSet(sProcedure, vParametroOS_Numero);

                    if (BD.ValidarDataSet(dsOS_Numero, out sErro))
                    {
                        txtsNumeroOS.Text = RETORNO.DATASET(dsOS_Numero, 0, "sNumeroOS");
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }

            }
             
            

            RegistraScript("");

        }

        protected void ddlCliente_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (ClienteAnterior != ddlidCliente.Text)
            {
                FUNCOES.Popula_Combo(ddlsReferencia, "sp_Select 'Flow_Clientes_Referencia'," + ddlidCliente.Text, "idPedido", "sReferencia", false, "Sem Pedido", "0");
            }
            FUNCOES.Popula_Combo(ddlidEndereco, "sp_Select 'Flow_Clientes_Endereco'," + ddlidCliente.Text, "idEndereco", "sEnderecoCompleto", false, "Selecione um Endereco", "0");
            FUNCOES.Popula_Combo(ddlidContato, "sp_Select 'Flow_Clientes_Contato'," + ddlidCliente.Text, "idContato", "sContatoCompleto", false, "Selecione um Contato", "0");
            RegistraScript("");
        }

        protected void ddlReferencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtidPedido.Text = ddlsReferencia.SelectedValue.ToString();

            RegistraScript("");
        }

        protected void ddlTipoTransporte_SelectedIndexChanged(object sender, EventArgs e)
        {
            DIV_FROTA.Visible = false;
            DIV_ALUGADO.Visible = false;
            DIV_TRANSPORTE.Visible = false;
            if (ddlTipoTransporte.SelectedValue == "1")
            {
                DIV_FROTA.Visible = true;
                FUNCOES.Popula_Combo(ddlsFrota, "sp_Select 'Flow_Veiculos_Frota'", "idVeiculo", "sFrotaCompleto", false, "-", "0");
            }
            else if (ddlTipoTransporte.SelectedValue == "2")
            {
                DIV_ALUGADO.Visible = true;
            }
            else
            {
                DIV_TRANSPORTE.Visible = true;
            }

            RegistraScript("");
        }

        private bool ValidarDados_Colaboradores(ref string sMensagem)
        {

            if (ddlColaborador.SelectedValue == "0")
            {
                MensagemColaboradores.MostraMensagem_Erro("Selecione um Colaborador!");
                return false;
            }
            if (txtdtPrevisaoInicio.Text.Length < 5)
            {
                MensagemColaboradores.MostraMensagem_Erro("Data inválida!");
                return false;

            }
            if (txtdtPrevisaoTermino.Text.Length < 5)
            {
                MensagemColaboradores.MostraMensagem_Erro("Data inválida!");
                return false;
            }
            return true;
        }

        private bool ValidarDados_EPI(ref string sMensagem)
        {

            if (ddlEPI.SelectedValue == "0")
            {
                MensagemEPI.MostraMensagem_Erro("Selecione um EPI!");
                return false;
            }
            
            if (txtnQuantidadeEPI.Text == "0")
            {
                MensagemEPI.MostraMensagem_Erro("Quantidade inválida!");
                return false;
            }
            return true;
        }

        private bool ValidarDados_Acomodacao(ref string sMensagem)
        {

            if (ddlTipoAcomodacao.SelectedValue == "")
            {
                MensagemAcomodacao.MostraMensagem_Erro("Selecione um Tipo de Acomodacao!");
                return false;
            }

           return true;
        }

        private bool ValidarDados_Transporte(ref string sMensagem)
        {

            if (ddlTipoTransporte.SelectedValue == "")
            {
                MensagemTransporte.MostraMensagem_Erro("Selecione um Tipo de Transporte!");
                return false;
            }
                       
            return true;
        }

        protected void lnkIniciarOrdemServico_Click(object sender, EventArgs e)
        {
            OrdemServico_Acao_MostrarCaixa("Iniciar Ordem Serviço");
        }

        protected void lnkRejeitarOrdemServico_Click(object sender, EventArgs e)
        {
            OrdemServico_Acao_MostrarCaixa("Rejeitar Ordem Serviço");
        }

        protected void lnkFinalizarOrdemServico_Click(object sender, EventArgs e)
        {

            //Pesquisar(Request["id"].ToString(), false);
            OrdemServico_Acao_MostrarCaixa("Finalizar Ordem Serviço");
        }

        void OrdemServico_Acao_MostrarCaixa(string sTitulo)
        {
            div_AlterarStatus.Visible = false;
            cmdEditar.Enabled = false;
            cmdAcao.Visible = false;
            lblOrdemServico_Acao_Titulo.Text = sTitulo;
            FUNCOES.Popula_Combo(ddlAlterarStatus, "sp_Select 'Flow_OrdemServico_Status', @sPesquisa = 'S'", "idStatus", "sDscStatus", false);

            if (sTitulo == "Iniciar Ordem Serviço")
            {
                lnkFinalizarOrdemServico.Visible = true;
                lnkRejeitarOrdemServico.Visible = true;
            }

            if (sTitulo == "Finalizar Ordem Serviço")
            {
                //div_AlterarStatus.Visible = true;
            }

            if (sTitulo == "Alterar Status")
            {
                div_AlterarStatus.Visible = true;
                cmdAcao.Visible = false;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Collapse", "$('#div_OrdemServico_Acoes').collapse();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Focus", "$('[id$=txtOrdemServico_ObservacaoStatus]').focus();", true);
        }
        
        void OrdemServico_Acao_EsconderCaixa()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Hide", "$('#div_OrdemServico_Acoes').hide();", true);
        }

        protected void cmdOrdemServico_Acao_OK_Click(object sender, EventArgs e)
        {
            string idStatus = "0";
            string sErro = "";

            if (lblOrdemServico_Acao_Titulo.Text == "Alterar Status")
            {
                if (ddlAlterarStatus.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione o Status!");
                    return;
                }
                else
                {
                    idStatus = ddlAlterarStatus.SelectedValue;
                }
            }



            if (txtOrdemServico_ObservacaoStatus.Text.Length < 6)
            {
                MensagemPagina.MostraMensagem_Erro("Insira um Motivo/Observação válido!");
            }
            else
            {
                DataSet dsTarefas;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", lblOrdemServico_Acao_Titulo.Text);
                vParametros.Add("@idOrdemServico", hddidOrdemServico.Value);
                vParametros.Add("@sObservacaoStatus", txtOrdemServico_ObservacaoStatus.Text);
                vParametros.Add("@idStatus", idStatus);
                vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                dsTarefas = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsTarefas, out sErro))
                {
                    MensagemPagina.MostraMensagem_Sucesso(RETORNO.DATASET(dsTarefas, "msg"));
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro(RETORNO.DATASET(dsTarefas, "msg"));
                }
                Pesquisar(hddidOrdemServico.Value, false);
                cmdEditar.Enabled = true;
                //cmdAcao.Visible = true;
                OrdemServico_Acao_EsconderCaixa();
            }

            RegistraScript("");
        }

        protected void cmdOrdemServico_Acao_Cancelar_Click(object sender, EventArgs e)
        {
            cmdEditar.Enabled = true;
            cmdAcao.Visible = true;
            Pesquisar(Request["id"].ToString(), false);
            OrdemServico_Acao_EsconderCaixa();
        }

        protected void cmdAlterarStatus_Click(object sender, EventArgs e)
        {

            //Pesquisar(Request["id"].ToString(), false);
            OrdemServico_Acao_MostrarCaixa("Alterar Status");
            cmdAcao.Visible = false;
        }
                
        protected void cmdEditar_Click(object sender, EventArgs e)
        {
            cmdEditar.Visible = false;
            cmdSalvar.Visible = true;
            ddlidSolicitante.Attributes.Remove("disabled");
            ddlidDepartamento.Attributes.Remove("disabled");
            ddlidTipoOrdemServico.Attributes.Remove("disabled");
            ddlidCliente.Attributes.Remove("disabled");
            ddlsReferencia.Attributes.Remove("disabled");
            ddlidEndereco.Attributes.Remove("disabled");
            ddlidContato.Attributes.Remove("disabled");
            txtdtOrdemServico.ReadOnly = false;
            txtsCentroCusto.ReadOnly = false;

            RegistraScript("");

        }
    }
}