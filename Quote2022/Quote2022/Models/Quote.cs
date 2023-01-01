using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote2022.Models
{
    public class Quote
    {
        public string Symbol { get; set; }
        public DateTime Timed { get; set; }
        public string TimeString => Timed.ToString(Timed.TimeOfDay == TimeSpan.Zero ? "yyyy-Mm-dd" : "HH:mm");
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public long Volume { get; set; }
        public override string ToString() => Symbol + "\t" + Timed + "\t" + Open + "\t" + High + "\t" + Low + "\t" + Close + "\t" + Volume;
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
