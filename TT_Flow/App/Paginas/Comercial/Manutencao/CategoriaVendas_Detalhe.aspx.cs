using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.Data;
using System.Globalization;
                             
namespace TT_Flow.App.Paginas.Comercial.Manutencao
{
    public partial class CategoriaVendas_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Categoria de Vendas";
        string sProcedure = "sp_Manipula_tbl_Flow_Comercial_CategoriaVendas";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Comercial.CategoriaVendas.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Comercial.CategoriaVendas.Incluir, true);
                    Pesquisar("0");
                }
            }

        }
        private bool ValidarDados()
        {
            if (Validacoes.ValidarTexto(txtsDscCategoriaVendas))
            {
                MensagemPagina.MostraMensagem_Erro("Informe um Descrição válida!");
                return false;
            }
            if (txtsCodigoCategoriaVendas.Text.Trim().Length > 20)
            {
                MensagemPagina.MostraMensagem_Erro("O Código pode ter no máximo 20 caracteres!");
                return false;
            }
            if (Validacoes.ValidarTexto(txtnPadraoHHInstalacao))
            {
                MensagemPagina.MostraMensagem_Erro("Informe o Padrão de HH Instalação!");
                return false;
            }
            // A aplicação roda em pt-BR (Web.config: globalization culture="pt-BR"), onde o
            // ponto é separador de MILHAR. Sem esta checagem, "2.5" seria lido como 25 --
            // um erro de 10x silencioso num campo que alimenta o preço da instalação.
            if (txtnPadraoHHInstalacao.Text.Contains("."))
            {
                MensagemPagina.MostraMensagem_Erro("Use vírgula como separador decimal no Padrão de HH Instalação (exemplo: 2,5).");
                return false;
            }
            if (!Validacoes.ValidarMoeda(txtnPadraoHHInstalacao))
            {
                MensagemPagina.MostraMensagem_Erro("O Padrão de HH Instalação deve ser um valor numérico!");
                return false;
            }
            // Zero é permitido aqui: significa "esta categoria não gera horas de instalação".
            // Diferente da configuração por Produto (aba Instalação/Obra), onde a linha só
            // existe para declarar horas e por isso exige HH maior que zero.
            if (ConverterHH(txtnPadraoHHInstalacao.Text) < 0)
            {
                MensagemPagina.MostraMensagem_Erro("O Padrão de HH Instalação não pode ser negativo!");
                return false;
            }
            return true;
        }

        static readonly CultureInfo culturaBR = CultureInfo.CreateSpecificCulture("pt-BR");

        /// <summary>
        /// Converte para decimal aceitando tanto o texto digitado pelo usuário ("2,5")
        /// quanto o valor devolvido pelo banco ("2.5000"), sem confundir com separador
        /// de milhar.
        /// </summary>
        static decimal ConverterHH(string sValor)
        {
            string sTexto = (sValor ?? "").Trim();

            if (decimal.TryParse(sTexto, NumberStyles.Number, culturaBR, out decimal nValor))
                return nValor;

            if (decimal.TryParse(sTexto, NumberStyles.Number, CultureInfo.InvariantCulture, out nValor))
                return nValor;

            return decimal.Zero;
        }

        /// <summary>
        /// Formata o valor vindo do banco para exibição, deixando o campo vazio quando
        /// a categoria ainda não tem padrão configurado (NULL no banco).
        /// </summary>
        static string FormatarHH(string sValorBanco)
        {
            if (string.IsNullOrWhiteSpace(sValorBanco))
                return "";

            return ConverterHH(sValorBanco).ToString("N2", culturaBR);
        }

        protected void Pesquisar(string idPesquisa)
        {
            string sErro = "";
            try
            {
                LimpaCampos();
                if (idPesquisa != "0")
                {
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao",         "CONSULTAR_DETALHE");
                    vParametros.Add("@idCategoriaVendas", idPesquisa);
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidCategoriaVendas.Value              = RETORNO.DATASET(dsPesquisa, 0, "idCategoriaVendas");
                        txtidCategoriaVendas.Text               = RETORNO.DATASET(dsPesquisa, 0, "idCategoriaVendas");
                        txtsDscCategoriaVendas.Text             = RETORNO.DATASET(dsPesquisa, 0, "sDscCategoriaVendas");
                        txtsCodigoCategoriaVendas.Text          = RETORNO.DATASET(dsPesquisa, 0, "sCodigoCategoriaVendas");
                        txtnPadraoHHInstalacao.Text             = FormatarHH(RETORNO.DATASET(dsPesquisa, 0, "nPadraoHHInstalacao"));

                        ddlsExibeAcervo.SelectedValue           = RETORNO.DATASET(dsPesquisa, 0, "sExibeAcervo");   //Agnes Partal * 04/07/2024 

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscCategoriaVendas.Text);
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Comercial.CategoriaVendas.Alterar);
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    BreadCrumb_Pagina.TitulodaPagina = "Novo";
                    lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                    cmdSalvar.Text = "Incluir";
                }

                txtsDscCategoriaVendas.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }
        void LimpaCampos()
        {
            txtidCategoriaVendas.Text = "Novo";
            txtsDscCategoriaVendas.Text = "";
            txtsCodigoCategoriaVendas.Text = "";
            txtnPadraoHHInstalacao.Text = "";
            hddidCategoriaVendas.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }


        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidCategoriaVendas = hddidCategoriaVendas.Value.Split(',');
                    string idCategoriaVendas = vidCategoriaVendas[0].ToString();

                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao",             "SALVAR");
                    vParametros.Add("@idCategoriaVendas",   idCategoriaVendas);
                    vParametros.Add("@sDscCategoriaVendas", txtsDscCategoriaVendas.Text);
                    vParametros.Add("@sCodigoCategoriaVendas", txtsCodigoCategoriaVendas.Text.Trim());
                    // Enviado com ponto decimal: o parâmetro no banco é DECIMAL(18,4).
                    vParametros.Add("@nPadraoHHInstalacao",  ConverterHH(txtnPadraoHHInstalacao.Text).ToString("0.####", CultureInfo.InvariantCulture));
                    vParametros.Add("@sSituacao",           ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    
                    //Agnes Partal * 04/07/2024 ------------------------------------------------
                    vParametros.Add("@sExibeAcervo",           ddlsExibeAcervo.SelectedValue);
                    //--------------------------------------------------------------------------

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idCategoriaVendas = RETORNO.DATASET(dsSalvar, 0, "idCategoriaVendas");
                        Pesquisar(idCategoriaVendas);
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
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
        }

    }
}