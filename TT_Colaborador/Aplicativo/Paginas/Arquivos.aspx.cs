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
using System.Text;
using System.Web.Services;
using System.Drawing;

namespace TT_Colaborador.Aplicativo.Paginas
{
    public partial class Arquivos : Page
    {
        #region | Construtores

        public static string imagemPadraoBase64 = "";
        public static string imagemID = "";
        public static string FileName = "";
        public static string Extension = "";
        public static byte[] FileBytes = null;

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            string sTipoObjeto = "Pedido";

            if (!IsPostBack)
            {
                if (Request["idObjeto"] != null)
                {
                    if (Request["sTipoObjeto"] != null)
                    {
                        sTipoObjeto = Request["sTipoObjeto"].ToString();
                        if (sTipoObjeto == "Produtos")
                        {
                            foreach (DataControlField coluna in gv_Arquivo.Columns)
                            {
                                if (coluna.HeaderText.Equals("Deletar"))
                                {
                                    coluna.Visible = true;
                                    break;
                                }
                            }

                            img_Produto.Visible = true;
                        }
                        else
                        {
                            foreach (DataControlField coluna in gv_Arquivo.Columns)
                            {
                                if (coluna.HeaderText.Equals("Deletar"))
                                {
                                    coluna.Visible = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (Request["sPermiteNovoArquivo"] != null)
                    {
                        if (Request["sPermiteNovoArquivo"] == "S")
                            div_EnviarArquivos.Visible = true;
                        else
                            div_EnviarArquivos.Visible = false;
                    }

                    FUNCOES.Popula_Combo(ddlidTipoArquivo, "sp_Select 'FLOW_Arquivos_Tipo', @idPesquisa=" + IDENTITY.Variaveis.idUsuario() + ", @sPesquisa='" + sTipoObjeto + "'", "idTipoArquivo", "sDscTipoArquivo", false, "Selecione o Tipo do Arquivo", "0");

                    PesquisarArquivo(Request["idObjeto"].ToString(), "", sTipoObjeto);
                }
                else
                    PesquisarArquivo("0", "", sTipoObjeto);
            }

            var requestTarget = this.Request["__EVENTTARGET"];
            if (requestTarget == "dialog_Apagar")
                ExcluirArquivo();

            RegistraScript();
        }

        protected void PesquisarArquivo(string idObjeto, string idCliente, string sTipoObjeto)
        {
            hddidObjeto.Value = idObjeto;
            hddsTipoObjeto.Value = sTipoObjeto;

            txtEnviarArquivo_sDscArquivo.Text = "";

            //Thiago Rodrigues - 14 / 06 / 2024
            if (!string.IsNullOrEmpty(Request["sExibicao"]) && Convert.ToBoolean(Request["sExibicao"]))
            {
                div_EnviarArquivos.Visible = false;
                gv_Arquivo.Columns[6].Visible = false;
            }
            else
                div_EnviarArquivos.Visible = true;

            if (ddlidTipoArquivo.Items.Count == 2)
                ddlidTipoArquivo.SelectedIndex = 1;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Consultar" },
                { "@sTipoObjeto", sTipoObjeto },
                { "@idObjeto", idObjeto },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
            };

            if (Convert.ToBoolean(Request["SubTipo"]))
                vParametros.Add("@idOrdem", "");
            if (hddsTipoObjeto.Value == "EmpresaSTSO" || hddsTipoObjeto.Value == "ColaboradorSTSO")
                vParametros.Add("@sObservacao", "");

            DataSet dsArquivos = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (BD.ValidarDataSet(dsArquivos, out string sErro))
            {
                gv_Arquivo.DataSource = dsArquivos.Tables[0];
                gv_Arquivo.DataBind();
                //btDownload.Visible = true;
                //if (hddsTipoObjeto.Value != "PedidoART")
                //    div_Ordem.Visible = false;
            }
            else
            {
                gv_Arquivo.DataSource = null;
                gv_Arquivo.DataBind();
                //btDownload.Visible = false;
                //div_Ordem.Visible = false;
            }

            if (Convert.ToBoolean(Request["SubTipo"]))
                AtualizarGrid();

            ConsultarImagem(idObjeto, sTipoObjeto);
            RegistraScript();

        }
        void AtualizarGrid()
        {
            GridViewHelper helper = new GridViewHelper(gv_Arquivo);
            helper.GroupHeader += new GroupEvent(helper_GroupHeader);
            helper.RegisterGroup("sDscCategoria", true, true);
            helper.ApplyGroupSort();
        }

        private void helper_GroupHeader(string groupName, object[] values, GridViewRow row)
        {
            row.BackColor = Color.FromArgb(173, 216, 230);
            row.Cells[0].Font.Bold = true;
            row.Cells[0].ForeColor = Color.Black;
            row.Cells[0].Text = row.Cells[0].Text.ToUpper();
            row.Cells[0].HorizontalAlign = HorizontalAlign.Left;
        }
        #endregion

        #region | Imagens

        [WebMethod]
        public static string SaveNewImageSrc(string imgId, string newSrc, string filename, string extension)
        {
            string base64String = "";
            int startIndex = newSrc.IndexOf("base64,") + "base64".Length;

            if (startIndex >= 0)
                base64String = newSrc.Substring(startIndex + 1);

            FileName = filename;
            Extension = extension;
            FileBytes = Convert.FromBase64String(base64String);

            return "Arquivo salvo com sucesso.";
        }

        protected void SetImageData(byte[] inArray)
        {
            string strBase64Logo = Convert.ToBase64String(inArray);

            if (!string.IsNullOrWhiteSpace(strBase64Logo))
                img_Produto.ImageUrl = "data:Image/png;base64," + strBase64Logo;
        }

        protected void ConsultarImagem(string idObjeto, string sTipoObjeto)
        {
            try
            {
                if (idObjeto != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_IMAGEM" },
                        { "@idTipoArquivo", "201" },
                        { "@idObjeto", idObjeto }
                    };
                    DataTable dsPesquisa = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);
                    DataRow imgBd = dsPesquisa.Rows[0];

                    byte[] valorImgBd = (byte[])imgBd["vbArquivo"];

                    string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);

                    img_Produto.ImageUrl = imgUrl;
                    img_Produto.ImageUrl = imgUrl;
                    img_Produto.Style["display"] = "block";
                }
            }
            catch { }
        }

        #endregion

        bool ValidarEnvioArquivo()
        {
            bool bRetorno = true;

            if (ddlidTipoArquivo.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione o Tipo de Arquivo!");
                ddlidTipoArquivo.Focus();
                return false;
            }

            if (!fu_Arquivo.HasFile && FileName == "")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um arquivo para enviar!");
                fu_Arquivo.Focus();
                return false;
            }

            if (txtEnviarArquivo_sDscArquivo.Text.Length < 10)
            {
                MensagemPagina.MostraMensagem_Erro("Informe uma Descrição do Arquivo válida!");
                txtEnviarArquivo_sDscArquivo.Focus();
                return false;
            }

            return bRetorno;
        }

        void RegistraScript()
        {
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "carregarIMG", ResolveUrl("~/App/JS/UploadImagem.js"));

            StringBuilder sb = new StringBuilder();

            sb.Append("$(function() {");
            sb.Append("$(\"#dialog_Apagar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");

            sb.Append("\"Sim\": function() {");
            sb.Append("$('#loadingOverlay').css('display', 'flex');"); // <--- ADICIONE ISSO
            sb.Append("__doPostBack(\"dialog_Apagar\", \"\");");
            sb.Append("$(this).dialog(\"close\");");
            sb.Append("},");

            sb.Append("\"Não\": function() {");
            sb.Append("$(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$('.ExcluirArquivo').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$(hddExcluirArquivo).val($(this).closest('tr').attr('id'));");
            sb.Append("$('#dialog_Apagar').dialog('open');");
            sb.Append("});");
            sb.Append("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "JS_ExcluirAquivo", sb.ToString(), true);

            //sb.Append("\"Sim\": function() {");
            //sb.Append("__doPostBack(\"dialog_Apagar\", \"\");");
            //sb.Append("$(this).dialog(\"close\");");
            //sb.Append("},");
        }

        bool Documento_Deletar(string idArquivo)
        {
            bool bRetorno = false;

            try
            {
                Dictionary<string, string> vParametroArquivo_Excluir = new Dictionary<string, string>
                {
                    { "@sFuncao", "DOCUMENTO_DELETAR" },
                    { "@idArquivo", idArquivo },
                    { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
                };
                DataSet dsDocumento_Excluir = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametroArquivo_Excluir);

                if (!string.IsNullOrEmpty(Request["idObjeto"]) && !string.IsNullOrEmpty(Request["idUsuario"]))
                    RegistraHistoricoSolicitacao("Deletou um Arquivo", Request["idObjeto"], Request["statusAtual"], Request["idUsuario"]);

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            return bRetorno;
        }

        #region | Eventos

        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {
            if (ValidarEnvioArquivo())
            {
                Byte[] lObjArquivo = null;
                Stream lObjConteudoArquivo;
                string sNomeArquivo;
                string lStrCaminhoArquivo;
                string lStrNomeArquivo;
                string lStrExtencaoArquivo;


                if (FileName != "")
                {
                    lObjArquivo = null;
                    lObjConteudoArquivo = new MemoryStream(FileBytes);
                    sNomeArquivo = FileName;
                    lStrCaminhoArquivo = "";
                    lStrNomeArquivo = Path.GetFileName(lStrCaminhoArquivo);
                    lStrExtencaoArquivo = Path.GetExtension(lStrNomeArquivo);
                    FileName = "";
                }
                else
                {
                    lObjArquivo = null;
                    lObjConteudoArquivo = fu_Arquivo.PostedFile.InputStream;
                    sNomeArquivo = fu_Arquivo.FileName;
                    lStrCaminhoArquivo = fu_Arquivo.PostedFile.FileName;
                    lStrNomeArquivo = Path.GetFileName(lStrCaminhoArquivo);
                    lStrExtencaoArquivo = Path.GetExtension(lStrNomeArquivo);
                }

                try
                {
                    TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                    lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(lStrNomeArquivo, lStrCaminhoArquivo, lObjConteudoArquivo);
                }
                catch
                {
                    return;
                }

                FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                Arquivo.idTipoArquivo = Convert.ToInt32(ddlidTipoArquivo.SelectedValue);
                Arquivo.idObjeto = Convert.ToInt32(hddidObjeto.Value);
                Arquivo.sNomeArquivo = sNomeArquivo;
                Arquivo.sDscArquivo = txtEnviarArquivo_sDscArquivo.Text;
                Arquivo.sObservacao = "";
                Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                Arquivo.vbArquivo = lObjArquivo;

                DataSet dsItem = Arquivo.EnviarArquivo(Arquivo);

                if (BD.ValidarDataSet(dsItem, out string sErro))
                {
                    PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);

                    if (!string.IsNullOrEmpty(Request["idObjeto"]) && !string.IsNullOrEmpty(Request["idStatus"]) && !string.IsNullOrEmpty(Request["idUsuario"]))
                        RegistraHistoricoSolicitacao("Adicionou um Arquivo", Request["idObjeto"], Request["idStatus"], Request["idUsuario"]);
                    else
                        return;

                    string msg = "Arquivo enviado com sucesso!";

                    try
                    {
                        if (RETORNO.DATASET(dsItem, 0, "idTipoArquivo") == "600")
                        {
                            TT.FrameWork.Arquivo arquivo = new TT.FrameWork.Arquivo();

                            foreach (DataRow linha in dsItem.Tables[0].Rows)
                            {
                                FileStream manualChangeLog = arquivo.TransformarArrayBytesEmArquivo((byte[])linha["vbArquivo"], Server.MapPath("~/Manuais/" + RETORNO.DATASET(dsItem, 0, "sNomeArquivo")));

                                msg = "Manual enviado e adicionado à pasta de Manuais com sucesso!";
                            }
                        }
                    }
                    catch { }

                    MensagemPagina.MostraMensagem_Sucesso(msg);

                    if (hddsTipoObjeto.Value == "Solicitação")

                    {
                        string jsRefresh = @"
                        if (window.parent && typeof(window.parent.AtualizarPaginaPai) === 'function') {
                          window.parent.AtualizarPaginaPai(); 
                        }";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "RefreshParentSafe", jsRefresh, true);
                    }
                }

            }
        }

        bool RegistraHistoricoSolicitacao(string sMotivo, string idSolicitacao, string idStatus, string idUsuario)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "INSERE-LOG-ARQUIVO" },
                { "@idSolicitacao", idSolicitacao },
                { "@idStatus", idStatus },
                { "@idUsuarioSolicitacao", idUsuario },
                { "@sMotivo", sMotivo }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Solicitacoes", vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                return true;
            else
                return false;
        }

        protected void gv_Arquivo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument.ToString() != "")
            {
                if (e.CommandName == "Download")
                {
                    int index = int.Parse(e.CommandArgument.ToString());
                    var gr = gv_Arquivo.Rows[index];
                    string idArquivo = gr.Cells[0].Text;

                    Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "CONSULTAR_DETALHE" },
                        {"@idArquivo",                  idArquivo}
                    };
                    DataTable dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

                    foreach (DataRow item in dtArquivo.Rows)
                    {

                        FileStream lObjFile;
                        string sNomeArquivo = item["sNomeArquivo"].ToString().Replace(",", "");
                        TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                        lObjFile = objArquivo.TransformarArrayBytesEmArquivo((byte[])item["vbArquivo"], Server.MapPath("~/Download/" + sNomeArquivo));
                        lObjFile.Close();

                        Response.ContentType = "application/octet-stream";
                        Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", sNomeArquivo));
                        Response.TransmitFile(Server.MapPath("~/Download/" + sNomeArquivo));
                        Response.End();
                    }
                }
            }
        }

        protected void gv_Arquivo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            gv_Arquivo.RowDeleting += gv_Arquivo_RowDeleting;
            GRID.EsconderColunas(e, 0);

            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.Attributes.Add("id", gv_Arquivo.DataKeys[e.Row.RowIndex]["idArquivo"].ToString());
        }
        protected void gv_Arquivo_PreRender(object sender, EventArgs e)
        {
            // REMOVIDO O IF (gv_Arquivo.Rows.Count > 0)
            // O cabeçalho deve ser renderizado SEMPRE para o DataTables funcionar
            if (gv_Arquivo.HeaderRow != null)
            {
                gv_Arquivo.UseAccessibleHeader = true;
                gv_Arquivo.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }
        void ExcluirArquivo()
        {
            if (hddExcluirArquivo.Value != null)
            {
                Documento_Deletar(hddExcluirArquivo.Value);
                PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
            }
        }

        protected void gv_Arquivo_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string idArquivo = gv_Arquivo.Rows[e.RowIndex].Cells[0].Text.ToString();

            Documento_Deletar(idArquivo);
            PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
        }

        #endregion
    }
}