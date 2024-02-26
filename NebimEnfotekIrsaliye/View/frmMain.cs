using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevExpress.XtraReports.UI;
using System.IO;
using System.Threading;

namespace NebimEnfotekIrsaliye.View
{
    public partial class frmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        Controller.clsMain cMain;
        public frmMain anaFrm
        {
            get;
            set;
        }
        public View.frmNebimV3Settings frmNebimV3Settings;
        public View.frmSQLSettings frmSQLSettings;
        public View.frmServisAyarlari frmServisAyarlari;
        public View.frmEnfoTekSettings frmEnfoTekSettings;
        public View.frmTransmissionSettings frmTransmissionSettings;
        public View.frmIrsaliyeAktarimi frmIrsaliyeAktarimi;

        public frmMain(Controller.clsMain cMain)
        {
            InitializeComponent();
            this.cMain = cMain;
        }
        private void frmMain_Shown(object sender, EventArgs e)
        {
            lblVersiyon.Text = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString().Replace(".", "0");

            View.bgForm frmacilis = new View.bgForm();
            frmacilis.MdiParent = this;
            frmacilis.Show();
        }
        private void btnNebim_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (frmNebimV3Settings == null || frmNebimV3Settings.IsDisposed)
            {
                frmNebimV3Settings = new View.frmNebimV3Settings(cMain);
                frmNebimV3Settings.Show();
            }
            else
            {
                frmNebimV3Settings.Activate();
            }
        }
        private void btnNebimBaglantiAyarlari_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (frmSQLSettings == null || frmSQLSettings.IsDisposed)
            {
                frmSQLSettings = new View.frmSQLSettings();
                frmSQLSettings.Show();
            }
            else
            {
                frmSQLSettings.Activate();
            }
        }
        private void btnEnfoTek_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (frmEnfoTekSettings == null || frmEnfoTekSettings.IsDisposed)
            {
                frmEnfoTekSettings = new View.frmEnfoTekSettings();
                frmEnfoTekSettings.Show();
            }
            else
            {
                frmEnfoTekSettings.Activate();
            }
        }
        private void btnServisAyarlari_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (frmServisAyarlari == null || frmServisAyarlari.IsDisposed)
            {
                frmServisAyarlari = new View.frmServisAyarlari();
                frmServisAyarlari.Show();
            }
            else
            {
                frmServisAyarlari.Activate();
            }
        }
        private void btnAktarimAyarlari_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (frmTransmissionSettings == null || frmTransmissionSettings.IsDisposed)
            {
                frmTransmissionSettings = new View.frmTransmissionSettings();
                frmTransmissionSettings.Show();
            }
            else
            {
                frmTransmissionSettings.Activate();
            }
        }
        private void btnIrsaliye_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (frmIrsaliyeAktarimi == null || frmIrsaliyeAktarimi.IsDisposed)
            {
                frmIrsaliyeAktarimi = new View.frmIrsaliyeAktarimi();
                frmIrsaliyeAktarimi.MdiParent = this;
                frmIrsaliyeAktarimi.Show();
            }
            else
            {
                frmIrsaliyeAktarimi.Activate();
            }
        }
        private void tiVersiyon_Tick(object sender, EventArgs e)
        {
            string strVersiyon = cMain.CheckpUpdate();
            string AppVersion = cMain.NewVersion();
            string OldVersion = cMain.GetVersion();
            string strTanim = "OpenCart";

            if (strVersiyon != "En Güncel Sürüme Sahipsiniz...")
            {
                if (strVersiyon != "")
                {
                    if (MessageBox.Show("Yeni Güncel Versiyon Bulundu.\nŞimdi Yeni Versiyona Geçiş Yapılsın mı?", "Güncelle", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                    {
                        string strAppExe = System.Environment.GetCommandLineArgs()[0];
                        string strArgs = cMain.AppName + " " + cMain.AppDesc.Trim().Replace(" ", "//") + " " + OldVersion + " " + AppVersion + " " + strAppExe + "";
                        System.Diagnostics.Process.Start("Entegrasyon_Update.exe", strTanim);
                        Application.Exit();
                    }
                }
            }
        }
        private void tiTime_Tick(object sender, EventArgs e)
        {
            lblTarih.Text = DateTime.Now.ToString();
        }
        private void lblStatusStripLabel_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("https://www.verimek.com/");
        }
    }
}
