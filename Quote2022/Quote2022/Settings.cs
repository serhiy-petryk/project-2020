﻿using System;
using System.Globalization;
using System.Text;

namespace Quote2022
{
    public static class Settings
    {
        public readonly static Encoding Encoding = Encoding.GetEncoding(1252);//Western European (Windows)
        public readonly static DateTimeFormatInfo fiDateInvariant = CultureInfo.InvariantCulture.DateTimeFormat;
        public readonly static NumberFormatInfo fiNumberInvariant = CultureInfo.InvariantCulture.NumberFormat;

        internal const string DbConnectionString = "Data Source=localhost;Initial Catalog=dbQuote2022;Integrated Security=True;Connect Timeout=150;";

        private const string BaseFolder = @"E:\Quote\";

        internal const string DayYahooFolder = BaseFolder + @"WebData\Daily\Yahoo\";
        internal const string DayYahooIndexesFolder = BaseFolder + @"WebData\Daily\Yahoo\Indexes\";
        internal const string SplitYahooFolder = BaseFolder + @"WebData\Splits\Yahoo\";
        internal const string SymbolsYahooLookupFolder = BaseFolder + @"WebData\Symbols\YahooLookup\";
        internal const string MinuteYahooFolder = BaseFolder + @"WebData\Minute\Yahoo\";
        internal const string MinuteYahooZipFilePattern = "YahooMinute_202?????.zip";
        internal const string MinuteYahooZipCacheFile = MinuteYahooFolder + "Cache.zip";
        internal const string MinuteYahooTextCacheFile = MinuteYahooFolder + "Cache.txt";
        internal const string MinuteYahooZipCacheEntry = "Cache.txt";

        internal const string SplitInvestingFolder = BaseFolder + @"WebData\Splits\Investing\";
        internal const string SplitInvestingHistoryFolder = BaseFolder + @"WebData\Splits\InvestingHistory\";

        internal const string DayEoddataFolder = BaseFolder + @"WebData\Daily\Eoddata";
        internal const string SymbolsEoddataFolder = BaseFolder + @"WebData\Symbols\Eoddata\";
        internal const string SplitEoddataFolder = BaseFolder + @"WebData\Splits\Eoddata\";

        internal const string SymbolsNanexTemplateUrl = @"http://www.nxcoreapi.com/symbols.php?m_exchange={0}&m_type={1}";
        internal const string SymbolsNanexTemplateFile =BaseFolder + @"WebData\Symbols\Nanex\{0}_{1}.txt";
        internal static string[] NanexExchanges = new string[] { "AMEX", "BATS", "NQNM", "NQSC", "NYSE", "PACF" };

        internal const string ScreenerNasdaqFolder = BaseFolder + @"WebData\StockScreener\Nasdaq\";
        internal const string SymbolsNasdaqFolder = BaseFolder + @"WebData\Symbols\Nasdaq\";
        internal const string TimeSalesNasdaqUrlTemplate = @"https://api.nasdaq.com/api/quote/{1}/realtime-trades?&limit=200000&fromTime={0}";
        internal const string TimeSalesNasdaqFolder = BaseFolder + @"WebData\Minute\Nasdaq\";
        internal const string TimeSalesNasdaqFileTemplate = BaseFolder + @"WebData\Minute\Nasdaq\TS_{2}\ts_{1}_{2}_{0}.json";

        internal const string StockSplitHistoryFolder = BaseFolder + @"WebData\Splits\StockSplitHistory\";

        internal const string ProfileQuantumonlineFolder = BaseFolder + @"WebData\Symbols\Quantumonline\Profiles\";

        internal const string ScreenerTradingViewFolder = BaseFolder + @"WebData\StockScreener\TradingView\";
        internal const string ScreenerTradingViewFileTemplate = ScreenerTradingViewFolder + @"TVScreener_{0}.json";

        internal static DateTime[] BadYahooIntradayDates = new[]
        {
            new DateTime(2022, 10, 28), new DateTime(2022, 11, 1), new DateTime(2022, 11, 2),
            new DateTime(2022, 11, 25), new DateTime(2023, 1, 3)
        };


    }
}
