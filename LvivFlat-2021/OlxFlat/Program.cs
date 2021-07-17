using System;
using System.Net;
using System.Windows.Forms;

namespace OlxFlat
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Temp.Developers.ParseZnList();

            Application.Run(new Form1());
        }
    }
}
