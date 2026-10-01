using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using TT.FrameWork;
using static TT.FrameWork.BD;

namespace TT_Flow.App
{
    public partial class Pagina_Ajax : Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        #region | WebMethods

        /// <summary>
        /// O @sTipo = -2 é exclusivo do filtro da aba Instalação/Obra do Produto (Serviço +
        /// Sub-Serviço + Recurso). Nenhuma outra tela manda esse valor, então liberar o termo
        /// vazio aqui não muda o comportamento de nenhum autocomplete existente.
        /// Chega como número no JSON, daí o Trim antes de comparar.
        /// </summary>
        static bool EhFiltroInstalacao(string sTipo)
        {
            return sTipo != null && sTipo.Trim() == "-2";
        }

        [WebMethod]
        public static string[] GetServicos_Recursos(string sDscProduto, string sTipo)
        {
            string sFuncao = "CONSULTAR";
            string sDscPesquisa = sDscProduto.Trim('"');
            List<string> lstProdutos = new List<string>();
            lstProdutos.Clear();

            // O filtro da aba Instalação/Obra abre a lista completa ao focar o campo, então
            // aceita termo vazio. Os demais continuam exigindo 3 caracteres: são buscas em
            // Produtos, onde trazer tudo não faria sentido nem caberia na tela.
            if (sDscPesquisa.Length > 3 || EhFiltroInstalacao(sTipo))
            {
                string sSql = "sp_Manipula_tbl_Flow_Produtos";

                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscPesquisa },
                    { "@sCodigo", sDscPesquisa },
                    { "@sSituacao", "S" },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                    { "@sTipo", sTipo }
                };
                DataTable tb = BD.ExecutarDataTable(sSql, vParametros, false);

                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}|{3}|{4}|{5}",
                        row["sDscProduto"],
                        row["sCodigo"],
                        row["idItem"],
                        row["sDscTipoProduto"],
                        row["idTipoProduto"],
                        row["sUnidade"]
                        );
                    lstProdutos.Add(lista);
                }
            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static string[] GetServicos_Recursos_Codigo(string sDscProduto, string sTipo)
        {
            string sFuncao = "CONSULTAR";
            string sDscPesquisa = sDscProduto.Trim('"');
            List<string> lstProdutos = new List<string>();
            lstProdutos.Clear();

            if (sDscPesquisa.Length > 0 || EhFiltroInstalacao(sTipo))
            {
                string sSql = "sp_Manipula_tbl_Flow_Produtos";

                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscPesquisa },
                    { "@sCodigo", sDscPesquisa },
                    { "@sSituacao", "S" },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                    { "@sTipo", sTipo }
                };
                DataTable tb = BD.ExecutarDataTable(sSql, vParametros, false);

                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}|{3}|{4}|{5}",
                        row["sCodigo"],
                        row["sDscProduto"],
                        row["idItem"],
                        row["sDscTipoProduto"],
                        row["idTipoProduto"],
                        row["sUnidade"]
                        );
                    lstProdutos.Add(lista);
                }


            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static string[] GetProdutos_x_Tipo(string sDscProduto, string idTipoProduto, string idFamilia, string idGrupo, string idPaisOrigem, string FTidParceiro, string sTipo)
        {
            string sFuncao = "CONSULTAR";
            string sDscPesquisa = sDscProduto.Trim('"');
            List<string> lstProdutos = new List<string>();
            lstProdutos.Clear();

            if (sDscProduto.Length > 3)
            {
                string sSql = "sp_Manipula_tbl_Flow_Produtos";

                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscPesquisa },
                    { "@sCodigo", sDscPesquisa },
                    { "@idTipoProduto", idTipoProduto },
                    { "@idFamilia", idFamilia },
                    { "@idGrupo", idGrupo },
                    { "@idPais", idPaisOrigem },
                    { "@sSituacao", "S" },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                    { "@sTipo", sTipo }
                };
                DataTable tb = BD.ExecutarDataTable(sSql, vParametros, false);

                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}|{3}|{4}",
                        row["sDscProduto"],
                        row["sCodigo"],
                        row["idItem"],
                        row["sUnidade"],
                        row["sDscTipoProduto"]
                        );
                    lstProdutos.Add(lista);
                }


            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static string[] GetProdutos(string sDscProduto, string idTipoProduto, string idFamilia, string idGrupo, string idPaisOrigem, string FTidParceiro)
        {
            string sFuncao = "CONSULTAR";
            string sDscPesquisa = sDscProduto.Trim('"');
            List<string> lstProdutos = new List<string>();
            lstProdutos.Clear();
            if (sDscProduto.Length > 3)
            {
                DataTable tb;
                string sSql = "sp_Manipula_tbl_Flow_Produtos";
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscPesquisa },
                    { "@sCodigo", sDscPesquisa },
                    { "@idTipoProduto", idTipoProduto },
                    { "@idFamilia", idFamilia },
                    { "@idGrupo", idGrupo },
                    { "@idPais", idPaisOrigem },
                    { "@sSituacao", "S" },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                };

                tb = BD.ExecutarDataTable(sSql, vParametros, false);
                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}|{3}|{4}{5}",
                        row["sDscProduto"],
                        row["sCodigo"],
                        row["idItem"],
                        row["sUnidade"],
                        row["sDscProduto"],
                        row["nImpostos"]
                        );
                    lstProdutos.Add(lista);
                }


            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static string[] GetProdutos_Codigo(string sDscProduto, string idTipoProduto, string idFamilia, string idGrupo, string idPaisOrigem, string FTidParceiro)
        {
            string sFuncao = "CONSULTAR";
            string sDscPesquisa = sDscProduto.Trim('"');
            List<string> lstProdutos = new List<string>();
            lstProdutos.Clear();
            if (sDscProduto.Length > 0)
            {
                DataTable tb;
                string sSql = "sp_Manipula_tbl_Flow_Produtos";
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscPesquisa },
                    { "@sCodigo", sDscPesquisa },
                    { "@idTipoProduto", idTipoProduto },
                    { "@idFamilia", idFamilia },
                    { "@idGrupo", idGrupo },
                    { "@idPais", idPaisOrigem },
                    { "@sSituacao", "S" },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                };

                tb = BD.ExecutarDataTable(sSql, vParametros, false);
                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}|{3}|{4}{5}",
                        row["sCodigo"],
                        row["sDscProduto"],
                        row["idItem"],
                        row["sUnidade"],
                        row["sDscProduto"],
                        row["nImpostos"]
                        );
                    lstProdutos.Add(lista);
                }


            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static string[] GetProdutos_Orcamento(string sDscProduto, string idTipoProduto, string idFamilia, string idGrupo, string idPaisOrigem, string idTabela, string idParceiro)
        {
            string sDscPesquisa = sDscProduto.Trim('"');
            List<string> lstProdutos = new List<string>();

            if (sDscProduto.Length > 0)
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_PRODUTOS_x_PARCEIRO" },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscPesquisa },
                    { "@sCodigo", sDscPesquisa },
                    { "@idTipoProduto", idTipoProduto },
                    { "@idFamilia", idFamilia },
                    { "@idGrupo", idGrupo },
                    { "@idPais", idPaisOrigem },
                    { "@idTabela", idTabela },
                    { "@sSituacao", "S" },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                    { "@idParceiro", idParceiro }
                };
                DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Produtos", vParametros, false);

                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}|{3}",
                        row["sDscProduto"],
                        row["sCodigo"],
                        row["idItem"],
                        row["nTotal"]
                        );
                    lstProdutos.Add(lista);
                }
            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static string[] GetProdutos_Orcamento_Codigo(string sDscProduto, string idTipoProduto, string idFamilia, string idGrupo, string idPaisOrigem, string idTabela, string idParceiro)
        {
            string sDscPesquisa = sDscProduto.Trim('"');
            List<string> lstProdutos = new List<string>();

            if (sDscProduto.Length > 0)
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_PRODUTOS_x_PARCEIRO" },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscPesquisa },
                    { "@sCodigo", sDscPesquisa },
                    { "@idTipoProduto", idTipoProduto },
                    { "@idFamilia", idFamilia },
                    { "@idGrupo", idGrupo },
                    { "@idPais", idPaisOrigem },
                    { "@idTabela", idTabela },
                    { "@sSituacao", "S" },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                    { "@idParceiro", idParceiro }
                };
                DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Produtos", vParametros, false);

                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}|{3}",
                        row["sCodigo"],
                        row["sDscProduto"],
                        row["idItem"],
                        row["nTotal"]
                        );
                    lstProdutos.Add(lista);
                }
            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static string[] GetProdutosCotacao(string sDscProduto, string idTipoProduto, string idFamilia, string idGrupo, string idPaisOrigem, string FTidParceiro)
        {
            string sFuncao = "CONSULTAR";
            string sDscPesquisa = sDscProduto.Trim('"');
            List<string> lstProdutos = new List<string>();
            lstProdutos.Clear();
            if (sDscProduto.Length > 3)
            {
                DataTable tb;
                string sSql = "sp_Manipula_tbl_Flow_Produtos";
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscPesquisa },
                    { "@sCodigo", sDscPesquisa },
                    { "@idTipoProduto", idTipoProduto },
                    { "@idFamilia", idFamilia },
                    { "@idGrupo", idGrupo },
                    { "@idPais", idPaisOrigem },
                    { "@sSituacao", "S" },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                };

                tb = BD.ExecutarDataTable(sSql, vParametros, false);
                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}|{3}|{4}{5}",
                        row["sDscProduto"],
                        row["sCodigo"],
                        row["idItem"],
                        row["sUnidade"],
                        row["sDscProduto"],
                        row["nImpostos"]
                        );
                    lstProdutos.Add(lista);
                }


            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static string[] GetParceiros(string sRazaoSocial)
        {
            string sFuncao = "CONSULTAR";
            string _sRazaoSocial = sRazaoSocial.Trim('"');
            List<string> lstProdutos = new List<string>();
            lstProdutos.Clear();
            if (sRazaoSocial.Length > 3)
            {
                DataTable tb;
                string sSql = "sp_Manipula_tbl_Flow_Clientes";
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sRazaoSocial", _sRazaoSocial }
                };

                tb = BD.ExecutarDataTable(sSql, vParametros, false);
                foreach (DataRow row in tb.Rows)
                {
                    string lista = "";
                    try
                    {
                        lista = string.Format(
                            "{0}|{1}|{2}",
                            row["sRazaoSocial"],
                            row["sCPF_CNPJ"].ToString().Replace(".", "").Replace("-", "").Replace("/", "").Length == 14 ? Convert.ToInt64(row["sCPF_CNPJ"].ToString()).ToString(@"00\.000\.000\/0000\-00") : row["sCPF_CNPJ"],
                            row["idCliente"]
                            );
                    }
                    catch
                    {
                        continue;
                    }
                    lstProdutos.Add(lista);
                }
            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static string[] GetParceiros_Codigo(string sCNPJ)
        {
            string sFuncao = "CONSULTAR";
            string _sRazaoSocial = sCNPJ.Trim('"');
            List<string> lstProdutos = new List<string>();
            lstProdutos.Clear();
            if (sCNPJ.Length > 0)
            {
                DataTable tb;
                string sSql = "sp_Manipula_tbl_Flow_Clientes";
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sRazaoSocial", _sRazaoSocial }
                };

                tb = BD.ExecutarDataTable(sSql, vParametros, false);
                foreach (DataRow row in tb.Rows)
                {
                    string lista = "";
                    try
                    {
                        lista = string.Format(
                            "{0}|{1}|{2}",
                            row["sCPF_CNPJ"].ToString().Replace(".", "").Replace("-", "").Replace("/", "").Length == 14 ? Convert.ToInt64(row["sCPF_CNPJ"].ToString()).ToString(@"00\.000\.000\/0000\-00") : row["sCPF_CNPJ"],
                            row["sRazaoSocial"],
                            row["idCliente"]
                            );
                    }
                    catch
                    {
                        continue;
                    }
                    lstProdutos.Add(lista);
                }
            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static string[] GetClientesParceiros(string sRazaoSocial, string sidTipoParceiro)
        {
            string sFuncao = "Flow_Parceiros";
            string _sRazaoSocial = sRazaoSocial.Trim('"');
            List<string> lstProdutos = new List<string>();
            lstProdutos.Clear();
            if (sRazaoSocial.Length > 3)
            {
                DataTable tb;
                string sSql = "sp_Manipula_tbl_Flow_Clientes";
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sRazaoSocial", _sRazaoSocial },
                    { "@sidTipoParceiro", sidTipoParceiro }
                };

                tb = BD.ExecutarDataTable(sSql, vParametros, false);
                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}",
                        row["sRazaoSocial"],
                        row["idCliente"],
                        row["sCPF_CNPJ"]
                        );
                    lstProdutos.Add(lista);
                }
            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static string[] GetColaboradores(string sDscColaborador)
        {
            string sFuncao = "Flow_Colaboradores";
            string _sDscColaborador = sDscColaborador.Trim('"');
            List<string> lstColaboradores = new List<string>();

            if (sDscColaborador.Length > 3)
            {
                DataTable tb;
                string sSql = "sp_Manipula_tbl_Flow_Colaboradores";
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sDscColaborador", _sDscColaborador }
                };

                tb = BD.ExecutarDataTable(sSql, vParametros, false);
                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}",
                        row["sDscColaborador"],
                        row["idColaborador"],
                        row["sCPF"]
                    );
                    lstColaboradores.Add(lista);
                }
            }

            return lstColaboradores.ToArray();
        }

        [WebMethod]
        public static string[] GetColaboradores_Codigo(string sCPF)
        {
            string sFuncao = "Flow_Colaboradores";
            string _sDscColaborador = sCPF.Trim('"');
            List<string> lstColaboradores = new List<string>();

            if (sCPF.Length > 0)
            {
                DataTable tb;
                string sSql = "sp_Manipula_tbl_Flow_Colaboradores";
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sDscColaborador", _sDscColaborador }
                };

                tb = BD.ExecutarDataTable(sSql, vParametros, false);
                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}",
                        row["sCPF"],
                        row["sDscColaborador"],
                        row["idColaborador"]
                    );
                    lstColaboradores.Add(lista);
                }
            }

            return lstColaboradores.ToArray();
        }

        [WebMethod]
        public static string[] GetRecurso(string sRecurso)
        {
            string _sDscRecurso = sRecurso.Trim('"');
            List<string> lstRecursos = new List<string>();
            lstRecursos.Clear();
            if (_sDscRecurso.Length > 1)
            {
                DataTable tb;
                string sSql = "sp_Flow_Valida_Recursos_x_Usuarios";
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@idUsuario", Identity.Variaveis.idUsuario() },
                    { "@sDscRecurso", _sDscRecurso },
                    { "@sSalvaLog", "N" }
                };

                tb = BD.ExecutarDataTable(sSql, vParametros, false);
                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}",
                        row["sDscRecurso"],
                        row["sURL"]
                    );
                    lstRecursos.Add(lista);
                }

                AdicionarRecursoIA(lstRecursos, _sDscRecurso, Permissao.IA.Consultar, "Assistente IA", "/App/Paginas/IA/Chat.aspx");
                AdicionarRecursoIA(lstRecursos, _sDscRecurso, Permissao.IA.VisualizarArquivos, "IA - Arquivos", "/App/Paginas/IA/Arquivos.aspx");
                AdicionarRecursoIA(lstRecursos, _sDscRecurso, Permissao.IA.BaseConhecimento, "IA - Conhecimento", "/App/Paginas/IA/Conhecimento.aspx");
                AdicionarRecursoIA(lstRecursos, _sDscRecurso, Permissao.IA.VisualizarUso, "IA - Uso", "/App/Paginas/IA/Uso.aspx");
                AdicionarRecursoIA(lstRecursos, _sDscRecurso, Permissao.IA.VisualizarAuditoria, "IA - Auditoria", "/App/Paginas/IA/Auditoria.aspx");
                AdicionarRecursoIA(lstRecursos, _sDscRecurso, Permissao.IA.AdministrarFerramentas, "IA - Configuracao", "/App/Paginas/IA/Configuracao.aspx");
            }

            return lstRecursos.ToArray();
        }

        private static void AdicionarRecursoIA(List<string> recursos, string termo, int idPermissao, string titulo, string url)
        {
            if (!Funcoes.ValidaPermissao(idPermissao, false))
                return;

            if (!TextoContem(titulo, termo) && !TextoContem(url, termo) && !TextoContem("Inteligencia Artificial IA", termo))
                return;

            string item = string.Format("{0}|{1}", titulo, url);
            if (!recursos.Any(r => r.EndsWith("|" + url, StringComparison.OrdinalIgnoreCase)))
                recursos.Add(item);
        }

        private static bool TextoContem(string texto, string termo)
        {
            return (texto ?? string.Empty).IndexOf(termo ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        //Agnes Partal * 19/07/2024 --------------------------------------------//
        [WebMethod]
        public static string GetProdutoDetalhes(string idProduto)
        {
            try
            {
                string imgUrl = "";
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                DataSet ds;
                vParametros.Add("@sFuncao", "CONSULTAR_PRODUTO");
                vParametros.Add("@idProduto", idProduto);
                vParametros.Add("@idTipoArquivo", "201");
                ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametros);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        DataRow imgBd = ds.Tables[1].Rows[0];
                        imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);
                    }

                    var produto = new
                    {
                        sCodigo = Retorno.DATASET(ds, 0, "sCodigo"),
                        sDsc = Retorno.DATASET(ds, 0, "sDscProduto"),
                        sTipo = Retorno.DATASET(ds, 0, "sDscTipoProduto"),
                        sGrupo = Retorno.DATASET(ds, 0, "sDscGrupo"),
                        sFabricante = Retorno.DATASET(ds, 0, "sFabricante"),
                        sLocalArmazenamento = Retorno.DATASET(ds, 0, "sDscLocalArmazenamento"),
                        sFamilia = Retorno.DATASET(ds, 0, "sDscFamilia"),
                        sCodigoCEST = Retorno.DATASET(ds, 0, "sCodigoCEST") != "" ? Retorno.DATASET(ds, 0, "sCodigoCEST") + " - " + Retorno.DATASET(ds, 0, "sDscCEST") : Retorno.DATASET(ds, 0, "sCodigoCEST"),
                        sCodigoNCM = Retorno.DATASET(ds, 0, "sCodigoNCM") != "" ? Retorno.DATASET(ds, 0, "sCodigoNCM") + " - " + Retorno.DATASET(ds, 0, "sDscNCM") : Retorno.DATASET(ds, 0, "sCodigoNCM"),
                        sCategoriaVendas = Retorno.DATASET(ds, 0, "sDscCategoriaVendas"),
                        sPaisOrigem = Retorno.DATASET(ds, 0, "sDscPais"),
                        imagem = imgUrl
                    };

                    return JsonConvert.SerializeObject(produto);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao obter os detalhes do produto: " + ex.Message);
                return JsonConvert.SerializeObject(new { error = true, message = ex.Message });
            }
        }
        //----------------------------------------------------------------------//

        [WebMethod]
        public static string SalvarEvento_Calendario(int idEvento, int idUsuario, string sDscTitulo, string sDscEvento, string sDiaInteiro, string sUrl, DateTime dtInicial, DateTime? dtFinal, string sAtivo)
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "SALVAR_EVENTO" },
                    { "@idEvento", idEvento.ToString() },
                    { "@idUsuario", idUsuario.ToString() },
                    { "@sDscTitulo", sDscTitulo },
                    { "@sDscEvento", sDscEvento },
                    { "@sDiaInteiro", sDiaInteiro },
                    { "@sUrl", sUrl },
                    { "@dtInicial", dtInicial.ToString() },
                    { "@dtFinal", dtFinal.ToString() },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                    { "@sAtivo", sAtivo }
                };
                DataTable dt = ExecutarDataTable("sp_Manipula_tbl_Flow_Calendario_Eventos_x_Usuario", vParametros);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0].Field<int>("nRet").Equals(0))
                        return string.Format("{4} \"retorno\": 0, \"id\": {0}, \"usuario\": \"{1}\", \"data\": \"{2}\", \"msg\": \"{3}\" {5}", dt.Rows[0].Field<int>("id"), dt.Rows[0].Field<string>("usuarioAtualizacao"), dt.Rows[0].Field<DateTime>("dtAtualizacao").ToString("MM/dd/yyyy HH:mm"), dt.Rows[0].Field<string>("sMsg").Replace("\"", "'"), "{", "}");
                    else
                        throw new Exception(dt.Rows[0].Field<string>("sMsg").Replace("\"", "'"));
                }
            }
            catch (Exception ex)
            {
                return string.Format("{1} \"retorno\": 1, \"msg\": {0} {2}", ex.Message, "{", "}");
            }

            return string.Empty;
        }

        [WebMethod]
        public static void Salvar_NovoApontamento_Atividades_Calendario(int idAtividade, int idUsuario, string sObservacao, decimal nHoras, DateTime dtApontamento)
        {
            Dictionary<string, string> vParametros_Apontamentos = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_APONTAMENTOS" },
                { "@idAtividade", idAtividade.ToString() },
                { "@idTipo", "4" },
                { "@sObservacao", sObservacao },
                { "@nHoras", nHoras.ToString().Replace(",", ".") },
                { "@dtApontamento", dtApontamento.ToString() },
                { "@idUsuario", idUsuario.ToString() }
            };
            DataSet ds_Apontamentos = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Atividades", vParametros_Apontamentos);

            if (!BD.ValidarDataSet(ds_Apontamentos, out string sErro) && !string.IsNullOrEmpty(sErro))
                throw new Exception("Erro ao Salvar os Apontamentos: " + sErro);
        }


        [WebMethod]
        public static string[] GetServicos_Fabricados(string sDscProduto)
        {
            string sTipo = "45";
            string sFuncao = "CONSULTAR";
            string sDscPesquisa = sDscProduto.Trim('"');
            List<string> lstProdutos = new List<string>();
            lstProdutos.Clear();

            if (sDscPesquisa.Length > 3)
            {
                string sSql = "sp_Manipula_tbl_Flow_Produtos";

                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscPesquisa },
                    { "@sCodigo", sDscPesquisa },
                    { "@sSituacao", "S" },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                    { "@sTipo", sTipo }
                };
                DataTable tb = BD.ExecutarDataTable(sSql, vParametros, false);

                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}|{3}|{4}|{5}",
                        row["sDscProduto"],
                        row["sCodigo"],
                        row["idItem"],
                        row["sDscTipoProduto"],
                        row["idTipoProduto"],
                        row["sUnidade"]
                        );
                    lstProdutos.Add(lista);
                }
            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static string[] GetServicos_Fabricados_Codigo(string sDscProduto)
        {
            string sTipo = "45";
            string sFuncao = "CONSULTAR";
            string sDscPesquisa = sDscProduto.Trim('"');
            List<string> lstProdutos = new List<string>();
            lstProdutos.Clear();

            if (sDscPesquisa.Length > 0)
            {
                string sSql = "sp_Manipula_tbl_Flow_Produtos";

                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscPesquisa },
                    { "@sCodigo", sDscPesquisa },
                    { "@sSituacao", "S" },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                    { "@sTipo", sTipo }
                };
                DataTable tb = BD.ExecutarDataTable(sSql, vParametros, false);

                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}|{3}|{4}|{5}",
                        row["sCodigo"],
                        row["sDscProduto"],
                        row["idItem"],
                        row["sDscTipoProduto"],
                        row["idTipoProduto"],
                        row["sUnidade"]
                        );
                    lstProdutos.Add(lista);
                }


            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static object Salvar_ModalUsuario(string idDashboard, string idPonto, string sSenha, string sConfirma_Senha)
        {
            string sErro = string.Empty;

            try
            {
                if (sSenha.Length < 3 || !sSenha.Equals(sConfirma_Senha)) { sErro = "A Senha deve possuir ao menos 3 caracteres e deve coincidir com a Confirmação!"; goto Retorno; }

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "SALVAR_EXIBICAO" },
                    { "@idDashboard", idDashboard },
                    { "@idPonto", idPonto },
                    { "@sSenha", sSenha },
                    { "@idUsuario", Identity.Variaveis.idUsuario() }
                };
                BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios", vParametros);
            }
            catch (Exception ex)
            {
                sErro = $"Houve um Erro ao Salvar as informações do Usuário!<br />Erro: {ex.Message}";
            }

        Retorno:
            return new { nRet = string.IsNullOrEmpty(sErro) ? 0 : 1, msg = string.IsNullOrEmpty(sErro) ? "Informações salvas com sucesso!" : sErro };
        }

        [WebMethod]
        public static Objeto_Autocomplete[] Get(List<Objeto_Autocomplete> list, string sDsc, string sCodigo)
        {
            sDsc = sDsc.Replace("\"", "").Trim().ToLower();
            return list.Where(s => Convert.ToBoolean(sCodigo) ? s.sCodigo.Trim().ToLower().Contains(sDsc) : s.sDsc.Trim().ToLower().Contains(sDsc)).ToArray();
        }

        public class Objeto_Autocomplete
        {
            public int id;
            public string sCodigo;
            public string sDsc;
        }

        #endregion
    }
}
