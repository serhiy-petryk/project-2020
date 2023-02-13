using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quote2022.Helpers;

namespace Quote2022.Actions
{
    public class MinuteAlphaVantage_SplitData
    {
        private static bool _isBusy = false;

        public static void Start(string zipFile, Action<string> showStatusAction)
        {
            if (_isBusy)
            {
                MessageBox.Show("MinuteAlphaVantage_SplitData is working now .. Can't run it again.");
                return;
            }

            _isBusy = true;

            var sw = new Stopwatch();
            sw.Start();

            var destinationFolder = Path.GetDirectoryName(zipFile) + @"\" + @"DataByDate\";
            var errorLog = new List<string>();
            var cnt = 0;
            using (var zip = new ZipReader(zipFile))
                foreach (var file in zip)
                    if (file.Length > 0)
                    {
                        cnt++;
                        if (cnt % 10 == 0)
                            showStatusAction($"MinuteAlphaVantage_SplitData is working. Processed {cnt} files");

                        var lines = file.Content.Split(new[] { Environment.NewLine },
                            StringSplitOptions.RemoveEmptyEntries);
                        if (lines.Length == 0)
                        {
                            // errorLog.Add($"{fileId}\tEmpty file");
                            continue;
                        }

                        if (lines[0] != "time,open,high,low,close,volume" &&
                            lines[0] != "timestamp,open,high,low,close,volume")
                        {
                            if (lines.Length > 1 && lines[1].Contains("Invalid API call"))
                            {
                                errorLog.Add($"{file.FileNameWithoutExtension}\tInvalid API call");
                            }
                            else if (lines.Length > 1 && lines[1].Contains("Thank you for using Alpha"))
                            {
                                errorLog.Add($"{file.FileNameWithoutExtension}\tThank you for using");
                            }
                            else
                            {
                                errorLog.Add($"{file.FileNameWithoutExtension}\tBad header");
                            }

                            continue;
                        }

                        var symbol = file.FileNameWithoutExtension.Split('_')[1];
                        var i = symbol.IndexOf('.');
                        if (i != -1)
                        {
                            throw new Exception("Need to check");
                            symbol = symbol.Substring(0, i - 1);
                        }

                        if (lines.Length == 1)
                        {
                            // blankFiles.Add(new BlankFile { File = fileId, Created = File.GetCreationTime(file), Symbol = symbol });
                        }

                        var lastDate = DateTime.MinValue;
                        var linesToSave = new List<string>();
                        for (var k = lines.Length - 1; k >= 1; k--)
                        {
                            var line = lines[k];
                            var date = DateTime.ParseExact(line.Substring(0, line.IndexOf(',')),
                                "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).Date;
                            if (date != lastDate)
                            {
                                SaveQuotesToFile(symbol, lastDate, linesToSave);
                                linesToSave.Clear();
                                linesToSave.Add($"# File {file.FileNameWithoutExtension}. Created at {file.Created:yyyy-MM-dd HH:mm:ss}");
                                lastDate = date;
                            }
                            linesToSave.Add(line);
                        }
                    }

            sw.Stop();
            Debug.Print($"MinuteAlphaVantage_SplitData. {cnt} items. {sw.ElapsedMilliseconds} millisecs");

            _isBusy = false;
            showStatusAction($"MinuteAlphaVantage_SplitData finished. Found {errorLog.Count} errors. See Debug.Output window");

            Debug.Print($"******  MinuteAlphaVantage_SplitData errors  *******");
            foreach(var line in errorLog)
                Debug.Print(line);

            void SaveQuotesToFile(string symbol, DateTime date, List<string> content)
            {
                if (content.Count <2 ) return;
                var dataFolder = destinationFolder + date.ToString("yyyy-MM-dd") + @"\";
                if (!Directory.Exists(dataFolder))
                    Directory.CreateDirectory(dataFolder);

                var fn = dataFolder + symbol + "_" + date.ToString("yyyyMMdd") + ".csv";
                if (File.Exists(fn))
                {
                    // Check content of old and new files
                    var oldLines = File.ReadAllLines(fn);
                    if (oldLines.Length != content.Count)
                                                throw new Exception($"Different content. File: {fn}. Id of new file: {content[0]}");
                    for (var k = 1; k < oldLines.Length; k++)
                    {
                        if (oldLines[k] != content[k])
                            throw new Exception($"Different content in {k} line of files. File: {fn}. Id of new file: {content[0]}.{Environment.NewLine}{oldLines[k]}{Environment.NewLine}{content[k]}");
                    }
                }
                else File.WriteAllLines(fn, content);
            }

        }
    }
}
