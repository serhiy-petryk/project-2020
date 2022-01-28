﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WpfSpLib.Helpers
{
    public static class VisualHelper
    {
        public static IEnumerable<DependencyObject> GetVisualParents(this DependencyObject current)
        {
            while (current != null)
            {
                yield return current;
                current = VisualTreeHelper.GetParent(current) ?? (current as FrameworkElement)?.Parent;
            }
        }

        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject current)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
            {
                var child = VisualTreeHelper.GetChild(current, i);
                yield return child;

                foreach (var childOfChild in GetVisualChildren(child))
                    yield return childOfChild;
            }
        }

        public static bool IsMouseOverElement(this FrameworkElement element, Func<IInputElement, Point> getPositionOfMouse)
        {
            var p = getPositionOfMouse(element);
            var bounds = GetBoundsOfElement(element);
            return bounds.Contains(p);
        }

        public static Rect GetBoundsOfElement(this FrameworkElement element) => new Rect(-element.Margin.Left, -element.Margin.Top, element.ActualWidth + element.Margin.Left + element.Margin.Right, element.ActualHeight + element.Margin.Top + element.Margin.Bottom);

        public static Rect GetVisibleRect(this FrameworkElement item, Visual visualAncestor)
        {
            // based on https://social.msdn.microsoft.com/Forums/vstudio/en-US/b57531cc-fdb1-4d0e-9650-324630343d62/screen-rectangle-for-visible-part-of-wpf-uielement-windows-forms-wpf-interop?forum=wpf
            // Visual _rootVisual = HwndSource.FromVisual(item).RootVisual;
            var transformToRoot = item.TransformToAncestor(visualAncestor);
            // Rect screenRect = new Rect(transformToRoot.Transform(new Point(0, 0)), transformToRoot.Transform(new Point(item.ActualWidth, item.ActualHeight)));
            var screenRect = transformToRoot.TransformBounds(GetBoundsOfElement(item));
            var parent = VisualTreeHelper.GetParent(item);
            while (parent != null && parent != visualAncestor)
            {
                var visual = parent as Visual;
                var element = parent as FrameworkElement;
                if (visual != null && element != null)
                {
                    transformToRoot = visual.TransformToAncestor(visualAncestor);
                    var pointAncestorTopLeft = transformToRoot.Transform(new Point(0, 0));
                    var pointAncestorBottomRight = transformToRoot.Transform(new Point(element.ActualWidth, element.ActualHeight));
                    var ancestorRect = new Rect(pointAncestorTopLeft, pointAncestorBottomRight);
                    screenRect.Intersect(ancestorRect);
                }
                parent = VisualTreeHelper.GetParent(parent);
            }

            // at this point screenRect is the bounding rectangle for the visible portion of "this" element
            return screenRect;
        }


    }
}