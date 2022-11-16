using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Quote2022
{
    public static class Download
    {
        #region ==============  Nanex Symbols  =================
        public static string[] SymbolsNanex_Download(Action<string> showStatusAction)
        {
            var timeStamp = DateTime.Today.ToString("yyyyMMdd");
            var files = new List<string>();
            showStatusAction("Nanex Symbols Downloading ...");
            foreach (var exchange in Settings.NanexExchanges)
            {
                var file = string.Format(Settings.SymbolsNanexTemplateFile, exchange, timeStamp);
                if (File.Exists(file)) File.Delete(file);
                var url = string.Format(Settings.SymbolsNanexTemplateUrl, exchange, "e"); // e - Equities
                DownloadPage(url, file);
                files.Add(file);
            }
            showStatusAction("Nanex Symbols Downloaded");
            return files.ToArray();
        }
        #endregion

        private static string DownloadPage(string url, string filename)
        {
            string response = null;
            using (var wc = new WebClient())
            {
                try
                {
                    var bb = wc.DownloadData(url);
                    response = Encoding.UTF8.GetString(bb);
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
