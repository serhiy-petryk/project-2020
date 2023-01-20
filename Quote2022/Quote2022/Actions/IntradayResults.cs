﻿using System;
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
        public static Dictionary<string, Func<IEnumerable<IntradayQuote>, List<object[]>>> ActionList =
            new Dictionary<string, Func<IEnumerable<IntradayQuote>, List<object[]>>>
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

        public static List<object[]> ByTime(IEnumerable<IntradayQuote> iQuotes) =>
            ByQuoteProperty(iQuotes, new[] { "Time" }, (quote) => quote.TimeFrameId);

        public static List<object[]> ByDate(IEnumerable<IntradayQuote> iQuotes) =>
            ByQuoteProperty(iQuotes, new[] { "Date" }, (quote) => quote.Timed.Date);

        public static List<object[]> ByDayOfWeek(IEnumerable<IntradayQuote> iQuotes) =>
            ByQuoteProperty(iQuotes, new[] { "Day of Week" }, (quote) => quote.Timed.DayOfWeek);

        public static List<object[]> ByWeek(IEnumerable<IntradayQuote> iQuotes)
        {
            var lines = new List<object[]> { TradeStatistics.GetHeaders(new[] { "Dates of Week" }) };
            foreach (var g in iQuotes.GroupBy(a => a.Timed.Year * 100 + CsUtils.GetWeekOfDate(a.Timed)).OrderBy(a => a.Key))
            {
                var label = g.Min(a => a.Timed.Date).ToString("yyyy-MM-dd") + "/" +
                            g.Max(a => a.Timed.Date).ToString("yyyy-MM-dd");
                var ts = new TradeStatistics(g);
                lines.Add(ts.GetValues(new object[] { label }).ToArray());
            }

            return lines;
        }

        public static List<object[]> ByKind(IEnumerable<IntradayQuote> iQuotes)
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
                var ts = new TradeStatistics(kvp.Value);
                lines.Add(ts.GetValues(new object[] { kvp.Key }));
            }

            return lines;
        }

        public static List<object[]> BySector(IEnumerable<IntradayQuote> iQuotes) =>
            BySymbolProperty(iQuotes, new[] { "Sector" }, (symbol) => symbol.Sector);

        public static List<object[]> ByIndustry(IEnumerable<IntradayQuote> iQuotes) =>
            BySymbolProperty(iQuotes, new[] { "Industry" }, (symbol) => symbol.Industry);

        public static List<object[]> BySymbol(IEnumerable<IntradayQuote> iQuotes) =>
            ByQuoteProperty(iQuotes, new[] { "Symbol" }, (quote) => quote.Symbol);

        public static List<object[]> ByTradeValue(IEnumerable<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var lines = new List<object[]> { TradeStatistics.GetHeaders(new[] { "TradeValueId", "Min of Trade Value" }) };
            foreach (var oo in iQuotes.GroupBy(o => symbols[o.Symbol].TradeValueId).OrderBy(a => a.Key))
            {
                var min = symbols.Where(a => a.Value.TradeValueId == oo.Key).Min(a => a.Value.TradeValue) / 1000000.0;
                var ts = new TradeStatistics(oo);
                lines.Add(ts.GetValues(new object[] { oo.Key, min }));
            }
            return lines;
        }

        public static List<object[]> ByTradingViewType(IEnumerable<IntradayQuote> iQuotes) =>
            BySymbolProperty(iQuotes, new[] { "TvType" }, (symbol) => symbol.TvType);

        public static List<object[]> ByTradingViewSubtype(IEnumerable<IntradayQuote> iQuotes) =>
            BySymbolProperty(iQuotes, new[] { "TvType/Subtype" }, (symbol) => symbol.TvFullType);

        public static List<object[]> ByTradingViewSector(IEnumerable<IntradayQuote> iQuotes) =>
            BySymbolProperty(iQuotes, new[] { "TvSector" }, (symbol) => symbol.TvSector);

        public static List<object[]> ByTradingViewSectorAndIndustry(IEnumerable<IntradayQuote> iQuotes) =>
            BySymbolProperty(iQuotes, new[] { "TvSector/Industry" }, (symbol) => symbol.TvSectorAndIndustry);

        public static List<object[]> ByTradingViewRecommend(IEnumerable<IntradayQuote> iQuotes)
        {
            var symbols = DataSources.GetActiveSymbols();
            var lines = new List<object[]> { TradeStatistics.GetHeaders(new[] { "RecommendId", "Min of Recommend" }) };
            foreach (var oo in iQuotes.GroupBy(a => symbols[a.Symbol].TvRecommendId).OrderBy(a => a.Key))
            {
                var min = symbols.Where(a => Equals(a.Value.TvRecommendId, oo.Key)).Min(a => a.Value.TvRecommend);
                var ts = new TradeStatistics(oo);
                lines.Add(ts.GetValues(new object[] { oo.Key, min }));
            }

            return lines;
        }

        public static List<object[]> ByKindAndTime(IEnumerable<IntradayQuote> iQuotes)
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
            foreach (var g1 in data.GroupBy(a => a.Key.Item1).OrderBy(a => a.Key))
            foreach (var g2 in g1.GroupBy(a => a.Key.Item2).OrderBy(a => a.Key))
            {
                var ts = new TradeStatistics(data[new Tuple<string, TimeSpan>(g1.Key, g2.Key)]);
                lines.Add(ts.GetValues(new object[] {g1.Key, g2.Key}));
            }

            return lines;
        }

        public static List<object[]> ByKindAndDayOfWeek(IEnumerable<IntradayQuote> iQuotes)
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
            foreach (var g1 in data.GroupBy(a => a.Key.Item1).OrderBy(a => a.Key))
            foreach (var g2 in g1.GroupBy(a => a.Key.Item2).OrderBy(a => a.Key))
            {
                var ts = new TradeStatistics(data[new Tuple<string, DayOfWeek>(g1.Key, g2.Key)]);
                lines.Add(ts.GetValues(new object[] {g1.Key, g2.Key}));
            }

            return lines;
        }

        public static List<object[]> BySectorAndIndustry(IEnumerable<IntradayQuote> iQuotes) =>
            ByTwoProperties(iQuotes, new[] { "Sector", "Industry" }, (quote, symbol) => symbol.Sector,
                (quote, symbol) => symbol.Industry);

        public static List<object[]> ByExchangeAndAsset(IEnumerable<IntradayQuote> iQuotes) => ByTwoProperties(iQuotes,
            new[] { "Exchange", "Asset" }, (quote, symbol) => symbol.Exchange, (quote, symbol) => symbol.Asset);

        public static List<object[]> ByTradingViewSectorAndTime(IEnumerable<IntradayQuote> iQuotes) => ByTwoProperties(iQuotes,
            new[] { "TvSector", "Time" }, (quote, symbol) => symbol.TvSector, (quote, symbol) => quote.TimeFrameId);

        public static void ByTimeNew(Action<string> showStatusAction, IEnumerable<Quote> quotes, IEnumerable<TimeSpan> timeFrames, bool closeInNextFrame)
        {
            var oo = QuoteLoader.GetIntradayQuotes(showStatusAction, quotes, timeFrames, closeInNextFrame);

            Debug.Print(string.Join("\t", TradeStatistics.GetHeaders(new[] { "Time" })));
            var group = oo.GroupBy(o => o.TimeFrameId);
            foreach (var g in group.OrderBy(a => a.Key))
            {
                var qq = g.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)qq);
                Debug.Print(string.Join("\t", ts.GetValues(new object[] { g.Key }).Select(CsUtils.GetString)));
            }
        }

        #region =================  Private methods  =================
        private static List<object[]> ByQuoteProperty(IEnumerable<IntradayQuote> iQuotes, string[] startHeaders, Func<IntradayQuote, object> propertyValue)
        {
            var lines = new List<object[]> { TradeStatistics.GetHeaders(startHeaders) };
            foreach (var g in iQuotes.GroupBy(propertyValue).OrderBy(o => o.Key))
            {
                var ts = new TradeStatistics(g);
                lines.Add(ts.GetValues(new object[] { g.Key }));
            }

            return lines;
        }

        private static List<object[]> BySymbolProperty(IEnumerable<IntradayQuote> iQuotes, string[] startHeaders, Func<SymbolsOfDataSource, object> propertyValue)
        {
            var symbols = DataSources.GetActiveSymbols();
            var lines = new List<object[]> { TradeStatistics.GetHeaders(startHeaders) };
            foreach (var g in iQuotes.GroupBy(o => propertyValue(symbols[o.Symbol])).OrderBy(o => o.Key))
            {
                var ts = new TradeStatistics(g);
                lines.Add(ts.GetValues(new object[] { g.Key }));
            }

            return lines;
        }

        private static List<object[]> ByTwoProperties(IEnumerable<IntradayQuote> iQuotes, string[] startHeaders, Func<IntradayQuote, SymbolsOfDataSource, object> firstProperty, Func<IntradayQuote, SymbolsOfDataSource, object> secondProperty)
        {
            var symbols = DataSources.GetActiveSymbols();
            var lines = new List<object[]> { TradeStatistics.GetHeaders(startHeaders) };

            foreach (var g1 in iQuotes.GroupBy(o => firstProperty(o, symbols[o.Symbol])).OrderBy(o => o.Key))
            foreach (var g2 in g1.GroupBy(o => secondProperty(o, symbols[o.Symbol])).OrderBy(o => o.Key))
            {
                var ts = new TradeStatistics(g2);
                lines.Add(ts.GetValues(new object[] {g1.Key, g2.Key}));
            }

            return lines;
        }
        #endregion
    }
}
