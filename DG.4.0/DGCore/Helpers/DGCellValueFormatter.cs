using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using DGCore.PD;

namespace DGCore.Helpers
{
    public class DGCellValueFormatter
    {
        private static CultureInfo _culture = CultureInfo.CurrentCulture;
        private static TypeConverter _dateTimeConverter = new DateTimeConverter();

        //===========================
        private Type _propertyType;
        private string _format;
        private TypeConverter _converter;
        public DGCellValueFormatter(IMemberDescriptor propertyDescriptor)
        {
            var pd = propertyDescriptor as PropertyDescriptor;
            _propertyType = pd.PropertyType;
            _format = propertyDescriptor.Format;
            _converter = (propertyDescriptor as PropertyDescriptor).Converter;

        }

        public object GetValueForPrinter(object value)
        {
           // Debug.Print($"Type: {value?.GetType().Name}");
            if (value == null || value is string || value is bool || value is byte[]) return value;
            if (value is DateTime dt && string.IsNullOrEmpty(_format))
                return _dateTimeConverter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));
            if (value is IFormattable formattable)
                return formattable.ToString(_format, _culture);
            if (_converter != null) // Dynamic types
                return value.ToString();
            throw new Exception($"Trap!!! DGCellValueFormatter.GetValueForPrint. Data type: {value.GetType().Name}");
        }
    }
}
