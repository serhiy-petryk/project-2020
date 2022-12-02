using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022
{
    public static class Parse
    {
        #region ========  Nasdaq Stock Screener Parse & SaveToDB  ========
        public static void ScreenerNasdaq_Parse(string file, Action<string> showStatusAction)
        {
            showStatusAction($"ScreenerNasdaq file parsing & save to database started.");
            var items = new List<ScreenerNasdaq>();

            var lines = File.ReadAllLines(file, Encoding.Default); // or Encoding.UTF7
            if (lines.Length == 0 || !Equals(lines[0], "Symbol,Name,Last Sale,Net Change,% Change,Market Cap,Country,IPO Year,Volume,Sector,Industry"))
                throw new Exception($"Invalid Nasdaq stock screener file structure! {file}");

            string lastLine = null;
            for (var k = 1; k < lines.Length; k++)
            {
                if (Equals(lastLine, lines[k])) continue;
                items.Add(new ScreenerNasdaq(lines[k]));
            }

            var ss = Path.GetFileNameWithoutExtension(file).Split('_');
            var timeStamp = DateTime.ParseExact(ss[ss.Length - 1].Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
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

        #region ========  Eoddata split parse + save to db  ========
        public static void SplitEoddata_Parse(string file, Action<string> showStatusAction)
        {
            showStatusAction($"SplitEoddata file parsing started.");
            var splits = new List<object[]>();

            var lines = File.ReadAllLines(file, Encoding.Default); // or Encoding.UTF7
            if (lines.Length == 0 || !Equals(lines[0], "Exchange\tSymbol\tDate\tRatio"))
                throw new Exception($"Invalid Eoddata split file structure! {file}");

            string lastLine = null;
            for (var k = 1; k < lines.Length; k++)
            {
                if (Equals(lastLine, lines[k])) continue;

                var ss = lines[k].Split('\t');
                if (ss.Length != 4)
                    throw new Exception($"Invalid line of Eoddata split file! Line: {lines[k]}, file: {file}");
                var date = DateTime.Parse(ss[2].Trim(), CultureInfo.InvariantCulture);

                splits.Add(new object[] { ss[0].Trim(), ss[1].Trim(), date, ss[3].Trim().Replace('-',':')});
                lastLine = lines[k];
            }

            var timeStamp = DateTime.ParseExact(Path.GetFileNameWithoutExtension(file).Split('_')[1].Trim(), "yyyyMMdd",
                CultureInfo.InvariantCulture);
            if (splits.Count > 0)
                SaveToDb.SplitEoddata_SaveToDb(splits, timeStamp);

            showStatusAction($"SplitEoddata file parsing finished and saved to DB!!! {Path.GetFileName(file)}");
        }
        #endregion

        #region ========  Investing.com split parse + save to db  ========
        // Url: https://www.investing.com/stock-split-calendar/ => copy data to Excel => to text file
        public static void SplitInvesting_Parse(string file, Action<string> showStatusAction)
        {
            showStatusAction($"SplitInvesting file parsing started.");
            var splits = new List<object[]>();

            var lines = File.ReadAllLines(file, Encoding.Default); // or Encoding.UTF7
            if (lines.Length == 0 || !Equals(lines[0], "Split date\tCompany\tSplit ratio"))
                throw new Exception($"Invalid Investing.com split file structure! {file}");

            DateTime? lastDate = null;
            string lastLine = null;
            for (var k = 1; k < lines.Length; k++)
            {
                if (Equals(lastLine, lines[k])) continue;

                var ss = lines[k].Split('\t');
                if (ss.Length != 3)
                    throw new Exception($"Invalid line of Investing.com split file! Line: {lines[k]}, file: {file}");
                if(string.IsNullOrEmpty(ss[0]) && !lastDate.HasValue)
                    throw new Exception($"Error: Empty first date in Investing.com split file! Line: {lines[k]}, file: {file}");
                var date = string.IsNullOrEmpty(ss[0]) ? lastDate.Value : DateTime.ParseExact(ss[0], "MMM dd, yyyy", CultureInfo.InvariantCulture);
                var s = ss[1].Trim();
                if (!s.EndsWith(")"))
                    throw new Exception($"Error: The line must be ended with ')'! Line: {lines[k]}, file: {file}");
                var i = s.LastIndexOf("(", StringComparison.InvariantCultureIgnoreCase);
                var symbol = s.Substring(i + 1, s.Length - i-2).Trim().ToUpper();
                var name = s.Substring(0, i).Trim();

                splits.Add(new object[] {date, symbol, name, ss[2].Trim()});
                lastLine = lines[k];
                lastDate = date;
            }

            var timeStamp = DateTime.ParseExact(Path.GetFileNameWithoutExtension(file).Split('_')[1].Trim(), "yyyyMMdd",
                CultureInfo.InvariantCulture);
            if (splits.Count > 0)
                SaveToDb.SplitInvesting_SaveToDb(splits, timeStamp);

            showStatusAction($"SplitInvesting file parsing finished and saved to DB!!! {Path.GetFileName(file)}");
        }
        #endregion

        #region ========  Yahoo split parse + save to db  (zip) ========
        public static void SplitYahoo_Parse(string file, Action<string> showStatusAction)
        {
            showStatusAction($"SplitYahoo file parsing started.");
            var splits = new Dictionary<string, Dictionary<DateTime, string>>();

            using (var zip = new ZipReader(file))
                foreach (var item in zip)
                {
                    if (item.Length > 0 && item.FileNameWithoutExtension.ToUpper().StartsWith("Y"))
                    {
                        var symbol = item.FileNameWithoutExtension.Substring(1).ToUpper();
                        var symbolSplits = new Dictionary<DateTime, string>();
                        var lines = item.Content.Split('\n');
                        if (lines[0].Equals("Date,Stock Splits"))
                        {
                            for (var k = 1; k < lines.Length; k++)
                            {
                                var ss = lines[k].Split(',');
                                symbolSplits.Add(DateTime.ParseExact(ss[0], "yyyy-MM-dd", CultureInfo.InvariantCulture), ss[1].Trim());
                            }
                        }
                        else
                            throw new Exception("");

                        if (symbolSplits.Count > 0)
                            splits.Add(symbol, symbolSplits);
                    }

                }

            var timeStamp = DateTime.ParseExact(Path.GetFileNameWithoutExtension(file).Split('_')[1].Trim(), "yyyyMMdd",
                CultureInfo.InvariantCulture);
            if (splits.Count > 0)
                SaveToDb.SplitYahoo_SaveToDb(splits, timeStamp);

            showStatusAction($"SplitYahoo file parsing finished and saved to DB!!! {Path.GetFileName(file)}");
        }
        #endregion

        #region ========  Tickertech symbols parse + save to db  ========
        public static void SymbolsTickertech_Parse(Action<string> showStatusAction)
        {
            using (var data = new DataTable())
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                showStatusAction($"SymbolsTickertech. Before parsing procedure");

                data.Columns.Add(new DataColumn("Symbol", typeof(string)));
                data.Columns.Add(new DataColumn("Exchange", typeof(string)));
                data.Columns.Add(new DataColumn("Name", typeof(string)));
                data.Columns.Add(new DataColumn("Created", typeof(DateTime)));

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
                    foreach (var item in items)
                        data.Rows.Add(item.Symbol, item.Exchange, item.Name, date);

                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "truncate table Bfr_SymbolsEoddata";
                    cmd.ExecuteNonQuery();
                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 300;
                        sbc.DestinationTableName = "Bfr_SymbolsEoddata";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                    items.Clear();
                    data.Rows.Clear();

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

        #region ========  Tickertech daily/split parse  ========
        public static void DayTickertech_Parse(Action<string> showStatusAction, bool split)
        {
            var items = new List<DayTickertech>();
            var splits = new Dictionary<string, Dictionary<DateTime, string>>();
            var files = Directory.GetFiles(Settings.DayTickertechFolder);
            var cnt = 0;
            foreach (var file in files)
            {
                var symbol = Path.GetFileNameWithoutExtension(file).Split('.')[0].Substring(1);
                var s1 = File.ReadAllText(file);
                var i1 = s1.IndexOf(">Ratio<", StringComparison.InvariantCulture);
                var symbolSplits = new Dictionary<DateTime, string>();
                if (i1 > 0)
                {
                    var i2 = s1.IndexOf("</table", i1, StringComparison.InvariantCulture);
                    var ss1 = s1.Substring(i1, i2 - i1).Split(new string[] { "</tr>" }, StringSplitOptions.None);
                    for (var k = 1; k < ss1.Length - 1; k++)
                    {
                        var ss2 = ss1[k].Split(new string[] { "</td>" }, StringSplitOptions.None);
                        var i3 = ss2[0].LastIndexOf("<", StringComparison.InvariantCulture);
                        var i4 = ss2[0].LastIndexOf(">", i3, StringComparison.InvariantCulture);
                        var date = DateTime.Parse(ss2[0].Substring(i4 + 1, i3 - i4 - 1), CultureInfo.InvariantCulture);
                        
                        i3 = ss2[1].LastIndexOf("<", StringComparison.InvariantCulture);
                        i4 = ss2[1].LastIndexOf(">", i3, StringComparison.InvariantCulture);
                        var r1 = float.Parse(ss2[1].Substring(i4 + 1, i3 - i4 - 1), CultureInfo.InvariantCulture);

                        i3 = ss2[3].LastIndexOf("<", StringComparison.InvariantCulture);
                        i4 = ss2[3].LastIndexOf(">", i3, StringComparison.InvariantCulture);
                        var r2 = float.Parse(ss2[3].Substring(i4 + 1, i3 - i4 - 1), CultureInfo.InvariantCulture);
                        symbolSplits.Add(date, $"{r1.ToString(CultureInfo.InvariantCulture)}:{r2.ToString(CultureInfo.InvariantCulture)}");
                    }

                    if (!splits.ContainsKey(symbol))
                        splits.Add(symbol, symbolSplits);
                }

                i1 = s1.IndexOf(">Volume<", StringComparison.InvariantCulture);
                if (i1 > 0)
                {
                    var i2 = s1.IndexOf("</table", i1, StringComparison.InvariantCulture);
                    var ss1 = s1.Substring(i1, i2 - i1).Split(new string[] {"</td>"}, StringSplitOptions.None);
                    var date = DateTime.MinValue;
                    var data = new float[5];
                    for (var k = 0; k < 6; k++)
                    {
                        var i3 = ss1[k].LastIndexOf("<", StringComparison.InvariantCulture);
                        var i4 = ss1[k].LastIndexOf(">", i3, StringComparison.InvariantCulture);
                        if (k==0)
                            date = DateTime.Parse(ss1[k].Substring(i4 + 1, i3 - i4 - 1), CultureInfo.InvariantCulture);
                        else
                        data[k - 1] = float.Parse(ss1[k].Substring(i4 + 1, i3 - i4 - 1), CultureInfo.InvariantCulture);
                    }
                    items.Add(new DayTickertech(symbol, date, data, symbolSplits));

                    if (items.Count > 100000)
                    {
                        cnt += items.Count;
                        showStatusAction($"DayTickertech is saving to DB: {items.Count:N0} ({cnt:N0}) records");
                        if (!split)
                            SaveToDb.DayTickertech_SaveToDb(items);
                        items.Clear();
                    }

                }
            }

            if (items.Count > 0 && !split)
            {
                cnt += items.Count;
                showStatusAction($"DayTickertech is saving to DB: {items.Count:N0} ({cnt:N0}) records");
                SaveToDb.DayTickertech_SaveToDb(items);
                items.Clear();
            }
            else if (split)
                SaveToDb.SplitTickertech_SaveToDb(splits);

            showStatusAction($"Day Tickertech files parsing finished!!!");
        }
        #endregion

        #region ========  Eoddata symbols parse + save to db  ========
        public static void SymbolsEoddata_Parse(Action<string> showStatusAction)
        {
            using (var data = new DataTable())
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                showStatusAction($"SymbolsEoddata. Before parsing procedure");

                data.Columns.Add(new DataColumn("Symbol", typeof(string)));
                data.Columns.Add(new DataColumn("Exchange", typeof(string)));
                data.Columns.Add(new DataColumn("Name", typeof(string)));
                data.Columns.Add(new DataColumn("Created", typeof(DateTime)));

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
                    foreach (var item in items)
                        data.Rows.Add(item.Symbol, item.Exchange, item.Name, date);

                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "truncate table Bfr_SymbolsEoddata";
                    cmd.ExecuteNonQuery();
                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 300;
                        sbc.DestinationTableName = "Bfr_SymbolsEoddata";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                    items.Clear();
                    data.Rows.Clear();

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
        public static void DayEoddata_Parse(Action<string> showStatusAction)
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

            var data = new List<DayEoddata>();
            var files = Directory.GetFiles(Settings.DayEoddataFolder);
            var cnt = 0;
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
                            data.Add(new DayEoddata(ss[0], line.Split(',')));
                        prevLine = line;
                    }

                    if (data.Count > 100000)
                    {
                        cnt += data.Count;
                        showStatusAction($"DayEoddata is saving to DB: {data.Count:N0} ({cnt:N0}) records");
                        SaveToDb.DayEoddata_SaveToDb(data);
                        data.Clear();
                    }
                }

            }
            if (data.Count > 0)
            {
                SaveToDb.DayEoddata_SaveToDb(data);
                data.Clear();
            }

            SaveToDb.RefreshTradingDays();

            showStatusAction($"DayEoddata file parsing finished!!!");
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
            var cnt = 0;
            var files = Directory.GetFiles(Settings.DayYahooFolder);
            var clearTable = true;
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
            }

            if (data.Count > 0)
            {
                SaveToDb.DayYahoo_SaveToDb(data, clearTable);
                data.Clear();
            }

            showStatusAction($"DayYahoo file parsing finished!!!");
        }
        public static void xxDayYahoo_Parse(Action<string> showStatusAction)
        {
            var data = new List<DayYahoo>();
            var cnt = 0;
            var files = Directory.GetFiles(Settings.DayYahooFolder);
            var clearTable = true;
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
            }

            if (data.Count > 0)
            {
                SaveToDb.DayYahoo_SaveToDb(data, clearTable);
                data.Clear();
            }

            showStatusAction($"DayYahoo file parsing finished!!!");
        }
        #endregion

        #region ===========  Common  =========
        private static string GetTDElement(string s)
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
        #endregion
    }
}
