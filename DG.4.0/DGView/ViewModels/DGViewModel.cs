﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using DGCore.DGVList;
using DGCore.Sql;
using DGCore.UserSettings;
using DGView.Views;
using WpfSpLib.Common;
using WpfSpLib.Controls;

namespace DGView.ViewModels
{
    public partial class DGViewModel: Component, INotifyPropertyChanged, IUserSettingSupport<DGV>
    {
        #region ==========  Static section  ===================
        public static DataGridView CreateDataGrid(MwiContainer host, string title)
        {
            var dgView = new DataGridView();
            var child = new MwiChild
            {
                Title = title,
                Content = dgView,
                Height = Math.Max(200.0, Window.GetWindow(host).ActualHeight * 2 / 3),
                MaxWidth = Math.Max(200.0, Window.GetWindow(host).ActualWidth * 2 / 3)
            };
            var timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 3) };
            timer.Tick += OnDispatcherTimerTick;
            timer.Start();

            var b = new Binding { Path = new PropertyPath("ActualThemeColor"), Source = host, Converter = ColorHslBrush.Instance, ConverterParameter = "+45%:+0%:+0%" };
            child.SetBinding(MwiChild.ThemeColorProperty, b);

            host.Children.Add(child);

            void OnDispatcherTimerTick(object sender, EventArgs e)
            {
                var timer2 = (DispatcherTimer)sender;
                timer2.Stop();
                timer2.Tick -= OnDispatcherTimerTick;
                child.MaxWidth = 3000;
            }

            return dgView;
        }

        #endregion
        public DataGrid DGControl { get; }
        public IDGVList Data { get; private set; }
        public PropertyDescriptorCollection Properties => Data.Properties;

        public DGViewModel(DataGrid dataGrid)
        {
            DGControl = dataGrid;
            InitCommands();
        }

        public void Bind(DataSourceBase ds, string layoutID, string startUpParameters, string startUpLayoutName, DGV settings)
        {
            LayoutId = layoutID;
            DGCore.Misc.DependentObjectManager.Bind(ds, this);

            Task.Factory.StartNew(() =>
            {
                StartUpParameters = startUpParameters;
                if (!string.IsNullOrEmpty(startUpLayoutName))
                    LastAppliedLayoutName = startUpLayoutName;

                // Not need! DGCore.Misc.DependentObjectManager.Bind(ds, this); // Register object    
                var listType = typeof(DGVList<>).MakeGenericType(ds.ItemType);
                var dataSource = (IDGVList)Activator.CreateInstance(listType, ds, (Func<DGCore.Utils.IDGColumnHelper[]>)GetColumnHelpers);
                Data = dataSource;

                Unwire();
                Wire();

                Task.Factory.StartNew(() => Data.UnderlyingData.GetData(false));

                DGControl.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    DGControl.ItemsSource = (IEnumerable)Data;
                    Helpers.DataGridHelper.GenerateColumns(this);
                    if (settings != null)
                        ((IUserSettingSupport<DGV>) this).SetSetting(settings);
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
        protected override void Dispose(bool disposing)
        {
            Unwire();
            Data.UnderlyingData.DataLoadingCancelFlag = true;
            _dataRecordsTimer.Stop();
            Data.Dispose(); // DGVList

            base.Dispose(disposing);

            Data = null;
        }
        #endregion

        #region =========  DataStateChanged  ============
        private void Wire()
        {
          Data.DataStateChanged += DataSource_DataStateChanged;
          _dataRecordsTimer.Tick += OnDataRecordsTimerTick;
        }

        private void Unwire()
        {
            if (Data != null)
                Data.DataStateChanged -= DataSource_DataStateChanged;
            _dataRecordsTimer.Tick -= OnDataRecordsTimerTick;
        }

        private Stopwatch _dataLoadedTimer;
        private readonly DispatcherTimer _dataRecordsTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(250)};
        private int? _dataLoadedTime;

        private void DataSource_DataStateChanged(object sender, DataSourceBase.SqlDataEventArgs e)
        {
            // Execute in main thread
            DGControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                switch (e.EventKind)
                {
                    case DataSourceBase.DataEventKind.Clear:
                        _dataLoadedTime = null;
                        _dataLoadedTimer = new Stopwatch();
                        _dataLoadedTimer.Start();
                        _dataRecordsTimer.Start();
                        break;
                    case DataSourceBase.DataEventKind.Loading:
                        break;
                    case DataSourceBase.DataEventKind.Loaded:
                        _dataLoadedTimer.Stop();
                        _dataRecordsTimer.Stop();
                        _dataLoadedTime = Convert.ToInt32(_dataLoadedTimer.ElapsedMilliseconds);
                        Data.RefreshData();
                        break;
                    case DataSourceBase.DataEventKind.BeforeRefresh:
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
                    _dataLoadedTime = null;
            }));

        }
        #endregion
        private void OnDataRecordsTimerTick(object sender, EventArgs e)
        {
          OnPropertiesChanged(nameof(StatusTextOfLeftLabel));
        }
    }
}
