using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using TT_Flow.FrameWork;
using TT.FrameWork;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Usuarios_Detalhe : Page
    {
        #region | Construtores

        public List<cls_Departamentos_Usuarios> bs_Departamentos_Usuarios
        {
            get
            {
                if (ViewState["bs_Departamentos_Usuarios"] == null)
                {
                    ViewState["bs_Departamentos_Usuarios"] = new List<cls_Departamentos_Usuarios>();
                }
                return (List<cls_Departamentos_Usuarios>)ViewState["bs_Departamentos_Usuarios"];
            }
            set
            {
                ViewState["bs_Departamentos_Usuarios"] = value;
            }

        }

        public List<cls_Usuarios_Empresa> bs_Empresa
        {
            get
            {
                if (ViewState["bs_Empresa"] == null)
                {
                    ViewState["bs_Empresa"] = new List<cls_Usuarios_Empresa>();
                }
                return (List<cls_Usuarios_Empresa>)ViewState["bs_Empresa"];
            }
            set
            {
                ViewState["bs_Empresa"] = value;
            }

        }

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            string idParceiro = IDENTITY.Variaveis.idParceiro();
            FUNCOES.ValidaPermissao(Permissao.Usuarios.Consultar, true);

            gv_Empresa.Visible = true;
            LinkButton1.Visible = false;
            LinkButton2.Visible = false;
            LinkButton3.Visible = false;
            LinkButton4.Visible = false;
            gv_Empresa.Columns[0].Visible = false;
            cmdLoginUsuario_DEV.Visible = FUNCOES.ValidaPermissao(Permissao.Usuarios.EfetuarLogin);

            if (FUNCOES.ValidaPermissao(Permissao.Usuarios.VerSenha))
            {
                LinkButton1.Visible = true;
                LinkButton2.Visible = true;
            }

            if (!FUNCOES.ValidaPermissao(Permissao.Usuarios.Alterar, false))
            {
                cmdExcluir.Visible = false;
                cmdForcarLogoff.Visible = false;
                cmdSalvar.Visible = false;
                AlterarEstadoControles(false);
            }

            if (!IsPostBack)
            {
                cmdExcluir.Visible = FUNCOES.ValidaPermissao(Permissao.Usuarios.Excluir, false);
                cmdForcarLogoff.Visible = FUNCOES.ValidaPermissao(Permissao.Usuarios.Excluir, false);

                if (Request["sf"] != null)
                {
                    if (idParceiro == "0")
                    {
                        string[] sFuncoesParceiro = Request["sf"].ToString().Split('|');
                        idParceiro = sFuncoesParceiro[0].ToString();
                        hddsFuncaoParceiro.Value = sFuncoesParceiro[1].ToString();
                    }
                }

                if (Request["idu"] != null)
                    PesquisarUsuario(Request["idu"].ToString(), Convert.ToInt32(idParceiro));
                else
                    PesquisarUsuario("0", Convert.ToInt32(idParceiro));

                if (Request["msg"] == "1") MensagemPagina.MostraMensagem_Sucesso("Login efetuado com sucesso!");
            }
            else
            {
                var requestTarget = Request["__EVENTTARGET"];
                if (requestTarget == "funcao_SAIR") FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                else if (requestTarget == "funcao_SALVAR") GravarUsuario(ctrl_Recursos.RecuperarPermissao());
                else if (requestTarget == "funcao_EXCLUIR") ExcluirUsuario();
                else if (requestTarget == "funcao_LOGOFF") ForcarLogoffUsuario();
            }

            RegistraScript();
        }

        void PesquisarUsuario(string idUsuario, int idParceiro)
        {
            string sDscParceiro = IDENTITY.DADOS.sDscParceiro(idParceiro);

            try
            {
                LimparCampos();
                AlterarEstadoControles(true);

                hddIdUsuario.Value = "0";
                hddidCliente.Value = "0";
                cmdSalvar.Text = "Salvar";
                cmdExcluir.Text = "Excluir";
                DIV_PrefixoLogin.Visible = false;

                if (idUsuario != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@idUsuario", idUsuario },
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idParceiro", idParceiro.ToString() }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        idParceiro = Convert.ToInt32(RETORNO.DATASET(dsPesquisa, 0, "idParceiro"));
                        FUNCOES.Popula_Combo(ddlidPerfil, "sp_Select 'Flow_Usuarios_Perfil'," + idParceiro.ToString(), "idPerfil", "sDscPerfil", false);
                        FUNCOES.Popula_Combo(ddlPaginaInicial, $"sp_Select 'Flow_DashBoards', {idUsuario}, @idFiltro=2", "idRecurso", "sDscRecurso", false, "Selecione a Página Inicial", "0");
                        FUNCOES.Popula_Combo(ddlDepartamentos, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Selecione o Departamento", "0");
                        FUNCOES.Popula_Combo(ddlTodasEmpresa, "sp_Select 'Flow_Usuario_x_Empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
                        ddlPaginaInicial.SelectedValue = RETORNO.DATASET(dsPesquisa, "idPaginaInicial");
                        txtsLogin.Text = RETORNO.DATASET(dsPesquisa, 0, "sLogin");
                        txtsDsUsuario.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario");
                        txtsSenha.Text = RETORNO.DATASET(dsPesquisa, 0, "sSenha");
                        txtsSenha1.Text = RETORNO.DATASET(dsPesquisa, 0, "sSenha");
                        txtsSenha_Confirmacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sSenha");
                        txtsSenha_Confirmacao.Attributes["value"] = RETORNO.DATASET(dsPesquisa, 0, "sSenha");
                        txtsSenha.Attributes["value"] = RETORNO.DATASET(dsPesquisa, 0, "sSenha");
                        txtdtValidade.Text = RETORNO.DATASET(dsPesquisa, 0, "dtValidade");
                        ddlTpUsuario.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sTipo");
                        hddidCliente.Value = RETORNO.DATASET(dsPesquisa, 0, "idCliente");
                        AjustarCliente(RETORNO.DATASET(dsPesquisa, 0, "sTipo"), RETORNO.DATASET(dsPesquisa, 0, "idCliente"));
                        txtsEmail.Text = RETORNO.DATASET(dsPesquisa, 0, "sEmail");
                        hddidParceiro.Value = idParceiro.ToString();
                        ddlsVendedor.Text = "Vendedor";
                        ddlsVendedor.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sVendedor"));
                        ddlsComprador.Text = "Comprador";
                        ddlsComprador.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sComprador"));

                        PreencherDepartamentos(dsPesquisa);
                        PreencherPerfildeAcesso(dsPesquisa);
                        ConfiguraParceiro(idParceiro);

                        ddlAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        PainelAtualizacao.Visible = true;

                        lblTituloPagina.Text = string.Format("Usuário {0} - {1}", RETORNO.DATASET(dsPesquisa, 0, "sLogin"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));
                        BreadCrumb.TitulodaPagina = txtsLogin.Text;
                        hddIdUsuario.Value = idUsuario;

                        cmdSalvar.Text = "Salvar";

                        lblTituloSalvar.Text = "Confirma a alteração nos dados do Usuário " + RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario") + "?";
                        lblTituloExcluir.Text = "Confirma a Exclusão do Usuário " + RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario") + "?";
                        lblTituloLogoff.Text = "Forçar o Logoff do Usuário " + RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario") + "?";

                        if (RETORNO.DATASET(dsPesquisa, 0, "sSituacao") == "E")
                        {
                            cmdForcarLogoff.Visible = false;
                            cmdExcluir.Text = "Reativar";
                            cmdSalvar.Visible = false;
                            AlterarEstadoControles(false);
                            lblTituloExcluir.Text = "Confirma a reativação do Usuário " + RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario") + "?";
                        }

                        if (idParceiro != 0)
                        {
                            lblTituloPagina.Text = string.Format("{0} - Parceiro: {1}", lblTituloPagina.Text, IDENTITY.DADOS.sDscParceiro(idParceiro));
                            txtsLogin.Text = txtsLogin.Text.Replace(lblsPrefixoLogin.Text, "");
                        }

                        if (hddsFuncaoParceiro.Value == "03E")
                        {
                            cmdExcluir.Visible = true;
                            cmdForcarLogoff.Visible = false;
                            cmdSalvar.Visible = false;
                            AlterarEstadoControles(false);
                        }

                        Popular_dtgPagamento(dsPesquisa);
                    }
                    else
                        throw new Exception("Erro ao consultar BD: " + sErro);
                }
                else
                {
                    BreadCrumb.TitulodaPagina = "Novo Usuário";
                    lblTituloPagina.Text = "Novo Usuário";
                    cmdSalvar.Text = "Incluir";
                    cmdExcluir.Visible = false;
                    cmdForcarLogoff.Visible = false;
                    lblTituloSalvar.Text = "Confirma a Inclusão do Usuário?";
                    txtsLogin.Focus();
                    hddIdUsuario.Value = "0";

                    if (IDENTITY.Variaveis.idCliente() != "0")
                    {
                        hddidCliente.Value = IDENTITY.Variaveis.idCliente();
                        ddlTpUsuario.SelectedValue = "C";
                        AjustarCliente("C", IDENTITY.Variaveis.idCliente());
                    }

                    ConfiguraParceiro(idParceiro);

                    if (idParceiro != 0)
                        lblTituloPagina.Text = string.Format("{0} - Parceiro: {1}", lblTituloPagina.Text, sDscParceiro);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao consultar BD: " + ex.Message);
            }
        }

        #endregion

        #region | Utils

        void LimparCampos()
        {
            DIV_Cliente.Visible = false;
            PainelAtualizacao.Visible = false;
            txtsSenha1.Visible = false;
            ddlsVendedor.Situacao_Definir("N");
            ddlsVendedor.Text = "Vendedor";
            ddlsComprador.Situacao_Definir("N");
            ddlsComprador.Text = "Comprador";
            bs_Departamentos_Usuarios.Clear();

            tab_Departamentos.Visible = false;
            tab_permissoes.Visible = false;
            tab_Empresa.Visible = false;
        }

        void ConfiguraParceiro(int idParceiro)
        {
            if (idParceiro != 0)
            {
                DIV_TipoParceiro.Visible = false;
                DIV_Vendedor.Visible = false;
                cmdExcluir.Visible = false;
                cmdForcarLogoff.Visible = false;
                DIV_PrefixoLogin.Visible = true;
                lblsPrefixoLogin.Text = string.Format("P{0}.", idParceiro.ToString());
                tab_Departamentos.Visible = false;
                hddidParceiro.Value = idParceiro.ToString();
            }
        }

        void PreencherPerfildeAcesso(DataSet dsPesquisa)
        {
            ddlidPerfil.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idPerfil");
            hddsRecursos.Value = RETORNO.DATASET(dsPesquisa, 0, "sRecursos");

            ddlidPerfil_SelectedIndexChanged(null, null);

            tab_permissoes.Visible = true;
            tab_Empresa.Visible = true;
        }

        void PreencherDepartamentos(DataSet dsRegistro)
        {
            foreach (DataRow row in dsRegistro.Tables[1].Rows)
            {
                cls_Departamentos_Usuarios objItem = new cls_Departamentos_Usuarios();
                objItem.idRegistro = Convert.ToInt32(row["idRegistro"].ToString());
                objItem.idDepartamento = Convert.ToInt32(row["idDepartamento"].ToString());
                objItem.idUsuario = Convert.ToInt32(row["idUsuario"].ToString());
                objItem.sDscDepartamento = row["sDscDepartamento"].ToString();
                objItem.sNotificacaoEmail = row["sNotificacaoEmail"].ToString();
                objItem.sGestorDepartamento = row["sGestorDepartamento"].ToString();
                bs_Departamentos_Usuarios.Add(objItem);
            }

            dtgDepartamentos_DataBind();
            tab_Departamentos.Visible = true;
        }

        void AjustarCliente(string sTipo, string idCliente)
        {
            DIV_Cliente.Visible = false;

            if (sTipo == "C")
            {
                if (idCliente != "0")
                {
                    ddlTpUsuario.SelectedValue = "C";

                    if (IDENTITY.Variaveis.idCliente() != "0")
                    {
                        ddlTpUsuario.Attributes.Add("disabled", "disabled");
                        ddlCliente.Attributes.Add("disabled", "disabled");
                    }
                }

                FUNCOES.Popula_Combo(ddlCliente, "sp_Select 'CLIENTE', " + IDENTITY.Variaveis.idCliente(), "idCliente", "sDscCliente", false, "Selecione o Ciente", "0");
                ddlCliente.SelectedValue = idCliente;
                DIV_Cliente.Visible = true;
            }

            string sSenha = txtsSenha.Text;
            string sSenhaConfirmacao = txtsSenha_Confirmacao.Text;

            txtsSenha1.Text = sSenha;
            txtsSenha_Confirmacao.Text = sSenhaConfirmacao;
            txtsSenha.Attributes["value"] = sSenha;
            txtsSenha_Confirmacao.Attributes["value"] = sSenhaConfirmacao;
        }

        void ForcarLogoffUsuario()
        {
            try
            {
                string[] vidUsuario = hddIdUsuario.Value.Split(',');
                string idUsuarioUtilizar = vidUsuario[0].ToString();

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "FORCA_LOGIN" },
                    { "@idUsuario", idUsuarioUtilizar },
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                };
                DataSet dsForcar = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", vParametros);

                if (BD.ValidarDataSet(dsForcar, out string sErro))
                {
                    idUsuarioUtilizar = RETORNO.DATASET(dsForcar, 0, "idUsuario");
                    PesquisarUsuario(idUsuarioUtilizar, 0);
                    MensagemPagina.MostraMensagem_Sucesso("Logoff Forçado com Sucesso!");
                }
                else
                    throw new Exception("BD: " + sErro.ToString());
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }

            RegistraScript();
        }

        bool AplicarValidacoes()
        {
            string sMensagemErro = "";
            string sSenha = txtsSenha.Text;
            string sSenhaConfirmacao = txtsSenha_Confirmacao.Text;

            txtsSenha.Attributes["value"] = sSenha;
            txtsSenha_Confirmacao.Attributes["value"] = sSenhaConfirmacao;

            if (txtsLogin.Text.Length < 3)
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um nome válido para Login!";

            if (txtsDsUsuario.Text.Length < 3)
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o nome do Usuário!";

            if (txtsSenha.Text.Length == 0)
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Senha!";

            if (txtsSenha.Text != txtsSenha_Confirmacao.Text)
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Senha e confirmação de senha são diferentes!";

            if (!Validacoes.ValidarEmail(txtsEmail.Text))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um e-mail Válido!";

            if (ddlTpUsuario.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Usuário!";

            if (ddlCliente.Visible == true)
            {
                if (ddlCliente.SelectedValue == "0")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Cliente";
            }

            if (!Validacoes.ValidarData(txtdtValidade) && txtdtValidade.Text != "")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de validade inválida!";
            else
            {
                if (DateTime.TryParse(txtdtValidade.Text, out DateTime dataValidade))
                {
                    if (dataValidade < DateTime.Today)
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de validade deve ser maior ou igual à data atual!";
                }
                else
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Formato de data inválido!";
            }


            if (sMensagemErro != "")
            {
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                return false;
            }

            return true;
        }

        void AlterarEstadoControles(bool bStatus)
        {
            string sStatusCombo = "disabled";
            if (bStatus)
                sStatusCombo = "enabled";

            txtsLogin.ReadOnly = !bStatus;
            txtsDsUsuario.ReadOnly = !bStatus;
            txtsSenha.ReadOnly = !bStatus;
            txtsSenha_Confirmacao.ReadOnly = !bStatus;
            txtsEmail.ReadOnly = !bStatus;
            txtdtValidade.ReadOnly = !bStatus;
            dtgDepartamentos.Enabled = bStatus;

            if (!bStatus)
            {
                cmdIncluirSelecao.Visible = false;
                cmdEmpresa.Visible = false;
                LinkButton1.Visible = false;
                LinkButton2.Visible = false;
                LinkButton3.Visible = false;
                LinkButton4.Visible = false;
            }

            ddlTodasEmpresa.Attributes.Remove("disabled");
            ddlTodasEmpresa.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlsGestorDepartamento.Attributes.Remove("disabled");
            ddlsGestorDepartamento.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlsNotificacaoEmail.Attributes.Remove("disabled");
            ddlsNotificacaoEmail.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlDepartamentos.Attributes.Remove("disabled");
            ddlDepartamentos.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlAtivo.Attributes.Remove("disabled");
            ddlAtivo.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlTpUsuario.Attributes.Remove("disabled");
            ddlTpUsuario.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlsVendedor.ReadOnly = !bStatus;
            ddlAtivo.ReadOnly = !bStatus;

            ddlsVendedor.Attributes.Add(sStatusCombo, sStatusCombo);
            ctrl_Recursos.ReadOnly = !bStatus;

            ddlsComprador.ReadOnly = !bStatus;
            ddlAtivo.ReadOnly = !bStatus;

            ddlsComprador.Attributes.Add(sStatusCombo, sStatusCombo);
            ctrl_Recursos.ReadOnly = !bStatus;

            ddlidPerfil.Attributes.Remove("disabled");
            ddlidPerfil.Attributes.Add(sStatusCombo, sStatusCombo);

            ddlPaginaInicial.Attributes.Remove("disabled");
            ddlPaginaInicial.Attributes.Add(sStatusCombo, sStatusCombo);
        }

        private bool ValidarDados_Departamento(ref string sMensagem)
        {
            if (ddlDepartamentos.SelectedValue == "0")
            {
                sMensagem = "Selecione um Departamento!";
                return false;
            }

            if (bs_Departamentos_Usuarios.Exists(x => x.idDepartamento == Convert.ToInt32(ddlDepartamentos.SelectedValue)))
            {
                sMensagem = "O Usuário " + txtsDsUsuario.Text + " já pertence ao departamento " + ddlDepartamentos.SelectedItem;
                return false;
            }

            return true;
        }

        private bool ValidarDados_Empresa(ref string sMensagem)
        {
            var empresa = bs_Empresa.Where(x => x.idEmpresa == Convert.ToInt32(ddlTodasEmpresa.SelectedValue) && x.sFuncao != "Excluir_Empresa").FirstOrDefault();

            if (ddlTodasEmpresa.SelectedValue == "0")
            {
                MensagemPagina1.MostraMensagem_Erro(sMensagem = "Selecione Uma Empresa!");
                return false;
            }
            if (empresa != null)
            {
                if (empresa.sFuncao != "Excluir_Empresa")
                {
                    MensagemPagina1.MostraMensagem_Erro(sMensagem = "O Usuário " + txtsDsUsuario.Text + " já pertence a empresa " + ddlTodasEmpresa.SelectedItem);
                    return false;
                }
                else
                    empresa.sFuncao = "Inserir_Empresa";
            }

            return true;
        }

        void Popular_dtgPagamento(DataSet dsRegistro)
        {
            bs_Empresa.Clear();
            foreach (DataRow row in dsRegistro.Tables[2].Rows)
            {
                cls_Usuarios_Empresa objItem = new cls_Usuarios_Empresa();
                objItem.idLinha = bs_Empresa.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";
                objItem.idUsuario = Convert.ToInt32(row["idUsuario"].ToString());
                objItem.idEmpresa = Convert.ToInt32(row["idEmpresa"].ToString());
                objItem.sDscEmpresa = row["sDscEmpresa"].ToString();
                objItem.idRegistro = Convert.ToInt32(row["idRegistro"].ToString());
                bs_Empresa.Add(objItem);
            }

            gv_Empresa_DataBind();
        }

        void PopularQRCode(DataSet ds)
        {
            object valor = RETORNO.DATASET(ds, 0, "vbQRCode");

            if (valor != null && valor != DBNull.Value)
            {
                string base64 = valor.ToString();

                if (!string.IsNullOrEmpty(base64))
                {
                    imgQRCode.ImageUrl = "data:image/png;base64," + base64;
                    imgQRCode.Visible = true;

                    cmdDownload.NavigateUrl = "data:image/png;base64," + base64;
                    cmdDownload.Attributes["download"] = $"QRCode-Usuario-{hddIdUsuario.Value}.png";
                    cmdDownload.Visible = true;
                    cmdGerarQRCode.Visible = false;
                }
                else
                {
                    imgQRCode.Visible = false;
                    cmdDownload.Visible = false;
                    cmdGerarQRCode.Visible = true;
                }
            }
            else
            {
                imgQRCode.Visible = false;
                cmdDownload.Visible = false;
                cmdGerarQRCode.Visible = true;
            }
        }

        #endregion

        #region | gv_Empresa

        void gv_Empresa_DataBind()
        {
            gv_Empresa.DataSource = bs_Empresa.Where(x => x.sFuncao != "Excluir_Empresa").OrderBy(x => x.idEmpresa);
            gv_Empresa.DataBind();

            // --------------- HIGOR MAESTRELLO 21/06/2024 -----------------------------

            var EmpresasParaAdicionar = bs_Empresa.Where(x => x.sFuncao == "Excluir_Empresa").Select(x => x.sDscEmpresa).ToList();
            var idEmpresasParaAdicionar = bs_Empresa.Where(x => x.sFuncao == "Excluir_Empresa").Select(x => x.idEmpresa).ToList();
            for (int i = 0; i < EmpresasParaAdicionar.Count; i++)
            {
                string desc = EmpresasParaAdicionar[i];
                string id = idEmpresasParaAdicionar[i].ToString();

                if (!ddlTodasEmpresa.Items.Cast<ListItem>().Any(item => item.Value == id))
                    ddlTodasEmpresa.Items.Add(new ListItem(desc, id));
            }

            var idsEmpresas = bs_Empresa.Where(x => x.sFuncao != "Excluir_Empresa").Select(x => x.idEmpresa).ToList();
            foreach (ListItem item in ddlTodasEmpresa.Items.Cast<ListItem>().ToList())
            {
                if (!string.IsNullOrEmpty(item.Value) && idsEmpresas.Contains(Convert.ToInt32(item.Value)))
                    ddlTodasEmpresa.Items.Remove(item);
            }

            bool itemJaExiste = false;
            foreach (ListItem listItem in ddlTodasEmpresa.Items)
            {
                if (listItem.Text == "Selecionar Todos" && listItem.Value == "999")
                {
                    itemJaExiste = true;
                    break;
                }
            }

            if (!itemJaExiste)
            {
                ListItem itemTodos = new ListItem("Selecionar Todos", "999");
                ddlTodasEmpresa.Items.Insert(1, itemTodos);
            }

            // -------------------------------------------------------------------------
        }

        protected void gv_Empresa_RowDataBound(object sender, GridViewRowEventArgs e) { }

        protected void gv_Empresa_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(gv_Empresa.DataKeys[e.RowIndex]["idLinha"].ToString());

            bs_Empresa[bs_Empresa.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "Excluir_Empresa";

            gv_Empresa_DataBind();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "aba_empresa", "$('#aba_empresa').tab('show');", true);
        }

        #endregion

        #region | dtgDepartamentos

        void dtgDepartamentos_DataBind()
        {
            dtgDepartamentos.DataSource = bs_Departamentos_Usuarios;
            dtgDepartamentos.DataBind();
        }

        protected void dtgSelecao_RowDataBound(object sender, GridViewRowEventArgs e) => Grid.EsconderColunas(e, 0);

        protected void dtgSelecao_RowDeleting(object sender, GridViewDeleteEventArgs e) { bs_Departamentos_Usuarios.RemoveAt(e.RowIndex); dtgDepartamentos_DataBind(); }

        #endregion

        #region | Salvar

        void GravarUsuario(string sPermissao)
        {
            string idCliente = "0";
            string idParceiro = IDENTITY.Variaveis.idParceiro();

            if (AplicarValidacoes())
            {
                try
                {
                    if (idParceiro == "0")
                        idParceiro = hddidParceiro.Value;

                    string[] vidUsuario = hddIdUsuario.Value.Split(',');
                    string idUsuarioUtilizar = vidUsuario[0].ToString();

                    if (ddlTpUsuario.SelectedValue == "C")
                        idCliente = ddlCliente.SelectedValue;

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idUsuario", idUsuarioUtilizar },
                        { "@idParceiro", idParceiro },
                        { "@idCliente", idCliente },
                        { "@sLogin", lblsPrefixoLogin.Text + txtsLogin.Text },
                        { "@sDsUsuario", txtsDsUsuario.Text },
                        { "@sSenha", txtsSenha.Text },
                        { "@sEmail", txtsEmail.Text },
                        { "@dtValidade", txtdtValidade.Text },
                        { "@sTipo", ddlTpUsuario.SelectedValue },
                        { "@sVendedor", ddlsVendedor.Situacao_Recuperar() },
                        { "@sComprador", ddlsComprador.Situacao_Recuperar() },
                        { "@idPerfil", ddlidPerfil.SelectedValue },
                        { "@idDashboard", ddlPaginaInicial.SelectedValue },
                        { "@sRecursos", sPermissao },
                        { "@sSituacao", ddlAtivo.Situacao_Recuperar() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                    };
                    DataSet dsGravar = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", vParametros);

                    if (BD.ValidarDataSet(dsGravar, out string sErro))
                    {
                        idUsuarioUtilizar = RETORNO.DATASET(dsGravar, 0, "idUsuario");
                        idParceiro = RETORNO.DATASET(dsGravar, 0, "idParceiro");

                        //if (ddlAtivo.Situacao_Recuperar() != "N")
                        Salvar_Departamentos_x_Usuarios(idUsuarioUtilizar);

                        Salvar_Empresa(idUsuarioUtilizar);
                        PesquisarUsuario(idUsuarioUtilizar, Convert.ToInt32(idParceiro));
                        MensagemPagina.MostraMensagem_Sucesso("Usuário gravado com sucesso!");
                    }
                    else throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }

                RegistraScript();
            }
        }

        void ExcluirUsuario()
        {
            string sFuncao = "EXCLUIR_USUARIO";
            string sSituacao = "E";
            string sMensgem = "Usuário Excluido com sucesso!";

            try
            {
                string[] vidUsuario = hddIdUsuario.Value.Split(',');
                string idUsuarioUtilizar = vidUsuario[0].ToString();

                if (cmdExcluir.Text != "Excluir")
                {
                    sFuncao = "REATIVAR_USUARIO";
                    sSituacao = "S";
                    sMensgem = "Usuário Reativado com sucesso!";
                }

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@idUsuario", idUsuarioUtilizar },
                    { "@sSituacao", sSituacao },
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                };
                DataSet dsExcluir = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", vParametros);

                if (BD.ValidarDataSet(dsExcluir, out string sErro))
                {
                    idUsuarioUtilizar = RETORNO.DATASET(dsExcluir, 0, "idUsuario");
                    PesquisarUsuario(idUsuarioUtilizar, 0);
                    MensagemPagina.MostraMensagem_Sucesso(sMensgem);

                    if (hddsFuncaoParceiro.Value == "03E")
                        Funcoes.DirecionaPagina(string.Format("app/Paginas/Manutencao/Parceiros_Detalhe.aspx?id={0}", hddidParceiro.Value));
                }
                else
                    throw new Exception("BD: " + sErro.ToString());
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }

            RegistraScript();
        }

        bool Salvar_Departamentos_x_Usuarios(string idUsuario)
        {
            try
            {
                Dictionary<string, string> vParametroSelecao_Excluir = new Dictionary<string, string>
                {
                    { "@sFuncao", "DELETE_POR_USUARIO" },
                    { "@idUsuario", idUsuario }
                };
                BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Departamentos", vParametroSelecao_Excluir);

                foreach (var linha in bs_Departamentos_Usuarios)
                {
                    if (idUsuario != "0")
                    {
                        Dictionary<string, string> vParametroDepartamentos_Incluir = new Dictionary<string, string>
                        {
                            { "@sFuncao", "INCLUIR_DEPARTAMENTOS_X_USUARIOS" },
                            { "@idDepartamento", linha.idDepartamento.ToString() },
                            { "@idUsuario", idUsuario },
                            { "@sNotificacaoEmail", linha.sNotificacaoEmail },
                            { "@sGestorDepartamento", linha.sGestorDepartamento },
                            { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                        };
                        BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Departamentos", vParametroDepartamentos_Incluir);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            return false;
        }

        bool Salvar_Empresa(string idUsuario)
        {
            try
            {
                string[] vidUsuario = hddIdUsuario.Value.Split(',');
                string idUsuarioUtilizar = vidUsuario[0].ToString();

                foreach (var Linha in bs_Empresa)
                {
                    if (Linha.sFuncao == "Excluir_Empresa")
                        Linha.sFuncao = "Excluir_Empresa";

                    if (Linha.sFuncao != "Excluir_Empresa")
                        Linha.sFuncao = "Inserir_Empresa";

                    Dictionary<string, string> vParametroEmpresa = new Dictionary<string, string>
                    {
                        ["@sFuncao"] = Linha.sFuncao,
                        ["@idUsuario"] = idUsuarioUtilizar,
                        ["@idEmpresa"] = Linha.idEmpresa.ToString(),
                        ["@sDscEmpresa"] = Linha.sDscEmpresa,
                        ["@idRegistro"] = Linha.idRegistro.ToString()
                    };
                    BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", vParametroEmpresa);
                }

                return true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            return false;

        }

        #endregion

        #region | Eventos

        protected void ddlTpUsuario_SelectedIndexChanged(object sender, EventArgs e) => AjustarCliente(ddlTpUsuario.SelectedValue, hddidCliente.Value);

        protected void txtsSenha_TextChanged(object sender, EventArgs e) { }

        protected void txtsSenha_Confirmacao_TextChanged(object sender, EventArgs e) { }

        protected void ddlCliente_SelectedIndexChanged(object sender, EventArgs e) => hddidCliente.Value = ddlCliente.SelectedValue;

        protected void ddlidPerfil_SelectedIndexChanged(object sender, EventArgs e)
        {
            ctrl_Recursos.Visible = false;

            if (ddlidPerfil.SelectedValue == "-99")
            {
                ctrl_Recursos.sRecursos = hddsRecursos.Value;
                ctrl_Recursos.Visible = true;
                ctrl_Recursos.Enable = true;
                ctrl_Recursos.ConsultarPermissao("-99");
            }
            else if (ddlidPerfil.SelectedValue != "0")
            {
                ctrl_Recursos.sRecursos = "";
                ctrl_Recursos.Visible = true;
                ctrl_Recursos.Enable = false;
                ctrl_Recursos.ConsultarPermissao(ddlidPerfil.SelectedValue);
            }
        }

        protected void cmdIncluirSelecao_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_Departamento(ref sMensagem))
            {

                cls_Departamentos_Usuarios objItem = new cls_Departamentos_Usuarios();
                objItem.idUsuario = Convert.ToInt32(hddIdUsuario.Value);
                objItem.idDepartamento = Convert.ToInt32(ddlDepartamentos.SelectedValue);
                objItem.sDscDepartamento = ddlDepartamentos.SelectedItem.ToString();
                objItem.sNotificacaoEmail = ddlsNotificacaoEmail.SelectedValue;
                objItem.sGestorDepartamento = ddlsGestorDepartamento.SelectedValue;
                bs_Departamentos_Usuarios.Add(objItem);
                dtgDepartamentos_DataBind();

                ddlDepartamentos.SelectedValue = "0";
                ddlsNotificacaoEmail.SelectedValue = "S";
            }
            else
                MensagemDepartamentos.MostraMensagem_Erro(sMensagem);
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            if (hddsFuncaoParceiro.Value != "")
                FUNCOES.DirecionaPagina(string.Format("app/Paginas/Manutencao/Parceiros_Detalhe.aspx?id={0}", hddidParceiro.Value));
            else
                FUNCOES.DirecionaPagina(string.Format("app/Paginas/Manutencao/Usuarios.aspx"));
        }

        protected void txtdtValidade_TextChanged(object sender, EventArgs e) { }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            txtsSenha.TextMode = TextBoxMode.SingleLine;
            LinkButton1.Visible = false;
            LinkButton3.Visible = true;
            txtsSenha.Attributes["value"] = txtsSenha.Text;
            txtsSenha_Confirmacao.Attributes["value"] = txtsSenha_Confirmacao.Text;
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            txtsSenha_Confirmacao.TextMode = TextBoxMode.SingleLine;
            LinkButton2.Visible = false;
            LinkButton4.Visible = true;
            txtsSenha.Attributes["value"] = txtsSenha.Text;
            txtsSenha_Confirmacao.Attributes["value"] = txtsSenha_Confirmacao.Text;
        }

        protected void LinkButton3_Click(object sender, EventArgs e)
        {
            txtsSenha.TextMode = TextBoxMode.Password;
            LinkButton1.Visible = true;
            LinkButton3.Visible = false;
            txtsSenha.Attributes["value"] = txtsSenha.Text;
            txtsSenha_Confirmacao.Attributes["value"] = txtsSenha_Confirmacao.Text;
        }

        protected void LinkButton4_Click(object sender, EventArgs e)
        {
            txtsSenha_Confirmacao.TextMode = TextBoxMode.Password;
            LinkButton2.Visible = true;
            LinkButton4.Visible = false;
            txtsSenha.Attributes["value"] = txtsSenha.Text;
            txtsSenha_Confirmacao.Attributes["value"] = txtsSenha_Confirmacao.Text;
        }

        protected void cmdEmpresa_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_Empresa(ref sMensagem))
            {
                // --------------- HIGOR MAESTRELLO 21/06/2024 -----------------------------
                if (ddlTodasEmpresa.SelectedValue == "999")
                {
                    List<string> listaIds = new List<string>();
                    List<string> listaDesc = new List<string>();
                    foreach (ListItem item in ddlTodasEmpresa.Items)
                    {
                        if (item.Value != "999" && item.Value != "0")
                        {
                            listaIds.Add(item.Value);
                            listaDesc.Add(item.Text);
                        }
                    }

                    for (int i = 0; i < listaIds.Count; i++)
                    {
                        string desc = listaDesc[i];
                        string id = listaIds[i].ToString();
                        cls_Usuarios_Empresa objItem = new cls_Usuarios_Empresa();
                        objItem.idLinha = bs_Empresa.Count() + 1;
                        objItem.sFuncao = "Inserir_Empresa";
                        objItem.idEmpresa = Convert.ToInt32(id);
                        objItem.sDscEmpresa = desc;
                        bs_Empresa.Add(objItem);
                    }
                }
                else
                {
                    cls_Usuarios_Empresa objItem = new cls_Usuarios_Empresa();
                    objItem.idLinha = bs_Empresa.Count() + 1;
                    objItem.sFuncao = "Inserir_Empresa";
                    objItem.idEmpresa = Convert.ToInt32(ddlTodasEmpresa.SelectedValue);
                    objItem.sDscEmpresa = ddlTodasEmpresa.SelectedItem.Text;
                    bs_Empresa.Add(objItem);
                }
                // -------------------------------------------------------------------------

                gv_Empresa_DataBind();
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "aba_empresa", "$('#aba_empresa').tab('show');", true);
        }

        protected void cmdGerarQRCode_Click(object sender, EventArgs e)
        {
            QRCoderTT gerar = new QRCoderTT();

            if (!string.IsNullOrEmpty(hddIdUsuario.Value))
            {
                int idUsuario = Convert.ToInt32(hddIdUsuario.Value);
                // Gera os bytes do QR Code.
                byte[] qrCodeBytes = gerar.GerarQrCodeUsuario(idUsuario, "");

                // Converte os bytes para string Base64.
                string base64 = Convert.ToBase64String(qrCodeBytes);

                Dictionary<string, string> parametros = new Dictionary<string, string>
                {
                    { "sFuncao", "SALVAR-QR-CODE" },
                    { "idUsuario", idUsuario.ToString() },
                    { "vbQRCode", base64 }
                };
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", parametros, true);

                if (BD.ValidarDataSet(ds))
                {
                    PopularQRCode(ds);
                    MensagemPaginaQr.MostraMensagem_Sucesso("QR Code gerado e salvo com sucesso!");
                }
                else
                    MensagemPaginaQr.MostraMensagem_Erro("Falha ao salvar o QR Code.");
            }
            else
                MensagemPaginaQr.MostraMensagem_Erro("Nenhum usuário encontrado para gerar QR Code.");
        }

        protected void cmdLoginUsuario_DEV_Click(object sender, EventArgs e)
        {
            Session["sLoginVia"] = null;
            IDENTITY.Usuario.Logoff();
            IDENTITY.Usuario.Login_Usuario(txtsLogin.Text, txtsSenha.Text, out _);
            try { IDENTITY.Usuario.Login_Sessao(Session["sChaveSessao"].ToString(), null, false); } catch { }
        }

        #endregion

        #region | Script

        protected string RetornarScripts()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //Mensagens de Confirmação
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
            sb.Append("$v192('[id*=cphCorpo_cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Excluir\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_EXCLUIR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=cphCorpo_cmdExcluir]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Excluir').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Logoff\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_LOGOFF\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=cphCorpo_cmdForcarLogoff]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Logoff').dialog('open');");
            sb.Append("});");

            sb.Append("});");

            sb.Append("$(function() {$('[id*=txtdtValidade]').datepicker({");
            sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

            return sb.ToString();
        }

        void RegistraScript()
        {
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page);

            ddlsVendedor.Text = "Vendedor";
            ddlsComprador.Text = "Comprador";
            ddlidPerfil_SelectedIndexChanged(null, null);

            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddShowModalScript", RetornarScripts(), true);
        }

        #endregion
    }
}