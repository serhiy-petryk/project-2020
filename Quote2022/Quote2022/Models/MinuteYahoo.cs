using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote2022.Models
{
    public class MinuteYahoo
    {
        public cChart Chart { get; set; }
        public Quote[] Quotes { get; set; }

        public static DateTime TimeStampToDateTime(long timeStamp, IEnumerable<cTradingPeriod> periods)
        {
            var aa = periods.Where(p => p.Start <= timeStamp && p.End >= timeStamp).ToArray();
            if (aa.Length == 1)
                return (new DateTime(1970, 1, 1)).AddSeconds(timeStamp).AddSeconds(aa[0].GmtOffset);
            throw new Exception("Check TimeStampToDateTime procedure in Quote2022.Models.MinuteYahoo");
        }

        public void Normilize()
        {
            if (Chart.Result[0].TimeStamp == null)
            {
                Quotes = new Quote[0];
                if (Chart.Result[0].Indicators.Quote[0].Close != null)
                    throw new Exception("Check Normilize procedure in  in Quote2022.Models.MinuteYahoo");
                return;
            }

            var periods = new List<MinuteYahoo.cTradingPeriod>();
            var a1 = Chart.Result[0].Meta.TradingPeriods;
            var len1 = a1.GetLength(1);
            for (var k1 = 0; k1 < a1.Length; k1++)
            for (var k2 = 0; k2 < len1; k2++)
                periods.Add(a1[k1, k2]);
            
            periods = periods.OrderBy(a => a.Start).ToList();
            
            Quotes = new Quote[Chart.Result[0].TimeStamp.Length];
            for (var k = 0; k < Quotes.Length; k++)
            {
                Quotes[k] = new Quote()
                {
                    Timed = TimeStampToDateTime(Chart.Result[0].TimeStamp[k], periods),
                    Open = Chart.Result[0].Indicators.Quote[0].Open[k],
                    High = Chart.Result[0].Indicators.Quote[0].High[k],
                    Low = Chart.Result[0].Indicators.Quote[0].Low[k],
                    Close = Chart.Result[0].Indicators.Quote[0].Close[k],
                    Volume = Chart.Result[0].Indicators.Quote[0].Volume[k]
                };
            }
        }

        #region ===============  SubClasses  ==================
        public class cChart
        {
            public cResult[] Result { get; set; }
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
            public cTradingPeriod[,] TradingPeriods { get; set; }
        }

        public class cIndicators
        {
            public cQuote[] Quote { get; set; }
        }

        public class cQuote
        {
          public decimal?[] Open { get; set; }
          public decimal?[] High { get; set; }
          public decimal?[] Low { get; set; }
          public decimal?[] Close { get; set; }
          public long?[] Volume { get; set; }
        }

        public class cTradingPeriod
        {
          public string TimeZone { get; set; }
          public long Start { get; set; }
          public long End { get; set; }
          public long GmtOffset { get; set; }
        }

        public class Quote
        {
            public DateTime Timed { get; set; }
            public decimal? Open { get; set; }
            public decimal? High { get; set; }
            public decimal? Low { get; set; }
            public decimal? Close { get; set; }
            public long? Volume { get; set; }
            public override string ToString()
            {
                return Timed + " " + (Open.HasValue ? Math.Round(Open.Value, 4).ToString() : "") + " " +
                       (High.HasValue ? Math.Round(High.Value, 4).ToString() : "") + " " +
                       (Low.HasValue ? Math.Round(Low.Value, 4).ToString() : "") + " " +
                       (Close.HasValue ? Math.Round(Close.Value, 4).ToString() : "") + " " +
                       Volume;
            }
        }
        #endregion
    }
}
