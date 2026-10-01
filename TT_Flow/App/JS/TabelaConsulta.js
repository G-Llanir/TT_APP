$(document).ready(function () {
    $('#<%= gv.ClientID %>').DataTable({
        paging: true,
        pageLength: 50,
/*        order: [[<%= OrdenarColuna.ToString() %>, '<%= TipoOrdenacao.ToLower() %>']],*/
        info: false,
        language: { url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json' }
    });
});

//$(document).ready(function () {
//    // 'ordenarColuna' será o valor do controle HTML com o ID especificado
//    var ordenarColuna = Number(ordenarColuna); // Converter para um número, se necessário

//    $('#<%= gv.ClientID %>').DataTable({
//        paging: true,
//        pageLength: 50,
//        order: [[ordenarColuna, tipoOrdenacao]],
//        info: false,
//        language: { url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json' },
//    });
//});