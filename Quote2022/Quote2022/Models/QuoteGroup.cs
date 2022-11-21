using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote2022.Models
{
    public class QuoteGroup
    {
        public int Cnt;
        public float OpenToClose;
        public double D;
        public double SellN1;
        public double BuyN1;
        public double H;
        public double L;

        public DateTime MinDate;
        public DateTime MaxDate;

        public QuoteGroup(IList<DayEoddataExtended> data)
        {
            Cnt = data.Count();
            OpenToClose = data.Average(a => a.OpenToClose);
            D = 100.0 * Math.Sqrt(data.Average(v => Math.Pow(v.OpenToClose - OpenToClose, 2)));

            var cnt1 = data.Count(a => a.IsValid1);
            var sellN1Cnt = data.Count(a => a.IsValid1 && a.Open_N1 > (a.CL_N1 + float.Epsilon));
            var buyN1Cnt = data.Count(a => a.IsValid1 && a.Open_N1 < (a.CL_N1 - float.Epsilon));
            SellN1 = 100.0 * sellN1Cnt / cnt1;
            BuyN1 = 100.0 * buyN1Cnt / cnt1;

            H = 100.0 * data.Count(a => (a.High - a.Open) < 0.00995) / Cnt;
            L = 100.0 * data.Count(a => (a.Open - a.Low) < 0.00995) / Cnt;

            MinDate = data.Min(a => a.Date);
            MaxDate = data.Max(a => a.Date);
        }

        public static string GetHeader1() => "Cnt\tOpenToClose\tD\tSell_N1\tBuy_N1\tH\tL\tMinDate\tMaxDate";

        public string GetContent1() =>
            $"{Cnt}\t{OpenToClose}\t{Math.Round(D, 1)}\t{Math.Round(SellN1, 1)}\t{Math.Round(BuyN1, 1)}\t{Math.Round(H, 2)}\t{Math.Round(L, 2)}\t{MinDate:dd.MM.yyyy}\t{MaxDate:dd.MM.yyyy}";
    }
}
