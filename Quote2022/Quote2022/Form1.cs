using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Quote2022.Actions;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022
{
    public partial class Form1 : Form
    {
        private object _lock = new object();

        public Form1()
        {
            InitializeComponent();
            statusLabel.Text = "";

            clbIntradayDataList.Items.AddRange(IntradayResults.ActionList.Select(a => a.Key).ToArray());
        }

        private void ShowStatus(string message)
        {
            lock (_lock)
            {
                statusLabel.Text = message;
            }
            Application.DoEvents();
        }

        private void btnDayYahooParse_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenZipFileDialog(Settings.DayYahooFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.DayYahoo_Parse(fn, ShowStatus);
        }

        private void btnDayYahooIndexesParse_Click(object sender, EventArgs e) => Parse.IndexesYahoo(ShowStatus);

        private void btnSymbolsNanex_Click(object sender, EventArgs e)
        {
            var files = Download.SymbolsNanex_Download(ShowStatus);
            var data = new List<SymbolsNanex>();
            Parse.SymbolsNanex_Parse(files, data, ShowStatus);
            ShowStatus($"Save Nanex Symbols");
            SaveToDb.SymbolsNanex_SaveToDb(data);
            ShowStatus($"Nanex Symbols: FINISHED!");
        }

        private void btnDayEoddataParse_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();
            SaveToDb.DayEoddata_SaveToDb(Parse.DayEoddata_Data(ShowStatus));
            ShowStatus($"DayEoddata file parsing finished!!!");

            // SaveToDb.ClearDbTable("xDayEoddata");
            // Parse.DayEoddata_Parse(ShowStatus);
            sw.Stop();
            Debug.Print("Time: " + sw.ElapsedMilliseconds);
        }

        private void btnSplitYahooParse_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenZipFileDialog(Settings.SplitYahooFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.SplitYahoo_Parse(fn, ShowStatus);
        }

        private void btnSplitInvestingParse_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenTxtFileDialog(Settings.SplitInvestingFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.SplitInvesting_Parse(fn, ShowStatus);
        }

        private void btnSplitEoddataParse_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenTxtFileDialog(Settings.SplitEoddataFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.SplitEoddata_Parse(fn, ShowStatus);
        }

        private void btnAlgorithm1_Click(object sender, EventArgs e)
        {
            var dataSet = new List<string>();
            if (cb2013.Checked) dataSet.Add("2013");
            if (cb2022.Checked) dataSet.Add("2022");
            Helpers.Algorithm1.Execute(dataSet, ShowStatus);
        }

        private void btnDailyEoddataCheck_Click(object sender, EventArgs e) => Parse.DayEoddata_Check(ShowStatus);

        private void btnParseScreenerNasdaqParse_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenZipFileDialog(Settings.ScreenerNasdaqFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.ScreenerNasdaq_ParseAndSaveToDb(fn, ShowStatus);
        }

        private void btnStockSplitHistoryParse_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenZipFileDialog(Settings.StockSplitHistoryFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.StockSplitHistory_Parse(fn, ShowStatus);
        }

        private void btnSplitInvestingHistoryParse_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(Settings.SplitInvestingHistoryFolder, "*.txt");
            var data = new Dictionary<string, SplitModel>();
            foreach (var file in files)
                Parse.SplitInvestingHistory_Parse(file, data, ShowStatus);

            ShowStatus($"SplitInvestingHistory is saving to database");
            SaveToDb.SplitInvestingHistory_SaveToDb(data.Values);
            ShowStatus($"SplitInvestingHistory parse & save to database FINISHED!");
        }

        private void btnQuantumonlineProfilesParse_Click(object sender, EventArgs e)
        {
            Parse.ProfileQuantumonline_ParseAndSaveToDb(@"E:\Quote\WebData\Symbols\Quantumonline\Profiles\Profiles.zip", ShowStatus);
            return;
            if (CsUtils.OpenZipFileDialog(Settings.ProfileQuantumonlineFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.ProfileQuantumonline_ParseAndSaveToDb(fn, ShowStatus);
        }

        private void btnSymbolsStockanalysisDownload_Click(object sender, EventArgs e)
        {
            Download.SymbolsStockanalysis_Download(ShowStatus);
        }

        private void btnSymbolsStockanalysisParse_Click(object sender, EventArgs e) => Parse.SymbolsStockanalysis_ParseAndSaveToDb(ShowStatus);

        private void btnSymbolsNasdaqParse_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenZipFileDialog(Settings.SymbolsNasdaqFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.SymbolsNasdaq_ParseAndSaveToDb(fn, ShowStatus);
        }

        private void btnRefreshSymbolsData_Click(object sender, EventArgs e)
        {
            ShowStatus($"RefreshSymbolsData is starting");
            SaveToDb.RunProcedure("pRefreshSymbols");
            ShowStatus($"RefreshSymbolsData FINISHED!");
        }

        private void btnSymbolsEoddataParse_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenZipFileDialog(Settings.SymbolsEoddataFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.SymbolsEoddata_ParseAndSaveToDb(fn, ShowStatus);
        }

        private void btnTimeSalesNasdaqDownload_Click(object sender, EventArgs e)
        {
            Download.TimeSalesNasdaq_Download(ShowStatus);
        }

        private void btnSymbolsQuantumonlineDownload_Click(object sender, EventArgs e) => Download.SymbolsQuantumonline_Download(ShowStatus);

        private void btnSymbolsQuantumonlineParse_Click(object sender, EventArgs e) => Parse.SymbolsQuantumonlineZip_Parse(ShowStatus);

        private void btnRefreshSpitsData_Click(object sender, EventArgs e)
        {
            ShowStatus($"RefreshSpitsData is starting");
            SaveToDb.RunProcedure("pRefreshSplits");
            ShowStatus($"RefreshSplitsData FINISHED!");
        }

        private void btnTimeSalesNasdaqSaveLog_Click(object sender, EventArgs e)
        {
            ShowStatus($"Started");
            var folders = Directory.GetDirectories(@"E:\Quote\WebData\Minute\Nasdaq", "TS_2022*", SearchOption.TopDirectoryOnly);
            foreach (var folder in folders)
            {
                Check.TimeSalesNasdaq_SaveLog(folder, true, ShowStatus);
            }
            ShowStatus($"FINISHED!");
        }

        private void btnTimeSalesNasdaqSaveSummary_Click(object sender, EventArgs e)
        {
            ShowStatus($"Started");
            var folders = Directory.GetDirectories(@"E:\Quote\WebData\Minute\Nasdaq", "TS_2022*", SearchOption.TopDirectoryOnly);
            foreach (var folder in folders)
            {
                Check.TimeSalesNasdaq_SaveSummary(folder, true, ShowStatus);
            }
            ShowStatus($"FINISHED!");
        }

        private void btnUpdateTradingDays_Click(object sender, EventArgs e)
        {
            ShowStatus($"UpdateTradingDays is starting");
            SaveToDb.RunProcedure("pRefreshTradingDays");
            ShowStatus($"UpdateTradingDays FINISHED!");
        }

        //==================
        //  Temp section
        //=================
        private void btnTemp_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void btnSymbolsYahooLookupDownload_Click(object sender, EventArgs e)
        {
            ShowStatus($"SymbolsYahooLookupDownload Started (equity)");
            Download.SymbolsYahooLookup_Download("equity", ShowStatus);
            ShowStatus($"SymbolsYahooLookupDownload Started (etf)");
            Download.SymbolsYahooLookup_Download("etf", ShowStatus);
            ShowStatus($"SymbolsYahooLookupDownload FINISHED!");
        }

        private void btnSymbolsYahooLookupParse_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenFileDialogGeneric(Settings.SymbolsYahooLookupFolder, @"*_202?????.zip files (*.zip)|*_202?????.zip") is string fn && !string.IsNullOrEmpty(fn))
                Parse.SymbolsYahooLookup_ParseAndSaveToDb(fn, ShowStatus);
        }

        private void btnDayYahooDownload_Click(object sender, EventArgs e)
        {
            Download.DayYahoo_Download(ShowStatus);
        }

        private void btnMinuteYahooCheck_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenFileDialogGeneric(Settings.MinuteYahooFolder, @"YahooMinute_202?????.zip file (*.zip)|YahooMinute_202?????.zip") is string fn && !string.IsNullOrEmpty(fn))
                Check.MinuteYahoo_SaveLog(new [] {fn}, ShowStatus);
            // Check.MinuteYahoo_SaveLog(null, ShowStatus);
        }

        #region ============  Intradaya Statistics  ==============
        private void btnIntradayByTime_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByTime);
        private void btnIntradayByDate_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByDate);
        private void btnIntradayByDayOfWeek_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByDayOfWeek);
        private void btnIntradayByWeek_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByWeek);
        private void btnIntradayByKind_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByKind);
        private void btnIntradayBySector_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.BySector);
        private void btnIntradayByIndustry_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByIndustry);
        private void btnIntradayBySymbol_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.BySymbol);
        private void btnIntradayByTradeValue_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByTradeValue);
        private void btnIntradayByTradingViewType_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByTradingViewTypeAndSubtype);
        private void btnIntradayByTradingViewSector_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByTradingViewSector);
        private void btnIntradayByKindAndTime_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByKindAndTime);
        private void btnIntradayByKindAndDayOfWeek_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByKindAndDayOfWeek);
        private void btnIntradayBySectorAndIndustry_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.BySectorAndIndustry);
        private void btnIntradayByExchangeAndAsset_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByExchangeAndAsset);
        private void btnIntradayByRecommend_Click(object sender, EventArgs e) => IntradayClickAction(IntradayResults.ByTradingViewRecommend);

        private void IntradayClickAction(Action<List<IntradayQuote>> action)
        {
            var sw = new Stopwatch();
            sw.Start();

            var closeInNextFrame = !(rbFullDayBy30.Checked || rbFullDayBy90.Checked);
            var quotes = QuoteLoader.MinuteYahoo_GetQuotes(ShowStatus, GetTimeFrames(), closeInNextFrame, cbUseZipCache.Checked, cbUseLastQuotes.Checked);
            action(quotes);

            sw.Stop();
            Debug.Print($"StopWatch: {sw.ElapsedMilliseconds:N0}");
        }

        private List<TimeSpan> GetTimeFrames()
        {
            var fullDay= rbFullDayBy30.Checked || rbFullDayBy90.Checked;
            var is30MinutesInterval = rbFullDayBy30.Checked || rbPartialDayBy30.Checked;

            if (is30MinutesInterval)
            {
                return fullDay
                    ? CsUtils.GetTimeFrames(new TimeSpan(9, 30, 0), new TimeSpan(16, 00, 0), new TimeSpan(0, 30, 0))
                    : CsUtils.GetTimeFrames(new TimeSpan(10, 00, 0), new TimeSpan(15, 30, 0), new TimeSpan(0, 30, 0));
            }

            // 90/105 minutes
            if (fullDay)
                return new List<TimeSpan>()
                {
                    new TimeSpan(9, 30, 0), new TimeSpan(11, 15, 0), new TimeSpan(12, 45, 0), new TimeSpan(14, 15, 0),
                    new TimeSpan(16, 00, 0)
                };

            return new List<TimeSpan>()
            {
                new TimeSpan(9, 45, 0), new TimeSpan(11, 15, 0), new TimeSpan(12, 45, 0), new TimeSpan(14, 15, 0),
                new TimeSpan(15, 45, 0)
            };
        }

        private void btnCheckYahooMinuteData_Click(object sender, EventArgs e) => Check.MinuteYahoo_CheckData(ShowStatus);
        private void btnPrepareYahooMinuteZipCache_Click(object sender, EventArgs e) => QuoteLoader.MinuteYahoo_PrepareTextCache(ShowStatus);

        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();
            var aa1 = QuoteLoader.GetYahooIntradayQuotesFromZipCache(ShowStatus, Path.Combine(Settings.MinuteYahooFolder, "Cache.zip"));
            var dd = aa1.ToDictionary(a => a.Symbol + a.TimeString, a => a);
            sw.Stop();
            var a1 = sw.ElapsedMilliseconds;
            // return;

            /*var sw1 = new Stopwatch();
            sw1.Start();
            var aa = QuoteLoader.GetYahooIntradayQuotesFromTextCache(ShowStatus, Path.Combine(Settings.MinuteYahooFolder, "Cache.txt"));
            var dd = aa.ToDictionary(a => a.Symbol + a.TimeString, a => a);*/
            

            // sw.Stop();
            // var a1 = sw.ElapsedMilliseconds;
            Debug.Print($"Duration: {a1}. Records: {dd.Count}");
            // return;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var d1 = GC.GetTotalMemory(true);

            var symbols = DataSources.GetActiveSymbols();
            var zipFiles = Directory.GetFiles(Settings.MinuteYahooFolder, "YahooMinute_202?????.zip");
            // var zipFiles = Directory.GetFiles(Settings.MinuteYahooFolder, "YahooMinute_2022111?.zip");
            // QuoteLoader.PrepareYahooIntradayTextCache(ShowStatus, zipFiles, Path.Combine(Settings.MinuteYahooFolder, "Cache.txt"), (s => !symbols.ContainsKey(s)));
            // return;


            foreach (var zFile in zipFiles)
            {
                var oo = QuoteLoader.GetYahooIntradayQuotes(ShowStatus, new [] {zFile},
                    CsUtils.GetTimeFrames(new TimeSpan(9, 30, 0), new TimeSpan(16, 00, 0), new TimeSpan(0, 1, 0)),
                    (s => !symbols.ContainsKey(s)), false, false);

                foreach (var q in oo)
                {
                    var q1 = (Quote) q;
                    var key = q.Symbol + q1.TimeString;
                    if (dd.ContainsKey(key) && string.Equals(q.ToStringOfBaseClass(), dd[key].ToString()))
                        dd.Remove(key);
                    else
                        throw new Exception("AAA");
                }
            }

            /* Debug.Print($"Time\t{TradeStatistics.GetHeader()}");
             var group = oo.GroupBy(o => o.TimeFrameId);
             foreach (var g in group.OrderBy(a => a.Key))
             {
                 var quotes = g.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                 var ts = new TradeStatistics((IList<Quote>)quotes);
                 Debug.Print($"{CsUtils.GetString(g.Key)}\t{ts.GetContent()}");
             }*/

            Debug.Print($"Rest: {dd.Count}");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var symbols = DataSources.GetActiveSymbols();
            var quotes = QuoteLoader.GetYahooIntradayQuotesFromZipCache(ShowStatus, Path.Combine(Settings.MinuteYahooFolder, "Cache.zip")).Where(a=> symbols[a.Symbol].Sector == "Technology").ToArray();

            /*for (var m = 1; m <= 390; m++)
            {
                if ((390 % m) == 0)
                {
                    var timeFrames = CsUtils.GetTimeFrames(new TimeSpan(9, 30, 0), new TimeSpan(16, 0, 0), new TimeSpan(0, m, 0));
                    Debug.Print($"=============  {DateTime.Now}  =============");
                    Debug.Print($"From {timeFrames[0]:hh\\:mm} to {timeFrames[timeFrames.Count - 1]:hh\\:mm} by {m} min");
                    IntradayResults.ByTimeNew(ShowStatus, quotes, timeFrames, false);
                }
            }*/
            for (var m = 1; m <= 360; m++)
            {
                if ((360 % m) == 0)
                {
                    var timeFrames = CsUtils.GetTimeFrames(new TimeSpan(9, 45, 0), new TimeSpan(15, 45, 0), new TimeSpan(0, m, 0));
                    Debug.Print($"=============  {DateTime.Now}  =============");
                    Debug.Print($"From {timeFrames[0]:hh\\:mm} to {timeFrames[timeFrames.Count - 1]:hh\\:mm} by {m} min (closeInNextFrame)");
                    IntradayResults.ByTimeNew(ShowStatus, quotes, timeFrames, true);
                }
            }
            return;

            //var timeFrames = IntradayResults.GetTimeFrames(rbFullDayBy30.Checked || rbFullDayBy90.Checked, rbFullDayBy30.Checked || rbPartialDayBy30.Checked);
            // IntradayResults.ByTimeNew(ShowStatus, quotes, timeFrames, !rbFullDayBy30.Checked || rbFullDayBy90.Checked);
            ShowStatus("button3_Click finish");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Download.TradingViewScreener_Download(ShowStatus);
            // var o = JsonConvert.DeserializeObject<ScreenerTradingView>(File.ReadAllText(Settings.ScreenerTradingViewFile));
        }

        private void btnScreenerTradingViewDownload_Click(object sender, EventArgs e)
        {
            var timeStamp = DateTime.Now.AddHours(4).AddDays(-1).Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            var fileName = string.Format(Settings.ScreenerTradingViewFileTemplate, timeStamp);

            Download.ScreenerTradingView_Download(ShowStatus, fileName);

        }

        private void btnTradingViewScreenerParse_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenFileDialogGeneric(Settings.ScreenerTradingViewFolder, @"TVScreener_202?????.zip file (*.zip)|TVScreener_202?????.zip") is string fn && !string.IsNullOrEmpty(fn))
                Parse.ScreenerTradingView_ParseAndSaveToDb(fn, ShowStatus);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (clbIntradayDataList.CheckedItems.Count == 0)
            {
                MessageBox.Show(@"Виберіть хоча б один тип даних");
                return;
            }

            var sw = new Stopwatch();
            sw.Start();

            var closeInNextFrame = !(rbFullDayBy30.Checked || rbFullDayBy90.Checked);
            var timeFrames = GetTimeFrames();
            var quotes = QuoteLoader.MinuteYahoo_GetQuotes(ShowStatus, timeFrames, closeInNextFrame, cbUseZipCache.Checked, cbUseLastQuotes.Checked);
            var sbParameters = new StringBuilder();
            if (timeFrames.Count > 1)
                sbParameters.Append(
                    $"Time: {CsUtils.GetString(timeFrames[0])}-{CsUtils.GetString(timeFrames[timeFrames.Count - 1])}, interval: {CsUtils.GetString(timeFrames[1] - timeFrames[0])}");
            else if (timeFrames.Count == 1)
                sbParameters.Append($"Time: {CsUtils.GetString(timeFrames[0])}");
            if (closeInNextFrame)
                sbParameters.Append(", closeInNextFrame");


            foreach (var o in clbIntradayDataList.CheckedItems)
            {
                Debug.Print("===================================================================");
                Debug.Print(o.ToString());
                Debug.Print(sbParameters.ToString());
                IntradayResults.ActionList[(string) o](quotes);
            }

            sw.Stop();
            Debug.Print($"StopWatch: {sw.ElapsedMilliseconds:N0}");
        }
    }
}
