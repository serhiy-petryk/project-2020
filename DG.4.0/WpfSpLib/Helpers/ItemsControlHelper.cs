using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace WpfSpLib.Helpers
{
    public static class ItemsControlHelper
    {
        public static async Task AddOrReorderItem(this ItemsControl control, object item, int newIndex)
        {
            var items = (IList)(control.ItemsSource ?? control.Items);
            var oldIndex = items.IndexOf(item);
            if (Equals(oldIndex, newIndex)) return;

            // To prevent flicker during drop item
            control.ItemContainerGenerator.StatusChanged += OnItemContainerGeneratorStatusChanged;

            FrameworkElement element;
            if (oldIndex >= 0)
            {
                element = control.ItemContainerGenerator.ContainerFromIndex(oldIndex) as FrameworkElement;
                if (element != null)
                    await Task.WhenAll(AnimationHelper.GetHeightContentAnimations(element, false));
                items.RemoveAt(oldIndex);
                element?.SetCurrentValueSmart(FrameworkElement.HeightProperty, double.NaN);
            }

            if (item is TabItem tabItem)
                ((TabControl)tabItem.Parent)?.Items.Remove(tabItem);
            items.Insert(newIndex, item);

            VisualHelper.DoEvents(DispatcherPriority.Render);

            element = control.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
            if (element != null)
            {
                element.SetCurrentValueSmart(UIElement.OpacityProperty, 1.0);
                await Task.WhenAll(AnimationHelper.GetHeightContentAnimations(element, true));
                element.SetCurrentValueSmart(FrameworkElement.HeightProperty, double.NaN);
            }

            control.ItemContainerGenerator.StatusChanged -= OnItemContainerGeneratorStatusChanged;

            void OnItemContainerGeneratorStatusChanged(object sender, EventArgs e)
            {
                var generator = (ItemContainerGenerator)sender;
                if (generator.Status != GeneratorStatus.ContainersGenerated) return;
                foreach (var item2 in generator.Items)
                    if (generator.ContainerFromItem(item2) is FrameworkElement fe && fe.Opacity > double.Epsilon && fe.ActualHeight < double.Epsilon && fe.ActualWidth < double.Epsilon)
                        fe.SetCurrentValueSmart(UIElement.OpacityProperty, 0.0);
            }
        }
    }
}
