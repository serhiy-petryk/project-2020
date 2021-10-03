using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DGCore.Helpers
{
    public static class SaveData
    {
        #region ===========  Text file  ================
        public static void SaveAndOpenDataToTextFile(string filename, IEnumerable objectsToSave, PropertyDescriptor[] properties)
        {
            var folder = Path.GetTempPath();
            var fullFilename = Utils.Tips.GetNearestNewFileName(folder, filename);
            SaveDataToTextFile(fullFilename, objectsToSave, properties);
            // Open new file
            if (File.Exists(fullFilename))
            {
                using (var p = new Process())
                {
                    p.StartInfo.FileName = @"notepad.exe";
                    p.StartInfo.Arguments = fullFilename;
                    p.Start();
                }
            }
        }

        private static void SaveDataToTextFile(string filename, IEnumerable objectsToSave, PropertyDescriptor[] properties)
        {
            using (var sw = new StreamWriter(filename, false, Encoding.Unicode))
            {
                // Save header
                var ss1 = new string[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                    ss1[i] = properties[i].DisplayName;
                sw.WriteLine(string.Join("\t", ss1));

                // Save data
                foreach (var item in objectsToSave)
                {
                    ss1 = new string[properties.Length];
                    for (int i1 = 0; i1 < properties.Length; i1++)
                    {
                        object o = properties[i1].GetValue(item);
                        if (o is DGVList.DGVGroupTotalValueProxy proxy)
                            o = proxy.GetValue(properties[i1].Name);
                        if (o != null) ss1[i1] = o.ToString();
                    }

                    sw.WriteLine(string.Join("\t", ss1));
                }
            }
        }
        #endregion
    }
}
