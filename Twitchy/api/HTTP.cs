using System.Collections.Specialized;
using System.Net;

namespace Twitchy.api
{
    public static class Http
    {
        public static string Post(string uri, NameValueCollection pairs)
        {
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = client.UploadValues(uri, pairs);
            }
            return System.Text.Encoding.UTF8.GetString(response);
        }

        public static string Get(string uri)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(uri);
            }
        }
    }
}
