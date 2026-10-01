<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SwitchAtivo.ascx.cs" Inherits="TT_Flow.App.Controles.SwitchAtivo" %>

<style>
    .switch {
        position: relative;
        display: inline-block;
        width: 60px;
        height: 25px;
    }

        .switch input {
            opacity: 0;
            width: 0;
            height: 0;
        }

    .slider {
        position: absolute;
        cursor: pointer;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background-color: red; 
        transition: .4s;
        border-radius: 34px;
    }

        .slider:before {
            content: "Não";
            position: absolute;
            left: 70%;
            top: 50%;
            transform: translate(-50%, -50%);
            color: white;
            font-size: 12px;
            font-weight: bold;
            transition: .4s;
        }

    input:checked + .slider {
        background-color: green; 
    }

        input:checked + .slider:before {
            content: "Sim";
            left: 30%;
        }

    .slider:after {
        content: "";
        position: absolute;
        height: 20px;
        width: 22px;
        left: 3px;
        bottom: 2.5px;
        background-color: white;
        transition: .4s;
        border-radius: 15px;
    }

    input:checked + .slider:after {
        transform: translateX(34px);
    }

    .switch-bloquear {
        pointer-events: none;
    }
</style>

<div class="form-group" style="display: table-caption;">

    <label>
        <asp:Label runat="server" ID="lblTitulo" Style="white-space: nowrap;"></asp:Label>
    </label>

    <label class="<%= hddSwitchBloquear.Value == "S" ? "switch switch-bloquear" : "switch" %>">
        <input type="checkbox"  id="<%= this.ClientID %>_idSwitch" <%= hddSwitch.Value == "S" ? "checked='checked'" : "" %>>
        <span class="slider"></span>
    </label>

    <asp:HiddenField ID="hddSwitch" runat="server" Value="<%= hddSwitch.Value %>" />
    <asp:HiddenField ID="hddSwitchBloquear" runat="server" Value="<%= hddSwitchBloquear.Value %>" />
</div>


<%--Thiago Rodrigues 21/11/2024--%>
<script>
    (function (idSwitchClientID, hddSwitchClientID) {
        function initSwitch() {
            var idSwitch = document.getElementById(idSwitchClientID);
            var hddSwitch = document.getElementById(hddSwitchClientID);

            if (!idSwitch || !hddSwitch) {
                console.warn('Elementos não encontrados para SwitchAtivo.');
                return;
            }

            idSwitch.addEventListener('change', function () {
                hddSwitch.value = idSwitch.checked ? "S" : "N";
            });
        }

        // Reexecutar após atualização parcial do UpdatePanel
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            initSwitch();
        });

        // Inicializar no carregamento inicial
        document.addEventListener('DOMContentLoaded', function () {
            initSwitch();
        });
    })('<%= this.ClientID %>_idSwitch', '<%= hddSwitch.ClientID %>');
</script>

<%--<script>
    (function (idSwitchClientID, hddSwitchClientID) {
        document.addEventListener('DOMContentLoaded', function () {

            var idSwitch = document.getElementById(idSwitchClientID);
            var hddSwitch = document.getElementById(hddSwitchClientID);
            //console.log('hddSwitch element:', hddSwitch);
            idSwitch.addEventListener('change', function () {
                if (idSwitch.checked) {
                    hddSwitch.value = "S";
                } else {
                    hddSwitch.value = "N";
                }
                console.log(hddSwitch.value);
            });

        });
    })('<%= this.ClientID %>_idSwitch', '<%= hddSwitch.ClientID %>');
</script>--%>



