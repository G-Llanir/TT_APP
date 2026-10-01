using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using System.IO;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.Web.Services;
using System.Data;
using System.Reflection.Emit;
using TT.FrameWork;
using static Permissao;
using System.ComponentModel;
using System.Drawing;
namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class ImportadorSaldo_Detalhe : System.Web.UI.Page
    {
        string tipoDado = "301";
        static string _idArquivo = "";
        string sProcedure = "sp_Manipula_tbl_Flow_Produtos";
        string sTituloPagina = "Importador de Saldo/Tabelas carregadas";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gv_TabelaSaldo.Visible = false;
                div_ExibeTabela.Visible = false;

                if (Request["id"] != "0")
                {
                    PesquisarArquivo("1", Request["id"].ToString(), "Saldo");
                }
                else
                {
                    PesquisarArquivo("1", "", "Saldo");
                }

                foreach (DataControlField coluna in gv_Arquivo.Columns)
                {
                    if (coluna.HeaderText.Equals("ID"))
                    {
                        coluna.Visible = false;
                        break;
                    }
                }
            }
        }

        protected void gv_Arquivo_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gv_Arquivo_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void gv_Arquivo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string sErro = "";
            if (e.CommandArgument.ToString() != "")
            {
                string cmName = e.CommandName.ToString();
                int index = int.Parse(e.CommandArgument.ToString());
                var gr = gv_Arquivo.Rows[index];
                string idArquivo = gr.Cells[0].Text; //Codigo do arquivo Chamado

                if (e.CommandName == "Download")
                {


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
                }
                else if (e.CommandName == "ExibirTabela")
                {
                    DataSet dsArquivos;
                    Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();

                    vParametrosItem.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametrosItem.Add("@idArquivo", idArquivo);
                    dsArquivos = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);


                    if (BD.ValidarDataSet(dsArquivos, out sErro))
                    {
                        DataTable dt = ExcelToDataTable((byte[])dsArquivos.Tables[0].Rows[0]["vbArquivo"], true);
                        gv_TabelaSaldo.DataSource = dt;
                        gv_TabelaSaldo.DataBind();
                        ViewState["gv_TabelaSaldo"] = dt;
                        _idArquivo = idArquivo;
                        gv_TabelaSaldo.Visible = true;
                        div_ExibeTabela.Visible = true;
                        lblTituloTabelaSaldo.Text = "Tabela" + " " + RETORNO.DATASET(dsArquivos, "sNomeArquivo");

                    }
                    else
                    {
                        gv_TabelaSaldo.DataSource = null;
                        gv_TabelaSaldo.DataBind();
                    }
                }
            }
        }

        private DataTable ExcelToDataTable(byte[] excelDocumentAsBytes, bool hasHeaderRow)
        {
            DataTable dt = new DataTable();
            string errorMessages = "";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            HttpPostedFile postedFile = fu_Planilha.PostedFile;


            using (MemoryStream stream = new MemoryStream(excelDocumentAsBytes))
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                if (excelPackage.Workbook.Worksheets.Count < 1)
                {
                    throw new Exception("Não há tabela dentro da planilha.");
                }

                if (excelPackage.Workbook.Worksheets.Count == 0)
                {
                    throw new Exception("There is only one worksheet in the Excel package. Index out of range exception could occur.");
                }
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0];
                //check if the worksheet is completely empty
                if (worksheet.Dimension == null)
                {
                    return dt;
                }

                //add the columns to the datatable
                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                {
                    string columnName = "Column " + j;
                    var excelCell = worksheet.Cells[1, j].Value;

                    if (excelCell != null)
                    {
                        var excelCellDataType = excelCell;

                        //if there is a headerrow, set the next cell for the datatype and set the column name
                        if (hasHeaderRow == true)
                        {
                            excelCellDataType = worksheet.Cells[2, j].Value;

                            columnName = excelCell.ToString();

                            //check if the column name already exists in the datatable, if so make a unique name
                            if (dt.Columns.Contains(columnName) == true)
                            {
                                columnName = columnName + "_" + j;
                            }
                        }

                        //try to determine the datatype for the column (by looking at the next column if there is a header row)
                        if (excelCellDataType is DateTime)
                        {
                            dt.Columns.Add(columnName, typeof(DateTime));
                        }
                        else if (excelCellDataType is Boolean)
                        {
                            dt.Columns.Add(columnName, typeof(Boolean));
                        }
                        else if (excelCellDataType is Double)
                        {
                            //determine if the value is a decimal or int by looking for a decimal separator
                            //not the cleanest of solutions but it works since excel always gives a double
                            if (excelCellDataType.ToString().Contains(".") || excelCellDataType.ToString().Contains(","))
                            {
                                dt.Columns.Add(columnName, typeof(Decimal));
                            }
                            else
                            {
                                dt.Columns.Add(columnName, typeof(Int64));
                            }
                        }
                        else
                        {
                            dt.Columns.Add(columnName, typeof(String));
                        }
                    }
                    else
                    {
                        dt.Columns.Add(columnName, typeof(String));
                    }
                }

                //start adding data the datatable here by looping all rows and columns
                for (int i = worksheet.Dimension.Start.Row + Convert.ToInt32(hasHeaderRow); i <= worksheet.Dimension.End.Row; i++)
                {
                    //create a new datatable row
                    DataRow row = dt.NewRow();

                    //loop all columns
                    for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                    {
                        var excelCell = worksheet.Cells[i, j].Value;

                        //add cell value to the datatable
                        if (excelCell != null)
                        {
                            try
                            {
                                row[j - 1] = excelCell;
                            }
                            catch
                            {
                                errorMessages += "Row " + (i - 1) + ", Column " + j + ". Invalid " + dt.Columns[j - 1].DataType.ToString().Replace("System.", "") + " value:  " + excelCell.ToString() + "<br>";
                            }
                        }
                    }

                    //add the new row to the datatable
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }



        protected void ddlidTipoArquivo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {
            if (ValidarEnvioArquivo())
            {
                string sErro = "";
                Byte[] lObjArquivo = null;
                Stream lObjConteudoArquivo;
                string sNomeArquivo;
                string lStrCaminhoArquivo;
                string lStrNomeArquivo;
                string lStrExtencaoArquivo;

                lObjArquivo = null;
                lObjConteudoArquivo = fu_Planilha.PostedFile.InputStream;
                sNomeArquivo = fu_Planilha.FileName;
                lStrCaminhoArquivo = fu_Planilha.PostedFile.FileName;
                lStrNomeArquivo = Path.GetFileName(lStrCaminhoArquivo);
                lStrExtencaoArquivo = Path.GetExtension(lStrNomeArquivo);

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
                Arquivo.idTipoArquivo = Convert.ToInt32(tipoDado);
                Arquivo.idObjeto = Convert.ToInt32("1");
                Arquivo.sNomeArquivo = sNomeArquivo;
                Arquivo.sDscArquivo = txtEnviarArquivo_sDscArquivo.Text;
                Arquivo.sObservacao = "";
                Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                Arquivo.vbArquivo = lObjArquivo;

                DataSet dsItem = Arquivo.EnviarArquivo(Arquivo);
                if (BD.ValidarDataSet(dsItem, out sErro))
                {
                    PesquisarArquivo(hddidObjeto.Value, "", hddsTipoObjeto.Value);
                    MensagemPagina.MostraMensagem_Sucesso("Arquivo enviado com sucesso!");
                }
            }
        }
        protected void PesquisarArquivo(string idObjeto, string idArquivo, string sTipoObjeto)
        {

            //hddidObjeto.Value = "1";
            hddsTipoObjeto.Value = sTipoObjeto;
            txtEnviarArquivo_sDscArquivo.Text = "";

            string sErro = "";
            DataSet dsArquivos;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();

            if (idObjeto != "" && idArquivo == "")
            {
                vParametros.Add("@sFuncao", "Consultar");
                vParametros.Add("@sTipoObjeto", sTipoObjeto);
                vParametros.Add("@idObjeto", idObjeto);
                dsArquivos = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametros);

                if (BD.ValidarDataSet(dsArquivos, out sErro))
                {
                    gv_Arquivo.DataSource = dsArquivos.Tables[0];
                    gv_Arquivo.DataBind();
                    lblTituloTabelaSaldo.Text = "Tabela" + " " + RETORNO.DATASET(dsArquivos, "sNomeArquivo");
                }
                else
                {
                    gv_Arquivo.DataSource = null;
                    gv_Arquivo.DataBind();
                }
            }
            else if (idArquivo != "0")
            {
                vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                vParametros.Add("@idArquivo", idArquivo);
                dsArquivos = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametros);



                if (BD.ValidarDataSet(dsArquivos, out sErro))
                {
                    DataTable dt = ExcelToDataTable((byte[])dsArquivos.Tables[0].Rows[0]["vbArquivo"], false);
                    gv_TabelaSaldo.DataSource = dt;
                    gv_TabelaSaldo.DataBind();
                    ViewState["gv_TabelaSaldo"] = dt;
                    _idArquivo = idArquivo;
                    gv_TabelaSaldo.Visible = true;
                    div_ExibeTabela.Visible = true;
                    lblTituloTabelaSaldo.Text = "Tabela" + " " + RETORNO.DATASET(dsArquivos, "sNomeArquivo");
                    updpTabelaSaldo.Update();


                }
                else
                {
                    gv_Arquivo.DataSource = null;
                    gv_Arquivo.DataBind();
                }
            }

            lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
        }

        bool ValidarEnvioArquivo()
        {
            bool bRetorno = true;

            if (!fu_Planilha.HasFile)
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um arquivo para enviar!");
                gv_TabelaSaldo.Focus();
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

        protected void fu_Planilha_DataBinding(object sender, EventArgs e)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            HttpPostedFile postedFile = fu_Planilha.PostedFile;


            if (fu_Planilha.HasFile)
            {
                try
                {
                    using (ExcelPackage excel = new ExcelPackage(postedFile.InputStream))
                    {
                        ExcelWorksheet ws = excel.Workbook.Worksheets[1];

                    }
                }
                catch { }
            }
        }
        protected void CarregarExcelParaGV(string endereco, string extensao, string hdr)
        {
            //DataTable dt;
            //Dictionary<string, string> vParametros = new Dictionary<string, string>();
            //vParametros.Add("@sFuncao", "C")


            //dt = BD.ExecutarDataTable();
        }

        protected void gv_TabelaSaldo_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gv_Arquivo_DataBinding(object sender, EventArgs e)
        {

        }

        protected void cmdAtualizarBanco_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                // create columns in the DataTable based on the columns in the GridView
                //foreach (DataControlField column in gv_TabelaSaldo.Columns)
                //{
                //    string columnName = string.Empty;
                //    if (column is BoundField)
                //    {
                //        columnName = ((BoundField)column).DataField;
                //    }
                //    else if (column is TemplateField)
                //    {
                //        columnName = ((TemplateField)column).HeaderText;
                //    }
                //    else if (column is CommandField)
                //    {
                //        columnName = ((CommandField)column).HeaderText;
                //    }
                //    else
                //    {
                //        continue; // skip other types of columns
                //    }
                //    dt.Columns.Add(columnName);
                //}

                DataTable dt = new DataTable();
                string nBtSalvar = cmdAtualizarBanco.Text;

                if (nBtSalvar == "Atualizar Saldo")
                {
                    if (ViewState["gv_TabelaSaldo"] != null)
                    {
                        //colocar foco na tela e informar que está carregando no banco de dados
                        fu_Planilha.Focus();

                        int nSalvosBd = 0;
                        string sErro = "";
                        DataTable _dt = (DataTable)ViewState["gv_TabelaSaldo"];
                        DataTable dt_NaoAtualizado = new DataTable();

                        dt_NaoAtualizado.Columns.Add("sCodigo", typeof(string));
                        dt_NaoAtualizado.Columns.Add("nEstoqueAtual", typeof(int));
                        dt_NaoAtualizado.Columns.Add("sDscProduto", typeof(string));


                        foreach (DataRow r in _dt.Rows)
                        {
                            // ds.Tables[0].Rows[nLinha][sCampo].ToString().Trim(); }
                            if (!r.IsNull(0))
                            {

                                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                                vParametros.Add("@sFuncao", "INCLUIR_SALDO");
                                vParametros.Add("@sCodigo", (string)r["CÓD_PRODUTO"]);
                                vParametros.Add("@nEstoqueAtual", r["Estoque"].ToString());
                                vParametros.Add("@idArquivo", _idArquivo);
                                vParametros.Add("@sSituacaoCadastral", "S");
                                vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                                DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                                if (BD.ValidarDataSet(dsSalvar, out sErro))
                                {
                                    string idArquivo = RETORNO.DATASET(dsSalvar, 0, "idArquivo");
                                    nSalvosBd++;


                                }
                                //adiciona em um datatable os dados para mostrar novamente na tela
                                else if (sErro.ToString() == "Erro Geral: Não é possível encontrar a tabela 0.")
                                {
                                    DataRow novaRow = dt_NaoAtualizado.NewRow();
                                    novaRow["sCodigo"] = r["CÓD_PRODUTO"].ToString();
                                    novaRow["sDscProduto"] = r["DESCRIÇÃO"].ToString();
                                    try
                                    {
                                        novaRow["nEstoqueAtual"] = r["Estoque"].ToString();
                                    }
                                    catch
                                    {
                                        novaRow["nEstoqueAtual"] = r["Estoque"].ToString();
                                    }
                                    dt_NaoAtualizado.Rows.Add(novaRow);
                                    //throw new Exception("BD: " + sErro.ToString());
                                }
                            }
                        }
                        MensagemPagina.MostraMensagem_Sucesso(nSalvosBd.ToString() + " " + "Registros gravados com sucesso");
                        if (dt_NaoAtualizado.Rows.Count > 0)
                        {
                            lblTituloTabelaSaldo.Text = "Itens não carregados no banco";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(gv_TabelaSaldo, dt_NaoAtualizado), true);
                            
                        }
                        //Mudar nome para que ele possa incluir os novos dados no banco de dados
                        if (dt_NaoAtualizado.Rows.Count > 1)
                        {
                            ViewState["gv_TabelaSaldo"] = dt_NaoAtualizado;
                            cmdAtualizarBanco.Text = "Atualizar Produtos";
                        }
                        else
                            cmdAtualizarBanco.Visible = false;
                    }
                }
                else if (nBtSalvar == "Atualizar Produtos")
                {
                    string sErro = "";
                    string sCodigo = "";
                    DataTable _dt = (DataTable)ViewState["gv_TabelaSaldo"];

                    foreach (DataRow r in _dt.Rows)
                    {
                        if (!r.IsNull(0))
                        {
                            Dictionary<string, string> vParametros = new Dictionary<string, string>();
                            vParametros.Add("@sFuncao", "INCLUIR_PRODUTOS");
                            vParametros.Add("@sCodigo", r["sCodigo"].ToString());
                            vParametros.Add("@sDscProduto", r["sDscProduto"].ToString());
                            vParametros.Add("@nEstoqueAtual", r["nEstoqueAtual"].ToString());
                            vParametros.Add("@sSituacaoCadastral", "N");
                            vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                            DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                            if (BD.ValidarDataSet(ds, out sErro))
                            {
                                sCodigo = RETORNO.DATASET(ds, 0, "idItem");
                            }
                            else
                            {
                                throw new Exception("BD: " + sErro.ToString());
                            }
                        }

                    }

                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                    lblTituloTabelaSaldo.Text = "Dados da tabela abaixo salvos no sistema";
                }
                else
                {
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                    lblTituloTabelaSaldo.Text = "Dados da tabela abaixo salvos no sistema";
                    gv_TabelaSaldo.Visible = false;
                }

            }
        }
        private bool ValidarDados()
        {
            return true;
        }
    }
}