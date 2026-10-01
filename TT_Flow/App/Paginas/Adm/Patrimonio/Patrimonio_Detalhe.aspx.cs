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
using static Permissao;
using TT_Flow.FrameWork;
using System.Data.SqlClient;
using GRID = TT.FrameWork.Grid;
using TT_Hub.App.Paginas.Requisicao;
using System.Globalization;
using System.Linq;

namespace TT_Flow.App.Paginas.Adm.Patrimonio
{
    public partial class Patrimonio_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Patrimônio";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Patrimonio";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Patrimonio.ControlePatrimonio.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Patrimonio.ControlePatrimonio.Incluir, true);
                    Pesquisar("0", true);
                }

                // Verificar o valor atual do campo e ajustar a largura
                if (txtidPatrimonio.Text.Trim().Equals("Novo", StringComparison.OrdinalIgnoreCase))
                {
                    txtidPatrimonio.Style["width"] = "80px"; // Defina a largura desejada
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
                    Salvar_Patrimonio();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidPatrimonio.Value, true);
                }
            }

            RegistraScript("");

        }


        #endregion

        #region | Metodos Banco de Dados
        protected void Pesquisar(string idPatrimonio, bool bEdicao)
        {
            PopularCombos();

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idPatrimonio != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idPatrimonio", idPatrimonio);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
                    
                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidPatrimonio.Value = RETORNO.DATASET(dsPesquisa, 0, "idPatrimonio");
                        txtidPatrimonio.Text = hddidPatrimonio.Value;
                        txtsEtiquetaPatrimonio.Text = RETORNO.DATASET(dsPesquisa, 0, "sEtiquetaPatrimonio");
                        txtsDscPatrimonio.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscPatrimonio");
                        txtsMarca.Text = RETORNO.DATASET(dsPesquisa, 0, "sMarca");
                        txtsModelo.Text = RETORNO.DATASET(dsPesquisa, 0, "sModelo");
                        txtsCor.Text = RETORNO.DATASET(dsPesquisa, 0, "sCor");
                        txtNumeroSerie.Text = RETORNO.DATASET(dsPesquisa, 0, "nNumeroSerie");

                        if (RETORNO.DATASET(dsPesquisa, 0, "dtCompra").ToString() == "01/01/1900 00:00:00")
                        {
                            txtsdtCompra.Text = null;
                        }
                        else
                        {
                            var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtCompra").ToString());
                            txtsdtCompra.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                        }
                        ddlidGrupo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idGrupo");
                        txtsNotaFiscal.Text = RETORNO.DATASET(dsPesquisa, 0, "sNotaFiscal");
                        txtnValorCompra.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorCompra");
                        txtsObservacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacao");
                        ddlidCategoria.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCategoria");
                        ddlidEstadoConservacao.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEstadoConservacao");
                        ddlidParceiro.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idParceiro");
                        ddlidLocal.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idLocal");
                        hddidResponsavel.Value = RETORNO.DATASET(dsPesquisa, 0, "idResponsavel");
                        Popular_aba_Arquivo(idPatrimonio);
                        CarregaimgPatrimonio(hddidPatrimonio.Value);
                        Popular_Combo_idResponsavel(ddlidLocal.SelectedValue);
                        ddlidResponsavel.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idResponsavel");
                        ddlidStatus.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idStatus");//Status está com algum problema
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        if (Convert.ToBoolean(Request["duplicar"]))
                        {
                            txtidPatrimonio.Text = "";
                            aba_Movimentacao.Visible = false;
                            LimparCamposDuplicando();
                        }

                        lblTituloPagina.Text = string.Format("Patrimônio {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscPatrimonio"));
                        BreadCrumb.TitulodaPagina = string.Format("Patrimônio {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscPatrimonio"));

                        lblTituloSalvar.Text = "Confirma a Alteração do " + lblTituloPagina.Text + "?";

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Patrimonio.Categoria.Alterar);


                        aba_Movimentacao.Visible = true;
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
                    txtidPatrimonio.Text = "Novo";
                    cmAvancar.Visible = false;
                    cmRetornar.Visible = false;
                    DIV_IMG.Visible = false;
                    lblTituloSalvar.Text = "Confirma a Inclusão da Patrimonio?";
                    cmdSalvar.Text = "Incluir";
                    txtidPatrimonio.Focus();
                }
                RegistraScript("");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                
                if(ex.Message == "Nenhum Registro Encontrado")
                   Response.Redirect("Patrimonio_Detalhe.aspx");
            }

        }

        void Salvar_Patrimonio()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidPatrimonio = hddidPatrimonio.Value.Split(',');
                    string idPatrimonio = vidPatrimonio[0].ToString();

                    string[] vidResponsavel = hddidResponsavel.Value.Split(',');
                    string idResponsavel = vidResponsavel[0].ToString();


                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    if (Convert.ToBoolean(Request["duplicar"]))
                    {
                        txtidPatrimonio.Text = "";
                    }
                    else
                    {
                        txtidPatrimonio.Text = idPatrimonio;
                    }
                    vParametros.Add("@idPatrimonio", txtidPatrimonio.Text);
                    vParametros.Add("@sDscPatrimonio", txtsDscPatrimonio.Text);
                    vParametros.Add("@sEtiquetaPatrimonio", txtsEtiquetaPatrimonio.Text);
                    vParametros.Add("@sMarca", txtsMarca.Text);
                    vParametros.Add("@sModelo", txtsModelo.Text);
                    vParametros.Add("@sCor", txtsCor.Text);
                    vParametros.Add("@nNumeroSerie", txtNumeroSerie.Text);

                    if (txtsdtCompra.Text == "")
                    {
                        vParametros.Add("@dtCompra", txtsdtCompra.Text);
                    }
                    else
                    {
                        DateTime dt = DateTime.Parse(txtsdtCompra.Text.ToString(), CultureInfo.InvariantCulture);
                        vParametros.Add("@dtCompra", dt.ToString());
                    }
                    vParametros.Add("@sNotaFiscal", txtsNotaFiscal.Text);
                    vParametros.Add("@nValorCompra", BD.Conversoes.Numerico(txtnValorCompra));
                    vParametros.Add("@sObservacao", txtsObservacao.Text);
                    vParametros.Add("@idCategoria", ddlidCategoria.SelectedValue);
                    vParametros.Add("@idEstadoConservacao", ddlidEstadoConservacao.SelectedValue);
                    vParametros.Add("@idParceiro", ddlidParceiro.SelectedValue);
                    vParametros.Add("@idLocal", ddlidLocal.SelectedValue);
                    vParametros.Add("@idResponsavel", ddlidResponsavel.SelectedValue);
                    vParametros.Add("@idStatus", ddlidStatus.SelectedValue);
                    vParametros.Add("@idGrupo", ddlidGrupo.SelectedValue);
                    vParametros.Add("@idProduto", hddidProduto.Value);

                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idPatrimonio = RETORNO.DATASET(dsSalvar, "idPatrimonio");

                        Pesquisar(idPatrimonio, false);
                        MensagemPagina.MostraMensagem_Sucesso(string.Format("{1} gravado com sucesso!  </br><a href='{0}?id=0'>Clique aqui para incluir um novo {1}.</a>", Request.RawUrl.ToString(), sTituloPagina));
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

        #region | Carregar Imagem Patrimônio

        protected void CarregaimgPatrimonio(string idObjeto)
        {
            DataTable dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_IMAGEM");
            vParametros.Add("@idTipoArquivo", "120");
            vParametros.Add("@idObjeto", idObjeto);
            dsPesquisa = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dsPesquisa.Rows.Count > 0)
            {
                DataRow imgBd = dsPesquisa.Rows[0];
                byte[] valorImgBd = (byte[])imgBd["vbArquivo"];
                string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);
                imgPatrimonio.ImageUrl = imgUrl;
                imgPatrimonio.Visible = true;
                DIV_IMG.Visible = true;
            }
            else
            {
                DIV_IMG.Visible = false;
            }

        }

        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidPatrimonio.Value = "0";
            txtidPatrimonio.Text = "Novo";

            txtsDscPatrimonio.Text = "";

            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
            aba_Arquivo.Visible = false;
            aba_Movimentacao.Visible = false;

        }

        void LimparCamposDuplicando()
        {
            txtsEtiquetaPatrimonio.Text = "";
            //txtsDscPatrimonio.Text = txtsDscPatrimonio.Text + " - DUPLICADO";
            hddidPatrimonio.Value = "0";
        }

        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (!Validacoes.ValidarTexto(txtidPatrimonio))
            {
                sMensagemErro = "Informe um Código para o Patrimônio!";
            }

            if ((string.IsNullOrEmpty(ddlidResponsavel.SelectedValue) || Convert.ToInt32(ddlidResponsavel.SelectedValue) == 0) && string.IsNullOrEmpty(hddidResponsavel.Value))
            {
                sMensagemErro = "Selecione um Responsável Novamente";
            }

            if (string.IsNullOrEmpty(ddlidLocal.SelectedValue) || Convert.ToInt32(ddlidLocal.SelectedValue) == 0)
            {
                sMensagemErro = "Selecione um Local";
            }

            if (string.IsNullOrEmpty(ddlidCategoria.SelectedValue) || Convert.ToInt32(ddlidCategoria.SelectedValue) == 0)
            {
                sMensagemErro = "Selecione uma Categoria";
            }

            if (string.IsNullOrEmpty(ddlidEstadoConservacao.SelectedValue) || Convert.ToInt32(ddlidEstadoConservacao.SelectedValue) == 0)
            {
                sMensagemErro = "Selecione um Estado";
            }

            if (string.IsNullOrEmpty(ddlidStatus.SelectedValue) || Convert.ToInt32(ddlidStatus.SelectedValue) == 0)
            {
                sMensagemErro = "Selecione um Status";
            }

            if (string.IsNullOrEmpty(txtsDscPatrimonio.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Descrição de Patrimônio inválida!";
            }

            if (string.IsNullOrEmpty(txtsEtiquetaPatrimonio.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "A Etiqueta do Patrimônio é Obrigatório!";
            }


            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }


            return bRetorno;
        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidCategoria, "sp_Select 'Flow_Patrimonio_Categorias'", "idCategoria", "sDscCategoria", false, "Selecione a Categoria", "0");
            FUNCOES.Popula_Combo(ddlidLocal, "sp_Select 'Flow_Patrimonio_Local'", "idLocal", "sDscLocal", false, "Selecione o Local ", "0");
            FUNCOES.Popula_Combo(ddlidStatus, "sp_Select 'Flow_Patrimonio_Status'", "idStatusPatrimonio", "sDscStatusPatrimonio", false, "Selecione o Status ", "0");
            FUNCOES.Popula_Combo(ddlidEstadoConservacao, "sp_Select 'Flow_Patrimonio_EstadoConservacao'", "idEstadoConservacao", "sDscEstadoConservacao", false, "Selecione o Estado de Conservação", "0");
            FUNCOES.Popula_Combo(ddlidParceiro, "sp_Select 'Flow_Parceiros_Fornecedores'", "idCliente", "Razao_CNPJ", false, "Selecione o Fornecedor", "0");
            FUNCOES.Popula_Combo(ddlidGrupo, "sp_Select 'Flow_Patrimonio_Grupo'", "idPatrimonioGrupo", "sDscPatrimonio", false, "Selecione o Grupo", "0");
        }

        #endregion

        #region | Funções do Campo Patrimonio/Produto

        protected void txtsCodigoProduto_TextChanged(object sender, EventArgs e)
        {
            SqlDataReader sdr = BD.ExecutarDataReader("sp_Select 'FLOW_Produtos_Codigo', 0, 'S', '" + txtsDscPatrimonio.Text + "'");

            while (sdr.Read())
            {
                hddidProduto.Value = sdr["idItem"].ToString();
                txtsDscPatrimonio.Text = sdr["sDscProduto"].ToString();
            }

            RegistraScript("");
        }
        #endregion

        #region | Campo Produto Função Global    
        [System.Web.Services.WebMethod]
        public static string[] GetProdutos(string sDscProduto)//Atualmente esse está sendo usado
        {
            List<string> lstProdutos = new List<string>();
            if (sDscProduto.Length > 3)
            {
                SqlDataReader sdr = BD.ExecutarDataReader("sp_Select 'FLOW_Produtos', 0, 'S', '" + sDscProduto + "'");
                while (sdr.Read())
                {
                    lstProdutos.Add(string.Format("{2}|{0}|{1}|{3}", sdr["idItem"], sdr["sCodigo"], sdr["sDscProduto"], sdr["sUnidade"]));
                }
            }
            return lstProdutos.ToArray();
        }
        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$v192(function() {");
            sb.Append("$v192(\"[id$=txtsDscPatrimonio]\").autocomplete({");
            sb.Append("source: function(request, response) {");
            sb.Append("$v192.ajax({");

            sb.Append("url: '/app/Paginas/Adm/Patrimonio/Patrimonio_Detalhe.aspx/GetProdutos',");
            sb.Append("data: \"{ 'sDscProduto': '\" + request.term + \"'}\",");
            sb.Append("dataType: \"json\",");

            sb.Append("type: \"POST\",");
            sb.Append("contentType: \"application/json; charset=utf-8\",");

            sb.Append("success: function(data) {");
            sb.Append("response($v192.map(data.d, function(item) {");
            sb.Append("return {");

            sb.Append("label: item.split('|')[0],");
            sb.Append("val: item.split('|')[2],");
            sb.Append("id: item.split('|')[1],");
            sb.Append("un: item.split('|')[3]");
            sb.Append("}");
            sb.Append("}))");
            sb.Append("},");
            sb.Append("error: function(response) {");
            sb.Append("alert(response.responseText);");
            sb.Append("},");

            sb.Append("failure: function(response) {");

            sb.Append("alert(response.responseText);");
            sb.Append("}");
            sb.Append("});");
            sb.Append("},");

            sb.Append("select: function(e, i) {");
            sb.Append("$(\"[id$=txtsCodigoProduto]\").val(i.item.val);");
            sb.Append("$(\"[id$=hddidProduto]\").val(i.item.id);");

            sb.Append("},");


            sb.Append("minLength: 3");
            sb.Append("});});");


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

            sb.Append("$('[id*=txtnValorCompra]').mask('000.000.000.000.000,00', { reverse: true });");

            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }


        #endregion

        #region | EVENTOS E POPCOMBOS ESPECIAIS
        protected void ddlidLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(ddlidLocal.SelectedValue) != 0)
            {
                Popular_Combo_idResponsavel(ddlidLocal.SelectedValue);
            }

        }
        protected void cmdAvancar_click(object sender, EventArgs e)
        {
            int id = 0;
            if (txtidPatrimonio.Text != "Novo")
                 id = Convert.ToInt32(txtidPatrimonio.Text) + 1;         

            Response.Redirect($"Patrimonio_Detalhe.aspx?id={id}");
        }

        protected void cmdRetornar_click(object sender, EventArgs e)
        {           
            int id = 0;
           
            if (txtidPatrimonio.Text != "Novo")
                id = Convert.ToInt32(txtidPatrimonio.Text) - 1;

            Response.Redirect($"Patrimonio_Detalhe.aspx?id={id}");
        }

        void Popular_Combo_idResponsavel(string idLocal)
        {
            FUNCOES.Popula_Combo(ddlidResponsavel, "sp_Select 'Flow_Usuarios_Patrimonio'", Convert.ToInt32(idLocal), "idUsuario", "sDscUsuario", false, "Selecione o Responsável", "0");

        }

        void Popular_aba_Arquivo(string idPatrimonio)
        {
            frmArquivos.Attributes.Add("src", string.Format("~/app/Paginas/Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idPatrimonio, "Patrimonio"));
            aba_Arquivo.Visible = true;
        }

        #endregion

    }
}