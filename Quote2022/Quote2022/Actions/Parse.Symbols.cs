using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quote2022.Helpers;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static partial class Parse
    {
        #region ==================  SymbolsNasdaq_ParseAndSaveToDb  ==========================
        public static void SymbolsNasdaq_ParseAndSaveToDb(string zipFile, Action<string> showStatusAction)
        {
            var validAssets = new Dictionary<string, object>() { { "STOCKS", null }, { "ETF", null } };
            var validExchanges = new Dictionary<string, object>()
            {
                {"AMEX", null}, {"BAT", null}, {"DUAL LISTED", null}, {"NASDAQ", null}, {"NASDAQ-CM", null},
                {"NASDAQ-GM", null}, {"NASDAQ-GS", null}, {"NYSE", null}, {"PSE", null}
            };
            var symbols = new Dictionary<string, List<SymbolsNasdaqDbItem>>();
            using (var zip = new ZipReader(zipFile))
                foreach (var file in zip.Where(a => a.FullName.EndsWith(".txt", true, CultureInfo.InvariantCulture)))
                {
                    showStatusAction($"{file.FileNameWithoutExtension} file is parsing.");
                    var oo = JsonConvert.DeserializeObject<SymbolsNasdaqFile>(file.Content);
                    if (oo.Status.rCode == 200)
                    {
                        foreach (var o in oo.data.Where(a => !string.IsNullOrEmpty(a.Exchange) && validAssets.ContainsKey(a.Asset) && validExchanges.ContainsKey(a.Exchange)))
                        {
                            var dbItem = new SymbolsNasdaqDbItem(o, file.Created);
                            if (!symbols.ContainsKey(dbItem.Symbol))
                                symbols.Add(dbItem.Symbol, new List<SymbolsNasdaqDbItem>());
                            symbols[dbItem.Symbol].Add(dbItem);
                        }
                    }
                    else
                        throw new Exception(
                            $"Invalid status rCode in {file.FileNameWithoutExtension} file. status rCode: {oo.Status.rCode}");

                    // break;
                }

            // Check
            foreach (var items in symbols.Values)
            {
                var firstItem = items[0];
                for (var k = 1; k < items.Count; k++)
                {
                    if (!firstItem.IsEqual(items[k]))
                        throw new Exception($"SymbolsNasdaq_ParseAndSaveToDb error! Different items for symbol {firstItem.Symbol}");
                }
            }

            showStatusAction("Saving data to database (SymbolsNasdaq_ParseAndSaveToDb method).");

            var data = symbols.Values.Select(a => a[0]).ToArray();
            SaveToDb.ClearAndSaveToDbTable(data, "Bfr_SymbolsNasdaq", "Exchange", "Symbol", "Name", "MrktCategory",
                "SubCategory", "Nasdaq100", "Region", "Asset", "Industry", "Flag", "Created");

            var ss = Path.GetFileNameWithoutExtension(zipFile).Split('_');
            var timeStamp = DateTime.ParseExact(ss[ss.Length - 1].Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
            SaveToDb.RunProcedure("pUpdateSymbolsNasdaq", new Dictionary<string, object> { { "@Date", timeStamp } });

            showStatusAction("FINISHED (SymbolsNasdaq_ParseAndSaveToDb method)!");
        }
        #endregion

        #region ==================  SymbolsNasdaqAll_ParseAndSaveToDb  ==========================
        public static void SymbolsNasdaqAll_ParseAndSaveToDb(string zipFile, Action<string> showStatusAction)
        {
            var symbols = new Dictionary<string, List<SymbolsNasdaqDbItem>>();
            using (var zip = new ZipReader(zipFile))
                foreach (var file in zip.Where(a => a.FullName.EndsWith(".txt", true, CultureInfo.InvariantCulture)))
                {
                    showStatusAction($"{file.FileNameWithoutExtension} file is parsing.");
                    var oo = JsonConvert.DeserializeObject<SymbolsNasdaqFile>(file.Content);
                    if (oo.Status.rCode == 200)
                    {
                        foreach (var o in oo.data)
                        {
                            var dbItem = new SymbolsNasdaqDbItem(o, file.Created);
                            if (!symbols.ContainsKey(dbItem.Key))
                                symbols.Add(dbItem.Key, new List<SymbolsNasdaqDbItem>());
                            symbols[dbItem.Key].Add(dbItem);
                        }
                    }
                    else
                        throw new Exception(
                            $"Invalid status rCode in {file.FileNameWithoutExtension} file. status rCode: {oo.Status.rCode}");

                    // break;
                }

            // Check
            foreach (var items in symbols.Values)
            {
                var firstItem = items[0];
                for (var k = 1; k < items.Count; k++)
                {
                    if (!firstItem.IsEqual(items[k]))
                        throw new Exception($"SymbolsNasdaqAll_ParseAndSaveToDb error! Different items for symbol {firstItem.Symbol}");
                }
            }

            showStatusAction("Saving data to database (SymbolsNasdaqAll_ParseAndSaveToDb method).");

            var data = symbols.Values.Select(a => a[0]).ToArray();
            SaveToDb.ClearAndSaveToDbTable(data, "Bfr_SymbolsNasdaq", "Exchange", "Symbol", "Name", "MrktCategory",
                "SubCategory", "Nasdaq100", "Region", "Asset", "Industry", "Flag", "Created");

            var ss = Path.GetFileNameWithoutExtension(zipFile).Split('_');
            var timeStamp = DateTime.ParseExact(ss[ss.Length - 1].Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
            SaveToDb.RunProcedure("pUpdateSymbolsNasdaqAll", new Dictionary<string, object> {{"@Date", timeStamp}});

            showStatusAction("FINISHED (SymbolsNasdaqAll_ParseAndSaveToDb method)!");
        }
        #endregion

        #region ========  Eoddata symbols parse + save to db ========
        public static void SymbolsEoddata_ParseAndSaveToDb(Action<string> showStatusAction)
        {
            var validExchanges = new Dictionary<string, object> {{"AMEX", null}, {"NASDAQ", null}, {"NYSE", null}};

            SaveToDb.RunProcedure("pUpdateSymbolsEoddata_Before");

            var items = new List<SymbolsEoddata>();
            // var files = Directory.GetFiles(Settings.SymbolsEoddataFolder, "*.txt", SearchOption.AllDirectories);
            var files = Directory.GetFiles(Settings.SymbolsEoddataFolder, "*.txt", SearchOption.TopDirectoryOnly);
            var orderedFiles = files.OrderBy(Path.GetFileName);
            foreach (var file in orderedFiles)
            {
                showStatusAction($"SymbolsEoddata file is parsing: {Path.GetFileName(file)}");
                var ss = Path.GetFileNameWithoutExtension(file).Split('_');
                var exchange = ss[0].Trim().ToUpper();
                if (!validExchanges.ContainsKey(exchange))
                    continue;

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
                SaveToDb.ClearAndSaveToDbTable(items, "Bfr_SymbolsEoddata", "Symbol", "Exchange", "Name", "Created");
                SaveToDb.RunProcedure("pUpdateSymbolsEoddata", new Dictionary<string, object> { { "@Exchange", exchange }, { "@Date", date } });
                items.Clear();
            }

            showStatusAction($"SymbolsEoddata file parsing finished!!!");
        }
        #endregion

        #region ========  Eoddata symbols parse + save to db (all) ========
        public static void SymbolsEoddataAll_ParseAndSaveToDb(Action<string> showStatusAction)
        {
            var items = new List<SymbolsEoddata>();
            // var files = Directory.GetFiles(Settings.SymbolsEoddataFolder, "*.txt", SearchOption.AllDirectories);
            var files = Directory.GetFiles(Settings.SymbolsEoddataFolder, "*.txt", SearchOption.TopDirectoryOnly);
            var orderedFiles = files.OrderBy(Path.GetFileName);
            foreach (var file in orderedFiles)
            {
                showStatusAction($"SymbolsEoddata (all) file is parsing: {Path.GetFileName(file)}");
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
                SaveToDb.ClearAndSaveToDbTable(items, "Bfr_SymbolsEoddata", "Symbol", "Exchange", "Name", "Created");
                SaveToDb.RunProcedure("pUpdateSymbolsEoddataAll", new Dictionary<string, object> { { "@Exchange", exchange }, { "@Date", date } });
                items.Clear();
            }

            showStatusAction($"SymbolsEoddata (all) file parsing finished!!!");
        }
        #endregion
    }
}
