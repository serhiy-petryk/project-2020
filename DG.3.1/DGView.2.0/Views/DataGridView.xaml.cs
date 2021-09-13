using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
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
        private const bool IsVerticalScrollbarDeferred = false;
        private static Brush[] _groupBrushes;
        public DGViewModel ViewModel => (DGViewModel)DataContext;

        public DataGridView()
        {
            InitializeComponent();
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
            }

            DataGrid.SelectedCellsChanged += OnDataGridSelectedCellsChanged;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            DataContext = new DGViewModel(this);
        }

        private void OnDataGridSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            ViewModel.OnPropertiesChanged(nameof(ViewModel.IsSetFilterOnValueOrSortingEnable), nameof(ViewModel.IsClearSortingEnable));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (IsVerticalScrollbarDeferred)
            {
                UnwireScrollViewer();
                _scrollViewer = WpfSpLib.Common.Tips.GetVisualChildren(DataGrid).OfType<ScrollViewer>()
                    .FirstOrDefault();
                WireScrollViewer();
            }
        }
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            UnwireScrollViewer();
            if (this.IsElementDisposing())
            {
                DataGrid.SelectedCellsChanged -= OnDataGridSelectedCellsChanged;
                _scrollViewer = null;
                DataGrid.ItemsSource = null;
                DataGrid.Columns.Clear();
                ViewModel.Dispose();
            }
        }

        private void DataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Row numeration
            // not working e.Row.SetCurrentValueSmart(DataGridRow.HeaderProperty, (e.Row.GetIndex() + 1).ToString());
            var rowHeaderText = (e.Row.GetIndex() + 1).ToString("N0", LocalizationHelper.CurrentCulture);
            if (!Equals(e.Row.Header, rowHeaderText))
            e.Row.Header = rowHeaderText;

            // Show totals for group item (nested properties)
            if (e.Row.DataContext is IDGVList_GroupItem item)
            {
                var rowBrush = _groupBrushes[item.Level == 0 ? 0 : ((item.Level - 1) % (_groupBrushes.Length - 1)) + 1];
                if (e.Row.Background != rowBrush) e.Row.Background = rowBrush;

                e.Row.Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Set content of group item count column
                    if (ViewModel.GroupItemCountColumn?.GetCellContent(e.Row) is TextBlock cell)
                        cell.SetCurrentValueSmart(TextBlock.TextProperty, item.ItemCount.ToString("N0", LocalizationHelper.CurrentCulture));

                    // Set content of group columns
                    for (var k = 0; k < ViewModel._groupColumns.Count; k++)
                    {
                        var cellContent = ViewModel._groupColumns[k].GetCellContent(e.Row);
                        if (cellContent != null)
                        {
                            var path = WpfSpLib.Common.Tips.GetVisualChildren(cellContent).OfType<Path>().First();
                            if (item.Level > 0 && k == (item.Level - 1))
                            {
                                var geometry = item.IsExpanded ? DGViewModel.MinusSquareGeometry : DGViewModel.PlusSquareGeometry;
                                if (path.Data != geometry)
                                    path.SetCurrentValueSmart(Path.DataProperty, geometry);
                            }
                            else
                            {
                                if (path.Data != Geometry.Empty)
                                    path.SetCurrentValueSmart(Path.DataProperty, Geometry.Empty);
                            }
                        }
                    }

                    // ===================
                    var totals = item.GetTotalsForWpfDataGrid();
                    if (totals == null) return;

                    var cellPresenter = WpfSpLib.Common.Tips.GetVisualChildren(e.Row).OfType<DataGridCellsPresenter>().First();
                    cellPresenter.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                    cellPresenter.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
                    SetTotalValuesForNestedProperties(e.Row, totals);
                }), DispatcherPriority.Normal);
            }
            else
            {
                if (e.Row.Background != null) e.Row.Background = null;

                e.Row.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (ViewModel.GroupItemCountColumn?.GetCellContent(e.Row) is TextBlock cell)
                        cell.SetCurrentValueSmart(TextBlock.TextProperty, null);

                    // Set content of group columns
                    for (var k = 0; k < ViewModel._groupColumns.Count; k++)
                    {
                        var cellContent = ViewModel._groupColumns[k].GetCellContent(e.Row);
                        if (cellContent != null)
                        {
                            var path = WpfSpLib.Common.Tips.GetVisualChildren(cellContent).OfType<Path>().First();
                            if (path.Data != Geometry.Empty)
                                path.SetCurrentValueSmart(Path.DataProperty, Geometry.Empty);
                        }
                    }
                }), DispatcherPriority.Normal);
            }
        }

        private void DataGrid_OnUnloadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is IDGVList_GroupItem)
            {
                // Clear content of group item count column
                if (ViewModel.GroupItemCountColumn?.GetCellContent(e.Row) is TextBlock cell)
                    cell.SetCurrentValueSmart(TextBlock.TextProperty, "");

                var cellPresenter = WpfSpLib.Common.Tips.GetVisualChildren(e.Row).OfType<DataGridCellsPresenter>().First();
                cellPresenter.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
            }
        }

        private static PropertyInfo piHost = typeof(ItemContainerGenerator).GetProperty("Host", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            var generator = (ItemContainerGenerator)sender;
            if (generator.Status != GeneratorStatus.ContainersGenerated) return;

            var cellPresenter = (DataGridCellsPresenter)piHost.GetValue(generator);
            var row = (DataGridRow)cellPresenter.TemplatedParent;
            if (row.DataContext is IDGVList_GroupItem item)
            {
                var totals = item.GetTotalsForWpfDataGrid();
                if (totals == null) return;

                SetTotalValuesForNestedProperties(row, totals);
            }
        }

        private void SetTotalValuesForNestedProperties(DataGridRow row, Dictionary<string, object[]> values)
        {
            foreach (var kvp in values)
            {
                var columnIndex = (int?) kvp.Value[1];
                if (!columnIndex.HasValue)
                    columnIndex = Helpers.DataGridHelper.GetColumnIndexByPropertyName(DataGrid, kvp.Key);

                if (columnIndex >= 0)
                {
                    var cellContent = DataGrid.Columns[columnIndex.Value].GetCellContent(row);
                    var value = kvp.Value[0] is double d && double.IsNaN(d) ? "" : kvp.Value[0].ToString();
                    if (cellContent is TextBlock textBlock && textBlock.Text != value)
                    {
                        // Debug.Print($"Cell: {row.Header}, {kvp.Value[0]}, {textBlock.Text}");
                        textBlock.SetCurrentValueSmart(TextBlock.TextProperty, kvp.Value[0].ToString());
                    }
                }
            }
        }

        private void OnGroupLevelContextMenuOpened(object sender, RoutedEventArgs e)
        {
            var cm = (ContextMenu) sender;
            cm.Items.Clear();

            var currentGroupLevel = ViewModel.Data.ExpandedGroupLevel;
            var showUpperLevels = ViewModel.Data.ShowGroupsOfUpperLevels;
            var cnt = 0;
            for (var i = 0; i < ViewModel.Data.Groups.Count; i++)
            {
                var item = new MenuItem {Header = (i + 1) + " рівень", Command=ViewModel.CmdSetGroupLevel, CommandParameter = i+1 };
                cm.Items.Add(item);
                if ((i == 0 && currentGroupLevel == 1) || (i + 1) == currentGroupLevel && showUpperLevels)
                    item.IsChecked = true;
                cnt++;
            }
            for (int i = 1; i < ViewModel.Data.Groups.Count; i++)
            {
                var item = new MenuItem { Header = (i + 1) + " рівень (не показувати рядки вищого рівня)", Command = ViewModel.CmdSetGroupLevel, CommandParameter = - (i + 1) };
                cm.Items.Add(item);
                if ((i + 1) == currentGroupLevel && !showUpperLevels)
                    item.IsChecked = true;
                cnt++;
            }

            var item2 = new MenuItem { Header = "Вся інформація", Command = ViewModel.CmdSetGroupLevel};
            cm.Items.Add(item2);
            if (currentGroupLevel == int.MaxValue && showUpperLevels)
                item2.IsChecked = true;
        }

        private void OnSetSettingsContextMenuOpened(object sender, RoutedEventArgs e)
        {
            var cm = (ContextMenu)sender;
            foreach (var mi in WpfSpLib.Common.Tips.GetVisualChildren(cm).OfType<MenuItem>())
                mi.IsChecked = Equals(mi.Header, ViewModel.LastAppliedLayoutName);
        }
        private void OnRowViewModeContextMenuOpened(object sender, RoutedEventArgs e)
        {
            var cm = (ContextMenu)sender;
            foreach (var mi in WpfSpLib.Common.Tips.GetVisualChildren(cm).OfType<MenuItem>())
            {
                var rowViewMode = (DGCore.Common.Enums.DGRowViewMode)Enum.Parse(typeof(DGCore.Common.Enums.DGRowViewMode), (string)mi.CommandParameter);
                mi.IsChecked = Equals(rowViewMode, ViewModel.RowViewMode);
            }
        }

        #region ========  ScrollViewer  =========
        private ScrollViewer _scrollViewer;
        private void WireScrollViewer()
        {
            if (_scrollViewer != null)
            {
                foreach (var bar in WpfSpLib.Common.Tips.GetVisualChildren(_scrollViewer).OfType<ScrollBar>())
                    bar.PreviewMouseLeftButtonDown += OnScrollBarPreviewMouseLeftButtonDown;
            }
        }
        private void UnwireScrollViewer()
        {
            if (_scrollViewer != null)
            {
                foreach (var bar in WpfSpLib.Common.Tips.GetVisualChildren(_scrollViewer).OfType<ScrollBar>())
                    bar.PreviewMouseLeftButtonDown -= OnScrollBarPreviewMouseLeftButtonDown;
                _scrollViewer = null;
            }
        }

        private void OnScrollBarPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var bar = (ScrollBar)sender;
            var sv = (ScrollViewer)bar.TemplatedParent;
            var isDeferredScrollingEnabled = bar.Orientation == Orientation.Vertical;
            if (sv.IsDeferredScrollingEnabled != isDeferredScrollingEnabled)
                sv.IsDeferredScrollingEnabled = isDeferredScrollingEnabled;
        }
        #endregion

        private void OnDataGridCellMouseEnter(object sender, MouseEventArgs e)
        {
            var cell = (DataGridCell) sender;
            if (cell.Column is DataGridTextColumn txtColumn)
            {
                var txtBlock = WpfSpLib.Common.Tips.GetVisualChildren(cell).OfType<TextBlock>().FirstOrDefault();
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
                var image = WpfSpLib.Common.Tips.GetVisualChildren(cell).OfType<Image>().FirstOrDefault();
                if (image != null && image.Source != null)
                {
                    if (image.ActualWidth < (image.Source.Width - 0.001) || image.ActualHeight < (image.Source.Height - 0.001))
                    {
                        var toolTipPanel = new Grid();
                        var origImage = new Image {Source = image.Source};
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
            var cell = (DataGridCell) sender;
            // Toggle item group
            if (cell.DataContext is IDGVList_GroupItem item && item.Level > 0)
            {
                if (ViewModel._groupColumns[item.Level - 1] == cell.Column)
                {
                    var row = DataGridRow.GetRowContainingElement(cell);
                    ViewModel.Data.ItemExpandedChanged(row.GetIndex());
                }
            }
        }
    }
}
