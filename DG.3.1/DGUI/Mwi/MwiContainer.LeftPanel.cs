using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DGUI.Common;

namespace DGUI.Mwi
{
    public partial class MwiContainer
    {
        public static readonly DependencyProperty LeftPanelProperty = DependencyProperty.Register("LeftPanel", typeof(UIElement), typeof(MwiContainer), new FrameworkPropertyMetadata(null, OnLeftPanelPropertyChanged));
        public UIElement LeftPanel
        {
            get => (UIElement)GetValue(LeftPanelProperty);
            set => SetValue(LeftPanelProperty, value);
        }

        public void HideLeftPanel() => LeftPanelButton.IsChecked = false;

        private static void OnLeftPanelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is UIElement && d is MwiContainer)
                ((UIElement)e.NewValue).SetValue(MwiContainerProperty, d);
        }

        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var newWidth = LeftPanelContainer.ActualWidth + e.HorizontalChange;
            if (newWidth >= 0)
                LeftPanelContainer.Width = newWidth;
        }

        private void LeftPanelButton_IsChecked_OnChanged(object sender, RoutedEventArgs e)
        {
            var button = (ToggleButton) sender;
            var sbs = button.Resources["Animation"] as Storyboard[];
            if (sbs == null)
            {
                PrepareToggleButtonAnimation(button);
                sbs = (Storyboard[])button.Resources["Animation"];

                var startWidth = Math.Max(LeftPanelContainer.ActualWidth, 10);
                sbs[0].Children.Insert(0, AnimationHelper.GetWidthAnimation(LeftPanelContainer, 0.0, startWidth, FillBehavior.Stop));
                sbs[0].Children.Add(AnimationHelper.GetOpacityAnimation(LeftPanelContainer, 0.0, 1));
                sbs[0].Children.Add(AnimationHelper.GetOpacityAnimation(MwiCanvas, 1.0, 0.8));

                sbs[1].Children.Insert(0, AnimationHelper.GetWidthAnimation(LeftPanelContainer, 0.0, 0.0));
                sbs[1].Children.Add(AnimationHelper.GetOpacityAnimation(MwiCanvas, 0.8, 1));
                sbs[1].Children[0].Completed += (o, args) => LeftPanelContainer.Visibility = Visibility.Hidden;
            }

            if (button.IsChecked == true)
            {
                LeftPanelContainer.Visibility = Visibility.Visible;
                sbs[0].Begin();
            }
            else
            {
                ((DoubleAnimation)sbs[0].Children[0]).To = LeftPanelContainer.ActualWidth;
                ((DoubleAnimation)sbs[1].Children[0]).From = LeftPanelContainer.ActualWidth;
                sbs[1].Begin();
            }
        }

        private void PrepareToggleButtonAnimation(ToggleButton button)
        {
            var sbFirst = new Storyboard();
            var sbSecond = new Storyboard();

            var geometries = (Geometry[])button.Tag;
            var buttonPath = Tips.FindVisualChildren<Path>(button).First();
            var aa = AnimationHelper.GetPathAnimations(buttonPath, geometries[0], geometries[1]);
            foreach (var a in aa[0])
                sbFirst.Children.Add(a);
            foreach (var a in aa[1])
                sbSecond.Children.Add(a);
            button.Resources.Add("Animation", new[] {sbFirst, sbSecond});
        }
    }

}
