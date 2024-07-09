using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Terranova_WebAPI.Helpers
{
    public class WebHelper
    {
        public async Task<string> HttpPostAsync(string url, string payload)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] data = Encoding.UTF8.GetBytes(payload);
            req.ContentLength = data.Length;
            using (Stream responseStream = req.GetRequestStream())
            {
                responseStream.Write(data, 0, data.Length);
                responseStream.Close();
            }

            // Process the response
            var resp = await req.GetResponseAsync();

            if (resp == null)
                // No HTTP response content obtained for web POST request;
                return null;

            StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
            string retval = sr.ReadToEnd().Trim();
            return retval;
        }
    }
}