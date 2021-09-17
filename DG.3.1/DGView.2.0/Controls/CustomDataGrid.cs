using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using DGCore.DGVList;
using DGView.ViewModels;
using WpfSpLib.Helpers;

namespace DGView.Controls
{
    public class CustomDataGrid : DataGrid
    {
        private static SolidColorBrush[] _groupBrushes;
        private static Brush _groupBorderBrush;

        public DGViewModel ViewModel { get; set; }

        public CustomDataGrid()
        {
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
                _groupBorderBrush = Application.Current.Resources["PrimaryBrush"] as Brush;
            }

            VirtualizingPanel.SetVirtualizationMode(this, VirtualizationMode.Recycling);
        }

        private void OnRowIsReady(DataGridRow row)
        {
            var cellsPresenter = WpfSpLib.Common.Tips.GetVisualChildren(row).OfType<DataGridCellsPresenter>().First();

            UpdateCells(row, cellsPresenter);
        }

        private void UpdateCells(DataGridRow row, DataGridCellsPresenter cellsPresenter)
        {
            var isGroupRow = row.DataContext is IDGVList_GroupItem;
            var groupItem = isGroupRow ? (IDGVList_GroupItem)row.DataContext : null;

            // Set content of group item count column
            if (groupItem != null && ViewModel.GroupItemCountColumn?.GetCellContent(row) is TextBlock txtBlock)
                txtBlock.SetCurrentValueSmart(TextBlock.TextProperty, groupItem.ItemCount.ToString("N0", LocalizationHelper.CurrentCulture));

            // for (var k = 0; k < Columns.Count; k++)
            for (var k = 0; k < ViewModel._groupColumns.Count; k++)
            {

                if (ViewModel._groupColumns[k].Visibility != Visibility.Visible) continue;

                if (!(cellsPresenter.ItemContainerGenerator.ContainerFromIndex(k) is DataGridCell cell))
                {
                    Debug.Print($"No cell: {row.GetIndex()}, {k}");
                    continue;
                }

                var path = WpfSpLib.Common.Tips.GetVisualChildren(cell).OfType<Path>().FirstOrDefault();
                SolidColorBrush cellBrush = null;
                var pathData = Geometry.Empty;
                Thickness border = new Thickness(0, 0 , 0, 1);

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
                            border = new Thickness();
                    }
                }

                if (cell.Background != cellBrush)
                    cell.SetCurrentValue(BackgroundProperty, cellBrush);
                if (pathData != null && path.Data != pathData)
                    path.SetCurrentValue(Path.DataProperty, pathData);
                if (cell.BorderThickness != border)
                    cell.SetCurrentValue(BorderThicknessProperty, border);
                if (cell.BorderBrush != _groupBorderBrush)
                    cell.SetCurrentValue(BorderBrushProperty, _groupBorderBrush);
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

            var row = (DataGridRow) element;
            var cellsPresenter = WpfSpLib.Common.Tips.GetVisualChildren(row).OfType<DataGridCellsPresenter>().First();

            var isGroupRow = row.DataContext is IDGVList_GroupItem;
            var groupItem = isGroupRow ? (IDGVList_GroupItem)row.DataContext : null;

            // Clear content of group item count column
            if (groupItem != null && ViewModel.GroupItemCountColumn?.GetCellContent(row) is TextBlock txtBlock)
                txtBlock.SetCurrentValueSmart(TextBlock.TextProperty, null);

            // Clear content of group columns
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

        private void OnRowLoaded(object sender, RoutedEventArgs e)
        {
            var row = (DataGridRow)sender;
            row.Loaded -= OnRowLoaded;
            OnRowIsReady(row);
        }

        protected override void OnLoadingRow(DataGridRowEventArgs e)
        {
            base.OnLoadingRow(e);

            var rowHeaderText = (e.Row.GetIndex() + 1).ToString("N0", LocalizationHelper.CurrentCulture);
            if (!Equals(e.Row.Header, rowHeaderText)) e.Row.Header = rowHeaderText;

            var isGroupRow = e.Row.DataContext is IDGVList_GroupItem;
            var groupItem = isGroupRow ? (IDGVList_GroupItem)e.Row.DataContext : null;
            e.Row.Tag = isGroupRow ? "1" : null;


            var rowBrush = isGroupRow ? _groupBrushes[groupItem.Level == 0 ? 0 : ((groupItem.Level - 1) % (_groupBrushes.Length - 1)) + 1] : null;
            if (!Equals(rowBrush, e.Row.Background))
            {
                e.Row.SetCurrentValue(BackgroundProperty, rowBrush);
                // e.Row.Background = rowBrush;
            }
        }

    }
}
