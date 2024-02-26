using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebimEnfotekIrsaliye_SYS.Data
{
   public class RoundsmanInfo
    {
        public Guid ShipmentHeaderID { get; set; }
        public string RoundsmanCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FirstLastName { get; set; }
        public string VehiclePlateNum { get; set; }
    }
}
