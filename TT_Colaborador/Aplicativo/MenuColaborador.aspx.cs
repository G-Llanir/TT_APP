using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using TT_Colaborador.Aplicativo.Controles;

namespace TT_Colaborador
{
    public partial class MenuColaborador : Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Area";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Pesquisar(IDENTITY.Variaveis.idUsuario());
        }

        protected void Pesquisar(string idUsuario)
        {
            try
            {
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
                        var idColaborador = RETORNO.DATASET(dsPesquisa, 0, "idColaborador");
                        PesquisarColaborador(idColaborador);
                        carregaimgColaborador(idColaborador);
                    }
                    else
                    {
                        lblNomeColaborador.InnerText = "USUÁRIO SEM ASSOCIAÇÃO A UM COLABORADOR!";
                        txtsEmail.Visible = false;
                        txtsTelCelular.Visible = false;
                        txtsRamal.Visible = false;
                        txtdtNascimento.Visible = false;
                        lblDepartamento.Visible = false;
                    }

                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        protected void PesquisarColaborador(string idColaborador)
        {
            try
            {
                if (idColaborador != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-DADOS" },
                        { "@idColaborador", idColaborador }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        lblNomeColaborador.InnerText = RETORNO.DATASET(dsPesquisa, 0, "sDscColaborador");
                        txtsEmail.Text = RETORNO.DATASET(dsPesquisa, 0, "sEmail");
                        txtsTelCelular.Text = RETORNO.DATASET(dsPesquisa, 0, "sTelCelular");
                        txtsRamal.Text = RETORNO.DATASET(dsPesquisa, 0, "sRamal");
                        txtdtNascimento.Text = RETORNO.DATASET(dsPesquisa, 0, "dtNascimento");
                        lblDepartamento.InnerText = RETORNO.DATASET(dsPesquisa, 0, "sDscDepartamento");
                        carregaimgColaborador(idColaborador);
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

        protected void carregaimgColaborador(string idObjeto)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_IMAGEM" },
                { "@idTipoArquivo", "40" },
                { "@idObjeto", idObjeto }
            };
            DataTable dsPesquisa = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dsPesquisa.Rows.Count > 0)
            {
                string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])dsPesquisa.Rows[0]["vbArquivo"]);
                imgColaborador.ImageUrl = imgUrl;
                imgColaborador.Visible = true;
            }
        }
    }
}