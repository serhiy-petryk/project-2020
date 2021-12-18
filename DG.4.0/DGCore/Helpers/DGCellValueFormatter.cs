using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using DGCore.PD;

namespace DGCore.Helpers
{
    public class DGCellValueFormatter
    {
        // private static CultureInfo _culture = CultureInfo.CurrentCulture;
        private static TypeConverter _dateTimeConverter = new DateTimeConverter();

        //===========================
        public readonly bool IsValid;
        public readonly Type PropertyType;

        private readonly PropertyDescriptor _pd;
        private string _format;

        private Func<object, object> _funcGetValueForPrinter;
        private Func<object, object> _funcGetValueForClipboard;
        private Func<object, string> _funcGetStringForFind;

        public DGCellValueFormatter(IMemberDescriptor propertyDescriptor)
        {
            IsValid = propertyDescriptor != null;
            if (!IsValid) return;

            _format = propertyDescriptor.Format;
            _pd = propertyDescriptor as PropertyDescriptor;
            PropertyType = Utils.Types.GetNotNullableType(_pd.PropertyType);
            var converter = _pd.Converter;

            if (PropertyType == typeof(string))
            {
                _funcGetValueForPrinter = o => o;
                _funcGetValueForClipboard = _funcGetValueForPrinter;
                _funcGetStringForFind = o => (string)o;
            }
            else if (PropertyType == typeof(byte[]))
            {
                _funcGetValueForPrinter = o => o;
                _funcGetValueForClipboard = _funcGetValueForPrinter;
                _funcGetStringForFind = o => null;
            }
            else if (PropertyType == typeof(bool))
            {
                _funcGetValueForPrinter = o => o;
                _funcGetValueForClipboard = o => o?.ToString();
                _funcGetStringForFind = o => null;
            }
            else if (PropertyType == typeof(DateTime) && string.IsNullOrEmpty(_format))
            {
                _funcGetValueForPrinter = o => _dateTimeConverter.ConvertTo(null, CultureInfo.CurrentCulture, o, typeof(string));
                _funcGetValueForClipboard = _funcGetValueForPrinter;
                _funcGetStringForFind = o => (string)_dateTimeConverter.ConvertTo(null, CultureInfo.CurrentCulture, o, typeof(string));
            }
            else if (typeof(IFormattable).IsAssignableFrom(PropertyType))
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
                throw new Exception($"Trap!!! DGCellValueFormatter.GetValueForPrint. Data type: {PropertyType}");
        }

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
        public bool DoesPropertyOfItemContainText(object item, string searchText)
        {
            if (!IsValid) return false;

            var value = _funcGetStringForFind(_pd.GetValue(item));
            return value != null && value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        public IEnumerable<string> GetUniqueStrings(IEnumerable<object> items)
        {
          if (!IsValid) return new string[0];
          return items.Select(item => _funcGetValueForPrinter(_pd.GetValue(item))).OfType<string>().Distinct();
        }
    }
}
