namespace OlxFlat
{
    public static class Settings
    {
        internal const double _usRate = 27.3;
        internal const double _euroRate = 1.18;

        internal const string OlxStartRangeUrl = @"https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Bfilter_float_price%3Ato%5D={1}&search%5Border%5D=created_at%3Adesc&page={0}&currency=USD";
        internal const string OlxEndRangeUrl = @"https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Bfilter_float_price%3Afrom%5D={1}&search%5Border%5D=created_at%3Adesc&page={0}&currency=USD";
        internal const string OlxRangeUrl = @"https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Bfilter_float_price%3Afrom%5D={1}&search%5Bfilter_float_price%3Ato%5D={2}&search%5Border%5D=created_at%3Adesc&page={0}&currency=USD";

        internal static int[] OlxPriceRange = {30, 35, 40, 45, 50, 55, 60, 70, 80, 100}; // Thousands $

        private const string OlxFileFolder = @"E:\Temp\flat_test\olx\";
        internal const string OlxFileListFolder = OlxFileFolder + @"list\";
        internal const string OlxFileListTemplate = OlxFileListFolder + @"olx_{0}.txt";

        internal const string OlxFileDetailsFolder = OlxFileFolder + @"details\";
        internal const string OlxFileDetailsTemplate = OlxFileDetailsFolder + @"olx_details_{0}.txt";

        // Dom.RIA
        private const string DomRiaFileFolder = @"E:\Temp\flat_test\dom.ria\";
        internal const string DomRiaListFileFolder = DomRiaFileFolder + @"list\";
        internal const string DomRiaListFileTemplate = DomRiaListFileFolder + @"dom.ria.list_{0}.txt";
        internal const string DomRiaDetailsFileFolder = DomRiaFileFolder + @"details\";
        internal const string DomRiaDetailsFileTemplate = DomRiaDetailsFileFolder + @"dom.ria.details_{0}.txt";
        internal const string DomRiaApiCharacteristicsFile = DomRiaFileFolder + @"api\options_1_2_1.json";

        internal const string DomRiaListTemplateUrl = @"https://dom.ria.com/node/searchEngine/v2/?links-under-filter=on&category=1&realty_type=2&operation_type=1&fullCategoryOperation=1_2_1&wo_dupl=1&page={0}&state_id=5&city_id=5&limit=1000&sort=p_a&period=0";
        internal const string DomRiaDetailsTemplateUrl = @"https://dom.ria.com/node/searchEngine/v2/view/realty/{0}?lang_id=4";

        // Real-estate
        private const string RealEstate_FileFolder = @"E:\Temp\flat_test\RealEstate\";
        internal const string RealEstateList_FileFolder = RealEstate_FileFolder + @"list\";
        internal const string RealEstateList_FileTemplate = RealEstateList_FileFolder + @"RealEstate_{0}.txt";

        internal const string RealEstateDetails_FileFolder = RealEstate_FileFolder + @"details\";
        internal const string RealEstateDetails_FileTemplate = RealEstateDetails_FileFolder + @"realestate_details_{0}.txt";

        // https://www.real-estate.lviv.ua/%D0%BF%D1%80%D0%BE%D0%B4%D0%B0%D0%B6-%D0%BA%D0%B2%D0%B0%D1%80%D1%82%D0%B8%D1%80/%D0%BC%D1%96%D1%81%D1%82%D0%BE-%D0%BB%D1%8C%D0%B2%D1%96%D0%B2/sort-date_insd/p-5
        internal const string RealEstateList_TemplateUrl = @"https://www.real-estate.lviv.ua/%D0%BF%D1%80%D0%BE%D0%B4%D0%B0%D0%B6-%D0%BA%D0%B2%D0%B0%D1%80%D1%82%D0%B8%D1%80/%D0%BC%D1%96%D1%81%D1%82%D0%BE-%D0%BB%D1%8C%D0%B2%D1%96%D0%B2/sort-date_insd/p-{0}";

        //===================
        internal const string DbConnectionString = "Data Source=localhost;Initial Catalog=dbLvivFlat2021;Integrated Security=True";

    }
}
