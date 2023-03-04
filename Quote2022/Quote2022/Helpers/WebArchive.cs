using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quote2022.Actions;

namespace Quote2022.Helpers
{
    public class WebArchive
    {
        public static void DownloadData(string url, string filenameTemplate, Action<string> showStatusAction)
        {
            var sourceUrl = System.Net.WebUtility.UrlEncode(url);

            var requestUrl = @"https://web.archive.org/__wb/sparkline?output=json&url=" + sourceUrl + @"&collection=web";
            var yearsJson = Download.GetString(requestUrl);
            var ooYearList = JsonConvert.DeserializeObject<cYearList>(yearsJson);

            var years = ooYearList.status.Keys.OrderBy(a => a).ToArray();
            foreach (var year in years)
            {
                requestUrl = $"https://web.archive.org/__wb/calendarcaptures/2?url={sourceUrl}&date={year}&groupby=day";
                var daysJson = Download.GetString(requestUrl);
                var ooDayList = JsonConvert.DeserializeObject<cDayList>(daysJson);

                foreach (var day in ooDayList.items)
                {
                    if (day[1] == 200)
                    {
                        var timeStamp = $"{year}{day[0]:0000}";
                        requestUrl = $"https://web.archive.org/__wb/calendarcaptures/2?url={sourceUrl}&date={timeStamp}";
                        var dayItemsJson = Download.GetString(requestUrl);
                        var dayItems = JsonConvert.DeserializeObject<cDayList>(dayItemsJson);

                        var dayItem = dayItems.items[dayItems.items.Length - 1];
                        if (dayItem[1] == 200)
                        {
                            var filename = string.Format(filenameTemplate, timeStamp);
                            if (!File.Exists(filename))
                            {
                                requestUrl = $"https://web.archive.org/web/{timeStamp}{dayItem[0]:000000}/{sourceUrl}";

                                showStatusAction($"WebArchive.DownloadData. Download to {Path.GetFileName(filename)}");
                                Download.DownloadPage(requestUrl, filename);
                                if (File.Exists(filename))
                                {
                                    var fileTime = DateTime.ParseExact($"{timeStamp}{dayItem[0]:000000}",
                                        "yyyyMMddHHmmss",
                                        CultureInfo.InvariantCulture);
                                    File.SetCreationTime(filename, fileTime);
                                    File.SetLastWriteTime(filename, fileTime);
                                }
                            }
                        }
                        // else
                           // throw new Exception($"Check day items for {dayItem[0]} of {day[0]} day of {year} year. Url: {url}");
                    }
                    /*else
                    {
                        throw new Exception($"Check day items for {day[0]} day of {year} year. Url: {url}");
                    }*/
                }
            }
            showStatusAction($"WebArchive.DownloadData finished for {Path.GetFileName(filenameTemplate)}");
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
    }
}
