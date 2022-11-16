using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quote2022.Models;

namespace Quote2022.Helpers
{
    public class Algorithm1
    {
        public static void Execute(Action<string> showStatusAction)
        {
            var data = new List<DayEoddataExtended>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                showStatusAction($"Algorithm1. Reading data ...");
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "SELECT * from vDayEoddataExtended";
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            data.Add(new DayEoddataExtended(rdr));
                }
            }
            showStatusAction($"Algorithm1. End reading data. Records: {data.Count}");


        }
    }
}
