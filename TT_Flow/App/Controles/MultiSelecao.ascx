<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultiSelecao.ascx.cs" Inherits="TT_Flow.App.Controles.MultiSelecao" %>


<script type="text/javascript">
    $(function carregarMultiselect() {
        $('[id*=lstidFluxo]').multiselect({
            buttonWidth: '195px',
            includeSelectAllOption: true,
            maxHeight: 300,
            dropRight: true,
            nSelectedText: ' - Fluxos Selecionados!',
            allSelectedText: 'Todos os Fluxos',
            enableFiltering: false,
            
        });
    });
</script>

<div class="form-group">
    <asp:ListBox ID="lstidFluxo" class="form-control" runat="server" SelectionMode="Multiple"></asp:ListBox>
</div>