using System;

namespace Quote2022.Models
{
    public class ScreenerTradingView
    {
        public int totalCount;
        public Item[] data;

        public class Item
        {
            public string s;
            public object[] d;

            public DbItem GetDbItem(DateTime timeStamp)
            {
                var ss = s.Split(':');
                if (!string.Equals(ss[1], (string)d[1]))
                    throw new Exception("Check 'Symbol' in ScreenerTradingView.Item.");
                if (ss[1] == "AAA")
                {

                }

                var item = new DbItem
                {
                    Exchange = ss[0], Symbol = ss[1], Name = CheckNull((string) d[14]),
                    Type = CheckNull((string) d[15]), Subtype = CheckNull((string) d[16]),
                    Sector = CheckNull((string) d[12]), Industry = CheckNull((string) d[13]),
                    Close = Convert.ToSingle(d[2]), Volume = Convert.ToInt64(d[6]),
                    MarketCap = d[8] == null ? (long?) null : Convert.ToInt64(d[8]), Recommend = Convert.ToSingle(d[5]),
                    TimeStamp = timeStamp
                };
                return item;

                string CheckNull(string s) => string.IsNullOrEmpty(s) ? null : s;
            }
        }

        public class DbItem
        {
            public string Exchange;
            public string Symbol;
            public string Name;
            public string Type;
            public string Subtype;
            public string Sector;
            public string Industry;
            public float Close;
            public long Volume;
            public long? MarketCap;
            public float Recommend;
            public DateTime TimeStamp;
        }

    }
}
