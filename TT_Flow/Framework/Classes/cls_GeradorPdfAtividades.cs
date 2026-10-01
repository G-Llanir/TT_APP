using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing; // Usado por ZXing para Bitmap
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using TT.FrameWork;
using ZXing;
using ZXing.QrCode;
using Image = iText.Layout.Element.Image;
using Rectangle = iText.Kernel.Geom.Rectangle;

public class GeradorPdfAtividades
{
    // Definimos as margens como constantes para usar de forma segura nos métodos de desenho
    private const float MARGEM_SUPERIOR = 100f;
    private const float MARGEM_DIREITA = 36f;
    private const float MARGEM_INFERIOR = 100f;
    private const float MARGEM_ESQUERDA = 36f;

    private static readonly DeviceRgb CINZA_TEXTO = new DeviceRgb(85, 85, 85);
    private static readonly DeviceRgb PRETO_TEXTO = new DeviceRgb(0, 0, 0);
    private string CaminhoDaUSer;
    public byte[] GerarRelatorioAtividadesPdf(DataSet dadosDoRelatorio, string logoPath, string caminhoDaUSer)
    {
        CaminhoDaUSer = caminhoDaUSer;
        if (dadosDoRelatorio == null || dadosDoRelatorio.Tables.Count < 1)
        {
            throw new ArgumentException("O DataSet fornecido é nulo ou não contém a tabela principal.");
        }

        using (MemoryStream ms = new MemoryStream())
        {
            PdfWriter writer = new PdfWriter(ms);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4);

            document.SetMargins(MARGEM_SUPERIOR, MARGEM_DIREITA, MARGEM_INFERIOR, MARGEM_ESQUERDA);

            #region Lógica Principal
            DataTable dadosPrincipais = dadosDoRelatorio.Tables[0];
            DataTable dadosApontamentos = dadosDoRelatorio.Tables.Count > 1 ? dadosDoRelatorio.Tables[1] : null;
            DataTable dadosEPIs = dadosDoRelatorio.Tables.Count > 2 ? dadosDoRelatorio.Tables[2] : null;
            DataTable dadosConsumiveis = dadosDoRelatorio.Tables.Count > 3 ? dadosDoRelatorio.Tables[3] : null;

            string nomeUsuarioImpressao = "Não Informado";
            if (dadosPrincipais.Rows.Count > 0 && dadosPrincipais.Columns.Contains("NomeUsuarioImpressao") && dadosPrincipais.Rows[0]["NomeUsuarioImpressao"] != DBNull.Value)
            {
                nomeUsuarioImpressao = dadosPrincipais.Rows[0]["NomeUsuarioImpressao"].ToString();
            }

            var projetos = dadosPrincipais.AsEnumerable()
                .GroupBy(row => new
                {
                    idProjeto = row.Field<int>("idProjeto"),
                    sDscTituloProjeto = row.Field<string>("sDscTituloProjeto"),
                    sNumeroRequisicao = row.Field<object>("sNumeroRequisicao")?.ToString() ?? string.Empty,
                })
                .OrderBy(g => g.Key.idProjeto);

            bool primeiroPaiDeTodos = true;

            foreach (var projetoGroup in projetos)
            {
                AdicionarInfoBloco(document, projetoGroup.Key);

                var atividadesPai = projetoGroup
                    .GroupBy(row => new
                    {
                        idAtividadePai = row.Field<int?>("idAtividadePai"),
                        sDscTituloAtividadePai = row.Field<string>("sDscTituloAtividadePai")
                    })
                    .OrderBy(g => g.Key.idAtividadePai);

                foreach (var atividadePaiGroup in atividadesPai)
                {
                    if (atividadePaiGroup.Key.idAtividadePai == null) continue;

                    // Guarda o número da página ANTES de adicionar o conteúdo deste "pai"
                    int paginaInicialDoPai = pdf.GetNumberOfPages();
                    if (paginaInicialDoPai == 0) paginaInicialDoPai = 1;

                    if (!primeiroPaiDeTodos)
                    {
                        document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                        paginaInicialDoPai = pdf.GetNumberOfPages();
                    }
                    primeiroPaiDeTodos = false;

                    #region Adicionar Conteúdo Principal
                    Table tabelaMestre = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
                    Paragraph tituloPai = new Paragraph(atividadePaiGroup.Key.sDscTituloAtividadePai)
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(12).SetMarginTop(15).SetMarginBottom(5);
                    tabelaMestre.AddCell(new Cell().Add(tituloPai).SetBorder(Border.NO_BORDER));

                    var atividadesFilhas = atividadePaiGroup
                        .Where(r => r.Field<int?>("idAtividade") != null)
                        .GroupBy(row => new
                        {
                            idAtividade = row.Field<int>("idAtividade"),
                            uuidAtividade = row.Field<object>("uuidAtividade")?.ToString() ?? string.Empty,
                            sDscTituloAtividade = row.Field<string>("sDscTituloAtividade"),
                            sDscStatusAtividade = dadosPrincipais.Columns.Contains("sDscStatusAtividade") ? row.Field<string>("sDscStatusAtividade") : "Status Não Disponível",
                            idRecurso = row.Field<int?>("idRecurso")
                        })
                        .OrderBy(g => g.Key.idAtividade);

                    foreach (var atividadeFilhaGroup in atividadesFilhas)
                    {
                        // ... (seu código para montar a 'tabelaAtividadeWrapper' continua aqui, sem alterações)
                        Table tabelaAtividadeWrapper = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth().SetMarginBottom(10);
                        Cell cellWrapper = new Cell().SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 0.5f)).SetPadding(10);
                        Table activityTable = new Table(UnitValue.CreatePercentArray(new float[] { 1f, 5f })).UseAllAvailableWidth();
                        Cell qrCodeCell = new Cell().SetBorder(null).SetVerticalAlignment(VerticalAlignment.TOP);
                        try
                        {
                            byte[] qrCodeBytes = GerarQrCodeParaAtividade(atividadeFilhaGroup.Key.idAtividade, atividadeFilhaGroup.Key.uuidAtividade);
                            ImageData qrCodeData = ImageDataFactory.Create(qrCodeBytes);
                            Image qrCodeImage = new Image(qrCodeData).SetWidth(60).SetHeight(60);
                            qrCodeCell.Add(qrCodeImage);
                        }
                        catch { /* Ignora erro de QR Code */ }
                        activityTable.AddCell(qrCodeCell);
                        Cell textCell = new Cell().SetBorder(null).SetPaddingLeft(10);
                        textCell.Add(new Paragraph(atividadeFilhaGroup.Key.sDscTituloAtividade).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(11).SetFontColor(PRETO_TEXTO));
                        textCell.Add(new Paragraph("Status: " + atividadeFilhaGroup.Key.sDscStatusAtividade).SetFontColor(ColorConstants.BLACK).SetFontSize(9).SetMarginBottom(8));
                        AdicionarDetalhesDaAtividade(textCell, atividadeFilhaGroup, dadosEPIs, dadosConsumiveis, dadosApontamentos);
                        activityTable.AddCell(textCell);
                        cellWrapper.Add(activityTable);
                        tabelaAtividadeWrapper.AddCell(cellWrapper);
                        tabelaMestre.AddCell(new Cell().Add(tabelaAtividadeWrapper).SetBorder(Border.NO_BORDER));
                    }
                    document.Add(tabelaMestre);
                    document.Add(CriarBlocoAssinatura());
                    #endregion

                    // --- NOVA LÓGICA DE DESENHO ---
                    // Força a renderização do conteúdo que acabamos de adicionar.
                    // Isso cria todas as páginas necessárias e as torna "reais".
                    document.Flush();

                    // Descobre a última página que foi gerada por este "pai".
                    int paginaFinalDoPai = pdf.GetNumberOfPages();

                    // Agora iteramos SOMENTE sobre as páginas que este "pai" ocupou
                    // e desenhamos o cabeçalho e rodapé nelas.
                    for (int i = paginaInicialDoPai; i <= paginaFinalDoPai; i++)
                    {
                        PdfPage page = pdf.GetPage(i);
                        // AQUI 'page' NUNCA SERÁ NULO.
                        DesenharCabecalhoFixo(page, logoPath, nomeUsuarioImpressao);
                        DesenharRodapeFixo(page, i);
                    }
                }
            }
            #endregion

            // O laço FOR no final foi REMOVIDO.
            document.Close();
            return ms.ToArray();
        }
    }

    #region Métodos Auxiliares Ajustados (Não dependem mais do 'document')

    private void DesenharCabecalhoFixo(PdfPage page, string logoPath, string usuario)
    {
        if (page == null) return; // Guarda de segurança extra
        Rectangle pageSize = page.GetPageSize();

        Rectangle headerArea = new Rectangle(
            MARGEM_ESQUERDA,
            pageSize.GetTop() - MARGEM_SUPERIOR,
            pageSize.GetWidth() - MARGEM_ESQUERDA - MARGEM_DIREITA,
            60f
        );

        PdfCanvas pdfCanvas = new PdfCanvas(page);
        Canvas canvas = new Canvas(pdfCanvas, headerArea);

        var verdeEscuro = new DeviceRgb(0x1A, 0x4C, 0x2E);
        var verdeClaro = new DeviceRgb(0x00, 0xA6, 0x51);
        Table tableCabecalho = new Table(UnitValue.CreatePercentArray(new float[] { 60, 40 })).UseAllAvailableWidth();
        Table tabelaLogoNome = new Table(UnitValue.CreatePercentArray(new float[] { 1, 4 })).UseAllAvailableWidth().SetBorder(Border.NO_BORDER);

        Cell logoCell = new Cell().SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
        if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
        {
            ImageData imageData = ImageDataFactory.Create(logoPath);
            Image logo = new Image(imageData).SetWidth(50f);
            logoCell.Add(logo);
        }
        tabelaLogoNome.AddCell(logoCell);

        Paragraph nomeEmpresa = new Paragraph("Tec and Tec").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(20).SetFontColor(verdeEscuro);
        tabelaLogoNome.AddCell(new Cell().Add(nomeEmpresa).SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetPaddingLeft(10));
        tableCabecalho.AddCell(new Cell().Add(tabelaLogoNome).SetBorder(Border.NO_BORDER));

        Cell cellRelatorio = new Cell().SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT)
            .Add(new Paragraph("Documento de Fabricação").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(14).SetFontColor(verdeClaro))
            .Add(new Paragraph($"Data de Emissão: {DateTime.Now:dd/MM/yyyy}").SetFontSize(9))
            .Add(new Paragraph($"Emitido por: {usuario}").SetFontSize(9));
        tableCabecalho.AddCell(cellRelatorio);

        canvas.Add(tableCabecalho).Close();
    }

    private void DesenharRodapeFixo(PdfPage page, int pageNum)
    {
        if (page == null) return; // Guarda de segurança extra
        Rectangle pageSize = page.GetPageSize();

        Rectangle footerArea = new Rectangle(
            MARGEM_ESQUERDA,
            MARGEM_INFERIOR - 80f,
            pageSize.GetWidth() - MARGEM_ESQUERDA - MARGEM_DIREITA,
            80f
        );

        PdfCanvas pdfCanvas = new PdfCanvas(page);
        Canvas canvas = new Canvas(pdfCanvas, footerArea);

        Table tabelaInternaRodape = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
        Cell cellInfo = new Cell().SetBorder(Border.NO_BORDER)
            .Add(new Paragraph("TEC AND TEC LATAM AMERICA LTDA").SetFontSize(8))
            .Add(new Paragraph("RUA MARINA CRESPI 188 LETRA B, MOOCA - SAO PAULO-SP").SetFontSize(8))
            .Add(new Paragraph("CNPJ: 19.132.916/0002-04").SetFontSize(8));
        tabelaInternaRodape.AddCell(cellInfo);

        Table wrapperRodape = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
        Cell cellUnica = new Cell().Add(tabelaInternaRodape)
            .SetBorder(Border.NO_BORDER).SetBorderTop(new SolidBorder(ColorConstants.GRAY, 0.5f)).SetPaddingTop(5);
        wrapperRodape.AddCell(cellUnica);

        canvas.Add(wrapperRodape);

        canvas.ShowTextAligned(
            new Paragraph($"Página {pageNum}").SetFontSize(8).SetFontColor(ColorConstants.GRAY),
            pageSize.GetLeft() + (pageSize.GetWidth() / 2),
            MARGEM_INFERIOR - 90,
            TextAlignment.CENTER
        ).Close();
    }


    // MÉTODO AUXILIAR PARA A ASSINATURA (PARA ADICIONAR AO FLUXO)
    private Table CriarBlocoAssinatura()
    {
        Table tabelaAssinatura = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }))
            .UseAllAvailableWidth()
            .SetMarginTop(40);

        tabelaAssinatura.AddCell(new Cell().SetBorder(Border.NO_BORDER)); // Célula vazia para alinhar à direita

        Cell cellAssinatura = new Cell().SetBorder(Border.NO_BORDER)
            .Add(new Paragraph("____________________________").SetFontSize(9).SetHorizontalAlignment(HorizontalAlignment.CENTER))
            .Add(new Paragraph("Assinatura / Carimbo").SetFontSize(9).SetHorizontalAlignment(HorizontalAlignment.CENTER));

        tabelaAssinatura.AddCell(cellAssinatura);
        tabelaAssinatura.SetKeepTogether(true);

        return tabelaAssinatura;
    }

    private byte[] GerarQrCodeParaAtividade(int idAtividade, string uuidAtividade)
    {
        string qrCodeIdentifier = !string.IsNullOrEmpty(uuidAtividade) ? uuidAtividade : idAtividade.ToString();
        string qrCodeContent = $"https://t-flow.tecandtec.com.br/App/Paginas/LeitorAtividade.aspx?id={qrCodeIdentifier}";
        var writer = new BarcodeWriter { Format = BarcodeFormat.QR_CODE, Options = new QrCodeEncodingOptions { Width = 200, Height = 200, Margin = 0 } };
        using (Bitmap qrCodeBitmap = writer.Write(qrCodeContent))
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                qrCodeBitmap.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }
    }

    private void AdicionarInfoBloco(Document document, dynamic projetoKey)
    {
        var tableInfo = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth().SetMarginTop(10).SetMarginBottom(20);
        Cell cellProjeto = new Cell().SetBorder(Border.NO_BORDER).SetPaddingRight(10)
            .Add(new Paragraph("Projeto:").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(10))
            .Add(new Paragraph($"{projetoKey.sDscTituloProjeto}\nRequisição: {projetoKey.sNumeroRequisicao}").SetFontColor(CINZA_TEXTO).SetFontSize(9));
        tableInfo.AddCell(cellProjeto);
        document.Add(tableInfo);
    }

    private void AdicionarDetalhesDaAtividade(Cell textCell, IGrouping<object, DataRow> atividadeFilhaGroup, DataTable dadosEPIs, DataTable dadosConsumiveis, DataTable dadosApontamentos)
    {
        var verdeClaro = new DeviceRgb(0x00, 0xA6, 0x51);
        var key = atividadeFilhaGroup.Key;
        //var usuariosAtividade = atividadeFilhaGroup.Where(r => r.Field<string>("NomeUsuarioAtividade") != null).Select(r => r.Field<string>("NomeUsuarioAtividade")).Distinct().OrderBy(u => u);
        //if (usuariosAtividade.Any())
        //{
        //    textCell.Add(new Paragraph("Membros Atribuídos:").SetFontColor(ColorConstants.DARK_GRAY).SetFontSize(8).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetMarginTop(5).SetMarginBottom(2));
        //    List membrosAtividadeList = new List().SetListSymbol("• ").SetFontColor(CINZA_TEXTO).SetFontSize(8).SetMarginLeft(10);
        //    foreach (var usuarioNome in usuariosAtividade) { membrosAtividadeList.Add(new ListItem(usuarioNome)); }
        //    textCell.Add(membrosAtividadeList);
        //}
        var usuariosAtividadeFoto = atividadeFilhaGroup.Where(r => r.Field<string>("NomeUsuarioAtividade") != null)
                                    .Select(r => new { Nome = r.Field<string>("NomeUsuarioAtividade"), IdUsuario = r.Field<int>("idUsuarioAtividade") })
                                    .Distinct();

        if (usuariosAtividadeFoto.Any())
        {
            textCell.Add(new Paragraph("Membros Atribuídos:").SetFontColor(ColorConstants.DARK_GRAY).SetFontSize(8).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetMarginTop(5).SetMarginBottom(2));

            Table tabelaUsuarios = new Table(UnitValue.CreatePercentArray(new float[] { 1, 10 })).SetFontSize(8).SetFontColor(CINZA_TEXTO).SetMarginLeft(10);

            foreach (var usuario in usuariosAtividadeFoto)
            {
                byte[] imagemBytes = null;
                Image imagemUsuario = null;

                if (usuario.IdUsuario > 0)
                {
                    imagemBytes = this.GetImagemColaboradorBytes(usuario.IdUsuario);
                }

                if (imagemBytes != null && imagemBytes.Length > 0)
                {
                    try
                    {
                        ImageData imageData = ImageDataFactory.Create(imagemBytes);
                        imagemUsuario = new Image(imageData).SetWidth(15).SetHeight(15).SetBorderRadius(new BorderRadius(100)).SetMargins(2, 5, 2, 5);
                    }
                    catch { /* Ignora erros na imagem e continua sem ela */ }
                }

                // Se a imagem não foi encontrada, usa a imagem padrão.
                if (imagemUsuario == null)
                {
                    try
                    {
                        // SUBSTITUA O CAMINHO PARA A SUA IMAGEM PADRÃO
                        ImageData imageDataDefault = ImageDataFactory.Create(CaminhoDaUSer);
                        imagemUsuario = new Image(imageDataDefault).SetWidth(15).SetHeight(15).SetBorderRadius(new BorderRadius(100)).SetMargins(2, 5, 2, 5);
                    }
                    catch { /* Ignora erro na imagem padrão */ }
                }

                Cell imagemCell = new Cell().SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetPadding(0);
                if (imagemUsuario != null)
                {
                    imagemCell.Add(imagemUsuario);
                }

                Cell nomeCell = new Cell().SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetPadding(0);
                nomeCell.Add(new Paragraph(usuario.Nome).SetMarginLeft(5));

                tabelaUsuarios.AddCell(imagemCell);
                tabelaUsuarios.AddCell(nomeCell);
            }
            textCell.Add(tabelaUsuarios);
        }
        int? idRecursoAtual = (int?)key.GetType().GetProperty("idRecurso").GetValue(key, null);
        if (!idRecursoAtual.HasValue) return;
        int idAtividadeAtual = (int)key.GetType().GetProperty("idAtividade").GetValue(key, null);
        var episDaAtividade = dadosEPIs?.AsEnumerable().Where(row => row.Field<int?>("idRecurso") == idRecursoAtual).ToList();
        if (episDaAtividade != null && episDaAtividade.Any())
        {
            textCell.Add(new Paragraph("EPIs Necessários:").SetFontColor(ColorConstants.DARK_GRAY).SetFontSize(8).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetMarginTop(8).SetMarginBottom(4));
            Table tabelaEPIs = new Table(UnitValue.CreatePercentArray(new float[] { 1, 5 })).UseAllAvailableWidth();
            tabelaEPIs.AddHeaderCell(new Cell().Add(new Paragraph("Quantidade")).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(8).SetFontColor(ColorConstants.WHITE).SetBackgroundColor(verdeClaro).SetBorder(Border.NO_BORDER).SetPadding(2));
            tabelaEPIs.AddHeaderCell(new Cell().Add(new Paragraph("Descrição")).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(8).SetFontColor(ColorConstants.WHITE).SetBackgroundColor(verdeClaro).SetBorder(Border.NO_BORDER).SetPadding(2));
            foreach (var epi in episDaAtividade)
            {
                tabelaEPIs.AddCell(new Cell().Add(new Paragraph(epi.Field<object>("nQuantidade")?.ToString() ?? "N/A")).SetFontSize(8).SetFontColor(CINZA_TEXTO).SetBorder(Border.NO_BORDER).SetPadding(2));
                tabelaEPIs.AddCell(new Cell().Add(new Paragraph(epi.Field<string>("sDscProduto") ?? "N/A")).SetFontSize(8).SetFontColor(CINZA_TEXTO).SetBorder(Border.NO_BORDER).SetPadding(2));
            }
            textCell.Add(tabelaEPIs);
        }
        var consumiveisDaAtividade = dadosConsumiveis?.AsEnumerable().Where(row => row.Field<int?>("idRecurso") == idRecursoAtual).ToList();
        if (consumiveisDaAtividade != null && consumiveisDaAtividade.Any())
        {
            textCell.Add(new Paragraph("Consumíveis Necessários:").SetFontColor(ColorConstants.DARK_GRAY).SetFontSize(8).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetMarginTop(8).SetMarginBottom(4));
            Table tabelaConsumiveis = new Table(UnitValue.CreatePercentArray(new float[] { 1, 5 })).UseAllAvailableWidth();
            tabelaConsumiveis.AddHeaderCell(new Cell().Add(new Paragraph("Quantidade")).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(8).SetFontColor(ColorConstants.WHITE).SetBackgroundColor(verdeClaro).SetBorder(Border.NO_BORDER).SetPadding(2));
            tabelaConsumiveis.AddHeaderCell(new Cell().Add(new Paragraph("Descrição")).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(8).SetFontColor(ColorConstants.WHITE).SetBackgroundColor(verdeClaro).SetBorder(Border.NO_BORDER).SetPadding(2));
            foreach (var consumivel in consumiveisDaAtividade)
            {
                tabelaConsumiveis.AddCell(new Cell().Add(new Paragraph(consumivel.Field<object>("nQuantidade")?.ToString() ?? "N/A")).SetFontSize(8).SetFontColor(CINZA_TEXTO).SetBorder(Border.NO_BORDER).SetPadding(2));
                tabelaConsumiveis.AddCell(new Cell().Add(new Paragraph(consumivel.Field<string>("sDscProduto") ?? "N/A")).SetFontSize(8).SetFontColor(CINZA_TEXTO).SetBorder(Border.NO_BORDER).SetPadding(2));
            }
            textCell.Add(tabelaConsumiveis);
        }
        var apontamentosDaAtividade = dadosApontamentos?.AsEnumerable().Where(ap => ap.Field<int>("idAtividade") == idAtividadeAtual).OrderBy(ap => ap.Field<DateTime>("DataApontamento")).ToList();
        if (apontamentosDaAtividade != null && apontamentosDaAtividade.Any())
        {
            textCell.Add(new Paragraph("Apontamentos:").SetFontColor(ColorConstants.DARK_GRAY).SetFontSize(8).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetMarginTop(5).SetMarginBottom(2));
            List listaApontamentos = new List().SetListSymbol("").SetMarginLeft(10).SetFontSize(8).SetFontColor(CINZA_TEXTO);
            foreach (var apontamento in apontamentosDaAtividade)
            {
                string linha = $"• {apontamento.Field<DateTime>("DataApontamento"):dd/MM/yyyy HH:mm} - {apontamento.Field<string>("UsuarioApontamento")}: {apontamento.Field<string>("DescricaoApontamento")}";
                listaApontamentos.Add(new ListItem(linha));
            }
            textCell.Add(listaApontamentos);
        }
    }

    #endregion

    #region  | Imagem Colaborador
    private byte[] GetImagemColaboradorBytes(int idUsuario)
    {
        int idColaborador = -1;

        DataTable dtColaborador = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_WMS_Fabricacao", new Dictionary<string, string> {
        { "@sFuncao", "CONSULTAR-ASSOCIADOS" },
        { "@idUsuario", idUsuario.ToString() },
    });

        DataRow colaboradorRow = dtColaborador.AsEnumerable()
                                            .FirstOrDefault(r => r.Field<int>("idUsuarioIntegrado") == idUsuario);

        if (colaboradorRow != null)
        {
            idColaborador = colaboradorRow.Field<int>("idColaborador");
        }

        if (idColaborador > 0)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
        {
            { "@sFuncao", "CONSULTAR_IMAGEM" },
            { "@idTipoArquivo", "40" },
            { "@idObjeto", idColaborador.ToString() }
        };

            DataTable dtPesquisa = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dtPesquisa.Rows.Count > 0)
            {
                DataRow imgBd = dtPesquisa.Rows[0];
                return (byte[])imgBd["vbArquivo"];
            }
        }

        return null;
    }


    #endregion
}