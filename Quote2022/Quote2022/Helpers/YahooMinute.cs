using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Quote2022.Actions;
using Quote2022.Models;

namespace Quote2022.Helpers
{
    public class YahooMinute
    {
        public static void Test()
        {
            var printDetails = false;
            var symbols = SaveToDb.GetSymbolsAndKinds(750);
            var data = new Dictionary<Tuple<string, DateTime>, List<Quote>>();
            var files = Directory.GetFiles(@"E:\Quote\WebData\Minute\Yahoo\YahooMinute_20221224", "yMin-*.txt", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var symbol = Path.GetFileNameWithoutExtension(file).Substring(5);
                if (!symbols.ContainsKey(symbol))
                    continue;

                var validQuotes = new List<Quote>();

                var content = File.ReadAllText(file);
                var o = JsonConvert.DeserializeObject<Models.MinuteYahoo>(content);

                var minuteQuotes = o.GetQuotes();
                var timeK = 0;
                Quote currentQuote = null;

                foreach (var q in minuteQuotes.OrderBy(a => a.Timed))
                {
                    if (currentQuote != null && currentQuote.Timed.Date != q.Timed.Date)
                    {
                        timeK = 0;
                        currentQuote = null;
                    }

                    var newTimeK = Convert.ToInt32(q.Timed.TimeOfDay.TotalSeconds / 60.0) / 30;
                    if (newTimeK != timeK)
                    {
                        if (currentQuote != null)
                        {
                            currentQuote.Close = q.Open;
                            if (currentQuote.High < currentQuote.Close)
                                currentQuote.High = currentQuote.Close;
                            if (currentQuote.Low > currentQuote.Close)
                                currentQuote.Low = currentQuote.Close;

                            if (currentQuote.Timed.TimeOfDay >= new TimeSpan(10, 0, 0) &&
                                currentQuote.Timed.TimeOfDay < new TimeSpan(15, 30, 0))
                            {
                                if (currentQuote.TimeString == "15:30")
                                {

                                }
                                validQuotes.Add(currentQuote);
                            }
                        }

                        currentQuote = new Quote
                        {
                            Symbol = symbol,
                            Timed = q.Timed,
                            Open = q.Open,
                            High = q.High,
                            Low = q.Low,
                            Close = float.NaN,
                            Volume = q.Volume
                        };
                        timeK = newTimeK;
                    }
                    else
                    {
                        if (q.High > currentQuote.High) currentQuote.High = q.High;
                        if (q.Low < currentQuote.Low) currentQuote.Low = q.Low;
                        currentQuote.Volume += q.Volume;
                    }
                }

                foreach (var kind in symbols[symbol])
                foreach (var q in validQuotes)
                {
                    var key = new Tuple<string, DateTime>(kind, q.Timed.Date);
                    if (!data.ContainsKey(key))
                        data.Add(key, new List<Quote>());
                    data[key].Add(q);
                }
            }

            if (!printDetails)
                Debug.Print($"Kind\tDate\t{TradeStatistics.GetHeader()}");

            var groups1 = data.GroupBy(a => a.Key.Item1).ToList();
            foreach (var g1 in groups1)
            {
                var groups2 = g1.GroupBy(a => a.Key.Item2).ToList();
                foreach (var g2 in groups2)
                {
                    var aa1 = g2.OrderBy(a => a.Key.Item1).ThenBy(a => a.Key.Item2).ToList();
                    var key = new Tuple<string, DateTime>(g1.Key, g2.Key);
                    var quotes = data[key].OrderBy(a => a.Timed).ThenBy(a => a.Symbol).ToList();

                    if (printDetails)
                    {
                        Debug.Print($"DETAILS for {CsUtils.GetString(g1.Key)}\t{CsUtils.GetString(g2.Key)}");
                        foreach (var q in quotes)
                            Debug.Print(q.ToString());
                    }
                    else
                    {
                        var ts = new TradeStatistics(quotes);
                        Debug.Print($"{CsUtils.GetString(g1.Key)}\t{CsUtils.GetString(g2.Key)}\t{ts.GetContent()}");
                    }
                }
            }
        }

        /* public static void Test()
        {
            var symbols = SaveToDb.GetSymbolsAndKinds(1000);
            var data = new List<QuoteWithGroup>();
            var data2 = new Dictionary<Tuple<string,DateTime>, List<Quote>>();
            var files = Directory.GetFiles(@"E:\Quote\WebData\Minute\Yahoo\YahooMinute_20221224", "yMin-*.txt", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var symbol = Path.GetFileNameWithoutExtension(file).Substring(5);
                if (!symbols.ContainsKey(symbol))
                    continue;

                var validQuotes = new List<Quote>();

                var content = File.ReadAllText(file);
                var o = JsonConvert.DeserializeObject<Models.MinuteYahoo>(content);

                var minuteQuotes = o.GetQuotes();
                var timeK = 0;
                Quote currentQuote = null;

                foreach (var q in minuteQuotes.OrderBy(a => a.Timed))
                {
                    if (currentQuote != null && currentQuote.Timed.Date != q.Timed.Date)
                    {
                        timeK = 0;
                        currentQuote = null;
                    }

                    var newTimeK = Convert.ToInt32(q.Timed.TimeOfDay.TotalSeconds / 60.0) / 30;
                    if (newTimeK != timeK)
                    {
                        if (currentQuote != null)
                        {
                            currentQuote.Close = q.Open;
                            if (currentQuote.High < currentQuote.Close)
                                currentQuote.High = currentQuote.Close;
                            if (currentQuote.Low > currentQuote.Close)
                                currentQuote.Low = currentQuote.Close;

                            if (currentQuote.Timed.TimeOfDay >= new TimeSpan(10, 0, 0) &&
                                currentQuote.Timed.TimeOfDay < new TimeSpan(15, 30, 0))
                            {
                                if (currentQuote.TimeString == "15:30")
                                {

                                }
                                validQuotes.Add(currentQuote);
                            }
                        }

                        currentQuote = new Quote
                        {
                            Symbol = symbol,
                            Timed = q.Timed,
                            Open = q.Open,
                            High = q.High,
                            Low = q.Low,
                            Close = float.NaN,
                            Volume = q.Volume
                        };
                        timeK = newTimeK;
                    }
                    else
                    {
                        if (q.High > currentQuote.High) currentQuote.High = q.High;
                        if (q.Low < currentQuote.Low) currentQuote.Low = q.Low;
                        currentQuote.Volume += q.Volume;
                    }
                }

                foreach (var kind in symbols[symbol])
                foreach (var q in validQuotes)
                {
                    data.Add(new QuoteWithGroup(q, kind, q.Timed.Date));
                    var key = new Tuple<string, DateTime>(kind, q.Timed.Date);
                    if (!data2.ContainsKey(key))
                        data2.Add(key, new List<Quote>());
                    data2[key].AddRange(validQuotes);
                }
            }

            /*Debug.Print($"Kind\tDate\t{TradeStatistics.GetHeader()}");
            var groups1 = data2.GroupBy(a => a.Key.Item1).ToList();
            foreach (var g1 in groups1)
            {
                var groups2 = g1.GroupBy(a => a.Key.Item2).ToList();
                foreach (var g2 in groups2)
                {
                    var aa1 = g2.OrderBy(a => a.Key.Item1).ThenBy(a => a.Key.Item2).ToList();
                    var key = new Tuple<string, DateTime>(g1.Key, g2.Key);
                    var ts = new TradeStatistics(data2[key]);
                    Debug.Print($"{g1}\t{g2}\t{ts.GetContent()}");
                }
            }

            //var groups1 = data.GroupBy(a => a.G1).ToList();
            Debug.Print($"Date\t{TradeStatistics.GetHeader()}");
            foreach (var g1 in groups1)
            {
                /*var groups2 = g1.GroupBy(a => a.G2).ToList();
                foreach (var g2 in groups2)
                {
                    var aa1 = g2.OrderBy(a => a.G1).ThenBy(a => a.G2).ToList();
                    var ts = new TradeStatistics((IList<Quote>)aa1);
                    Debug.Print($"{g1}\t{g2}\t{ts.GetContent()}");
                    /*var ts = new TradeStatistics(g2.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).ToList());
                    Debug.Print($"{g.Key:yyyy-MM-dd}\t{ts.GetContent()}");
                }
            }
        }*/

                public static void TestX()
        {
            var validQuotes = new List<Quote>();

            // var symbols = SaveToDb.GetLargestStocks(500);
            // var symbols = SaveToDb.GetLargestNotSP500Stocks(500);
            var symbols = SaveToDb.GetLargestNotETF(500);
            // var symbols = SaveToDb.GetSP500Stocks();
            //            var files = Directory.GetFiles(@"E:\Quote\WebData\Minute\Yahoo\YahooMinute_20221224", "yMin-AA.txt", SearchOption.AllDirectories);
            var files = Directory.GetFiles(@"E:\Quote\WebData\Minute\Yahoo\YahooMinute_20221224", "yMin-*.txt", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var symbol = Path.GetFileNameWithoutExtension(file).Substring(5);
                if (!symbols.Contains(symbol))
                    continue;

                var content = File.ReadAllText(file);
                var o = JsonConvert.DeserializeObject<Models.MinuteYahoo>(content);

                // Debug.Print($"{o.Chart.Result[0].Meta.Symbol}\t{o.Chart.Result[0].Meta.ExchangeName}\t{o.Chart.Result[0].Meta.InstrumentType}");
                var quotes = o.GetQuotes();
                var timeK = 0;
                Quote currentQuote = null;
                //                foreach (var q in quotes.Where(a => a.Timed.DayOfWeek == DayOfWeek.Wednesday).OrderBy(a => a.Timed))
                foreach (var q in quotes.OrderBy(a => a.Timed))
                {
                    if (currentQuote != null && currentQuote.Timed.Date != q.Timed.Date)
                    {
                        timeK = 0;
                        currentQuote = null;
                    }

                    var newTimeK = Convert.ToInt32(q.Timed.TimeOfDay.TotalSeconds / 60.0) / 30;
                    if (newTimeK != timeK)
                    {
                        if (currentQuote != null)
                        {
                            currentQuote.Close = q.Open;
                            if (currentQuote.High < currentQuote.Close)
                                currentQuote.High = currentQuote.Close;
                            if (currentQuote.Low > currentQuote.Close)
                                currentQuote.Low = currentQuote.Close;

                            if (currentQuote.Timed.TimeOfDay >= new TimeSpan(10, 0, 0) &&
                                currentQuote.Timed.TimeOfDay < new TimeSpan(15, 30, 0))
                            {
                                if (currentQuote.TimeString == "15:30")
                                {

                                }
                                validQuotes.Add(currentQuote);
                            }
                        }

                        currentQuote = new Quote
                        {
                            Symbol = symbol,
                            Timed = q.Timed,
                            Open = q.Open,
                            High = q.High,
                            Low = q.Low,
                            Close = float.NaN,
                            Volume = q.Volume
                        };
                        timeK = newTimeK;
                    }
                    else
                    {
                        if (q.High > currentQuote.High) currentQuote.High = q.High;
                        if (q.Low < currentQuote.Low) currentQuote.Low = q.Low;
                        currentQuote.Volume += q.Volume;
                    }
                }
            }

            /*foreach (var q in validQuotes.OrderBy(a => a.Timed))
                Debug.Print(q.ToString());*/

            var groups = validQuotes.GroupBy(a => a.Timed.Date).ToList();
            Debug.Print($"Date\t{TradeStatistics.GetHeader()}");
            foreach (var g in groups)
            {
                var ts = new TradeStatistics(g.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).ToList());
                Debug.Print($"{g.Key:yyyy-MM-dd}\t{ts.GetContent()}");
            }

        }

        public static void Test2()
        {
            var validQuotes = new List<Quote>();

            var symbols = SaveToDb.GetLargestStocks(500);
            //            var files = Directory.GetFiles(@"E:\Quote\WebData\Minute\Yahoo\YahooMinute_20221224", "yMin-AA.txt", SearchOption.AllDirectories);
            var files = Directory.GetFiles(@"E:\Quote\WebData\Minute\Yahoo\YahooMinute_20221224", "yMin-*.txt", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var symbol = Path.GetFileNameWithoutExtension(file).Substring(5);
                if (!symbols.Contains(symbol))
                    continue;

                var content = File.ReadAllText(file);
                var o = JsonConvert.DeserializeObject<Models.MinuteYahoo>(content);

                // Debug.Print($"{o.Chart.Result[0].Meta.Symbol}\t{o.Chart.Result[0].Meta.ExchangeName}\t{o.Chart.Result[0].Meta.InstrumentType}");
                var quotes = o.GetQuotes();
                var timeK = 0;
                Quote currentQuote = null;
                foreach (var q in quotes.Where(a => a.Timed.DayOfWeek == DayOfWeek.Wednesday).OrderBy(a => a.Timed))
                {
                    var newTimeK = Convert.ToInt32(q.Timed.TimeOfDay.TotalSeconds / 60.0) / 30;
                    if (newTimeK != timeK)
                    {
                        if (currentQuote != null)
                        {
                            currentQuote.Close = q.Open;
                            if (currentQuote.High < currentQuote.Close)
                                currentQuote.High = currentQuote.Close;
                            if (currentQuote.Low > currentQuote.Close)
                                currentQuote.Low = currentQuote.Close;

                            if (currentQuote.Timed.TimeOfDay >= new TimeSpan(10, 0, 0) && currentQuote.Timed.TimeOfDay <= new TimeSpan(15, 30, 0))
                                validQuotes.Add(currentQuote);
                        }

                        currentQuote = new Quote
                        {
                            Symbol = symbol,
                            Timed = q.Timed,
                            Open = q.Open,
                            High = q.High,
                            Low = q.Low,
                            Close = float.NaN,
                            Volume = q.Volume
                        };
                        timeK = newTimeK;
                    }
                    else
                    {
                        if (q.High > currentQuote.High) currentQuote.High = q.High;
                        if (q.Low < currentQuote.Low) currentQuote.Low = q.Low;
                        currentQuote.Volume += q.Volume;
                    }
                }
            }

            foreach (var q in validQuotes.OrderBy(a => a.Timed))
                Debug.Print(q.ToString());
        }
    }
}
