namespace NebimEnfotekIrsaliye.View
{
    partial class frmTransmissionSettings
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTransmissionSettings));
            this.pnBottom = new System.Windows.Forms.Panel();
            this.btnKaydet = new DevExpress.XtraEditors.SimpleButton();
            this.pnFill = new System.Windows.Forms.Panel();
            this.tbControl = new System.Windows.Forms.TabControl();
            this.tbAyar1 = new System.Windows.Forms.TabPage();
            this.txtCompanyCode = new DevExpress.XtraEditors.TextEdit();
            this.lblCompanyCode = new System.Windows.Forms.Label();
            this.txtSuresi = new DevExpress.XtraEditors.TextEdit();
            this.txtOfficeCode = new DevExpress.XtraEditors.TextEdit();
            this.txtPostTerminalID = new DevExpress.XtraEditors.TextEdit();
            this.txtDepoKodu = new DevExpress.XtraEditors.TextEdit();
            this.lblStokServisCalismaSuresi = new System.Windows.Forms.Label();
            this.lblStokServisSuresi = new System.Windows.Forms.Label();
            this.lblOfficeCode = new System.Windows.Forms.Label();
            this.lblPostTerminal = new System.Windows.Forms.Label();
            this.lblDepoKodu = new System.Windows.Forms.Label();
            this.imgList = new System.Windows.Forms.ImageList();
            this.cmbTestIslem = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnBottom.SuspendLayout();
            this.pnFill.SuspendLayout();
            this.tbControl.SuspendLayout();
            this.tbAyar1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSuresi.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOfficeCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPostTerminalID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepoKodu.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pnBottom
            // 
            this.pnBottom.Controls.Add(this.btnKaydet);
            this.pnBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnBottom.Location = new System.Drawing.Point(0, 213);
            this.pnBottom.Name = "pnBottom";
            this.pnBottom.Size = new System.Drawing.Size(394, 30);
            this.pnBottom.TabIndex = 0;
            // 
            // btnKaydet
            // 
            this.btnKaydet.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnKaydet.Location = new System.Drawing.Point(319, 0);
            this.btnKaydet.Name = "btnKaydet";
            this.btnKaydet.Size = new System.Drawing.Size(75, 30);
            this.btnKaydet.TabIndex = 0;
            this.btnKaydet.Text = "&Kaydet";
            this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);
            // 
            // pnFill
            // 
            this.pnFill.Controls.Add(this.tbControl);
            this.pnFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnFill.Location = new System.Drawing.Point(0, 0);
            this.pnFill.Name = "pnFill";
            this.pnFill.Size = new System.Drawing.Size(394, 213);
            this.pnFill.TabIndex = 0;
            // 
            // tbControl
            // 
            this.tbControl.Controls.Add(this.tbAyar1);
            this.tbControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbControl.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tbControl.Location = new System.Drawing.Point(0, 0);
            this.tbControl.Name = "tbControl";
            this.tbControl.SelectedIndex = 0;
            this.tbControl.Size = new System.Drawing.Size(394, 213);
            this.tbControl.TabIndex = 0;
            // 
            // tbAyar1
            // 
            this.tbAyar1.Controls.Add(this.cmbTestIslem);
            this.tbAyar1.Controls.Add(this.txtCompanyCode);
            this.tbAyar1.Controls.Add(this.lblCompanyCode);
            this.tbAyar1.Controls.Add(this.txtSuresi);
            this.tbAyar1.Controls.Add(this.txtOfficeCode);
            this.tbAyar1.Controls.Add(this.txtPostTerminalID);
            this.tbAyar1.Controls.Add(this.txtDepoKodu);
            this.tbAyar1.Controls.Add(this.label1);
            this.tbAyar1.Controls.Add(this.lblStokServisCalismaSuresi);
            this.tbAyar1.Controls.Add(this.lblStokServisSuresi);
            this.tbAyar1.Controls.Add(this.lblOfficeCode);
            this.tbAyar1.Controls.Add(this.lblPostTerminal);
            this.tbAyar1.Controls.Add(this.lblDepoKodu);
            this.tbAyar1.Location = new System.Drawing.Point(4, 22);
            this.tbAyar1.Name = "tbAyar1";
            this.tbAyar1.Padding = new System.Windows.Forms.Padding(3);
            this.tbAyar1.Size = new System.Drawing.Size(386, 187);
            this.tbAyar1.TabIndex = 0;
            this.tbAyar1.Text = "Ayar 1";
            this.tbAyar1.UseVisualStyleBackColor = true;
            // 
            // txtCompanyCode
            // 
            this.txtCompanyCode.Location = new System.Drawing.Point(178, 12);
            this.txtCompanyCode.Name = "txtCompanyCode";
            this.txtCompanyCode.Size = new System.Drawing.Size(130, 20);
            this.txtCompanyCode.TabIndex = 0;
            // 
            // lblCompanyCode
            // 
            this.lblCompanyCode.AutoSize = true;
            this.lblCompanyCode.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCompanyCode.Location = new System.Drawing.Point(17, 13);
            this.lblCompanyCode.Name = "lblCompanyCode";
            this.lblCompanyCode.Size = new System.Drawing.Size(95, 15);
            this.lblCompanyCode.TabIndex = 91;
            this.lblCompanyCode.Text = "Company Kodu :";
            // 
            // txtSuresi
            // 
            this.txtSuresi.Location = new System.Drawing.Point(178, 115);
            this.txtSuresi.Name = "txtSuresi";
            this.txtSuresi.Size = new System.Drawing.Size(84, 20);
            this.txtSuresi.TabIndex = 16;
            // 
            // txtOfficeCode
            // 
            this.txtOfficeCode.Location = new System.Drawing.Point(178, 85);
            this.txtOfficeCode.Name = "txtOfficeCode";
            this.txtOfficeCode.Size = new System.Drawing.Size(130, 20);
            this.txtOfficeCode.TabIndex = 3;
            // 
            // txtPostTerminalID
            // 
            this.txtPostTerminalID.Location = new System.Drawing.Point(178, 60);
            this.txtPostTerminalID.Name = "txtPostTerminalID";
            this.txtPostTerminalID.Size = new System.Drawing.Size(130, 20);
            this.txtPostTerminalID.TabIndex = 2;
            // 
            // txtDepoKodu
            // 
            this.txtDepoKodu.Location = new System.Drawing.Point(178, 35);
            this.txtDepoKodu.Name = "txtDepoKodu";
            this.txtDepoKodu.Size = new System.Drawing.Size(130, 20);
            this.txtDepoKodu.TabIndex = 1;
            // 
            // lblStokServisCalismaSuresi
            // 
            this.lblStokServisCalismaSuresi.AutoSize = true;
            this.lblStokServisCalismaSuresi.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblStokServisCalismaSuresi.Location = new System.Drawing.Point(17, 117);
            this.lblStokServisCalismaSuresi.Name = "lblStokServisCalismaSuresi";
            this.lblStokServisCalismaSuresi.Size = new System.Drawing.Size(155, 15);
            this.lblStokServisCalismaSuresi.TabIndex = 80;
            this.lblStokServisCalismaSuresi.Text = "Stok Servis Çalışma Süresi :";
            // 
            // lblStokServisSuresi
            // 
            this.lblStokServisSuresi.AutoSize = true;
            this.lblStokServisSuresi.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblStokServisSuresi.Location = new System.Drawing.Point(268, 117);
            this.lblStokServisSuresi.Name = "lblStokServisSuresi";
            this.lblStokServisSuresi.Size = new System.Drawing.Size(45, 15);
            this.lblStokServisSuresi.TabIndex = 84;
            this.lblStokServisSuresi.Text = "Dakika";
            // 
            // lblOfficeCode
            // 
            this.lblOfficeCode.AutoSize = true;
            this.lblOfficeCode.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblOfficeCode.Location = new System.Drawing.Point(17, 89);
            this.lblOfficeCode.Name = "lblOfficeCode";
            this.lblOfficeCode.Size = new System.Drawing.Size(69, 15);
            this.lblOfficeCode.TabIndex = 89;
            this.lblOfficeCode.Text = "Ofis Kodu : ";
            // 
            // lblPostTerminal
            // 
            this.lblPostTerminal.AutoSize = true;
            this.lblPostTerminal.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblPostTerminal.Location = new System.Drawing.Point(17, 64);
            this.lblPostTerminal.Name = "lblPostTerminal";
            this.lblPostTerminal.Size = new System.Drawing.Size(101, 15);
            this.lblPostTerminal.TabIndex = 90;
            this.lblPostTerminal.Text = "POS Terminal ID :";
            // 
            // lblDepoKodu
            // 
            this.lblDepoKodu.AutoSize = true;
            this.lblDepoKodu.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblDepoKodu.Location = new System.Drawing.Point(17, 39);
            this.lblDepoKodu.Name = "lblDepoKodu";
            this.lblDepoKodu.Size = new System.Drawing.Size(75, 15);
            this.lblDepoKodu.TabIndex = 78;
            this.lblDepoKodu.Text = "Depo Kodu : ";
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "turkey.png");
            this.imgList.Images.SetKeyName(1, "unitedkingdom.png");
            this.imgList.Images.SetKeyName(2, "france.png");
            this.imgList.Images.SetKeyName(3, "germany.png");
            this.imgList.Images.SetKeyName(4, "russia.png");
            // 
            // cmbTestIslem
            // 
            this.cmbTestIslem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTestIslem.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.cmbTestIslem.FormattingEnabled = true;
            this.cmbTestIslem.Items.AddRange(new object[] {
            "Evet",
            "Hayır"});
            this.cmbTestIslem.Location = new System.Drawing.Point(178, 141);
            this.cmbTestIslem.Name = "cmbTestIslem";
            this.cmbTestIslem.Size = new System.Drawing.Size(84, 21);
            this.cmbTestIslem.TabIndex = 92;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(17, 147);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 15);
            this.label1.TabIndex = 80;
            this.label1.Text = "Test İşlem :";
            // 
            // frmTransmissionSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 243);
            this.Controls.Add(this.pnFill);
            this.Controls.Add(this.pnBottom);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTransmissionSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Aktarım Ayarları";
            this.Shown += new System.EventHandler(this.frmTransmissionSettings_Shown);
            this.pnBottom.ResumeLayout(false);
            this.pnFill.ResumeLayout(false);
            this.tbControl.ResumeLayout(false);
            this.tbAyar1.ResumeLayout(false);
            this.tbAyar1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSuresi.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOfficeCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPostTerminalID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepoKodu.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnBottom;
        private DevExpress.XtraEditors.SimpleButton btnKaydet;
        private System.Windows.Forms.Panel pnFill;
        private System.Windows.Forms.TabControl tbControl;
        private System.Windows.Forms.TabPage tbAyar1;
        private DevExpress.XtraEditors.TextEdit txtCompanyCode;
        private System.Windows.Forms.Label lblCompanyCode;
        private DevExpress.XtraEditors.TextEdit txtSuresi;
        private DevExpress.XtraEditors.TextEdit txtOfficeCode;
        private DevExpress.XtraEditors.TextEdit txtPostTerminalID;
        private DevExpress.XtraEditors.TextEdit txtDepoKodu;
        private System.Windows.Forms.Label lblStokServisCalismaSuresi;
        private System.Windows.Forms.Label lblStokServisSuresi;
        private System.Windows.Forms.Label lblOfficeCode;
        private System.Windows.Forms.Label lblPostTerminal;
        private System.Windows.Forms.Label lblDepoKodu;
        public System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.ComboBox cmbTestIslem;
        private System.Windows.Forms.Label label1;
    }
}