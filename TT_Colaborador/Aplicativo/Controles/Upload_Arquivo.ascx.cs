using System;
using System.Text;
using System.Web.UI;

namespace TT_Colaborador.Aplicativo.Controles
{
    public partial class Upload_Arquivo : UserControl
    {
        /// <summary>
        /// Propriedade do Controle utilizado para passar o ID do asp:FileUpload.
        /// <br />
        /// O asp:FileUpload deve ser incluso dentro da Página que está usando este Controle, caso o asp:FileUpload esteja dentro de um asp:UpdatePanel, será necessário usar asp:PostBackTrigger no botão que irá pegar ou validar o Arquivo.
        /// <br /><br />
        /// Isto é necessário porque o asp:FileUpload necessita de um PostBack completo na Página para passar o Arquivo para o Server-side
        /// </summary>
        public string ID_FileUpload { get => hddFileUpload_ID.Value; set => hddFileUpload_ID.Value = value; }
        public string FuncaoPersonalizada_Script = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function() {");
            sb.AppendLine("     var $uploadContainer = $('[id*=" + uploadContainer.ClientID + "]');");
            sb.AppendLine("     var $fileInput = $('[id*=" + ID_FileUpload + "]');");
            sb.AppendLine("     var $fileNameDisplay = $('[id*=" + fileName.ClientID + "]');");
            sb.AppendLine("");
            sb.AppendLine("     $uploadContainer.off('click').on('click', function () {");
            sb.AppendLine("          $fileInput.click();");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $fileInput.on('click', function (e) {");
            sb.AppendLine("          e.stopPropagation();");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $uploadContainer.off('dragover').on('dragover', function (e) {");
            sb.AppendLine("          e.preventDefault();");
            sb.AppendLine("          e.stopPropagation();");
            sb.AppendLine("          $uploadContainer.addClass('dragover');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $uploadContainer.off('dragleave').on('dragleave', function (e) {");
            sb.AppendLine("          e.preventDefault();");
            sb.AppendLine("          e.stopPropagation();");
            sb.AppendLine("          $uploadContainer.removeClass('dragover');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $uploadContainer.off('drop').on('drop', function (e) {");
            sb.AppendLine("          e.preventDefault();");
            sb.AppendLine("          e.stopPropagation();");
            sb.AppendLine("          $uploadContainer.removeClass('dragover');");
            sb.AppendLine("          var files = e.originalEvent.dataTransfer.files;");
            sb.AppendLine("          $fileInput[0].files = files;");
            sb.AppendLine("          displayFileName(files[0].name);");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $fileInput.off('change').on('change', function () {");
            sb.AppendLine("          if (this.files.length > 0) {");
            sb.AppendLine("              displayFileName(this.files[0].name);");
            sb.AppendLine($"            {FuncaoPersonalizada_Script};");
            sb.AppendLine("          }");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     function displayFileName(name) {");
            sb.AppendLine("          $fileNameDisplay.text(name);");
            sb.AppendLine("     }");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_UploadArquivo_" + Guid.NewGuid(), sb.ToString(), true);
        }
    }
}