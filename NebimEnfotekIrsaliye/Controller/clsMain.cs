using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Serilog;
using Dapper;
using SM_Lib;
using SM_Lib.Utils;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LiteDB;
using NebimEnfotekIrsaliye.Utility;
using NebimEnfotekIrsaliye.Data;
using System.Text.RegularExpressions;

namespace NebimEnfotekIrsaliye.Controller
{
    public class clsMain
    {
        private SM_Lib.Controller.clsLicence cLicence;
        private SM_Lib.Controller.clsSettings clsSettings;

        public string AppName = "NebimEnfotekIrsaliye";
        public string Tanim = "NebimEnfotekIrsaliye";
        public string AppVersion = "";
        public string OldVersion = "";
        public string strFile;
        public string AppDesc = "";
        public string VersionWebAdress = "";
        System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("tr-TR");

        public bool Init(Label lblBilgi)
        {
            bool blResult = false;
            string strResult = "";
            try
            {
                #region Loglama Kütüphanesi başlatılıyor...
                lblBilgi.Text = "Loglama Kütüphanesi başlatılıyor...";
                SharpConfig.Configuration cfgApp;
                if (!System.IO.File.Exists("App.cfg"))
                {
                    //APPCFG bulamadik, olusturalim.
                    cfgApp = new SharpConfig.Configuration();
                    SharpConfig.Section secDebug = cfgApp["Debug Config"];
                    secDebug["MinDebugLevel"].SetValue<Serilog.Events.LogEventLevel>(Serilog.Events.LogEventLevel.Error);
                    cfgApp.Save("App.cfg");
                }
                else
                {
                    cfgApp = SharpConfig.Configuration.LoadFromFile("App.cfg");
                }

                #region Log klasoru yoksa olusturalim
                if (!System.IO.Directory.Exists(Application.StartupPath + @"\Logs"))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(Application.StartupPath + @"\Logs");
                    }
                    catch (Exception excMain)
                    {
                        throw new Exception(String.Format("{0} klasörü oluşturulamadı!", Application.StartupPath + @"\Logs"), excMain);
                    }
                }
                #endregion

                Log.Logger = new LoggerConfiguration()
                                .Enrich.WithProcessId()
                                .Enrich.WithProperty("UserName", System.Security.Principal.WindowsIdentity.GetCurrent().Name)
                                .WriteTo
                                .RollingFile(Application.StartupPath + @"\Logs\Log_" + Environment.MachineName + "_" + Environment.UserName + @"_{Date}.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] ({UserName})({ProcessId}) {Message}{NewLine}{Exception}") //, Serilog.Events.LogEventLevel.Debug /*cfgApp["Debug Config"]["MinDebugLevel"].GetValue<Serilog.Events.LogEventLevel>()*/)
                                .MinimumLevel.Is(Serilog.Events.LogEventLevel.Debug)
                                .CreateLogger();

                Log.Information("Application started at {Now}", DateTime.Now);
                #endregion
        
                lblBilgi.Text = "Lisans Okunuyor...";
                Application.DoEvents();
                clsSettings = new SM_Lib.Controller.clsSettings();
                strResult = clsSettings.LoadLisans();
                if (strResult != "")
                    throw new Exception("Lisans Hatası!");

                lblBilgi.Text = "Modüller Okunuyor...";
                Application.DoEvents();
                cLicence = new SM_Lib.Controller.clsLicence(clsSettings);
                strResult = cLicence.GetAppLisansCheck(AppName, GetVersion());
                if (strResult != "")
                {
                    if (strResult == "İnternet Bağlantısı Bulunmamaktadır.")
                    {
                        cSettings.LoadConnect();
                        int dKontrol = cSettings._INTERNET_CONTROL + 1;
                        if (dKontrol <= 10)
                        {
                            cSettings.SaveConnect(dKontrol);
                        }
                        else
                        {
                            strResult = "İnternet Olmadan Programa Giriş Hakkı Sayınızı Doldurdunuz.";
                            throw new Exception("Modül Hatası!");
                        }
                    }
                    else
                    {
                        throw new Exception("Modül Hatası!");
                    }
                }
                else if (IsConnected())
                {
                    cSettings.SaveConnect(0);
                }

                AppVersion = NewVersion();
                OldVersion = GetVersion();
                
                cSettings.AppName = AppName;

                string strVersiyon = CheckpUpdate();
                if (strVersiyon != "En Güncel Sürüme Sahipsiniz...")
                {
                    if (strVersiyon != "")
                    {
                        if (MessageBox.Show("Yeni Güncel Versiyon Bulundu.\nŞimdi Yeni Versiyona Geçiş Yapılsın mı?", "Güncelle", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                        {
                            string strAppExe = System.Environment.GetCommandLineArgs()[0];
                            string strArgs = AppName + " " + AppDesc.Trim().Replace(" ", "//") + " " + OldVersion + " " + AppVersion + " " + strAppExe + "";
                            Process.Start("Entegrasyon_Update.exe", Tanim);
                            Application.Exit();
                        }
                    }
                }

                blResult = true;

            }
           catch (Exception ex)
            {
                blResult = false;
                MessageBox.Show(ex.Message + "\nDetay : " + strResult, "Giriş Başarısız!!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return blResult;
        }
        public string GetVersion()
        {
            string strVersion = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("00") + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("00") + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("00") + Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString("00");
            return strVersion;
        }
        public string NewVersion()
        {
            using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
            {
                AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                VersionWebAdress = appSettings.VersionWebAdress;
            }

            ListBox lstProgram = GetDirectoryListingRegexForUrl(VersionWebAdress, AppName);
            string[] strAppVersiyon = new string[2];
            for (int i = 0; i < lstProgram.Items.Count; i++)
            {
                if (!lstProgram.Items[i].ToString().Contains("_Integration"))
                {
                    strAppVersiyon = lstProgram.Items[i].ToString().Split('_');
                    break;
                }
            }
            AppVersion = strAppVersiyon[strAppVersiyon.Length - 1].ToString().Split('.')[0].ToString();

            using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
            {
                AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                appSettings.AppName = AppName;
                appSettings.Version = AppVersion;
                appSettings.VersionWebAdress = VersionWebAdress;
                liteRepository.Update<AppSettings>(appSettings, null);
            }
            return AppVersion;
        }
        public string CheckpUpdate()
        {
            string strResult = "";

            if (Convert.ToInt32(AppVersion) > Convert.ToInt32(OldVersion))
            {
                strResult = "Yeni Sürüm Bulundu. Güncel Sürüm : " + AppVersion + " ";
            }
            else
            {
                strResult = "En Güncel Sürüme Sahipsiniz...";
            }
            return strResult;
        }
        public ListBox GetDirectoryListingRegexForUrl(string url, string strAppName)
        {
            ListBox lstResult = new ListBox();

            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Regex regex = new Regex("href=\"(?<Link>.*?)\"",
            RegexOptions.IgnoreCase
            | RegexOptions.CultureInvariant
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled);

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();

                MatchCollection matches = regex.Matches(result);
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        if (match.Groups[1].Value.Contains(strAppName))
                        {
                            string[] strSplit = match.Groups[1].Value.ToString().Split('/');

                            for (int i = 0; i < strSplit.Length; i++)
                            {
                                string strProgram = match.Groups[1].Value.ToString().Split('/')[2].ToString();
                                lstResult.Items.Add(strProgram);
                                break;
                            }
                        }
                    }
                }

                int rows = lstResult.Items.Count;
                string[] array = new string[rows];
                for (int i = 0; i < lstResult.Items.Count; i++)
                {
                    array[i] = lstResult.Items[i].ToString();
                }
                Array.Reverse(array);
                lstResult.Items.Clear();
                lstResult.Items.AddRange(array);
            }

            return lstResult;
        }
        public bool IsConnected()
        {
            try
            {
                System.Net.Sockets.TcpClient kontrol_client = new System.Net.Sockets.TcpClient("www.google.com.tr", 80);
                kontrol_client.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class RefreshTokenResultJSON
    {
        public string SessionID { get; set; }
    }
}
