using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Quote2022.Models
{
    public class MinuteYahoo
    {
        public cChart Chart { get; set; }

        private static Dictionary<Tuple<string, DateTime>, string[]> _corrections = null;
        public static Dictionary<Tuple<string, DateTime>, string[]> GetCorrections()
        {
            if (_corrections == null)
            {
                var lines = File.ReadAllLines(Settings.MinuteYahooCorrectionFiles)
                    .Where(a => !string.IsNullOrEmpty(a) && !a.Trim().StartsWith("#"));
                foreach (var line in lines)
                {
                    var ss = line.Split('\t');
                    _corrections.Add(new Tuple<string, DateTime>(ss[0], DateTime.Parse(ss[1], CultureInfo.InvariantCulture)), ss);
                }
            }

            return _corrections;
        }

        private static DateTime TimeStampToDateTime(long timeStamp, IEnumerable<cTradingPeriod> periods)
        {
            var aa = periods.Where(p => p.Start <= timeStamp && p.End >= timeStamp).ToArray();
            if (aa.Length == 1)
                return (new DateTime(1970, 1, 1)).AddSeconds(timeStamp).AddSeconds(aa[0].GmtOffset);
            throw new Exception("Check TimeStampToDateTime procedure in Quote2022.Models.MinuteYahoo");
        }

        public List<Quote> GetQuotes(string symbol)
        {
            var quotes = new List<Quote>();
            if (Chart.Result[0].TimeStamp == null)
            {
                if (Chart.Result[0].Indicators.Quote[0].Close != null)
                    throw new Exception("Check Normilize procedure in  in Quote2022.Models.MinuteYahoo");
                return quotes;
            }

            var periods = new List<MinuteYahoo.cTradingPeriod>();
            var a1 = Chart.Result[0].Meta.TradingPeriods;
            var len1 = a1.GetLength(1);
            for (var k1 = 0; k1 < a1.Length; k1++)
            for (var k2 = 0; k2 < len1; k2++)
                periods.Add(a1[k1, k2]);
            
            periods = periods.OrderBy(a => a.Start).ToList();

            for (var k = 0; k < Chart.Result[0].TimeStamp.Length; k++)
            {
                if (Chart.Result[0].Indicators.Quote[0].Open[k].HasValue &&
                    Chart.Result[0].Indicators.Quote[0].High[k].HasValue &&
                    Chart.Result[0].Indicators.Quote[0].Low[k].HasValue &&
                    Chart.Result[0].Indicators.Quote[0].Close[k].HasValue &&
                    Chart.Result[0].Indicators.Quote[0].Volume[k].HasValue)
                {
                    quotes.Add(new Quote()
                    {
                        Symbol = symbol,
                        Timed = TimeStampToDateTime(Chart.Result[0].TimeStamp[k], periods),
                        Open = ConvertToFloat(Chart.Result[0].Indicators.Quote[0].Open[k].Value),
                        High = ConvertToFloat(Chart.Result[0].Indicators.Quote[0].High[k].Value),
                        Low = ConvertToFloat(Chart.Result[0].Indicators.Quote[0].Low[k].Value),
                        Close = ConvertToFloat(Chart.Result[0].Indicators.Quote[0].Close[k].Value),
                        Volume = Chart.Result[0].Indicators.Quote[0].Volume[k].Value
                    });
                }
                else if (!Chart.Result[0].Indicators.Quote[0].Open[k].HasValue &&
                         !Chart.Result[0].Indicators.Quote[0].High[k].HasValue &&
                         !Chart.Result[0].Indicators.Quote[0].Low[k].HasValue &&
                         !Chart.Result[0].Indicators.Quote[0].Close[k].HasValue &&
                         !Chart.Result[0].Indicators.Quote[0].Volume[k].HasValue)
                {

                }
                else
                    throw new Exception($"Please, check quote data for {Chart.Result[0].TimeStamp[k]} timestamp (k={k})");
            }

            if (quotes.Count > 0 && quotes[quotes.Count - 1].Timed.TimeOfDay == new TimeSpan(16, 0, 0))
                quotes.RemoveAt(quotes.Count - 1);
            return quotes;

            float ConvertToFloat(double o) => Convert.ToSingle(o);
        }

        #region ===============  SubClasses  ==================
        public class cChart
        {
            public cResult[] Result { get; set; }
            public cError Error { get; set; }
        }

        public class cError
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }

        public class cResult
        {
            public cMeta Meta { get; set; }
            public long[] TimeStamp { get; set; }
            public cIndicators Indicators { get; set; }
        }

        public class cMeta
        {
            public string Currency { get; set; }
            public string Symbol { get; set; }
            public string ExchangeName { get; set; }
            public string InstrumentType { get; set; }
            public cTradingPeriod[,] TradingPeriods { get; set; }
        }

        public class cIndicators
        {
            public cQuote[] Quote { get; set; }
        }

        public class cQuote
        {
          public double?[] Open { get; set; }
          public double?[] High { get; set; }
          public double?[] Low { get; set; }
          public double?[] Close { get; set; }
          public long?[] Volume { get; set; }
        }

        public class cTradingPeriod
        {
          public string TimeZone { get; set; }
          public long Start { get; set; }
          public long End { get; set; }
          public long GmtOffset { get; set; }
        }
        #endregion
    }
}
