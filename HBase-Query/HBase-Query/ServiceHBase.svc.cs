using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
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

        public MoodsTweets GetTop10PositiveNegativeTweets(string table, string key)
        {
            WriteToFile("GetTop10PositiveNegativeTweets", "Start", table, key);

            const string Positive = "Positive";
            const string Negative = "Negative";

            string url = "http://" + HBaseIp + "/" + table + "/" + key;
            var data = ConnectToHBaseService(url);

            MoodsTweets moodsTweets = new MoodsTweets();
            moodsTweets.negativeComments = new List<Tweet>();
            moodsTweets.positiveComments = new List<Tweet>();

            foreach (Row row in data.Row)
            {
                foreach (Cell cell in row.Cell)
                {
                    try
                    {
                        Tweet moodObj = JsonConvert.DeserializeObject<Tweet>(this.DecodeBase64(cell.dollar));
                        if(moodObj.sentiment == Positive)
                        {
                            moodsTweets.positiveComments.Add(moodObj);
                        }
                        else if(moodObj.sentiment == Negative)
                        {
                            moodsTweets.negativeComments.Add(moodObj);
                        }
                    }
                    catch(Exception ex)
                    {
                        WriteToFile("GetTop10PositiveNegativeTweets", "Exception: "+ex.ToString(), table, key);
                    }
                }
            }

            moodsTweets.positiveComments = moodsTweets.positiveComments.OrderBy(nc => nc.posScore).Take(10).ToList();
            moodsTweets.negativeComments = moodsTweets.negativeComments.OrderBy(nc => nc.negScore).Take(10).ToList();

            WriteToFile("GetTop10PositiveNegativeTweets", "End", table, key);

            return moodsTweets;
        }

        public CategoricalTweets GetTweetsByCategoryAndDay(string table, string category, string date = "")
        {
            string key = date == string.Empty ? category : category + "-" + date;
            string url = "http://" + HBaseIp + "/" + table + "/" + key;

            WriteToFile("GetTweetsByCategoryAndDay", "Start", table, key);

            var data = ConnectToHBaseService(url);

            CategoricalTweets categoricalTweets = new CategoricalTweets();
            categoricalTweets.date = date;
            categoricalTweets.category = category;
            categoricalTweets.tweets = new List<Tweet>();

            foreach (Row row in data.Row)
            {
                foreach (Cell cell in row.Cell)
                {
                    try
                    {
                        Tweet moodObj = JsonConvert.DeserializeObject<Tweet>(this.DecodeBase64(cell.dollar));
                        categoricalTweets.tweets.Add(moodObj);
                    }
                    catch (Exception ex)
                    {
                        WriteToFile("GetTweetsByCategoryAndDay", "Exception: " + ex.ToString(), table, key);
                    }
                }
            }

            WriteToFile("GetTweetsByCategoryAndDay", "End", table, key);

            return categoricalTweets;
        }

        public CategoricalTweets GetTweetsByCategory(string table, string category)
        {
            WriteToFile("GetTweetsByCategory", "Start", table, category);
            WriteToFile("GetTweetsByCategory", "Go To GetTweetsByCategoryAndDay", table, category);
            return GetTweetsByCategoryAndDay(table, category);
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
