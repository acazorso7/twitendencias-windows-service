using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace HBase_Query
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public RootObject GetTweets(string table, string key)
        {
            var request = (HttpWebRequest)WebRequest.Create("http://192.168.1.131:8080/"+table+"/"+key);
            request.Accept = "application/json";
            request.UserAgent = "curl/7.37.0";

            //github test  sds
            //Push
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            string json = reader.ReadLine();

            var result = JsonConvert.DeserializeObject<RootObject>(json);

            foreach (Row row in result.Row)
            {
                row.key = this.DecodeBase64(row.key);
                foreach (Cell cell in row.Cell)
                {
                    cell.column = this.DecodeBase64(cell.column);
                    cell.dollar = this.DecodeBase64(cell.dollar);
                }
            }

            return result;


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
