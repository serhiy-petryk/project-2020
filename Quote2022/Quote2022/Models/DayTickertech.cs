using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Quote2022.Models
{
    public class DayTickertech
    {
        public string Symbol;
        public DateTime Date;
        public float Open;
        public float High;
        public float Low;
        public float Close;
        public float Volume;
        public float SplitK;

        public DayTickertech(string symbol, DateTime date, float[] data, Dictionary<DateTime, string> divs)
        {
            SplitK = 1.0f;
            foreach (var kvp in divs.Where(a => a.Key >= date))
            {
                var ss = kvp.Value.Split(':');
                SplitK *= float.Parse(ss[0], CultureInfo.InvariantCulture);
                SplitK /= float.Parse(ss[1], CultureInfo.InvariantCulture);
            }
            Symbol = symbol;
            Date = date;
            Open = data[0] * SplitK;
            High = data[1] * SplitK;
            Low = data[2] * SplitK;
            Close = data[3]*SplitK;
            Volume = data[4]*SplitK;
        }
    }
}
