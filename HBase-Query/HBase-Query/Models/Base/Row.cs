using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HBase_Query.Models.Base
{
    public class Row
    {
        public string key { get; set; }
        public List<Cell> Cell { get; set; }
    }
}