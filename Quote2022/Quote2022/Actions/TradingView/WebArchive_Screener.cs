using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OfficeOpenXml.ConditionalFormatting;

namespace Quote2022.Actions.TradingView
{
    public static class WebArchive_Screener
    {
        #region =============  Download Html Data  ===============
        public static void DownloadHtmlData(Action<string> showStatusAction)
        {
            var url = @"https://scanner.tradingview.com/america/scan";
            //            var filenameTemplate =$"E:\\Quote\\WebArchive\\Symbols\\Eoddata\\{exchange}\\{exchange}_{letter}_{{0}}.htm";
            var filenameTemplate = @"E:\Quote\WebArchive\Screener\TradingView\America_Scan\Html\TvScreener_{0}.htm";

            var sourceUrl = System.Net.WebUtility.UrlEncode(url);

            var requestUrl = @"https://web.archive.org/__wb/sparkline?output=json&url=" + sourceUrl + @"&collection=web";
            var yearsJson = Actions.Download.GetString(requestUrl);
            var ooYearList = JsonConvert.DeserializeObject<cYearList>(yearsJson);

            var years = ooYearList.status.Keys.OrderBy(a => a).ToArray();
            foreach (var year in years)
            {
                if (!(year == "2022" || year == "2023")) continue;

                requestUrl = $"https://web.archive.org/__wb/calendarcaptures/2?url={sourceUrl}&date={year}&groupby=day";
                var daysJson = Actions.Download.GetString(requestUrl);
                var ooDayList = JsonConvert.DeserializeObject<cDayList>(daysJson.Replace("\"-\"", "200"));

                foreach (var day in ooDayList.items)
                {
                    if (day[1] == 200)
                    {
                        var timeStamp = $"{year}{day[0]:0000}";
                        requestUrl = $"https://web.archive.org/__wb/calendarcaptures/2?url={sourceUrl}&date={timeStamp}";
                        var dayItemsJson = Actions.Download.GetString(requestUrl);
                        var dayItems = JsonConvert.DeserializeObject<cDayList>(dayItemsJson.Replace("\"-\"", "200"));

                        foreach (var dayItem in dayItems.items)
                        {
                            if (dayItem[1] == 200)
                            {
                                var id = $"{timeStamp}{dayItem[0]:000000}";
                                var filename = string.Format(filenameTemplate, id);
                                if (!File.Exists(filename))
                                {
                                    requestUrl = $"https://web.archive.org/web/{id}/{sourceUrl}";

                                    showStatusAction($"TradingView.WebArchive_Screener.DownloadData. Download to {Path.GetFileName(filename)}");
                                    Actions.Download.DownloadPage(requestUrl, filename);
                                    if (File.Exists(filename))
                                    {
                                        var fileTime = DateTime.ParseExact($"{id}", "yyyyMMddHHmmss",
                                            CultureInfo.InvariantCulture);
                                        File.SetCreationTime(filename, fileTime);
                                        File.SetLastWriteTime(filename, fileTime);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            showStatusAction($"TradingView.WebArchive_Screener finished for {Path.GetFileName(filenameTemplate)}");
        }

        public class cYearList
        {
            public Dictionary<string, object> status;
            public string first_ts;
            public string last_ts;
        }
        public class cDayList
        {
            public int[][] items;
        }
        #endregion

        #region =============  Download Json Data  ===============
        public static void DownloadJsonData(Action<string> showStatusAction)
        {
            var sourceFolder = @"E:\Quote\WebArchive\Screener\TradingView\America_Scan\Html\";
            var destinationFolder = @"E:\Quote\WebArchive\Screener\TradingView\America_Scan\Json\";

            showStatusAction($"TradingView.WebArchive_Screener.DownloadJsonData. Prepare urls and file names");

            var files = Directory.GetFiles(sourceFolder, "*.htm", SearchOption.AllDirectories);
            var urlsAndFilenames = new List<Tuple<string,string>>();
            foreach (var file in files)
            {
                var s = File.ReadAllText(file);
                var i1 = s.IndexOf("id=\"playback\"", StringComparison.InvariantCultureIgnoreCase);
                var i2 = s.IndexOf("src=\"", i1 + 14, StringComparison.InvariantCultureIgnoreCase);
                var i3 = s.IndexOf("\"", i2 + 6, StringComparison.InvariantCultureIgnoreCase);
                var url = s.Substring(i2 + 5, i3 - i2 - 5);
                var filename = destinationFolder + Path.GetFileNameWithoutExtension(file) + ".json";
                urlsAndFilenames.Add(new Tuple<string, string>(url, filename));
            }

            var cnt = 0;
            foreach (var urlAndFilename in urlsAndFilenames)
            {
                cnt++;

                if (cnt%10 == 0)
                    showStatusAction($"TradingView.WebArchive_Screener.DownloadJsonData. {cnt} from {urlsAndFilenames.Count} items are ready");

                if (!File.Exists(urlAndFilename.Item2))
                    Download.DownloadPage(urlAndFilename.Item1, urlAndFilename.Item2);
            }
            showStatusAction($"TradingView.WebArchive_Screener.DownloadJsonData finished");
        }

        #endregion

        #region =============  Parse Data  ===============
        public class Item
        {
            public string Symbol;
            public string Exchange;
            public DateTime TimeStamp;
            public string Name;
            public float High;
            public float Low;
            public float Close;
            public float Change;
            public float Volume;
        }

        public static void ParseData(Action<string> showStatusAction)
        {
            var folder = @"E:\Quote\WebArchive\Screener\TradingView\America_Scan";
            var files = Directory.GetFiles(folder, "*.htm", SearchOption.AllDirectories);
            var cnt = 0;
            foreach (var file in files)
            {
                cnt++;
                if (cnt % 10 == 0)
                    showStatusAction($"TradingView.WebArchive_Screener.ParseData. Processed {cnt} files from {files.Length}");
                var s = File.ReadAllText(file);

            }

            showStatusAction($"TradingView.WebArchive_Screener.ParseData finished");
        }
        #endregion

    }
}
