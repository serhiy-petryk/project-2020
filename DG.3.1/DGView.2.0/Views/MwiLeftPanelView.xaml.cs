using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using DGCore.Menu;
using DGCore.UserSettings;
using DGView.ViewModels;
using WpfSpLib.Common;
using WpfSpLib.Controls;
using WpfSpLib.Helpers;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for MwiLeftPanelView.xaml
    /// </summary>
    public partial class MwiLeftPanelView : UserControl, INotifyPropertyChanged
    {
        private MwiContainer Host => MwiContainer.GetMwiContainer(this);

        public DGCore.Misc.DataDefinition DataDefinition { get; private set; }
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
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    // BringIntoView for TreeViewItem
                    var scrollViewer = Tips.GetVisualParents(item).OfType<ScrollViewer>().FirstOrDefault();
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
                }), DispatcherPriority.ContextIdle);
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
            var menuItems = Tips.GetVisualChildren(MenuTreeView).OfType<TextBlock>().Where(item => item.DataContext == menuOption);
            foreach (var item in menuItems.Where(i => i.TextDecorations.Contains(TextDecorations.Strikethrough[0])))
                item.TextDecorations = new TextDecorationCollection();

            var icons = Tips.GetVisualChildren(MenuTreeView).OfType<Viewbox>().Where(item => item.DataContext == menuOption).ToArray();
            icons[0].Visibility = Visibility.Collapsed;
            icons[1].Visibility = Visibility.Visible;

            CbDataSettingName.SelectedIndex = -1;

            Task.Factory.StartNew(() => {
                try
                {
                    // Check on database error
                    DataDefinition = menuOption?.GetDataDefiniton();
                    // Thread.Sleep(2000);
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (DataDefinition != null)
                        {
                            var userSettingProperties = new FakeUserSettingProperties
                            {
                                SettingKind = DGViewModel.UserSettingsKind,
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
                            icons[0].Visibility = Visibility.Visible;
                            icons[1].Visibility = Visibility.Collapsed;
                        }
                    }));
                }
                catch (Exception ex)
                {
                    foreach (var item in menuItems)
                        item.TextDecorations = TextDecorations.Strikethrough;
                    DataDefinition = null;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        icons[0].Visibility = Visibility.Visible;
                        icons[1].Visibility = Visibility.Collapsed;
                    }));
                    DialogMessage.ShowDialog(ex.ToString(), "Помилка", DialogMessage.DialogMessageIcon.Error);
                }
            });
        }

        private void ActionProcedure()
        {
            var mo = MenuTreeView.SelectedItem as MenuOption;
            var dataDefinition = mo?.GetDataDefiniton();
            if (dataDefinition == null)
                return;

            var parameters = dataDefinition.DbParameters;
            var startUpParameters = parameters == null || parameters._parameters.Count == 0
                ? dataDefinition.WhereFilter.StringPresentation
                : dataDefinition.DbParameters.GetStringPresentation();

            var dgView = new DataGridView();
            var child = new MwiChild
            {
                Title = mo.Label,
                Content = dgView,
                Height = Math.Max(200.0, Window.GetWindow(Host).ActualHeight * 2 / 3),
                MaxWidth = Math.Max(200.0, Window.GetWindow(Host).ActualWidth * 2 / 3)
            };
            var timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            timer.Tick += OnDispatcherTimerTick;
            timer.Start();

            var b = new Binding { Path = new PropertyPath("ActualThemeColor"), Source = Host, Converter = ColorHslBrush.Instance, ConverterParameter="+45%:+0%:+0%" };
            child.SetBinding(MwiChild.ThemeColorProperty, b);

            Host.Children.Add(child);
            dgView.ViewModel.Bind(dataDefinition.GetDataSource(dgView.ViewModel), dataDefinition.SettingID, startUpParameters, (string) CbDataSettingName.SelectedValue, null);

            Host.HideLeftPanel();

            void OnDispatcherTimerTick(object sender, EventArgs e)
            {
                var timer2 = (DispatcherTimer)sender;
                timer2.Stop();
                timer2.Tick -= OnDispatcherTimerTick;
                child.MaxWidth = 3000;
            }
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
        }
        #endregion

        private void LoadDataFromProcedure_OnClick(object sender, RoutedEventArgs e)
        {
            ActionProcedure();
        }
    }

}
