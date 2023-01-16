﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static class IntradayResults
    {
        public static Dictionary<string, Action<List<IntradayQuote>>> ActionList =
            new Dictionary<string, Action<List<IntradayQuote>>>
            {
                {"By Time", ByTime}, {"By Date", ByDate}, {"By Day of Week", ByDayOfWeek}, {"By Week", ByWeek},
                {"By Kind", ByKind}, {"By Sector", BySector}, {"By Industry", ByIndustry}, {"By Symbol", BySymbol},
                {"By Trade Value", ByTradeValue}, {"By TradingView Type", ByTradingViewType},
                {"By TradingView Subtype", ByTradingViewSubtype},
                {"By TradingViewSector", ByTradingViewSector}, {"By TradingView Recommend", ByTradingViewRecommend}
            };

        public static void ByTradingViewRecommend(List<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<object, List<Quote>>();
            foreach (var q in iQuotes)
            {
                var key = symbols[q.Symbol].TvRecommendId;
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

            Debug.Print($"RecommendId\tMin of Recommend\t{TradeStatistics.GetHeader()}");
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var min = symbols.Where(a => Equals(a.Value.TvRecommendId, kvp.Key)).Min(a => a.Value.TvRecommend);
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{kvp.Key}\t{min}\t{ts.GetContent()}");
            }
        }

        public static void ByTime(List<IntradayQuote> iQuotes)
        {
            Debug.Print($"Time\t{TradeStatistics.GetHeader()}");
            var group = iQuotes.GroupBy(o => o.TimeFrameId);
            foreach (var g in group.OrderBy(a => a.Key))
            {
                var quotes = g.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{CsUtils.GetString(g.Key)}\t{ts.GetContent()}");
            }
        }

        public static void ByDate(List<IntradayQuote> iQuotes)
        {
            Debug.Print($"Date\t{TradeStatistics.GetHeader()}");
            var group = iQuotes.GroupBy(o => o.Timed.Date);
            foreach (var g in group.OrderBy(a => a.Key))
            {
                var quotes = g.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{CsUtils.GetString(g.Key)}\t{ts.GetContent()}");
            }
        }

        public static void ByDayOfWeek(List<IntradayQuote> iQuotes)
        {
            Debug.Print($"DayOfWeek\t{TradeStatistics.GetHeader()}");
            var group = iQuotes.GroupBy(o => o.Timed.DayOfWeek);
            foreach (var g in group.OrderBy(a => a.Key))
            {
                var quotes = g.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{CsUtils.GetString(g.Key)}\t{ts.GetContent()}");
            }
        }

        public static void ByWeek(List<IntradayQuote> iQuotes)
        {
            var data = new Dictionary<int, List<Quote>>();
            foreach (var q in iQuotes)
            {
                var key = q.Timed.Year * 100 + CsUtils.GetWeekOfDate(q.Timed);
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

            Debug.Print($"DateOfWeek\t{TradeStatistics.GetHeader()}");
            foreach (var g in data.OrderBy(a => a.Key))
            {
                var quotes = g.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var label = quotes.Max(a => a.Timed.Date).ToString("yyyy-MM-dd");
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{label}\t{ts.GetContent()}");
            }
        }

        public static void ByKind(List<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<string, List<Quote>>();
            foreach (var q in iQuotes)
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

        public static void BySector(List<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<string, List<Quote>>();
            foreach (var q in iQuotes)
            {
                var key = symbols[q.Symbol].Sector ?? "";
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

            Debug.Print($"Sector\t{TradeStatistics.GetHeader()}");
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{kvp.Key}\t{ts.GetContent()}");
            }
        }

        public static void ByIndustry(List<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<string, List<Quote>>();
            foreach (var q in iQuotes)
            {
                var key = symbols[q.Symbol].Industry ?? "";
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

            Debug.Print($"Industry\t{TradeStatistics.GetHeader()}");
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{kvp.Key}\t{ts.GetContent()}");
            }
        }

        public static void BySymbol(List<IntradayQuote> iQuotes)
        {
            Debug.Print($"Symbol\t{TradeStatistics.GetHeader()}");
            var group = iQuotes.GroupBy(o => o.Symbol);
            foreach (var g in group.OrderBy(a => a.Key))
            {
                var quotes = g.OrderBy(a => a.Timed).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{CsUtils.GetString(g.Key)}\t{ts.GetContent()}");
            }
        }

        public static void ByTradeValue(List<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<int, List<Quote>>();
            foreach (var q in iQuotes)
            {
                var key = symbols[q.Symbol].TradeValueId;
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

            Debug.Print($"TradeValueId\tMin of Trade Value\t{TradeStatistics.GetHeader()}");
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var min = symbols.Where(a => a.Value.TradeValueId == kvp.Key).Min(a => a.Value.TradeValue) / 1000000.0;
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{kvp.Key}\t{min}\t{ts.GetContent()}");
            }
        }

        public static void ByTradingViewSector(List<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<string, List<Quote>>();
            foreach (var q in iQuotes)
            {
                var key = symbols[q.Symbol].TvSector ?? "";
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

            Debug.Print($"TvSector\t{TradeStatistics.GetHeader()}");
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{kvp.Key}\t{ts.GetContent()}");
            }
        }

        public static void ByTradingViewType(List<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<string, List<Quote>>();
            foreach (var q in iQuotes)
            {
                var key = symbols[q.Symbol].TvType ?? "";
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

            Debug.Print($"TvType\t{TradeStatistics.GetHeader()}");
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{kvp.Key}\t{ts.GetContent()}");
            }
        }

        public static void ByTradingViewSubtype(List<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<string, List<Quote>>();
            foreach (var q in iQuotes)
            {
                var key = symbols[q.Symbol].TvFullType ?? "";
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

            Debug.Print($"TvType\\Subtype\t{TradeStatistics.GetHeader()}");
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{kvp.Key}\t{ts.GetContent()}");
            }
        }

        public static void ByKindAndTime(List<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<Tuple<string, TimeSpan>, List<Quote>>();
            foreach (var q in iQuotes)
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

                    var ts = new TradeStatistics(quotes);
                    Debug.Print($"{CsUtils.GetString(g1.Key)}\t{CsUtils.GetString(g2.Key)}\t{ts.GetContent()}");
                }
            }
        }

        public static void ByKindAndDayOfWeek(List<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<Tuple<string, DayOfWeek>, List<Quote>>();
            foreach (var q in iQuotes)
                foreach (var kind in symbols[q.Symbol].Kinds)
                {
                    var key = new Tuple<string, DayOfWeek>(kind, q.Timed.DayOfWeek);
                    if (!data.ContainsKey(key))
                        data.Add(key, new List<Quote>());
                    data[key].Add(q);
                }

            Debug.Print($"Kind\tDayOfWeek\t{TradeStatistics.GetHeader()}");

            var groups1 = data.GroupBy(a => a.Key.Item1).ToList();
            foreach (var g1 in groups1.OrderBy(a => a.Key))
            {
                var groups2 = g1.GroupBy(a => a.Key.Item2).ToList();
                foreach (var g2 in groups2.OrderBy(a => a.Key))
                {
                    var key = new Tuple<string, DayOfWeek>(g1.Key, g2.Key);
                    var quotes = data[key].OrderBy(a => a.Timed).ThenBy(a => a.Symbol).ToList();

                    var ts = new TradeStatistics(quotes);
                    Debug.Print($"{CsUtils.GetString(g1.Key)}\t{CsUtils.GetString(g2.Key)}\t{ts.GetContent()}");
                }
            }
        }

        public static void BySectorAndIndustry(List<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<Tuple<string, string>, List<Quote>>();
            foreach (var q in iQuotes)
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

        public static void ByExchangeAndAsset(List<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<Tuple<string, string>, List<Quote>>();
            foreach (var q in iQuotes)
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
    }
}
