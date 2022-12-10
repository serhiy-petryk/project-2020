using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            // var aa1 = Parse.SymbolsQuantumonlineList_Parse(@"E:\Temp\Quote\Test\s!.html", ShowStatus);
            // return;
            Download.SymbolsQuantumonline_Download(ShowStatus);
            return;
            /*var cc = new char[]
            {
                (char) 35, (char) 59, (char) 61, (char) 91, (char) 93, (char) 94, (char) 95, (char) 96, (char) 123,
                (char) 125, (char) 126, (char) 127, (char) 71, (char) 69, (char) 78, (char) 82, (char) 65, (char) 76,
                (char) 32, (char) 77, (char) 79, (char) 84, (char) 83, (char) 67, (char) 46, (char) 44, (char) 52,
                (char) 55, (char) 53, (char) 37, (char) 73, (char) 66, (char) 68, (char) 86, (char) 74, (char) 85,
                (char) 80, (char) 70, (char) 75, (char) 49, (char) 50, (char) 48, (char) 51, (char) 72, (char) 89,
                (char) 45, (char) 56, (char) 87, (char) 54, (char) 81, (char) 90, (char) 88, (char) 38, (char) 39,
                (char) 57, (char) 199, (char) 195, (char) 40, (char) 41, (char) 36, (char) 43, (char) 33, (char) 8212,
                (char) 174, (char) 8217, (char) 8211, (char) 64, (char) 201, (char) 209, (char) 205, (char) 214
            };

            var pathTemplate = @"E:\Temp\Quote\Test\s{0}.txt";
            foreach (var c in cc)
                File.Create(string.Format(pathTemplate, c.ToString()));
            */

            var path = @"E:\Quote\WebData\Symbols\Quantumonline4";
            var files = Directory.GetFiles(path);
            var letters = new Dictionary<char, object>();
            var cnt = 0;
            foreach (var file in files)
            {
                cnt++;
                var content = File.ReadAllText(file);
                if (content.Contains("Sorry, but there were no matches for")) { }
                else if (content.Contains("Matching Security Names for"))
                {
                    var i1 = content.IndexOf("Matching Security Names for", StringComparison.InvariantCulture);
                    var i2 = content.IndexOf("</table", i1+ 10, StringComparison.InvariantCulture);
                    var rows = content.Substring(i1, i2 - i1).Split(new string[] {"</tr>"}, StringSplitOptions.None);
                    for (var k = 1; k < rows.Length - 1; k++)
                    {
                        var row = rows[k].Trim();
                        if (string.IsNullOrEmpty(row)) continue;
                        var cells = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        var name = cells[1].Replace("</font>","").Trim();
                        i1 = name.LastIndexOf('>');
                        name = name.Substring(i1 + 1).Trim().ToUpper();
                        i1 = name.IndexOfAny(new char[] { '\r', '\n' });
                        if (i1 != -1)
                            name = name.Substring(0, i1);

                        foreach (var c in name.ToCharArray())
                        {
                            if (c == 10 || c == 13)
                            {
                                Debug.Print($"New line: {file}");
                            }
                            if (c == 9)
                            {
                                Debug.Print($"Tab: {file}. Name: {name}");
                            }
                            if (!letters.ContainsKey(c))
                            {
                                Debug.Print("Letter: " + c + $" ({(int)c})");
                                letters.Add(c, null);
                            }

                        }
                        //if (name.EndsWith(@"</font>")
                    }

                }
                else if (content.Contains("An error occurred while executing the application")) { }
                else
                {
                    
                }
            }

            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
            {
                if (letters.ContainsKey(c))
                    Debug.Print("Invalid char: " + $" ({(int)c})");
            }

            var freeChars = new Dictionary<char, object>();
            for (var k = 1; k < 128; k++)
            {

                if (k < 'a' || k > 'z')
                    freeChars.Add((char)k, null);
            }

            foreach (var c in invalidChars)
                if (freeChars.ContainsKey(c))
                    freeChars.Remove(c);

            foreach (var c in letters.Keys)
                if (freeChars.ContainsKey(c))
                    freeChars.Remove(c);

            // Url chars
            Debug.Print("Url chars:");
            foreach (var c in letters.Keys.OrderBy(a=>a))
                Debug.Print($"{(int)c}");

            // Free chars
            Debug.Print("Free chars:");
            foreach (var c in freeChars.Keys)
                Debug.Print($"{(int)c}");

            foreach (var c in invalidChars)
            {
                if (freeChars.ContainsKey(c))
                    Debug.Print("Invalid char: " + c + $" ({(int)c})");
            }

            // Valid chars
            Debug.Print("Valid chars:");
            foreach (var c in letters.Keys)
            {
                if (!invalidChars.Contains(c))
                    Debug.Print($"{(int)c}");
            }

            /*char[] alpha = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ ".ToCharArray();
            var ss = new List<string>();
            foreach (var a1 in alpha)
            foreach (var a2 in alpha)
            {
                ss.Add(a1.ToString() + a2.ToString());

            }

            var ss1 = ss.ToArray();
            foreach (var a1 in alpha)
            foreach (var a2 in ss1)
            {
                ss.Add(a1.ToString() + a2);

            }

            foreach(var a1 in ss)
                Debug.Print(a1);*/

            /*var path = @"E:\Quote\WebData\Splits\Yahoo\YahooSplits_20221203\";
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                File.Move(file, file.Replace("-", ""));
            }

            MessageBox.Show("Finished!");*/
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
