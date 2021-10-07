﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
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
        public DGViewModel ViewModel => DataGrid.ViewModel;

        public DataGridView()
        {
            InitializeComponent();

            DataGrid.SelectedCellsChanged += OnDataGridSelectedCellsChanged;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            DataContext = ViewModel;
        }

        private void OnDataGridSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            ViewModel.OnPropertiesChanged(nameof(ViewModel.IsSetFilterOnValueOrSortingEnable), nameof(ViewModel.IsClearSortingEnable));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // DataGrid.ViewModel = ViewModel;
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

        #region ==========  Print  ===============

        public void Print()
        {
            // From https://stackoverflow.com/questions/7931961/wpf-printing-to-fit-page
            // Open print dialog
            PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
            if (printDlg.ShowDialog() == true)
            {
                //get selected printer capabilities
                System.Printing.PrintCapabilities capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);

                //get scale of the print wrt to screen of WPF visual
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / this.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
                                                                                                       this.ActualHeight);

                //Transform the Visual to scale
                this.LayoutTransform = new ScaleTransform(scale, scale);

                //get the size of the printer page
                Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                //update the layout of the visual to the printer page size.
                this.Measure(sz);
                this.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

                //now print the visual to printer to fit on the one page.
                printDlg.PrintVisual(this, "First Fit to Page WPF Print");

            }
        }
        #endregion
    }
}
