using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DGWnd.Utils;

namespace DGWnd.UI {
  public partial class frmSetFilter : Form {

    DGCore.Filters.FilterLineBase _filterItem = null;
    bool _isValueListExclusive = false;
    ICollection _dataSource;

    private frmSetFilter() {
      InitializeComponent();
//      Utils.DGVClipboard.Clipboard_Attach(this.dgvList);
  //    Utils.Dgv.CreateComboColumnsForEnumerations(this.dgvList);
    }

    public frmSetFilter(ICollection dataSource, DGCore.Filters.FilterLineBase filterItem):this() {
      this._filterItem = filterItem;
      this._dataSource = dataSource;
    }
    private void frmSetFilter_Load(object sender, EventArgs e) {
      Utils.DGVClipboard.Clipboard_Attach(this.dgvList);
      this.cbNot.Checked = this._filterItem.Not;
      this.dgvList.DataSource = _filterItem.FrmItems;
      _filterItem.FrmItems.Clear();
      foreach (DGCore.Filters.FilterLineSubitem o in _filterItem.Items) {
        _filterItem.FrmItems.Add(o);
      }
      DGVUtils.CreateComboColumnsForEnumerations(this.dgvList);
      Ini();
      // Bug!! see http://stackoverflow.com/questions/14934003/system-invalidoperationexception-this-operation-cannot-be-performed-while-an-au
      this.dgvList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    }


    void Ini() {
      // Refresh item list for combobox
      List<object> xx = new List<object>();
      /* if (this._dataSource != null) {
        if (_filterItem is Misc.FilterList.FilterLine_Item ) {//&& this.dgvFilter.Visible) {
          // Get sorted distinct values from object list
          Misc.FilterList.FilterLine_Item item = (Misc.FilterList.FilterLine_Item) _filterItem;
          IEnumerable<object> e0 = Enumerable.Cast<object>(this._dataSource);
          IEnumerable<object> e1 = Enumerable.Select<object, object>(e0, delegate(object o1) { return item._pd.GetValue(o1); });
          IEnumerable<object> e2 = Enumerable.Distinct<object>(e1);
          xx = Enumerable.ToList<object>(e2);
          for (int i = 0; i < xx.Count; i++) {
            if (xx[i] == null) xx.RemoveAt(i--);
          }
          // Add items from filter item object
          foreach (Misc.FilterList.FilterLineSubitem x1 in _filterItem.Items) {
            if (x1.Value1 != null && !xx.Contains(x1.Value1)) xx.Add(x1.Value1);
            if (x1.Value2 != null && !xx.Contains(x1.Value2)) xx.Add(x1.Value2);
          }
          xx.Sort();
        }
      }*/
      // Set value type for DGV combobox columns
      this.colCB_Value1.CellTemplate.ValueType = _filterItem.PropertyType;
      this.colCB_Value2.CellTemplate.ValueType = _filterItem.PropertyType;
      this.Col_value1.CellTemplate.ValueType = _filterItem.PropertyType;
      this.Col_value2.CellTemplate.ValueType = _filterItem.PropertyType;

      if (xx.Count == 0) {
        this.Col_value1.Visible = true;
        this.Col_value2.Visible = true;
        this.colCB_Value1.Visible = false;
        this.colCB_Value2.Visible = false;
      }
      else {
        this.Col_value1.Visible = false;
        this.Col_value2.Visible = false;
        this.colCB_Value1.Visible = true;
        this.colCB_Value2.Visible = true;
        // Init value list for DGV combobox columns
        this.colCB_Value1.Items.Clear();
        this.colCB_Value1.Items.AddRange(xx.ToArray());
        this.colCB_Value2.Items.Clear();
        this.colCB_Value2.Items.AddRange(xx.ToArray());
      }
    }

    private void dgvList_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e) {
      // Refresh list of operands for Operand column
      DataGridViewCell cell = this.dgvList.Rows[e.RowIndex].Cells[e.ColumnIndex];
      if (cell.ValueType == typeof(DGCore.Common.Enums.FilterOperand)) {
//        BindingSource bs = (BindingSource)this.dgvList.DataSource;
  //      Misc.FilterObject.FilterLineBase x = bs.Current as Misc.FilterObject.FilterLineBase;
        // does not work with new rows         UserControls.PDCFilter.FilterElement.ListElement x = cell.OwningRow.DataBoundItem as UserControls.PDCFilter.FilterElement.ListElement;
        if (_filterItem != null)
            ((DataGridViewComboBoxCell) cell).DataSource = _filterItem.PossibleOperands;
      }
    }

    private void dgvList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
      // DGVComboBoxCell is going to edit mode by F2: do not clear the cell context when date has not null miliseconds
      if (e.Value is DateTime && String.IsNullOrEmpty(e.CellStyle.Format)) {
        e.CellStyle.Format = (((DateTime)e.Value).TimeOfDay == TimeSpan.Zero ? "d" : "g");
      }

    }

    private void dgvList_CellParsing(object sender, DataGridViewCellParsingEventArgs e) {
      // Occur after Apply button pressed (dgv.EndEdit command is issued) (cb_Leave is not raising in this case)
      object cellValue = OnComboBoxControlLeave();
      if (cellValue != null) {
        e.Value = cellValue;
        e.ParsingApplied = true;
      }

    }

    private void dgvList_DataError(object sender, DataGridViewDataErrorEventArgs e) {

    }

    private void dgvList_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e) {
      DataGridView dgv = (DataGridView)sender;
      if (e.Control is ComboBox && !dgv.CurrentCell.ValueType.IsEnum) {
        ComboBox cb = (ComboBox)e.Control;
        cb.DropDownStyle = (_isValueListExclusive ? ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown);
        cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        cb.AutoCompleteSource = AutoCompleteSource.ListItems;
        cb.CausesValidation = true;
        cb.Leave += new EventHandler(cb_Leave); // add new item into combobox items list
        cb.Validated += new EventHandler(cb_Validated);// commit changes
      }

    }

    private void dgvList_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
      DataGridView dgv = (DataGridView)sender;
      DataGridViewRow r = dgv.Rows[e.RowIndex];
      // Set backColor for cells (editable: LightCyan; readonly: WhiteSmoke);
      foreach (DataGridViewCell cell in r.Cells) {
        if (cell.ReadOnly) cell.Style.BackColor = Color.WhiteSmoke;
        else cell.Style.BackColor = Color.LightCyan;
      }
    }

    void cb_Validated(object sender, EventArgs e) {
      IDataGridViewEditingControl c = (IDataGridViewEditingControl)sender;
      DataGridView dgv = c.EditingControlDataGridView;
      // Commit changes
      if (dgv.CurrentCell != null && dgv.CurrentCell.IsInEditMode) {
        dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
      }
    }

    // =====  Combobox: leave, formating, check new item
    void cb_Leave(object sender, EventArgs e) {
      object value = OnComboBoxControlLeave();
      if (value == null && dgvList.CurrentCell != null) {
        dgvList.CurrentCell.Value = dgvList.CurrentCell.InheritedStyle.DataSourceNullValue;
      }
    }

    object OnComboBoxControlLeave() {
      object value = null;
      if (this.dgvList.CurrentCell != null && this.dgvList.CurrentCell is DataGridViewComboBoxCell) {
        try {
          string dpName = dgvList.CurrentCell.OwningColumn.DataPropertyName;
          if (dpName.StartsWith("Value")) {
              DGCore.Filters.FilterLine_Item item = _filterItem as DGCore.Filters.FilterLine_Item;
            value = DGCore.Utils.Tips.ConvertTo(dgvList.CurrentCell.EditedFormattedValue, dgvList.CurrentCell.ValueType, item._pd.Converter);
          }
          else {
            value = DGCore.Utils.Tips.ConvertTo(dgvList.CurrentCell.EditedFormattedValue, dgvList.CurrentCell.ValueType, null);
          }
        }
        catch (Exception ex) {
        }
      }
      if (value != null && value != DBNull.Value) {
        ComboBox cb = this.dgvList.EditingControl as ComboBox;
        if (!cb.Items.Contains(value)) {
          // Add new item into combobox items list
          DataGridViewComboBoxColumn col = this.dgvList.CurrentCell.OwningColumn as DataGridViewComboBoxColumn;
          DataGridViewComboBoxCell.ObjectCollection oo1 = ((DataGridViewComboBoxCell)col.CellTemplate).Items;
          oo1.Add(value);// Add new item to template and current cells 
          MethodInfo miSort = oo1.GetType().GetMethod("SortInternal", BindingFlags.Instance | BindingFlags.NonPublic);
          miSort.Invoke(oo1, null); // sort items of template cell
          oo1 = ((DataGridViewComboBoxCell)dgvList.CurrentCell).Items;// sort items of current cell
          miSort.Invoke(oo1, null);
          cb.Items.Add(value);
        }
      }
      return value;
    }

    public static void CreateComboColumnsForEnumerations(DataGridView dgv) {
      for (int i = 0; i < dgv.Columns.Count; i++) {
        DataGridViewColumn col = dgv.Columns[i];
        if (col.ValueType != null && col.ValueType.IsEnum && col is DataGridViewTextBoxColumn) {
          // create combo column for enum types
          DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
          cmb.ValueType = col.ValueType;
          cmb.Name = col.Name;
          cmb.DataPropertyName = col.DataPropertyName;
          cmb.HeaderText = col.HeaderText;
          cmb.DisplayStyleForCurrentCellOnly = true;//???
          Array values = Enum.GetValues(col.ValueType);
          cmb.DataSource = values;
          cmb.MaxDropDownItems = Math.Min(22, values.Length);

          // Copy the default style
          PropertyInfo[] pis = col.DefaultCellStyle.GetType().GetProperties();
          foreach (PropertyInfo pi in pis) {
            if (pi.CanRead && pi.CanWrite) {
              pi.SetValue(cmb.DefaultCellStyle, pi.GetValue(col.DefaultCellStyle, null), null);
            }
          }

          // Measure the column width
          TypeConverter tc = TypeDescriptor.GetConverter(cmb.ValueType);
          List<string> ss = new List<string>();
          if (tc != null) {
            foreach (object o in (Array)cmb.DataSource) {
              ss.Add((string)tc.ConvertTo(o, typeof(string)));
            }
          }
          else {
            foreach (object o in values) {
              ss.Add(o.ToString());
            }
          }
          Font f = dgv.DefaultCellStyle.Font;
          float width = 0;
          using (Graphics g = dgv.CreateGraphics()) {
            foreach (string s in ss) {
              width = Math.Max(width, g.MeasureString(s, f).Width);
            }
          }
          cmb.Width = Convert.ToInt32(width) + 4;

          // replace original column with new combo column
          dgv.Columns.RemoveAt(i);
          dgv.Columns.Insert(i, cmb);

        }
      }
    }


    private void btnClearFilter_Click(object sender, EventArgs e) {
      this._filterItem.FrmItems.Clear();
    }

    private void btnOK_Click(object sender, EventArgs e) {
      this._filterItem.Not=this.cbNot.Checked;
      this._filterItem.Items.Clear();
      foreach (DGCore.Filters.FilterLineSubitem o in this._filterItem.FrmItems) {
        this._filterItem.Items.Add(o);
      }
      this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e) {
      this.Close();
    }


  }
}