using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace WpfSpLib.Helpers
{
    public interface IIsEmptySupport
    {
        // if IsEmpty will be property than it will be show in datagrid as column
        // Attribute [Browsable(false)] doesn't work (see https://stackoverflow.com/questions/31430659/why-browsablefalse-does-not-hide-columns-in-datagrid)
        public bool IsEmpty();
    }

    public static class DataGridHelper
    {
        public static void DataGrid_OnRowEditEnding(DataGrid dataGrid, DataGridRowEditEndingEventArgs e)
        {
            if (e.Cancel || e.EditAction != DataGridEditAction.Commit) return;

            if (e.Row.DataContext is IIsEmptySupport item && dataGrid.ItemsSource is IList itemList)
            {
                dataGrid.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (item.IsEmpty())
                    {
                        itemList.Remove(item);
                        dataGrid.UpdateAllBindings();
                        // dataGrid.Items.Refresh();
                    }
                }), DispatcherPriority.Loaded);
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
    }
}
