using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DGCore.Common;
using DGView.Helpers;
using DGView.ViewModels;
using WpfSpLib.Controls;
using WpfSpLib.Helpers;

namespace DGView.Controls.Printing
{
    internal class DGDirectRenderingPrintContentGenerator : IPrintContentGenerator, INotifyPropertyChanged, IDisposable
    {
        private const double Epsilon = 1e-10;
        public bool StopPrintGeneration { get; set; }
        private int _generatedPages;

        public int GeneratedPages
        {
            get => _generatedPages;
            set
            {
                _generatedPages = value;
                OnPropertiesChanged(nameof(GeneratedPages));
            }
        }

        private DGViewModel _viewModel;
        private readonly SolidColorBrush _headerBackground = new SolidColorBrush(Color.FromRgb(240, 241, 242));
        private DateTime _timeStamp;
        private Pen _gridPen;

        private IList _items;
        private DataGridColumn[] _columns;
        private Size _pageSize;
        private Thickness _pageMargins;
        private double _gridScale;
        private double _halfOfGridLineThickness;
        private double _fontSize;

        private double _pixelsPerDpi;
        private Typeface _baseTypeface;
        private Typeface _titleTypeface;
        private double _titleHeight;
        private double _headerHeight;

        private int[] _rowNumbers;
        private double _actualGridRowHeaderWidth;
        private double _actualGridColumnHeaderHeight;
        private double[] _actualGridRowHeights;
        private double[] _actualGridColumnWidths;
        private List<int[]> _itemsPerPage;

        public DGDirectRenderingPrintContentGenerator(DGViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void GeneratePrintContent(FixedDocument document, Thickness margins)
        {
            _pageSize = document.DocumentPaginator.PageSize;
            _pageMargins = margins;
            DataGridHelper.GetSelectedArea(_viewModel.DGControl, out var items, out var columns);
            _items = items;
            _columns = columns;
            _timeStamp = DateTime.Now;

            if (_items.Count == 0)
            {
                new DialogBox(DialogBox.DialogBoxKind.Warning) { Message = "No items to print!", Buttons = new[] { "OK" } }.ShowDialog();
                return;
            }

            _pixelsPerDpi = VisualTreeHelper.GetDpi(_viewModel.DGControl).PixelsPerDip;
            _baseTypeface = new Typeface(_viewModel.DGControl.FontFamily, _viewModel.DGControl.FontStyle, _viewModel.DGControl.FontWeight, _viewModel.DGControl.FontStretch);
            _titleTypeface = new Typeface(_viewModel.DGControl.FontFamily, _viewModel.DGControl.FontStyle, FontWeights.SemiBold, _viewModel.DGControl.FontStretch);

            _titleHeight = new FormattedText("A", LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _titleTypeface, 16.0, Brushes.Black, _pixelsPerDpi).Height;
             _headerHeight = CalculateHeaderHeight();
            _rowNumbers = CalculateRowNumbers();

            var gridRowHeaderWidth = new FormattedText(
                                         _rowNumbers[_rowNumbers.Length - 1].ToString("N0", LocalizationHelper.CurrentCulture),
                                         LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface,
                                         _viewModel.DGControl.FontSize, Brushes.Black, _pixelsPerDpi).Width + 5.0;

            var pageWidth = _pageSize.Width - _pageMargins.Left - _pageMargins.Right;
            var gridWidth = _columns.Sum(c => c.ActualWidth) + 1 + gridRowHeaderWidth;
            _gridScale = gridWidth > pageWidth ? pageWidth / gridWidth : 1.0;
            _halfOfGridLineThickness = _gridScale / 2;
            _fontSize = _viewModel.DGControl.FontSize * _gridScale;

            _gridPen = new Pen(Brushes.Black, _gridScale);

            _actualGridRowHeaderWidth = gridRowHeaderWidth * _gridScale;
            _actualGridColumnHeaderHeight = CalculateActualGridColumnHeaderHeight();
            _actualGridRowHeights = CalculateActualGridRowHeights();
            _actualGridColumnWidths = CalculateActualGridColumnWidths();
            _itemsPerPage = CalculateItemsPerPage();

            GeneratedPages = 0;

            for (var k = 0; k < _itemsPerPage.Count; k++)
            {
                DoEventsHelper.DoEvents();
                if (StopPrintGeneration)
                    break;
                var pageElement = GetPageElement(k);
                var fixedPage = new FixedPage();
                fixedPage.Children.Add(pageElement);
                var pageContent = new PageContent { Child = fixedPage };
                document.Pages.Add(pageContent);
            }
        }

        private FrameworkElement GetPageElement(int pageNo)
        {
            var renderElement = new RenderElement(pageNo, RenderPage)
            {
                Margin = new Thickness(_pageMargins.Left, _pageMargins.Top, 0, 0),
                Width = _pageSize.Width - _pageMargins.Left - _pageMargins.Right,
                Height = _pageSize.Height - _pageMargins.Top - _pageMargins.Bottom
            };
            return renderElement;
        }

        #region ========  Preparing methods  ============
        private double CalculateHeaderHeight()
        {
            var headerHeight = 5.0;
            var leftHeaderFormattedText = new FormattedText("A", LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _titleTypeface, 16.0, Brushes.Black, _pixelsPerDpi);
            headerHeight += leftHeaderFormattedText.Height;

            // Draw page subheader
            var subHeaders = _viewModel.GetSubheaders_ExcelAndPrint();
            if (subHeaders != null && subHeaders.Length > 0)
            {
                var pageWidth = _pageSize.Width - _pageMargins.Left - _pageMargins.Right;
                var subHeaderText = string.Join(Environment.NewLine, subHeaders);
                var subHeaderFormattedText = new FormattedText(subHeaderText, LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, 12.0, Brushes.Black, _pixelsPerDpi);
                subHeaderFormattedText.MaxTextWidth = pageWidth;
                subHeaderFormattedText.Trimming = TextTrimming.CharacterEllipsis;
                headerHeight += subHeaderFormattedText.Height + 2.0;
            }

            return headerHeight;
        }

        private int[] CalculateRowNumbers()
        {
            var rowNumbers = new int[_items.Count];
            var currentNo = 0;
            var currentItem = _items[currentNo];
            var cnt = 0;
            foreach (var item in _viewModel.DGControl.ItemsSource)
            {
                cnt++;
                if (item == currentItem)
                {
                    rowNumbers[currentNo++] = cnt;
                    if (currentNo >= _items.Count) break;
                    currentItem = _items[currentNo];
                }
                if (currentNo >= _items.Count) break;
            }

            return rowNumbers;
        }

        private double CalculateActualGridColumnHeaderHeight()
        {
            var headerHeight = new FormattedText("A", LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, _viewModel.DGControl.FontSize, Brushes.Black, _pixelsPerDpi).Height;
            foreach (var column in _columns)
            {
                var text = (column.Header ?? "").ToString();
                if (!string.IsNullOrEmpty(text))
                {
                    var formattedText = new FormattedText(text, LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, _viewModel.DGControl.FontSize, Brushes.Black, _pixelsPerDpi);
                    formattedText.MaxTextWidth = column.ActualWidth - 5.0;
                    var cellHeight = formattedText.Height;
                    if (cellHeight > headerHeight)
                        headerHeight = cellHeight;
                }
            }

            return (headerHeight + 5.0) * _gridScale;
        }

        private double[] CalculateActualGridRowHeights()
        {
            var rowHeights = new double[_items.Count];

            var minRowHeight = new FormattedText("A", LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, _viewModel.DGControl.FontSize, Brushes.Black, _pixelsPerDpi).Height;
            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var rowHeight = minRowHeight;
                foreach (var column in _columns.Where(c => !string.IsNullOrEmpty(c.SortMemberPath)))
                {
                    var value = _viewModel.Properties[column.SortMemberPath].GetValue(item);
                    if (value != null)
                    {
                        var cellText = new FormattedText(value.ToString(), LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, _viewModel.DGControl.FontSize, Brushes.Black, _pixelsPerDpi);
                        if (_viewModel.RowViewMode == Enums.DGRowViewMode.WordWrap)
                            cellText.MaxTextWidth = column.ActualWidth - 5.0;
                        if (cellText.Height > rowHeight)
                            rowHeight = cellText.Height;
                    }
                }

                rowHeights[i] = (rowHeight + 5.0) * _gridScale;
            }

            return rowHeights;
        }

        private double[] CalculateActualGridColumnWidths() => _columns.Select(c => c.ActualWidth * _gridScale).ToArray();

        private List<int[]> CalculateItemsPerPage()
        {
            var itemsPerPage = new List<int[]>();
            var startItemNo = 0;

            var dataAreaHeight = _pageSize.Height - _pageMargins.Top - _pageMargins.Bottom - _headerHeight - _actualGridColumnHeaderHeight - _gridScale;
            while (startItemNo < _items.Count)
            {
                var pageHeight = 0.0;
                var currentItemNo = startItemNo;
                while ((currentItemNo < _items.Count && pageHeight < (dataAreaHeight - _actualGridRowHeights[currentItemNo] + Epsilon)) || currentItemNo == startItemNo)
                {
                    pageHeight += _actualGridRowHeights[currentItemNo++];
                }
                itemsPerPage.Add(new[] { startItemNo, currentItemNo - startItemNo });
                startItemNo = currentItemNo;
            }

            return itemsPerPage;
        }

        #endregion

        #region ===========  Rendering methods  ================
        private void RenderPage(int pageNo, DrawingContext dc)
        {
            if (dc == null)
                return;

            var minItemNo = _itemsPerPage[pageNo][0];
            var maxItemNo = minItemNo + _itemsPerPage[pageNo][1] - 1;
            var yOffset = 0.0;

            // Draw background for test
            var pageWidth = _pageSize.Width - _pageMargins.Left - _pageMargins.Right;
            var pageHeight = _pageSize.Height - _pageMargins.Top - _pageMargins.Bottom;
            dc.DrawRectangle(Brushes.Yellow, null, new Rect(0, 0, pageWidth, pageHeight));

            // Draw page header
            var rightHeaderText = $"{_timeStamp:G} / Сторінка {pageNo + 1} з {_itemsPerPage.Count}";
            var rightHeaderFormattedText = new FormattedText(rightHeaderText, LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, 12.0, Brushes.Black, _pixelsPerDpi);
            rightHeaderFormattedText.MaxTextWidth = pageWidth;
            rightHeaderFormattedText.Trimming = TextTrimming.CharacterEllipsis;
            rightHeaderFormattedText.MaxLineCount = 1;
            var x = Math.Max(0, pageWidth - rightHeaderFormattedText.Width);
            var y = (_titleHeight - rightHeaderFormattedText.Height) / 2;
            dc.DrawText(rightHeaderFormattedText, new Point(x, y));

            var leftHeaderText = _viewModel.Title;
            var lefttHeaderFormattedText = new FormattedText(leftHeaderText, LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _titleTypeface, 16.0, Brushes.Black, _pixelsPerDpi);
            lefttHeaderFormattedText.MaxTextWidth = pageWidth - rightHeaderFormattedText.Width;
            lefttHeaderFormattedText.Trimming = TextTrimming.CharacterEllipsis;
            lefttHeaderFormattedText.MaxLineCount = 1;
            dc.DrawText(lefttHeaderFormattedText, new Point(0.0, 0.0));
            yOffset += lefttHeaderFormattedText.Height;

            // Draw page subheader
            var subHeaders = _viewModel.GetSubheaders_ExcelAndPrint();
            if (subHeaders != null && subHeaders.Length > 0)
            {
                var subHeaderText = string.Join(Environment.NewLine, subHeaders);
                var subHeaderFormattedText = new FormattedText(subHeaderText, LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, 12.0, Brushes.Black, _pixelsPerDpi);
                subHeaderFormattedText.MaxTextWidth = pageWidth;
                subHeaderFormattedText.Trimming = TextTrimming.CharacterEllipsis;
                dc.DrawText(subHeaderFormattedText, new Point(0, yOffset + 2));
                yOffset += subHeaderFormattedText.Height + 2.0;
            }

            yOffset += 5.0;
            var gridLineWidth = _actualGridRowHeaderWidth + _actualGridColumnWidths.Sum() + _gridScale;
            var gridLineHeight = _actualGridColumnHeaderHeight + _actualGridRowHeights.Where((h, index) => index >= minItemNo && index <= maxItemNo).Sum() + _gridScale;

            // Draw background of grid column header
            var pen = new Pen(Brushes.Red, _gridScale);
            dc.DrawRectangle(_headerBackground, pen, new Rect(_halfOfGridLineThickness, yOffset + _halfOfGridLineThickness, gridLineWidth - _gridScale, _actualGridColumnHeaderHeight));
            var yGridOffset = yOffset + _halfOfGridLineThickness + _actualGridColumnHeaderHeight;

            // Draw background of grid row header
            pen = new Pen(Brushes.Orange, _gridScale);
            dc.DrawRectangle(Brushes.GreenYellow, pen, new Rect(_halfOfGridLineThickness, yGridOffset, _actualGridRowHeaderWidth, gridLineHeight - _actualGridColumnHeaderHeight - _gridScale));

            // Draw horizontal grid lines
            dc.DrawLine(_gridPen, new Point(0, yGridOffset), new Point(gridLineWidth, yGridOffset));
            pen = new Pen(Brushes.BlueViolet, _gridScale);
            for (var i = minItemNo; i <= maxItemNo; i++)
            {
                yGridOffset += _actualGridRowHeights[i];
                dc.DrawLine(pen, new Point(0, yGridOffset), new Point(gridLineWidth, yGridOffset));
            }

            // Draw vertical grid lines
            var xGridOffset = _halfOfGridLineThickness + _actualGridRowHeaderWidth;
            var yTo = yOffset + gridLineHeight - _gridScale;
            dc.DrawLine(_gridPen, new Point(xGridOffset, yOffset), new Point(xGridOffset, yTo));
            for (var i = 0; i < _actualGridColumnWidths.Length; i++)
            {
                xGridOffset += _actualGridColumnWidths[i];
                dc.DrawLine(_gridPen, new Point(xGridOffset, yOffset), new Point(xGridOffset, yTo));
            }

            // Draw grid header text
            xGridOffset = _actualGridRowHeaderWidth;
            for (var i = 0; i < _columns.Length; i++)
            {
                var column = _columns[i];
                var x11 = xGridOffset + 3.0 * _gridScale;
                var y11 = yOffset + 3.0 * _gridScale; ;
                dc.DrawRectangle(Brushes.Aqua, null, new Rect(x11, y11, _actualGridColumnWidths[i] - 5.0 * _gridScale, _actualGridColumnHeaderHeight - 5.0 * _gridScale));

                // var x11 = xGridOffset + 2.0 * _gridScale;
                // var x12 = _actualGridColumnWidths[i] -3.0 * _gridScale;
                // dc.DrawRectangle(Brushes.Aqua, null, new Rect(x11, yOffset + 1.5 * _gridScale, x12, _actualGridColumnHeaderHeight -3*_gridScale));
                var header = (column.Header ?? "").ToString();
                if (!string.IsNullOrEmpty(header))
                {
                    var cellText = new FormattedText(header, LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, _fontSize, Brushes.Black, _pixelsPerDpi);
                    cellText.MaxTextWidth = _actualGridColumnWidths[i] - 4.5 * _gridScale;
                    cellText.MaxTextHeight = _actualGridColumnHeaderHeight - 4.5 * _gridScale;
                    y = yOffset + (_actualGridColumnHeaderHeight + _gridScale - cellText.Height) / 2; // center
                    dc.DrawText(cellText, new Point(xGridOffset + 3.0 * _gridScale, y)); // left alignment
                }

                xGridOffset += _actualGridColumnWidths[i];
            }

            // Draw grid rows text
            yGridOffset = yOffset + _actualGridColumnHeaderHeight;
            for (var i = minItemNo; i <= maxItemNo; i++)
            {
                var x11 = 3.0 * _gridScale;
                var y11 = yGridOffset + 3.0 * _gridScale; ;
                dc.DrawRectangle(Brushes.Aqua, null, new Rect(x11, y11, _actualGridRowHeaderWidth- 5.0*_gridScale, _actualGridRowHeights[i] - 5.0* _gridScale));
                var text = _rowNumbers[i].ToString("N0", LocalizationHelper.CurrentCulture);
                var cellText = new FormattedText(text, LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, _fontSize, Brushes.Black, _pixelsPerDpi);

                cellText.MaxTextWidth = _actualGridRowHeaderWidth - 4.5 * _gridScale;
                cellText.MaxTextHeight = _actualGridRowHeights[i] - 4.5 * _gridScale;
                x = (_actualGridRowHeaderWidth + _gridScale - cellText.Width) / 2.0; // center
                y = yGridOffset + (_actualGridRowHeights[i] + _gridScale - cellText.Height) / 2; // center
                dc.DrawText(cellText, new Point(x, y));
                yGridOffset += _actualGridRowHeights[i];
            }


            // Draw cell content
            /* for (var k1 = itemsInfo[0]; k1 < itemsInfo[0] + itemsInfo[1]; k1++)
            {
                var item = _items[k1];
                PrintTextOfRow(dc, k1, offset);
                offset += _rowHeights[k1] * _gridScale;
            }*/
        }

        /*private void PrintTextOfRow(DrawingContext dc, int rowNo, double offset)
        {
            var tb = new TextBlock();
            tb.FontSize *= _gridScale;
            // Row number
            var text = _rowNumbers[rowNo].ToString("N0", LocalizationHelper.CurrentCulture);
            var formattedText = ControlHelper.GetFormattedText(text, tb);
            var xOffset = (_gridRowHeaderWidth - 1.0 - formattedText.Width) / 2;
            dc.DrawText(formattedText, new Point(1.0 + xOffset, offset));
        }*/
        #endregion

        #region =============  Common methods ==============
        #endregion

        #region ===========  INotifyPropertyChanged  ==============
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region ===========  IDisposing  ==============
        public void Dispose()
        {
            _viewModel = null;
            _items = null;
            _columns = null;
            _pageSize = Size.Empty;
            _rowNumbers = null;
            _actualGridRowHeights = null;
            _actualGridColumnWidths = null;
            _itemsPerPage = null;
        }
        #endregion
    }
}
