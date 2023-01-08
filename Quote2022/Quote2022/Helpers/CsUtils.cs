﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Quote2022.Helpers
{
    public static class CsUtils
    {
        public static string OpenZipFileDialog(string folder) => OpenFileDialogGeneric(folder, @"zip files (*.zip)|*.zip");
        public static string OpenTxtFileDialog(string folder) => OpenFileDialogGeneric(folder, @"Text|*.txt");
        public static string OpenCsvFileDialog(string folder) => OpenFileDialogGeneric(folder, @"CSV Files (*.csv)|*.csv");
        public static string OpenFileDialogGeneric(string folder, string filter)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = folder;
                ofd.RestoreDirectory = true;
                ofd.Multiselect = false;
                ofd.Filter = filter;
                if (ofd.ShowDialog() == DialogResult.OK)
                    return ofd.FileName;
                return null;
            }
        }

        public static string GetString(object o)
        {
            if (o is DateTime dt)
                return dt.ToString(dt.TimeOfDay == TimeSpan.Zero ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm");
            else if (Equals(o, null)) return null;
            return o.ToString();
        }

        public static List<TimeSpan> GetTimeFrames(TimeSpan from, TimeSpan to, TimeSpan interval)
        {
            var data = new List<TimeSpan>();
            var ts = from;
            while (ts <= to)
            {
                data.Add(ts);
                ts = ts.Add(interval);
            }
            return data;
        }

        public static DateTime RoundDown(this DateTime dt, TimeSpan d)
        { // see answer of redent84 in https://stackoverflow.com/questions/7029353/how-can-i-round-up-the-time-to-the-nearest-x-minutes
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }

    }
}
