using System;
using System.IO;
using System.IO.Compression;

namespace Data.Actions.TradingView
{
    public class ScreenerLoader
    {
        private const string parameters = @"{""filter"":[{""left"":""exchange"",""operation"":""in_range"",""right"":[""AMEX"",""NASDAQ"",""NYSE""]}],""options"":{""lang"":""en""},""markets"":[""america""],""symbols"":{""query"":{""types"":[]},""tickers"":[]},""columns"":[""minmov"",""name"",""close"",""change"",""change_abs"",""Recommend.All"",""volume"",""Value.Traded"",""market_cap_basic"",""price_earnings_ttm"",""earnings_per_share_basic_ttm"",""number_of_employees"",""sector"",""industry"",""description"",""type"",""subtype""],""sort"":{""sortBy"":""name"",""sortOrder"":""asc""},""range"":[0,20000]}";

        public static void Start(Action<string> showStatus)
        {
            showStatus($"TradingView.ScreenerLoader.Download started");
            // DownloadPage_POST(@"https://scanner.tradingview.com/america/scan", fileName, parameters);
            // showStatus($"TradingView.ScreenerLoader.Download finished. Filename{}");

            Download(showStatus);
        }

        private static void Download(Action<string> showStatus)
        {
            return;
            string zipName = "archive.zip";

            if (File.Exists(zipName))
            {
                File.Delete(zipName);
            }

            var files = Directory.GetFiles(@"data", "*.jpg");

            using (var archive = ZipFile.Open(zipName, ZipArchiveMode.Create))
            {

                foreach (var file in files)
                {
                    archive.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }
        }

        public static void XX()
        {
            /*var timeStamp = DateTime.Now.AddHours(4).AddDays(-1).Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        var fileName = string.Format(Settings.ScreenerTradingViewFileTemplate, timeStamp);

        Download.ScreenerTradingView_Download(ShowStatus, fileName);*/

            /*
             *         public static void ScreenerTradingView_Download(Action<string> showStatusAction, string fileName)
        {
            const string parameters1 = @"{""filter"":[{""left"":""type"",""operation"":""in_range"",""right"":[""stock"",""dr"",""fund""]},{""left"":""subtype"",""operation"":""in_range"",""right"":[""common"",""foreign-issuer"","""",""etf"",""etf,odd"",""etf,otc"",""etf,cfd""]},{""left"":""exchange"",""operation"":""in_range"",""right"":[""AMEX"",""NASDAQ"",""NYSE""]},{""left"":""is_primary"",""operation"":""equal"",""right"":true},{""left"":""active_symbol"",""operation"":""equal"",""right"":true}],""options"":{""lang"":""ru""},""markets"":[""america""],""symbols"":{""query"":{""types"":[]},""tickers"":[]},""columns"":[""logoid"",""name"",""close"",""change"",""change_abs"",""Recommend.All"",""volume"",""Value.Traded"",""market_cap_basic"",""price_earnings_ttm"",""earnings_per_share_basic_ttm"",""number_of_employees"",""sector"",""industry"",""description"",""type"",""subtype"",""update_mode"",""pricescale"",""minmov"",""fractional"",""minmove2"",""currency"",""fundamental_currency_code""],""sort"":{""sortBy"":""market_cap_basic"",""sortOrder"":""desc""},""range"":[0,20000]}";
            const string parameters = @"{""filter"":[{""left"":""exchange"",""operation"":""in_range"",""right"":[""AMEX"",""NASDAQ"",""NYSE""]}],""options"":{""lang"":""en""},""markets"":[""america""],""symbols"":{""query"":{""types"":[]},""tickers"":[]},""columns"":[""minmov"",""name"",""close"",""change"",""change_abs"",""Recommend.All"",""volume"",""Value.Traded"",""market_cap_basic"",""price_earnings_ttm"",""earnings_per_share_basic_ttm"",""number_of_employees"",""sector"",""industry"",""description"",""type"",""subtype""],""sort"":{""sortBy"":""name"",""sortOrder"":""asc""},""range"":[0,20000]}";

            showStatusAction($"ScreenerTradingView_Download started");
            DownloadPage_POST(@"https://scanner.tradingview.com/america/scan", fileName, parameters);
            showStatusAction($"ScreenerTradingView_Download FINISHED. File name: {fileName}");
        }


             */

        }
    }
}
