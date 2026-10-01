<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SwitchAtivo.ascx.cs" Inherits="TT_Flow.App.Controles.SwitchAtivo" %>

<style>
    #<%= ClientID %>.switch-ativo {
        --switch-tamanho: <%= sTamanho_Switch %>;
        --cor-nao: <%= sCorFundo_Nao %>;
        --cor-sim: <%= sCorFundo_Sim %>;
        --cor-texto-nao: <%= sCorTexto_Nao %>;
        --cor-texto-sim: <%= sCorTexto_Sim %>;
        --posicao-nao: <%= sPosicao_Nao %>;
        --posicao-sim: <%= sPosicao_Sim %>;
    }

    #<%= ClientID %>.switch-ativo .switch {
        position: relative;
        display: inline-block;
        width: var(--switch-tamanho);
        height: 25px;
    }

    #<%= ClientID %>.switch-ativo .switch input {
        opacity: 0;
        width: 0;
        height: 0;
    }

    #<%= ClientID %>.switch-ativo .slider {
        position: absolute;
        cursor: pointer;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background-color: var(--cor-nao);
        transition: .4s;
        border-radius: 34px;
    }

    #<%= ClientID %>.switch-ativo .slider:before {
        content: <%= sNao %>;
        position: absolute;
        left: var(--posicao-nao);
        top: 10%;
        color: var(--cor-texto-nao);
        font-size: 1.4rem;
        font-weight: bold;
        transition: .4s;
    }

    #<%= ClientID %>.switch-ativo input:checked + .slider {
        background-color: var(--cor-sim);
    }

    #<%= ClientID %>.switch-ativo input:checked + .slider:before {
        content: <%= sSim %>;
        color: var(--cor-texto-sim);
        left: 7.5%;
    }

    #<%= ClientID %>.switch-ativo .slider:after {
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

    #<%= ClientID %>.switch-ativo input:checked + .slider:after {
        transform: translateX(var(--posicao-sim));
    }

    #<%= ClientID %>.switch-ativo .switch-bloquear {
        pointer-events: none;
    }
</style>

<div id="<%= ClientID %>" class="switch-ativo">
    <div style="display: table-caption;">

        <label runat="server" id="lblTitulo" style="white-space: nowrap;"></label>

        <label class='<%= hddSwitchBloquear.Value == "S" ? "switch switch-bloquear" : "switch" %>'>
            <input type="checkbox" id="<%= ClientID %>_idSwitch" <%= CheckboxCheckedAttribute %> />
            <span class="slider"></span>
        </label>

        <asp:HiddenField ID="hddSwitch" runat="server" />
        <asp:HiddenField ID="hddSwitchBloquear" runat="server" />

    </div>
</div>