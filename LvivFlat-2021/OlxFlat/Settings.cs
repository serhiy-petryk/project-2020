namespace OlxFlat
{
    public static class Settings
    {
        internal const double _usRate = 27.3;
        internal const double _euroRate = 1.18;

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
        internal const string OlxFileDetailsTemplate = OlxFileDetailsFolder + @"olx_details_{0}.txt";

        // Dom.RIA
        private const string DomRiaFileFolder = @"E:\Temp\flat_test\dom.ria\";
        internal const string DomRiaFileListFolder = DomRiaFileFolder + @"list\";
        internal const string DomRiaFileListTemplate = DomRiaFileListFolder + @"dom.ria_{0}.txt";

        internal const string DomRiaUrlTemplateUrl = @"https://dom.ria.com/node/searchEngine/v2/?links-under-filter=on&category=1&realty_type=2&operation_type=1&fullCategoryOperation=1_2_1&wo_dupl=1&page=1&state_id=5&city_id=5&limit=100&sort=p_a&period=0";
        internal const string DomRiaUrlTemplateUrX = @"https://dom.ria.com/node/searchEngine/v2/?links-under-filter=on&category=1&realty_type=2&operation_type=1&fullCategoryOperation=1_2_1&wo_dupl=1&page=1&state_id=5&city_id=5&limit=100&sort=p_a&period=0";


        //===================
        internal const string DbConnectionString = "Data Source=localhost;Initial Catalog=dbLvivFlat2021;Integrated Security=True";

    }
}
