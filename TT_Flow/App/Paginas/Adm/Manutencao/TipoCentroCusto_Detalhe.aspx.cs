using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Paginas.Adm.Manutencao;
using TT_Flow.FrameWork.Classes;
using TT_Hub.App.Paginas.Adm.Manutencao;
using static Permissao.Patrimonio;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using GRID = TT.FrameWork.Grid;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.Adm.Manutencao
{
    public partial class TipoCentroCusto_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Tipos de Centro de Custo";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_TipoCentroCusto";
        public List<cls_CentroDeCusto_Categoria> bs_Categoria_CC
        {
            get
            {
                if (ViewState["bs_Categoria_CC"] == null)
                {
                    ViewState["bs_Categoria_CC"] = new List<cls_CentroDeCusto_Categoria>();
                }
                return (List<cls_CentroDeCusto_Categoria>)ViewState["bs_Categoria_CC"];
            }
            set
            {
                ViewState["bs_Categoria_CC"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.TpCentroCusto, true);      
            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString());

                }
                else
                {
                    Pesquisar("0");
                }
            }
            RegistraScript();
        }

        void LimpaCampos()
        {
            txtidTipoCentroCusto.Text = "Novo";
            txtsDscTipo.Text = "";
            hddidTipoCentroCusto.Value = "0";

            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
            //divCategoria.Visible = false;
        }
        void RegistraScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("$v192(function() {");

            // Aplica a máscara aos campos de inclusão (fora do GridView)
            sb.AppendLine("$('[id$=txtnTetoCategoria]').mask('0.000.000.009,90', { reverse: true });");
            sb.AppendLine("$('[id$=txtnSaldoCategoria]').mask('0.000.000.009,90', { reverse: true });");

            // Aplica a máscara aos campos do GridView (em modo de edição)
            sb.AppendLine("$('[id*=txtGridTeto]').mask('0.000.000.009,90', { reverse: true });");
            sb.AppendLine("$('[id*=txtGridSaldo]').mask('0.000.000.009,90', { reverse: true });");

            sb.AppendLine("});");


            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }
        protected void Pesquisar(string idTipoCentroCusto)
        {
            string sErro = "";
            try
            {
                LimpaCampos();
                if (idTipoCentroCusto != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<string, string> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idTipoCentroCusto", idTipoCentroCusto);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidTipoCentroCusto.Value = RETORNO.DATASET(dsPesquisa, 0, "idTipoCentroCusto");
                        txtidTipoCentroCusto.Text = RETORNO.DATASET(dsPesquisa, 0, "idTipoCentroCusto");
                        txtsDscTipo.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscTipo");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sAtivo"));

                        lblTituloPagina.Text = string.Format("Editar {0}: {1}", sTituloPagina, txtsDscTipo.Text);

                        CarregarGridCategorias(hddidTipoCentroCusto.Value);
                        //divCategoria.Visible = true;

                        cmdSalvar.Visible = false;

                        if (FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.AlterarTpCentroCusto, false))
                        {
                            cmdSalvar.Visible = true;
                        }
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                {
                    cmdSalvar.Visible = false;

                    if (FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.InserirTpCentroCusto, false))
                    {
                        cmdSalvar.Visible = true;
                    }
                    CarregarGridCategorias("0");
                    //divCategoria.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        private bool ValidarDados()
        {
            if (string.IsNullOrWhiteSpace(txtsDscTipo.Text))
            {
                MensagemPagina.MostraMensagem_Erro("Informe uma Descrição válida para o Tipo de Centro de Custo.");
                return false;
            }
            return true;
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idTipoCentroCusto", hddidTipoCentroCusto.Value);
                    vParametros.Add("@sDscTipo", txtsDscTipo.Text);
                    vParametros.Add("@sAtivo", ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        string idTipoCentroCusto = RETORNO.DATASET(dsSalvar, 0, "idTipoCentroCusto");
                        hddidTipoCentroCusto.Value = idTipoCentroCusto;

                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");

                        SalvarCategorias(idTipoCentroCusto);
                        Pesquisar(idTipoCentroCusto);
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro);
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }
            RegistraScript();
        }


        #region | Categorias
        private int DeletingRowIndex
        {
            get { return (int)(ViewState["DeletingRowIndex"] ?? -1); }
            set { ViewState["DeletingRowIndex"] = value; }
        }
        public List<cls_CentroDeCusto_Categoria> bs_CentroDeCusto_Categorias
        {
            get
            {
                if (ViewState["bs_CentroDeCusto_Categorias"] == null)
                {
                    ViewState["bs_CentroDeCusto_Categorias"] = new List<cls_CentroDeCusto_Categoria>();
                }
                return (List<cls_CentroDeCusto_Categoria>)ViewState["bs_CentroDeCusto_Categorias"];
            }
            set
            {
                ViewState["bs_CentroDeCusto_Categorias"] = value;
            }
        }

        private void CarregarGridCategorias(string idTipoCC)
        {
            hddidTipoCentroCusto.Value = idTipoCC;

            //if (!string.IsNullOrEmpty(hddidTipoCentroCusto.Value) && hddidTipoCentroCusto.Value != "0")
            //{
            //    btnSalvarCategorias.Visible = FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.AlterarTpCentroCusto, false);             
            //}
            //else
            //{
            //    btnSalvarCategorias.Visible = false;
            //}
       

            //SwitchAtivoPadrao.Definir("N", "É Padrão?", "N");

            try
            {
                var parametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR-CATEGORIA-CC" },
                    { "@idTipoCentroCusto", idTipoCC }
                };

                bs_CentroDeCusto_Categorias = BD.ExecutarLista<cls_CentroDeCusto_Categoria>(sProcedure, parametros, false);

                BindGridCategorias();
            }
            catch (Exception ex)
            {
                MensagemPaginaCategoria.MostraMensagem_Erro(ex.Message);
            }
        }

        private void BindGridCategorias()
        {
            gridCategorias.DataSource = bs_CentroDeCusto_Categorias;
            gridCategorias.DataBind();
            RegistraScript();
        }

        protected void cmdIncluirCategoriaCC_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtsDscCategoria.Text))
            {
                MensagemPaginaCategoria.MostraMensagem_Erro("A descrição da categoria é obrigatória.");
                return;
            }

            AtualizarListaComDadosDaGrid();

            string nomeCategoria = txtsDscCategoria.Text.Trim();

            if (bs_CentroDeCusto_Categorias.Any(c => c.sDscCategoria.Equals(nomeCategoria, StringComparison.OrdinalIgnoreCase)))
            {
                MensagemPaginaCategoria.MostraMensagem_Erro("Já existe uma categoria com este nome na lista.");
                return;
            }

            var novaCategoria = new cls_CentroDeCusto_Categoria
            {
                idCategoria = 0, // Será gerado pelo banco ao inserir
                sDscCategoria = txtsDscCategoria.Text,
                nTeto = BD.Conversoes.Numerico_Decimal(txtnTetoCategoria.Text),
                nSaldo = BD.Conversoes.Numerico_Decimal(txtnSaldoCategoria.Text),
                sPadrao = "N",
                idTipoCentroCusto = hddidTipoCentroCusto.Value != "0" ? (int?)Convert.ToInt32(hddidTipoCentroCusto.Value) : null
            };

            bs_CentroDeCusto_Categorias.Add(novaCategoria);

            BindGridCategorias();
            LimparCamposInclusaoCategoria();

            MensagemPaginaCategoria.MostraMensagem("Categoria adicionada à lista. Clique em 'Salvar' para registrar.", "info", false);
            RegistraScript();
        }
        bool SalvarCategorias(string idTipoCC)
        {
            AtualizarListaComDadosDaGrid();

            if (!ValidarCategorias())
            {
                return false; // A validação exibirá a mensagem de erro
            }

            try
            {
                foreach (var categoria in bs_CentroDeCusto_Categorias)
                {
                    // Lógica para salvar todas as categorias, independente de id ou tipo
                    var parametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR-CATEGORIA-CC" },
                        // Se idCategoria for 0 ou negativo, é um INSERT. Se for positivo, é um UPDATE.
                        { "@idCategoria", categoria.idCategoria > 0 ? categoria.idCategoria.ToString() : "0" },
                        { "@sDscCategoria", categoria.sDscCategoria },
                        { "@nTetoCategoria", categoria.nTeto?.ToString(CultureInfo.InvariantCulture) },
                        { "@nSaldoCategoria", categoria.nSaldo?.ToString(CultureInfo.InvariantCulture) },
                        { "@sPadrao", categoria.sPadrao },
                        { "@idTipoCentroCusto", categoria.idTipoCentroCusto?.ToString() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                        { "@sExclusao", "N" }
                    };

                    BD.ExecutarComandoLista(sProcedure, parametros, true);
                }

                MensagemPaginaCategoria.MostraMensagem_Sucesso("Categorias salvas com sucesso!");
                CarregarGridCategorias(hddidTipoCentroCusto.Value);
                return true;
            }
            catch (Exception ex)
            {
                MensagemPaginaCategoria.MostraMensagem_Erro($"Erro ao salvar categorias: {ex.Message}");
                return false;
            }
        }
        protected void btnSalvarCategorias_Click(object sender, EventArgs e)
        {
            SalvarCategorias(hddidTipoCentroCusto.Value);
            RegistraScript();
        }

        protected void gridCategorias_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gridCategorias.EditIndex = e.NewEditIndex;
            this.DeletingRowIndex = -1;
            BindGridCategorias();
        }

        protected void gridCategorias_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;
            gridCategorias.EditIndex = e.RowIndex;
            this.DeletingRowIndex = e.RowIndex;
            BindGridCategorias();
        }

        protected void gridCategorias_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gridCategorias.EditIndex = -1;
            this.DeletingRowIndex = -1;
            BindGridCategorias();
        }

        protected void gridCategorias_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            AtualizarListaComDadosDaGrid(e.RowIndex);
            gridCategorias.EditIndex = -1;
            this.DeletingRowIndex = -1;
            BindGridCategorias();
        }

        protected void gridCategorias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ConfirmDelete")
            {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);
                object key = gridCategorias.DataKeys[rowIndex].Value;

                var itemParaExcluir = bs_CentroDeCusto_Categorias.FirstOrDefault(c => c.GridKey.Equals(key));

                if (itemParaExcluir != null)
                {
                    if (itemParaExcluir.idCategoria > 0)
                    {

                        try
                        {
                            if (FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.AlterarTpCentroCusto, false))
                            {
                                var parametros = new Dictionary<string, string>
                            {
                                { "@sFuncao", "SALVAR-CATEGORIA-CC" },
                                { "@idCategoria", itemParaExcluir.idCategoria.ToString() },
                                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                                { "@sExclusao", "S" }
                            };
                                BD.ExecutarComandoLista(sProcedure, parametros, false);
                                bs_CentroDeCusto_Categorias.Remove(itemParaExcluir);
                                MensagemPaginaCategoria.MostraMensagem_Sucesso("Categoria excluída com sucesso!");
                            }
                            else
                            {
                                MensagemPaginaCategoria.MostraMensagem_Aviso("É Necessário ter a Permissão de alterar para excluir o itém");
                            }
                        }
                        catch (Exception ex)
                        {
                            MensagemPaginaCategoria.MostraMensagem_Erro($"Erro ao excluir categoria: {ex.Message}");
                            return;
                        }
                    }
                    else
                    {
                        bs_CentroDeCusto_Categorias.Remove(itemParaExcluir);
                        MensagemPaginaCategoria.MostraMensagem("Categoria removida da lista.", "info", false);
                    }

                    gridCategorias.EditIndex = -1;
                    this.DeletingRowIndex = -1;
                    BindGridCategorias();
                }
            }
        }

        protected void gridCategorias_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowState.HasFlag(DataControlRowState.Edit))
                {
                    var lnkSalvar = e.Row.FindControl("lnkSalvar");
                    var lnkCancelar = e.Row.FindControl("lnkCancelar");
                    var lnkConfirmarExclusao = e.Row.FindControl("lnkConfirmarExclusao");
                    var lnkCancelarExclusao = e.Row.FindControl("lnkCancelarExclusao");

                    bool isDeleting = (e.Row.RowIndex == this.DeletingRowIndex);

                    if (lnkSalvar != null) lnkSalvar.Visible = !isDeleting;
                    if (lnkCancelar != null) lnkCancelar.Visible = !isDeleting;
                    if (lnkConfirmarExclusao != null) lnkConfirmarExclusao.Visible = isDeleting;
                    if (lnkCancelarExclusao != null) lnkCancelarExclusao.Visible = isDeleting;

                    if (lnkSalvar.Visible)
                    {

                    }

                    if (isDeleting)
                    {
                        var txtDesc = e.Row.FindControl("txtGridDscCategoria") as TextBox;
                        var txtTeto = e.Row.FindControl("txtGridTeto") as TextBox;
                        var txtSaldo = e.Row.FindControl("txtGridSaldo") as TextBox;

                        if (txtDesc != null) txtDesc.ReadOnly = true;
                        if (txtTeto != null) txtTeto.ReadOnly = true;
                        if (txtSaldo != null) txtSaldo.ReadOnly = true;
                    }
                }
            }
            RegistraScript();
        }

        private void LimparCamposInclusaoCategoria()
        {
            txtsDscCategoria.Text = string.Empty;
            txtnTetoCategoria.Text = string.Empty;
            txtnSaldoCategoria.Text = string.Empty;
            //SwitchAtivoPadrao.Definir("N", "É Padrão?", "N");
        }

        private void AtualizarListaComDadosDaGrid(int rowIndex = -1)
        {
            int index = (rowIndex != -1) ? rowIndex : gridCategorias.EditIndex;

            if (index >= 0 && index < gridCategorias.Rows.Count)
            {
                GridViewRow row = gridCategorias.Rows[index];
                // Pega o valor da nossa nova chave única e confiável
                object key = gridCategorias.DataKeys[index].Value;

                // Encontra o item na lista comparando a GridKey de cada um com a chave da linha
                var itemParaAtualizar = bs_CentroDeCusto_Categorias.FirstOrDefault(c => c.GridKey.Equals(key));

                if (itemParaAtualizar != null)
                {
                    string novoNomeCategoria = ((TextBox)row.FindControl("txtGridDscCategoria")).Text.Trim();

                    if (bs_CentroDeCusto_Categorias.Any(c => c.sDscCategoria.Equals(novoNomeCategoria, StringComparison.OrdinalIgnoreCase) && !c.GridKey.Equals(key)))
                    {
                        // Mostra a mensagem de erro e impede a atualização.
                        MensagemPaginaCategoria.MostraMensagem_Erro("Já existe uma categoria com este nome na lista. Por favor, use um nome diferente.");
                        return; // Impede a continuação do método
                    }

                    itemParaAtualizar.sDscCategoria = ((TextBox)row.FindControl("txtGridDscCategoria")).Text;
                    itemParaAtualizar.nTeto = BD.Conversoes.Numerico_Decimal(((TextBox)row.FindControl("txtGridTeto")).Text);
                    itemParaAtualizar.nSaldo = BD.Conversoes.Numerico_Decimal(((TextBox)row.FindControl("txtGridSaldo")).Text);
                    //itemParaAtualizar.bPadrao = ((CheckBox)row.FindControl("chkPadrao")).Checked;
                }
                if (itemParaAtualizar.idTipoCentroCusto == 0 || itemParaAtualizar.idTipoCentroCusto == null)
                {
                    itemParaAtualizar.idTipoCentroCusto = Convert.ToInt32(hddidTipoCentroCusto.Value);
                }
            }
            AtualizarIdTipoCentroDeCustoNaLista();
        }

        private void AtualizarIdTipoCentroDeCustoNaLista()
        {
            // Obtém o valor do hddidTipoCentroCusto uma única vez, fora do loop, para otimizar.
            int idTipoCentroDeCusto = Convert.ToInt32(hddidTipoCentroCusto.Value);

            // Itera sobre cada item na sua lista de categorias.
            foreach (var categoria in bs_CentroDeCusto_Categorias)
            {
                // Verifica se o idTipoCentroCusto do item é 0 ou nulo.
                if (categoria.idTipoCentroCusto == 0 || categoria.idTipoCentroCusto == null)
                {
                    // Se a condição for verdadeira, atualiza o valor.
                    categoria.idTipoCentroCusto = idTipoCentroDeCusto;
                }
            }
        }

        private bool ValidarCategorias()
        {
            for (int i = 0; i < bs_CentroDeCusto_Categorias.Count; i++)
            {
                var item = bs_CentroDeCusto_Categorias[i];
                if (string.IsNullOrWhiteSpace(item.sDscCategoria))
                {
                    MensagemPaginaCategoria.MostraMensagem_Erro($"A descrição da categoria na linha {i + 1} não pode ser vazia.");
                    return false;
                }
            }
            return true;
        }

        [Serializable]
        public class cls_CentroDeCusto_Categoria
        {
            private static int _tempIdCounter = -1;
            public int TempId { get; private set; }

            public int idCategoria { get; set; }
            public string sDscCategoria { get; set; }
            public decimal? nTeto { get; set; }
            public decimal? nSaldo { get; set; }
            public DateTime? dtAtualizacao { get; set; }
            public int? idUsuario { get; set; }
            public int? idTipoCentroCusto { get; set; }
            public string sPadrao { get; set; }

            public bool bPadrao
            {
                get { return sPadrao == "S"; }
                set { sPadrao = value ? "S" : "N"; }
            }

            public cls_CentroDeCusto_Categoria()
            {
                this.TempId = _tempIdCounter--;
            }

            public object GridKey
            {
                get
                {
                    return this.idCategoria > 0 ? (object)this.idCategoria : (object)this.TempId;
                }
            }
        }
        #endregion
    }

}