using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using TT.FrameWork;
using static TT.FrameWork.Identity;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace Api
{
    public class Colaborador : IHttpHandler
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
            string funcao = context.Request.QueryString["sFuncao"];

            if (funcao == "cID")
            {
                try
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", idUsuario }
                    };

                    DataSet dsPesquisa = BD.ExecutarDataSet(
                        "sp_Manipula_tbl_Flow_Colaboradores_Area",
                        vParametros
                    );

                    BD.ValidarDataSet(dsPesquisa, out string sErro);

                    string idColaborador =
                        RETORNO.DATASET(dsPesquisa, 0, "idColaborador");

                    context.Response.Write(
                        serializer.Serialize(new
                        {
                            sucesso = true,
                            idColaborador
                        })
                    );
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
            else
            {
                string carregaimgColaborador(string idObjeto)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_IMAGEM" },
                        { "@idTipoArquivo", "40" },
                        { "@idObjeto", idObjeto }
                    };

                    DataTable dsPesquisa = BD.ExecutarDataTable(
                        "sp_Manipula_tbl_Flow_Arquivos",
                        vParametros
                    );

                    if (dsPesquisa.Rows.Count > 0)
                    {
                        string imgUrl =
                            "data:image/jpg;base64," +
                            Convert.ToBase64String(
                                (byte[])dsPesquisa.Rows[0]["vbArquivo"]
                            );

                        return imgUrl;
                    }

                    return "";
                }

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

                    if (idColaborador == "0" || string.IsNullOrEmpty(idColaborador))
                        throw new Exception(
                            "Usuário sem associação a um colaborador."
                        );

                    Dictionary<string, string> parametrosColaborador =
                        new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTAR-DADOS" },
                            { "@idColaborador", idColaborador }
                        };

                    DataSet dsColaborador = BD.ExecutarDataSet(
                        "sp_Manipula_tbl_Flow_Colaboradores_Area",
                        parametrosColaborador
                    );

                    if (!BD.ValidarDataSet(dsColaborador, out string erroColaborador))
                        throw new Exception(erroColaborador);

                    var perfil = new
                    {
                        nome = RETORNO.DATASET(
                            dsColaborador, 0, "sDscColaborador"),

                        email = RETORNO.DATASET(
                            dsColaborador, 0, "sEmail"),

                        telefoneCelular = RETORNO.DATASET(
                            dsColaborador, 0, "sTelCelular"),

                        ramal = RETORNO.DATASET(
                            dsColaborador, 0, "sRamal"),

                        dataNascimento = RETORNO.DATASET(
                            dsColaborador, 0, "dtNascimento"),

                        departamento = RETORNO.DATASET(
                            dsColaborador, 0, "sDscDepartamento")
                    };

                    string imgUrl = carregaimgColaborador(idColaborador);

                    context.Response.Write(
                        serializer.Serialize(new
                        {
                            sucesso = true,
                            perfil,
                            foto = imgUrl
                        })
                    );
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
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
