using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Activation;
using System.Text;
using System.Linq;
using HBase_Query.Models;
using HBase_Query.Models.Base;

namespace HBase_Query
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ServiceHBase : IServiceHBase
    {
        public const string HBaseIp = "192.168.1.43:8080";

        public MoodsTweets GetTop10PositiveNegativeTweets(string key)
        {
            WriteToFile("GetTop10PositiveNegativeTweets", "Start", "mood", key);

            const string Positive = "Positive";
            const string Negative = "Negative";

            string url = "http://" + HBaseIp + "/mood/" + key;
            var data = ConnectToHBaseService(url);

            MoodsTweets moodsTweets = new MoodsTweets();
            moodsTweets.negativeComments = new List<TweetExtended>();
            moodsTweets.positiveComments = new List<TweetExtended>();

            foreach (Row row in data.Row)
            {
                foreach (Cell cell in row.Cell)
                {
                    try
                    {
                        TweetExtended tweetExtended = JsonConvert.DeserializeObject<TweetExtended>(this.DecodeBase64(cell.dollar));
                        if (tweetExtended.sentiment == Positive)
                        {
                            moodsTweets.positiveComments.Add(tweetExtended);
                        }
                        else if (tweetExtended.sentiment == Negative)
                        {
                            moodsTweets.negativeComments.Add(tweetExtended);
                        }
                    }
                    catch(Exception ex)
                    {
                        WriteToFile("GetTop10PositiveNegativeTweets", "Exception: "+ex.ToString(), "mood", key);
                    }
                }
            }

            moodsTweets.positiveComments = moodsTweets.positiveComments.OrderBy(nc => nc.posScore).Take(10).ToList();
            moodsTweets.negativeComments = moodsTweets.negativeComments.OrderBy(nc => nc.negScore).Take(10).ToList();

            WriteToFile("GetTop10PositiveNegativeTweets", "End", "mood", key);

            return moodsTweets;
        }


        public Ratio GetPositiveAndNegativeRatioByDate(string key, string category)
        {
            WriteToFile("GetPositiveAndNegativeRatioByDate", "Start", "minmood", key);

            string url = "http://" + HBaseIp + "/minmood/" + key;
            var data = ConnectToHBaseService(url);

            Ratio positiveNegativeRatio = new Ratio();
            positiveNegativeRatio.negative = 0;
            positiveNegativeRatio.positive = 0;

            foreach (Row row in data.Row)
            {
                foreach (Cell cell in row.Cell)
                {
                    try
                    {
                        TweetBase tweetBase = JsonConvert.DeserializeObject<TweetBase>(this.DecodeBase64(cell.dollar));
                        if (tweetBase.category == category || category == "General")
                        {
                            decimal positiveScore = tweetBase.posScore == string.Empty ? 0 : Convert.ToDecimal(tweetBase.posScore);
                            decimal negativeScore = tweetBase.negScore == string.Empty ? 0 : Convert.ToDecimal(tweetBase.negScore);
                            if (positiveScore > negativeScore)
                            {
                                positiveNegativeRatio.positive++;
                            }
                            else if (positiveScore < negativeScore)
                            {
                                positiveNegativeRatio.negative++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToFile("GetPositiveAndNegativeRatioByDate", "Exception: " + ex.ToString(), "minmood", key);
                    }
                }
            }

            WriteToFile("GetPositiveAndNegativeRatioByDate", "End", "minmood", key);

            return positiveNegativeRatio;
        }

        public CategoricalTweets GetTweetsByCategoryAndDate(string category, string date = "")
        {
            const string Positive = "Positive";
            const string Negative = "Negative";

            string key = date == string.Empty ? category+"-*" : category + "-" + date;
            string url = "http://" + HBaseIp + "/categories/" + key;

            WriteToFile("GetTweetsByCategoryAndDate", "Start", "categories", key);

            var data = ConnectToHBaseService(url);

            CategoricalTweets categoricalTweets = new CategoricalTweets();
            categoricalTweets.date = date;
            categoricalTweets.category = category;
            categoricalTweets.moodTweets = new MoodsTweets();
            categoricalTweets.moodTweets.negativeComments = new List<TweetExtended>();
            categoricalTweets.moodTweets.positiveComments = new List<TweetExtended>();

            foreach (Row row in data.Row)
            {
                foreach (Cell cell in row.Cell)
                {
                    try
                    {
                        TweetExtended tweetExtended = JsonConvert.DeserializeObject<TweetExtended>(this.DecodeBase64(cell.dollar));
                        if (tweetExtended.sentiment == Positive)
                        {
                            categoricalTweets.moodTweets.positiveComments.Add(tweetExtended);
                        }
                        else if (tweetExtended.sentiment == Negative)
                        {
                            categoricalTweets.moodTweets.negativeComments.Add(tweetExtended);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToFile("GetTweetsByCategoryAndDate", "Exception: " + ex.ToString(), "categories", key);
                    }
                }
            }

            categoricalTweets.moodTweets.positiveComments = categoricalTweets.moodTweets.positiveComments.OrderBy(nc => nc.posScore).Take(10).ToList();
            categoricalTweets.moodTweets.negativeComments = categoricalTweets.moodTweets.negativeComments.OrderBy(nc => nc.negScore).Take(10).ToList();

            WriteToFile("GetTweetsByCategoryAndDate", "End", "categories", key);

            return categoricalTweets;
        }

        public CategoricalTweets GetTweetsByCategory(string category)
        {
            WriteToFile("GetTweetsByCategory", "Start", "categories", category);
            WriteToFile("GetTweetsByCategory", "Go To GetTweetsByCategoryAndDay", "categories", category);
            return GetTweetsByCategoryAndDate("categories", category);
        }

        private string DecodeBase64(string value)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }

        private RootObject ConnectToHBaseService(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "application/json";
            request.UserAgent = "curl/7.37.0";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            string json = reader.ReadLine();

            return JsonConvert.DeserializeObject<RootObject>(json);
        }

        private void WriteToFile(string text, string status, string table, string key)
        {
            DateTime currentDate = DateTime.Now;
            string message = string.Format("{0} {1}: {2} table: {3}, key {4}", 
                currentDate.ToString("dd/MM/yyyy"), status, text, table, key);

            string path = "ServiceLog_"+ currentDate.ToString("dd_MM_yyyy") + ".txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
    }
}
