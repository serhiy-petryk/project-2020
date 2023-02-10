﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Quote2022.Actions
{
    public static class IntradayAlphaVantage_Check
    {
        public class LogEntry
        {
            public string File;
            public string Symbol;
            public DateTime Date;
            public TimeSpan MinTime = TimeSpan.MaxValue;
            public TimeSpan MaxTime = TimeSpan.MinValue;
            public short Count;
            public float Open;
            public float High = float.MinValue;
            public float Low = float.MaxValue;
            public float Close;
            public float Volume;
            public override string ToString() => $"{File}\t{Symbol}\t{Date:yyyy-MM-dd}\t{Helpers.CsUtils.GetString(MinTime)}\t{Helpers.CsUtils.GetString(MaxTime)}\t{Count}\t{Open}\t{High}\t{Low}\t{Close}\t{Volume}";
        }

        private static bool _isBusy = false;
        private static Action<string> _showStatusAction;

        public static void Start(Action<string> showStatusAction, string folder)
        {
            if (_isBusy)
            {
                MessageBox.Show("IntradayAlphaVantage_Download is working now .. Can't run it again.");
                return;
            }

            _isBusy = true;
            _showStatusAction = showStatusAction;

            var folderId = Path.GetFileName(folder) + @"\";
            // delete old log in database
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"DELETE IntradayAlphaVantageFileLog WHERE [file] like '{folderId}%'";
                cmd.ExecuteNonQuery();
            }

            var errorLog = new List<string>();
            var log = new List<LogEntry>();
            var files = Directory.GetFiles(folder, "*.csv");
            var cnt = 0;
            foreach (var file in files)
            {
                var fileId = folderId + Path.GetFileName(file);

                cnt++;
                if (cnt%100 == 0)
                    _showStatusAction($"IntradayAlphaVantage_Check is working. Process {cnt} from {files.Length} files");

                var context = File.ReadAllLines(file);
                if (context.Length == 0)
                {
                    errorLog.Add($"{fileId}\tEmpty file");
                    continue;
                }
                if (context[0] != "time,open,high,low,close,volume")
                {
                    errorLog.Add($"{fileId}\tBad header");
                    continue;
                }

                var symbol = Path.GetFileNameWithoutExtension(file).Split('_')[1];
                var i = symbol.IndexOf('.');
                if (i != -1)
                {
                    throw new Exception("Need to check");
                    symbol = symbol.Substring(0, i - 1);
                }
                LogEntry logEntry = null;
                var lastDate = DateTime.MinValue;
                for (var k = 1; k < context.Length; k++)
                {
                    var line = context[k];
                    var lines = line.Split(',');
                    if (lines.Length != 6)
                    {
                        errorLog.Add($"{fileId}\tBad {k} line length\t{line}");
                        continue;
                    }

                    var dateAndTime = DateTime.ParseExact(lines[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    var date = dateAndTime.Date;
                    var time = dateAndTime.TimeOfDay;
                    var open = float.Parse(lines[1], CultureInfo.InvariantCulture);
                    var high = float.Parse(lines[2], CultureInfo.InvariantCulture);
                    var low = float.Parse(lines[3], CultureInfo.InvariantCulture);
                    var close = float.Parse(lines[4], CultureInfo.InvariantCulture);
                    var volume = long.Parse(lines[5], CultureInfo.InvariantCulture);

                    if (open > high || open < low || close > high || close < low || low < 0)
                        errorLog.Add($"{fileId}\tBad prices in {k} line\t{line}");
                    if (volume<0)
                        errorLog.Add($"{fileId}\tBad volume in {k} line\t{line}");
                    if (volume==0 && high!=low)
                        errorLog.Add($"{fileId}\tPrices are not equal when volume=0 in {k} line\t{line}");

                    if (date != lastDate)
                    {
                        logEntry = new LogEntry {File=fileId, Symbol = symbol, Date = date};
                        log.Add(logEntry);
                    }

                    logEntry.Count++;
                    if (logEntry.MinTime > time) logEntry.MinTime = time;
                    if (logEntry.MaxTime < time) logEntry.MaxTime = time;

                    logEntry.Open = open;
                    if (high > logEntry.High) logEntry.High = high;
                    if (low < logEntry.Low) logEntry.Low = low;
                    if (logEntry.Count == 1)
                        logEntry.Close = close;
                    logEntry.Volume += volume;

                    lastDate = date;
                }
            }

            _showStatusAction($"IntradayAlphaVantage_Check. Save data to database ...");
            // Save items to database table
            SaveToDb.SaveToDbTable(log, "IntradayAlphaVantageFileLog", "File", "Symbol", "Date", "MinTime", "MaxTime",
                "Count", "Open", "High", "Low", "Close", "Volume");

            var errorFileName = Directory.GetParent(folder) + $"\\ErrorLog_{Path.GetFileName(folder)}.txt";
            File.AppendAllText(errorFileName, $"File\tMessage\tContent{Environment.NewLine}");
            File.AppendAllLines(errorFileName, errorLog);

            _isBusy = false;
            _showStatusAction($"IntradayAlphaVantage_Check finished. Found {errorLog.Count} errors");
        }
    }
}
