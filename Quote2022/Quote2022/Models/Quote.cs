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
        public string ToStringOfBaseClass() => base.ToString();
        // public override string ToString() => Symbol + "\t" + CsUtils.GetString(Timed) + "-" + CloseAt.ToString(@"hh\:mm") + "\t" + Open + "\t" + High + "\t" + Low + "\t" + Close + "\t" + Volume;
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
