using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebimEnfotekIrsaliye.Data
{
    public class PostalAddress
    {
        public Guid ShipmentHeaderID { get; set; }
        public Guid PostalAddressID { get; set; }
        public string CurrAccCode { get; set; }
        public string CurrAccDescription { get; set; }
        public string Address { get; set; }
        public string SiteName { get; set; }
        public string BuildingName { get; set; }
        public string BuildingNum { get; set; }
        public string FloorNum { get; set; }
        public string DoorNum { get; set; }
        public string CountryCode { get; set; }
        public string CountryDescription { get; set; }
        public string CityDescription { get; set; }
        public string DistrictDescription { get; set; }
        public string ZipCode { get; set; }
        public string TaxOfficeCode { get; set; }
        public string TaxOfficeDescription { get; set; }
        public string TaxNumber { get; set; }
        public string IdentityNum { get; set; }
    }
}
