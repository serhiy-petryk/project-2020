using System;
using System.Windows.Forms;

namespace DGWnd.DGV
{
  public partial class DGVCube
  {
    public event EventHandler _OnRowViewModeChanged;

    private DGCore.Common.Enums.DGRowViewMode _rowViewMode = DGCore.Common.Enums.DGRowViewMode.OneRow;
    public DGCore.Common.Enums.DGRowViewMode _RowViewMode
    {
      get { return this._rowViewMode; }
      set
      {
        if (value != _rowViewMode)
        {
          _rowViewMode = value;
          _OnRowViewModeChanged?.Invoke(this, new EventArgs());
          var thisViewMode = this._RowViewMode == DGCore.Common.Enums.DGRowViewMode.WordWrap ? DataGridViewTriState.True : DataGridViewTriState.False;

          foreach (DataGridViewColumn c in this.Columns)
            if (c.DefaultCellStyle.WrapMode != thisViewMode)
              c.DefaultCellStyle.WrapMode = thisViewMode;

          ResizeColumnWidth();
        }
      }
    }


  }
}
