using System.Diagnostics;
using System.Net;

namespace Quote2022.Helpers
{
    public static class VPN
    {
        public static void GetMyIp()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var wc = new Actions.Download.WebClientEx();
            wc.TimeoutInMilliSeconds = 10000;

            var sw = new Stopwatch();
            sw.Start();

            // 1-5 secs            Actions.Download.DownloadPage("https://www.whatismyip.com/", @"E:\Temp\xx.html", true);
            // error            Actions.Download.DownloadPage("https://ipaddress.my/", @"E:\Temp\xx.html");
            // 1-5 secs Actions.Download.DownloadPage("https://myip.com.ua/", @"E:\Temp\xx.html");
            // forbidden Actions.Download.DownloadPage("https://whatismyipaddress.com/", @"E:\Temp\xx.html");
            // bad Actions.Download.DownloadPage("https://www.speedtest.net/", @"E:\Temp\xx.html");
            // Actions.Download.DownloadPage("http://ip-info.ff.avast.com/v1/info", @"E:\Temp\xx.html");

            var json2 = wc.DownloadString("https://www.find-ip.net");
            var i1 = json2.IndexOf("<div class=\"ipcontent pure-u-13-24\">");
            var i2 = json2.IndexOf("</div>", i1 + 35);
            var ip = json2.Substring(i1 + 36, i2 - i1 - 36).Trim();

            Debug.Print($"Time1: {sw.ElapsedMilliseconds}");

            var json = wc.DownloadString("http://ip-info.ff.avast.com/v1/info");
            var k1 = json.IndexOf("\"ip\":");
            var k2 = json.IndexOf(",", k1 + 3);
            var a1 = json.Substring(k1 + 6, k2 - k1 - 7);


            var s = json.Substring(1,json.Length-2);
            var ss1 = s.Split(',');

            sw.Stop();

            Debug.Print($"Time2: {sw.ElapsedMilliseconds}");

            var json1 = wc.DownloadString("http://ip-info.ff.avast.com/v1/info");

            /*            string json = (new WebClient()).DownloadString("https://www.expressvpn.com/what-is-my-ip");
                        // Response.Write(json);
                        //https://www.expressvpn.com/what-is-my-ip
                        using (var wc = new WebClient())
                        {
                            var response = wc.DownloadString("https://www.expressvpn.com/what-is-my-ip");
                            // var response = wc.DownloadString("https://www.github.com");
                            File.WriteAllText(@"E:\Temp\xx.html", response);
                        }*/

        }
    }
}
