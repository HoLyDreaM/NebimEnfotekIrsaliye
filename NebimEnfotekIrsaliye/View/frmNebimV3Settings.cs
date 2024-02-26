using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using NebimEnfotekIrsaliye.Data;
using NebimEnfotekIrsaliye.Utility;
using LiteDB;
using System.Data;

namespace NebimEnfotekIrsaliye.View
{
    public partial class frmNebimV3Settings : Form
    {
        Controller.clsMain cMain;
        DataTable dtProduct = new DataTable();
        public frmNebimV3Settings(Controller.clsMain cMain)
        {
            InitializeComponent();
            this.cMain = cMain;
        }
        private void frmNebimV3Settings_Shown(object sender, EventArgs e)
        {
            using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
            {
                AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                this.txtServer.Text = appSettings.NebimV3Ip;
                this.txtDatabase.Text = appSettings.NebimV3DatabaseName;
                this.txtUser.Text = appSettings.NebimV3UserName;
                this.txtPassword.Text = appSettings.NebimV3PassWord;
                this.txtUserGrp.Text = appSettings.NebimV3UserGroup;
            }
        }
        private void btKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
                {
                    AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                    appSettings.NebimV3Ip = this.txtServer.Text;
                    appSettings.NebimV3DatabaseName = this.txtDatabase.Text;
                    appSettings.NebimV3UserName = this.txtUser.Text;
                    appSettings.NebimV3PassWord = this.txtPassword.Text;
                    appSettings.NebimV3UserGroup = this.txtUserGrp.Text;
                    liteRepository.Update<AppSettings>(appSettings, null);
                }
                MessageBox.Show("İşlem Tamam");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata : " + ex.Message);
            }
        }
        private void btnKapat_Click(object sender, EventArgs e)
        {
            base.Close();
        }
    }
}
