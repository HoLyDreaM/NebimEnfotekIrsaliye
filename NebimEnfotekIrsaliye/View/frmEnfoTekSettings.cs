using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using NebimEnfotekIrsaliye.Data;
using NebimEnfotekIrsaliye.Utility;
using LiteDB;

namespace NebimEnfotekIrsaliye.View
{
    public partial class frmEnfoTekSettings : Form
    {
        public frmEnfoTekSettings()
        {
            InitializeComponent();
        }
        private void frmEnfoTekSettings_Shown(object sender, EventArgs e)
        {
            using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
            {
                AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                this.txtServer.Text = appSettings.EnfoTekSQLServer;
                this.txtDatabase.Text = appSettings.EnfoTekSQLDatabase;
                this.txtUser.Text = appSettings.EnfoTekSQLUser;
                this.txtPassword.Text = appSettings.EnfoTekSQLPass;
            }
        }
        private void btKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
                {
                    AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                    appSettings.EnfoTekSQLServer = this.txtServer.Text;
                    appSettings.EnfoTekSQLDatabase = this.txtDatabase.Text;
                    appSettings.EnfoTekSQLUser = this.txtUser.Text;
                    appSettings.EnfoTekSQLPass = this.txtPassword.Text;
                    liteRepository.Update<AppSettings>(appSettings, null);
                }
                MessageBox.Show("İşlem Tamam");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        private void btnKapat_Click(object sender, EventArgs e)
        {
            base.Close();
        }
    }
}
