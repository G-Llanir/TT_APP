using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using TT.FrameWork;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Grid;
using static TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas
{
    public partial class Arquivos : Page
    {
        #region | Construtores

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
                if (Request.GetValue("idObjeto") != null)
                {
                    if (Request.GetValue("sTipoObjeto") != null)
                    {
                        sTipoObjeto = Request.GetValue("sTipoObjeto");
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
                            if (ValidaPermissao(Permissao.Pedidos.ExcluirArquivos))
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
                    }

                    if (Request.GetValue("sEmpresaxColaborador") != null)
                        hddsEmpresaxColaborador.Value = Request.GetValue("sEmpresaxColaborador").Replace("-", "");

                    if (Request["idPedido"] != null)
                        hddsidPedido.Value = Request["idPedido"].ToString();

                    if (Request["sPermiteNovoArquivo"] != null)
                    {
                        if (Request["sPermiteNovoArquivo"] == "S")
                            div_EnviarArquivos.Visible = true;
                        else
                            div_EnviarArquivos.Visible = false;
                    }

                    div_subtipo.Visible = false;
                    div_dtExpiracaoDoc.Visible = false;
                    div_dtRegistroDoc.Visible = false;
                    div_Ordem.Visible = false;

                    if (Convert.ToBoolean(Request["SubTipo"]))
                    {
                        Popula_Combo(ddlidTipoArquivo, "sp_Select 'FLOW_Arquivos_Tipo',@idUsuario=" + Variaveis.idUsuario() + ", @idPesquisa=" + Variaveis.idUsuario() + ", @sPesquisa='" + "Empresas" + "'", "idTipoArquivo", "sDscTipoArquivo", false, "Selecione o Tipo do Arquivo", "0");
                        if (!ValidaPermissao(Permissao.Empresas.ArquivoBalancos)) ddlidTipoArquivo.Items.Remove(ddlidTipoArquivo.Items.FindByValue("107"));
                        if (!ValidaPermissao(Permissao.Empresas.ArquivoCertidõesEmpresa)) ddlidTipoArquivo.Items.Remove(ddlidTipoArquivo.Items.FindByValue("105"));
                        if (!ValidaPermissao(Permissao.Empresas.ArquivoCertidõesSocios)) ddlidTipoArquivo.Items.Remove(ddlidTipoArquivo.Items.FindByValue("106"));
                        if (!ValidaPermissao(Permissao.Empresas.ArquivoDocumento)) ddlidTipoArquivo.Items.Remove(ddlidTipoArquivo.Items.FindByValue("102"));
                        if (!ValidaPermissao(Permissao.Empresas.ArquivoSeguros)) ddlidTipoArquivo.Items.Remove(ddlidTipoArquivo.Items.FindByValue("108"));
                        if (!ValidaPermissao(Permissao.Empresas.ArquivoSocios)) ddlidTipoArquivo.Items.Remove(ddlidTipoArquivo.Items.FindByValue("104"));
                        if (!ValidaPermissao(Permissao.Empresas.ArquivoSTSO)) ddlidTipoArquivo.Items.Remove(ddlidTipoArquivo.Items.FindByValue("103"));
                    }
                    else
                        Popula_Combo(ddlidTipoArquivo, "sp_Select 'FLOW_Arquivos_Tipo',@idUsuario=" + Variaveis.idUsuario() + ",@idPesquisa=" + Variaveis.idUsuario() + ", @sPesquisa='" + sTipoObjeto + "'", "idTipoArquivo", "sDscTipoArquivo", false, "Selecione o Tipo do Arquivo", "0");

                    PesquisarArquivo(Request["idObjeto"].ToString(), "", sTipoObjeto);
                }
                else
                    PesquisarArquivo("0", "", sTipoObjeto);
            }

            var requestTarget = Request["__EVENTTARGET"];
            if (requestTarget == "dialog_Apagar") ExcluirArquivo();
            if (requestTarget == "dialog_arquivo") DownloadArquivo(hddArquivo.Value);
            if (requestTarget == "dialog_CheckArquivo")
            {
                if (hddChecked.Value != "")
                    DownloadArquivoCheck(hddChecked.Value);
                else
                {
                    MensagemPagina1.MostraMensagem_Erro("Selecione um Arquivo !");
                    PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
                }
            }
            if (requestTarget == "dialog_Aceitar") AceitarArquivo();
            if (requestTarget == "dialog_Arquivar") ArquivarDoc();

            RegistraScript();
        }

        protected void PesquisarArquivo(string idObjeto, string idCliente, string sTipoObjeto)
        {
            hddidObjeto.Value = idObjeto;
            hddsTipoObjeto.Value = sTipoObjeto;

            txtEnviarArquivo_sDscArquivo.Text = "";

            if (!string.IsNullOrEmpty(Request["sExibicao"]) && Convert.ToBoolean(Request["sExibicao"]))
            {
                div_EnviarArquivos.Visible = false;
                gv_Arquivo.Columns[11].Visible = false;
            }
            else div_EnviarArquivos.Visible = true;

            if (ddlidTipoArquivo.Items.Count == 2)
                ddlidTipoArquivo.SelectedIndex = 1;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Consultar" },
                { "@sTipoObjeto", sTipoObjeto },
                { "@idObjeto", idObjeto },
                { "@idUsuario", Variaveis.idUsuario() }
            };

            if (Convert.ToBoolean(Request["SubTipo"]))
                vParametros.Add("@idOrdem", ddlOrdem.SelectedValue);
            if (hddsTipoObjeto.Value == "EmpresaSTSO" || hddsTipoObjeto.Value == "ColaboradorSTSO")
                vParametros.Add("@sObservacao", hddsidPedido.Value);

            DataSet dsArquivos = ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (ValidarDataSet(dsArquivos, out string sErro))
            {
                gv_Arquivo.DataSource = dsArquivos.Tables[0];
                gv_Arquivo.DataBind();
                btDownload.Visible = true;

                if (hddsTipoObjeto.Value != "PedidoART")
                    div_Ordem.Visible = false;
            }
            else
            {
                gv_Arquivo.DataSource = null;
                gv_Arquivo.DataBind();
                btDownload.Visible = false;
                div_Ordem.Visible = false;
            }

            if (Convert.ToBoolean(Request["SubTipo"]))
            {
                GridViewHelper helper = new GridViewHelper(gv_Arquivo);
                helper.GroupHeader += new GroupEvent(helper_GroupHeader);
                helper.RegisterGroup("sDscCategoria", true, true);
                helper.ApplyGroupSort();
            }

            ConsultarImagem(idObjeto, sTipoObjeto);
            RegistraScript();
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
                    DataSet dsPesquisa = ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametros);
                    DataRow imgBd = dsPesquisa.Tables[0].Rows[0];

                    img_Produto.ImageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);
                    img_Produto.Style["display"] = "block";
                }
            }
            catch { }
        }

        #endregion

        #region | Utils

        void LimpaCampos()
        {
            ddlidTipoArquivo.SelectedIndex = 0;

            if (Convert.ToBoolean(Request["SubTipo"]))
            {
                ddlSubtipoArquivo.SelectedIndex = 0;
                txtdtExpiracaoDoc.Text = "";
                txtdtRegistroDoc.Text = "";

                if (ddlidTipoArquivo.SelectedValue == "0")
                {
                    div_subtipo.Visible = false;
                    div_dtExpiracaoDoc.Visible = false;
                    div_dtRegistroDoc.Visible = false;
                }
            }
        }

        bool ValidarEnvioArquivo()
        {
            bool bRetorno = true;

            if (ddlidTipoArquivo.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione o Tipo de Arquivo!");
                ddlidTipoArquivo.Focus();
                PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
                return false;
            }

            if (!fu_Arquivo.HasFile && FileName == "")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um arquivo para enviar!");
                fu_Arquivo.Focus();
                PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
                return false;
            }

            if (!string.IsNullOrEmpty(hddsExtensoes.Value) && !hddsExtensoes.Value.Contains(Path.GetExtension(fu_Arquivo.FileName).ToLower()))
            {
                MensagemPagina.MostraMensagem_Erro($"Para o Tipo de Arquivo selecionado, é necessário que o Arquivo possua uma das seguintes extensões: \"{hddsExtensoes.Value.Replace(",", ", ").Trim()}\"!");
                fu_Arquivo.Focus();
                PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
                return false;
            }

            if (Convert.ToBoolean(Request["SubTipo"]) && ddlSubtipoArquivo.Items.Count > 1)
            {
                if (ddlSubtipoArquivo.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione o sub Tipo de Arquivo!");
                    ddlSubtipoArquivo.Focus();
                    PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
                    return false;
                }

                if (txtdtExpiracaoDoc.Text != "" && txtdtRegistroDoc.Text != "")
                {
                    DateTime expiracao = DateTime.Parse(txtdtExpiracaoDoc.Text);
                    DateTime registro = DateTime.Parse(txtdtRegistroDoc.Text);

                    if (expiracao < registro)
                    {
                        MensagemPagina.MostraMensagem_Erro("A Data de Expiração não pode ser anterior à Data de Registro!");
                        txtdtExpiracaoDoc.Focus();
                        PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
                        return false;
                    }
                }
            }

            return bRetorno;
        }

        void AceitarArquivo()
        {
            Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
            {
                { "@sFuncao", "ALTERAR" },
                { "@idArquivo", hddAceitarDoc.Value },
                { "@sAceitarDoc", "S" },
                { "@sObservacao", $"Data expiração aceita em {DateTime.Now:dd/MM/yyyy} por {Variaveis.sUsuarioLogado()}" }
            };
            ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

            PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
        }

        void ArquivarDoc()
        {
            if (hddArquivarDoc.Value != "")
            {
                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "ARQUIVAR_DOC" },
                    { "@idArquivo", hddArquivarDoc.Value },
                    { "@sArquivado", "S" },
                    { "@idUsuarioArquivado", Variaveis.idUsuario() }
                };
                ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParam);

                PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
            }
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
                    { "@idUsuario", Variaveis.idUsuario() }
                };
                ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametroArquivo_Excluir);

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            return bRetorno;
        }

        void ExcluirArquivo()
        {
            if (hddExcluirArquivo.Value != null)
            {
                Documento_Deletar(hddExcluirArquivo.Value);
                PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
            }
        }

        void DownloadArquivo(string arquivoId)
        {
            Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idArquivo", arquivoId }
            };
            DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

            foreach (DataRow item in tb.Rows)
            {
                string sNomeArquivo = item["sNomeArquivo"].ToString().Replace(",", "");

                FileStream lObjFile;
                lObjFile = new Arquivo().TransformarArrayBytesEmArquivo((byte[])item["vbArquivo"], Server.MapPath("~/Download/" + sNomeArquivo));
                lObjFile.Close();

                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}", sNomeArquivo));
                Response.TransmitFile(Server.MapPath("~/Download/" + sNomeArquivo));
                Response.End();
            }

            PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
        }

        void DownloadArquivoCheck(string arquivoId)
        {
            Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHECHECK" },
                { "@idArquivoCheck", arquivoId }
            };
            DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

            string zipFileName = $"Arquivos_{DateTime.Now:yyyyMMddHHmmss}.zip";
            string zipFilePath = Server.MapPath($"~/Download/{zipFileName}");

            using (FileStream zipFile = new FileStream(zipFilePath, FileMode.Create))
            {
                using (ZipArchive zipArchive = new ZipArchive(zipFile, ZipArchiveMode.Create))
                {
                    foreach (DataRow item in tb.Rows)
                    {
                        ZipArchiveEntry zipEntry = zipArchive.CreateEntry(item["sNomeArquivo"].ToString().Replace(",", ""));
                        using (Stream entryStream = zipEntry.Open())
                        {
                            byte[] arquivoBytes = (byte[])item["vbArquivo"];
                            entryStream.Write(arquivoBytes, 0, arquivoBytes.Length);
                        }
                    }
                }
            }

            Response.ContentType = "application/zip";
            Response.AppendHeader("Content-Disposition", $"attachment; filename={zipFileName}");
            Response.TransmitFile(zipFilePath);
            Response.End();

            hddChecked.Value = "";

            File.Delete(zipFilePath);

            PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
        }

        private void helper_GroupHeader(string groupName, object[] values, GridViewRow row)
        {
            row.BackColor = Color.FromArgb(173, 216, 230);
            row.Cells[0].Font.Bold = true;
            row.Cells[0].ForeColor = Color.Black;
            row.Cells[0].Text = row.Cells[0].Text.ToUpper();
            row.Cells[0].HorizontalAlign = HorizontalAlign.Left;
        }

        private string ConvertSortDirectionToSql(SortDirection sortDirection)
        {
            string m_SortDirection = string.Empty;

            switch (sortDirection)
            {
                case SortDirection.Ascending: m_SortDirection = "ASC"; break;
                case SortDirection.Descending: m_SortDirection = "DESC"; break;
            }

            return m_SortDirection;
        }

        #endregion

        #region | Eventos

        protected void gv_Arquivo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (Convert.ToBoolean(Request["SubTipo"]))
                EsconderColunas(e, 2);
            else
                EsconderColunas(e, 1, 2, 7, 8, 9);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("id", gv_Arquivo.DataKeys[e.Row.RowIndex]["idArquivo"].ToString());

                if (e.Row.Cells[7].Text != "&nbsp;" && !string.IsNullOrEmpty(e.Row.Cells[7].Text))
                    e.Row.Cells[7].Text = DateTime.Parse(e.Row.Cells[7].Text).ToString("dd/MM/yyyy");

                if (e.Row.Cells[8].Text != "&nbsp;" && !string.IsNullOrEmpty(e.Row.Cells[8].Text))
                    e.Row.Cells[8].Text = DateTime.Parse(e.Row.Cells[8].Text).ToString("dd/MM/yyyy");

                if (gv_Arquivo.DataKeys[e.Row.RowIndex]["sAceitarDoc"].ToString().Equals("S"))
                    (e.Row.Cells[9].FindControl("btnAceitar") as Button).Visible = false;

                if (e.Row.Cells[7].Text == "" || e.Row.Cells[7].Text == "&nbsp;")
                    (e.Row.Cells[8].FindControl("btnAceitar") as Button).Visible = false;
            }
        }

        protected void gv_Arquivo_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Documento_Deletar(gv_Arquivo.Rows[e.RowIndex].Cells[0].Text.ToString());
            PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
        }

        protected void gv_Arquivo_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (Convert.ToBoolean(Request["SubTipo"]))
            {
                if (gv_Arquivo.DataSource is DataTable tb)
                {
                    gv_Arquivo.DataSource = new DataView(tb) { Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection) };
                    gv_Arquivo.DataBind();
                }
            }
        }

        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {
            if (ValidarEnvioArquivo())
            {
                byte[] lObjArquivo = null;
                Stream lObjConteudoArquivo;
                string sNomeArquivo;
                string lStrCaminhoArquivo;
                string lStrNomeArquivo;

                if (FileName != "")
                {
                    lObjArquivo = null;
                    lObjConteudoArquivo = new MemoryStream(FileBytes);
                    sNomeArquivo = FileName;
                    lStrCaminhoArquivo = "";
                    lStrNomeArquivo = Path.GetFileName(lStrCaminhoArquivo);
                    FileName = "";
                }
                else
                {
                    lObjArquivo = null;
                    lObjConteudoArquivo = fu_Arquivo.PostedFile.InputStream;
                    sNomeArquivo = fu_Arquivo.FileName;
                    lStrCaminhoArquivo = fu_Arquivo.PostedFile.FileName;
                    lStrNomeArquivo = Path.GetFileName(lStrCaminhoArquivo);
                }

                try
                {
                    lObjArquivo = new Arquivo().TransformaArquivoEmArrayBytes(lStrNomeArquivo, lStrCaminhoArquivo, lObjConteudoArquivo);
                }
                catch
                {
                    return;
                }

                cls_Arquivos Arquivo = new cls_Arquivos();
                if (Convert.ToBoolean(Request["SubTipo"]) && ddlSubtipoArquivo.Items.Count > 1)
                {
                    Arquivo.idTipoArquivo = Convert.ToInt32(ddlSubtipoArquivo.SelectedValue);
                    Arquivo.dtExpiracaoDoc = txtdtExpiracaoDoc.Text;
                    Arquivo.dtRegistroDoc = txtdtRegistroDoc.Text;
                }
                else Arquivo.idTipoArquivo = Convert.ToInt32(ddlidTipoArquivo.SelectedValue);

                Arquivo.idObjeto = Convert.ToInt32(hddidObjeto.Value);
                Arquivo.sNomeArquivo = sNomeArquivo;
                Arquivo.sDscArquivo = txtEnviarArquivo_sDscArquivo.Text;

                if (hddsidPedido.Value != "")
                    Arquivo.sObservacao = hddsidPedido.Value;
                else
                    Arquivo.sObservacao = "";

                Arquivo.idUsuario = Convert.ToInt32(Variaveis.idUsuario());
                Arquivo.vbArquivo = lObjArquivo;

                DataSet ds = Arquivo.EnviarArquivo(Arquivo);

                if (ValidarDataSet(ds, out _))
                {
                    if (hddsTipoObjeto.Value == "EmpresaSTSO" || hddsTipoObjeto.Value == "ColaboradorSTSO")
                    {
                        Dictionary<string, string> vParametro = new Dictionary<string, string>
                        {
                            { "@sFuncao", "INSERIR_DOCUMENTO_STSO" },
                            { "@idArquivo", DATASET(ds, "idArquivo") },
                            { "@idPedido", hddsidPedido.Value },
                            { "@sDscsArquivo", hddsEmpresaxColaborador.Value + " - " + hddsTipoObjeto.Value + " - " + hddsTipoObjeto.Value + " - " + sNomeArquivo.Replace("-", "_") + " - " + txtEnviarArquivo_sDscArquivo.Text.Replace("-", "_") },
                            { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                            { "@sAprovacao", "N" }
                        };

                        if (hddsTipoObjeto.Value == "EmpresaSTSO")
                            vParametro["@sTipo"] = "Empresa";
                        else
                            vParametro["@sTipo"] = "Colaborador";

                        ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametro);
                    }

                    PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);

                    string msg = "Arquivo enviado com sucesso!";

                    try
                    {
                        if (DATASET(ds, "idTipoArquivo") == "600")
                        {
                            Arquivo arquivo = new Arquivo();

                            foreach (DataRow linha in ds.Tables[0].Rows)
                            {
                                arquivo.TransformarArrayBytesEmArquivo((byte[])linha["vbArquivo"], Server.MapPath("~/Manuais/" + DATASET(ds, "sNomeArquivo")));
                                msg = "Manual enviado e adicionado à pasta de Manuais com sucesso!";
                            }
                        }
                    }
                    catch { }

                    MensagemPagina.MostraMensagem_Sucesso(msg);
                }

                LimpaCampos();
            }
        }

        protected void ddlidTipoArquivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(Request["SubTipo"]))
            {
                if (ddlidTipoArquivo.SelectedValue == "0")
                {
                    div_subtipo.Visible = false;
                    div_dtExpiracaoDoc.Visible = false;
                    div_dtRegistroDoc.Visible = false;
                }
                else
                {
                    Popula_Combo(ddlSubtipoArquivo, "sp_Select 'FLOW_Arquivos_Tipo',@idUsuario=" + Variaveis.idUsuario() + ", @idPesquisa=" + Variaveis.idUsuario() + ", @sPesquisa='" + ddlidTipoArquivo.SelectedItem.ToString() + "'", "idTipoArquivo", "sDscTipoArquivo", false, "Selecione o Tipo do Arquivo", "0");

                    if (ddlSubtipoArquivo.Items.Count > 1)
                    {
                        div_subtipo.Visible = true;
                        div_dtExpiracaoDoc.Visible = true;
                        div_dtRegistroDoc.Visible = true;
                    }
                }

                PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
            }

            if (ddlidTipoArquivo.SelectedValue != "0")
            {
                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE_TIPO_ARQUIVO" },
                    { "@idTipoArquivo", ddlidTipoArquivo.SelectedValue }
                };
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParam);

                string extensoes = DATASET(ds, "sExtensoesArquivo");

                fu_Arquivo.Attributes.Remove("accept");
                fu_Arquivo.Attributes.Add("accept", string.IsNullOrEmpty(extensoes) || extensoes.Replace(".", "").Replace(",", "").Trim().Length <= 0 ? "" : extensoes);

                hddsExtensoes.Value = fu_Arquivo.Attributes["accept"];
            }
        }

        protected void ddlOrdem_SelectedIndexChanged(object sender, EventArgs e)
        {
            hddidOrdem.Value = ddlOrdem.SelectedValue;
            PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
        }

        #endregion

        #region | Script

        void RegistraScript()
        {
            ScriptManager.RegisterClientScriptInclude(Page, GetType(), "carregarIMG", ResolveUrl("~/App/JS/UploadImagem.js"));

            StringBuilder sb = new StringBuilder();

            sb.Append("$v192(function() {");
            sb.Append("$v192(\"#dialog_Apagar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"dialog_Apagar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192('.ExcluirArquivo').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192(hddExcluirArquivo).val($v192(this).closest('tr').attr('id'));");
            sb.Append("$v192('#dialog_Apagar').dialog('open');");
            sb.Append("});");
            sb.Append("});");

            sb.Append("$v192('.Arquivo').find('a').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192(hddArquivo).val($v192(this).closest('tr').attr('id'));");
            sb.Append("__doPostBack(\"dialog_arquivo\", \"\");");
            sb.Append("arquivo(arquivoId);");
            sb.Append("});");

            sb.Append("$('.Todos').change(function(){ var isChecked = $(this).find('input').is(':checked'); console.log(isChecked); $('.Individual').find('input').prop('checked', isChecked); });");

            sb.Append("$v192('.btDownload').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("var hddChecked = [];");
            sb.Append("$v192('.Individual').each(function() {");
            sb.Append("var checkboxId = $v192(this).closest('tr').attr('id');");
            sb.Append("if ($v192(this).find('input').is(':checked')) {");
            sb.Append("hddChecked.push(checkboxId);");
            sb.Append("}");
            sb.Append("});");
            sb.Append("if (hddChecked.length > 0) {");
            sb.Append("$('[id*=hddChecked]').val(hddChecked.join(','));");
            sb.Append("__doPostBack('dialog_CheckArquivo', '');");
            sb.Append("} else {");
            sb.Append("$('[id*=hddChecked]').val('');");
            sb.Append("__doPostBack('dialog_CheckArquivo', '');");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192(function() {");
            sb.Append("$v192(\"#dialog_Aceitar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"dialog_Aceitar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192('.AceitarDoc').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192(hddAceitarDoc).val($v192(this).closest('tr').attr('id'));");
            sb.Append("$v192('#dialog_Aceitar').dialog('open');");
            sb.Append("});");
            sb.Append("});");

            sb.Append("$v192('.ArquivarDoc').click(function(e) {");
            sb.Append("    e.preventDefault();");
            sb.Append("    var hddArquivarDoc = [];");
            sb.Append("    $v192('.Individual').each(function() {");
            sb.Append("        var checkboxId = $v192(this).closest('tr').attr('id');");
            sb.Append("        if ($v192(this).find('input').is(':checked')) {");
            sb.Append("            hddArquivarDoc.push(checkboxId);");
            sb.Append("        }");
            sb.Append("    });");
            sb.Append("    if (hddArquivarDoc.length > 0) {");
            sb.Append("        $('[id*=hddArquivarDoc]').val(hddArquivarDoc.join(','));");
            sb.Append("    } else {");
            sb.Append("        $('[id*=hddArquivarDoc]').val('');");
            sb.Append("    }");
            sb.Append("    $v192('#dialog_Arquivar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(function() {");
            sb.Append("    $v192(\"#dialog_Arquivar\").dialog({");
            sb.Append("        resizable: false,");
            sb.Append("        height: \"auto\",");
            sb.Append("        width: 400,");
            sb.Append("        modal: true,");
            sb.Append("        autoOpen: false,");
            sb.Append("        buttons: {");
            sb.Append("            \"Sim\": function() {");
            sb.Append("                __doPostBack(\"dialog_Arquivar\", \"\");");
            sb.Append("                $v192(this).dialog(\"close\");");
            sb.Append("            },");
            sb.Append("            \"Não\": function() {");
            sb.Append("                $v192(this).dialog(\"close\");");
            sb.Append("            }");
            sb.Append("        }");
            sb.Append("    });");
            sb.Append("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "JS_ExcluirAquivo", sb.ToString(), true);
        }

        #endregion
    }
}