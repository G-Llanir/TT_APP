using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using TT.FrameWork;
using static TT.FrameWork.Identity;
using BD = TT.FrameWork.BD;

namespace Api
{
    public class LoginHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var serializer = new JavaScriptSerializer();

            try
            {
                var sessao = "";
                var sLogin = "";
                var sDscUsuario = "";
                var idCliente = "";
                var idUsuario = "";
                var idColaborador = "";
                var sDsCliente = "";
                var idParceiro = "";
                var sDscParceiro = "";
                var sTipo = "";
                var sPermissao = "";
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    string json = reader.ReadToEnd();

                    LoginRequest request =
                        serializer.Deserialize<LoginRequest>(json);

                    int dtresultado = 1;

                    DataSet dsLogin = BD.ExecutarDataSet("sp_Login_ValidaUsuario", new Dictionary<string, string> { { "@sLogin", request.Usuario } });
                    string sdtValidade = BD.Retorno.DATASET(dsLogin, "dtValidade");

                    if (sdtValidade != "")
                    {
                        DateTime dtValidade = Convert.ToDateTime(sdtValidade);
                        DateTime dtHoje = DateTime.Today;
                        dtresultado = DateTime.Compare(dtValidade, dtHoje);
                    }

                    if (dtresultado > 0)
                    {
                        if (BD.ValidarDataSet(dsLogin))
                        {
                            if (request.Senha.Trim().Equals(BD.Retorno.DATASET(dsLogin, "sSenha")))
                            {

                                Dictionary<string, string> vParametrosSessao = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "GERAR SESSAO" },
                                    { "@idUsuario", BD.Retorno.DATASET(dsLogin, "idUsuario")}
                                };
                                DataSet dsSessao = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios_Sessao", vParametrosSessao);
                                sessao = BD.Retorno.DATASET(dsSessao, "sChavesessao");
                                sLogin = BD.Retorno.DATASET(dsLogin, "sLogin");
                                sDscUsuario = BD.Retorno.DATASET(dsLogin, "sDscUsuario");
                                idCliente = BD.Retorno.DATASET(dsLogin, "idCliente");
                                idUsuario = BD.Retorno.DATASET(dsLogin, "idUsuario");
                                idColaborador = BD.Retorno.DATASET(dsLogin, "idColaborador");
                                sDsCliente = BD.Retorno.DATASET(dsLogin, "sDsCliente");
                                idParceiro = BD.Retorno.DATASET(dsLogin, 0, "idParceiro");
                                sDscParceiro = BD.Retorno.DATASET(dsLogin, 0, "sDscParceiro");
                                sTipo = BD.Retorno.DATASET(dsLogin, "sTipo");
                                sPermissao = BD.Retorno.DATASET(dsLogin, "sPermissao");
                            }
                            else
                                throw new Exception("Usuário e senha inválidos!");
                        }
                    }
                    else
                        throw new Exception("Usuário expirado!");

                    context.Response.Write(
                        serializer.Serialize(new
                        {
                            sucesso = true,
                            session = sessao,
                            login = sLogin,
                            nome = sDscUsuario,
                            cliente = sDsCliente,
                            idUsuario = idUsuario,
                            idColaborador = idColaborador,
                            permissao = sPermissao,
                            tipo = sTipo,
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
    }

    public class LoginRequest
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }
}
