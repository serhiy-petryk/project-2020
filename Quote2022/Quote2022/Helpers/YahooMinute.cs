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
            /*var printDetails = true;
            var symbols = DataSources.GetSymbolsAndKinds(750);
            var data = new Dictionary<Tuple<string, DateTime>, List<Quote>>();

            var oo = QuoteLoader.GetYahooIntradayQuotes(
                new[] {@"E:\Quote\WebData\Minute\Yahoo\YahooMinute_20221224.zip"},
                CsUtils.GetTimeFrames(new TimeSpan(10, 0, 0), new TimeSpan(15, 30, 0), new TimeSpan(0, 30, 0)),
                (s => !symbols.ContainsKey(s)), true);

            foreach (var q in oo.OrderBy(a => a.Timed))
            foreach (var kind in symbols[q.Symbol])
            {
                var key = new Tuple<string, DateTime>(kind, q.Timed.Date);
                if (!data.ContainsKey(key))
                    data.Add(key, new List<Quote>());
                data[key].Add(q);
            }

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
            }*/
        }

    }
}
