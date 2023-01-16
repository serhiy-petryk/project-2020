using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static class DataSources
    {
        private static Dictionary<Tuple<int, DateTime, DateTime>, Dictionary<string, SymbolsOfDataSource>>
            _activeSymbols = new Dictionary<Tuple<int, DateTime, DateTime>, Dictionary<string, SymbolsOfDataSource>>();

        public static Dictionary<string, SymbolsOfDataSource> GetActiveSymbols(int numberOfSymbols = 1000, DateTime? from = null, DateTime? to = null)
        {
            from = from ?? new DateTime(2022, 11, 1);
            to = to ?? new DateTime(2022, 12, 31);

            var key = new Tuple<int, DateTime, DateTime>(numberOfSymbols, from.Value, to.Value);
            if (!_activeSymbols.ContainsKey(key))
            {
                var symbols = new Dictionary<string, SymbolsOfDataSource>();
                using (var conn = new SqlConnection(Settings.DbConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText =
                        $"select a.Exchange, a.Symbol, ISNULL(ISNULL(b.[Index], a.Asset),'STOCKS') Kind, c.TradeValue, " +
                        "a.Asset, a.Sector, a.Industry, a.TvType, a.TvSubtype, a.TvSector, a.TvIndustry, a.TvRecommend " +
                        "FROM SymbolsEoddata a " +
                        "left join [Indexes] b on a.Symbol = b.Symbol " +
                        $"inner join (select TOP {numberOfSymbols} exchange, symbol, SUM([Close] * Volume) TradeValue, " +
                        "MIN([Close]) MinClose, MAX([Close]) MaxClose, Min(Volume) MinVolume FROM DayEoddata " +
                        $"WHERE date between '{from:yyyy-MM-dd}' and '{to:yyyy-MM-dd}' " +
                        "GROUP BY Exchange, Symbol " +
                        "HAVING Min([Close])>=1.0 AND MAX([Close])<=300 AND Min(Volume)>=300000 " +
                        "ORDER BY TradeValue DESC) c on a.Exchange = c.Exchange and a.Symbol = c.Symbol " +
                        "ORDER BY c.TradeValue desc";
                    /*cmd.CommandText = $"select a.Symbol, ISNULL(ISNULL(b.[Index], a.Asset),'STOCKS') Kind, c.TradeValue, a.Asset, a.Sector, a.Industry " +
                                      "FROM SymbolsEoddata a " +
                                      "left join [Indexes] b on a.Symbol = b.Symbol " +
                                      $"inner join (select TOP {numberOfSymbols} symbol, SUM([Close] * Volume) TradeValue, " +
                                      "MIN([Close]) MinClose, MAX([Close]) MaxClose, Min(Volume) MinVolume FROM DayEoddata " +
                                      $"WHERE date between '{from:yyyy-MM-dd}' and '{to:yyyy-MM-dd}' " +
                                      "GROUP BY Symbol " +
                                      "HAVING Min([Close])>=1.0 AND MAX([Close])<=300 AND Min(Volume)>=300000 " +
                                      "ORDER BY TradeValue DESC) c on a.Symbol = c.Symbol " +
                                      "ORDER BY c.TradeValue desc";*/

                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                        {
                            var o = new SymbolsOfDataSource(rdr);
                            if (!symbols.ContainsKey(o.Symbol))
                                symbols.Add(o.Symbol, o);
                            symbols[o.Symbol].Kinds.Add((string) rdr["Kind"]);
                        }
                }


                //var gData = Chunk(data.OrderBy(fGroups[k]), (data.Count + chunkSize - 1) / chunkSize);

                var chunkCount = 10;
                var aa = Algorithm1.Chunk(symbols.Values.OrderByDescending(a => a.TradeValue),
                    Algorithm1.GetChunkSize(symbols.Count, chunkCount)).ToList();

                for (var k = 0; k < aa.Count; k++)
                    foreach (var a1 in aa[k])
                        a1.TradeValueId = k;

                foreach (var a in symbols.Values.Where(a => !a.TvRecommend.HasValue))
                    a.TvRecommendId = 0;

                var aa1 = symbols.Values.Where(a => a.TvRecommend.HasValue).OrderBy(a => a.TvRecommend.Value).ToList();
                aa = Algorithm1.Chunk(aa1, Algorithm1.GetChunkSize(aa1.Count, chunkCount)).ToList();

                for (var k = 0; k < aa.Count; k++)
                    foreach (var a1 in aa[k])
                        a1.TvRecommendId = k+1;

                _activeSymbols[key] = symbols;
            }

            return _activeSymbols[key];
        }

        public static Dictionary<string, List<string>> xxGetSymbolsAndKinds(int numberOfSymbols = 1000, DateTime? from = null, DateTime? to = null)
        {
            from = from ?? new DateTime(2022, 11, 1);
            to = to ?? new DateTime(2022, 12, 31);

            var symbols = new Dictionary<string, List<string>>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"select a.Exchange, a.Symbol, ISNULL(ISNULL(b.[Index], a.Asset),'STOCKS') Kind, c.TradeValue, a.Asset, a.Sector, a.Industry " +
                                  "FROM SymbolsEoddata a " +
                                  "left join [Indexes] b on a.Symbol = b.Symbol " +
                                  $"inner join (select TOP {numberOfSymbols} exchange, symbol, SUM([Close] * Volume) TradeValue, " +
                                  "MIN([Close]) MinClose, MAX([Close]) MaxClose, Min(Volume) MinVolume FROM DayEoddata " +
                                  $"WHERE date between '{from:yyyy-MM-dd}' and '{to:yyyy-MM-dd}' " +
                                  "GROUP BY Exchange, Symbol " +
                                  "HAVING Min([Close])>=1.0 AND MAX([Close])<=300 AND Min(Volume)>=300000 " +
                                  "ORDER BY TradeValue DESC) c on a.Exchange = c.Exchange and a.Symbol = c.Symbol " +
                                  "ORDER BY c.TradeValue DESC";

                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                    {
                        var symbol = (string)rdr["Symbol"];
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
                cmd.CommandText = $"SELECT TOP {numberOfSymbols} symbol, AVG([Close]*Volume) TradeValue FROM DayEoddata " +
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
                cmd.CommandText = $"SELECT TOP {numberOfSymbols} a.symbol, AVG(a.[Close]*a.Volume) TradeValue " +
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
                cmd.CommandText = $"SELECT TOP {numberOfSymbols} a.symbol, AVG(a.[Close]*a.Volume) TradeValue " +
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
                cmd.CommandText = $"SELECT TOP {numberOfSymbols} a.symbol, AVG(a.[Close]*a.Volume) TradeValue " +
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
    }
}
