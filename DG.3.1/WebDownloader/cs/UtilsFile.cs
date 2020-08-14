using System;
using System.IO;
using System.Text;

namespace WebDownloader {

  class csUtilsFile {

    public static string GetStringFromStream(Stream stream, Encoding encoding) {
      stream.Position = 0;
      int bufferSize = 1024 * 8;
      byte[] bb = new byte[bufferSize];
      StringBuilder sb = new StringBuilder();
      long bytes = 0;
      int cnt = 0;
      do {
        cnt = stream.Read(bb, 0, bufferSize);
        if (cnt > 0) {
          sb.Append(encoding.GetString(bb, 0, cnt));
          bytes += cnt;
        }
      }
      while (cnt > 0);
      return sb.ToString();
    }

    public static long SaveStreamToFile(Stream stream, string fileName) {
      int bufferSize = 1024 * 8;
      byte[] bb = new byte[bufferSize];
      long bytes = 0;
      int cnt = 0;
      using (FileStream fs = new FileStream(fileName, FileMode.CreateNew)) {
        do {
          cnt = stream.Read(bb, 0, bufferSize);
          if (cnt > 0) {
            fs.Write(bb, 0, cnt);
            bytes += cnt;
          }
        }

        while (cnt > 0);
        fs.Close();
      }
      return bytes;
    }

    public static string GetNearestNewFileName(string path, string fileName) {
      string sPath;
      sPath = (path.EndsWith(@"\") ? path : path + @"\");
      if (!File.Exists(sPath + fileName)) return (sPath + fileName);
      int t = fileName.LastIndexOf(".");
      string s1;
      if (t > 0)
        s1 = sPath + fileName.Substring(0, t) + "#{0}." + fileName.Substring(t + 1);
      else {
        s1 = sPath + fileName + "#{0}";
      }
      for (int i = 0; i < 1000; i++) {
        string s2 = String.Format(s1, i.ToString());
        if (!File.Exists(s2)) return s2;
      }
      return "";
    }
  }
}
