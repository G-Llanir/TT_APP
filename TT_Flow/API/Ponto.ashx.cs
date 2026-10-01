using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using TT.FrameWork;

namespace Api
{
    public class Ponto : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var serializer = new JavaScriptSerializer();

            DataSet dsSessao = Autenticacao.ValidarSessao(context);

            if (dsSessao == null)
            {
                context.Response.StatusCode = 401;

                RetornarJson(context, new
                {
                    sucesso = false,
                    mensagem = "Sessão inválida ou não informada."
                });

                return;
            }

            string idUsuario =
                Autenticacao.ObterIdUsuario(dsSessao);

            try
            {
                string funcao =
                    context.Request.QueryString["sFuncao"];

                if (funcao == "SALVAR")
                    Salvar(context, idUsuario);
                else if (funcao == "CONSULTAR")
                    Consultar(context, idUsuario);
                else
                    RetornarJson(context, new
                    {
                        sucesso = false,
                        mensagem = "Função inválida."
                    });
            }
            catch (Exception ex)
            {
                RetornarJson(context, new
                {
                    sucesso = false,
                    mensagem = ex.Message
                });
            }
        }

        private void Salvar(
            HttpContext context,
            string idUsuario)
        {
            string json;

            using (var reader =
                new StreamReader(context.Request.InputStream))
            {
                json = reader.ReadToEnd();
            }

            var serializer = new JavaScriptSerializer();

            var dados =
                serializer.Deserialize<Dictionary<string, object>>(json);

            int idTipoPonto =
                Convert.ToInt32(dados["idTipoPonto"]);

            // FOTO
            if (!dados.ContainsKey("foto") ||
                dados["foto"] == null)
            {
                RetornarJson(context, new
                {
                    sucesso = false,
                    mensagem = "Foto não informada."
                });

                return;
            }

            string base64 =
                dados["foto"].ToString();

            if (base64.Contains(","))
                base64 =
                    base64.Substring(
                        base64.IndexOf(",") + 1
                    );

            byte[] foto =
                Convert.FromBase64String(base64);

            string fotoHex =
                BitConverter
                    .ToString(foto)
                    .Replace("-", "");

            // LOCALIZAÇÃO
            decimal? latitude =
                dados.ContainsKey("latitude") &&
                dados["latitude"] != null
                    ? Convert.ToDecimal(
                        dados["latitude"],
                        CultureInfo.InvariantCulture)
                    : (decimal?)null;

            decimal? longitude =
                dados.ContainsKey("longitude") &&
                dados["longitude"] != null
                    ? Convert.ToDecimal(
                        dados["longitude"],
                        CultureInfo.InvariantCulture)
                    : (decimal?)null;

            // OBSERVAÇÃO
            string observacao =
                dados.ContainsKey("observacao") &&
                dados["observacao"] != null
                    ? dados["observacao"].ToString()
                    : null;

            var parametros =
                new Dictionary<string, string>
                {
                    { "@sFuncao", "SALVAR" },
                    { "@idUsuario", idUsuario },
                    { "@idTipoPonto", idTipoPonto.ToString() },
                    { "@vbFoto", fotoHex },
                    {
                        "@nLatitude",
                        latitude.HasValue
                            ? latitude.Value.ToString(
                                CultureInfo.InvariantCulture)
                            : null
                    },
                    {
                        "@nLongitude",
                        longitude.HasValue
                            ? longitude.Value.ToString(
                                CultureInfo.InvariantCulture)
                            : null
                    },
                    { "@sObservacao", observacao }
                };

            DataSet ds =
                BD.ExecutarDataSet(
                    "sp_Manipula_tbl_Flow_Ponto",
                    parametros
                );

            RetornarJson(context, new
            {
                sucesso = true,
                idPonto =
                    ds.Tables[0].Rows[0]["idPonto"]
            });
        }

        private void Consultar(
            HttpContext context,
            string idUsuario)
        {
            var parametros =
                new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR" },
                    { "@idUsuario", idUsuario }
                };

            DataSet ds =
                BD.ExecutarDataSet(
                    "sp_Manipula_tbl_Flow_Ponto",
                    parametros
                );

            RetornarJson(
                context,
                ConverterTabela(ds.Tables[0])
            );
        }

        private List<object> ConverterTabela(
            DataTable tabela)
        {
            var lista =
                new List<object>();

            foreach (DataRow row in tabela.Rows)
            {
                var item =
                    new Dictionary<string, object>();

                foreach (DataColumn column in tabela.Columns)
                {
                    item[column.ColumnName] =
                        row[column] == DBNull.Value
                            ? null
                            : row[column];
                }

                lista.Add(item);
            }

            return lista;
        }

        private void RetornarJson(
            HttpContext context,
            object objeto)
        {
            var serializer =
                new JavaScriptSerializer();

            context.Response.Write(
                serializer.Serialize(objeto)
            );
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
