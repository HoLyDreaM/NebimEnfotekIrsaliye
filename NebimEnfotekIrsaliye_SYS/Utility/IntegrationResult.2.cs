using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebimEnfotekIrsaliye_SYS.Utility
{
    public class IntegrationResult<T> : IntegrationResult where T : class
    {
        public string ReturnCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
