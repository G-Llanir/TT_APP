public class Permissao
{
    public static class Atividades
    {
        public static class Projetos
        {
            public static int Consultar = 500;
            public static int Incluir = 501;
            public static int Alterar = 502;
            public static int AssociarUsuarios = 538;
            public static int Nivel_Gestor = 555;
            public static int Nivel_Diretor = 556;
        }
    }

    public static class Mensagens
    {
        public static int Consultar = 517;
        public static int Incluir = 530;
    }

    public static class Conversas
    {
        public static int Consultar = 519;
        public static int Editar = 531;
        public static int Incluir = 532;
    }

    public static class Benefícios
    {
        public static int Consultar = 521;
    }

    public static class Solicitações
    {
        public static int Consultar = 523;
        public static int Editar = 533;
        public static int Incluir = 534;
    }

    public static class Veiculos
    {
        public static int Consultar = 611;
        public static int Incluir = 612;
        public static int Editar = 613;
    }

    public static class Entrega_de_EPI
    {
        public static int Consultar = 525;
        public static int Editar = 535;
    }

    public static class Relatorio_de_Gastos
    {
        public static int Consultar = 527;
        public static int Editar = 536;
        public static int Incluir = 537;
    }

    public static class TerceirosPJ
    {
        public static int Terceiro = 622;
        public static int Consultar = 623;
        public static int GerarRelatorio = 624;
        public static int GerarAjuste = 625;
        public static int GerarFechamento = 626;
        public static int EditarAjuste = 627;
        public static int COntroleHorasTFlow = 628;
    }
}