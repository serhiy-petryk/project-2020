using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Quote2022.Actions;
using Quote2022.Models;

namespace Quote2022.Helpers
{
    public class YahooMinute
    {
        public static void Test()
        {
            var validQuotes = new List<Quote>();

            var symbols = SaveToDb.GetLargestStocks(500);
//            var files = Directory.GetFiles(@"E:\Quote\WebData\Minute\Yahoo\YahooMinute_20221224", "yMin-AA.txt", SearchOption.AllDirectories);
            var files = Directory.GetFiles(@"E:\Quote\WebData\Minute\Yahoo\YahooMinute_20221224", "yMin-*.txt", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var symbol = Path.GetFileNameWithoutExtension(file).Substring(5);
                if (!symbols.Contains(symbol))
                    continue;

                var content = File.ReadAllText(file);
                var o = JsonConvert.DeserializeObject<Models.MinuteYahoo>(content);

                // Debug.Print($"{o.Chart.Result[0].Meta.Symbol}\t{o.Chart.Result[0].Meta.ExchangeName}\t{o.Chart.Result[0].Meta.InstrumentType}");
                var quotes = o.GetQuotes();
                var timeK = 0;
                Quote currentQuote = null;
                foreach (var q in quotes.Where(a=>a.Timed.DayOfWeek == DayOfWeek.Wednesday).OrderBy(a=>a.Timed))
                {
                    var newTimeK = Convert.ToInt32(q.Timed.TimeOfDay.TotalSeconds / 60.0) / 30;
                    if (newTimeK != timeK)
                    {
                        if (currentQuote != null)
                        {
                            currentQuote.Close = q.Open;
                            if (currentQuote.High < currentQuote.Close)
                                currentQuote.High = currentQuote.Close;
                            if (currentQuote.Low > currentQuote.Close)
                                currentQuote.Low = currentQuote.Close;

                            if (currentQuote.Timed.TimeOfDay >= new TimeSpan(10,0,0) && currentQuote.Timed.TimeOfDay <= new TimeSpan(15, 30, 0))
                                validQuotes.Add(currentQuote);
                        }

                        currentQuote = new Quote
                        {
                            Symbol = symbol,
                            Timed = q.Timed,
                            Open = q.Open,
                            High = q.High,
                            Low = q.Low,
                            Close = float.NaN,
                            Volume = q.Volume
                        };
                        timeK = newTimeK;
                    }
                    else
                    {
                        if (q.High > currentQuote.High) currentQuote.High = q.High;
                        if (q.Low < currentQuote.Low) currentQuote.Low = q.Low;
                        currentQuote.Volume += q.Volume;
                    }
                }
            }

            foreach(var q in validQuotes.OrderBy(a=>a.Timed))
                Debug.Print(q.ToString());
        }
  }
}
