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
using TT_Hub.App.Paginas.Manutencao;
using static System.Net.Mime.MediaTypeNames;
using System.Net.NetworkInformation;
using TT.FrameWork;
using static TT.FrameWork.BD;
using TT_Flow.App.Paginas;

namespace TT_Flow.App.Paginas
{

    public partial class Kanban : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {


            /*FUNCOES.ValidarSessao();*/ //Essa é uma função para a pagina sistemas que verifica o login do usuario

            FUNCOES.ValidarPermissaoAcesso();

            //!postBak da um refresh e refaz a pesquisa 
            if (!IsPostBack)
            {
                if (Request["idPedido"] != null)
                {

                    PesquisarKanban(Request["idPedido"].ToString());


                }
                else
                {
                    //aqui vai ficar uma menssagem de erro, pedindo um numero de pedido.
                    PesquisarKanban("164");
                    //era 169
                }
            }

        }

        protected void PesquisarKanban(string idPedido)
        {
            //hddidPedido.Value = idPedido;

            string sErro;
            DataSet dsKanban;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "Consultar");
            vParametros.Add("@idPedido", idPedido);

            dsKanban = BD.ExecutarDataSet("sp_Flow_Kanban", vParametros);

            if (BD.ValidarDataSet(dsKanban, out sErro))
            {
                StringBuilder strIcones = new StringBuilder();
                StringBuilder strLinha1 = new StringBuilder();
                StringBuilder strLinha2 = new StringBuilder();
                StringBuilder strLinha3 = new StringBuilder();

                for (int i = 0; i < dsKanban.Tables[0].Rows.Count; i++)
                {
                    
                    //Status
                    strLinha1.AppendLine("<td class=\"statusKanban\">");
                    strLinha1.AppendLine("<span id=\"Item1_" + RETORNO.DATASET(dsKanban, i, "idStatus") + "\">" + RETORNO.DATASET(dsKanban, i, "sDscStatus") + "</span>");
                    strLinha1.AppendLine("</td>");

                    //Icone
                    strLinha2.AppendLine("<td class=\"iconesKanban\">");
                    strLinha2.AppendLine("<img  id=\"image_" + RETORNO.DATASET(dsKanban, i, "idStatus") + "\" class=\"img-responsive\" src=\"" + "data:image/jpg;base64," + Convert.ToBase64String((byte[])dsKanban.Tables[0].Rows[i]["vbArquivo"]) + "\" />");
                    strLinha2.AppendLine("</td>");

                    //Data Conclusao
                    strLinha3.AppendLine("<td class=\"dataKanban\">");
                    strLinha3.AppendLine(string.Format("<span id=\"dtConclusao_{0}\">{1}</span>", RETORNO.DATASET(dsKanban, i, "idStatus"), RETORNO.DATASET(dsKanban, i, "dtConclusao")));
                    strLinha3.AppendLine("</td>");

                }

                //Inicio tabela
                strIcones.AppendLine("<table>");

                strIcones.AppendLine("<tr>");
                strIcones.AppendLine(strLinha1.ToString());
                strIcones.AppendLine("</tr>");

                strIcones.AppendLine("<tr>");
                strIcones.AppendLine(strLinha2.ToString());
                strIcones.AppendLine("</tr>");

                strIcones.AppendLine("<tr>");
                strIcones.AppendLine(strLinha3.ToString());
                strIcones.AppendLine("</tr>");

                //final tabela 
                strIcones.AppendLine("</table>");

                ltrBotoes.Text = strIcones.ToString();
            }
            else
            {
                throw new Exception(sErro);
            }

        }


    }
}
