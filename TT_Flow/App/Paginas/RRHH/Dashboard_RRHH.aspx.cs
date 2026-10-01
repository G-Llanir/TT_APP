using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.Services;
using System.Web.Script.Serialization;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using System.Web.UI.WebControls;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Web.UI.HtmlControls;

namespace TT_Flow.Dashboards.RRHH
{
    // http://localhost:6997/App/Paginas/RRHH/Dashboard_RRHH.aspx
    public partial class DashboardRRHH : Page
    {
        private static int TAB_Grafico_NR = 0;
        private static int TAB_Grafico_EPINovo = 1;
        private static int TAB_Grafico_Documentos = 2;
        private static int TAB_Grafico_PlanoSaude = 3;
        private static int TAB_Aniversario = 4;
        private static int TAB_Ferias = 5;

        private static string sProcedure = "sp_Flow_DashBoard_RRHH";
        

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "ManualDoUsuário_DashBoardRRHH-Agnes_Partal.docx";

            if (Request["__EVENTTARGET"] == "dialog_checksemprevisao")
            {
                if (switchPag.Checked)
                {
                    AtualizarDashBoard();
                    switchPag.Checked = true;
                }
                else
                {                   
                    AtualizarDashBoard();
                    switchPag.Checked = false;
                }
            }

            if (!IsPostBack && FUNCOES.ValidaPermissao(Permissao.RRHH.DashboardRRHH.Consultar))
            {
                AtualizarDashBoard();

                if (!FUNCOES.ValidaPermissao(Permissao.RRHH.DashboardRRHH.VisualizaAniversario))
                    divPanelAniversarios.Visible = false;               

            }            

        }

        void AtualizarDashBoard()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string> { { "@sFuncao", "POPULA_GRAFICO" } };
            //--------------- Higor Maestrello 11/07/2024 -----------------
            //vParametros.Add("@idUsuario", Identity.Variaveis.idUsuario());
            //if (Identity.Variaveis.idEmpresa() == "Brasil")
            //{
            //    vParametros.Add("@idPais", "1");
            //}
            //else if (Identity.Variaveis.idEmpresa() == "EUA")
            //{
            //    vParametros.Add("@idPais", "2");
            //}
            //-------------------------------------------------------
            if (switchPag.Checked)
                vParametros.Add("@SemPrevisao", "S");

            DataSet dsDashBoard = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsDashBoard))
            {
                // Calendario

                Calendario_View.EstilosPersonalizados_Titulo = "P";
                Calendario_View.TipoVisualizacao = 1;
                Calendario_View.PainelLateralFixo = true;
                Calendario_View.RegistraScript_Dashboard_View(dsDashBoard.Tables[6], false);


                StringBuilder sb = new StringBuilder();

                // -------------------------------------------------
                // Grafico NR, ASO e OUTROS

                sb.AppendLine("var morrisDonutNR = Morris.Donut({");
                sb.AppendLine("element: 'GraficoNR',");
                sb.AppendLine("data: [");

                for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[TAB_Grafico_NR].Columns.Count; nLinhasOrigem++)
                {
                    sb.Append("{label: '" + dsDashBoard.Tables[TAB_Grafico_NR].Columns[nLinhasOrigem].ColumnName.ToString() + "',");
                    sb.Append("	value: '" + dsDashBoard.Tables[TAB_Grafico_NR].Rows[0].Field<int>(nLinhasOrigem).ToString() + "'}");

                    if (nLinhasOrigem < dsDashBoard.Tables[TAB_Grafico_NR].Columns.Count - 1)
                        sb.AppendLine(", ");
                }

                sb.AppendLine("],");
                sb.AppendLine("colors:['#FF0000',\"#FFFF00\", \"#0e4ce8\",\"#00FF00\"],");
                sb.AppendLine("resize: true");
                sb.AppendLine("}).on('click', function (i, row) {");
                sb.AppendLine("var idLinha = row.label + \"NR\";");
                sb.AppendLine("abrirModal(idLinha);");
                sb.AppendLine("});");             

                // -------------------------------------------------                
                // Grafico EPI (Sistema Novo)

                sb.AppendLine("var morrisDonutEPINovo = Morris.Donut({");
                sb.AppendLine("element: 'GraficoEPINovo',");
                sb.AppendLine("data: [");

                for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[TAB_Grafico_EPINovo].Columns.Count; nLinhasOrigem++)
                {
                    sb.Append("{label: '" + dsDashBoard.Tables[TAB_Grafico_EPINovo].Columns[nLinhasOrigem].ColumnName.ToString() + "',");
                    sb.Append("	value: '" + dsDashBoard.Tables[TAB_Grafico_EPINovo].Rows[0].Field<int>(nLinhasOrigem).ToString() + "'}");

                    if (nLinhasOrigem < dsDashBoard.Tables[TAB_Grafico_EPINovo].Columns.Count - 1)
                        sb.AppendLine(", ");
                }

                sb.AppendLine("],");
                sb.AppendLine("colors:['#FF0000',\"#FFFF00\", \"#0e4ce8\",\"#00FF00\"],");
                sb.AppendLine("resize: true");
                sb.AppendLine("}).on('click', function (i, row) {");
                sb.AppendLine("var idLinha = row.label + \"EPINovo\";");
                sb.AppendLine("console.log(idLinha)");
                sb.AppendLine("abrirModal(idLinha);");
                sb.AppendLine("});");

                // -------------------------------------------------
                // Grafico Documentos 

                sb.AppendLine("var morrisDonutDoc = Morris.Donut({");
                sb.AppendLine("element: 'GraficoDocumentos',");
                sb.AppendLine("data: [");

                for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[TAB_Grafico_Documentos].Columns.Count; nLinhasOrigem++)
                {
                    sb.Append("{label: '" + dsDashBoard.Tables[TAB_Grafico_Documentos].Columns[nLinhasOrigem].ColumnName.ToString() + "',");
                    sb.Append("	value: '" + dsDashBoard.Tables[TAB_Grafico_Documentos].Rows[0].Field<int>(nLinhasOrigem).ToString() + "'}");

                    if (nLinhasOrigem < dsDashBoard.Tables[TAB_Grafico_Documentos].Columns.Count - 1)
                        sb.AppendLine(", ");
                }

                sb.AppendLine("],");
                sb.AppendLine("colors:['#FF0000',\"#FFFF00\", \"#0e4ce8\",\"#00FF00\"],");
                sb.AppendLine("resize: true");
                sb.AppendLine("}).on('click', function (i, row) {");
                sb.AppendLine("var idLinha = row.label + \"DOC\";");
                sb.AppendLine("abrirModal(idLinha);");
                sb.AppendLine("});");

                // -------------------------------------------------
                // Grafico Plano Saude 

                sb.AppendLine("var morrisDonutPlano = Morris.Donut({");
                sb.AppendLine("element: 'GraficoPlanoSaude',");
                sb.AppendLine("data: [");

                for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[TAB_Grafico_PlanoSaude].Columns.Count; nLinhasOrigem++)
                {
                    sb.Append("{label: '" + dsDashBoard.Tables[TAB_Grafico_PlanoSaude].Columns[nLinhasOrigem].ColumnName.ToString() + "',");
                    sb.Append("	value: '" + dsDashBoard.Tables[TAB_Grafico_PlanoSaude].Rows[0].Field<int>(nLinhasOrigem).ToString() + "'}");

                    if (nLinhasOrigem < dsDashBoard.Tables[TAB_Grafico_PlanoSaude].Columns.Count - 1)
                        sb.AppendLine(", ");
                }

                sb.AppendLine("],");
                sb.AppendLine("colors:['#0e4ce8', '#FF0000', '#00FF00', '#FFFF00', '#FFA500'],");
                sb.AppendLine("resize: true");
                sb.AppendLine("}).on('click', function (i, row) {");
                sb.AppendLine("var idLinha = row.label + \"Plano\";");
                sb.AppendLine("abrirModalPlanoSaude(idLinha);");
                sb.AppendLine("});");

                // -------------------------------------------------
                // Grafico Férias (NOVA IMPLEMENTAÇÃO)
                sb.AppendLine("var morrisDonutFerias = Morris.Donut({");
                sb.AppendLine("element: 'GraficoFerias',");
                sb.AppendLine("data: [");
                for (int nLinhasOrigem = 0; nLinhasOrigem < dsDashBoard.Tables[TAB_Ferias].Columns.Count; nLinhasOrigem++)
                {
                    sb.Append("{label: '" + dsDashBoard.Tables[TAB_Ferias].Columns[nLinhasOrigem].ColumnName.ToString() + "',");
                    sb.Append(" value: '" + dsDashBoard.Tables[TAB_Ferias].Rows[0].Field<int>(nLinhasOrigem).ToString() + "'}");
                    if (nLinhasOrigem < dsDashBoard.Tables[TAB_Ferias].Columns.Count - 1)
                        sb.AppendLine(", ");
                }
                sb.AppendLine("],");
                sb.AppendLine("colors:['#0e4ce8', '#FF0000', '#FFA500', '#00FF00'],");
                sb.AppendLine("resize: true");
                sb.AppendLine("}).on('click', function (i, row) {");
                sb.AppendLine("var idLinha = row.label.replace('(', '').replace(')', '').replace(/ /g, '') + \"Ferias\";");
                sb.AppendLine("abrirModal(idLinha);");
                sb.AppendLine("});");

                // -------------------------------------------------

                sb.Append("$v192('.checksemprevisao').click(function(e) {");
                sb.Append("    e.preventDefault();");
                sb.Append("    __doPostBack('dialog_checksemprevisao', '');");
                sb.Append("});");

                //--------------------------------------------------------------------------------------------------------------------------------
                //--------------------------------------------------------Tabela Aniversário--------------------------------------------------------

                gvAniversarios.DataSource = dsDashBoard.Tables[TAB_Aniversario];
                gvAniversarios.DataBind();

                //--------------------------------------------------------------------------------------------------------------------------------
                //--------------------------------------------------------Tabelas do Modal--------------------------------------------------------

                sb.AppendLine("function abrirModal(idLinha){");
                sb.AppendLine("$.ajax({");
                sb.AppendLine("url: \" /app/Paginas/RRHH/Dashboard_RRHH.aspx/PopularModal\",");
                sb.AppendLine("data: JSON.stringify({ id: idLinha }),");
                sb.AppendLine("type: 'POST',");
                sb.AppendLine("dataType: 'json',");
                sb.AppendLine("contentType: 'application/json; charset=utf-8',");
                sb.AppendLine("success: function(response) {");
                sb.AppendLine("var dados;");
                sb.AppendLine("try{");
                sb.AppendLine("dados = JSON.parse(response.d);}");
                sb.AppendLine("catch (e){");
                sb.AppendLine("console.error(\"Erro no parsing: \", e);}");
                sb.AppendLine("$(\"#modalTitulo\").text(dados.titulo);");
                sb.AppendLine("if ($.fn.DataTable.isDataTable('#tbModal')) {");
                sb.AppendLine("$('#tbModal').DataTable().destroy();}");
                sb.AppendLine("var tbody = $(\"#tbModal tbody\");");
                sb.AppendLine("tbody.empty();");
                sb.AppendLine("$.each(dados.tabela, function(i, item) {");
                sb.AppendLine("var botaoDownload = (item.idArquivo !== \"\" && item.idArquivo !== \"0\") ? \" <td style='text-align: center; vertical - align: middle;'> <button type='button' class='fa-file fa' id='\" + item.idArquivo + \"' onclick='botao(this)' style='border: none'></button></td>\" : \"<td></td>\";");
                sb.AppendLine("var linha = \" <tr> \" +");
                sb.AppendLine("\" <td data-order='\" + converterDataParaOrder(item.dtVencimento) + \"'> \" + formatarData(item.dtVencimento) + \" </td> \" +");
                sb.AppendLine("\" <td> \" + item.sTipo + \" </td> \" +");
                sb.AppendLine("\"<td><a href='\" + item.urlColaborador + \"' target='_blank'>\" + item.sNomeColaborador + \"</a></td>\" +");
                sb.AppendLine("\" <td> \" + item.sDepartamento + \" </td> \" +");
                sb.AppendLine("\" <td> \" + item.sTelefone + \" </td> \" +");
                sb.AppendLine("\" <td data-order='\" + converterDataParaOrder(item.dtEmissao) + \"'> \" + formatarData(item.dtEmissao) + \" </td> \" +");
                sb.AppendLine("botaoDownload +");
                sb.AppendLine("\" </tr> \";");
                sb.AppendLine("tbody.append(linha);");
                sb.AppendLine("});");
                sb.AppendLine("$('#tbModal').DataTable({");
                sb.AppendLine("pageLength:25,");
                sb.AppendLine("\"language\": {");
                sb.AppendLine("\"url\": \"//cdn.datatables.net/plug-ins/1.10.21/i18n/Portuguese-Brasil.json\"},");
                sb.AppendLine("\"destroy\": true,");
                sb.AppendLine("\"order\": [],");
                sb.AppendLine("\"columnDefs\": [");
                sb.AppendLine("{ \"type\": \"date-br\", \"targets\": [0, 5] }"); 
                sb.AppendLine("]");
                sb.AppendLine("});");
                sb.AppendLine("$(\"#modal\").modal(\"show\");}");
                sb.AppendLine("});}");

                sb.AppendLine("function abrirModalPlanoSaude(idLinha){");
                sb.AppendLine("$.ajax({");
                sb.AppendLine("url: \" /app/Paginas/RRHH/Dashboard_RRHH.aspx/PopularModal\",");
                sb.AppendLine("data: JSON.stringify({ id: idLinha }),");
                sb.AppendLine("type: 'POST',");
                sb.AppendLine("dataType: 'json',");
                sb.AppendLine("contentType: 'application/json; charset=utf-8',");
                sb.AppendLine("success: function(response) {");
                sb.AppendLine("var dados;");
                sb.AppendLine("try{");
                sb.AppendLine("dados = JSON.parse(response.d);}");
                sb.AppendLine("catch (e){");
                sb.AppendLine("console.error(\"Erro no parsing: \", e);}");
                sb.AppendLine("$(\"#modalTituloPlanoSaude\").text(dados.titulo);");
                sb.AppendLine("if ($.fn.DataTable.isDataTable('#tbModalPlanoSaude')) {");
                sb.AppendLine("$('#tbModalPlanoSaude').DataTable().destroy();}");
                sb.AppendLine("var tbody = $(\"#tbModalPlanoSaude tbody\");");
                sb.AppendLine("tbody.empty();");
                sb.AppendLine("$.each(dados.tabela, function(i, item) {");
                sb.AppendLine("var botaoDownload = (item.idArquivo !== \"\" && item.idArquivo !== \"0\") ? \" <td style='text-align: center; vertical - align: middle;'> <button type='button' class='fa-file fa' id='\" + item.idArquivo + \"' onclick='botao(this)' style='border: none'></button></td>\" : \"<td></td>\";");
                sb.AppendLine("var linha = \" <tr> \" +");
                sb.AppendLine("\" <td data-order='\" + converterDataParaOrder(item.dtVencimento) + \"'> \" + formatarData(item.dtVencimento) + \" </td> \" +");
                sb.AppendLine("\" <td> \" + item.sTipo + \" </td> \" +");
                sb.AppendLine("\" <td> \" + item.sConvenio + \" </td> \" +");
                sb.AppendLine("\"<td><a href='\" + item.urlColaborador + \"' target='_blank'>\" + item.sNomeColaborador + \"</a></td>\" +");
                sb.AppendLine("\" <td> \" + item.sDepartamento + \" </td> \" +");
                sb.AppendLine("\" <td> \" + item.sTelefone + \" </td> \" +");
                sb.AppendLine("\" <td data-order='\" + converterDataParaOrder(item.dtEmissao) + \"'> \" + formatarData(item.dtEmissao) + \" </td> \" +");
                sb.AppendLine("botaoDownload +");
                sb.AppendLine("\" </tr> \";");
                sb.AppendLine("tbody.append(linha);");
                sb.AppendLine("});");
                sb.AppendLine("$('#tbModalPlanoSaude').DataTable({");
                sb.AppendLine("pageLength:25,");
                sb.AppendLine("\"language\": {");
                sb.AppendLine("\"url\": \"//cdn.datatables.net/plug-ins/1.10.21/i18n/Portuguese-Brasil.json\"},");
                sb.AppendLine("\"destroy\": true,");
                sb.AppendLine("\"order\": [],");
                sb.AppendLine("\"columnDefs\": [");
                sb.AppendLine("{ \"type\": \"date-br\", \"targets\": [0, 5] }");
                sb.AppendLine("]");
                sb.AppendLine("});");
                sb.AppendLine("$(\"#modalPlanoSaude\").modal(\"show\");}");
                sb.AppendLine("});}");

                sb.AppendLine("function converterDataParaOrder(data) {");
                sb.AppendLine("if (!data || data === '') return '';");
                sb.AppendLine("var partes = data.split('/');");
                sb.AppendLine("if (partes.length !== 3) return data;");
                sb.AppendLine("return partes[2] + '-' + partes[1] + '-' + partes[0];"); 
                sb.AppendLine("}");

                sb.AppendLine("function formatarData(data) {");
                sb.AppendLine("return data;"); 
                sb.AppendLine("}");


                //sb.AppendLine("$('#tbModal').DataTable({");
                //sb.AppendLine("pageLength:25,");
                //sb.AppendLine("\"language\": {");
                //sb.AppendLine("\"url\": \"//cdn.datatables.net/plug-ins/1.10.21/i18n/Portuguese-Brasil.json\"},");
                //sb.AppendLine("\"destroy\": true,");
                //sb.AppendLine("\"order\": []");
                //sb.AppendLine("});");
                //sb.AppendLine("$(\"#modal\").modal(\"show\");}");
                //sb.AppendLine("});}");

                sb.AppendLine("function formatarData(dataString){");
                sb.AppendLine("var partes = dataString.split(\"/\");");
                sb.AppendLine("if (partes.length === 3){");
                sb.AppendLine("var dia = partes[0];");
                sb.AppendLine("var mes = partes[1];");
                sb.AppendLine("var ano = partes[2];");
                sb.AppendLine("dia = dia.length === 2 ? dia : \"0\" + dia;");
                sb.AppendLine("mes = mes.length === 2 ? mes : \"0\" + mes;");
                sb.AppendLine("ano = ano.substring(0, 4);");
                sb.AppendLine("return dia + '/' + mes + '/' + ano;}");
                sb.AppendLine("else{");
                sb.AppendLine("return \"\";}}");

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DashBoard_RRHH", sb.ToString(), true);
            }
        }

        private static List<object> BuscarDados(int nTabela)
        {
            List<object> lPopulaGrid = new List<object>();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "POPULA_MODAL" },
                { "@numeroTabela", nTabela.ToString() }
            };
            DataSet dsDashBoard = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsDashBoard))
            {
                if (nTabela >= 15 && nTabela <= 19)
                {
                    foreach (DataRow item in dsDashBoard.Tables[0].Rows)
                    {
                        lPopulaGrid.Add(new
                        {
                            urlColaborador = string.Format("/App/Paginas/RRHH/Colaboradores_Detalhe.aspx?id={0}&sTipo={1}", item["idColaborador"].ToString(), item["sParametroUrl"].ToString()),
                            sNomeColaborador = item["sDscColaborador"].ToString(),
                            sDepartamento = item["sDscDepartamento"].ToString(),
                            sTelefone = item["sTelCelular"].ToString(),
                            sTipo = item["sTipo"].ToString(),
                            sConvenio = item["sDscPlanoSaude"].ToString(),
                            dtEmissao = item["dtEmissao"].ToString(),
                            dtVencimento = item["dtVencimento"].ToString(),
                            idArquivo = item["idArquivo"].ToString()
                        });
                    }
                }
                else
                {
                    foreach (DataRow item in dsDashBoard.Tables[0].Rows)
                    {
                        lPopulaGrid.Add(new
                        {
                            urlColaborador = string.Format("/App/Paginas/RRHH/Colaboradores_Detalhe.aspx?id={0}&sTipo={1}", item["idColaborador"].ToString(), item["sParametroUrl"].ToString()),
                            sNomeColaborador = item["sDscColaborador"].ToString(),
                            sDepartamento = item["sDscDepartamento"].ToString(),
                            sTelefone = item["sTelCelular"].ToString(),
                            sTipo = item["sTipo"].ToString(),
                            dtEmissao = item["dtEmissao"].ToString(),
                            dtVencimento = item["dtVencimento"].ToString(),
                            idArquivo = item["idArquivo"].ToString()
                        });
                    }
                }


                return lPopulaGrid;
            }

            return lPopulaGrid;
        }

        [WebMethod]
        public static string PopularModal(string id)
        {
            object value = new { };
            var json = value;

            switch (id)
            {
                case "VencidoNR":
                    json = new { titulo = "NR, ASO e Outros - Vencidos", tabela = BuscarDados(3) };
                    break;
                case "Vence em 7 diasNR":
                    json = new { titulo = "NR, ASO e Outros - Vence em 7 dias", tabela = BuscarDados(4) };
                    break;
                case "Vence em 30 diasNR":
                    json = new { titulo = "NR, ASO e Outros - Vence em 30 dias", tabela = BuscarDados(5) };
                    break;
                case "OKNR":
                    json = new { titulo = "NR, ASO e Outros - OK", tabela = BuscarDados(6) };
                    break;
                case "VencidoEPI": 
                    json = new { titulo = "EPI (Sistema Antigo) - Vencidos", tabela = BuscarDados(7) };
                    break;
                case "Vence em 7 diasEPI":
                    json = new { titulo = "EPI (Sistema Antigo) - Vence em 7 dias", tabela = BuscarDados(8) };
                    break;
                case "Vence em 30 diasEPI":
                    json = new { titulo = "EPI (Sistema Antigo) - Vence em 30 dias", tabela = BuscarDados(9) };
                    break;
                case "OKEPI": 
                    json = new { titulo = "EPI (Sistema Antigo) - OK", tabela = BuscarDados(10) };
                    break;
                case "VencidoEPINovo":
                    json = new { titulo = "EPI (Sistema Novo) - Vencidos", tabela = BuscarDados(20) };
                    break;
                case "Vence em 7 diasEPINovo":
                    json = new { titulo = "EPI (Sistema Novo) - Vence em 7 dias", tabela = BuscarDados(21) };
                    break;
                case "Vence em 30 diasEPINovo":
                    json = new { titulo = "EPI (Sistema Novo) - Vence em 30 dias", tabela = BuscarDados(22) };
                    break;
                case "OKEPINovo":
                    json = new { titulo = "EPI (Sistema Novo) - OK", tabela = BuscarDados(23) };
                    break;
                case "VencidoDOC":
                    json = new { titulo = "Documentos - Vencidos", tabela = BuscarDados(11) };
                    break;
                case "Vence em 7 diasDOC":
                    json = new { titulo = "Documentos - Vence em 7 dias", tabela = BuscarDados(12) };
                    break;
                case "Vence em 30 diasDOC":
                    json = new { titulo = "Documentos - Vence em 30 dias", tabela = BuscarDados(13) };
                    break;
                case "OKDOC":
                    json = new { titulo = "Documentos - OK", tabela = BuscarDados(14) };
                    break;
                case "AtrasoPlano":
                    json = new { titulo = "Plano Saúde - Atrasados", tabela = BuscarDados(15) };
                    break;
                case "Sem PrevisãoPlano":
                    json = new { titulo = "Plano Saúde - Sem Previsão", tabela = BuscarDados(16) };
                    break;
                case "Vence em 30 diasPlano":
                    json = new { titulo = "Plano Saúde - Vence em 30 dias", tabela = BuscarDados(17) };
                    break;
                case "OkPlano":
                    json = new { titulo = "Plano Saúde - OK", tabela = BuscarDados(18) };
                    break;
                case "Vence + 30 diasPlano":
                    json = new { titulo = "Plano Saúde - Vence + 30 dias", tabela = BuscarDados(19) };
                    break;

                // ----- CASES PARA FÉRIAS -----
                case "AprovadoSupervisorFerias":
                    json = new { titulo = "Férias - Aprovado Supervisor", tabela = BuscarDados(24) };
                    break;
                case "RejeitadosFerias":
                    json = new { titulo = "Férias - Rejeitados", tabela = BuscarDados(25) };
                    break;
                case "Aprovados60DiasFerias":
                    json = new { titulo = "Férias - Aprovados RH (60 Dias)", tabela = BuscarDados(26) };
                    break;
                case "FinalizadosFerias":
                    json = new { titulo = "Férias - Finalizados", tabela = BuscarDados(27) };
                    break;
                // ------------------------------------

                default: 
                    json = "Erro: Dados não encontrados.";
                    break;
            }

            return new JavaScriptSerializer().Serialize(json);
        }

        [WebMethod]
        public static string ArquivoDownload(int idArquivo)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "DOWNLOAD_ARQUIVO" },
                { "@idArquivo", idArquivo.ToString() }
            };
            DataSet dsArquivo = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsArquivo))
                return new JavaScriptSerializer().Serialize(new { Base64 = Convert.ToBase64String((byte[])dsArquivo.Tables[0].Rows[0]["vbArquivo"]), NomeArquivo = RETORNO.DATASET(dsArquivo, dsArquivo.Tables.Count - 1, 0, "sNomeArquivo") });

            return "Erro ao baixar o arquivo: Arquivo não encontrado!";
        }

        protected void gvAniversarios_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DateTime aniversario;
                if (DateTime.TryParse(DataBinder.Eval(e.Row.DataItem, "dtAniversario").ToString(), out aniversario))
                {
                    if (aniversario.Month == DateTime.Today.Month && aniversario.Day == DateTime.Today.Day)
                    {
                        e.Row.CssClass = "success";
                    }
                }

                string sDtAniversario = DataBinder.Eval(e.Row.DataItem, "dtAniversario")?.ToString();

                if (string.IsNullOrEmpty(sDtAniversario))
                    return;

                if (DateTime.TryParseExact(sDtAniversario,"dd/MM", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                {
                    string signo = ObterSigno(data);

                    string iconeUnicode = MapearSigno(signo);

                    TableCell celulaSigno = e.Row.Cells[3]; 

                    celulaSigno.Controls.Clear();

                    var container = new HtmlGenericControl("span");
                    container.Attributes["class"] = "signo-icone";
                    container.InnerHtml = iconeUnicode;

                    celulaSigno.Controls.Add(container);
                    celulaSigno.Controls.Add(new LiteralControl($" {signo}"));
                }

            }
        }

        private string ObterSigno(DateTime data)
        {
            int dia = data.Day;
            int mes = data.Month;

            if ((mes == 3 && dia >= 21) || (mes == 4 && dia <= 19))
                return "Áries";
            if ((mes == 4 && dia >= 20) || (mes == 5 && dia <= 20))
                return "Touro";
            if ((mes == 5 && dia >= 21) || (mes == 6 && dia <= 20))
                return "Gêmeos";
            if ((mes == 6 && dia >= 21) || (mes == 7 && dia <= 22))
                return "Câncer";
            if ((mes == 7 && dia >= 23) || (mes == 8 && dia <= 22))
                return "Leão";
            if ((mes == 8 && dia >= 23) || (mes == 9 && dia <= 22))
                return "Virgem";
            if ((mes == 9 && dia >= 23) || (mes == 10 && dia <= 22))
                return "Libra";
            if ((mes == 10 && dia >= 23) || (mes == 11 && dia <= 21))
                return "Escorpião";
            if ((mes == 11 && dia >= 22) || (mes == 12 && dia <= 21))
                return "Sagitário";
            if ((mes == 12 && dia >= 22) || (mes == 1 && dia <= 19))
                return "Capricórnio";
            if ((mes == 1 && dia >= 20) || (mes == 2 && dia <= 18))
                return "Aquário";
            if ((mes == 2 && dia >= 19) || (mes == 3 && dia <= 20))
                return "Peixes";

            return "";
        }

        private string MapearSigno(string signo)
        {
            switch (signo)
            {
                case "Áries":
                    return "&#9800;";
                case "Touro":
                    return "&#9801;";
                case "Gêmeos":
                    return "&#9802;";
                case "Câncer":
                    return "&#9803;";
                case "Leão":
                    return "&#9804;";
                case "Virgem":
                    return "&#9805;";
                case "Libra":
                    return "&#9806;";
                case "Escorpião":
                    return "&#9807;";
                case "Sagitário":
                    return "&#9808;";
                case "Capricórnio":
                    return "&#9809;";
                case "Aquário":
                    return "&#9810;";
                case "Peixes":
                    return "&#9811;";   
            }

            return "";
        }
    }
}