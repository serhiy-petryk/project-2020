﻿using System;
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
        private List<UserSettingsDbObject> DataSource => (List<UserSettingsDbObject>) DataGrid.ItemsSource;

        public DGSaveSettingView(DGViewModel viewModel, string lastAppliedLayoutName)
        {
            InitializeComponent();
            DataContext = this;
            _viewModel = viewModel;

            var oo = UserSettingsUtils.GetUserSettingDbObjects(viewModel);
            DataGrid.ItemsSource = oo;
            DataGrid.SelectedItem = DataGrid.Items.OfType<object>().FirstOrDefault();
            NewSettingName.Text = lastAppliedLayoutName;
            Dispatcher.BeginInvoke(new Action(() => { NewSettingName.Focus(); }), DispatcherPriority.Background);
            Unloaded += OnUnloaded;

            CmdDeleteRow = new RelayCommand(cmdDeleteRow, o => DataGrid.SelectedItems.Count == 1);
            CmdSaveChanges = new RelayCommand(cmdSaveChanges, o => DataSource.Any(o1 => o1.IsDeleted));
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
            var result = UserSettingsUtils.SaveChangedSettings(DataSource, _viewModel);
            if (result == 0)
                Shared.ShowMessage("Не було змінено жодного налаштування", "", Enums.MessageBoxButtons.OK, Enums.MessageBoxIcon.Warning);
            else if (result <0)
                Shared.ShowMessage("Помилка! Було спроба записати налаштування, для якого Ви не маєте права це робити", "", Enums.MessageBoxButtons.OK, Enums.MessageBoxIcon.Error);
            else
            {
                Shared.ShowMessage($"Було записано {result} змінених налаштуваннь", "", Enums.MessageBoxButtons.OK, Enums.MessageBoxIcon.Information);
                var oo = UserSettingsUtils.GetUserSettingDbObjects(_viewModel);
                DataGrid.ItemsSource = oo;
                DataGrid.SelectedItem = DataGrid.Items.OfType<object>().FirstOrDefault();
            }
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
                ApplicationCommands.Close.Execute(null, this);
            }
        }

        private void cmdSetSetting(object p)
        {
            if (DataGrid.SelectedItems.Count == 1)
            {
                var item = (UserSettingsDbObject)DataGrid.SelectedItems[0];
                _viewModel.cmdSetSetting(item.SettingId);
            }
            ApplicationCommands.Close.Execute(null, this);
        }
        #endregion
    }
}
