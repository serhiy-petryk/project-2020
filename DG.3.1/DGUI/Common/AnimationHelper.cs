using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DGUI.Mwi;

namespace DGUI.Common
{
    public static class AnimationHelper
    {
        public const int ANIMATION_DURATION_IN_MILLISECONDS = 120;
        public static Timeline GetWidthAnimation(FrameworkElement element, double from, double to, FillBehavior fillBehavior = FillBehavior.HoldEnd) =>
            GetFromToAnimation(element, FrameworkElement.WidthProperty, from, to, fillBehavior);
        public static Timeline GetHeightAnimation(FrameworkElement element, double from, double to, FillBehavior fillBehavior = FillBehavior.HoldEnd) =>
            GetFromToAnimation(element, FrameworkElement.HeightProperty, from, to, fillBehavior);
        public static Timeline GetLeftAnimation(FrameworkElement element, double from, double to, FillBehavior fillBehavior = FillBehavior.HoldEnd) =>
            GetFromToAnimation(element, Canvas.LeftProperty, from, to, fillBehavior);
        public static Timeline GetWindowLeftAnimation(Window element, double from, double to, FillBehavior fillBehavior = FillBehavior.HoldEnd) =>
            GetFromToAnimation(element, Window.LeftProperty, from, to, fillBehavior);
        public static Timeline GetTopAnimation(FrameworkElement element, double from, double to, FillBehavior fillBehavior = FillBehavior.HoldEnd) =>
            GetFromToAnimation(element, Canvas.TopProperty, from, to, fillBehavior);
        public static Timeline GetWindowTopAnimation(Window element, double from, double to, FillBehavior fillBehavior = FillBehavior.HoldEnd) =>
            GetFromToAnimation(element, Window.TopProperty, from, to, fillBehavior);
        public static Timeline GetOpacityAnimation(FrameworkElement element, double from, double to, FillBehavior fillBehavior = FillBehavior.HoldEnd) =>
            GetFromToAnimation(element, UIElement.OpacityProperty, from, to, fillBehavior);
        public static Timeline GetPositionAnimation(MwiChild element, Point from, Point to, FillBehavior fillBehavior = FillBehavior.HoldEnd) =>
            GetFromToAnimation(element, MwiChild.PositionProperty, from, to, fillBehavior);
        public static LinearGradientBrush RunLinearGradientBrushAnimation(LinearGradientBrush newBrush, LinearGradientBrush oldBrush)
        {
            // usage: tabItem.Background = AnimationHelper.RunLinearGradientBrushAnimation(newBrush, (LinearGradientBrush)tabItem.Background);
            if (newBrush.GradientStops.Count != oldBrush.GradientStops.Count)
                throw new Exception("RunLinearGradientBrushAnimation error! Different size of newBrush and oldBrush GradientStops collection");

            newBrush = newBrush.Clone();
            for (var k = 0; k < newBrush.GradientStops.Count; k++)
            {
                var ca = new ColorAnimation(oldBrush.GradientStops[k].Color, newBrush.GradientStops[k].Color, TimeSpan.FromMilliseconds(ANIMATION_DURATION_IN_MILLISECONDS));
                newBrush.GradientStops[k].BeginAnimation(GradientStop.ColorProperty, ca);
            }
            return newBrush;
        }

        public static TimelineCollection[] GetPathAnimations(Path element, Geometry from, Geometry to)
        {
            var result = new[] { new TimelineCollection(), new TimelineCollection() };
            result[0].Add(GetFrameAnimation(element, Path.DataProperty, to));
            result[1].Add(GetFrameAnimation(element, Path.DataProperty, from));

            var widthPart1Animation = GetWidthAnimation(element, element.Width, 0);
            var widthPart2Animation = GetWidthAnimation(element, 0, element.Width);
            widthPart2Animation.BeginTime = TimeSpan.FromMilliseconds(ANIMATION_DURATION_IN_MILLISECONDS);
            widthPart2Animation.Duration = TimeSpan.FromMilliseconds(ANIMATION_DURATION_IN_MILLISECONDS * 2);
            result[0].Add(widthPart1Animation);
            result[0].Add(widthPart2Animation);
            result[1].Add(widthPart1Animation);
            result[1].Add(widthPart2Animation);

            return result;
        }

        //====================   Private section   ===================
        private static Timeline GetFromToAnimation(FrameworkElement element, DependencyProperty dataProperty, object from, object to, FillBehavior fillBehavior = FillBehavior.HoldEnd)
        {
            if (element == null || dataProperty == null || from == null || to == null)
                throw new NullReferenceException();
            if (!dataProperty.PropertyType.IsInstanceOfType(from) || !dataProperty.PropertyType.IsInstanceOfType(to))
                throw new Exception("GetFromToAnimation error. Data type of 'from'/'to' parameter doesn't match dataProperty.PropertyType.");

            AnimationTimeline animation = null;
            if (dataProperty.PropertyType == typeof(double))
                animation = new DoubleAnimation { From = (double)from, To = (double)to };
            else if (dataProperty.PropertyType == typeof(Point))
                animation = new PointAnimation { From = (Point)from, To = (Point)to };

            if (animation != null)
            {
                animation.Duration = TimeSpan.FromMilliseconds(ANIMATION_DURATION_IN_MILLISECONDS);
                animation.FillBehavior = fillBehavior;
                Storyboard.SetTarget(animation, element);
                Storyboard.SetTargetProperty(animation, new PropertyPath(dataProperty));
                return animation;
            }

            throw new NotImplementedException();
        }

        private static ObjectAnimationUsingKeyFrames GetFrameAnimation(FrameworkElement element, DependencyProperty dataProperty, object value)
        {
            if (element == null || dataProperty == null || value == null)
                throw new NullReferenceException();
            if (!dataProperty.PropertyType.IsInstanceOfType(value))
                throw new Exception("GetFrameAnimation error. Data type of 'value' parameter doesn't match dataProperty.PropertyType.");

            var animation = new ObjectAnimationUsingKeyFrames();
            animation.KeyFrames.Add(new DiscreteObjectKeyFrame { Value = value, KeyTime = TimeSpan.FromMilliseconds(ANIMATION_DURATION_IN_MILLISECONDS) });

            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, new PropertyPath(dataProperty));
            return animation;
        }

    }
}
