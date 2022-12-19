using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quote2022.Models;

namespace Quote2022.Actions
{
    public static partial class Parse
    {
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
