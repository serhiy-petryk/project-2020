﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfSpLib.Helpers;

namespace DGView.Controls.Printing
{
    public class PrintingCanvas: Canvas
    {
        TextBlock tb = new TextBlock{FontSize = 3.5};
        Pen pen = new Pen(Brushes.Red, 0.1);
  
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            for (var k1 = 0; k1 < 200; k1++)
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
            }

        }
    }
}
