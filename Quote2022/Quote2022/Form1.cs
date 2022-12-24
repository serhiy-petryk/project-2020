using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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

        private void btnTemp_Click(object sender, EventArgs e)
        {
            var fn = @"E:\Quote\WebData\Minute\Nasdaq\ts-A-20221222-0930.2.json";
            var oo = JsonConvert.DeserializeObject<TimeSalesNasdaq>(File.ReadAllText(fn));
            return;

            /*var folder = @"E:\Quote\WebData\Minute\Nasdaq\TS_20221222";
            var files = Directory.GetFiles(folder, "*.json");
            foreach (var file in files)
            {
                var newName = file.Replace("-20221221-", "-20221222-");
                File.Move(file, newName);
            }
            return;*/

            var file2 = @"E:\Quote\WebData\Minute\Nasdaq\TS_20221222.ToCompare_1200.zip";
            var file1 = @"E:\Quote\WebData\Minute\Nasdaq\TS_20221222.ToCompare.zip";

            var aa1 = new Dictionary<string, int>();
            var aa2 = new Dictionary<string, int>();

            using (var zip = new ZipReader(file1))
                foreach (var file in zip.Where(a => a.FullName.EndsWith(".json", true, CultureInfo.InvariantCulture)))
                {
                    aa1.Add(file.FileNameWithoutExtension, file.Content.Length);
                }
            using (var zip = new ZipReader(file2))
                foreach (var file in zip.Where(a => a.FullName.EndsWith(".json", true, CultureInfo.InvariantCulture)))
                {
                    aa2.Add(file.FileNameWithoutExtension, file.Content.Length);
                }

            foreach (var kvp in aa1)
            {
                if (aa2.ContainsKey(kvp.Key))
                {
                    if (aa2[kvp.Key] != kvp.Value)
                        Debug.Print($"Different Len: {kvp.Key}. Len1: {kvp.Value}. Len2: {aa2[kvp.Key]}");
                }
                else
                    Debug.Print($"Missing file in set2: {kvp.Key}. Len: {kvp.Value}.");
            }

            foreach (var kvp in aa2)
            {
                if (!aa2.ContainsKey(kvp.Key))
                    Debug.Print($"Missing file in set1: {kvp.Key}. Len: {kvp.Value}.");
            }

            return;

            // Download.ProfilesQuantumonline_Download(ShowStatus);
            // return;
            // Parse.SymbolsQuantumonlineZip_CheckMinLevel(ShowStatus);
            // return;
            Parse.SymbolsQuantumonlineZip_Parse(ShowStatus);
            return;
            // Download.SymbolsQuantumonline_Rename(ShowStatus);
            // return;
            // var aa1 = Parse.SymbolsQuantumonline_Parse(@"E:\Temp\Quote\Test\s!.html", ShowStatus);
            // return;
            Download.SymbolsQuantumonline_Download(ShowStatus);
            return;

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
            //if (CsUtils.OpenCsvFileDialog(Settings.ScreenerNasdaqFolder) is string fn && !string.IsNullOrEmpty(fn))
              //  Parse.ScreenerNasdaq_ParseAndSaveToDb(fn, ShowStatus);
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
            // Parse.ProfileQuantumonline_Parse(@"E:\Quote\WebData\Symbols\Quantumonline\Profiles\Test.zip", ShowStatus);
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
            // Parse.SymbolsNasdaq_ParseAndSaveToDb(@"E:\Quote\WebData\Symbols\Nasdaq\NasdaqLookup_20221217.zip", ShowStatus);
            // return;
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
            // Parse.SymbolsEoddata_ParseAndSaveToDb(ShowStatus);
            if (CsUtils.OpenZipFileDialog(Settings.SymbolsEoddataFolder) is string fn && !string.IsNullOrEmpty(fn))
                Parse.SymbolsEoddata_ParseAndSaveToDb(fn, ShowStatus);
        }

        private void btnTimeSalesNasdaqDownload_Click(object sender, EventArgs e)
        {
            Download.TimeSalesNasdaq_Download(ShowStatus);
        }

        private void btnSymbolsQuantumonlineDownload_Click(object sender, EventArgs e) => Download.SymbolsQuantumonline_Download(ShowStatus);

        private void btnSymbolsQuantumonlineParse_Click(object sender, EventArgs e) => Parse.SymbolsQuantumonlineZip_Parse(ShowStatus);

        private void btnTimeSalesNasdaqCheck_Click(object sender, EventArgs e)
        {
            var file2 = @"E:\Quote\WebData\Minute\Nasdaq\TS_20221222.ToCompare_1200.zip";
            var file1 = @"E:\Quote\WebData\Minute\Nasdaq\TS_20221222.ToCompare.zip";

            var aa1 = new Dictionary<string, TimeSalesNasdaq.Sale[]>();
            var aa2 = new Dictionary<string, TimeSalesNasdaq.Sale[]>();

            using (var zip = new ZipReader(file1))
                foreach (var file in zip.Where(a => a.FullName.EndsWith(".json", true, CultureInfo.InvariantCulture)))
                {
                    var s = file.Content.StartsWith("{") ? file.Content : file.Content.Substring(1);
                    var oo = JsonConvert.DeserializeObject<TimeSalesNasdaq>(s);
                    if (oo.status.bCodeMessage != null)
                    {
                        foreach (var eroor in oo.status.bCodeMessage.Where(a => a.code != 1001))
                            Debug.Print($"Status: {oo.status.bCodeMessage[0].errorMessage}. Status: {oo.status.bCodeMessage[0].code}. File: {file.FileNameWithoutExtension}. zip: 1");
                    }
                    aa1.Add(file.FileNameWithoutExtension, oo.GetSales());
                }
            using (var zip = new ZipReader(file2))
                foreach (var file in zip.Where(a => a.FullName.EndsWith(".json", true, CultureInfo.InvariantCulture)))
                {
                    var s = file.Content.StartsWith("{") ? file.Content : file.Content.Substring(1);
                    var oo = JsonConvert.DeserializeObject<TimeSalesNasdaq>(s);
                    if (oo.status.bCodeMessage != null)
                    {
                        foreach (var eroor in oo.status.bCodeMessage.Where(a => a.code != 1001))
                            Debug.Print($"Status: {oo.status.bCodeMessage[0].errorMessage}. Status: {oo.status.bCodeMessage[0].code}. File: {file.FileNameWithoutExtension}. zip: 2");
                    }
                    aa2.Add(file.FileNameWithoutExtension, oo.GetSales());
                }

            foreach (var kvp in aa1)
            {
                if (aa2.ContainsKey(kvp.Key))
                {
                    var b = TimeSalesNasdaq.IsEqual(kvp.Value, aa2[kvp.Key]);
                    if (b.HasValue && !b.Value)
                    {

                    }
                }
            }

            /*
            foreach (var kvp in aa1)
            {
                if (aa2.ContainsKey(kvp.Key))
                {
                    if (aa2[kvp.Key] != kvp.Value)
                        Debug.Print($"Different Len: {kvp.Key}. Len1: {kvp.Value}. Len2: {aa2[kvp.Key]}");
                }
                else
                    Debug.Print($"Missing file in set2: {kvp.Key}. Len: {kvp.Value}.");
            }

            foreach (var kvp in aa2)
            {
                if (!aa2.ContainsKey(kvp.Key))
                    Debug.Print($"Missing file in set1: {kvp.Key}. Len: {kvp.Value}.");
            }*/

            return;

        }

        private void btnTimeSalesNasdaqReload_Click(object sender, EventArgs e)
        {
            Download.TimeSalesNasdaq_Reload(ShowStatus);
        }

        private void btnRefreshSpitsData_Click(object sender, EventArgs e)
        {
            ShowStatus($"RefreshSpitsData is starting");
            SaveToDb.RunProcedure("pRefreshSplits");
            ShowStatus($"RefreshSplitsData FINISHED!");
        }
    }
}
