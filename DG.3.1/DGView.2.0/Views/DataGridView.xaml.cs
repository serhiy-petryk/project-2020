using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using DGCore.DGVList;
using DGView.ViewModels;
using WpfSpLib.Helpers;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for DataGridView.xaml
    /// </summary>
    public partial class DataGridView : UserControl
    {
        public DataGridViewModel ViewModel => (DataGridViewModel)DataContext;

        public DataGridView()
        {
            InitializeComponent();
            Unloaded += OnUnloaded;
            DataContext = new DataGridViewModel(this);
        }

        public void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.IsElementDisposing())
            {
                DataGrid.ItemsSource = null;
                DataGrid.Columns.Clear();
                ViewModel.Dispose();
            }
        }

        private void DataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Row numeration
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();

            // Show totals for group item (nested properties)
            if (e.Row.DataContext is IDGVList_GroupItem item)
            {
                var totals = item.GetTotalsForWpfDataGrid();
                if (totals == null) return;

                e.Row.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var cellPresenter = WpfSpLib.Common.Tips.GetVisualChildren(e.Row).OfType<DataGridCellsPresenter>().First();
                    cellPresenter.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
                    SetTotalValues(e.Row, cellPresenter, totals);
                }), DispatcherPriority.Normal);

            }
        }
        private void DataGrid_OnUnloadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is IDGVList_GroupItem)
            {
                var cellPresenter = WpfSpLib.Common.Tips.GetVisualChildren(e.Row).OfType<DataGridCellsPresenter>().First();
                cellPresenter.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
            }
        }

        private static PropertyInfo piPeer = typeof(ItemContainerGenerator).GetProperty("Peer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            var generator = (ItemContainerGenerator)sender;
            if (generator.Status != GeneratorStatus.ContainersGenerated) return;

            var cellPresenter = (DataGridCellsPresenter)piPeer.GetValue(generator);
            var row = (DataGridRow)cellPresenter.TemplatedParent;
            if (row.DataContext is IDGVList_GroupItem item)
            {
                var totals = item.GetTotalsForWpfDataGrid();
                if (totals == null) return;

                SetTotalValues(row, cellPresenter, totals);
            }
        }

        private void SetTotalValues(DataGridRow row, DataGridCellsPresenter cellPresenter, Dictionary<string, object[]> values)
        {
            foreach (var kvp in values)
            {
                var columnIndex = (int?) kvp.Value[1];
                if (!columnIndex.HasValue)
                {
                    for (var k = 0; k < DataGrid.Columns.Count; k++)
                    {
                        if (kvp.Key == DataGrid.Columns[k].SortMemberPath)
                        {
                            kvp.Value[1] = k;
                            columnIndex = k;
                            break;
                        }
                    }

                    if (!columnIndex.HasValue)
                        kvp.Value[1] = -1;
                }

                if (columnIndex >= 0)
                {
                    var cell = (DataGridCell)cellPresenter.ItemContainerGenerator.ContainerFromIndex(columnIndex.Value);
                    if (cell != null)
                    {
                        var textBlock = WpfSpLib.Common.Tips.GetVisualChildren(cell).OfType<TextBlock>().FirstOrDefault();
                        if (textBlock != null)
                            textBlock.Text = kvp.Value[0].ToString();
                    }
                }
            }
        }

    }
}
