using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static partial class SaveToDb
    {
        #region ===============  Investing.com Splits  ==================
        public static void SplitInvestingHistory_SaveToDb(IEnumerable<SplitModel> items)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                ClearAndSaveToDbTable(conn, items, "Bfr_SplitInvestingHistory", "Symbol", "Date", "Name", "Title",
                    "Ratio", "K", "TimeStamp");

                cmd.CommandText = "INSERT INTO SplitInvestingHistory " +
                                  "SELECT a.* FROM Bfr_SplitInvestingHistory a " +
                                  "LEFT JOIN SplitInvestingHistory b ON a.Symbol = b.Symbol AND a.Date = b.Date " +
                                  "WHERE b.Symbol IS NULL";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT count(*) as Recs FROM Bfr_SplitInvestingHistory a " +
                                  "INNER JOIN SplitInvestingHistory b ON a.Symbol = b.Symbol AND a.Date = b.Date " +
                                  "WHERE a.Ratio<>b.Ratio";
                //                    "WHERE a.Split<>b.Split and a.Symbol<>'HIHI'";
                var recs = cmd.ExecuteScalar();
                if (!Equals(recs, 0))
                    throw new Exception($"Invalid {recs} splits in SplitInvestingHistory table in DB");

                cmd.CommandText = "pRefreshSplits";
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region ===============  StockSplitHistory  ==================
        public static void StockSplitHistory_SaveToDb(List<SplitModel> items)
        {
            ClearAndSaveToDbTable(items, "StockSplitHistory", "Symbol", "Date", "Ratio", "K", "TimeStamp");
        }
        #endregion

        #region ===============  Eoddata Splits  ==================
        public static void SplitEoddata_SaveToDb(IEnumerable<SplitModel> items)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                ClearAndSaveToDbTable(conn, items, "Bfr_SplitEoddata", "Exchange", "Symbol", "Date", "Ratio", "K", "TimeStamp");
                cmd.CommandText = "INSERT INTO SplitEoddata " +
                                  "SELECT a.* FROM Bfr_SplitEoddata a " +
                                  "LEFT JOIN SplitEoddata b ON a.Exchange=b.Exchange AND a.Symbol = b.Symbol AND a.Date = b.Date " +
                                  "WHERE b.Symbol IS NULL";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT count(*) as Recs FROM Bfr_SplitEoddata a " +
                                  "INNER JOIN SplitEoddata b ON a.Exchange=b.Exchange AND a.Symbol = b.Symbol AND a.Date = b.Date " +
                                  "WHERE a.Ratio<>b.Ratio";
                var recs = cmd.ExecuteScalar();
                if (!Equals(recs, 0))
                    throw new Exception($"Invalid {recs} splits in SplitEoddata table in DB");

                cmd.CommandText = "pRefreshSplits";
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region ===============  Investing.com Splits  ==================
        public static void SplitInvesting_SaveToDb(IEnumerable<SplitModel> items)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                ClearAndSaveToDbTable(conn, items, "Bfr_SplitInvesting", "Symbol", "Date", "Name", "Ratio", "K",
                    "TimeStamp");
                cmd.CommandText = "INSERT INTO SplitInvesting " +
                                  "SELECT a.* FROM Bfr_SplitInvesting a " +
                                  "LEFT JOIN SplitInvesting b ON a.Symbol = b.Symbol AND a.Date = b.Date " +
                                  "WHERE b.Symbol IS NULL";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT count(*) as Recs FROM Bfr_SplitInvesting a " +
                                  "INNER JOIN SplitInvesting b ON a.Symbol = b.Symbol AND a.Date = b.Date " +
                                  "WHERE a.Split<>b.Split and a.Symbol<>'HIHI'";
                var recs = cmd.ExecuteScalar();
                if (!Equals(recs, 0))
                    throw new Exception($"Invalid {recs} splits in SplitInvesting table in DB");

                cmd.CommandText = "pRefreshSplits";
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region ===============  Yahoo Splits  ==================
        public static void SplitYahoo_SaveToDb(Dictionary<string, Dictionary<DateTime, string>> items, DateTime timeStamp)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                ClearAndSaveToDbTable(conn, items, "Bfr_SplitYahoo", "Symbol", "Date", "Ratio", "K", "TimeStamp");
                cmd.CommandText = "INSERT INTO SplitYahoo " +
                                  "SELECT a.* FROM Bfr_SplitYahoo a " +
                                  "LEFT JOIN SplitYahoo b ON a.Symbol = b.Symbol AND a.Date = b.Date " +
                                  "WHERE b.Symbol IS NULL";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT count(*) as Recs FROM Bfr_SplitYahoo a " +
                                  "INNER JOIN SplitYahoo b ON a.Symbol = b.Symbol AND a.Date = b.Date " +
                                  "WHERE a.Split<>b.Split " +
                                  "and NOT (a.Symbol='CLSN' and a.Date='2013-10-29') " +
                                  "and NOT(a.Symbol= 'CTXS' and a.Date= '2017-02-01') " +
                                  "and NOT(a.Symbol= 'ENPC' and a.Date= '2021-03-26') " +
                                  "and NOT(a.Symbol= 'USWS' and a.Date= '2021-10-01')";
                var recs = cmd.ExecuteScalar();
                if (!Equals(recs, 0))
                    throw new Exception($"Invalid {recs} splits in SplitYahoo table in DB");

                cmd.CommandText = "pRefreshSplits";
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        private static float GetSplitK(string split)
        {
            var ss = (split).Split(':');
            var k = float.Parse(ss[0], CultureInfo.InvariantCulture);
            k /= float.Parse(ss[1], CultureInfo.InvariantCulture);
            return k;
        }

    }
}
