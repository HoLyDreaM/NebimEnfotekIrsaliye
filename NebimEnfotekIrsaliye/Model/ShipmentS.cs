using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebimEnfotekIrsaliye.Model
{
    public class ShipmentS
    {
        public class Shipment
        {
            public int ModelType { get; set; }
            public int TransTypeCode { get; set; }
            public string ProcessCode { get; set; }
            public string ShippingNumber { get; set; }
            public bool IsReturn { get; set; }
            public string ShippingDate { get; set; }
            public TimeSpan ShippingTime { get; set; }
            public string OperationDate { get; set; }
            public TimeSpan OperationTime { get; set; }
            public string Series { get; set; }
            public string SeriesNumber { get; set; }
            public string Description { get; set; }
            public string InternalDescription { get; set; }
            public string ToStoreCode { get; set; }
            public string ShipmentMethodCode { get; set; }
            public Guid ShippingPostalAddressID { get; set; }
            public string RoundsmanCode { get; set; }
            public string DeliveryCompanyCode { get; set; }
            public string LogisticsCompanyBOL { get; set; }
            public int CompanyCode { get; set; }
            public string OfficeCode { get; set; }
            public string WarehouseCode { get; set; }
            public string ToWarehouseCode { get; set; }
            public string ImportFileNumber { get; set; }
            public string ExportFileNumber { get; set; }
            public bool IsOrderBase { get; set; }
            public bool IsCompleted { get; set; }
            public bool IsPrinted { get; set; }
            public bool IsLocked { get; set; }
            public bool IsTransferApproved { get; set; }
            public List<Lines> Lines { get; set; }
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
    }
}
