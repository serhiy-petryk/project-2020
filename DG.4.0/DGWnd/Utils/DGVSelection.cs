using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace DGWnd.Utils {
  public class DGVSelection {

    public static void Statistics_Recalculate(DataGridView dgv, out int count, out double min, out double max, out double sum, out double average) {
      IEnumerable<DataGridViewCell> selectedCells;
      IEnumerable<int> selectedBands;
      GetSelectedEnumerables(dgv, out selectedCells, out selectedBands);
      PropertyDescriptorCollection pdc = DGVUtils.GetInternalPropertyDescriptorCollection(dgv);
      PropertyDescriptor[] properties = new PropertyDescriptor[dgv.Columns.Count];
      int livePropertiesCount = 0;
      for (int i = 0; i < dgv.Columns.Count; i++) {
        string s = dgv.Columns[i].DataPropertyName;
        if (!string.IsNullOrEmpty(s)) {
          Type valueType = DGCore.Utils.Types.GetNotNullableType(pdc[s].PropertyType);
          if (valueType == typeof(char) || valueType == typeof(byte) || valueType == typeof(sbyte) || valueType == typeof(short) ||
            valueType == typeof(ushort) || valueType == typeof(int) || valueType == typeof(uint) || valueType == typeof(long) ||
            valueType == typeof(ulong) || valueType == typeof(decimal) || valueType == typeof(float) || valueType == typeof(double)) {

            livePropertiesCount++;
            properties[i] = pdc[s];
          }
        }
      }
      PropertyDescriptor[] liveProperties = new PropertyDescriptor[livePropertiesCount];
      livePropertiesCount = 0;
      for (int i = 0; i < properties.Length; i++) {
        if (properties[i]!=null) liveProperties[livePropertiesCount++]= properties[i];
      }

      IList data = (IList)ListBindingHelper.GetList(dgv.DataSource, dgv.DataMember);

      double min1 = double.MaxValue;
      double max1 = double.MinValue;
      double sum1 = 0;
      int count1 = 1;
      double min2 = double.MaxValue;
      double max2 = double.MinValue;
      double sum2 = 0;
      int count2 = 1;

      switch (dgv.SelectionMode) {
        case DataGridViewSelectionMode.CellSelect:
          CalculateByCells(data, selectedCells, properties, out count1, out sum1, out min1, out max1);
          break;
        case DataGridViewSelectionMode.RowHeaderSelect:
          CalculateByCells(data, selectedCells, properties, out count1, out sum1, out min1, out max1);
          CalculateByRows(data, selectedBands, liveProperties, out count2, out sum2, out min2, out max2);
          break;
        case DataGridViewSelectionMode.FullRowSelect:
          CalculateByRows(data, selectedBands, liveProperties, out count2, out sum2, out min2, out max2);
          break;
        case DataGridViewSelectionMode.ColumnHeaderSelect:
          CalculateByCells(data, selectedCells, properties, out count1, out sum1, out min1, out max1);
          CalculateByColumns(data, selectedBands, properties, out count2, out sum2, out min2, out max2);
          break;
        case DataGridViewSelectionMode.FullColumnSelect:
          CalculateByColumns(data, selectedBands, properties, out count2, out sum2, out min2, out max2);
          break;

      }
      if (count1 > 0 && count2 == 0) {
        min = min1; max = max1; sum = Math.Round(sum1, 6); average = Math.Round(sum1 / count1, 6); count = count1;
      }
      else if (count1 == 0 && count2 > 0) {
        min = min2; max = max2; sum = Math.Round(sum2, 6); average = Math.Round(sum2 / count2, 6); count = count2;
      }
      else if (count1 > 0 && count2 > 0) {
        min = Math.Min(min1, min2); max = Math.Max(max1, max2);
        sum = Math.Round(sum1 + sum2, 6); count = count1 + count2; average = Math.Round(sum / count, 6);
      }
      else {
        min = double.NaN; max = double.NaN; sum = double.NaN; average = double.NaN; count = 0;
      }
    }

    static void CalculateByCells(IList data, IEnumerable<DataGridViewCell> cells, PropertyDescriptor[] properties, out int iCount, out double dSum, out double dMin, out double dMax) {
      double min = double.MaxValue;
      double max = double.MinValue;
      double sum = 0;
      int count = 0;

      foreach (DataGridViewCell cell in cells) {
//        if (cell.RowIndex >= 0 && cell.RowIndex < data.Count && cell.ColumnIndex >= 0 && cell.ColumnIndex <= properties.Length) {
          if (properties[cell.ColumnIndex] != null) {
            object o = properties[cell.ColumnIndex].GetValue(data[cell.RowIndex]);
            if (o != null) {
              double d = (o is double? (double)o: Convert.ToDouble(o));
              if (!Double.IsNaN(d)) {
                count++;
                if (min > d) min = d;
                if (max < d) max = d;
                sum += d;
              }
            }
          }
        }
  //    }
      iCount = count;
      dSum = sum;
      dMin = min;
      dMax = max;
    }

    static void CalculateByRows(IList data, IEnumerable<int> rowNumbers, PropertyDescriptor[] properties, out int iCount, out double dSum, out double dMin, out double dMax) {
      double min = double.MaxValue;
      double max = double.MinValue;
      double sum = 0;
      int count = 0;
      int maxRowNo = data.Count;

      foreach (int rowNo in rowNumbers) {
      //  if (rowNo <= maxRowNo) {
          foreach (PropertyDescriptor pd in properties) {
              object o = pd.GetValue(data[rowNo]);
              if (o != null) {
                double d = (o is double? (double)o: Convert.ToDouble(o));
                if (!Double.IsNaN(d)) {
                  count++;
                  if (min > d) min = d;
                  if (max < d) max = d;
                  sum += d;
                }//if (!Double.IsNaN(d)) {
              }//if (o != null) {
          }//foreach (PropertyDescriptor pd
       // }
      }//foreach (int rowNo
      iCount = count;
      dSum =  sum;
      dMin =  min;
      dMax =max;
    }

    static void CalculateByColumns(IList data, IEnumerable<int> colNumbers, PropertyDescriptor[] properties, out int iCount, out double dSum, out double dMin, out double dMax) {
      double min = double.MaxValue;
      double max = double.MinValue;
      double sum = 0;
      int count = 0;

      foreach (int colNo in colNumbers) {
        if (colNo >= 0 && colNo <= properties.Length) {
          PropertyDescriptor pd = properties[colNo];
          if (pd != null) {
            foreach (object item in data) {
              object o = pd.GetValue(item);
              if (o != null) {
                double d = (o is double? (double)o: Convert.ToDouble(o));
                if (!Double.IsNaN(d)) {
                  count++;
                  if (min > d) min = d;
                  if (max < d) max = d;
                  sum += d;
                }
              }
            }
          }
        }
      }
      iCount = count;
      dSum = sum;
      dMin = min;
      dMax = max;
    }

    public static void GetSelectedEnumerables(DataGridView dgv, out IEnumerable<DataGridViewCell> selectedCells, out IEnumerable<int> selectedBands) {
      FieldInfo fi1 = typeof(DataGridView).GetField("_individualSelectedCells", BindingFlags.Instance | BindingFlags.NonPublic);
      FieldInfo fi2 = typeof(DataGridView).GetField("_selectedBandIndexes", BindingFlags.Instance | BindingFlags.NonPublic);
      IEnumerable cells = (IEnumerable)fi1.GetValue(dgv);
      IEnumerable bands = (IEnumerable)fi2.GetValue(dgv);
      selectedCells = System.Linq.Enumerable.Cast<DataGridViewCell>(cells);
      selectedBands = System.Linq.Enumerable.Cast<int>(bands);
    }


    //=====================================================================
    // GetSelectedArea: out = rowNumbers, objects, columns 
    public static void GetSelectedArea(DataGridView dgv, out int[] selectedRowNumbers, out object[] selectedObjects, out DataGridViewColumn[] selectedColumnsInDisplayOrder) {
      GetSelectedArea(dgv, out selectedRowNumbers, out selectedColumnsInDisplayOrder);
      selectedObjects = new object[selectedRowNumbers.Length];
      object data = ListBindingHelper.GetList(dgv.DataSource, dgv.DataMember);
      if (data is IList) {
        IList data1 = (IList)data;
        for (int i = 0; i < selectedObjects.Length; i++) selectedObjects[i] = data1[selectedRowNumbers[i]];
      }
      else {
        throw new Exception("AAA");
      }
    }

    // GetSelectedArea: out = objects, columns 
    public static void GetSelectedArea(DataGridView dgv,  out object[] selectedObjects, out DataGridViewColumn[] selectedColumnsInDisplayOrder) {
      int[] selectedRowNumbers;
      GetSelectedArea(dgv,  out selectedRowNumbers, out selectedObjects, out selectedColumnsInDisplayOrder);
      selectedRowNumbers = null;
    }

    // GetSelectedArea: out = rowNumbers, columns 
    public static void GetSelectedArea(DataGridView dgv,  out int[] selectedRowNumbers, out DataGridViewColumn[] selectedColumnsInDisplayOrder) {
      // DataSource of Datagridview must be IList
      List<int> rows = new List<int>();
      List<int> cols = new List<int>();
      DataGridViewColumn[] cc = DGVUtils.GetColumnsInDisplayOrder(dgv, true);

      FieldInfo fi1 = typeof(DataGridView).GetField("_individualSelectedCells", BindingFlags.Instance | BindingFlags.NonPublic);
      FieldInfo fi2 = typeof(DataGridView).GetField("_selectedBandIndexes", BindingFlags.Instance | BindingFlags.NonPublic);
      IEnumerable cells = (IEnumerable)fi1.GetValue(dgv);
      IEnumerable bands = (IEnumerable)fi2.GetValue(dgv);

      switch (dgv.SelectionMode) {
        case DataGridViewSelectionMode.CellSelect:
          foreach (DataGridViewCell cell in cells) {
            if (!rows.Contains(cell.RowIndex)) rows.Add(cell.RowIndex);
            if (!cols.Contains(cell.ColumnIndex)) cols.Add(cell.ColumnIndex);
          }
          break;

        case DataGridViewSelectionMode.FullRowSelect:
        case DataGridViewSelectionMode.RowHeaderSelect:
          foreach (int i in bands) rows.Add(i);
          if (rows.Count > 0) {
            foreach (DataGridViewColumn c in cc) cols.Add(c.Index);
          }
          if (dgv.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect) {
            foreach (DataGridViewCell cell in cells) {
              if (!rows.Contains(cell.RowIndex)) rows.Add(cell.RowIndex);
              if (!cols.Contains(cell.ColumnIndex)) cols.Add(cell.ColumnIndex);
            }
          }
          break;

        case DataGridViewSelectionMode.FullColumnSelect:
        case DataGridViewSelectionMode.ColumnHeaderSelect:
          foreach (int i in bands) cols.Add(i);
          if (cols.Count > 0) {
            for (int i = 0; i < dgv.Rows.Count; i++) rows.Add(i);
          }
          if (dgv.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect) {
            foreach (DataGridViewCell cell in cells) {
              if (!rows.Contains(cell.RowIndex)) rows.Add(cell.RowIndex);
              if (!cols.Contains(cell.ColumnIndex)) cols.Add(cell.ColumnIndex);
            }
          }
          break;
      }
      // Prepare object list
      rows.Sort();
      // Remove new row number
      if (dgv.Rows.Count > 0 && dgv.Rows[dgv.Rows.Count - 1].IsNewRow && rows.Count > 0) {
        if (rows[rows.Count - 1] == dgv.Rows.Count - 1) rows.RemoveAt(rows.Count - 1);
      }
      // Prepare column list
     /* if (onlyDataColumns) {
        for (int i = 0; i < cols.Count; i++) {
          if (String.IsNullOrEmpty(dgv.Columns[cols[i]].DataPropertyName)) cols.RemoveAt(i--);
        }
      }*/
      selectedColumnsInDisplayOrder = new DataGridViewColumn[cols.Count];
      int cnt = 0;
      foreach (DataGridViewColumn c in cc) {
        if (cols.Contains(c.Index)) selectedColumnsInDisplayOrder[cnt++] = c;
      }
      selectedRowNumbers = rows.ToArray();
    }

    //GetSaveArea
    public static void GetSaveArea(DataGridView dgv,  out object[] selectedObjects, out DataGridViewColumn[] selectedColumnsInDisplayOrder) {
      GetSelectedArea(dgv, out selectedObjects, out selectedColumnsInDisplayOrder);
      if (selectedObjects.Length == 1 && selectedObjects.Length == 1) {// Selected only 1 cell == save all file
        selectedColumnsInDisplayOrder = DGVUtils.GetColumnsInDisplayOrder(dgv, true);
        object data = ListBindingHelper.GetList(dgv.DataSource, dgv.DataMember);
        if (data is IList) {
          IList data1 = (IList)data;
          if (dgv is DGV.DGVCube) {
            DGV.DGVCube x = (DGV.DGVCube)dgv;
            List<object> oo = new List<object>();
            for (int i = 0; i < data1.Count; i++) {
              if (x._IsItemVisible(data1[i])) oo.Add(data1[i]);
            }
            selectedObjects = oo.ToArray();
          }
          else {
            selectedObjects = new object[data1.Count];
            for (int i = 0; i < data1.Count; i++) selectedObjects[i] = data1[i];
          }
        }
        else {
          throw new Exception("AAA");
        }
      }
    }
  }
}
