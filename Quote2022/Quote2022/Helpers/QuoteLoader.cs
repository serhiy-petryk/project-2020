using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static List<IntradayQuote> GetYahooIntradayQuotes(Action<string> showStatusAction, IEnumerable<string> zipFiles,IEnumerable<TimeSpan> timeFrames, Func<string, bool> skipIf, bool closeIsInNextFrame, bool useLastData)
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

                                var thisTimeSpan = frames.FirstOrDefault(ts =>
                                    q.Timed.TimeOfDay >= ts.Item1 && q.Timed.TimeOfDay < ts.Item2);
                                if (Equals(thisTimeSpan, lastTimeSpan))
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

                                    currentQuote = thisTimeSpan == null
                                        ? null
                                        : new IntradayQuote
                                        {
                                            Symbol = symbol, Timed = q.Timed, Open = q.Open, High = q.High, Low = q.Low,
                                            Close = closeIsInNextFrame ? float.NaN : q.Close, Volume = q.Volume,
                                            CloseAt = closeIsInNextFrame ? TimeSpan.Zero : q.Timed.TimeOfDay
                                        };
                                    lastTimeSpan = thisTimeSpan;
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
