using System;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DGCore.Common;
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
        public Type ItemType;
        private string StartUpParameters { get; set; }
        private string _lastAppliedLayoutName { get; set; }
        // private DataSourceBase DataSource { get; set; }
        // public new DGCore.DGVList.IDGVList DataSource => base.DataSource == null ? null : (DGCore.DGVList.IDGVList)base.DataSource;
        // public IDGVList DataSource { get; set; }
        public IDGVList Data;

        private Enums.DGCellViewMode _cellViewMode = Enums.DGCellViewMode.OneRow;
        public Enums.DGCellViewMode CellViewMode
        {
            get => _cellViewMode;
            set
            {
                _cellViewMode = value;
                SetCellElementStyleAndWidth();
                OnPropertiesChanged(nameof(CellViewMode), nameof(CellViewModeLabel));
            }
        }
        public string CellViewModeLabel => _cellViewMode.ToString();

        private DataGridColumn[] VisibleColumns;


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

            ItemType = ds.ItemType;
            var listType = typeof(DGVList<>).MakeGenericType(ds.ItemType);
            // var dataSource = (IDGVList)Activator.CreateInstance(listType, ds, (Func<DGCore.Utils.DGVColumnHelper[]>)GetColumnHelpers);
            var dataSource = (IDGVList)Activator.CreateInstance(listType, ds, null);
            Data = dataSource;
            DGControl.ItemsSource = (IEnumerable)Data;

            Unwire();
            Wire();
            SetEnabled(false);

            Helpers.DataGridHelper.GenerateColumns(this);

            VisibleColumns = Helpers.DataGridHelper.GetColumnsInDisplayOrder(DGControl, true);

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
        private void DataSource_DataStateChanged(object sender, DataSourceBase.SqlDataEventArgs e)
        {
            switch (e.EventKind)
            {
                case DataSourceBase.DataEventKind.Loaded:
                    // Run in WPF thread
                    DGControl.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        SetEnabled(true);
                        Data.RefreshData();
                    }));
                    // DataSource_Loaded();
                    break;
                case DataSourceBase.DataEventKind.BeforeRefresh:
                    // DataSource_BeforeRefresh();
                    break;
                case DataSourceBase.DataEventKind.Refreshed:
                    // DataSource_AfterRefresh();
                    SetColumnVisibility();
                    var totalRows = Data.UnderlyingData.GetData(false).Count;
                    var dgvRows = Data.FilteredRowCount;
                    var prefix = "";
                    if (Data.UnderlyingData.IsPartiallyLoaded)
                        prefix = "Дані завантажені частково. ";
                    View.lblRecords.Text = prefix + "Елементів: " + (totalRows == dgvRows ? "" : totalRows.ToString("N0") + " / ") + dgvRows.ToString("N0");

                    break;
            }

        }

        private void SetEnabled(bool isEnabled)
        {
            View.CommandBar.IsEnabled = isEnabled;
            DGControl.IsEnabled = isEnabled;
        }
        #endregion
    }
}
