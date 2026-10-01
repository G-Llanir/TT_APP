<%@ Page Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Manutencao_NR.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Manutencao_NR" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">


    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="NR - Normas Regulamentadoras"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <asp:Panel ID="pnManutencaoNR" runat="server">

                <div class="col-lg-12 form-group" style="padding: 0px;">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <div class="panel panel-primary" id="div_gvConsulta" runat="server">

                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Manutenção</h3>
                        </div>

                        <div class="panel-body">

                            <div class="form-group">

                                <%-- <div class="col-lg-12" runat="server" id="div_editar">
                                    <div class="form-group">
                                        <asp:Button runat="server" ID="btnEditarValidade" class="btn btn-warning" Text="Editar Validades" OnClick="btnEditarValidade_Click" />
                                    </div>
                                </div>--%>

                                <div class="col-lg-12" runat="server" id="div_salvar">
                                    <div class="form-group">
                                        <asp:Button runat="server" ID="btnSalvar" class="btn btn-success" Text="Salvar" />
                                        <asp:Button runat="server" ID="btnNovo" class="btn btn-primary" Text="Novo" />
                                    </div>
                                </div>

                            </div>

                            <div class="table-responsive form-group col-lg-12">
                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover " DataKeyNames="idTipoNR"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">

                                    <Columns>

                                        <asp:BoundField DataField="idTipoNR" HeaderText="ID">
                                            <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscNR" HeaderText="Documento">
                                            <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="Validade Administrativo">
                                            <HeaderStyle Width="15%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <div class="form-stacked row">
                                                    <div class="col-lg-5">
                                                        <asp:TextBox runat="server" ID="txtnValidadeAdministrativo" Width="100%" class="form-control" TextMode="Number" min="0" Text='<%# Eval("nValidade_Administrativo") %>'></asp:TextBox>
                                                    </div>
                                                    <div class="col-lg-5">
                                                        <asp:DropDownList runat="server" ID="ddlsPeriodicidadeAdministrativo" Width="100%" class="form-control" SelectedValue='<%# Eval("sPeriodicidadeAdministrativo") %>'>
                                                            <asp:ListItem Value="M">Mês</asp:ListItem>
                                                            <asp:ListItem Value="A">Ano</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-lg-1">
                                                        <asp:CheckBox runat="server" ID="cbValidadeAdministrativa" Checked='<%# Eval("sValidadeAdministrativo").Equals("S")?true:false%>' />
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Validade Operacional">
                                            <HeaderStyle Width="15%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <ItemTemplate>

                                                <div class="form-stacked row">
                                                    <div class="col-lg-5">
                                                        <asp:TextBox runat="server" ID="txtnValidadeOperacional" class="form-control" TextMode="Number" min="0" Text='<%# Eval("nValidade_Operacional") %>'></asp:TextBox>
                                                    </div>
                                                    <div class="col-lg-5">
                                                        <asp:DropDownList runat="server" ID="ddlsPeriodicidadeOperacional" class="form-control" SelectedValue='<%# Eval("sPeriodicidadeOperacional") %>'>
                                                            <asp:ListItem Value="M">Mês</asp:ListItem>
                                                            <asp:ListItem Value="A">Ano</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-lg-1">
                                                        <asp:CheckBox runat="server" ID="cbValidadeOperacional" Checked='<%# Eval("sValidadeOperacional").Equals("S")?true:false%>' />
                                                    </div>
                                                </div>

                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário Atualização">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>


            </asp:Panel>

        </div>
    </div>

    <div id="dialog-Salvar" class="modal" title="Salvar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloSalvar" runat="server" Text="Salvar"></asp:Label>
        </p>
    </div>

    <div class="modal fade" id="modalNovo">
        <div class="modal-dialog" style="width: 60%;">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3 class="modal-title">Novo NR</h3>
                </div>
                <div class="modal-body">

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Incluir" />

                    <div class="col-lg-3 form-group">

                        <label>Tipo NR</label>
                        <asp:TextBox runat="server" ID="txtNr" placeholder="Insira o NR" class="form-control"></asp:TextBox>

                    </div>

                    <div class="col-lg-9 form-group">

                        <label>Descrição</label>
                        <asp:TextBox runat="server" ID="txtDescricao" placeholder="Insira a descrição" class="form-control"></asp:TextBox>

                    </div>

                    <div class="col-lg-3 form-group">

                        <label>Validade Administrativa</label>
                        <div class="input-group" style="width: 100%; gap: 2%; display: flex">

                            <asp:TextBox runat="server" ID="txtValidadeAdm" class="form-control" Style="width: 40%" TextMode="Number"></asp:TextBox>

                            <asp:DropDownList runat="server" ID="ddlValidadeAdministrativa" placeholder="Selecione uma Validade" class="form-control" Style="width: 40%; z-index: 1; background-color: #fff; text-align: start">
                                <asp:ListItem Text="Ano" Value="A" />
                                <asp:ListItem Text="Mês" Value="M" />
                            </asp:DropDownList>
                        </div>

                    </div>

                    <div class="col-lg-3 form-group">

                        <label>Validade Operacional</label>


                        <div class="input-group" style="width: 100%; gap: 2%; display: flex">
                            <asp:TextBox runat="server" ID="txtValidadeOpe" class="form-control" Style="width: 40%" TextMode="Number"></asp:TextBox>

                            <asp:DropDownList runat="server" ID="ddlPeriocidadeOperacional" placeholder="Selecione uma Validade" class="form-control" Style="width: 40%; z-index: 1; background-color: #fff; text-align: start">
                                <asp:ListItem Text="Ano" Value="A" />
                                <asp:ListItem Text="Mês" Value="M" />
                            </asp:DropDownList>
                        </div>

                    </div>

                </div>

                <div class="modal-footer">
                    <asp:Button Text="Incluir" runat="server" ID="btnInlcuir" class="btn-success btn" OnClick="btnInlcuir_Click" />
   
                </div>
            </div>
        </div>
    </div>
</asp:Content>
