using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FastMember;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static partial class SaveToDb
    {
        public static Tuple<string, float> GetSplitsForWeek(string symbol, DateTime fromDate)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT Ratio, K from Splits WHERE symbol='{symbol}' and  date >='{fromDate:yyyy-MM-dd}' and date<'{fromDate.AddDays(7):yyyy-MM-dd}'";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        return new Tuple<string, float>((string) rdr["Ratio"], Convert.ToSingle(rdr["K"]));
            }
            return null;
        }

        public static Dictionary<Tuple<string, DateTime>, Quote> GetQuoteForWeek(DateTime fromDate)
        {
            var quotes = new Dictionary<Tuple<string, DateTime>, Quote>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT isnull(b.YahooSymbol, a.Symbol) Symbol, a.Date, a.[Open], a.High, a.Low, a.[Close], a.Volume " +
                                  $"from DayEoddata a left join SymbolsEoddata b on a.Symbol = b.Symbol and a.Exchange = b.Exchange " +
                                  $"WHERE date >= '{fromDate:yyyy-MM-dd}' and date<'{fromDate.AddDays(7):yyyy-MM-dd}'";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                    {
                        var q = new Quote
                        {
                            Symbol = (string)rdr["Symbol"],
                            Timed = (DateTime)rdr["Date"],
                            Open = (float)rdr["Open"],
                            High = (float)rdr["High"],
                            Low = (float)rdr["Low"],
                            Close = (float)rdr["Close"],
                            Volume = Convert.ToInt64((float)rdr["Volume"])
                        };
                        quotes.Add(new Tuple<string, DateTime>(q.Symbol, q.Timed), q);
                    }
            }
            return quotes;
        }

        public static Quote GetEodQuote(string symbol, DateTime date)
        {
            var q = (Quote)null;
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT * from DayEoddata WHERE symbol='{symbol}' AND date='{date:yyyy-MM-dd}'";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                    {
                        q = new Quote
                        {
                            Symbol = (string)rdr["Symbol"],
                            Timed = (DateTime)rdr["Date"],
                            Open = (float)rdr["Open"],
                            High = (float)rdr["High"],
                            Low = (float)rdr["Low"],
                            Close = (float)rdr["Close"],
                            Volume = Convert.ToInt64((float)rdr["Volume"])
                        };
                        break;
                    }
            }
            return q;
        }

        public static void RunProcedure(string procedureName, Dictionary<string, object> paramaters = null)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = procedureName;
                cmd.CommandTimeout = 150;
                cmd.CommandType = CommandType.StoredProcedure;
                if (paramaters != null)
                {
                    foreach (var kvp in paramaters)
                        cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                }
                cmd.ExecuteNonQuery();
            }
        }

        public static void SaveToDbTable<T>(SqlConnection conn, IEnumerable<T> items, string destinationTable, params string[] properties)
        {
            using (var reader = ObjectReader.Create(items, properties))
            using (var bcp = new SqlBulkCopy(conn))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                bcp.BulkCopyTimeout = 300;
                bcp.DestinationTableName = destinationTable;
                bcp.WriteToServer(reader);
            }
        }

        public static void SaveToDbTable<T>(IEnumerable<T> items, string destinationTable, params string[] properties)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
                SaveToDbTable(conn, items, destinationTable, properties);
        }

        public static void ClearAndSaveToDbTable<T>(SqlConnection conn,  IEnumerable<T> items, string destinationTable, params string[] properties)
        {
            using (var reader = ObjectReader.Create(items, properties))
            using (var cmd = conn.CreateCommand())
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.CommandTimeout = 150;
                cmd.CommandText = $"truncate table [{destinationTable}]";
                cmd.ExecuteNonQuery();

                using (var bcp = new SqlBulkCopy(conn))
                {
                    bcp.BulkCopyTimeout = 300;
                    bcp.DestinationTableName = destinationTable;
                    bcp.WriteToServer(reader);
                }
            }
        }

        public static void ClearAndSaveToDbTable<T>(IEnumerable<T> items, string destinationTable, params string[] properties)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
                ClearAndSaveToDbTable(conn, items, destinationTable, properties);
        }

        #region ===============  Eoddata Daily  ==================

        public static void DayEoddata_SaveToDb(IEnumerable<DayEoddata> items, Action<string> showStatusAction)
        {
            showStatusAction($"DayEoddata_SaveToDb. Saving data to database ...");
            SaveToDbTable(items, "DayEoddata", "Symbol", "Exchange", "Date", "Open", "High", "Low", "Close", "Volume");
            SaveToDb.RunProcedure("pUpdateDayEoddata");
        }

        #endregion

        #region ===============  Symbols Nanex  ==================
        public static void SymbolsNanex_SaveToDb(IEnumerable<SymbolsNanex> items)
        {
             SaveToDb.ClearAndSaveToDbTable(items, "Bfr_SymbolsNanex", "Symbol", "Exchange", "Name", "Activity",
                "LastQuoteDate", "LastTradeDate", "YahooID", "EoddataID", "MsnID", "GoogleID", "Type", "Created");
        }
        #endregion

        #region ===============  Yahoo Indexes  ==================
        public static void IndexesYahoo_SaveToDb(IEnumerable<DayYahoo> items)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                ClearAndSaveToDbTable(conn, items, "Bfr_DayYahooIndexes", "Symbol", "Date", "Open", "High", "Low", "Close",
                    "Volume", "AdjClose");

                cmd.CommandText =
                    "INSERT into DayYahooIndexes (Symbol, Date, [Open], High, Low, [Close], Volume, AdjClose) " +
                    "SELECT a.Symbol, a.Date, a.[Open], a.High, a.Low, a.[Close], a.Volume, a.AdjClose " +
                    "from Bfr_DayYahooIndexes a " +
                    "left join DayYahooIndexes b on a.Symbol = b.Symbol and a.Date = b.Date " +
                    "where b.Symbol is null";
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region ===============  Yahoo Daily  ==================
        public static void DayYahoo_SaveToDb(IEnumerable<DayYahoo> items, bool clearTable)
        {
            if (clearTable)
                ClearAndSaveToDbTable(items, "DayYahoo", "Symbol", "Date", "Open", "High", "Low", "Close", "Volume",
                    "AdjClose");
            else
                SaveToDbTable(items, "DayYahoo", "Symbol", "Date", "Open", "High", "Low", "Close", "Volume",
                    "AdjClose");
        }
        #endregion
    }
}
