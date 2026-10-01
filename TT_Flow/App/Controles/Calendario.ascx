<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendario.ascx.cs" Inherits="TT_Flow.App.Controles.Calendario" %>

<script src="/App/JS/FullCalendar.global.min.js" type="text/javascript"></script>
<script src="/App/JS/sweetalert2-v11_15_2.js" type="text/javascript"></script>

<style>
    .div_dataEvento {
        transition: linear ease-in-out .35s;
    }

    .hide {
        display: none;
    }

    .show {
        display: block;
    }

    .btn-url {
        border: solid 1px grey;
        border-radius: 5px;
    }

        .btn-url:hover {
            background-color: #ddd;
        }

        .btn-url:active {
            background-color: #ddd;
        }

        .btn-url:focus {
            background-color: #ddd;
        }

    /* ----------------------------------------------------------------------------- */
    /* Estilos para os modais e os campos dentro deles, do FullCalendar */

    .modalSwal_Personalizado.pequeno {
        width: 25% !important;
    }

    .modalSwal_Personalizado.medio {
        width: 35% !important;
    }

    .modalSwal_Personalizado.grande {
        width: 50% !important;
    }

    .modalSwal_Personalizado.gigante {
        width: 75% !important;
    }

    div:where(.swal2-container) div:where(.swal2-popup) {
        padding: 0 0 1em;
    }

    div:where(.swal2-container) h2:where(.swal2-title) {
        padding: .8em 1em .5em;
        font-size: 2em;
    }

    div:where(.swal2-container) .swal2-html-container {
        padding: 1em 1.6em 0;
        font-size: 1.25em;
    }

    div:where(.swal2-container) input {
        margin: 0;
    }

    div:where(.swal2-container) div:where(.swal2-actions) {
        margin: 0;
        justify-content: start;
        padding: 0 30px;
    }

    div:where(.swal2-draggable) div:where(.swal2-actions) {
        justify-content: center !important;
        padding: 0 !important;
    }

    div:where(.swal2-container) button:where(.swal2-styled) {
        font-size: 1.5em;
    }

    div:where(.swal2-container) div:where(.swal2-validation-message) {
        background: #eee;
        font-size: 1.25em;
    }

    /* ----------------------------------------------------------------------------- */
    /* Estilos para o switch de 'Evento o dia inteiro?' */

    .switchPersonalizado {
        display: inline-flex;
        align-items: start;
        justify-content: start;
        width: 75px;
        height: 35px;
        background-color: #ccc;
        border-radius: 50em;
        cursor: pointer;
        transition: background-color 0.4s;
        padding: 5px;
    }

        .switchPersonalizado input {
            display: none;
        }

        .switchPersonalizado .spanSwitch {
            display: block;
            width: 38%;
            aspect-ratio: 1;
            background-color: white;
            border-radius: 50%;
            transition: transform 0.4s;
            transform: translateX(0);
        }

    .switchAtivo {
        transform: translateX(160%) !important;
    }

    /* ----------------------------------------------------------------------------- */
    /* Estilos para o calendário ser renderizado corretamente */

    [id*="div_calendario"] {
        height: 100vh;
        max-height: 100%;
        z-index: 0;
        position: relative;
    }

    /* ----------------------------------------------------------------------------- */
    /* Estilos para os Eventos inativos */

    .eventoInativo {
        background-color: #eee !important;
        color: black !important;
        border-color: #aaa !important;
        text-decoration: line-through;
        pointer-events: all;
    }

    /* ----------------------------------------------------------------------------- */
    /* Estilos para os Toasts de mensagens */

    .custom_toast {
        font-size: 1.5rem !important;
        border-radius: 10px;
        box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.2);
    }

    .custom_toast.swal2-icon-success {
        background-color: #a5dc86 !important;
    }

    .custom_toast.swal2-icon-error {
        background-color: #f27474 !important;
    }

    .custom_toast.swal2-icon-warning {
        background-color: #f8bb86 !important;
    }

    .custom_toast.swal2-icon-info {
        background-color: #3fc3ee !important;
    }

    .custom_toast.swal2-icon-question {
        background-color: #87adbd !important;
    }

    /* ----------------------------------------------------------------------------- */
    /* Estilos para os Toasts de mensagens */

    .fc-list-event .fc-list-event-graphic {
        padding-left: 0 !important;
    }

    .fc-list-event .fc-list-event-title {
        padding-left: 5px !important;
    }
</style>

<div runat="server" id="div_calendario" class="calendario"></div>

<asp:HiddenField runat="server" ID="hdd_RegistraScript_Global" Value="true" />