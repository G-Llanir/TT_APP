<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Workflow_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.IA.Workflow_Detalhe" ResponseEncoding="utf-8" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta charset="utf-8" />
    <link href="/App/css/vendor/drawflow.min.css" rel="stylesheet" />
    <script src="/App/JS/vendor/drawflow.min.js" charset="utf-8"></script>
    <style>
        .wf-mono { font-family: Consolas, Monaco, monospace; }
        .wf-page-head small { color: #607487; }
        .wf-tab-content { padding-top: 12px; }
        .wf-input-list { display: grid; gap: 8px; }
        .wf-input-sort-hint { margin: 0 0 8px; padding: 7px 9px; border: 1px solid #dbe5ee; border-radius: 6px; background: #fbfdff; color: #52687c; font-size: 12px; }
        .wf-input-sort-hint i { color: #3d7fc2; margin-right: 4px; }
        .wf-input-row { display: grid; grid-template-columns: 28px minmax(0, 1fr) 34px; gap: 8px; align-items: stretch; padding: 8px; border: 1px solid #dbe5ee; border-radius: 7px; background: #fff; box-shadow: 0 1px 3px rgba(31, 49, 68, .04); }
        .wf-input-row.wf-input-drop-target { border-color: #2f80d0; box-shadow: 0 0 0 3px rgba(47, 128, 208, .14); }
        .wf-input-row.wf-input-dragging { opacity: .55; }
        .wf-input-order { display: flex; flex-direction: column; gap: 4px; align-items: stretch; }
        .wf-input-order .btn { width: 28px; height: 24px; padding: 0; }
        .wf-input-drag { cursor: grab; color: #52687c; }
        .wf-input-drag:active { cursor: grabbing; }
        .wf-input-card-body { min-width: 0; }
        .wf-input-grid { display: grid; grid-template-columns: minmax(130px, 1.25fr) 105px minmax(130px, 1fr) 132px; gap: 7px; align-items: start; }
        .wf-input-field label { display: block; margin: 0 0 3px; color: #52687c; font-size: 11px; font-weight: normal; }
        .wf-input-field .form-control { min-width: 0; }
        .wf-input-desc-wrap { margin-top: 7px; }
        .wf-input-desc-wrap textarea { min-height: 52px; resize: vertical; }
        .wf-input-remove { display: flex; align-items: flex-start; }
        .wf-input-remove .btn { width: 32px; padding-left: 0; padding-right: 0; }
        .wf-input-head { display: none; }
        .wf-input-tools { display: flex; gap: 5px; flex-wrap: wrap; align-items: center; margin-top: 8px; }
        .wf-input-legend { margin-top: 8px; color: #657789; }
        .wf-test-input-row { display: flex; gap: 6px; align-items: center; margin-bottom: 6px; }
        .wf-test-input-row label { width: 140px; margin: 0; font-family: Consolas, Monaco, monospace; font-size: 12px; font-weight: normal; }
        .wf-test-input-field { flex: 1; min-width: 0; }
        .wf-test-input-field .help-block { margin: 2px 0 0; font-size: 11px; color: #718291; }
        .wf-test-input-row input, .wf-test-input-row textarea { width: 100%; }
        .wf-produtos-input-row, .wf-lista-input-row { align-items: flex-start; }
        .wf-produtos-editor, .wf-lista-editor { border: 1px solid #dbe5ee; border-radius: 6px; background: #fbfdff; padding: 7px; }
        .wf-produtos-head, .wf-produto-row { display: grid; grid-template-columns: minmax(180px, 1.4fr) minmax(130px, .9fr) 82px 126px 82px 34px; gap: 5px; align-items: center; }
        .wf-produtos-head { color: #52687c; font-size: 11px; margin-bottom: 4px; }
        .wf-produtos-rows { display: grid; gap: 5px; margin-bottom: 6px; }
        .wf-produto-row .form-control, .wf-lista-row .form-control { min-width: 0; }
        .wf-lista-rows { display: grid; gap: 5px; margin-bottom: 6px; }
        .wf-lista-row { display: grid; grid-template-columns: minmax(120px, 1fr) 34px; gap: 5px; align-items: center; }
        .wf-lista-actions { display: flex; align-items: center; gap: 8px; flex-wrap: wrap; }
        .wf-lista-actions .text-muted { font-size: 11px; }
        .wf-test-group-title { margin: 8px 0 5px; padding-top: 6px; border-top: 1px solid #e3eaf2; color: #34495e; font-weight: bold; font-size: 12px; }
        .wf-test-group-title:first-child { margin-top: 0; padding-top: 0; border-top: 0; }
        .wf-conditional-inputs { margin-top: 10px; border: 1px dashed #b7c8d9; border-radius: 6px; padding: 8px 10px; background: #fbfcfd; }
        .wf-conditional-inputs summary { cursor: pointer; color: #415466; }
        .wf-conditional-group { margin-top: 8px; padding-top: 2px; }
        .wf-test-input-row-conditional label { color: #52687c; }
        .wf-conditional-list { display: flex; gap: 4px; flex-wrap: wrap; }
        .wf-shell { display: grid; grid-template-columns: 210px minmax(520px, 1fr) 330px; gap: 10px; min-height: 650px; }
        .wf-palette, .wf-props, .wf-canvas-wrap { border: 1px solid #d6e0ea; background: #fff; }
        .wf-palette, .wf-props { border-radius: 6px; padding: 10px; box-shadow: 0 1px 3px rgba(38, 58, 77, .05); }
        .wf-palette { max-height: 650px; overflow: auto; }
        .wf-canvas-wrap { position: relative; border-radius: 6px; overflow: hidden; box-shadow: inset 0 1px 0 rgba(255, 255, 255, .7); }
        .wf-canvas-toolbar { display: flex; align-items: center; gap: 6px; padding: 8px 10px; border-bottom: 1px solid #d6e0ea; background: #fbfcfd; overflow-x: auto; white-space: nowrap; }
        .wf-canvas-toolbar .wf-spacer { flex: 1; }
        .wf-canvas-toolbar .btn.active { background: #e8f3ff; border-color: #2f80d0; color: #225f9d; box-shadow: inset 0 1px 2px rgba(47, 128, 208, .18); }
        .wf-canvas-exec-status { min-height: 40px; padding: 8px 10px; border-bottom: 1px solid #d6e0ea; background: #fff; }
        .wf-canvas-exec-status .alert { margin-bottom: 0; padding: 6px 10px; }
        .wf-canvas-exec-actions { margin-top: 6px; display: flex; gap: 8px; align-items: flex-start; flex-wrap: wrap; }
        .wf-canvas-exec-actions .wf-canvas-exec-buttons { display: flex; gap: 6px; flex-wrap: wrap; }
        .wf-canvas-exec-actions .wf-canvas-exec-text { min-width: 220px; }
        .wf-canvas-exec-actions .wf-canvas-exec-desc { margin-top: 3px; color: #627181; }
        .wf-canvas { height: 590px; background-color: #f6f8fb; background-image: linear-gradient(#e3eaf2 1px, transparent 1px), linear-gradient(90deg, #e3eaf2 1px, transparent 1px); background-size: 24px 24px; }
        .wf-canvas.wf-pan-mode { cursor: grab; }
        .wf-canvas.wf-pan-active, .wf-canvas.wf-pan-mode:active { cursor: grabbing; }
        .wf-palette-title, .wf-props-title { margin: 0 0 8px; font-weight: bold; color: #2f4050; }
        .wf-palette-item { width: 100%; display: flex; align-items: center; gap: 8px; padding: 8px 9px; margin-bottom: 7px; border: 1px solid #d6dee8; border-left: 4px solid #8aa0b5; border-radius: 5px; background: #fff; cursor: grab; text-align: left; color: #2f4050; }
        .wf-palette-item:hover { border-color: #7aa7d9; background: #f5f9fd; box-shadow: 0 1px 4px rgba(46, 91, 135, .12); }
        .wf-palette-item[data-wf-tipo="ferramenta"] { border-left-color: #3d7fc2; }
        .wf-palette-item[data-wf-tipo="condicao"] { border-left-color: #d59b2f; }
        .wf-palette-item[data-wf-tipo="definir"] { border-left-color: #2d9aa6; }
        .wf-palette-item[data-wf-tipo="ia"] { border-left-color: #7b61a8; }
        .wf-palette-item[data-wf-tipo="merge"] { border-left-color: #587f92; }
        .wf-palette-item[data-wf-tipo="aprovacao"] { border-left-color: #b7722d; }
        .wf-palette-item[data-wf-tipo="loop"] { border-left-color: #3d8f77; }
        .wf-palette-item[data-wf-tipo="espera"] { border-left-color: #4d83aa; }
        .wf-palette-item[data-wf-tipo="fim"] { border-left-color: #6b7c8d; }
        .wf-palette-item i { width: 16px; text-align: center; }
        .wf-canvas-node { width: 250px; min-width: 190px; max-width: 310px; }
        .wf-node-head { display: flex; align-items: center; gap: 7px; font-weight: bold; min-width: 0; }
        .wf-node-title { min-width: 0; flex: 1; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
        .wf-node-id { font-family: Consolas, Monaco, monospace; font-size: 11px; color: #657789; white-space: nowrap; }
        .wf-node-min-btn { width: 22px; height: 22px; padding: 0; line-height: 20px; border: 1px solid #d1dde8; border-radius: 4px; background: #f8fbfd; color: #52687c; flex: 0 0 22px; }
        .wf-node-min-btn:hover { border-color: #7aa7d9; background: #eef6ff; color: #2f6fa8; }
        .wf-node-body { margin-top: 6px; max-height: 118px; overflow: auto; overflow-x: auto; padding-right: 2px; }
        .wf-node-caption { font-size: 12px; color: #2f4050; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
        .wf-node-meta { margin-top: 4px; font-size: 11px; color: #718291; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
        .wf-canvas-node.wf-node-collapsed { width: 205px; }
        .wf-canvas-node.wf-node-collapsed .wf-node-body { display: none; }
        .wf-node-input-pill { margin-top: 7px; display: inline-flex; align-items: center; gap: 5px; border: 1px solid #cdd9e5; border-radius: 12px; padding: 2px 8px; background: #f7fafc; color: #415466; font-size: 11px; cursor: pointer; }
        .wf-node-input-pill:hover { border-color: #7aa7d9; background: #eef6ff; color: #2f6fa8; }
        .wf-node-input-pill.wf-node-input-pendente { border-color: #d59b2f; background: #fff7e6; color: #8a6119; }
        .wf-tool-picker .chosen-container { width: 100% !important; }
        .wf-tool-picker .chosen-container-single .chosen-single { min-height: 30px; line-height: 28px; border-color: #cbd8e4; background: #fff; }
        .wf-tool-picker .chosen-container-single .chosen-single div b { background-position-y: 5px; }
        .wf-tool-picker .chosen-container .chosen-search input[type="text"] { min-height: 28px; }
        .wf-tool-picker .chosen-container .chosen-drop { z-index: 25; }
        .wf-tool-picker .chosen-container .chosen-results li.highlighted { background-color: #eef6ff; color: #253746; }
        .wf-list-tool-picker { margin-top: 7px; max-width: 520px; }
        .drawflow .drawflow-node { border: 1px solid #b8c9d9; border-radius: 6px; box-shadow: 0 2px 9px rgba(31, 49, 68, .12); background: #fff; color: #2f4050; width: auto; max-width: 330px; padding: 0; transition: box-shadow .15s ease, border-color .15s ease, background-color .15s ease; }
        .drawflow .drawflow-node:hover { box-shadow: 0 4px 12px rgba(31, 49, 68, .16); }
        .drawflow .drawflow-node.selected { background: #f8fbff !important; border-color: #2f80d0; box-shadow: 0 0 0 3px rgba(47, 128, 208, .22), 0 8px 18px rgba(31, 49, 68, .18); color: #253746; }
        .drawflow .drawflow-node .drawflow_content_node { padding: 10px 12px; }
        .drawflow .drawflow-node.wf-node-inicio { border-top: 4px solid #4f9d5d; }
        .drawflow .drawflow-node.wf-node-fim { border-top: 4px solid #6b7c8d; }
        .drawflow .drawflow-node.wf-node-ferramenta { border-top: 4px solid #3d7fc2; }
        .drawflow .drawflow-node.wf-node-condicao { border-top: 4px solid #d59b2f; }
        .drawflow .drawflow-node.wf-node-definir { border-top: 4px solid #2d9aa6; }
        .drawflow .drawflow-node.wf-node-ia { border-top: 4px solid #7b61a8; }
        .drawflow .drawflow-node.wf-node-merge { border-top: 4px solid #587f92; }
        .drawflow .drawflow-node.wf-node-aprovacao { border-top: 4px solid #b7722d; }
        .drawflow .drawflow-node.wf-node-loop { border-top: 4px solid #3d8f77; }
        .drawflow .drawflow-node.wf-node-espera { border-top: 4px solid #4d83aa; }
        .drawflow .drawflow-node.wf-status-ok { box-shadow: 0 0 0 2px rgba(79, 157, 93, .25); }
        .drawflow .drawflow-node.wf-status-erro { box-shadow: 0 0 0 2px rgba(197, 76, 73, .30); }
        .drawflow .drawflow-node.wf-status-erro-tratado { box-shadow: 0 0 0 2px rgba(213, 155, 47, .34); }
        .drawflow .drawflow-node.wf-status-pausado { box-shadow: 0 0 0 2px rgba(213, 155, 47, .32); }
        .drawflow .drawflow-node.wf-status-executando { box-shadow: 0 0 0 3px rgba(47, 128, 208, .28), 0 0 18px rgba(47, 128, 208, .22); animation: wfPulseExec 1.1s ease-in-out infinite; }
        @keyframes wfPulseExec {
            0%, 100% { transform: translateY(0); }
            50% { transform: translateY(-2px); }
        }
        .drawflow .drawflow-node .input, .drawflow .drawflow-node .output { width: 15px; height: 15px; background: #fff; border: 2px solid #8aa0b5; }
        .drawflow .drawflow-node .input { background: #f7fafc; border-color: #9aacbe; }
        .drawflow .drawflow-node .output { background: #eef6ff; border-color: #4f85bf; }
        .drawflow .drawflow-node.wf-node-condicao .output.output_1 { background: #edf9f0; border-color: #4f9d5d; }
        .drawflow .drawflow-node.wf-node-condicao .output.output_2 { background: #fff7e6; border-color: #d59b2f; }
        .drawflow .drawflow-node.wf-node-ferramenta .output.output_2,
        .drawflow .drawflow-node.wf-node-ia .output.output_2 { background: #fff1f0; border-color: #c65d5d; }
        .drawflow .drawflow-node.wf-node-aprovacao .output.output_1 { background: #edf9f0; border-color: #4f9d5d; }
        .drawflow .drawflow-node.wf-node-aprovacao .output.output_2 { background: #fff1f0; border-color: #c65d5d; }
        .drawflow .drawflow-node.wf-node-loop .output.output_1 { background: #eef6ff; border-color: #4f85bf; }
        .drawflow .drawflow-node.wf-node-loop .output.output_2 { background: #f3f6f9; border-color: #6b7c8d; }
        .drawflow .drawflow-node.wf-node-espera .output.output_1 { background: #eef7fb; border-color: #4d83aa; }
        .drawflow .connection .main-path { stroke: #63809b; stroke-width: 3px; }
        .drawflow .connection .main-path:hover { stroke: #2f80d0; }
        .drawflow .connection.output_1 .main-path { stroke: #4f9d5d; }
        .drawflow .connection.output_2 .main-path { stroke: #d59b2f; }
        .drawflow .connection .main-path.selected { stroke: #2f80d0 !important; }
        .drawflow-delete { background: #253746; border-color: #fff; box-shadow: 0 2px 8px rgba(31, 49, 68, .24); }
        .wf-prop-scroll { max-height: 585px; overflow: auto; padding-right: 2px; }
        .wf-ref { display: flex; gap: 4px; }
        .wf-ref select { width: 44%; }
        .wf-map-scroll { max-height: 330px; overflow: auto; overflow-x: auto; border: 1px solid #e6edf4; border-radius: 5px; background: #fff; }
        .wf-map-scroll .table { min-width: 560px; margin-bottom: 0; }
        .wf-map-row td { padding: 3px 4px; vertical-align: middle; }
        .wf-map-row .wf-param { font-family: Consolas, Monaco, monospace; font-size: 12px; white-space: nowrap; }
        .wf-param-summary { border: 1px solid #e3eaf2; border-radius: 6px; background: #fbfcfd; padding: 8px; margin: 8px 0 10px; }
        .wf-param-summary-line { display: flex; align-items: center; gap: 6px; font-size: 12px; margin-bottom: 4px; }
        .wf-param-summary-line:last-child { margin-bottom: 0; }
        .wf-param-summary .wf-param-preview { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; color: #52687c; }
        .wf-param-modal { z-index: 1065; }
        .wf-param-modal .modal-dialog { width: 92%; max-width: 1120px; }
        .wf-param-modal.wf-modal-fallback { display: block; opacity: 1; background: rgba(18, 31, 44, .38); overflow: auto; }
        .wf-param-modal .modal-content { border-radius: 6px; overflow: hidden; }
        .wf-param-modal .modal-header, .wf-param-modal .modal-footer { background: #fbfcfd; }
        .wf-param-modal .modal-body { max-height: calc(100vh - 210px); overflow: auto; background: #f6f8fb; }
        .wf-param-modal-title { display: flex; align-items: center; gap: 8px; min-width: 0; }
        .wf-param-modal-title strong { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
        .wf-param-modal-title .close { margin-left: auto; }
        .wf-param-card { border: 1px solid #d9e3ed; border-radius: 6px; background: #fff; margin-bottom: 9px; padding: 10px; }
        .wf-param-card.wf-param-missing { border-color: #d59b2f; box-shadow: inset 3px 0 0 #d59b2f; }
        .wf-param-card.wf-param-empty { border-color: #9fb3c7; box-shadow: inset 3px 0 0 #9fb3c7; }
        .wf-param-card-head { display: flex; align-items: flex-start; gap: 10px; margin-bottom: 8px; }
        .wf-param-card-main { min-width: 0; flex: 1; }
        .wf-param-card-main strong { font-family: Consolas, Monaco, monospace; color: #2f4050; }
        .wf-param-card-main .help-block { margin: 2px 0 0; }
        .wf-param-card-type { white-space: nowrap; color: #657789; font-size: 11px; }
        .wf-param-editor-row { display: grid; grid-template-columns: 190px minmax(0, 1fr) 130px; gap: 8px; align-items: start; }
        .wf-param-value { min-width: 0; }
        .wf-param-value textarea { resize: vertical; min-height: 62px; }
        .wf-param-empty-toggle { padding-top: 5px; font-size: 12px; white-space: nowrap; }
        .wf-param-empty-toggle input { margin-top: 0; vertical-align: middle; }
        .wf-param-modal-actions { display: flex; align-items: center; gap: 6px; flex-wrap: wrap; }
        .wf-param-status { margin-right: auto; color: #52687c; }
        .wf-req { color: #d9534f; font-weight: bold; }
        .wf-tool-desc { font-size: 11px; color: #777; margin: 4px 0 6px; }
        .wf-trace-passo, .wf-list-no { border: 1px solid #ddd; border-radius: 4px; padding: 8px 10px; margin-bottom: 7px; background: #fff; }
        .wf-trace-io { font-family: Consolas, Monaco, monospace; font-size: 11px; white-space: pre-wrap; word-break: break-word; color: #444; margin-top: 6px; background: #f7f7f7; border: 1px solid #eee; border-radius: 3px; padding: 6px 8px; }
        .wf-json-box { margin-top: 7px; border: 1px solid #d7e0ea; border-radius: 5px; background: #fbfcfe; overflow: hidden; }
        .wf-json-head { display: flex; align-items: center; gap: 8px; padding: 6px 8px; border-bottom: 1px solid #e5ebf2; color: #34495e; }
        .wf-json-head .btn { margin-left: auto; }
        .wf-json-code, .wf-json-full { margin: 0; font-family: Consolas, Monaco, monospace; font-size: 11px; line-height: 1.45; white-space: pre; overflow: auto; tab-size: 2; }
        .wf-json-code { max-height: 220px; padding: 8px; background: #101820; color: #dce7f0; }
        .wf-json-code-compact { max-height: 155px; border: 1px solid #f0c5c5; border-radius: 5px; background: #241617; color: #f6dddd; padding: 8px; }
        .wf-json-modal { z-index: 1070; }
        .wf-json-modal.wf-modal-fallback { display: block; opacity: 1; background: rgba(11, 20, 32, .55); overflow: auto; }
        .wf-json-modal .modal-dialog { width: 92%; max-width: 1180px; }
        .wf-json-modal .modal-content { border-radius: 6px; overflow: hidden; }
        .wf-json-modal .modal-header, .wf-json-modal .modal-footer { background: #f8fafc; }
        .wf-json-modal .modal-body { max-height: calc(100vh - 175px); overflow: auto; padding: 0; background: #101820; }
        .wf-json-modal pre.wf-json-full { min-height: 420px; padding: 14px 16px; color: #dce7f0; background: #101820; border: 0; border-radius: 0; box-shadow: none; }
        .wf-json-title { display: flex; align-items: center; gap: 8px; min-width: 0; }
        .wf-json-title strong { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
        .wf-json-title .close { margin-left: auto; }
        .wf-json-key { color: #78d6ff !important; }
        .wf-json-string { color: #8fdc8f !important; }
        .wf-json-number { color: #f3bf61 !important; }
        .wf-json-bool { color: #c6a8ff !important; }
        .wf-json-null { color: #9aa6b2 !important; }
        .wf-noid { font-family: Consolas, Monaco, monospace; font-size: 11px; color: #fff; background: #3d7fc2; padding: 1px 5px; border-radius: 3px; margin-right: 6px; }
        .wf-list-no.wf-t-condicao { border-left: 4px solid #d59b2f; }
        .wf-list-no.wf-t-definir { border-left: 4px solid #2d9aa6; }
        .wf-list-no.wf-t-ia { border-left: 4px solid #7b61a8; }
        .wf-list-no.wf-t-merge { border-left: 4px solid #587f92; }
        .wf-list-no.wf-t-aprovacao { border-left: 4px solid #b7722d; }
        .wf-list-no.wf-t-loop { border-left: 4px solid #3d8f77; }
        .wf-list-no.wf-t-ferramenta { border-left: 4px solid #3d7fc2; }
        .wf-ia-input-row { display: grid; grid-template-columns: 100px 95px 1fr 30px; gap: 4px; align-items: center; margin-bottom: 5px; }
        .wf-input-popup { position: absolute; z-index: 12; width: 450px; max-width: calc(100% - 24px); display: none; border: 1px solid #b8c9d9; border-radius: 6px; background: #fff; box-shadow: 0 12px 28px rgba(31, 49, 68, .24); padding: 10px; }
        .wf-input-popup.wf-runtime-input-popup { width: 560px; border-color: #8fb3d8; box-shadow: 0 16px 34px rgba(31, 49, 68, .26); }
        .wf-input-popup:before { content: ""; position: absolute; left: 28px; top: -8px; width: 14px; height: 14px; background: #fff; border-left: 1px solid #b8c9d9; border-top: 1px solid #b8c9d9; transform: rotate(45deg); }
        .wf-input-popup.wf-input-popup-above:before { top: auto; bottom: -8px; border-left: 0; border-top: 0; border-right: 1px solid #b8c9d9; border-bottom: 1px solid #b8c9d9; }
        .wf-input-popup-head { display: flex; align-items: center; gap: 8px; margin-bottom: 8px; }
        .wf-input-popup-head strong { color: #2f4050; }
        .wf-input-popup-head .btn { margin-left: auto; }
        .wf-input-popup-body { max-height: 340px; overflow: auto; }
        .wf-runtime-input-popup .wf-input-popup-body { max-height: min(430px, calc(100vh - 250px)); }
        .wf-runtime-group { margin-bottom: 10px; }
        .wf-input-popup .table { margin-bottom: 0; }
        .wf-error-popup { position: absolute; z-index: 14; width: 390px; max-width: calc(100% - 24px); display: none; border: 1px solid #c65d5d; border-radius: 6px; background: #fff; box-shadow: 0 14px 30px rgba(98, 39, 39, .22); padding: 10px; }
        .wf-error-popup:before { content: ""; position: absolute; left: 28px; top: -8px; width: 14px; height: 14px; background: #fff; border-left: 1px solid #c65d5d; border-top: 1px solid #c65d5d; transform: rotate(45deg); }
        .wf-error-popup.wf-error-popup-above:before { top: auto; bottom: -8px; border-left: 0; border-top: 0; border-right: 1px solid #c65d5d; border-bottom: 1px solid #c65d5d; }
        .wf-error-popup-head { display: flex; align-items: center; gap: 8px; color: #8f2f2f; margin-bottom: 8px; }
        .wf-error-popup-head strong { color: #8f2f2f; }
        .wf-error-popup-head .btn { margin-left: auto; }
        .wf-error-popup-body { max-height: 260px; overflow: auto; }
        .wf-error-popup-msg { white-space: pre-wrap; font-family: Consolas, Monaco, monospace; font-size: 11px; background: #fff6f6; border: 1px solid #f0c5c5; border-radius: 5px; padding: 7px; color: #5f2a2a; }
        .wf-canvas-runtime { border-top: 1px solid #e3eaf2; margin-top: 10px; padding-top: 10px; max-height: 305px; overflow: auto; overflow-x: hidden; }
        .wf-canvas-runtime-head { display: flex; align-items: center; gap: 6px; margin-bottom: 7px; }
        .wf-canvas-runtime-head .btn-group { margin-left: auto; }
        .wf-canvas-runtime .wf-test-input-row { display: block; margin-bottom: 7px; }
        .wf-canvas-runtime .wf-test-input-row label { display: block; width: auto; margin-bottom: 2px; color: #415466; }
        .wf-canvas-runtime textarea { resize: vertical; }
        @media (max-width: 1200px) {
            .wf-shell { grid-template-columns: 180px minmax(420px, 1fr); }
            .wf-props { grid-column: 1 / span 2; }
            .wf-param-editor-row { grid-template-columns: 1fr; }
            .wf-param-empty-toggle { padding-top: 0; }
            .wf-input-grid { grid-template-columns: minmax(130px, 1fr) 105px; }
        }
        .wf-editor-page { margin-top: 8px; }
        .wf-editor-header { display: flex; align-items: center; gap: 12px; padding: 11px 14px; border: 1px solid #d6e0ea; border-radius: 8px; background: #fff; box-shadow: 0 2px 8px rgba(31, 49, 68, .08); }
        .wf-editor-back { flex: 0 0 auto; }
        .wf-editor-title { min-width: 0; flex: 1; display: flex; align-items: center; gap: 9px; flex-wrap: wrap; }
        .wf-editor-title h1 { margin: 0; font-size: 22px; line-height: 1.2; color: #253746; }
        .wf-editor-subtitle { color: #607487; font-size: 13px; }
        .wf-editor-meta { display: flex; align-items: center; gap: 6px; color: #657789; font-size: 12px; white-space: nowrap; }
        .wf-editor-actions { flex: 0 0 auto; display: flex; align-items: center; gap: 6px; flex-wrap: wrap; justify-content: flex-end; }
        .wf-editor-actions .dropdown-menu { right: 0; left: auto; }
        .wf-editor-status { border: 1px solid #cbd8e4; border-radius: 12px; padding: 2px 8px; background: #f7fafc; color: #52687c; font-size: 12px; }
        .wf-editor-status.wf-status-active { border-color: #9ed2a8; background: #edf9f0; color: #2f7040; }
        .wf-editor-status.wf-status-inactive { border-color: #d6dee8; background: #f3f6f9; color: #6b7c8d; }
        .wf-legacy-tabs { margin-top: 10px; }
        .wf-legacy-tab { display: none !important; }
        .wf-tabs-compact > li > a { padding: 8px 12px; }
        .wf-shell { grid-template-columns: 238px minmax(620px, 1fr) 360px; gap: 12px; min-height: calc(100vh - 285px); }
        .wf-shell.wf-inspector-collapsed { grid-template-columns: 238px minmax(620px, 1fr); }
        .wf-palette { max-height: calc(100vh - 258px); padding: 0; overflow: hidden; display: flex; flex-direction: column; }
        .wf-panel-head { display: flex; align-items: center; gap: 8px; padding: 10px 11px; border-bottom: 1px solid #e3eaf2; background: #fbfcfd; }
        .wf-panel-head strong { color: #253746; }
        .wf-panel-head .btn { margin-left: auto; }
        .wf-panel-body { min-height: 0; overflow: auto; padding: 10px; }
        .wf-node-search { margin-bottom: 10px; }
        .wf-node-category-title { margin: 12px 0 6px; color: #657789; font-size: 11px; font-weight: bold; text-transform: uppercase; }
        .wf-node-category-title:first-of-type { margin-top: 0; }
        .wf-palette-empty { display: none; padding: 8px; border: 1px dashed #cbd8e4; border-radius: 5px; color: #657789; background: #fbfcfd; }
        .wf-shell.wf-library-collapsed { grid-template-columns: 44px minmax(620px, 1fr) 360px; }
        .wf-shell.wf-library-collapsed.wf-inspector-collapsed { grid-template-columns: 44px minmax(620px, 1fr); }
        .wf-shell.wf-library-collapsed .wf-palette .wf-panel-body,
        .wf-shell.wf-library-collapsed .wf-palette .wf-panel-head strong { display: none; }
        .wf-shell.wf-library-collapsed .wf-palette { overflow: visible; }
        .wf-shell.wf-library-collapsed .wf-panel-head { justify-content: center; padding: 10px 6px; }
        .wf-shell.wf-library-collapsed .wf-panel-head .btn { margin-left: 0; }
        .wf-canvas-wrap { min-height: calc(100vh - 258px); display: flex; flex-direction: column; overflow: hidden; }
        .wf-canvas-toolbar { position: absolute; z-index: 8; top: 10px; left: 10px; right: 10px; border: 1px solid #d6e0ea; border-radius: 7px; background: rgba(255, 255, 255, .95); box-shadow: 0 6px 18px rgba(31, 49, 68, .12); }
        .wf-canvas-toolbar .text-muted { font-size: 12px; }
        .wf-canvas-exec-status { position: absolute; z-index: 7; left: 10px; right: 10px; bottom: 10px; min-height: 0; border: 1px solid #d6e0ea; border-radius: 7px; background: rgba(255, 255, 255, .94); box-shadow: 0 6px 18px rgba(31, 49, 68, .10); }
        .wf-canvas { flex: 1; height: auto; min-height: 640px; }
        .wf-canvas-stats { padding: 2px 7px; border-radius: 12px; background: #f3f6f9; color: #52687c; font-size: 12px; }
        .wf-props { max-height: calc(100vh - 258px); padding: 0; overflow: hidden; display: flex; flex-direction: column; }
        .wf-props.wf-inspector-closed { display: none; }
        .wf-prop-scroll { max-height: none; min-height: 0; flex: 1; overflow: auto; padding: 10px; }
        .wf-bottom-panel { margin-top: 12px; border: 1px solid #d6e0ea; border-radius: 8px; background: #fff; box-shadow: 0 2px 8px rgba(31, 49, 68, .07); overflow: hidden; }
        .wf-bottom-panel.wf-bottom-collapsed .wf-bottom-body { display: none; }
        .wf-bottom-head { display: flex; align-items: center; gap: 8px; padding: 8px 10px; border-bottom: 1px solid #e3eaf2; background: #fbfcfd; }
        .wf-bottom-head strong { color: #253746; }
        .wf-bottom-tabs { display: flex; align-items: center; gap: 4px; margin-left: 12px; flex-wrap: wrap; }
        .wf-bottom-tabs .btn.active { background: #e8f3ff; border-color: #2f80d0; color: #225f9d; }
        .wf-bottom-head .wf-spacer { flex: 1; }
        .wf-bottom-body { max-height: 330px; overflow: auto; padding: 12px; }
        .wf-bottom-pane { display: none; }
        .wf-bottom-pane.active { display: block; }
        .wf-bottom-exec-actions { display: flex; gap: 6px; align-items: center; flex-wrap: wrap; margin-bottom: 8px; }
        .wf-exec-escolhas { display: grid; gap: 6px; margin: 10px 0; max-width: 760px; }
        .wf-exec-escolha { display: flex; align-items: flex-start; gap: 9px; margin: 0; padding: 9px 10px; border: 1px solid #d9c88a; border-radius: 5px; background: #fffdf5; cursor: pointer; font-weight: normal; }
        .wf-exec-escolha:hover { border-color: #b7952e; background: #fff9df; }
        .wf-exec-escolha input { margin-top: 3px; flex: 0 0 auto; }
        .wf-exec-escolha span { display: block; min-width: 0; }
        .wf-exec-escolha strong, .wf-exec-escolha small { display: block; overflow-wrap: anywhere; }
        .wf-exec-escolha small { margin-top: 2px; color: #66727c; }
        .wf-canvas-runtime { border-top: 0; margin-top: 0; padding-top: 0; max-height: none; overflow: visible; }
        .wf-canvas-runtime-head { margin-bottom: 9px; }
        .wf-config-modal .modal-dialog { width: 94%; max-width: 1360px; }
        .wf-config-modal.wf-modal-fallback { display: block; opacity: 1; background: rgba(18, 31, 44, .38); overflow: auto; }
        .wf-config-modal .modal-body { max-height: calc(100vh - 190px); overflow: auto; background: #f6f8fb; }
        .wf-config-modal .panel { margin-bottom: 0; }
        .wf-config-flow-top { margin-bottom: 12px; }
        .wf-config-flow-body { display: grid; grid-template-columns: minmax(220px, 1fr) minmax(260px, 1.2fr) minmax(260px, 1.2fr) 150px; gap: 10px 14px; align-items: start; }
        .wf-config-flow-body .form-group { margin-bottom: 0; }
        .wf-config-field-desc { grid-column: span 2; }
        .wf-config-field-permission { grid-column: span 3; }
        .wf-shortcuts-modal .modal-dialog { width: 92%; max-width: 680px; }
        .wf-shortcuts-modal.wf-modal-fallback { display: block; opacity: 1; background: rgba(18, 31, 44, .38); overflow: auto; }
        .wf-shortcut-list { display: grid; grid-template-columns: 190px minmax(0, 1fr); gap: 7px 12px; }
        .wf-shortcut-key { display: inline-block; padding: 4px 7px; border: 1px solid #cbd8e4; border-radius: 4px; background: #f7fafc; color: #253746; font-family: Consolas, Monaco, monospace; font-size: 12px; }
        .wf-shortcut-desc { color: #415466; padding-top: 4px; }
        .wf-hidden-actions { position: absolute; left: -9999px; width: 1px; height: 1px; overflow: hidden; }
        @media (max-width: 1500px) {
            .wf-shell { grid-template-columns: 220px minmax(520px, 1fr) 330px; }
            .wf-shell.wf-inspector-collapsed { grid-template-columns: 220px minmax(520px, 1fr); }
        }
        @media (max-width: 1180px) {
            .wf-editor-header { align-items: flex-start; flex-wrap: wrap; }
            .wf-editor-actions { width: 100%; justify-content: flex-start; }
            .wf-shell, .wf-shell.wf-library-collapsed { grid-template-columns: minmax(0, 1fr); }
            .wf-palette, .wf-props { max-height: none; }
            .wf-shell.wf-library-collapsed .wf-palette .wf-panel-body,
            .wf-shell.wf-library-collapsed .wf-palette .wf-panel-head strong { display: block; }
            .wf-props.wf-inspector-closed { display: none; }
            .wf-canvas-toolbar { position: relative; top: auto; left: auto; right: auto; border-left: 0; border-right: 0; border-top: 0; border-radius: 0; box-shadow: none; }
            .wf-canvas-exec-status { position: relative; left: auto; right: auto; bottom: auto; border-left: 0; border-right: 0; border-radius: 0; box-shadow: none; }
            .wf-canvas { min-height: 560px; }
            .wf-config-flow-body { grid-template-columns: 1fr 1fr; }
            .wf-config-field-desc, .wf-config-field-permission { grid-column: span 2; }
        }
        @media (max-width: 760px) {
            .wf-editor-title h1 { font-size: 18px; }
            .wf-editor-actions .btn { padding-left: 8px; padding-right: 8px; }
            .wf-bottom-tabs { margin-left: 0; width: 100%; }
            .wf-bottom-body { max-height: 420px; }
            .wf-input-row { grid-template-columns: 28px minmax(0, 1fr); }
            .wf-input-grid { grid-template-columns: 1fr; }
            .wf-input-remove { grid-column: 2; }
            .wf-config-flow-body { grid-template-columns: 1fr; }
            .wf-config-field-desc, .wf-config-field-permission { grid-column: span 1; }
            .wf-test-input-row { display: block; }
            .wf-test-input-row label { width: auto; margin-bottom: 3px; }
            .wf-produtos-head { display: none; }
            .wf-produto-row { grid-template-columns: 1fr 78px; }
            .wf-produto-nome, .wf-produto-tabela { grid-column: 1 / -1; }
            .wf-produto-tipo, .wf-produto-desconto { grid-column: auto; }
            .wf-shortcut-list { grid-template-columns: 1fr; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <div class="form-stacked row wf-editor-page">
        <asp:HiddenField ID="hddId" runat="server" />
        <asp:HiddenField ID="hddGrafo" runat="server" />

        <div class="col-lg-12">
            <div class="wf-editor-header">
                <a href="Workflows.aspx" class="btn btn-default wf-editor-back"><i class="fa fa-arrow-left"></i> Voltar</a>
                <div class="wf-editor-title">
                    <h1><asp:Label ID="lblTituloPagina" runat="server" Text="Workflow"></asp:Label></h1>
                    <span class="wf-editor-subtitle">Automação IA</span>
                    <span id="wfHeaderAtivoBadge" class="wf-editor-status">Carregando</span>
                </div>
                <div class="wf-editor-meta">
                    <span id="wfHeaderResumo">0 nós</span>
                </div>
                <div class="wf-editor-actions">
                    <button type="button" class="btn btn-default" onclick="wfOpenConfigModal()"><i class="fa fa-cog"></i> Configurações</button>
                    <button type="button" class="btn btn-success" onclick="wfHeaderSalvar()"><i class="fa fa-save"></i> Salvar</button>
                    <button type="button" class="btn btn-info" onclick="wfTestar()"><i class="fa fa-play"></i> Testar</button>
                    <button type="button" class="btn btn-primary" onclick="wfExecutarReal()"><i class="fa fa-bolt"></i> Executar</button>
                    <div class="btn-group">
                        <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            <i class="fa fa-ellipsis-v"></i>
                        </button>
                        <ul class="dropdown-menu">
                            <li><a href="javascript:void(0)" onclick="wfBottomTab('entradas')"><i class="fa fa-sign-in"></i> Entradas de execução</a></li>
                            <li><a href="javascript:void(0)" onclick="wfBottomTab('trace')"><i class="fa fa-list-alt"></i> Trace</a></li>
                            <li><a href="javascript:void(0)" onclick="wfValidarAgora()"><i class="fa fa-check-circle"></i> Validar workflow</a></li>
                            <li role="separator" class="divider"></li>
                            <li><a href="javascript:void(0)" onclick="wfOpenListaAvancada()"><i class="fa fa-list"></i> Lista avançada</a></li>
                            <li><a href="javascript:void(0)" onclick="wfAbrirJsonGrafo()"><i class="fa fa-code"></i> Ver grafo JSON</a></li>
                            <li role="separator" class="divider"></li>
                            <li><a href="javascript:void(0)" onclick="wfExportarWorkflow()"><i class="fa fa-download"></i> Exportar workflow</a></li>
                        </ul>
                    </div>
                </div>
            </div>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Workflow" />
        </div>
        <div class="col-lg-12">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>

        <div class="col-lg-12">
            <ul class="nav nav-tabs wf-tabs-compact wf-legacy-tabs" id="wfTabs">
                <li class="active"><a href="#abaCanvas" data-toggle="tab"><i class="fa fa-object-group"></i> Canvas</a></li>
                <li><a href="#abaLista" data-toggle="tab"><i class="fa fa-list"></i> Lista avançada</a></li>
                <li class="wf-legacy-tab"><a href="#abaExecucao" data-toggle="tab"><i class="fa fa-play-circle-o"></i> Teste/Execução</a></li>
            </ul>

            <div class="tab-content wf-tab-content">
                <div class="tab-pane active" id="abaCanvas">
                    <div class="wf-shell" id="wfEditorShell">
                        <div class="wf-palette" id="wfNodeLibrary">
                            <div class="wf-panel-head">
                                <strong><i class="fa fa-cubes"></i> Nós</strong>
                                <button type="button" class="btn btn-default btn-xs" onclick="wfToggleNodeLibrary()" title="Recolher biblioteca"><i class="fa fa-angle-left"></i></button>
                            </div>
                            <div class="wf-panel-body">
                                <div class="input-group input-group-sm wf-node-search">
                                    <span class="input-group-addon"><i class="fa fa-search"></i></span>
                                    <input type="text" id="wfNodeSearch" class="form-control" placeholder="Buscar nó" oninput="wfFiltrarNos(this.value)" />
                                </div>
                                <div class="wf-node-category" data-wf-category-block>
                                    <div class="wf-node-category-title">Fluxo</div>
                                    <button type="button" class="wf-palette-item" draggable="true" data-wf-tipo="inicio" data-wf-label="inicio começo start" onclick="wfCanvasAddNo('inicio')"><i class="fa fa-play"></i> Início</button>
                                    <button type="button" class="wf-palette-item" draggable="true" data-wf-tipo="fim" data-wf-label="fim encerrar finalizar" onclick="wfCanvasAddNo('fim')"><i class="fa fa-flag-checkered"></i> Fim</button>
                                </div>
                                <div class="wf-node-category" data-wf-category-block>
                                    <div class="wf-node-category-title">Ações</div>
                                    <button type="button" class="wf-palette-item" draggable="true" data-wf-tipo="ferramenta" data-wf-label="ferramenta tool ação" onclick="wfCanvasAddNo('ferramenta')"><i class="fa fa-plug"></i> Ferramenta</button>
                                    <button type="button" class="wf-palette-item" draggable="true" data-wf-tipo="ia" data-wf-label="ia inteligência prompt normalizar" onclick="wfCanvasAddNo('ia')"><i class="fa fa-magic"></i> IA</button>
                                    <button type="button" class="wf-palette-item" draggable="true" data-wf-tipo="definir" data-wf-label="definir valor variavel set" onclick="wfCanvasAddNo('definir')"><i class="fa fa-pencil-square-o"></i> Definir valor</button>
                                </div>
                                <div class="wf-node-category" data-wf-category-block>
                                    <div class="wf-node-category-title">Controle</div>
                                    <button type="button" class="wf-palette-item" draggable="true" data-wf-tipo="condicao" data-wf-label="condição se verdadeiro falso branch" onclick="wfCanvasAddNo('condicao')"><i class="fa fa-code-fork"></i> Condição</button>
                                    <button type="button" class="wf-palette-item" draggable="true" data-wf-tipo="merge" data-wf-label="merge juntar ramos unir" onclick="wfCanvasAddNo('merge')"><i class="fa fa-compress"></i> Merge</button>
                                    <button type="button" class="wf-palette-item" draggable="true" data-wf-tipo="aprovacao" data-wf-label="aprovação aprovar confirmar humano" onclick="wfCanvasAddNo('aprovacao')"><i class="fa fa-check-square-o"></i> Aprovação</button>
                                    <button type="button" class="wf-palette-item" draggable="true" data-wf-tipo="loop" data-wf-label="loop repetir laço" onclick="wfCanvasAddNo('loop')"><i class="fa fa-repeat"></i> Loop</button>
                                    <button type="button" class="wf-palette-item" draggable="true" data-wf-tipo="espera" data-wf-label="espera aguardar pausar data evento" onclick="wfCanvasAddNo('espera')"><i class="fa fa-clock-o"></i> Espera</button>
                                </div>
                                <div id="wfPaletteEmpty" class="wf-palette-empty">Nenhum nó encontrado.</div>
                            </div>
                        </div>

                        <div class="wf-canvas-wrap">
                            <div class="wf-canvas-toolbar">
                                <button type="button" class="btn btn-default btn-xs" onclick="wfCanvasZoomOut()" title="Diminuir zoom"><i class="fa fa-search-minus"></i></button>
                                <button type="button" class="btn btn-default btn-xs" onclick="wfCanvasZoomReset()" title="Zoom padrão"><i class="fa fa-dot-circle-o"></i></button>
                                <button type="button" class="btn btn-default btn-xs" onclick="wfCanvasZoomIn()" title="Aumentar zoom"><i class="fa fa-search-plus"></i></button>
                                <button type="button" id="wfBtnModoMoverCanvas" class="btn btn-default btn-xs" onclick="wfCanvasModoMover()" title="Mover pelo Canvas"><i class="fa fa-hand-paper-o"></i></button>
                                <button type="button" class="btn btn-default btn-xs" onclick="wfCanvasCentralizar()" title="Centralizar"><i class="fa fa-crosshairs"></i></button>
                                <button type="button" class="btn btn-default btn-xs" onclick="wfCanvasOrganizar()" title="Organizar automaticamente"><i class="fa fa-sitemap"></i></button>
                                <button type="button" class="btn btn-default btn-xs" onclick="wfCanvasRecolherTodos()" title="Recolher nós"><i class="fa fa-compress"></i></button>
                                <button type="button" class="btn btn-default btn-xs" onclick="wfCanvasExpandirTodos()" title="Expandir nós"><i class="fa fa-expand"></i></button>
                                <button type="button" class="btn btn-default btn-xs" onclick="wfToggleInspector()" title="Abrir/fechar inspetor"><i class="fa fa-sliders"></i></button>
                                <button type="button" class="btn btn-default btn-xs" onclick="wfOpenShortcutsModal()" title="Ver atalhos"><i class="fa fa-keyboard-o"></i> Atalhos</button>
                                <span id="wfCanvasStats" class="wf-canvas-stats">0 nós</span>
                                <span class="wf-spacer"></span>
                                <button type="button" class="btn btn-info btn-xs" onclick="wfValidarAgora()" title="Validar workflow"><i class="fa fa-check-circle"></i> Validar</button>
                                <button type="button" class="btn btn-danger btn-xs" onclick="wfCanvasRemoverSelecionado()" title="Remover nó selecionado"><i class="fa fa-trash"></i></button>
                            </div>
                            <div id="wfCanvasExecStatus" class="wf-canvas-exec-status">
                                <span class="text-muted">Pronto para testar ou executar o workflow.</span>
                            </div>
                            <div id="wfCanvas" class="wf-canvas"></div>
                            <div id="wfInputPopup" class="wf-input-popup"></div>
                            <div id="wfErroPopup" class="wf-error-popup"></div>
                        </div>

                        <div class="wf-props wf-inspector-closed" id="wfInspectorPanel">
                            <div class="wf-panel-head">
                                <strong><i class="fa fa-sliders"></i> Inspetor</strong>
                                <button type="button" class="btn btn-default btn-xs" onclick="wfCloseInspector()" title="Fechar inspetor"><i class="fa fa-times"></i></button>
                            </div>
                            <div id="wfProps" class="wf-prop-scroll">
                                <p class="text-muted">Selecione um nó no Canvas.</p>
                            </div>
                        </div>
                    </div>

                    <div class="wf-bottom-panel" id="wfBottomPanel">
                        <div class="wf-bottom-head">
                            <strong><i class="fa fa-columns"></i> Painel</strong>
                            <div class="wf-bottom-tabs">
                                <button type="button" class="btn btn-default btn-xs active" data-wf-bottom-tab="entradas" onclick="wfBottomTab('entradas')"><i class="fa fa-sign-in"></i> Entradas</button>
                                <button type="button" class="btn btn-default btn-xs" data-wf-bottom-tab="execucao" onclick="wfBottomTab('execucao')"><i class="fa fa-play-circle-o"></i> Execução</button>
                                <button type="button" class="btn btn-default btn-xs" data-wf-bottom-tab="trace" onclick="wfBottomTab('trace')"><i class="fa fa-list-alt"></i> Trace</button>
                                <button type="button" class="btn btn-default btn-xs" data-wf-bottom-tab="problemas" onclick="wfBottomTab('problemas')"><i class="fa fa-exclamation-circle"></i> Problemas</button>
                            </div>
                            <span class="wf-spacer"></span>
                            <button type="button" class="btn btn-default btn-xs" onclick="wfToggleBottomPanel()" title="Recolher painel"><i class="fa fa-angle-down"></i></button>
                        </div>
                        <div class="wf-bottom-body">
                            <div class="wf-bottom-pane active" data-wf-bottom-pane="entradas">
                                <div class="wf-canvas-runtime">
                                    <div class="wf-canvas-runtime-head">
                                        <strong>Entradas da execução</strong>
                                        <div class="btn-group btn-group-xs">
                                            <button type="button" class="btn btn-default active" data-wf-modo="inputs" onclick="wfSetTesteModo('inputs')">Campos</button>
                                            <button type="button" class="btn btn-default" data-wf-modo="json" onclick="wfSetTesteModo('json')">JSON</button>
                                        </div>
                                    </div>
                                    <div id="wfCanvasTestInputsCampos"></div>
                                    <div id="wfCanvasTestJsonBox" style="display:none;">
                                        <a href="javascript:void(0)" class="small" onclick="wfPreencherTeste()">Preencher com as entradas</a>
                                        <textarea id="wfCanvasTestInputs" class="form-control wf-mono" rows="5" style="margin-top:4px;" oninput="wfTesteJsonChange(this)">{}</textarea>
                                    </div>
                                </div>
                            </div>
                            <div class="wf-bottom-pane" data-wf-bottom-pane="execucao">
                                <div class="wf-bottom-exec-actions">
                                    <button type="button" class="btn btn-info btn-sm" id="wfBtnTestar" onclick="wfTestar()"><i class="fa fa-play"></i> Testar fluxo</button>
                                    <button type="button" class="btn btn-primary btn-sm" id="wfBtnExecutarReal" onclick="wfExecutarReal()"><i class="fa fa-play"></i> Executar workflow</button>
                                </div>
                                <div id="wfExecRealAprovacao" class="alert alert-warning" style="display:none;margin-top:8px;">
                                    <p><strong id="wfExecRealTitulo">Confirmação pendente</strong></p>
                                    <p><span id="wfExecRealRotulo">Ferramenta:</span> <strong id="wfExecRealFerramenta"></strong></p>
                                    <p class="text-muted" id="wfExecRealDescricao" style="display:none;margin-top:-6px;"></p>
                                    <div id="wfExecRealEscolhas" class="wf-exec-escolhas" style="display:none;"></div>
                                    <button type="button" class="btn btn-warning btn-sm" id="wfBtnConfirmarReal" onclick="wfConfirmarReal()">
                                        <i class="fa fa-check"></i> Confirmar passo
                                    </button>
                                    <button type="button" class="btn btn-default btn-sm" id="wfBtnRejeitarReal" style="display:none;" onclick="wfRejeitarReal()">
                                        <i class="fa fa-times"></i> Rejeitar
                                    </button>
                                    <button type="button" class="btn btn-default btn-sm" id="wfBtnCancelarReal" onclick="wfCancelarReal()">
                                        <i class="fa fa-ban"></i> Cancelar execução
                                    </button>
                                </div>
                                <div class="wf-hidden-actions">
                                    <div id="wfTestInputsCampos"></div>
                                    <div id="wfTestJsonBox" style="display:none;">
                                        <textarea id="wfTestInputs" class="form-control wf-mono" rows="3" oninput="wfTesteJsonChange(this)">{}</textarea>
                                    </div>
                                </div>
                                <p class="text-muted">Use esta área para iniciar teste, execução real e responder pausas de confirmação.</p>
                            </div>
                            <div class="wf-bottom-pane" data-wf-bottom-pane="trace">
                                <div id="wfTrace"></div>
                            </div>
                            <div class="wf-bottom-pane" data-wf-bottom-pane="problemas">
                                <div id="wfCanvasAvisos"></div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="tab-pane" id="abaLista">
                    <div class="panel panel-primary">
                        <div class="panel-heading"><h3 class="panel-title"><i class="fa fa-list"></i> Lista avançada dos nós</h3></div>
                        <div class="panel-body">
                            <p class="text-muted">Use esta aba para conferir a estrutura e selecionar nós rapidamente. Fluxos com ramificações devem ser conectados pelo Canvas.</p>
                            <div id="wfListaNos"></div>
                            <button type="button" class="btn btn-success btn-sm" onclick="wfAddPasso('ferramenta')"><i class="fa fa-plus"></i> Ferramenta</button>
                            <button type="button" class="btn btn-warning btn-sm" onclick="wfAddPasso('condicao')"><i class="fa fa-code-fork"></i> Condição</button>
                            <button type="button" class="btn btn-default btn-sm" onclick="wfAddPasso('definir')"><i class="fa fa-pencil-square-o"></i> Definir valor</button>
                            <button type="button" class="btn btn-info btn-sm" onclick="wfAddPasso('ia')"><i class="fa fa-magic"></i> IA</button>
                            <button type="button" class="btn btn-default btn-sm" onclick="wfAddPasso('merge')"><i class="fa fa-compress"></i> Merge</button>
                            <button type="button" class="btn btn-warning btn-sm" onclick="wfAddPasso('aprovacao')"><i class="fa fa-check-square-o"></i> Aprovação</button>
                            <button type="button" class="btn btn-default btn-sm" onclick="wfAddPasso('loop')"><i class="fa fa-repeat"></i> Loop</button>
                            <button type="button" class="btn btn-default btn-sm" onclick="wfAddPasso('espera')"><i class="fa fa-clock-o"></i> Espera</button>
                        </div>
                    </div>
                </div>

                <div class="tab-pane" id="abaExecucao">
                    <div class="alert alert-info">Teste, execução, trace e problemas agora ficam no painel inferior do Canvas.</div>
                </div>
            </div>
        </div>

        <fieldset class="col-lg-12 form-stacked actions wf-hidden-actions">
            <asp:Button ID="cmdSalvar" CssClass="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" OnClientClick="return wfSalvar();" />
            <a href="Workflows.aspx" class="btn btn-lg btn-default">Voltar</a>
        </fieldset>
    </div>

    <div class="modal fade wf-config-modal" id="wfConfigModal" tabindex="-1" role="dialog" aria-labelledby="wfConfigModalTitulo">
        <div class="modal-dialog modal-grande" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-title wf-json-title">
                        <strong id="wfConfigModalTitulo"><i class="fa fa-cog"></i> Configurações do workflow</strong>
                        <button type="button" class="close" data-dismiss="modal" onclick="wfCloseConfigModal()"><span aria-hidden="true">&times;</span></button>
                    </div>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-lg-12 wf-config-flow-top">
                            <div class="panel panel-primary">
                                <div class="panel-heading"><h3 class="panel-title"><i class="fa fa-cog"></i> Dados do fluxo</h3></div>
                                <div class="panel-body wf-config-flow-body">
                                    <div class="form-group wf-config-field-name">
                                        <label>Nome interno</label>
                                        <asp:TextBox ID="txtNome" CssClass="form-control wf-mono" runat="server" MaxLength="64" placeholder="workflow_fechar_pedido" />
                                        <span class="help-block">Letras, números, _ e - (sem espaço/acento). Imutável depois de criado.</span>
                                    </div>
                                    <div class="form-group wf-config-field-desc">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="txtDescricao" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="3" MaxLength="4000" />
                                    </div>
                                    <div class="form-group wf-config-field-type">
                                        <label>Tipo do workflow
                                            <i class="fa fa-question-circle" data-toggle="tooltip" title="READ apenas consulta. WRITE representa ação/alteração e exige a permissão IA.ExecutarAcoes para aparecer no chat e executar."></i>
                                        </label>
                                        <asp:DropDownList ID="ddlEscopoWorkflow" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="Leitura / consulta (READ)" Value="READ" />
                                            <asp:ListItem Text="Alteração (WRITE)" Value="WRITE" />
                                        </asp:DropDownList>
                                        <span class="help-block">Mesmo em READ, passos internos WRITE continuam pausando e exigindo confirmação.</span>
                                    </div>
                                    <div class="form-group wf-config-field-permission">
                                        <label>Permissão necessária</label>
                                        <asp:DropDownList ID="ddlRecurso" CssClass="form-control" runat="server" />
                                    </div>
                                    <div class="form-group wf-config-field-active">
                                        <label>Ativo</label>
                                        <asp:DropDownList ID="ddlAtivo" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="Não" Value="N" />
                                            <asp:ListItem Text="Sim" Value="S" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-12">
                            <div class="panel panel-default">
                                <div class="panel-heading"><h3 class="panel-title"><i class="fa fa-sign-in"></i> Entradas do fluxo</h3></div>
                                <div class="panel-body">
                                    <div id="wfInputs"></div>
                                    <div class="wf-input-tools">
                                        <button type="button" class="btn btn-default btn-xs" onclick="wfAddInput()"><i class="fa fa-plus"></i> Entrada</button>
                                        <button type="button" class="btn btn-info btn-xs" onclick="wfAplicarModeloEntradasOrcamento()"><i class="fa fa-magic"></i> Modelo orçamento</button>
                                    </div>
                                    <span class="help-block wf-input-legend">Use <strong>Obrigatória</strong> para dados pedidos no início, <strong>Opcional</strong> para dados extras e <strong>Condicional</strong> para dados solicitados só quando um nó precisar.</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default btn-sm" data-dismiss="modal" onclick="wfCloseConfigModal()">Fechar</button>
                    <button type="button" class="btn btn-success btn-sm" onclick="wfHeaderSalvar()"><i class="fa fa-save"></i> Salvar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade wf-shortcuts-modal" id="wfShortcutsModal" tabindex="-1" role="dialog" aria-labelledby="wfShortcutsModalTitulo">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-title wf-json-title">
                        <strong id="wfShortcutsModalTitulo"><i class="fa fa-keyboard-o"></i> Atalhos do Canvas</strong>
                        <button type="button" class="close" data-dismiss="modal" onclick="wfCloseShortcutsModal()"><span aria-hidden="true">&times;</span></button>
                    </div>
                </div>
                <div class="modal-body">
                    <div class="wf-shortcut-list">
                        <span class="wf-shortcut-key">Ctrl + S</span>
                        <span class="wf-shortcut-desc">Salvar o workflow.</span>
                        <span class="wf-shortcut-key">Ctrl + Enter</span>
                        <span class="wf-shortcut-desc">Testar o fluxo com as entradas atuais.</span>
                        <span class="wf-shortcut-key">Ctrl + Shift + Enter</span>
                        <span class="wf-shortcut-desc">Executar o workflow salvo.</span>
                        <span class="wf-shortcut-key">Delete</span>
                        <span class="wf-shortcut-desc">Remover o nó selecionado.</span>
                        <span class="wf-shortcut-key">Esc</span>
                        <span class="wf-shortcut-desc">Fechar modais, popups e avisos abertos.</span>
                        <span class="wf-shortcut-key">/</span>
                        <span class="wf-shortcut-desc">Focar a busca da biblioteca de nós.</span>
                        <span class="wf-shortcut-key">Espaço + arrastar</span>
                        <span class="wf-shortcut-desc">Mover o Canvas na horizontal e vertical.</span>
                        <span class="wf-shortcut-key">Botão do meio + arrastar</span>
                        <span class="wf-shortcut-desc">Mover o Canvas sem ativar o modo mão.</span>
                        <span class="wf-shortcut-key">Modo mão</span>
                        <span class="wf-shortcut-desc">Quando ativo, arraste o fundo do Canvas para navegar.</span>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default btn-sm" data-dismiss="modal" onclick="wfCloseShortcutsModal()">Fechar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade wf-param-modal" id="wfParametrosModal" tabindex="-1" role="dialog" aria-labelledby="wfParametrosModalTitulo">
        <div class="modal-dialog modal-grande" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-title wf-param-modal-title">
                        <i class="fa fa-sliders"></i>
                        <strong id="wfParametrosModalTitulo">Parâmetros do nó</strong>
                        <span id="wfParametrosModalSubtitulo" class="text-muted"></span>
                        <button type="button" class="close" data-dismiss="modal" onclick="wfFecharParametrosModal()"><span aria-hidden="true">&times;</span></button>
                    </div>
                </div>
                <div class="modal-body">
                    <div id="wfParametrosModalAviso"></div>
                    <div id="wfParametrosModalBody"></div>
                </div>
                <div class="modal-footer wf-param-modal-actions">
                    <span id="wfParametrosModalStatus" class="wf-param-status"></span>
                    <button type="button" id="wfBtnAplicarPadroesParametros" class="btn btn-default btn-sm" onclick="wfAplicarPadroesParametros()">
                        <i class="fa fa-magic"></i> Aplicar padrões seguros
                    </button>
                    <button type="button" class="btn btn-primary btn-sm" data-dismiss="modal" onclick="wfFecharParametrosModal()">
                        <i class="fa fa-check"></i> Concluir
                    </button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade wf-json-modal" id="wfJsonModal" tabindex="-1" role="dialog" aria-labelledby="wfJsonModalTitulo">
        <div class="modal-dialog modal-grande" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-title wf-json-title">
                        <strong id="wfJsonModalTitulo"><i class="fa fa-code"></i> JSON</strong>
                        <button type="button" class="close" data-dismiss="modal" onclick="wfFecharJsonViewer()"><span aria-hidden="true">&times;</span></button>
                    </div>
                </div>
                <div class="modal-body">
                    <pre id="wfJsonModalConteudo" class="wf-json-full"></pre>
                </div>
                <div class="modal-footer">
                    <button type="button" id="wfJsonModalBaixar" class="btn btn-default btn-sm" style="display:none;" onclick="wfBaixarJsonViewer()"><i class="fa fa-download"></i> Baixar arquivo</button>
                    <button type="button" class="btn btn-default btn-sm" data-dismiss="modal" onclick="wfFecharJsonViewer()">Fechar</button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        window.WF_CATALOGO = <%= CatalogoJson %>;
        window.WF_IDS = {
            hddGrafo: '<%= hddGrafo.ClientID %>',
            hddId: '<%= hddId.ClientID %>'
        };
        window.WF_URLS = {
            testar: '<%= ResolveUrl("~/App/Paginas/IA/Workflow_Detalhe.aspx/ExecutarTeste") %>',
            iniciar: '<%= ResolveUrl("~/App/Paginas/IA/Workflow_Detalhe.aspx/IniciarExecucao") %>',
            confirmar: '<%= ResolveUrl("~/App/Paginas/IA/Workflow_Detalhe.aspx/ConfirmarExecucao") %>',
            selecionarOpcao: '<%= ResolveUrl("~/App/Paginas/IA/Workflow_Detalhe.aspx/SelecionarOpcaoExecucao") %>',
            informarEntradas: '<%= ResolveUrl("~/App/Paginas/IA/Workflow_Detalhe.aspx/InformarEntradasExecucao") %>',
            cancelar: '<%= ResolveUrl("~/App/Paginas/IA/Workflow_Detalhe.aspx/CancelarExecucao") %>',
            rejeitar: '<%= ResolveUrl("~/App/Paginas/IA/Workflow_Detalhe.aspx/RejeitarExecucao") %>',
            validar: '<%= ResolveUrl("~/App/Paginas/IA/Workflow_Detalhe.aspx/ValidarGrafo") %>',
            exportar: '<%= ResolveUrl("~/App/Paginas/IA/Workflow_Detalhe.aspx/ExportarWorkflow") %>'
        };
    </script>
    <script src="/App/JS/ia-workflow-canvas.js?v=20260922-zoom-centralizado" charset="utf-8"></script>
</asp:Content>
