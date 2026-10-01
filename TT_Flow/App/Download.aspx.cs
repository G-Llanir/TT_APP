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
using System.IO.Compression;

namespace TT_Flow.App
{
    public partial class Download : System.Web.UI.Page
    {
        public static string ID = "";
        public static string sTp = "1";
        public static string idParceiro = "";
        public static string Usuario = "";
        public static string Senha = "";
        public static string idUsuarioSTSO = "";
        public static string sChave = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["id"] != null)
            {
                ID = Request["id"].ToString();
            }
            if (Request["sTp"] != null)
            {
                sTp = Request["sTp"].ToString();
            }
            else
            {
                sTp = "1";
            }
            if (Request["idP"] != null)
            {
                idParceiro = Request["idP"].ToString();
            }
            DIV_Login.Visible = false;
            DIV1.Visible = true;
            DIV2.Visible = false;
            DIV3.Visible = false;
            DIV5.Visible = false;
            btnCadastre.Visible = false;
            Span1.InnerText = "P" + idParceiro + ".";
        }

        protected void btnAcessar_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                DataTable dtArquivo;
                vParametrosItem = new Dictionary<string, string>
                {
                    {"@sFuncao", "Acessar_Download" },
                    {"@sChave", ID},
                    {"@sSenha", txtSenha.Text}
                };

                dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Pedidos", vParametrosItem);

                Senha = txtSenha.Text;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Senha Correta.")
                {
                    Senha = txtSenha.Text;
                    DIV_Login.Visible = false;
                    DIV1.Visible = true;
                    DIV2.Visible = false;
                }
                else
                {
                    Mensagem.MostraMensagem_Erro(ex.Message);
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                DataSet dtArquivo;
                vParametrosItem = new Dictionary<string, string>
                {
                        {"@sFuncao", "CONSULTA PEDIDOS STSO DOWNLOAD" },
                        {"@sChave", ID},
                        {"@idTipo", sTp},
                        {"@sSenha", txtsChave.Text},
                        {"@idCliente", idParceiro},
                        {"@sUsuario", "P" + idParceiro + "." + txtUsuario.Text.Replace("P" + idParceiro + ".", "")},
                        {"@sSenhaCadastro", txtsSenha.Text}
                };

                dtArquivo = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametrosItem);
                string SenhaSTSO = RETORNO.DATASET(dtArquivo, 1, 0, "sSenha");
                string UsuarioSTSO = RETORNO.DATASET(dtArquivo, 1, 0, "sUsuario");
                string EmailSTSO = RETORNO.DATASET(dtArquivo, 1, 0, "sEndereco");
                string NomeSTSO = RETORNO.DATASET(dtArquivo, 1, 0, "sNome");
                string idUsuario = RETORNO.DATASET(dtArquivo, 1, 0, "idUsuario");

                if (EmailSTSO != "")
                {
                    gv_Pedidos.DataSource = dtArquivo.Tables[0];
                    gv_Pedidos.DataBind();
                    Usuario = txtUsuario.Text;
                    Senha = txtsSenha.Text;
                    DIV3.Visible = true;
                    DIV1.Visible = false;
                    DIV5.Visible = false;
                    MensagemPagina4.MostraMensagem("O pedido destacado em azul corresponde ao link de download selecionado.", "info", true);
                }
                else
                {
                    DIV1.Visible = false;
                    DIV2.Visible = false;
                    DIV3.Visible = false;
                    DIV5.Visible = true;
                    Span2.InnerText = Span1.InnerText;
                    var usuario = UsuarioSTSO.Split('.');
                    int totalPartes = usuario.Length;
                    string txtUsuario = "";

                    if (totalPartes > 2)
                    {
                        txtUsuario += usuario[1] + ".";

                        for (int i = 2; i < totalPartes; i++)
                        {
                            if (i == totalPartes - 1)
                            {
                                txtUsuario += usuario[i];
                            }
                            else
                            {
                                txtUsuario += usuario[i] + ".";
                            }
                        }
                    }
                    else
                    {
                        txtUsuario = usuario[1];
                    }
                    sChave = txtsChave.Text;
                    txtsUsuário_AlterarSenha.Text = txtUsuario;
                    txtsSenha_AlterarSenha.Text = SenhaSTSO;
                    txtsEmail_AlterarSenha.Text = EmailSTSO;
                    txtsNome_AlterarSenha.Text = NomeSTSO;
                    idUsuarioSTSO = idUsuario;
                    MensagemPagina3.MostraMensagem_Aviso("No primeiro acesso, altere sua <br>senha e cadastre um e-mail.");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina1.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void Cadastre_Click(object sender, EventArgs e)
        {
            DIV2.Visible = true;
            DIV1.Visible = false;
            DIV_Login.Visible = false;
            LinkButton3.Visible = false;
            lblsPrefixoLogin.InnerText = "P" + idParceiro + ".";
        }

        protected void btnCadastro_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (ValidarCadastro())
            {
                try
                {
                    Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                    DataSet dtArquivo;
                    vParametrosItem = new Dictionary<string, string>
                    {
                    {"@sFuncao", "Cadastrar_STSO" },
                    {"@idEmpresa", idParceiro},
                    {"@sEmail", txtsEmail.Text},
                    {"@sUsuario", "P" + idParceiro + "." + txtsUsuario.Text},
                    {"@sSenha", txtsSenhaCadastro.Text},
                    {"@sNome", txtsNome.Text}
                    };

                    dtArquivo = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametrosItem);

                    DIV2.Visible = false;
                    DIV1.Visible = true;
                    DIV_Login.Visible = false;
                    LinkButton3.Visible = false;
                    MensagemPagina1.MostraMensagem_Sucesso("Cadastro Efetuado com Sucesso!");
                }
                catch (Exception ex)
                {
                    MensagemPagina2.MostraMensagem_Erro(ex.Message);
                }
            }
            else
            {
                DIV2.Visible = true;
                DIV1.Visible = false;
                DIV_Login.Visible = false;
                LinkButton3.Visible = false;
            }
        }

        private bool ValidarCadastro()
        {
            string sMensagem = "";

            if (txtsNome.Text == "")
            {
                sMensagem = "Descreva o Nome Completo!";
            }
            if (txtsUsuario.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o Usuario!";
            }
            if (txtsUsuario.Text == "P" + idParceiro + ".")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o Usuario!";
            }
            if (txtsSenhaCadastro.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva a Senha!";
            }
            if (txtsEmail.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o Email!";
            }

            if (sMensagem != "")
            {
                MensagemPagina2.MostraMensagem_Erro(sMensagem);
                return false;
            }

            return true;
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            DIV2.Visible = true;
            DIV1.Visible = false;
            DIV_Login.Visible = false;
            LinkButton3.Visible = false;
            string sSenha = txtsSenhaCadastro.Text;
            txtsSenhaCadastro.TextMode = TextBoxMode.SingleLine;
            LinkButton1.Visible = false;
            LinkButton3.Visible = true;
            txtsSenhaCadastro.Attributes["value"] = sSenha;
        }

        protected void LinkButton3_Click(object sender, EventArgs e)
        {
            DIV2.Visible = true;
            DIV1.Visible = false;
            DIV_Login.Visible = false;
            LinkButton3.Visible = false;
            string sSenha = txtsSenhaCadastro.Text;
            txtsSenhaCadastro.TextMode = TextBoxMode.Password;
            LinkButton1.Visible = true;
            LinkButton3.Visible = false;
            txtsSenhaCadastro.Attributes["value"] = sSenha;
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            DIV2.Visible = false;
            DIV1.Visible = true;
            DIV_Login.Visible = false;
            LinkButton3.Visible = false;
        }

        protected void gv_Pedidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DateTime data = DateTime.Parse(e.Row.Cells[4].Text);
                e.Row.Cells[4].Text = data.ToString("dd/MM/yyyy");

                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "cor").ToString();
            }
        }

        protected void gv_Pedidos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idPedido = 0;
            if (e.CommandName == "Download")
            {
                idPedido = int.Parse(e.CommandArgument.ToString());

                try
                {
                    Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                    DataTable dtArquivo;
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao", "Download_STSO" },
                        {"@idPedido", idPedido.ToString()},
                        {"@idTipo", sTp},
                        {"@sUsuario", Span1.InnerText + Usuario},
                        {"@sSenhaCadastro", Senha}
                    };

                    dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Pedidos", vParametrosItem);

                    if (dtArquivo.Rows.Count > 0)
                    {
                        string zipFileName = $"Arquivos_{DateTime.Now:yyyyMMddHHmmss}.zip";
                        string zipFilePath = Server.MapPath($"~/Download/{zipFileName}");

                        using (FileStream zipFile = new FileStream(zipFilePath, FileMode.Create))
                        {
                            using (ZipArchive zipArchive = new ZipArchive(zipFile, ZipArchiveMode.Create))
                            {
                                foreach (DataRow item in dtArquivo.Rows)
                                {
                                    string PastaStatus = "";
                                    string sNomeArquivo = item["sNomeArquivo"].ToString().Replace(",", "");

                                    string sDscsArquivo = item["sDscsArquivo"].ToString().Replace(",", "");
                                    if (item["sAprovacao"].ToString() == "S")
                                    {
                                        PastaStatus = "Aprovados";
                                    }
                                    else
                                    {
                                        PastaStatus = "Pendentes";
                                    }
                                    string pasta = sDscsArquivo.Split('-')[0].Trim();

                                    string caminhoCompleto = $"{pasta}/{PastaStatus}/{sNomeArquivo}";

                                    ZipArchiveEntry zipEntry = zipArchive.CreateEntry(caminhoCompleto);

                                    using (Stream entryStream = zipEntry.Open())
                                    {
                                        byte[] arquivoBytes = (byte[])item["vbArquivo"];
                                        entryStream.Write(arquivoBytes, 0, arquivoBytes.Length);
                                    }
                                }
                            }
                        }

                        FUNCOES.DownloadArquivo(Page, zipFileName);
                        DIV_Login.Visible = false;
                        DIV1.Visible = false;
                    }
                    else
                    {
                        Mensagem.MostraMensagem_Erro("Nenhum arquivo encontrado!");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Senha incorreta.")
                    {
                        MensagemPagina1.MostraMensagem_Erro("Senha Incorreta!");
                        DIV_Login.Visible = false;
                        DIV1.Visible = true;
                        txtsSenha.Text = "";
                    }
                    else
                    {
                        MensagemPagina1.MostraMensagem_Erro(ex.Message);
                    }
                }

                DIV3.Visible = true;
                DIV1.Visible = false;
            }
        }

        protected void btnSalvar_AlterarSenha_Click(object sender, EventArgs e)
        {
            if (ValidarAtualizar())
            {
                Dictionary<String, String> vParametrosAtualizar = new Dictionary<string, string>();
                DataSet dtAtualizar;
                vParametrosAtualizar = new Dictionary<string, string>
                    {
                        {"@sFuncao", "Alterar_Usuario_STSO" },
                        {"@sEmailSTSO", txtsEmail_AlterarSenha.Text},
                        {"@sSenhaSTSO", txtsSenha_AlterarSenha.Text},
                        {"@sUsuario", Span2.InnerText + txtsUsuário_AlterarSenha.Text},
                        {"@sNomeSTSO", txtsNome_AlterarSenha.Text},
                        {"@idUsuarioAtualizacao", idUsuarioSTSO},
                    };

                dtAtualizar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametrosAtualizar);

                Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                DataSet dtArquivo;
                vParametrosItem = new Dictionary<string, string>
                {
                    {"@sFuncao", "CONSULTA PEDIDOS STSO DOWNLOAD" },
                    {"@sChave", ID},
                    {"@idTipo", sTp},
                    {"@sSenha", sChave},
                    {"@idCliente", idParceiro},
                    {"@sUsuario", Span2.InnerText + txtsUsuário_AlterarSenha.Text},
                    {"@sSenhaCadastro", txtsSenha_AlterarSenha.Text}
                };

                dtArquivo = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametrosItem);

                gv_Pedidos.DataSource = dtArquivo.Tables[0];
                gv_Pedidos.DataBind();
                Usuario = txtsUsuário_AlterarSenha.Text;
                Senha = txtsSenha_AlterarSenha.Text;
                DIV3.Visible = true;
                DIV1.Visible = false;
                DIV5.Visible = false;
                MensagemPagina4.MostraMensagem("O pedido destacado em azul corresponde ao link de download selecionado.", "info", true);
            }
            else
            {
                DIV1.Visible = false;
                DIV2.Visible = false;
                DIV3.Visible = false;
                DIV5.Visible = true;
            }
        }

        private bool ValidarAtualizar()
        {
            string sMensagem = "";

            if (txtsNome_AlterarSenha.Text == "")
            {
                sMensagem = "Descreva o Nome Completo!";
            }
            if (txtsUsuário_AlterarSenha.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o Usuario!";
            }
            if (Span2.InnerText == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o Usuario!";
            }
            if (txtsSenha_AlterarSenha.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva a Senha!";
            }
            if (txtsEmail_AlterarSenha.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o Email!";
            }

            if (sMensagem != "")
            {
                MensagemPagina3.MostraMensagem_Erro(sMensagem);
                return false;
            }

            return true;
        }
    }
}