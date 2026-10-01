<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Consulta_Colaboradores.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Consulta_Colaboradores" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .label-default {
            background-color: black;
        }

        .modal-body {
            display: flex;
            flex-wrap: nowrap;
            overflow: auto;
            max-height: 100vh;
            max-width: 100%;
            padding: 20px;
            box-sizing: border-box;
        }

        ul.nivel {
            padding: 0 1.5em;
        }

        .nivel {
            display: flex;
            flex-direction: column;
            align-items: center;
            min-width: max-content;
        }

            .nivel > ul {
                display: flex;
                flex-wrap: wrap;
                justify-content: center;
                gap: 1em;
                margin: 40px 0;
                padding: 0;
                list-style: none;
            }

                .nivel > ul::before {
                    content: '';
                    position: absolute;
                    top: -40px;
                    left: 50%;
                    transform: translateX(-50%);
                    width: 1.9px;
                    height: 20px;
                    background-color: #333;
                }

            .nivel ul {
                position: relative;
            }

                .nivel ul::after {
                    content: '';
                    position: absolute;
                    top: -20px;
                    left: 0;
                    right: 0;
                    height: 20px;
                    border-top: 2px solid #333;
                    border-radius: 50px;
                }

                .nivel ul.nivel::after {
                    content: '';
                    position: absolute;
                    top: -20px;
                    left: 50%;
                    transform: translateX(-50%);
                    width: 10px;
                    height: 15px;
                    background-color: #333;
                    clip-path: polygon(45% 0, 45% 70%, 0 70%, 50% 100%, 100% 70%, 65% 70%, 65% 0);
                }

        .colaboradores {
            margin-bottom: 0 !important;
        }

            .colaboradores:not(:has(.nivel)) {
                max-width: calc((15em * 3) + (1em * 3));
            }

            .colaboradores:has(> .nivel):has(> .nivelColaboradores) {
                max-width: calc((15em * 6) + (1em * 6));
            }

        .colaborador:hover {
            cursor: pointer;
        }

        .colaborador {
            border: 3px solid black;
            border-radius: 10px;
            width: 15em;
            height: max-content;
            min-height: 5em;
            overflow: hidden;
            list-style: none;
        }

            .colaborador > a {
                text-decoration: none;
                color: black;
            }

                .colaborador > a:hover {
                    text-decoration: none;
                }

                .colaborador > a > .card {
                    display: flex;
                    flex-wrap: wrap;
                    justify-content: center;
                    text-align: center;
                    font-family: "Segoe UI", Arial, sans-serif;
                    font-size: 1.15em;
                }

                    .colaborador > a > .card > .depto {
                        width: 100%;
                        background-color: black;
                        color: white;
                        font-size: .6em;
                        clip-path: polygon(5% -1px, 95% -1px, 85% 95%, 15% 95%);
                    }

                    .colaborador > a > .card > .icone {
                        width: 20%;
                        align-self: center;
                    }

                    .colaborador > a > .card > .nome_cargo {
                        width: 80%;
                        display: flex;
                        flex-wrap: wrap;
                        justify-content: left;
                        text-align: left;
                    }

                        .colaborador > a > .card > .nome_cargo > .nome {
                            width: 100%;
                        }

                        .colaborador > a > .card > .nome_cargo > .cargo {
                            width: 100%;
                            font-size: .75em;
                            font-weight: 400;
                        }

            .nivel label,
            .colaborador label {
                margin: 0;
            }

                .nivel label:hover,
                .colaborador label:hover {
                    cursor: pointer;
                }

            .colaborador:hover {
                box-shadow: 3px 3px 7px black, -3px -3px 7px black;
            }

            .colaborador.supervisor:hover {
                box-shadow: 3px 3px 7px green, -3px -3px 7px green;
            }

            .colaborador.supervisor {
                border-color: green;
                font-weight: bold;
            }

                .colaborador.supervisor .icone {
                    color: green !important;
                }

                .colaborador.supervisor .depto {
                    background-color: green !important;
                }

            .colaborador .checkColaborador {
                position: absolute;
                display: flex;
                justify-content: center;
                align-items: center;
                cursor: pointer;
                width: 1.5em;
                height: 1.5em;
                border: 3px solid black;
                border-radius: 50%;
                color: black;
                background-color: white;
                margin: -.15em 0 0 13.25em !important;
            }

            .colaborador.supervisor .checkColaborador {
                border-color: green;
                color: green;
            }
    </style>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Colaboradores"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa</h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">

                        <div class="col-lg-4 form-group">
                            <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                        </div>

                        <div class="col-lg-3 form-group">
                            <asp:DropDownList ID="ddlidDepartamento" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                        </div>

                        <div class="col-lg-5 form-group">
                            <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisa(this)" />
                            <asp:Button runat="server" ID="cmdOrganograma" class="btn btn-info" Text="Organograma" />
                        </div>

                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-12">

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <asp:Panel ID="pnResultado" runat="server">
                        <div class="panel panel-primary">
                            <div class="panel-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="dtgvConsulta" class="table  table-striped table-hover "
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                        <Columns>


                                            <asp:BoundField DataField="idColaborador" HeaderText="ID">
                                                <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            
                                            <asp:BoundField DataField="sDscColaborador" HeaderText="Colaborador">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            
<%--                                            <asp:BoundField DataField="sDscCargo" HeaderText="Cargo">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>--%>



                                            <asp:BoundField DataField="sDscDepartamento" HeaderText="Departamento">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sEmail" HeaderText="E-mail">
                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sTelCelular" HeaderText="Telefone Celular">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAniversario" HeaderText="Aniversário">
                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sRamal" HeaderText="Ramal">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>

                </div>
            </div>

        </div>
    </div>

    <div class="modal fade" id="imagemModal">
        <div class="modal-dialog" style="width: 95%;">
            <div class="modal-content">
                <div class="modal-header">

                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3 class="modal-title">Organograma</h3>

                    <div style="margin-top: 10px;">
                        <div class="panel panel-primary">
                            <div class="panel-heading togglePanel" style="cursor: pointer">
                                <h3 class="panel-title"><i class="fa fa-map-marker"></i> Localizador <i class="fa fa-chevron-down" style="float: right;"></i></h3>
                            </div>
                            <div class="panel-body">
                                <div class="col-lg-6 padd-0 input-group">
                                    <asp:TextBox ID="txtColaborador_Organograma" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                    <span class="input-group-btn">
                                        <asp:LinkButton runat="server" ID="cmdAbrir_Selecionados" CssClass="btn btn-info cmdSelecionados">Abrir Selecionados <i class="fa fa-external-link"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" ID="cmdLimpar_Selecionados" CssClass="btn btn-danger cmdLimpa_Selecionados">Limpar Selecionados <i class="fa fa-times"></i></asp:LinkButton>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                <div class="modal-body">

                    <asp:Literal runat="server" ID="divOrganograma_Completo"></asp:Literal>

                </div>
            </div>
        </div>
    </div>

</asp:Content>