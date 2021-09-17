using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
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

        private void OnRowReady(DataGridRow row)
        {
            var cellsPresenter = WpfSpLib.Common.Tips.GetVisualChildren(row).OfType<DataGridCellsPresenter>().First();
            UpdateCells(row, cellsPresenter);
        }

        private void UpdateCells(DataGridRow row, DataGridCellsPresenter cellsPresenter)
        {
            // for (var k = 0; k < Columns.Count; k++)
            for (var k = 0; k < ViewModel._groupColumns.Count; k++)
            {

                var cell = cellsPresenter.ItemContainerGenerator.ContainerFromIndex(k) as DataGridCell;
                if (cell == null)
                {
                    // Debug.Print($"No cell: {row.GetIndex()}, {k}");
                    continue;
                }

                var isGroupRow = cell.DataContext is IDGVList_GroupItem;
                var groupItem = isGroupRow ? (IDGVList_GroupItem)cell.DataContext : null;

                if (!ViewModel._groupColumns.Contains(Columns[k]))
                {
                    if (cell.Background != null)
                    {
                        cell.SetCurrentValue(BackgroundProperty, null);
                        // cell.Background = cellBrush;
                    }
                    continue;
                }

                if (!isGroupRow)
                {
                    var cellBrush = _groupBrushes[k % (_groupBrushes.Length - 1) + 1];
                    if (cell.Background != cellBrush)
                    {
                        cell.SetCurrentValue(BackgroundProperty, cellBrush);
                        // cell.Background = cellBrush;
                    }
                }
                else
                {
                    if (k < (groupItem.Level - 1))
                    {
                        var cellBrush = _groupBrushes[k % (_groupBrushes.Length - 1) + 1];
                        if (cell.Background != cellBrush)
                        {
                            cell.SetCurrentValue(BackgroundProperty, cellBrush);
                            // cell.Background= cellBrush;
                        }
                    }
                    else if (k > (groupItem.Level - 1))
                    {
                        if (cell.Background != null)
                        {
                            cell.SetCurrentValue(BackgroundProperty, null);
                            // cell.Background = null;
                        }
                    }
                    else if (groupItem.Level > 0)
                    {
                        if (cell.Background != null)
                        {
                            cell.SetCurrentValue(BackgroundProperty, null);
                            // cell.Background = null;
                        }
                    }
                }
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var row = (DataGridRow)element;
            if (row.IsLoaded)
                OnRowReady(row);
            else
            {
                row.Loaded -= OnRowLoaded;
                row.Loaded += OnRowLoaded;
            }
        }

        /*protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            var row = (DataGridRow) element;
            var cellsPresenter = WpfSpLib.Common.Tips.GetVisualChildren(row).OfType<DataGridCellsPresenter>().First();
            for (var k = 0; k < ViewModel._groupColumns.Count; k++)
            {
                var cell = cellsPresenter.ItemContainerGenerator.ContainerFromIndex(k) as DataGridCell;
                if (cell?.Background != null)
                {
                    cell.SetCurrentValue(BackgroundProperty, null);
                    // cell.Background = cellBrush;
                }

            }
        }*/

        private void OnRowLoaded(object sender, RoutedEventArgs e)
        {
            var row = (DataGridRow)sender;
            row.Loaded -= OnRowLoaded;
            OnRowReady(row);
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
