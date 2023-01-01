using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FastMember;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static partial class SaveToDb
    {
        public static Dictionary<string, List<string>> GetSymbolsAndKinds(int numberOfSymbols)
        {
            var symbols = new Dictionary<string, List<string>>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"select TOP {numberOfSymbols} a.Exchange, a.Symbol, ISNULL(ISNULL(b.[Index], a.Asset),'STOCKS') Kind, "+
                                  "c.Turnover from SymbolsEoddata a left join [Indexes] b on a.Symbol = b.Symbol "+
                                  "inner join (select exchange, symbol, AVG([Close] * Volume) Turnover FROM DayEoddata "+
                                  "WHERE date between '2022-12-12' and '2022-12-17' AND [close] between 1 and 300 and volume> 1000000 "+
                                  "GROUP BY Exchange, Symbol ) c on a.Exchange = c.Exchange and a.Symbol = c.Symbol "+
                                  "order by c.Turnover desc";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                    {
                        var symbol = (string) rdr["Symbol"];
                        var kind = (string)rdr["Kind"];
                        if (!symbols.ContainsKey(symbol))
                            symbols.Add(symbol, new List<string>());
                        symbols[symbol].Add(kind);
                    }
            }

            return symbols;
        }

        public static List<string> GetSP500Stocks()
        {
            var symbols = new List<string>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT a.symbol FROM DayEoddata a inner join SP500List b on a.Symbol = b.Symbol " +
                                  "where a.date between '2022-12-12' and '2022-12-17' and a.Exchange in ('NYSE', 'NASDAQ') AND " +
                                  "a.[close] between 1 and 300 and volume> 1000000 AND a.symbol not like '%-%' and a.symbol not like '%.%' " +
                                  "group by a.symbol order by 1";
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
                                  "where date between '2022-12-12' and '2022-12-17' and Exchange in ('NYSE', 'NASDAQ') AND " +
                                  "[close] between 1 and 300 and volume>1000000 AND " +
                                  "symbol not like '%-%' and symbol not like '%.%' " +
                                  "group by symbol order by 2 desc";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        symbols.Add((string)rdr["Symbol"]);
            }

            return symbols;
        }

        public static List<string> GetLargestNotSP500Stocks(int numberOfSymbols)
        {
            var symbols = new List<string>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT TOP {numberOfSymbols} a.symbol, AVG(a.[Close]*a.Volume) turnover " +
                                  "FROM DayEoddata a LEFT JOIN SP500List b on a.Symbol = b.Symbol " +
                                  "WHERE a.date between '2022-12-12' and '2022-12-17' and " +
                                  "a.Exchange in ('NYSE', 'NASDAQ') AND a.[close] between 1 and 300 and a.volume > 1000000 AND " +
                                  "a.symbol not like '%-%' and a.symbol not like '%.%' AND b.Symbol is null " +
                                  "GROUP BY a.symbol order by 2 desc";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        symbols.Add((string)rdr["Symbol"]);
            }

            return symbols;
        }

        public static List<string> GetLargestETF(int numberOfSymbols)
        {
            var symbols = new List<string>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT TOP {numberOfSymbols} a.symbol, AVG(a.[Close]*a.Volume) turnover " +
                                  "FROM DayEoddata a INNER JOIN SymbolsEoddata b on a.Exchange = b.Exchange AND a.Symbol = b.Symbol " +
                                  "WHERE a.date between '2022-12-12' and '2022-12-17' and a.Exchange in ('NYSE', 'NASDAQ') AND " +
                                  "a.[close] between 1 and 300 and a.volume > 1000000 AND a.symbol not like '%-%' and " +
                                  "a.symbol not like '%.%' AND b.Asset='ETF' " +
                                  "group by a.symbol order by 2 desc";
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        symbols.Add((string)rdr["Symbol"]);
            }

            return symbols;
        }

        public static List<string> GetLargestNotETF(int numberOfSymbols)
        {
            var symbols = new List<string>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT TOP {numberOfSymbols} a.symbol, AVG(a.[Close]*a.Volume) turnover " +
                                  "FROM DayEoddata a INNER JOIN SymbolsEoddata b on a.Exchange = b.Exchange AND a.Symbol = b.Symbol " +
                                  "WHERE a.date between '2022-12-12' and '2022-12-17' and a.Exchange in ('NYSE', 'NASDAQ') AND " +
                                  "a.[close] between 1 and 300 and a.volume > 1000000 AND a.symbol not like '%-%' and " +
                                  "a.symbol not like '%.%' AND isnull(b.Asset,'')<>'ETF' " +
                                  "group by a.symbol order by 2 desc";
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
