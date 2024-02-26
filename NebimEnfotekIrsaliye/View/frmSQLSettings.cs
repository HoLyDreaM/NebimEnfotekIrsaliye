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
    public partial class frmSQLSettings : Form
    {
        public frmSQLSettings()
        {
            InitializeComponent();
        }
        private void frmSQLSettings_Shown(object sender, EventArgs e)
        {
            using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
            {
                AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                this.txtServer.Text = appSettings.SQLServer;
                this.txtDatabase.Text = appSettings.SQLDatabase;
                this.txtUser.Text = appSettings.SQLUser;
                this.txtPassword.Text = appSettings.SQLPass;
            }
        }
        private void btKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
                {
                    AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                    appSettings.SQLServer = this.txtServer.Text;
                    appSettings.SQLDatabase = this.txtDatabase.Text;
                    appSettings.SQLUser = this.txtUser.Text;
                    appSettings.SQLPass = this.txtPassword.Text;
                    liteRepository.Update<AppSettings>(appSettings, null);
                }
                MessageBox.Show("İşlem Tamam");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata:"  + ex.Message);
            }
        }
        private void btnKapat_Click(object sender, EventArgs e)
        {
            base.Close();
        }
    }
}
