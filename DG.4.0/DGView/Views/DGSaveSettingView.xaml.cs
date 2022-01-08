using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using DGCore.UserSettings;
using DGView.ViewModels;
using WpfSpLib.Common;
using WpfSpLib.Helpers;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for SaveSettingView.xaml
    /// </summary>
    public partial class DGSaveSettingView : UserControl
    {
        private DGViewModel _viewModel;
        // private readonly string _lastAppliedLayoutName;

        public DGSaveSettingView(DGViewModel viewModel, string lastAppliedLayoutName)
        {
            _viewModel = viewModel;
            // _lastAppliedLayoutName = lastAppliedLayoutName;
            InitializeComponent();
            DataContext = this;

            var oo = UserSettingsUtils.GetUserSettingDbObjects(viewModel);
            DataGrid.ItemsSource = oo;
            DataGrid.SelectedItem = DataGrid.Items.OfType<object>().FirstOrDefault();
            NewSettingName.Text = lastAppliedLayoutName;
            Dispatcher.BeginInvoke(new Action(() => { NewSettingName.Focus(); }), DispatcherPriority.Background);
            Unloaded += OnUnloaded;

            CmdDeleteRow = new RelayCommand(cmdDeleteRow);
            CmdSetSetting = new RelayCommand(cmdSetSetting);

        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.IsElementDisposing())
            {
                Unloaded -= OnUnloaded;
                _viewModel = null;
            }
        }

        private void DataGrid_OnSorting(object sender, DataGridSortingEventArgs e)
        {
            if (e.Column.SortDirection == ListSortDirection.Descending)
            {
                var dg = (DataGrid) sender;
                var view = CollectionViewSource.GetDefaultView(dg.ItemsSource);
                var sd = view.SortDescriptions.OfType<SortDescription>().FirstOrDefault(d => d.PropertyName == e.Column.SortMemberPath);
                view.SortDescriptions.Remove(sd);
                e.Column.SortDirection = null;
                e.Handled = true;
            }
        }

        #region =========  Commands  ===========
        public RelayCommand CmdDeleteRow { get; }
        public RelayCommand CmdSetSetting { get; }
        private void cmdDeleteRow(object p)
        {
            if (DataGrid.SelectedItems.Count == 1)
            {
                var item = (UserSettingsDbObject)DataGrid.SelectedItems[0];
                item.IsDeleted = !item.IsDeleted;
                item.OnPropertiesChanged("IsDeleted");
            }
        }
        private void cmdSetSetting(object p)
        {
            if (DataGrid.SelectedItems.Count == 1)
            {
                var item = (UserSettingsDbObject)DataGrid.SelectedItems[0];
                _viewModel.cmdSetSetting(item.SettingId);
            }
            ApplicationCommands.Close.Execute(null, null);
        }
        #endregion
    }
}
