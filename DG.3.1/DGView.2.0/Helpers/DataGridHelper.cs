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
        public static DataGridColumn[] GetColumnsInDisplayOrder(DataGrid dgv, bool onlyVisibleColumns) => dgv.Columns
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
                        column = GetNumericColumn();// (DataGridBoundColumn)dgv.FindResource("NumberColumn");
                        break;

                    case TypeCode.String:
                    case TypeCode.DateTime:
                        column = GetTextColumn();// (DataGridBoundColumn)dgv.FindResource("TextColumn");
                        break;

                    case TypeCode.Object:
                        //if (propertyType == typeof(TimeSpan))
                        //  column = (DataGridBoundColumn) dgv.FindResource("TextColumn");
                        //else
                        column = GetTextColumn();// (DataGridBoundColumn)dgv.FindResource("TextColumn");
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

                    var columnHeaderStyle = dgv.FindResource("DataGridHeaderStyle") as Style;
                    // Add tooltip to column header
                    if (!string.IsNullOrEmpty(pd.Description))
                        columnHeaderStyle.Setters.Add(new Setter(ToolTipService.ToolTipProperty, pd.Description));
                    column.HeaderStyle = columnHeaderStyle;
                }
            }
        }

        private static DataGridBoundColumn GetTextColumn()
        {
            var style = new Style();
            style.Setters.Add(new Setter(TextBox.TextWrappingProperty, TextWrapping.Wrap));
            var c = new DataGridTextColumn { Width = DataGridLength.Auto, MaxWidth = 500, ElementStyle = style };
            return c;
        }
        private static DataGridBoundColumn GetNumericColumn()
        {
            var style = new Style();
            style.Setters.Add(new Setter(TextBox.TextAlignmentProperty, TextAlignment.Right));
            var c = new DataGridTextColumn { Width = DataGridLength.Auto, MaxWidth = 500, ElementStyle = style};
            return c;
        }
    }
}