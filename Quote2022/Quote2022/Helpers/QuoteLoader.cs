using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Quote2022.Models;

namespace Quote2022.Helpers
{
    public static class QuoteLoader
    {
        private static DateTime[] BadYahooIntradayDates = new[]
        {
            new DateTime(2022, 10, 28), new DateTime(2022, 11, 1), new DateTime(2022, 11, 2), new DateTime(2022, 11, 25)
        };

        public static List<IntradayQuote> lastData;

        public static IEnumerable<Quote> GetYahooIntradayQuotesFromZipCache(Action<string> showStatusAction,
            string zipFile)
        {
            string lastSymbol = null;
            var lastDate = DateTime.MinValue;
            var lastTime = TimeSpan.Zero;
            var lastPrice = float.MinValue;
            var lastVolume = long.MinValue;

            var cnt = 0;

            using (var zip = new ZipReaderStream(zipFile))
                foreach (var item in zip)
                    if (item.Length > 0)
                    {
                        var a = item.Reader.ReadLine();
                        if (!string.Equals(a, "Symbol\tDate\tTime\tOpen\tHigh\tLow\tClose\tVolume"))
                            throw new Exception($"GetYahooIntradayQuotesFromTextCache. Bad header of cache file {zipFile}");
                        while ((a = item.Reader.ReadLine()) != null)
                        {
                            var aa = a.Split('\t');
                            var symbol = string.IsNullOrEmpty(aa[0]) ? lastSymbol : aa[0];
                            var date = string.IsNullOrEmpty(aa[1]) ? lastDate : DateTime.ParseExact(aa[1], "yyyyMMdd", CultureInfo.InvariantCulture);
                            var time = string.IsNullOrEmpty(aa[2]) ? lastTime : TimeSpan.ParseExact(aa[2], "hh\\:mm", CultureInfo.InvariantCulture);
                            var open = string.IsNullOrEmpty(aa[3]) ? lastPrice : float.Parse(aa[3], CultureInfo.InvariantCulture);
                            lastPrice = open;
                            var high = string.IsNullOrEmpty(aa[4]) ? lastPrice : float.Parse(aa[4], CultureInfo.InvariantCulture);
                            lastPrice = high;
                            var low = string.IsNullOrEmpty(aa[5]) ? lastPrice : float.Parse(aa[5], CultureInfo.InvariantCulture);
                            lastPrice = low;
                            var close = string.IsNullOrEmpty(aa[6]) ? lastPrice : float.Parse(aa[6], CultureInfo.InvariantCulture);
                            var volume = string.IsNullOrEmpty(aa[7]) ? lastVolume : long.Parse(aa[7], CultureInfo.InvariantCulture);

                            lastSymbol = symbol;
                            lastDate = date;
                            lastTime = time + new TimeSpan(0, 1, 0);
                            lastPrice = close;
                            lastVolume = volume;

                            if (BadYahooIntradayDates.Contains(date))
                                continue;

                            cnt++;
                            if ((cnt % 100000) == 0)
                                showStatusAction($"GetYahooIntradayQuotesFromTextCache read {cnt:N0} quotes");

                            var quote = new Quote
                            {
                                Symbol = symbol,
                                Timed = date + time,
                                Open = open,
                                High = high,
                                Low = low,
                                Close = close,
                                Volume = volume
                            };
                            yield return quote;
                        }
                    }
        }

        public static IEnumerable<Quote> GetYahooIntradayQuotesFromTextCache(Action<string> showStatusAction, string textFile)
        {
            string lastSymbol = null;
            var lastDate = DateTime.MinValue;
            var lastTime = TimeSpan.Zero;
            var lastPrice = float.MinValue;
            var lastVolume = long.MinValue;

            var cnt = 0;
            using (var reader = new StreamReader(textFile))
            {
                var a = reader.ReadLine();
                if (!string.Equals(a, "Symbol\tDate\tTime\tOpen\tHigh\tLow\tClose\tVolume"))
                    throw new Exception($"GetYahooIntradayQuotesFromTextCache. Bad header of cache file {textFile}");
                while ((a = reader.ReadLine()) != null)
                {
                    var aa = a.Split('\t');
                    var symbol = string.IsNullOrEmpty(aa[0]) ? lastSymbol : aa[0];
                    var date = string.IsNullOrEmpty(aa[1]) ? lastDate : DateTime.ParseExact(aa[1], "yyyyMMdd", CultureInfo.InvariantCulture);
                    var time = string.IsNullOrEmpty(aa[2]) ? lastTime : TimeSpan.ParseExact(aa[2], "hh\\:mm", CultureInfo.InvariantCulture);
                    var open = string.IsNullOrEmpty(aa[3]) ? lastPrice : float.Parse(aa[3], CultureInfo.InvariantCulture);
                    lastPrice = open;
                    var high = string.IsNullOrEmpty(aa[4]) ? lastPrice : float.Parse(aa[4], CultureInfo.InvariantCulture);
                    lastPrice = high;
                    var low = string.IsNullOrEmpty(aa[5]) ? lastPrice : float.Parse(aa[5], CultureInfo.InvariantCulture);
                    lastPrice = low;
                    var close = string.IsNullOrEmpty(aa[6]) ? lastPrice : float.Parse(aa[6], CultureInfo.InvariantCulture);
                    var volume = string.IsNullOrEmpty(aa[7]) ? lastVolume : long.Parse(aa[7], CultureInfo.InvariantCulture);

                    lastSymbol = symbol;
                    lastDate = date;
                    lastTime = time + new TimeSpan(0, 1, 0);
                    lastPrice = close;
                    lastVolume = volume;

                    if (BadYahooIntradayDates.Contains(date))
                        continue;

                    cnt++;
                    if ((cnt % 100000) == 0)
                        showStatusAction($"GetYahooIntradayQuotesFromTextCache read {cnt:N0} quotes");

                    var quote = new Quote
                    {
                        Symbol = symbol,
                        Timed = date + time,
                        Open = open,
                        High = high,
                        Low = low,
                        Close = close,
                        Volume = volume
                    };
                    yield return quote;
                }
            }
        }

        public static void PrepareYahooIntradayTextCache(Action<string> showStatusAction, IEnumerable<string> zipFiles, string textFile, Func<string, bool> skipIf)
        {
            var ts1Minute = new TimeSpan(0,1,0);

            var data = new List<IntradayQuote>();
            var cnt = 0;
            string lastSymbol = null;
            var lastDate = DateTime.MinValue;
            var lastTime = TimeSpan.Zero;
            var lastPrice = float.MinValue;
            var lastVolume = long.MinValue;

            using (var outputFile = new StreamWriter(textFile))
            {
                outputFile.WriteLine("Symbol\tDate\tTime\tOpen\tHigh\tLow\tClose\tVolume");
                foreach (var zipFile in zipFiles)
                {
                    showStatusAction($"PrepareYahooIntradayTextCache is working for {Path.GetFileName(zipFile)}");
                    using (var zip = new ZipReader(zipFile))
                        foreach (var item in zip)
                            if (item.Length > 0 && item.FileNameWithoutExtension.ToUpper().StartsWith("YMIN-"))
                            {
                                var symbol = item.FileNameWithoutExtension.Substring(5);
                                if (skipIf != null && skipIf(symbol))
                                    continue;

                                cnt++;
                                if ((cnt % 100) == 0)
                                    showStatusAction($"PrepareYahooIntradayTextCache is working for {Path.GetFileName(zipFile)}. Total file processed: {cnt:N0}");

                                var o = JsonConvert.DeserializeObject<Models.MinuteYahoo>(item.Content);
                                var minuteQuotes = o.GetQuotes().OrderBy(a=>a.Timed);
                                foreach (var q in minuteQuotes)
                                {
                                    if (BadYahooIntradayDates.Contains(q.Timed.Date))
                                        continue;

                                    var sb = new StringBuilder();
                                    sb.Append((string.Equals(lastSymbol, symbol) ? "" : symbol) + "\t");
                                    sb.Append((Equals(lastDate, q.Timed.Date) ? "" : q.Timed.Date.ToString("yyyyMMdd")) + "\t");
                                    sb.Append((Equals(lastTime.Add(ts1Minute), q.Timed.TimeOfDay) ? "" : q.Timed.TimeOfDay.ToString("hh\\:mm")) + "\t");
                                    sb.Append((Equals(lastPrice, q.Open) ? "" : q.Open.ToString(CultureInfo.InvariantCulture)) + "\t");
                                    lastPrice = q.Open;
                                    sb.Append((Equals(lastPrice, q.High) ? "" : q.High.ToString(CultureInfo.InvariantCulture)) + "\t");
                                    lastPrice = q.High;
                                    sb.Append((Equals(lastPrice, q.Low) ? "" : q.Low.ToString(CultureInfo.InvariantCulture)) + "\t");
                                    lastPrice = q.Low;
                                    sb.Append((Equals(lastPrice, q.Close) ? "" : q.Close.ToString(CultureInfo.InvariantCulture)) + "\t");
                                    sb.Append(Equals(lastVolume, q.Volume) ? "" : q.Volume.ToString(CultureInfo.InvariantCulture));
                                    outputFile.WriteLine(sb.ToString());
                                    var aa = sb.ToString().Split('\t');

                                    lastSymbol = symbol;
                                    lastDate = q.Timed.Date;
                                    lastTime = q.Timed.TimeOfDay;
                                    lastPrice = q.Close;
                                    lastVolume = q.Volume;
                                }
                            }
                }
            }

            showStatusAction($"PrepareYahooIntradayTextCache FINISHED!");
        }

        public static List<IntradayQuote> GetYahooIntradayQuotes(Action<string> showStatusAction, IEnumerable<string> zipFiles, IEnumerable<TimeSpan> timeFrames, Func<string, bool> skipIf, bool closeIsInNextFrame, bool useLastData)
        {
            if (useLastData)
                return lastData ?? new List<IntradayQuote>();

            var frames = new List<Tuple<TimeSpan, TimeSpan>>();
            TimeSpan? lastTime = null;
            foreach (var ts in timeFrames.OrderBy(a => a))
            {
                if (lastTime.HasValue) frames.Add(new Tuple<TimeSpan, TimeSpan>(lastTime.Value, ts));
                lastTime = ts;
            }

            var data = new List<IntradayQuote>();
            var cnt = 0;
            foreach (var zipFile in zipFiles)
            {
                showStatusAction($"GetYahooIntradayQuotes is working for {Path.GetFileName(zipFile)}");
                using (var zip = new ZipReader(zipFile))
                    foreach (var item in zip)
                        if (item.Length > 0 && item.FileNameWithoutExtension.ToUpper().StartsWith("YMIN-"))
                        {
                            var symbol = item.FileNameWithoutExtension.Substring(5);
                            if (skipIf != null && skipIf(symbol))
                                continue;

                            cnt++;
                            if ((cnt % 100) == 0)
                                showStatusAction($"GetYahooIntradayQuotes is working for {Path.GetFileName(zipFile)}. Total file processed: {cnt:N0}");

                            var o = JsonConvert.DeserializeObject<Models.MinuteYahoo>(item.Content);
                            var minuteQuotes = o.GetQuotes();

                            Tuple<TimeSpan, TimeSpan> lastTimeSpan = null;
                            IntradayQuote currentQuote = null;
                            foreach (var q in minuteQuotes.OrderBy(a => a.Timed))
                            {
                                //                                if (q.Timed.Date == new DateTime(2022, 11, 25)) // Not full day
                                if (BadYahooIntradayDates.Contains(q.Timed.Date))
                                    continue;

                                if (currentQuote != null && currentQuote.Timed.Date != q.Timed.Date)
                                {
                                    if (closeIsInNextFrame)
                                        Debug.Print($"GetYahooIntradayQuotes. Not full quote: {currentQuote}");
                                    else
                                        data.Add(currentQuote);

                                    lastTimeSpan = null;
                                    currentQuote = null;
                                }

                                var thisFrame = frames.FirstOrDefault(ts => q.Timed.TimeOfDay >= ts.Item1 && q.Timed.TimeOfDay < ts.Item2);
                                if (Equals(thisFrame, lastTimeSpan))
                                {
                                    if (currentQuote != null)
                                    {
                                        if (q.High > currentQuote.High) currentQuote.High = q.High;
                                        if (q.Low < currentQuote.Low) currentQuote.Low = q.Low;
                                        currentQuote.Volume += q.Volume;
                                        if (!closeIsInNextFrame)
                                        {
                                            currentQuote.Close = q.Close;
                                            currentQuote.CloseAt = q.Timed.TimeOfDay;
                                        }
                                    }
                                }
                                else
                                {
                                    if (currentQuote != null)
                                    {
                                        if (closeIsInNextFrame)
                                        {
                                            currentQuote.Close = q.Open;
                                            currentQuote.CloseAt = q.Timed.TimeOfDay;
                                            if (currentQuote.High < currentQuote.Close)
                                                currentQuote.High = currentQuote.Close;
                                            if (currentQuote.Low > currentQuote.Close)
                                                currentQuote.Low = currentQuote.Close;
                                        }

                                        data.Add(currentQuote);
                                    }

                                    currentQuote = thisFrame == null
                                        ? null
                                        : new IntradayQuote
                                        {
                                            Symbol = symbol,
                                            Timed = q.Timed,
                                            Open = q.Open,
                                            High = q.High,
                                            Low = q.Low,
                                            Close = closeIsInNextFrame ? float.NaN : q.Close,
                                            Volume = q.Volume,
                                            CloseAt = closeIsInNextFrame ? TimeSpan.Zero : q.Timed.TimeOfDay,
                                            TimeFrameId = thisFrame.Item1
                                        };
                                    lastTimeSpan = thisFrame;
                                }
                            }

                            if (currentQuote != null)
                            {
                                if (closeIsInNextFrame)
                                    Debug.Print($"GetYahooIntradayQuotes. Not full quote: {currentQuote}");
                                else
                                    data.Add(currentQuote);
                            }
                        }
            }

            showStatusAction($"GetYahooIntradayQuotes FINISHED!");

            lastData = data;
            return data;
        }
    }
}
