using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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
        public DGViewModel ViewModel => (DGViewModel)DataContext;

        public DataGridView()
        {
            InitializeComponent();

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
            DataGrid.ViewModel = ViewModel;
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

        #region ==========  Event handlers of CommandBar  ==========
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
        #endregion

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
                    ViewModel.SetColumnVisibility();
                }
            }
        }
    }
}
