using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Quote2022.Helpers;

namespace Quote2022.Models
{
    public class TradeStatistics
    {
        public const double StartAmount = 1.0;
        public double OpenToClose;
        public double OpenToCloseDisp;
        public double UpPercent;
        public double DownPercent;

        private int Cnt;
        public double BuyK;
        public double SellK;
        public double BuyEndAmt;
        public double SellEndAmt;
        public int BuyWins;
        public int SellWins;

        public double BuyDrawUp;
        public double BuyDrawDown;
        public double SellDrawUp;
        public double SellDrawDown;
        public string BuyDrawUpKey;
        public string BuyDrawDownKey;
        public string SellDrawUpKey;
        public string SellDrawDownKey;
        
        public int BuyMaxLossCnt;
        public int SellMaxLossCnt;
        public string BuyMaxLossKey;
        public string SellMaxLossKey;
        public string StartBuyMaxLossKey;
        public string StartSellMaxLossKey;

        public double Limit1BuyAmt;
        public double Limit1BuyProfit;
        public int Limit1BuyStarted;
        public double Limit1BuyMinAmt;
        public double Limit1SellAmt;
        public double Limit1SellProfit;
        public int Limit1SellStarted;
        public double Limit1SellMinAmt;

        public double Limit3BuyAmt;
        public double Limit3BuyProfit;
        public int Limit3BuyStarted;
        public double Limit3BuyMinAmt;
        public double Limit3SellAmt;
        public double Limit3SellProfit;
        public int Limit3SellStarted;
        public double Limit3SellMinAmt;

        public double Limit10BuyAmt;
        public double Limit10BuyProfit;
        public int Limit10BuyStarted;
        public double Limit10BuyMinAmt;
        public double Limit10SellAmt;
        public double Limit10SellProfit;
        public int Limit10SellStarted;
        public double Limit10SellMinAmt;

        public TradeStatistics(IList<Quote> data, decimal stop = 0.01M, bool isStopPercent = false, bool printDetails = false)
        {
            /*Cnt = data.Count;
            OpenToClose = data.Average(a => a.OpenToClose);
            D = 100.0 * Math.Sqrt(data.Average(a => Math.Pow(a.OpenToClose - OpenToClose, 2)));
            H = 100.0 * data.Count(a => (a.Open - a.Low) < (Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.CL_P1) : stop) - 0.00005)) / Cnt;
            L = 100.0 * data.Count(a => (a.High - a.Open) < (Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.CL_P1) : stop) - 0.00005)) / Cnt;
            MinDate = data.Min(a => a.Date);
            MaxDate = data.Max(a => a.Date);*/

            // var data_N1 = data.Where(a => a.IsValid1).ToList();
            Cnt = data.Count;
            OpenToClose = data.Average(a => 1.0 * a.Open / a.Close);
            OpenToCloseDisp = 100.0 * Math.Sqrt(data.Average(a => Math.Pow(a.Open / a.Close - OpenToClose, 2)));
            var buyN1Cnt = data.Count(a => a.Open < (a.Close - float.Epsilon));
            var sellN1Cnt = data.Count(a => a.Open > (a.Close + float.Epsilon));
            UpPercent = 100.0 * buyN1Cnt / data.Count;
            DownPercent = 100.0 * sellN1Cnt / data.Count;
            // H_N1 = 100.0 * data.Count(a => a.Open<a.Close && (a.Open - a.Low) < (Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.Open) : stop) - 0.00005)) / Cnt_N1;
            // L_N1 = 100.0 * data.Count(a => (a.High - a.Open) < (Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.Open) : stop) - 0.00005)) / Cnt_N1;

            BuyK = data.Average(a => (a.Open - a.Low) < (Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.Open) : stop) - 0.00005) ?
                a.Close / a.Open : (a.Open - Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.Open) : stop)) / a.Open);
            SellK = data.Average(a => (a.High - a.Open) < (Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.Open) : stop) - 0.00005) ?
                a.Open / a.Close : a.Open / (a.Open + Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.Open) : stop)));

            if (printDetails)
               Debug.Print($"BuyK\tBuyPrice\tSellK\tSellPrice\tSymbol\tDate\tOpen\tHigh\tLow\tClose\tVolume");

            BuyEndAmt = StartAmount;
            SellEndAmt = StartAmount;
            BuyWins = 0;
            SellWins = 0;
            BuyMaxLossCnt = 0;
            SellMaxLossCnt = 0;
            BuyMaxLossKey = null;
            SellMaxLossKey = null;
            var currBuyMaxLossCnt = 0;
            var currSellMaxLossCnt = 0;

            BuyDrawUp = 1.0;
            BuyDrawDown = 1.0;
            BuyDrawUpKey = null;
            BuyDrawDownKey = null;
            string sBuyMin = null;
            string sBuyMax = null;
            var buyMin = StartAmount;
            var buyMax = StartAmount;

            SellDrawUp = 1.0;
            SellDrawDown = 1.0;
            SellDrawUpKey = null;
            SellDrawDownKey = null;
            string sSellMin = null;
            string sSellMax = null;
            var sellMin = StartAmount;
            var sellMax = StartAmount;

            Limit1BuyAmt = StartAmount;
            Limit1BuyProfit = 0.0;
            Limit1BuyStarted = 0;
            Limit1BuyMinAmt = StartAmount;
            Limit1SellAmt = StartAmount;
            Limit1SellProfit = 0.0;
            Limit1SellStarted = 0;
            Limit1SellMinAmt = StartAmount;

            Limit3BuyAmt = StartAmount;
            Limit3BuyProfit = 0.0;
            Limit3BuyStarted = 0;
            Limit3BuyMinAmt = StartAmount;
            Limit3SellAmt = StartAmount;
            Limit3SellProfit = 0.0;
            Limit3SellStarted = 0;
            Limit3SellMinAmt = StartAmount;

            Limit10BuyAmt = StartAmount;
            Limit10BuyProfit = 0.0;
            Limit10BuyStarted = 0;
            Limit10BuyMinAmt = StartAmount;
            Limit10SellAmt = StartAmount;
            Limit10SellProfit = 0.0;
            Limit10SellStarted = 0;
            Limit10SellMinAmt = StartAmount;

            var cnt = 0;
            foreach (var o in data)
            {
                cnt++;
                var stopAmt = isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, o.Open) : stop;
                var buyAbs = Algorithm1.BuyAbs(stopAmt, o.Open, o.High, o.Low, o.Close);
                var buyAbsPrice = Algorithm1.BuyAbsPrice(stopAmt, o.Open, o.High, o.Low, o.Close);
                var sellAbs = Algorithm1.SellAbs(stopAmt, o.Open, o.High, o.Low, o.Close);
                var sellAbsPrice = Algorithm1.SellAbsPrice(stopAmt, o.Open, o.High, o.Low, o.Close);

                BuyEndAmt = BuyEndAmt * Convert.ToDouble(buyAbs);
                SellEndAmt = SellEndAmt * Convert.ToDouble(sellAbs);

                // Buy/Sell DrawUp/Down
                if (BuyEndAmt < buyMin)
                {
                    sBuyMin = GetString(BuyEndAmt);
                    buyMin = BuyEndAmt;
                }
                if (BuyEndAmt > buyMax)
                {
                    sBuyMax = GetString(BuyEndAmt);
                    buyMax = BuyEndAmt;
                }
                if (SellEndAmt < sellMin)
                {
                    sSellMin = GetString(SellEndAmt);
                    sellMin = SellEndAmt;
                }
                if (SellEndAmt > sellMax)
                {
                    sSellMax = GetString(SellEndAmt);
                    sellMax = SellEndAmt;
                }

                if (BuyEndAmt / buyMin > BuyDrawUp)
                {
                    BuyDrawUp = BuyEndAmt / buyMin;
                    BuyDrawUpKey = sBuyMin + "=>" + GetString(BuyEndAmt);
                }
                if (buyMax / BuyEndAmt > BuyDrawDown) {
                    BuyDrawDown = buyMax / BuyEndAmt;
                    BuyDrawDownKey = sBuyMax + "=>" + GetString(BuyEndAmt);
                }
                if (SellEndAmt / sellMin > SellDrawUp)
                {
                    SellDrawUp = SellEndAmt / sellMin;
                    SellDrawUpKey = sSellMin + "=>" + GetString(SellEndAmt);
                }
                if (sellMax / SellEndAmt > SellDrawDown)
                {
                    SellDrawDown = sellMax / SellEndAmt;
                    SellDrawDownKey = sSellMax + "=>" + GetString(SellEndAmt);
                    // SellDrawDownKey = DoubleDoString(100.0 / SellDrawDown) + "%: " + sSellMax + "=>" + GetString(SellAmt);
                }

                if (buyAbsPrice > Convert.ToDecimal(o.Open))
                {
                    BuyWins++;
                    currBuyMaxLossCnt=0;
                    StartBuyMaxLossKey = o.Symbol + "/" + o.TimeString;
                }
                //  else if (buyAbsPrice < Convert.ToDecimal(o.Open_N1))
                else
                {
                    currBuyMaxLossCnt++;
                    if (currBuyMaxLossCnt > BuyMaxLossCnt)
                    {
                        BuyMaxLossCnt = currBuyMaxLossCnt;
                        BuyMaxLossKey = StartBuyMaxLossKey + " - " + o.Symbol + "/" + o.TimeString;
                    }
                }

                if (sellAbsPrice < Convert.ToDecimal(o.Open))
                {
                    SellWins++;
                    currSellMaxLossCnt = 0;
                    StartSellMaxLossKey = o.Symbol + "/" + o.TimeString;
                }
                // else if (sellAbsPrice > Convert.ToDecimal(o.Open_N1))
                else
                {
                    currSellMaxLossCnt++;
                    if (currSellMaxLossCnt > SellMaxLossCnt)
                    {
                        SellMaxLossCnt = currSellMaxLossCnt;
                        SellMaxLossKey = StartSellMaxLossKey + " - " + o.Symbol + "/" + o.TimeString;
                    }
                };

                // Limit x1
                Limit1BuyAmt = Limit1BuyAmt * Convert.ToDouble(buyAbs);
                if (Limit1BuyAmt > StartAmount)
                {
                    Limit1BuyProfit += Limit1BuyAmt - StartAmount;
                    Limit1BuyAmt = StartAmount;
                    if (Limit1BuyStarted == 0) Limit1BuyStarted = cnt;
                }
                else if (Limit1BuyProfit < double.Epsilon && Limit1BuyMinAmt > Limit1BuyAmt)
                    Limit1BuyMinAmt = Limit1BuyAmt;

                Limit1SellAmt = Limit1SellAmt * Convert.ToDouble(sellAbs);
                if (Limit1SellAmt > StartAmount)
                {
                    Limit1SellProfit += Limit1SellAmt - StartAmount;
                    Limit1SellAmt = StartAmount;
                    if (Limit1SellStarted == 0) Limit1SellStarted = cnt;
                }
                else if (Limit1SellProfit < double.Epsilon && Limit1SellMinAmt > Limit1SellAmt)
                    Limit1SellMinAmt = Limit1SellAmt;

                // Limit x3
                Limit3BuyAmt = Limit3BuyAmt * Convert.ToDouble(buyAbs);
                if (Limit3BuyAmt > 3.0 * StartAmount)
                {
                    Limit3BuyProfit += Limit3BuyAmt - 3.0 * StartAmount;
                    Limit3BuyAmt = 3.0 * StartAmount;
                    if (Limit3BuyStarted == 0) Limit3BuyStarted = cnt;
                }
                else if (Limit3BuyProfit < double.Epsilon && Limit3BuyMinAmt > Limit3BuyAmt)
                    Limit3BuyMinAmt = Limit3BuyAmt;

                Limit3SellAmt = Limit3SellAmt * Convert.ToDouble(sellAbs);
                if (Limit3SellAmt > 3.0 * StartAmount)
                {
                    Limit3SellProfit += Limit3SellAmt - 3.0 * StartAmount;
                    Limit3SellAmt = 3.0 * StartAmount;
                    if (Limit3SellStarted == 0) Limit3SellStarted = cnt;
                }
                else if (Limit3SellProfit < double.Epsilon && Limit3SellMinAmt > Limit3SellAmt)
                    Limit3SellMinAmt = Limit3SellAmt;

                // Limit x10
                Limit10BuyAmt = Limit10BuyAmt * Convert.ToDouble(buyAbs);
                if (Limit10BuyAmt > 10.0 * StartAmount)
                {
                    Limit10BuyProfit += Limit10BuyAmt - 10.0 * StartAmount;
                    Limit10BuyAmt = 10.0 * StartAmount;
                    if (Limit10BuyStarted == 0) Limit10BuyStarted = cnt;
                }
                else if (Limit10BuyProfit < double.Epsilon && Limit10BuyMinAmt > Limit10BuyAmt)
                    Limit10BuyMinAmt = Limit10BuyAmt;

                Limit10SellAmt = Limit10SellAmt * Convert.ToDouble(sellAbs);
                if (Limit10SellAmt > 10.0 * StartAmount)
                {
                    Limit10SellProfit += Limit10SellAmt - 10.0 * StartAmount;
                    Limit10SellAmt = 10.0 * StartAmount;
                    if (Limit10SellStarted == 0) Limit10SellStarted = cnt;
                }
                else if (Limit10SellProfit < double.Epsilon && Limit10SellMinAmt > Limit10SellAmt)
                    Limit10SellMinAmt = Limit10SellAmt;

                if (printDetails)
                    Debug.Print(buyAbs + "\t" + buyAbsPrice + "\t" + sellAbs + "\t" + sellAbsPrice + "\t" + o.Symbol +
                                "\t" + o.TimeString + "\t" + o.Open + "\t" +
                                o.High + "\t" + o.Low + "\t" + o.Close + "\t" + o.Volume.ToString("F0"));

                string GetString(double amt)
                {
                    return DoubleDoString(amt) + ", " + o.Symbol + "/" + o.TimeString;
                }
            }
        }

        public static string GetHeader() => $"Cnt\t(Open/CL-1),%\tDisp\tUp, %\tDown, %\tBuyK\tSellK\t(BuyK+ SellK)/2\tBuy EndAmt\tSell EndAmt\t" +
                                             $"Buy Wins,%\tSell Wins,%\tBuyMax LossCnt\tSellMax LossCnt\t" +
                                             $"BuyDraw Up,%\tBuyDraw Down,%\tSellDraw Up,%\tSellDraw Down,%\t" +
                                             $"Limit3 BuyCnt\tLimit3 SellCnt\t\t" +
                                             $"Limit1Buy\tLimit1Sell\tLimit3Buy\tLimit3Sell\t" +
                                             $"BuyDrawUpKey\tBuyDrawDownKey\tSellDrawUpKey\tSellDrawDownKey\t" +
                                             $"BuyMaxLossKey\tSellMaxLossKey";
        public string GetContent()
        {
            return $"{Cnt}\t{(OpenToClose - 1.0) * 100.0}\t{Math.Round(OpenToCloseDisp, 2)}\t" +
                   $"{Math.Round(UpPercent, 1)}\t{Math.Round(DownPercent, 1)}\t" +
                   $"{(BuyK - 1.0) * 100.0}\t{(SellK - 1.0) * 100.0}\t{(BuyK - 1.0) * 50.0+(SellK - 1.0) * 50.0}"+
                   $"\t{BuyEndAmt}\t{SellEndAmt}\t" +
                   $"{Math.Round(100.0 * BuyWins / Cnt, 2)}\t{Math.Round(100.0 * SellWins / Cnt, 2)}\t" +
                   $"{BuyMaxLossCnt}\t{SellMaxLossCnt}\t" +
                   $"{BuyDrawUp * 100.0}\t{100.0 / BuyDrawDown}\t" + $"{SellDrawUp * 100.0}\t{100.0 / SellDrawDown}\t" +
                   $"{Limit3BuyStarted}\t{Limit3SellStarted}\t\t" +
                   $"{GetLimitString(Limit1BuyStarted, Limit1BuyProfit, Limit1BuyAmt, Limit1BuyMinAmt)}\t" +
                   $"{GetLimitString(Limit1SellStarted, Limit1SellProfit, Limit1SellAmt, Limit1SellMinAmt)}\t" +
                   $"{GetLimitString(Limit3BuyStarted, Limit3BuyProfit, Limit3BuyAmt, Limit3BuyMinAmt)}\t" +
                   $"{GetLimitString(Limit3SellStarted, Limit3SellProfit, Limit3SellAmt, Limit3SellMinAmt)}\t" +
                   $"{BuyDrawUpKey}\t{BuyDrawDownKey}\t{SellDrawUpKey}\t{SellDrawDownKey}\t" +
                   $"{BuyMaxLossKey}\t{SellMaxLossKey}";
        }

        private static string DoubleDoString(double amt)
        {
            if (Math.Abs(amt) < double.Epsilon) return "0";
            if (Math.Abs(amt) < 1) return amt.ToString("F4", CultureInfo.InvariantCulture);
            if (Math.Abs(amt) < 10) return amt.ToString("F2", CultureInfo.InvariantCulture);
            if (Math.Abs(amt) < 1000) return amt.ToString("F1", CultureInfo.InvariantCulture);
            if (Math.Abs(amt) < 1000000) return amt.ToString("F0", CultureInfo.InvariantCulture);
            return amt.ToString("E2", CultureInfo.InvariantCulture);
        }

        private static string GetLimitString(int limitStarted, double limitProfit, double limitAmt, double limitMinAmt) =>
            $"{limitStarted}/{DoubleDoString(limitProfit)}/{DoubleDoString(limitAmt)}/{DoubleDoString(limitMinAmt)}";
    }
}
