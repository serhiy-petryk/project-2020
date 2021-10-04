﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using DGCore.Common;

namespace DGCore.Helpers
{
    public static class SaveData
    {
        #region ===========  Excel file  ================
        private static readonly int _headerExcelColor = Utils.ExcelApp.GetExcelColor(240, 240, 240);

        public static void SaveAndOpenDataToXlsFile(string filename, string header, string[] subHeaders, IList objectsToSave, PropertyDescriptorForDataGridColumn[] properties, List<string> groupColumnNames, int[] groupColors)
        {
            var folder = Path.GetTempPath();
            var fullFileName = Utils.Tips.GetNearestNewFileName(folder, filename);
            SaveDataToXlsFile(fullFileName, header, subHeaders, objectsToSave, properties, groupColumnNames, groupColors);
            // Open new file
            if (File.Exists(fullFileName))
            {
                using (var excel = new Utils.ExcelApp(fullFileName, false))
                {
                    excel.Visible = true;
                    excel.ScreenUpdating = true;
                    //excel.Activate();
                    //excel.SetWindowState(Utils.ExcelApp.xlWindowState.xlMaximized);
                }
            }
        }

        private static void SaveDataToXlsFile(string filename, string header, string[] subHeaders, IList objectsToSave, PropertyDescriptorForDataGridColumn[] properties, List<string> groupColumnNames, int[] groupColors)
        {
            var itemRowNos = new List<int>();
            using (var excel = new Utils.ExcelApp())
            {

                var maxRow = Math.Min(excel.RowLimit - subHeaders.Length - 1, objectsToSave.Count);
                if (maxRow < objectsToSave.Count)
                {
                    if (Shared.ShowMessage(
                            string.Format("Програма Ексель може записати тільки {0} елементів із {1}. Записувати?",
                                maxRow.ToString(), objectsToSave.Count.ToString()), null, Enums.MessageBoxButtons.YesNo,
                            Enums.MessageBoxIcon.Warning) != Enums.MessageBoxResult.Yes)
                    {
                        excel.Book_Close(false);
                        return;
                    }
                }

                // Write to file
                var headerRowNumber = subHeaders.Length + 1;
                var excelSheetName = header.Replace(":", "").Replace("?", "").Replace("*", "").Replace("[", "")
                    .Replace(":]", "").Replace("\\", "").Replace("/", "");
                if (excelSheetName.Length > 31)
                    excelSheetName = excelSheetName.Substring(0, 31);

                excel.Sheet_Name = excelSheetName;
                excel.ScreenUpdating = false;

                // Write subheaders
                for (int i = 0; i < subHeaders.Length; i++)
                {
                    excel.Range_SetCurrentByCell(0, i + 1);
                    excel.Range_SetValue(subHeaders[i]);
                }

                // Write column header
                // PropertyDescriptorCollection pdc = DGVUtils.GetInternalPropertyDescriptorCollection(dgv);
                // PropertyDescriptor[] properties = new PropertyDescriptor[columns.Count];
                for (int i = 0; i < properties.Length; i++)
                {
                    excel.Range_SetCurrentByColumn(i);
                    // excel.Range_Format = DGCore.Utils.ExcelApp.GetExcelFormatString(columns[i].ValueType, columns[i].InheritedStyle.Format);
                    excel.Range_Format = Utils.ExcelApp.GetExcelFormatString(properties[i].PropertyType, properties[i]._format);
                    excel.Range_SetCurrentByCell(i, headerRowNumber);
                    excel.Range_Format = "@";
                    excel.Range_WrapText = true;
                    excel.Range_SetHorisontalAlignment(Utils.ExcelApp.xlHorizontalAlignment.xlCenter);
                    excel.Range_SetVerticalAlignment(Utils.ExcelApp.xlVerticalAlignment.xlVAlignCenter);
                    // excel.Range_SetBackColor(GetExcelColor(columns[i].HeaderCell.InheritedStyle.BackColor));
                    var index = groupColumnNames.IndexOf(properties[i].Name);
                    excel.Range_SetBackColor( index == -1 ? _headerExcelColor : groupColors[index + 1]);
                    /*string s = columns[i].DataPropertyName;
                    if (!String.IsNullOrEmpty(s) && pdc[s] != null)
                    {
                        properties[i] = pdc[s];
                    }*/
                    if (!string.IsNullOrEmpty(properties[i].DisplayName))
                        excel.Range_SetValue(properties[i].DisplayName);
                    excel.Range_SetBorder();
                }

                // Prepare data array
                var xlData = new object[maxRow, properties.Length];
                var groupRowNos = new Dictionary<int, List<int>>();
                for (var i2 = 0; i2 < maxRow; i2++)
                {
                    var groupItem = objectsToSave[i2] as DGVList.IDGVList_GroupItem;
                    if (groupItem == null) itemRowNos.Add(i2);
                    else
                    {
                        var groupLevel = groupItem.Level;
                        if (!groupRowNos.ContainsKey(groupLevel)) groupRowNos.Add(groupLevel, new List<int>());
                        groupRowNos[groupLevel].Add(i2);
                    }
                    for (var i1 = 0; i1 < properties.Length; i1++)
                    {
                        var pd = properties[i1];
                        if (pd.PropertyType.IsClass && pd.PropertyType != typeof(string))
                        {// Nested objects
                            var o = pd.GetValue(objectsToSave[i2]);
                            xlData[i2, i1] = o?.ToString();
                        }
                        else
                            xlData[i2, i1] = pd.GetValue(objectsToSave[i2]);

                        if (Equals(xlData[i2, i1], double.NaN))
                            xlData[i2, i1] = null;
                    }
                }

                // Write data to Excel
                excel.Range_SetCurrentByRegion(0, headerRowNumber + 1, properties.Length, maxRow);
                excel.Range_SetValue(xlData);

                // Set row colors
                var rangeSeparator = CultureInfo.InstalledUICulture.TextInfo.ListSeparator;
                var ii = new List<int>(groupRowNos.Keys);
                ii.Sort(); ii.Reverse();
                for (var i = 0; i < ii.Count; i++)
                {
                    var sb = new StringBuilder();
                    var flag = false;
                    // Range string can not be > 256: we need to split string
                    var sRanges = new List<string>();
                    foreach (var i1 in groupRowNos[ii[i]])
                    {
                        sb.Append((flag ? rangeSeparator : "") + Utils.ExcelApp.GetRangeStringFromRegion(0, headerRowNumber + 1 + i1, properties.Length, 1));
                        if (sb.Length > 230)
                        {
                            flag = false;
                            sRanges.Add(sb.ToString());
                            sb = new StringBuilder();
                        }
                        else flag = true;
                    }
                    if (sb.Length > 0) sRanges.Add(sb.ToString());
                    foreach (var s in sRanges)
                    {
                        excel.Range_SetCurrentByString(s);
                        // excel.Range_SetBackColor(GetExcelColor(groupPens[ii[i]].Color));
                        excel.Range_SetBackColor(groupColors[ii[i]]);
                        excel.Range_SetBorder();
                        if (i > 0)
                        {// start to set bold font only from second group
                            excel.Range_SetFont(true, i + 10);
                        }
                    }
                }

                // Freeze Panes
                excel.Book_FreezePanes(0, headerRowNumber + 1, true);
                // Set autofilter
                excel.SetAutoFilter(0, headerRowNumber, properties.Length, maxRow + 1);
                // Adjust column widths
                excel.Range_AutoFitColumns();

                // Write Header
                excel.Range_SetCurrentByCell(0, 0);
                excel.Range_SetValue(header);
                excel.Range_SetFont(true, 12);
                // Write current datetime
                excel.Range_SetCurrentByCell(Math.Max(1, properties.Length - 1), 0);
                excel.Range_SetValue(DateTime.Now);
                excel.Range_SetHorisontalAlignment(Utils.ExcelApp.xlHorizontalAlignment.xlRight);
                excel.Range_Format = Utils.ExcelApp.GetExcelDateTimeFormatFromVSFormatString("G");
                // Clear selection
                excel.Range_SetCurrentByCell(0, headerRowNumber);
                excel.Range_Select();
                // Save file
                excel.Book_Save(filename, true);
            }
        }

            #endregion

            #region ===========  Text file  ================
            public static void SaveAndOpenDataToTextFile(string filename, IEnumerable objectsToSave, PropertyDescriptor[] properties)
        {
            var folder = Path.GetTempPath();
            var fullFileName = Utils.Tips.GetNearestNewFileName(folder, filename);
            SaveDataToTextFile(fullFileName, objectsToSave, properties);
            // Open new file
            if (File.Exists(fullFileName))
            {
                using (var p = new Process())
                {
                    p.StartInfo.FileName = @"notepad.exe";
                    p.StartInfo.Arguments = fullFileName;
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
