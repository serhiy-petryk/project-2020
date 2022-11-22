using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Quote2022.Models;

namespace Quote2022.Helpers
{
    public class Algorithm1
    {
        public static void Execute(IEnumerable<string> dataSets, Action<string> showStatusAction)
        {
            var data = GetData(dataSets, showStatusAction);

            /*// General total
            var total = new QuoteGroup(data);
            Debug.Print($"\n**All records of data set**\n{QuoteGroup.GetHeader1()}");
            Debug.Print(total.GetContent1());

            // Group by day of week
            var aa1 = data.GroupBy(a => a.Date.DayOfWeek).OrderBy(a=>(int)a.Key);
            var aa2 = aa1.ToDictionary(a=>a.Key, a=> new QuoteGroup(a.ToList()));
            Debug.Print($"\n**Group by Day of Week**\nDayNo\tDayName\t{QuoteGroup.GetHeader1()}");
            foreach (var kvp in aa2)
                Debug.Print($"{(int) kvp.Key}\t{kvp.Key}\t{kvp.Value.GetContent1()}");*/

            // 
            foreach (var a11 in data.Where(a => a.IsValid1).OrderBy(a=>a.Symbol).ThenBy(a=>a.Date))
            {
                var x1 = BuyPerc(0.1M, a11.CL, a11.Open_N1, a11.Low_N1, a11.CL_N1);
                var x2 = GetBuyPercPrice(0.1M, a11.CL, a11.Open_N1, a11.Low_N1, a11.CL_N1);
                Debug.Print(a11.Symbol + "\t" + a11.Date.ToString("dd.MM.yyyy") + "\t" + a11.CL + "\t" + a11.Open_N1 + "\t" + a11.Low_N1 + "\t" + a11.CL_N1 + "\t" + x1 + "\t" + x2);
            }
            /*var b1 = data.Where(a => a.IsValid1).Average(a => BuyAbs(0.01M, a.Open_N1, a.Low_N1, a.CL_N1));
            var b1p = data.Where(a => a.IsValid1).Average(a => BuyPerc(0.1M, a.CL, a.Open_N1, a.Low_N1, a.CL_N1));
            var s1 = data.Where(a => a.IsValid1).Average(a => SellAbs(0.01M, a.Open_N1, a.High_N1, a.CL_N1));
            var s1p = data.Where(a => a.IsValid1).Average(a => SellPerc(0.1M, a.CL, a.Open_N1, a.High_N1, a.CL_N1));
            Debug.Print(b1 + "\t" + b1p + "\t" + s1 + "\t" + s1p);*/
            Debug.Print($"\n");
        }

        private static List<DayEoddataExtended> GetData(IEnumerable<string> dataSets, Action<string> showStatusAction)
        {
            var data = new Dictionary<string, DayEoddataExtended>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;
                    foreach (var dataSet in dataSets)
                    {
                        switch (dataSet)
                        {
                            case "2013":
                                cmd.CommandText = "SELECT * from vDayEoddata2013Extended";
                                break;
                            case "2022":
                                cmd.CommandText = "SELECT * from vDayEoddataExtended";
                                break;
                            default:
                                throw new Exception($"{dataSet} is not valid data set");
                        }

                        if (!string.IsNullOrEmpty(cmd.CommandText))
                        {
                            showStatusAction($"Reading data from {dataSet} data set ...");
                            using (var rdr = cmd.ExecuteReader())
                                while (rdr.Read())
                                {
                                    var o = new DayEoddataExtended(rdr);
                                    if (!data.ContainsKey(o.Key))
                                    data.Add(o.Key, o);
                                }
                        }
                    }
                }
            }
            showStatusAction($"End reading data. Records: {data.Count}");
            return data.Values.ToList();
        }

        private static Decimal GetBuyAbsPrice(decimal stopDelta, float open, float low, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            return (dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close);
        }

        private static decimal GetBuyPercPrice(decimal stopPercent, float lastClose, float open, float low, float close)
        {
            var dLastClose = Convert.ToDecimal(lastClose);
            var dOpen = Convert.ToDecimal(open);
            var stopDelta = Math.Round(dLastClose * stopPercent / 100.0M, 2);
            if (stopDelta < 0.01M)
                stopDelta = 0.01M;

            return (dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close);
        }

        private static Decimal GetSellAbsPrice(decimal stopDelta, float open, float high, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            return (Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close);
        }

        private static decimal GetSellPercPrice(decimal stopPercent, float lastClose, float open, float high, float close)
        {
            var dLastClose = Convert.ToDecimal(lastClose);
            var dOpen = Convert.ToDecimal(open);
            var stopDelta = Math.Round(dLastClose * stopPercent / 100.0M, 2);
            if (stopDelta < 0.01M)
                stopDelta = 0.01M;

            return (Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close);
        }

        //==============================
        private static decimal BuyAbs(decimal stopDelta, float open, float low, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            // return (dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close);
            return ((dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close)) / dOpen;
        }

        private static decimal BuyPerc(decimal stopPercent, float lastClose, float open, float low, float close)
        {
            var dLastClose = Convert.ToDecimal(lastClose);
            var dOpen = Convert.ToDecimal(open);
            var stopDelta = Math.Round(dLastClose * stopPercent / 100.0M, 2);
            if (stopDelta < 0.01M)
                stopDelta = 0.01M;

            // return (dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close);
            return ((dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close)) / dOpen;
        }

        private static decimal SellAbs(decimal stopDelta, float open, float high, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            // return (Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close);
            return dOpen / ((Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close));
        }
        private static decimal SellPerc(decimal stopPercent, float lastClose, float open, float high, float close)
        {
            var dLastClose = Convert.ToDecimal(lastClose);
            var dOpen = Convert.ToDecimal(open);
            var stopDelta = Math.Round(dLastClose * stopPercent / 100.0M, 2);
            if (stopDelta < 0.01M)
                stopDelta = 0.01M;

            // return (Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close);
            return dOpen/((Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close));
        }



    }
}
