using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xaml;
using DGView.Controls.DialogItems;
using DGView.Mwi;
using DGView.ViewModels;

namespace DGView.Common
{
    public class Tips
    {
        public static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
        public static CultureInfo CurrentCulture => Thread.CurrentThread.CurrentCulture;

        public const double SCREEN_TOLERANCE = 0.001;
        private static Rect? _maximizedWindowRectangle;

        public static Action EmptyDelegate = delegate { };

        public static string SerializeToXaml(object o) => XamlServices.Save(o);
        public static object DeserializeFromXaml(string xamlContent) => XamlServices.Parse(xamlContent);

        public static Rect GetMaximizedWindowRectangle()
        {
            if (!_maximizedWindowRectangle.HasValue)
            {
                var window = new Window { WindowState = WindowState.Maximized };
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

        public static IEnumerable<DependencyObject> GetVisualParents(DependencyObject current)
        {
            while (current != null)
            {
                yield return current;
                current = VisualTreeHelper.GetParent(current);
            }
        }

        public static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject current)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
            {
                var child = VisualTreeHelper.GetChild(current, i);
                yield return child;

                foreach (var childOfChild in GetVisualChildren(child))
                    yield return childOfChild;
            }
        }

        public static IEnumerable<object> GetLogicalChildren(DependencyObject current) // not checked
        {
            foreach (var child in LogicalTreeHelper.GetChildren(current))
            {
                yield return child;
                if (child is DependencyObject)
                    yield return GetLogicalChildren((DependencyObject)child);
            }
        }

        public static bool IsTextTrimmed(FrameworkElement textBlock)
        {
            textBlock.Measure(new Size(double.PositiveInfinity, height: double.PositiveInfinity));
            return (textBlock.ActualWidth + textBlock.Margin.Left + textBlock.Margin.Right) < textBlock.DesiredSize.Width ||
                   (textBlock.ActualHeight + textBlock.Margin.Top + textBlock.Margin.Bottom) < textBlock.DesiredSize.Height;
        }

        #region =============  Colors  =============
        public static Brush GetActualBackgroundBrush(DependencyObject d)
        {
            // valid only for SolidColorBrush
            foreach (var c in GetVisualParents(d).Where(a1 => a1 is Control || a1 is Panel))
            {
                var brush = c is Control ? ((Control)c).Background : ((Panel)c).Background;
                if (brush is SolidColorBrush)
                {
                    var color = ((SolidColorBrush)brush).Color;
                    if (color != Colors.Transparent)
                        return brush;
                }
                else if (brush != null)
                    return brush;
            }
            return null;
        }
        public static Color GetActualBackgroundColor(DependencyObject d)
        {
            var color = GetColorFromBrush(GetActualBackgroundBrush(d));
            return color;
        }
        public static Color GetColorFromBrush(Brush brush)
        {
            if (brush is SolidColorBrush)
                return ((SolidColorBrush)brush).Color;
            if (brush is LinearGradientBrush)
            {
                var gcs = ((LinearGradientBrush)brush).GradientStops;
                var color = gcs[gcs.Count / 2].Color;
                return color;
            }
            return Colors.Transparent;
        }

        public static Brush GetActualForegroundBrush(DependencyObject d)
        {
            // valid only for SolidColorBrush
            foreach (var c in GetVisualParents(d).OfType<Control>())
            {
                var brush = c.Foreground;
                if (brush is SolidColorBrush)
                {
                    if (((SolidColorBrush)brush).Color != Colors.Transparent)
                        return brush;
                }
                else if (brush != null)
                    return brush;
            }
            return null;
        }
        #endregion

        // ===================================
        public static void ShowMwiChildDialog(UIElement content, string title, Size? size = null)
        {
            var dialog = new MwiChild
            {
                Title = title,
                Content = content,
                Width = size?.Width ?? double.NaN,
                Height = size?.Height ?? double.NaN
            };

            var style = dialog.TryFindResource("MovableDialogStyle") as Style;
            DialogItems.ShowDialog(AppViewModel.Instance.ContainerControl.ContainerForDialog, dialog, style,
                GetAfterCreationCallbackForDialog(dialog, true));
        }
        private static Action<DialogItems> GetAfterCreationCallbackForDialog(FrameworkElement content, bool closeOnClickBackground)
        {
            return dialogItems =>
            {
                dialogItems.CloseOnClickBackground = closeOnClickBackground;

                // center content position
                if (double.IsNaN(content.Width) || double.IsNaN(content.Height))
                    content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                var mwiChild = (MwiChild)dialogItems.Items[0];
                mwiChild.Focused = true;
                mwiChild.Position = new Point(
                    Math.Max(0,
                        (dialogItems.ItemsPresenter.ActualWidth -
                         (double.IsNaN(content.Width) ? content.DesiredSize.Width : content.Width)) / 2),
                    Math.Max(0,
                        (dialogItems.ItemsPresenter.ActualHeight - (double.IsNaN(content.Height)
                             ? content.DesiredSize.Height
                             : content.Height)) / 2));
            };
        }

        #region =============  Type Utilities ==============
        // 
        public static Type GetNotNullableType(Type type) => IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type;
        public static bool IsNullableType(Type type) => type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static object GetDefaultOfType(Type type)
        {
            if (type == typeof(DateTime))
                return DateTime.Today;

            var currentType = MethodBase.GetCurrentMethod().DeclaringType;
            var method = currentType.GetMethod("GetDefaultGeneric", BindingFlags.Static | BindingFlags.NonPublic);
            return method.MakeGenericMethod(type).Invoke(null, null);
        }
        private static T GetDefaultGeneric<T>() => default(T);
        #endregion
    }
}
