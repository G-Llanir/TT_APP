using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using TT.FrameWork;
using TT_Flow.App.Paginas.PCP;
using static TT.FrameWork.BD;

namespace Api
{
    public class Veiculos : IHttpHandler
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;

            DataSet dsSessao =
                Autenticacao.ValidarSessao(context);

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
            string permissao =
                Autenticacao.obterPermissaoUsuario(dsSessao);

            if (!(permissao.IndexOf("|" + Permissao.Veiculos.Consultar.ToString() + "|") > -1))
            {
                context.Response.StatusCode = 403;

                RetornarJson(context, new
                {
                    sucesso = false,
                    mensagem = "Não tem permissão para visualizar a página."
                });
                return;
            }


            try
            {
                string funcao =
                    context.Request.QueryString["sFuncao"] ?? "LISTAR";

                switch (funcao.ToUpperInvariant())
                {
                    case "LISTAR":
                        Listar(context);
                        break;

                    case "CADASTRAR":
                        Cadastrar(context, idUsuario);
                        break;

                    case "MOVIMENTACOES":
                        ListaMovimentacoes(context, idUsuario);
                        break;

                    case "DETALHES":
                        MovimentacoesDetalhes(context);
                        break;

                    default:
                        RetornarJson(context, new
                        {
                            sucesso = false,
                            mensagem = "sFuncao inválida."
                        });
                        break;
                }
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

        private void Listar(HttpContext context)
        {
            string tabela =
                context.Request.QueryString["sTabela"];

            if (string.IsNullOrWhiteSpace(tabela))
            {
                RetornarJson(context, new
                {
                    sucesso = false,
                    mensagem = "Informe sTabela."
                });

                return;
            }

            DataSet ds =
                BD.ExecutarDataSet(
                    "sp_Select",
                    new Dictionary<string, string>
                    {
                        { "@sTabela", tabela }
                    });

            RetornarJson(
                context,
                ConverterTabela(ds)
            );
        }

        private void ListaMovimentacoes(
            HttpContext context,
            string idUsuario)
        {
            Dictionary<string, string> vParametros =
                new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTA_MOVIMENTACAO" },
                    { "@sPesquisa", "" },
                    { "@idVeiculo", "0" },
                    { "@idUsuario", idUsuario }
                };

            DataSet ds =
                ExecutarDataSet(
                    "sp_Manipula_tbl_Flow_Veiculos",
                    vParametros);

            RetornarJson(
                context,
                ConverterTabela(ds)
            );
        }

        private void MovimentacoesDetalhes(
            HttpContext context)
        {
            string idMov =
                context.Request.QueryString["idMov"];

            if (string.IsNullOrWhiteSpace(idMov))
            {
                RetornarJson(context, new
                {
                    sucesso = false,
                    mensagem = "Informe idMov."
                });

                return;
            }

            Dictionary<string, string> vParametros =
                new Dictionary<string, string>
                {
                    {
                        "@sFuncao",
                        "CONSULTA_MOVIMENTACAO_DETALHE"
                    },
                    {
                        "@idMovimentacao",
                        idMov
                    }
                };

            DataSet ds =
                ExecutarDataSet(
                    "sp_Manipula_tbl_Flow_Veiculos",
                    vParametros);

            if (ds == null ||
                ds.Tables.Count == 0)
            {
                RetornarJson(context, new
                {
                    sucesso = false,
                    mensagem = "A procedure não retornou dados."
                });

                return;
            }

            RetornarJson(context, new
            {
                movimentacao =
                    ConverterDataTable(ds.Tables[0]),

                composicao =
                    ds.Tables.Count > 1
                        ? ConverterDataTable(ds.Tables[1])
                        : new List<object>()
            });
        }

        private void Cadastrar(
            HttpContext context,
            string idUsuario)
        {
            string json;

            using (StreamReader reader =
                new StreamReader(context.Request.InputStream))
            {
                json = reader.ReadToEnd();
            }

            if (string.IsNullOrWhiteSpace(json))
                throw new Exception(
                    "O corpo da requisição está vazio.");

            VeiculoRequest request =
                new JavaScriptSerializer
                {
                    MaxJsonLength = int.MaxValue
                }.Deserialize<VeiculoRequest>(json);

            if (request == null)
                throw new Exception("Dados inválidos.");

            int idMovimentacao =
                SalvarMovimentacao(
                    request,
                    idUsuario
                );

            SalvarComposicoes(
                request,
                idMovimentacao,
                idUsuario
            );

            RetornarJson(context, new
            {
                sucesso = true,
                idMovimentacao = idMovimentacao,
                mensagem = "Veículo cadastrado com sucesso."
            });
        }

        private int SalvarMovimentacao(
            VeiculoRequest item,
            string idUsuario)
        {
            var parametros =
                new Dictionary<string, string>
                {
                    {
                        "@sFuncao",
                        "SALVAR_MOVIMENTACAO"
                    },
                    {
                        "@idMovimentacao",
                        ValorInteiro(item.idMovimentacao)
                    },
                    {
                        "@idVeiculo",
                        ValorInteiro(item.idVeiculo)
                    },
                    {
                        "@sMarca_Movimentacao",
                        !item.bAlugado
                            ? null
                            : item.sMarca_Movimentacao
                    },
                    {
                        "@sModelo_Movimentacao",
                        !item.bAlugado
                            ? null
                            : item.sModelo_Movimentacao
                    },
                    {
                        "@sPlaca_Movimentacao",
                        !item.bAlugado
                            ? null
                            : item.sPlaca_Movimentacao
                    },
                    {
                        "@sAnoModelo_Movimentacao",
                        !item.bAlugado
                            ? null
                            : item.sAnoModelo_Movimentacao
                    },
                    {
                        "@sCor_Movimentacao",
                        !item.bAlugado
                            ? null
                            : item.sCor_Movimentacao
                    },
                    {
                        "@sSeguro_Movimentacao",
                        !item.bAlugado
                            ? null
                            : item.sSeguro_Movimentacao
                    },
                    {
                        "@sRastreador_Movimentacao",
                        !item.bAlugado
                            ? null
                            : item.sRastreador_Movimentacao
                    },
                    {
                        "@sLocadora_Movimentacao",
                        !item.bAlugado
                            ? null
                            : item.sLocadora_Movimentacao
                    },
                    {
                        "@dtRetirada_Movimentacao",
                        !item.bAlugado
                            ? null
                            : item.dtRetirada_Movimentacao
                    },
                    {
                        "@dtDevolucao_Movimentacao",
                        !item.bAlugado
                            ? null
                            : item.dtDevolucao_Movimentacao
                    },
                    {
                        "@sRodizio_Movimentacao",
                        !item.bAlugado
                            ? null
                            : item.sRodizio_Movimentacao
                    },
                    {
                        "@idUsuario",
                        idUsuario
                    }
                };

            DataSet ds =
                BD.ExecutarDataSet(
                    "sp_Manipula_tbl_Flow_Veiculos",
                    parametros);

            return ObterIdMovimentacao(ds);
        }

        private void SalvarComposicoes(
            VeiculoRequest request,
            int idMovimentacao,
            string idUsuario)
        {
            if (request.viagens == null ||
                request.viagens.Count == 0)
                return;

            int idRegistro = -1;

            foreach (ViagemRequest item in request.viagens)
            {
                var parametros =
                    new Dictionary<string, string>
                    {
                        {
                            "@sFuncao",
                            "SALVAR_MOVIMENTACAO_COMPOSICAO"
                        },
                        {
                            "@idMovimentacao",
                            idMovimentacao.ToString()
                        },
                        {
                            "@idRegistro",
                            idRegistro.ToString()
                        },
                        {
                            "@idTipoViagem",
                            ValorInteiro(item.idTipoViagem)
                        },
                        {
                            "@idCentroCusto_Movimentacao",
                            ValorInteiro(
                                item.idCentroCusto_Movimentacao)
                        },
                        {
                            "@sObservacao",
                            item.sObservacao
                        },
                        {
                            "@nKilometros",
                            ValorDecimal(item.nKilometros)
                        },
                        {
                            "@idSituacao_Tanque",
                            ValorInteiro(item.idSituacao_Tanque)
                        },
                        {
                            "@nLatitude",
                            ValorDecimal(item.nLatitude)
                        },
                        {
                            "@nLongitude",
                            ValorDecimal(item.nLongitude)
                        },
                        {
                            "@idUsuario",
                            idUsuario
                        },
                        {
                            "@sPneusDianteiros",
                            item.sPneusDianteiros
                        },
                        {
                            "@sPneusTraseiros",
                            item.sPneusTraseiros
                        },
                        {
                            "@sRodasDiateiras",
                            item.sRodasDiateiras
                        },
                        {
                            "@sRodasTraseiras",
                            item.sRodasTraseiras
                        },
                        {
                            "@sBancos",
                            item.sBancos
                        },
                        {
                            "@sPainel",
                            item.sPainel
                        },
                        {
                            "@sConsoles",
                            item.sConsoles
                        },
                        {
                            "@sForro",
                            item.sForro
                        },
                        {
                            "@sTapetes",
                            item.sTapetes
                        },
                        {
                            "@sCalotas",
                            item.sCalotas
                        },
                        {
                            "@sRetrovisores",
                            item.sRetrovisores
                        },
                        {
                            "@sPalhetas",
                            item.sPalhetas
                        },
                        {
                            "@sTriangulo",
                            item.sTriangulo
                        },
                        {
                            "@sMacaco",
                            item.sMacaco
                        },
                        {
                            "@sEstepe",
                            item.sEstepe
                        },
                        {
                            "@sBateria",
                            item.sBateria
                        },
                        {
                            "@sChaves",
                            item.sChaves
                        },
                        {
                            "@sDocumentos",
                            item.sDocumentos
                        },
                        {
                            "@sSom",
                            item.sSom
                        },
                        {
                            "@sCaixaSelada",
                            item.sCaixaSelada
                        },
                        {
                            "@vbArquivo_Painel",
                            item.vbArquivo_Painel
                        },
                        {
                            "@vbArquivo_Frente",
                            item.vbArquivo_Frente
                        },
                        {
                            "@vbArquivo_Traseira",
                            item.vbArquivo_Traseira
                        },
                        {
                            "@vbArquivo_Lat_Direita",
                            item.vbArquivo_Lat_Direita
                        },
                        {
                            "@vbArquivo_Lat_Esquerda",
                            item.vbArquivo_Lat_Esquerda
                        }
                    };

                BD.ExecutarDataSet(
                    "sp_Manipula_tbl_Flow_Veiculos",
                    parametros);

                idRegistro--;
            }
        }

        private int ObterIdMovimentacao(DataSet ds)
        {
            if (ds == null ||
                ds.Tables.Count == 0 ||
                ds.Tables[0].Rows.Count == 0)
            {
                throw new Exception(
                    "A procedure não retornou o idMovimentacao.");
            }

            DataRow row =
                ds.Tables[0].Rows[0];

            if (row.Table.Columns.Contains(
                "idMovimentacao"))
            {
                return Convert.ToInt32(
                    row["idMovimentacao"]);
            }

            return Convert.ToInt32(row[0]);
        }

        private object ConverterTabela(DataSet ds)
        {
            var lista =
                new List<object>();

            if (ds == null ||
                ds.Tables.Count == 0)
                return lista;

            return ConverterDataTable(
                ds.Tables[0]
            );
        }

        private List<object> ConverterDataTable(
            DataTable tabela)
        {
            var lista =
                new List<object>();

            if (tabela == null)
                return lista;

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

        private string ValorInteiro(int? valor)
        {
            return valor.HasValue
                ? valor.Value.ToString(
                    CultureInfo.InvariantCulture)
                : "";
        }

        private string ValorDecimal(decimal? valor)
        {
            return valor.HasValue
                ? valor.Value.ToString(
                    CultureInfo.InvariantCulture)
                : "";
        }

        private void RetornarJson(
            HttpContext context,
            object objeto)
        {
            context.Response.Write(
                new JavaScriptSerializer
                {
                    MaxJsonLength = int.MaxValue
                }.Serialize(objeto));
        }
    }

    public class VeiculoRequest
    {
        public int? idMovimentacao { get; set; }
        public int? idVeiculo { get; set; }
        public bool bAlugado { get; set; }

        public string sMarca_Movimentacao { get; set; }
        public string sModelo_Movimentacao { get; set; }
        public string sPlaca_Movimentacao { get; set; }
        public string sAnoModelo_Movimentacao { get; set; }
        public string sCor_Movimentacao { get; set; }
        public string sSeguro_Movimentacao { get; set; }
        public string sRastreador_Movimentacao { get; set; }
        public string sLocadora_Movimentacao { get; set; }
        public string dtRetirada_Movimentacao { get; set; }
        public string dtDevolucao_Movimentacao { get; set; }
        public string sRodizio_Movimentacao { get; set; }

        public List<ViagemRequest> viagens { get; set; }
    }

    public class ViagemRequest
    {
        public int? idRegistro { get; set; }
        public int? idTipoViagem { get; set; }
        public int? idCentroCusto_Movimentacao { get; set; }
        public int? idSituacao_Tanque { get; set; }

        public string sObservacao { get; set; }
        public decimal? nKilometros { get; set; }
        public decimal? nLatitude { get; set; }
        public decimal? nLongitude { get; set; }

        public string sPneusDianteiros { get; set; }
        public string sPneusTraseiros { get; set; }
        public string sRodasDiateiras { get; set; }
        public string sRodasTraseiras { get; set; }
        public string sBancos { get; set; }
        public string sPainel { get; set; }
        public string sConsoles { get; set; }
        public string sForro { get; set; }
        public string sTapetes { get; set; }
        public string sCalotas { get; set; }
        public string sRetrovisores { get; set; }
        public string sPalhetas { get; set; }
        public string sTriangulo { get; set; }
        public string sMacaco { get; set; }
        public string sEstepe { get; set; }
        public string sBateria { get; set; }
        public string sChaves { get; set; }
        public string sDocumentos { get; set; }
        public string sSom { get; set; }
        public string sCaixaSelada { get; set; }

        public string vbArquivo_Painel { get; set; }
        public string vbArquivo_Frente { get; set; }
        public string vbArquivo_Traseira { get; set; }
        public string vbArquivo_Lat_Direita { get; set; }
        public string vbArquivo_Lat_Esquerda { get; set; }
    }
}
