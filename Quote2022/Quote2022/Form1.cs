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
            cbIntradayStopInPercent_CheckedChanged(null, null);
            for (var item = 0; item < clbIntradayDataList.Items.Count; item++)
                clbIntradayDataList.SetItemChecked(item, true);

            // ExcelTest();
        }

        private void cbIntradayStopInPercent_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIntradayStopInPercent.Checked)
            {
                nudIntradayStop.DecimalPlaces = 1;
                nudIntradayStop.Minimum = 0.1M;
                nudIntradayStop.Increment = 0.1M;
                nudIntradayStop.Value = 0.5M;
            }
            else
            {
                nudIntradayStop.DecimalPlaces = 2;
                nudIntradayStop.Minimum = 0.01M;
                nudIntradayStop.Increment = 0.01M;
                nudIntradayStop.Value = 0.03M;
            }
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
            SaveToDb.DayEoddata_SaveToDb(Parse.DayEoddata_Data(ShowStatus), ShowStatus);
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

        private void btnMinuteYahooLog_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenFileDialogGeneric(Settings.MinuteYahooDataFolder, @"YahooMinute_202?????.zip file (*.zip)|YahooMinute_202?????.zip") is string fn && !string.IsNullOrEmpty(fn))
                Check.MinuteYahoo_SaveLog(new [] {fn}, ShowStatus);
        }

        #region ============  Intradaya Statistics  ==============
        private void btnCheckYahooMinuteData_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenFileDialogMultiselect(Settings.MinuteYahooDataFolder, @"YahooMinute_202?????.zip file (*.zip)|YahooMinute_202?????.zip") is string[] files && files.Length > 0)
                Check.MinuteYahoo_CheckData(ShowStatus, files);
        }

        private void btnPrepareYahooMinuteZipCache_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenFileDialogMultiselect(Settings.MinuteYahooDataFolder, @"YahooMinute_202?????.zip file (*.zip)|YahooMinute_202?????.zip") is string[] files && files.Length > 0)
                QuoteLoader.MinuteYahoo_PrepareTextCache(ShowStatus, files);
        }

        private void btnIntradayGenerateReport_Click(object sender, EventArgs e)
        {
            if (clbIntradayDataList.CheckedItems.Count == 0)
            {
                MessageBox.Show(@"Виберіть хоча б один тип даних");
                return;
            }

            var iParameters = IntradayGetParameters();
            if (iParameters.TimeFrames == null) return;

            var zipFile = CsUtils.OpenFileDialogGeneric(Settings.MinuteYahooCacheFolder, @"Cache*.zip file (*.zip)|Cache*.zip");
            if (string.IsNullOrEmpty(zipFile)) return;

            var sw = new Stopwatch();
            sw.Start();
            ShowStatus($"Data generation for report");

            // Define minute quotes
            var quotesInfoMinute = new QuotesInfo();
            var minuteQuotes = QuoteLoader.MinuteYahoo_GetQuotesFromZipCache(ShowStatus, zipFile, true, quotesInfoMinute);

            // Prepare quote list
            var quotesInfo = new QuotesInfo();
            var quotes = QuoteLoader.GetIntradayQuotes(null, minuteQuotes, iParameters, quotesInfo).ToArray();
            Debug.Print($"*** After GetIntradayQuotes. StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");

            var data = new Dictionary<string, ExcelHelper.StatisticsData>();
            foreach (var o in clbIntradayDataList.CheckedItems)
            {
                var key = IntradayResults.ActionList[(string)o].Method.Name;
                Debug.Print($"*** Before prepare {key}. StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");
                ShowStatus($"Generation '{key}' report");

                var reportLines = IntradayResults.ActionList[(string)o](quotes, iParameters);
                var sd = new ExcelHelper.StatisticsData
                {
                    Title = o.ToString(), Header1 = quotesInfo.GetStatus(), Header2 = iParameters.GetTimeFramesInfo(),
                    Header3 = iParameters.GetFeesInfo(), Table = reportLines
                };
                data.Add(key, sd);
                IntradayPrintReportLines(reportLines);

                Debug.Print($"*** After prepare {key}. StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");
            }

            var excelFileName = IntradayGetExcelFileName(zipFile, "Intraday", iParameters);
            Helpers.ExcelHelper.SaveStatisticsToExcel(data, excelFileName, quotesInfoMinute.GetStatus());

            sw.Stop();
            Debug.Print($"*** btnIntradayGenerateReport_Click finished. StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");
            ShowStatus($"Report is ready! Filename: {excelFileName}");
        }

        private void btnIntradayByTimeReports_Click(object sender, EventArgs e) =>
            IntradayByTimeReport(true, "M{0}M", "IntradayByTime");

        private void btnIntradayByTimeReportsClosedInNextFrame_Click(object sender, EventArgs e) =>
            IntradayByTimeReport(false, "N{0}M", "IntradayByTimeNext");

        private void IntradayByTimeReport(bool fullDay, string sheetNameTemplate, string fileNamePrefix)
        {
            var zipFile = CsUtils.OpenFileDialogGeneric(Settings.MinuteYahooCacheFolder, @"Cache*.zip file (*.zip)|Cache*.zip");
            if (string.IsNullOrEmpty(zipFile)) return;

            var sw = new Stopwatch();
            sw.Start();

            var iParameters = IntradayGetParameters();
            iParameters.CloseInNextFrame = !fullDay;
            var startTime = fullDay ? new TimeSpan(9, 30, 0) : new TimeSpan(9, 45, 0);
            var endTime = fullDay ? new TimeSpan(16, 00, 0) : new TimeSpan(15, 45, 0);
            var durationInMinutes = Convert.ToInt32((endTime - startTime).TotalMinutes);

            // Get minute quotes
            var quotesInfoMinute = new QuotesInfo();
            var minuteQuotes = QuoteLoader.MinuteYahoo_GetQuotesFromZipCache(ShowStatus, zipFile, true, quotesInfoMinute).ToArray();
            Debug.Print($"*** After load StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");

            var data = new Dictionary<string, ExcelHelper.StatisticsData>();
            for (var m = 1; m <= durationInMinutes; m++)
            {
                if ((durationInMinutes % m) == 0)
                {
                    Debug.Print($"*** Before process {m}min StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");
                    ShowStatus($" Data generation for {m} minute frames");

                    var quotesInfo = new QuotesInfo();
                    iParameters.TimeFrames = CsUtils.GetTimeFrames(startTime, endTime, new TimeSpan(0, m, 0));
                    var quotes = QuoteLoader.GetIntradayQuotes(ShowStatus, minuteQuotes, iParameters, quotesInfo);
                    var reportLines = IntradayResults.ByTime(quotes, iParameters);
                    var sd = new ExcelHelper.StatisticsData
                    {
                        Title = $"By Time ({m}min)", Header1 = quotesInfo.GetStatus(),
                        Header2 = iParameters.GetTimeFramesInfo(), Header3 = iParameters.GetFeesInfo(),
                        Table = reportLines
                    };
                    data.Add(string.Format(sheetNameTemplate, m.ToString()), sd);
                    IntradayPrintReportLines(reportLines);

                    Debug.Print($"*** After process {m}min StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");
                }
            }

            ShowStatus("Saving to excel");
            var excelFileName = IntradayGetExcelFileName(zipFile, fileNamePrefix, iParameters);
            Helpers.ExcelHelper.SaveStatisticsToExcel(data, excelFileName, quotesInfoMinute.GetStatus());
            ShowStatus($"Finished! Filename: {excelFileName}");

            sw.Stop();
            Debug.Print($"*** Finished StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");
        }

        private void btnIntradaySaveToDB_Click(object sender, EventArgs e)
        {
            var iParameters = IntradayGetParameters();
            if (iParameters.TimeFrames == null) return;

            var zipFile = CsUtils.OpenFileDialogGeneric(Settings.MinuteYahooCacheFolder, @"Cache*.zip file (*.zip)|Cache*.zip");
            if (string.IsNullOrEmpty(zipFile)) return;

            var sw = new Stopwatch();
            sw.Start();
            ShowStatus($"Data generation for report");

            // Define minute quotes
            var quotesInfoMinute = new QuotesInfo();
            var minuteQuotes = QuoteLoader.MinuteYahoo_GetQuotesFromZipCache(ShowStatus, zipFile, true, quotesInfoMinute);

            // Prepare quote list
            var closeInNextFrame = cbCloseInNextFrame.Checked;
            var quotesInfo = new QuotesInfo();
            var quotes = QuoteLoader.GetIntradayQuotes(null, minuteQuotes, iParameters, quotesInfo)
                .Select(a => new StatisticsQuote(a, iParameters)).ToArray();

            ShowStatus($"Saving data to to 'Bfr_StatQuote' table of database...");

            SaveToDb.ClearAndSaveToDbTable(quotes, "Bfr_StatQuote", "Symbol", "Date", "TimeFrameId", "Time", "Open",
                "High", "Low", "Close", "Volume", "BuyPerc", "SellPerc", "BuyAmt", "SellAmt", "BuyWins", "SellWins",
                "Week", "DayOfWeek", "Stop", "IsStopPerc", "Fees", "PriceId");

            Debug.Print($"*** After GetIntradayQuotes. StopWatch: {sw.ElapsedMilliseconds:N0}. Used memory: {CsUtils.MemoryUsedInBytes:N0}");

            sw.Stop();
            ShowStatus($"Finished! Data saved to 'Bfr_StatQuote' table of database. Duration: {sw.ElapsedMilliseconds:N0} milliseconds");
        }


        private void btnIntradayPrintDetails_Click(object sender, EventArgs e)
        {
            // var quotes = GetIntradayQuotes().Where(a => a.Symbol == "PULS").OrderBy(a => a.Timed).ToList();
        }

        private IntradayParameters IntradayGetParameters()
        {
            var p = new IntradayParameters
            {
                TimeFrames = IntradayGetTimeFrames(), CloseInNextFrame = cbCloseInNextFrame.Checked,
                Fees = nudIntradayFees.Value, Stop = nudIntradayStop.Value,
                IsStopPercent = cbIntradayStopInPercent.Checked
            };
            return p;
        }

        private string IntradayGetExcelFileName(string dataFileName, string fileNamePrefix, IntradayParameters iParameters)
        {
            var aa = Path.GetFileNameWithoutExtension(dataFileName).Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            var excelFilename = Settings.MinuteYahooReportFolder + fileNamePrefix + "_" + aa[aa.Length - 1] + "-" +
                                iParameters.GetFileNameSuffix() + ".xlsx";
            return excelFilename;
        }

        private string xIntradayGetTimeFramesInfo(IList<TimeSpan> timeFrames, bool closeInNextFrame)
        {
            var sbParameters = new StringBuilder();
            if (timeFrames.Count > 1)
                sbParameters.Append($"Time frames: {CsUtils.GetString(timeFrames[0])}-{CsUtils.GetString(timeFrames[timeFrames.Count - 1])}, interval: {CsUtils.GetString(timeFrames[1] - timeFrames[0])}");
            else if (timeFrames.Count == 1)
                sbParameters.Append($"Time frames: {CsUtils.GetString(timeFrames[0])}");

            if (closeInNextFrame)
                sbParameters.Append(", closeInNextFrame");

            return sbParameters.ToString();
        }

        private List<TimeSpan> IntradayGetTimeFrames()
        {
            var interval = new TimeSpan(0, Convert.ToInt32(nudInterval.Value), 0);
            var from = new TimeSpan(Convert.ToInt32(nudFromHour.Value), Convert.ToInt32(nudFromMinute.Value), 0);
            var to = new TimeSpan(Convert.ToInt32(nudToHour.Value), Convert.ToInt32(nudToMinute.Value), 0);
            string error = null;
            if (from > to)
                error = "Time frame error: 'From' value must be less than 'To' value";
            else if (interval.TotalMinutes < 1)
                error = "Time frame error: 'Interval' value must be greater than or equal to 1";
            else if (interval.TotalMinutes > (to-from).TotalMinutes)
                error = "Time frame error: 'Interval' value must be less than or equal to difference between 'To' and 'From'";
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var timeFrames = CsUtils.GetTimeFrames(from, to, interval);
            return timeFrames;
        }

        private void IntradayPrintReportLines(List<object[]> reportLines)
        {
            Debug.Print("===================================================================");
            foreach (var value in reportLines)
            {
                var sb = new StringBuilder();
                foreach (var a in value)
                    sb.Append((sb.Length == 0 ? "" : "\t") + CsUtils.GetString(a));

                Debug.Print(sb.ToString());
            }
        }
        #endregion

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
            ss.Insert(1, new[] { "Time frames: 09:30-16:00, interval: 00:30" });

            var data = new Dictionary<string, List<object[]>>();
            data.Add("ByTimeX", ss);

            Helpers.ExcelTest.AATable(data);
            ShowStatus("Finished!");*/
        }

        private void ExcelTest()
        {
            var iParameters = IntradayGetParameters();
            if (iParameters.TimeFrames == null) return;

            var zipFile = CsUtils.OpenFileDialogGeneric(Settings.MinuteYahooCacheFolder, @"Cache*.zip file (*.zip)|Cache*.zip");
            if (string.IsNullOrEmpty(zipFile)) return;

            var quotesInfoMinute = new QuotesInfo();
            var minuteQuotes = QuoteLoader.MinuteYahoo_GetQuotesFromZipCache(ShowStatus, zipFile, true, quotesInfoMinute);

            var quotesInfo = new QuotesInfo();
            var quotes = QuoteLoader.GetIntradayQuotes(null, minuteQuotes, iParameters, quotesInfo).ToArray();

            var reportLines = IntradayResults.ByTime(quotes, iParameters);
            IntradayPrintReportLines(reportLines);

            var data = new Dictionary<string, ExcelHelper.StatisticsData>();
            var statisticsData = new ExcelHelper.StatisticsData()
            {
                Title = "ByTimeX", Header1 = quotesInfo.GetStatus(), Header2 = iParameters.GetTimeFramesInfo(),
                Header3 = iParameters.GetFeesInfo(), Table = reportLines
            };
            data.Add("ByTimeX", statisticsData);
            var excelFileName = IntradayGetExcelFileName(zipFile, "Test", iParameters);
            Helpers.ExcelHelper.SaveStatisticsToExcel(data, excelFileName);

            ShowStatus($"Finished! Filename: {excelFileName}");
        }

        private void btnCompareMinuteYahooZips_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenFileDialogMultiselect(Settings.MinuteYahooDataFolder,
                    @"YahooMinute_202*.zip file (*.zip)|YahooMinute_202*.zip", "Select two YahooMinute files") is string[] files &&
                files.Length > 0)
            {
                if (files.Length != 2)
                    MessageBox.Show("You should to select 2 YahooMinute files");
                else
                {
                    Check.MinuteYahoo_CompareZipFiles(ShowStatus, files[0], files[1]);
                }
            }

        }

        private void btnMinuteYahooErrorCheck_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenFileDialogMultiselect(Settings.MinuteYahooDataFolder, @"YahooMinute_202*.zip file (*.zip)|YahooMinute_202*.zip") is string[] files && files.Length > 0)
                Check.MinuteYahoo_ErrorCheck(files, ShowStatus);
        }
    }
}
