// стандартный подход (перебор ячеек + вызов FormattedValue занимает 11 сек/100,000 записей (см. ниже button5)
// перебор массива данных + вызов System.Windows.Forms.Formatter.FormatObjectInternal через Emit занимает 1,7 сек/100,000 записей (см. ниже button8)
// подмена работы Formatter + перебор массива занимает 0,5 сек/100,000 записей (см. ниже button9)
// простой перебор массива + вызов getter занимает 0,2 сек/100,000 записей (см. ниже button9)

// стандартный алгоритм форматирования данных смотри в System.Windows.Forms.Formatter.FormatObjectInternal
// Он базируется на 4-6 подходах:
// источник строка, источник IFormatable, есть ли converter, источник IConvertible можно ли использовать Convert.ChangeType) 

//Для фаст фильтра возможно нужно создать свой Конвертер, который будет :
// - форматирование данных
// - может ли форматировать данные (нужно для Check в DataReader)
// - (DropDownTextBox- ??? стоит ли делать, может проще - это реализовать в самом контроле) поддержка стандартных значений из Базы данных

using DGCore.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DGCore.Helpers;

namespace DGWnd.Utils
{
    public class CheckStateConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(bool) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return CheckState.Indeterminate;
            if (value is bool o)
                return o ? CheckState.Checked : CheckState.Unchecked;
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(bool) || base.CanConvertFrom(context, sourceType);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is CheckState o)
                return o == CheckState.Indeterminate ? (bool?)null : o == CheckState.Checked;
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class DGVColumnHelper : IDGColumnHelper
    {

        static DGVColumnHelper()
        {
            TypeDescriptor.AddAttributes(typeof(CheckState), new TypeConverterAttribute(typeof(CheckStateConverter)));
        }

        private int _method = -1;
        private string _format;
        private object _nullValue;
        private IFormatProvider _formatProvider;
        private TypeConverter _converter;
        private TypeConverter _converter1;// for image
        private Type _formattedValueType;
        private DataGridViewImageCellLayout _imageLayout = DataGridViewImageCellLayout.NotSet;

        private ValueConverter _valueConverter;

        public DGVColumnHelper(DataGridViewColumn dgvColumn)
        {
            this._formattedValueType = dgvColumn.CellTemplate.FormattedValueType;
            if (dgvColumn is DataGridViewImageColumn)
            {
                this._imageLayout = ((DataGridViewImageColumn)dgvColumn).ImageLayout;
                // if this._imageLayout equals to NotSet == Not image columns
                if (this._imageLayout == DataGridViewImageCellLayout.NotSet) this._imageLayout = DataGridViewImageCellLayout.Normal;
            }
            if (!String.IsNullOrEmpty(dgvColumn.DataPropertyName))
            {
                //      if (!String.IsNullOrEmpty(dgvColumn.DataPropertyName) && !(dgvColumn is DataGridViewCheckBoxColumn)) {
                PropertyDescriptorCollection pdc = DGVUtils.GetInternalPropertyDescriptorCollection(dgvColumn.DataGridView);
                PropertyDescriptor = pdc[dgvColumn.DataPropertyName];
                if (PropertyDescriptor != null)
                {
                    if (PropertyDescriptor is DGCore.PD.IMemberDescriptor)
                    {
                        _nullValue = ((DGCore.PD.IMemberDescriptor)PropertyDescriptor).DbNullValue;
                    }

                    //**          if (Utils.Types.IsNullableType(dgvColumn.ValueType)) _nullableConverter = TypeDescriptor.GetConverter(dgvColumn.ValueType);
                    Type valueType = DGCore.Utils.Types.GetNotNullableType(dgvColumn.ValueType);

                    this._format = dgvColumn.InheritedStyle.Format;
                    this._formatProvider = dgvColumn.InheritedStyle.FormatProvider;
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
                    else if (_formattedValueType == typeof(CheckState)) _method = 6;
                    //            if (dgvColumn.ValueType == typeof(CheckState)) _method = 6;
                    //        else if (Utils.Types.GetNotNullableType(dgvColumn.ValueType) == typeof(bool)) _method = 5;
                    //        else throw new Exception ("AAAA");
                    else if (_formattedValueType == typeof(Image) || _formattedValueType == typeof(Icon)) _method = 7;
                    else _method = 8;
                }
            }

            _valueConverter = new ValueConverter(PropertyDescriptor, _formattedValueType);
        }

        public void GetColumnSize(Graphics g, Font font, IEnumerable<object> items, out float colWidth, out float rowHeight, List<float> rowHeights)
        {
            colWidth = 0f;
            rowHeight = 0f;
            if (this.IsValid)
            {
                switch (this._imageLayout)
                {
                    case DataGridViewImageCellLayout.NotSet: // Not image column
                                                             // Get first not nullable value to define value type
                        object x = null;
                        foreach (object o in items)
                        {
                            x = PropertyDescriptor.GetValue(o);
                            if (x != null) break;
                        }
                        // analyze value type
                        if (x == null) return;// all values are null
                        if (x is CheckState || x is bool)
                        {
                            rowHeight = 18f;
                            colWidth = 18f;
                            return;
                        }
                        IEnumerable<object> e1 = System.Linq.Enumerable.Select<object, object>(items, delegate (object o1) { return PropertyDescriptor.GetValue(o1); });
                        IEnumerable<object> e2 = System.Linq.Enumerable.Distinct<object>(e1);
                        bool rowHeightFlag = true;
                        foreach (object o in e2)
                        {
                            object o1 = this.GetFormattedValueFromValue(o, false);
                            if (o1 != null)
                            {
                                if (o1 is string)
                                {
                                    SizeF size = g.MeasureString((string)o1, font);
                                    if (size.Width > colWidth) colWidth = size.Width;
                                    if (rowHeightFlag)
                                    {
                                        rowHeight = size.Height;
                                        rowHeightFlag = false;
                                    }
                                }
                                else
                                {
                                    throw new Exception("XXX");
                                }
                            }
                        }
                        break;
                    case DataGridViewImageCellLayout.Normal:
                        foreach (object x1 in items)
                        {
                            object x2 = PropertyDescriptor.GetValue(x1);
                            if (x2 != null)
                            {
                                Bitmap bm = (Bitmap)this.GetFormattedValueFromValue(x2, false);// must be Bitmap object
                                if (bm.Width > colWidth) colWidth = bm.Width;
                                rowHeights.Add(bm.Height);
                            }
                            else
                            {
                                rowHeights.Add(0f);
                            }
                        }
                        break;
                    default: // Stretched/Zoomed images
                        colWidth = -1f;
                        rowHeight = -1f;
                        break;
                }
            }
        }

        private object GetFormattedValueFromValue(object value, bool clipboardMode)
        {
            /*if (clipboardMode)
                return _valueConverter.GetClipboardString(value);
            else
                return _valueConverter.GetValue(value);*/

            if (value == null || !IsValid || (_nullValue != null && Equals(_nullValue, value)))
                return null;
            if (value.ToString().EndsWith("1890,1"))
            {

            }
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
                case 6:
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
                    }
                case 7: return this._converter1.ConvertFrom(value);
                default: return value.ToString();// case this._method = 4 or 8
            }
        }

        public override string ToString() => PropertyDescriptor.ToString();

        #region ==========  IDGColumnHelper  =============
        public PropertyDescriptor PropertyDescriptor { get; }
        public bool IsValid => this._method != -1;
        public object GetFormattedValueFromItem(object item, bool clipboardMode) => // numbers in clipboard mode is showing without format
          item == null || !IsValid ? null : GetFormattedValueFromValue(PropertyDescriptor.GetValue(item), clipboardMode);
        public bool Contains(object item, string searchString) => // Does formatted value of item contain searchString ?
                  GetFormattedValueFromItem(item, false)?.ToString().IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
        #endregion
    }
}
