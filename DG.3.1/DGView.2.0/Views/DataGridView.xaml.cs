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
using DGView.ViewModels;
using WpfSpLib.Controls;
using WpfSpLib.Helpers;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for DataGridView.xaml
    /// </summary>
    public partial class DataGridView : UserControl
    {
        public static ObservableCollection<string> LogData = new ObservableCollection<string>();

        // internal DGListComponent _dGListComponent;
        // private DGCore.Misc.DataDefinition _dataDefinition { get; }
        // private string _startUpParameters;

        public DataGridViewModel ViewModel => (DataGridViewModel)DataContext;

        public DataGridView(MwiContainer container, MenuOption menuOption, string startUpLayoutName, DGV settings)
        {
            InitializeComponent();
            Unloaded += OnUnloaded;

            var dataDefinition = menuOption.GetDataDefiniton();
            var parameters = dataDefinition.DbParameters;
            var startUpParameters = parameters == null || parameters._parameters.Count == 0
                ? dataDefinition.WhereFilter.StringPresentation
                : dataDefinition.DbParameters.GetStringPresentation();

            // _dGListComponent = new DGListComponent();
            DataContext = new DataGridViewModel(this, dataDefinition.GetDataSource(ViewModel), startUpParameters);

            container.Children.Add(new MwiChild
            {
                Title = menuOption.Label,
                Content = this,
                Height = Math.Max(200.0, Window.GetWindow(container).ActualHeight * 2 / 3)
            });

            Bind();
        }

        public void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.IsElementDisposing())
            {
                DataGrid.ItemsSource = null;
                DataGrid.Columns.Clear();
                ViewModel.Dispose();
            }
        }

        private void Bind()
        {
            if (!CommandBar.IsEnabled)
                return;

            CommandBar.IsEnabled = false;

            var listType = typeof(DGCore.DGVList.DGVList<>).MakeGenericType(ViewModel.DataSource.ItemType);
            var dataSource = (DGCore.DGVList.IDGVList)Activator.CreateInstance(listType, ViewModel.DataSource, null);
            ViewModel.Data = dataSource;

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
                    DataGrid.ItemsSource = (IEnumerable)dataSource;
                    sw.Stop();
                    var d2 = sw.Elapsed.TotalMilliseconds;
                    LogData.Add("Load data time: " + d1);
                    LogData.Add("Get data time: " + d2);
                    if (!DataGridViewModel.AUTOGENERATE_COLUMNS)
                        CreateColumnsRecursive(ViewModel.DataSource.ItemType, new List<string>(), 0);
                    CommandBar.IsEnabled = true;
                }), DispatcherPriority.Normal);
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

    }
}
