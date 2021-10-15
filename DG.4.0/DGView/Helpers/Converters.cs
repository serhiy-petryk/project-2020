using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using DGCore.DGVList;

namespace DGView.Helpers
{
    public class BackgroundOfSelectedMenuItemConverter : IMultiValueConverter
    {
        public static BackgroundOfSelectedMenuItemConverter Instance = new BackgroundOfSelectedMenuItemConverter();
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values == null || values.Length < 2 || !Equals(values[0], values[1]) ? null: Brushes.PaleGreen;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class ComparisonConverter : IValueConverter
    {
        public static ComparisonConverter Instance = new ComparisonConverter();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : Binding.DoNothing;
        }
    }

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
