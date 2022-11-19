using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Quote2022.Models;

namespace Quote2022.Helpers
{
    public class Algorithm1
    {
        public static void Execute(IEnumerable<string> dataSets, Action<string> showStatusAction)
        {
            var data = GetData(dataSets, showStatusAction);
            Calculate(data);
            /*var a1 = data.Where(a => !a.IsValid1).ToArray();
            var a2 = data.Where(a => !a.IsValid2).ToArray();
            var a3 = data.Where(a => !a.IsValid3).ToArray();
            var a4 = data.Where(a => !a.IsValid4).ToArray();
            var a5 = data.Where(a => !a.IsValid5).ToArray();*/
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

            /*var k1 = 1.0;
            var k11 = 0.0;
            foreach (var a in data)
            {
                k1 *= a.OpenToClose;
                k11 += a.OpenToClose;
            }
            var k2 = k1 / cnt;
            var k21 = k11 / cnt;*/
            /*var a1 = data.Where(a => a.High < (a.Open + 0.00995F)).OrderBy(a => a.Symbol).ThenBy(a => a.Date).ToArray();
            var a12 = a1.Select(a => a.Symbol + "\t" + a.Date.ToString("dd.MM.yyyy") + "\t" + a.Open.ToString() + "\t" + a.High.ToString()).ToArray();
            foreach(var a in a12) { 
                Debug.Print(a);*/
        }

        private static List<DayEoddataExtended> GetData(IEnumerable<string> dataSets, Action<string> showStatusAction)
        {
            var data = new List<DayEoddataExtended>();
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
                                cmd.CommandText = null;
                                break;
                        }

                        if (!string.IsNullOrEmpty(cmd.CommandText))
                        {
                            showStatusAction($"Reading data from {dataSet} data set ...");
                            using (var rdr = cmd.ExecuteReader())
                                while (rdr.Read())
                                    data.Add(new DayEoddataExtended(rdr));
                        }
                    }
                }
            }
            showStatusAction($"End reading data. Records: {data.Count}");
            return data;
        }
    }
}
