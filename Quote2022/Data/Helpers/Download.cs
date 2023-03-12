using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace Data.Helpers
{
    public static class Download
    {
        public static string DownloadPage_POST(string url, string filename, object parameters)
        {
            // see https://stackoverflow.com/questions/5401501/how-to-post-data-to-specific-url-using-webclient-in-c-sharp
            string response = null;
            using (var wc = new WebClient())
            {
                if (ServicePointManager.DefaultConnectionLimit != int.MaxValue)
                {
                    ServicePointManager.DefaultConnectionLimit = int.MaxValue;
                    WebRequest.DefaultWebProxy = null;
                }
                wc.Proxy = null;

                try
                {
                    if (parameters is NameValueCollection nvc)
                        response = Encoding.UTF8.GetString(wc.UploadValues(url, "POST", nvc));
                    else if (parameters is string json)
                        response = wc.UploadString(url, "POST", json);
                    else
                        throw new Exception("DownloadPage_POST. Invalid type of request parameters");
                }
                catch (Exception ex)
                {
                    if (ex is WebException webEx && webEx.Response is HttpWebResponse webResponse)
                    {
                        if (webResponse.StatusCode == HttpStatusCode.NotFound)
                            response = "NotFound";
                        else if (webResponse.StatusCode == HttpStatusCode.Moved)
                            response = "Moved";
                    }
                    else
                        throw ex;
                }
            }

            if (!string.IsNullOrEmpty(filename))
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                File.WriteAllText(filename, response, Encoding.UTF8);
            }

            return response;
        }


    }
}
