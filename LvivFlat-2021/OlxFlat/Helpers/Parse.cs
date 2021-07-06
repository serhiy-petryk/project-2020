using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using OlxFlat.Models;

namespace OlxFlat.Helpers
{
    public static class Parse
    {
        //============  OLX  ============
        public static void OlxParse(Action<string> showStatusAction)
        {
            var items = new Dictionary<int, OlxList>();
            var files = Directory.GetFiles(Settings.OlxFileListFolder, "*.txt");
            foreach (var file in files)
            {
                showStatusAction($"OLX parse: {file}");
                OlxParseFile(file, items);
            }

            Debug.Print($"OlxList items: {items.Count}");
            Debug.Print($"Stat: {OlxList._olxMax1}, {OlxList._olxMax2}, {OlxList._olxMax3}");

            showStatusAction($"OLXList: SaveToDb");
            SaveToDb.OlxListSave(items);

            showStatusAction($"OLXList: finished");
        }

        private static void OlxParseFile(string filename, Dictionary<int, OlxList> items)
        {
            var s = File.ReadAllText(filename, Encoding.UTF8);
            var fileDate = File.GetLastWriteTime(filename).Date;
            var ss1 = s.Split(new[] { "<tr class=\"wrap\"" }, StringSplitOptions.None);
            for (var k2 = 1; k2 < ss1.Length; k2++)
            {
                var item = new OlxList(ss1[k2], fileDate);
                if (items.ContainsKey(item.Id))
                    item.CheckEquality(items[item.Id]);
                else
                    items.Add(item.Id, item);
            }
        }
    }
}
