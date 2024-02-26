using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebimEnfotekIrsaliye.Model
{
    public class SubCustomerList
    {
        public string CustomerCode { get; set; }
        public string CustomerDescription { get; set; }
        public string SubCurrAccCode { get; set; }
        public string CompanyName { get; set; }
        public bool IsVIP { get; set; }
        public bool IsIndividualAcc { get; set; }
        public string CurrencyCode { get; set; }
        public double CreditLimit { get; set; }
        public bool IsBlocked { get; set; }
        public int CompanyCode { get; set; }
        public string OfficeCode { get; set; }
        public Guid SubCurrAccID { get; set; }
    }
}
