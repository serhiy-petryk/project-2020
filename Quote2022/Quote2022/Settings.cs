using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Quote2022
{
    public static class Settings
    {
        public readonly static Encoding Encoding = Encoding.GetEncoding(1252);//Western European (Windows)
        public readonly static DateTimeFormatInfo fiDateInvariant = CultureInfo.InvariantCulture.DateTimeFormat;
        public readonly static NumberFormatInfo fiNumberInvariant = CultureInfo.InvariantCulture.NumberFormat;

        internal const string DbConnectionString = "Data Source=localhost;Initial Catalog=dbQuote2022;Integrated Security=True;Connect Timeout=150;";

        private const string BaseFolder = @"E:\Quote\";

        internal const string DayYahooFolder = BaseFolder + @"WebData\Daily\Yahoo\Price_20221014\";
        internal const string DayYahooIndexesFolder = BaseFolder + @"WebData\Daily\Yahoo\Indexes\";
        internal const string SplitYahooFolder = BaseFolder + @"WebData\Splits\Yahoo\";

        internal const string SplitInvestingFolder = BaseFolder + @"WebData\Splits\Investing\";

        internal const string DayEoddataFolder = BaseFolder + @"WebData\Daily\Eoddata";
        internal const string SymbolsEoddataFolder = BaseFolder + @"WebData\Symbols\Eoddata\";
        internal const string SplitEoddataFolder = BaseFolder + @"WebData\Splits\Eoddata\";

        internal const string DayTickertechFolder = @"E:\Temp\Quote\Daily\Tickertech\";

        internal const string SymbolsNanexTemplateUrl = @"http://www.nxcoreapi.com/symbols.php?m_exchange={0}&m_type={1}";
        internal const string SymbolsNanexTemplateFile =BaseFolder + @"WebData\Symbols\Nanex\{0}_{1}.txt";
        internal static string[] NanexExchanges = new string[] { "AMEX", "BATS", "NQNM", "NQSC", "NYSE", "PACF" };

        internal const string SymbolsTickertechFolder = BaseFolder + @"E:\Temp\Quote\Symbols\Tickertech\";

    }
}
