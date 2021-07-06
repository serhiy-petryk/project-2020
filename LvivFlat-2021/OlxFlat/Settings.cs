namespace OlxFlat
{
    public static class Settings
    {
        internal const string OlxUrl = @"https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Border%5D=created_at%3Adesc&page={0}&currency=USD";

        //https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Border%5D=created_at%3Adesc&search%5Bpaidads_listing%5D=1&view=list
        //https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Border%5D=created_at%3Adesc&search%5Bpaidads_listing%5D=1&view=list&currency=USD
        // https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Border%5D=created_at%3Adesc&search%5Bpaidads_listing%5D=1&view=list&currency=USD&page=2
        // https://www.olx.ua/nedvizhimost/kvartiry-komnaty/prodazha-kvartir-komnat/lvov/?search%5Border%5D=created_at%3Adesc&page=25&currency=USD
        internal const string OlxFileFolder = @"E:\Temp\flat_test\olx\";
        internal const string OlxFileListTemplate = OlxFileFolder + @"list\olx_{0}.txt";
        internal const string OlxFileDetailsFolder = OlxFileFolder + @"details\";
        internal const string OlxFileDetailsTemplate = OlxFileFolder + @"details\olx_{0}.txt";
    }
}
