using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quote2022.Models
{
    public class SymbolsYahooLookup
    {
        public Finance finance;

        public class Finance
        {
            public Result[] result;
            public string error;
        }

        public class Result
        {
            public int start;
            public int count;
            public int total;
            public Document[] documents;
        }

        public class Document
        {
            public string Key => symbol + "^" + exchange;
            public string symbol; //:	AAIC
            public string shortName; //	:	Arlington Asset Investment Corp
            public string regularMarketPrice; //	:	2.83
            public string regularMarketChange; //	:	-0.03
            public string regularMarketPercentChange; //	:	-1.04895
            public string industryName; //	:	Real Estate
            public string quoteType; //	:	equity
            public string exchange; //	:	NYQ
            public string industryLink; //	:	https://finance.yahoo.com/sector/real_estate
            public int rank; //	:	20285
            public DateTime TimeStamp;
        }
    }
}
