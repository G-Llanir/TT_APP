<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="FAQ_Consulta.aspx.cs" Inherits="TT_Flow.App.Paginas.FAQ.FAQ_Consulta" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .textoResumido {
            white-space: nowrap;
            overflow-x: hidden;
            text-overflow: ellipsis;
            max-width: 100%;
            max-height: 3ch;
            vertical-align: middle;
            display: flex;
        }

            .textoResumido:hover {
                cursor: pointer;
            }

        /* FAQ */
        .faq-hyperlink {
            color: black;
        }

            .faq-hyperlink:hover {
                text-decoration: none;
                color: black;
            }

        .faq-item {
            width: 100%;
            padding: 10px;
        }

            .faq-item:hover {
                border: 2px double lightskyblue;
            }

        .faq-question {
            font-size: 18px;
            font-weight: 600;
            color: #1ab394;
            display: block;
        }

            .faq-question:hover {
                color: #1ab394;
                cursor: pointer;
            }

        .faq-answer {
            margin-top: 10px;
            background: #f3f3f4;
            border: 1px solid #e7eaec;
            border-radius: 3px;
            padding: 15px;
        }

        .faq-item .tag-list {
            width: 100%;
            max-height: 100px;
            flex-wrap: wrap;
            display: flex;
            overflow: hidden;
        }

        .faq-item .tag-item {
            background: #f3f3f4;
            padding: 2px 6px;
            font-size: 10px;
            text-transform: uppercase;
        }

        .caixaBotao {
        }

            .caixaBotao:hover {
                box-shadow: 3px 3px 7px #337ab7, -3px -3px 7px #337ab7;
                -webkit-transition: box-shadow ease-out 0.1s;
                transition: box-shadow ease-out 0.1s;
            }
    </style>

    <script type="text/javascript">

        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }


        //$(function () {
        //    $('[id*=txtdtCriado]').datepicker({
        //        autoclose: true,
        //        format: 'dd/mm/yyyy',
        //        language: 'pt-BR'
        //    });

        //    $('[id*=txtdtCriado]').mask('99/99/9999');

        //});

    </script>

    <asp:UpdatePanel ID="UpdDetalhe" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="FAQ TecandTec"></asp:Label>
                        <small>FAQ</small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="FAQ" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" runat="server" placeholder="Pesquisar"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:TextBox ID="txtdtCriado" class="form-control" runat="server" placeholder="Data Criado" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-2"></div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlStatus" class="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlCategoria" class="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlDepartamento" class="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:Button ID="cmdNovoTema" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovoFaq_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </div>
                </div>

            </div>

            <asp:Panel ID="pnResultado" runat="server">
                <asp:Repeater ID="rptFAQ" runat="server" OnItemDataBound="rptFaq_idb">
                    <ItemTemplate>
                        <div class="form-stacker">
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:HyperLink ID="hplFaq" runat="server" CssClass="faq-hyperlink" NavigateUrl='<%# string.Format("FAQ_Detalhe.aspx?id={0}", DataBinder.Eval(Container.DataItem, "idFaq")) %>'>
                                        <div class="wrapper wrapper-content animated fadeInRight panel panel-primary">
                                            <div class="faq-item panel-body">
                                                <div class="row">

                                                    <div class="col-md-7">
                                                        <label id="hplksTema" runat="server" class="faq-question" text=""><%#DataBinder.Eval(Container.DataItem, "sTitulo") %></label>
                                                        <label class="textoResumido"><%#DataBinder.Eval(Container.DataItem, "sCorpo")%></label>
                                                        <br />
                                                        <small>Departamento: <strong><%#DataBinder.Eval(Container.DataItem,"sDscDepartamento") %></strong> <i class="fa fa-clock-o"></i> <%#DataBinder.Eval(Container.DataItem,"dtCriado") %></small>
                                                    </div>
                                                    <div class="col-md-3">
                                                        <span class="small font-bold"><%#DataBinder.Eval(Container.DataItem, "sDscCategoria") %></span>

                                                        <div class="tag-list">
                                                            <asp:Repeater ID="rptFAQ_Tags" runat="server">
                                                                <ItemTemplate>
                                                                    <div>
                                                                        <span class="tag-item"><%#DataBinder.Eval(Container.DataItem, "value") %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </div>
                                                    </div>
                                                    <br />
                                                    <div class="col-md-2 text-right">
                                                        <i class="fa fa-comments"></i>
                                                        <asp:Label runat="server" ID="nComentarios" class="small font-bold"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:HyperLink>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
