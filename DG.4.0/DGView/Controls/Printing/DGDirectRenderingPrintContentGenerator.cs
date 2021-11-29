using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        private Pen _gridPen;

        private IList _items;
        private DataGridColumn[] _columns;
        private Size _pageSize;
        private Thickness _pageMargins;
        private double _gridScale;

        private int[] _rowNumbers;
        private double[] _gridRowHeights;
        private List<int[]> _itemsPerPage = new List<int[]>();
        private double _gridRowHeaderWidth;
        private double _gridColumnHeaderHeight;
        private DateTime _timeStamp;

        private double _pixelsPerDpi;
        private Typeface _baseTypeface;
        private Typeface _titleTypeface;
        private double _titleHeight;
        private double _headerHeight;

        public DGDirectRenderingPrintContentGenerator(DGViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void GeneratePrintContent(FixedDocument document, Thickness margins)
        {
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
            // _headerHeight = new FormattedText("A", LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _headerTypeface, 16.0, Brushes.Black, _pixelsPerDpi).Height;

            CalculateHeaderHeight();
            CalculateGridHeaderRowHeight();
            PrepareRowNumbers();
            CalculateRowHeights();

            _pageSize = document.DocumentPaginator.PageSize;
            _pageMargins = margins;

            var pageWidth = _pageSize.Width - _pageMargins.Left - _pageMargins.Right;
            var gridWidth = _columns.Sum(c => c.ActualWidth) + 1 + _gridRowHeaderWidth;
            _gridScale = gridWidth > pageWidth ? pageWidth / gridWidth : 1.0;
            var availableHeight = (_pageSize.Height - _pageMargins.Top - _pageMargins.Bottom - _headerHeight) / _gridScale;

            _gridPen = new Pen(Brushes.Black, _gridScale);

            GeneratedPages = 0;

            CalculateItemsPerPage(availableHeight);

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
        private void PrepareRowNumbers()
        {
            _rowNumbers = new int[_items.Count];
            var currentNo = 0;
            var currentItem = _items[currentNo];
            var cnt = 0;
            foreach (var item in _viewModel.DGControl.ItemsSource)
            {
                cnt++;
                if (item == currentItem)
                {
                    _rowNumbers[currentNo++] = cnt;
                    if (currentNo >= _items.Count) break;
                    currentItem = _items[currentNo];
                }
                if (currentNo >= _items.Count) break;
            }
            _gridRowHeaderWidth = ControlHelper.GetFormattedText(
                                  _rowNumbers[_rowNumbers.Length - 1].ToString("N0", LocalizationHelper.CurrentCulture),
                                  _viewModel.DGControl).Width + 5.0;
        }

        private void CalculateGridHeaderRowHeight()
        {
            var headerHeight = new FormattedText("A", LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, _viewModel.DGControl.FontSize, Brushes.Black, _pixelsPerDpi).Height;
            for (var k1 = 0; k1 < _columns.Length; k1++)
            {
                var text = (_columns[k1].Header ?? "").ToString();
                if (!string.IsNullOrEmpty(text))
                {
                    var formattedText = new FormattedText(text, LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, _viewModel.DGControl.FontSize, Brushes.Black, _pixelsPerDpi);
                    formattedText.MaxTextWidth = _columns[k1].ActualWidth - 5.0;
                    var cellHeight = formattedText.Height;
                    if (cellHeight > headerHeight)
                        headerHeight = cellHeight;
                }
            }

            _gridColumnHeaderHeight = headerHeight + 5.0;
        }

        private void CalculateRowHeights()
        {
            _gridRowHeights = new double[_items.Count];

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

                _gridRowHeights[i] = rowHeight + 5.0;
            }
        }

        internal void CalculateHeaderHeight()
        {
            _titleHeight = new FormattedText("A", LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _titleTypeface, 16.0, Brushes.Black, _pixelsPerDpi).Height;

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

            _headerHeight = headerHeight;
        }

        private void CalculateItemsPerPage(double availableHeight)
        {
            _itemsPerPage.Clear();
            var startItemNo = 0;

            var dataAreaHeight = (_pageSize.Height - _pageMargins.Top - _pageMargins.Bottom - _headerHeight)/_gridScale - _gridColumnHeaderHeight;
            while (startItemNo < _items.Count)
            {
                var pageHeight = 0.0;
                var currentItemNo = startItemNo;
                while ((currentItemNo < _items.Count && pageHeight < dataAreaHeight - _gridRowHeights[currentItemNo]) || currentItemNo == startItemNo)
                {
                    pageHeight += _gridRowHeights[currentItemNo++];
                }
                _itemsPerPage.Add(new[] { startItemNo, currentItemNo - startItemNo });
                startItemNo = currentItemNo;
            }
        }

        #endregion

        #region ===========  Rendering methods  ================
        private void RenderPage(int pageNo, DrawingContext dc)
        {
            if (pageNo == 0)
                Debug.Print($"Canvas Render: {pageNo}");
            if (dc == null)
                return;

            var itemsInfo = _itemsPerPage[pageNo];
            var fontSize = _viewModel.DGControl.FontSize * _gridScale;
            var yOffset = 0.0;

            // Prepare some data
            var actualColumnWidths = new double[_columns.Length + 1];
            actualColumnWidths[0] = _gridRowHeaderWidth * _gridScale;
            for (var i = 1; i < actualColumnWidths.Length; i++)
                actualColumnWidths[i] = _columns[i - 1].ActualWidth * _gridScale;

            var actualRowHeights = new double[itemsInfo[1] + 1];
            actualRowHeights[0] = _gridColumnHeaderHeight * _gridScale;
            for (var i = 1; i < actualRowHeights.Length; i++)
                actualRowHeights[i] = _gridRowHeights[i - 1] * _gridScale;

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
            var gridLineWidth = actualColumnWidths.Sum() + _gridScale;
            var gridLineHeight = actualRowHeights.Sum() + _gridScale;

            // Draw background of grid column header
            var pen = new Pen(Brushes.Red, _gridScale);
            dc.DrawRectangle(_headerBackground, pen, new Rect(_gridScale / 2, yOffset, gridLineWidth - _gridScale, actualRowHeights[0]));
            var yGridOffset = yOffset + actualRowHeights[0];

            // Draw background of grid row header
            pen = new Pen(Brushes.Orange, _gridScale);
            dc.DrawRectangle(_headerBackground, pen, new Rect(_gridScale / 2, yGridOffset, actualColumnWidths[0], gridLineHeight - actualRowHeights[0] - _gridScale));

            // Draw horizontal grid lines
            dc.DrawLine(_gridPen, new Point(0, yGridOffset), new Point(gridLineWidth, yGridOffset));
            for (var i = 1; i < actualRowHeights.Length; i++)
            {
                yGridOffset += actualRowHeights[i];
                dc.DrawLine(_gridPen, new Point(0, yGridOffset), new Point(gridLineWidth, yGridOffset));
            }

            // Draw vertical grid lines
            var xGridOffset = _gridScale/2 + actualColumnWidths[0];
            var yTo = yOffset + gridLineHeight - _gridScale;
            dc.DrawLine(_gridPen, new Point(xGridOffset, yOffset), new Point(xGridOffset, yTo));
            for (var i = 1; i < actualColumnWidths.Length; i++)
            {
                xGridOffset += actualColumnWidths[i];
                dc.DrawLine(_gridPen, new Point(xGridOffset, yOffset), new Point(xGridOffset, yTo));
            }

            // Draw grid header text
            xGridOffset = actualColumnWidths[0];
            for (var i = 0; i < _columns.Length; i++)
            {
                var column = _columns[i];
                var header = (column.Header ?? "").ToString();
                if (!string.IsNullOrEmpty(header))
                {
                    var cellText = new FormattedText(header, LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, fontSize, Brushes.Black, _pixelsPerDpi);
                    cellText.MaxTextWidth = actualColumnWidths[i + 1] - 5.0 * _gridScale;
                    cellText.MaxTextHeight = actualRowHeights[0] - 5.0 * _gridScale;
                    y = yOffset + (actualRowHeights[0] - cellText.Height) / 2;
                    dc.DrawText(cellText, new Point(xGridOffset + 2.5 * _gridScale, y));
                }

                xGridOffset += actualColumnWidths[i+1];
            }

            // Draw grid rows text

            // Draw cell content
            /* for (var k1 = itemsInfo[0]; k1 < itemsInfo[0] + itemsInfo[1]; k1++)
            {
                var item = _items[k1];
                PrintTextOfRow(dc, k1, offset);
                offset += _rowHeights[k1] * _gridScale;
            }*/
        }

        private void PrintTextOfRow(DrawingContext dc, int rowNo, double offset)
        {
            var tb = new TextBlock();
            tb.FontSize *= _gridScale;
            // Row number
            var text = _rowNumbers[rowNo].ToString("N0", LocalizationHelper.CurrentCulture);
            var formattedText = ControlHelper.GetFormattedText(text, tb);
            var xOffset = (_gridRowHeaderWidth - 1.0 - formattedText.Width) / 2;
            dc.DrawText(formattedText, new Point(1.0 + xOffset, offset));
        }
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
            _gridRowHeights = null;
            _itemsPerPage = null;
        }
        #endregion
    }
}
