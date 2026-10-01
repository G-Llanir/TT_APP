
jQuery(window).on('load', function () {
    var jQuery_1_8_24 = $.noConflict(true);
    var inputFile;
    jQuery_1_8_24(function () {


        jQuery_1_8_24('#dialog').dialog({
            autoOpen: false,
            modal: true,
            height: 400,
            width: 400,
            buttons: {
                "Mudar Imagem": function () {
                    var txtName = '[id *= img_Produto]';
                    inputFile = jQuery_1_8_24('<input type="file" id="file" name="file" ClientIDMode="Static" accept="image/*">');
                    dialog.append(inputFile);

                    const img = jQuery_1_8_24('[id *= img_Produto]');
                    const originalSrc = img.attr('src');
                    if (inputFile[0].files.length > 0) {

                    } else {
                        inputFile.click();
                        inputFile.on('change', function (e) {
                            var reader = new FileReader();
                            reader.onload = function (e) {
                                img.attr('src', e.target.result);
                                saveNewImageSrc(e.target.result);

                            }
                            reader.readAsDataURL(inputFile[0].files[0]);
                            var filename = inputFile[0].files[0].name;
                            var extension = filename.substr(filename.lastIndexOf('.') + 1);
                        });

                        jQuery_1_8_24('#dialog').dialog("close");
                    }

                },
                Cancel: function () {

                    jQuery_1_8_24('#dialog').dialog("close");
                    img.attr('src', originalSrc);

                }
            }
        });


        jQuery_1_8_24('[id*=img_Produto]').click(function () {
            jQuery_1_8_24('#dialog').html('');
            jQuery_1_8_24('#dialog').append(jQuery_1_8_24(this).clone());
            jQuery_1_8_24('#dialog').dialog('open');
        });



        function saveNewImageSrc(newSrc) {
            var imgId = jQuery_1_8_24('[id*=img_Produto]').attr('id');
            var data = { 'imgId': imgId, 'newSrc': newSrc, 'filename': filename, 'extension': extension };
            var filename = inputFile[0].files[0].name;
            var extension = filename.substr(filename.lastIndexOf('.') + 1);
            data.filename = filename;
            data.extension = extension;
            jQuery_1_8_24.ajax({
                type: "POST",
                url: "/App/Paginas/Arquivos.aspx/SaveNewImageSrc",
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    console.log(response.d);
                },
                error: function (xhr, status, error) {
                    console.log("Erro : " + error);
                }
            });
            jQuery_1_8_24("#ddlidTipoArquivo").prop('selectedIndex', 1);
            jQuery_1_8_24("#txtEnviarArquivo_sDscArquivo").val(inputFile[0].files[0].name);
        }
    });

});