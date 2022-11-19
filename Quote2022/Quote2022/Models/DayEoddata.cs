using System;
using System.Globalization;

namespace Quote2022.Models
{
    public class DayEoddata
    {
        public string Symbol;
        public string Exchange;
        public DateTime Date;
        public float Open;
        public float High;
        public float Low;
        public float Close;
        public float Volume;

        public DayEoddata(string exchange, string[] ss)
        {
            Symbol = ss[0];
            Exchange = exchange.Trim().ToUpper();
            Date = DateTime.ParseExact(ss[1], "yyyyMMdd", CultureInfo.InvariantCulture);
            Open = float.Parse(ss[2].Trim(), CultureInfo.InvariantCulture);
            High = float.Parse(ss[3].Trim(), CultureInfo.InvariantCulture);
            Low = float.Parse(ss[4].Trim(), CultureInfo.InvariantCulture);
            Close = float.Parse(ss[5].Trim(), CultureInfo.InvariantCulture);
            Volume = float.Parse(ss[6].Trim(), CultureInfo.InvariantCulture);
        }
    }
}
