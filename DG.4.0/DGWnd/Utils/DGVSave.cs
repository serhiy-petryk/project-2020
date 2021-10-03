using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DGWnd.Utils {
  public class DGVSave {

    readonly static NumberFormatInfo fiNumberInvariant = CultureInfo.InvariantCulture.NumberFormat;
    const string xmlHeader = @"<?xml version=""1.0""?><Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
 xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:x=""urn:schemas-microsoft-com:office:excel""
 xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet"" xmlns:html=""http://www.w3.org/TR/REC-html40"">
<Styles><Style ss:ID=""Default"" ss:Name=""Normal""><Alignment ss:Vertical=""Bottom"" ss:WrapText=""0""/><Borders/>
<Font ss:FontName=""Courier New"" x:CharSet=""204"" x:Family=""Modern""/>
<Interior/><NumberFormat/><Protection/></Style>
<Style ss:ID=""CH""><Alignment ss:Horizontal=""Center"" ss:Vertical=""Bottom""/>
<Font ss:FontName=""Tahoma"" x:CharSet=""204"" x:Family=""Swiss""/>
<Interior ss:Color=""gainsboro"" ss:Pattern=""Solid""/><NumberFormat ss:Format=""@""/></Style>";

    public static void SaveDGVToStreamAsXML(Stream stream, DataGridView dgv, Encoding encoding, string header) {
      object[] objectsToCopy;
      DataGridViewColumn[] colsToCopy;
      DGVSelection.GetSelectedArea(dgv,  out objectsToCopy, out colsToCopy);
      PropertyDescriptorCollection pdc = DGVUtils.GetInternalPropertyDescriptorCollection(dgv);
      PropertyDescriptor[] properties = new PropertyDescriptor[colsToCopy.Length];
      for (int i = 0; i < properties.Length; i++) {
        properties[i] = pdc[colsToCopy[i].DataPropertyName];
      }

      StringBuilder sbStyle = new StringBuilder();
      StringBuilder sbCols = new StringBuilder();
      StringBuilder sbHeader = new StringBuilder();
      //Prepare column list        
      int cnt = 0;
      string ss;
      for (int i = 0; i < colsToCopy.Length; i++) {
        DataGridViewColumn col = colsToCopy[i];
        sbStyle.Append(String.Format(@"<Style ss:ID=""s{0}"">{1}</Style>", cnt, xmlGetColumnFormat(col)));
        sbCols.Append(String.Format(@"<Column ss:Width=""{0}"" ss:StyleID=""s{1}""/>", col.Width, cnt));
        string colTitle = decodeToXML(col.HeaderText);
        ss = (colTitle.Length > 30 ? colTitle.Substring(0, 30) : colTitle);
        sbHeader.Append(String.Format(@"<Cell ss:StyleID=""CH""><Data ss:Type=""String"">{0}</Data></Cell>", ss));
        cnt++;
      }
      // Save header of stream
      Byte[] bb = encoding.GetBytes(xmlHeader);
      stream.Write(bb, 0, bb.Length);
      // Save styles
      bb = encoding.GetBytes(sbStyle.ToString());
      stream.Write(bb, 0, bb.Length);
      // Save Worksheet header
      ss = decodeToXML(header);
      if (ss.Length > 30) ss = ss.Substring(0, 30);
      string s1 = String.Format(@"</Styles><Worksheet ss:Name=""{0}"">
<Table ss:ExpandedColumnCount=""{1}"" x:FullColumns=""1"" x:FullRows=""1"">", ss, colsToCopy.Length);
      bb = encoding.GetBytes(s1);
      stream.Write(bb, 0, bb.Length);
      bb = encoding.GetBytes(sbCols.ToString() + @"<Row ss:Style=""CH"">" + sbHeader.ToString() + @"</Row>");
      stream.Write(bb, 0, bb.Length);
      bool blankFlag = false;
      // Save data
      foreach (object o in objectsToCopy) {
        StringBuilder sbTmp = new StringBuilder("<Row>");
        int colCnt = 0;
        for (int i = 0; i < colsToCopy.Length; i++) {
          PropertyDescriptor pd = properties[i];
          sbTmp.Append(xmlFormatCell(pd == null ? null : pd.GetValue(o), colCnt++, ref blankFlag));
        }
        bb = encoding.GetBytes(sbTmp.ToString() + "</Row>");
        stream.Write(bb, 0, bb.Length);
      }
      // Save trailer
      string tmp = @"</Table><WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
              <Selected/><ProtectObjects>False</ProtectObjects>
        <ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet></Workbook>" + Environment.NewLine;
      bb = encoding.GetBytes(tmp);
      stream.Write(bb, 0, bb.Length);
    }

    private static string decodeToXML(string s) {
      return s.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
    }
    private static string xmlGetColumnFormat(DataGridViewColumn col) {
      string excelFormat = DGCore.Utils.ExcelApp.GetExcelFormatString(col.ValueType, col.InheritedStyle.Format);
      if (string.IsNullOrEmpty(excelFormat)) return @"<NumberFormat/>";
      else return String.Format(@"<NumberFormat ss:Format=""{0}""/>", excelFormat);
    }
    private static string xmlFormatCell(object dbValue, int index, ref bool blankFlag) {
      string s = @"<Cell{0}><Data{1}>{2}</Data></Cell>";//0-ColIndex, 1-Type, 2-Data
      string s1 = (blankFlag ? String.Format(@" ss:Index=""{0}""", index + 1) : "");
      blankFlag = false;
      if (dbValue == null || dbValue == DBNull.Value) {
        blankFlag = true; return "";
      }
      else if (dbValue is string) {
        return String.Format(s, s1, @" ss:Type=""String""", decodeToXML(dbValue.ToString()));
      }
      else if (dbValue is bool) {
        return String.Format(s, s1, @" ss:Type=""Boolean""", (bool)dbValue ? 1 : 0);
      }
      else if (DGCore.Utils.Types.IsNumericType(dbValue.GetType())) {
        return String.Format(s, s1, @" ss:Type=""Number""", decodeToXML(Convert.ToString(dbValue, fiNumberInvariant)));
      }
      else if (dbValue is DateTime) {
        DateTime dt = (DateTime)dbValue;
        return String.Format(s, s1, @" ss:Type=""DateTime""", dt.ToString("s") + ".000");
      }
      else {
        return String.Format(s, s1, @" ss:Type=""String""", "For " + dbValue.GetType().Name + " casting did not defined");
      }
    }

    //===========================================================
    public static bool SaveDGVToXLSFile(DataGridView dgv, string header, string[] subHeaders, string filename) {
      // return value: true if saved, false - if not saved (when records number> excel rowlimit and user canceled procedure)
      
      object[] objectsToCopy;
      DataGridViewColumn[] xColumns;
      DGVSelection.GetSaveArea(dgv, out objectsToCopy, out xColumns);
      List<DataGridViewColumn> columns = new List<DataGridViewColumn>(); ;
      int groupItemCountColumnNo = -1;
      for (int i=0; i<xColumns.Length; i++){
        if (! xColumns[i].Name.StartsWith("#group_")) columns.Add(xColumns[i]);
        if (xColumns[i].Name == "#group_ItemCount") {
          groupItemCountColumnNo = columns.Count;
          columns.Add(xColumns[i]);
        }
      }
/*      // Set group column numbers
      int[] groupColumnNumbers = new int[columns.Count];
      int groupItemCountColumnNo = -1;
      for (int i = 0; i < columns.Count; i++) {
        DataGridViewColumn col = columns[i];
        int groupNo;
        if ((col is DataGridViewTextBoxColumn) && String.IsNullOrEmpty(col.DataPropertyName) &&
          col.Name.StartsWith("#group_") && int.TryParse(col.Name.Substring(7), out groupNo)) {

          groupColumnNumbers[i] = groupNo+1;
        }
        else groupColumnNumbers[i] = -1;
        if (col.Name == "#group_ItemCount") groupItemCountColumnNo = i;
      }*/


      using (DGCore.Utils.ExcelApp excel = new DGCore.Utils.ExcelApp()) {

        // Get DGV data
//        IList dgvData = (IList)ListBindingHelper.GetList(dgv.DataSource, dgv.DataMember);
        int maxRow = Math.Min(excel.RowLimit - subHeaders.Length - 1, objectsToCopy.Length);
        if (maxRow < objectsToCopy.Length) {
          if (MessageBox.Show(String.Format("Програма Ексель може записати тільки {0} елементів із {1}. Записувати?",
            maxRow.ToString(), objectsToCopy.Length.ToString()), null, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) {
            excel.Book_Close(false);
            return false;
          }
        }

        // Write to file
        int headerRowNumber = subHeaders.Length + 1;
        //        DataGridViewColumn[] columns = Utils.DGV.GetColumnsInDisplayOrder(dgv, true);
        var excelSheetName = header.Replace(":", "").Replace("?", "").Replace("*", "").Replace("[", "")
          .Replace(":]", "").Replace("\\", "").Replace("/", "");
        if (excelSheetName.Length > 31)
          excelSheetName = excelSheetName.Substring(0, 31);

        excel.Sheet_Name = excelSheetName;
        excel.ScreenUpdating = false;
        //       excel.Visible = true;

        // Write subheaders
        for (int i = 0; i < subHeaders.Length; i++) {
//          excel.Range_SetCurrentByRegion(0, i + 1, columns.Count, 1);
//          excel.Range_Merge(Utils.ExcelApp.xlHorizontalAlignment.xlLeft);
          excel.Range_SetCurrentByCell(0,i+1);
          excel.Range_SetValue(subHeaders[i]);
        }
        // Write column header
        PropertyDescriptorCollection pdc = DGVUtils.GetInternalPropertyDescriptorCollection(dgv);
        PropertyDescriptor[] properties = new PropertyDescriptor[columns.Count];
        for (int i = 0; i < columns.Count; i++) {
          excel.Range_SetCurrentByColumn(i);
          excel.Range_Format = DGCore.Utils.ExcelApp.GetExcelFormatString(columns[i].ValueType, columns[i].InheritedStyle.Format);
          excel.Range_SetCurrentByCell(i, headerRowNumber);
          excel.Range_Format = "@";
          excel.Range_WrapText = true;
          excel.Range_SetHorisontalAlignment(DGCore.Utils.ExcelApp.xlHorizontalAlignment.xlCenter);
          excel.Range_SetVerticalAlignment(DGCore.Utils.ExcelApp.xlVerticalAlignment.xlVAlignCenter);
//          excel.Range_BackColor = Color.Gainsboro;
          excel.Range_SetBackColor (GetExcelColor(columns[i].HeaderCell.InheritedStyle.BackColor));
          string s = columns[i].DataPropertyName;
          if (!String.IsNullOrEmpty(s) && pdc[s] != null) {
            properties[i] = pdc[s];
          }
          if (!String.IsNullOrEmpty(columns[i].HeaderText)) {
            excel.Range_SetValue(columns[i].HeaderText);
          }
          excel.Range_SetBorder();
        }

        // Prepare data array
        object[,] xlData = new object[maxRow, columns.Count];
        Dictionary<int, List<int>> groupRowNos = new Dictionary<int, List<int>>();
        List<int> itemRowNos = new List<int>();
        for (int i2 = 0; i2 < maxRow; i2++) {
            DGCore.DGVList.IDGVList_GroupItem groupItem = objectsToCopy[i2] as DGCore.DGVList.IDGVList_GroupItem;
          if (groupItem == null) itemRowNos.Add(i2);
          else {
            int groupLevel = groupItem.Level;
            if (!groupRowNos.ContainsKey(groupLevel)) groupRowNos.Add(groupLevel, new List<int>());
            groupRowNos[groupLevel].Add(i2);
          }
          for (int i1 = 0; i1 < properties.Length; i1++) {
            PropertyDescriptor pd = properties[i1];
            if (pd != null) {
              if (pd.PropertyType.IsClass && pd.PropertyType != typeof(string)) {// Nested objects
                object o = pd.GetValue(objectsToCopy[i2]);
                xlData[i2, i1] = (o == null ? null : o.ToString());
              }
              else {
                xlData[i2, i1] = pd.GetValue(objectsToCopy[i2]);
              }
            }
            else if (objectsToCopy[i2] is DGCore.DGVList.IDGVList_GroupItem) {
                DGCore.DGVList.IDGVList_GroupItem x = (DGCore.DGVList.IDGVList_GroupItem)objectsToCopy[i2];
/*              if (x.Level==groupColumnNumbers[i1]) {
                xlData[i2, i1] = (x.IsExpanded ? "-" : "+");
              }*/
              if (i1 == groupItemCountColumnNo) xlData[i2, i1] = x.ItemCount;
            }
            if (object.Equals(xlData[i2, i1], double.NaN))
              xlData[i2, i1] = null;
          }
        }
        // Write data to Excel
        excel.Range_SetCurrentByRegion(0, headerRowNumber + 1, properties.Length, maxRow);
        excel.Range_SetValue(xlData);

        // Set row colors
        string rangeSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        if (dgv is DGV.DGVCube) {
          List<Pen> groupPens = ((DGV.DGVCube)dgv)._groupPens;
          /*if (itemRowNos.Count == 0) {// only group rows == maximum group must be draw as item row
            if (groupRowNos.Keys.Count > 0) {
              int i = System.Linq.Enumerable.Max<int>(groupRowNos.Keys);// define max group row no
              if (i > 0) {// reoder group info (row numbers of max group => item row numbers
                itemRowNos = groupRowNos[i];
                groupRowNos.Remove(i);
              }
            }
          }*/
          List<int> ii = new List<int>(groupRowNos.Keys);
          ii.Sort(); ii.Reverse();
          for (int i = 0; i < ii.Count; i++) {
            StringBuilder sb = new StringBuilder();
            bool flag = false;
            // Range string can not be > 256: we need to split string
            List<string> sRanges = new List<string>();
            foreach (int i1 in groupRowNos[ii[i]] ) {
              sb.Append((flag ? rangeSeparator : "") + DGCore.Utils.ExcelApp.GetRangeStringFromRegion(0, headerRowNumber + 1 + i1, properties.Length, 1));
              if (sb.Length > 230) {
                flag = false;
                sRanges.Add(sb.ToString());
                sb = new StringBuilder();
              }
              else flag = true;
            }
            if (sb.Length > 0) sRanges.Add(sb.ToString());
            foreach (string s in sRanges) {
              excel.Range_SetCurrentByString(s);
              excel.Range_SetBackColor (GetExcelColor(groupPens[ii[i]].Color));
              excel.Range_SetBorder();
              if (i > 0) {// start to set bold font only from second group
                excel.Range_SetFont(true, i + 7);
              }
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
/*        if (columns.Count > 2) {// Merge cells
          excel.Range_SetCurrentByRegion(0, 0, columns.Count - 1, 1);
          excel.Range_Merge(Utils.ExcelApp.xlHorizontalAlignment.xlLeft);
        }
        else {
          excel.Range_SetCurrentByCell(0, 0);
        }*/
        excel.Range_SetCurrentByCell(0, 0);
        excel.Range_SetValue(header);
        excel.Range_SetFont(true, 12);
        // Write current datetime
        excel.Range_SetCurrentByCell(Math.Max(1, columns.Count - 1), 0);
        excel.Range_SetValue(DateTime.Now);
        excel.Range_SetHorisontalAlignment(DGCore.Utils.ExcelApp.xlHorizontalAlignment.xlRight);
        excel.Range_Format = DGCore.Utils.ExcelApp.GetExcelDateTimeFormatFromVSFormatString("G");
        // Clear selection
        excel.Range_SetCurrentByCell(0, headerRowNumber);
        excel.Range_Select();
        // Save file
        excel.Book_Save(filename, true);
      }
      return true;
    }

    public static int GetExcelColor(Color color)
    {
      //      return (color.R << 16) + (color.G << 8) + color.B;
      return (color.B << 16) + (color.G << 8) + color.R;
    }


  }
}
