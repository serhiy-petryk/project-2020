using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
            ExcelTest();
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
            if (CsUtils.OpenFileDialogGeneric(Settings.MinuteYahooDataFolder, @"YahooMinute_202?????.zip file (*.zip)|YahooMinute_202?????.zip") is string fn && !string.IsNullOrEmpty(fn))
                Check.MinuteYahoo_SaveLog(new [] {fn}, ShowStatus);
            // Check.MinuteYahoo_SaveLog(null, ShowStatus);
        }

        #region ============  Intradaya Statistics  ==============
        private void btnCheckYahooMinuteData_Click(object sender, EventArgs e) => Check.MinuteYahoo_CheckData(ShowStatus);
        private void btnPrepareYahooMinuteZipCache_Click(object sender, EventArgs e) => QuoteLoader.MinuteYahoo_PrepareTextCache(ShowStatus);

        private void btnIntradayGenerateReport_Click(object sender, EventArgs e)
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
            var sbParameters = new StringBuilder();
            if (timeFrames.Count > 1)
                sbParameters.Append($"Time: {CsUtils.GetString(timeFrames[0])}-{CsUtils.GetString(timeFrames[timeFrames.Count - 1])}, interval: {CsUtils.GetString(timeFrames[1] - timeFrames[0])}");
            else if (timeFrames.Count == 1)
                sbParameters.Append($"Time: {CsUtils.GetString(timeFrames[0])}");
            if (closeInNextFrame)
                sbParameters.Append(", closeInNextFrame");

            ShowStatus($"Data generation for report");
            var quotes = GetIntradayQuotes().ToArray();
            Debug.Print($"*** After GetIntradayQuotes. StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");

            var quoteCount = 0;
            var minDate = DateTime.MaxValue;
            var maxDate = DateTime.MinValue;
            var symbols = new Dictionary<string, object>();
            foreach (var q in quotes)
            {
                quoteCount++;
                if (q.Timed < minDate) minDate = q.Timed;
                if (q.Timed > maxDate) maxDate = q.Timed;
                if (!symbols.ContainsKey(q.Symbol))
                    symbols.Add(q.Symbol, null);
            }

            var summaryHeader = $"Data from {CsUtils.GetString(minDate)} to {CsUtils.GetString(maxDate)}, {symbols.Count} symbols, {quoteCount:N0} quotes";

            var data = new Dictionary<string, ExcelUtils.StatisticsData>();
            foreach (var o in clbIntradayDataList.CheckedItems)
            {
                var key = IntradayResults.ActionList[(string)o].Method.Name;
                Debug.Print($"*** Before prepare {key}. StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");
                ShowStatus($"Generation '{key}' report");
                var lines = IntradayResults.ActionList[(string)o](quotes);
                var sd = new ExcelUtils.StatisticsData{ Header1 = o.ToString(), Header2 = summaryHeader, Header3 = sbParameters.ToString(), Table = lines};
                data.Add(key, sd);

                Debug.Print("===================================================================");
                foreach (var value in lines)
                {
                    var sb = new StringBuilder();
                    foreach (var a in value)
                        sb.Append((sb.Length == 0 ? "" : "\t") + CsUtils.GetString(a));

                    Debug.Print(sb.ToString());
                }
                Debug.Print($"*** After prepare {key}. StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");
            }

            Helpers.ExcelUtils.SaveStatisticsToExcel(data, Settings.MinuteYahooLogFolder + "TestEPPlus.xlsx", summaryHeader);

            sw.Stop();
            Debug.Print($"*** btnIntradayGenerateReport_Click finished. StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");
            ShowStatus($"Report is ready!");
        }

        private void btnIntradayPrintDetails_Click(object sender, EventArgs e)
        {
            var quotes = GetIntradayQuotes().Where(a => a.Symbol == "PULS").OrderBy(a => a.Timed).ToList();
        }

        private IEnumerable<IntradayQuote> GetIntradayQuotes()
        {
            var closeInNextFrame = !(rbFullDayBy30.Checked || rbFullDayBy90.Checked);
            var timeFrames = GetTimeFrames();
            var quotes = QuoteLoader.MinuteYahoo_GetQuotes(ShowStatus, timeFrames, closeInNextFrame, cbUseZipCache.Checked);
            return quotes;
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

        private void btnIntradayGenerateByTimeReports_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var symbols = DataSources.GetActiveSymbols();
            var minuteQuotes = QuoteLoader.MinuteYahoo_GetQuotesFromZipCache(ShowStatus, true).ToArray();

            Debug.Print($"*** After load StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");

            var data = new Dictionary<string, ExcelUtils.StatisticsData>();
            for (var m = 1; m <= 390; m++)
            {
                continue;

                if ((390 % m) == 0)
                {
                    ShowStatus($" Data generation for {m} minute frames");
                    var timeFrames = CsUtils.GetTimeFrames(new TimeSpan(9, 30, 0), new TimeSpan(16, 0, 0),
                        new TimeSpan(0, m, 0));
                    var sbParameters = new StringBuilder();
                    if (timeFrames.Count > 1)
                        sbParameters.Append($"Time: {CsUtils.GetString(timeFrames[0])}-{CsUtils.GetString(timeFrames[timeFrames.Count - 1])}, interval: {CsUtils.GetString(timeFrames[1] - timeFrames[0])}");
                    else if (timeFrames.Count == 1)
                        sbParameters.Append($"Time: {CsUtils.GetString(timeFrames[0])}");
                    // if (closeInNextFrame)
                    // sbParameters.Append(", closeInNextFrame");

                    var quotes = QuoteLoader.GetIntradayQuotes(ShowStatus, minuteQuotes, timeFrames, false);
                    var lines = IntradayResults.ByTime(quotes);
                    var sd = new ExcelUtils.StatisticsData
                        {Header2 = $"By Time ({m}min)", Header3 = sbParameters.ToString(), Table = lines};
                    data.Add($"M{m}M", sd);

                    /*Debug.Print("===================================================================");
                    foreach (var value in lines)
                    {
                        var sb = new StringBuilder();
                        foreach (var a in value)
                            sb.Append((sb.Length == 0 ? "" : "\t") + CsUtils.GetString(a));

                        Debug.Print(sb.ToString());
                    }

                    Debug.Print($"=============  {DateTime.Now}  =============");
                    Debug.Print(
                        $"From {timeFrames[0]:hh\\:mm} to {timeFrames[timeFrames.Count - 1]:hh\\:mm} by {m} min");
                    IntradayResults.ByTimeNew(ShowStatus, quotes, timeFrames, false);*/
                }
            }
            // ShowStatus("Save to excel");
            // Helpers.ExcelUtils.SaveStatisticsToExcel(data, Settings.MinuteYahooLogFolder + "TestByTime.xlsx");

            data.Clear();
            for (var m = 1; m <= 360; m++)
            {
                if ((360 % m) == 0)
                {
                    if (m<180) continue;
                    Debug.Print($"*** Before process {m}min StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");

                    ShowStatus($" Data generation for {m} minute frames");
                    var timeFrames = CsUtils.GetTimeFrames(new TimeSpan(9, 45, 0), new TimeSpan(15, 45, 0), new TimeSpan(0, m, 0));
                    var sbParameters = new StringBuilder();
                    if (timeFrames.Count > 1)
                        sbParameters.Append($"Time: {CsUtils.GetString(timeFrames[0])}-{CsUtils.GetString(timeFrames[timeFrames.Count - 1])}, interval: {CsUtils.GetString(timeFrames[1] - timeFrames[0])}");
                    else if (timeFrames.Count == 1)
                        sbParameters.Append($"Time: {CsUtils.GetString(timeFrames[0])}");
                    sbParameters.Append(", closeInNextFrame");

                    var quotes = QuoteLoader.GetIntradayQuotes(ShowStatus, minuteQuotes, timeFrames, true);
                    var lines = IntradayResults.ByTime(quotes);
                    var sd = new ExcelUtils.StatisticsData
                        { Header2 = $"By Time ({m}min)", Header3 = sbParameters.ToString(), Table = lines };
                    data.Add($"N{m}M", sd);

                    /*Debug.Print($"=============  {DateTime.Now}  =============");
                    Debug.Print($"From {timeFrames[0]:hh\\:mm} to {timeFrames[timeFrames.Count - 1]:hh\\:mm} by {m} min (closeInNextFrame)");
                    IntradayResults.ByTimeNew(ShowStatus, quotes, timeFrames, true);*/

                    Debug.Print($"*** After process {m}min StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");
                }
            }

            sw.Stop();
            Debug.Print($"*** Finished StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");

            ShowStatus("Save to excel");
            Helpers.ExcelUtils.SaveStatisticsToExcel(data, Settings.MinuteYahooLogFolder + "TestByTimeNext.xlsx");

            ShowStatus("Finished");
        }

        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var symbols = DataSources.GetActiveSymbols();
            var quotes = QuoteLoader.MinuteYahoo_GetQuotesFromZipCache(ShowStatus, true).ToArray();

            Debug.Print($"*** After load StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");

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

                    Debug.Print($"*** After process {m}min StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");
                }
            }

            sw.Stop();
            Debug.Print($"*** Finished StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");
            ShowStatus("button3_Click finish");
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

        private void btnTemp_Click(object sender, EventArgs e)
        {
            /*var quotes = GetIntradayQuotes();
            var ss = IntradayResults.ByTimeX(quotes);
            ss.Insert(0, new[] { "ByTimeX" });
            ss.Insert(1, new[] { "Time: 09:30-16:00, interval: 00:30" });

            var data = new Dictionary<string, List<object[]>>();
            data.Add("ByTimeX", ss);

            Helpers.ExcelTest.AATable(data);
            ShowStatus("Finished!");*/
        }

        private void ExcelTest()
        {
            var quotes = GetIntradayQuotes();
            var ss = IntradayResults.ByTime(quotes);
            // ss.Insert(0, new[] { "ByTimeX" });
            // ss.Insert(1, new[] { "Time: 09:30-16:00, interval: 00:30" });

            foreach (var aa in ss)
            {
                var sb = new StringBuilder();
                var first = true;
                foreach (var a in aa)
                {
                    if (first)
                        sb.Append(CsUtils.GetString(a));
                    else
                        sb.Append("\t" + CsUtils.GetString(a));
                    first = false;
                }
                Debug.Print(sb.ToString());
            }

            var data = new Dictionary<string, ExcelUtils.StatisticsData>();
            var aa1 = new ExcelUtils.StatisticsData()
                {Header2 = "ByTime", Header3 = "Time: 09:30-16:00, interval: 00:30", Table = ss};
            data.Add("ByTimeX", aa1);

            // Helpers.ExcelTestXml.AAXmlTable(data);
            Helpers.ExcelUtils.SaveStatisticsToExcel(data, Settings.MinuteYahooLogFolder + "TestEPPlus.xlsx");

            ShowStatus("Finished!");
        }
    }
}
