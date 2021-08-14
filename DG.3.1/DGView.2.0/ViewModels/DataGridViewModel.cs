using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DGCore.DGVList;
using DGCore.Sql;
using DGCore.UserSettings;
using DGView.Helpers;
using DGView.Views;

namespace DGView.ViewModels
{
    public class DataGridViewModel : DependencyObject, INotifyPropertyChanged, IComponent, IUserSettingSupport<DGV>
    {
        public const bool AUTOGENERATE_COLUMNS = false;

        private readonly DataGridView DGView;
        public DataGrid DGControl => DGView.DataGrid;
        private string LayoutId { get; set; }
        private string StartUpParameters { get; set; }
        // private DataSourceBase DataSource { get; set; }
        // public new DGCore.DGVList.IDGVList DataSource => base.DataSource == null ? null : (DGCore.DGVList.IDGVList)base.DataSource;
        // public IDGVList DataSource { get; set; }
        public IDGVList Data;

        public DataGridViewModel(DataGridView view)
        {
            DGView = view;
        }
        public void Bind(DataSourceBase ds, string layoutID, string startUpParameters, string startUpLayoutName, DGV settings)
        {
            // DataSource = ds;
            LayoutId = layoutID;
            StartUpParameters = startUpParameters;

            // Load data
            DGView.CommandBar.IsEnabled = false;
            Unwire();

            DGCore.Misc.DependentObjectManager.Bind(ds, this); // Register object    

            var listType = typeof(DGVList<>).MakeGenericType(ds.ItemType);
            // var dataSource = (IDGVList)Activator.CreateInstance(listType, ds, (Func<DGCore.Utils.DGVColumnHelper[]>)GetColumnHelpers);
            var dataSource = (IDGVList)Activator.CreateInstance(listType, ds, null);
            Data = dataSource;

            //DGView.DataGrid.ItemsSource = (IEnumerable)dataSource;
            DGView.DataGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
               // var aa1 = DGView.DataGrid.ItemsSource = (IEnumerable)dataSource;

            }), DispatcherPriority.ApplicationIdle);
            //return;

            Wire();

            // AdjustCheckBoxColumns();
            var properties = Data.Properties;
            foreach (var col in DGView.DataGrid.Columns)
            {
            }
            if (!DataGridViewModel.AUTOGENERATE_COLUMNS)
                DataGridHelper.CreateColumnsRecursive(DGView, ds.ItemType, new List<string>(), 0);

            Task.Factory.StartNew(() =>
            {
                var sw = new Stopwatch();
                sw.Start();

                // var dataSource = Activator.CreateInstance(listType, ds, (Func<Utils.DGVColumnHelper[]>)GetColumnHelpers);
                //DGView.DataGrid.ItemsSource = (IEnumerable)dataSource;
                dataSource.RefreshData();

                sw.Stop();
                var d1 = sw.Elapsed.TotalMilliseconds;
                sw.Reset();
                sw.Start();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    DGView.DataGrid.ItemsSource = (IEnumerable)dataSource;
                    sw.Stop();
                    var d2 = sw.Elapsed.TotalMilliseconds;
                    Debug.Print($"Load data time: {d1}");
                    Debug.Print($"Get data time: {d2}");
                    //if (!DataGridViewModel.AUTOGENERATE_COLUMNS)
                      //  DGView.CreateColumnsRecursive(ds.ItemType, new List<string>(), 0);
                    DGView.CommandBar.IsEnabled = true;
                }), DispatcherPriority.Normal);
            });

        }

        public void BindOld(DataSourceBase ds, string layoutID, string startUpParameters, string startUpLayoutName, DGV settings)
        {
            // DataSource = ds;
            LayoutId = layoutID;
            StartUpParameters = startUpParameters;

            // Load data
            DGView.CommandBar.IsEnabled = false;

            var listType = typeof(DGVList<>).MakeGenericType(ds.ItemType);
            var dataSource = (IDGVList)Activator.CreateInstance(listType, ds, null);
            Data = dataSource;

            Task.Factory.StartNew(() =>
            {
                var sw = new Stopwatch();
                sw.Start();

                // var dataSource = Activator.CreateInstance(listType, ds, (Func<Utils.DGVColumnHelper[]>)GetColumnHelpers);
                dataSource.RefreshData();

                sw.Stop();
                var d1 = sw.Elapsed.TotalMilliseconds;
                sw.Reset();
                sw.Start();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    DGView.DataGrid.ItemsSource = (IEnumerable)dataSource;
                    sw.Stop();
                    var d2 = sw.Elapsed.TotalMilliseconds;
                    Debug.Print($"Load data time: {d1}");
                    Debug.Print($"Get data time: {d2}");
                    //if (!DataGridViewModel.AUTOGENERATE_COLUMNS)
                      //  DGView.CreateColumnsRecursive(ds.ItemType, new List<string>(), 0);
                    DGView.CommandBar.IsEnabled = true;
                }), DispatcherPriority.Normal);
            });

        }

        #region ===========  Commands  ==============
        public void SetQuickTextFilter(string filterText)
        {
            Data.A_FastFilterChanged(filterText);
        }
        #endregion

        #region ===========  INotifyPropertyChanged  ==============
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region ===========  IComponent  ==============
        private bool _disposing = false;
        public void Dispose()
        {
            if (_disposing)
                return;

            _disposing = true;
            Unwire();
            // DataSource.Dispose();
            Data?.Dispose();
            Disposed?.Invoke(this, new EventArgs());
            Data = null;
        }

        public ISite Site { get; set; }
        public event EventHandler Disposed;
        #endregion

        #region ========   IUserSettingSupport<DGV>  ========
        public string _layoutID;
        internal const string UserSettingsKind = "DGV_Setting";

        public string SettingKind => UserSettingsKind;
        public string SettingKey => _layoutID;
        public DGV GetSettings()
        {
            var o = new DGV
            {
                /*WhereFilter = ((IUserSettingSupport<List<Filter>>)DataSource.WhereFilter)?.GetSettings(),
                FilterByValue = ((IUserSettingSupport<List<Filter>>)DataSource.FilterByValue)?.GetSettings(),
                ShowTotalRow = DataSource.ShowTotalRow,
                ExpandedGroupLevel = DataSource.ExpandedGroupLevel,
                ShowGroupsOfUpperLevels = DataSource.ShowGroupsOfUpperLevels,
                BaseFont = this.Font,
                IsGridVisible = this._IsGridVisible,
                CellViewMode = this._CellViewMode,
                TextFastFilter = DataSource.TextFastFilter*/
            };
            // ApplyColumnLayout(o);
            return o;
        }

        public DGV GetBlankSetting()
        {
            // Utils.Dgv.EndEdit(this);
            /*DataSource.ResetSettings();
            Font = _startupFont;
            CellBorderStyle = DataGridViewCellBorderStyle.Single; // For _IsGridVisible
            _CellViewMode = Enums.DGCellViewMode.OneRow;

            // For AllColumns
            _allValidColumnNames = Columns.Cast<DataGridViewColumn>()
                .Where(col => !string.IsNullOrEmpty(col.DataPropertyName) && !col.DataPropertyName.Contains('.'))
                .Select(col => col.DataPropertyName).ToList();

            ResizeColumnWidth(); // !!! Before SaveColumnInfo*/
            return ((IUserSettingSupport<DGV>)this).GetSettings();
        }

        public void SetSetting(DGV settings)
        {
            // throw new NotImplementedException();
        }
        #endregion

        #region =====================
        private void Wire() => Data.DataStateChanged += DataSource_DataStateChanged;

        private void Unwire()
        {
            if (Data != null)
                Data.DataStateChanged -= DataSource_DataStateChanged;
        }
        private void DataSource_DataStateChanged(object sender, DataSourceBase.SqlDataEventArgs e)
        {
        }

        #endregion
    }
}
