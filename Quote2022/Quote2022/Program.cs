﻿using System;
using System.Net;
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


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
