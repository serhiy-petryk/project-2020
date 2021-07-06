using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using OlxFlat.Helpers;

namespace OlxFlat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnParseOlx_Click(object sender, EventArgs e)
        {
            Parse.OlxParse(ShowStatus);
            return;

            var cnt = 0;
            var fileCount = 25;
            for (var k1 = 0; k1 < fileCount; k1++)
            {
                var filename = string.Format(Settings.OlxFileListTemplate, (k1 + 1).ToString("000"));
                var s = File.ReadAllText(filename, Encoding.UTF8);
                // var ss1 = s.Split(new[] { "class=\"offer-wrapper\"" }, StringSplitOptions.None);
                var ss1 = s.Split(new[] { "<tr class=\"wrap\"" }, StringSplitOptions.None);
                for (var k2 = 1; k2 < ss1.Length; k2++)
                {
                    var i1 = ss1[k2].IndexOf("</table>", StringComparison.InvariantCultureIgnoreCase);
                    if (i1 < 1)
                        throw new Exception("Trap!!!");

                    cnt++;
                    var s2 = ss1[k2].Substring(0, i1);
                    var promoted = s2.IndexOf("offer promoted", StringComparison.InvariantCultureIgnoreCase) > 0;
                    i1 = s2.IndexOf("data-id=\"", StringComparison.InvariantCultureIgnoreCase);

                    if (i1 == -1)
                        throw new Exception("Trap!!!");

                    var id = ParseString(s2, i1 + 7);
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

                    Debug.Print($"Id: {cnt}, {clock}, {location}, {promoted}, {id}, {href}, {image}, {name}, {price}, {dogovorna}");
                }

                var s3 = ss1[ss1.Length - 1];
                var i5 = s3.LastIndexOf("item fleft", StringComparison.InvariantCultureIgnoreCase);
                i5 = s3.IndexOf("<span>", i5 + 7, StringComparison.InvariantCultureIgnoreCase);
                var i6 = s3.IndexOf("</span>", i5 + 4, StringComparison.InvariantCultureIgnoreCase);
                var pages = s3.Substring(i5 + 6, i6 - i5 - 6).Trim();
            }
        }

        private static int GetPages(string fileContent)
        {
            var i5 = fileContent.LastIndexOf("item fleft", StringComparison.InvariantCultureIgnoreCase);
            i5 = fileContent.IndexOf("<span>", i5 + 7, StringComparison.InvariantCultureIgnoreCase);
            var i6 = fileContent.IndexOf("</span>", i5 + 4, StringComparison.InvariantCultureIgnoreCase);
            var pages = fileContent.Substring(i5 + 6, i6 - i5 - 6).Trim();
            return int.Parse(pages);
        }

        private static string ParseString(string source, int startIndex)
        {
            var i1 = source.IndexOf('"', startIndex);
            var i2 = source.IndexOf('"', i1 + 1);
            var s = source.Substring(i1 + 1, i2 - i1 - 1);
            return s;
        }

        // ==================================
        private void btnParseOlxDetails_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(Settings.OlxFileDetailsFolder);
            foreach (var file in files)
            {
                var s = File.ReadAllText(file, Encoding.UTF8);
                var ss = s.Split(new[] { "img data-src=" }, StringSplitOptions.None);
                var images = new List<string>();
                for (var k1 = 1; k1 < ss.Length; k1++)
                {
                    images.Add(ParseString(ss[k1], 0));
                }

                var s1 = ss[ss.Length - 1];
                var i1 = s1.IndexOf("ad-posted-at", StringComparison.InvariantCultureIgnoreCase);
                var i2 = s1.IndexOf(">", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
                var i3 = s1.IndexOf("<", i2, StringComparison.InvariantCultureIgnoreCase);
                var dated = s1.Substring(i2 + 1, i3 - i2 - 1);

                Debug.Print($"{dated}");
            }

        }

        private void btnLoadFromWeb_Click(object sender, EventArgs e)
        {
            
            Download.OlxDownload(ShowStatus);
            return;

            var pages = 25;
            using (var wc = new WebClient())
            {
                for (var k1 = 0; k1 < pages; k1++)
                {
                    string uri = string.Format(Settings.OlxUrl, k1+1);
                    byte[] bb = wc.DownloadData(uri);
                    string s = Encoding.UTF8.GetString(bb);
                    string fn = string.Format(Settings.OlxFileListTemplate, (k1+1).ToString("000"));
                    if (File.Exists(fn)) File.Delete(fn);
                    File.WriteAllText(fn, s, Encoding.UTF8);
                }
            }

        }

        private void ShowStatus(string message)
        {
            lblStatus.Text = message;
            Application.DoEvents();

        }
    }
}
