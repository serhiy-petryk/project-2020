using System;
using System.Windows.Forms;

namespace DGWnd.DGV
{
  public partial class DGVCube
  {
    public event EventHandler _OnCellViewModeChanged;

    private Common.Enums.DGCellViewMode _cellViewMode = Common.Enums.DGCellViewMode.OneRow;
    public Common.Enums.DGCellViewMode _CellViewMode
    {
      get { return this._cellViewMode; }
      set
      {
        if (value != _cellViewMode)
        {
          _cellViewMode = value;
          _OnCellViewModeChanged?.Invoke(this, new EventArgs());
          var thisViewMode = this._CellViewMode == Common.Enums.DGCellViewMode.WordWrap ? DataGridViewTriState.True : DataGridViewTriState.False;

          foreach (DataGridViewColumn c in this.Columns)
            if (c.DefaultCellStyle.WrapMode != thisViewMode)
              c.DefaultCellStyle.WrapMode = thisViewMode;

          ResizeColumnWidth();
        }
      }
    }


  }
}
