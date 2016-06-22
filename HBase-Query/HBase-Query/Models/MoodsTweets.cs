using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HBase_Query.Models
{
    public class MoodsTweets
    {
        public List<TweetExtended> positiveComments { get; set; }

        public List<TweetExtended> negativeComments { get; set; }
    }
}