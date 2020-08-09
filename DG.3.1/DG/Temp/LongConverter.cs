using System;
using System.Windows.Data;

namespace DG.Temp
{
    [ValueConversion(typeof(long), typeof(string))]
    public class LongConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            // Возвращаем строку в формате 123.456.789 руб.
            return ((long)value).ToString("#,###", culture) + " руб.";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            long result;
            if (long.TryParse(value.ToString(), System.Globalization.NumberStyles.Any,
                culture, out result))
            {
                return result;
            }
            else if (long.TryParse(value.ToString().Replace(" руб.", ""), System.Globalization.NumberStyles.Any,
                culture, out result))
            {
                return result;
            }
            return value;
        }
    }
}
