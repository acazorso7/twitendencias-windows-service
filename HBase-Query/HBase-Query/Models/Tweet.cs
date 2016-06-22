using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HBase_Query.Models
{
    public class Tweet
    {
        public string idTweet { get; set; }
        public string user { get; set; }

        public string location { get; set; }

        public string numFollowers { get; set; }

        public string tweet { get; set; }

        public string posScore { get; set; }

        public string negScore { get; set; }

        public string sentiment { get; set; }
    }
}