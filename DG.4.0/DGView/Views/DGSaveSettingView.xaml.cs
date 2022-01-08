using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using DGCore.UserSettings;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for SaveSettingView.xaml
    /// </summary>
    public partial class DGSaveSettingView : UserControl
    {
        private readonly DGCore.UserSettings.IUserSettingProperties _properties;
        private readonly string _lastAppliedLayoutName;

        public DGSaveSettingView(DGCore.UserSettings.IUserSettingProperties o, string lastAppliedLayoutName)
        {
            _properties = o;
            _lastAppliedLayoutName = lastAppliedLayoutName;
            InitializeComponent();

            var oo = DGCore.UserSettings.UserSettingsUtils.GetUserSettingDbObjects(_properties);
            DataGrid.ItemsSource = oo;
            DataGrid.SelectedItem = DataGrid.Items.OfType<object>().FirstOrDefault();
            NewSettingName.Text = _lastAppliedLayoutName;
            Dispatcher.BeginInvoke(new Action(() => { NewSettingName.Focus(); }), DispatcherPriority.Background);
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

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItems.Count == 1)
            {
                var item = (UserSettingsDbObject)DataGrid.SelectedItems[0];
                item.IsDeleted = !item.IsDeleted;
                item.OnPropertiesChanged("IsDeleted");
            }
        }
    }
}
