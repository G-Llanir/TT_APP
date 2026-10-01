<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="OcorrenciasElogios_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.OcorrenciasElogios_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>



    <div id="hdd">
        <asp:HiddenField ID="hddidRegistroEvento" runat="server" />
        <asp:HiddenField ID="hddidColaborador" runat="server" Value="0"/>
        <asp:HiddenField ID="hddEPIs" runat="server" Value="[]" />
        <asp:HiddenField ID="hddIncluir_idEPI" runat="server" Value="0" />

        <asp:HiddenField ID="hddsCadeado" runat="server" Value="N" />
        <asp:HiddenField ID="hddsPermissaoCadeado" runat="server" Value="0" />
    </div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Ocorrências e Elogios"></asp:Label></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>
    </div>
    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

    <div>
        <ul id="tab_OcorrenciaElogio" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#ocorrenciaElogio" id="aba-ocorrenciaElogio" role="tab" data-toggle="tab" aria-controls="ocorrenciaElogio" aria-expanded="false"><b>Ocorrência/Elogio</b></a>
            </li>

            <li role="presentation" runat="server" id="aba_Arquivo">
                <a href="#arquivo" role="tab" id="arquivo-tab" data-toggle="tab" aria-controls="Arquivo"><b>Arquivo</b></a>
            </li>

        </ul>
    </div>

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="ocorrenciaElogio" aria-labelledby="ocorrenciaElogio">
            <br />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>

                    <div class="form-stacked row">

                        <div class="col-lg-12 row ">
                            <div class="col-lg-2" id="div_id" runat="server">
                                <div class="form-group">
                                    <label>ID</label>
                                    <asp:TextBox ID="txtidRegistroEvento" class="form-control CaixaTextoMini" runat="server" disabled="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-12 ">
                            <div class="form-group row">

                                <div class="col-lg-2" id="div_Advertencia" runat="server">
                                    <div class="form-group">
                                        <label>Tipo</label>
                                        <asp:DropDownList ID="ddlTipo" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTipo_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2" id="div_dtEvento" runat="server">
                                    <div class="form-group">
                                        <label>Data</label>
                                        <asp:TextBox ID="txtdtEvento" class="form-control " type="date" data-mask="00/00/0000" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-4" id="div_Colaborador" runat="server" visible="false">
                                    <div class="form-group">
                                        <label>Colaborador</label>
                                        <asp:DropDownList ID="ddlidColaborador" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-4" id="div_idGravidade" runat="server" visible="false">
                                    <div class="form-group">
                                        <label>Gravidade </label>
                                        <asp:DropDownList ID="ddlidGravidade" class="form-control " runat="server"></asp:DropDownList>
                                    </div>
                                </div>


                                <div class="col-lg-4" id="div_Motivo" runat="server" visible="false">
                                    <div class="form-group">
                                        <label>Motivo Advertência</label>
                                        <asp:DropDownList ID="ddlMotivo" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="col-lg-12 ">
                            <div class="form-group row">

                                <div class="col-lg-5" id="div_sDscEvento" runat="server" visible="false">
                                    <div class="form-group">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="txtsDscEvento" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-1" id="div_idReincidencia" runat="server" visible="false">
                                    <div class="form-group">
                                        <label>Reincidência? </label>
                                        <asp:DropDownList ID="ddlidReincidencia" class="form-control" runat="server">
                                            <asp:ListItem Value="0">Selecione</asp:ListItem>
                                            <asp:ListItem Value="1">Não</asp:ListItem>
                                            <asp:ListItem Value="2">Sim</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2" id="div_idTipoEvento" runat="server" visible="false">
                                    <div class="form-group">
                                        <label>Local</label>
                                        <asp:DropDownList ID="ddlidTipoEvento" class="form-control " runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidTipoEvento_SelectedIndexChanged">
                                            <asp:ListItem Value="0">Selecione</asp:ListItem>
                                            <asp:ListItem Value="1">Cliente</asp:ListItem>
                                            <asp:ListItem Value="2">Interno</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-4" id="div_projeto" runat="server" visible="false">
                                    <div class="form-group">
                                        <label>Projeto</label>
                                        <asp:DropDownList ID="ddlidProjeto" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12 ">
                            <div class="form-group row">
                                <div class="col-lg-2" id="div_sCodReferencia" runat="server" visible="false">
                                    <div class="form-group">
                                        <label>Referência</label>
                                        <asp:TextBox ID="txtsCodReferencia" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" id="div_idCustoOcorrencia" runat="server" visible="false">
                                    <div class="form-group">
                                        <label>Teve Custo? </label>
                                        <asp:DropDownList ID="ddlidCustoOcorrencia" class="form-control " runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlidCustoOcorrencia_SelectedIndexChanged">
                                            <asp:ListItem Value="0">Selecione</asp:ListItem>
                                            <asp:ListItem Value="1">Não</asp:ListItem>
                                            <asp:ListItem Value="2">Sim</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="div_ValorOcorrencia" visible="false">
                                    <div class="form-group">
                                        <label>Custo</label>
                                        <asp:TextBox ID="txtnValorOcorrencia" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>


                                <div class="col-lg-3" runat="server" id="div_idResponsavelOcorrencia" visible="false">
                                    <div class="form-group">
                                        <label>Responsável </label>
                                        <asp:DropDownList ID="ddlidResponsavelOcorrencia" class="form-control " runat="server">
                                            <asp:ListItem Value="0">Selecione</asp:ListItem>
                                            <asp:ListItem Value="1">Colaborador</asp:ListItem>
                                            <asp:ListItem Value="2">Empresa</asp:ListItem>
                                            <asp:ListItem Value="3">Colaborador/Empresa</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-1" runat="server" id="div_idAfastamentoEvento" visible="false">
                                    <div class="form-group">
                                        <label>Afastamento </label>
                                        <asp:DropDownList ID="ddlidAfastamentoEvento" class="form-control " runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlidAfastamentoEvento_SelectedIndexChanged">
                                            <asp:ListItem Value="1">Não</asp:ListItem>
                                            <asp:ListItem Value="2">Sim</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="div_InicioAfastamento" visible="false">
                                    <div class="form-group">
                                        <label>Data Afastamento </label>
                                        <asp:TextBox ID="txtdtInicioAfastamento" class="form-control " type="date" data-mask="00/00/0000" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="div_RetornoAfastamento" visible="false">
                                    <div class="form-group">
                                        <label>Data Retorno </label>
                                        <asp:TextBox ID="txtdtRetornoAfastamento" class="form-control " type="date" data-mask="00/00/0000" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12" runat="server" id="div_sObservacaoEvento">
                            <div class="form-group">
                                <label>Observações</label>
                                <asp:TextBox ID="txtsObservacaoEvento" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="150px"></asp:TextBox>
                            </div>
                        </div>

                    </div>

                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <%--------------------------------------Tab-Arquivo----------------------------------------------------------------------------------------------------------------------------------------------------------------%>
        <div role="tabpanel" class="tab-pane fade" id="arquivo" aria-labelledby="Arquivo">
            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px" runat="server" id="DIV_Arquivos">
                <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
            </div>
        </div>
        <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>

    </div>


    <fieldset class="form-stacked actions">
        <asp:Button ID="BtnSalvarEvento" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="BtnSalvarEvento_Click" />
        &nbsp;
                <asp:Button ID="cmdPDF" class="btn btn-lg btn-danger" runat="server" Text="" OnClick="cmdPDF_Click" />
        &nbsp;
                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field-cancel" title="Voltar" onclick="history.go(-1)">&nbsp;
            
    </fieldset>


</asp:Content>
