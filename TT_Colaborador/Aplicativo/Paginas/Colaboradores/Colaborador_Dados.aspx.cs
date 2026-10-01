using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using System.Text;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Data.SqlClient;

namespace TT_Colaborador.Aplicativo.Paginas.Colaboradores
{
    public partial class Colaborador_Dados : Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Area";

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Pesquisar(IDENTITY.Variaveis.idUsuario());

                if (Convert.ToBoolean(Session["AssinaturaSalva"]))
                {
                    Session["AssinaturaSalva"] = false;
                    MensagemPagina.MostraMensagem_Sucesso("Assinatura Salva com sucesso!");
                }

                if (Request["msg"] == "1") MensagemPagina.MostraMensagem_Aviso("É necessário cadastrar uma assinatura!");
            }

            RegistraScript();
        }

        protected void Pesquisar(string idUsuario)
        {
            cmdNovaAssinatura.Visible = true;
            div_AssinaturaVisualizacao.Visible = true;
            div_AssinaturaEdicao.Visible = false;

            try
            {
                LimpaCampos();

                if (idUsuario != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", idUsuario }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidColaborador.Value = RETORNO.DATASET(dsPesquisa, 0, "idColaborador");

                        try
                        {
                            string assinatura = Convert.ToBase64String(dsPesquisa.Tables[0].Rows[0]["vbAssinatura"] as byte[]);

                            if (string.IsNullOrEmpty(assinatura))
                            {
                                cmdNovaAssinatura.Visible = false;
                                div_AssinaturaVisualizacao.Visible = false;
                                div_AssinaturaEdicao.Visible = true;

                                imgAssinatura.Alt = " ";
                            }
                            else imgAssinatura.Src = string.Format("data:image/png;base64,{0}", assinatura);
                        }
                        catch
                        {
                            cmdNovaAssinatura.Visible = false;
                            div_AssinaturaVisualizacao.Visible = false;
                            div_AssinaturaEdicao.Visible = true;

                            imgAssinatura.Alt = " ";
                        }

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                    }
                    else
                        throw new Exception(sErro);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        #endregion

        #region | Utils

        void LimpaCampos()
        {
            hddidColaborador.Value = "0";

            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
        }

        private bool ValidarDados()
        {
            if (string.IsNullOrEmpty(hddAssinatura.Value))
            {
                FUNCOES.Scripts.FocusScript(Page, div_Assinatura.ClientID);
                MensagemPagina.MostraMensagem_Erro("É necessário aplicar a Assinatura no campo de Assinatura!", false);

                return false;
            }

            return true;
        }

        void Salvar_Assinatura()
        {
            if (ValidarDados())
            {
                try
                {
                    string[] vidColaborador = hddidColaborador.Value.Split(',');
                    string idColaborador = vidColaborador[0].ToString();

                    SqlDataAdapter da = new SqlDataAdapter(sProcedure, BD.StringDeConexao);

                    da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;

                    da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "SALVAR_ASSINATURA";
                    da.SelectCommand.Parameters.Add("@idColaborador", SqlDbType.Int).Value = idColaborador;
                    da.SelectCommand.Parameters.Add("@idUsuarioAtualizacao", SqlDbType.Int).Value = IDENTITY.Variaveis.idUsuario();
                    da.SelectCommand.Parameters.Add("@vbAssinatura", SqlDbType.VarBinary).Value = Convert.FromBase64String(hddAssinatura.Value.Split(',')[1]);

                    da.Fill(new DataSet());

                    Session["AssinaturaSalva"] = true;
                    FUNCOES.DirecionaPagina("Aplicativo/Paginas/Colaboradores/Colaborador_Dados.aspx");
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }

            updDetalhe.Update();
        }

        #endregion

        #region | Eventos

        protected void cmdSalvar_Click(object sender, EventArgs e) => Salvar_Assinatura();

        protected void cmdNovaAssinatura_Click(object sender, EventArgs e)
        {
            cmdNovaAssinatura.Visible = false;
            div_AssinaturaVisualizacao.Visible = false;
            div_AssinaturaEdicao.Visible = true;

            updDetalhe.Update();
        }

        protected void cmdCancelar_Click(object sender, EventArgs e) => FUNCOES.DirecionaPagina("Aplicativo/Paginas/Colaboradores/Colaborador_Dados");

        #endregion

        #region | Script 

        void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("$(document).ready(function () {\r\n");

            sb.Append("     var canvas = $('[id*=canvaAssinatura]')[0];\r\n");
            sb.Append("     var context = canvas.getContext('2d');\r\n");
            sb.Append("     var drawing = false;\r\n");
            sb.Append("     context.strokeStyle = '#000';\r\n");
            sb.Append("     context.lineWidth = 2;\r\n\r\n");

            // Função para obter a posição do mouse/touch no canvas
            sb.Append("     function getPosition(e, canvas) {\r\n");
            sb.Append("         var rect = canvas.getBoundingClientRect();\r\n");
            sb.Append("         var scaleX = canvas.width / rect.width;\r\n");
            sb.Append("         var scaleY = canvas.height / rect.height;\r\n");
            sb.Append("         var x = (e.clientX || e.touches[0].clientX) - rect.left;\r\n");
            sb.Append("         var y = (e.clientY || e.touches[0].clientY) - rect.top;\r\n");
            sb.Append("         return { x: x * scaleX, y: y * scaleY };\r\n");
            sb.Append("     }\r\n\r\n");

            // Script para 'desenhar' a assinatura em Desktop
            sb.Append("     $('#canvaAssinatura').mousedown(function (e) {\r\n");
            sb.Append("         drawing = true;\r\n");
            sb.Append("         context.beginPath();\r\n");
            sb.Append("         var pos = getPosition(e, canvas);\r\n");
            sb.Append("         context.moveTo(pos.x, pos.y);\r\n");
            sb.Append("     });\r\n\r\n");

            sb.Append("     $('#canvaAssinatura').mousemove(function (e) {\r\n");
            sb.Append("         if (drawing) {\r\n");
            sb.Append("             var pos = getPosition(e, canvas);\r\n");
            sb.Append("             console.log(pos);\r\n");
            sb.Append("             context.lineTo(pos.x, pos.y);\r\n");
            sb.Append("             context.stroke();\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n\r\n");

            sb.Append("     $('#canvaAssinatura').mouseup(function () {\r\n");
            sb.Append("         drawing = false;\r\n");
            sb.Append("         $('[id*=hddAssinatura]').val(canvas.toDataURL('image/png'));\r\n");
            sb.Append("     });\r\n\r\n");

            sb.Append("     $('#canvaAssinatura').mouseleave(function () {\r\n");
            sb.Append("         drawing = false;\r\n");
            sb.Append("     });\r\n\r\n");

            // Script para 'desenhar' a assinatura em Tela touch
            sb.Append("     $('#canvaAssinatura').on('touchstart', function (e) {\r\n");
            sb.Append("         drawing = true;\r\n");
            sb.Append("         var pos = getPosition(e, canvas);\r\n");
            sb.Append("         context.beginPath();\r\n");
            sb.Append("         context.moveTo(pos.x, pos.y);\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("     });\r\n\r\n");

            sb.Append("     $('#canvaAssinatura').on('touchmove', function (e) {\r\n");
            sb.Append("         if (drawing) {\r\n");
            sb.Append("             var pos = getPosition(e, canvas);\r\n");
            sb.Append("             context.lineTo(pos.x, pos.y);\r\n");
            sb.Append("             context.stroke();\r\n");
            sb.Append("         }\r\n");
            sb.Append("         e.preventDefault();\r\n"); // Impede o scroll da página
            sb.Append("     });\r\n\r\n");

            sb.Append("     $('#canvaAssinatura').on('touchend', function () {\r\n");
            sb.Append("         drawing = false;\r\n");
            sb.Append("         $('[id*=hddAssinatura]').val(canvas.toDataURL('image/png'));\r\n");
            sb.Append("     });\r\n\r\n");

            sb.Append($"    $('#{cmdLimpaAssinatura.ClientID}').click(function (e) {{\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         context.clearRect(0, 0, canvas.width, canvas.height);\r\n");
            sb.Append("         $('[id*=hddAssinatura]').val('');\r\n");
            sb.Append("     });\r\n\r\n");

            sb.Append("});\r\n\r\n");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        #endregion
    }
}