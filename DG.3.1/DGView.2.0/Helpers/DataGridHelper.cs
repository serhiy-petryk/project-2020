﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DGView.ViewModels;

namespace DGView.Helpers
{
    public static class DataGridHelper
    {
        public static void SetColumnVisibility(DataGridColumn column, bool isVisible)
        {
            if (column.Visibility == Visibility.Visible && !isVisible)
                column.Visibility = Visibility.Collapsed;
            else if (column.Visibility != Visibility.Visible && isVisible)
                column.Visibility = Visibility.Visible;
        }

        public static int GetColumnIndexByPropertyName(DataGrid dgv, string propertyName)
        {
            for (var k = 0; k < dgv.Columns.Count; k++)
            {
                if (string.Equals(dgv.Columns[k].SortMemberPath, propertyName, StringComparison.InvariantCultureIgnoreCase))
                    return k;
            }
            return -1;
        }

        public static DataGridColumn[] GetColumnsInDisplayOrder(DataGrid dg, bool onlyVisibleColumns) => dg.Columns
            .Where(c => c.Visibility == Visibility.Visible || !onlyVisibleColumns).OrderBy(c => c.DisplayIndex).ToArray();

        public static void GenerateColumns(DataGridViewModel viewModel)
        {
            var dgv = viewModel.View;
            foreach (PropertyDescriptor pd in viewModel.Data.Properties)
            {
                DataGridBoundColumn column = null;
                var propertyType = DGCore.Utils.Types.GetNotNullableType(pd.PropertyType);
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
                        column = viewModel.View.Resources["NumericColumn"] as DataGridBoundColumn;
                        break;

                    case TypeCode.String:
                    case TypeCode.DateTime:
                        column = viewModel.View.Resources["TextColumn"] as DataGridBoundColumn;
                        break;

                    case TypeCode.Object:
                        //if (propertyType == typeof(TimeSpan))
                        //  column = (DataGridBoundColumn) dgv.FindResource("TextColumn");
                        //else
                        column = viewModel.View.Resources["TextColumn"] as DataGridBoundColumn;
                        break;

                    default:
                        throw new Exception("Check CreateColumns method");
                }

                if (column != null)
                {
                    column.Header = pd.DisplayName.Replace("^", " ");
                    var binding = new Binding(pd.Name);
                    if (propertyType == typeof(TimeSpan))
                        binding.StringFormat = null;
                    column.Binding = binding;
                    // ??? Sort support for BindingList=> doesn't work column.SortMemberPath = prefixes.Count == 0 ? t.Name : string.Join(".", prefixes) + "." + t.Name;
                    viewModel.DGControl.Columns.Add(column);
                    column.Visibility = pd.Name.Contains(".") ? Visibility.Collapsed : Visibility.Visible;

                    /* Create datagrid header style programmatically
                    // Create data template for column header
                    var dt = new DataTemplate();
                    var rectangleFactory = new FrameworkElementFactory(typeof(TextBlock));
                    rectangleFactory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
                    var b1 = new Binding { Path = new PropertyPath("Header"), Source = column };
                    rectangleFactory.SetBinding(TextBlock.TextProperty, b1);
                    dt.VisualTree = rectangleFactory;

                    var style = new Style(typeof(DataGridColumnHeader));
                    style.Setters.Add(new Setter(ContentControl.ContentTemplateProperty, dt));
                    column.HeaderStyle = style;*/

                    var columnHeaderStyle = dgv.FindResource("DataGridColumnHeaderStyle") as Style;
                    // Add tooltip to column header
                    if (!string.IsNullOrEmpty(pd.Description))
                        columnHeaderStyle.Setters.Add(new Setter(ToolTipService.ToolTipProperty, pd.Description));
                    column.HeaderStyle = columnHeaderStyle;
                }
            }
        }
    }
}