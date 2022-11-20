using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Quote2022.Models;

namespace Quote2022.Helpers
{
    public class YahooMinute
    {
        public static void Test()
        {
            var files = Directory.GetFiles(@"E:\Quote\WebData\Minute\", "*.txt", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                var o = JsonConvert.DeserializeObject<Models.MinuteYahoo>(content);
                o.Normilize();
            }
        }
  }
}
