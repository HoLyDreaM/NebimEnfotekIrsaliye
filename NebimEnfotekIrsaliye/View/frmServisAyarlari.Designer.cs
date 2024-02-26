namespace NebimEnfotekIrsaliye.View
{
    partial class frmServisAyarlari
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmServisAyarlari));
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.pictur = new DevExpress.XtraEditors.PictureEdit();
            this.btnServisOlustur = new DevExpress.XtraEditors.SimpleButton();
            this.btnServisKur = new DevExpress.XtraEditors.SimpleButton();
            this.btnBaslat = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.btnDurdur = new DevExpress.XtraEditors.SimpleButton();
            this.txtDisplayAdi = new DevExpress.XtraEditors.TextEdit();
            this.btnYenidenBaslat = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.btnServisSil = new DevExpress.XtraEditors.SimpleButton();
            this.txtServiceStatus = new DevExpress.XtraEditors.TextEdit();
            this.txtServiceName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictur.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDisplayAdi.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.pictur);
            this.groupControl1.Controls.Add(this.btnServisOlustur);
            this.groupControl1.Controls.Add(this.btnServisKur);
            this.groupControl1.Controls.Add(this.btnBaslat);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Controls.Add(this.btnDurdur);
            this.groupControl1.Controls.Add(this.txtDisplayAdi);
            this.groupControl1.Controls.Add(this.btnYenidenBaslat);
            this.groupControl1.Controls.Add(this.labelControl2);
            this.groupControl1.Controls.Add(this.btnServisSil);
            this.groupControl1.Controls.Add(this.txtServiceStatus);
            this.groupControl1.Controls.Add(this.txtServiceName);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Location = new System.Drawing.Point(12, 12);
            this.groupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(776, 181);
            this.groupControl1.TabIndex = 37;
            this.groupControl1.Text = "ENFOTEK - NEBİM SERVİS YÖNETİMİ";
            // 
            // pictur
            // 
            this.pictur.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictur.EditValue = global::NebimEnfotekIrsaliye.Properties.Resources.No_entry;
            this.pictur.Location = new System.Drawing.Point(446, 92);
            this.pictur.Name = "pictur";
            this.pictur.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pictur.Properties.Appearance.Options.UseBackColor = true;
            this.pictur.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pictur.Properties.ZoomAccelerationFactor = 1D;
            this.pictur.Size = new System.Drawing.Size(28, 29);
            this.pictur.TabIndex = 36;
            // 
            // btnServisOlustur
            // 
            this.btnServisOlustur.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnServisOlustur.Appearance.Options.UseFont = true;
            this.btnServisOlustur.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSiparisServisOlustur.ImageOptions.Image")));
            this.btnServisOlustur.Location = new System.Drawing.Point(363, 30);
            this.btnServisOlustur.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnServisOlustur.Name = "btnServisOlustur";
            this.btnServisOlustur.Size = new System.Drawing.Size(184, 36);
            this.btnServisOlustur.TabIndex = 35;
            this.btnServisOlustur.Text = "Servis Oluştur";
            this.btnServisOlustur.Click += new System.EventHandler(this.btnServisOlustur_Click);
            // 
            // btnServisKur
            // 
            this.btnServisKur.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnServisKur.Appearance.Options.UseFont = true;
            this.btnServisKur.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSiparisServisKur.ImageOptions.Image")));
            this.btnServisKur.Location = new System.Drawing.Point(17, 28);
            this.btnServisKur.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnServisKur.Name = "btnServisKur";
            this.btnServisKur.Size = new System.Drawing.Size(163, 36);
            this.btnServisKur.TabIndex = 4;
            this.btnServisKur.Text = "Servis Kur";
            this.btnServisKur.Click += new System.EventHandler(this.btnServisKur_Click);
            // 
            // btnBaslat
            // 
            this.btnBaslat.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnBaslat.Appearance.Options.UseFont = true;
            this.btnBaslat.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSiparisBaslat.ImageOptions.Image")));
            this.btnBaslat.Location = new System.Drawing.Point(17, 128);
            this.btnBaslat.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnBaslat.Name = "btnBaslat";
            this.btnBaslat.Size = new System.Drawing.Size(106, 33);
            this.btnBaslat.TabIndex = 0;
            this.btnBaslat.Text = "Başlat";
            this.btnBaslat.Click += new System.EventHandler(this.btnBaslat_Click);
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl3.Appearance.ForeColor = System.Drawing.Color.Black;
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Appearance.Options.UseForeColor = true;
            this.labelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.labelControl3.Location = new System.Drawing.Point(170, 72);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(153, 19);
            this.labelControl3.TabIndex = 31;
            this.labelControl3.Text = "Görünen Ad";
            // 
            // btnDurdur
            // 
            this.btnDurdur.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnDurdur.Appearance.Options.UseFont = true;
            this.btnDurdur.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSiparisDurdur.ImageOptions.Image")));
            this.btnDurdur.Location = new System.Drawing.Point(129, 128);
            this.btnDurdur.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnDurdur.Name = "btnDurdur";
            this.btnDurdur.Size = new System.Drawing.Size(132, 33);
            this.btnDurdur.TabIndex = 1;
            this.btnDurdur.Text = "Durdur";
            this.btnDurdur.Click += new System.EventHandler(this.btnDurdur_Click);
            // 
            // txtDisplayAdi
            // 
            this.txtDisplayAdi.EnterMoveNextControl = true;
            this.txtDisplayAdi.Location = new System.Drawing.Point(170, 91);
            this.txtDisplayAdi.Name = "txtDisplayAdi";
            this.txtDisplayAdi.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtDisplayAdi.Properties.Appearance.Options.UseFont = true;
            this.txtDisplayAdi.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.txtDisplayAdi.Properties.AppearanceFocused.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtDisplayAdi.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtDisplayAdi.Properties.AppearanceFocused.Options.UseFont = true;
            this.txtDisplayAdi.Properties.AutoHeight = false;
            this.txtDisplayAdi.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.txtDisplayAdi.Properties.ReadOnly = true;
            this.txtDisplayAdi.Size = new System.Drawing.Size(153, 31);
            this.txtDisplayAdi.TabIndex = 30;
            // 
            // btnYenidenBaslat
            // 
            this.btnYenidenBaslat.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnYenidenBaslat.Appearance.Options.UseFont = true;
            this.btnYenidenBaslat.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSiparisYenidenBaslat.ImageOptions.Image")));
            this.btnYenidenBaslat.Location = new System.Drawing.Point(267, 128);
            this.btnYenidenBaslat.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnYenidenBaslat.Name = "btnYenidenBaslat";
            this.btnYenidenBaslat.Size = new System.Drawing.Size(182, 33);
            this.btnYenidenBaslat.TabIndex = 2;
            this.btnYenidenBaslat.Text = "Yeniden Başlat";
            this.btnYenidenBaslat.Click += new System.EventHandler(this.btnYenidenBaslat_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl2.Appearance.ForeColor = System.Drawing.Color.Black;
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Appearance.Options.UseForeColor = true;
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.labelControl2.Location = new System.Drawing.Point(323, 72);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(153, 19);
            this.labelControl2.TabIndex = 29;
            this.labelControl2.Text = "Servis Durumu";
            // 
            // btnServisSil
            // 
            this.btnServisSil.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnServisSil.Appearance.Options.UseFont = true;
            this.btnServisSil.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSiparisServisSil.ImageOptions.Image")));
            this.btnServisSil.Location = new System.Drawing.Point(186, 30);
            this.btnServisSil.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnServisSil.Name = "btnServisSil";
            this.btnServisSil.Size = new System.Drawing.Size(171, 36);
            this.btnServisSil.TabIndex = 5;
            this.btnServisSil.Text = "Servis Sil";
            this.btnServisSil.Click += new System.EventHandler(this.btnServisSil_Click);
            // 
            // txtServiceStatus
            // 
            this.txtServiceStatus.EnterMoveNextControl = true;
            this.txtServiceStatus.Location = new System.Drawing.Point(323, 91);
            this.txtServiceStatus.Name = "txtServiceStatus";
            this.txtServiceStatus.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtServiceStatus.Properties.Appearance.Options.UseFont = true;
            this.txtServiceStatus.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.txtServiceStatus.Properties.AppearanceFocused.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtServiceStatus.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtServiceStatus.Properties.AppearanceFocused.Options.UseFont = true;
            this.txtServiceStatus.Properties.AutoHeight = false;
            this.txtServiceStatus.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.txtServiceStatus.Properties.ReadOnly = true;
            this.txtServiceStatus.Size = new System.Drawing.Size(153, 31);
            this.txtServiceStatus.TabIndex = 28;
            // 
            // txtServiceName
            // 
            this.txtServiceName.EnterMoveNextControl = true;
            this.txtServiceName.Location = new System.Drawing.Point(17, 91);
            this.txtServiceName.Name = "txtServiceName";
            this.txtServiceName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtServiceName.Properties.Appearance.Options.UseFont = true;
            this.txtServiceName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.txtServiceName.Properties.AppearanceFocused.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtServiceName.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtServiceName.Properties.AppearanceFocused.Options.UseFont = true;
            this.txtServiceName.Properties.AutoHeight = false;
            this.txtServiceName.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.txtServiceName.Properties.ReadOnly = true;
            this.txtServiceName.Size = new System.Drawing.Size(153, 31);
            this.txtServiceName.TabIndex = 26;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.ForeColor = System.Drawing.Color.Black;
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Appearance.Options.UseForeColor = true;
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.labelControl1.Location = new System.Drawing.Point(17, 72);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(153, 19);
            this.labelControl1.TabIndex = 27;
            this.labelControl1.Text = "Hizmet Adı";
            // 
            // frmServisAyarlari
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 207);
            this.Controls.Add(this.groupControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmServisAyarlari";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Servis Ayarları";
            this.Shown += new System.EventHandler(this.frmServisAyarlari_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictur.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDisplayAdi.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceName.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.PictureEdit pictur;
        private DevExpress.XtraEditors.SimpleButton btnServisOlustur;
        private DevExpress.XtraEditors.SimpleButton btnServisKur;
        private DevExpress.XtraEditors.SimpleButton btnBaslat;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnDurdur;
        public DevExpress.XtraEditors.TextEdit txtDisplayAdi;
        private DevExpress.XtraEditors.SimpleButton btnYenidenBaslat;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton btnServisSil;
        public DevExpress.XtraEditors.TextEdit txtServiceStatus;
        public DevExpress.XtraEditors.TextEdit txtServiceName;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}