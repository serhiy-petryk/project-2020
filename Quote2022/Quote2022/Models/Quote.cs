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
    }

    public class IntradayQuote: Quote
    {
        public TimeSpan TimeFrameId;
        public TimeSpan CloseAt;
        public override string ToString() => Symbol + "\t" + CsUtils.GetString(Timed) +"-" + CloseAt.ToString(@"hh\:mm") + "\t" + Open + "\t" + High + "\t" + Low + "\t" + Close + "\t" + Volume;
    }

    public class QuoteWithGroup : Quote
    {
        public object G1;
        public object G2;
        public object G3;

        public QuoteWithGroup(Quote q, object group1, object group2=null, object group3 =null)
        {
            Symbol = q.Symbol;
            Timed = q.Timed;
            Open = q.Open;
            High = q.High;
            Low = q.Low;
            Close = q.Close;
            Volume = q.Volume;
            G1 = group1;
            G2 = group2;
            G3 = group3;
        }
    }
}
