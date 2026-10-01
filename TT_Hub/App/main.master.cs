using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Hub;
using Funcoes = TT.FrameWork.Funcoes;

namespace TT_Hub.App
{
    public partial class MainMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Funcoes.ValidarSessao();
        }

        protected void timer_main_Atualizar_Tick(object sender, EventArgs e)
        {
            Funcoes.ValidarSessao();
        }

        protected void lnkDashBoard_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/dashboard.aspx");
            //Funcoes.DirecionaPagina("main.aspx");
        }
        protected void lnkLogoff_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/login.aspx");
        }


        protected void lnkCad_Consulta_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/Paginas/Manutencao/Cliente.aspx");
        }

        protected void lnkCad_Autorizacao_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/Paginas/Manutencao/Equipamentos.aspx");

        }

        protected void lnkRel_MalaDireta_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/Paginas/MalaDireta/MalaDireta.aspx?sFuncao=PADRAO");
        }

        protected void lnkRel_MalaDireta_Aniversario_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/Paginas/MalaDireta/MalaDireta.aspx?sFuncao=ANIVERSARIO");
        }
        protected void lnkRel_MalaDireta_Remetente_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/Paginas/MalaDireta/MalaDireta.aspx?sFuncao=REMETENTE");
        }

        protected void LinkButton5_Click(object sender, EventArgs e)
        {

        }

        protected void LinkButton6_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/Paginas/Painel/Eventos.aspx");
        }

        protected void lnkManut_TipoOficio_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/Paginas/Manutencao/Tipo_Oficio.aspx");
        }

        protected void lnkOficio_Novo_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/Paginas/Painel");
        }



        protected void lnkManut_Categoria_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/Paginas/Manutencao/Categoria.aspx");
        }

        protected void lnk_Usuarios_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/Paginas/Manutencao/Usuarios.aspx");

        }

        protected void lnkOcorrencias_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/Paginas/Consulta/Ocorrencia.aspx");
        }

        protected void lnkMonitoramentoPing_Click(object sender, EventArgs e)
        {
            Funcoes.DirecionaPagina("app/Paginas/Consulta/Monitoramento_Conectividade.aspx");
        }
    }
}