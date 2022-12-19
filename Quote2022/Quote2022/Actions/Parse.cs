using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FastMember;
using Newtonsoft.Json;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static partial class Parse
    {
        #region ==================  ProfileQuantumonline_Parse  ==========================
        public static void SymbolsStockanalysis_ParseAndSaveToDb(Action<string> showStatusAction)
        {
            var filename = @"E:\Quote\WebData\Symbols\Stockanalysis\Stockanalysis.StockExchanges_20221216.txt";
            var oo = JsonConvert.DeserializeObject<string[,]>(File.ReadAllText(filename));
            var items = new List<SymbolsStockanalysisExchanges>();
            for (var i = 0; i < oo.GetLength(0); i++)
                items.Add(new SymbolsStockanalysisExchanges(oo[i, 0], oo[i, 1], File.GetCreationTime(filename)));

            SaveToDb.ClearAndSaveToDbTable(items, "Bfr_SymbolsStockanalysisStockExchanges", "Symbol", "Exchange",
                "Created");

            filename = @"E:\Quote\WebData\Symbols\Stockanalysis\Stockanalysis.EtfExchanges_20221216.txt";
            oo = JsonConvert.DeserializeObject<string[,]>(File.ReadAllText(filename));
            items = new List<SymbolsStockanalysisExchanges>();
            for (var i = 0; i < oo.GetLength(0); i++)
                items.Add(new SymbolsStockanalysisExchanges(oo[i, 0], oo[i, 1], File.GetCreationTime(filename)));

            SaveToDb.ClearAndSaveToDbTable(items, "Bfr_SymbolsStockanalysisEtfExchanges", "Symbol", "Exchange",
                "Created");


        }
        #endregion

        #region ==================  ProfileQuantumonline_Parse  ==========================
        public static void ProfileQuantumonline_ParseAndSaveToDb(string zipFile, Action<string> showStatusAction)
        {
            var profiles = new Dictionary<string, List<ProfilesQuantumonline>>();
            using (var zip = new ZipReader(zipFile))
                foreach (var file in zip.Where(a => a.FullName.EndsWith(".html", true, CultureInfo.InvariantCulture)))
                {
                    var item = new ProfilesQuantumonline(file.Content, file.FileNameWithoutExtension, file.Created);
                    if (!item.IsInvalid)
                    {
                        if (!profiles.ContainsKey(item.SymbolKey))
                            profiles.Add(item.SymbolKey, new List<ProfilesQuantumonline>());
                        profiles[item.SymbolKey].Add(item);

                        //if (profiles.Count > 1000)
                        //    break;
                    }
                }

            var data = profiles.Values.Select(a => a[0]).ToArray();
            SaveToDb.ClearAndSaveToDbTable(data, "ProfilesQuantumonline", "SymbolKey",
                "Exchange", "Symbol", "CUSIP", "Name", "PrevCUSIP", "NewSymbol", "NewSymbolChanged", "NewName",
                "PrevSymbol", "PrevSymbolChanged", "PrevName", "PrevNameChanged", "IpoDate", "IpoSize", "IpoPrice",
                "Type", "SubType", "CapStockType", "MarketCap", "IsDead", "TimeStamp");

        }
        #endregion

        #region ==================  ProfileQuantumonline_Parse  ==========================
        public static void xxProfileQuantumonline_Parse(string zipFile, Action<string> showStatusAction)
        {
            var keys1 = new Dictionary<string, object>();
            var keys2 = new Dictionary<string, object>();

            var cnt = 0;
            var profiles = new Dictionary<string, List<ProfilesQuantumonline>>();
            using (var zip = new ZipReader(zipFile))
                foreach (var file in zip.Where(a => a.FullName.EndsWith(".html", true, CultureInfo.InvariantCulture)))
                {
                    if (file.Content.IndexOf("Not Found!", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        Debug.Print($"Check not found symbol for '{file.FileNameWithoutExtension}' file");
                        // throw new Exception($"Check not found symbol for '{file.FileNameWithoutExtension}' file");
                        continue;
                    }
                    var i1 = file.Content.IndexOf("<table bgcolor=\"#DCFDD7\"", StringComparison.InvariantCultureIgnoreCase);
                    if (i1 < 0) throw new Exception("Check ProfileQuantumonline_Parse procedure");
                    var i2 = file.Content.IndexOf(@"Company's Online Information Links", i1, StringComparison.InvariantCultureIgnoreCase);
                    if (i2 < 0) throw new Exception("Check ProfileQuantumonline_Parse procedure");
                    var i3 = file.Content.Substring(0, i2).LastIndexOf("<p>", StringComparison.InvariantCultureIgnoreCase);
                    if (i3 < 0) throw new Exception("Check ProfileQuantumonline_Parse procedure");

                    var s = file.Content.Substring(i1 + 20, i3 - i1 - 20);

                    // Remove tables
                    i2 = s.IndexOf("</table>", StringComparison.InvariantCultureIgnoreCase);
                    while (i2 != -1)
                    {
                        i1 = s.LastIndexOf("<table", i2, StringComparison.InvariantCultureIgnoreCase);
                        s = s.Substring(0, i1) + s.Substring(i2+8);
                        i2 = s.IndexOf("</table>", StringComparison.InvariantCultureIgnoreCase);
                    }

                    var ss1 = s.Split(new string[] {"</font>"}, StringSplitOptions.RemoveEmptyEntries);
                    var rows = new List<string>();
                    var rowAttributes = new List<string>();
                    /*for (var k = 0; k < ss1.Length; k++)
                    {
                        var s1 = ss1[k];
                        if (s1.Trim().EndsWith("</center>") && s1.IndexOf("Company's Online Profile", StringComparison.InvariantCultureIgnoreCase) == -1)
                        {
                        }
                    }*/
                    foreach (var s1 in ss1)
                    {
//                        if (s1.Trim().EndsWith("</center>") && s1.IndexOf("Company's Online Profile", StringComparison.InvariantCultureIgnoreCase) == -1)
                    //        if (s1.IndexOf("Company's Online Profile", StringComparison.InvariantCultureIgnoreCase) == -1)
                      //  {
                                i1 = s1.LastIndexOf("<font", StringComparison.InvariantCultureIgnoreCase);
                            i2 = s1.IndexOf(">", i1 + 5, StringComparison.InvariantCultureIgnoreCase);
                            var s2 = RemoveTags(s1.Substring(i2 + 1)).Trim();
                            if (!string.IsNullOrEmpty(s2))
                            {
                                rows.Add(s2);
                                var attribute = s1.Substring(i1 + 5, i2 - i1 - 5).Trim();
                                rowAttributes.Add(attribute);
                            }
                       // }
                    }

                    if (rows.Count <2)
                        throw new Exception("Check ProfileQuantumonline_Parse procedure");
                    if (rowAttributes[0].IndexOf("+1", StringComparison.InvariantCulture) == -1)
                        throw new Exception("Check ProfileQuantumonline_Parse procedure");
                    if (rows[1].IndexOf("Ticker Symbol:", StringComparison.InvariantCultureIgnoreCase) == -1)
                        throw new Exception("Check ProfileQuantumonline_Parse procedure");

                    string marketCapRow = null;
                    var otherRowTypes = new string[]
                    {
                        "Company's Online Profile", "Link to IPO Prospectus", "Link to Preliminary IPO Prospectus",
                        "Link to Free Writing IPO Prospectus", "Click for current", "Go to Parent Company's Record",
                        "Find All Related Securities for"
                    };


                    DateTime? ipoDate = null;
                    float? ipoSize = null;
                    float? ipoPrice = null;
                    string newSymbol = null;
                    DateTime? newSymbolChanged = null;
                    string newName = null;
                    string prevSymbol = null;
                    DateTime? prevSymbolChanged = null;
                    string prevName = null;
                    DateTime? prevNameChanged = null;
                    string type = null;
                    string subType = null;
                    DateTime? changed = null; // ????
                    string capStockType = null;
                    string sMarketCap = null;

                    for (var k1 = 2; k1 < rows.Count; k1++)
                    {
                        var row = rows[k1];
                        var xsymbol = file.FileNameWithoutExtension.Substring(1);
                        if (row.StartsWith("IPO -", StringComparison.CurrentCultureIgnoreCase)) //IPO - 2/2/2021 - 87.00 Million Units @ $10.00/unit.
                        {
                        }
                        else if (row.EndsWith("/share.")) //6.67 Million Shares @ $15.00/share. (ABDC*)
                        {
                        }
                        else if (row.EndsWith("/ADR.")) //11.43 Million ADRs @ $17.50/ADR. (ABLX*)
                        {
                        }
                        else if (row.EndsWith("/unit.")) //7.50 Million Units @ $10.00/unit. (ACAXU)
                        {
                        }
                        else if (row.EndsWith("/note.")) //Free Writing Prospectus Filed - 10/8/2020 - 16.00 Million Notes @ $25/note. (BAMH)
                        {
                        }
                        else if (row.IndexOf("Market Value", StringComparison.CurrentCultureIgnoreCase) != -1) // Large Cap Stock -&nbsp;&nbsp; \n Market Value $22.5 Billion
                        {
                            // marketCapRow = row;

                            var ss = row.Split(new string[] { "&nbsp;" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var s1 in ss)
                            {
                                var s2 = s1.Trim();
                                if (s2.StartsWith("Market Value"))
                                    sMarketCap = s2.Substring(12).Trim();
                                else if (s2.EndsWith(" Cap Stock -"))
                                    capStockType = s2.Substring(0, s2.Length - 12).Trim();
                                else if (string.Equals(s2, "Cap Stock -")) { }
                                else
                                    throw new Exception("Check 'Market Value' in ProfileQuantumonline_Parse procedure");
                            }
                            Debug.Print($"CapStock: {capStockType}. Market cap: {sMarketCap}");
                        }

                        else if (row.StartsWith("Security Type:")) //Security Type:&nbsp;&nbsp; /n <a href="listwipo.cfm?type=SPACs&RequestTimeout=60">Special Purpose Acquisition Company (SPAC)</a>
                        {
                            var ss = row.Split(new string[] { "&nbsp;" }, StringSplitOptions.RemoveEmptyEntries);
                            for (var k = 0; k < ss.Length; k++)
                                ss[k] = ss[k].Trim();
                            if (ss.Length >= 2 && ss[0] == "Security Type:" && ss[1].EndsWith("</a>"))
                            {
                                ss[1] = ss[1].Substring(0, ss[1].Length - 4);
                                i1 = ss[1].LastIndexOf(">", StringComparison.InvariantCultureIgnoreCase);
                                type = ss[1].Substring(i1 + 1);
                            }
                            
                            if (ss.Length == 5 && ss[3].EndsWith("ETF SubType:") && ss[4].EndsWith("</a>"))
                            {
                                ss[4] = ss[4].Substring(0, ss[4].Length - 4);
                                i1 = ss[4].LastIndexOf(">", StringComparison.InvariantCultureIgnoreCase);
                                subType = ss[4].Substring(i1 + 1);
                            }
                            else if (ss.Length >= 4)
                                throw new Exception("Check 'Security Type:' in ProfileQuantumonline_Parse procedure");

                            if (string.IsNullOrEmpty(type))
                                throw new Exception("Check 'Security Type:' in ProfileQuantumonline_Parse procedure");

                            // Debug.Print($"SecType: {type}. SubType: {subType}");
                        }

                        else if (row.StartsWith("* Symbol changed!")) //* Symbol changed!! New symbol: <a href="search.cfm?tickersymbol=SCOK&sopt=symbol">SCOK</a> \nas of 2/03/2010
                        {
                            i2 = row.LastIndexOf("</a>", StringComparison.InvariantCultureIgnoreCase);
                            i1 = row.LastIndexOf(">", i2, StringComparison.InvariantCultureIgnoreCase);
                            newSymbol = row.Substring(i1 + 1, i2 - i1 - 1);

                            i1 = row.LastIndexOf("as of ", StringComparison.InvariantCultureIgnoreCase);
                            if (i1 != -1)
                                newSymbolChanged = DateTime.ParseExact(row.Substring(i1 + 6), "M/dd/yyyy", CultureInfo.InvariantCulture);
                        }

                        else if (row.StartsWith("New Company Name:")) // New Company Name: SinoCoking Coal and Coke Chemical Industries, Inc.
                        {
                            newName = row.Substring(17).Trim();
                            // Debug.Print($"NewName: {newName}");
                        }

                        else if (row.StartsWith("Previous Ticker Symbol:")) // Previous Ticker Symbol: ABLC &nbsp;&nbsp;&nbsp;Changed: 6/29/2000
                        {
                            var ss = row.Split(new string[] {"&nbsp;"}, StringSplitOptions.RemoveEmptyEntries);
                            prevSymbol = ss[0].Substring(23).Trim();
                            if (ss.Length == 2 && ss[1].StartsWith("Changed:"))
                                prevSymbolChanged = DateTime.ParseExact(ss[1].Substring(8).Trim(), "M/dd/yyyy", CultureInfo.InvariantCulture);
                            else if (ss.Length != 1)
                                throw new Exception("Check 'Previous Ticker Symbol:' in ProfileQuantumonline_Parse procedure");
                            // Debug.Print($"PrevSymbol: {prevSymbol} at {prevSymbolChanged:dd.MM.yyyy}");
                        }

                        else if (row.StartsWith("Previous Name:")) // //Previous Name: FelCor Lodging Trust, $1.95 Series A Cumul Convertible Preferred Stock &nbsp;&nbsp;&nbsp;Changed: 9/01/2017 
                        {
                            var ss = row.Split(new string[] { "&nbsp;" }, StringSplitOptions.RemoveEmptyEntries);
                            prevName = ss[0].Substring(14).Trim();
                            if (ss.Length == 2 && ss[1].StartsWith("Changed:"))
                                prevNameChanged = DateTime.ParseExact(ss[1].Substring(8).Trim(), "M/dd/yyyy", CultureInfo.InvariantCulture);
                            else if (ss.Length != 1)
                                throw new Exception("Check 'Previous Name:' in ProfileQuantumonline_Parse procedure");
                            // Debug.Print($"PrevName: {prevName} at {prevNameChanged:dd.MM.yyyy}");
                        }

                        else if (row.StartsWith("&nbsp;&nbsp;&nbsp;Changed:")) // &nbsp;&nbsp;&nbsp;Changed: 7/24/2000 (APW*)
                        {
                            var ss = row.Split(new string[] { "&nbsp;" }, StringSplitOptions.RemoveEmptyEntries);
                            if (ss.Length == 1 && ss[0].StartsWith("Changed:"))
                                changed = DateTime.ParseExact(ss[0].Substring(8).Trim(), "M/dd/yyyy", CultureInfo.InvariantCulture);
                            else
                                throw new Exception("Check 'Chenged:' in ProfileQuantumonline_Parse procedure");
                        }
                        else if (row.StartsWith("ADR with an ADR ratio")) { }
                        else if (row.StartsWith("Security has been ") && row.IndexOf("Called for:", StringComparison.InvariantCultureIgnoreCase)!=-1) { }//Security has been [Partially] Called for
                        else if (row.StartsWith("* NOTE:")) { }
                        else if (row.StartsWith("Preliminary Prospectus Filed")) { }
                        else if (row.StartsWith("Security's Distribution is Suspended!")) { }
                        else if (row.StartsWith("A tender offer has been")) { }
                        else if (otherRowTypes.Count(a => row.IndexOf(a, StringComparison.CurrentCultureIgnoreCase) != -1) != 0) { }
                        else
                            throw new Exception("Check ProfileQuantumonline_Parse procedure");
                    }

                    var name = rows[0];

                    string symbol = null;
                    string cusip = null;
                    string exchange = null;
                    string prevCusip = null;
                    var ss2 = rows[1].Split(new string[] {"&nbsp;"}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s2 in ss2)
                    {
                        var k = s2.IndexOf(':');
                        var key = s2.Substring(0, k).Trim();
                        var value = s2.Substring(k+1).Trim();
                        if (key == "Ticker Symbol")
                            symbol = value;
                        else if (key == "Exchange")
                            exchange = value;
                        else if (key == "CUSIP")
                            cusip = value;
                        else if (key == "Previous CUSIP")
                            prevCusip = value;
                        else
                            throw new Exception("Check 'Ticker Symbol' in ProfileQuantumonline_Parse procedure");
                    }

                    if (string.IsNullOrEmpty(exchange) && !string.Equals(exchange, null))
                        exchange = null;

                    if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(cusip))
                        throw new Exception("Check 'Ticker Symbol' in ProfileQuantumonline_Parse procedure");

                    //if (changed.HasValue)
                    //  Debug.Print($"Change for {file.FileNameWithoutExtension.Substring(1)}. NewSymbol {newSymbol} at {newSymbolChanged:dd.MM.yyyy}. NewName: {newName}. PrevSymbol {prevSymbol} at {prevSymbolChanged:dd.MM.yyyy}. PrevName: {prevName} at {prevNameChanged:dd.MM.yyyy}. Changed:{changed.Value:dd.MM.yyyy}");

                    if (!string.Equals(marketCapRow, null))
                    {
                        var a2 = RemoveTags(marketCapRow).Split(new string[] {"&nbsp;"}, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var aa1 in a2)
                        {
                            var aa2 = aa1.Trim();
                            var key2 = aa2.StartsWith("Market Value") ? "Market Value" : aa2;
                            if (!keys2.ContainsKey(key2))
                                keys2.Add(key2, null);
                        }
                    }

                }

            foreach (var x1 in keys1.Keys)
                Debug.Print($"Key1: {x1}");
            foreach (var x1 in keys2.Keys)
                Debug.Print($"Key2: {x1}");

            string RemoveTags(string o)
            {
                var o1 = o.Replace("<p>", "").Replace("<br>", "").Replace("<b>", "").Replace("</b>", "").Replace("<center>", "").Replace("</center>", "");
                //if (o1.IndexOf('>')!=-1 || o1.IndexOf('<') != -1)
                  //  throw new Exception("Check RemoveTags method in ProfileQuantumonline_Parse");
                return o1;
            }
        }
        #endregion

        #region ==================  SymbolsQuantumonline_Parse  ==========================
        public static void SymbolsQuantumonlineZip_CheckMinLevel(Action<string> showStatusAction)
        {
            var zipFile = @"E:\Quote\WebData\Symbols\Quantumonline\QuantumonlineListNew.zip";

            var cnt = 0;
            var urls = new Dictionary<string, List<SymbolsQuantumonline>>();
            using (var zip = new ZipReader(zipFile))
            {
                foreach (var file in zip.Where(a => a.FullName.EndsWith(".html", true, CultureInfo.InvariantCulture)))
                {
                    if (file.FileNameWithoutExtension.Replace("%", "[%]").Substring(1).Length > 20)
                    {
                        // var item = SymbolsQuantumonlineContent_Parse(file.Content, file.FileNameWithoutExtension);
                        // Debug.Print(item == null ? "NULL" : item.ToString());
                    }
                    else
                    {
                        var newItems = SymbolsQuantumonlineContent_Parse(file.Content, file.FileNameWithoutExtension, File.GetCreationTime(zipFile));
                        cnt += newItems.Count;
                        foreach (var item in newItems)
                        {
                            if (item.MinLevel > (file.FileNameWithoutExtension.Length - 2))
                                item.MinLevel = file.FileNameWithoutExtension.Length - 2;
                            if (!urls.ContainsKey(item.SymbolKey))
                                urls.Add(item.SymbolKey, new List<SymbolsQuantumonline>());
                            urls[item.SymbolKey].Add(item);
                        }
                    }
                }
            }

            var aa1 = urls.ToDictionary(a => a.Key, a => a.Value.Min(a1 => a1.MinLevel));
            var aa2 = aa1.GroupBy(a => a.Value).Select(a => new Tuple<int, int>(a.Key, a.Count())).OrderBy(a => a.Item1).ToArray();

            Debug.Print($"Item count: {cnt}. File: {Path.GetFileName(zipFile)}");
        }
        #endregion

        #region ==================  SymbolsQuantumonline_Parse  ==========================
        public static void SymbolsQuantumonlineZip_Parse(Action<string> showStatusAction)
        {
            var items = new Dictionary<string, List<SymbolsQuantumonline>>();
            var file1 = @"E:\Quote\WebData\Symbols\Quantumonline\QuantumonlineListNew.zip";
            SymbolsQuantumonline_GetUrls(file1, items);
            Debug.Print($"Items 1: {items.Count}");

            file1 = @"E:\Quote\WebData\Symbols\Quantumonline\QuantumonlineList.zip";
            SymbolsQuantumonline_GetUrls(file1, items, true);
            Debug.Print($"Items 2: {items.Count}");

            file1 = @"E:\Quote\WebData\Symbols\Quantumonline\QuantumonlineListOld.zip";
            SymbolsQuantumonline_GetUrls(file1, items, true);
            Debug.Print($"Items 3: {items.Count}");

            SaveToDb.SymbolsQuantumonline_SaveToDb(items.Select(a=>a.Value[0]));
        }

        public static void SymbolsQuantumonline_GetUrls(string zipFile, Dictionary<string, List<SymbolsQuantumonline>> urls, bool trace = false)
        {
            var cnt = 0;
            // var urls = new Dictionary<string, string>();
            using (var zip = new ZipReader(zipFile))
            {
                foreach (var file in zip.Where(a => a.FullName.EndsWith(".html", true, CultureInfo.InvariantCulture)))
                {
                    if (file.FileNameWithoutExtension.Replace("%", "[%]").Substring(1).Length > 20)
                    {
                        // var item = SymbolsQuantumonlineContent_Parse(file.Content, file.FileNameWithoutExtension);
                        // Debug.Print(item == null ? "NULL" : item.ToString());
                    }
                    else
                    {
                        var newItems = SymbolsQuantumonlineContent_Parse(file.Content, file.FileNameWithoutExtension, File.GetCreationTime(zipFile));
                        cnt += newItems.Count;
                        foreach (var item in newItems)
                        {
                            if (!urls.ContainsKey(item.Url))
                                urls.Add(item.Url, new List<SymbolsQuantumonline>());
                            urls[item.Url].Add(item);
                        }

                        //if (cnt > 100)
                          // break;
                    }
                }
            }

            Debug.Print($"Item count: {cnt}. File: {Path.GetFileName(zipFile)}");
        }

        private static void xxSymbolsQuantumonline_ParseZipFile(string zipFile, Dictionary<string, SymbolsQuantumonline> items, bool trace = false)
        {
            var cnt = 0;
            var urls = new Dictionary<string, string>();
            using (var zip = new ZipReader(zipFile))
            {
                foreach (var file in zip.Where(a => a.FullName.EndsWith(".html", true, CultureInfo.InvariantCulture)))
                {
                    if (file.FileNameWithoutExtension.Replace("%", "[%]").Substring(1).Length > 20)
                    {
                        // var item = SymbolsQuantumonlineContent_Parse(file.Content, file.FileNameWithoutExtension);
                        // Debug.Print(item == null ? "NULL" : item.ToString());
                    }
                    else
                    {
                        var newItems = SymbolsQuantumonlineContent_Parse(file.Content, file.FileNameWithoutExtension, File.GetCreationTime(zipFile));
                        cnt += newItems.Count;
                        foreach (var item in newItems)
                        {
                            if (!items.ContainsKey(item.ToString()))
                            {
                                if (item.Symbol == "EXTN" || item.Symbol == "BAM")
                                {
                                    continue;
                                }

                                items.Add(item.ToString(), item);
                                urls.Add(item.SymbolKey, null);

                                /*if (trace || string.Equals(item.Symbol, "IBMK")
                                          || string.Equals(item.Symbol, "BKFPF")
                                          || string.Equals(item.Symbol, "BROXF")
                                          || string.Equals(item.Symbol, "ACEV")
                                          || string.Equals(item.Symbol, "ACEVW")
                                          || string.Equals(item.Symbol, "ACEVU")
                                          || string.Equals(item.Symbol, "BAM")
                                          || string.Equals(item.Symbol, "VCKAU")
                                )*/
                                if (trace)
                                    Debug.Print($"New item: {item.ToString()}. File: {file.FileNameWithoutExtension}. ZipFile: {zipFile}");
                            }
                        }
                    }
                }
            }

            Debug.Print($"Item count: {cnt}. File: {Path.GetFileName(zipFile)}");
        }

        #endregion

        #region ========  SymbolsQuantumonline_Parse  ========
        public static List<SymbolsQuantumonline> SymbolsQuantumonlineContent_Parse(string content, string fileName, DateTime timeStamp)
        {
            var items = new List<SymbolsQuantumonline>();
            if (content.Contains("Matching Security Names for"))
            {
                var i1 = content.IndexOf("Matching Security Names for", StringComparison.InvariantCulture);
                var i2 = content.IndexOf("</table", i1 + 10, StringComparison.InvariantCulture);
                var rows = content.Substring(i1, i2 - i1).Split(new string[] { "</tr>" }, StringSplitOptions.None);
                for (var k = 1; k < rows.Length - 1; k++)
                {
                    var row = rows[k].Trim();
                    if (string.IsNullOrEmpty(row)) continue;
                    var cells = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);

                    var symbol = cells[0].Replace("</font>", "").Replace("</b>", "").Replace("</a>", "").Trim();
                    i1 = symbol.LastIndexOf('>');
                    symbol = symbol.Substring(i1 + 1).Trim();

                    var name = cells[1].Replace("</font>", "").Trim();
                    i1 = name.LastIndexOf('>');
                    name = name.Substring(i1 + 1).TrimStart();
                    /*var ss = name.Split(new[] {"More results"}, StringSplitOptions.RemoveEmptyEntries);
                    if (ss.Length != 1)
                    {

                    }
                    name = ss[ss.Length - 1].Trim();*/
                    //i1 = name.IndexOfAny(new char[] { '\r', '\n' });
                    //if (i1 != -1)
                    //  name = name.Substring(0, i1);

                    var exchange = cells[2].Replace("</font>", "").Replace("</b>", "").Trim();
                    i1 = exchange.LastIndexOf('>');
                    exchange = exchange.Substring(i1 + 1).Trim();

                    i1 = cells[0].IndexOf("href=\"", StringComparison.InvariantCulture);
                    i2 = cells[0].IndexOf("\"", i1 + 6, StringComparison.InvariantCulture);
                    var url = cells[0].Substring(i1 + 6, i2 - i1 - 6);

                    items.Add(new SymbolsQuantumonline(symbol, exchange, name, url, timeStamp));
                }
            }
            else if (content.Contains("Sorry, but there were no matches for")) { }
            else if (content.Contains("An error occurred while executing the application")) { }
            else
                // return null;
                throw new Exception($"Check Parse action for QuantumonlineSymbols in {fileName} file");

            return items;
        }
        #endregion

        #region ========  Nasdaq Stock Screener Parse & SaveToDB  ========
        public static void ScreenerNasdaq_Parse(string file, Action<string> showStatusAction)
        {
            showStatusAction($"ScreenerNasdaq file parsing & save to database started.");

            var ss = Path.GetFileNameWithoutExtension(file).Split('_');
            var timeStamp = DateTime.ParseExact(ss[ss.Length - 1].Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
            var items = new List<ScreenerNasdaq>();
            var lines = File.ReadAllLines(file, Encoding.Default); // or Encoding.UTF7
            if (lines.Length == 0 || !Equals(lines[0], "Symbol,Name,Last Sale,Net Change,% Change,Market Cap,Country,IPO Year,Volume,Sector,Industry"))
                throw new Exception($"Invalid Nasdaq stock screener file structure! {file}");

            string lastLine = null;
            for (var k = 1; k < lines.Length; k++)
            {
                if (Equals(lastLine, lines[k])) continue;
                items.Add(new ScreenerNasdaq(timeStamp, lines[k]));
            }

            if (items.Count > 0)
                SaveToDb.ScreenerNasdaq_SaveToDb(items, timeStamp);

            showStatusAction($"ScreenerNasdaq file parsing & save to database FINISHED!!!");
        }
        #endregion

        #region ========  Check Eoddata daily database  ========
        public static void DayEoddata_Check(Action<string> showStatusAction)
        {
            var badLines = new List<string>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;

                    var files = Directory.GetFiles(Settings.DayEoddataFolder);
                    foreach (var file in files)
                    {
                        showStatusAction($"DayEoddata file is checking: {Path.GetFileName(file)}");

                        var ss = Path.GetFileNameWithoutExtension(file).Split('_');
                        var exchange = ss[0];
                        var date = DateTime.ParseExact(ss[1], "yyyyMMdd", CultureInfo.InvariantCulture);
                        cmd.CommandText = $"SELECT * from DayEoddata WHERE Exchange='{exchange}' and date='{date:yyyy-MM-dd}' order by Symbol";
                        var dbLines = new List<string>();
                        using (var rdr = cmd.ExecuteReader())
                            while (rdr.Read())
                            {
                                var o = new DayEoddata(rdr);
                                dbLines.Add(
                                    $"{o.Symbol},{o.Date:yyyyMMdd},{o.Open.ToString(CultureInfo.InvariantCulture)},{o.High.ToString(CultureInfo.InvariantCulture)},{o.Low.ToString(CultureInfo.InvariantCulture)},{o.Close.ToString(CultureInfo.InvariantCulture)},{o.Volume.ToString("F0", CultureInfo.InvariantCulture)}");
                            }

                        string content = null;
                        using (var _zip = new ZipReader(file))
                        {
                            var fileContents = _zip.Select(a => a.Content).ToArray();
                            if (fileContents.Length == 1)
                                content = fileContents[0];
                            else
                                throw new Exception($"Error in zip file structure: {file}");
                        }
                        var fileLines = new List<string>();
                        var lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        string prevLine = null; // To ignore dublicates
                        foreach (var line in lines)
                        {
                            if (!string.Equals(line, prevLine))
                                fileLines.Add(line);
                            prevLine = line;
                        }

                        if (dbLines.Count != fileLines.Count)
                            throw new Exception($"Number of lines for {file} file don't match. Db lines:{dbLines.Count}, File lines:{fileLines.Count} ");
                        dbLines.Sort();
                        fileLines.Sort();
                        for (var k = 0; k < dbLines.Count; k++)
                        {
                            if (!string.Equals(dbLines[k], fileLines[k]))
                            {
                                Debug.Print(Path.GetFileNameWithoutExtension(file) + "\t" + dbLines[k]);
                                Debug.Print(Path.GetFileNameWithoutExtension(file) + "\t" + fileLines[k]+"\n");
                                badLines.Add(Path.GetFileNameWithoutExtension(file) + "\t" + dbLines[k]);
                                badLines.Add(Path.GetFileNameWithoutExtension(file) + "\t" + fileLines[k]);
                            }
                        }
                        /*var dbContent = string.Join(Environment.NewLine, dbLines);
                        var fileContent = string.Join(Environment.NewLine, fileLines);
                        if (dbContent!=fileContent)
                            throw new Exception($"Data of {file} file don't match");*/
                    }
                }

            }

            if (badLines.Count>0)
                showStatusAction($"Found {badLines.Count/2} don't matched lines. See Debug Output in Visual Studio for details");
            else
                showStatusAction($"DayEoddata file check finished!!!");
        }
        #endregion

        #region ========  Eoddata daily parse + save to db  ========
        public static IEnumerable<DayEoddata> DayEoddata_Data(Action<string> showStatusAction)
        {
            var saved = new Dictionary<Tuple<string, string>, string>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                showStatusAction($"DayEoddata. Before parsing procedure");
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "SELECT distinct Exchange, date from DayEoddata";
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            saved.Add(new Tuple<string, string>((string)rdr["Exchange"],
                                    ((DateTime)rdr["Date"]).ToString("yyyyMMdd", CultureInfo.InvariantCulture)), null);
                }
            }

            var files = Directory.GetFiles(Settings.DayEoddataFolder);
            foreach (var file in files)
            {
                var ss = Path.GetFileNameWithoutExtension(file).Split('_');
                var key = new Tuple<string, string>(ss[0].ToUpper(), ss[1]);
                if (!saved.ContainsKey(key))
                {
                    showStatusAction($"DayEoddata file is parsing: {Path.GetFileName(file)}");
                    string content = null;
                    using (var _zip = new ZipReader(file))
                    {
                        var fileContents = _zip.Select(a => a.Content).ToArray();
                        if (fileContents.Length == 1)
                            content = fileContents[0];
                        else
                            throw new Exception($"Error in zip file structure: {file}");
                    }

                    var lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    string prevLine = null; // To ignore dublicates
                    foreach (var line in lines)
                    {
                        if (!string.Equals(line, prevLine))
                        {
                            prevLine = line;
                            yield return new DayEoddata(ss[0], line.Split(','));
                        }
                    }
                }

            }
            showStatusAction($"Data is saving to database ... Wait, please.");
        }
        #endregion

        #region ==========  Symbols Nanex Parse =============
        public static void SymbolsNanex_Parse(string[] files, List<SymbolsNanex> data, Action<string> showStatusAction)
        {
            foreach (var file in files)
            {
                var exchange = Path.GetFileNameWithoutExtension(file).Split('_')[0];
                var s = File.ReadAllText(file, Settings.Encoding);
                var created = File.GetLastWriteTime(file);
                var i1 = s.IndexOf("<table", StringComparison.Ordinal);
                var i2 = s.IndexOf(@"</table>", StringComparison.Ordinal);
                if (i1 > 0 && i2 > i1)
                {
                    var ss = s.Substring(i1, i2 - i1).Split(new string[] { "<tr" }, StringSplitOptions.RemoveEmptyEntries);
                    var uniqueIDs = new List<string>();
                    for (var i = 0; i < ss.Length; i++)
                    {
                        string[] ss1 = ss[i].Split(new string[] { "<td" }, StringSplitOptions.RemoveEmptyEntries);
                        if (ss1.Length == 8)
                        {
                            string exchange1 = GetTDElement(ss1[1]);
                            string type = GetTDElement(ss1[2]);
                            if (type.ToLower() != "type")
                            {// not header string
                                string symbol = GetTDElement(ss1[3]);
                                string name = GetTDElement(ss1[4]);
                                string activity = GetTDElement(ss1[5]);
                                string lastQuoteDate = GetTDElement(ss1[6]);
                                string lastTradeDate = GetTDElement(ss1[7]);
                                var e = new SymbolsNanex(exchange, type, symbol, name, activity, lastQuoteDate, lastTradeDate, created);
                                if (!e.IsTest)
                                {
                                    var uniqueID = (symbol + "\t" + exchange).ToUpper();
                                    if (uniqueIDs.Contains(uniqueID))
                                    {// check on uniqueID
                                        int k = uniqueIDs.IndexOf(uniqueID);
                                        var e1 = data[k];
                                        if ((e1.LastQuoteDate ?? DateTime.MinValue) < (e.LastQuoteDate ?? DateTime.MinValue))
                                        {
                                            data.RemoveAt(k);
                                            uniqueIDs.RemoveAt(k);
                                            data.Add(e);
                                            uniqueIDs.Add(uniqueID);
                                        }
                                    }
                                    else
                                    {
                                        data.Add(e);
                                        uniqueIDs.Add(uniqueID);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                    throw new Exception("Invalid Nanex symbol file context. Filename: " + file);
            }

            string GetTDElement(string s)
            {
                var s1 = s.Trim();
                if (s1.ToLower().EndsWith("</tr>")) s1 = s1.Substring(0, s1.Length - 5);
                var i1 = s1.LastIndexOf("<", StringComparison.Ordinal);
                var i2 = s1.Substring(0, i1).LastIndexOf(">", StringComparison.Ordinal);
                if (i2 > i1)
                    throw new Exception("Invalid TD token for Nanex symbol file. Token value: " + s);
                else
                    return s1.Substring(i2 + 1, i1 - i2 - 1).Trim();
            }
        }
        #endregion

        public static void DayYahooIndexes(Action<string> showStatusAction)
        {
            var data = new List<DayYahoo>();
            var cnt = 0;
            var files = Directory.GetFiles(Settings.DayYahooIndexesFolder);
            foreach (var file in files)
            {
                showStatusAction($"DayYahoo index file is parsing: {Path.GetFileName(file)}");
                var lines = File.ReadAllLines(file);
                if (lines.Length == 0)
                    throw new Exception($"Invalid Day Yahoo quote file (no text lines): {file}");
                if (lines[0] != "Date,Open,High,Low,Close,Adj Close,Volume")
                    throw new Exception($"Invalid Day Yahoo quote file (check file header): {file}");
                var symbol = Path.GetFileNameWithoutExtension(file).Split('_')[0];

                for (var k = 1; k < lines.Length; k++)
                {
                    if (!String.IsNullOrEmpty(lines[k].Trim()))
                    {
                        if (lines[k].Contains("null"))
                            Debug.Print($"{symbol}, {lines[k]}");
                        else
                        {
                            var ss = lines[k].Split(',');
                            var item = new DayYahoo(symbol, ss);
                            data.Add(item);
                            cnt++;
                        }
                    }
                }
            }

            if (data.Count > 0)
            {
                SaveToDb.IndexesYahoo_SaveToDb(data);
                data.Clear();
            }

            showStatusAction($"Updating TradingDays table in database");
            SaveToDb.RefreshTradingDays();
            showStatusAction($"DayYahoo file parsing finished!!!");
        }

        #region ========  Yahoo daily parse + save to db  ========
        public static void DayYahoo_Parse(string zipfile, Action<string> showStatusAction)
        {
            var data = new List<DayYahoo>();
            var clearTable = true;
            var cnt = 0;
            using (var zip = new ZipReader(zipfile))
                foreach (var file in zip)
                {
                    if (file.Length > 0 && file.FileNameWithoutExtension.ToUpper().StartsWith("Y"))
                    {
                        showStatusAction($"DayYahoo file is parsing: {file.FullName}");
                        var lines = file.Content.Split('\n');
                        if (lines.Length == 0)
                            throw new Exception($"Invalid Day Yahoo quote file (no text lines): {file}");
                        if (lines[0] != "Date,Open,High,Low,Close,Adj Close,Volume")
                            throw new Exception($"Invalid Day Yahoo quote file (check file header): {file}");
                        var symbol = file.FileNameWithoutExtension.Substring(1);
                        // if (symbol == "TOPS") continue; // TOPS & GMGI have big prices

                        for (var k = 1; k < lines.Length; k++)
                        {
                            if (!String.IsNullOrEmpty(lines[k].Trim()))
                            {
                                if (lines[k].Contains("null"))
                                    Debug.Print($"{symbol}, {lines[k]}");
                                else
                                {
                                    var ss = lines[k].Split(',');
                                    var item = new DayYahoo(symbol, ss);
                                    data.Add(item);
                                    cnt++;
                                }
                            }
                        }

                        if (data.Count > 100000)
                        {
                            showStatusAction($"DayYahoo file is saving to DB: {data.Count:N0} ({cnt:N0}) records");
                            SaveToDb.DayYahoo_SaveToDb(data, clearTable);
                            clearTable = false;
                            data.Clear();
                        }
                    }
                }


            /*var files = Directory.GetFiles(Settings.DayYahooFolder);
            foreach (var file in files)
            {
                showStatusAction($"DayYahoo file is parsing: {Path.GetFileName(file)}");
                var lines = File.ReadAllLines(file);
                if (lines.Length == 0)
                    throw new Exception($"Invalid Day Yahoo quote file (no text lines): {file}");
                if (lines[0] != "Date,Open,High,Low,Close,Adj Close,Volume")
                    throw new Exception($"Invalid Day Yahoo quote file (check file header): {file}");
                var symbol = Path.GetFileNameWithoutExtension(file).Substring(1);
                // if (symbol == "TOPS") continue; // TOPS & GMGI have big prices

                for (var k = 1; k < lines.Length; k++)
                {
                    if (!String.IsNullOrEmpty(lines[k].Trim()))
                    {
                        if (lines[k].Contains("null"))
                            Debug.Print($"{symbol}, {lines[k]}");
                        else
                        {
                            var ss = lines[k].Split(',');
                            var item = new DayYahoo(symbol, ss);
                            data.Add(item);
                            cnt++;
                        }
                    }
                }

                if (data.Count > 100000)
                {
                    showStatusAction($"DayYahoo file is saving to DB: {data.Count:N0} ({cnt:N0}) records");
                    SaveToDb.DayYahoo_SaveToDb(data, clearTable);
                    clearTable = false;
                    data.Clear();
                }
            }*/

            if (data.Count > 0)
            {
                SaveToDb.DayYahoo_SaveToDb(data, clearTable);
                data.Clear();
            }

            showStatusAction($"DayYahoo file parsing finished!!!");
        }
        #endregion
    }
}
