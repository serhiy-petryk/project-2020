using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace WpfSpLib.Helpers
{
    public static class ItemsControlHelper
    {
        public static async Task AddOrReorderItems(this ItemsControl control, object[] sourceData, int insertIndex)
        {
            var items = GetItemsHost(control).Children.OfType<FrameworkElement>().ToArray();
            var itemsSource = (IList)(control.ItemsSource ?? control.Items);

            // To prevent flicker during drop item
            control.ItemContainerGenerator.StatusChanged += OnItemContainerGeneratorStatusChanged;

            var animations = new List<Task>();
            var elements = new List<FrameworkElement>();
            foreach (var item in sourceData)
            {
                var oldIndex = itemsSource.IndexOf(item);
                if (oldIndex >= 0)
                {
                    var insertingElement = items.FirstOrDefault(o => o.DataContext == item);
                    if (insertingElement != null)
                    {
                        elements.Add(insertingElement);
                        animations.AddRange(AnimationHelper.GetHeightContentAnimations(insertingElement, false));
                    }
                }
            }

            await Task.WhenAll(animations.ToArray());
            foreach (var element in elements)
                element.SetCurrentValueSmart(FrameworkElement.HeightProperty, double.NaN);

            foreach (var item in sourceData)
            {
                var oldIndex = itemsSource.IndexOf(item);
                if (oldIndex >= 0)
                {
                    if (oldIndex < insertIndex) insertIndex--;
                    itemsSource.RemoveAt(oldIndex);
                }

                if (item is TabItem tabItem)
                    ((TabControl)tabItem.Parent)?.Items.Remove(tabItem);
                itemsSource.Insert(insertIndex++, item);
            }

            VisualHelper.DoEvents(DispatcherPriority.Background);

            animations.Clear();
            elements.Clear();
            foreach (var item in sourceData)
            {
                if (control.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement itemVisual)
                {
                    itemVisual.SetCurrentValueSmart(UIElement.OpacityProperty, 1.0);
                    animations.AddRange(AnimationHelper.GetHeightContentAnimations(itemVisual, true));
                    elements.Add(itemVisual);
                }
            }

            await Task.WhenAll(animations);
            foreach (var element in elements)
                element.SetCurrentValueSmart(FrameworkElement.HeightProperty, double.NaN);

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

        public static async Task AddOrReorderItem(this ItemsControl control, object item, int newIndex)
        {
            var itemsSource = (IList)(control.ItemsSource ?? control.Items);
            var oldIndex = itemsSource.IndexOf(item);
            if (Equals(oldIndex, newIndex)) return;

            // To prevent flicker during drop item
            control.ItemContainerGenerator.StatusChanged += OnItemContainerGeneratorStatusChanged;

            FrameworkElement element;
            if (oldIndex >= 0)
            {
                // var visualItems = GetItemsHost(control).Children.OfType<FrameworkElement>();
                // element = visualItems.FirstOrDefault(o => o.DataContext == item);
                element = control.ItemContainerGenerator.ContainerFromIndex(oldIndex) as FrameworkElement;
                if (element != null)
                    await Task.WhenAll(AnimationHelper.GetHeightContentAnimations(element, false));
                itemsSource.RemoveAt(oldIndex);
                element?.SetCurrentValueSmart(FrameworkElement.HeightProperty, double.NaN);
            }

            if (item is TabItem tabItem)
                ((TabControl)tabItem.Parent)?.Items.Remove(tabItem);
            itemsSource.Insert(newIndex, item);

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

        private static PropertyInfo _piItemsHost;
        public static Panel GetItemsHost(this ItemsControl itemsControl)
        {
            if (_piItemsHost == null)
                _piItemsHost = typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return (Panel)_piItemsHost.GetValue(itemsControl);
        }

        private static PropertyInfo _piDataGridHeaderHost;
        public static FrameworkElement GetHeaderHost(this ItemsControl itemsControl)
        {
            if (itemsControl is DataGrid)
            {
                if (_piDataGridHeaderHost == null)
                    _piDataGridHeaderHost = typeof(DataGrid).GetProperty("ColumnHeadersPresenter", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                return (DataGridColumnHeadersPresenter)_piDataGridHeaderHost.GetValue(itemsControl);
            }
            return null;
        }

        public static IEnumerable<ElementOfItemsControl> GetAllVisualItems(this ItemsControl control)
        {
            var panel = GetItemsHost(control);
            for (var i = 0; i < panel.Children.Count; i++)
            {
                if (panel.Children[i] is FrameworkElement element)
                {
                    yield return new ElementOfItemsControl(element, control);
                    if (element is ItemsControl itemsControl) // TreeViewItem
                    {
                        foreach (var childItem in GetAllVisualItems(itemsControl))
                            yield return childItem;
                    }
                }
            }
        }


        #region =============  Helper classes  ===============
        public class ElementOfItemsControl
        {
            public ItemsControl ItemsControl { get; }
            public FrameworkElement VisualElement { get; }

            public ElementOfItemsControl(FrameworkElement element, ItemsControl itemsControl)
            {
                ItemsControl = itemsControl;
                VisualElement = element;
                if (element is HeaderedItemsControl control) // TreeViewItem
                {
                    var aa1 = control.GetVisualChildren().OfType<ContentPresenter>().Where(o => o.ContentSource == "Header");
                    var aa2 = aa1.Where(o => o.GetVisualParents().OfType<ItemsControl>().FirstOrDefault() == element).ToArray();
                    if (aa2.Length == 1)
                        VisualElement = aa2[0];
                }
            }
        }
        #endregion
    }
}
