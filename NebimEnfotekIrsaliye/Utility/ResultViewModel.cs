using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebimEnfotekIrsaliye.Data
{
    public class ResultViewModel<T> where T : class
    {
        public string ReturnCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
