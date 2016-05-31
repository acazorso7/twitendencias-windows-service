using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Linq;

namespace HBase_Query
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public MoodLists GetTop10PositiveNegativeTweets(string table, string key)
        {
            const string Positive = "Positive";
            const string Negative = "Negative";

            var request = (HttpWebRequest)WebRequest.Create("http://192.168.1.43:8080/"+table+"/"+key);
            request.Accept = "application/json";
            request.UserAgent = "curl/7.37.0";

            //github test  sds

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            string json = reader.ReadLine();

            var result = JsonConvert.DeserializeObject<RootObject>(json);

            MoodLists listMoods = new MoodLists();
            listMoods.negativeComments = new List<Mood>();
            listMoods.positiveComments = new List<Mood>();

            foreach (Row row in result.Row)
            {
                foreach (Cell cell in row.Cell)
                {
                    try
                    {
                        Mood moodObj = JsonConvert.DeserializeObject<Mood>(this.DecodeBase64(cell.dollar));
                        if(moodObj.sentiment == Positive)
                        {
                            listMoods.positiveComments.Add(moodObj);
                        }
                        else if(moodObj.sentiment == Negative)
                        {
                            listMoods.negativeComments.Add(moodObj);
                        }
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }

            listMoods.positiveComments = listMoods.positiveComments.OrderBy(nc => nc.posScore).Take(10).ToList();
            listMoods.negativeComments = listMoods.negativeComments.OrderBy(nc => nc.negScore).Take(10).ToList();

            return listMoods;
        }

        public string DecodeBase64(string value)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }
    }

    public class Cell
    {
        public string column { get; set; }
        public string timestamp { get; set; }

        [JsonProperty("$")]
        public string dollar { get; set; }
    }

    public class MoodLists
    {
        public List<Mood> positiveComments { get; set; }

        public List<Mood> negativeComments { get; set; }
    }

    public class Mood
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

    public class Row
    {
        public string key { get; set; }
        public List<Cell> Cell { get; set; }
    }

    public class RootObject
    {
        public List<Row> Row { get; set; }
    }
}
