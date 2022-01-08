using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using DGCore.Common;
using DGCore.UserSettings;
using DGView.Helpers;
using DGView.ViewModels;
using WpfSpLib.Common;
using WpfSpLib.Controls;
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

            CmdDeleteRow = new RelayCommand(cmdDeleteRow, o => DataGrid.SelectedItems.Count == 1);
            CmdSaveChanges = new RelayCommand(cmdSaveChanges, o => ((List<UserSettingsDbObject>)DataGrid.ItemsSource).Any(o1 => o1.IsDeleted));
            CmdSaveNewSetting = new RelayCommand(cmdSaveNewSetting, o => !string.IsNullOrEmpty(NewSettingName.Text));
            CmdSetSetting = new RelayCommand(cmdSetSetting, o => DataGrid.SelectedItems.Count == 1);
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
        public RelayCommand CmdSaveChanges { get; }
        public RelayCommand CmdSaveNewSetting { get; }
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
        private void cmdSaveChanges(object p)
        {
            /*var result = DGCore.UserSettings.UserSettingsUtils.SaveChangedSettings((List<DGCore.UserSettings.UserSettingsDbObject>)dataGridView1.DataSource, _properties);
            if (result == 0)
            {
                MessageBox.Show(@"Не було змінено жодного налаштування", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (result > 0)
            {
                MessageBox.Show($@"Було записано {result} змінених налаштуваннь", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Flag = true;
                var cm = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                var data = dataGridView1.DataSource;
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = data;
            }
            else
            {
                MessageBox.Show(@"Помилка! Було спроба записати налаштування, для якого Ви не маєте права це робити", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void cmdSaveNewSetting(object p)
        {
            var settingId = NewSettingName.Text;
            var allowEdit = cbAllowEditToOthers.IsChecked ?? false;
            var allowView = cbAllowViewToOthers.IsChecked ?? false;

            if (string.IsNullOrEmpty(settingId))
            {
                Shared.ShowMessage(@"Налаштування не може бути пустим", "", Enums.MessageBoxButtons.OK, Enums.MessageBoxIcon.Error);
                return;
            }

            if (UserSettingsUtils.SaveNewSetting(_viewModel, settingId, allowView, allowEdit))
            {
                _viewModel.cmdSetSetting(settingId);
                ApplicationCommands.Close.Execute(null, null);
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
