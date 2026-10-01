using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.IO;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.tool.xml;
using Image = iTextSharp.text.Image;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using Document = iTextSharp.text.Document;
using Element = iTextSharp.text.Element;
using iTextSharp.tool.xml.html;



namespace TT_Flow.App.Paginas.Qualidade.Manutencao
{
    public partial class InstrucaoTecnica_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Instrução Técnica";
        string sProcedure = "sp_Manipula_tbl_Flow_Qualidade_IT";
        static int contador = 1;

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();


            if (!IsPostBack)
            {
                //PopularCombos();


                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Qualidade.Manutencao.InstrucaoTecnica.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Qualidade.Manutencao.InstrucaoTecnica.Incluir, true);
                    Pesquisar("0", true);

                }
                if (Session["PDFSucesso"] != null && (bool)Session["PDFSucesso"])
                {
                    MensagemPagina.MostraMensagem_Sucesso("PDF gerado com sucesso!");
                    Session["PDFSucesso"] = false;
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
                else if (requestTarget == "funcao_SALVAR")
                {
                    Salvar_InstrucaoTecnica();
                }
                else
                {
                    Pesquisar(hddidIT.Value, true);
                }
            }

            var toolbar = txtDscEditor.FindControl("ctl01") as WebControl;

            if (toolbar != null)
            {
                var buttonToRemove = toolbar.FindControl("ctl40");

                if (buttonToRemove != null)
                {
                    toolbar.Controls.Remove(buttonToRemove);
                }
            }

            RegistraScript("");

        }


        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idIT, bool bEdicao)
        {
            //PopularCombos();

            //cmdEditar.Visible = false;
            cmdGerarPDF.Visible = false;
            aba_Historico.Visible = false;
            aba_Arquivo.Visible = false;

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idIT != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idIT", idIT);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidIT.Value = RETORNO.DATASET(dsPesquisa, 0, "idIT");
                        txtidIT.Text = RETORNO.DATASET(dsPesquisa, 0, "idIT");
                        txtdtInclusao.Text = RETORNO.DATASET(dsPesquisa, 0, "dtInclusao");
                        txtsTituloIT.Text = RETORNO.DATASET(dsPesquisa, 0, "sTituloIT");
                        txtDscEditor.Content = RETORNO.DATASET(dsPesquisa, 0, "sDscIT");
                        txtnRevisao.Text = RETORNO.DATASET(dsPesquisa, 0, "nRevisao");
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        lblTituloPagina.Text = string.Format("Instrução Técnica {0}", RETORNO.DATASET(dsPesquisa, 0, "sTituloIT"));
                        BreadCrumb.TitulodaPagina = string.Format("Instrução Técnica {0}", RETORNO.DATASET(dsPesquisa, 0, "sTituloIT"));

                        lblTituloSalvar.Text = "Confirma a Alteração da " + lblTituloPagina.Text + "?";
                        //lblTituloEditar.Text = "Deseja editar a " + lblTituloPagina.Text + "?";

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Qualidade.Manutencao.InstrucaoTecnica.Alterar);
                        cmdGerarPDF.Visible = true;

                        Popular_Aba_Historico(dsPesquisa);
                        Popular_Documentos(RETORNO.DATASET(dsPesquisa, 0, "idIT"));


                        if (!bEdicao)
                        {
                            //Popular_Aba_Documentos(idColaborador);

                            //if (!FUNCOES.ValidaPermissao(Permissao.Adm.Alterar))
                            //{
                            //    cmdEditar.Visible = false;
                            //    cmdSalvar.Visible = false;
                            //}
                        }

                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    BreadCrumb.TitulodaPagina = string.Format("Novo {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    txtidIT.Text = "Novo";
                    divdtInclusao.Visible = false;
                    txtnRevisao.Text = "1";
                    lblTituloSalvar.Text = "Confirma a Inclusão da Instrução Técnica?";
                    cmdSalvar.Text = "Incluir";
                    //txtdtInclusao.Focus();


                }
                RegistraScript("");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }


        void Salvar_InstrucaoTecnica()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidIT = hddidIT.Value.Split(',');
                    string idIT = vidIT[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idIT", idIT);
                    vParametros.Add("@dtInclusao", txtdtInclusao.Text);
                    vParametros.Add("@nRevisao", (Int32.Parse(txtnRevisao.Text) + 1).ToString());
                    vParametros.Add("@sTituloIT", txtsTituloIT.Text);
                    vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@sDscIT", txtDscEditor.Content);

                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idIT = RETORNO.DATASET(dsSalvar, "idIT");
                        Pesquisar(idIT, false);
                        string sUrl = string.Format("/app/Paginas/Qualidade/Manutencao/InstrucaoTecnica_Detalhe.aspx");
                        //MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                        MensagemPagina.MostraMensagem_Sucesso(string.Format("{0} gravada com sucesso!  </br>", sTituloPagina));

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

            RegistraScript("");
        }

        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidIT.Value = "0";
            txtdtInclusao.Text = "";
            txtsTituloIT.Text = "";
            txtnRevisao.Text = "0";
            txtDscEditor.Content = "";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;

        }

        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtsTituloIT.Text.Length < 1)
            {
                sMensagemErro = "Título Invalido!";
            }

            if (txtDscEditor.Content.Length < 5)
            {
                sMensagemErro = "Descrição inválida!";
            }


            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }


            return bRetorno;
        }
        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //Mensagens de Confirmação
            sb.Append("$v192(function() {");

            sb.Append("$v192(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Editar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Editar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=cmdEditar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Editar').dialog('open');");
            sb.Append("});");


            sb.Append("});");
            //Caixa de seleção de datas
            //sb.Append("$(function() {$('[id*=txtdtRequisicao]').datepicker({");
            //sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }
        #endregion

        #region | Combos/DDL
        //void PopularCombos()
        //{
        //}
        #endregion

        #region | Aba Historico
        void Popular_Aba_Historico(DataSet ds)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(gv_Historico, ds.Tables[1], 1, "desc"), true);
            aba_Historico.Visible = true;
        }

        protected void gv_Historico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[4].Text = HttpUtility.HtmlDecode(e.Row.Cells[4].Text);
        }

        #endregion

        #region | Aba Documentos 

        void Popular_Documentos(string idIT)
        {
            frmArquivos.Attributes.Add("src", string.Format("~/App/Paginas/Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idIT, "InstrucaoTecnica"));
            frmArquivos.Visible = true;
            DIV_Arquivos.Visible = true;
            aba_Arquivo.Visible = true;
        }


        #endregion        

        #region | Gera PDF 
        private void GeraPDF()
        {

            Document doc = new Document(PageSize.A4);
            doc.SetMargins(50, 50, 125, 100);
            DataSet ds;
            Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>();
            vParametrosProduct.Add("@sFuncao", "CONSULTAR_DETALHE");
            vParametrosProduct.Add("@idIT", hddidIT.Value);

            ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Qualidade_IT", vParametrosProduct);
            string sDscIT = RETORNO.DATASET(ds, 0, "sDscIT");
            string sTituloIT = RETORNO.DATASET(ds, 0, "sTituloIT");
            string sDocumento = "PR_" + contador;
            int nRevisao = int.Parse(RETORNO.DATASET(ds, 0, "nRevisao"));
            DateTime dtInclusao = DateTime.Parse(RETORNO.DATASET(ds, 0, "dtInclusao"));


            string sDataHora = FUNCOES.CarimboDataHora();

            string sNomeArquivo = sDocumento + "_Rev" + nRevisao + "_" + Regex.Replace(sTituloIT, "[^a-zA-Z0-9 ]", "") + "_" + sDataHora + ".pdf";
            string sCaminho = Server.MapPath("~/App/Download/") + sNomeArquivo;

            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(sCaminho, FileMode.Create));

            writer.PageEvent = new Eventos(sTituloIT, sDocumento, nRevisao, dtInclusao, hddidIT.Value);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(sDscIT);
            
            doc.Open();

            doc.NewPage();

            foreach (HtmlNode node in htmlDoc.DocumentNode.ChildNodes)
            {
                if (node.Name == "#text" || !node.InnerHtml.Contains("<img") && !string.IsNullOrEmpty(node.InnerText))
                {
                    if (node.ChildNodes.Any(n => n.NodeType == HtmlNodeType.Element))
                    {
                        try
                        {
                            string sText1 = node.OuterHtml;
                            using (StringReader sr = new StringReader(sText1.Replace("&gt;", ">").Replace("&lt;", "<")))
                            {
                                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);
                            }
                        }


                        catch (Exception)
                        {
                            string sText = node.OuterHtml;
                            string sPadrao = @"<br\s*/?\s*(?!/>)";
                            sText = Regex.Replace(sText, sPadrao, "<br /");
                            using (StringReader sr = new StringReader(sText.Replace("<hr>", "<hr/>")))
                            {
                                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);
                            }
                        }                        
                    }
                    else
                    {
                        string text = node.InnerText.Replace("&gt;", ">").Replace("&lt;", "<");
                        Paragraph paragraph = new Paragraph(text);
                        doc.Add(paragraph);
                    }
                }
                else if (node.InnerHtml.Contains("<img"))
                {

                    string[] parts = Regex.Split(node.OuterHtml, @"(<img[^>]+>)", RegexOptions.IgnoreCase);
                    if (parts.Length >= 2)
                    {
                        using (StringReader sr = new StringReader(parts[0].Replace("<br>", "<br/>").Replace("<hr>", "<hr/>").Replace("&gt;", ">").Replace("&lt;", "<")))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

                        }


                        var match = Regex.Match(parts[1].Replace("alt=\"\"", "alt=\"\" /").Replace("<br>", "<br/>"), @"src=""(.*?)""", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            string srcValue = match.Groups[1].Value;
                            var base64Content = Regex.Match(srcValue, @"data:image\/[a-zA-Z]+;base64,(.*)").Groups[1].Value;
                            byte[] imageBytes = Convert.FromBase64String(base64Content);
                            Image image = Image.GetInstance(imageBytes);
                            image.Alignment = Image.ALIGN_CENTER;
                            float width = 200;
                            float height = image.Height * (width / image.Width);
                            image.ScaleToFit(width, height);
                            doc.Add(image);
                        }

                    }
                }

                else if (node.Name == "img" || node.InnerHtml.StartsWith("<img"))
                {
                    // Processa a imagem

                    var match = Regex.Match(node.OuterHtml, @"src=""(.*?)""", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        string srcValue = match.Groups[1].Value;
                        var base64Content = Regex.Match(srcValue, @"data:image\/[a-zA-Z]+;base64,(.*)").Groups[1].Value;
                        byte[] imageBytes = Convert.FromBase64String(base64Content);
                        Image image = Image.GetInstance(imageBytes);
                        image.Alignment = Image.ALIGN_CENTER;
                        float width = 200;
                        float height = image.Height * (width / image.Width);
                        image.ScaleToFit(width, height);
                        doc.Add(image);
                    }

                }
            }


            doc.Close();
            contador++;
            FUNCOES.DownloadArquivo(Page, sNomeArquivo);
            MensagemPagina.MostraMensagem_Sucesso("PDF gerado com sucesso!");

        }


        protected void cmdGeraPDF_Click(object sender, EventArgs e)
        {
            GeraPDF();
        }
        #endregion


    }

    class Eventos : PdfPageEventHelper
    {
        readonly string sTitulo;
        readonly string sSiglaDocumento;
        readonly int nRev;
        readonly DateTime dtDocumento;
        public PdfContentByte cb;
        public PdfTemplate template;
        readonly string sIdIT;

        public Eventos(string sTituloIT, string sDocumento, int nRevisao, DateTime dtInclusao, string id)
        {
            sTitulo = sTituloIT;
            sSiglaDocumento = sDocumento;
            nRev = nRevisao;
            dtDocumento = dtInclusao;
            sIdIT = id;
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);
            cb = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);
        }

        public override void OnStartPage(PdfWriter writer, Document doc)
        {
            Paragraph espaco = new Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 12));
            Paragraph espaco2 = new Paragraph(" ", new Font(Font.FontFamily.HELVETICA, 12));
            doc.Add(espaco);
            doc.Add(espaco2);
        }

        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            base.OnEndPage(writer, doc);

            if (writer.PageNumber == 1)
            {
                DataSet ds;
                Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>();
                vParametrosProduct.Add("@sFuncao", "CONSULTAR_DETALHE");
                vParametrosProduct.Add("@idIT", sIdIT);
                ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Qualidade_IT", vParametrosProduct);


                // Criação do cabeçalho aqui dentro
                PdfPTable cabecalho = new PdfPTable(4)
                {
                    TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin,
                    WidthPercentage = 20
                };
                cabecalho.SetWidths(new float[] { 100f, 180f, 60f, 70f });

                // Coluna 1: Imagem no canto esquerdo
                string caminhoImagem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App", "img", "instrucaoTecnica_pdf.jpg");
                Image imagem = Image.GetInstance(caminhoImagem);
                imagem.ScaleAbsolute(10, 20);
                imagem.BorderWidthBottom = 0.4f;
                imagem.BorderWidthTop = 0.4f;

                PdfPCell celulaImagem = new PdfPCell(imagem, true)
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    FixedHeight = 100,
                    BorderWidth = 1f,
                    BorderColor = BaseColor.BLACK
                };
                cabecalho.AddCell(celulaImagem);

                // Coluna 2: Título no centro
                PdfPCell celulaTitulo = new PdfPCell(new Phrase(sTitulo, FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.BOLD, BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 100,
                    BorderWidth = 1f,
                    BorderColor = BaseColor.BLACK
                };
                cabecalho.AddCell(celulaTitulo);

                // Colunas 3: Informações
                PdfPTable informacoes = new PdfPTable(1);
                PdfPCell celulaInformacoes = new PdfPCell(new Phrase("Documento:\n\n", FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
                celulaInformacoes.Border = PdfPCell.NO_BORDER;
                informacoes.AddCell(celulaInformacoes);

                celulaInformacoes = new PdfPCell(new Phrase("Data:\n\n", FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
                celulaInformacoes.Border = PdfPCell.NO_BORDER;
                informacoes.AddCell(celulaInformacoes);

                celulaInformacoes = new PdfPCell(new Phrase("Rev:\n\n", FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
                celulaInformacoes.Border = PdfPCell.NO_BORDER;
                informacoes.AddCell(celulaInformacoes);

                celulaInformacoes = new PdfPCell(new Phrase(string.Format("Página:"), FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
                celulaInformacoes.Border = PdfPCell.NO_BORDER;
                informacoes.AddCell(celulaInformacoes);

                PdfPCell celulaInformacoesGeral = new PdfPCell(informacoes)
                {
                    Colspan = 1,
                    FixedHeight = 100,
                    BorderWidth = 1f,
                    BorderColor = BaseColor.BLACK
                };
                cabecalho.AddCell(celulaInformacoesGeral);

                // Coluna 4: Valores
                PdfPTable valores = new PdfPTable(1);
                PdfPCell celulaValores = new PdfPCell(new Phrase(sSiglaDocumento + "\n\n", FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
                celulaValores.Border = PdfPCell.NO_BORDER;
                valores.AddCell(celulaValores);

                celulaValores = new PdfPCell(new Phrase(dtDocumento.ToString("dd/MM/yyyy" + "\n\n"), FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
                celulaValores.Border = PdfPCell.NO_BORDER;
                valores.AddCell(celulaValores);

                celulaValores = new PdfPCell(new Phrase(nRev + "\n\n", FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
                celulaValores.Border = PdfPCell.NO_BORDER;
                valores.AddCell(celulaValores);

                celulaValores = new PdfPCell(new Phrase(string.Format("", writer.PageNumber, writer.PageNumber), FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
                celulaValores.Border = PdfPCell.NO_BORDER;
                valores.AddCell(celulaValores);

                PdfPCell celulaValoresGeral = new PdfPCell(valores)
                {
                    Colspan = 1,
                    FixedHeight = 100,
                    BorderWidth = 1f,
                    BorderColor = BaseColor.BLACK
                };
                cabecalho.AddCell(celulaValoresGeral);

                //Coluna 1: Elaborado e Responsavel
                PdfPCell celula1 = new PdfPCell(new Phrase("Elaborado / Data: \n\n\n\n\n\n\n Responsável da Qualidade", FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.NORMAL, BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_TOP,
                    BorderWidth = 1f,
                    BorderColor = BaseColor.BLACK
                };
                cabecalho.AddCell(celula1);

                //Coluna 2: Vazia
                PdfPCell celula2 = new PdfPCell()
                {
                    BorderWidth = 1f,
                    BorderColor = BaseColor.BLACK
                };
                cabecalho.AddCell(celula2);

                //Coluna 3: Aprovado e Data              
                PdfPCell celula3 = new PdfPCell(new Phrase("Aprovado / Data: \n\n\n\n\n\n\n Gerente", FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.NORMAL, BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_TOP,
                    Colspan = 2,
                    BorderWidth = 1f,
                    BorderColor = BaseColor.BLACK
                };
                cabecalho.AddCell(celula3);

                cabecalho.WriteSelectedRows(0, -1, doc.LeftMargin, doc.PageSize.Height - doc.TopMargin + cabecalho.TotalHeight - 100, writer.DirectContent);

                //Criação tabela meio documento
                PdfPTable tabTitulo = new PdfPTable(1);
                PdfPCell celulaTituloCentral = new PdfPCell(new Phrase(sTitulo.ToUpper(), FontFactory.GetFont(FontFactory.HELVETICA, 17, Font.BOLD, BaseColor.GRAY)));
                celulaTituloCentral.HorizontalAlignment = Element.ALIGN_CENTER;
                celulaTituloCentral.BorderWidth = 0.5f;
                tabTitulo.AddCell(celulaTituloCentral);

                //Posição tabela Titulo                
                tabTitulo.TotalWidth = celulaTituloCentral.Width + 250;
                float xTitulo = (doc.PageSize.Width - doc.LeftMargin - doc.RightMargin) / 3;
                float yPosicaoDaTabelaTitulo = doc.BottomMargin + 200 + cabecalho.TotalHeight;
                tabTitulo.WriteSelectedRows(0, -1, xTitulo, yPosicaoDaTabelaTitulo, writer.DirectContent);


                //Criação tabela Historico
                PdfPTable tabHistorico = new PdfPTable(3);
                tabHistorico.TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
                tabHistorico.SetWidths(new float[] { 50f, 50f, 70f });

                PdfPCell celulaTituloHistorico = new PdfPCell(new Phrase("RESUMO DE MODIFICAÇÕES DO DOCUMENTO", FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.NORMAL, BaseColor.BLACK)))
                {
                    Colspan = 3,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                tabHistorico.AddCell(celulaTituloHistorico);

                //Coluna 1 tabela Historico 
                PdfPCell celulaHist1 = new PdfPCell(new Phrase("REVISÃO", FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.NORMAL, BaseColor.BLACK)));
                celulaHist1.HorizontalAlignment = Element.ALIGN_CENTER;
                tabHistorico.AddCell(celulaHist1);

                //Coluna 2 tabela Historico
                PdfPCell celulaHist2 = new PdfPCell(new Phrase("DATA", FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.NORMAL, BaseColor.BLACK)));
                celulaHist2.HorizontalAlignment = Element.ALIGN_CENTER;
                tabHistorico.AddCell(celulaHist2);

                //Coluna 3 tabela Historico
                PdfPCell celulaHist3 = new PdfPCell(new Phrase("USUÁRIO", FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.NORMAL, BaseColor.BLACK)));
                celulaHist3.HorizontalAlignment = Element.ALIGN_CENTER;
                tabHistorico.AddCell(celulaHist3);

                foreach (DataRow item in ds.Tables[2].Rows)
                {
                    PdfPCell celHist11 = new PdfPCell(new Phrase(item["nRevisao"].ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.NORMAL, BaseColor.BLACK)));
                    celHist11.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabHistorico.AddCell(celHist11);

                    PdfPCell celHist2 = new PdfPCell(new Phrase(item.Field<DateTime>("dtAtualizacao").ToString("dd/MM/yyyy"), FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.NORMAL, BaseColor.BLACK)));
                    celHist2.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabHistorico.AddCell(celHist2);

                    PdfPCell celHist3 = new PdfPCell(new Phrase(item.Field<string>("sDscUsuarioAtualizacao"), FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.NORMAL, BaseColor.BLACK)));
                    celHist3.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabHistorico.AddCell(celHist3);
                }


                //Posição tabela Historico                 
                decimal fator = ((decimal)nRev / 10);
                decimal tamanhoTabela;
                if (fator < 1)
                {
                    tamanhoTabela = 100;
                }
                else if (fator >= 1 && fator <= 2)
                {
                    tamanhoTabela = fator * 130;
                }
                else
                {

                    tamanhoTabela = fator * 105;

                }


                float yPosicaoDaTabela = doc.BottomMargin + (float)tamanhoTabela;
                tabHistorico.WriteSelectedRows(0, -1, doc.LeftMargin, yPosicaoDaTabela, writer.DirectContent);

                Rodape(writer, doc);
            }
            else
            {
                Cabecalho(writer, doc);
                Rodape(writer, doc);
            }
        }

        public override void OnCloseDocument(PdfWriter writer, Document doc)
        {
            base.OnCloseDocument(writer, doc);
            //Incluir numero total de paginas
            template.BeginText();
            template.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 9);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber));
            template.EndText();

        }

        private void Cabecalho(PdfWriter writer, Document doc)
        {
            // Criação do cabeçalho aqui dentro
            PdfPTable cabecalho = new PdfPTable(4)
            {
                TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin,
                WidthPercentage = 20
            };
            cabecalho.SetWidths(new float[] { 100f, 180f, 60f, 70f });

            // Coluna 1: Imagem no canto esquerdo
            string caminhoImagem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App", "img", "instrucaoTecnica_pdf.jpg");
            Image imagem = Image.GetInstance(caminhoImagem);
            float desiredWidth = 1;
            float desiredHeight = imagem.Height * (desiredWidth / imagem.Width);
            imagem.ScaleAbsolute(desiredWidth, desiredHeight - 50);
            imagem.BorderWidthBottom = 0.4f;
            imagem.BorderWidthTop = 0.4f;

            PdfPCell celulaImagem = new PdfPCell(imagem, true)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                FixedHeight = 100,
                BorderWidth = 1f,
                BorderColor = BaseColor.BLACK
            };
            cabecalho.AddCell(celulaImagem);

            // Coluna 2: Título no centro
            PdfPCell celulaTitulo = new PdfPCell(new Phrase(sTitulo, FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.BOLD, BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                FixedHeight = 100,
                BorderWidth = 1f,
                BorderColor = BaseColor.BLACK
            };
            cabecalho.AddCell(celulaTitulo);

            // Colunas 3: Informações
            PdfPTable informacoes = new PdfPTable(1);
            PdfPCell celulaInformacoes = new PdfPCell(new Phrase("Documento:\n\n", FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
            celulaInformacoes.Border = PdfPCell.NO_BORDER;
            informacoes.AddCell(celulaInformacoes);

            celulaInformacoes = new PdfPCell(new Phrase("Data:\n\n", FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
            celulaInformacoes.Border = PdfPCell.NO_BORDER;
            informacoes.AddCell(celulaInformacoes);

            celulaInformacoes = new PdfPCell(new Phrase("Rev:\n\n", FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
            celulaInformacoes.Border = PdfPCell.NO_BORDER;
            informacoes.AddCell(celulaInformacoes);

            celulaInformacoes = new PdfPCell(new Phrase(string.Format("Página:"), FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
            celulaInformacoes.Border = PdfPCell.NO_BORDER;
            informacoes.AddCell(celulaInformacoes);

            PdfPCell celulaInformacoesGeral = new PdfPCell(informacoes)
            {
                Colspan = 1,
                FixedHeight = 100,
                BorderWidth = 1f,
                BorderColor = BaseColor.BLACK
            };
            cabecalho.AddCell(celulaInformacoesGeral);

            // Coluna 4: Valores
            PdfPTable valores = new PdfPTable(1);
            PdfPCell celulaValores = new PdfPCell(new Phrase(sSiglaDocumento + "\n\n", FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
            celulaValores.Border = PdfPCell.NO_BORDER;
            valores.AddCell(celulaValores);

            celulaValores = new PdfPCell(new Phrase(dtDocumento.ToString("dd/MM/yyyy" + "\n\n"), FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
            celulaValores.Border = PdfPCell.NO_BORDER;
            valores.AddCell(celulaValores);

            celulaValores = new PdfPCell(new Phrase(nRev + "\n\n", FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
            celulaValores.Border = PdfPCell.NO_BORDER;
            valores.AddCell(celulaValores);

            celulaValores = new PdfPCell(new Phrase(string.Format("", writer.PageNumber, writer.PageNumber), FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.BLACK)));
            celulaValores.Border = PdfPCell.NO_BORDER;
            valores.AddCell(celulaValores);

            PdfPCell celulaValoresGeral = new PdfPCell(valores)
            {
                Colspan = 1,
                FixedHeight = 100,
                BorderWidth = 1f,
                BorderColor = BaseColor.BLACK
            };
            cabecalho.AddCell(celulaValoresGeral);

            cabecalho.WriteSelectedRows(0, -1, doc.LeftMargin, doc.PageSize.Height - doc.TopMargin + cabecalho.TotalHeight, writer.DirectContent);
        }

        private void Rodape(PdfWriter writer, Document doc)
        {
            // Numero de paginas
            int pageN = writer.PageNumber;
            string text = pageN + " / ";
            float yPos;
            float len = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED).GetWidthPoint(text, 9);

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 9);

            if (writer.PageNumber == 1)
            {
                yPos = doc.Top - 1;
            }
            else
            {
                yPos = doc.Top + 23;
            }

            cb.SetTextMatrix(463, yPos);

            cb.ShowText(text);
            cb.EndText();
            cb.AddTemplate(template, 463 + len, yPos);

            PdfContentByte cb2 = writer.DirectContent;
            cb2.SetColorStroke(BaseColor.BLACK);
            cb2.SetLineWidth(1f);

            // Define o início e o fim da linha do rodapé
            float xStart = doc.LeftMargin;
            float xEnd = doc.PageSize.Width - doc.RightMargin;
            float yPosition = doc.BottomMargin - 50;

            // Desenha a linha
            cb2.MoveTo(xStart, yPosition);
            cb2.LineTo(xEnd, yPosition);
            cb2.Stroke();
        }
    }
}