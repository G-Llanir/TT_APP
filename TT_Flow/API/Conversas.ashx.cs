using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using TT.FrameWork;
using TT_Flow.FrameWork;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace Api
{
    public class ConversasHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var serializer = new JavaScriptSerializer();

            DataSet dsSessao = Autenticacao.ValidarSessao(context);

            if (dsSessao == null)
            {
                context.Response.StatusCode = 401;
                context.Response.Write(
                    serializer.Serialize(new
                    {
                        sucesso = false,
                        mensagem = "Sessão inválida ou não informada."
                    })
                );
                return;
            }

            string idUsuario = Autenticacao.ObterIdUsuario(dsSessao);
            string sFunction = context.Request["sFunction"];
            string idRegistroConvesa = context.Request["idConversa"];

            List<Conversa> conversas = new List<Conversa>();
            List<DirectMessage> mensagens = new List<DirectMessage>();

            try
            {
                Dictionary<string, string> parametrosUsuario =
                    new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", idUsuario }
                    };

                DataSet dsUsuario = BD.ExecutarDataSet(
                    "sp_Manipula_tbl_Flow_Colaboradores_Area",
                    parametrosUsuario
                );

                if (!BD.ValidarDataSet(dsUsuario, out string erroUsuario))
                    throw new Exception(erroUsuario);

                string idColaborador =
                    RETORNO.DATASET(dsUsuario, 0, "idColaborador");

                if (string.IsNullOrEmpty(idColaborador) || idColaborador == "0")
                    throw new Exception(
                        "Usuário sem associação a um colaborador."
                    );

                if (sFunction == "list")
                {
                    Dictionary<string, string> vParametros =
                        new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTAR-CONVERSAS" },
                            { "@idColaborador", idColaborador },
                            { "@idUsuarioAtualizacao", idUsuario },
                            { "@sApenasRh", "N" }
                        };

                    DataSet ds = BD.ExecutarDataSet(
                        "sp_Manipula_tbl_Flow_Colaboradores_Area",
                        vParametros
                    );

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        conversas.Add(new Conversa
                        {
                            idRegistroConvesa = Convert.ToInt32(row[0]),
                            idConversa = Convert.ToInt32(row[1]),
                            idColaborador = Convert.ToInt32(row[2]),
                            sDscConversa = row[3].ToString(),
                            dtEventoConversa = row[4].ToString(),
                            dtResolucaoConversa = row[5].ToString(),
                            dtRepostaConversa = row[6].ToString(),
                            sObservacaoConversa = row[7].ToString(),
                            dtProximaConversa = row[8].ToString(),
                            sCor = row[9].ToString(),
                            idAcao = Convert.ToInt32(row[10]),
                            sDscAcao = row[11].ToString(),
                            idMeio = Convert.ToInt32(row[12]),
                            sDscMeio = row[13].ToString(),
                            idStatusConversa = Convert.ToInt32(row[14]),
                            sDscStatusConversa = row[15].ToString(),
                            idTipoEvento = Convert.ToInt32(row[16]),
                            sDscTipoEvento = row[17].ToString(),
                            sDscEventoConversa = row[18].ToString(),
                            nQtdConversasPendentes = Convert.ToInt32(row[19])
                        });
                    }

                    context.Response.Write(
                        serializer.Serialize(new
                        {
                            sucesso = true,
                            conversas
                        })
                    );
                }
                else
                {
                    Dictionary<string, string> vParametros =
                        new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONVERSA-DETALHES" },
                            { "@idRegistroConvesa", idRegistroConvesa },
                            { "@idUsuarioAtualizacao", idUsuario }
                        };

                    DataSet dsEditar = BD.ExecutarDataSet(
                        "sp_Manipula_tbl_Flow_Colaboradores_Area",
                        vParametros
                    );

                    vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONVERSA-HISTORICO" },
                        { "@idRegistroConvesa", idRegistroConvesa }
                    };

                    DataSet dsHistorico = BD.ExecutarDataSet(
                        "sp_Manipula_tbl_Flow_Colaboradores_Area",
                        vParametros
                    );

                    string idArquivo =
                        RETORNO.DATASET(dsEditar, 0, "idArquivo");

                    foreach (DataRow row in dsHistorico.Tables[0].Rows)
                    {
                        mensagens.Add(new DirectMessage
                        {
                            sObservacaoConversa = row[0].ToString(),
                            idUsuarioAtualizacao = Convert.ToInt32(row[1]),
                            sDscConversa = row[2].ToString(),
                            sDscUsuario = row[3].ToString(),
                            dtAtualizacao = row[4].ToString(),
                            imgColaborador = row[5].ToString()
                        });
                    }

                    context.Response.Write(
                        serializer.Serialize(new
                        {
                            sucesso = true,
                            idArquivo,
                            mensagens
                        })
                    );
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 400;

                context.Response.Write(
                    serializer.Serialize(new
                    {
                        sucesso = false,
                        mensagem = ex.Message
                    })
                );
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public class Conversa
        {
            public int idRegistroConvesa { get; set; }
            public int idConversa { get; set; }
            public int idColaborador { get; set; }
            public string sDscConversa { get; set; }
            public string dtEventoConversa { get; set; }
            public string dtResolucaoConversa { get; set; }
            public string dtRepostaConversa { get; set; }
            public string sObservacaoConversa { get; set; }
            public string dtProximaConversa { get; set; }
            public string sCor { get; set; }
            public int idAcao { get; set; }
            public string sDscAcao { get; set; }
            public int idMeio { get; set; }
            public string sDscMeio { get; set; }
            public int idStatusConversa { get; set; }
            public string sDscStatusConversa { get; set; }
            public int idTipoEvento { get; set; }
            public string sDscTipoEvento { get; set; }
            public string sDscEventoConversa { get; set; }
            public int nQtdConversasPendentes { get; set; }
            public int idArquivo { get; set; }
        }

        public class DirectMessage
        {
            public string sObservacaoConversa { get; set; }
            public int idUsuarioAtualizacao { get; set; }
            public string sDscConversa { get; set; }
            public string sDscUsuario { get; set; }
            public string dtAtualizacao { get; set; }
            public string imgColaborador { get; set; }
        }
    }
}
