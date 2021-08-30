using System;
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

        public static void GenerateColumns(DataGridViewModel viewModel)
        {
            foreach (PropertyDescriptor pd in viewModel.Properties)
            {
                var propertyType = DGCore.Utils.Types.GetNotNullableType(pd.PropertyType);
                var column = propertyType == typeof(bool) ? (DataGridBoundColumn)new DataGridCheckBoxColumn() : new DataGridTextColumn();

                column.Header = pd.DisplayName.Replace("^", " ");
                var binding = new Binding(pd.Name);
                if (propertyType == typeof(TimeSpan))
                    binding.StringFormat = null;
                column.Binding = binding;
                // ??? Sort support for BindingList=> doesn't work column.SortMemberPath = prefixes.Count == 0 ? t.Name : string.Join(".", prefixes) + "." + t.Name;
                viewModel.DGControl.Columns.Add(column);
                column.Visibility = pd.Name.Contains(".") ? Visibility.Collapsed : Visibility.Visible;
                column.Width = DataGridLength.Auto;

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

                var columnHeaderStyle = viewModel.View.Resources["DataGridColumnHeaderStyle"] as Style;
                // Add tooltip to column header
                if (!string.IsNullOrEmpty(pd.Description))
                    columnHeaderStyle.Setters.Add(new Setter(ToolTipService.ToolTipProperty, pd.Description));
                column.HeaderStyle = columnHeaderStyle;

                if (column is DataGridCheckBoxColumn)
                {
                    var elementStyle = viewModel.View.Resources["DataGridCheckBoxColumnElementStyle"] as Style;
                    column.ElementStyle = elementStyle;
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