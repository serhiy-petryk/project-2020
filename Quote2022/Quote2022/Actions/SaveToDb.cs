using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FastMember;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static partial class SaveToDb
    {
        public static List<string> GetSP500Stocks(int numberOfSymbols)
        {
            var symbols = new List<string>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT TOP {numberOfSymbols} symbol, AVG([Close]*Volume) turnover FROM DayEoddata " +
                                  "where date>= DATEADD(day, -7, GetDate()) and Exchange in ('NYSE', 'NASDAQ') AND " +
                                  "[close] between 1 and 300 and volume>1000000 AND " +
                                  "len(symbol)< 5 and symbol not like '%-%' and symbol not like '%.%' " +
                                  "group by symbol order by 2 desc";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        symbols.Add((string)rdr["Symbol"]);
            }

            return symbols;
        }

        public static List<string> GetLargestStocks(int numberOfSymbols)
        {
            var symbols = new List<string>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT TOP {numberOfSymbols} symbol, AVG([Close]*Volume) turnover FROM DayEoddata " +
                                  "where date>= DATEADD(day, -7, GetDate()) and Exchange in ('NYSE', 'NASDAQ') AND " +
                                  "[close] between 1 and 300 and volume>1000000 AND " +
                                  "len(symbol)< 5 and symbol not like '%-%' and symbol not like '%.%' " +
                                  "group by symbol order by 2 desc";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        symbols.Add((string)rdr["Symbol"]);
            }

            return symbols;
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

        public static void DayEoddata_SaveToDb(IEnumerable<DayEoddata> items) => SaveToDbTable(items, "DayEoddata",
            "Symbol", "Exchange", "Date", "Open", "High", "Low", "Close", "Volume");
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
