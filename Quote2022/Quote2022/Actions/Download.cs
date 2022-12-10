using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Quote2022.Actions
{
    public static class Download
    {
        #region ==============================================

        public static void SymbolsQuantumonline_Download(Action<string> showStatusAction)
        {
            var urlChars = new char[]
            {
                (char) 32, (char) 33, (char) 34, (char) 36, (char) 37, (char) 38, (char) 39, (char) 40, (char) 41,
                (char) 42, (char) 43, (char) 44, (char) 45, (char) 46, (char) 47, (char) 48, (char) 49, (char) 50,
                (char) 51, (char) 52, (char) 53, (char) 54, (char) 55, (char) 56, (char) 57, (char) 58, (char) 63,
                (char) 64, (char) 65, (char) 66, (char) 67, (char) 68, (char) 69, (char) 70, (char) 71, (char) 72,
                (char) 73, (char) 74, (char) 75, (char) 76, (char) 77, (char) 78, (char) 79, (char) 80, (char) 81,
                (char) 82, (char) 83, (char) 84, (char) 85, (char) 86, (char) 87, (char) 88, (char) 89, (char) 90,
                (char) 174, (char) 195, (char) 199, (char) 201, (char) 205, (char) 209, (char) 214, (char) 8211,
                (char) 8212, (char) 8217
            };

            var urlAndFileCharXref = new Dictionary<char, char>()
            {
                {(char) 34, (char) 96}, {(char) 42, (char) 35}, {(char) 47, (char) 61}, {(char) 58, (char) 59},
                {(char) 63, (char) 94}
            };

            /*var fileAndUrlCharXref = new Dictionary<char, char>();
            foreach(var kvp in urlAndFileCharXref)
                fileAndUrlCharXref.Add(kvp.Value, kvp.Key);

            var pathTemplate = @"E:\Temp\Quote\Test\s{0}.txt";
            foreach (var c in urlChars)
            {
                var c1 = urlAndFileCharXref.ContainsKey(c) ? urlAndFileCharXref[c] : c;
                File.Create(string.Format(pathTemplate, c1.ToString()));
            }*/

            var urlTemplate = @"https://www.quantumonline.com/search.cfm?tickersymbol={0}&sopt=sname&1.0.1=Search";
            var fileTemplate = @"E:\Temp\Quote\Test\s{0}.html";
            var level = 0;
            var finishedUrlKeys = new List<string>();
            var keys = new List<string> {""};
            while (level < 100 || keys.Count == 0)
            {
                var urlKeys = new Dictionary<string, string>();
                foreach (var key in keys)
                foreach (var c in urlChars)
                {
                    var newKey = c.ToString() + key;
                    if (!urlKeys.ContainsKey(newKey) && finishedUrlKeys.FirstOrDefault(f => newKey.Contains(f)) == null)
                        urlKeys.Add(newKey, GetFileNameKey(newKey));

                    newKey = key + c.ToString();
                    if (!urlKeys.ContainsKey(newKey) && finishedUrlKeys.FirstOrDefault(f => newKey.Contains(f)) == null)
                        urlKeys.Add(newKey, GetFileNameKey(newKey));
                }

                if (urlKeys.ContainsKey(" "))
                    urlKeys.Remove(" ");
                if (urlKeys.ContainsKey("  "))
                    urlKeys.Remove("  ");

                showStatusAction($"QuantumonlineSymbols downloading... Level: {level}. Urls: {urlKeys.Count}");
                Parallel.ForEach(urlKeys, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (kvp) =>
                {
                    // showStatusAction($"RealEstate list download. Prices: {startPrice}-{endPrice} ('000$). Remain {pages - k2} pages");
                    var url = string.Format(urlTemplate, Uri.EscapeDataString(kvp.Key));
                    var filename = string.Format(fileTemplate, kvp.Value);
                    Debug.Print(url);
                    if (!File.Exists(filename))
                        DownloadPage(url, filename);
                });

                keys.Clear();
                foreach (var urlKey in urlKeys)
                {
                    var filename = string.Format(fileTemplate, urlKey.Value);
                    var items = Parse.SymbolsQuantumonlineList_Parse(filename, showStatusAction);
                    if (items.Count >=200)
                        keys.Add(urlKey.Key);
                    else if (items.Count>0)
                        finishedUrlKeys.Add(urlKey.Key);
                }
                level++;
            }

            string GetFileNameKey(string urlKey)
            {
                var cc = urlKey.ToCharArray();
                for (var k = 0; k < cc.Length; k++)
                {
                    if (urlAndFileCharXref.ContainsKey(cc[k]))
                        cc[k] = urlAndFileCharXref[cc[k]];
                }
                return new string(cc);
            }

            /*Parallel.ForEach(Enumerable.Range(2, pages - 1), new ParallelOptions { MaxDegreeOfParallelism = 10 }, (k2) =>
            {
                showStatusAction($"RealEstate list download. Prices: {startPrice}-{endPrice} ('000$). Remain {pages - k2} pages");
                var url = string.Format(Settings.RealEstateList_TemplateUrl, k2, startPrice * 1000, endPrice * 1000);
                var filename = string.Format(Settings.RealEstateList_FileTemplate, k2, startPrice, endPrice);
                DownloadPage(url, filename);
            });*/
        }

        #endregion
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
