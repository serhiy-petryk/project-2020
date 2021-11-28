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
    internal class DGPrintContentGeneratorUsingDirectRendering : IPrintContentGenerator, INotifyPropertyChanged, IDisposable
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

        internal IList _items;
        private DataGridColumn[] _columns;
        private Size _pageSize;
        private Thickness _pageMargins;
        private double _gridScale;

        internal int[] _rowNumbers;
        internal double[] _rowHeights;
        internal List<int[]> _itemsPerPage = new List<int[]>();
        private double _rowHeaderWidth;
        // private int _currentItemNo;
        private DateTime _timeStamp;

        public DGPrintContentGeneratorUsingDirectRendering(DGViewModel viewModel)
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
                new DialogBox(DialogBox.DialogBoxKind.Warning) { Message = "No items to print!", Buttons = new[] { "OK" } }
                    .ShowDialog();
                return;
            }

            PrepareRowNumbers();
            CalculateRowHeights();

            _pageSize = document.DocumentPaginator.PageSize;
            _pageMargins = margins;

            var pageWidth = _pageSize.Width - _pageMargins.Left - _pageMargins.Right;
            var gridWidth = _columns.Sum(c => c.ActualWidth) + 1 + _rowHeaderWidth;
            _gridScale = gridWidth > pageWidth ? pageWidth / gridWidth : 1.0;
            var availableHeight = (_pageSize.Height - _pageMargins.Top - _pageMargins.Bottom - CalculateHeaderHeight()) / _gridScale;

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

            dc.DrawRectangle(Brushes.Yellow, null, new Rect(0, 0, 682, 912));
            var itemsInfo = _itemsPerPage[pageNo];
            var offset = CalculateHeaderHeight();
            for (var k1 = itemsInfo[0]; k1 < itemsInfo[0] + itemsInfo[1]; k1++)
            {
                var item = _items[k1];
                PrintTextOfRow(dc, k1, offset);
                offset += _rowHeights[k1] * _gridScale;
            }
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
