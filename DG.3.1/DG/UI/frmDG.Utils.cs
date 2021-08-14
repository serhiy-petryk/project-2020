using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;
using DG.Common;

namespace DG.UI
{
    /// <summary>
    /// Interaction logic for MainWindowSort.xaml
    /// </summary>
    public partial class frmDG
    {
        private void DgClear()
        {
            _dGListComponent?.Dispose();
            DataGrid.ItemsSource = null;
            // _dg.Items.Clear();
            DataGrid.Columns.Clear();
        }

        private void CreateColumns() => CreateColumnsRecursive(_dataType, new List<string>(), 0);
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

        private void Bind(DGCore.Misc.DataDefinition dd, string layoutID, string startUpParameters, string startUpLayoutName, DGCore.UserSettings.DGV settings)
        {
            if (!IsCommandBarEnabled)
                return;

            IsCommandBarEnabled = false;
            DgClear();
            _dGListComponent = new DGListComponent();

            Task.Factory.StartNew(() =>
            {
                var sw = new Stopwatch();
                sw.Start();

                var ds = dd.GetDataSource(_dGListComponent);
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
                      IsCommandBarEnabled = true;
                  }
                );
            });
        }


        private void BtnSvgImageTest_OnClick(object sender, RoutedEventArgs e)
        {
        }
        private void BtnSettings_OnClick(object sender, RoutedEventArgs e)
        {
        }
        private void BtnSelectLayout_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.Placement = PlacementMode.Bottom;
            button.ContextMenu.IsOpen = true;
        }
        private void BtnSaveLayout_OnClick(object sender, RoutedEventArgs e)
        {
        }
        private void BtnToggleGrid_OnClick(object sender, RoutedEventArgs e)
        {
        }
        private void BtnSetFont_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
