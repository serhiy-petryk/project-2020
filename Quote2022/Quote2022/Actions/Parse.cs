using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static partial class Parse
    {
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

        #region ========  Eoddata symbols parse + save to db  ========
        public static void SymbolsEoddata_Parse(Action<string> showStatusAction)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                showStatusAction($"SymbolsEoddata. Before parsing procedure");

                conn.Open();
                cmd.CommandTimeout = 150;
                cmd.CommandText =
                    "SELECT a.Symbol, a.Exchange, null as Name, GETDATE() as Date, CONVERT(date, '2022-09-01') Date " +
                    "FROM (SELECT DISTINCT Symbol, Exchange from DayEoddata) a " +
                    "LEFT JOIN SymbolsEoddata b ON a.Symbol = b.Symbol and a.Exchange = b.Exchange " +
                    "WHERE b.Symbol is null";
                cmd.ExecuteNonQuery();

                var cnt = 0;
                var items = new List<SymbolsEoddata>();
                var files = Directory.GetFiles(Settings.SymbolsEoddataFolder);
                Array.Sort(files, StringComparer.InvariantCulture);
                foreach (var file in files)
                {
                    showStatusAction($"DayEoddata file is parsing: {Path.GetFileName(file)}");
                    var ss = Path.GetFileNameWithoutExtension(file).Split('_');
                    var exchange = ss[0].Trim().ToUpper();
                    var date = File.GetCreationTime(file);
                    var lines = File.ReadLines(file);
                    var firstLine = true;

                    // Add data to array (items)
                    foreach (var line in lines)
                    {
                        if (firstLine)
                        {
                            if (!string.Equals(line, "Symbol\tDescription"))
                                throw new Exception($"SymbolsEoddata_Parse error! Please, check the first line of {file} file");
                            firstLine = false;
                        }
                        else if (!string.IsNullOrEmpty(line))
                            items.Add(new SymbolsEoddata(exchange, date, line.Split('\t')));
                    }

                    // Save data to buffer table of data server
                    cnt += items.Count;
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "truncate table Bfr_SymbolsEoddata";
                    cmd.ExecuteNonQuery();

                    SaveToDb.SaveToDbTable(conn, items, "Bfr_SymbolsEoddata", "Symbol", "Exchange", "Name", "Created");
                    items.Clear();

                    // Update data on data server
                    cmd.CommandText = "UPDATE a SET a.Deleted=NULL FROM SymbolsEoddata a " +
                                      "INNER JOIN Bfr_SymbolsEoddata b ON a.Symbol = b.Symbol and a.Exchange = b.Exchange " +
                                      "WHERE a.Deleted is NOT NULL";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE a SET a.Name=b.Name FROM SymbolsEoddata a " +
                                      "INNER JOIN Bfr_SymbolsEoddata b ON a.Symbol = b.Symbol and a.Exchange = b.Exchange " +
                                      "WHERE b.Name is NOT NULL and ISNULL(a.Name, '') <> b.Name";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE a SET a.Deleted=@Date FROM SymbolsEoddata a " +
                                      "LEFT JOIN Bfr_SymbolsEoddata b ON a.Symbol = b.Symbol and a.Exchange = b.Exchange " +
                                      "WHERE a.Exchange = @Exchange and b.Symbol IS NULL and a.Deleted IS NULL";
                    cmd.Parameters.Add(new SqlParameter("@Date", date));
                    cmd.Parameters.Add(new SqlParameter("@Exchange", exchange));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = "INSERT INTO SymbolsEoddata (Symbol, Exchange, Name, Created) " +
                                      "SELECT a.Symbol, a.Exchange, a.Name, a.Created FROM Bfr_SymbolsEoddata a " +
                                      "LEFT JOIN SymbolsEoddata b ON a.Symbol = b.Symbol AND a.Exchange = b.Exchange " +
                                      "WHERE b.Symbol IS NULL";
                    cmd.ExecuteNonQuery();
                }

            }

            showStatusAction($"SymbolsEoddata file parsing finished!!!");
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
