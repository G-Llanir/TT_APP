using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using TT.FrameWork;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Atividades
{
    public partial class Projetos_Detalhe : Page
    {
        #region | Propriedades

        static string sTituloPagina = "Projeto";
        static string sProcedure = "sp_Manipula_tbl_Flow_Atividades";

        static int nTabela_Projeto = 0;
        static int nTabela_Deptos = 1;
        static int nTabela_Usuarios = 2;
        static int nTabela_Historico = 3;

        static int nTabela_Detalhe_Atividades = 0;
        static int nTabela_Detalhe_Atividades_Usuarios = 1;
        static int nTabela_Detalhe_Apontamentos = 2;
        static int nTabela_Detalhe_Apontamentos_Historico = 3;
        static int nTabela_Detalhe_Deptos = 4;
        static int nTabela_Detalhe_Usuarios = 5;

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-Projetos.pdf";

            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    ValidaPermissao(Permissao.Atividades.Projetos.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    ValidaPermissao(Permissao.Atividades.Projetos.Incluir, true);
                    Pesquisar("0");
                }

                Scripts.FocusScript(Page, txtsTitulo.ClientID);
            }

            if (hddidProjeto.Value.Equals("0")) manual.sConfig_Guia = manual.ConfigurarGuia(GerarConfig_Manual(true));
            //else manual.sConfig_Introducao = manual.ConfigurarIntroducao(GerarConfig_Manual(false));

            MensagemPagina.MostraMensagem_Sucesso(".", false);
            Mensagem_gvAtividades.MostraMensagem_Sucesso(".", false);
            Mensagem_IncluirAtividade.MostraMensagem_Sucesso(".", false);
            Mensagem_Modal_EditarAtividade.MostraMensagem_Sucesso(".", false);
            Mensagem_Modal_Apontamento.MostraMensagem_Sucesso(".", false);
            Mensagem_Modal_IncluirAtividade.MostraMensagem_Sucesso(".", false);
        }

        protected void Pesquisar(string idPesquisa)
        {
            try
            {
                string filtroDepto = !ValidaPermissao(Permissao.Atividades.Projetos.Nivel_Diretor, false) ? $"'Departamentos_x_Usuarios', {Variaveis.idUsuario()}" : "'Flow_Departamentos'";

                Popula_Combo(ddlEmpresa, $"sp_Select 'Flow_Empresa'", "idEmpresa", "sDscCodigoEmpresa", false, "Selecione uma Empresa", "0");
                Popula_Combo(ddlDepartamento, $"sp_Select {filtroDepto}", "idDepartamento", "sDscDepartamento", false);

                hddidUsuario.Value = Variaveis.idUsuario();
                hddsDscUsuario.Value = Variaveis.sUsuarioLogado();

                if (idPesquisa != "0")
                {
                    cmdSalvar.Visible = ValidaPermissao(Permissao.Atividades.Projetos.Alterar);

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idProjeto", idPesquisa }
                    };
                    DataSet ds = ExecutarDataSet(sProcedure, vParametros);

                    if (ValidarDataSet(ds, out string sErro))
                    {
                        PainelAtualizacao.Visible = true;

                        hddidProjeto.Value = DATASET(ds, "idProjeto");
                        txtidProjeto.Text = hddidProjeto.Value;
                        txtsTitulo.Text = DATASET(ds, "sDscTitulo");
                        txtsDescricao.Value = DATASET(ds, "sDscDescricao");
                        txtnHoras_Previsao.Text = DATASET(ds, "nHoras_Previsao");
                        ddlEmpresa.SelectedValue = DATASET(ds, "idEmpresa");

                        string deptos = "";
                        foreach (DataRow row in ds.Tables[nTabela_Deptos].Rows)
                        {
                            try
                            {
                                deptos += row["idDepartamento"].ToString() + ",";
                                ddlDepartamento.Items.FindByValue(row["idDepartamento"].ToString()).Selected = true;
                            }
                            catch { }
                        }
                        deptos = deptos.TrimEnd(',').Trim();

                        Popula_Combo(ddlAdm, $"sp_Select 'Usuarios_x_Departamentos__Projetos', @sPesquisa='{deptos}', @idUsuario={Variaveis.idUsuario()}", "idUsuario", "sDscUsuario", false);
                        Popula_Combo(ddlMembros, $"sp_Select 'Usuarios_x_Departamentos__Projetos', @sPesquisa='{deptos}', @idUsuario={Variaveis.idUsuario()}", "idUsuario", "sDscUsuario", false);

                        frmArquivos.Attributes.Add("src", string.Format("../Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idPesquisa, "Projetos"));

                        PainelAtualizacao.Atualizar(DATASET(ds, "dtAtualizacao"), DATASET(ds, "sDscUsuarioAtualizacao"));
                        lblTituloPagina.Text = string.Format("{0} {1}", sTituloPagina, txtsTitulo.Text);

                        foreach (DataRow row in ds.Tables[nTabela_Usuarios].Rows)
                        {
                            try
                            {
                                ListBox ddl = row["idTipo"].ToString().Equals("1") ? ddlAdm : ddlMembros;
                                ddl.Items.FindByValue(row["idUsuario"].ToString()).Selected = true;
                            }
                            catch { }
                        }

                        Popular_Aba_Historico(ds.Tables[nTabela_Historico]);
                    }
                    else throw new Exception(sErro);
                }
                else
                {
                    aba_Arquivos.Visible = false;
                    aba_Historico.Visible = false;
                    PainelAtualizacao.Visible = false;

                    ddlEmpresa.RemoveAttribute("disabled");
                    ddlDepartamento.RemoveAttribute("disabled");

                    hddidProjeto.Value = "0";
                    txtnHoras_Previsao.Text = "0,00";
                    lblTituloPagina.Text = $"Novo {sTituloPagina}";
                    BreadCrumb.TitulodaPagina = "Incluir";
                    txtidProjeto.Text = "Novo";
                    cmdSalvar.Text = "Incluir";

                    Scripts.FocusScript(Page, txtsTitulo.ClientID);
                }

                gvAtividades_Master.DataSource = new List<int> { 0 };
                gvAtividades_Master.DataBind();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("<b>Erro: </b>Erro ao Consultar os Dados do Projeto!<br />Erro na Consulta: " + ex.Message);
            }
        }

        #endregion

        #region | Manual - Modo Guia

        public string GerarConfig_Manual(bool bNovoProjeto)
        {
            string sConfig;

            if (bNovoProjeto)
            {
                sConfig = $@"
                    {manual.ConstruirPasso("Introdução", @"Bem-vindo ao Modo Guia, aqui você será orientado pelos fluxos e campos da Página, visitando e conhecendo suas funções.
                                            <br /><br /><b>Obs:</b> o ideal é que este Modo seja utilizado antes de realizar alterações ou preencher os campos da Página.")}
                    , {manual.ConstruirPasso("Cadastro Inicial", @"Estes são os campos do Cadastro Inicial de um Novo Projeto.<br />É necessário preenchê-los antes de prosseguir com o Guia.
                                                <br /><br /><b>Obs:</b> também será necessário preencher a Descrição do Projeto.", "#cadastroInicial", "bottom", classeTooltip: "semBotoes")}
                    , {manual.ConstruirPasso("Atividades", @"Nesta seção será possível cadastrar as Atividades do Projeto.<br />Após cadastradas, as Atividades serão exibidas aqui em formato de lista.
                                                <br /><br />Se deseja prosseguir com o Guia e conhecer as funções das Atividades, clique no botão para Incluir Atividade.<br /><br /><b>Obs:</b> Não é necessário criar Atividades para cadastrar um Novo Projeto.",
                                                $"#{div_Atividades.ClientID}", "top", classeTooltip: "semBotoes")}
                    , {manual.ConstruirPasso("Nova Atividade", @"Nesta Tela são cadastradas as informações para Criar uma Nova Atividade, destes campos as <u>Horas e Datas são opcionais</u>.
                                                <br /><br />Após preencher os campos, basta clicar em <b>Incluir</b> para prosseguir com o Guia.",
                                                "#modalIncluirAtividade .modal-content", "left", classeTooltip: "semBotoes")}
                    , {manual.ConstruirPasso("Composição e Edição", @"Aqui podemos ver as Atividades criadas e também é possível utilizar a função de Composição (1ª coluna) para visualizar o que compõe cada Atividade.<br />
                                                <br /><br />Para prosseguir com o Guia, clique no botão para <b>Editar Atividade</b> (última coluna) em qualquer Atividade!", $"#{div_gvAtividades.ClientID}", "top", classeTooltip: "semBotoes")}
                    , {manual.ConstruirPasso("Edição e Apontamentos", @"Nesta Tela é possível alterar as informações das Atividades e/ou seu Status.<br />
                                                Em uma Atividade com Status <b>Não Iniciada</b>, utilizando o botão de <b>Ação</b> será possível Iniciar ou Excluir a Atividade.<br /><br />
                                                <b>Obs: </b>as opções dentro do botão de <b>Ação</b> são definidas de acordo com o Status Atual da Atividade, então por exemplo se uma Atividade estiver Em Andamento, teremos as opções de Pausar e Finalizar.
                                                <br /><br />Para prosseguir com o Guia, clique no botão para <b>Cancelar</b>!",
                                                "#modalEditarAtividade .modal-content", "left", classeTooltip: "semBotoes")}
                    , {manual.ConstruirPasso("Apontamentos", @"Além das Funções apresentadas também existem as funções de Apontamentos de Horas, para criar um Apontamento é necessário ter uma Atividade com Status <b>Em Andamento</b>.<br />
                                                Nesta Atividade, haverá um botão de <b>Novo Apontamento</b> ao lado do botão de <b>Editar Atividade</b> (ambos na última coluna da lista de Atividades), e clicando nele será exibida uma Tela simples
                                                para cadastro do Apontamento de Horas. Com base nestes Apontamentos serão calculadas as Horas Realizadas e Disponíveis das Atividades!
                                                <br /><br /><b>Obs:</b> os Apontamentos de Horas, assim como o histórico de alterações nas Atividades, são exibidos dentro da Composição de cada Atividade!.")}
                    , {manual.ConstruirPasso("Fim do Guia", @"Isto conclui o Modo Guia da Página de Novos Projetos.<br />Para mais informações sobre as Funções desta Página, verifique o Manual.<br /><br />
                                                Lembre-se sempre que para registrar toda e qualquer alteração feita nesta Página, é necessário utilizar o botão de <b>Salvar</b>!", $"#{cmdSalvar.ClientID}", "top")}
                ";

                manual.sEvento_BeforeChange = $"if ($('.semBotoes') && $('.semBotoes').length) $('#{cmdSalvar.ClientID}').focus();";
            }
            else
            {
                sConfig = $@"
                    {manual.ConstruirPasso("Visão Geral", "Esta é a Página de <b>Projetos</b>, onde é possível consultar e cadastrar Projetos, Atividades e Apontamentos.")}
                    , {manual.ConstruirPasso("Filtros", "Utilizando estes campos é possível filtrar a consulta dos dados.", "painelFiltros")}
                ";
            }

            return sConfig;
        }

        #endregion

        #region | Aba Histórico

        void Popular_Aba_Historico(DataTable dt)
        {
            gv_Historico.DataSource = dt;
            gv_Historico.DataBind();

            aba_Historico.Visible = gv_Historico.Rows.Count > 0;
        }

        protected void gv_Historico_RowDataBound(object sender, GridViewRowEventArgs e) => e.Row.Cells[1].Text = HttpUtility.HtmlDecode(e.Row.Cells[1].Text);

        #endregion

        #region | WebMethods

        [WebMethod]
        public static object Get_PopulaInfo_Projeto(string sidProjeto)
        {
            List<cls_Atividades> list_Atividades = new List<cls_Atividades>();
            List<cls_Apontamentos> list_Apontamentos = new List<cls_Apontamentos>();
            List<cls_Apontamentos_Historico> list_Apontamentos_Historico = new List<cls_Apontamentos_Historico>();
            Dictionary<string, List<string>> dict_Associados = new Dictionary<string, List<string>>();
            Dictionary<string, string> dict_Deptos = new Dictionary<string, string>();
            Dictionary<string, string> dict_Adm = new Dictionary<string, string>();
            Dictionary<string, string> dict_Membros = new Dictionary<string, string>();
            bool bAdm = false;
            bool bMembro = false;

            if (!string.IsNullOrEmpty(sidProjeto) && sidProjeto != "0")
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE_ATIVIDADES" },
                    { "@idProjeto", sidProjeto }
                };
                DataSet ds = ExecutarDataSet(sProcedure, vParametros);

                // Atividades
                foreach (DataRow row in ds.Tables[nTabela_Detalhe_Atividades].Rows)
                {
                    int.TryParse(row["idAtividade"].ToString(), out int idAtividade);
                    int.TryParse(row["idAtividadePai"].ToString(), out int idAtividadePai);
                    int.TryParse(row["idProjeto"].ToString(), out int idProjeto);
                    int.TryParse(row["idStatus"].ToString(), out int idStatus);
                    decimal.TryParse(row["nHoras_Previsao"].ToString(), out decimal nHoras_Previsao);
                    DateTime.TryParse(row["dtInicial"].ToString(), out DateTime dtInicial);
                    DateTime.TryParse(row["dtInicial_Previsao"].ToString(), out DateTime dtInicial_Previsao);
                    DateTime.TryParse(row["dtFinal"].ToString(), out DateTime dtFinal);
                    DateTime.TryParse(row["dtFinal_Previsao"].ToString(), out DateTime dtFinal_Previsao);

                    cls_Atividades atividade = new cls_Atividades
                    {
                        idAtividade = idAtividade,
                        idAtividadePai = idAtividadePai,
                        idProjeto = idProjeto,
                        idStatus = idStatus,
                        sDscTitulo = row["sDscTitulo"].ToString(),
                        sDscDescricao = row["sDscDescricao"].ToString(),
                        nHoras_Previsao = nHoras_Previsao,
                        dtInicial = dtInicial,
                        dtInicial_Previsao = dtInicial_Previsao,
                        dtFinal = dtFinal,
                        dtFinal_Previsao = dtFinal_Previsao,
                        bAtividadePai = row["sAtividadePai"].ToString().Trim().ToUpper() == "S"
                    };
                    list_Atividades.Add(atividade);
                }

                // Usuários Associados - Atividades
                foreach (DataRow row in ds.Tables[nTabela_Detalhe_Atividades_Usuarios].Rows)
                {
                    if (dict_Associados.ContainsKey(row["idAtividade"].ToString()))
                        dict_Associados[row["idAtividade"].ToString()].Add(row["idUsuario"].ToString());
                    else
                        dict_Associados.Add(row["idAtividade"].ToString(), new List<string> { row["idUsuario"].ToString() });
                }

                // Apontamentos
                foreach (DataRow row in ds.Tables[nTabela_Detalhe_Apontamentos].Rows)
                {
                    int.TryParse(row["idApontamento"].ToString(), out int idApontamento);
                    int.TryParse(row["idAtividade"].ToString(), out int idAtividade);
                    int.TryParse(row["idTipo"].ToString(), out int idTipo);
                    int.TryParse(row["idUsuario"].ToString(), out int idUsuario);
                    decimal.TryParse(row["nHoras"].ToString(), out decimal nHoras);
                    DateTime.TryParse(row["dtApontamento"].ToString(), out DateTime dtApontamento);

                    cls_Apontamentos atividade = new cls_Apontamentos
                    {
                        idApontamento = idApontamento,
                        idAtividade = idAtividade,
                        idTipo = idTipo,
                        sObservacao = row["sObservacao"].ToString(),
                        nHoras = nHoras,
                        dtApontamento = dtApontamento,
                        idUsuario = idUsuario,
                        sDscUsuario = row["sDscUsuario"].ToString(),
                        bSalva = false
                    };

                    list_Apontamentos.Add(atividade);
                }

                // Apontamentos - Histórico
                foreach (DataRow row in ds.Tables[nTabela_Detalhe_Apontamentos_Historico].Rows)
                {
                    int.TryParse(row["idHistorico"].ToString(), out int idHistorico);
                    int.TryParse(row["idApontamento"].ToString(), out int idApontamento);
                    int.TryParse(row["idUsuario"].ToString(), out int idUsuario);
                    DateTime.TryParse(row["dtAlteracao"].ToString(), out DateTime dtAlteracao);

                    cls_Apontamentos_Historico historico = new cls_Apontamentos_Historico
                    {
                        idHistorico = idHistorico,
                        idApontamento = idApontamento,
                        sDscMotivo = row["sDscMotivo"].ToString(),
                        sDscAlteracao = row["sDscAlteracao"].ToString(),
                        idUsuario = idUsuario,
                        sDscUsuario = row["sDscUsuario"].ToString(),
                        dtAlteracao = dtAlteracao
                    };
                    list_Apontamentos_Historico.Add(historico);
                }

                // Departamentos - Projeto
                foreach (DataRow row in ds.Tables[nTabela_Detalhe_Deptos].Rows)
                {
                    string idDepto = row["idDepartamento"].ToString();

                    if (dict_Deptos.ContainsKey(idDepto))
                        dict_Deptos[idDepto] = row["sDscDepartamento"].ToString();
                    else
                        dict_Deptos.Add(idDepto, row["sDscDepartamento"].ToString());
                }

                // Usuários Associados - Projeto
                foreach (DataRow row in ds.Tables[nTabela_Detalhe_Usuarios].Rows)
                {
                    string idUsuario = row["idUsuario"].ToString();
                    bool adm = row["idTipo"].ToString().Equals("1");
                    bool membro = row["idTipo"].ToString().Equals("2");

                    if (adm)
                    {
                        if (dict_Adm.ContainsKey(idUsuario))
                            dict_Adm[idUsuario] = row["sDscUsuario"].ToString();
                        else
                            dict_Adm.Add(idUsuario, row["sDscUsuario"].ToString());

                        if (idUsuario.Equals(Variaveis.idUsuario()))
                            bAdm = true;
                    }
                    else if (membro)
                    {
                        if (dict_Membros.ContainsKey(idUsuario))
                            dict_Membros[idUsuario] = row["sDscUsuario"].ToString();
                        else
                            dict_Membros.Add(idUsuario, row["sDscUsuario"].ToString());

                        if (idUsuario.Equals(Variaveis.idUsuario()))
                            bMembro = true;
                    }
                }
            }
            else
            {
                dict_Adm.Add(Variaveis.idUsuario(), Variaveis.sUsuarioLogado());
                bAdm = true;
            }

            return new
            {
                atividades = list_Atividades,
                associados = dict_Associados,
                apontamentos = list_Apontamentos,
                apontamentos_Historico = list_Apontamentos_Historico,
                deptos = dict_Deptos,
                administradores = dict_Adm,
                membros = dict_Membros,
                associaUsuarios = ValidaPermissao(Permissao.Atividades.Projetos.AssociarUsuarios),
                bAdm,
                bMembro,
                nNivel_Permissao = ValidaPermissao(Permissao.Atividades.Projetos.Nivel_Diretor) ? 3 : ValidaPermissao(Permissao.Atividades.Projetos.Nivel_Gestor) ? 2 : 1
            };
        }

        [WebMethod]
        public static object Post_SalvarDados(int idProjeto, int idEmpresa, int idUsuario, Dictionary<string, string> deptos, string titulo, string descricao, string horas, Dictionary<string, string> adms, Dictionary<string, string> membros,
            List<cls_Atividades> atividades, Dictionary<string, List<int>> usuariosAssociados, List<cls_Apontamentos> apontamentos, List<cls_Apontamentos_Historico> apontamentos_Historico)
        {
            try
            {
                string sErro = string.Empty;
                bool bGera_Log = idProjeto != 0;

                // Salvar Projeto
                {
                    Dictionary<string, string> vParam = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_PROJETO" },
                        { "@idProjeto", idProjeto.ToString() },
                        { "@idEmpresa", idEmpresa.ToString() },
                        { "@sDscTitulo", titulo.Trim() },
                        { "@sDscDescricao", descricao.Trim() },
                        { "@nHoras_Previsao", horas.Replace(".", "").Replace(",", ".") },
                        { "@idUsuarioAtualizacao", idUsuario.ToString() }
                    };
                    DataSet ds = ExecutarDataSet(sProcedure, vParam);

                    if (ValidarDataSet(ds, out sErro))
                    {
                        string sDscAtividade = Regex.Replace(descricao, "<.*?>", string.Empty);
                        sDscAtividade = HttpUtility.HtmlDecode(sDscAtividade);

                        if (idProjeto == 0 && atividades.Count <= 0)
                        {
                            atividades.Add(new cls_Atividades { idAtividade = 1, idAtividadePai = 0, idStatus = 1, sDscTitulo = titulo, sDscDescricao = string.Empty, nHoras_Previsao = decimal.Parse(horas), dtInicial = null, dtInicial_Previsao = null, dtFinal = null, dtFinal_Previsao = null });
                            apontamentos.Add(new cls_Apontamentos { idApontamento = 1, idAtividade = 1, idTipo = 1, nHoras = 0, idUsuario = idUsuario, sObservacao = "Atividade Incluída.", dtApontamento = DateTime.Now, bSalva = true });
                            usuariosAssociados.Add("1", membros.Count > 0 ? membros.Select(m => int.Parse(m.Key)).ToList() : new List<int> { idUsuario });
                        }

                        int.TryParse(DATASET(ds, "idProjeto"), out idProjeto);
                    }
                    else if (!string.IsNullOrEmpty(sErro))
                        throw new Exception("Erro ao Salvar o Projeto: " + sErro);
                }

                // Salvar Projeto - Departamentos
                {
                    Dictionary<string, string> vParametros_Deptos = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_PROJETO_DEPTOS" },
                        { "@idProjeto", idProjeto.ToString() },
                        { "@deptos", string.Join("|", deptos.Select(d => d.Key)) },
                        { "@idUsuarioAtualizacao", idUsuario.ToString() },
                        { "@sGera_Log", bGera_Log ? "S" : "N" }
                    };
                    DataSet ds_Deptos = ExecutarDataSet(sProcedure, vParametros_Deptos);

                    if (!ValidarDataSet(ds_Deptos, out sErro) && !string.IsNullOrEmpty(sErro))
                        throw new Exception("Erro ao Salvar os Departamentos do Projeto: " + sErro);
                }

                // Salvar Projeto - Usuários
                {
                    Dictionary<string, string> vParametros_Usuarios = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_PROJETO_USUARIOS" },
                        { "@idProjeto", idProjeto.ToString() },
                        { "@adms", string.Join("|", adms.Select(a => a.Key)) },
                        { "@membros", string.Join("|", membros.Select(m => m.Key)) },
                        { "@idUsuarioAtualizacao", idUsuario.ToString() },
                        { "@sGera_Log", bGera_Log ? "S" : "N" }
                    };
                    DataSet ds_Usuarios = ExecutarDataSet(sProcedure, vParametros_Usuarios);

                    if (!ValidarDataSet(ds_Usuarios, out sErro) && !string.IsNullOrEmpty(sErro))
                        throw new Exception("Erro ao Salvar os Usuários vinculados ao Projeto: " + sErro);
                }

                // Salvar Atividades
                foreach (var atividade in atividades)
                {
                    if (idProjeto == 0 && atividade.idStatus == 4)
                        continue;

                    Dictionary<string, string> vParametros_Atividades = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_ATIVIDADES" },
                        { "@idProjeto", idProjeto.ToString() },
                        { "@idAtividade", atividade.idAtividade.ToString() },
                        { "@idAtividadePai", atividade.idAtividadePai.ToString() },
                        { "@idStatus", atividade.idStatus.ToString() },
                        { "@sDscTitulo", atividade.sDscTitulo.Trim() },
                        { "@sDscDescricao", atividade.sDscDescricao.Trim() },
                        { "@nHoras_Previsao", atividade.nHoras_Previsao.HasValue ? atividade.nHoras_Previsao.Value.ToString().Replace(",", ".") : "0" },
                        { "@dtInicial", atividade.dtInicial.ToString() },
                        { "@dtInicial_Previsao", atividade.dtInicial_Previsao.ToString() },
                        { "@dtFinal", atividade.dtFinal.ToString() },
                        { "@dtFinal_Previsao", atividade.dtFinal_Previsao.ToString() },
                        { "@sAtividadePai", atividade.bAtividadePai ? "S" : "N" }
                    };
                    DataSet ds_Atividades = ExecutarDataSet(sProcedure, vParametros_Atividades);

                    if (ValidarDataSet(ds_Atividades, out sErro))
                    {
                        int.TryParse(DATASET(ds_Atividades, "idAtividade"), out int idAtividade);

                        if (atividade.idAtividade != idAtividade)
                        {
                            foreach (var micro in atividades.Where(a => a.idAtividadePai == atividade.idAtividade))
                            {
                                micro.idAtividadePai = idAtividade;
                            }

                            foreach (var apontamento in apontamentos.Where(a => a.idAtividade == atividade.idAtividade))
                            {
                                apontamento.idAtividade = idAtividade;
                            }
                        }

                        if (usuariosAssociados.ContainsKey(atividade.idAtividade.ToString()))
                        {
                            Dictionary<string, string> vParametros_Atividades_Usuarios = new Dictionary<string, string>
                            {
                                { "@sFuncao", "SALVAR_ATIVIDADES_USUARIOS" },
                                { "@idAtividade", idAtividade.ToString() },
                                { "@usuarios", string.Join("|", usuariosAssociados[atividade.idAtividade.ToString()]) }
                            };
                            DataSet ds_Atividades_Usuarios = ExecutarDataSet(sProcedure, vParametros_Atividades_Usuarios);

                            if (!ValidarDataSet(ds_Atividades_Usuarios, out sErro) && !string.IsNullOrEmpty(sErro))
                                throw new Exception("Erro ao Salvar os Usuários associados às Atividades: " + sErro);
                        }

                        atividade.idAtividade = idAtividade;
                    }
                    else if (!string.IsNullOrEmpty(sErro))
                        throw new Exception("Erro ao Salvar as Atividades: " + sErro);
                }

                // Salvar Apontamentos
                foreach (var apontamento in apontamentos)
                {
                    if (!apontamento.bSalva)
                        continue;

                    var atividade = atividades.FirstOrDefault(a => a.idAtividade == apontamento.idAtividade);

                    if (idProjeto == 0 && atividade.idStatus == 4)
                        continue;

                    Dictionary<string, string> vParametros_Apontamentos = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_APONTAMENTOS" },
                        { "@idProjeto", idProjeto.ToString() },
                        { "@idApontamento", apontamento.idApontamento.ToString() },
                        { "@idAtividade", apontamento.idAtividade.ToString() },
                        { "@idStatus", atividade.idStatus.ToString() },
                        { "@idTipo", apontamento.idTipo.ToString() },
                        { "@sObservacao", apontamento.sObservacao.Trim() },
                        { "@nHoras", apontamento.nHoras.ToString().Replace(",", ".") },
                        { "@dtApontamento", apontamento.dtApontamento.ToString() },
                        { "@idUsuario", apontamento.idUsuario.ToString() }
                    };
                    DataSet ds_Apontamentos = ExecutarDataSet(sProcedure, vParametros_Apontamentos);

                    if (ValidarDataSet(ds_Apontamentos, out sErro))
                    {
                        int.TryParse(DATASET(ds_Apontamentos, "idApontamento"), out int idApontamento);

                        foreach (var historico in apontamentos_Historico.Where(h => h.idApontamento == apontamento.idApontamento))
                        {
                            Dictionary<string, string> vParametros_Apontamentos_Historico = new Dictionary<string, string>
                            {
                                { "@sFuncao", "SALVAR_APONTAMENTOS_HISTORICO" },
                                { "@idHistorico", historico.idHistorico.ToString() },
                                { "@idApontamento", idApontamento.ToString() },
                                { "@sDscMotivo", historico.sDscMotivo.Trim() },
                                { "@sDscAlteracao", historico.sDscAlteracao.Trim() },
                                { "@idUsuario", historico.idUsuario.ToString() }
                            };
                            ExecutarDataSet(sProcedure, vParametros_Apontamentos_Historico);
                        }
                    }
                    else if (!string.IsNullOrEmpty(sErro))
                        throw new Exception("Erro ao Salvar os Apontamentos: " + sErro);
                }
            }
            catch (Exception ex)
            {
                return new { bSucesso = false, sMensagem = ex.Message, idProjeto };
            }

            return new { bSucesso = true, sMensagem = string.Empty, idProjeto };
        }

        [WebMethod]
        public static object Get_PopulaCombos_Usuarios(Dictionary<string, string> deptos)
        {
            Dictionary<string, string> dict_Usuarios = new Dictionary<string, string>();
            List<string> list_gestores = new List<string>();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Usuarios_x_Departamentos__Projetos" },
                { "@deptos", string.Join(",", deptos.Keys) },
                { "@idUsuario", Variaveis.idUsuario() }
            };
            DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Atividades", vParametros);

            foreach (DataRow row in tb.Rows)
            {
                dict_Usuarios.Add(row["idUsuario"].ToString(), row["sDscUsuario"].ToString());

                if (row["sGestor"].ToString().Trim().ToUpper() == "S")
                    list_gestores.Add(row["idUsuario"].ToString());
            }

            return new
            {
                usuarios = dict_Usuarios,
                gestores = list_gestores
            };
        }

        #endregion
    }
}
