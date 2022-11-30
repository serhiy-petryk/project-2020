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
        public double Up_N1;
        public double Down_N1;
        public double H_N1;
        public double L_N1;

        public DateTime MinDate;
        public DateTime MaxDate;

        private int Cnt_N1;
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

        public double Limit3BuyAmt;
        public double Limit3BuyProfit;
        public int Limit3BuyStarted;
        public double Limit3SellAmt;
        public double Limit3SellProfit;
        public int Limit3SellStarted;

        public double Limit10BuyAmt;
        public double Limit10BuyProfit;
        public int Limit10BuyStarted;
        public double Limit10SellAmt;
        public double Limit10SellProfit;
        public int Limit10SellStarted;

        public double Limit30BuyAmt;
        public double Limit30BuyProfit;
        public int Limit30BuyStarted;
        public double Limit30SellAmt;
        public double Limit30SellProfit;
        public int Limit30SellStarted;

        public QuoteGroup(IList<DayEoddataExtended> data, decimal stop = 0.01M, bool isStopPercent = false, bool printDetails = false)
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
            Up_N1 = 100.0 * buyN1Cnt / data_N1.Count;
            Down_N1 = 100.0 * sellN1Cnt / data_N1.Count;
            H_N1 = 100.0 * data_N1.Count(a => (a.Open_N1 - a.Low_N1) < (Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.CL) : stop) - 0.00005)) / Cnt_N1;
            L_N1 = 100.0 * data_N1.Count(a => (a.High_N1 - a.Open_N1) < (Convert.ToDouble(isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, a.CL) : stop) - 0.00005)) / Cnt_N1;

            if (printDetails)
               Debug.Print($"BuyK\tBuyPrice\tSellK\tSellPrice\tSymbol\tDate\tPrevCL\tOpen\tHigh\tLow\tClose\tVolume");

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

            Limit3BuyAmt = StartAmount;
            Limit3BuyProfit = 0.0;
            Limit3BuyStarted = 0;
            Limit3SellAmt = StartAmount;
            Limit3SellProfit = 0.0;
            Limit3SellStarted = 0;

            Limit10BuyAmt = StartAmount;
            Limit10BuyProfit = 0.0;
            Limit10BuyStarted = 0;
            Limit10SellAmt = StartAmount;
            Limit10SellProfit = 0.0;
            Limit10SellStarted = 0;

            Limit30BuyAmt = StartAmount;
            Limit30BuyProfit = 0.0;
            Limit30BuyStarted = 0;
            Limit30SellAmt = StartAmount;
            Limit30SellProfit = 0.0;
            Limit30SellStarted = 0;

            var cnt = 0;
            foreach (var o in data.Where(a => a.IsValid1).OrderBy(a => a.Date).ThenBy(a => a.Symbol))
            {
                cnt++;
                var stopAmt = isStopPercent ? Algorithm1.GetStopDeltaForPercent(stop, o.CL) : stop;
                var buyAbs = Algorithm1.BuyAbs(stopAmt, o.Open_N1, o.High_N1, o.Low_N1, o.CL_N1);
                var buyAbsPrice = Algorithm1.BuyAbsPrice(stopAmt, o.Open_N1, o.High_N1, o.Low_N1, o.CL_N1);
                var sellAbs = Algorithm1.SellAbs(stopAmt, o.Open_N1, o.High_N1, o.Low_N1, o.CL_N1);
                var sellAbsPrice = Algorithm1.SellAbsPrice(stopAmt, o.Open_N1, o.High_N1, o.Low_N1, o.CL_N1);

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

                // Limit x3
                Limit3BuyAmt = Limit3BuyAmt * Convert.ToDouble(buyAbs);
                Limit3SellAmt = Limit3SellAmt * Convert.ToDouble(sellAbs);
                if (Limit3BuyAmt > 3.0 * StartAmount)
                {
                    Limit3BuyProfit += Limit3BuyAmt - 3.0 * StartAmount;
                    Limit3BuyAmt = 3.0 * StartAmount;
                    if (Limit3BuyStarted == 0) Limit3BuyStarted = cnt;
                }
                if (Limit3SellAmt > 3.0 * StartAmount)
                {
                    Limit3SellProfit += Limit3SellAmt - 3.0 * StartAmount;
                    Limit3SellAmt = 3.0 * StartAmount;
                    if (Limit3SellStarted == 0) Limit3SellStarted = cnt;
                }

                // Limit x10
                Limit10BuyAmt = Limit10BuyAmt * Convert.ToDouble(buyAbs);
                Limit10SellAmt = Limit10SellAmt * Convert.ToDouble(sellAbs);
                if (Limit10BuyAmt > 10.0 * StartAmount)
                {
                    Limit10BuyProfit += Limit10BuyAmt - 10.0 * StartAmount;
                    Limit10BuyAmt = 10.0 * StartAmount;
                    if (Limit10BuyStarted == 0) Limit10BuyStarted = cnt;
                }
                if (Limit10SellAmt > 10.0 * StartAmount)
                {
                    Limit10SellProfit += Limit10SellAmt - 10.0 * StartAmount;
                    Limit10SellAmt = 10.0 * StartAmount;
                    if (Limit10SellStarted == 0) Limit10SellStarted = cnt;
                }

                // Limit x30
                Limit30BuyAmt = Limit30BuyAmt * Convert.ToDouble(buyAbs);
                Limit30SellAmt = Limit30SellAmt * Convert.ToDouble(sellAbs);
                if (Limit30BuyAmt > 30.0 * StartAmount)
                {
                    Limit30BuyProfit += Limit30BuyAmt - 30.0 * StartAmount;
                    Limit30BuyAmt = 30.0 * StartAmount;
                    if (Limit30BuyStarted == 0) Limit30BuyStarted = cnt;
                }
                if (Limit30SellAmt > 30.0 * StartAmount)
                {
                    Limit30SellProfit += Limit30SellAmt - 30.0 * StartAmount;
                    Limit30SellAmt = 30.0 * StartAmount;
                    if (Limit30SellStarted == 0) Limit30SellStarted = cnt;
                }

                string GetString(double amt)
                {
                    return DoubleDoString(amt) + ", " + o.Symbol + "/" + o.Date.ToString("yyyy-MM-dd");
                }
            }

        }

        public static string GetHeader1Old() => "Cnt\tOpenToCL\tD\tH\tL\tMinDate\tMaxDate\tR_N1\tD_N1\tH_N1\tL_N1\tBuy_N1\tSell_N1\tBuyAmt\tSellAmt\tBuyWins,%\tSellWins,%\tBuyMaxLossCnt\tSellMaxLossCnt\tLimit3Buy\tLimit3Sell\tLimit10Buy\tLimit10Sell\tLimit30Buy\tLimit30Sell\tBuyDrawUp\tBuyDrawDown\tSellDrawUp\tSellDrawDown";
        public static string GetHeader1() => "Cnt\tOpnToCL\tD\tH\tL\tMinDate\tMaxDate\t\t" +
                                             $"R_N1\tD_N1\tH_N1\tL_N1\tUp, %\tDown, %\tBuy EndAmt\tSell EndAmt\t" +
                                             $"Buy Wins,%\tSell Wins,%\tBuyMax LossCnt\tSellMax LossCnt\t" +
                                             $"BuyDraw Up,%\tBuyDraw Down,%\tSellDraw Up,%\tSellDraw Down,%\t" +
                                             $"Limit3Buy\tLimit3Sell\tLimit10Buy\tLimit10Sell\tLimit30Buy\tLimit30Sell\t\t" +
                                             $"BuyDrawUpKey\tBuyDrawDownKey\tSellDrawUpKey\tSellDrawDownKey";
        public static string GetHeader2() => "Cnt\tOpnToCL\tD\tH\tL\t\t" +
                                             $"R_N1\tD_N1\tH_N1\tL_N1\tUp %\tDown %\tBuy EndAmt\tSell EndAmt\t" +
                                             $"Buy Wins,%\tSell Wins,%\tBuyMax LossCnt\tSellMax LossCnt\t" +
                                             $"BuyDraw Up,%\tBuyDraw Down,%\tSellDraw Up,%\tSellDraw Down,%\t" +
                                             $"Limit3Buy\tLimit3Sell\tLimit10Buy\tLimit10Sell\tLimit30Buy\tLimit30Sell\t\t" +
                                             $"BuyDrawUpKey\tBuyDrawDownKey\tSellDrawUpKey\tSellDrawDownKey";
        public static string GetHeader2Old() => "Cnt\tOpenToCL\tD\tH\tL\tR_N1\tD_N1\tH_N1\tL_N1\tBuy_N1\tSell_N1\tBuyAmt\tSellAmt\tBuyWins,%\tSellWins,%\tBuyMaxLossCnt\tSellMaxLossCnt\tLimit3Buy\tLimit3Sell\tLimit10Buy\tLimit10Sell\tLimit30Buy\tLimit30Sell\tBuyDrawUp\tBuyDrawDown\tSellDrawUp\tSellDrawDown";

        public string GetContent1() =>
            $"{Cnt}\t{OpenToClose}\t{Math.Round(D, 1)}\t{Math.Round(H, 2)}\t{Math.Round(L, 2)}\t{MinDate:dd.MM.yyyy}\t{MaxDate:dd.MM.yyyy}\t\t" +
            $"{R_N1}\t{Math.Round(D_N1, 1)}\t{Math.Round(H_N1, 2)}\t{Math.Round(L_N1, 2)}\t" +
            $"{Math.Round(Up_N1, 1)}\t{Math.Round(Down_N1, 1)}\t{BuyEndAmt}\t{SellEndAmt}\t" +
            $"{Math.Round(100.0 * BuyWins / Cnt_N1, 2)}\t{Math.Round(100.0 * SellWins / Cnt_N1, 2)}\t" +
            $"{BuyMaxLossCnt}\t{SellMaxLossCnt}\t" +
            $"{BuyDrawUp * 100.0}\t{100.0 / BuyDrawDown}\t" + $"{SellDrawUp * 100.0}\t{100.0 / SellDrawDown}\t" +
            $"{GetLimitString(Limit3BuyStarted, Limit3BuyProfit, Limit3BuyAmt)}\t" +
            $"{GetLimitString(Limit3SellStarted, Limit3SellProfit, Limit3SellAmt)}\t" +
            $"{GetLimitString(Limit10BuyStarted, Limit10BuyProfit, Limit10BuyAmt)}\t" +
            $"{GetLimitString(Limit10SellStarted, Limit10SellProfit, Limit10SellAmt)}\t" +
            $"{GetLimitString(Limit30BuyStarted, Limit30BuyProfit, Limit30BuyAmt)}\t" +
            $"{GetLimitString(Limit30SellStarted, Limit30SellProfit, Limit30SellAmt)}\t\t" +
            $"{BuyDrawUpKey}\t{BuyDrawDownKey}\t{SellDrawUpKey}\t{SellDrawDownKey}";
        public string GetContent2() =>
            $"{Cnt}\t{OpenToClose}\t{Math.Round(D, 1)}\t{Math.Round(H, 2)}\t{Math.Round(L, 2)}\t\t" +
            $"{R_N1}\t{Math.Round(D_N1, 1)}\t{Math.Round(H_N1, 2)}\t{Math.Round(L_N1, 2)}\t" +
            $"{Math.Round(Up_N1, 1)}\t{Math.Round(Down_N1, 1)}\t{BuyEndAmt}\t{SellEndAmt}\t" +
            $"{Math.Round(100.0 * BuyWins / Cnt_N1, 2)}\t{Math.Round(100.0 * SellWins / Cnt_N1, 2)}\t" +
            $"{BuyMaxLossCnt}\t{SellMaxLossCnt}\t" +
            $"{BuyDrawUp * 100.0}\t{100.0 / BuyDrawDown}\t" + $"{SellDrawUp * 100.0}\t{100.0 / SellDrawDown}\t" +
            $"{GetLimitString(Limit3BuyStarted, Limit3BuyProfit, Limit3BuyAmt)}\t" +
            $"{GetLimitString(Limit3SellStarted, Limit3SellProfit, Limit3SellAmt)}\t" +
            $"{GetLimitString(Limit10BuyStarted, Limit10BuyProfit, Limit10BuyAmt)}\t" +
            $"{GetLimitString(Limit10SellStarted, Limit10SellProfit, Limit10SellAmt)}\t" +
            $"{GetLimitString(Limit30BuyStarted, Limit30BuyProfit, Limit30BuyAmt)}\t" +
            $"{GetLimitString(Limit30SellStarted, Limit30SellProfit, Limit30SellAmt)}\t\t" +
            $"{BuyDrawUpKey}\t{BuyDrawDownKey}\t{SellDrawUpKey}\t{SellDrawDownKey}";
        public string GetContent1Old() =>
            $"{Cnt}\t{OpenToClose}\t{Math.Round(D, 1)}\t{Math.Round(H, 2)}\t{Math.Round(L, 2)}\t{MinDate:dd.MM.yyyy}\t{MaxDate:dd.MM.yyyy}\t{R_N1}\t{Math.Round(D_N1, 1)}\t{Math.Round(H_N1, 2)}\t{Math.Round(L_N1, 2)}\t{Math.Round(Up_N1, 1)}\t{Math.Round(Down_N1, 1)}\t{BuyEndAmt}\t{SellEndAmt}\t{Math.Round(100.0 * BuyWins / Cnt_N1, 2)}\t{Math.Round(100.0 * SellWins / Cnt_N1, 2)}\t{BuyMaxLossCnt}\t{SellMaxLossCnt}\t{GetLimitString(Limit3BuyStarted, Limit3BuyProfit, Limit3BuyAmt)}\t{GetLimitString(Limit3SellStarted, Limit3SellProfit, Limit3SellAmt)}\t{GetLimitString(Limit10BuyStarted, Limit10BuyProfit, Limit10BuyAmt)}\t{GetLimitString(Limit10SellStarted, Limit10SellProfit, Limit10SellAmt)}\t{GetLimitString(Limit30BuyStarted, Limit30BuyProfit, Limit30BuyAmt)}\t{GetLimitString(Limit30SellStarted, Limit30SellProfit, Limit30SellAmt)}\t{BuyDrawUpKey}\t{BuyDrawDownKey}\t{SellDrawUpKey}\t{SellDrawDownKey}";

        public string GetContent2Old() =>
            $"{Cnt}\t{OpenToClose}\t{Math.Round(D, 1)}\t{Math.Round(H, 2)}\t{Math.Round(L, 2)}\t{R_N1}\t{Math.Round(D_N1, 1)}\t{Math.Round(H_N1, 2)}\t{Math.Round(L_N1, 2)}\t{Math.Round(Up_N1, 1)}\t{Math.Round(Down_N1, 1)}\t{BuyEndAmt}\t{SellEndAmt}\t{Math.Round(100.0 * BuyWins / Cnt_N1, 2)}\t{Math.Round(100.0 * SellWins / Cnt_N1, 2)}\t{BuyMaxLossCnt}\t{SellMaxLossCnt}\t{GetLimitString(Limit3BuyStarted, Limit3BuyProfit, Limit3BuyAmt)}\t{GetLimitString(Limit3SellStarted, Limit3SellProfit, Limit3SellAmt)}\t{GetLimitString(Limit10BuyStarted, Limit10BuyProfit, Limit10BuyAmt)}\t{GetLimitString(Limit10SellStarted, Limit10SellProfit, Limit10SellAmt)}\t{GetLimitString(Limit30BuyStarted, Limit30BuyProfit, Limit30BuyAmt)}\t{GetLimitString(Limit30SellStarted, Limit30SellProfit, Limit30SellAmt)}\t{BuyDrawUpKey}\t{BuyDrawDownKey}\t{SellDrawUpKey}\t{SellDrawDownKey}";
        
        private static string DoubleDoString(double amt)
        {
            if (amt < 1) return amt.ToString("F4", CultureInfo.InvariantCulture);
            if (amt < 10) return amt.ToString("F2", CultureInfo.InvariantCulture);
            if (amt < 1000) return amt.ToString("F1", CultureInfo.InvariantCulture);
            if (amt < 1000000) return amt.ToString("F0", CultureInfo.InvariantCulture);
            return amt.ToString("E2", CultureInfo.InvariantCulture);
        }

        private static string GetLimitString(int limitStarted, double limitProfit, double limitAmt) =>
            $"{limitStarted}/{DoubleDoString(limitProfit)}/{DoubleDoString(limitAmt)}";
    }
}
