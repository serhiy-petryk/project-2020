using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static class IntradayResults
    {
        public static void ByKind(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var symbols = DataSources.GetSymbolsAndKinds();
            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            var data = new Dictionary<string, List<Quote>>();
            foreach (var q in oo.OrderBy(a => a.Timed))
            foreach (var kind in symbols[q.Symbol])
            {
                if (!data.ContainsKey(kind))
                    data.Add(kind, new List<Quote>());
                data[kind].Add(q);
            }

            Debug.Print($"Kind\t{TradeStatistics.GetHeader()}");
            foreach (var kvp in data)
            {
                var quotes = kvp.Value.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{kvp.Key}\t{ts.GetContent()}");
            }
        }

        public static void ByTime(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var symbols = DataSources.GetSymbolsAndKinds();
            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            Debug.Print($"Time\t{TradeStatistics.GetHeader()}");
            var group = oo.GroupBy(o => o.TimeFrameId);
            foreach (var g in group)
            {
                var quotes = g.OrderBy(a => a.Timed).ThenBy(a => a.Symbol).Select(a => (Quote)a).ToList();
                var ts = new TradeStatistics((IList<Quote>)quotes);
                Debug.Print($"{CsUtils.GetString(g.Key)}\t{ts.GetContent()}");
            }
        }

        public static void ByKindAndDate(bool fullTime, bool is30MinuteInterval, Action<string> showStatusAction, IEnumerable<string> zipFiles, bool useLastData)
        {
            var printDetails = false;
            var symbols = DataSources.GetSymbolsAndKinds();
            var data = new Dictionary<Tuple<string, DateTime>, List<Quote>>();

            var oo = QuoteLoader.GetYahooIntradayQuotes(showStatusAction, zipFiles, GetTimeFrames(fullTime, is30MinuteInterval),
                (s => !symbols.ContainsKey(s)), !fullTime, useLastData);

            foreach (var q in oo.OrderBy(a => a.Timed))
                foreach (var kind in symbols[q.Symbol])
                {
                    var key = new Tuple<string, DateTime>(kind, q.Timed.Date);
                    if (!data.ContainsKey(key))
                        data.Add(key, new List<Quote>());
                    data[key].Add(q);
                }

            Debug.Print($"Kind\tDate\t{TradeStatistics.GetHeader()}");

            var groups1 = data.GroupBy(a => a.Key.Item1).ToList();
            foreach (var g1 in groups1)
            {
                var groups2 = g1.GroupBy(a => a.Key.Item2).ToList();
                foreach (var g2 in groups2)
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

        private static List<TimeSpan> GetTimeFrames(bool fullDay)
        {
            return fullDay
                ? CsUtils.GetTimeFrames(new TimeSpan(9, 30, 0), new TimeSpan(16, 00, 0), new TimeSpan(0, 30, 0))
                : CsUtils.GetTimeFrames(new TimeSpan(10, 00, 0), new TimeSpan(15, 30, 0), new TimeSpan(0, 30, 0));
        }
        private static List<TimeSpan> GetTimeFrames(bool fullDay, bool is30MinutesInterval)
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
