
<%@ Page Language="C#" validateRequest="false"  AutoEventWireup="true" CodeBehind="ConsultarEnvios.aspx.cs" Inherits="TT_Hub.T_Box.ConsultarEnvios" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>T-Box - Últimos eventos</title>
        <link href="https://login.tecandtec.com.br/app/css/bootstrap.min.css" rel="stylesheet"/>
        <link href="https://login.tecandtec.com.br/app/css/sb-admin.css" rel="stylesheet"/>
        <link href="https://login.tecandtec.com.br/app/css/TT.css" rel="stylesheet" />

</head>
<body>
    <form id="frmConsultar" runat="server">
        <div>

            <div class="form-stacked" align="left">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Configuração</b></h3>
                    </div>
                    <div class="panel-body">
						<div class="col-lg-12">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Forçar Retorno</label>
                                        <asp:TextBox ID="txtsRetorno" class="form-control"  runat="server" TextMode="MultiLine" Height="140px"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <fieldset class="form-stacked actions">
                            <asp:Button ID="cmdSalvarDados" class="btn btn-sm btn-success" runat="server" Text="Salvar"  OnClick="cmdSalvar_Click"  />
                            &nbsp;
                            <asp:Button ID="cmdLimparEnvios" class="btn btn-sm btn-warning" runat="server" Text="Limpar Eventos " OnClick="cmdLimparEnvios_Click"  />
       
                        </fieldset>
                        
                    </div>
                </div>
            </div>


            <br />
            <br />
            <br />
            <br />
            


            <div class="form-stacked" align="left">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Últimos eventos</b></h3>
                    </div>
                    <div class="panel-body">
						<div class="col-lg-12">
                        <asp:GridView ID="gvResultado" class="table table-striped table-bordered table-hover table-condensed "  Width="98%" CellSpacing="1"  CellPadding="1"   AutoGenerateColumns="False" GridLines="None"   runat="server">

                <Columns>
                 <asp:BoundField DataField="idRegistro" HeaderText="ID" >
                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                    <ItemStyle Width="20px"  HorizontalAlign="Left" VerticalAlign="Middle" />
                </asp:BoundField>
                
                 <asp:BoundField DataField="dtregistro" HeaderText="Data/Hora" >
                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                    <ItemStyle Width="20px"  HorizontalAlign="Left" VerticalAlign="Middle" />
                </asp:BoundField>
                <asp:BoundField DataField="sStringRecebida" HeaderText="Corpo" >
                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                </asp:BoundField>

                <asp:BoundField DataField="sIP" HeaderText="IP" >
                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                    <ItemStyle Width="20px"  HorizontalAlign="Left" VerticalAlign="Middle" />
                </asp:BoundField>

                <asp:BoundField DataField="sTamanhoPost" HeaderText="Tamanho" >
                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                    <ItemStyle Width="20px"  HorizontalAlign="Left" VerticalAlign="Middle" />
                </asp:BoundField>

                </Columns>
                

            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
