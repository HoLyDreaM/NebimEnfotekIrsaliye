using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Diagnostics;
using System.ServiceProcess;
using System.Linq;
using System.IO;

namespace NebimEnfotekIrsaliye.View
{
    public partial class frmServisAyarlari : Form
    {
        ServiceController EnfotekService;
        public frmServisAyarlari()
        {
            InitializeComponent();
        }
        private void frmServisAyarlari_Shown(object sender, EventArgs e)
        {
            ServisDurumu();
        }
        private void btnServisKur_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = Application.StartupPath + @"\CreateService.bat";
                proc.StartInfo.Verb = "runas";
                proc.Start();

                System.Threading.Thread.Sleep(1000);

                ServisDurumu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void btnServisSil_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = Application.StartupPath + @"\DeleteService.bat";
                proc.StartInfo.Verb = "runas";
                proc.Start();

                System.Threading.Thread.Sleep(1000);

                ServisDurumu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void btnServisOlustur_Click(object sender, EventArgs e)
        {
            {
                string FileCreate = Application.StartupPath + "\\CreateService.bat";
                string FileDelete = Application.StartupPath + "\\DeleteService.bat";

                if (!Directory.Exists(FileCreate))
                {
                    File.Delete(FileCreate);
                }

                if (!Directory.Exists(FileDelete))
                {
                    File.Delete(FileDelete);
                }

                string Create = @"echo off " + Environment.NewLine +
                     Environment.NewLine +
    "\"%SystemRoot%\\Microsoft.NET\\Framework\\v4.0.30319\\Installutil\"" + " /i " + "\"" + Application.StartupPath + "\\NebimEnfotekIrsaliye_SYS.exe\" " + Environment.NewLine +
     Environment.NewLine +
    "echo \"####################### Islem Bitmistir #######################\" " + Environment.NewLine +
     Environment.NewLine;

                string Delete = @"echo off " + Environment.NewLine +
                    Environment.NewLine +
    "\"%SystemRoot%\\Microsoft.NET\\Framework\\v4.0.30319\\Installutil\"" + " /u " + "\"" + Application.StartupPath + "\\NebimEnfotekIrsaliye_SYS.exe\" " + Environment.NewLine +
        Environment.NewLine +
    "echo \"####################### Islem Bitmistir #######################\" " + Environment.NewLine +
     Environment.NewLine;

                using (FileStream fs = new FileStream(FileCreate, FileMode.CreateNew))
                {
                    using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8, 512))
                    {
                        writer.Write(Create);
                    }
                }

                using (FileStream fs = new FileStream(FileDelete, FileMode.CreateNew))
                {
                    using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8, 512))
                    {
                        writer.Write(Delete);
                        MessageBox.Show("Servis Dosyaları Başarılı Bir Şekilde Oluşturulmuştur", "İşlem Sonucu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        private void btnBaslat_Click(object sender, EventArgs e)
        {
            if (EnfotekService != null)
            {
                if (EnfotekService.Status != ServiceControllerStatus.Running)
                {
                    StartService(EnfotekService.ServiceName, 30000);
                    ServisDurumu();
                }
                else
                    MessageBox.Show("Servis Zaten Yürütülüyor!!");
            }
            else
                MessageBox.Show("Servis Kurulu Değil.Öncelikle Servisi Kurmalısınız!");
        }
        private void btnDurdur_Click(object sender, EventArgs e)
        {
            if (EnfotekService != null)
            {
                if (EnfotekService.Status != ServiceControllerStatus.Stopped)
                {
                    StopService(EnfotekService.ServiceName, 30000);
                    ServisDurumu();
                }
                else
                    MessageBox.Show("Servis Zaten Durmuş!");
            }
            else
                MessageBox.Show("Servis Kurulu Değil.Öncelikle Servisi Kurmalısınız!");

        }
        private void btnYenidenBaslat_Click(object sender, EventArgs e)
        {
            if (EnfotekService != null)
            {
                RestartService(EnfotekService.ServiceName, 30000, 1);
                ServisDurumu();
            }
        }
        private void ServisDurumu()
        {
            ServiceController[] services = ServiceController.GetServices();
            EnfotekService = services.Where(f => f.ServiceName.Equals("EnfoTek - Nebim Servis Entegrasyonu")).FirstOrDefault();

            if (EnfotekService != null)
            {
                txtServiceName.Text = EnfotekService.ServiceName.ToString();
                txtDisplayAdi.Text = EnfotekService.DisplayName.ToString();
                txtServiceStatus.Text = EnfotekService.Status.ToString();


                switch (EnfotekService.Status)
                {
                    case ServiceControllerStatus.Running:
                        pictur.Image = Properties.Resources.Play;
                        break;

                    case ServiceControllerStatus.Stopped:
                        pictur.Image = Properties.Resources.Stop;
                        break;

                    default:
                        pictur.Image = Properties.Resources.No_entry;
                        break;
                }
            }
            else
            {
                pictur.Image = Properties.Resources.No_entry;
                txtServiceName.Text = "NULL";
                txtDisplayAdi.Text = "NULL";
                txtServiceStatus.Text = "NULL";
            }
        }
        public static void StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);

            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        public static void StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        public void RestartService(string serviceName, int timeoutMilliseconds, int dDurum)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                if (service.Status != ServiceControllerStatus.Stopped)
                {
                    service.Stop();
                    if (dDurum == 1)
                    {
                        pictur.Image = Properties.Resources.Stop;
                    }
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
