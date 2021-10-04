using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DGWnd.DGV
{
  public partial class DGVCube
  {
    private int? _visibleGroupNumberStart =>
      DataSource.Groups.Count == 0 ? (int?)null : (DataSource.ShowGroupsOfUpperLevels ? 1 : DataSource.ExpandedGroupLevel);
    private int? _visibleGroupNumberEnd => DataSource.Groups.Count == 0 ? (int?)null : Math.Min(DataSource.CurrentExpandedGroupLevel, DataSource.Groups.Count);

    protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
    {
      if (e.ColumnIndex < 0)
      {
        base.OnCellPainting(e);
        return;
      }

      var column = this.Columns[e.ColumnIndex];
      var displayIndex = (e.ColumnIndex < 0 ? -1 : Array.IndexOf(this._visibleColumns, column));
      if (e.RowIndex == -1 && _visibleGroupNumberStart.HasValue && displayIndex < (_visibleGroupNumberEnd - _visibleGroupNumberStart+1) )
      {
        // Group header cell == erase internal border
        e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);
        using (var transparentBorderPen = new Pen(column.HeaderCell.InheritedStyle.SelectionForeColor))
        {
          if (displayIndex > 0)
          {
            // Erase left border line
            var p1 = new Point(e.CellBounds.Left, e.CellBounds.Bottom - 2);
            var p2 = new Point(e.CellBounds.Left, e.CellBounds.Top + 2);
            e.Graphics.DrawLine(transparentBorderPen, p1, p2);
          }
          if (displayIndex < (_visibleGroupNumberEnd - _visibleGroupNumberStart))
          {
            // Erase right border line
            var p1 = new Point(e.CellBounds.Right - 1, e.CellBounds.Bottom - 2);
            var p2 = new Point(e.CellBounds.Right - 1, e.CellBounds.Top + 2);
            e.Graphics.DrawLine(transparentBorderPen, p1, p2);
          }
        }
        e.Handled = true;
      }
      else if (e.RowIndex>=0)
      {
        var cell = this[e.ColumnIndex, e.RowIndex];
        var isCustomCellPaint = false;
        if (cell is DataGridViewCheckBoxCell && cell.ReadOnly && (cell.Value == null || cell.Value == DBNull.Value))
        {
          // DataGridViewCheckBoxCell null values: show all exclude ContentForeground
          e.Paint(e.CellBounds, DataGridViewPaintParts.All & (~DataGridViewPaintParts.ContentForeground));
          isCustomCellPaint = true;
        }

        if (cell.Visible && DataSource.IsGroupMode)
        {
          var o = cell.OwningRow.DataBoundItem as DGCore.DGVList.IDGVList_GroupItem;
          var groupLevel = o?.Level ?? -1;

          if (!isCustomCellPaint)
            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

          if (groupLevel == -1)
          {// ============ item row
            var nextGroupLevel = DrawCell_GetRowGroupLevel(e.RowIndex + 1);
            // cell.ToolTipText = $"{displayIndex},{groupLevel},{nextGroupLevel},{_visibleGroupNumberStart}";
            if (_visibleGroupNumberStart.HasValue && displayIndex < (_visibleGroupNumberEnd - _visibleGroupNumberStart + 1))
            {
              DrawCell_RightBorder(e.Graphics, e.CellBounds, _groupBorderPen, 0); // Draw right border
              if (nextGroupLevel > (displayIndex + _visibleGroupNumberStart))
                DrawCell_BottomBorder(e.Graphics, e.CellBounds, _groupPens[displayIndex + _visibleGroupNumberStart.Value], 1); // Erase bottom border
              else if (nextGroupLevel < Int32.MaxValue)
                DrawCell_BottomBorder(e.Graphics, e.CellBounds, _groupBorderPen, 0); // Draw bottom border
            }
            else if (nextGroupLevel < Int32.MaxValue)
            { // Group end 
              DrawCell_BottomBorder(e.Graphics, e.CellBounds, _groupBorderPen, 0); // Draw bottom border
            }
          }
          else if (groupLevel == 0)
          {
            // ========== Total row
            if (this._IsGridVisible && displayIndex >= 0 && displayIndex < (this._visibleColumns.Length - 1))
              DrawCell_RightBorder(e.Graphics, e.CellBounds, _groupPens[0], 1); // Erase right border
            DrawCell_BottomBorder(e.Graphics, e.CellBounds, _groupBorderPen, 0); // Draw bottom border
          }
          else
          {
            // ========= Group row: (groupLevel > 0)
            if (displayIndex == groupLevel - _visibleGroupNumberStart) // Draw plus/minus image
              DrawCell_Image(e.Graphics, e.CellBounds, o.IsExpanded);

            // Bottom border
            if (displayIndex >= (_visibleGroupNumberEnd - _visibleGroupNumberStart + 1))
              DrawCell_BottomBorder(e.Graphics, e.CellBounds, _groupBorderPen, 0); // Set bottom border if bellow is the same group
            else
            {
              var nextGroupLevel = DrawCell_GetRowGroupLevel(e.RowIndex + 1);
              // cell.ToolTipText = $"{displayIndex},{groupLevel},{nextGroupLevel},{_visibleGroupNumberStart}";
              if (nextGroupLevel < groupLevel)
              {
                if ((displayIndex + _visibleGroupNumberStart) >= nextGroupLevel)
                  DrawCell_BottomBorder(e.Graphics, e.CellBounds, _groupBorderPen, 0); // Set bottom of older next group
                else
                  DrawCell_BottomBorder(e.Graphics, e.CellBounds, _groupPens[displayIndex + _visibleGroupNumberStart.Value], 1);// Erase
              }
              else if (nextGroupLevel == groupLevel)
              {
                if ((displayIndex + _visibleGroupNumberStart) >= groupLevel)
                  DrawCell_BottomBorder(e.Graphics, e.CellBounds, _groupBorderPen, 0);
                else
                  DrawCell_BottomBorder(e.Graphics, e.CellBounds, _groupPens[displayIndex + _visibleGroupNumberStart.Value], 1);
              }
              else
              {
                if ((displayIndex + _visibleGroupNumberStart) >= nextGroupLevel)
                  DrawCell_BottomBorder(e.Graphics, e.CellBounds, _groupBorderPen, 0);
                else
                  DrawCell_BottomBorder(e.Graphics, e.CellBounds, _groupPens[displayIndex + _visibleGroupNumberStart.Value], 1);
              }
            }

            // Right border
            if (displayIndex > (groupLevel - _visibleGroupNumberStart - 1) && displayIndex < (this._visibleColumns.Length - 1))
              DrawCell_RightBorder(e.Graphics, e.CellBounds, _groupPens[groupLevel], 1);// Erase right border
            else
              DrawCell_RightBorder(e.Graphics, e.CellBounds, _groupBorderPen, 0); // Draw right border of group
          }

          e.Handled = true;
        }

        if (isCustomCellPaint)
          e.Handled = true;
      }
      base.OnCellPainting(e);
    }

    public void DrawPrint_OwnerDraw(object sender, ThirdParty.SP_DGVCellDrawingEventArgs e)
    {
      int displayIndex = Array.IndexOf<DataGridViewColumn>(this._visibleColumns, e.Column);
      if (e.RowIndex >= 0)
      {// items
        // Draw background
        Color backColor = DrawCell_GetBackcolor(e.Column, e.BoundItem);
        using (Brush backBrush = new SolidBrush(backColor))
        {
          e.g.FillRectangle(backBrush, e.CellBounds);
        }
        e.PaintParts = e.PaintParts & (~DataGridViewPaintParts.Background);

        // Draw borders
        DGCore.DGVList.IDGVList_GroupItem o = e.BoundItem as DGCore.DGVList.IDGVList_GroupItem;
        var groupLevel = o?.Level ?? -1;

        if (groupLevel >= 0 && e.Column == this._groupItemCountColumn)
        {
          e.customFormattedValue = o.ItemCount.ToString("N0");
        }

        // Draw left border
        if (displayIndex > 0)
        {
          DrawPrint_RightBorder(e.g, new PointF(e.CellBounds.X, e.CellBounds.Top), new PointF(e.CellBounds.X, e.CellBounds.Bottom), this._visibleColumns[displayIndex - 1], groupLevel);
        }
        // Draw right border
        if (displayIndex < (this._visibleColumns.Length - 1))
        {
          DrawPrint_RightBorder(e.g, new PointF(e.CellBounds.Right, e.CellBounds.Top), new PointF(e.CellBounds.Right, e.CellBounds.Bottom), e.Column, groupLevel);
        }
        // Draw bottom border
        int k = DrawCell_GetRowGroupLevel(e.RowIndex + 1);
        if (k == Int32.MaxValue) k = -1;// reformat nextRowGroupLevel
        DrawPrint_BottomBorder(e.g, new PointF(e.CellBounds.X, e.CellBounds.Bottom), new PointF(e.CellBounds.Right, e.CellBounds.Bottom), e.Column, groupLevel, k);
        // Draw top border
        if (e.RowIndex > 0)
        {
          k = DrawCell_GetRowGroupLevel(e.RowIndex - 1);
          if (k == Int32.MaxValue) k = -1;// reformat previousRowGroupLevel
          DrawPrint_BottomBorder(e.g, new PointF(e.CellBounds.X, e.CellBounds.Top), new PointF(e.CellBounds.Right, e.CellBounds.Top),
            e.Column, k, groupLevel);
        }
        e.PaintParts = e.PaintParts & (~DataGridViewPaintParts.Border);
        if (groupLevel > 0 && displayIndex == (groupLevel - _visibleGroupNumberStart))
        {
          DrawPrint_Image(e.g, e.CellBounds, o.IsExpanded);
          e.PaintParts = DataGridViewPaintParts.None;
        }
      }//if (e.RowIndex >= 0)
      else if (displayIndex >= 0 && e.Column.Index >= 0 && _visibleGroupNumberStart.HasValue && displayIndex < (_visibleGroupNumberEnd - _visibleGroupNumberStart + 1))
      {
        // group column headers
        using (Brush backBrush = new SolidBrush(e.CellStyle.BackColor))
        {
          e.g.FillRectangle(backBrush, e.CellBounds);
        }
        var p1 = new PointF(e.CellBounds.Left, e.CellBounds.Top);
        var p2 = new PointF(e.CellBounds.Left, e.CellBounds.Bottom);
        var p3 = new PointF(e.CellBounds.Right, e.CellBounds.Top);
        var p4 = new PointF(e.CellBounds.Right, e.CellBounds.Bottom);
        if (displayIndex == 0)
        {
          e.g.DrawLines(this._gridPen, new PointF[] { p3, p1, p2 });
        }
        else if (displayIndex == (_visibleGroupNumberEnd - _visibleGroupNumberStart))
        {
          e.g.DrawLines(this._gridPen, new PointF[] { p1, p3, p4 });
        }
        else
        {
          e.g.DrawLine(this._gridPen, p1, p3);
          e.g.DrawLine(this._gridPen, p2, p4);
        }
        e.PaintParts = DataGridViewPaintParts.None;
      } //else if (displayIndex >= 0 && e.Column.Index >= 0 && displayIndex < (this._groups.Count))
    }

    private static void DrawCell_BottomBorder(Graphics g, Rectangle cellBounds, Pen pen, int erase)
    {
      var p1 = new Point(cellBounds.Left, cellBounds.Bottom - 1);
      var p2 = new Point(cellBounds.Right - 1 - erase, cellBounds.Bottom - 1);
      g.DrawLines(pen, new[] { p1, p2 });
    }
    private static void DrawCell_RightBorder(Graphics g, Rectangle cellBounds, Pen pen, int erase)
    {
      var p1 = new Point(cellBounds.Right - 1, cellBounds.Bottom - 1 - erase);
      var p2 = new Point(cellBounds.Right - 1, cellBounds.Top);
      g.DrawLines(pen, new[] { p1, p2 });
    }

    private void DrawPrint_BottomBorder(Graphics g, PointF p1, PointF p2, DataGridViewColumn column, int groupLevel, int nextRowGroupLevel)
    {
      int displayIndex = Array.IndexOf<DataGridViewColumn>(this._visibleColumns, column);
      if (displayIndex < (_visibleGroupNumberEnd - _visibleGroupNumberStart + 1))
      { // group header column
        if (nextRowGroupLevel >= 0 && (displayIndex + _visibleGroupNumberStart) >= nextRowGroupLevel)
          g.DrawLine(_groupBorderPen, p1, p2);
        return;
      }
      if (groupLevel >= 0 || nextRowGroupLevel >= 0)
      {
        g.DrawLine(_groupBorderPen, p1, p2);
        return;
      }

      if (this._IsGridVisible)
        g.DrawLine(this._gridPen, p1, p2);
    }

    private void DrawPrint_RightBorder(Graphics g, PointF p1, PointF p2, DataGridViewColumn column, int groupLevel)
    {
      int displayIndex = Array.IndexOf<DataGridViewColumn>(this._visibleColumns, column);
      if (displayIndex == this._visibleColumns.Length - 1)
      {// right border of DGV
        g.DrawLine(_gridPen, p1, p2);
        return;
      }
      if (displayIndex < (_visibleGroupNumberEnd - _visibleGroupNumberStart + 1))
      {// group header column
        if (groupLevel < 0)
        {// item row
          g.DrawLine(_groupBorderPen, p1, p2);
          return;
        }
        if ((displayIndex + _visibleGroupNumberStart) < groupLevel)
        {// not total row
          g.DrawLine(_groupBorderPen, p1, p2);
        }
        return;
      }
      if (groupLevel < 0 && this._IsGridVisible)
      {// item row
        g.DrawLine(this._gridPen, p1, p2);
        return;
      }
    }

    private Color DrawCell_GetBackcolor(DataGridViewColumn dgvColumn, object item)
    {
      if (item is DGCore.DGVList.IDGVList_GroupItem)
      {
        var groupItem = (DGCore.DGVList.IDGVList_GroupItem)item;
        var level = groupItem.Level;
        var displayIndex = Array.IndexOf<DataGridViewColumn>(this._visibleColumns, dgvColumn);
        if ((displayIndex + _visibleGroupNumberStart.Value - 1) < level)
          return _groupPens[displayIndex + _visibleGroupNumberStart.Value].Color;

        return _groupPens[level].Color;
      }

      return dgvColumn.InheritedStyle.BackColor;
    }

    private int DrawCell_GetRowGroupLevel(int rowIndex)
    {
      var data = (IBindingList)this.DataSource;

      if (data.Count <= rowIndex)
        return -1;// last line

      var groupItem = data[rowIndex] as DGCore.DGVList.IDGVList_GroupItem;
      return groupItem?.Level ?? int.MaxValue;
    }

  }
}
