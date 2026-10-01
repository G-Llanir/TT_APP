<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChartControl.ascx.cs" Inherits="TT_Flow.App.Controles.ChartControl" %>

<div style="<%= ContainerStyle %>">
    <canvas id="<%= this.ClientID %>_chartCanvas"></canvas>
</div>
