using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DGCore.DGVList;
using DGView.ViewModels;
using WpfSpLib.Common;
using WpfSpLib.Helpers;

namespace DGView.Controls
{
    /// <summary>
    /// Interaction logic for CustomDataGrid.xaml
    /// </summary>
    public partial class CustomDataGrid 
    {
        private static SolidColorBrush[] _groupBrushes;
        public static Brush GroupBorderBrush { get; private set; }

        public DGViewModel ViewModel { get; }

        public CustomDataGrid()
        {
            InitializeComponent();
            ViewModel = new DGViewModel(this);
            DataContext = ViewModel;

            if (_groupBrushes == null)
            {
                _groupBrushes = new[]
                {
                    Brushes.Gainsboro, new SolidColorBrush(Color.FromArgb(255, 255, 153, 204)),
                    new SolidColorBrush(Color.FromArgb(255, 255,204, 153)),
                    new SolidColorBrush(Color.FromArgb(255, 255,255,153)),
                    new SolidColorBrush(Color.FromArgb(255, 204, 255,204)),
                    new SolidColorBrush(Color.FromArgb(255, 204,255,255)),
                    new SolidColorBrush(Color.FromArgb(255, 153, 204, 255)),
                    new SolidColorBrush(Color.FromArgb(255,204, 153,  255))
                };
                GroupBorderBrush = Application.Current.Resources["PrimaryBrush"] as Brush;
            }

            VirtualizingPanel.SetVirtualizationMode(this, VirtualizationMode.Recycling);
        }

        #region =======  Override methods  ============
        protected override void OnLoadingRow(DataGridRowEventArgs e)
        {
            base.OnLoadingRow(e);

            var rowHeaderText = (e.Row.GetIndex() + 1).ToString("N0", LocalizationHelper.CurrentCulture);
            if (!Equals(e.Row.Header, rowHeaderText)) e.Row.Header = rowHeaderText;

            var isGroupRow = e.Row.DataContext is IDGVList_GroupItem;
            var groupItem = isGroupRow ? (IDGVList_GroupItem)e.Row.DataContext : null;
            e.Row.Tag = isGroupRow ? "1" : null;

            // Set font of row
            if (isGroupRow)
            {
                var factor = ViewModel._fontFactors[groupItem.Level];
                if (factor > 0.5)
                {
                    e.Row.FontSize = factor * FontSize;
                    e.Row.FontWeight = FontWeights.Bold;
                }
            }

            var rowBrush = isGroupRow ? _groupBrushes[groupItem.Level == 0 ? 0 : ((groupItem.Level - 1) % (_groupBrushes.Length - 1)) + 1] : null;
            if (!Equals(rowBrush, e.Row.Background))
            {
                e.Row.SetCurrentValue(BackgroundProperty, rowBrush);
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var row = (DataGridRow)element;
            if (row.IsLoaded)
                OnRowIsReady(row);
            else
            {
                row.Loaded -= OnRowLoaded;
                row.Loaded += OnRowLoaded;
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            var row = (DataGridRow)element;

            var isGroupRow = row.DataContext is IDGVList_GroupItem;
            var groupItem = isGroupRow ? (IDGVList_GroupItem)row.DataContext : null;

            row.FontSize = FontSize;
            row.FontWeight = FontWeight;

            // Clear content of group item count column
            if (groupItem != null && ViewModel.GroupItemCountColumn?.GetCellContent(row) is TextBlock txtBlock)
                txtBlock.SetCurrentValueSmart(TextBlock.TextProperty, null);

            // Clear content of group columns
            var cellsPresenter = row.GetVisualChildren().OfType<DataGridCellsPresenter>().FirstOrDefault();
            if (cellsPresenter == null) return;
            for (var k = 0; k < ViewModel._groupColumns.Count; k++)
            {
                var cell = cellsPresenter.ItemContainerGenerator.ContainerFromIndex(k) as DataGridCell;
                if (cell?.Background != null)
                {
                    cell.SetCurrentValue(BackgroundProperty, null);
                    // cell.Background = cellBrush;
                }

            }
        }

        protected override void OnSelectedCellsChanged(SelectedCellsChangedEventArgs e)
        {
            base.OnSelectedCellsChanged(e);
            foreach (var cellInfo in e.RemovedCells.Where(c => c.IsValid))
            {
                var k = ViewModel._groupColumns.IndexOf(cellInfo.Column);
                if (k < 0) continue;

                var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);
                if (cellContent == null) continue;

                var cell = (DataGridCell)cellContent.Parent;
                var isGroupRow = cellInfo.Item is IDGVList_GroupItem;
                var groupItem = isGroupRow ? (IDGVList_GroupItem)cellInfo.Item : null;
                SolidColorBrush cellBrush = null;
                if (!isGroupRow)
                    cellBrush = _groupBrushes[k % (_groupBrushes.Length - 1) + 1];
                else if (k < (groupItem.Level - 1))
                    cellBrush = _groupBrushes[k % (_groupBrushes.Length - 1) + 1];

                if (cell.Background != cellBrush)
                    cell.SetCurrentValue(BackgroundProperty, cellBrush);
                if (cell.BorderBrush != GroupBorderBrush)
                    cell.SetCurrentValue(BorderBrushProperty, GroupBorderBrush);
            }
        }
        #endregion

        #region ======  Private Methods  ==========
        private void OnRowLoaded(object sender, RoutedEventArgs e)
        {
            var row = (DataGridRow)sender;
            row.Loaded -= OnRowLoaded;
            OnRowIsReady(row);
        }

        private void OnRowIsReady(DataGridRow row)
        {
            var cellsPresenter = row.GetVisualChildren().OfType<DataGridCellsPresenter>().FirstOrDefault();
            if (cellsPresenter != null)
                UpdateCells(row, cellsPresenter);
        }

        private void UpdateCells(DataGridRow row, DataGridCellsPresenter cellsPresenter)
        {
            var isGroupRow = row.DataContext is IDGVList_GroupItem;
            var groupItem = isGroupRow ? (IDGVList_GroupItem)row.DataContext : null;
            var rowIndex = row.GetIndex();

            // Set content of group item count column
            if (groupItem != null && ViewModel.GroupItemCountColumn?.GetCellContent(row) is TextBlock txtBlock)
                txtBlock.SetCurrentValueSmart(TextBlock.TextProperty, groupItem.ItemCount.ToString("N0", LocalizationHelper.CurrentCulture));

            // for (var k = 0; k < Columns.Count; k++)
            var firstVisibleColumn = true;
            for (var k = 0; k < ViewModel._groupColumns.Count; k++)
            {

                if (ViewModel._groupColumns[k].Visibility != Visibility.Visible) continue;

                if (!(cellsPresenter.ItemContainerGenerator.ContainerFromIndex(k) is DataGridCell cell))
                {
                    Debug.Print($"No cell: {rowIndex}, {k}");
                    continue;
                }

                var borderDot = cell.GetVisualChildren().OfType<Panel>().FirstOrDefault(p => p.Width < 1.1);
                var isBorderDotVisible = false;
                var path = cell.GetVisualChildren().OfType<Path>().FirstOrDefault();
                SolidColorBrush cellBrush = null;
                var pathData = Geometry.Empty;
                var border = new Thickness(0, 0, 0, 1);

                if (!isGroupRow)
                {
                    cellBrush = _groupBrushes[k % (_groupBrushes.Length - 1) + 1];
                    border = new Thickness(0, 0, 1, 0);
                }
                else
                {
                    if (k < (groupItem.Level - 1))
                    {
                        cellBrush = _groupBrushes[k % (_groupBrushes.Length - 1) + 1];
                        border = new Thickness(0, 0, 1, 0);
                    }
                    else if (k > (groupItem.Level - 1))
                    {
                    }
                    else if (groupItem.Level > 0)
                    {
                        pathData = groupItem.IsExpanded ? DGViewModel.MinusSquareGeometry : DGViewModel.PlusSquareGeometry;
                        if (groupItem.IsExpanded)
                        {
                            border = new Thickness();
                            isBorderDotVisible = true;
                        }
                    }
                }

                // Set bottom border 
                var bottomBorder = false;
                var nextIndex = rowIndex + 1;
                var nextItem = nextIndex < Items.Count ? Items[nextIndex] : null;
                if (nextItem == null)
                    bottomBorder = true;
                else
                {
                    var nextItemLevel = nextItem is IDGVList_GroupItem nextGroupItem ? nextGroupItem.Level : -1;
                    var thisItemLevel = groupItem?.Level ?? -1;
                    if ((nextItemLevel > 0 || nextItemLevel < thisItemLevel) && k >= (nextItemLevel - 1) && nextItemLevel > 0)
                        bottomBorder = true;
                }
                if (bottomBorder)
                    border.Bottom = 1;

                // Set left border 
                if (firstVisibleColumn)
                {
                    firstVisibleColumn = false;
                    if (rowIndex != 0 || (isGroupRow && groupItem.Level > 0))
                        border.Left = 1;
                }

                // Set border dot visibility
                if (borderDot != null)
                    borderDot.Visibility = isBorderDotVisible ? Visibility.Visible : Visibility.Collapsed;

                // Set margin of path
                if (path != null && pathData != Geometry.Empty)
                {
                    var box = path.Parent as Viewbox;
                    var yMargin = (cell.ActualHeight - 13) / 2;
                    var xMargin = (cell.ActualWidth - 13) / 2;
                    var boxMargin = new Thickness(xMargin - border.Left - 0.5, yMargin - border.Top - 0.5,
                        xMargin - border.Right + 0.5, yMargin - border.Bottom + 0.5);
                    box.Margin = boxMargin;
                }

                if (cell.Background != cellBrush)
                    cell.SetCurrentValue(BackgroundProperty, cellBrush);
                if (pathData != null && path.Data != pathData)
                    path.SetCurrentValue(Path.DataProperty, pathData);
                if (cell.BorderThickness != border)
                    cell.SetCurrentValue(BorderThicknessProperty, border);
                if (cell.BorderBrush != GroupBorderBrush)
                    cell.SetCurrentValue(BorderBrushProperty, GroupBorderBrush);
            }
        }
        #endregion

        #region ===========  Event handlers  ============
        private void OnDataGridCellMouseEnter(object sender, MouseEventArgs e)
        {
            var cell = (DataGridCell)sender;
            if (cell.Column is DataGridTextColumn txtColumn)
            {
                var txtBlock = cell.GetVisualChildren().OfType<TextBlock>().FirstOrDefault();
                if (txtBlock != null)
                {
                    if (!string.IsNullOrEmpty(txtBlock.Text) && WpfSpLib.Common.Tips.IsTextTrimmed(txtBlock))
                        ToolTipService.SetToolTip(cell, txtBlock.Text);
                    else
                        ToolTipService.SetToolTip(cell, null);
                }
            }
            else if (cell.Column is DataGridTemplateColumn templateColumn)
            {
                var image = cell.GetVisualChildren().OfType<Image>().FirstOrDefault();
                if (image?.Source != null)
                {
                    if (image.ActualWidth < (image.Source.Width - 0.001) || image.ActualHeight < (image.Source.Height - 0.001))
                    {
                        var toolTipPanel = new Grid();
                        var origImage = new Image { Source = image.Source };
                        toolTipPanel.Children.Add(origImage);
                        ToolTipService.SetToolTip(cell, toolTipPanel);
                    }
                    else
                        ToolTipService.SetToolTip(cell, null);
                }
            }
        }

        private void OnDataGridCellPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = (DataGridCell)sender;
            // Toggle item group
            if (cell.DataContext is IDGVList_GroupItem item && item.Level > 0)
            {
                if (ViewModel._groupColumns[item.Level - 1] == cell.Column)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var row = DataGridRow.GetRowContainingElement(cell);
                        ViewModel.Data.ItemExpandedChanged(row.GetIndex());
                        ViewModel.SetColumnVisibility();
                    }));
                }
            }
        }
        #endregion
    }
}
