using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DGWnd.Utils;

namespace DGWnd.UserControls {
  public partial class UC_Filter : UserControl {

    public DGCore.Filters.FilterList FilterList;
    ICollection _dataSource;
    Label lblPseudoToolTip = new Label();
    System.Drawing.Font fontBold=null;
    System.Drawing.Font fontRegular=null;
    string displayNameFilter = "";

    public UC_Filter() {
      InitializeComponent();
      this.bsFilter.DataSource = typeof(DGCore.Filters.FilterList);
      //Utils.DGVClipboard.Clipboard_Attach(this.dgvList);
      Utils.DGVClipboard.Clipboard_Attach(this.dgvFilter);

      // Set transparent image as default value into Infocolumn of DGVFilter
      DataGridViewImageColumn colImage = (DataGridViewImageColumn)dgvFilter.Columns["colInfo"];
      colImage.DefaultCellStyle.NullValue = DGWnd.Properties.Resources.Transparent;
// does not work: to do later (llok at group column)       colImage.DefaultCellStyle.NullValue = null;

      // Tooltip label
      lblPseudoToolTip.BackColor = SystemColors.Info;
      lblPseudoToolTip.ForeColor = SystemColors.InfoText;
      lblPseudoToolTip.Visible = false;
      lblPseudoToolTip.AutoSize = true;
      lblPseudoToolTip.Padding = new Padding(3);
      lblPseudoToolTip.BorderStyle = BorderStyle.FixedSingle;
      lblPseudoToolTip.Font = new Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.Controls.Add(lblPseudoToolTip);
      lblPseudoToolTip.BringToFront();
    }

    public override void Refresh() {
      base.Refresh();
      int i1 = this.txtFilterText.Width;
      this.txtFilterText.MaximumSize = new Size(this.txtFilterText.Parent.ClientSize.Width, 0);
      this.txtFilterText.Text = this.FilterList.StringPresentation;
      this.txtFilterText.Height = Convert.ToInt32(Convert.ToDouble(this.txtFilterText.PreferredSize.Height) * 0.88);
      int i2 = this.txtFilterText.Width;

      CurrencyManager cm = (CurrencyManager)BindingContext[dgvFilter.DataSource];
      cm.SuspendBinding();
      foreach (DataGridViewRow r in dgvFilter.Rows) {
        bool displayRow;
        if (String.IsNullOrEmpty(displayNameFilter)) {
          displayRow = true;
          dgvFilter.Columns["colFilterDisplayName"].HeaderCell.Value="Назва колонки (для установки фільтра рядків натисніть тут)";
        }
        else {
          dgvFilter.Columns["colFilterDisplayName"].HeaderCell.Value = "Назва колонки (для зміни фільтра рядків натисніть тут)";
          displayRow = r.Cells["colFilterDisplayName"].FormattedValue.ToString().ToUpper().Contains(displayNameFilter.ToUpper());
        }
        if (r.Visible != displayRow) r.Visible = displayRow;
      }
      cm.ResumeBinding();
      //this.lblFilterText.Text = this._filterObject.GetStringPresentation();
    }

    public void Bind(DGCore.Filters.FilterList filterList, ICollection dataSource) {
      this.FilterList = filterList;
      this._dataSource = dataSource;// items list for comboboxes

      if (!DGCore.Utils.Tips.IsDesignMode) {
          DGVUtils.CreateComboColumnsForEnumerations(this.dgvFilter);
      }
      this.bsFilter.DataSource = FilterList;

      /*            this.dgvFilter.DataSource = _filterObject;
      foreach (DataGridViewColumn c in this.dgvFilter.Columns) {
        c.SortMode = DataGridViewColumnSortMode.Automatic;
      }*/
      this.dgvFilter.Columns[DGVUtils.GetColumnIndexByPropertyName(this.dgvFilter, "IgnoreCase")].Visible = this.FilterList.IgnoreCaseSupport;
      //this.dgvFilter.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

      // Set left datagrid width
      //SetWidthForDGVFilter();
      // Hide dgvList when no rows in dgvFilter
      //if (this.bsFilter.List.Count == 0) this.dgvList.Visible = false;
    }

    /**public object GetSettingInfo() {
      return this.FilterList.GetSettingInfo();
    }
    public void SetSettingInfo(object settingInfo) {
      if (settingInfo is object[]) {
        this.FilterList.SetSettings((object[])settingInfo);
      }
    }*/

    // =========  DataGridView service  ================== 
    private void dgv_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
      DataGridView dgv = (DataGridView)sender;
      DataGridViewRow r = dgv.Rows[e.RowIndex];

      if (dgv == this.dgvFilter) {
          DGCore.Filters.FilterLineBase fline = (DGCore.Filters.FilterLineBase)r.DataBoundItem;
        int i = DGVUtils.GetColumnIndexByPropertyName(this.dgvFilter, "IgnoreCase");
        // change cell type to check box for string properties
        if (fline.PropertyType == typeof(string) && !(r.Cells[i] is DataGridViewCheckBoxCell)) {
          DataGridViewCheckBoxCell cell = new DataGridViewCheckBoxCell(false);
          r.Cells[i] = cell;
        }
        // Set readOnly property for IgnoreCase cell
        bool readOnly = r.Cells[i] is DataGridViewTextBoxCell;
        if (r.Cells[i].ReadOnly != readOnly) r.Cells[i].ReadOnly = readOnly;
        // Set image for info column
        DataGridViewImageCell infoCell = (DataGridViewImageCell)r.Cells["colInfo"];
        DataGridViewTextBoxCell filterTextCell = (DataGridViewTextBoxCell)r.Cells["StringPresentation"];
        if (fontRegular == null) {
          fontRegular = new Font(filterTextCell.InheritedStyle.Font, FontStyle.Regular);
//          fontRegular.Size = 8.25;
          fontBold = new Font(filterTextCell.InheritedStyle.Font, FontStyle.Bold);
  //        fontBold.Size = 9.25;
        }

        string filterText=fline.StringPresentation;
        if (String.IsNullOrEmpty(fline.Description)) {
          if (infoCell.Value != infoCell.InheritedStyle.NullValue) infoCell.Value = infoCell.InheritedStyle.NullValue;
          infoCell.ToolTipText = "";
        }
        else {
          infoCell.ToolTipText = fline.Description;
        }
        // Set filterText/info cell value
        if (String.IsNullOrEmpty(filterText)) {
          filterTextCell.Style.Font = fontRegular;
          filterTextCell.Style.ForeColor = Color.Black;
          filterTextCell.Value = fline.Description;
        }
        else {
          filterTextCell.Style.Font = fontBold;
          filterTextCell.Style.ForeColor = Color.Blue;
          filterTextCell.Value = filterText;
        }
      }
      // Set backColor for cells (editable: LightCyan; readonly: WhiteSmoke);
      foreach (DataGridViewCell cell in r.Cells) {
        if (cell.ReadOnly) cell.Style.BackColor = Color.WhiteSmoke;
        else cell.Style.BackColor = Color.LightCyan;
      }

    }

    private void dgvFilter_RowEnter(object sender, DataGridViewCellEventArgs e) {
      // Refresh item list for combobox
      List<object> xx = new List<object>();
      DataGridViewRow row = dgvFilter.Rows[e.RowIndex];
      DGCore.Filters.FilterLineBase o = (DGCore.Filters.FilterLineBase)row.DataBoundItem;
      if (this._dataSource != null) {
        if (o is DGCore.Filters.FilterLine_Item && this.dgvFilter.Visible) {
                    // Get sorted distinct values from object list
                    DGCore.Filters.FilterLine_Item item = (DGCore.Filters.FilterLine_Item)o;
          IEnumerable<object> e0 = Enumerable.Cast<object>(this._dataSource);
          IEnumerable<object> e1 = Enumerable.Select<object, object>(e0, delegate(object o1) { return item._pd.GetValue(o1); });
          IEnumerable<object> e2 = Enumerable.Distinct<object>(e1);
          xx = Enumerable.ToList<object>(e2);
          for (int i = 0; i < xx.Count; i++) {
            if (xx[i] == null) xx.RemoveAt(i--);
          }
          // Add items from filter item object
          foreach (DGCore.Filters.FilterLineSubitem x1 in o.Items) {
            if (x1.Value1 != null && !xx.Contains(x1.Value1)) xx.Add(x1.Value1);
            if (x1.Value2 != null && !xx.Contains(x1.Value2)) xx.Add(x1.Value2);
          }
          xx.Sort();
        }
      }
      // Refresh filter string presentation
      this.txtFilterText.Text = this.FilterList.StringPresentation;
      //this.lblFilterText.Text = this._filterObject.GetStringPresentation();
    }

    private void dgvList_DataError(object sender, DataGridViewDataErrorEventArgs e) {

    }

    private void dgvList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
      // DGVComboBoxCell is going to edit mode by F2: do not clear the cell context when date has not null miliseconds
      if (e.Value is DateTime && String.IsNullOrEmpty(e.CellStyle.Format))
        e.CellStyle.Format = (((DateTime)e.Value).TimeOfDay == TimeSpan.Zero ? "d" : "g");
    }

    private void dgvFilter_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
      // Show tooltip
      if (e.ColumnIndex == dgvFilter.Columns["colInfo"].Index && e.RowIndex >= 0) {
        string toolTipText = dgvFilter.Rows[e.RowIndex].Cells["colInfo"].ToolTipText;
        lblPseudoToolTip.Text = toolTipText;
        lblPseudoToolTip.Visible = !String.IsNullOrEmpty(lblPseudoToolTip.Text);
        lblPseudoToolTip.MaximumSize = new Size(dgvFilter.Width, 0);
        Point p1 = this.dgvFilter.Location;
        Rectangle r1 = this.dgvFilter.GetRowDisplayRectangle(e.RowIndex, true);
        lblPseudoToolTip.Location = new Point(p1.X + r1.X + 2, p1.Y + r1.Bottom + 6);
      }
    }

    private void dgvFilter_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
      // close tooltip:
      lblPseudoToolTip.Visible = false;
    }

    private void dgvFilter_CellClick(object sender, DataGridViewCellEventArgs e) {
      if (e.RowIndex < 0) return;
      DataGridViewColumn c = dgvFilter.Columns[e.ColumnIndex];
      if (c == btnEdit) {
        using (Form frm = new UI.frmSetFilter(this._dataSource, (DGCore.Filters.FilterLineBase)this.dgvFilter.CurrentRow.DataBoundItem)) {
          frm.StartPosition = FormStartPosition.CenterScreen;
          DialogResult x = frm.ShowDialog();
          if (x == DialogResult.OK) {
            this.Refresh();
          }
        }
      }
    }

    private void dgvFilter_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
      if (e.ColumnIndex >= 0) {
        DataGridViewColumn c =  dgvFilter.Columns[e.ColumnIndex];
        if (c.DataPropertyName == "DisplayName") {
          Point p = GetControlPosition(this.ParentForm, 370, 150);
          displayNameFilter = Microsoft.VisualBasic.Interaction.InputBox("Уведіть текст для фільтрації списка колонок", "Текст фільтра для списка колонок", displayNameFilter, p.X, p.Y);
          this.Refresh();
        }
      }
    }

    public static Point GetControlPosition(Form mainForm, int approxInputBoxWidth, int approxInputBoxHeight) {
      int left = mainForm.Left + (mainForm.Width / 2) - (approxInputBoxWidth / 2);
      left = left < 0 ? 0 : left;
      int top = mainForm.Top + (mainForm.Height / 2) - (approxInputBoxHeight / 2);
      top = top < 0 ? 0 : top;
      return new Point(left, top);
    }


  }
}
