using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HBase_Query.Models.Base
{
    public class TweetBase
    {
        public string idTweet { get; set; }
        public string user { get; set; }

        public string posScore { get; set; }

        public string negScore { get; set; }

        public string category { get; set; }
    }
}