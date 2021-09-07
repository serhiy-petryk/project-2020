﻿using System;
using System.Collections;
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
    public partial class DataGridViewModel : DependencyObject, INotifyPropertyChanged, IComponent, IUserSettingSupport<DGV>
    {
        public DataGridView View { get; }
        public DataGrid DGControl => View.DataGrid;
        public IDGVList Data { get; private set; }
        public PropertyDescriptorCollection Properties => Data.Properties;

        // private Type _itemType;

        public DataGridViewModel(DataGridView view)
        {
            View = view;
            InitCommands();
        }

        public void Bind(DGCore.Misc.DataDefinition dd, string startUpParameters, string startUpLayoutName, DGV settings)
        {
            Task.Factory.StartNew(() =>
            {
                LayoutId = dd.SettingID;
                StartUpParameters = startUpParameters;
                if (!string.IsNullOrEmpty(startUpLayoutName))
                    LastAppliedLayoutName = startUpLayoutName;

                var ds = dd.GetDataSource(this);
                // Not need! DGCore.Misc.DependentObjectManager.Bind(ds, this); // Register object    

                // _itemType = ds.ItemType;
                var listType = typeof(DGVList<>).MakeGenericType(ds.ItemType);
                var dataSource = (IDGVList)Activator.CreateInstance(listType, ds, (Func<DGCore.Utils.IDGColumnHelper[]>)GetColumnHelpers);
                Data = dataSource;

                Unwire();
                Wire();

                Task.Factory.StartNew(() => Data.UnderlyingData.GetData(false));

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    DGControl.ItemsSource = (IEnumerable)Data;
                    Helpers.DataGridHelper.GenerateColumns(this);
                    if (settings != null)
                        ((IUserSettingSupport<DGV>)this).SetSetting(settings);
                    else
                        UserSettingsUtils.Init(this, startUpLayoutName);
                }));
            });
        }

        #region ===========  INotifyPropertyChanged  ==============
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertiesChanged(params string[] propertyNames)
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

        #region =========  DataStateChanged  ============
        private void Wire() => Data.DataStateChanged += DataSource_DataStateChanged;

        private void Unwire()
        {
            if (Data != null)
                Data.DataStateChanged -= DataSource_DataStateChanged;
        }

        private Stopwatch _loadDataTimer;
        private void DataSource_DataStateChanged(object sender, DataSourceBase.SqlDataEventArgs e)
        {
            // Execute in main thread
            DGControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                switch (e.EventKind)
                {
                    case DataSourceBase.DataEventKind.Clear:
                        DataLoadingRows = 0;
                        DataLoadedTime = null;
                        _loadDataTimer = new Stopwatch();
                        _loadDataTimer.Start();
                        break;
                    case DataSourceBase.DataEventKind.Loading:
                        DataLoadingRows = e.RecordCount;
                        break;
                    case DataSourceBase.DataEventKind.Loaded:
                        _loadDataTimer.Stop();
                        DataLoadedTime = Convert.ToInt32(_loadDataTimer.ElapsedMilliseconds);
                        Data.RefreshData();
                        break;
                    case DataSourceBase.DataEventKind.BeforeRefresh:
                        RestoreColumnLayout(GetSettings());
                        break;
                    case DataSourceBase.DataEventKind.Refreshed:
                        // Restore last active cell
                        if (_lastCurrentCellInfo.IsValid)
                        {
                            var index = DGControl.Items.IndexOf(_lastCurrentCellInfo.Item);
                            if (index >= 0)
                            {
                                DGControl.Focus();
                                DGControl.SelectedCells.Add(new DataGridCellInfo(_lastCurrentCellInfo.Item, _lastCurrentCellInfo.Column));
                                DGControl.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    DGControl.ScrollIntoView(DGControl.SelectedCells[0].Item);
                                }), DispatcherPriority.Loaded);
                            }
                            _lastCurrentCellInfo = new DataGridCellInfo();
                        }
                        break;
                }
                DataStatus = e.EventKind;

                // Clear DataLoadedTime
                if (DataStatus == DataSourceBase.DataEventKind.Refreshed)
                    DataLoadedTime = null;
            }));

        }
        #endregion
    }
}
