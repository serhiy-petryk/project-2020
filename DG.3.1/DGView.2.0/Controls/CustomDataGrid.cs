﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            cellsPresenter.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
            cellsPresenter.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            UpdateCells(row, cellsPresenter);
        }

        private void UpdateCells(DataGridRow row, DataGridCellsPresenter cellsPresenter)
        {
            // for (var k = 0; k < Columns.Count; k++)
            for (var k = 0; k < ViewModel._groupColumns.Count; k++)
            {

                // if (ViewModel._groupColumns[k].Visibility != Visibility.Visible) continue;

                var cell = cellsPresenter.ItemContainerGenerator.ContainerFromIndex(k) as DataGridCell;
                if (cell == null)
                {
                    // Debug.Print($"No cell: {row.GetIndex()}, {k}");
                    return;
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
                    return;
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

        private void ClearCell(DataGridCell cell)
        {
            if (cell.Background != null)
                cell.SetCurrentValue(BackgroundProperty, null);
        }

        private static PropertyInfo piHost = typeof(ItemContainerGenerator).GetProperty("Host", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        // private static MethodInfo miHost = typeof(ItemContainerGenerator).GetMethod("get_Host", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private static MethodInfo miRecyclableContainers = typeof(ItemContainerGenerator).GetMethod("get_RecyclableContainers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private Dictionary<ItemContainerGenerator, Tuple<DataGridCellsPresenter, Func<IEnumerable>>> _aa = new Dictionary<ItemContainerGenerator, Tuple<DataGridCellsPresenter, Func<IEnumerable>>>();
        private Dictionary<ItemContainerGenerator, Func<IEnumerable>> _aa2 = new Dictionary<ItemContainerGenerator, Func<IEnumerable>>();

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            // Debug.Print($"StatusChanged: {tmp2++}");
            var generator = (ItemContainerGenerator)sender;
            if (generator.Status != GeneratorStatus.ContainersGenerated) return;

            /*if (!_aa.ContainsKey(generator))
            {
                var presenter = (DataGridCellsPresenter)piHost.GetValue(generator);
                Func<IEnumerable> c2 = (Func<IEnumerable>)miRecyclableContainers.CreateDelegate(typeof(Func<IEnumerable>), generator);
                _aa.Add(generator, new Tuple<DataGridCellsPresenter, Func<IEnumerable>>(presenter, c2));
                Debug.Print($"New generator: {_aa.Count}");
            }*/

            if (!_aa2.ContainsKey(generator))
            {
                // var presenter = (DataGridCellsPresenter)piHost.GetValue(generator);
                Func<IEnumerable> c2 = (Func<IEnumerable>)miRecyclableContainers.CreateDelegate(typeof(Func<IEnumerable>), generator);
                _aa2.Add(generator, c2);
                // Debug.Print($"New generator: {_aa.Count}");
            }

            // var a1 = _aa[generator];
            // var cells = (ICollection)a1.Item2();
            var cells = (ICollection)_aa2[generator]();

            if (cells.Count == 0)
            {
                // var row = (DataGridRow)a1.Item1.TemplatedParent;
                // var i = row.GetIndex();
                // Debug.Print($"UpdateCells: {i}");
                // UpdateCells(row, a1.Item1);
            }
            else
            {
                // var row = (DataGridRow)a1.Item1.TemplatedParent;
                // var i = row.GetIndex();
                foreach (DataGridCell cell in cells)
                {
                    //var k = Columns.IndexOf(cell.Column);
                    //Debug.Print($"UpdateCell: {i}, {k}, {cell.Column.SortMemberPath}");
                    // UpdateCell(cell);
                    ClearCell(cell);
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
            if (!Equals(e.Row.Header, rowHeaderText))
                e.Row.Header = rowHeaderText;

            var isGroupRow = e.Row.DataContext is IDGVList_GroupItem;
            var groupItem = isGroupRow ? (IDGVList_GroupItem)e.Row.DataContext : null;

            var rowBrush = isGroupRow ? _groupBrushes[groupItem.Level == 0 ? 0 : ((groupItem.Level - 1) % (_groupBrushes.Length - 1)) + 1] : null;
            if (!Equals(rowBrush, e.Row.Background))
            {
                e.Row.SetCurrentValue(BackgroundProperty, rowBrush);
                // e.Row.Background = rowBrush;
            }
        }

    }
}
