using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Quote2022.Actions;
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

        private void btnNasdaqStockScreener_Click(object sender, EventArgs e)
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
            ShowStatus($"Started");
            ShowStatus($"FINISHED!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Helpers.YahooMinute.Test();
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
            if (CsUtils.OpenFileDialogGeneric(Settings.SymbolsYahooLookupFolder, @"zip files (*.zip)|*_202?????.zip") is string fn && !string.IsNullOrEmpty(fn))
                Parse.SymbolsYahooLookup_ParseAndSaveToDb(fn, ShowStatus);
        }
    }
}
