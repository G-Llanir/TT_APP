using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TT_Flow.FrameWork.IA;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.IA
{
    public partial class Workflow_Detalhe : System.Web.UI.Page
    {
        private readonly cls_IA_Repositorio _repositorio = new cls_IA_Repositorio();
        private static readonly Regex _rgxNome = new Regex(@"^[a-zA-Z0-9_-]{1,64}$", RegexOptions.Compiled);

        // JSON do catálogo de ferramentas (nome/escopo/params), injetado no JS do builder.
        protected string CatalogoJson = "[]";

        public class WorkflowTesteResponse
        {
            public bool Sucesso { get; set; }
            public string Status { get; set; }
            public string TipoPausa { get; set; }
            public string DadosPausaJson { get; set; }
            public string Mensagem { get; set; }
            public object Trace { get; set; }
            public string SaidasJson { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.IA.AdministrarWorkflows, true);
            CatalogoJson = MontarCatalogoJson();

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlRecurso, "sp_Select 'Recursos'", "idRecurso", "sDscRecurso", true, "Selecione a permissão", "0");

                int id = ParaInt(Request.QueryString["id"]);
                if (id > 0)
                {
                    CarregarWorkflow(id);

                    string avisosSalvos = Session["wfAvisosGrafo"] as string;
                    if (!string.IsNullOrEmpty(avisosSalvos))
                    {
                        Session.Remove("wfAvisosGrafo");
                        MensagemPagina.MostraMensagem_Aviso("Workflow salvo. Pontos de atenção no grafo:<br>" + avisosSalvos, false);
                    }
                }
                else
                {
                    PrepararNovo();
                }
            }
        }

        private void PrepararNovo()
        {
            hddId.Value = "0";
            lblTituloPagina.Text = "Novo workflow";
            BreadCrumb_Pagina.TitulodaPagina = "Novo workflow";
            txtNome.ReadOnly = false;
            Selecionar(ddlEscopoWorkflow, "READ");
            Selecionar(ddlAtivo, "N");
            // Grafo inicial mínimo: um início e um fim, sem passos.
            hddGrafo.Value = "{\"versao\":2,\"entradas\":[],\"nos\":[{\"id\":\"n1\",\"tipo\":\"inicio\",\"proximo\":\"nFim\",\"x\":80,\"y\":230},{\"id\":\"nFim\",\"tipo\":\"fim\",\"x\":390,\"y\":230}]}";
        }

        private void CarregarWorkflow(int id)
        {
            IAWorkflow wf = _repositorio.ObterWorkflow(id);
            if (wf == null)
            {
                MensagemPagina.MostraMensagem_Erro("Workflow não localizado.");
                cmdSalvar.Visible = false;
                return;
            }

            hddId.Value = wf.IdWorkflowIA.ToString();
            lblTituloPagina.Text = "Workflow: " + wf.Nome;
            BreadCrumb_Pagina.TitulodaPagina = wf.Nome;
            txtNome.Text = wf.Nome;
            txtNome.ReadOnly = true; // Nome interno é imutável (chave de execução).
            txtDescricao.Text = wf.Descricao;
            Selecionar(ddlEscopoWorkflow, NormalizarEscopoWorkflow(wf.Escopo));
            Selecionar(ddlRecurso, wf.IdRecursoNecessario.ToString());
            Selecionar(ddlAtivo, wf.Ativo ? "S" : "N");
            hddGrafo.Value = string.IsNullOrWhiteSpace(wf.GrafoJson)
                ? "{\"entradas\":[],\"nos\":[{\"id\":\"n1\",\"tipo\":\"inicio\",\"proximo\":\"nFim\"},{\"id\":\"nFim\",\"tipo\":\"fim\"}]}"
                : wf.GrafoJson;
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            int id = ParaInt(hddId.Value);
            bool novo = id == 0;
            string nome = (txtNome.Text ?? string.Empty).Trim();

            if (novo && !_rgxNome.IsMatch(nome))
            {
                MensagemPagina.MostraMensagem_Erro("Nome interno inválido. Use apenas letras, números, underline ou hífen (sem espaço/acento).");
                return;
            }

            int idRecurso = ParaInt(ddlRecurso.SelectedValue);
            if (idRecurso <= 0)
            {
                MensagemPagina.MostraMensagem_Erro("Selecione a permissão (recurso) necessária para o workflow.");
                return;
            }

            string erroFerramentas = ValidarFerramentasDoGrafo(hddGrafo.Value);
            if (!string.IsNullOrWhiteSpace(erroFerramentas))
            {
                MensagemPagina.MostraMensagem_Erro(erroFerramentas);
                return;
            }

            // Referências entre nós: erro bloqueia o salvamento; aviso salva e aparece na tela depois do redirecionamento.
            List<IAWorkflowAvisoGrafo> achados = cls_IA_WorkflowValidador.Validar(hddGrafo.Value, cls_IA_ToolRegistry.Obter);
            List<IAWorkflowAvisoGrafo> errosGrafo = achados.FindAll(delegate (IAWorkflowAvisoGrafo a) { return a.Nivel == "ERRO"; });
            if (errosGrafo.Count > 0)
            {
                MensagemPagina.MostraMensagem_Erro("Não é possível salvar o workflow:<br>" + FormatarAvisosGrafo(errosGrafo));
                return;
            }
            List<IAWorkflowAvisoGrafo> avisosGrafo = achados.FindAll(delegate (IAWorkflowAvisoGrafo a) { return a.Nivel == "AVISO"; });

            IAWorkflow wf = new IAWorkflow
            {
                IdWorkflowIA = id,
                Nome = nome,
                Descricao = (txtDescricao.Text ?? string.Empty).Trim(),
                IdRecursoNecessario = idRecurso,
                Escopo = NormalizarEscopoWorkflow(ddlEscopoWorkflow.SelectedValue),
                Ativo = ddlAtivo.SelectedValue == "S",
                GrafoJson = string.IsNullOrWhiteSpace(hddGrafo.Value) ? "{}" : hddGrafo.Value,
                SchemaParametrosJson = SchemaFromGrafo(hddGrafo.Value)
            };

            string mensagem;
            int salvo = _repositorio.SalvarWorkflow(wf, out mensagem);
            if (salvo > 0)
            {
                hddId.Value = salvo.ToString();
                if (avisosGrafo.Count > 0) Session["wfAvisosGrafo"] = FormatarAvisosGrafo(avisosGrafo);
                else Session.Remove("wfAvisosGrafo");
                Response.Redirect("Workflow_Detalhe.aspx?id=" + salvo.ToString(), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro(string.IsNullOrWhiteSpace(mensagem) ? "Não foi possível salvar o workflow." : mensagem);
            }
        }

        // Mensagens do validador de referências em HTML seguro (MostraMensagem escreve o texto sem codificar).
        private static string FormatarAvisosGrafo(List<IAWorkflowAvisoGrafo> lista)
        {
            const int maximo = 10;
            List<string> linhas = new List<string>();
            for (int i = 0; i < lista.Count && i < maximo; i++)
            {
                linhas.Add("&bull; " + System.Web.HttpUtility.HtmlEncode(lista[i].Mensagem));
            }
            if (lista.Count > maximo) linhas.Add("&hellip; e mais " + (lista.Count - maximo) + ".");
            return string.Join("<br>", linhas.ToArray());
        }

        public class WorkflowValidacaoResponse
        {
            public bool Sucesso { get; set; }
            public string Mensagem { get; set; }
            public List<string> Erros { get; set; }
            public List<string> Avisos { get; set; }
        }

        // Valida as referências do grafo aberto no editor, sem salvar (botão Validar).
        [WebMethod(EnableSession = true)]
        public static WorkflowValidacaoResponse ValidarGrafo(string grafoJson)
        {
            WorkflowValidacaoResponse resposta = new WorkflowValidacaoResponse { Erros = new List<string>(), Avisos = new List<string>() };
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                resposta.Mensagem = "Sem permissão para validar workflows.";
                return resposta;
            }

            foreach (IAWorkflowAvisoGrafo achado in cls_IA_WorkflowValidador.Validar(grafoJson, cls_IA_ToolRegistry.Obter))
            {
                (achado.Nivel == "ERRO" ? resposta.Erros : resposta.Avisos).Add(achado.Mensagem);
            }
            resposta.Sucesso = true;
            return resposta;
        }

        // Executa o grafo em modo teste (sem salvar). READ-only; passos WRITE pausam.
        [WebMethod(EnableSession = true)]
        public static WorkflowTesteResponse ExecutarTeste(string grafoJson, string entradasJson)
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return new WorkflowTesteResponse { Sucesso = false, Mensagem = "Sem permissão para testar workflows." };
            }

            JObject entradas;
            try { entradas = string.IsNullOrWhiteSpace(entradasJson) ? new JObject() : JObject.Parse(entradasJson); }
            catch { entradas = new JObject(); }

            IAWorkflow wf = new IAWorkflow { Nome = "(teste)", Escopo = "READ", GrafoJson = grafoJson };
            IAWorkflowResultado res = new cls_IA_WorkflowEngine().Executar(0, wf, entradas);

            return new WorkflowTesteResponse { Sucesso = res.Sucesso, Status = res.Status, TipoPausa = res.TipoPausa, DadosPausaJson = res.DadosPausaJson, Mensagem = res.Mensagem, Trace = res.Trace, SaidasJson = res.SaidaFinal != null ? res.SaidaFinal.ToString(Formatting.None) : "{}" };
        }

        public class WorkflowExportResponse
        {
            public bool Sucesso { get; set; }
            public string Mensagem { get; set; }
            public string WorkflowJson { get; set; }
        }

        // Exporta o pacote do workflow salvo (grafo + metadados) no formato aceito por ImportarWorkflow, em
        // Workflows.aspx - para versionar em SQL/IA/Workflows/ ou aplicar em outro ambiente. Le sempre o nome
        // gravado no banco (nunca o do campo em tela ainda nao salvo), e so metadados de configuracao: nenhum
        // dado de negocio.
        [WebMethod(EnableSession = true)]
        public static WorkflowExportResponse ExportarWorkflow(int idWorkflowIA)
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return new WorkflowExportResponse { Sucesso = false, Mensagem = "Sem permissão para exportar workflows." };
            }

            if (idWorkflowIA <= 0)
            {
                return new WorkflowExportResponse { Sucesso = false, Mensagem = "Salve o workflow antes de exportar." };
            }

            cls_IA_Repositorio repositorio = new cls_IA_Repositorio();
            IAWorkflow workflow = repositorio.ObterWorkflow(idWorkflowIA);
            if (workflow == null)
            {
                return new WorkflowExportResponse { Sucesso = false, Mensagem = "Workflow não encontrado." };
            }

            try
            {
                string json = repositorio.ExportarWorkflow(workflow.Nome);
                return string.IsNullOrWhiteSpace(json)
                    ? new WorkflowExportResponse { Sucesso = false, Mensagem = "Não foi possível exportar o workflow." }
                    : new WorkflowExportResponse { Sucesso = true, WorkflowJson = json };
            }
            catch (Exception ex)
            {
                return new WorkflowExportResponse { Sucesso = false, Mensagem = ex.Message };
            }
        }

        [WebMethod(EnableSession = true)]
        public static IAWorkflowExecResponse IniciarExecucao(int idWorkflowIA, string entradasJson)
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return ErroExecucao("Sem permissão para executar workflows.");
            }

            if (idWorkflowIA <= 0)
            {
                return ErroExecucao("Salve o workflow antes de executar.");
            }

            JObject entradas;
            try
            {
                entradas = string.IsNullOrWhiteSpace(entradasJson) ? new JObject() : JObject.Parse(entradasJson);
            }
            catch
            {
                return ErroExecucao("JSON de entradas inválido.");
            }

            return new cls_IA_WorkflowExecucaoService().Iniciar(idWorkflowIA, entradas);
        }

        [WebMethod(EnableSession = true)]
        public static IAWorkflowExecResponse ConfirmarExecucao(int idExecucaoIA)
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return ErroExecucao("Sem permissão para confirmar workflows.");
            }

            if (idExecucaoIA <= 0)
            {
                return ErroExecucao("Execução inválida.");
            }

            return new cls_IA_WorkflowExecucaoService().Confirmar(idExecucaoIA);
        }

        [WebMethod(EnableSession = true)]
        public static IAWorkflowExecResponse SelecionarOpcaoExecucao(int idExecucaoIA, string valor)
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return ErroExecucao("Sem permissão para selecionar opções do workflow.");
            }

            if (idExecucaoIA <= 0 || string.IsNullOrWhiteSpace(valor))
            {
                return ErroExecucao("Execução ou opção inválida.");
            }

            return new cls_IA_WorkflowExecucaoService().SelecionarOpcao(idExecucaoIA, valor, 0);
        }

        [WebMethod(EnableSession = true)]
        public static IAWorkflowExecResponse InformarEntradasExecucao(int idExecucaoIA, string entradasJson)
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return ErroExecucao("Sem permissão para informar entradas de workflow.");
            }

            if (idExecucaoIA <= 0)
            {
                return ErroExecucao("Execução inválida.");
            }

            JObject entradas;
            try
            {
                entradas = string.IsNullOrWhiteSpace(entradasJson) ? new JObject() : JObject.Parse(entradasJson);
            }
            catch
            {
                return ErroExecucao("JSON de entradas inválido.");
            }

            return new cls_IA_WorkflowExecucaoService().InformarEntradas(idExecucaoIA, entradas);
        }

        [WebMethod(EnableSession = true)]
        public static IAWorkflowExecResponse CancelarExecucao(int idExecucaoIA)
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return ErroExecucao("Sem permissão para cancelar workflows.");
            }

            if (idExecucaoIA <= 0)
            {
                return ErroExecucao("Execução inválida.");
            }

            return new cls_IA_WorkflowExecucaoService().Cancelar(idExecucaoIA);
        }

        [WebMethod(EnableSession = true)]
        public static IAWorkflowExecResponse RejeitarExecucao(int idExecucaoIA)
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return ErroExecucao("Sem permissão para rejeitar workflows.");
            }

            if (idExecucaoIA <= 0)
            {
                return ErroExecucao("Execução inválida.");
            }

            return new cls_IA_WorkflowExecucaoService().Rejeitar(idExecucaoIA);
        }

        [WebMethod(EnableSession = true)]
        public static IAWorkflowLoteResponse ProcessarEsperasAgendadas()
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return ErroLote("Sem permissão para processar esperas de workflow.");
            }

            return new cls_IA_WorkflowExecucaoService().ProcessarEsperasAgendadas(50);
        }

        [WebMethod(EnableSession = true)]
        public static IAWorkflowLoteResponse LiberarEventoEspera(string evento)
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return ErroLote("Sem permissão para liberar eventos de workflow.");
            }

            if (string.IsNullOrWhiteSpace(evento))
            {
                return ErroLote("Informe o nome do evento.");
            }

            return new cls_IA_WorkflowExecucaoService().LiberarEsperasPorEvento(evento, 50);
        }

        private static IAWorkflowExecResponse ErroExecucao(string mensagem)
        {
            return new IAWorkflowExecResponse
            {
                Status = "ERRO",
                Mensagem = mensagem,
                Trace = new List<IAWorkflowTracePasso>()
            };
        }

        private static IAWorkflowLoteResponse ErroLote(string mensagem)
        {
            return new IAWorkflowLoteResponse
            {
                Status = "ERRO",
                Mensagem = mensagem,
                Execucoes = new List<IAWorkflowExecResponse>()
            };
        }

        // Deriva um JSON schema mínimo dos inputs declarados (para a IA, na Fase 3).
        private static string SchemaFromGrafo(string grafoJson)
        {
            try
            {
                JObject grafo = JObject.Parse(grafoJson ?? "{}");
                JArray entradas = grafo["entradas"] as JArray;
                JObject props = new JObject();
                JArray required = new JArray();
                foreach (JToken t in entradas ?? new JArray())
                {
                    string nome = Convert.ToString(t["nome"]).Trim();
                    if (nome.Length == 0) continue;
                    string tipo = Convert.ToString(t["tipo"]);
                    JObject propriedade = new JObject
                    {
                        { "type", string.IsNullOrWhiteSpace(tipo) ? "string" : tipo },
                        { "description", DescricaoEntradaWorkflow(t as JObject) }
                    };
                    JArray opcoes = OpcoesEntradaWorkflow(t as JObject);
                    if (opcoes.Count > 0)
                    {
                        propriedade["enum"] = opcoes;
                    }
                    props[nome] = propriedade;

                    string obrigatoriedade = Convert.ToString(t["obrigatoriedade"]).Trim();
                    if (string.IsNullOrWhiteSpace(obrigatoriedade) || string.Equals(obrigatoriedade, "obrigatorio", StringComparison.OrdinalIgnoreCase))
                    {
                        required.Add(nome);
                    }
                }
                return new JObject { { "type", "object" }, { "properties", props }, { "required", required }, { "additionalProperties", false } }
                    .ToString(Formatting.None);
            }
            catch { return string.Empty; }
        }

        private static JArray OpcoesEntradaWorkflow(JObject entrada)
        {
            JArray valores = new JArray();
            foreach (JToken opcao in (entrada != null ? entrada["opcoes"] as JArray : null) ?? new JArray())
            {
                JObject objeto = opcao as JObject;
                string valor = Convert.ToString(objeto != null ? objeto["valor"] : opcao).Trim();
                if (valor.Length > 0)
                {
                    valores.Add(valor);
                }
            }
            return valores;
        }

        private static string ValidarFerramentasDoGrafo(string grafoJson)
        {
            try
            {
                JObject grafo = JObject.Parse(string.IsNullOrWhiteSpace(grafoJson) ? "{}" : grafoJson);
                foreach (JToken token in (grafo["nos"] as JArray) ?? new JArray())
                {
                    JObject no = token as JObject;
                    if (no == null || !string.Equals(Convert.ToString(no["tipo"]), "ferramenta", StringComparison.OrdinalIgnoreCase)) continue;
                    string nome = Convert.ToString(no["ferramenta"]);
                    string diagnostico = cls_IA_ToolRegistry.DiagnosticarDisponibilidade(nome, false);
                    if (!string.IsNullOrWhiteSpace(diagnostico)) return "Não é possível salvar o workflow: " + diagnostico;
                }
            }
            catch (Exception ex)
            {
                return "Não é possível salvar o workflow: grafo inválido. " + ex.Message;
            }
            return string.Empty;
        }

        private static string DescricaoEntradaWorkflow(JObject entrada)
        {
            if (entrada == null)
            {
                return string.Empty;
            }

            string grupo = Convert.ToString(entrada["grupo"]).Trim();
            string obrigatoriedade = Convert.ToString(entrada["obrigatoriedade"]).Trim();
            string descricao = Convert.ToString(entrada["descricao"]).Trim();

            List<string> partes = new List<string>();
            if (!string.IsNullOrWhiteSpace(grupo)) partes.Add("Grupo: " + grupo);
            if (!string.IsNullOrWhiteSpace(obrigatoriedade)) partes.Add("Uso: " + obrigatoriedade);
            if (!string.IsNullOrWhiteSpace(descricao)) partes.Add(descricao);
            return string.Join(". ", partes.ToArray());
        }

        private static string MontarCatalogoJson()
        {
            JArray arr = new JArray();
            foreach (IAFerramentaDefinicao f in cls_IA_ToolRegistry.ListarTodas())
            {
                if (f.EhWorkflow())
                {
                    continue;
                }

                JObject props = f.SchemaParametros != null ? f.SchemaParametros["properties"] as JObject : null;
                JArray required = f.SchemaParametros != null ? f.SchemaParametros["required"] as JArray : null;
                var obrigatorios = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (JToken req in required ?? new JArray()) obrigatorios.Add(Convert.ToString(req));

                JArray parametros = new JArray();
                if (props != null)
                {
                    foreach (JProperty p in props.Properties())
                    {
                        JObject definicao = p.Value as JObject ?? new JObject();
                        JToken tipo = definicao["type"];
                        string tipoTexto = "string";
                        if (tipo != null)
                        {
                            if (tipo.Type == JTokenType.Array)
                            {
                                List<string> tipos = new List<string>();
                                foreach (JToken itemTipo in (JArray)tipo)
                                {
                                    tipos.Add(Convert.ToString(itemTipo));
                                }
                                tipoTexto = string.Join("|", tipos.ToArray());
                            }
                            else
                            {
                                tipoTexto = tipo.ToString();
                            }
                        }

                        JObject parametro = new JObject
                        {
                            { "nome", p.Name },
                            { "tipo", tipoTexto },
                            { "obrigatorio", obrigatorios.Contains(p.Name) },
                            { "descricao", Convert.ToString(definicao["description"]) }
                        };

                        if (definicao["enum"] != null) parametro["enum"] = definicao["enum"].DeepClone();
                        if (definicao["minimum"] != null) parametro["minimo"] = definicao["minimum"].DeepClone();
                        if (definicao["maximum"] != null) parametro["maximo"] = definicao["maximum"].DeepClone();
                        if (definicao["minLength"] != null) parametro["minLength"] = definicao["minLength"].DeepClone();
                        if (definicao["maxLength"] != null) parametro["maxLength"] = definicao["maxLength"].DeepClone();

                        parametros.Add(parametro);
                    }
                }

                arr.Add(new JObject
                {
                    { "nome", f.Nome },
                    { "escopo", string.IsNullOrWhiteSpace(f.Escopo) ? "READ" : f.Escopo },
                    { "descricao", f.Descricao },
                    { "params", parametros }
                });
            }
            return arr.ToString(Formatting.None);
        }

        private static void Selecionar(DropDownList ddl, string valor)
        {
            ListItem item = ddl.Items.FindByValue(valor ?? string.Empty);
            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
        }

        private static string NormalizarEscopoWorkflow(string escopo)
        {
            escopo = (escopo ?? string.Empty).Trim().ToUpperInvariant();
            return escopo == "WRITE" ? "WRITE" : "READ";
        }

        private static int ParaInt(string valor)
        {
            int r;
            return int.TryParse((valor ?? string.Empty).Trim(), out r) ? r : 0;
        }
    }
}
