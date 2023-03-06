using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OfficeOpenXml.ConditionalFormatting;

namespace Quote2022.Actions.TradingView
{
    public static class WebArchive_Profile
    {
        #region =============  Download Data  ===============
        public static void DownloadData(Action<string> showStatusAction)
        {
            // showStatusAction($"TradingView.WebArchive_Screener finished for {Path.GetFileName(filenameTemplate)}");
        }

        public class cYearList
        {
            public Dictionary<string, object> status;
            public string first_ts;
            public string last_ts;
        }
        public class cDayList
        {
            public int[][] items;
        }
        #endregion
    }
}
