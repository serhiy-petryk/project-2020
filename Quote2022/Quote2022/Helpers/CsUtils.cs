using System;
using System.Windows.Forms;

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

    }
}
