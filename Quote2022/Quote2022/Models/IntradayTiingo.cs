using System;

namespace Quote2022.Models
{
    public class IntradayTiingo
    {
        public cData data;

        public class cData
        {
            public cAsset asset;
            // public cPrice[] prices;
            public string prices;
            // dividends[], splits[]
        }
        public class cAsset
        {
            public DateTime startDate;
            public DateTime endDate;
        }
        public class cPrice
        {
            public long date;
            public float open;
            public float high;
            public float low;
            public float close;
            public float volume;
            // public string date;
            /*public string open;
            public string high;
            public string low;
            public string close;
            public string volume;*/
        }
    }
}
