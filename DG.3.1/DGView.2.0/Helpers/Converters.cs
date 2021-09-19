using System;
using System.Globalization;
using System.Windows.Data;
using DGCore.DGVList;

namespace DGView.Helpers
{
    public class IsGroupItemConverter : IValueConverter
    {
        public static IsGroupItemConverter Instance = new IsGroupItemConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is IDGVList_GroupItem;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class TestConverter : IValueConverter
    {
        public static TestConverter Instance = new TestConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class OpacityForDataGridRowHeaderConverter : IValueConverter
    {
        public static OpacityForDataGridRowHeaderConverter Instance = new OpacityForDataGridRowHeaderConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value.GetType().Name == "NamedObject" => new row ({{NewItemPlaceholder}}) in DataGrid
            return value == null || Equals(value.GetType().Name, "NamedObject") ? 0.0 : 1.0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
