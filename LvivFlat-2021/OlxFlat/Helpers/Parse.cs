using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using OlxFlat.Models;
using RealEstateFlat.Models;

namespace OlxFlat.Helpers
{
    public static class Parse
    {
        public static void VN_House_Details_Parse(Action<string> showStatusAction)
        {
            var files = new List<string>();
            var missingFiles = new List<string>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "select * from vVN_House_Details_NewToDownload";
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                        {
                            var filename = string.Format(Settings.VN_House_Details_FileTemplate, (string)rdr["id"]);
                            if (File.Exists(filename))
                                files.Add(filename);
                            else
                                missingFiles.Add(filename);
                        }
                }
            }

            var items = new List<VnHouseDetails>();
            var cnt = files.Count;
            foreach (var file in files)
            {
                cnt--;
                if (cnt % 10 == 0)
                    showStatusAction($"VN House details parse. Remain {cnt} files");
                items.Add(new VnHouseDetails(file));
            }

            Debug.Print($"VnHouseDetails items: {items.Count}");

            showStatusAction($"VnHouseDetails: SaveToDb");
            // SaveToDb.RealEstateDetails_Save(items);

            showStatusAction($"VnHouseDetails parsing: finished");

        }

        #region ==============  VN.Houses list  ==================
        public static void VN_House_List_Parse(Action<string> showStatusAction)
        {
            var items = new Dictionary<string, VnHouseList>();
            var files = Directory.GetFiles(Settings.VN_House_List_FileFolder, "*.txt");
            foreach (var file in files)
            {
                showStatusAction($"VN house list parsing: {file}");
                VN_Houses_ParseFile(file, items);
            }

            showStatusAction($"VN House list: SaveToDb");
            SaveToDb.VN_House_List_Save(items.Values);

            showStatusAction($"VN houses parsing: finished");
        }

        private static int cnt = 0;
        private static void VN_Houses_ParseFile(string filename, Dictionary<string, VnHouseList> items)
        {
            var s1 = File.ReadAllText(filename, Encoding.UTF8);
            var fileDate = File.GetLastWriteTime(filename).Date;
            var i1 = s1.IndexOf("section section--building-cards", StringComparison.InvariantCultureIgnoreCase);
            var i2 = s1.IndexOf("section section--pagination", i1, StringComparison.InvariantCultureIgnoreCase);
            var s2 = s1.Substring(i1, i2-i1);
            var ss1 = s2.Split(new string[] {"\"building-card__heading\""}, StringSplitOptions.None);
            for (var k = 1; k < ss1.Length; k++)
            {
                var item = new VnHouseList(ss1[k]);
                items.Add(item.Id, item);
            }
        }

        #endregion

        #region ================  RealEstate details  ====================
        public static void RealEstateDetails_Parse(Action<string> showStatusAction)
        {
            var files = new List<string>();
            var missingFiles = new List<string>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "select * from vRealEstateDetails_NewToDownload";
                    // cmd.CommandText = "select * from RealEstate";
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                        {
                            var filename = string.Format(Settings.RealEstateDetails_FileTemplate, (int)rdr["id"]);
                            if (File.Exists(filename))
                                files.Add(filename);
                            else
                                missingFiles.Add(filename);
                        }
                }
            }

            var items = new List<RealEstateDetails>();
            // var notFoundItems = new List<RealEstateDetails>();
            // var files = Directory.GetFiles(Settings.RealEstateFileDetailsFolder, "*.txt");
            var cnt = files.Count;
            foreach (var file in files)
            {
                cnt--;
                if (cnt % 10 == 0)
                    showStatusAction($"RealEstate details parse. Remain {cnt} files");
                /* var item = new RealEstateDetails(file);
                if (item.NotFound)
                    notFoundItems.Add(item);
                else
                    items.Add(item);*/
                items.Add(new RealEstateDetails(file));
            }

            Debug.Print($"RealEstateDetails items: {items.Count}");

            showStatusAction($"RealEstateDetails: SaveToDb");
            SaveToDb.RealEstateDetails_Save(items);

            showStatusAction($"RealEstateDetails: finished");
        }
        #endregion

        #region ==============  RealEstate list  ==================
        public static void RealEstateList_Parse(Action<string> showStatusAction)
        {
            var items = new Dictionary<int, RealEstateList>();
            var files = Directory.GetFiles(Settings.RealEstateList_FileFolder, "*.txt");
            foreach (var file in files)
            {
                showStatusAction($"RealEstate list parse: {file}");
                RealEstateList_ParseFile(file, items);
            }

            Debug.Print($"RealEstate list items: {items.Count}");

            showStatusAction($"RealEstate list: SaveToDb");
            SaveToDb.RealEstateList_Save(items.Values);

            showStatusAction($"RealEstate list parse: finished");
        }

        private static void RealEstateList_ParseFile(string filename, Dictionary<int, RealEstateList> items)
        {
            var s = File.ReadAllText(filename, Encoding.UTF8);
            var fileDate = File.GetLastWriteTime(filename).Date;
            var ss1 = s.Split(new[] { "\"col-dense-right " }, StringSplitOptions.None);
            for (var k2 = 1; k2 < ss1.Length; k2++)
            {
                if (k2 == ss1.Length - 1)
                {
                    var a1 = ss1[k2].IndexOf("ads-in-list-wrapper", StringComparison.InvariantCultureIgnoreCase);
                    var a2 = ss1[k2].LastIndexOf("<div ", a1, StringComparison.InvariantCultureIgnoreCase);
                    ss1[k2] = ss1[k2].Substring(0, a2);
                }

                if (ss1[k2].IndexOf("ads-in-list-wrapper", StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                }
                else if (ss1[k2].IndexOf("application/ld+json", StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                }
                else if (ss1[k2].IndexOf("data-id=", StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    var item = new RealEstateList(ss1[k2], fileDate);
                    if (items.ContainsKey(item.Id))
                        item.CheckDuplicate(items[item.Id]);
                    else
                        items.Add(item.Id, item);
                }
                else if (ss1[k2].IndexOf(">ЗАБУДОВНИК</", StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                }
                else if (ss1[k2].IndexOf(">Нова черга</", StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                }
                else if (ss1[k2].IndexOf(">Старт продажу</", StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                }
                else
                    throw new Exception($"RealEstateList_ParseFile. Check file {filename}");
            }
        }

        #endregion

        #region ================  DomRia details  ====================
        public static void DomRiaDetails_Parse(Action<string> showStatusAction)
        {
            /*var listFiles = Directory.GetFiles(Settings.DomRiaListFileFolder, "*.txt");
            var listIds = new Dictionary<int, object>();
            foreach (var fn in listFiles)
            {
                var content = File.ReadAllText(fn);
                var o = JsonConvert.DeserializeObject<DomRiaList>(content);
                foreach (var item in o.Items)
                    listIds.Add(item, null);
            }*/

            var existingIds = SaveToDb.DomRia_GetExistingIds();
            var data = new Dictionary<int, DomRiaDetails>();
            var files = Directory.GetFiles(Settings.DomRiaDetailsFileFolder, "*.txt");
            var cnt = files.Length;
            foreach (var file in files)
            {
                cnt--;
                if (cnt % 10 == 0)
                    showStatusAction($"DomRia details parse. Remain {cnt} files");

                var ss1 = file.Split(new[] { ".", "_" }, StringSplitOptions.None);
                var id = int.Parse(ss1[ss1.Length - 2]);

                if (!existingIds.ContainsKey(id))
                // if (listIds.ContainsKey(id))
                {
                    var settings = new JsonSerializerSettings
                    {
                        DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ", DateTimeZoneHandling = DateTimeZoneHandling.Local
                    };
                    var o = JsonConvert.DeserializeObject<DomRiaDetails>(File.ReadAllText(file), settings);
                    o.ApplyCharacteristics();
                    data.Add(id, o);
                }
            }

            showStatusAction($"DomRia Details: SaveToDb");
            SaveToDb.DomRiaDetails_Save(data.Values);
            showStatusAction($"DomRia Details: FINISHED!");
        }
        #endregion

        #region ================  OLX details  ====================

        public static void OlxDetails_Parse(Action<string> showStatusAction)
        {
            var files = new List<string>();
            var missingFiles = new List<string>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "select * from vOlxDetails_NewToDownload";
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                        {
                            var filename = string.Format(Settings.OlxFileDetailsTemplate, (int)rdr["id"]);
                            if (File.Exists(filename))
                                files.Add(filename);
                            else
                                missingFiles.Add(filename);
                        }
                }
            }

            var items = new List<OlxDetails>();
            // var files = Directory.GetFiles(Settings.OlxFileDetailsFolder, "*.txt");
            var cnt = files.Count;
            foreach (var file in files)
            {
                cnt--;
                if (cnt % 10 == 0)
                    showStatusAction($"OLX details parse. Remain {cnt} files");

                items.Add(new OlxDetails(file));
            }

            Debug.Print($"OlxDetails items: {items.Count}");

            showStatusAction($"OLXDetails: SaveToDb");
            SaveToDb.OlxDetails_Save(items);

            showStatusAction($"OLXDetails: finished");
        }
        #endregion

        #region ================  OLX list  ====================
        public static void OlxList_Parse(Action<string> showStatusAction)
        {
            var items = new Dictionary<int, OlxList>();
            var files = Directory.GetFiles(Settings.OlxFileListFolder, "*.txt");
            foreach (var file in files)
            {
                showStatusAction($"OLX parse: {file}");
                OlxList_ParseFile(file, items);
            }

            Debug.Print($"OlxList items: {items.Count}");

            showStatusAction($"OLXList: SaveToDb");
            SaveToDb.OlxList_Save(items.Values);

            showStatusAction($"OLXList: finished");
        }

        private static void OlxList_ParseFile(string filename, Dictionary<int, OlxList> items)
        {
            var s = File.ReadAllText(filename, Encoding.UTF8);
            var fileDate = File.GetLastWriteTime(filename).Date;
            var ss1 = s.Split(new[] { "<tr class=\"wrap\"" }, StringSplitOptions.None);
            for (var k2 = 1; k2 < ss1.Length; k2++)
            {
                var item = new OlxList(ss1[k2], fileDate);
                if (items.ContainsKey(item.Id))
                    item.CheckEquality(items[item.Id]);
                else
                    items.Add(item.Id, item);
            }
        }
        #endregion

        public static string ParseString_Quotes(string source, int startIndex)
        {
            var i1 = source.IndexOf('"', startIndex);
            var i2 = source.IndexOf('"', i1 + 1);
            var s = source.Substring(i1 + 1, i2 - i1 - 1);
            return s;
        }

        public static string ParseString_Braces(string source, int startIndex)
        {
            var i1 = source.IndexOf('>', startIndex);
            var i2 = source.IndexOf("</", i1 + 1);
            var s = source.Substring(i1 + 1, i2 - i1 - 1);
            return s;
        }

    }
}
