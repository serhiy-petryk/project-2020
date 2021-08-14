using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DGView.Views;

namespace DGView.Helpers
{
    public static class DataGridHelper
    {
        public static void CreateColumnsRecursive(DataGridView dgv, Type type, List<string> prefixes, int level)
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
                        column = (DataGridBoundColumn) dgv.FindResource("NumberColumn");
                        break;

                    case TypeCode.String:
                    case TypeCode.DateTime:
                        column = (DataGridBoundColumn) dgv.FindResource("TextColumn");
                        break;

                    case TypeCode.Object:
                        if (propertyType == typeof(TimeSpan))
                            column = (DataGridBoundColumn) dgv.FindResource("TextColumn");
                        else
                        {
                            var newPrefixes = new List<string>(prefixes);
                            newPrefixes.Add(t.Name);
                            CreateColumnsRecursive(dgv, propertyType, newPrefixes, level + 1);
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
                    dgv.DataGrid.Columns.Add(column);
                    column.Visibility = level == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
            }

        }
    }
}