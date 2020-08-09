using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xaml;

namespace DG.Common
{
    public static class WpfUtils
    {
        public static IEnumerable<T> GetChildOfType<T>(DependencyObject o) where T : DependencyObject
        {
            if (o != null)
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
                {
                    var child = VisualTreeHelper.GetChild(o, i);
                    if (child is T)
                        yield return (T)child;
                    foreach (var a in GetChildOfType<T>(child))
                        yield return a;
                }
        }

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

        public static void SaveResourceToFile(ResourceDictionary resource, string resourceId)
        {
            using (var writer = new StreamWriter("Themes\\" + resourceId + ".xaml"))
                System.Windows.Markup.XamlWriter.Save(resource, writer);
        }

        public static string SerializeToXaml(object o) => XamlServices.Save(o);
        public static object DeserializeToXaml(string xamlContent) => XamlServices.Parse(xamlContent);

        // public static SolidColorBrush GetBrushFromHexColor(string hexColor) => (SolidColorBrush)(new BrushConverter().ConvertFrom(hexColor));
        public static SolidColorBrush GetBrushFromHexColor(string hexColor) =>
          new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor));
        public static SolidColorBrush GetLinearGradientBrushFromHexColor(string hexColor) => (SolidColorBrush)(new BrushConverter().ConvertFrom(hexColor));
    }
}
