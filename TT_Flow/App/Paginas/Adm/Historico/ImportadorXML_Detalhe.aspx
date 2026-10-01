<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ImportadorXML_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Historico.ImportadorXML_Detalhe" Async="true" %>

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
                <asp:Label ID="lblTituloPagina" runat="server" Text="Importação de XML"></asp:Label>
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

            <li role="presentation" runat="server" id="aba_PDF">
                <a href="#PDF" role="tab" id="PDF-tab" data-toggle="tab" aria-controls="PDF"><b>NF-e</b></a>
            </li>
        </ul>
    </div>

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="ide" aria-labelledby="ide-tab">
            <br />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>

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
                                                <asp:FileUpload ID="fu_Arquivo" class="form-control-file" runat="server" Width="400px" AllowMultiple="true"/>
                                            </div>
                                        </div>

                                        <div class="col-lg-12" runat="server" id="DIV_Financeiro">
                                            <div class="col-lg-2 row" runat="server">
                                                <div class="form-group">
                                                    <label>Tipo</label>
                                                    <asp:DropDownList ID="ddlTipo" runat="server" class="Caixa_Selecao form-control ">
                                                        <asp:ListItem Value="0" Text="Selecione o Tipo" />
                                                        <asp:ListItem Value="1" Text="Compras" />
                                                        <asp:ListItem Value="2" Text="Devolução" />
                                                        <asp:ListItem Value="3" Text="Vendas" />
                                                        <asp:ListItem Value="4" Text="Outros" />
                                                    </asp:DropDownList>
                                                </div>
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

                                    <%--ide--%>
                                    <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                                        <ContentTemplate>
                                            <div class="panel panel-default" runat="server" id="div35">
                                                <div class="panel-heading">
                                                    <h3 class="panel-title"><b>ide</b></h3>
                                                </div>

                                                <div class="panel-body row">
                                                    <div class="form-stacked">
                                                        <div class="col-lg-12" runat="server" id="DIV36">
                                                            <div class="row">

                                                                <div class="col-lg-2" runat="server" id="DIV37">
                                                                    <div class="form-group">
                                                                        <label>cUF</label>
                                                                        <asp:TextBox ID="cUF" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV38">
                                                                    <div class="form-group">
                                                                        <label>cNF</label>
                                                                        <asp:TextBox ID="cNF" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV39">
                                                                    <div class="form-group">
                                                                        <label>natOp</label>
                                                                        <asp:TextBox ID="natOp" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV40">
                                                                    <div class="form-group">
                                                                        <label>mod</label>
                                                                        <asp:TextBox ID="mod" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV41">
                                                                    <div class="form-group">
                                                                        <label>serie</label>
                                                                        <asp:TextBox ID="serie" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV42">
                                                                    <div class="form-group">
                                                                        <label>nNF</label>
                                                                        <asp:TextBox ID="nNF" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV43">
                                                                    <div class="form-group">
                                                                        <label>dhEmi</label>
                                                                        <asp:TextBox ID="dhEmi" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV44">
                                                                    <div class="form-group">
                                                                        <label>dhSaiEnt</label>
                                                                        <asp:TextBox ID="dhSaiEnt" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV45">
                                                                    <div class="form-group">
                                                                        <label>tpNF</label>
                                                                        <asp:TextBox ID="tpNF" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV46">
                                                                    <div class="form-group">
                                                                        <label>idDest</label>
                                                                        <asp:TextBox ID="idDest" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV47">
                                                                    <div class="form-group">
                                                                        <label>cMunFG</label>
                                                                        <asp:TextBox ID="cMunFG" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV48">
                                                                    <div class="form-group">
                                                                        <label>tpImp</label>
                                                                        <asp:TextBox ID="tpImp" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV49">
                                                                    <div class="form-group">
                                                                        <label>tpEmis</label>
                                                                        <asp:TextBox ID="tpEmis" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV50">
                                                                    <div class="form-group">
                                                                        <label>cDV</label>
                                                                        <asp:TextBox ID="cDV" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV51">
                                                                    <div class="form-group">
                                                                        <label>tpAmb</label>
                                                                        <asp:TextBox ID="tpAmb" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV53">
                                                                    <div class="form-group">
                                                                        <label>finNFe</label>
                                                                        <asp:TextBox ID="finNFe" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV54">
                                                                    <div class="form-group">
                                                                        <label>indFinal</label>
                                                                        <asp:TextBox ID="indFinal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV55">
                                                                    <div class="form-group">
                                                                        <label>indPres</label>
                                                                        <asp:TextBox ID="indPres" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV56">
                                                                    <div class="form-group">
                                                                        <label>procEmi</label>
                                                                        <asp:TextBox ID="procEmi" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV57">
                                                                    <div class="form-group">
                                                                        <label>verProc</label>
                                                                        <asp:TextBox ID="verProc" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>

                                    <%--Emit--%>
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <div class="panel panel-default" runat="server" id="div23">
                                                <div class="panel-heading">
                                                    <h3 class="panel-title"><b>Emit</b></h3>
                                                </div>

                                                <div class="panel-body row">
                                                    <div class="form-stacked">
                                                        <div class="col-lg-12" runat="server" id="DIV3">
                                                            <div class="row">

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
                                        </ContentTemplate>
                                    </asp:UpdatePanel>

                                    <%--Dest--%>
                                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                        <ContentTemplate>
                                            <div class="panel panel-default" runat="server" id="div2">
                                                <div class="panel-heading">
                                                    <h3 class="panel-title"><b>Dest</b></h3>
                                                </div>

                                                <div class="panel-body row">
                                                    <div class="form-stacked">
                                                        <div class="col-lg-12" runat="server" id="DIV6">
                                                            <div class="row">

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
                                        </ContentTemplate>
                                    </asp:UpdatePanel>

                                    <%--Itens--%>
                                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                        <ContentTemplate>
                                            <div class="panel panel-default" runat="server" id="div13">
                                                <div class="panel-heading">
                                                    <div class="row">
                                                        <div class="col-lg-9" style="margin-right: 110px;">
                                                            <h3 id="Item_H3" runat="server" class="panel-title"><b>Itens</b></h3>
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
                                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" DataKeyNames="idItens" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound"
                                                                            OnRowCommand="dtgvConsulta_RowCommand">
                                                                            <Columns>

                                                                                <asp:TemplateField HeaderText="Ordem">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="nOrdem" runat="server" Text='<%# Bind("nItem") %>' ClientIDMode="Static"></asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="2%" />
                                                                                </asp:TemplateField>

                                                                                <asp:TemplateField HeaderText="cProd">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="cProd" runat="server" Text='<%# Bind("cProd") %>' ClientIDMode="Static"></asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="7%" />
                                                                                </asp:TemplateField>

                                                                                <asp:TemplateField HeaderText="xProd">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="xProd" runat="server" Text='<%# Bind("xProd") %>' ClientIDMode="Static"></asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="18%" />
                                                                                </asp:TemplateField>

                                                                                <asp:TemplateField HeaderText="uCom">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="uCom" runat="server" Text='<%# Bind("uCom") %>' ClientIDMode="Static"></asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="2%" />
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
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="5%" />
                                                                                </asp:TemplateField>

                                                                                <asp:TemplateField HeaderText="vUnCom">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="vUnCom" runat="server" Text='<%# Bind("vUnCom") %>' ClientIDMode="Static"></asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="5%" />
                                                                                </asp:TemplateField>

                                                                                <asp:TemplateField HeaderText="vProd">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="vProd" runat="server" Text='<%# Bind("vProd") %>' ClientIDMode="Static"></asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="5%" />
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
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                    </div>
                                                                </div>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>

                                    <%--total--%>
                                    <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                                        <ContentTemplate>
                                            <div class="panel panel-default" runat="server" id="div8">
                                                <div class="panel-heading">
                                                    <h3 class="panel-title"><b>total</b></h3>
                                                </div>

                                                <div class="panel-body row">
                                                    <div class="form-stacked">
                                                        <div class="col-lg-12" runat="server" id="DIV9">
                                                            <div class="row">

                                                                <div class="col-lg-2" runat="server" id="DIV10">
                                                                    <div class="form-group">
                                                                        <label>vBC</label>
                                                                        <asp:TextBox ID="vBCTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV12">
                                                                    <div class="form-group">
                                                                        <label>vICMS</label>
                                                                        <asp:TextBox ID="vICMSTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV15">
                                                                    <div class="form-group">
                                                                        <label>vICMSDeson</label>
                                                                        <asp:TextBox ID="vICMSDesonTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV17">
                                                                    <div class="form-group">
                                                                        <label>vFCP</label>
                                                                        <asp:TextBox ID="vFCPTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV18">
                                                                    <div class="form-group">
                                                                        <label>vBCST</label>
                                                                        <asp:TextBox ID="vBCSTTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV19">
                                                                    <div class="form-group">
                                                                        <label>vST</label>
                                                                        <asp:TextBox ID="vSTTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV20">
                                                                    <div class="form-group">
                                                                        <label>vFCPST</label>
                                                                        <asp:TextBox ID="vFCPSTTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV21">
                                                                    <div class="form-group">
                                                                        <label>vFCPSTRet</label>
                                                                        <asp:TextBox ID="vFCPSTRetTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV22">
                                                                    <div class="form-group">
                                                                        <label>vProd</label>
                                                                        <asp:TextBox ID="vProdTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV24">
                                                                    <div class="form-group">
                                                                        <label>vFrete</label>
                                                                        <asp:TextBox ID="vFreteTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV25">
                                                                    <div class="form-group">
                                                                        <label>vSeg</label>
                                                                        <asp:TextBox ID="vSegTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV26">
                                                                    <div class="form-group">
                                                                        <label>vDesc</label>
                                                                        <asp:TextBox ID="vDescTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV27">
                                                                    <div class="form-group">
                                                                        <label>vII</label>
                                                                        <asp:TextBox ID="vIITotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV28">
                                                                    <div class="form-group">
                                                                        <label>vIPI</label>
                                                                        <asp:TextBox ID="vIPITotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV29">
                                                                    <div class="form-group">
                                                                        <label>vIPIDevol</label>
                                                                        <asp:TextBox ID="vIPIDevolTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV30">
                                                                    <div class="form-group">
                                                                        <label>vPIS</label>
                                                                        <asp:TextBox ID="vPISTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV31">
                                                                    <div class="form-group">
                                                                        <label>vCOFINS</label>
                                                                        <asp:TextBox ID="vCOFINSTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV32">
                                                                    <div class="form-group">
                                                                        <label>vOutro</label>
                                                                        <asp:TextBox ID="vOutroTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV33">
                                                                    <div class="form-group">
                                                                        <label>vNF</label>
                                                                        <asp:TextBox ID="vNFTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2" runat="server" id="DIV34">
                                                                    <div class="form-group">
                                                                        <label>vTotTrib</label>
                                                                        <asp:TextBox ID="vTotTribTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>

                                    <%--transp--%>
                                    <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                        <ContentTemplate>
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
                                        </ContentTemplate>
                                    </asp:UpdatePanel>

                                    <%--infAdic--%>
                                    <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                        <ContentTemplate>
                                            <div class="panel panel-default" runat="server" id="div5">
                                                <div class="panel-heading">
                                                    <h3 class="panel-title"><b>infAdic</b></h3>
                                                </div>

                                                <div class="panel-body row">
                                                    <div class="form-stacked">
                                                        <div class="col-lg-12" runat="server" id="DIV7">
                                                            <div class="row">

                                                                <div class="col-lg-12">
                                                                    <div class="form-group">
                                                                        <label>Informações Adicionais ao Fisco</label>
                                                                        <asp:TextBox ID="txtInfoFisco" class="form-control" runat="server" TextMode="MultiLine" Height="70px" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-12">
                                                                    <div class="form-group">
                                                                        <label>Informações Complementares</label>
                                                                        <asp:TextBox ID="txtInfoComplementares" class="form-control" runat="server" TextMode="MultiLine" Height="70px" ReadOnly="true"></asp:TextBox>
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
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

        <div role="tabpanel" class="tab-pane fade" id="PDF" aria-labelledby="PDF-tab">
            <br />
            <div class="panel panel-default" runat="server" id="DIV16">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>PDF</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina2" />
                        </div>
                        <div class="col-lg-12">
                            <iframe id="pdfViewer" runat="server" width="100%" height="1200px"></iframe>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <fieldset class="form-stacked actions">
        <asp:Button ID="btnXML" class="btn btn-lg btn-primary" runat="server" Text="Importar XML" OnClick="cmdXML_Click" />
        <a href="javascript:history.back()" class="btn btn-lg btn-warning" title="Voltar">Voltar</a>
    </fieldset>

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
