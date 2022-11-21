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
            var x = Convert.ToInt32(12.67892345F * 10000.0);
            var data = GetData(dataSets, showStatusAction);

            // General total
            var total = new QuoteGroup(data);
            Debug.Print($"\n**All records of data set**\n{QuoteGroup.GetHeader1()}");
            Debug.Print(total.GetContent1());

            // Group by day of week
            var aa1 = data.GroupBy(a => a.Date.DayOfWeek).OrderBy(a=>(int)a.Key);
            var aa2 = aa1.ToDictionary(a=>a.Key, a=> new QuoteGroup(a.ToList()));
            Debug.Print($"\n**Group by Day of Week**\nDayNo\tDayName\t{QuoteGroup.GetHeader1()}");
            foreach (var kvp in aa2)
                Debug.Print($"{(int) kvp.Key}\t{kvp.Key}\t{kvp.Value.GetContent1()}");

            // 
            var aa11 = data.Where(a => a.IsValid1).Average(a => BuyAbs(0.01, a.Open_N1, a.Low_N1, a.CL_N1));
            var aa121 = data.Where(a => a.IsValid1).Average(a => BuyPerc(a.CL, 0.1, a.Open_N1, a.Low_N1, a.CL_N1));
            var aa122 = data.Where(a => a.IsValid1).Average(a => BuyPerc(a.CL, 1.0, a.Open_N1, a.Low_N1, a.CL_N1));
            Debug.Print($"\n");
        }

        private static void Calculate(IEnumerable<DayEoddataExtended> data)
        {
            /*select count(*) Cnt, AVG([Open]/ CL) as OpenToClose, ROUND(STDEV([Open]/ CL)*100,1) as D,
CAST(ROUND(SUM(iif([Open_N1]>[CL_N1],1.0,0.0))*100.0/Count(*),1) as real) as Sell_N1,
CAST(ROUND(SUM(iif([Open_N1]<[CL_N1],1.0,0.0))*100.0/Count(*),1) as real) as Buy_N1,
cast(round(100.0*sum(iif([High]<[Open]+0.005,1,0))/count(*),2) as real) H0,
cast(round(100.0*sum(iif([Low]>[Open]-0.005,1,0))/count(*),2) as real) L0,
min(Date) MinDate, Max(Date) MaxDate
from vDayEoddataExtended;*/
            var cnt = data.Count();
            var openToClose = data.Average(a => a.OpenToClose);
            /*var x1 = data.Max(a => a.OpenToClose);
            var x2 = data.Min(a => a.OpenToClose);
            var x31 = data.Where(a => a.OpenToClose > 2.8).ToArray();
            var x32 = data.Where(a => a.OpenToClose < 1/2.8).ToArray();*/

            var d = Math.Sqrt(data.Average(v => Math.Pow(v.OpenToClose - openToClose, 2)));
            var cnt1 = data.Count(a=>a.IsValid1);
            var sellN1 = data.Count(a => a.IsValid1 && a.Open_N1 > (a.CL_N1 + float.Epsilon));
            var buyN1 = data.Count(a => a.IsValid1 && a.Open_N1 < (a.CL_N1 - float.Epsilon));
            var sellN1Perc = 100.0 * sellN1 / cnt1;
            var buyN1Perc = 100.0 * buyN1 / cnt1;

            var h0 = 100.0 * data.Count(a => (a.High - a.Open) < 0.00995) / cnt;
            var h0_ = data.Count(a => (a.High - a.Open) < 0.00995);
            var l0 = 100.0 * data.Count(a => (a.Open - a.Low) < 0.00995) / cnt;
            var l0_ = data.Count(a => (a.Open - a.Low) < 0.00995);
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

        private static double BuyAbs(double stopDelta, float open, float low, float close) =>
            ((open - low) > stopDelta ? open - stopDelta : close) / open;

        private static double BuyPerc(float lastClose, double stopPercent, float open, float low, float close)
        {
            var stopDelta = Math.Round(lastClose * stopPercent / 100.0, 2);
            if (stopDelta < 0.01)
                stopDelta = 0.01;
            return ((open - low) > stopDelta ? open - stopDelta : close) / open;
        }
    }
}
