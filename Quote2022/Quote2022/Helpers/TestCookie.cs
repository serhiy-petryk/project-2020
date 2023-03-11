using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quote2022.Actions;

namespace Quote2022.Helpers
{
    public static class TestCookie
    {
        public static void Test()
        {
            var a1 = Download.GetResponse("https://eoddata.com/symbols.aspx");
        }
    }
}
