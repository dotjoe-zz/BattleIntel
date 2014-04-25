using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupMe
{
    using GroupMe.Responses;
    using Newtonsoft.Json;
    using System.IO;
    using System.Net;

    public class GroupMeService
    {
        private const string apiBaseUrl = @"https://api.groupme.com/v3";
        private string accessToken;

        public GroupMeService(string accessToken)
        {
            this.accessToken = accessToken;
        }

        public IList<Group> GroupsIndex(int page = 1, int per_page = 10)
        {
            string url = GetApiUrl("groups") + string.Format("&page={0}&per_page={1}", page, per_page);
            return GET<IList<Group>>(url);
        }

        /// <summary>
        /// concatenates the base api url, the action, and the token query string param.
        /// Append additional query string with a leading ampersand "&" delimiter
        /// </summary>
        /// <param name="action">the action to hit, does not need leading / </param>
        /// <returns></returns>
        private string GetApiUrl(string action)
        {
            return string.Format("{0}/{1}?token={2}", apiBaseUrl, action.TrimStart('/'), accessToken);
        }

        private T GET<T>(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            
            request.Method = "GET";
            request.Accept = "application/json";

            ResponseEnvelope<T> envelope;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var obj = reader.ReadToEnd();
                envelope = JsonConvert.DeserializeObject<ResponseEnvelope<T>>(obj);
            }

            //TODO handle response status codes
            //if (envelope.meta.code == 200) 
                return envelope.response;
        }

        private T POST<T>(string url, object data)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.Accept = "application/json";

            if (data != null)
            {
                string dataString = JsonConvert.SerializeObject(data);
                byte[] bytes = new ASCIIEncoding().GetBytes(dataString);

                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = bytes.Length;

                Stream reqStream = request.GetRequestStream();
                reqStream.Write(bytes, 0, bytes.Length);
                reqStream.Close();
            }

            ResponseEnvelope<T> envelope;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var obj = reader.ReadToEnd();
                envelope = JsonConvert.DeserializeObject<ResponseEnvelope<T>>(obj);
            }

            //TODO handle response status codes
            //if (envelope.meta.code == 200) 
            return envelope.response;
        }
    }

}

namespace GroupMe.Responses { 

    class ResponseEnvelope<T> 
    {
        public T response { get; set; }
        public Meta meta { get; set; }
    }

    class Meta
    {
        public int code { get; set; }
        public List<string> errors { get; set; }
    }

    public class Group
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class GroupMessages
    {
        public int count { get; set; }
        public List<Message> messages { get; set; }
    }

    public class Message
    {
        public string id { get; set; }
        public int created_at { get; set; }
        public string user_id { get; set; }
        public string group_id { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public bool system { get; set; }
        //public List<Attachment> attachments { get; set; }
    }
}


