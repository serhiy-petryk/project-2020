using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfSpLib.Helpers;

namespace DGView.Controls.Printing
{
    internal class DGPrintingCanvas: Canvas, IDisposable
    {
        private TextBlock tb = new TextBlock{FontSize = 3.5};
        private Pen pen = new Pen(Brushes.Red, 0.1);
        private DGPrintContentGeneratorUsingDirectRendering _generator;
        private int _pageNo;

        public DGPrintingCanvas(DGPrintContentGeneratorUsingDirectRendering generator, int pageNo)
        {
            _generator = generator;
            _pageNo = pageNo;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_pageNo == 0)
                Debug.Print($"Canvas Render: {_pageNo}");
            if (_generator == null)
                return;

            var itemsInfo = _generator._itemsPerPage[_pageNo];
            var offset = _generator.GetHeaderHeight();
            for (var k1 = itemsInfo[0]; k1 < itemsInfo[0] + itemsInfo[1]; k1++)
            {
                var item = _generator._items[k1];
                PrintTextOfRow(dc, k1, offset);
                offset += _generator._rowHeights[k1];
            }

            /*for (var k1 = 0; k1 < 200; k1++)
            {
                dc.DrawLine(pen, new Point(0, (k1+1) * 4), new Point(ActualWidth, (k1 + 1) * 4));
                for (var k2 = 0; k2 < 30; k2++)
                {
                    var text = ControlHelper.GetFormattedText("Test text:" + k1, tb);
                    // dc.DrawRectangle(Brushes.Transparent, pen, new Rect(k2 * 20, k1 * 4, 20, 4));
                    dc.DrawText(text, new Point(k2 * 20, k1 * 4));
                }
            }

            for (var k2 = 0; k2 < 30; k2++)
            {
                dc.DrawLine(pen, new Point((k2+1) *20, 0), new Point((k2+1)*20, ActualHeight));
            }*/
        }

        private void PrintTextOfRow(DrawingContext dc, int rowNo, double offset)
        {
            // Row number
            var text = _generator._rowNumbers[rowNo].ToString("N0", LocalizationHelper.CurrentCulture);
            var formattedText = ControlHelper.GetFormattedText(text, tb);
            dc.DrawText(formattedText, new Point(1, offset));
        }

        public void Dispose()
        {
            _generator = null;
            Width = 0.0;
            Height = 0.0;
        }
    }
}
