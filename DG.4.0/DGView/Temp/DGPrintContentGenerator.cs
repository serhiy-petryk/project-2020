using System;
using System.Collections;
using System.Globalization;
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
                Background = Brushes.LightYellow,
                Margin = new Thickness(_pageMargins.Left, _pageMargins.Top, 0, 0),
                Width = _pageSize.Width - _pageMargins.Left - _pageMargins.Right
            };

            // Print header
            var headerRow = new Grid {HorizontalAlignment = HorizontalAlignment.Stretch};
            stackPanel.Children.Add(headerRow);
            var leftHeader = new TextBlock { Text = _title, FontSize = 12.0 / 72.0 * 96.0, FontWeight = FontWeights.SemiBold };
            headerRow.Children.Add(leftHeader);

            var rightHeader = new TextBlock
            {
                Text = $"{DateTime.Now:G} / Сторінка {pageNo} з {pages}", FontSize = 8.0 / 72.0 * 96.0,
                HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center
            };
            headerRow.Children.Add(rightHeader);

            // Print subHeader
            if (!string.IsNullOrEmpty(_subTitle))
            {
                var subHeader = new TextBlock
                    { Text = _subTitle, HorizontalAlignment = HorizontalAlignment.Left, FontSize = 8.0 / 72.0 * 96.0 };
                stackPanel.Children.Add(subHeader);
                var subHeader2 = new TextBlock
                    { Text = _subTitle, HorizontalAlignment = HorizontalAlignment.Left};
                stackPanel.Children.Add(subHeader2);
            }

            _rows = _rowCount;
            return stackPanel;
        }

    }
    }
