using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Quote2022.Models
{
    public class MinuteYahoo
    {
        public class QuoteCorrection
        {
            public float[] PriceValues;
            public int? VolumeFactor;
            public float? Split;
            public bool Remove = false;
            public bool PriceChecked = false;
            public bool SplitChecked = false;
        }

        public cChart Chart { get; set; }

        private static Dictionary<string, Dictionary<DateTime, QuoteCorrection>> _allCorrections = null;

        public static bool IsQuotePriceChecked(Quote q) => _allCorrections.ContainsKey(q.Symbol) &&
                                                           _allCorrections[q.Symbol].ContainsKey(q.Timed) &&
                                                           _allCorrections[q.Symbol][q.Timed].PriceChecked;
        public static bool IsQuoteSplitChecked(Quote q) => _allCorrections.ContainsKey(q.Symbol) &&
                                                           _allCorrections[q.Symbol].ContainsKey(q.Timed) &&
                                                           _allCorrections[q.Symbol][q.Timed].SplitChecked;

        public static Dictionary<DateTime, QuoteCorrection> GetCorrections(string symbol)
        {
            if (_allCorrections == null)
            {
                _allCorrections=new Dictionary<string, Dictionary<DateTime, QuoteCorrection>>();
                var lines = File.ReadAllLines(Settings.MinuteYahooCorrectionFiles)
                    .Where(a => !string.IsNullOrEmpty(a) && !a.Trim().StartsWith("#"));
                foreach (var line in lines)
                {
                    var ss = line.Split('\t');
                    var symbolKey = ss[0].Trim().ToUpper();
                    if (!_allCorrections.ContainsKey(symbolKey))
                        _allCorrections.Add(symbolKey, new Dictionary<DateTime, QuoteCorrection>());

                    var a1 = _allCorrections[symbolKey];
                    var date = DateTime.Parse(ss[1], CultureInfo.InvariantCulture);
                    if (!a1.ContainsKey(date))
                        a1.Add(date, new QuoteCorrection());
                    
                    var a2 = a1[date];
                    switch (ss[2].Trim().ToUpper())
                    {
                        case "REMOVE":
                        case "DELETE":
                            a2.Remove = true;
                            break;
                        case "PRICE":
                            a2.PriceValues = new float[4];
                            for (var k = 0; k < 4; k++)
                                a2.PriceValues[k] = float.Parse(ss[k + 3].Trim(), CultureInfo.InvariantCulture);
                            break;
                        case "SPLIT":
                            var f1 = float.Parse(ss[3].Trim(), CultureInfo.InvariantCulture);
                            var f2 = float.Parse(ss[4].Trim(), CultureInfo.InvariantCulture);
                            a2.Split = f1 / f2;
                            break;
                        case "PRICECHECKED":
                            a2.PriceChecked = true;
                            break;
                        case "SPLITCHECKED":
                            a2.SplitChecked = true;
                            break;
                        case "VOLUME":
                            a2.VolumeFactor = int.Parse(ss[3].Trim());
                            break;
                        default:
                            throw new Exception($"Check MinuteYahoo correction file: {Settings.MinuteYahooCorrectionFiles}. '{ss[2]}' is invalid action");
                    }
                }
            }

            return _allCorrections.ContainsKey(symbol) ? _allCorrections[symbol] : null;
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
            var corrections = GetCorrections(symbol);
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
                    var q = new Quote()
                    {
                        Symbol = symbol,
                        Timed = TimeStampToDateTime(Chart.Result[0].TimeStamp[k], periods),
                        Open = ConvertToFloat(Chart.Result[0].Indicators.Quote[0].Open[k].Value),
                        High = ConvertToFloat(Chart.Result[0].Indicators.Quote[0].High[k].Value),
                        Low = ConvertToFloat(Chart.Result[0].Indicators.Quote[0].Low[k].Value),
                        Close = ConvertToFloat(Chart.Result[0].Indicators.Quote[0].Close[k].Value),
                        Volume = Chart.Result[0].Indicators.Quote[0].Volume[k].Value
                    };

                    //CCNC	2022-11-08 15:59
                    if ((q.Symbol == "CHRA") && (q.Timed == new DateTime(2022, 12, 29, 15,59,0)))
                    {

                    }
                    if (corrections != null) CorrectQuote(q);
                    if (!float.IsNaN(q.Open))
                        quotes.Add(q);
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

            void CorrectQuote(Quote q1)
            {
                if (corrections.ContainsKey(q1.Timed))
                {
                    var x1 = corrections[q1.Timed];
                    if (x1.Remove)
                    {
                        q1.Open = float.NaN;
                        return;
                    }

                    if (x1.PriceValues != null)
                    {
                        q1.Open = x1.PriceValues[0];
                        q1.High = x1.PriceValues[1];
                        q1.Low = x1.PriceValues[2];
                        q1.Close = x1.PriceValues[3];
                    }
                    if (x1.VolumeFactor.HasValue)
                        q1.Volume = q1.Volume / x1.VolumeFactor.Value;
                }

                if (corrections.ContainsKey(q1.Timed.Date))
                {
                    var x1 = corrections[q1.Timed.Date];
                    if (x1.Split.HasValue)
                    {
                        q1.Open = Convert.ToSingle(Math.Round(q1.Open * x1.Split.Value,4));
                        q1.High = Convert.ToSingle(Math.Round(q1.High * x1.Split.Value, 4));
                        q1.Low = Convert.ToSingle(Math.Round(q1.Low * x1.Split.Value, 4));
                        q1.Close = Convert.ToSingle(Math.Round(q1.Close * x1.Split.Value, 4));
                    }
                }
            }
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
