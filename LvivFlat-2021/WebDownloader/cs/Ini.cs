using System;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Net;

namespace WebDownloader {
  class csIni {

    public static string pathExe = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    public static string pathLog = $@"{pathExe}\Log\";

    static csIni() {
      LogFolderClear();
      IniHttp();
    }

    static string GetMyDocumentFolder() {
      return Environment.GetFolderPath(Environment.SpecialFolder.System);
    }

    private static void LogFolderClear() {
      if (!Directory.Exists(csIni.pathLog)) Directory.CreateDirectory(csIni.pathLog);
      string[] files = Directory.GetFiles(csIni.pathLog);
      DateTime cutOffDate = DateTime.Now.AddDays(-1);
      for (int i = 0; i < files.Length; i++) {
        if (File.GetLastWriteTime(files[i]) < cutOffDate) File.Delete(files[i]);
      }
    }

    private static void IniHttp() {
      WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
      HttpWebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
      //      HttpWebRequest.DefaultWebProxy.als = CredentialCache.DefaultCredentials;
      FtpWebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
      System.Net.ServicePointManager.Expect100Continue = false;
      System.Net.ServicePointManager.DefaultConnectionLimit = 1000;
      int t1; int t2;
      ThreadPool.GetAvailableThreads(out t1, out t2);
      ThreadPool.GetMaxThreads(out t1, out t2);
      ThreadPool.SetMaxThreads(1000, 1500);
      ThreadPool.GetAvailableThreads(out t1, out t2);
      ThreadPool.GetMaxThreads(out t1, out t2);
      ThreadPool.GetMinThreads(out t1, out t2);
    }

  }
}
