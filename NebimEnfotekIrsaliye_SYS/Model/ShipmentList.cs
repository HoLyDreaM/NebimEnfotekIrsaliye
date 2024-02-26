using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebimEnfotekIrsaliye_SYS.Model
{
    public class ShipmentList
    {
        public string ShippingNumber { get; set; }
        public bool IsReturn { get; set; }
        public DateTime ShippingDate { get; set; }
        public TimeSpan ShippingTime { get; set; }
        public int CurrAccTypeCode { get; set; }
        public string VendorCode { get; set; }
        public string VendorDescription { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerDescription { get; set; }
        public string RetailCustomerCode { get; set; }
        public string StoreCurrAccCode { get; set; }
        public string StoreDescription { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstLastName { get; set; }
        public string SubCurrAccCode { get; set; }
        public string SubCurrAccCompanyName { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string ImportFileNumber { get; set; }
        public int Qty1 { get; set; }
        public int InvoicedQty1 { get; set; }
        public string ProcessCode { get; set; }
        public string Series { get; set; }
        public string SeriesNumber { get; set; }
        public string LogisticsCompanyBOL { get; set; }
        public int CompanyCode { get; set; }
        public string OfficeCode { get; set; }
        public string StoreCode { get; set; }
        public string WarehouseCode { get; set; }
        public string ToWarehouseCode { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsLocked { get; set; }
        public bool IsTransferApproved { get; set; }
        public bool IsOrderBase { get; set; }
        public bool IsPrinted { get; set; }
        public string ApplicationCode { get; set; }
        public string ApplicationDescription { get; set; }
        public Guid ApplicationID { get; set; }
        public Guid ShipmentHeaderID { get; set; }
    }
}
