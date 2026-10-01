using System;
using System.Web.UI;
using System.Text;
using System.Data;
using System.Collections.Generic;
using TT.FrameWork;
using System.Globalization;
using static TT.FrameWork.Identity;

namespace TT_Flow.App.Controles
{
    public partial class Calendario : UserControl
    {
        #region | Construtores

        private static readonly string sProcedure_Calendario = "sp_Manipula_tbl_Flow_Calendario_Eventos_x_Usuario";

        /// <summary>
        /// Propriedade utilizada para armazenar um identificador único para cada instância da '#div_Calendario', do Controle de Calendário.
        /// </summary>
        private string idUnico_div = string.Empty;

        /// <summary>
        /// Propriedade utilizada para diferenciar em qual contexto ou para qual Tipo está sendo renderizado o Calendário, sendo:
        /// <br />
        /// 0 = Evento Personalizado
        /// <br />
        /// 1 = Pedido
        /// <br />
        /// 2 = Atividade
        /// </summary>
        public int TipoEvento = 0;

        /// <summary>
        /// Propriedade utilizada para diferenciar qual modo de Visualização está sendo renderizado o Calendário, sendo:
        /// <br />
        /// 0 = Padrão
        /// <br />
        /// 1 = Permite visualização detalhada, mas não permite edição
        /// </summary>
        public int TipoVisualizacao = 0;

        /// <summary>
        /// Propriedade utilizada para definir estilos personalizados para o título do Calendário.
        /// <br />
        /// De forma que é possível definir um estilo completo e único ou utilizar um dos modelos prontos, sendo eles:
        /// <br />
        /// <br />
        /// P = Pequeno
        /// <br />
        /// M = Médio
        /// <br />
        /// G = Grande
        /// </summary>
        public string EstilosPersonalizados_Titulo = "M";

        /// <summary>
        /// Propriedade utilizada para definir se os scripts
        /// </summary>
        public bool RegistraScript_Global { get => Convert.ToBoolean(hdd_RegistraScript_Global.Value); set => hdd_RegistraScript_Global.Value = value.ToString().ToLower().Trim(); }

        /// <summary>
        /// Quando true, não registra handlers globais de resize nem dispara resize na janela
        /// (uso em painéis laterais com altura fixa, ex.: Dashboard RRHH).
        /// </summary>
        public bool PainelLateralFixo { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e) => idUnico_div = div_calendario.ClientID;

        #region | Utils

        /// <summary>
        /// Função utilizada para Converter o objeto de Dados recebido em uma string, em formato de array, de Eventos a serem exibidos no Calendário.
        /// </summary>
        /// <param name="dados">Recebe o objeto com os Dados dos Eventos.</param>
        /// <returns>Retorna uma string construída dinâmicamente contendo as informações dos Eventos.</returns>
        protected string ConverteDados_x_Eventos(object dados)
        {
            if (dados is null) return string.Empty;

            StringBuilder sb = new StringBuilder();
            DataTable dt = dados is DataSet ds ? ds.Tables[0] : (DataTable)dados;

            string Esc(string texto) => (texto ?? "").Replace("'", "\\'");

            foreach (DataRow row in dt.Rows)
            {
                int idTipo = row.IsNull("idTipo") ? 0 : row.Field<int>("idTipo");

                if (idTipo == 2)
                {
                    int idAtividade = row.Field<int>("idAtividade");
                    int idProjeto = row.Field<int>("idProjeto");
                    int idStatus = row.Field<int>("idStatus");
                    string sCor = idStatus == 1 ? "gray" : idStatus == 2 ? "skyblue" : idStatus == 3 ? "yellow" : idStatus == 4 ? "red" : idStatus == 5 ? "green" : idStatus == 6 ? "orange" : "";
                    string dtIni = row.IsNull("dtInicial") ? (row.IsNull("dtInicial_Previsao") ? "" : row.Field<DateTime>("dtInicial_Previsao").ToString("yyyy-MM-ddTHH:mm:ss")) : row.Field<DateTime>("dtInicial").ToString("yyyy-MM-ddTHH:mm:ss");
                    string dtFim = row.IsNull("dtFinal") ? (row.IsNull("dtFinal_Previsao") ? "" : row.Field<DateTime>("dtFinal_Previsao").ToString("yyyy-MM-ddTHH:mm:ss")) : row.Field<DateTime>("dtFinal").ToString("yyyy-MM-ddTHH:mm:ss");
                    bool bSemData = string.IsNullOrEmpty(dtIni);

                    if (bSemData) dtIni = "2000-01-01T00:00:00";

                    sb.AppendLine("{");
                    sb.AppendLine($"id: {idAtividade},");
                    sb.AppendLine($"title: '{Esc(row.Field<string>("sDscTitulo"))}',");
                    sb.AppendLine("allDay: false,");
                    sb.AppendLine($"display: '',");
                    sb.AppendLine($"url: '/App/Paginas/Atividades/Projetos_Detalhe.aspx?id={idProjeto}',");
                    sb.AppendLine($"color: '{sCor}',");
                    sb.AppendLine($"textColor: '',");
                    sb.AppendLine($"start: '{dtIni}',");
                    sb.AppendLine($"end: '{dtFim}',");
                    sb.AppendLine($"className: '',");
                    sb.AppendLine("extendedProps: {");
                    sb.AppendLine($"descricao: `{Esc(row.Field<string>("sDscDescricao"))}`,");
                    sb.AppendLine($"idUsuario: '{Esc(row.Field<string>("sUsuarios"))}',");
                    sb.AppendLine("usuarioAtualizacao: '',");
                    sb.AppendLine("dataAtualizacao: '',");
                    sb.AppendLine("ativo: 'S',");
                    sb.AppendLine("extra: '',");
                    sb.AppendLine($"idTipo: {idTipo},");
                    sb.AppendLine("nVlrConclusao: 0,");
                    sb.AppendLine($"dtInicial_Previsao: '{(row.IsNull("dtInicial_Previsao") ? "" : row.Field<DateTime>("dtInicial_Previsao").ToString("yyyy-MM-ddTHH:mm:ss"))}',");
                    sb.AppendLine($"dtFinal_Previsao: '{(row.IsNull("dtFinal_Previsao") ? "" : row.Field<DateTime>("dtFinal_Previsao").ToString("yyyy-MM-ddTHH:mm:ss"))}',");
                    sb.AppendLine($"nHoras_Realizadas: {row.Field<decimal>("nHoras_Realizadas").ToString(CultureInfo.InvariantCulture)},");
                    sb.AppendLine($"nHoras_Previsao: {row.Field<decimal>("nHoras_Previsao").ToString(CultureInfo.InvariantCulture)},");
                    sb.AppendLine($"nHoras_Disponiveis: {row.Field<decimal>("nHoras_Disponiveis").ToString(CultureInfo.InvariantCulture)},");
                    sb.AppendLine($"sUsuarios_NovoApontamento: '{Esc(row.Field<string>("sUsuarios_NovoApontamento"))}',");
                    sb.AppendLine($"sAssociaUsuario: '{(row.Field<string>("sAssociaUsuario") == "S" || Funcoes.ValidaPermissao(Permissao.Atividades.Projetos.AssociarUsuarios, false) ? "" : "disabled=\"disabled\"")}',");
                    sb.AppendLine($"idStatus: {idStatus},");
                    sb.AppendLine($"sDscStatus: '{Esc(row.Field<string>("sDscStatus"))}',");
                    sb.AppendLine($"dtInicial: '{(row.IsNull("dtInicial") ? "" : row.Field<DateTime>("dtInicial").ToString("yyyy-MM-ddTHH:mm:ss"))}',");
                    sb.AppendLine($"dtFinal: '{(row.IsNull("dtFinal") ? "" : row.Field<DateTime>("dtFinal").ToString("yyyy-MM-ddTHH:mm:ss"))}',");
                    sb.AppendLine($"semData: {bSemData.ToString().ToLower()}");
                    sb.AppendLine("}");
                    sb.AppendLine("},");
                }
                else
                {
                    int idEvento = row.Field<int>("idEvento");
                    string allDay = row.Field<string>("sDiaInteiro") == "S" ? "true" : "false";
                    string dtInicial = row.Field<DateTime>("dtInicial").ToString("yyyy-MM-ddTHH:mm:ss");
                    string dtFinal = row.IsNull("dtFinal") ? "" : row.Field<DateTime>("dtFinal").ToString("yyyy-MM-ddTHH:mm:ss");
                    string classe = row.Field<string>("sAtivo") == "N" ? "eventoInativo" : "";

                    sb.AppendLine("{");
                    sb.AppendLine($"id: {idEvento},");
                    sb.AppendLine($"title: '{Esc(row.Field<string>("sDscTitulo"))}',");
                    sb.AppendLine($"allDay: {allDay},");
                    sb.AppendLine($"display: '{Esc(row.Field<string>("sDisplay"))}',");
                    sb.AppendLine($"url: `{Esc(row.Field<string>("sUrl"))}` ,");
                    sb.AppendLine($"color: '{row.Field<string>("sCor") ?? ""}',");
                    sb.AppendLine($"textColor: '{row.Field<string>("sCorTexto") ?? ""}',");
                    sb.AppendLine($"start: '{dtInicial}',");
                    sb.AppendLine($"end: '{dtFinal}',");
                    sb.AppendLine($"className: '{classe}',");
                    sb.AppendLine("extendedProps: {");
                    sb.AppendLine($"descricao: `{Esc(row.Field<string>("sDscEvento"))}`,");
                    sb.AppendLine($"idUsuario: '{row.Field<int>("idUsuario")}',");
                    sb.AppendLine($"usuarioAtualizacao: '{Esc(row.Field<string>("sDscUsuarioAtualizacao"))}',");
                    sb.AppendLine($"dataAtualizacao: '{row.Field<DateTime>("dtAtualizacao").ToString("yyyy-MM-ddTHH:mm:ss")}',");
                    sb.AppendLine($"ativo: '{row.Field<string>("sAtivo")}',");
                    sb.AppendLine($"extra: `{Esc(row.Field<string>("sDscExtra"))}`,");
                    sb.AppendLine($"idTipo: {idTipo},");
                    sb.AppendLine($"nVlrConclusao: {row.Field<int>("nVlrConclusao")},");
                    sb.AppendLine("dtInicial_Previsao: '',");
                    sb.AppendLine("dtFinal_Previsao: '',");
                    sb.AppendLine("nHoras_Realizadas: 0,");
                    sb.AppendLine("nHoras_Previsao: 0,");
                    sb.AppendLine("nHoras_Disponiveis: 0,");
                    sb.AppendLine("sUsuarios_NovoApontamento: '',");
                    sb.AppendLine("sAssociaUsuario: '',");
                    sb.AppendLine("idStatus: 0,");
                    sb.AppendLine("sDscStatus: '',");
                    sb.AppendLine("dtInicial: '',");
                    sb.AppendLine("dtFinal: '',");
                    sb.AppendLine("semData: false");
                    sb.AppendLine("}");
                    sb.AppendLine("},");
                }
            }

            if (sb.Length > 0 && sb[sb.Length - 3] == ',')
                sb.Length -= 3;

            return sb.ToString();
        }

        /// <summary>
        /// Função utilizada para consultar para quais Usuários, o Usuário Logado possui permissão para registrar eventos.
        /// </summary>
        /// <returns>Retorna uma string construída dinâmicamente contendo as informações dos Usuários.</returns>
        protected string PopulaUsuarios(bool bAtividades)
        {
            StringBuilder sUsuarios = new StringBuilder();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_USUARIOS" },
                { "@idUsuario", Variaveis.idUsuario() }
            };
            DataTable dt = BD.ExecutarDataTable(sProcedure_Calendario, vParametros);

            if (dt.Rows.Count > 0)
            {
                sUsuarios.Append("<div class=\"col-lg-8 form-group\" style=\"text-align: start;\">\r\n");
                sUsuarios.Append($"      <label for=\"usuario_Evento\">Colaborador</label>\r\n");
                sUsuarios.Append($"      <select id=\"usuario_Evento\" class=\"form-control\">\r\n");

                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        sUsuarios.AppendFormat("<option value=\"{0}\">{1}</option>\r\n", row.Field<int>("idUsuario"), row.Field<string>("sDscUsuario"));
                    }
                    catch { }
                }

                sUsuarios.Append("      </select>\r\n");
                sUsuarios.Append("</div>\r\n");
            }

            return sUsuarios.ToString();
        }

        /// <summary>
        /// Função utilizada para consultar os Eventos do Usuário Logado.
        /// </summary>
        /// <returns>Retorna um DataSet com os dados dos Eventos do Usuário.</returns>
        public DataSet RetornaEventos_x_Usuario()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idUsuario", Variaveis.idUsuario() }
            };
            return BD.ExecutarDataSet(sProcedure_Calendario, vParametros);
        }

        #endregion

        #region | Scripts

        public void RegistraScript() => RegistraScript(RetornaEventos_x_Usuario());

        public void RegistraScript(object dados) => RegistraScript(dados, "dayGridMonth", "listWeek multiMonthYear,dayGridMonth,timeGridWeek,timeGridDay addEventButton", "title", "prevYear,prev,today,next,nextYear", true, "pt-br",
                            "Intl.DateTimeFormat().resolvedOptions().timeZone", "new Date().toISOString().split('T')[0]", false, true, true, false, true,
                            "[\r\n{\r\ndaysOfWeek: [ 1, 2, 3, 4 ],\r\nstartTime: '08:00',\r\nendTime: '18:00',\r\ncolor: '#ff9f89',\r\ntextColor: '#000000'\r\n},\r\n{\r\ndaysOfWeek: [ 5 ],\r\nstartTime: '08:00',\r\nendTime: '17:00',\r\ncolor: '#ff9f89',\r\ntextColor: '#000000'\r\n}\r\n]",
                            "today: 'Hoje',\r\nyear: 'Ano',\r\nmonth: 'Mês',\r\nweek: 'Semana',\r\nday: 'Dia',\r\nlistWeek: 'Lista Semanal'\r\n", false, false);

        public void RegistraScript_Dashboard_View() => RegistraScript_Dashboard_View(RetornaEventos_x_Usuario());

        public void RegistraScript_Dashboard_View(object dados) => RegistraScript(dados, "listDay", "", "title", "prevWeek,prev,today,next,nextWeek", false, "pt-br",
                            "Intl.DateTimeFormat().resolvedOptions().timeZone", "new Date().toISOString().split('T')[0]", false, false, true, false, true,
                            "[\r\n{\r\ndaysOfWeek: [ 1, 2, 3, 4 ],\r\nstartTime: '08:00',\r\nendTime: '18:00',\r\ncolor: '#ff9f89',\r\ntextColor: '#000000'\r\n},\r\n{\r\ndaysOfWeek: [ 5 ],\r\nstartTime: '08:00',\r\nendTime: '17:00',\r\ncolor: '#ff9f89',\r\ntextColor: '#000000'\r\n}\r\n]",
                            "today: 'Hoje',\r\nyear: 'Ano',\r\nmonth: 'Mês',\r\nweek: 'Semana',\r\nday: 'Dia',\r\nlist: 'Lista Diária'\r\n", false, false);

        public void RegistraScript_Dashboard_View(object dados, bool bEdita) => RegistraScript(dados, "listDay", "", "title", "prevWeek,prev,today,next,nextWeek", false, "pt-br",
                           "Intl.DateTimeFormat().resolvedOptions().timeZone", "new Date().toISOString().split('T')[0]", false, false, bEdita, false, true,
                           "[\r\n{\r\ndaysOfWeek: [ 1, 2, 3, 4 ],\r\nstartTime: '08:00',\r\nendTime: '18:00',\r\ncolor: '#ff9f89',\r\ntextColor: '#000000'\r\n},\r\n{\r\ndaysOfWeek: [ 5 ],\r\nstartTime: '08:00',\r\nendTime: '17:00',\r\ncolor: '#ff9f89',\r\ntextColor: '#000000'\r\n}\r\n]",
                           "today: 'Hoje',\r\nyear: 'Ano',\r\nmonth: 'Mês',\r\nweek: 'Semana',\r\nday: 'Dia',\r\nlist: 'Lista Diária'\r\n", false, false);

        public void RegistraScriptViwer(object dados) => RegistraScript(dados, "dayGridMonth", "listWeek multiMonthYear,dayGridMonth,timeGridWeek,timeGridDay", "title", "prevYear,prev,today,next,nextYear", true, "pt-br",
                            "Intl.DateTimeFormat().resolvedOptions().timeZone", "new Date().toISOString().split('T')[0]", false, true, true, false, true,
                            "[\r\n{\r\ndaysOfWeek: [ 1, 2, 3, 4 ],\r\nstartTime: '08:00',\r\nendTime: '18:00',\r\ncolor: '#ff9f89',\r\ntextColor: '#000000'\r\n},\r\n{\r\ndaysOfWeek: [ 5 ],\r\nstartTime: '08:00',\r\nendTime: '17:00',\r\ncolor: '#ff9f89',\r\ntextColor: '#000000'\r\n}\r\n]",
                            "today: 'Hoje',\r\nyear: 'Ano',\r\nmonth: 'Mês',\r\nweek: 'Semana',\r\nday: 'Dia',\r\nlistWeek: 'Lista Semanal'\r\n", false, true);

        public void RegistraScript_Atividades(object dados) => RegistraScript(dados, "dayGridMonth", "listWeek multiMonthYear,dayGridMonth,timeGridWeek,timeGridDay", "title", "prevYear,prev,today,next,nextYear", false, "pt-br",
                            "Intl.DateTimeFormat().resolvedOptions().timeZone", "new Date().toISOString().split('T')[0]", false, true, true, false, true,
                            "[\r\n{\r\ndaysOfWeek: [ 1, 2, 3, 4 ],\r\nstartTime: '08:00',\r\nendTime: '18:00',\r\ncolor: '#ff9f89',\r\ntextColor: '#000000'\r\n},\r\n{\r\ndaysOfWeek: [ 5 ],\r\nstartTime: '08:00',\r\nendTime: '17:00',\r\ncolor: '#ff9f89',\r\ntextColor: '#000000'\r\n}\r\n]",
                            "today: 'Hoje',\r\nyear: 'Ano',\r\nmonth: 'Mês',\r\nweek: 'Semana',\r\nlistWeek: 'Lista Semanal'\r\n", false, false);

        /// <summary>
        /// Este é o método Principal que Renderiza o Calendário, este recebe vários parâmetros com o intuito de deixar o Calendário o mais flexível possível.<br />
        /// Caso não tenha a certeza de qual valor passar em algum parâmetro, por favor consulte as anotções presentes neste método para cada parâmetro, ou utilize uma das versões 'prontas' que já incluem todas ou parte das configurações possíveis para o Calendário.<br />
        /// Demais dúvidas, segue documentação oficial ---> https://fullcalendar.io/docs.
        /// </summary>
        /// <param name="dados">Recebe o objeto com os dados 'brutos', como por exemplo um DataSet vindo do banco de dados com os Eventos do Usuário em questão.</param>
        /// <param name="sVisualizacaoInicial">Recebe o valor que define qual tipo de visualização será exibida quando o Calendário for carregado inicialmente.</param>
        /// <param name="sHeaderToolbar_Left">Recebe o valor que representa quais botões aparecerão do <b>lado esquerdo</b> da Toolbar (Barra de Ferramentas) superior.</param>
        /// <param name="sHeaderToolbar_Center">Recebe o valor que representa quais botões aparecerão na <b>parte central</b> da Toolbar (Barra de Ferramentas) superior.</param>
        /// <param name="sHeaderToolbar_Right">Recebe o valor que representa quais botões aparecerão do <b>lado direito</b> da Toolbar (Barra de Ferramentas) superior.</param>
        /// <param name="bNovoEvento">Recebe o valor que define se será incluso o botão para registrar um Novo Evento.</param>
        /// <param name="sLocal">Recebe o valor que define qual o Local será utilizado de base como a língua padrão, para a maior parte dos textos no Calendário.</param>
        /// <param name="sFusoHorario">Recebe o valor do Fuso Horário a ser seguido no Calendário.</param>
        /// <param name="sDataInicial">Recebe o valor da Data Inicial onde o Calendário será renderizado.</param>
        /// <param name="bIndicadorAgora">Recebe o valor que define se aparecerá um indicador de 'agora'.</param>
        /// <param name="bNavLinks">Recebe o valor que define se as Datas presentes no Calendário podem ser utilizadas como botões de navegação.</param>
        /// <param name="bEdita">Recebe o valor que define se será permitido Editar os Eventos no Calendário.</param>
        /// <param name="bSeleciona">Recebe o valor que define se será permitido Selecionar os Eventos no Calendário.</param>
        /// <param name="bMaxEventos_x_Dia">Recebe o valor que defina se aparecerá um pop-up para exibir os Eventos, no caso de ultrapassar a quantidade de Eventos em um mesmo dia.</param>
        /// <param name="sHorarioTrabalho">Recebe a lista com o Horário de Trabalho/Horário Comercial.</param>
        /// <param name="sTextosBotoes">Recebe a lista com os textos personalizados para os botões da Toolbar (Barra de Ferramentas) superior.</param>
        /// <param name="bArrasta_Solta">Recebe o valor que define se será registrado o evento para Arrastar e Soltar os Eventos.</param>
        /// <param name="bViewPedidos">Recebe o valor que define se é a visualização da Página de Pedidos.</param>
        public void RegistraScript(object dados, string sVisualizacaoInicial, string sHeaderToolbar_Left, string sHeaderToolbar_Center, string sHeaderToolbar_Right, bool bNovoEvento, string sLocal, string sFusoHorario, string sDataInicial,
            bool bIndicadorAgora, bool bNavLinks, bool bEdita, bool bSeleciona, bool bMaxEventos_x_Dia, string sHorarioTrabalho, string sTextosBotoes, bool bArrasta_Solta, bool bViewPedidos)
        {
            idUnico_div = div_calendario.ClientID;

            StringBuilder sb = new StringBuilder();

            sb.Append("\r\n\r\n");
            sb.Append("(function () {\r\n");
            sb.Append("     const Toast = Swal.mixin({\r\n");
            sb.Append("         toast: true,\r\n");
            sb.Append("         position: 'center',\r\n");
            sb.Append("         iconColor: 'white',\r\n");
            sb.Append("         customClass: {\r\n");
            sb.Append("             popup: 'custom_toast'\r\n");
            sb.Append("         },\r\n");
            sb.Append("         showConfirmButton: false,\r\n");
            sb.Append("         timer: 2000,\r\n");
            sb.Append("         timerProgressBar: true,\r\n");
            sb.Append("         didOpen: (toast) => {\r\n");
            sb.Append("             toast.onmouseenter = Swal.stopTimer,\r\n");
            sb.Append("             toast.onmouseleave = Swal.resumeTimer\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n");
            sb.Append("     const ErrorToast = Swal.mixin({\r\n");
            sb.Append("         toast: true,\r\n");
            sb.Append("         position: 'center',\r\n");
            sb.Append("         iconColor: 'white',\r\n");
            sb.Append("         customClass: {\r\n");
            sb.Append("             popup: 'custom_toast'\r\n");
            sb.Append("         },\r\n");
            sb.Append("         showConfirmButton: true\r\n");
            sb.Append("     });\r\n");
            sb.Append($"     var div_calendario = document.getElementById('{idUnico_div}');\r\n");
            sb.Append("     if (div_calendario._calendar) {\r\n");
            sb.Append("         div_calendario._calendar.destroy();\r\n");
            sb.Append("     }\r\n");
            sb.Append("     var calendario = new FullCalendar.Calendar(div_calendario, {\r\n");
            sb.Append("         initialView: '" + sVisualizacaoInicial + "',\r\n");
            sb.Append("         headerToolbar: {\r\n");
            sb.Append("             left: '" + sHeaderToolbar_Left + "',\r\n");
            sb.Append("             center: '" + sHeaderToolbar_Center + "',\r\n");
            sb.Append("             right: '" + sHeaderToolbar_Right + "'\r\n");
            sb.Append("         },\r\n");
            sb.Append("         customButtons: {\r\n");
            sb.Append("             prevWeek: {\r\n");
            sb.Append("                 text: '',\r\n");
            sb.Append("                 icon: 'fc-icon fc-icon-chevrons-left',\r\n");
            sb.Append("                 click: function() {\r\n");
            sb.Append("                     calendario.incrementDate({ weeks: -1 });\r\n");
            sb.Append("                 }\r\n");
            sb.Append("             },\r\n");
            sb.Append("             nextWeek: {\r\n");
            sb.Append("                 text: '',\r\n");
            sb.Append("                 icon: 'fc-icon fc-icon-chevrons-right',\r\n");
            sb.Append("                 click: function() {\r\n");
            sb.Append("                     calendario.incrementDate({ weeks: 1 });\r\n");
            sb.Append("                 }\r\n");
            sb.Append("             },\r\n");
            sb.Append("             prevMonth: {\r\n");
            sb.Append("                 text: '',\r\n");
            sb.Append("                 icon: 'fc-icon fc-icon-chevrons-left',\r\n");
            sb.Append("                 click: function() {\r\n");
            sb.Append("                     calendario.incrementDate({ months: -1 });\r\n");
            sb.Append("                 }\r\n");
            sb.Append("             },\r\n");
            sb.Append("             nextMonth: {\r\n");
            sb.Append("                 text: '',\r\n");
            sb.Append("                 icon: 'fc-icon fc-icon-chevrons-right',\r\n");
            sb.Append("                 click: function() {\r\n");
            sb.Append("                     calendario.incrementDate({ months: 1 });\r\n");
            sb.Append("                 }\r\n");
            sb.Append("             }\r\n");

            if (bNovoEvento)
            {
                sb.Append("             , addEventButton: {\r\n");
                sb.Append("                 text: 'Novo Evento',\r\n");
                sb.Append("                 click: function() {\r\n");
                sb.Append("                     Swal.fire({\r\n");
                sb.Append("                         title: 'Novo Evento',\r\n");
                sb.Append("                         html: `\r\n");
                sb.Append("                             <hr style=\"margin: -5px 0 25px 0; border-color: #bbb;\">\r\n");
                sb.Append("                             <div class=\"col-lg-12 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                                 <label for=\"tituloEvento\">Título</label>\r\n");
                sb.Append("                                 <input id=\"tituloEvento\" class=\"form-control\" />\r\n");
                sb.Append("                             </div>\r\n");
                sb.Append("                             <div class=\"col-lg-12\" style=\"padding: 0; text-align: start;\">\r\n");
                sb.Append("                                 <div class=\"col-lg-2 form-group div_diaInteiroSwitch\" style=\"text-align: start;\">\r\n");
                sb.Append("                                     <label for=\"diaInteiroSwitch\" style=\"width: 100%;\">Dia inteiro?</label>\r\n");
                sb.Append("                                     <label class=\"diaInteiroSwitch switchPersonalizado\">\r\n");
                sb.Append("                                         <input id=\"diaInteiroSwitch\" type=\"checkbox\">\r\n");
                sb.Append("                                         <span class=\"spanSwitch spanSwitch_novo\"></span>\r\n");
                sb.Append("                                     </label>\r\n");
                sb.Append("                                 </div>\r\n");
                sb.Append("                                 <div class=\"col-lg-4 form-group div_diaInteiro hide\" style=\"text-align: start;\">\r\n");
                sb.Append("                                     <label for=\"dataEvento\">Data do Evento</label>\r\n");
                sb.Append("                                     <input id=\"dataEvento\" type=\"date\" class=\"form-control\" />\r\n");
                sb.Append("                                 </div>\r\n");
                sb.Append("                                 <div class=\"col-lg-9 div_datas show\" style=\"padding: 0; text-align: start;\">\r\n");
                sb.Append("                                     <div class=\"col-lg-6 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                                         <label for=\"dataInicio_Evento\">Data inicial do Evento</label>\r\n");
                sb.Append("                                         <input id=\"dataInicio_Evento\" type=\"datetime-local\" class=\"form-control\" />\r\n");
                sb.Append("                                     </div>\r\n");
                sb.Append("                                     <div class=\"col-lg-6\" style=\"text-align: start;\">\r\n");
                sb.Append("                                         <label for=\"dataFinal_Evento\">Data final do Evento</label>\r\n");
                sb.Append("                                         <input id=\"dataFinal_Evento\" type=\"datetime-local\" class=\"form-control\" />\r\n");
                sb.Append("                                     </div>\r\n");
                sb.Append("                                 </div>\r\n");
                sb.Append("                             </div>\r\n");
                sb.Append("                             " + PopulaUsuarios(false) + "\r\n");
                sb.Append("                             <div class=\"col-lg-12 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                                 <label for=\"url_Evento\">Link do Evento</label>\r\n");
                sb.Append("                                 <textarea id=\"url_Evento\" class=\"form-control\" style=\"height: 75px;\"></textarea>\r\n");
                sb.Append("                             </div>\r\n");
                sb.Append("                             <div class=\"col-lg-12 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                                 <label for=\"descricao_Evento\">Descrição</label>\r\n");
                sb.Append("                                 <textarea id=\"descricao_Evento\" class=\"form-control\" style=\"height: 100px;\"></textarea>\r\n");
                sb.Append("                             </div>\r\n");
                sb.Append("                         `,\r\n");
                sb.Append("                         focusConfirm: false,\r\n");
                sb.Append("                         showCancelButton: true,\r\n");
                sb.Append("                         confirmButtonText: 'Salvar',\r\n");
                sb.Append("                         confirmButtonColor: '#5cb85c',\r\n");
                sb.Append("                         cancelButtonText: 'Cancelar',\r\n");
                sb.Append("                         cancelButtonColor: '#d9534f',\r\n");
                sb.Append("                         didOpen: () => {\r\n");
                sb.Append("                             setTimeout(() => {\r\n");
                sb.Append("                                 $('.swal2-modal').addClass('modalSwal_Personalizado grande');\r\n");
                sb.Append("                             }, 10);\r\n");
                sb.Append("                         },\r\n");
                sb.Append("                         preConfirm: () => {\r\n");
                sb.Append("                             const titulo = document.getElementById('tituloEvento').value;\r\n");
                sb.Append("                             const diaInteiro = document.getElementById('diaInteiroSwitch').checked;\r\n");
                sb.Append("                             const url = document.getElementById('url_Evento').value;\r\n");
                sb.Append("                             const data = diaInteiro ? document.getElementById('dataEvento').value : null;\r\n");
                sb.Append("                             const dataInicial = !diaInteiro ? document.getElementById('dataInicio_Evento').value : null;\r\n");
                sb.Append("                             const dataFinal = !diaInteiro ? document.getElementById('dataFinal_Evento').value : null;\r\n");
                sb.Append("                             const descricao = document.getElementById('descricao_Evento').value;\r\n");
                sb.Append("                             const ddlUsuario = document.getElementById('usuario_Evento');\r\n\r\n");
                sb.Append("                             const idUsuario = ddlUsuario ? ddlUsuario.options[ddlUsuario.selectedIndex].value : 0;\r\n");
                sb.Append("                             if (diaInteiro && (!titulo || !data)) {\r\n");
                sb.Append("                                 Swal.showValidationMessage('É necessário preencher tanto o campo do Título quanto o campo de Data!');\r\n");
                sb.Append("                                 return false;\r\n");
                sb.Append("                             } else if (!diaInteiro && (!titulo || !dataInicial || !dataFinal)) {\r\n");
                sb.Append("                                 Swal.showValidationMessage('É necessário preencher tanto o campo do Título quanto os campos de Data inicial e final!');\r\n");
                sb.Append("                                 return false;\r\n");
                sb.Append("                             } else if (!descricao || !idUsuario) {\r\n");
                sb.Append("                                 Swal.showValidationMessage('É necessário preencher o campo da Descrição e selecionar um Usuário!');\r\n");
                sb.Append("                                 return false;\r\n");
                sb.Append("                             }\r\n");
                sb.Append("                             return { titulo, url, data, dataInicial, dataFinal, descricao, idUsuario };\r\n");
                sb.Append("                         }\r\n");
                sb.Append("                     }).then((resultado) => {\r\n");
                sb.Append("                         if (resultado.isConfirmed) {\r\n");
                sb.Append("                             const { titulo, url, data, dataInicial, dataFinal, descricao, idUsuario } = resultado.value;\r\n");
                sb.Append("                             const dataEvento = data ? new Date(data) : null;\r\n");
                sb.Append("                             const dataInicial_Evento = new Date(dataInicial);\r\n");
                sb.Append("                             const dataFinal_Evento = new Date(dataFinal);\r\n");
                sb.Append("                             if (dataEvento && !isNaN(dataEvento.valueOf())) {\r\n");
                sb.Append("                                 $v192.ajax({\r\n");
                sb.Append("                                     url:'/API/Pagina_Ajax.aspx/SalvarEvento_Calendario',\r\n");
                sb.Append("                                     data: JSON.stringify({\r\n");
                sb.Append("                                         'idEvento': 0,\r\n");
                sb.Append("                                         'idUsuario': idUsuario,\r\n");
                sb.Append("                                         'sDscTitulo': titulo,\r\n");
                sb.Append("                                         'sDscEvento': descricao,\r\n");
                sb.Append("                                         'sDiaInteiro': 'S',\r\n");
                sb.Append("                                         'sUrl': url,\r\n");
                sb.Append("                                         'dtInicial': dataEvento,\r\n");
                sb.Append("                                         'dtFinal': null,\r\n");
                sb.Append("                                         'sAtivo': 'S'\r\n");
                sb.Append("                                     }),\r\n");
                sb.Append("                                     dataType: \"json\",\r\n");
                sb.Append("                                     type: \"POST\",\r\n");
                sb.Append("                                     contentType: \"application/json; charset=utf-8\",\r\n");
                sb.Append("                                     success: function(response) {\r\n");
                sb.Append("                                         if (response) {\r\n");
                sb.Append("                                             response = JSON.parse(response.d);\r\n");
                sb.Append("                                             if (response.retorno == 0) {\r\n");
                sb.Append("                                                 calendario.addEvent({\r\n");
                sb.Append("                                                     id: response.id,\r\n");
                sb.Append("                                                     title: titulo,\r\n");
                sb.Append("                                                     start: dataEvento,\r\n");
                sb.Append("                                                     allDay: true,\r\n");
                sb.Append("                                                     display: '',\r\n");
                sb.Append("                                                     url: url,\r\n");
                sb.Append("                                                     color: '',\r\n");
                sb.Append("                                                     textColor: '',\r\n");
                sb.Append("                                                     className: '',\r\n");
                sb.Append("                                                     extendedProps: {\r\n");
                sb.Append("                                                         descricao: descricao,\r\n");
                sb.Append("                                                         idUsuario: idUsuario,\r\n");
                sb.Append("                                                         usuarioAtualizacao: response.usuario,\r\n");
                sb.Append("                                                         dataAtualizacao: new Date(response.data),\r\n");
                sb.Append("                                                         ativo: 'S',\r\n");
                sb.Append("                                                         extra: '',\r\n");
                sb.Append("                                                         idTipo: 0,\r\n");
                sb.Append("                                                         nVlrConclusao: 0\r\n");
                sb.Append("                                                     }\r\n");
                sb.Append("                                                 });\r\n");
                sb.Append("                                                 Toast.fire({\r\n");
                sb.Append("                                                     title: 'Salvo! ' + response.msg,\r\n");
                sb.Append("                                                     icon: 'success'\r\n");
                sb.Append("                                                 });\r\n");
                sb.Append("                                             } else {\r\n");
                sb.Append("                                                 ErrorToast.fire({\r\n");
                sb.Append("                                                     title: 'Erro! ' + response.msg,\r\n");
                sb.Append("                                                     icon: 'error'\r\n");
                sb.Append("                                                 });\r\n");
                sb.Append("                                             }\r\n");
                sb.Append("                                         }\r\n");
                sb.Append("                                     },\r\n");
                sb.Append("                                     error: function(response) {\r\n");
                sb.Append("                                         ErrorToast.fire({\r\n");
                sb.Append("                                             title: 'Erro! ' + response.msg,\r\n");
                sb.Append("                                             icon: 'error'\r\n");
                sb.Append("                                         });\r\n");
                sb.Append("                                     }\r\n");
                sb.Append("                                 });\r\n");
                sb.Append("                             } else if (dataInicial_Evento && dataFinal_Evento && !isNaN(dataInicial_Evento.valueOf()) && !isNaN(dataFinal_Evento.valueOf())) {\r\n");
                sb.Append("                                 $v192.ajax({\r\n");
                sb.Append("                                     url:'/API/Pagina_Ajax.aspx/SalvarEvento_Calendario',\r\n");
                sb.Append("                                     data: JSON.stringify({\r\n");
                sb.Append("                                         'idEvento': 0,\r\n");
                sb.Append("                                         'idUsuario': idUsuario,\r\n");
                sb.Append("                                         'sDscTitulo': titulo,\r\n");
                sb.Append("                                         'sDscEvento': descricao,\r\n");
                sb.Append("                                         'sDiaInteiro': 'N',\r\n");
                sb.Append("                                         'sUrl': url,\r\n");
                sb.Append("                                         'dtInicial': dataInicial_Evento,\r\n");
                sb.Append("                                         'dtFinal': dataFinal_Evento,\r\n");
                sb.Append("                                         'sAtivo': 'S'\r\n");
                sb.Append("                                     }),\r\n");
                sb.Append("                                     dataType: \"json\",\r\n");
                sb.Append("                                     type: \"POST\",\r\n");
                sb.Append("                                     contentType: \"application/json; charset=utf-8\",\r\n");
                sb.Append("                                     success: function(response) {\r\n");
                sb.Append("                                         if (response) {\r\n");
                sb.Append("                                             response = JSON.parse(response.d);\r\n");
                sb.Append("                                             dataInicial_Evento.setHours(dataInicial_Evento.getHours() - 3);\r\n");
                sb.Append("                                             dataFinal_Evento.setHours(dataFinal_Evento.getHours() - 3);\r\n");
                sb.Append("                                             if (response.retorno == 0) {\r\n");
                sb.Append("                                                 calendario.addEvent({\r\n");
                sb.Append("                                                     id: response.id,\r\n");
                sb.Append("                                                     title: titulo,\r\n");
                sb.Append("                                                     start: dataInicial_Evento.toISOString(),\r\n");
                sb.Append("                                                     end: dataFinal_Evento.toISOString(),\r\n");
                sb.Append("                                                     allDay: false,\r\n");
                sb.Append("                                                     display: '',\r\n");
                sb.Append("                                                     url: url,\r\n");
                sb.Append("                                                     color: '',\r\n");
                sb.Append("                                                     textColor: '',\r\n");
                sb.Append("                                                     className: '',\r\n");
                sb.Append("                                                     extendedProps: {\r\n");
                sb.Append("                                                         descricao: descricao,\r\n");
                sb.Append("                                                         idUsuario: idUsuario,\r\n");
                sb.Append("                                                         usuarioAtualizacao: response.usuario,\r\n");
                sb.Append("                                                         dataAtualizacao: new Date(response.data),\r\n");
                sb.Append("                                                         ativo: 'S',\r\n");
                sb.Append("                                                         extra: '',\r\n");
                sb.Append("                                                         idTipo: 0,\r\n");
                sb.Append("                                                         nVlrConclusao: 0\r\n");
                sb.Append("                                                     }\r\n");
                sb.Append("                                                 });\r\n");
                sb.Append("                                                 Toast.fire({\r\n");
                sb.Append("                                                     title: 'Salvo! ' + response.msg,\r\n");
                sb.Append("                                                     icon: 'success'\r\n");
                sb.Append("                                                 });\r\n");
                sb.Append("                                             } else {\r\n");
                sb.Append("                                                 ErrorToast.fire({\r\n");
                sb.Append("                                                     title: 'Erro! ' + response.msg,\r\n");
                sb.Append("                                                     icon: 'error'\r\n");
                sb.Append("                                                 });\r\n");
                sb.Append("                                             }\r\n");
                sb.Append("                                         }\r\n");
                sb.Append("                                     },\r\n");
                sb.Append("                                     error: function(response) {\r\n");
                sb.Append("                                         ErrorToast.fire({\r\n");
                sb.Append("                                             title: 'Erro! ' + response.msg,\r\n");
                sb.Append("                                             icon: 'error'\r\n");
                sb.Append("                                         });\r\n");
                sb.Append("                                     }\r\n");
                sb.Append("                                 });\r\n");
                sb.Append("                             } else {\r\n");
                sb.Append("                                 ErrorToast.fire({\r\n");
                sb.Append("                                     title: 'Erro! Data em formato inválido.',\r\n");
                sb.Append("                                     icon: 'error'\r\n");
                sb.Append("                                 });\r\n");
                sb.Append("                             }\r\n");
                sb.Append("                         }\r\n");
                sb.Append("                     }).catch((error) => {\r\n");
                sb.Append("                         ErrorToast.fire({\r\n");
                sb.Append("                             title: 'Erro! Houve um erro na tentativa de Registrar as informações do Novo Evento! Erro: ' + error.message,\r\n");
                sb.Append("                             icon: 'error'\r\n");
                sb.Append("                         });\r\n");
                sb.Append("                     });\r\n");
                sb.Append("                 }\r\n");
                sb.Append("             }\r\n");
            }

            sb.Append("         },\r\n");

            sb.Append("         locale: '" + sLocal + "',\r\n");
            sb.Append("         timeZone: " + sFusoHorario + ",\r\n");
            sb.Append("         initialDate: " + sDataInicial + ",\r\n");
            sb.Append("         nowIndicator: " + bIndicadorAgora.ToString().ToLower() + ",\r\n");
            sb.Append("         navLinks: " + bNavLinks.ToString().ToLower() + ",\r\n");
            sb.Append("         editable: " + bEdita.ToString().ToLower() + ",\r\n");
            sb.Append("         selectable: " + bSeleciona.ToString().ToLower() + ",\r\n");
            sb.Append("         dayMaxEvents: " + bMaxEventos_x_Dia.ToString().ToLower() + ",\r\n");
            sb.Append("         businessHours: " + sHorarioTrabalho + ",\r\n");
            sb.Append("         buttonText: {\r\n" + sTextosBotoes + "},\r\n");
            sb.Append("         events: [\r\n");
            sb.Append("             " + ConverteDados_x_Eventos(dados) + "\r\n");
            sb.Append("         ],\r\n");
            sb.Append("         eventDidMount: function (info) {\r\n");
            sb.Append("             info.el.setAttribute('data-id', info.event.id);\r\n");
            //sb.Append("             const tipo = info.event.extendedProps.idTipo;\r\n");
            //sb.Append("             console.log(info.view.type, tipo);\r\n");
            //sb.Append("             if (info.view.type === 'listAtividades' && tipo != 2) {\r\n");
            //sb.Append("                 info.el.style.display = 'none';\r\n");
            //sb.Append("             }\r\n");
            sb.Append("         },\r\n");
            sb.Append("         views: {\r\n");
            sb.Append("             listDay: {\r\n");
            sb.Append("                 titleFormat: {\r\n");
            sb.Append("                     day: 'numeric',\r\n");
            sb.Append("                     month: 'numeric',\r\n");
            sb.Append("                     year: 'numeric'\r\n");
            sb.Append("                 }\r\n");
            sb.Append("             }\r\n");
            //sb.Append("             listAtividades: {\r\n");
            //sb.Append("                 type: 'list',\r\n");
            //sb.Append("                 visibleRange: function(currentDate) {\r\n");
            //sb.Append("                     return {\r\n");
            //sb.Append("                         start: '1999-12-31',\r\n");
            //sb.Append($"                         end: '{dataFinal_ListaGeral}'\r\n");
            //sb.Append("                     };\r\n");
            //sb.Append("                 }\r\n");
            //sb.Append("             }\r\n");
            sb.Append("         },\r\n");

            if (bEdita)
            {
                sb.Append("         eventClick: function(info) {\r\n");
                sb.Append("             info.jsEvent.preventDefault();\r\n");
                sb.Append("             const { id, title, allDay, display, url, backgroundColor, textColor, start, end, className, extendedProps } = info.event;\r\n");
                sb.Append("             const formatDate = (date) => {\r\n");
                sb.Append("                 return date && !isNaN(new Date(date).getTime()) ? new Date(date).toISOString().slice(0, 10) : '';\r\n");
                sb.Append("             };\r\n");
                sb.Append("             const formatDateTime = (date) => {\r\n");
                sb.Append("                 return date && !isNaN(new Date(date).getTime()) ? new Date(date).toISOString().slice(0, 16) : '';\r\n");
                sb.Append("             };\r\n");
                sb.Append("             const infoExtra = extendedProps.extra ?\r\n");
                sb.Append("                         `<div class=\"col-lg-12 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                             <label for=\"eventExtra\">Informações Extras</label>\r\n");
                sb.Append("                             <textarea id=\"eventExtra\" class=\"form-control\" style=\"height: 100px;\" disabled=\"disabled\">${extendedProps.extra || ''}</textarea>\r\n");
                sb.Append("                         </div>`\r\n");
                sb.Append("                 : '';\r\n");
                sb.Append("             const barraConclusao = extendedProps.idTipo == 1 ?\r\n");
                sb.Append("                     `<div class=\"col-lg-12\" style=\"text-align: start;\">\r\n");
                sb.Append("                         <label style=\"float: left;\">Conclusão Geral das Tarefas do Departamento: ${ extendedProps.nVlrConclusao }%</label>\r\n");
                sb.Append("                         <div class=\"col-lg-12 form-group progress\" style=\"padding: 0;background-color: #eee;border: 1px solid gray;\">\r\n");
                sb.Append("                             <div class=\"progress-bar progress-bar-success progress-bar-striped active\" role=\"progressbar\" aria-valuenow=\"0\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: ${ extendedProps.nVlrConclusao }%\"></div>\r\n");
                sb.Append("                         </div>\r\n");
                sb.Append("                     </div>`\r\n");
                sb.Append("                 : '';\r\n");
                sb.Append($"             const botaoLink = {(!bViewPedidos).ToString().ToLower()} ?\r\n");
                sb.Append("                     `<div class=\"col-lg-12 form-group\" style=\"${url && url.trim() ? '' : 'display:none;'}\">\r\n");
                sb.Append("                         <input type=\"button\" id=\"eventLink_Click\" class=\"btn btn-md btn-link btn-url\" value=\"Link do ${extendedProps.idTipo == 1 ? 'Pedido' : extendedProps.idTipo == 2 ? 'Projeto' : 'Evento'}\" data-url=\"${url}\" />\r\n");
                sb.Append("                     </div>`\r\n");
                sb.Append("                 : '';\r\n");
                sb.Append("             const status_DatasPrevistas = extendedProps.idTipo == 2 ?\r\n");
                sb.Append("                     `<div class=\"col-lg-4 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                         <label>Status</label>\r\n");
                sb.Append("                         <div class=\"input-group\">\r\n");
                sb.Append("                             <input id=\"eventStatus\" type=\"text\" disabled=\"disabled\" class=\"form-control\" value=\"${extendedProps.sDscStatus}\" />\r\n");
                sb.Append("                             <span class=\"input-group-btn\" ${extendedProps.idStatus == 4 || extendedProps.idStatus == 5 ? 'style=\"display: none;\"' : ''}>\r\n");
                sb.Append("                                 <a href=\"${url}&atividade=${id}\" target=\"_blank\" class=\"btn btn-primary\" data-toggle=\"tooltip_Calendario\" title=\"Alterar Status\"><i class=\"fa fa-pencil\"></i></a>\r\n");
                sb.Append("                             </span>\r\n");
                sb.Append("                         </div>\r\n");
                sb.Append("                     </div>\r\n");
                sb.Append("                     <div class=\"col-lg-12\" style=\"padding: 0; text-align: start;\">\r\n");
                sb.Append("                         <div class=\"col-lg-9 div_dates_prev\" style=\"padding: 0; text-align: start;\">\r\n");
                sb.Append("                             <div class=\"col-lg-6 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                                 <label for=\"eventStart_Prev\">Data Inicial Prevista</label>\r\n");
                sb.Append("                                 <input id=\"eventStart_Prev\" type=\"datetime-local\" disabled=\"disabled\" class=\"form-control\" value=\"${formatDateTime(extendedProps.dtInicial_Previsao)}\" />\r\n");
                sb.Append("                             </div>\r\n");
                sb.Append("                             <div class=\"col-lg-6\" style=\"text-align: start;\">\r\n");
                sb.Append("                                 <label for=\"eventEnd_Prev\">Data Final Prevista</label>\r\n");
                sb.Append("                                 <input id=\"eventEnd_Prev\" type=\"datetime-local\" disabled=\"disabled\" class=\"form-control\" value=\"${formatDateTime(extendedProps.dtFinal_Previsao)}\" />\r\n");
                sb.Append("                             </div>\r\n");
                sb.Append("                         </div>\r\n");
                sb.Append("                     </div>`\r\n");
                sb.Append("                 : '';\r\n");
                sb.Append("             const switch_Data = extendedProps.idTipo != 2 ?\r\n");
                sb.Append("                     `<div class=\"col-lg-2 form-group div_switchAllDay\" style=\"text-align: start;\">\r\n");
                sb.Append("                         <label for=\"eventAllDay\" style=\"width: 100%;\">Dia inteiro?</label>\r\n");
                sb.Append("                         <label class=\"eventAllDay switchPersonalizado\">\r\n");
                sb.Append("                             <input id=\"eventAllDay\" type=\"checkbox\">\r\n");
                sb.Append("                             <span class=\"spanSwitch spanSwitch_Edit\"></span>\r\n");
                sb.Append("                         </label>\r\n");
                sb.Append("                     </div>\r\n");
                sb.Append("                     <div class=\"col-lg-4 form-group div_allDay\" style=\"text-align: start;\">\r\n");
                sb.Append("                         <label for=\"eventDate\">Data do Evento</label>\r\n");
                sb.Append("                         <input id=\"eventDate\" type=\"date\" class=\"form-control\" value=\"${formatDate(start)}\" />\r\n");
                sb.Append("                     </div>`\r\n");
                sb.Append("                 : '';\r\n");
                sb.Append("             const camposHoras__Usuarios = extendedProps.idTipo == 2 ?\r\n");
                sb.Append("                     `<div class=\"col-lg-12\" style=\"padding: 0; text-align: start;\">\r\n");
                sb.Append("                         <div class=\"col-lg-4 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                             <label for=\"eventRealizadas\">Horas Realizadas</label>\r\n");
                sb.Append("                             <input id=\"eventRealizadas\" class=\"form-control\" disabled=\"disabled\" value=\"${extendedProps.nHoras_Realizadas ? extendedProps.nHoras_Realizadas.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : '0,00'}\" />\r\n");
                sb.Append("                         </div>\r\n");
                sb.Append("                         <div class=\"col-lg-4\" style=\"text-align: start;\">\r\n");
                sb.Append("                             <label for=\"eventPrevisao\">Horas Previstas</label>\r\n");
                sb.Append("                             <input id=\"eventPrevisao\" class=\"form-control\" disabled=\"disabled\" value=\"${extendedProps.nHoras_Previsao ? extendedProps.nHoras_Previsao.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : '0,00'}\" />\r\n");
                sb.Append("                         </div>\r\n");
                sb.Append("                         <div class=\"col-lg-4\" style=\"text-align: start;\">\r\n");
                sb.Append("                             <label for=\"eventDisponiveis\">Horas Disponíveis</label>\r\n");
                sb.Append("                             <input id=\"eventDisponiveis\" class=\"form-control\" disabled=\"disabled\" value=\"${extendedProps.nHoras_Disponiveis ? extendedProps.nHoras_Disponiveis.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : '0,00'}\" />\r\n");
                sb.Append("                         </div>\r\n");
                sb.Append("                     </div>\r\n");
                sb.Append("                     <div class=\"col-lg-12\" style=\"text-align: start;\">\r\n");
                sb.Append("                         <label>Usuários Associados</label>\r\n");
                sb.Append("                         <div class=\"usuarios form-group\">\r\n");
                sb.Append("                             ${extendedProps.idUsuario}\r\n");
                sb.Append("                         </div>\r\n");
                sb.Append("                     </div>`\r\n");
                sb.Append("                 : `" + PopulaUsuarios(false) + "`\r\n");
                sb.Append($"             const campoURL = {(!bViewPedidos).ToString().ToLower()} ?\r\n");
                sb.Append("                     `<div class=\"col-lg-12 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                         <label for=\"eventURL\">Link do ${extendedProps.idTipo == 1 ? 'Pedido' : extendedProps.idTipo == 2 ? 'Projeto' : 'Evento'}</label>\r\n");
                sb.Append("                         <textarea id=\"eventURL\" class=\"form-control\" style=\"height: 75px;\" ${extendedProps.idTipo == 2 ? 'disabled=\"disabled\"' : ''}>${url}</textarea>\r\n");
                sb.Append("                     </div>`\r\n");
                sb.Append("                 : ''\r\n");
                sb.Append("             const campoAtivo_UltimaAtualizacao = extendedProps.idTipo != 2 ?\r\n");
                sb.Append("                     `<div class=\"col-lg-12 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                         <label for=\"eventAtivo\">Ativo</label>\r\n");
                sb.Append("                         <select id=\"eventAtivo\" class=\"form-control CaixaTextoMini\">\r\n");
                sb.Append("                             <option value=\"S\">Sim</option>\r\n");
                sb.Append("                             <option value=\"N\">Não</option>\r\n");
                sb.Append("                         </select>\r\n");
                sb.Append("                     </div>\r\n");
                sb.Append("                     <div class=\"col-lg-12\" style=\"text-align: start;\">\r\n");
                sb.Append("                         <br />\r\n");
                sb.Append("                         <br />\r\n");
                sb.Append("                         <p>Última Atualização Por <label>${extendedProps.usuarioAtualizacao || ''}</label> em <label>${new Date(extendedProps.dataAtualizacao).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' }) || ''}</label></p>\r\n");
                sb.Append("                     </div>`\r\n");
                sb.Append("                 : ''\r\n");
                sb.Append("             Swal.fire({\r\n");
                sb.Append("                 title: title,\r\n");
                sb.Append("                 html: `\r\n");
                sb.Append("                     <hr style=\"margin: -5px 0 25px 0; border-color: #bbb;\">\r\n");

                sb.Append("                     ${ barraConclusao }\r\n");
                sb.Append("                     ${ botaoLink }\r\n");

                sb.Append("                     <div class=\"col-lg-12 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                         <label for=\"eventID\">ID</label>\r\n");
                sb.Append("                         <input id=\"eventID\" class=\"form-control CaixaTextoMini\" value=\"${id}\" disabled=\"disabled\" />\r\n");
                sb.Append("                     </div>\r\n");
                sb.Append("                     <div class=\"col-lg-12 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                         <label for=\"eventTitle\">Título</label>\r\n");
                sb.Append("                         <input id=\"eventTitle\" class=\"form-control\" value=\"${title}\" ${extendedProps.idTipo == 2 ? 'disabled=\"disabled\"' : ''} />\r\n");
                sb.Append("                     </div>\r\n");

                sb.Append("                     ${ status_DatasPrevistas }\r\n");

                sb.Append("                     <div class=\"col-lg-12\" style=\"padding: 0; text-align: start;\">\r\n");

                sb.Append("                         ${ switch_Data }\r\n");

                sb.Append("                         <div class=\"col-lg-9 div_dates\" style=\"padding: 0; text-align: start;\">\r\n");
                sb.Append("                             <div class=\"col-lg-6 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                                 <label for=\"eventStart\">Data Inicial do Evento</label>\r\n");
                sb.Append("                                 <input id=\"eventStart\" type=\"datetime-local\" class=\"form-control\" value=\"${formatDateTime(extendedProps.idTipo != 2 ? start : extendedProps.dtInicial)}\" ${extendedProps.idTipo == 2 ? 'disabled=\"disabled\"' : ''} />\r\n");
                sb.Append("                             </div>\r\n");
                sb.Append("                             <div class=\"col-lg-6\" style=\"text-align: start;\">\r\n");
                sb.Append("                                 <label for=\"eventEnd\">Data final do Evento</label>\r\n");
                sb.Append("                                 <input id=\"eventEnd\" type=\"datetime-local\" class=\"form-control\" value=\"${formatDateTime(extendedProps.idTipo != 2 ? end : extendedProps.dtFinal)}\" ${extendedProps.idTipo == 2 ? 'disabled=\"disabled\"' : ''} />\r\n");
                sb.Append("                             </div>\r\n");
                sb.Append("                         </div>\r\n");
                sb.Append("                     </div>\r\n");

                sb.Append("                     ${ camposHoras__Usuarios }\r\n");
                sb.Append("                     ${ campoURL }\r\n");

                sb.Append("                     <div class=\"col-lg-12 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                         <label for=\"eventDescription\">Descrição</label>\r\n");
                sb.Append("                         <textarea id=\"eventDescription\" class=\"form-control\" style=\"height: 100px;\" ${extendedProps.idTipo == 2 ? 'disabled=\"disabled\"' : ''}>${extendedProps.descricao || ''}</textarea>\r\n");
                sb.Append("                     </div>\r\n");

                sb.Append("                     ${ infoExtra }\r\n");
                sb.Append("                     ${ campoAtivo_UltimaAtualizacao }\r\n");

                sb.Append("                 `,\r\n");
                sb.Append("                 focusConfirm: false,\r\n");
                sb.Append("                 showConfirmButton: extendedProps.idTipo == 2 ? extendedProps.idStatus == 2 : true,\r\n");
                sb.Append("                 confirmButtonText: extendedProps.idTipo == 2 ? 'Novo Apontamento' : 'Atualizar',\r\n");
                sb.Append("                 confirmButtonColor: extendedProps.idTipo == 2 ? '#5bc0de' : '#5cb85c',\r\n");
                sb.Append("                 showCancelButton: true,\r\n");
                sb.Append("                 cancelButtonText: 'Voltar',\r\n");
                sb.Append("                 cancelButtonColor: '#f0ad4e',\r\n");
                sb.Append("                 didOpen: () => {\r\n");
                sb.Append("                     setTimeout(() => {\r\n");
                sb.Append("                         $('[data-toggle=tooltip_Calendario]').tooltip();\r\n");
                sb.Append("                         $('.swal2-modal').addClass('modalSwal_Personalizado grande');\r\n");
                sb.Append("                         $('#eventTitle').focus();\r\n");
                sb.Append("                     }, 10);\r\n");
                sb.Append("                     if (document.getElementById('usuario_Evento')) {\r\n");
                sb.Append("                         document.getElementById('usuario_Evento').value = extendedProps.idUsuario;\r\n");
                sb.Append("                     }\r\n");
                sb.Append("                     if (document.getElementById('eventAtivo')) {\r\n");
                sb.Append("                         document.getElementById('eventAtivo').value = extendedProps.ativo;\r\n");
                sb.Append("                     }\r\n");
                sb.Append("                     if (allDay) {\r\n");
                sb.Append("                         $('#eventAllDay').prop('checked', true).trigger('change');\r\n");
                sb.Append("                         $('.div_allDay').addClass('show');\r\n");
                sb.Append("                         $('.div_dates').addClass('hide');\r\n");
                sb.Append("                     } else {\r\n");
                sb.Append("                         $('.div_allDay').addClass('hide');\r\n");
                sb.Append("                         $('.div_dates').addClass('show');\r\n");
                sb.Append("                     }\r\n");
                sb.Append("                 },\r\n");

                sb.Append("                 preConfirm: () => {\r\n");
                sb.Append("                     if (extendedProps.idTipo != 2) {\r\n");
                sb.Append("                         const evento = {\r\n");
                sb.Append("                             id: id,\r\n");
                sb.Append("                             title: document.getElementById('eventTitle').value,\r\n");
                sb.Append("                             display: display,\r\n");
                sb.Append("                             allDay: document.getElementById('eventAllDay').checked,\r\n");
                sb.Append("                             url: document.getElementById('eventURL').value,\r\n");
                sb.Append("                             color: backgroundColor,\r\n");
                sb.Append("                             textColor: textColor,\r\n");
                sb.Append("                             date: document.getElementById('eventDate').value,\r\n");
                sb.Append("                             start: document.getElementById('eventStart').value,\r\n");
                sb.Append("                             end: document.getElementById('eventEnd').value,\r\n");
                sb.Append("                             descricao: document.getElementById('eventDescription').value,\r\n");
                sb.Append("                             idUsuario: document.getElementById('usuario_Evento').options[document.getElementById('usuario_Evento').selectedIndex].value,\r\n");
                sb.Append("                             ativo: document.getElementById('eventAtivo').options[document.getElementById('eventAtivo').selectedIndex].value\r\n");
                sb.Append("                         };\r\n");
                sb.Append("                         if (evento.allDay && (!evento.title || !evento.date)) {\r\n");
                sb.Append("                             Swal.showValidationMessage('É necessário preencher tanto o campo do Título quanto o campo de Data!');\r\n");
                sb.Append("                             return false;\r\n");
                sb.Append("                         } else if (!evento.allDay && (!evento.title || !evento.start || !evento.end)) {\r\n");
                sb.Append("                             Swal.showValidationMessage('É necessário preencher tanto o campo do Título quanto os campos de Data inicial e final!');\r\n");
                sb.Append("                             return false;\r\n");
                sb.Append("                         } else if (!evento.descricao && !evento.idUsuario) {\r\n");
                sb.Append("                             Swal.showValidationMessage('É necessário preencher o campo da Descrição e selecionar um Usuário!');\r\n");
                sb.Append("                             return false;\r\n");
                sb.Append("                         }\r\n");
                sb.Append("                         return evento;\r\n");
                sb.Append("                     } else {\r\n");
                sb.Append("                         Swal.fire({\r\n");
                sb.Append("                             title: 'Novo Apontamento',\r\n");
                sb.Append("                             html: `\r\n");
                sb.Append("                                 <hr style=\"margin: -5px 0 25px 0; border-color: #bbb;\">\r\n");
                sb.Append("                                 <div class=\"col-lg-12 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                                     <label>Usuários</label>\r\n");
                sb.Append("                                     <select id=\"ddlUsuario_Evento\" class=\"form-control\" ${extendedProps.sAssociaUsuario}>${extendedProps.sUsuarios_NovoApontamento}</select>\r\n");
                sb.Append("                                 </div>\r\n");
                sb.Append("                                 <div class=\"col-lg-4 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                                     <label>Horas Realizadas</label>\r\n");
                sb.Append("                                     <input type=\"text\" id=\"horas\" class=\"form-control nHoras\" />\r\n");
                sb.Append("                                 </div>\r\n");
                sb.Append("                                 <div class=\"col-lg-12 form-group\" style=\"text-align: start;\">\r\n");
                sb.Append("                                     <label>Observação</label>\r\n");
                sb.Append("                                     <textarea id=\"obs\" class=\"form-control\" style=\"height: 150px\"></textarea>\r\n");
                sb.Append("                                 </div>\r\n");
                sb.Append("                             `,\r\n");
                sb.Append("                             showConfirmButton: true,\r\n");
                sb.Append("                             showCancelButton: true,\r\n");
                sb.Append("                             confirmButtonText: 'Salvar',\r\n");
                sb.Append("                             confirmButtonColor: '#5cb85c',\r\n");
                sb.Append("                             cancelButtonText: 'Cancelar',\r\n");
                sb.Append("                             cancelButtonColor: '#d9534f',\r\n");
                sb.Append("                             didOpen: () => {\r\n");
                sb.Append("                                 $('.nHoras').unmask().mask('000.009,99', { reverse: true });\r\n\r\n");
                sb.Append("                                 $('.swal2-modal').addClass('modalSwal_Personalizado medio');\r\n");
                sb.Append("                                 if (document.getElementById('ddlUsuario_Evento')) {\r\n");
                sb.Append($"                                    document.getElementById('ddlUsuario_Evento').value = {Identity.Variaveis.idUsuario()};\r\n");
                sb.Append("                                 }\r\n");
                sb.Append("                             },\r\n");
                sb.Append("                             preConfirm: () => {\r\n");
                sb.Append("                                 const apontamento = {\r\n");
                sb.Append("                                     id: id,\r\n");
                sb.Append("                                     idUsuario: parseInt(document.getElementById('ddlUsuario_Evento').value) || 0,\r\n");
                sb.Append("                                     sObs: $('#obs').val().replace(/\\n/g, '<br />') || '',\r\n");
                sb.Append("                                     nHoras: parseFloat($('#horas').val().replace('.', '').replace(',', '.')) || 0,\r\n");
                sb.Append("                                     dtApontamento: new Date()\r\n");
                sb.Append("                                 };\r\n");
                sb.Append("                                 if (!apontamento.nHoras) {\r\n");
                sb.Append("                                     Swal.showValidationMessage('É necessário preencher as Horas realizadas no Apontamento!');\r\n");
                sb.Append("                                     return false;\r\n");
                sb.Append("                                 } else if (!apontamento.sObs) {\r\n");
                sb.Append("                                     Swal.showValidationMessage('É necessário preencher a Observação do Apontamento!');\r\n");
                sb.Append("                                     return false;\r\n");
                sb.Append("                                 }\r\n");
                sb.Append("                                 return apontamento;\r\n");
                sb.Append("                             }\r\n");
                sb.Append("                         }).then((novoApontamento) => {\r\n");
                sb.Append("                             if (novoApontamento.isConfirmed) {\r\n");
                sb.Append("                                 const apontamento = novoApontamento.value;\r\n");
                sb.Append("                                 $v192.ajax({\r\n");
                sb.Append("                                     url:'/API/Pagina_Ajax.aspx/Salvar_NovoApontamento_Atividades_Calendario',\r\n");
                sb.Append("                                     data: JSON.stringify({\r\n");
                sb.Append("                                         'idAtividade': apontamento.id,\r\n");
                sb.Append("                                         'idUsuario': apontamento.idUsuario,\r\n");
                sb.Append("                                         'sObservacao': apontamento.sObs,\r\n");
                sb.Append("                                         'nHoras': apontamento.nHoras,\r\n");
                sb.Append("                                         'dtApontamento': apontamento.dtApontamento\r\n");
                sb.Append("                                     }),\r\n");
                sb.Append("                                     dataType: \"json\",\r\n");
                sb.Append("                                     type: \"POST\",\r\n");
                sb.Append("                                     contentType: \"application/json; charset=utf-8\",\r\n");
                sb.Append("                                     success: function() {\r\n");
                sb.Append("                                         const realizadas = (parseFloat(extendedProps.nHoras_Realizadas) || 0) + (parseFloat(apontamento.nHoras) || 0);\r\n");
                sb.Append("                                         info.event.setExtendedProp('nHoras_Realizadas', realizadas);\r\n");
                sb.Append("                                         info.event.setExtendedProp('nHoras_Disponiveis', (parseFloat(extendedProps.nHoras_Previsao) || 0) - realizadas);\r\n");
                //sb.Append("                                         setTimeout(function () {\r\n");
                //sb.Append("                                             $(calendario.el).find(`.fc-event[data-id=\"${id}\"]`).trigger('click');\r\n");
                //sb.Append("                                         }, 250);\r\n");
                sb.Append("                                         Toast.fire({\r\n");
                sb.Append("                                             title: 'Apontamento de Horas para Atividade registrado com sucesso!',\r\n");
                sb.Append("                                             icon: 'success'\r\n");
                sb.Append("                                         });\r\n");
                sb.Append("                                     },\r\n");
                sb.Append("                                     error: function(response) {\r\n");
                sb.Append("                                         ErrorToast.fire({\r\n");
                sb.Append("                                             title: 'Erro! ' + response.responseText,\r\n");
                sb.Append("                                             icon: 'error'\r\n");
                sb.Append("                                         });\r\n");
                sb.Append("                                     }\r\n");
                sb.Append("                                 });");
                sb.Append("                             }\r\n");
                sb.Append("                         }).catch((error) => {\r\n");
                sb.Append("                             ErrorToast.fire({\r\n");
                sb.Append("                                 title: 'Erro! Houve um erro ao Salvar um Novo Apontamento! Erro: ' + error.message,\r\n");
                sb.Append("                                 icon: 'error'\r\n");
                sb.Append("                             });\r\n");
                sb.Append("                         });\r\n");
                sb.Append("                     }\r\n");
                sb.Append("                 }\r\n");
                sb.Append("             }).then((eventoAtualizado) => {\r\n");
                sb.Append("                 if (extendedProps.idTipo != 2) {\r\n");
                sb.Append("                     if (eventoAtualizado.isConfirmed) {\r\n");
                sb.Append("                         const evento = eventoAtualizado.value;\r\n");
                sb.Append("                         $v192.ajax({\r\n");
                sb.Append("                             url:'/API/Pagina_Ajax.aspx/SalvarEvento_Calendario',\r\n");
                sb.Append("                             data: JSON.stringify({\r\n");
                sb.Append("                                 'idEvento': evento.id,\r\n");
                sb.Append("                                 'idUsuario': evento.idUsuario,\r\n");
                sb.Append("                                 'sDscTitulo': evento.title,\r\n");
                sb.Append("                                 'sDscEvento': evento.descricao,\r\n");
                sb.Append("                                 'sDiaInteiro': evento.allDay ? 'S' : 'N',\r\n");
                sb.Append("                                 'sUrl': evento.url,\r\n");
                sb.Append("                                 'dtInicial': evento.allDay ? evento.date : evento.start,\r\n");
                sb.Append("                                 'dtFinal': evento.allDay ? null : (evento.end || null),\r\n");
                sb.Append("                                 'sAtivo': evento.ativo\r\n");
                sb.Append("                             }),\r\n");
                sb.Append("                             dataType: \"json\",\r\n");
                sb.Append("                             type: \"POST\",\r\n");
                sb.Append("                             contentType: \"application/json; charset=utf-8\",\r\n");
                sb.Append("                             success: function(response) {\r\n");
                sb.Append("                                 if (response) {\r\n");
                sb.Append("                                     response = JSON.parse(response.d);\r\n");
                sb.Append("                                     if (response.retorno == 0) {\r\n");
                sb.Append("                                         info.event.setProp('title', evento.title);\r\n");
                sb.Append("                                         info.event.setAllDay(evento.allDay);\r\n");
                sb.Append("                                         info.event.setProp('url', evento.url);\r\n");
                sb.Append("                                         info.event.setProp('classNames', evento.ativo == 'S' ? [] : ['eventoInativo']);\r\n");
                sb.Append("                                         info.event.setStart(evento.allDay ? evento.date : evento.start);\r\n");
                sb.Append("                                         info.event.setEnd(evento.allDay ? null : (evento.end || null));\r\n");
                sb.Append("                                         info.event.setExtendedProp('descricao', evento.descricao);\r\n");
                sb.Append("                                         info.event.setExtendedProp('idUsuario', evento.idUsuario);\r\n");
                sb.Append("                                         info.event.setExtendedProp('usuarioAtualizacao', response.usuario);\r\n");
                sb.Append("                                         info.event.setExtendedProp('dataAtualizacao', new Date(response.data));\r\n");
                sb.Append("                                         info.event.setExtendedProp('ativo', evento.ativo);\r\n");
                sb.Append("                                         Toast.fire({\r\n");
                sb.Append("                                             title: 'Atualizado! Evento atualizado com sucesso.',\r\n");
                sb.Append("                                             icon: 'success'\r\n");
                sb.Append("                                         });\r\n");
                sb.Append("                                     } else {\r\n");
                sb.Append("                                         ErrorToast.fire({\r\n");
                sb.Append("                                             title: 'Erro! ' + response.msg,\r\n");
                sb.Append("                                             icon: 'error'\r\n");
                sb.Append("                                         });\r\n");
                sb.Append("                                     }\r\n");
                sb.Append("                                 }\r\n");
                sb.Append("                             },\r\n");
                sb.Append("                             error: function(response) {\r\n");
                sb.Append("                                 ErrorToast.fire({\r\n");
                sb.Append("                                     title: 'Erro! ' + response.responseText,\r\n");
                sb.Append("                                     icon: 'error'\r\n");
                sb.Append("                                 });\r\n");
                sb.Append("                             }\r\n");
                sb.Append("                         });");
                sb.Append("                     }\r\n");
                sb.Append("                 }\r\n");
                sb.Append("             }).catch((error) => {;\r\n");
                sb.Append("                 if (extendedProps.idTipo != 2) {\r\n");
                sb.Append("                     ErrorToast.fire({\r\n");
                sb.Append("                         title: 'Erro! Houve um erro na tentativa de Atualizar as informações do Evento! Erro: ' + error.message,\r\n");
                sb.Append("                         icon: 'error'\r\n");
                sb.Append("                     });\r\n");
                sb.Append("                 }\r\n");
                sb.Append("             });\r\n");

                sb.Append("         },\r\n");
                sb.Append("         eventDrop: function(info) {\r\n");
                sb.Append("             info.jsEvent.preventDefault();\r\n");

                if (bArrasta_Solta)
                {
                    sb.Append("             const { id, title, allDay, url, start, end, className, extendedProps } = info.event;\r\n");
                    sb.Append("             var start_3 = new Date(start);\r\n");
                    sb.Append("             start_3.setHours(start_3.getHours() + 3);\r\n");
                    sb.Append("             end_3 = end ? new Date(end) : null;\r\n");
                    sb.Append("             if (end_3) {\r\n");
                    sb.Append("                 end_3.setHours(end_3.getHours() + 3);\r\n");
                    sb.Append("             }\r\n");
                    sb.Append("             start_3 = start_3.toISOString();\r\n");
                    sb.Append("             end_3 = end_3 ? end_3.toISOString() : null;\r\n");
                    sb.Append("             $v192.ajax({\r\n");
                    sb.Append("                 url:'/API/Pagina_Ajax.aspx/SalvarEvento_Calendario',\r\n");
                    sb.Append("                 data: JSON.stringify({\r\n");
                    sb.Append("                     'idEvento': id,\r\n");
                    sb.Append("                     'idUsuario': extendedProps.idUsuario,\r\n");
                    sb.Append("                     'sDscTitulo': title,\r\n");
                    sb.Append("                     'sDscEvento': extendedProps.descricao,\r\n");
                    sb.Append("                     'sDiaInteiro': allDay ? 'S' : 'N',\r\n");
                    sb.Append("                     'sUrl': url,\r\n");
                    sb.Append("                     'dtInicial': start_3,\r\n");
                    sb.Append("                     'dtFinal': end_3 || null,\r\n");
                    sb.Append("                     'sAtivo': extendedProps.ativo\r\n");
                    sb.Append("                 }),\r\n");
                    sb.Append("                 dataType: \"json\",\r\n");
                    sb.Append("                 type: \"POST\",\r\n");
                    sb.Append("                 contentType: \"application/json; charset=utf-8\",\r\n");
                    sb.Append("                 success: function(response) {\r\n");
                    sb.Append("                     if (response) {\r\n");
                    sb.Append("                         response = JSON.parse(response.d);\r\n");
                    sb.Append("                         if (response.retorno == 0) {\r\n");
                    sb.Append("                             info.event.setStart(start);\r\n");
                    sb.Append("                             info.event.setEnd(end || null);\r\n");
                    sb.Append("                             Toast.fire({\r\n");
                    sb.Append("                                 title: 'Salvo! Evento atualizado com sucesso!',\r\n");
                    sb.Append("                                 icon: 'success'\r\n");
                    sb.Append("                             });\r\n");
                    sb.Append("                         } else {\r\n");
                    sb.Append("                             ErrorToast.fire({\r\n");
                    sb.Append("                                 title: 'Erro! ' + response.msg,\r\n");
                    sb.Append("                                 icon: 'error'\r\n");
                    sb.Append("                             });\r\n");
                    sb.Append("                         }\r\n");
                    sb.Append("                     }\r\n");
                    sb.Append("                 },\r\n");
                    sb.Append("                 error: function(response) {\r\n");
                    sb.Append("                     ErrorToast.fire({\r\n");
                    sb.Append("                         title: 'Erro! ' + response.responseText,\r\n");
                    sb.Append("                         icon: 'error'\r\n");
                    sb.Append("                     });\r\n");
                    sb.Append("                 }\r\n");
                    sb.Append("             });");
                }

                sb.Append("         }\r\n");
            }
            else
            {
                sb.Append("         eventClick: function(info) {\r\n");
                sb.Append("             info.jsEvent.preventDefault();\r\n");
                sb.Append("         }\r\n");
            }

            sb.Append("     });\r\n");
            sb.Append("     calendario.render();\r\n");
            sb.Append("     div_calendario._calendar = calendario;\r\n");

            if (!PainelLateralFixo)
            {
                sb.Append("     calendario.updateSize();\r\n");

                sb.Append("     $(window).on('resize', function () {\r\n");
                sb.Append("         calendario.updateSize();\r\n");
                sb.Append("     });\r\n");

                sb.Append("     setTimeout(function () {\r\n");
                sb.Append("         $(window).trigger('resize');\r\n");
                sb.Append("     }, 250);\r\n");
            }

            sb.Append("})();\r\n\r\n");

            string script = sb.ToString();

            if (TipoEvento == 2)
                script = script.Replace("o Evento", "a Atividade").Replace("o evento", "a atividade").Replace("Evento", "Atividade").Replace("evento", "atividade")
                                .Replace("o Pedido", "a Atividade").Replace("o pedido", "a atividade").Replace("Pedido", "Atividade").Replace("pedido", "atividade");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), $"js_Calendario_FullCalendar_{TipoEvento}_{TipoVisualizacao}", script, true);

            if (RegistraScript_Global)
            {
                RegistraScript_Global = false;

                sb.Clear();

                sb.Append("$(document).ready(function() {\r\n");

                sb.Append("     $(document).on('change', '#diaInteiroSwitch', function() {\r\n");
                sb.Append("         if ($(this).is(\":checked\")) {\r\n");
                sb.Append("             $('.diaInteiroSwitch').css('background-color', 'forestgreen');\r\n");
                sb.Append("             $('.div_diaInteiro').removeClass('show hide').addClass('show');\r\n");
                sb.Append("             $('.div_datas').removeClass('show hide').addClass('hide');\r\n");
                sb.Append("             $('.spanSwitch_novo').removeClass('switchAtivo').addClass('switchAtivo');\r\n");
                sb.Append("         } else {\r\n");
                sb.Append("             $('.diaInteiroSwitch').css('background-color', '#ccc');\r\n");
                sb.Append("             $('.div_diaInteiro').removeClass('show hide').addClass('hide');\r\n");
                sb.Append("             $('.div_datas').removeClass('show hide').addClass('show');\r\n");
                sb.Append("             $('.spanSwitch_novo').removeClass('switchAtivo');\r\n");
                sb.Append("         }\r\n");
                sb.Append("     });\r\n\r\n");

                sb.Append("     $(document).on('change', '#eventAllDay', function() {\r\n");
                sb.Append("         if ($(this).is(\":checked\")) {\r\n");
                sb.Append("             $('.eventAllDay').css('background-color', 'forestgreen');\r\n");
                sb.Append("             $('.div_allDay').removeClass('show hide').addClass('show');\r\n");
                sb.Append("             $('.div_dates').removeClass('show hide').addClass('hide');\r\n");
                sb.Append("             $('.spanSwitch_Edit').removeClass('switchAtivo').addClass('switchAtivo');\r\n");
                sb.Append("         } else {\r\n");
                sb.Append("             $('.eventAllDay').css('background-color', '#ccc');\r\n");
                sb.Append("             $('.div_allDay').removeClass('show hide').addClass('hide');\r\n");
                sb.Append("             $('.div_dates').removeClass('show hide').addClass('show');\r\n");
                sb.Append("             $('.spanSwitch_Edit').removeClass('switchAtivo');\r\n");
                sb.Append("         }\r\n");
                sb.Append("     });\r\n\r\n");

                sb.Append("     $(document).on('click', '#eventLink_Click', function(e) {\r\n");
                sb.Append("         e.preventDefault();\r\n");
                sb.Append("          window.open($(e.target).data('url')); \r\n");
                sb.Append("     });\r\n\r\n");

                sb.Append("});\r\n\r\n");

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Calendario_Modal", sb.ToString(), true);
            }
        }

        public void Desativar_Calendario()
        {
            StringBuilder sb = new StringBuilder();

            idUnico_div = div_calendario.ClientID;

            sb.Append($"var div_calendario = document.getElementById('{idUnico_div}');\r\n");
            sb.Append("if (div_calendario._calendar) {\r\n");
            sb.Append("     div_calendario._calendar.destroy();\r\n");
            sb.Append("}\r\n");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), $"js_Calendario_FullCalendar_{TipoEvento}_{TipoVisualizacao}", sb.ToString(), true);
        }

        #endregion
    }
}