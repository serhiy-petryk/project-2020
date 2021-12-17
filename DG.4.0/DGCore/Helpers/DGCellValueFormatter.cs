﻿using System;
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
        private Type _propertyType;
        private string _format;
        // private TypeConverter _converter;
        private Func<object, object> _funcGetValueForPrinter;
        private int _kind;
        public DGCellValueFormatter(IMemberDescriptor propertyDescriptor)
        {
            IsValid = propertyDescriptor != null;
            if (!IsValid)
            {
                _funcGetValueForPrinter = o => null;
                return;
            }

            _format = propertyDescriptor.Format;
            var pd = propertyDescriptor as PropertyDescriptor;
            _propertyType = Utils.Types.GetNotNullableType(pd.PropertyType);
            var converter = pd.Converter;

            if (_propertyType == typeof(string) || _propertyType == typeof(bool) || _propertyType == typeof(byte[]))
            {
                _kind = 1;
                _funcGetValueForPrinter = o => o;
            }
            else if (_propertyType == typeof(DateTime) && string.IsNullOrEmpty(_format))
            {
                _kind = 2;
                _funcGetValueForPrinter = o => _dateTimeConverter.ConvertTo(null, CultureInfo.CurrentCulture, o, typeof(string));
            }
            else if (typeof(IFormattable).IsAssignableFrom(_propertyType))
            {
                _kind = 3;
                _funcGetValueForPrinter = o => ((IFormattable) o)?.ToString(_format, CultureInfo.CurrentCulture);
            }
            else if (converter != null)
            {
                _kind = 5;
                _funcGetValueForPrinter = o => o?.ToString();
            }
            else
                throw new Exception($"Trap!!! DGCellValueFormatter.GetValueForPrint. Data type: {_propertyType}");
        }

        public object GetValueForPrinter(object value) => _funcGetValueForPrinter(value);
    }
}
