using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static class IntradayResults
    {
        public static Dictionary<string, Func<List<IntradayQuote>, List<object[]>>> ActionList =
            new Dictionary<string, Func<List<IntradayQuote>, List<object[]>>>
            {
                {"By Time", ByTime}, {"By Date", ByDate}, {"By Day of Week", ByDayOfWeek}, {"By Week", ByWeek},
                {"By Kind", ByKind}, {"By Sector", BySector}, {"By Industry", ByIndustry}, {"By Symbol", BySymbol},
                {"By Trade Value", ByTradeValue}, {"By TradingView Type", ByTradingViewType},
                {"By TradingView Subtype", ByTradingViewSubtype}, {"By TradingViewSector", ByTradingViewSector},
                {"By TradingView SectorAndIndustry", ByTradingViewSectorAndIndustry},
                {"By TradingView Recommend", ByTradingViewRecommend}, {"By Kind and Time", ByKindAndTime},
                {"By Kind and DayOfWeek", ByKindAndDayOfWeek}, {"By Sector and Industry", BySectorAndIndustry},
                {"By Exchange and Asset", ByExchangeAndAsset}, {"By TradingViewSector and Time", ByTradingViewSectorAndTime}
            };

        public static List<object[]> ByTime(List<IntradayQuote> iQuotes) =>
            ByOneProperty(iQuotes, new[] {"Time"}, (quote, symbol) => quote.TimeFrameId);

        public static List<object[]> ByDate(List<IntradayQuote> iQuotes) =>
            ByOneProperty(iQuotes, new[] {"Date"}, (quote, symbol) => quote.Timed.Date);

        public static List<object[]> ByDayOfWeek(List<IntradayQuote> iQuotes) =>
            ByOneProperty(iQuotes, new[] {"Day of Week"}, (quote, symbol) => quote.Timed.DayOfWeek);

        public static List<object[]> ByWeek(List<IntradayQuote> iQuotes)
        {
            var data = new Dictionary<int, List<Quote>>();
            foreach (var q in iQuotes)
            {
                var key = q.Timed.Year * 100 + CsUtils.GetWeekOfDate(q.Timed);
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

            var lines = new List<object[]> { TradeStatistics.GetHeaders(new [] {"Date of Week"}) };
            foreach (var g in data.OrderBy(a => a.Key))
            {
                var quotes = g.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var label = quotes.Max(a => a.Timed.Date).ToString("yyyy-MM-dd");
                var ts = new TradeStatistics((IList<Quote>)quotes);
                var oo = new List<object> { label };
                oo.AddRange(ts.GetValues());
                lines.Add(oo.ToArray());
            }

            return lines;
        }

        public static List<object[]> ByKind(List<IntradayQuote> iQuotes)
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

            var lines = new List<object[]> { TradeStatistics.GetHeaders(new[] { "Kind" }) };
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                var oo = new List<object> { kvp.Key };
                oo.AddRange(ts.GetValues());
                lines.Add(oo.ToArray());
            }

            return lines;
        }

        public static List<object[]> BySector(List<IntradayQuote> iQuotes) =>
            ByOneProperty(iQuotes, new[] {"Sector"}, (quote, symbol) => symbol.Sector);

        public static List<object[]> ByIndustry(List<IntradayQuote> iQuotes) =>
            ByOneProperty(iQuotes, new[] {"Industry"}, (quote, symbol) => symbol.Industry);

        public static List<object[]> BySymbol(List<IntradayQuote> iQuotes) =>
            ByOneProperty(iQuotes, new[] {"Symbol"}, (quote, symbol) => quote.Symbol);

        public static List<object[]> ByTradeValue(List<IntradayQuote> iQuotes)
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

            var lines = new List<object[]> { TradeStatistics.GetHeaders(new[] { "TradeValueId", "Min of Trade Value" }) };
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var min = symbols.Where(a => a.Value.TradeValueId == kvp.Key).Min(a => a.Value.TradeValue) / 1000000.0;
                var ts = new TradeStatistics((IList<Quote>)quotes);
                var oo = new List<object> { kvp.Key, min };
                oo.AddRange(ts.GetValues());
                lines.Add(oo.ToArray());
            }
            return lines;
        }

        public static List<object[]> ByTradingViewType(List<IntradayQuote> iQuotes) =>
            ByOneProperty(iQuotes, new[] {"TvType"}, (quote,symbol) => symbol.TvType);

        public static List<object[]> ByTradingViewSubtype(List<IntradayQuote> iQuotes) =>
            ByOneProperty(iQuotes, new[] {"TvType/Subtype"}, (quote,symbol) => symbol.TvFullType);

        public static List<object[]> ByTradingViewSector(List<IntradayQuote> iQuotes) =>
            ByOneProperty(iQuotes, new[] { "TvSector" }, (quote, symbol) => symbol.TvSector);

        public static List<object[]> ByTradingViewSectorAndIndustry(List<IntradayQuote> iQuotes) =>
            ByOneProperty(iQuotes, new[] {"TvSector/Industry"}, (quote, symbol) => symbol.TvSectorAndIndustry);

        public static List<object[]> ByTradingViewRecommend(List<IntradayQuote> iQuotes)
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

            var lines = new List<object[]> { TradeStatistics.GetHeaders(new[] { "RecommendId", "Min of Recommend" }) };
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var min = symbols.Where(a => Equals(a.Value.TvRecommendId, kvp.Key)).Min(a => a.Value.TvRecommend);
                var ts = new TradeStatistics((IList<Quote>)quotes);
                var oo = new List<object> { kvp.Key, min };
                oo.AddRange(ts.GetValues());
                lines.Add(oo.ToArray());
            }

            return lines;
        }

        public static List<object[]> ByKindAndTime(List<IntradayQuote> iQuotes)
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

            var lines = new List<object[]> { TradeStatistics.GetHeaders(new[] { "Kind", "Time" }) };
            var groups1 = data.GroupBy(a => a.Key.Item1).ToList();
            foreach (var g1 in groups1.OrderBy(a => a.Key))
            {
                var groups2 = g1.GroupBy(a => a.Key.Item2).ToList();
                foreach (var g2 in groups2.OrderBy(a => a.Key))
                {
                    var key = new Tuple<string, TimeSpan>(g1.Key, g2.Key);
                    var quotes = data[key].OrderBy(a => a.Timed).ThenBy(a => a.Symbol).ToList();
                    var ts = new TradeStatistics(quotes);
                    var oo = new List<object> { g1.Key, g2.Key };
                    oo.AddRange(ts.GetValues());
                    lines.Add(oo.ToArray());
                }
            }
            return lines;
        }

        public static List<object[]> ByKindAndDayOfWeek(List<IntradayQuote> iQuotes)
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

            var lines = new List<object[]> { TradeStatistics.GetHeaders(new[] { "Kind", "DayOfWeek" }) };
            var groups1 = data.GroupBy(a => a.Key.Item1).ToList();
            foreach (var g1 in groups1.OrderBy(a => a.Key))
            {
                var groups2 = g1.GroupBy(a => a.Key.Item2).ToList();
                foreach (var g2 in groups2.OrderBy(a => a.Key))
                {
                    var key = new Tuple<string, DayOfWeek>(g1.Key, g2.Key);
                    var quotes = data[key].OrderBy(a => a.Timed).ThenBy(a => a.Symbol).ToList();
                    var ts = new TradeStatistics(quotes);
                    var oo = new List<object> { g1.Key, g2.Key };
                    oo.AddRange(ts.GetValues());
                    lines.Add(oo.ToArray());
                }
            }
            return lines;
        }

        public static List<object[]> BySectorAndIndustry(List<IntradayQuote> iQuotes) =>
            ByTwoProperties(iQuotes, new[] {"Sector", "Industry"}, (quote, symbol) => symbol.Sector,
                (quote, symbol) => symbol.Industry);

        public static List<object[]> ByExchangeAndAsset(List<IntradayQuote> iQuotes) => ByTwoProperties(iQuotes,
            new[] {"Exchange", "Asset"}, (quote, symbol) => symbol.Exchange, (quote, symbol) => symbol.Asset);

        public static List<object[]> ByTradingViewSectorAndTime(List<IntradayQuote> iQuotes) => ByTwoProperties(iQuotes,
            new[] {"TvSector", "Time"}, (quote, symbol) => symbol.TvSector, (quote, symbol) => quote.TimeFrameId);

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

        #region =================  Private methods  =================
        private static List<object[]> ByOneProperty(List<IntradayQuote> iQuotes, string[] startHeaders, Func<IntradayQuote, SymbolsOfDataSource, object> propertyValue)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<object, List<Quote>>();
            foreach (var q in iQuotes)
            {
                var key = propertyValue(q, symbols[q.Symbol]) ?? "";
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

            var lines = new List<object[]> { TradeStatistics.GetHeaders(startHeaders) };
            foreach (var kvp in data.OrderBy(a => a.Key))
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                var oo = new List<object> { kvp.Key };
                oo.AddRange(ts.GetValues());
                lines.Add(oo.ToArray());
            }
            return lines;
        }

        private static List<object[]> ByTwoProperties(List<IntradayQuote> iQuotes, string[] startHeaders, Func<IntradayQuote, SymbolsOfDataSource, object> firstProperty, Func<IntradayQuote, SymbolsOfDataSource, object> secondProperty)
        {
            var symbols = DataSources.GetActiveSymbols();
            var data = new Dictionary<Tuple<object, object>, List<Quote>>();
            foreach (var q in iQuotes)
            {
                var symbol = symbols[q.Symbol];
                // var key = new Tuple<string, string>(symbol.Sector, symbol.Industry);
                var key = new Tuple<object, object>(firstProperty(q, symbol) ?? "", secondProperty(q, symbol) ?? "");
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

            var lines = new List<object[]> { TradeStatistics.GetHeaders(startHeaders) };
            var groups1 = data.GroupBy(a => a.Key.Item1).ToList();
            foreach (var g1 in groups1.OrderBy(a => a.Key))
            {
                var groups2 = g1.GroupBy(a => a.Key.Item2).ToList();
                foreach (var g2 in groups2.OrderBy(a => a.Key))
                {
                    var key = new Tuple<object, object>(g1.Key, g2.Key);
                    var quotes = data[key].OrderBy(a => a.Timed).ThenBy(a => a.Symbol).ToList();
                    var ts = new TradeStatistics(quotes);
                    var oo = new List<object> { g1.Key, g2.Key };
                    oo.AddRange(ts.GetValues());
                    lines.Add(oo.ToArray());
                }
            }
            return lines;
        }
        #endregion
    }
}
