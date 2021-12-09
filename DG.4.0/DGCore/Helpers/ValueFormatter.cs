using System;
using System.ComponentModel;
using System.Globalization;
using DGCore.PD;

namespace DGCore.Helpers
{
    public class ValueConverter
    {
        public bool IsValid => _property != null;

        private PropertyDescriptor _property;
        private Type _sourceType;
        private Type _targetType;
        private string _format;
        private object _nullValue;
        private TypeConverter _sourceConverter;
        private TypeConverter _targetConverter;// for image

        // private Func<object, string> _getStringFunction;
        private Func<object, object> _getValueFunction;
        private Func<object, string> _getClipboardStringFunction;

        public ValueConverter(PropertyDescriptor propertyDescriptor, Type targetType)
        {
            _property = propertyDescriptor;
            if (_property != null && _property is IMemberDescriptor descriptor)
            {
                _nullValue = descriptor.DbNullValue;
                _sourceType = Utils.Types.GetNotNullableType(_property.PropertyType);
                _targetType = targetType ?? _sourceType;
                _format = descriptor.Format;
                _sourceConverter = TypeDescriptor.GetConverter(_sourceType);
                _targetConverter = TypeDescriptor.GetConverter(_targetType); // for Images

                if (_targetType == typeof(string))
                {
                    if (_sourceType == typeof(string))
                    {
                        _getValueFunction = o => o;
                        _getClipboardStringFunction = o => (string)o;
                    }
                    else if (_sourceType.GetInterface("System.IFormattable") != null && !string.IsNullOrEmpty(this._format))
                    {
                        _getValueFunction = o => ((IFormattable)o)?.ToString(_format, CultureInfo.CurrentCulture);
                        _getClipboardStringFunction = o => o?.ToString();
                    }
                    else if (_sourceConverter != null && _sourceConverter.CanConvertTo(typeof(string)))
                    {
                        _getValueFunction = o => _sourceConverter.ConvertTo(o, typeof(string));
                        _getClipboardStringFunction = o => (string)_sourceConverter.ConvertTo(o, typeof(string));
                    }
                    else if (_sourceType is IConvertible)
                    {
                        _getValueFunction = o => Convert.ChangeType(o, TypeCode.String, CultureInfo.CurrentCulture);
                        _getClipboardStringFunction = o => (string) Convert.ChangeType(o, TypeCode.String, CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        _getValueFunction = o => o?.ToString();
                        _getClipboardStringFunction = o => o?.ToString();
                    }


                }
                else if (_targetType == typeof(bool))
                {
                    _getValueFunction = o => o;
                    _getClipboardStringFunction = o => o?.ToString();
                }
                else
                {
                    if (_sourceConverter.CanConvertTo(_targetType))
                    {
                        _getValueFunction = o => _sourceConverter.ConvertTo(o, _targetType);
                        _getClipboardStringFunction = o => _sourceConverter.ConvertTo(o, _targetType)?.ToString();
                    }
                    else if (_targetConverter.CanConvertFrom(_sourceType))
                    {
                        _getValueFunction = o => _targetConverter.ConvertFrom(o);
                        _getClipboardStringFunction = o => _targetConverter.ConvertFrom(o)?.ToString();
                    }
                    else
                        throw new Exception($"Trap!!! ValueConverter doesn't defined for '{_targetType.Name}' type");
                }
                // else if (_formattedValueType == typeof(Image) || _formattedValueType == typeof(Icon)) _method = 7;
                // else _method = 8;
                //        else if (_formattedValueType == typeof(CheckState)) _method = 6;



            }
            else
            {
                _getValueFunction = o => null;
                _getClipboardStringFunction= o => null;
            }
        }

        // public string GetString(object o) => _getStringFunction(o);
        public string GetClipboardString(object o) => _getClipboardStringFunction(o);
        public object GetValue(object o) => _getValueFunction(o);
    }
}
