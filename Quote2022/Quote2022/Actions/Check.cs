using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static class Check
    {
        public static void MinuteYahoo_CheckData(string[] zipFiles, Action<string> showStatusAction)
        {
            var cnt = 0;
            foreach (var q in QuoteLoader.GetYahooIntradayQuotesFromZipFiles(showStatusAction, zipFiles, null).Where(a=>!string.IsNullOrEmpty(a.QuoteError)))
            {
                Debug.Print(q.QuoteError + "\t" + q.ToString());
                cnt++;
            }
            showStatusAction($"MinuteYahoo_CheckData FINISHED! Found {cnt} quotes with error. See 'Output window'");
        }

        public static void MinuteYahoo_SaveLog(string[] zipFiles, Action<string> showStatusAction)
        {
            var log = new Dictionary<string, Dictionary<DateTime, object[]>>();
            if (zipFiles == null)
                zipFiles = Directory.GetFiles(Settings.MinuteYahooFolder, "*.zip");
            Array.Sort(zipFiles);

            var cnt = 0;
            foreach (var zipFile in zipFiles)
            {
                showStatusAction($"MinuteYahoo_SaveLog is working for {Path.GetFileName(zipFile)}");

                var dates = new Dictionary<DateTime, object[]>();
                log.Add(Path.GetFileName(zipFile), dates);
                using (var zip = new ZipReader(zipFile))
                    foreach (var item in zip)
                        if (item.Length > 0)
                        {
                            if (!item.FileNameWithoutExtension.ToUpper().StartsWith("YMIN-"))
                                throw new Exception(
                                    $"Bad file name in {zipFile}: {item.FileNameWithoutExtension}. Must starts with 'yMin-'");

                            cnt++;
                            if ((cnt % 100) == 0)
                                showStatusAction($"MinuteYahoo_SaveLog is working for {Path.GetFileName(zipFile)}. Total file processed: {cnt:N0}");

                            var symbol = item.FileNameWithoutExtension.Substring(5);
                            var o = JsonConvert.DeserializeObject<Models.MinuteYahoo>(item.Content);
                            var minuteQuotes = o.GetQuotes(symbol);
                            var groups = minuteQuotes.GroupBy(a => a.Timed.Date);
                            foreach (var group in groups)
                            {
                                var groupByTime = group.GroupBy(a => a.Timed.RoundDown(new TimeSpan(0, 30, 0)).TimeOfDay).ToArray();
                                var groupByTimeCount = new int[13];
                                foreach (var gKey in groupByTime)
                                {
                                    if (gKey.Key == new TimeSpan(9, 30, 0))
                                        groupByTimeCount[0] = gKey.Count();
                                    else if (gKey.Key == new TimeSpan(10, 00, 0))
                                        groupByTimeCount[1] = gKey.Count();
                                    else if (gKey.Key == new TimeSpan(10, 30, 0))
                                        groupByTimeCount[2] = gKey.Count();
                                    else if (gKey.Key == new TimeSpan(11, 00, 0))
                                        groupByTimeCount[3] = gKey.Count();
                                    else if (gKey.Key == new TimeSpan(11, 30, 0))
                                        groupByTimeCount[4] = gKey.Count();
                                    else if (gKey.Key == new TimeSpan(12, 00, 0))
                                        groupByTimeCount[5] = gKey.Count();
                                    else if (gKey.Key == new TimeSpan(12, 30, 0))
                                        groupByTimeCount[6] = gKey.Count();
                                    else if (gKey.Key == new TimeSpan(13, 00, 0))
                                        groupByTimeCount[7] = gKey.Count();
                                    else if (gKey.Key == new TimeSpan(13, 30, 0))
                                        groupByTimeCount[8] = gKey.Count();
                                    else if (gKey.Key == new TimeSpan(14, 00, 0))
                                        groupByTimeCount[9] = gKey.Count();
                                    else if (gKey.Key == new TimeSpan(14, 30, 0))
                                        groupByTimeCount[10] = gKey.Count();
                                    else if (gKey.Key == new TimeSpan(15, 00, 0))
                                        groupByTimeCount[11] = gKey.Count();
                                    else if (gKey.Key == new TimeSpan(15, 30, 0))
                                        groupByTimeCount[12] = gKey.Count();
                                    else
                                        throw new Exception($"MinuteYahoo_SaveLog. Check groupByTimeCount array");
                                }

                                if (!dates.ContainsKey(group.Key))
                                    dates.Add(group.Key,
                                        new object[]
                                        {
                                            group.Min(a => a.Timed.TimeOfDay), group.Max(a => a.Timed.TimeOfDay), 1,
                                            group.Count(), groupByTimeCount
                                        });
                                else
                                {
                                    var oo = dates[group.Key];
                                    var minTime = group.Min(a => a.Timed.TimeOfDay);
                                    var maxTime = group.Max(a => a.Timed.TimeOfDay);
                                    if (minTime < (TimeSpan) oo[0]) oo[0] = minTime;
                                    if (maxTime > (TimeSpan) oo[1]) oo[1] = maxTime;
                                    oo[2] = (int) oo[2] + 1;
                                    oo[3] = (int) oo[3] + group.Count();
                                    var gg = (int[]) oo[4];
                                    for (var k = 0; k < groupByTimeCount.Length; k++)
                                        gg[k] += groupByTimeCount[k];
                                }
                            }

                        }
            }

            var logFileName = Settings.MinuteYahooFolder + "log.txt";
            if (File.Exists(logFileName))
                File.Delete(logFileName);

            File.AppendAllLines(logFileName, new [] { "File\tDate\tMinTime\tMaxTime\tSymbolCount\tQuoteCount\t09:30\t10:00\t10:30\t11:00\t11:30\t12:00\t12:30\t13:00\t13:30\t14:00\t14:30\t15:00\t15:30" });

            foreach (var o1 in log)
            {
                File.AppendAllLines(logFileName,
                    o1.Value.Select(a =>
                        o1.Key + "\t" + CsUtils.GetString(a.Key) + "\t" + a.Value[0] + "\t" + a.Value[1] + "\t" +
                        a.Value[2] + "\t" + a.Value[3] + "\t" +
                        string.Join("\t", ((int[]) a.Value[4]).Select(a1 => a1.ToString()))));
            }
            showStatusAction($"MinuteYahoo_SaveLog FINISHED!");
        }

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
