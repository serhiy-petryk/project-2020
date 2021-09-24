using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DG.Common;
using MahApps.Metro.Controls;

namespace DG.UI
{
    /// <summary>
    /// Interaction logic for MainWindowSort.xaml
    /// </summary>
    public partial class frmDG : MetroWindow
    {
        private const bool AUTOGENERATE_COLUMNS = true;

        public static readonly DependencyProperty IsCommandBarEnabledProperty =
          DependencyProperty.Register("IsCommandBarEnabled", typeof(bool), typeof(frmDG),
            new UIPropertyMetadata(true));

        public bool IsCommandBarEnabled
        {
            get => (bool)GetValue(IsCommandBarEnabledProperty);
            set => SetValue(IsCommandBarEnabledProperty, value);
        }

        //===========
        public static ObservableCollection<string> LogData = new ObservableCollection<string>();
        private DGCore.Menu.RootMenu _rootMenu;
        private static Type _dataType;
        private DGListComponent _dGListComponent;

        public frmDG()
        {
            InitializeComponent();
            DataContext = new ViewModels.DataGridViewModel();
            Cb_Log.ItemsSource = LogData;
            LogData.Add("Test");
        }

        private void frmDG_OnLoaded(object sender, RoutedEventArgs e)
        {
            _rootMenu = new DGCore.Menu.RootMenu();
            Title = _rootMenu.ApplicationTitle;
            FontFamily = new FontFamily("Microsoft Sans Serif");
            FontSize = 9.0 * (96.0 / 72.0);

            // Attach event for opacity in case of Enabled/Disabled
            foreach (var btn in CommandToolBar.Items.Cast<UIElement>().Where(item => item is ButtonBase))
                btn.IsEnabledChanged += UIElement_OnIsEnabledChanged;

            DataGrid.AutoGenerateColumns = AUTOGENERATE_COLUMNS;

            // Toggle grid visibility services
            var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(DataGrid.GridLinesVisibilityProperty, typeof(DataGrid));
            dpd.AddValueChanged(DataGrid, GridLinesVisibilityChanged);
            DataGrid.GridLinesVisibility = DataGridGridLinesVisibility.All;

            Cb_Themes.ItemsSource = Themes.ThemeInfo.Themes;
            Cb_Themes.SelectedValue = Themes.ThemeInfo.Themes.FirstOrDefault(th => string.Equals(th.Id, Themes.ThemeInfo.StartupTheme, StringComparison.InvariantCultureIgnoreCase));
            Cb_Themes.Width = Common.WpfUtils.GetListWidth(Cb_Themes);

            Btn_CellViewMode.ItemsSource = CellViewModeClass.CellViewModeValues;
            Btn_CellViewMode.SelectedValue = Btn_CellViewMode.Items[1];
            Btn_CellViewMode.Width = Common.WpfUtils.GetListWidth(Btn_CellViewMode);

            DownButton1.ItemsSource = Themes.ThemeInfo.Themes;
            DownButton1.DisplayMemberPath = "Name";

            Btn_GroupLayer.ItemsSource = Themes.ThemeInfo.Themes;
            var fiMenu = typeof(DropDownButton).GetField("menu", BindingFlags.Instance | BindingFlags.NonPublic);
            var cm = fiMenu.GetValue(Btn_GroupLayer) as ContextMenu;
            cm.Loaded += Cm_Loaded;

            cm = fiMenu.GetValue(Btn_OpenSetting) as ContextMenu;
            cm.Loaded += Cm_Loaded;
        }
        private void frmDG_OnClosed(object sender, EventArgs e)
        {
            // Detach event for opacity in case of Enabled/Disabled
            foreach (var btn in CommandToolBar.Items.Cast<UIElement>().Where(item => item is ButtonBase))
                btn.IsEnabledChanged -= UIElement_OnIsEnabledChanged;

            var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(DataGrid.GridLinesVisibilityProperty,
              typeof(DataGrid));
            dpd.RemoveValueChanged(DataGrid, GridLinesVisibilityChanged);

            var fiMenu = typeof(DropDownButton).GetField("menu", BindingFlags.Instance | BindingFlags.NonPublic);
            var cm = fiMenu.GetValue(Btn_GroupLayer) as ContextMenu;
            cm.Loaded -= Cm_Loaded;

            cm = fiMenu.GetValue(Btn_OpenSetting) as ContextMenu;
            cm.Loaded -= Cm_Loaded;

            Cb_Themes.SelectionChanged -= Do_ThemesChanged;

            DgClear();
        }

        // ===========  Service methods  ===============
        private void UIElement_OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) => (sender as FrameworkElement).Opacity = (bool)e.NewValue ? 1.0 : 0.5;
        private void DropDownButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button && Equals(button.IsChecked, true))
            {
                var items = button.Resources["items"];
                if (items is ContextMenu)
                {
                    var cm = (ContextMenu)items;
                    if (cm.PlacementTarget == null)
                    {
                        cm.PlacementTarget = button;
                        cm.Placement = PlacementMode.Bottom;
                        cm.Closed += (senderClosed, eClosed) => ((ToggleButton)sender).IsChecked = false;
                        cm.Loaded += Cm_Loaded;
                    }
                    cm.IsOpen = true;
                }
                else if (items is Popup)
                {
                    var popup = (Popup)items;
                    if (popup.PlacementTarget == null)
                    {
                        popup.PlacementTarget = button;
                        popup.Placement = PlacementMode.Bottom;
                        popup.Closed += (senderClosed, eClosed) => ((ToggleButton)sender).IsChecked = false;
                    }
                    popup.IsOpen = true;
                }
            }
        }

        private void Cm_Loaded(object sender, RoutedEventArgs e)
        {
            var menu = (ItemsControl)sender;
            var items = Common.WpfUtils.GetChildOfType<MenuItem>(menu).ToArray();

            var aa = new List<DependencyObject>();
            DependencyObject parentDepObj = (ItemsControl)sender;
            do
            {
                aa.Add(parentDepObj);
                parentDepObj = VisualTreeHelper.GetParent(parentDepObj);
            }
            while (parentDepObj != null);
            var popup = (FrameworkElement)aa[aa.Count - 1];

            var zoom = uiScaleSlider.Value;
            if (zoom > 1)
            {
                var pTransform = popup.RenderTransform as ScaleTransform;
                if (pTransform != null)
                {
                    ((ScaleTransform)popup.RenderTransform).ScaleX = 1;
                    ((ScaleTransform)popup.RenderTransform).ScaleY = 1;
                }

                // Transform menu items
                foreach (var mi in items)
                {
                    var actualSize = (double[])mi.Tag;
                    if (actualSize == null)
                    {
                        actualSize = new[] { mi.ActualWidth, mi.ActualHeight };
                        mi.Tag = actualSize;
                        mi.RenderTransform = new ScaleTransform(zoom, zoom, 0, 0);
                    }
                    else
                    {
                        var transform = (ScaleTransform)mi.RenderTransform;
                        transform.ScaleX = zoom;
                        transform.ScaleY = zoom;
                    }
                    mi.Width = actualSize[0] * zoom;
                    mi.Height = actualSize[1] * zoom;
                }
            }
            else
            {
                // Reset items scale
                foreach (var mi in items)
                {
                    var actualSize = (double[])mi.Tag;
                    if (actualSize != null)
                    {
                        ((ScaleTransform)mi.RenderTransform).ScaleX = 1;
                        ((ScaleTransform)mi.RenderTransform).ScaleY = 1;
                        mi.Width = actualSize[0];
                        mi.Height = actualSize[1];
                    }
                }
                // Transform top popup
                var transform = popup.RenderTransform as ScaleTransform;
                if (transform == null)
                {
                    transform = new ScaleTransform(zoom, zoom, 0, 0);
                    popup.RenderTransform = transform;
                }
                else
                {
                    ((ScaleTransform)popup.RenderTransform).ScaleX = zoom;
                    ((ScaleTransform)popup.RenderTransform).ScaleY = zoom;
                }
            }
        }

        //==========================================
        private void Load_OnClick(object sender, RoutedEventArgs e)
        {
            var x1 = (DGCore.Menu.MenuOption)((DGCore.Menu.SubMenu)((DGCore.Menu.SubMenu)_rootMenu.Items[1]).Items[1]).Items[1];
            var dd = x1.GetDataDefiniton();
            Bind(dd, null, null, null, null);
        }

        private void LoadGlDocList_OnClick(object sender, RoutedEventArgs e)
        {
            var x1 = (DGCore.Menu.MenuOption)((DGCore.Menu.SubMenu)((DGCore.Menu.SubMenu)_rootMenu.Items[1]).Items[0]).Items[0];
            var dd = x1.GetDataDefiniton();
            Bind(dd, null, null, null, null);
        }

        private void Clear_OnClick(object sender, RoutedEventArgs e) => DgClear();

        private void Records_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Debug_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Btn_OpenSetting.HasItems || Btn_OpenSetting.ItemsSource == Themes.ThemeInfo.Themes)
                Btn_OpenSetting.ItemsSource = CellViewModeClass.CellViewModeValues;
            else
                Btn_OpenSetting.ItemsSource = Themes.ThemeInfo.Themes;
        }

        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            double delta = 0.01;

            if (e.HorizontalChange < delta || e.HorizontalChange > delta)
                Canvas.SetLeft(AAA, Canvas.GetLeft(AAA) + e.HorizontalChange);
            if (e.VerticalChange < delta || e.VerticalChange > delta)
                Canvas.SetTop(AAA, Canvas.GetTop(AAA) + e.VerticalChange);

        }
    }
}
