<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Forum_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Forum.Forum_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HtmlEditor" TagPrefix="HTMLEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .box {
            background: #fff;
            padding: 30px;
            margin: 0 0 24px 0
        }

            .box.box-without-padding {
                padding: 0
            }

            .box.box-without-sidepadding {
                padding: 8px 0
            }

                .box.box-without-sidepadding .boxTitle {
                    margin-left: 30px
                }

            .box.box-without-bottom-padding {
                padding-bottom: 0
            }

            .box .tableWrap {
                margin: 0 -30px
            }

            .box .table-responsive {
                width: auto
            }

        .simpleListings {
            padding: 0;
            margin: 0
        }

            .simpleListings li {
                list-style-type: none;
                padding: 0 0 20px 0;
                margin: 20px 0 0 0;
                border-bottom: 1px solid #e6e7ed;
                position: relative
            }

                .simpleListings li:first-child {
                    margin-top: 0
                }

                .simpleListings li:only-child {
                    border-bottom: 0
                }

                .simpleListings li .title {
                    font-size: 14px;
                    font-size: 1.4rem;
                    font-weight: 700;
                    text-transform: uppercase;
                    margin: 0 0 2px 0
                }

                    .simpleListings li .title span {
                        font-weight: 400;
                        text-transform: none
                    }

                    .simpleListings li .title a:hover {
                        color: #fe5621
                    }

                .simpleListings li .info {
                    color: #919599;
                    font-style: italic
                }

                .simpleListings li p {
                    margin: 7px 0 0 0
                }

                .simpleListings li img {
                    margin: 20px 0
                }

        .userActivities {
            margin-bottom: 25px
        }

            .userActivities + .showMore {
                margin: 0 0 0 70px
            }

            .userActivities .i {
                margin-top: 25px;
                position: relative
            }

                .userActivities .i:first-child {
                    margin-top: 0
                }

                .userActivities .i .image {
                    position: absolute;
                    top: 20px;
                    left: 0
                }

                .userActivities .i .activityContent {
                    margin: 0 0 0 70px;
                    border: 1px solid #e6e7ed;
                    min-height: 70px
                }

                    .userActivities .i .activityContent:after,
                    .userActivities .i .activityContent:before {
                        content: '';
                        left: 48px;
                        top: 40px;
                        border: solid transparent;
                        height: 0;
                        width: 0;
                        position: absolute;
                        pointer-events: none
                    }

                    .userActivities .i .activityContent:after {
                        border-right-color: #fff;
                        border-width: 12px;
                        margin-top: -12px
                    }

                    .userActivities .i .activityContent:before {
                        border-right-color: #dcdcdc;
                        border-width: 11px;
                        margin-top: -11px
                    }

            .userActivities ul {
                padding: 20px 25px
            }

                .userActivities ul li .title {
                    font-size: 16px;
                    font-size: 1.6rem;
                    text-transform: none
                }

            .userActivities .status li {
                padding-bottom: 0
            }

                .userActivities .status li .share {
                    margin: 20px 0 0 0
                }

                    .userActivities .status li .share a {
                        color: #fe5621;
                        display: inline-block;
                        margin: 0 0 0 20px
                    }

                        .userActivities .status li .share a:first-child {
                            margin: 0
                        }

                        .userActivities .status li .share a .fa {
                            color: #9da2a6;
                            margin: 0 3px 0 0;
                            -webkit-transition: color .3s ease;
                            -moz-transition: color .3s ease;
                            -ms-transition: color .3s ease;
                            -o-transition: color .3s ease
                        }

                        .userActivities .status li .share a:hover .fa {
                            color: #fe5621
                        }

            .userActivities .comments {
                background: #f5f6fa;
                border-top: 1px solid #e6e7ed
            }

                .userActivities .comments li:last-child {
                    border-bottom: 0;
                    padding-bottom: 0
                }

                .userActivities .comments li .image {
                    position: absolute;
                    left: 0;
                    top: 0
                }

                    .userActivities .comments li .image img {
                        margin: 0
                    }

                .userActivities .comments li .c {
                    margin: 0 0 0 70px
                }

                    .userActivities .comments li .c .form-control {
                        border-left: 1px solid #e6e7ed;
                        padding: 10px 18px;
                        margin: 0 0 10px 0
                    }

                        .userActivities .comments li .c .form-control:focus {
                            border-color: #e6e7ed
                        }

                .userActivities .comments li.showComments {
                    border-bottom: 0;
                    padding: 0;
                    margin: 0 0 15px 0
                }

                    .userActivities .comments li.showComments .fa {
                        color: #9da2a6;
                        margin: 0 3px 0 0
                    }

                    .userActivities .comments li.showComments a:hover {
                        color: #fe5621
                    }

        .forumHeading {
            width: 100%;
            padding: 5px;
            display: flex;
            font-style: italic;
            font-family: Georgia;
        }

            .forumHeading h3 {
                margin-top: auto;
                margin-bottom: auto;
            }

            .forumHeading div {
                margin: auto;
            }

        .forumBody {
            width: 100%;
            margin: 30px 0px 20px 0px;
            font-size: large;
            font-style: italic;
            font-family: Georgia;
        }
    </style>

    <asp:UpdatePanel ID="UpdHeading" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text=""></asp:Label>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
                <div class="col-lg-12">
                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Aba Geral -->
    <div id="geral">
        <asp:UpdatePanel ID="UpdGeral" runat="server">
            <ContentTemplate>
                <br />
                <!-- Consultar Forum -->
                <div class="forum">
                    <div class="forumHeading">
                        <h3 id="forum_title" runat="server"></h3>
                        <div>
                            <h4 id="forum_tags" runat="server"></h4>
                        </div>
                    </div>
                    <hr style="border-color: black; margin: 0px;" />
                    <div class="forumBody">
                        <asp:Literal ID="ltBody" runat="server"></asp:Literal>
                    </div>
                </div>
                <br />
                <!-- Respostas -->
                <div class="Comments" id="comments" runat="server">
                    <div class="form-stacked">
                        <div class="row">
                            <div class="box">
                                <div class="userActivities">
                                    <!-- Responder -->
                                    <div class="i">
                                        <a href="#" title="#" class="image">
                                            <asp:Image ImageUrl="https://avataaars.io/?avatarStyle=Circle&topType=ShortHairShortFlat&accessoriesType=Blank&hairColor=Black&facialHairType=Blank&clotheType=BlazerShirt&eyeType=Default&eyebrowType=Default&mouthType=Default&skinColor=Light" runat="server" ID="imgUsuario" alt="#" Width="44" Height="44" />
                                        </a>
                                        <div class="activityContent">
                                            <ul class="simpleListings status">
                                                <li>
                                                    <div class="c">
                                                        <form>
                                                            <asp:TextBox ID="txtsCorpoResposta" runat="server" placeholder="Responda aqui..." class="form-control" Style="min-height: 100px;" TextMode="MultiLine"></asp:TextBox>
                                                            <br />

                                                            <asp:Button ID="btnSubmitComent" runat="server" class="btn btn-sm btn-gray" Text="Publicar Resposta" OnClick="cmdSalvarResposta_Click"></asp:Button>
                                                        </form>
                                                    </div>
                                                </li>
                                            </ul>
                                        </div>
                                    </div>

                                    <!-- Consultar Respostas -->
                                    <br />
                                    <div id="divResultado" runat="server" class="i">
                                        <asp:Repeater ID="rptRespostas" runat="server">
                                            <ItemTemplate>
                                                <ul class="simpleListings comments">
                                                    <li>
                                                        <a href="#" title="#" class="image">
                                                            <img alt="#" width="44" height="44" src="<%# carregaimgColaborador(Eval("imgColaborador")) %>" />
                                                        </a>
                                                        <div class="c">
                                                            <div class="title">
                                                                <asp:HyperLink runat="server" ID="hplsUsuario" NavigateUrl='<%# string.Format("../Colaboradores_Exibicao.aspx?idu={0}" , DataBinder.Eval(Container.DataItem, "idUsuario")) %>'><%# DataBinder.Eval(Container.DataItem, "sDscUsuario") %></asp:HyperLink>
                                                            </div>
                                                            <div class="info"><span><%# DataBinder.Eval(Container.DataItem, "dtResposta") %></span></div>
                                                            <p><%# DataBinder.Eval(Container.DataItem, "sDscResposta") %></p>
                                                        </div>
                                                    </li>
                                                </ul>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                <fieldset class="form-stacked actions" id="fsBtns" runat="server">
                    <asp:Button runat="server" ID="cmdNovoFaq" class="btn btn-lg btn-success" Text="Criar FAQ" OnClick="cmdNovoFaq_Click" />
                    <asp:Button ID="cmdEditar" class="btn btn-lg btn-primary" runat="server" Text="Editar" OnClick="cmdEditar_Click" />
                    <input type="submit" name="cancel" class="btn btn-lg btn-warning" id="field_cancel" value="Voltar" title="Voltar" onclick="history.go(-1)" runat="server" />
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <!-- Aba Detalhe -->
    <div id="detalhe">
        <asp:UpdatePanel ID="UpdDetalhe" runat="server">
            <ContentTemplate>
                <br />
                <!-- Detalhe Forum -->
                <div class="panel panel-default">

                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Detalhe</b></h3>
                    </div>
                    <div class="panel-body">
                        <br />
                        <div class="form-stacked row">
                            <div class="col-lg-8">
                                <div class="form-group" style="float: left">
                                    <label>ID</label>
                                    <asp:TextBox ID="txtidForum" runat="server" class="form-control" disabled=""></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-4">
                                <div class="form-group" runat="server" id="divAlterarStatus">
                                    <label>Status</label>
                                    <asp:DropDownList ID="ddlAlterarStatus" runat="server" class="form-control"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Título</label>
                                    <asp:TextBox ID="txtsDscTitulo" runat="server" class="form-control" MaxLength="200"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>TAG's</label><label style="float: right;"><small runat="server" id="smExemplos" style="color: grey;">Exemplos: TagExemplo Tag2 TagDiferente</small></label>
                                    <asp:TextBox ID="txtsTag" runat="server" class="form-control" MaxLength="200"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Nova Categoria</label><small> (Opcional)</small>
                                    <asp:TextBox ID="txtNovaCategoria" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <label>Categoria</label>
                                    <asp:DropDownList ID="ddlCategoria" runat="server" class="form-control"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Departamento</label>
                                    <asp:DropDownList ID="ddlDepartamento" runat="server" class="form-control"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <label>Data de Criação</label>
                                    <asp:TextBox ID="txtdtCriado" class="form-control" runat="server" MaxLength="10" data-mask="00/00/0000" disabled=""></asp:TextBox>
                                </div>
                            </div>


                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="hddidNCM" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>

        <div runat="server" id="divDescricao">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Descrição</b></h3>
                </div>
                <div class="panel-body">
                    <div class="col-lg-12">
                        <div class="form-group">
                            <br />
                            <label>Editar Descrição</label>
                            <%--<HTMLEditor:Editor ID="edsCorpo" runat="server" Width="100%" Height="400px" />--%>
                            <textarea runat="server" id="edsCorpo" class="htmlEditor" ></textarea>
                        </div>
                    </div>
                </div>
            </div>

            <br />

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacaoDetalhe" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                <input type="submit" name="cancel" class="btn btn-lg btn-danger" id="field_cancelDetalhe" value="Cancelar" title="Voltar" onclick="history.go(-1)" runat="server" />
            </fieldset>
        </div>
    </div>

    <br />
</asp:Content>
