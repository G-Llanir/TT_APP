using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using TT_Flow.FrameWork;
using static System.Globalization.CultureInfo;
using static System.IO.Path;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Funcoes.Extenso;
using static TT.FrameWork.Grid;
using static TT.FrameWork.Identity;
using static TT.FrameWork.Validacoes;
using Image = iTextSharp.text.Image;
using List_Item = System.Web.UI.WebControls.ListItem;

namespace TT_Flow.App.Paginas.Comercial
{
    public partial class Orcamento_Detalhe : Page
    {
        #region | Construtores

        #region | Procedures

        public static readonly string sProcedure = "sp_Manipula_tbl_Flow_Pedidos";
        public static readonly string sProcedure_Tipo = "sp_Manipula_tbl_Flow_Comercial_Orcamento_Tipo";
        public static readonly string sProcedure_Produtos = "sp_Manipula_tbl_Flow_Produtos";
        public static readonly string sProcedure_TabelaPreco = "sp_Manipula_tbl_Flow_Comercial_TabelaPreco";
        public static readonly string sProcedure_Clientes = "sp_Manipula_tbl_Flow_Clientes";
        public static readonly string sProcedure_CRM = "sp_Manipula_tbl_Flow_Comercial_CRM";
        public static readonly string sProcedure_Empresas = "sp_Manipula_tbl_Flow_Empresas";
        public static readonly string sProcedure_PedidosCotacao__CondPagamento_Parceiro = "sp_Manipula_tbl_Flow_Pedidos_Cotacao";
        public static readonly string sProcedure_CondPagamento = "sp_Manipula_tbl_Flow_CondicaodePagamento";
        public static readonly string sProcedure_Regras = "sp_Manipula_tbl_Flow_Fiscal_Regras";
        public static readonly string sProcedure_Moedas = "sp_Manipula_tbl_Flow_Adm_Moedas";

        #endregion

        #region | Colunas de GridView

        public static readonly int Escopos_Coluna__idEscopo = 0;
        public static readonly int Escopos_Coluna__idCategoria = 1;
        public static readonly int Escopos_Coluna__Pergunta = 2;
        public static readonly int Escopos_Coluna__PrimeiraOpcao = 3;

        public static readonly int Servicos_Coluna__ID = 0;
        public static readonly int Servicos_Coluna__Ordem = 1;
        public static readonly int Servicos_Coluna__Quantidade = 6;
        public static readonly int Servicos_Coluna__Prazo = 7;
        public static readonly int Servicos_Coluna__Valor = 8;
        public static readonly int Servicos_Coluna__Margem = 9;
        public static readonly int Servicos_Coluna__Desconto = 10;

        public static readonly int Servicos_Composicao_1_Coluna__ID = 0;
        public static readonly int Servicos_Composicao_1_Coluna__Ordem = 1;
        public static readonly int Servicos_Composicao_1_Coluna__Quantidade = 6;
        public static readonly int Servicos_Composicao_1_Coluna__Valor = 7;
        public static readonly int Servicos_Composicao_1_Coluna__Margem = 8;

        public static readonly int Servicos_Composicao_2_Coluna__ID = 0;
        public static readonly int Servicos_Composicao_2_Coluna__Ordem = 1;
        public static readonly int Servicos_Composicao_2_Coluna__Quantidade = 6;
        public static readonly int Servicos_Composicao_2_Coluna__Valor = 7;
        public static readonly int Servicos_Composicao_2_Coluna__Margem = 8;

        public static readonly int Servicos_Composicao_3_Coluna__ID = 0;
        public static readonly int Servicos_Composicao_3_Coluna__Quantidade = 6;
        public static readonly int Servicos_Composicao_3_Coluna__Valor = 7;
        public static readonly int Servicos_Composicao_3_Coluna__Margem = 8;

        public static readonly int ControleMargem_Coluna__ID = 0;
        public static readonly int ControleMargem_Coluna__SubServico = 1;
        public static readonly int ControleMargem_Coluna__Margem = 2;

        public static readonly int Produtos_Coluna__ID = 0;
        public static readonly int Produtos_Coluna__Prazo = 5;
        public static readonly int Produtos_Coluna__Quantidade = 7;
        public static readonly int Produtos_Coluna__Valor = 8;
        public static readonly int Produtos_Coluna__Desconto = 9;

        public static readonly int Produtos_Composicao_Coluna__ID = 0;
        public static readonly int Produtos_Composicao_Coluna__Ordem = 1;

        public static readonly int Produtos_Comparativos_Coluna__CheckBox = 0;
        public static readonly int Produtos_Comparativos_Coluna__ID = 1;
        public static readonly int Produtos_Comparativos_Coluna__Ordem_Com_Composicao = 2;
        public static readonly int Produtos_Comparativos_Coluna__Ordem = 3;
        public static readonly int Produtos_Comparativos_Coluna__Prazo = 7;
        public static readonly int Produtos_Comparativos_Coluna__Prazo_Edição = 8;
        public static readonly int Produtos_Comparativos_Coluna__Valor = 11;
        public static readonly int Produtos_Comparativos_Coluna__Desconto = 13;
        public static readonly int Produtos_Comparativos_Coluna__Valor_Desconto = 14;

        public static readonly int Produtos_Comparativos_Composicao_Coluna__ID = 0;
        public static readonly int Produtos_Comparativos_Composicao_Coluna__Ordem = 1;
        public static readonly int Produtos_Comparativos_Composicao_Coluna__Quantidade = 5;

        public static readonly int Servicos_Comparativos_Coluna__ID = 0;
        public static readonly int Servicos_Comparativos_Coluna__Ordem = 1;
        public static readonly int Servicos_Comparativos_Coluna__Quantidade = 7;
        public static readonly int Servicos_Comparativos_Coluna__Valor = 8;
        public static readonly int Servicos_Comparativos_Coluna__Margem = 9;
        public static readonly int Servicos_Comparativos_Coluna__Desconto = 10;

        #endregion

        #region | Listas

        /// <summary>
        /// Lista utilizada para armazenar os Serviços, que podem ser incluídos em uma Empreitada
        /// </summary>
        public List<cls_Comercial_Tabelas> listServicos_Incluir_Empreitada
        {
            get
            {
                if (ViewState["listServicos_Incluir_Empreitada"] == null)
                {
                    ViewState["listServicos_Incluir_Empreitada"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listServicos_Incluir_Empreitada"];
            }
            set
            {
                ViewState["listServicos_Incluir_Empreitada"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar os Serviços, para a aplicação da Empreitada
        /// </summary>
        public List<cls_Comercial_Tabelas> listServicos_Empreitada
        {
            get
            {
                if (ViewState["listServicos_Empreitada"] == null)
                {
                    ViewState["listServicos_Empreitada"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listServicos_Empreitada"];
            }
            set
            {
                ViewState["listServicos_Empreitada"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar a Composição dos Serviços, para a aplicação da Empreitada
        /// </summary>
        public List<cls_Comercial_Tabelas> listServicos_Composicao_Filhos_Empreitada
        {
            get
            {
                if (ViewState["listServicos_Composicao_Filhos_Empreitada"] == null)
                {
                    ViewState["listServicos_Composicao_Filhos_Empreitada"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listServicos_Composicao_Filhos_Empreitada"];
            }
            set
            {
                ViewState["listServicos_Composicao_Filhos_Empreitada"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar a Composição dos Serviços, para a aplicação da Empreitada
        /// </summary>
        public List<cls_Comercial_Tabelas> listServicos_Composicao_Netos_Empreitada
        {
            get
            {
                if (ViewState["listServicos_Composicao_Netos_Empreitada"] == null)
                {
                    ViewState["listServicos_Composicao_Netos_Empreitada"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listServicos_Composicao_Netos_Empreitada"];
            }
            set
            {
                ViewState["listServicos_Composicao_Netos_Empreitada"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar a Composição dos Serviços, para a aplicação da Empreitada
        /// </summary>
        public List<cls_Comercial_Tabelas> listServicos_Composicao_Bisnetos_Empreitada
        {
            get
            {
                if (ViewState["listServicos_Composicao_Bisnetos_Empreitada"] == null)
                {
                    ViewState["listServicos_Composicao_Bisnetos_Empreitada"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listServicos_Composicao_Bisnetos_Empreitada"];
            }
            set
            {
                ViewState["listServicos_Composicao_Bisnetos_Empreitada"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar os Produtos do Orçamento, para a aplicação da Empreitada
        /// </summary>
        public List<cls_Comercial_Tabelas> listProdutos_Empreitada
        {
            get
            {
                if (ViewState["listProdutos_Empreitada"] == null)
                {
                    ViewState["listProdutos_Empreitada"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listProdutos_Empreitada"];
            }
            set
            {
                ViewState["listProdutos_Empreitada"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar os Produtos do Orçamento
        /// </summary>
        public List<cls_Comercial_Tabelas> listProdutos
        {
            get
            {
                if (ViewState["listProdutos"] == null)
                {
                    ViewState["listProdutos"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listProdutos"];
            }
            set
            {
                ViewState["listProdutos"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar os Produtos do Orçamento, que compõem os Sistemas, após manipulações pelos Comparativos
        /// </summary>
        public List<cls_Comercial_Tabelas> listProdutos_Composicao
        {
            get
            {
                if (ViewState["listProdutos_Composicao"] == null)
                {
                    ViewState["listProdutos_Composicao"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listProdutos_Composicao"];
            }
            set
            {
                ViewState["listProdutos_Composicao"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar os Produtos do Orçamento, para as manipulações da aba de Comparativos
        /// </summary>
        public List<cls_Comercial_Tabelas> listProdutos_Comparativos
        {
            get
            {
                if (ViewState["listProdutos_Comparativos"] == null)
                {
                    ViewState["listProdutos_Comparativos"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listProdutos_Comparativos"];
            }
            set
            {
                ViewState["listProdutos_Comparativos"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar os Produtos do Orçamento, de Composição de Sistemas, para as manipulações da aba de Comparativos
        /// </summary>
        public List<cls_Comercial_Tabelas> listProdutos_Comparativos_Composicao
        {
            get
            {
                if (ViewState["listProdutos_Comparativos_Composicao"] == null)
                {
                    ViewState["listProdutos_Comparativos_Composicao"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listProdutos_Comparativos_Composicao"];
            }
            set
            {
                ViewState["listProdutos_Comparativos_Composicao"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar os Serviços
        /// </summary>
        public List<cls_Comercial_Tabelas> listServicos_Recursos
        {
            get
            {
                if (ViewState["listServicos_Recursos"] == null)
                {
                    ViewState["listServicos_Recursos"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listServicos_Recursos"];
            }
            set
            {
                ViewState["listServicos_Recursos"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar Sub-Serviços ou Recursos
        /// </summary>
        public List<cls_Comercial_Tabelas> listServicos_Recursos_Composicao_Filhos
        {
            get
            {
                if (ViewState["listServicos_Recursos_Composicao_Filhos"] == null)
                {
                    ViewState["listServicos_Recursos_Composicao_Filhos"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listServicos_Recursos_Composicao_Filhos"];
            }
            set
            {
                ViewState["listServicos_Recursos_Composicao_Filhos"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar os Recursos
        /// </summary>
        public List<cls_Comercial_Tabelas> listServicos_Recursos_Composicao_Netos
        {
            get
            {
                if (ViewState["listServicos_Recursos_Composicao_Netos"] == null)
                {
                    ViewState["listServicos_Recursos_Composicao_Netos"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listServicos_Recursos_Composicao_Netos"];
            }
            set
            {
                ViewState["listServicos_Recursos_Composicao_Netos"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar os Recursos
        /// </summary>
        public List<cls_Comercial_Tabelas> listServicos_Recursos_Composicao_Bisnetos
        {
            get
            {
                if (ViewState["listServicos_Recursos_Composicao_Bisnetos"] == null)
                {
                    ViewState["listServicos_Recursos_Composicao_Bisnetos"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listServicos_Recursos_Composicao_Bisnetos"];
            }
            set
            {
                ViewState["listServicos_Recursos_Composicao_Bisnetos"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar os Serviços para as manipulações da aba de Comparativos
        /// </summary>
        public List<cls_Comercial_Tabelas> listServicos_Comparativos
        {
            get
            {
                if (ViewState["listServicos_Comparativos"] == null)
                {
                    ViewState["listServicos_Comparativos"] = new List<cls_Comercial_Tabelas>();
                }
                return (List<cls_Comercial_Tabelas>)ViewState["listServicos_Comparativos"];
            }
            set
            {
                ViewState["listServicos_Comparativos"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar as informações dos Escopos
        /// </summary>
        public DataTable dt_CheckList
        {
            get
            {
                if (ViewState["dt_CheckList"] == null)
                {
                    ViewState["dt_CheckList"] = new DataTable();
                }
                return (DataTable)ViewState["dt_CheckList"];
            }
            set
            {
                ViewState["dt_CheckList"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar as informações das Opções selecionadas em cada Pergunta nas CheckLists
        /// </summary>
        public List<cls_Categoria> list_Perguntas_x_Opcoes
        {
            get
            {
                if (ViewState["list_Perguntas_x_Opcoes"] == null)
                {
                    ViewState["list_Perguntas_x_Opcoes"] = new List<cls_Categoria>();
                }
                return (List<cls_Categoria>)ViewState["list_Perguntas_x_Opcoes"];
            }
            set
            {
                ViewState["list_Perguntas_x_Opcoes"] = value;
            }
        }

        /// <summary>
        /// Lista utilizada para armazenar os nomes dos Arquivos do PDF
        /// </summary>
        public List<string> list_PDFs
        {
            get
            {
                if (ViewState["list_PDFs"] == null) ViewState["list_PDFs"] = new List<string>();
                return (List<string>)ViewState["list_PDFs"];
            }
            set => ViewState["list_PDFs"] = value;
        }

        #endregion

        #region | Propriedades

        private static int Nivel_Permissao
        {
            get
            {
                if (ValidaPermissao(Permissao.Comercial.Orcamento.Nivel_Adm)) return 4;
                else if (ValidaPermissao(Permissao.Comercial.Orcamento.Nivel_3)) return 3;
                else if (ValidaPermissao(Permissao.Comercial.Orcamento.Nivel_2)) return 2;
                else if (ValidaPermissao(Permissao.Comercial.Orcamento.Nivel_1)) return 1;
                else return 0;
            }
        }

        private static int nMax_Desconto_Servicos { get { int n = Nivel_Permissao; return n == 4 ? 30 : n == 3 ? 20 : n == 2 ? 10 : n == 1 ? 6 : 0; } }
        private static int nMax_Desconto_Produtos { get { int n = Nivel_Permissao; return n == 4 ? 15 : n == 3 ? 10 : n == 2 ? 5 : n == 1 ? 3 : 0; } }

        #endregion

        #region | Classes

        public class NovaCondicao_Pers
        {
            public string tipo { get; set; }
            public string porcentagem { get; set; }
            public string ddl { get; set; }
        }

        public class CheckBoxItem : ITemplate
        {
            private int _nOpcao;
            private string _sOpcao;
            private GridView _gv;
            private bool _bView;

            public CheckBoxItem(int nOpcao, string sOpcao, GridView gv, bool bView)
            {
                _nOpcao = nOpcao;
                _sOpcao = sOpcao;
                _gv = gv;
                _bView = bView;
            }

            public void InstantiateIn(Control container)
            {
                CheckBox checkBox = new CheckBox
                {
                    ID = _bView ? "cb_Opcao_View_" + _nOpcao + "|" + _nOpcao + "|" + _gv.HeaderRow.Cells[_nOpcao + 3].Text + "|" : "cb_Opcao_" + _nOpcao + "|" + +_nOpcao + "|" + _sOpcao + "|"
                };

                if (_bView)
                {
                    checkBox.Attributes.Remove("onclick");
                    checkBox.Attributes.Add("onclick", "return false;");
                }

                container.Controls.Add(checkBox);
            }
        }

        public class CheckBox_Todos : ITemplate
        {
            private int _nOpcao;
            private string _sOpcao;

            public CheckBox_Todos(string sOpcao, int nOpcao)
            {
                _nOpcao = nOpcao;
                _sOpcao = sOpcao;
            }

            public void InstantiateIn(Control container)
            {
                HtmlGenericControl pOpcao = new HtmlGenericControl("p")
                {
                    InnerText = _sOpcao
                };

                HtmlGenericControl divOpcao = new HtmlGenericControl("div");
                CheckBox checkBox = new CheckBox
                {
                    ID = "cb_Opcoes_Todos|" + _nOpcao
                };
                checkBox.Attributes.Add("style", "width: 100%; display: flex; justify-content: center;");

                divOpcao.Controls.Add(checkBox);

                container.Controls.Add(pOpcao);
                container.Controls.Add(divOpcao);
            }
        }

        protected class Escopo
        {
            public Escopo() { }
            public string Categoria { get; set; }
            public string[] Cabeçalho { get; set; }
            public List<string[]> Linhas { get; set; }
        }

        protected class PDF_Header : PdfPageEventHelper
        {
            public override void OnStartPage(PdfWriter writer, Document doc)
            {
                base.OnStartPage(writer, doc);

                PdfPTable cabecalho = new PdfPTable(2)
                {
                    TotalWidth = doc.PageSize.Width - doc.RightMargin - doc.LeftMargin
                };
                cabecalho.SetWidths(new float[] { 20, 80 });

                Image imagem = Image.GetInstance(Combine(AppDomain.CurrentDomain.BaseDirectory, "App", "img", "LogoTT_Horizontal.png"));
                imagem.ScaleAbsolute(100f, 100f);

                PdfPCell celulaLogo = new PdfPCell(imagem, true)
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    MinimumHeight = 60,
                    Border = 0
                };
                cabecalho.AddCell(celulaLogo);

                PdfPCell celulaTitulo = new PdfPCell(new Phrase("Orçamento de Vendas", FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD, BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    MinimumHeight = 50,
                    Border = 0
                };
                cabecalho.AddCell(celulaTitulo);

                cabecalho.WriteSelectedRows(0, -1, 9.5f, doc.Top + 95, writer.DirectContent);
            }
        }

        public class Text_Location : LocationTextExtractionStrategy
        {
            public List<TextChunk> TextChunks { get; private set; } = new List<TextChunk>();

            public override void RenderText(TextRenderInfo renderInfo)
            {
                base.RenderText(renderInfo);

                var baseline = renderInfo.GetBaseline().GetStartPoint();
                var topRight = renderInfo.GetAscentLine().GetEndPoint();
                float x = baseline[0];
                float y = baseline[1];
                float width = topRight[0] - x;
                float height = topRight[1] - y;

                TextChunks.Add(new TextChunk(renderInfo.GetText(), x, y, width, height));
            }

            public class TextChunk
            {
                public string Text { get; }
                public float X { get; }
                public float Y { get; }
                public float Width { get; }
                public float Height { get; }

                public TextChunk(string text, float x, float y, float width, float height)
                {
                    Text = text;
                    X = x;
                    Y = y;
                    Width = width;
                    Height = height;
                }
            }
        }

        #endregion

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-Orcamento.pdf";

            txtEstimativaEntrega.Attributes["placeholder"] = DateTime.Today.AddDays(1).ToString("dd/MM/yyyy");

            if (string.IsNullOrEmpty(Request["id"])) DirecionaPagina("App/Paginas/Comercial/Orcamento_Detalhe.aspx?id=0");

            div_recarregaServicos.Visible = false;

            FiltroPesquisaParceiros.TipoFiltroPesquisa = "4";
            FiltroPesquisaParceiros.ClientID_FocusPersonalizado = cmdSelecionarParceiro.ClientID;
            FiltroPesquisaProdutos.TipoFiltroPesquisa = "Orcamento";

            if (!IsPostBack)
            {
                if (Request["id"] == "0") ValidaPermissao(Permissao.Comercial.Orcamento.Incluir, true);
                else if (Request["id"] != null) ValidaPermissao(Permissao.Comercial.Orcamento.Consultar, true);

                Pesquisar(Request["id"], false);

                if (Request["msg"] == "1") MensagemPagina_View.MostraMensagem_Sucesso("Registro gravado com sucesso!", false);
                else if (Request["msg"] == "2") MensagemPagina_View.MostraMensagem_Sucesso("Pedido vinculado com sucesso!", false);
                else if (Request["msg"] == "3") MensagemPagina_View.MostraMensagem_Sucesso("Status alterado com sucesso!", false);

                //Agnes Partal - 01/07/2024 -----------
                if (Request["PDF"] == "true") ScriptManager.RegisterStartupScript(Page, Page.GetType(), "generatePDF", "window.onload = function() { document.getElementById('" + cmdGeraPDF.ClientID + "').click(); };", true);
                //------------------------------------
            }

            FiltroPesquisaProdutos.idTabelaParceiro = ddlTabela.SelectedValue == null || ddlTabela.SelectedValue == "" || ddlTabela.SelectedValue == "0" ? "0" : ddlTabela.SelectedValue;

            if (hddProgresso.Value.Contains("Empresa") && listServicos_Recursos.Count <= 0)
            {
                div_pn3.Visible = false;
                div_recarregaServicos.Visible = true;
                MensagemPaginaServicos_Recursos.MostraMensagem_Aviso("<b>Aviso:</b> Não há Serviços disponíveis ou liberados!", false);
            }
            else if (Convert.ToBoolean(hddOcamento_Empreitada.Value))
            {
                div_pn3.Visible = false;
                div_pn4.Visible = false;
                MensagemPaginaServicos_Recursos.MostraMensagem_Aviso("<b>Aviso:</b> Em uma Empreitada não é possível Editar Serviços ou Escopos!", false);
                MensagemPagina_ForaProdutos.MostraMensagem_Aviso("<b>Aviso:</b> Em uma Empreitada não é possível Editar ou Incluir Produtos!", false);
            }
            else div_pn3.Visible = true;

            if (ddlEmpresa_Orcamento.Items.Count > 0 && ddlEmpresa_Orcamento.SelectedItem.Text != "Selecione a Empresa")
            {
                int nPaisEmpresa = 2;

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idParceiro", ddlEmpresa_Orcamento.SelectedValue }
                };
                DataSet ds = ExecutarDataSet(sProcedure_Empresas, vParametros);

                if (ValidarDataSet(ds))
                {
                    try
                    {
                        nPaisEmpresa = string.IsNullOrEmpty(DATASET(ds, 2, 0, "idTipoPais")) ? 2 : int.Parse(DATASET(ds, 2, 0, "idTipoPais"));
                    }
                    catch { }
                }

                FiltroPesquisaProdutos.TerritorioEmpresa = nPaisEmpresa;
            }

            FiltroPesquisaProdutos.AlteraCampos_x_Tipo(0, true, true);

            var requestTarget = Request["__EVENTTARGET"];
            if (requestTarget == "funcao_SALVAR")
            {
                AtualizaClasseGeral();
                SalvarOrcamento(Request["id"], false);
            }
            else if (requestTarget == "funcao_APLICAR_COMPARATIVOS")
            {
                AtualizaClasses_Comparativo();
                SalvarOrcamento(Request["id"], true);
            }
            else if (requestTarget == "funcao_EDITAR") Pesquisar(Request["id"], true);
            else if (requestTarget == "funcao_VINCULAR") AbrirModal_VinculaCRM();
            else if (requestTarget == "funcao_VINCULAR_NOVO_PEDIDO") AbrirModal_VinculaPedido(true);
            else if (requestTarget == "funcao_VINCULAR_PEDIDO") AbrirModal_VinculaPedido(false);
            else if (requestTarget == "funcao_ATUALIZA_OPCAO")
            {
                Atualiza_Opcoes_CheckList();
                MantemEtapa_Pos_PostBack(3);

                Scripts.Mantem_AbaAtiva(Page, "aba-CheckList");
                Scripts.FocusScript(Page, hddOpcaoSelecionada_Focus.Value);
            }
            else if (requestTarget == "funcao_EXCLUIR_PRODUTOS") Excluir_Produtos();
            else if (requestTarget == "funcao_UNIFICAR_PRODUTOS") UnificarProdutosDuplicados(false);
            else if (requestTarget == "funcao_UNIFICAR_TODOS") UnificarProdutosDuplicados(true);
            else if (requestTarget == "funcao_ATUALIZA_SERVICOS_ESCOPOS")
            {
                try
                {
                    hddidTiposServicos.Value = "|";
                    hddidEscopos.Value = "|";

                    foreach (List_Item item in lstTipoServicos_TipoOrcamento.Items)
                    {
                        if (item.Selected)
                            hddidTiposServicos.Value += item.Value + "|";
                    }

                    foreach (List_Item item in lstEscopos_TipoOrcamento.Items)
                    {
                        if (item.Selected)
                            hddidEscopos.Value += item.Value + "|";
                    }
                }
                catch { }

                Popula_gvServicos_Recursos(ddlTipoOrcamento.SelectedValue, true);
                Popula_gvCheckList(ddlTipoOrcamento.SelectedValue, true);

                div_recarregaServicos.Visible = !(listServicos_Recursos.Count > 0);

                AtualizaBarraProgresso(1, true);
                MantemEtapa_Pos_PostBack(1);
            }

            // ---------------------------------------------------------
            // Mensagens Fixas

            if (!div_pn5.Visible) MensagemPagina_View.MostraMensagem_Aviso("<b>Aviso:</b> Preencha as outras etapas antes de poder ter uma Visualização Geral!", false);

            if (Request["id"] == "0" || cmdSalvar.Visible)
            {
                cmdAtualiza.Visible = true;
                MensagemPaginaDentro_View.MostraMensagem("<b>Informativo:</b> É possível que nem todas as informações do Orçamento tenham sido carregadas automaticamente, se necessário aperte o botão de Atualizar Informações (<i class=\"fa fa-refresh\"></i>) localizado logo abaixo, e verifique novamente!", "info", false);
            }
            else cmdAtualiza.Visible = false;

            if (listProdutos.Exists(p => (p.bLiberado && p.Preco.Equals(decimal.Zero)) || p.IdItem.Equals(0)))
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Explicação:</b> Os Itens destacados em vermelho representam Itens que não podem ser Salvos.<br />- Caso estes Itens não estejam cadastrados como Produtos, haverá um botão (<i class='fa fa-link'></i>) localizado na coluna de Código do Item em questão, onde nele será posível Vincular este Item à um Produto já cadastrado!<br />- Também é possível que estes Itens não estejam cadastrados ou liberados na Tabela de Preços selecionada, e por isso estariam com Valor = 0,00. Neste caso haverá um botão (<i class=\'fa fa-money'></i>) localizado na coluna de Valor do Item em questão, onde ele será responsável por atualizar o valor do Item de acordo com seu valor atual na Tabela de Preços selecionada!", false);

            MensagemPagina_Modal_NovoEndereco_Fixa.MostraMensagem_Aviso("<b>Aviso:</b> Esta tela fará o registro de um novo Endereço de Entrega no cadastro do Parceiro selecionado!", false);

            MensagemPagina_Modal_ImportarProdutos.MostraMensagem(@"<b>Lembrete</b><br />- O arquivo Excel será lido, considerando apenas as primeiras colunas do Arquivo selecionado, dentro do seguinte esquema:<br /><br />
                                                                        <table class=""importarProdutos_msg"">
                                                                            <thead>
                                                                                <tr>
                                                                                    <th>Ordem</th>
                                                                                    <th>Código do Produto</th>
                                                                                    <th>Quantidade</th>
                                                                                </tr>
                                                                            </thead>
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td>10</td>
                                                                                    <td>Código_do_Item</td>
                                                                                    <td>2</td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>", "INFO", false);

            MensagemPagina_Modal_ImportarProdutos_Pedidos.MostraMensagem("<b>Lembrete:</b> Para Importar os Itens, o Pedido deve possuir ao menos 1 Produto!", "INFO", false);
            MensagemPagina_Modal_ImportarProdutos_LM.MostraMensagem("<b>Lembrete:</b> Para Importar os Itens, o Pedido deve possuir uma Lista de Materiais, e a Lista de Materiais deve incluir ao menos 1 Produto!", "INFO", false);
            MensagemPagina_Modal_ImportarProdutos_Orcamento.MostraMensagem("<b>Lembrete:</b> Para Importar os Itens, o Orçamento deve possuir ao menos 1 Produto!", "INFO", false);
            MensagemPagina_Fixa_Modal_ImportarProdutos_TabelaPreco.MostraMensagem_Aviso("<b>Aviso:</b> Esta aba fará a Importação de Todos os Produtos cadastrados, que estiverem Liberados, na Tabela de Preços exibida abaixo!", false);
            MensagemPagina_Modal_NovaCondicaoPagamento_Fixa.MostraMensagem_Aviso("<b>Aviso:</b> Esta tela fará o registro de uma Nova Condição de Pagamento no cadastro do Parceiro selecionado!", false);
            MensagemPagina_Modal_NovaCondicaoPagamento_Fixa_Personalizada.MostraMensagem_Aviso("<b>Aviso:</b> Esta tela fará a criação de uma Nova Condição de Pagamento, que só poderá ser aplicada neste Orçamento!", false);

            if (int.TryParse(hddTipoSituacao_Parceiro.Value, out int idTipoSituacao) && idTipoSituacao > 0)
                MensagemPaginaDentro_View.MostraMensagem_Aviso($@"<b>Aviso:</b> O <a href='/App/Paginas/Manutencao/Parceiros_Detalhe.aspx?id={hddidCliente.Value}' target='_blank'>Parceiro</a> selecionado neste Orçamento está com Status bloqueado (Bloqueio {(idTipoSituacao.Equals(1) ? "Administrativo" : "Financeiro")})!<br />Nesta situação não será possível dar continuidade ao Pedido vinculado à este Orçamento (caso exista).", false);

            if (listProdutos.Any(p => p.idRegra == 0 && p.bLiberado)) MensagemPagina_ProdutosRegras_Fixa.MostraMensagem_Aviso(ValidaRegraFiscal_hddMsg(), false);

            ExcelImportar.ID_FileUpload = ImportarArquivo.ID;
            RegistraScript();
        }

        protected void Pesquisar(string idOrcamento, bool bEditar)
        {
            ddlUF_Entrega.Attributes.Add("disabled", "disabled");
            ddlMunicipio_Entrega.Attributes.Add("disabled", "disabled");

            cmdVincular_group.Visible = false;
            cmdCRM.Visible = false;
            cmdVincular_Pedido_group.Visible = false;
            cmdPedido.Visible = false;
            cmdCotacao.Visible = false;
            cmdAplicar_Comparativos.Visible = false;

            div_cmdEmpreitada.Visible = false;
            div_cmdDrawback.Visible = false;
            div_Comparativos_Totais_Empreitada.Visible = false;
            divIncluirServico_Empreitada.Visible = false;

            div_Fluxo.Visible = false;
            div_lstTipoServicos.Visible = false;
            div_lstEscopos.Visible = false;
            div_AtualizarServicos.Visible = false;
            div_empresa.Visible = false;

            div_Cidade_NovoEndereco.Visible = false;

            div_Orcamento_id.Visible = false;

            div_ImpostosProdutos_Comparativos_Ajustado.Visible = false;
            div_Comparativos_Totais_Ajustado.Visible = false;
            div_Desconto_Global_Produtos_Comparativos.Visible = false;
            div_Prazo_Global_Produtos_Comparativos.Visible = false;

            div_pn5.Visible = false;
            aba_Orcamento.Visible = false;
            div_Orcamento_View.Visible = false;
            div_panel_InfoInicial_View.Visible = false;
            div_panelFinalizacao_View.Visible = false;
            aba_Servicos_Recursos_View.Visible = false;
            div_Servicos_Recursos_View.Visible = false;
            aba_Produtos.Visible = false;
            div_Produtos_View.Visible = false;
            aba_Comparativos.Visible = false;

            cmdAvancar.Visible = false;
            cmdRetornar.Visible = false;

            div_MensagemPaginaGeral.Visible = false;
            div_Voltar.Visible = false;
            div_Salvar_Sempre_1.Visible = false;
            div_Salvar_Sempre_2.Visible = false;
            div_Salvar_Sempre_3.Visible = false;
            div_Salvar_Sempre_4.Visible = false;

            cmdSalvar.Visible = true;
            cmdSalvar.Text = "Criar Orçamento";
            cmdEditar.Visible = false;

            cmdSalvar_View_CheckList.Visible = true;
            cmdSalvar_View_CheckList.Text = "Criar Orçamento";
            cmdEditar_View_CheckList.Visible = false;

            cmdSalvar_View_Servicos.Visible = true;
            cmdSalvar_View_Servicos.Text = "Criar Orçamento";
            cmdEditar_View_Servicos.Visible = false;

            cmdSalvar_View_Produtos.Visible = true;
            cmdSalvar_View_Produtos.Text = "Criar Orçamento";
            cmdEditar_View_Produtos.Visible = false;

            cmdSalvar_View_Historico.Visible = true;
            cmdSalvar_View_Historico.Text = "Criar Orçamento";
            cmdEditar_View_Historico.Visible = false;

            aba_Historico.Visible = false;

            lblTituloPagina.Visible = false;

            lblTituloPagina.Text = "Novo Orçamento";
            BreadCrumb_Pagina.TitulodaPagina = "Novo Orçamento";

            div_cmdNovaCondicaoPagamento.Visible = false;
            div_NovaCondPgto_Motivo.Visible = false;
            div_NovaCondPgto_Motivo_View.Visible = false;

            PopulaCombos();

            smDesconto_Servicos_Recursos.InnerText = $"- Máximo: {nMax_Desconto_Servicos}%";
            smDesconto_Produtos.InnerText = $"- Máximo: {nMax_Desconto_Produtos}%";
            smDesconto_Produtos_Comparativos.InnerText = $"- Máximo: {nMax_Desconto_Produtos}%";

            if (idOrcamento != "0")
            {
                ValidaPermissao(Permissao.Comercial.Orcamento.Consultar, true);

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTA PEDIDO" },
                    { "@idTipo", "1" },
                    { "@idPedido", idOrcamento }
                };
                DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                if (ValidarDataSet(dsPesquisa))
                {
                    if (!DATASET(dsPesquisa, "sTabelaTipo_LPU").Equals("S"))
                        cbLPU.Visible = false;

                    hddTipoSituacao_Parceiro.Value = DATASET(dsPesquisa, "idTipoSituacaoCliente");
                    hddsTipoDrawback.Value = DATASET(dsPesquisa, "sTipoDrawback");
                    hddidStatus.Value = DATASET(dsPesquisa, "idStatus");
                    hddDtPedido.Value = DATASET(dsPesquisa, "dtPedido");
                    hddDtAtualizacaoPedido.Value = DATASET(dsPesquisa, "dtUltimaAtualizacao");
                    hddNumeroPedido.Value = DATASET(dsPesquisa, "nNumeroPedido");

                    if (!Convert.ToBoolean(Request["duplicar"]))
                    {
                        hddVincula_CRM.Value = DATASET(dsPesquisa, "idCRM");
                        hddVincula_Cotacao.Value = DATASET(dsPesquisa, "idCotacao");
                    }

                    if (DATASET(dsPesquisa, "sConfidencial").Equals("S"))
                        ValidaPermissao(Permissao.Comercial.Orcamento.Nivel_3, true);
                    else if (!DATASET(dsPesquisa, "idVendedor").Equals(Variaveis.idUsuario()) && !(Nivel_Permissao >= 2))
                        ValidaPermissao(Permissao.Comercial.Orcamento.Nivel_3, true);

                    if (!Convert.ToBoolean(Request["duplicar"]))
                    {
                        hddPedidoVinculado.Value = DATASET(dsPesquisa, "idPedido_Vinculado");
                        hddPedidoVinculado_TarefaFinalizada.Value = DATASET(dsPesquisa, "sTarefaFinalizada");
                    }

                    hddidTipoOrcamento.Value = DATASET(dsPesquisa, "idTipoOrcamento");
                    hddidEmpresa.Value = DATASET(dsPesquisa, "idEmpresa");
                    hddidContato.Value = DATASET(dsPesquisa, "idContato_Cliente");
                    hddidEndereco_Fiscal.Value = DATASET(dsPesquisa, "idEnderecoEntrega");
                    hddidEndereco_Entrega.Value = DATASET(dsPesquisa, "idEnderecoDestino");
                    hddidTipoCliente.Value = DATASET(dsPesquisa, "idTipoCliente");
                    hddidSegmentos.Value = DATASET(dsPesquisa, "sidSegmentosCliente");
                    hddidTiposServicos.Value = DATASET(dsPesquisa, "sidTiposServicos");
                    hddidEscopos.Value = DATASET(dsPesquisa, "sidEscopos");
                    hddsDscTiposServicos.Value = DATASET(dsPesquisa, "ssDscTiposServicos");
                    hddsDscEscopos.Value = DATASET(dsPesquisa, "ssDscEscopos");
                    hddidInstalador.Value = DATASET(dsPesquisa, "idInstalador");
                    hddMoeda.Value = DATASET(dsPesquisa, "idTipoMoeda");
                    ddlidInstalador.SelectedValue = DATASET(dsPesquisa, "idInstalador");
                    txtCusto_Aduaneiro.Text = DATASET(dsPesquisa, "nCusto_Aduaneiro");
                    txtCusto_Despachante.Text = DATASET(dsPesquisa, "nCusto_Despachante");
                    txtInstalador_View.Text = DATASET(dsPesquisa, "sDscInstalador");
                    hddCondPgto_Alterada.Value = string.IsNullOrEmpty(DATASET(dsPesquisa, "sAlteracaoCondPag_Motivo")) ? "false" : "true";

                    if (Convert.ToBoolean(hddCondPgto_Alterada.Value))
                    {
                        div_NovaCondPgto_Motivo.Visible = true;
                        div_NovaCondPgto_Motivo_View.Visible = true;

                        txtNovaCondPgto_Motivo.Text = DATASET(dsPesquisa, "sAlteracaoCondPag_Motivo");
                        txtNovaCondPgto_Motivo_View.Text = txtNovaCondPgto_Motivo.Text;
                    }

                    hddOcamento_Empreitada.Value = DATASET(dsPesquisa, "sEmpreitada").Equals("S") ? "true" : "false";

                    Popula_Combo(ddlEmpresa_Comparativo_Ajustado, "sp_Select 'Flow_Empresa_X_TipoOrcamento', @idPesquisa=" + hddidTipoOrcamento.Value, "idParceiro", "sDscEmpresa", false, "Selecione a Empresa", "-2");

                    try { ddlEmpresa_Comparativo_Ajustado.SelectedValue = hddidEmpresa.Value; } catch { }

                    hddidCliente.Value = DATASET(dsPesquisa, 0, 0, "idCliente");
                    hddsTipoCliente.Value = DATASET(dsPesquisa, "sTipoCliente");
                    Popula_Combo(ddlCondicaoPagamento, $"sp_Select 'FLOW_CondicaoDePagamento', @idPesquisa={hddidCliente.Value}, @idPais={Request["id"]}, @idFiltro=2", "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione", "-1");

                    div_divProgresso.Visible = false;
                    divStories.Visible = false;

                    div_Tabela_Obs.Visible = false;
                    div_Tabela_Obs_View.Visible = false;

                    cmdCRM.Visible = hddVincula_CRM.Value != "0";
                    cmdCRM.NavigateUrl = $"/App/Paginas/Comercial/CRM.aspx?id={hddVincula_CRM.Value}";

                    try { hddnRevisao.Value = (int.Parse(DATASET(dsPesquisa, "nRevisao")) + 1).ToString(); }
                    catch { hddnRevisao.Value = "0"; }

                    if (!bEditar && !Convert.ToBoolean(Request["duplicar"]) && !Convert.ToBoolean(Request["revisao"]))
                    {
                        lblTituloPagina.Visible = true;
                        div_Orcamento_id.Visible = true;

                        aba_Comparativos.Visible = true;

                        cmdAvancar.Visible = true;
                        cmdRetornar.Visible = true;

                        cmdVincular_group.Visible = hddVincula_CRM.Value == "0";

                        spanSalvar_Obs.Visible = false;
                        spanSalvar_Obs_2.Visible = false;
                        spanSalvar_Obs_3.Visible = false;
                        spanSalvar_Obs_4.Visible = false;

                        div_pn5.Visible = true;
                        aba_Orcamento.Visible = true;
                        div_Orcamento_View.Visible = true;
                        div_panel_InfoInicial_View.Visible = true;
                        div_panelFinalizacao_View.Visible = true;
                        aba_Servicos_Recursos_View.Visible = listServicos_Recursos.Count > 0;
                        div_Servicos_Recursos_View.Visible = true;
                        aba_Produtos.Visible = listProdutos.Count > 0;
                        div_Produtos_View.Visible = true;

                        pn1.Visible = false;
                        pn2.Visible = false;
                        pn3.Visible = false;
                        pn4.Visible = false;
                        pn1.Attributes.Remove("class");
                        pn1.Attributes.Add("class", "painel desaparece2");
                        pn2.Attributes.Remove("class");
                        pn2.Attributes.Add("class", "painel desaparece2");
                        pn3.Attributes.Remove("class");
                        pn3.Attributes.Add("class", "painel desaparece2");
                        pn4.Attributes.Remove("class");
                        pn4.Attributes.Add("class", "painel desaparece2");
                        pn5.Attributes.Remove("class");
                        pn5.Attributes.Add("style", "width: 100%;");

                        div_BreadCrumb.Attributes.Remove("style");
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Page-wrapper_addClass", "$('#page-wrapper').addClass('page-wrapper');", true);

                        divRegiaoVoltar.Visible = false;
                        divRegiaoAvancar.Visible = false;

                        cmdSalvar.Visible = false;
                        cmdSalvar_View_CheckList.Visible = false;
                        cmdSalvar_View_Servicos.Visible = false;
                        cmdSalvar_View_Produtos.Visible = false;
                        cmdSalvar_View_Historico.Visible = false;

                        bool bAltera = ValidaPermissao(Permissao.Comercial.Orcamento.Alterar, false);

                        cmdEditar.Visible = bAltera;
                        cmdEditar_View_CheckList.Visible = bAltera;
                        cmdEditar_View_Servicos.Visible = bAltera;
                        cmdEditar_View_Produtos.Visible = bAltera;
                        cmdEditar_View_Historico.Visible = bAltera;

                        field_cancel.Attributes["value"] = "Voltar";
                        voltar_1.Attributes["value"] = "Voltar";
                        voltar_2.Attributes["value"] = "Voltar";
                        voltar_3.Attributes["value"] = "Voltar";
                        voltar_6.Attributes["value"] = "Voltar";

                        lbl_modalAplicarComparativos.InnerText = "Deseja aplicar os Ajustes realizados nos Comparativos?";

                        div_Moeda_View.Visible = DATASET(dsPesquisa, "sCliente_Nacional") == "N";
                        div_Custo_View.Visible = DATASET(dsPesquisa, "sCliente_Nacional") == "N";

                        try
                        {
                            div_cmdDrawback.Visible = DATASET(dsPesquisa, "sDrawback") == "S";

                            txtID_Orcamento.Text = DATASET(dsPesquisa, "idPedido");
                            txtNumero_View.Text = DATASET(dsPesquisa, "nNumeroPedido");
                            ddlStatus.SelectedValue = hddidStatus.Value;

                            ddlTipoOrcamento.SelectedValue = DATASET(dsPesquisa, 0, "idTipoOrcamento");
                            ddlConfidencial.SelectedValue = DATASET(dsPesquisa, 0, "sConfidencial");
                            ddlVendedor.SelectedValue = DATASET(dsPesquisa, 0, "idVendedor_Usuario");

                            hddidVendedor.Value = DATASET(dsPesquisa, 0, "idVendedor");
                            hddMunicipio_Entrega_SelectedValue.Value = DATASET(dsPesquisa, 0, "idCidade_Entrega");
                            hddUF_Fiscal_SelectedValue.Value = DATASET(dsPesquisa, 0, "sUF_Fiscal").ToUpper();
                            hddMunicipio_Fiscal.Value = DATASET(dsPesquisa, 0, "sCidade");
                            hddMunicipio_Origem.Value = DATASET(dsPesquisa, 0, "sDscMunicipio_Origem");
                            hddidDestinoVenda.Value = DATASET(dsPesquisa, 0, "sDestinoVenda");
                            hddidTabela.Value = DATASET(dsPesquisa, 0, "idTabelaPreco");
                            hddidFormaEnvio.Value = DATASET(dsPesquisa, 0, "idTipoEnvio");
                            hddidCondicaoPagamento.Value = DATASET(dsPesquisa, 0, "idCondicaoPagamento");
                            hddidFluxo.Value = DATASET(dsPesquisa, 0, "idFluxo");

                            txtCNPJ_View.Text = DATASET(dsPesquisa, 0, 0, "sCNPJ_Cliente");
                            txtRazaoSocial_View.Text = DATASET(dsPesquisa, 0, 0, "sRazaoSocial");
                            txtEmpresa_View.Text = DATASET(dsPesquisa, 0, 0, "sDscEmpresa");
                            txtComparativo_Empresa.Text = DATASET(dsPesquisa, 0, 0, "sDscEmpresa");
                            txtTipoOrcamento_View.Text = DATASET(dsPesquisa, 0, 0, "sDscTipoOrcamento");
                            txtFluxo_View.Text = DATASET(dsPesquisa, 0, 0, "sDscFluxo");
                            txtControle_TT_View.Text = DATASET(dsPesquisa, 0, 0, "nControleTT");
                            txtControle_TT_id.Text = DATASET(dsPesquisa, 0, 0, "nControleTT");
                            txtReferencia.Text = DATASET(dsPesquisa, 0, 0, "sReferencia");
                            txtReferencia_View.Text = DATASET(dsPesquisa, 0, 0, "sReferencia");
                            txtreferencia_id.Text = DATASET(dsPesquisa, 0, 0, "sReferenciaCompleta");
                            txtIE_View.Text = DATASET(dsPesquisa, 0, 0, "sIE_Cliente");
                            txtMoeda_View.Text = DATASET(dsPesquisa, 0, 0, "sDscMoeda");
                            hddMoeda_Simbolo.Value = DATASET(dsPesquisa, 0, 0, "sSimboloMoeda");
                            txtCambio_View.Text = DATASET(dsPesquisa, 0, 0, "nCambio");
                            txtCusto_Aduaneiro_View.Text = DATASET(dsPesquisa, 0, 0, "nCusto_Aduaneiro");
                            txtCusto_Despachante_View.Text = DATASET(dsPesquisa, 0, 0, "nCusto_Despachante");
                            txtPagamento_View.Text = DATASET(dsPesquisa, 0, 0, "sDscCondicaoPagamento");
                            txtVendedor_View.Text = DATASET(dsPesquisa, 0, 0, "sDscVendedorUsuario");
                            txtFormaEnvio_View.Text = DATASET(dsPesquisa, 0, 0, "sDscTipoEnvio");
                            txtEstimativa_View.Text = DATASET(dsPesquisa, 0, 0, "dtEstimativaEntrega");
                            txtEndereco_View.Text = DATASET(dsPesquisa, 0, 0, "sLogradouroEntrega");
                            txtObs_View.Text = DATASET(dsPesquisa, 0, 0, "sObservacao");
                            txtObservacao.Text = DATASET(dsPesquisa, 0, 0, "sObservacao");
                            txtConfidencial_View.Text = DATASET(dsPesquisa, 0, 0, "sConfidencialCompleto");
                            txtTransporte_View.Text = DATASET(dsPesquisa, 0, 0, "sEmpresaTransporte");
                            txtFrete_View.Text = Math.Round(decimal.Parse(DATASET(dsPesquisa, 0, 0, "nFretePrevisto")), 2).ToString();
                            txtTabela_View.Text = DATASET(dsPesquisa, 0, 0, "sDscTabelaPreco");
                            txtTabela_Obs_View.Text = DATASET(dsPesquisa, 0, 0, "sAlteracaoTabelaPreco_Obs");
                            txtRevisao_View.Text = DATASET(dsPesquisa, 0, 0, "idVersao");
                            txtDestinoVenda_View.Text = DATASET(dsPesquisa, 0, 0, "sDscDestinoVenda");
                            txtEnderecoEntrega_View.Text = DATASET(dsPesquisa, 0, 0, "sDscEnderecoDestino");
                            txtUF_Entrega_View.Text = DATASET(dsPesquisa, 0, 0, "sUF_Entrega").ToUpper();
                            txtDiasPrevisao_View.Text = DATASET(dsPesquisa, 0, 0, "nDiasPrevisao");
                            txtUF_Fiscal_View.Text = DATASET(dsPesquisa, 0, 0, "sUF_Fiscal").ToUpper();
                            txtUF_Origem_View.Text = DATASET(dsPesquisa, 0, 0, "sUF_Origem").ToUpper();
                            txtUF_Origem_Comparativo.Text = DATASET(dsPesquisa, 0, 0, "sUF_Origem").ToUpper();
                            txtUF_Origem_Comparativo_Ajustado.Text = DATASET(dsPesquisa, 0, 0, "sUF_Origem").ToUpper();
                            txtMunicipio_Entrega_View.Text = DATASET(dsPesquisa, 0, 0, "sCidade_Entrega");
                            txtMunicipioFiscal_View.Text = DATASET(dsPesquisa, 0, 0, "sCidade");
                            txtValidade_View.Text = DATASET(dsPesquisa, 0, 0, "nValidadeOrcamento");
                            txtContato_View.Text = DATASET(dsPesquisa, 0, 0, "sDscContato_Cliente");
                            txtTipoCliente_View.Text = DATASET(dsPesquisa, 0, 0, "sDscTipoCliente");

                            PopulaSegmentosCliente();
                            PopulaTipoServicos();
                            PopulaEscopos();

                            hddProgresso.Value = "|Cliente|Empresa|TipoOrcamento|Fluxo|TabelaPreco|Endereco_Fiscal|Endereco_Entrega|Contato|Referencia|DestinoVenda|Validade|Pagamento|Vendedor|Envio|DataEntrega|Servico|Escopo|Produto|";

                            if (txtTabela_Obs_View.Text.Length > 0)
                                div_Tabela_Obs_View.Visible = true;

                            if (txtRevisao_View.Text.Equals("0"))
                                div_Revisao_View.Visible = false;
                            else
                                div_Revisao_View.Visible = true;

                            Consulta_gvCheckList(idOrcamento);
                        }
                        catch (Exception ex)
                        {
                            MensagemPagina_View.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na Consulta dos Dados do Orçamento! <br /> Erro: " + ex.Message, false);
                        }

                        try
                        {
                            DateTime.TryParse(txtEstimativa_View.Text, out DateTime dt);
                            txtPrevisaoEntrega.Text = dt.ToString("yyyy-MM-dd");
                        }
                        catch { }

                        PopulaClasses(dsPesquisa);

                        if (listProdutos.Count > 0 && !listProdutos.Any(p => p.bSistema) && listServicos_Recursos.Count > 0) div_cmdEmpreitada.Visible = true;
                        else div_cmdEmpreitada.Visible = false;

                        lblTituloEditar.Text = string.Format("Deseja Editar o Orçamento {0}?", txtReferencia_View.Text);

                        aba_Comparativos.Visible = !Convert.ToBoolean(hddOcamento_Empreitada.Value) && (hddMoeda.Value == "0" || hddMoeda.Value == "2");

                        if (hddPedidoVinculado.Value != "0")
                        {
                            cmdPedido.Visible = true;
                            cmdPedido.NavigateUrl = $"/App/Paginas/Pedidos_Detalhe.aspx?id={hddPedidoVinculado.Value}";

                            cmdVincular_Pedido_group.Visible = false;

                            aba_Comparativos.Visible = false;

                            if (hddPedidoVinculado_TarefaFinalizada.Value == "S")
                            {
                                cmdEditar.Visible = false;
                                cmdEditar_View_CheckList.Visible = false;
                                cmdEditar_View_Servicos.Visible = false;
                                cmdEditar_View_Produtos.Visible = false;
                                cmdEditar_View_Historico.Visible = false;
                            }
                        }
                        else
                        {
                            cmdPedido.Visible = false;
                            cmdVincular_Pedido_group.Visible = true;
                        }

                        cmdCotacao.Visible = hddVincula_Cotacao.Value != "0";
                        cmdCotacao.NavigateUrl = $"/App/Paginas/Comercial/Cotacao_Detalhe.aspx?id={hddVincula_Cotacao.Value}";
                    }
                    else
                    {
                        if (!bEditar)
                        {
                            hddsTipoDrawback.Value = "";
                            hddidStatus.Value = "";
                            hddDtPedido.Value = DateTime.Today.ToString();
                            hddDtAtualizacaoPedido.Value = DateTime.Now.ToString();
                            hddOcamento_Empreitada.Value = "false";
                        }

                        if (!Convert.ToBoolean(Request["duplicar"]))
                        {
                            FiltroPesquisaParceiros.ConfigurarControles(false);
                            cmdSelecionarParceiro.Visible = false;
                        }
                        else
                            hddNumeroPedido.Value = "0";

                        div_Salvar_Sempre_1.Visible = true;
                        div_Salvar_Sempre_2.Visible = true;
                        div_Salvar_Sempre_3.Visible = true;
                        div_Salvar_Sempre_4.Visible = true;

                        hddsEdicao.Value = "S";

                        spanSalvar_Obs.Visible = true;
                        spanSalvar_Obs_2.Visible = true;
                        spanSalvar_Obs_3.Visible = true;
                        spanSalvar_Obs_4.Visible = true;

                        divStories.Visible = true;
                        divProgressoStories5.Attributes["style"] = "width: 100%";

                        pn1.Visible = true;
                        pn2.Visible = true;
                        pn3.Visible = true;
                        pn4.Visible = true;
                        pn5.Visible = true;
                        pn1.Attributes.Remove("class");
                        pn1.Attributes.Add("class", "painel aparece");
                        pn2.Attributes.Remove("class");
                        pn2.Attributes.Add("class", "painel desaparece2");
                        pn3.Attributes.Remove("class");
                        pn3.Attributes.Add("class", "painel desaparece2");
                        pn4.Attributes.Remove("class");
                        pn4.Attributes.Add("class", "painel desaparece2");
                        pn5.Attributes.Remove("class");
                        pn5.Attributes.Remove("style");
                        pn5.Attributes.Add("class", "painel desaparece2");

                        div_BreadCrumb.Attributes.Add("style", "padding: 0 30px 0 30px;");
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Page-wrapper_removeClass", "$('#page-wrapper').removeClass('page-wrapper');", true);

                        divRegiaoVoltar.Visible = true;
                        divRegiaoAvancar.Visible = true;

                        aba_Orcamento.Visible = true;
                        div_Orcamento_View.Visible = true;
                        div_panel_InfoInicial_View.Visible = true;
                        div_panelFinalizacao_View.Visible = true;
                        aba_Produtos.Visible = listProdutos.Count > 0;
                        aba_Servicos_Recursos_View.Visible = listServicos_Recursos.Count > 0;

                        cmdSalvar.Visible = true;
                        cmdSalvar.Text = "Salvar";
                        cmdEditar.Visible = false;
                        field_cancel.Attributes["value"] = "Cancelar";

                        cmdSalvar_View_CheckList.Visible = true;
                        cmdSalvar_View_CheckList.Text = "Salvar";
                        cmdEditar_View_CheckList.Visible = false;
                        voltar_1.Attributes["value"] = "Cancelar";

                        cmdSalvar_View_Servicos.Visible = true;
                        cmdSalvar_View_Servicos.Text = "Salvar";
                        cmdEditar_View_Servicos.Visible = false;
                        voltar_2.Attributes["value"] = "Cancelar";

                        cmdSalvar_View_Produtos.Visible = true;
                        cmdSalvar_View_Produtos.Text = "Salvar";
                        cmdEditar_View_Produtos.Visible = false;
                        voltar_3.Attributes["value"] = "Cancelar";

                        cmdSalvar_View_Historico.Visible = true;
                        cmdSalvar_View_Historico.Text = "Salvar";
                        cmdEditar_View_Historico.Visible = false;
                        voltar_6.Attributes["value"] = "Cancelar";

                        div_Fluxo.Visible = true;
                        div_empresa.Visible = true;

                        div_cmdNovaCondicaoPagamento.Visible = Nivel_Permissao >= 3;

                        div_Moeda.Visible = DATASET(dsPesquisa, "sCliente_Nacional") == "N";
                        div_Custo.Visible = DATASET(dsPesquisa, "sCliente_Nacional") == "N";

                        if (Convert.ToBoolean(hddOcamento_Empreitada.Value))
                        {
                            FiltroPesquisaParceiros.ConfigurarControles(false);

                            ddlTipoOrcamento.Attributes.Add("disabled", "disabled");
                            ddlFluxo.Attributes.Add("disabled", "disabled");
                            lstTipoServicos_TipoOrcamento.Attributes.Add("disabled", "disabled");
                            lstEscopos_TipoOrcamento.Attributes.Add("disabled", "disabled");
                            ddlEmpresa_Orcamento.Attributes.Add("disabled", "disabled");
                            ddlTabela.Attributes.Add("disabled", "disabled");
                            ddlEndereco.Attributes.Add("disabled", "disabled");
                            ddlEndereco_Entrega.Attributes.Add("disabled", "disabled");
                            ddlDestinoVenda.Attributes.Add("disabled", "disabled");

                            cmdSelecionarParceiro.Attributes["class"] += " desabilitado";
                            div_cmdNovoEndereco.Visible = false;

                            Scripts.DesativaClick(Page, cmdSelecionarParceiro.ClientID);
                        }

                        try
                        {
                            if (Convert.ToBoolean(Request["duplicar"]))
                                txtID_Orcamento.Text = "Duplicado";
                            if (Convert.ToBoolean(Request["revisao"]))
                                txtID_Orcamento.Text = "Revisão";

                            if (!(Nivel_Permissao >= 2))
                                ddlTabela.Attributes.Add("disabled", "disabled");

                            hddProgresso.Value = "|Cliente|Empresa|TipoOrcamento|Fluxo|TabelaPreco|Endereco_Fiscal|Endereco_Entrega|Contato|Referencia|DestinoVenda|Validade|Pagamento|Vendedor|Envio|DataEntrega|Servico|Escopo|Produto|";

                            hddidCliente.Value = DATASET(dsPesquisa, 0, 0, "idCliente");
                            hddsTipoCliente.Value = DATASET(dsPesquisa, "sTipoCliente");
                            FiltroPesquisaParceiros.IDParceiro = hddidCliente.Value;
                            FiltroPesquisaParceiros.sCNPJ_CPF_Parceiro_Colaborador = DATASET(dsPesquisa, 0, 0, "sCNPJ_Cliente");
                            FiltroPesquisaParceiros.sDscParceiro_Colaborador = DATASET(dsPesquisa, 0, 0, "sRazaoSocial");

                            ddlTipoOrcamento.SelectedValue = DATASET(dsPesquisa, 0, 0, "idTipoOrcamento");

                            int nPaisEmpresa = 2;
                            Popula_Combo(ddlEmpresa_Orcamento, "sp_Select 'Flow_Empresa_X_TipoOrcamento', @idPesquisa=" + ddlTipoOrcamento.SelectedValue, "idParceiro", "sDscEmpresa", false, "Selecione a Empresa", "-2");

                            try
                            {
                                ddlEmpresa_Orcamento.SelectedValue = DATASET(dsPesquisa, 0, 0, "idEmpresa");

                                Dictionary<string, string> vParametrosEmpresa = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "CONSULTAR_DETALHE" },
                                    { "@idParceiro", ddlEmpresa_Orcamento.SelectedValue }
                                };
                                DataSet dsEmpresa = ExecutarDataSet(sProcedure_Empresas, vParametrosEmpresa);

                                if (ValidarDataSet(dsEmpresa))
                                {
                                    try
                                    {
                                        nPaisEmpresa = string.IsNullOrEmpty(DATASET(dsEmpresa, 2, 0, "idTipoPais")) ? 2 : int.Parse(DATASET(dsEmpresa, 2, 0, "idTipoPais"));
                                    }
                                    catch { }
                                }
                                FiltroPesquisaProdutos.TerritorioEmpresa = nPaisEmpresa;
                            }
                            catch
                            {
                                FiltroPesquisaProdutos.TerritorioEmpresa = nPaisEmpresa;
                                MensagemPaginaInfoInicial.MostraMensagem_Erro($"<b>Erro:</b> A Empresa salva no Registro deste Orçamento não pode oferecer este Tipo de Orçamento, antes de fazer quaisquer alterações, por favor valide na página da Empresa <a onclick=\"event.preventDefault(); window.open(this.href, '_blank');\" href=\"{string.Format("/App/Paginas/Manutencao/Empresas_Detalhe.aspx?id={0}", DATASET(dsPesquisa, 0, 0, "idEmpresaParceiro"))}\"><i class=\"fa fa-arrow-right\"></i> {DATASET(dsPesquisa, 0, 0, "sDscEmpresa")}</a>.", false);
                            }

                            Popula_Combo(ddlFluxo, "sp_Select 'Flow_Fluxo_x_Orcamento_Tipo', @idPesquisa = " + ddlTipoOrcamento.SelectedValue, "idFluxo", "sDscFluxo", false, "Selecione o Fluxo", "0");
                            ddlFluxo.SelectedValue = DATASET(dsPesquisa, "idFluxo");

                            try
                            {
                                Popula_Combo(lstTipoServicos_TipoOrcamento, "sp_Select 'Flow_Composicao_x_Orcamento_Tipo', @idFiltro=1, @idPesquisa=" + ddlTipoOrcamento.SelectedValue, "idObjeto", "sDscObjeto", false);
                                Popula_Combo(lstEscopos_TipoOrcamento, "sp_Select 'Flow_Composicao_x_Orcamento_Tipo', @idFiltro=2, @idPesquisa=" + ddlTipoOrcamento.SelectedValue, "idEscopo", "sDscEscopo", false);

                                if (lstTipoServicos_TipoOrcamento.Items.Count <= 0)
                                {
                                    div_lstTipoServicos.Visible = false;
                                    div_AtualizarServicos.Visible = false;
                                }
                                else
                                {
                                    div_lstTipoServicos.Visible = true;
                                    div_AtualizarServicos.Visible = true;

                                    foreach (List_Item item in lstTipoServicos_TipoOrcamento.Items)
                                    {
                                        if (hddidTiposServicos.Value.Split('|').Contains(item.Value))
                                            item.Selected = true;
                                    }
                                }

                                if (lstEscopos_TipoOrcamento.Items.Count <= 0)
                                {
                                    div_lstEscopos.Visible = false;
                                    div_AtualizarServicos.Visible = false;
                                }
                                else
                                {
                                    div_lstEscopos.Visible = true;
                                    div_AtualizarServicos.Visible = true;

                                    foreach (List_Item item in lstEscopos_TipoOrcamento.Items)
                                    {
                                        if (hddidEscopos.Value.Split('|').Contains(item.Value))
                                            item.Selected = true;
                                    }
                                }
                            }
                            catch { }

                            txtControle_TT.Text = DATASET(dsPesquisa, "nControleTT");
                            txtReferencia.Text = DATASET(dsPesquisa, "sReferencia");
                            txtIE.Text = DATASET(dsPesquisa, "sIE_Cliente");
                            ddlMoeda.SelectedValue = DATASET(dsPesquisa, "idTipoMoeda");
                            txtCambio.Text = DATASET(dsPesquisa, "nCambio");
                            ddlVendedor.SelectedValue = DATASET(dsPesquisa, "idVendedor_Usuario");
                            hddidVendedor.Value = DATASET(dsPesquisa, "idVendedor");
                            ddlFormaEnvio.SelectedValue = DATASET(dsPesquisa, "idTipoEnvio");

                            DateTime.TryParse(DATASET(dsPesquisa, "dtEstimativaEntrega"), out DateTime dtEstimativaEntrega);
                            txtEstimativaEntrega.Text = dtEstimativaEntrega.ToString("yyyy-MM-dd");

                            try
                            {
                                txtObservacao.Text = DATASET(dsPesquisa, "sObservacao");
                                ddlConfidencial.SelectedValue = DATASET(dsPesquisa, "sConfidencial");
                                txtTransporte.Text = DATASET(dsPesquisa, "sEmpresaTransporte");
                                txtFrete.Text = decimal.Parse(DATASET(dsPesquisa, "nFretePrevisto")).ToString("N2");

                                ddlUF_Entrega.SelectedValue = DATASET(dsPesquisa, "sUF_Entrega").ToUpper();
                                hddUF_Entrega_SelectedValue.Value = DATASET(dsPesquisa, "sUF_Entrega").ToUpper();
                                txtDiasPrevisao.Text = DATASET(dsPesquisa, "nDiasPrevisao");
                                txtUF_Fiscal.Text = DATASET(dsPesquisa, "sUF_Fiscal").ToUpper();
                                hddUF_Fiscal_SelectedValue.Value = DATASET(dsPesquisa, "sUF_Fiscal").ToUpper();

                                txtMunicipioFiscal.Text = DATASET(dsPesquisa, "sCidade");

                                txtValidade.Text = DATASET(dsPesquisa, "nValidadeOrcamento");
                            }
                            catch { }

                            try
                            {
                                Popula_Combo(ddlEndereco, "sp_Select 'Flow_Clientes_Endereco', " + hddidCliente.Value + ", @idFiltro=1", "idEndereco", "sEndereco", false, "Selecione o Endereço", "0");
                                ddlEndereco.SelectedValue = DATASET(dsPesquisa, "idEnderecoEntrega");
                            }
                            catch (Exception ex)
                            {
                                MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na Consulta dos Dados do Endereço Fiscal!<br />Erro Fiscal: " + ex.Message, false);
                            }

                            try
                            {
                                string idPais = Variaveis.idEmpresa() == "Brasil" ? "2" : Variaveis.idEmpresa() == "0" ? "2" : "0";
                                Popula_Combo(ddlTabela, string.Format("sp_Select 'Flow_Comercial_TabelaPreco__Orcamento', @idPesquisa={0}, @idFiltro={1}", hddidCliente.Value, idPais), "idTabela", "sDscTabela", false, "Selecione uma Tabela de Preço", "0");
                                ddlTabela.SelectedValue = DATASET(dsPesquisa, "idTabelaPreco");
                                txtTabelaObs.Text = DATASET(dsPesquisa, "sAlteracaoTabelaPreco_Obs");
                            }
                            catch (Exception ex)
                            {
                                MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na Consulta dos Dados da Tabela de Preço, caso a Tabela salva para este Orçamento seja do Tipo LPU, é possível que seu tempo de Vigência tenha expirado!<br />Erro da Tabela: " + ex.Message, false);
                            }

                            try
                            {
                                Popula_Combo(ddlEndereco_Entrega, "sp_Select 'Flow_Clientes_Endereco', " + hddidCliente.Value + ", @idFiltro=3", "idEndereco", "sEndereco", false, "Selecione um Endereço de Entrega", "0");
                                ddlEndereco_Entrega.Items.Add(new List_Item("Coleta", "-1"));
                                ddlEndereco_Entrega.SelectedValue = DATASET(dsPesquisa, "idEnderecoDestino");
                            }
                            catch (Exception ex)
                            {
                                MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na Consulta dos Dados do Endereço de Entrega!<br />Erro na Entrega: " + ex.Message, false);
                            }

                            try
                            {
                                Popula_Combo(ddlMunicipio_Entrega, "sp_Select 'CIDADE', @sPesquisa='" + hddUF_Entrega_SelectedValue.Value + "'", "idCidade", "sCidade", false, "Selecione", "0");
                                ddlMunicipio_Entrega.SelectedValue = DATASET(dsPesquisa, "idCidade_Entrega");
                                hddMunicipio_Entrega_SelectedValue.Value = DATASET(dsPesquisa, "idCidade_Entrega");

                                Popula_Combo(ddlContato, "sp_Select 'Flow_Clientes_Contato', " + hddidCliente.Value, "idContato", "sNome", false, "Selecione", "0");
                                ddlContato.SelectedValue = DATASET(dsPesquisa, "idContato_Cliente");

                                Popula_Combo(ddlTipoCliente, "sp_Select 'Flow_Segmentos_TipoCliente', 2", "idSegmento_TipoCliente", "sDscSegmento_TipoCliente", false, "Selecione", "0");
                                ddlTipoCliente.SelectedValue = DATASET(dsPesquisa, "idTipoCliente");
                            }
                            catch (Exception ex)
                            {
                                MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na Consulta dos Dados de Município de Entrega, do Contato do Cliente ou do Tipo de Cliente!<br />Erro na Consulta: " + ex.Message, false);
                            }

                            try
                            {
                                foreach (List_Item item in lstSegmentosCliente.Items)
                                {
                                    if (hddidSegmentos.Value.Split('|').Contains(item.Value))
                                        item.Selected = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de popular os Segmentos!<br />Erro nos Segmentos: " + ex.Message, false);
                            }

                            FiltroPesquisaProdutos.idTabelaParceiro = ddlTabela.SelectedValue;
                            FiltroPesquisaProdutos.RegistrarScriptPesquisar();

                            ddlDestinoVenda.SelectedValue = DATASET(dsPesquisa, "sDestinoVenda");

                            if (Nivel_Permissao >= 2 && txtTabelaObs.Text.Length > 0)
                                div_Tabela_Obs.Visible = true;

                            try
                            {
                                ddlCondicaoPagamento.SelectedValue = DATASET(dsPesquisa, "idCondicaoPagamento");
                            }
                            catch
                            {
                                ddlCondicaoPagamento.Items.Add(DATASET(dsPesquisa, "sDscCondicaoPagamento"));
                                ddlCondicaoPagamento.Items.FindByText(DATASET(dsPesquisa, "sDscCondicaoPagamento")).Value = DATASET(dsPesquisa, "idCondicaoPagamento");
                                ddlCondicaoPagamento.SelectedValue = DATASET(dsPesquisa, "idCondicaoPagamento");
                            }

                            txtCNPJ_View.Text = DATASET(dsPesquisa, 0, 0, "sCNPJ_Cliente");
                            txtRazaoSocial_View.Text = DATASET(dsPesquisa, 0, 0, "sRazaoSocial");
                            txtEmpresa_View.Text = DATASET(dsPesquisa, 0, 0, "sDscEmpresa");
                            txtTipoOrcamento_View.Text = DATASET(dsPesquisa, 0, 0, "sDscTipoOrcamento");
                            txtFluxo_View.Text = DATASET(dsPesquisa, 0, 0, "sDscFluxo");
                            txtControle_TT_View.Text = DATASET(dsPesquisa, 0, 0, "nControleTT");
                            txtControle_TT_id.Text = DATASET(dsPesquisa, 0, 0, "nControleTT");
                            txtReferencia_View.Text = DATASET(dsPesquisa, 0, 0, "sReferencia");
                            txtreferencia_id.Text = DATASET(dsPesquisa, 0, 0, "sReferenciaCompleta");
                            txtIE_View.Text = DATASET(dsPesquisa, 0, 0, "sIE_Cliente");
                            txtPagamento_View.Text = DATASET(dsPesquisa, 0, 0, "sDscCondicaoPagamento");
                            txtVendedor_View.Text = DATASET(dsPesquisa, 0, 0, "sDscVendedorUsuario");
                            txtFormaEnvio_View.Text = DATASET(dsPesquisa, 0, 0, "sDscTipoEnvio");
                            txtEstimativa_View.Text = DATASET(dsPesquisa, 0, 0, "dtEstimativaEntrega");
                            txtEndereco_View.Text = DATASET(dsPesquisa, 0, 0, "sLogradouroEntrega");
                            txtObs_View.Text = DATASET(dsPesquisa, 0, 0, "sObservacao");
                            txtConfidencial_View.Text = ddlConfidencial.SelectedItem.Text;
                            txtTransporte_View.Text = DATASET(dsPesquisa, 0, 0, "sEmpresaTransporte");
                            txtFrete_View.Text = Math.Round(decimal.Parse(DATASET(dsPesquisa, 0, 0, "nFretePrevisto")), 2).ToString();
                            txtTabela_View.Text = DATASET(dsPesquisa, 0, 0, "sDscTabelaPreco");
                            txtTabela_Obs_View.Text = DATASET(dsPesquisa, 0, 0, "sAlteracaoTabelaPreco_Obs");
                            txtDestinoVenda_View.Text = DATASET(dsPesquisa, 0, 0, "sDscDestinoVenda");
                            txtEnderecoEntrega_View.Text = DATASET(dsPesquisa, 0, 0, "sDscEnderecoDestino");
                            txtUF_Entrega_View.Text = DATASET(dsPesquisa, 0, 0, "sUF_Entrega").ToUpper();
                            txtDiasPrevisao_View.Text = DATASET(dsPesquisa, 0, 0, "nDiasPrevisao");
                            txtUF_Fiscal_View.Text = DATASET(dsPesquisa, 0, 0, "sUF_Fiscal").ToUpper();
                            txtUF_Origem_View.Text = DATASET(dsPesquisa, 0, 0, "sUF_Origem").ToUpper();
                            txtMunicipio_Entrega_View.Text = DATASET(dsPesquisa, 0, 0, "sCidade_Entrega");
                            txtMunicipioFiscal_View.Text = DATASET(dsPesquisa, 0, 0, "sCidade");
                            txtValidade_View.Text = DATASET(dsPesquisa, 0, 0, "nValidadeOrcamento");
                            txtContato_View.Text = DATASET(dsPesquisa, 0, 0, "sDscContato_Cliente");
                            txtTipoCliente_View.Text = DATASET(dsPesquisa, 0, 0, "sDscTipoCliente");

                            PopulaSegmentosCliente();
                            PopulaTipoServicos();
                            PopulaEscopos();

                            if (txtTabelaObs.Text.Length > 0)
                            {
                                div_Tabela_Obs.Visible = true;
                                div_Tabela_Obs_View.Visible = true;
                            }

                            try
                            {
                                if (Convert.ToBoolean(Request["duplicar"]))
                                {
                                    txtreferencia_id.Text += " - Duplicado";

                                    div_Revisao.Visible = false;
                                    div_Revisao_View.Visible = false;
                                    txtRevisao.Text = "0";
                                }
                                else if (Convert.ToBoolean(Request["revisao"]))
                                {
                                    div_Revisao.Visible = true;
                                    div_Revisao_View.Visible = true;

                                    txtRevisao.Text = (int.Parse(DATASET(dsPesquisa, "nRevisao")) + 1).ToString();
                                    txtRevisao_View.Text = txtRevisao.Text;

                                    txtreferencia_id.Text += " - Rev " + txtRevisao.Text;
                                }
                                else
                                {
                                    div_Revisao.Visible = true;
                                    div_Revisao_View.Visible = true;

                                    txtRevisao.Text = DATASET(dsPesquisa, "idVersao");
                                    txtRevisao_View.Text = txtRevisao.Text;
                                }
                            }
                            catch { }
                        }
                        catch (Exception ex)
                        {
                            div_MensagemPaginaGeral.Visible = true;
                            MensagemPaginaGeral.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na Consulta dos Dados do Orçamento! <br /> Erro: " + ex.Message, false);
                        }

                        if (!bEditar)
                            PopulaClasses(dsPesquisa);

                        gvServicos_Recursos_dataBind();
                        gvProdutos_dataBind();
                        Consulta_gvCheckList(idOrcamento);

                        lblTituloSalvar.Text = string.Format("Deseja Salvar o Orçamento {0}?", txtReferencia_View.Text);

                        AtualizaBarraProgresso(0, true);
                        MantemEtapa_Pos_PostBack(1);
                    }

                    decimal total = listServicos_Recursos.Where(s => s.bLiberado).Sum(s => s.NTotal);
                    txtTotalServicos_View.Text = Math.Round(total, 2).ToString();
                    txtTotalServico_Recurso_View.Text = Math.Round(total, 2).ToString();

                    total = listServicos_Recursos.Where(s => s.bLiberado && s.bProjeto).Sum(s => s.NTotal);
                    txtTotalProjeto_View.Text = Math.Round(total, 2).ToString();

                    total = listProdutos.Where(p => p.bLiberado).Sum(p => p.NTotal);
                    txtTotalProduto_View.Text = Math.Round(total, 2).ToString();
                    txtTotalProdutos_View.Text = Math.Round(total, 2).ToString();

                    txtTotal_View.Text = (decimal.Parse(string.IsNullOrEmpty(txtTotalProduto_View.Text) ? "0" : txtTotalProduto_View.Text) + decimal.Parse(string.IsNullOrEmpty(txtTotalServico_Recurso_View.Text) ? "0" : txtTotalServico_Recurso_View.Text) + decimal.Parse(string.IsNullOrEmpty(txtFrete_View.Text) ? "0" : txtFrete_View.Text) + decimal.Parse(string.IsNullOrEmpty(txtCusto_Aduaneiro_View.Text) ? "0" : txtCusto_Aduaneiro_View.Text) + decimal.Parse(string.IsNullOrEmpty(txtCusto_Despachante_View.Text) ? "0" : txtCusto_Despachante_View.Text)).ToString("N2");

                    if (decimal.Parse(txtTotalServico_Recurso_View.Text) > decimal.Zero)
                    {
                        if (decimal.Parse(txtTotalProjeto_View.Text) < (decimal.Parse(txtTotal_View.Text) * (decimal)0.08))
                        {
                            var att = txtTotalProjeto_View.Attributes.Keys.Cast<string>();

                            if (!att.Contains("style"))
                                txtTotalProjeto_View.Attributes.Add("style", "background-color: #A9DF8D");
                        }
                        else
                        {
                            var att = txtTotalProjeto_View.Attributes.Keys.Cast<string>();

                            if (att.Contains("style"))
                                txtTotalProjeto_View.Attributes.Remove("style");
                        }
                    }

                    PopulaTotais();
                    PopulaHistorico(dsPesquisa.Tables[2]);
                }
                else
                {
                    pnGeral.Visible = false;
                    div_MensagemPaginaGeral.Visible = true;
                    div_Voltar.Visible = true;

                    MensagemPaginaGeral.MostraMensagem_Erro("<b>Erro:</b> Não foram encontrados os dados deste Orçamento!", false);
                }
            }
            else
            {
                ValidaPermissao(Permissao.Comercial.Orcamento.Incluir, true);

                txtID_Orcamento.Text = "Novo";

                hddsEdicao.Value = "S";
                hddDtPedido.Value = DateTime.Today.ToString();

                spanSalvar_Obs.Visible = true;
                spanSalvar_Obs_2.Visible = true;
                spanSalvar_Obs_3.Visible = true;
                spanSalvar_Obs_4.Visible = true;

                liPersonalizada.Visible = false;
                div_Enderecos.Visible = false;
                div_IE.Visible = false;
                div_Moeda.Visible = false;
                div_Custo.Visible = false;
                div_Contato.Visible = false;
                div_Tabela_Obs.Visible = false;
                div_Tabela_Obs_View.Visible = false;
                divs_End_Fiscal.Visible = false;
                divs_End_Entrega.Visible = false;
                div_Confidencial.Visible = false;
                lblTituloSalvar.Text = "Deseja Criar o Novo Orçamento?";
                txtRevisao.Text = "0";
                div_Revisao.Visible = false;
                txtRevisao_View.Text = "0";
                div_Revisao_View.Visible = false;

                if (!string.IsNullOrEmpty(Request["crm"]))
                {
                    cmdVoltarEtapa.Visible = true;
                    cmdAvancarEtapa.Visible = true;
                    divStories.Visible = true;
                    div_tabela.Visible = true;
                    div_empresa.Visible = true;
                    div_orcamento.Visible = true;

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idRegistroCRM", Request["crm"] }
                    };
                    DataSet dsCRM = ExecutarDataSet(sProcedure_CRM, vParametros);

                    if (ValidarDataSet(dsCRM))
                    {
                        hddVincula_CRM.Value = DATASET(dsCRM, "idRegistroCRM");

                        ddlVendedor.SelectedValue = DATASET(dsCRM, "idVendedor_Usuario");
                        hddidVendedor.Value = DATASET(dsCRM, "idVendedor");
                        ddlTipoOrcamento.SelectedValue = DATASET(dsCRM, "idTipoCotacao");
                        hddidCliente.Value = DATASET(dsCRM, "idParceiro");
                        hddsTipoCliente.Value = DATASET(dsCRM, "sTipo");
                        FiltroPesquisaParceiros.IDParceiro = hddidCliente.Value;

                        Dictionary<string, string> vParametrosCliente = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTAR_DETALHE" },
                            { "@idParceiro", hddidCliente.Value }
                        };
                        DataSet dsCliente = ExecutarDataSet(sProcedure_Clientes, vParametrosCliente);

                        FiltroPesquisaParceiros.sCNPJ_CPF_Parceiro_Colaborador = DATASET(dsCliente, "sCPF_CNPJ");
                        FiltroPesquisaParceiros.sDscParceiro_Colaborador = DATASET(dsCliente, "sRazaoSocial");
                        txtReferencia.Text = DATASET(dsCRM, "sReferencia");
                        txtObservacao.Text = DATASET(dsCRM, "sObservacao");
                        txtEstimativaEntrega.Text = DateTime.Parse(DATASET(dsCRM, "dtPrevisao")).ToString("dd/MM/yyyy");
                        ddlConfidencial.SelectedValue = DATASET(dsCRM, "sConfidencial");

                        txtVendedor_View.Text = ddlVendedor.SelectedItem.Text;
                        txtTipoOrcamento_View.Text = ddlTipoOrcamento.SelectedItem.Text;
                        txtRazaoSocial_View.Text = FiltroPesquisaParceiros.sDscParceiro_Colaborador;
                        txtCNPJ_View.Text = FiltroPesquisaParceiros.sCNPJ_CPF_Parceiro_Colaborador;
                        txtReferencia_View.Text = txtReferencia.Text;
                        txtObs_View.Text = txtObservacao.Text;
                        DateTime.TryParse(txtEstimativaEntrega.Text, out DateTime dtPrevisaoEntrega);
                        txtEstimativa_View.Text = dtPrevisaoEntrega.ToString("dd/MM/yyyy");
                        txtConfidencial_View.Text = ddlConfidencial.SelectedItem.Text;

                        hddProgresso.Value = "|Cliente|Empresa|TipoOrcamento|Servico|Vendedor|DataEntrega|";

                        cmdSelecionarParceiro_Click(cmdSelecionarParceiro, new EventArgs());
                        ddlTipoOrcamento_SelectedIndexChanged(ddlTipoOrcamento, new EventArgs());

                        AtualizaBarraProgresso(0, true);
                        MantemEtapa_Pos_PostBack(1);
                    }
                    else MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Não foram encontrados os dados do CRM a ser vinculado!", false);
                }
                else if (!string.IsNullOrEmpty(Request["idCotacao"]))
                {
                    cmdVoltarEtapa.Visible = false;
                    cmdAvancarEtapa.Visible = false;
                    divStories.Visible = false;

                    Dictionary<string, string> vParamCotacao = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTA_DETALHE_COTACAO" },
                        { "@idPedido", Request["idCotacao"] }
                    };
                    DataTable tbCotacao = ExecutarDataTable(sProcedure, vParamCotacao);
                    DataSet dsCotacao = new DataSet(); dsCotacao.Tables.Add(tbCotacao);

                    if (ValidarDataSet(dsCotacao))
                    {
                        try
                        {
                            hddVincula_Cotacao.Value = DATASET(dsCotacao, "idCotacao");

                            hddidCliente.Value = DATASET(dsCotacao, "idParceiro");
                            hddsTipoCliente.Value = DATASET(dsCotacao, "sTipoCliente");
                            FiltroPesquisaParceiros.IDParceiro = hddidCliente.Value;

                            FiltroPesquisaParceiros.sCNPJ_CPF_Parceiro_Colaborador = DATASET(dsCotacao, "sCPF_Parceiro");
                            txtCNPJ_View.Text = FiltroPesquisaParceiros.sCNPJ_CPF_Parceiro_Colaborador;
                            FiltroPesquisaParceiros.sDscParceiro_Colaborador = DATASET(dsCotacao, "sDscParceiro");
                            txtRazaoSocial_View.Text = FiltroPesquisaParceiros.sDscParceiro_Colaborador;

                            cmdSelecionarParceiro_Click(cmdSelecionarParceiro, new EventArgs());

                            hddidTabela.Value = DATASET(dsCotacao, "idTabelaPreco");
                            ddlTabela.SelectedValue = hddidTabela.Value;
                            txtTabela_View.Text = ddlTabela.SelectedItem.Text;
                            hddidEndereco_Entrega.Value = DATASET(dsCotacao, "idEnderecoEntrega");
                            ddlEndereco_Entrega.SelectedValue = hddidEndereco_Entrega.Value;
                            txtEnderecoEntrega_View.Text = ddlEndereco_Entrega.SelectedItem.Text;
                            hddidCondicaoPagamento.Value = DATASET(dsCotacao, "idCondicaoPagamento");
                            ddlCondicaoPagamento.SelectedValue = hddidCondicaoPagamento.Value;
                            txtPagamento_View.Text = ddlCondicaoPagamento.SelectedItem.Text;

                            txtIE.Text = DATASET(dsCotacao, "sDscIE");
                            txtIE_View.Text = txtIE.Text;
                            txtReferencia.Text = DATASET(dsCotacao, "sReferencia");
                            txtReferencia_View.Text = txtReferencia.Text;
                            txtObservacao.Text = DATASET(dsCotacao, "sObservacao");
                            txtObs_View.Text = txtObservacao.Text;
                        }
                        catch { }

                        txtConfidencial_View.Text = ddlConfidencial.SelectedItem.Text;

                        AtualizaBarraProgresso(0, true);
                        MantemEtapa_Pos_PostBack(0);
                    }
                    else MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Não foram encontrados os dados da Cotação a ser vinculada!", false);
                }
                else
                {
                    cmdVoltarEtapa.Visible = false;
                    cmdAvancarEtapa.Visible = false;
                    divStories.Visible = false;
                    div_tabela.Visible = false;
                    div_empresa.Visible = false;
                    div_orcamento.Visible = false;

                    Scripts.FocusScript(Page, "cphCorpo_FiltroPesquisaParceiros_FT_txtComposicao_sCNPJarceiro_sCPFColaborador");
                }
            }

            if (listServicos_Recursos.Any(s => s.bLiberado))
            {
                aba_Servicos_Recursos_View.Visible = true;

                if (dt_CheckList.Rows.Count > 0)
                {
                    aba_CheckList_View.Visible = true;
                    cbEscopos_PDF.Visible = true;
                }
                else
                {
                    aba_CheckList_View.Visible = false;
                    cbEscopos_PDF.Visible = false;
                }
            }
            else
            {
                aba_Servicos_Recursos_View.Visible = false;
                aba_CheckList_View.Visible = false;
                cbEscopos_PDF.Visible = false;
            }

            if (listProdutos.Any(p => p.bLiberado))
                aba_Produtos.Visible = true;
            else
                aba_Produtos.Visible = false;

            try
            {
                divLM_ImportarProdutos.Visible = false;

                txtTabelaPreco_ImportarProdutos.Text = !string.IsNullOrEmpty(ddlTabela.SelectedValue) && ddlTabela.SelectedValue != "0" ? string.Format("ID: {0} - Tabela: {1}, Tipo: Vendas Customizadas", ddlTabela.SelectedValue, ddlTabela.SelectedItem.Text) : txtTabela_View.Text;

                Popula_Combo(ddlPedidos_ImportarProdutos, "sp_Select 'IMPORTACAO_ITENS_ORCAMENTO'", "idPedido", "sDscPedido", false, "Selecione um Pedido para Importar os Itens", "0");
                Popula_Combo(ddlSelecionaPedido_LM_ImportarProdutos, "sp_Select 'IMPORTACAO_ITENS_ORCAMENTO', @idFiltro=1", "idPedido", "sDscPedido", false, "Selecione um Pedido que possui LM para Importar os Itens", "0");
                Popula_Combo(ddlOrcamento_ImportarProdutos, "sp_Select 'IMPORTACAO_ITENS_ORCAMENTO', @idFiltro=3", "idPedido", "sDscPedido", false, "Selecione um Orçamento para Importar os Itens", "0");

                // Remove este Orçamento da Lista de Orçamentos e Remove o Pedido Vinculado das Listas de Pedidos
                if (idOrcamento != "0")
                {
                    ddlOrcamento_ImportarProdutos.Items.Remove(ddlOrcamento_ImportarProdutos.Items.FindByValue(idOrcamento.TrimStart('0')));

                    if (!hddPedidoVinculado.Value.Equals("0"))
                    {
                        ddlPedidos_ImportarProdutos.Items.Remove(ddlPedidos_ImportarProdutos.Items.FindByValue(hddPedidoVinculado.Value));
                        ddlSelecionaPedido_LM_ImportarProdutos.Items.Remove(ddlSelecionaPedido_LM_ImportarProdutos.Items.FindByValue(hddPedidoVinculado.Value));
                    }
                }
            }
            catch { }

            UpdModal_Importar_Pedidos.Update();
            UpdModal_Importar_LM.Update();
            UpdModal_Importar_Orcamento.Update();
            UpdModal_Importar_TabelaPreco.Update();

            if (!div_cmdEmpreitada.Visible || !div_cmdDrawback.Visible) div_Espaco__Empreitada_Drawback.Attributes["class"] = "col-lg-6";

            if (Request["id"] != "0")
            {
                lblTituloPagina.Text = Convert.ToBoolean(Request["duplicar"]) ? "Duplicar Orçamento" : Convert.ToBoolean(Request["revisao"]) ? "Nova Revisão de Orçamento" : $"Orçamento N°{txtNumero_View.Text}: {txtreferencia_id.Text}";
                BreadCrumb_Pagina.TitulodaPagina = Convert.ToBoolean(Request["duplicar"]) ? "Duplicar Orçamento" : Convert.ToBoolean(Request["revisao"]) ? "Nova Revisão de Orçamento" : $"Orçamento N°{txtNumero_View.Text}: {txtreferencia_id.Text}";
            }

            if (listProdutos.Any(p => p.idRegra == 0 && p.bLiberado) && hddPedidoVinculado.Value == "0") cmdVincular_Pedido_group.Visible = false;
        }

        #endregion

        #region | Combos

        protected void PopulaCombos()
        {
            Popula_Combo(ddlStatus, "sp_Select 'Flow_Status', 1", "idStatus", "sDscStatus", false, "Todos os Status", "0");
            Popula_Combo(ddlSistemas_Comparativo, "sp_Select 'Flow_Produtos_x_Tipos_Sistema', @idFiltro=1", "idItem", "sDscProduto", false, "Selecione o Sistema", "0");
            Popula_Combo(ddlidInstalador, "sp_Select 'Flow_Parceiro_Importacao_Instalador'", "idParceiro", "sRazaoSocial", false, "Selecione o Instalador", "0");
            ddlMoeda.Popula_Combo("sp_Select 'tbl_Flow_Tipo_Moeda_CambioCadastrado'", "idMoeda", "sDscTipoMoeda", false, "Selecione a Moeda", "0");
            Popula_Combo(ddlTipoCliente, "sp_Select 'Flow_Segmentos_TipoCliente', 2", "idSegmento_TipoCliente", "sDscSegmento_TipoCliente", false, "Selecione o Tipo", "0");
            Popula_Combo(lstSegmentosCliente, "sp_Select 'Flow_Segmentos_TipoCliente', 1", "idSegmento_TipoCliente", "sDscSegmento_TipoCliente", false);
            Popula_Combo(ddlTipoOrcamento, "sp_Select 'Flow_Orcamento_Tipo'", "idTipoOrcamento", "sDscTipoOrcamento", false, "Selecione o Tipo", "0");
            Popula_Combo(ddlVendedor, "sp_Select 'FLOW_Vendedores'", "idUsuario", "sDscUsuario", false, "Selecione o Vendedor", "0");
            Popula_Combo(ddlFormaEnvio, "sp_Select 'Flow_Pedidos_TipoEnvio', 2", "idTipoEnvio", "sDscTipoEnvio", false, "Selecione", "0");
            Popula_Combo(ddlUF_Entrega, "sp_Select 'Flow_Estado'", "sEstado", "sEstado", false, "Selecione o Estado", "0");
            Popula_Combo(ddlEstado_NovoEndereco, "sp_Select 'Flow_Estado'", "sEstado", "sEstado", false, "Selecione o Estado", "0");

            Popula_Combo(ddlVincularCRM, "sp_Select 'Flow_Vincular_CRM'", "idRegistroCRM", "sReferencia", false, "Selecione um CRM", "0");
            Popula_Combo(ddlVincularPedido, "sp_Select 'Flow_Vincular_Pedido', 2", "idPedido", "sDscPedido", false, "Selecione um Pedido", "0");

            Popula_Combo(ddlTipos_Parcelas_Nova_CondicaoPagamento, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false);
            Popula_Combo(ddlNovaCondicaoPagamento, "sp_Select 'tbl_Flow_CondicaodePagamento'", "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione uma Condição de Pagamento", "-1");

            // Filtra as Condições de Pagamento que já estão cadastradas
            foreach (List_Item item in ddlCondicaoPagamento.Items)
            {
                int.TryParse(item.Value, out int idCondicaoPagamento);

                try
                {
                    if (idCondicaoPagamento > 0)
                        ddlNovaCondicaoPagamento.Items.Remove(ddlNovaCondicaoPagamento.Items.FindByValue(item.Value));
                }
                catch { }
            }

            // Pré-seleciona o campo de Vendedor
            try
            {
                ddlVendedor.SelectedValue = Variaveis.idUsuario();
                if (!hddProgresso.Value.Contains("Vendedor"))
                    hddProgresso.Value += "Vendedor|";
            }
            catch { }
        }

        #endregion

        #region | gvCheckList - Escopos

        protected void Consulta_gvCheckList(string idOrcamento)
        {
            list_Perguntas_x_Opcoes.Clear();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_ESCOPOS" },
                { "@idOrcamento", idOrcamento }
            };
            DataSet ds = ExecutarDataSet(sProcedure_Tipo, vParametros);

            if (ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    cls_Categoria item = new cls_Categoria
                    {
                        idEscopo = Convert.ToInt32(row["idEscopo"].ToString()),
                        idCategoria = Convert.ToInt32(row["idCategoriaEscopo"].ToString()),
                        sPerguntas = row["idPergunta"].ToString().Length > 0 ? row["idPergunta"].ToString() + "|" + row["sPergunta"].ToString() : row["sPergunta"].ToString(),
                        sOpcoes = row["idOpcao"].ToString() + "|" + row["sOpcao"].ToString()
                    };

                    list_Perguntas_x_Opcoes.Add(item);
                }

                Popula_gvCheckList(DATASET(ds, 1, 0, "idTipoOrcamento"), false);
            }
            else
            {
                dt_CheckList = null;

                aba_CheckList.Visible = false;
                div_CheckList.Visible = false;
                aba_CheckList_View.Visible = false;
                div_CheckList_View.Visible = false;
            }
        }

        protected void Popula_gvCheckList(string idTipoOrcamento, bool bNovo)
        {
            dt_CheckList.Clear();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_ESCOPOS" },
                { "@idTipoOrcamento", idTipoOrcamento },
                { "@sidEscopos", hddidEscopos.Value.Length > 1 ? hddidEscopos.Value : "" }
            };

            DataSet ds = ExecutarDataSet(sProcedure_Tipo, vParametros);

            if (ValidarDataSet(ds))
            {
                dt_CheckList = ds.Tables[0];

                if (bNovo)
                    list_Perguntas_x_Opcoes.Clear();

                rptCategoriasEscopos_dataBind();

                aba_CheckList.Visible = true;
                div_CheckList.Visible = true;
                aba_CheckList_View.Visible = true;
                div_CheckList_View.Visible = true;
            }
            else
            {
                dt_CheckList.Clear();
                list_Perguntas_x_Opcoes.Clear();

                aba_CheckList.Visible = false;
                div_CheckList.Visible = false;
                aba_CheckList_View.Visible = false;
                div_CheckList_View.Visible = false;
            }
        }

        protected void rptCategoriasEscopos_dataBind()
        {
            rptCategoriaEscopos.DataSource = dt_CheckList;
            rptCategoriaEscopos.DataBind();

            rptCheckList_View.DataSource = dt_CheckList;
            rptCheckList_View.DataBind();

            if (!hddProgresso.Value.Contains("Escopo") && rptCategoriaEscopos.Items.Count > 0)
                hddProgresso.Value += "Escopo|";
        }

        protected void rptCategoriaEscopos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRow dr = (rptCategoriaEscopos.DataSource as DataTable).Rows[e.Item.ItemIndex];

                GridView gv = e.Item.FindControl("gvCheckList") as GridView;

                string[] opcoes = dr.Field<string>("sOpcoes").Split('|');

                int i = 0;

                foreach (string opcao in opcoes.Where(o => o.Length > 0))
                {
                    TemplateField tf = new TemplateField
                    {
                        HeaderTemplate = new CheckBox_Todos(opcao, i),

                        ItemTemplate = new CheckBoxItem(i, opcao, gv, false)
                    };
                    tf.HeaderStyle.Width = Unit.Percentage(opcao.Length < 10 ? 5 : opcao.Length < 20 ? 10 : 15);
                    tf.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    tf.ItemStyle.VerticalAlign = VerticalAlign.Middle;

                    gv.Columns.Add(tf);

                    i++;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("sPergunta");
                dt.Columns.Add("idCategoria");
                dt.Columns.Add("idEscopo");
                dt.Columns.Add("sDscCategoria");

                foreach (string pergunta in dr.Field<string>("sPerguntas").Split('|').Where(p => p.Length > 0))
                {
                    dt.Rows.Add(pergunta, dr.Field<int>("idCategoria"), dr.Field<int>("idEscopo"), dr.Field<string>("sDscCategoria"));
                }

                gv.DataSource = dt;
                gv.DataBind();
            }
        }

        protected void gvCheckList_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridView gv = sender as GridView;

                if (gv != null)
                {
                    GridViewRow titleRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

                    TableCell titleCell = new TableCell
                    {
                        ColumnSpan = gv.Columns.Count,
                        HorizontalAlign = HorizontalAlign.Center,
                        CssClass = "checkList_Title"
                    };
                    titleRow.Cells.Add(titleCell);

                    gv.Controls[0].Controls.AddAt(0, titleRow);
                }
            }
        }

        protected void gvCheckList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView gv = sender as GridView;

                string pergunta = HttpUtility.HtmlDecode((e.Row.Cells[Escopos_Coluna__Pergunta].Controls[1] as Label).Text);

                e.Row.ID = e.Row.RowIndex.ToString();
                e.Row.ClientIDMode = ClientIDMode.Static;

                if (list_Perguntas_x_Opcoes != null && list_Perguntas_x_Opcoes.Count > 0)
                {
                    var pergunta_x_opcao = list_Perguntas_x_Opcoes.FirstOrDefault(p => p.idEscopo.ToString() == e.Row.Cells[Escopos_Coluna__idEscopo].Text && p.idCategoria.ToString() == e.Row.Cells[Escopos_Coluna__idCategoria].Text && p.sPerguntas.Equals(pergunta));

                    if (pergunta_x_opcao != null)
                        (e.Row.Cells[Convert.ToInt32(pergunta_x_opcao.sOpcoes.Split('|')[0]) + 3].Controls[0] as CheckBox).Checked = true;
                    else
                    {
                        try
                        {
                            var list = list_Perguntas_x_Opcoes.Where(p => p.idEscopo.ToString() == e.Row.Cells[Escopos_Coluna__idEscopo].Text && p.idCategoria.ToString() == e.Row.Cells[Escopos_Coluna__idCategoria].Text && p.sPerguntas.Split('|').Length > 1);
                            pergunta_x_opcao = list.FirstOrDefault(p => p.sPerguntas.Split('|')[1].Equals(pergunta) || p.sPerguntas.Split('|')[0].Equals(e.Row.ID.ToString()));

                            if (pergunta_x_opcao != null)
                                (e.Row.Cells[Convert.ToInt32(pergunta_x_opcao.sOpcoes.Split('|')[0]) + 3].Controls[0] as CheckBox).Checked = true;
                            else
                                (e.Row.Cells[Escopos_Coluna__PrimeiraOpcao].Controls[0] as CheckBox).Checked = true;
                        }
                        catch
                        {
                            (e.Row.Cells[Escopos_Coluna__PrimeiraOpcao].Controls[0] as CheckBox).Checked = true;
                        }
                    }
                }
                else
                    (e.Row.Cells[Escopos_Coluna__PrimeiraOpcao].Controls[0] as CheckBox).Checked = true;

                (gv.Controls[0].Controls[0] as GridViewRow).Cells[0].Text = gv.DataKeys[0]["sDscCategoria"].ToString();

                (e.Row.Cells[Escopos_Coluna__Pergunta].Controls[1] as Label).Text = HttpUtility.HtmlDecode(pergunta);
            }
        }

        protected void rptCheckList_View_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRow dr = (rptCheckList_View.DataSource as DataTable).Rows[e.Item.ItemIndex];

                GridView gv = e.Item.FindControl("gvCheckList_View") as GridView;

                string[] opcoes = dr.Field<string>("sOpcoes").Split('|');

                int i = 0;

                foreach (string opcao in opcoes.Where(o => o.Length > 0))
                {
                    TemplateField tf = new TemplateField
                    {
                        HeaderText = opcao,

                        ItemTemplate = new CheckBoxItem(i, opcao, gv, true)
                    };
                    tf.HeaderStyle.Width = Unit.Percentage(opcao.Length < 10 ? 5 : opcao.Length < 20 ? 10 : 15);
                    tf.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    tf.ItemStyle.VerticalAlign = VerticalAlign.Middle;

                    gv.Columns.Add(tf);

                    i++;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("sPergunta");
                dt.Columns.Add("idCategoria");
                dt.Columns.Add("idEscopo");
                dt.Columns.Add("sDscCategoria");

                foreach (string pergunta in dr.Field<string>("sPerguntas").Split('|').Where(p => p.Length > 0))
                {
                    dt.Rows.Add(pergunta, dr.Field<int>("idCategoria"), dr.Field<int>("idEscopo"), dr.Field<string>("sDscCategoria"));
                }

                gv.DataSource = dt;
                gv.DataBind();
            }
        }

        protected void gvCheckList_View_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridView gv = sender as GridView;

                if (gv != null)
                {
                    GridViewRow titleRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

                    TableCell titleCell = new TableCell
                    {
                        ColumnSpan = gv.Columns.Count,
                        HorizontalAlign = HorizontalAlign.Center,
                        CssClass = "checkList_Title"
                    };
                    titleRow.Cells.Add(titleCell);

                    gv.Controls[0].Controls.AddAt(0, titleRow);
                }
            }
        }

        protected void gvCheckList_View_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridView gv = sender as GridView;

                string pergunta = HttpUtility.HtmlDecode(e.Row.Cells[Escopos_Coluna__Pergunta].Text);

                e.Row.ID = e.Row.RowIndex.ToString();
                e.Row.ClientIDMode = ClientIDMode.Static;

                if (list_Perguntas_x_Opcoes != null && list_Perguntas_x_Opcoes.Count > 0)
                {
                    var pergunta_x_opcao = list_Perguntas_x_Opcoes.FirstOrDefault(p => p.idEscopo.ToString() == e.Row.Cells[Escopos_Coluna__idEscopo].Text && p.idCategoria.ToString() == e.Row.Cells[Escopos_Coluna__idCategoria].Text && p.sPerguntas.Equals(pergunta));

                    if (pergunta_x_opcao != null)
                        (e.Row.Cells[Convert.ToInt32(pergunta_x_opcao.sOpcoes.Split('|')[0]) + 3].Controls[0] as CheckBox).Checked = true;
                    else
                    {
                        try
                        {
                            var list = list_Perguntas_x_Opcoes.Where(p => p.idEscopo.ToString() == e.Row.Cells[Escopos_Coluna__idEscopo].Text && p.idCategoria.ToString() == e.Row.Cells[Escopos_Coluna__idCategoria].Text && p.sPerguntas.Split('|').Length > 1);
                            pergunta_x_opcao = list.FirstOrDefault(p => p.sPerguntas.Split('|')[1].Equals(pergunta) || p.sPerguntas.Split('|')[0].Equals(e.Row.ID.ToString()));

                            if (pergunta_x_opcao != null)
                                (e.Row.Cells[Convert.ToInt32(pergunta_x_opcao.sOpcoes.Split('|')[0]) + 3].Controls[0] as CheckBox).Checked = true;
                            else
                                (e.Row.Cells[Escopos_Coluna__PrimeiraOpcao].Controls[0] as CheckBox).Checked = true;
                        }
                        catch
                        {
                            (e.Row.Cells[Escopos_Coluna__PrimeiraOpcao].Controls[0] as CheckBox).Checked = true;
                        }
                    }
                }
                else
                    (e.Row.Cells[Escopos_Coluna__PrimeiraOpcao].Controls[0] as CheckBox).Checked = true;

                (gv.Controls[0].Controls[0] as GridViewRow).Cells[0].Text = gv.DataKeys[0]["sDscCategoria"].ToString();

                e.Row.Cells[Escopos_Coluna__Pergunta].Text = HttpUtility.HtmlDecode(pergunta);
            }
        }

        #endregion

        #region | gvServicos_Recursos - Serviços

        protected void Popula_gvServicos_Recursos(string idTipoOrcamento, bool bRecarregar)
        {
            if (listServicos_Recursos.Count > 0 && !bRecarregar)
            {
                listServicos_Recursos.ForEach(s => { s.SFuncao = "EXCLUIR ITEM"; s.bLiberado = false; });
                listServicos_Recursos_Composicao_Filhos.ForEach(s => { s.SFuncao = "EXCLUIR ITEM"; s.bLiberado = false; });
                listServicos_Recursos_Composicao_Netos.ForEach(s => { s.SFuncao = "EXCLUIR ITEM"; s.bLiberado = false; });
                listServicos_Recursos_Composicao_Bisnetos.ForEach(s => { s.SFuncao = "EXCLUIR ITEM"; s.bLiberado = false; });
            }
            else
            {
                listServicos_Recursos.Clear();
                listServicos_Recursos_Composicao_Filhos.Clear();
                listServicos_Recursos_Composicao_Netos.Clear();
                listServicos_Recursos_Composicao_Bisnetos.Clear();
            }

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_SERVICOS_RECURSOS_x_ORCAMENTO" },
                { "@idTipoOrcamento", idTipoOrcamento },
                { "@sidTipoServicos", hddidTiposServicos.Value.Length > 1 ? hddidTiposServicos.Value : "" }
            };
            DataSet ds = ExecutarDataSet(sProcedure_Tipo, vParametros);

            if (ValidarDataSet(ds))
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var item = listServicos_Recursos.FirstOrDefault(s => s.IdItem == int.Parse(row["idItem"].ToString()));

                    if (listServicos_Recursos.Count > 0 && item != null)
                    {
                        item.idRegistro = item.idRegistro > 0 ? item.idRegistro : GerarNovo_idRegistro(listServicos_Recursos);
                        item.SFuncao = "INCLUIR ITEM";
                        item.nOrdem = listServicos_Recursos.Count + 10;
                        item.SCodigo = row["sCodigo"].ToString();
                        item.SDscProduto = row["sDscProduto"].ToString();
                        item.TipoProduto = row["sTipo"].ToString();
                        item.idTipo = int.Parse(row["idTipo"].ToString());
                        item.IdGrupoProduto = int.Parse(row["idGrupo"].ToString());
                        item.IdFamiliaProduto = int.Parse(row["idFamilia"].ToString());
                        item.sDscGrupoProduto = row["sDscGrupo"].ToString();
                        item.sDscFamiliaProduto = row["sDscFamilia"].ToString();
                        item.SUnidade = row["sUnidade"].ToString();
                        item.NQuantidade = 1;
                        item.NFator = Math.Round(decimal.Parse(txtDescontoServico.Text), 2);
                        item.bLiberado = false;
                        item.bProjeto = row["sProjeto"].ToString().ToUpper().Equals("S");
                        item.dtInclusao = txtPrazoServico.Text;
                        item.idTipoRegra = int.Parse(row["idTipoRegra"].ToString());

                        PopulaComposicao(item, false);
                    }
                    else
                    {
                        var novoServico_Recurso = new cls_Comercial_Tabelas
                        {
                            idRegistro = GerarNovo_idRegistro(listServicos_Recursos),
                            SFuncao = "INCLUIR ITEM",
                            nOrdem = listServicos_Recursos.Count + 10,
                            IdItem = int.Parse(row["idItem"].ToString()),
                            SCodigo = row["sCodigo"].ToString(),
                            SDscProduto = row["sDscProduto"].ToString(),
                            TipoProduto = row["sTipo"].ToString(),
                            idTipo = int.Parse(row["idTipo"].ToString()),
                            IdGrupoProduto = int.Parse(row["idGrupo"].ToString()),
                            IdFamiliaProduto = int.Parse(row["idFamilia"].ToString()),
                            sDscGrupoProduto = row["sDscGrupo"].ToString(),
                            sDscFamiliaProduto = row["sDscFamilia"].ToString(),
                            SUnidade = row["sUnidade"].ToString(),
                            NQuantidade = 1,
                            NFator = Math.Round(decimal.Parse(txtDescontoServico.Text), 2),
                            bLiberado = false,
                            bProjeto = row["sProjeto"].ToString().ToUpper().Equals("S"),
                            dtInclusao = txtPrazoServico.Text,
                            idTipoRegra = int.Parse(row["idTipoRegra"].ToString())
                        };

                        listServicos_Recursos.Add(novoServico_Recurso);

                        PopulaComposicao(novoServico_Recurso, false);
                    }
                }

                div_pn3.Visible = true;
            }
            else
            {

                listServicos_Recursos.Clear();
                listServicos_Recursos_Composicao_Filhos.Clear();
                listServicos_Recursos_Composicao_Netos.Clear();
                listServicos_Recursos_Composicao_Bisnetos.Clear();

                if (!hddProgresso.Value.Contains("Servico"))
                    hddProgresso.Value = hddProgresso.Value.Replace("Servico|", "");

                div_pn3.Visible = false;
                div_recarregaServicos.Visible = true;
                MensagemPaginaServicos_Recursos.MostraMensagem_Aviso("<b>Aviso:</b> Não há Serviços disponíveis para este Tipo de Orçamento!", false);
            }

            gvServicos_Recursos_dataBind();
        }

        protected void gvServicos_Recursos_dataBind()
        {
            gvServicos_Recursos.DataSource = listServicos_Recursos.OrderBy(s => (!s.bLiberado, s.nOrdem));
            gvServicos_Recursos.DataBind();

            if (!hddProgresso.Value.Contains("Servico") && gvServicos_Recursos.Rows.Count > 0)
                hddProgresso.Value += "Servico|";
        }

        protected void gvServicos_Recursos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.FindControl("btnToggle").Visible = false;

                var servico = listServicos_Recursos.FirstOrDefault(s => s.idRegistro.ToString().Equals(((Label)e.Row.Cells[Servicos_Coluna__ID].FindControl("lblidRegistro")).Text));

                if (servico != null)
                {
                    bool bValidado = servico.bLiberado;

                    (e.Row.Cells[e.Row.Cells.Count - 1].FindControl("cbValidado") as CheckBox).Checked = bValidado;

                    if (bValidado)
                        e.Row.CssClass = "success";
                    else
                        e.Row.CssClass = "danger";

                    HtmlGenericControl div = e.Row.FindControl("divServico_Comparativo") as HtmlGenericControl;
                    HtmlGenericControl icone = e.Row.FindControl("iconeServico_Comparativo") as HtmlGenericControl;

                    if (servico.NAjuste > decimal.Zero)
                    {
                        div.Attributes["class"] = "positivo";
                        icone.Attributes["class"] = "fa fa-plus";
                    }
                    else if (servico.NAjuste == decimal.Zero)
                    {
                        div.Attributes["class"] = "";
                        icone.Attributes["class"] = "fa fa-plus";
                    }
                    else
                    {
                        div.Attributes["class"] = "negativo";
                        icone.Attributes["class"] = "fa fa-minus";
                    }

                    var gv = e.Row.FindControl("gvServicos_Recursos_Composicao_1") as GridView;

                    if (gv != null)
                    {
                        gv.DataSource = listServicos_Recursos_Composicao_Filhos.Where(s => s.idItemPai.Equals(servico.idRegistro) && s.idItemAvo.Equals(servico.idItemPai) && s.idItemBisavo.Equals(servico.idItemAvo));
                        gv.DataBind();

                        if (gv.Rows.Count > 0)
                            e.Row.FindControl("btnToggle").Visible = true;
                    }
                }
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_0";
        }

        protected void gvServicos_Recursos_Composicao_1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.FindControl("btnToggle").Visible = false;

                var subServico = listServicos_Recursos_Composicao_Filhos.FirstOrDefault(s => s.idRegistro.ToString() == ((Label)e.Row.Cells[Servicos_Composicao_1_Coluna__ID].FindControl("lblidRegistro")).Text);

                if (subServico != null)
                {
                    if (subServico.bLiberado)
                    {
                        (e.Row.Cells[e.Row.Cells.Count - 1].FindControl("cbValidado") as CheckBox).Checked = true;
                        e.Row.CssClass = "success";
                    }
                    else
                    {
                        (e.Row.Cells[e.Row.Cells.Count - 1].FindControl("cbValidado") as CheckBox).Checked = false;
                        e.Row.CssClass = "danger";
                    }

                    var gv = e.Row.FindControl("gvServicos_Recursos_Composicao_2") as GridView;

                    if (gv != null)
                    {
                        gv.DataSource = listServicos_Recursos_Composicao_Netos.Where(s => s.idItemPai.Equals(subServico.idRegistro) && s.idItemAvo.Equals(subServico.idItemPai) && s.idItemBisavo.Equals(subServico.idItemAvo));
                        gv.DataBind();

                        if (gv.Rows.Count > 0)
                            e.Row.FindControl("btnToggle").Visible = true;
                    }
                }

                e.Row.CssClass += " comp comp1";
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_1";
        }

        protected void gvServicos_Recursos_Composicao_2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.FindControl("btnToggle").Visible = false;

                var recurso = listServicos_Recursos_Composicao_Netos.FirstOrDefault(s => s.idRegistro.ToString() == ((Label)e.Row.Cells[Servicos_Composicao_2_Coluna__ID].FindControl("lblidRegistro")).Text);

                if (recurso != null)
                {
                    if (recurso.bLiberado)
                    {
                        (e.Row.Cells[e.Row.Cells.Count - 1].FindControl("cbValidado") as CheckBox).Checked = true;
                        e.Row.CssClass = "success";
                    }
                    else
                    {
                        (e.Row.Cells[e.Row.Cells.Count - 1].FindControl("cbValidado") as CheckBox).Checked = false;
                        e.Row.CssClass = "danger";
                    }

                    var gv = e.Row.FindControl("gvServicos_Recursos_Composicao_3") as GridView;

                    if (gv != null)
                    {
                        gv.DataSource = listServicos_Recursos_Composicao_Bisnetos.Where(s => s.idItemPai.Equals(recurso.idRegistro) && s.idItemAvo.Equals(recurso.idItemPai) && s.idItemBisavo.Equals(recurso.idItemAvo));
                        gv.DataBind();

                        if (gv.Rows.Count > 0)
                            e.Row.FindControl("btnToggle").Visible = true;
                    }
                }

                e.Row.CssClass += " comp comp2";
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_2";
        }

        protected void gvServicos_Recursos_Composicao_3_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var recurso = listServicos_Recursos_Composicao_Bisnetos.FirstOrDefault(s => s.idRegistro.ToString() == ((Label)e.Row.Cells[Servicos_Composicao_3_Coluna__ID].FindControl("lblidRegistro")).Text);

                if (recurso != null)
                {
                    if (recurso.bLiberado)
                    {
                        (e.Row.Cells[e.Row.Cells.Count - 1].FindControl("cbValidado") as CheckBox).Checked = true;
                        e.Row.CssClass = "success";
                    }
                    else
                    {
                        (e.Row.Cells[e.Row.Cells.Count - 1].FindControl("cbValidado") as CheckBox).Checked = false;
                        e.Row.CssClass = "danger";
                    }
                }

                e.Row.CssClass += " comp comp3";
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_3";
        }

        protected void gvControle_Margem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                (e.Row.Cells[ControleMargem_Coluna__ID].Controls[1] as Label).Text = HttpUtility.HtmlDecode((e.Row.Cells[ControleMargem_Coluna__ID].Controls[1] as Label).Text.Split('|')[0]);
                (e.Row.Cells[ControleMargem_Coluna__SubServico].Controls[1] as Label).Text = HttpUtility.HtmlDecode((e.Row.Cells[ControleMargem_Coluna__SubServico].Controls[1] as Label).Text.Split('|')[1]);
            }
        }

        protected void gvServicos_Recursos_View_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var servico = listServicos_Recursos.FirstOrDefault(s => s.idRegistro.ToString().Equals(e.Row.Cells[Servicos_Coluna__ID].Text));

                if (servico != null)
                {
                    var total = servico.NTotal;

                    HtmlGenericControl div = e.Row.FindControl("divServico_Comparativo") as HtmlGenericControl;
                    HtmlGenericControl icone = e.Row.FindControl("iconeServico_Comparativo") as HtmlGenericControl;

                    if (servico.NAjuste > decimal.Zero)
                    {
                        div.Attributes["class"] = "positivo";
                        icone.Attributes["class"] = "fa fa-plus";
                    }
                    else if (servico.NAjuste == decimal.Zero)
                    {
                        div.Attributes["class"] = "";
                        icone.Attributes["class"] = "fa fa-plus";
                    }
                    else
                    {
                        div.Attributes["class"] = "negativo";
                        icone.Attributes["class"] = "fa fa-minus";
                    }

                    var gv = e.Row.FindControl("gvComposicao_Servicos_View_1") as GridView;

                    if (gv != null)
                    {
                        gv.DataSource = listServicos_Recursos_Composicao_Filhos.Where(s => s.bLiberado && s.idItemPai.Equals(servico.idRegistro) && s.idItemAvo.Equals(servico.idItemPai) && s.idItemBisavo.Equals(servico.idItemAvo));
                        gv.DataBind();

                        if (gv.Rows.Count <= 0)
                            e.Row.FindControl("btnToggle").Visible = false;
                        else
                            e.Row.FindControl("btnToggle").Visible = true;
                    }
                    else
                        e.Row.FindControl("btnToggle").Visible = false;
                }
                else
                    e.Row.FindControl("btnToggle").Visible = false;
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_0";
        }

        protected void gvComposicao_Servicos_View_1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var servico = listServicos_Recursos_Composicao_Filhos.FirstOrDefault(s => s.idRegistro.ToString().Equals(e.Row.Cells[Servicos_Coluna__ID].Text));

                if (servico != null)
                {
                    var gv = e.Row.FindControl("gvComposicao_Servicos_View_2") as GridView;

                    if (gv != null)
                    {
                        gv.DataSource = listServicos_Recursos_Composicao_Netos.Where(s => s.bLiberado && s.idItemPai.Equals(servico.idRegistro) && s.idItemAvo.Equals(servico.idItemPai) && s.idItemBisavo.Equals(servico.idItemAvo));
                        gv.DataBind();

                        if (gv.Rows.Count <= 0)
                            e.Row.FindControl("btnToggle").Visible = false;
                        else
                            e.Row.FindControl("btnToggle").Visible = true;
                    }
                    else
                        e.Row.FindControl("btnToggle").Visible = false;
                }
                else
                    e.Row.FindControl("btnToggle").Visible = false;
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_1";
        }

        protected void gvComposicao_Servicos_View_2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var servico = listServicos_Recursos_Composicao_Netos.FirstOrDefault(s => s.idRegistro.ToString().Equals(e.Row.Cells[Servicos_Coluna__ID].Text));

                if (servico != null)
                {
                    var gv = e.Row.FindControl("gvComposicao_Servicos_View_3") as GridView;

                    if (gv != null)
                    {
                        gv.DataSource = listServicos_Recursos_Composicao_Bisnetos.Where(s => s.bLiberado && s.idItemPai.Equals(servico.idRegistro) && s.idItemAvo.Equals(servico.idItemPai) && s.idItemBisavo.Equals(servico.idItemAvo));
                        gv.DataBind();

                        if (gv.Rows.Count <= 0)
                            e.Row.FindControl("btnToggle").Visible = false;
                        else
                            e.Row.FindControl("btnToggle").Visible = true;
                    }
                    else
                        e.Row.FindControl("btnToggle").Visible = false;
                }
                else
                    e.Row.FindControl("btnToggle").Visible = false;
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_2";
        }

        protected void gvComposicao_Servicos_View_3_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_3";
        }

        #endregion

        #region | gvProdutos - Produtos

        protected void gvProdutos_dataBind()
        {
            lstProdutos_NaoCadastrados.Items.Clear();

            gvProdutos.DataSource = listProdutos.Where(p => p.bLiberado).OrderBy(p => (!p.bLiberado, p.nOrdem));
            gvProdutos.DataBind();

            if (gvProdutos.Rows.Count > 0)
            {
                if (!hddProgresso.Value.Contains("Produto")) hddProgresso.Value += "Produto|";

                if (listProdutos.Any(p => !p.bSistema)) gvProdutos.HeaderRow.Cells[Produtos_Coluna__Valor].Controls[1].Visible = true;
                else gvProdutos.HeaderRow.Cells[Produtos_Coluna__Valor].Controls[1].Visible = false;

                if (listProdutos.Where(p => p.bLiberado && p.IdItem > 0).GroupBy(p => p.IdItem).Any(g => g.Skip(1).Any())) gvProdutos.HeaderRow.FindControl("cmdUnificarTodos").Visible = true;
                else gvProdutos.HeaderRow.FindControl("cmdUnificarTodos").Visible = false;

                if (lstProdutos_NaoCadastrados.Items.Count > 0) gvProdutos.HeaderRow.FindControl("cmdVincularTodosProdutos").Visible = true;
                else gvProdutos.HeaderRow.FindControl("cmdVincularTodosProdutos").Visible = false;
            }
            else if (hddProgresso.Value.Contains("Produto")) hddProgresso.Value = hddProgresso.Value.Replace("Produto|", "");
        }

        protected void gvProdutos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.FindControl("btnToggle").Visible = false;

                    var produto = listProdutos.FirstOrDefault(p => p.idRegistro.Equals(int.Parse(e.Row.Cells[Produtos_Coluna__ID].Text)));

                    if (produto.bImportado)
                        e.Row.CssClass = "info";

                    if (produto.idRegra <= 0)
                        e.Row.CssClass = "warning";

                    if (produto.Preco <= decimal.Zero)
                        e.Row.CssClass = "danger";

                    if (produto.bSistema)
                    {
                        e.Row.Cells[Produtos_Composicao_Coluna__Ordem].Controls[3].Visible = true;
                        e.Row.Cells[Produtos_Coluna__Valor].Controls[3].Visible = false;
                    }
                    else
                    {
                        e.Row.Cells[Produtos_Composicao_Coluna__Ordem].Controls[3].Visible = false;
                        e.Row.Cells[Produtos_Coluna__Valor].Controls[3].Visible = true;
                    }

                    if (produto.IdItem.Equals(0))
                    {
                        lstProdutos_NaoCadastrados.Items.Add(new List_Item(string.Format("{0} - {1}", produto.SCodigo, produto.SDscProduto), produto.idRegistro.ToString()));

                        e.Row.FindControl("div_modaisCodigo").Visible = true;
                        e.Row.FindControl("cmdVincularProdutos").Visible = true;
                        e.Row.FindControl("cmdUnificar").Visible = false;
                        e.Row.FindControl("lnkAtualiza_Valor").Visible = false;
                    }
                    else
                    {
                        e.Row.FindControl("cmdVincularProdutos").Visible = false;
                        e.Row.FindControl("lnkAtualiza_Valor").Visible = true;

                        if (listProdutos.Where(p => p.IdItem.Equals(produto.IdItem) && p.bLiberado).Skip(1).Any())
                        {
                            e.Row.FindControl("div_modaisCodigo").Visible = true;
                            e.Row.FindControl("cmdUnificar").Visible = true;
                        }
                        else
                        {
                            e.Row.FindControl("div_modaisCodigo").Visible = false;
                            e.Row.FindControl("cmdUnificar").Visible = false;
                        }
                    }

                    try { (e.Row.FindControl("nUnitario") as TextBox).ReadOnly = !ValidaPermissao(Permissao.Comercial.Orcamento.AlterarValorComDesconto, false); } catch { }

                    var gv = e.Row.FindControl("gvProdutos_Composicao") as GridView;

                    if (gv != null)
                    {
                        gv.DataSource = listProdutos_Composicao.Where(p => p.idItemPai.Equals(produto.idRegistro)).OrderBy(p => p.nOrdem);
                        gv.DataBind();

                        if (gv.Rows.Count > 0)
                            e.Row.FindControl("btnToggle").Visible = true;
                    }
                }
                else if (e.Row.RowType == DataControlRowType.Header)
                    e.Row.CssClass = "cabecalho_Composicao_0";
            }
            catch { }
        }

        protected void gvProdutos_Composicao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_1";
        }

        protected void gvProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (!e.CommandName.Equals("Delete"))
                {
                    int.TryParse(e.CommandArgument.ToString(), out int id);

                    var produto = listProdutos.FirstOrDefault(p => p.idRegistro.Equals(id));

                    if (produto != null)
                    {
                        if (!produto.bImportado && !produto.bSistema && produto.IdItem > 0)
                            AtualizaValores_Produtos(produto);
                    }
                    else
                        throw new Exception("Produto não encontrado!");

                    MantemEtapa_Pos_PostBack(4);
                    AtualizaBarraProgresso(4, false);
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de Atualizar os Valores do Produto!<br />Erro ao atualizar os valores do Produto: " + ex.Message, false);
            }
        }

        protected void gv_Produtos_View_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var produto = listProdutos.FirstOrDefault(p => p.idRegistro.ToString().Equals(e.Row.Cells[Produtos_Coluna__ID].Text));

                if (produto.idRegra <= 0)
                    e.Row.CssClass = "warning";

                if (produto.bSistema)
                {
                    var gv = e.Row.FindControl("gv_Produtos_View_1") as GridView;

                    if (gv != null)
                    {
                        gv.DataSource = listProdutos_Composicao.Where(p => p.bLiberado && p.idItemPai.Equals(produto.idRegistro)).OrderBy(p => p.nOrdem);
                        gv.DataBind();

                        if (gv.Rows.Count <= 0)
                            e.Row.FindControl("btnToggle").Visible = false;
                        else
                            e.Row.FindControl("btnToggle").Visible = true;
                    }
                    else
                        e.Row.FindControl("btnToggle").Visible = false;
                }
                else
                    e.Row.FindControl("btnToggle").Visible = false;
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_0";
        }

        protected void gv_Produtos_View_1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_1";
        }

        #endregion

        #region | gvHistorico - Histórico

        protected void PopulaHistorico(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                aba_Historico.Visible = true;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_Historico", DataBindComScriptData(gvHistorico, dt, 2, new int[1] { 2 }, "desc", "false", "''"), true);
            }
            else aba_Historico.Visible = false;
        }

        #endregion

        #region | Comparativos

        #region | gvComparativoProdutos - Produtos

        protected void gvComparativoProdutos_dataBind()
        {
            if (Convert.ToBoolean(hddComparativos_Empreitada.Value))
            {
                gvComparativoProdutos.DataSource = listProdutos_Empreitada.OrderBy(p => p.nOrdem);
                gvComparativoProdutos.DataBind();

                // Coluna de Seleção de Produtos para o Comparativo de Produtos
                gvComparativoProdutos.Columns[0].HeaderStyle.CssClass = "id";
                gvComparativoProdutos.Columns[0].ItemStyle.CssClass = "id";

                // Colunas de Ordem
                // 2 - Ordem com botão de Composição
                // 3 - Apenas a Ordem
                gvComparativoProdutos.Columns[2].HeaderStyle.CssClass = "id";
                gvComparativoProdutos.Columns[2].ItemStyle.CssClass = "id";
                gvComparativoProdutos.Columns[3].HeaderStyle.CssClass = "";
                gvComparativoProdutos.Columns[3].ItemStyle.CssClass = "";

                // Colunas de Prazo
                // 9 - Prazo para Consulta
                // 10 - Editar Prazo
                gvComparativoProdutos.Columns[9].HeaderStyle.CssClass = "";
                gvComparativoProdutos.Columns[9].ItemStyle.CssClass = "";
                gvComparativoProdutos.Columns[10].HeaderStyle.CssClass = "id";
                gvComparativoProdutos.Columns[10].ItemStyle.CssClass = "id";
            }
            else
            {
                if (listProdutos_Comparativos != null && listProdutos_Comparativos.Any(p => p.bLiberado))
                {
                    gvComparativoProdutos.DataSource = listProdutos_Comparativos.Where(p => p.bLiberado).OrderBy(p => (!p.bLiberado, p.nOrdem));
                    gvComparativoProdutos.DataBind();

                    if (hddComparativoSistemas.Value.Equals("S"))
                    {
                        foreach (var sistema in listProdutos_Comparativos.Where(p => p.bSistema))
                        {
                            ddlSistemas_Comparativo.Items.Remove(ddlSistemas_Comparativo.Items.FindByValue(sistema.IdItem.ToString()));
                        }

                        if (ddlSistemas_Comparativo.Items.Count <= 1 || !listProdutos_Comparativos.Any(p => !p.bSistema))
                        {
                            gvComparativoProdutos.Columns[0].HeaderStyle.CssClass = "id";
                            gvComparativoProdutos.Columns[0].ItemStyle.CssClass = "id";

                            div_Converter_Comparativo.Visible = false;
                        }
                        else
                        {
                            gvComparativoProdutos.Columns[0].HeaderStyle.CssClass = "";
                            gvComparativoProdutos.Columns[0].ItemStyle.CssClass = "";

                            div_Converter_Comparativo.Visible = true;
                        }

                        // Colunas de Ordem
                        // 2 - Ordem com botão de Composição
                        // 3 - Apenas a Ordem
                        gvComparativoProdutos.Columns[2].HeaderStyle.CssClass = "";
                        gvComparativoProdutos.Columns[2].ItemStyle.CssClass = "";
                        gvComparativoProdutos.Columns[3].HeaderStyle.CssClass = "id";
                        gvComparativoProdutos.Columns[3].ItemStyle.CssClass = "id";

                        // Colunas de Prazo
                        // 9 - Prazo para Consulta
                        // 10 - Editar Prazo
                        gvComparativoProdutos.Columns[9].HeaderStyle.CssClass = "id";
                        gvComparativoProdutos.Columns[9].ItemStyle.CssClass = "id";
                        gvComparativoProdutos.Columns[10].HeaderStyle.CssClass = "";
                        gvComparativoProdutos.Columns[10].ItemStyle.CssClass = "";
                    }
                }
                else
                {
                    gvComparativoProdutos.DataSource = listProdutos.Where(p => p.bLiberado).OrderBy(p => (!p.bLiberado, p.nOrdem));
                    gvComparativoProdutos.DataBind();
                }
            }
        }

        protected void gvComparativoProdutos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.FindControl("btnToggle").Visible = false;

                var sistema = listProdutos_Comparativos.FirstOrDefault(p => p.bSistema && p.idRegistro.Equals(int.Parse(e.Row.Cells[Produtos_Comparativos_Coluna__ID].Text)));

                if (listProdutos_Comparativos.Count > 0 && sistema != null)
                {
                    e.Row.Cells[Produtos_Comparativos_Coluna__Ordem_Com_Composicao].Controls[3].Visible = true;
                    e.Row.Cells[Produtos_Comparativos_Coluna__CheckBox].Controls[1].Visible = false;
                }
                else
                {
                    sistema = listProdutos.FirstOrDefault(p => p.bSistema && p.idRegistro.Equals(int.Parse(e.Row.Cells[Produtos_Comparativos_Coluna__ID].Text)));

                    if (sistema != null)
                    {
                        e.Row.Cells[Produtos_Comparativos_Coluna__Ordem_Com_Composicao].FindControl("btnToggle").Visible = true;
                        e.Row.Cells[Produtos_Comparativos_Coluna__Ordem_Com_Composicao].FindControl("cmdDuplicarSistema").Visible = true;
                        e.Row.Cells[Produtos_Comparativos_Coluna__CheckBox].Controls[1].Visible = false;
                    }
                    else
                    {
                        e.Row.Cells[Produtos_Comparativos_Coluna__Ordem_Com_Composicao].FindControl("btnToggle").Visible = false;
                        e.Row.Cells[Produtos_Comparativos_Coluna__Ordem_Com_Composicao].FindControl("cmdDuplicarSistema").Visible = false;
                        e.Row.Cells[Produtos_Comparativos_Coluna__CheckBox].Controls[1].Visible = true;
                    }
                }

                if (!hddComparativoSistemas.Value.Equals("S") && !Convert.ToBoolean(hddComparativos_Empreitada.Value))
                {
                    (e.Row.Cells[Produtos_Comparativos_Coluna__Desconto].Controls[1] as TextBox).ReadOnly = true;
                    (e.Row.Cells[Produtos_Comparativos_Coluna__Valor_Desconto].Controls[1] as TextBox_Padrao).ReadOnly = true;
                }
                else
                {
                    (e.Row.Cells[Produtos_Comparativos_Coluna__Desconto].Controls[1] as TextBox).ReadOnly = false;
                    (e.Row.Cells[Produtos_Comparativos_Coluna__Valor_Desconto].Controls[1] as TextBox_Padrao).ReadOnly = false;
                }

                var gv = e.Row.FindControl("gvProdutos_Composicao_Comparativos") as GridView;

                if (gv != null && sistema != null)
                {
                    gv.DataSource = listProdutos_Comparativos_Composicao.Where(s => s.idItemPai.Equals(sistema.idRegistro));
                    gv.DataBind();

                    if (gv.Rows.Count > 0)
                        e.Row.FindControl("btnToggle").Visible = true;
                }
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_0";
        }

        protected void gvProdutos_Composicao_Comparativos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "cabecalho_Composicao_1";
        }

        #endregion

        #region | gvComparativoServicos - Serviços

        protected void gvComparativoServicos_dataBind()
        {
            if (Convert.ToBoolean(hddComparativos_Empreitada.Value))
            {
                gvComparativoServicos.DataSource = listServicos_Empreitada.Where(p => p.bLiberado).OrderBy(s => s.nOrdem);
                gvComparativoServicos.DataBind();

                gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 1].HeaderStyle.CssClass = "";
                gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 1].ItemStyle.CssClass = "";
            }
            else
            {
                if (listServicos_Comparativos != null && listServicos_Comparativos.Any(p => p.bLiberado))
                {
                    gvComparativoServicos.DataSource = listServicos_Comparativos.Where(p => p.bLiberado).OrderBy(p => (!p.bLiberado, p.nOrdem));
                    gvComparativoServicos.DataBind();

                    gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 1].HeaderStyle.CssClass = "id";
                    gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 1].ItemStyle.CssClass = "id";
                }
            }

            // Colunas de Total
            // -3 = Editar Total
            // -2 = Consultar Total
            gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 3].HeaderStyle.CssClass = "";
            gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 3].ItemStyle.CssClass = "";
            gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 2].HeaderStyle.CssClass = "id";
            gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 2].ItemStyle.CssClass = "id";
        }

        protected void gvComparativoServicos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var servico = listServicos_Comparativos.FirstOrDefault(s => s.idRegistro.ToString().Equals(e.Row.Cells[Servicos_Comparativos_Coluna__ID].Text));

                if (Convert.ToBoolean(hddComparativos_Empreitada.Value))
                    servico = listServicos_Empreitada.FirstOrDefault(s => s.idRegistro.ToString().Equals(e.Row.Cells[Servicos_Comparativos_Coluna__ID].Text));

                if (servico != null)
                {
                    HtmlGenericControl div = e.Row.FindControl("divServico_Comparativo") as HtmlGenericControl;
                    HtmlGenericControl icone = e.Row.FindControl("iconeServico_Comparativo") as HtmlGenericControl;

                    decimal totalServico = Math.Round((servico.Preco * servico.NMargem) - (servico.Preco * servico.NMargem * (servico.NFator / 100)), 2);
                    decimal totalAjustado = servico.NTotal;
                    decimal ajusteCompleto = totalAjustado > decimal.Zero && totalServico > decimal.Zero ? totalAjustado / totalServico : 0;

                    if (ajusteCompleto > decimal.One)
                    {
                        div.Attributes["class"] = "positivo";
                        icone.Attributes["class"] = "fa fa-plus";
                    }
                    else if (ajusteCompleto == decimal.One || servico.NAjuste == decimal.Zero)
                    {
                        div.Attributes["class"] = "";
                        icone.Attributes["class"] = "fa fa-plus";
                    }
                    else
                    {
                        div.Attributes["class"] = "negativo";
                        icone.Attributes["class"] = "fa fa-minus";
                    }

                    if (e.Row.Cells[Servicos_Comparativos_Coluna__Valor].Text.Length > 0 && decimal.Parse(e.Row.Cells[Servicos_Comparativos_Coluna__Valor].Text) <= decimal.Zero)
                        (e.Row.FindControl("nTotalServicos_Comparativos") as TextBox).ReadOnly = true;
                    else
                        (e.Row.FindControl("nTotalServicos_Comparativos") as TextBox).ReadOnly = false;
                }
            }
        }

        protected void gvComparativoServicos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                var servico = listServicos_Empreitada.FirstOrDefault(s => s.idRegistro.ToString().Equals(gvComparativoServicos.Rows[e.RowIndex].Cells[Servicos_Comparativos_Coluna__ID].Text));

                if (servico != null)
                {
                    foreach (var s in listServicos_Composicao_Filhos_Empreitada.Where(s => s.bLiberado && s.idItemPai.Equals(servico.idRegistro) && s.idItemAvo.Equals(servico.idItemPai) && s.idItemBisavo.Equals(servico.idItemAvo)))
                    {
                        s.SFuncao = "EXCLUIR ITEM";
                        s.bLiberado = false;
                    }

                    foreach (var s in listServicos_Composicao_Netos_Empreitada.Where(s => s.bLiberado && s.idItemAvo.Equals(servico.idItemPai) && s.idItemBisavo.Equals(servico.idItemAvo)))
                    {
                        s.SFuncao = "EXCLUIR ITEM";
                        s.bLiberado = false;
                    }

                    foreach (var s in listServicos_Composicao_Bisnetos_Empreitada.Where(s => s.bLiberado && s.idItemBisavo.Equals(servico.idItemAvo)))
                    {
                        s.SFuncao = "EXCLUIR ITEM";
                        s.bLiberado = false;
                    }

                    servico.SFuncao = "EXCLUIR ITEM";
                    servico.bLiberado = false;
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro ao Excluir o Serviço!<br /> Erro de Exclusão na Empreitada: " + ex.Message, false);
            }

            Scripts.Mantem_AbaAtiva(Page, "aba-Comparativo_Servicos");
            AtualizaClasses_Comparativo();
        }

        #endregion

        #endregion

        #region | Salvar

        /// <summary>
        /// Método utilizado para Salvar todas as informações do Orçamento no banco de Dados
        /// </summary>
        /// <param name="idOrcamento">Recebe o ID do Orçamento</param>
        /// <param name="bComparativo">Define se o Orçamento deve ser Salvo normalmente, ou utilizar uma configuração específica para os Comparativos.</param>
        protected void SalvarOrcamento(string idOrcamento, bool bComparativo)
        {
            int etapa = 5;

            try
            {
                etapa = hdd_ID_cmdSalvar.Value.Length > 0 ? hdd_ID_cmdSalvar.Value.Contains("_Sempre") ? int.Parse(hdd_ID_cmdSalvar.Value.Replace("cmdSalvar_Sempre_", "")) : 5 : 5;
            }
            catch { }

            try
            {
                string nRevisao = "0";

                try
                {
                    nRevisao = string.IsNullOrEmpty(txtRevisao_View.Text) || string.IsNullOrEmpty(txtRevisao_View.Text) ? "0" : txtRevisao_View.Text;

                    if (Convert.ToBoolean(Request["duplicar"]))
                        idOrcamento = "0";
                    else if (Convert.ToBoolean(Request["revisao"]))
                    {
                        idOrcamento = "0";
                        nRevisao = txtRevisao.Text;
                    }
                    else if (bComparativo)
                    {
                        idOrcamento = "0";
                        nRevisao = hddnRevisao.Value;
                    }
                }
                catch { }

                if (bComparativo)
                    AtualizaClasses_Comparativo();
                else
                    AtualizaBarraProgresso(0, false);

                if (bComparativo || ValidarDados(idOrcamento))
                {
                    if (hddAtualiza_IE_Parceiro.Value == "1" && !bComparativo)
                    {
                        Dictionary<string, string> vParametrosIE = new Dictionary<string, string>
                        {
                            { "@sFuncao", "ALTERAR_INSCRICAO_ESTADUAL" },
                            { "@idParceiro", hddidCliente.Value },
                            { "@sRG_IE", txtIE.Text }
                        };
                        ExecutarDataSet(sProcedure_Clientes, vParametrosIE);
                    }

                    decimal totalProdutos = 0, totalServicos = 0;
                    if (bComparativo)
                    {
                        if (Convert.ToBoolean(hddComparativos_Empreitada.Value)) totalProdutos += listProdutos_Empreitada.Sum(p => p.NTotal);
                        else if (hddComparativoProdutos.Value.Equals("S")) totalProdutos += listProdutos_Comparativos.Sum(p => p.NTotal);
                        else if (Convert.ToBoolean(hddsDrawback.Value)) totalProdutos += listProdutos.Sum(p => p.NTotal);
                    }
                    else totalProdutos += listProdutos.Where(p => p.bLiberado).Sum(p => p.NTotal);

                    if (Convert.ToBoolean(hddComparativos_Empreitada.Value)) totalServicos += listServicos_Empreitada.Sum(p => p.NTotal);
                    else totalServicos += listServicos_Recursos.Where(s => s.bLiberado).Sum(s => s.NTotal);

                    DateTime.TryParse(!bComparativo ? txtEstimativaEntrega.Text : txtEstimativa_View.Text, out DateTime dtPrevisaoEntrega);

                    Dictionary<string, string> vParametrosSalvar = new Dictionary<string, string>
                    {
                        { "@sFuncao", idOrcamento.Trim() == "0" ? "INCLUIR PEDIDO" : "ALTERAR PEDIDO" },
                        { "@idPedido", idOrcamento.Trim() },
                        { "@idTipo", "1" },
                        { "@idTipoOrcamento", !bComparativo ? ddlTipoOrcamento.SelectedValue : hddidTipoOrcamento.Value },
                        { "@idCliente", hddidCliente.Value },
                        { "@idFluxo", !bComparativo ? ddlFluxo.SelectedValue : hddidFluxo.Value },
                        { "@idVendedor", !bComparativo ?  ddlVendedor.SelectedValue : hddidVendedor.Value },
                        { "@idCondicaoDePagamento", !bComparativo ? ddlCondicaoPagamento.SelectedValue : hddidCondicaoPagamento.Value },
                        { "@idEmpresa", !bComparativo ? ddlEmpresa_Orcamento.SelectedValue : hddidEmpresa.Value },
                        { "@idEnderecoEntrega", !bComparativo ? ddlEndereco.SelectedValue : hddidEndereco_Fiscal.Value },
                        { "@nControleTT", !bComparativo ? txtControle_TT.Text : txtControle_TT_View.Text },
                        { "@sReferencia", !bComparativo ? txtReferencia.Text : txtReferencia_View.Text },
                        { "@dtPedido", hddDtPedido.Value },
                        { "@dtEstimativaEntrega", dtPrevisaoEntrega.ToString("dd/MM/yyyy") },
                        { "@sObservacao", !bComparativo ? txtObservacao.Text : txtObs_View.Text },
                        { "@idUsuarioInclusao", Variaveis.idUsuario().Trim() },
                        { "@idTipoEnvio", !bComparativo ? ddlFormaEnvio.SelectedValue : hddidFormaEnvio.Value },
                        { "@nVlrProdutos", Math.Round(totalProdutos, 2).ToString().Replace(".", "").Replace(',', '.').Trim() },
                        { "@nVlrServicos", Math.Round(totalServicos, 2).ToString().Replace(".", "").Replace(',', '.').Trim() },
                        { "@sConfidencial", !bComparativo ? ddlConfidencial.SelectedValue : txtConfidencial_View.Text.ToUpper().StartsWith("S") ? "S" : "N" },
                        { "@sEmpresaTransporte", !bComparativo ? txtTransporte.Text : txtTransporte_View.Text },
                        { "@nFretePrevisto", !bComparativo ? Math.Round(decimal.Parse(string.IsNullOrEmpty(txtFrete.Text) ? "0,00" : txtFrete.Text), 2).ToString().Replace(".", "").Replace(',', '.').Trim() : Math.Round(decimal.Parse(string.IsNullOrEmpty(txtFrete_View.Text) ? "0,00" : txtFrete_View.Text), 2).ToString().Replace(".", "").Replace(',', '.').Trim() },
                        { "@idTabelaPreco", !bComparativo ? ddlTabela.SelectedValue : hddidTabela.Value },
                        { "@sAlteracaoTabelaPreco_Obs", !bComparativo ? txtTabelaObs.Text : txtTabela_Obs_View.Text },
                        { "@idVersao", nRevisao },
                        { "@sDestinoVenda", !bComparativo ? ddlDestinoVenda.SelectedValue : hddidDestinoVenda.Value },
                        { "@idEnderecoDestino", !bComparativo ? ddlEndereco_Entrega.SelectedValue : hddidEndereco_Entrega.Value },
                        { "@sUF_Entrega", !bComparativo ? hddUF_Entrega_SelectedValue.Value.ToUpper() : txtUF_Entrega_View.Text.ToUpper() },
                        { "@nDiasPrevisao", !bComparativo ? txtDiasPrevisao.Text : txtDiasPrevisao_View.Text },
                        { "@sUF_Fiscal", !bComparativo ?  txtUF_Fiscal.Text.ToUpper() : txtUF_Fiscal_View.Text.ToUpper() },
                        { "@idCidade_Entrega", hddMunicipio_Entrega_SelectedValue.Value },
                        { "@nValidadeOrcamento", !bComparativo ? txtValidade.Text.Length > 0 ? txtValidade.Text : "5" : txtValidade_View.Text.Length > 0 ? txtValidade_View.Text : "5" },
                        { "@idContato_Cliente", !bComparativo ? ddlContato.SelectedValue : hddidContato.Value },
                        { "@idTipoCliente", !bComparativo ? ddlTipoCliente.SelectedValue : hddidTipoCliente.Value },
                        { "@sidSegmentosCliente", hddidSegmentos.Value },
                        { "@sidTiposServicos", hddidTiposServicos.Value },
                        { "@sidEscopos", hddidEscopos.Value },
                        { "@sAlteracaoItens", "S" },
                        { "@sComparativos", !Convert.ToBoolean(hddComparativos_Empreitada.Value) && bComparativo ? "S" : "N" },
                        { "@sEmpreitada", Convert.ToBoolean(hddComparativos_Empreitada.Value) ? "S" : "N" },
                        { "@sDuplicado", Convert.ToBoolean(Request["duplicar"]) ? "S" : "N" },
                        { "@sTipoDrawback", hddsTipoDrawback.Value ?? "" },
                        { "@sAlteracaoCondPag_Motivo", !bComparativo ? txtNovaCondPgto_Motivo.Text : txtNovaCondPgto_Motivo_View.Text },
                        { "@idInstalador", ddlidInstalador.SelectedValue },
                        { "@idTipoMoeda", ddlMoeda.SelectedValue },
                        { "@nCusto_Aduaneiro", txtCusto_Aduaneiro.Text.StringToDecimalString() },
                        { "@nCusto_Despachante", txtCusto_Despachante.Text.StringToDecimalString() }
                    };

                    string revisao = txtRevisao_View.Text.Trim();
                    if (Convert.ToBoolean(Request["revisao"]) || bComparativo || !(string.IsNullOrEmpty(revisao) || revisao == "0"))
                    {
                        vParametrosSalvar.Add("@nNumeroPedido", hddNumeroPedido.Value);
                        vParametrosSalvar.Add("@idCRM", hddVincula_CRM.Value);
                    }
                    else if (!string.IsNullOrEmpty(Request["crm"])) vParametrosSalvar.Add("@idCRM", Request["crm"]);
                    else if (!string.IsNullOrEmpty(Request["idCotacao"])) vParametrosSalvar.Add("@idCotacao", hddVincula_Cotacao.Value);

                    DataSet dsSalvar = ExecutarDataSet(sProcedure, vParametrosSalvar);

                    if (ValidarDataSet(dsSalvar))
                    {
                        string idItem = DATASET(dsSalvar, 0, 0, "idPedido");
                        int.TryParse(hddPedidoVinculado.Value, out int idPedidoVinculado);

                        if (SalvarProdutos(idItem, bComparativo) && SalvarServicos_Recursos(idItem, bComparativo, idPedidoVinculado > 0) && SalvarEscopos(idItem))
                            DirecionaPagina(string.Format("App/Paginas/Comercial/Orcamento_Detalhe.aspx?id={0}&msg=1", idItem));
                        else
                        {
                            MensagemPagina(etapa, "ERRO", "Houve um erro na tentativa de Salvar os dados de Produtos e Serviços!");
                            MantemEtapa_Pos_PostBack(etapa);
                        }
                    }
                    else
                    {
                        MensagemPagina(etapa, "ERRO", DATASET(dsSalvar, "msg").Length > 0 ? DATASET(dsSalvar, "msg") : "Houve um erro ao recuperar os dados Salvos do Orçamento!");
                        MantemEtapa_Pos_PostBack(etapa);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina(etapa, "ERRO", "Houve um erro na tentativa de Salvar os dados! <br />Erro: " + ex.Message);

                if (bComparativo)
                    AlinhaPagina();
                else
                    MantemEtapa_Pos_PostBack(etapa);
            }
        }

        /// <summary>
        /// Função utilizada para Salvar as informações dos Serviços e suas Composições no banco de Dados
        /// </summary>
        /// <param name="idOrcamento">Recebe o ID do Orçamento</param>
        /// <returns><b>True:</b> Informações dos Serviços e suas Composições Salvas com sucesso.<br />
        /// <b>False:</b> Informações dos Serviços e suas Composições <b>não</b> Salvas por Erro.</returns>
        protected bool SalvarServicos_Recursos(string idOrcamento, bool bComparativo, bool bPedido)
        {
            try
            {
                if (bComparativo)
                {
                    if (Convert.ToBoolean(hddComparativos_Empreitada.Value))
                    {
                        if (listServicos_Empreitada.Count > 0)
                        {
                            foreach (cls_Comercial_Tabelas servico_Recurso in listServicos_Empreitada)
                            {
                                Dictionary<string, string> vParametrosSalvarServicos_Recursos = new Dictionary<string, string>
                                {
                                    { "@sFuncao", servico_Recurso.SFuncao },
                                    { "@idPedido", idOrcamento },
                                    { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : servico_Recurso.idRegistro.ToString() },
                                    { "@sCodigo", servico_Recurso.SCodigo },
                                    { "@sDscProduto", servico_Recurso.SDscProduto },
                                    { "@sUnidade", servico_Recurso.SUnidade },
                                    { "@nQuantidade", Math.Round(servico_Recurso.NQuantidade, 2).ToString().Replace(",", ".") },
                                    { "@nValorUnitario", Math.Round(servico_Recurso.Preco, 2).ToString().Replace(",", ".") },
                                    { "@nDesconto", Math.Round(servico_Recurso.NFator, 2).ToString().Replace(",", ".") },
                                    { "@nMargem", Math.Round(servico_Recurso.NMargem, 2).ToString().Replace(",", ".") },
                                    { "@nAjuste", Math.Round(servico_Recurso.NAjuste, 2).ToString().Replace(",", ".") },
                                    { "@nValorTotal", Math.Round(servico_Recurso.NTotal, 2).ToString().Replace(",", ".") },
                                    { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                    { "@dtPrevisaoEntrega", DateTime.Parse(hddDtPedido.Value).AddDays(double.Parse(servico_Recurso.dtInclusao)).ToString() }
                                };

                                DataSet dsServicos = ExecutarDataSet(sProcedure, vParametrosSalvarServicos_Recursos);

                                if (ValidarDataSet(dsServicos))
                                {
                                    int.TryParse(DATASET(dsServicos, "idRegistro"), out int id);

                                    if (id > 0)
                                    {
                                        foreach (var p in listServicos_Composicao_Filhos_Empreitada.Where(p => p.idItemPai > 0 && p.idItemPai.Equals(servico_Recurso.idRegistro) && p.idItemAvo.Equals(servico_Recurso.idItemPai) && p.idItemBisavo.Equals(servico_Recurso.idItemAvo)))
                                        {
                                            p.idItemPai = id;
                                        }

                                        foreach (var p in listServicos_Composicao_Netos_Empreitada.Where(p => p.idItemAvo.Equals(servico_Recurso.idRegistro) && p.idItemBisavo.Equals(servico_Recurso.idItemPai)))
                                        {
                                            p.idItemAvo = id;
                                        }

                                        foreach (var p in listServicos_Composicao_Bisnetos_Empreitada.Where(p => p.idItemBisavo.Equals(servico_Recurso.idRegistro)))
                                        {
                                            p.idItemBisavo = id;
                                        }
                                    }
                                }
                            }
                        }
                        if (listServicos_Composicao_Filhos_Empreitada.Count > 0)
                        {
                            foreach (cls_Comercial_Tabelas servico_Recurso in listServicos_Composicao_Filhos_Empreitada)
                            {
                                Dictionary<string, string> vParametrosSalvarServicos_Recursos = new Dictionary<string, string>
                                {
                                    { "@sFuncao", servico_Recurso.SFuncao },
                                    { "@idPedido", idOrcamento },
                                    { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : servico_Recurso.idRegistro.ToString() },
                                    { "@sCodigo", servico_Recurso.SCodigo },
                                    { "@sDscProduto", servico_Recurso.SDscProduto },
                                    { "@sUnidade", servico_Recurso.SUnidade },
                                    { "@nQuantidade", Math.Round(servico_Recurso.NQuantidade, 2).ToString().Replace(",", ".") },
                                    { "@nValorUnitario", Math.Round(servico_Recurso.Preco, 2).ToString().Replace(",", ".") },
                                    { "@nMargem", Math.Round(servico_Recurso.NMargem, 2).ToString().Replace(",", ".") },
                                    { "@nValorTotal", Math.Round(servico_Recurso.NTotal, 2).ToString().Replace(",", ".") },
                                    { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                    { "@idProdutoPai", servico_Recurso.idItemPai.ToString() }
                                };

                                DataSet dsServicos = ExecutarDataSet(sProcedure, vParametrosSalvarServicos_Recursos);

                                if (ValidarDataSet(dsServicos))
                                {
                                    int.TryParse(DATASET(dsServicos, "idRegistro"), out int id);

                                    if (id > 0)
                                    {
                                        foreach (var p in listServicos_Composicao_Netos_Empreitada.Where(p => p.idItemPai > 0 && p.idItemPai.Equals(servico_Recurso.idRegistro) && p.idItemAvo.Equals(servico_Recurso.idItemPai) && p.idItemBisavo.Equals(servico_Recurso.idItemAvo)))
                                        {
                                            p.idItemPai = id;
                                        }

                                        foreach (var p in listServicos_Composicao_Bisnetos_Empreitada.Where(p => p.idItemAvo.Equals(servico_Recurso.idRegistro) && p.idItemBisavo.Equals(servico_Recurso.idItemPai)))
                                        {
                                            p.idItemAvo = id;
                                        }
                                    }
                                }
                            }
                        }
                        if (listServicos_Composicao_Netos_Empreitada.Count > 0)
                        {
                            foreach (cls_Comercial_Tabelas servico_Recurso in listServicos_Composicao_Netos_Empreitada)
                            {
                                Dictionary<string, string> vParametrosSalvarServicos_Recursos = new Dictionary<string, string>
                                {
                                    { "@sFuncao", servico_Recurso.SFuncao },
                                    { "@idPedido", idOrcamento },
                                    { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : servico_Recurso.idRegistro.ToString() },
                                    { "@sCodigo", servico_Recurso.SCodigo },
                                    { "@sDscProduto", servico_Recurso.SDscProduto },
                                    { "@sUnidade", servico_Recurso.SUnidade },
                                    { "@nQuantidade", Math.Round(servico_Recurso.NQuantidade, 2).ToString().Replace(",", ".") },
                                    { "@nValorUnitario", Math.Round(servico_Recurso.Preco, 2).ToString().Replace(",", ".") },
                                    { "@nMargem", Math.Round(servico_Recurso.NMargem, 2).ToString().Replace(",", ".") },
                                    { "@nValorTotal", Math.Round(servico_Recurso.NTotal, 2).ToString().Replace(",", ".") },
                                    { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                    { "@idProdutoPai", servico_Recurso.idItemPai.ToString() },
                                    { "@idProdutoAvo", servico_Recurso.idItemAvo.ToString() }
                                };

                                DataSet dsServicos = ExecutarDataSet(sProcedure, vParametrosSalvarServicos_Recursos);

                                if (ValidarDataSet(dsServicos))
                                {
                                    int.TryParse(DATASET(dsServicos, "idRegistro"), out int id);

                                    if (id > 0)
                                    {
                                        foreach (var p in listServicos_Composicao_Bisnetos_Empreitada.Where(p => p.idItemPai > 0 && p.idItemPai.Equals(servico_Recurso.idRegistro) && p.idItemAvo.Equals(servico_Recurso.idItemPai) && p.idItemBisavo.Equals(servico_Recurso.idItemAvo)))
                                        {
                                            p.idItemPai = id;
                                        }
                                    }
                                }
                            }
                        }
                        if (listServicos_Composicao_Bisnetos_Empreitada.Count > 0)
                        {
                            foreach (cls_Comercial_Tabelas servico_Recurso in listServicos_Composicao_Bisnetos_Empreitada)
                            {
                                Dictionary<string, string> vParametrosSalvarServicos_Recursos = new Dictionary<string, string>
                                {
                                    { "@sFuncao", servico_Recurso.SFuncao },
                                    { "@idPedido", idOrcamento },
                                    { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : servico_Recurso.idRegistro.ToString() },
                                    { "@sCodigo", servico_Recurso.SCodigo },
                                    { "@sDscProduto", servico_Recurso.SDscProduto },
                                    { "@sUnidade", servico_Recurso.SUnidade },
                                    { "@nQuantidade", Math.Round(servico_Recurso.NQuantidade, 2).ToString().Replace(",", ".") },
                                    { "@nValorUnitario", Math.Round(servico_Recurso.Preco, 2).ToString().Replace(",", ".") },
                                    { "@nMargem", Math.Round(servico_Recurso.NMargem, 2).ToString().Replace(",", ".") },
                                    { "@nValorTotal", Math.Round(servico_Recurso.NTotal, 2).ToString().Replace(",", ".") },
                                    { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                    { "@idProdutoPai", servico_Recurso.idItemPai.ToString() },
                                    { "@idProdutoAvo", servico_Recurso.idItemAvo.ToString() },
                                    { "@idProdutoBisavo", servico_Recurso.idItemBisavo.ToString() }
                                };
                                ExecutarDataSet(sProcedure, vParametrosSalvarServicos_Recursos);
                            }
                        }
                    }
                    else
                    {
                        if (listServicos_Comparativos.Count > 0)
                        {
                            foreach (cls_Comercial_Tabelas servico_Recurso in listServicos_Comparativos)
                            {
                                Dictionary<string, string> vParametrosSalvarServicos_Recursos = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "INCLUIR ITEM" },
                                    { "@idPedido", idOrcamento },
                                    { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : servico_Recurso.idRegistro.ToString() },
                                    { "@sCodigo", servico_Recurso.SCodigo },
                                    { "@sDscProduto", servico_Recurso.SDscProduto },
                                    { "@sUnidade", servico_Recurso.SUnidade },
                                    { "@nQuantidade", Math.Round(servico_Recurso.NQuantidade, 2).ToString().Replace(",", ".") },
                                    { "@nValorUnitario", Math.Round(servico_Recurso.Preco, 2).ToString().Replace(",", ".") },
                                    { "@nDesconto", Math.Round(servico_Recurso.NFator, 2).ToString().Replace(",", ".") },
                                    { "@nMargem", Math.Round(servico_Recurso.NMargem, 2).ToString().Replace(",", ".") },
                                    { "@nAjuste", Math.Round(servico_Recurso.NAjuste, 2).ToString().Replace(",", ".") },
                                    { "@nValorTotal", Math.Round(servico_Recurso.NTotal, 2).ToString().Replace(",", ".") },
                                    { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                    { "@dtPrevisaoEntrega", DateTime.Parse(hddDtPedido.Value).AddDays(double.Parse(servico_Recurso.dtInclusao)).ToString() }
                                };
                                DataSet dsServicos = ExecutarDataSet(sProcedure, vParametrosSalvarServicos_Recursos);

                                if (ValidarDataSet(dsServicos))
                                {
                                    int.TryParse(DATASET(dsServicos, "idRegistro"), out int id);

                                    if (id > 0)
                                    {
                                        foreach (var p in listServicos_Recursos_Composicao_Filhos.Where(p => p.idItemPai > 0 && p.idItemPai.Equals(servico_Recurso.idRegistro) && p.idItemAvo.Equals(servico_Recurso.idItemPai) && p.idItemBisavo.Equals(servico_Recurso.idItemAvo)))
                                        {
                                            p.idItemPai = id;
                                        }

                                        foreach (var p in listServicos_Recursos_Composicao_Netos.Where(p => p.idItemAvo.Equals(servico_Recurso.idRegistro) && p.idItemBisavo.Equals(servico_Recurso.idItemPai)))
                                        {
                                            p.idItemAvo = id;
                                        }

                                        foreach (var p in listServicos_Recursos_Composicao_Bisnetos.Where(p => p.idItemBisavo.Equals(servico_Recurso.idRegistro)))
                                        {
                                            p.idItemBisavo = id;
                                        }

                                        servico_Recurso.idRegistro = id;
                                    }
                                }
                            }
                        }
                        if (listServicos_Recursos_Composicao_Filhos.Count > 0)
                        {
                            foreach (cls_Comercial_Tabelas servico_Recurso in listServicos_Recursos_Composicao_Filhos.Where(ss => listServicos_Comparativos.Exists(s => s.idRegistro.Equals(ss.idItemPai))))
                            {
                                Dictionary<string, string> vParametrosSalvarServicos_Recursos = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "INCLUIR ITEM" },
                                    { "@idPedido", idOrcamento },
                                    { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : servico_Recurso.idRegistro.ToString() },
                                    { "@sCodigo", servico_Recurso.SCodigo },
                                    { "@sDscProduto", servico_Recurso.SDscProduto },
                                    { "@sUnidade", servico_Recurso.SUnidade },
                                    { "@nQuantidade", Math.Round(servico_Recurso.NQuantidade, 2).ToString().Replace(",", ".") },
                                    { "@nValorUnitario", Math.Round(servico_Recurso.Preco, 2).ToString().Replace(",", ".") },
                                    { "@nMargem", Math.Round(servico_Recurso.NMargem, 2).ToString().Replace(",", ".") },
                                    { "@nValorTotal", Math.Round(servico_Recurso.NTotal, 2).ToString().Replace(",", ".") },
                                    { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                    { "@idProdutoPai", servico_Recurso.idItemPai.ToString() }
                                };
                                DataSet dsServicos = ExecutarDataSet(sProcedure, vParametrosSalvarServicos_Recursos);

                                if (ValidarDataSet(dsServicos))
                                {
                                    int.TryParse(DATASET(dsServicos, "idRegistro"), out int id);

                                    if (id > 0)
                                    {
                                        foreach (var p in listServicos_Recursos_Composicao_Netos.Where(p => p.idItemPai > 0 && p.idItemPai.Equals(servico_Recurso.idRegistro) && p.idItemAvo.Equals(servico_Recurso.idItemPai) && p.idItemBisavo.Equals(servico_Recurso.idItemAvo)))
                                        {
                                            p.idItemPai = id;
                                        }

                                        foreach (var p in listServicos_Recursos_Composicao_Bisnetos.Where(p => p.idItemAvo.Equals(servico_Recurso.idRegistro) && p.idItemBisavo.Equals(servico_Recurso.idItemPai)))
                                        {
                                            p.idItemAvo = id;
                                        }

                                        servico_Recurso.idRegistro = id;
                                    }
                                }
                            }
                        }
                        if (listServicos_Recursos_Composicao_Netos.Count > 0)
                        {
                            foreach (cls_Comercial_Tabelas servico_Recurso in listServicos_Recursos_Composicao_Netos.Where(ss => listServicos_Comparativos.Exists(s => s.idRegistro.Equals(ss.idItemAvo)) && listServicos_Recursos_Composicao_Filhos.Exists(s => s.idRegistro.Equals(ss.idItemPai))))
                            {
                                Dictionary<string, string> vParametrosSalvarServicos_Recursos = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "INCLUIR ITEM" },
                                    { "@idPedido", idOrcamento },
                                    { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : servico_Recurso.idRegistro.ToString() },
                                    { "@sCodigo", servico_Recurso.SCodigo },
                                    { "@sDscProduto", servico_Recurso.SDscProduto },
                                    { "@sUnidade", servico_Recurso.SUnidade },
                                    { "@nQuantidade", Math.Round(servico_Recurso.NQuantidade, 2).ToString().Replace(",", ".") },
                                    { "@nValorUnitario", Math.Round(servico_Recurso.Preco, 2).ToString().Replace(",", ".") },
                                    { "@nMargem", Math.Round(servico_Recurso.NMargem, 2).ToString().Replace(",", ".") },
                                    { "@nValorTotal", Math.Round(servico_Recurso.NTotal, 2).ToString().Replace(",", ".") },
                                    { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                    { "@idProdutoPai", servico_Recurso.idItemPai.ToString() },
                                    { "@idProdutoAvo", servico_Recurso.idItemAvo.ToString() }
                                };

                                DataSet dsServicos = ExecutarDataSet(sProcedure, vParametrosSalvarServicos_Recursos);

                                if (ValidarDataSet(dsServicos))
                                {
                                    int.TryParse(DATASET(dsServicos, "idRegistro"), out int id);

                                    if (id > 0)
                                    {
                                        foreach (var p in listServicos_Recursos_Composicao_Bisnetos.Where(p => p.idItemPai > 0 && p.idItemPai.Equals(servico_Recurso.idRegistro) && p.idItemAvo.Equals(servico_Recurso.idItemPai) && p.idItemBisavo.Equals(servico_Recurso.idItemAvo)))
                                        {
                                            p.idItemPai = id;
                                        }

                                        servico_Recurso.idRegistro = id;
                                    }
                                }
                            }
                        }
                        if (listServicos_Recursos_Composicao_Bisnetos.Count > 0)
                        {
                            foreach (cls_Comercial_Tabelas servico_Recurso in listServicos_Recursos_Composicao_Bisnetos.Where(ss => listServicos_Comparativos.Exists(s => s.idRegistro.Equals(ss.idItemBisavo)) && listServicos_Recursos_Composicao_Filhos.Exists(s => s.idRegistro.Equals(ss.idItemAvo)) && listServicos_Recursos_Composicao_Netos.Exists(s => s.idRegistro.Equals(ss.idItemPai))))
                            {
                                Dictionary<string, string> vParametrosSalvarServicos_Recursos = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "INCLUIR ITEM" },
                                    { "@idPedido", idOrcamento },
                                    { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : servico_Recurso.idRegistro.ToString() },
                                    { "@sCodigo", servico_Recurso.SCodigo },
                                    { "@sDscProduto", servico_Recurso.SDscProduto },
                                    { "@sUnidade", servico_Recurso.SUnidade },
                                    { "@nQuantidade", Math.Round(servico_Recurso.NQuantidade, 2).ToString().Replace(",", ".") },
                                    { "@nValorUnitario", Math.Round(servico_Recurso.Preco, 2).ToString().Replace(",", ".") },
                                    { "@nMargem", Math.Round(servico_Recurso.NMargem, 2).ToString().Replace(",", ".") },
                                    { "@nValorTotal", Math.Round(servico_Recurso.NTotal, 2).ToString().Replace(",", ".") },
                                    { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                    { "@idProdutoPai", servico_Recurso.idItemPai.ToString() },
                                    { "@idProdutoAvo", servico_Recurso.idItemAvo.ToString() },
                                    { "@idProdutoBisavo", servico_Recurso.idItemBisavo.ToString() }
                                };
                                ExecutarDataSet(sProcedure, vParametrosSalvarServicos_Recursos);
                            }
                        }
                    }
                }
                else
                {
                    if (listServicos_Recursos.Count > 0)
                    {
                        foreach (cls_Comercial_Tabelas servico_Recurso in listServicos_Recursos)
                        {
                            decimal preco = servico_Recurso.Preco;
                            if (bPedido && hddMoeda.Value != "0")
                            {
                                decimal.TryParse(txtCambio_View.Text, out decimal nCambio);
                                preco = servico_Recurso.NTotal / nCambio;
                            }

                            Dictionary<string, string> vParametrosSalvarServicos_Recursos = new Dictionary<string, string>
                            {
                                { "@sFuncao", servico_Recurso.SFuncao },
                                { "@idPedido", idOrcamento },
                                { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : servico_Recurso.idRegistro.ToString() },
                                { "@sCodigo", servico_Recurso.SCodigo },
                                { "@sDscProduto", servico_Recurso.SDscProduto },
                                { "@sUnidade", servico_Recurso.SUnidade },
                                { "@nQuantidade", Math.Round(servico_Recurso.NQuantidade, 2).ToString().Replace(",", ".") },
                                { "@nValorUnitario", Math.Round(preco, 2).ToString().Replace(",", ".") },
                                { "@nDesconto", Math.Round(servico_Recurso.NFator, 2).ToString().Replace(",", ".") },
                                { "@nMargem", Math.Round(servico_Recurso.NMargem, 2).ToString().Replace(",", ".") },
                                { "@nAjuste", Math.Round(servico_Recurso.NAjuste, 2).ToString().Replace(",", ".") },
                                { "@nValorTotal", Math.Round(servico_Recurso.NTotal, 2).ToString().Replace(",", ".") },
                                { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                { "@dtPrevisaoEntrega", DateTime.Parse(hddDtPedido.Value).AddDays(double.Parse(servico_Recurso.dtInclusao)).ToString() }
                            };
                            DataSet dsServicos = ExecutarDataSet(sProcedure, vParametrosSalvarServicos_Recursos);

                            if (ValidarDataSet(dsServicos))
                            {
                                int.TryParse(DATASET(dsServicos, "idRegistro"), out int id);

                                if (id > 0)
                                {
                                    foreach (var p in listServicos_Recursos_Composicao_Filhos.Where(p => p.idItemPai > 0 && p.idItemPai.Equals(servico_Recurso.idRegistro) && p.idItemAvo.Equals(servico_Recurso.idItemPai) && p.idItemBisavo.Equals(servico_Recurso.idItemAvo)))
                                    {
                                        p.idItemPai = id;
                                    }

                                    foreach (var p in listServicos_Recursos_Composicao_Netos.Where(p => p.idItemAvo.Equals(servico_Recurso.idRegistro) && p.idItemBisavo.Equals(servico_Recurso.idItemPai)))
                                    {
                                        p.idItemAvo = id;
                                    }

                                    foreach (var p in listServicos_Recursos_Composicao_Bisnetos.Where(p => p.idItemBisavo.Equals(servico_Recurso.idRegistro)))
                                    {
                                        p.idItemBisavo = id;
                                    }
                                }
                            }
                        }
                    }
                    if (listServicos_Recursos_Composicao_Filhos.Count > 0 && !bPedido)
                    {
                        foreach (cls_Comercial_Tabelas servico_Recurso in listServicos_Recursos_Composicao_Filhos)
                        {
                            Dictionary<string, string> vParametrosSalvarServicos_Recursos = new Dictionary<string, string>
                            {
                                { "@sFuncao", servico_Recurso.SFuncao },
                                { "@idPedido", idOrcamento },
                                { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : servico_Recurso.idRegistro.ToString() },
                                { "@sCodigo", servico_Recurso.SCodigo },
                                { "@sDscProduto", servico_Recurso.SDscProduto },
                                { "@sUnidade", servico_Recurso.SUnidade },
                                { "@nQuantidade", Math.Round(servico_Recurso.NQuantidade, 2).ToString().Replace(",", ".") },
                                { "@nValorUnitario", Math.Round(servico_Recurso.Preco, 2).ToString().Replace(",", ".") },
                                { "@nMargem", Math.Round(servico_Recurso.NMargem, 2).ToString().Replace(",", ".") },
                                { "@nValorTotal", Math.Round(servico_Recurso.NTotal, 2).ToString().Replace(",", ".") },
                                { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                { "@idProdutoPai", servico_Recurso.idItemPai.ToString() }
                            };

                            DataSet dsServicos = ExecutarDataSet(sProcedure, vParametrosSalvarServicos_Recursos);

                            if (ValidarDataSet(dsServicos))
                            {
                                int.TryParse(DATASET(dsServicos, "idRegistro"), out int id);

                                if (id > 0)
                                {
                                    foreach (var p in listServicos_Recursos_Composicao_Netos.Where(p => p.idItemPai > 0 && p.idItemPai.Equals(servico_Recurso.idRegistro) && p.idItemAvo.Equals(servico_Recurso.idItemPai) && p.idItemBisavo.Equals(servico_Recurso.idItemAvo)))
                                    {
                                        p.idItemPai = id;
                                    }

                                    foreach (var p in listServicos_Recursos_Composicao_Bisnetos.Where(p => p.idItemAvo.Equals(servico_Recurso.idRegistro) && p.idItemBisavo.Equals(servico_Recurso.idItemPai)))
                                    {
                                        p.idItemAvo = id;
                                    }
                                }
                            }
                        }
                    }
                    if (listServicos_Recursos_Composicao_Netos.Count > 0 && !bPedido)
                    {
                        foreach (cls_Comercial_Tabelas servico_Recurso in listServicos_Recursos_Composicao_Netos)
                        {
                            Dictionary<string, string> vParametrosSalvarServicos_Recursos = new Dictionary<string, string>
                            {
                                { "@sFuncao", servico_Recurso.SFuncao },
                                { "@idPedido", idOrcamento },
                                { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : servico_Recurso.idRegistro.ToString() },
                                { "@sCodigo", servico_Recurso.SCodigo },
                                { "@sDscProduto", servico_Recurso.SDscProduto },
                                { "@sUnidade", servico_Recurso.SUnidade },
                                { "@nQuantidade", Math.Round(servico_Recurso.NQuantidade, 2).ToString().Replace(",", ".") },
                                { "@nValorUnitario", Math.Round(servico_Recurso.Preco, 2).ToString().Replace(",", ".") },
                                { "@nMargem", Math.Round(servico_Recurso.NMargem, 2).ToString().Replace(",", ".") },
                                { "@nValorTotal", Math.Round(servico_Recurso.NTotal, 2).ToString().Replace(",", ".") },
                                { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                { "@idProdutoPai", servico_Recurso.idItemPai.ToString() },
                                { "@idProdutoAvo", servico_Recurso.idItemAvo.ToString() }
                            };

                            DataSet dsServicos = ExecutarDataSet(sProcedure, vParametrosSalvarServicos_Recursos);

                            if (ValidarDataSet(dsServicos))
                            {
                                int.TryParse(DATASET(dsServicos, "idRegistro"), out int id);

                                if (id > 0)
                                {
                                    foreach (var p in listServicos_Recursos_Composicao_Bisnetos.Where(p => p.idItemAvo.Equals(servico_Recurso.idRegistro) && p.idItemBisavo.Equals(servico_Recurso.idItemPai)))
                                    {
                                        p.idItemPai = id;
                                    }
                                }
                            }
                        }
                    }
                    if (listServicos_Recursos_Composicao_Bisnetos.Count > 0 && !bPedido)
                    {
                        foreach (cls_Comercial_Tabelas servico_Recurso in listServicos_Recursos_Composicao_Bisnetos)
                        {
                            Dictionary<string, string> vParametrosSalvarServicos_Recursos = new Dictionary<string, string>
                            {
                                { "@sFuncao", servico_Recurso.SFuncao },
                                { "@idPedido", idOrcamento },
                                { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : servico_Recurso.idRegistro.ToString() },
                                { "@sCodigo", servico_Recurso.SCodigo },
                                { "@sDscProduto", servico_Recurso.SDscProduto },
                                { "@sUnidade", servico_Recurso.SUnidade },
                                { "@nQuantidade", Math.Round(servico_Recurso.NQuantidade, 2).ToString().Replace(",", ".") },
                                { "@nValorUnitario", Math.Round(servico_Recurso.Preco, 2).ToString().Replace(",", ".") },
                                { "@nMargem", Math.Round(servico_Recurso.NMargem, 2).ToString().Replace(",", ".") },
                                { "@nValorTotal", Math.Round(servico_Recurso.NTotal, 2).ToString().Replace(",", ".") },
                                { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                { "@idProdutoPai", servico_Recurso.idItemPai.ToString() },
                                { "@idProdutoAvo", servico_Recurso.idItemAvo.ToString() },
                                { "@idProdutoBisavo", servico_Recurso.idItemBisavo.ToString() }
                            };
                            ExecutarDataSet(sProcedure, vParametrosSalvarServicos_Recursos);
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Função utilizada para Salvar as informações dos Produtos no banco de Dados
        /// </summary>
        /// <param name="idOrcamento">Recebe o ID do Orçamento</param>
        /// <returns><b>True:</b> Informações dos Produtos Salvas com sucesso.<br />
        /// <b>False:</b> Informações dos Produtos <b>não</b> Salvas por Erro.</returns>
        protected bool SalvarProdutos(string idOrcamento, bool bComparativo, bool bPedido = false)
        {
            try
            {
                if (bComparativo)
                {
                    if (Convert.ToBoolean(hddComparativos_Empreitada.Value))
                    {
                        if (listProdutos_Empreitada.Count > 0)
                        {
                            foreach (cls_Comercial_Tabelas produto in listProdutos_Empreitada)
                            {
                                double.TryParse(produto.dtInclusao, out double dt);

                                Dictionary<string, string> vParametrosSalvarProdutos = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "INCLUIR ITEM" },
                                    { "@idPedido", idOrcamento },
                                    { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : produto.idRegistro.ToString() },
                                    { "@nOrdem", produto.nOrdem.ToString() },
                                    { "@sCodigo", produto.SCodigo },
                                    { "@sDscProduto", produto.SDscProduto },
                                    { "@sUnidade", produto.SUnidade },
                                    { "@nQuantidade", Math.Round(produto.NQuantidade, 2).ToString().Replace(",", ".") },
                                    { "@nValorUnitario", Math.Round(produto.Preco, 2).ToString().Replace(",", ".") },
                                    { "@nDesconto", Math.Round(produto.NFator, 4).ToString().Replace(",", ".") },
                                    { "@nValorTotal", Math.Round(produto.NTotal, 2).ToString().Replace(",", ".") },
                                    { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                    { "@dtPrevisaoEntrega", DateTime.Parse(hddDtPedido.Value).AddDays(dt).ToString() }
                                };
                                ExecutarDataSet(sProcedure, vParametrosSalvarProdutos);
                            }
                        }
                    }
                    else
                    {
                        if (listProdutos_Comparativos.Count > 0)
                        {
                            foreach (cls_Comercial_Tabelas produto in listProdutos_Comparativos)
                            {
                                double.TryParse(produto.dtInclusao, out double dt);

                                Dictionary<string, string> vParametrosSalvarProdutos = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "INCLUIR ITEM" },
                                    { "@idPedido", idOrcamento },
                                    { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : produto.idRegistro.ToString() },
                                    { "@nOrdem", produto.nOrdem.ToString() },
                                    { "@sCodigo", produto.SCodigo },
                                    { "@sDscProduto", produto.SDscProduto },
                                    { "@sUnidade", produto.SUnidade },
                                    { "@nQuantidade", Math.Round(produto.NQuantidade, 2).ToString().Replace(",", ".") },
                                    { "@nValorUnitario", Math.Round(produto.Preco, 2).ToString().Replace(",", ".") },
                                    { "@nDesconto", Math.Round(produto.NFator, 4).ToString().Replace(",", ".") },
                                    { "@nValorTotal", Math.Round(produto.NTotal, 2).ToString().Replace(",", ".") },
                                    { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                    { "@dtPrevisaoEntrega", DateTime.Parse(hddDtPedido.Value).AddDays(dt).ToString() }
                                };
                                DataSet dsProdutos = ExecutarDataSet(sProcedure, vParametrosSalvarProdutos);

                                if (ValidarDataSet(dsProdutos))
                                {
                                    int.TryParse(DATASET(dsProdutos, "idRegistro"), out int id);

                                    if (id > 0)
                                    {
                                        foreach (var p in listProdutos_Comparativos_Composicao.Where(p => p.idItemPai > 0 && p.idItemPai.Equals(produto.idRegistro)))
                                        {
                                            p.idItemPai = id;
                                        }
                                    }
                                }
                            }
                        }
                        else if (listProdutos.Count > 0)
                        {
                            foreach (cls_Comercial_Tabelas produto in listProdutos)
                            {
                                double.TryParse(produto.dtInclusao, out double dt);

                                Dictionary<string, string> vParametrosSalvarProdutos = new Dictionary<string, string>
                                {
                                    { "@sFuncao", produto.SFuncao },
                                    { "@idPedido", idOrcamento },
                                    { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : produto.idRegistro.ToString() },
                                    { "@nOrdem", produto.nOrdem.ToString() },
                                    { "@sCodigo", produto.SCodigo },
                                    { "@sDscProduto", produto.SDscProduto },
                                    { "@sUnidade", produto.SUnidade },
                                    { "@nQuantidade", Math.Round(produto.NQuantidade, 2).ToString().Replace(",", ".") },
                                    { "@nValorUnitario", Math.Round(produto.Preco, 2).ToString().Replace(",", ".") },
                                    { "@nDesconto", Math.Round(produto.NFator, 4).ToString().Replace(",", ".") },
                                    { "@nValorTotal", Math.Round(produto.NTotal, 2).ToString().Replace(",", ".") },
                                    { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                    { "@dtPrevisaoEntrega", DateTime.Parse(hddDtPedido.Value).AddDays(dt).ToString() }
                                };
                                ExecutarDataSet(sProcedure, vParametrosSalvarProdutos);
                            }
                        }

                        if (listProdutos_Comparativos_Composicao.Count > 0)
                        {
                            foreach (cls_Comercial_Tabelas produto in listProdutos_Comparativos_Composicao)
                            {
                                double.TryParse(produto.dtInclusao, out double dt);

                                Dictionary<string, string> vParametrosSalvarProdutos = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "INCLUIR ITEM" },
                                    { "@idPedido", idOrcamento },
                                    { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : produto.idRegistro.ToString() },
                                    { "@nOrdem", produto.nOrdem.ToString() },
                                    { "@sCodigo", produto.SCodigo },
                                    { "@sDscProduto", produto.SDscProduto },
                                    { "@sUnidade", produto.SUnidade },
                                    { "@nQuantidade", Math.Round(produto.NQuantidade, 2).ToString().Replace(",", ".") },
                                    { "@nValorUnitario", Math.Round(produto.Preco, 2).ToString().Replace(",", ".") },
                                    { "@nDesconto", Math.Round(produto.NFator, 4).ToString().Replace(",", ".") },
                                    { "@nValorTotal", Math.Round(produto.NTotal, 2).ToString().Replace(",", ".") },
                                    { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                    { "@dtPrevisaoEntrega", DateTime.Parse(hddDtPedido.Value).AddDays(dt).ToString() },
                                    { "@idProdutoPai", produto.idItemPai.ToString() }
                                };
                                ExecutarDataSet(sProcedure, vParametrosSalvarProdutos);
                            }
                        }
                    }
                }
                else
                {
                    if (listProdutos.Count > 0)
                    {
                        foreach (cls_Comercial_Tabelas produto in listProdutos)
                        {
                            double.TryParse(produto.dtInclusao, out double dt);

                            Dictionary<string, string> vParametrosSalvarProdutos = new Dictionary<string, string>
                            {
                                { "@sFuncao", produto.SFuncao },
                                { "@idPedido", idOrcamento },
                                { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : produto.idRegistro.ToString() },
                                { "@nOrdem", produto.nOrdem.ToString() },
                                { "@sCodigo", produto.SCodigo },
                                { "@sDscProduto", produto.SDscProduto },
                                { "@sUnidade", produto.SUnidade },
                                { "@nQuantidade", Math.Round(produto.NQuantidade, 2).ToString().Replace(",", ".") },
                                { "@nValorUnitario", Math.Round(produto.Preco, 2).ToString().Replace(",", ".") },
                                { "@nDesconto", Math.Round(produto.NFator, 4).ToString().Replace(",", ".") },
                                { "@nValorTotal", Math.Round(produto.NTotal, 2).ToString().Replace(",", ".") },
                                { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                { "@dtPrevisaoEntrega", DateTime.Parse(hddDtPedido.Value).AddDays(dt).ToString() }
                            };
                            DataSet dsProdutos = ExecutarDataSet(sProcedure, vParametrosSalvarProdutos);

                            if (ValidarDataSet(dsProdutos))
                            {
                                int.TryParse(DATASET(dsProdutos, "idRegistro"), out int id);

                                if (id > 0)
                                {
                                    foreach (var p in listProdutos_Composicao.Where(p => p.idItemPai > 0 && p.idItemPai.Equals(produto.idRegistro)))
                                    {
                                        p.idItemPai = id;
                                    }
                                }
                            }
                        }
                    }

                    if (listProdutos_Composicao.Count > 0)
                    {
                        foreach (cls_Comercial_Tabelas produto in listProdutos_Composicao)
                        {
                            double.TryParse(produto.dtInclusao, out double dt);

                            Dictionary<string, string> vParametrosSalvarProdutos = new Dictionary<string, string>
                            {
                                { "@sFuncao", produto.SFuncao },
                                { "@idPedido", idOrcamento },
                                { "@idItem", Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]) ? "0" : produto.idRegistro.ToString() },
                                { "@nOrdem", produto.nOrdem.ToString() },
                                { "@sCodigo", produto.SCodigo },
                                { "@sDscProduto", produto.SDscProduto },
                                { "@sUnidade", produto.SUnidade },
                                { "@nQuantidade", Math.Round(produto.NQuantidade, 2).ToString().Replace(",", ".") },
                                { "@nValorUnitario", Math.Round(produto.Preco, 2).ToString().Replace(",", ".") },
                                { "@nDesconto", Math.Round(produto.NFator, 4).ToString().Replace(",", ".") },
                                { "@nValorTotal", Math.Round(produto.NTotal, 2).ToString().Replace(",", ".") },
                                { "@idUsuarioInclusao", Variaveis.idUsuario() },
                                { "@dtPrevisaoEntrega", DateTime.Parse(hddDtPedido.Value).AddDays(dt).ToString() },
                                { "@idProdutoPai", produto.idItemPai.ToString() }
                            };
                            ExecutarDataSet(sProcedure, vParametrosSalvarProdutos);
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Função utilizada para Salvar as informações dos Escopos no banco de Dados
        /// </summary>
        /// <param name="idOrcamento">Recebe o ID do Orçamento</param>
        /// <returns><b>True:</b> Informações dos Escopos Salvas com sucesso.<br />
        /// <b>False:</b> Informações dos Escopos <b>não</b> Salvas por Erro.</returns>
        protected bool SalvarEscopos(string idOrcamento)
        {
            try
            {
                if (listServicos_Recursos.Any(s => s.bLiberado))
                {
                    foreach (cls_Categoria item in list_Perguntas_x_Opcoes)
                    {
                        Dictionary<string, string> vParametrosMatrizEscopo = new Dictionary<string, string>
                        {
                            { "@sFuncao", "SALVAR_MATRIZ_ESCOPO" },
                            { "@idOrcamento", idOrcamento },
                            { "@idEscopo", item.idEscopo.ToString() },
                            { "@idCategoria", item.idCategoria.ToString() },
                            { "@idOpcao", item.sOpcoes.Split('|')[0] },
                            { "@sPergunta", item.sPerguntas },
                            { "@sOpcao", item.sOpcoes.Split('|')[1] }
                        };

                        if (item.sPerguntas.Split('|').Length > 1)
                        {
                            vParametrosMatrizEscopo.Add("@idPergunta", item.sPerguntas.Split('|')[0]);
                            vParametrosMatrizEscopo["@sPergunta"] = item.sPerguntas.Split('|')[1];
                        }

                        ExecutarDataSet(sProcedure_Tipo, vParametrosMatrizEscopo);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region | Utils

        #region | Popula

        /// <summary>
        /// Método utilizado para Popular as Listas de Classes.
        /// </summary>
        /// <param name="ds">Recebe um DataSet com as informações do Orçamento e os Itens.</param>
        protected void PopulaClasses(DataSet ds)
        {
            listProdutos.Clear();
            listServicos_Recursos.Clear();
            listServicos_Recursos_Composicao_Filhos.Clear();
            listServicos_Recursos_Composicao_Netos.Clear();
            listServicos_Recursos_Composicao_Bisnetos.Clear();

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                var item = new cls_Comercial_Tabelas();
                string idDestino = DATASET(ds, "sDestinoVenda").Equals("C") ? "1" : DATASET(ds, "sDestinoVenda").Equals("R") ? "2" : "3";
                string sUFDestino = hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : DATASET(ds, "sUF_Fiscal").ToUpper().Trim();

                if (row["sTipoItem"].ToString().Equals("Servico_Recurso"))
                {
                    idDestino = row["idTipoRegra"].ToString();

                    try { item.idRegistro = int.Parse(row["idItem"].ToString()); } catch { item.idRegistro = GerarNovo_idRegistro(listServicos_Recursos); }
                    item.SFuncao = "INCLUIR ITEM";
                    item.nOrdem = listServicos_Recursos.Count(p => p.bLiberado) + 10;
                    item.IdItem = int.Parse(row["idProduto"].ToString());
                    item.SCodigo = row["sCodigo"].ToString();
                    item.SDscProduto = row["sDscProduto"].ToString();
                    item.TipoProduto = row["sTipo"].ToString();
                    item.idTipo = int.Parse(row["idTipo"].ToString());
                    item.SUnidade = row["sUnidade"].ToString();
                    item.IdGrupoProduto = int.Parse(row["idGrupo"].ToString());
                    item.IdFamiliaProduto = int.Parse(row["idFamilia"].ToString());
                    item.sDscGrupoProduto = row["sDscGrupo"].ToString();
                    item.sDscFamiliaProduto = row["sDscFamilia"].ToString();
                    item.NFator = Math.Round(decimal.Parse(row["nDesconto"].ToString()), 2);
                    item.NMargem = Math.Round(decimal.Parse(row["nMargem"].ToString()), 2);

                    item.Preco = Math.Round(decimal.Parse(row["nValorUnitario"].ToString()), 2);
                    item.NQuantidade = Math.Round(decimal.Parse(row["nQuantidade"].ToString()), 2);
                    item.NTotal = Math.Round(decimal.Parse(row["nValorTotal"].ToString()), 2);
                    item.bLiberado = true;

                    if (row["idProdutoBisavo"].ToString() != "0")
                    {
                        item.idItemPai = int.Parse(row["idProdutoPai"].ToString());
                        item.idItemAvo = int.Parse(row["idProdutoAvo"].ToString());
                        item.idItemBisavo = int.Parse(row["idProdutoBisavo"].ToString());

                        listServicos_Recursos_Composicao_Bisnetos.Add(item);
                        listServicos_Composicao_Bisnetos_Empreitada.Add(item);
                    }
                    else if (row["idProdutoAvo"].ToString() != "0")
                    {
                        item.idItemPai = int.Parse(row["idProdutoPai"].ToString());
                        item.idItemAvo = int.Parse(row["idProdutoAvo"].ToString());

                        listServicos_Recursos_Composicao_Netos.Add(item);
                        listServicos_Composicao_Netos_Empreitada.Add(item);
                    }
                    else if (row["idProdutoPai"].ToString() != "0")
                    {
                        item.idItemPai = int.Parse(row["idProdutoPai"].ToString());

                        listServicos_Recursos_Composicao_Filhos.Add(item);
                        listServicos_Composicao_Filhos_Empreitada.Add(item);
                    }
                    else
                    {
                        item.bProjeto = row["sProjeto"].ToString().ToUpper().Equals("S");

                        try
                        {
                            item.NAjuste = Math.Round(decimal.Parse(row["nAjuste"].ToString()), 2);

                            item.idRegra = int.Parse(row["idRegra"].ToString());
                            item.dtInclusao = row["dtPrevisaoEntrega"].ToString().Length > 0 || !row["dtPrevisaoEntrega"].ToString().Equals("01/01/1900 00:00:00") ? (DateTime.Parse(row["dtPrevisaoEntrega"].ToString()) - DateTime.Parse(hddDtPedido.Value)).Days.ToString().Length > 4 ? "15" : (DateTime.Parse(row["dtPrevisaoEntrega"].ToString()) - DateTime.Parse(hddDtPedido.Value)).Days.ToString() : "15";

                            DataSet dsRegras = ConsultaImpostos_Produtos(false, row["idProduto"].ToString(), DATASET(ds, "idEmpresa"), DATASET(ds, "idCliente"), idDestino, sUFDestino, "0", item.NTotal.ToString().Replace(',', '.'));

                            item.sNCM = DATASET(dsRegras, "sCodigoFederal");
                            item.sCST = DATASET(dsRegras, "sCodigoMunicipal");

                            item.NIPI = Math.Round(decimal.Parse(DATASET(dsRegras, "nISS")), 2);
                            item.nVlr_IPI = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrISS")), 2);
                            item.NPIS = Math.Round(decimal.Parse(DATASET(dsRegras, "nPIS")), 2);
                            item.nVlr_PIS = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrPIS")), 2);
                            item.NCOFINS = Math.Round(decimal.Parse(DATASET(dsRegras, "nCOFINS")), 2);
                            item.nVlr_COFINS = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrCOFINS")), 2);
                            item.NCSSL = Math.Round(decimal.Parse(DATASET(dsRegras, "nCSSL")), 2);
                            item.nVlr_CSSL = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrCSSL")), 2);
                            item.NIRPJ = Math.Round(decimal.Parse(DATASET(dsRegras, "nIR")), 2);
                            item.nVlr_IRPJ = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrIR")), 2);
                            item.NICMS = Math.Round(decimal.Parse(DATASET(dsRegras, "nINSS")), 2);
                            item.nVlr_ICMS = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrINSS")), 2);
                        }
                        catch { }

                        if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                        {
                            item.NIPI = 0;
                            item.nVlr_IPI = 0;
                            item.NPIS = 0;
                            item.nVlr_PIS = 0;
                            item.NCOFINS = 0;
                            item.nVlr_COFINS = 0;
                            item.NCSSL = 0;
                            item.nVlr_CSSL = 0;
                            item.NIRPJ = 0;
                            item.nVlr_IRPJ = 0;
                            item.NICMS = 0;
                            item.nVlr_ICMS = 0;
                        }

                        listServicos_Recursos.Add(item);
                        listServicos_Comparativos.Add(item);
                        listServicos_Empreitada.Add(item);
                    }
                }
                else
                {
                    DataSet dsRegras = ConsultaImpostos_Produtos(false, row["idProduto"].ToString(), DATASET(ds, "idEmpresa"), DATASET(ds, "idCliente"), idDestino, sUFDestino, "0", row["nValorUnitario"].ToString().Replace(',', '.'), row["sSistema"].ToString());

                    string ncm = DATASET(dsRegras, "sCodigoNCM") == "0" ? "Não Cadastrado" : DATASET(dsRegras, "sCodigoNCM");
                    string cest = DATASET(dsRegras, "sCodigoCEST") == "0" ? "Não Cadastrado" : DATASET(dsRegras, "sCodigoCEST");
                    decimal.TryParse(DATASET(dsRegras, "nMVA"), out decimal MVA);
                    decimal.TryParse(DATASET(dsRegras, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                    try { item.idRegistro = int.Parse(row["idItem"].ToString()); } catch { item.idRegistro = GerarNovo_idRegistro(listServicos_Recursos); }
                    item.SFuncao = "INCLUIR ITEM";
                    item.nOrdem = int.Parse(row["nOrdem"].ToString());
                    item.IdItem = int.Parse(row["idProduto"].ToString());
                    item.SCodigo = row["sCodigo"].ToString();
                    item.SDscProduto = row["sDscProduto"].ToString();
                    item.sNCM = ncm;
                    item.sCEST = cest;
                    item.TipoProduto = row["sTipo"].ToString();
                    item.SUnidade = row["sUnidade"].ToString();
                    item.IdGrupoProduto = int.Parse(row["idGrupo"].ToString());
                    item.IdFamiliaProduto = int.Parse(row["idFamilia"].ToString());
                    item.sDscGrupoProduto = row["sDscGrupo"].ToString();
                    item.sDscFamiliaProduto = row["sDscFamilia"].ToString();
                    item.sIndustrializado = row["sIndustrializado"].ToString() == "S" ? "Sim" : "Não";
                    item.sOrigem = row["sOrigem"].ToString();

                    item.bLiberado = true;
                    item.bSistema = row["sSistema"].ToString().Equals("S");

                    try
                    {
                        int.TryParse(DATASET(dsRegras, "idRegra"), out int idRegra);
                        item.idRegra = idRegra;

                        item.sCFOP = DATASET(dsRegras, "sCFOP");
                        item.sCST = DATASET(dsRegras, "CST");

                        item.NIPI = Math.Round(decimal.Parse(DATASET(dsRegras, "nIPI")), 2);
                        item.NPIS = Math.Round(decimal.Parse(DATASET(dsRegras, "nPIS")), 2);
                        item.NCOFINS = Math.Round(decimal.Parse(DATASET(dsRegras, "nCOFINS")), 2);
                        item.NICMS = Math.Round(decimal.Parse(DATASET(dsRegras, "nICMS")), 2);
                        item.NDIFAL = Math.Round(decimal.Parse(DATASET(dsRegras, "nDIFAL")), 2);
                        item.NST = Math.Round(decimal.Parse(DATASET(dsRegras, "nICMSST")), 2);

                        item.nReducao = Math.Round(decimal.Parse(DATASET(dsRegras, "nRedBC")), 2);

                        item.bBaseCalcICMS_com_IPI = DATASET(dsRegras, "sBaseCalculo").Equals("PI");
                    }
                    catch { }

                    item.nPesoBruto = Math.Round(decimal.Parse(row["nPesoBruto"].ToString()), 2);
                    item.nPesoLiquido = Math.Round(decimal.Parse(row["nPesoNeto"].ToString()), 2);
                    item.nVolume = Math.Round(decimal.Parse(row["nVolume"].ToString()), 2);
                    item.NFator = Math.Round(decimal.Parse(row["nDesconto"].ToString()), 4);
                    item.Preco = Math.Round(decimal.Parse(row["nValorUnitario"].ToString()), 2);
                    item.nUnitario = Math.Round(item.Preco - (item.Preco * (item.NFator / 100)), 2);
                    item.NQuantidade = Math.Round(decimal.Parse(row["nQuantidade"].ToString()), 2);
                    item.NTotal = Math.Round(item.nUnitario * item.NQuantidade, 2);

                    if (hddMoeda.Value != "0" && hddMoeda.Value != "2" && decimal.TryParse(txtCambio_View.Text, out decimal cambio))
                    {
                        MVA = 0;

                        item.NII = 0;
                        item.NIPI = 0;
                        item.NPIS = 0;
                        item.NCOFINS = 0;
                        item.NICMS = 0;
                        item.NDIFAL = 0;
                        item.NST = 0;

                        item.NTotal *= cambio;
                    }

                    try
                    {
                        RecalculaImpostos(item);
                    }
                    catch { }

                    try
                    {
                        MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, hddidDestinoVenda.Value.Equals("R"), DATASET(dsRegras, "sICMSST").Equals("S"), ncm, cest, decimal.Parse(DATASET(dsRegras, "nICMS")), DATASET(dsRegras, "dtUltimaConsulta"), DATASET(dsRegras, 1, 0, "sMensagem_Erro_LegisWeb"));

                        if (MVA > 0)
                        {
                            var st = CalculaValor_ST(item.NTotal, item.nVlr_ICMS * item.NQuantidade, MVA, ICMS_interno_destino);
                            item.nVlr_ST = st.Item1;
                            item.NST = st.Item2;
                        }
                    }
                    catch { }

                    if (!string.IsNullOrEmpty(hddsTipoDrawback.Value))
                    {
                        item.NIPI = 0.00m;
                        item.nVlr_IPI = 0.00m;
                        item.NPIS = 0.00m;
                        item.nVlr_PIS = 0.00m;
                        item.NCOFINS = 0.00m;
                        item.nVlr_COFINS = 0.00m;
                        item.NST = 0.00m;
                        item.nVlr_ST = 0.00m;
                        item.NDIFAL = 0.00m;
                        item.nVlr_DIFAL = 0.00m;

                        if (hddsTipoDrawback.Value.Equals("I"))
                        {
                            item.NICMS = 0.00m;
                            item.nVlr_ICMS = 0.00m;
                        }
                    }
                    else if (item.bSistema)
                    {
                        item.NST = 0.00m;
                        item.nVlr_ST = 0.00m;
                    }

                    item.dtInclusao = row["dtPrevisaoEntrega"].ToString().Length > 0 || !row["dtPrevisaoEntrega"].ToString().Equals("01/01/1900 00:00:00") ? (DateTime.Parse(row["dtPrevisaoEntrega"].ToString()) - DateTime.Parse(hddDtPedido.Value)).Days.ToString().Length > 4 ? "15" : (DateTime.Parse(row["dtPrevisaoEntrega"].ToString()) - DateTime.Parse(hddDtPedido.Value)).Days.ToString() : "15";

                    if (row["idProdutoPai"].ToString() != "0")
                    {
                        item.idItemPai = int.Parse(row["idProdutoPai"].ToString());
                        listProdutos_Composicao.Add(item);
                    }
                    else
                    {
                        listProdutos.Add(item);
                        listProdutos_Empreitada.Add(item);
                    }
                }
            }

            if (listServicos_Recursos.Count <= 0)
            {
                aba_Servicos_Recursos_View.Visible = false;
                aba_Comparativo_Servicos.Visible = false;
                div_pn3.Visible = false;
                div_recarregaServicos.Visible = true;
                MensagemPaginaServicos_Recursos.MostraMensagem_Aviso("<b>Aviso:</b> Não há Serviços disponíveis para este Tipo de Orçamento!", false);
            }
            else
            {
                gvServicos_Recursos_View.DataSource = listServicos_Recursos.Where(s => s.bLiberado).OrderBy(s => s.nOrdem);
                gvServicos_Recursos_View.DataBind();

                aba_Servicos_Recursos_View.Visible = true;

                gvComparativoServicos.DataSource = listServicos_Recursos.Where(s => s.bLiberado).OrderBy(s => s.nOrdem);
                gvComparativoServicos.DataBind();

                gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 3].HeaderStyle.CssClass = "";
                gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 3].ItemStyle.CssClass = "";
                gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 2].HeaderStyle.CssClass = "id";
                gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 2].ItemStyle.CssClass = "id";

                aba_Comparativo_Servicos.Visible = true;
            }

            if (!listProdutos.Any(p => !p.bLiberado) && !listServicos_Recursos.Any(p => p.bLiberado))
                aba_Comparativos.Visible = false;

            if (listProdutos.Count <= 0)
                aba_Produtos.Visible = false;
            else
            {
                gv_Produtos_View.DataSource = listProdutos.Where(p => p.bLiberado).OrderBy(p => p.nOrdem);
                gv_Produtos_View.DataBind();

                gvComparativoProdutos.DataSource = listProdutos.Where(p => p.bLiberado).OrderBy(p => (!p.bLiberado, p.nOrdem));
                gvComparativoProdutos.DataBind();

                gvComparativoProdutos.Columns[2].HeaderStyle.CssClass = "id";
                gvComparativoProdutos.Columns[2].ItemStyle.CssClass = "id";
                gvComparativoProdutos.Columns[3].HeaderStyle.CssClass = "";
                gvComparativoProdutos.Columns[3].ItemStyle.CssClass = "";

                gvComparativoProdutos.Columns[9].HeaderStyle.CssClass = "";
                gvComparativoProdutos.Columns[9].ItemStyle.CssClass = "";
                gvComparativoProdutos.Columns[10].HeaderStyle.CssClass = "id";
                gvComparativoProdutos.Columns[10].ItemStyle.CssClass = "id";

                aba_Produtos.Visible = true;
                aba_Comparativo_Produtos.Visible = true;
            }

            if (aba_Comparativos.Visible)
            {
                if (listProdutos.Count <= 1)
                {
                    aba_Comparativo_Produtos.Visible = false;

                    Scripts.Mantem_AbaAtiva(Page, "aba-Comparativo_Servicos");
                }
                else if (!listServicos_Recursos.Any(p => p.bLiberado))
                    aba_Comparativo_Servicos.Visible = false;
            }
        }

        /// <summary>
        /// Método utilizado para Popular as Listas de Classes da Composição de um Serviço.
        /// </summary>
        /// <param name="item">Recebe a Classe completa que representa o Serviço.</param>
        /// <param name="bEmpreitada">Define se as Classes a serem pouladas são as de Empreitada.</param>
        protected void PopulaComposicao(cls_Comercial_Tabelas item, bool bEmpreitada)
        {
            try
            {
                Dictionary<string, string> vParametrosComposicao = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_COMPOSICAO" },
                    { "@idProduto", item.IdItem.ToString() },
                    { "@sUF_Destino", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim() }
                };
                DataSet dsComposicao = ExecutarDataSet(sProcedure_Produtos, vParametrosComposicao);

                if (dsComposicao.Tables[1].Rows.Count > 0)
                {
                    int idTipoPai = int.Parse(DATASET(dsComposicao, "idTipo"));
                    decimal precoFilho = 0;
                    decimal totalFilho = 0;

                    foreach (DataRow row in dsComposicao.Tables[1].Rows)
                    {
                        int idTipoFilho = int.Parse(row["idTipo"].ToString());
                        int idItemFilho = int.Parse(row["idItemComposicao"].ToString());

                        if (idTipoPai.Equals(1))
                        {
                            var itemFilho = bEmpreitada ? listServicos_Composicao_Filhos_Empreitada.FirstOrDefault(s => s.IdItem == idItemFilho && s.idItemPai == item.idRegistro && s.IdItem != item.idRegistro) : listServicos_Recursos_Composicao_Filhos.FirstOrDefault(s => s.IdItem == idItemFilho && s.idItemPai == item.idRegistro && s.IdItem != item.idRegistro);

                            if (itemFilho == null && idItemFilho != item.idRegistro)
                            {
                                cls_Comercial_Tabelas filho = new cls_Comercial_Tabelas
                                {
                                    idRegistro = GerarNovo_idRegistro(bEmpreitada ? listServicos_Composicao_Filhos_Empreitada : listServicos_Recursos_Composicao_Filhos),
                                    SFuncao = item.SFuncao,
                                    nOrdem = bEmpreitada ? listServicos_Composicao_Filhos_Empreitada.Count(s => s.bLiberado && s.idItemPai == item.idRegistro) + 10 : listServicos_Recursos_Composicao_Filhos.Count(s => s.bLiberado && s.idItemPai == item.idRegistro) + 10,
                                    IdItem = idItemFilho,
                                    idItemPai = item.idRegistro,
                                    idItemAvo = item.idItemPai,
                                    idItemBisavo = item.idItemAvo,
                                    SCodigo = row["sCodigo"].ToString(),
                                    SDscProduto = row["sDscProduto"].ToString(),
                                    TipoProduto = row["sDscTipoProduto"].ToString(),
                                    idTipo = idTipoFilho,
                                    SUnidade = row["sUnidade"].ToString(),
                                    NQuantidade = decimal.Parse(row["nQuantidade"].ToString()),
                                    NFator = Math.Round(decimal.Parse(txtDescontoServico.Text), 2),
                                    bLiberado = item.bLiberado
                                };

                                try
                                {
                                    Dictionary<string, string> vParametrosComposicao_1 = new Dictionary<string, string>
                                    {
                                        { "@sFuncao", "CONSULTAR_COMPOSICAO" },
                                        { "@idProduto", filho.IdItem.ToString() },
                                        { "@sUF_Destino", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim() }
                                    };
                                    DataSet dsComposicao_1 = ExecutarDataSet(sProcedure_Produtos, vParametrosComposicao_1);

                                    if (dsComposicao_1.Tables[1].Rows.Count > 0)
                                    {
                                        decimal precoFilho_1 = 0;
                                        decimal totalFilho_1 = 0;

                                        foreach (DataRow row_1 in dsComposicao_1.Tables[1].Rows)
                                        {
                                            int idTipoFilho_1 = int.Parse(row_1["idTipo"].ToString());
                                            int idItemFilho_1 = int.Parse(row_1["idItemComposicao"].ToString());

                                            var itemFilho_1 = bEmpreitada ? listServicos_Composicao_Netos_Empreitada.FirstOrDefault(s => s.IdItem == idItemFilho_1 && s.idItemPai == filho.idRegistro && s.idItemAvo == item.idRegistro && s.IdItem != filho.idRegistro) : listServicos_Recursos_Composicao_Netos.FirstOrDefault(s => s.IdItem == idItemFilho_1 && s.idItemPai == filho.idRegistro && s.idItemAvo == item.idRegistro && s.IdItem != filho.idRegistro);

                                            if (itemFilho_1 == null && idItemFilho_1 != idItemFilho)
                                            {
                                                cls_Comercial_Tabelas filho_1 = new cls_Comercial_Tabelas
                                                {
                                                    idRegistro = GerarNovo_idRegistro(bEmpreitada ? listServicos_Composicao_Netos_Empreitada : listServicos_Recursos_Composicao_Netos),
                                                    SFuncao = filho.SFuncao,
                                                    nOrdem = bEmpreitada ? listServicos_Composicao_Netos_Empreitada.Count(s => s.bLiberado && s.idItemPai == filho.idRegistro && s.idItemAvo == item.idRegistro) + 10 : listServicos_Recursos_Composicao_Netos.Count(s => s.bLiberado && s.idItemPai == filho.idRegistro && s.idItemAvo == item.idRegistro) + 10,
                                                    IdItem = idItemFilho_1,
                                                    idItemPai = filho.idRegistro,
                                                    idItemAvo = filho.idItemPai,
                                                    idItemBisavo = filho.idItemAvo,
                                                    SCodigo = row_1["sCodigo"].ToString(),
                                                    SDscProduto = row_1["sDscProduto"].ToString(),
                                                    TipoProduto = row_1["sDscTipoProduto"].ToString(),
                                                    idTipo = idTipoFilho_1,
                                                    SUnidade = row_1["sUnidade"].ToString(),
                                                    NQuantidade = decimal.Parse(row_1["nQuantidade"].ToString()),
                                                    Preco = Math.Round(decimal.Parse(row_1["nPreco"].ToString()), 2),
                                                    NFator = Math.Round(decimal.Parse(txtDescontoServico.Text), 2),
                                                    NMargem = Math.Round(decimal.Parse(row_1["nFator"].ToString()), 2)
                                                };
                                                filho_1.NMargem = filho_1.NMargem;
                                                filho_1.bLiberado = filho.bLiberado;

                                                try
                                                {
                                                    Dictionary<string, string> vParametrosComposicao_2 = new Dictionary<string, string>
                                                    {
                                                        { "@sFuncao", "CONSULTAR_COMPOSICAO" },
                                                        { "@idProduto", filho_1.IdItem.ToString() },
                                                        { "@sUF_Destino", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim() }
                                                    };
                                                    DataSet dsComposicao_2 = ExecutarDataSet(sProcedure_Produtos, vParametrosComposicao_2);

                                                    if (dsComposicao_2.Tables[1].Rows.Count > 0)
                                                    {
                                                        decimal totalFilho_2 = 0;

                                                        foreach (DataRow row_2 in dsComposicao_2.Tables[1].Rows)
                                                        {
                                                            int idTipoFilho_2 = int.Parse(row_2["idTipo"].ToString());
                                                            int idItemFilho_2 = int.Parse(row_2["idItemComposicao"].ToString());

                                                            var itemFilho_2 = bEmpreitada ? listServicos_Composicao_Bisnetos_Empreitada.FirstOrDefault(s => s.IdItem == idItemFilho_2 && s.idItemPai == filho_1.idRegistro && s.idItemAvo == filho.idRegistro && s.idItemBisavo == item.idRegistro && s.IdItem != idItemFilho_1) : listServicos_Recursos_Composicao_Bisnetos.FirstOrDefault(s => s.IdItem == idItemFilho_2 && s.idItemPai == filho_1.idRegistro && s.idItemAvo == filho.idRegistro && s.idItemBisavo == item.idRegistro && s.IdItem != idItemFilho_1);

                                                            if (itemFilho_2 == null && idItemFilho_2 != idItemFilho_1)
                                                            {
                                                                cls_Comercial_Tabelas filho_2 = new cls_Comercial_Tabelas
                                                                {
                                                                    idRegistro = GerarNovo_idRegistro(bEmpreitada ? listServicos_Composicao_Bisnetos_Empreitada : listServicos_Recursos_Composicao_Bisnetos),
                                                                    SFuncao = filho_1.SFuncao,
                                                                    nOrdem = bEmpreitada ? listServicos_Composicao_Bisnetos_Empreitada.Count(s => s.bLiberado && s.idItemPai == filho_1.idRegistro && s.idItemAvo == filho.idRegistro && s.idItemBisavo == item.idRegistro) + 10 : listServicos_Recursos_Composicao_Bisnetos.Count(s => s.bLiberado && s.idItemPai == filho_1.idRegistro && s.idItemAvo == filho.idRegistro && s.idItemBisavo == item.idRegistro) + 10,
                                                                    IdItem = idItemFilho_2,
                                                                    idItemPai = filho_1.idRegistro,
                                                                    idItemAvo = filho_1.idItemPai,
                                                                    idItemBisavo = filho_1.idItemAvo,
                                                                    SCodigo = row_2["sCodigo"].ToString(),
                                                                    SDscProduto = row_2["sDscProduto"].ToString(),
                                                                    TipoProduto = row_2["sDscTipoProduto"].ToString(),
                                                                    idTipo = idTipoFilho_2,
                                                                    SUnidade = row_2["sUnidade"].ToString(),
                                                                    NQuantidade = decimal.Parse(row_2["nQuantidade"].ToString()),
                                                                    Preco = Math.Round(decimal.Parse(row_2["nPreco"].ToString()), 2),
                                                                    NFator = Math.Round(decimal.Parse(txtDescontoServico.Text), 2),
                                                                    NMargem = Math.Round(decimal.Parse(row_2["nFator"].ToString()), 2)
                                                                };
                                                                filho_2.NMargem = filho_2.NMargem;
                                                                filho_2.bLiberado = filho_1.bLiberado;

                                                                try
                                                                {
                                                                    filho_2.NTotal = Math.Round((filho_2.Preco * filho_2.NMargem * filho_2.NQuantidade) - (filho_2.Preco * filho_2.NMargem * filho_2.NQuantidade * (filho_2.NFator / 100)), 2);

                                                                    if (!bEmpreitada)
                                                                        listServicos_Recursos_Composicao_Bisnetos.Add(filho_2);

                                                                    listServicos_Composicao_Bisnetos_Empreitada.Add(filho_2);

                                                                    totalFilho_2 += filho_2.NTotal;
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    throw new Exception(ex.Message);
                                                                }
                                                            }
                                                        }

                                                        filho_1.Preco = Math.Round(totalFilho_2, 2);
                                                    }

                                                    filho_1.NTotal = Math.Round((filho_1.Preco * filho_1.NMargem * filho_1.NQuantidade) - (filho_1.Preco * filho_1.NMargem * filho_1.NQuantidade * (filho_1.NFator / 100)), 2);

                                                    if (!bEmpreitada)
                                                        listServicos_Recursos_Composicao_Netos.Add(filho_1);

                                                    listServicos_Composicao_Netos_Empreitada.Add(filho_1);

                                                    precoFilho_1 += filho_1.Preco;
                                                    totalFilho_1 += filho_1.NTotal;
                                                }
                                                catch (Exception ex)
                                                {
                                                    throw new Exception(ex.Message);
                                                }
                                            }
                                        }

                                        filho.Preco = Math.Round(precoFilho_1, 2);
                                        filho.NTotal = Math.Round(totalFilho_1 * filho.NQuantidade, 2);
                                        filho.NMargem = Math.Round(totalFilho_1 > decimal.Zero && filho.Preco > decimal.Zero ? totalFilho_1 / filho.Preco : decimal.Zero, 2);
                                    }

                                    if (!bEmpreitada)
                                        listServicos_Recursos_Composicao_Filhos.Add(filho);

                                    listServicos_Composicao_Filhos_Empreitada.Add(filho);

                                    precoFilho += filho.Preco;
                                    totalFilho += filho.NTotal;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                            }
                        }
                        else if (idTipoPai.Equals(3))
                        {
                            var itemFilho_1 = bEmpreitada ? listServicos_Composicao_Netos_Empreitada.FirstOrDefault(s => s.IdItem == idItemFilho && s.idItemPai == item.idRegistro && s.idItemAvo == item.idItemPai && s.idItemBisavo == item.idItemAvo && s.IdItem != item.idRegistro) : listServicos_Recursos_Composicao_Netos.FirstOrDefault(s => s.IdItem == idItemFilho && s.idItemPai == item.idRegistro && s.idItemAvo == item.idItemPai && s.idItemBisavo == item.idItemAvo && s.IdItem != item.idRegistro);

                            if (itemFilho_1 == null && idItemFilho != item.idRegistro)
                            {
                                cls_Comercial_Tabelas filho_1 = new cls_Comercial_Tabelas
                                {
                                    idRegistro = GerarNovo_idRegistro(bEmpreitada ? listServicos_Composicao_Netos_Empreitada : listServicos_Recursos_Composicao_Netos),
                                    SFuncao = item.SFuncao,
                                    nOrdem = bEmpreitada ? listServicos_Composicao_Netos_Empreitada.Count(s => s.bLiberado && s.idItemPai == item.idRegistro && s.idItemAvo == item.idItemPai && s.idItemBisavo == item.idItemAvo) + 10 : listServicos_Recursos_Composicao_Netos.Count(s => s.bLiberado && s.idItemPai == item.idRegistro && s.idItemAvo == item.idItemPai && s.idItemBisavo == item.idItemAvo) + 10,
                                    IdItem = idItemFilho,
                                    idItemPai = item.idRegistro,
                                    idItemAvo = item.idItemPai,
                                    idItemBisavo = item.idItemAvo,
                                    SCodigo = row["sCodigo"].ToString(),
                                    SDscProduto = row["sDscProduto"].ToString(),
                                    TipoProduto = row["sDscTipoProduto"].ToString(),
                                    idTipo = idTipoFilho,
                                    SUnidade = row["sUnidade"].ToString(),
                                    NQuantidade = decimal.Parse(row["nQuantidade"].ToString()),
                                    Preco = Math.Round(decimal.Parse(row["nPreco"].ToString()), 2),
                                    NFator = Math.Round(decimal.Parse(txtDescontoServico.Text), 2),
                                    NMargem = Math.Round(decimal.Parse(row["nFator"].ToString()), 2)
                                };

                                filho_1.bLiberado = item.bLiberado;

                                try
                                {
                                    Dictionary<string, string> vParametrosComposicao_2 = new Dictionary<string, string>
                                    {
                                        { "@sFuncao", "CONSULTAR_COMPOSICAO" },
                                        { "@idProduto", filho_1.IdItem.ToString() },
                                        { "@sUF_Destino", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim() }
                                    };
                                    DataSet dsComposicao_2 = ExecutarDataSet(sProcedure_Produtos, vParametrosComposicao_2);

                                    if (dsComposicao_2.Tables[1].Rows.Count > 0)
                                    {
                                        decimal totalFilho_2 = 0;

                                        foreach (DataRow row_2 in dsComposicao_2.Tables[1].Rows)
                                        {
                                            int idTipoFilho_2 = int.Parse(row_2["idTipo"].ToString());
                                            int idItemFilho_2 = int.Parse(row_2["idItemComposicao"].ToString());
                                            int idItemPai_2 = int.Parse(row_2["iditem"].ToString());

                                            var itemFilho_2 = bEmpreitada ? listServicos_Composicao_Bisnetos_Empreitada.FirstOrDefault(s => s.IdItem == idItemFilho_2 && s.idItemPai == idItemPai_2 && s.idItemAvo == filho_1.idItemPai && s.idItemBisavo == filho_1.idItemAvo && s.IdItem != idItemFilho) : listServicos_Recursos_Composicao_Bisnetos.FirstOrDefault(s => s.IdItem == idItemFilho_2 && s.idItemPai == idItemPai_2 && s.idItemAvo == filho_1.idItemPai && s.idItemBisavo == filho_1.idItemAvo && s.IdItem != idItemFilho);

                                            if (itemFilho_2 == null && idItemFilho_2 != idItemFilho)
                                            {
                                                cls_Comercial_Tabelas filho_2 = new cls_Comercial_Tabelas
                                                {
                                                    idRegistro = GerarNovo_idRegistro(bEmpreitada ? listServicos_Composicao_Bisnetos_Empreitada : listServicos_Recursos_Composicao_Bisnetos),
                                                    SFuncao = filho_1.SFuncao,
                                                    nOrdem = bEmpreitada ? listServicos_Composicao_Bisnetos_Empreitada.Count(s => s.bLiberado && s.idItemPai == idItemPai_2 && s.idItemAvo == filho_1.idItemPai && s.idItemBisavo == filho_1.idItemAvo) + 10 : listServicos_Recursos_Composicao_Bisnetos.Count(s => s.bLiberado && s.idItemPai == idItemPai_2 && s.idItemAvo == filho_1.idItemPai && s.idItemBisavo == filho_1.idItemAvo) + 10,
                                                    IdItem = idItemFilho_2,
                                                    idItemPai = idItemPai_2,
                                                    idItemAvo = filho_1.idItemPai,
                                                    idItemBisavo = filho_1.idItemAvo,
                                                    SCodigo = row_2["sCodigo"].ToString(),
                                                    SDscProduto = row_2["sDscProduto"].ToString(),
                                                    TipoProduto = row_2["sDscTipoProduto"].ToString(),
                                                    idTipo = idTipoFilho_2,
                                                    SUnidade = row_2["sUnidade"].ToString(),
                                                    NQuantidade = decimal.Parse(row_2["nQuantidade"].ToString()),
                                                    Preco = Math.Round(decimal.Parse(row_2["nPreco"].ToString()), 2),
                                                    NFator = Math.Round(decimal.Parse(txtDescontoServico.Text), 2),
                                                    NMargem = Math.Round(decimal.Parse(row_2["nFator"].ToString()), 2)
                                                };

                                                filho_2.bLiberado = filho_1.bLiberado;

                                                try
                                                {
                                                    filho_2.NTotal = Math.Round((filho_2.Preco * filho_2.NMargem * filho_2.NQuantidade) - (filho_2.Preco * filho_2.NMargem * filho_2.NQuantidade * (filho_2.NFator / 100)), 2);

                                                    if (!bEmpreitada)
                                                        listServicos_Recursos_Composicao_Bisnetos.Add(filho_2);

                                                    listServicos_Composicao_Bisnetos_Empreitada.Add(filho_2);

                                                    totalFilho_2 += filho_2.NTotal;
                                                }
                                                catch (Exception ex)
                                                {
                                                    throw new Exception(ex.Message);
                                                }
                                            }
                                        }

                                        filho_1.Preco = Math.Round(totalFilho_2, 2);
                                    }

                                    filho_1.NTotal = Math.Round((filho_1.Preco * filho_1.NMargem * filho_1.NQuantidade) - (filho_1.Preco * filho_1.NMargem * filho_1.NQuantidade * (filho_1.NFator / 100)), 2);

                                    if (bEmpreitada)
                                        listServicos_Recursos_Composicao_Netos.Add(filho_1);

                                    listServicos_Composicao_Netos_Empreitada.Add(filho_1);

                                    precoFilho += filho_1.Preco;
                                    totalFilho += filho_1.NTotal;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                            }
                        }
                        else if (idTipoPai.Equals(2))
                        {
                            if (item.idItemPai > 0 && item.idItemAvo > 0)
                            {
                                var itemFilho = bEmpreitada ? listServicos_Composicao_Bisnetos_Empreitada.FirstOrDefault(s => s.IdItem == idItemFilho && s.idItemPai == item.idRegistro && s.idItemAvo == item.idItemPai && s.idItemBisavo == item.idItemAvo && s.IdItem != item.idRegistro) : listServicos_Recursos_Composicao_Bisnetos.FirstOrDefault(s => s.IdItem == idItemFilho && s.idItemPai == item.idRegistro && s.idItemAvo == item.idItemPai && s.idItemBisavo == item.idItemAvo && s.IdItem != item.idRegistro);

                                if (itemFilho == null && idItemFilho != item.idRegistro)
                                {
                                    cls_Comercial_Tabelas filho = new cls_Comercial_Tabelas
                                    {
                                        idRegistro = GerarNovo_idRegistro(bEmpreitada ? listServicos_Composicao_Bisnetos_Empreitada : listServicos_Recursos_Composicao_Bisnetos),
                                        SFuncao = item.SFuncao,
                                        nOrdem = bEmpreitada ? listServicos_Composicao_Bisnetos_Empreitada.Count(s => s.bLiberado && s.idItemPai == item.idRegistro && s.idItemAvo == item.idItemPai && s.idItemBisavo == item.idItemAvo) + 10 : listServicos_Recursos_Composicao_Bisnetos.Count(s => s.bLiberado && s.idItemPai == item.idRegistro && s.idItemAvo == item.idItemPai && s.idItemBisavo == item.idItemAvo) + 10,
                                        IdItem = idItemFilho,
                                        idItemPai = item.idRegistro,
                                        idItemAvo = item.idItemPai,
                                        idItemBisavo = item.idItemAvo,
                                        SCodigo = row["sCodigo"].ToString(),
                                        SDscProduto = row["sDscProduto"].ToString(),
                                        TipoProduto = row["sDscTipoProduto"].ToString(),
                                        idTipo = idTipoFilho,
                                        SUnidade = row["sUnidade"].ToString(),
                                        NQuantidade = decimal.Parse(row["nQuantidade"].ToString()),
                                        Preco = Math.Round(decimal.Parse(row["nPreco"].ToString()), 2),
                                        NFator = Math.Round(decimal.Parse(txtDescontoServico.Text), 2),
                                        NMargem = Math.Round(decimal.Parse(row["nFator"].ToString()), 2)
                                    };

                                    filho.NTotal = Math.Round((filho.Preco * filho.NMargem * filho.NQuantidade) - (filho.Preco * filho.NMargem * filho.NQuantidade * (filho.NFator / 100)), 2);
                                    filho.bLiberado = item.bLiberado;

                                    try
                                    {
                                        if (bEmpreitada)
                                            listServicos_Recursos_Composicao_Bisnetos.Add(filho);

                                        listServicos_Composicao_Bisnetos_Empreitada.Add(filho);

                                        precoFilho += filho.Preco;
                                        totalFilho += filho.NTotal;
                                    }
                                    catch { }
                                }
                            }
                            else if (item.idItemPai > 0)
                            {
                                var itemFilho = bEmpreitada ? listServicos_Composicao_Netos_Empreitada.FirstOrDefault(s => s.IdItem == idItemFilho && s.idItemPai == item.idRegistro && s.idItemAvo == item.idItemPai && s.idItemBisavo == item.idItemAvo && s.IdItem != item.idRegistro) : listServicos_Recursos_Composicao_Netos.FirstOrDefault(s => s.IdItem == idItemFilho && s.idItemPai == item.idRegistro && s.idItemAvo == item.idItemPai && s.idItemBisavo == item.idItemAvo && s.IdItem != item.idRegistro);

                                if (itemFilho == null && idItemFilho != item.idRegistro)
                                {
                                    cls_Comercial_Tabelas filho = new cls_Comercial_Tabelas
                                    {
                                        idRegistro = GerarNovo_idRegistro(bEmpreitada ? listServicos_Composicao_Netos_Empreitada : listServicos_Recursos_Composicao_Netos),
                                        SFuncao = item.SFuncao,
                                        nOrdem = bEmpreitada ? listServicos_Composicao_Netos_Empreitada.Count(s => s.bLiberado && s.idItemPai == item.idRegistro && s.idItemAvo == item.idItemPai && s.idItemBisavo == item.idItemAvo) + 10 : listServicos_Recursos_Composicao_Netos.Count(s => s.bLiberado && s.idItemPai == item.idRegistro && s.idItemAvo == item.idItemPai && s.idItemBisavo == item.idItemAvo) + 10,
                                        IdItem = idItemFilho,
                                        idItemPai = item.idRegistro,
                                        idItemAvo = item.idItemPai,
                                        idItemBisavo = item.idItemAvo,
                                        SCodigo = row["sCodigo"].ToString(),
                                        SDscProduto = row["sDscProduto"].ToString(),
                                        TipoProduto = row["sDscTipoProduto"].ToString(),
                                        idTipo = idTipoFilho,
                                        SUnidade = row["sUnidade"].ToString(),
                                        NQuantidade = decimal.Parse(row["nQuantidade"].ToString()),
                                        Preco = Math.Round(decimal.Parse(row["nPreco"].ToString()), 2),
                                        NFator = Math.Round(decimal.Parse(txtDescontoServico.Text), 2),
                                        NMargem = Math.Round(decimal.Parse(row["nFator"].ToString()), 2)
                                    };

                                    filho.NTotal = Math.Round((filho.Preco * filho.NMargem * filho.NQuantidade) - (filho.Preco * filho.NMargem * filho.NQuantidade * (filho.NFator / 100)), 2);
                                    filho.bLiberado = item.bLiberado;

                                    try
                                    {
                                        if (bEmpreitada)
                                            listServicos_Recursos_Composicao_Netos.Add(filho);

                                        listServicos_Composicao_Netos_Empreitada.Add(filho);

                                        precoFilho += filho.Preco;
                                        totalFilho += filho.NTotal;
                                    }
                                    catch { }
                                }
                            }
                        }
                    }

                    if (precoFilho > 0 && totalFilho > 0)
                    {
                        item.Preco = Math.Round(precoFilho, 2);
                        item.NTotal = Math.Round(totalFilho * item.NQuantidade, 2);
                        item.NMargem = Math.Round(totalFilho > decimal.Zero && item.Preco > decimal.Zero ? totalFilho / item.Preco : decimal.Zero, 2);

                        if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                            item.NTotal *= cambio;
                        else
                        {
                            DataSet dsRegras = ConsultaImpostos_Produtos(false, item.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, item.idTipoRegra.ToString(), hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", Math.Round(item.Preco, 2).ToString().Replace(",", "."));

                            if (ValidarDataSet(dsRegras))
                            {
                                item.sNCM = DATASET(dsRegras, "sCodigoFederal");
                                item.sCST = DATASET(dsRegras, "sCodigoMunicipal");

                                item.NIPI = Math.Round(decimal.Parse(DATASET(dsRegras, "nISS")), 2);
                                item.nVlr_IPI = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrISS")), 2);
                                item.NCSSL = Math.Round(decimal.Parse(DATASET(dsRegras, "nCSSL")), 2);
                                item.nVlr_CSSL = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrCSSL")), 2);
                                item.NIRPJ = Math.Round(decimal.Parse(DATASET(dsRegras, "nIR")), 2);
                                item.nVlr_IRPJ = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrIR")), 2);
                                item.NICMS = Math.Round(decimal.Parse(DATASET(dsRegras, "nINSS")), 2);
                                item.nVlr_ICMS = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrINSS")), 2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Houve um erro ao Popular a cadeia de Composição dos Serviços para este Orçamento!<br />Erro da Composição: " + ex.Message);
            }
        }

        /// <summary>
        /// Método utilizado para Popular a Grid de Visualização dos Totais na Etapa de Visualização.
        /// </summary>
        protected void PopulaTotais()
        {
            decimal total_Produtos = 0, totalLiquido_Produtos = 0, COFINS_Produtos = 0, PIS_Produtos = 0, ICMS = 0, BC_ICMS = 0, Red_BC_ICMS = 0, ST = 0, DIFAL = 0, IPI = 0;
            decimal total_Servicos = 0, totalLiquido_Servicos = 0, INSS = 0, ISS = 0;

            foreach (var p in listProdutos.Where(p => p.bLiberado))
            {
                IPI += p.nVlr_IPI * p.NQuantidade;
                PIS_Produtos += p.nVlr_PIS * p.NQuantidade;
                COFINS_Produtos += p.nVlr_COFINS * p.NQuantidade;
                BC_ICMS += p.nBaseCalc_ICMS_Original * p.NQuantidade;
                Red_BC_ICMS += p.nVlrReducao * p.NQuantidade;
                ICMS += p.nVlr_ICMS * p.NQuantidade;

                ST += p.nVlr_ST;
                DIFAL += p.nVlr_DIFAL * p.NQuantidade;

                totalLiquido_Produtos += p.nVlr_Liquido * p.NQuantidade;
                total_Produtos += p.NTotal;
            }

            foreach (var s in listServicos_Recursos.Where(s => s.bLiberado))
            {
                total_Servicos += s.NTotal;

                INSS += s.nVlr_ICMS * s.NQuantidade;
                ISS += s.nVlr_IPI * s.NQuantidade;
            }

            totalLiquido_Servicos = total_Servicos - INSS - ISS;

            sTotalLiquido.Text = totalLiquido_Produtos.ToString("N2");
            sTotal_sem_IPI.Text = (total_Produtos - IPI).ToString("N2");
            sTotal_com_IPI.Text = total_Produtos.ToString("N2");

            sTotal_COFINS.Text = COFINS_Produtos.ToString("N2");
            sTotal_PIS.Text = PIS_Produtos.ToString("N2");
            sTotal_ICMS.Text = PopulaCampoTotal_ICMS(ICMS, BC_ICMS, Red_BC_ICMS);
            sTotal_ST.Text = ST.ToString("N2");
            sTotal_DIFAL.Text = DIFAL.ToString("N2");
            sTotal_IPI.Text = IPI.ToString("N2");

            tbcLiquido_ImpostosProdutos_Comparativos.Text = totalLiquido_Produtos.ToString("N2");
            tbcSemIPI_ImpostosProdutos_Comparativos.Text = (total_Produtos - IPI).ToString("N2");
            tbcIPI_ImpostosProdutos_Comparativos.Text = total_Produtos.ToString("N2");

            tbcCOFINS_ImpostosProdutos_Comparativos.Text = COFINS_Produtos.ToString("N2");
            tbcPIS_ImpostosProdutos_Comparativos.Text = PIS_Produtos.ToString("N2");
            tbcICMS_ImpostosProdutos_Comparativos.Text = PopulaCampoTotal_ICMS(ICMS, BC_ICMS, Red_BC_ICMS);
            tbcST_ImpostosProdutos_Comparativos.Text = ST.ToString("N2");
            tbcDIFAL_ImpostosProdutos_Comparativos.Text = DIFAL.ToString("N2");
            tbcVlr_IPI_ImpostosProdutos_Comparativos.Text = IPI.ToString("N2");

            txtComparativo_LiquidoProdutos.Text = totalLiquido_Produtos.ToString("N2");
            txtComparativo_BrutoProdutos.Text = total_Produtos.ToString("N2");
            txtComparativo_LiquidoServicos.Text = totalLiquido_Servicos.ToString("N2");
            txtComparativo_BrutoServicos.Text = total_Servicos.ToString("N2");
            txtComparativo_LiquidoOrcamento.Text = (totalLiquido_Produtos + totalLiquido_Servicos).ToString("N2");
            txtComparativo_BrutoOrcamento.Text = (total_Produtos + total_Servicos).ToString("N2");

            txtTotalServicos_View.Text = total_Servicos.ToString("N2");
            txtTotalServico_Recurso_View.Text = total_Servicos.ToString("N2");
            txtTotalServico.Text = total_Servicos > decimal.Zero ? total_Servicos.ToString("N2") : "0,00";
            txtTotalProjeto_View.Text = listServicos_Recursos.Where(s => s.bLiberado && s.bProjeto).Sum(s => s.NTotal).ToString("N2");

            decimal total_Geral_Produtos = total_Produtos + ST;
            txtTotalProduto_View.Text = total_Geral_Produtos.ToString("N2");
            txtTotalProdutos_View.Text = total_Geral_Produtos.ToString("N2");
            txtTotalProdutos.Text = total_Produtos.ToString("N2");
            txtTotal_View.Text = (decimal.Parse(string.IsNullOrEmpty(txtTotalProduto_View.Text) ? "0" : txtTotalProduto_View.Text) + decimal.Parse(string.IsNullOrEmpty(txtTotalServico_Recurso_View.Text) ? "0" : txtTotalServico_Recurso_View.Text) + decimal.Parse(string.IsNullOrEmpty(txtFrete_View.Text) ? "0" : txtFrete_View.Text) + decimal.Parse(string.IsNullOrEmpty(txtCusto_Aduaneiro_View.Text) ? "0" : txtCusto_Aduaneiro_View.Text) + decimal.Parse(string.IsNullOrEmpty(txtCusto_Despachante_View.Text) ? "0" : txtCusto_Despachante_View.Text)).ToString("N2");
        }

        /// <summary>
        /// Método utilizado para Popular os campos de Totais da aba de Comparativos.
        /// </summary>
        protected void PopulaTotais_Comparativo()
        {
            div_ImpostosProdutos_Comparativos_Ajustado.Visible = true;
            div_Comparativos_Totais_Ajustado.Visible = true;
            div_Desconto_Global_Produtos_Comparativos.Visible = true;
            div_Prazo_Global_Produtos_Comparativos.Visible = true;

            decimal total_Produtos = 0, totalLiquido_Produtos = 0, COFINS_Produtos = 0, PIS_Produtos = 0, ICMS = 0, BC_ICMS = 0, Red_BC_ICMS = 0, ST = 0, DIFAL = 0, IPI = 0;
            decimal total_Servicos = 0, totalLiquido_Servicos = 0, INSS = 0, ISS = 0;

            List<cls_Comercial_Tabelas> lista_produtos = new List<cls_Comercial_Tabelas>();
            List<cls_Comercial_Tabelas> lista_servicos = new List<cls_Comercial_Tabelas>();

            if (hddComparativoProdutos.Value == "S")
            {
                if (Convert.ToBoolean(hddComparativos_Empreitada.Value))
                    lista_produtos = listProdutos_Empreitada;
                else
                    lista_produtos = listProdutos_Comparativos;
            }
            else
            {
                if (Convert.ToBoolean(hddComparativos_Empreitada.Value))
                    lista_produtos = listProdutos_Empreitada;
                else
                    lista_produtos = listProdutos;
            }

            if (Convert.ToBoolean(hddComparativos_Empreitada.Value))
                lista_servicos = listServicos_Empreitada;
            else
                lista_servicos = listServicos_Comparativos;

            foreach (var p in lista_produtos.Where(p => p.bLiberado))
            {
                IPI += p.nVlr_IPI * p.NQuantidade;
                PIS_Produtos += p.nVlr_PIS * p.NQuantidade;
                COFINS_Produtos += p.nVlr_COFINS * p.NQuantidade;
                BC_ICMS += p.nBaseCalc_ICMS * p.NQuantidade;
                Red_BC_ICMS += p.nVlrReducao * p.NQuantidade;
                ICMS += p.nVlr_ICMS * p.NQuantidade;

                ST += p.nVlr_ST;
                DIFAL += p.nVlr_DIFAL * p.NQuantidade;

                totalLiquido_Produtos += p.nVlr_Liquido * p.NQuantidade;
                total_Produtos += p.NTotal;
            }

            foreach (var s in lista_servicos.Where(s => s.bLiberado))
            {
                total_Servicos += s.NTotal;

                INSS += s.nVlr_ICMS * s.NQuantidade;
                ISS += s.nVlr_IPI * s.NQuantidade;
            }

            totalLiquido_Servicos = total_Servicos - INSS - ISS;

            txtComparativos_LiquidoProdutos_Ajustado.Text = totalLiquido_Produtos.ToString("N2");
            txtComparativos_BrutoProdutos_Ajustado.Text = total_Produtos.ToString("N2");
            txtComparativos_LiquidoServicos_Ajustado.Text = totalLiquido_Servicos.ToString("N2");
            txtComparativos_BrutoServicos_Ajustado.Text = total_Servicos.ToString("N2");
            txtComparativos_LiquidoOrcamento_Ajustado.Text = (totalLiquido_Produtos + totalLiquido_Servicos).ToString("N2");
            txtComparativos_BrutoOrcamento_Ajustado.Text = (total_Produtos + total_Servicos).ToString("N2");

            try
            {
                txtTotalProdutos_Empreitada.Text = (totalLiquido_Produtos * 100 / (totalLiquido_Produtos + totalLiquido_Servicos)).ToString("N2");
                txtTotalServicos_Empreitada.Text = (totalLiquido_Servicos * 100 / (totalLiquido_Produtos + totalLiquido_Servicos)).ToString("N2");
            }
            catch { }

            txtTotalOrcamento_Empreitada.Text = (totalLiquido_Produtos + totalLiquido_Servicos).ToString("N2");

            tbcLiquido_ImpostosProdutos_Comparativos_Ajustado.Text = totalLiquido_Produtos.ToString("N2");
            tbcSemIPI_ImpostosProdutos_Comparativos_Ajustado.Text = (total_Produtos - IPI).ToString("N2");
            tbcIPI_ImpostosProdutos_Comparativos_Ajustado.Text = total_Produtos.ToString("N2");

            tbcCOFINS_ImpostosProdutos_Comparativos_Ajustado.Text = COFINS_Produtos.ToString("N2");
            tbcPIS_ImpostosProdutos_Comparativos_Ajustado.Text = PIS_Produtos.ToString("N2");
            tbcICMS_ImpostosProdutos_Comparativos_Ajustado.Text = PopulaCampoTotal_ICMS(ICMS, BC_ICMS, Red_BC_ICMS);
            tbcST_ImpostosProdutos_Comparativos_Ajustado.Text = ST.ToString("N2");
            tbcDIFAL_ImpostosProdutos_Comparativos_Ajustado.Text = DIFAL.ToString("N2");
            tbcVlr_IPI_ImpostosProdutos_Comparativos_Ajustado.Text = IPI.ToString("N2");
        }

        /// <summary>
        /// Método utilizado para Popular o Repeater que exibe os Segmentos do Cliente na Etapa de Visualização.
        /// </summary>
        protected void PopulaSegmentosCliente()
        {
            try
            {
                if (hddidSegmentos.Value.Length > 1)
                {
                    div_rptSegmentos_View.Visible = true;

                    rptSegmentos_View.DataSource = hddidSegmentos.Value.Split('|').Where(s => s.Length > 0);
                    rptSegmentos_View.DataBind();
                }
                else
                    div_rptSegmentos_View.Visible = false;
            }
            catch (Exception ex)
            {
                MensagemPaginaDentro_View.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de Popular os Segmentos do Cliente para visualização!<br />Erro nos Segmentos: " + ex.Message, false);
            }
        }

        /// <summary>
        /// Método utilizado para Popular o Repeater que exibe os Tipos de Serviços selecionados, para Visualização.
        /// </summary>
        protected void PopulaTipoServicos()
        {
            try
            {
                if (hddsDscTiposServicos.Value.Length > 1)
                {
                    div_rptTiposServicos_View.Visible = true;

                    rptTiposServicos_View.DataSource = hddsDscTiposServicos.Value.Split('|').Where(s => s.Length > 0);
                    rptTiposServicos_View.DataBind();
                }
                else
                    div_rptTiposServicos_View.Visible = false;
            }
            catch (Exception ex)
            {
                MensagemPaginaDentro_View.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de Popular os Tipos de Serviços para visualização!<br />Erro nos Tipos: " + ex.Message, false);
            }
        }

        /// <summary>
        /// Método utilizado para Popular o Repeater que exibe os Escopos selecionados, para Visualização.
        /// </summary>
        protected void PopulaEscopos()
        {
            try
            {
                if (hddsDscEscopos.Value.Length > 1)
                {
                    div_rptEscopos_View.Visible = true;

                    rptEscopos_View.DataSource = hddsDscEscopos.Value.Split('|').Where(s => s.Length > 0);
                    rptEscopos_View.DataBind();
                }
                else
                    div_rptEscopos_View.Visible = false;
            }
            catch (Exception ex)
            {
                MensagemPaginaDentro_View.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de Popular os Escopos para visualização!<br />Erro nos Escopos para visualização: " + ex.Message, false);
            }
        }

        /// <summary>
        /// Método utilizado para Popular a Lista de Serviços para serem incluídos em uma Empreitada.
        /// </summary>
        protected void PopulaServicos_Empreitada()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_SERVICOS_RECURSOS_x_ORCAMENTO" },
                { "@idTipoOrcamento", hddidTipoOrcamento.Value },
                { "@sidTipoServicos", hddidTiposServicos.Value.Length > 1 ? hddidTiposServicos.Value : "" }
            };

            DataSet ds = ExecutarDataSet(sProcedure_Tipo, vParametros);

            if (ValidarDataSet(ds))
            {
                hddIncluirServicos.Value = string.Empty;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var item1 = listServicos_Incluir_Empreitada.FirstOrDefault(s => s.IdItem == int.Parse(row["idItem"].ToString()));

                    if (item1 == null)
                    {
                        hddIncluirServicos.Value += string.Format("[{0}|{1}|{2}]", row["idItem"].ToString(), row["sCodigo"].ToString(), row["sDscProduto"].ToString());

                        var novoServico_Recurso = new cls_Comercial_Tabelas
                        {
                            idRegistro = GerarNovo_idRegistro(listServicos_Recursos),
                            SFuncao = "INCLUIR ITEM",
                            nOrdem = listServicos_Recursos.Count(p => p.bLiberado) + 10,
                            IdItem = int.Parse(row["idItem"].ToString()),
                            SCodigo = row["sCodigo"].ToString(),
                            SDscProduto = row["sDscProduto"].ToString(),
                            TipoProduto = row["sTipo"].ToString(),
                            idTipo = int.Parse(row["idTipo"].ToString()),
                            IdGrupoProduto = int.Parse(row["idGrupo"].ToString()),
                            IdFamiliaProduto = int.Parse(row["idFamilia"].ToString()),
                            sDscGrupoProduto = row["sDscGrupo"].ToString(),
                            sDscFamiliaProduto = row["sDscFamilia"].ToString(),
                            SUnidade = row["sUnidade"].ToString(),
                            NQuantidade = 1,
                            NFator = Math.Round(decimal.Parse(txtDescontoServico.Text), 2),
                            bLiberado = false,
                            bProjeto = row["sProjeto"].ToString().ToUpper().Equals("S"),
                            dtInclusao = txtPrazoServico.Text,
                            idTipoRegra = int.Parse(row["idTipoRegra"].ToString())
                        };

                        listServicos_Incluir_Empreitada.Add(novoServico_Recurso);
                    }
                }

                if (hddIncluirServicos.Value == string.Empty)
                    hddIncluirServicos.Value = "[]";

                divIncluirServico_Empreitada.Visible = true;
            }
            else
            {
                listServicos_Incluir_Empreitada.Clear();

                divIncluirServico_Empreitada.Visible = false;
                MensagemPaginaServicos_Comparativos.MostraMensagem_Aviso("<b>Aviso:</b> Não existem Serviços para serem Inclusos nesta Empreitada, de acordo com o Tipo de Orçamento e os Tipos de Serviços selecionados!", false);
            }
        }

        protected string PopulaCampoTotal_ICMS(decimal nICMS, decimal nBaseCalc_ICMS, decimal nReducao)
        {
            return $@"
                    <div class='d-flex space-b fw-no btn-group'>
                        <span>{nICMS:N2}</span>
                        <button class='btn-link dropdown-toggle' data-toggle='dropdown'>
                            <i class='fa fa-chevron-up'></i>
                        </button>
                        <div class='dropdown-menu padd-0 m-0' style='width: max-content;'>
                            <table class='table table-bordered table-hover dataTable' style='width: max-content;'>
                                <tr>
                                    <th>Base de Cálculo do ICMS</th>
                                    <th>Redução</th>
                                </tr>
                                <tr>
                                    <td>{nBaseCalc_ICMS:N2}</td>
                                    <td>{nReducao:N2}</td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    ";
        }

        #endregion

        #region | Classes

        /// <summary>
        /// Método utilizado para Atualizar as Listas de Classes, atualizando as informações da Página.
        /// </summary>
        /// <param name="indexClasse">Recebe um Index que representa qual Lista de Classes deve ser atualizada.</param>
        protected void AtualizaClasses(int indexClasse)
        {
            switch (indexClasse)
            {
                case 0:
                    foreach (GridViewRow row in gvProdutos.Rows)
                    {
                        var produto = listProdutos.FirstOrDefault(p => row.Cells[Produtos_Coluna__ID].Text.Equals("0") ? p.SCodigo.ToLower().Trim().Equals((row.FindControl("cmdCodigo") as LinkButton).Text) : p.idRegistro.ToString() == row.Cells[Produtos_Coluna__ID].Text);

                        if (produto != null)
                        {
                            decimal.TryParse((row.Cells[Produtos_Coluna__Valor].FindControl("nPreco") as Label).Text, out decimal preco);
                            decimal.TryParse((row.Cells[Produtos_Coluna__Desconto].FindControl("nDesc_Produtos") as TextBox).Text, out decimal desconto);
                            decimal.TryParse((row.Cells[Produtos_Coluna__Quantidade].FindControl("nQuantidade_Produtos") as TextBox_Padrao).Text, out decimal qtd);
                            int.TryParse((row.Cells[Produtos_Composicao_Coluna__Ordem].FindControl("nOrdem") as TextBox).Text, out int ordem);

                            produto.bLiberado = true;

                            produto.nOrdem = ordem > 0 ? ordem : listProdutos.OrderBy(p => p.nOrdem).Last().nOrdem + 10;
                            produto.dtInclusao = string.IsNullOrEmpty((row.Cells[Produtos_Coluna__Prazo].FindControl("nPrazo") as TextBox).Text) ? "0" : (row.Cells[Produtos_Coluna__Prazo].FindControl("nPrazo") as TextBox).Text;
                            produto.Preco = Math.Round(preco, 2);
                            produto.NFator = Math.Round(desconto, 4);
                            produto.NQuantidade = Math.Round(qtd, 2);
                            produto.nUnitario = Math.Round(produto.Preco - (produto.Preco * (produto.NFator / 100)), 2);
                            produto.NTotal = Math.Round(produto.nUnitario * produto.NQuantidade, 2);

                            DataSet ds = ConsultaImpostos_Produtos(false, produto.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", produto.Preco.ToString().Replace(',', '.'));
                            decimal.TryParse(DATASET(ds, "nICMS"), out decimal nICMS);
                            int.TryParse(DATASET(ds, "idRegra"), out int idRegra);

                            try
                            {
                                produto.idRegra = idRegra;
                                produto.sCFOP = DATASET(ds, "sCFOP");
                                produto.sCST = DATASET(ds, "CST");
                                produto.NIPI = Math.Round(decimal.Parse(DATASET(ds, "nIPI")), 2);
                                produto.NPIS = Math.Round(decimal.Parse(DATASET(ds, "nPIS")), 2);
                                produto.NCOFINS = Math.Round(decimal.Parse(DATASET(ds, "nCOFINS")), 2);
                                produto.NICMS = Math.Round(nICMS, 2);
                                produto.NDIFAL = Math.Round(decimal.Parse(DATASET(ds, "nDIFAL")), 2);
                                produto.nReducao = Math.Round(decimal.Parse(DATASET(ds, "nRedBC")), 2);
                                produto.bBaseCalcICMS_com_IPI = DATASET(ds, "sBaseCalculo").Equals("PI");
                            }
                            catch { }

                            // Cálculo ST
                            if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !produto.bSistema && (ddlMoeda.SelectedValue == "0" || ddlMoeda.SelectedValue == "2"))
                            {
                                decimal.TryParse(DATASET(ds, "nMVA"), out decimal MVA);
                                decimal.TryParse(DATASET(ds, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);
                                string sMensagem_Erro_LegisWeb = ds != null && ds.Tables.Count > 1 ? DATASET(ds, 1, 0, "sMensagem_Erro_LegisWeb") ?? string.Empty : string.Empty;

                                try
                                {
                                    MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), (string.IsNullOrEmpty(DATASET(ds, "sICMSST")) ? string.Empty : DATASET(ds, "sICMSST")).Equals("S"), produto.sNCM, produto.sCEST, nICMS, DATASET(ds, "dtUltimaConsulta"), sMensagem_Erro_LegisWeb);

                                    if (MVA > 0)
                                    {
                                        var st = CalculaValor_ST(produto.NTotal, produto.nVlr_ICMS * produto.NQuantidade, MVA, ICMS_interno_destino);
                                        produto.nVlr_ST = st.Item1;
                                        produto.NST = st.Item2;
                                    }
                                }
                                catch { }
                            }

                            if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                            {
                                produto.NII = 0;
                                produto.NIPI = 0;
                                produto.NPIS = 0;
                                produto.NCOFINS = 0;
                                produto.NICMS = 0;
                                produto.NDIFAL = 0;
                                produto.NST = 0;

                                produto.NTotal *= cambio;
                            }

                            try { RecalculaImpostos(produto); }
                            catch { }

                            if (produto.bSistema)
                            {
                                if (row.FindControl("gvProdutos_Composicao") is GridView gv)
                                {
                                    foreach (GridViewRow rowComp in gv.Rows)
                                    {
                                        var composicao = listProdutos_Composicao.FirstOrDefault(p => p.idRegistro.ToString().Equals(rowComp.Cells[Produtos_Composicao_Coluna__ID].Text) && p.idItemPai.Equals(produto.idRegistro));

                                        if (composicao != null)
                                        {
                                            int.TryParse((rowComp.Cells[Produtos_Composicao_Coluna__Ordem].FindControl("nOrdem") as TextBox).Text, out int ordemComposicao);

                                            composicao.bLiberado = true;
                                            composicao.nOrdem = ordemComposicao;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 1:
                    foreach (GridViewRow row in gvServicos_Recursos.Rows)
                    {
                        var servico_recurso = listServicos_Recursos.FirstOrDefault(p => p.idRegistro.ToString() == (row.Cells[Servicos_Coluna__ID].FindControl("lblidRegistro") as Label).Text);

                        if (servico_recurso != null)
                        {
                            if ((row.Cells[row.Cells.Count - 1].FindControl("cbValidado") as CheckBox).Checked)
                            {
                                servico_recurso.SFuncao = "INCLUIR ITEM";
                                servico_recurso.bLiberado = true;
                            }
                            else
                            {
                                servico_recurso.SFuncao = "EXCLUIR ITEM";
                                servico_recurso.bLiberado = false;
                            }

                            decimal.TryParse((row.Cells[Servicos_Coluna__Valor].FindControl("hddPreco") as HiddenField).Value, out decimal preco);
                            decimal.TryParse((row.Cells[Servicos_Coluna__Margem].FindControl("hddMargem") as HiddenField).Value, out decimal margem);
                            decimal.TryParse((row.Cells[Servicos_Coluna__Desconto].FindControl("nDesconto") as TextBox_Padrao).Text, out decimal desconto);
                            decimal.TryParse((row.Cells[Servicos_Coluna__Quantidade].FindControl("nQtd") as TextBox_Padrao).Text, out decimal qtd);
                            int.TryParse((row.Cells[Servicos_Coluna__Ordem].FindControl("nOrdem") as TextBox).Text, out int ordem);

                            servico_recurso.dtInclusao = (row.Cells[Servicos_Coluna__Prazo].FindControl("nPrazo") as TextBox).Text;
                            servico_recurso.nOrdem = ordem;
                            servico_recurso.NQuantidade = Math.Round(qtd, 2);
                            servico_recurso.NFator = Math.Round(desconto, 2);
                            servico_recurso.Preco = Math.Round(preco, 2);
                            servico_recurso.NMargem = Math.Round(margem, 2);

                            decimal total = (servico_recurso.Preco * servico_recurso.NQuantidade * servico_recurso.NMargem) - (servico_recurso.Preco * servico_recurso.NQuantidade * servico_recurso.NMargem * (servico_recurso.NFator / 100));

                            servico_recurso.NTotal = Math.Round(servico_recurso.NAjuste > decimal.Zero ? (total * (servico_recurso.NAjuste / 100)) + total : total - (total * (servico_recurso.NAjuste * -1 / 100)), 2);

                            if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                                servico_recurso.NTotal *= cambio;

                            var gv_1 = row.FindControl("gvServicos_Recursos_Composicao_1") as GridView;
                            if (gv_1 != null)
                            {
                                foreach (GridViewRow row_1 in gv_1.Rows)
                                {
                                    var subServico = listServicos_Recursos_Composicao_Filhos.FirstOrDefault(s => s.idRegistro.ToString().Equals((row_1.Cells[Servicos_Composicao_1_Coluna__ID].FindControl("lblidRegistro") as Label).Text)
                                                                                                        && s.idItemPai.Equals(servico_recurso.idRegistro) && s.idItemAvo.Equals(servico_recurso.idItemPai) && s.idItemBisavo.Equals(servico_recurso.idItemAvo));

                                    if (subServico != null)
                                    {
                                        if ((row_1.Cells[row_1.Cells.Count - 1].FindControl("cbValidado") as CheckBox).Checked)
                                        {
                                            subServico.SFuncao = "INCLUIR ITEM";
                                            subServico.bLiberado = true;
                                        }
                                        else
                                        {
                                            subServico.SFuncao = "EXCLUIR ITEM";
                                            subServico.bLiberado = false;
                                        }

                                        decimal.TryParse((row_1.Cells[Servicos_Composicao_1_Coluna__Valor].FindControl("hddPreco") as HiddenField).Value, out decimal preco_1);
                                        decimal.TryParse((row_1.Cells[Servicos_Composicao_1_Coluna__Margem].FindControl("hddMargem") as HiddenField).Value, out decimal margem_1);
                                        decimal.TryParse((row_1.Cells[Servicos_Composicao_1_Coluna__Quantidade].FindControl("nQtd") as TextBox_Padrao).Text, out decimal qtd_1);

                                        subServico.NQuantidade = Math.Round(qtd_1, 2);
                                        subServico.Preco = Math.Round(preco_1, 2);
                                        subServico.NMargem = Math.Round(margem_1, 2);

                                        subServico.NTotal = Math.Round(preco_1 * qtd_1 * margem_1, 2);

                                        var gv_2 = row_1.FindControl("gvServicos_Recursos_Composicao_2") as GridView;

                                        if (gv_2 != null)
                                        {
                                            foreach (GridViewRow row_2 in gv_2.Rows)
                                            {
                                                var recurso_1 = listServicos_Recursos_Composicao_Netos.FirstOrDefault(s => s.idRegistro.ToString().Equals((row_2.Cells[Servicos_Composicao_2_Coluna__ID].FindControl("lblidRegistro") as Label).Text)
                                                                                                                    && s.idItemPai.Equals(subServico.idRegistro) && s.idItemAvo.Equals(subServico.idItemPai) && s.idItemBisavo.Equals(subServico.idItemAvo));

                                                if (recurso_1 != null)
                                                {
                                                    if ((row_2.Cells[row_2.Cells.Count - 1].FindControl("cbValidado") as CheckBox).Checked)
                                                    {
                                                        recurso_1.SFuncao = "INCLUIR ITEM";
                                                        recurso_1.bLiberado = true;
                                                    }
                                                    else
                                                    {
                                                        recurso_1.SFuncao = "EXCLUIR ITEM";
                                                        recurso_1.bLiberado = false;
                                                    }

                                                    decimal.TryParse((row_2.Cells[Servicos_Composicao_2_Coluna__Valor].FindControl("nPreco") as TextBox_Padrao).Text, out decimal preco_2);
                                                    decimal.TryParse((row_2.Cells[Servicos_Composicao_2_Coluna__Margem].FindControl("nMargem") as TextBox).Text, out decimal margem_2);
                                                    decimal.TryParse((row_2.Cells[Servicos_Composicao_2_Coluna__Quantidade].FindControl("nQtd") as TextBox_Padrao).Text, out decimal qtd_2);

                                                    recurso_1.NQuantidade = Math.Round(qtd_2, 2);
                                                    recurso_1.Preco = Math.Round(preco_2, 2);
                                                    recurso_1.NMargem = Math.Round(margem_2, 2);

                                                    recurso_1.NTotal = Math.Round(preco_2 * qtd_2 * margem_2, 2);

                                                    var gv_3 = row_2.FindControl("gvServicos_Recursos_Composicao_3") as GridView;

                                                    if (gv_3 != null)
                                                    {
                                                        foreach (GridViewRow row_3 in gv_3.Rows)
                                                        {
                                                            var recurso_2 = listServicos_Recursos_Composicao_Bisnetos.FirstOrDefault(s => s.idRegistro.ToString().Equals((row_3.Cells[Servicos_Composicao_3_Coluna__ID].FindControl("lblidRegistro") as Label).Text)
                                                                                                                                && s.idItemPai.Equals(recurso_1.idRegistro) && s.idItemAvo.Equals(recurso_1.idItemPai) && s.idItemBisavo.Equals(recurso_1.idItemAvo));

                                                            if (recurso_2 != null)
                                                            {
                                                                if ((row_3.Cells[row_3.Cells.Count - 1].FindControl("cbValidado") as CheckBox).Checked)
                                                                {
                                                                    recurso_2.SFuncao = "INCLUIR ITEM";
                                                                    recurso_2.bLiberado = true;
                                                                }
                                                                else
                                                                {
                                                                    recurso_2.SFuncao = "EXCLUIR ITEM";
                                                                    recurso_2.bLiberado = false;
                                                                }

                                                                decimal.TryParse((row_3.Cells[Servicos_Composicao_3_Coluna__Valor].FindControl("nPreco") as TextBox_Padrao).Text, out decimal preco_3);
                                                                decimal.TryParse((row_3.Cells[Servicos_Composicao_3_Coluna__Margem].FindControl("nMargem") as TextBox).Text, out decimal margem_3);
                                                                decimal.TryParse((row_3.Cells[Servicos_Composicao_3_Coluna__Quantidade].FindControl("nQtd") as TextBox_Padrao).Text, out decimal qtd_3);

                                                                recurso_2.NQuantidade = Math.Round(qtd_3, 2);
                                                                recurso_2.Preco = Math.Round(preco_3, 2);
                                                                recurso_2.NMargem = Math.Round(margem_3, 2);

                                                                recurso_2.NTotal = Math.Round(preco_3 * qtd_3 * margem_3, 2);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Método utilizado para chamar o Método 'AtualizaClasses', em todas as suas opções, desta forma, atualizando todas as Listas de Classes necessárias de uma vez.
        /// </summary>
        protected void AtualizaClasseGeral()
        {
            AtualizaClasses(0);
            AtualizaClasses(1);

            if (dt_CheckList.Rows.Count > 0)
            {
                Atualiza_Opcoes_CheckList();
                rptCategoriasEscopos_dataBind();
                hddOpcaoSelecionada.Value = "";
            }

            Atualiza_Repeaters();
        }

        /// <summary>
        /// Método utilizado para Atualizar as Listas de Classes, utilizadas na aba de Comparativos, atualizando suas informações de acordo com os campos da Página.
        /// </summary>
        protected void AtualizaClasses_Comparativo()
        {
            bool bEmpreitada = Convert.ToBoolean(hddComparativos_Empreitada.Value);
            bool bDrawback = Convert.ToBoolean(hddsDrawback.Value);

            if (!bEmpreitada)
                txtComparativos_LiquidoProdutos_Ajustado.ReadOnly = true;
            else
                txtComparativos_LiquidoProdutos_Ajustado.ReadOnly = false;

            if (!bDrawback)
            {
                if (!bEmpreitada && listProdutos.Count > 0 && !listProdutos.Any(p => p.bSistema) && listServicos_Recursos.Count > 0)
                    div_cmdEmpreitada.Visible = true;

                foreach (GridViewRow row in gvComparativoProdutos.Rows)
                {
                    var produto = listProdutos_Comparativos.FirstOrDefault(p => p.idRegistro.ToString() == row.Cells[Produtos_Comparativos_Coluna__ID].Text);

                    if (hddComparativoProdutos.Value != "S")
                        produto = listProdutos.FirstOrDefault(p => p.idRegistro.ToString() == row.Cells[Produtos_Comparativos_Coluna__ID].Text);

                    if (bEmpreitada)
                        produto = listProdutos_Empreitada.FirstOrDefault(p => p.idRegistro.ToString() == row.Cells[Produtos_Comparativos_Coluna__ID].Text);

                    if (produto != null)
                    {
                        produto.nOrdem = int.Parse((row.Cells[Produtos_Comparativos_Coluna__Ordem_Com_Composicao].FindControl("txtnOrdem") as TextBox).Text);
                        produto.dtInclusao = (row.Cells[Produtos_Comparativos_Coluna__Prazo_Edição].FindControl("prazo") as TextBox).Text;
                        produto.NFator = Math.Round(decimal.Parse((row.Cells[Produtos_Comparativos_Coluna__Desconto].FindControl("nDto") as TextBox).Text), 4);
                        produto.nUnitario = Math.Round(produto.Preco - (produto.Preco * (produto.NFator / 100)), 2);
                        produto.NTotal = Math.Round(produto.nUnitario * produto.NQuantidade, 2);

                        try
                        {
                            RecalculaImpostos(produto);
                        }
                        catch { }

                        // Cálculo ST
                        if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !produto.bSistema)
                        {
                            DataSet ds = ConsultaImpostos_Produtos(false, produto.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", produto.Preco.ToString().Replace(',', '.'));

                            decimal.TryParse(DATASET(ds, "nMVA"), out decimal MVA);
                            decimal.TryParse(DATASET(ds, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                            try
                            {
                                MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(ds, "sICMSST").Equals("S"), produto.sNCM, produto.sCEST, decimal.Parse(DATASET(ds, "nICMS")), DATASET(ds, "dtUltimaConsulta"), DATASET(ds, 1, 0, "sMensagem_Erro_LegisWeb"));

                                if (MVA > 0)
                                {
                                    var st = CalculaValor_ST(produto.NTotal, produto.nVlr_ICMS * produto.NQuantidade, MVA, ICMS_interno_destino);
                                    produto.nVlr_ST = st.Item1;
                                    produto.NST = st.Item2;
                                }
                            }
                            catch { }
                        }

                        if (produto.bSistema && !bEmpreitada)
                        {
                            var gv = row.FindControl("gvProdutos_Composicao_Comparativos") as GridView;

                            if (gv != null)
                            {
                                foreach (GridViewRow rowComp in gv.Rows)
                                {
                                    var composicao = listProdutos_Comparativos_Composicao.FirstOrDefault(p => p.idRegistro.ToString() == rowComp.Cells[Produtos_Comparativos_Composicao_Coluna__ID].Text && p.idItemPai.Equals(produto.idRegistro));

                                    if (hddComparativoProdutos.Value != "S")
                                        composicao = listProdutos_Composicao.FirstOrDefault(p => p.idRegistro.ToString() == rowComp.Cells[Produtos_Comparativos_Composicao_Coluna__ID].Text && p.idItemPai.Equals(produto.idRegistro));

                                    if (composicao != null)
                                    {
                                        decimal.TryParse((rowComp.Cells[Produtos_Comparativos_Composicao_Coluna__Quantidade].FindControl("txtnQuantidade") as TextBox_Padrao).Text, out decimal qtd);
                                        int.TryParse((rowComp.Cells[Produtos_Comparativos_Composicao_Coluna__Ordem].FindControl("txtnOrdem") as TextBox).Text, out int ordemComposicao);

                                        composicao.bLiberado = true;

                                        composicao.nOrdem = ordemComposicao;
                                        composicao.NQuantidade = qtd;
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (GridViewRow row in gvComparativoServicos.Rows)
                {
                    var servico = listServicos_Comparativos.FirstOrDefault(s => s.idRegistro.ToString().Equals(row.Cells[Servicos_Comparativos_Coluna__ID].Text));

                    if (bEmpreitada)
                        servico = listServicos_Empreitada.FirstOrDefault(s => s.idRegistro.ToString().Equals(row.Cells[Servicos_Comparativos_Coluna__ID].Text));

                    if (servico != null)
                    {
                        servico.nOrdem = int.Parse((row.Cells[Servicos_Comparativos_Coluna__Ordem].FindControl("txtnOrdem") as TextBox).Text);
                        servico.dtInclusao = (row.FindControl("prazo") as TextBox).Text;
                    }
                }
            }

            ddlStatus.Visible = false;
            cmdVincular_Pedido_group.Visible = false;
            cmdEditar.Visible = false;
            cmdEditar_View_CheckList.Visible = false;
            cmdEditar_View_Servicos.Visible = false;
            cmdEditar_View_Produtos.Visible = false;
            cmdEditar_View_Historico.Visible = false;
            cmdAplicar_Comparativos.Visible = false;

            gvComparativoProdutos_dataBind();
            gvComparativoServicos_dataBind();

            if (!aba_Comparativo_Produtos.Visible)
                Scripts.Mantem_AbaAtiva(Page, "aba-Comparativo_Servicos");

            PopulaTotais_Comparativo();
            AlinhaPagina();

            try
            {
                if (bEmpreitada)
                {
                    if (decimal.Parse(string.IsNullOrEmpty(txtTotalProdutos_Empreitada.Text) ? "0.00" : txtTotalProdutos_Empreitada.Text) <= decimal.Parse(hddMax_Material_Empreitada.Value))
                        cmdAplicar_Comparativos.Visible = true;
                    else
                        cmdAplicar_Comparativos.Visible = false;
                }
                else
                    cmdAplicar_Comparativos.Visible = true;
            }
            catch { }

            if (bEmpreitada)
            {
                int valor = hddMunicipio_Origem.Value.ToUpper() == "SAO PAULO" && hddMunicipio_Fiscal.Value.ToUpper() == "SAO PAULO" ? 40 : 50;
                hddMax_Material_Empreitada.Value = valor.ToString();
                MensagemPagina_Comparativos_Fixa.MostraMensagem_Aviso("<b>ATENÇÃO</b>, será necessário que o Valor Total Líquido dos Produtos não ultrapasse o limite de " + valor + "%, em comparação ao Valor Total Líquido do Orçamento!", false);
            }

            if (Math.Round(decimal.Parse(txtComparativos_BrutoServicos_Ajustado.Text.Replace(".", "").Replace(",", ".")), 2) < Math.Round(decimal.Parse(txtComparativo_BrutoServicos.Text.Replace(".", "").Replace(",", ".")), 2))
                MensagemPagina_Comparativos.MostraMensagem_Aviso("<b>Aviso:</b> Com as alterações atuais, o Total Líquido do Orçamento foi reduzido!");
        }

        /// <summary>
        /// Método utilizado para Atualizar a Lista de Classes com as Opções selecionadas nas CheckLists.
        /// </summary>
        protected void Atualiza_Opcoes_CheckList()
        {
            string[] opcoes = hddOpcaoSelecionada.Value.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string dado in opcoes)
            {
                int idEscopo = Convert.ToInt32(dado.Split('|')[0]);
                int idCategoria = Convert.ToInt32(dado.Split('|')[1]);
                string pergunta = dado.Split('|')[2];
                string opcao = dado.Split('|')[3] + "|" + dado.Split('|')[4];

                string pergunta_1 = pergunta;

                if (pergunta.Split(';').Length > 1)
                {
                    pergunta = pergunta.Split(';')[0] + "|" + pergunta.Replace(pergunta.Split(';')[0] + ";", "");
                    pergunta_1 = pergunta.Split('|')[1];
                }

                var pergunta_x_opcao = list_Perguntas_x_Opcoes.FirstOrDefault(p => p.idEscopo.Equals(idEscopo) && p.idCategoria.Equals(idCategoria) && p.sPerguntas.Equals(pergunta_1));

                if (pergunta_x_opcao != null)
                {
                    pergunta_x_opcao.sPerguntas = pergunta;
                    pergunta_x_opcao.sOpcoes = opcao;
                }
                else
                {
                    pergunta_x_opcao = list_Perguntas_x_Opcoes.FirstOrDefault(p => p.idEscopo.Equals(idEscopo) && p.idCategoria.Equals(idCategoria) && p.sPerguntas.Equals(pergunta));

                    if (pergunta_x_opcao != null)
                    {
                        pergunta_x_opcao.sPerguntas = pergunta;
                        pergunta_x_opcao.sOpcoes = opcao;
                    }
                    else
                    {
                        cls_Categoria c = new cls_Categoria
                        {
                            idEscopo = idEscopo,
                            idCategoria = idCategoria,
                            sPerguntas = pergunta,
                            sOpcoes = opcao
                        };

                        list_Perguntas_x_Opcoes.Add(c);
                    }
                }
            }

            int nPerguntas = 0;

            foreach (DataRow row in dt_CheckList.Rows)
            {
                nPerguntas += (row.Field<string>("sPerguntas") ?? "").Split('|').Count(s => s.Length > 0);
            }

            if (list_Perguntas_x_Opcoes.Count < nPerguntas)
            {
                foreach (RepeaterItem item in rptCategoriaEscopos.Items)
                {
                    GridView gv = item.FindControl("gvCheckList") as GridView;

                    if (gv != null)
                    {
                        foreach (GridViewRow row in gv.Rows)
                        {
                            if (row.RowType == DataControlRowType.DataRow)
                            {
                                int idEscopo = int.Parse(row.Cells[Escopos_Coluna__idEscopo].Text);
                                int idCategoria = int.Parse(row.Cells[Escopos_Coluna__idCategoria].Text);
                                string pergunta = row.ID + "|" + (row.Cells[Escopos_Coluna__Pergunta].Controls[1] as Label).Text;

                                var pergunta_x_opcao = list_Perguntas_x_Opcoes.FirstOrDefault(p => p.idEscopo.Equals(idEscopo) && p.idCategoria.Equals(idCategoria) && p.sPerguntas.Equals(pergunta));

                                if (pergunta_x_opcao == null)
                                {
                                    cls_Categoria c = new cls_Categoria
                                    {
                                        idEscopo = idEscopo,
                                        idCategoria = idCategoria,
                                        sPerguntas = pergunta,
                                        sOpcoes = "0|" + gv.HeaderRow.Cells[Escopos_Coluna__PrimeiraOpcao].Text
                                    };

                                    list_Perguntas_x_Opcoes.Add(c);
                                }
                            }
                        }
                    }
                }
            }

            hddOpcaoSelecionada.Value = "";
        }

        /// <summary>
        /// Método utilizado para Manter os Produtos mesmo após alterações que afetem os Valores dos Produtos, como por exemplo, ao Selecionar um Parceiro, ou Tabela de Preço.
        /// </summary>
        /// <param name="idTabela">Recebe o ID da Tabela de Preço selecionada.</param>
        protected void MantemProdutos(string idTabela)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idTabela", idTabela }
            };
            DataSet ds = ExecutarDataSet(sProcedure_TabelaPreco, vParametros);

            if (ValidarDataSet(ds))
            {
                try
                {
                    ds.Tables[1].PrimaryKey = new DataColumn[] { ds.Tables[1].Columns[0] };

                    foreach (cls_Comercial_Tabelas produto in listProdutos.Where(p => p.bLiberado))
                    {
                        try
                        {
                            if (ds.Tables[1].Rows.Find(produto.IdItem) is DataRow row)
                            {
                                produto.SFuncao = "INCLUIR ITEM";
                                produto.Preco = Math.Round(decimal.Parse(row["nTotal"].ToString()), 2);
                                produto.nUnitario = Math.Round(produto.Preco - (produto.Preco * (produto.NFator / 100)), 2);
                                produto.NTotal = Math.Round(produto.nUnitario * produto.NQuantidade, 2);
                                produto.bImportado = true;

                                if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                                {
                                    produto.NII = 0;
                                    produto.NIPI = 0;
                                    produto.NPIS = 0;
                                    produto.NCOFINS = 0;
                                    produto.NICMS = 0;
                                    produto.NDIFAL = 0;
                                    produto.NST = 0;

                                    produto.NTotal *= cambio;
                                }

                                try
                                {
                                    RecalculaImpostos(produto);
                                }
                                catch { }

                                // Cálculo ST
                                if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !produto.bSistema && (ddlMoeda.SelectedValue == "0" || ddlMoeda.SelectedValue == "2"))
                                {
                                    DataSet dsRegras = ConsultaImpostos_Produtos(false, produto.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", produto.Preco.ToString().Replace(',', '.'));

                                    decimal.TryParse(DATASET(dsRegras, "nMVA"), out decimal MVA);
                                    decimal.TryParse(DATASET(dsRegras, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                                    MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(dsRegras, "sICMSST").Equals("S"), produto.sNCM, produto.sCEST, decimal.Parse(DATASET(dsRegras, "nICMS")), DATASET(dsRegras, "dtUltimaConsulta"), DATASET(dsRegras, 1, 0, "sMensagem_Erro_LegisWeb"));

                                    try
                                    {
                                        if (MVA > 0)
                                        {
                                            var st = CalculaValor_ST(produto.NTotal, produto.nVlr_ICMS * produto.NQuantidade, MVA, ICMS_interno_destino);
                                            produto.nVlr_ST = st.Item1;
                                            produto.NST = st.Item2;
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    MensagemPaginaInfoInicial.MostraMensagem_Aviso($"<b>Aviso: </b>Não foi possível atualizar automaticamente os valores dos Produtos presentes no Orçamento, de acordo com a Tabela de Preços selecionada!<br />Possível erro ao tentar atualizar automaticamente: {ex.Message}", false);
                }
            }

            gvProdutos_dataBind();
        }

        /// <summary>
        /// Método utilizado para Excluir Todos os Produtos presentes no Orçamento.
        /// </summary>
        protected void Excluir_Produtos()
        {
            try
            {
                AtualizaClasseGeral();

                foreach (GridViewRow row in gvProdutos.Rows)
                {
                    if ((row.FindControl("cbExcluir_Produto") as CheckBox).Checked)
                    {
                        string sCodigo = (row.FindControl("cmdCodigo") as LinkButton).Text.ToLower().Trim();
                        if (int.TryParse(row.Cells[Produtos_Coluna__ID].Text, out int id) && !string.IsNullOrEmpty(sCodigo))
                        {
                            var produto = listProdutos.FirstOrDefault(p => p.idRegistro.Equals(id) && p.SCodigo.ToLower().Trim().Equals(sCodigo));

                            if (produto != null)
                            {
                                if (produto.IdItem.Equals(0))
                                    listProdutos.Remove(produto);
                                else
                                {
                                    produto.SFuncao = "EXCLUIR ITEM";
                                    produto.bLiberado = false;
                                }
                            }
                        }
                    }
                }

                gvProdutos_dataBind();
                MantemEtapa_Pos_PostBack(4);
                AtualizaBarraProgresso(4, false);
            }
            catch (Exception ex)
            {
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro ao Excluir os Produtos selecionados!<br /> Erro ao Excluir: " + ex.Message, false);
            }
        }

        /// <summary>
        /// Método utilizado para Importar Itens de diferentes origens, utilizando um mesmo processo.
        /// </summary>
        /// <param name="dt">Recebe o DataTable com os Dados dos Produtos a serem Importados.</param>
        protected void ImportarItens(DataTable dt)
        {
            if (dt != null)
            {
                AtualizaClasseGeral();
                string mensagem = "";

                foreach (DataRow item in dt.Rows)
                {
                    DataSet ds = ConsultaImpostos_Produtos(true, item.Field<string>("sCodigo").Trim(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : hddUF_Fiscal_SelectedValue.Value, ddlTabela.SelectedValue);

                    if (ValidarDataSet(ds))
                    {
                        string msg = "";
                        string sNaoExiste = "";
                        decimal nQuantidade = 0;

                        try { decimal.TryParse(item.Field<decimal>("nQuantidade").ToString(), out nQuantidade); }
                        catch { }

                        try { msg = DATASET(ds, "sMsg"); }
                        catch { }

                        try { sNaoExiste = DATASET(ds, "sNaoExiste"); }
                        catch { }

                        if (!string.IsNullOrEmpty(msg))
                            mensagem += string.Format("<br /><br />{0}", msg);
                        else
                        {
                            int.TryParse(DATASET(ds, "idItem"), out int idItem);

                            var novoProduto = new cls_Comercial_Tabelas
                            {
                                idRegistro = GerarNovo_idRegistro(listProdutos),
                                SFuncao = "INCLUIR ITEM",
                                SCodigo = item.Field<string>("sCodigo").Trim(),
                                nOrdem = listProdutos.Any(p => p.bLiberado) ? listProdutos.OrderBy(p => p.nOrdem).Last(p => p.bLiberado).nOrdem + 10 : 10,
                                IdItem = 0,

                                NQuantidade = nQuantidade > 0 ? nQuantidade : 1,

                                bLiberado = true,
                                dtInclusao = txtPrazoProduto.Text
                            };

                            if (string.IsNullOrEmpty(sNaoExiste))
                            {
                                string ncm = DATASET(ds, 1, 0, "sCodigoNCM") == "0" ? "Não Cadastrado" : DATASET(ds, 1, 0, "sCodigoNCM");
                                string cest = DATASET(ds, 1, 0, "sCodigoCEST") == "0" ? "Não Cadastrado" : DATASET(ds, 1, 0, "sCodigoCEST");
                                int.TryParse(DATASET(ds, 1, 0, "idRegra"), out int idRegra);
                                decimal.TryParse(DATASET(ds, 1, 0, "nMVA"), out decimal MVA);
                                decimal.TryParse(DATASET(ds, 1, 0, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                                try
                                {
                                    MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(ds, 1, 0, "sICMSST").Equals("S"), ncm, cest, decimal.Parse(DATASET(ds, 1, 0, "nICMS")), DATASET(ds, "dtUltimaConsulta"), DATASET(ds, 2, 0, "sMensagem_Erro_LegisWeb"));
                                }
                                catch { }

                                novoProduto = new cls_Comercial_Tabelas
                                {
                                    idRegistro = GerarNovo_idRegistro(listProdutos),
                                    SFuncao = "INCLUIR ITEM",
                                    nOrdem = listProdutos.Any(p => p.bLiberado) ? listProdutos.OrderBy(p => p.nOrdem).Last(p => p.bLiberado).nOrdem + 10 : 10,
                                    IdItem = idItem,
                                    SCodigo = item.Field<string>("sCodigo").Trim(),
                                    SDscProduto = DATASET(ds, "sDscProduto"),
                                    sNCM = ncm,
                                    sCEST = cest,
                                    TipoProduto = DATASET(ds, "sTipoProduto"),
                                    IdGrupoProduto = int.Parse(DATASET(ds, "idGrupo")),
                                    IdFamiliaProduto = int.Parse(DATASET(ds, "idFamilia")),
                                    sDscGrupoProduto = DATASET(ds, "sDscGrupo"),
                                    sDscFamiliaProduto = DATASET(ds, "sDscFamilia"),
                                    SUnidade = DATASET(ds, "sUnidade"),
                                    sIndustrializado = DATASET(ds, 1, 0, "sProdutoIndustrializado") == "S" ? "Sim" : "Não",
                                    sOrigem = DATASET(ds, 1, 0, "sProdutoImportado") == "S" ? "IMP" : "BR",
                                    idRegra = idRegra,

                                    sCFOP = DATASET(ds, 1, 0, "sCFOP"),
                                    sCST = DATASET(ds, 1, 0, "CST"),

                                    NIPI = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nIPI")), 2),
                                    NPIS = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nPIS")), 2),
                                    NCOFINS = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nCOFINS")), 2),
                                    NICMS = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nICMS")), 2),
                                    NDIFAL = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nDIFAL")), 2),
                                    NST = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nICMSST")), 2),

                                    bBaseCalcICMS_com_IPI = DATASET(ds, 1, 0, "sBaseCalculo").Equals("PI"),

                                    NQuantidade = nQuantidade > 0 ? nQuantidade : 1,
                                    nPesoBruto = Math.Round(decimal.Parse(DATASET(ds, "nPesoBruto")), 2),
                                    nPesoLiquido = Math.Round(decimal.Parse(DATASET(ds, "nPesoNeto")), 2),
                                    nVolume = Math.Round(decimal.Parse(DATASET(ds, "nVolume")), 2),
                                    NFator = Math.Round(decimal.Parse(string.IsNullOrEmpty(txtDescontoProduto.Text) ? "0" : txtDescontoProduto.Text), 4),
                                    Preco = Math.Round(decimal.Parse(DATASET(ds, "nTotal")), 2),

                                    bLiberado = true,
                                    dtInclusao = txtPrazoProduto.Text
                                };

                                novoProduto.nUnitario = Math.Round(novoProduto.Preco - (novoProduto.Preco * (novoProduto.NFator / 100)), 2);
                                novoProduto.NTotal = Math.Round(novoProduto.nUnitario * novoProduto.NQuantidade, 2);

                                if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                                {
                                    novoProduto.NII = 0;
                                    novoProduto.NIPI = 0;
                                    novoProduto.NPIS = 0;
                                    novoProduto.NCOFINS = 0;
                                    novoProduto.NICMS = 0;
                                    novoProduto.NDIFAL = 0;
                                    novoProduto.NST = 0;

                                    novoProduto.NTotal *= cambio;
                                }

                                try
                                {
                                    RecalculaImpostos(novoProduto);
                                }
                                catch { }

                                // Cálculo ST
                                if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !novoProduto.bSistema)
                                {
                                    try
                                    {
                                        if (MVA > 0)
                                        {
                                            var st = CalculaValor_ST(novoProduto.NTotal, novoProduto.nVlr_ICMS * novoProduto.NQuantidade, MVA, ICMS_interno_destino);
                                            novoProduto.nVlr_ST = st.Item1;
                                            novoProduto.NST = st.Item2;
                                        }
                                    }
                                    catch { }
                                }
                            }

                            listProdutos.Add(novoProduto);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(mensagem))
                    throw new Exception(string.Format("<br />- Alguns Produtos não foram Importados corretamente!<br />{0}", mensagem));
            }
            else throw new Exception("Não foram encontrados Itens para serem Importados!");
        }

        /// <summary>
        /// Método utilizado para Unificar Produtos Duplicados no Orçamento.
        /// </summary>
        /// <param name="bTodos">Recebe um valor que define se Todos os Itens Duplicados, presentes no Orçamento, devem ser Unificados.</param>
        protected void UnificarProdutosDuplicados(bool bTodos)
        {
            if (bTodos)
            {
                try
                {
                    var produtosDuplicados = listProdutos.Where(p => p.IdItem > 0 && p.bLiberado).GroupBy(p => p.IdItem).Where(g => g.Skip(1).Any());

                    foreach (var duplicado in produtosDuplicados)
                    {
                        var produto = listProdutos.FirstOrDefault(p => p.IdItem.Equals(duplicado.Key));

                        if (produto != null)
                        {
                            int ordem = 0;
                            decimal qtd = decimal.Zero;

                            foreach (var item in listProdutos.Where(p => p.IdItem.Equals(produto.IdItem) && p.bLiberado))
                            {
                                item.SFuncao = "EXCLUIR ITEM";
                                item.bLiberado = false;

                                ordem = ordem.Equals(0) || item.nOrdem < ordem ? item.nOrdem : ordem;
                                qtd += item.NQuantidade;
                            }

                            produto.SFuncao = "INCLUIR ITEM";
                            produto.bLiberado = true;
                            produto.nOrdem = ordem;
                            produto.NQuantidade = qtd;
                            produto.nUnitario = Math.Round(produto.Preco - (produto.Preco * (produto.NFator / 100)), 2);
                            produto.NTotal = Math.Round(produto.nUnitario * produto.NQuantidade, 2);

                            if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                            {
                                produto.NII = 0;
                                produto.NIPI = 0;
                                produto.NPIS = 0;
                                produto.NCOFINS = 0;
                                produto.NICMS = 0;
                                produto.NDIFAL = 0;
                                produto.NST = 0;

                                produto.NTotal *= cambio;
                            }

                            try
                            {
                                RecalculaImpostos(produto);
                            }
                            catch { }

                            // Cálculo ST
                            if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !produto.bSistema && (ddlMoeda.SelectedValue == "0" || ddlMoeda.SelectedValue == "2"))
                            {
                                DataSet ds = ConsultaImpostos_Produtos(false, produto.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", produto.Preco.ToString().Replace(',', '.'));

                                decimal.TryParse(DATASET(ds, "nMVA"), out decimal MVA);
                                decimal.TryParse(DATASET(ds, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                                try
                                {
                                    MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(ds, "sICMSST").Equals("S"), produto.sNCM, produto.sCEST, decimal.Parse(DATASET(ds, "nICMS")), DATASET(ds, "dtUltimaConsulta"), DATASET(ds, 1, 0, "sMensagem_Erro_LegisWeb"));

                                    if (MVA > 0)
                                    {
                                        var st = CalculaValor_ST(produto.NTotal, produto.nVlr_ICMS * produto.NQuantidade, MVA, ICMS_interno_destino);
                                        produto.nVlr_ST = st.Item1;
                                        produto.NST = st.Item2;
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> Não foi possível Unificar Todos os Produtos Duplicados!<br />Erro ao Unificar Todos: " + ex.Message, true);
                }
            }
            else
            {
                try
                {
                    int.TryParse(hddUnificarProdutos.Value, out int idRegistro);
                    var produto = listProdutos.FirstOrDefault(p => p.idRegistro.Equals(idRegistro));

                    if (produto != null)
                    {
                        int ordem = 0;
                        decimal qtd = decimal.Zero;

                        foreach (var item in listProdutos.Where(p => p.IdItem.Equals(produto.IdItem) && p.bLiberado))
                        {
                            item.SFuncao = "EXCLUIR ITEM";
                            item.bLiberado = false;

                            ordem = ordem.Equals(0) || item.nOrdem < ordem ? item.nOrdem : ordem;
                            qtd += item.NQuantidade;
                        }

                        produto.SFuncao = "INCLUIR ITEM";
                        produto.bLiberado = true;

                        produto.nOrdem = ordem;
                        produto.NQuantidade = qtd;
                        produto.nUnitario = Math.Round(produto.Preco - (produto.Preco * (produto.NFator / 100)), 2);
                        produto.NTotal = Math.Round(produto.nUnitario * produto.NQuantidade, 2);

                        if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                        {
                            produto.NII = 0;
                            produto.NIPI = 0;
                            produto.NPIS = 0;
                            produto.NCOFINS = 0;
                            produto.NICMS = 0;
                            produto.NDIFAL = 0;
                            produto.NST = 0;

                            produto.NTotal *= cambio;
                        }

                        try
                        {
                            RecalculaImpostos(produto);
                        }
                        catch { }

                        // Cálculo ST
                        if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !produto.bSistema && (ddlMoeda.SelectedValue == "0" || ddlMoeda.SelectedValue == "2"))
                        {
                            DataSet ds = ConsultaImpostos_Produtos(false, produto.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", produto.Preco.ToString().Replace(',', '.'));

                            decimal.TryParse(DATASET(ds, "nMVA"), out decimal MVA);
                            decimal.TryParse(DATASET(ds, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                            try
                            {
                                MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(ds, "sICMSST").Equals("S"), produto.sNCM, produto.sCEST, decimal.Parse(DATASET(ds, "nICMS")), DATASET(ds, "dtUltimaConsulta"), DATASET(ds, 1, 0, "sMensagem_Erro_LegisWeb"));

                                if (MVA > 0)
                                {
                                    var st = CalculaValor_ST(produto.NTotal, produto.nVlr_ICMS * produto.NQuantidade, MVA, ICMS_interno_destino);
                                    produto.nVlr_ST = st.Item1;
                                    produto.NST = st.Item2;
                                }
                            }
                            catch { }
                        }
                    }
                    else
                        throw new Exception("Produto não encontrado!");
                }
                catch (Exception ex)
                {
                    MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> Não foi possível Unificar o Produto!<br />Erro ao Unificar: " + ex.Message, true);
                }
            }

            gvProdutos_dataBind();
            AtualizaClasseGeral();
            MantemEtapa_Pos_PostBack(4);
        }

        /// <summary>
        /// Função recursiva utilizada para gerar um Novo e único idRegistro.
        /// </summary>
        /// <param name="list">Recebe a Lista de Itens de Destino, para garantir que o idRegistro é único.</param>
        /// <returns>Retorna um Novo idRegistro único na Lista.</returns>
        protected int GerarNovo_idRegistro(IEnumerable<cls_Comercial_Tabelas> list, int idInicial = 0)
        {
            int idRegistro = 1;

            if (idInicial > 0)
                idRegistro = idInicial;

            if (list.Any())
            {
                foreach (var p in list.OrderBy(p => p.idRegistro))
                {
                    if (p.idRegistro.Equals(idRegistro))
                        idRegistro++;
                }

                if (list.FirstOrDefault(p => p.idRegistro.Equals(idRegistro)) != null)
                    idRegistro = GerarNovo_idRegistro(list, idRegistro);
            }

            return idRegistro;
        }

        /// <summary>
        /// Método utilizado para Atualizar os Valores e Impostos do Produto recebido.
        /// </summary>
        /// <param name="produto">Recebe a instância do Produto a ser Atualizado.</param>
        protected void AtualizaValores_Produtos(cls_Comercial_Tabelas produto)
        {
            if (produto != null)
            {
                DataSet ds = ConsultaImpostos_Produtos(false, produto.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), ddlTabela.SelectedValue, "0", produto.bSistema ? "S" : "N");

                if (ValidarDataSet(ds))
                {
                    int.TryParse(DATASET(ds, "idRegra"), out int idRegra);
                    string ncm = DATASET(ds, "sCodigoNCM") == "0" ? "Não Cadastrado" : DATASET(ds, "sCodigoNCM");
                    string cest = DATASET(ds, "sCodigoCEST") == "0" ? "Não Cadastrado" : DATASET(ds, "sCodigoCEST");
                    decimal.TryParse(DATASET(ds, "nMVA"), out decimal MVA);
                    decimal.TryParse(DATASET(ds, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                    try
                    {
                        MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(ds, "sICMSST").Equals("S"), ncm, cest, decimal.Parse(DATASET(ds, "nICMS")), DATASET(ds, "dtUltimaConsulta"), DATASET(ds, 1, 0, "sMensagem_Erro_LegisWeb"));
                    }
                    catch { }

                    produto.idRegra = idRegra;

                    produto.sCFOP = DATASET(ds, "sCFOP");
                    produto.sCST = DATASET(ds, "CST");

                    produto.NIPI = Math.Round(decimal.Parse(DATASET(ds, "nIPI")), 2);
                    produto.NPIS = Math.Round(decimal.Parse(DATASET(ds, "nPIS")), 2);
                    produto.NCOFINS = Math.Round(decimal.Parse(DATASET(ds, "nCOFINS")), 2);
                    produto.NICMS = Math.Round(decimal.Parse(DATASET(ds, "nICMS")), 2);
                    produto.NDIFAL = Math.Round(decimal.Parse(DATASET(ds, "nDIFAL")), 2);
                    produto.NST = Math.Round(decimal.Parse(DATASET(ds, "nICMSST")), 2);

                    produto.bBaseCalcICMS_com_IPI = DATASET(ds, "sBaseCalculo").Equals("PI");

                    produto.nPesoBruto = Math.Round(decimal.Parse(DATASET(ds, "nPesoBruto")), 2);
                    produto.nPesoLiquido = Math.Round(decimal.Parse(DATASET(ds, "nPesoNeto")), 2);
                    produto.nVolume = Math.Round(decimal.Parse(DATASET(ds, "nVolume")), 2);

                    produto.Preco = Math.Round(decimal.Parse(DATASET(ds, "nTotal")), 2);
                    produto.nUnitario = Math.Round(produto.Preco - (produto.Preco * (produto.NFator / 100)), 2);
                    produto.NTotal = Math.Round(produto.nUnitario * produto.NQuantidade, 2);

                    if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                    {
                        produto.NII = 0;
                        produto.NIPI = 0;
                        produto.NPIS = 0;
                        produto.NCOFINS = 0;
                        produto.NICMS = 0;
                        produto.NDIFAL = 0;
                        produto.NST = 0;

                        produto.NTotal *= cambio;
                    }

                    try
                    {
                        RecalculaImpostos(produto);
                    }
                    catch { }

                    // Cálculo ST
                    if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !produto.bSistema && (ddlMoeda.SelectedValue == "0" || ddlMoeda.SelectedValue == "2"))
                    {
                        try
                        {
                            if (MVA > 0)
                            {
                                var st = CalculaValor_ST(produto.NTotal, produto.nVlr_ICMS * produto.NQuantidade, MVA, ICMS_interno_destino);
                                produto.nVlr_ST = st.Item1;
                                produto.NST = st.Item2;
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Função utilizada para Recalcular os Valores dos Impostos de um Item.
        /// </summary>
        /// <param name="item">Recebe a instância do Item onde serão recalculados os Impostos.</param>
        protected void RecalculaImpostos(cls_Comercial_Tabelas item)
        {
            item.nVlr_IPI = item.nUnitario - (item.nUnitario / (item.NIPI / 100 + 1));

            item.nBaseCalc_ICMS = item.bBaseCalcICMS_com_IPI ? item.nUnitario : item.nUnitario - item.nVlr_IPI;
            item.nBaseCalc_ICMS_Original = item.nBaseCalc_ICMS;

            item.nVlrReducao = item.nReducao > 0 ? item.nBaseCalc_ICMS * (item.nReducao / 100) : 0;
            item.nBaseCalc_ICMS -= item.nVlrReducao;

            item.nVlr_ICMS = item.nBaseCalc_ICMS * (item.NICMS / 100);

            item.nVlr_PIS = (item.nUnitario - item.nVlr_IPI - item.nVlr_ICMS) * (item.NPIS / 100);
            item.nVlr_COFINS = (item.nUnitario - item.nVlr_IPI - item.nVlr_ICMS) * (item.NCOFINS / 100);

            item.nVlr_Liquido = item.nUnitario - item.nVlr_IPI - item.nVlr_PIS - item.nVlr_COFINS - item.nVlr_ICMS;

            item.nVlr_DIFAL = item.nUnitario * (item.NDIFAL / 100);
        }

        protected void IncluirItem()
        {
            if (ComposicaoValidarDados())
            {
                AtualizaClasseGeral();

                DataSet ds = ConsultaImpostos_Produtos(false, FiltroPesquisaProdutos.IdItem, ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", Math.Round(FiltroPesquisaProdutos.hddnValorProduto, 2).ToString().Replace(',', '.'));

                string ncm = DATASET(ds, "sCodigoNCM") == "0" ? "Não Cadastrado" : DATASET(ds, "sCodigoNCM");
                string cest = DATASET(ds, "sCodigoCEST") == "0" ? "Não Cadastrado" : DATASET(ds, "sCodigoCEST");
                int.TryParse(DATASET(ds, "idRegra"), out int idRegra);
                decimal.TryParse(DATASET(ds, "nMVA"), out decimal MVA);
                decimal.TryParse(DATASET(ds, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                try
                {
                    MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(ds, "sICMSST").Equals("S"), ncm, cest, decimal.Parse(DATASET(ds, "nICMS")), DATASET(ds, "dtUltimaConsulta"), DATASET(ds, 1, 0, "sMensagem_Erro_LegisWeb"));
                }
                catch { }

                var novoProduto = new cls_Comercial_Tabelas
                {
                    idRegistro = GerarNovo_idRegistro(listProdutos),
                    SFuncao = "INCLUIR ITEM",
                    nOrdem = listProdutos.Count(p => p.bLiberado) == 0 ? 10 : listProdutos.Where(p => p.bLiberado).Max(p => p.nOrdem) + 10,
                    IdItem = int.Parse(FiltroPesquisaProdutos.IdItem),
                    SCodigo = FiltroPesquisaProdutos.SCodigo,
                    SDscProduto = FiltroPesquisaProdutos.SDscProduto,
                    sNCM = ncm,
                    sCEST = cest,
                    TipoProduto = DATASET(ds, "sDscTipoProduto"),
                    IdGrupoProduto = int.Parse(DATASET(ds, "idGrupo")),
                    IdFamiliaProduto = int.Parse(DATASET(ds, "idFamilia")),
                    sDscGrupoProduto = DATASET(ds, "sDscGrupo"),
                    sDscFamiliaProduto = DATASET(ds, "sDscFamilia"),
                    SUnidade = DATASET(ds, "sUnidade"),
                    sIndustrializado = DATASET(ds, "sProdutoIndustrializado") == "S" ? "Sim" : "Não",
                    sOrigem = DATASET(ds, "sProdutoImportado") == "S" ? "IMP" : "BR",

                    idRegra = idRegra,

                    sCFOP = DATASET(ds, "sCFOP"),
                    sCST = DATASET(ds, "CST"),

                    NIPI = Math.Round(decimal.Parse(DATASET(ds, "nIPI")), 2),
                    NPIS = Math.Round(decimal.Parse(DATASET(ds, "nPIS")), 2),
                    NCOFINS = Math.Round(decimal.Parse(DATASET(ds, "nCOFINS")), 2),
                    NICMS = Math.Round(decimal.Parse(DATASET(ds, "nICMS")), 2),
                    NDIFAL = Math.Round(decimal.Parse(DATASET(ds, "nDIFAL")), 2),
                    NST = Math.Round(decimal.Parse(DATASET(ds, "nICMSST")), 2),

                    bBaseCalcICMS_com_IPI = DATASET(ds, "nBaseCalculoICMS").Equals("PI"),

                    NQuantidade = FiltroPesquisaProdutos.NQuantidade,
                    nPesoBruto = Math.Round(decimal.Parse(DATASET(ds, "nPesoBruto")), 2),
                    nPesoLiquido = Math.Round(decimal.Parse(DATASET(ds, "nPesoNeto")), 2),
                    nVolume = Math.Round(decimal.Parse(DATASET(ds, "nVolume")), 2),
                    NFator = Math.Round(decimal.Parse(string.IsNullOrEmpty(txtDescontoProduto.Text) ? "0" : txtDescontoProduto.Text), 4),
                    Preco = Math.Round(FiltroPesquisaProdutos.hddnValorProduto, 2)
                };

                novoProduto.nUnitario = Math.Round(novoProduto.Preco - (novoProduto.Preco * (novoProduto.NFator / 100)), 2);
                novoProduto.NTotal = Math.Round(novoProduto.nUnitario * novoProduto.NQuantidade, 2);

                if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                {
                    novoProduto.NII = 0;
                    novoProduto.NIPI = 0;
                    novoProduto.NPIS = 0;
                    novoProduto.NCOFINS = 0;
                    novoProduto.NICMS = 0;
                    novoProduto.NDIFAL = 0;
                    novoProduto.NST = 0;

                    novoProduto.NTotal *= cambio;
                }

                try
                {
                    RecalculaImpostos(novoProduto);
                }
                catch { }

                // Cálculo ST
                if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !novoProduto.bSistema && (ddlMoeda.SelectedValue == "0" || ddlMoeda.SelectedValue == "2"))
                {
                    try
                    {
                        if (MVA > 0)
                        {
                            var st = CalculaValor_ST(novoProduto.NTotal, novoProduto.nVlr_ICMS * novoProduto.NQuantidade, MVA, ICMS_interno_destino);
                            novoProduto.nVlr_ST = st.Item1;
                            novoProduto.NST = st.Item2;
                        }
                    }
                    catch { }
                }

                novoProduto.bLiberado = true;
                novoProduto.dtInclusao = txtPrazoProduto.Text;

                listProdutos.Add(novoProduto);

                if (string.IsNullOrEmpty(DATASET(ds, "idRegra")) || DATASET(ds, "idRegra").Equals("0"))
                    MensagemPaginaProdutos.MostraMensagem_Aviso("<b>Aviso:</b> Não foi encontrada uma Regra Fiscal que esteja de acordo com as especificações deste Orçamento, por isso os Impostos dos Produtos não serão carregados!<br />É possível que a causa esteja inclusa no cadastro do Parceiro, no Tipo de Venda, no Endereço de Entrega ou na Empresa selecionada no Orçamento!", false);
            }
        }

        #endregion

        #region | Validar

        /// <summary>
        /// Função utilizada para Validar os dados do Produto para que o mesmo seja incluso no Orçamento.
        /// </summary>
        /// <returns><b>True:</b> Informações do Produto Válidas para Inclusão.<br/><b>False:</b> Informações do Produto Inválidas ou faltantes para Inclusão.</returns>
        protected bool ComposicaoValidarDados()
        {
            if (FiltroPesquisaProdutos.SDscProduto.Length <= 0)
            {
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> A Descrição do Produto deve ser preenchida!", false);
                return false;
            }
            if (FiltroPesquisaProdutos.SCodigo.Length <= 0)
            {
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> O Código do Produto deve estar preenchido!", false);
                return false;
            }
            if (FiltroPesquisaProdutos.NQuantidade <= 0)
            {
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> A Quantidade do Produto deve ser maior que Zero!", false);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Função utilizada para Validar os Dados do Orçamento para Salvá-lo.
        /// </summary>
        /// <returns><b>True:</b> Dados do Orçamento Válidos para Salvamento.<br /><b>False:</b> Dados do Orçamento Inválidos ou faltantes para Salvamentos.</returns>
        protected bool ValidarDados(string id)
        {
            bool retorno = true;

            divProgressoStories1.Attributes["class"] = divProgressoStories1.Attributes["class"].Replace("danger", "success");
            divProgressoStories2.Attributes["class"] = divProgressoStories2.Attributes["class"].Replace("danger", "success");
            divProgressoStories3.Attributes["class"] = divProgressoStories3.Attributes["class"].Replace("danger", "success");
            divProgressoStories4.Attributes["class"] = divProgressoStories4.Attributes["class"].Replace("danger", "success");

            cmdSelecionarParceiro.Attributes["class"] = cmdSelecionarParceiro.Attributes["class"].Replace(" erro", "");
            ddlEmpresa_Orcamento.Attributes["class"] = ddlEmpresa_Orcamento.Attributes["class"].Replace(" erro", "");
            ddlTipoOrcamento.Attributes["class"] = ddlTipoOrcamento.Attributes["class"].Replace(" erro", "");
            ddlFluxo.Attributes["class"] = ddlFluxo.Attributes["class"].Replace(" erro", "");
            ddlTabela.Attributes["class"] = ddlTabela.Attributes["class"].Replace(" erro", "");
            ddlEndereco.Attributes["class"] = ddlEndereco.Attributes["class"].Replace(" erro", "");
            ddlEndereco_Entrega.Attributes["class"] = ddlEndereco_Entrega.Attributes["class"].Replace(" erro", "");
            ddlContato.Attributes["class"] = ddlContato.Attributes["class"].Replace(" erro", "");
            txtReferencia.Attributes["class"] = txtReferencia.Attributes["class"].Replace(" erro", "");
            ddlDestinoVenda.Attributes["class"] = ddlDestinoVenda.Attributes["class"].Replace(" erro", "");
            txtValidade.Attributes["class"] = txtValidade.Attributes["class"].Replace(" erro", "");
            ddlCondicaoPagamento.Attributes["class"] = ddlCondicaoPagamento.Attributes["class"].Replace(" erro", "");
            ddlVendedor.Attributes["class"] = ddlVendedor.Attributes["class"].Replace(" erro", "");
            ddlFormaEnvio.Attributes["class"] = ddlFormaEnvio.Attributes["class"].Replace(" erro", "");
            txtEstimativaEntrega.Attributes["class"] = txtEstimativaEntrega.Attributes["class"].Replace(" erro", "");
            div_incluirProduto.Attributes["class"] = div_incluirProduto.Attributes["class"].Replace(" erro", "");
            div_gvProdutos.Attributes["class"] = div_gvProdutos.Attributes["class"].Replace(" erro", "");

            if (id == "0")
            {
                // ---------------------------------------------
                // Etapa 1 - Cadastro Inicial

                if (!hddProgresso.Value.Contains("Cliente"))
                {
                    MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Parceiro válido!", false);
                    div_cliente.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("Empresa"))
                {
                    ddlEmpresa_Orcamento.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("TipoOrcamento"))
                {
                    ddlTipoOrcamento.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("Fluxo"))
                {
                    ddlFluxo.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("TabelaPreco"))
                {
                    ddlTabela.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("Endereco_Fiscal"))
                {
                    ddlEndereco.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("Endereco_Entrega"))
                {
                    ddlEndereco_Entrega.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("Contato"))
                {
                    ddlContato.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("DestinoVenda"))
                {
                    ddlDestinoVenda.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }

                if (!retorno)
                {
                    divProgressoStories1.Attributes["class"] = divProgressoStories1.Attributes["class"].Replace("success", "danger");
                    painelCollapse1.Attributes["class"] = painelCollapse1.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");

                    return retorno;
                }

                // ---------------------------------------------
                // Etapa 2 - Informações do Orçamento

                if (!hddProgresso.Value.Contains("Referencia"))
                {
                    txtReferencia.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("Validade"))
                {
                    txtValidade.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("Pagamento"))
                {
                    ddlCondicaoPagamento.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("Vendedor"))
                {
                    ddlVendedor.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("Envio"))
                {
                    ddlFormaEnvio.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }
                if (!hddProgresso.Value.Contains("DataEntrega"))
                {
                    MensagemPaginaSegundaInfo.MostraMensagem_Erro("<b>Erro:</b> É necessário preencher o campo de Estimativa de Entrega com uma data válida!", false);
                    txtEstimativaEntrega.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }

                if (!retorno)
                {
                    divProgressoStories2.Attributes["class"] = divProgressoStories2.Attributes["class"].Replace("success", "danger");
                    painelCollapse2.Attributes["class"] = painelCollapse2.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");

                    return retorno;
                }

                // ---------------------------------------------
                // Etapas 3 e 4 - Serviços e Produtos

                if (!(listProdutos.Any(p => p.bLiberado) || listServicos_Recursos.Any(s => s.bLiberado)))
                {
                    MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> É necessário que o Orçamento possua ao menos 1 Produto <b>ou</b> 1 Serviço!", false);
                    divProgressoStories4.Attributes["class"] = divProgressoStories4.Attributes["class"].Replace("success", "danger");
                    painelCollapse4.Attributes["class"] = painelCollapse4.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");
                    div_incluirProduto.Attributes["class"] += " erro";
                    MantemEtapa_Pos_PostBack(4);
                    AtualizaBarraProgresso(4, false);
                    retorno = false;
                }
            }
            else
            {
                // ---------------------------------------------
                // Etapa 1 - Cadastro Inicial

                if (!(FiltroPesquisaParceiros.sCNPJ_CPF_Parceiro_Colaborador.Length > 0 && FiltroPesquisaParceiros.sDscParceiro_Colaborador.Length > 0))
                {
                    MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Parceiro válido!", false);
                    cmdSelecionarParceiro.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (ddlEmpresa_Orcamento.SelectedItem.Text == "Selecione a Empresa")
                {
                    ddlEmpresa_Orcamento.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (ddlTipoOrcamento.SelectedValue == "0")
                {
                    ddlTipoOrcamento.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (ddlFluxo.SelectedValue == "0")
                {
                    ddlFluxo.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (ddlTabela.SelectedValue == "0")
                {
                    ddlTabela.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (ddlEndereco.SelectedValue == "0")
                {
                    ddlEndereco.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (ddlEndereco_Entrega.SelectedValue == "0")
                {
                    ddlEndereco_Entrega.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (ddlContato.SelectedValue == "0")
                {
                    ddlContato.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }

                if (!retorno)
                {
                    divProgressoStories1.Attributes["class"] = divProgressoStories1.Attributes["class"].Replace("success", "danger");
                    painelCollapse1.Attributes["class"] = painelCollapse1.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");

                    return retorno;
                }

                // ---------------------------------------------
                // Etapa 2 - Informações do Orçamento

                if (txtReferencia.Text.Length < 3 || txtReferencia.Text.Length > 60)
                {
                    txtReferencia.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }
                if (ddlDestinoVenda.SelectedValue != "C" && ddlDestinoVenda.SelectedValue != "R" && ddlDestinoVenda.SelectedValue != "I")
                {
                    ddlDestinoVenda.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(1, false);
                    MantemEtapa_Pos_PostBack(1);
                    retorno = false;
                }
                if (string.IsNullOrEmpty(txtValidade.Text) || string.IsNullOrWhiteSpace(txtValidade.Text))
                {
                    txtValidade.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }
                if (ddlCondicaoPagamento.SelectedValue == "-1")
                {
                    ddlCondicaoPagamento.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }
                if (ddlVendedor.SelectedValue == "0")
                {
                    ddlVendedor.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }
                if (ddlFormaEnvio.SelectedValue == "0")
                {
                    ddlFormaEnvio.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }
                if (ValidaDatas(DateTime.Today.ToString(), txtEstimativaEntrega.Text, true).Length > 0)
                {
                    MensagemPaginaSegundaInfo.MostraMensagem_Erro("<b>Erro:</b> É necessário preencher o campo de Estimativa de Entrega com uma data válida!", false);
                    txtEstimativaEntrega.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }
                if (div_NovaCondPgto_Motivo.Visible && Convert.ToBoolean(hddCondPgto_Alterada.Value) && string.IsNullOrEmpty(txtNovaCondPgto_Motivo.Text))
                {
                    MensagemPaginaSegundaInfo.MostraMensagem_Erro("<b>Erro:</b> Ao cadastrar uma Nova Condição de Pagamento utilizando a função (<i class=\"fa fa-refresh\"></i>) de Nova Condição de Pagamento, passa a ser obrigatória a inclusão de um Motivo!", false);
                    txtNovaCondPgto_Motivo.Attributes["class"] += " erro";
                    AtualizaBarraProgresso(2, false);
                    MantemEtapa_Pos_PostBack(2);
                    retorno = false;
                }

                if (!retorno)
                {
                    divProgressoStories2.Attributes["class"] = divProgressoStories2.Attributes["class"].Replace("success", "danger");
                    painelCollapse2.Attributes["class"] = painelCollapse2.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");

                    return retorno;
                }

                // ---------------------------------------------
                // Etapas 3 e 4 - Serviços e Produtos

                if (!(listProdutos.Any(p => p.bLiberado) || listServicos_Recursos.Any(s => s.bLiberado)))
                {
                    MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> É necessário que o Orçamento possua ao menos 1 Produto <b>ou</b> 1 Serviço!", false);
                    divProgressoStories4.Attributes["class"] = divProgressoStories4.Attributes["class"].Replace("success", "danger");
                    painelCollapse4.Attributes["class"] = painelCollapse4.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");
                    div_incluirProduto.Attributes["class"] += " erro";
                    MantemEtapa_Pos_PostBack(4);
                    AtualizaBarraProgresso(4, false);
                    retorno = false;
                }
            }

            if (listProdutos.Any(p => p.bLiberado && p.IdItem.Equals(0)))
            {
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> Não é possível Salvar o Orçamento possuindo Produtos não cadastrados!", false);
                divProgressoStories4.Attributes["class"] = divProgressoStories4.Attributes["class"].Replace("success", "danger");
                painelCollapse4.Attributes["class"] = painelCollapse4.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");
                div_incluirProduto.Attributes["class"] += " erro";
                MantemEtapa_Pos_PostBack(4);
                AtualizaBarraProgresso(4, false);
                retorno = false;
            }

            if (listProdutos.Any(p => p.bLiberado && (p.Preco <= decimal.Zero || p.NQuantidade <= decimal.Zero || p.NTotal <= decimal.Zero)))
            {
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> Não é possível Salvar o Orçamento possuindo Produtos com Valor, Quantidade ou Total igual a 0 (Zero)!", false);
                divProgressoStories4.Attributes["class"] = divProgressoStories4.Attributes["class"].Replace("success", "danger");
                painelCollapse4.Attributes["class"] = painelCollapse4.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");
                div_incluirProduto.Attributes["class"] += " erro";
                MantemEtapa_Pos_PostBack(4);
                AtualizaBarraProgresso(4, false);
                retorno = false;
            }

            if (div_Tabela_Obs.Visible && txtTabelaObs.Text.Length < 3)
            {
                MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Ao alterar a Tabela de Preços, é necessário adicionar uma Observação com o Motivo da Alteração, com ao menos 3 caracteres!", false);
                divProgressoStories1.Attributes["class"] = divProgressoStories1.Attributes["class"].Replace("success", "danger");
                painelCollapse1.Attributes["class"] = painelCollapse1.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");
                txtTabelaObs.Attributes["class"] += " erro";
                AtualizaBarraProgresso(1, false);
                MantemEtapa_Pos_PostBack(1);
                retorno = false;
            }

            if (string.IsNullOrEmpty(txtControle_TT.Text))
            {
                MensagemPaginaSegundaInfo.MostraMensagem_Erro("<b>Erro:</b> É necessário preencher o número de Controle TT!", false);
                divProgressoStories2.Attributes["class"] = divProgressoStories2.Attributes["class"].Replace("success", "danger");
                painelCollapse2.Attributes["class"] = painelCollapse2.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");
                txtControle_TT.Attributes["class"] += " erro";
                AtualizaBarraProgresso(2, false);
                MantemEtapa_Pos_PostBack(2);
                retorno = false;
            }

            if (div_Moeda.Visible && ddlMoeda.SelectedValue == "0")
            {
                MensagemPaginaSegundaInfo.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar uma Moeda!", false);
                divProgressoStories1.Attributes["class"] = divProgressoStories1.Attributes["class"].Replace("success", "danger");
                painelCollapse1.Attributes["class"] = painelCollapse1.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");
                ddlMoeda.Classe = " erro";
                AtualizaBarraProgresso(1, false);
                MantemEtapa_Pos_PostBack(1);
                retorno = false;
            }

            return retorno;
        }

        /// <summary>
        /// Método utilizado para Validar se existe Regra Fiscal definida nos Produtos presentes no Orçamento.
        /// <br />Em caso de não possuir será exibida uma mensagem Fixa na Aba de Produts e de Documentos da Etapa de Visualização Geral e Confirmação.
        /// </summary>
        /// <returns>Retorna uma mensagem de Aviso em string.</returns>
        protected string ValidaRegraFiscal_hddMsg()
        {
            string msg = "";
            int nTD = 1;

            foreach (cls_Comercial_Tabelas item in listProdutos.Where(p => p.bLiberado).OrderBy(p => p.nOrdem))
            {
                if (item.idRegra <= 0)
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg += "<b>Itens sem Regra Fiscal </b><a class=\"exibeItens_msg\" data-toggle=\"tooltip\" title=\"Exibir Itens\"><i class=\"fa fa-chevron-down\"></i></a>";

                        msg += $@"  <div class=""div_tableRegraFiscal_msg id"">
                                        <table class=""faltaRegraFiscal_msg"">
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <ul>
                                                            <li>
                                                                Item {item.nOrdem}: {item.SCodigo} - {item.SDscProduto}
                                                            </li>
                                                        </ul>
                                                    </td>";
                    }
                    else if (nTD == 1)
                    {
                        msg += $@"      <tr>
                                            <td>
                                                <ul>
                                                    <li>
                                                        Item {item.nOrdem}: {item.SCodigo} - {item.SDscProduto}
                                                    </li>
                                                </ul>
                                            </td>";
                    }
                    else if (nTD == 3)
                    {
                        msg += $@"          <td>
                                                <ul>
                                                    <li>
                                                        Item {item.nOrdem}: {item.SCodigo} - {item.SDscProduto}
                                                    </li>
                                                </ul>
                                            </td>
                                        </tr>";

                        nTD = 0;
                    }
                    else
                    {
                        msg += $@"           <td>
                                                <ul>
                                                    <li>
                                                        Item {item.nOrdem}: {item.SCodigo} - {item.SDscProduto}
                                                    </li>
                                                </ul>
                                            </td>";
                    }

                    nTD++;
                }
            }

            if (!string.IsNullOrEmpty(msg))
            {
                msg += msg.EndsWith("</tr>") ? @"         </tbody>
                                                        </table>
                                                    </div>"
                                                : @"            </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>";

                if (!listProdutos.Any(p => p.idRegra > 0)) msg = "<b>Aviso:</b> Não foi encontrada Regra Fiscal para o Orçamento, desta forma nenhum dos Produtos cadastrados possuirão seus Impostos preenchidos!";
                else if (Request["id"] == "0") msg = $"<b>Aviso:</b> Não foi encontrada Regra Fiscal para os seguintes Produtos: <br /><br />{msg}<br /><br />Pela falta de uma Regra Fiscal, estes Produtos não possuirão seus Impostos preenchidos!";
                else msg = $"<b>Aviso:</b> Não foi encontrada Regra Fiscal para os seguintes Produtos: <br /><br />{msg}<br /><br />Pela falta de uma Regra Fiscal, estes Produtos não possuirão seus Impostos preenchidos!";
            }

            return msg;
        }

        #endregion

        #region | Modal

        /// <summary>
        /// Método utilizado para Abrir o Modal utilizado para Vincular o Orçamento à um CRM.
        /// </summary>
        protected void AbrirModal_VinculaCRM()
        {
            Scripts.AbrirModal(Page, "modalVincularCRM");
            MantemEtapa_Pos_PostBack(5);
            pn5.Attributes["class"] = "painel";
            div_Paineis.Attributes["style"] = "display: flex; height: 80%; width: 100%; padding: 0 30px 0 30px;";
        }

        /// <summary>
        /// Método utilizado para Abrir o Modal para Vincular o Orçamento com um Pedido existente.
        /// </summary>
        protected void AbrirModal_VinculaPedido(bool bNovo)
        {
            div_ddlVinculaPedido.Visible = !bNovo;
            cmdConfirmar_Vinculo_Pedido.Visible = !bNovo;

            div_txtPrevisaoEntrega.Visible = bNovo;
            cmdVincular_Novo_Pedido.Visible = bNovo;

            if (bNovo)
            {
                modalVincularPedido_titulo.InnerText = "Novo Pedido";
                DateTime.TryParse(txtEstimativa_View.Text, out DateTime dt);
                txtPrevisaoEntrega.Text = dt.ToString("yyyy-MM-dd");
            }
            else
                modalVincularPedido_titulo.InnerText = "Vincular à um Pedido";

            Scripts.AbrirModal(Page, "modalVincularPedido");
            MantemEtapa_Pos_PostBack(5);
            pn5.Attributes["class"] = "painel";
            div_Paineis.Attributes["style"] = "display: flex; height: 80%; width: 100%; padding: 0 30px 0 30px;";
        }

        /// <summary>
        /// Método utilizado para Abrir o Modal utilizado para criação de um Novo Endereço de Entrega ao Parceiro selecionado.
        /// </summary>
        protected void AbrirModal_NovoEndereco()
        {
            Scripts.RemoverBackdrop_Modal(Page);
            Scripts.AbrirModal(Page, "modal_NovoEndereco");
            AtualizaBarraProgresso(1, false);
            MantemEtapa_Pos_PostBack(1);
        }

        /// <summary>
        /// Função utilizada para Validar os Dados para Criar um Novo Endereço.
        /// </summary>
        /// <returns><b>True:</b> Dados do Novo Endereço Válidos para Criação.<br /><b>False:</b> Dados do Novo Endereço Inválidos ou faltantes para Criação.</returns>
        protected bool ValidaNovoEndereco()
        {
            if (txtCEP_NovoEndereco.Text.Trim().Length != 9)
            {
                MensagemPagina_Modal_NovoEndereco.MostraMensagem_Erro("<b>Erro:</b> O campo de CEP é obrogatório e deve seguir o padrão <b>XXXXX-XXX</b>!", false);
                return false;
            }
            if (txtLogradouro_NovoEndereco.Text.Length < 5)
            {
                MensagemPagina_Modal_NovoEndereco.MostraMensagem_Erro("<b>Erro:</b> É necessário preencher o Logradouro do Novo Endereço com ao menos 5 caracteres!", false);
                return false;
            }
            if (txtNumero_NovoEndereco.Text.Length < 1)
            {
                MensagemPagina_Modal_NovoEndereco.MostraMensagem_Erro("<b>Erro:</b> É necessário preencher o Número do Novo Endereço!", false);
                return false;
            }
            if (txtPais_NovoEndereco.Text.Length < 3)
            {
                MensagemPagina_Modal_NovoEndereco.MostraMensagem_Erro("<b>Erro:</b> É necessário preencher o País do Novo Endereço com ao menos 3 caracteres!", false);
                return false;
            }
            if (ddlEstado_NovoEndereco.SelectedValue == "0")
            {
                MensagemPagina_Modal_NovoEndereco.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Estado para o Novo Endereço!", false);
                return false;
            }
            if (ddlCidade_NovoEndereco.SelectedValue == "0")
            {
                MensagemPagina_Modal_NovoEndereco.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar uma Cidade para o Novo Endereço!", false);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Método utilizado para limpar os campos preenchidos e selecionados no Modal de Criação de um Novo Endereço.
        /// </summary>
        protected void LimparCampos_NovoEndereco()
        {
            txtCEP_NovoEndereco.Text = string.Empty;
            txtLogradouro_NovoEndereco.Text = string.Empty;
            txtNumero_NovoEndereco.Text = string.Empty;
            txtComplemento_NovoEndereco.Text = string.Empty;
            txtPais_NovoEndereco.Text = string.Empty;
            txtBairro_NovoEndereco.Text = string.Empty;
            ddlEstado_NovoEndereco.SelectedValue = "0";
            ddlCidade_NovoEndereco.SelectedValue = "0";

            div_Cidade_NovoEndereco.Visible = false;
            ddlCidade_NovoEndereco.Items.Clear();
        }

        #endregion

        #region | Outros

        /// <summary>
        /// Método utilizado para Atualizar as Barras de Progresso.
        /// </summary>
        protected void AtualizaBarraProgresso(int etapa, bool bLimpa)
        {
            try
            {
                // -------------------------------------------------------------------------------------------------------------
                // Atualização dos campos que não possuem Eventos próprios na Barra de Progresso

                try
                {
                    if (bLimpa)
                    {
                        divProgressoStories1.Attributes["class"] = divProgressoStories1.Attributes["class"].Replace("danger", "success");
                        divProgressoStories2.Attributes["class"] = divProgressoStories2.Attributes["class"].Replace("danger", "success");
                        divProgressoStories3.Attributes["class"] = divProgressoStories3.Attributes["class"].Replace("danger", "success");
                        divProgressoStories4.Attributes["class"] = divProgressoStories4.Attributes["class"].Replace("danger", "success");

                        div_cliente.Attributes["class"] = div_cliente.Attributes["class"].Replace(" erro", "");
                        ddlEmpresa_Orcamento.Attributes["class"] = ddlEmpresa_Orcamento.Attributes["class"].Replace(" erro", "");
                        ddlTipoOrcamento.Attributes["class"] = ddlTipoOrcamento.Attributes["class"].Replace(" erro", "");
                        ddlFluxo.Attributes["class"] = ddlFluxo.Attributes["class"].Replace(" erro", "");
                        ddlTabela.Attributes["class"] = ddlTabela.Attributes["class"].Replace(" erro", "");
                        ddlEndereco.Attributes["class"] = ddlEndereco.Attributes["class"].Replace(" erro", "");
                        ddlEndereco_Entrega.Attributes["class"] = ddlEndereco_Entrega.Attributes["class"].Replace(" erro", "");
                        ddlContato.Attributes["class"] = ddlContato.Attributes["class"].Replace(" erro", "");
                        txtReferencia.Attributes["class"] = txtReferencia.Attributes["class"].Replace(" erro", "");
                        ddlDestinoVenda.Attributes["class"] = ddlDestinoVenda.Attributes["class"].Replace(" erro", "");
                        txtValidade.Attributes["class"] = txtValidade.Attributes["class"].Replace(" erro", "");
                        ddlCondicaoPagamento.Attributes["class"] = ddlCondicaoPagamento.Attributes["class"].Replace(" erro", "");
                        ddlVendedor.Attributes["class"] = ddlVendedor.Attributes["class"].Replace(" erro", "");
                        ddlFormaEnvio.Attributes["class"] = ddlFormaEnvio.Attributes["class"].Replace(" erro", "");
                        txtEstimativaEntrega.Attributes["class"] = txtEstimativaEntrega.Attributes["class"].Replace(" erro", "");
                        div_incluirProduto.Attributes["class"] = div_incluirProduto.Attributes["class"].Replace(" erro", "");
                        div_gvProdutos.Attributes["class"] = div_gvProdutos.Attributes["class"].Replace(" erro", "");
                    }

                    string mensagem = string.Empty;

                    // Campo de Contato
                    if (!string.IsNullOrEmpty(ddlContato.SelectedValue) && ddlContato.SelectedValue != "0")
                    {
                        if (!hddProgresso.Value.Contains("Contato"))
                            hddProgresso.Value += "Contato|";

                        ddlContato.Attributes["class"] = ddlContato.Attributes["class"].Replace(" erro", "");
                    }
                    else
                    {
                        if (hddProgresso.Value.Contains("Contato"))
                            hddProgresso.Value = hddProgresso.Value.Replace("Contato|", "");

                        if (etapa.Equals(1))
                        {
                            ddlContato.Attributes["class"] += " erro";
                            mensagem += "É necessário selecionar o Contato do Cliente!<br />";
                        }
                    }
                    hddidContato.Value = string.IsNullOrEmpty(ddlContato.SelectedValue) || ddlContato.SelectedValue == "0" ? hddidContato.Value : ddlContato.SelectedValue;
                    txtContato_View.Text = string.IsNullOrEmpty(ddlContato.SelectedValue) || ddlContato.SelectedValue == "0" ? txtContato_View.Text : ddlContato.SelectedItem.Text;

                    // Campo de Tipo de Cliente
                    hddidTipoCliente.Value = string.IsNullOrEmpty(ddlTipoCliente.SelectedValue) || ddlTipoCliente.SelectedValue == "0" ? hddidTipoCliente.Value : ddlTipoCliente.SelectedValue;
                    txtTipoCliente_View.Text = string.IsNullOrEmpty(ddlTipoCliente.SelectedValue) || ddlTipoCliente.SelectedValue == "0" ? txtTipoCliente_View.Text : ddlTipoCliente.SelectedItem.Text;

                    // Campo de Observação de Tabela de Preços
                    txtTabela_Obs_View.Text = string.IsNullOrEmpty(txtTabelaObs.Text) ? txtTabela_Obs_View.Text : txtTabelaObs.Text;

                    // Campo de N° Controle TT
                    txtControle_TT_View.Text = string.IsNullOrEmpty(txtControle_TT.Text) ? txtControle_TT_View.Text : txtControle_TT.Text;

                    // Campo de Referência
                    if (txtReferencia.Text.Length >= 3 && txtReferencia.Text.Length <= 60)
                    {
                        if (!hddProgresso.Value.Contains("Referencia"))
                            hddProgresso.Value += "Referencia|";

                        txtReferencia.Attributes["class"] = txtReferencia.Attributes["class"].Replace(" erro", "");
                    }
                    else
                    {
                        if (hddProgresso.Value.Contains("Referencia"))
                            hddProgresso.Value = hddProgresso.Value.Replace("Referencia|", "");

                        if (etapa.Equals(2))
                        {
                            txtReferencia.Attributes["class"] += " erro";
                            mensagem += "A Referência do Orçamento é obrigatória, deve possuir pelo menos 3 caracteres e não deve ultrapassar 60 caracteres!<br />";
                        }
                    }

                    txtReferencia_View.Text = string.IsNullOrEmpty(txtReferencia.Text) ? txtReferencia_View.Text : txtReferencia.Text;

                    // Campo de IE
                    if (txtIE_View.Text != txtIE.Text)
                        hddAtualiza_IE_Parceiro.Value = "1";

                    txtIE_View.Text = txtIE.Text;

                    // Campos de Moeda
                    txtMoeda_View.Text = ddlMoeda.SelectedValue == "0" ? txtMoeda_View.Text : ddlMoeda.SelectedItem.Text;
                    txtCambio_View.Text = string.IsNullOrEmpty(txtCambio.Text) ? txtCambio_View.Text : txtCambio.Text;

                    // Campo de Condição de Pagamento
                    if (!string.IsNullOrEmpty(ddlCondicaoPagamento.SelectedValue) && ddlCondicaoPagamento.SelectedValue != "-1")
                    {
                        if (!hddProgresso.Value.Contains("Pagamento"))
                            hddProgresso.Value += "Pagamento|";

                        ddlCondicaoPagamento.Attributes["class"] = ddlCondicaoPagamento.Attributes["class"].Replace(" erro", "");
                    }
                    else
                    {
                        if (hddProgresso.Value.Contains("Pagamento"))
                            hddProgresso.Value = hddProgresso.Value.Replace("Pagamento|", "");

                        if (etapa.Equals(2))
                        {
                            ddlCondicaoPagamento.Attributes["class"] += " erro";
                            mensagem += "É necessário selecionar uma Condição de Pagamento!<br />";
                        }
                    }

                    // Campo de Motivo da Alteração da Cond. de Pgto.
                    txtNovaCondPgto_Motivo_View.Text = string.IsNullOrEmpty(txtNovaCondPgto_Motivo.Text) ? txtNovaCondPgto_Motivo_View.Text : txtNovaCondPgto_Motivo.Text;

                    // Campo de Forma de Envio
                    if (!string.IsNullOrEmpty(ddlFormaEnvio.SelectedValue) && ddlFormaEnvio.SelectedValue != "0")
                    {
                        if (!hddProgresso.Value.Contains("Envio"))
                            hddProgresso.Value += "Envio|";

                        ddlFormaEnvio.Attributes["class"] = ddlFormaEnvio.Attributes["class"].Replace(" erro", "");
                    }
                    else
                    {
                        if (hddProgresso.Value.Contains("Envio"))
                            hddProgresso.Value = hddProgresso.Value.Replace("Envio|", "");

                        if (etapa.Equals(2))
                        {
                            ddlFormaEnvio.Attributes["class"] += " erro";
                            mensagem += "É necessário selecionar uma Forma de Envio!<br />";
                        }
                    }

                    // Campo de Emresa de Transporte
                    txtTransporte_View.Text = string.IsNullOrEmpty(txtTransporte.Text) ? txtTransporte_View.Text : txtTransporte.Text;

                    // Campo de Previsao de Entrega
                    if (string.IsNullOrEmpty(ValidaDatas(DateTime.Today.ToString(), txtEstimativaEntrega.Text, true)))
                    {
                        if (!hddProgresso.Value.Contains("DataEntrega"))
                            hddProgresso.Value += "DataEntrega|";

                        txtEstimativaEntrega.Attributes["class"] = txtEstimativaEntrega.Attributes["class"].Replace(" erro", "");
                    }
                    else
                    {
                        if (hddProgresso.Value.Contains("DataEntrega"))
                            hddProgresso.Value = hddProgresso.Value.Replace("DataEntrega|", "");

                        if (etapa.Equals(2))
                        {
                            txtEstimativaEntrega.Attributes["class"] += " erro";
                            mensagem += "É necessário preencher a Estimativa de Entrega!<br />";
                        }
                    }
                    DateTime.TryParse(txtEstimativaEntrega.Text, out DateTime dtPrevisaoEntrega);
                    txtEstimativa_View.Text = string.IsNullOrEmpty(txtEstimativaEntrega.Text) ? txtEstimativa_View.Text : dtPrevisaoEntrega.ToString("dd/MM/yyyy");

                    // Campo de Dias de Execução
                    txtDiasPrevisao_View.Text = string.IsNullOrEmpty(txtDiasPrevisao.Text) ? txtDiasPrevisao_View.Text : txtDiasPrevisao.Text;

                    // Campo de Validade do Orçamento
                    if (string.IsNullOrEmpty(txtValidade.Text))
                    {
                        if (hddProgresso.Value.Contains("Validade")) hddProgresso.Value = hddProgresso.Value.Replace("Validade|", "");

                        if (etapa.Equals(2))
                        {
                            txtValidade.Attributes["class"] += " erro";
                            mensagem += "É necessário preencher a Validade do Orçamento!<br />";
                        }
                    }
                    else
                    {
                        if (!hddProgresso.Value.Contains("Validade"))
                            hddProgresso.Value += "Validade|";

                        txtValidade.Attributes["class"] = txtValidade.Attributes["class"].Replace(" erro", "");
                    }

                    txtValidade_View.Text = string.IsNullOrEmpty(txtValidade.Text) ? txtValidade_View.Text : txtValidade.Text;

                    // Campo de Frete
                    txtFrete_View.Text = string.IsNullOrEmpty(txtFrete.Text) ? txtFrete_View.Text : txtFrete.Text;

                    // Campo de Instalador
                    txtInstalador_View.Text = ddlidInstalador.SelectedValue == "0" ? txtInstalador_View.Text : ddlidInstalador.SelectedItem.Text;

                    // Campos de Custo
                    txtCusto_Aduaneiro_View.Text = string.IsNullOrEmpty(txtCusto_Aduaneiro.Text) ? txtCusto_Aduaneiro_View.Text : txtCusto_Aduaneiro.Text;
                    txtCusto_Despachante_View.Text = string.IsNullOrEmpty(txtCusto_Despachante.Text) ? txtCusto_Despachante_View.Text : txtCusto_Despachante.Text;

                    // Campo de Observação
                    txtObs_View.Text = string.IsNullOrEmpty(txtObservacao.Text) ? txtObs_View.Text : txtObservacao.Text;

                    if ((etapa.Equals(1) || etapa.Equals(2)) && !string.IsNullOrEmpty(mensagem))
                    {
                        mensagem = mensagem.Remove(mensagem.Length - 6);

                        if (etapa.Equals(1))
                        {
                            MensagemPaginaInfoInicial.MostraMensagem_Erro(string.Format("<b>Erro:</b> {0}", mensagem), false);

                            divProgressoStories1.Attributes["class"] = divProgressoStories1.Attributes["class"].Replace("success", "danger");
                            painelCollapse1.Attributes["class"] = painelCollapse1.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");
                        }
                        else if (etapa.Equals(2))
                        {
                            MensagemPaginaSegundaInfo.MostraMensagem_Erro(string.Format("<b>Erro:</b> {0}", mensagem), false);

                            divProgressoStories2.Attributes["class"] = divProgressoStories2.Attributes["class"].Replace("success", "danger");
                            painelCollapse2.Attributes["class"] = painelCollapse2.Attributes["class"].Replace("cmdStories_focus", "cmdStories_error");
                        }
                    }
                    else
                    {
                        divProgressoStories1.Attributes["class"] = divProgressoStories1.Attributes["class"].Replace("danger", "success");
                        divProgressoStories2.Attributes["class"] = divProgressoStories2.Attributes["class"].Replace("danger", "success");
                    }

                    try
                    {
                        txtTabelaPreco_ImportarProdutos.Text = !string.IsNullOrEmpty(ddlTabela.SelectedValue) && ddlTabela.SelectedValue != "0" ? string.Format("ID: {0} - Tabela: {1}, Tipo: Vendas Customizadas", ddlTabela.SelectedValue, ddlTabela.SelectedItem.Text) : txtTabela_View.Text;

                        Popula_Combo(ddlPedidos_ImportarProdutos, "sp_Select 'IMPORTACAO_ITENS_ORCAMENTO'", "idPedido", "sDscPedido", false, "Selecione um Pedido para Importar os Itens", "0");
                        Popula_Combo(ddlSelecionaPedido_LM_ImportarProdutos, "sp_Select 'IMPORTACAO_ITENS_ORCAMENTO', @idFiltro=1", "idPedido", "sDscPedido", false, "Selecione um Pedido que possui LM para Importar os Itens", "0");
                        Popula_Combo(ddlOrcamento_ImportarProdutos, "sp_Select 'IMPORTACAO_ITENS_ORCAMENTO', @idFiltro=3", "idPedido", "sDscPedido", false, "Selecione um Orçamento para Importar os Itens", "0");

                        // Remove este Orçamento da Lista de Orçamentos e Remove o Pedido Vinculado das Listas de Pedidos
                        if (!string.IsNullOrEmpty(Request["id"]) && Request["id"] != "0")
                        {
                            ddlOrcamento_ImportarProdutos.Items.Remove(ddlOrcamento_ImportarProdutos.Items.FindByValue(Request["id"].TrimStart('0')));

                            if (!hddPedidoVinculado.Value.Equals("0"))
                            {
                                ddlPedidos_ImportarProdutos.Items.Remove(ddlPedidos_ImportarProdutos.Items.FindByValue(hddPedidoVinculado.Value));
                                ddlSelecionaPedido_LM_ImportarProdutos.Items.Remove(ddlSelecionaPedido_LM_ImportarProdutos.Items.FindByValue(hddPedidoVinculado.Value));
                            }
                        }
                    }
                    catch { }

                    UpdModal_Importar_Pedidos.Update();
                    UpdModal_Importar_LM.Update();
                    UpdModal_Importar_Orcamento.Update();
                    UpdModal_Importar_TabelaPreco.Update();

                    try
                    {
                        txtVincularProduto.Text = hddVincularProduto.Value;

                        Popula_Combo(ddlVincularProduto, $"sp_Select 'Flow_Produtos_Vincular', @idPesquisa={hddidCliente.Value}", "idItem", "sDscProduto", false, "Selecione um Produto para Vincular", "0");
                        Popula_Combo(ddlVincularTodosProdutos, $"sp_Select 'Flow_Produtos_Vincular', @idPesquisa={hddidCliente.Value}", "idItem", "sDscProduto", false, "Selecione um Produto para Vincular", "0");
                    }
                    catch { }
                }
                catch { }

                // -------------------------------------------------------------------------------------------------------------

                AtualizaClasseGeral();

                int porcentagemAtual = 0;
                int nCampos = 18;
                int valorCampo = 100 / nCampos;

                int porcentagemAtualStories1 = 0;
                int nCamposStories1 = 9;
                int valorCampoStories1 = 100 / nCamposStories1;

                int porcentagemAtualStories2 = 0;
                int nCamposStories2 = 6;
                int valorCampoStories2 = 100 / nCamposStories2;

                int porcentagemAtualStories3 = 0;
                int nCamposStories3 = 2;
                int valorCampoStories3 = 100 / nCamposStories3;

                int porcentagemAtualStories4 = 0;
                int nCamposStories4 = 1;
                int valorCampoStories4 = 100 / nCamposStories4;

                // -------------------------------------------------------------------------------------------------------------
                // Etapa 1 - Cadastro Inicial

                if (hddProgresso.Value.Contains("Cliente"))
                {
                    porcentagemAtual += valorCampo + 10;
                    porcentagemAtualStories1 += valorCampoStories1 + 1;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panel_InfoInicial_View.Visible = true;

                    txtCNPJ_View.Text = FiltroPesquisaParceiros.sCNPJ_CPF_Parceiro_Colaborador.Length > 0 ? FiltroPesquisaParceiros.sCNPJ_CPF_Parceiro_Colaborador : txtCNPJ_View.Text;
                    txtRazaoSocial_View.Text = FiltroPesquisaParceiros.sDscParceiro_Colaborador.Length > 0 ? FiltroPesquisaParceiros.sDscParceiro_Colaborador : txtRazaoSocial_View.Text;

                    txtTabela_View.Text = ddlTabela.Items != null && ddlTabela.Items.Count > 0 ? ddlTabela.SelectedItem.Text : txtTabela_View.Text;
                    txtConfidencial_View.Text = ddlConfidencial.SelectedItem.Text;
                }

                if (hddProgresso.Value.Contains("Endereco_Fiscal"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories1 += valorCampoStories1;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panel_InfoInicial_View.Visible = true;

                    txtEndereco_View.Text = ddlEndereco.Items != null && ddlEndereco.Items.Count > 0 ? ddlEndereco.SelectedItem.Text : txtEndereco_View.Text;
                }

                if (hddProgresso.Value.Contains("Endereco_Entrega"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories1 += valorCampoStories1;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panel_InfoInicial_View.Visible = true;

                    txtEnderecoEntrega_View.Text = ddlEndereco_Entrega.Items != null && ddlEndereco_Entrega.Items.Count > 0 ? ddlEndereco_Entrega.SelectedItem.Text : txtEnderecoEntrega_View.Text;
                }

                if (hddProgresso.Value.Contains("Contato"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories1 += valorCampoStories1;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panel_InfoInicial_View.Visible = true;

                    txtContato_View.Text = ddlContato.Items != null && ddlContato.Items.Count > 0 ? ddlContato.SelectedItem.Text : txtContato_View.Text;
                }

                if (hddProgresso.Value.Contains("DestinoVenda"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories1 += valorCampoStories1;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panelFinalizacao_View.Visible = true;

                    txtDestinoVenda_View.Text = ddlDestinoVenda.Items != null && ddlDestinoVenda.Items.Count > 0 && ddlDestinoVenda.SelectedValue != "-1" ? ddlDestinoVenda.SelectedItem.Text : txtDestinoVenda_View.Text;

                    txtObs_View.Text = txtObservacao.Text.Length > 0 ? txtObservacao.Text : txtObs_View.Text;
                }

                if (hddProgresso.Value.Contains("TabelaPreco"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories1 += valorCampoStories1;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panel_InfoInicial_View.Visible = true;

                    txtTabela_View.Text = ddlTabela.Items != null && ddlTabela.Items.Count > 0 ? ddlTabela.SelectedItem.Text : txtTabela_View.Text;
                }

                if (hddProgresso.Value.Contains("TipoOrcamento"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories1 += valorCampoStories1;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panel_InfoInicial_View.Visible = true;

                    txtTipoOrcamento_View.Text = ddlTipoOrcamento.Items != null && ddlTipoOrcamento.Items.Count > 0 ? ddlTipoOrcamento.SelectedItem.Text : txtTipoOrcamento_View.Text;
                }

                if (hddProgresso.Value.Contains("Fluxo"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories1 += valorCampoStories1;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panel_InfoInicial_View.Visible = true;

                    try
                    {
                        txtFluxo_View.Text = ddlFluxo.SelectedItem.Text;
                    }
                    catch { }
                }

                if (hddProgresso.Value.Contains("Empresa"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories1 += valorCampoStories1;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panel_InfoInicial_View.Visible = true;

                    txtEmpresa_View.Text = ddlEmpresa_Orcamento.Items != null && ddlEmpresa_Orcamento.Items.Count > 0 ? ddlEmpresa_Orcamento.SelectedItem.Text : txtEmpresa_View.Text;
                }

                // -------------------------------------------------------------------------------------------------------------
                // Etapa 2 - Informações do Orçamento

                if (hddProgresso.Value.Contains("Referencia"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories2 += valorCampoStories2 + 4;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panelFinalizacao_View.Visible = true;

                    txtReferencia_View.Text = !string.IsNullOrEmpty(txtReferencia.Text) && txtReferencia.Text.Length > 3 ? txtReferencia.Text : txtReferencia_View.Text;
                    txtreferencia_id.Text = txtReferencia_View.Text;

                    txtObs_View.Text = txtObservacao.Text.Length > 0 ? txtObservacao.Text : txtObs_View.Text;
                }

                if (hddProgresso.Value.Contains("Validade"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories2 += valorCampoStories2;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panelFinalizacao_View.Visible = true;

                    txtValidade_View.Text = !string.IsNullOrEmpty(txtValidade.Text) && txtValidade.Text.Length > 3 ? txtValidade.Text : txtValidade_View.Text;

                    txtObs_View.Text = txtObservacao.Text.Length > 0 ? txtObservacao.Text : txtObs_View.Text;
                }

                if (hddProgresso.Value.Contains("Pagamento"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories2 += valorCampoStories2;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panelFinalizacao_View.Visible = true;

                    txtPagamento_View.Text = ddlCondicaoPagamento.Items != null && ddlCondicaoPagamento.Items.Count > 0 && ddlCondicaoPagamento.SelectedValue != "-1" ? ddlCondicaoPagamento.SelectedItem.Text : txtPagamento_View.Text;

                    txtObs_View.Text = txtObservacao.Text.Length > 0 ? txtObservacao.Text : txtObs_View.Text;
                }

                if (hddProgresso.Value.Contains("Vendedor"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories2 += valorCampoStories2;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panelFinalizacao_View.Visible = true;

                    txtVendedor_View.Text = ddlVendedor.Items != null && ddlVendedor.Items.Count > 0 ? ddlVendedor.SelectedItem.Text : txtVendedor_View.Text;

                    txtObs_View.Text = txtObservacao.Text.Length > 0 ? txtObservacao.Text : txtObs_View.Text;
                }

                if (hddProgresso.Value.Contains("Envio"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories2 += valorCampoStories2;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panelFinalizacao_View.Visible = true;

                    txtFormaEnvio_View.Text = ddlFormaEnvio.Items != null && ddlFormaEnvio.Items.Count > 0 && ddlFormaEnvio.SelectedValue != "0" ? ddlFormaEnvio.SelectedItem.Text : txtFormaEnvio_View.Text;

                    txtObs_View.Text = txtObservacao.Text.Length > 0 ? txtObservacao.Text : txtObs_View.Text;
                }

                if (hddProgresso.Value.Contains("DataEntrega"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories2 += valorCampoStories2;

                    div_pn5.Visible = true;
                    aba_Orcamento.Visible = true;
                    div_Orcamento_View.Visible = true;
                    div_panelFinalizacao_View.Visible = true;

                    DateTime.TryParse(txtEstimativaEntrega.Text, out DateTime dtPrevisaoEntrega);
                    txtEstimativa_View.Text = dtPrevisaoEntrega.ToString("dd/MM/yyyy");
                    txtObs_View.Text = txtObservacao.Text.Length > 0 ? txtObservacao.Text : txtObs_View.Text;
                }

                // -------------------------------------------------------------------------------------------------------------
                // Serviços e Recursos

                if (listServicos_Recursos.Any(s => s.bLiberado))
                {
                    if (!hddProgresso.Value.Contains("Servico"))
                        hddProgresso.Value += "Servico|";
                }

                if (hddProgresso.Value.Contains("Servico"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories3 += valorCampoStories3;

                    div_pn5.Visible = true;
                    aba_Servicos_Recursos_View.Visible = listServicos_Recursos.Any(s => s.bLiberado);
                    div_Servicos_Recursos_View.Visible = true;

                    gvServicos_Recursos_View.DataSource = listServicos_Recursos.Where(s => s.bLiberado).OrderBy(s => s.nOrdem);
                    gvServicos_Recursos_View.DataBind();

                    PopulaTotais();

                    if (decimal.Parse(txtTotalServico_Recurso_View.Text) > decimal.Zero && decimal.Parse(txtTotalProjeto_View.Text) < (decimal.Parse(txtTotal_View.Text) * (decimal)0.08))
                        txtTotalProjeto_View.Attributes.Add("style", "background-color: #A9DF8D");
                    else
                        txtTotalProjeto_View.Attributes.Remove("style");
                }

                if (hddProgresso.Value.Contains("Escopo"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories3 += valorCampoStories3;
                }

                // -------------------------------------------------------------------------------------------------------------
                // Etapa 4 - Seleção de Produtos

                if (hddProgresso.Value.Contains("Produto"))
                {
                    porcentagemAtual += valorCampo;
                    porcentagemAtualStories4 += valorCampoStories4;

                    div_pn5.Visible = true;
                    aba_Produtos.Visible = listProdutos.Any(p => p.bLiberado);
                    aba_Comparativo_Produtos.Visible = aba_Produtos.Visible;
                    div_Produtos_View.Visible = true;

                    divExcluirTodos_Produtos.Visible = true;

                    gv_Produtos_View.DataSource = listProdutos.Where(p => p.bLiberado).OrderBy(p => p.nOrdem);
                    gv_Produtos_View.DataBind();

                    PopulaTotais();
                }
                else
                    divExcluirTodos_Produtos.Visible = false;

                // -------------------------------------------------------------------------------------------------------------
                // Outras Manipulações

                if (!hddProgresso.Value.Contains("Produto") && !hddProgresso.Value.Contains("Servico"))
                {
                    txtTotalProduto_View.Text = "0,00";
                    txtTotalProdutos_View.Text = "0,00";
                    txtTotalProdutos.Text = "0,00";

                    txtTotal_View.Text = "0,00";
                }

                txtUF_Fiscal.Text = hddUF_Fiscal_SelectedValue.Value;
                ddlUF_Entrega.SelectedValue = hddUF_Entrega_SelectedValue.Value;
                ddlMunicipio_Entrega.SelectedValue = hddMunicipio_Entrega_SelectedValue.Value;

                if (hddProgresso.Value.Contains("Cliente") && hddProgresso.Value.Contains("Empresa") && hddProgresso.Value.Contains("Vendedor") && hddProgresso.Value.Contains("Pagamento") && (hddProgresso.Value.Contains("Produto") || hddProgresso.Value.Contains("Servico")))
                    aba_Documentos.Visible = true;
                else
                    aba_Documentos.Visible = false;

                if (listServicos_Recursos.Any(s => s.bLiberado))
                {
                    aba_Servicos_Recursos_View.Visible = true;

                    if (dt_CheckList.Rows.Count > 0)
                    {
                        aba_CheckList_View.Visible = true;
                        cbEscopos_PDF.Visible = true;
                    }
                    else
                    {
                        aba_CheckList_View.Visible = false;
                        cbEscopos_PDF.Visible = false;
                    }
                }
                else
                {
                    aba_Servicos_Recursos_View.Visible = false;
                    aba_CheckList_View.Visible = false;
                    cbEscopos_PDF.Visible = false;
                }

                if (listProdutos.Any(p => p.bLiberado))
                    aba_Produtos.Visible = true;
                else
                    aba_Produtos.Visible = false;

                try
                {
                    if (lstEscopos_TipoOrcamento.Items.Count > 1 || dt_CheckList == null)
                    {
                        hddidEscopos.Value = "|";

                        foreach (List_Item item in lstEscopos_TipoOrcamento.Items)
                        {
                            if (item.Selected)
                                hddidEscopos.Value += string.Format("{0}|", item.Value);
                        }

                        Popula_gvCheckList(hddidTipoOrcamento.Value, false);
                    }
                }
                catch { }

                // -------------------------------------------------------------------------------------------------------------
                // Aplicação do Progresso nas barras superiores

                divProgresso.Attributes["style"] = string.Format("width: {0}%", porcentagemAtual);
                divProgressoStories1.Attributes["style"] = string.Format("width: {0}%", porcentagemAtualStories1);
                divProgressoStories2.Attributes["style"] = string.Format("width: {0}%", porcentagemAtualStories2);
                divProgressoStories3.Attributes["style"] = string.Format("width: {0}%", porcentagemAtualStories3);
                divProgressoStories4.Attributes["style"] = string.Format("width: {0}%", porcentagemAtualStories4);

                // -------------------------------------------------------------------------------------------------------------
            }
            catch { }
        }

        /// <summary>
        /// Método utilizado para Manter a Etapa Ativa, após qualquer PostBack.
        /// </summary>
        /// <param name="idEtapa">Recebe o número da Etapa a ser Mantida como Ativa.</param>
        protected void MantemEtapa_Pos_PostBack(int idEtapa)
        {
            switch (idEtapa)
            {
                case 1:     // Etapa 1 = Cadastro Inicial
                    pn1.Attributes.Remove("class");
                    pn1.Attributes.Add("class", "painel aparece");

                    pn2.Attributes.Remove("class");
                    pn2.Attributes.Add("class", "painel desaparece2");
                    pn3.Attributes.Remove("class");
                    pn3.Attributes.Add("class", "painel desaparece2");
                    pn4.Attributes.Remove("class");
                    pn4.Attributes.Add("class", "painel desaparece2");
                    pn5.Attributes.Remove("class");
                    pn5.Attributes.Add("class", "painel desaparece2");

                    painelCollapse1.Attributes["class"] = "cmdStories cmdStories_focus";

                    painelCollapse2.Attributes["class"] = "cmdStories";
                    painelCollapse3.Attributes["class"] = "cmdStories";
                    painelCollapse4.Attributes["class"] = "cmdStories";
                    painelCollapse5.Attributes["class"] = "cmdStories";
                    break;

                case 2:     // Etapa 2 = Informações do Orçamento
                    pn2.Attributes.Remove("class");
                    pn2.Attributes.Add("class", "painel aparece");

                    pn1.Attributes.Remove("class");
                    pn1.Attributes.Add("class", "painel desaparece2");
                    pn3.Attributes.Remove("class");
                    pn3.Attributes.Add("class", "painel desaparece2");
                    pn4.Attributes.Remove("class");
                    pn4.Attributes.Add("class", "painel desaparece2");
                    pn5.Attributes.Remove("class");
                    pn5.Attributes.Add("class", "painel desaparece2");

                    painelCollapse2.Attributes["class"] = "cmdStories cmdStories_focus";

                    painelCollapse1.Attributes["class"] = "cmdStories";
                    painelCollapse3.Attributes["class"] = "cmdStories";
                    painelCollapse4.Attributes["class"] = "cmdStories";
                    painelCollapse5.Attributes["class"] = "cmdStories";
                    break;

                case 3:     // Etapa 3 = Serviços e Recursos
                    pn3.Attributes.Remove("class");
                    pn3.Attributes.Add("class", "painel aparece");

                    pn1.Attributes.Remove("class");
                    pn1.Attributes.Add("class", "painel desaparece2");
                    pn2.Attributes.Remove("class");
                    pn2.Attributes.Add("class", "painel desaparece2");
                    pn4.Attributes.Remove("class");
                    pn4.Attributes.Add("class", "painel desaparece2");
                    pn5.Attributes.Remove("class");
                    pn5.Attributes.Add("class", "painel desaparece2");

                    painelCollapse3.Attributes["class"] = "cmdStories cmdStories_focus";

                    painelCollapse1.Attributes["class"] = "cmdStories";
                    painelCollapse2.Attributes["class"] = "cmdStories";
                    painelCollapse4.Attributes["class"] = "cmdStories";
                    painelCollapse5.Attributes["class"] = "cmdStories";
                    break;

                case 4:     // Etapa 4 = Seleção de Produtos
                    pn4.Attributes.Remove("class");
                    pn4.Attributes.Add("class", "painel aparece");

                    pn1.Attributes.Remove("class");
                    pn1.Attributes.Add("class", "painel desaparece2");
                    pn2.Attributes.Remove("class");
                    pn2.Attributes.Add("class", "painel desaparece2");
                    pn3.Attributes.Remove("class");
                    pn3.Attributes.Add("class", "painel desaparece2");
                    pn5.Attributes.Remove("class");
                    pn5.Attributes.Add("class", "painel desaparece2");

                    painelCollapse4.Attributes["class"] = "cmdStories cmdStories_focus";

                    painelCollapse1.Attributes["class"] = "cmdStories";
                    painelCollapse2.Attributes["class"] = "cmdStories";
                    painelCollapse3.Attributes["class"] = "cmdStories";
                    painelCollapse5.Attributes["class"] = "cmdStories";
                    break;

                case 5:     // Etapa 5 = Visualização Geral / Confirmação
                    pn5.Attributes.Remove("class");
                    pn5.Attributes.Add("class", "painel aparece");

                    pn1.Attributes.Remove("class");
                    pn1.Attributes.Add("class", "painel desaparece2");
                    pn2.Attributes.Remove("class");
                    pn2.Attributes.Add("class", "painel desaparece2");
                    pn3.Attributes.Remove("class");
                    pn3.Attributes.Add("class", "painel desaparece2");
                    pn4.Attributes.Remove("class");
                    pn4.Attributes.Add("class", "painel desaparece2");

                    painelCollapse5.Attributes["class"] = "cmdStories cmdStories_focus";

                    painelCollapse1.Attributes["class"] = "cmdStories";
                    painelCollapse2.Attributes["class"] = "cmdStories";
                    painelCollapse3.Attributes["class"] = "cmdStories";
                    painelCollapse4.Attributes["class"] = "cmdStories";
                    break;
            }

            AtualizaClasseGeral();
            gvProdutos_dataBind();
            gvServicos_Recursos_dataBind();

            Scripts.EsconderCampo(Page, "desaparece2", true);
        }

        /// <summary>
        /// Método utilizado para exibir uma Mensagem, dinamicamente, para qualquer Etapa.
        /// </summary>
        /// <param name="etapa">Recebe o número que representa a Etapa, onde a Mensagem será exibida, sendo elas:<br />1 - Cadastro Inicial<br />2 - Informações do Orçamento<br />3 - Serviços e Recursos<br />4 - Seleção de Produtos<br />5 - Visualização Geral e Confirmação</param>
        /// <param name="tipo">Recebe o Tipo da mensagem que será exibida.<br />Tipos:<br /><br /> ERRO<br /> SUCESSO<br /> AVISO<br /> INFO</param>
        /// <param name="mensagem">Recebe o texto da Mensagem que será exibida.</param>
        protected void MensagemPagina(int etapa, string tipo, string mensagem)
        {
            mensagem = mensagem.Trim();
            tipo = tipo.ToUpper();

            switch (etapa)
            {
                case 1:
                    MensagemPaginaInfoInicial.MostraMensagem(mensagem, tipo, false);
                    break;
                case 2:
                    MensagemPaginaSegundaInfo.MostraMensagem(mensagem, tipo, false);
                    break;
                case 3:
                    MensagemPagina_Dentro_Servicos_Recursos.MostraMensagem(mensagem, tipo, false);
                    break;
                case 4:
                    MensagemPaginaProdutos.MostraMensagem(mensagem, tipo, false);
                    break;
                case 5:
                    MensagemPaginaDentro_View.MostraMensagem(mensagem, tipo, false);
                    break;
            }
        }

        /// <summary>
        /// Este é um método criado com o único propósito de ajustar o alinhamento da página de Detalhe, quando estiver na Consulta de um Orçamento, no caso de ocorrerem PostBack's
        /// </summary>
        protected void AlinhaPagina()
        {
            pn5.Attributes.Remove("class");
            pn5.Attributes.Add("class", "painel");
            div_BreadCrumb.Attributes.Remove("style");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_AlinhaPagina_Comparativos", "$('#page-wrapper').removeClass('page-wrapper').addClass('page-wrapper');\r\n", true);

            Scripts.Mantem_AbaAtiva(Page, "aba-Comparativos");

            rptCategoriasEscopos_dataBind();
        }

        /// <summary>
        /// Função utilizada para gerar uma Nova Linha a partir de uma linha já existente em uma GridView.<br />
        /// Para ser utilizada, esta função é chamada diretamente no código HTML (.aspx) da página em questão.<br />
        /// Será necessário adicionar o código que chama esta função na última coluna da GridView desejada, já que esta função manipula a célula e a linha onde foi chamada, para fechá-las e gerar a Linha adicional.
        /// </summary>
        /// <param name="id">Recebe o ID do objeto que terá sua Composição exibida.</param>
        /// <returns></returns>
        public string NovaLinha(string id)
        {
            /* 
            * Passo a passo:
            * 1. Fecha a célula atual
            * 2. Fecha a linha Atual
            * 3. Cria uma nova linha com o ID e a classe <TR id='...' style='...'>
            * 4. Cria uma célula em branco: <td></td>
            * 5. Cria uma nova célula para conter o gridview
            ************************************************************/
            if (!string.IsNullOrEmpty(id.ToString()))
                return $"</td></tr><tr id='{id}' class='collapsed-row'><td></td><td colspan='100' style='padding:0px; margin:0px;'>";
            else
                return string.Empty;
        }

        /// <summary>
        /// Função utilizada para gerar uma Nova Linha a partir de uma linha já existente em uma GridView.<br />
        /// Para ser utilizada, esta função é chamada diretamente no código HTML (.aspx) da página em questão.<br />
        /// Será necessário adicionar o código que chama esta função na última coluna da GridView desejada, já que esta função manipula a célula e a linha onde foi chamada, para fechá-las e gerar a Linha adicional.
        /// </summary>
        /// <param name="id">Recebe o ID do objeto que terá sua Composição exibida.</param>
        /// <param name="gridNome">Recebe o nome da GridView onde será exibida a Composição do Item em questão.</param>
        /// <returns></returns>
        public string NovaLinha(object id, string gridNome)
        {
            /* 
            * Passo a passo:
            * 1. Fecha a célula atual
            * 2. Fecha a linha Atual
            * 3. Cria uma nova linha com o ID e a classe <TR id='...' style='...'>
            * 4. Cria uma célula em branco: <TD></TD>
            * 5. Cria uma nova célula para conter o gridview
            ************************************************************/
            if (id != null && !string.IsNullOrEmpty(id.ToString()))
            {
                // Se houver um ID, retorna a nova linha com o ID e a classe
                return string.Format(@"</td></tr><tr id='tr{0}{1}' class='collapsed-row'>
                               <td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            }
            else
            {
                // Se não houver ID, retorna uma string vazia para que nada seja renderizado e o botão de colapso desapareça
                return string.Empty;
            }
        }

        /// <summary>
        /// Método utilizado para Inicializar o necessário para que o Orçamento possa ser aplicado como Empreitada
        /// </summary>
        protected void IniciaEmpreitada()
        {
            div_ComparativosProdutos.Visible = false;

            foreach (cls_Comercial_Tabelas item in listProdutos_Empreitada.OrderBy(p => p.nOrdem))
            {
                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_ITEM_x_TIPO_TABELA" },
                    { "@idTipoTabela", "10" },
                    { "@idItem", item.IdItem.ToString() }
                };
                DataSet ds = ExecutarDataSet(sProcedure_TabelaPreco, vParam);

                if (ValidarDataSet(ds))
                {
                    item.Preco = decimal.Parse(DATASET(ds, "nTotal"));
                    item.nUnitario = item.Preco;
                }

                item.NTotal = Math.Round((item.Preco - (item.Preco * (item.NFator / 100))) * item.NQuantidade, 2);
            }

            gvComparativoProdutos.DataSource = listProdutos_Empreitada.OrderBy(p => p.nOrdem);
            gvComparativoProdutos.DataBind();

            // Coluna de Seleção de Produtos para o Comparativo de Produtos
            gvComparativoProdutos.Columns[0].HeaderStyle.CssClass = "id";
            gvComparativoProdutos.Columns[0].ItemStyle.CssClass = "id";

            // Colunas de Ordem
            // 2 - Ordem com botão de Composição
            // 3 - Apenas a Ordem
            gvComparativoProdutos.Columns[2].HeaderStyle.CssClass = "id";
            gvComparativoProdutos.Columns[2].ItemStyle.CssClass = "id";
            gvComparativoProdutos.Columns[3].HeaderStyle.CssClass = "";
            gvComparativoProdutos.Columns[3].ItemStyle.CssClass = "";

            // Colunas de Prazo
            // 9 - Prazo para Consulta
            // 10 - Editar Prazo
            gvComparativoProdutos.Columns[9].HeaderStyle.CssClass = "";
            gvComparativoProdutos.Columns[9].ItemStyle.CssClass = "";
            gvComparativoProdutos.Columns[10].HeaderStyle.CssClass = "id";
            gvComparativoProdutos.Columns[10].ItemStyle.CssClass = "id";

            gvComparativoServicos.DataSource = listServicos_Empreitada.OrderBy(s => s.nOrdem);
            gvComparativoServicos.DataBind();

            // Colunas de Total
            // -3 = Editar Total
            // -2 = Consultar Total
            // -1 = Excluir Serviço
            gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 3].HeaderStyle.CssClass = "";
            gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 3].ItemStyle.CssClass = "";
            gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 2].HeaderStyle.CssClass = "id";
            gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 2].ItemStyle.CssClass = "id";
            gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 1].HeaderStyle.CssClass = "";
            gvComparativoServicos.Columns[gvComparativoServicos.Columns.Count - 1].ItemStyle.CssClass = "";
        }

        /// <summary>
        /// Método utilizado para Atualizar os Repeaters de visualização.
        /// </summary>
        protected void Atualiza_Repeaters()
        {
            string segmentos = "|";
            string tipos = "|";
            string escopos = "|";

            // -------------------------------------------------------------------------------------------------------------
            // Popular a visualização dos Segmentos do Cliente

            foreach (List_Item item in lstSegmentosCliente.Items)
            {
                if (item.Selected)
                    segmentos += item.Value + "|";
            }

            hddidSegmentos.Value = string.IsNullOrEmpty(segmentos.Trim().Replace("|", "")) ? hddidSegmentos.Value : segmentos;

            PopulaSegmentosCliente();

            // -------------------------------------------------------------------------------------------------------------
            // Popular a visualização dos Tipos de Serviços

            foreach (List_Item item in lstTipoServicos_TipoOrcamento.Items)
            {
                if (item.Selected)
                    tipos += item.Text + "|";
            }

            hddsDscTiposServicos.Value = string.IsNullOrEmpty(tipos.Trim().Replace("|", "")) ? hddsDscTiposServicos.Value : tipos;

            PopulaTipoServicos();

            // -------------------------------------------------------------------------------------------------------------
            // Popular a visualização dos Escopos

            foreach (List_Item item in lstEscopos_TipoOrcamento.Items)
            {
                if (item.Selected)
                    escopos += item.Text + "|";
            }

            hddsDscEscopos.Value = string.IsNullOrEmpty(escopos.Trim().Replace("|", "")) ? hddsDscEscopos.Value : escopos;

            PopulaEscopos();

            // -------------------------------------------------------------------------------------------------------------
        }

        protected DataSet ConsultaImpostos_Produtos(bool bCodigo, string sCodigo__id_Produto, string idEmpresa__idParceiro, string idParceiro__idParceiroCliente, string idDestino, string sUF_Destino, string idTabela = "0", string nVlrProduto = "0", string sSistema = "N")
        {
            string proc = sProcedure;
            Dictionary<string, string> vParam = null;

            if (bCodigo)
            {
                proc = sProcedure_Produtos;

                vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTA_PRODUTO_x_CODIGO" },
                    { "@sCodigo", sCodigo__id_Produto },
                    { "@idTabela", idTabela },
                    { "@idParceiro", idEmpresa__idParceiro },
                    { "@idParceiro_Cliente", idParceiro__idParceiroCliente },
                    { "@idDestino", idDestino },
                    { "@sUF_Destino", sUF_Destino },
                    { "@sSitema", sSistema },
                    { "@vlrFrete", string.IsNullOrEmpty(txtFrete_View.Text) || txtFrete_View.Text == "0" ? "0" : txtFrete_View.Text.Replace(".", "").Replace(',', '.').Trim() }
                };
            }
            else
            {
                proc = sProcedure;

                vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_IMPOSTOS_x_PRODUTO" },
                    { "@idProduto", sCodigo__id_Produto },
                    { "@idTabelaPreco", idTabela },
                    { "@idEmpresa", idEmpresa__idParceiro },
                    { "@idParceiro", idParceiro__idParceiroCliente },
                    { "@idDestino", idDestino },
                    { "@nVlrProduto", nVlrProduto },
                    { "@sUF_Destino", sUF_Destino },
                    { "@sSitema", sSistema },
                    { "@nFretePrevisto", string.IsNullOrEmpty(txtFrete_View.Text) || txtFrete_View.Text == "0" ? "0" : txtFrete_View.Text.Replace(".", "").Replace(',', '.').Trim() }
                };
            }

            return ExecutarDataSet(proc, vParam);
        }

        protected byte[] Conuslta_ImagemFornecedores()
        {
            byte[] img = null;

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_IMAGEM_FORNECEDORES" },
                    { "@idParceiro", hddidEmpresa.Value }
                };
                DataSet ds = ExecutarDataSet(sProcedure_Empresas, vParametros);

                img = (byte[])ds.Tables[0].Rows[0]["imgFornecedores"];
            }
            catch { }

            return img;
        }

        #endregion

        #endregion

        #region | Eventos

        #region | Click

        protected void cmdSelecionarParceiro_Click(object sender, EventArgs e)
        {
            if (FiltroPesquisaParceiros.ValidaParceiro())
            {
                AtualizaClasseGeral();

                lnkAtualizaInfo.Visible = true;

                div_Enderecos.Visible = true;
                div_IE.Visible = true;
                div_Contato.Visible = true;
                div_tabela.Visible = true;
                div_orcamento.Visible = true;
                div_Confidencial.Visible = true;
                div_Revisao.Visible = Request["id"] != "0";

                if (!hddProgresso.Value.Contains("Cliente"))
                    hddProgresso.Value += "Cliente|";

                if (!hddProgresso.Value.Contains("DestinoVenda"))
                    hddProgresso.Value += "DestinoVenda|";

                string sMensagem = "";

                hddidCliente.Value = FiltroPesquisaParceiros.IDParceiro.ToString();
                FiltroPesquisaProdutos.IDParceiro = hddidCliente.Value;

                string idPais = Variaveis.idEmpresa() == "Brasil" ? "2" : Variaveis.idEmpresa() == "0" ? "2" : "0";
                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "SelecionaParceiro__Orcamentos" },
                    { "@idPesquisa", hddidCliente.Value },
                    { "@idFiltro", idPais },
                    { "@idFiltro_1", Request["id"] }
                };

                List<cls_Multiplos_Combos> ddls = new List<cls_Multiplos_Combos>
                {
                    new cls_Multiplos_Combos { ddl = ddlTabela, sCampoCodigo = "idTabela", sCampoDescricao = "sDscTabela", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione a Tabela de Preço", sValorPrimeiraLinha = "0" },
                    new cls_Multiplos_Combos { ddl = ddlCondicaoPagamento, sCampoCodigo = "idCondicaoPagamento", sCampoDescricao = "sDscCondicaoPagamento", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione", sValorPrimeiraLinha = "-1" },
                    new cls_Multiplos_Combos { ddl = ddlEndereco, sCampoCodigo = "idEndereco", sCampoDescricao = "sEndereco", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione o Endereço Fiscal", sValorPrimeiraLinha = "0" },
                    new cls_Multiplos_Combos { ddl = ddlEndereco_Entrega, sCampoCodigo = "idEndereco", sCampoDescricao = "sEndereco", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione o Endereço de Entrega", sValorPrimeiraLinha = "0" },
                    new cls_Multiplos_Combos { ddl = ddlContato, sCampoCodigo = "idContato", sCampoDescricao = "sNome", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione o Contato", sValorPrimeiraLinha = "0" }
                };
                Popula_Multiplos_Combos(ddls, vParam);

                try
                {
                    ddlEndereco.SelectedIndex = 1;
                    ddlEndereco_SelectedIndexChanged(null, new EventArgs());

                    ddlEndereco_Entrega.SelectedIndex = 1;
                    ddlEndereco_Entrega_SelectedIndexChanged(null, new EventArgs());

                    ddlContato.SelectedIndex = 1;
                    hddidContato.Value = ddlContato.SelectedValue;
                }
                catch { }

                if (ddlContato.Items.Count <= 1)
                {
                    sMensagem += $"<div class=\"link\"><b>Aviso:</b> Não existem Contatos cadastrados no Parceiro selecionado, será necessário validar na Página do Parceiro  <a href=\"/Manutencao/Parceiros_Detalhe.aspx?id={hddidCliente.Value}\"><b><i class=\"fa fa-arrow-right\"></i></b> {FiltroPesquisaParceiros.sDscParceiro_Colaborador}</a>!</div>";
                    ddlContato.Attributes.Add("disabled", "disabled");
                    div_Contato_contato.Attributes["class"] += " chosenColor warning";
                }
                else
                {
                    ddlContato.Attributes.Remove("disabled");
                    div_Contato_contato.Attributes["class"] = div_Contato_contato.Attributes["class"].Replace(" chosenColor warning", "");
                }

                ddlEndereco_Entrega.Items.Add(new List_Item("Coleta", "-1"));

                Dictionary<string, string> vParametrosTabela = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idParceiro", hddidCliente.Value }
                };
                DataSet ds = ExecutarDataSet(sProcedure_Clientes, vParametrosTabela);

                if (ValidarDataSet(ds))
                {
                    hddsTipoCliente.Value = DATASET(ds, "sTipo");

                    string sidTabela = DATASET(ds, "sidTabela").Trim();
                    int.TryParse(sidTabela, out int idTabela);

                    if (!string.IsNullOrEmpty(sidTabela) && idTabela.Equals(0))
                    {
                        foreach (string sid in sidTabela.Split('|'))
                        {
                            if (int.TryParse(sid.Trim(), out int id))
                            {
                                idTabela = id;
                                break;
                            }
                        }
                    }

                    if (idTabela <= 0)
                    {
                        div_tabela.Attributes["class"] += " chosenColor warning";
                        sMensagem += string.IsNullOrEmpty(sMensagem) ? "" : "<br /><br />";
                        sMensagem += "<b>Aviso:</b> Não existe Tabela de Preços cadastrada no Parceiro selecionado, uma Tabela padronizada foi selecionada automaticamente!";
                    }
                    else if (!(Nivel_Permissao >= 2))
                        ddlTabela.Attributes.Add("disabled", "disabled");
                    else
                        ddlTabela.Attributes.Remove("disabled");

                    txtIE.Text = DATASET(ds, "sRG_IE");

                    div_Moeda.Visible = DATASET(ds, "sCliente_Nacional") == "N";
                    ddlMoeda.SelectedValue = "2";
                    txtCambio.Text = "1,0000";

                    div_Custo.Visible = DATASET(ds, "sCliente_Nacional") == "N";
                    txtCusto_Aduaneiro.Text = "0,00";
                    txtCusto_Despachante.Text = "0,00";
                }

                if (!string.IsNullOrEmpty(sMensagem))
                    MensagemPaginaInfoInicial.MostraMensagem_Aviso(sMensagem, false);

                txtIE_View.Text = txtIE.Text;

                if (ddlTabela.Items.Count > 1)
                {
                    ddlTabela.SelectedIndex = 1;
                    txtTabela_View.Text = ddlTabela.SelectedItem.Text;
                    txtTabelaPreco_ImportarProdutos.Text = txtTabela_View.Text;
                    cmdTabelaPreco_ImportarProdutos.Visible = true;
                }
                else
                {
                    ddlTabela.SelectedIndex = 0;
                    txtTabela_View.Text = string.Empty;
                    txtTabelaPreco_ImportarProdutos.Text = string.Empty;
                    cmdTabelaPreco_ImportarProdutos.Visible = false;

                    MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Não foi possível encontrar as Tabelas de Preços disponíveis para o Parceiro selecionado!", false);
                }

                UpdModal_Importar_Pedidos.Update();
                UpdModal_Importar_LM.Update();
                UpdModal_Importar_Orcamento.Update();
                UpdModal_Importar_TabelaPreco.Update();

                if (!hddProgresso.Value.Contains("TabelaPreco"))
                    hddProgresso.Value += "TabelaPreco|";

                int nPaisEmpresa = 2;

                if (ddlEmpresa_Orcamento.Items.Count > 0 && ddlEmpresa_Orcamento.SelectedItem.Text != "Selecione a Empresa")
                {
                    Dictionary<string, string> vParametrosEmpresa = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idParceiro", ddlEmpresa_Orcamento.SelectedValue }
                    };
                    DataSet dsEmpresa = ExecutarDataSet(sProcedure_Empresas, vParametrosEmpresa);

                    if (ValidarDataSet(dsEmpresa))
                    {
                        try
                        {
                            nPaisEmpresa = string.IsNullOrEmpty(DATASET(dsEmpresa, 2, 0, "idTipoPais")) ? 2 : int.Parse(DATASET(dsEmpresa, 2, 0, "idTipoPais"));
                        }
                        catch { }
                    }
                }

                FiltroPesquisaProdutos.TerritorioEmpresa = nPaisEmpresa;
                FiltroPesquisaProdutos.idTabelaParceiro = ddlTabela.SelectedValue;
                FiltroPesquisaProdutos.RegistrarScriptPesquisar();

                if (string.IsNullOrEmpty(Request["crm"]) && string.IsNullOrEmpty(Request["idCotacao"])) MensagemPaginaInfoInicial.MostraMensagem_Sucesso("Parceiro selecionado com sucesso!", false);

                if (listProdutos.Count > 0)
                    MantemProdutos(ddlTabela.SelectedValue);

                Scripts.FocusScript(Page, ddlEndereco.ClientID);
            }
            else
            {
                listProdutos.Clear();

                cmdVoltarEtapa.Visible = false;
                cmdAvancarEtapa.Visible = false;
                divStories.Visible = false;
                div_orcamento.Visible = false;
                div_Confidencial.Visible = false;
                div_tabela.Visible = false;
                div_Tabela_Obs.Visible = false;
                div_Revisao.Visible = false;
                div_IE.Visible = false;
                div_Enderecos.Visible = false;
                div_Contato.Visible = false;
                div_empresa.Visible = false;

                if (hddProgresso.Value.Contains("Cliente"))
                    hddProgresso.Value = hddProgresso.Value.Replace("Cliente|", "");

                if (hddProgresso.Value.Contains("DestinoVenda"))
                    hddProgresso.Value = hddProgresso.Value.Replace("DestinoVenda|", "");

                if (hddProgresso.Value.Contains("TabelaPreco"))
                    hddProgresso.Value = hddProgresso.Value.Replace("TabelaPreco|", "");

                MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Não foi possível selecionar o Parceiro!", false);

                FiltroPesquisaParceiros.Focus_sCodigo();
            }

            AtualizaBarraProgresso(0, true);
            MantemEtapa_Pos_PostBack(1);
        }

        protected void cmdIncluirProduto_Click(object sender, EventArgs e)
        {
            if (!hddProgresso.Value.Contains("Cliente"))
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Parceiro antes de Incluir Produtos!", false);
            else try { IncluirItem(); } catch (Exception ex) { MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro ao Incluir o Produto ao Orçamento!<br /><b>Erro: </b>" + ex.Message, false); }

            FiltroPesquisaProdutos.LimparCampos();
            FiltroPesquisaProdutos.Focus_sCodigo();
            MantemEtapa_Pos_PostBack(4);
            AtualizaBarraProgresso(4, false);
        }

        protected void cmdAtualiza_Click(object sender, EventArgs e)
        {
            Thread.Sleep(250);

            if (aba_Comparativos.Visible)
                AtualizaClasses_Comparativo();
            else
            {
                int etapa = 5;

                string id = (sender as LinkButton).ID;
                if (id.Contains("Inicial"))
                    etapa = 1;
                else if (id.Contains("Info"))
                    etapa = 2;
                else if (id.Contains("Servicos"))
                    etapa = 3;
                else if (id.Contains("Produtos"))
                    etapa = 4;

                MantemEtapa_Pos_PostBack(etapa);
                AtualizaBarraProgresso(etapa, true);
            }
        }

        protected void cmdVincularNovo_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR" },
                { "@dtInclusao", DateTime.Now.ToString() },
                { "@idTipoCotacao", ddlTipoOrcamento.SelectedValue },
                { "@idVendedor", hddidVendedor.Value },
                { "@idCliente", hddidCliente.Value },
                { "@sDscParceiro", txtRazaoSocial_View.Text },
                { "@sReferencia", txtReferencia.Text },
                { "@sObservacao", txtObservacao.Text },
                { "@dtPrevisao", txtEstimativa_View.Text },
                { "@sConfidencial", ddlConfidencial.SelectedValue },
                { "@nServico", txtTotalServico_Recurso_View.Text.StringToDecimalString() },
                { "@nMaterial", txtTotalProduto_View.Text.StringToDecimalString() },
                { "@nValor", (decimal.Parse(txtTotalProduto_View.Text) + decimal.Parse(txtTotalServico_Recurso_View.Text)).ToString().StringToDecimalString() },
                { "@idUsuarioatualizacao", Variaveis.idUsuario() },
                { "@idOrcamento", hddNumeroPedido.Value }
            };
            DataSet ds = ExecutarDataSet(sProcedure_CRM, vParametros);

            Dictionary<string, string> vParametrosOrcamento = new Dictionary<string, string>
            {
                { "@sFuncao", "VINCULAR_CRM" },
                { "@idCRM", DATASET(ds, "idRegistroCRM") },
                { "@idPedido", hddNumeroPedido.Value }
            };
            ExecutarDataSet(sProcedure, vParametrosOrcamento);

            DirecionaPagina(string.Format("App/Paginas/Comercial/CRM.aspx?id={0}&orcamento={1}", DATASET(ds, "idRegistroCRM"), DATASET(ds, "idOrcamento")));
        }

        protected void cmdVincular_Click(object sender, EventArgs e)
        {
            if (ddlVincularCRM.SelectedValue != "0")
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "VINCULAR_CRM" },
                    { "@idCRM", ddlVincularCRM.SelectedValue },
                    { "@idPedido", hddNumeroPedido.Value }
                };

                ExecutarDataSet(sProcedure, vParametros);

                Scripts.RemoverBackdrop_Modal(Page);
                Scripts.FecharModal(Page, "modalVincularCRM");
                MensagemPaginaDentro_View.MostraMensagem_Sucesso("CRM vinculado com sucesso!", false);

                MantemEtapa_Pos_PostBack(5);
                cmdVincular_group.Visible = false;
            }
            else
            {
                MensagemPaginaModalVincula.MostraMensagem_Erro("<b>Erro:</b> Para vincular à um CRM Existente é necessário selecionar o CRM desejado!", false);
                Scripts.AbrirModal(Page, "modalVincularCRM");
            }

            pn5.Attributes["class"] = "painel";
            div_Paineis.Attributes["style"] = "display: flex; height: 80%; width: 100%; padding: 0 30px 0 30px;";
        }

        protected void cmdFecharModal_Click(object sender, EventArgs e) => Scripts.FecharModal(Page, "modalVincularCRM");

        protected void cmdNovoEndereco_Salvar_Click(object sender, EventArgs e)
        {
            if (ValidaNovoEndereco())
            {
                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "ENDERECO_INCLUIR" },
                    { "@idParceiro", hddidCliente.Value },
                    { "@idTipoEndereco", "3" }, // 3 - Entrega
                    { "@sCEP", txtCEP_NovoEndereco.Text },
                    { "@sLogradouro", txtLogradouro_NovoEndereco.Text },
                    { "@sNumero", txtNumero_NovoEndereco.Text },
                    { "@sComplemento", txtComplemento_NovoEndereco.Text },
                    { "@sBairro", txtBairro_NovoEndereco.Text },
                    { "@sCidade", ddlCidade_NovoEndereco.SelectedItem.Text },
                    { "@sEstado", ddlEstado_NovoEndereco.SelectedValue },
                    { "@sPais", txtPais_NovoEndereco.Text.ToUpper() }
                };

                DataSet ds = ExecutarDataSet(sProcedure_Clientes, vParam);

                LimparCampos_NovoEndereco();

                Popula_Combo(ddlEndereco_Entrega, "sp_Select 'Flow_Clientes_Endereco', " + hddidCliente.Value + ", @idFiltro=3", "idEndereco", "sEndereco", false, "Selecione um Endereço de Entrega", "0");

                if (ValidarDataSet(ds))
                {
                    ddlEndereco_Entrega.SelectedValue = DATASET(ds, "idCliente_Endereco");
                    ddlEndereco_Entrega_SelectedIndexChanged(ddlEndereco_Entrega, new EventArgs());
                }

                MensagemPagina_Modal_NovoEndereco.MostraMensagem_Sucesso("Novo Endereço de Entrega registrado com sucesso!", false);
            }

            AbrirModal_NovoEndereco();
        }

        protected void cmdAtualiza_Valores_Produtos_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (cls_Comercial_Tabelas produto in listProdutos)
                {
                    if (!produto.bImportado && !produto.bSistema && produto.IdItem > 0)
                        AtualizaValores_Produtos(produto);
                }

                gvProdutos_dataBind();
            }
            catch (Exception ex)
            {
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de Atualizar os Valores dos Produtos!<br />Erro ao atualizar: " + ex.Message, false);
            }

            MantemEtapa_Pos_PostBack(4);
            AtualizaBarraProgresso(4, false);
        }

        #region | Avançar e Retornar

        protected void cmdAvancar_click(object sender, EventArgs e)
        {
            int.TryParse(txtID_Orcamento.Text, out int id);

            Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_PROX_PEDIDO" },
                    { "@idPedido", id.ToString() },
                    { "@idTipo", "1" },
                    { "@idFiltro", "1" }
                };

            DataSet ds = ExecutarDataSet(sProcedure, vParam);

            if (ValidarDataSet(ds))
            {
                id = int.Parse(DATASET(ds, "idPedido"));
                DirecionaPagina(string.Format("App/Paginas/Comercial/Orcamento_Detalhe.aspx?id={0}", id));
            }
            else
                MensagemPaginaDentro_View.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na Consulta do ID do próximo Orçamento!", false);
        }

        protected void cmdRetornar_click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtID_Orcamento.Text);

            Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_PROX_PEDIDO" },
                    { "@idPedido", id.ToString() },
                    { "@idTipo", "1" },
                    { "@idFiltro", "2" }
                };

            DataSet ds = ExecutarDataSet(sProcedure, vParam);

            if (ValidarDataSet(ds))
            {
                id = int.Parse(DATASET(ds, "idPedido"));
                DirecionaPagina($"App/Paginas/Comercial/Orcamento_Detalhe.aspx?id={id}");
            }
            else
                MensagemPaginaDentro_View.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na Consulta do ID do Orçamento anterior!", false);
        }

        #endregion

        protected void cmdConverter_Sistema_Comparativos_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlSistemas_Comparativo.SelectedValue != "0")
                {
                    hddComparativoProdutos.Value = "S";

                    int.TryParse(ddlSistemas_Comparativo.SelectedValue, out int idSistema);

                    List<string> idSelecionados = new List<string>();

                    foreach (GridViewRow row in gvComparativoProdutos.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            if ((row.Cells[Produtos_Comparativos_Coluna__CheckBox].Controls[1] as CheckBox).Checked)
                                idSelecionados.Add(row.Cells[Produtos_Comparativos_Coluna__ID].Text);
                        }
                    }

                    if (idSelecionados.Count > 1)
                    {
                        int ordemSistema = 0;
                        decimal precoSistema = 0;
                        string prazoSistema = "0";
                        int idRegistro_Sistema = GerarNovo_idRegistro(listProdutos_Comparativos.Where(p => p.bSistema));

                        foreach (GridViewRow row in gvComparativoProdutos.Rows)
                        {
                            if (row.RowType == DataControlRowType.DataRow)
                            {
                                var produtoOriginal = listProdutos.FirstOrDefault(p => p.idRegistro.ToString() == row.Cells[Produtos_Comparativos_Coluna__ID].Text);

                                if (produtoOriginal != null)
                                {
                                    cls_Comercial_Tabelas produto = new cls_Comercial_Tabelas
                                    {
                                        idRegistro = GerarNovo_idRegistro(listProdutos_Comparativos_Composicao),
                                        SFuncao = "INCLUIR ITEM",
                                        IdItem = produtoOriginal.IdItem,
                                        idItemPai = idRegistro_Sistema,
                                        nOrdem = produtoOriginal.nOrdem,
                                        SCodigo = produtoOriginal.SCodigo,
                                        SDscProduto = produtoOriginal.SDscProduto,
                                        sNCM = produtoOriginal.sNCM,
                                        TipoProduto = produtoOriginal.TipoProduto,
                                        IdGrupoProduto = produtoOriginal.IdGrupoProduto,
                                        IdFamiliaProduto = produtoOriginal.IdFamiliaProduto,
                                        sDscGrupoProduto = produtoOriginal.sDscGrupoProduto,
                                        sDscFamiliaProduto = produtoOriginal.sDscFamiliaProduto,
                                        SUnidade = produtoOriginal.SUnidade,
                                        sIndustrializado = produtoOriginal.sIndustrializado,
                                        sOrigem = produtoOriginal.sOrigem,

                                        sCFOP = produtoOriginal.sCFOP,
                                        sCST = produtoOriginal.sCST,

                                        NIPI = produtoOriginal.NIPI,
                                        NPIS = produtoOriginal.NPIS,
                                        NCOFINS = produtoOriginal.NCOFINS,
                                        NICMS = produtoOriginal.NICMS,
                                        NST = produtoOriginal.NST,

                                        nBaseCalc_ICMS = produtoOriginal.nBaseCalc_ICMS,
                                        nVlr_IPI = produtoOriginal.nVlr_IPI,
                                        nVlr_PIS = produtoOriginal.nVlr_PIS,
                                        nVlr_COFINS = produtoOriginal.nVlr_COFINS,
                                        nVlr_ICMS = produtoOriginal.nVlr_ICMS,
                                        nVlr_DIFAL = produtoOriginal.nVlr_DIFAL,
                                        nVlr_ST = produtoOriginal.nVlr_ST,
                                        nVlr_Liquido = produtoOriginal.nVlr_Liquido,

                                        NQuantidade = produtoOriginal.NQuantidade,
                                        nPesoBruto = produtoOriginal.nPesoBruto,
                                        nPesoLiquido = produtoOriginal.nPesoLiquido,
                                        nVolume = produtoOriginal.nVolume,
                                        NFator = produtoOriginal.NFator,
                                        Preco = produtoOriginal.Preco,
                                        nUnitario = produtoOriginal.nUnitario,
                                        NTotal = produtoOriginal.NTotal,
                                        bLiberado = true,
                                        bSistema = produtoOriginal.bSistema,
                                        dtInclusao = produtoOriginal.dtInclusao
                                    };

                                    if ((row.Cells[Produtos_Comparativos_Coluna__CheckBox].Controls[1] as CheckBox).Checked)
                                    {
                                        ordemSistema = ordemSistema > 0 ? ordemSistema : produtoOriginal.nOrdem;

                                        decimal valor = produtoOriginal.NIPI > decimal.Zero ? produtoOriginal.Preco / (produtoOriginal.NIPI / 100 + 1) * produtoOriginal.NQuantidade : produtoOriginal.Preco * produtoOriginal.NQuantidade;
                                        precoSistema += valor - (valor * (produtoOriginal.NFator / 100));

                                        try
                                        {
                                            listProdutos_Comparativos.Remove(listProdutos_Comparativos.Where(p => p.IdItem == produtoOriginal.IdItem).First());
                                        }
                                        catch { }

                                        listProdutos_Comparativos_Composicao.Add(produto);
                                    }
                                    else if (listProdutos_Comparativos.FirstOrDefault(p => p.idRegistro.Equals(produtoOriginal.idRegistro)) == null)
                                    {
                                        produto.idRegistro = produtoOriginal.idRegistro > 0 ? produtoOriginal.idRegistro : produto.idRegistro;
                                        listProdutos_Comparativos.Add(produto);

                                        if (produtoOriginal.bSistema)
                                        {
                                            foreach (var p in listProdutos_Composicao.Where(p => p.idItemPai.Equals(produto.idRegistro)))
                                            {
                                                listProdutos_Comparativos_Composicao.Add(p);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        DataSet ds = ConsultaImpostos_Produtos(false, ddlSistemas_Comparativo.SelectedValue, hddidEmpresa.Value, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", precoSistema.ToString().Replace(",", "."), "S");

                        if (ValidarDataSet(ds))
                        {
                            int.TryParse(DATASET(ds, "idRegra"), out int idRegra);

                            cls_Comercial_Tabelas sistema = new cls_Comercial_Tabelas
                            {
                                idRegistro = idRegistro_Sistema,
                                SFuncao = "INCLUIR ITEM",
                                nOrdem = ordemSistema > 0 ? ordemSistema : listProdutos_Comparativos.Count(p => p.bSistema) + 10,
                                IdItem = idSistema,
                                SCodigo = DATASET(ds, "sCodigo"),
                                SDscProduto = DATASET(ds, "sDscProduto"),
                                sNCM = DATASET(ds, "sCodigoNCM") == "0" ? "Não Cadastrado" : DATASET(ds, "sCodigoNCM"),
                                TipoProduto = DATASET(ds, "sDscTipoProduto"),
                                IdGrupoProduto = int.Parse(DATASET(ds, "idGrupo")),
                                IdFamiliaProduto = int.Parse(DATASET(ds, "idFamilia")),
                                sDscGrupoProduto = DATASET(ds, "sDscGrupo"),
                                sDscFamiliaProduto = DATASET(ds, "sDscFamilia"),
                                SUnidade = DATASET(ds, "sUnidade"),
                                sIndustrializado = DATASET(ds, "sProdutoIndustrializado") == "S" ? "Sim" : "Não",
                                sOrigem = DATASET(ds, "sProdutoImportado") == "S" ? "IMP" : "BR",

                                idRegra = idRegra,

                                sCFOP = DATASET(ds, "sCFOP"),
                                sCST = DATASET(ds, "CST"),

                                NIPI = Math.Round(decimal.Parse(DATASET(ds, "nIPI")), 2),
                                NPIS = Math.Round(decimal.Parse(DATASET(ds, "nPIS")), 2),
                                NCOFINS = Math.Round(decimal.Parse(DATASET(ds, "nCOFINS")), 2),
                                NICMS = Math.Round(decimal.Parse(DATASET(ds, "nICMS")), 2),
                                NST = 0.00m,
                                NDIFAL = 0.00m,

                                nBaseCalc_ICMS = Math.Round(decimal.Parse(DATASET(ds, "nBaseCalculoICMS")), 2),
                                nVlr_IPI = Math.Round(decimal.Parse(DATASET(ds, "nVlrIPI")), 2),
                                nVlr_PIS = Math.Round(decimal.Parse(DATASET(ds, "nVlrPIS")), 2),
                                nVlr_COFINS = Math.Round(decimal.Parse(DATASET(ds, "nVlrCOFINS")), 2),
                                nVlr_ICMS = Math.Round(decimal.Parse(DATASET(ds, "nVlrICMS")), 2),
                                nVlr_ST = 0.00m,
                                nVlr_DIFAL = 0.00m,
                                nVlr_Liquido = Math.Round(decimal.Parse(DATASET(ds, "nVlrLiquido")), 2),

                                NQuantidade = 1.00M,
                                nPesoBruto = Math.Round(decimal.Parse(DATASET(ds, "nPesoBruto")), 2),
                                nPesoLiquido = Math.Round(decimal.Parse(DATASET(ds, "nPesoNeto")), 2),
                                nVolume = Math.Round(decimal.Parse(DATASET(ds, "nVolume")), 2),
                                bLiberado = true,
                                bSistema = true,

                                NFator = Math.Round(decimal.Zero, 4)
                            };
                            sistema.Preco = Math.Round(precoSistema * (sistema.NIPI / 100 + 1), 2);
                            sistema.nUnitario = Math.Round(sistema.Preco, 2);
                            sistema.NTotal = Math.Round(sistema.Preco, 2);

                            foreach (var p in listProdutos_Comparativos_Composicao.Where(p => p.idItemPai.Equals(sistema.idRegistro) && !string.IsNullOrEmpty(p.dtInclusao)))
                            {
                                if (int.Parse(p.dtInclusao) > int.Parse(prazoSistema))
                                    prazoSistema = p.dtInclusao;
                            }

                            sistema.dtInclusao = prazoSistema;

                            listProdutos_Comparativos.Add(sistema);

                            hddComparativoSistemas.Value = "S";

                            PopulaTotais_Comparativo();

                            if (txtComparativos_LiquidoProdutos_Ajustado.Text != txtComparativo_LiquidoProdutos.Text)
                            {
                                decimal totalLiquido = decimal.Parse(txtComparativo_LiquidoProdutos.Text);
                                decimal totalLiquidoAjustado = decimal.Parse(txtComparativos_LiquidoProdutos_Ajustado.Text);
                                decimal diferenca = (totalLiquido - totalLiquidoAjustado) / listProdutos_Comparativos.Count(p => p.bLiberado);

                                foreach (var p in listProdutos_Comparativos.Where(p => p.bLiberado))
                                {
                                    p.NFator = Math.Round(diferenca / p.nVlr_Liquido * 100 * -1, 4);
                                    p.NTotal = Math.Round(p.Preco * p.NQuantidade - (p.Preco * p.NQuantidade * (p.NFator / 100)), 2);
                                    p.nUnitario = p.NTotal;
                                }

                                MensagemPagina_Comparativo_Produtos.MostraMensagem_Aviso("<b>Aviso:</b> Caso o Desconto dos Itens seja Negativo, seu Total aumenta!<br />O Desconto foi aplicado automaticamente, com o intuito de igualar os Totais Líquidos de Produtos, no entanto é recomendada uma segunda verificação por parte do Usuário!", false);
                            }
                        }
                        else
                            MensagemPagina_Comparativo_Produtos.MostraMensagem_Erro("<b>Erro:</b> Não foi possível encontrar as informações do Sistema selecionado!", false);
                    }
                    else
                        MensagemPagina_Comparativo_Produtos.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar mais de um Produto para convertê-los em um Sistema!", false);
                }
                else
                {
                    MensagemPagina_Comparativo_Produtos.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Sistema para converter os Produtos selecionados no Sistema!", false);
                    Scripts.FocusScript(Page, ddlSistemas_Comparativo.ClientID);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Comparativo_Produtos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de converter os Produtos em um Sistema!<br />Erro ao converter: " + ex.Message, false);
            }

            AtualizaClasses_Comparativo();
        }

        protected void voltar_Click(object sender, EventArgs e)
        {
            DirecionaPagina("App/Paginas/Comercial/Orcamento_Detalhe.aspx?id=" + Request["id"]);
        }

        protected void cmdEmpreitada_Click(object sender, EventArgs e)
        {
            try
            {
                cmdAplicar_Comparativos.Text = "Aplicar Empreitada";
                lbl_modalAplicarComparativos.InnerText = "Deseja aplicar o Orçamento como uma Empreitada?";

                hddComparativos_Empreitada.Value = "true";
                IniciaEmpreitada();
                PopulaServicos_Empreitada();
                RegistraScript();

                div_cmdEmpreitada.Visible = false;
                div_cmdDrawback.Visible = false;
                div_Comparativos_Totais_Empreitada.Visible = true;
                divIncluirServico_Empreitada.Visible = true;
            }
            catch (Exception ex)
            {
                MensagemPagina_Comparativos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de iniciar a aplicação do Orçamento como Empreitada!<br />Erro de Empreitada: " + ex.Message, false);
            }

            AtualizaClasses_Comparativo();
        }

        protected void cmdIncluirServico_Empreitada_Click(object sender, EventArgs e)
        {
            try
            {
                string id = hddidIncluirServico.Value;
                var item = listServicos_Empreitada.FirstOrDefault(s => s.IdItem.ToString().Equals(id));
                var incluirItem = listServicos_Incluir_Empreitada.FirstOrDefault(s => s.IdItem.ToString().Equals(id));

                if (incluirItem != null)
                {
                    if (item == null)
                    {
                        DataSet dsRegras = ConsultaImpostos_Produtos(false, id, hddidEmpresa.Value, hddidCliente.Value, incluirItem.idTipoRegra.ToString(), hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", incluirItem.Preco.ToString());

                        cls_Comercial_Tabelas servico = new cls_Comercial_Tabelas
                        {
                            idRegistro = GerarNovo_idRegistro(listServicos_Empreitada),
                            SFuncao = incluirItem.SFuncao,
                            IdItem = int.Parse(id),
                            nOrdem = listServicos_Empreitada.Where(s => s.bLiberado).OrderBy(s => s.nOrdem).Last().nOrdem + 10,
                            SCodigo = incluirItem.SCodigo,
                            SDscProduto = incluirItem.SDscProduto,
                            TipoProduto = incluirItem.TipoProduto,
                            SUnidade = incluirItem.SUnidade,
                            NQuantidade = 1,
                            NFator = 0,
                            dtInclusao = "15",
                            idTipo = incluirItem.idTipo,
                            IdGrupoProduto = incluirItem.IdGrupoProduto,
                            IdFamiliaProduto = incluirItem.IdFamiliaProduto,
                            sDscGrupoProduto = incluirItem.sDscGrupoProduto,
                            sDscFamiliaProduto = incluirItem.sDscFamiliaProduto,
                            idTipoRegra = incluirItem.idTipoRegra,
                            bLiberado = true,
                            bProjeto = DATASET(dsRegras, "sProjeto").ToUpper().Equals("S"),
                            sNCM = DATASET(dsRegras, "sCodigoFederal"),
                            sCST = DATASET(dsRegras, "sCodigoMunicipal"),
                            NIPI = Math.Round(decimal.Parse(DATASET(dsRegras, "nISS")), 2),
                            nVlr_IPI = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrISS")), 2),
                            NCSSL = Math.Round(decimal.Parse(DATASET(dsRegras, "nCSSL")), 2),
                            nVlr_CSSL = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrCSSL")), 2),
                            NIRPJ = Math.Round(decimal.Parse(DATASET(dsRegras, "nIR")), 2),
                            nVlr_IRPJ = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrIR")), 2),
                            NICMS = Math.Round(decimal.Parse(DATASET(dsRegras, "nINSS")), 2),
                            nVlr_ICMS = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrINSS")), 2)
                        };

                        listServicos_Empreitada.Add(servico);

                        PopulaComposicao(servico, true);
                    }
                    else if (!item.bLiberado)
                    {
                        item.bLiberado = true;
                        item.SFuncao = "INCLUIR ITEM";

                        foreach (var s in listServicos_Composicao_Filhos_Empreitada.Where(s => s.bLiberado && s.idItemPai.Equals(item.idRegistro) && s.idItemAvo.Equals(item.idItemPai) && s.idItemBisavo.Equals(item.idItemAvo)))
                        {
                            s.SFuncao = "INCLUIR ITEM";
                            s.bLiberado = true;
                        }

                        foreach (var s in listServicos_Composicao_Netos_Empreitada.Where(s => s.bLiberado && s.idItemAvo.Equals(item.idItemPai) && s.idItemBisavo.Equals(item.idItemAvo)))
                        {
                            s.SFuncao = "INCLUIR ITEM";
                            s.bLiberado = true;
                        }

                        foreach (var s in listServicos_Composicao_Bisnetos_Empreitada.Where(s => s.bLiberado && s.idItemBisavo.Equals(item.idItemAvo)))
                        {
                            s.SFuncao = "INCLUIR ITEM";
                            s.bLiberado = true;
                        }
                    }
                    else
                        MensagemPaginaServicos_Comparativos.MostraMensagem_Erro("<b>Erro:</b> Não é possível incluir um Serviço já existente na Empreitada!", false);
                }
                else
                    MensagemPaginaServicos_Comparativos.MostraMensagem_Erro("<b>Erro:</b> Não foi possível encontrar as informações do Serviço para que seja incluído na Empreitada!", false);

                hddidIncluirServico.Value = "0";
                txtIncluirServico_Empreitada_Codigo.Text = string.Empty;
                txtIncluirServico_Empreitada_Descricao.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MensagemPaginaServicos_Comparativos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro ao Incluir um Novo Serviço à Empreitada!<br />Erro ao incluir Serviço: " + ex.Message, false);
            }

            AtualizaClasses_Comparativo();

            Scripts.FocusScript(Page, txtIncluirServico_Empreitada_Codigo.ClientID);
            Scripts.Mantem_AbaAtiva(Page, "aba-Comparativo_Servicos");
        }

        protected void cmdVincular_Pedido_Click(object sender, EventArgs e)
        {
            decimal totalProdutos = listProdutos.Where(p => p.bLiberado).Sum(p => p.NTotal - p.nVlr_ST);
            decimal totalServicos = listServicos_Recursos.Where(s => s.bLiberado).Sum(s => s.NTotal);

            DateTime.TryParse(string.IsNullOrEmpty(txtPrevisaoEntrega.Text) ? txtEstimativa_View.Text : txtPrevisaoEntrega.Text, out DateTime dtEstimativaEntrega);

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "INCLUIR PEDIDO" },
                { "@idTipo", "2" },
                { "@idPedido_Vinculado", Request["id"].Trim() },
                { "@idTipoOrcamento", hddidTipoOrcamento.Value },
                { "@idCliente", hddidCliente.Value },
                { "@idFluxo", hddidFluxo.Value },
                { "@idVendedor", hddidVendedor.Value },
                { "@idCondicaoDePagamento", hddidCondicaoPagamento.Value },
                { "@idEmpresa", hddidEmpresa.Value },
                { "@idEnderecoEntrega", hddidEndereco_Entrega.Value },
                { "@idEnderecoDestino", hddidEndereco_Fiscal.Value },
                { "@sReferencia", txtReferencia_View.Text },
                { "@dtPedido", DateTime.Now.ToString("dd/MM/yyyy") },
                { "@dtEstimativaEntrega", dtEstimativaEntrega.ToString("dd/MM/yyyy") },
                { "@nControleTT", txtControle_TT_View.Text },
                { "@sObservacao", txtObs_View.Text },
                { "@idUsuarioInclusao", Variaveis.idUsuario().Trim() },
                { "@idTipoEnvio", hddidFormaEnvio.Value },
                { "@nVlrProdutos", Math.Round(totalProdutos, 2).ToString().Replace(".", "").Replace(',', '.').Trim() },
                { "@nVlrServicos", Math.Round(totalServicos, 2).ToString().Replace(".", "").Replace(',', '.').Trim() },
                { "@sConfidencial", txtConfidencial_View.Text.ToUpper().StartsWith("S") ? "S" : "N" },
                { "@sEmpresaTransporte", txtTransporte_View.Text },
                { "@nFretePrevisto", Math.Round(decimal.Parse(string.IsNullOrEmpty(txtFrete_View.Text) ? "0,00" : txtFrete_View.Text), 2).ToString().Replace(".", "").Replace(',', '.').Trim() },
                { "@idTabelaPreco", hddidTabela.Value },
                { "@sAlteracaoTabelaPreco_Obs", txtTabela_Obs_View.Text },
                { "@sDestinoVenda", hddidDestinoVenda.Value },
                { "@sUF_Entrega", txtUF_Entrega_View.Text.ToUpper() },
                { "@nDiasPrevisao", txtDiasPrevisao_View.Text },
                { "@sUF_Fiscal", txtUF_Fiscal_View.Text.ToUpper() },
                { "@idCidade_Entrega", hddMunicipio_Entrega_SelectedValue.Value },
                { "@nValidadeOrcamento", txtValidade_View.Text.Length > 0 ? txtValidade_View.Text : "5" },
                { "@idContato_Cliente", hddidContato.Value },
                { "@idTipoCliente", hddidTipoCliente.Value },
                { "@sidSegmentosCliente", hddidSegmentos.Value },
                { "@sidTiposServicos", hddidTiposServicos.Value },
                { "@sidEscopos", hddidEscopos.Value },
                { "@sTipoDrawback", hddsTipoDrawback.Value ?? "" },
                { "@idTipoFaturamento", Convert.ToBoolean(hddOcamento_Empreitada.Value) ? "4" : listProdutos.Count > 0 && listServicos_Recursos.Count > 0 ? "3" : listServicos_Recursos.Count > 0 ? "2" : listProdutos.Count > 0 ?  "1" : "0" },
                { "@idInstalador", hddidInstalador.Value },
                { "@idCRM", hddVincula_CRM.Value },
                { "@idTipoMoeda", hddMoeda.Value },
                { "@nCusto_Aduaneiro", txtCusto_Aduaneiro.Text.StringToDecimalString() },
                { "@nCusto_Despachante", txtCusto_Despachante.Text.StringToDecimalString() }
            };
            DataSet ds = ExecutarDataSet(sProcedure, vParametros);

            string idPedido = DATASET(ds, "idPedido");

            try
            {
                if (DATASET(ds, "sErro") != "1")
                {
                    SalvarProdutos(idPedido, false, true);
                    SalvarServicos_Recursos(idPedido, false, true);
                    SalvarEscopos(idPedido);

                    ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@sFuncao", "ATUALIZA_DIFAL_ST" }, { "@idPedido", idPedido } });
                    ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@sFuncao", "GRAVAR_FATURAMENTOS" }, { "@idPedido", idPedido } });
                }
            }
            catch { }

            DirecionaPagina($"/App/Paginas/Pedidos_Detalhe.aspx?id={idPedido}");
        }

        protected void cmdConfirmar_Vinculo_Pedido_Click(object sender, EventArgs e)
        {
            if (ddlVincularPedido.SelectedValue != "0")
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "VINCULAR_PEDIDO" },
                    { "@idPedido", Request["id"].Trim() },
                    { "@idPedido_Vinculado", ddlVincularPedido.SelectedValue }
                };
                ExecutarDataSet(sProcedure, vParametros);

                Scripts.RemoverBackdrop_Modal(Page);
                Scripts.FecharModal(Page, "modalVincularPedido");

                DirecionaPagina($"App/Paginas/Comercial/Orcamento_Detalhe.aspx?id={Request["id"].Trim()}&msg=2");
            }
            else
            {
                MensagemPaginaModalVincula_Pedido.MostraMensagem_Erro("<b>Erro:</b> Para vincular à um Pedido Existente é necessário selecionar o Pedido desejado!", false);
                Scripts.AbrirModal(Page, "modalVincularPedido");

                pn5.Attributes["class"] = "painel";
                div_Paineis.Attributes["style"] = "display: flex; height: 80%; width: 100%; padding: 0 30px 0 30px;";
            }
        }

        protected void cmdExcel_ImportarProdutos_Click(object sender, EventArgs e)
        {
            if (!hddProgresso.Value.Contains("Cliente"))
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Parceiro antes de Importar Produtos!", false);
            else
            {
                try
                {
                    var itens = ExcelImportar.Retornar_Itens_Excel__Orcamento(ImportarArquivo);

                    if (itens != null)
                    {
                        AtualizaClasseGeral();
                        string mensagem = "";

                        foreach (cls_WMS_Produtos item in itens)
                        {
                            DataSet ds = ConsultaImpostos_Produtos(true, item.SCodigo.Trim(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : hddUF_Fiscal_SelectedValue.Value, ddlTabela.SelectedValue);

                            if (ValidarDataSet(ds))
                            {
                                string msg = "";
                                string sNaoExiste = "";

                                try { msg = DATASET(ds, "sMsg"); }
                                catch { }

                                try { sNaoExiste = DATASET(ds, "sNaoExiste"); }
                                catch { }

                                if (!string.IsNullOrEmpty(msg))
                                    mensagem += string.Format("<br /><br />{0}", msg);
                                else
                                {
                                    int.TryParse(DATASET(ds, "idItem"), out int idItem);

                                    var novoProduto = new cls_Comercial_Tabelas
                                    {
                                        idRegistro = GerarNovo_idRegistro(listProdutos),
                                        SFuncao = "INCLUIR ITEM",
                                        SCodigo = item.SCodigo.Trim(),
                                        nOrdem = item.nOrdem > 0 ? item.nOrdem : listProdutos.Any(p => p.bLiberado) ? listProdutos.OrderBy(p => p.nOrdem).Last(p => p.bLiberado).nOrdem + 10 : 10,
                                        IdItem = 0,

                                        NQuantidade = item.NQuantidade > 0 ? item.NQuantidade : 1,

                                        bLiberado = true,
                                        dtInclusao = txtPrazoProduto.Text
                                    };

                                    if (string.IsNullOrEmpty(sNaoExiste))
                                    {
                                        string ncm = DATASET(ds, 1, 0, "sCodigoNCM") == "0" ? "Não Cadastrado" : DATASET(ds, 1, 0, "sCodigoNCM");
                                        string cest = DATASET(ds, 1, 0, "sCodigoCEST") == "0" ? "Não Cadastrado" : DATASET(ds, 1, 0, "sCodigoCEST");
                                        int.TryParse(DATASET(ds, 1, 0, "idRegra"), out int idRegra);
                                        decimal.TryParse(DATASET(ds, 1, 0, "nMVA"), out decimal MVA);
                                        decimal.TryParse(DATASET(ds, 1, 0, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                                        try
                                        {
                                            MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(ds, 1, 0, "sICMSST").Equals("S"), ncm, cest, decimal.Parse(DATASET(ds, 1, 0, "nICMS")), DATASET(ds, 1, 0, "dtUltimaConsulta"), DATASET(ds, 2, 0, "sMensagem_Erro_LegisWeb"));
                                        }
                                        catch { }

                                        novoProduto = new cls_Comercial_Tabelas
                                        {
                                            idRegistro = GerarNovo_idRegistro(listProdutos),
                                            SFuncao = "INCLUIR ITEM",
                                            nOrdem = item.nOrdem > 0 ? item.nOrdem : listProdutos.Any(p => p.bLiberado) ? listProdutos.OrderBy(p => p.nOrdem).Last(p => p.bLiberado).nOrdem + 10 : 10,
                                            IdItem = idItem,
                                            SCodigo = item.SCodigo.Trim(),
                                            SDscProduto = DATASET(ds, "sDscProduto"),
                                            sNCM = DATASET(ds, "sCodigoNCM").Equals("0") ? "Não Cadastrado" : DATASET(ds, "sCodigoNCM"),
                                            TipoProduto = DATASET(ds, "sTipoProduto"),
                                            IdGrupoProduto = int.Parse(DATASET(ds, "idGrupo")),
                                            IdFamiliaProduto = int.Parse(DATASET(ds, "idFamilia")),
                                            sDscGrupoProduto = DATASET(ds, "sDscGrupo"),
                                            sDscFamiliaProduto = DATASET(ds, "sDscFamilia"),
                                            SUnidade = DATASET(ds, "sUnidade"),
                                            sIndustrializado = DATASET(ds, 1, 0, "sProdutoIndustrializado") == "S" ? "Sim" : "Não",
                                            sOrigem = DATASET(ds, 1, 0, "sProdutoImportado") == "S" ? "IMP" : "BR",
                                            idRegra = idRegra,

                                            sCFOP = DATASET(ds, 1, 0, "sCFOP"),
                                            sCST = DATASET(ds, 1, 0, "CST"),

                                            NIPI = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nIPI")), 2),
                                            NPIS = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nPIS")), 2),
                                            NCOFINS = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nCOFINS")), 2),
                                            NICMS = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nICMS")), 2),
                                            NST = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nICMSST")), 2),

                                            bBaseCalcICMS_com_IPI = DATASET(ds, 1, 0, "nBaseCalculoICMS").Equals("PI"),

                                            NQuantidade = item.NQuantidade > 0 ? item.NQuantidade : 1,
                                            nPesoBruto = Math.Round(decimal.Parse(DATASET(ds, "nPesoBruto")), 2),
                                            nPesoLiquido = Math.Round(decimal.Parse(DATASET(ds, "nPesoNeto")), 2),
                                            nVolume = Math.Round(decimal.Parse(DATASET(ds, "nVolume")), 2),
                                            NFator = Math.Round(decimal.Parse(string.IsNullOrEmpty(txtDescontoProduto.Text) ? "0" : txtDescontoProduto.Text), 4),
                                            Preco = Math.Round(decimal.Parse(DATASET(ds, "nTotal")), 2),

                                            bLiberado = true,
                                            dtInclusao = txtPrazoProduto.Text
                                        };

                                        novoProduto.nUnitario = Math.Round(novoProduto.Preco - (novoProduto.Preco * (novoProduto.NFator / 100)), 2);
                                        novoProduto.NTotal = Math.Round(novoProduto.nUnitario * novoProduto.NQuantidade, 2);

                                        if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                                        {
                                            novoProduto.NII = 0;
                                            novoProduto.NIPI = 0;
                                            novoProduto.NPIS = 0;
                                            novoProduto.NCOFINS = 0;
                                            novoProduto.NICMS = 0;
                                            novoProduto.NDIFAL = 0;
                                            novoProduto.NST = 0;

                                            novoProduto.NTotal *= cambio;
                                        }

                                        try
                                        {
                                            RecalculaImpostos(novoProduto);
                                        }
                                        catch { }

                                        // Cálculo ST
                                        if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !novoProduto.bSistema && (ddlMoeda.SelectedValue == "0" || ddlMoeda.SelectedValue == "2"))
                                        {
                                            try
                                            {
                                                if (MVA > 0)
                                                {
                                                    var st = CalculaValor_ST(novoProduto.NTotal, novoProduto.nVlr_ICMS * novoProduto.NQuantidade, MVA, ICMS_interno_destino);
                                                    novoProduto.nVlr_ST = st.Item1;
                                                    novoProduto.NST = st.Item2;
                                                }
                                            }
                                            catch { }
                                        }
                                    }

                                    listProdutos.Add(novoProduto);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(mensagem))
                            MensagemPagina_Modal_ImportarProdutos_Aviso.MostraMensagem_Aviso(string.Format("<b>Aviso:</b><br />- Alguns Produtos não foram Importados corretamente!<br />{0}", mensagem), false);
                        else
                            MensagemPagina_Modal_ImportarProdutos.MostraMensagem_Sucesso("Todos os Produtos foram Importados com sucesso!", false);
                    }
                    else
                        MensagemPagina_Modal_ImportarProdutos.MostraMensagem_Sucesso("Não foram encontrados Itens no Arquivo selecionado!", false);
                }
                catch (Exception ex)
                {
                    ex.Source = ex.Message;

                    if (ex.Message.Contains("header signature"))
                        ex.Source = "<br />- Não foi possível ler o arquivo selecionado, é possível que este esteja corrompido!<br />- Por favor passe as informações dos Itens, para um outro Arquivo, de preferência um Novo Arquivo Excel em branco, e tente novamente!";

                    MensagemPagina_Modal_ImportarProdutos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro ao Importar os Produtos do Arquivo Excel!<br /><b>Erro ao Importar Excel: </b>" + ex.Source, false);
                }

                Scripts.RemoverBackdrop_Modal(Page);
                Scripts.AbrirModal(Page, "modalUpload_ImportarExcel");
            }

            MantemEtapa_Pos_PostBack(4);
            AtualizaBarraProgresso(4, false);
        }

        protected void cmdPedidos_ImportarProdutos_Click(object sender, EventArgs e)
        {
            if (!hddProgresso.Value.Contains("Cliente"))
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Parceiro antes de Importar Produtos!", false);
            else
            {
                if (ddlPedidos_ImportarProdutos.SelectedValue == "0")
                {
                    MensagemPagina_Modal_ImportarProdutos_Pedidos.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Pedido para Importar os Itens!", false);

                    Scripts.FocusScript(Page, ddlPedidos_ImportarProdutos.ClientID);
                }
                else
                {
                    try
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTA PEDIDO" },
                            { "@idPedido", ddlPedidos_ImportarProdutos.SelectedValue }
                        };

                        ImportarItens(ExecutarDataSet(sProcedure, vParametros).Tables[1]);

                        MensagemPagina_Modal_ImportarProdutos_Pedidos.MostraMensagem_Sucesso("Todos os Itens foram Importados com sucesso!", false);
                    }
                    catch (Exception ex)
                    {
                        MensagemPagina_Modal_ImportarProdutos_Pedidos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de Importar os Itens do Pedido selecionado!<br />Erro ao Importar Pedidos: " + ex.Message, false);
                    }
                }

                MantemEtapa_Pos_PostBack(4);
                AtualizaBarraProgresso(4, false);

                Scripts.RemoverBackdrop_Modal(Page);
                Scripts.AbrirModal(Page, "modalUpload_ImportarExcel");
                Scripts.Mantem_AbaAtiva(Page, "aba-Pedidos");
            }
        }

        protected void cmdLM_ImportarProdutos_Click(object sender, EventArgs e)
        {
            if (!hddProgresso.Value.Contains("Cliente"))
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Parceiro antes de Importar Produtos!", false);
            else
            {
                if (ddlSelecionaPedido_LM_ImportarProdutos.SelectedValue == "0" || ddlLM_ImportarProdutos.SelectedValue == "0")
                {
                    MensagemPagina_Modal_ImportarProdutos_LM.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar uma LM, do Pedido selecionado, para Importar os Itens!", false);

                    Scripts.FocusScript(Page, ddlLM_ImportarProdutos.ClientID);
                }
                else
                {
                    try
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTAR_DETALHE" },
                            { "@idLM", ddlLM_ImportarProdutos.SelectedValue }
                        };

                        ImportarItens(ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos_LM", vParametros).Tables[1]);

                        MensagemPagina_Modal_ImportarProdutos_LM.MostraMensagem_Sucesso("Todos os Itens foram Importados com sucesso!", false);
                    }
                    catch (Exception ex)
                    {
                        MensagemPagina_Modal_ImportarProdutos_LM.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de Importar os Itens da Lista de Materiais selecionada!<br />Erro ao Importar LM: " + ex.Message, false);
                    }
                }

                MantemEtapa_Pos_PostBack(4);
                AtualizaBarraProgresso(4, false);

                Scripts.RemoverBackdrop_Modal(Page);
                Scripts.AbrirModal(Page, "modalUpload_ImportarExcel");
                Scripts.Mantem_AbaAtiva(Page, "aba-LM");
            }
        }

        protected void cmdOrcamento_ImportarProdutos_Click(object sender, EventArgs e)
        {
            if (!hddProgresso.Value.Contains("Cliente"))
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Parceiro antes de Importar Produtos!", false);
            else
            {
                if (ddlOrcamento_ImportarProdutos.SelectedValue == "0")
                {
                    MensagemPagina_Modal_ImportarProdutos_Orcamento.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Orçamento para Importar os Itens!", false);

                    Scripts.FocusScript(Page, ddlOrcamento_ImportarProdutos.ClientID);
                }
                else
                {
                    try
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTA PEDIDO" },
                            { "@idPedido", ddlOrcamento_ImportarProdutos.SelectedValue }
                        };

                        ImportarItens(ExecutarDataSet(sProcedure, vParametros).Tables[1]);

                        MensagemPagina_Modal_ImportarProdutos_Orcamento.MostraMensagem_Sucesso("Todos os Itens foram Importados com sucesso!", false);
                    }
                    catch (Exception ex)
                    {
                        MensagemPagina_Modal_ImportarProdutos_Orcamento.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de Importar os Itens do Orçamento selecionado!<br />Erro ao Importar Orçamento: " + ex.Message, false);
                    }
                }

                MantemEtapa_Pos_PostBack(4);
                AtualizaBarraProgresso(4, false);

                Scripts.RemoverBackdrop_Modal(Page);
                Scripts.AbrirModal(Page, "modalUpload_ImportarExcel");
                Scripts.Mantem_AbaAtiva(Page, "aba-Orcamentos");
            }
        }

        protected void cmdTabelaPreco_ImportarProdutos_Click(object sender, EventArgs e)
        {
            if (!hddProgresso.Value.Contains("Cliente"))
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Parceiro antes de Importar Produtos!", false);
            else
            {
                try
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_ITENS_DISPONIVEIS" },
                        { "@idTabela", ddlTabela.SelectedValue }
                    };

                    ImportarItens(ExecutarDataTable(sProcedure_TabelaPreco, vParametros));

                    MensagemPagina_Fixa_Modal_ImportarProdutos_TabelaPreco.MostraMensagem_Sucesso("Todos os Itens foram Importados com sucesso!", false);
                }
                catch (Exception ex)
                {
                    MensagemPagina_Fixa_Modal_ImportarProdutos_TabelaPreco.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de Importar os Itens da Tabela de Preços selecionada!<br />Erro ao Importar Tabela de Preço: " + ex.Message, false);
                }

                MantemEtapa_Pos_PostBack(4);
                AtualizaBarraProgresso(4, false);

                Scripts.RemoverBackdrop_Modal(Page);
                Scripts.AbrirModal(Page, "modalUpload_ImportarExcel");
                Scripts.Mantem_AbaAtiva(Page, "aba-TabelaPreco");
            }
        }

        protected void cmdVincularProduto_Click(object sender, EventArgs e)
        {
            Scripts.RemoverBackdrop_Modal(Page);

            try
            {
                AtualizaClasseGeral();

                if (ddlVincularProduto.SelectedValue != "0")
                {
                    DataSet ds = ConsultaImpostos_Produtos(true, ddlVincularProduto.SelectedValue, ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : hddUF_Fiscal_SelectedValue.Value, ddlTabela.SelectedValue);

                    if (ValidarDataSet(ds))
                    {
                        var produtoExistente = listProdutos.FirstOrDefault(p => p.SCodigo.Trim().ToLower().Equals(hddVincularProduto.Value.Split('|')[0].ToLower().Trim()));

                        if (produtoExistente != null)
                        {
                            produtoExistente.SFuncao = "INCLUIR ITEM";
                            produtoExistente.IdItem = int.Parse(ddlVincularProduto.SelectedValue);
                            produtoExistente.SCodigo = DATASET(ds, "sCodigo");
                            produtoExistente.SDscProduto = DATASET(ds, "sDscProduto");
                            produtoExistente.sNCM = DATASET(ds, "sCodigoNCM") == "0" ? "Não Cadastrado" : DATASET(ds, "sCodigoNCM");
                            produtoExistente.TipoProduto = DATASET(ds, "sTipoProduto");
                            produtoExistente.IdGrupoProduto = int.Parse(DATASET(ds, "idGrupo"));
                            produtoExistente.IdFamiliaProduto = int.Parse(DATASET(ds, "idFamilia"));
                            produtoExistente.sDscGrupoProduto = DATASET(ds, "sDscGrupo");
                            produtoExistente.sDscFamiliaProduto = DATASET(ds, "sDscFamilia");
                            produtoExistente.SUnidade = DATASET(ds, "sUnidade");
                            produtoExistente.sIndustrializado = DATASET(ds, 1, 0, "sProdutoIndustrializado") == "S" ? "Sim" : "Não";
                            produtoExistente.sOrigem = DATASET(ds, 1, 0, "sProdutoImportado") == "S" ? "IMP" : "BR";
                            produtoExistente.sCFOP = DATASET(ds, 1, 0, "sCFOP");
                            produtoExistente.sCST = DATASET(ds, 1, 0, "CST");
                            produtoExistente.NIPI = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nIPI")), 2);
                            produtoExistente.NPIS = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nPIS")), 2);
                            produtoExistente.NCOFINS = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nCOFINS")), 2);
                            produtoExistente.NICMS = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nICMS")), 2);
                            produtoExistente.NDIFAL = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nDIFAL")), 2);
                            produtoExistente.NST = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nICMSST")), 2);
                            produtoExistente.bBaseCalcICMS_com_IPI = DATASET(ds, 1, 0, "nBaseCalculoICMS").Equals("PI");
                            produtoExistente.nPesoBruto = Math.Round(decimal.Parse(DATASET(ds, "nPesoBruto")), 2);
                            produtoExistente.nPesoLiquido = Math.Round(decimal.Parse(DATASET(ds, "nPesoNeto")), 2);
                            produtoExistente.nVolume = Math.Round(decimal.Parse(DATASET(ds, "nVolume")), 2);
                            produtoExistente.NFator = Math.Round(decimal.Parse(string.IsNullOrEmpty(txtDescontoProduto.Text) ? "0" : txtDescontoProduto.Text), 4);
                            produtoExistente.Preco = Math.Round(decimal.Parse(DATASET(ds, "nTotal")), 2);
                            produtoExistente.nUnitario = Math.Round(produtoExistente.Preco - (produtoExistente.Preco * (produtoExistente.NFator / 100)), 2);
                            produtoExistente.NTotal = Math.Round(produtoExistente.nUnitario * produtoExistente.NQuantidade, 2);

                            if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                            {
                                produtoExistente.NII = 0;
                                produtoExistente.NIPI = 0;
                                produtoExistente.NPIS = 0;
                                produtoExistente.NCOFINS = 0;
                                produtoExistente.NICMS = 0;
                                produtoExistente.NDIFAL = 0;
                                produtoExistente.NST = 0;

                                produtoExistente.NTotal *= cambio;
                            }

                            try
                            {
                                RecalculaImpostos(produtoExistente);
                            }
                            catch { }

                            // Cálculo ST
                            if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !produtoExistente.bSistema && (ddlMoeda.SelectedValue == "0" || ddlMoeda.SelectedValue == "2"))
                            {
                                DataSet dsRegras = ConsultaImpostos_Produtos(false, produtoExistente.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", produtoExistente.Preco.ToString().Replace(',', '.'));

                                decimal.TryParse(DATASET(dsRegras, "nMVA"), out decimal MVA);
                                decimal.TryParse(DATASET(dsRegras, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                                try
                                {
                                    MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(dsRegras, "sICMSST").Equals("S"), produtoExistente.sNCM, produtoExistente.sCEST, decimal.Parse(DATASET(dsRegras, "nICMS")), DATASET(dsRegras, "dtUltimaConsulta"), DATASET(dsRegras, 1, 0, "sMensagem_Erro_LegisWeb"));

                                    if (MVA > 0)
                                    {
                                        var st = CalculaValor_ST(produtoExistente.NTotal, produtoExistente.nVlr_ICMS * produtoExistente.NQuantidade, MVA, ICMS_interno_destino);
                                        produtoExistente.nVlr_ST = st.Item1;
                                        produtoExistente.NST = st.Item2;
                                    }
                                }
                                catch { }
                            }

                            produtoExistente.bLiberado = true;
                            produtoExistente.dtInclusao = txtPrazoProduto.Text;
                        }
                    }

                    MensagemPaginaProdutos.MostraMensagem_Sucesso("Produto Vinculado com sucesso!", false);
                    Scripts.FecharModal(Page, "modalVincularProdutos");
                }
                else
                {
                    MensagemPagina_Modal_VincularProdutos.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Produto para Vincular!", false);
                    Scripts.AbrirModal(Page, "modalVincularProdutos");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Modal_VincularProdutos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de Vincular Produtos!<br />Erro ao Vincular Produtos: " + ex.Message, false);
                Scripts.AbrirModal(Page, "modalVincularProdutos");
            }

            MantemEtapa_Pos_PostBack(4);
            AtualizaBarraProgresso(4, false);
        }

        protected void cmdDuplicarSistema_Click(object sender, EventArgs e)
        {
            AtualizaClasses_Comparativo();

            try
            {
                LinkButton lnk = sender as LinkButton;

                if (lnk.CommandName.Equals("Duplicar"))
                {
                    int.TryParse(lnk.CommandArgument, out int idRegistro);
                    var sistema = listProdutos_Comparativos.FirstOrDefault(p => p.bSistema && p.idRegistro.Equals(idRegistro));

                    if (sistema != null)
                    {
                        var sistemaDuplicado = new cls_Comercial_Tabelas
                        {
                            idRegistro = GerarNovo_idRegistro(listProdutos_Comparativos),
                            SFuncao = "INCLUIR ITEM",
                            IdItem = sistema.IdItem,
                            nOrdem = sistema.nOrdem,
                            SCodigo = sistema.SCodigo,
                            SDscProduto = sistema.SDscProduto,
                            sNCM = sistema.sNCM,
                            TipoProduto = sistema.TipoProduto,
                            IdGrupoProduto = sistema.IdGrupoProduto,
                            IdFamiliaProduto = sistema.IdFamiliaProduto,
                            sDscGrupoProduto = sistema.sDscGrupoProduto,
                            sDscFamiliaProduto = sistema.sDscFamiliaProduto,
                            SUnidade = sistema.SUnidade,
                            sIndustrializado = sistema.sIndustrializado,
                            sOrigem = sistema.sOrigem,

                            sCFOP = sistema.sCFOP,
                            sCST = sistema.sCST,

                            NIPI = sistema.NIPI,
                            NPIS = sistema.NPIS,
                            NCOFINS = sistema.NCOFINS,
                            NICMS = sistema.NICMS,
                            NST = sistema.NST,

                            nBaseCalc_ICMS = sistema.nBaseCalc_ICMS,
                            nVlr_IPI = sistema.nVlr_IPI,
                            nVlr_PIS = sistema.nVlr_PIS,
                            nVlr_COFINS = sistema.nVlr_COFINS,
                            nVlr_ICMS = sistema.nVlr_ICMS,
                            nVlr_DIFAL = sistema.nVlr_DIFAL,
                            nVlr_ST = sistema.nVlr_ST,
                            nVlr_Liquido = sistema.nVlr_Liquido,

                            NQuantidade = sistema.NQuantidade,
                            nPesoBruto = sistema.nPesoBruto,
                            nPesoLiquido = sistema.nPesoLiquido,
                            nVolume = sistema.nVolume,

                            NFator = sistema.NFator,
                            Preco = sistema.Preco,
                            nUnitario = sistema.nUnitario,
                            NTotal = sistema.NTotal,
                            bSistema = true,
                            bLiberado = true,
                            dtInclusao = sistema.dtInclusao
                        };

                        listProdutos_Comparativos.Add(sistemaDuplicado);

                        var novosItens = new List<cls_Comercial_Tabelas>();

                        foreach (var p in listProdutos_Comparativos_Composicao)
                        {
                            if (!p.idItemPai.Equals(idRegistro))
                                continue;

                            novosItens.Add(new cls_Comercial_Tabelas
                            {
                                idRegistro = GerarNovo_idRegistro(listProdutos_Comparativos_Composicao),
                                SFuncao = "INCLUIR ITEM",
                                IdItem = p.IdItem,
                                idItemPai = sistemaDuplicado.idRegistro,
                                nOrdem = p.nOrdem,
                                SCodigo = p.SCodigo,
                                SDscProduto = p.SDscProduto,
                                sNCM = p.sNCM,
                                TipoProduto = p.TipoProduto,
                                IdGrupoProduto = p.IdGrupoProduto,
                                IdFamiliaProduto = p.IdFamiliaProduto,
                                sDscGrupoProduto = p.sDscGrupoProduto,
                                sDscFamiliaProduto = p.sDscFamiliaProduto,
                                SUnidade = p.SUnidade,
                                sIndustrializado = p.sIndustrializado,
                                sOrigem = p.sOrigem,

                                sCFOP = p.sCFOP,
                                sCST = p.sCST,

                                NIPI = p.NIPI,
                                NPIS = p.NPIS,
                                NCOFINS = p.NCOFINS,
                                NICMS = p.NICMS,
                                NST = p.NST,

                                nBaseCalc_ICMS = p.nBaseCalc_ICMS,
                                nVlr_IPI = p.nVlr_IPI,
                                nVlr_PIS = p.nVlr_PIS,
                                nVlr_COFINS = p.nVlr_COFINS,
                                nVlr_ICMS = p.nVlr_ICMS,
                                nVlr_DIFAL = p.nVlr_DIFAL,
                                nVlr_ST = p.nVlr_ST,
                                nVlr_Liquido = p.nVlr_Liquido,

                                NQuantidade = p.NQuantidade,
                                nPesoBruto = p.nPesoBruto,
                                nPesoLiquido = p.nPesoLiquido,
                                nVolume = p.nVolume,

                                NFator = p.NFator,
                                Preco = p.Preco,
                                nUnitario = p.nUnitario,
                                NTotal = p.NTotal,
                                bLiberado = true,
                                dtInclusao = p.dtInclusao
                            });
                        }

                        listProdutos_Comparativos_Composicao.AddRange(novosItens);

                    }
                    else
                        throw new Exception("Sistema não encontrado!");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Comparativo_Produtos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de Duplicar o Sistema!<br />Erro ao Duplicar: " + ex.Message, true);
            }

            AtualizaClasses_Comparativo();
        }

        protected void cmdVincularTodosProdutos_Click(object sender, EventArgs e)
        {
            Scripts.RemoverBackdrop_Modal(Page);

            try
            {
                AtualizaClasseGeral();

                int.TryParse(ddlVincularTodosProdutos.SelectedValue, out int idItem);

                if (idItem > 0)
                {
                    foreach (List_Item item in lstProdutos_NaoCadastrados.Items)
                    {
                        if (item.Selected)
                        {
                            var produtoExistente = listProdutos.FirstOrDefault(p => p.idRegistro.ToString().Equals(item.Value.Trim()));

                            if (produtoExistente != null)
                            {
                                DataSet ds = ConsultaImpostos_Produtos(true, idItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : hddUF_Fiscal_SelectedValue.Value, ddlTabela.SelectedValue);

                                if (ValidarDataSet(ds))
                                {
                                    produtoExistente.SFuncao = "INCLUIR ITEM";
                                    produtoExistente.IdItem = idItem;
                                    produtoExistente.SCodigo = DATASET(ds, "sCodigo");
                                    produtoExistente.SDscProduto = DATASET(ds, "sDscProduto");
                                    produtoExistente.sNCM = DATASET(ds, "sCodigoNCM") == "0" ? "Não Cadastrado" : DATASET(ds, "sCodigoNCM");
                                    produtoExistente.TipoProduto = DATASET(ds, "sTipoProduto");
                                    produtoExistente.IdGrupoProduto = int.Parse(DATASET(ds, "idGrupo"));
                                    produtoExistente.IdFamiliaProduto = int.Parse(DATASET(ds, "idFamilia"));
                                    produtoExistente.sDscGrupoProduto = DATASET(ds, "sDscGrupo");
                                    produtoExistente.sDscFamiliaProduto = DATASET(ds, "sDscFamilia");
                                    produtoExistente.SUnidade = DATASET(ds, "sUnidade");
                                    produtoExistente.sIndustrializado = DATASET(ds, 1, 0, "sProdutoIndustrializado") == "S" ? "Sim" : "Não";
                                    produtoExistente.sOrigem = DATASET(ds, 1, 0, "sProdutoImportado") == "S" ? "IMP" : "BR";
                                    produtoExistente.sCFOP = DATASET(ds, 1, 0, "sCFOP");
                                    produtoExistente.sCST = DATASET(ds, 1, 0, "CST");
                                    produtoExistente.NIPI = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nIPI")), 2);
                                    produtoExistente.NPIS = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nPIS")), 2);
                                    produtoExistente.NCOFINS = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nCOFINS")), 2);
                                    produtoExistente.NICMS = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nICMS")), 2);
                                    produtoExistente.NST = Math.Round(decimal.Parse(DATASET(ds, 1, 0, "nICMSST")), 2);
                                    produtoExistente.bBaseCalcICMS_com_IPI = DATASET(ds, 1, 0, "nBaseCalculoICMS").Equals("PI");
                                    produtoExistente.nPesoBruto = Math.Round(decimal.Parse(DATASET(ds, "nPesoBruto")), 2);
                                    produtoExistente.nPesoLiquido = Math.Round(decimal.Parse(DATASET(ds, "nPesoNeto")), 2);
                                    produtoExistente.nVolume = Math.Round(decimal.Parse(DATASET(ds, "nVolume")), 2);
                                    produtoExistente.NFator = Math.Round(decimal.Parse(string.IsNullOrEmpty(txtDescontoProduto.Text) ? "0" : txtDescontoProduto.Text), 4);
                                    produtoExistente.Preco = Math.Round(decimal.Parse(DATASET(ds, "nTotal")), 2);
                                    produtoExistente.nUnitario = Math.Round(produtoExistente.Preco - (produtoExistente.Preco * (produtoExistente.NFator / 100)), 2);
                                    produtoExistente.NTotal = Math.Round(produtoExistente.nUnitario * produtoExistente.NQuantidade, 2);

                                    if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                                    {
                                        produtoExistente.NII = 0;
                                        produtoExistente.NIPI = 0;
                                        produtoExistente.NPIS = 0;
                                        produtoExistente.NCOFINS = 0;
                                        produtoExistente.NICMS = 0;
                                        produtoExistente.NDIFAL = 0;
                                        produtoExistente.NST = 0;

                                        produtoExistente.NTotal *= cambio;
                                    }

                                    try
                                    {
                                        RecalculaImpostos(produtoExistente);
                                    }
                                    catch { }

                                    // Cálculo ST
                                    if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !produtoExistente.bSistema && (ddlMoeda.SelectedValue == "0" || ddlMoeda.SelectedValue == "2"))
                                    {
                                        DataSet dsRegras = ConsultaImpostos_Produtos(false, produtoExistente.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", produtoExistente.Preco.ToString().Replace(',', '.'));

                                        decimal.TryParse(DATASET(dsRegras, "nMVA"), out decimal MVA);
                                        decimal.TryParse(DATASET(dsRegras, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                                        try
                                        {
                                            MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(dsRegras, "sICMSST").Equals("S"), produtoExistente.sNCM, produtoExistente.sCEST, decimal.Parse(DATASET(dsRegras, "nICMS")), DATASET(dsRegras, "dtUltimaConsulta"), DATASET(dsRegras, 1, 0, "sMensagem_Erro_LegisWeb"));

                                            if (MVA > 0)
                                            {
                                                var st = CalculaValor_ST(produtoExistente.NTotal, produtoExistente.nVlr_ICMS * produtoExistente.NQuantidade, MVA, ICMS_interno_destino);
                                                produtoExistente.nVlr_ST = st.Item1;
                                                produtoExistente.NST = st.Item2;
                                            }
                                        }
                                        catch { }
                                    }

                                    produtoExistente.bLiberado = true;
                                    produtoExistente.dtInclusao = txtPrazoProduto.Text;
                                }
                            }
                        }
                    }

                    MensagemPaginaProdutos.MostraMensagem_Sucesso("Produtos Vinculados com sucesso!", false);
                    Scripts.FecharModal(Page, "modalVincularTodosProdutos");
                }
                else
                {
                    MensagemPagina_Modal_VincularTodosProdutos.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Produto para Vincular!", false);
                    Scripts.AbrirModal(Page, "modalVincularTodosProdutos");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Modal_VincularTodosProdutos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de Vincular Todos os Produtos!<br />Erro ao Vincular Todos os Produtos: " + ex.Message, false);
                Scripts.AbrirModal(Page, "modalVincularTodosProdutos");
            }

            MantemEtapa_Pos_PostBack(4);
            AtualizaBarraProgresso(4, true);
        }

        protected void cmdRecarregaServicos_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (List_Item item in lstTipoServicos_TipoOrcamento.Items)
                {
                    if (item.Selected)
                        hddidTiposServicos.Value += item.Value + "|";
                }

                foreach (List_Item item in lstEscopos_TipoOrcamento.Items)
                {
                    if (item.Selected)
                        hddidEscopos.Value += item.Value + "|";
                }
            }
            catch { }

            Popula_gvServicos_Recursos(ddlTipoOrcamento.SelectedValue, true);
            Popula_gvCheckList(ddlTipoOrcamento.SelectedValue, true);

            div_recarregaServicos.Visible = false;

            MantemEtapa_Pos_PostBack(3);
            AtualizaBarraProgresso(3, false);
        }

        protected void cmdDrawback_Click(object sender, EventArgs e)
        {
            string id = ((Button)sender).ID;

            var lista = hddComparativoProdutos.Value.Equals("S") ? listProdutos_Comparativos : listProdutos;

            foreach (var p in lista)
            {
                if (!p.bLiberado)
                    continue;

                p.Preco = Math.Round(p.Preco - p.nVlr_IPI - p.nVlr_PIS - p.nVlr_COFINS - p.nVlr_DIFAL, 2);

                if (id.Contains("Parcial"))
                    hddsTipoDrawback.Value = "P";
                else if (id.Contains("ZonaFranca"))
                    hddsTipoDrawback.Value = "Z";
                else if (id.Contains("Integral"))
                {
                    p.Preco = Math.Round(p.Preco - p.nVlr_ICMS, 2);

                    p.NICMS = 0.00m;
                    p.nVlr_ICMS = 0.00m;

                    hddsTipoDrawback.Value = "I";
                }

                p.NIPI = 0.00m;
                p.nVlr_IPI = 0.00m;
                p.NPIS = 0.00m;
                p.nVlr_PIS = 0.00m;
                p.NCOFINS = 0.00m;
                p.nVlr_COFINS = 0.00m;
                p.NST = 0.00m;
                p.nVlr_ST = 0.00m;
                p.NDIFAL = 0.00m;
                p.nVlr_DIFAL = 0.00m;

                p.NTotal = Math.Round(p.Preco * p.NQuantidade - (p.Preco * p.NQuantidade * (p.NFator / 100)), 2);
            }

            div_cmdEmpreitada.Visible = false;
            div_cmdDrawback.Visible = false;

            hddsDrawback.Value = "true";

            AtualizaClasses_Comparativo();
        }

        protected void cmdNovaCondicaoPagamento_Salvar_Click(object sender, EventArgs e)
        {
            string condPgto = ddlNovaCondicaoPagamento.SelectedValue;

            if (condPgto == "-1" || condPgto == "0")
                MensagemPagina_Modal_NovaCondicaoPagamento.MostraMensagem_Erro("É necessário selecionar uma Condição de Pagamento!", false);
            else
            {
                hddCondPgto_Alterada.Value = "true";
                div_NovaCondPgto_Motivo.Visible = true;
                div_NovaCondPgto_Motivo_View.Visible = true;

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "INCLUIR_PERMISSOES_PAGAMENTO" },
                    { "@idParceiro", hddidCliente.Value },
                    { "@idCondicaoPagamento", condPgto },
                    { "@idTipoCondicoesdePagamento", "2" }
                };
                ExecutarDataSet(sProcedure_PedidosCotacao__CondPagamento_Parceiro, vParametros);

                Popula_Combo(ddlCondicaoPagamento, $"sp_Select 'FLOW_CondicaoDePagamento', @idPesquisa={hddidCliente.Value}, @idPais={Request["id"]}, @idFiltro=2", "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione", "-1");
                ddlCondicaoPagamento.SelectedValue = condPgto;
                txtPagamento_View.Text = ddlCondicaoPagamento.SelectedItem.Text;
                ddlNovaCondicaoPagamento.SelectedValue = "-1";

                MensagemPagina_Modal_NovaCondicaoPagamento.MostraMensagem_Sucesso("Condição de Pagamento registrada com sucesso!", false);
            }

            Scripts.RemoverBackdrop_Modal(Page);
            Scripts.AbrirModal(Page, "modal_novaCondicaoPagamento");
            AtualizaBarraProgresso(2, true);
            MantemEtapa_Pos_PostBack(2);
        }

        protected void cmdNovaCondicaoPagamento_Personalizada_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDescricao_Nova_CondicaoPagamento.Text))
                MensagemPagina_Modal_NovaCondicaoPagamento.MostraMensagem_Erro("É necessário preencher o campo de Descrição!", false);
            else if (!int.TryParse(txtQtdParcelas_Nova_CondicaoPagamento.Text, out int qtd) || qtd <= 0)
                MensagemPagina_Modal_NovaCondicaoPagamento.MostraMensagem_Erro("É necessário incluir ao menos 1 Parcela!", false);
            else if (string.IsNullOrEmpty(hddNovaCondPgto_Pers_Dados.Value))
                MensagemPagina_Modal_NovaCondicaoPagamento.MostraMensagem_Erro("É necessário incluir ao menos 1 Parcela!", false);
            else
            {
                try
                {
                    Dictionary<string, string> vParam = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@sDscCondicaoPagamento", txtDescricao_Nova_CondicaoPagamento.Text },
                        { "@nQtdParcelas", txtQtdParcelas_Nova_CondicaoPagamento.Text },
                        { "@sSituacao", "S" },
                        { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                        { "@idPedido", Request["id"] }
                    };
                    DataSet ds = ExecutarDataSet(sProcedure_CondPagamento, vParam);

                    if (DATASET(ds, "RET").Equals("0"))
                    {
                        string idCond = DATASET(ds, "idCondicaoPagamento");
                        var parcelas = JsonConvert.DeserializeObject<List<NovaCondicao_Pers>>(hddNovaCondPgto_Pers_Dados.Value);

                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "SALVAR_CONDICAOPAGAMENTO" },
                            { "@idCondicaoPagamento", idCond },
                            { "@idTipoCondicaoPagamento", "" },
                            { "@nPorcentagemValor", "" },
                            { "@nDDL", "" }
                        };

                        foreach (var parcela in parcelas)
                        {
                            vParametros["@idTipoCondicaoPagamento"] = parcela.tipo;
                            vParametros["@nPorcentagemValor"] = parcela.porcentagem.Replace("%", "").Replace(",", ".");
                            vParametros["@nDDL"] = parcela.ddl;
                            ExecutarDataSet(sProcedure_CondPagamento, vParametros);
                        }

                        Popula_Combo(ddlCondicaoPagamento, $"sp_Select 'FLOW_CondicaoDePagamento', @idPesquisa={hddidCliente.Value}, @idPais={Request["id"]}, @idFiltro=2", "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione", "-1");
                        ddlCondicaoPagamento.SelectedValue = idCond;
                        MensagemPagina_Modal_NovaCondicaoPagamento.MostraMensagem_Sucesso("Nova Condição de Pagamento Personalizada registrada com sucesso!", false);
                    }
                    else { throw new Exception(DATASET(ds, "msg")); }
                }
                catch (Exception ex)
                {
                    MensagemPagina_Modal_NovaCondicaoPagamento.MostraMensagem_Erro("<b>Erro: </b>Houve um erro na tentativa de Cadastrar uma Nova Condição de Pagamento Personalizada!<br />Erro: " + ex.Message, false);
                }
            }

            txtDescricao_Nova_CondicaoPagamento.Text = "";
            txtQtdParcelas_Nova_CondicaoPagamento.Text = "";
            hddNovaCondPgto_Pers_Dados.Value = "";
            Scripts.RemoverBackdrop_Modal(Page);
            Scripts.AbrirModal(Page, "modal_novaCondicaoPagamento");
            Scripts.Mantem_AbaAtiva(Page, "aba-Personalizada");
            AtualizaBarraProgresso(2, true);
            MantemEtapa_Pos_PostBack(2);
        }

        #endregion

        #region | TextChanged

        protected void txtCEP_NovoEndereco_TextChanged(object sender, EventArgs e)
        {
            div_Cidade_NovoEndereco.Visible = false;

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sCEP", txtCEP_NovoEndereco.Text.Replace("-", "") }
                };
                DataSet dsCEP = ExecutarDataSet("sp_Consulta_CEP", vParametros);

                if (ValidarDataSet(dsCEP))
                {
                    txtLogradouro_NovoEndereco.Text = DATASET(dsCEP, 0, "sLogradouro");
                    txtBairro_NovoEndereco.Text = DATASET(dsCEP, 0, "sBairro");
                    ddlEstado_NovoEndereco.SelectedValue = DATASET(dsCEP, 0, "sUF");

                    Popula_Combo(ddlCidade_NovoEndereco, "sp_Select 'CIDADE', @sPesquisa='" + ddlEstado_NovoEndereco.SelectedValue + "'", "idCidade", "sCidade", false, "Selecione uma Cidade", "0");

                    div_Cidade_NovoEndereco.Visible = true;

                    try
                    {
                        ddlCidade_NovoEndereco.SelectedValue = DATASET(dsCEP, 0, "idCidade");
                    }
                    catch
                    {
                        MensagemPagina_Modal_NovoEndereco.MostraMensagem_Erro("<b>Erro:</b> Não foi encontrada a cidade definida, de acoedo com o CEP!");
                    }
                }
                else
                    MensagemPagina_Modal_NovoEndereco.MostraMensagem_Aviso("<b>Aviso:</b> Não foi encontrado local com o CEP fornecido!", false);
            }
            catch (Exception ex)
            {
                MensagemPagina_Modal_NovoEndereco.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de preencher as informações do Local a partir do CEP fornecido!<br />Erro no Local: " + ex.Message, false);
            }

            Scripts.FocusScript(Page, txtLogradouro_NovoEndereco.ClientID);
            AbrirModal_NovoEndereco();
        }

        protected void nTotalServicos_Comparativos_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                GridViewRow row = txt.NamingContainer as GridViewRow;

                decimal.TryParse(row.Cells[Servicos_Comparativos_Coluna__Valor].Text, out decimal preco);
                decimal.TryParse(row.Cells[Servicos_Comparativos_Coluna__Quantidade].Text, out decimal qtd);
                decimal.TryParse(row.Cells[Servicos_Comparativos_Coluna__Margem].Text, out decimal margem);
                decimal.TryParse(row.Cells[Servicos_Comparativos_Coluna__Desconto].Text, out decimal desconto);

                decimal totalServico = 0;
                decimal totalAjustado = 0;
                decimal ajusteCompleto = 0;
                decimal ajusteFinal = 0;

                var servico = listServicos_Comparativos.FirstOrDefault(s => s.idRegistro.ToString().Equals(row.Cells[Servicos_Comparativos_Coluna__ID].Text));

                if (Convert.ToBoolean(hddComparativos_Empreitada.Value))
                    servico = listServicos_Empreitada.FirstOrDefault(s => s.idRegistro.ToString().Equals(row.Cells[Servicos_Comparativos_Coluna__ID].Text));

                if (servico != null)
                {
                    totalAjustado = decimal.Parse(txt.Text);

                    if (!string.IsNullOrEmpty(txt.Text) && !string.IsNullOrWhiteSpace(txt.Text))
                    {
                        totalServico = (preco * qtd * margem) - (preco * qtd * margem * (desconto / 100));

                        ajusteCompleto = totalAjustado / totalServico;

                        ajusteFinal = ajusteCompleto > 1 ? (ajusteCompleto - 1) * 100 : (ajusteCompleto * 100) - 100;
                    }
                    else
                    {
                        txt.Text = Math.Round(totalServico, 2).ToString();
                        ajusteFinal = -100;
                    }

                    servico.NAjuste = Math.Round(ajusteFinal, 2);
                    servico.NTotal = Math.Round(totalAjustado, 2);
                }
                else MensagemPaginaServicos_Comparativos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de alterar o Total do Serviço!", false);

                Scripts.FocusScript(Page, txt.ClientID);
            }
            catch (Exception ex)
            {
                MensagemPaginaServicos_Comparativos.MostraMensagem_Erro("<b>Erro:</b> Houve um erro ao calcular automaticamente o Ajuste para o Comparativo do Serviço!<br />Erro ao Comparar Serviços: " + ex.Message, false);
            }

            AtualizaClasses_Comparativo();

            Scripts.Mantem_AbaAtiva(Page, "aba-Comparativo_Servicos");
        }

        protected void txtDescontoProduto_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AtualizaClasseGeral();

                decimal.TryParse((sender as TextBox).Text, out decimal nValor);

                if (nValor > nMax_Desconto_Produtos)
                {
                    nValor = nMax_Desconto_Produtos;

                    (sender as TextBox).Text = nValor.ToString("N4");
                    MensagemPaginaProdutos.MostraMensagem_Erro($"O máximo de desconto permitido é {nValor}%!");
                }

                foreach (var p in listProdutos)
                {
                    if (!p.bLiberado)
                        continue;

                    p.SFuncao = "INCLUIR ITEM";
                    p.NFator = Math.Round(nValor, 4);
                    p.nUnitario = Math.Round(p.Preco - (p.Preco * (p.NFator / 100)), 2);
                    p.NTotal = Math.Round(p.nUnitario * p.NQuantidade, 2);

                    if (ddlMoeda.SelectedValue != "0" && ddlMoeda.SelectedValue != "2" && decimal.TryParse(txtCambio.Text, out decimal cambio))
                    {
                        p.NII = 0;
                        p.NIPI = 0;
                        p.NPIS = 0;
                        p.NCOFINS = 0;
                        p.NICMS = 0;
                        p.NDIFAL = 0;
                        p.NST = 0;

                        p.NTotal *= cambio;
                    }

                    try
                    {
                        RecalculaImpostos(p);
                    }
                    catch { }

                    // Cálculo ST
                    if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !p.bSistema && (ddlMoeda.SelectedValue == "0" || ddlMoeda.SelectedValue == "2"))
                    {
                        DataSet ds = ConsultaImpostos_Produtos(false, p.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", p.Preco.ToString().Replace(',', '.'));

                        decimal.TryParse(DATASET(ds, "nMVA"), out decimal MVA);
                        decimal.TryParse(DATASET(ds, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                        try
                        {
                            MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(ds, "sICMSST").Equals("S"), p.sNCM, p.sCEST, decimal.Parse(DATASET(ds, "nICMS")), DATASET(ds, "dtUltimaConsulta"), DATASET(ds, 1, 0, "sMensagem_Erro_LegisWeb"));

                            if (MVA > 0)
                            {
                                var st = CalculaValor_ST(p.NTotal, p.nVlr_ICMS * p.NQuantidade, MVA, ICMS_interno_destino);
                                p.nVlr_ST = st.Item1;
                                p.NST = st.Item2;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaProdutos.MostraMensagem_Erro("<b>Erro:</b> Houve um Erro no cálculo dos valores dos Produtos!<br />Erro nos cálculos: " + ex.Message);
            }

            Scripts.FocusScript(Page, txtPrazoProduto.ClientID);

            gvProdutos_dataBind();
            MantemEtapa_Pos_PostBack(4);
            AtualizaBarraProgresso(4, false);
        }

        protected void txtCalculaProdutos_Comparativos_TextChanged(object sender, EventArgs e)
        {
            string id = string.Empty;
            string classe = string.Empty;
            string msg = string.Empty;

            try
            {
                id = (sender as TextBox).ClientID;
                classe = (sender as TextBox).Attributes["class"].Replace("form-control", "").Trim().ToLower();
                decimal.TryParse((sender as TextBox).Text, out decimal nValor);

                if (classe.Contains("ajusteporcentagem"))
                {
                    decimal.TryParse(txtTotalProdutos_Empreitada.Text, out decimal nPercentualTotal_Produtos);
                    decimal.TryParse(txtComparativos_LiquidoProdutos_Ajustado.Text, out decimal nTotalAjustado_Produtos);
                    decimal nTotalOriginal_Produtos = listProdutos_Empreitada.Where(p => p.bLiberado).Sum(p => (p.Preco - p.nVlr_IPI - p.nVlr_PIS - p.nVlr_COFINS - p.nVlr_ICMS - p.nVlr_DIFAL) * p.NQuantidade);
                    decimal nTotalOriginal_Servicos = listServicos_Empreitada.Where(s => s.bLiberado).Sum(s => (s.Preco - s.nVlr_ICMS - s.nVlr_IPI) * s.NQuantidade * s.NMargem);
                    decimal nTotal_Orcamento = 0, nTotalNovo_Produtos = 0, nTotalNovo_Servicos = 0;

                    nTotal_Orcamento = nTotalOriginal_Produtos + nTotalOriginal_Servicos;

                    if (!id.Contains("Empreitada"))
                        nPercentualTotal_Produtos = nTotalAjustado_Produtos * 100 / nTotal_Orcamento;

                    nTotalNovo_Produtos = nTotal_Orcamento * (nPercentualTotal_Produtos / 100);
                    nTotalNovo_Servicos = nTotalOriginal_Servicos + (nTotal_Orcamento - nTotalNovo_Produtos - nTotalOriginal_Servicos);

                    decimal nFator_Produtos = 1 - nTotalNovo_Produtos / nTotalOriginal_Produtos;
                    decimal nFator_Servicos = nTotalNovo_Servicos / nTotalOriginal_Servicos;

                    foreach (var p in listProdutos_Empreitada)
                    {
                        if (!p.bLiberado)
                            continue;

                        p.NFator = Math.Round(nFator_Produtos * 100, 4);
                        p.nUnitario = Math.Round(p.Preco - (p.Preco * nFator_Produtos), 2);
                        p.NTotal = Math.Round((p.Preco * p.NQuantidade) - (p.Preco * p.NQuantidade * nFator_Produtos), 2);
                    }

                    foreach (var s in listServicos_Empreitada)
                    {
                        if (!s.bLiberado)
                            continue;

                        decimal totalOriginal = (s.Preco * s.NQuantidade * s.NMargem) - (s.Preco * s.NQuantidade * s.NMargem * (s.NFator / 100));
                        decimal totalNovo = totalOriginal * nFator_Servicos;

                        if (totalOriginal > 0 && totalNovo > 0)
                        {
                            decimal ajusteCompleto = totalNovo / totalOriginal;
                            decimal ajusteFinal = ajusteCompleto > 1 ? (ajusteCompleto - 1) * 100 : (ajusteCompleto * 100) - 100;

                            s.NAjuste = Math.Round(ajusteFinal, 2);
                            s.NTotal = Math.Round(totalNovo, 2);
                        }
                    }
                }
                else
                {
                    GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;

                    int.TryParse(row.Cells[Produtos_Comparativos_Coluna__ID].Text.Trim(), out int idProduto);

                    var produto = listProdutos_Comparativos.FirstOrDefault(p => p.idRegistro.Equals(idProduto));

                    if (Convert.ToBoolean(hddComparativos_Empreitada.Value))
                        produto = listProdutos_Empreitada.FirstOrDefault(p => p.idRegistro.Equals(idProduto));

                    if (produto != null)
                    {
                        if (!string.IsNullOrEmpty(classe))
                        {
                            decimal nDesconto = 0;
                            decimal nUnitario = 0;

                            if (classe.Contains("descontocomparativo"))
                            {
                                nDesconto = nValor;
                                nUnitario = produto.Preco - (produto.Preco * (nDesconto / 100));
                            }
                            else if (classe.Contains("valorcomparativo"))
                            {
                                nUnitario = nValor;
                                nDesconto = (produto.Preco - nUnitario) / produto.Preco * 100;
                            }

                            if (nDesconto > nMax_Desconto_Produtos)
                                throw new Exception($"O valor de desconto nos Produtos não pode exceder {nMax_Desconto_Produtos}%");

                            produto.SFuncao = "INCLUIR ITEM";
                            produto.NFator = Math.Round(nDesconto, 4);
                            produto.nUnitario = Math.Round(nUnitario, 2);
                            produto.NTotal = Math.Round(produto.nUnitario * produto.NQuantidade, 2);

                            try
                            {
                                RecalculaImpostos(produto);
                            }
                            catch { }

                            // Cálculo ST
                            if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !produto.bSistema)
                            {
                                DataSet ds = ConsultaImpostos_Produtos(false, produto.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", produto.Preco.ToString().Replace(',', '.'));

                                decimal.TryParse(DATASET(ds, "nMVA"), out decimal MVA);
                                decimal.TryParse(DATASET(ds, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                                try
                                {
                                    MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(ds, "sICMSST").Equals("S"), produto.sNCM, produto.sCEST, decimal.Parse(DATASET(ds, "nICMS")), DATASET(ds, "dtUltimaConsulta"), DATASET(ds, 1, 0, "sMensagem_Erro_LegisWeb"));

                                    if (MVA > 0)
                                    {
                                        var st = CalculaValor_ST(produto.NTotal, produto.nVlr_ICMS * produto.NQuantidade, MVA, ICMS_interno_destino);
                                        produto.nVlr_ST = st.Item1;
                                        produto.NST = st.Item2;
                                    }
                                }
                                catch { }
                            }
                        }
                        else throw new Exception("Campo alterado não encontrado!");
                    }
                    else throw new Exception("Produto não encontrado!");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Comparativo_Produtos.MostraMensagem_Erro("<b>Erro:</b> Houve um Erro no cálculo dos valores dos Produtos!<br />Erro no cálculo em Comparativos: " + ex.Message);
            }

            if (!string.IsNullOrEmpty(id))
                Scripts.FocusScript(Page, id);

            try
            {
                gvComparativoProdutos_dataBind();
                gvComparativoServicos_dataBind();
                AtualizaClasses_Comparativo();

                Scripts.Mantem_AbaAtiva(Page, "aba-Comparativo_Produtos");
            }
            catch { }
        }

        protected void txtDescontoProduto_Comparativos_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal.TryParse((sender as TextBox).Text, out decimal nValor);

                if (nValor > nMax_Desconto_Produtos)
                    throw new Exception($"O máximo de desconto permitido é {nMax_Desconto_Produtos}%!");

                foreach (var p in listProdutos_Comparativos)
                {
                    if (!p.bLiberado)
                        continue;

                    p.SFuncao = "INCLUIR ITEM";
                    p.NFator = Math.Round(nValor, 4);
                    p.nUnitario = Math.Round(p.Preco - (p.Preco * (p.NFator / 100)), 2);
                    p.NTotal = Math.Round(p.nUnitario * p.NQuantidade, 2);

                    try
                    {
                        RecalculaImpostos(p);
                    }
                    catch { }

                    // Cálculo ST
                    if (string.IsNullOrEmpty(hddsTipoDrawback.Value) && !p.bSistema)
                    {
                        DataSet ds = ConsultaImpostos_Produtos(false, p.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", p.Preco.ToString().Replace(',', '.'));

                        decimal.TryParse(DATASET(ds, "nMVA"), out decimal MVA);
                        decimal.TryParse(DATASET(ds, "nICMS_Interno_Destino"), out decimal ICMS_interno_destino);

                        try
                        {
                            MVA = Retorna_MVA_LegisWeb(txtUF_Origem_View.Text, hddUF_Fiscal_SelectedValue.Value, MVA, ddlDestinoVenda.SelectedValue.Equals("R"), DATASET(ds, "sICMSST").Equals("S"), p.sNCM, p.sCEST, decimal.Parse(DATASET(ds, "nICMS")), DATASET(ds, "dtUltimaConsulta"), DATASET(ds, 1, 0, "sMensagem_Erro_LegisWeb"));

                            if (MVA > 0)
                            {
                                var st = CalculaValor_ST(p.NTotal, p.nVlr_ICMS * p.NQuantidade, MVA, ICMS_interno_destino);
                                p.nVlr_ST = st.Item1;
                                p.NST = st.Item2;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Comparativo_Produtos.MostraMensagem_Erro("<b>Erro:</b> Houve um Erro no cálculo dos valores dos Produtos!<br />Erro nos cálculos em Comparativos: " + ex.Message);
            }

            Scripts.FocusScript(Page, txtPrazo_Global_Produtos_Comparativos.ClientID);

            try
            {
                gvComparativoProdutos_dataBind();
                gvComparativoServicos_dataBind();
                AtualizaClasses_Comparativo();

                Scripts.Mantem_AbaAtiva(Page, "aba-Comparativo_Produtos");
            }
            catch { }
        }

        #endregion

        #region | SelectedIndexChanged

        protected void ddlDestinoVenda_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!hddProgresso.Value.Contains("DestinoVenda"))
                hddProgresso.Value += "DestinoVenda|";

            hddidDestinoVenda.Value = ddlDestinoVenda.SelectedValue;
            txtDestinoVenda_View.Text = ddlDestinoVenda.SelectedItem.Text;

            if (listServicos_Recursos.Count > 0)
            {
                foreach (var item in listServicos_Recursos)
                {
                    DataSet dsRegras = ConsultaImpostos_Produtos(false, item.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, item.idTipoRegra.ToString(), hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", Math.Round(item.Preco, 2).ToString().Replace(",", "."));

                    if (ValidarDataSet(dsRegras))
                    {
                        item.sNCM = DATASET(dsRegras, "sCodigoFederal");
                        item.sCST = DATASET(dsRegras, "sCodigoMunicipal");
                        item.NIPI = Math.Round(decimal.Parse(DATASET(dsRegras, "nISS")), 2);
                        item.nVlr_IPI = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrISS")), 2);
                        item.NCSSL = Math.Round(decimal.Parse(DATASET(dsRegras, "nCSSL")), 2);
                        item.nVlr_CSSL = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrCSSL")), 2);
                        item.NIRPJ = Math.Round(decimal.Parse(DATASET(dsRegras, "nIR")), 2);
                        item.nVlr_IRPJ = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrIR")), 2);
                        item.NICMS = Math.Round(decimal.Parse(DATASET(dsRegras, "nINSS")), 2);
                        item.nVlr_ICMS = Math.Round(decimal.Parse(DATASET(dsRegras, "nVlrINSS")), 2);
                    }
                }
            }

            if (listProdutos.Count > 0)
            {
                foreach (var item in listProdutos.Where(p => p.bLiberado).OrderBy(p => p.nOrdem))
                {
                    DataSet dsRegras = ConsultaImpostos_Produtos(false, item.IdItem.ToString(), ddlEmpresa_Orcamento.SelectedValue, hddidCliente.Value, ddlDestinoVenda.SelectedValue.Equals("C") ? "1" : ddlDestinoVenda.SelectedValue.Equals("R") ? "2" : "3", hddidEndereco_Entrega.Value == "-1" && hddsTipoCliente.Value == "F" ? txtUF_Origem_View.Text.ToUpper().Trim() : txtUF_Fiscal.Text.ToUpper().Trim(), "0", Math.Round(item.Preco, 2).ToString().Replace(",", "."), item.bSistema ? "S" : "N");

                    if (ValidarDataSet(dsRegras))
                    {
                        int.TryParse(DATASET(dsRegras, "idRegra"), out int idRegra);

                        item.sNCM = DATASET(dsRegras, "sCodigoNCM") == "0" ? "Não Cadastrado" : DATASET(dsRegras, "sCodigoNCM");
                        item.TipoProduto = DATASET(dsRegras, "sDscTipoProduto");
                        item.IdGrupoProduto = int.Parse(DATASET(dsRegras, "idGrupo"));
                        item.IdFamiliaProduto = int.Parse(DATASET(dsRegras, "idFamilia"));
                        item.sDscGrupoProduto = DATASET(dsRegras, "sDscGrupo");
                        item.sDscFamiliaProduto = DATASET(dsRegras, "sDscFamilia");
                        item.SUnidade = DATASET(dsRegras, "sUnidade");
                        item.sIndustrializado = DATASET(dsRegras, "sProdutoIndustrializado") == "S" ? "Sim" : "Não";
                        item.sOrigem = DATASET(dsRegras, "sProdutoImportado") == "S" ? "IMP" : "BR";
                        item.idRegra = idRegra;
                        item.sCFOP = DATASET(dsRegras, "sCFOP");
                        item.sCST = DATASET(dsRegras, "CST");
                        item.NIPI = Math.Round(decimal.Parse(DATASET(dsRegras, "nIPI")), 2);
                        item.NPIS = Math.Round(decimal.Parse(DATASET(dsRegras, "nPIS")), 2);
                        item.NCOFINS = Math.Round(decimal.Parse(DATASET(dsRegras, "nCOFINS")), 2);
                        item.NICMS = Math.Round(decimal.Parse(DATASET(dsRegras, "nICMS")), 2);
                        item.NST = Math.Round(decimal.Parse(DATASET(dsRegras, "nICMSST")), 2);
                        item.nReducao = Math.Round(decimal.Parse(DATASET(dsRegras, "nRedBC")), 2);

                        RecalculaImpostos(item);

                        item.nPesoBruto = Math.Round(decimal.Parse(DATASET(dsRegras, "nPesoBruto")), 2);
                        item.nPesoLiquido = Math.Round(decimal.Parse(DATASET(dsRegras, "nPesoNeto")), 2);
                        item.nVolume = Math.Round(decimal.Parse(DATASET(dsRegras, "nVolume")), 2);
                    }
                }
            }

            AtualizaBarraProgresso(1, false);
            MantemEtapa_Pos_PostBack(1);

            Scripts.FocusScript(Page, ddlFormaEnvio.ClientID);
        }

        protected void ddlTipoOrcamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTipoOrcamento.SelectedValue != "0")
            {
                div_Fluxo.Visible = true;

                string idPais = Variaveis.idEmpresa() == "Brasil" ? "2" : Variaveis.idEmpresa() == "0" ? "2" : "0";
                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "SelecionaTipoOrcamento__Orcamentos" },
                    { "@idPesquisa", ddlTipoOrcamento.SelectedValue }
                };

                List<cls_Multiplos_Combos> ddls = new List<cls_Multiplos_Combos>
                {
                    new cls_Multiplos_Combos { ddl = ddlEmpresa_Orcamento, sCampoCodigo = "idParceiro", sCampoDescricao = "sDscEmpresa", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione a Empresa", sValorPrimeiraLinha = "-2" },
                    new cls_Multiplos_Combos { ddl = ddlFluxo, sCampoCodigo = "idFluxo", sCampoDescricao = "sDscFluxo", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione o Fluxo", sValorPrimeiraLinha = "0" },
                    new cls_Multiplos_Combos { lst = lstTipoServicos_TipoOrcamento, sCampoCodigo = "idObjeto", sCampoDescricao = "sDscObjeto", bConcatenarCodigo_Descricao = false },
                    new cls_Multiplos_Combos { lst = lstEscopos_TipoOrcamento, sCampoCodigo = "idEscopo", sCampoDescricao = "sDscEscopo", bConcatenarCodigo_Descricao = false }
                };
                Popula_Multiplos_Combos(ddls, vParam);

                if (lstTipoServicos_TipoOrcamento.Items.Count <= 0)
                {
                    div_lstTipoServicos.Visible = false;
                    div_AtualizarServicos.Visible = false;
                }
                else
                {
                    div_lstTipoServicos.Visible = true;
                    div_AtualizarServicos.Visible = true;
                }

                if (lstEscopos_TipoOrcamento.Items.Count <= 0)
                {
                    div_lstEscopos.Visible = false;
                    div_AtualizarServicos.Visible = false;
                }
                else
                {
                    div_lstEscopos.Visible = true;
                    div_AtualizarServicos.Visible = true;
                }

                if (dt_CheckList.Rows.Count > 0)
                    MensagemPaginaInfoInicial.MostraMensagem_Aviso("<b>Aviso:</b> Ao selecionar outro Tipo de Orçamento a Matriz de Escopo é reiniciada e deve ser preenchida novamente!", false);

                if (!hddProgresso.Value.Contains("Cliente"))
                {
                    ddlTipoOrcamento.SelectedValue = "0";
                    div_Fluxo.Visible = false;
                    div_empresa.Visible = false;

                    MensagemPaginaInfoInicial.MostraMensagem_Aviso("<b>Aviso:</b> É necessário selecionar um Cliente antes do Tipo de Orçamento!", false);
                }
                else
                {
                    if (ddlTabela.SelectedValue == "0")
                    {
                        ddlTipoOrcamento.SelectedValue = "0";
                        div_Fluxo.Visible = false;
                        div_empresa.Visible = false;

                        MensagemPaginaInfoInicial.MostraMensagem_Aviso("<b>Aviso:</b> É necessário selecionar uma Tabela de Preços!", false);

                        Scripts.FocusScript(Page, ddlTabela.ClientID);
                    }
                    else
                    {
                        if (ddlEndereco.SelectedValue == "0")
                        {
                            ddlTipoOrcamento.SelectedValue = "0";
                            div_Fluxo.Visible = false;
                            div_empresa.Visible = false;

                            MensagemPaginaInfoInicial.MostraMensagem_Aviso("<b>Aviso:</b> É necessário selecionar um Endereço Fiscal!", false);

                            Scripts.FocusScript(Page, ddlEndereco.ClientID);
                        }
                        else
                        {
                            if (ddlEndereco_Entrega.SelectedValue == "0")
                            {
                                ddlTipoOrcamento.SelectedValue = "0";
                                div_Fluxo.Visible = false;
                                div_empresa.Visible = false;

                                MensagemPaginaInfoInicial.MostraMensagem_Aviso("<b>Aviso:</b> É necessário selecionar o Endereço de Entrega!", false);

                                Scripts.FocusScript(Page, ddlEndereco_Entrega.ClientID);
                            }
                            else
                            {
                                if (!hddProgresso.Value.Contains("TipoOrcamento"))
                                    hddProgresso.Value += "TipoOrcamento|";

                                MensagemPaginaInfoInicial.MostraMensagem("<b>Informativo:</b> É necessário selecionar um Fluxo!", "info", false);

                                Scripts.FocusScript(Page, ddlFluxo.ClientID);
                            }
                        }
                    }
                }
            }
            else
            {
                listServicos_Recursos.Clear();
                listServicos_Recursos_Composicao_Filhos.Clear();
                listServicos_Recursos_Composicao_Netos.Clear();
                listServicos_Recursos_Composicao_Bisnetos.Clear();

                div_Fluxo.Visible = false;
                div_empresa.Visible = false;

                if (hddProgresso.Value.Contains("TipoOrcamento"))
                    hddProgresso.Value = hddProgresso.Value.Replace("TipoOrcamento|", "");
                if (hddProgresso.Value.Contains("Servico"))
                    hddProgresso.Value = hddProgresso.Value.Replace("Servico|", "");

                Scripts.FocusScript(Page, ddlTipoOrcamento.ClientID);
            }

            hddidTipoOrcamento.Value = ddlTipoOrcamento.SelectedValue;

            AtualizaBarraProgresso(1, false);
            MantemEtapa_Pos_PostBack(1);
        }

        protected void ddlFluxo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlFluxo.SelectedValue != "0")
            {
                if (!hddProgresso.Value.Contains("Fluxo"))
                    hddProgresso.Value += "Fluxo|";

                div_empresa.Visible = true;
                MensagemPaginaInfoInicial.MostraMensagem("<b>Informativo:</b> É necessário selecionar uma Empresa!", "info", false);
            }
            else
            {
                div_Fluxo.Visible = true;

                if (hddProgresso.Value.Contains("Fluxo"))
                    hddProgresso.Value = hddProgresso.Value.Replace("Fluxo|", "");

                div_empresa.Visible = false;
                MensagemPaginaInfoInicial.MostraMensagem("<b>Informativo:</b> É necessário selecionar um Fluxo!", "info", false);
            }

            Scripts.FocusScript(Page, ddlFluxo.ClientID);

            AtualizaBarraProgresso(1, false);
            MantemEtapa_Pos_PostBack(1);
        }

        protected void ddlEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEmpresa_Orcamento.SelectedValue != "-2")
            {
                cmdVoltarEtapa.Visible = true;
                cmdAvancarEtapa.Visible = true;
                divStories.Visible = true;

                int nPaisEmpresa = 2;

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idParceiro", ddlEmpresa_Orcamento.SelectedValue }
                };
                DataSet ds = ExecutarDataSet(sProcedure_Empresas, vParametros);

                if (ValidarDataSet(ds))
                {
                    try
                    {
                        nPaisEmpresa = string.IsNullOrEmpty(DATASET(ds, 2, 0, "idTipoPais")) ? 2 : int.Parse(DATASET(ds, 2, 0, "idTipoPais"));
                    }
                    catch { }

                    txtUF_Origem_View.Text = string.IsNullOrEmpty(DATASET(ds, 2, 0, "sEstado")) ? string.Empty : DATASET(ds, 2, 0, "sEstado").ToUpper();
                }
                else txtUF_Origem_View.Text = string.Empty;

                hddidEmpresa.Value = ddlEmpresa_Orcamento.SelectedValue;
                FiltroPesquisaProdutos.TerritorioEmpresa = nPaisEmpresa;

                string sTabelaSelecionada = ddlTabela.SelectedValue;

                // Empresa: BRASIL - idPais = 1
                // Tabelas de Preço: BRASIL - idMoedaOrigem = 2
                string idPais = Variaveis.idEmpresa() == "Brasil" ? "2" : Variaveis.idEmpresa() == "0" ? "2" : "0";
                Popula_Combo(ddlTabela, string.Format("sp_Select 'Flow_Comercial_TabelaPreco__Orcamento', @idPesquisa={0}, @idFiltro={1}", hddidCliente.Value, idPais), "idTabela", "sDscTabela", false, "Selecione uma Tabela de Preço", "0");

                try
                {
                    ddlTabela.SelectedValue = sTabelaSelecionada.Equals("0") ? ddlTabela.Items[1].Value : sTabelaSelecionada;
                }
                catch
                {
                    ddlTabela.SelectedValue = ddlTabela.Items[1].Value;
                }

                try
                {
                    hddidTiposServicos.Value = "|";
                    hddidEscopos.Value = "|";

                    foreach (List_Item item in lstTipoServicos_TipoOrcamento.Items) { if (item.Selected) hddidTiposServicos.Value += item.Value + "|"; }
                    foreach (List_Item item in lstEscopos_TipoOrcamento.Items) { if (item.Selected) hddidEscopos.Value += item.Value + "|"; }
                }
                catch { }

                Popula_gvServicos_Recursos(ddlTipoOrcamento.SelectedValue, false);
                Popula_gvCheckList(ddlTipoOrcamento.SelectedValue, true);

                if (!string.IsNullOrEmpty(Request["idCotacao"]) && gvProdutos.Rows.Count <= 0)
                {
                    try
                    {

                        Dictionary<string, string> vParamCotacao = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTA_ITENS_COTACAO" },
                            { "@idPedido", Request["idCotacao"] }
                        };
                        DataTable dt = ExecutarDataTable(sProcedure, vParamCotacao);

                        foreach (DataRow row in dt.Rows)
                        {
                            FiltroPesquisaProdutos.IdItem = row["idProduto"].ToString();
                            FiltroPesquisaProdutos.SCodigo = row["sCodigo"].ToString();
                            FiltroPesquisaProdutos.SDscProduto = row["sDscProduto"].ToString();
                            FiltroPesquisaProdutos.NQuantidade = Convert.ToDecimal(row["nQuantidade"]);
                            FiltroPesquisaProdutos.hddnValorProduto = Convert.ToDecimal(row["nValorUnitario"]);

                            IncluirItem();
                        }

                        gvProdutos_dataBind();
                    }
                    catch (Exception ex)
                    {
                        MensagemPaginaProdutos.MostraMensagem_Erro($"<b>Erro:</b> Houve um erro ao incluir os Itens da Cotação!<br />Erro nos Itens da Cotação: {ex.Message}");
                    }
                }

                if (!hddProgresso.Value.Contains("Empresa")) hddProgresso.Value += "Empresa|";
            }
            else
            {
                cmdVoltarEtapa.Visible = false;
                cmdAvancarEtapa.Visible = false;
                divStories.Visible = false;

                listProdutos.Clear();
                listServicos_Recursos.Clear();
                listServicos_Recursos_Composicao_Filhos.Clear();
                listServicos_Recursos_Composicao_Netos.Clear();
                listServicos_Recursos_Composicao_Bisnetos.Clear();

                FiltroPesquisaProdutos.TerritorioEmpresa = 0;
                txtUF_Origem_View.Text = string.Empty;

                if (hddProgresso.Value.Contains("Empresa"))
                    hddProgresso.Value = hddProgresso.Value.Replace("Empresa|", "");

                Scripts.FocusScript(Page, ddlEmpresa_Orcamento.ClientID);
            }

            AtualizaBarraProgresso(1, false);
            MantemEtapa_Pos_PostBack(1);
        }

        protected void ddlVendedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlVendedor.SelectedValue != "0")
            {
                Dictionary<string, string> vParamentros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_VENDEDOR" },
                    { "@idUsuarioAtualizacao", ddlVendedor.SelectedValue }
                };

                DataSet ds = ExecutarDataSet(sProcedure_CRM, vParamentros);

                if (ValidarDataSet(ds))
                    hddidVendedor.Value = DATASET(ds, "idVendedor");

                if (!hddProgresso.Value.Contains("Vendedor"))
                    hddProgresso.Value += "Vendedor|";

                Scripts.FocusScript(Page, ddlDestinoVenda.ClientID);
            }
            else
            {
                hddidVendedor.Value = "0";

                if (hddProgresso.Value.Contains("Vendedor"))
                    hddProgresso.Value = hddProgresso.Value.Replace("Vendedor|", "");

                Scripts.FocusScript(Page, ddlVendedor.ClientID);
            }

            AtualizaBarraProgresso(2, false);
            MantemEtapa_Pos_PostBack(2);
        }

        protected void ddlEndereco_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEndereco.SelectedValue != "0")
            {
                if (!hddProgresso.Value.Contains("Endereco_Fiscal"))
                    hddProgresso.Value += "Endereco_Fiscal|";

                hddidEndereco_Fiscal.Value = ddlEndereco.SelectedValue;

                divs_End_Fiscal.Visible = true;

                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTA_DETALHE_ENDERECO" },
                    { "@idCliente_Endereco", ddlEndereco.SelectedValue }
                };
                DataSet ds = ExecutarDataSet(sProcedure_Clientes, vParam);

                if (ValidarDataSet(ds))
                {
                    try
                    {
                        txtUF_Fiscal.Text = DATASET(ds, "sEstado");
                        hddUF_Fiscal_SelectedValue.Value = DATASET(ds, "sEstado");
                        txtUF_Fiscal_View.Text = txtUF_Fiscal.Text;

                        txtMunicipioFiscal.Text = DATASET(ds, "sCidade");
                        txtMunicipioFiscal_View.Text = txtMunicipioFiscal.Text;

                        txtEndereco_View.Text = ddlEndereco.SelectedItem.Text;

                        txtEnderecoEntrega_View.Text = ddlEndereco.SelectedItem.Text;
                        Scripts.FocusScript(Page, ddlEndereco_Entrega.ClientID);
                    }
                    catch (Exception ex)
                    {
                        MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Houve um erro na tentativa de preencher as informações do Endereço Fiscal selecionado!<br />Erro nas informações do Endereço Fiscal: " + ex.Message, false);
                    }
                }
                else
                {
                    if (hddProgresso.Value.Contains("Endereco_Fiscal"))
                        hddProgresso.Value = hddProgresso.Value.Replace("Endereco_Fiscal|", "");

                    divs_End_Fiscal.Visible = false;

                    txtUF_Fiscal.Text = string.Empty;
                    txtUF_Fiscal_View.Text = string.Empty;

                    txtMunicipioFiscal.Text = string.Empty;
                    txtMunicipioFiscal_View.Text = string.Empty;

                    txtEndereco_View.Text = string.Empty;

                    Scripts.FocusScript(Page, ddlEndereco.ClientID);

                    MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Houve um erro ao Consultar as informações do Endereço Fiscal selecionado!", false);
                }
            }
            else
            {
                if (hddProgresso.Value.Contains("Endereco_Fiscal"))
                    hddProgresso.Value = hddProgresso.Value.Replace("Endereco_Fiscal|", "");

                divs_End_Fiscal.Visible = false;

                txtUF_Fiscal.Text = string.Empty;
                txtUF_Fiscal_View.Text = string.Empty;

                txtMunicipioFiscal.Text = string.Empty;
                txtMunicipioFiscal_View.Text = string.Empty;

                txtEndereco_View.Text = string.Empty;

                Scripts.FocusScript(Page, ddlEndereco.ClientID);
            }

            if (!(sender is null))
            {
                AtualizaBarraProgresso(1, false);
                MantemEtapa_Pos_PostBack(1);
            }
        }

        protected void ddlTabela_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTabela.SelectedValue == "0")
            {
                if (hddProgresso.Value.Contains("TabelaPreco"))
                    hddProgresso.Value = hddProgresso.Value.Replace("TabelaPreco|", "");

                listProdutos.Clear();

                cmdAvancarEtapa.Visible = false;
                cmdVoltarEtapa.Visible = false;
                divStories.Visible = false;
                div_Tabela_Obs.Visible = false;

                cmdTabelaPreco_ImportarProdutos.Visible = false;

                txtTabela_View.Text = "";
                txtTabelaPreco_ImportarProdutos.Text = "";

                MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> É necessáiro selecionar uma Tabela de Preços!", false);

                Scripts.FocusScript(Page, ddlTabela.ClientID);
            }
            else
            {
                AtualizaClasseGeral();

                if (!hddProgresso.Value.Contains("TabelaPreco"))
                    hddProgresso.Value += "TabelaPreco|";

                cmdAvancarEtapa.Visible = true;
                cmdVoltarEtapa.Visible = true;
                divStories.Visible = true;
                div_Tabela_Obs.Visible = true;

                cmdTabelaPreco_ImportarProdutos.Visible = true;

                txtTabela_View.Text = ddlTabela.SelectedItem.Text;

                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idTabela", ddlTabela.SelectedValue }
                };
                DataSet ds = ExecutarDataSet(sProcedure_TabelaPreco, vParam);

                if (DATASET(ds, "idTipoTabela").Equals("11"))
                    cbLPU.Visible = true;
                else
                    cbLPU.Visible = false;

                txtTabelaPreco_ImportarProdutos.Text = string.Format("ID: {0} - Tabela: {1}{2}", ddlTabela.SelectedValue, ddlTabela.SelectedItem.Text, ValidarDataSet(ds) && !string.IsNullOrEmpty(DATASET(ds, "sDscTipoTabela")) ? $", Tipo: {DATASET(ds, "sDscTipoTabela")}" : "");

                FiltroPesquisaProdutos.idTabelaParceiro = ddlTabela.SelectedValue;

                if (listProdutos.Count > 0)
                    MantemProdutos(ddlTabela.SelectedValue);

                MensagemPaginaInfoInicial.MostraMensagem_Aviso("<b>Aviso:</b> É necessário adicionar uma Observação com o Motivo da alteração da Tabela de Preço!", false);

                Scripts.FocusScript(Page, txtTabelaObs.ClientID);
            }

            UpdModal_Importar_Pedidos.Update();
            UpdModal_Importar_LM.Update();
            UpdModal_Importar_Orcamento.Update();
            UpdModal_Importar_TabelaPreco.Update();

            AtualizaBarraProgresso(1, false);
            MantemEtapa_Pos_PostBack(1);
        }

        protected void ddlConfidencial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlConfidencial.SelectedValue == "S")
            {
                txtConfidencial_View.Text = ddlConfidencial.SelectedItem.Text;

                MensagemPaginaInfoInicial.MostraMensagem_Aviso("<b>Aviso:</b> Agora este Orçamento <b>é</b> Confidencial!", false);
            }
            else
            {
                txtConfidencial_View.Text = ddlConfidencial.SelectedItem.Text;

                MensagemPaginaInfoInicial.MostraMensagem_Aviso("<b>Aviso:</b> Agora este Orçamento <b>não é</b> Confidencial!", false);
            }

            AtualizaBarraProgresso(1, false);
            MantemEtapa_Pos_PostBack(1);
        }

        protected void ddlEndereco_Entrega_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEndereco_Entrega.SelectedValue != "0")
            {
                if (!hddProgresso.Value.Contains("Endereco_Entrega"))
                    hddProgresso.Value += "Endereco_Entrega|";

                divs_End_Entrega.Visible = true;
                hddidEndereco_Entrega.Value = ddlEndereco_Entrega.SelectedValue;
                txtEnderecoEntrega_View.Text = ddlEndereco_Entrega.SelectedItem.Text;

                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTA_DETALHE_ENDERECO" },
                    { "@idCliente_Endereco", ddlEndereco_Entrega.SelectedValue }
                };
                DataSet ds = ExecutarDataSet(sProcedure_Clientes, vParam);

                if (ValidarDataSet(ds))
                {
                    try
                    {
                        ddlUF_Entrega.SelectedValue = DATASET(ds, "sEstado");
                        hddUF_Entrega_SelectedValue.Value = DATASET(ds, "sEstado");
                        txtUF_Entrega_View.Text = ddlUF_Entrega.SelectedItem.Text;

                        Popula_Combo(ddlMunicipio_Entrega, "sp_Select 'CIDADE', @sPesquisa=" + ddlUF_Entrega.SelectedValue, "idCidade", "sCidade", false, "Selecione", "0");
                    }
                    catch (Exception ex)
                    {
                        MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Houve um erro no cadastro do UF referente ao Endereço de Entrega selecionado!<br />Erro no UF do Endereço de Entrega: " + ex.Message, false);
                    }

                    try
                    {
                        ddlMunicipio_Entrega.SelectedValue = DATASET(ds, "idCidade");
                        hddMunicipio_Entrega_SelectedValue.Value = DATASET(ds, "idCidade");
                        txtMunicipio_Entrega_View.Text = ddlMunicipio_Entrega.SelectedItem.Text;

                        Scripts.FocusScript(Page, ddlContato.ClientID);
                    }
                    catch (Exception ex)
                    {
                        MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Houve um erro no cadastro do Município referente ao Endereço de Entrega selecionado!<br />Erro no Município do Endereço de Entrega: " + ex.Message, false);
                    }
                }
                else
                {
                    if (ddlEndereco_Entrega.SelectedValue != "-1")
                    {
                        txtEnderecoEntrega_View.Text = string.Empty;

                        if (hddProgresso.Value.Contains("Endereco_Entrega"))
                            hddProgresso.Value = hddProgresso.Value.Replace("Endereco_Entrega|", "");

                        Scripts.FocusScript(Page, ddlEndereco_Entrega.ClientID);

                        MensagemPaginaInfoInicial.MostraMensagem_Erro("<b>Erro:</b> Houve um erro ao Consultar as informações do Endereço de Entrega selecionado!", false);
                    }
                    else
                        txtEnderecoEntrega_View.Text = "Coleta";

                    divs_End_Entrega.Visible = false;

                    ddlUF_Entrega.SelectedValue = "0";
                    txtUF_Entrega_View.Text = string.Empty;

                    ddlMunicipio_Entrega.SelectedValue = "0";
                    txtMunicipio_Entrega_View.Text = string.Empty;
                }
            }
            else
            {
                if (hddProgresso.Value.Contains("Endereco_Entrega"))
                    hddProgresso.Value = hddProgresso.Value.Replace("Endereco_Entrega|", "");

                divs_End_Entrega.Visible = false;

                ddlUF_Entrega.SelectedValue = "0";
                txtUF_Entrega_View.Text = string.Empty;

                ddlMunicipio_Entrega.SelectedValue = "0";
                txtMunicipio_Entrega_View.Text = string.Empty;

                txtEnderecoEntrega_View.Text = string.Empty;

                Scripts.FocusScript(Page, ddlEndereco_Entrega.ClientID);
            }

            if (!(sender is null))
            {
                AtualizaBarraProgresso(1, false);
                MantemEtapa_Pos_PostBack(1);
            }
        }

        protected void ddlEstado_NovoEndereco_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEstado_NovoEndereco.SelectedValue != "0")
            {
                div_Cidade_NovoEndereco.Visible = true;
                Popula_Combo(ddlCidade_NovoEndereco, "sp_Select 'CIDADE', @sPesquisa='" + ddlEstado_NovoEndereco.SelectedValue + "'", "idCidade", "sCidade", false, "Selecione uma Cidade", "0");
                Scripts.FocusScript(Page, ddlCidade_NovoEndereco.ClientID);
            }
            else
                div_Cidade_NovoEndereco.Visible = false;

            AbrirModal_NovoEndereco();
        }

        protected void ddlEmpresa_Comparativo_Ajustado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEmpresa_Comparativo_Ajustado.SelectedValue != "-2")
            {
                div_Comparativos_Ajustado.Visible = true;

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idParceiro", ddlEmpresa_Comparativo_Ajustado.SelectedValue }
                };
                DataSet ds = ExecutarDataSet(sProcedure_Empresas, vParametros);

                if (ValidarDataSet(ds))
                {
                    txtUF_Origem_Comparativo_Ajustado.Text = string.IsNullOrEmpty(DATASET(ds, 2, 0, "sEstado")) ? string.Empty : DATASET(ds, 2, 0, "sEstado").ToUpper();
                    hddMunicipio_Origem.Value = DATASET(ds, 2, 0, "sDscMunicipio").ToUpper();
                }
                else
                {
                    txtUF_Origem_Comparativo_Ajustado.Text = string.Empty;
                    hddMunicipio_Origem.Value = string.Empty;
                }
            }
            else
            {
                div_Comparativos_Ajustado.Visible = false;

                txtUF_Origem_Comparativo_Ajustado.Text = string.Empty;
                hddMunicipio_Origem.Value = string.Empty;

                MensagemPagina_Comparativos.MostraMensagem_Erro("<b>Erro:</b> Para aplicar os Comparativos é necessário haver uma Empresa selecionada!", false);
            }

            hddidEmpresa.Value = ddlEmpresa_Comparativo_Ajustado.SelectedValue;

            int valor = hddMunicipio_Origem.Value.ToUpper() == "SAO PAULO" && hddMunicipio_Fiscal.Value.ToUpper() == "SAO PAULO" ? 40 : 50;
            hddMax_Material_Empreitada.Value = valor.ToString();

            AtualizaClasses_Comparativo();
        }

        protected void ddlSelecionaPedido_LM_ImportarProdutos_SelectedIndexChanged(object sender, EventArgs e)
        {
            string value = "0";

            if (ddlSelecionaPedido_LM_ImportarProdutos.SelectedValue == "0")
            {
                divLM_ImportarProdutos.Visible = false;
                ddlLM_ImportarProdutos.SelectedValue = "0";

                MensagemPagina_Modal_ImportarProdutos_LM.MostraMensagem_Erro("<b>Erro:</b> É necessário selecionar um Pedido para selecionar uma de suas Listas de Materiais, e a partir daí Importar os Itens!", false);

                Scripts.FocusScript(Page, ddlSelecionaPedido_LM_ImportarProdutos.ClientID);
            }
            else
            {
                divLM_ImportarProdutos.Visible = true;

                Popula_Combo(ddlLM_ImportarProdutos, $"sp_Select 'IMPORTACAO_ITENS_ORCAMENTO', {ddlSelecionaPedido_LM_ImportarProdutos.SelectedValue}, @idFiltro=2", "idLM", "sDscLM", false, "Selecione uma LM do Pedido selecionado", "0");

                value = ddlSelecionaPedido_LM_ImportarProdutos.SelectedValue;

                Scripts.FocusScript(Page, ddlLM_ImportarProdutos.ClientID);
            }

            MantemEtapa_Pos_PostBack(4);
            AtualizaBarraProgresso(4, false);

            if (!value.Equals("0"))
                ddlSelecionaPedido_LM_ImportarProdutos.SelectedValue = value;

            Scripts.RemoverBackdrop_Modal(Page);
            Scripts.AbrirModal(Page, "modalUpload_ImportarExcel");
            Scripts.Mantem_AbaAtiva(Page, "aba-LM");
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            hddidStatus.Value = ddlStatus.SelectedValue;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "ALTERAR_STATUS" },
                { "@idPedido", Request["id"] },
                { "@idStatus", ddlStatus.SelectedValue },
                { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
            };
            ExecutarDataSet(sProcedure, vParametros);

            DirecionaPagina($"App/Paginas/Comercial/Orcamento_Detalhe.aspx?id={Request["id"]}&msg=3");
        }

        protected void ddlMoeda_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlMoeda.SelectedValue == "0")
                MensagemPaginaDentro_View.MostraMensagem_Erro("É obrigatório selecionar uma Moeda!");
            else
            {
                DataSet ds = ExecutarDataSet(sProcedure_Moedas, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idMoeda", ddlMoeda.SelectedValue } });

                hddMoeda.Value = ddlMoeda.SelectedValue;
                hddMoeda_Simbolo.Value = DATASET(ds, "sSimbolo");

                decimal.TryParse(DATASET(ds, "nValorCambio"), out decimal nCambio);
                txtCambio.Text = nCambio.ToString("N4");
            }

            cmdAtualiza_Click(lnkAtualizaInicial, null);
        }

        #endregion

        #region | ItemDataBound

        protected void rptSegmentos_View_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string idSegmento = e.Item.DataItem.ToString();

                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idSegmento_TipoCliente", idSegmento }
                };
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes_Segmentos_TipoCliente", vParam);

                if (ValidarDataSet(ds))
                {
                    Label lbl = e.Item.FindControl("lblSegmento_View") as Label;

                    lbl.Text = DATASET(ds, "sDscSegmento_TipoCliente");
                    lbl.Attributes["class"] += string.Format(" label label-{0}", string.IsNullOrEmpty(DATASET(ds, "sCor")) || string.IsNullOrWhiteSpace(DATASET(ds, "sCor")) || DATASET(ds, "sCor").Equals("0") ? "primary" : DATASET(ds, "sCor"));
                }
            }
        }

        protected void rptTiposServicos_View_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lbl = e.Item.FindControl("lblTipo_View") as Label;

                lbl.Text = e.Item.DataItem.ToString();
                lbl.Attributes["class"] += " label label-default";
            }
        }

        protected void rptEscopos_View_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lbl = e.Item.FindControl("lblEscopo_View") as Label;

                lbl.Text = e.Item.DataItem.ToString();
                lbl.Attributes["class"] += " label label-default";
            }
        }

        #endregion

        #endregion

        #region | Script

        /// <summary>
        /// Método utilizado para aplicar funções à Página via JavaScript, incluindo Máscaras para campos, Cálculos automáticos na Págia, entre outras funcionalidades especiais.
        /// </summary>
        protected void RegistraScript()
        {
            Scripts.EsconderCampo(Page, "desaparece2", true);
            Scripts.Aplica_TooltipPersonalizado(Page, "tooltip");

            StringBuilder sb = new StringBuilder();

            sb.Append(RetornaScript_IncluirServicos(txtIncluirServico_Empreitada_Codigo.ClientID, true));
            sb.Append(RetornaScript_IncluirServicos(txtIncluirServico_Empreitada_Descricao.ClientID, false));

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina_IncluirServico", sb.ToString(), true);

            sb.Clear();

            // Aplica Símbolo da Moeda selecionada
            sb.Append($"setInterval(function() {{ $('b.sSimboloMoeda').text($('#{hddMoeda_Simbolo.ClientID}').val()); }}, 1000);");

            // Collapse para exibir a mensagem com os Itens
            sb.Append("$('.exibeItens_msg').off('click').on('click', function(e) {\r\n");
            sb.Append("     e.preventDefault();\r\n");
            sb.Append("     var icone = $(this).find('i');\r\n");
            sb.Append("     var div = $(this).siblings('.div_tableRegraFiscal_msg');\r\n");
            sb.Append("     if (icone.hasClass('fa fa-chevron-down')) {\r\n");
            sb.Append("         div.removeClass('id');\r\n");
            sb.Append("         icone.removeClass('fa fa-chevron-down').addClass('fa fa-chevron-up');\r\n");
            sb.Append("     }\r\n");
            sb.Append("     else if (icone.hasClass('fa fa-chevron-up')) {\r\n");
            sb.Append("         div.addClass('id');\r\n");
            sb.Append("         icone.removeClass('fa fa-chevron-up').addClass('fa fa-chevron-down');\r\n");
            sb.Append("     }\r\n");
            sb.Append("});\r\n\r\n");

            // Animação da Navegação do botão lateral Esquerdo
            sb.Append("$('[id*=cmdVoltarEtapa]').off('click').on('click', function() {\r\n");
            sb.Append("     $(\".painel\").each(function() {\r\n");
            sb.Append("         if ($(this).hasClass('aparece')) {\r\n");
            sb.Append("             var divId = \"#\" + (parseInt($(this).attr(\"id\").replace(\"cphCorpo_pn\", '')) - 1);\r\n");
            sb.Append("             divId = divId.replace(\"#\", \"#cphCorpo_pn\");\r\n");
            sb.Append("             divId = divId.replace(\"0\", \"5\");\r\n");
            sb.Append("             var stories = divId.replace(\"_pn\", \"_painelCollapse\");\r\n");
            sb.Append("             $('.cmdStories').removeClass('cmdStories_focus');\r\n");
            sb.Append("             $(stories).addClass('cmdStories_focus');\r\n");
            sb.Append("             $(\".painel\").not(divId).removeClass(\"aparece\").removeClass(\"desaparece2\").removeClass(\"desaparece1\").removeClass(\"desaparece_stories\").addClass(\"desaparece2\");\r\n");
            sb.Append("             $(\".painel\").not(divId).hide();\r\n");
            sb.Append("             $(divId).removeClass(\"desaparece2\").removeClass(\"desaparece1\").removeClass(\"desaparece_stories\").addClass(\"desaparece1\").show();\r\n");
            sb.Append("             $(divId).removeClass(\"desaparece2\").removeClass(\"desaparece1\").removeClass(\"desaparece_stories\").addClass(\"aparece\");\r\n");
            sb.Append("             return false;\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n");
            sb.Append("});\r\n\r\n");

            // Animação da Navegação do botão lateral Direito
            sb.Append("$('[id*=cmdAvancarEtapa]').off('click').on('click', function() {\r\n");
            sb.Append("     $(\".painel\").each(function() {\r\n");
            sb.Append("         if ($(this).hasClass('aparece')) {\r\n");
            sb.Append("             var divId = \"#\" + (parseInt($(this).attr(\"id\").replace(\"cphCorpo_pn\", '')) + 1);\r\n");
            sb.Append("             divId = divId.replace(\"#\", \"#cphCorpo_pn\");\r\n");
            sb.Append("             divId = divId.replace(\"6\", \"1\");\r\n");
            sb.Append("             var stories = divId.replace(\"_pn\", \"_painelCollapse\");\r\n");
            sb.Append("             $('.cmdStories').removeClass('cmdStories_focus');\r\n");
            sb.Append("             $(stories).addClass('cmdStories_focus');\r\n");
            sb.Append("             $(\".painel\").not(divId).removeClass(\"aparece\").removeClass(\"desaparece2\").removeClass(\"desaparece1\").removeClass(\"desaparece_stories\").addClass(\"desaparece1\");\r\n");
            sb.Append("             $(\".painel\").not(divId).hide();\r\n");
            sb.Append("             $(divId).removeClass(\"desaparece2\").removeClass(\"desaparece1\").removeClass(\"desaparece_stories\").addClass(\"desaparece2\").show();\r\n");
            sb.Append("             $(divId).removeClass(\"desaparece2\").removeClass(\"desaparece1\").removeClass(\"desaparece_stories\").addClass(\"aparece\");\r\n");
            sb.Append("             return false;\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n");
            sb.Append("});\r\n\r\n");

            // Animação da Navegação dos Stories
            sb.Append("$('.cmdStories').off('click').on('click', function() {\r\n");
            sb.Append("     $('.cmdStories').removeClass('cmdStories_focus');\r\n");
            sb.Append("     $(this).addClass('cmdStories_focus');\r\n");
            sb.Append("     var divId = \"#\" + (parseInt($(this).attr(\"id\").replace(\"cphCorpo_painelCollapse\", '')));\r\n");
            sb.Append("     divId = divId.replace(\"#\", \"#cphCorpo_pn\");\r\n");
            sb.Append("     $(\".painel\").not(divId).removeClass(\"desaparece_stories\").removeClass(\"aparece\").removeClass(\"desaparece2\").removeClass(\"desaparece1\").addClass(\"desaparece_stories\");\r\n");
            sb.Append("     $(\".painel\").not(divId).hide();\r\n");
            sb.Append("     $(divId).removeClass(\"desaparece2\").removeClass(\"desaparece1\").removeClass(\"aparece\").removeClass(\"desaparece_stories\").addClass(\"desaparece_stories\").show();\r\n");
            sb.Append("     $(divId).removeClass(\"desaparece2\").removeClass(\"desaparece1\").removeClass(\"aparece\").removeClass(\"desaparece_stories\").addClass(\"aparece\");\r\n");
            sb.Append("     return false;\r\n");
            sb.Append("});\r\n\r\n");

            // CheckBox para Selecionar Todos os Produtos
            sb.Append("$('.selecionaTodosProdutos').off('change').on('change', function(e) {\r\n");
            sb.Append("     var $this = $(this).find('input');\r\n");
            sb.Append("     var validado = $this.is(':checked') || false;\r\n");
            sb.Append("     var table = $this.closest('table');\r\n");
            sb.Append("     $(table).find('tr').each(function () {\r\n");
            sb.Append("         var $row = $(this);\r\n");
            sb.Append("         if ($row.find('[id*=cbExcluir_Produto]').length > 0) {\r\n");
            sb.Append("             $row.find('[id*=cbExcluir_Produto]').prop('checked', validado);\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n");
            sb.Append("});\r\n\r\n");

            // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- //

            // Validar Serviço único + Composição
            sb.AppendLine("$('[id*=cbValidado]').off('change').on('change', function (e) {");
            sb.AppendLine("     e.stopPropagation();");
            sb.AppendLine("     var $row = $(this).closest('tr');");
            sb.AppendLine("     var id = (!$row.hasClass('comp') ? 'tr_1' : $row.hasClass('comp1') ? 'tr_2' : $row.hasClass('comp2') ? 'tr_3' : '') + '_Servico-' + $row.find('.idItem').text() + '-' + $row.find('.idPais').text().split('-')[0] + '-' + $row.find('.idPais').text().split('-')[1] + '-' + $row.find('.idPais').text().split('-')[2];");
            sb.AppendLine("     var cbValidaTodos_Composicao = $row.closest('table').find('#' + id).find('tr').first().find('[id*=cbValida_Todos]');");
            sb.AppendLine("     if ($(this).is(\":checked\")) {");
            sb.AppendLine("         $row.addClass(\"success\").removeClass(\"danger\");");
            sb.AppendLine("         cbValidaTodos_Composicao.prop('checked', true);");
            sb.AppendLine("         cbValidaTodos_Composicao.trigger('change');");
            sb.AppendLine("         $row.find('.qtd').trigger('input');");
            sb.AppendLine("     } else {");
            sb.AppendLine("         $row.addClass(\"danger\").removeClass(\"success\");");
            sb.AppendLine("         cbValidaTodos_Composicao.prop('checked', false);");
            sb.AppendLine("         cbValidaTodos_Composicao.trigger('change');");
            sb.AppendLine("         $row.find('.qtd').trigger('input');");
            sb.AppendLine("     }");
            sb.AppendLine("});");

            sb.AppendLine();

            // Validar Todos os Serviços
            sb.AppendLine("$('[id*=cbValida_Todos]').off('change').on('change', function () {");
            sb.AppendLine("     var validado = $(this).is(\":checked\");");
            sb.AppendLine("     var table = $(this).closest('table');");
            sb.AppendLine("     $(table).find('tr').each(function () {");
            sb.AppendLine("         var $row = $(this);");
            sb.AppendLine("         if ($row.find('[id*=cbValidado]').length > 0) {");
            sb.AppendLine("             if (validado) {");
            sb.AppendLine("                 $row.find('[id*=cbValidado]').prop('checked', true);");
            sb.AppendLine("                 if (!$row.hasClass('collapsed-row')) {");
            sb.AppendLine("                     $row.addClass(\"success\").removeClass(\"danger\");");
            sb.AppendLine("                     $row.find('.qtd').trigger('input');");
            sb.AppendLine("                 }");
            sb.AppendLine("             } else {");
            sb.AppendLine("                 $row.find('[id*=cbValidado]').prop('checked', false);");
            sb.AppendLine("                 if (!$row.hasClass('collapsed-row')) {");
            sb.AppendLine("                     $row.addClass(\"danger\").removeClass(\"success\");");
            sb.AppendLine("                     $row.find('.qtd').trigger('input');");
            sb.AppendLine("                 }");
            sb.AppendLine("             }");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("});\r\n");

            sb.AppendLine();

            // Função para Selecionar Todos, em Comparativos de Produtos
            sb.Append("$('[id*=cbComparativo_Todos]').off('change').on('change', function () {\r\n");
            sb.Append("     var validado = $(this).is(\":checked\");\r\n");
            sb.Append("     var table = $(this).closest('table');\r\n");
            sb.Append("     $(table).find('tr').each(function () {\r\n");
            sb.Append("         var $row = $(this);\r\n");
            sb.Append("         if ($row.find('[id*=cbComparativo]').length > 0) {\r\n");
            sb.Append("             if (validado) {\r\n");
            sb.Append("                 $row.find('[id*=cbComparativo]').prop('checked', true);\r\n");
            sb.Append("             }\r\n");
            sb.Append("             else {\r\n");
            sb.Append("                 $row.find('[id*=cbComparativo]').prop('checked', false);\r\n");
            sb.Append("             }\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n");
            sb.Append("});\r\n\r\n");

            // Função para permitir apenas que uma Opção seja selecionada por Pergunta e salvar as Opções selecionadas
            sb.Append("$('[id*=cb_Opcao]').off('change').on('change', function () {\r\n");
            sb.Append("     if ($(this).is(\":checked\")) {\r\n");
            sb.Append("         $(this).closest('tr').find('[id*=cb_Opcao]').not(this).prop('checked', false);\r\n");
            sb.Append("         $('[id*=hddOpcaoSelecionada]').val($('[id*=hddOpcaoSelecionada]').val() + '[' + $(this).closest('tr').find('.escopo').text() + '|' + $(this).closest('tr').find('.categoria').text() + '|' + $(this).closest('tr').attr('id') + ';' + " +
                                "$(this).closest('tr').find('.pergunta').text() + '|' + $(this).attr('id').split('|')[1] + '|' + $(this).attr('id').split('|')[2] + ']');\r\n");
            sb.Append("         if ($('[id*=hddOpcaoSelecionada]').val().length > 2000) {\r\n");
            sb.Append("             $('[id*=hddOpcaoSelecionada_Focus]').val($(this).closest('tr').find('.pergunta').attr('id'));\r\n");
            sb.Append("             __doPostBack(\"funcao_ATUALIZA_OPCAO\", \"\");\r\n");
            sb.Append("         }\r\n");
            sb.Append("     }\r\n");
            sb.Append("     else {\r\n");
            sb.Append("         $(this).prop('checked', true);\r\n");
            sb.Append("     }\r\n");
            sb.Append("});\r\n\r\n");

            // Função para selecionar Todas as Opções da mesma coluna e salvar as Opções selecionadas
            sb.Append("$('[id*=cb_Opcoes_Todos]').off('change').on('change', function () {\r\n");
            sb.Append("     if ($(this).is(\":checked\")) {\r\n");
            sb.Append("         var table = $(this).closest('table');\r\n");
            sb.Append("         var id = $(this).attr('id').split('|')[1];\r\n");
            sb.Append("         var opcao = '[id*=cb_Opcao_' + id + ']';\r\n");
            sb.Append("         var focus = $(table).attr('id');\r\n");
            sb.Append("         $(this).closest('tr').find('[id*=cb_Opcoes_Todos]').not(this).prop('checked', false);\r\n");
            sb.Append("         $(table).find('tr').each(function () {\r\n");
            sb.Append("             var $row = $(this);\r\n");
            sb.Append("             var cbOpcao = $row.find(opcao);\r\n");
            sb.Append("             if (cbOpcao.length > 0) {\r\n");
            sb.Append("                 cbOpcao.prop('checked', true);\r\n");
            sb.Append("                 $row.find('[id*=cb_Opcao]').not(cbOpcao).prop('checked', false);\r\n");
            sb.Append("                 $('[id*=hddOpcaoSelecionada]').val($('[id*=hddOpcaoSelecionada]').val() + '[' + $(this).find('.escopo').text() + '|' + $(this).find('.categoria').text() + '|' + $(this).attr('id') + ';' + " +
                                        "$(this).find('.pergunta').text() + '|' + $(cbOpcao).attr('id').split('|')[1] + '|' + $(cbOpcao).attr('id').split('|')[2] + ']');\r\n");
            sb.Append("                 focus = $(this).find('.pergunta').attr('id');\r\n");
            sb.Append("             }\r\n");
            sb.Append("         });\r\n");
            sb.Append("         if ($('[id*=hddOpcaoSelecionada]').val().length > 2000) {\r\n");
            sb.Append("             $('[id*=hddOpcaoSelecionada_Focus]').val(focus);\r\n");
            sb.Append("             __doPostBack(\"funcao_ATUALIZA_OPCAO\", \"\");\r\n");
            sb.Append("         }\r\n");
            sb.Append("     }\r\n");
            sb.Append("});\r\n\r\n");

            // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
            // Máscaras

            sb.Append("$('.ajuste, .valor, .total, .total_st, .totalGeral, [id*=txtFrete], [id*=nTotalServicos], [id*=nMargem]').mask('000.000.009,99', { reverse: true });\r\n");

            sb.Append("if ($('[id*=txtDiasPrevisao]').length > 0) {\r\n");
            sb.Append("     $('[id*=txtDiasPrevisao]').mask('0000', { reverse: true });\r\n");
            sb.Append("}\r\n");
            sb.Append("if ($('[id*=txtValidade]').length > 0) {\r\n");
            sb.Append("     $('[id*=txtValidade]').mask('0000', { reverse: true });\r\n");
            sb.Append("}\r\n");
            sb.Append("if ($('[id*=txtPrazo]').length > 0) {\r\n");
            sb.Append("     $('[id*=txtPrazo]').mask('0000', { reverse: true });\r\n");
            sb.Append("}\r\n");
            sb.Append("if ($('.prazo').length > 0) {\r\n");
            sb.Append("     $('.prazo').mask('0000', { reverse: true });\r\n");
            sb.Append("}\r\n");
            sb.Append("if ($('[id*=nOrdem]').length > 0) {\r\n");
            sb.Append("     $('[id*=nOrdem]').mask('0000', { reverse: true });\r\n");
            sb.Append("}\r\n");
            sb.Append("if ($('[id*=txtQtdParcelas_Nova_CondicaoPagamento]').length > 0) {\r\n");
            sb.Append("     $('[id*=txtQtdParcelas_Nova_CondicaoPagamento]').mask('000', { reverse: true });\r\n");
            sb.Append("}\r\n");
            sb.Append("if ($('.ddlParcela').length > 0) {\r\n");
            sb.Append("     $('.ddlParcela').mask('000', { reverse: true });\r\n");
            sb.Append("}\r\n");
            sb.Append("if ($('.porcentagemParcela').length > 0) {\r\n");
            sb.Append("     $('.porcentagemParcela').mask('000,00', { reverse: true });\r\n");
            sb.Append("}\r\n");
            sb.Append("if ($('[id*=txtNumero_NovoEndereco]').length > 0) {\r\n");
            sb.Append("     $('[id*=txtNumero_NovoEndereco]').mask('000000000000', { reverse: true });\r\n");
            sb.Append("}\r\n");
            sb.Append("if ($('[id*=txtCEP_NovoEndereco]').length > 0) {\r\n");
            sb.Append("     $('[id*=txtCEP_NovoEndereco]').mask('99999-999');\r\n");
            sb.Append("}\r\n\r\n");

            // Função para que os links abram em uma nova aba
            sb.Append("$('.link').find('a').click(function() { window.open(window.location.protocol + '//' + window.location.host + '/App/Paginas' + $(this).attr('href'), '_blank'); return false; });\r\n\r\n");

            string descontoMaximo_s = nMax_Desconto_Servicos.ToString();

            // Cálculo Individual Serviços
            sb.Append("$(document).off('input.servico', 'input[id*=nDesconto], input[id*=nQtd]').on('input.servico', 'input[id*=nDesconto], input[id*=nQtd]', function () {\r\n");
            sb.Append("     var $row = $(this).closest('tr');\r\n");
            sb.Append("     if ($row.find('input[id*=nDesconto]').length > 0) {\r\n");
            sb.Append("         var nDesconto = parseFloat($row.find('input[id*=nDesconto]').val().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("         if (nDesconto > parseFloat(" + descontoMaximo_s + ")) {\r\n");
            sb.Append("             nDesconto = parseFloat(" + descontoMaximo_s + ")\r\n");
            sb.Append("             $row.find('input[id*=nDesconto]').val(nDesconto.toFixed(2).replace('.', ','));\r\n");
            sb.Append("         } else if ($row.find('input[id*=nDesconto]').val().length > 0 && $row.find('input[id*=nDesconto]').val() != '-' && isNaN(nDesconto)) {\r\n");
            sb.Append("             nDesconto = parseFloat(0);\r\n");
            sb.Append("             $row.find('input[id*=nDesconto]').val(nDesconto.toFixed(2).replace('.', ','));\r\n");
            sb.Append("         } else if (/[^\\d,.-]/.test($row.find('input[id*=nDesconto]').val())) {\r\n");
            sb.Append("             $row.find('input[id*=nDesconto]').val(nDesconto.toFixed(2).replace('.', ','));\r\n");
            sb.Append("         }\r\n");
            sb.Append("         var valor = parseFloat($row.find('.valor').text().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("         var qtd = parseFloat($row.find('.qtd').val().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("         var margem = parseFloat($row.find('.margem').text().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("         var ajuste = parseFloat($row.find('.ajuste').text().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("         var total = isNaN(valor) ? 0 : (valor * qtd * margem) - (valor * qtd * margem * (nDesconto / 100));\r\n");
            sb.Append("         if ($row.find('[id*=iconeServico_Comparativo]').hasClass('fa-plus')) {\r\n");
            sb.Append("             total = (total * (ajuste / 100)) + total;\r\n");
            sb.Append("         } else {\r\n");
            sb.Append("             total = total - (total * (ajuste * -1 / 100));\r\n");
            sb.Append("         }\r\n");
            sb.Append("         var cambio = parseFloat(($('.nCambio').val() || '').replace(/\\./g, '').replace(',', '.')) || 0;\r\n");
            sb.Append("         console.log('Total atual: ' + $row.find('.total').text() + ' | Total calculado: ' + FormatarValor(total, 2) + ' | Câmbio: ' + FormatarValor(cambio, 4), $row);\r\n");
            sb.Append("         total = cambio > 0 ? total * cambio : total;\r\n");
            sb.Append("         $row.find('.total').text(FormatarValor(total, 2));\r\n");
            sb.Append("     }\r\n");
            sb.Append("});\r\n\r\n");

            // Cálculos - Sub-Serviços & Recursos
            sb.Append("$('.valor, [id*=nMargem], input[id*=nQtd]').off('input').on('input', function () {\r\n");
            sb.Append("     var $row = $(this).closest('tr');\r\n");
            sb.Append("     if ($row.find('[id*=nMargem]').length > 0 && $row.find('input[id*=nDesconto]').length <= 0) {\r\n");
            sb.Append("         var nMargem = parseFloat($row.find('[id*=nMargem]').val().replace(/\\./g, '').replace(',', '.').length <= 0 ? $row.find('[id*=nMargem]').text().replace(/\\./g, '').replace(',', '.') : $row.find('[id*=nMargem]').val().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("         nMargem = nMargem.length <= 0 ? 0 : nMargem;\r\n");
            sb.Append("         var nQtd = parseFloat($row.find('input[id*=nQtd]').val().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("         var valor = parseFloat($row.find('.valor').val().replace(/\\./g, '').replace(',', '.').length <= 0 ? $row.find('.valor').text().replace(/\\./g, '').replace(',', '.') : $row.find('.valor').val().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("         valor = valor.length <= 0 ? 0 : valor;\r\n");
            sb.Append("         var total = isNaN(nQtd) || isNaN(valor) ? 0 : isNaN(nMargem) ? valor * nQtd : valor * nMargem * nQtd;\r\n");
            sb.Append("         $row.find('.total').text(FormatarValor(total, 2));\r\n\r\n");

            sb.Append("         var $table = $row.closest('table');\r\n");
            sb.Append("         var $rowClass = $row.hasClass('comp1') ? 'comp1' : $row.hasClass('comp2') ? 'comp2' : $row.hasClass('comp3') ? 'comp3' : 'comp';\r\n");
            sb.Append("         var idPai = $row.find('.idPais').text().split('-')[0];\r\n");
            sb.Append("         var idAvo = $row.find('.idPais').text().split('-')[1];\r\n");
            sb.Append("         var idBisavo = $row.find('.idPais').text().split('-')[2];\r\n");
            sb.Append("         var valor_Composicao = 0;\r\n");
            sb.Append("         var total_Composicao = 0;\r\n");
            sb.Append("         $('#' + $table.attr('id') + ' .qtd').each(function () {\r\n");
            sb.Append("             $row = $(this).closest('tr');\r\n");
            sb.Append("             if ($row.hasClass('success') && $row.hasClass($rowClass)) {\r\n");
            sb.Append("                 valor_Composicao += $row.find('.valor').text().length > 0 ? parseFloat($row.find('.valor').text().replace(/\\./g, '').replace(',', '.')) : parseFloat($row.find('.valor').val().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("                 total_Composicao += parseFloat($row.find('.total').text().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("             }\r\n");
            sb.Append("         });\r\n");
            sb.Append("         if ($table.attr('id').includes('_Composicao_3')) {\r\n");
            sb.Append("             var $table_RecursoPai = $table.parents('table').first();\r\n");
            sb.Append("             $('#' + $table_RecursoPai.attr('id') + ' .qtd').each(function () {\r\n");
            sb.Append("                 $row = $(this).closest('tr');\r\n");
            sb.Append("                 if ($row.hasClass('success') && $row.hasClass('comp2')) {\r\n");
            sb.Append("                     if ($row.find('.idItem').text() == idPai && $row.find('.idPais').text().split('-')[0] == idAvo && $row.find('.idPais').text().split('-')[1] == idBisavo) {\r\n");
            sb.Append("                         $row.find('[id*=hddPreco]').val(total_Composicao.toFixed(2).replace('.', ','));\r\n");
            sb.Append("                         $row.find('.valor').val(FormatarValor(total_Composicao, 2));\r\n");
            sb.Append("                     }\r\n");
            sb.Append("                 }\r\n");
            sb.Append("             });\r\n");
            sb.Append("         } else if ($table.attr('id').includes('_Composicao_2')) {\r\n");
            sb.Append("             var $table_SubServicos = $table.parents('table').first();\r\n");
            sb.Append("             $('#' + $table_SubServicos.attr('id') + ' .qtd').each(function () {\r\n");
            sb.Append("                 $row = $(this).closest('tr');\r\n");
            sb.Append("                 if ($row.hasClass('success') && $row.hasClass('comp1')) {\r\n");
            sb.Append("                     if ($row.find('.idItem').text() == idPai && $row.find('.idPais').text().split('-')[0] == idAvo && $row.find('.idPais').text().split('-')[1] == idBisavo) {\r\n");
            sb.Append("                         var margem = valor_Composicao > 0 && total_Composicao > 0 ? total_Composicao / valor_Composicao : 0;\r\n");
            sb.Append("                         $row.find('[id*=nMargem]').text(FormatarValor(margem, 2));\r\n");
            sb.Append("                         $row.find('[id*=hddMargem]').val(margem.toFixed(2).replace('.', ','));\r\n");
            sb.Append("                         $row.find('[id*=hddPreco]').val(valor_Composicao.toFixed(2).replace('.', ','));\r\n");
            sb.Append("                         $row.find('.valor').text(FormatarValor(valor_Composicao, 2));\r\n");
            sb.Append("                         $row.find('.total').text(FormatarValor(total_Composicao, 2));\r\n");
            sb.Append("                         $row.find('input[id*=nQtd]').trigger('input');\r\n");
            sb.Append("                     }\r\n");
            sb.Append("                 }\r\n");
            sb.Append("             });\r\n");
            sb.Append("         } else if ($table.attr('id').includes('_Composicao_1')) {\r\n");
            sb.Append("             var $table_Servicos = $table.parents('table').first();\r\n");
            sb.Append("             $('#' + $table_Servicos.attr('id') + ' .qtd').each(function () {\r\n");
            sb.Append("                 $row = $(this).closest('tr');\r\n");
            sb.Append("                 if ($row.hasClass('success') && !$row.hasClass('comp')) {\r\n");
            sb.Append("                     if ($row.find('.idItem').text() == idPai && $row.find('.idPais').text().split('-')[0] == idAvo && $row.find('.idPais').text().split('-')[1] == idBisavo) {\r\n");
            sb.Append("                         var margem = valor_Composicao > 0 && total_Composicao > 0 ? total_Composicao / valor_Composicao : 0;\r\n");
            sb.Append("                         $row.find('[id*=nMargem]').text(FormatarValor(margem, 2));\r\n");
            sb.Append("                         $row.find('[id*=hddMargem]').val(margem.toFixed(2).replace('.', ','));\r\n");
            sb.Append("                         $row.find('[id*=hddPreco]').val(valor_Composicao.toFixed(2).replace('.', ','));\r\n");
            sb.Append("                         $row.find('.valor').text(FormatarValor(valor_Composicao, 2));\r\n");
            sb.Append("                         $row.find('.total').text(FormatarValor(total_Composicao, 2));\r\n");
            sb.Append("                         $row.find('input[id*=nQtd]').trigger('input');\r\n");
            sb.Append("                     }\r\n");
            sb.Append("                 }\r\n");
            sb.Append("             });\r\n");
            sb.Append("         }\r\n");
            sb.Append("     }\r\n");
            sb.Append("});\r\n\r\n");

            // Cálculo Individual Produtos
            sb.Append("$('[id*=nDesc_Produtos], input[id*=nQuantidade_Produtos], [id*=nUnitario]').off('input').on('input', function () {\r\n");
            sb.Append("     var $row = $(this).closest('tr');\r\n");
            sb.Append("     var $this = $(this);\r\n");
            sb.Append("     var valor = parseFloat($row.find('.valor').text().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("     var qtd = parseFloat($row.find('.qtd').val().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("     var nDesconto = parseFloat($row.find('[id*=nDesc_Produtos]').val().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("     var unitario = parseFloat($row.find('[id*=nUnitario]').val().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("     if ($row.find('[id*=nDesc_Produtos]').val().length > 0 && $row.find('[id*=nDesc_Produtos]').val() != '-' && isNaN(nDesconto)) {\r\n");
            sb.Append("         nDesconto = parseFloat(0);\r\n");
            sb.Append("         $row.find('[id*=nDesc_Produtos]').val(nDesconto.toFixed(4).replace('.', ','));\r\n");
            sb.Append("     }\r\n");
            sb.Append("     if ($this.hasClass('unitario')) {\r\n");
            sb.Append("         nDesconto = ((valor - unitario) / valor) * 100;\r\n");
            sb.Append("         $row.find('[id*=nDesc_Produtos]').val(nDesconto.toFixed(4).replace('.', ','));\r\n");
            sb.Append("     }\r\n");
            sb.Append("     unitario = valor - (valor * (nDesconto / 100));\r\n");
            sb.Append("     var total = isNaN(valor) ? 0 : unitario * qtd;\r\n");
            sb.Append("     var cambio = parseFloat(($('.nCambio').val() || '').replace(/\\./g, '').replace(',', '.')) || 0;\r\n");
            sb.Append("     total = cambio > 0 ? total * cambio : total;\r\n");
            sb.Append("     $row.find('[id*=nUnitario]').val(unitario.toFixed(2).replace('.', ','));\r\n");
            sb.Append("     $row.find('.total').text(FormatarValor(total, 2));\r\n");
            sb.Append("});\r\n\r\n");

            // Manipulação Individual do Prazo
            sb.Append(" $('[id*=nPrazo]').off('input').on('input', function () {\r\n");
            sb.Append("     var $row = $(this).closest('tr');\r\n");
            sb.Append("     var txtPrazo = $row.find('[id*=nMargem]').length > 0 ? $('[id*=txtPrazoServico]').val() : $('[id*=txtPrazoProduto]').val();\r\n");
            sb.Append("     var prazo = $row.find('[id*=nPrazo]').val();\r\n");
            sb.Append("     prazo = isNaN(prazo) || parseInt(prazo) < 0 ? txtPrazo : prazo;\r\n");
            sb.Append("     $row.find('[id*=nPrazo]').val(prazo);\r\n");
            sb.Append(" });\r\n\r\n");

            // Aplica Margem Global no Contrle de Margem
            sb.Append("if ($('[id*=txtMargemServico]').length > 0) {\r\n");
            sb.Append("     $('[id*=txtMargemServico]').off('input').on('input', function () {\r\n");
            sb.Append("         var nMargem = parseFloat($(this).val().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("         nMargem = $(this).val().length <= 0 ? 0 : nMargem;\r\n");
            sb.Append("         $('[id*=gvControle_Margem] .margemGlobal').each(function () {\r\n");
            sb.Append("             var $row = $(this).closest('tr');\r\n");
            sb.Append("             $row.find('.margemGlobal').val(nMargem.toFixed(2).replace('.', ','));\r\n");
            sb.Append("         });\r\n");
            sb.Append("     })\r\n;");
            sb.Append("}\r\n\r\n");

            // Cálculo Desconto Global Serviços
            sb.Append("if ($('[id*=txtDescontoServico]').length > 0) {\r\n");
            sb.Append("     $('[id*=txtDescontoServico]').off('input').on('input', function () {\r\n");
            sb.Append("         var nDesconto = parseFloat($(this).val().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("         if (nDesconto > parseFloat(" + descontoMaximo_s + ")) {\r\n");
            sb.Append("             nDesconto = parseFloat(" + descontoMaximo_s + ")\r\n");
            sb.Append("             $(this).val(nDesconto.toFixed(2).replace('.', ','));\r\n");
            sb.Append("         }\r\n");
            sb.Append("         else if ($(this).val().length > 0 && $(this).val() != '-' && isNaN(nDesconto)) {\r\n");
            sb.Append("             nDesconto = parseFloat(0);\r\n");
            sb.Append("             $(this).val(nDesconto.toFixed(2).replace('.', ','));\r\n");
            sb.Append("         }\r\n");
            sb.Append("         else if (/[^\\d,.-]/.test($(this).val())) {\r\n");
            sb.Append("             $(this).val(nDesconto.toFixed(2).replace('.', ','));\r\n");
            sb.Append("         }\r\n");
            sb.Append("         $('[id*=gvServicos_Recursos] input[id*=nDesconto]').each(function () {\r\n");
            sb.Append("             var $row = $(this).closest('tr');\r\n");
            sb.Append("             var valor = parseFloat($row.find('.valor').text().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("             var qtd = parseFloat($row.find('.qtd').val().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("             var nMargem = parseFloat($row.find('[id*=nMargem]').text().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("             var ajuste = parseFloat($row.find('.ajuste').text().replace(/\\./g, '').replace(',', '.'));\r\n");
            sb.Append("             var total = isNaN(valor) ? 0 : (valor * qtd * nMargem) - ((valor * qtd * nMargem) * (nDesconto / 100));\r\n");
            sb.Append("             if ($row.find('[id*=iconeServico_Comparativo]').hasClass('fa-plus')) {\r\n");
            sb.Append("                 total = (total * (ajuste / 100)) + total;\r\n");
            sb.Append("             }\r\n");
            sb.Append("             else {\r\n");
            sb.Append("                 total = total - (total * (ajuste / 100));\r\n");
            sb.Append("             }\r\n");
            sb.Append("             var cambio = parseFloat(($('.nCambio').val() || '').replace(/\\./g, '').replace(',', '.')) || 0;\r\n");
            sb.Append("             total = cambio > 0 ? total * cambio : total;\r\n");
            sb.Append("             $row.find('.total').text(FormatarValor(total, 2));\r\n");
            sb.Append("             $row.find('input[id*=nDesconto]').val(nDesconto.toFixed(2).replace('.', ','));\r\n");
            sb.Append("         });\r\n");
            sb.Append("     })\r\n;");
            sb.Append("}\r\n\r\n");

            // Aplica Prazo Global em Serviços
            sb.Append("if ($('[id*=txtPrazoServico]').length > 0) {\r\n");
            sb.Append("     $('[id*=txtPrazoServico]').off('input').on('input', function () {\r\n");
            sb.Append("         var nPrazo = $(this).val();\r\n");
            sb.Append("         nPrazo = isNaN(nPrazo) || parseInt(nPrazo) < 0 ? 15 : nPrazo;\r\n");
            sb.Append("         $(this).val(nPrazo);\r\n");
            sb.Append("         $('[id*=gvServicos_Recursos] [id*=nPrazo]').each(function () {\r\n");
            sb.Append("             $(this).closest('tr').find('[id*=nPrazo]').val(nPrazo);\r\n");
            sb.Append("         });\r\n");
            sb.Append("     });\r\n");
            sb.Append("}\r\n\r\n");

            // Aplica Prazo Global em Produtos
            sb.Append("if ($('[id*=txtPrazoProduto]').length > 0) {\r\n");
            sb.Append("     $('[id*=txtPrazoProduto]').off('input').on('input', function () {\r\n");
            sb.Append("         var nPrazo = $(this).val();\r\n");
            sb.Append("         nPrazo = isNaN(nPrazo) || parseInt(nPrazo) < 0 ? 15 : nPrazo;\r\n");
            sb.Append("         $(this).val(nPrazo);\r\n");
            sb.Append("         $('[id*=gvProdutos] [id*=nPrazo]').each(function () {\r\n");
            sb.Append("             $(this).closest('tr').find('[id*=nPrazo]').val(nPrazo);\r\n");
            sb.Append("         });\r\n");
            sb.Append("     });\r\n");
            sb.Append("}\r\n\r\n");

            // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- //

            // Manipulação do Prazo de Produtos - Comparativos
            sb.Append("$('[id*=prazo]').off('input').on('input', function () {\r\n");
            sb.Append("     var $row = $(this).closest('tr');\r\n");
            sb.Append("     var txtPrazo = $('[id*=txtPrazo_Global_Produtos_Comparativos]').val();\r\n");
            sb.Append("     var prazo = $row.find('[id*=prazo]').val();\r\n");
            sb.Append("     prazo = isNaN(prazo) || parseInt(prazo) < 0 ? txtPrazo : prazo;\r\n");
            sb.Append("     $row.find('[id*=prazo]').val(prazo);\r\n");
            sb.Append("});\r\n\r\n");

            // Aplica Prazo Global em Produtos - Comparativos
            sb.Append("if ($('[id*=txtPrazo_Global_Produtos_Comparativos]').length > 0) {\r\n");
            sb.Append("     $('[id*=txtPrazo_Global_Produtos_Comparativos]').off('input').on('input', function () {\r\n");
            sb.Append("         var nPrazo = $(this).val();\r\n");
            sb.Append("         nPrazo = isNaN(nPrazo) || parseInt(nPrazo) < 0 ? 15 : nPrazo;\r\n");
            sb.Append("         $(this).val(nPrazo);\r\n");
            sb.Append("         $('[id*=gvComparativoProdutos] [id*=prazo]').each(function () {\r\n");
            sb.Append("             $(this).closest('tr').find('[id*=prazo]').val(nPrazo);\r\n");
            sb.Append("         });\r\n");
            sb.Append("     });\r\n");
            sb.Append("}\r\n\r\n");

            // Aplica Prazo Global em Serviços - Empreitada
            sb.Append("if ($('[id*=txtPrazo_Global_Servicos_Empreitada]').length > 0) {\r\n");
            sb.Append("     $('[id*=txtPrazo_Global_Servicos_Empreitada]').off('input').on('input', function () {\r\n");
            sb.Append("         var nPrazo = $(this).val();\r\n");
            sb.Append("         nPrazo = isNaN(nPrazo) || parseInt(nPrazo) < 0 ? 15 : nPrazo;\r\n");
            sb.Append("         $(this).val(nPrazo);\r\n");
            sb.Append("         $('[id*=gvComparativoServicos] [id*=prazo]').each(function () {\r\n");
            sb.Append("             $(this).closest('tr').find('[id*=prazo]').val(nPrazo);\r\n");
            sb.Append("         });\r\n");
            sb.Append("     });\r\n");
            sb.Append("}\r\n\r\n");

            // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- //

            // Aplica Quantidade de Parcelas - Modal Nova Condição de Pagamento Personalizada
            sb.Append("if ($('[id*=txtQtdParcelas_Nova_CondicaoPagamento]').length > 0) {\r\n");
            sb.Append("     var qtdAlterada = false;\r\n");
            sb.Append("     $('[id*=txtQtdParcelas_Nova_CondicaoPagamento]').off('input').on('input', function () {\r\n");
            sb.Append("         qtdAlterada = true;\r\n");
            sb.Append("     });\r\n");
            sb.Append("     $('[id*=txtQtdParcelas_Nova_CondicaoPagamento]').off('blur').on('blur', function () {\r\n");
            sb.Append("         if (qtdAlterada) {\r\n");
            sb.Append("             var qtd = parseInt($(this).val() || '0');\r\n");
            sb.Append("             var $table = $('[id*=tblParcelas_Nova_CondicaoPagamento]');\r\n");
            sb.Append("             var $tbody = $table.find('tbody.bodyReal');\r\n");
            sb.Append("             $tbody.empty();\r\n");
            sb.Append("             if (qtd > 0) {\r\n");
            sb.Append("                 $table.removeClass('invisivel');\r\n");
            sb.Append("                 $('[id*=cmdNovaCondicaoPagamento_Personalizada]').removeClass('invisivel');\r\n");
            sb.Append("                 var $tbody_Modelo = $table.find('tbody.bodyModelo');\r\n");
            sb.Append("                 for (var i = 1; i <= qtd; i++) {\r\n");
            sb.Append("                     var $modelo = $tbody_Modelo.find('.tr_Modelo').clone();\r\n");
            sb.Append("                     $modelo.removeClass().addClass('tr_Real tr_Real_' + i);\r\n");
            sb.Append("                     $tbody.append($modelo);\r\n");
            sb.Append("                     if (i == qtd) {\r\n");
            sb.Append("                         var $botao = $tbody_Modelo.find('.tr_Botao').clone();\r\n");
            sb.Append("                         $botao.removeClass().addClass('tr_Botao_' + i);\r\n");
            sb.Append("                         $tbody.append($botao);\r\n");
            sb.Append("                     }\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 $table.find('tbody.bodyReal tr:first').find('.tipoParcela').focus();\r\n");
            sb.Append("             } else {\r\n");
            sb.Append("                 $(this).focus();\r\n");
            sb.Append("                 $table.addClass('invisivel');\r\n");
            sb.Append("                 $('[id*=cmdNovaCondicaoPagamento_Personalizada]').addClass('invisivel');\r\n");
            sb.Append("             }\r\n");
            sb.Append("             qtdAlterada = false;\r\n");
            sb.Append("             $table.find('div.tooltip').css('display', 'none');\r\n");
            sb.Append("             $table.find('[data-toggle=tooltip]').tooltip();\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n\r\n");

            sb.Append("     $('[id*=tblParcelas_Nova_CondicaoPagamento]').off('click', '[id*=lnkAdicionarParcela]').on('click', '[id*=lnkAdicionarParcela]', function (e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         var qtd = parseInt($('[id*=txtQtdParcelas_Nova_CondicaoPagamento]').val() || '0') + 1;\r\n");
            sb.Append("         $('[id*=txtQtdParcelas_Nova_CondicaoPagamento]').val(qtd);\r\n");
            sb.Append("         var $table = $('[id*=tblParcelas_Nova_CondicaoPagamento]');\r\n");
            sb.Append("         var $tbody = $table.find('tbody.bodyReal');\r\n");
            sb.Append("         $tbody.find('tr:last').remove();\r\n");
            sb.Append("         $table.removeClass('invisivel');\r\n");
            sb.Append("         var $tbody_Modelo = $table.find('tbody.bodyModelo');\r\n");
            sb.Append("         var $modelo = $tbody_Modelo.find('.tr_Modelo').clone();\r\n");
            sb.Append("         var $botao = $tbody_Modelo.find('.tr_Botao').clone();\r\n");
            sb.Append("         $modelo.removeClass().addClass('tr_Real tr_Real_' + qtd);\r\n");
            sb.Append("         $tbody.append($modelo);\r\n");
            sb.Append("         $tbody.append($botao);\r\n");
            sb.Append("         $table.find('div.tooltip').css('display', 'none');\r\n");
            sb.Append("         $table.find('[data-toggle=tooltip]').tooltip();\r\n");
            sb.Append("     });\r\n\r\n");

            sb.Append("     $('[id*=tblParcelas_Nova_CondicaoPagamento]').off('click', '[id*=lnkExcluirParcela]').on('click', '[id*=lnkExcluirParcela]', function (e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         var qtd = parseInt($('[id*=txtQtdParcelas_Nova_CondicaoPagamento]').val() || '0') - 1;\r\n");
            sb.Append("         $('[id*=txtQtdParcelas_Nova_CondicaoPagamento]').val(qtd);\r\n");
            sb.Append("         var $table = $('[id*=tblParcelas_Nova_CondicaoPagamento]');\r\n");
            sb.Append("         $table.find('tbody.bodyReal').find($(this).closest('tr')).remove();\r\n");
            sb.Append("         $table.find('div.tooltip').css('display', 'none');\r\n");
            sb.Append("         $table.find('[data-toggle=tooltip]').tooltip();\r\n");
            sb.Append("         if ($table.find('tbody.bodyReal tr').length > 0) {\r\n");
            sb.Append("             $table.removeClass('invisivel');\r\n");
            sb.Append("             $('[id*=cmdNovaCondicaoPagamento_Personalizada]').removeClass('invisivel');\r\n");
            sb.Append("         } else {\r\n");
            sb.Append("             $table.addClass('invisivel');\r\n");
            sb.Append("             $('[id*=cmdNovaCondicaoPagamento_Personalizada]').addClass('invisivel');\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n\r\n");

            sb.Append("     $('[id*=cmdNovaCondicaoPagamento_Personalizada]').off('click').on('click', function () {\r\n");
            sb.Append("         var dados_JSON = [];\r\n");
            sb.Append("         var linhas = $('[id*=tblParcelas_Nova_CondicaoPagamento]').find('tbody.bodyReal tr.tr_Real');\r\n");
            sb.Append("         $(linhas).each(function() {\r\n");
            sb.Append("             var tipo = $(this).find('.tipoParcela').val();\r\n");
            sb.Append("             var porcentagem = $(this).find('.porcentagemParcela').val();\r\n");
            sb.Append("             var ddl = $(this).find('.ddlParcela').val();\r\n\r\n");

            sb.Append("             dados_JSON.push({\r\n");
            sb.Append("                 tipo: tipo,\r\n");
            sb.Append("                 porcentagem: porcentagem,\r\n");
            sb.Append("                 ddl: ddl\r\n");
            sb.Append("             });\r\n\r\n");

            sb.Append("             $(this).find('.tipoParcela').val('1');\r\n");
            sb.Append("             $(this).find('.porcentagemParcela').val('0,00');\r\n");
            sb.Append("             $(this).find('.ddlParcela').val('0');\r\n");
            sb.Append("         });\r\n");
            sb.Append("         $('[id*=hddNovaCondPgto_Pers_Dados]').val(JSON.stringify(dados_JSON));\r\n");
            sb.Append("     });\r\n\r\n");

            sb.Append("}\r\n\r\n");

            // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- //

            sb.Append("$v192(function() {\r\n");

            // Modal de Confirmação - Salvar
            sb.Append("     $v192(\"#dialog-Salvar\").dialog({\r\n");
            sb.Append("         resizable: false,\r\n");
            sb.Append("         height: \"auto\",\r\n");
            sb.Append("         width: 400,\r\n");
            sb.Append("         modal: true,\r\n");
            sb.Append("         autoOpen: false,\r\n");
            sb.Append("         buttons:\r\n");
            sb.Append("         {\r\n");
            sb.Append("             \"Sim\": function() {\r\n");
            sb.Append("                 __doPostBack(\"funcao_SALVAR\", \"\");\r\n");
            sb.Append("                 $v192(this).dialog(\"close\");\r\n");
            sb.Append("             },\r\n");
            sb.Append("             \"Não\": function() {\r\n");
            sb.Append("                 $v192(this).dialog(\"close\");\r\n");
            sb.Append("             },\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n");
            sb.Append("     $v192('[id*=cmdSalvar]').click(function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $('[id*=hdd_ID_cmdSalvar]').val($(this).attr('id'));\r\n");
            sb.Append("         $v192('#dialog-Salvar').dialog('open');\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal de Confirmação - Editar
            sb.Append("     $v192(\"#dialog-Editar\").dialog({\r\n");
            sb.Append("         resizable: false,\r\n");
            sb.Append("         height: \"auto\",\r\n");
            sb.Append("         width: 400,\r\n");
            sb.Append("         modal: true,\r\n");
            sb.Append("         autoOpen: false,\r\n");
            sb.Append("         buttons:\r\n");
            sb.Append("             {\r\n");
            sb.Append("                 \"Sim\": function() {\r\n");
            sb.Append("                     __doPostBack(\"funcao_EDITAR\", \"\");\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("                 \"Não\": function() {\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("             }\r\n");
            sb.Append("     });\r\n");
            sb.Append("     $v192('[id*=cmdEditar]').click(function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $v192('#dialog-Editar').dialog('open');\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal de Confirmação - Aplicar Comparativos
            sb.Append("     $v192(\"#dialog-Aplicar_Comparativos\").dialog({\r\n");
            sb.Append("         resizable: false,\r\n");
            sb.Append("         height: \"auto\",\r\n");
            sb.Append("         width: 400,\r\n");
            sb.Append("         modal: true,\r\n");
            sb.Append("         autoOpen: false,\r\n");
            sb.Append("         buttons:\r\n");
            sb.Append("             {\r\n");
            sb.Append("                 \"Sim\": function() {\r\n");
            sb.Append("                     __doPostBack(\"funcao_APLICAR_COMPARATIVOS\", \"\");\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("                 \"Não\": function() {\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("             }\r\n");
            sb.Append("     });\r\n");
            sb.Append("     $v192('[id*=cmdAplicar_Comparativos]').click(function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $v192('#dialog-Aplicar_Comparativos').dialog('open');\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal de Confirmação - Excluir Todos os Produtos
            sb.Append("     $v192(\"#dialog-ExcluirTodosProdutos\").dialog({\r\n");
            sb.Append("         resizable: false,\r\n");
            sb.Append("         height: \"auto\",\r\n");
            sb.Append("         width: 400,\r\n");
            sb.Append("         modal: true,\r\n");
            sb.Append("         autoOpen: false,\r\n");
            sb.Append("         buttons:\r\n");
            sb.Append("             {\r\n");
            sb.Append("                 \"Sim\": function() {\r\n");
            sb.Append("                     __doPostBack(\"funcao_EXCLUIR_PRODUTOS\", \"\");\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("                 \"Não\": function() {\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("             }\r\n");
            sb.Append("     });\r\n");
            sb.Append("     $v192('.excluirTodosProdutos').click(function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $v192('#dialog-ExcluirTodosProdutos').dialog('open');\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal de Confirmação - Unificar Produtos Duplicados
            sb.Append("     $v192(\"#dialog-UnificarProdutosDuplicados\").dialog({\r\n");
            sb.Append("         resizable: false,\r\n");
            sb.Append("         height: \"auto\",\r\n");
            sb.Append("         width: 400,\r\n");
            sb.Append("         modal: true,\r\n");
            sb.Append("         autoOpen: false,\r\n");
            sb.Append("         buttons:\r\n");
            sb.Append("             {\r\n");
            sb.Append("                 \"Sim\": function() {\r\n");
            sb.Append("                     __doPostBack(\"funcao_UNIFICAR_PRODUTOS\", \"\");\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("                 \"Não\": function() {\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("             }\r\n");
            sb.Append("     });\r\n");
            sb.Append("     $v192('.unificarProduto').click(function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $('[id*=hddUnificarProdutos]').val($(this).closest('tr').find('.idItem').text());\r\n");
            sb.Append("         $('#dialog-UnificarProdutosDuplicados').find('b').text('Deseja unificar todas as duplicatas do Produto de código ' + $(this).closest($('.modaisCodigo')).siblings('.link').find('a').text() + '?');\r\n");
            sb.Append("         $v192('#dialog-UnificarProdutosDuplicados').dialog('open');\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal de Confirmação - Unificar Todos os Produtos Duplicados
            sb.Append("     $v192(\"#dialog-UnificarTodosProdutosDuplicados\").dialog({\r\n");
            sb.Append("         resizable: false,\r\n");
            sb.Append("         height: \"auto\",\r\n");
            sb.Append("         width: 400,\r\n");
            sb.Append("         modal: true,\r\n");
            sb.Append("         autoOpen: false,\r\n");
            sb.Append("         buttons:\r\n");
            sb.Append("             {\r\n");
            sb.Append("                 \"Sim\": function() {\r\n");
            sb.Append("                     __doPostBack(\"funcao_UNIFICAR_TODOS\", \"\");\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("                 \"Não\": function() {\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("             }\r\n");
            sb.Append("     });\r\n");
            sb.Append("     $v192('.unificarTodos').click(function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $('#dialog-UnificarTodosProdutosDuplicados').find('b').text('Deseja unificar todas as duplicatas presentes no Orçamento?');\r\n");
            sb.Append("         $v192('#dialog-UnificarTodosProdutosDuplicados').dialog('open');\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal de Confirmação - Atualizar Serviços e Escopos
            sb.Append("     $v192(\"#dialog-AtualizarServicos_Escopos\").dialog({\r\n");
            sb.Append("         resizable: false,\r\n");
            sb.Append("         height: \"auto\",\r\n");
            sb.Append("         width: 400,\r\n");
            sb.Append("         modal: true,\r\n");
            sb.Append("         autoOpen: false,\r\n");
            sb.Append("         buttons:\r\n");
            sb.Append("             {\r\n");
            sb.Append("                 \"Sim\": function() {\r\n");
            sb.Append("                     __doPostBack(\"funcao_ATUALIZA_SERVICOS_ESCOPOS\", \"\");\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("                 \"Não\": function() {\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("             }\r\n");
            sb.Append("     });\r\n");
            sb.Append("     $v192('[id*=lnkAtualizarServicos_Escopos]').click(function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $v192('#dialog-AtualizarServicos_Escopos').dialog('open');\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal Personalizado - Controle de Margem por Sub-Serviço
            sb.Append("     $('.controle').off('click').on('click', function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         __doPostBack(\"funcao_CONTROLE\", \"\");\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal Personalizado - Vincular CRM
            sb.Append("     $('.crm').off('click').on('click', function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         __doPostBack(\"funcao_VINCULAR\", \"\");\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal Personalizado - Vincular Pedido
            sb.Append("     $('.pedido').off('click').on('click', function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         if ($(this).hasClass('novo')) {\r\n");
            sb.Append("             __doPostBack(\"funcao_VINCULAR_NOVO_PEDIDO\", \"\");\r\n");
            sb.Append("         }\r\n");
            sb.Append("         else {\r\n");
            sb.Append("             __doPostBack(\"funcao_VINCULAR_PEDIDO\", \"\");\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal Personalizado - Novo Endereço de Entrega
            sb.Append("     $('.novoEndereco').off('click').on('click', function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $('#modal_NovoEndereco').modal('show');\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal Personalizado - Nova Condição de Pagamento
            sb.Append("     $('.novaCondPgto').off('click').on('click', function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $('#modal_novaCondicaoPagamento').modal('show');\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal Personalizado - Importar Produtos via Upload de Excel
            sb.Append("     $('.importar').off('click').on('click', function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $('#modalUpload_ImportarExcel').modal('show');\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal Personalizado - Vincular Produtos
            sb.Append("     $('.vincularProduto').off('click').on('click', function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $('[id*=hddVincularProduto]').val($(this).closest($('.modaisCodigo')).siblings('.link').find('a').text());\r\n");
            sb.Append("         $('[id*=txtVincularProduto]').val($(this).closest($('.modaisCodigo')).siblings('.link').find('a').text());\r\n");
            sb.Append("         $('#modalVincularProdutos').modal('show');\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal Personalizado - Vincular Todos os Produtos
            sb.Append("     $('.vincularTodosProdutos').off('click').on('click', function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $('#modalVincularTodosProdutos').modal('show');\r\n");
            sb.Append("     });\r\n\r\n");

            sb.Append("});\r\n\r\n");

            // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- //

            // Script para exibir a Composição dos Itens no método por Linhas
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

            // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- //

            // Desativar a tecla ENTER, exceto quando estiver em algum botão ou no campo de Observação
            sb.Append("$(document).off('keydown').on('keydown', function(e) {\r\n");
            sb.Append("     if (e.key === \"Enter\") {\r\n");
            sb.Append("         var activeElement = document.activeElement;\r\n");
            sb.Append($"         var txtObservacao = $(activeElement).is('#{txtObservacao.ClientID}');\r\n");
            sb.Append("         if (!txtObservacao && activeElement.tagName !== 'A' && activeElement.tagName !== 'BUTTON' && activeElement.type !== 'submit') {\r\n");
            sb.Append("             e.preventDefault();\r\n");
            sb.Append("         }\r\n");
            sb.Append("     }\r\n");
            sb.Append("});\r\n\r\n");

            // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- //

            // Adiciona o Click para Serviços que possuem o Card
            sb.Append("$('[id*=lnkCard]').off('click').on('click', function() { window.open(window.location.protocol + '//' + window.location.host + '/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id=' + $(this).attr('data-idproduto'), '_blank'); return false; });\r\n\r\n");

            // Adiciona o Card de informações dos Itens
            sb.Append("     var cardTimer = { };\r\n");
            sb.Append("     function mostraCard(element, idProduto, tabela) {\r\n");
            sb.Append("         cardTimer[idProduto + '_' + tabela] = setTimeout(function() {\r\n");
            sb.Append("             $.ajax({\r\n");
            sb.Append("                 url: \"/API/Pagina_Ajax.aspx/GetProdutoDetalhes\",\r\n");
            sb.Append("                 data: JSON.stringify({ idProduto: idProduto }),\r\n");
            sb.Append("                 type: 'POST',\r\n");
            sb.Append("                 dataType: 'json',\r\n");
            sb.Append("                 contentType: 'application/json; charset=utf-8',\r\n");
            sb.Append("                 success: function(response) {\r\n");
            sb.Append("                     var produto = JSON.parse(response.d);\r\n");
            sb.Append("                     var cardProduto = `\r\n");
            sb.Append("                         <div class=\"card\">\r\n");
            sb.Append("                             <div class=\"card-body d-flex\">\r\n");
            sb.Append("                                 <div class=\"flex-shrink-0\" style=\"min-inline-size: fit-content;\">\r\n");
            sb.Append("                                     ${produto.imagem? `<img src = \"${produto.imagem}\" alt=\"Imagem do Produto\" class=\"img-fluid img-thumbnail\" style=\"width: 100px; height: auto;\" />` : ''}\r\n");
            sb.Append("                                 </div>\r\n");
            sb.Append("                                 <div class=\"flex-grow-1 d-flex flex-column ms-3\">\r\n");
            sb.Append("                                     <div class=\"d-flex\">\r\n");
            sb.Append("                                         ${produto.sCategoriaVendas? `<div class=\"card-text me-3\"> <strong>Categoria Vendas: </strong>${produto.sCategoriaVendas\r\n}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sFabricante ? `<div class= \"card-text me-3\"> <strong > Fabricante: </strong >${ produto.sFabricante}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sGrupo ? `<div class= \"card-text me-3\"> <strong > Grupo: </strong >${ produto.sGrupo}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sFamilia ? `<div class= \"card-text me-3\"> <strong > Família: </strong >${ produto.sFamilia}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sPaisOrigem ? `<div class= \"card-text me-3\"> <strong > Origem: </strong >${ produto.sPaisOrigem}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sLocalArmazenamento ? `<div class= \"card-text me-3\"> <strong > Local Armazenamento: </strong >${ produto.sLocalArmazenamento}</div>` : ''}\r\n");
            sb.Append("                                     </div>\r\n");
            sb.Append("                                 </div>\r\n");
            sb.Append("                             </div>\r\n");
            sb.Append("                         </div>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                     var cardId = idProduto + '_' + tabela;\r\n");
            sb.Append("                     var card = document.getElementById(cardId);\r\n");
            sb.Append("                     card.innerHTML = cardProduto;\r\n");
            sb.Append("                     var rect = element.getBoundingClientRect();\r\n");
            sb.Append("                     var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;\r\n");
            sb.Append("                     var scrollLeft = document.documentElement.scrollLeft || document.body.scrollLeft;\r\n");
            sb.Append("                     hideAllCards();\r\n");
            sb.Append("                     card.style.top = (rect.top + scrollTop - 10) + 'px';\r\n");
            sb.Append("                     card.style.left = (rect.right + scrollLeft + element.offsetWidth + 10) + 'px';\r\n");
            sb.Append("                     card.style.display = 'block';\r\n");
            sb.Append("                 },\r\n");
            sb.Append("                 error: function(error) {\r\n");
            sb.Append("                     console.error(\"Erro ao obter os detalhes do produto:\", error);\r\n");
            sb.Append("                 }\r\n");
            sb.Append("             });\r\n");
            sb.Append("         }, 300);\r\n");
            sb.Append("     }\r\n\r\n");
            sb.Append("     function escondeCard(idProduto, tabela) {\r\n");
            sb.Append("         var cardId = idProduto + '_' + tabela;\r\n");
            sb.Append("         var card = document.getElementById(cardId);\r\n");
            sb.Append("         clearTimeout(cardTimer[idProduto + '_' + tabela]);\r\n");
            sb.Append("         card.style.display = 'none';\r\n");
            sb.Append("     }\r\n\r\n");
            sb.Append("     function hideAllCards() {\r\n");
            sb.Append("         var cards = document.querySelectorAll('.product-card');\r\n");
            sb.Append("         cards.forEach(function(card) {\r\n");
            sb.Append("             card.style.display = 'none';\r\n");
            sb.Append("         });\r\n");
            sb.Append("     }\r\n\r\n");
            sb.Append("     function openModal(idProduto) {\r\n");
            sb.Append("         $.ajax({\r\n");
            sb.Append("             url: \"/API/Pagina_Ajax.aspx/GetProdutoDetalhes\",\r\n");
            sb.Append("             data: JSON.stringify({ idProduto: idProduto }),\r\n");
            sb.Append("             type: 'POST',\r\n");
            sb.Append("             dataType: 'json',\r\n");
            sb.Append("             contentType: 'application/json; charset=utf-8',\r\n");
            sb.Append("             success: function(response) {\r\n");
            sb.Append("                 var produto = JSON.parse(response.d);\r\n");
            sb.Append("                 var tituloProduto = `\r\n");
            sb.Append("                     <button type = \"button\" class= \"close\" data - dismiss = \"modal\" aria - label = \"Close\">\r\n");
            sb.Append("                         <span aria - hidden = \"true\" > &times;</span>\r\n");
            sb.Append("                     </button>\r\n");
            sb.Append("                     <h5 class= \"modal-title\" id = \"detailsModalLabel\" > ${ produto.sCodigo} - ${ produto.sDsc}</h5>\r\n");
            sb.Append("                 `;\r\n");
            sb.Append("                 var modalInfo = document.getElementById('modalInfo');\r\n");
            sb.Append("                 modalInfo.innerHTML = tituloProduto;\r\n");
            sb.Append("                 var imagem = '';\r\n");
            sb.Append("                 if (produto.imagem) {\r\n");
            sb.Append("                     imagem += `\r\n");
            sb.Append("                         <div style = \"text-align: center; margin-bottom: 20px;\">\r\n");
            sb.Append("                             <img src = \"${produto.imagem}\" alt = \"Imagem do Produto\" class= \"img-fluid\" style = \"width: 300px; height: auto;\" />\r\n");
            sb.Append("                         </div>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 var tabelaProduto = '<table class=\"table table-bordered\">';\r\n");
            sb.Append("                 if (produto.sCategoriaVendas) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Categoria Vendas </th>\r\n");
            sb.Append("                             <td>${ produto.sCategoriaVendas}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sTipo) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Tipo </th>\r\n");
            sb.Append("                             <td>${ produto.sTipo}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sGrupo) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Grupo </th>\r\n");
            sb.Append("                             <td>${ produto.sGrupo}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sFabricante) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Fabricante </th>\r\n");
            sb.Append("                             <td>${ produto.sFabricante}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sLocalArmazenamento) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Local Armazenamento </th>\r\n");
            sb.Append("                             <td>${ produto.sLocalArmazenamento}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sFamilia) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Família </th>\r\n");
            sb.Append("                             <td>${ produto.sFamilia}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sCodigoCEST) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> CEST </th>\r\n");
            sb.Append("                             <td>${ produto.sCodigoCEST}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sCodigoNCM) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> NCM </th>\r\n");
            sb.Append("                             <td>${ produto.sCodigoNCM}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sPaisOrigem) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Origem </th>\r\n");
            sb.Append("                             <td>${ produto.sPaisOrigem}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 tabelaProduto += `</table >`;\r\n");
            sb.Append("                 var modalBody = document.getElementById('modalBody');\r\n");
            sb.Append("                 modalBody.innerHTML = imagem + tabelaProduto;\r\n");
            sb.Append("                 $('#produtoDetalheModal').modal('show');\r\n");
            sb.Append("             },\r\n");
            sb.Append("             error: function(error) {\r\n");
            sb.Append("                 console.error(\"Erro ao obter os detalhes do produto:\", error);\r\n");
            sb.Append("             }\r\n");
            sb.Append("         });\r\n");
            sb.Append("     }\r\n");
            sb.Append("     function openProductDetail(idItem) {\r\n");
            sb.Append("         var url = '/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id=' + idItem;\r\n");
            sb.Append("         window.open(url, '_blank');\r\n");
            sb.Append("         return false;\r\n");
            sb.Append("     }\r\n");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScript_Orcamentos", sb.ToString(), true);
        }

        /// <summary>
        /// Método utilizado para Retornar um script usado para Incluir Serviços em uma Empreitada.
        /// </summary>
        /// <param name="clientID">Recebe o ClientID do campo de onde o script será ativado.</param>
        /// <param name="bCodigo">Recebe um parâmetro que define se o campo representa o Código do Serviço.</param>
        /// <returns>Retorna o script em uma String.</returns>
        protected string RetornaScript_IncluirServicos(string clientID, bool bCodigo)
        {
            string servicos = JsonConvert.SerializeObject(hddIncluirServicos.Value.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries));

            StringBuilder sb = new StringBuilder();

            sb.Append("$v192(function() {\r\n");
            sb.Append("$v192(\"[id*=" + clientID + "]\").autocomplete({\r\n");
            sb.Append("source: function(request, response) {\r\n");
            sb.Append("$v192.ajax({\r\n");
            sb.Append("url: '/app/Paginas/Comercial/Orcamento_Detalhe.aspx/GetServicos',\r\n");
            sb.Append("data: JSON.stringify({\r\n");

            sb.Append("'hddServicos': " + servicos + ", \r\n");
            sb.Append("'sDscServico': JSON.stringify(request.term), \r\n");
            sb.Append("'sCodigo': '" + bCodigo.ToString() + "'\r\n");

            sb.Append("}),\r\n"); // Adicione uma vírgula após a chave 'data'

            sb.Append("dataType: \"json\",\r\n");
            sb.Append("type: \"POST\",\r\n");
            sb.Append("contentType: \"application/json; charset=utf-8\",\r\n");
            sb.Append("success: function(data) {\r\n");
            sb.Append("response($v192.map(data.d, function(item) {\r\n");
            sb.Append("return {\r\n");

            sb.Append("label: item.split('|')[" + (bCodigo ? 1 : 2) + "],\r\n");

            sb.Append(hddidIncluirServico.ClientID + ": item.split('|')[0],\r\n");
            sb.Append(txtIncluirServico_Empreitada_Codigo.ClientID + ": item.split('|')[1],\r\n");
            sb.Append(txtIncluirServico_Empreitada_Descricao.ClientID + ": item.split('|')[2]\r\n");

            sb.Append("};\r\n");
            sb.Append("}));\r\n"); // Adicione parênteses de fechamento para a função 'map'
            sb.Append("},\r\n");
            sb.Append("error: function(response) {\r\n");
            sb.Append("console.log(response.responseText);\r\n");
            sb.Append("},\r\n");
            sb.Append("failure: function(response) {\r\n");
            sb.Append("console.log(response.responseText);\r\n");
            sb.Append("}\r\n");
            sb.Append("});\r\n");
            sb.Append("},\r\n");
            sb.Append("select: function(e, i) {\r\n");

            sb.Append("$(\"[id$=" + hddidIncluirServico.ClientID + "]\").val(i.item." + hddidIncluirServico.ClientID + ");\r\n");
            sb.Append("$(\"[id$=" + txtIncluirServico_Empreitada_Codigo.ClientID + "]\").val(i.item." + txtIncluirServico_Empreitada_Codigo.ClientID + ");\r\n");
            sb.Append("$(\"[id$=" + txtIncluirServico_Empreitada_Descricao.ClientID + "]\").val(i.item." + txtIncluirServico_Empreitada_Descricao.ClientID + ");\r\n");

            sb.Append("$('[id$=" + cmdIncluirServico_Empreitada.ClientID + "]').focus();");

            sb.Append("},\r\n");
            sb.Append("minLength: 3\r\n");
            sb.Append("});\r\n");
            sb.Append("});\r\n\r\n");

            return sb.ToString();
        }

        #endregion

        #region | WebMethod's

        [WebMethod]
        public static string[] GetServicos(string[] hddServicos, string sDscServico, string sCodigo)
        {
            sDscServico = sDscServico.Replace("\"", "").Trim();

            bool bCodigo = Convert.ToBoolean(sCodigo);

            string[] servicos = hddServicos.Where(s => bCodigo ? s.Split('|')[1].ToLower().Contains(sDscServico.ToLower()) : s.Split('|')[2].ToLower().Contains(sDscServico.ToLower())).ToArray();

            return servicos;
        }

        #endregion

        #region | PDF

        /// <summary>
        /// Método do Evendo de Click do botão 'cmdGeraPDF'. Este é utilizado para chamar os Métodos que farão a geração dos Documentos em PDF e exibí-lo para o Usuário.
        /// </summary>
        protected void cmdGeraPDF_Click(object sender, EventArgs e)
        {
            try
            {
                list_PDFs.Clear();

                try
                {
                    if (hddsEdicao.Value.Equals("S"))
                    {
                        MantemEtapa_Pos_PostBack(5);
                        AtualizaBarraProgresso(5, false);
                    }
                }
                catch { }

                string sChecklists_PDF = string.Empty;
                bool bChecklists_PDF = false;
                int totalPaginas = 0;

                if (dt_CheckList.Rows.Count > 0 && cbEscopos_PDF.Checked)
                {
                    sChecklists_PDF = GerarPDF_Orcamento_Escopos();

                    if (!string.IsNullOrEmpty(sChecklists_PDF))
                        bChecklists_PDF = true;

                    if (bChecklists_PDF)
                    {
                        using (PdfReader reader = new PdfReader(Server.MapPath("~/Download/") + sChecklists_PDF))
                        {
                            totalPaginas = reader.NumberOfPages;
                        }
                    }
                }

                totalPaginas = GerarPDF_Orcamento(hddNumeroPedido.Value, totalPaginas, 1, bChecklists_PDF);

                bool bRascunho = Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]);
                string sImagem = Combine(AppDomain.CurrentDomain.BaseDirectory, "App", "img", bRascunho ? "Rascunho_Marca_Dagua.png" : "LogoTT_Horizontal.png");

                byte[] PDF;
                if (bRascunho)
                    PDF = Adiciona_MarcaDagua(list_PDFs[0], sImagem, "", null, null, 0, 25, 50, -100, 0, 0.45f);
                else
                    PDF = Adiciona_MarcaDagua(list_PDFs[0], sImagem, "");

                string sPDF = list_PDFs[0].Replace(".pdf", "_Marcado.pdf");
                File.WriteAllBytes(Server.MapPath("~/Download/") + sPDF, PDF);
                list_PDFs[0] = sPDF;

                if (bChecklists_PDF)
                {
                    sChecklists_PDF = Adiciona_Rodape(sChecklists_PDF, totalPaginas);

                    byte[] PDF_Checklist;
                    if (bRascunho)
                        PDF_Checklist = Adiciona_MarcaDagua(sChecklists_PDF, sImagem, "", null, null, 0, 25, 50, -100, 0, 0.45f);
                    else
                        PDF_Checklist = Adiciona_MarcaDagua(sChecklists_PDF, sImagem, "");

                    sChecklists_PDF = sChecklists_PDF.Replace(".pdf", "_Marcado.pdf");

                    File.WriteAllBytes(Server.MapPath("~/Download/") + sChecklists_PDF, PDF_Checklist);
                    list_PDFs.Add(sChecklists_PDF);
                }

                GeraPDFCombinado();

                if (Request["id"] == "0")
                    MensagemPaginaDentro_View.MostraMensagem_Sucesso("Documento gerado com sucesso!<br /><b>Lembrete:</b> Ainda é necessário Salvar o Orçamento para que o mesmo seja registrado!", true);
                else
                    MensagemPaginaDentro_View.MostraMensagem_Sucesso("Documento gerado com sucesso!", true);
            }
            catch (Exception ex)
            {
                MensagemPaginaDentro_View.MostraMensagem_Erro("<b>Erro:</b> Houve um erro ao Gerar o Documento PDF de Orçamento! </br>Error: " + ex.Message);
            }

            string destinoVenda = txtDestinoVenda_View.Text;
            string IE = txtIE_View.Text;

            try
            {
                MantemEtapa_Pos_PostBack(5);
            }
            catch { }

            txtDestinoVenda_View.Text = destinoVenda;
            txtIE_View.Text = IE;

            if (hddsEdicao.Value.Equals("N"))
            {
                pn5.Attributes.Remove("class");
                pn5.Attributes.Add("class", "painel");
                div_BreadCrumb.Attributes.Remove("style");
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_AlinhaPagina", "$('#page-wrapper').css({'left': '2.5%';});", true);
            }

            aba_Documentos.Visible = true;

            Scripts.Mantem_AbaAtiva(Page, "aba-Documentos");
        }

        /// <summary>
        /// Método do Evendo de Click do botão 'cmdGeraExcel'´. Este é utilizado para chamar os Métodos que farão a geração dos Documentos em Excel e exibí-lo para o Usuário.
        /// </summary>
        protected void cmdGeraExcel_Click(object sender, EventArgs e)
        {
            try
            {
                list_PDFs.Clear();

                GerarPDF_Orcamento(hddNumeroPedido.Value, 0, 2, false);

                DownloadArquivo(Page, list_PDFs[0]);
            }
            catch (Exception ex)
            {
                MensagemPaginaDentro_View.MostraMensagem_Erro("<b>Erro:</b> Houve um erro ao Gerar o Documento em Excel de Orçamento! </br>Erro Excel: " + ex.Message + (ex.InnerException != null ? string.Format("<br />{0}", ex.InnerException.Message) : ""));
            }

            try
            {
                MantemEtapa_Pos_PostBack(5);
            }
            catch { }

            if (hddsEdicao.Value.Equals("N"))
            {
                pn5.Attributes.Remove("class");
                pn5.Attributes.Add("class", "painel");
                div_BreadCrumb.Attributes.Remove("style");
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_AlinhaPagina", "$('#page-wrapper').css({'left': '2.5%';});", true);
            }

            aba_Documentos.Visible = true;

            Scripts.Mantem_AbaAtiva(Page, "aba-Documentos");
        }

        /// <summary>
        /// Função utilizado para Gerar a maior parte do Documento PDF, pelo 'Report Viewer'.
        /// </summary>
        /// <param name="nNumeroPedido">Recebe o Número do Orçamento, para o caso do PDF estar sendo gerado antes do Orçamento ser Salvo.</param>
        /// <param name="nTotalPaginas">Recebe o Total de Páginas já geradas por outras partes do Documento, para aplicar no Rodapé.</param>
        /// <param name="nFormato">Recebe o Formato em que o Documento será gerado.</param>
        /// <returns>Retorna o número de Páginas deste Documento gerado pelo Report Viewer, para que este seja o ponto de partida, no Contador de Páginas, do Rodapé em outras partes do Documento.</returns>
        /// <exception cref="Exception">Gera uma exceção em caso de erros na geração do Documento pelo Report Viewer, com uma mensagem exibindo detalhadamente o Erro gerado.</exception>
        protected int GerarPDF_Orcamento(string nNumeroPedido, int nTotalPaginas, int nFormato, bool bChecklists_PDF)
        {
            try
            {
                bool bNacional = hddMoeda.Value == "0" || hddMoeda.Value == "2";
                IFormatProvider formato = GetCultureInfo(bNacional ? "pt-BR" : "en-US");
                string sPrazo = bNacional ? "Imediato" : "Immediate";
                string sDias = bNacional ? "Dias" : "Days";

                nNumeroPedido = string.IsNullOrEmpty(nNumeroPedido) || nNumeroPedido.Equals("0") ? " " : nNumeroPedido;

                ReportViewer rv = new ReportViewer { ProcessingMode = ProcessingMode.Local };
                rv.LocalReport.EnableExternalImages = true;
                rv.LocalReport.EnableHyperlinks = true;
                rv.LocalReport.ReportPath = $@"App\Reports\{(bNacional ? (!cbMinimo_PDF.Checked ? "Orcamento_Minimo" : "Orcamento") : "Orcamento_EN")}.rdlc";

                string sComposicao = "N";
                decimal baseCalc_ICMS = 0,
                        reducaoBaseCalc_ICMS = 0,
                        totalDIFAL = 0,
                        totalST = 0,
                        totalProdutos = 0,
                        totalServicos = 0,
                        totalDesconto = 0,
                        totalIPI = 0,
                        totalICMS = 0,
                        totalINSS = 0,
                        totalISS = 0,
                        totalPIS = 0,
                        totalCOFINS = 0,
                        totalIRPJ = 0,
                        totalCSSL = 0,
                        nIRPJ = 0,
                        nCSSL = 0,
                        PesoBruto = 0,
                        PesoLiq = 0;

                List<cls_Comercial_Tabelas> list_Prod = new List<cls_Comercial_Tabelas>();
                List<cls_Comercial_Tabelas> list_Serv = new List<cls_Comercial_Tabelas>();
                List<cls_Comercial_Tabelas> list_Comp = new List<cls_Comercial_Tabelas>();
                Dictionary<string, string> parametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTA_INFO_PRODUTOS_PARCEIRO" },
                    { "@idItem", "0" },
                    { "@idParceiro_Cliente", hddidCliente.Value },
                    { "@idPais", hddidEmpresa.Value },
                    { "@idMoeda", hddMoeda.Value }
                };

                foreach (var p in listProdutos.Where(p => p.bLiberado).OrderBy(p => p.nOrdem))
                {
                    cls_Comercial_Tabelas produto = new cls_Comercial_Tabelas
                    {
                        IdItem = p.IdItem,
                        nOrdem = p.nOrdem,
                        SCodigo = p.SCodigo,
                        SDscProduto = p.SDscProduto,
                        sNCM = p.sNCM,
                        sCFOP = p.sCFOP,
                        sCST = p.sCST,
                        SUnidade = p.SUnidade,
                        Preco = p.Preco,
                        NFator = p.NFator,
                        NIPI = p.NIPI,
                        NPIS = p.NPIS,
                        NCOFINS = p.NCOFINS,
                        NICMS = p.NICMS,
                        NST = p.NST,
                        NDIFAL = p.NDIFAL,
                        nVlr_IPI = p.nVlr_IPI,
                        nVlr_PIS = p.nVlr_PIS,
                        nVlr_COFINS = p.nVlr_COFINS,
                        nBaseCalc_ICMS = p.nBaseCalc_ICMS_Original,
                        nVlrReducao = p.nVlrReducao,
                        nVlr_ICMS = p.nVlr_ICMS,
                        nVlr_ST = p.nVlr_ST,
                        nVlr_DIFAL = p.nVlr_DIFAL,
                        nVlr_Liquido = p.nVlr_Liquido,
                        NQuantidade = p.NQuantidade,
                        NTotal = p.NTotal,
                        nPesoBruto = p.nPesoBruto,
                        nPesoLiquido = p.nPesoLiquido,
                        nVolume = p.nVolume,
                        dtInclusao = p.dtInclusao,
                    };

                    parametros["@idItem"] = p.IdItem.ToString();
                    DataSet dsProdutos = ExecutarDataSet(sProcedure_Produtos, parametros);

                    if (ValidarDataSet(dsProdutos))
                    {
                        produto.sCodigoPai = DATASET(dsProdutos, "sCodigoCliente");
                        produto.sDscProdutoPai = DATASET(dsProdutos, "sDscProdutoCliente");
                        produto.SDscVendasEN = DATASET(dsProdutos, 1, 0, "sDscDescricao");

                        if (DATASET(dsProdutos, "sListarOrcamento").ToUpper().Trim().Equals("S"))
                            produto.sLink = DATASET(dsProdutos, "sDscEndereco");
                    }

                    list_Prod.Add(produto);
                }

                foreach (var s in listServicos_Recursos.Where(s => s.bLiberado).OrderBy(s => s.nOrdem))
                {
                    cls_Comercial_Tabelas servico = new cls_Comercial_Tabelas
                    {
                        IdItem = s.IdItem,
                        nOrdem = s.nOrdem,
                        SCodigo = s.SCodigo,
                        SDscProduto = s.SDscProduto,
                        sNCM = s.sNCM,
                        sCST = s.sCST,
                        SUnidade = s.SUnidade,
                        NQuantidade = s.NQuantidade,
                        Preco = s.Preco,
                        NFator = s.NFator,
                        NMargem = s.NMargem,
                        NIPI = s.NIPI,
                        NICMS = s.NICMS,
                        NPIS = s.NPIS,
                        NCOFINS = s.NCOFINS,
                        NIRPJ = s.NIRPJ,
                        NCSSL = s.NCSSL,
                        nVlr_IPI = s.nVlr_IPI,
                        nVlr_ICMS = s.nVlr_ICMS,
                        nVlr_PIS = s.nVlr_PIS,
                        nVlr_COFINS = s.nVlr_COFINS,
                        nVlr_IRPJ = s.nVlr_IRPJ,
                        nVlr_CSSL = s.nVlr_CSSL,
                        NTotal = s.NTotal,
                        dtInclusao = s.dtInclusao
                    };

                    if (!bNacional)
                    {
                        parametros["@idItem"] = s.IdItem.ToString();
                        DataSet ds = ExecutarDataSet(sProcedure_Produtos, parametros);

                        if (ValidarDataSet(ds))
                            servico.SDscVendasEN = DATASET(ds, 1, 0, "sDscDescricao");
                    }

                    list_Serv.Add(servico);
                }

                if (cbComposicaoSistema_PDF.Checked && listProdutos.Any(p => p.bSistema))
                {
                    sComposicao = "S";

                    foreach (var p in listProdutos_Composicao.Where(p => p.bLiberado).OrderBy(p => (listProdutos.FirstOrDefault(s => s.idRegistro.Equals(p.idItemPai)).nOrdem, p.nOrdem)))
                    {
                        cls_Comercial_Tabelas comp = new cls_Comercial_Tabelas
                        {
                            IdItem = p.IdItem,
                            nOrdem = p.nOrdem,
                            SCodigo = p.SCodigo,
                            sNCM = p.sNCM,
                            SDscProduto = p.SDscProduto,
                            SUnidade = p.SUnidade,
                            NQuantidade = p.NQuantidade,
                            idItemPai = p.idItemPai,
                            sLinkPai = listProdutos.FirstOrDefault(s => s.idRegistro.Equals(p.idItemPai)).sLink,
                            nOrdemPai = listProdutos.FirstOrDefault(s => s.idRegistro.Equals(p.idItemPai)).nOrdem,
                            sCodigoPai = listProdutos.FirstOrDefault(s => s.idRegistro.Equals(p.idItemPai)).SCodigo,
                            sDscProdutoPai = listProdutos.FirstOrDefault(s => s.idRegistro.Equals(p.idItemPai)).SDscProduto
                        };

                        parametros["@idItem"] = p.IdItem.ToString();
                        DataSet ds = ExecutarDataSet(sProcedure_Produtos, parametros);

                        if (ValidarDataSet(ds))
                        {
                            comp.SDscVendasEN = DATASET(ds, 1, 0, "sDscDescricao");

                            if (DATASET(ds, "sListarOrcamento").ToUpper().Trim().Equals("S"))
                                comp.sLink = DATASET(ds, "sDscEndereco");
                        }

                        list_Comp.Add(comp);
                    }
                }

                foreach (var p in list_Prod)
                {
                    if (bNacional)
                    {
                        totalIPI += p.nVlr_IPI * p.NQuantidade;
                        baseCalc_ICMS += p.nBaseCalc_ICMS * p.NQuantidade;
                        reducaoBaseCalc_ICMS += p.nVlrReducao * p.NQuantidade;
                        totalICMS += p.nVlr_ICMS * p.NQuantidade;
                        totalDIFAL += p.nVlr_DIFAL * p.NQuantidade;
                        totalST += p.nVlr_ST;
                    }

                    p.nDesconto = Math.Round(p.Preco - (p.Preco - (p.Preco * (p.NFator / 100))), 2);

                    if (p.nDesconto > decimal.Zero)
                        totalDesconto += p.nDesconto * p.NQuantidade;

                    p.Preco = Math.Round(p.Preco - (p.Preco * (p.NFator / 100)), 2);

                    if (bNacional)
                    {
                        p.sIPI = string.Format("{0} %| {1}", p.NIPI.ToString("N2"), p.nVlr_IPI.ToString("N2"));
                        p.sPIS = string.Format("{0} %| {1}", p.NPIS.ToString("N2"), p.nVlr_PIS.ToString("N2"));
                        p.sCOFINS = string.Format("{0} %| {1}", p.NCOFINS.ToString("N2"), p.nVlr_COFINS.ToString("N2"));
                        p.sICMS = string.Format("{0} %| {1}", p.NICMS.ToString("N2"), p.nVlr_ICMS.ToString("N2"));
                        p.sST = string.Format("{0} %| {1}", p.NST.ToString("N2"), p.nVlr_ST.ToString("N2"));
                        p.sDIFAL = string.Format("{0} %| {1}", p.NDIFAL.ToString("N2"), p.nVlr_DIFAL.ToString("N2"));
                    }

                    totalProdutos += Math.Round(p.NTotal + p.nVlr_ST, 2);
                    PesoBruto += Math.Round(p.nPesoBruto * p.NQuantidade, 2);
                    PesoLiq += Math.Round(p.nPesoLiquido * p.NQuantidade, 2);

                    p.NTotal = Math.Round(p.NTotal + p.nVlr_ST, 2);

                    if (cbLPU.Checked)
                    {
                        p.SCodigo = string.Format("{0}\r\n{1}", p.SCodigo, string.IsNullOrEmpty(p.sNCM) || p.sNCM.Equals("0.00") || p.sNCM.StartsWith("N") ? string.Empty : p.sNCM);
                        p.Preco = p.Preco / (p.NIPI / 100 + 1);
                    }
                    else
                        p.SCodigo = string.Format("{0}\r\n{1}{2}", p.SCodigo, cbCliente_PDF.Checked && !string.IsNullOrEmpty(p.sCodigoPai) ? string.Format("{0}\r\n", p.sCodigoPai) : string.Empty, string.IsNullOrEmpty(p.sNCM) || p.sNCM.Equals("0.00") || p.sNCM.StartsWith("N") ? string.Empty : p.sNCM);

                    p.SDscProduto = string.Format("{0}{1}", bNacional ? p.SDscProduto : IsNull(p.SDscVendasEN, p.SDscProduto), cbCliente_PDF.Checked && !string.IsNullOrEmpty(p.sDscProdutoPai) ? string.Format("\r\n{0}", p.sDscProdutoPai) : string.Empty);

                    p.sOrdem = string.Format("{0}{1}", p.nOrdem.ToString("N0", formato), string.IsNullOrEmpty(p.sLink) ? string.Empty : "\r\n>>"); // Ordem / Link
                    p.dtInclusao = p.dtInclusao.Length > 0 ? p.dtInclusao.Equals("0") ? sPrazo : string.Format("{0} {1}", p.dtInclusao, sDias) : $"15 {sDias}"; // Prazo de Entrega
                    p.SUnidade = string.Format("{0}\r\n{1}", p.SUnidade, p.NQuantidade.ToString("N2", formato)); // Unidade / Quantidade

                    if (bNacional)
                    {
                        p.nTotalComposicao = Math.Round(p.nVlr_Liquido * p.NQuantidade, 2); // Total Líquido
                        p.NIPI = Math.Round(p.Preco - p.nVlr_IPI, 2); // Valor sem IPI

                        p.sIPI = p.sIPI.Replace("|", "\r\n");
                        p.sPIS = p.sPIS.Replace("|", "\r\n");
                        p.sCOFINS = p.sCOFINS.Replace("|", "\r\n");
                        p.sICMS = p.sICMS.Replace("|", "\r\n");
                        p.sST = p.sST.Replace("|", "\r\n");
                        p.sDIFAL = p.sDIFAL.Replace("|", "\r\n");

                        p.sCST = string.Format("{0}\r\n{1}", p.sCST, p.sCFOP);
                    }
                    else
                    {
                        p.sPIS = $"{hddMoeda_Simbolo.Value} {p.Preco.ToString("N2", formato)}";
                        p.sIPI = $"{hddMoeda_Simbolo.Value} {p.NTotal.ToString("N2", formato)}";
                    }
                }

                foreach (var s in list_Serv)
                {
                    s.nDesconto = Math.Round((s.Preco * s.NMargem) - ((s.Preco * s.NMargem) - (s.Preco * s.NMargem * (s.NFator / 100))), 2);

                    totalDesconto += s.nDesconto;

                    s.Preco = Math.Round(s.Preco - (s.Preco * (s.NFator / 100)), 2);

                    totalINSS += s.nVlr_ICMS;
                    totalISS += s.nVlr_IPI;
                    totalPIS += s.nVlr_PIS;
                    totalCOFINS += s.nVlr_COFINS;
                    totalIRPJ += s.nVlr_IRPJ;
                    totalCSSL += s.nVlr_CSSL;

                    nIRPJ += s.NIRPJ;
                    nCSSL += s.NCSSL;

                    s.sIPI = string.Format("{0} %| {1}", s.NIPI.ToString("N2"), s.nVlr_IPI.ToString("N2"));
                    s.sICMS = string.Format("{0} %| {1}", s.NICMS.ToString("N2"), s.nVlr_ICMS.ToString("N2"));

                    totalServicos += Math.Round(s.NTotal, 2);

                    s.SCodigo = s.SCodigo + "\r\n" + (string.IsNullOrEmpty(s.sNCM) || s.sNCM.Equals("0.00") || s.sNCM.StartsWith("N") ? "" : s.sNCM);
                    s.SDscProduto = bNacional ? s.SDscProduto : IsNull(s.SDscVendasEN, s.SDscProduto);

                    s.dtInclusao = !string.IsNullOrEmpty(s.dtInclusao) ? s.dtInclusao.Equals("0") ? sPrazo : string.Format("{0} {1}", s.dtInclusao, sDias) : $"15 {sDias}";

                    if (bNacional)
                    {
                        s.sIPI = s.sIPI.Replace("|", "\r\n");
                        s.sICMS = s.sICMS.Replace("|", "\r\n");
                    }
                    else
                        s.sIPI = $"{hddMoeda_Simbolo.Value} {s.NTotal.ToString("N2", formato)}";

                    s.sCST = string.IsNullOrEmpty(s.sCST) || s.sCST.Equals("0.00") ? " " : s.sCST;
                }

                foreach (var c in list_Comp)
                {
                    c.SCodigo = c.SCodigo + "\r\n" + (string.IsNullOrEmpty(c.sNCM) || c.sNCM.Equals("0.00") || c.sNCM.StartsWith("N") ? "" : c.sNCM);
                    c.SDscProduto = bNacional ? c.SDscProduto : IsNull(c.SDscVendasEN, c.SDscProduto);

                    c.sOrdem = string.Format("{0}.{1}{2}", c.nOrdemPai, c.nOrdem, string.IsNullOrEmpty(c.sLink) ? string.Empty : "\r\n>>");
                    c.sOrdemPai = string.Format("{0}{1}", c.nOrdemPai, string.IsNullOrEmpty(c.sLinkPai) ? string.Empty : "\r\n>>");

                    c.SUnidade = string.Format("{0}\r\n{1}", c.SUnidade, c.NQuantidade.ToString("N0", formato));
                }

                rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Produtos", list_Prod));
                rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Servicos", list_Serv));
                rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Vazio", new List<string>() { "vazio" }));
                rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Composicao", list_Comp));

                DataSet dsTabela = ExecutarDataSet(sProcedure_TabelaPreco, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idTabela", string.IsNullOrEmpty(hddidTabela.Value) ? ddlTabela.SelectedValue : hddidTabela.Value } });
                DataSet dsEmpresa = ExecutarDataSet(sProcedure_Empresas, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idParceiro", hddidEmpresa.Value } });
                DataSet dsCliente = ExecutarDataSet(sProcedure_Clientes, new Dictionary<string, string> { { "@sFuncao", "CONSULTA_DETALHE_CONTATO" }, { "@idParceiro", hddidCliente.Value }, { "@idContato", hddidContato.Value }, { "@idCliente_Endereco", hddidEndereco_Fiscal.Value } });
                DataSet dsEntrega = ExecutarDataSet(sProcedure_Clientes, new Dictionary<string, string> { { "@sFuncao", "CONSULTA_DETALHE_ENDERECO" }, { "@idCliente_Endereco", hddidEndereco_Entrega.Value } });

                DateTime.TryParse(hddDtAtualizacaoPedido.Value, out DateTime dtAtualizacaoOrcamento);
                DateTime.TryParse(DATASET(dsTabela, "dtVigencia_Inicial"), out DateTime dtVigencia_Inicial);
                DateTime.TryParse(DATASET(dsTabela, "dtVigencia_Final"), out DateTime dtVigencia_Final);

                string lpu = $"Data de Vigência: {dtVigencia_Inicial:dd/MM/yyyy} - {dtVigencia_Final:dd/MM/yyyy}\r\nTaxa de Câmbio: {DATASET(dsTabela, "sSimboloMoedaOrigem")} / {DATASET(dsTabela, "sSimboloMoedaDestino")} = {DATASET(dsTabela, "nTaxaCambio")}";

                string sDscEmpresa = DATASET(dsEmpresa, 2, 0, "sDscEmpresa");
                string sEnderecoEmpresa = string.Format("{0}\r\n{1}", DATASET(dsEmpresa, 2, 0, "sEnderecoEmpresa_1"), DATASET(dsEmpresa, 2, 0, "sEnderecoEmpresa_2"));
                string CNPJ = DATASET(dsEmpresa, 2, 0, "CNPJ");
                string sIE_Empresa = DATASET(dsEmpresa, 2, 0, "sIE_Empresa");

                string sDscEndereco_Cliente = string.Format("{0}\r\n{1}", DATASET(dsCliente, "sEndereco_Cliente_1"), DATASET(dsCliente, "sEndereco_Cliente_2"));
                string sContato_Cliente = DATASET(dsCliente, "sNome");

                sContato_Cliente += string.IsNullOrEmpty(DATASET(dsCliente, "sTelefone")) ? string.Empty : " / " + DATASET(dsCliente, "sTelefone");
                sContato_Cliente += string.IsNullOrEmpty(DATASET(dsCliente, "sEmail")) ? string.Empty : " / " + DATASET(dsCliente, "sEmail");

                string sDscEndereco_Entrega = hddidEndereco_Entrega.Value == "-1" ? "Coleta" : string.Format("{0} {1}", DATASET(dsEntrega, "sEndereco_1"), DATASET(dsEntrega, "sEndereco_2"));

                decimal.TryParse(txtTotal_View.Text, out decimal totalGeral);
                decimal.TryParse(txtFrete_View.Text, out decimal frete);
                decimal.TryParse(txtCusto_Aduaneiro_View.Text, out decimal nCusto_Aduaneiro);
                decimal.TryParse(txtCusto_Despachante_View.Text, out decimal nCusto_Despachante);
                int.TryParse(hddMoeda.Value, out int idMoeda);

                string produtos_servicos = "";

                if (list_Prod.Count > 0) produtos_servicos += "Produtos_";
                if (list_Serv.Count > 0) produtos_servicos += "_Servicos";

                if (Convert.ToBoolean(hddOcamento_Empreitada.Value))
                {
                    sComposicao = "N";
                    produtos_servicos = produtos_servicos.Replace("Produtos_", "Materiais_");
                }

                if (cbLPU.Checked)
                    produtos_servicos = produtos_servicos.Replace("Produtos_", "LPU_");

                ReportParameter[] rp = new ReportParameter[52];

                rp[0] = new ReportParameter("Empresa_RazaoSocial", sDscEmpresa != null && sDscEmpresa.Length > 0 ? sDscEmpresa : "N/A");
                rp[1] = new ReportParameter("Empresa_Endereco", sEnderecoEmpresa.Replace("\r\n", "").Length > 0 ? sEnderecoEmpresa : "N/A");
                rp[2] = new ReportParameter("Empresa_CNPJ", string.IsNullOrWhiteSpace(CNPJ) ? "N/A" : Formatar_CNPJ_CPF(CNPJ));
                rp[3] = new ReportParameter("Empresa_IE", sIE_Empresa != null && sIE_Empresa.Length > 0 ? sIE_Empresa : "N/A");
                rp[4] = new ReportParameter("NumeroOrcamento", nNumeroPedido.PadLeft(6, '0'));
                rp[5] = new ReportParameter("DataOrcamento", dtAtualizacaoOrcamento.ToString("dd/MM/yyyy"));
                rp[6] = new ReportParameter("Cliente_RazaoSocial", txtRazaoSocial_View.Text.Length > 0 ? txtRazaoSocial_View.Text : "N/A");
                rp[7] = new ReportParameter("Cliente_Endereco", string.IsNullOrEmpty(sDscEndereco_Cliente.Trim().Replace("\r\n", "")) ? "N/A" : sDscEndereco_Cliente);
                rp[8] = new ReportParameter("Cliente_CNPJ_CPF", string.IsNullOrWhiteSpace(txtCNPJ_View.Text) ? "N/A" : bNacional ? RemovePontuacao_CNPJ_CPF(txtCNPJ_View.Text).Length == 14 ? $"<b>CNPJ:</b> {Formatar_CNPJ_CPF(txtCNPJ_View.Text)}" : RemovePontuacao_CNPJ_CPF(txtCNPJ_View.Text).Length == 11 ? $"<b>CPF:</b> {Formatar_CNPJ_CPF(txtCNPJ_View.Text)}" : txtCNPJ_View.Text : $"<b>Tax ID:</b> {Formatar_CNPJ_CPF(txtCNPJ_View.Text)}");
                rp[9] = new ReportParameter("Referencia", string.IsNullOrEmpty(txtReferencia_View.Text) ? "N/A" : string.Format("{0}{1}", string.IsNullOrEmpty(txtControle_TT_View.Text) ? "" : $"{txtControle_TT_View.Text} - ", txtReferencia_View.Text));
                rp[10] = new ReportParameter("Total_Produtos", (totalProdutos - totalIPI).ToString("N2", formato));
                rp[11] = new ReportParameter("Total_Desconto", totalDesconto.ToString("N2", formato));
                rp[12] = new ReportParameter("Total_IPI", totalIPI.ToString("N2", formato));
                rp[13] = new ReportParameter("Total_Geral", (totalGeral - frete - nCusto_Aduaneiro - nCusto_Despachante).ToString("N2", formato));
                rp[14] = new ReportParameter("Cond_Pagamento", txtPagamento_View.Text.Length > 0 ? txtPagamento_View.Text : "N/A");
                rp[15] = new ReportParameter("Peso_Bruto", PesoBruto.ToString("N2", formato));
                rp[16] = new ReportParameter("Peso_Liq", PesoLiq.ToString("N2", formato));
                rp[17] = new ReportParameter("Base_Calc_ICMS", baseCalc_ICMS.ToString("N2", formato));
                rp[18] = new ReportParameter("Transportadora", txtTransporte_View.Text.Length > 0 ? txtTransporte_View.Text : "N/A");
                rp[19] = new ReportParameter("Vendedor", txtVendedor_View.Text.Length > 0 ? txtVendedor_View.Text : "N/A");
                rp[20] = new ReportParameter("Cliente_Contato", string.IsNullOrEmpty(sContato_Cliente) ? "N/A" : sContato_Cliente);
                rp[21] = new ReportParameter("Obs", txtObs_View.Text.Length > 0 ? txtObs_View.Text : "N/A");
                rp[22] = new ReportParameter("Endereco_Entrega", string.IsNullOrEmpty(sDscEndereco_Entrega) ? "N/A" : sDscEndereco_Entrega);
                rp[23] = new ReportParameter("Valor_ICMS", totalICMS.ToString("N2", formato));
                rp[24] = new ReportParameter("Base_Cal_ICMS_ST", 0.ToString("N2", formato));
                rp[25] = new ReportParameter("Valor_ICMS_ST", totalST.ToString("N2", formato));
                rp[26] = new ReportParameter("Valor_DIFAL", (bNacional ? totalDIFAL : nCusto_Despachante).ToString("N2", formato));
                rp[27] = new ReportParameter("Valor_Frete", txtFrete_View.Text.Length > 0 ? decimal.Parse(txtFrete_View.Text).ToString("N2", formato) : "0,00");
                rp[28] = new ReportParameter("Valor_Seguro", 0.ToString("N2", formato));
                rp[29] = new ReportParameter("Outras_Despesas", (bNacional ? 0 : nCusto_Aduaneiro).ToString("N2", formato));
                rp[30] = new ReportParameter("Destino_Venda", txtDestinoVenda_View.Text.Length > 0 ? txtDestinoVenda_View.Text : "N/A");
                rp[31] = new ReportParameter("Forma_Envio", txtFormaEnvio_View.Text.Length > 0 ? txtFormaEnvio_View.Text : "N/A");
                rp[32] = new ReportParameter("Total_Extenso", EscreverExtenso(Math.Round(cb_Frete.Checked ? totalGeral : totalGeral - frete, 2), bNacional ? Idioma.ptBR : Idioma.enUS, bNacional ? Moeda.BRL : (Moeda)idMoeda));
                rp[33] = bNacional ? new ReportParameter("Guia_DIFAL", totalDIFAL > decimal.Zero ? "X" : " ") : new ReportParameter("Simbolo_Moeda", IsNull(hddMoeda_Simbolo.Value, "R$"));
                rp[34] = new ReportParameter("Validade_Orcamento", txtValidade_View.Text.Length > 0 ? txtValidade_View.Text : "5");
                rp[35] = new ReportParameter("Total_Servicos", totalServicos.ToString("N2", formato));
                rp[36] = new ReportParameter("Espaco", " ");
                rp[37] = new ReportParameter("TotalPaginas", "0");
                rp[38] = new ReportParameter("Total_Orcamento", (cb_Frete.Checked ? totalGeral : totalGeral - frete).ToString("N2", formato));
                rp[39] = new ReportParameter("Aparece_Produtos_Servicos", produtos_servicos);
                rp[40] = new ReportParameter("Valor_INSS", totalINSS.ToString("N2", formato));
                rp[41] = new ReportParameter("Valor_ISS", totalISS.ToString("N2", formato));
                rp[42] = new ReportParameter("Valor_PIS", totalPIS.ToString("N2", formato));
                rp[43] = new ReportParameter("Valor_COFINS", totalCOFINS.ToString("N2", formato));
                rp[44] = new ReportParameter("IRPJ", $"{hddMoeda_Simbolo.Value} {totalIRPJ.ToString("N2", formato)}");
                rp[45] = new ReportParameter("CSSL", $"{hddMoeda_Simbolo.Value} {totalCSSL.ToString("N2", formato)}");
                rp[46] = new ReportParameter("Aparece_Composicao", sComposicao);
                rp[47] = new ReportParameter("Aparece_Assinatura", dt_CheckList.Rows.Count > 0 && cbEscopos_PDF.Checked ? "S" : "N");
                rp[48] = new ReportParameter("Assinatura", " ");
                rp[49] = new ReportParameter("Detalhes_LPU", lpu);
                rp[50] = new ReportParameter("Aparece_Frete", cb_Frete.Checked ? "S" : "N");
                rp[51] = new ReportParameter("Valor_Reducao_BC_ICMS", reducaoBaseCalc_ICMS.ToString("N2", formato));

                rv.LocalReport.SetParameters(rp);
                rv.LocalReport.Refresh();

                string sNomeArquivo = string.Format("{3}_{0}_{1}.{2}", nNumeroPedido, CarimboDataHora(), nFormato.Equals(1) ? "pdf" : "xlsx", txtReferencia_View.Text.Replace("-", " ").Replace(" ", "_").Replace("___", "_").Replace("__", "_").Replace("<", "").Replace(">", "")
                                                                                                                                                        .Replace("$", "").Replace("%", "").Replace("&", "").Replace("/", "").Replace("\\", "").Replace("|", "").Replace("?", "").Replace("@", "")
                                                                                                                                                        .Replace("=", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace(";", ""));
                int totalPaginas_PDF = nTotalPaginas;
                bool bNovaPagina = false;

                byte[] bytes;
                string[] streamIds;
                string mimeType, encoding, extension;
                Warning[] warnings;

                if (nFormato.Equals(1))
                {
                    bytes = rv.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);

                    PdfReader reader = new PdfReader(Server.MapPath("~/Download/") + sNomeArquivo);
                    Text_Location strategy = new Text_Location();
                    PdfTextExtractor.GetTextFromPage(reader, reader.NumberOfPages, strategy);

                    foreach (var chunk in strategy.TextChunks)
                    {
                        if (chunk.Text.Contains("Data:"))
                        {
                            if (chunk.Y < 145)
                            {
                                bNovaPagina = true;
                                totalPaginas_PDF++;
                            }
                        }
                    }

                    reader.Close();

                    totalPaginas_PDF += reader.NumberOfPages;

                    File.Delete(Server.MapPath("~/Download/") + sNomeArquivo);

                    rp[37] = new ReportParameter("TotalPaginas", totalPaginas_PDF.ToString());

                    rv.LocalReport.SetParameters(rp);
                    rv.LocalReport.Refresh();
                }

                bytes = rv.LocalReport.Render(nFormato.Equals(1) ? "PDF" : "Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);

                if (nFormato.Equals(1) && !bChecklists_PDF)
                {
                    var imgFornecedores = Conuslta_ImagemFornecedores();

                    if (imgFornecedores != null)
                    {
                        PdfReader reader = new PdfReader(Server.MapPath("~/Download/") + sNomeArquivo);

                        sNomeArquivo = sNomeArquivo.Replace(".pdf", "_Rodape.pdf");

                        using (FileStream fs = new FileStream(Server.MapPath("~/Download/") + sNomeArquivo, FileMode.Create, FileAccess.Write))
                        {
                            if (!bNovaPagina)
                            {
                                PdfStamper pdfStamper = new PdfStamper(reader, fs);

                                Rectangle pageSize = reader.GetPageSizeWithRotation(reader.NumberOfPages);

                                PdfContentByte cb = pdfStamper.GetOverContent(reader.NumberOfPages);

                                PdfPTable rodape = new PdfPTable(1)
                                {
                                    TotalWidth = pageSize.Width - 17,
                                    HorizontalAlignment = Element.ALIGN_CENTER
                                };

                                PdfPCell ImagemFornecedores = new PdfPCell(Image.GetInstance(imgFornecedores), true)
                                {
                                    FixedHeight = 120,
                                    Border = Rectangle.NO_BORDER
                                };

                                rodape.AddCell(ImagemFornecedores);

                                rodape.WriteSelectedRows(0, -1, 9, 125, cb);

                                pdfStamper.Close();
                            }
                            else
                            {
                                PdfStamper pdfStamper = new PdfStamper(reader, fs);

                                pdfStamper.InsertPage(totalPaginas_PDF, reader.GetPageSizeWithRotation(totalPaginas_PDF - 1));

                                Rectangle pageSize = reader.GetPageSizeWithRotation(totalPaginas_PDF);

                                PdfContentByte cb = pdfStamper.GetOverContent(totalPaginas_PDF);

                                PdfPTable cabecalho = new PdfPTable(2)
                                {
                                    TotalWidth = pageSize.Width - 110
                                };
                                cabecalho.SetWidths(new float[] { 20, 80 });

                                Image imagem = Image.GetInstance(Combine(AppDomain.CurrentDomain.BaseDirectory, "App", "img", "LogoTT_Horizontal.png"));
                                imagem.ScaleAbsolute(100f, 100f);

                                PdfPCell celulaLogo = new PdfPCell(imagem, true)
                                {
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    MinimumHeight = 60,
                                    Border = 0
                                };
                                cabecalho.AddCell(celulaLogo);

                                PdfPCell celulaTitulo = new PdfPCell(new Phrase("Orçamento de Vendas", FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD, BaseColor.BLACK)))
                                {
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    MinimumHeight = 50,
                                    Border = 0
                                };
                                cabecalho.AddCell(celulaTitulo);

                                cabecalho.WriteSelectedRows(0, -1, 9.5f, pageSize.Top - 30, cb);

                                PdfPTable rodape = new PdfPTable(1)
                                {
                                    TotalWidth = pageSize.Width - 17,
                                    HorizontalAlignment = Element.ALIGN_CENTER
                                };

                                PdfPCell ImagemFornecedores = new PdfPCell(Image.GetInstance(imgFornecedores), true)
                                {
                                    FixedHeight = 120,
                                    Border = Rectangle.NO_BORDER
                                };

                                PdfPCell pagina = new PdfPCell(new Phrase(string.Format("Página - {0} / {1}", totalPaginas_PDF, totalPaginas_PDF), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8)))
                                {
                                    Border = Rectangle.NO_BORDER,
                                    HorizontalAlignment = Element.ALIGN_RIGHT
                                };

                                rodape.AddCell(ImagemFornecedores);
                                rodape.AddCell(pagina);

                                rodape.WriteSelectedRows(0, -1, 9, 140, cb);

                                if (Request["id"] == "0" || Convert.ToBoolean(Request["duplicar"]) || Convert.ToBoolean(Request["revisao"]))
                                {
                                    Image img = Image.GetInstance(Combine(AppDomain.CurrentDomain.BaseDirectory, "App", "img", "Rascunho_Marca_Dagua.png"));

                                    img.SetAbsolutePosition(pageSize.Left + 25, pageSize.Bottom + 50);
                                    img.ScaleToFit(pageSize.Width, pageSize.Height - 100);

                                    PdfGState gstate = new PdfGState
                                    {
                                        FillOpacity = 0.45f,
                                        StrokeOpacity = 0.45f
                                    };
                                    cb.SetGState(gstate);
                                    cb.AddImage(img);
                                }
                                else
                                {
                                    Image img = Image.GetInstance(Combine(AppDomain.CurrentDomain.BaseDirectory, "App", "img", "LogoTT_Horizontal.png"));
                                    img.RotationDegrees = 30;

                                    img.SetAbsolutePosition(pageSize.Left + 100, pageSize.Bottom + 50);
                                    img.ScaleToFit(pageSize.Width, pageSize.Height - 100);

                                    PdfGState gstate = new PdfGState
                                    {
                                        FillOpacity = 0.15f,
                                        StrokeOpacity = 0.15f
                                    };
                                    cb.SetGState(gstate);
                                    cb.AddImage(img);
                                }

                                pdfStamper.Close();
                            }

                            reader.Close();
                        }
                    }
                }

                list_PDFs.Add(sNomeArquivo);

                return totalPaginas_PDF;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? "<br />- " + ex.InnerException.InnerException != null ? ex.InnerException.InnerException.ToString() : ex.InnerException.Message : string.Empty + "<br />- " + ex.Message);
            }
        }

        /// <summary>
        /// Função utilizada para gerar a parte do Documento PDF onde ficam os Escopos, pelo 'iTextSharp'.
        /// </summary>
        /// <returns>Retorna o nome do arquivo do Documento gerado.</returns>
        protected string GerarPDF_Orcamento_Escopos()
        {
            string sCaminho = "Escopos_Orcamento_" + CarimboDataHora() + "_Parcial_02" + ".pdf";

            using (FileStream fs = new FileStream(Server.MapPath("~/Download/") + sCaminho, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Document doc = new Document(PageSize.A4.Rotate(), 55, 55, 100, 175);

                PdfWriter writer = PdfWriter.GetInstance(doc, fs);

                PDF_Header pdfHeader = new PDF_Header();
                writer.PageEvent = pdfHeader;

                doc.Open();

                List<Escopo> escopos = new List<Escopo>();

                foreach (DataRow row in dt_CheckList.Rows)
                {
                    var dados = new Escopo();
                    var linhas = new List<string[]>();
                    int nOpcoes = row["sOpcoes"].ToString().Split('|').Count(o => o.Length > 0);
                    int n = 1;

                    dados.Categoria = row["sDscCategoria"].ToString();
                    dados.Cabeçalho = new string[nOpcoes + 1];
                    dados.Cabeçalho[0] = "Tópicos";

                    foreach (var o in row["sOpcoes"].ToString().Split('|').Where(o => o.Length > 0))
                    {
                        dados.Cabeçalho[n] = o;
                        n++;
                    }

                    n = 0;

                    foreach (var p in row["sPerguntas"].ToString().Split('|').Where(p => p.Length > 0))
                    {
                        string[] linha = new string[nOpcoes + 1];

                        linha[0] = p;

                        try
                        {
                            for (int i = 0; i < nOpcoes; i++)
                            {
                                linha[i + 1] = list_Perguntas_x_Opcoes.FirstOrDefault(po => po.idEscopo.ToString().Equals(row["idEscopo"].ToString()) && po.idCategoria.ToString().Equals(row["idCategoria"].ToString()) && po.sPerguntas.ToString().Equals(p)).sOpcoes.Split('|')[1].Equals(row["sOpcoes"].ToString().Split('|').Where(o => o.Length > 0).ToArray()[i]) ? "X" : " ";
                            }

                            linhas.Add(linha);
                        }
                        catch
                        {
                            for (int i = 0; i < nOpcoes; i++)
                            {
                                var list = list_Perguntas_x_Opcoes.Where(po => po.idEscopo.ToString().Equals(row["idEscopo"].ToString()) && po.idCategoria.ToString().Equals(row["idCategoria"].ToString()) && po.sPerguntas.Split('|').Length > 1);
                                var pergunta_x_opcao = list.FirstOrDefault(po => po.sPerguntas.Split('|')[1].Equals(p) || po.sPerguntas.Split('|')[0].Equals(n.ToString()));

                                if (pergunta_x_opcao != null)
                                {
                                    if (!string.IsNullOrEmpty(pergunta_x_opcao.sOpcoes.Split('|')[1].Replace("&nbsp;", string.Empty)))
                                        linha[i + 1] = pergunta_x_opcao.sOpcoes.Split('|')[1].Equals(row["sOpcoes"].ToString().Split('|').Where(o => o.Length > 0).ToArray()[i]) ? "X" : " ";
                                    else
                                        linha[1] = "X";
                                }
                            }

                            linhas.Add(linha);
                        }

                        n++;
                    }

                    dados.Linhas = linhas;

                    escopos.Add(dados);
                }

                foreach (var escopo in escopos)
                {
                    PdfPTable table = new PdfPTable(escopo.Cabeçalho.Length)
                    {
                        WidthPercentage = 112.5f,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        SpacingBefore = 20
                    };

                    PdfPCell categoria = new PdfPCell(new Phrase(escopo.Categoria, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8)))
                    {
                        BackgroundColor = new BaseColor(0, 140, 0),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Colspan = escopo.Cabeçalho.Length,
                        MinimumHeight = 18.5f
                    };
                    categoria.Phrase.Font.Color = BaseColor.WHITE;

                    table.AddCell(categoria);

                    foreach (var header in escopo.Cabeçalho)
                    {
                        PdfPCell headerCell = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8)))
                        {
                            BackgroundColor = new BaseColor(0, 140, 0),
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            MinimumHeight = 22
                        };
                        headerCell.Phrase.Font.Color = BaseColor.WHITE;

                        table.AddCell(headerCell);
                    }

                    foreach (var row in escopo.Linhas)
                    {
                        int i = 0;

                        foreach (var cell in row)
                        {
                            PdfPCell cellItem = new PdfPCell(new Phrase(cell, FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                            {
                                HorizontalAlignment = i == 0 ? Element.ALIGN_LEFT : Element.ALIGN_CENTER,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                MinimumHeight = 18.5f
                            };
                            table.AddCell(cellItem);

                            i++;
                        }
                    }

                    float[] widths = new float[table.NumberOfColumns];

                    widths[0] = 75f;

                    for (int i = 1; i <= table.NumberOfColumns - 1; i++)
                    {
                        widths[i] = 6.25f;
                    }

                    table.SetWidths(widths);

                    doc.Add(table);
                }

                PdfPTable tabelaAssinatura = new PdfPTable(6)
                {
                    WidthPercentage = 112.5f,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    SpacingBefore = 5
                };

                Phrase razao = new Phrase();
                Chunk chunk_1 = new Chunk("Razão Social: ", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8));
                Chunk chunk_2 = new Chunk(txtRazaoSocial_View.Text.Length > 0 ? txtRazaoSocial_View.Text : "N/A", FontFactory.GetFont(FontFactory.HELVETICA, 8));
                razao.Add(chunk_1);
                razao.Add(chunk_2);

                Phrase cpf = new Phrase();
                Chunk chunk_3 = new Chunk(txtCNPJ_View.Text.Length > 0 ? txtCNPJ_View.Text.Replace(".", "").Replace("-", "").Replace("/", "").Length == 14 ? "CNPJ: " : txtCNPJ_View.Text.Replace(".", "").Replace("-", "").Replace("/", "").Length == 11 ? "CPF: " : "" : "N/A", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8));
                Chunk chunk_4 = new Chunk(txtCNPJ_View.Text.Length > 0 ? txtCNPJ_View.Text.Replace(".", "").Replace("-", "").Replace("/", "").Length == 14 ? Convert.ToInt64(txtCNPJ_View.Text.Replace(".", "").Replace("-", "").Replace("/", "")).ToString(@"00\.000\.000\/0000\-00") : txtCNPJ_View.Text.Replace(".", "").Replace("-", "").Replace("/", "").Length == 11 ? Convert.ToInt64(txtCNPJ_View.Text.Replace(".", "").Replace("-", "").Replace("/", "")).ToString(@"000\.000\.000\-00") : txtCNPJ_View.Text : "N/A", FontFactory.GetFont(FontFactory.HELVETICA, 8));
                cpf.Add(chunk_3);
                cpf.Add(chunk_4);

                PdfPCell razaoSocial = new PdfPCell(razao)
                {
                    BackgroundColor = BaseColor.WHITE,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_BOTTOM,
                    MinimumHeight = 22.5f,
                    BorderColor = BaseColor.WHITE,
                    Colspan = 4
                };

                PdfPCell CPF_CNPJ = new PdfPCell(cpf)
                {
                    BackgroundColor = BaseColor.WHITE,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_BOTTOM,
                    MinimumHeight = 22.5f,
                    BorderColor = BaseColor.WHITE,
                    Colspan = 4
                };

                PdfPCell Nome = new PdfPCell(new Phrase("Nome:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8)))
                {
                    BackgroundColor = BaseColor.WHITE,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_BOTTOM,
                    MinimumHeight = 22.5f,
                    BorderColor = BaseColor.WHITE,
                    Colspan = 1
                };

                PdfPCell Data = new PdfPCell(new Phrase("Data:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8)))
                {
                    BackgroundColor = BaseColor.WHITE,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_BOTTOM,
                    MinimumHeight = 22.5f,
                    BorderColor = BaseColor.WHITE,
                    Colspan = 1
                };

                PdfPCell CampoAssinatura = new PdfPCell(new Phrase("Assinatura / Carimbo", FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                {
                    BackgroundColor = BaseColor.WHITE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    MinimumHeight = 22.5f,
                    BorderColor = BaseColor.WHITE,
                    BorderColorTop = BaseColor.BLACK,
                    BorderWidthTop = 1.3f,
                    Colspan = 1
                };

                PdfPCell EspacoAssinatura = new PdfPCell(new Phrase(" "))
                {
                    BackgroundColor = BaseColor.WHITE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    MinimumHeight = 22.5f,
                    BorderColor = BaseColor.WHITE,
                    Colspan = 1,
                    Rowspan = 3
                };

                PdfPCell Sublinhado = new PdfPCell(new Phrase(" ", FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                {
                    BackgroundColor = BaseColor.WHITE,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_BOTTOM,
                    MinimumHeight = 22.5f,
                    BorderColor = BaseColor.WHITE,
                    BorderColorBottom = BaseColor.BLACK,
                    BorderWidthBottom = 1.25f,
                    Colspan = 1
                };

                PdfPCell Espaço = new PdfPCell(new Phrase(" ", FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                {
                    BackgroundColor = BaseColor.WHITE,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_BOTTOM,
                    MinimumHeight = 22.5f,
                    BorderColor = BaseColor.WHITE,
                    Colspan = 1
                };

                tabelaAssinatura.AddCell(razaoSocial);
                tabelaAssinatura.AddCell(Espaço);
                tabelaAssinatura.AddCell(EspacoAssinatura);
                tabelaAssinatura.AddCell(CPF_CNPJ);
                tabelaAssinatura.AddCell(Espaço);
                tabelaAssinatura.AddCell(Nome);
                tabelaAssinatura.AddCell(Sublinhado);
                tabelaAssinatura.AddCell(Sublinhado);
                tabelaAssinatura.AddCell(Espaço);
                tabelaAssinatura.AddCell(Espaço);
                tabelaAssinatura.AddCell(Data);
                tabelaAssinatura.AddCell(Sublinhado);
                tabelaAssinatura.AddCell(Espaço);
                tabelaAssinatura.AddCell(Espaço);
                tabelaAssinatura.AddCell(Espaço);
                tabelaAssinatura.AddCell(CampoAssinatura);

                tabelaAssinatura.SetWidths(new float[6] { 4, 11.5f, 20, 15.5f, 15, 34 });

                doc.Add(tabelaAssinatura);

                doc.Close();
                writer.Close();
            }

            return sCaminho;
        }

        protected void GeraPDFCombinado()
        {
            try
            {
                string[] arquivosParaCombinar = new string[list_PDFs.Count];

                for (int i = 0; i < list_PDFs.Count; i++)
                {
                    arquivosParaCombinar[i] = Server.MapPath("~/Download/") + list_PDFs[i];
                }

                string nomePersonalizado = string.Format("{0}.pdf", txtReferencia_View.Text.Replace("-", " ").Replace(" ", "_").Replace("___", "_").Replace("__", "_")
                                                                                            .Replace("!", "").Replace("@", "").Replace("#", "").Replace("$", "").Replace("%", "").Replace("¨", "").Replace("&", "").Replace("*", "")
                                                                                            .Replace("(", "").Replace(")", "").Replace("=", "").Replace("+", "").Replace("§", "").Replace("[", "").Replace("]", "").Replace("{", "")
                                                                                            .Replace("}", "").Replace("ª", "").Replace("º", "").Replace("\\", "").Replace("|", "").Replace("<", "").Replace(">", "").Replace(";", "")
                                                                                            .Replace(":", "").Replace("/", "").Replace("?", "").Replace("°", "").Replace("\"", "").Replace("'", ""));

                string arquivoFinal = Server.MapPath("~/Download/") + nomePersonalizado;

                CombinePDFs(arquivosParaCombinar, arquivoFinal);

                DownloadArquivo(Page, nomePersonalizado);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Método utilizado para aplicar o Rodapé à parte do Documento PDF gerado pelo 'iTextSharp'.
        /// </summary>
        /// <param name="sNomeArquivo">Recebe o nome do Arquivo a ser aplicado o Rodapé.</param>
        /// <param name="nTotalPaginas">Recebe o total de Páginas do Documento já completo, incluindo todas as suas partes.</param>
        protected string Adiciona_Rodape(string sNomeArquivo, int nTotalPaginas)
        {
            var imgFornecedores = Conuslta_ImagemFornecedores();

            if (imgFornecedores is null)
                return sNomeArquivo;

            PdfReader pdfReader = new PdfReader(Server.MapPath("~/Download/") + sNomeArquivo);

            int nPaginaInicial = nTotalPaginas + 1 - pdfReader.NumberOfPages;

            sNomeArquivo = sNomeArquivo.Replace(".pdf", "_Rodape.pdf");

            using (FileStream fs = new FileStream(Server.MapPath("~/Download/") + sNomeArquivo, FileMode.Create, FileAccess.Write))
            {
                PdfStamper pdfStamper = new PdfStamper(pdfReader, fs);

                for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                {
                    Rectangle pageSize = pdfReader.GetPageSizeWithRotation(i);

                    PdfContentByte cb = pdfStamper.GetOverContent(i);

                    PdfPTable rodape = new PdfPTable(1)
                    {
                        TotalWidth = pageSize.Width - 17,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };

                    if (i == pdfReader.NumberOfPages)
                    {
                        PdfPCell ImagemFornecedores = new PdfPCell(Image.GetInstance(imgFornecedores), true)
                        {
                            FixedHeight = 120,
                            Border = Rectangle.NO_BORDER
                        };
                        rodape.AddCell(ImagemFornecedores);
                    }

                    PdfPCell pagina = new PdfPCell(new Phrase(string.Format("Página - {0} / {1}", nPaginaInicial, nTotalPaginas), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8)))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    rodape.AddCell(pagina);

                    rodape.WriteSelectedRows(0, -1, 9, i == pdfReader.NumberOfPages ? 140 : 25, cb);

                    nPaginaInicial++;
                }

                pdfStamper.Close();
            }

            pdfReader.Close();

            return sNomeArquivo;
        }

        #endregion
    }
}