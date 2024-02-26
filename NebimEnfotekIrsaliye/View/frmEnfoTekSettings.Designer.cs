namespace NebimEnfotekIrsaliye.View
{
    partial class frmEnfoTekSettings
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
            this.lblSunucuIp = new System.Windows.Forms.Label();
            this.lblKullaniciAdi = new System.Windows.Forms.Label();
            this.lblParola = new System.Windows.Forms.Label();
            this.lblVeritabani = new System.Windows.Forms.Label();
            this.txtServer = new DevExpress.XtraEditors.TextEdit();
            this.txtUser = new DevExpress.XtraEditors.TextEdit();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.txtDatabase = new DevExpress.XtraEditors.TextEdit();
            this.btKaydet = new DevExpress.XtraEditors.SimpleButton();
            this.btnKapat = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtServer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabase.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSunucuIp
            // 
            this.lblSunucuIp.AutoSize = true;
            this.lblSunucuIp.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblSunucuIp.Location = new System.Drawing.Point(20, 24);
            this.lblSunucuIp.Name = "lblSunucuIp";
            this.lblSunucuIp.Size = new System.Drawing.Size(75, 15);
            this.lblSunucuIp.TabIndex = 0;
            this.lblSunucuIp.Text = "SUNUCU IP : ";
            // 
            // lblKullaniciAdi
            // 
            this.lblKullaniciAdi.AutoSize = true;
            this.lblKullaniciAdi.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblKullaniciAdi.Location = new System.Drawing.Point(20, 50);
            this.lblKullaniciAdi.Name = "lblKullaniciAdi";
            this.lblKullaniciAdi.Size = new System.Drawing.Size(93, 15);
            this.lblKullaniciAdi.TabIndex = 0;
            this.lblKullaniciAdi.Text = "KULLANICI ADI : ";
            // 
            // lblParola
            // 
            this.lblParola.AutoSize = true;
            this.lblParola.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblParola.Location = new System.Drawing.Point(20, 76);
            this.lblParola.Name = "lblParola";
            this.lblParola.Size = new System.Drawing.Size(57, 15);
            this.lblParola.TabIndex = 0;
            this.lblParola.Text = "PAROLA : ";
            // 
            // lblVeritabani
            // 
            this.lblVeritabani.AutoSize = true;
            this.lblVeritabani.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblVeritabani.Location = new System.Drawing.Point(20, 102);
            this.lblVeritabani.Name = "lblVeritabani";
            this.lblVeritabani.Size = new System.Drawing.Size(78, 15);
            this.lblVeritabani.TabIndex = 0;
            this.lblVeritabani.Text = "VERİTABANI : ";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(187, 17);
            this.txtServer.Name = "txtServer";
            this.txtServer.Properties.Appearance.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtServer.Properties.Appearance.Options.UseFont = true;
            this.txtServer.Size = new System.Drawing.Size(158, 22);
            this.txtServer.TabIndex = 0;
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(187, 43);
            this.txtUser.Name = "txtUser";
            this.txtUser.Properties.Appearance.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtUser.Properties.Appearance.Options.UseFont = true;
            this.txtUser.Size = new System.Drawing.Size(158, 22);
            this.txtUser.TabIndex = 1;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(187, 69);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.Appearance.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtPassword.Properties.Appearance.Options.UseFont = true;
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(158, 22);
            this.txtPassword.TabIndex = 2;
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(187, 95);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Properties.Appearance.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtDatabase.Properties.Appearance.Options.UseFont = true;
            this.txtDatabase.Size = new System.Drawing.Size(158, 22);
            this.txtDatabase.TabIndex = 3;
            // 
            // btKaydet
            // 
            this.btKaydet.Location = new System.Drawing.Point(187, 123);
            this.btKaydet.Name = "btKaydet";
            this.btKaydet.Size = new System.Drawing.Size(71, 23);
            this.btKaydet.TabIndex = 4;
            this.btKaydet.Text = "&Kaydet";
            this.btKaydet.Click += new System.EventHandler(this.btKaydet_Click);
            // 
            // btnKapat
            // 
            this.btnKapat.Location = new System.Drawing.Point(275, 123);
            this.btnKapat.Name = "btnKapat";
            this.btnKapat.Size = new System.Drawing.Size(70, 23);
            this.btnKapat.TabIndex = 5;
            this.btnKapat.Text = "&Kapat";
            this.btnKapat.Click += new System.EventHandler(this.btnKapat_Click);
            // 
            // frmEnfoTekSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(394, 151);
            this.Controls.Add(this.btnKapat);
            this.Controls.Add(this.btKaydet);
            this.Controls.Add(this.txtDatabase);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.lblVeritabani);
            this.Controls.Add(this.lblParola);
            this.Controls.Add(this.lblKullaniciAdi);
            this.Controls.Add(this.lblSunucuIp);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEnfoTekSettings";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SQL Bağlantı Ayarları";
            this.Shown += new System.EventHandler(this.frmEnfoTekSettings_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.txtServer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabase.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSunucuIp;
        private System.Windows.Forms.Label lblKullaniciAdi;
        private System.Windows.Forms.Label lblParola;
        private System.Windows.Forms.Label lblVeritabani;
        private DevExpress.XtraEditors.TextEdit txtServer;
        private DevExpress.XtraEditors.TextEdit txtUser;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraEditors.TextEdit txtDatabase;
        private DevExpress.XtraEditors.SimpleButton btKaydet;
        private DevExpress.XtraEditors.SimpleButton btnKapat;
    }
}