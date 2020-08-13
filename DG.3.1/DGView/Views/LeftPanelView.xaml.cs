using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DGCore.Menu;
using DGCore.UserSettings;
using DGView.ViewModels;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for LeftPanelView.xaml
    /// </summary>
    public partial class LeftPanelView: UserControl, INotifyPropertyChanged
    {
        public DGCore.Misc.DataDefiniton DataDefinition { get; private set; }
        public List<string> DbSettingNames { get; private set; }
        public DGCore.Filters.FilterList DbWhereFilter { get; private set; }

        private MenuOption _menuOption;

        public LeftPanelView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void LeftPanelView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var rootMenu = new RootMenu(DGCore.Misc.AppSettings.CONFIG_FILE_NAME);
                MenuTreeView.ItemsSource = rootMenu.Items;
            }
        }

        private void TreeViewItem_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (TreeViewItem)sender;
            _menuOption = item.DataContext as MenuOption;
            if (_menuOption == null)
            { // Submenu
                item.IsExpanded = !item.IsExpanded;
                OnPropertiesChanged(new[] { nameof(DataDefinition), nameof(DbSettingNames), nameof(DbWhereFilter) });
            }
            e.Handled = true;
        }
        private void Menu_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _menuOption = e.NewValue as MenuOption;
            if (_menuOption != null)
                ActivateMenuOption(e);
        }


        private void TreeViewItem_OnExpanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
            if (item.Items.Count > 0)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
                {
                    // BringIntoView for TreeViewItem
                    if (Common.Tips.GetVisualParents(item).FirstOrDefault(a => a is ScrollViewer) is ScrollViewer scroller)
                    {
                        var upPoint = item.TransformToVisual(scroller).Transform(new Point(0, 0)).Y + scroller.VerticalOffset;
                        var downPoint = upPoint + item.ActualHeight;
                        if (scroller.ActualHeight + scroller.VerticalOffset < downPoint)
                        {
                            var newOffset = Math.Min(upPoint, downPoint - scroller.ActualHeight);
                            var sb = new Storyboard();
                            sb.Children.Add(Common.AnimationHelper.GetScrollViewerVerticalOffsetAnimation(scroller, scroller.VerticalOffset, newOffset));
                            sb.Begin();
                        }
                    }
                }));

                e.Handled = true;
            }
        }

        //============================================================
        //===========  INotifyPropertyChanged  =======================

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertiesChanged(string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void LoadData_OnClick(object sender, RoutedEventArgs e)
        {
            if (_menuOption != null)
            {
                var dgView = new DataGridView(_menuOption);
                AppViewModel.Instance.ContainerControl.HideLeftPanel();
            }
        }

        private void OnFilterEditPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var cell = (DataGridCell)sender;
            var filterLine = cell.DataContext as DGCore.Filters.FilterLineBase;
            var view = new FilterLineView(filterLine);
            var height = Math.Max(200, Window.GetWindow(this).ActualHeight * 2 / 3);
            Common.Tips.ShowMwiChildDialog(view, "Dialog", new Size(double.NaN, height));
        }

        private void OnDefinitionGridLoaded(object sender, RoutedEventArgs e)
        {
            // Set initial width of LeftPanel
            if (AppViewModel.Instance.ContainerControl != null) // check => to prevent MS designer error
                AppViewModel.Instance.ContainerControl.LeftPanelContainer.Width = Math.Min(800, SystemParameters.WorkArea.Width * 0.7);
        }

        private void ActivateMenuOption(RoutedEventArgs e)
        {
            try
            {
                // Check on database error
                DataDefinition = _menuOption.GetDataDefiniton();
                if (DataDefinition != null)
                {
                    var userSettingProperties = new FakeUserSettingProperties
                    {
                        SettingKind = DataGridView.UserSettingsKind,
                        SettingKey = DataDefinition.SettingID
                    };
                    DbSettingNames = UserSettingsUtils.GetKeysFromDb(userSettingProperties);
                    //this.cbDataSettingName.Items.Clear();
                    //this.cbDataSettingName.Items.AddRange(settingNames.ToArray());
                    DbWhereFilter = DataDefinition.WhereFilter;
                    // IgnoreCaseDgColumn.Visibility = DbWhereFilter.IgnoreCaseSupport ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                OnPropertiesChanged(new[] { nameof(DataDefinition), nameof(DbSettingNames), nameof(DbWhereFilter) });
                e.Handled = true;
            }
        }
    }

}
