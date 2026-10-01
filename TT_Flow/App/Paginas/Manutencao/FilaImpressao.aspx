<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="FilaImpressao.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.FilaImpressao" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
        /*modal*/
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

        .bg-tt {
            background-color: #009a22 !important;
        }
        /* Menu Modal */
        .mini-menu {
            background-color: #009a22;
            border-radius: 5px;
            padding: 10px;
        }

            .mini-menu ul {
                list-style-type: none;
                padding: 0;
                display: flex;
            }

            .mini-menu li {
                margin-right: 10px;
            }

                .mini-menu li:last-child {
                    margin-right: 0;
                }

                .mini-menu li a {
                    color: #FFFFFF;
                    text-decoration: none;
                    padding: 5px 10px;
                    display: flex;
                    align-items: center;
                    border-radius: 3px;
                    transition: background-color 0.3s ease;
                }

                    .mini-menu li a:hover {
                        background-color: #8efa5c;
                        color: #009a22;
                    }

        .arrow {
            margin-left: 5px;
        }

        .activeMn {
            background-color: #FFFFFF;
            color: #009a22;
        }
    </style>
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                function MudarBotao() {
                    document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
                }
            </script>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Manutenção</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <div class="col-lg-4">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-lg-2">
                                    <asp:TextBox ID="txtDtImpressao" class="form-control" TextMode="Date" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-lg-2">
                                    <asp:DropDownList id="ddlsFiltroFila" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Todos na Fila" Value="T" selected="True"/>
                                        <asp:ListItem Text="Ativados na Fila" Value="N" />
                                        <asp:ListItem Text="Desativados na Fila" Value="S"/>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-lg-4">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>


                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    <asp:Panel ID="pnResultado" class="" runat="server">
                        <div id="resultado">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="col-lg-12">
                                        <div class="row">
                                            <label class="btn btn-primary" id="lblSelectAll" runat="server">
                                                <asp:CheckBox ID="chkSelectAll" Text=" Selecionar Todos" AutoPostBack="true" OnCheckedChanged="chkSelectAll_CheckedChanged" runat="server" />
                                                <asp:Button ID="cmdCancelarFilas" class="btn btn-danger" runat="server" Text="Desativar Impressões" OnClick="cmdCancelarFilas_Click" />
                                                <asp:Button ID="cmdRecolocarFilas" class="btn btn-success" runat="server" Text="Reativar Impressões" OnClick="cmdRecolocarFilas_Click" />
                                            </label>
                                        </div>
                                        <br />
                                        <div class="row">
                                            <div class="table-responsive">
                                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" DataKeyNames="idEtiqueta" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                                    <Columns>

                                                        <asp:TemplateField HeaderText="+" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="8%" ItemStyle-VerticalAlign="Middle">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkOpcao" CssClass="rowCheckbox" runat="server" Text=' <%#" . " + Eval("idEtiqueta") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <%-- <asp:BoundField DataField="idEtiqueta" HeaderText="ID">
                           <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                           <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                       </asp:BoundField>--%>

                                                        <asp:BoundField DataField="sCodigoBarras" HeaderText="Código Barras">
                                                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <ItemStyle Width="30%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscEtiqueta" HeaderText="Descrição">
                                                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <ItemStyle Width="30%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sCancelado" HeaderText="Cancelado">
                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>


                                                        <asp:BoundField DataField="dtImpressao" HeaderText="Impressão">
                                                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="Ação">
                                                            <ItemTemplate>
                                                                <div style="display: flex; gap: 5px;">
                                                               <asp:LinkButton ID="cmdCancelarImpressao" CssClass="btn btn-sm btn-danger" runat="server"
    OnClick="CancelarImpressao_Click" CommandArgument='<%# Eval("idEtiqueta") %>' 
    ToolTip="Desativar a impressão da etiqueta">
    <i class="fa fa-ban"></i>
</asp:LinkButton>

<asp:LinkButton ID="cmdRecolocarImpressao" CssClass="btn btn-sm btn-success" runat="server"
    OnClick="RecolocarImpressao_Click" CommandArgument='<%# Eval("idEtiqueta") %>' 
    ToolTip="Reativar a impressão da etiqueta">
    <i class="fa fa-exchange"></i>
</asp:LinkButton>
                                                                </div>
                                                            </ItemTemplate>
                                                            <HeaderStyle Width="15%" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:HiddenField ID="hddPageStart" runat="server" />
                                                <asp:HiddenField ID="hddPageLength" runat="server" />
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
                <asp:HiddenField runat="server" ID="hddidEtiqueta" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        // ---------- Checkbox ALL / individuais ----------
        function toggleSelectAll(masterCheckbox) {
            var grid = document.getElementById('<%= dtgvConsulta.ClientID %>');
            if (!grid) return;

            var checkboxes = grid.querySelectorAll('input.rowCheckbox');
            checkboxes.forEach(function (cb) {
                cb.checked = masterCheckbox.checked;
            });
        }

        function updateHeaderCheckbox() {
            var grid = document.getElementById('<%= dtgvConsulta.ClientID %>');
            if (!grid) return;

            var checkboxes = grid.querySelectorAll('input.rowCheckbox');
            var headerCheckbox = grid.querySelector('input[id$="chkSelectAll"]');
            if (!headerCheckbox) return;

            var allChecked = true;
            checkboxes.forEach(function (cb) {
                if (!cb.checked) allChecked = false;
            });

            headerCheckbox.checked = allChecked;
        }

        function wireRowCheckboxEvents() {
            var grid = document.getElementById('<%= dtgvConsulta.ClientID %>');
            if (!grid) return;

            var checkboxes = grid.querySelectorAll('input.rowCheckbox');
            checkboxes.forEach(function (cb) {
                cb.removeEventListener('click', updateHeaderCheckbox);
                cb.addEventListener('click', updateHeaderCheckbox);
            });
        }

        // ---------- Integração com DataTables ----------
        var dtConsulta;            // instância global
        var tableId = '#<%= dtgvConsulta.ClientID %>';

        function storePageInfo() {
            if (!dtConsulta) return;
            var info = dtConsulta.page.info();               // start, length
            document.getElementById('<%= hddPageStart.ClientID  %>').value = info.start;
            document.getElementById('<%= hddPageLength.ClientID %>').value = info.length;
        }

        function hookDataTable() {
            if ($.fn.DataTable.isDataTable(tableId)) {
                dtConsulta = $(tableId).DataTable({ retrieve: true });

                var start = parseInt(document.getElementById('<%= hddPageStart.ClientID %>').value || 0);
                var length = parseInt(document.getElementById('<%= hddPageLength.ClientID %>').value || 10);
                var page = Math.floor(start / length);

                dtConsulta.page(page).draw('page'); // <- volta à página anterior
                storePageInfo();

                dtConsulta.on('draw', storePageInfo);
            } else {
                $(document).one('init.dt', function (e, settings) {
                    if (settings.nTable.id === '<%= dtgvConsulta.ClientID %>') {
                        dtConsulta = new $.fn.dataTable.Api(settings);

                        var start = parseInt(document.getElementById('<%= hddPageStart.ClientID %>').value || 0);
                        var length = parseInt(document.getElementById('<%= hddPageLength.ClientID %>').value || 10);
                        var page = Math.floor(start / length);

                        dtConsulta.page(page).draw('page');
                        storePageInfo();

                        dtConsulta.on('draw', storePageInfo);
                    }
                });
            }
        }

        // ---------- inicialização ----------
        document.addEventListener('DOMContentLoaded', function () {
            wireRowCheckboxEvents();
            hookDataTable();
        });

        // Para UpdatePanel – reconecta depois do post-back parcial
        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                wireRowCheckboxEvents();
                hookDataTable();
            });
        }
    </script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/inputmask/5.0.8/jquery.inputmask.min.js"></script>
</asp:Content>
