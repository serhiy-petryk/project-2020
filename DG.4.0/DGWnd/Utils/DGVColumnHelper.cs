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
using System.Windows.Forms;
using DGCore.Helpers;
using DGCore.PD;

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

    public class DGVColumnHelper
    {
        static DGVColumnHelper()
        {
            TypeDescriptor.AddAttributes(typeof(CheckState), new TypeConverterAttribute(typeof(CheckStateConverter)));
        }


        public DGCellValueFormatter ValueFormatter;
        public PropertyDescriptor PropertyDescriptor { get; }

        private bool _isValid => PropertyDescriptor != null;
        private DataGridViewImageCellLayout _imageLayout = DataGridViewImageCellLayout.NotSet;

        public DGVColumnHelper(DataGridViewColumn dgvColumn)
        {
            if (dgvColumn is DataGridViewImageColumn)
            {
                _imageLayout = ((DataGridViewImageColumn)dgvColumn).ImageLayout;
                // if this._imageLayout equals to NotSet == Not image columns
                if (this._imageLayout == DataGridViewImageCellLayout.NotSet) this._imageLayout = DataGridViewImageCellLayout.Normal;
            }
            var pdc = DGVUtils.GetInternalPropertyDescriptorCollection(dgvColumn.DataGridView);
            PropertyDescriptor = pdc[dgvColumn.DataPropertyName];
            ValueFormatter = new DGCellValueFormatter((IMemberDescriptor)PropertyDescriptor);
        }

        public void GetColumnSize(Graphics g, Font font, IEnumerable<object> items, out float colWidth, out float rowHeight, List<float> rowHeights)
        {
            colWidth = 0f;
            rowHeight = 0f;
            if (_isValid)
            {
                switch (this._imageLayout)
                {
                    case DataGridViewImageCellLayout.NotSet: // Not image column
                        var propertyType = Types.GetNotNullableType(PropertyDescriptor.PropertyType);
                        if ( propertyType == typeof(bool))
                        {
                            rowHeight = 18f;
                            colWidth = 18f;
                            return;
                        }

                        var rowHeightFlag = true;
                        var values = ValueFormatter.GetUniqueStrings(items);
                        foreach (var o in values)
                        {
                            if (o != null)
                            {
                                var size = g.MeasureString((string)o, font);
                                if (size.Width > colWidth) colWidth = size.Width;
                                if (rowHeightFlag)
                                {
                                    rowHeight = size.Height;
                                    rowHeightFlag = false;
                                }
                            }
                        }

                        break;
                    case DataGridViewImageCellLayout.Normal:
                        foreach (var item in items)
                        {
                            var value = ValueFormatter.GetValueForPrinterFromItem(item);
                            if (value == null)
                                rowHeights.Add(0x0F);
                            else
                            {
                                var bm = (Bitmap)Tips.ByteArrayToBitmapConverter.ConvertFrom(value);
                                if (bm.Width > colWidth) colWidth = bm.Width;
                                rowHeights.Add(bm.Height);
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

        public override string ToString() => PropertyDescriptor.ToString();
    }
}
