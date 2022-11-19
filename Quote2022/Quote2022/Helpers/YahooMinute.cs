using System.IO;
using Newtonsoft.Json;

namespace Quote2022.Helpers
{
    public class YahooMinute
    {
        public static void Test()
        {
            var fn = @"E:\Quote\WebData\Minute\YahooMinute_20221119\yMin-AA.txt";
            var content = File.ReadAllText(fn);
            var o = JsonConvert.DeserializeObject<Models.YahooMinute>(content);

        }
    }
}
