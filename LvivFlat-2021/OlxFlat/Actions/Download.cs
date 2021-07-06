using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace OlxFlat.Actions
{
    public static class Download
    {
        //============  OLX  ============
        public static void OlxDownload(Action<string> showStatusAction)
        {

            showStatusAction("OLX: Delete files");
            var files = Directory.GetFiles(Settings.OlxFileListFolder, "*.txt");
            foreach(var fn in files)
                File.Delete(fn);

            showStatusAction($"OLX: Download start range to {Settings.OlxPriceRange[0]}");
            OlxDownload(null, Settings.OlxPriceRange[0]);

            for (var k = 1; k < Settings.OlxPriceRange.Length; k++)
            {
                showStatusAction($"OLX: Download range from {Settings.OlxPriceRange[k - 1]} to {Settings.OlxPriceRange[k]}");
                OlxDownload(Settings.OlxPriceRange[k - 1], Settings.OlxPriceRange[k]);
            }

            showStatusAction($"OLX: Download end range from {Settings.OlxPriceRange[Settings.OlxPriceRange.Length - 1]}");
            OlxDownload(Settings.OlxPriceRange[Settings.OlxPriceRange.Length - 1], null);

            showStatusAction($"OLX: Downloaded");
        }

        private static void OlxDownload(int? startPrice, int? endPrice)
        {
            if (!startPrice.HasValue)
            {
                var url = string.Format(Settings.OlxStartRangeUrl, 1, endPrice.Value * 1000);
                var fileId = endPrice + "_1";
                var filename = string.Format(Settings.OlxFileListTemplate, fileId);
                var fileContent = DownloadPage(url, filename);
                var pages = GetOlxPages(fileContent);
                for (var k = 1; k < pages; k++)
                {
                    url = string.Format(Settings.OlxStartRangeUrl, k + 1, endPrice.Value * 1000);
                    fileId = endPrice + "_" + (k+1);
                    filename = string.Format(Settings.OlxFileListTemplate, fileId);
                    DownloadPage(url, filename);
                }
                Debug.Print($"OLX: {filename}, {url}");
            }
            else if (!endPrice.HasValue)
            {
                var url = string.Format(Settings.OlxEndRangeUrl, 1, startPrice.Value * 1000);
                var fileId = startPrice + "_1";
                var filename = string.Format(Settings.OlxFileListTemplate, fileId);
                var fileContent = DownloadPage(url, filename);
                var pages = GetOlxPages(fileContent);
                for (var k = 1; k < pages; k++)
                {
                    url = string.Format(Settings.OlxEndRangeUrl, k + 1, startPrice.Value * 1000);
                    fileId = startPrice + "_" + (k + 1);
                    filename = string.Format(Settings.OlxFileListTemplate, fileId);
                    DownloadPage(url, filename);
                }
                Debug.Print($"OLX: {filename}, {url}");
            }
            else
            {
                var url = string.Format(Settings.OlxRangeUrl, 1, startPrice.Value * 1000, endPrice.Value * 1000);
                var fileId = startPrice + "_" + endPrice + "_1" ;
                var filename = string.Format(Settings.OlxFileListTemplate, fileId);
                var fileContent = DownloadPage(url, filename);
                var pages = GetOlxPages(fileContent);
                for (var k = 1; k < pages; k++)
                {
                    url = string.Format(Settings.OlxRangeUrl, k+1, startPrice.Value * 1000, endPrice.Value * 1000);
                    fileId = startPrice + "_" + endPrice + "_" + (k+1);
                    filename = string.Format(Settings.OlxFileListTemplate, fileId);
                    DownloadPage(url, filename);
                }
                Debug.Print($"OLX: {filename}, {url}");
            }
        }

        private static int GetOlxPages(string fileContent)
        {
            var i5 = fileContent.LastIndexOf("item fleft", StringComparison.InvariantCultureIgnoreCase);
            i5 = fileContent.IndexOf("<span>", i5 + 7, StringComparison.InvariantCultureIgnoreCase);
            var i6 = fileContent.IndexOf("</span>", i5 + 4, StringComparison.InvariantCultureIgnoreCase);
            var pages = fileContent.Substring(i5 + 6, i6 - i5 - 6).Trim();
            return int.Parse(pages);
        }
        //===============================
        private static string DownloadPage(string url, string filename)
        {
            using (var wc = new WebClient())
            {
                byte[] bb = wc.DownloadData(url);
                string s = Encoding.UTF8.GetString(bb);
                if (File.Exists(filename))
                    File.Delete(filename);
                File.WriteAllText(filename, s, Encoding.UTF8);
                return s;
            }
        }
    }
}
