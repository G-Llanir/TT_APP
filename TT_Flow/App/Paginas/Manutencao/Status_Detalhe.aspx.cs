using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.IO;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Status_Detalhe : Page
    {
        string sTituloPagina = "Status";
        string sProcedure = "sp_Manipula_tbl_Flow_Status";

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            Page.Form.Attributes.Add("enctype", "multipart/form-data");

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlsCor, "sp_Select 'COR'", "sCor", "sDscCor", false, "Selecione a Cor", "");
                FUNCOES.Popula_Combo(ddlidTipoArquivo, "sp_Select 'FLOW_Arquivos_Tipo', @sPesquisa='Kanban'", "idTipoArquivo", "sDscTipoArquivo", false, "Selecione o Tipo do Arquivo", "0");


                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Status.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Status.Incluir, true);
                    Pesquisar("0");
                }

                ddlsTipo_SelectedIndexChanged(objSender, objEventArgs);

            }
        }

        protected void Pesquisar(string idPesquisa)
        {
            div_EnviarArquivos.Visible = false;
            div_ddlAlteraEndereco.Visible = false;

            try
            {
                LimpaCampos();

                if (idPesquisa != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idStatus", idPesquisa }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidStatus.Value = RETORNO.DATASET(dsPesquisa, 0, "idStatus");
                        txtidStatus.Text = RETORNO.DATASET(dsPesquisa, 0, "idStatus");
                        txtsDscStatus.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscStatus");
                        ddlsTipo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sTipo");

                        if (ddlsTipo.SelectedValue == "Pedido")
                            div_ddlAlteraEndereco.Visible = true;

                        ddlAlteraEndereco.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sAlteraEndereco_Pedido");

                        ddlsExibeCliente.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sExibeCliente");
                        ddlsPedidoFinalizado.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sPedidoFinalizado");
                        ddlsLiberadoFaturamento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sLiberadoFaturamento");
                        ddlLogistica.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sLogistica");

                        //Agnes Partal - 27/06/2024
                        ddlsExibeKanban.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sExibeKanban");
                        ddlCancelado.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sStatusCancelamento");

                        ddlsCor.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sCor");
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscStatus.Text);
                        PesquisarArquivo(RETORNO.DATASET(dsPesquisa, 0, "idStatus"));
                        div_EnviarArquivos.Visible = true;

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Status.Alterar);
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                {
                    BreadCrumb.TitulodaPagina = "Incluir";
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    cmdSalvar.Text = "Incluir";
                }

                txtsDscStatus.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        #endregion

        #region | Eventos

        /*
         ● O campo "cmdEnviarArquivos_Click" é um evento desencadeado quando o "Button ID="cmdEnviarArquivos" eh apertado.

         ● Ao iniciar ele cai no "if" que chama o metodo "bool ValidarEnvioArquivo()" logo em seguida ocorre o erro, por algum motivo ele não esta reconhecendo 
           que existe um arquivo esse erro esta ocorrendo no if:  "(!fu_Arquivo.HasFile)"

         ● Apos validar os arquivos ele volta para o if abaixo?
         
         ● if (BD.ValidarDataSet(dsItem, out sErro))
           
         */
        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {
            if (ValidarEnvioArquivo())

            {
                string sErro = "";
                Byte[] lObjArquivo = null;
                Stream lObjConteudoArquivo = fu_Arquivo.PostedFile.InputStream;
                string sNomeArquivo = fu_Arquivo.FileName;
                string lStrCaminhoArquivo = fu_Arquivo.PostedFile.FileName;
                string lStrNomeArquivo = Path.GetFileName(lStrCaminhoArquivo);
                string lStrExtencaoArquivo = Path.GetExtension(lStrNomeArquivo);

                try
                {
                    TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                    lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(lStrNomeArquivo, lStrCaminhoArquivo, lObjConteudoArquivo);
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                }

                FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                Arquivo.idTipoArquivo = Convert.ToInt32(ddlidTipoArquivo.SelectedValue);
                Arquivo.idObjeto = Convert.ToInt32(hddidStatus.Value);
                Arquivo.sNomeArquivo = sNomeArquivo;

                // ● esse comando abaixo le oq foi selecionado e preenche o o campo sDscArquivo do SQL Server
                Arquivo.sDscArquivo = ddlidTipoArquivo.SelectedItem.ToString(); // ou ddlidTipoArquivo.Text; ->Não precisa de ToString pq Text ja é uma string
                // Arquivo.idTipoArquivo = 98;
                Arquivo.sObservacao = "";
                Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                Arquivo.vbArquivo = lObjArquivo;

                DataSet dsItem = Arquivo.EnviarArquivo(Arquivo);
                if (BD.ValidarDataSet(dsItem, out sErro))
                {
                    PesquisarArquivo(hddidStatus.Value);
                    MensagemPagina.MostraMensagem_Sucesso("Arquivo enviado com sucesso!");
                    Pesquisar(hddidStatus.Value);
                }
            }
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                try
                {
                    string[] vidStatus = hddidStatus.Value.Split(',');
                    string idStatus = vidStatus[0].ToString();

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idStatus", idStatus },
                        { "@sDscStatus", txtsDscStatus.Text },
                        { "@sExibeCliente", ddlsExibeCliente.SelectedValue },
                        { "@sExibeKanban", ddlsExibeKanban.SelectedValue },
                        { "@sStatusCancelamento", ddlCancelado.SelectedValue },
                        { "@sLiberadoFaturamento", ddlsLiberadoFaturamento.SelectedValue },
                        { "@sLogistica", ddlLogistica.SelectedValue },
                        { "@sPedidoFinalizado", ddlsPedidoFinalizado.SelectedValue },
                        { "@sCor", ddlsCor.SelectedValue },
                        { "@sSituacao", ComboAtivo.Situacao_Recuperar() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                        { "@sTipo", ddlsTipo.SelectedValue },
                        { "@sAlteraEndereco_Pedido", ddlAlteraEndereco.SelectedValue }
                    };
                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        idStatus = RETORNO.DATASET(dsSalvar, 0, "idStatus");
                        Pesquisar(idStatus);
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                    }
                    else
                        throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }
        }

        protected void ddlsTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ddlsTipo.SelectedValue)
            {
                case "":
                    lblPedidoFinalizado.Text = "Pedido Finalizado";
                    divExibeCliente.Visible = false;
                    divLiberadoFaturamento.Visible = false;
                    divExibeKanban.Visible = false;
                    divCancelado.Visible = false;
                    break;

                case "CRM":
                    lblPedidoFinalizado.Text = "Processo Finalizado";
                    divExibeKanban.Visible = true;
                    divCancelado.Visible = true;
                    divExibeCliente.Visible = false;
                    divLiberadoFaturamento.Visible = false;
                    break;
                case "Comex":
                    lblPedidoFinalizado.Text = "Processo Finalizado";
                    divExibeCliente.Visible = false;
                    divLiberadoFaturamento.Visible = false;
                    break;
                case "Pedido":
                    lblPedidoFinalizado.Text = "Pedido Finalizado";
                    divExibeCliente.Visible = true;
                    divLiberadoFaturamento.Visible = true;
                    div_ddlAlteraEndereco.Visible = true;
                    break;

            }
        }

        #endregion

        #region | Arquivos

        /* O campo "PesquisarArquivo" inicia quando entramos no detalhe de qualque satatus (page load?).
         
        RETORNA:

        ● protected void PesquisarArquivo(string idStatus, string idCliente, string sMetodoChamada) 
          - idStatus = depende do item selecionado na pagina
          - idCliente = "" 
          - sMetodoChamada = "Inicializar"

        ● sERRO = "" (vazio?)
        ● dsArquivos = null
        ● Dictionary =  Count = 3 (Conta os "vParametros"?) 

        ● if (BD.ValidarDataSet(dsArquivos, out sErro)) = false

        ○ Apos passar por todas a linhas o programa vai para linha 33 em: PesquisarArquivo(Request["id"].ToString(), "", "Inicializar");]
        

        Conclusão:
        
        Esta linha faz a pesquisa no SLQ usando a função "CONSULTAR" para trazer os dados?
         */
        protected void PesquisarArquivo(string idStatus)
        {
            hddidStatus.Value = idStatus;
            ddlidTipoArquivo.SelectedIndex = 0;
            //ddlidTipoIcone.SelectedValue = "0";

            string sErro = "";
            DataSet dsArquivos;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "Consultar");
            //vParametros.Add("@idTipoArquivo", "1");

            //vParametros.Add("@idTipoArquivo", ddlidTipoArquivo.SelectedValue);


            //vParametros.Add("@idTipoIcone", "1");
            vParametros.Add("@sTipoObjeto", "Kanban");
            vParametros.Add("@idObjeto", idStatus);
            dsArquivos = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (BD.ValidarDataSet(dsArquivos, out sErro))
            {
                gv_Arquivo.DataSource = dsArquivos.Tables[0];
                gv_Arquivo.DataBind();
            }
        }

        /*
        ● No if (!fu_Arquivo.HasFile) quando a exclamação é retirada o erro que gera a menssagem
        "Selecione um arquivo para enviar!" para de ocorrer.
        Porem ocorre um erro no "protected void cmdEnviarArquivos_Click" 
        dando a menssagem "'Referência de objeto não definida para uma instância de um objeto.'" 
        na linha "Stream lObjConteudoArquivo = fu_Arquivo.PostedFile.InputStream;"

        ● O metodo "bool ValidarEnvioArquivo" faz a validação dos arquivos, no caso do upload de arquivos é usado o
        if  "(fu_Arquivo.HasFile)" (fu = file upload?) e (HasFile é uma propriedade que verifica se existe um arquivo)
        
         */
        bool ValidarEnvioArquivo()
        {
            bool bRetorno = true;

            if (ddlidTipoArquivo.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione o Tpo de Arquivo!");
                ddlidTipoArquivo.Focus();
                return false;
            }

            //if (ddlidTipoIcone.SelectedValue == "0")
            //{
            //    MensagemPagina.MostraMensagem_Erro("Selecione o Tpo de Arquivo!");
            //    ddlidTipoIcone.Focus();
            //    return false;
            //}



            if (!fu_Arquivo.HasFile)
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um arquivo para enviar!");
                fu_Arquivo.Focus();
                return false;
            }
            //if (txtEnviarArquivo_sDscArquivo.Text.Length < 10)
            //{
            //    MensagemPagina.MostraMensagem_Erro("Informe uma Descrição do Arquivo válida!");
            //    txtEnviarArquivo_sDscArquivo.Focus();
            //    return false;
            //}

            return bRetorno;

        }

        protected void gv_Arquivo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView dr = (DataRowView)e.Row.DataItem;
                string imageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])dr["vbArquivo"]);
                (e.Row.FindControl("Image1") as Image).ImageUrl = imageUrl;
            }
            GRID.EsconderColunas(e, 0);
        }

        protected void gv_Arquivo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = int.Parse(e.CommandArgument.ToString());
            var gr = gv_Arquivo.Rows[index];
            string idArquivo = gr.Cells[0].Text;

            switch (e.CommandName)
            {
                case "Download":
                    Dictionary<string, string> vParametrosItem = new Dictionary<string, string>();
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "CONSULTAR_DETALHE" },
                        {"@idArquivo",                  idArquivo}
                    };
                    DataTable dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

                    foreach (DataRow item in dtArquivo.Rows)
                    {
                        string sNomeArquivo = item["sNomeArquivo"].ToString();
                        TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                        FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo((byte[])item["vbArquivo"], Server.MapPath("~/Download/" + sNomeArquivo));
                        lObjFile.Close();

                        Response.ContentType = "application/octet-stream";
                        Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", sNomeArquivo));
                        Response.TransmitFile(Server.MapPath("~/Download/" + sNomeArquivo));
                        Response.End();
                    }

                    break;
            }
        }

        #endregion

        #region | Utils

        void LimpaCampos()
        {
            txtidStatus.Text = "Novo";
            txtsDscStatus.Text = "";
            ddlsExibeCliente.SelectedValue = "N";
            ddlsPedidoFinalizado.SelectedValue = "N";
            ddlsLiberadoFaturamento.SelectedValue = "N";
            ddlLogistica.SelectedValue = "N";
            ddlsCor.SelectedValue = "";
            hddidStatus.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
            ddlsTipo.SelectedValue = "";
            ddlsExibeKanban.SelectedValue = "N";

        }

        private bool ValidarDados()
        {
            if (txtsDscStatus.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um Descrição válida para a Status!");
                return false;
            }

            return true;
        }

        #endregion
    }
}