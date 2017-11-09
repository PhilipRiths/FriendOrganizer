using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.RestClient
{ 
    public class RestClient
    { 
        public string EndPoint { get; set; }
        public HttpVerb httpMethod { get; set; }

        public RestClient()
        {
            EndPoint = string.Empty;
            httpMethod = HttpVerb.GET;
        }

        public string MakeRequest()
        {
            string strResponseValue = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(EndPoint);

            request.Method = httpMethod.ToString();

            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())

            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException();

                }
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            strResponseValue = reader.ReadToEnd();
                        }
                    }
                }
            }
            return strResponseValue;
        }
    }
}
