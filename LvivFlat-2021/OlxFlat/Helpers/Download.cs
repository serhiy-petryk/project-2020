﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using OlxFlat.Models;

namespace OlxFlat.Helpers
{
    public static class Download
    {
        #region ===============  RealEstate list  ====================
        public static void RealEstate_Download(Action<string> showStatusAction)
        {
            showStatusAction("RealEstate: Delete files");
            var files = Directory.GetFiles(Settings.RealEstateList_FileFolder, "*.txt");
            foreach (var fn in files)
                File.Delete(fn);

            var url = string.Format(Settings.RealEstateList_TemplateUrl, 1);
            var filename = string.Format(Settings.RealEstateList_FileTemplate, 1);
            var content = DownloadPage(url, filename);
            // var content = File.ReadAllText(filename);
            var i1 = content.IndexOf("\"last page-item\"", StringComparison.InvariantCultureIgnoreCase);
            var i2 = content.IndexOf("</", i1, StringComparison.InvariantCultureIgnoreCase);
            var i3 = content.LastIndexOf(">", i2, StringComparison.InvariantCultureIgnoreCase);
            var sLastPage = content.Substring(i3 + 1, i2 - i3 - 1);
            var lastPage = int.Parse(sLastPage.Trim());

            for (var k = 2; k <= lastPage; k++)
            {
                if ((lastPage - k) % 10 == 0)
                    showStatusAction($"RealEstate list download. Remain {lastPage - k} pages");

                url = string.Format(Settings.RealEstateList_TemplateUrl, k);
                filename = string.Format(Settings.RealEstateList_FileTemplate, k);
                DownloadPage(url, filename);
            }

            showStatusAction($"RealEstate: Downloaded");
        }
        #endregion

        #region ===============  DomRia list  ====================
        public static void DomRia_Download(Action<string> showStatusAction)
        {
            // var data = new Dictionary<int, object> { { 19895499, null } };
            DomRiaList_Download(showStatusAction);
            DomRiaDetails_Download(showStatusAction);
        }

        public static void DomRiaList_Download(Action<string> showStatusAction)
        {
            showStatusAction("Dom.Ria.List: Delete files");

            var files = Directory.GetFiles(Settings.DomRiaListFileFolder, "*.txt");
            foreach (var fn in files)
                File.Delete(fn);

            var itemCount = int.MaxValue;
            var ids = new List<int>();
            var k = 0;
            while (ids.Count < itemCount)
            {
                var url = string.Format(Settings.DomRiaListTemplateUrl, k);
                var filename = string.Format(Settings.DomRiaListFileTemplate, k);
                var content = DownloadPage(url, filename);
                var o = JsonConvert.DeserializeObject<DomRiaList>(content);

                itemCount = o.Count;
                ids.AddRange(o.Items);
                k++;
                if (k > 100)
                    throw new Exception("Check DomRiaList_Download.");

                showStatusAction($"Downloaded {ids.Count} DomRia list ids");
            }

            showStatusAction($"Dom.Ria.List: Downloaded");
        }
        #endregion

        public static void DomRiaDetails_Download(Action<string> showStatusAction)
        {
            showStatusAction("Dom.Ria.Details: Get id list of item to download");
            var files = Directory.GetFiles(Settings.DomRiaListFileFolder, "*.txt");
            var ids = new Dictionary<int, object>();
            foreach (var fn in files)
            {
                var content = File.ReadAllText(fn);
                var o = JsonConvert.DeserializeObject<DomRiaList>(content);
                foreach(var item in o.Items)
                    ids.Add(item, null);
            }

            foreach (var item in SaveToDb.DomRia_GetExistingIds())
            {
                if (ids.ContainsKey(item.Key))
                    ids.Remove(item.Key);
            }

            showStatusAction("Dom.Ria.Details: Start download");
            var cnt = ids.Count;
            foreach (var id in ids.Keys)
            {
                var url = string.Format(Settings.DomRiaDetailsTemplateUrl, id);
                var filename = string.Format(Settings.DomRiaDetailsFileTemplate, id);
                showStatusAction($"Download Dom.Ria details. Remain {cnt--} items. {id}");
                DownloadPage(url, filename);
            }
            showStatusAction($"Dom.Ria.Details: Downloaded");
        }

        #region ===============  OLX details  ====================

        public static void OlxDetails_Download(Action<string> showStatusAction)
        {
            var source = new List<(int, string)>();

            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from vOlxDetails_NewToDownload";
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            source.Add(((int)rdr["id"], (string)rdr["href"]));
                }
            }

            var cnt = source.Count;
            foreach (var item in source)
            {
                cnt--;
                showStatusAction($"Download Olx details. Remain {cnt} files. {item.Item1}");
                var filename = string.Format(Settings.OlxFileDetailsTemplate, item.Item1);
                DownloadPage(item.Item2, filename);
            }

            showStatusAction($"Download Olx details finished.");
        }

        #endregion

        #region ===============  OLX list  ====================
        public static void OlxList_Download(Action<string> showStatusAction)
        {
            showStatusAction("OLX: Delete files");
            var files = Directory.GetFiles(Settings.OlxFileListFolder, "*.txt");
            foreach (var fn in files)
                File.Delete(fn);

            OlxList_Download(null, Settings.OlxPriceRange[0], $"OlxList. Price less than {Settings.OlxPriceRange[0]}'000$", showStatusAction);

            for (var k = 1; k < Settings.OlxPriceRange.Length; k++)
                OlxList_Download(Settings.OlxPriceRange[k - 1], Settings.OlxPriceRange[k],
                    $"OlxList. Prices: {Settings.OlxPriceRange[k - 1]}-{Settings.OlxPriceRange[k]} ('000$)",
                    showStatusAction);

            OlxList_Download(Settings.OlxPriceRange[Settings.OlxPriceRange.Length - 1], null, $"OlxList. Price greater than {Settings.OlxPriceRange[Settings.OlxPriceRange.Length - 1]}'000$", showStatusAction);

            showStatusAction($"OLX: Downloaded");
        }

        private static void OlxList_Download(int? startPrice, int? endPrice, string messagePrefix, Action<string> showStatusAction)
        {
            if (!startPrice.HasValue)
            {
                var url = string.Format(Settings.OlxStartRangeUrl, 1, endPrice.Value * 1000);
                var fileId = endPrice + "_1";
                var filename = string.Format(Settings.OlxFileListTemplate, fileId);
                showStatusAction(messagePrefix + ": load first page");
                var fileContent = DownloadPage(url, filename);
                var pages = OlxList_GetPages(fileContent);
                for (var k = 1; k < pages; k++)
                {
                    url = string.Format(Settings.OlxStartRangeUrl, k + 1, endPrice.Value * 1000);
                    fileId = endPrice + "_" + (k + 1);
                    filename = string.Format(Settings.OlxFileListTemplate, fileId);
                    showStatusAction(messagePrefix + $": remain {pages - k} pages");
                    DownloadPage(url, filename);
                }
            }
            else if (!endPrice.HasValue)
            {
                var url = string.Format(Settings.OlxEndRangeUrl, 1, startPrice.Value * 1000);
                var fileId = startPrice + "_1";
                var filename = string.Format(Settings.OlxFileListTemplate, fileId);
                showStatusAction(messagePrefix + ": load first page");
                var fileContent = DownloadPage(url, filename);
                var pages = OlxList_GetPages(fileContent);
                for (var k = 1; k < pages; k++)
                {
                    url = string.Format(Settings.OlxEndRangeUrl, k + 1, startPrice.Value * 1000);
                    fileId = startPrice + "_" + (k + 1);
                    filename = string.Format(Settings.OlxFileListTemplate, fileId);
                    showStatusAction(messagePrefix + $": remain {pages - k} pages");
                    DownloadPage(url, filename);
                }
            }
            else
            {
                var url = string.Format(Settings.OlxRangeUrl, 1, startPrice.Value * 1000, endPrice.Value * 1000);
                var fileId = startPrice + "_" + endPrice + "_1";
                var filename = string.Format(Settings.OlxFileListTemplate, fileId);
                showStatusAction(messagePrefix + ": load first page");
                var fileContent = DownloadPage(url, filename);
                var pages = OlxList_GetPages(fileContent);
                for (var k = 1; k < pages; k++)
                {
                    url = string.Format(Settings.OlxRangeUrl, k + 1, startPrice.Value * 1000, endPrice.Value * 1000);
                    fileId = startPrice + "_" + endPrice + "_" + (k + 1);
                    filename = string.Format(Settings.OlxFileListTemplate, fileId);
                    showStatusAction(messagePrefix + $": remain {pages - k} pages");
                    DownloadPage(url, filename);
                }
            }
        }

        private static int OlxList_GetPages(string fileContent)
        {
            var i5 = fileContent.LastIndexOf("item fleft", StringComparison.InvariantCultureIgnoreCase);
            i5 = fileContent.IndexOf("<span>", i5 + 7, StringComparison.InvariantCultureIgnoreCase);
            var i6 = fileContent.IndexOf("</span>", i5 + 4, StringComparison.InvariantCultureIgnoreCase);
            var pages = fileContent.Substring(i5 + 6, i6 - i5 - 6).Trim();
            return int.Parse(pages);
        }
        #endregion

        //===============================
        private static string DownloadPage(string url, string filename)
        {
            using (var wc = new WebClient())
            {
                byte[] bb = wc.DownloadData(url);
                string s = Encoding.UTF8.GetString(bb);
                if (File.Exists(filename))
                    File.Delete(filename);
                if (!string.IsNullOrEmpty(filename))
                    File.WriteAllText(filename, s, Encoding.UTF8);
                return s;
            }
        }
    }
}
