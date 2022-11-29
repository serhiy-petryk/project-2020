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

        private static decimal[] stops =
        {
            10000M, 0.01M, 0.02M, 0.05M, 0.1M, 0.2M, 0.5M, 1M
        };

        public static void Execute(IEnumerable<string> dataSets, Action<string> showStatusAction)
        {
            var data = GetData(dataSets, showStatusAction);

            PrintGeneral(data);
            // PrintStops(data);

            // var mondayDetails = new QuoteGroup(data.Where(o=>o.Date.DayOfWeek == DayOfWeek.Monday).ToList(), 0.01M, false, true);

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
        }

        private static void PrintGeneral(List<DayEoddataExtended> data)
        {
            // General total
            var total = new QuoteGroup(data, 0.01M, false, false);
            Debug.Print($"\n**All records of data set**\n\t{QuoteGroup.GetHeader1()}");
            Debug.Print("\t" + total.GetContent1());

            // Group by day of week
            var aa1 = data.GroupBy(a => a.Date.DayOfWeek).OrderBy(a=>(int)a.Key);
            var aa2 = aa1.ToDictionary(a=>a.Key, a=> new QuoteGroup(a.ToList(), 0.01M, false, false));
            Debug.Print($"\n**Group by Day of Week**\nDay\t{QuoteGroup.GetHeader1()}");
            foreach (var kvp in aa2)
                Debug.Print($"{kvp.Key}\t{kvp.Value.GetContent1()}");
        }

        private static void PrintStops(List<DayEoddataExtended> data)
            {
            Debug.Print($"\n**All records of data set**\n\tStop\t{QuoteGroup.GetHeader1()}");
            foreach (var stop in stops)
            {
                var dataStop = new QuoteGroup(data, stop, false, false);
                Debug.Print($"\t{stop}$\t{dataStop.GetContent1()}");
            }
            foreach (var stop in stops)
            {
                var dataStop = new QuoteGroup(data, stop * 10, true, false);
                Debug.Print($"\t{stop * 10}%\t{dataStop.GetContent1()}");
            }
            // Group by day of week
            Debug.Print($"\n**Group by Day of Week**\nStop\tDay\t{QuoteGroup.GetHeader1()}");
            var aa1 = data.GroupBy(a => a.Date.DayOfWeek).OrderBy(a => (int)a.Key);
            foreach (var dataByDay in aa1)
            {
                var day = dataByDay.First().Date.DayOfWeek;
                foreach (var stop in stops)
                {
                    var dataStop = new QuoteGroup(dataByDay.ToList(), stop, false, false);
                    Debug.Print($"{stop}$\t{day}\t{dataStop.GetContent1()}");
                }
                foreach (var stop in stops)
                {
                    var dataStop = new QuoteGroup(dataByDay.ToList(), stop * 10, true, false);
                    Debug.Print($"{stop * 10}%\t{day}\t{dataStop.GetContent1()}");
                }
            }
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
            var dOpen = Convert.ToDecimal(open);
            var stopDelta = GetStopDeltaForPercent(stopPercent, lastClose);
            return (dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close);
        }

        public static Decimal SellAbsPrice(decimal stopDelta, float open, float high, float low, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            return (Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close);
        }

        public static decimal SellPercPrice(decimal stopPercent, float lastClose, float open, float high, float low, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            var stopDelta = GetStopDeltaForPercent(stopPercent, lastClose);
            return (Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close);
        }

        //==============================
        public static decimal BuyAbs(decimal stopDelta, float open, float high, float low, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            return ((dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close)) / dOpen;
        }

        public static decimal BuyPerc(decimal stopPercent, float lastClose, float open, float high, float low, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            var stopDelta = GetStopDeltaForPercent(stopPercent, lastClose);
            return ((dOpen - Convert.ToDecimal(low)) >= stopDelta ? dOpen - stopDelta : Convert.ToDecimal(close)) / dOpen;
        }

        public static decimal SellAbs(decimal stopDelta, float open, float high, float low, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            return dOpen / ((Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close));
        }
        public static decimal SellPerc(decimal stopPercent, float lastClose, float open, float high, float low, float close)
        {
            var dOpen = Convert.ToDecimal(open);
            var stopDelta = GetStopDeltaForPercent(stopPercent, lastClose);
            return dOpen/((Convert.ToDecimal(high) - dOpen) >= stopDelta ? dOpen + stopDelta : Convert.ToDecimal(close));
        }

        public static decimal GetStopDeltaForPercent(decimal stopPercent, float lastClose)
        {
            var dLastClose = Convert.ToDecimal(lastClose);
            var stopDelta = Math.Round(dLastClose * stopPercent / 100.0M, 2);
            if (stopDelta < 0.01M)
                stopDelta = 0.01M;
            return stopDelta;
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
