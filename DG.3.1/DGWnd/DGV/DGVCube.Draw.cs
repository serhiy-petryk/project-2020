using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DGWnd.DGV {
  public partial class DGVCube {

    List<DataGridViewColumn> _groupColumns = new List<DataGridViewColumn>();
    DataGridViewColumn _groupItemCountColumn = null;

    static Pen[] _defaultGroupPens = new Pen[] { Pens.Gainsboro, new Pen(Color.FromArgb(255, 255,153,204)), 
      new Pen(Color.FromArgb(255, 255,204, 153)), new Pen(Color.FromArgb(255, 255,255,153)), 
      new Pen(Color.FromArgb(255, 204, 255,204)), new Pen(Color.FromArgb(255, 204,255,255)), 
      new Pen(Color.FromArgb(255, 153, 204, 255)), new Pen(Color.FromArgb(255,204, 153,  255))};
    //    static Pen[] _defaultGroupPens = new Pen[] { Pens.Bisque, Pens.Beige, Pens.LightBlue, Pens.Pink, Pens.Violet, 
  //    Pens.GreenYellow, Pens.Gold, Pens.LightCoral,Pens.Yellow, Pens.LightGreen};
    static Pen _groupBorderPen = Pens.Blue;
    Pen _gridPen;
    public List<Pen> _groupPens = new List<Pen>(new Pen[] { _defaultGroupPens[0] });
    DataGridViewColumn[] _visibleColumns;
    static Pen _treeCrossPen = Pens.DarkSlateGray;

    // bool _IsGroupMode => DataSource.Groups.Count > 0 || DataSource.ShowTotalRow;

    private bool _IsGridVisible {
      get { return CellBorderStyle != DataGridViewCellBorderStyle.None; }
      set { CellBorderStyle = (value ? DataGridViewCellBorderStyle.Single : DataGridViewCellBorderStyle.None); }
    }
    public bool _IsItemVisible(object dataItem) {
      if (DataSource.ShowGroupsOfUpperLevels || !DataSource.IsGroupMode)
        return true;
      DGCore.DGVList.IDGVList_GroupItem o = dataItem as DGCore.DGVList.IDGVList_GroupItem;
      if (o == null) return true;
      int groupLevel = o.Level;
      return (groupLevel >= DataSource.ExpandedGroupLevel);
    }

    protected override void OnRowPrePaint(DataGridViewRowPrePaintEventArgs e) {
      DataGridViewRow row = this.Rows[e.RowIndex];
      if (DataSource.IsGroupMode) {
          DGCore.DGVList.IDGVList_GroupItem o = row.DataBoundItem as DGCore.DGVList.IDGVList_GroupItem;
        if (o != null) {
          int groupLevel = o.Level;
          /*if (!this._showGroupsOfUpperLevels) {
            bool visibleFlag = (groupLevel >= this.ExpandedGroupLevel);
            if (visibleFlag != row.Visible) {
              CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[this.DataSource];
              currencyManager1.SuspendBinding();
              row.Visible = !row.Visible;
              currencyManager1.ResumeBinding();
            }
          }*/
//          if (groupLevel >0 && this._groupFonts[groupLevel]!=null ) {
          if (this._groupFonts[groupLevel] != row.DefaultCellStyle.Font ) {
            row.DefaultCellStyle.Font = this._groupFonts[groupLevel];
          }
          if (!row.ReadOnly) row.ReadOnly = true;
          // Adjust color for group row
          if (row.DefaultCellStyle.BackColor != this._groupPens[groupLevel].Color) row.DefaultCellStyle.BackColor = this._groupPens[groupLevel].Color;

          // Adjust color for cells of parent groups 
          if (_visibleGroupNumberStart.HasValue)
          {
            for (int i = _visibleGroupNumberStart.Value; i < groupLevel; i++)
            {
              var columnNo = _visibleColumns[i - _visibleGroupNumberStart.Value].Index;
              Color c = this._groupPens[i].Color;
              if (row.Cells[columnNo].Style.BackColor != c)
                row.Cells[columnNo].Style.BackColor = c;
            }
          }
          var itemCountCell = row.Cells[this._groupItemCountColumn.Index];
          UI.frmLog.AddEntry("OnRowPrePaint After");
          itemCountCell.Value = o.ItemCount;
        }
      }
      // Row numeration
      if (row.HeaderCell.Value == null || (row.HeaderCell.Value.ToString() != (e.RowIndex + 1).ToString())) {
        //? Row scrolling is very slowly ??? 
        //if (row.HeaderCell.Style.Alignment != DataGridViewContentAlignment.MiddleLeft)
         // row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
        if (row.HeaderCell.Style.Padding != Padding.Empty)
          row.HeaderCell.Style.Padding = Padding.Empty;
        var value = (e.RowIndex + 1).ToString();
        if (!object.Equals(row.HeaderCell.Value, value))
          row.HeaderCell.Value = value;
      }
      // Resize cells
      if (!(row.Tag is int && (int)row.Tag == this._layoutCount)) {
        switch (this._RowViewMode) {
          case DGCore.Common.Enums.DGRowViewMode.NotSet:
            if (row.Height != this.RowTemplate.Height) row.Height = this.RowTemplate.Height;
            break;
          case DGCore.Common.Enums.DGRowViewMode.OneRow:
            foreach (DataGridViewCell cell in row.Cells) {
              if (cell.Visible && cell.PreferredSize.Width > cell.OwningColumn.Width && cell.OwningColumn.Resizable != DataGridViewTriState.False) {
                cell.OwningColumn.Width = Math.Min(1000, cell.PreferredSize.Width);
              }
            }
            if (row.Height != this.RowTemplate.Height) row.Height = this.RowTemplate.Height;
            break;
          case DGCore.Common.Enums.DGRowViewMode.WordWrap:
            // flipped: because the start height of cell is one line text and cell height is changing to a few line text
            int newRowHeight = this.RowTemplate.Height;
            foreach (DataGridViewCell c in row.Cells) {
              if (c.Visible) {
                if (c.Size.Width < (Math.Min(1000, c.PreferredSize.Width) + 1)) {
                  c.OwningColumn.Width = Math.Min(1000, c.PreferredSize.Width) + 1;
                }
                if (c.OwningColumn.Resizable != DataGridViewTriState.False) {
                  if (c.FormattedValue is string) {
                    using (Graphics g = this.CreateGraphics()) {
                      int i1 = DataGridViewCell.MeasureTextHeight(g, c.FormattedValue.ToString(), this.Font, c.Size.Width - 1, TextFormatFlags.WordBreak);
                      if (newRowHeight < (i1 + 3)) newRowHeight = i1 + 3;
                    }
                  }
                  else if (c.FormattedValue is Bitmap) {
                    int i1 = ((Bitmap)c.FormattedValue).Height;
                    if (newRowHeight < (i1 + 3)) newRowHeight = i1 + 3;
                  }
                }
              }
            }
            if (row.Height != newRowHeight) row.Height = newRowHeight;
            break;
        }
        row.Tag = this._layoutCount;
      }

      base.OnRowPrePaint(e);
    }

    private static void DrawCell_Image(Graphics g, Rectangle cellBounds, bool isExpanded) {
      int baseSize = Convert.ToInt32(Convert.ToSingle(Math.Min(cellBounds.Height, cellBounds.Width)) * 0.5f);
      if (baseSize < 8) baseSize = Convert.ToInt32(Convert.ToSingle(baseSize) * 1.2f);// Increase base size when font is small
      if (baseSize % 2 != 0) baseSize++;
      Rectangle r = new Rectangle(cellBounds.X - 1 + (cellBounds.Width - baseSize) / 2, cellBounds.Y - 1 + (cellBounds.Height - baseSize) / 2, baseSize, baseSize);
      using (System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(r,
        Color.White, Color.LightSteelBlue, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal)) {
          g.FillRectangle(brush, r);
      }
      g.DrawRectangle(Pens.SlateGray, r);
      int delta = (baseSize - 1) / 4;
      g.DrawLine(_treeCrossPen, new Point(r.X + delta + 1, r.Y + baseSize / 2), new Point(r.X + baseSize - delta - 1, r.Y + baseSize / 2));
      if (!isExpanded) {
        g.DrawLine(_treeCrossPen, new Point(r.X + baseSize / 2, r.Y + delta + 1), new Point(r.X + baseSize / 2, r.Y + baseSize - delta - 1));
      }
    }
    private static void DrawPrint_Image(Graphics g, RectangleF cellBounds, bool isExpanded) {
      float baseSize = Math.Min(cellBounds.Height, cellBounds.Width) * 0.5f;
      if (baseSize < 8f) baseSize = baseSize * 1.2f;// Increase base size when font is small
      float x = cellBounds.X + (cellBounds.Width - baseSize) / 2f;
      float y = cellBounds.Y + (cellBounds.Height - baseSize) / 2f;
      using (System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
        new RectangleF(x, y, baseSize,baseSize),
        Color.White, Color.LightSteelBlue, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal)) {
        g.FillRectangle(brush, x, y, baseSize, baseSize);
      }
      g.DrawRectangle(Pens.SlateGray, x, y, baseSize, baseSize);
      float delta = baseSize / 4f;
      g.DrawLine(_treeCrossPen, new PointF(x + delta, y + baseSize / 2f), new PointF(x + baseSize - delta, y + baseSize / 2f));
      if (!isExpanded) {
        g.DrawLine(_treeCrossPen, new PointF(x + baseSize / 2f, y + delta), new PointF(x + baseSize / 2, y + baseSize - delta));
      }
    }

    private void _SetGroupColumns() {
      if (_groupItemCountColumn == null)
      {
        _groupItemCountColumn = new DataGridViewTextBoxColumn
        {
          Name = "#group_ItemCount",
          HeaderText = @"К-сть елементів",
          ReadOnly = true,
          Resizable = DataGridViewTriState.True,
          DefaultCellStyle = { NullValue = null, Alignment = DataGridViewContentAlignment.MiddleCenter, Format = "N0" },
          CellTemplate = { Style = {Alignment = DataGridViewContentAlignment.MiddleCenter}},
          ValueType = typeof(int),
          SortMode = DataGridViewColumnSortMode.NotSortable
        };
        Columns.Add(_groupItemCountColumn);
      }
      // Create new group columns if neccessary
      while (DataSource.Groups.Count > _groupColumns.Count) {
        var groupColumn = new DataGridViewTextBoxColumn
        {
          ReadOnly = true,
          HeaderText = (_groupColumns.Count + 1).ToString(),
          Resizable = DataGridViewTriState.False,
          SortMode = DataGridViewColumnSortMode.NotSortable,
          AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
          Name = "#group_" + this._groupColumns.Count,
          DefaultCellStyle = {NullValue = null}
        };
        Columns.Add(groupColumn);
        _groupColumns.Add(groupColumn);
        if (_groupColumns.Count >= _groupPens.Count) {
          // Need add new pen
          var penNo = (_groupColumns.Count - 1) % (_defaultGroupPens.Length - 1) + 1;
          _groupPens.Add(_defaultGroupPens[penNo]);
        }
      }
      // clear Column header backcolor
      foreach (DataGridViewColumn c in this.Columns) {
        c.HeaderCell.Style.BackColor = new Color();
      }

      // Set visible for group Columns and order for data fgroup columns
      for (int i = 0; i < this._groupColumns.Count; i++) {
        if (i < DataSource.Groups.Count) {
          this._groupColumns[i].Visible = DataSource.ShowGroupsOfUpperLevels || i >= (DataSource.ExpandedGroupLevel - 1);
/*          DataGridViewColumn dataColumn = this._groupColumns[i].Tag as DataGridViewColumn;
                    if (dataColumn == null || (dataColumn.DataPropertyName != this._groups[i].PropertyDescriptor.Name)) {
                      int k = Utils.DGV.GetColumnIndexByPropertyName(this, this._groups[i].PropertyDescriptor.Name);
                      if (k >= 0) {
                        dataColumn = this.Columns[k];
                        this._groupColumns[i].Tag = dataColumn;
                      }
                    }*/
          foreach (DataGridViewColumn c in this.Columns) {
            if (c.DataPropertyName == DataSource.Groups[i].PropertyDescriptor.Name) {
              if (c.HeaderCell.Style.BackColor != this._groupPens[i + 1].Color) c.HeaderCell.Style.BackColor = this._groupPens[i + 1].Color;
            }
          }
//          if (dataColumn != null && dataColumn.HeaderCell.Style.BackColor != this._groupPens[i + 1].Color) dataColumn.HeaderCell.Style.BackColor = this._groupPens[i + 1].Color;
          if (this._groupColumns[i].Width != (this.Font.Height + 7))
            this._groupColumns[i].Width = this.Font.Height + 7;// difference is from 7(Font=16pt) to 9(Font=9pt) pixels

          if (this._groupColumns[i].DefaultCellStyle.BackColor != this._groupPens[i + 1].Color)
            this._groupColumns[i].DefaultCellStyle.BackColor = this._groupPens[i + 1].Color;
        }
        else {// blank GroupColumn
          this._groupColumns[i].Visible = false;
          // this._groupColumns[i].Tag = null;// Clear dataColumn in tag of groupcolumn
        }
      }
    }

  }
}
