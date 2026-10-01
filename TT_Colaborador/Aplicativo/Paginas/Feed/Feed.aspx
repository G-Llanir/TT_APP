<%@ Page Title="" Language="C#" MasterPageFile="~/site.master" AutoEventWireup="true" CodeBehind="Feed.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Feed.Feed" %>

<%@ Register Src="~/Aplicativo/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="updTimeline" runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                // Seu código JavaScript aqui
            </script>

            <div class="container mt-4">
                <div class="row">
                    <div class="col-lg-12">
                        <h1 class="mb-3">Minha Timeline</h1>
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12">
                        <ul class="nav nav-tabs">
                            <li class="nav-item">
                                <a class="nav-link active" data-toggle="tab" href="#feed">Feed</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" data-toggle="tab" href="#particular">Particular</a>
                            </li>
                        </ul>

                        <div class="tab-content">
                            <div id="feed" class="tab-pane fade show active">
                                <div class="container mt-4">
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <div class="card">
                                                <div class="card-body">
                                                    <!-- Área para publicar -->
                                                    <div class="mb-3">
                                                        <textarea class="form-control" placeholder="O Que Há de Novo?"></textarea>
                                                        <button class="btn btn-primary mt-2">Publicar</button>
                                                    </div>

                                                    <!-- Aqui será substituido por:  <asp:Literal ID="LiteralFeed" runat="server" EnableViewState="false"></asp:Literal>-->
                                                    <div class="feed-item">
                                                        <div class="feed-header">
                                                            <img src="profile-image.jpg" class="feed-avatar" />
                                                            <h5 class="feed-user">Nome do Usuário</h5>
                                                            <small class="feed-time">Há 5 minutos</small>
                                                        </div>
                                                        <p class="feed-text">Conteúdo do post...</p>
                                                        <div class="feed-actions">
                                                            <button class="btn btn-sm btn-primary">Curtir</button>
                                                            <button class="btn btn-sm btn-secondary">Comentar</button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Continue com o restante do seu código -->
                                </div>
                            </div>
                            <div id="particular" class="tab-pane fade">
                                <!-- Conteúdo do feed particular -->
                                <!-- Repita os blocos de itens de feed aqui -->
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Continue com o restante do seu código -->
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
