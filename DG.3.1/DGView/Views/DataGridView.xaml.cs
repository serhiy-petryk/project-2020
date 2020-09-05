using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using DGCore.Menu;
using DGCore.UserSettings;
using DGView.Common;
using DGView.ViewModels;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for DataGridView.xaml
    /// </summary>
    public partial class DataGridView: IUserSettingSupport<DGV>, IDisposable
    {
        public static ObservableCollection<string> LogData = new ObservableCollection<string>();

        private DGListComponent _dGListComponent;
        private DGCore.Misc.DataDefiniton _dataDefinition { get; }
        private string _startUpParameters;

        public DataGridViewModel ViewModel => (DataGridViewModel)DataContext;

        public DataGridView(MenuOption menuOption, string startUpLayoutName, DGV settings)
        {
            InitializeComponent();

            DataContext = new DataGridViewModel(this);
            var container = AppViewModel.Instance.ContainerControl;
            container.Children.Add(new Mwi.MwiChild
            {
                Title = menuOption.Label,
                Content = this,
                Height = Math.Max(200.0, Window.GetWindow(container).ActualHeight * 2 / 3)
            });

            _dataDefinition = menuOption.GetDataDefiniton();
            var parameters = _dataDefinition.DbParameters;
            _startUpParameters = parameters == null || parameters._parameters.Count == 0
                ? _dataDefinition.WhereFilter.StringPresentation
                : _dataDefinition.DbParameters.GetStringPresentation();
            Bind(menuOption);
        }

        private void Bind(MenuOption menuOption)
        {
            if (!CommandBar.IsEnabled)
                return;

            CommandBar.IsEnabled = false;
            DgClear();
            _dGListComponent = new DGListComponent();

            Task.Factory.StartNew(() =>
            {
                var sw = new Stopwatch();
                sw.Start();

                var ds = _dataDefinition.GetDataSource(_dGListComponent);
                Type listType = typeof(DGCore.DGVList.DGVList<>).MakeGenericType(ds.ItemType);
                // var dataSource = Activator.CreateInstance(listType, ds, (Func<Utils.DGVColumnHelper[]>)GetColumnHelpers);
                var dataSource = (DGCore.DGVList.IDGVList)Activator.CreateInstance(listType, ds, null);
                _dGListComponent.Data = dataSource;
                dataSource.RefreshData();

                sw.Stop();
                var d1 = sw.Elapsed.TotalMilliseconds;
                sw.Reset();
                sw.Start();
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (Action)delegate ()
                    {
                        DataGrid.ItemsSource = (IEnumerable)dataSource;
                        sw.Stop();
                        var d2 = sw.Elapsed.TotalMilliseconds;
                        LogData.Add("Load data time: " + d1);
                        LogData.Add("Get data time: " + d2);
                        if (!DataGridViewModel.AUTOGENERATE_COLUMNS)
                            CreateColumnsRecursive(_dataDefinition.ItemType, new List<string>(), 0);
                        CommandBar.IsEnabled = true;
                    }
                );
            });
        }

        private void CreateColumnsRecursive(Type type, List<string> prefixes, int level)
        {
            if (level > 10)
                return;

            var types = type.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
            // var types = type.GetFields(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
            foreach (var t in types)
            {
                DataGridBoundColumn column = null;

                var propertyType = DGCore.Utils.Types.GetNotNullableType(t.PropertyType);
                switch (Type.GetTypeCode(propertyType))
                {
                    case TypeCode.Boolean:
                        column = new DataGridCheckBoxColumn();
                        break;

                    case TypeCode.Byte:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        column = (DataGridBoundColumn)FindResource("NumberColumn");
                        break;

                    case TypeCode.String:
                    case TypeCode.DateTime:
                        column = (DataGridBoundColumn)FindResource("TextColumn");
                        break;

                    case TypeCode.Object:
                        if (propertyType == typeof(TimeSpan))
                            column = (DataGridBoundColumn)FindResource("TextColumn");
                        else
                        {
                            var newPrefixes = new List<string>(prefixes);
                            newPrefixes.Add(t.Name);
                            CreateColumnsRecursive(propertyType, newPrefixes, level + 1);
                        }
                        break;

                    default:
                        throw new Exception("Check CreateColumns method");
                }

                if (column != null)
                {
                    column.Header = (string.Join(" ", prefixes) + " " + t.Name).Trim();
                    var binding = new Binding(prefixes.Count == 0 ? t.Name : string.Join(".", prefixes) + "." + t.Name);
                    if (propertyType == typeof(TimeSpan))
                        binding.StringFormat = null;
                    //if (t1 == typeof(long))
                    // binding.Converter = new Temp.LongConverter();
                    column.Binding = binding;
                    // ??? Sort support for BindingList=> doesn't work column.SortMemberPath = prefixes.Count == 0 ? t.Name : string.Join(".", prefixes) + "." + t.Name;
                    DataGrid.Columns.Add(column);
                }
            }
        }


        private void DgClear()
        {
            _dGListComponent?.Dispose();
            DataGrid.ItemsSource = null;
            // _dg.Items.Clear();
            DataGrid.Columns.Clear();
        }

        public void Dispose()
        {
            DgClear();
            _dataDefinition.Dispose(); // ??? 
        }

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
