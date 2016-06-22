using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HBase_Query.Models
{
    public class CategoricalTweets
    {
        public string category { get; set; }

        public string date { get; set; }

        public MoodsTweets moodTweets { get; set; }
    }
}