﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DGView.ViewModels;
using WpfSpLib.Helpers;

namespace DGView.Helpers
{
    public static class DataGridHelper
    {
        public static void GetSelectedArea(DataGrid dg, out IList objectsToCopy, out DataGridColumn[] columns)
        {
            var validColumns = dg.Columns.Where(c => c.Visibility == Visibility.Visible);
            if (dg.SelectedCells.Count < 2)
            {
                objectsToCopy = (IList)dg.ItemsSource;
                columns = validColumns.OrderBy(c => c.DisplayIndex).ToArray();
                return;
            }

            var rowItems = dg.ItemsSource.Cast<object>().Select((item, index) => new { index, item }).ToDictionary(x => x.item, x => x.index);
            var tempColumns = validColumns.ToDictionary(x => x, x => x.DisplayIndex);
            var selectedItems = new Dictionary<object, int>();
            var selectedColumns = new Dictionary<DataGridColumn, int>();

            foreach (var item in dg.SelectedItems)
            {
                selectedItems.Add(item, rowItems[item]);
                rowItems.Remove(item);
            }

            foreach (var cell in dg.SelectedCells.Where(c => c.IsValid && c.Column.Visibility == Visibility.Visible))
            {
                if (rowItems.Count > 0 && rowItems.ContainsKey(cell.Item))
                {
                    selectedItems.Add(cell.Item, rowItems[cell.Item]);
                    rowItems.Remove(cell.Item);
                }

                if (tempColumns.Count > 0 && tempColumns.ContainsKey(cell.Column))
                {
                    selectedColumns.Add(cell.Column, tempColumns[cell.Column]);
                    tempColumns.Remove(cell.Column);
                }

                if (rowItems.Count == 0 && tempColumns.Count == 0)
                    break;
            }

            objectsToCopy = selectedItems.OrderBy(o => o.Value).Select(o => o.Key).ToArray();
            columns = selectedColumns.OrderBy(o => o.Value).Select(o => o.Key).ToArray();
        }

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
                if (dgv.Columns[k].SortMemberPath == propertyName)
                    return k;
            return -1;
        }

        public static TextAlignment? GetDefaultColumnAlignment(Type type)
        {
            type = DGCore.Utils.Types.GetNotNullableType(type);
            if (type == null) return null;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return TextAlignment.Center;
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
                    return TextAlignment.Right;
                case TypeCode.String:
                case TypeCode.DateTime:
                    return TextAlignment.Left;
                case TypeCode.Object:
                    return TextAlignment.Left;

                default:
                    throw new Exception("Check DataGridHelper.GetDefaultColumnAlignment method");
            }
        }

        public static void GenerateColumns(DGViewModel viewModel)
        {
            foreach (PropertyDescriptor pd in viewModel.Properties)
            {
                var propertyType = DGCore.Utils.Types.GetNotNullableType(pd.PropertyType);
                DataGridColumn column;
                if (propertyType == typeof(bool)) column = new DataGridCheckBoxColumn();
                else if (propertyType == typeof(byte[]))
                {
                    var template = TemplateGenerator.CreateDataTemplate(() =>
                        {
                            var result = new Image();
                            result.SetBinding(Image.SourceProperty, pd.Name);
                            return result;
                        }
                    );
                    column = new DataGridTemplateColumn {CellTemplate = template, SortMemberPath = pd.Name};
                }
                else column = new DataGridTextColumn();

                // = propertyType == typeof(bool) ? (DataGridBoundColumn)new DataGridCheckBoxColumn() : new DataGridTextColumn();

                column.Header = pd.DisplayName.Replace("^", " ");
                if (column is DataGridBoundColumn boundColumn)
                {
                    var binding = new Binding(pd.Name);
                    if (propertyType == typeof(TimeSpan))
                        binding.StringFormat = null;
                    boundColumn.Binding = binding;
                }

                // ??? Sort support for BindingList=> doesn't work column.SortMemberPath = prefixes.Count == 0 ? t.Name : string.Join(".", prefixes) + "." + t.Name;
                viewModel.DGControl.Columns.Add(column);
                column.CanUserSort = typeof(IComparable).IsAssignableFrom(propertyType);
                column.Visibility = pd.Name.Contains(".") ? Visibility.Collapsed : Visibility.Visible;
                column.Width = DataGridLength.Auto;
                column.MaxWidth = 2000;

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

                var columnHeaderStyle = viewModel.DGControl.Resources["DataGridColumnHeaderStyle"] as Style;
                // Add tooltip to column header
                if (!string.IsNullOrEmpty(pd.Description))
                    columnHeaderStyle.Setters.Add(new Setter(ToolTipService.ToolTipProperty, pd.Description));
                column.HeaderStyle = columnHeaderStyle;

                if (column is DataGridCheckBoxColumn checkBoxColumn)
                {
                    var elementStyle = viewModel.DGControl.Resources["DataGridCheckBoxColumnElementStyle"] as Style;
                    checkBoxColumn.ElementStyle = elementStyle;
                }
            }

            // Set IsFrozen to false (by default all columns after 'viewModel.DGControl.Columns.Add(column)' are frozen)
            if (viewModel.DGControl.Columns.Count > 0)
            {
                viewModel.DGControl.FrozenColumnCount = 1;
                viewModel.DGControl.FrozenColumnCount = 0;
            }
        }
    }
}