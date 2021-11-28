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

        internal IList _items;
        private DataGridColumn[] _columns;
        private Size _pageSize;
        private Thickness _pageMargins;
        private double _gridScale;

        internal int[] _rowNumbers;
        internal double[] _rowHeights;
        internal List<int[]> _itemsPerPage = new List<int[]>();
        private double _rowHeaderWidth;
        private DateTime _timeStamp;

        private double _pixelsPerDpi;
        private Typeface _baseTypeface;
        private Typeface _headerTypeface;
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
            _headerTypeface = new Typeface(_viewModel.DGControl.FontFamily, _viewModel.DGControl.FontStyle, FontWeights.SemiBold, _viewModel.DGControl.FontStretch);
            _headerHeight = new FormattedText("A", LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _headerTypeface, 16.0, Brushes.Black, _pixelsPerDpi).Height;

            PrepareRowNumbers();
            CalculateRowHeights();

            _pageSize = document.DocumentPaginator.PageSize;
            _pageMargins = margins;

            var pageWidth = _pageSize.Width - _pageMargins.Left - _pageMargins.Right;
            var gridWidth = _columns.Sum(c => c.ActualWidth) + 1 + _rowHeaderWidth;
            _gridScale = gridWidth > pageWidth ? pageWidth / gridWidth : 1.0;
            var availableHeight = (_pageSize.Height - _pageMargins.Top - _pageMargins.Bottom - CalculateHeaderHeight()) / _gridScale;

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
            _rowHeaderWidth = ControlHelper.GetFormattedText(
                                  _rowNumbers[_rowNumbers.Length - 1].ToString("N0", LocalizationHelper.CurrentCulture),
                                  _viewModel.DGControl).Width + 5.0;
        }

        private void CalculateRowHeights()
        {
            _rowHeights = new double[_items.Count];
            var minRowHeight = ControlHelper.GetFormattedText("A", _viewModel.DGControl).Height;
            for (var k1 = 0; k1 < _items.Count; k1++)
            {
                var item = _items[k1];
                var rowHeight = minRowHeight;
                foreach (var column in _columns.Where(c => !string.IsNullOrEmpty(c.SortMemberPath)))
                {
                    var value = _viewModel.Properties[column.SortMemberPath].GetValue(item);
                    if (value != null)
                    {
                        var cellText = ControlHelper.GetFormattedText(value.ToString(), _viewModel.DGControl);
                        if (_viewModel.RowViewMode == Enums.DGRowViewMode.WordWrap)
                            cellText.MaxTextWidth = column.ActualWidth - 5.0;
                        if (cellText.Height > rowHeight)
                            rowHeight = cellText.Height;
                    }
                }

                _rowHeights[k1] = rowHeight + 5.0;
            }
        }

        internal double CalculateHeaderHeight()
        {
            return 26.28;
        }

        private void CalculateItemsPerPage(double availableHeight)
        {
            _itemsPerPage.Clear();
            var startItemNo = 0;

            while (startItemNo < _items.Count)
            {
                var pageHeight = 0.0;
                var currentItemNo = startItemNo;
                while ((currentItemNo < _items.Count && pageHeight < availableHeight + _rowHeights[currentItemNo]) || currentItemNo == startItemNo)
                {
                    pageHeight += _rowHeights[currentItemNo++];
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

            // Prepare some data
            var actualColumnWidths = new double[_columns.Length + 1];
            actualColumnWidths[0] = _rowHeaderWidth * _gridScale;
            for (var k = 0; k < _columns.Length; k++)
                actualColumnWidths[k + 1] = _columns[k].ActualWidth * _gridScale;

            var pageWidth = _pageSize.Width - _pageMargins.Left - _pageMargins.Right;
            var pageHeight = _pageSize.Height - _pageMargins.Top - _pageMargins.Bottom;
            dc.DrawRectangle(Brushes.Yellow, null, new Rect(0, 0, pageWidth, pageHeight));

            // Draw page header
            var rightHeaderText = $"{_timeStamp:G} / Сторінка {pageNo + 1} з {_itemsPerPage.Count}";
            var rightHeaderFormattedText = new FormattedText(rightHeaderText, LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _baseTypeface, 12.0, Brushes.Black, _pixelsPerDpi);
            var x = Math.Max(0, pageWidth - rightHeaderFormattedText.Width);
            var y = (_headerHeight - rightHeaderFormattedText.Height) / 2;
            dc.DrawText(rightHeaderFormattedText, new Point(x, y));

            var leftHeaderText = _viewModel.Title;
            var lefttHeaderFormattedText = new FormattedText(leftHeaderText, LocalizationHelper.CurrentCulture, FlowDirection.LeftToRight, _headerTypeface, 16.0, Brushes.Black, _pixelsPerDpi);
            lefttHeaderFormattedText.MaxTextWidth = pageWidth - rightHeaderFormattedText.Width;
            lefttHeaderFormattedText.Trimming = TextTrimming.CharacterEllipsis;
            lefttHeaderFormattedText.MaxLineCount = 1;
            dc.DrawText(lefttHeaderFormattedText, new Point(0.0, 0.0));

            // Draw page subheader

            // Draw item's grid
            var offset = CalculateHeaderHeight();
            var yGridOffset = offset;
            var gridLineWidth = actualColumnWidths.Sum() + _gridScale;
            var itemsInfo = _itemsPerPage[pageNo];

            for (var k1 = itemsInfo[0]; k1 < itemsInfo[0] + itemsInfo[1]; k1++)
            {
                dc.DrawLine(_gridPen, new Point(0, yGridOffset), new Point(gridLineWidth, yGridOffset));
                yGridOffset += _rowHeights[k1] * _gridScale;
            }
            dc.DrawLine(_gridPen, new Point(0, yGridOffset), new Point(gridLineWidth, yGridOffset));

            var xGridOffset = 0.0;
            for (var k1 = 0; k1 < actualColumnWidths.Length; k1++)
            {
                dc.DrawLine(_gridPen, new Point(xGridOffset, offset), new Point(xGridOffset, yGridOffset));
                xGridOffset += actualColumnWidths[k1];
            }
            dc.DrawLine(_gridPen, new Point(xGridOffset, offset), new Point(xGridOffset, yGridOffset));

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
            var xOffset = (_rowHeaderWidth - 1.0 - formattedText.Width) / 2;
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
            _rowHeights = null;
            _itemsPerPage = null;
        }
        #endregion
    }
}
