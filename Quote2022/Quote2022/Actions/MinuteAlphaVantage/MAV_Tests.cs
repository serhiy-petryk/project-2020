using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote2022.Actions.MinuteAlphaVantage
{
    public static class MAV_Tests
    {
        public static void CheckMissingDays(Action<string> showStatus)
        {
            showStatus($"CheckMissingDays started. Load data from database ...");

            var tradingDays = new List<DateTime>();
            var symbolList = new Dictionary<string, DateTime>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "SELECT date from TradingDays WHERE date>'2021-01-01' and date<'2023-02-01'";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        tradingDays.Add((DateTime)rdr["Date"]);

                cmd.CommandText = "SELECT symbol, min(Date) MinDate, max(Date) MaxDate from FileLogMinuteAlphaVantage group by symbol";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        symbolList.Add((string)rdr["Symbol"], (DateTime)rdr["MinDate"]);
            }

            var cnt = 0;
            foreach (var kvp in symbolList)
            {
                cnt++;
                if (cnt % 10 == 0)
                    showStatus($"CheckMissingDays. Processed {cnt} symbols from {symbolList.Count}");

                var dates = tradingDays.Where(d => d >= kvp.Value).ToArray();

                var actualDates = new List<DateTime>();
                using (var conn = new SqlConnection(Settings.DbConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = $"select distinct date from FileLogMinuteAlphaVantage where symbol='{kvp.Key}'";
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            actualDates.Add((DateTime) rdr["Date"]);
                }

                var missingDates = dates.Where(d => actualDates.All(d2 => d2 != d)).OrderBy(d => d).ToArray();

                if (missingDates.Length != 0)
                {
                    foreach(var d in missingDates)
                        Debug.Print($"Missing\t{kvp.Key}\t{d:yyyy-MM-dd}");
                }
            }

            showStatus($"CheckMissingDays finished");
        }

        public static void RenameFiles(Action<string> showStatus)
        {
            var rr = new Dictionary<string, string>();
            for (var k = 1; k <= 12; k++)
            {
                rr.Add("year2month" + k, "Y2M" + k);
                rr.Add("year1month" + k, "Y1M" + k);
            }

            var folder = @"E:\Quote\WebData\Minute\AlphaVantage\DataBuffer.Y2M12\MissingSymbols\";
            var files = Directory.GetFiles(folder, "*.csv", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var fn = Path.GetFileNameWithoutExtension(file);
                if (fn.IndexOf("year", StringComparison.InvariantCulture) == -1 ||
                    fn.IndexOf("month", StringComparison.InvariantCulture) == -1)
                {
                }
                else
                {
                    var newFn = file;
                    foreach (var kvp in rr)
                    {
                        if (newFn.IndexOf("AV" + kvp.Key + "_", StringComparison.InvariantCulture) != -1)
                        {
                            newFn = file.Replace("AV" + kvp.Key + "_", "av" + kvp.Value + "_");
                            break;
                        }
                    }

                    if (string.Equals(newFn, file, StringComparison.InvariantCultureIgnoreCase))
                        throw new Exception("Check");

                    File.Move(file, newFn);
                }
            }

            showStatus($"RenameFiles finished");
        }
    }
}
