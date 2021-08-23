﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        public void Bind(DataSourceBase ds, string layoutID, string startUpParameters, string startUpLayoutName, DGV settings)
        {
            LayoutId = layoutID;
            StartUpParameters = startUpParameters;
            if (!string.IsNullOrEmpty(startUpLayoutName))
                _lastAppliedLayoutName = startUpLayoutName;

            DGCore.Misc.DependentObjectManager.Bind(ds, this); // Register object    

            // _itemType = ds.ItemType;
            var listType = typeof(DGVList<>).MakeGenericType(ds.ItemType);
            // var dataSource = (IDGVList)Activator.CreateInstance(listType, ds, (Func<DGCore.Utils.DGVColumnHelper[]>)GetColumnHelpers);
            var dataSource = (IDGVList)Activator.CreateInstance(listType, ds, null);
            Data = dataSource;
            DGControl.ItemsSource = (IEnumerable)Data;

            Unwire();
            Wire();

            Helpers.DataGridHelper.GenerateColumns(this);

            // VisibleColumns = Helpers.DataGridHelper.GetColumnsInDisplayOrder(DGControl, true);

            if (settings != null)
                ((IUserSettingSupport<DGV>)this).SetSetting(settings);
            else
                UserSettingsUtils.Init(this, startUpLayoutName);

            Task.Factory.StartNew(() =>
            {
                Data.UnderlyingData.GetData(false);
            });
        }

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

        #region =====================
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
                        SetEnabled(false);
                        StatusLoadingRows = 0;
                        DataLoadedTime = null;
                        _loadDataTimer = new Stopwatch();
                        _loadDataTimer.Start();

                        break;
                    case DataSourceBase.DataEventKind.Loading:
                        StatusLoadingRows = e.RecordCount;
                        break;
                    case DataSourceBase.DataEventKind.Loaded:
                        _loadDataTimer.Stop();
                        DataLoadedTime = Convert.ToInt32(_loadDataTimer.ElapsedMilliseconds);
                        SetEnabled(true);

                        Data.RefreshData();
                        // DataSource_Loaded();
                        break;
                    case DataSourceBase.DataEventKind.BeforeRefresh:
                        SetEnabled(false);
                        // DataSource_BeforeRefresh();
                        // StatusLoadingRows = 0;
                        // OnPropertiesChanged(nameof(IsPartiallyLoaded));
                        break;
                    case DataSourceBase.DataEventKind.Refreshed:
                        // DataSource_AfterRefresh();
                        SetColumnVisibility();

                        // OnPropertiesChanged(nameof(IsPartiallyLoaded), nameof(StatusRowsLabel), nameof(DataLoadedTime));
                        SetEnabled(true);
                        break;
                }
                DataStatus = e.EventKind;
                // Clear DataLoadedTime
                // DataLoadedTime = null;
            }));

        }

        private void SetEnabled(bool isEnabled)
        {
            View.CommandBar.IsEnabled = isEnabled;
            DGControl.IsEnabled = isEnabled;
        }
        #endregion
    }
}
