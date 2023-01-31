﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static class Check
    {
        public static void MinuteYahoo_CompareZipFiles(Action<string> showStatusAction, string firstFile,
            string secondFile)
        {
            var symbols = new Dictionary<string, int>();

            showStatusAction($"MinuteYahoo_CompareZipFiles. Read file entries of first file.");
            using (var zip = new ZipReader(firstFile))
            {
                symbols = zip.Where(a => a.Length > 0 && a.FileNameWithoutExtension.ToUpper().StartsWith("YMIN-"))
                    .Select(a => a.FileNameWithoutExtension.Substring(5)).ToDictionary(a => a, a => 1);
            }

            showStatusAction($"MinuteYahoo_CompareZipFiles. Read file entries of second file.");
            using (var zip = new ZipReader(secondFile))
            {
                foreach (var item in zip)
                    if (item.Length > 0 && item.FileNameWithoutExtension.ToUpper().StartsWith("YMIN-"))
                    {
                        var symbol = item.FileNameWithoutExtension.Substring(5);
                        if (symbols.ContainsKey(symbol))
                            symbols[symbol] += 2;
                        else
                            symbols.Add(symbol, 2);
                    }
            }

            var a1 = symbols.Where(a => a.Value != 3);
            var log = new List<string>();
            log.Add("Compare two MinuteYahoo zip files");
            log.Add($"First file:\t{Path.GetFileName(firstFile)}");
            log.Add($"Second file:\t{Path.GetFileName(secondFile)}");
            var priceLog = new List<string>(log);

            var flag = true;
            foreach (var kvp in symbols.Where(a => a.Value != 3))
            {
                if (flag)
                {
                    flag = false;
                    log.Add(null);
                }

                if (kvp.Value == 1)
                    log.Add($"{kvp.Key}\tsymbol missing in second file");
                else
                    log.Add($"{kvp.Key}\tsymbol missing in first file");
            }

            showStatusAction($"MinuteYahoo_CompareZipFiles. Reading context of first file ...");

            var symbolsToCompare = symbols.Where(a => a.Value == 3).ToDictionary(a => a.Key, a => (object) null);
            var fistFileQuotes = new Dictionary<string, List<Quote>>();
            using (var zip = new ZipReader(firstFile))
            {
                fistFileQuotes = zip
                    .Where(a => a.FileNameWithoutExtension.ToUpper().StartsWith("YMIN-") &&
                                symbolsToCompare.ContainsKey(a.FileNameWithoutExtension.Substring(5).ToUpper()))
                    .ToDictionary(a => a.FileNameWithoutExtension.Substring(5).ToUpper(),
                        a => JsonConvert.DeserializeObject<Models.MinuteYahoo>(a.Content)
                            .GetQuotes(a.FileNameWithoutExtension.Substring(5).ToUpper()));
            }


            var cnt = 0;
            log.Add(null);
            var count1 = 0;
            var count2 = 0;
            long volume1 = 0;
            long volume2 = 0;
            var volumeLog = new List<string> { null };
            using (var zip = new ZipReader(secondFile))
                foreach (var item in zip.Where(a=>a.Length>0))
                {
                    var symbol = item.FileNameWithoutExtension.Substring(5).ToUpper();
                    if (!fistFileQuotes.ContainsKey(symbol))
                        continue;

                    cnt++;
                    if ((cnt % 100) == 0)
                    {
                        showStatusAction(
                            $"MinuteYahoo_CompareZipFiles. Compared {cnt} items from {symbolsToCompare.Count} in two zip files.");
                    }

                    var o = JsonConvert.DeserializeObject<Models.MinuteYahoo>(item.Content);
                    var minuteQuotes2 = o.GetQuotes(symbol);

                    if (fistFileQuotes[symbol].Count != minuteQuotes2.Count)
                    {
                        count1 += fistFileQuotes[symbol].Count;
                        count2 += minuteQuotes2.Count;
                        log.Add($"{symbol}\t{fistFileQuotes[symbol].Count}\t{minuteQuotes2.Count}\t quotes in first and second files");
                    }

                    var vol1 = fistFileQuotes[symbol].Sum(a => a.Volume);
                    var vol2 = minuteQuotes2.Sum(a => a.Volume);
                    if (vol1 != vol2)
                    {
                        volume1 += vol1;
                        volume2 += vol2;
                        volumeLog.Add($"{symbol}\t{vol1}\t{vol2}\t volumes in first and second files");
                    }

                    var aa1 = fistFileQuotes[symbol].ToDictionary(a => new Tuple<string, DateTime>(a.Symbol, a.Timed), a => a);
                    var aa2 = minuteQuotes2.ToDictionary(a => new Tuple<string, DateTime>(a.Symbol, a.Timed), a => a);
                    var keys = aa1.Keys.Union(aa2.Keys).OrderBy(a=>a.Item1).ThenBy(a=>a.Item2).ToArray();
                    foreach (var key in keys)
                    {
                        var q1 = aa1.ContainsKey(key) ? aa1[key] : null;
                        var q2 = aa2.ContainsKey(key) ? aa2[key] : null;
                        if (q1!=null  && q2!=null)
                        {
                            if (Math.Abs(q1.Open - q2.Open) < 0.005 && Math.Abs(q1.High - q2.High) < 0.005 && Math.Abs(q1.Low - q2.Low) < 0.005 && Math.Abs(q1.Close - q2.Close) < 0.005) 
                                continue;
                        }
                        var sb = new StringBuilder();
                        sb.Append(key.Item1 + "\t" + key.Item2.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)+"\t");
                        if (q1 == null)
                            sb.Append($"\t\t\t\t\t");
                        else
                            sb.Append($"{q1.Open}\t{q1.High}\t{q1.Low}\t{q1.Close}\t{q1.Volume}\t");
                        if (q2 == null)
                            sb.Append($"\t\t\t\t\t");
                        else
                            sb.Append($"{q2.Open}\t{q2.High}\t{q2.Low}\t{q2.Close}\t{q2.Volume}\t");
                        if (q1!=null && q2!=null)
                            sb.Append($"{q1.Open - q2.Open}\t{q1.High - q2.High}\t{q1.Low - q2.Low}\t{q1.Close - q2.Close}\t{q1.Volume - q2.Volume}");
                        else
                            sb.Append($"\t\t\t\t");

                        priceLog.Add(sb.ToString());
                    }
                }
            log.Add($"TOTAL count difference\t{count1}\t{count2}");
            log.AddRange(volumeLog);
            log.Add($"TOTAL volume difference\t{volume1}\t{volume2}");


            var logFileName = Settings.MinuteYahooLogFolder + "CompareLog.txt";
            if (File.Exists(logFileName))
                File.Delete(logFileName);
            var priceLogFileName = Settings.MinuteYahooLogFolder + "CompareLog.Price.txt";
            if (File.Exists(priceLogFileName))
                File.Delete(priceLogFileName);

            File.AppendAllLines(logFileName, log);
            File.AppendAllLines(priceLogFileName, priceLog);

            showStatusAction($"MinuteYahoo_CompareZipFiles FINISHED! Log file: {logFileName}");

            string ToString(float f)
            {
                // return Math.Round(f,4).ToString());
                return "";
            }
        }

        public static void MinuteYahoo_CheckData(Action<string> showStatusAction, string[] zipFiles)
        {
            var cnt = 0;
            foreach (var q in QuoteLoader.MinuteYahoo_GetQuotesFromZipFiles(showStatusAction, zipFiles, false, false).Where(a => !string.IsNullOrEmpty(a.QuoteError)))
            {
                Debug.Print(q.QuoteError + "\t" + q.ToString());
                cnt++;
            }
            showStatusAction($"MinuteYahoo_CheckData FINISHED! Found {cnt} quotes with error. See 'Output window'");
        }

        public static void MinuteYahoo_SaveLog(string[] zipFiles, Action<string> showStatusAction)
        {
            var errors = new Dictionary<string, string>();
            var log = new Dictionary<string, Dictionary<DateTime, object[]>>();
            if (zipFiles == null)
                zipFiles = Directory.GetFiles(Settings.MinuteYahooLogFolder, "*.zip");
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
                            if (o.Chart.Error != null)
                            {
                                errors.Add(symbol, o.Chart.Error.Description ?? o.Chart.Error.Code);
                                continue;
                            }

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
                                    if (minTime < (TimeSpan)oo[0]) oo[0] = minTime;
                                    if (maxTime > (TimeSpan)oo[1]) oo[1] = maxTime;
                                    oo[2] = (int)oo[2] + 1;
                                    oo[3] = (int)oo[3] + group.Count();
                                    var gg = (int[])oo[4];
                                    for (var k = 0; k < groupByTimeCount.Length; k++)
                                        gg[k] += groupByTimeCount[k];
                                }
                            }

                        }
            }

            var logFileName = Settings.MinuteYahooLogFolder + "log.txt";
            if (File.Exists(logFileName))
                File.Delete(logFileName);

            if (errors.Count > 0)
            {
                File.AppendAllLines(logFileName, new[] { "Errors:" });
                File.AppendAllLines(logFileName, errors.Select(a=>a.Key + "\t" + a.Value));
            }

            File.AppendAllLines(logFileName, new[] { "File\tDate\tMinTime\tMaxTime\tSymbolCount\tQuoteCount\t09:30\t10:00\t10:30\t11:00\t11:30\t12:00\t12:30\t13:00\t13:30\t14:00\t14:30\t15:00\t15:30" });

            foreach (var o1 in log)
            {
                File.AppendAllLines(logFileName,
                    o1.Value.Select(a =>
                        o1.Key + "\t" + CsUtils.GetString(a.Key) + "\t" + a.Value[0] + "\t" + a.Value[1] + "\t" +
                        a.Value[2] + "\t" + a.Value[3] + "\t" +
                        string.Join("\t", ((int[])a.Value[4]).Select(a1 => a1.ToString()))));
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
