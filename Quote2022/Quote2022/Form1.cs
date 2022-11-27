using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
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
            var data = new List<SymbolsNanex>();
            Parse.SymbolsNanex_Parse(files, data, ShowStatus);
            ShowStatus($"Save Nanex Symbols");
            SaveToDb.SymbolsNanex_SaveToDb(data);
            ShowStatus($"Nanex Symbols: FINISHED!");
        }

        private void btnDayEoddataParse_Click(object sender, EventArgs e) => Parse.DayEoddata_Parse(ShowStatus);

        private void btnSymbolsEoddataParse_Click(object sender, EventArgs e) => Parse.SymbolsEoddata_Parse(ShowStatus);

        private void btnTemp_Click(object sender, EventArgs e)
        {
            var path1 = @"E:\Quote\WebData\Minute\x2YahooMinute_20221126\";
            var path2 = @"E:\Quote\WebData\Minute\YahooMinute_20221126\";
            var files = Directory.GetFiles(path1);
            foreach (var file in files)
            {
                var content1 = File.ReadAllText(file);
                var content2 = File.ReadAllText(path2 + Path.GetFileName(file));
                /*if (!string.Equals(content1, content2))
                {
                    var l1 = content1.Length;
                    var l2 = content2.Length;
                    var aa1 = content1.ToCharArray();
                    var aa2 = content2.ToCharArray();
                    for (var k = 0; k < aa1.Length; k++)
                    {
                        if (aa1[k] != aa2[k])
                            Debug.Print(k.ToString() + "\t" + aa1[k] + "\t" + aa2[k] + "\t" + Path.GetFileName(file));
                    }
                }*/
                if (content1.Length != content2.Length)
                {
                    Debug.Print(content1.Length + "\t" + content2.Length + "\t" + Path.GetFileName(file));
                }
            }

            MessageBox.Show("Finished!");
        }

        private void btnDayTickertechParse_Click(object sender, EventArgs e) => Parse.DayTickertech_Parse(ShowStatus, false);
        private void btnSplitTickertechParse_Click(object sender, EventArgs e) => Parse.DayTickertech_Parse(ShowStatus, true);

        private void btnSymbolsTickertechParse_Click(object sender, EventArgs e) => Parse.SymbolsTickertech_Parse(ShowStatus);

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

        private void button1_Click(object sender, EventArgs e)
        {
            Helpers.YahooMinute.Test();
        }

        private void btnDailyEoddataCheck_Click(object sender, EventArgs e) => Parse.DayEoddata_Check(ShowStatus);

    }
}
