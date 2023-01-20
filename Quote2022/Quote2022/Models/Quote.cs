using System;
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
        public TimeSpan CloseAt;
        public string ToStringOfBaseClass() => base.ToString();
        public override string ToString() => Symbol + "\t" + CsUtils.GetString(Timed) + "-" + CloseAt.ToString(@"hh\:mm") + "\t" + Open + "\t" + High + "\t" + Low + "\t" + Close + "\t" + Volume;
    }
}
