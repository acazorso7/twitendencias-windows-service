using HBase_Query.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace HBase_Query
{
    [ServiceContract]
    public interface IServiceHBase
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]

        MoodsTweets GetTop10PositiveNegativeTweets(string key);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]

        Ratio GetPositiveAndNegativeRatioByDate(string key, string category);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]

        CategoricalTweets GetTweetsByCategoryAndDate(string date, string category);


        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]

        CategoricalTweets GetTweetsByCategory(string category);
    }
}
