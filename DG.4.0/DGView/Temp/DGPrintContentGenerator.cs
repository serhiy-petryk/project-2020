using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using DGView.Helpers;
using DGView.ViewModels;
using WpfSpLib.Helpers;

namespace DGView.Temp
{
    public class DGPrintContentGenerator : IPrintContentGenerator
    {
        public bool StopGeneration { get; set; }

        private CultureInfo _currentCulture => LocalizationHelper.CurrentCulture;

        private DGViewModel _viewModel;

        private IList _items;
        private DataGridColumn[] _columns;

        private Size _pageSize;
        private Thickness _pageMargins;
        private int _rows;
        private readonly SolidColorBrush _headerBackground = new SolidColorBrush(Color.FromRgb(240, 241, 242));

        // private int _columnCount;
        private int _rowCount = 1;
        public DGPrintContentGenerator(DGViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void GenerateContent(FixedDocument document, Thickness margins)
        {
            DataGridHelper.GetSelectedArea(_viewModel.DGControl, out var items, out var columns);
            _items = items;
            _columns = columns;

            _pageSize = document.DocumentPaginator.PageSize;
            _pageMargins = margins;

            _rows = 0;
            var pageNo = 0;
            var pages = 1;
            while (_rows < _rowCount && !StopGeneration)
            {
                var fixedPage = new FixedPage();
                fixedPage.Children.Add(GetPageContent(pageNo++, pages, document));
                var pageContent = new PageContent { Child = fixedPage };
                document.Pages.Add(pageContent);
            }
        }

        private FrameworkElement GetPageContent(int pageNo, int pages, FixedDocument document)
        {
            var stackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(_pageMargins.Left, _pageMargins.Top, 0, 0),
                Width = _pageSize.Width - _pageMargins.Left - _pageMargins.Right
            };

            var offset = 0.0;

            // Print header
            var headerRow = new Grid { HorizontalAlignment = HorizontalAlignment.Stretch };
            stackPanel.Children.Add(headerRow);
            var leftHeader = new TextBlock { Text = _viewModel.Title, FontSize = 16, FontWeight = FontWeights.SemiBold };
            headerRow.Children.Add(leftHeader);

            var rightHeader = new TextBlock
            {
                Text = $"{DateTime.Now:G} / Сторінка {pageNo + 1} з {pages}",
                FontSize = 11,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            var rightHeader2 = new TextBlock
            {
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            var b = new Binding { Path = new PropertyPath("Pages.Count"), Source = document };
            rightHeader2.SetBinding(TextBlock.TextProperty, b);

            // rightHeader2.Text
            // headerRow.Children.Add(rightHeader);
            headerRow.Children.Add(rightHeader2);
            offset += ControlHelper.GetFormattedText(leftHeader.Text, leftHeader).Height;

            // Print subHeader
            var subHeaders = _viewModel.GetSubheaders_ExcelAndPrint();
            if (subHeaders != null)
            {
                var subHeaderText = string.Join(Environment.NewLine, subHeaders);
                var subHeaderControl = new TextBlock
                    { Text = subHeaderText, HorizontalAlignment = HorizontalAlignment.Left, FontSize = 11 };
                stackPanel.Children.Add(subHeaderControl);
                offset += ControlHelper.GetFormattedText(subHeaderControl.Text, subHeaderControl).Height;
            }

            var dgArea = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0)
            };
            var dgWidth = _columns.Sum(c => c.ActualWidth) + 1;
            var availableHeight = _pageSize.Height - _pageMargins.Top - _pageMargins.Bottom - offset;
            if (dgWidth > stackPanel.Width)
            {
                var scale = stackPanel.Width / dgWidth;
                dgArea.LayoutTransform = new ScaleTransform(scale, scale);
                availableHeight /= scale;
            }

            var dgOffset = 0.0;

            // Print datagrid header
            var dgHeaders = new StackPanel() { Orientation = Orientation.Horizontal };
            var currentOffset = 0.0;
            for (var k = 0; k < _columns.Length; k++)
            {
                var c = _columns[k];
                var rightBorder = k == _columns.Length - 1 ? 1.0 : 0.0;
                var borderThickness = new Thickness(1, 1, rightBorder, 0);
                var border = new Border
                {
                    BorderThickness = borderThickness,
                    Width = c.ActualWidth + rightBorder,
                    BorderBrush = Brushes.Gray,
                    Background = _headerBackground
                };
                var dgHeader = new TextBlock
                {
                    Text = (c.Header ?? "").ToString(),
                    Width = c.ActualWidth - 1,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Left,
                    Padding = new Thickness(3)
                };
                border.Child = dgHeader;
                dgHeaders.Children.Add(border);

                var a1 = ControlHelper.GetFormattedText(dgHeader.Text, dgHeader);
                a1.MaxTextWidth = c.ActualWidth - 1.0 - dgHeader.Padding.Left - dgHeader.Padding.Top;
                if (a1.Height > currentOffset)
                    currentOffset = a1.Height;
            }
            dgArea.Children.Add(dgHeaders);
            stackPanel.Children.Add(dgArea);
            dgOffset += currentOffset;

            // Items
            var oo = new List<object>();
            var dd = new List<double>();
            var tb1 = new TextBlock();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (var k1 = 0; k1 < _items.Count; k1++)
            {
                var rowHeight = 0.0;
                for (var k2 = 0; k2 < _columns.Length; k2++)
                {
                    var c = _columns[k2];
                    /* var o = c.GetCellContent(_items[k1]);
                    /* if (o is TextBlock tb)
                    {
                        var a1 = ControlHelper.GetFormattedText(tb.Text, tb, TextFormattingMode.Ideal);
                        a1.MaxTextWidth = c.ActualWidth - 5.0;
                        if (a1.Height > rowHeight)
                            rowHeight = a1.Height;
                    }*/

                    if (!string.IsNullOrEmpty(c.SortMemberPath))
                    {
                        var value = _viewModel.Properties[c.SortMemberPath].GetValue(_items[k1]);
                        if (value != null)
                        {
                            var a1 = ControlHelper.GetFormattedText(value.ToString(), tb1);
                            a1.MaxTextWidth = c.ActualWidth - 5.0;
                            if (a1.Height > rowHeight)
                                rowHeight = a1.Height;
                        }
                    }
                }
                dd.Add(rowHeight);
            }

            sw.Stop();
            Debug.Print($"SW: {sw.ElapsedMilliseconds}, {oo.Count}");
            _rows = _rowCount;
            return stackPanel;
        }

    }
}
