using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
using TT_Flow.FrameWork;
using TT.FrameWork;
using System.Web.DynamicData;
using System.Globalization;
using System.IO.Compression;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class Documentos_RRHH : System.Web.UI.Page
    {
        public List<FrameWork.cls_Arquivos> bs_Documentos
        {
            get
            {
                if (ViewState["bs_Documentos"] == null)
                {
                    ViewState["bs_Documentos"] = new List<FrameWork.cls_Arquivos>();
                }

                return (List<FrameWork.cls_Arquivos>)ViewState["bs_Documentos"];
            }

            set { ViewState["bs_Documentos"] = value; }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();
            string sTipoObjeto = "Pedido";
            if (!IsPostBack)
            {
                if (Request["idObjeto"] != null)
                {
                    if (Request["sTipoObjeto"] != null)
                    {
                        sTipoObjeto = Request["sTipoObjeto"].ToString();
                    }

                    FUNCOES.Popula_Combo(ddlidTipoDocumento, "sp_Select 'FLOW_Arquivos_Tipo', @idPesquisa=" + IDENTITY.Variaveis.idUsuario() + ", @sPesquisa='" + sTipoObjeto + "'", "idTipoArquivo", "sDscTipoArquivo", false, "Selecione o Tipo do Documento", "0");

                    PesquisarArquivo(Request["idObjeto"].ToString(), "", sTipoObjeto);
                }
                else
                {
                    PesquisarArquivo("0", "", sTipoObjeto);
                }
            }
            var requestTarget = this.Request["__EVENTTARGET"];
            if (requestTarget == "dialog_Apagar")
            {
                ExcluirArquivo();
            }
            if (requestTarget == "dialog_Aceitar")
            {
                AceitarArquivo();
            }
            //------------------Higor Maestrello 18-06-2024------------------------------
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
            if (requestTarget == "dialog_Arquivar")
            {
                ArquivarDoc();
            }
            //-------------------------------------------------------------------------------
            RegistraScript();
        }

        protected void PesquisarArquivo(string idObjeto, string idCliente, string sTipoObjeto)
        {
            hddidObjeto.Value = idObjeto;
            hddsTipoObjeto.Value = sTipoObjeto;
            txtEnviarArquivo_sDscArquivo.Text = "";

            if (ddlidTipoDocumento.Items.Count == 2)
            {
                ddlidTipoDocumento.SelectedIndex = 1;
            }
            else
            {
                ddlidTipoDocumento.SelectedIndex = 0;
            }


            string sErro = "";
            DataSet dsDocumentos;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "Consultar");
            vParametros.Add("@sTipoObjeto", sTipoObjeto);
            vParametros.Add("@idObjeto", idObjeto);
            vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

            dsDocumentos = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (BD.ValidarDataSet(dsDocumentos, out sErro))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(gv_Arquivo, dsDocumentos), true);
                btDownload.Visible = true;
            }
            else
            {
                gv_Arquivo.DataSource = null;
                gv_Arquivo.DataBind();
                btDownload.Visible = false;
            }

        }

        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {
            if (ValidarEnvioArquivo())
            {
                string sErro = "";
                Byte[] lObjArquivo = null;
                Stream lObjConteudoArquivo = fu_Documento.PostedFile.InputStream;
                string sNomeArquivo = fu_Documento.FileName;
                string lStrCaminhoArquivo = fu_Documento.PostedFile.FileName;
                string lStrNomeArquivo = Path.GetFileName(lStrCaminhoArquivo);
                string lStrExtencaoArquivo = Path.GetExtension(lStrNomeArquivo);

                try
                {
                    TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                    lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(lStrNomeArquivo, lStrCaminhoArquivo, lObjConteudoArquivo);
                }
                catch
                {
                    return;
                }

                TT_Flow.FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                Arquivo.idTipoArquivo = Convert.ToInt32(ddlidTipoDocumento.SelectedValue);
                Arquivo.idObjeto = Convert.ToInt32(hddidObjeto.Value);
                Arquivo.sNomeArquivo = sNomeArquivo;
                Arquivo.sDscArquivo = txtEnviarArquivo_sDscArquivo.Text;
                Arquivo.sObservacao = "";
                Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                Arquivo.vbArquivo = lObjArquivo;
                Arquivo.dtExpiracaoDoc = txtdtExpiracaoDoc.Text;

                DataSet dsItem = Arquivo.EnviarArquivo(Arquivo);
                if (BD.ValidarDataSet(dsItem, out sErro))
                {
                    PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
                    MensagemPagina.MostraMensagem_Sucesso("Documento enviado com sucesso!");
                }
            }
        }

        bool ValidarEnvioArquivo()
        {
            bool bRetorno = true;

            if (ddlidTipoDocumento.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione o Tipo de Documento!");
                ddlidTipoDocumento.Focus();
                return false;
            }

            if (!fu_Documento.HasFile)
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um Documento para enviar!");
                fu_Documento.Focus();
                return false;
            }

            if (txtdtExpiracaoDoc.Text != "")
            {
                DateTime dataExpiracao;
                if (!DateTime.TryParse(txtdtExpiracaoDoc.Text, out dataExpiracao))
                {
                    MensagemPagina.MostraMensagem_Erro("Formato de data inválido!");
                    txtdtExpiracaoDoc.Focus();
                    return false;
                }
            }
            //if (txtEnviarArquivo_sDscArquivo.Text.Length< 10)
            //{
            //    MensagemPagina.MostraMensagem_Erro("Informe uma Descrição do Documento válida!");
            //    txtEnviarArquivo_sDscArquivo.Focus();
            //    return false;
            //}

            return bRetorno;

        }

        protected void gv_Arquivo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument.ToString() != "")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                var gr = gv_Arquivo.Rows[index];
                string idArquivo = gr.Cells[1].Text; //Codigo do arquivo Chamado


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

                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Tab_Arquivo", "$('#tab_Pedido a[href=\"#arquivos\"]').tab('show');", true);

            }

        }

        protected void gv_Arquivo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 1);
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("id", gv_Arquivo.DataKeys[e.Row.RowIndex]["idArquivo"].ToString());

                string aceito = gv_Arquivo.DataKeys[e.Row.RowIndex]["sAceitarDoc"].ToString();
                if (aceito.Equals("S"))
                {
                    (e.Row.Cells[6].FindControl("btnAceitar") as Button).Visible = false;
                }
                if (e.Row.Cells[5].Text == "" || e.Row.Cells[5].Text == "&nbsp;")
                {
                    (e.Row.Cells[6].FindControl("btnAceitar") as Button).Visible = false;
                }
                if (e.Row.Cells[5].Text != "" && e.Row.Cells[5].Text != "&nbsp;")
                {
                    e.Row.Cells[5].Text = DateTime.Parse(e.Row.Cells[5].Text).ToString("dd/MM/yyyy");
                }
            }
        }

        void RegistraScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

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
            sb.Append("$v192(hddAceitarDoc).val($v192(this).attr('id'));");
            sb.Append("$v192('#dialog_Aceitar').dialog('open');");
            sb.Append("});");
            sb.Append("});");

            //------------------Higor Maestrello 18-06-2024-----------------------------
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
            //-----------------------------------------------------------------------------

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "JS_ExcluirAquivo", sb.ToString(), true);
        }

        bool Documento_Deletar(string idArquivo)
        {
            bool bRetorno = false;
            try
            {

                DataSet dsDocumento_Excluir;
                Dictionary<String, String> vParametroArquivo_Excluir = new Dictionary<string, string>();
                vParametroArquivo_Excluir.Add("@sFuncao", "DOCUMENTO_DELETAR");
                vParametroArquivo_Excluir.Add("@idArquivo", idArquivo);
                vParametroArquivo_Excluir.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                dsDocumento_Excluir = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametroArquivo_Excluir);


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

        void AceitarArquivo()
        {
            int index = int.Parse(hddAceitarDoc.Value.Replace("gv_Arquivo_btnAceitar_", ""));
            var gr = gv_Arquivo.Rows[index];
            string idArquivo = gr.Cells[1].Text;

            Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
            DataTable dtArquivo;
            vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "ALTERAR" },
                        {"@idArquivo",                  idArquivo},
                        {"@sAceitarDoc",                "S"},
                        {"@sObservacao",                string.Format("Data expiração aceita em {0} por {1}", DateTime.Now.ToString("dd/MM/yyyy"), IDENTITY.Variaveis.sUsuarioLogado())}
                    };

            dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);
            PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
        }

        //------------------Higor Maestrello 18-06-2024------------------------------
        void DownloadArquivoCheck(string arquivoId)
        {
            Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
            DataTable dtArquivo;
            vParametrosItem = new Dictionary<string, string>
            {
                {"@sFuncao", "CONSULTAR_DETALHECHECK" },
                {"@idArquivoCheck", arquivoId}
            };

            dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

            // Nome do arquivo zip que será criado
            string zipFileName = $"Arquivos_{DateTime.Now:yyyyMMddHHmmss}.zip";
            string zipFilePath = Server.MapPath($"~/Download/{zipFileName}");

            using (FileStream zipFile = new FileStream(zipFilePath, FileMode.Create))
            {
                using (ZipArchive zipArchive = new ZipArchive(zipFile, ZipArchiveMode.Create))
                {
                    foreach (DataRow item in dtArquivo.Rows)
                    {
                        string sNomeArquivo = item["sNomeArquivo"].ToString().Replace(",", "");

                        // Criar uma entrada no arquivo zip
                        ZipArchiveEntry zipEntry = zipArchive.CreateEntry(sNomeArquivo);

                        // Escrever os dados do arquivo dentro da entrada do zip
                        using (Stream entryStream = zipEntry.Open())
                        {
                            byte[] arquivoBytes = (byte[])item["vbArquivo"];
                            entryStream.Write(arquivoBytes, 0, arquivoBytes.Length);
                        }
                    }
                }
            }
            FUNCOES.DownloadArquivo(Page, zipFileName);
            PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
        }
        //---------------------------------------------------------------------------

        void ArquivarDoc()
        {
            if (hddArquivarDoc.Value != "")
            {
                string idArquivo = hddArquivarDoc.Value;

                Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                DataTable dtArquivo;
                vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "ARQUIVAR_DOC" },
                        {"@idArquivo",                  idArquivo},
                        {"@sArquivado",                 "S"},
                        {"@idUsuarioArquivado",        IDENTITY.Variaveis.idUsuario()}
                    };

                dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);
                PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
            }
        }
    }

    //protected void gv_Arquivo_RowDeleting(object sender, GridViewDeleteEventArgs e)
    //{
    //    string idArquivo = "";
    //    idArquivo = gv_Arquivo.Rows[e.RowIndex].Cells[0].Text.ToString();
    //    Documento_Deletar(idArquivo);
    //    PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);


    //}
}