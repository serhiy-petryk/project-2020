using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote2022.Models
{
    public class SymbolsQuantumonlineList
    {
        public string Symbol;
        public string Exchange;
        public string Name;
        public string Url;
        public bool IsDelisted;

        public SymbolsQuantumonlineList(string symbol, string exchange, string name, string url)
        {
            if (symbol.EndsWith("*"))
            {
                Symbol = symbol.Substring(0, symbol.Length - 1);
                IsDelisted = true;
            }
            else
                Symbol = symbol;

            Exchange = exchange;
            Name = name;
            Url = url;
        }
    }
}
