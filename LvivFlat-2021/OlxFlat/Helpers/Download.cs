using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace OlxFlat.Helpers
{
    public static class Download
    {
        //============  OLX  ============
        #region ===============  OLX details  ====================

        public static void OlxDetails_Download(Action<string> showStatusAction)
        {
            var source = new List<(int, string)>
            {
                (710960367, "https://www.olx.ua/d/obyavlenie/prodam-1-k-kvartiru-v-zhk-famlya-po-vul-kulparkvska-ob-IDM77gH.html#c70df8fd89"),
                (710955266, "https://www.olx.ua/d/obyavlenie/prodazh-kvartiri-vul-chornovola-IDM75Vq.html#954597d17f"),
                (702690885, "https://www.olx.ua/d/obyavlenie/prodazh-2kmn-kvartiri-na-vul-shota-rustavel-10hv-pshkom-vd-tsentru-IDLypZX.html#c70df8fd89;promoted"),
                (710923335, "https://www.olx.ua/d/obyavlenie/prodazh-kvartiri-naukova-IDM6XDp.html#954597d17f"),
                (710948976, "https://www.olx.ua/d/obyavlenie/prodazh-prostoro-3k-kvartiri-u-frankvskomu-r-n-52-tis-dol-IDM74iY.html#c70df8fd89"),
                (710939289, "https://www.olx.ua/d/obyavlenie/prodazh-kvartiri-chornovola-IDM71MJ.html#c70df8fd89"),
                (704261056, "https://www.olx.ua/d/obyavlenie/prodazh-1-km-kvartiri-v-novobudov-po-vul-pd-goloskom-IDLF0ti.html#c70df8fd89"),
                (700521329, "https://www.olx.ua/d/obyavlenie/prodatsya-2-km-kvartira-po-vul-gorodotsky-IDLpjB7.html#954597d17f"),
                (708282529, "https://www.olx.ua/d/obyavlenie/pospshayte-pridbati-novu-2k-kvartiru-zhk-na-vul-striysky-zabudovnik-IDLWSDL.html#c70df8fd89"),
                (701762624, "https://www.olx.ua/d/obyavlenie/1-km-novobudova-mazepi-mikolaychuka-2-f-kotel-47-kv-m-rem50000torg-IDLuvv0.html#954597d17f"),
                (710932628, "https://www.olx.ua/d/obyavlenie/prodazh-kvartiri-shevchenka-IDM703i.html#c70df8fd89")
            };
            foreach (var item in source)
            {
                showStatusAction($"Download Olx details: {item.Item1}, {item.Item2}");
                var filename = string.Format(Settings.OlxFileDetailsTemplate, item.Item1);
                DownloadPage(item.Item2, filename);
            }
        }

        #endregion

        #region ===============  OLX list  ====================
        public static void OlxList_Download(Action<string> showStatusAction)
        {
            showStatusAction("OLX: Delete files");
            var files = Directory.GetFiles(Settings.OlxFileListFolder, "*.txt");
            foreach(var fn in files)
                File.Delete(fn);

            showStatusAction($"OLX: Download start range to {Settings.OlxPriceRange[0]}");
            OlxList_Download(null, Settings.OlxPriceRange[0]);

            for (var k = 1; k < Settings.OlxPriceRange.Length; k++)
            {
                showStatusAction($"OLX: Download range from {Settings.OlxPriceRange[k - 1]} to {Settings.OlxPriceRange[k]}");
                OlxList_Download(Settings.OlxPriceRange[k - 1], Settings.OlxPriceRange[k]);
            }

            showStatusAction($"OLX: Download end range from {Settings.OlxPriceRange[Settings.OlxPriceRange.Length - 1]}");
            OlxList_Download(Settings.OlxPriceRange[Settings.OlxPriceRange.Length - 1], null);

            showStatusAction($"OLX: Downloaded");
        }

        private static void OlxList_Download(int? startPrice, int? endPrice)
        {
            if (!startPrice.HasValue)
            {
                var url = string.Format(Settings.OlxStartRangeUrl, 1, endPrice.Value * 1000);
                var fileId = endPrice + "_1";
                var filename = string.Format(Settings.OlxFileListTemplate, fileId);
                var fileContent = DownloadPage(url, filename);
                var pages = OlxList_GetPages(fileContent);
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
                var pages = OlxList_GetPages(fileContent);
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
                var pages = OlxList_GetPages(fileContent);
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
                File.WriteAllText(filename, s, Encoding.UTF8);
                return s;
            }
        }
    }
}
