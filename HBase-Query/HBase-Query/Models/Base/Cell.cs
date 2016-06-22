using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HBase_Query.Models.Base
{
    public class Cell
    {
        public string column { get; set; }
        public string timestamp { get; set; }

        [JsonProperty("$")]
        public string dollar { get; set; }
    }
}