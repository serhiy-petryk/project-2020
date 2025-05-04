using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace WpfSpLib.Helpers
{
    public static class DataGridHelper
    {
        public static void Control_OnClosing(DependencyObject control)
        {
            // To force the end editing for all datagrids 
            foreach (var datagrid in control.GetVisualChildren().OfType<DataGrid>())
            {
                datagrid.CommitEdit(DataGridEditingUnit.Cell, true);
                datagrid.CommitEdit(DataGridEditingUnit.Row, true);
                datagrid.CommitEdit();
                datagrid.CancelEdit(DataGridEditingUnit.Cell);
                datagrid.CancelEdit(DataGridEditingUnit.Row);
                datagrid.CancelEdit();
            }
        }

        public static void DataGrid_OnSorting(DataGrid dataGrid, DataGridSortingEventArgs e)
        {
            if (e.Column.SortDirection == ListSortDirection.Descending)
            {
                var view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
                var sd = view.SortDescriptions.OfType<SortDescription>().FirstOrDefault(d => d.PropertyName == e.Column.SortMemberPath);
                view.SortDescriptions.Remove(sd);
                e.Column.SortDirection = null;
                e.Handled = true;
            }
        }

        public static DataGridCell GetDataGridCell(DataGridCellInfo cellInfo)
        {
            var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);
            if (cellContent != null)
                return (DataGridCell)cellContent.Parent;

            return null;
        }

        public static DataGridCell GetDataGridCell(DataGrid dataGrid, DataGridRow row, DataGridColumn column)
        {
            var columnIndex = dataGrid.Columns.IndexOf(column);
            if (columnIndex < 0) return null;

            // Get the row's visual container
            var presenter = row.GetVisualChildren().OfType<DataGridCellsPresenter>().FirstOrDefault();
            if (presenter == null)
            {
                // Force row generation
                dataGrid.ScrollIntoView(row.Item);
                presenter = row.GetVisualChildren().OfType<DataGridCellsPresenter>().FirstOrDefault();
            }

            return presenter?.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
        }
        public static DataGridRow GetDataGridRow(DataGrid datagrid, DataGridCellInfo cellInfo) =>
            datagrid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item) as DataGridRow;
    }
}
