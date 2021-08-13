using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using DGCore.Menu;
using DGCore.UserSettings;
using DGView.Temp;
using WpfSpLib.Controls;
using WpfSpLib.Helpers;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for MwiLeftPanelView.xaml
    /// </summary>
    public partial class MwiLeftPanelView : UserControl, INotifyPropertyChanged, IUserSettingSupport<List<Filter>>
    {
        private MwiContainer Host => MwiContainer.GetMwiContainer(this);

        public DGCore.Misc.DataDefiniton DataDefinition { get; private set; }
        public string SettingKeyOfDataDefinition => DataDefinition?.SettingID;
        public string ErrorText { get; private set; }

        public MwiLeftPanelView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void LeftPanelView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Width = Math.Min(800, SystemParameters.WorkArea.Width * 0.7);
                // Load menu items
                var rootMenu = new RootMenu(DGCore.Misc.AppSettings.CONFIG_FILE_NAME);
                MenuTreeView.ItemsSource = rootMenu.Items;
                // Set application header
                if (!string.IsNullOrEmpty(rootMenu.ApplicationTitle) && Window.GetWindow(this) is MwiStartup app)
                    app.Title = rootMenu.ApplicationTitle;
            }
        }

        private void TreeViewItem_OnExpanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
            if (item.Items.Count > 0)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
                {
                    // BringIntoView for TreeViewItem
                    var scrollViewer = WpfSpLib.Common.Tips.GetVisualParents(item).OfType<ScrollViewer>().FirstOrDefault();
                    if (scrollViewer != null)
                    {
                        var upPoint = item.TransformToVisual(scrollViewer).Transform(new Point(0, 0)).Y + scrollViewer.VerticalOffset;
                        var downPoint = upPoint + item.ActualHeight;
                        if (scrollViewer.ActualHeight + scrollViewer.VerticalOffset < downPoint)
                        {
                            var newOffset = Math.Min(upPoint, downPoint - scrollViewer.ActualHeight);
                            scrollViewer.BeginAnimationAsync(ScrollViewerAnimator.VerticalOffsetProperty, scrollViewer.VerticalOffset, newOffset);
                            //var sb = new Storyboard();
                            //sb.Children.Add(WpfSpLib.Helpers.AnimationHelper.GetScrollViewerVerticalOffsetAnimation(scrollViewer, scrollViewer.VerticalOffset, newOffset));
                            //sb.Begin();
                        }
                    }
                }));
                e.Handled = true;
            }
        }

        private void Menu_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var menuOption = e.NewValue as MenuOption;
            if (menuOption != null)
            {
                ActivateMenuOption(menuOption);
                e.Handled = true;
            }
            RefreshUI();
        }

        private void TreeViewItem_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (TreeViewItem)sender;
            if (!(item.DataContext is MenuOption)) // Submenu
                item.IsExpanded = !item.IsExpanded;
            e.Handled = true;
            RefreshUI();
        }

        private void MenuOption_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                e.Handled = true;
                ActionProcedure();
            }
        }

        private void ActivateMenuOption(MenuOption menuOption)
        {
            if (menuOption == null)
                return;

            // Reset error decoration of menu item
            var menuItems = WpfSpLib.Common.Tips.GetVisualChildren(MenuTreeView).OfType<TextBlock>().Where(item => item.DataContext == menuOption);
            foreach (var item in menuItems.Where(i => i.TextDecorations.Contains(TextDecorations.Strikethrough[0])))
                item.TextDecorations = new TextDecorationCollection();

            try
            {
                // Check on database error
                CbDataSettingName.SelectedIndex = -1;
                DataDefinition = menuOption?.GetDataDefiniton();
                if (DataDefinition != null)
                {
                    var userSettingProperties = new FakeUserSettingProperties
                    {
                        SettingKind = DataGridView.UserSettingsKind,
                        SettingKey = DataDefinition.SettingID
                    };

                    if (!string.IsNullOrEmpty(SettingKeyOfDataDefinition))
                        CbDataSettingName.ItemsSource = UserSettingsUtils.GetKeysFromDb(userSettingProperties);

                    var parameters = DataDefinition.DbParameters;
                    if (parameters == null || parameters._parameters.Count == 0)
                    {
                        DbProcedureParameterArea.Visibility = Visibility.Collapsed;
                        FilterArea.Visibility = DataDefinition.WhereFilter == null ? Visibility.Collapsed : Visibility.Visible;
                        ErrorText = null;
                        if (DataDefinition.WhereFilter != null)
                            DbFilterView.Bind(DataDefinition.WhereFilter, DataDefinition.SettingID, ActionProcedure, null);
                    }
                    else
                    {
                        DbProcedureParameterArea.Visibility = Visibility.Visible;
                        FilterArea.Visibility = Visibility.Collapsed;
                        ErrorText = DataDefinition.DbParameters.GetError();
                        // ToDo: Bind ParameterView & parameter list: this.pg.SelectedObject = parameters;
                    }
                }
            }
            catch (Exception ex)
            {
                foreach (var item in menuItems)
                    item.TextDecorations = TextDecorations.Strikethrough;
                DataDefinition = null;
                DialogMessage.ShowDialog(ex.ToString(), "Помилка", DialogMessage.DialogMessageIcon.Error);
            }
        }

        private void ActionProcedure()
        {
            var mo = MenuTreeView.SelectedItem as MenuOption;
            var dd = mo?.GetDataDefiniton();
            if (dd == null)
                return;

            var dgView = new DataGridView(Host, mo, (string)CbDataSettingName.SelectedValue, null);
            
            Host.HideLeftPanel();
        }

        #region ============  INotifyPropertyChanged  ============
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void RefreshUI()
        {
            OnPropertiesChanged(nameof(DataDefinition), nameof(SettingKeyOfDataDefinition), nameof(ErrorText));
            OnPropertiesChanged(nameof(DbWhereFilter), nameof(FilterText)); // old code
        }
        #endregion

        // ==================  Old code  ===================

        public DGCore.Filters.FilterList DbWhereFilter => DataDefinition?.WhereFilter;
        public string FilterText => DbWhereFilter?.StringPresentation;
        private string _lastSelectedSetting;

        private void LoadData_OnClick(object sender, RoutedEventArgs e)
        {
            /*if (_menuOption != null)
            {
                var dgView = new DataGridView(_menuOption);
                AppViewModel.Instance.ContainerControl.HideLeftPanel();
            }*/
        }

        private void OnFilterEditPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            /** var cell = (DataGridCell)sender;
            var filterLine = cell.DataContext as DGCore.Filters.FilterLineBase;
            var view = new FilterLineView(filterLine);
            var height = Math.Max(200, Window.GetWindow(this).ActualHeight * 2 / 3);
            WpfSpLib.Common.Tips.ShowMwiChildDialog(view, "Dialog", new Size(double.NaN, height));*/
        }

        private void OnDefinitionGridLoaded(object sender, RoutedEventArgs e)
        {
            /** // Set initial width of LeftPanel
            if (AppViewModel.Instance.ContainerControl != null) // check => to prevent MS designer error
                AppViewModel.Instance.ContainerControl.LeftPanelContainer.Width = Math.Min(800, SystemParameters.WorkArea.Width * 0.7);*/
        }

        private void OpenSettingButton_OnChecked(object sender, RoutedEventArgs e)
        {
            var button = (ToggleButton)sender;
            if (button.IsChecked == true)
            {
                DropDownButtonHelper.OpenDropDownMenu(sender);
                var keys = UserSettingsUtils.GetKeysFromDb(this);
                var cm = button.Resources.Values.OfType<ContextMenu>().First();
                cm.Items.Clear();
                foreach (var key in keys)
                {
                    cm.Items.Add(new MenuItem
                    {
                        Header = key,
                        IsChecked = key == _lastSelectedSetting,
                        // DataContext = item,
                        Command = new WpfSpLib.Common.RelayCommand((p) => SetSetting(key))
                    });
                }
            }
        }

        private void SetSetting(string settingName)
        {
            UserSettingsUtils.SetSetting(this, settingName);
            _lastSelectedSetting = settingName;
            RefreshUI();
        }

        #region  ==============  IUserSettingSupport  ===================
        public string SettingKind => "DGV_DBFilter";
        public string SettingKey => DataDefinition.SettingID;

        List<Filter> IUserSettingSupport<List<Filter>>.GetSettings() =>
            ((IUserSettingSupport<List<Filter>>)DbWhereFilter).GetSettings();

        List<Filter> IUserSettingSupport<List<Filter>>.GetBlankSetting() =>
            ((IUserSettingSupport<List<Filter>>)DbWhereFilter).GetBlankSetting();

        void IUserSettingSupport<List<Filter>>.SetSetting(List<Filter> settings) =>
            ((IUserSettingSupport<List<Filter>>)DbWhereFilter).SetSetting(settings);
        #endregion

        private void ClearFilter_OnClick(object sender, RoutedEventArgs e)
        {
            // this.ucFilter.FilterList.ClearFilter();
        }
    }

}
