using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OlxFlat.Actions
{
    public static class Parse
    {
        //============  OLX  ============
        private static int olxItemCount = 0;
        public static void OlxParse(Action<string> showStatusAction)
        {
            var files = Directory.GetFiles(Settings.OlxFileListFolder, "*.txt");
            foreach (var file in files)
            {
                showStatusAction($"OLX parse: {file}");
                OlxParseFile(file);
            }
        }

        private static void OlxParseFile(string filename)
        {
            var s = File.ReadAllText(filename, Encoding.UTF8);
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
                var href = ParseString(s2, i1 + 4);
                i1 = s2.IndexOf("src=\"", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                var image = ParseString(s2, i1 + 3);
                i1 = s2.IndexOf("href=\"", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                var href2 = ParseString(s2, i1 + 4);
                i1 = s2.IndexOf("<strong>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                var i2 = s2.IndexOf("</strong>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                var name = s2.Substring(i1 + 8, i2 - i1 - 8);
                name = System.Web.HttpUtility.HtmlDecode(name);

                i1 = s2.IndexOf("class=\"price\"", i2 + 7, StringComparison.InvariantCultureIgnoreCase);
                i1 = s2.IndexOf("<strong>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                i2 = s2.IndexOf("</strong>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                var price = s2.Substring(i1 + 8, i2 - i1 - 8);

                var i11 = s2.IndexOf("location-filled", i2 + 7, StringComparison.InvariantCultureIgnoreCase);
                var dogovorna = s2.Substring(i1, i11 - i1).IndexOf("оговорная", StringComparison.InvariantCultureIgnoreCase) > 0;

                i1 = s2.IndexOf("</i>", i11 + 7, StringComparison.InvariantCultureIgnoreCase);
                i2 = s2.IndexOf("</span>", i1 + 4, StringComparison.InvariantCultureIgnoreCase);
                var location = s2.Substring(i1 + 4, i2 - i1 - 4);

                i1 = s2.IndexOf("clock", i2 + 7, StringComparison.InvariantCultureIgnoreCase);
                i1 = s2.IndexOf("</i>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                i2 = s2.IndexOf("</span>", i1 + 4, StringComparison.InvariantCultureIgnoreCase);
                var clock = s2.Substring(i1 + 4, i2 - i1 - 4);

                if (href != href2)
                    throw new Exception("Trap! Different href");

                Debug.Print($"Id: {olxItemCount}, {clock}, {location}, {promoted}, {id}, {href}, {image}, {name}, {price}, {dogovorna}");
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
