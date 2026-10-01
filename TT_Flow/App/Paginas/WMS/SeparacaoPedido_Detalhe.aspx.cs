using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.Web.Services;
using System.Data;
using static Permissao;
using TT_Flow.FrameWork;
using TT_Flow.App.Controles;
using Microsoft.Reporting.WebForms;
using System.Globalization;
using iTextSharp.tool.xml.html;
using iTextSharp.text;

namespace TT_Flow.App.Paginas.WMS
{
    public partial class SeparacaoPedido_Detalhe : System.Web.UI.Page
    {
        #region | Classes
        string sProcedure = "sp_Manipula_FLow_WMS_OPI";
        string sTituloPagina = "Separação Pedido: ";
        public static int idUnitizado = 0;
        public static string INcluirItem = "";
        public List<FrameWork.cls_WMS_Produtos> ls_unitizadosItensModal
        {
            get
            {
                if (ViewState["ls_unitizadosItensModal"] == null)
                {
                    ViewState["ls_unitizadosItensModal"] = new List<FrameWork.cls_WMS_Produtos>();
                }
                return (List<FrameWork.cls_WMS_Produtos>)ViewState["ls_unitizadosItensModal"];
            }
            set
            {
                ViewState["ls_unitizadosItensModal"] = value;
            }
        }

        public List<FrameWork.cls_WMS_Produtos> ls_Itens
        {
            get
            {
                if (ViewState["ls_Itens"] == null)
                {
                    ViewState["ls_Itens"] = new List<FrameWork.cls_WMS_Produtos>();
                }
                return (List<FrameWork.cls_WMS_Produtos>)ViewState["ls_Itens"];
            }
            set
            {
                ViewState["ls_Itens"] = value;
            }
        }

        public List<FrameWork.cls_WMS_Produtos> ls_UnitizadosItens
        {
            get
            {
                if (ViewState["ls_UnitizadosItens"] == null)
                {
                    ViewState["ls_UnitizadosItens"] = new List<FrameWork.cls_WMS_Produtos>();
                }
                return (List<FrameWork.cls_WMS_Produtos>)ViewState["ls_UnitizadosItens"];
            }
            set
            {
                ViewState["ls_UnitizadosItens"] = value;
            }
        }

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

        public EntidadeFuncoes<cls_WMS_VolumesItens> conversorVolumes = new EntidadeFuncoes<cls_WMS_VolumesItens>();

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

        public EntidadeFuncoes<cls_WMS_Produtos> conversorProdutos = new EntidadeFuncoes<cls_WMS_Produtos>();
        #endregion

        #region | PageLoad
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] != "0")
                {
                    Pesquisar(Request["id"].ToString());
                    hddidOPI.Value = Request["id"].ToString();
                    if (Session["MensagemSucesso"] != null)
                    {
                        MensagemPagina.MostraMensagem_Sucesso(Session["MensagemSucesso"].ToString());
                        Session.Remove("MensagemSucesso");
                    }
                    if (Request["msg"] == "S")
                    {
                        if (Request["Uni"] != null)
                            MensagemPagina.MostraMensagem_Sucesso("Os itens foram separados com sucesso!");
                        else
                            MensagemPagina.MostraMensagem_Sucesso("Os itens foram salvo com sucesso!!");
                        bool Finalizar = false;
                        foreach (GridViewRow item in gv_Arquivo.Rows)
                        {
                            if ((item.FindControl("FaltaSeparar") as Label).Text == "0,00")
                            {
                                Finalizar = true;
                            }
                            else
                            {
                                Finalizar = false;
                                break;
                            }
                        }
                        if (Finalizar == true)
                        {
                            DataSet dsProduto = new DataSet();
                            Dictionary<string, string> vParametros = new Dictionary<string, string>();
                            vParametros.Add("@sFuncao", "CONCLUIR_SEPARACAO");
                            vParametros.Add("@sSeparacao", "S");
                            vParametros.Add("@idOPI", hddidOPI.Value);
                            dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);
                        }
                        else
                        {
                            DataSet dsProduto = new DataSet();
                            Dictionary<string, string> vParametros = new Dictionary<string, string>();
                            vParametros.Add("@sFuncao", "CONCLUIR_SEPARACAO");
                            vParametros.Add("@sSeparacao", "E");
                            vParametros.Add("@idOPI", hddidOPI.Value);
                            dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);
                        }

                        Pesquisar(hddidOPI.Value);
                    }
                }
            }

            ScriptLeitor();
            RegistraScript();
            RegistraScriptCamposPeso();
            LeitorQuagga1.DesligarCam();
        }
        #endregion

        #region | Pesquisar
        protected void Pesquisar(string idOPI)
        {
            string sErro = "";
            try
            {
                DivBipadorVolume.Visible = false;
                DIV_UnitizadosItens.Visible = false;
                DIV_gv_UnitizadosItens.Visible = false;
                Div_FormEnvios.Visible = false;
                DIV8.Visible = false;
                div_botoesVolumes.Visible = false;
                DIV_UNITIZADOS_Modal.Visible = false;
                DIV_ENVIOS.Visible = false;
                DIV_Separacao.Visible = false;
                DIV17.Visible = false;
                ViewArquivos2.Visible = false;
                DIV13.Visible = false;
                PopularCombos();
                DataSet dsPesquisa;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR-ITENS-SEPARACAO");
                vParametros.Add("@idOPI", idOPI);
                dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    txtnNumeroPedido.Text = RETORNO.DATASET(dsPesquisa, 0, "sPedidoCliente");
                    txtnControleTT.Text = RETORNO.DATASET(dsPesquisa, 0, "nControleTT");
                    txtsReferencia.Text = RETORNO.DATASET(dsPesquisa, 0, "sReferencia");
                    txtsRazaoSocial.Text = RETORNO.DATASET(dsPesquisa, 0, "sRazaoSocial");
                    txtsEmpresa.Text = RETORNO.DATASET(dsPesquisa, 0, "sEmpresa");
                    txtdtPedido.Text = RETORNO.DATASET(dsPesquisa, 0, "dtPedido");
                    txtsEnderecoEntrega.Text = RETORNO.DATASET(dsPesquisa, 0, "sEnderecoEntrega");
                    txtdtEstimativaEntrega.Text = RETORNO.DATASET(dsPesquisa, 0, "dtEstimativaEntrega");
                    txtsDscTipoEnvio.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscTipoEnvio");
                    hddidPedido.Value = RETORNO.DATASET(dsPesquisa, 0, "idPedido");
                    lblTituloPagina.Text = sTituloPagina + RETORNO.DATASET(dsPesquisa, 0, "nPedido");
                    hddnPedido.Value = RETORNO.DATASET(dsPesquisa, 0, "nPedido");
                    string sDscTipoStatus = RETORNO.DATASET(dsPesquisa, 0, "sSeparacao");
                    lblsDscTipoStatus.Text = sDscTipoStatus;
                    lblsDscTipoStatus.CssClass = string.Format("label label-{0}", RETORNO.DATASET(dsPesquisa, 0, "sCorSeparacao"));
                    hddidOPI.Value = idOPI;
                    hddidStatus.Value = RETORNO.DATASET(dsPesquisa, 0, "idStatus");
                    hddsDscOPI.Value = RETORNO.DATASET(dsPesquisa, 0, "sDscOPI");
                    if (dsPesquisa.Tables[2].Rows.Count > 0)
                    {
                        Popular_Itens(dsPesquisa);
                        gv_Unitizados.DataSource = dsPesquisa.Tables[2];
                        gv_Unitizados.DataBind();
                    }
                    else
                    {
                        aba_Unitizados.Visible = false;
                    }
                    if (lblsDscTipoStatus.Text == "Separado")
                    {
                        bool envios = false;
                        string sFuncao = "CONSULTAR_ENVIOS_SEPARACAO";
                        DataSet tb;
                        string sSql = "sp_Manipula_FLow_WMS_OPI";
                        Dictionary<String, String> vParametrosenvio = new Dictionary<string, string>();
                        vParametrosenvio.Add("@sFuncao", sFuncao);
                        vParametrosenvio.Add("@idOPI", idOPI);
                        tb = BD.ExecutarDataSet(sSql, vParametrosenvio, false);

                        if (tb.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in tb.Tables[0].Rows)
                            {
                                if (row["nEnviar"].ToString() != "0,00")
                                {
                                    envios = true;
                                    break;
                                }
                            }
                        }
                        if (envios != true)
                        {
                            div23.Visible = false;
                            DIV10.Visible = false;
                            div20.Visible = false;
                            div22.Visible = false;
                            btnSalvar.Visible = false;
                            DIV17.Visible = true;
                            gv_EnviosVi.DataSource = dsPesquisa.Tables[4];
                            gv_EnviosVi.DataBind();
                        }
                    }
                    else if (lblsDscTipoStatus.Text == "Em Andamento")
                    {
                        //DIV_Envios.Visible = true;
                    }
                    gv_Arquivo.DataSource = dsPesquisa;
                    gv_Arquivo.DataBind();
                    if (dsPesquisa.Tables[1].Rows.Count > 0)
                    {
                        aba_Historico.Visible = true;
                        gvHistorico.DataSource = dsPesquisa.Tables[1];
                        gvHistorico.DataBind();
                    }
                    else
                        aba_Historico.Visible = false;
                    if (dsPesquisa.Tables[4].Rows.Count > 0)
                    {
                        dtgvEnvios.DataSource = dsPesquisa.Tables[4];
                        dtgvEnvios.DataBind();
                    }
                    else
                        divTableEnvios.Visible = false;

                    if (dsPesquisa.Tables[5].Rows.Count > 0)
                    {
                        gv_Separados.DataSource = dsPesquisa.Tables[5];
                        gv_Separados.DataBind();
                    }
                    else
                        DIV18.Visible = false;

                    PopularVolumes();
                    PopularItens(idOPI);

                    DIV_UNITIZADOS_Modal.Visible = true;
                    DIV_Separacao.Visible = true;
                    DivBipadorVolume.Visible = false;
                    if (DIV_UnitizadosItens.Visible == true)
                    {
                        DIV_UnitizadosItens.Visible = false;
                        DIV_IdEmbalagem.Visible = true;
                        DIV_sDscUnitizado.Visible = true;
                        DIV_ProximoUnitizado.Visible = true;
                    }
                    //DIV_CodigoEAN.Attributes["class"] += " visible";
                    DIV_separacaoitens.Visible = false;
                    DIV_ENVIOS.Visible = true;
                    if (divCodigoB.Visible == true)
                    {
                        DivBipadorVolume.Visible = true;
                        LeitorQuagga1.AbrirCamera();
                    }
                    else
                        DivBipadorVolume.Visible = false;

                    if (DIV8.Visible == true)
                    {
                        DIV8.Visible = false;
                        DIV10.Visible = true;
                        DivBipadorVolume.Visible = false;
                    }
                    DIV10.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (ls_Itens.Count > 0)
            {
                bool Inserir = false;

                foreach (var list in ls_Itens)
                {
                    if (list.sExibePedido == "S")
                    {
                        string InserirQauntidade = list.NQuantidade.ToString();
                        foreach (GridViewRow item in gv_Arquivo.Rows)
                        {
                            decimal Quantidade = decimal.Parse(item.Cells[5].Text) - decimal.Parse(item.Cells[6].Text);
                            if (Quantidade >= decimal.Parse(InserirQauntidade) && list.SCodigo == item.Cells[1].Text)
                            {
                                DataSet dsProduto = new DataSet();
                                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                                vParametros.Add("@sFuncao", "INCLUIR_SEPARACAO");
                                vParametros.Add("@idPedido", hddidPedido.Value);
                                vParametros.Add("@idProduto", list.IdItem.ToString());
                                vParametros.Add("@nQuantidadeSeparacao", InserirQauntidade);
                                vParametros.Add("@idOPI", hddidOPI.Value);
                                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                                vParametros.Add("@sCodigoBarras", list.SCodigoBarras);
                                dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);

                                Inserir = true;

                                DataSet dsProduto_SEPARACAO = new DataSet();
                                Dictionary<string, string> vParametros_SEPARACAO = new Dictionary<string, string>();
                                vParametros_SEPARACAO.Add("@sFuncao", "SEPARACAO_ITEM");
                                vParametros_SEPARACAO.Add("@sSeparacao", "S");
                                vParametros_SEPARACAO.Add("@sCodigoEAN", list.SCodigoBarras);
                                vParametros_SEPARACAO.Add("@idOPI", hddidOPI.Value);
                                dsProduto_SEPARACAO = BD.ExecutarDataSet(sProcedure, vParametros_SEPARACAO);
                            }
                        }
                    }
                    Pesquisar(hddidOPI.Value);
                }
                if (Inserir == false)
                    MensagemPagina.MostraMensagem_Erro("A quantidade informada é superior à quantidade no pedido!");
                else
                {
                    FUNCOES.DirecionaPagina("App/Paginas/WMS/SeparacaoPedido_Detalhe.aspx?id=" + hddidOPI.Value + "&msg=S");
                }
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Adicione o código Barras do item!");
                LeitorQuagga1.RegisterQuaggaLibrary();
                LeitorQuagga1.RegistraQuaggaScript();
                LeitorQuagga1.AbrirCamera();
            }
        }
        #endregion

        #region | Eventos
        protected void gv_Arquivo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
            GRID.EsconderColunas(e, 0);
        }

        protected void AbrirCamera_Click(object sender, EventArgs e)
        {
            DivBipadorVolume.Visible = true;
            DIV_CodigoEAN.Visible = true;
            DivManual.Visible = true;
            txtsCodigoEAN.Visible = true;
            cmdIncluirUnitizadoItem.Visible = true;
            LeitorQuagga1.txtClient = $"{txtsCodigoEAN.ClientID}";
            LeitorQuagga1.click = $"{cmdIncluirUnitizadoItem.ClientID}";
            LeitorQuagga1.RegisterQuaggaLibrary();
            LeitorQuagga1.RegistraQuaggaScript();
            LeitorQuagga1.AbrirCamera();
            UpdatePanel2.Update();
            INcluirItem = "S";
            DIV_CodigoEAN.Attributes["class"] += " visible";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "$('#modalUnitizado').modal('show');", true);
        }

        void dtgUNItensDataBind()
        {
            dtgUNItens.DataSource = ls_unitizadosItensModal.Where(c => c.sExibePedido.ToString() != "N");
            dtgUNItens.DataBind();
        }

        protected void btnIncluirEAN_Click(object sender, EventArgs e)
        {
            if (txtsCodigo_EAN.Text != "")
            {
                DataSet dsProduto = new DataSet();
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR-CODIGO-PRODUTOS");
                vParametros.Add("@sCodigoEAN", txtsCodigo_EAN.Text);
                vParametros.Add("@idOPI", hddidOPI.Value);
                dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsProduto))
                {
                    if (RETORNO.DATASET(dsProduto, 0, "sSeparacao") == "S")
                    {
                        MensagemPagina.MostraMensagem_Aviso("O código de barras '" + txtsCodigo_EAN.Text + "' já foi separado.");
                        txtsCodigo_EAN.Text = "";
                        //LeitorQuagga1.AbrirCamera();
                        return;
                    }
                    bool Inserir = false;
                    bool Separado = false;
                    foreach (GridViewRow item in gv_Arquivo.Rows)
                    {
                        if (item.Cells[1].Text == RETORNO.DATASET(dsProduto, 0, "sCodigo"))
                        {
                            if (decimal.Parse(item.Cells[5].Text) != decimal.Parse(item.Cells[6].Text))
                            {
                                List<cls_WMS_Produtos> novosItens = dsProduto.Tables[0].AsEnumerable().Select(row =>
                                {
                                    return new cls_WMS_Produtos()
                                    {
                                        IdItem = Convert.ToInt32(row["idItem"]),
                                        SDscProduto = row["sDscProduto"].ToString(),
                                        SCodigo = row["sCodigo"].ToString(),
                                        NQuantidade = 1,
                                        sExibePedido = "S",
                                        nOrdem = ls_Itens.Count + 1,
                                        SCodigoBarras = txtsCodigo_EAN.Text,
                                    };
                                }).ToList();

                                bool jaExiste = false;
                                if (txtsCodigoBarras.Text.StartsWith("PR"))
                                {
                                    foreach (var row in ls_Itens)
                                    {
                                        if (row.SCodigoBarras == txtsCodigo_EAN.Text && row.sExibePedido != "N")
                                        {
                                            jaExiste = true;
                                            break;
                                        }
                                    }
                                }

                                if (jaExiste)
                                {
                                    MensagemPagina.MostraMensagem_Erro("Este código de barras já foi adicionado.");
                                }
                                else
                                    ls_Itens.AddRange(novosItens);

                                gv_IncluirItensDataBind();
                                Inserir = true;
                            }
                            else
                            {
                                Separado = true;
                            }
                        }
                    }
                    if (Inserir == false)
                        if (Separado != true)
                            MensagemPagina.MostraMensagem_Erro("O item não pertence ao pedido!");
                        else
                            MensagemPagina.MostraMensagem_Erro("O item " + RETORNO.DATASET(dsProduto, 0, "sCodigo") + " ja foi separado!");
                    else
                        txtsCodigo_EAN.Text = "";
                }
                else
                {
                    MensagemPagina.MostraMensagem_Aviso("Escaneie um Código de Barras Válido para o Produto Desejado!");
                }
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Descreva o código Barras!");
            }

            MensagemPagina1.MostraMensagem_Aviso("Ao incluir o item a OPI deve ser salvo!");
            txtsCodigoBarras.Focus();
        }

        void gv_IncluirItensDataBind()
        {
            gv_IncluirItens.DataSource = ls_Itens.Where(c => c.sExibePedido.ToString() != "N");
            gv_IncluirItens.DataBind();
        }

        protected void gv_IncluirItens_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            int nOrdem = Convert.ToInt32(gv_IncluirItens.Rows[index].Cells[0].Text);
            string sExibePedido = ls_Itens[ls_Itens.FindIndex(x => x.nOrdem.Equals(nOrdem))].sExibePedido;

            ls_Itens[ls_Itens.FindIndex(x => x.nOrdem.Equals(nOrdem))].sExibePedido = "N";

            gv_IncluirItensDataBind();
        }

        protected void gv_IncluirItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlEmbalagemUnitizado, "sp_Select 'Flow_WMS_Embalagem'", "idEmbalagem", "sDScEmbalagem", false, "Selecione uma Embalagem", "0");
            FUNCOES.Popula_Combo(ddlEmbalagem, "sp_Select 'Flow_WMS_Embalagem'", "idEmbalagem", "sDScEmbalagem", false, "Selecione uma Embalagem", "0");
        }

        protected void ExportarExcel_Click(object sender, EventArgs e)
        {
            EXC_OPI();
        }

        void EXC_OPI()
        {
            try
            {
                Microsoft.Reporting.WebForms.ReportViewer rv4 = new Microsoft.Reporting.WebForms.ReportViewer();

                rv4.ProcessingMode = ProcessingMode.Local;
                rv4.LocalReport.EnableExternalImages = true;

                rv4.LocalReport.ReportPath = Server.MapPath("~/App/Reports/EnvioOPI.rdlc");

                string sFuncao = "CONSULTAR-ITENS";

                DataSet dsItensExcel;
                string sSql = "sp_Manipula_FLow_WMS_OPI";
                Dictionary<String, String> vParametrosItens = new Dictionary<string, string>();
                vParametrosItens.Add("@sFuncao", sFuncao);
                vParametrosItens.Add("@idOPI", hddidOPI.Value);

                dsItensExcel = BD.ExecutarDataSet(sSql, vParametrosItens);
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@idOPI", hddidOPI.Value);
                vParametros.Add("@sFuncao", "CONSULTAR");

                //if(selectedEnvioIds != null)
                //{
                //    EXC_ENVIO(selectedEnvioIds);
                //}                      

                DataSet dsOPI;
                dsOPI = BD.ExecutarDataSet("sp_Manipula_FLow_WMS_OPI", vParametros);

                if (dsOPI.Tables.Count == 0 || dsOPI.Tables[0].Rows.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao Gerar Excel: Nenhum dado encontrado.");
                    return;
                }
                if (dsItensExcel.Tables.Count == 0 || dsItensExcel.Tables[0].Rows.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Aviso("Sem Itens Nessa OPI");
                }


                rv4.LocalReport.DataSources.Clear();
                rv4.LocalReport.DataSources.Add(new ReportDataSource("dsOPI", dsOPI.Tables[0]));
                rv4.LocalReport.DataSources.Add(new ReportDataSource("dsItens", dsItensExcel.Tables[0]));
                DataRow row = dsOPI.Tables[0].AsEnumerable().FirstOrDefault();
                DataRow row2 = dsItensExcel.Tables[0].AsEnumerable().FirstOrDefault();

                if (row != null)
                {
                    ReportParameter[] rp = new ReportParameter[5];

                    // Substituindo com os dados das colunas especificadas
                    rp[0] = new ReportParameter("idOPI", row["idOPI"].ToString());
                    rp[1] = new ReportParameter("sReferencia", row["sReferencia"].ToString());
                    rp[2] = new ReportParameter("sCliente", row["sCliente"].ToString());
                    rp[3] = new ReportParameter("sStatus", row["sStatus"].ToString());
                    rp[4] = new ReportParameter("dtOPI", row["dtOPI"].ToString());

                    rv4.LocalReport.SetParameters(rp);
                    rv4.LocalReport.Refresh();

                    Microsoft.Reporting.WebForms.Warning[] warnings;
                    string[] streamIds;
                    string mimeType, encoding, extension;

                    byte[] bytes = rv4.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                    string sNomeArquivoOriginal = "PL_" + dsOPI.Tables[0].Rows[0]["sReferencia"].ToString() + "_" + FUNCOES.CarimboDataHora() + ".xls";
                    string sNomeArquivo = LimparNomeArquivo(sNomeArquivoOriginal);

                    File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);
                    Pesquisar(hddidOPI.Value);
                    FUNCOES.DownloadArquivo(Page, sNomeArquivo);
                    // Mostrar mensagem de sucesso
                    MensagemPagina.MostraMensagem_Sucesso("Excel da OPI gerada com sucesso!");
                }
                else
                {
                    // Lidar com o caso onde não há dados
                    MensagemPagina.MostraMensagem_Erro("Erro ao Gerar Excel");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao gerar o Excel da OPI! </br>" + ex.Message);
            }
        }
        #endregion

        #region | Script
        void RegistraScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$('[id*=txtNPesoLiquido]').mask('0.000.000.009,99', { reverse: true });");
            sb.Append("$('[id*=txtNPesoBruto]').mask('0.000.000.009,99', { reverse: true });");
            sb.Append("$('[id*=txtNComprimento]').mask('0.000.000.009,99', { reverse: true });");
            sb.Append("$('[id*=txtNLargura]').mask('0.000.000.009,99', { reverse: true });");
            sb.Append("$('[id*=txtNAltura]').mask('0.000.000.009,99', { reverse: true });");

            sb.AppendLine("    var leitura = $('#" + hddleitura.ClientID + "').val() || null;");
            sb.AppendLine("    var ultimaleitura = $('#" + hddultimaleitura.ClientID + "').val() || 0;");
            sb.AppendLine("    var isProcessed = false;");
            sb.AppendLine("    Quagga.onDetected(function (data) {");
            sb.AppendLine("            const agora = Date.now();");
            sb.AppendLine("            if(data.codeResult.code !== leitura || (agora - ultimaleitura) > 1000){");
            sb.AppendLine("                 leitura = data.codeResult.code;");
            sb.AppendLine("                 $('#" + hddleitura.ClientID + "').val(leitura);");
            sb.AppendLine("                 ultimaleitura = agora;");
            sb.AppendLine("                 $('#" + hddultimaleitura.ClientID + "').val(agora);");
            sb.AppendLine("                 document.querySelector('#" + txtsCodigoEAN.ClientID + "').value = data.codeResult.code;");
            sb.AppendLine("                 document.querySelector('#" + cmdIncluirUnitizadoItem.ClientID + "').click();");
            sb.AppendLine("                 console.log($('#" + hddleitura.ClientID + "').val(), $('#" + hddultimaleitura.ClientID + "').val());");
            sb.AppendLine("");
            sb.AppendLine("             }");
            sb.AppendLine("    });");

            sb.Append("$('.composicaoLinha').addClass('fa fa-plus');\r\n");
            sb.Append("$('.composicaoLinha').off('click').on('click', function() {\r\n");
            sb.Append("     var icon = $(this);\r\n");
            sb.Append("     var divId = $(this).data('div-id');\r\n");
            sb.Append("     var current = $('#' + divId).css('display');\r\n");
            sb.Append("     if (current == 'none') {\r\n");
            sb.Append("         $('#' + divId).show('slow');\r\n");
            sb.Append("         icon.removeClass('fa fa-plus').addClass('fa fa-minus');\r\n");
            sb.Append("     } else {\r\n");
            sb.Append("         $('#' + divId).hide('slow');\r\n");
            sb.Append("         icon.removeClass('fa fa-minus').addClass('fa fa-plus');\r\n");
            sb.Append("     }\r\n");
            sb.Append("     return false;\r\n");
            sb.Append("});\r\n\r\n");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina" + Guid.NewGuid(), sb.ToString(), true);
        }

        void RegistraScriptCamposPeso()
        {
            string script = @"<script>
                $('[id*=txtNPesoLiquido]').mask('000.000.000.000.000,00', { reverse: true });
                $('[id*=txtNPesoBruto]').mask('000.000.000.000.000,00', { reverse: true });
                $('[id*=txtNComprimento]').mask('000.000.000.000.000,00', { reverse: true });
                $('[id*=txtNLargura]').mask('000.000.000.000.000,00', { reverse: true });
                $('[id*=txtNAltura]').mask('000.000.000.000.000,00', { reverse: true });

                function calcularPesoTotal() {
                    var largura = parseFloat(document.getElementById('" + txtNLargura.ClientID + @"').value.replace(/\./g, '').replace(',', '.'));
                    var altura = parseFloat(document.getElementById('" + txtNAltura.ClientID + @"').value.replace(/\./g, '').replace(',', '.'));
                    var comprimento = parseFloat(document.getElementById('" + txtNComprimento.ClientID + @"').value.replace(/\./g, '').replace(',', '.'));

                    console.log('Largura: ' + largura);
                    console.log('Altura: ' + altura);
                    console.log('Comprimento: ' + comprimento);

                    if (!isNaN(largura) && !isNaN(altura) && !isNaN(comprimento)) {
                        var volumeCubico = comprimento * largura * altura;
                        console.log('Volume Cubico: ' + volumeCubico);
                        document.getElementById('" + lblVolumeCubico.ClientID + @"').innerHTML = 'Volume m³: ' + volumeCubico.toFixed(2) + ' m³';
                    } else {
                        document.getElementById('" + lblVolumeCubico.ClientID + @"').innerHTML = 'Calculando...';
                    }
                }

                // Re-registrar máscaras após uma atualização parcial da página
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
                    $('[id*=txtNPesoLiquido]').mask('000.000.000.000.000,00', { reverse: true });
                    $('[id*=txtNPesoBruto]').mask('000.000.000.000.000,00', { reverse: true });
                    $('[id*=txtNComprimento]').mask('000.000.000.000.000,00', { reverse: true });
                    $('[id*=txtNLargura]').mask('000.000.000.000.000,00', { reverse: true });
                    $('[id*=txtNAltura]').mask('000.000.000.000.000,00', { reverse: true });
                });
            </script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "calcularPesoTotal", script, false);
        }

        void ScriptLeitor()
        {
            //LeitorQuagga1.txtClient = $"{txtsCodigoEAN.ClientID}";
            //LeitorQuagga1.click = $"{cmdIncluirUnitizadoItem.ClientID}";
            //LeitorQuagga1.RegisterQuaggaLibrary();
            //LeitorQuagga1.AbrirCamera();
            ////LeitorQuagga1.RegistraQuaggaScript($"{txtsCodigoEAN.ClientID}", $"{cmdIncluirUnitizadoItem.ClientID}");
            //LeitorQuagga1.RegistraQuaggaScript();
        }
        #endregion

        #region | Modal
        protected void cmdIncluirUnitizadoItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtsCodigoEAN.Text))
            {
                if (INcluirItem == "S")
                {
                    txtsCodigo_EAN.Text = txtsCodigoEAN.Text;
                    btnIncluirEAN_Click(sender, e);
                    LeitorQuagga1.FecharCamera();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "fecharModal", "$('#modalUnitizado').modal('hide');", true);
                    //LeitorQuagga1.DesligarCam();
                    UpdatePanel2.Update();
                }
                if (INcluirItem == "U")
                {
                    txtsCodigoBarras.Text = txtsCodigoEAN.Text;
                    IncluirUnitizadosItens_Click(sender, e);
                    LeitorQuagga1.FecharCamera();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "fModal", "$('#modalUnitizado').modal('hide');", true);
                    UpdatePanel2.Update();
                }
                if (INcluirItem == "E")
                {
                    txtsCodigoBarrasVolume.Text = txtsCodigoEAN.Text;
                    cmdIncluirVolume_Click(sender, e);
                    LeitorQuagga1.FecharCamera();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "fechar", "$('#modalUnitizado').modal('hide');", true);
                    UpdatePanel2.Update();
                }
                var currentClass = DIV_CodigoEAN.Attributes["class"];
                if (!currentClass.Contains("visible"))
                {
                    DataSet dsProduto = new DataSet();
                    Dictionary<string, string> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR-CODIGO-PRODUTOS");
                    vParametros.Add("@sCodigoEAN", txtsCodigoEAN.Text);
                    vParametros.Add("@idOPI", hddidOPI.Value);
                    dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsProduto))
                    {
                        if (RETORNO.DATASET(dsProduto, 0, "sSeparacao") == "S")
                        {
                            MensagemPagina.MostraMensagem_Aviso("O código de barras '" + txtsCodigoEAN.Text + "' já foi separado.");
                            txtsCodigoEAN.Text = "";
                            //LeitorQuagga1.AbrirCamera();
                            return;
                        }
                        bool Inserir = false;
                        bool Separado = false;
                        bool jaExiste = false;
                        foreach (GridViewRow item in gv_Arquivo.Rows)
                        {
                            if (item.Cells[1].Text == RETORNO.DATASET(dsProduto, 0, "sCodigo"))
                            {
                                if (decimal.Parse(item.Cells[5].Text) != decimal.Parse(item.Cells[6].Text))
                                {
                                    List<cls_WMS_Produtos> novosItens = dsProduto.Tables[0].AsEnumerable().Select(row =>
                                    {
                                        return new cls_WMS_Produtos()
                                        {
                                            IdItem = Convert.ToInt32(row["idItem"]),
                                            SDscProduto = row["sDscProduto"].ToString(),
                                            SCodigo = row["sCodigo"].ToString(),
                                            NQuantidade = 1,
                                            sExibePedido = "S",
                                            nOrdem = ls_unitizadosItensModal.Count + 1,
                                            SCodigoBarras = txtsCodigoEAN.Text,
                                        };
                                    }).ToList();

                                    if (txtsCodigoBarras.Text.StartsWith("PR"))
                                    {
                                        foreach (var row in ls_unitizadosItensModal)
                                        {
                                            if (row.SCodigoBarras == txtsCodigoEAN.Text && row.sExibePedido != "N")
                                            {
                                                jaExiste = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (jaExiste)
                                    {
                                        MensagemPagina.MostraMensagem_Erro("Este código de barras já foi adicionado.");
                                    }
                                    else
                                        ls_unitizadosItensModal.AddRange(novosItens);

                                    dtgUNItensDataBind();
                                    Inserir = true;
                                }
                                else
                                {
                                    Separado = true;
                                }
                            }
                        }
                        if (Inserir == false)
                            if (Separado != true)
                                MensagemPagina.MostraMensagem_Erro("O item não pertence ao pedido!");
                            else
                                MensagemPagina.MostraMensagem_Erro("O item " + RETORNO.DATASET(dsProduto, 0, "sCodigo") + " ja foi separado!");
                        else
                        {
                            if (jaExiste != true)
                                MensagemPagina.MostraMensagem_Sucesso("O item foi incluido com sucesso!");
                        }
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Aviso("Escaneie um Código de Barras Válido para o Produto Desejado!");
                    }
                }
            }
            else
            {
                MensagemPagina.MostraMensagem_Aviso("Descreva o Campo do Código de Barras!");
            }
            txtsCodigoEAN.Text = "";
            txtsCodigoBarras.Text = "";
            txtsCodigoBarrasVolume.Text = "";
            //LeitorQuagga1.AbrirCamera();
            RegistraScript();
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (ls_unitizadosItensModal.Count > 0)
            {
                foreach (GridViewRow row in dtgUNItens.Rows)
                {
                    if (row.Cells[3].Text != "")
                    {
                        string InserirQauntidade = row.Cells[3].Text;
                        bool Inserir = false;
                        foreach (var list in ls_unitizadosItensModal)
                        {
                            if (list.sExibePedido == "S")
                            {
                                foreach (GridViewRow item in gv_Arquivo.Rows)
                                {
                                    decimal Quantidade = decimal.Parse(item.Cells[5].Text) - decimal.Parse(item.Cells[6].Text);
                                    if (Quantidade >= decimal.Parse(InserirQauntidade) && item.Cells[1].Text == row.Cells[1].Text && list.SCodigo == item.Cells[1].Text)
                                    {
                                        DataSet dsProduto = new DataSet();
                                        Dictionary<string, string> vParametros = new Dictionary<string, string>();
                                        vParametros.Add("@sFuncao", "INCLUIR_SEPARACAO");
                                        vParametros.Add("@idPedido", hddidPedido.Value);
                                        vParametros.Add("@idProduto", list.IdItem.ToString());
                                        vParametros.Add("@nQuantidadeSeparacao", InserirQauntidade);
                                        vParametros.Add("@idOPI", hddidOPI.Value);
                                        vParametros.Add("@sCodigoBarras", list.SCodigoBarras);
                                        vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                                        dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);

                                        Inserir = true;

                                        DataSet dsProduto_SEPARACAO = new DataSet();
                                        Dictionary<string, string> vParametros_SEPARACAO = new Dictionary<string, string>();
                                        vParametros_SEPARACAO.Add("@sFuncao", "SEPARACAO_ITEM");
                                        vParametros_SEPARACAO.Add("@sSeparacao", "S");
                                        vParametros_SEPARACAO.Add("@sCodigoEAN", list.SCodigoBarras);
                                        vParametros_SEPARACAO.Add("@idOPI", hddidOPI.Value);
                                        dsProduto_SEPARACAO = BD.ExecutarDataSet(sProcedure, vParametros_SEPARACAO);
                                    }
                                }
                            }
                            Pesquisar(hddidOPI.Value);
                        }
                        if (Inserir == false)
                            MensagemPagina.MostraMensagem_Erro("A quantidade informada é superior à quantidade no pedido!");
                        else
                        {
                            FUNCOES.DirecionaPagina("App/Paginas/WMS/SeparacaoPedido_Detalhe.aspx?id=" + hddidOPI.Value + "&msg=S");
                        }
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro("Descreva a Quantidade do Item!");
                        break;
                    }
                }
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Adicione o código Barras do item!");
            }
        }

        protected void dtgUNItens_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            int nOrdem = Convert.ToInt32(dtgUNItens.Rows[index].Cells[0].Text);
            string sExibePedido = ls_unitizadosItensModal[ls_unitizadosItensModal.FindIndex(x => x.nOrdem.Equals(nOrdem))].sExibePedido;

            ls_unitizadosItensModal[ls_unitizadosItensModal.FindIndex(x => x.nOrdem.Equals(nOrdem))].sExibePedido = "N";

            dtgUNItensDataBind();
            LeitorQuagga1.AbrirCamera();
        }

        protected void dtgUNItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }
        #endregion

        #region | Unitizado
        protected void ProximoUnitizado_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlEmbalagemUnitizado.SelectedValue != "0" && txtsDscUnitizado.Text != "")
                {
                    string sErro = "";
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR-UNITIZADO");
                    vParametros.Add("@sDscUnitizado", txtsDscUnitizado.Text);
                    vParametros.Add("@idProdutoEmbalagem", ddlEmbalagemUnitizado.SelectedValue);
                    vParametros.Add("@idOPI", hddidOPI.Value);
                    vParametros.Add("@idUnitizado", "0");
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        idUnitizado = int.Parse(RETORNO.DATASET(dsPesquisa, 0, "idUnitizado"));
                        DIV_UNITIZADOS_Modal.Visible = true;
                        DivBipadorVolume.Visible = true;
                        DIV_UnitizadosItens.Visible = true;
                        DIV_IdEmbalagem.Visible = false;
                        DIV_sDscUnitizado.Visible = false;
                        DIV_ProximoUnitizado.Visible = false;
                        DIV_SalvarUnitizado.Visible = false;
                        DIV_Separacao.Visible = true;
                        LeitorQuagga1.AbrirCamera();
                    }
                }
                else
                {
                    string mensagem = "";
                    if (ddlEmbalagemUnitizado.SelectedValue == "0")
                    {
                        mensagem += (mensagem != "" ? "</br>" : "") + "Selecione o tipo de embalagem!";
                    }
                    if (txtsDscUnitizado.Text == "")
                    {
                        mensagem += (mensagem != "" ? "</br>" : "") + "Informe a descrição do unitizado!";
                    }
                    MensagemPagina.MostraMensagem_Erro(mensagem);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void IncluirUnitizadosItens_Click(object sender, EventArgs e)
        {
            if (txtsCodigoBarras.Text != "")
            {
                DataSet dsProduto = new DataSet();
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR-CODIGO-PRODUTOS");
                vParametros.Add("@sCodigoEAN", txtsCodigoBarras.Text);
                vParametros.Add("@idOPI", hddidOPI.Value);
                dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsProduto))
                {
                    if (RETORNO.DATASET(dsProduto, 0, "sSeparacao") == "S")
                    {
                        MensagemPagina.MostraMensagem_Aviso("O código de barras '" + txtsCodigoBarras.Text + "' já foi separado.");
                        txtsCodigoBarras.Text = "";
                        //LeitorQuagga1.AbrirCamera();
                        return;
                    }
                    bool Inserir = false;
                    bool Separado = false;
                    foreach (GridViewRow item in gv_Arquivo.Rows)
                    {
                        if (item.Cells[1].Text == RETORNO.DATASET(dsProduto, 0, "sCodigo"))
                        {
                            if (decimal.Parse(item.Cells[5].Text) != decimal.Parse(item.Cells[6].Text))
                            {
                                List<cls_WMS_Produtos> novosItens = dsProduto.Tables[0].AsEnumerable().Select(row =>
                                {
                                    return new cls_WMS_Produtos()
                                    {
                                        IdItem = Convert.ToInt32(row["idItem"]),
                                        SDscProduto = row["sDscProduto"].ToString(),
                                        SCodigo = row["sCodigo"].ToString(),
                                        NQuantidade = 1,
                                        sExibePedido = "S",
                                        nOrdem = ls_UnitizadosItens.Count + 1,
                                        SCodigoBarras = txtsCodigoBarras.Text,
                                        IdUnitizadoItem = 0,
                                        IdUnitizado = idUnitizado,
                                        SFuncao = "INCLUIR_UNITIZADOS_SEPARACAO",
                                    };
                                }).ToList();

                                bool jaExiste = false;
                                if (txtsCodigoBarras.Text.StartsWith("PR"))
                                {
                                    foreach (var row in ls_UnitizadosItens)
                                    {
                                        if (row.SCodigoBarras == txtsCodigoBarras.Text && row.SFuncao != "EXCLUIR_UNITIZADOS_SEPARACAO")
                                        {
                                            jaExiste = true;
                                            break;
                                        }
                                    }
                                }

                                if (jaExiste)
                                {
                                    MensagemPagina.MostraMensagem_Erro("Este código de barras já foi adicionado.");
                                }
                                else
                                    ls_UnitizadosItens.AddRange(novosItens);

                                gv_UnitizadosItensDataBind();
                                DIV_gv_UnitizadosItens.Visible = true;
                                DIV_SalvarUnitizado.Visible = true;
                                Inserir = true;
                            }
                            else
                            {
                                Separado = true;
                            }
                        }
                    }
                    if (Inserir == false)
                        if (Separado != true)
                            MensagemPagina.MostraMensagem_Erro("O item não pertence ao pedido!");
                        else
                            MensagemPagina.MostraMensagem_Erro("O item " + RETORNO.DATASET(dsProduto, 0, "sCodigo") + " ja foi separado!");
                    else
                        txtsCodigo_EAN.Text = "";
                }
                else
                {
                    MensagemPagina.MostraMensagem_Aviso("Escaneie um Código de Barras Válido para o Produto Desejado!");
                }

            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Descreva o código Barras!");
            }
            //LeitorQuagga1.AbrirCamera();
        }

        void gv_UnitizadosItensDataBind()
        {
            gv_UnitizadosItens.DataSource = ls_UnitizadosItens.Where(c => c.SFuncao != "EXCLUIR_UNITIZADOS_SEPARACAO");
            gv_UnitizadosItens.DataBind();
        }

        protected void gv_UnitizadosItens_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            int nOrdem = Convert.ToInt32(gv_UnitizadosItens.Rows[index].Cells[0].Text);
            string SFuncao = ls_UnitizadosItens[ls_UnitizadosItens.FindIndex(x => x.nOrdem.Equals(nOrdem))].SFuncao;

            ls_UnitizadosItens[ls_UnitizadosItens.FindIndex(x => x.nOrdem.Equals(nOrdem))].SFuncao = "EXCLUIR_UNITIZADOS_SEPARACAO";

            gv_UnitizadosItensDataBind();
            LeitorQuagga1.AbrirCamera();
        }

        protected void gv_UnitizadosItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0, 1);
        }

        protected void gv_Unitizados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var gv = e.Row.FindControl("gv_abaUnitizadosItens") as GridView;
                if (gv != null)
                {
                    DataTable dt = ds.Tables[3].Copy();
                    int contador = 0;
                    if (dt.Rows.Count > 0)
                    {
                        while (true)
                        {
                            if (dt.Rows[contador].Field<int>("idUnitizado") != int.Parse((e.Row.FindControl("idUnitizado") as Label).Text))
                                dt.Rows.RemoveAt(contador);
                            else
                                contador++;
                            if (contador >= dt.Rows.Count)
                                break;
                        }
                        gv.DataSource = dt;
                        gv.DataBind();
                    }
                }

                LinkButton lnkEditar = e.Row.FindControl("lnkEditar") as LinkButton;
                LinkButton btnFecharCaixa = e.Row.FindControl("btnFecharCaixa") as LinkButton;

                if (e.Row.Cells[3].Text == "S")
                {
                    lnkEditar.Visible = false;
                    btnFecharCaixa.Visible = false;
                }
                else
                {
                    lnkEditar.Visible = true;
                    btnFecharCaixa.Visible = true;
                }
            }
            GRID.EsconderColunas(e, 3);
        }

        protected void gv_abaUnitizadosItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0, 1, 2);
        }

        protected void btnSalvarUnitizado_Click(object sender, EventArgs e)
        {
            try
            {
                if (ls_UnitizadosItens.Count > 0)
                {
                    bool Inserir = false;

                    foreach (var list in ls_UnitizadosItens)
                    {
                        string InserirQauntidade = list.NQuantidade.ToString();
                        foreach (GridViewRow item in gv_Arquivo.Rows)
                        {
                            decimal Quantidade = decimal.Parse(item.Cells[5].Text) - decimal.Parse(item.Cells[6].Text);
                            if (Quantidade >= decimal.Parse(InserirQauntidade) && list.SCodigo == item.Cells[1].Text)
                            {
                                DataSet dsProduto = new DataSet();
                                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                                vParametros.Add("@sFuncao", list.SFuncao);
                                vParametros.Add("@idPedido", hddidPedido.Value);
                                vParametros.Add("@idProduto", list.IdItem.ToString());
                                vParametros.Add("@nQuantidadeSeparacao", InserirQauntidade.Replace(",", "."));
                                vParametros.Add("@idOPI", hddidOPI.Value);
                                vParametros.Add("@idUnitizado", list.IdUnitizado.ToString());
                                vParametros.Add("@idUnitizadoItem", list.IdUnitizadoItem.ToString());
                                vParametros.Add("@sCodigoBarras", list.SCodigoBarras);
                                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                                dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);

                                Inserir = true;

                                if (list.SFuncao != "EXCLUIR_UNITIZADOS_SEPARACAO")
                                {
                                    DataSet dsProduto_SEPARACAO = new DataSet();
                                    Dictionary<string, string> vParametros_SEPARACAO = new Dictionary<string, string>();
                                    vParametros_SEPARACAO.Add("@sFuncao", "SEPARACAO_ITEM");
                                    vParametros_SEPARACAO.Add("@sSeparacao", "S");
                                    vParametros_SEPARACAO.Add("@sCodigoEAN", list.SCodigoBarras);
                                    vParametros_SEPARACAO.Add("@idOPI", hddidOPI.Value);
                                    dsProduto_SEPARACAO = BD.ExecutarDataSet(sProcedure, vParametros_SEPARACAO);
                                }
                            }
                        }
                        DIV_IdEmbalagem.Visible = true;
                        DIV_sDscUnitizado.Visible = true;
                        DIV_ProximoUnitizado.Visible = true;
                        Pesquisar(hddidOPI.Value);
                        DIV_UNITIZADOS_Modal.Visible = true;
                    }
                    if (Inserir == false)
                        MensagemPagina.MostraMensagem_Erro("A quantidade informada é superior à quantidade no pedido!");
                    else
                    {
                        MensagemPagina.MostraMensagem_Sucesso("Os itens foram salvo com sucesso!");
                        txtsDscUnitizado.Text = "";
                        bool Finalizar = false;
                        foreach (GridViewRow item in gv_Arquivo.Rows)
                        {
                            if ((item.FindControl("FaltaSeparar") as Label).Text == "0,00")
                            {
                                Finalizar = true;
                            }
                            else
                            {
                                Finalizar = false;
                                break;
                            }
                        }
                        if (Finalizar == true)
                        {
                            DataSet dsProduto = new DataSet();
                            Dictionary<string, string> vParametros = new Dictionary<string, string>();
                            vParametros.Add("@sFuncao", "CONCLUIR_SEPARACAO");
                            vParametros.Add("@sSeparacao", "S");
                            vParametros.Add("@idOPI", hddidOPI.Value);
                            dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);
                        }
                        else
                        {
                            DataSet dsProduto = new DataSet();
                            Dictionary<string, string> vParametros = new Dictionary<string, string>();
                            vParametros.Add("@sFuncao", "CONCLUIR_SEPARACAO");
                            vParametros.Add("@sSeparacao", "E");
                            vParametros.Add("@idOPI", hddidOPI.Value);
                            dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);
                        }
                        ls_UnitizadosItens.Clear();
                        aba_Unitizados.Visible = true;
                        FUNCOES.DirecionaPagina("App/Paginas/WMS/SeparacaoPedido_Detalhe.aspx?id=" + hddidOPI.Value + "&msg=S" + "&Uni=S");
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void cmdUnitizados_Click(object sender, EventArgs e)
        {
            DIV_Separacao.Visible = true;
            //DIV_CodigoEAN.Attributes["class"] += " visible";
            DIV_separacaoitens.Visible = false;
            DIV_ENVIOS.Visible = false;
            DIV_UNITIZADOS_Modal.Visible = true;
            if (DIV_UnitizadosItens.Visible == true)
            {
                LeitorQuagga1.AbrirCamera();
                DivBipadorVolume.Visible = true;
            }
            else
                DivBipadorVolume.Visible = false;
        }

        protected void btnFecharCaixa_Click(object sender, EventArgs e)
        {
            LinkButton btnFecharCaixa = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btnFecharCaixa.NamingContainer;
            string idUnitizado = (row.FindControl("idUnitizado") as Label).Text;

            DataSet dsProduto = new DataSet();
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "Finalizar_Unitizado");
            vParametros.Add("@sFinalizado", "S");
            vParametros.Add("@idUnitizado", idUnitizado);
            dsProduto = BD.ExecutarDataSet(sProcedure, vParametros);

            Pesquisar(hddidOPI.Value);
            DIV_UNITIZADOS_Modal.Visible = true;
            DIV_IdEmbalagem.Visible = true;
            DIV_sDscUnitizado.Visible = true;
            DIV_ProximoUnitizado.Visible = true;
            MensagemPagina.MostraMensagem_Sucesso("Caixa foi fechado com sucesso!");
        }

        protected void gv_Unitizados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Editar")
            {
                ls_UnitizadosItens.Clear();
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = ((GridView)sender).Rows[index];

                idUnitizado = Convert.ToInt32(gv_Unitizados.DataKeys[index].Value);
                DIV_ProximoUnitizado.Visible = false;
                DIV_sDscUnitizado.Visible = false;
                DIV_IdEmbalagem.Visible = false;
                DIV_SalvarUnitizado.Visible = false;
                //Div14.Visible = true;
                DivBipadorVolume.Visible = true;
                DIV_UnitizadosItens.Visible = true;
                DIV_SalvarUnitizado.Visible = true;

                GridView gvItens = (GridView)row.FindControl("gv_abaUnitizadosItens");

                if (gvItens != null)
                {
                    foreach (GridViewRow item in gvItens.Rows)
                    {
                        string rawText = HttpUtility.HtmlDecode(item.Cells[3].Text);
                        string[] SDscProduto = rawText.Split(new[] { " - " }, StringSplitOptions.None);

                        FrameWork.cls_WMS_Produtos objItem = new FrameWork.cls_WMS_Produtos();
                        objItem.IdItem = int.Parse(item.Cells[2].Text);
                        objItem.SDscProduto = SDscProduto.Length > 1 ? SDscProduto[1] : "";
                        objItem.SCodigo = SDscProduto.Length > 0 ? SDscProduto[0] : "";
                        objItem.NQuantidade = decimal.Parse(item.Cells[4].Text);
                        objItem.sExibePedido = "S";
                        objItem.nOrdem = ls_UnitizadosItens.Count + 1;
                        objItem.SCodigoBarras = item.Cells[5].Text;
                        objItem.IdUnitizadoItem = int.Parse(item.Cells[1].Text);
                        objItem.IdUnitizado = int.Parse(item.Cells[0].Text);
                        objItem.SFuncao = "INCLUIR_UNITIZADOS_SEPARACAO";
                        ls_UnitizadosItens.Add(objItem);
                    }
                }

                gv_UnitizadosItensDataBind();
                DIV_gv_UnitizadosItens.Visible = true;
                LeitorQuagga1.AbrirCamera();
            }
        }
        #endregion

        #region | Volumes Envios
        void PopularItens(string idOPI)
        {
            string sFuncao = "CONSULTAR_ENVIOS_SEPARACAO";

            DataTable tb;
            string sSql = "sp_Manipula_FLow_WMS_OPI";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idOPI", idOPI);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            for (int i = tb.Rows.Count - 1; i >= 0; i--)
            {
                DataRow linha = tb.Rows[i];
                if (linha["nQtdEnviado"].ToString() == "0,0000")
                {
                    tb.Rows.RemoveAt(i);
                }
            }

            if (tb.Rows.Count > 0)
            {
                dtgItens.DataSource = tb;
                dtgItens.DataBind();
                DIV_dtgItens.Visible = true;
            }
            else
            {
                DIV_dtgItens.Visible = false;
            }
        }

        protected void btnEnvio_Click(object sender, EventArgs e)
        {
            DIV8.Visible = true;
            DivBipadorVolume.Visible = true;
            LeitorQuagga1.AbrirCamera();
            DIV10.Visible = false;
        }

        protected void cmdIncluirVolume_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtsCodigoBarrasVolume.Text))
                {
                    bool incluir = false;
                    if (txtsCodigoBarrasVolume.Text.Substring(0, 2) != "PR" && txtsCodigoBarrasVolume.Text.Substring(0, 2) != "UN" && txtsCodigoBarrasVolume.Text.Substring(0, 2) != "VOL")
                    {
                        foreach (GridViewRow linha in dtgItens.Rows)
                        {
                            if (linha.Cells[0].Text == txtsCodigoBarrasVolume.Text)
                            {
                                if (linha.Cells[5].Text != "0,00")
                                {
                                    incluir = true;
                                }
                                else
                                {
                                    throw new Exception("O item não foi separado!");
                                }
                            }
                        }
                    }
                    DataSet dsVolumeItem = new DataSet();
                    Dictionary<string, string> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR-CODIGO-BARRAS-VOL");
                    vParametros.Add("@sCodigoBarras", txtsCodigoBarrasVolume.Text);
                    vParametros.Add("@idOPI", hddidOPI.Value);
                    dsVolumeItem = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (dsVolumeItem.Tables[0].Rows.Count > 0)
                    {
                        if (txtsCodigoBarrasVolume.Text.Substring(0, 2) == "PR")
                        {
                            if (RETORNO.DATASET(dsVolumeItem, 0, "sSeparacao") == "N")
                            {
                                MensagemPagina.MostraMensagem_Aviso("O código de barras '" + txtsCodigoBarrasVolume.Text + "' não foi separado!.");
                                txtsCodigoBarrasVolume.Text = "";
                                //LeitorQuagga1.AbrirCamera();
                                return;
                            }
                            if (BD.ValidarDataSet(dsVolumeItem))
                            {
                                List<cls_WMS_VolumesItens> novosItens = dsVolumeItem.Tables[0].AsEnumerable().Select(row =>
                                {
                                    return new cls_WMS_VolumesItens()
                                    {
                                        IdObjeto = Convert.ToInt32(row["idProduto"]),
                                        SDscObjeto = row["sDscProduto"].ToString(),
                                        SCodigoBarras = txtsCodigoBarrasVolume.Text,
                                        //NVolume = Convert.ToInt32(txtnVolume.Text),
                                    };
                                }).ToList();

                                ls_VolumesItens.AddRange(novosItens);

                                dtgVolumesItensDataBind();
                                cmdGerarVolume.Visible = true;
                            }
                            else
                            {
                                MensagemPagina.MostraMensagem_Aviso("Código de Barras Inválido para o Produto Desejado!");
                            }
                        }
                        else if (txtsCodigoBarrasVolume.Text.Substring(0, 2) == "UN")
                        {
                            if (RETORNO.DATASET(dsVolumeItem, 0, "sFinalizado") == "N")
                            {
                                MensagemPagina.MostraMensagem_Aviso("A caixa '" + txtsCodigoBarrasVolume.Text + "' precisa ser fechada antes de continuar.");
                                txtsCodigoBarrasVolume.Text = "";
                                //LeitorQuagga1.AbrirCamera();
                                return;
                            }
                            if (RETORNO.DATASET(dsVolumeItem, 0, "sSeparacao") == "S")
                            {
                                MensagemPagina.MostraMensagem_Aviso("O código de barras '" + txtsCodigoBarrasVolume.Text + "' já foi enviado.");
                                txtsCodigoBarrasVolume.Text = "";
                                //LeitorQuagga1.AbrirCamera();
                                return;
                            }
                            if (BD.ValidarDataSet(dsVolumeItem))
                            {
                                List<cls_WMS_VolumesItens> novosItens = dsVolumeItem.Tables[0].AsEnumerable().Select(row =>
                                {
                                    return new cls_WMS_VolumesItens()
                                    {
                                        IdObjeto = Convert.ToInt32(row["idUnitizado"]),
                                        SDscObjeto = row["sDscUnitizado"].ToString(),
                                        SCodigoBarras = txtsCodigoBarrasVolume.Text,
                                        //NVolume = Convert.ToInt32(txtnVolume.Text),
                                    };
                                }).ToList();

                                ls_VolumesItens.AddRange(novosItens);

                                dtgVolumesItensDataBind();
                                cmdGerarVolume.Visible = true;
                            }
                            else
                            {
                                MensagemPagina.MostraMensagem_Aviso("Código de Barras InVálido para o Unitizado Desejado!");
                            }
                        }
                        else if (txtsCodigoBarrasVolume.Text.Substring(0, 2) == "VOL")
                        {
                            if (RETORNO.DATASET(dsVolumeItem, 0, "sSeparacao") == "S")
                            {
                                MensagemPagina.MostraMensagem_Aviso("O código de barras '" + txtsCodigoBarrasVolume.Text + "' já foi enviado.");
                                txtsCodigoBarrasVolume.Text = "";
                                //LeitorQuagga1.AbrirCamera();
                                return;
                            }
                            if (BD.ValidarDataSet(dsVolumeItem))
                            {
                                List<cls_WMS_VolumesItens> novosItens = dsVolumeItem.Tables[0].AsEnumerable().Select(row =>
                                {
                                    return new cls_WMS_VolumesItens()
                                    {
                                        IdObjeto = Convert.ToInt32(row["idVolume"]),
                                        SDscObjeto = row["sDscVolume"].ToString(),
                                        SCodigoBarras = txtsCodigoBarrasVolume.Text,
                                        //NVolume = Convert.ToInt32(txtnVolume.Text),
                                    };
                                }).ToList();

                                ls_VolumesItens.AddRange(novosItens);

                                dtgVolumesItensDataBind();
                                cmdGerarVolume.Visible = true;
                            }
                            else
                            {
                                MensagemPagina.MostraMensagem_Aviso("Código de Barras InVálido para o Unitizado Desejado!");
                            }
                        }
                        else
                        {
                            List<cls_WMS_VolumesItens> novosItens = dsVolumeItem.Tables[0].AsEnumerable().Select(row =>
                            {
                                return new cls_WMS_VolumesItens()
                                {
                                    IdObjeto = Convert.ToInt32(row["idProdutos"]),
                                    SDscObjeto = row["sDscProduto"].ToString(),
                                    SCodigoBarras = txtsCodigoBarrasVolume.Text,
                                    //NVolume = Convert.ToInt32(txtnVolume.Text),
                                };
                            }).ToList();

                            ls_VolumesItens.AddRange(novosItens);

                            dtgVolumesItensDataBind();
                            cmdGerarVolume.Visible = true;
                        }
                        div_botoesVolumes.Visible = true;
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro("Código de barras informado não pertence ao pedido!");
                        ddlEmbalagem.SelectedValue = "0";
                    }
                    txtsCodigoBarrasVolume.Text = "";
                }
                else
                {
                    string mensagem = "";
                    if (txtsCodigoBarrasVolume.Text == "")
                    {
                        mensagem += (mensagem != "" ? "</br>" : "") + "Descreva o código de barras!";
                    }
                    if (ddlEmbalagem.SelectedValue == "0")
                    {
                        mensagem += (mensagem != "" ? "</br>" : "") + "Selecione a embalagem!";
                    }
                    MensagemPagina.MostraMensagem_Erro(mensagem);
                }
                //LeitorQuagga1.AbrirCamera();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        void dtgVolumesItensDataBind()
        {
            dtgVolumesItens.DataSource = ls_VolumesItens;
            dtgVolumesItens.DataBind();
        }

        protected void dtgvEnvios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                System.Web.UI.WebControls.Image img = (System.Web.UI.WebControls.Image)e.Row.FindControl("imgProduto");

                TextBox txtIdProduto = (TextBox)e.Row.FindControl("txtIdProduto");
                if (txtIdProduto != null)
                {
                    CarregaImgProduto(txtIdProduto.Text, img);
                }
            }
        }

        protected void GerarExcelEnvio_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string idEnvioOPI = btn.CommandArgument;
            EXC_ENVIO(idEnvioOPI);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenmodalEnvio", "$('#modalEnvio').modal('hide');", true);
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

                // Executar a consulta
                dsEnvios = BD.ExecutarDataSet("sp_Manipula_FLow_WMS_OPI", vParametrosEnvios);

                if (dsEnvios.Tables.Count == 0 || dsEnvios.Tables[0].Rows.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Aviso("Não foi encontrado dados nesse envio");
                    return;
                }

                DataSet dsVolumes = new DataSet();

                Dictionary<string, string> vParametrosVolumes = new Dictionary<string, string>();
                vParametrosVolumes.Add("@sFuncao", "CONSULTAR-VOLUMES-EXCEL");
                vParametrosVolumes.Add("@idEnvioOPI", idEnvio);

                // Executar a consulta
                dsVolumes = BD.ExecutarDataSet("sp_Manipula_FLow_WMS_OPI", vParametrosVolumes);

                if (dsVolumes.Tables.Count == 0 || dsVolumes.Tables[0].Rows.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Aviso("Não foi encontrado dados nesse envio");
                    return;
                }

                DataSet dsFotosVolumes = new DataSet();

                Dictionary<string, string> vParametrosFotos = new Dictionary<string, string>();
                vParametrosFotos.Add("@sFuncao", "CONSULTAR-IMAGEM-VOLUME");
                vParametrosFotos.Add("@idEnvioOPI", idEnvio);

                // Executar a consulta
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

                Pesquisar(hddidOPI.Value);
                FUNCOES.DownloadArquivo(Page, sNomeArquivo);
                MensagemPagina.MostraMensagem_Sucesso("Excel do Envio gerado com sucesso!");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao gerar o Excel do Envio! </br>" + ex.Message);
            }
        }

        protected void cmdGerarVolume_Click(object sender, EventArgs e)
        {
            if (Atualizar_VolumesItensClasse() && ValidarDadosVolumes())
            {
                string sErro = "";
                try
                {
                    string[] vidOPI = hddidOPI.Value.Split(',');
                    string idOPI = vidOPI[0].ToString();
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR-VOLUME");

                    hddidOPI.Value = idOPI;
                    vParametros.Add("@idOPI", hddidOPI.Value);
                    vParametros.Add("@idProdutoEmbalagem", ddlEmbalagem.SelectedValue);
                    vParametros.Add("@sDscVolume", "Volume");
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    //vParametros.Add("@sUnitizado", ddlsUnitizado.SelectedValue);

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {

                        hddidVolume.Value = RETORNO.DATASET(dsSalvar, 0, "idVolume");

                        VolumesItens_Salvar(hddidVolume.Value);
                        PopularVolumes();
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso", false);
                        LimpaCamposVolumes();
                        cmdGerarVolume.Visible = false;
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message, false);
                }
            }
            LeitorQuagga1.AbrirCamera();
            div_botoesVolumes.Visible = true;
        }

        protected void cmdEfetuarEnvio_Click(object sender, EventArgs e)
        {
            Gerar_Envio();
            DIV_Separacao.Visible = true;
            //DIV_CodigoEAN.Attributes["class"] += " visible";
            DIV_separacaoitens.Visible = false;
            DIV_UNITIZADOS_Modal.Visible = false;
            if (divCodigoB.Visible == true)
            {
                DivBipadorVolume.Visible = true;
                LeitorQuagga1.AbrirCamera();
            }
            else
                DivBipadorVolume.Visible = false;

            if (DIV8.Visible == true)
            {
                DIV8.Visible = false;
                DIV10.Visible = true;
                DivBipadorVolume.Visible = false;
            }
            DIV10.Visible = true;
            divTableEnvios.Visible = true;
            Pesquisar(hddidOPI.Value);
            DIV_ENVIOS.Visible = true;

            updModal.Update();
        }

        private int contadorVolumes()
        {
            int quantidadeMarcada = 0;

            foreach (GridViewRow row in dtgVolumes.Rows)
            {
                CheckBox chkOpcaoItemVolume = (CheckBox)row.FindControl("chkOpcaoItemVolume");

                if (chkOpcaoItemVolume.Checked)
                {
                    quantidadeMarcada++;
                }
            }
            return quantidadeMarcada;
        }

        private bool ValidarDadosEnvios()
        {
            bool bRetorno = true;
            string sMensagemErro = "";


            if (string.IsNullOrEmpty(txtDtEnvio.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Data";
            }
            if (string.IsNullOrEmpty(txtNPesoLiquido.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Peso Líquido";
            }
            if (string.IsNullOrEmpty(txtNPesoBruto.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Peso do Produto";
            }
            if (string.IsNullOrEmpty(txtNPesoBruto.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Peso Bruto";
            }
            if (string.IsNullOrEmpty(txtNComprimento.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Comprimento";
            }
            if (string.IsNullOrEmpty(txtNLargura.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a Largura";
            }
            if (string.IsNullOrEmpty(txtNAltura.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a Altura";
            }
            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro, false);
            }

            return bRetorno;
        }

        void Gerar_Envio()
        {
            string sErro = "";
            int qtdVolumes = contadorVolumes();
            if (ValidarDadosEnvios() && qtdVolumes != 0)
            {
                try
                {
                    string[] vidOPI = hddidOPI.Value.Split(',');
                    string idOPI = vidOPI[0].ToString();
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "GERAR-ENVIO");

                    hddidOPI.Value = idOPI;
                    vParametros.Add("@idOPI", hddidOPI.Value);
                    vParametros.Add("@nQtdVolumes", qtdVolumes.ToString());
                    if (txtDtEnvio.Text == "")
                    {
                        vParametros.Add("@dtEnvio", txtDtEnvio.Text);
                    }
                    else
                    {
                        DateTime dt = DateTime.Parse(txtDtEnvio.Text.ToString(), CultureInfo.InvariantCulture);
                        vParametros.Add("@dtEnvio", dt.ToString());
                    }
                    vParametros.Add("@idUsuarioEnvio", IDENTITY.Variaveis.idUsuario());

                    vParametros.Add("@nPesoLiquido", BD.Conversoes.Numerico(txtNPesoLiquido));
                    vParametros.Add("@nPesoBruto", BD.Conversoes.Numerico(txtNPesoBruto));
                    vParametros.Add("@nComprimento", BD.Conversoes.Numerico(txtNComprimento));
                    vParametros.Add("@nLargura", BD.Conversoes.Numerico(txtNLargura));
                    vParametros.Add("@nAltura", BD.Conversoes.Numerico(txtNAltura));
                    //vParametros.Add("@nQtdVolumes", BD.Conversoes.Numerico(txtNQtdVolumes));

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {

                        hddidEnvioOPI.Value = RETORNO.DATASET(dsSalvar, 0, "idEnvioOPI");

                        if (EnvioVolumes_Salvar(hddidEnvioOPI.Value))
                        {
                            Pesquisar(idOPI);
                            LimparCamposVolumes();
                            MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso", false);
                            Session["MensagemSucesso"] = "Registro gravado com sucesso";

                            if (hddidStatus.Value != "4")
                                Alterar_Status(hddidOPI.Value, "3");

                            //Response.Redirect(Request.RawUrl);
                        }
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message, false);
                }
            }
            else if (qtdVolumes == 0)
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um Volume!");
            }
        }

        void Alterar_Status(string idOPI, string idStatus)
        {
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "INCLUIR-STATUS-ENVIO");
            vParametros.Add("@idOPI", idOPI);
            vParametros.Add("@idStatus", idStatus);
            DataSet dsStatus = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsStatus))
            {
                hddidStatus.Value = RETORNO.DATASET(dsStatus, 0, "idStatus");
            }
        }

        void LimparCamposVolumes()
        {
            var dt = DateTime.Now;
            txtDtEnvio.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
            ddlEmbalagem.SelectedValue = "0";
            txtNPesoLiquido.Text = "";
            txtNPesoBruto.Text = "";
            txtNComprimento.Text = "";
            txtNLargura.Text = "";
            txtNAltura.Text = "";
        }

        bool EnvioVolumes_Salvar(string idEnvio)
        {
            bool validado = false;
            string sErro;
            try
            {
                foreach (GridViewRow row in dtgVolumes.Rows)
                {
                    CheckBox chkOpcaoItemVolume = (CheckBox)row.FindControl("chkOpcaoItemVolume");

                    if (chkOpcaoItemVolume.Checked)
                    {
                        Dictionary<String, String> vParametros = new Dictionary<string, string>();
                        string idVolume = dtgVolumes.DataKeys[row.RowIndex].Value.ToString();
                        vParametros.Add("@sFuncao", "INCLUIR-ITENS-ENVIO");
                        vParametros.Add("@idEnvioOPI", idEnvio);
                        vParametros.Add("@idVolume", idVolume);
                        vParametros.Add("@sDscOPI", hddsDscOPI.Value);
                        vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                        vParametros.Add("@idPedido", hddidPedido.Value);
                        if (txtDtEnvio.Text == "")
                        {
                            vParametros.Add("@dtEnvio", txtDtEnvio.Text);
                        }
                        else
                        {
                            DateTime dt = DateTime.Parse(txtDtEnvio.Text.ToString(), CultureInfo.InvariantCulture);
                            vParametros.Add("@dtEnvio", dt.ToString());
                        }

                        DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);
                        if (BD.ValidarDataSet(dsSalvar, out sErro))
                        {
                            //hddidMovimentacao.Value = RETORNO.DATASET(dsSalvar, "idMovimentacao");
                            validado = true;
                        }
                        else
                        {
                            validado = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message, false);
                return false;
            }

            return validado;
        }

        bool Atualizar_VolumesItensClasse()
        {
            for (int i = 0; i < dtgVolumesItens.Rows.Count; i++)
            {
                GridViewRow item = dtgVolumesItens.Rows[i];

                if (item.RowType == DataControlRowType.DataRow)
                {
                    try
                    {
                        TextBox txtnVolumeGV = (TextBox)item.FindControl("txtnVolumeGV");
                        if (Convert.ToInt32(txtnVolumeGV.Text) > 0)
                        {
                            ls_VolumesItens[i].NVolume = Convert.ToInt32(txtnVolumeGV.Text);
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem_Aviso("Lembrete: A Ordem Foi Adicionada Como Zero.", false);
                            return true;
                        }
                    }
                    catch
                    {
                        MensagemPagina.MostraMensagem_Erro("Dados Incorretos na Grid", false);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ValidarDadosVolumes()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlEmbalagem.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Embalagem para o Volume!";
            }
            if (ls_VolumesItens.Count == 0)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Adicione Algum Item ao Volume!";
            }
            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro, false);
            }

            return bRetorno;
        }

        protected void VolumesItens_Salvar(string id)
        {
            if (ls_VolumesItens.Count > 0)
            {
                DataSet dsVolumesItens = new DataSet();
                Dictionary<string, string> vParametrosItens = new Dictionary<string, string>();
                vParametrosItens.Add("@sFuncao", "INCLUIR-VOLUMES-ITENS");
                ls_VolumesItens.ForEach(a =>
                {
                    vParametrosItens["@idVolume"] = id;
                    vParametrosItens["@idObjeto"] = a.IdObjeto.ToString();
                    vParametrosItens["@nVolume"] = a.NVolume.ToString();
                    vParametrosItens["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario().ToString();
                    vParametrosItens["@sCodigoBarras"] = a.SCodigoBarras;
                    vParametrosItens["@sDscObjeto"] = a.SDscObjeto;

                    dsVolumesItens = BD.ExecutarDataSet(sProcedure, vParametrosItens);


                    DataSet dsProduto_SEPARACAO = new DataSet();
                    Dictionary<string, string> vParametros_SEPARACAO = new Dictionary<string, string>();
                    vParametros_SEPARACAO.Add("@sFuncao", "SEPARACAO_ITEM");
                    vParametros_SEPARACAO.Add("@sSeparacao", "S");
                    vParametros_SEPARACAO.Add("@sCodigoEAN", a.SCodigoBarras);
                    vParametros_SEPARACAO.Add("@idOPI", hddidOPI.Value);
                    dsProduto_SEPARACAO = BD.ExecutarDataSet(sProcedure, vParametros_SEPARACAO);
                });

                if (BD.ValidarDataSet(dsVolumesItens))
                {
                    ls_VolumesItens.Clear();
                    dtgVolumesItensDataBind();
                    txtsCodigoBarrasVolume.Text = "";
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Erro Ao Salvar");
                }
            }
            else
            {
                MensagemPagina.MostraMensagem_Aviso("O Unitizado foi Salvo sem Itens!", false);
            }
        }

        void LimpaCamposVolumes()
        {
            txtsCodigoBarrasVolume.Text = "";
            ddlEmbalagem.SelectedValue = "0";
        }

        void PopularVolumes()
        {
            DataSet dsVolumes = new DataSet();
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-VOLUMES-OPCOES");
            vParametros.Add("@idOPI", hddidOPI.Value);
            dsVolumes = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsVolumes))
            {
                div_VolumesSalvos.Visible = true;
                dtgVolumesDataBind(dsVolumes);
            }
            else
            {
                div_VolumesSalvos.Visible = false;
            }

        }

        void dtgVolumesDataBind(DataSet dsVolumes)
        {
            dtgVolumes.DataSource = dsVolumes;
            dtgVolumes.DataBind();
        }

        protected void chkOpcaoItemVolume_CheckedChanged(object sender, EventArgs e)
        {
            RegistraScriptCamposPeso();
            CheckBox chk = (CheckBox)sender;

            if (chk.Checked)
            {
                Div_FormEnvios.Visible = true;
                div_botoesVolumes.Visible = true;
                cmdEfetuarEnvio.Visible = true;
                cmdGerarVolume.Visible = false;
                var dt = DateTime.Now;
                txtDtEnvio.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
            }
            if (!chk.Checked)
            {
                foreach (GridViewRow row in dtgVolumes.Rows)
                {
                    CheckBox chkItens = row.FindControl("chkOpcaoItemVolume") as CheckBox;
                    if (chkItens != null && chkItens.Checked)
                    {
                        if (chkItens.Checked)
                        {
                            Div_FormEnvios.Visible = true;
                            cmdEfetuarEnvio.Visible = true;
                            break;
                        }
                    }
                    else
                    {
                        Div_FormEnvios.Visible = false;
                        cmdEfetuarEnvio.Visible = false;
                    }
                }
            }

            if (dtgVolumesItens.Rows.Count > 0)
            {
                cmdGerarVolume.Visible = true;
            }
            LeitorQuagga1.AbrirCamera();
            if (DIV_CodigoEAN.Visible != true)
                DivBipadorVolume.Visible = true;
        }

        protected void cmdAbrirArquivos_Click(object sender, EventArgs e)
        {
            LinkButton cmdArquivo = (LinkButton)sender;
            string idVolume = cmdArquivo.CommandArgument;

            ViewArquivos2.Visible = true;
            Popular_aba_ArquivoVolume(idVolume);
        }

        void Popular_aba_ArquivoVolume(string idVolume)
        {
            div_botoesVolumes.Visible = false;
            frmArquivos2.Attributes.Add("src", $"~/app/Paginas/Arquivos.aspx?idObjeto={idVolume}&sTipoObjeto={"Volume"}");
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

        protected void dtgVolumes_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void dtgVolumesItensGV_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView dtgVolumesItensGV = (GridView)sender;
                LinkButton btnToggle = (LinkButton)e.Row.FindControl("btnToggle");

                if (dtgVolumesItensGV != null)
                {
                    string idObjeto = dtgVolumesItensGV.DataKeys[e.Row.RowIndex].Value.ToString();

                    var dtgItensVL = (GridView)e.Row.FindControl("dtgItensVL");
                    ConsultaItensVL(idObjeto);
                    dtgItensVL.DataSource = ls_VolumesProdutosGV;
                    dtgItensVL.DataBind();
                }
            }
        }

        protected void dtgVolumeItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView dtgVolumesItensGV = (GridView)sender;
                LinkButton btnToggle = (LinkButton)e.Row.FindControl("btnToggle");

                if (dtgVolumesItensGV != null)
                {
                    string idObjeto = dtgVolumesItensGV.DataKeys[e.Row.RowIndex].Value.ToString();

                    var dtgItensVL = (GridView)e.Row.FindControl("dtgItensVL");
                    ConsultaItensVL(idObjeto);
                    dtgItensVL.DataSource = ls_VolumesProdutosGV;
                    dtgItensVL.DataBind();
                }
            }
        }

        void ConsultaItensVL(string id)
        {
            string sFuncao = "CONSULTAR-UNITIZADOS-ITENS";
            DataSet dsProdutos;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idUnitizado", id);

            dsProdutos = BD.ExecutarDataSet(sProcedure, vParametros, false);

            ls_VolumesProdutosGV = conversorProdutos.ConverterDataSet(dsProdutos, "Table");
        }

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

        private DataSet ds;
        void Popular_Itens(DataSet ds)
        {
            this.ds = ds;
        }

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

        protected void btnEnvios_Click(object sender, EventArgs e)
        {
            RegistraScript();
            DIV_UNITIZADOS_Modal.Visible = true;
            DIV_ENVIOS.Visible = false;
            DIV_Separacao.Visible = true;
            //DIV_CodigoEAN.Attributes["class"] += " visible";
            DIV_separacaoitens.Visible = false;
            DivBipadorVolume.Visible = false;
            if (DIV_UnitizadosItens.Visible == true)
            {
                DIV_UnitizadosItens.Visible = false;
                DIV_IdEmbalagem.Visible = true;
                DIV_sDscUnitizado.Visible = true;
                DIV_ProximoUnitizado.Visible = true;
            }
        }

        protected void cmdEnvios_Click(object sender, EventArgs e)
        {
            DIV_Separacao.Visible = true;
            //DIV_CodigoEAN.Attributes["class"] += " visible";
            DIV_separacaoitens.Visible = false;
            DIV_UNITIZADOS_Modal.Visible = false;
            DIV_ENVIOS.Visible = true;
            if (divCodigoB.Visible == true)
            {
                DivBipadorVolume.Visible = true;
                LeitorQuagga1.AbrirCamera();
            }
            else
                DivBipadorVolume.Visible = false;

            if (DIV8.Visible == true)
            {
                DIV8.Visible = false;
                DIV10.Visible = true;
                DivBipadorVolume.Visible = false;
            }
            DIV10.Visible = true;
        }

        protected void cmdVoltarModal_Click(object sender, EventArgs e)
        {
            DIV_Separacao.Visible = true;
            //DIV_CodigoEAN.Attributes["class"] += " visible";
            DIV_separacaoitens.Visible = false;
            DIV_UNITIZADOS_Modal.Visible = false;
            DIV_ENVIOS.Visible = true;
            if (divCodigoB.Visible == true)
            {
                DivBipadorVolume.Visible = true;
                LeitorQuagga1.AbrirCamera();
            }
            else
                DivBipadorVolume.Visible = false;

            if (DIV8.Visible == true)
            {
                DIV8.Visible = false;
                DIV10.Visible = true;
                DivBipadorVolume.Visible = false;
            }
            if (dtgVolumesItens.Rows.Count > 0)
            {
                div_botoesVolumes.Visible = true;
                cmdGerarVolume.Visible = true;
            }
        }

        protected void btnVisualizarEnvios_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalEnvio').modal('show');", true);
        }
        #endregion

        #region | Historico
        protected void gvHistorico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }
        #endregion

        protected void gv_Separados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        protected void btnAbrirUnit_Click(object sender, EventArgs e)
        {
            DivBipadorVolume.Visible = true;
            DIV_CodigoEAN.Visible = true;
            DivManual.Visible = true;
            txtsCodigoEAN.Visible = true;
            cmdIncluirUnitizadoItem.Visible = true;
            LeitorQuagga1.txtClient = $"{txtsCodigoEAN.ClientID}";
            LeitorQuagga1.click = $"{cmdIncluirUnitizadoItem.ClientID}";
            LeitorQuagga1.RegisterQuaggaLibrary();
            LeitorQuagga1.RegistraQuaggaScript();
            LeitorQuagga1.AbrirCamera();
            UpdatePanel2.Update();
            INcluirItem = "U";
            DIV_CodigoEAN.Attributes["class"] += " visible";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "$('#modalUnitizado').modal('show');", true);
        }
    }
}