using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static class IntradayResults
    {
        public static void ByTradingViewType(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var symbols = DataSources.GetActiveSymbols();
            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            var data = new Dictionary<string, List<Quote>>();
            foreach (var q in oo.OrderBy(a => a.Timed))
            foreach (var kind in symbols[q.Symbol].Kinds)
            {
                if (!data.ContainsKey(kind))
                    data.Add(kind, new List<Quote>());
                data[kind].Add(q);
            }

            Debug.Print($"Kind\t{TradeStatistics.GetHeader()}");
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{kvp.Key}\t{ts.GetContent()}");
            }
        }

        public static void ByTimeNew(Action<string> showStatusAction, IEnumerable<Quote> quotes, IEnumerable<TimeSpan> timeFrames, bool closeInNextFrame)
        {
            var symbols = DataSources.GetActiveSymbols();
            // var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, quotes, GetTimeFrames(fullTime, is30MinuteInterval), !fullTime);
            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, quotes, timeFrames, closeInNextFrame);

            Debug.Print($"Time\t{TradeStatistics.GetHeader()}");
            var group = oo.GroupBy(o => o.TimeFrameId);
            foreach (var g in group.OrderBy(a => a.Key))
            {
                var qq = g.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)qq);
                Debug.Print($"{CsUtils.GetString(g.Key)}\t{ts.GetContent()}");
            }
        }

        public static void ByTradeValue(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var symbols = DataSources.GetActiveSymbols();
            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            var data = new Dictionary<int, List<Quote>>();
            foreach (var q in oo.OrderBy(a => a.Timed))
            {
                var symbol = symbols[q.Symbol];
                if (!data.ContainsKey(symbol.TradeValueId))
                    data.Add(symbol.TradeValueId, new List<Quote>());
                data[symbol.TradeValueId].Add(q);
            }

            Debug.Print($"TradeValueId\tMin\t{TradeStatistics.GetHeader()}");
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var min = symbols.Where(a => a.Value.TradeValueId == kvp.Key).Min(a => a.Value.TradeValue)/1000000.0;
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{kvp.Key}\t{min}\t{ts.GetContent()}");
            }
        }

        public static void ByKind(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var symbols = DataSources.GetActiveSymbols();
            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            var data = new Dictionary<string, List<Quote>>();
            foreach (var q in oo.OrderBy(a => a.Timed))
            foreach (var kind in symbols[q.Symbol].Kinds)
            {
                if (!data.ContainsKey(kind))
                    data.Add(kind, new List<Quote>());
                data[kind].Add(q);
            }

            Debug.Print($"Kind\t{TradeStatistics.GetHeader()}");
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{kvp.Key}\t{ts.GetContent()}");
            }
        }

        public static void ByTime(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var symbols = DataSources.GetActiveSymbols();
            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            Debug.Print($"Time\t{TradeStatistics.GetHeader()}");
            var group = oo.GroupBy(o => o.TimeFrameId);
            foreach (var g in group.OrderBy(a => a.Key))
            {
                var quotes = g.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{CsUtils.GetString(g.Key)}\t{ts.GetContent()}");
            }
        }

        public static void ByDate(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var symbols = DataSources.GetActiveSymbols();
            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            Debug.Print($"Date\t{TradeStatistics.GetHeader()}");
            var group = oo.GroupBy(o => o.Timed.Date);
            foreach (var g in group.OrderBy(a => a.Key))
            {
                var quotes = g.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{CsUtils.GetString(g.Key)}\t{ts.GetContent()}");
            }
        }

        public static void BySector(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var symbols = DataSources.GetActiveSymbols();
            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            var data = new Dictionary<string, List<Quote>>();
            foreach (var q in oo)
            {
                var symbol = symbols[q.Symbol];
                if (!data.ContainsKey(symbol.Sector))
                    data.Add(symbol.Sector, new List<Quote>());
                data[symbol.Sector].Add(q);
            }


            Debug.Print($"Sector\t{TradeStatistics.GetHeader()}");
            var group = data.GroupBy(o => o.Key);
            foreach (var g in group.OrderBy(a => a.Key))
            {
                var quotes = data[g.Key].OrderBy(a => a.Timed).ThenBy(a => a.Symbol).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{CsUtils.GetString(g.Key)}\t{ts.GetContent()}");
            }
        }

        public static void ByIndustry(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var symbols = DataSources.GetActiveSymbols();
            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            var data = new Dictionary<string, List<Quote>>();
            foreach (var q in oo)
            {
                var symbol = symbols[q.Symbol];
                if (!data.ContainsKey(symbol.Industry))
                    data.Add(symbol.Industry, new List<Quote>());
                data[symbol.Industry].Add(q);
            }


            Debug.Print($"Industry\t{TradeStatistics.GetHeader()}");
            var group = data.GroupBy(o => o.Key);
            foreach (var g in group.OrderBy(a => a.Key))
            {
                var quotes = data[g.Key].OrderBy(a => a.Timed).ThenBy(a => a.Symbol).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{CsUtils.GetString(g.Key)}\t{ts.GetContent()}");
            }
        }

        public static void BySectorAndIndustry(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<Tuple<string, string>, List<Quote>>();

            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            foreach (var q in oo)
            {
                var symbol = symbols[q.Symbol];
                    var key = new Tuple<string, string>(symbol.Sector, symbol.Industry);
                    if (!data.ContainsKey(key))
                        data.Add(key, new List<Quote>());
                    data[key].Add(q);
                }

            Debug.Print($"Sector\tIndustry\t{TradeStatistics.GetHeader()}");

            var groups1 = data.GroupBy(a => a.Key.Item1).ToList();
            foreach (var g1 in groups1.OrderBy(a => a.Key))
            {
                var groups2 = g1.GroupBy(a => a.Key.Item2).ToList();
                foreach (var g2 in groups2.OrderBy(a => a.Key))
                {
                    var key = new Tuple<string, string>(g1.Key, g2.Key);
                    var quotes = data[key].OrderBy(a => a.Timed).ThenBy(a => a.Symbol).ToList();
                    var ts = new TradeStatistics(quotes);
                    Debug.Print($"{CsUtils.GetString(g1.Key)}\t{CsUtils.GetString(g2.Key)}\t{ts.GetContent()}");
                }
            }
        }

        public static void BySymbol(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var symbols = DataSources.GetActiveSymbols();
            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            Debug.Print($"Symbol\t{TradeStatistics.GetHeader()}");
            var group = oo.GroupBy(o => o.Symbol);
            foreach (var g in group.OrderBy(a => a.Key))
            {
                var quotes = g.OrderBy(a => a.Timed).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{CsUtils.GetString(g.Key)}\t{ts.GetContent()}");
            }
        }

        public static void ByKindAndDate(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var printDetails = false;
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<Tuple<string, DateTime>, List<Quote>>();

            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            foreach (var q in oo.OrderBy(a => a.Timed))
                foreach (var kind in symbols[q.Symbol].Kinds)
                {
                    var key = new Tuple<string, DateTime>(kind, q.Timed.Date);
                    if (!data.ContainsKey(key))
                        data.Add(key, new List<Quote>());
                    data[key].Add(q);
                }

            Debug.Print($"Kind\tDate\t{TradeStatistics.GetHeader()}");

            var groups1 = data.GroupBy(a => a.Key.Item1).ToList();
            foreach (var g1 in groups1.OrderBy(a => a.Key))
            {
                var groups2 = g1.GroupBy(a => a.Key.Item2).ToList();
                foreach (var g2 in groups2.OrderBy(a => a.Key))
                {
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

        public static void ByKindAndTime(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var printDetails = false;
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<Tuple<string, TimeSpan>, List<Quote>>();

            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            foreach (var q in oo.OrderBy(a => a.Timed))
                foreach (var kind in symbols[q.Symbol].Kinds)
                {
                    var key = new Tuple<string, TimeSpan>(kind, q.TimeFrameId);
                    if (!data.ContainsKey(key))
                        data.Add(key, new List<Quote>());
                    data[key].Add(q);
                }

            Debug.Print($"Kind\tTime\t{TradeStatistics.GetHeader()}");

            var groups1 = data.GroupBy(a => a.Key.Item1).ToList();
            foreach (var g1 in groups1.OrderBy(a => a.Key))
            {
                var groups2 = g1.GroupBy(a => a.Key.Item2).ToList();
                foreach (var g2 in groups2.OrderBy(a => a.Key))
                {
                    var key = new Tuple<string, TimeSpan>(g1.Key, g2.Key);
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

        public static void ByExchangeAndAsset(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<Tuple<string, string>, List<Quote>>();

            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            foreach (var q in oo)
            {
                var symbol = symbols[q.Symbol];
                var key = new Tuple<string, string>(symbol.Exchange, symbol.Asset);
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

            Debug.Print($"Exchange\tAsset\t{TradeStatistics.GetHeader()}");

            var groups1 = data.GroupBy(a => a.Key.Item1).ToList();
            foreach (var g1 in groups1.OrderBy(a => a.Key))
            {
                var groups2 = g1.GroupBy(a => a.Key.Item2).ToList();
                foreach (var g2 in groups2.OrderBy(a => a.Key))
                {
                    var key = new Tuple<string, string>(g1.Key, g2.Key);
                    var quotes = data[key].OrderBy(a => a.Timed).ThenBy(a => a.Symbol).ToList();
                    var ts = new TradeStatistics(quotes);
                    Debug.Print($"{CsUtils.GetString(g1.Key)}\t{CsUtils.GetString(g2.Key)}\t{ts.GetContent()}");
                }
            }
        }


        private static List<TimeSpan> GetTimeFrames(bool fullDay)
        {
            return fullDay
                ? CsUtils.GetTimeFrames(new TimeSpan(9, 30, 0), new TimeSpan(16, 00, 0), new TimeSpan(0, 30, 0))
                : CsUtils.GetTimeFrames(new TimeSpan(10, 00, 0), new TimeSpan(15, 30, 0), new TimeSpan(0, 30, 0));
        }
        public static List<TimeSpan> GetTimeFrames(bool fullDay, bool is30MinutesInterval)
        {
            if (is30MinutesInterval)
            {
                return fullDay
                    ? CsUtils.GetTimeFrames(new TimeSpan(9, 30, 0), new TimeSpan(16, 00, 0), new TimeSpan(0, 30, 0))
                    : CsUtils.GetTimeFrames(new TimeSpan(10, 00, 0), new TimeSpan(15, 30, 0), new TimeSpan(0, 30, 0));
            }

            // 90/105 minutes
            if (fullDay)
                return new List<TimeSpan>()
                {
                    new TimeSpan(9, 30, 0), new TimeSpan(11, 15, 0), new TimeSpan(12, 45, 0), new TimeSpan(14, 15, 0),
                    new TimeSpan(16, 00, 0)
                };

            return new List<TimeSpan>()
            {
                new TimeSpan(9, 45, 0), new TimeSpan(11, 15, 0), new TimeSpan(12, 45, 0), new TimeSpan(14, 15, 0),
                new TimeSpan(15, 45, 0)
            };
        }
    }
}
