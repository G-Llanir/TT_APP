<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimerPicker.ascx.cs" Inherits="TT_Flow.App.Controles.TimerPicker" %>



<link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-timepicker/0.5.2/css/bootstrap-timepicker.min.css" rel="stylesheet">
<script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-timepicker/0.5.2/js/bootstrap-timepicker.min.js"></script>

      
<link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap-glyphicons.css">


<b><asp:Label ID="lbl" runat="server" Text="Label"></asp:Label></b>
<asp:TextBox ID="txtbTimerPicker" runat="server" data-provide="timepicker" class="timepicker"></asp:TextBox>
<%--<input id="txtTimerPicker" runat="server" data-provide="timepicker" class="timepicker" />--%>
