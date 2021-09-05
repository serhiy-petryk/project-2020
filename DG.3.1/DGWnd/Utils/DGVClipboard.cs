using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace DGWnd.Utils {
  public static class DGVClipboard {

    public static void Clipboard_Attach(DataGridView dgv) {
      dgv.KeyDown -= new KeyEventHandler(clipboard_dgv_KeyDown);
      dgv.KeyDown += new KeyEventHandler(clipboard_dgv_KeyDown);
    }
    static void clipboard_dgv_KeyDown(object sender, KeyEventArgs e) {
      DataGridView dgv = (DataGridView)sender;
      if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert)) {
        object data = ListBindingHelper.GetList(dgv.DataSource, dgv.DataMember);
        if (data != null) Clipboard_CopyByObjectsAsText(dgv);
        //        if (data is IBindingList) Clipboard_CopyByObjects(dgv);
        else Clipboard_CopyByCellsAsText(dgv);
        e.Handled = true;
      }
      else if (e.Control && e.KeyCode == Keys.V && !dgv.ReadOnly) {
        Clipboard_Paste(dgv);
        e.Handled = true;
      }
    }

    //==================
    public static void Clipboard_CopyByCellsAsText(DataGridView dgv) {
      Clipboard.Clear();// Clear memory after previous Ctr+C
      int[] selectedRows;
      DataGridViewColumn[] selectedColumns;
      DGVSelection.GetSelectedArea(dgv, out selectedRows, out selectedColumns);
      string[] ss1 = new string[selectedRows.Length];
      int i1 = 0;
      if (selectedRows.Length == 1 && selectedColumns.Length == 1) {
        // Single cell
        DataGridViewCell cell = dgv.Rows[selectedRows[0]].Cells[selectedColumns[0].Index];
        if (cell.FormattedValue is Bitmap) Clipboard.SetImage((Bitmap)cell.FormattedValue);
        else Clipboard.SetDataObject(cell.Value);
      }
      else       {
        // Many cells
        foreach (int i in selectedRows) {
          string[] ss2 = new string[selectedColumns.Length];
          int i2 = 0;
          foreach (DataGridViewColumn col in selectedColumns) {
            DataGridViewCell cell = dgv.Rows[i].Cells[col.Index];
            string s = null;
            if (cell.Value == null || cell.Value == DBNull.Value || cell.Value.GetType().IsArray) s = null;//cell.Value.GetType().IsArray for Image(Byte[])
            else {
              TypeConverter tc = TypeDescriptor.GetConverter(cell.Value.GetType());
              if (tc != null && tc.CanConvertTo(typeof(string))) s = tc.ConvertToString(cell.Value);
            }
            ss2[i2++] = s;
          }
          ss1[i1++] = String.Join("\t", ss2);
        }
        Clipboard.SetDataObject(String.Join(Environment.NewLine, ss1));
      }
    }

    static string cellPrefix = ((char)0xA0).ToString();
    public static void Clipboard_CopyByObjectsAsText(DataGridView dgv) {
      Stopwatch sw = new Stopwatch();
      sw.Start();

      Clipboard.Clear();// Clear memory after previous Ctr+C
      object[] objectsToCopy;
      DataGridViewColumn[] colsToCopy;
      Utils.DGVSelection.GetSelectedArea(dgv, out objectsToCopy, out colsToCopy);
      Utils.DGVColumnHelper[] helpers = new Utils.DGVColumnHelper[colsToCopy.Length];
      for (int i = 0; i < colsToCopy.Length; i++) {
        helpers[i] = new Utils.DGVColumnHelper(colsToCopy[i]);
      }
      if (objectsToCopy.Length == 1 && helpers.Length == 1) {
        // Single cell
        object o1 = helpers[0].GetFormattedValueFromItem(objectsToCopy[0], true);
        if (o1 is Bitmap) Clipboard.SetImage((Bitmap)o1);
        else if (o1!=null) Clipboard.SetDataObject(o1);
      }
      else {
        // Many cells
        List<string> ss1 = new List<string>();
        // Column headers
        string[] ss3 = new string[colsToCopy.Length];
        for (int i = 0; i < ss3.Length; i++) ss3[i] = colsToCopy[i].HeaderText;
        ss1.Add(String.Join("\t", ss3));
        // Data
        foreach (object o in objectsToCopy) {
          string[] ss2 = new string[helpers.Length];
          for (int i = 0; i < helpers.Length; i++) {
            object o1 = helpers[i].GetFormattedValueFromItem(o,true);
            if (o1 is string) {
              string s1 = (string)o1;
              if (helpers[i].PropertyDescriptor.PropertyType == typeof(string)) {// Check for string type == formatted value not like number or date
                if (s1.StartsWith(cellPrefix)) ss2[i] = s1;
                else ss2[i] = cellPrefix + s1;
              }
              else ss2[i] = s1;
            }
            else ss2[i] = null;
          }
          ss1.Add( string.Join("\t", ss2));
        }
        sw.Stop();
        double d1 = sw.Elapsed.TotalMilliseconds;
//        sw.Reset();
  //      sw.Start();
        Clipboard.SetDataObject(string.Join(Environment.NewLine, ss1.ToArray()));
        sw.Stop();
        double d2 = sw.Elapsed.TotalMilliseconds;
      }
    }

    public static void Clipboard_Paste(DataGridView dgv) {
      String HtmlFormat = Clipboard.GetData("HTML Format") as String;
      List<List<string>> rowContents = new List<List<string>>();
      if (HtmlFormat != null) {
        // Remove html tags to just extract row information and store it in rowContents
        System.Text.RegularExpressions.Regex TRregex = new System.Text.RegularExpressions.Regex(@"<( )*tr([^>])*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        System.Text.RegularExpressions.Regex TDregex = new System.Text.RegularExpressions.Regex(@"<( )*td([^>])*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        System.Text.RegularExpressions.Match trMatch = TRregex.Match(HtmlFormat);
        while (!String.IsNullOrEmpty(trMatch.Value)) {
          int rowStart = trMatch.Index + trMatch.Length;
          int rowEnd = HtmlFormat.IndexOf("</tr>", rowStart, StringComparison.OrdinalIgnoreCase);
          System.Text.RegularExpressions.Match tdMatch = TDregex.Match(HtmlFormat, rowStart, rowEnd - rowStart);
          List<string> rowContent = new List<string>();
          while (!String.IsNullOrEmpty(tdMatch.Value)) {
            int cellStart = tdMatch.Index + tdMatch.Length;
            int cellEnd = HtmlFormat.IndexOf("</td>", cellStart, StringComparison.OrdinalIgnoreCase);
            String cellContent = HtmlFormat.Substring(cellStart, cellEnd - cellStart);
            cellContent = System.Text.RegularExpressions.Regex.Replace(cellContent, @"<( )*br( )*>", "\r\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            cellContent = System.Text.RegularExpressions.Regex.Replace(cellContent, @"<( )*li( )*>", "\r\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            cellContent = System.Text.RegularExpressions.Regex.Replace(cellContent, @"<( )*div([^>])*>", "\r\n\r\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            cellContent = System.Text.RegularExpressions.Regex.Replace(cellContent, @"<( )*p([^>])*>", "\r\n\r\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            cellContent = System.Web.HttpUtility.HtmlDecode(cellContent);
//            cellContent = cellContent.Replace("&nbsp;", " ");
            rowContent.Add(cellContent);
            tdMatch = tdMatch.NextMatch();
          }
          if (rowContent.Count > 0) {
            rowContents.Add(rowContent);
          }
          trMatch = trMatch.NextMatch();
        }
      }
      else {
        // Clipboard is not in html format, read as text
        String CopiedText = Clipboard.GetText();
//        String[] lines = CopiedText.Split('\n');
        String[] lines = CopiedText.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines) {
          List<string> rowContent = new List<string>(line.Split('\t'));
          if (rowContent.Count > 0) {
            rowContents.Add(rowContent);
          }
        }
      }//if (HtmlFormat != null) {

      // Check rowContents
      if (rowContents.Count == 0) return; // no paste rows
      int colsInContent = rowContents[0].Count;
      foreach (List<string> ss in rowContents) {
        if (ss.Count != colsInContent) return;
      }

      DataGridViewColumn[] cols = DGCore.Utils.Dgv.GetColumnsInDisplayOrder(dgv, true);
      int iRow = dgv.CurrentCell.RowIndex;
      int iColumn = dgv.CurrentCell.ColumnIndex;

      dgv.SuspendLayout();

      // TO DO: Переделать сделать через делегаты (для каждого типа свой делегат), 
      // в случае IBindingList сделать insert item + update properties
      // может быть для своего типа Datasource, использовать специальную функцию типа BatchInsert
      object ds = ListBindingHelper.GetList(dgv.DataSource, dgv.DataMember);
      IBindingList bindingList = ds as IBindingList;
//      if (bindingList != null) Binding_Suspend(bindingList);
      if (dgv.CurrentRow.IsNewRow) {
        if (bindingList == null) {
          dgv.Rows.Add(rowContents.Count);
        }
        else {
          bindingList.RemoveAt(bindingList.Count - 1); /// !!! Run before add new rows
          for (int i = 0; i < rowContents.Count; i++) {
            Object obj = bindingList.AddNew();
          }
        }
      }

//      Utils.DGV.Binding_Resume(ds);
      // Adjust current row
      if (dgv.CurrentRow.Index != iRow || dgv.CurrentCell.ColumnIndex != iColumn) {
        dgv.CurrentCell = dgv[iColumn, iRow];
      }

      // paste the data starting at the current cell
      //??? Display index is valid for invisible columns   int iCol = dgv.CurrentCell.OwningColumn.DisplayIndex;
      int iStartColumn = Array.IndexOf(cols, dgv.CurrentCell.OwningColumn);
      if (dgv.CurrentRow.Selected) iStartColumn = 0;// If row is selected, then start cell must be the first cell in row
      foreach (List<String> rowContent in rowContents) {
        if (iRow >= dgv.Rows.Count || dgv.Rows[iRow].IsNewRow) {
          break;
        }
        int iCol = iStartColumn;
        foreach (String cellContent in rowContent) {
          try {
            if (iCol < cols.Length) {
              DataGridViewCell cell = dgv[cols[iCol].Index, iRow];
              if (!cell.ReadOnly) {
                cell.Value = cell.ParseFormattedValue(cellContent, cell.InheritedStyle, null, TypeDescriptor.GetConverter(cell.ValueType));
                if (cell is DataGridViewComboBoxCell) {// Add new value to ComboBoxCell items if it does not have it
                  DataGridViewComboBoxCell cbCell = (DataGridViewComboBoxCell)cell;
                  if (cell.Value != null && !cbCell.Items.Contains(cell.Value)) {
                    cbCell.Items.Add(cell.Value);
                  }
                }
              }
            }
          }
          catch (Exception ex) {
          }
          iCol++;
        }
        iRow++;
      }
      //if (bindingList != null) Binding_Resume(bindingList);
      dgv.ResumeLayout(true);
      // Refresh & Invalidate need to update item column in dgvFilter of UC_Filter.cs control
      dgv.TopLevelControl.Refresh();
      dgv.TopLevelControl.Invalidate();
    }

    static void Binding_Suspend(object bindingList) {
      // get bindingList: ListBindingHelper.GetList(this.DataSource, this.DataMember);
      if (bindingList is BindingSource) {
        ((BindingSource)bindingList).SuspendBinding();
      }
      else {
        PropertyInfo pi = bindingList.GetType().GetProperty("RaiseListChangedEvents", BindingFlags.Instance | BindingFlags.Public);
        MethodInfo mi = bindingList.GetType().GetMethod("ResetBindings", BindingFlags.Instance | BindingFlags.Public);
        if (pi != null && mi != null) {// BindingList<>
          pi.SetValue(bindingList, false, null);
        }
        else {// ? DataView
        }
      }
    }

    static void Binding_Resume(object bindingList) {
      // get bindingList: ListBindingHelper.GetList(this.DataSource, this.DataMember);
      if (bindingList is BindingSource) {
        ((BindingSource)bindingList).ResetBindings(false);
        ((BindingSource)bindingList).ResumeBinding();
      }
      else {
        PropertyInfo pi = bindingList.GetType().GetProperty("RaiseListChangedEvents", BindingFlags.Instance | BindingFlags.Public);
        MethodInfo mi = bindingList.GetType().GetMethod("ResetBindings", BindingFlags.Instance | BindingFlags.Public);
        if (pi != null && mi != null) {// BindingList<>
          pi.SetValue(bindingList, true, null);
          mi.Invoke(bindingList, null);
        }
        else {// ? DataView
        }
      }
    }

  }
}
