using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        private void btnDayYahooIndexesParse_Click(object sender, EventArgs e) => Parse.DayYahooIndexes(ShowStatus);

        private void btnSymbolsNanex_Click(object sender, EventArgs e)
        {
            var files = Download.SymbolsNanex_Download(ShowStatus);
            // var files = Directory.GetFiles(@"E:\Quote\WebData\Symbols\Nanex", "*_20221208.txt");
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

        private void btnSymbolsEoddataParse_Click(object sender, EventArgs e) => Parse.SymbolsEoddata_Parse(ShowStatus);

        private void btnTemp_Click(object sender, EventArgs e)
        {
            var path = @"E:\Quote\WebData\Splits\Yahoo\YahooSplits_20221203\";
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                File.Move(file, file.Replace("-", ""));
            }

            MessageBox.Show("Finished!");
        }

        private void btnSplitYahooParse_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenZipFileDialog(Settings.SplitYahooFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.SplitYahoo_Parse(fn, ShowStatus);
        }

        private void btnSplitInvestingParse_Click(object sender, EventArgs e)
        {
            /*var files = Directory.GetFiles(Settings.SplitInvestingFolder, "*.txt");
            Array.Sort(files);
            foreach (var file in files)
                Parse.SplitInvesting_Parse(file, ShowStatus);
            return;*/

            if (CsUtils.OpenTxtFileDialog(Settings.SplitInvestingFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.SplitInvesting_Parse(fn, ShowStatus);
        }

        private void btnSplitEoddataParse_Click(object sender, EventArgs e)
        {
            /*var files = Directory.GetFiles(Settings.SplitEoddataFolder, "*.txt");
            Array.Sort(files);
            foreach (var file in files)
                Parse.SplitEoddata_Parse(file, ShowStatus);
            return;*/

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

        private void button1_Click(object sender, EventArgs e)
        {
            Helpers.YahooMinute.Test();
        }

        private void btnDailyEoddataCheck_Click(object sender, EventArgs e) => Parse.DayEoddata_Check(ShowStatus);

        private void btnNasdaqStockScreener_Click(object sender, EventArgs e)
        {
            if (CsUtils.OpenCsvFileDialog(Settings.ScreenerNasdaqFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.ScreenerNasdaq_Parse(fn, ShowStatus);
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
    }
}
