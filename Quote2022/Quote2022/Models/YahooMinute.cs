using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote2022.Models
{
    public class YahooMinute
    {
        public cChart Chart { get; set; }

        public class cChart
        {
            public cResult[] Result { get; set; }
        }
        public class cResult
        {
            public cMeta Meta { get; set; }
        }

        public class cMeta
        {
            public string Currency { get; set; }
            public string Symbol { get; set; }
            public string ExchangeName { get; set; }
        }
    }
}
