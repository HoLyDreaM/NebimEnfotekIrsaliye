using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebimEnfotekIrsaliye.Data
{
    public class AppSettings
    {
        public int Id { get; set; }

        public string SQLServer { get; set; }
        public string SQLDatabase { get; set; }
        public string SQLUser { get; set; }
        public string SQLPass { get; set; }

        public string EnfoTekSQLServer { get; set; }
        public string EnfoTekSQLDatabase { get; set; }
        public string EnfoTekSQLUser { get; set; }
        public string EnfoTekSQLPass { get; set; }

        public string IrsaliyeTest { get; set; }

        public int CompanyCode { get; set; }
        public string OfficeCode { get; set; }
        public string POSTerminalID { get; set; }
        public string DepoKodu { get; set; }
        public int ServisSuresi { get; set; }

        public bool NebimIntegrator { get; set; }
        public string NebimV3Path { get; set; }
        public string NebimV3Ip { get; set; }
        public string NebimV3UserName { get; set; }
        public string NebimV3UserGroup { get; set; }
        public string NebimV3PassWord { get; set; }
        public string NebimV3DatabaseName { get; set; }
        public string AppName { get; set; }
        public string Version { get; set; }
        public string VersionWebAdress { get; set; }
    }
}
