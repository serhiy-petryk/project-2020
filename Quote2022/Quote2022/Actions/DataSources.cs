﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote2022.Actions
{
    public static class DataSources
    {
        public static Dictionary<string, List<string>> GetSymbolsAndKinds(int numberOfSymbols=1000, DateTime? from = null, DateTime? to = null)
        {
            from = from ?? new DateTime(2022, 11, 1);
            to = to ?? new DateTime(2022, 12, 31);

            var symbols = new Dictionary<string, List<string>>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"select a.Exchange, a.Symbol, ISNULL(ISNULL(b.[Index], a.Asset),'STOCKS') Kind, c.Turnover "+
                                  "FROM SymbolsEoddata a "+
                                  "left join [Indexes] b on a.Symbol = b.Symbol " +
                                  $"inner join (select TOP {numberOfSymbols} exchange, symbol, SUM([Close] * Volume) Turnover, " +
                                  "MIN([Close]) MinClose, MAX([Close]) MaxClose, Min(Volume) MinVolume FROM DayEoddata " +
                                  $"WHERE date between '{from:yyyy-MM-dd}' and '{to:yyyy-MM-dd}' " +
                                  "GROUP BY Exchange, Symbol "+
                                  "HAVING Min([Close])>=1.0 AND MAX([Close])<=300 AND Min(Volume)>=300000 "+
                                  "ORDER BY Turnover DESC) c on a.Exchange = c.Exchange and a.Symbol = c.Symbol " +
                                  "ORDER BY c.Turnover desc";

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
    }
}
