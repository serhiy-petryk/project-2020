using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WpfSpLib.Helpers;

namespace DGView.Temp
{
    public class DGPrintContentGenerator : IPrintContentGenerator
    {
        public bool StopGeneration { get; set; }

        private CultureInfo _currentCulture => LocalizationHelper.CurrentCulture;

        private IList _items;
        private DataGridColumn[] _columns;
        private string _title;
        private string _subTitle;

        private Size _pageSize;
        private Thickness _pageMargins;
        private int _rows;
        private readonly SolidColorBrush _headerBackground = new SolidColorBrush(Color.FromRgb(246,247,248));

        // private int _columnCount;
        private int _rowCount=1;
        public DGPrintContentGenerator(IList items, DataGridColumn[] columns, string title, string[] subHeaders)
        {
            _items = items;
            _columns = columns;
            _title = title;
            if (subHeaders != null)
                _subTitle = string.Join(Environment.NewLine, subHeaders);
        }

        public void GenerateContent(FixedDocument document, Thickness margins)
        {
            _pageSize = document.DocumentPaginator.PageSize;
            _pageMargins = margins;

            _rows = 0;
            var pageNo = 0;
            var pages = 1;
            while (_rows < _rowCount && !StopGeneration)
            {
                var fixedPage = new FixedPage();
                fixedPage.Children.Add(GetPageContent(pageNo++, pages));
                var pageContent = new PageContent { Child = fixedPage };
                document.Pages.Add(pageContent);
            }
        }

        private FrameworkElement GetPageContent(int pageNo, int pages)
        {
            var stackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(_pageMargins.Left, _pageMargins.Top, 0, 0),
                Width = _pageSize.Width - _pageMargins.Left - _pageMargins.Right
            };

            var offset = 0.0;

            // Print header
            var headerRow = new Grid {HorizontalAlignment = HorizontalAlignment.Stretch};
            stackPanel.Children.Add(headerRow);
            var leftHeader = new TextBlock { Text = _title, FontSize = 16, FontWeight = FontWeights.SemiBold };
            headerRow.Children.Add(leftHeader);

            var rightHeader = new TextBlock
            {
                Text = $"{DateTime.Now:G} / Сторінка {pageNo} з {pages}", FontSize = 11,
                HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center
            };
            headerRow.Children.Add(rightHeader);
            offset += ControlHelper.MeasureString(leftHeader.Text, leftHeader, TextFormattingMode.Ideal).Height;

            // Print subHeader
            if (!string.IsNullOrEmpty(_subTitle))
            {
                var subHeader = new TextBlock
                    { Text = _subTitle, HorizontalAlignment = HorizontalAlignment.Left, FontSize = 11 };
                stackPanel.Children.Add(subHeader);
                var subTitleSize = ControlHelper.MeasureString(subHeader.Text, subHeader, TextFormattingMode.Ideal);
                offset += ControlHelper.MeasureString(subHeader.Text, subHeader, TextFormattingMode.Ideal).Height;
            }

            var dgArea = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0)
            };
            var dgWidth = _columns.Sum(c => c.ActualWidth) + 1;
            var availableHeight = _pageSize.Height - _pageMargins.Top - _pageMargins.Bottom;
            if (dgWidth > stackPanel.Width)
            {
                var scale = stackPanel.Width / dgWidth;
                dgArea.LayoutTransform = new ScaleTransform(scale, scale);
                availableHeight /= scale;
            }

            var dgHeaders = new StackPanel(){Orientation = Orientation.Horizontal};
            for (var k=0; k<_columns.Length ;k++)
            {
                var c = _columns[k];
                var rightBorder = k == _columns.Length - 1 ? 1.0 : 0.0;
                var borderThickness = new Thickness(1, 1, rightBorder, 0);
                var border = new Border
                {
                    BorderThickness = borderThickness, BorderBrush = Brushes.Gray, Width = c.ActualWidth,
                    Background = _headerBackground
                };
                var dgHeader = new TextBlock
                {
                    Text = (c.Header ?? "").ToString(), Width = c.ActualWidth-1, TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Center, Padding = new Thickness(3)
                };
                border.Child = dgHeader;
                dgHeaders.Children.Add(border);
            }

            dgArea.Children.Add(dgHeaders);
            stackPanel.Children.Add(dgArea);
            _rows = _rowCount;
            return stackPanel;
        }

    }
}
