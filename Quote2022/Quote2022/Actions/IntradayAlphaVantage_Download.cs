using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quote2022.Actions
{
    public static class IntradayAlphaVantage_Download
    {
        public class ApiKey
        {
            public string Key;
            public DateTime LastUsed = DateTime.MinValue;
            public string proxy;
        }

        private static ApiKey[] _apiKeys = new[]
        {
            new ApiKey {Key = "VW3GN7E91208316M"},
            new ApiKey {Key = "S14Q8OT8OSR54L1A"},
            new ApiKey {Key = "F0QQ7UQ720NEIC45"},
            new ApiKey {Key = "I9UBCSI1VT4OQ6CH"},
            new ApiKey {Key = "P1DS3CT3MECPS9NT"},
            new ApiKey {Key = "P9YLZ5JNSTZTTMAS"},
            new ApiKey {Key = "TK4Q66GMN8YDXDVZ"},
            new ApiKey {Key = "TXQMV0KYX4WBX7VS"},
            new ApiKey {Key = "QDYJLC03FUZX4VN2"},
            new ApiKey {Key = "HB2ZP18A4CQ1CSL0"},
            new ApiKey {Key = "U1FZHPMB84QQO0RK"},
            new ApiKey {Key = "C4AWOH9D18QWQAXH"},
            new ApiKey {Key = "YNV3GOZNPZ6Q4356"},
            new ApiKey {Key = "8BXYPAN6NJX752KF"},
            new ApiKey {Key = "PFE56O2C0ZSK3MSX"},
            new ApiKey {Key = "D1NCT4OKI3HJEXXT"},
            new ApiKey {Key = "0O0M6UZNPQZJ4I06"},
            new ApiKey {Key = "P4A9QDRT6ZY2I37F"},
            new ApiKey {Key = "7HK5XE51JVU1EG1Y"},
            new ApiKey {Key = "1W9I92WB9NL66OD9"},
            new ApiKey {Key = "0IZX0UFOJU6G7JE3"}
        };

        const string DataFolder = @"E:\Quote\WebData\Minute\AlphaVantage\YearMonth\";
        const string SymbolListFileName = @"E:\Quote\WebData\Minute\AlphaVantage\SymbolsToDownload.txt";
        const string DontDownloadFileName = @"E:\Quote\WebData\Minute\AlphaVantage\SymbolsToDontDownload .txt";
        const string ProxyListFileName = @"E:\Quote\WebData\Minute\AlphaVantage\ProxyList.txt";
        const string ProxyLogFileName = @"E:\Quote\WebData\Minute\AlphaVantage\ProxyLog.txt";

        private static readonly TimeSpan _delay = new TimeSpan(0, 0, 63);
        // private static readonly TimeSpan _delay = new TimeSpan(0, 0, 9);

        private static bool _isBusy = false;
        private static object _lockObject = new object();

        private static string _lastIp;
        private static DateTime _lastIpUsed = DateTime.MinValue;
        private static System.Timers.Timer _timer;

        private static List<Tuple<string, string>> _urlsAndFilenames;
        private static Action<string> _showStatusAction;

        private static int _totalItems;
        private static int _downloadedItems;
        private static bool _stopFlag;

        public static void Stop() => _stopFlag = true;

        public static void RefreshProxyList()
        {
            if (_showStatusAction != null)
                _showStatusAction($"IntradayAlphaVantage_Download. Before proxy list refresh ...");
            var uniqueProxy = new Dictionary<string, object>();
            foreach (var item in File.ReadAllLines(ProxyListFileName)
                .Where(a => !string.IsNullOrEmpty(a) && !a.StartsWith("#")))
            {
                var proxy = item.Split('\t')[0];
                if (!uniqueProxy.ContainsKey(proxy))
                    uniqueProxy.Add(proxy, null);
            }

            lock (_lockObject)
            {
                var proxies = uniqueProxy.Keys.ToList();
                var apiKeys = _apiKeys.ToList();
                for (var k = 0; k < proxies.Count; k++)
                {
                    var key = apiKeys.FirstOrDefault(a => a.proxy == proxies[k]);
                    if (key != null)
                    {
                        proxies.Remove(proxies[k--]);
                        apiKeys.Remove(key);
                    }
                }

                foreach (var proxy in proxies)
                {
                    if (apiKeys.Count == 0) break;
                    apiKeys[0].proxy = proxy;
                    apiKeys.RemoveAt(0);
                }

                foreach (var key in apiKeys)
                    key.proxy = null;
            }

            if (_showStatusAction != null)
                _showStatusAction($"IntradayAlphaVantage_Download. Proxy list refreshed. {uniqueProxy.Count} proxy items. {_apiKeys.Length} api keys.");
        }

        public static void Start(Action<string> showStatusAction)
        {
            if (_isBusy)
            {
                MessageBox.Show("IntradayAlphaVantage_Download is working now .. Can't run it again.");
                return;
            }

            _isBusy = true;
            _showStatusAction = showStatusAction;

            _showStatusAction($"IntradayAlphaVantage_Download. Define urls and filenames to download.");
            var periodIds = new Dictionary<string, DateTime>();
            // for (var k = 12; k >= 12; k--)
            //    periodIds.Add($"year2month{k}", DateTime.Today.AddYears(-1).AddMonths(-k));
            for (var k = 12; k >= 1; k--)
                periodIds.Add($"year2month{k}", DateTime.Today.AddDays(-30 * (12 + k)));
            for (var k = 12; k >= 1; k--)
                periodIds.Add($"year1month{k}", DateTime.Today.AddDays(-30 * k));

            var dontDownloadSymbol = File.ReadAllLines(DontDownloadFileName).Where(a => !string.IsNullOrEmpty(a) && !a.StartsWith("#")).ToDictionary(a => a, a => (object)null);
            var symbols = new Dictionary<string, DateTime[]>();
            var ss = File.ReadAllLines(SymbolListFileName).Where(a => !a.StartsWith("#"));
            foreach (var s in ss)
            {
                if (string.IsNullOrEmpty(s)) continue;
                var ss1 = s.Split('\t');
                if (ss1.Length == 5 && !dontDownloadSymbol.ContainsKey(ss1[0].Trim().ToUpper()))
                {
                    symbols.Add(ss1[0].Trim(), new DateTime[] { DateTime.Parse(ss1[3].Trim(), CultureInfo.InvariantCulture), DateTime.Parse(ss1[4].Trim(), CultureInfo.InvariantCulture) });
                }
            }

            _urlsAndFilenames = new List<Tuple<string, string>>();
            foreach (var kvp1 in symbols)
                foreach (var kvp2 in periodIds)
                {
                    var filename = DataFolder + $"AV{kvp2.Key}_{kvp1.Key}.csv";
                    if (!File.Exists(filename))
                    {
                        _urlsAndFilenames.Add(new Tuple<string, string>(
                            @"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY_EXTENDED&symbol=" +
                            kvp1.Key + $"&interval=1min&slice={kvp2.Key}&adjusted=false&datatype=csv&apikey={{0}}",
                            filename));
                    }
                }

            RefreshProxyList();
            _totalItems = _urlsAndFilenames.Count;
            _downloadedItems = 0;
            _stopFlag = false;

            var tasks = _apiKeys.Select(a => Task.Factory.StartNew(() => Download(a)));
            Task.WaitAll(tasks.ToArray());

            _showStatusAction($"IntradayAlphaVantage_Download. Finished.");
        }

        private static void Download(ApiKey _apiKey)
        {
            while (true)
            {
                var ok = 0;
                var bad = 0;
                for (var k = 0; k < 5; k++)
                {
                    while (_apiKey.proxy == null)
                    {
                        if (_stopFlag)
                            return;
                        Thread.Sleep(10000);
                    }

                    Tuple<string, string> item = null;
                    lock (_lockObject)
                    {
                        if (_urlsAndFilenames.Count > 0)
                            item = _urlsAndFilenames[0];
                        _urlsAndFilenames.RemoveAt(0);
                    }

                    if (item == null)
                        return;

                    var url = string.Format(item.Item1, _apiKey.Key);
                    Debug.Print($"{DateTime.Now}. Api: {_apiKey.Key}. Proxy: {_apiKey.proxy}. BeforeDownload: {url}, {item.Item2}");
                    if (!Actions.Download.DownloadPageProxy(url, item.Item2, _apiKey.proxy))
                    {
                        lock (_lockObject)
                        {
                            _urlsAndFilenames.Insert(0, item);
                        }
                        bad++;
                    }
                    else
                        ok++;
                }

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = $"INSERT into QuoteProxyLog (Date, Proxy, Bad, OK) VALUES(GetDate(), '{_apiKey.proxy}', {bad}, {ok})";
                    cmd.ExecuteNonQuery();
                }

                if (_stopFlag)
                    return;

                Thread.Sleep(61000);
            }
        }
    }
}
