using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using TT.FrameWork;

namespace Api
{
    public class Solicitacoes : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var serializer = new JavaScriptSerializer();

            context.Response.ContentType = "application/json";

            DataSet dsSessao = Autenticacao.ValidarSessao(context);

            if (dsSessao == null)
            {
                context.Response.StatusCode = 401;

                context.Response.Write(serializer.Serialize(new
                {
                    sucesso = false,
                    mensagem = "Sessão inválida ou não informada."
                }));

                return;
            }

            string idUsuario =
                Autenticacao.ObterIdUsuario(dsSessao);

            string sFuncao =
                context.Request.QueryString["sFuncao"];

            try
            {
                switch (sFuncao)
                {
                    case "LISTA":
                        Lista(context, serializer, idUsuario);
                        break;

                    case "TIPOS":
                        Tipos(context, serializer);
                        break;

                    case "CADASTRO":
                        Cadastro(context, serializer, idUsuario);
                        break;

                    default:
                        context.Response.Write(serializer.Serialize(new
                        {
                            sucesso = false,
                            mensagem = "Função não informada ou inválida."
                        }));
                        break;
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;

                context.Response.Write(serializer.Serialize(new
                {
                    sucesso = false,
                    mensagem = ex.Message
                }));
            }
        }

        private void Lista(
            HttpContext context,
            JavaScriptSerializer serializer,
            string idUsuario)
        {
            List<Solicitacao> solicitacoes =
                new List<Solicitacao>();

            Dictionary<string, string> vParametros =
                new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR" },
                    { "@sSistema", "TCOLABORADOR" },
                    { "@sDscSolicitacao", "" },
                    { "@idTipoSolicitacao", "0" },
                    { "@dtInicio", "" },
                    { "@dtFinal", "" },
                    { "@idUsuarioLogado", idUsuario }
                };

            DataSet dsPesquisa =
                BD.ExecutarDataSet(
                    "sp_Manipula_tbl_Flow_Solicitacoes",
                    vParametros
                );

            foreach (DataRow row in dsPesquisa.Tables[0].Rows)
            {
                solicitacoes.Add(new Solicitacao
                {
                    idSolicitacao = Convert.ToInt32(row[0]),
                    sDscSolicitacao = row[1].ToString(),
                    idUsuarioSolicitacao = Convert.ToInt32(row[2]),
                    sSolicitante = row[3].ToString(),
                    sEmailSolicitante = row[4].ToString(),
                    idDepartamento = Convert.ToInt32(row[5]),
                    sDepartamento = row[6].ToString(),
                    sDscUsuarioAtualizacao = row[7].ToString(),
                    dtAtualizacao = row[8].ToString(),
                    dtInicio = row[9].ToString(),
                    dtFinal = row[10].ToString(),
                    idTipo = Convert.ToInt32(row[11]),
                    sTipo = row[12].ToString(),
                    sFluxoTerceiro = row[13].ToString(),
                    idStatus = Convert.ToInt32(row[14]),
                    sStatus = row[15].ToString(),
                    sLiberaRH = row[16].ToString(),
                    sLiberaSupervisor = row[17].ToString(),
                    sLiberaColaborador = row[18].ToString(),
                    sLiberaTerceiros = row[19].ToString(),
                    sExibeStatus = row[20].ToString(),
                    sCor = row[21].ToString(),
                    dtSolicitacao = row[22].ToString(),
                    sObservacao = row[23].ToString()
                });
            }

            context.Response.Write(serializer.Serialize(new
            {
                sucesso = true,
                dados = solicitacoes
            }));
        }

        private void Tipos(
            HttpContext context,
            JavaScriptSerializer serializer)
        {
            Dictionary<string, string> vParametros =
                new Dictionary<string, string>
                {
                    { "@sTabela", "Flow_Solicitacao_Tipo" }
                };

            DataSet dsTipos =
                BD.ExecutarDataSet(
                    "sp_Select",
                    vParametros
                );

            List<object> tipos =
                new List<object>();

            foreach (DataRow row in dsTipos.Tables[0].Rows)
            {
                tipos.Add(new
                {
                    idTipo = row[0],
                    sTipo = row[1]
                });
            }

            context.Response.Write(serializer.Serialize(new
            {
                sucesso = true,
                dados = tipos
            }));
        }

        private void Cadastro(
            HttpContext context,
            JavaScriptSerializer serializer,
            string idUsuario)
        {
            try
            {
                CadastroRequest request;

                using (var reader =
                    new StreamReader(context.Request.InputStream))
                {
                    string json = reader.ReadToEnd();

                    request =
                        serializer.Deserialize<CadastroRequest>(json);
                }

                if (request == null)
                {
                    throw new Exception(
                        "Dados da solicitação não informados."
                    );
                }

                if (string.IsNullOrWhiteSpace(
                    request.sDscSolicitacao))
                {
                    throw new Exception(
                        "Título da solicitação não informado."
                    );
                }

                if (request.idTipo <= 0)
                {
                    throw new Exception(
                        "Tipo de solicitação não informado."
                    );
                }

                Dictionary<string, string> vParametros =
                    new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idSolicitacao", "0" },
                        { "@sDscSolicitacao",
                            request.sDscSolicitacao },
                        { "@idTipo",
                            request.idTipo.ToString() },
                        { "@idStatus", "1" },
                        { "@idUsuarioSolicitacao",
                            idUsuario },
                        { "@idUsuarioAtualizacao",
                            idUsuario },
                        { "@sObservacaoSolicitacao",
                            request.sObservacaoSolicitacao ?? "" }
                    };

                if (!string.IsNullOrWhiteSpace(
                    request.dtInicio))
                {
                    DateTime dataInicio =
                        DateTime.Parse(request.dtInicio);

                    vParametros.Add(
                        "@dtInicio",
                        dataInicio.ToString()
                    );
                }

                if (request.idTipo == 1 &&
                    !string.IsNullOrWhiteSpace(
                        request.dtFinal))
                {
                    DateTime dataFinal =
                        DateTime.Parse(request.dtFinal);

                    vParametros.Add(
                        "@dtFinal",
                        dataFinal.ToString()
                    );
                }

                DataSet dsSalvar =
                    BD.ExecutarDataSet(
                        "sp_Manipula_tbl_Flow_Solicitacoes",
                        vParametros
                    );

                if (!BD.ValidarDataSet(
                    dsSalvar,
                    out string sErro))
                {
                    context.Response.StatusCode = 400;

                    context.Response.Write(
                        serializer.Serialize(new
                        {
                            sucesso = false,
                            mensagem = "BD: " + sErro
                        })
                    );

                    return;
                }

                context.Response.Write(
                    serializer.Serialize(new
                    {
                        sucesso = true,
                        mensagem =
                            "Solicitação efetuada com sucesso!"
                    })
                );
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;

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

        public class CadastroRequest
        {
            public int idTipo { get; set; }
            public string sDscSolicitacao { get; set; }
            public string dtInicio { get; set; }
            public string dtFinal { get; set; }
            public string sObservacaoSolicitacao { get; set; }
        }

        public class Solicitacao
        {
            public int idSolicitacao { get; set; }
            public string sDscSolicitacao { get; set; }
            public int idUsuarioSolicitacao { get; set; }
            public string sSolicitante { get; set; }
            public string sEmailSolicitante { get; set; }
            public int idDepartamento { get; set; }
            public string sDepartamento { get; set; }
            public string sDscUsuarioAtualizacao { get; set; }
            public string dtAtualizacao { get; set; }
            public string dtInicio { get; set; }
            public string dtFinal { get; set; }
            public int idTipo { get; set; }
            public string sTipo { get; set; }
            public string sFluxoTerceiro { get; set; }
            public int idStatus { get; set; }
            public string sStatus { get; set; }
            public string sLiberaRH { get; set; }
            public string sLiberaSupervisor { get; set; }
            public string sLiberaColaborador { get; set; }
            public string sLiberaTerceiros { get; set; }
            public string sExibeStatus { get; set; }
            public string sCor { get; set; }
            public string dtSolicitacao { get; set; }
            public string sObservacao { get; set; }
        }
    }
}
