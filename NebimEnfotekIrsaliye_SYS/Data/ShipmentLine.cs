using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebimEnfotekIrsaliye_SYS.Data
{
    public class ShipmentLine
    {
        public int MH_EIRSALIYE_KALEMIID { get; set; }
        public int MH_EIRSALIYEID { get; set; }
        public string MODEL { get; set; }
        public string OLDMODEL { get; set; }
        public string STOK_KODU { get; set; }
        public string STOK_ADI { get; set; }
        public string RENK { get; set; }
        public string BEDEN { get; set; }
        public decimal MIKTAR { get; set; }
        public string FIYAT { get; set; }
        public string DOVIZ_KODU { get; set; }
        public string BIRIM { get; set; }
        public string KALEM_ACIKLAMA { get; set; }
        public string SIP_TEMSILCI { get; set; }
        public string NEBIMSTOK { get; set; }
    }
}
