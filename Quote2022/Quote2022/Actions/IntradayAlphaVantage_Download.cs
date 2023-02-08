using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace Quote2022.Actions
{
    public static class IntradayAlphaVantage_Download
    {
        public class ApiKey
        {
            public string Key;
            public DateTime LastUsed = DateTime.MinValue;
        }

        private static ApiKey[] _apiKeys = new[]
        {
            new ApiKey {Key = "TK4Q66GMN8YDXDVZ"},
            new ApiKey {Key = "TXQMV0KYX4WBX7VS"},
            new ApiKey {Key = "QDYJLC03FUZX4VN2"}
        };

        const string DataFolder = @"E:\Quote\WebData\Minute\AlphaVantage\YearMonth\";
        const string SymbolListFileName = @"E:\Quote\WebData\Minute\AlphaVantage\SymbolsToDownload.txt";
        const string DontDownloadFileName = @"E:\Quote\WebData\Minute\AlphaVantage\SymbolsToDontDownload .txt";

        private static readonly TimeSpan _delay = new TimeSpan(0, 0, 63);
        // private static readonly TimeSpan _delay = new TimeSpan(0, 0, 9);

        private static bool _isBusy = false;
        private static object _lockObject = new object();

        private static string _lastIp;
        private static DateTime _lastIpUsed = DateTime.MinValue;
        private static System.Timers.Timer _timer;

        private static List<Tuple<string, string>[]> _urlsAndFilenames;
        private static int _urlsAndFilenamesCount;
        private static Action<string> _showStatusAction;

        private static int _totalItems;
        private static int _downloadedItems;

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

            var dontDownloadSymbol = File.ReadAllLines(DontDownloadFileName).Where(a => !string.IsNullOrEmpty(a) && !a.StartsWith("#")).ToDictionary(a => a, a => (object) null);
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

            _urlsAndFilenames = new List<Tuple<string, string>[]>();
            var lastUrlAndFilenames = new List<Tuple<string, string>>();
            foreach (var kvp1 in symbols)
                foreach (var kvp2 in periodIds)
                {
                    var filename = DataFolder + $"AV{kvp2.Key}_{kvp1.Key}.csv";
                    if (!File.Exists(filename))
                    {
                        lastUrlAndFilenames.Add(new Tuple<string, string>(@"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY_EXTENDED&symbol=" + kvp1.Key +
                                                                          $"&interval=1min&slice={kvp2.Key}&adjusted=false&datatype=csv&apikey={{0}}", filename));
                        if (lastUrlAndFilenames.Count == 5)
                        {
                            _urlsAndFilenames.Add(lastUrlAndFilenames.ToArray());
                            lastUrlAndFilenames.Clear();
                        }
                    }
                }

            if (lastUrlAndFilenames.Count > 0)
            {
                _urlsAndFilenames.Add(lastUrlAndFilenames.ToArray());
                lastUrlAndFilenames.Clear();
            }

            _urlsAndFilenames = _urlsAndFilenames.ToList();
            _urlsAndFilenamesCount = 0;
            _totalItems = _urlsAndFilenames.SelectMany(a => a).Count();
            _downloadedItems = 0;

            if (_timer == null)
            {
                _timer = new System.Timers.Timer { Interval = 1000, AutoReset = true, Enabled = true };
                _timer.Elapsed += Timer_Elapsed;
            }
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // var b1 = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            if (Monitor.TryEnter(_lockObject))
            {
                try
                {
                    var ip = GetCurrentIp();
                    if (!string.Equals(ip.ToString(), _lastIp) || ((DateTime.Now - _lastIpUsed) > _delay))
                    {
                        Debug.Print($"{DateTime.Now}. LastIP: {_lastIp}. CureentIp: {ip}");
                        var freeApiKey = GetFreeApiKey();
                        if (freeApiKey != null)
                        {
                            if (_urlsAndFilenames.Count <= _urlsAndFilenamesCount)
                            {
                                Finished();
                                return;
                            }

                            var items = _urlsAndFilenames[_urlsAndFilenamesCount++];
                            // Task.Factory.StartNew(() => DownloadBatch(items, freeApiKey));
                            foreach(var item in items)
                            {
                                Download(item, freeApiKey);
                            }
                            freeApiKey.LastUsed = DateTime.Now;

                            _downloadedItems += items.Length;
                            _showStatusAction(
                                $"IntradayAlphaVantage_Download. Downloaded {_downloadedItems:N0} from {_totalItems:N0}");

                            // freeApiKey.LastUsed = DateTime.MaxValue;
                            _lastIp = GetCurrentIp();
                            _lastIpUsed = DateTime.Now;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_lockObject);
                }
            }
        }

        private static void Download(Tuple<string, string> urlAndFileName, ApiKey apiKey)
        {
            var url = string.Format(urlAndFileName.Item1, apiKey.Key);
            Debug.Print($"{DateTime.Now}. Api: {apiKey.Key}. BeforeDownload: {url}, {urlAndFileName.Item2}");
            // Thread.Sleep(200);
            Actions.Download.DownloadPage(url, urlAndFileName.Item2);

            if (File.Exists(urlAndFileName.Item2))
            {
                var s = File.ReadAllText(urlAndFileName.Item2);
                if (s.Contains("Thank you for using"))
                    throw new Exception($"{DateTime.Now}. Thank you error in IntradayAlphaVantage_Download. Urls: {urlAndFileName.Item2}");
            }
        }

        private static ApiKey GetFreeApiKey()
        {
            var now = DateTime.Now;
            return _apiKeys.FirstOrDefault(a => (now - a.LastUsed) > _delay);
        }

        private static void Finished()
        {
            _urlsAndFilenames.Clear();

            _timer.Elapsed -= Timer_Elapsed;
            _timer.Stop();
            _timer.Dispose();
            _timer = null;

            _showStatusAction("IntradayAlphaVantage_Download finished!");
            _isBusy = false;
        }

        // private static DateTime _ipDateTime = DateTime.MinValue;
        // private static string _temp;
        private static string GetCurrentIp()
        {
            using (var wc = new WebClient())
            {
                /*var json = wc.DownloadString("http://ip-info.ff.avast.com/v1/info");
                var k1 = json.IndexOf("\"ip\":");
                var k2 = json.IndexOf(",", k1 + 3);
                var ip = json.Substring(k1 + 6, k2 - k1 - 7);
                return ip;*/
                var content = wc.DownloadString("https://www.find-ip.net");
                var i1 = content.IndexOf("<div class=\"ipcontent pure-u-13-24\">");
                var i2 = content.IndexOf("</div>", i1 + 35);
                var ip = content.Substring(i1 + 36, i2 - i1 - 36).Trim();
                return ip;
            }

            /*if (_temp == null)
            {
                _temp = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                    .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork).ToString();
                _ipDateTime = DateTime.Now;
            }
            else
            {
                if ((DateTime.Now - _ipDateTime) > new TimeSpan(0, 0, 2))
                {
                    _temp = _temp + "_" + _temp.Length;
                    _ipDateTime = DateTime.Now;
                }
            }

            return _temp;*/
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork).ToString();
        }
    }
}
