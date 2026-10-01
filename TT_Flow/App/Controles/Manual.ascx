<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Manual.ascx.cs" Inherits="TT_Flow.App.Controles.Manual" %>

<style>
    .btn:focus {
        outline: none;
    }

    .divManual {
        position: fixed;
        top: 15%;
        right: 0;
        transition: 1s;
        z-index: 1000;
    }

        .divManual button {
            font: bold 1.8rem Arial !important;
        }

        .divManual > button {
            border-radius: 4px 0 0 4px;
        }

    @media (max-width: 1400px) {
        .divManual {
            top: 20%;
        }
    }

    @media (max-width: 768px) {
        .divManual {
            top: 25%;
        }
    }

    .semBotoes .introjs-tooltipbuttons {
        display: none;
    }

    .introjs-button {
        padding: 1rem;
        border-radius: .5em;
        color: white;
        font-size: 1.5rem;
        font-weight: bold;
        font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
        text-shadow: none;
    }

    .introjs-prevbutton {
        background-color: #d9534f;
        border-color: #d43f3a;
    }

        .introjs-prevbutton:focus {
            background-color: #d9534f;
            border-color: #d43f3a;
            color: white;
        }

        .introjs-prevbutton:hover {
            background-color: #c9302c;
            border-color: #ac2925;
            color: white;
        }

        .introjs-prevbutton:active {
            background-color: #ac2925;
            border-color: #761c19;
            color: white;
        }

    .introjs-nextbutton, .introjs-donebutton {
        background-color: #5cb85c;
        border-color: #4cae4c;
    }

        .introjs-nextbutton:focus, .introjs-donebutton:focus {
            background-color: #5cb85c;
            border-color: #4cae4c;
            color: white;
        }

        .introjs-nextbutton:hover, .introjs-donebutton:hover {
            background-color: #449d44;
            border-color: #398439;
            color: white;
        }

        .introjs-nextbutton:active, .introjs-donebutton:active {
            background-color: #398439;
            border-color: #255625;
            color: white;
        }

    .permiteClick_Atraves {
        pointer-events: none;
    }

    .introjs-tooltip {
        min-width: min-content;
        max-width: max-content;
        width: 50rem;
        border-radius: 15px;
    }

    @media (max-width: 1400px) {
        .introjs-tooltip {
            width: 30rem;
        }
    }

    .introjs-tooltip-header {
        padding: 10px 20px;
        border-radius: 13px 13px 0 0;
        background-color: rgb(0, 160, 0);
    }

        .introjs-tooltip-header > a {
            color: black;
        }

    .introjs-arrow.top {
        border-bottom-color: rgb(0, 160, 0);
    }

    .introjs-tooltiptext {
        padding: 5px 20px;
    }

    .bordaDivisoria {
        border-left: solid 1px forestgreen !important;
    }
</style>

<div class="divManual btn-group">
    <button type="button" class="btn btn-success escondeManual" data-toggle="tooltip_left" title="Expandir"><i class="fa fa-chevron-right"></i></button>
    <div class="divEsconde btn-group">
        <button type="button" runat="server" id="cmdManual" class="btn btn-success bordaDivisoria">Manual</button>
        <button type="button" runat="server" id="cmdIntroducao" class="btn btn-success bordaDivisoria">Introdução</button>
        <button type="button" runat="server" id="cmdGuia" class="btn btn-success bordaDivisoria">Modo Guia</button>
    </div>
</div>