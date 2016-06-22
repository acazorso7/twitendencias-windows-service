using HBase_Query.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HBase_Query.Models
{
    public class TweetExtended : TweetBase
    {

        public string location { get; set; }

        public string numFollowers { get; set; }

        public string tweet { get; set; }

        public string sentiment { get; set; }
    }
}