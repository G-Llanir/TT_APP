using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using TT.FrameWork;

namespace Api
{
    public class Agenda : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var serializer = new JavaScriptSerializer();

            DataSet dsSessao = Autenticacao.ValidarSessao(context);

            if (dsSessao == null)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
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

            List<Atividade> atividades = new List<Atividade>();
            DateTime dataSelecionada;

            using (var reader = new StreamReader(context.Request.InputStream))
            {
                string json = reader.ReadToEnd();

                UsuarioRequest request =
                    serializer.Deserialize<UsuarioRequest>(json);

                dataSelecionada = DateTime.Parse(request.dtConsulta);
            }

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idUsuario", idUsuario }
            };

            DataSet dsPesquisa = BD.ExecutarDataSet(
                "sp_Manipula_tbl_Flow_Calendario_Eventos_x_Usuario",
                vParametros
            );

            foreach (DataRow row in dsPesquisa.Tables[0].Rows)
            {
                DateTime? dtInicial = row.IsNull(10)
                    ? (DateTime?)null
                    : Convert.ToDateTime(row[10]);

                DateTime? dtFinal = row.IsNull(11)
                    ? (DateTime?)null
                    : Convert.ToDateTime(row[11]);

                bool dentroDoPeriodo =
                    (!dtInicial.HasValue || dataSelecionada >= dtInicial.Value.Date) &&
                    (!dtFinal.HasValue || dataSelecionada <= dtFinal.Value.Date);

                if (dentroDoPeriodo)
                {
                    atividades.Add(new Atividade
                    {
                        id = Convert.ToInt32(row[0]),
                        idEvento = Convert.ToInt32(row[1]),
                        idUsuario = Convert.ToInt32(row[2]),
                        sDscTitulo = row[3].ToString(),
                        sDscEvento = row[4].ToString(),
                        sDiaInteiro = row[5].ToString(),
                        sDisplay = row[6].ToString(),
                        sUrl = row[7].ToString(),
                        sCor = row[8].ToString(),
                        sCorTexto = row[9].ToString(),
                        dtInicial = row[10].ToString(),
                        dtFinal = row[11].ToString(),
                        idUsuarioAtualizacao = Convert.ToInt32(row[12]),
                        dtAtualizacao = row[13].ToString(),
                        sAtivo = row[14].ToString(),
                        idTipo = Convert.ToInt32(row[15]),
                        idFiltro = Convert.ToInt32(row[16]),
                        sDscUsuario = row[17].ToString(),
                        sDscUsuarioAtualizacao = row[18].ToString(),
                        idDepartamento = Convert.ToInt32(row[19]),
                        sDscDepartamento = row[20].ToString(),
                        sDscExtra = row[21].ToString(),
                        nVlrConclusao = Convert.ToDecimal(row[22]),
                        idProjeto = Convert.ToInt32(row[23]),
                        idAtividade = Convert.ToInt32(row[24]),
                        sDscDescricao = row[25].ToString(),
                        idStatus = Convert.ToInt32(row[26]),
                        sDscStatus = row[27].ToString(),
                        sUsuarios = row[28].ToString(),
                        sUsuarios_NovoApontamento = row[29].ToString(),
                        nHoras_Realizadas = Convert.ToDecimal(row[30]),
                        nHoras_Previsao = Convert.ToDecimal(row[31]),
                        nHoras_Disponiveis = Convert.ToDecimal(row[32]),
                        dtInicial_Previsao = row[33].ToString(),
                        dtFinal_Previsao = row[34].ToString(),
                        sAssociaUsuario = row[35].ToString()
                    });
                }
            }

            context.Response.ContentType = "application/json";
            context.Response.Write(serializer.Serialize(atividades));
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public class UsuarioRequest
        {
            public string dtConsulta { get; set; }
        }

        public class Atividade
        {
            public int id { get; set; }
            public int idEvento { get; set; }
            public int idUsuario { get; set; }
            public string sDscTitulo { get; set; }
            public string sDscEvento { get; set; }
            public string sDiaInteiro { get; set; }
            public string sDisplay { get; set; }
            public string sUrl { get; set; }
            public string sCor { get; set; }
            public string sCorTexto { get; set; }
            public string dtInicial { get; set; }
            public string dtFinal { get; set; }
            public int idUsuarioAtualizacao { get; set; }
            public string dtAtualizacao { get; set; }
            public string sAtivo { get; set; }
            public int idTipo { get; set; }
            public int idFiltro { get; set; }
            public string sDscUsuario { get; set; }
            public string sDscUsuarioAtualizacao { get; set; }
            public int idDepartamento { get; set; }
            public string sDscDepartamento { get; set; }
            public string sDscExtra { get; set; }
            public decimal nVlrConclusao { get; set; }
            public int idProjeto { get; set; }
            public int idAtividade { get; set; }
            public string sDscDescricao { get; set; }
            public int idStatus { get; set; }
            public string sDscStatus { get; set; }
            public string sUsuarios { get; set; }
            public string sUsuarios_NovoApontamento { get; set; }
            public decimal nHoras_Realizadas { get; set; }
            public decimal nHoras_Previsao { get; set; }
            public decimal nHoras_Disponiveis { get; set; }
            public string dtInicial_Previsao { get; set; }
            public string dtFinal_Previsao { get; set; }
            public string sAssociaUsuario { get; set; }
        }
    }
}
