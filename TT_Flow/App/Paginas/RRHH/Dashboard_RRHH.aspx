<%@ Page Title="" Language="C#" MasterPageFile="~/app/main.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Dashboard_RRHH.aspx.cs" Inherits="TT_Flow.Dashboards.RRHH.DashboardRRHH" %>

<%@ Register Src="~/app/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb_Pagina" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/Calendario.ascx" TagPrefix="uc1" TagName="Calendario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css"
        integrity="sha512-..." crossorigin="anonymous" referrerpolicy="no-referrer" />

    <style> 
        .fas, .far, .fab {
            font-family: "Font Awesome 6 Free" !important; 
            font-weight: 900 !important;
        }

     /*   .right-section {
            height: auto !important;
        }*/

        @media (min-width: 1200px) {
            .col-lg-12 {
                width: 97% !important; 
                margin: 0 auto !important;
                padding: 0 !important;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="Server">

    <style>
        #GraficoNR path, #GraficoEPINovo path, #GraficoDocumentos path, #GraficoPlanoSaude path {
            cursor: pointer;
        }

        .table-hover tbody tr:hover {
            background-color: #D3D3D3;
        }

        .modal-body {
            max-height: 850px;
            overflow-y: auto;
            overflow-x: auto;
        }

        .modal-largo {
            width: 80% !important;
            max-width: none !important;
        }

        .modal-header-content {
            display: flex;
            align-items: center;
        }

        .modal-logo img {
            max-width: 50px;
            margin-right: 10px;
        }

        .modal-title-container {
            flex-grow: 1;
        }

        .modal-title {
            color: #009A22;
            text-shadow: 2px 2px 2px rgba(0, 0, 0, 0.2);
            font-size: 24px;
            font-weight: bold;
            margin: 0;
        }

        body {
            font-family: Arial, sans-serif;
        }

        .gridViewStyle {
            border-collapse: collapse;
            margin: 20px 0;
            width: 100%;
            box-shadow: 0 2px 15px rgba(0, 0, 0, 0.15);
        }

            .gridViewStyle th, .gridViewStyle td {
                border: 1px solid #ddd;
                padding: 12px 15px;
                text-align: left;
            }

            .gridViewStyle tr:nth-child(even) {
                background-color: #f2f2f2;
            }

            .gridViewStyle tr:hover {
                background-color: #ddd;
            }

            .gridViewStyle a {
                color: #007bff;
                text-decoration: none;
            }

                .gridViewStyle a:hover {
                    text-decoration: underline;
                }



        .table.dataTable thead th {
            background-color: #009A22;
            color: #ffffff
        }

        th {
            width: auto;
            white-space: nowrap;
        }

        #tbNR {
            max-width: 150px;
        }

        .switch {
            position: relative;
            display: inline-block;
            width: 40px;
            height: 24px;
        }

            .switch input {
                opacity: 0;
                width: 0;
                height: 0;
            }

        .slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #ccc;
            transition: .4s;
            border-radius: 24px;
        }

            .slider:before {
                position: absolute;
                content: "";
                height: 16px;
                width: 16px;
                left: 4px;
                bottom: 4px;
                background-color: white;
                transition: .4s;
                border-radius: 50%;
            }

        input:checked + .slider {
            background-color: #2196F3;
        }

        input:focus + .slider {
            box-shadow: 0 0 1px #2196F3;
        }

        input:checked + .slider:before {
            -webkit-transform: translateX(16px);
            transform: translateX(16px);
        }

        [id*="upDashboard"] {
            height: auto !important;
            min-height: 0;
        }

        .dashboard-container {
            display: grid;
            grid-template-columns: 1fr 1fr 2fr 2fr;
            grid-template-rows: 380px 380px;
            gap: 24px;
            padding: 8px;
            background-color: #f5f5f5;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin: 5px;
            align-items: stretch;
            overflow: hidden;
        }

        .dashboard-cell {
            min-width: 0;
            min-height: 0;
            display: flex;
            flex-direction: column;
        }

        .dashboard-cell-span-2 {
            grid-row: span 2;
            display: flex;
            flex-direction: column;
            gap: 24px;
        }

        .dashboard-cell-nr {
            grid-column: 1;
            grid-row: 1;
        }

        .dashboard-cell-epi {
            grid-column: 2;
            grid-row: 1;
        }

        .dashboard-cell-docs {
            grid-column: 1;
            grid-row: 2;
        }

        .dashboard-cell-plano {
            grid-column: 2;
            grid-row: 2;
        }

        .dashboard-cell-aniversarios {
            grid-column: 3;
            grid-row: 1 / span 2;
        }

        .dashboard-cell-lateral {
            grid-column: 4;
            grid-row: 1 / span 2;
            overflow: hidden;
        }

        .dashboard-cell .panel.panel-primary {
            flex: 1 1 auto;
            display: flex;
            flex-direction: column;
            min-height: 0;
            margin-bottom: 0;
            overflow: hidden;
        }

            .dashboard-cell .panel.panel-primary > .panel-heading {
                flex: 0 0 auto;
            }

            .dashboard-cell .panel.panel-primary > .panel-body {
                flex: 1 1 auto;
                min-height: 0;
                display: flex;
                flex-direction: column;
                overflow: hidden;
            }

        .dashboard-plano-heading {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
        }

            .dashboard-plano-heading .panel-title {
                flex: 1 1 auto;
                min-width: 0;
                margin: 0;
            }

            .dashboard-plano-heading .toggle-switch {
                flex: 0 0 auto;
                line-height: 1;
            }

        .dashboard-cell-grafico .grafico-container {
            display: flex;
            flex: 1 1 auto;
            width: 100%;
            min-height: 0;
            height: auto;
            max-height: none;
            overflow: hidden;
        }

        .dashboard-cell-aniversarios .panel-body {
            overflow-x: auto;
            overflow-y: auto;
        }

        .dashboard-lateral-stack {
            flex: 1 1 auto;
            min-height: 0;
            max-height: 100%;
            display: flex;
            flex-direction: column;
            gap: 24px;
            height: 100%;
            overflow: hidden;
        }

        .dashboard-ferias-card {
            flex: 0 0 auto;
        }

            .dashboard-ferias-card .grafico-container {
                display: flex;
                width: 100%;
                height: 240px;
                max-height: 240px;
                min-height: 200px;
                overflow: hidden;
            }

        .grafico-area {
            flex: 1 1 auto;
            min-width: 0;
            width: 100%;
            height: 100%;
            max-height: 100%;
            overflow: hidden;
        }

        #div_Calendario_View {
            display: flex;
            flex-direction: column;
            width: 100%;
            min-height: 0;
            flex: 1 1 auto;
            height: 100%;
            padding-right: 0;
        }

        .dashboard-calendario-lateral {
            width: 100%;
            flex: 1 1 auto;
            min-height: 0;
            display: flex;
            flex-direction: column;
            overflow: hidden;
        }

            .dashboard-calendario-lateral [id*="div_calendario"] {
                flex: 1 1 auto;
                min-height: 0 !important;
                height: 100% !important;
                max-height: 100% !important;
                overflow: hidden;
            }

                .dashboard-calendario-lateral [id*="div_calendario"] .fc {
                    height: 100% !important;
                    max-height: 100%;
                }

        @media (min-width: 1024px) and (max-width: 1399px) {
            .dashboard-container {
                grid-template-columns: 1fr 1fr 1.5fr 1.5fr;
                grid-template-rows: 340px 340px;
                gap: 16px;
            }

            .dashboard-cell-span-2 {
                gap: 16px;
            }

            .dashboard-lateral-stack {
                gap: 16px;
            }
        }

        @media (max-width: 1023px) {
            .dashboard-container {
                grid-template-columns: 1fr 1fr;
                grid-template-rows: none;
                gap: 16px;
            }

            .dashboard-cell-lateral .dashboard-lateral-stack,
            #dashboardCalendarioLateral {
                height: auto !important;
                max-height: none !important;
            }

            .dashboard-cell-nr {
                grid-column: 1;
                grid-row: auto;
            }

            .dashboard-cell-epi {
                grid-column: 2;
                grid-row: auto;
            }

            .dashboard-cell-docs {
                grid-column: 1;
                grid-row: auto;
            }

            .dashboard-cell-plano {
                grid-column: 2;
                grid-row: auto;
            }

            .dashboard-cell-aniversarios {
                grid-column: 1 / -1;
                grid-row: auto;
            }

            .dashboard-cell-lateral {
                grid-column: 1 / -1;
                grid-row: auto;
            }

            .dashboard-cell-span-2 {
                grid-row: auto;
            }

            .dashboard-cell-grafico .grafico-container {
                min-height: 260px;
                max-height: 300px;
            }

        }

        @media (max-width: 767px) {
            .dashboard-container {
                grid-template-columns: 1fr;
            }

            .dashboard-cell-nr,
            .dashboard-cell-epi,
            .dashboard-cell-docs,
            .dashboard-cell-plano,
            .dashboard-cell-aniversarios,
            .dashboard-cell-lateral {
                grid-column: 1;
            }
        }

        .signo-icone {
            font-size: 1.3em;
            font-weight: bold;
            font-family: "Segoe UI Symbol";
            margin-right: 6px;
        }
    </style>

    <uc1:Manual runat="server" ID="manual" />

    <div class="row">
        <div class="col-lg-9">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Dashboard RRHH"></asp:Label>
            </h1>
        </div>
        <div class="col-lg-12">
            <uc1:BreadCrumb_Pagina runat="server" ID="BreadCrumb_Pagina" NivelPagina="1" TitulodaPagina="DashBoard RRHH" />
        </div>
    </div>

    <asp:UpdatePanel ID="upDashboard" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="dashboard-container" runat="server" id="div_dashboardContainer">

                <div class="dashboard-cell dashboard-cell-grafico dashboard-cell-nr">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i> NR, ASO e Outros</h3>
                        </div>
                        <div class="panel-body">
                            <div class="grafico-container">
                                <div id="GraficoNR" class="grafico-area"></div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="dashboard-cell dashboard-cell-grafico dashboard-cell-epi">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i> EPI</h3>
                        </div>
                        <div class="panel-body">
                            <div class="grafico-container">
                                <div id="GraficoEPINovo" class="grafico-area"></div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="dashboard-cell dashboard-cell-aniversarios dashboard-cell-span-2" runat="server" id="divPanelAniversarios">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-birthday-cake"></i>&nbsp; Aniversários dos próximos 30 dias</h3>
                        </div>
                        <div class="panel-body">
                            <asp:GridView ID="gvAniversarios" class="table table-striped table-bordered table-hover table-condensed"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvAniversarios_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="sDscColaborador" HeaderText="Colaborador">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dtAniversario" HeaderText="Data">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDiaSemana" HeaderText="Dia da Semana">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sSigno" HeaderText="Horóscopo">
                                        <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>

                <div class="dashboard-cell dashboard-cell-lateral dashboard-cell-span-2">
                    <div class="dashboard-lateral-stack">
                        <div id="div_Calendario_View">
                            <div id="dashboardCalendarioLateral" class="dashboard-calendario-lateral">
                                <uc1:Calendario runat="server" ID="Calendario_View" />
                            </div>
                        </div>

                        <div class="dashboard-ferias-card">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i> Férias - 60 Dias</h3>
                                </div>
                                <div class="panel-body">
                                    <div class="grafico-container">
                                        <div id="GraficoFerias" class="grafico-area"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="dashboard-cell dashboard-cell-grafico dashboard-cell-docs">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i> Documentos</h3>
                        </div>
                        <div class="panel-body">
                            <div class="grafico-container">
                                <div id="GraficoDocumentos" class="grafico-area"></div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="dashboard-cell dashboard-cell-grafico dashboard-cell-plano">
                    <div class="panel panel-primary">
                        <div class="panel-heading dashboard-plano-heading">
                            <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i> Plano Saúde</h3>
                            <div class="toggle-switch" runat="server" id="Div1">
                                <label class="switch" title="Exibir sem previsão">
                                    <input type="checkbox" id="switchPag" onchange="updateGraphMode();" class="checksemprevisao" runat="server">
                                    <span class="slider round"></span>
                                </label>
                            </div>
                        </div>
                        <div class="panel-body">
                            <div class="grafico-container">
                                <div id="GraficoPlanoSaude" class="grafico-area"></div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>



            <div id="data"></div>

            <div class="modal fade" id="modal" tabindex="-1" role="dialog">
                <div class="modal-dialog modal-dialog-centered modal-largo" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <div class="modal-header-content">
                                <div class="modal-logo">
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                                </div>
                                <div class="modal-title-container">
                                    <h4 class="modal-title" id="modalTitulo"></h4>
                                </div>
                            </div>
                            <div class="modal-body">
                                <div class="scrollable-grid">
                                    <div class="form-stacked row">
                                        <div class="col-lg-12 table-responsive">
                                            <table id="tbModal" class="table table-striped table-bordered table-hover gridViewStyle">
                                                <thead>
                                                    <tr>
                                                        <th style="max-width: 20px;">Data Vencimento</th>
                                                        <th>Tipo</th>
                                                        <th>Nome Colaborador </th>
                                                        <th>Departamento</th>
                                                        <th>Telefone</th>
                                                        <th style="max-width: 20px;">Data Emissão</th>
                                                        <th style="max-width: 8px;">Arquivo</th>
                                                    </tr>
                                                </thead>
                                                <tbody></tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="modalPlanoSaude" tabindex="-1" role="dialog">
                <div class="modal-dialog modal-dialog-centered modal-largo" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <div class="modal-header-content">
                                <div class="modal-logo">
                                    <asp:Image ID="Image2" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                                </div>
                                <div class="modal-title-container">
                                    <h4 class="modal-title" id="modalTituloPlanoSaude"></h4>
                                </div>
                            </div>
                            <div class="modal-body">
                                <div class="scrollable-grid">
                                    <div class="form-stacked row">
                                        <div class="col-lg-12 table-responsive">
                                            <table id="tbModalPlanoSaude" class="table table-striped table-bordered table-hover gridViewStyle">
                                                <thead>
                                                    <tr>
                                                        <th style="max-width: 20px;">Data Vencimento</th>
                                                        <th>Tipo</th>
                                                        <th>Convênio/Plano</th>
                                                        <th>Nome Colaborador/Dependente </th>
                                                        <th>Departamento</th>
                                                        <th>Telefone</th>
                                                        <th style="max-width: 20px;">Data Emissão</th>
                                                        <th style="max-width: 8px;">Arquivo</th>
                                                    </tr>
                                                </thead>
                                                <tbody></tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>


        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="timer_Atualizar" EventName="Tick" />
        </Triggers>
    </asp:UpdatePanel>

    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

    <script type="text/javascript">

        //function pageLoad(sender, args) {
        //    if (args.get_isPartialLoad()) {
        //        aplicarManipuladorModal();
        //    } else {
        //        aplicarManipuladorModal();
        //    }
        //}

        function formatarData(dataString) {
            var partes = dataString.split("/");

            if (partes.length === 3) {
                var dia = partes[0];
                var mes = partes[1];
                var ano = partes[2];

                dia = dia.length === 2 ? dia : "0" + dia;
                mes = mes.length === 2 ? mes : "0" + mes;
                ano = ano.substring(0, 4);
                return dia + '/' + mes + '/' + ano;
            } else {
                return "Formato de data inválido";
            }
        }

        function converterDataParaOrder(data) {
            if (!data || data == '') {
                return '';
            }
            var partes = data.split('/');
            if (partes.length !== 3) {
                return data;
            }
            return partes[2] + '-' + partes[1] + "-" + partes[0];
        }

        function ajax(idLinha) {
            console.log(idLinha);
            $.ajax({
                url: "/app/Paginas/RRHH/Dashboard_RRHH.aspx/PopularModal",
                data: JSON.stringify({ id: idLinha }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (response) {
                    var dados;
                    try {
                        dados = JSON.parse(response.d);
                    } catch (e) {
                        console.error("Erro no parsing:", e);
                    }
                    $("#modalTitulo").text(dados.titulo);

                    if ($.fn.DataTable.isDataTable('#tbModal')) {
                        $('#tbModal').DataTable().destroy();
                    }
                    var tbody = $("#tbModal tbody");
                    tbody.empty();
                    $.each(dados.tabela, function (i, item) {
                        var botaoDownload = (item.idArquivo !== "" && item.idArquivo !== "0") ? "<td style='text-align: center; vertical - align: middle;'> <button type='button' class='fa-file fa' id='" + item.idArquivo + "' onclick='botao(this)' style='border: none'></button></td>" : "<td></td>";
                        var linha = "<tr>" +
                            "<td data-order='converterDataParaOrder(" + item.dtVencimento + ")'>" + formatarData(item.dtVencimento) + "</td>" +
                            "<td>" + item.sTipo + "</td>" +
                            "<td><a href='" + item.urlColaborador + "' target='_blank'>" + item.sNomeColaborador + "</a></td>" +
                            "<td>" + item.sDepartamento + "</td>" +
                            "<td>" + item.sTelefone + "</td>" +
                            "<td data-order='converterDataParaOrder(" + item.dtEmissao + ")'>" + formatarData(item.dtEmissao) + "</td>" +
                            botaoDownload +
                            "</tr>";
                        tbody.append(linha);
                    });

                    $('#tbModal').DataTable({
                        pageLength: 25,
                        "language": {
                            "url": "//cdn.datatables.net/plug-ins/1.10.21/i18n/Portuguese-Brasil.json"
                        },
                        "destroy": true,
                        "order": [],
                        "columDefs": [
                            { "type": "date-br", "targets": [0, 5] }
                        ]
                    });

                    $("#modal").modal("show");
                }

            });

        }

        function ajaxPlanoSaude(idLinha) {
            console.log(idLinha);
            $.ajax({
                url: "/app/Paginas/RRHH/Dashboard_RRHH.aspx/PopularModal",
                data: JSON.stringify({ id: idLinha }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (response) {
                    var dados;
                    try {
                        dados = JSON.parse(response.d);
                    } catch (e) {
                        console.error("Erro no parsing:", e);
                    }
                    $("#modalTituloPlanoSaude").text(dados.titulo);

                    if ($.fn.DataTable.isDataTable('#tbModalPlanoSaude')) {
                        $('#tbModalPlanoSaude').DataTable().destroy();
                    }
                    var tbody = $("#tbModalPlanoSaude tbody");
                    tbody.empty();
                    $.each(dados.tabela, function (i, item) {
                        var botaoDownload = (item.idArquivo !== "" && item.idArquivo !== "0") ? "<td style='text-align: center; vertical - align: middle;'> <button type='button' class='fa-file fa' id='" + item.idArquivo + "' onclick='botao(this)' style='border: none'></button></td>" : "<td></td>";
                        var linha = "<tr>" +
                            "<td data-order='converterDataParaOrder(" + item.dtVencimento + ")'>" + formatarData(item.dtVencimento) + "</td>" +
                            "<td>" + item.sTipo + "</td>" +
                            "<td>" + item.sConvenio + "</td>" +
                            "<td><a href='" + item.urlColaborador + "' target='_blank'>" + item.sNomeColaborador + "</a></td>" +
                            "<td>" + item.sDepartamento + "</td>" +
                            "<td>" + item.sTelefone + "</td>" +
                            "<td data-order='converterDataParaOrder(" + item.dtEmissao + ")'>" + formatarData(item.dtEmissao) + "</td>" +
                            botaoDownload +
                            "</tr>";
                        tbody.append(linha);
                    });

                    $('#tbModalPlanoSaude').DataTable({
                        pageLength: 25,
                        "language": {
                            "url": "//cdn.datatables.net/plug-ins/1.10.21/i18n/Portuguese-Brasil.json"
                        },
                        "destroy": true,
                        "order": [],
                        "columDefs": [
                            { "type": "date-br", "targets": [0, 5] }
                        ]
                    });

                    $("#modalPlanoSaude").modal("show");
                }

            });

        }

        //function ajax(idLinha) {
        //    $.ajax({
        //        url: "/app/Paginas/RRHH/Dashboard_RRHH.aspx/PopularModal",
        //        data: JSON.stringify({ id: idLinha }),
        //        type: 'POST',
        //        dataType: 'json',
        //        contentType: 'application/json; charset=utf-8',
        //        success: function (response) {
        //            var dados;
        //            try {
        //                dados = JSON.parse(response.d);
        //            } catch (e) {
        //                console.error("Erro no parsing:", e);
        //            }
        //            $("#modalTitulo").text(dados.titulo);

        //            var existeDscPlano = dados.tabela.some(item => item.sDscPlano && item.sDscPlano.trim() !== "");
        //            var $theadTr = $('#tbModal thead tr');
        //            var $planoTh = $theadTr.find('th[data-column="sDscPlano"]');

        //            console.log(existeDscPlano);
        //            console.log($planoTh.length);
        //            if (existeDscPlano && $planoTh.length === 0) {
        //                $theadTr.append('<th data-column="sDscPlano">Plano</th>');
        //            } else if (!existeDscPlano && $planoTh.length > 0) {
        //                $planoTh.remove();
        //            }

        //            console.log($theadTr.value + " -- " + $planoTh.value);

        //            //if ($.fn.DataTable.isDataTable('#tbModal')) {
        //            //    $('#tbModal').DataTable().destroy();
        //            //}
        //            var tbody = $("#tbModal tbody");
        //            tbody.empty();
        //            $.each(dados.tabela, function (i, item) {
        //                var botaoDownload = (item.idArquivo !== "" && item.idArquivo !== "0") ? "<td style='text-align: center; vertical - align: middle;'> <button type='button' class='fa-file fa' id='" + item.idArquivo + "' onclick='botao(this)' style='border: none'></button></td>" : "<td></td>";
        //                var sDscPlano = item.sDscPlano !== "" ? "<td>" + item.sDscPlano + "</td>" : "";
        //                var linha = "<tr>" +
        //                    "<td>" + formatarData(item.dtVencimento) + "</td>" +
        //                    "<td>" + item.sTipo + "</td>" +
        //                    "<td><a href='" + item.urlColaborador + "' target='_blank'>" + item.sNomeColaborador + "</a></td>" +
        //                    "<td>" + item.sDepartamento + "</td>" +
        //                    "<td>" + item.sTelefone + "</td>" +
        //                    "<td>" + formatarData(item.dtEmissao) + "</td>" +
        //                    sDscPlano +
        //                    botaoDownload +
        //                    "</tr>";
        //                tbody.append(linha);
        //            });

        //            //$('#tbModal').DataTable({

        //            //    pageLength: 25,
        //                  responsive: true
        //            //    "language": {
        //            //        "url": "//cdn.datatables.net/plug-ins/1.10.21/i18n/Portuguese-Brasil.json"
        //            //    },
        //            //    "order": []
        //            //});

        //            $("#modal").modal("show");
        //        }
        //    });

        //}

        <%--function aplicarManipuladorModal() {
            $("#<%= rowNrVencido.ClientID %>").css("cursor", "pointer");
            $("#<%= rowNrVencido.ClientID %>").click(function () {
                var idLinha = 'VencidoNR';
                ajax(idLinha);
            });


            $("#<%= rowNrVence7.ClientID %>").css("cursor", "pointer");
            $("#<%= rowNrVence7.ClientID %>").click(function () {
                var idLinha = 'Vence em 7 diasNR';
                ajax(idLinha);
            });

            $("#<%= rowNrVence30.ClientID %>").css("cursor", "pointer");
            $("#<%= rowNrVence30.ClientID %>").click(function () {
                var idLinha = 'Vence em 30 diasNR';
                ajax(idLinha);
            });

            $("#<%= rowNROk.ClientID %>").css("cursor", "pointer");
            $("#<%= rowNROk.ClientID %>").click(function () {
                var idLinha = 'OKNR';
                ajax(idLinha);
            });

            $("#<%= rowEpiNovoVencido.ClientID %>").css("cursor", "pointer");
            $("#<%= rowEpiNovoVencido.ClientID %>").click(function () {
                var idLinha = 'VencidoEPINovo';
                ajax(idLinha);

            });
            $("#<%= rowEpiNovoVence7.ClientID %>").css("cursor", "pointer");
            $("#<%= rowEpiNovoVence7.ClientID %>").click(function () {
                var idLinha = 'Vence em 7 diasEPINovo';
                ajax(idLinha);
            });

            $("#<%= rowEpiNovoVence30.ClientID %>").css("cursor", "pointer");
            $("#<%= rowEpiNovoVence30.ClientID %>").click(function () {
                var idLinha = 'Vence em 30 diasEPINovo';
                ajax(idLinha);
            });

            $("#<%= rowEpiNovoOk.ClientID %>").css("cursor", "pointer");
            $("#<%= rowEpiNovoOk.ClientID %>").click(function () {
                var idLinha = 'OKEPINovo';
                ajax(idLinha);
            });


            $("#<%= rowDocVencido.ClientID %>").css("cursor", "pointer");
            $("#<%= rowDocVencido.ClientID %>").click(function () {
                var idLinha = 'VencidoDOC';
                ajax(idLinha);
            });

            $("#<%= rowDocVence7.ClientID %>").css("cursor", "pointer");
            $("#<%= rowDocVence7.ClientID %>").click(function () {
                var idLinha = 'Vence em 7 diasDOC';
                ajax(idLinha);
            });

            $("#<%= rowDocVence30.ClientID %>").css("cursor", "pointer");
            $("#<%= rowDocVence30.ClientID %>").click(function () {
                var idLinha = 'Vence em 30 diasDOC';
                ajax(idLinha);
            });

            $("#<%= rowDocOk.ClientID %>").css("cursor", "pointer");
            $("#<%= rowDocOk.ClientID %>").click(function () {
                var idLinha = 'OKDOC';
                ajax(idLinha);
            });

            $("#<%= rowPlanoVencido.ClientID %>").css("cursor", "pointer");
            $("#<%= rowPlanoVencido.ClientID %>").click(function () {
                var idLinha = 'AtrasoPlano';
                ajaxPlanoSaude(idLinha);
            });

            $("#<%= rowPlanoVence7.ClientID %>").css("cursor", "pointer");
            $("#<%= rowPlanoVence7.ClientID %>").click(function () {
                var idLinha = 'Sem PrevisãoPlano';
                ajaxPlanoSaude(idLinha);
            });

            $("#<%= rowPlanoVence30.ClientID %>").css("cursor", "pointer");
            $("#<%= rowPlanoVence30.ClientID %>").click(function () {
                var idLinha = 'Vence em 30 diasPlano';
                ajaxPlanoSaude(idLinha);
            });

            $("#<%= rowPlanoVenceMais30.ClientID %>").css("cursor", "pointer");
            $("#<%= rowPlanoVenceMais30.ClientID %>").click(function () {
                var idLinha = 'Vence + 30 diasPlano';
                ajaxPlanoSaude(idLinha);
            });

            $("#<%= rowPlanoOk.ClientID %>").css("cursor", "pointer");
            $("#<%= rowPlanoOk.ClientID %>").click(function () {
                var idLinha = 'OkPlano';
                ajaxPlanoSaude(idLinha);
            });

            // ========== INÍCIO DA NOVA IMPLEMENTAÇÃO: JAVASCRIPT DE FÉRIAS ==========
            $("#<%= TableRow1.ClientID %>").css("cursor", "pointer").off('click').on('click', function () { ajax('AprovadoSupervisorFerias'); });
            $("#<%= TableRow2.ClientID %>").css("cursor", "pointer").off('click').on('click', function () { ajax('RejeitadosFerias'); });
            $("#<%= TableRow3.ClientID %>").css("cursor", "pointer").off('click').on('click', function () { ajax('Aprovados60 DiasFerias'); });
            $("#<%= TableRow4.ClientID %>").css("cursor", "pointer").off('click').on('click', function () { ajax('FinalizadosFerias'); });
            // ========== FIM DA NOVA IMPLEMENTAÇÃO ==========
        }--%>

        function botao(id, name) {
            var idArquivo = id.id;
            $.ajax({
                url: "/app/Paginas/RRHH/Dashboard_RRHH.aspx/ArquivoDownload",
                type: "POST",
                data: JSON.stringify({ idArquivo: idArquivo }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var resposta = JSON.parse(response.d);
                    var base64Arquivo = resposta.Base64;
                    var nomeDoArquivo = resposta.NomeArquivo;
                    var url = "data:application/octet-stream;base64," + base64Arquivo;

                    $("<a />", {
                        "href": url,
                        "download": nomeDoArquivo,
                        "text": ""
                    }).appendTo("body")[0].click();

                },
                error: function (error) {
                    console.error("Erro ao fazer download do arquivo", error);
                }
            });

        }

        function dashboardCalendarioLateral_TrancarLayout() {
            var $cell = $('.dashboard-cell-lateral');
            var $stack = $cell.find('.dashboard-lateral-stack');
            var $calendario = $('#dashboardCalendarioLateral');
            if (!$cell.length || !$stack.length || !$calendario.length) {
                return;
            }

            if (window.innerWidth < 1024) {
                $stack.add($calendario).css({
                    height: '',
                    maxHeight: '',
                    overflow: ''
                });
                $calendario.css('flex', '');
                return;
            }

            var cellHeight = $cell.innerHeight();
            if (cellHeight <= 0) {
                return;
            }

            $stack.css({
                height: cellHeight + 'px',
                maxHeight: cellHeight + 'px',
                overflow: 'hidden'
            });

            var feriasH = $stack.find('.dashboard-ferias-card').outerHeight(true) || 0;
            var stackStyle = window.getComputedStyle($stack[0]);
            var stackGap = parseFloat(stackStyle.rowGap || stackStyle.gap) || 0;
            var calendarioHeight = Math.max(140, cellHeight - feriasH - stackGap);
            $calendario.css({
                height: calendarioHeight + 'px',
                maxHeight: calendarioHeight + 'px',
                flex: '0 0 auto'
            });
        }

        function dashboardCalendarioLateral_Redimensionar() {
            dashboardCalendarioLateral_TrancarLayout();

            var divCalendario = document.querySelector('#dashboardCalendarioLateral [id*="div_calendario"]');
            if (!divCalendario || !divCalendario._calendar) {
                return;
            }

            divCalendario.style.height = '100%';
            divCalendario.style.maxHeight = '100%';
            divCalendario._calendar.updateSize();
        }

        function dashboardCalendarioLateral_AgendarLayout() {
            setTimeout(function () {
                dashboardCalendarioLateral_TrancarLayout();
                dashboardCalendarioLateral_Redimensionar();
            }, 300);
        }

        $(function () {
            dashboardCalendarioLateral_AgendarLayout();

            var resizeTimer;
            $(window).off('resize.dashboardRrhh').on('resize.dashboardRrhh', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    dashboardCalendarioLateral_TrancarLayout();
                    dashboardCalendarioLateral_Redimensionar();
                }, 150);
            });
        });

        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                dashboardCalendarioLateral_AgendarLayout();
            });
        }

    </script>

</asp:Content>
