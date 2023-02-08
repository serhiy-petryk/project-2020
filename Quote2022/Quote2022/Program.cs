using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace Quote2022
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            // WebRequest.DefaultWebProxy = null;*/

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            HttpWebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            //      HttpWebRequest.DefaultWebProxy.als = CredentialCache.DefaultCredentials;
            FtpWebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            System.Net.ServicePointManager.Expect100Continue = true;
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //var a1 = Dns.GetHostEntry(Dns.GetHostName());
            //var vpn = NetworkInterface.GetAllNetworkInterfaces().First(x => x.Name == "VPNConnection");
            //var ip = vpn.GetIPProperties().UnicastAddresses.First(x => x.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Address.ToString();
            // Helpers.VPN.GetMyIp();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
