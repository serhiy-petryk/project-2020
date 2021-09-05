using DGCore.DGVList;
using DGCore.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DGView.Helpers
{
    public class DGColumnHelper : IDGColumnHelper
    {
        private static PropertyInfo _piDataGridOwner = typeof(DataGridColumn).GetProperty("DataGridOwner", BindingFlags.Instance | BindingFlags.NonPublic);

        private int _method = -1;
        private string _format;
        private object _nullValue;
        private IFormatProvider _formatProvider;
        private TypeConverter _converter;
        private TypeConverter _converter1;// for image
        private Type _formattedValueType;
        // private DataGridViewImageCellLayout _imageLayout = DataGridViewImageCellLayout.NotSet;
        public DGColumnHelper(DataGridColumn dgColumn)
        {
            var dg = (DataGrid)_piDataGridOwner.GetValue(dgColumn, null);
            var data = (IDGVList)dg.ItemsSource;
            PropertyDescriptor = data.Properties.OfType<PropertyDescriptor>().Where(pd => pd.Name == dgColumn.SortMemberPath).FirstOrDefault();

            if (PropertyDescriptor != null)
            {
                this._formattedValueType = PropertyDescriptor.PropertyType;

                if (PropertyDescriptor is DGCore.PD.IMemberDescriptor)
                {
                    _nullValue = ((DGCore.PD.IMemberDescriptor)PropertyDescriptor).DbNullValue;
                }

                //**          if (Utils.Types.IsNullableType(dgvColumn.ValueType)) _nullableConverter = TypeDescriptor.GetConverter(dgvColumn.ValueType);
                // Type valueType = DGCore.Utils.Types.GetNotNullableType(dgColumn.ValueType);
                Type valueType = DGCore.Utils.Types.GetNotNullableType(PropertyDescriptor.PropertyType);

                // this._format = dgColumn.InheritedStyle.Format;
                // this._formatProvider = dgColumn.InheritedStyle.FormatProvider;
                this._converter = TypeDescriptor.GetConverter(valueType);
                this._converter1 = TypeDescriptor.GetConverter(this._formattedValueType);// for Images
                /*if (this._converter1 != null) {
                  bool b1 = this._converter1.CanConvertFrom(valueType);
                }*/
                if (_formattedValueType == typeof(string))
                {
                    if (valueType == typeof(string)) this._method = 0;
                    else if (valueType.GetInterface("System.IFormattable") != null && !String.IsNullOrEmpty(this._format))
                    {
                        this._method = (DGCore.Utils.Types.IsNumericType(valueType) ? 9 : 1);// format doesnot applied to numbers in Clipboard mode
                    }
                    else if (_converter != null && _converter.CanConvertTo(typeof(string))) _method = 2;
                    else if (valueType is IConvertible) _method = 3;
                    else _method = 4;
                }
                else if (_formattedValueType == typeof(bool)) _method = 5;
                // else if (_formattedValueType == typeof(CheckState)) _method = 6;
                /* DGV           if (dgvColumn.ValueType == typeof(CheckState)) _method = 6;
                //        else if (Utils.Types.GetNotNullableType(dgvColumn.ValueType) == typeof(bool)) _method = 5;
                //        else throw new Exception ("AAAA");*/
                // else if (_formattedValueType == typeof(Image) || _formattedValueType == typeof(Icon)) _method = 7;
                else _method = 8;
            }

        }

        private object GetFormattedValueFromValue(object value, bool clipboardMode)
        {
            if (value == null || !IsValid || (_nullValue != null && Equals(_nullValue, value)))
                return null;
            switch (_method)
            {
                case 0: return (string)value;
                case 1: return ((IFormattable)value).ToString(this._format, this._formatProvider);
                case 9:
                    if (clipboardMode) return value.ToString();// numbers in clipboard mode
                    else return ((IFormattable)value).ToString(this._format, this._formatProvider);
                case 2: return (string)this._converter.ConvertTo(value, typeof(string));
                case 3: return (string)Convert.ChangeType(value, TypeCode.String, this._formatProvider);
                //        case 5: return ((bool)value).ToString();
                case 5: return (clipboardMode ? ((bool)value).ToString() : value);
                /*case 6:
                    if (clipboardMode)
                    {
                        return value.ToString();
                    }
                    else
                    {
                        if (value is bool)
                        {
                            return (bool)value ? CheckState.Checked : CheckState.Unchecked;
                        }
                        return Convert.ChangeType(value, typeof(CheckState));
                    }*/
                case 7: return this._converter1.ConvertFrom(value);
                default: return value.ToString();// case this._method = 4 or 8
            }
        }

        public override string ToString() => PropertyDescriptor.ToString();

        #region ==========  IDGColumnHelper  =============
        public PropertyDescriptor PropertyDescriptor { get; }

        public bool IsValid => PropertyDescriptor != null;

        public bool Contains(object item, string searchString) => // Does formatted value of item contain searchString ?
                  GetFormattedValueFromItem(item, false)?.ToString().IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;

        public object GetFormattedValueFromItem(object item, bool clipboardMode) => // numbers in clipboard mode is showing without format
          item == null || !IsValid ? null : GetFormattedValueFromValue(PropertyDescriptor.GetValue(item), clipboardMode);
        #endregion
    }
}
