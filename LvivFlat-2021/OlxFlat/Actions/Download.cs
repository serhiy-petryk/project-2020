using System;
using System.IO;
using System.Net;
using System.Text;

namespace OlxFlat.Actions
{
    public static class Download
    {
        public static void OlxDownload()
        {
        }

        public static void OlxDownload(int? startPrice, int? endPrice)
        {
            // string urlTemplate;
            if (!startPrice.HasValue)
            {
                var url = string.Format(Settings.OlxStartRangeUrl, 1, endPrice.Value);
                var fileId = endPrice.ToString() + "_1";
                var filename = string.Format(Settings.OlxFileListTemplate, fileId);
                var fileContent = DownloadPage(url, filename);
                var pages = GetOlxPages(fileContent);
                for (var k = 1; k < pages; k++)
                {
                    url = string.Format(Settings.OlxStartRangeUrl, 1, endPrice.Value);
                    fileId = endPrice.ToString() + "_1";
                    filename = string.Format(Settings.OlxFileListTemplate, fileId);
                    DownloadPage(url, filename);
                }
            }
        }

        //============  OLX  ============
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
