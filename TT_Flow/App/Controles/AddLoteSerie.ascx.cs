using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static TT.FrameWork.BD;
using TT.FrameWork;
using TT_Flow.FrameWork;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;

namespace TT_Flow.App.Controles
{
    public partial class AddLoteSerie : System.Web.UI.UserControl
    {
        #region | Classes
        public List<cls_LoteSeries> ls_Series
        {
            get
            {
                if (ViewState["ls_Series"] == null)
                {
                    ViewState["ls_Series"] = new List<cls_LoteSeries>();
                }
                return (List<cls_LoteSeries>)ViewState["ls_Series"];
            }
            set
            {
                ViewState["ls_Series"] = value;
            }
        }
        #endregion

        #region | Globais

        string sProcedure = "sp_Manipula_tbl_Flow_Produtos_Movimentacao";
        string sProcedureEtiqueta = "sp_Manipula_tbl_Flow_WMS_OPI_Etiqueta";
        string sProcedureLocal = "sp_Manipula_tbl_Flow_WMS_LocalArmazenamento";

        #endregion

        #region | Props
        public string IdObjeto { get; set; } //Objeto de Controle, caso não seja o IdMovimentação;
        public string STipoObjeto { get; set; } = "Movimentação";//Nome do Tipo do Objeto de Controle, caso não seja o Movimentação;
        public string IdProduto { get; set; }
        public double NQuantidade { get; set; }
        public string sDscObjeto { get; set; }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopularCombos();
                if (string.IsNullOrEmpty(STipoObjeto))
                {
                    MensagemPaginaControle.MostraMensagem("<b>Info: </b>Declare um <b>STipoObjeto</b> no hmtl com o tipo de objeto desejado para o controle ser liberado.", "info", false);
                    div_Controle.Visible = false;
                }
                if (IdObjeto == "0" || string.IsNullOrEmpty(IdObjeto))
                {
                    MensagemPaginaControle.MostraMensagem("<b>Info: </b>Inicialize o controle corretamente usando o método <b>InicializaProdutoSerie(idProduto, nQuantidade, idObjeto)</b>, para o controle ser liberado.", "info", false);
                    div_Controle.Visible = false;
                }

                if ((IdObjeto != "0" && !string.IsNullOrEmpty(IdObjeto)) && (!string.IsNullOrEmpty(STipoObjeto)))
                {
                    div_Controle.Visible = true;
                }
            }
            else
            {
                
            }
                
        }

        #region | Serielização

        public void InicializaProdutoSerie(string idProduto, int nQuantidade, string idObjeto)
        {
            string sFuncao = "CONSULTAR-PRODUTO";
            string sErro = "";
            DataSet dsProduto;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idProduto", idProduto);

            IdProduto = idProduto;

            dsProduto = BD.ExecutarDataSet(sProcedure, vParametros, false);
            if (BD.ValidarDataSet(dsProduto, out sErro))
            {
                lblTituloProduto.Text = RETORNO.DATASET(dsProduto, "sDscProduto");
                NQuantidade = nQuantidade;
                hddidProdutoSerie.Value = idProduto;
                hddnQtdSerie.Value = nQuantidade.ToString();
                hddsProdutoSerie.Value = lblTituloProduto.Text;
                IdObjeto = idObjeto;
            }
        }

        public void PopularSerie(string idObjeto)
        {
            string sFuncao = "CONSULTAR-SERIES";
            string sErro = "";
            DataSet dsSeries;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idProduto", IdProduto);

            if(STipoObjeto == "Movimentação")
               vParametros.Add("@idMovimentacao", idObjeto);
            else
               vParametros.Add("@idObjeto", idObjeto);

            dsSeries = BD.ExecutarDataSet(sProcedure, vParametros, false);

            if (BD.ValidarDataSet(dsSeries, out sErro))
            {
                ls_Series.Clear();

                ls_Series = dsSeries.Tables[0].AsEnumerable().Select(row =>
                {
                    return new cls_LoteSeries
                    {
                        IdProduto = Convert.ToInt32(row["idProduto"]),
                        SdscProduto = row["sDscProduto"].ToString(),
                        SCodigoBarras = row["sCodigoBarras"].ToString(),
                        IdLocal = Convert.ToInt32(row["idLocalArmazenamento"]),
                        SLote = row["sLote"].ToString(),
                        NSerie = row["nSerie"].ToString(),
                        SCodigo = row["sCodigo"].ToString(),
                        IdPosicao = Convert.ToInt32(row["idPosicao"]),
                        STipoObjeto = row["sTipoObjeto"].ToString(),
                        IdObjeto = Convert.ToInt32(row["idObjeto"])

                    };
                }).ToList();

                dtgSerie_DataBind();

                if (ls_Series.Count > 0)
                {
                    cmdSalvarSeries.Visible = true;
                }
                else
                {
                    cmdSalvarSeries.Visible = false;
                }

                if (!string.IsNullOrEmpty(hddnQtdSerie.Value) && hddnQtdSerie.Value == ls_Series.Count.ToString())
                {
                    cmdIncluirSerie.Visible = false;
                    if (ls_Series.Count == Convert.ToInt32(hddnQtdSerie.Value))
                    {
                        MensagemPaginaSeries.MostraMensagem("<b>Lembrete:</b> Limite de inclusão atingido. Salve os Itens Adicionados Para Atualizar.", "info", false);
                    }
                }
                else
                {
                    cmdIncluirSerie.Visible = true;
                }
                div_gvSerie.Visible = true;
            }
            else
            {
                div_gvSerie.Visible = false;
                MensagemPaginaSeries.MostraMensagem_Erro("Nenhuma Série Adicionada", false);
            }
        }
        void dtgSerie_DataBind()
        {
            dtgSerie.DataSource = ls_Series;
            try
            {
                dtgSerie.DataBind();
            }
            catch (Exception ex)
            {

                MensagemPaginaSeries.MostraMensagem_Erro($"ERRO: {ex}");
            }

        }
        protected void dtgSerie_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlArmazenamento = e.Row.FindControl("ddlArmazenamento") as DropDownList;
                DropDownList ddlPosicaoPai = e.Row.FindControl("ddlPosicaoPai") as DropDownList;

                FUNCOES.Popula_Combo(ddlArmazenamento, "sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_LOCAL_ARMAZENAMENTO'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Selecione o Local", "0");


                var item = (cls_LoteSeries)e.Row.DataItem;

                if (item != null && ddlArmazenamento.Items.FindByValue(item.IdLocal.ToString()) != null)
                {
                    ddlArmazenamento.SelectedValue = item.IdLocal.ToString();
                    FUNCOES.Popula_Combo(ddlPosicaoPai, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlArmazenamento.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Selecione a Posição", "0");
                    ddlPosicaoPai.SelectedValue = item.IdPosicao.ToString();
                }

                if (item.SExclusao == "S")
                {
                    e.Row.Visible = false;
                }

            }
        }
        bool AtualizarClasseSerie()
        {
            List<FrameWork.cls_LoteSeries> listaAtual = new List<FrameWork.cls_LoteSeries>();
            string sMensagemErro = "";

            foreach (GridViewRow row in dtgSerie.Rows)
            {
                TextBox txtsLote = (TextBox)row.FindControl("txtsLote");
                TextBox txtNSerie = (TextBox)row.FindControl("txtNSerie");
                DropDownList ddlArmazenamento = (DropDownList)row.FindControl("ddlArmazenamento");
                DropDownList ddlPosicaoPai = (DropDownList)row.FindControl("ddlPosicaoPai");

                HiddenField hddidRegistro = (HiddenField)row.FindControl("hddidRegistro");
                HiddenField hddidEtiqueta = (HiddenField)row.FindControl("hddidEtiqueta");
                HiddenField hddidProdutoSerie = (HiddenField)row.FindControl("hddidProdutoSerie");
                HiddenField hddsProdutoSerie = (HiddenField)row.FindControl("hddsProdutoSerie");
                HiddenField hddsCodigoBarras = (HiddenField)row.FindControl("hddsCodigoBarras");
                HiddenField hddidOPI = (HiddenField)row.FindControl("hddidOPI");
                HiddenField hddidObjeto = (HiddenField)row.FindControl("hddidObjeto");
                HiddenField hddsExclusao = (HiddenField)row.FindControl("hddsExclusao");
                HiddenField hddsTipoObjeto = (HiddenField)row.FindControl("hddsTipoObjeto");
                HiddenField hddidMovimentacao = (HiddenField)row.FindControl("hddidMovimentacao");
                HiddenField hddsTipoMov = (HiddenField)row.FindControl("hddsTipoMov");

                string lote = txtsLote.Text.Trim();
                string serie = txtNSerie.Text.Trim();
                int idArmazenamento = string.IsNullOrEmpty(ddlArmazenamento.SelectedValue) ? 0 : int.Parse(ddlArmazenamento.SelectedValue);

                int idRegistro = string.IsNullOrEmpty(hddidRegistro.Value) ? 0 : int.Parse(hddidRegistro.Value);
                int idEtiqueta = string.IsNullOrEmpty(hddidEtiqueta.Value) ? 0 : int.Parse(hddidEtiqueta.Value);
                int idProduto = string.IsNullOrEmpty(hddidProdutoSerie.Value) ? 0 : int.Parse(hddidProdutoSerie.Value);
                int idObjeto = string.IsNullOrEmpty(hddidObjeto.Value) ? 0 : int.Parse(hddidObjeto.Value);
                string sDscProduto = hddsProdutoSerie.Value;
                string sCodigoBarras = hddsCodigoBarras.Value;
                string sExclusao = hddsExclusao.Value;
                int idPosicao = string.IsNullOrEmpty(ddlPosicaoPai.SelectedValue) ? 0 : int.Parse(ddlPosicaoPai.SelectedValue);
                int idMovimentacao = string.IsNullOrEmpty(hddidMovimentacao.Value) ? 0 : int.Parse(hddidMovimentacao.Value);
                string sTipoObjeto = hddsTipoObjeto.Value;

                var itemExistente = listaAtual.FirstOrDefault(item => item.NSerie == serie || item.SLote == lote);
                if (itemExistente != null)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Não pode haver séries ou lotes repetidos.";
                }
                if (idArmazenamento == 0) //idPosicao == 0 Retirado
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O Local de Armazenamento não pode ser vazio.";
                }

                var novoItem = new FrameWork.cls_LoteSeries
                {
                    IdRegistro = idRegistro,
                    IdEtiqueta = idEtiqueta,
                    IdProduto = idProduto,
                    SdscProduto = sDscProduto,
                    SCodigoBarras = sCodigoBarras,
                    IdLocal = idArmazenamento,
                    SLote = lote,
                    NSerie = serie,
                    IdMovimentacao = idMovimentacao,
                    STipoObjeto = sTipoObjeto,
                    IdObjeto = hddsTipoObjeto.Value == "Movimentação" ? 0 : idObjeto,
                    SExclusao = sExclusao,
                    IdPosicao = idPosicao,
                    STipoMov = hddsTipoMov.Value
                };

                DataSet dsPesquisa;
                Dictionary<string, string> vParametros = new Dictionary<string, string>
        {
            { "@sFuncao", "VERIFICAR-SERIES" },
            { "@nSerie", novoItem.NSerie }
        };

                dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);
                if (!string.IsNullOrEmpty(RETORNO.DATASET(dsPesquisa, "nSerie")))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O Número de Série já existe no Banco de Dados e não pode ser repetido.";
                    //return false;
                }

                listaAtual.Add(novoItem);
            }

            if (!string.IsNullOrEmpty(sMensagemErro))
            {
                MensagemPaginaSeries.MostraMensagem_Erro(sMensagemErro, false);
                dtgSerie_DataBind();
                return false;
            }
            else
            {
                ls_Series = listaAtual;
                dtgSerie_DataBind();
                return true;
            }

        }
        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlLocal, "sp_Manipula_tbl_Flow_Produtos_Movimentacao 'FLOW_LOCAL_ARMAZENAMENTO'", "idLocalArmazenamento", "sDscLocalArmazenamento", false, "Selecione o Local", "0");
        }
        void LimparCamposSeries()
        {
            txtsLoteEtiquetas.Text = "";
            txtnSerie.Text = "";
            ddlLocal.SelectedValue = "0";

            hddidProdutoSerie.Value = "";
            hddnQtdSerie.Value = "";
            hddsProdutoSerie.Value = "";
        }
        bool ValidarDadosSeries()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (string.IsNullOrEmpty(txtsLoteEtiquetas.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Lote.";
            }
            if (string.IsNullOrEmpty(txtnSerie.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Número de Série.";
            }
            if (ddlLocal.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Local.";
            }
            //if (ddlPosicaoPai.SelectedValue == "0")
            //{
            //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Posição.";
            //}

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPaginaSeries.MostraMensagem_Erro(sMensagemErro, false);
            }

            return bRetorno;
        }
        protected void IncluirSerie_Click(object sender, EventArgs e)
        {
            string sMensagemErro = "";
            string id = ddlLocal.SelectedValue;
            if (ValidarDadosSeries())
            {
                int idLocal = Convert.ToInt32(ddlLocal.SelectedValue);
                string sLote = txtsLoteEtiquetas.Text;
                string nSerie = txtnSerie.Text;
                int idPosicao = Convert.ToInt32(ddlPosicaoPai.SelectedValue);

                var itemExistente = ls_Series.FirstOrDefault(item => item.NSerie == nSerie || item.SLote == sLote);

                if (itemExistente != null)
                {
                    MensagemPaginaSeries.MostraMensagem_Erro("Não pode haver séries repetidas.");
                }
                else
                {
                    var novoItem = new cls_LoteSeries()
                    {
                        IdProduto = Convert.ToInt32(hddidProdutoSerie.Value),
                        NQuantidade = Convert.ToInt32(hddnQtdSerie.Value),
                        SdscProduto = hddsProdutoSerie.Value.ToString(),
                        SLote = sLote.ToString(),
                        NSerie = nSerie.ToString(),
                        IdLocal = Convert.ToInt32(idLocal),
                        IdPosicao = Convert.ToInt32(idPosicao),
                        STipoObjeto = this.STipoObjeto.ToString()
                    };

                    var ultimoItemAdicionado = novoItem;

                    foreach (var item in ls_Series.Where(item => item.SExclusao == "N"))
                    {
                        DataSet dsPesquisa;
                        Dictionary<string, string> vParametros = new Dictionary<string, string>();
                        vParametros.Add("@sFuncao", "VERIFICAR-SERIES");
                        vParametros.Add("@nSerie", item.NSerie);

                        dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);
                        if (string.IsNullOrEmpty(RETORNO.DATASET(dsPesquisa, "nSerie")))
                        {
                            if (item.NSerie.Length >= 4 && item.SLote.Length >= 4)
                            {
                                dtgSerie_DataBind();
                            }
                            else
                            {
                                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "A Série e Lote Precisam ter 4 ou mais Caracateres.";
                            }
                        }
                        else
                        {
                            sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O Número de Série já existe no Banco de Dados e não pode ser repetido.";
                        }
                        //if (sMensagemErro != "")
                        //{                           
                        //    MensagemPaginaSeries.MostraMensagem_Erro(sMensagemErro, false);
                        //    //ls_EtiquetasSeries.Remove(ultimoItemAdicionado);
                        //}

                    }
                    if (!string.IsNullOrEmpty(sMensagemErro))
                    {
                        MensagemPaginaSeries.MostraMensagem_Erro(sMensagemErro, false);
                    }
                    else
                    {
                        ls_Series.Add(novoItem);
                        dtgSerie_DataBind();
                        div_gvSerie.Visible = true;
                    }
                }
                int qtdSeriesValidas = ls_Series.Count(i => i.SExclusao != "S");

                if (qtdSeriesValidas > 0)
                {
                    cmdSalvarSeries.Visible = true;
                }
                else
                {
                    cmdSalvarSeries.Visible = false;
                }
            }
            int qtdSeriesValidas2 = ls_Series.Count(i => i.SExclusao != "S");
            if (!string.IsNullOrEmpty(hddnQtdSerie.Value))
            {
                if (qtdSeriesValidas2 == Convert.ToInt32(hddnQtdSerie.Value))
                {
                    cmdIncluirSerie.Visible = false;
                    MensagemPaginaSeries.MostraMensagem("<b>Lembrete:</b> Limite de inclusão atingido. Salve os Itens Adicionados Para Atualizar.", "info", false);
                }
                else
                {
                    cmdIncluirSerie.Visible = true;
                }
            }
            else
            {
                cmdIncluirSerie.Visible = true;
            }



            LimparCamposSeries();
        }
        protected void SalvarSeries_Click(object sender, EventArgs e)
        {
            Salvar_Series();
        }
        protected void ExcluirSerie_Click(object sender, EventArgs e)
        {
            LinkButton btnExcluir = (LinkButton)sender;
            string idRegistro = btnExcluir.CommandArgument;

            if (idRegistro != "0")
            {
                var serie = ls_Series.FirstOrDefault(et => et.IdRegistro == Convert.ToInt32(idRegistro));
                if (serie != null)
                {
                    serie.SExclusao = "S";
                }

                GridViewRow rowToDelete = (GridViewRow)btnExcluir.NamingContainer;
                rowToDelete.Visible = false;
            }
            else
            {
                GridViewRow row = (GridViewRow)btnExcluir.NamingContainer;
                int rowIndex = row.RowIndex;
                ls_Series.RemoveAt(rowIndex);
            }

            dtgSerie.DataSource = ls_Series;
            dtgSerie.DataBind();

            int qtdSeriesValidas = ls_Series.Count(i => i.SExclusao != "S");

            if (qtdSeriesValidas > 0)
            {
                cmdSalvarSeries.Visible = true;
            }
            else
            {
                cmdSalvarSeries.Visible = false;
            }

            if (!string.IsNullOrEmpty(hddnQtdSerie.Value))
            {
                if (qtdSeriesValidas == Convert.ToInt32(hddnQtdSerie.Value))
                {
                    cmdIncluirSerie.Visible = false;
                    MensagemPaginaSeries.MostraMensagem("<b>Lembrete:</b> Limite de inclusão atingido. Salve os Itens Adicionados Para Atualizar.", "info", false);
                }
                else
                {
                    cmdIncluirSerie.Visible = true;
                }
            }


            //cmdSalvarEtiquetas.Visible = true;
        }
        void Salvar_Series()
        {
            if (AtualizarClasseSerie())
            {
                string sErro = "";

                try
                {
                    DataSet dsSalvar = new DataSet();
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    vParametros.Clear();

                    vParametros.Add("@sFuncao", "SALVAR-ITEM-SERIE");

                    ls_Series.ForEach(e =>
                    {
                        vParametros["@idRegistro"] = e.IdRegistro.ToString();
                        //vParametros["@idEtiqueta"] = hddidEtiqueta.Value;
                        vParametros["@idProduto"] = e.IdProduto.ToString();
                        //vParametros["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario();
                        vParametros["@idLocalArmazenamento"] = e.IdLocal.ToString();
                        vParametros["@sCodigoBarras"] = e.SCodigoBarras.ToString();
                        vParametros["@sLote"] = e.SLote.ToString();
                        vParametros["@nSerie"] = e.NSerie.ToString();
                        vParametros["@idPosicao"] = e.IdPosicao.ToString();
                        vParametros["@sExclusao"] = string.IsNullOrEmpty(e.SExclusao) ? "N" : e.SExclusao;
                        vParametros["@sTipoObjeto"] = e.STipoObjeto.ToString();
                        vParametros["@sTipoMov"] = e.STipoMov.ToString();

                        if (STipoObjeto != "Movimentação") 
                           vParametros["@idObjeto"] = e.IdObjeto.ToString();
                        else
                            vParametros["@idMovimentacao"] = e.IdMovimentacao.ToString();

                           dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);
                    });


                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        //Pesquisar(idOPI, false);                       
                        MensagemPaginaSeries.MostraMensagem_Sucesso("Series Salvas com Sucesso!");
                        //PopularSerie();
                    }
                    else
                    {
                        MensagemPaginaSeries.MostraMensagem_Erro("BD: " + sErro.ToString());
                    }

                }
                catch (Exception ex)
                {
                    MensagemPaginaSeries.MostraMensagem_Erro(ex.Message);
                }

                dtgSerie_DataBind();
            }

        }
        protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            string id = ddlLocal.SelectedValue;
            FUNCOES.Popula_Combo(ddlPosicaoPai, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlLocal.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Selecione a Posicao", "0");
        }
        protected void ddlLocal_Salvos_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlLocal = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlLocal.NamingContainer;
            DropDownList ddlPosicaoPai = (DropDownList)row.FindControl("ddlPosicaoPai");

            if (ddlPosicaoPai != null)
            {
                FUNCOES.Popula_Combo(ddlPosicaoPai, $"{sProcedureLocal} 'FLOW-POSICOES', @idLocalArmazenamento={ddlLocal.SelectedValue}", "idPosicao", "sCodigoLocal", false, "Selecione a Posição", "0");
            }
        }
        #endregion

    }
}