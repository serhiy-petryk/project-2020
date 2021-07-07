using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        private void ShowStatus(string message)
        {
            lblStatus.Text = message;
            Application.DoEvents();
        }

        private void btnOlxList_LoadFromWeb_Click(object sender, EventArgs e) => Download.OlxList_Download(ShowStatus);
        private void btnOlxList_Parse_Click(object sender, EventArgs e) => Parse.OlxList_Parse(ShowStatus);


        private void btnOlxDetails_LoadFromWeb_Click(object sender, EventArgs e) => Download.OlxDetails_Download(ShowStatus);
        private void btnOlxDetails_Parse_Click(object sender, EventArgs e) => Parse.OlxDetails_Parse(ShowStatus);
    }
}
