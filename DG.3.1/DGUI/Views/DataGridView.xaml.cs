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
using DGUI.Common;
using DGUI.ViewModels;

namespace DGUI.Views
{
    /// <summary>
    /// Interaction logic for DataGridView.xaml
    /// </summary>
    public partial class DataGridView
    {
        public const bool AUTOGENERATE_COLUMNS = false;
        public static ObservableCollection<string> LogData = new ObservableCollection<string>();

        private DGCore.Misc.DataDefiniton _dataDefinition { get; }
        private DGListComponent _dGListComponent;

        public DataGridView(MenuOption menuOption)
        {
            InitializeComponent();

            DataContext = this;
            _dataDefinition = menuOption.GetDataDefiniton();
            var container = AppViewModel.Instance.MwiContainer;
            container.Children.Add(new Mwi.MwiChild
            {
                Title = menuOption.Label,
                Content = this
            });
            Bind(null, null, null, null);
        }

        private void Bind(string layoutID, string startUpParameters, string startUpLayoutName, DGCore.UserSettings.DGV settings)
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
                        if (!AUTOGENERATE_COLUMNS)
                            CreateColumns();
                        CommandBar.IsEnabled = true;
                    }
                );
            });

        }

        private void CreateColumns() => CreateColumnsRecursive(_dataDefinition.ItemType, new List<string>(), 0);
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


        private void DataGridView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            DgClear();
            _dataDefinition.Dispose(); // ??? 
        }
    }
}
