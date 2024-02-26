using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebimEnfotekIrsaliye.Data
{
    public class ShipmentHeader
    {
        public string AktarimNotu { get; set; }
        public int MH_EIRSALIYEID { get; set; }
        public string UUID { get; set; }
        public string TIP { get; set; }
        public string IRS_NO { get; set; }
        public DateTime TARIH { get; set; }
        public string CUST_CARI_ISIM { get; set; }
        public string CUST_CARI_VERGI_NO { get; set; }
        public string CUST_CARI_TCKIMLIKNO { get; set; }
        public string CUST_CARI_ADRES { get; set; }
        public string SHIP_SEVKTAR { get; set; }
        public string SHIP_CARRIER_ISIM { get; set; }
        public string SHIP_CARRIER_VERGI_DAIRESI { get; set; }
        public string SHIP_CARRIER_VERGI_NO { get; set; }
        public string SHIP_CARRIER_TCKIMLIKNO { get; set; }
        public string SHIP_CARRIER_ADRES { get; set; }
        public string SHIP_CARRIER_IL { get; set; }
        public string SHIP_CARRIER_ILCE { get; set; }
        public string SHIP_CARRIER_POSTA_KODU { get; set; }
        public string SHIP_CARRIER_TEL_NO { get; set; }
        public string SHIP_ARAC_PLAKA_NO { get; set; }
        public string SHIP_DORSE_PLAKA1 { get; set; }
        public string SHIP_DRIVER_PERSON1_ADI { get; set; }
        public string SHIP_DRIVER_PERSON1_SOYADI { get; set; }
        public string SHIP_DRIVER_PERSON1_TCKIMLIKNO { get; set; }
        public string HAREKET_TIPI { get; set; }
    }
}
