using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static class Download
    {
        #region ==================  SymbolsQuantumonlineList_Download  ==========================

        public static void SymbolsQuantumonlineList_Rename(Action<string> showStatusAction)
        {
            var oldPath = @"E:\Temp\Quote\QuantumonlineList\";
            var newPath = @"E:\Temp\Quote\QuantumonlineListNew\";
            var levelPathes = new List<string>();

            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);

            for (var k = 0; k < 20; k++)
            {
                var levelPath = newPath + "Level" + k.ToString("D2") + @"\";
                levelPathes.Add(levelPath);
                if (!Directory.Exists(levelPath))
                    Directory.CreateDirectory(levelPath);
            }

            var files = Directory.GetFiles(oldPath, "*.html");
            for (var k = 0; k < files.Length; k++)
            {
                var fn = Path.GetFileNameWithoutExtension(files[k]);
                var newFn = levelPathes[fn.Length - 2] + Path.GetFileName(files[k]);
                File.Move(files[k], newFn);
            }
        }
        #endregion

        #region ==================  SymbolsQuantumonlineProfiles_Download  ==========================

        public static void SymbolsQuantumonlineProfiles_Download(Action<string> showStatusAction)
        {
            const string folderList = @"E:\Quote\WebData\Symbols\Quantumonline\";
            const string folderProfile = @"E:\Quote\WebData\Symbols\Quantumonline\Profiles\";

            var items = new Dictionary<string, SymbolsQuantumonlineList>();
            var zipFiles = Directory.GetFiles(folderList, "*.zip");
            foreach (var zipFile in zipFiles)
                using (var zip = new ZipReader(zipFile))
                    foreach (var file in zip.Where(a => a.FullName.EndsWith(".html", true, CultureInfo.InvariantCulture)))
                    {
                        if (file.FileNameWithoutExtension.Replace("%", "[%]").Substring(1).Length <= 20)
                        {
                            var newItems = Parse.SymbolsQuantumonlineContent_Parse(file.Content, file.FileNameWithoutExtension);
                            foreach (var item in newItems)
                                if (!items.ContainsKey(item.ToString()))
                                    items.Add(item.ToString(), item);
                            //if (!urls.ContainsKey(item.Url))
                            //  urls.Add(item.Url, null);
                            // break;
                        }
                    }

            //var urls = items.Values.Select(a=>a.Url).ToList();
            //var aa1 = urls.Distinct().ToList();

            var urls = new Dictionary<string, string>();
            foreach (var item in items.Values)
            {
                if (item.Url.StartsWith("search.cfm?tickersymbol=") && item.Url.EndsWith("&sopt=symbol"))
                {
                    var symbol = item.Url.Substring(24, item.Url.Length - 36).Replace("*", "#");
                    var url = @"https://www.quantumonline.com/" + item.Url;
                    if (!urls.ContainsKey(url))
                        urls.Add(url, folderProfile + @"\s" + symbol + ".html");
                }
                else
                    throw new Exception("Check ");
            }

            Parallel.ForEach(urls, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (kvp) =>
            {
                DownloadPage(kvp.Key, kvp.Value);
            });

        }
        #endregion

        #region ==================  SymbolsQuantumonlineList_Download  ==========================
        public static void SymbolsQuantumonline_Download(Action<string> showStatusAction)
        {
            var urlChars = new char[]
            {
                (char) 32, (char) 33, (char) 34, (char) 36, (char) 37, (char) 38, /*(char) 39,*/ (char) 40, (char) 41,
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
            var toDownloadKeysTemplate = @"E:\Temp\Quote\Test\ToDownloadKeys-{0}.txt";
            var finishedKeysTemplate = @"E:\Temp\Quote\Test\FinishedKeys-{0}.txt";
            var level = 0;
            var finishedUrlKeys = new List<string>();
            var keys = new List<string> {""};
            while (keys.Count != 0)
            {
                if (File.Exists(string.Format(toDownloadKeysTemplate, level.ToString())))
                {
                    var fileName = string.Format(toDownloadKeysTemplate, level.ToString());
                    keys.Clear();
                    foreach (var line in File.ReadAllLines(fileName))
                        keys.Add(line);

                    fileName = string.Format(finishedKeysTemplate, level.ToString());
                    finishedUrlKeys.Clear();
                    foreach (var line in File.ReadAllLines(fileName))
                        finishedUrlKeys.Add(line);

                    Debug.Print($"SymbolsQuantumonline_Download. Downloaded {keys.Count} items for level: {level}. FinishedUrlKeys: {finishedUrlKeys.Count}");
                }
                else
                {
                    var urlKeys = new Dictionary<string, string>();
                    foreach (var key in keys)
                    foreach (var c in urlChars)
                    {
                        var newKey = c.ToString() + key;
                        if (!urlKeys.ContainsKey(newKey) && finishedUrlKeys.FirstOrDefault(f => newKey.Contains(f)) == null && GetLenUrl(newKey)<=20)
                            urlKeys.Add(newKey, GetFileName(newKey));

                        newKey = key + c.ToString();
                        if (!urlKeys.ContainsKey(newKey) && finishedUrlKeys.FirstOrDefault(f => newKey.Contains(f)) == null && GetLenUrl(newKey) <= 20)
                            urlKeys.Add(newKey, GetFileName(newKey));
                    }

                    if (urlKeys.ContainsKey(" "))
                        urlKeys.Remove(" ");
                    if (urlKeys.ContainsKey("  "))
                        urlKeys.Remove("  ");

                    showStatusAction($"QuantumonlineSymbols downloading... Level: {level}. Urls: {urlKeys.Count}");
                    var toDownloadKeys = urlKeys.Where(a => !File.Exists(a.Value)).ToArray();
                    Debug.Print($"SymbolsQuantumonline_Download. Downloading {toDownloadKeys.Length} from {urlKeys.Count} items for level: {level}. FinishedUrlKeys: {finishedUrlKeys.Count}. {DateTime.Now}");
                    Parallel.ForEach(toDownloadKeys, new ParallelOptions {MaxDegreeOfParallelism = 10}, (kvp) =>
                    {
                        DownloadPage(GetUrl(kvp.Key), kvp.Value);
                    });

                    keys.Clear();
                    foreach (var urlKey in urlKeys)
                    {
                        var items = Parse.SymbolsQuantumonlineContent_Parse(File.ReadAllText(urlKey.Value), urlKey.Value);
                        var checkedItems = items.Where(a => (a.HtmlName + " ").IndexOf(urlKey.Key, StringComparison.InvariantCultureIgnoreCase) == -1).ToArray();
                        if (checkedItems.Length != 0)
                        {
                            Debug.Print($"Not Checked. Key: {urlKey.Key}. Name: {checkedItems[0].HtmlName}");
                            throw new Exception($"Not Checked. Key: {urlKey.Key}. Name: {checkedItems[0].HtmlName}");
                        }

                        if (items.Count >= 200)
                            keys.Add(urlKey.Key);
                        else if (items.Count > 0)
                            finishedUrlKeys.Add(urlKey.Key);
                    }

                    var fileName = string.Format(toDownloadKeysTemplate, level.ToString());
                    System.IO.File.WriteAllLines(fileName, keys);
                    fileName = string.Format(finishedKeysTemplate, level.ToString());
                    System.IO.File.WriteAllLines(fileName, finishedUrlKeys);

                    // Debug.Print($"MaxLength: {SymbolsQuantumonlineList.n1}, {SymbolsQuantumonlineList.n2}, {SymbolsQuantumonlineList.n3}, {SymbolsQuantumonlineList.n4}");
                }

                level++;
            }

            Debug.Print($"FINISHED!!! {DateTime.Now}");
            showStatusAction($"FINISHED!!!");

            string GetUrl(string urlKey) => string.Format(urlTemplate, Uri.EscapeDataString(urlKey.Replace("%", "[%]")));
            int GetLenUrl(string urlKey) => urlKey.Replace("%", "[%]").Length;

            string GetFileName(string urlKey)
            {
                var cc = urlKey.ToCharArray();
                for (var k = 0; k < cc.Length; k++)
                {
                    if (urlAndFileCharXref.ContainsKey(cc[k]))
                        cc[k] = urlAndFileCharXref[cc[k]];
                }
                return string.Format(fileTemplate, new string(cc));
            }
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
