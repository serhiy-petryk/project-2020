using OlxFlat.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace OlxFlat.Models
{
    public class OlxDetails
    {
        public static Dictionary<string, int> _param = new Dictionary<string, int>();

        private static CultureInfo _ruCulture = new CultureInfo("ru");

        public string Name;
        public int Price;
        public DateTime Dated;
        public bool Dogovirna;
        public List<string> ImageRefs = new List<string>();
        public List<string> Parameters = new List<string>();

        public OlxDetails(string filename)
        {
            var s = File.ReadAllText(filename, Encoding.UTF8);
            var fileDate = File.GetLastWriteTime(filename).Date;

            var ss1 = s.Split(new[] { "\"adPhotos-swiperSlide\"" }, StringSplitOptions.None);
            for (var k1 = 1; k1 < ss1.Length; k1++)
            {
                var a1 = ss1[k1].IndexOf("img data-src=", StringComparison.InvariantCultureIgnoreCase);
                if (a1 == -1)
                    a1 = ss1[k1].IndexOf("img src=", StringComparison.InvariantCultureIgnoreCase);
                if (a1 == -1)
                        throw new Exception("Trap! OlxDetails. Image");
                var imageRef = Parse.ParseString_Quotes(ss1[k1], a1 + 7);
                if (!imageRef.Contains("image;s="))
                    throw new Exception("Trap! OlxDetails. Image");
                ImageRefs.Add(imageRef.Trim());
            }

            var s1 = ss1[ss1.Length - 1];
            var i1 = s1.IndexOf("ad-posted-at", StringComparison.InvariantCultureIgnoreCase);
            var sDated = Parse.ParseString_Braces(s1, i1 + 7);//s1.Substring(i2 + 1, i3 - i2 - 1);
            Dated = DateTime.Parse(sDated, _ruCulture);

            i1 = s1.IndexOf("\"ad_title\"", i1, StringComparison.InvariantCultureIgnoreCase);
            Name = Parse.ParseString_Braces(s1, i1+7);

            i1 = s1.IndexOf("\"ad-price-container\"", i1, StringComparison.InvariantCultureIgnoreCase);
            i1 = s1.IndexOf("<h3 ", i1, StringComparison.InvariantCultureIgnoreCase);
            var s2 = Parse.ParseString_Braces(s1, i1 + 7);
            var sPrice = s2.Replace("<!-- -->", "").Replace(" ", "").Trim();

            var rate = 1.0;
            if (sPrice.EndsWith("грн."))
            {
                sPrice = sPrice.Replace("грн.", "");
                rate = Settings._usRate;
            }
            else if (sPrice.EndsWith("$"))
                sPrice = sPrice.Replace("$", "");
            else
                throw new Exception("Trap! Price");
            var price = double.Parse(sPrice, CultureInfo.InvariantCulture) / rate;
            Price = Convert.ToInt32(price);

            var i2 = s1.IndexOf("\"negotiable-label\"", i1, StringComparison.InvariantCultureIgnoreCase);
            Dogovirna = i2 > 0;

            //css-xl6fe0-Text eu5v0x0
            i1 = s1.IndexOf("\"css-xl6fe0-Text eu5v0x0\"", i1, StringComparison.InvariantCultureIgnoreCase);
            while (i1 > 0)
            {
                Parameters.Add(Parse.ParseString_Braces(s1, i1).Trim());
                i1 = s1.IndexOf("\"css-xl6fe0-Text eu5v0x0\"", i1 + 10, StringComparison.InvariantCultureIgnoreCase);
            }

            foreach (var p in Parameters)
            {
                var ss2 = p.Split(':');
                var a1 = ss2.Length == 1 ? "_" + ss2[0].Trim() : ss2[0];
                if (!_param.ContainsKey(a1))
                    _param.Add(a1,0);
                _param[a1]++;
            }
        }
    }
}
