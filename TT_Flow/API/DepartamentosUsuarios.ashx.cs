using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

using BD = TT.FrameWork.BD;

namespace Api
{
    public class DepartamentosUsuarios : IHttpHandler
    {
        private readonly string sProcedure =
            "sp_Manipula_tbl_Flow_Mensagens";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType =
                "application/json";

            context.Response.ContentEncoding =
                Encoding.UTF8;

            context.Response.Charset =
                "utf-8";

            JavaScriptSerializer serializer =
                new JavaScriptSerializer();

            DataSet dsSessao =
                Autenticacao.ValidarSessao(context);

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

            string idUsuario =
                Autenticacao.ObterIdUsuario(dsSessao);

            try
            {
                string acao =
                    context.Request.QueryString["acao"];

                if (acao == "departamentos")
                {
                    RetornarDepartamentos(
                        context,
                        idUsuario
                    );
                }
                else if (acao == "usuarios")
                {
                    RetornarUsuarios(context);
                }
                else
                {
                    context.Response.StatusCode = 400;

                    RetornarJson(
                        context,
                        new
                        {
                            erro = "Ação inválida."
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;

                RetornarJson(
                    context,
                    new
                    {
                        erro = ex.Message
                    }
                );
            }
        }

        private void RetornarDepartamentos(
            HttpContext context,
            string idUsuario)
        {
            DataTable tabela = null;

            try
            {
                Dictionary<string, string> parametros =
                    new Dictionary<string, string>();

                parametros.Add(
                    "@sFuncao",
                    "CONSULTAR_DEPARTAMENTOS_MENSAGENS"
                );

                parametros.Add(
                    "@idUsuarioPesquisa",
                    idUsuario
                );

                DataSet ds =
                    BD.ExecutarDataSet(
                        sProcedure,
                        parametros
                    );

                if (BD.ValidarDataSet(ds))
                {
                    tabela = ds.Tables[0];
                }
            }
            catch
            {
                tabela = null;
            }

            if (tabela == null ||
                tabela.Rows.Count == 0)
            {
                tabela =
                    BD.ExecutarDataTable(
                        "sp_Select 'Flow_Departamentos'"
                    );
            }

            List<object> departamentos =
                new List<object>();

            if (tabela != null)
            {
                foreach (DataRow row in tabela.Rows)
                {
                    departamentos.Add(
                        new
                        {
                            idDepartamento =
                                ObterValor(
                                    row,
                                    "idDepartamento"
                                ),

                            sDscDepartamento =
                                ObterValor(
                                    row,
                                    "sDscDepartamento"
                                )
                        }
                    );
                }
            }

            RetornarJson(
                context,
                departamentos
            );
        }

        private void RetornarUsuarios(
            HttpContext context)
        {
            string departamentosParam =
                context.Request.QueryString[
                    "departamentos"
                ];

            if (string.IsNullOrWhiteSpace(
                departamentosParam))
            {
                RetornarJson(
                    context,
                    new List<object>()
                );

                return;
            }

            string[] departamentos =
                departamentosParam.Split(',');

            DataTable tbUsuarios =
                CriarTabelaUsuarios();

            foreach (
                string idDepartamentoTexto
                in departamentos)
            {
                int idDepartamento;

                if (!int.TryParse(
                    idDepartamentoTexto.Trim(),
                    out idDepartamento))
                {
                    continue;
                }

                DataTable tbDepartamentoUsuarios =
                    BD.ExecutarDataTable(
                        "sp_Select 'Usuarios_x_Departamentos'," +
                        idDepartamento
                    );

                AdicionarUsuarios(
                    tbUsuarios,
                    tbDepartamentoUsuarios,
                    idDepartamento
                );
            }

            List<object> usuarios =
                new List<object>();

            foreach (DataRow row in tbUsuarios.Rows)
            {
                usuarios.Add(
                    new
                    {
                        idUsuario =
                            row["idUsuario"],

                        sDscUsuario =
                            row["sDscUsuario"],

                        sEmail =
                            row["sEmail"],

                        sDscUsuarioEmail =
                            row["sDscUsuarioEmail"],

                        idDepartamento =
                            row["idDepartamento"]
                    }
                );
            }

            RetornarJson(
                context,
                usuarios
            );
        }

        private DataTable CriarTabelaUsuarios()
        {
            DataTable tabela =
                new DataTable();

            tabela.Columns.Add(
                "idUsuario",
                typeof(string)
            );

            tabela.Columns.Add(
                "sDscUsuario",
                typeof(string)
            );

            tabela.Columns.Add(
                "sEmail",
                typeof(string)
            );

            tabela.Columns.Add(
                "sDscUsuarioEmail",
                typeof(string)
            );

            tabela.Columns.Add(
                "idDepartamento",
                typeof(int)
            );

            return tabela;
        }

        private void AdicionarUsuarios(
            DataTable tbDestino,
            DataTable tbOrigem,
            int idDepartamento)
        {
            if (tbOrigem == null)
            {
                return;
            }

            if (!tbOrigem.Columns.Contains(
                "idUsuario"))
            {
                return;
            }

            foreach (DataRow row in tbOrigem.Rows)
            {
                string idUsuario =
                    row["idUsuario"].ToString();

                if (string.IsNullOrWhiteSpace(
                    idUsuario))
                {
                    continue;
                }

                DataRow[] usuariosExistentes =
                    tbDestino.Select(
                        "idUsuario = '" +
                        idUsuario.Replace(
                            "'",
                            "''"
                        ) +
                        "'"
                    );

                if (usuariosExistentes.Length > 0)
                {
                    continue;
                }

                string nome =
                    ObterValor(
                        row,
                        "sDscUsuario"
                    );

                string email =
                    ObterValor(
                        row,
                        "sEmail"
                    );

                if (string.IsNullOrWhiteSpace(
                    nome))
                {
                    nome = idUsuario;
                }

                string nomeEmail;

                if (string.IsNullOrWhiteSpace(
                    email))
                {
                    nomeEmail = nome;
                }
                else
                {
                    nomeEmail =
                        nome +
                        " - " +
                        email;
                }

                DataRow novo =
                    tbDestino.NewRow();

                novo["idUsuario"] =
                    idUsuario;

                novo["sDscUsuario"] =
                    nome;

                novo["sEmail"] =
                    email;

                novo["sDscUsuarioEmail"] =
                    nomeEmail;

                novo["idDepartamento"] =
                    idDepartamento;

                tbDestino.Rows.Add(
                    novo
                );
            }
        }

        private string ObterValor(
            DataRow row,
            string nomeColuna)
        {
            if (row == null ||
                row.Table == null ||
                !row.Table.Columns.Contains(
                    nomeColuna))
            {
                return "";
            }

            if (row[nomeColuna] == null ||
                row[nomeColuna] == DBNull.Value)
            {
                return "";
            }

            return row[nomeColuna].ToString();
        }

        private void RetornarJson(
            HttpContext context,
            object objeto)
        {
            JavaScriptSerializer serializer =
                new JavaScriptSerializer();

            context.Response.Write(
                serializer.Serialize(objeto)
            );
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
