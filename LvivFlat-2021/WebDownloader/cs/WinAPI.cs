using System;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;

namespace WebDownloader {
  class csWinApi {

    [DllImport("wininet.dll", SetLastError = true)]
    private static extern bool InternetGetCookie(string url, string cookieName, StringBuilder cookieData, ref int size);

    public static CookieContainer GetUriCookieContainer(string url) {
      // Taken from www.pinvoke.net
      int datasize = 256;
      StringBuilder cookieData = new StringBuilder(datasize);
      if (!InternetGetCookie(url, null, cookieData, ref datasize)) {
        if (datasize < 0)
          return null;
        // Allocate stringbuilder large enough to hold the cookie
        cookieData = new StringBuilder(datasize);
        if (!InternetGetCookie(url, null, cookieData, ref datasize))
          return null;
      }
      if (cookieData.Length > 0) {
        CookieContainer cookies = new CookieContainer();
        cookies.SetCookies(new Uri(url), cookieData.ToString().Replace(';', ','));
        return cookies;
      }
      else return null;
    }
  }
}
