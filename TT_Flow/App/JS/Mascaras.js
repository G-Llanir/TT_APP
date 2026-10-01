var $j = jQuery.noConflict();

(function ($j) {
    $(function () {
        // Página NCM
        // ------------------------------------------
        if ($('[id*=txtsCodigoNCM]').length > 0) {
            $('[id*=txtsCodigoNCM]').mask('9999.99.99', { placeholder: "Digite o NCM" }, { reverse: true });
        }
        if ($('[id*=cphCorpo_txtnII]').length > 0) {
            $('[id*=cphCorpo_txtnII]').mask('0009,99', { reverse: true });
        }
        if ($('[id*=cphCorpo_txtnIPI]').length > 0) {
            $('[id*=cphCorpo_txtnIPI]').mask('0009,99', { reverse: true });
        }
        if ($('[id*=cphCorpo_txtnPIS]').length > 0) {
            $('[id*=cphCorpo_txtnPIS]').mask('0009,99', { reverse: true });
        }
        if ($('[id*=cphCorpo_txtnCOFINS]').length > 0) {
            $('[id*=cphCorpo_txtnCOFINS]').mask('0009,99', { reverse: true });
        }
        if ($('[id*=cphCorpo_txtnICMS]').length > 0) {
            $('[id*=cphCorpo_txtnICMS]').mask('0009,99', { reverse: true });
        }
        if ($('[id*=cphCorpo_txtnII_Internacional]').length > 0) {
            $('[id*=cphCorpo_txtnII_Internacional]').mask('0009,99', { reverse: true });
        }
        if ($('[id*=cphCorpo_txtnIPI_Internacional]').length > 0) {
            $('[id*=cphCorpo_txtnIPI_Internacional]').mask('0009,99', { reverse: true });
        }
        if ($('[id*=cphCorpo_txtnPIS_Internacional]').length > 0) {
            $('[id*=cphCorpo_txtnPIS_Internacional]').mask('0009,99', { reverse: true });
        }
        if ($('[id*=cphCorpo_txtnCOFINS_Internacional]').length > 0) {
            $('[id*=cphCorpo_txtnCOFINS_Internacional]').mask('0009,99', { reverse: true });
        }
        if ($('[id*=cphCorpo_txtnICMS_Internacional]').length > 0) {
            $('[id*=cphCorpo_txtnICMS_Internacional]').mask('0009,99', { reverse: true });
        }
        // ------------------------------------------
        //
        // Páigna Produtos
        // ------------------------------------------
        if ($('[id*=FN_ValorUnitario]').length > 0) {
            $('[id*=FN_ValorUnitario]').mask('0000000000009,99', { reverse: true });
        }
        if ($('[id*=txtComposicao_nQuantidade]').length > 0) {
            $('[id*=txtComposicao_nQuantidade]').mask('000000,9999', { reverse: true, translation: { '.': '' } });
        }
        if ($('[id*=FT_txtnValor]').length > 0) {
            $('[id*=FT_txtnValor]').mask('000000,9999', { reverse: true, translation: { '.': '' } });
        }

        if ($('[id*=txtnAltura').length > 0) {
            $('[id*=txtnAltura]').mask('0000009999', { reverse: true, translation: { '.': '' } });
        }

        if ($('[id*=txtnComprimento').length > 0) {
            $('[id*=txtnComprimento]').mask('0000009999', { reverse: true, translation: { '.': '' } });
        }

        if ($('[id*=txtnLargura').length > 0) {
            $('[id*=txtnLargura]').mask('0000009999', { reverse: true, translation: { '.': '' } });
        }

        if ($('[id*=txtnQuantidade').length > 0) {
            $('[id*=txtnQuantidade]').mask('0.000.000.009,9999', { reverse: true, translation: { '.': '' } });
        }

        if ($('[id*=txtnVolume').length > 0) {
            $('[id*=txtnAltura], [id*=txtnComprimento], [id*=txtnLargura]').on('input', function () {
                var txtnAltura = parseFloat($('[id*=txtnAltura]').val().replace(',', '.'));
                var txtnComprimento = parseFloat($('[id*=txtnComprimento]').val().replace(',', '.'));
                var txtnLargura = parseFloat($('[id*=txtnLargura]').val().replace(',', '.'));
                var total = isNaN(txtnAltura) || isNaN(txtnComprimento) || isNaN(txtnLargura) ? 0 : txtnAltura * txtnComprimento * txtnLargura / parseFloat(1000000000);
                $('[id*=txtnVolume]').val(total.toFixed(4).replace('.', ','));
            });
        }

        // ------------------------------------------
        //
        // ?
        // ------------------------------------------

        if ($('[id*=nQuantidadeItens]').length > 0) {
            $('[id*=nQuantidadeItens]').on('input', function () {
                var $row = $(this).closest('tr');


                var precoValue = $row.find('[id*=nValorUnitario]').val();
                var QtdeValue = $(this).val();

                if (QtdeValue.includes(',')) {
                    QtdeValue = QtdeValue.replace(',', '.');
                }
                precoValue = (typeof precoValue !== 'undefined') ? precoValue : '0,00';

                // Verificar se os valores contêm vírgula para substituir
                if (precoValue.includes(',')) {
                    precoValue = precoValue.replace(',', '.');
                }
                var preco = parseFloat(precoValue);
                var Qtde = parseFloat(QtdeValue);

                var total = isNaN(preco) || isNaN(Qtde) ? 0 : preco * Qtde;



                // Apenas para debug, este item é só pra ver se tipo, ele insere 1 ou seja, todo o código ta funcionando
                // deve haverr algum conteúdo errado que está sendo inserido que está fazendo dar erro antes de chegar aqui
                $row.find('[id*=nValorTotal]').val(total.toFixed(4).replace('.', ','));


                var ntotal = 0;
                $('[id*=dtgSelecaoItensPreCotacao] [id*=nValorTotal]').each(function () {
                    var $row = $(this).closest('tr');
                    var valor = parseFloat($row.find('[id*=nValorTotal]').val().replace('.', ','));
                    if (!isNaN(valor)) {
                        ntotal += valor;
                    }
                });

                $('[id*=dtgSelecaoItensPreCotacao] [id*=txtbTotal]').val(ntotal.toFixed(4).replace('.', ','));
                $('[id*=nTotalTabelaItens]').val(ntotal.toFixed(4)).toString();

            });
        }
        if ($('[id*=txtnPesoNeto').length > 0) {
            $('[id*=txtnPesoNeto').on('input', function () {
                var currentInput = $(this).val();
                var fixedInput = currentInput.replace(/^(\d{2})(\d{3}) /, "$1.$2,").replace(/[^\d.,]/g, "");
                $(this).val(fixedInput);
            });
        }
        if ($('[id*=txtnPesoBruto').length > 0) {
            $('[id*=txtnPesoBruto').on('input', function () {
                var currentInput = $(this).val();
                var fixedInput = currentInput.replace(/^(\d{2})(\d{3}) /, "$1.$2,").replace(/[^\d.,]/g, "");
                $(this).val(fixedInput);
            });
        }
        if ($('[id*=txtnEstoqueMinimo').length > 0) {
            $('[id*=txtnEstoqueMinimo').on('input', function () {
                var currentInput = $(this).val();
                var fixedInput = currentInput.replace(/^(\d{2})(\d{3}) /, "$1.$2,").replace(/[^\d.,]/g, "");
                $(this).val(fixedInput);
            });
        }
        if ($('[id*=txtnEstoqueAtual').length > 0) {
            $('[id*=txtnEstoqueAtual').on('input', function () {
                var currentInput = $(this).val();
                var fixedInput = currentInput.replace(/^(\d{2})(\d{3}) /, "$1.$2,").replace(/[^\d.,]/g, "");
                $(this).val(fixedInput);
            });
        }
        if ($('[id*=txtnCubagem').length > 0) {
            $('[id*=txtnCubagem').on('input', function () {
                var currentInput = $(this).val();
                var fixedInput = currentInput.replace(/^(\d{2})(\d{3}) /, "$1.$2,").replace(/[^\d.,]/g, "");
                $(this).val(fixedInput);
            });
        }

        if ($('[id*=txtnUMZ_Ratio').length > 0) {
            $('[id*=txtnUMZ_Ratio').on('input', function () {
                var currentInput = $(this).val();
                var fixedInput = currentInput.replace(/^(\d{2})(\d{3}) /, "$1.$2,").replace(/[^\d.,]/g, "");
                $(this).val(fixedInput);
            });
        }
        // ------------------------------------------
        //
        // Página Pedidos
        // ------------------------------------------

        if ($('[id*=txtnExWorks]').length > 0) {
            $('[id*=txtnExWorks]').mask('999999999999999,99', { reverse: true });
        }
        if ($('[id*=txtnInlandF]').length > 0) {
            $('[id*=txtnInlandF]').mask('999999999999999,99', { reverse: true });
        }
        if ($('[id*=txtnHandling]').length > 0) {
            $('[id*=txtnHandling]').mask('999999999999999,99', { reverse: true });
        }
        if ($('[id*=txtnConsular]').length > 0) {
            $('[id*=txtnConsular]').mask('999999999999999,99', { reverse: true });
        }
        //if ($('[id*=txtnOcean_Air]').length > 0) {
        //    $('[id*=txtnOcean_Air]').mask('999999999999999,99', { reverse: true });
        //}
        //if ($('[id*=txtnInsurance]').length > 0) {
        //    $('[id*=txtnInsurance]').mask('999999999999999,99', { reverse: true });
        //}
        //if ($('[id*=txtnOther_Charges]').length > 0) {
        //    $('[id*=txtnOther_Charges]').mask('999999999999999,99', { reverse: true });
        //}
        if ($('[id*=txtnPesoBruto]').length > 0) {
            $('[id*=txtnPesoBruto]').mask('0000000000009,9999', { reverse: true });
        }
        if ($('[id*=PesoEnvio]').length > 0) {
            $('[id*=PesoEnvio]').mask('0000000000009,9999', { reverse: true });
        }
        if ($('[id*=ComprimentoEnvio]').length > 0) {
            $('[id*=ComprimentoEnvio]').mask('0009,9999', { reverse: true });
        }
        if ($('[id*=AlturaEnvio]').length > 0) {
            $('[id*=AlturaEnvio]').mask('0009,9999', { reverse: true });
        }
        if ($('[id*=LarguraEnvio]').length > 0) {
            $('[id*=LarguraEnvio]').mask('0009,9999', { reverse: true });
        };

        if ($('[id*=nVolume]').length > 0) {
            $('[id*=txtnLarguraEnvio], [id*=txtnComprimentoEnvio], [id*=txtnAlturaEnvio]').on('input', function () {
                var $row = $(this).closest('tr');
                var nLarguraEnvio = parseFloat($row.find('[id*=txtnLarguraEnvio]').val().replace(',', '.'));
                var nComprimentoEnvio = parseFloat($row.find('[id*=txtnComprimentoEnvio]').val().replace(',', '.'));
                var nAlturaEnvio = parseFloat($row.find('[id*=txtnAlturaEnvio]').val().replace(',', '.'));
                var total = isNaN(nLarguraEnvio) || isNaN(nComprimentoEnvio) || isNaN(nAlturaEnvio) ? 0 : nLarguraEnvio * nComprimentoEnvio * nAlturaEnvio;
                $row.find('[id*=nVolume]').text(total.toFixed(4).replace('.', ','));
                $('[id*=Envio_hddEditarEnvios]').val('S');
            });
        }
    });
})($j);





