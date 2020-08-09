using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace DGView.Controls
{
    public static class ControlHelper
    {
        public static double GetListWidth(ItemsControl control, double additionalSpace = 24)
        {
            var listWidth = 0.0;
            PropertyInfo propertyInfo = null;
            foreach (var item in control.Items)
            {
                if (propertyInfo == null && !string.IsNullOrEmpty(control.DisplayMemberPath))
                    propertyInfo = item.GetType().GetProperty(control.DisplayMemberPath);

                var prompt = propertyInfo == null ? item.ToString() : propertyInfo.GetValue(item).ToString();
                var formattedText = new FormattedText(
                    prompt,
                    Thread.CurrentThread.CurrentCulture,
                    control.FlowDirection,
                    new Typeface(control.FontFamily, control.FontStyle, control.FontWeight, control.FontStretch),
                    control.FontSize,
                    control.Foreground,
                    new NumberSubstitution());
                if (formattedText.Width > listWidth)
                    listWidth = formattedText.Width;
            }
            return listWidth + additionalSpace;
        }
    }
}
