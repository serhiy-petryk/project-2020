﻿using System;
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
        #region ================  OLX details  ====================

        public static void OlxDetails_Parse(Action<string> showStatusAction)
        {
            var items = new List<OlxDetails>();
            var files = Directory.GetFiles(Settings.OlxFileDetailsFolder, "*.txt");
            foreach (var file in files)
            {
                showStatusAction($"OLX details parse: {file}");
                items.Add(new OlxDetails(file));
            }

            var a1 = OlxDetails._param;
            Debug.Print($"OlxDetails items: {items.Count}");
            // Debug.Print($"Stat: {OlxList._olxMax1}, {OlxList._olxMax2}, {OlxList._olxMax3}");

            // showStatusAction($"OLXDetails: SaveToDb");
            // SaveToDb.OlxDetails_Save(items);

            showStatusAction($"OLXDetails: finished");
        }
        #endregion

        #region ================  OLX list  ====================
        public static void OlxList_Parse(Action<string> showStatusAction)
        {
            var items = new Dictionary<int, OlxList>();
            var files = Directory.GetFiles(Settings.OlxFileListFolder, "*.txt");
            foreach (var file in files)
            {
                showStatusAction($"OLX parse: {file}");
                OlxList_ParseFile(file, items);
            }

            Debug.Print($"OlxList items: {items.Count}");
            Debug.Print($"Stat: {OlxList._olxMax1}, {OlxList._olxMax2}, {OlxList._olxMax3}");

            showStatusAction($"OLXList: SaveToDb");
            SaveToDb.OlxList_Save(items);

            showStatusAction($"OLXList: finished");
        }

        private static void OlxList_ParseFile(string filename, Dictionary<int, OlxList> items)
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
        #endregion

        public static string ParseString_Quotes(string source, int startIndex)
        {
            var i1 = source.IndexOf('"', startIndex);
            var i2 = source.IndexOf('"', i1 + 1);
            var s = source.Substring(i1 + 1, i2 - i1 - 1);
            return s;
        }

        public static string ParseString_Braces(string source, int startIndex)
        {
            var i1 = source.IndexOf('>', startIndex);
            var i2 = source.IndexOf("</", i1 + 1);
            var s = source.Substring(i1 + 1, i2 - i1 - 1);
            return s;
        }

    }
}
