using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Quote2022.Models;
using Quote2022.Properties;

namespace Quote2022
{
    public static class SaveToDb
    {
        #region ===============  Eoddata Splits  ==================
        public static void SplitEoddata_SaveToDb(List<object[]> items, DateTime timeStamp)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Exchange", typeof(string)));
                data.Columns.Add(new DataColumn("Symbol", typeof(string)));
                data.Columns.Add(new DataColumn("Date", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Split", typeof(string)));
                data.Columns.Add(new DataColumn("K", typeof(float)));
                data.Columns.Add(new DataColumn("TimeStamp", typeof(DateTime)));

                foreach (var item in items)
                    data.Rows.Add(item[0], item[1], item[2], item[3], GetSplitK((string) item[3]), timeStamp);

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "truncate table [Bfr_SplitEoddata]";
                    cmd.ExecuteNonQuery();
                    using (var sbc = new SqlBulkCopy(conn))

                    {
                        sbc.BulkCopyTimeout = 300;
                        sbc.DestinationTableName = "Bfr_SplitEoddata";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }

                    cmd.CommandText = "INSERT INTO SplitEoddata " +
                                      "SELECT a.* FROM Bfr_SplitEoddata a " +
                                      "LEFT JOIN SplitEoddata b ON a.Exchange=b.Exchange AND a.Symbol = b.Symbol AND a.Date = b.Date " +
                                      "WHERE b.Symbol IS NULL";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT count(*) as Recs FROM Bfr_SplitEoddata a " +
                                      "INNER JOIN SplitEoddata b ON a.Exchange=b.Exchange AND a.Symbol = b.Symbol AND a.Date = b.Date " +
                                      "WHERE a.Split<>b.Split";
                    var recs = cmd.ExecuteScalar();
                    if (!Equals(recs, 0))
                        throw new Exception($"Invalid {recs} splits in SplitEoddata table in DB");

                    cmd.CommandText = "pRefreshSplits";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region ===============  Refresh TradingDays table  ==================
        public static void RefreshTradingDays()
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandTimeout = 150;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "pRefreshTradingDays";
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region ===============  Investing.com Splits  ==================
        public static void SplitInvesting_SaveToDb(List<object[]> items, DateTime timeStamp)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Symbol", typeof(string)));
                data.Columns.Add(new DataColumn("Name", typeof(string)));
                data.Columns.Add(new DataColumn("Date", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Split", typeof(string)));
                data.Columns.Add(new DataColumn("K", typeof(float)));
                data.Columns.Add(new DataColumn("TimeStamp", typeof(DateTime)));

                foreach (var item in items)
                {
                    var ss = ((string)item[3]).Split(':');
                    var k = float.Parse(ss[0], CultureInfo.InvariantCulture);
                    k /= float.Parse(ss[1], CultureInfo.InvariantCulture);
                    data.Rows.Add(item[1], item[2], item[0], item[3], k, timeStamp);
                }

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "truncate table [Bfr_SplitInvestor]";
                    cmd.ExecuteNonQuery();
                    using (var sbc = new SqlBulkCopy(conn))

                    {
                        sbc.BulkCopyTimeout = 300;
                        sbc.DestinationTableName = "Bfr_SplitInvestor";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }

                    cmd.CommandText = "INSERT INTO SplitInvestor " +
                                      "SELECT a.* FROM Bfr_SplitInvestor a " +
                                      "LEFT JOIN SplitInvestor b ON a.Symbol = b.Symbol AND a.Date = b.Date " +
                                      "WHERE b.Symbol IS NULL";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT count(*) as Recs FROM Bfr_SplitInvestor a " +
                                      "INNER JOIN SplitInvestor b ON a.Symbol = b.Symbol AND a.Date = b.Date " +
                                      "WHERE a.Split<>b.Split";
                    var recs = cmd.ExecuteScalar();
                    if (!Equals(recs, 0))
                        throw new Exception($"Invalid {recs} splits in SplitInvestor table in DB");

                    cmd.CommandText = "pRefreshSplits";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region ===============  Yahoo Splits  ==================
        public static void SplitYahoo_SaveToDb(Dictionary<string, Dictionary<DateTime, string>> items, DateTime timeStamp)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Symbol", typeof(string)));
                data.Columns.Add(new DataColumn("Date", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Split", typeof(string)));
                data.Columns.Add(new DataColumn("K", typeof(float)));
                data.Columns.Add(new DataColumn("TimeStamp", typeof(DateTime)));

                foreach (var aa in items)
                    foreach (var kvp in aa.Value)
                    data.Rows.Add(aa.Key, kvp.Key, kvp.Value, GetSplitK(kvp.Value), timeStamp);

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "truncate table [Bfr_SplitYahoo]";
                    cmd.ExecuteNonQuery();
                    using (var sbc = new SqlBulkCopy(conn))

                    {
                        sbc.BulkCopyTimeout = 300;
                        sbc.DestinationTableName = "Bfr_SplitYahoo";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }

                    cmd.CommandText = "INSERT INTO SplitYahoo " +
                                      "SELECT a.* FROM Bfr_SplitYahoo a " +
                                      "LEFT JOIN SplitYahoo b ON a.Symbol = b.Symbol AND a.Date = b.Date " +
                                      "WHERE b.Symbol IS NULL";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT count(*) as Recs FROM Bfr_SplitYahoo a " +
                                      "INNER JOIN SplitYahoo b ON a.Symbol = b.Symbol AND a.Date = b.Date " +
                                      "WHERE a.Split<>b.Split "+
                                      "and NOT (a.Symbol='CLSN' and a.Date='2013-10-29') "+
                                      "and NOT(a.Symbol= 'CTXS' and a.Date= '2017-02-01') "+
                                      "and NOT(a.Symbol= 'ENPC' and a.Date= '2021-03-26') " +
                                      "and NOT(a.Symbol= 'USWS' and a.Date= '2021-10-01')";
                    var recs = cmd.ExecuteScalar();
                    if (!Equals(recs, 0))
                        throw new Exception($"Invalid {recs} splits in SplitYahoo table in DB");

                    cmd.CommandText = "pRefreshSplits";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region ===============  Tickertech Splits  ==================
        public static void SplitTickertech_SaveToDb(Dictionary<string, Dictionary<DateTime, string>> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Symbol", typeof(string)));
                data.Columns.Add(new DataColumn("Date", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Split", typeof(string)));
                data.Columns.Add(new DataColumn("K", typeof(float)));

                foreach (var aa in items)
                foreach (var kvp in aa.Value)
                {
                    var ss = kvp.Value.Split(':');
                    var k = float.Parse(ss[0], CultureInfo.InvariantCulture);
                    k /= float.Parse(ss[1], CultureInfo.InvariantCulture);
                    data.Rows.Add(aa.Key, kvp.Key, kvp.Value, k);
                }

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                {
                    conn.Open();
                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 300;
                        sbc.DestinationTableName = "SplitTickertech";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }
        }
        #endregion

        #region ===============  Tickertech Daily  ==================
        public static void DayTickertech_SaveToDb(IEnumerable<DayTickertech> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Symbol", typeof(string)));
                data.Columns.Add(new DataColumn("Date", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Open", typeof(float)));
                data.Columns.Add(new DataColumn("High", typeof(float)));
                data.Columns.Add(new DataColumn("Low", typeof(float)));
                data.Columns.Add(new DataColumn("Close", typeof(float)));
                data.Columns.Add(new DataColumn("Volume", typeof(float)));
                data.Columns.Add(new DataColumn("SplitK", typeof(float)));

                foreach (var item in items)
                    data.Rows.Add(item.Symbol, item.Date, item.Open, item.High, item.Low, item.Close, item.Volume, item.SplitK);

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                {
                    conn.Open();
                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 300;
                        sbc.DestinationTableName = "DayTickertech";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }
        }
        #endregion

        #region ===============  Eoddata Daily  ==================
        public static void DayEoddata_SaveToDb(IEnumerable<DayEoddata> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Symbol", typeof(string)));
                data.Columns.Add(new DataColumn("Exchange", typeof(string)));
                data.Columns.Add(new DataColumn("Date", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Open", typeof(float)));
                data.Columns.Add(new DataColumn("High", typeof(float)));
                data.Columns.Add(new DataColumn("Low", typeof(float)));
                data.Columns.Add(new DataColumn("Close", typeof(float)));
                data.Columns.Add(new DataColumn("Volume", typeof(float)));

                foreach (var item in items)
                    data.Rows.Add(item.Symbol, item.Exchange, item.Date, item.Open, item.High, item.Low, item.Close, item.Volume);

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                {
                    conn.Open();
                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 300;
                        sbc.DestinationTableName = "DayEoddata";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }
        }
        #endregion

        #region ===============  Symbols Nanex  ==================

        public static void SymbolsNanex_SaveToDb(IEnumerable<SymbolsNanex> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Symbol", typeof(string)));
                data.Columns.Add(new DataColumn("Exchange", typeof(string)));
                data.Columns.Add(new DataColumn("Name", typeof(string)));
                data.Columns.Add(new DataColumn("Activity", typeof(int)));
                data.Columns.Add(new DataColumn("LastQuoteDate", typeof(DateTime)));
                data.Columns.Add(new DataColumn("LastTradeDate", typeof(DateTime)));
                data.Columns.Add(new DataColumn("YahooId", typeof(string)));
                data.Columns.Add(new DataColumn("EoddataId", typeof(string)));
                data.Columns.Add(new DataColumn("MsnId", typeof(string)));
                data.Columns.Add(new DataColumn("GoogleId", typeof(string)));
                data.Columns.Add(new DataColumn("Type", typeof(string)));
                data.Columns.Add(new DataColumn("Created", typeof(DateTime)));

                foreach (var item in items)
                    data.Rows.Add(item.Symbol, item.Exchange, item.Name, item.Activity, item.LastQuoteDate, item.LastTradeDate,
                        item.YahooID, item.EoddataID, item.MsnID, item.GoogleID, item.Type, item.Created);

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "truncate table [Bfr_SymbolsNanex]";
                    cmd.ExecuteNonQuery();

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 300;
                        sbc.DestinationTableName = "Bfr_SymbolsNanex";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }
        }

        #endregion

        #region ===============  Yahoo Indexes  ==================
        public static void IndexesYahoo_SaveToDb(IEnumerable<DayYahoo> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Symbol", typeof(string)));
                data.Columns.Add(new DataColumn("Date", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Open", typeof(float)));
                data.Columns.Add(new DataColumn("High", typeof(float)));
                data.Columns.Add(new DataColumn("Low", typeof(float)));
                data.Columns.Add(new DataColumn("Close", typeof(float)));
                data.Columns.Add(new DataColumn("Volume", typeof(float)));
                data.Columns.Add(new DataColumn("AdjClose", typeof(float)));

                foreach (var item in items)
                    data.Rows.Add(item.Symbol, item.Date, item.Open, item.High, item.Low, item.Close, item.Volume,
                        item.AdjClose);

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "truncate table [Bfr_DayYahooIndexes]";
                    cmd.ExecuteNonQuery();

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 300;
                        sbc.DestinationTableName = "Bfr_DayYahooIndexes";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }

                    cmd.CommandText =
                        "INSERT into DayYahooIndexes (Symbol, Date, [Open], High, Low, [Close], Volume, AdjClose) " +
                        "SELECT a.Symbol, a.Date, a.[Open], a.High, a.Low, a.[Close], a.Volume, a.AdjClose " +
                        "from Bfr_DayYahooIndexes a " +
                        "left join DayYahooIndexes b on a.Symbol = b.Symbol and a.Date = b.Date " +
                        "where b.Symbol is null";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region ===============  Yahoo Daily  ==================
        public static void DayYahoo_SaveToDb(IEnumerable<DayYahoo> items, bool clearTable)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Symbol", typeof(string)));
                data.Columns.Add(new DataColumn("Date", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Open", typeof(float)));
                data.Columns.Add(new DataColumn("High", typeof(float)));
                data.Columns.Add(new DataColumn("Low", typeof(float)));
                data.Columns.Add(new DataColumn("Close", typeof(float)));
                data.Columns.Add(new DataColumn("Volume", typeof(float)));
                data.Columns.Add(new DataColumn("AdjClose", typeof(float)));

                foreach (var item in items)
                    data.Rows.Add(item.Symbol, item.Date, item.Open, item.High, item.Low, item.Close, item.Volume,
                        item.AdjClose);

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();

                    if (clearTable)
                    {
                        cmd.CommandTimeout = 150;
                        cmd.CommandText = "truncate table [DayYahoo]";
                        cmd.ExecuteNonQuery();
                    }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 300;
                        sbc.DestinationTableName = "DayYahoo";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
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
