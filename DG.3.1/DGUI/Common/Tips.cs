using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Xaml;

namespace DGUI.Common
{
    public class Tips
    {
        public const double SCREEN_TOLERANCE = 0.001;
        private static Rect? _maximizedWindowRectangle;

        public  static Action EmptyDelegate = delegate { };

        public static string SerializeToXaml(object o) => XamlServices.Save(o);
        public static object DeserializeToXaml(string xamlContent) => XamlServices.Parse(xamlContent);

        public static Rect GetMaximizedWindowRectangle()
        {
            if (!_maximizedWindowRectangle.HasValue)
            {
                var window = new Window {WindowState = WindowState.Maximized};
                window.Show();
                var delta = Math.Min(0, Math.Min(window.Left, window.Top));
                _maximizedWindowRectangle = new Rect(window.Left - delta, window.Top - delta, window.ActualWidth + 2 * delta, window.ActualHeight + 2 * delta);
                window.Close();
            }
            return _maximizedWindowRectangle.Value;
        }

        public static bool AreEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d2) < SCREEN_TOLERANCE;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject current) where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
            {
                var child = VisualTreeHelper.GetChild(current, i);
                if (child is T)
                    yield return (T)child;

                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }

        public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject current) where T : DependencyObject // not checked
        {
            foreach (var child in LogicalTreeHelper.GetChildren(current))
                if (child is DependencyObject)
                {
                    if (child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindLogicalChildren<T>((DependencyObject)child))
                        yield return childOfChild;
                }
        }

        public static bool IsTextTrimmed(FrameworkElement textBlock)
        {
            textBlock.Measure(new Size(double.PositiveInfinity, height: double.PositiveInfinity));
            return (textBlock.ActualWidth + textBlock.Margin.Left + textBlock.Margin.Right) < textBlock.DesiredSize.Width ||
                   (textBlock.ActualHeight + textBlock.Margin.Top + textBlock.Margin.Bottom) < textBlock.DesiredSize.Height;
        }
    }
}
