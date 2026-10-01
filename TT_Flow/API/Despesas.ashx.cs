using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using TT.FrameWork;
using TT_Flow.FrameWork;

namespace Api
{
    public class Despesas : IHttpHandler
    {
        private const string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Relatorio_Despesas";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            try
            {
                DataSet dsSessao = Autenticacao.ValidarSessao(context);

                if (dsSessao == null)
                {
                    Responder(context, new { sucesso = false, mensagem = "Sessão inválida ou expirada." });
                    return;
                }

                string idUsuario = Autenticacao.ObterIdUsuario(dsSessao);
                string permissao = Autenticacao.obterPermissaoUsuario(dsSessao);

                if (!(permissao.IndexOf("|" + Permissao.RelatorioDespesas.Consultar.ToString() + "|") > -1))
                {
                    context.Response.StatusCode = 403;

                    Responder(context, new
                    {
                        sucesso = false,
                        mensagem = "Não tem permissão para visualizar a página."
                    });
                    return;
                }




                string sFuncao = context.Request["sFuncao"];

                if (string.IsNullOrEmpty(sFuncao))
                    sFuncao = "CONSULTAR-DESPESAS-COLABORADOR";

                switch (sFuncao.ToUpper())
                {
                    case "FLOW-TIPOS":
                        ConsultarTipos(context);
                        break;

                    case "INSERIR_DESPESAS_AREACOLABORADOR":
                        InserirDespesa(context, serializer, idUsuario);
                        break;

                    case "CONSULTAR-DESPESAS-COLABORADOR":
                        ConsultarDespesas(context, serializer, idUsuario);
                        break;

                    default:
                        Responder(context, new { sucesso = false, mensagem = "Função não encontrada." });
                        break;
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                Responder(context, new { sucesso = false, mensagem = ex.Message });
            }
        }

        // ============================================================
        // TIPOS
        // ============================================================

        private void ConsultarTipos(HttpContext context)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "FLOW-TIPOS" }
            };

            DataSet dsTipos = BD.ExecutarDataSet(sProcedure, parametros);
            List<TipoDespesa> tipos = new List<TipoDespesa>();

            if (BD.ValidarDataSet(dsTipos))
            {
                foreach (DataRow row in dsTipos.Tables[0].Rows)
                {
                    tipos.Add(new TipoDespesa
                    {
                        idTipoGastos = Convert.ToInt32(row["idTipoGastos"]),
                        sDscGasto = row["sDscGasto"].ToString()
                    });
                }
            }

            Responder(context, new { sucesso = true, tipos = tipos });
        }

        // ============================================================
        // INSERIR DESPESA
        // ============================================================

        private void InserirDespesa(HttpContext context, JavaScriptSerializer serializer, string idUsuario)
        {
            string json;

            using (StreamReader reader = new StreamReader(context.Request.InputStream))
                json = reader.ReadToEnd();

            CadastroRequest request = serializer.Deserialize<CadastroRequest>(json);

            if (request == null)
            {
                Responder(context, new { sucesso = false, mensagem = "Dados da despesa não informados." });
                return;
            }

            if (request.idDespesas <= 0)
            {
                Responder(context, new { sucesso = false, mensagem = "Informe o id da despesa." });
                return;
            }

            if (request.nValor <= 0)
            {
                Responder(context, new { sucesso = false, mensagem = "Informe um valor válido." });
                return;
            }

            if (request.idTipoGastos <= 0)
            {
                Responder(context, new { sucesso = false, mensagem = "Selecione o tipo da despesa." });
                return;
            }

            if (string.IsNullOrWhiteSpace(request.dtDespesa))
            {
                Responder(context, new { sucesso = false, mensagem = "Informe a data da despesa." });
                return;
            }

            DateTime dataDespesa;

            if (!DateTime.TryParse(request.dtDespesa, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dataDespesa))
            {
                Responder(context, new { sucesso = false, mensagem = "A data da despesa é inválida." });
                return;
            }

            string dataDespesaFormatada = dataDespesa.ToString("yyyyMMdd"); ;

            if (string.IsNullOrWhiteSpace(request.sNomeArquivo) ||
                string.IsNullOrWhiteSpace(request.vbArquivo))
            {
                Responder(context, new
                {
                    sucesso = false,
                    mensagem = "É obrigatório anexar o comprovante da despesa."
                });

                return;
            }

            // ========================================================
            // INSERIR_DESPESAS_AREACOLABORADOR
            // ========================================================

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "INSERIR_DESPESAS_AREACOLABORADOR" },
                { "@idDespesas", request.idDespesas.ToString() },
                { "@dtDespesa", dataDespesaFormatada },
                { "@nValor", request.nValor.ToString(CultureInfo.InvariantCulture) },
                { "@sLocal", request.sLocal },
                { "@idTipoGastos", request.idTipoGastos.ToString() },
                { "@sFrequencia", "" },
                { "@nQtdDias", "0" },
                { "@idFormaPagamento", "0" },
                { "@idCategoriaPagar", "0" },
                { "@sTipoItem", "L" },
                { "@sidParticipantes", "" },
                { "@idUsuario", idUsuario }
            };

            DataSet ds = BD.ExecutarDataSet(sProcedure, parametros);

            if (!BD.ValidarDataSet(ds))
            {
                Responder(context, new
                {
                    sucesso = false,
                    mensagem = "Não foi possível inserir a despesa."
                });

                return;
            }

            int nRet = 0;

            try
            {
                nRet = Convert.ToInt32(BD.Retorno.DATASET(ds, "nRet"));
            }
            catch
            {
                nRet = 0;
            }

            if (nRet != 0)
            {
                Responder(context, new
                {
                    sucesso = false,
                    mensagem = BD.Retorno.DATASET(ds, "msg")
                });

                return;
            }

            int idItens = 0;

            try
            {
                idItens = Convert.ToInt32(BD.Retorno.DATASET(ds, "idItens"));
            }
            catch
            {
                idItens = 0;
            }

            // ========================================================
            // COMPROVANTE
            // ========================================================

            byte[] arquivoBytes;

            try
            {
                arquivoBytes = Convert.FromBase64String(request.vbArquivo);
            }
            catch
            {
                Responder(context, new
                {
                    sucesso = false,
                    mensagem = "O arquivo enviado não possui um formato válido."
                });

                return;
            }

            cls_Arquivos arquivo = new cls_Arquivos();

            arquivo.idTipoArquivo = 8888;
            arquivo.idObjeto = idItens;
            arquivo.sNomeArquivo = request.sNomeArquivo;
            arquivo.sDscArquivo = string.IsNullOrWhiteSpace(request.sDscArquivo)
                ? request.sNomeArquivo
                : request.sDscArquivo;
            arquivo.sObservacao = "";
            arquivo.vbArquivo = arquivoBytes;
            arquivo.idUsuario = Convert.ToInt32(idUsuario);
            arquivo.dtExpiracaoDoc = DateTime.Now.AddYears(1).ToString("yyyyMMdd");
            arquivo.dtRegistroDoc = DateTime.Now.ToString("yyyyMMdd");

            DataSet dsArquivo = arquivo.EnviarArquivo(arquivo);

            if (!BD.ValidarDataSet(dsArquivo))
            {
                Responder(context, new
                {
                    sucesso = false,
                    mensagem = "A despesa foi inserida, mas não foi possível salvar o comprovante."
                });

                return;
            }

            int idArquivo = 0;

            try
            {
                idArquivo = Convert.ToInt32(BD.Retorno.DATASET(dsArquivo, "idArquivo"));
            }
            catch
            {
                idArquivo = 0;
            }

            Responder(context, new
            {
                sucesso = true,
                mensagem = "Despesa inserida com sucesso.",
                idDespesas = request.idDespesas,
                idItens = idItens,
                idArquivo = idArquivo
            });
        }

        // ============================================================
        // CONSULTA
        // ============================================================

        private void ConsultarDespesas(HttpContext context, JavaScriptSerializer serializer, string idUsuario)
        {
            List<Despesa> despesas = new List<Despesa>();

            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR-DESPESAS-COLABORADOR" },
                { "@idUsuario", idUsuario },
                { "@sPesquisa", "" },
                { "@idStatus", "0" }
            };

            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, parametros);

            if (BD.ValidarDataSet(dsPesquisa))
            {
                foreach (DataRow row in dsPesquisa.Tables[0].Rows)
                {
                    List<Registro> historico = new List<Registro>();

                    Dictionary<string, string> parametrosHistorico = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-HISTORICO" },
                        { "@idDespesas", Convert.ToInt32(row[0]).ToString() }
                    };

                    DataSet dsHistorico = BD.ExecutarDataSet(sProcedure, parametrosHistorico);

                    if (BD.ValidarDataSet(dsHistorico) && dsHistorico.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rowR in dsHistorico.Tables[0].Rows)
                        {
                            historico.Add(new Registro
                            {
                                idHistorico = Convert.ToInt32(rowR[0]),
                                sDscMotivo = rowR[1].ToString(),
                                idStatus = Convert.ToInt32(rowR[2]),
                                dtStatus = rowR[3].ToString(),
                                dtStatusFormatada = rowR[4].ToString(),
                                idDespesa = Convert.ToInt32(rowR[5]),
                                idUsuario = Convert.ToInt32(rowR[6]),
                                sDscUsuario = rowR[7].ToString(),
                                sStatus = rowR[8].ToString(),
                                sCor = rowR[9].ToString()
                            });
                        }
                    }

                    despesas.Add(new Despesa
                    {
                        idDespesas = Convert.ToInt32(row[0]),
                        sDscMotivo = row[1].ToString(),
                        idPedido = Convert.ToInt32(row[2]),
                        sDscPedido = row[3].ToString(),
                        idTipo = Convert.ToInt32(row[4]),
                        sDscObservacao = row[5].ToString(),
                        idUsuario = Convert.ToInt32(row[6]),
                        sStatus = row[7].ToString(),
                        sCor = row[8].ToString(),
                        nValor = Convert.ToDecimal(row[9]),
                        nValorLimite = Convert.ToDecimal(row[10]),
                        idStatus = Convert.ToInt32(row[11]),
                        sDscUsuario = row[12].ToString(),
                        dtInclusao = row[13].ToString(),
                        sTipo = row[14].ToString(),
                        historico = historico
                    });
                }
            }

            Responder(context, new { sucesso = true, despesas = despesas });
        }

        // ============================================================
        // RESPONSE
        // ============================================================

        private void Responder(HttpContext context, object objeto)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.Write(serializer.Serialize(objeto));
        }

        public bool IsReusable
        {
            get { return false; }
        }

        // ============================================================
        // MODELS
        // ============================================================

        public class CadastroRequest
        {
            public int idDespesas { get; set; }
            public decimal nValor { get; set; }
            public int idTipoGastos { get; set; }
            public string dtDespesa { get; set; }
            public string sNomeArquivo { get; set; }
            public string sDscArquivo { get; set; }
            public string vbArquivo { get; set; }
            public string sLocal { get; set; }
        }

        public class TipoDespesa
        {
            public int idTipoGastos { get; set; }
            public string sDscGasto { get; set; }
        }

        public class Despesa
        {
            public int idDespesas { get; set; }
            public string sDscMotivo { get; set; }
            public int idPedido { get; set; }
            public string sDscPedido { get; set; }
            public int idTipo { get; set; }
            public string sDscObservacao { get; set; }
            public int idUsuario { get; set; }
            public string sStatus { get; set; }
            public string sCor { get; set; }
            public decimal nValor { get; set; }
            public decimal nValorLimite { get; set; }
            public int idStatus { get; set; }
            public string sDscUsuario { get; set; }
            public string dtInclusao { get; set; }
            public string sTipo { get; set; }
            public List<Registro> historico { get; set; }
        }

        public class Registro
        {
            public int idHistorico { get; set; }
            public string sDscMotivo { get; set; }
            public int idStatus { get; set; }
            public string dtStatus { get; set; }
            public string dtStatusFormatada { get; set; }
            public int idDespesa { get; set; }
            public int idUsuario { get; set; }
            public string sDscUsuario { get; set; }
            public string sStatus { get; set; }
            public string sCor { get; set; }
        }
    }
}
