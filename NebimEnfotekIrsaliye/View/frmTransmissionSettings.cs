using LiteDB;
using NebimEnfotekIrsaliye.Data;
using NebimEnfotekIrsaliye.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace NebimEnfotekIrsaliye.View
{
    public partial class frmTransmissionSettings : Form
    {
        public frmTransmissionSettings()
        {
            InitializeComponent();
        }
        private void frmTransmissionSettings_Shown(object sender, EventArgs e)
        {
            using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
            {
                AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);

                this.txtCompanyCode.Text = appSettings.CompanyCode.ToString();
                this.txtDepoKodu.Text = appSettings.DepoKodu;
                this.txtPostTerminalID.Text = appSettings.POSTerminalID;
                this.txtOfficeCode.Text = appSettings.OfficeCode;
                this.txtSuresi.Text = (appSettings.ServisSuresi / 60000).ToString();
                this.cmbTestIslem.SelectedItem = appSettings.IrsaliyeTest;
            }
        }
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
                {
                    AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                    appSettings.CompanyCode = Convert.ToInt32(this.txtCompanyCode.Text);
                    appSettings.DepoKodu = this.txtDepoKodu.Text;
                    appSettings.POSTerminalID = this.txtPostTerminalID.Text;
                    appSettings.OfficeCode = this.txtOfficeCode.Text;
                    appSettings.ServisSuresi = Convert.ToInt32(this.txtSuresi.Text) * 60000;
                    appSettings.IrsaliyeTest = this.cmbTestIslem.SelectedItem.ToString();

                    liteRepository.Update<AppSettings>(appSettings, null);
                }
                MessageBox.Show("Ayarlar Başarılı Şekilde Kaydedildi.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem Sırasında Hata Oluştu. Detay:\n" + " : " + ex.Message);
            }
        }
    }
}