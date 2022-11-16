using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var data1 = new Dictionary<string, string>();
            var data2 = new Dictionary<string, string>();
            var path = @"E:\Temp\Quote\Splits\Yahoo_20221106";
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var newFile = file.Replace("ySplit-", "y");
                File.Move(file, newFile);
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

        private void btnAlgorithm1_Click(object sender, EventArgs e) => Helpers.Algorithm1.Execute(ShowStatus);
    }
}
