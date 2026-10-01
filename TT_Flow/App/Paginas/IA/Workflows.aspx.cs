using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using TT_Flow.FrameWork.IA;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.IA
{
    public partial class Workflows : System.Web.UI.Page
    {
        private readonly cls_IA_Repositorio _repositorio = new cls_IA_Repositorio();

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.IA.AdministrarWorkflows, true);

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = "Workflows IA";
                Carregar();
            }
        }

        private void Carregar()
        {
            dtgvWorkflows.DataSource = _repositorio.ListarWorkflows(true);
            dtgvWorkflows.DataBind();
        }

        protected void dtgvWorkflows_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Ponto de extensão (cores/estado), mantido simples por ora.
        }

        protected static string PermissaoWorkflowHtml(object idRecurso, object descricao)
        {
            string id = Convert.ToString(idRecurso);
            string texto = Convert.ToString(descricao);

            if (string.IsNullOrWhiteSpace(texto))
            {
                texto = "Descrição da permissão não informada";
            }

            return "<span class=\"wf-permissao-id\">" + System.Web.HttpUtility.HtmlEncode(id) + "</span>" +
                   "<span class=\"wf-permissao-desc\">" + System.Web.HttpUtility.HtmlEncode(texto) + "</span>";
        }

        protected void chkAtivo_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            GridViewRow linha = (GridViewRow)chk.NamingContainer;
            int id = Convert.ToInt32(dtgvWorkflows.DataKeys[linha.RowIndex].Value);

            if (_repositorio.SetWorkflowAtivo(id, chk.Checked))
            {
                MensagemPagina.MostraMensagem_Sucesso(chk.Checked ? "Workflow ativado." : "Workflow desativado.");
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Não foi possível alterar o status do workflow.");
            }

            Carregar();
        }

        protected static string EntradasWorkflowAttr(object grafoJson)
        {
            try
            {
                JObject grafo = JObject.Parse(Convert.ToString(grafoJson) ?? "{}");
                JArray entradas = grafo["entradas"] as JArray ?? new JArray();
                return System.Web.HttpUtility.HtmlAttributeEncode(entradas.ToString(Newtonsoft.Json.Formatting.None));
            }
            catch
            {
                return "[]";
            }
        }

        protected static bool WorkflowAtivo(object ativo)
        {
            string valor = Convert.ToString(ativo);
            return valor.Equals("True", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("S", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("1", StringComparison.OrdinalIgnoreCase);
        }

        protected static bool PodeExecutarWorkflow(object ativo, object grafoJson, object escopo)
        {
            if (!WorkflowAtivo(ativo))
            {
                return false;
            }

            return !cls_IA_WorkflowExecucaoService.RequerPermissaoExecutarAcoes(Convert.ToString(grafoJson), Convert.ToString(escopo))
                || cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.ExecutarAcoes);
        }

        // Arquivar exige estar inativo primeiro (regra do banco, revalidada em ArquivarWorkflow); o botão só
        // aparece nesse caso pra não gerar clique que sempre falharia.
        protected static bool PodeArquivarWorkflow(object ativo)
        {
            return !WorkflowAtivo(ativo);
        }

        public class WorkflowImportResponse
        {
            public bool Sucesso { get; set; }
            public string Mensagem { get; set; }
            public int IdWorkflowIA { get; set; }
            public string SNomeInterno { get; set; }
        }

        // Aplica um pacote exportado por Workflow_Detalhe.aspx/ExportarWorkflow (ou por
        // EXEC dbo.sp_IA_Workflow_Exportar direto no banco): atualiza pelo sNomeInterno se o workflow ja
        // existe (preserva idWorkflowIA e as execucoes ligadas a ele), ou cria um novo. Mesma validacao de
        // sempre (permissao/escopo/JSON) roda de novo quando o workflow for executado.
        [WebMethod(EnableSession = true)]
        public static WorkflowImportResponse ImportarWorkflow(string sWorkflowJson)
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return new WorkflowImportResponse { Sucesso = false, Mensagem = "Sem permissão para importar workflows." };
            }

            if (string.IsNullOrWhiteSpace(sWorkflowJson))
            {
                return new WorkflowImportResponse { Sucesso = false, Mensagem = "Cole ou escolha o arquivo JSON exportado do workflow." };
            }

            try
            {
                int idWorkflowIA;
                string sNomeInterno;
                bool ok = new cls_IA_Repositorio().ImportarWorkflow(sWorkflowJson, out idWorkflowIA, out sNomeInterno);
                return ok
                    ? new WorkflowImportResponse { Sucesso = true, IdWorkflowIA = idWorkflowIA, SNomeInterno = sNomeInterno }
                    : new WorkflowImportResponse { Sucesso = false, Mensagem = "Não foi possível aplicar o workflow." };
            }
            catch (Exception ex)
            {
                return new WorkflowImportResponse { Sucesso = false, Mensagem = ex.Message };
            }
        }

        public class WorkflowArquivarResponse
        {
            public bool Sucesso { get; set; }
            public string Mensagem { get; set; }
        }

        // Tira o workflow da lista principal sem apagar nada (reversível por DesarquivarWorkflow). O banco
        // recusa (com mensagem) se o workflow ainda estiver ativo - desative pelo checkbox Ativo antes.
        [WebMethod(EnableSession = true)]
        public static WorkflowArquivarResponse ArquivarWorkflow(int idWorkflowIA)
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return new WorkflowArquivarResponse { Sucesso = false, Mensagem = "Sem permissão para arquivar workflows." };
            }

            if (idWorkflowIA <= 0)
            {
                return new WorkflowArquivarResponse { Sucesso = false, Mensagem = "Workflow inválido." };
            }

            string mensagem;
            bool ok = new cls_IA_Repositorio().SetWorkflowArquivado(idWorkflowIA, true, out mensagem);
            return new WorkflowArquivarResponse
            {
                Sucesso = ok,
                Mensagem = ok ? string.Empty : (string.IsNullOrWhiteSpace(mensagem) ? "Não foi possível arquivar o workflow." : mensagem)
            };
        }

        // Sem pré-condição: volta a aparecer na lista principal, mas continua inativo (reativar é um passo à
        // parte, pelo checkbox Ativo - evita religar sozinho algo que tinha sido desativado de propósito).
        [WebMethod(EnableSession = true)]
        public static WorkflowArquivarResponse DesarquivarWorkflow(int idWorkflowIA)
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return new WorkflowArquivarResponse { Sucesso = false, Mensagem = "Sem permissão para desarquivar workflows." };
            }

            if (idWorkflowIA <= 0)
            {
                return new WorkflowArquivarResponse { Sucesso = false, Mensagem = "Workflow inválido." };
            }

            string mensagem;
            bool ok = new cls_IA_Repositorio().SetWorkflowArquivado(idWorkflowIA, false, out mensagem);
            return new WorkflowArquivarResponse
            {
                Sucesso = ok,
                Mensagem = ok ? string.Empty : (string.IsNullOrWhiteSpace(mensagem) ? "Não foi possível desarquivar o workflow." : mensagem)
            };
        }

        public class WorkflowArquivadoItem
        {
            public int IdWorkflowIA { get; set; }
            public string Nome { get; set; }
            public string Descricao { get; set; }
        }

        public class WorkflowsArquivadosResponse
        {
            public bool Sucesso { get; set; }
            public string Mensagem { get; set; }
            public List<WorkflowArquivadoItem> Workflows { get; set; }
        }

        // Alimenta o modal "Arquivados" de Workflows.aspx.
        [WebMethod(EnableSession = true)]
        public static WorkflowsArquivadosResponse ListarWorkflowsArquivados()
        {
            if (!cls_IA_Permissoes.ValidaPermissaoEstrita(Permissao.IA.AdministrarWorkflows))
            {
                return new WorkflowsArquivadosResponse { Sucesso = false, Mensagem = "Sem permissão para administrar workflows." };
            }

            List<IAWorkflow> lista = new cls_IA_Repositorio().ListarWorkflowsArquivados();
            return new WorkflowsArquivadosResponse
            {
                Sucesso = true,
                Workflows = lista.ConvertAll(w => new WorkflowArquivadoItem { IdWorkflowIA = w.IdWorkflowIA, Nome = w.Nome, Descricao = w.Descricao })
            };
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
                return ErroExecucao("Workflow inválido.");
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
    }
}
