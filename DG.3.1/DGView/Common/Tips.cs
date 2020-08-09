﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xaml;

namespace DGView.Common
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
                    yield return GetLogicalChildren((DependencyObject) child);
            }
        }

        public static bool IsTextTrimmed(FrameworkElement textBlock)
        {
            textBlock.Measure(new Size(double.PositiveInfinity, height: double.PositiveInfinity));
            return (textBlock.ActualWidth + textBlock.Margin.Left + textBlock.Margin.Right) < textBlock.DesiredSize.Width ||
                   (textBlock.ActualHeight + textBlock.Margin.Top + textBlock.Margin.Bottom) < textBlock.DesiredSize.Height;
        }

        //=============================
        public static Brush GetActualForegroundBrush(DependencyObject d)
        {
            // valid only for SolidColorBrush
            foreach (var o in GetVisualParents(d).Where(a1 => a1 is Control))
            {
                var c = (Control)o;
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

    }
}
