using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OfficeOpenXml.ConditionalFormatting;

namespace Quote2022.Actions.TradingView
{
    public static class WebArchive_Profile
    {
        #region =============  Download Data  ===============
        public static void DownloadData(Action<string> showStatusAction)
        {
            var urlTemplate = "https://www.tradingview.com/symbols/{0}/";
            var filenameTemplate = @"E:\Quote\WebArchive\Screener\TradingView\Profiles\TvProfile_{0}_{1}.html";

            var ids = new Dictionary<string, object>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "select * from SymbolsEoddata where (TvType is null or ((isnull(TvType,'') not in ('warrant','right','dr', 'structured') and TvSubtype is null))) and TradingViewSymbol is not null";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                    {
                        var id = (string)rdr["Exchange"] + "-" + (string)rdr["TradingViewSymbol"];
                        ids.Add(id, null);
                    }
            }

            foreach (var kvp in ids)
            {
                var exchangeAndSymbol = kvp.Key.Replace("-", "_").Replace(@"/","^");
                var url = string.Format(urlTemplate, kvp.Key);
                var sourceUrl = System.Net.WebUtility.UrlEncode(url);

                var requestUrl = @"https://web.archive.org/__wb/sparkline?output=json&url=" + sourceUrl + @"&collection=web";
                var yearsJson = Actions.Download.GetString(requestUrl);
                var ooYearList = JsonConvert.DeserializeObject<cYearList>(yearsJson);

                var years = ooYearList.status.Keys.OrderBy(a => a).ToArray();
                foreach (var year in years)
                {
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
                                    var filename = string.Format(filenameTemplate, exchangeAndSymbol, id);
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

            }

            // showStatusAction($"TradingView.WebArchive_Screener finished for {Path.GetFileName(filenameTemplate)}");
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
    }
}
