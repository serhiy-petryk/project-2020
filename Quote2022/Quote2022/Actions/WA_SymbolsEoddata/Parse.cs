using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote2022.Actions.WA_SymbolsEoddata
{
    public static class Parse
    {
        public class Item
        {
            public string Symbol;
            public string Exchange;
            public DateTime TimeStamp;
            public string Name;
            public float High;
            public float Low;
            public float Close;
            public float Change;
            public float Volume;
        }

        public static void ParseData(string folder, Action<string> showStatusAction)
        {
            var items = new List<Item>();
            Actions.SaveToDb.ClearAndSaveToDbTable(items, "dbQuote2023..HSymbolsEoddata", "Symbol", "Exchange",
                "Timestamp", "High", "Low", "Close", "Changed", "Volume");

            var files = Directory.GetFiles(folder, "*.htm", SearchOption.AllDirectories);
            var cnt = 0;
            foreach (var file in files)
            {
                cnt++;
                if (cnt % 10 == 0)
                    showStatusAction($"WA_SymbolsEoddata.ParseData. Processed {cnt} files from {files.Length}");

                var ss1 = Path.GetFileNameWithoutExtension(file).Split('_');
                var exchange = ss1[0];
                var timeStamp = DateTime.ParseExact(ss1[2], "yyyyMMdd", CultureInfo.InvariantCulture);

                var s = File.ReadAllText(file);
                var i1 = s.IndexOf("class=\"quotes\"", StringComparison.InvariantCultureIgnoreCase);
                var i2 = s.IndexOf("</table>", i1 + 14, StringComparison.InvariantCultureIgnoreCase);
                var s1 = s.Substring(i1 + 14, i2 - i1 - 14);
                ss1 = s1.Split(new[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                for (var k = 1; k < ss1.Length; k++)
                {
                    if (string.IsNullOrWhiteSpace(ss1[k])) continue;

                    var ss2 = ss1[k].Split(new[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                    var symbol = GetValue(ss2[0].Replace("</a>", ""));
                    var name = GetValue(ss2[1]);
                    var high = float.Parse(GetValue(ss2[2]), CultureInfo.InvariantCulture);
                    var low = float.Parse(GetValue(ss2[3]), CultureInfo.InvariantCulture);
                    var close = float.Parse(GetValue(ss2[4]), CultureInfo.InvariantCulture);
                    var change = float.Parse(GetValue(ss2[6]), CultureInfo.InvariantCulture);
                    var volume = float.Parse(GetValue(ss2[5]), CultureInfo.InvariantCulture);
                    items.Add(new Item
                    {
                        Symbol = symbol, Exchange = exchange, TimeStamp = timeStamp, Name = name, High = high,
                        Low = low, Close = close, Change = change, Volume = volume
                    });
                }

                if (items.Count > 100000)
                {
                    showStatusAction($"WA_SymbolsEoddata.ParseData. Saving data to database ...");
                    Actions.SaveToDb.SaveToDbTable(items, "dbQuote2023..HSymbolsEoddata", "Symbol", "Exchange",
                        "TimeStamp", "Name", "High", "Low", "Close", "Change", "Volume");
                    items.Clear();
                }

                string GetValue(string cell)
                {
                    var k1 = cell.LastIndexOf(">", StringComparison.InvariantCultureIgnoreCase);
                    var a1 = cell.Substring(k1 + 1);
                    return a1;
                }
            }

            if (items.Count > 100000)
            {
                showStatusAction($"WA_SymbolsEoddata.ParseData. Saving data to database ...");
                Actions.SaveToDb.SaveToDbTable(items, "dbQuote2023..HSymbolsEoddata", "Symbol", "Exchange",
                    "Timestamp", "High", "Low", "Close", "Changed", "Volume");
                items.Clear();
            }

            showStatusAction($"WA_SymbolsEoddata.ParseData finished");
        }
    }
}
