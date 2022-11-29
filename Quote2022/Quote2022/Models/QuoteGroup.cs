using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Quote2022.Helpers;

namespace Quote2022.Models
{
    public class QuoteGroup
    {
        public const double StartAmount = 1.0;
        public int Cnt;
        public float OpenToClose;
        public double D;
        public double H;
        public double L;
        public double R_N1;
        public double D_N1;
        public double BuyN1;
        public double SellN1;
        public double H_N1;
        public double L_N1;

        public DateTime MinDate;
        public DateTime MaxDate;

        private int Cnt_N1;
        public double BuyAmt;
        public double SellAmt;
        public int BuyWins;
        public int SellWins;
        public string SBuyDrawUp;
        public string SBuyDrawDown;
        public string SSellDrawUp;
        public string SSellDrawDown;
        public int BuyMaxLossCnt;
        public int SellMaxLossCnt;
        public string BuyMaxLossKey;
        public string SellMaxLossKey;
        public string StartBuyMaxLossKey;
        public string StartSellMaxLossKey;

        public QuoteGroup(IList<DayEoddataExtended> data, decimal stop, bool isStopPercent, bool printDetails = false)
        {
            Cnt = data.Count;
            OpenToClose = data.Average(a => a.OpenToClose);
            D = 100.0 * Math.Sqrt(data.Average(a => Math.Pow(a.OpenToClose - OpenToClose, 2)));
            H = 100.0 * data.Count(a => (a.Open - a.Low) < (Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.CL_P1) : stop) - 0.00005)) / Cnt;
            L = 100.0 * data.Count(a => (a.High - a.Open) < (Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.CL_P1) : stop) - 0.00005)) / Cnt;
            MinDate = data.Min(a => a.Date);
            MaxDate = data.Max(a => a.Date);


            var data_N1 = data.Where(a => a.IsValid1).ToList();
            Cnt_N1 = data_N1.Count;
            R_N1 = data_N1.Average(a => a.R1.Value);
            D_N1 = 100.0 * Math.Sqrt(data_N1.Average(a => Math.Pow(a.R1.Value - R_N1, 2)));
            var buyN1Cnt = data_N1.Count(a => a.Open_N1 < (a.CL_N1 - float.Epsilon));
            var sellN1Cnt = data_N1.Count(a => a.Open_N1 > (a.CL_N1 + float.Epsilon));
            BuyN1 = 100.0 * buyN1Cnt / data_N1.Count;
            SellN1 = 100.0 * sellN1Cnt / data_N1.Count;
            H_N1 = 100.0 * data_N1.Count(a => (a.Open_N1 - a.Low_N1) < (Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.CL) : stop) - 0.00005)) / Cnt_N1;
            L_N1 = 100.0 * data_N1.Count(a => (a.High_N1 - a.Open_N1) < (Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.CL) : stop) - 0.00005)) / Cnt_N1;

            if (printDetails)
               Debug.Print($"BuyK\tBuyPrice\tSellK\tSellPrice\tSymbol\tDate\tPrevCL\tOpen\tHigh\tLow\tClose\tVolume");

            BuyAmt = StartAmount;
            SellAmt = StartAmount;
            BuyWins = 0;
            SellWins = 0;
            BuyMaxLossCnt = 0;
            SellMaxLossCnt = 0;
            BuyMaxLossKey = null;
            SellMaxLossKey = null;
            var currBuyMaxLossCnt = 0;
            var currSellMaxLossCnt = 0;

            SBuyDrawUp = null;
            SBuyDrawDown = null;
            string sBuyMin = null;
            string sBuyMax = null;
            var buyMin = StartAmount;
            var buyMax = StartAmount;
            var buyDrawUp = 1.0;
            var buyDrawDown = 1.0;

            SSellDrawUp = null;
            SSellDrawDown = null;
            string sSellMin = null;
            string sSellMax = null;
            var sellMin = StartAmount;
            var sellMax = StartAmount;
            var sellDrawUp = 1.0;
            var sellDrawDown = 1.0;
            foreach (var o in data.Where(a => a.IsValid1).OrderBy(a => a.Date).ThenBy(a => a.Symbol))
            {
                var stopAmt = isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, o.CL) : stop;
                var buyAbs = Algorithm1.BuyAbs(stopAmt, o.Open_N1, o.High_N1, o.Low_N1, o.CL_N1);
                var buyAbsPrice = Algorithm1.BuyAbsPrice(stopAmt, o.Open_N1, o.High_N1, o.Low_N1, o.CL_N1);
                var sellAbs = Algorithm1.SellAbs(stopAmt, o.Open_N1, o.High_N1, o.Low_N1, o.CL_N1);
                var sellAbsPrice = Algorithm1.SellAbsPrice(stopAmt, o.Open_N1, o.High_N1, o.Low_N1, o.CL_N1);

                BuyAmt = BuyAmt * Convert.ToDouble(buyAbs);
                SellAmt = SellAmt * Convert.ToDouble(sellAbs);

                if (BuyAmt < buyMin)
                {
                    sBuyMin = GetString(BuyAmt);
                    buyMin = BuyAmt;
                }
                if (BuyAmt > buyMax)
                {
                    sBuyMax = GetString(BuyAmt);
                    buyMax = BuyAmt;
                }
                if (SellAmt < sellMin)
                {
                    sSellMin = GetString(SellAmt);
                    sellMin = SellAmt;
                }
                if (SellAmt > sellMax)
                {
                    sSellMax = GetString(SellAmt);
                    sellMax = SellAmt;
                }

                if (BuyAmt / buyMin > buyDrawUp)
                {
                    buyDrawUp = BuyAmt / buyMin;
                    SBuyDrawUp = DoubleDoString(100.0 * buyDrawUp) + "%: " + sBuyMin + "=>" + GetString(BuyAmt);
                }
                if (buyMax / BuyAmt > buyDrawDown) {
                    buyDrawDown = buyMax / BuyAmt;
                    SBuyDrawDown = DoubleDoString(100.0/buyDrawDown) + "%: " + sBuyMax + "=>" + GetString(BuyAmt);
                }
                if (SellAmt / sellMin > sellDrawUp)
                {
                    sellDrawUp = SellAmt / sellMin;
                    SSellDrawUp = DoubleDoString(100.0 * sellDrawUp) + "%: " + sSellMin + "=>" + GetString(SellAmt);
                }
                if (sellMax / SellAmt > sellDrawDown)
                {
                    sellDrawDown = sellMax / SellAmt;
                    SSellDrawDown = DoubleDoString(100.0/sellDrawDown) + "%: " + sSellMax + "=>" + GetString(SellAmt);
                }

                if (buyAbsPrice > Convert.ToDecimal(o.Open_N1))
                {
                    BuyWins++;
                    currBuyMaxLossCnt=0;
                    StartBuyMaxLossKey = o.Symbol + "/" + o.Date.ToString("yyyy-MM-dd");
                }
                //  else if (buyAbsPrice < Convert.ToDecimal(o.Open_N1))
                else
                {
                    currBuyMaxLossCnt++;
                    if (currBuyMaxLossCnt > BuyMaxLossCnt)
                    {
                        BuyMaxLossCnt = currBuyMaxLossCnt;
                        BuyMaxLossKey = StartBuyMaxLossKey + " - " + o.Symbol + "/" + o.Date.ToString("yyyy-MM-dd");
                    }
                }

                if (sellAbsPrice < Convert.ToDecimal(o.Open_N1))
                {
                    SellWins++;
                    currSellMaxLossCnt = 0;
                    StartSellMaxLossKey = o.Symbol + "/" + o.Date.ToString("yyyy-MM-dd");
                }
                // else if (sellAbsPrice > Convert.ToDecimal(o.Open_N1))
                else
                {
                    currSellMaxLossCnt++;
                    if (currSellMaxLossCnt > SellMaxLossCnt)
                    {
                        SellMaxLossCnt = currSellMaxLossCnt;
                        SellMaxLossKey = StartSellMaxLossKey + " - " + o.Symbol + "/" + o.Date.ToString("yyyy-MM-dd");
                    }
                };

                if (printDetails)
                    Debug.Print(buyAbs + "\t" + buyAbsPrice + "\t" + sellAbs + "\t" + sellAbsPrice + "\t" + o.Symbol +
                                "\t" + o.Date.ToString("dd.MM.yyyy") + "\t" + o.CL + "\t" + o.Open_N1 + "\t" +
                                o.High_N1 + "\t" + o.Low_N1 + "\t" + o.CL_N1 + "\t" + o.VLM_N1.ToString("F0"));
                string GetString(double amt)
                {
                    return DoubleDoString(amt) + ", " + o.Symbol + "/" + o.Date.ToString("yyyy-MM-dd");
                }
                string DoubleDoString(double amt)
                {
                    if (amt < 10) return amt.ToString("F2", CultureInfo.InvariantCulture);
                    if (amt < 1000) return amt.ToString("F1", CultureInfo.InvariantCulture);
                    if (amt < 1000000) return amt.ToString("F0", CultureInfo.InvariantCulture);
                    return amt.ToString("E2", CultureInfo.InvariantCulture);
                }
            }

        }

        public static string GetHeader1() => "Cnt\tOpenToCL\tD\tH\tL\tMinDate\tMaxDate\tR_N1\tD_N1\tH_N1\tL_N1\tBuy_N1\tSell_N1\tBuyAmt\tSellAmt\tBuyWins,%\tSellWins,%\tBuyMaxLossCnt\tSellMaxLossCnt\tBuyDrawUp\tBuyDrawDown\tSellDrawUp\tSellDrawDown";

        public string GetContent1() =>
            $"{Cnt}\t{OpenToClose}\t{Math.Round(D, 1)}\t{Math.Round(H, 2)}\t{Math.Round(L, 2)}\t{MinDate:dd.MM.yyyy}\t{MaxDate:dd.MM.yyyy}\t{R_N1}\t{Math.Round(D_N1, 1)}\t{Math.Round(H_N1, 2)}\t{Math.Round(L_N1, 2)}\t{Math.Round(BuyN1, 1)}\t{Math.Round(SellN1, 1)}\t{BuyAmt}\t{SellAmt}\t{Math.Round(100.0 * BuyWins / Cnt_N1, 2)}\t{Math.Round(100.0 * SellWins / Cnt_N1, 2)}\t{BuyMaxLossCnt}\t{SellMaxLossCnt}\t{SBuyDrawUp}\t{SBuyDrawDown}\t{SSellDrawUp}\t{SSellDrawDown}";
    }
}
