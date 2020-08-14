using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

//[module:CLSCompliant(true)]
namespace DGWnd.ThirdParty //AllocationRequest
{
  /// <summary>
  /// Class for the ownerdraw event. Provide the caller with the cell data, the current
  /// graphics context and the location in which to draw the cell.
  /// </summary>
  public class SP_DGVCellDrawingEventArgs : EventArgs {
    public Graphics g;
    public RectangleF CellBounds;
    public DataGridViewCellStyle CellStyle;
    public DataGridViewColumn Column;
    public object BoundItem;
    public int RowIndex;
    public string customFormattedValue;
//    public object FormattedValue;
    public DataGridViewPaintParts PaintParts;
//    public Boolean Handled;

    public SP_DGVCellDrawingEventArgs(Graphics g, RectangleF cellBounds, DataGridViewCellStyle style, DataGridViewColumn column, object boundItem, int rowIndex)
      : base() {
      this.g = g;
      CellBounds = cellBounds;
      CellStyle = style;
      this.Column = column;
      this.BoundItem = boundItem;
      this.RowIndex = rowIndex;
  //    this.FormattedValue = formattedValue;
      PaintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.Border;
//      Handled = false;
    }
  }

  public class DGVCellDrawingEventArgs : EventArgs {
    public Graphics g;
    public RectangleF DrawingBounds;
    public DataGridViewCellStyle CellStyle;
    public int row;
    public int column;
    public Boolean Handled;

    public DGVCellDrawingEventArgs(Graphics g, RectangleF bounds, DataGridViewCellStyle style,
        int row, int column)
      : base() {
      this.g = g;
      DrawingBounds = bounds;
      CellStyle = style;
      this.row = row;
      this.column = column;
      Handled = false;
    }
  }

  /// <summary>
  /// Delegate for ownerdraw cells - allow the caller to provide drawing for the cell
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public delegate void CellOwnerDrawEventHandler(object sender, DGVCellDrawingEventArgs e);
  public delegate void SP_CellOwnerDrawEventHandler(object sender, SP_DGVCellDrawingEventArgs e);

  /// <summary>
  /// Hold Extension methods
  /// </summary>
  public static class Extensions {
    /// <summary>
    /// Extension method to print all the "ImbeddedImages" in a provided list
    /// </summary>
    /// <typeparam name="?"></typeparam>
    /// <param name="list"></param>
    /// <param name="g"></param>
    /// <param name="pagewidth"></param>
    /// <param name="pageheight"></param>
    /// <param name="margins"></param>
    public static void DrawImbeddedImage<T>(IEnumerable<T> list, Graphics g, int pagewidth, int pageheight, Margins margins, float _paintMarginX, float _paintMarginY) {
      foreach (T t in list) {
        if (t is DGVPrinter.ImbeddedImage) {
          DGVPrinter.ImbeddedImage ii = (DGVPrinter.ImbeddedImage)Convert.ChangeType(t, typeof(DGVPrinter.ImbeddedImage));
          // Fix - DrawImageUnscaled was actually scaling the images!!?! Oh well...
          //g.DrawImageUnscaled(ii.theImage, ii.upperleft(pagewidth, pageheight, margins));
          g.DrawImage(ii.theImage,
              new Rectangle(ii.upperleft(pagewidth, pageheight, margins, _paintMarginX, _paintMarginY),
              new Size(ii.theImage.Width, ii.theImage.Height)));
        }
      }
    }

  }
}
