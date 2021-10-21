using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DGView.Temp
{
    public interface IPrintContentGenerator
    {
        bool StopGeneration { get; set; }
        void GenerateContent(FixedDocument document, Size pageSize, Thickness margins);
    }

    public class PrintContentGeneratorSample: IPrintContentGenerator
    {
        const double columnWidth = 70.0;
        const double rowHeight = 30.0;

        private int _rows;
        private Size _pageSize;
        private Thickness _pageMargins;
        private double _printingHeightNotScaled;
        private double _scale;


        private int _columnCount;
        private int _rowCount;
        public PrintContentGeneratorSample(int columnCount = 20, int rowCount = 500)
        {
            _columnCount = columnCount;
            _rowCount = rowCount;
        }

        public bool StopGeneration { get; set; }

        public void GenerateContent(FixedDocument fixedDoc, Size pageSize, Thickness margins)
        {
            StopGeneration = false;
            _rows = 0;
            _pageSize = pageSize;
            _pageMargins = margins;
            fixedDoc.DocumentPaginator.PageSize = _pageSize;

            var desiredWidth = columnWidth * _columnCount;
            var printingWidth = _pageSize.Width - _pageMargins.Left - _pageMargins.Right;
            _scale = desiredWidth > printingWidth ? printingWidth / desiredWidth : 1.0;
            _printingHeightNotScaled = (_pageSize.Height - _pageMargins.Top - _pageMargins.Bottom) / _scale;

            var itemsInPage = Math.Max(1, Math.Floor((_printingHeightNotScaled - rowHeight)/ rowHeight));
            var pages = Convert.ToInt32( Math.Ceiling(_rowCount / itemsInPage));

            var pageNo = 0;
            while (_rows < _rowCount && !StopGeneration)
            {
                var fixedPage = new FixedPage();
                fixedPage.Children.Add(GetPageContent(pageNo++, pages));
                var pageContent = new PageContent { Child = fixedPage };
                fixedDoc.Pages.Add(pageContent);
            }
        }

        private FrameworkElement GetPageContent(int pageNo, int pages)
        {
            var stackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Background = Brushes.LightYellow,
                Margin = new Thickness(_pageMargins.Left, _pageMargins.Top, 0, 0),
            };
            if (_scale != 1.0)
                stackPanel.LayoutTransform = new ScaleTransform(_scale, _scale);

            var textBlock = new TextBlock(){Text = $"Page: {pageNo+1} / {pages}", Height = rowHeight, HorizontalAlignment = HorizontalAlignment.Right};
            stackPanel.Children.Add(textBlock);

            var currentHeight = rowHeight;
            var rowItemCount = 0;
            while (rowItemCount == 0 || (currentHeight + rowHeight) < _printingHeightNotScaled)
            {
                Helpers.DoEventsHelper.DoEvents();
                stackPanel.Children.Add(GetRow(rowItemCount));
                currentHeight += rowHeight;
                rowItemCount++;
                _rows++;
                if (_rows >= _rowCount)
                    break;
            }
            return stackPanel;
        }

        private FrameworkElement GetRow(int rowItemCount)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal };
            for (var k2 = 0; k2 < _columnCount; k2++)
            {
                /*var s1 = 0;
                for (var k3 = 0; k3 < 200000; k3++)
                {
                    s1 = s1 + k3;
                }*/

                var border = new Border
                {
                    BorderBrush = Brushes.Black,
                    Background = Brushes.Aqua,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Width = columnWidth,
                    Height = rowHeight
                };
                var text = k2 == 0 ? rowItemCount.ToString() : $"{_rows}, {k2}";
                var cell = new TextBlock
                {
                    Text = text,
                    Background = Brushes.Aqua,
                    Padding = new Thickness(4),
                    VerticalAlignment = VerticalAlignment.Center
                };
                border.Child = cell;
                row.Children.Add(border);
            }

            return row;
        }
    }
}
