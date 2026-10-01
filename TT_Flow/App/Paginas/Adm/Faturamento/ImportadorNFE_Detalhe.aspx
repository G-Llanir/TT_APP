<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ImportadorNFE_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Faturamento.ImportadorNFE_Detalhe" Async="true" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        td .chosen-container-single {
            max-width: 400px !important;
        }

        td.Amarelo .chosen-single {
            background-color: #fff3cd !important;
        }

        .modal-title {
            color: #009A22;
            text-shadow: 2px 2px 2px rgba(0, 0, 0, 0.2);
            font-size: 24px;
            font-weight: bold;
            margin: 0;
        }

        .modal-Titulo {
            color: white;
            text-shadow: 2px 2px 2px rgba(0, 0, 0, 0.2);
            font-size: 20px;
            font-weight: bold;
            margin: 0;
            background: #d9534f;
            padding: 10px;
            border-radius: 6px;
            margin: -10px;
        }

        .modal-largo {
            width: 80% !important;
            max-width: none !important;
        }

        .modal-Medio {
            width: 60% !important;
            max-width: none !important;
        }

        .modal-Pequeno {
            width: 35% !important;
            max-width: none !important;
        }

        .modal-logo img {
            max-width: 50px;
            margin-right: 10px;
        }

        .modal-header-content {
            display: flex;
            align-items: center;
        }

        .modal-footer {
            text-align: left !important;
        }

        .modal-backdrop.show {
            z-index: 1060;
        }

        .modal.show {
            z-index: 1070;
        }

        .modal-backdrop.fade.show {
            display: none;
        }
    </style>

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Importador de NF-e"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
    </div>

    <div>
        <ul id="tab_Importador" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#ide" id="ide-tab" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Detalhe</b></a>
            </li>

            <li role="presentation" runat="server" id="aba_NFe">
                <a href="#NFe" role="tab" id="NFe-tab" data-toggle="tab" aria-controls="Documentos"><b>Documentos Fiscais</b></a>
            </li>
        </ul>
    </div>

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="ide" aria-labelledby="ide-tab">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <br />
                    <div class="panel panel-default" runat="server" id="div_Importador">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Detalhe</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked">
                                <div class="col-lg-12" runat="server" id="DIV_NOVO">
                                    <div class="row">

                                        <div class="col-lg-6" id="div4" runat="server">
                                            <div class="form-group">
                                                <label>Selecione o Arquivo</label>
                                                <asp:FileUpload ID="fu_Arquivo" class="form-control-file" runat="server" Width="400px" />
                                            </div>
                                        </div>

                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="Descrição"></asp:Label></label>
                                                <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control CaixaTextoGigante" runat="server" MaxLength="300"></asp:TextBox>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="DIV_DADOS">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Detalhe</b></h3>
                        </div>
                        <div class="panel-body row">
                            <div class="form-stacked">
                                <div class="col-lg-12" runat="server" id="div_ide">
                                    <div class="panel panel-default" runat="server" id="div23">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Emit</b></h3>
                                        </div>

                                        <div class="panel-body row">
                                            <div class="form-stacked">
                                                <div class="col-lg-12" runat="server" id="DIV3">
                                                    <div class="row">

                                                        <div class="col-lg-12" runat="server" id="DIV8">
                                                            <div class="row">
                                                                <div class="col-lg-6" runat="server" id="Div9">
                                                                    <div class="form-group">
                                                                        <label>Parceiro</label>
                                                                        <asp:DropDownList ID="ddlidParceiroEmit" runat="server" class="form-control yes_no select Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidParceiroEmit_SelectedIndexChanged"></asp:DropDownList>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-6" runat="server" id="Div10">
                                                                    <div class="form-group">
                                                                        <label>Endereço</label>
                                                                        <asp:DropDownList ID="ddlidEnderecoEmit" runat="server" class="form-control yes_no select Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidEnderecoEmit_SelectedIndexChanged"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_CNPJ">
                                                            <div class="form-group">
                                                                <label>CNPJ</label>
                                                                <asp:TextBox ID="txtemitCNPJ" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3" runat="server" id="DIV_xNome">
                                                            <div class="form-group">
                                                                <label>xNome</label>
                                                                <asp:TextBox ID="txtemitxNome" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3" runat="server" id="DIV_xFant">
                                                            <div class="form-group">
                                                                <label>xFant</label>
                                                                <asp:TextBox ID="txtemitxFant" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4" runat="server" id="DIV_xLgr">
                                                            <div class="form-group">
                                                                <label>xLgr</label>
                                                                <asp:TextBox ID="txtemitxLgr" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_nro">
                                                            <div class="form-group">
                                                                <label>nro</label>
                                                                <asp:TextBox ID="txtemitnro" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_xBairro">
                                                            <div class="form-group">
                                                                <label>xBairro</label>
                                                                <asp:TextBox ID="txtemitxBairro" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_cMun">
                                                            <div class="form-group">
                                                                <label>cMun</label>
                                                                <asp:TextBox ID="txtemitcMun" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_xMun">
                                                            <div class="form-group">
                                                                <label>xMun</label>
                                                                <asp:TextBox ID="txtemitxMun" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_UF">
                                                            <div class="form-group">
                                                                <label>UF</label>
                                                                <asp:TextBox ID="txtemitUF" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_CEP">
                                                            <div class="form-group">
                                                                <label>CEP</label>
                                                                <asp:TextBox ID="txtemitCEP" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_cPais">
                                                            <div class="form-group">
                                                                <label>cPais</label>
                                                                <asp:TextBox ID="txtemitcPais" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_xPais">
                                                            <div class="form-group">
                                                                <label>xPais</label>
                                                                <asp:TextBox ID="txtemitxPais" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_fone">
                                                            <div class="form-group">
                                                                <label>fone</label>
                                                                <asp:TextBox ID="txtemitfone" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_IE">
                                                            <div class="form-group">
                                                                <label>IE</label>
                                                                <asp:TextBox ID="txtemitIE" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_CRT">
                                                            <div class="form-group">
                                                                <label>CRT</label>
                                                                <asp:TextBox ID="txtemitCRT" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                    <div class="panel panel-default" runat="server" id="div2">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Dest</b></h3>
                                        </div>

                                        <div class="panel-body row">
                                            <div class="form-stacked">
                                                <div class="col-lg-12" runat="server" id="DIV6">
                                                    <div class="row">

                                                        <div class="col-lg-12" runat="server" id="DIV7">
                                                            <div class="row">
                                                                <div class="col-lg-6" runat="server" id="Div27">
                                                                    <div class="form-group">
                                                                        <label>Parceiro</label>
                                                                        <asp:DropDownList ID="ddlidParceiroDest" runat="server" class="form-control yes_no select Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidParceiroDest_SelectedIndexChanged"></asp:DropDownList>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-6" runat="server" id="Div5">
                                                                    <div class="form-group">
                                                                        <label>Endereço</label>
                                                                        <asp:DropDownList ID="ddlidEnderecoDest" runat="server" class="form-control yes_no select Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidEnderecoDest_SelectedIndexChanged"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_destCNPJ">
                                                            <div class="form-group">
                                                                <label id="lblCPF_CNPJ" runat="server">CNPJ</label>
                                                                <asp:TextBox ID="txtdestCNPJ" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3" runat="server" id="DIV_destxNome">
                                                            <div class="form-group">
                                                                <label>xNome</label>
                                                                <asp:TextBox ID="txtdestxNome" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3" runat="server" id="DIV_destxLgr">
                                                            <div class="form-group">
                                                                <label>xLgr</label>
                                                                <asp:TextBox ID="txtdestxLgr" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_destnro">
                                                            <div class="form-group">
                                                                <label>nro</label>
                                                                <asp:TextBox ID="txtdestnro" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_destxCpl">
                                                            <div class="form-group">
                                                                <label>xCpl</label>
                                                                <asp:TextBox ID="txtdestxCpl" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4" runat="server" id="DIV_destxBairro">
                                                            <div class="form-group">
                                                                <label>xBairro</label>
                                                                <asp:TextBox ID="txtdestxBairro" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_destcMun">
                                                            <div class="form-group">
                                                                <label>cMun</label>
                                                                <asp:TextBox ID="txtdestcMun" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_destxMun">
                                                            <div class="form-group">
                                                                <label>xMun</label>
                                                                <asp:TextBox ID="txtdestxMun" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_destUF">
                                                            <div class="form-group">
                                                                <label>UF</label>
                                                                <asp:TextBox ID="txtdestUF" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_destcPais">
                                                            <div class="form-group">
                                                                <label>cPais</label>
                                                                <asp:TextBox ID="txtdestcPais" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_destxPais">
                                                            <div class="form-group">
                                                                <label>xPais</label>
                                                                <asp:TextBox ID="txtdestxPais" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_destindIEDest">
                                                            <div class="form-group">
                                                                <label>indIEDest</label>
                                                                <asp:TextBox ID="txtdestindIEDest" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="DIV_IEdest">
                                                            <div class="form-group">
                                                                <label>IE</label>
                                                                <asp:TextBox ID="txtIEdest" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                    <div class="panel panel-default" runat="server" id="div13">
                                        <div class="panel-heading">
                                            <div class="row">
                                                <div class="col-lg-9">
                                                    <h3 id="Item_H3" runat="server" class="panel-title"><b>Itens</b></h3>
                                                </div>
                                                <div class="col-lg-3" runat="server" id="div_Ordem">
                                                    <asp:DropDownList ID="ddlPedidos_Geral" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlPedidos_Geral_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panel-body row">
                                            <div class="form-stacked">
                                                <div class="col-lg-12" runat="server" id="DIV14">
                                                    <div class="row">

                                                        <div class="col-lg-12" runat="server" id="DIV11">
                                                            <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                                                        </div>

                                                        <div class="col-lg-12" runat="server" id="DIV1">
                                                            <div class="">
                                                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover table-condensed table-responsive tablesorter"
                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" DataKeyNames="idItens, nItem" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound"
                                                                    OnRowCommand="dtgvConsulta_RowCommand">
                                                                    <Columns>

                                                                        <asp:TemplateField HeaderText="Ordem">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="nOrdem" runat="server" Text='<%# Bind("nItem") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="2%" />
                                                                        </asp:TemplateField>

                                                                        <asp:BoundField DataField="cProd" HeaderText="cProd">
                                                                            <ItemStyle Width="7%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                        </asp:BoundField>

                                                                        <asp:BoundField DataField="xProd" HeaderText="xProd">
                                                                            <ItemStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                                                        </asp:BoundField>

                                                                        <asp:TemplateField HeaderText="uCom">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="uCom" runat="server" Text='<%# Bind("uCom") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="center" VerticalAlign="Middle" Width="2%" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="NCM">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="NCM" runat="server" Text='<%# Bind("NCM") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="5%" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="cEAN">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="EAN" runat="server" Text='<%# Bind("cEAN") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="5%" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="CFOP">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="CFOP" runat="server" Text='<%# Bind("CFOP") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="2%" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="qCom">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="qCom" runat="server" Text='<%# Bind("qCom") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="right" VerticalAlign="Middle" Width="5%" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="vUnCom">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="vUnCom" runat="server" Text='<%# Bind("vUnCom") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="right" VerticalAlign="Middle" Width="5%" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="vProd">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="vProd" runat="server" Text='<%# Bind("vProd") %>' ClientIDMode="Static" DataFormatString="{0:N2}"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="right" VerticalAlign="Middle" Width="8%" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="cEANTrib">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="cEANTrib" runat="server" Text='<%# Bind("cEANTrib") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="5%" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="uTrib">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="uTrib" runat="server" Text='<%# Bind("uTrib") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="2%" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="qTrib">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="qTrib" runat="server" Text='<%# Bind("qTrib") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="5%" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="vUnTrib">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="vUnTrib" runat="server" Text='<%# Bind("vUnTrib") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="5%" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="indTot">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="indTot" runat="server" Text='<%# Bind("indTot") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="2%" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="xPed">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="xPed" runat="server" Text='<%# Bind("xPed") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="2%" />
                                                                        </asp:TemplateField>


                                                                        <asp:TemplateField HeaderText="Pedido">
                                                                            <ItemTemplate>
                                                                                <asp:DropDownList ID="ddlPedidos" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlPedidos_SelectedIndexChanged"></asp:DropDownList>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>


                                                                        <asp:TemplateField HeaderText="Produto">
                                                                            <ItemTemplate>
                                                                                <asp:DropDownList ID="Produtos_ddlidItem" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                                                <asp:Label ID="Produtos_txtidItem" runat="server" Text='<%# Bind("sDscProduto_Pesquisa") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>


                                                                        <asp:TemplateField HeaderText="">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton ID="lnk_Editar" CommandArgument='<%# Bind("nItem") %>' runat="server" ToolTip="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>

                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Pedido">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="sReferencia" runat="server" Text='<%# Bind("sReferencia") %>' ClientIDMode="Static"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%" />
                                                                        </asp:TemplateField>

                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>


                                    <div class="panel panel-default" runat="server" id="div52">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>transp</b></h3>
                                        </div>

                                        <div class="panel-body row">
                                            <div class="form-stacked">
                                                <div class="col-lg-12" runat="server" id="DIV60">
                                                    <div class="row">

                                                        <div class="col-lg-4" runat="server" id="div_ModoFrete">
                                                            <div class="form-group">
                                                                <label>Modo Frete</label>
                                                                <asp:DropDownList ID="ddlModoFrete" runat="server" class="form-control yes_no select Caixa_Selecao">
                                                                    <asp:ListItem Value="">Selecione o Modo Frete</asp:ListItem>
                                                                    <asp:ListItem Value="0">0 - Contrat. conta do Remetente (CIF)</asp:ListItem>
                                                                    <asp:ListItem Value="1">1 - Contrat. conta do Destinatário (FOB)</asp:ListItem>
                                                                    <asp:ListItem Value="2">2 - Contrat. conta de Terceiros</asp:ListItem>
                                                                    <asp:ListItem Value="3">3 - Transp. próp. conta de Remetente</asp:ListItem>
                                                                    <asp:ListItem Value="4">4 - Transp. próp. conta de Destinatário</asp:ListItem>
                                                                    <asp:ListItem Value="9">9 - Sem ocorrência de transporte</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-12 row">
                                                            <div class="col-lg-6" runat="server" id="div_Transportadora">
                                                                <div class="form-group">
                                                                    <label>Transportadora</label>
                                                                    <asp:DropDownList ID="ddlidTranportadora" class="form-control  Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidTranportadora_SelectedIndexChanged"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-12 row" runat="server" id="div_InfoTransportadora">
                                                                <div class="col-lg-3" runat="server" id="DIV_CNPJTrans">
                                                                    <div class="form-group">
                                                                        <label>CNPJ</label>
                                                                        <asp:TextBox ID="txtCNPJTrans" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-6" runat="server" id="DIV_NomeTrans">
                                                                    <div class="form-group">
                                                                        <label>Nome</label>
                                                                        <asp:TextBox ID="txtNomeTrans" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV_IETrans">
                                                                    <div class="form-group">
                                                                        <label>IE</label>
                                                                        <asp:TextBox ID="txtIETrans" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-6" runat="server" id="DIV_EndereçoTrans">
                                                                    <div class="form-group">
                                                                        <label>Endereço</label>
                                                                        <asp:TextBox ID="txtEnderecoTrans" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV_MunicípioTrans">
                                                                    <div class="form-group">
                                                                        <label>Município</label>
                                                                        <asp:TextBox ID="txtMunicipiotrans" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-1" runat="server" id="DIV_UFTrans">
                                                                    <div class="form-group">
                                                                        <label>UF</label>
                                                                        <asp:TextBox ID="txtUFTrans" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-12 row">
                                                            <div class="col-lg-2" runat="server" id="DIV_VolumesTrans">
                                                                <div class="form-group">
                                                                    <label>Qtd Volumes</label>
                                                                    <asp:TextBox ID="txtVolumes" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-2" runat="server" id="DIV_EspécieTrans">
                                                                <div class="form-group">
                                                                    <label>Espécie</label>
                                                                    <asp:TextBox ID="txtEspecie" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-2" runat="server" id="DIV_LíquidoTrans">
                                                                <div class="form-group">
                                                                    <label>Peso Líquido</label>
                                                                    <asp:TextBox ID="txtPesoLiquido" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-2" runat="server" id="DIV_BrutoTrans">
                                                                <div class="form-group">
                                                                    <label>Peso Bruto</label>
                                                                    <asp:TextBox ID="txtPesoBruto" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
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
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="NFe" aria-labelledby="NFe-tab">

            <asp:UpdatePanel ID="updPanel_DocumentoFiscais" runat="server">
                <ContentTemplate>

                    <br />
                    <div class="panel panel-default" runat="server" id="DIV16">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Documentos Fiscais</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_DocumentosFiscais" />
                                </div>
                                <div class="col-lg-12 table-responsive">
                                    <asp:GridView ID="gvDocumentosFiscais" class="table table-striped table-bordered table-hover "
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="chNFe, idObjeto, sXML_Autorizado, idXML"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvDocumentosFiscais_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="idXML" HeaderText="ID">
                                                <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="idObjeto" HeaderText="ID Pedido">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sdtEvento_Formatada" HeaderText="Data ">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nSerieNF" HeaderText="Série">
                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nNumeroNF" HeaderText="Número">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="chNFe" HeaderText="Chave NFe">
                                                <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sStatus" HeaderText="Status">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="xMotivo" HeaderText="Observação">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Ações">
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdGerarNFe" class="btn btn-sm btn-success" Text="Enviar Sefaz" OnClick="DocumentosFiscais_cmdGerarNFe_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDownloadXML" class="btn btn-sm btn-primary" Text="XML" OnClick="DocumentosFiscais_cmdDownloadXML_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDownloadDANFE" class="btn btn-sm btn-warning" Text="DANFE" OnClick="DocumentosFiscais_cmdDownloadDANFE_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdCartaCorrecao" class="btn btn-sm btn-success" ToolTip="Carta de Correção" Text="CC-e" OnClick="DocumentosFiscais_cmdCartaCorrecao_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDownloadCCeXML" class="btn btn-sm btn-primary" Text="XML" OnClick="DocumentosFiscais_cmdDownloadCCeXML_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDownloadCCePDF" class="btn btn-sm btn-danger" Text="PDF" OnClick="DocumentosFiscais_cmdDownloadCCePDF_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdCancelarNFE" class="btn btn-sm btn-danger" ToolTip="Cancelar NF-e" Text="Cancelar NF-e" OnClick="DocumentosFiscais_cmdCancelarNFE_Click"></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="nNumeroCartaCorrecao" HeaderText="Qtd Cartas" />
                                            <asp:BoundField DataField="sCancelamento" HeaderText="Tem Cancelamento" />
                                            <asp:BoundField DataField="sChaveNFE" HeaderText="ChaveNFe" />

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>

                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

        <fieldset class="form-stacked actions">
            <div class="col-lg-12 row" runat="server" id="DIV15">

                <div class="col-lg-3 row" runat="server" id="Div17">
                    <div class="form-group">
                        <label>Armazenamento</label>
                        <asp:DropDownList ID="ddlidArmazenamento" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-4" runat="server" id="DIV12">
                    <div class="form-group">
                        <label>Descrição Movimentação</label>
                        <asp:TextBox ID="txtsObservacao" class="form-control" runat="server"></asp:TextBox>
                    </div>
                </div>

            </div>

            <asp:Button ID="btnSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="DETALHE" OnClick="btnSalvar_Click" />
            <asp:Button ID="cmdGerarNFPiloto" class="btn  btn-lg btn-info" runat="server" Text="Gerar Espelho" ValidationGroup="DETALHE" OnClick="cmdGerarNFPiloto_Click" />
            <%--        <asp:Button ID="btnGerar" class="btn  btn-lg btn-info" runat="server" Text="Gerar NF-e" ValidationGroup="DETALHE" OnClick="btnGerar_Click" />--%>
            <asp:Button ID="cmdImportarNovoXML" class="btn btn-lg btn-primary" runat="server" Text="Importar XML" OnClick="cmdImportarNovoXML_Click" />
            <asp:Button ID="btnExcluir" CssClass="btn btn-lg btn-danger" runat="server" Text="Excluir" />
            <a href="javascript:history.back()" class="btn btn-lg btn-warning" title="Voltar">Voltar</a>
        </fieldset>

        <div class="modal fade" id="modalImposto" tabindex="-1" role="dialog" aria-labelledby="modalImpostoLabel" aria-hidden="true" data-backdrop="static">
            <asp:UpdatePanel ID="updModal" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-dialog modal-xl" role="document">
                        <div class="modal-content ">
                            <div class="modal-header">
                                <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                                <asp:Label class="modal-title" ID="lblModalImposto" runat="server">Imposto</asp:Label>
                            </div>
                            <div class="modal-body">
                                <div class="form-group row">
                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaUnitizados" />
                                    </div>
                                    <div id="divMultiView" runat="server" class="col-lg-12">

                                        <div class="panel panel-default" runat="server" id="div19">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>ICMS</b></h3>
                                            </div>
                                            <div class="panel-body row">
                                                <div class="form-stacked">
                                                    <div class="col-lg-12" runat="server" id="DIV20">
                                                        <div class="row">

                                                            <div class="col-lg-12" runat="server" id="DIV21">
                                                                <div class="row">
                                                                    <div class="col-lg-2" runat="server" id="DIV18">
                                                                        <div class="form-group">
                                                                            <label>orig</label>
                                                                            <asp:TextBox ID="txtorigICMS" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV22">
                                                                        <div class="form-group">
                                                                            <label>CST</label>
                                                                            <asp:TextBox ID="txtCST" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV24">
                                                                        <div class="form-group">
                                                                            <label>modBC</label>
                                                                            <asp:TextBox ID="txtmodBC" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV25">
                                                                        <div class="form-group">
                                                                            <label>vBC</label>
                                                                            <asp:TextBox ID="txtvBC" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV26">
                                                                        <div class="form-group">
                                                                            <label>pRedBC</label>
                                                                            <asp:TextBox ID="txtpRedBC" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV28">
                                                                        <div class="form-group">
                                                                            <label>pICMS</label>
                                                                            <asp:TextBox ID="txtpICMS" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV29">
                                                                        <div class="form-group">
                                                                            <label>vICMS</label>
                                                                            <asp:TextBox ID="txtvICMS" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panel panel-default" runat="server" id="div30">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>IPI</b></h3>
                                            </div>
                                            <div class="panel-body row">
                                                <div class="form-stacked">
                                                    <div class="col-lg-12" runat="server" id="DIV31">
                                                        <div class="row">

                                                            <div class="col-lg-12" runat="server" id="DIV32">
                                                                <div class="row">
                                                                    <div class="col-lg-2" runat="server" id="DIV33">
                                                                        <div class="form-group">
                                                                            <label>cEnq</label>
                                                                            <asp:TextBox ID="txtcEnq" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV34">
                                                                        <div class="form-group">
                                                                            <label>CST</label>
                                                                            <asp:TextBox ID="txtCSTIPI" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV35">
                                                                        <div class="form-group">
                                                                            <label>vBC</label>
                                                                            <asp:TextBox ID="txtvBCIPI" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV36">
                                                                        <div class="form-group">
                                                                            <label>pIPI</label>
                                                                            <asp:TextBox ID="txtpIPI" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV37">
                                                                        <div class="form-group">
                                                                            <label>vIPI</label>
                                                                            <asp:TextBox ID="txtvIPI" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panel panel-default" runat="server" id="div40">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>II</b></h3>
                                            </div>
                                            <div class="panel-body row">
                                                <div class="form-stacked">
                                                    <div class="col-lg-12" runat="server" id="DIV41">
                                                        <div class="row">

                                                            <div class="col-lg-12" runat="server" id="DIV42">
                                                                <div class="row">
                                                                    <div class="col-lg-2" runat="server" id="DIV43">
                                                                        <div class="form-group">
                                                                            <label>vBC</label>
                                                                            <asp:TextBox ID="txtvBCII" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV44">
                                                                        <div class="form-group">
                                                                            <label>vDespAdu</label>
                                                                            <asp:TextBox ID="txtvDespAdu" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV45">
                                                                        <div class="form-group">
                                                                            <label>vII</label>
                                                                            <asp:TextBox ID="txtvII" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV46">
                                                                        <div class="form-group">
                                                                            <label>vIOF</label>
                                                                            <asp:TextBox ID="txtvIOF" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panel panel-default" runat="server" id="div38">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>PIS</b></h3>
                                            </div>
                                            <div class="panel-body row">
                                                <div class="form-stacked">
                                                    <div class="col-lg-12" runat="server" id="DIV39">
                                                        <div class="row">

                                                            <div class="col-lg-12" runat="server" id="DIV47">
                                                                <div class="row">
                                                                    <div class="col-lg-2" runat="server" id="DIV48">
                                                                        <div class="form-group">
                                                                            <label>CST</label>
                                                                            <asp:TextBox ID="txtCSTPIS" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV49">
                                                                        <div class="form-group">
                                                                            <label>vBC</label>
                                                                            <asp:TextBox ID="txtvBCPIS" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV50">
                                                                        <div class="form-group">
                                                                            <label>pPIS</label>
                                                                            <asp:TextBox ID="txtpPIS" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV51">
                                                                        <div class="form-group">
                                                                            <label>vPIS</label>
                                                                            <asp:TextBox ID="txtvPIS" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panel panel-default" runat="server" id="div53">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>COFINS</b></h3>
                                            </div>
                                            <div class="panel-body row">
                                                <div class="form-stacked">
                                                    <div class="col-lg-12" runat="server" id="DIV54">
                                                        <div class="row">

                                                            <div class="col-lg-12" runat="server" id="DIV55">
                                                                <div class="row">
                                                                    <div class="col-lg-2" runat="server" id="DIV56">
                                                                        <div class="form-group">
                                                                            <label>CST</label>
                                                                            <asp:TextBox ID="txtCSTCOFINS" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV57">
                                                                        <div class="form-group">
                                                                            <label>vBC</label>
                                                                            <asp:TextBox ID="txtvBCCOFINS" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV58">
                                                                        <div class="form-group">
                                                                            <label>pCOFINS</label>
                                                                            <asp:TextBox ID="txtpCOFINS" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2" runat="server" id="DIV59">
                                                                        <div class="form-group">
                                                                            <label>vCOFINS</label>
                                                                            <asp:TextBox ID="txtvCOFINS" class="form-control" runat="server"></asp:TextBox>
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
                                <div class="modal-footer row">
                                    <asp:LinkButton ID="cmdSalvar" CssClass="btn btn-success" Text="Salvar" runat="server" OnClick="cmdSalvar_Click" />
                                    <asp:LinkButton CssClass="btn btn-danger" Text="Cancelar" runat="server" data-dismiss="modal" />
                                </div>
                            </div>
                        </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <div id="dialog-Excluir" class="modal" title="Excluir">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloExcluir" runat="server" Text="Confirma a Exclusão dessa NFe?"></asp:Label>
        </p>
    </div>

    <div class="modal fade" id="Modal_AcoesNFe" tabindex="-1" data-backdrop="static" role="dialog">
        <div class="modal-dialog modal-dialog-centered" role="document" id="div_Modal" runat="server">
            <div class="modal-content">
                <asp:UpdatePanel ID="updAcoesNFe" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" id="cmdFecharModalAcoesNFe" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <div class="modal-header-content">
                                <div class="modal-logo">
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/App/img/LogoTT.png" AlternateText="LogoTipo" />
                                </div>
                                <div class="modal-title-container">
                                    <asp:Label runat="server" class="modal-title" ID="lblTitulos_AcoesNFe" Text="" Font-Bold="true"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <div class="modal-body">
                            <div class="panel-body">
                                <div class="form-stacked row">
                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaAcoesNFe" />
                                    </div>

                                    <div class="col-lg-12">
                                        <div class="form-group row">
                                            <label>
                                                <asp:Label runat="server" ID="lblTituloJustificativa_AcoesNFe" Text="Informe uma justificativa"></asp:Label></label>
                                            <asp:TextBox ID="txtsJustificativa_AcoesNFe" class="form-control" runat="server" MaxLength="255" TextMode="MultiLine" Height="95px"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <asp:HiddenField ID="hddsAcaoNFe" runat="server" />
                        <asp:HiddenField ID="hddidXML" runat="server" />

                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="modal-footer text-left">
                    <asp:LinkButton ID="cmdConfirmar_AcoesNFe" CssClass="btn btn-success" Text="OK" OnClick="cmdConfirmar_AcoesNFe_Click" runat="server" />
                    <asp:LinkButton CssClass="btn btn-danger" Text="Cancelar" runat="server" data-dismiss="modal" />
                </div>
            </div>
        </div>

    </div>
    <asp:HiddenField ID="hddsNomeArquivo" runat="server" />
    <asp:HiddenField ID="hddidArquivo" runat="server" />
    <asp:HiddenField ID="hddsEnderecoEmit" runat="server" />
    <asp:HiddenField ID="hddsEnderecoDest" runat="server" />
    <asp:HiddenField ID="hddsCaminho_UniNFe" runat="server" />
    <asp:HiddenField ID="hddidStatus" runat="server" />
    <asp:HiddenField ID="hddidImportador" runat="server" />
    <asp:HiddenField ID="hddidTipoObjeto" runat="server" />
    <asp:HiddenField ID="hddnItem" runat="server" />
    <asp:HiddenField ID="hddidItens" runat="server" />
    <asp:HiddenField ID="hddidEmpresa" runat="server" />

    <%----------------------------------------------------------------------------------------------------------------------------------------------%>
</asp:Content>
