﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DGView.Helpers
{
    public class OpacityForDataGridRowHeaderConverter : DependencyObject, IValueConverter
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
