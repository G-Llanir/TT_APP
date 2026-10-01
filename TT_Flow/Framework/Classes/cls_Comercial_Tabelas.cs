using System;
using System.Data;
using System.Linq;

namespace TT_Flow.FrameWork
{
    #region | Interfaces

    public interface IComercial_TabelaPreco
    {
        int IdTabela { get; set; }
        int IdTipoTabela { get; set; }
        decimal Preco { get; set; }
        string SdscTabela { get; set; }
        string SdscTipoTabela { get; set; }
    }

    #endregion

    #region | Classes

    [Serializable]
    public class cls_Comercial_Tabelas : cls_WMS_Produtos, IComercial_TabelaPreco
    {
        public cls_Comercial_Tabelas() { }

        #region | Propriedades

        public int idRegistro { get; set; }
        public int IdTipoTabela { get; set; }
        public int IdTabela { get; set; }
        public int IdTabelaOrigem { get; set; }
        public int idRegra { get; set; }
        public int idTipoRegra { get; set; }

        public decimal nUnitario { get; set; }
        public decimal Preco { get; set; }
        public decimal Preco_Zona_SD { get; set; }
        public decimal Preco_Zona_ND { get; set; }
        public decimal Preco_Zona_N { get; set; }
        public decimal Preco_Zona_CO { get; set; }
        public decimal Preco_Zona_S { get; set; }
        public decimal NAjuste { get; set; }
        public decimal NFator { get; set; }
        public decimal NEnvio { get; set; }
        public decimal NLocal { get; set; }
        public decimal NMargem { get; set; }
        public decimal NImpostos { get; set; }
        public decimal NII { get; set; }
        public decimal NIPI { get; set; }
        public decimal NPIS { get; set; }
        public decimal NCOFINS { get; set; }
        public decimal NICMS { get; set; }
        public decimal NST { get; set; }
        public decimal NDIFAL { get; set; }
        public decimal NIRPJ { get; set; }
        public decimal NCSSL { get; set; }
        public decimal NTotal { get; set; }
        public decimal nBaseCalc_ICMS { get; set; }
        public decimal nBaseCalc_ICMS_Original { get; set; }
        public decimal nVlr_II { get; set; }
        public decimal nVlr_IPI { get; set; }
        public decimal nVlr_PIS { get; set; }
        public decimal nVlr_COFINS { get; set; }
        public decimal nVlr_ICMS { get; set; }
        public decimal nVlr_DIFAL { get; set; }
        public decimal nVlr_ST { get; set; }
        public decimal nVlr_Liquido { get; set; }
        public decimal nVlr_IRPJ { get; set; }
        public decimal nVlr_CSSL { get; set; }
        public decimal nDesconto { get; set; }
        public decimal nReducao { get; set; }
        public decimal nVlrReducao { get; set; }

        public string SdscTabela { get; set; }
        public string SdscTipoTabela { get; set; }
        public string sCEST { get; set; }
        public string SCodigoComDescricao { get; set; }
        public string dtInclusao { get; set; }
        public string sII { get; set; }
        public string sIPI { get; set; }
        public string sPIS { get; set; }
        public string sCOFINS { get; set; }
        public string sICMS { get; set; }
        public string sST { get; set; }
        public string sDIFAL { get; set; }

        public bool bImportado { get; set; }
        public bool bLiberado { get; set; }
        public bool bSistema { get; set; }
        public bool bBaseCalcICMS_com_IPI { get; set; }

        #endregion

        public static string removeCaracteres(string sObjeto)
        {
            string sObjetoSemCaracter = new string(sObjeto
                .Where(c => Char.IsDigit(c) || c == ',' || c == '.')
                .Select(c => c == ',' ? '.' : c)
                .ToArray());

            if (sObjetoSemCaracter == "")
                sObjetoSemCaracter = "0";

            return sObjetoSemCaracter;
        }
    }

    [Serializable]
    public class cls_Comercial__Controles_Fator
    {
        #region | Propriedades

        public int idObjeto { get; set; }
        public int idTipoObjeto { get; set; }
        public string sDscObjeto { get; set; }
        public decimal nValor { get; set; }
        public string sBloquearEdicao { get; set; }

        #endregion
    }

    [Serializable]
    public class cls_Comercial_Dashboard_CRM
    {
        public string sNomeGrafico { get; set; }
        public string sPeriodo { get; set; }
        public string sIndicador { get; set; }
        public int nQuantidade { get; set; }
        public int sFiltro { get; set; }
        public int idVendedor { get; set; }
        public string[] sLabels { get; set; }

    }

    #endregion
}