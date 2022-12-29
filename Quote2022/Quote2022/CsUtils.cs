using System;
using System.Windows.Forms;

namespace Quote2022
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

        public static string StringFromDateTime(DateTime? dt)
        {
            if (dt.HasValue)
            {
                if (dt.Value.TimeOfDay == TimeSpan.Zero) return dt.Value.ToString("yyyy-MM-dd");
                else return dt.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else return "";
        }
    }
}
