using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote2022.Models
{
    public class YahooMinute
    {
        public cChart Chart { get; set; }
        public Quote[] Quotes { get; set; }

        public DateTime TimeStampToDateTime(long timeStamp)
        {
            return (new DateTime(1970, 1, 1)).AddSeconds(timeStamp).AddSeconds(Chart.Result[0].Meta.GmtOffset);
        }

        public void Normilize()
        {
            Quotes = new Quote[Chart.Result[0].TimeStamp.Length];
            for (var k = 0; k < Quotes.Length; k++)
                Quotes[k] = new Quote()
                {
                    TimeStamp = Chart.Result[0].TimeStamp[k],
                    GmtOffset = Chart.Result[0].Meta.GmtOffset,
                    Open = Chart.Result[0].Indicators.Quote[0].Open[k],
                    High = Chart.Result[0].Indicators.Quote[0].High[k],
                    Low = Chart.Result[0].Indicators.Quote[0].Low[k],
                    Close = Chart.Result[0].Indicators.Quote[0].Close[k],
                    Volume = Chart.Result[0].Indicators.Quote[0].Volume[k]
                };
        }

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
            public long GmtOffset { get; set; }
            public string TimeZone { get; set; }
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

        public class Quote
        {
            public long TimeStamp { get; set; }
            public long GmtOffset { get; set; }

            public DateTime Timed => (new DateTime(1970, 1, 1)).AddSeconds(TimeStamp).AddSeconds(GmtOffset);
            // public DateTime timestamp;
            public decimal? Open { get; set; }
            public decimal? High { get; set; }
            public decimal? Low { get; set; }
            public decimal? Close { get; set; }
            public long? Volume { get; set; }
            public override string ToString()
            {
                return TimeStamp + " " + Timed + " " + (Open.HasValue ? Math.Round(Open.Value, 4).ToString() : "") + " " +
                       (High.HasValue ? Math.Round(High.Value, 4).ToString() : "") + " " +
                       (Low.HasValue ? Math.Round(Low.Value, 4).ToString() : "") + " " +
                       (Close.HasValue ? Math.Round(Close.Value, 4).ToString() : "") + " " +
                       Volume;
            }
        }
    }
}
