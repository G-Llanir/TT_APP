<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SwitchAtivo.ascx.cs" Inherits="TT_Colaborador.Aplicativo.Controles.SwitchAtivo" %>

<style>
    .switch {
        position: relative;
        display: inline-block;
        width: <%= sTamanho_Switch %>;
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
        background-color: <%= sCorFundo_Nao %>;
        transition: .4s;
        border-radius: 34px;
    }

        .slider:before {
            content: <%= sNao %>;
            position: absolute;
            left: <%= sPosicao_Nao %>;
            top: 8%;
            color: <%= sCorTexto_Nao %>;
            font-size: .85em;
            font-weight: bold;
            transition: .4s;
        }

    input:checked + .slider {
        background-color: <%= sCorFundo_Sim %>;
    }

        input:checked + .slider:before {
            content: <%= sSim %>;
            color: <%= sCorTexto_Sim %>;
            left: 10%;
        }

    .slider:after {
        content: "";
        position: absolute;
        height: 20px;
        width: 20px;
        left: 3px;
        bottom: 2.23px;
        background-color: white;
        transition: .4s;
        border-radius: 15px;
    }

    input:checked + .slider:after {
        transform: translateX(<%= sPosicao_Sim %>);
    }

    .switch-bloquear {
        pointer-events: none;
    }
</style>

<div style="display: flex;align-items: center;min-height: 38px;">

    <label runat="server" id="lblTitulo" style="white-space: nowrap;"></label>

    <label class="<%= hddSwitchBloquear.Value == "S" ? "switch switch-bloquear" : "switch" %>">
        <input type="checkbox" id="<%= this.ClientID %>_idSwitch" <%= hddSwitch.Value == "S" ? "checked='checked'" : "" %>>
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
