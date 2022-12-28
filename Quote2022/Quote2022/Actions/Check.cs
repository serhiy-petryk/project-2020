using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static class Check
    {
        public static void TimeSalesNasdaq_SaveLog(string folder, bool overwrite, Action<string> showStatusAction)
        {

            var logFileName = Path.Combine(folder, "filesLog.txt");
            if (File.Exists(logFileName) && !overwrite)
                return;

            var files = Directory.GetFiles(folder, "*.json");
            var numberOfFiles = new Dictionary<string, int>();
            string timeStamp = null;
            foreach (var file in files)
            {
                var ss = Path.GetFileName(file).Split('_');
                var symbol = ss[1];
                if (timeStamp == null)
                {
                    timeStamp = ss[2];
                }

                if (!numberOfFiles.ContainsKey(symbol))
                    numberOfFiles.Add(symbol, 1);
                else
                    numberOfFiles[symbol]++;
            }

            var log = new Dictionary<string, string>();
            foreach (var kvp in numberOfFiles)
                log.Add(kvp.Key, kvp.Value == 13 ? null : "Loaded not all files");

            var fileTemplate = Path.GetFileName(Settings.TimeSalesNasdaqFileTemplate);
            var kvps = log.Where(a => string.IsNullOrEmpty(a.Value)).ToArray();
            var nextDayCount = 0;
            foreach (var kvp in kvps)
            {
                TimeSalesNasdaq.TopTableRow dayKey = null;
                foreach (var time in TimeSalesNasdaq.UrlTimes)
                {
                    var filename = Path.Combine(folder,
                        string.Format(fileTemplate, time.Replace(":", ""), kvp.Key, timeStamp));
                    var content = File.ReadAllText(filename);
                    if (content.StartsWith(@"<!DOCTYPE html>"))
                    {
                        log[kvp.Key] = $"{Path.GetFileName(filename)}\tBadContent";
                        break;
                    }

                    var o = JsonConvert.DeserializeObject<TimeSalesNasdaq>(content);
                    if (o == null)
                    {
                        log[kvp.Key] = $"{Path.GetFileName(filename)}\tCanNotDeserialize";
                        break;
                    }

                    if (o.data == null)
                    {
                        log[kvp.Key] = $"{Path.GetFileName(filename)}\t{o.status.bCodeMessage[0].errorMessage}";
                        break;
                    }

                    if (dayKey == null)
                        dayKey = o.DaySummaryInfo;

                    var diff = o.GetSummarryDifference(dayKey);
                    if (diff != null)
                    {
                        log[kvp.Key] =
                            $"{Path.GetFileName(filename)}\t{diff}\t{o.DaySummaryInfo}\t{dayKey}";
                        break;
                    }

                    if (o.data.topTable.rows[0].todayHighLow == "N/A")
                        nextDayCount++;

                }
            }

            // Save log
            if (File.Exists(logFileName))
                File.Delete(logFileName);

            File.AppendAllText(logFileName, $"{folder}\tLog" + Environment.NewLine);
            if (nextDayCount != 0)
            {
                File.AppendAllText(logFileName, $"{nextDayCount}\tNextDayCount" + Environment.NewLine);
            }

            var cnt = 0;
            foreach (var kvp in log)
            {
                File.AppendAllText(logFileName, cnt + "\t" + kvp.Key + "\t" + kvp.Value + Environment.NewLine);
                cnt++;
            }
        }

        public static void TimeSalesNasdaq_SaveSummary(string folder, bool overwrite, Action<string> showStatusAction)
        {
            var summaryFileName = Path.Combine(folder, "Summary.txt");
            if (File.Exists(summaryFileName) && !overwrite)
                return;

            var summaryLines = new List<string>();
            summaryLines.Add($"Symbol\tFiles\tBadFiles\tTrades\tOpen\tHigh\tLow\tClose\tVolume");

            showStatusAction($"TimeSalesNasdaq_SaveSummary is working for {folder}");

            // File.AppendAllText(summaryWipFileName, $"Symbol\tFiles\tBadFiles\tTrades\tOpen\tHigh\tLow\tClose\tVolume" + Environment.NewLine);

            var files = Directory.GetFiles(folder, "*.json");
            Array.Sort(files);

            var symbolList = new Dictionary<string, object>();
            string timeStamp = null;
            foreach (var file in files)
            {
                var ss = Path.GetFileName(file).Split('_');
                var symbol = ss[1];
                if (timeStamp == null)
                {
                    timeStamp = ss[2];
                }

                if (!symbolList.ContainsKey(symbol))
                    symbolList.Add(symbol, null);
            }

            var fileTemplate = Path.GetFileName(Settings.TimeSalesNasdaqFileTemplate);

            foreach (var symbol in symbolList.Keys)
            {
                var badFiles = 0;
                var fileCount = 0;
                var tradeCount = 0;
                float? open = null;
                float? high = null;
                float? low = null;
                float? close = null;
                long? volume = null;
                TimeSalesNasdaq.TopTableRow dayKey = null;
                foreach (var time in TimeSalesNasdaq.UrlTimes)
                {
                    var filename = Path.Combine(folder, string.Format(fileTemplate, time.Replace(":", ""), symbol, timeStamp));
                    if (!File.Exists(filename))
                    {
                        badFiles++;
                        continue;
                    }

                    fileCount++;
                    var content = File.ReadAllText(filename);
                    if (content.StartsWith(@"<!DOCTYPE html>"))
                    {
                        badFiles++;
                        continue;
                    }

                    var o = JsonConvert.DeserializeObject<TimeSalesNasdaq>(content);
                    if (o == null || o.data == null)
                    {
                        badFiles++;
                        continue;
                    }

                    var trades = o.GetTrades();
                    tradeCount += trades.Length;
                    for (var k = trades.Length - 1; k >= 0; k--)
                    {
                        volume = (volume ?? 0) + trades[k].Volume;
                        if (!open.HasValue)
                            open = trades[k].Price;
                        if (!high.HasValue)
                            high = trades[k].Price;
                        if (!low.HasValue)
                            low = trades[k].Price;
                        close = trades[k].Price;

                        if (high.Value < trades[k].Price)
                            high = trades[k].Price;
                        if (low.Value > trades[k].Price)
                            low = trades[k].Price;
                    }
                }

                // Save summary
                //$"Symbol\tFiles\tBadFiles\tTrades\tOpen\tHigh\tLow\tClose\tVolume"
                summaryLines.Add($"{symbol}\t{fileCount}\t{badFiles}\t{tradeCount}\t{open}\t{high}\t{low}\t{close}\t{volume}");
            }

            if (File.Exists(summaryFileName)) File.Delete(summaryFileName);
            File.AppendAllLines(summaryFileName, summaryLines);
        }
    }
}
