﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DGCore.Common;
using DGCore.Menu;
using DGCore.UserSettings;
using DGView.Temp;
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
        public bool IsCbDataSettingEnabled => CbDataSettingName.ItemsSource is IList list && list.Count > 0;
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
                RootMenu rootMenu;
                try
                {
                    // Load menu items
                    rootMenu = new RootMenu();
                }
                catch (Exception ex)
                {
                    var sb = new StringBuilder();
                    if (ex is LoadJsonConfigException loadException)
                    {
                        sb.AppendLine($@"Помилка у файлі конфігурації: {loadException.FileName}");
                        if (loadException.LineNumber.HasValue)
                            sb.AppendLine($"Рядок файлу: {loadException.LineNumber}. Позиція: {loadException.Position}.");
                        sb.AppendLine(null);
                        sb.AppendLine($@"Текст помилки:");
                        sb.AppendLine(loadException.Message);
                    }
                    else
                        sb.AppendLine(ex.Message);

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        new DialogBox(DialogBox.DialogBoxKind.Error)
                        {
                            Host = Host,
                            Caption = "Помилка",
                            Message = sb.ToString()
                        }.ShowDialog();
                        Application.Current.Shutdown();
                    }), DispatcherPriority.ApplicationIdle);
                    return;
                }

                MenuTreeView.ItemsSource = rootMenu.Items;
                // Set application header
                if (!string.IsNullOrEmpty(rootMenu.ApplicationTitle) && Window.GetWindow(this) is MwiStartup app)
                    app.Title = rootMenu.ApplicationTitle;

                var mwiContainer = this.GetVisualParents().OfType<MwiContainer>().FirstOrDefault();
                var resizeThumb = mwiContainer?.GetVisualChildren().OfType<FrameworkElement>().FirstOrDefault(a => a.Name == "LeftPanelDragThumb");
                if (resizeThumb != null)
                {
                    var b = new Binding { Path = new PropertyPath("Background"), Source = this, Converter = ColorHslBrush.Instance, ConverterParameter = "+5%" };
                    resizeThumb.SetBinding(BackgroundProperty, b);
                    resizeThumb.Opacity = 1.0;
                }
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
                            try
                            {
                                Helpers.DoEventsHelper.DoEvents();
                                var userSettingProperties = new FakeUserSettingProperties
                                {
                                    SettingKind = DGViewModel.UserSettingsKind,
                                    SettingKey = DataDefinition.SettingID
                                };

                                if (!string.IsNullOrEmpty(DataDefinition?.SettingID))
                                {
                                    var settingKeys = UserSettingsUtils.GetKeysFromDb(userSettingProperties);
                                    CbDataSettingName.ItemsSource = settingKeys;
                                    if (settingKeys.Count > 0)
                                        CbDataSettingName.Width = settingKeys.Max(k =>
                                                                      ControlHelper.MeasureString(k, CbDataSettingName,
                                                                          TextFormattingMode.Display).Width) + 10.0;
                                }

                                var parameters = DataDefinition.DbParameters;
                                if (parameters == null || parameters._parameters.Count == 0)
                                {
                                    DbProcedureParameterArea.Visibility = Visibility.Collapsed;
                                    FilterArea.Visibility = DataDefinition.WhereFilter == null
                                        ? Visibility.Collapsed
                                        : Visibility.Visible;
                                    ErrorText = null;
                                    if (DataDefinition.WhereFilter != null)
                                        DbFilterView.Bind(DataDefinition.WhereFilter, DataDefinition.SettingID,
                                            ActionProcedure, null);
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
                            catch (Exception ex)
                            {
                                ShowError(ex);
                            }
                        }
                    }));
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }

                void ShowError(Exception ex)
                {
                    foreach (var item in menuItems)
                        item.TextDecorations = TextDecorations.Strikethrough;
                    DataDefinition = null;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        icons[0].Visibility = Visibility.Visible;
                        icons[1].Visibility = Visibility.Collapsed;
                    }));

                    new DialogBox(DialogBox.DialogBoxKind.Error)
                    {
                        Host = Host,
                        Caption = "Помилка",
                        Message = ex.Message,
                        Buttons = new[] { "OK" },
                        Details = ex.ToString()
                    }.ShowDialog();
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

            var dgView = DGViewModel.CreateDataGrid(Host, mo.Label);
            dgView.ViewModel.Bind(dataDefinition.GetDataSource(dgView.ViewModel), dataDefinition.SettingID, startUpParameters, (string) CbDataSettingName.SelectedValue, null);

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
            OnPropertiesChanged(nameof(DataDefinition), nameof(IsCbDataSettingEnabled), nameof(ErrorText));
        }
        #endregion

        private void LoadDataFromProcedure_OnClick(object sender, RoutedEventArgs e)
        {
            ActionProcedure();
        }

        private void OnTreeViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var grid = (Grid)sender;
            var topGrid = (Grid)grid.Parent;
            var leftPanel = this.GetVisualParents().OfType<FrameworkElement>().FirstOrDefault(a => a.Name == "LeftPanelContainer");
            leftPanel?.SetCurrentValueSmart(MinWidthProperty, grid.ActualWidth + topGrid.ColumnDefinitions[2].MinWidth + 14);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Memory: {DGCore.Utils.Tips.MemoryUsedInBytes:N0} байт");
        }

        private void OnPrintClick(object sender, RoutedEventArgs e)
        {
            new PrintPreviewWindow(new PrintContentGeneratorSample()) {Owner = Window.GetWindow(this)}.ShowDialog();
        }
    }

}
