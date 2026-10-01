<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="RegistroVeiculo_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.RegistroVeiculo_Detalhe" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height:600px" runat ="server" visible="false"></div>


    <script type="text/javascript">

        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }


        $(function () {


            $('[id*=txtdtVencimentoSeguro]').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            });


            $('[id*=txtsAnoModelo]').mask('9999/9999');
            $('[id*=txtsRenavam]').mask('999999999999');
            $('[id*=txtsPlaca]').mask('SSS-0A00');
            $('[id*=txtdtVencimentoSeguro]').mask('99/99/9999');


        });

    </script>


        <div class="form-stacked row">
            <div class="col-lg-12">
                <h1><asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                <uc1:BreadCrumb runat="server" ID="BreadCrumb"  NivelPagina="3" TitulodaPagina="Detalhe"/>
            </div>
        </div>
                   
        <div class="form-stacked row">
            
            <div class="col-lg-12">
                <uc1:MensagemPagina runat="server" id="MensagemPagina" />
            </div>
            
            <div class="col-lg-12">
                <div class="form-group">
                    <label>ID </label>
                    <asp:TextBox ID="txtidVeiculo" class="form-control CaixaTextoMini" runat="server"  ></asp:TextBox>
                </div>
            </div>
            
            <div class="col-lg-6">
                <div class="form-group row">
                    <div class="col-lg-4">
                        <div class="form-group">
                            <label>Placa </label>
                            <asp:TextBox ID="txtsPlaca" class="form-control uppercase " runat="server"  MaxLength="8" ></asp:TextBox>
                        </div>
                    </div>
                    
                </div>
            </div>
             
            <div class="col-lg-12">
                <div class="form-group row">

                    <div class="col-lg-6">
                        <div class="form-group">
                            <label>Marca </label>
                            <asp:TextBox ID="txtsMarca" class="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>

                     <div class="col-lg-6">
                        <div class="form-group">
                            <label>Modelo </label>
                            <asp:TextBox ID="txtsModelo" class="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-6 ">
                <div class="form-group row">
                    
                    <div class="col-lg-6">
                        <div class="form-group">
                            <label>RENAVAM </label>
                            <asp:TextBox ID="txtsRenavam" class="form-control " runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="col-lg-6">
                        <div class="form-group">
                            <label>Cor </label>
                            <asp:TextBox ID="txtsCor" class="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
           
             <div class="col-lg-6">
                <div class="form-group row">
                     <div class="col-lg-4">
                        <div class="form-group">
                            <label>Ano/Modelo</label>
                            <asp:TextBox ID="txtsAnoModelo" class="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>  
                </div>
            </div>
             

             <div class="col-lg-6">
                <div class="form-group">
                    <label>Seguro </label>
                    <asp:TextBox ID="txtsSeguro" class="form-control" runat="server" MaxLength="100"></asp:TextBox>
                </div>
            </div>

             <div class="col-lg-6">
                <div class="form-group">
                    <label>Vencimento Seguro </label>
                    <asp:TextBox ID="txtdtVencimentoSeguro" class="form-control CaixaTextoGrande" runat="server"></asp:TextBox>
                </div>
            </div>
            
            <div class="col-lg-4">
                <div class="form-group">
                    <label>Local </label>
                    <asp:TextBox ID="txtsLocal" class="form-control" runat="server" MaxLength="100"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-2">
                <div class="form-group">
                    <label>Possui Rastreador? </label>
                    <asp:DropDownList ID="ddlsRstreador" runat="server" class="form-control yes_no select" >
                        <asp:ListItem Value="Sim">Sim</asp:ListItem>
                        <asp:ListItem Value="Não">Não</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="col-lg-4">
                <div class="form-group">
                    <label>Proprietario </label>
                    <asp:TextBox ID="txtsProprietario" class="form-control" runat="server" MaxLength="100"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-4">
                <div class="form-group">
                    <label>Km Contrato </label>
                    <asp:TextBox ID="txtsContratoKm" class="form-control" runat="server" MaxLength="100"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-4">
                <div class="form-group">
                    <label>Duração Contrato </label>
                    <asp:TextBox ID="txtsContratoDuracao" class="form-control" runat="server" MaxLength="100"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-2">
                <div class="form-group">
                    <label>Data contrato </label>
                    <asp:TextBox ID="txtdtContrato" class="form-control" runat="server" MaxLength="100"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-2">
                <div class="form-group">
                    <label>Dia Rodizio </label>
                    <asp:DropDownList ID="ddlsRodizio" runat="server" class="form-control yes_no select" >
                        <asp:ListItem Value="Segunda-feira">Segunda-feira</asp:ListItem>
                        <asp:ListItem Value="Terça-feira">Terça-feira</asp:ListItem>
                        <asp:ListItem Value="Quarta-feira">Quarta-feira</asp:ListItem>
                        <asp:ListItem Value="Quinta-feira">Quinta-feira</asp:ListItem>
                        <asp:ListItem Value="Sexta-feira">Sexta-feira</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="col-lg-4">
                <div class="form-group">
                    <label>Tipo Registro </label>
                    <asp:TextBox ID="txtsTipoRegistro" class="form-control" runat="server" MaxLength="100"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-2">
                <div class="form-group">
                    <label>N.º Lugares</label>
                    <asp:TextBox ID="txtsLugaresAuto" class="form-control" runat="server" MaxLength="100"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-12">
                <div class="form-group">
                    <label>Dados do Seguro </label>
                    <asp:TextBox ID="txtsDadosSeguto" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="120px"></asp:TextBox>
                </div>
            </div>



        </div>
        <br />
        
           
        
        <uc1:PainelAtualizacao runat="server" id="PainelAtualizacao" />
             
            
        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
            &nbsp;
            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;
        </fieldset>

        <asp:HiddenField ID="hddidVeiculo" runat="server" />


</asp:Content>
