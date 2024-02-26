using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using LiteDB;
using SM_Lib.View;
using NebimEnfotekIrsaliye.Data;
using NebimEnfotekIrsaliye.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SM_Lib.Utils;
using System.Threading;
using DevExpress.XtraGrid;


namespace NebimEnfotekIrsaliye.View
{
    public partial class frmIrsaliyeAktarimi : Form
    {
        DataTable dtSippment = new DataTable();
        DataTable dtSippments = new DataTable();
        DataTable dtSippmentProduct = new DataTable();
        DataSet dsMain = new DataSet();

        public string strIrsaliyeID;
        bool blnCheck = false;
        BackgroundWorker bwList = new BackgroundWorker();
        BackgroundWorker bwInsert = new BackgroundWorker();
        public frmIrsaliyeAktarimi()
        {
            InitializeComponent();
            prBar.Visible = false;

            bwList.DoWork += new DoWorkEventHandler(bw_DoWork);
            bwList.RunWorkerCompleted += bwList_RunWorkerCompleted;
            bwList.ProgressChanged += bwList_ProgressChanged;

            bwInsert.DoWork += new DoWorkEventHandler(bw_DoWorkAktar);
            bwInsert.RunWorkerCompleted += bwInsert_RunWorkerCompleted;
            bwInsert.ProgressChanged += bwInsert_ProgressChanged;
        }
        private void frmIrsaliyeAktarimi_Shown(object sender, EventArgs e)
        {
            btnAktar.Enabled = false;
        }
        private void btnListele_Click(object sender, EventArgs e)
        {
            prBar.Visible = true;
            bwList.WorkerReportsProgress = true;
            bwList.RunWorkerAsync();
        }
        void bw_DoWork(Object sender, DoWorkEventArgs e)
        {
            int dSay = 0;

            try
            {
                ClearConstraints(dsMain);
                dsMain.Relations.Clear();
                dsMain.Tables.Clear();
                dtSippment.Rows.Clear();
                dtSippments.Rows.Clear();
                dtSippmentProduct.Rows.Clear();

                IntegrationResult<List<ShipmentHeader>> Sippment = Functions.GetSippment();
                IntegrationResult<List<ShipmentHeader>> Sippment2 = Functions.GetSippment2();
                dtSippment = Functions.ConvertToDataTable(Sippment.Data);
                dtSippments = Functions.ConvertToDataTable(Sippment2.Data);
                strIrsaliyeID = "";

                for (int i = 0; i < dtSippment.Rows.Count; i++)
                {
                    bool blnCheckIrs = Functions.EnfotekIrsCheck(Convert.ToInt32(dtSippment.Rows[i]["MH_EIRSALIYEID"].ToString()));

                    if (blnCheckIrs)
                    {
                        DataRow _ravi = dtSippments.NewRow();
                        _ravi["AktarimNotu"] = dtSippment.Rows[i]["AktarimNotu"].ToString();
                        _ravi["MH_EIRSALIYEID"] = dtSippment.Rows[i]["MH_EIRSALIYEID"].ToString();
                        _ravi["UUID"] = dtSippment.Rows[i]["UUID"].ToString();
                        _ravi["TIP"] = dtSippment.Rows[i]["TIP"].ToString();
                        _ravi["IRS_NO"] = dtSippment.Rows[i]["IRS_NO"].ToString();
                        _ravi["TARIH"] = dtSippment.Rows[i]["TARIH"].ToString();
                        _ravi["CUST_CARI_ISIM"] = dtSippment.Rows[i]["CUST_CARI_ISIM"].ToString();
                        _ravi["CUST_CARI_VERGI_NO"] = dtSippment.Rows[i]["CUST_CARI_VERGI_NO"].ToString();
                        _ravi["CUST_CARI_TCKIMLIKNO"] = dtSippment.Rows[i]["CUST_CARI_TCKIMLIKNO"].ToString();
                        _ravi["CUST_CARI_ADRES"] = dtSippment.Rows[i]["CUST_CARI_ADRES"].ToString();
                        _ravi["SHIP_SEVKTAR"] = dtSippment.Rows[i]["SHIP_SEVKTAR"].ToString();
                        _ravi["SHIP_CARRIER_ISIM"] = dtSippment.Rows[i]["SHIP_CARRIER_ISIM"].ToString();
                        _ravi["SHIP_CARRIER_VERGI_DAIRESI"] = dtSippment.Rows[i]["SHIP_CARRIER_VERGI_DAIRESI"].ToString();
                        _ravi["SHIP_CARRIER_VERGI_NO"] = dtSippment.Rows[i]["SHIP_CARRIER_VERGI_NO"].ToString();
                        _ravi["SHIP_CARRIER_TCKIMLIKNO"] = dtSippment.Rows[i]["SHIP_CARRIER_TCKIMLIKNO"].ToString();
                        _ravi["SHIP_CARRIER_ADRES"] = dtSippment.Rows[i]["SHIP_CARRIER_ADRES"].ToString();
                        _ravi["SHIP_CARRIER_IL"] = dtSippment.Rows[i]["SHIP_CARRIER_IL"].ToString();
                        _ravi["SHIP_CARRIER_ILCE"] = dtSippment.Rows[i]["SHIP_CARRIER_ILCE"].ToString();
                        _ravi["SHIP_CARRIER_POSTA_KODU"] = dtSippment.Rows[i]["SHIP_CARRIER_POSTA_KODU"].ToString();
                        _ravi["SHIP_CARRIER_TEL_NO"] = dtSippment.Rows[i]["SHIP_CARRIER_TEL_NO"].ToString();
                        _ravi["SHIP_ARAC_PLAKA_NO"] = dtSippment.Rows[i]["SHIP_ARAC_PLAKA_NO"].ToString();
                        _ravi["SHIP_DORSE_PLAKA1"] = dtSippment.Rows[i]["SHIP_DORSE_PLAKA1"].ToString();
                        _ravi["SHIP_DRIVER_PERSON1_ADI"] = dtSippment.Rows[i]["SHIP_DRIVER_PERSON1_ADI"].ToString();
                        _ravi["SHIP_DRIVER_PERSON1_SOYADI"] = dtSippment.Rows[i]["SHIP_DRIVER_PERSON1_SOYADI"].ToString();
                        _ravi["SHIP_DRIVER_PERSON1_TCKIMLIKNO"] = dtSippment.Rows[i]["SHIP_DRIVER_PERSON1_TCKIMLIKNO"].ToString();
                        _ravi["HAREKET_TIPI"] = dtSippment.Rows[i]["HAREKET_TIPI"].ToString();
                        dtSippments.Rows.Add(_ravi);
                    }
                }

                if (dtSippments.Rows.Count > 0)
                {
                    foreach (DataRow drSippment in dtSippments.Rows)
                    {
                        strIrsaliyeID += drSippment["MH_EIRSALIYEID"].ToString() + ",";
                    }

                    strIrsaliyeID = strIrsaliyeID.TrimEnd(',');

                    IntegrationResult<List<ShipmentLine>> SippmentProduct = Functions.GetSippmentProduct(strIrsaliyeID);
                    dtSippmentProduct = Functions.ConvertToDataTable(SippmentProduct.Data);

                    dsMain.Tables.Add(dtSippments);
                    dsMain.Tables.Add(dtSippmentProduct);

                    string strRelationName = grdShipment.LevelTree.Nodes[0].RelationName.ToString();

                    DataColumn keyColumn = dsMain.Tables["Table1"].Columns["MH_EIRSALIYEID"];
                    DataColumn foreignKeyColumn = dsMain.Tables["Table2"].Columns["MH_EIRSALIYEID"];
                    dsMain.Relations.Add(strRelationName, keyColumn, foreignKeyColumn);

                    btnAktar.Enabled = true;
                    dSay = dtSippments.Rows.Count;
                }

                foreach (DataColumn dc in dtSippments.Columns)
                {
                    if (dc.ColumnName == "AktarimNotu")
                    {
                        dc.ReadOnly = false;
                        break;
                    }
                }

                for (int i = 0; i < dSay; i++)
                {
                    Thread.Sleep(100);
                    bwList.ReportProgress((100 * i) / dSay);
                }
                e.Result = dsMain;
            }
            catch (Exception ex)
            {
                e.Cancel = true;
            }
        }
        private void btnAktar_Click(object sender, EventArgs e)
        {
            prBar.Visible = true;
            bwInsert.WorkerReportsProgress = true;
            bwInsert.RunWorkerAsync();
        }
        void bw_DoWorkAktar(Object sender, DoWorkEventArgs e)
        {
            int dSay = 0, dKontrol = 0, dEnfotekIrsID = 0;
            bool blnDocumentNumberCheck = false;
            try
            {
                string strSessionID = "", strMessage = "", strMusteriKodu = "", strPostalID = "", strSeries = "", strIrsaliyeNo = "", strSeriNumber = "";

                using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
                {
                    AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);

                    Functions.Connect(appSettings.NebimV3Ip, appSettings.NebimV3UserName, appSettings.NebimV3UserGroup,
                    appSettings.NebimV3PassWord, appSettings.NebimV3DatabaseName, out strSessionID);

                    dSay = dtSippments.Rows.Count;

                    foreach (DataRow drSippment in dtSippments.Rows)
                    {
                        dKontrol++;
                        dEnfotekIrsID = Convert.ToInt32(drSippment["MH_EIRSALIYEID"].ToString());
                        string strTaxNumber = "", strVergi = "";

                        IntegrationResult<List<ShipmentLine>> SippmentProduct = null;

                        SippmentProduct = Functions.GetSippmentProductInsert(drSippment["MH_EIRSALIYEID"].ToInt());

                        if (!string.IsNullOrEmpty(strSessionID))
                        {
                            blnDocumentNumberCheck = Functions.EnfotekIrsCheck(dEnfotekIrsID);

                            if (blnDocumentNumberCheck)
                            {
                                IntegrationResult<List<ShipmentHeader>> ShipmentList = Functions.GetProductList(drSippment["MH_EIRSALIYEID"].ToInt());
                                
                                string strTypeKodu = drSippment["HAREKET_TIPI"].ToString() == "İade Çıkış" ? "1" : "3";
                                strTaxNumber = "";
                                strVergi = "";

                                if(string.IsNullOrEmpty(drSippment["CUST_CARI_VERGI_NO"].ToString()))
                                {
                                    strTaxNumber = drSippment["CUST_CARI_TCKIMLIKNO"].ToString();
                                    strVergi = "IdentityNum";
                                }
                                else
                                {
                                    strTaxNumber = drSippment["CUST_CARI_VERGI_NO"].ToString();
                                    strVergi = "TaxNumber";
                                }

                                DataTable dtMusteriBilgileri = Functions.NebimMusteriKodu(strTaxNumber, strTypeKodu, strVergi);
                                int dCustomerAdresTypeCount = dtMusteriBilgileri.Select("AddressTypeCode = 2").Count();

                                foreach (DataRow item in dtMusteriBilgileri.Rows)
                                {
                                    int dAddressTypeCode = Convert.ToInt32(item["AddressTypeCode"].ToString());
                                    if(dCustomerAdresTypeCount > 0)
                                    {
                                        foreach(DataRow drs in dtMusteriBilgileri.Select("AddressTypeCode = 2"))
                                        {
                                            strMusteriKodu = item["CurrAccCode"].ToString();
                                            strPostalID = item["PostalAddressID"].ToString();
                                            strIrsaliyeNo = drSippment["IRS_NO"].ToString();
                                            var Seriler = strIrsaliyeNo.ToCharArray().Where(x => Char.IsLetter(x));
                                            var SerilNolar = strIrsaliyeNo.ToCharArray().Where(x => Char.IsDigit(x));
                                            strSeriNumber = "";
                                            strSeries = "";
                                            foreach (var itemx in Seriler)
                                            {
                                                strSeries += itemx.ToString();
                                            }

                                            foreach (var itemy in SerilNolar)
                                            {
                                                strSeriNumber += itemy.ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (DataRow drs in dtMusteriBilgileri.Select("AddressTypeCode = 1"))
                                        {
                                            strMusteriKodu = item["CurrAccCode"].ToString();
                                            strPostalID = item["PostalAddressID"].ToString();
                                            strIrsaliyeNo = drSippment["IRS_NO"].ToString();
                                            var Seriler = strIrsaliyeNo.ToCharArray().Where(x => Char.IsLetter(x));
                                            var SerilNolar = strIrsaliyeNo.ToCharArray().Where(x => Char.IsDigit(x));
                                            strSeriNumber = "";
                                            strSeries = "";
                                            foreach (var itemx in Seriler)
                                            {
                                                strSeries += itemx.ToString();
                                            }

                                            foreach (var itemy in SerilNolar)
                                            {
                                                strSeriNumber += itemy.ToString();
                                            }
                                        }
                                    }
                                    break;
                                }

                                List<ShipmentLine> products = Functions.DataTableToList<ShipmentLine>(Functions.ConvertToDataTable(SippmentProduct.Data));

                                var ProductGrp = products.GroupBy(g => new
                                {
                                    g.KALEM_ACIKLAMA,
                                    g.MODEL,
                                    g.OLDMODEL,
                                    g.STOK_ADI,
                                    g.STOK_KODU
                                }).Select(group => new
                                {
                                    KALEM_ACIKLAMA = group.Key.KALEM_ACIKLAMA,
                                    MODEL = group.Key.MODEL,
                                    OLDMODEL = group.Key.MODEL,
                                    STOK_ADI = group.Key.STOK_ADI.Trim(),
                                    STOK_KODU = group.Key.STOK_KODU.Trim()
                                });

                                bool blnCheck = false;

                                foreach (var item in ProductGrp)
                                {
                                    if (item.KALEM_ACIKLAMA.ToString().Contains("ASORTİ") || item.KALEM_ACIKLAMA.ToString().Contains("asorti")
                                         || item.KALEM_ACIKLAMA.ToString().Contains(" ASS ") || item.KALEM_ACIKLAMA.ToString().Contains(" ass "))
                                    {
                                        blnCheck = true;
                                    }
                                }

                                if ((blnCheck) && (strTaxNumber == "7280059048"))
                                {
                                    foreach (var items in SippmentProduct.Data)
                                    {
                                        string strNebimItemCode = Functions.CheckEnfotekModel(items.MODEL, true);

                                        bool blnPKTModel = string.IsNullOrEmpty(strNebimItemCode) ? false : Convert.ToBoolean(Functions.CheckPKTModel(strNebimItemCode));

                                        if(!blnPKTModel)
                                        {
                                            strNebimItemCode = Functions.RandomCode() + "-AS";
                                        }

                                        if(!string.IsNullOrEmpty(strNebimItemCode))
                                        {
                                            string[] ModelSplit = items.MODEL.ToString().Trim().Split('-');
                                            bool blnModelChecks = false;

                                            for (int i = 0; i < ModelSplit.Length; i++)
                                            {
                                                if (ModelSplit[i].ToString().Trim().Contains("-AS"))
                                                {
                                                    blnModelChecks = true;
                                                }
                                            }
                                            if (!blnModelChecks)
                                            {
                                                items.MODEL = strNebimItemCode;
                                            }
                                        }
                                    }

                                    IntegrationResult integrationProduct = Functions.SendProductPenti(Functions.ConvertToDataTable(SippmentProduct.Data), SippmentProduct.Data, strSessionID, appSettings.NebimV3Ip);

                                    if (integrationProduct.ReturnCode != "OK")
                                    {
                                        string strError = string.Format("{0} {1}", integrationProduct.ReturnCode, integrationProduct.Message);

                                        drSippment["AktarimNotu"] = integrationProduct.Message;
                                    }
                                    else
                                    {
                                        IntegrationResult integrationResult = Functions.SendSippment(ShipmentList.Data, SippmentProduct.Data,
                                        strIrsaliyeNo, dEnfotekIrsID, strSessionID, appSettings.NebimV3Ip, strMusteriKodu, drSippment["HAREKET_TIPI"].ToString(),
                                        Convert.ToInt32(drSippment["MH_EIRSALIYEID"].ToString()), strSeries, strSeriNumber, Convert.ToDateTime(drSippment["TARIH"].ToString()), strPostalID, strPostalID, true);

                                        if (integrationResult.ReturnCode != "OK")
                                        {
                                            string strError = string.Format("{0} {1}", integrationResult.ReturnCode, integrationResult.Message);
                                        }

                                        drSippment["AktarimNotu"] = integrationResult.Message;
                                    }
                                }
                                else
                                {
                                    IntegrationResult integrationProduct = Functions.SendProduct(Functions.ConvertToDataTable(SippmentProduct.Data), SippmentProduct.Data, strSessionID, appSettings.NebimV3Ip);

                                    if (integrationProduct.ReturnCode != "OK")
                                    {
                                        string strError = string.Format("{0} {1}", integrationProduct.ReturnCode, integrationProduct.Message);

                                        drSippment["AktarimNotu"] = integrationProduct.Message;
                                    }
                                    else
                                    {
                                        IntegrationResult integrationResult = Functions.SendSippment(ShipmentList.Data, SippmentProduct.Data,
                                        strIrsaliyeNo, dEnfotekIrsID, strSessionID, appSettings.NebimV3Ip, strMusteriKodu, drSippment["HAREKET_TIPI"].ToString(),
                                        Convert.ToInt32(drSippment["MH_EIRSALIYEID"].ToString()), strSeries, strSeriNumber, Convert.ToDateTime(drSippment["TARIH"].ToString()), strPostalID, strPostalID, false);

                                        if (integrationResult.ReturnCode != "OK")
                                        {
                                            string strError = string.Format("{0} {1}", integrationResult.ReturnCode, integrationResult.Message);
                                        }

                                        drSippment["AktarimNotu"] = integrationResult.Message;
                                    }
                                }
                            }
                            else
                            {
                                drSippment["AktarimNotu"] = "Bu Sipariş Daha Önceden Aktarılmıştır.";
                            }

                            blnCheck = true;
                        }

                        Thread.Sleep(100);
                        bwInsert.ReportProgress((100 * dKontrol) / dSay);
                    }

                    Functions.Disconnect(appSettings.NebimV3Ip, appSettings.NebimV3UserName,
                    appSettings.NebimV3UserGroup, appSettings.NebimV3PassWord, appSettings.NebimV3DatabaseName, out strMessage);

                    if (blnCheck)
                    {
                        MessageBox.Show("İrsaliye Aktarımı Tamamlanmıştır.", "Basarili", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                btnAktar.Enabled = false;
            }
            catch(Exception ex)
            {
                e.Cancel = true;
            }
        }
        public void ClearConstraints(DataSet dataSet)
        {
            foreach (DataTable table in dataSet.Tables)
                table.Constraints.Clear();
        }
        private void MainInit()
        {
            this.prBar.Position = 0;
            this.prBar.Visible = false;
        }
        private void bwList_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.prBar.Position = e.ProgressPercentage;
        }
        private void bwList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DataSet ds = (DataSet)e.Result;
            grdShipment.DataSource = ds.Tables["Table1"];
            grdShipment.ForceInitialize();

            GridView gvProduct = new GridView(grdShipment);
            gvProduct.PopulateColumns(ds.Tables["Table2"]);
            this.MainInit();
        }
        private void bwInsert_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.prBar.Position = e.ProgressPercentage;
        }
        private void bwInsert_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.MainInit();
        }

    }
}
