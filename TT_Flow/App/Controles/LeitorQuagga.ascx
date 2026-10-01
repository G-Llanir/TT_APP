<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeitorQuagga.ascx.cs" Inherits="TT_Flow.App.Controles.LeitorQuagga" %>

<div class="row">
    <div class="col-lg-12">
        <div class="row">
            <div class="col-xs-12" id="DivBipador" runat="server">
                <div class="col-lg-12">
                    <div class="table-responsive" style="border: 2px solid #ccc; border-radius: 5px; margin-bottom: 10px;">
                        <div id="camera"></div>
                        <div id="resultado" style="font-size: 18px;"></div>
                    </div>
                </div>
            </div>
        </div>
        <br />
          <div class="row">
            <div class="col-lg-12" id="DivManual" runat="server">
                <div class="col-xs-6">
                    <asp:TextBox ID="txtsCodigoControle" CssClass="form-control" runat="server" placeholder="Código de Barras" style="display: none;"></asp:TextBox>
                </div>
            </div>
        </div>
        <br />
    </div>
</div>
