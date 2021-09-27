using System;
using System.Drawing;
using System.Windows.Forms;

namespace DGWnd.DGV {

  public partial class DGVCube {

    const int delta = 2;
    static System.Reflection.FieldInfo fiDataGridViewOper = null;
    int scrollMode = 0;// 0-no scroll, 1-scroll into left, -1-scroll into right

    Rectangle GetColumnHeaderRectangle(DataGridView dgv) {
      DataGridViewColumn c1 = this.Columns.GetFirstColumn(DataGridViewElementStates.Displayed, DataGridViewElementStates.Frozen);
      DataGridViewColumn c2 = this.Columns.GetLastColumn(DataGridViewElementStates.Displayed, DataGridViewElementStates.Frozen);
      Rectangle r1 = this.GetCellDisplayRectangle(c1.Index, -1, false);
      Rectangle r2 = this.GetCellDisplayRectangle(c2.Index, -1, false);
      r1.Size = new System.Drawing.Size(r2.X - r1.X + r2.Width, r1.Height);
      return new Rectangle(r1.X, r1.Y, r2.X - r1.X + r2.Width, r1.Height);
    }
    private bool IsRelocating() {
      if (fiDataGridViewOper == null) {
        fiDataGridViewOper = typeof(DataGridView).GetField("_dataGridViewOper", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.NonPublic);
      }
      System.Collections.Specialized.BitVector32 dataGridViewOper = (System.Collections.Specialized.BitVector32)fiDataGridViewOper.GetValue(this);
      return dataGridViewOper[0x00000020];
    }

    protected override void OnMouseMove(MouseEventArgs e) {
      // see BeginColumnRelocation method for DGV
      //http://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/DataGridViewMethods.cs,67690b1cc575a76e,references
      if (e.Button == System.Windows.Forms.MouseButtons.Left && (this.PreferredSize.Width > this.DisplayRectangle.Width) && IsRelocating()) {
        DataGridView.HitTestInfo info = this.HitTest(e.X, e.Y);
        if (info.Type == DataGridViewHitTestType.ColumnHeader || info.Type == DataGridViewHitTestType.VerticalScrollBar) {
          if (this.HorizontalScrollingOffset > 0) {
            int divider = this.RowHeadersVisible ? this.RowHeadersWidth : 0;
            DataGridViewColumn c = this.Columns.GetLastColumn(DataGridViewElementStates.Frozen, DataGridViewElementStates.None);
            if (c != null) {
              Rectangle rr = this.GetCellDisplayRectangle(c.Index, -1, false);
              divider = rr.X + rr.Width;
            }
            if (e.X < (divider + delta)) {
              if (scrollMode == 1) return;
              Rectangle r = GetColumnHeaderRectangle(this);
              scrollMode = 1;
              int cnt = 0;
              while (this.HorizontalScrollingOffset > 0 && scrollMode == 1) {
                this.HorizontalScrollingOffset = Math.Max(0, this.HorizontalScrollingOffset - 10);
                this.Invalidate(r);
                if (cnt % 5 == 0) Application.DoEvents();
              }
              scrollMode = 0;
              return;
            }
          }
          if ((this.HorizontalScrollBar.Value + this.HorizontalScrollBar.LargeChange) < this.HorizontalScrollBar.Maximum) {
            int divider = this.DisplayRectangle.Width;
            if (e.X > (divider - delta - 2)) {
              if (scrollMode == -1) return;
              Rectangle r = GetColumnHeaderRectangle(this);
              scrollMode = -1;
              int cnt = 0;
              while ((this.HorizontalScrollBar.Value + this.HorizontalScrollBar.LargeChange) < this.HorizontalScrollBar.Maximum && scrollMode == -1) {
                this.HorizontalScrollingOffset += 10;
                this.Invalidate(r);
                if (cnt % 5 == 0) Application.DoEvents();
              }
              scrollMode = 0;
              return;
            }
          }
        }
      }
      if (scrollMode != 0) {
        scrollMode = 0;
      }
      base.OnMouseMove(e);
    }
  }
}
