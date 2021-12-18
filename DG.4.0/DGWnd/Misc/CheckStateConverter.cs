using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace DGWnd.Misc
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
}
