using System;
using System.Collections.Generic;
using System.Data;

namespace Quote2022.Models
{
    public class SymbolsOfDataSource
    {
        // public string ExchangeAndSymbol => Symbol + @"/" + Exchange;
        public string Exchange;
        public string Symbol;
        public string Asset;
        public string Sector;
        public string Industry;
        public List<string> Kinds = new List<string>();
        public double Turnover;
        public int TurnoverId = int.MinValue;

        public SymbolsOfDataSource(IDataReader rdr)
        {
            Exchange = (string)rdr["Exchange"];
            Symbol = (string)rdr["Symbol"];
            Asset = GetDbString(rdr["Asset"]);
            Sector = GetDbString(rdr["Sector"]) ?? Asset;
            Industry = GetDbString(rdr["Industry"]) ?? Asset;
            Turnover = (double)rdr["Turnover"];
        }

        private static string GetDbString(object rdrValue) => ReferenceEquals(rdrValue, DBNull.Value) ? null : (string) rdrValue;
    }

}
