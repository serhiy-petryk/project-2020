using System;
using System.ComponentModel;
using System.Globalization;
using DGCore.PD;

namespace DGCore.Helpers
{
    public class DGCellValueFormatter
    {
        // private static CultureInfo _culture = CultureInfo.CurrentCulture;
        private static TypeConverter _dateTimeConverter = new DateTimeConverter();

        //===========================
        public readonly bool IsValid;
        private readonly PropertyDescriptor _pd;
        private Type _propertyType;
        private string _format;
        // private TypeConverter _converter;
        private Func<object, object> _funcGetValueForPrinter;
        private Func<object, object> _funcGetValueForClipboard;
        private Func<object, string> _funcGetStringForFind;
        private int _kind;

        public DGCellValueFormatter(IMemberDescriptor propertyDescriptor)
        {
            IsValid = propertyDescriptor != null;
            if (!IsValid) return;

            _format = propertyDescriptor.Format;
            _pd = propertyDescriptor as PropertyDescriptor;
            _propertyType = Utils.Types.GetNotNullableType(_pd.PropertyType);
            var converter = _pd.Converter;

            if (_propertyType == typeof(string))
            {
                _funcGetValueForPrinter = o => o;
                _funcGetValueForClipboard = _funcGetValueForPrinter;
                _funcGetStringForFind = o => (string)o;
            }
            else if (_propertyType == typeof(byte[]))
            {
                _funcGetValueForPrinter = o => o;
                _funcGetValueForClipboard = _funcGetValueForPrinter;
                _funcGetStringForFind = o => null;
            }
            else if (_propertyType == typeof(bool))
            {
                _funcGetValueForPrinter = o => o;
                _funcGetValueForClipboard = o => o?.ToString();
                _funcGetStringForFind = o => null;
            }
            else if (_propertyType == typeof(DateTime) && string.IsNullOrEmpty(_format))
            {
                _funcGetValueForPrinter = o => _dateTimeConverter.ConvertTo(null, CultureInfo.CurrentCulture, o, typeof(string));
                _funcGetValueForClipboard = _funcGetValueForPrinter;
                _funcGetStringForFind = o => (string)_dateTimeConverter.ConvertTo(null, CultureInfo.CurrentCulture, o, typeof(string));
            }
            else if (typeof(IFormattable).IsAssignableFrom(_propertyType))
            {
                _funcGetValueForPrinter = o => ((IFormattable)o)?.ToString(_format, CultureInfo.CurrentCulture);
                _funcGetValueForClipboard = o => o?.ToString();
                _funcGetStringForFind = o => ((IFormattable)o)?.ToString(_format, CultureInfo.CurrentCulture);
            }
            else if (converter != null)
            {
                _funcGetValueForPrinter = o => o?.ToString();
                _funcGetValueForClipboard = _funcGetValueForPrinter;
                _funcGetStringForFind = o => o?.ToString();
            }
            else
                throw new Exception($"Trap!!! DGCellValueFormatter.GetValueForPrint. Data type: {_propertyType}");
        }

        public object xxGetValueForPrinter(object value) => _funcGetValueForPrinter(value);
        public object GetValueForPrinterFromItem(object item)
        {
            if (!IsValid) return null;

            var value = _pd.GetValue(item);
            return _funcGetValueForPrinter(value);
        }
        public object GetValueForClipboardFromItem(object item)
        {
            if (!IsValid) return null;

            var value = _pd.GetValue(item);
            return _funcGetValueForClipboard(value);
        }
        public string GetStringForFind(object item)
        {
            if (!IsValid) return null;

            var value = _pd.GetValue(item);
            return _funcGetStringForFind(value);
        }
    }
}
