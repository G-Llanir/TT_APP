using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;
using IDENTITY = TT.FrameWork.Identity;
using BD = TT.FrameWork.BD;

namespace TT_Colaborador.Aplicativo.Paginas.Despesas
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // ESTA LINHA É ESSENCIAL PARA O AJAX FUNCIONAR:
    [System.Web.Script.Services.ScriptService]
    public class DespesasService : System.Web.Services.WebService
    {
        static string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Relatorio_Despesas";

        // --- DTOs ---
        public class DtoHeader
        {
            public string Motivo { get; set; }
            public string CentroCusto { get; set; }
            public string Periodo { get; set; }
            public List<DtoTipo> ListaTipos { get; set; }
            public List<DtoLancamento> MeusLancamentos { get; set; }
        }

        public class DtoTipo { public string Id { get; set; } public string Descricao { get; set; } }

        public class DtoLancamento
        {
            public int IdItem { get; set; }
            public string Data { get; set; }
            public string Tipo { get; set; }
            public string Descricao { get; set; }
            public string ValorFormatado { get; set; }
            public decimal ValorDecimal { get; set; }
            public bool TemAnexo { get; set; }
        }

        public class DtoInputLancamento
        {
            public string IdDespesa { get; set; }
            public string IdTipo { get; set; }
            public string Data { get; set; }
            public string Valor { get; set; }
            public string Descricao { get; set; }
            public string NomeArquivo { get; set; }
            public string BufferArquivo { get; set; }
        }

        public class DtoRetorno { public bool Sucesso { get; set; } public string Mensagem { get; set; } }

        // --- MÉTODOS ---

        [WebMethod(EnableSession = true)]
        public DtoHeader CarregarDadosIniciais(string idDespesa)
        {
            if (IDENTITY.Variaveis.idUsuario() == "0") return null; // Retorna null se não logado

            var retorno = new DtoHeader();
            retorno.ListaTipos = new List<DtoTipo>();
            retorno.MeusLancamentos = new List<DtoLancamento>();

            try
            {
                // 1. Header
                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("@sFuncao", "CONSULTAR_DETALHE");
                p.Add("@idDespesas", idDespesa);
                DataSet ds = BD.ExecutarDataSet(sProcedure, p);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    retorno.Motivo = dr["sDscMotivo"].ToString();
                    DateTime dtIni = dr["dtInicioVigencia"] != DBNull.Value ? Convert.ToDateTime(dr["dtInicioVigencia"]) : DateTime.Now;
                    DateTime dtFim = dr["dtFimVigencia"] != DBNull.Value ? Convert.ToDateTime(dr["dtFimVigencia"]) : DateTime.Now;
                    retorno.Periodo = $"{dtIni:dd/MM} a {dtFim:dd/MM/yyyy}";
                    retorno.CentroCusto = "CC " + dr["idPedido"].ToString();
                }

                // 2. Tipos
                Dictionary<string, string> pTipos = new Dictionary<string, string> { { "@sFuncao", "FLOW-TIPOS" } };
                DataSet dsTipos = BD.ExecutarDataSet(sProcedure, pTipos);
                foreach (DataRow row in dsTipos.Tables[0].Rows)
                {
                    retorno.ListaTipos.Add(new DtoTipo { Id = row["idTipoGastos"].ToString(), Descricao = row["sDscGasto"].ToString() });
                }

                // 3. Lançamentos
                Dictionary<string, string> pLanc = new Dictionary<string, string>();
                pLanc.Add("@sFuncao", "CONSULTAR_LANCAMENTOS");
                pLanc.Add("@idDespesas", idDespesa);
                DataSet dsLanc = BD.ExecutarDataSet(sProcedure, pLanc);

                if (dsLanc.Tables.Count > 0)
                {
                    string idUserLogado = IDENTITY.Variaveis.idUsuario().ToString();
                    foreach (DataRow row in dsLanc.Tables[0].Rows)
                    {
                        string donoDoLancamento = "";
                        if (dsLanc.Tables[0].Columns.Contains("sidParticipantes"))
                            donoDoLancamento = row["sidParticipantes"].ToString();

                        if (donoDoLancamento == idUserLogado || donoDoLancamento == "")
                        {
                            retorno.MeusLancamentos.Add(new DtoLancamento
                            {
                                IdItem = Convert.ToInt32(row["idItens"]),
                                Data = Convert.ToDateTime(row["dtDespesa"]).ToString("dd/MM/yyyy"),
                                Tipo = row["sDescTipo"].ToString(),
                                Descricao = row["sLocal"].ToString(),
                                ValorDecimal = Convert.ToDecimal(row["nValor"]),
                                ValorFormatado = Convert.ToDecimal(row["nValor"]).ToString("N2"),
                                TemAnexo = false
                            });
                        }
                    }
                }
            }
            catch { }
            return retorno;
        }

        [WebMethod(EnableSession = true)]
        public DtoRetorno SalvarLancamento(DtoInputLancamento dados)
        {
            if (IDENTITY.Variaveis.idUsuario() == "0") return new DtoRetorno { Sucesso = false, Mensagem = "Sessão expirada." };

            var resp = new DtoRetorno { Sucesso = false };
            try
            {
                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("@sFuncao", "INSERIR_DESPESAS");
                p.Add("@idDespesas", dados.IdDespesa);
                p.Add("@sTipoItem", "L");
                p.Add("@idTipoGastos", dados.IdTipo);
                p.Add("@nValor", dados.Valor.Replace("R$", "").Trim().Replace(".", "").Replace(",", "."));
                p.Add("@dtDespesa", DateTime.Parse(dados.Data).ToString("yyyy-MM-dd HH:mm:ss"));
                p.Add("@sLocal", dados.Descricao);
                p.Add("@sidParticipantes", IDENTITY.Variaveis.idUsuario().ToString());
                p.Add("@idUsuario", IDENTITY.Variaveis.idUsuario().ToString());
                p.Add("@sFrequencia", ""); p.Add("@nQtdDias", "0"); p.Add("@idFormaPagamento", "0"); p.Add("@idCategoriaPagar", "0");

                DataSet ds = BD.ExecutarDataSet(sProcedure, p);

                // Arquivo
                if (!string.IsNullOrEmpty(dados.BufferArquivo) && !string.IsNullOrEmpty(dados.NomeArquivo))
                {
                    string idGerado = "0";
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.Contains("idItens"))
                        idGerado = ds.Tables[0].Rows[0]["idItens"].ToString();

                    if (idGerado != "0")
                    {
                        byte[] bytesArquivo = Convert.FromBase64String(dados.BufferArquivo);
                        TT_Colaborador.FrameWork.cls_Arquivos objArquivo = new TT_Colaborador.FrameWork.cls_Arquivos();
                        objArquivo.idTipoArquivo = 8888;
                        objArquivo.idObjeto = int.Parse(idGerado);
                        objArquivo.sNomeArquivo = dados.NomeArquivo;
                        objArquivo.sDscArquivo = "Anexo Mobile";
                        objArquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        objArquivo.vbArquivo = bytesArquivo;
                        objArquivo.EnviarArquivo(objArquivo);
                    }
                }
                resp.Sucesso = true;
            }
            catch (Exception ex) { resp.Mensagem = ex.Message; }
            return resp;
        }

        [WebMethod(EnableSession = true)]
        public void ExcluirLancamento(string idItem)
        {
            if (IDENTITY.Variaveis.idUsuario() == "0") return;
            try
            {
                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("@sFuncao", "EXCLUIR_DESPESAS");
                p.Add("@idItens", idItem);
                BD.ExecutarDataSet(sProcedure, p);
            }
            catch { }
        }
    }
}