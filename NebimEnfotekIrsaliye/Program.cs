using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Serilog;
using System.IO;
using LiteDB;
using NebimEnfotekIrsaliye.Utility;
using NebimEnfotekIrsaliye.Data;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;

namespace NebimEnfotekIrsaliye
{
    static class Program
    {
        internal static ApplicationContext ac = new ApplicationContext();
        static Controller.clsMain cMain = new Controller.clsMain();
        public static string strUrlAddress = "";

        [STAThread]
        static void Main()
        {
            try
            {
                Application.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;

                DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
                Control.CheckForIllegalCrossThreadCalls = false;

                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ErrorHandlerUIThread);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ErrorHandlerDomain);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                var webRequest = WebRequest.Create(@"https://raw.githubusercontent.com/HoLyDreaM/ProjectVersion/main/README.md");

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    strUrlAddress = reader.ReadToEnd().Trim();
                }

                if (!File.Exists(Path.GetFullPath(App.GetAppDataFile)))
                {
                    using (LiteDatabase liteDatabase = new LiteDatabase(App.GetAppDataFile, null, null))
                    {
                        if (!liteDatabase.CollectionExists("AppSettings"))
                        {
                            liteDatabase.GetCollection<AppSettings>("AppSettings").Insert(new AppSettings
                            {
                                Id = 1,
                                VersionWebAdress = strUrlAddress
                            });
                        }
                    }
                }
                else
                {
                    using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
                    {
                        AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                        appSettings.VersionWebAdress = strUrlAddress;

                        liteRepository.Update<AppSettings>(appSettings, null);
                    }
                }

                View.frmSplash fSplash = new View.frmSplash(cMain);
                if (fSplash.ShowDialog() == DialogResult.OK)
                {
                    fSplash.Close();
                    ac.MainForm = new View.frmMain(cMain);
                    Application.Run(ac);
                }
            }
            catch (Exception ex)
            {
                Log.Error("İşlem Sırasında Bilinmeyen Bir Hata Oluştu. Detay:\n" + ex.Message.ToString());
                throw new Exception("İşlem Sırasında Bilinmeyen Bir Hata Oluştu. Detay:\n", ex);
            }
        }
        static void ErrorHandlerDomain(object sender, UnhandledExceptionEventArgs args)
        {
            Exception excMain = (Exception)args.ExceptionObject;
            SM_Lib.Utils.cUtils.ShowError("Çalışma sırasında beklenmedik hata oluştu.\nDetay : " + excMain.ToString(), true);
            Serilog.Log.Fatal("Çalışma sırasında beklenmedik hata oluştu.Detay:{Error}", excMain);
            //MessageBox.Show("Çalışma sırasında beklenmedik hata oluştu.\nDetay : " + excMain.ToString(), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        static void ErrorHandlerUIThread(object sender, System.Threading.ThreadExceptionEventArgs args)
        {
            Exception excMain = args.Exception;
            SM_Lib.Utils.cUtils.ShowError("Çalışma sırasında beklenmedik hata oluştu.\nDetay : " + excMain.ToString(), true);
            Serilog.Log.Fatal("Çalışma sırasında beklenmedik hata oluştu.Detay:{Error}", excMain);
            //MessageBox.Show("Çalışma sırasında beklenmedik hata oluştu.\nDetay : " + excMain.ToString(), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
