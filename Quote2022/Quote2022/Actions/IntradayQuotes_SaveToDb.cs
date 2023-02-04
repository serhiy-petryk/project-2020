using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static class IntradayQuotes_SaveToDb
    {
        public static void Execute(string[] zipFiles, Action<string> showStatus)
        {
            var from = new TimeSpan(9, 45, 0);
            var to = new TimeSpan(15, 45, 0);

            var quotes = QuoteLoader.MinuteYahoo_GetQuotesFromZipFiles(showStatus, zipFiles, false, false);

            var iQuotes = new List<IntradayQuoteForDay>();
            SaveToDb.ClearAndSaveToDbTable(iQuotes, "Bfr_IntradayQuoteForDay", "Symbol", "Date", "Open", "High", "Low", "Close", "Volume", "Count", "OpenAt", "HighAt", "LowAt", "CloseAt");
            string lastSymbol = null;
            var lastDate = DateTime.MinValue;
            IntradayQuoteForDay currentQuote = null;
            foreach (var q in quotes)
            {
                if (!string.Equals(q.Symbol, lastSymbol) || !Equals(lastDate, q.Timed.Date))
                {
                    if (currentQuote != null && currentQuote.Count > 0)
                        iQuotes.Add(currentQuote);
                    currentQuote = new IntradayQuoteForDay { Symbol = q.Symbol, Date = q.Timed.Date };
                }

                var qTime = q.Timed.TimeOfDay;
                if (qTime >= from && qTime < to)
                {
                    currentQuote.Count++;
                    if (!currentQuote.Open.HasValue)
                    {
                        currentQuote.Open = q.Open;
                        currentQuote.OpenAt = qTime;
                    }

                    if ((currentQuote.High ?? float.MinValue) < q.High)
                    {
                        currentQuote.High = q.High;
                        currentQuote.HighAt = qTime;
                    }
                    if ((currentQuote.Low ?? float.MaxValue) > q.Low)
                    {
                        currentQuote.Low = q.Low;
                        currentQuote.LowAt = qTime;
                    }
                    currentQuote.Close = q.Close;
                    currentQuote.Volume += Convert.ToInt32(q.Volume);
                }
                else if (qTime >= to && !currentQuote.CloseAt.HasValue)
                {
                    currentQuote.Close = q.Open;
                    currentQuote.CloseAt = qTime;
                }

                lastSymbol = q.Symbol;
                lastDate = q.Timed.Date;
            }

            showStatus($"IntradayQuotes_SaveToDb. Save to database ...");
            SaveToDb.SaveToDbTable(iQuotes, "Bfr_IntradayQuoteForDay", "Symbol", "Date", "Open", "High", "Low", "Close", "Volume", "Count", "OpenAt", "HighAt", "LowAt", "CloseAt");
            SaveToDb.RunProcedure("pUpdateIntradayQuoteForDay");

            showStatus($"IntradayQuotes_SaveToDb. Finished!");
        }
    }
}
