using System;
using System.IO;

namespace Quote2022.Actions
{
    public static class Github
    {
        public static void NasdaqScreener(Action<string> ShowStatus)
        {
            var startUrl = "https://github.com/rreichel3/US-Stock-Symbols/commits/main";
            var filename = @"E:\Quote\WebArchive\Screener\NasdaqGithub\NG_0.html";

            Download.DownloadPage(startUrl, filename);

            var content = File.ReadAllText(filename);
        }
    }
}
