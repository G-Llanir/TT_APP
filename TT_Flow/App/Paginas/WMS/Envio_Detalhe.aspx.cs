using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT.FrameWork;
using System.Globalization;
using TT_Flow.FrameWork;
using System.Linq;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace TT_Flow.App.Paginas.WMS
{
    public partial class Envio_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Envio";
        string sProcedure = "sp_Manipula_FLow_WMS_OPI";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (!IsPostBack)
            {

                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    Pesquisar("0", true);
                }

            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidEnvio.Value, true);
                }
            }

            RegistraScript("");
            RegistrarColapsoScript();
        }


        #endregion

        #region | Classes
        //Volumes
        public EntidadeFuncoes<cls_WMS_VolumesItens> conversorVolumes = new EntidadeFuncoes<cls_WMS_VolumesItens>();
        public List<FrameWork.cls_WMS_VolumesItens> ls_VolumesItens
        {
            get
            {
                if (ViewState["ls_VolumesItens"] == null)
                {
                    ViewState["ls_VolumesItens"] = new List<cls_WMS_VolumesItens>();
                }
                return (List<cls_WMS_VolumesItens>)ViewState["ls_VolumesItens"];
            }
            set
            {
                ViewState["ls_Produtos"] = value;
            }
        }
        public List<FrameWork.cls_WMS_VolumesItens> ls_VolumesItensGV
        {
            get
            {
                if (ViewState["ls_VolumesItensGV"] == null)
                {
                    ViewState["ls_VolumesItensGV"] = new List<cls_WMS_VolumesItens>();
                }
                return (List<cls_WMS_VolumesItens>)ViewState["ls_VolumesItensGV"];
            }
            set
            {
                ViewState["ls_VolumesItensGV"] = value;
            }
        }

        public EntidadeFuncoes<cls_WMS_Produtos> conversorProdutos = new EntidadeFuncoes<cls_WMS_Produtos>();

        public List<cls_WMS_Produtos> ls_Produtos = new List<cls_WMS_Produtos>();
        public List<FrameWork.cls_WMS_Produtos> ls_VolumesProdutosGV
        {
            get
            {
                if (ViewState["ls_VolumesProdutosGV"] == null)
                {
                    ViewState["ls_VolumesProdutosGV"] = new List<FrameWork.cls_WMS_Produtos>();
                }
                return (List<FrameWork.cls_WMS_Produtos>)ViewState["ls_VolumesProdutosGV"];
            }
            set
            {
                ViewState["ls_VolumesProdutosGV"] = value;
            }
        }
        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idEnvio, bool bEdicao)
        {
            string sErro = "";

            try
            {
                PopularCombos();
                if (idEnvio != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR-ENVIOS");
                    vParametros.Add("@idEnvioOPI", idEnvio);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidEnvio.Value = RETORNO.DATASET(dsPesquisa, 0, "idEnvioOPI");
                        txtidEnvio.Text = RETORNO.DATASET(dsPesquisa, 0, "idEnvioOPI");
                        hddidOPI.Value = RETORNO.DATASET(dsPesquisa, 0, "idOPI");
                        //txtidOPI.Text = RETORNO.DATASET(dsPesquisa, 0, "idOPI");
                        //ddlEmbalagem.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEmbalagem");
                        txtsDscOPI.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscOPI");
                        txtsCliente.Text = RETORNO.DATASET(dsPesquisa, 0, "sCliente");
                        var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtEnvio").ToString());
                        txtDtEnvio.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');

                        txtNPesoLiquido.Text = RETORNO.DATASET(dsPesquisa, 0, "nPesoLiquido");
                        txtNPesoBruto.Text = RETORNO.DATASET(dsPesquisa, 0, "nPesoBruto");
                        txtNComprimento.Text = RETORNO.DATASET(dsPesquisa, 0, "nComprimento");
                        txtNLargura.Text = RETORNO.DATASET(dsPesquisa, 0, "nLargura");
                        txtNAltura.Text = RETORNO.DATASET(dsPesquisa, 0, "nAltura");
                        txtNQtdVolumes.Text = RETORNO.DATASET(dsPesquisa, 0, "nQtdVolumes").ToString();
                        txtNDimensoes.Text = RETORNO.DATASET(dsPesquisa, 0, "nDimensoes").ToString();
                        PopularVolumes(RETORNO.DATASET(dsPesquisa, 0, "idEnvioOPI"));
                        PainelAtualizacao.Visible = false;
                        //PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        //lblTituloPagina.Text = string.Format("Envio {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscEnvio"));
                        BreadCrumb.TitulodaPagina = "Envio";
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro(sErro);
                    }

                }

                RegistraScript("");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }
        void PopularCombos()
        {
            //FUNCOES.Popula_Combo(ddlEmbalagem, "sp_Select 'Flow_WMS_Embalagem'", "idEmbalagem", "sDScEmbalagem", false, "Selecione uma Embalagem", "0");
        }

        protected void Voltar_Click(object sender, EventArgs e)
        {
            Response.Redirect($"/App/Paginas/WMS/OPI_Detalhe.aspx?id={hddidOPI.Value}");
        }
        #endregion

        #region | Arquivos
        protected void AbrirArquivosVolumes_Click(object sender, EventArgs e)
        {
            LinkButton cmdArquivo = (LinkButton)sender;
            string idVolume = cmdArquivo.CommandArgument;

            lbltituloModal.Text = "Fotos do Volume";
            Popular_aba_ArquivoVolume(idVolume);
            AbrirModal_Click(sender, e);
            RegistrarColapsoScript();
        }
        void Popular_aba_ArquivoVolume(string idVolume)
        {
            frmArquivos2.Attributes.Add("src", $"~/app/Paginas/Arquivos.aspx?idObjeto={idVolume}&sTipoObjeto={"Volume"}");
        }
        #endregion

        #region | Exportar Excel
        protected void GerarExcelEnvio_Click(object sender, EventArgs e)
        {
            EXC_ENVIO(hddidEnvio.Value);
        }
        void EXC_ENVIO(string idEnvio)
        {
            try
            {
                Microsoft.Reporting.WebForms.ReportViewer rv4 = new Microsoft.Reporting.WebForms.ReportViewer();

                rv4.ProcessingMode = ProcessingMode.Local;
                rv4.LocalReport.EnableExternalImages = true;

                rv4.LocalReport.ReportPath = Server.MapPath("~/App/Reports/EnviosVolumes.rdlc");
                DataSet dsEnvios = new DataSet();

                Dictionary<string, string> vParametrosEnvios = new Dictionary<string, string>();
                vParametrosEnvios.Add("@sFuncao", "CONSULTAR-ENVIOS");
                vParametrosEnvios.Add("@idEnvioOPI", idEnvio);
                dsEnvios = BD.ExecutarDataSet("sp_Manipula_FLow_WMS_OPI", vParametrosEnvios);

                if (dsEnvios.Tables.Count == 0 || dsEnvios.Tables[0].Rows.Count == 0)
                {
                    MensagemPaginaAbaEnvios.MostraMensagem_Aviso("Não foi encontrado dados nesse envio");
                    return;
                }

                DataSet dsVolumes = new DataSet();

                Dictionary<string, string> vParametrosVolumes = new Dictionary<string, string>();
                vParametrosVolumes.Add("@sFuncao", "CONSULTAR-VOLUMES-EXCEL");
                vParametrosVolumes.Add("@idEnvioOPI", idEnvio);
                dsVolumes = BD.ExecutarDataSet("sp_Manipula_FLow_WMS_OPI", vParametrosVolumes);

                if (dsVolumes.Tables.Count == 0 || dsVolumes.Tables[0].Rows.Count == 0)
                {
                    MensagemPaginaAbaEnvios.MostraMensagem_Aviso("Não foi encontrado dados nesse envio");
                    return;
                }

                DataSet dsFotosVolumes = new DataSet();

                Dictionary<string, string> vParametrosFotos = new Dictionary<string, string>();
                vParametrosFotos.Add("@sFuncao", "CONSULTAR-IMAGEM-VOLUME");
                vParametrosFotos.Add("@idEnvioOPI", idEnvio);
                dsFotosVolumes = BD.ExecutarDataSet("sp_Manipula_FLow_WMS_OPI", vParametrosFotos);

                rv4.LocalReport.DataSources.Clear();
                rv4.LocalReport.DataSources.Add(new ReportDataSource("dsEnvio", dsEnvios.Tables[0]));
                rv4.LocalReport.DataSources.Add(new ReportDataSource("dsVolumesUnitizados", dsVolumes.Tables[0]));
                rv4.LocalReport.DataSources.Add(new ReportDataSource("dsFotosVolumes", dsFotosVolumes.Tables[0]));

                Microsoft.Reporting.WebForms.Warning[] warnings;
                string[] streamIds;
                string mimeType, encoding, extension;

                byte[] bytes = rv4.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                string dataSemBarras = dsEnvios.Tables[0].Rows[0]["dtEnvio"].ToString().Replace("/", "");

                string sNomeArquivoOriginal = "Envio_E" + dsEnvios.Tables[0].Rows[0]["idEnvioOPI"].ToString() + "DTENV" + dataSemBarras + "_" + FUNCOES.CarimboDataHora() + ".xls";
                string sNomeArquivo = LimparNomeArquivo(sNomeArquivoOriginal);
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);

                FUNCOES.DownloadArquivo(Page, sNomeArquivo);
                MensagemPaginaAbaEnvios.MostraMensagem_Sucesso("Excel do Envio gerado com sucesso!");
                Pesquisar(hddidEnvio.Value, false);
            }
            catch (Exception ex)
            {
                MensagemPaginaAbaEnvios.MostraMensagem_Erro("Erro ao gerar o Excel do Envio! </br>" + ex.Message);
            }
        }
        public string LimparNomeArquivo(string nomeArquivo)
        {
            // Defina os caracteres inválidos para nomes de arquivos e diretórios
            char[] caracteresInvalidos = System.IO.Path.GetInvalidFileNameChars();
            char[] caracteresInvalidosDiretorio = System.IO.Path.GetInvalidPathChars();

            // Converta o nome do arquivo em uma lista de caracteres
            List<char> caracteresValidos = new List<char>();

            // Itere sobre cada caractere no nome do arquivo
            foreach (char c in nomeArquivo)
            {
                // Se o caractere não estiver na lista de caracteres inválidos, adicione à lista de caracteres válidos
                if (!caracteresInvalidos.Contains(c) && !caracteresInvalidosDiretorio.Contains(c))
                {
                    caracteresValidos.Add(c);
                }
                else
                {
                    // Substitua caracteres inválidos por um underline (ou qualquer caractere válido que você prefira)
                    caracteresValidos.Add('_');
                }
            }

            // Converta a lista de caracteres válidos de volta para uma string
            return new string(caracteresValidos.ToArray());
        }
        #endregion

        #region | Função de Imagem
        protected void CarregaImgProduto(string idProduto, System.Web.UI.WebControls.Image img)
        {
            DataTable dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_IMAGEM");
            vParametros.Add("@idTipoArquivo", "201");
            vParametros.Add("@idObjeto", idProduto);
            dsPesquisa = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dsPesquisa.Rows.Count > 0)
            {
                DataRow imgBd = dsPesquisa.Rows[0];
                string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);

                img.ImageUrl = imgUrl;
                img.Visible = true;
            }
            else
            {
                img.ImageUrl = "/App/img/wms_dimensoes.svg";
                img.Visible = true;
            }
        }
        #endregion

        #region | Volumes 
        #region | DataGrid VolumeGeral
        void PopularVolumes(string id)
        {
            DataSet dsVolumes = new DataSet();
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-VOLUMES-ENVIOS");
            vParametros.Add("@idEnvio", id);
            dsVolumes = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsVolumes))
            {
                dtgVolumesDataBind(dsVolumes);
                //ls_unitizadosItensGV.Clear();
            }
            else
            {
                Div_Envios.Visible = false;
            }
        }
        void dtgVolumesDataBind(DataSet dsVolumes)
        {
            dtgVolumes.DataSource = dsVolumes;
            dtgVolumes.DataBind();
            RegistrarColapsoScript();
        }
        protected void dtgVolume_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string id = dtgVolumes.DataKeys[e.Row.RowIndex].Value.ToString();
                var dtgVolumesItensGV = (GridView)e.Row.FindControl("dtgVolumesItensGV");
                ConsultaVolumesItens(id);
                dtgVolumesItensGV.DataSource = ls_VolumesItensGV;
                dtgVolumesItensGV.DataBind();
            }
        }

        //protected void dtgVolumeItem_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        GridView dtgVolumesItensGV = (GridView)sender;

        //        if (dtgVolumesItensGV != null)
        //        {
        //            string idObjeto = dtgVolumesItensGV.DataKeys[e.Row.RowIndex].Value.ToString();

        //            var dtgItensVL = (GridView)e.Row.FindControl("dtgItensVL");
        //            ConsultaItensVL(idObjeto);
        //            dtgItensVL.DataSource = ls_VolumesProdutosGV;
        //            dtgItensVL.DataBind();
        //        }
        //    }
        //}
        protected void dtgVolumeItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView dtgVolumesItensGV = (GridView)sender;
                LinkButton btnToggle = (LinkButton)e.Row.FindControl("btnToggle");
                GridView dtgItensVL = (GridView)e.Row.FindControl("dtgItensVL");

                if (dtgVolumesItensGV != null)
                {
                    string idObjeto = dtgVolumesItensGV.DataKeys[e.Row.RowIndex].Value.ToString();

                    // Pega o código de barras pra saber a natureza do item (Produto, Unitizado ou Volume)
                    string codigoBarras = DataBinder.Eval(e.Row.DataItem, "SCodigoBarras").ToString().ToUpper();

                    // 1. SE FOR PRODUTO (Não tem nada dentro dele)
                    if (codigoBarras.StartsWith("PR"))
                    {
                        if (btnToggle != null) btnToggle.Visible = false; // Esconde o botão "+"

                        if (dtgItensVL != null)
                        {
                            dtgItensVL.DataSource = null; // Esvazia a sub-grid pra não pesar a tela
                            dtgItensVL.DataBind();
                        }
                    }
                    // 2. SE FOR UNITIZADO (Tem produtos dentro)
                    else if (codigoBarras.StartsWith("UN"))
                    {
                        if (btnToggle != null) btnToggle.Visible = true;

                        ConsultaItensVL(idObjeto, "UN");

                        if (dtgItensVL != null)
                        {
                            dtgItensVL.DataSource = ls_VolumesProdutosGV;
                            dtgItensVL.DataBind();
                        }
                    }
                    // 3. SE FOR VOLUME (Tem itens dentro)
                    else if (codigoBarras.StartsWith("VOL"))
                    {
                        if (btnToggle != null) btnToggle.Visible = true;

                        ConsultaItensVL(idObjeto, "VOL");

                        if (dtgItensVL != null)
                        {
                            dtgItensVL.DataSource = ls_VolumesProdutosGV;
                            dtgItensVL.DataBind();
                        }
                    }
                }
            }
        }

        void ConsultaVolumesItens(string id)
        {
            string sFuncao = "CONSULTAR-VOLUMES-ITENS";
            DataSet dsVolumesItens;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idVolume", id);

            dsVolumesItens = BD.ExecutarDataSet(sProcedure, vParametros, false);

            ls_VolumesItensGV = conversorVolumes.ConverterDataSet(dsVolumesItens, "Table");
        }

        void ConsultaItensVL(string id, string tipo)
        {
            if (tipo == "UN")
            {
                string sFuncao = "CONSULTAR-UNITIZADOS-ITENS-OPI";
                DataSet dsProdutos;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", sFuncao);
                vParametros.Add("@idUnitizado", id);

                dsProdutos = BD.ExecutarDataSet(sProcedure, vParametros, false);
                ls_VolumesProdutosGV = conversorProdutos.ConverterDataSet(dsProdutos, "Table");
            }
            else if (tipo == "VOL")
            {
                string sFuncao = "CONSULTAR-VOLUMES-ITENS";
                DataSet dsVolItens;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", sFuncao);
                vParametros.Add("@idVolume", id);

                dsVolItens = BD.ExecutarDataSet(sProcedure, vParametros, false);

                // Como a grid de nível 3 (dtgItensVL) exige uma lista de Produtos, 
                // nós mapeamos os itens do Volume para a estrutura correta.
                List<FrameWork.cls_WMS_Produtos> lstMapeada = new List<FrameWork.cls_WMS_Produtos>();

                if (BD.ValidarDataSet(dsVolItens))
                {
                    foreach (DataRow row in dsVolItens.Tables[0].Rows)
                    {
                        lstMapeada.Add(new FrameWork.cls_WMS_Produtos()
                        {
                            IdItem = Convert.ToInt32(row["idObjeto"]),
                            SCodigo = row["sCodigo"].ToString(),
                            SDscProduto = row["sDscProduto"].ToString(),
                            NQuantidade = Convert.ToDecimal(row["nQuantidade"]),
                            SCodigoBarras = row["sCodigoBarras"].ToString(),
                            sControlaGarantiaLote = ""
                        });
                    }
                }

                ls_VolumesProdutosGV = lstMapeada;
            }
        }

        //void ConsultaItensVL(string id)
        //{
        //    string sFuncao = "CONSULTAR-UNITIZADOS-ITENS-OPI";
        //    DataSet dsProdutos;
        //    Dictionary<String, String> vParametros = new Dictionary<string, string>();
        //    vParametros.Add("@sFuncao", sFuncao);
        //    vParametros.Add("@idUnitizado", id);

        //    dsProdutos = BD.ExecutarDataSet(sProcedure, vParametros, false);

        //    ls_VolumesProdutosGV = conversorProdutos.ConverterDataSet(dsProdutos, "Table");
        //}
        #endregion

        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$('[id*=txtNPesoLiquido]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtNPesoBruto]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtNComprimento]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtNLargura]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtNAltura]').mask('000.000.000.000.000,00', { reverse: true });");

            //sb.Append("});");


            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }
        void RegistrarColapsoScript()
        {
    //        string script = @"
    //<script type='text/javascript'>
    //    $(document).ready(function() {
    //        // Define o ícone inicial
    //        $('.toggle-icon').addClass('fa fa-plus');
            
    //        // Função para alternar ícones e mostrar/ocultar div
    //        $('.toggle-icon').click(function() {
    //            var icon = $(this);
    //            var divId = $(this).data('div-id');
    //            var current = $('#' + divId).css('display');
    //            if (current == 'none') {
    //                $('#' + divId).show('slow');
    //                icon.removeClass('fa fa-plus').addClass('fa fa-minus');
    //            } else {
    //                $('#' + divId).hide('slow');
    //                icon.removeClass('fa fa-minus').addClass('fa fa-plus');
    //            }
    //            return false; // Evita o postback
    //        });
    //    });
    //</script>";

    //        Page.ClientScript.RegisterStartupScript(this.GetType(), "ExibirOcultarScript", script);
        }

        #endregion

        #region | Método de Gerar Linha na Grid
        public String NovaLinha(object id, string gridNome)
        {
            /* 
            * 1. Fecha a célula atual
            * 2. Fecha a linha Atual
            * 3. Cria uma nova linha com o ID e a classe <TR id='...' style='...'>
            * 4. Cria uma célula em branco: <TD></TD>
            * 5. Cria uma nova célula para conter o gridview dtgUnitizadosItens
            ************************************************************/
            if (id != null && !string.IsNullOrEmpty(id.ToString()))
            {
                // Se houver um ID, retorna a nova linha com o ID e a classe
                return string.Format(@"</td></tr><tr id='tr{0}{1}' class='collapsed-row'>
                               <td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            }
            else
            {
                // Se não houver ID, retorna uma string vazia para que nada seja renderizado
                // e o botão de colapso desapareça
                return string.Empty;
            }
        }
        #endregion

        #region | Controle de Modal
        protected void AbrirModal_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalEnvio').modal('show');", true);
        }
        #endregion
    }
}