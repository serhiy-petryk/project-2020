using System;
using System.Collections.Generic;
using Quote2022.Helpers;

namespace Quote2022.Models
{
    public class Quote
    {
        public string Symbol;
        public DateTime Timed;
        public string TimeString => CsUtils.GetString(Timed);
        public float Open;
        public float High;
        public float Low;
        public float Close;
        public long Volume;
        public override string ToString() => Symbol + "\t" + CsUtils.GetString(Timed) + "\t" + Open + "\t" + High + "\t" + Low + "\t" + Close + "\t" + Volume;

        public string QuoteError =>
            (Open > High || Open < Low || Close > High || Close < Low || Low < 0)
                ? "BadPrices"
                : (Volume < 0 ? "BadVolume" : null);
    }

    public class IntradayQuote : Quote
    {
        public TimeSpan TimeFrameId;
        // public TimeSpan CloseAt;
        // public override string ToString() => Symbol + "\t" + CsUtils.GetString(Timed) + "-" + CloseAt.ToString(@"hh\:mm") + "\t" + Open + "\t" + High + "\t" + Low + "\t" + Close + "\t" + Volume;
    }

    public class StatisticsQuote : IntradayQuote
    {
        public DateTime Date => Timed.Date;
        public TimeSpan Time => Timed.TimeOfDay;
        public float OpenToCL => Convert.ToSingle((1.0 - 1.0 * Open / Close) * 100.0);
        public float BuyPerc => BuyAmt / Open * 100F;
        public float SellPerc => SellAmt / Open * 100F;
        public float BuyAmt;
        public float SellAmt;
        public bool BuyWins => BuyAmt > 0.0;
        public bool SellWins => SellAmt > 0.0;
        public DateTime Week => CsUtils.GetFirstDayOfWeek(Timed);
        public byte DayOfWeek => Convert.ToByte((int)Timed.DayOfWeek);
        public float Stop;
        public bool IsStopPerc;
        public float Fees;

        public StatisticsQuote(IntradayQuote q, IntradayParameters intradayParameters)
        {
            Symbol = q.Symbol;
            TimeFrameId = q.TimeFrameId;
            Timed = q.Timed;
            Open = q.Open;
            High = q.High;
            Low = q.Low;
            Close = q.Close;
            Volume = q.Volume;

            IsStopPerc = intradayParameters.IsStopPercent;
            Fees = Convert.ToSingle(intradayParameters.Fees);

            if (IsStopPerc)
            {
                Stop = Convert.ToSingle(Math.Round(Open * Convert.ToDouble(intradayParameters.Stop) / 100.0, 2));
                if (Stop < 0.01) Stop = 0.01F;
            }
            else
                Stop = Convert.ToSingle(intradayParameters.Stop);

            var buyEndAmt = (Open - Low) < (Stop - 0.00005F) ? Close : Open - Stop;
            var sellEndAmt = (High - Open) < (Stop - 0.00005) ? Close : Open + Stop;
            BuyAmt = Convert.ToSingle(Math.Round(buyEndAmt - Open - Fees, 4));
            SellAmt = Convert.ToSingle(Math.Round(Open - sellEndAmt - Fees, 4));
        }
    }

    public class QuotesInfo
    {
        public int Count = 0;
        public DateTime From = DateTime.MaxValue;
        public DateTime To = DateTime.MinValue;
        public Dictionary<string, object> Symbols = new Dictionary<string, object>();

        public void Update(Quote quote)
        {
            Count++;
            if (quote.Timed < From) From = quote.Timed;
            if (quote.Timed > To) To = quote.Timed;
            if (!Symbols.ContainsKey(quote.Symbol)) Symbols.Add(quote.Symbol, null);
        }

        public string GetStatus() =>
            $"Data from {CsUtils.GetString(From)} to {CsUtils.GetString(To)}, {Symbols.Count} symbols, {Count:N0} quotes";
    }
}
