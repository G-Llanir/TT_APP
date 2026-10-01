using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using TT_Hub.FrameWork;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using RETORNO = TT.FrameWork.BD.Retorno;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Hub.Prontoforms
{
    public partial class Clientes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sArquivoEntrada = "";
            try
            {
                

                string sIPSolicitacao = Request.ServerVariables["REMOTE_ADDR"].ToString();
                //Verificando envio via form-data;
                sArquivoEntrada = System.Web.HttpUtility.UrlDecode(Request.Form.ToString());
                //Verificando envio via raw
                sArquivoEntrada = new StreamReader(this.Request.InputStream).ReadToEnd();


                //Inclusão de Função de pegar os dados via RAW (padrão de envio PHP).
                if (sArquivoEntrada != "")
                {
                    
                    
                    //Nome do Arquivo
                    string sNomeArquivo =  "ProtoForms_Clientes" + "_"  + FUNCOES.CarimboDataHora() + ".csv";
                    string sPathSalvarArquivo = HttpContext.Current.Server.MapPath("~/Arquivos/" + sNomeArquivo);

                    //Gravando Arquivo em Disco
                    StreamWriter sArquivograva = new StreamWriter(sPathSalvarArquivo, true);
                    sArquivograva.Write(sArquivoEntrada.ToString());
                    sArquivograva.Close();



                }



            }
            catch (Exception ex)
            {
                sArquivoEntrada = "Erro ao recuperar parametros";
                Response.Write(ex.ToString());

            }



            Response.End();
            Response.Close();

        }

    }
}