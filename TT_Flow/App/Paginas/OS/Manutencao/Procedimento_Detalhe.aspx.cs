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
using GRID = TT.FrameWork.Grid;
using VALIDACOES = TT.FrameWork.Validacoes;
using System.IO;


namespace TT_Flow.App.Paginas.OS.Manutencao
{
    public partial class Procedimento_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Procedimento";
        string sProcedure = "sp_Manipula_tbl_Flow_Procedimentos";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();
                                
            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos_OS'", "idDepartamento", "sDscDepartamento", false, "Selecione um Departamento", "0");
                FUNCOES.Popula_Combo(ddlTipoProcedimento, "sp_Select 'Flow_Procedimentos_Tipo'", "idTipoProcedimento", "sDscTipoProcedimento", false, "Selecione um tipo de Procedimento", "0");
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.OrdemServico.Procedimentos.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.OrdemServico.Procedimentos.Incluir, true);
                    Pesquisar("0");
                }
            }
        }


        //protected void PesquisarArquivo(string idProcedimento)
        //{
        //    hddidProcedimento.Value = idProcedimento;
        //    string sErro = "";
        //    DataSet dsArquivos;
        //    Dictionary<String, String> vParametros = new Dictionary<string, string>();
        //    vParametros.Add("@sFuncao", "Consultar");
        //    vParametros.Add("@sTipoObjeto", "Kanban");
        //    vParametros.Add("@idObjeto", idProcedimento);
        //    dsArquivos = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametros);

        //    if (BD.ValidarDataSet(dsArquivos, out sErro))
        //    {
        //        gv_Arquivo.DataSource = dsArquivos.Tables[0];
        //        gv_Arquivo.DataBind();
        //    }
        //}





        protected void Pesquisar(string idPesquisa)
        {
            string sErro = "";
            //div_EnviarArquivos.Visible = false;
            try
            {
                LimpaCampos();
                div_EnviarArquivos.Visible = true;

                if (idPesquisa != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idProcedimento", idPesquisa);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidProcedimento.Value             = RETORNO.DATASET(dsPesquisa, 0, "idProcedimento");
                        txtidProcedimento.Text              = RETORNO.DATASET(dsPesquisa, 0, "idProcedimento");
                        ddlTipoProcedimento.SelectedValue   = RETORNO.DATASET(dsPesquisa, 0, "idTipoProcedimento");
                        txtsDscProcedimento.Text            = RETORNO.DATASET(dsPesquisa, 0, "sDscProcedimento");
                        sObservacaoProcedimento.Content     = RETORNO.DATASET(dsPesquisa, 0, "sObservacaoProcedimento");
                        ddlDepartamento.SelectedValue       = RETORNO.DATASET(dsPesquisa, 0, "idDepartamento");

                        if (RETORNO.DATASET(dsPesquisa, 0, "nTempo") != "0")
                        {
                            txtnTempo.Text = RETORNO.DATASET(dsPesquisa, 0, "nTempo");
                        }


                        ddlTipoTempo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sTipoTempo");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscProcedimento.Text);
                        FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                        gv_Arquivo.DataSource = Arquivo.ConsultarArquivos(Convert.ToInt32(RETORNO.DATASET(dsPesquisa, "idProcedimento")), "Procedim");
                        gv_Arquivo.DataBind();

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.OrdemServico.Procedimentos.Alterar);;


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
                    ddlDepartamento.Focus();
                }


            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }



        void LimpaCampos()
        {
            txtidProcedimento.Text = "Nova";
            ddlDepartamento.Attributes.Add("enabled", "enabled");
            ddlDepartamento.SelectedValue = "0";
            ddlTipoProcedimento.Attributes.Add("enabled", "enabled");
            ddlTipoProcedimento.SelectedValue = "0";
            txtsDscProcedimento.Text = "";
            hddidProcedimento.Value = "0";
            txtnTempo.Text = "";
            ddlTipoTempo.SelectedValue = "";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
            txtEnviarArquivo_sDscArquivo.Text = "";
        }


        private bool ValidarDados()
        {
            if (ddlDepartamento.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um Departamento");
                ddlDepartamento.Focus();
                return false;
            }
            
            if (ddlTipoProcedimento.SelectedValue == "0")
            {
               MensagemPagina.MostraMensagem_Erro("Informe um tipo de Procedimento!");
               ddlTipoProcedimento.Focus();
               return false;
            }

            if (txtsDscProcedimento.Text.Length < 12)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um nome válido para o Procedimento!");
                txtsDscProcedimento.Focus();
                return false;
            }
          
            if (txtnTempo.Text != "")
            {
                if (!VALIDACOES.ValidarNumerico(txtnTempo))
                {
                    MensagemPagina.MostraMensagem_Erro("Informe um valor válido");
                    txtnTempo.Focus();
                    return false;
                }
                else
                {
                    if (ddlTipoTempo.SelectedValue == "")
                    {
                        MensagemPagina.MostraMensagem_Erro("Escolha um formato de tempo");
                        ddlTipoTempo.Focus();
                        return false;
                    }
                }


            }

            return true;
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (ValidarDados())
            {

                try
                {
                    string[] vidProcedimento = hddidProcedimento.Value.Split(',');
                    string idProcedimento = vidProcedimento[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idProcedimento", idProcedimento);
                    vParametros.Add("@idDepartamento", ddlDepartamento.SelectedValue);
                    vParametros.Add("@idTipoProcedimento", ddlTipoProcedimento.SelectedValue);
                    vParametros.Add("@sDscProcedimento", txtsDscProcedimento.Text);
                    vParametros.Add("@sObservacaoProcedimento", sObservacaoProcedimento.Content);
                    vParametros.Add("@nTempo", txtnTempo.Text);
                    vParametros.Add("@sTipoTempo", ddlTipoTempo.SelectedValue);
                    vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        if (idProcedimento == "0")
                        {

                        }
                        idProcedimento = RETORNO.DATASET(dsSalvar, 0, "idProcedimento");
                        Pesquisar(idProcedimento);
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!  </br><a href='Procedimento_Detalhe.aspx?id=0'>Clique aqui para incluir uma novo procedimento.</a>");
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

                TT_Flow.FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                Arquivo.idTipoArquivo = 101;
                Arquivo.idObjeto = Convert.ToInt32(hddidProcedimento.Value);
                Arquivo.sNomeArquivo = sNomeArquivo;
                Arquivo.sDscArquivo = txtEnviarArquivo_sDscArquivo.Text;
                Arquivo.sObservacao = "";
                Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                Arquivo.vbArquivo = lObjArquivo;
                
                DataSet dsItem = Arquivo.EnviarArquivo(Arquivo);
                if (BD.ValidarDataSet(dsItem, out sErro))
                {
                    Pesquisar(hddidProcedimento.Value);
                    MensagemPagina.MostraMensagem_Sucesso("Arquivo enviado com sucesso!");
                    
                }
            }
        }


        bool ValidarEnvioArquivo()
        {
            bool bRetorno = true;

            if (!fu_Arquivo.HasFile)
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um arquivo para enviar!");
                fu_Arquivo.Focus();
                return false;
            }
            if (txtEnviarArquivo_sDscArquivo.Text.Length < 5)
            {
                MensagemArquivo.MostraMensagem_Erro("Informe uma Descrição do Arquivo válida!");
                txtEnviarArquivo_sDscArquivo.Focus();
                return false;
            }

            return bRetorno;

        }


        protected void gv_Arquivo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument.ToString() != "")
            { 
                int index = int.Parse(e.CommandArgument.ToString());
                var gr = gv_Arquivo.Rows[index];
                string idArquivo = gr.Cells[0].Text; //Codigo do arquivo Chamado
                        

                switch (e.CommandName)
                {
                    case "Download":

                    Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                    DataTable dtArquivo;
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "CONSULTAR_DETALHE" },
                        {"@idArquivo",                  idArquivo}
                    };

                    dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

                    foreach (DataRow item in dtArquivo.Rows)
                    {

                        FileStream lObjFile;
                        string sNomeArquivo = item["sNomeArquivo"].ToString();
                        TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                        lObjFile = objArquivo.TransformarArrayBytesEmArquivo((byte[])item["vbArquivo"], Server.MapPath("~/Download/" + sNomeArquivo));
                        lObjFile.Close();

                        Response.ContentType = "application/octet-stream";
                        Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", sNomeArquivo));
                        Response.TransmitFile(Server.MapPath("~/Download/" + sNomeArquivo));
                        Response.End();
                    }

                    break;

                }

            }
        }

        protected void gv_Arquivo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        bool Documento_Deletar(string idArquivo)
        {
            bool bRetorno = false;
            try
            {

                DataSet dsDocumento_Excluir;
                Dictionary<String, String> vParametroContato_Excluir = new Dictionary<string, string>();
                vParametroContato_Excluir.Add("@sFuncao", "DOCUMENTO_DELETAR");
                vParametroContato_Excluir.Add("@idArquivo", idArquivo);
                dsDocumento_Excluir = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametroContato_Excluir);


                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            return bRetorno;
        }

        
        protected void gv_Arquivo_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string idArquivo = "";
            idArquivo = gv_Arquivo.Rows[e.RowIndex].Cells[0].Text.ToString();
            Documento_Deletar(idArquivo);
            Pesquisar(hddidProcedimento.Value);


        }
    }

    
}