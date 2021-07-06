namespace OlxFlat
{
    public static class Settings
    {
        internal const string OlxUrl = @"https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Border%5D=created_at%3Adesc&page={0}&currency=USD";

        internal const string OlxStartRangeUrl = @"https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Bfilter_float_price%3Ato%5D={1}&search%5Border%5D=created_at%3Adesc&page={0}&currency=USD";
        internal const string OlxEndRangeUrl = @"https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Bfilter_float_price%3Afrom%5D={1}&search%5Border%5D=created_at%3Adesc&page={0}&currency=USD";
        internal const string OlxRangeUrl = @"https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Bfilter_float_price%3Afrom%5D={1}&search%5Bfilter_float_price%3Ato%5D={2}&search%5Border%5D=created_at%3Adesc&page={0}&currency=USD";

        internal static int[] OlxPriceRange = {30, 35, 40, 45, 50, 55, 60, 70, 80, 100}; // Thousands $

        //https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Border%5D=created_at%3Adesc&search%5Bpaidads_listing%5D=1&view=list
        //https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Border%5D=created_at%3Adesc&search%5Bpaidads_listing%5D=1&view=list&currency=USD
        // https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Border%5D=created_at%3Adesc&search%5Bpaidads_listing%5D=1&view=list&currency=USD&page=2
        // https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Border%5D=created_at%3Adesc&page=25&currency=USD

        // Price range
        // https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Bfilter_float_price%3Afrom%5D=free&search%5Bfilter_float_price%3Ato%5D=30000&page=10&currency=USD
        // https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Bfilter_float_price%3Afrom%5D=40000&search%5Bfilter_float_price%3Ato%5D=45000&page=10&currency=USD
        // Sorted
        // https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Bfilter_float_price%3Afrom%5D=40000&search%5Bfilter_float_price%3Ato%5D=45000&search%5Border%5D=created_at%3Adesc&page=10&currency=USD
        // No from price
        // https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Bfilter_float_price%3Ato%5D=50000&search%5Border%5D=created_at%3Adesc&page=10&currency=USD
        // No to price
        // https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Bfilter_float_price%3Afrom%5D=100000&search%5Border%5D=created_at%3Adesc&page=11&currency=USD

        private const string OlxFileFolder = @"E:\Temp\flat_test\olx\";
        internal const string OlxFileListFolder = OlxFileFolder + @"list\";
        internal const string OlxFileListTemplate = OlxFileListFolder + @"olx_{0}.txt";

        internal const string OlxFileDetailsFolder = OlxFileFolder + @"details\";
        internal const string OlxFileDetailsTemplate = OlxFileFolder + @"details\olx_{0}.txt";
    }
}
