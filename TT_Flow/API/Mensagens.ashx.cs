using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

using BD = TT.FrameWork.BD;

namespace Api
{
    public class Mensagens : IHttpHandler
    {
        private readonly string sProcedure = "sp_Manipula_tbl_Flow_Mensagens";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            DataSet dsSessao = Autenticacao.ValidarSessao(context);

            if (dsSessao == null)
            {
                context.Response.StatusCode = 401;

                RetornarJson(
                    context,
                    new
                    {
                        sucesso = false,
                        mensagem = "Sessão inválida ou não informada."
                    }
                );

                return;
            }

            string idUsuario = Autenticacao.ObterIdUsuario(dsSessao);

            try
            {
                string contentType = context.Request.ContentType ?? "";
                string idMensagem = context.Request["idMensagem"] ?? "0";

                if (context.Request.HttpMethod == "GET" &&
                    idMensagem != "0")
                {
                    ConsultarDetalheMensagem(
                        context,
                        idMensagem,
                        idUsuario
                    );

                    return;
                }

                if (context.Request.HttpMethod == "POST" &&
                    contentType.ToLower().Contains("application/json"))
                {
                    SalvarMensagem(
                        context,
                        idUsuario
                    );

                    return;
                }

                ConsultarMensagens(
                    context,
                    idUsuario
                );
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;

                RetornarJson(
                    context,
                    new
                    {
                        sucesso = false,
                        erro = ex.Message
                    }
                );
            }
        }

        private void ConsultarMensagens(
            HttpContext context,
            string idUsuario)
        {
            string departamento =
                context.Request["idDepartamento"] ?? "0";

            string assunto =
                context.Request["sAssunto"] ?? "";

            string aviso =
                context.Request["sAviso"] ?? "T";

            string direcao =
                context.Request["sDirecao"] ?? "";

            string filtro =
                context.Request["sFiltro"] ?? "";

            string tipo =
                context.Request["sTipo"] ?? "-99";

            List<Mensagem> mensagens =
                new List<Mensagem>();

            Dictionary<string, string> parametros =
                new Dictionary<string, string>
                {
                    {"@sFuncao", "CONSULTAR_MENSAGENS__AREA_COLABORADOR"},
                    {"@idUsuarioPesquisa", idUsuario},
                    {"@sDirecaoMensagem", direcao},
                    {"@sIsAviso", aviso},
                    {"@idDepartamentoDestino", departamento},
                    {"@sAssunto", assunto},
                    {"@idFiltroMensagem", filtro},
                    {"@idTipoObjeto", tipo}
                };

            DataSet ds =
                BD.ExecutarDataSet(
                    sProcedure,
                    parametros,
                    false
                );

            if (ds == null || ds.Tables.Count < 2)
            {
                RetornarJson(
                    context,
                    new
                    {
                        sucesso = true,
                        mensagens,
                        nLidas = 0
                    }
                );

                return;
            }

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                mensagens.Add(
                    new Mensagem
                    {
                        id = Convert.ToInt32(row[0]),
                        assunto = row[1].ToString(),
                        sCorpo = row[2].ToString(),
                        dtEnvio = row[3].ToString(),
                        sDscUsuarioRemetente = row[5].ToString(),
                        dtLeitura = row[6].ToString(),
                        sDscDepartamento = row[9].ToString(),
                        sDscUsuarioDestino = row[16].ToString()
                    }
                );
            }

            string nLidas = "0";

            if (ds.Tables[0].Rows.Count > 0 &&
                ds.Tables[0].Columns.Contains("nLidas"))
            {
                nLidas =
                    ds.Tables[0].Rows[0]["nLidas"].ToString();
            }

            RetornarJson(
                context,
                new
                {
                    sucesso = true,
                    mensagens,
                    nLidas
                }
            );
        }

        private void ConsultarDetalheMensagem(
            HttpContext context,
            string idMensagem,
            string idUsuarioPesquisa)
        {
            Dictionary<string, string> parametros =
                new Dictionary<string, string>
                {
                    {"@sFuncao", "CONSULTAR_DETALHE"},
                    {"@idMensagem", idMensagem},
                    {"@idUsuarioPesquisa", idUsuarioPesquisa}
                };

            DataSet dsPesquisa =
                BD.ExecutarDataSet(
                    sProcedure,
                    parametros
                );

            string sErro = "";

            if (!BD.ValidarDataSet(
                dsPesquisa,
                out sErro))
            {
                context.Response.StatusCode = 400;

                RetornarJson(
                    context,
                    new
                    {
                        sucesso = false,
                        erro = sErro
                    }
                );

                return;
            }

            if (dsPesquisa == null ||
                dsPesquisa.Tables.Count == 0 ||
                dsPesquisa.Tables[0].Rows.Count == 0)
            {
                context.Response.StatusCode = 404;

                RetornarJson(
                    context,
                    new
                    {
                        sucesso = false,
                        erro = "Mensagem não encontrada."
                    }
                );

                return;
            }

            DataRow row =
                dsPesquisa.Tables[0].Rows[0];

            string idRemetente =
                row["idUsuarioRemetente"].ToString();

            bool podeEditar =
                idRemetente == idUsuarioPesquisa;

            List<Dictionary<string, object>> usuarios =
                new List<Dictionary<string, object>>();

            List<Dictionary<string, object>> departamentos =
                new List<Dictionary<string, object>>();

            if (dsPesquisa.Tables.Count > 1)
            {
                usuarios =
                    ConverterTabela(
                        dsPesquisa.Tables[1]
                    );
            }

            if (dsPesquisa.Tables.Count > 2)
            {
                departamentos =
                    ConverterTabela(
                        dsPesquisa.Tables[2]
                    );
            }

            RetornarJson(
                context,
                new
                {
                    sucesso = true,

                    mensagem = new
                    {
                        idMensagem =
                            row["idMensagem"].ToString(),

                        idUsuarioRemetente =
                            idRemetente,

                        sDscUsuarioRemetente =
                            row["sDscUsuarioRemetente"].ToString(),

                        sAssunto =
                            row["sAssunto"].ToString(),

                        sIsAviso =
                            row["sIsAviso"].ToString(),

                        sPermiteComentario =
                            row["sPermiteComentario"].ToString(),

                        sTags =
                            row["sTags"]
                                .ToString()
                                .Replace("|", " ")
                                .Trim(),

                        sCorpo =
                            HttpUtility.HtmlDecode(
                                row["sCorpo"].ToString()
                            ),

                        dtInclusao =
                            row["dtInclusao"].ToString(),

                        idDepartamentoDestino =
                            row["idDepartamentoDestino"].ToString(),

                        podeEditar
                    },

                    departamentos,
                    usuarios
                }
            );
        }

        private void SalvarMensagem(
            HttpContext context,
            string idUsuarioAtual)
        {
            JavaScriptSerializer serializer =
                new JavaScriptSerializer();

            string json;

            using (StreamReader reader =
                new StreamReader(
                    context.Request.InputStream))
            {
                json = reader.ReadToEnd();
            }

            MensagemRequest request =
                serializer.Deserialize<MensagemRequest>(
                    json
                );

            if (request == null)
            {
                throw new Exception(
                    "Dados da mensagem inválidos."
                );
            }

            if (string.IsNullOrWhiteSpace(
                request.sAssunto))
            {
                throw new Exception(
                    "O assunto é obrigatório."
                );
            }

            if (string.IsNullOrWhiteSpace(
                request.sCorpo))
            {
                throw new Exception(
                    "O corpo da mensagem é obrigatório."
                );
            }

            if (string.IsNullOrWhiteSpace(
                    request.sDepartamentosDestino) &&
                string.IsNullOrWhiteSpace(
                    request.sUsuariosDestino))
            {
                throw new Exception(
                    "Informe pelo menos um destinatário."
                );
            }

            Dictionary<string, string> parametros =
                new Dictionary<string, string>
                {
                    {"@sFuncao", "SALVAR"},
                    {"@idMensagem", request.idMensagem ?? "0"},

                    {"@idUsuarioRemetente", idUsuarioAtual},

                    {"@idDepartamentoDestino", "0"},

                    {"@sDepartamentosDestino",
                        request.sDepartamentosDestino ?? ""},

                    {"@sUsuariosDestino",
                        request.sUsuariosDestino ?? ""},

                    {"@idTipoObjeto", "0"},

                    {"@sCorpo",
                        request.sCorpo ?? ""},

                    {"@sAssunto",
                        request.sAssunto ?? ""},

                    {"@sIsAviso",
                        request.sIsAviso ?? "N"},

                    {"@sPermiteComentario",
                        request.sPermiteComentario ?? "N"},

                    {"@sTags",
                        request.sTags ?? ""},

                    {"@idUsuarioAtualizacao",
                        idUsuarioAtual},

                    {"@idUsuarioDestino",
                        request.idUsuarioDestino ?? "0"}
                };

            DataSet dsSalvar =
                BD.ExecutarDataSet(
                    sProcedure,
                    parametros
                );

            string sErro = "";

            if (!BD.ValidarDataSet(
                dsSalvar,
                out sErro))
            {
                context.Response.StatusCode = 400;

                RetornarJson(
                    context,
                    new
                    {
                        sucesso = false,
                        erro = sErro
                    }
                );

                return;
            }

            string idMensagemSalva = "0";

            if (dsSalvar.Tables.Count > 0 &&
                dsSalvar.Tables[0].Rows.Count > 0 &&
                dsSalvar.Tables[0].Columns.Contains(
                    "idMensagem"))
            {
                idMensagemSalva =
                    dsSalvar.Tables[0]
                        .Rows[0]["idMensagem"]
                        .ToString();
            }

            RetornarJson(
                context,
                new
                {
                    sucesso = true,
                    mensagem =
                        "Mensagem gravada com sucesso.",
                    idMensagem = idMensagemSalva
                }
            );
        }

        private List<Dictionary<string, object>> ConverterTabela(
            DataTable tabela)
        {
            List<Dictionary<string, object>> lista =
                new List<Dictionary<string, object>>();

            foreach (DataRow row in tabela.Rows)
            {
                Dictionary<string, object> item =
                    new Dictionary<string, object>();

                foreach (DataColumn coluna in tabela.Columns)
                {
                    item[coluna.ColumnName] =
                        row[coluna] == DBNull.Value
                            ? null
                            : row[coluna];
                }

                lista.Add(item);
            }

            return lista;
        }

        private void RetornarJson(
            HttpContext context,
            object objeto)
        {
            JavaScriptSerializer serializer =
                new JavaScriptSerializer
                {
                    MaxJsonLength = int.MaxValue
                };

            context.Response.Write(
                serializer.Serialize(objeto)
            );
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public class Mensagem
        {
            public int id { get; set; }
            public string assunto { get; set; }
            public string sCorpo { get; set; }
            public string dtEnvio { get; set; }
            public string sDscUsuarioRemetente { get; set; }
            public string dtLeitura { get; set; }
            public string sDscDepartamento { get; set; }
            public string sDscUsuarioDestino { get; set; }
        }

        public class MensagemRequest
        {
            public string idMensagem { get; set; }
            public string sDepartamentosDestino { get; set; }
            public string sUsuariosDestino { get; set; }
            public string sCorpo { get; set; }
            public string sAssunto { get; set; }
            public string sIsAviso { get; set; }
            public string sPermiteComentario { get; set; }
            public string sTags { get; set; }
            public string idUsuarioDestino { get; set; }
        }
    }
}
