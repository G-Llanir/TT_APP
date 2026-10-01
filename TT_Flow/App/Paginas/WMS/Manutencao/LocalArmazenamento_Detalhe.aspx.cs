using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.Data.SqlClient;
using System.Data;
using TT_Flow.FrameWork.Classes;
using static Permissao.Patrimonio;

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class LocalArmazenamento_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Local de Armazenamento";
        string sProcedure = "sp_Manipula_tbl_Flow_WMS_LocalArmazenamento";
        public List<cls_WMS_Posicao> ls_Posicoes
        {
            get
            {
                if (ViewState["ls_Posicoes"] == null)
                {
                    ViewState["ls_Posicoes"] = new List<cls_WMS_Posicao>();
                }
                return (List<cls_WMS_Posicao>)ViewState["ls_Posicoes"];
            }
            set
            {
                ViewState["ls_Posicoes"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Registra_ScriptPosicao();
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(90, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(91, true);
                    Pesquisar("0");
                }
            }
            else
            {
                Registra_ScriptPosicao();
            }
        }

        protected void Pesquisar(string idLocalArmazenamento)
        {
            string sErro = "";
            try
            {
                LimpaCampos();
                if (idLocalArmazenamento != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<string, string> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idLocalArmazenamento", idLocalArmazenamento);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);


                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidLocalArmazenamento.Value = RETORNO.DATASET(dsPesquisa, 0, "idLocalArmazenamento");
                        txtidLocalArmazenamento.Text = RETORNO.DATASET(dsPesquisa, 0, "idLocalArmazenamento");
                        txtsDscLocalArmazenamento.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscLocalArmazenamento");

                        SwitchAtivo.Definir(RETORNO.DATASET(dsPesquisa, 0, "sComputaEstoque"), "Computa Estoque?", "");
                        SwitchAtivoPadrao.Definir(RETORNO.DATASET(dsPesquisa, 0, "sEstoquePadrao"), "Estoque Padrão?", "");
                        SwitchAtivoOPI.Definir(RETORNO.DATASET(dsPesquisa, 0, "sExibeOPI"), "Exibe em OPI?", "");

                        PopularPosicoes(hddidLocalArmazenamento.Value);
                        Popula_Combos();
                        ViewState["sCodigoPosicao"] = txtsDscLocalArmazenamento.Text.Substring(0, Math.Min(3, txtsDscLocalArmazenamento.Text.Length)).ToUpper() + txtidLocalArmazenamento.Text.PadLeft(2, '0') + ".";
                        txtsCodigoPosicao.Text = ViewState["sCodigoPosicao"].ToString();

                        div_posicionamento.Visible = true;
                        PainelAtualizacao.Visible = true;

                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscLocalArmazenamento.Text);
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(92);
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                {
                    div_posicionamento.Visible = false;
                }

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }
        void Popula_Combos()
        {
            Popula_didPosicaoPai();
            //FUNCOES.Popula_Combo(ddlidPosiçãoPai, $"{sProcedure} 'FLOW-POSICOES' ,@idLocalArmazenamento={hddidLocalArmazenamento.Value}", "idPosicao", "sCodigoLocal", false, "Selecione a Posicao", "0");
        }
        void Popula_didPosicaoPai()
        {
            string sFuncao = "FLOW-POSICOES";
            string sErro = "";
            DataSet dsPosicoesBanco;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idLocalArmazenamento", hddidLocalArmazenamento.Value);
            dsPosicoesBanco = BD.ExecutarDataSet(sProcedure, vParametros, false);

            List<cls_WMS_Posicao> posicoesExistentes = new List<cls_WMS_Posicao>();
            if (BD.ValidarDataSet(dsPosicoesBanco, out sErro))
            {
                posicoesExistentes = dsPosicoesBanco.Tables[0].AsEnumerable().Select(row =>
                {
                    return new cls_WMS_Posicao
                    {
                        IdPosicao = Convert.ToInt32(row["idPosicao"]),
                        SCodigoLocal = row["sCodigoLocal"].ToString()
                    };
                }).ToList();
            }

            var posicoesEmPreparo = ls_Posicoes
                .Where(p => p.SSituacao != "N" && p.IdPosicao == 0)
                .ToList();

            List<cls_WMS_Posicao> todasPosicoesParaCombo = new List<cls_WMS_Posicao>(posicoesExistentes);

            foreach (var novaPosicao in posicoesEmPreparo)
            {
                if (!todasPosicoesParaCombo.Any(p => p.SCodigoLocal == novaPosicao.SCodigoLocal))
                {
                    todasPosicoesParaCombo.Add(novaPosicao);
                }
            }

            todasPosicoesParaCombo = todasPosicoesParaCombo.OrderBy(p => p.SCodigoLocal).ToList();


            ddlidPosiçãoPai.Items.Clear();
            ddlidPosiçãoPai.AppendDataBoundItems = true;
            ddlidPosiçãoPai.Items.Add(new ListItem("Selecione a Posicao", "0"));

            foreach (var posicao in todasPosicoesParaCombo)
            {
                string value = posicao.IdPosicao != 0
                               ? posicao.IdPosicao.ToString()          // pai já salvo
                               : $"T|{posicao.IdTemporario}";          // pai temporário

                ddlidPosiçãoPai.Items.Add(new ListItem(posicao.SCodigoLocal, value));
            }
        }
        void LimpaCampos()
        {
            txtidLocalArmazenamento.Text = "Novo";
            txtsDscLocalArmazenamento.Text = "";
            hddidLocalArmazenamento.Value = "0";
            SwitchAtivo.Definir("N", "Computa Estoque?", "");
            SwitchAtivoPadrao.Definir("N", "Estoque Padrão?", "");
            SwitchAtivoOPI.Definir("N", "Exibe em OPI?", "");
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            cmdSalvar.Visible = FUNCOES.ValidaPermissao(91);
            lblTituloPagina.Text = sTituloPagina;
        }

        string msg = "";

        private bool ValidarDados()
        {
            if (txtsDscLocalArmazenamento.Text == "")
            {
                MensagemPagina.MostraMensagem_Erro("Informe um Descrição válida para o Local de Armazenamento");
                return false;
            }
            if (ValidaExistePadrao())
            {
                MensagemPagina.MostraMensagem_Erro(msg);
                SwitchAtivoPadrao.Definir("N", "Estoque Padrão?", "");
                return false;
            }

            return true;
        }

        bool ValidaExistePadrao()
        {
            string sFuncao = "VALIDA-EXISTE-PADRAO";
            string sErro = "";
            DataSet dsLocalPadrao;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idLocalArmazenamento", hddidLocalArmazenamento.Value);
            vParametros.Add("@sEstoquePadrao", SwitchAtivoPadrao.Recuperar());
            dsLocalPadrao = BD.ExecutarDataSet(sProcedure, vParametros, false);

            if (BD.ValidarDataSet(dsLocalPadrao, out sErro))
            {
                return false;
            }
            else
            {
                msg = sErro;
                return true;
            }
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {

                    string[] vidLocalArmazenamento = hddidLocalArmazenamento.Value.Split(',');
                    string idLocalArmazenamento = vidLocalArmazenamento[0].ToString();

                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idLocalArmazenamento", idLocalArmazenamento);
                    vParametros.Add("@sDscLocalArmazenamento", txtsDscLocalArmazenamento.Text);
                    vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    vParametros.Add("@sComputaEstoque", SwitchAtivo.Recuperar());
                    vParametros.Add("@sEstoquePadrao", SwitchAtivoPadrao.Recuperar());
                    vParametros.Add("@sExibeOPI", SwitchAtivoOPI.Recuperar());

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idLocalArmazenamento = RETORNO.DATASET(dsSalvar, 0, "idLocalArmazenamento");
                        hddidLocalArmazenamento.Value = idLocalArmazenamento;
                        //Salvar_Posicoes();
                        Pesquisar(idLocalArmazenamento);
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }

        }

        #region | Posicionamento
        void PopularPosicoes(string idLocal)
        {
            string sFuncao = "CONSULTAR-POSICAO";
            string sErro = "";
            DataSet dsPosicoes;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idLocalArmazenamento", idLocal);
            dsPosicoes = BD.ExecutarDataSet(sProcedure, vParametros, false);

            if (BD.ValidarDataSet(dsPosicoes, out sErro))
            {
                ls_Posicoes.Clear();

                ls_Posicoes = dsPosicoes.Tables[0].AsEnumerable().Select(row =>
                {
                    return new cls_WMS_Posicao
                    {
                        IdPosicao = Convert.ToInt32(row["idPosicao"]),
                        SDscPosicao = row["sDscPosicao"].ToString(),
                        SCodigoLocal = row["sCodigoLocal"].ToString(),
                        IdLocal = row["idLocal"] != DBNull.Value ? Convert.ToInt32(row["idLocal"]) : 0,
                        IdPosicaoPai = row["idPosicaoPai"] != DBNull.Value ? Convert.ToInt32(row["idPosicaoPai"]) : 0,
                        SDscLocal = row["sDscLocal"].ToString(),
                        SDscPai = row["sDscPosicaoPai"].ToString(),
                        SSituacao = row["sSituacao"].ToString(),
                    };
                }).ToList();

                div_gvPosicao.Visible = true;

                dtgPosicao_DataBind(ls_Posicoes);
            }
            else
            {
                div_gvPosicao.Visible = false;
                MensagemPaginaPosicao.MostraMensagem_Erro("Nenhuma Posição encontrada para o Local!", false);
            }
        }
        void dtgPosicao_DataBind(List<cls_WMS_Posicao> ls_Posicoes)
        {
            dtgPosicao.DataSource = ls_Posicoes;
            dtgPosicao.DataBind();
        }
        string MontarCodigoHierarquico(string codigoFilho, string prefixo, List<cls_WMS_Posicao> ls_Posicoes, string valorPai)
        {
            // Se não tem pai, retorna apenas prefixo + "." + código base
            if (string.IsNullOrEmpty(valorPai) || valorPai == "0")
                return $"{prefixo}.{codigoFilho}";

            cls_WMS_Posicao posicaoPai = null;

            if (valorPai.StartsWith("T|"))
            {
                Guid idTempPai = Guid.Parse(valorPai.Substring(2));
                posicaoPai = ls_Posicoes.FirstOrDefault(p => p.IdTemporario == idTempPai);
            }
            else if (int.TryParse(valorPai, out int idPaiReal) && idPaiReal != 0)
            {
                posicaoPai = ls_Posicoes.FirstOrDefault(p => p.IdPosicao == idPaiReal);
            }

            if (posicaoPai != null)
            {
                string sCodigoPai = posicaoPai.SCodigoLocal;

                // Remove prefixo do código pai (ex: "LOC13.")
                string caminhoPai = sCodigoPai.StartsWith(prefixo + ".")
                    ? sCodigoPai.Substring(prefixo.Length + 1)
                    : sCodigoPai;

                // Monta hierarquia: prefixo.caminhoPai.codigoFilho
                return $"{prefixo}.{caminhoPai}.{codigoFilho}".Replace("..", ".").Trim('.');
            }

            return $"{prefixo}.{codigoFilho}";
        }
        protected void IncluirPosicoes_Click(object sender, EventArgs e)
        {
            if (ValidarDadosPosicoes())
            {
                div_gvPosicao.Visible = true;

                int idPosicao = string.IsNullOrEmpty(hddidPosicao.Value) ? 0 : Convert.ToInt32(hddidPosicao.Value);
                string sCodigoFilho = txtsCodigoPosicao.Text.Trim().ToUpper(); // Ex: "LOC13.R02"
                string sDscPosicao = txtsDscPosicao.Text.Trim();

                string sPrefixo = sCodigoFilho.Substring(0, 5); // Ex: "LOC13"
                string codigoBase = sCodigoFilho.Length > 6 ? sCodigoFilho.Substring(6) : ""; // Remove "LOC13."

                string valorPai = ddlidPosiçãoPai.SelectedValue;

                string sCodigoHierarquico = MontarCodigoHierarquico(codigoBase, sPrefixo, ls_Posicoes, valorPai);

                var posicaoExistente = ls_Posicoes.FirstOrDefault(pos => pos.SCodigoLocal == sCodigoHierarquico);

                if (posicaoExistente != null)
                {
                    posicaoExistente.SDscPosicao = sDscPosicao;
                    posicaoExistente.SSituacao = ComboAtivo1.Situacao_Recuperar();

                    if (valorPai.StartsWith("T|"))
                    {
                        posicaoExistente.IdPosicaoPai = 0;
                        posicaoExistente.IdTemporarioPai = Guid.Parse(valorPai.Substring(2));
                    }
                    else
                    {
                        posicaoExistente.IdPosicaoPai = int.Parse(valorPai);
                        posicaoExistente.IdTemporarioPai = null;
                    }
                }
                else
                {
                    var novaPosicao = new cls_WMS_Posicao()
                    {
                        IdPosicao = idPosicao,
                        SDscPosicao = sDscPosicao,
                        SCodigoLocal = sCodigoHierarquico.ToUpper(),
                        IdLocal = Convert.ToInt32(hddidLocalArmazenamento.Value),
                        SDscLocal = txtsDscLocalArmazenamento.Text,
                        SDscPai = "Em Preparo",
                        IdUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario()),
                        SSituacao = ComboAtivo1.Situacao_Recuperar()
                    };

                    if (valorPai.StartsWith("T|"))
                    {
                        novaPosicao.IdPosicaoPai = 0;
                        novaPosicao.IdTemporarioPai = Guid.Parse(valorPai.Substring(2));
                    }
                    else
                    {
                        novaPosicao.IdPosicaoPai = int.Parse(valorPai);
                        novaPosicao.IdTemporarioPai = null;
                    }

                    ls_Posicoes.Add(novaPosicao);
                }

                dtgPosicao_DataBind(ls_Posicoes);
                Popula_didPosicaoPai();
                LimparCamposPosicoes();
            }
        }

        protected void ExcluirPosicoes_Click(object sender, EventArgs e)
        {
            LinkButton btnExcluir = (LinkButton)sender;
            string idPosicao = btnExcluir.CommandArgument;

            if (idPosicao != "0")
            {
                var posicao = ls_Posicoes.FirstOrDefault(p => p.IdPosicao == Convert.ToInt32(idPosicao));
                if (posicao != null)
                {
                    posicao.SSituacao = "N";
                }

                GridViewRow rowToDelete = (GridViewRow)btnExcluir.NamingContainer;
                rowToDelete.Visible = false;
            }
            else
            {
                GridViewRow row = (GridViewRow)btnExcluir.NamingContainer;
                int rowIndex = row.RowIndex;
                ls_Posicoes.RemoveAt(rowIndex);
            }

            int count = ls_Posicoes.Count(p => p.SSituacao == "S");

            if (count > 0)
            {
                div_gvPosicao.Visible = true;
            }
            else
            {
                div_gvPosicao.Visible = false;
            }

            dtgPosicao_DataBind(ls_Posicoes);
            Popula_didPosicaoPai();
        }

        protected void dtgPosicao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string situacao = "";

                cls_WMS_Posicao posicao = (cls_WMS_Posicao)e.Row.DataItem;

                if (posicao.SSituacao == "N")
                {
                    e.Row.Visible = false;
                }

                if (e.Row.DataItem != null)
                {
                    object valorSituacao = DataBinder.Eval(e.Row.DataItem, "SSituacao");
                    situacao = valorSituacao != null ? valorSituacao.ToString() : "";
                }

            }
        }
        protected void SalvarPosicoes_Click(object sender, EventArgs e)
        {
            Salvar_Posicoes();
        }
        void Salvar_Posicoes()
        {
            var mapaTempParaReal = new Dictionary<Guid, int>();
            string idLocal = hddidLocalArmazenamento.Value;

            // 1) Salva pais já reais (ou sem pai) primeiro
            foreach (var p in ls_Posicoes.Where(p => p.IdTemporarioPai == null))
                SalvaEAtualiza(p, idLocal, mapaTempParaReal);

            // 2) Agora resolve filhos cujo pai era temporário
            foreach (var f in ls_Posicoes.Where(p => p.IdTemporarioPai != null))
            {
                if (mapaTempParaReal.TryGetValue(f.IdTemporarioPai.Value, out int idPaiReal))
                    f.IdPosicaoPai = idPaiReal;  // substitui o pai

                SalvaEAtualiza(f, idLocal, mapaTempParaReal);
            }

            PopularPosicoes(idLocal);
            MensagemPaginaPosicao.MostraMensagem_Sucesso("Posições salvas com sucesso!");
            Popula_Combos();
            dtgPosicao_DataBind(ls_Posicoes);
        }

        void SalvaEAtualiza(cls_WMS_Posicao p, string idLocal,
                            Dictionary<Guid, int> mapaTemp)
        {
            var prm = new Dictionary<string, string>
            {
                ["@sFuncao"] = "SALVAR-POSICAO",
                ["@idPosicao"] = p.IdPosicao.ToString(),
                ["@idLocalArmazenamento"] = idLocal,
                ["@sDscPosicao"] = p.SDscPosicao,
                ["@sCodigoLocal"] = p.SCodigoLocal,
                ["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario(),
                ["@sSituacao"] = p.SSituacao ?? "S",
                ["@idPosicaoPai"] = p.IdPosicaoPai.ToString()
            };

            var ds = BD.ExecutarDataSet(sProcedure, prm);
            if (!BD.ValidarDataSet(ds, out string erro))
                throw new Exception(erro);

            // se era novo, grava ID real
            if (p.IdPosicao == 0 && ds.Tables[0].Columns.Contains("idPosicao"))
            {
                p.IdPosicao = Convert.ToInt32(ds.Tables[0].Rows[0]["idPosicao"]);
                mapaTemp[p.IdTemporario] = p.IdPosicao; // guarda p/ filhos
            }
        }
        bool ValidarDadosPosicoes()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (string.IsNullOrEmpty(txtsCodigoPosicao.Text))
            {
                sMensagemErro += "Informe o Código da Posição.";
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtsCodigoPosicao.Text.Substring(6), "^[RNE]\\d{2}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Código da Posição inválido. Use R, N ou E seguido de dois dígitos (ex: R01, N10, E05).";
            }

            if (string.IsNullOrEmpty(txtsDscPosicao.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a Descrição da Posição.";
            }

            if (!string.IsNullOrEmpty(sMensagemErro))
            {
                bRetorno = false;
                MensagemPaginaPosicao.MostraMensagem_Erro(sMensagemErro, false);
            }

            return bRetorno;
        }
        void LimparCamposPosicoes()
        {

            //txtsCodigoPosicao.Text = "";
            txtsDscPosicao.Text = "";
            ddlidPosiçãoPai.SelectedIndex = 0;

        }

        void Registra_ScriptPosicao()
        {
            string script = @"
            document.addEventListener('DOMContentLoaded', function () {
                let campo = document.getElementById('<%= txtsCodigoPosicao.ClientID %>');
                let prefixoFixo = campo.value.substring(0, 7); // Pegando os 7 primeiros caracteres

                // Função para garantir que o prefixo não seja modificado
                function mascaraCodigoPosicao() {
                    let valor = campo.value;

                    // Garante que o valor sempre começa com o prefixo fixo
                    if (!valor.startsWith(prefixoFixo)) {
                        campo.value = prefixoFixo;
                    }

                    let sufixo = valor.substring(prefixoFixo.length).toUpperCase();
                    let regex = /^[REN]?\d{0,2}$/;

                    if (!regex.test(sufixo)) {
                        campo.value = prefixoFixo + sufixo.substring(0, sufixo.length - 1);
                    }
                }

                campo.addEventListener('input', mascaraCodigoPosicao);

                // Evita que o usuário apague ou edite o prefixo
                campo.addEventListener('keydown', function (e) {
                    if (campo.selectionStart < prefixoFixo.length && e.key !== 'ArrowRight' && e.key !== 'ArrowLeft') {
                        e.preventDefault();
                    }
                });

                // Reexecuta o script após o UpdatePanel ser atualizado
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    mascaraCodigoPosicao();
                });
            });
        ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "MascaraCodigoPosicao", script, true);
        }
        #endregion
    }
}