using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;

namespace TT_Flow.App.Controles
{
    // Classe customizada para passar os dados no evento
    public class ItensImportadosEventArgs : EventArgs
    {
        public DataTable DadosImportados { get; set; }
        public string TipoOrigem { get; set; }
        public string IdReferencia { get; set; }
    }

    public partial class ImportadorItensModal : System.Web.UI.UserControl
    {
        // Declaração do Evento que a página pai vai escutar
        public delegate void ItensImportadosEventHandler(object sender, ItensImportadosEventArgs e);
        public event ItensImportadosEventHandler OnItensImportados;

        // Propriedade para definir a procedure dinamicamente pela página (opcional)
        public string ProcedureName { get; set; } = "sp_Manipula_tbl_Flow_Adm_EmissaoNFE";
        public string OpcoesVisiveis { get; set; } = "PEDIDO,OPI,LME,COMEX";

        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (ListItem item in rb_Resposta.Items)
            {
                item.Attributes.Add("class", "btn btn-primary");
            }
        }

        private void ConfigurarOpcoesRadio()
        {
            rb_Resposta.Items.Clear();

            if (string.IsNullOrEmpty(OpcoesVisiveis)) return;

            // Quebra as opções separadas por vírgula
            string[] opcoes = OpcoesVisiveis.Split(',');

            foreach (string opcao in opcoes)
            {
                string valor = opcao.Trim().ToUpper();
                string textoVisual = "";

                // Verifica qual opção foi solicitada e define o texto visual
                switch (valor)
                {
                    case "PEDIDO": textoVisual = "&nbsp;Pedido&nbsp;&nbsp;"; break;
                    case "OPI": textoVisual = "&nbsp;OPI&nbsp;&nbsp;"; break;
                    case "LME": textoVisual = "&nbsp;LME&nbsp;&nbsp;"; break;
                    case "COMEX": textoVisual = "&nbsp;COMEX&nbsp;&nbsp;"; break; // A nova opção!
                }

                if (!string.IsNullOrEmpty(textoVisual))
                {
                    ListItem item = new ListItem(textoVisual, valor);
                    item.Attributes.Add("class", "btn btn-primary");
                    rb_Resposta.Items.Add(item);
                }
            }
        }

        // Método público para a página pai chamar para abrir o modal
        public void AbrirModal()
        {
            ConfigurarOpcoesRadio();
            pnResultado.Visible = false;
            rb_Resposta.ClearSelection();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenImportacao", "$('#Modal_ImportarItens').modal('show');", true);
        }

        protected void cbTipoImportacao_CheckedChanged(object sender, EventArgs e)
        {
            pnResultado.Visible = false;
            gvImportacaoItens.Visible = false;
            gvImportacaoComex.Visible = false;

            string sPesquisa = rb_Resposta.SelectedValue;
            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();

            if (sPesquisa == "COMEX")
            {
                vParametros.Add("@sFuncao", "CONSULTAR");
                vParametros.Add("@sPesquisa", "");
                vParametros.Add("@sDscStatus ", "Autorizada");

                dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ImportadorNfe", vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTablesComex", TT.FrameWork.Grid.DataBindComScript_Geral(gvImportacaoComex, dsPesquisa.Tables[0], true, true, true, "false", "''", "10", true, true, true, 3, "desc", null), true);
                    gvImportacaoComex.Visible = true;
                    pnResultado.Visible = true;
                }
            }
            else
            {
                vParametros.Add("@sfuncao", "PESQUISA_ITENS");
                vParametros.Add("@sPesquisa", sPesquisa);

                dsPesquisa = BD.ExecutarDataSet(ProcedureName, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTablesItens", TT.FrameWork.Grid.DataBindComScript_Geral(gvImportacaoItens, dsPesquisa.Tables[0], true, true, true, "false", "''", "10", true, true, true, 3, "desc", null), true);
                    gvImportacaoItens.Visible = true;
                    pnResultado.Visible = true;
                }
            }

            string scriptManterAberto = "$('.modal-backdrop').remove(); $('#Modal_ImportarItens').modal('show');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ManterModalAberto", scriptManterAberto, true);
        }

        //protected void cbTipoImportacao_CheckedChanged(object sender, EventArgs e)
        //{
        //    pnResultado.Visible = false;
        //    string sPesquisa = rb_Resposta.SelectedValue;
        //    string sErro = "";
        //    DataSet dsPesquisa;

        //    Dictionary<String, String> vParametros = new Dictionary<string, string>();
        //    vParametros.Add("@sfuncao", "PESQUISA_ITENS");
        //    vParametros.Add("@sPesquisa", sPesquisa);

        //    // Assume que 'BD' é uma classe estática global sua
        //    dsPesquisa = BD.ExecutarDataSet(ProcedureName, vParametros);

        //    if (BD.ValidarDataSet(dsPesquisa, out sErro))
        //    {
        //        // Assume que 'TT.FrameWork.Grid...' é um utilitário global seu
        //        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript_Geral(gvImportacaoItens, dsPesquisa.Tables[0], true, true, true, "false", "''", "10", true, true, true, 3, "desc", null), true);
        //        pnResultado.Visible = true;
        //    }

        //    // MANTÉM O MODAL ABERTO APÓS O POSTBACK:
        //    // Remove o fundo escuro antigo (se houver) e manda abrir novamente
        //    string scriptManterAberto = "$('.modal-backdrop').remove(); $('#Modal_ImportarItens').modal('show');";
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ManterModalAberto", scriptManterAberto, true);
        //}

        protected void gvImportacaoComex_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Importacao")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string idImportador = gvImportacaoComex.DataKeys[index].Values[0].ToString();

                // Passa o ID da NFe e a string "COMEX" de volta pra página pai.
                // Usa a sua ProcedureName padrão do controle para trazer os itens dessa NFe.
                DispararEventoImportacao(idImportador, "COMEX", ProcedureName);
            }
        }

        protected void gvImportacaoItens_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Importacao")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string id = gvImportacaoItens.DataKeys[index].Values[0].ToString();
                string sTipo = gvImportacaoItens.DataKeys[index].Values[1].ToString();
                string sErro = "";
                DataSet dsPesquisa;

                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sfuncao", "CONSULTAR_ITENS");
                vParametros.Add("@sPesquisa", sTipo);
                vParametros.Add("@idPedido", id);

                dsPesquisa = BD.ExecutarDataSet(ProcedureName, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    // Se a página pai estiver escutando o evento, dispara passando o DataTable genérico
                    if (OnItensImportados != null)
                    {
                        var eventArgs = new ItensImportadosEventArgs
                        {
                            DadosImportados = dsPesquisa.Tables[0],
                            TipoOrigem = sTipo,
                            IdReferencia = id
                        };

                        OnItensImportados(this, eventArgs);
                    }
                }

                // FECHA O MODAL LIMPANDO O CACHE VISUAL DO BOOTSTRAP:
                string scriptFechar = "$('#Modal_ImportarItens').modal('hide'); $('.modal-backdrop').remove(); $('body').removeClass('modal-open');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModalItens", scriptFechar, true);
            }
        }

        protected void gvImportacaoItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Substitua Grid.EsconderColunas pela sua chamada real se aplicável
            // Grid.EsconderColunas(e, 0); 
            if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Visible = false;
            }
        }

        protected void gvImportacaoComex_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Se a sua proc do COMEX retornar a coluna sCor, mantenha a formatação igual a tela original
                try
                {
                    e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
                }
                catch { } // Ignora se a proc não tiver a coluna sCor
            }
        }

        private void DispararEventoImportacao(string id, string sTipo, string sProcedure)
        {
            string sErro = "";
            DataSet dsPesquisa;

            Dictionary<String, String> vParametros = new Dictionary<string, string>();

            // 1. SEPARA A FORMA DE CONSULTAR NO BANCO (COMEX vs OUTROS)
            if (sTipo == "COMEX")
            {
                vParametros.Add("@sFuncao", "CONSULTAR_DETALHE_ITENS");
                vParametros.Add("@idImportador", id);

                // Força a proc correta caso a procedure padrão não seja a do COMEX
                sProcedure = "sp_Manipula_tbl_Flow_ImportadorNfe";
            }
            else
            {
                vParametros.Add("@sfuncao", "CONSULTAR_ITENS");
                vParametros.Add("@sPesquisa", sTipo);
                vParametros.Add("@idPedido", id);
            }

            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                if (OnItensImportados != null)
                {
                    // 2. A MÁGICA DA TABELA 1:
                    // A proc do COMEX retorna 3 selects (Cabeçalho, Itens e NFes).
                    // A tabela de itens cai no índice 1. As demais procs retornam na 0.
                    DataTable dtItens = (sTipo == "COMEX") ? dsPesquisa.Tables[1] : dsPesquisa.Tables[0];

                    var eventArgs = new ItensImportadosEventArgs
                    {
                        DadosImportados = dtItens,
                        TipoOrigem = sTipo,
                        IdReferencia = id
                    };

                    OnItensImportados(this, eventArgs);
                }
            }

            string scriptFechar = "$('#Modal_ImportarItens').modal('hide'); $('.modal-backdrop').remove(); $('body').removeClass('modal-open');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModalItens", scriptFechar, true);
        }
    }
}