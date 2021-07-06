using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace OlxFlat.Helpers
{
    public static class Parse
    {
        private static CultureInfo _uaCulture = new CultureInfo("uk");
        private static CultureInfo _ruCulture = new CultureInfo("ru");
        //============  OLX  ============
        private static int olxItemCount = 0;
        private static Dictionary<string, int> _olxLocations = new Dictionary<string, int>();
        private static Dictionary<string, int> _olxClock = new Dictionary<string, int>();
        private static int _olxMax1 = 0;
        private static int _olxMax2 = 0;
        private static int _olxMax3 = 0;

        private static List<string> _prices = new List<string>();

        public static void OlxParse(Action<string> showStatusAction)
        {
            var files = Directory.GetFiles(Settings.OlxFileListFolder, "*.txt");
            foreach (var file in files)
            {
                showStatusAction($"OLX parse: {file}");
                OlxParseFile(file);
            }
            Debug.Print($"Stat: {_olxMax1}, {_olxMax2}, {_olxMax3}");
        }

        private static void OlxParseFile(string filename)
        {
            var s = File.ReadAllText(filename, Encoding.UTF8);
            var fileDate = File.GetLastWriteTime(filename).Date;
            var ss1 = s.Split(new[] { "<tr class=\"wrap\"" }, StringSplitOptions.None);
            for (var k2 = 1; k2 < ss1.Length; k2++)
            {
                var i1 = ss1[k2].IndexOf("</table>", StringComparison.InvariantCultureIgnoreCase);
                if (i1 < 1)
                    throw new Exception("Trap!!!");

                olxItemCount++;
                var s2 = ss1[k2].Substring(0, i1);
                var promoted = s2.IndexOf("offer promoted", StringComparison.InvariantCultureIgnoreCase) > 0;
                i1 = s2.IndexOf("data-id=\"", StringComparison.InvariantCultureIgnoreCase);

                if (i1 == -1)
                    throw new Exception("Trap!!!");

                var sid = ParseString(s2, i1 + 7);
                if (sid.Length !=9)
                    throw new Exception("Trap!!! Bad id");

                var id = int.Parse(sid);

                i1 = s2.IndexOf("href=\"", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                var href = ParseString(s2, i1 + 4).Trim();

                i1 = s2.IndexOf("src=\"", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                var image = ParseString(s2, i1 + 3).Trim();

                i1 = s2.IndexOf("href=\"", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                var href2 = ParseString(s2, i1 + 4).Trim();

                i1 = s2.IndexOf("<strong>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                var i2 = s2.IndexOf("</strong>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                var name = s2.Substring(i1 + 8, i2 - i1 - 8);
                name = System.Web.HttpUtility.HtmlDecode(name).Trim();

                i1 = s2.IndexOf("class=\"price\"", i2 + 7, StringComparison.InvariantCultureIgnoreCase);
                i1 = s2.IndexOf("<strong>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                i2 = s2.IndexOf("</strong>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                var sPrice = s2.Substring(i1 + 8, i2 - i1 - 8).Trim();
                if (!sPrice.EndsWith("$"))
                    throw new Exception("Trap! Price");
                if (sPrice.Contains("."))
                    _prices.Add(sPrice);
                var price = double.Parse(sPrice.Substring(0, sPrice.Length - 2).Trim().Replace(" ", ""), CultureInfo.InstalledUICulture);

                var i11 = s2.IndexOf("location-filled", i2 + 7, StringComparison.InvariantCultureIgnoreCase);
                var dogovorna = s2.Substring(i1, i11 - i1).IndexOf("оговорная", StringComparison.InvariantCultureIgnoreCase) > 0;

                i1 = s2.IndexOf("</i>", i11 + 7, StringComparison.InvariantCultureIgnoreCase);
                i2 = s2.IndexOf("</span>", i1 + 4, StringComparison.InvariantCultureIgnoreCase);
                var location = s2.Substring(i1 + 4, i2 - i1 - 4).Replace("Львов, ", "").Trim().Substring(0,5);

                i1 = s2.IndexOf("clock", i2 + 7, StringComparison.InvariantCultureIgnoreCase);
                i1 = s2.IndexOf("</i>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                i2 = s2.IndexOf("</span>", i1 + 4, StringComparison.InvariantCultureIgnoreCase);
                var clock = s2.Substring(i1 + 4, i2 - i1 - 4).Trim();
                var created = DateTime.Today;
                if (clock.StartsWith("Сегодня"))
                {
                    var a1 = TimeSpan.Parse(clock.Substring(7).Trim(), _uaCulture);
                    created = created.Add(a1);
                }
                else if (clock.StartsWith("Вчера"))
                {
                    var a1 = TimeSpan.Parse(clock.Substring(5).Trim(), _uaCulture);
                    created = created.Add(a1).AddDays(-1);
                }
                else
                {
                    var aa1 = clock.Split(' ');
                    if (aa1.Length != 3)
                        throw new Exception("Trap! Clocck");
                    created = DateTime.Parse(clock, _ruCulture);
                }

                if (href != href2)
                    throw new Exception("Trap! Different href");

                // Statistics
                if (!_olxLocations.ContainsKey(location))
                    _olxLocations.Add(location, 0);
                _olxLocations[location]++;

                if (!_olxClock.ContainsKey(clock))
                    _olxClock.Add(clock, 0);
                _olxClock[clock]++;

                if (href.Length > _olxMax1) _olxMax1 = href.Length;
                if (image.Length > _olxMax2) _olxMax2 = image.Length;
                if (name.Length > _olxMax3) _olxMax3 = name.Length;

                Debug.Print($"Id: {olxItemCount}, {created}, {location}, {promoted}, {id}, {href}, {image}, {name}, {price}, {dogovorna}");
            }
        }

        //==================
        private static string ParseString(string source, int startIndex)
        {
            var i1 = source.IndexOf('"', startIndex);
            var i2 = source.IndexOf('"', i1 + 1);
            var s = source.Substring(i1 + 1, i2 - i1 - 1);
            return s;
        }
    }
}
