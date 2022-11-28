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
        private const int ChunkSize = 8;

        private static Func<DayEoddataExtended, float>[] fGroups = {
            a => a.VlmToWAvg, a=>a.MaxPVlmToWAvg, a=>a.CloseToWAvg, a=>a.OpenToClose, a=>a.CL, a=>a.WAvgVLM, a=>a.WAvgVolatility,
            a=>(a.High-a.Low)/a.CL*100.0F/a.WAvgVolatility, a=>a.DJI_ToWAvg, a=> a.GSPC_ToWAvg
        };

        private static string[] sGroups =
        {
            "VlmToWAvg", "MaxPVlmToWAvg", "CloseToWAvg", "OpenToClose", "Close", "WAvgVLM", "WAvgVolatility",
            "(a.High-a.Low)/a.CL*100.0/a.WAvgVolatility", "DJI_ToWAvg", "GSPC_ToWAvg"
        };

        public static void Execute(IEnumerable<string> dataSets, Action<string> showStatusAction)
        {
            var data = GetData(dataSets, showStatusAction);

            // General total
            var total = new QuoteGroup(data);
            Debug.Print($"\n\t\t**All records of data set**\n{QuoteGroup.GetHeader1("\t\t")}");
            Debug.Print(total.GetContent1("\t\t"));

            // Group by day of week
            var aa1 = data.GroupBy(a => a.Date.DayOfWeek).OrderBy(a=>(int)a.Key);
            var aa2 = aa1.ToDictionary(a=>a.Key, a=> new QuoteGroup(a.ToList()));
            Debug.Print($"\n**Group by Day of Week**\nDayNo\tDayName\t{QuoteGroup.GetHeader1(null)}");
            foreach (var kvp in aa2)
                Debug.Print($"{(int) kvp.Key}\t{kvp.Key}\t{kvp.Value.GetContent1(null)}");

            // var mondayDetails = new QuoteGroup(data.Where(o=>o.Date.DayOfWeek == DayOfWeek.Monday).ToList(), true);

            /*for (var k = 0; k < fGroups.Length; k++)
            {
                var gData = Chunk(data.OrderBy(fGroups[k]), (data.Count + ChunkSize - 1) / ChunkSize);
                Debug.Print($"\n{sGroups[k]}");
                Debug.Print($"G{k}\tMin\tMax\t{QuoteGroup.GetHeader1(null)}");
                var cnt = 0;
                foreach (var d in gData)
                {
                    var min = d.Min(fGroups[k]);
                    var max = d.Max(fGroups[k]);
                    var qg = new QuoteGroup(d.ToList());
                    Debug.Print($"{cnt}\t{min}\t{max}\t{qg.GetContent1(null)}");
                    cnt++;
                }
            }*/


            /*var cnt = 0;
            foreach (var a11 in data.Where(a => a.IsValid1).OrderBy(a=>a.Symbol).ThenBy(a=>a.Date))
            {
                var xb1 = BuyAbs(0.01M, a11.Open_N1, a11.High_N1, a11.Low_N1, a11.CL_N1);
                var xb1Price = GetBuyAbsPrice(0.01M, a11.Open_N1, a11.Low_N1, a11.CL_N1);
                var xb1p = BuyPerc(0.1M, a11.CL, a11.Open_N1, a11.Low_N1, a11.CL_N1);
                var xb1pPrice = GetBuyPercPrice(0.1M, a11.CL, a11.Open_N1, a11.Low_N1, a11.CL_N1);
                var xs1 = SellAbs(0.01M, a11.Open_N1, a11.High_N1, a11.CL_N1);
                var xs1Price = GetSellAbsPrice(0.01M, a11.Open_N1, a11.High_N1, a11.CL_N1);
                var xs1p = SellPerc(0.1M, a11.CL, a11.Open_N1, a11.High_N1, a11.CL_N1);
                var xs1pPrice = GetSellPercPrice(0.1M, a11.CL, a11.Open_N1, a11.High_N1, a11.CL_N1);
                Debug.Print(a11.Symbol + "\t" + a11.Date.ToString("dd.MM.yyyy") + "\t" + a11.CL + "\t" + a11.Open_N1 + "\t" + a11.High_N1 + "\t" + a11.Low_N1 + "\t" + a11.CL_N1 + "\t" + xb1 + "\t" + xb1Price + "\t" + xb1p + "\t" + xb1pPrice + "\t" + xs1 + "\t" + xs1Price + "\t" + xs1p + "\t" + xs1pPrice);
                if (cnt++ >= 5000) break;
            }*/
            /*var b1 = data.Where(a => a.IsValid1).Average(a => BuyAbs(0.01M, a.Open_N1, a.Low_N1, a.CL_N1));
            var b1Price = data.Where(a => a.IsValid1).Average(a => GetBuyAbsPrice(0.01M, a.Open_N1, a.Low_N1, a.CL_N1));
            var b1p = data.Where(a => a.IsValid1).Average(a => BuyPerc(0.1M, a.CL, a.Open_N1, a.Low_N1, a.CL_N1));
            var b1pPrice = data.Where(a => a.IsValid1).Average(a => GetBuyPercPrice(0.1M, a.CL, a.Open_N1, a.Low_N1, a.CL_N1));
            var s1 = data.Where(a => a.IsValid1).Average(a => SellAbs(0.01M, a.Open_N1, a.High_N1, a.CL_N1));
            var s1Price = data.Where(a => a.IsValid1).Average(a => GetSellAbsPrice(0.01M, a.Open_N1, a.High_N1, a.CL_N1));
            var s1p = data.Where(a => a.IsValid1).Average(a => SellPerc(0.1M, a.CL, a.Open_N1, a.High_N1, a.CL_N1));
            var s1pPrice = data.Where(a => a.IsValid1).Average(a => GetSellPercPrice(0.1M, a.CL, a.Open_N1, a.High_N1, a.CL_N1));
            // Debug.Print(b1 + "\t" + b1Price + "\t" + b1p + "\t" + b1pPrice + "\t" +s1 + "\t" + s1Price + "\t" + s1p + "\t" + s1pPrice);*/
            // Debug.Print($"\n");
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

        public static Decimal BuyAbsPrice(decimal stopDelta, float open, float high, float low, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            return (dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close);
        }

        public static decimal BuyPercPrice(decimal stopPercent, float lastClose, float open, float high, float low, float close)
        {
            var dLastClose = Convert.ToDecimal(lastClose);
            var dOpen = Convert.ToDecimal(open);
            var stopDelta = Math.Round(dLastClose * stopPercent / 100.0M, 2);
            if (stopDelta < 0.01M)
                stopDelta = 0.01M;

            return (dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close);
        }

        public static Decimal SellAbsPrice(decimal stopDelta, float open, float high, float low, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            return (Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close);
        }

        public static decimal SellPercPrice(decimal stopPercent, float lastClose, float open, float high, float low, float close)
        {
            var dLastClose = Convert.ToDecimal(lastClose);
            var dOpen = Convert.ToDecimal(open);
            var stopDelta = Math.Round(dLastClose * stopPercent / 100.0M, 2);
            if (stopDelta < 0.01M)
                stopDelta = 0.01M;

            return (Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close);
        }

        //==============================
        public static decimal BuyAbs(decimal stopDelta, float open, float high, float low, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            // return (dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close);
            return ((dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close)) / dOpen;
        }

        public static decimal BuyPerc(decimal stopPercent, float lastClose, float open, float high, float low, float close)
        {
            var dLastClose = Convert.ToDecimal(lastClose);
            var dOpen = Convert.ToDecimal(open);
            var stopDelta = Math.Round(dLastClose * stopPercent / 100.0M, 2);
            if (stopDelta < 0.01M)
                stopDelta = 0.01M;

            // return (dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close);
            return ((dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close)) / dOpen;
        }

        public static decimal SellAbs(decimal stopDelta, float open, float high, float low, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            // return (Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close);
            return dOpen / ((Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close));
        }
        public static decimal SellPerc(decimal stopPercent, float lastClose, float open, float high, float low, float close)
        {
            var dLastClose = Convert.ToDecimal(lastClose);
            var dOpen = Convert.ToDecimal(open);
            var stopDelta = Math.Round(dLastClose * stopPercent / 100.0M, 2);
            if (stopDelta < 0.01M)
                stopDelta = 0.01M;

            // return (Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close);
            return dOpen/((Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close));
        }

        /// <summary>
        /// Break a list of items into chunks of a specific size
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize).ToList();
                source = source.Skip(chunksize);
            }
        }

    }
}
