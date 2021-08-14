using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DGCore.DGVList;
using DGCore.Sql;
using DGCore.UserSettings;
using DGView.Views;

namespace DGView.ViewModels
{
    public class DataGridViewModel : DependencyObject, INotifyPropertyChanged, IComponent, IUserSettingSupport<DGV>
    {
        public const bool AUTOGENERATE_COLUMNS = true;

        private readonly DataGridView _view;
        public DataGrid DGControl => _view.DataGrid;
        public string StartUpParameters { get; private set; }
        public DataSourceBase DataSource { get; private set; }

        public DataGridViewModel(DataGridView view)
        {
            _view = view;
        }
        public void Bind(DataSourceBase ds, string layoutID, string startUpParameters, string startUpLayoutName, DGV settings)
        {
            DataSource = ds;
            StartUpParameters = startUpParameters;

            // Load data
            _view.CommandBar.IsEnabled = false;

            var listType = typeof(DGVList<>).MakeGenericType(DataSource.ItemType);
            var dataSource = (IDGVList)Activator.CreateInstance(listType, DataSource, null);
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
                    _view.DataGrid.ItemsSource = (IEnumerable)dataSource;
                    sw.Stop();
                    var d2 = sw.Elapsed.TotalMilliseconds;
                    Debug.Print($"Load data time: {d1}");
                    Debug.Print($"Get data time: {d2}");
                    if (!DataGridViewModel.AUTOGENERATE_COLUMNS)
                        _view.CreateColumnsRecursive(DataSource.ItemType, new List<string>(), 0);
                    _view.CommandBar.IsEnabled = true;
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
        public IDGVList Data;
        public void Dispose()
        {
            if (_disposing)
                return;

            _disposing = true;
            DataSource.Dispose();
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

    }
}
