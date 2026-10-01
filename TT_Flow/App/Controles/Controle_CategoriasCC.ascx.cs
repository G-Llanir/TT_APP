using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Hub.App.Paginas.Adm.Manutencao;
using static TT.FrameWork.BD;

namespace TT_Flow.App.Controles
{
    public partial class Controle_CategoriasCC : UserControl
    {
        public TextBox txtBox { get; set; }
        public string SAcao { get; set; }
        public string sPlaceholder { get; set; } = "Selecione Uma Categoria";
        public DropDownList CentroDeCusto { get => ddlidCentroDeCusto; set => ddlidCentroDeCusto = value; }
        public DropDownList CategoriaCC { get => ddlCategoriaCC; set => ddlCategoriaCC = value; }
        public HiddenField HddidRegistroAntigo { get => hddidRegistroAntigo; set => hddidRegistroAntigo = value; }
        public string IdRegistroAntigo { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Funcoes.Popula_Combo(ddlidCentroDeCusto, "sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione um Centro de Custo", "0");
                if (Acoes.SOMAR.ToString().Equals(SAcao, StringComparison.OrdinalIgnoreCase) || Acoes.SUBTRAIR.ToString().Equals(SAcao, StringComparison.OrdinalIgnoreCase))
                {
                    AcaoEncontrada.Visible = true;
                    AcaoNaoEncontrada.Visible = false;
                }
                else
                {
                    AcaoEncontrada.Visible = false;
                    AcaoNaoEncontrada.Visible = true;
                }
            }
            else
            {
                if (Acoes.SOMAR.ToString().Equals(SAcao, StringComparison.OrdinalIgnoreCase) || Acoes.SUBTRAIR.ToString().Equals(SAcao, StringComparison.OrdinalIgnoreCase))
                {
                    AcaoEncontrada.Visible = true;
                    AcaoNaoEncontrada.Visible = false;
                }
                else
                {
                    AcaoEncontrada.Visible = false;
                    AcaoNaoEncontrada.Visible = true;
                }
            }
        }
        protected void ddlidCentroDeCusto_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarCategorias(ddlidCentroDeCusto.SelectedValue);
        }
        public void CarregarCategorias(string idCentroCusto)
        {
            Funcoes.Popula_Combo(ddlCategoriaCC, $"sp_Manipula_tbl_Flow_Adm_CentroCusto_Categorias 'FLOW-CATEGORIAS', @idCentroCusto={idCentroCusto}", "idRegistro", "sDscCategoria", false, $"{sPlaceholder}", "0");

            if (ddlCategoriaCC.Items.Count > 1)
            {
                DivCategoria.Visible = true;
            }
            else
            {
                DivCategoria.Visible = false;
            }
        }

        public bool AtualizarSaldoCategoria(TextBox txtBox)
        {
            string procedure = "sp_Manipula_tbl_Flow_Adm_CentroCusto_Categorias";
            DataSet ds = new DataSet();
            Dictionary<String, String> vParametros = new Dictionary<string, string>()
                {
                    { "@sFuncao", "ATUALIZAR-VALOR" },
                     { "@nValor",BD.Conversoes.Numerico(txtBox) },
                    { "@sAcao", SAcao.ToUpper() },
                    { "@idRegistro", ddlCategoriaCC.SelectedValue}
                };

            ds = BD.ExecutarDataSet(procedure, vParametros);

            if (hddidRegistroAntigo.Value != ddlCategoriaCC.SelectedValue)
            {
                if (hddidRegistroAntigo.Value == "0" || string.IsNullOrEmpty(hddidRegistroAntigo.Value))
                    return true;

                DataSet dsRestaurar = new DataSet();
                Dictionary<String, String> vParametros2 = new Dictionary<string, string>()
                {
                    { "@sFuncao", "ATUALIZAR-VALOR" },
                    { "@nValor", BD.Conversoes.Numerico(txtBox) },
                    { "@sAcao", SAcao.ToUpper() == Acoes.SUBTRAIR.ToString() ? Acoes.SOMAR.ToString() : Acoes.SUBTRAIR.ToString()},
                    { "@idRegistro", hddidRegistroAntigo.Value}
                };

                dsRestaurar = BD.ExecutarDataSet(procedure, vParametros2);

                if (BD.ValidarDataSet(dsRestaurar))
                {
                    hddidRegistroAntigo.Value = ddlCategoriaCC.SelectedValue;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (BD.ValidarDataSet(ds))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool AtualizarSaldoCategoria(AtualizacaoSaldoInfo info)
        {
    //        string procedure = "sp_Manipula_tbl_Flow_Adm_CentroCusto_Categorias";

    //        // --- 1. Executa a ação na categoria ATUAL ---
    //        Dictionary<String, String> vParametros = new Dictionary<string, string>()
    //{
    //    { "@sFuncao", "ATUALIZAR-VALOR" },
    //    { "@idCentroCusto", info.IdCentroCusto.ToString() }, // <-- ADICIONADO
    //    { "@nValor", info.Valor.ToString().Replace(",",".") },
    //    { "@sAcao", info.Acao.ToUpper() },
    //    { "@idRegistro", info.IdCategoria.ToString() }
    //};

    //        DataSet ds = BD.ExecutarDataSet(procedure, vParametros);
    //        if (!BD.ValidarDataSet(ds))
    //        {
    //            return false;
    //        }

    //        // --- 2. Se a categoria mudou, reverte a ação na categoria ANTIGA ---
    //        if (info.IdCategoriaAntiga != 0 && info.IdCategoriaAntiga != info.IdCategoria)
    //        {
    //            string acaoReversa = info.Acao.ToUpper() == "SOMAR" ? "SUBTRAIR" : "SOMAR";

    //            Dictionary<String, String> vParametros2 = new Dictionary<string, string>()
    //    {
    //        { "@sFuncao", "ATUALIZAR-VALOR" },
    //        // Assumindo que a categoria antiga pertence ao mesmo Centro de Custo
    //        { "@idCentroCusto", info.IdCentroCusto.ToString() }, // <-- ADICIONADO
    //        { "@nValor", info.Valor.ToString().Replace(",",".") },
    //        { "@sAcao", acaoReversa },
    //        { "@idRegistro", info.IdCategoriaAntiga.ToString() }
    //    };

    //            DataSet dsRestaurar = BD.ExecutarDataSet(procedure, vParametros2);
    //            return BD.ValidarDataSet(dsRestaurar);
    //        }

            return true;
        }

        //public bool AtualizarSaldoCategoria(TextBox txt, string sAcao)
        //{
        //    DataSet ds = new DataSet();
        //    Dictionary<String, String> vParametros = new Dictionary<string, string>()
        //        {
        //            { "@sFuncao", "ATUALIZAR-VALOR" },
        //            { "@nValor", txtBox.Text },
        //            { "@sAcao", sAcao.ToUpper() }
        //        };

        //    ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_CentroCusto_Categorias", vParametros);

        //    if (BD.ValidarDataSet(ds))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}

        public void AlteraTamanhoCampos(int nTamanhoCentroCusto, int nTamanhoCategoria)
        {
            Div_CentroCusto.Attributes["class"] = "col-lg-" + nTamanhoCentroCusto.ToString();
            // CORREÇÃO AQUI: Usar nTamanhoCategoria para a DivCategoria
            DivCategoria.Attributes["class"] = "col-lg-" + nTamanhoCategoria.ToString();
        }
        public void DefinirValoresSelecionados(string idCentroCusto, string idCategoria)
        {
            //Funcoes.Popula_Combo(ddlidCentroDeCusto, "sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione um Centro de Custo", "0");
            if (CentroDeCusto.Items.FindByValue(idCentroCusto) != null)
            {
                CentroDeCusto.SelectedValue = idCentroCusto;
            }

            CarregarCategorias(idCentroCusto);

            if (CategoriaCC.Items.FindByValue(idCategoria) != null)
            {
                CategoriaCC.SelectedValue = idCategoria;
                hddidRegistroAntigo.Value = idCategoria;
                if (!string.IsNullOrEmpty(idCategoria) && idCategoria != "0")
                {
                    DivCategoria.Visible = true;
                }
                else
                {
                    DivCategoria.Visible = false;
                }
            }
            else
            {
                DivCategoria.Visible = false;
            }
        }

        public void DefinirCategoriaNova(string idCentroCusto, string idCategoriaNova)
        {
            CarregarCategorias(idCentroCusto);

            if (CategoriaCC.Items.FindByValue(idCategoriaNova) != null)
            {
                ddlCategoriaCC.SelectedValue = idCategoriaNova;
            }
            else
            {
                ddlCategoriaCC.SelectedValue = "0";
            }

        }

        public void EsconderDivCategoria()
        {
            DivCategoria.Visible = false;
        }

    }
    public class AtualizacaoSaldoInfo
    {
        public int IdCentroCusto { get; set; } 
        public decimal Valor { get; set; }
        public string Acao { get; set; }
        public int IdCategoria { get; set; }
        public int IdCategoriaAntiga { get; set; }
    }

    public enum Acoes
    {
        SOMAR,
        SUBTRAIR
    }
}