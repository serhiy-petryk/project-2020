﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Quote2022.Models
{
    public class MinuteYahoo
    {
        public cChart Chart { get; set; }
        // public Quote[] Quotes { get; set; }

        public static DateTime TimeStampToDateTime(long timeStamp, IEnumerable<cTradingPeriod> periods)
        {
            var aa = periods.Where(p => p.Start <= timeStamp && p.End >= timeStamp).ToArray();
            if (aa.Length == 1)
                return (new DateTime(1970, 1, 1)).AddSeconds(timeStamp).AddSeconds(aa[0].GmtOffset);
            throw new Exception("Check TimeStampToDateTime procedure in Quote2022.Models.MinuteYahoo");
        }

        public Quote[] GetQuotes()
        {
            Quote[] quotes;
            if (Chart.Result[0].TimeStamp == null)
            {
                quotes = new Quote[0];
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
            
            quotes = new Quote[Chart.Result[0].TimeStamp.Length];
            for (var k = 0; k < quotes.Length; k++)
            {
                quotes[k] = new Quote()
                {
                    Timed = TimeStampToDateTime(Chart.Result[0].TimeStamp[k], periods),
                    Open = ConvertToFloat(Chart.Result[0].Indicators.Quote[0].Open[k]),
                    High = ConvertToFloat(Chart.Result[0].Indicators.Quote[0].High[k]),
                    Low = ConvertToFloat(Chart.Result[0].Indicators.Quote[0].Low[k]),
                    Close = ConvertToFloat(Chart.Result[0].Indicators.Quote[0].Close[k]),
                    Volume = Chart.Result[0].Indicators.Quote[0].Volume[k]
                };
            }

            return quotes;

            float? ConvertToFloat(double? o) => o.HasValue ? Convert.ToSingle(o.Value) : (float?) null;
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

        public class Quote
        {
            public string Symbol { get; set; }
            public DateTime Timed { get; set; }
            public float? Open { get; set; }
            public float? High { get; set; }
            public float? Low { get; set; }
            public float? Close { get; set; }
            public long? Volume { get; set; }
            public override string ToString() => Symbol + "\t" + Timed + "\t" + Open + "\t" + High + "\t" + Low + "\t" + Close + "\t" + Volume;
        }
        #endregion
    }
}
