namespace TT_Windows
{
    partial class frmServices
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();

            base.Dispose(disposing);
        }

        #region | Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.tmrCommboxRecepcao = new System.Windows.Forms.Timer(this.components);
            this.tmrValidaEquipamentoSemEventos = new System.Windows.Forms.Timer(this.components);
            this.tmrEnviaMensagensPendentes = new System.Windows.Forms.Timer(this.components);
            this.tmrPing = new System.Windows.Forms.Timer(this.components);
            this.tmrEnviarPagamentos = new System.Windows.Forms.Timer(this.components);
            this.tmrImportarXML = new System.Windows.Forms.Timer(this.components);
            this.tmrZplPrinter = new System.Windows.Forms.Timer(this.components);
            this.tmrGerenciadorNFe = new System.Windows.Forms.Timer(this.components);
            this.tmrAtualiza_ST_LegisWeb = new System.Windows.Forms.Timer(this.components);
            this.tmrAtualiza_Impostos_LegisWeb = new System.Windows.Forms.Timer(this.components);
            this.tmrEnviaEmails_AsSete = new System.Windows.Forms.Timer(this.components);
            this.tmrAtualiza_Status_ClientePontual = new System.Windows.Forms.Timer(this.components);
            this.tmrProcessaArquivosIA = new System.Windows.Forms.Timer(this.components);
            this.tmrConhecimentoPasta = new System.Windows.Forms.Timer(this.components);
            this.cmdAtualizar = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtLogImpressora = new System.Windows.Forms.TextBox();
            this.txtOutrosLogs = new System.Windows.Forms.TextBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtLogNFe = new System.Windows.Forms.TextBox();
            this.txtLogEmail = new System.Windows.Forms.TextBox();
            this.txtlogPing = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Registro de LOGs";
            // 
            // tmrCommboxRecepcao
            // 
            this.tmrCommboxRecepcao.Tick += new System.EventHandler(this.tmrCommboxRecepcao_Tick);
            // 
            // tmrValidaEquipamentoSemEventos
            // 
            this.tmrValidaEquipamentoSemEventos.Tick += new System.EventHandler(this.tmrValidaEquipamentoSemEventos_Tick);
            // 
            // tmrEnviaMensagensPendentes
            // 
            this.tmrEnviaMensagensPendentes.Tick += new System.EventHandler(this.tmrEnviaMensagensPendentes_Tick);
            // 
            // tmrPing
            // 
            this.tmrPing.Tick += new System.EventHandler(this.tmrPing_Tick);
            // 
            // tmrEnviarPagamentos
            // 
            this.tmrEnviarPagamentos.Tick += new System.EventHandler(this.tmrEnviarPagamentos_Tick);
            // 
            // tmrImportarXML
            // 
            this.tmrImportarXML.Tick += new System.EventHandler(this.tmrImportaXML_Tick);
            // 
            // tmrZplPrinter
            // 
            this.tmrZplPrinter.Tick += new System.EventHandler(this.tmrZplPrinter_Tick);
            // 
            // tmrGerenciadorNFe
            // 
            this.tmrGerenciadorNFe.Tick += new System.EventHandler(this.tmrGerenciadorNFe_Tick);
            // 
            // tmrAtualiza_ST_LegisWeb
            // 
            this.tmrAtualiza_ST_LegisWeb.Tick += new System.EventHandler(this.tmrAtualiza_ST_LegisWeb_Tick);
            // 
            // tmrAtualiza_Impostos_LegisWeb
            // 
            this.tmrAtualiza_Impostos_LegisWeb.Tick += new System.EventHandler(this.tmrAtualiza_Impostos_LegisWeb_Tick);
            // 
            // tmrEnviaEmails_AsSete
            // 
            this.tmrEnviaEmails_AsSete.Tick += new System.EventHandler(this.tmrEnviaEmails_AsSete_Tick);
            // 
            // tmrAtualiza_Status_ClientePontual
            // 
            this.tmrAtualiza_Status_ClientePontual.Tick += new System.EventHandler(this.tmrAtualiza_Status_ClientePontual_Tick);
            // 
            // tmrProcessaArquivosIA
            // 
            this.tmrProcessaArquivosIA.Enabled = true;
            this.tmrProcessaArquivosIA.Interval = 30000;
            this.tmrProcessaArquivosIA.Tick += new System.EventHandler(this.tmrProcessaArquivosIA_Tick);
            //
            // tmrConhecimentoPasta
            //
            this.tmrConhecimentoPasta.Interval = 300000;
            this.tmrConhecimentoPasta.Tick += new System.EventHandler(this.tmrConhecimentoPasta_Tick);
            // 
            // cmdAtualizar
            // 
            this.cmdAtualizar.BackColor = System.Drawing.Color.Blue;
            this.cmdAtualizar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdAtualizar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAtualizar.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.cmdAtualizar.Location = new System.Drawing.Point(250, 3);
            this.cmdAtualizar.Name = "cmdAtualizar";
            this.cmdAtualizar.Size = new System.Drawing.Size(177, 27);
            this.cmdAtualizar.TabIndex = 13;
            this.cmdAtualizar.Text = "Atualizar Parâmetros";
            this.cmdAtualizar.UseVisualStyleBackColor = false;
            this.cmdAtualizar.Click += new System.EventHandler(this.cmdAtualizar_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.txtLogImpressora, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtOutrosLogs, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtStatus, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtLogNFe, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtLogEmail, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtlogPing, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 39);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(890, 508);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // txtLogImpressora
            // 
            this.txtLogImpressora.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogImpressora.Location = new System.Drawing.Point(3, 357);
            this.txtLogImpressora.Multiline = true;
            this.txtLogImpressora.Name = "txtLogImpressora";
            this.txtLogImpressora.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogImpressora.Size = new System.Drawing.Size(439, 148);
            this.txtLogImpressora.TabIndex = 18;
            // 
            // txtOutrosLogs
            // 
            this.txtOutrosLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutrosLogs.Location = new System.Drawing.Point(448, 357);
            this.txtOutrosLogs.Multiline = true;
            this.txtOutrosLogs.Name = "txtOutrosLogs";
            this.txtOutrosLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutrosLogs.Size = new System.Drawing.Size(439, 148);
            this.txtOutrosLogs.TabIndex = 16;
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(3, 3);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(439, 171);
            this.txtStatus.TabIndex = 9;
            // 
            // txtLogNFe
            // 
            this.txtLogNFe.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogNFe.Location = new System.Drawing.Point(3, 180);
            this.txtLogNFe.Multiline = true;
            this.txtLogNFe.Name = "txtLogNFe";
            this.txtLogNFe.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogNFe.Size = new System.Drawing.Size(439, 171);
            this.txtLogNFe.TabIndex = 15;
            // 
            // txtLogEmail
            // 
            this.txtLogEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogEmail.Location = new System.Drawing.Point(448, 3);
            this.txtLogEmail.Multiline = true;
            this.txtLogEmail.Name = "txtLogEmail";
            this.txtLogEmail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogEmail.Size = new System.Drawing.Size(439, 171);
            this.txtLogEmail.TabIndex = 13;
            // 
            // txtlogPing
            // 
            this.txtlogPing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtlogPing.Location = new System.Drawing.Point(448, 180);
            this.txtlogPing.MaxLength = 40000;
            this.txtlogPing.Multiline = true;
            this.txtlogPing.Name = "txtlogPing";
            this.txtlogPing.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtlogPing.Size = new System.Drawing.Size(439, 171);
            this.txtlogPing.TabIndex = 17;
            // 
            // frmServices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 551);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.cmdAtualizar);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "frmServices";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TT_Windows";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmServices_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer tmrCommboxRecepcao;
        private System.Windows.Forms.Timer tmrValidaEquipamentoSemEventos;
        private System.Windows.Forms.Timer tmrEnviaMensagensPendentes;
        private System.Windows.Forms.Timer tmrPing;
        private System.Windows.Forms.Timer tmrEnviarPagamentos;
		private System.Windows.Forms.Timer tmrImportarXML;
        private System.Windows.Forms.Timer tmrZplPrinter;
        private System.Windows.Forms.Timer tmrGerenciadorNFe;
        private System.Windows.Forms.Timer tmrAtualiza_ST_LegisWeb;
        private System.Windows.Forms.Timer tmrAtualiza_Impostos_LegisWeb;
        private System.Windows.Forms.Timer tmrEnviaEmails_AsSete;
        private System.Windows.Forms.Timer tmrAtualiza_Status_ClientePontual;																			 
        private System.Windows.Forms.Timer tmrProcessaArquivosIA;
        private System.Windows.Forms.Timer tmrConhecimentoPasta;
        private System.Windows.Forms.Button cmdAtualizar;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtLogEmail;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.TextBox txtlogPing;
        private System.Windows.Forms.TextBox txtOutrosLogs;
        private System.Windows.Forms.TextBox txtLogNFe;
        private System.Windows.Forms.TextBox txtLogImpressora;
    }
}