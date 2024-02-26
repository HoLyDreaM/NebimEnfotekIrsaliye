using NebimEnfotekIrsaliye.Data;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SM_Lib.Utils;
using System.Threading;
using System.Reflection;
using System.ComponentModel;
using DevExpress.XtraGrid.Views.Grid;
using RestSharp;

namespace NebimEnfotekIrsaliye.Utility
{
    public static class Functions
    {
        static string strJson, strShippingNumber, strItemCode;
        public static bool Connect(string strIp, string strUserName, string strUserGroup, string strPass, string strDatabase, out string strMessage)
        {
            bool blnResult = false;
            strMessage = "";
            string strUrl = @"http://{0}/IntegratorService/Connect?DatabaseName={1}&UserGroupCode={2}&UserName={3}&Password={4}";

            strUrl = string.Format(strUrl, strIp, strDatabase, strUserGroup, strUserName, strPass);

            try
            {
                System.Net.WebRequest request = System.Net.HttpWebRequest.Create(strUrl);
                request.Method = "POST";
                request.ContentType = "application/json";

                Stream postStream = request.GetRequestStream();
                postStream.Flush();
                postStream.Close();

                using (System.Net.WebResponse response = request.GetResponse())
                {
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        dynamic jsonResponseText = streamReader.ReadToEnd();
                        RefreshTokenResultJSON jsonResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(jsonResponseText, typeof(RefreshTokenResultJSON));
                        strMessage = jsonResult.SessionID;
                    }
                }

                blnResult = true;
            }
            catch (Exception ex)
            {
                blnResult = false;
                strMessage = "Bilinmeyen Bir Hata Oluştu.Detay : \n" + ex.Message.ToString();
            }

            return blnResult;
        }
        public static bool Disconnect(string strIp, string strUserName, string strUserGroup, string strPass, string strDatabase, out string strMessage)
        {
            bool blnResult = false;
            strMessage = "";
            string strUrl = @"http://{0}/IntegratorService/Disconnect?DatabaseName={1}&UserGroupCode={2}&UserName={3}&Password={4}";

            strUrl = string.Format(strUrl, strIp, strDatabase, strUserGroup, strUserName, strPass);

            try
            {
                System.Net.WebRequest request = System.Net.HttpWebRequest.Create(strUrl);
                request.Method = "POST";
                request.ContentType = "application/json";

                Stream postStream = request.GetRequestStream();
                postStream.Flush();
                postStream.Close();

                using (System.Net.WebResponse response = request.GetResponse())
                {
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        dynamic jsonResponseText = streamReader.ReadToEnd();
                        RefreshTokenResultJSON jsonResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(jsonResponseText, typeof(RefreshTokenResultJSON));
                        strMessage = jsonResult.SessionID;
                    }
                }

                blnResult = true;
            }
            catch (Exception ex)
            {
                blnResult = false;
                strMessage = "Bilinmeyen Bir Hata Oluştu.Detay : \n" + ex.Message.ToString();
            }

            return blnResult;
        }

        public static string GetShippingSave(string strSessionID, string strIP, string strMusteriKodu, string strHareketTipi, 
            int dShippingID, string strSeries, string strSeriesNumber, string strShippingPostalAddressID, string strBillingPostalAddressID,
            List<ShipmentLine> orderProduct, DateTime dtTarih, bool blnPenti, out string strMessage)
        {
            strMessage = "";
            DataTable dtResult = new DataTable();
            string strUrl = "http://" + strIP + "/(S(" + strSessionID + "))/IntegratorService/Post?";
            string strOfficeCode = "", strCompanyCode = "", strPOSTerminalID = "", strDepoKodu = "", strSatisTemsilcisi = "";

            try
            {
                CultureInfo culture = new CultureInfo("en-US", true);

                using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
                {
                    AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                    strOfficeCode = appSettings.OfficeCode;
                    strCompanyCode = appSettings.CompanyCode.ToString();
                    strPOSTerminalID = appSettings.POSTerminalID;
                    strDepoKodu = appSettings.DepoKodu;

                    foreach (var item in orderProduct)
                    {
                        if(string.IsNullOrEmpty(item.NEBIMSTOK))
                        {
                            throw new Exception("Tanımsız Nebim Stok Kodu");
                        }

                        if(string.IsNullOrEmpty(strSatisTemsilcisi))
                        {
                            strSatisTemsilcisi = string.IsNullOrEmpty(item.SIP_TEMSILCI) ? "" : item.SIP_TEMSILCI.ToString().Trim();
                        }
                    }

                    if(string.IsNullOrEmpty(strSeriesNumber))
                    {
                        strSeriesNumber = "0";
                    }

                    string strTarih = "/Date({0})/";
                    strTarih = string.Format(strTarih, ToUnixTime(dtTarih));

                    if (blnPenti)
                    {
                        if (strHareketTipi.Contains("Yükleme"))
                        {
                            var Shipment = new Shipment()
                            {
                                ModelType = 60,
                                CustomerCode = strMusteriKodu,
                                CompanyCode = Convert.ToInt32(strCompanyCode),
                                OfficeCode = strOfficeCode,
                                WarehouseCode = strDepoKodu,
                                Series = strSeries,
                                SeriesNumber = Convert.ToInt64(strSeriesNumber),
                                Description = GetDescription(strSeries + strSeriesNumber, strMusteriKodu, strSatisTemsilcisi),
                                IsReturn = false,
                                Lines = new List<Lines>(GetUrunlerPenti(orderProduct)),
                                ShippingDate = strTarih,
                                OperationDate = strTarih,
                                ShippingPostalAddressID = strShippingPostalAddressID,
                                BillingPostalAddressID = strBillingPostalAddressID
                            };
                            var client = new RestClient(strUrl);
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/json");
                            var body = JsonConvert.SerializeObject(Shipment);
                            request.AddParameter("application/json", body, ParameterType.RequestBody);
                            IRestResponse response = client.Execute(request);

                            dynamic jsonResponseText = response.Content;
                            RefreshTokenResultJSON jsonResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(jsonResponseText, typeof(RefreshTokenResultJSON));
                            if (string.IsNullOrEmpty(jsonResult.ShippingNumber))
                            {
                                throw new Exception(jsonResult.ExceptionMessage.ToString());
                            }
                            else
                            {
                                strMessage = jsonResult.ShippingNumber;
                                strShippingNumber = jsonResult.ShippingNumber;
                            }
                        }
                    }
                    else
                    {
                        if (strHareketTipi.Contains("Yükleme"))
                        {
                            var Shipment = new Shipment()
                            {
                                ModelType = 60,
                                CustomerCode = strMusteriKodu,
                                CompanyCode = Convert.ToInt32(strCompanyCode),
                                OfficeCode = strOfficeCode,
                                WarehouseCode = strDepoKodu,
                                Series = strSeries,
                                SeriesNumber = Convert.ToInt64(strSeriesNumber),
                                Description = GetDescription(strSeries + strSeriesNumber, strMusteriKodu, strSatisTemsilcisi),
                                IsReturn = false,
                                Lines = new List<Lines>(GetUrunler(orderProduct)),
                                ShippingDate = strTarih,
                                OperationDate = strTarih,
                                ShippingPostalAddressID = strShippingPostalAddressID,
                                BillingPostalAddressID = strBillingPostalAddressID
                            };

                            //strJson = JsonConvert.SerializeObject(Shipment);
                            //strUrl += strJson;

                            var client = new RestClient(strUrl);
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/json");
                            var body = JsonConvert.SerializeObject(Shipment);
                            request.AddParameter("application/json", body, ParameterType.RequestBody);
                            IRestResponse response = client.Execute(request);

                            dynamic jsonResponseText = response.Content;
                            RefreshTokenResultJSON jsonResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(jsonResponseText, typeof(RefreshTokenResultJSON));
                            if (string.IsNullOrEmpty(jsonResult.ShippingNumber))
                            {
                                throw new Exception(jsonResult.ExceptionMessage.ToString());
                            }
                            else
                            {
                                strMessage = jsonResult.ShippingNumber;
                                strShippingNumber = jsonResult.ShippingNumber;
                            }
                        }
                        else
                        {
                            var Shipment = new Shipment2()
                            {
                                ModelType = 51,
                                VendorCode = strMusteriKodu,
                                CompanyCode = Convert.ToInt32(strCompanyCode),
                                OfficeCode = strOfficeCode,
                                WarehouseCode = strDepoKodu,
                                Series = strSeries,
                                SeriesNumber = Convert.ToInt64(strSeriesNumber),
                                Description = GetDescription(strSeries + strSeriesNumber, strMusteriKodu, strSatisTemsilcisi),
                                IsReturn = true,
                                ShippingPostalAddressID = strShippingPostalAddressID,
                                BillingPostalAddressID = strBillingPostalAddressID,
                                Lines = new List<Lines2>(GetUrunler2(orderProduct)),
                                ShippingDate = dtTarih,
                            };

                            //strJson = JsonConvert.SerializeObject(Shipment);
                            //strUrl += strJson;

                            var client = new RestClient(strUrl);
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/json");
                            var body = JsonConvert.SerializeObject(Shipment);
                            request.AddParameter("application/json", body, ParameterType.RequestBody);
                            IRestResponse response = client.Execute(request);

                            dynamic jsonResponseText = response.Content;
                            RefreshTokenResultJSON jsonResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(jsonResponseText, typeof(RefreshTokenResultJSON));
                            if (string.IsNullOrEmpty(jsonResult.ShippingNumber))
                            {
                                throw new Exception(jsonResult.ExceptionMessage.ToString());
                            }
                            else
                            {
                                strMessage = jsonResult.ShippingNumber;
                                strShippingNumber = jsonResult.ShippingNumber;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strMessage = "İşlem Sırasında Beklenmedik Bir Hata İle Karşılaşıldı. Detay:"  + ex.Message.ToString();
            }

            return strMessage;
        }

        public static string PostProduct(string strModel, string strModels, string strOldModel, string strStokAdi, string strRenk, string strColorCode, List<ShipmentLine> orderProduct, string strSessionID, string strIP, int dType, out string strMessage)
        {
            strMessage = "";
            string strUrl = "http://" + strIP + "/(S(" + strSessionID + "))/IntegratorService/Post?";
            try
            {
                //string strModels = strModel.Trim() + "-EFT";

                //if (strModels.Length > 16)
                //{
                //    strModels = ModelsReplace(strModels).ToUpper();

                //    if (strModels.Length > 16)
                //    {
                //        strModels = strModels.Substring(0, 16);
                //    }
                //}

                if (dType == 1)
                {
                    var Product = new Products()
                    {
                        ModelType = 4,
                        ItemCode = strModel,
                        BOMEntityCode = "",
                        ByWeight = false,
                        CommercialRoleCode = 0,
                        CustomsProductGroupCode = 0,
                        CustomsTariffNumberCode = "",
                        Descriptions = new List<Descriptions>(GetDescriptions(strStokAdi.Trim())),
                        DoNotLoadSectionTypeCodeAndDescriptionOnLoadItemSection = false,
                        GenerateOpticalDataMatrixCode = false,
                        GenerateSerialNumber = false,
                        GuaranteePeriod = 0,
                        IsBlocked = false,
                        IsPurchaseOrderClosed = false,
                        IsSalesOrderClosed = 0,
                        IsStoreOrderClosed = false,
                        IsSubsequentDeliveryForR = 0,
                        IsSubsequentDeliveryForRI = 0,
                        IsUTSDeclaratedItem = false,
                        ItemAccountGrCode = CheckItemAccountCode(strStokAdi.Trim()),
                        ItemDescription = "",
                        ItemDimTypeCode = 2,
                        ItemDiscountGrCode = "",
                        ItemPaymentPlanGrCode = "",
                        ItemTaxGrCode = "%10",
                        ItemVendorGrCode = "",
                        LinkedProductProperties = new LinkedProductProperties
                        {
                            ColorCode = "",
                            ContentItemCode = "",
                            ContentItemTypeCode = 1,
                            LinkedProductTypeCode = 0,
                            LotCode = ""
                        },
                        MaxCreditCardInstallmentCount = 12,
                        OrderLeadTime = 0,
                        OriginCountryCode = "",
                        PerceptionOfFashionCode = 0,
                        ProductCollectionGrCode = 0,
                        ProductFrameProperties = new ProductFrameProperties
                        {
                            BaseCurveRadius = 0,
                            BaseMaterialCode = "",
                            BrandCode = "",
                            BridgeWidth = 0,
                            CustomProcessGroupCode = "",
                            FrameShapeTypeCode = "",
                            FrameTypeCode = "",
                            LensHeight = 0,
                            LensWidth = 0,
                            ManufacturerCode = "",
                            OpticalSutCode = "",
                            TempleLength = 0
                        },
                        ProductHierarchyID = 0,
                        ProductLensProperties = new ProductLensProperties
                        {
                            BaseCurveRadius = 0,
                            BaseMaterialCode = "",
                            BrandCode = "",
                            CoatingTypeCode = "",
                            CustomProcessGroupCode = "",
                            Cylinder = 0,
                            Diameter = 0,
                            DisposeFrequency = 0,
                            EyeGlassSutTypeCode = 0,
                            FocalTypeCode = "",
                            GlassIndex = 0,
                            LensTypeCode = 0,
                            ManufacturerCode = "",
                            OpticalGroupRangeCode = "",
                            Sphere = 0,
                            WaterContent = 0
                        },
                        ProductTypeCode = 1,
                        PromotionGroupCode = "",
                        PromotionGroupCode2 = "",
                        ShelfLife = 0,
                        StoreCapacityLevelCode = "",
                        StorePriceLevelCode = 0,
                        SupplyPeriod = 0,
                        UnitConvertRate = 0,
                        UnitConvertRateNotFixed = false,
                        UnitOfMeasureCode1 = "AD",
                        UseBatch = false,
                        UseInternet = false,
                        UseManufacturing = false,
                        UsePOS = false,
                        UseRoll = false,
                        UseSerialNumber = false,
                        UseStore = false,
                        Variants = new List<Variant>(GetVariant(strColorCode, strModels, strOldModel, orderProduct))
                    };

                    //strJson = JsonConvert.SerializeObject(Product);
                    //strUrl += strJson;

                    var client = new RestClient(strUrl);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var body = JsonConvert.SerializeObject(Product);
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    dynamic jsonResponseText = response.Content;
                    RefreshTokenResultJSON jsonResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(jsonResponseText, typeof(RefreshTokenResultJSON));
                    if (string.IsNullOrEmpty(jsonResult.ItemCode))
                    {
                        throw new Exception(jsonResult.ExceptionMessage.ToString());
                    }
                    else
                    {
                        strMessage = jsonResult.ItemCode;
                        strItemCode = jsonResult.ItemCode;
                    }
                }
                else
                {
                    var Product = new Products()
                    {
                        ModelType = 4,
                        ItemCode = strModel,
                        BOMEntityCode = "",
                        ByWeight = false,
                        CommercialRoleCode = 0,
                        CustomsProductGroupCode = 0,
                        CustomsTariffNumberCode = "",
                        Descriptions = new List<Descriptions>(GetDescriptions(strStokAdi.Trim())),
                        DoNotLoadSectionTypeCodeAndDescriptionOnLoadItemSection = false,
                        GenerateOpticalDataMatrixCode = false,
                        GenerateSerialNumber = false,
                        GuaranteePeriod = 0,
                        IsBlocked = false,
                        IsPurchaseOrderClosed = false,
                        IsSalesOrderClosed = 0,
                        IsStoreOrderClosed = false,
                        IsSubsequentDeliveryForR = 0,
                        IsSubsequentDeliveryForRI = 0,
                        IsUTSDeclaratedItem = false,
                        ItemAccountGrCode = CheckItemAccountCode(strStokAdi.Trim()),
                        ItemDescription = strStokAdi.Trim(),
                        ItemDimTypeCode = 2,
                        ItemDiscountGrCode = "",
                        ItemPaymentPlanGrCode = "",
                        ItemTaxGrCode = "%10",
                        ItemVendorGrCode = "",
                        LinkedProductProperties = new LinkedProductProperties
                        {
                            ColorCode = "",
                            ContentItemCode = "",
                            ContentItemTypeCode = 1,
                            LinkedProductTypeCode = 0,
                            LotCode = ""
                        },
                        MaxCreditCardInstallmentCount = 12,
                        OrderLeadTime = 0,
                        OriginCountryCode = "",
                        PerceptionOfFashionCode = 0,
                        ProductCollectionGrCode = 0,
                        ProductFrameProperties = new ProductFrameProperties
                        {
                            BaseCurveRadius = 0,
                            BaseMaterialCode = "",
                            BrandCode = "",
                            BridgeWidth = 0,
                            CustomProcessGroupCode = "",
                            FrameShapeTypeCode = "",
                            FrameTypeCode = "",
                            LensHeight = 0,
                            LensWidth = 0,
                            ManufacturerCode = "",
                            OpticalSutCode = "",
                            TempleLength = 0
                        },
                        ProductHierarchyID = 0,
                        ProductLensProperties = new ProductLensProperties
                        {
                            BaseCurveRadius = 0,
                            BaseMaterialCode = "",
                            BrandCode = "",
                            CoatingTypeCode = "",
                            CustomProcessGroupCode = "",
                            Cylinder = 0,
                            Diameter = 0,
                            DisposeFrequency = 0,
                            EyeGlassSutTypeCode = 0,
                            FocalTypeCode = "",
                            GlassIndex = 0,
                            LensTypeCode = 0,
                            ManufacturerCode = "",
                            OpticalGroupRangeCode = "",
                            Sphere = 0,
                            WaterContent = 0
                        },
                        ProductTypeCode = 1,
                        PromotionGroupCode = "",
                        PromotionGroupCode2 = "",
                        ShelfLife = 0,
                        StoreCapacityLevelCode = "",
                        StorePriceLevelCode = 0,
                        SupplyPeriod = 0,
                        UnitConvertRate = 0,
                        UnitConvertRateNotFixed = false,
                        UnitOfMeasureCode1 = "AD",
                        UseBatch = false,
                        UseInternet = false,
                        UseManufacturing = false,
                        UsePOS = false,
                        UseRoll = false,
                        UseSerialNumber = false,
                        UseStore = false,
                        Variants = new List<Variant>(GetVariant(strColorCode, strModels, strOldModel, orderProduct))
                    };

                    var client = new RestClient(strUrl);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var body = JsonConvert.SerializeObject(Product);
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    dynamic jsonResponseText = response.Content;
                    RefreshTokenResultJSON jsonResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(jsonResponseText, typeof(RefreshTokenResultJSON));
                    if (string.IsNullOrEmpty(jsonResult.ItemCode))
                    {
                        throw new Exception(jsonResult.ExceptionMessage.ToString());
                    }
                    else
                    {
                        strMessage = jsonResult.ItemCode;
                        strItemCode = jsonResult.ItemCode;
                    }
                }
            }
            catch (Exception ex)
            {
                strMessage = "İşlem Sırasında Beklenmedik Bir Hata İle Karşılaşıldı. Detay:" + ex.Message.ToString();
            }

            return strMessage;
        }
        public static string PostProductPenti(string strModel, string strModels, string strOldModel, string strStokAdi, List<ShipmentLine> orderProduct, string strSessionID, string strIP, int dType, int dBirimType, out string strMessage)
        {
            strMessage = "";
            string strUrl = "http://" + strIP + "/(S(" + strSessionID + "))/IntegratorService/Post?";
            try
            {
                string strBirim = dBirimType == 1 ? "PKT" : "AD";

                if (dType == 1)
                {
                    var Product = new Products()
                    {
                        ModelType = 4,
                        ItemCode = strModel,
                        BOMEntityCode = "",
                        ByWeight = false,
                        CommercialRoleCode = 0,
                        CustomsProductGroupCode = 0,
                        CustomsTariffNumberCode = "",
                        Descriptions = new List<Descriptions>(GetDescriptions(strStokAdi.Trim())),
                        DoNotLoadSectionTypeCodeAndDescriptionOnLoadItemSection = false,
                        GenerateOpticalDataMatrixCode = false,
                        GenerateSerialNumber = false,
                        GuaranteePeriod = 0,
                        IsBlocked = false,
                        IsPurchaseOrderClosed = false,
                        IsSalesOrderClosed = 0,
                        IsStoreOrderClosed = false,
                        IsSubsequentDeliveryForR = 0,
                        IsSubsequentDeliveryForRI = 0,
                        IsUTSDeclaratedItem = false,
                        ItemAccountGrCode = CheckItemAccountCode(strStokAdi.Trim()),
                        ItemDescription = "",
                        ItemDimTypeCode = 0,
                        ItemDiscountGrCode = "",
                        ItemPaymentPlanGrCode = "",
                        ItemTaxGrCode = "%10",
                        ItemVendorGrCode = "",
                        LinkedProductProperties = new LinkedProductProperties
                        {
                            ColorCode = "",
                            ContentItemCode = "",
                            ContentItemTypeCode = 1,
                            LinkedProductTypeCode = 0,
                            LotCode = ""
                        },
                        MaxCreditCardInstallmentCount = 12,
                        OrderLeadTime = 0,
                        OriginCountryCode = "",
                        PerceptionOfFashionCode = 0,
                        ProductCollectionGrCode = 0,
                        ProductFrameProperties = new ProductFrameProperties
                        {
                            BaseCurveRadius = 0,
                            BaseMaterialCode = "",
                            BrandCode = "",
                            BridgeWidth = 0,
                            CustomProcessGroupCode = "",
                            FrameShapeTypeCode = "",
                            FrameTypeCode = "",
                            LensHeight = 0,
                            LensWidth = 0,
                            ManufacturerCode = "",
                            OpticalSutCode = "",
                            TempleLength = 0
                        },
                        ProductHierarchyID = 0,
                        ProductLensProperties = new ProductLensProperties
                        {
                            BaseCurveRadius = 0,
                            BaseMaterialCode = "",
                            BrandCode = "",
                            CoatingTypeCode = "",
                            CustomProcessGroupCode = "",
                            Cylinder = 0,
                            Diameter = 0,
                            DisposeFrequency = 0,
                            EyeGlassSutTypeCode = 0,
                            FocalTypeCode = "",
                            GlassIndex = 0,
                            LensTypeCode = 0,
                            ManufacturerCode = "",
                            OpticalGroupRangeCode = "",
                            Sphere = 0,
                            WaterContent = 0
                        },
                        ProductTypeCode = 1,
                        PromotionGroupCode = "",
                        PromotionGroupCode2 = "",
                        ShelfLife = 0,
                        StoreCapacityLevelCode = "",
                        StorePriceLevelCode = 0,
                        SupplyPeriod = 0,
                        UnitConvertRate = 0,
                        UnitConvertRateNotFixed = false,
                        UnitOfMeasureCode1 = strBirim,
                        UseBatch = false,
                        UseInternet = false,
                        UseManufacturing = false,
                        UsePOS = false,
                        UseRoll = false,
                        UseSerialNumber = false,
                        UseStore = false,
                        Variants = new List<Variant>(GetVariantPenti(strModels, strOldModel, orderProduct))
                    };

                    var client = new RestClient(strUrl);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var body = JsonConvert.SerializeObject(Product);
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    dynamic jsonResponseText = response.Content;
                    RefreshTokenResultJSON jsonResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(jsonResponseText, typeof(RefreshTokenResultJSON));
                    if (string.IsNullOrEmpty(jsonResult.ItemCode))
                    {
                        throw new Exception(jsonResult.ExceptionMessage.ToString());
                    }
                    else
                    {
                        strMessage = jsonResult.ItemCode;
                        strItemCode = jsonResult.ItemCode;
                    }
                }
                else
                {
                    var Product = new Products()
                    {
                        ModelType = 4,
                        ItemCode = strModel,
                        BOMEntityCode = "",
                        ByWeight = false,
                        CommercialRoleCode = 0,
                        CustomsProductGroupCode = 0,
                        CustomsTariffNumberCode = "",
                        Descriptions = new List<Descriptions>(GetDescriptions(strStokAdi.Trim())),
                        DoNotLoadSectionTypeCodeAndDescriptionOnLoadItemSection = false,
                        GenerateOpticalDataMatrixCode = false,
                        GenerateSerialNumber = false,
                        GuaranteePeriod = 0,
                        IsBlocked = false,
                        IsPurchaseOrderClosed = false,
                        IsSalesOrderClosed = 0,
                        IsStoreOrderClosed = false,
                        IsSubsequentDeliveryForR = 0,
                        IsSubsequentDeliveryForRI = 0,
                        IsUTSDeclaratedItem = false,
                        ItemAccountGrCode = CheckItemAccountCode(strStokAdi.Trim()),
                        ItemDescription = strStokAdi.Trim(),
                        ItemDimTypeCode = 0,
                        ItemDiscountGrCode = "",
                        ItemPaymentPlanGrCode = "",
                        ItemTaxGrCode = "%10",
                        ItemVendorGrCode = "",
                        LinkedProductProperties = new LinkedProductProperties
                        {
                            ColorCode = "",
                            ContentItemCode = "",
                            ContentItemTypeCode = 1,
                            LinkedProductTypeCode = 0,
                            LotCode = ""
                        },
                        MaxCreditCardInstallmentCount = 12,
                        OrderLeadTime = 0,
                        OriginCountryCode = "",
                        PerceptionOfFashionCode = 0,
                        ProductCollectionGrCode = 0,
                        ProductFrameProperties = new ProductFrameProperties
                        {
                            BaseCurveRadius = 0,
                            BaseMaterialCode = "",
                            BrandCode = "",
                            BridgeWidth = 0,
                            CustomProcessGroupCode = "",
                            FrameShapeTypeCode = "",
                            FrameTypeCode = "",
                            LensHeight = 0,
                            LensWidth = 0,
                            ManufacturerCode = "",
                            OpticalSutCode = "",
                            TempleLength = 0
                        },
                        ProductHierarchyID = 0,
                        ProductLensProperties = new ProductLensProperties
                        {
                            BaseCurveRadius = 0,
                            BaseMaterialCode = "",
                            BrandCode = "",
                            CoatingTypeCode = "",
                            CustomProcessGroupCode = "",
                            Cylinder = 0,
                            Diameter = 0,
                            DisposeFrequency = 0,
                            EyeGlassSutTypeCode = 0,
                            FocalTypeCode = "",
                            GlassIndex = 0,
                            LensTypeCode = 0,
                            ManufacturerCode = "",
                            OpticalGroupRangeCode = "",
                            Sphere = 0,
                            WaterContent = 0
                        },
                        ProductTypeCode = 1,
                        PromotionGroupCode = "",
                        PromotionGroupCode2 = "",
                        ShelfLife = 0,
                        StoreCapacityLevelCode = "",
                        StorePriceLevelCode = 0,
                        SupplyPeriod = 0,
                        UnitConvertRate = 0,
                        UnitConvertRateNotFixed = false,
                        UnitOfMeasureCode1 = strBirim,
                        UseBatch = false,
                        UseInternet = false,
                        UseManufacturing = false,
                        UsePOS = false,
                        UseRoll = false,
                        UseSerialNumber = false,
                        UseStore = false,
                        Variants = new List<Variant>(GetVariantPenti(strModels, strOldModel, orderProduct))
                    };

                    var client = new RestClient(strUrl);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var body = JsonConvert.SerializeObject(Product);
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    dynamic jsonResponseText = response.Content;
                    RefreshTokenResultJSON jsonResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(jsonResponseText, typeof(RefreshTokenResultJSON));
                    if (string.IsNullOrEmpty(jsonResult.ItemCode))
                    {
                        throw new Exception(jsonResult.ExceptionMessage.ToString());
                    }
                    else
                    {
                        strMessage = jsonResult.ItemCode;
                        strItemCode = jsonResult.ItemCode;
                    }
                }
            }
            catch (Exception ex)
            {
                strMessage = "İşlem Sırasında Beklenmedik Bir Hata İle Karşılaşıldı. Detay:" + ex.Message.ToString();
            }

            return strMessage;
        }
        public static string CheckItemAccountCode(string strDesc)
        {
            string strReturn = "";

            List<ItemAccountCodes> ItemAccountCode = new List<ItemAccountCodes>();
            List<ItemAccountCodes> ItemAccountCodeCocuk = new List<ItemAccountCodes>();

            ItemAccountCode.Add(new ItemAccountCodes() { CODE = "BLUZ", DESC = "Bluz" });
            ItemAccountCode.Add(new ItemAccountCodes() { CODE = "ELBİSE", DESC = "Elbise" });
            ItemAccountCode.Add(new ItemAccountCodes() { CODE = "PANTOLON", DESC = "Pantolon" });
            ItemAccountCode.Add(new ItemAccountCodes() { CODE = "CEKET", DESC = "Ceket" });
            ItemAccountCode.Add(new ItemAccountCodes() { CODE = "KABAN", DESC = "Palto" });
            ItemAccountCode.Add(new ItemAccountCodes() { CODE = "KABAN", DESC = "Kaban" });
            ItemAccountCode.Add(new ItemAccountCodes() { CODE = "ŞORTT", DESC = "Şort" });
            ItemAccountCode.Add(new ItemAccountCodes() { CODE = "TULUM", DESC = "Tulum" });
            ItemAccountCode.Add(new ItemAccountCodes() { CODE = "GÖMLEK", DESC = "Gömlek" });
            ItemAccountCode.Add(new ItemAccountCodes() { CODE = "ETEK", DESC = "Etek" });

            ItemAccountCodeCocuk.Add(new ItemAccountCodes() { CODE = "Ç-BLUZ", DESC = "Çocuk Bluz" });
            ItemAccountCodeCocuk.Add(new ItemAccountCodes() { CODE = "Ç-BLUZ", DESC = "Çocuk Gömlek" });
            ItemAccountCodeCocuk.Add(new ItemAccountCodes() { CODE = "Ç-ETEK", DESC = "Çocuk Etek" });
            ItemAccountCodeCocuk.Add(new ItemAccountCodes() { CODE = "Ç-ELBİSE", DESC = "Çocuk Elbise" });
            ItemAccountCodeCocuk.Add(new ItemAccountCodes() { CODE = "Ç-ELBİSE", DESC = "Çocuk Çocuk Elbise" });
            ItemAccountCodeCocuk.Add(new ItemAccountCodes() { CODE = "Ç-TULUM", DESC = "Çocuk Tulum" });
            ItemAccountCodeCocuk.Add(new ItemAccountCodes() { CODE = "Ç-PANTOLON", DESC = "Çocuk Pantolon" });
            ItemAccountCodeCocuk.Add(new ItemAccountCodes() { CODE = "Ç-ŞORT", DESC = "Çocuk Şort" });

            if (strDesc.Contains("Çocuk") || strDesc.Contains("ÇOCUK"))
            {
                foreach (var item in ItemAccountCodeCocuk)
                {
                    if (item.DESC.ToUpper() == strDesc.ToUpper())
                    {
                        strReturn = item.CODE.ToString();
                    }
                }
            }
            else
            {
                string[] strParcala = strDesc.Split(' ');

                for (int i = 0; i < strParcala.Length; i++)
                {
                    foreach (var item in ItemAccountCode)
                    {
                        if (item.DESC.ToUpper().Contains(strParcala[i]))
                        {
                            strReturn = item.CODE.ToString();
                        }
                    }
                }
            }
            return strReturn;
        }
        public static List<Lines> GetUrunlerPenti(List<ShipmentLine> orderProduct)
        {
            List<Lines> Urunlerimiz = new List<Lines>();

            CultureInfo culture = new CultureInfo("en-US", true);
            int dSay = 0;
            string strDovizKodu = "";

            foreach (ShipmentLine item in orderProduct)
            {
                item.FIYAT = string.IsNullOrEmpty(item.FIYAT) ? "0;0-0" : item.FIYAT;

                string strFiyat = item.FIYAT.Replace("(", "").Replace(")", "").ToString().Trim();
                string[] strFiyatBedenParcala = strFiyat.Split(';');
                string[] strFiyatimizx = strFiyatBedenParcala[1].Split('#');

                DataTable dtUrunler = GetNebimItems(item.STOK_KODU, item.NEBIMSTOK);
                string strMiktar = item.KALEM_ACIKLAMA.Split('#')[1].Split(':')[1].ToString();
                decimal flMiktar = item.MIKTAR / Convert.ToInt32(strMiktar);

                foreach (DataRow items in dtUrunler.Rows)
                {
                    string[] strFiyatimiz = strFiyatimizx[0].Split(':');
                    strDovizKodu = strFiyatimiz[2].Trim().ToString() == "TL" ? "TRY" : strFiyatimiz[2].Trim().ToString();

                    Urunlerimiz.Add(new Lines(dSay.ToString(), items["ItemTypeCode"].ToString(), items["ItemCode"].ToString(), "",
                                       "", "", "", Convert.ToString(Convert.ToInt32(flMiktar)), "", item.KALEM_ACIKLAMA, "", "",
                                       strDovizKodu, strFiyatimiz[1].Trim().ToString().Replace('.', ','), "", ""));
                }

                dSay++;
            }

            return Urunlerimiz;
        }
        public static List<Lines> GetUrunler(List<ShipmentLine> orderProduct)
        {
            List<Lines> Urunlerimiz = new List<Lines>();

            CultureInfo culture = new CultureInfo("en-US", true);
            int dSay = 0;
            string strDovizKodu = "";

            foreach (ShipmentLine item in orderProduct)
            {
                string strCheckFiyat = item.FIYAT.ToString() as string;

                if (string.IsNullOrEmpty(strCheckFiyat))
                {
                    item.FIYAT = "0";

                    string strFiyat = item.FIYAT.Replace("(", "").Replace(")", "").ToString().Trim();
                    string[] strFiyatBedenParcala = strFiyat.Split(';');

                    string[] strFiyatimizx = strFiyatBedenParcala[0].Split('#');

                    DataTable dtUrunler = GetNebimItems(item.STOK_KODU, item.NEBIMSTOK);
                    string strColorCode = GetNebimColorCode(item.RENK);

                    foreach (DataRow items in dtUrunler.Rows)
                    {
                        DataTable dtPrItemVariant = GetNebimItemsVariant(items["ItemCode"].ToString());
                        int dColorCheck = dtPrItemVariant.Select("ColorCode <> ''").Count();

                        if (dColorCheck > 0)
                        {
                            string[] strItemDimCodes = item.BEDEN.ToString().Split('(');

                            for (int i = 0; i < strItemDimCodes.Length; i++)
                            {
                                if (strItemDimCodes[i].Contains("Beden"))
                                {
                                    string[] strP1 = strItemDimCodes[i].ToString().Replace(" ", "").Trim().Split(';');

                                    for (int a = 1; a < strP1.Length; a++)
                                    {
                                        string[] strBedParcala = strP1[1].ToString().Replace(")", "").Split('#');

                                        for (int c = 0; c < strBedParcala.Length; c++)
                                        {
                                            string strBeden = strBedParcala[c].Split(':')[0].ToString().Trim();
                                            string strAdet = strBedParcala[c].Split(':')[1].ToString().Trim();
                                            string strPrice = "";

                                            if (strFiyatimizx.Length == strBedParcala.Length)
                                            {
                                                string[] strFiyatimiz = strFiyatimizx[c].Split(':');

                                                if (strFiyatimiz[0].ToString().Replace(" ", "").Trim() == strBeden.Replace(" ", "").Trim())
                                                {
                                                    strPrice = strFiyatimiz[1].Trim().ToString().Replace('.', ',');
                                                    strDovizKodu = strFiyatimiz[2].Trim().ToString() == "TL" ? "TRY" : strFiyatimiz[2].Trim().ToString();
                                                }
                                            }
                                            else
                                            {
                                                strPrice = "0";
                                            }

                                            Urunlerimiz.Add(new Lines(dSay.ToString(), items["ItemTypeCode"].ToString(), items["ItemCode"].ToString(), strColorCode,
                                                strBeden, "", "", Convert.ToString(Convert.ToInt32(strAdet)), "", item.KALEM_ACIKLAMA, "", "",
                                                strDovizKodu, strPrice.ToString(), "", ""));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (strFiyatimizx.Length >= 3)
                            {
                                string[] strFiyatimiz = strFiyatimizx[0].Split(':');
                                strDovizKodu = strFiyatimiz[2].Trim().ToString() == "TL" ? "TRY" : strFiyatimiz[2].Trim().ToString();

                                Urunlerimiz.Add(new Lines(dSay.ToString(), items["ItemTypeCode"].ToString(), items["ItemCode"].ToString(), "",
                                                   "", "", "", Convert.ToString(Convert.ToInt32(item.MIKTAR)), "", item.KALEM_ACIKLAMA, "", "",
                                                   strDovizKodu, strFiyatimiz[1].Trim().ToString().Replace('.', ','), "", ""));
                            }
                            else
                            {
                                Urunlerimiz.Add(new Lines(dSay.ToString(), items["ItemTypeCode"].ToString(), items["ItemCode"].ToString(), "",
                                                 "", "", "", Convert.ToString(Convert.ToInt32(item.MIKTAR)), "", item.KALEM_ACIKLAMA, "", "",
                                                 "TRY", "0", "", ""));
                            }
                        }
                    }
                }
                else
                {
                    //item.FIYAT = item.FIYAT.Trim().Replace('-', ':').Replace(',', '#');
                    item.FIYAT = string.IsNullOrEmpty(item.FIYAT) ? "0;0-0" : item.FIYAT;

                    string strFiyat = item.FIYAT.Replace("(", "").Replace(")", "").ToString().Trim();
                    string[] strFiyatBedenParcala = strFiyat.Split(';');
                    string[] strFiyatimizx = strFiyatBedenParcala[1].Split('#');

                    DataTable dtUrunler = GetNebimItems(item.STOK_KODU, item.NEBIMSTOK);
                    string strColorCode = GetNebimColorCode(item.RENK);

                    foreach (DataRow items in dtUrunler.Rows)
                    {
                        DataTable dtPrItemVariant = GetNebimItemsVariant(items["ItemCode"].ToString());
                        int dColorCheck = dtPrItemVariant.Select("ColorCode <> ''").Count();

                        if (dColorCheck > 0)
                        {
                            string[] strItemDimCodes = item.BEDEN.ToString().Split('(');

                            for (int i = 0; i < strItemDimCodes.Length; i++)
                            {
                                if (strItemDimCodes[i].Contains("Beden"))
                                {
                                    string[] strP1 = strItemDimCodes[i].ToString().Replace(" ", "").Trim().Split(';');

                                    for (int a = 1; a < strP1.Length; a++)
                                    {
                                        string[] strBedParcala = strP1[1].ToString().Replace(")", "").Split('#');

                                        for (int c = 0; c < strBedParcala.Length; c++)
                                        {
                                            string strBeden = strBedParcala[c].Split(':')[0].ToString().Trim();
                                            string strAdet = strBedParcala[c].Split(':')[1].ToString().Trim();
                                            string strPrice = "";

                                            if (strFiyatimizx.Length == strBedParcala.Length)
                                            {
                                                string[] strFiyatimiz = strFiyatimizx[c].Split(':');

                                                if (strFiyatimiz[0].ToString().Replace(" ", "").Trim() == strBeden.Replace(" ", "").Trim())
                                                {
                                                    strPrice = strFiyatimiz[1].Trim().ToString().Replace('.', ',');
                                                    strDovizKodu = strFiyatimiz[2].Trim().ToString() == "TL" ? "TRY" : strFiyatimiz[2].Trim().ToString();
                                                }
                                            }
                                            else
                                            {
                                                strPrice = "0";
                                            }

                                            Urunlerimiz.Add(new Lines(dSay.ToString(), items["ItemTypeCode"].ToString(), items["ItemCode"].ToString(), strColorCode,
                                                strBeden, "", "", Convert.ToString(Convert.ToInt32(strAdet)), "", item.KALEM_ACIKLAMA, "", "",
                                                strDovizKodu, strPrice.ToString(), "", ""));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            string[] strFiyatimiz = strFiyatimizx[0].Split(':');
                            strDovizKodu = strFiyatimiz[2].Trim().ToString() == "TL" ? "TRY" : strFiyatimiz[2].Trim().ToString();

                            Urunlerimiz.Add(new Lines(dSay.ToString(), items["ItemTypeCode"].ToString(), items["ItemCode"].ToString(), "",
                                               "", "", "", Convert.ToString(Convert.ToInt32(item.MIKTAR)), "", item.KALEM_ACIKLAMA, "", "",
                                               strDovizKodu, strFiyatimiz[1].Trim().ToString().Replace('.', ','), "", ""));
                        }
                    }
                }

                dSay++;
            }

            return Urunlerimiz;
        }
        public static List<Lines2> GetUrunler2(List<ShipmentLine> orderProduct)
        {
            List<Lines2> Urunlerimiz = new List<Lines2>();

            CultureInfo culture = new CultureInfo("en-US", true);
            int dSay = 0;
            string strDovizKodu = "";

            foreach (ShipmentLine item in orderProduct)
            {
                string strCheckFiyat = item.FIYAT.ToString() as string;

                if (string.IsNullOrEmpty(strCheckFiyat))
                {
                    item.FIYAT = "0";

                    string strFiyat = item.FIYAT.Replace("(", "").Replace(")", "").ToString().Trim();
                    string[] strFiyatBedenParcala = strFiyat.Split(';');

                    string[] strFiyatimizx = strFiyatBedenParcala[0].Split(',');

                    DataTable dtUrunler = GetNebimItems(item.STOK_KODU, item.NEBIMSTOK);
                    string strColorCode = GetNebimColorCode(item.RENK);

                    foreach (DataRow items in dtUrunler.Rows)
                    {
                        DataTable dtPrItemVariant = GetNebimItemsVariant(items["ItemCode"].ToString());
                        int dColorCheck = dtPrItemVariant.Select("ColorCode <> ''").Count();

                        if (dColorCheck > 0)
                        {
                            string[] strItemDimCodes = item.BEDEN.ToString().Split('(');

                            for (int i = 0; i < strItemDimCodes.Length; i++)
                            {
                                if (strItemDimCodes[i].Contains("Beden"))
                                {
                                    string[] strP1 = strItemDimCodes[i].ToString().Replace(" ", "").Trim().Split(';');

                                    for (int a = 1; a < strP1.Length; a++)
                                    {
                                        string[] strBedParcala = strP1[1].ToString().Replace(")", "").Split('#');

                                        for (int c = 0; c < strBedParcala.Length; c++)
                                        {
                                            string strBeden = strBedParcala[c].Split(':')[0].ToString().Trim();
                                            string strAdet = strBedParcala[c].Split(':')[1].ToString().Trim();
                                            string strPrice = "";

                                            if (strFiyatimizx.Length == strBedParcala.Length)
                                            {
                                                string[] strFiyatimiz = strFiyatimizx[c].Split(':');

                                                if (strFiyatimiz[0].ToString().Replace(" ", "").Trim() == strBeden.Replace(" ", "").Trim())
                                                {
                                                    strPrice = strFiyatimiz[1].Trim().ToString().Replace('.', ',');
                                                    strDovizKodu = strFiyatimiz[2].Trim().ToString() == "TL" ? "TRY" : strFiyatimiz[2].Trim().ToString();
                                                }
                                            }
                                            else
                                            {
                                                strPrice = "0";
                                            }

                                            Urunlerimiz.Add(new Lines2(true, "", strColorCode, "", false, false, "", "", false, "", "", "", "", "", items["ItemCode"].ToString(), strBeden, "", "",
                                                Convert.ToInt32(items["ItemTypeCode"].ToString()), item.KALEM_ACIKLAMA, "", "", "", Convert.ToInt32(strPrice.Trim()), strDovizKodu, "", "", Convert.ToInt32(strAdet),
                                                0, "", "", 0, 0, 0, dSay, "", "", ""));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (strFiyatimizx.Length >= 3)
                            {
                                string[] strFiyatimiz = strFiyatimizx[0].Split(':');
                                strDovizKodu = strFiyatimiz[2].Trim().ToString() == "TL" ? "TRY" : strFiyatimiz[2].Trim().ToString();

                                Urunlerimiz.Add(new Lines2(true, "", "", "", false, false, "", "", false, "", "", "", "", "", items["ItemCode"].ToString(),
                                    "", "", "", Convert.ToInt32(items["ItemTypeCode"].ToString()), item.KALEM_ACIKLAMA, "", "", "",
                                    Convert.ToInt32(strFiyatimiz[1].Trim().ToString().Replace('.', ',')), strDovizKodu, "", "", Convert.ToInt32(item.MIKTAR), 0, "", "", 0, 0, 0, dSay, "", "", ""));
                            }
                            else
                            {
                                Urunlerimiz.Add(new Lines2(true, "", "", "", false, false, "", "", false, "", "", "", "", "", items["ItemCode"].ToString(),
                                    "", "", "", Convert.ToInt32(items["ItemTypeCode"].ToString()), item.KALEM_ACIKLAMA, "", "", "",
                                    0, "TRY", "", "", Convert.ToInt32(item.MIKTAR), 0, "", "", 0, 0, 0, dSay, "", "", ""));
                            }
                        }
                    }
                    dSay++;
                }
                else
                {
                    //item.FIYAT = item.FIYAT.Trim().Replace('-', ':').Replace(',', '/');
                    item.FIYAT = string.IsNullOrEmpty(item.FIYAT) ? "0;0-0" : item.FIYAT;

                    string strFiyat = item.FIYAT.Replace("(", "").Replace(")", "").ToString().Trim();
                    string[] strFiyatBedenParcala = strFiyat.Split(';');
                    string[] strFiyatimizx = strFiyatBedenParcala[1].Split(',');

                    DataTable dtUrunler = GetNebimItems(item.STOK_KODU, item.NEBIMSTOK);
                    string strColorCode = GetNebimColorCode(item.RENK);

                    foreach (DataRow items in dtUrunler.Rows)
                    {
                        DataTable dtPrItemVariant = GetNebimItemsVariant(items["ItemCode"].ToString());
                        int dColorCheck = dtPrItemVariant.Select("ColorCode <> ''").Count();

                        if (dColorCheck > 0)
                        {
                            string[] strItemDimCodes = item.BEDEN.ToString().Split('(');

                            for (int i = 0; i < strItemDimCodes.Length; i++)
                            {
                                if (strItemDimCodes[i].Contains("Beden"))
                                {
                                    string[] strP1 = strItemDimCodes[i].ToString().Replace(" ", "").Trim().Split(';');

                                    for (int a = 1; a < strP1.Length; a++)
                                    {
                                        string[] strBedParcala = strP1[1].ToString().Replace(")", "").Split('#');

                                        for (int c = 0; c < strBedParcala.Length; c++)
                                        {
                                            string strBeden = strBedParcala[c].Split(':')[0].ToString().Trim();
                                            string strAdet = strBedParcala[c].Split(':')[1].ToString().Trim();
                                            string strPrice = "";

                                            if (strFiyatimizx.Length == strBedParcala.Length)
                                            {
                                                string[] strFiyatimiz = strFiyatimizx[c].Split(':');

                                                if (strFiyatimiz[0].ToString().Replace(" ", "").Trim() == strBeden.Replace(" ", "").Trim())
                                                {
                                                    strPrice = strFiyatimiz[1].Trim().ToString().Replace('.', ',');
                                                    strDovizKodu = strFiyatimiz[2].Trim().ToString() == "TL" ? "TRY" : strFiyatimiz[2].Trim().ToString();
                                                }
                                            }
                                            else
                                            {
                                                strPrice = "0";
                                            }

                                            Urunlerimiz.Add(new Lines2(true, "", strColorCode, "", false, false, "", "", false, "", "", "", "", "", items["ItemCode"].ToString(), strBeden, "", "",
                                                Convert.ToInt32(items["ItemTypeCode"].ToString()), item.KALEM_ACIKLAMA, "", "", "", Convert.ToInt32(strPrice.Trim()), strDovizKodu, "", "", Convert.ToInt32(strAdet),
                                                0, "", "", 0, 0, 0, dSay, "", "", ""));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            string[] strFiyatimiz = strFiyatimizx[0].Split(':');
                            strDovizKodu = strFiyatimiz[2].Trim().ToString() == "TL" ? "TRY" : strFiyatimiz[2].Trim().ToString();

                            Urunlerimiz.Add(new Lines2(true, "", "", "", false, false, "", "", false, "", "", "", "", "", items["ItemCode"].ToString(),
                                "", "", "", Convert.ToInt32(items["ItemTypeCode"].ToString()), item.KALEM_ACIKLAMA, "", "", "",
                                Convert.ToInt32(strFiyatimiz[1].Trim().ToString().Replace('.', ',')), strDovizKodu, "", "", Convert.ToInt32(item.MIKTAR), 0, "", "", 0, 0, 0, dSay, "", "", ""));
                        }
                    }
                    dSay++;
                }
            }

            return Urunlerimiz;
        }
        public static string GetDescription(string strIrsNo, string strCurrAccCode, string strSatisTemsilcisi)
        {
            string strReturn = "";
            strReturn = "İRSALİYE NO: " + strIrsNo + "" + Environment.NewLine;
            strReturn += "BU FATURADA BELİRTİLEN TÜM ÜRÜNLER, ÖDEME TAM OLARAK GERÇEKLEŞENE KADAR ÖZ TEKSTİL'İN MÜLKİYETİNDEDİR.";
            if (strCurrAccCode == "12010150" || strCurrAccCode == "320100047")
            {
                if(!string.IsNullOrEmpty(strSatisTemsilcisi))
                {
                    strReturn += Environment.NewLine + strSatisTemsilcisi;
                }
            }
            return strReturn;
        }
        public static List<Variant> GetVariant(string strColorCode, string strModel, string strOldModel, List<ShipmentLine> orderProduct)
        {
            List<Variant> Varyantlar = new List<Variant>();

            int dDurumSay = orderProduct.Where(x => x.MODEL == strModel || x.STOK_KODU == strModel || x.NEBIMSTOK == strModel).Count();

            string strModels = dDurumSay > 0 ? strModel : strOldModel;

            foreach (ShipmentLine item in orderProduct.Where(x=>x.MODEL == strModels || x.STOK_KODU == strModels || x.NEBIMSTOK == strModels))
            {
                string[] strItemDimCodes = item.BEDEN.Trim().ToString().Split('(');

                if (!string.IsNullOrEmpty(strColorCode))
                {
                    for (int i = 0; i < strItemDimCodes.Length; i++)
                    {
                        if (strItemDimCodes[i].Contains("Beden"))
                        {
                            string[] strP1 = strItemDimCodes[i].Trim().ToString().Split(';');

                            for (int a = 1; a < strP1.Length; a++)
                            {
                                string[] strBedParcala = strP1[1].Trim().ToString().Replace(")", "").Split('#');

                                for (int c = 0; c < strBedParcala.Length; c++)
                                {
                                    string strBeden = strBedParcala[c].Split(':')[0].ToString().Replace(" ","").Trim();
                                    int dItemDim1CodeCheck = 0;
                                    string strQuery = "SELECT COUNT(*) AS DURUM FROM dbo.cdItemDim1 WHERE ItemDim1Code = '{0}' AND IsBlocked = 0";
                                    strQuery = string.Format(strQuery, strBeden);

                                    using (SqlConnection cnn = new SqlConnection(MSSQLConnectionString()))
                                    {
                                        cnn.Open();
                                        dItemDim1CodeCheck = cnn.ExecuteScalar<int>(strQuery, null, null, 120, null);
                                        cnn.Close();
                                    }

                                    if (dItemDim1CodeCheck == 0)
                                    {
                                        ItemDim1Create(strBeden);
                                    }

                                    string strSorgu = @"SELECT ItemDim1Code FROM dbo.ItemDim1('TR') WHERE ItemDim1Code = '{0}' AND IsBlocked = 0";
                                    strSorgu = string.Format(strSorgu, strBeden);

                                    DataTable dtBedenler = new DataTable();

                                    using (SqlConnection cnn = new SqlConnection(MSSQLConnectionString()))
                                    {
                                        dtBedenler.Load(cnn.ExecuteReader(strSorgu, null, null, 120, null));
                                    }

                                    foreach (DataRow items in dtBedenler.Rows)
                                    {
                                        Varyantlar.Add(new Variant(strColorCode, false, false, false, 0, false, items["ItemDim1Code"].ToString(), "", "", false));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return Varyantlar;
        }
        public static List<Variant> GetVariantPenti(string strModel, string strOldModel, List<ShipmentLine> orderProduct)
        {
            List<Variant> Varyantlar = new List<Variant>();

            int dDurumSay = orderProduct.Where(x => x.MODEL == strModel || x.STOK_KODU == strModel || x.NEBIMSTOK == strModel).Count();

            string strModels = dDurumSay > 0 ? strModel : strOldModel;

            foreach (ShipmentLine item in orderProduct.Where(x => x.MODEL == strModels || x.STOK_KODU == strModels || x.NEBIMSTOK == strModels))
            {
                Varyantlar.Add(new Variant("", false, false, false, 0, false, "", "", "", false));
            }

            return Varyantlar;
        }
        private static string WhereItemDim(string strItemDim)
        {
            string strReturn = "";

            if (strItemDim == "30" || strItemDim== "32" || strItemDim== "34" || strItemDim == "36" || strItemDim == "38" || strItemDim == "40" || strItemDim == "42" || strItemDim == "44" || strItemDim == "46"
                 || strItemDim == "48" || strItemDim == "50" || strItemDim == "52" || strItemDim == "54" || strItemDim == "56" || strItemDim == "58" || strItemDim == "60" || strItemDim == "62")
            {
                strReturn = "ItemDim1Code IN('30','32','34','36','38','40','42','44','46','48','50','52','54','56','58','60','62')";
            }
            else if (strItemDim == "38L" || strItemDim == "38R" || strItemDim == "3XL" || strItemDim == "40L" || strItemDim == "42L" || strItemDim == "44L" || strItemDim == "44R" || strItemDim == "46R"
                || strItemDim == "48R" || strItemDim == "50R" || strItemDim == "40R")
            {
                strReturn = "ItemDim1Code IN('38L','38R','3XL','40L','40R','42L','44L','44R','46R','48R','50R')";
            }
            else if (strItemDim == "5/6" || strItemDim == "6/7" || strItemDim == "7/8" || strItemDim == "8/9" || strItemDim == "9/10" || strItemDim == "10/11" || strItemDim == "11/12" || strItemDim == "12/13" || strItemDim == "13/14")
            {
                strReturn = "ItemDim1Code IN('5/6','6/7','7/8','8/9','9/10','10/11','11/12','12/13','13/14')";
            }
            else if (strItemDim == "L" || strItemDim == "L-XL" || strItemDim == "M" || strItemDim == "M-L" || strItemDim == "S" || strItemDim == "SM" || strItemDim == "S-M" || strItemDim == "STD"
                 || strItemDim == "XL" || strItemDim == "XS" || strItemDim == "XS-S" || strItemDim == "XXL" || strItemDim == "XXS")
            {
                strReturn = "ItemDim1Code IN('L','L-XL','M','M-L','S','SM','S-M','STD','XL','XS','XS-S','XXL','XXS')";
            }
            else if (strItemDim == "0" || strItemDim == "1" || strItemDim == "2" || strItemDim == "3" || strItemDim == "4" || strItemDim == "5" || strItemDim == "6" || strItemDim == "7" || strItemDim == "8"
                 || strItemDim == "9" || strItemDim == "10" || strItemDim == "11" || strItemDim == "12" || strItemDim == "13" || strItemDim == "14" || strItemDim == "15" || strItemDim == "16")
            {
                strReturn = "ItemDim1Code IN('0','1','2','3','4','5','6','7','8','9','10','11','12','13','14','15','16')";
            }
            
            return strReturn;
        }
        private static bool ItemDim1Create(string ItemDim1Code)
        {
            bool blnReturn = false;

            try
            {
                using (SqlConnection SqlCnn = new SqlConnection(Functions.MSSQLConnectionString()))
                {
                    string GuidKey = Guid.NewGuid().ToString();

                    string strSorgu = @"INSERT INTO cdItemDim1(ItemDim1Code, ItemDimType1, ItemDimType2, ItemDimType3, ItemDimType4, ItemDimType5, ItemDimType6, 
                                        ItemDimType7, ItemDimType8, ItemDimType9, ItemDimType10, SortOrder, IsBlocked, 
                                        CreatedUserName, CreatedDate, LastUpdatedUserName, LastUpdatedDate, RowGuid) 
                                        VALUES (@_ItemDim1Code, N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, 0, N'INT', GETDATE(), N'INT', GETDATE(),@_RowGuid)";

                    blnReturn = SqlCnn.ExecuteScalar<int>(strSorgu, new
                    {
                        _ItemDim1Code = ItemDim1Code,
                        _RowGuid = GuidKey

                    }) == 0 ? true : false;
                }
            }
            catch
            {
            }

            return blnReturn;
        }
        public static List<Descriptions> GetDescriptions(string strDescName)
        {
            List<Descriptions> Descriptions = new List<Descriptions>();

            Descriptions.Add(new Descriptions("TR", strDescName));
            Descriptions.Add(new Descriptions("EN", strDescName));

            return Descriptions;
        }
        public static DataTable GetNebimItems(string strStockCode, string strModel)
        {
            DataTable dtReturn = new DataTable();
            string strProductTypeCode = "0", strItemCode = "";
            
            if (string.IsNullOrEmpty(strStockCode))
            {
                strProductTypeCode = "1";
                strItemCode = strModel;
            }
            else
            {
                strProductTypeCode = "0";
                strItemCode = strStockCode;
            }

            string strSorgu = "SELECT * FROM cdItem WHERE ItemCode = '{0}' AND ProductTypeCode = {1}";

            string strQuery = string.Format(strSorgu, strItemCode, strProductTypeCode);

            using (SqlConnection cnn = new SqlConnection(Functions.MSSQLConnectionString()))
            {
                dtReturn.Load(cnn.ExecuteReader(strQuery, null, null, null, null));
            }

            return dtReturn;
        }
        public static DataTable GetNebimItemsVariant(string strItemCode)
        {
            DataTable dtReturn = new DataTable();

            string strSorgu = string.Format("SELECT ColorCode FROM dbo.prItemVariant WHERE ItemCode ='{0}' GROUP BY ColorCode", strItemCode);

            using (SqlConnection cnn = new SqlConnection(Functions.MSSQLConnectionString()))
            {
                dtReturn.Load(cnn.ExecuteReader(strSorgu, null, null, null, null));
            }

            return dtReturn;
        }
        public static string GetNebimColorCode(string strColorName)
        {
            string strReturn = "";

            string strSorgu = string.Format("SELECT ColorCode FROM dbo.Color('TR') WHERE ColorDescription = '{0}'", strColorName);

            using (SqlConnection cnn = new SqlConnection(Functions.MSSQLConnectionString()))
            {
                strReturn = cnn.ExecuteScalar<string>(strSorgu, null, null, null, null);
            }

            return strReturn;
        }
        public static string MSSQLConnectionString()
        {
            string result;
            using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
            {
                AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                result = string.Format("Server={0};Database={1};User Id={2};Password = {3}; ", new object[]
                {
                    appSettings.SQLServer,
                    appSettings.SQLDatabase,
                    appSettings.SQLUser,
                    appSettings.SQLPass
                });
            }
            return result;
        }
        public static string MSSQLConnectionStringEnfotek()
        {
            string result;
            using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
            {
                AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);
                result = string.Format("Server={0};Database={1};User Id={2};Password = {3}; ", new object[]
                {
                    appSettings.EnfoTekSQLServer,
                    appSettings.EnfoTekSQLDatabase,
                    appSettings.EnfoTekSQLUser,
                    appSettings.EnfoTekSQLPass
                });
            }
            return result;
        }
        private static string GetSippmentSqlQuery()
        {
            string result = "";

            using (LiteRepository liteRepository = new LiteRepository(App.GetAppDataFile, null))
            {
                AppSettings appSettings = liteRepository.FirstOrDefault<AppSettings>((AppSettings x) => x.Id == 1, null);

                string strTestIslem = "";

                if(appSettings.IrsaliyeTest =="Evet")
                {
                    strTestIslem = "";
                }
                else
                {
                    strTestIslem = "AND ENTEGRATORE_GONDERILDI = 1";
                }

                result = @"SELECT TOP(400) * FROM(SELECT
                        '' AS AktarimNotu,
                        MH_EIRSALIYEID,
                        UUID,
                        TIP,
                        IRS_NO,
                        TARIH,
                        CUST_CARI_ISIM,
                        CUST_CARI_VERGI_NO,
                        CUST_CARI_TCKIMLIKNO,
                        CUST_CARI_ADRES,
                        SHIP_SEVKTAR,
                        SHIP_CARRIER_ISIM,
                        SHIP_CARRIER_VERGI_DAIRESI,
                        SHIP_CARRIER_VERGI_NO,
                        SHIP_CARRIER_TCKIMLIKNO,
                        SHIP_CARRIER_ADRES,
                        SHIP_CARRIER_IL,
                        SHIP_CARRIER_ILCE,
                        SHIP_CARRIER_POSTA_KODU,
                        SHIP_CARRIER_TEL_NO,
                        SHIP_ARAC_PLAKA_NO,
                        SHIP_DORSE_PLAKA1,
                        SHIP_DRIVER_PERSON1_ADI,
                        SHIP_DRIVER_PERSON1_SOYADI,
                        SHIP_DRIVER_PERSON1_TCKIMLIKNO,
                        HAREKET_TIPI
                        FROM dbo.MH_EIRSALIYE WITH(NOLOCK)
                        WHERE HAREKET_TIPI IN('İade Çıkış','Yükleme 1K','Yükleme 2K') {0}) AS TBL
                        ORDER BY MH_EIRSALIYEID DESC";

                result = string.Format(result, strTestIslem);
            }

            return result;
        }
        private static string GetSippmentSqlQuery2()
        {
            string result = "";

            result = @"
                        SELECT
                        '' AS AktarimNotu,
                        MH_EIRSALIYEID,
                        UUID,
                        TIP,
                        IRS_NO,
                        TARIH,
                        CUST_CARI_ISIM,
                        CUST_CARI_VERGI_NO,
                        CUST_CARI_TCKIMLIKNO,
                        CUST_CARI_ADRES,
                        SHIP_SEVKTAR,
                        SHIP_CARRIER_ISIM,
                        SHIP_CARRIER_VERGI_DAIRESI,
                        SHIP_CARRIER_VERGI_NO,
                        SHIP_CARRIER_TCKIMLIKNO,
                        SHIP_CARRIER_ADRES,
                        SHIP_CARRIER_IL,
                        SHIP_CARRIER_ILCE,
                        SHIP_CARRIER_POSTA_KODU,
                        SHIP_CARRIER_TEL_NO,
                        SHIP_ARAC_PLAKA_NO,
                        SHIP_DORSE_PLAKA1,
                        SHIP_DRIVER_PERSON1_ADI,
                        SHIP_DRIVER_PERSON1_SOYADI,
                        SHIP_DRIVER_PERSON1_TCKIMLIKNO,
                        HAREKET_TIPI
                        FROM dbo.MH_EIRSALIYE WITH(NOLOCK)
                        WHERE MH_EIRSALIYEID = 0 
                        ORDER BY MH_EIRSALIYEID ASC";

            return result;
        }
        private static string GetSippmentSqlQueryProduct(string strSippmentID)
        {
            string result;
            result = string.Format(@"
                                    SELECT
                                    MH_EIRSALIYE_KALEMIID,
                                    MH_EIRSALIYEID,
                                    ISNULL(MODEL,'') AS MODEL,
                                    ISNULL(MODEL,'') AS OLDMODEL,
                                    STOK_KODU,
                                    ISNULL(STOK_ADI,'') AS STOK_ADI,
									ISNULL(VARYANT,'') AS  RENK,
									ISNULL(BEDEN_DETAY,'') AS  BEDEN,
                                    MIKTAR,
                                    ISNULL(FIYATLI_BEDEN_DETAY,'') AS  FIYAT,
                                    DOVIZ_KODU,
                                    BIRIM,
                                    KALEM_ACIKLAMA,
									ISNULL(SIP_TEMSILCI,'') AS  SIP_TEMSILCI,
									ISNULL(MUH_STOK_KODU,'') AS NEBIMSTOK
                                    FROM dbo.MH_EIRSALIYE_KALEM  WITH(NOLOCK)
                                    WHERE MH_EIRSALIYEID IN ({0})", strSippmentID);
            return result;
        }
        private static string GetSippmentSqlList(int dSippmentID)
        {
            string result = "";

            result = string.Format(@"
                        SELECT
                        '' AS AktarimNotu,
                        MH_EIRSALIYEID,
                        UUID,
                        TIP,
                        IRS_NO,
                        TARIH,
                        CUST_CARI_ISIM,
                        CUST_CARI_VERGI_NO,
                        CUST_CARI_TCKIMLIKNO,
                        CUST_CARI_ADRES,
                        SHIP_SEVKTAR,
                        SHIP_CARRIER_ISIM,
                        SHIP_CARRIER_VERGI_DAIRESI,
                        SHIP_CARRIER_VERGI_NO,
                        SHIP_CARRIER_TCKIMLIKNO,
                        SHIP_CARRIER_ADRES,
                        SHIP_CARRIER_IL,
                        SHIP_CARRIER_ILCE,
                        SHIP_CARRIER_POSTA_KODU,
                        SHIP_CARRIER_TEL_NO,
                        SHIP_ARAC_PLAKA_NO,
                        SHIP_DORSE_PLAKA1,
                        SHIP_DRIVER_PERSON1_ADI,
                        SHIP_DRIVER_PERSON1_SOYADI,
                        SHIP_DRIVER_PERSON1_TCKIMLIKNO,
                        HAREKET_TIPI
                        FROM dbo.MH_EIRSALIYE WITH(NOLOCK)
                        WHERE MH_EIRSALIYEID = {0} 
                        ORDER BY MH_EIRSALIYEID ASC", dSippmentID);

            return result;
        }
        private static string GetSippmentSqlQueryInsert(int dSippmentID)
        {
            string result;
            result = string.Format(@"
                                    SELECT
                                    MH_EIRSALIYE_KALEMIID,
                                    MH_EIRSALIYEID,
                                    ISNULL(MODEL,'') AS MODEL,
                                    ISNULL(MODEL,'') AS OLDMODEL,
                                    STOK_KODU,
                                    ISNULL(STOK_ADI,'') AS STOK_ADI,
									ISNULL(VARYANT,'') AS  RENK,
									ISNULL(BEDEN_DETAY,'') AS  BEDEN,
                                    MIKTAR,
                                    ISNULL(FIYATLI_BEDEN_DETAY,'') AS  FIYAT,
                                    DOVIZ_KODU,
                                    BIRIM,
                                    KALEM_ACIKLAMA,
									ISNULL(SIP_TEMSILCI,'') AS  SIP_TEMSILCI,
									ISNULL(MUH_STOK_KODU,'') AS NEBIMSTOK
                                    FROM dbo.MH_EIRSALIYE_KALEM  WITH(NOLOCK)
                                    WHERE MH_EIRSALIYEID = {0}", dSippmentID);
            return result;
        }
        public static IntegrationResult<List<ShipmentHeader>> GetSippment()
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(Functions.MSSQLConnectionStringEnfotek()))
                {
                    List<ShipmentHeader> list = cnn.Query<ShipmentHeader>(GetSippmentSqlQuery(), (object)null, (IDbTransaction)null, true, new int?(), new CommandType?()).ToList<ShipmentHeader>();
                    IntegrationResult<List<ShipmentHeader>> integrationResult = new IntegrationResult<List<ShipmentHeader>>();
                    integrationResult.Data = list;
                    integrationResult.ReturnCode = "OK";
                    return integrationResult;
                }
            }
            catch (SqlException ex)
            {
                IntegrationResult<List<ShipmentHeader>> integrationResult = new IntegrationResult<List<ShipmentHeader>>();
                integrationResult.ReturnCode = "MSSQL-ERR";
                integrationResult.Message = ex.Message;
                return integrationResult;
            }
            catch (Exception ex)
            {
                IntegrationResult<List<ShipmentHeader>> integrationResult = new IntegrationResult<List<ShipmentHeader>>();
                integrationResult.ReturnCode = "APP-ERR";
                integrationResult.Message = ex.Message;
                return integrationResult;
            }
        }
        public static IntegrationResult<List<ShipmentHeader>> GetSippment2()
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(Functions.MSSQLConnectionStringEnfotek()))
                {
                    List<ShipmentHeader> list = cnn.Query<ShipmentHeader>(GetSippmentSqlQuery2(), (object)null, (IDbTransaction)null, true, new int?(), new CommandType?()).ToList<ShipmentHeader>();
                    IntegrationResult<List<ShipmentHeader>> integrationResult = new IntegrationResult<List<ShipmentHeader>>();
                    integrationResult.Data = list;
                    integrationResult.ReturnCode = "OK";
                    return integrationResult;
                }
            }
            catch (SqlException ex)
            {
                IntegrationResult<List<ShipmentHeader>> integrationResult = new IntegrationResult<List<ShipmentHeader>>();
                integrationResult.ReturnCode = "MSSQL-ERR";
                integrationResult.Message = ex.Message;
                return integrationResult;
            }
            catch (Exception ex)
            {
                IntegrationResult<List<ShipmentHeader>> integrationResult = new IntegrationResult<List<ShipmentHeader>>();
                integrationResult.ReturnCode = "APP-ERR";
                integrationResult.Message = ex.Message;
                return integrationResult;
            }
        }
        public static IntegrationResult<List<ShipmentLine>> GetSippmentProduct(string strShipmentID)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(Functions.MSSQLConnectionStringEnfotek()))
                {
                    List<ShipmentLine> list = cnn.Query<ShipmentLine>(Functions.GetSippmentSqlQueryProduct(strShipmentID), (object)null, (IDbTransaction)null, true, new int?(), new CommandType?()).ToList<ShipmentLine>();
                    IntegrationResult<List<ShipmentLine>> integrationResult = new IntegrationResult<List<ShipmentLine>>();
                    integrationResult.Data = list;
                    integrationResult.ReturnCode = "OK";
                    return integrationResult;
                }
            }
            catch (SqlException ex)
            {
                IntegrationResult<List<ShipmentLine>> integrationResult = new IntegrationResult<List<ShipmentLine>>();
                integrationResult.ReturnCode = "MSSQL-ERR";
                integrationResult.Message = ex.Message;
                return integrationResult;
            }
            catch (Exception ex)
            {
                IntegrationResult<List<ShipmentLine>> integrationResult = new IntegrationResult<List<ShipmentLine>>();
                integrationResult.ReturnCode = "APP-ERR";
                integrationResult.Message = ex.Message;
                return integrationResult;
            }
        }
        public static IntegrationResult<List<ShipmentHeader>> GetProductList(int dShipID)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(Functions.MSSQLConnectionStringEnfotek()))
                {
                    List<ShipmentHeader> list = cnn.Query<ShipmentHeader>(Functions.GetSippmentSqlList(dShipID), (object)null, (IDbTransaction)null, true, new int?(), new CommandType?()).ToList<ShipmentHeader>();
                    IntegrationResult<List<ShipmentHeader>> integrationResult = new IntegrationResult<List<ShipmentHeader>>();
                    integrationResult.Data = list;
                    integrationResult.ReturnCode = "OK";
                    return integrationResult;
                }
            }
            catch (SqlException ex)
            {
                IntegrationResult<List<ShipmentHeader>> integrationResult = new IntegrationResult<List<ShipmentHeader>>();
                integrationResult.ReturnCode = "MSSQL-ERR";
                integrationResult.Message = ex.Message;
                return integrationResult;
            }
            catch (Exception ex)
            {
                IntegrationResult<List<ShipmentHeader>> integrationResult = new IntegrationResult<List<ShipmentHeader>>();
                integrationResult.ReturnCode = "APP-ERR";
                integrationResult.Message = ex.Message;
                return integrationResult;
            }
        }
        public static IntegrationResult<List<ShipmentLine>> GetSippmentProductInsert(int dOrderID)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(Functions.MSSQLConnectionStringEnfotek()))
                {
                    List<ShipmentLine> list = cnn.Query<ShipmentLine>(Functions.GetSippmentSqlQueryInsert(dOrderID), (object)null, (IDbTransaction)null, true, new int?(), new CommandType?()).ToList<ShipmentLine>();
                    IntegrationResult<List<ShipmentLine>> integrationResult = new IntegrationResult<List<ShipmentLine>>();
                    integrationResult.Data = list;
                    integrationResult.ReturnCode = "OK";
                    return integrationResult;
                }
            }
            catch (SqlException ex)
            {
                IntegrationResult<List<ShipmentLine>> integrationResult = new IntegrationResult<List<ShipmentLine>>();
                integrationResult.ReturnCode = "MSSQL-ERR";
                integrationResult.Message = ex.Message;
                return integrationResult;
            }
            catch (Exception ex)
            {
                IntegrationResult<List<ShipmentLine>> integrationResult = new IntegrationResult<List<ShipmentLine>>();
                integrationResult.ReturnCode = "APP-ERR";
                integrationResult.Message = ex.Message;
                return integrationResult;
            }
        }
        public static bool EnfotekIrsCheck(int dEnfotekIrsID)
        {
            bool blnResult = false;

            string strSorgu = "SELECT COUNT(*) AS DURUM FROM EnfotekNebimEntegrasyon WHERE EnfotekIrsID = {0}";
            strSorgu = string.Format(strSorgu, dEnfotekIrsID);

            using (SqlConnection cnn = new SqlConnection(Functions.MSSQLConnectionString()))
            {
                int dDocumentCount = cnn.ExecuteScalar<int>(strSorgu, null, null, null, null);

                if (dDocumentCount == 0)
                {
                    blnResult = true;
                }
                else
                {
                    blnResult = false;
                }
            }

            return blnResult;
        }
        public static DataTable NebimMusteriKodu(string strTaxNumber, string strTypeCode, string strVergi)
        {
            DataTable dtReturn = new DataTable();

            string strSorgu = @"SELECT TOP(1) * FROM(
SELECT 
  A1.PostalAddressID, 
  A1.CurrAccTypeCode, 
  A1.CurrAccCode, 
  (SELECT CurrAccDescription FROM dbo.CurrAcc('TR') WHERE CurrAccCode = A1.CurrAccCode AND CurrAccTypeCode =  {1}) AS CurrAccDesc,
  A1.Address, 
  S1.StateDescription, 
  C1.CityDescription, 
  D1.DistrictDescription, 
  A1.ZipCode ,
  A1.AddressTypeCode
FROM 
  dbo.prCurrAccPostalAddress A1 WITH(NOLOCK) 
  INNER JOIN dbo.State('TR') S1 ON S1.StateCode = A1.StateCode 
  INNER JOIN dbo.City('TR') C1 ON C1.CityCode = A1.CityCode 
  INNER JOIN dbo.District('TR') D1 ON d1.DistrictCode = A1.DistrictCode 
WHERE 
  A1.CurrAccCode IN (
    SELECT 
      CurrAccCode 
    FROM 
      dbo.CurrAcc('TR') 
    WHERE 
      {2} = '{0}' 
      AND CurrAccTypeCode =  {1}
  ) 
    AND A1.CurrAccTypeCode =  {1}
	AND CAST(
    A1.AddressTypeCode AS NVARCHAR(100)
  ) = CAST(
    2 AS NVARCHAR(100))
UNION ALL
SELECT 
  A1.PostalAddressID, 
  A1.CurrAccTypeCode, 
  A1.CurrAccCode, 
  (SELECT CurrAccDescription FROM dbo.CurrAcc('TR') WHERE CurrAccCode = A1.CurrAccCode AND CurrAccTypeCode =  {1}) AS CurrAccDesc,
  A1.Address, 
  S1.StateDescription, 
  C1.CityDescription, 
  D1.DistrictDescription, 
  A1.ZipCode ,
  A1.AddressTypeCode
FROM 
  dbo.prCurrAccPostalAddress A1 WITH(NOLOCK) 
  INNER JOIN dbo.State('TR') S1 ON S1.StateCode = A1.StateCode 
  INNER JOIN dbo.City('TR') C1 ON C1.CityCode = A1.CityCode 
  INNER JOIN dbo.District('TR') D1 ON d1.DistrictCode = A1.DistrictCode 
WHERE 
  A1.CurrAccCode IN (
    SELECT 
      CurrAccCode 
    FROM 
      dbo.CurrAcc('TR') 
    WHERE 
      {2} = '{0}' 
      AND CurrAccTypeCode =  {1}
  ) 
    AND A1.CurrAccTypeCode =  {1}
	AND CAST(
    A1.AddressTypeCode AS NVARCHAR(100)
  ) = CAST(
    1 AS NVARCHAR(100))) AS TBL
	WHERE TBL.CurrAccDesc NOT LIKE '%Kullanma%'";

            strSorgu = string.Format(strSorgu, strTaxNumber, strTypeCode, strVergi);

            using (SqlConnection cnn = new SqlConnection(Functions.MSSQLConnectionString()))
            {
               dtReturn.Load(cnn.ExecuteReader(strSorgu, null, null, null, null));
            }

            return dtReturn;
        }
        public static IntegrationResult SendSippment(List<ShipmentHeader> Shipment, List<ShipmentLine> ShipmentLine, string strEnfotekIrsNo, int dEnfotekIrsID,
            string strSession, string strIP, string strMusteriKodu, string strHareketTipi, int dShippingID, string strSeries, string strSeriesNumber, 
            DateTime dtTarih, string strShippingPostalAddressID, string strBillingPostalAddressID, bool blnPenti)
        {
            IntegrationResult result;
            string strMessage = "";

            try
            {
                foreach (var item in ShipmentLine)
                {
                    if (!string.IsNullOrEmpty(item.RENK))
                    {
                        string strNebimItemCode = CheckEnfotekModel(item.OLDMODEL, blnPenti);
                        item.NEBIMSTOK = strNebimItemCode;
                    }
                    else if (string.IsNullOrEmpty(item.MODEL) && string.IsNullOrEmpty(item.NEBIMSTOK))
                    {
                        item.NEBIMSTOK = item.STOK_KODU;
                    }
                }

                if (blnPenti)
                {
                    GetShippingSave(strSession, strIP, strMusteriKodu, strHareketTipi, dShippingID, strSeries, strSeriesNumber, strShippingPostalAddressID, strBillingPostalAddressID, ShipmentLine, dtTarih, blnPenti, out strMessage);
                }
                else
                {
                    GetShippingSave(strSession, strIP, strMusteriKodu, strHareketTipi, dShippingID, strSeries, strSeriesNumber, strShippingPostalAddressID, strBillingPostalAddressID, ShipmentLine, dtTarih, blnPenti, out strMessage);
                }

                if (strMessage.Contains("Detay"))
                {
                    throw new Exception(strMessage);
                }
                else
                {
                    EnfotekSonucKayit(strEnfotekIrsNo, dEnfotekIrsID, strMessage);
                }

                return new IntegrationResult()
                {
                    ReturnCode = "OK",
                    Message = "İrsaliye Aktarımı Başarılı. Aktarılan İrsaliye Numarası :" + " " + strMessage
                };
            }
            catch (SqlException ex)
            {
                result = new IntegrationResult
                {
                    ReturnCode = "MSSQL-ERR",
                    Message = ex.Message
                };
            }
            catch (Exception ex2)
            {
                result = new IntegrationResult
                {
                    ReturnCode = "APP-ERR",
                    Message = ex2.Message
                };
            }

            return result;
        }

        public static IntegrationResult SendProduct(DataTable ShipmentLine, List<ShipmentLine> ShipmentLines, string strSession, string strIP)
        {
            IntegrationResult result;
            string strMessage = "";

            try
            {
                foreach (var item in ShipmentLines)
                {
                    if (!string.IsNullOrEmpty(item.RENK))
                    {
                        string strNebimItemCode = CheckEnfotekModel(item.MODEL, false);
                        item.NEBIMSTOK = strNebimItemCode;
                    }
                    else if (string.IsNullOrEmpty(item.MODEL) && string.IsNullOrEmpty(item.NEBIMSTOK))
                    {
                        item.NEBIMSTOK = item.STOK_KODU;
                    }
                }

                List<ShipmentLine> products = DataTableToList<ShipmentLine>(ShipmentLine);

                var ProductGrp = products.GroupBy(g => new
                {
                    g.RENK,
                    g.MODEL,
                    g.OLDMODEL,
                    g.NEBIMSTOK,
                    g.STOK_ADI,
                    g.STOK_KODU
                }).Select(group => new
                {
                    Renk = group.Key.RENK,
                    MODEL = group.Key.MODEL,
                    OLDMODEL = group.Key.OLDMODEL,
                    NEBIMSTOK = group.Key.NEBIMSTOK,
                    STOK_ADI = group.Key.STOK_ADI.Trim(),
                    STOK_KODU = group.Key.STOK_KODU.Trim()
                });


                foreach (var item in ProductGrp)
                {
                    string strColorName = item.Renk.ToString();
                    string strCheckColor = CheckColor(strColorName);
                    string strModelCheck = "";
                    string strSorgu = "SELECT ItemCode FROM cdItem WHERE ItemCode = '{0}'";
                    string strModel = "";

                    string strGelenModel = "", strCheckEdecegimizModel = "";

                    if (string.IsNullOrEmpty(item.MODEL.ToString().Trim()))
                    {
                        strCheckEdecegimizModel = item.STOK_KODU;
                    }
                    else
                    {
                        strCheckEdecegimizModel = item.OLDMODEL.ToString().Trim();
                    }

                    if(string.IsNullOrEmpty(item.NEBIMSTOK.ToString()))
                    {
                        using (SqlConnection SqlCnn = new SqlConnection(Functions.MSSQLConnectionString()))
                        {
                            string strModelSorgusu = string.Format("SELECT NebimStokKodu FROM dbo.EnfotekNebimStokKartlari WHERE EnfotekStokKodu = '{0}' AND PKT = 0  GROUP BY NebimStokKodu", strCheckEdecegimizModel);
                            strGelenModel = SqlCnn.ExecuteScalar<string>(strModelSorgusu, null, null, 120, null);
                        }
                    }
                    else
                    {
                        strGelenModel = item.NEBIMSTOK;
                    }

                    if(string.IsNullOrEmpty(strGelenModel))
                    {
                        if (string.IsNullOrEmpty(item.MODEL.ToString().Trim()))
                        {
                            strModel = item.STOK_KODU;
                        }
                        else
                        {
                            strModel = RandomCode() + "-EFT";
                        }
                    }
                    else
                    {
                        strModel = strGelenModel;
                    }

                    strSorgu = string.Format(strSorgu, strModel);

                    if (!string.IsNullOrEmpty(strColorName))
                    {
                        using (SqlConnection SqlCnn = new SqlConnection(Functions.MSSQLConnectionString()))
                        {
                            strModelCheck = SqlCnn.ExecuteScalar<string>(strSorgu, null, null, 120, null);
                        }

                        if (string.IsNullOrEmpty(strModelCheck))
                        {
                            if (string.IsNullOrEmpty(strCheckColor))
                            {
                                InsertColor(strColorName);

                                strCheckColor = CheckColor(strColorName);
                            }

                            PostProduct(strModel, item.MODEL.ToString(), item.OLDMODEL.ToString(), item.STOK_KODU.ToString() + " " + item.STOK_ADI.ToString(), item.Renk.ToString(), strCheckColor, ShipmentLines, strSession, strIP, 1, out strMessage);

                            if (!strMessage.Contains("İşlem Sırasında"))
                            {
                                InsertEnfotekItemCode(strMessage, item.OLDMODEL.ToString(), 0);
                            }
                            else
                            {
                                throw new ArgumentException(strMessage);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(CheckVariant(strModelCheck, strCheckColor)))
                            {
                                strCheckColor = CheckColor(strColorName);

                                if (string.IsNullOrEmpty(strCheckColor))
                                {
                                    InsertColor(strColorName);
                                    strCheckColor = CheckColor(strColorName);
                                }

                                PostProduct(strModel, item.MODEL.ToString(), item.OLDMODEL.ToString(), item.STOK_KODU.ToString() + " " + item.STOK_ADI.ToString(), item.Renk.ToString(), strCheckColor, ShipmentLines, strSession, strIP, 2, out strMessage);
                            }
                        }
                    }
                    else
                    {
                        foreach (var items in ShipmentLines.Where(x => x.STOK_KODU == item.STOK_KODU))
                        {
                            items.STOK_KODU = strModel;
                        }
                    }
                }

                return new IntegrationResult()
                {
                    ReturnCode = "OK",
                    Message = strMessage
                };
            }
            catch (SqlException ex)
            {
                result = new IntegrationResult
                {
                    ReturnCode = "MSSQL-ERR",
                    Message = ex.Message
                };
            }
            catch (Exception ex2)
            {
                result = new IntegrationResult
                {
                    ReturnCode = "APP-ERR",
                    Message = ex2.Message
                };
            }

            return result;
        }
        public static IntegrationResult SendProductPenti(DataTable ShipmentLine, List<ShipmentLine> ShipmentLines, string strSession, string strIP)
        {
            IntegrationResult result;
            string strMessage = "";
            int dBirimType = 0;

            try
            {
                foreach (var item in ShipmentLines)
                {
                    if (!string.IsNullOrEmpty(item.RENK))
                    {
                        string strNebimItemCode = CheckEnfotekModel(item.OLDMODEL, true);
                        if(!string.IsNullOrEmpty(strNebimItemCode))
                        {
                            item.NEBIMSTOK = strNebimItemCode;
                        }
                        else
                        {
                            item.NEBIMSTOK = "";
                        }
                    }
                    else if (string.IsNullOrEmpty(item.MODEL) && string.IsNullOrEmpty(item.NEBIMSTOK))
                    {
                        item.NEBIMSTOK = item.STOK_KODU;
                    }
                }

                List<ShipmentLine> products = DataTableToList<ShipmentLine>(ShipmentLine);

                var ProductGrp = products.GroupBy(g => new
                {
                    g.KALEM_ACIKLAMA,
                    g.MODEL,
                    g.NEBIMSTOK,
                    g.OLDMODEL,
                    g.STOK_ADI,
                    g.STOK_KODU
                }).Select(group => new
                {
                    KALEM_ACIKLAMA = group.Key.KALEM_ACIKLAMA,
                    MODEL = group.Key.MODEL,
                    NEBIMSTOK = group.Key.NEBIMSTOK,
                    OLDMODEL = group.Key.OLDMODEL,
                    STOK_ADI = group.Key.STOK_ADI.Trim(),
                    STOK_KODU = group.Key.STOK_KODU.Trim()
                });


                foreach (var item in ProductGrp)
                {
                    string strModelCheck = "";
                    string strSorgu = "SELECT ItemCode FROM cdItem WHERE ItemCode = '{0}'";
                    string strModel = "";

                    if (item.KALEM_ACIKLAMA.ToString().Contains("ASORTİ") || item.KALEM_ACIKLAMA.ToString().Contains("asorti")
                    || item.KALEM_ACIKLAMA.ToString().Contains(" ASS ") || item.KALEM_ACIKLAMA.ToString().Contains(" ass "))
                    {
                        dBirimType = 1;
                    }
                    else
                    {
                        dBirimType = 2;
                    }

                    string strGelenModel = "", strCheckEdecegimizModel = "";

                    if (string.IsNullOrEmpty(item.MODEL.ToString().Trim()))
                    {
                        strCheckEdecegimizModel = item.STOK_KODU;
                    }
                    else
                    {
                        strCheckEdecegimizModel = item.OLDMODEL.ToString().Trim();
                    }

                    if (string.IsNullOrEmpty(item.NEBIMSTOK.ToString()))
                    {
                        using (SqlConnection SqlCnn = new SqlConnection(Functions.MSSQLConnectionString()))
                        {
                            string strModelSorgusu = string.Format("SELECT NebimStokKodu FROM dbo.EnfotekNebimStokKartlari WHERE EnfotekStokKodu = '{0}' AND PKT = 1 GROUP BY NebimStokKodu", strCheckEdecegimizModel);
                            strGelenModel = SqlCnn.ExecuteScalar<string>(strModelSorgusu, null, null, 120, null);
                        }
                    }
                    else
                    {
                        strGelenModel = item.NEBIMSTOK;
                    }

                    if (string.IsNullOrEmpty(strGelenModel))
                    {
                        if (string.IsNullOrEmpty(item.MODEL.ToString().Trim()))
                        {
                            strModel = item.STOK_KODU;
                        }
                        else
                        {
                            string[] ModelSplit = item.MODEL.ToString().Trim().Split('-');
                            bool blnModelChecks = false;

                            for (int i = 0; i < ModelSplit.Length; i++)
                            {
                                if(ModelSplit[i].ToString().Trim().Contains("AS"))
                                {
                                    blnModelChecks = true;
                                }
                            }
                            if(!blnModelChecks)
                            {
                                strModel = RandomCode() + "-AS";
                            }
                            else
                            {
                                strModel = item.MODEL.ToString().Trim();
                            }
                        }
                    }
                    else
                    {
                        strModel = strGelenModel;
                    }

                    strSorgu = string.Format(strSorgu, strModel);

                    using (SqlConnection SqlCnn = new SqlConnection(Functions.MSSQLConnectionString()))
                    {
                        strModelCheck = SqlCnn.ExecuteScalar<string>(strSorgu, null, null, 120, null);
                    }

                    if (string.IsNullOrEmpty(strModelCheck))
                    {
                        PostProductPenti(strModel, item.MODEL.ToString(), item.OLDMODEL.ToString(), item.STOK_KODU.ToString() + " " + item.STOK_ADI.ToString(), ShipmentLines, strSession, strIP, 1, dBirimType, out strMessage);

                        if (!strMessage.Contains("İşlem Sırasında"))
                        {
                            InsertEnfotekItemCode(strMessage, item.OLDMODEL.ToString(), 1);
                        }
                        else
                        {
                            throw new ArgumentException(strMessage);
                        }
                    }
                    else
                    {
                        PostProductPenti(strModel, item.MODEL.ToString(), item.OLDMODEL.ToString(), item.STOK_KODU.ToString() + " " + item.STOK_ADI.ToString(), ShipmentLines, strSession, strIP, 2, dBirimType, out strMessage);

                        if (strMessage.Contains("İşlem Sırasında"))
                        {
                            throw new ArgumentException(strMessage);
                        }
                    }
                }

                return new IntegrationResult()
                {
                    ReturnCode = "OK",
                    Message = strMessage
                };
            }
            catch (SqlException ex)
            {
                result = new IntegrationResult
                {
                    ReturnCode = "MSSQL-ERR",
                    Message = ex.Message
                };
            }
            catch (Exception ex2)
            {
                result = new IntegrationResult
                {
                    ReturnCode = "APP-ERR",
                    Message = ex2.Message
                };
            }

            return result;
        }
        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }
        public static bool EnfotekSonucKayit(string strIrsNo, int dIrsID, string strSonuc)
        {
            bool blnReturn = false;

            try
            {
                using (SqlConnection SqlCnn = new SqlConnection(Functions.MSSQLConnectionString()))
                {
                    string strSorgu = @"INSERT INTO EnfotekNebimEntegrasyon(EnfotekIrsNo, EnfotekIrsID, Sonuc)
                                        VALUES (@_EnfotekIrsNo,@_EnfotekIrsID,@_Sonuc)";

                    blnReturn = SqlCnn.ExecuteScalar<int>(strSorgu, new
                    {
                        _EnfotekIrsNo = strIrsNo,
                        _EnfotekIrsID = dIrsID,
                        _Sonuc = strSonuc

                    }) == 0 ? true : false;
                }
            }
            catch
            {
            }

            return blnReturn;
        }
        public static string InsertColor(string strColorName)
        {
            string strReturn = "";

            try
            {
                using (SqlConnection SqlCnn = new SqlConnection(Functions.MSSQLConnectionString()))
                {
                    string strSorgu = @"SELECT MAX(CONVERT(INT,ColorCode))+1 AS ColorCode FROM dbo.cdColor WHERE ColorCode LIKE '[0-9]%'";

                    string strColorCode = SqlCnn.ExecuteScalar<string>(strSorgu, null, null, 120, null);

                    strSorgu = @"INSERT INTO cdColor(ColorCode,ColorHex,ColorCatalogCode1,ColorCatalogCode2,ColorCatalogCode3,
                                    IsBlocked,CreatedUserName,CreatedDate,LastUpdatedUserName,LastUpdatedDate,RowGuid)
                                    VALUES(@_ColorCode,'','','','',0,'Administrator',GETDATE(),'adm  ghsadm',GETDATE(),NEWID())";

                    SqlCnn.Execute(strSorgu, new
                    {
                        _ColorCode = strColorCode
                    }, null, 120, null);

                    strSorgu = @"INSERT INTO dbo.cdColorDesc (ColorCode, LangCode, ColorDescription, CreatedUserName, CreatedDate, LastUpdatedUserName, LastUpdatedDate, RowGuid)
                                VALUES (@_ColorCode, 'TR', @_ColorDescription, 'adm  ghsadm', GETDATE(), 'adm  ghsadm', GETDATE(), NEWID())";

                    SqlCnn.Execute(strSorgu, new
                    {
                        _ColorCode = strColorCode,
                        _ColorDescription = strColorName
                    }, null, 120, null);
                }
            }
            catch(Exception ex)
            {
                strReturn = "Renk Eklenirken Bilinmeyen Hata Oluştu. Detay:" + ex.Message.ToString();
            }

            return strReturn;
        }
        public static string CheckColor(string strColorName)
        {
            string strReturn = "";

            try
            {
                string strSorgu = @"SELECT ColorCode FROM dbo.Color('TR') WHERE LangCode = 'TR' AND ColorDescription = '{0}'";

                strSorgu = string.Format(strSorgu, strColorName);

                using (SqlConnection SqlCnn = new SqlConnection(Functions.MSSQLConnectionString()))
                {
                    strReturn = SqlCnn.ExecuteScalar<string>(strSorgu, null, null, 120, null);
                }
            }
            catch (Exception ex)
            {
                strReturn = "Renk Sorgulanırken Hata Oluştu. Detay:" + ex.Message.ToString();
            }

            return strReturn;
        }
        public static string CheckVariant(string strItemCode, string strColorCode)
        {
            string strReturn = "";

            try
            {
                string strSorgu = @"SELECT COUNT(*) AS Durum FROM prItemVariant WHERE ItemCode ='{0}' AND ColorCode = '{1}'";

                strSorgu = string.Format(strSorgu, strItemCode, strColorCode);

                using (SqlConnection SqlCnn = new SqlConnection(Functions.MSSQLConnectionString()))
                {
                    strReturn = SqlCnn.ExecuteScalar<string>(strSorgu, null, null, 120, null);
                }
            }
            catch (Exception ex)
            {
                strReturn = "Varyant Sorgulanırken Hata Oluştu. Detay:" + ex.Message.ToString();
            }

            return strReturn;
        }
        public static string CheckEnfotekModel(string strEnfotekStokKodu, bool blnPenti)
        {
            string strReturn = "", strSorgu = "";

            try
            {
                if(blnPenti)
                {
                    strSorgu = @"SELECT NebimStokKodu FROM dbo.EnfotekNebimStokKartlari WHERE EnfotekStokKodu = '{0}' AND PKT = 1";
                }
                else
                {
                    strSorgu = @"SELECT NebimStokKodu FROM dbo.EnfotekNebimStokKartlari WHERE EnfotekStokKodu = '{0}' AND PKT = 0";
                }

                strSorgu = string.Format(strSorgu, strEnfotekStokKodu);

                using (SqlConnection SqlCnn = new SqlConnection(Functions.MSSQLConnectionString()))
                {
                    strReturn = SqlCnn.ExecuteScalar<string>(strSorgu, null, null, 120, null);
                }
            }
            catch (Exception ex)
            {
                strReturn = "Ürün Sorgulanırken Hata Oluştu. Detay:" + ex.Message.ToString();
            }

            return strReturn;
        }
        public static string CheckPKTModel(string strEnfotekStokKodu)
        {
            string strReturn = "";

            try
            {
                string strSorgu = @"SELECT (CASE ItemDimTypeCode WHEN 0 THEN 'True' ELSE 'False' END) ItemDimTypeCode FROM dbo.cdItem WHERE ItemCode = '{0}'";

                strSorgu = string.Format(strSorgu, strEnfotekStokKodu);

                using (SqlConnection SqlCnn = new SqlConnection(Functions.MSSQLConnectionString()))
                {
                    strReturn = SqlCnn.ExecuteScalar<string>(strSorgu, null, null, 120, null);
                }
            }
            catch (Exception ex)
            {
                strReturn = "Ürün Sorgulanırken Hata Oluştu. Detay:" + ex.Message.ToString();
            }

            return strReturn;
        }
        public static string InsertEnfotekItemCode(string strItemCode, string strModel, int dPkt)
        {
            string strReturn = "";

            try
            {
                string strSorgu = @"INSERT INTO EnfotekNebimStokKartlari (NebimStokKodu, EnfotekStokKodu, Pkt)
                                VALUES (@_NebimStokKodu,@_EnfotekStokKodu, @_Pkt)";

                using (SqlConnection SqlCnn = new SqlConnection(Functions.MSSQLConnectionString()))
                {
                    SqlCnn.Execute(strSorgu, new
                    {
                        _NebimStokKodu = strItemCode,
                        _EnfotekStokKodu = strModel,
                        _Pkt = dPkt
                    }, null, 120, null);
                }
            }
            catch (Exception ex)
            {
                strReturn = "Ürün Kartı Eklenirken Hata Oluştu. Detay:" + ex.Message.ToString();
            }

            return strReturn;
        }
        public static string RandomCode()
        {
            Random rastgele = new Random();
            string harfler = "ABCDEFGHIJKLMNOPRSTUVYZ";
            string strReturn = "";
            for (int i = 0; i < 12; i++)
            {
                strReturn += harfler[rastgele.Next(harfler.Length)];
            }

            return strReturn;
        }

        public static List<T> DataTableToList<T>(this DataTable dataTable) where T : new()
        {
            var dataList = new List<T>();
            const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
            var objFieldNames = typeof(T).GetProperties(flags).Cast<System.Reflection.PropertyInfo>().
                Select(item => new
                {
                    Name = item.Name,
                    Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
                }).ToList();
            var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
                Select(item => new
                {
                    Name = item.ColumnName,
                    Type = item.DataType
                }).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var classObj = new T();

                foreach (var dtField in dtlFieldNames)
                {
                    System.Reflection.PropertyInfo propertyInfos = classObj.GetType().GetProperty(dtField.Name);

                    var field = objFieldNames.Find(x => x.Name == dtField.Name);

                    if (field != null)
                    {

                        if (propertyInfos.PropertyType == typeof(DateTime))
                        {
                            propertyInfos.SetValue
                            (classObj, convertToDateTime(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(Nullable<DateTime>))
                        {
                            propertyInfos.SetValue
                            (classObj, convertToDateTime(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(int))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToInt(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(long))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToLong(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(decimal))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToDecimal(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(String))
                        {
                            if (dataRow[dtField.Name].GetType() == typeof(DateTime))
                            {
                                propertyInfos.SetValue
                                (classObj, ConvertToDateString(dataRow[dtField.Name]), null);
                            }
                            else
                            {
                                propertyInfos.SetValue
                                (classObj, ConvertToString(dataRow[dtField.Name]), null);
                            }
                        }
                        else
                        {
                            propertyInfos.SetValue
                                (classObj, Convert.ChangeType(dataRow[dtField.Name], propertyInfos.PropertyType), null);
                        }
                    }
                }
                dataList.Add(classObj);
            }
            return dataList;
        }
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
        public static object ReturnEmptyIfNull(this object value)
        {
            if (value == DBNull.Value)
                return string.Empty;
            if (value == null)
                return string.Empty;
            return value;
        }
        public static object ReturnZeroIfNull(this object value)
        {
            if (value == DBNull.Value)
                return 0;
            if (value == null)
                return 0;
            return value;
        }
        public static object ReturnDateTimeMinIfNull(this object value)
        {
            if (value == DBNull.Value)
                return DateTime.MinValue;
            if (value == null)
                return DateTime.MinValue;
            return value;
        }
        private static DateTime convertToDateTime(object date)
        {
            return Convert.ToDateTime(ReturnDateTimeMinIfNull(date));
        }
        private static decimal ConvertToDecimal(object value)
        {
            return Convert.ToDecimal(ReturnZeroIfNull(value));
        }
        private static string ConvertToString(object value)
        {
            return Convert.ToString(ReturnEmptyIfNull(value));
        }
        private static string ConvertToDateString(object date)
        {
            if (date == null)
                return string.Empty;

            return date == null ? string.Empty : Convert.ToDateTime(date).ConvertDate();
        }
        private static int ConvertToInt(object value)
        {
            return Convert.ToInt32(ReturnZeroIfNull(value));
        }
        private static long ConvertToLong(object value)
        {
            return Convert.ToInt64(ReturnZeroIfNull(value));
        }
        public static string ConvertDate(this DateTime datetTime, bool excludeHoursAndMinutes = false)
        {
            if (datetTime != DateTime.MinValue)
            {
                if (excludeHoursAndMinutes)
                    return datetTime.ToString("yyyy-MM-dd");
                return datetTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            return null;
        }
        public static string ModelsReplace(string strModels)
        {
            strModels = strModels.Trim().ToLower();
            strModels = Regex.Replace(strModels, ",", "");
            strModels = Regex.Replace(strModels, "-", "");
            strModels = Regex.Replace(strModels, @"[^a-z0-9]", "");
            strModels = Regex.Replace(strModels, @"\s+", "");
            strModels = strModels.Trim();
            strModels = Regex.Replace(strModels, " ", "-");
            return strModels;
        }
    }
    public class ItemAccountCodes
    {
        public string CODE { get; set; }
        public string DESC { get; set; }
    }
    public class RefreshTokenResultJSON
    {
        public string SessionID { get; set; }
        public string CurrAccCode { get; set; }
        public string ShippingAddressID { get; set; }
        public string ShippingNumber { get; set; }
        public string ItemCode { get; set; }
        public string Token { get; set; }
        public string ExceptionMessage { get; set; }
    }
    public class Shipment
    {
        public int ModelType { get; set; }
        public string BillingPostalAddressID { get; set; }
        public int CompanyCode { get; set; }
        public string CustomerCode { get; set; }
        public bool IsReturn { get; set; }
        public List<Lines> Lines { get; set; }
        public string ShippingDate { get; set; }
        public string ShippingPostalAddressID { get; set; }
        public string OperationDate { get; set; }
        public string OfficeCode { get; set; }
        public string Series { get; set; }
        public long SeriesNumber { get; set; }
        public string Description { get; set; }
        public string WarehouseCode { get; set; }
    }
    public class Shipment2
    {
        public int ModelType { get; set; }
        public string BillingPostalAddressID { get; set; }
        public int CompanyCode { get; set; }
        public string VendorCode { get; set; }
        public bool IsReturn { get; set; }
        public List<Lines2> Lines { get; set; }
        public DateTime ShippingDate { get; set; }
        public string ShippingPostalAddressID { get; set; }
        public DateTime OperationDate { get; set; }
        public string OfficeCode { get; set; }
        public string Series { get; set; }
        public long SeriesNumber { get; set; }
        public string Description { get; set; }
        public string WarehouseCode { get; set; }
    }
    public class Lines2
    {
        public bool AllowNotExistReturnBatchCode { get; set; }
        public string BatchCode { get; set; }
        public string ColorCode { get; set; }
        public string DeliveryCompanyBarcode { get; set; }
        public bool DisableBeforeDoSave { get; set; }
        public bool DisableLoadRolls { get; set; }
        public string ExportFileNumber { get; set; }
        public string ImportFileNumber { get; set; }
        public bool IsInvoiced { get; set; }
        public string ITAtt01 { get; set; }
        public string ITAtt02 { get; set; }
        public string ITAtt03 { get; set; }
        public string ITAtt04 { get; set; }
        public string ITAtt05 { get; set; }
        public string ItemCode { get; set; }
        public string ItemDim1Code { get; set; }
        public string ItemDim2Code { get; set; }
        public string ItemDim3Code { get; set; }
        public int ItemTypeCode { get; set; }
        public string LineDescription { get; set; }
        public string LogisticsPackageNumber { get; set; }
        public string OrderAsnLineID { get; set; }
        public string OrderLineID { get; set; }
        public int Price { get; set; }
        public string PriceCurrencyCode { get; set; }
        public string PriceListLineID { get; set; }
        public string PurchasePlanCode { get; set; }
        public int Qty1 { get; set; }
        public int Qty2 { get; set; }
        public string ReturnReasonCode { get; set; }
        public string SectionCode { get; set; }
        public int SerialLineSumID { get; set; }
        public int ShipmentLineBOMID { get; set; }
        public int ShipmentLineSumID { get; set; }
        public int SortOrder { get; set; }
        public string SupportRequestHeaderID { get; set; }
        public string SupportRequestNumber { get; set; }
        public string UsedBarcode { get; set; }

        public Lines2()
        {
        }
        public Lines2(bool _AllowNotExistReturnBatchCode, string _BatchCode, string _ColorCode, string _DeliveryCompanyBarcode,
            bool _DisableBeforeDoSave, bool _DisableLoadRolls, string _ExportFileNumber, string _ImportFileNumber,
            bool _IsInvoiced, string _ITAtt01, string _ITAtt02, string _ITAtt03, string _ITAtt04, string _ITAtt05,
            string _ItemCode, string _ItemDim1Code, string _ItemDim2Code, string _ItemDim3Code, int _ItemTypeCode, string _LineDescription,
            string _LogisticsPackageNumber, string _OrderAsnLineID, 
            string _OrderLineID, int _Price, string _PriceCurrencyCode, string _PriceListLineID, string _PurchasePlanCode, int _Qty1, int _Qty2,
            string _ReturnReasonCode, string _SectionCode, int _SerialLineSumID, int _ShipmentLineBOMID, int _ShipmentLineSumID,int _SortOrder,
            string _SupportRequestHeaderID, string _SupportRequestNumber,string _UsedBarcode)
        {
            AllowNotExistReturnBatchCode = _AllowNotExistReturnBatchCode;
            BatchCode = _BatchCode;
            ColorCode = _ColorCode;
            DeliveryCompanyBarcode = _DeliveryCompanyBarcode;
            DisableBeforeDoSave = _DisableBeforeDoSave;
            DisableLoadRolls = _DisableLoadRolls;
            ExportFileNumber = _ExportFileNumber;
            ImportFileNumber = _ImportFileNumber;
            IsInvoiced = _IsInvoiced;
            ITAtt01 = _ITAtt01;
            ITAtt02 = _ITAtt02;
            ITAtt03 = _ITAtt03;
            ITAtt04 = _ITAtt04;
            ITAtt05 = _ITAtt05;
            ItemCode = _ItemCode;
            ItemDim1Code = _ItemDim1Code;
            ItemDim2Code = _ItemDim2Code;
            ItemDim3Code = _ItemDim3Code;
            ItemTypeCode = _ItemTypeCode;
            LineDescription = _LineDescription;
            LogisticsPackageNumber = _LogisticsPackageNumber;
            OrderAsnLineID = _OrderAsnLineID;
            OrderLineID = _OrderLineID;
            Price = _Price;
            PriceCurrencyCode = _PriceCurrencyCode;
            PriceListLineID = _PriceListLineID;
            PurchasePlanCode = _PurchasePlanCode;
            Qty1 = _Qty1;
            Qty2 = _Qty2;
            ReturnReasonCode = _ReturnReasonCode;
            SectionCode = _SectionCode;
            SerialLineSumID = _SerialLineSumID;
            ShipmentLineBOMID = _ShipmentLineBOMID;
            ShipmentLineSumID = _ShipmentLineSumID;
            SortOrder = _SortOrder;
            SupportRequestHeaderID = _SupportRequestHeaderID;
            SupportRequestNumber = _SupportRequestNumber;
            UsedBarcode = _UsedBarcode;
        }
    }
    public class Lines
    {
        public string SortOrder { get; set; }
        public string ItemTypeCode { get; set; }
        public string ItemCode { get; set; }
        public string ColorCode { get; set; }
        public string ItemDim1Code { get; set; }
        public string ItemDim2Code { get; set; }
        public string ItemDim3Code { get; set; }
        public string Qty1 { get; set; }
        public string PaymentPlanCode { get; set; }
        public string LineDescription { get; set; }
        public string DeliveryCompanyBarcode { get; set; }
        public string LogisticsPackageNumber { get; set; }
        public string PriceCurrencyCode { get; set; }
        public string Price { get; set; }
        public string ImportFileNumber { get; set; }
        public string ExportFileNumber { get; set; }
        public Lines()
        {
        }
        public Lines(string _SortOrder, string _ItemTypeCode, string _ItemCode, string _ColorCode,
            string _ItemDim1Code, string _ItemDim2Code, string _ItemDim3Code, string _Qty1,
            string _PaymentPlanCode, string _LineDescription, string _DeliveryCompanyBarcode, string _LogisticsPackageNumber,
            string _PriceCurrencyCode, string _Price, string _ImportFileNumber, string _ExportFileNumber)
        {
            SortOrder = _SortOrder;
            ItemTypeCode = _ItemTypeCode;
            ItemCode = _ItemCode;
            ColorCode = _ColorCode;
            ItemDim1Code = _ItemDim1Code;
            ItemDim2Code = _ItemDim2Code;
            ItemDim3Code = _ItemDim3Code;
            Qty1 = _Qty1;
            PaymentPlanCode = _PaymentPlanCode;
            LineDescription = _LineDescription;
            DeliveryCompanyBarcode = _DeliveryCompanyBarcode;
            LogisticsPackageNumber = _LogisticsPackageNumber;
            PriceCurrencyCode = _PriceCurrencyCode;
            Price = _Price;
            ImportFileNumber = _ImportFileNumber;
            ExportFileNumber = _ExportFileNumber;
        }
    }
    public class ShippingTime
    {
        public long Ticks { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Milliseconds { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public double TotalDays { get; set; }
        public double TotalHours { get; set; }
        public int TotalMilliseconds { get; set; }
        public double TotalMinutes { get; set; }
        public int TotalSeconds { get; set; }
    }

    public class Descriptions
    {
        public string DataLanguageCode { get; set; }
        public string Description { get; set; }

        public Descriptions()
        {

        }
        public Descriptions(string _DataLanguageCode, string _Description)
        {
            DataLanguageCode = _DataLanguageCode;
            Description = _Description;
        }
    }
    public class ProductFrameProperties
    {
        public int BaseCurveRadius { get; set; }
        public string BaseMaterialCode { get; set; }
        public string BrandCode { get; set; }
        public int BridgeWidth { get; set; }
        public string CustomProcessGroupCode { get; set; }
        public string FrameShapeTypeCode { get; set; }
        public string FrameTypeCode { get; set; }
        public int LensHeight { get; set; }
        public int LensWidth { get; set; }
        public string ManufacturerCode { get; set; }
        public string OpticalSutCode { get; set; }
        public int TempleLength { get; set; }
    }
    public class ProductLensProperties
    {
        public int BaseCurveRadius { get; set; }
        public string BaseMaterialCode { get; set; }
        public string BrandCode { get; set; }
        public string CoatingTypeCode { get; set; }
        public string CustomProcessGroupCode { get; set; }
        public int Cylinder { get; set; }
        public int Diameter { get; set; }
        public int DisposeFrequency { get; set; }
        public int EyeGlassSutTypeCode { get; set; }
        public string FocalTypeCode { get; set; }
        public int GlassIndex { get; set; }
        public int LensTypeCode { get; set; }
        public string ManufacturerCode { get; set; }
        public string OpticalGroupRangeCode { get; set; }
        public int Sphere { get; set; }
        public int WaterContent { get; set; }
    }
    public class LinkedProductProperties
    {
        public string ColorCode { get; set; }
        public string ContentItemCode { get; set; }
        public int ContentItemTypeCode { get; set; }
        public int LinkedProductTypeCode { get; set; }
        public string LotCode { get; set; }
    }
    public class Products
    {
        public int ModelType { get; set; }
        public string ItemCode { get; set; }
        public string BOMEntityCode { get; set; }
        public bool ByWeight { get; set; }
        public int CommercialRoleCode { get; set; }
        public int CustomsProductGroupCode { get; set; }
        public string CustomsTariffNumberCode { get; set; }
        public List<Descriptions> Descriptions { get; set; }
        public bool DoNotLoadSectionTypeCodeAndDescriptionOnLoadItemSection { get; set; }
        public bool GenerateOpticalDataMatrixCode { get; set; }
        public bool GenerateSerialNumber { get; set; }
        public int GuaranteePeriod { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsPurchaseOrderClosed { get; set; }
        public int IsSalesOrderClosed { get; set; }
        public bool IsStoreOrderClosed { get; set; }
        public int IsSubsequentDeliveryForR { get; set; }
        public int IsSubsequentDeliveryForRI { get; set; }
        public bool IsUTSDeclaratedItem { get; set; }
        public string ItemAccountGrCode { get; set; }
        public string ItemDescription { get; set; }
        public int ItemDimTypeCode { get; set; }
        public string ItemDiscountGrCode { get; set; }
        public string ItemPaymentPlanGrCode { get; set; }
        public string ItemTaxGrCode { get; set; }
        public string ItemVendorGrCode { get; set; }
        public LinkedProductProperties LinkedProductProperties { get; set; }
        public int MaxCreditCardInstallmentCount { get; set; }
        public int OrderLeadTime { get; set; }
        public string OriginCountryCode { get; set; }
        public int PerceptionOfFashionCode { get; set; }
        public int ProductCollectionGrCode { get; set; }
        public ProductFrameProperties ProductFrameProperties { get; set; }
        public int ProductHierarchyID { get; set; }
        public ProductLensProperties ProductLensProperties { get; set; }
        public int ProductTypeCode { get; set; }
        public string PromotionGroupCode { get; set; }
        public string PromotionGroupCode2 { get; set; }
        public int ShelfLife { get; set; }
        public string StoreCapacityLevelCode { get; set; }
        public int StorePriceLevelCode { get; set; }
        public int SupplyPeriod { get; set; }
        public int UnitConvertRate { get; set; }
        public bool UnitConvertRateNotFixed { get; set; }
        public string UnitOfMeasureCode1 { get; set; }
        public bool UseBatch { get; set; }
        public bool UseInternet { get; set; }
        public bool UseManufacturing { get; set; }
        public bool UsePOS { get; set; }
        public bool UseRoll { get; set; }
        public bool UseSerialNumber { get; set; }
        public bool UseStore { get; set; }
        public List<Variant> Variants { get; set; }
    }
    public class Variant
    {
        public string ColorCode { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsLocked { get; set; }
        public bool IsPurchaseOrderClosed { get; set; }
        public int IsSalesOrderClosed { get; set; }
        public bool IsStoreOrderClosed { get; set; }
        public string ItemDim1Code { get; set; }
        public string ItemDim2Code { get; set; }
        public string ItemDim3Code { get; set; }
        public bool UseInternet { get; set; }

        public Variant()
        {
        }
        public Variant(string _ColorCode, bool _IsBlocked, bool _IsLocked, bool _IsPurchaseOrderClosed,
            int _IsSalesOrderClosed, bool _IsStoreOrderClosed, string _ItemDim1Code, string _ItemDim2Code,
            string _ItemDim3Code, bool _UseInternet)
        {
            ColorCode = _ColorCode;
            IsBlocked = _IsBlocked;
            IsLocked = _IsLocked;
            IsPurchaseOrderClosed = _IsPurchaseOrderClosed;
            IsSalesOrderClosed = _IsSalesOrderClosed;
            IsStoreOrderClosed = _IsStoreOrderClosed;
            ItemDim1Code = _ItemDim1Code;
            ItemDim2Code = _ItemDim2Code;
            ItemDim3Code = _ItemDim3Code;
            UseInternet = _UseInternet;
        }
    }
}
