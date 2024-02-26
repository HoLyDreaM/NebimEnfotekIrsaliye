namespace NebimEnfotekIrsaliye.View
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.xtraTabbedMdiManager1 = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager(this.components);
            this.rbMenu = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnNebim = new DevExpress.XtraBars.BarButtonItem();
            this.btnEnfoTek = new DevExpress.XtraBars.BarButtonItem();
            this.btnResimAktarimi = new DevExpress.XtraBars.BarButtonItem();
            this.btnNebimBaglantiAyarlari = new DevExpress.XtraBars.BarButtonItem();
            this.btnServisAyarlari = new DevExpress.XtraBars.BarButtonItem();
            this.btnWebSiparis = new DevExpress.XtraBars.BarButtonItem();
            this.btnIrsaliye = new DevExpress.XtraBars.BarButtonItem();
            this.btnAktarimAyarlari = new DevExpress.XtraBars.BarButtonItem();
            this.rbAyarlar = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.rpNebimAyarlari = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rgAyarlar = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rpProgramAyarlari = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rpGenelMenu = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.tiVersiyon = new System.Windows.Forms.Timer(this.components);
            this.stStatus = new System.Windows.Forms.StatusStrip();
            this.lblStatusStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblVersiyonBaslik = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblVersiyon = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTarih = new System.Windows.Forms.ToolStripStatusLabel();
            this.tiTime = new System.Windows.Forms.Timer(this.components);
            this.btnFtpAyarlari = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabbedMdiManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbMenu)).BeginInit();
            this.stStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // xtraTabbedMdiManager1
            // 
            this.xtraTabbedMdiManager1.MdiParent = this;
            // 
            // rbMenu
            // 
            this.rbMenu.AllowTrimPageText = false;
            this.rbMenu.ApplicationIcon = global::NebimEnfotekIrsaliye.Properties.Resources.verimek_ico;
            this.rbMenu.ExpandCollapseItem.Id = 0;
            this.rbMenu.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.rbMenu.ExpandCollapseItem,
            this.btnNebim,
            this.btnEnfoTek,
            this.btnResimAktarimi,
            this.btnNebimBaglantiAyarlari,
            this.btnServisAyarlari,
            this.btnWebSiparis,
            this.btnIrsaliye,
            this.btnAktarimAyarlari});
            this.rbMenu.Location = new System.Drawing.Point(0, 0);
            this.rbMenu.MaxItemId = 18;
            this.rbMenu.Name = "rbMenu";
            this.rbMenu.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.rbAyarlar});
            this.rbMenu.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.rbMenu.Size = new System.Drawing.Size(801, 143);
            // 
            // btnNebim
            // 
            this.btnNebim.Caption = "Nebim Bağlantı Ayarları";
            this.btnNebim.Id = 1;
            this.btnNebim.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNebim.ImageOptions.Image")));
            this.btnNebim.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnNebim.ImageOptions.LargeImage")));
            this.btnNebim.Name = "btnNebim";
            this.btnNebim.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnNebim_ItemClick);
            // 
            // btnEnfoTek
            // 
            this.btnEnfoTek.Caption = "EnfoTek Bağlantı Ayarları";
            this.btnEnfoTek.Id = 2;
            this.btnEnfoTek.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEnfoTek.ImageOptions.Image")));
            this.btnEnfoTek.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnEnfoTek.ImageOptions.LargeImage")));
            this.btnEnfoTek.Name = "btnEnfoTek";
            this.btnEnfoTek.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnEnfoTek_ItemClick);
            // 
            // btnResimAktarimi
            // 
            this.btnResimAktarimi.Caption = "Resim Aktarımı";
            this.btnResimAktarimi.Id = 10;
            this.btnResimAktarimi.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnResimAktarimi.ImageOptions.Image")));
            this.btnResimAktarimi.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnResimAktarimi.ImageOptions.LargeImage")));
            this.btnResimAktarimi.LargeWidth = 120;
            this.btnResimAktarimi.Name = "btnResimAktarimi";
            // 
            // btnNebimBaglantiAyarlari
            // 
            this.btnNebimBaglantiAyarlari.Caption = "Nebim Bağlantı Ayarları";
            this.btnNebimBaglantiAyarlari.Id = 11;
            this.btnNebimBaglantiAyarlari.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNebimBaglantiAyarlari.ImageOptions.Image")));
            this.btnNebimBaglantiAyarlari.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnNebimBaglantiAyarlari.ImageOptions.LargeImage")));
            this.btnNebimBaglantiAyarlari.Name = "btnNebimBaglantiAyarlari";
            this.btnNebimBaglantiAyarlari.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnNebimBaglantiAyarlari_ItemClick);
            // 
            // btnServisAyarlari
            // 
            this.btnServisAyarlari.Caption = "Servis Ayarlari";
            this.btnServisAyarlari.Id = 12;
            this.btnServisAyarlari.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnServisAyarlari.ImageOptions.Image")));
            this.btnServisAyarlari.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnServisAyarlari.ImageOptions.LargeImage")));
            this.btnServisAyarlari.LargeWidth = 90;
            this.btnServisAyarlari.Name = "btnServisAyarlari";
            this.btnServisAyarlari.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnServisAyarlari_ItemClick);
            // 
            // btnWebSiparis
            // 
            this.btnWebSiparis.Id = 15;
            this.btnWebSiparis.Name = "btnWebSiparis";
            // 
            // btnIrsaliye
            // 
            this.btnIrsaliye.Caption = "İrsaliye Aktarım";
            this.btnIrsaliye.Id = 16;
            this.btnIrsaliye.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnIrsaliye.ImageOptions.Image")));
            this.btnIrsaliye.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnIrsaliye.ImageOptions.LargeImage")));
            this.btnIrsaliye.LargeWidth = 150;
            this.btnIrsaliye.Name = "btnIrsaliye";
            this.btnIrsaliye.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnIrsaliye_ItemClick);
            // 
            // btnAktarimAyarlari
            // 
            this.btnAktarimAyarlari.Caption = "Aktarım Ayarları";
            this.btnAktarimAyarlari.Id = 17;
            this.btnAktarimAyarlari.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAktarimAyarlari.ImageOptions.Image")));
            this.btnAktarimAyarlari.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnAktarimAyarlari.ImageOptions.LargeImage")));
            this.btnAktarimAyarlari.LargeWidth = 90;
            this.btnAktarimAyarlari.Name = "btnAktarimAyarlari";
            this.btnAktarimAyarlari.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAktarimAyarlari_ItemClick);
            // 
            // rbAyarlar
            // 
            this.rbAyarlar.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.rpNebimAyarlari,
            this.rgAyarlar,
            this.rpProgramAyarlari,
            this.rpGenelMenu});
            this.rbAyarlar.Name = "rbAyarlar";
            this.rbAyarlar.Text = "Ayarlar";
            // 
            // rpNebimAyarlari
            // 
            this.rpNebimAyarlari.ItemLinks.Add(this.btnNebim);
            this.rpNebimAyarlari.Name = "rpNebimAyarlari";
            this.rpNebimAyarlari.Text = "Nebim Ayarları";
            // 
            // rgAyarlar
            // 
            this.rgAyarlar.ItemLinks.Add(this.btnNebimBaglantiAyarlari);
            this.rgAyarlar.ItemLinks.Add(this.btnEnfoTek);
            this.rgAyarlar.Name = "rgAyarlar";
            this.rgAyarlar.Text = "Nebim - EnfoTek Ayarları";
            // 
            // rpProgramAyarlari
            // 
            this.rpProgramAyarlari.ItemLinks.Add(this.btnServisAyarlari);
            this.rpProgramAyarlari.ItemLinks.Add(this.btnAktarimAyarlari);
            this.rpProgramAyarlari.Name = "rpProgramAyarlari";
            this.rpProgramAyarlari.Text = "Program Ayarları";
            // 
            // rpGenelMenu
            // 
            this.rpGenelMenu.ItemLinks.Add(this.btnIrsaliye);
            this.rpGenelMenu.Name = "rpGenelMenu";
            this.rpGenelMenu.Text = "Program Genel Menusu";
            // 
            // tiVersiyon
            // 
            this.tiVersiyon.Enabled = true;
            this.tiVersiyon.Interval = 1800000;
            this.tiVersiyon.Tick += new System.EventHandler(this.tiVersiyon_Tick);
            // 
            // stStatus
            // 
            this.stStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatusStripLabel,
            this.lblVersiyonBaslik,
            this.lblVersiyon,
            this.lblTarih});
            this.stStatus.Location = new System.Drawing.Point(0, 383);
            this.stStatus.Name = "stStatus";
            this.stStatus.Size = new System.Drawing.Size(801, 22);
            this.stStatus.TabIndex = 2;
            this.stStatus.Text = "statusStrip1";
            // 
            // lblStatusStripLabel
            // 
            this.lblStatusStripLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblStatusStripLabel.Name = "lblStatusStripLabel";
            this.lblStatusStripLabel.Size = new System.Drawing.Size(691, 17);
            this.lblStatusStripLabel.Spring = true;
            this.lblStatusStripLabel.Click += new System.EventHandler(this.lblStatusStripLabel_Click);
            // 
            // lblVersiyonBaslik
            // 
            this.lblVersiyonBaslik.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblVersiyonBaslik.Image = global::NebimEnfotekIrsaliye.Properties.Resources.versions;
            this.lblVersiyonBaslik.Name = "lblVersiyonBaslik";
            this.lblVersiyonBaslik.Size = new System.Drawing.Size(79, 17);
            this.lblVersiyonBaslik.Text = "Versiyon : ";
            // 
            // lblVersiyon
            // 
            this.lblVersiyon.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblVersiyon.Name = "lblVersiyon";
            this.lblVersiyon.Size = new System.Drawing.Size(0, 17);
            // 
            // lblTarih
            // 
            this.lblTarih.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblTarih.Image = global::NebimEnfotekIrsaliye.Properties.Resources.saat;
            this.lblTarih.Name = "lblTarih";
            this.lblTarih.Size = new System.Drawing.Size(16, 17);
            // 
            // tiTime
            // 
            this.tiTime.Enabled = true;
            this.tiTime.Tick += new System.EventHandler(this.tiTime_Tick);
            // 
            // btnFtpAyarlari
            // 
            this.btnFtpAyarlari.Id = -1;
            this.btnFtpAyarlari.Name = "btnFtpAyarlari";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 405);
            this.Controls.Add(this.stStatus);
            this.Controls.Add(this.rbMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.Name = "frmMain";
            this.Ribbon = this.rbMenu;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enfotek - Nebim Entegrasyonu";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabbedMdiManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbMenu)).EndInit();
            this.stStatus.ResumeLayout(false);
            this.stStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraTabbedMdi.XtraTabbedMdiManager xtraTabbedMdiManager1;
        private DevExpress.XtraBars.Ribbon.RibbonControl rbMenu;
        private DevExpress.XtraBars.Ribbon.RibbonPage rbAyarlar;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rgAyarlar;
        private DevExpress.XtraBars.BarButtonItem btnNebim;
        private DevExpress.XtraBars.BarButtonItem btnEnfoTek;
        public System.Windows.Forms.Timer tiVersiyon;
        private DevExpress.XtraBars.BarButtonItem btnResimAktarimi;
        private DevExpress.XtraBars.BarButtonItem btnNebimBaglantiAyarlari;
        private DevExpress.XtraBars.BarButtonItem btnServisAyarlari;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpNebimAyarlari;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpProgramAyarlari;
        private System.Windows.Forms.StatusStrip stStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusStripLabel;
        private System.Windows.Forms.Timer tiTime;
        private System.Windows.Forms.ToolStripStatusLabel lblVersiyonBaslik;
        private System.Windows.Forms.ToolStripStatusLabel lblVersiyon;
        private System.Windows.Forms.ToolStripStatusLabel lblTarih;
        private DevExpress.XtraBars.BarButtonItem btnWebSiparis;
        private DevExpress.XtraBars.BarButtonItem btnFtpAyarlari;
        private DevExpress.XtraBars.BarButtonItem btnIrsaliye;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpGenelMenu;
        private DevExpress.XtraBars.BarButtonItem btnAktarimAyarlari;
    }
}

