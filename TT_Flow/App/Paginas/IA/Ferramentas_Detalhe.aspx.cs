using System;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using TT_Flow.FrameWork.IA;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.IA
{
    public partial class Ferramentas_Detalhe : System.Web.UI.Page
    {
        private readonly cls_IA_Repositorio _repositorio = new cls_IA_Repositorio();

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.IA.AdministrarFerramentas, true);

            if (!IsPostBack)
            {
                // Recursos (permissoes) para o dropdown. Popula antes de Carregar/Preparar para o
                // Selecionar encontrar o item; itens ficam no ViewState e sobrevivem ao postback do salvar.
                FUNCOES.Popula_Combo(ddlIdRecurso, "sp_Select 'Recursos'", "idRecurso", "sDscRecurso", true, "Selecione a permissao", "0");

                int id = ParaInt(Request.QueryString["id"]);
                if (id > 0)
                {
                    CarregarFerramenta(id);
                    if (Request.QueryString["salvo"] == "1")
                    {
                        MensagemPagina.MostraMensagem_Sucesso("Ferramenta salva com sucesso.");
                    }
                }
                else
                {
                    PrepararNova();
                }
            }

            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page, "tooltip");
        }

        private void PrepararNova()
        {
            hddId.Value = "0";
            lblTituloPagina.Text = "Nova ferramenta da IA";
            BreadCrumb_Pagina.TitulodaPagina = "Nova ferramenta";
            pnAvisoNova.Visible = true;
            pnAvisoInterna.Visible = false;
            cmdExcluir.Visible = false;
            txtNome.ReadOnly = false;

            Selecionar(ddlEscopo, "READ");
            Selecionar(ddlRequerConfirmacao, "N");
            Selecionar(ddlAtivo, "S");
            Selecionar(ddlTipoExecucao, "G"); // ferramenta nova nasce generica (configurada pela tela)
            txtMaxRegistros.Text = "20";
            txtIndiceTabela.Text = "0";
        }

        private void CarregarFerramenta(int id)
        {
            IAFerramentaDefinicao ferramenta = _repositorio.ObterFerramenta(id);
            if (ferramenta == null)
            {
                MensagemPagina.MostraMensagem_Erro("Ferramenta nao localizada.");
                cmdSalvar.Visible = false;
                return;
            }

            bool interna = cls_IA_ToolRegistry.EhInterna(ferramenta.Nome);

            hddId.Value = ferramenta.IdFerramentaIA.ToString();
            lblTituloPagina.Text = "Ferramenta: " + ferramenta.Nome;
            BreadCrumb_Pagina.TitulodaPagina = ferramenta.Nome;

            txtNome.Text = ferramenta.Nome;
            txtNome.ReadOnly = true; // o nome interno e imutavel na edicao (chave de execucao)
            txtModulo.Text = ferramenta.Modulo;
            Selecionar(ddlIdRecurso, ferramenta.IdRecursoNecessario.ToString());
            txtDescricao.Text = ferramenta.Descricao;
            Selecionar(ddlEscopo, string.IsNullOrWhiteSpace(ferramenta.Escopo) ? "READ" : ferramenta.Escopo.ToUpperInvariant());
            Selecionar(ddlRequerConfirmacao, ferramenta.RequerConfirmacao ? "S" : "N");
            txtMaxRegistros.Text = ferramenta.MaxRegistros.ToString();
            Selecionar(ddlAtivo, ferramenta.Ativo ? "S" : "N");
            txtSchema.Text = ferramenta.SchemaParametros != null ? ferramenta.SchemaParametros.ToString(Newtonsoft.Json.Formatting.Indented) : string.Empty;

            // Tipo de execucao: generica se tiver procedure configurada; senao interna (codigo)
            bool generica = ferramenta.EhGenerica();
            Selecionar(ddlTipoExecucao, generica ? "G" : "I");
            txtProcedureGenerica.Text = ferramenta.ProcedureGenerica ?? string.Empty;
            txtIndiceTabela.Text = ferramenta.IndiceTabelaRetorno.ToString();
            hddArgumentos.Value = ArgumentosDoSchema(ferramenta.SchemaParametros);
            hddMapa.Value = ArrayOuVazio(ferramenta.MapaParametros);
            hddColunas.Value = ArrayOuVazio(ferramenta.ColunasRetorno);

            pnAvisoNova.Visible = false;
            pnAvisoInterna.Visible = interna;
            // Interna nao pode ser excluida (o seed a recriaria); personalizada sim
            cmdExcluir.Visible = !interna;
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            int id = ParaInt(hddId.Value);
            bool nova = id == 0;

            string nome = (txtNome.Text ?? string.Empty).Trim();
            if (nova && nome.Length < 2)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um nome interno com ao menos 2 caracteres.");
                return;
            }

            if (nova && !_rgxNomeFerramenta.IsMatch(nome))
            {
                MensagemPagina.MostraMensagem_Erro("Nome interno invalido. Use apenas letras, numeros, underline ou hifen (sem espaco e sem acento).");
                return;
            }

            int idRecurso = ParaInt(ddlIdRecurso.SelectedValue);
            if (idRecurso <= 0)
            {
                MensagemPagina.MostraMensagem_Erro("Selecione a permissao (recurso) necessaria para a ferramenta.");
                return;
            }

            int maxRegistros = ParaInt(txtMaxRegistros.Text);
            if (maxRegistros <= 0)
            {
                maxRegistros = 20;
            }

            bool generica = ddlTipoExecucao.SelectedValue == "G";
            string escopo = ddlEscopo.SelectedValue;

            IAFerramentaDefinicao ferramenta = new IAFerramentaDefinicao
            {
                IdFerramentaIA = id,
                Nome = nome,
                Descricao = (txtDescricao.Text ?? string.Empty).Trim(),
                Modulo = (txtModulo.Text ?? string.Empty).Trim(),
                IdRecursoNecessario = idRecurso,
                Escopo = escopo,
                RequerConfirmacao = ddlRequerConfirmacao.SelectedValue == "S",
                MaxRegistros = maxRegistros,
                Ativo = ddlAtivo.SelectedValue == "S"
            };

            string schemaJson;

            if (generica)
            {
                string procedure = (txtProcedureGenerica.Text ?? string.Empty).Trim();
                if (procedure.Length == 0)
                {
                    MensagemPagina.MostraMensagem_Erro("Informe a procedure que a ferramenta deve executar.");
                    return;
                }

                JArray argumentos = ParseArray(hddArgumentos.Value);
                JArray mapa = ParseArray(hddMapa.Value);
                JArray colunas = ParseArray(hddColunas.Value);

                // Chave invalida aqui derruba a chamada INTEIRA no provider (todas as ferramentas juntas), nao so esta.
                string argInvalido = PrimeiroArgumentoInvalido(argumentos);
                if (argInvalido != null)
                {
                    MensagemPagina.MostraMensagem_Erro("Nome de argumento invalido: \"" + argInvalido
                        + "\". Use apenas letras, numeros, ponto ou hifen - sem espaco, acento ou underline. "
                        + "Os providers recusam a requisicao inteira se qualquer chave fugir disso.");
                    return;
                }

                // Mapa apontando para argumento inexistente manda '' para a procedure, sem erro nenhum.
                string paramOrfao = PrimeiroMapeamentoOrfao(argumentos, mapa);
                if (paramOrfao != null)
                {
                    MensagemPagina.MostraMensagem_Erro("O parametro \"" + paramOrfao + "\" tem origem \"argumento da IA\", "
                        + "mas o Valor nao corresponde a nenhum argumento da secao 1. Se voce renomeou o argumento, "
                        + "atualize o Valor aqui tambem - senao a procedure recebe vazio e a ferramenta nao acha nada.");
                    return;
                }

                if (string.Equals(escopo, "READ", StringComparison.OrdinalIgnoreCase) && colunas.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Erro("Ferramenta de consulta precisa de ao menos uma coluna de retorno.");
                    return;
                }

                // O schema (contrato de argumentos que a IA envia) e GERADO pelo construtor
                schemaJson = GerarSchemaDeArgumentos(argumentos);

                ferramenta.ProcedureGenerica = procedure;
                ferramenta.MapaParametros = mapa.ToString(Newtonsoft.Json.Formatting.None);
                ferramenta.ColunasRetorno = colunas.ToString(Newtonsoft.Json.Formatting.None);
                ferramenta.IndiceTabelaRetorno = ParaInt(txtIndiceTabela.Text);
            }
            else
            {
                // Interna: schema vem do textarea (JSON), sem config generica
                schemaJson = (txtSchema.Text ?? string.Empty).Trim();
                if (schemaJson.Length > 0)
                {
                    try
                    {
                        JObject.Parse(schemaJson);
                    }
                    catch (Exception ex)
                    {
                        MensagemPagina.MostraMensagem_Erro("O schema JSON e invalido: " + ex.Message);
                        return;
                    }
                }

                ferramenta.ProcedureGenerica = null;
                ferramenta.MapaParametros = null;
                ferramenta.ColunasRetorno = null;
                ferramenta.IndiceTabelaRetorno = 0;
            }

            string mensagem;
            int idSalvo = _repositorio.SalvarFerramenta(ferramenta, schemaJson, out mensagem);
            if (idSalvo <= 0)
            {
                MensagemPagina.MostraMensagem_Erro(string.IsNullOrWhiteSpace(mensagem) ? "Nao foi possivel salvar a ferramenta." : mensagem);
                return;
            }

            cls_IA_ToolRegistry.InvalidarCache();
            cls_IA_Auditoria.Registrar(0, nova ? "FERRAMENTA_CRIADA" : "FERRAMENTA_ATUALIZADA", "ALERTA", new
            {
                idFerramentaIA = idSalvo,
                ferramenta = ferramenta.Nome,
                escopo = ferramenta.Escopo,
                tipo = generica ? "GENERICA" : "INTERNA",
                procedure = ferramenta.ProcedureGenerica ?? string.Empty,
                recurso = ferramenta.IdRecursoNecessario,
                ativa = ferramenta.Ativo
            });

            Response.Redirect("~/App/Paginas/IA/Ferramentas_Detalhe.aspx?id=" + idSalvo + "&salvo=1");
        }

        protected void cmdTestar_Click(object sender, EventArgs e)
        {
            string procedure = (txtProcedureGenerica.Text ?? string.Empty).Trim();
            if (procedure.Length == 0)
            {
                MostrarTeste("<div class=\"alert alert-warning\">Informe a procedure antes de testar.</div>");
                return;
            }

            IAFerramentaDefinicao ferramenta = new IAFerramentaDefinicao
            {
                Nome = (txtNome.Text ?? string.Empty).Trim(),
                Escopo = ddlEscopo.SelectedValue,
                MaxRegistros = ParaInt(txtMaxRegistros.Text),
                ProcedureGenerica = procedure,
                MapaParametros = ParseArray(hddMapa.Value).ToString(Newtonsoft.Json.Formatting.None),
                ColunasRetorno = ParseArray(hddColunas.Value).ToString(Newtonsoft.Json.Formatting.None),
                IndiceTabelaRetorno = ParaInt(txtIndiceTabela.Text)
            };

            JObject argumentos;
            try
            {
                argumentos = JObject.Parse(string.IsNullOrWhiteSpace(hddTesteArgs.Value) ? "{}" : hddTesteArgs.Value);
            }
            catch
            {
                argumentos = new JObject();
            }

            bool simular = string.Equals(ferramenta.Escopo, "WRITE", StringComparison.OrdinalIgnoreCase);
            IAResultadoTeste resultado = cls_IA_ToolExecutor.TestarGenerica(ferramenta, argumentos, simular);
            MostrarTeste(AvisoSincronia() + RenderResultadoTeste(resultado));
        }

        // O teste roda a configuracao da TELA, mas o chat roda a versao SALVA. Divergir entre as duas ja fez
        // um teste passar verde com o chat quebrado - entao dizer qual das duas esta sendo testada e obrigatorio.
        private string AvisoSincronia()
        {
            int id = ParaInt(hddId.Value);
            if (id == 0)
            {
                return "<div class=\"alert alert-warning\"><i class=\"fa fa-exclamation-triangle\"></i> "
                     + "Ferramenta <strong>ainda não salva</strong>. Este teste usa o que está na tela — a IA só enxerga a ferramenta depois de <strong>Salvar</strong>.</div>";
            }

            IAFerramentaDefinicao salva = _repositorio.ObterFerramenta(id);
            if (salva != null && ConfiguracaoDivergente(salva))
            {
                return "<div class=\"alert alert-warning\"><i class=\"fa fa-exclamation-triangle\"></i> "
                     + "Há <strong>alterações não salvas</strong>: este teste usa a configuração da tela, mas o chat continua usando a versão salva. "
                     + "Clique em <strong>Salvar</strong> para valer.</div>";
            }

            return "<div class=\"text-muted\" style=\"margin-bottom:6px;\"><i class=\"fa fa-check\"></i> "
                 + "Testando a configuração salva — a mesma que a IA usa.</div>";
        }

        private bool ConfiguracaoDivergente(IAFerramentaDefinicao salva)
        {
            if (!string.Equals((txtProcedureGenerica.Text ?? string.Empty).Trim(), (salva.ProcedureGenerica ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (ParaInt(txtIndiceTabela.Text) != salva.IndiceTabelaRetorno)
            {
                return true;
            }

            if (ParaInt(txtMaxRegistros.Text) != salva.MaxRegistros)
            {
                return true;
            }

            if (!string.Equals(ddlEscopo.SelectedValue, salva.Escopo ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (NormalizarArray(hddMapa.Value) != NormalizarArray(salva.MapaParametros))
            {
                return true;
            }

            if (NormalizarArray(hddColunas.Value) != NormalizarArray(salva.ColunasRetorno))
            {
                return true;
            }

            string schemaTela = GerarSchemaDeArgumentos(ParseArray(hddArgumentos.Value));
            string schemaSalvo = salva.SchemaParametros != null
                ? salva.SchemaParametros.ToString(Newtonsoft.Json.Formatting.None)
                : string.Empty;

            return schemaTela != schemaSalvo;
        }

        private static string NormalizarArray(string json)
        {
            return ParseArray(json).ToString(Newtonsoft.Json.Formatting.None);
        }

        private void MostrarTeste(string html)
        {
            litResultadoTeste.Text = html;
            pnResultadoTeste.Visible = true;
        }

        protected void cmdExcluir_Click(object sender, EventArgs e)
        {
            int id = ParaInt(hddId.Value);
            string nome = (txtNome.Text ?? string.Empty).Trim();

            if (id <= 0)
            {
                return;
            }

            if (cls_IA_ToolRegistry.EhInterna(nome))
            {
                MensagemPagina.MostraMensagem_Erro("Ferramenta interna nao pode ser excluida. Desative-a se nao quiser que seja usada.");
                return;
            }

            if (!_repositorio.ExcluirFerramenta(id))
            {
                MensagemPagina.MostraMensagem_Erro("Nao foi possivel excluir a ferramenta.");
                return;
            }

            cls_IA_ToolRegistry.InvalidarCache();
            cls_IA_Auditoria.Registrar(0, "FERRAMENTA_EXCLUIDA", "ALERTA", new { idFerramentaIA = id, ferramenta = nome });

            Response.Redirect("~/App/Paginas/IA/Configuracao.aspx");
        }

        // ---- Construtor: schema <-> argumentos ----

        // Chaves de propriedade do schema: os providers exigem ^[a-zA-Z0-9.-]{1,64}$ (sem espaco, acento ou underline).
        // O nome da ferramenta e mais permissivo (aceita underline), por isso sao dois padroes.
        private static readonly Regex _rgxChaveArgumento = new Regex(@"^[a-zA-Z0-9.-]{1,64}$", RegexOptions.Compiled);
        private static readonly Regex _rgxNomeFerramenta = new Regex(@"^[a-zA-Z0-9_-]{1,64}$", RegexOptions.Compiled);

        // Devolve o @param cuja origem e "argumento da IA" mas cujo Valor nao existe na secao 1.
        private static string PrimeiroMapeamentoOrfao(JArray argumentos, JArray mapa)
        {
            System.Collections.Generic.HashSet<string> nomes =
                new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (JToken item in argumentos)
            {
                JObject arg = item as JObject;
                if (arg == null)
                {
                    continue;
                }

                string n = ValorTexto(arg, "nome").Trim();
                if (n.Length > 0)
                {
                    nomes.Add(n);
                }
            }

            foreach (JToken item in mapa)
            {
                JObject entrada = item as JObject;
                if (entrada == null)
                {
                    continue;
                }

                // Espelha o motor: so "fixo" e "token" nao referenciam argumento; o resto e "arg".
                string origem = ValorTexto(entrada, "origem").Trim().ToLowerInvariant();
                if (origem == "fixo" || origem == "token")
                {
                    continue;
                }

                string valor = ValorTexto(entrada, "valor").Trim();
                if (valor.Length == 0 || !nomes.Contains(valor))
                {
                    return ValorTexto(entrada, "param").Trim();
                }
            }

            return null;
        }

        private static string PrimeiroArgumentoInvalido(JArray argumentos)
        {
            foreach (JToken item in argumentos)
            {
                JObject arg = item as JObject;
                if (arg == null)
                {
                    continue;
                }

                string nome = ValorTexto(arg, "nome");
                if (nome.Length > 0 && !_rgxChaveArgumento.IsMatch(nome))
                {
                    return nome;
                }
            }

            return null;
        }

        // Gera o JSON Schema (contrato que a IA usa) a partir das linhas do construtor
        private static string GerarSchemaDeArgumentos(JArray argumentos)
        {
            JObject props = new JObject();
            JArray obrigatorios = new JArray();

            foreach (JToken item in argumentos)
            {
                JObject arg = item as JObject;
                if (arg == null)
                {
                    continue;
                }

                string nome = ValorTexto(arg, "nome");
                if (nome.Length == 0)
                {
                    continue;
                }

                string tipo = ValorTexto(arg, "tipo");
                string jsonType = string.Equals(tipo, "numero", StringComparison.OrdinalIgnoreCase) ? "integer" : "string";

                bool obrig;
                bool.TryParse(ValorTexto(arg, "obrigatorio"), out obrig);

                // strict:true (OpenAI) exige TODAS as chaves em required; "opcional" e expresso pelo tipo aceitar null
                JToken tipoToken = obrig ? (JToken)jsonType : new JArray(jsonType, "null");
                props[nome] = new JObject
                {
                    { "type", tipoToken },
                    { "description", ValorTexto(arg, "descricao") }
                };
                obrigatorios.Add(nome);
            }

            JObject schema = new JObject
            {
                { "type", "object" },
                { "properties", props },
                { "required", obrigatorios },
                { "additionalProperties", false }
            };
            return schema.ToString(Newtonsoft.Json.Formatting.None);
        }

        // Reconstroi as linhas de argumentos a partir do schema salvo (para reabrir a tela)
        private static string ArgumentosDoSchema(JObject schema)
        {
            JArray saida = new JArray();
            if (schema == null)
            {
                return "[]";
            }

            JObject props = schema["properties"] as JObject;
            if (props == null)
            {
                return "[]";
            }

            foreach (System.Collections.Generic.KeyValuePair<string, JToken> par in props)
            {
                JObject def = par.Value as JObject;
                JToken tipoToken = def != null ? def["type"] : null;

                // tipo pode ser "string" ou ["string","null"] (opcional)
                bool nullable = false;
                string baseType = "string";
                if (tipoToken is JArray)
                {
                    foreach (JToken t in (JArray)tipoToken)
                    {
                        string s = (string)t;
                        if (string.Equals(s, "null", StringComparison.OrdinalIgnoreCase))
                        {
                            nullable = true;
                        }
                        else
                        {
                            baseType = s;
                        }
                    }
                }
                else if (tipoToken != null)
                {
                    baseType = (string)tipoToken;
                }

                bool numero = string.Equals(baseType, "integer", StringComparison.OrdinalIgnoreCase) || string.Equals(baseType, "number", StringComparison.OrdinalIgnoreCase);

                saida.Add(new JObject
                {
                    { "nome", par.Key },
                    { "tipo", numero ? "numero" : "texto" },
                    { "descricao", def != null && def["description"] != null ? (string)def["description"] : string.Empty },
                    { "obrigatorio", !nullable }
                });
            }

            return saida.ToString(Newtonsoft.Json.Formatting.None);
        }

        private string RenderResultadoTeste(IAResultadoTeste r)
        {
            StringBuilder sb = new StringBuilder();
            string cor = r.Sucesso ? "alert-success" : "alert-danger";
            sb.Append("<div class=\"alert ").Append(cor).Append("\">").Append(Server.HtmlEncode(r.Mensagem ?? string.Empty)).Append("</div>");

            if (!string.IsNullOrEmpty(r.ParametrosResolvidos))
            {
                sb.Append("<div><strong>Parâmetros resolvidos:</strong> <span class=\"ff-mono\">")
                  .Append(Server.HtmlEncode(r.ParametrosResolvidos)).Append("</span></div>");
            }

            if (r.Simulacao)
            {
                sb.Append("<div class=\"text-muted\" style=\"margin-top:6px;\">Simulação: nada foi gravado. Ative a ferramenta e use o chat para executar de verdade (com confirmação).</div>");
                return sb.ToString();
            }

            if (r.Colunas != null && r.Colunas.Count > 0 && r.Linhas != null && r.Linhas.Count > 0)
            {
                sb.Append("<div class=\"table-responsive\" style=\"margin-top:8px;\"><table class=\"table table-bordered table-condensed\"><thead><tr>");
                foreach (string c in r.Colunas)
                {
                    sb.Append("<th>").Append(Server.HtmlEncode(c)).Append("</th>");
                }
                sb.Append("</tr></thead><tbody>");
                foreach (System.Collections.Generic.Dictionary<string, string> linha in r.Linhas)
                {
                    sb.Append("<tr>");
                    foreach (string c in r.Colunas)
                    {
                        string val;
                        linha.TryGetValue(c, out val);
                        sb.Append("<td>").Append(Server.HtmlEncode(val ?? string.Empty)).Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table></div>");
            }

            return sb.ToString();
        }

        private static JArray ParseArray(string json)
        {
            try
            {
                return string.IsNullOrWhiteSpace(json) ? new JArray() : JArray.Parse(json);
            }
            catch
            {
                return new JArray();
            }
        }

        private static string ArrayOuVazio(string json)
        {
            return string.IsNullOrWhiteSpace(json) ? "[]" : json;
        }

        private static string ValorTexto(JObject obj, string chave)
        {
            JToken token = obj[chave];
            return token != null && token.Type != JTokenType.Null ? token.ToString() : string.Empty;
        }

        private void Selecionar(System.Web.UI.WebControls.DropDownList ddl, string valor)
        {
            System.Web.UI.WebControls.ListItem item = ddl.Items.FindByValue(valor ?? string.Empty);
            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
        }

        private static int ParaInt(string valor)
        {
            int retorno;
            return int.TryParse((valor ?? string.Empty).Trim(), out retorno) ? retorno : 0;
        }
    }
}
