<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Impressoras.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Impressoras" %>

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
                                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Todos Status" Value="T" />
                                        <asp:ListItem Text="Ativas" Value="S" Selected="True" />
                                        <asp:ListItem Text="Desativadas" Value="N" />
                                    </asp:DropDownList>
                                </div>
                                <div class="col-lg-5">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
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
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" DataKeyNames="idImpressora" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                            <Columns>

                                                <%--<asp:HyperLinkField DataNavigateUrlFields="idImpressora"
                                                    DataTextField="idImpressora" HeaderText="ID"
                                                    DataNavigateUrlFormatString="Tarefas_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="8%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>--%>
                                                <asp:BoundField DataField="idImpressora" HeaderText="ID">
                                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <%--                                                <asp:HyperLinkField DataNavigateUrlFields="idImpressora"
                                                    DataTextField="sDscImpressora" HeaderText="Tarefa"
                                                    DataNavigateUrlFormatString="Tarefas_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="35%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>--%>

                                                <asp:BoundField DataField="sDscImpressora" HeaderText="Descrição">
                                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <ItemStyle Width="30%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sIP" HeaderText="IP">
                                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nPorta" HeaderText="Porta">
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                                    <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Ação">
                                                    <ItemTemplate>
                                                        <div style="display: flex; gap: 5px;">
                                                            <asp:LinkButton ID="cmdEditarImpressora" CssClass="btn btn-sm btn-warning" runat="server"
                                                                OnClick="EditarImpressora_Click" CommandArgument='<%# Eval("idImpressora") %>'>
                                                                <i class="fa fa-pencil"></i>
                                                            </asp:LinkButton>
                                                            <asp:LinkButton ID="cmdDesativarImpressora" CssClass="btn btn-sm btn-danger" runat="server"
                                                                OnClick="DesativarImpressora_Click" CommandArgument='<%# Eval("idImpressora") %>' ToolTip="Desativar Impressora">
                                                                <i class="fa fa-ban"></i>
                                                            </asp:LinkButton>
                                                            <asp:LinkButton ID="cmdAtivarImpressora" CssClass="btn btn-sm btn-success" runat="server"
                                                                OnClick="AtivarImpressora_Click" CommandArgument='<%# Eval("idImpressora") %>'  ToolTip="Ativar Impressora">
                                                                <i class="fa fa-check-circle" ></i>
                                                            </asp:LinkButton>
                                                        </div>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="15%" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sAtivo" HeaderText="sAtivo" Visible="false">
                                                    <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="modal fade" id="modalForm" tabindex="-1" role="dialog" aria-labelledby="modalFormLabel" aria-hidden="true" data-backdrop="static">
                            <asp:UpdatePanel ID="updModal" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="modal-dialog modal-lg" role="document">
                                        <div class="modal-content">
                                            <div class="modal-header">

                                                <h4 class="modal-title" id="lblModal">
                                                    <asp:Label ID="lbltituloModal" Text="Impressoras - Cadastro/Atualização" runat="server" />
                                                </h4>
                                            </div>
                                            <div class="modal-body">
                                                <div class="panel-body">

                                                    <div id="Div_Menu" runat="server" class="row">
                                                        <div class="mini-menu col-lg-12">
                                                            <ul>
                                                                <li>
                                                                    <asp:LinkButton ID="cmdInicio" runat="server" OnClientClick="toggleVisibility('cphCorpo_Div_Forms'); return false;"><b>Impressoras</b><span class="arrow">&rsaquo;</span></asp:LinkButton>
                                                                </li>
                                                            </ul>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12">
                                                        <br />
                                                        <div class="panel panel-default" runat="server" id="div_Impressora">
                                                            <div class="panel-body">
                                                                <div class="col-lg-12">
                                                                    <div id="Div_Forms" class="row" runat="server" style="display: block;">
                                                                        <div class="row">
                                                                            <div class="col-lg-12">
                                                                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaModal" />
                                                                            </div>
                                                                            <div class="col-lg-12">

                                                                                <div class="form-row">
                                                                                    <div class="row">
                                                                                        <div class="form-group col-lg-4">
                                                                                            <label id="lblTituloImressora" runat="server" for="txtsDscImpressora">Impressora</label>
                                                                                            <asp:TextBox ID="txtsImpressora" class="form-control" runat="server" placeholder="Nome da Impressora"></asp:TextBox>
                                                                                        </div>
                                                                                        <div class="form-group col-lg-3">
                                                                                            <label for="txtsIP">IP:</label>
                                                                                            <asp:TextBox ID="txtsIP" class="form-control" runat="server" placeholder="192.168.00.00"></asp:TextBox>
                                                                                        </div>
                                                                                        <div class="form-group col-lg-2" runat="server">
                                                                                            <label for="txtnPorta">Nº Porta:</label>
                                                                                            <asp:TextBox ID="txtnPorta" class="form-control" runat="server" placeholder="0000" TextMode="Number"></asp:TextBox>
                                                                                        </div>
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <%--observação--%>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                            </div>
                                                        </div>

                                                    </div>

                                                </div>
                                            </div>
                                            <div class="modal-footer">
                                                <asp:Button ID="cmdSalvar" Text="Salvar" runat="server" CssClass="btn btn-success" OnClick="SalvarImpressora_Click" />
                                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>

                    </asp:Panel>
                </div>
                <asp:HiddenField runat="server" ID="hddidImpressora" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/inputmask/5.0.8/jquery.inputmask.min.js"></script>
    <script type="text/javascript">
        function toggleVisibility(divId) {
            var divs = ['cphCorpo_Div_Forms'];

            divs.forEach(function (id) {
                var div = document.getElementById(id);
                if (id === divId) {
                    div.style.display = "block";
                } else {
                    div.style.display = "none";
                }
            });
        }
        function aplicarMascaras() {
            const ipField = document.getElementById('<%= txtsIP.ClientID %>');
            const portaField = document.getElementById('<%= txtnPorta.ClientID %>');

            if (ipField) {
                ipField.addEventListener('input', function () {
                    let valor = this.value.replace(/\D/g, '').substring(0, 12);
                    let formatado = '';
                    for (let i = 0; i < valor.length; i++) {
                        if (i === 3 || i === 6 || i === 9) formatado += '.';
                        formatado += valor[i];
                    }
                    this.value = formatado;
                });
            }

            if (portaField) {
                if (portaField.value !== "") {
                    return;
                }

                portaField.value = "0000";

                portaField.addEventListener('focus', function () {
                    if (this.value === "0000") this.value = "";
                });

                portaField.addEventListener('input', function () {
                    let valor = this.value.replace(/\D/g, '').substring(0, 4);
                    this.value = valor;
                });

                portaField.addEventListener('blur', function () {
                    let valor = this.value.replace(/\D/g, '');
                    this.value = valor.padStart(4, '0');
                });
            }
        }

        window.addEventListener('load', aplicarMascaras);

        if (typeof Sys !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(aplicarMascaras);
        }
    </script>
</asp:Content>
