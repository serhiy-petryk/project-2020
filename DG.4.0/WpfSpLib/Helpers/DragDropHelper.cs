﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfSpLib.Helpers
{
    public static class DragDropHelper
    {
        public static readonly StartDragInfo StartDrag_Info = new StartDragInfo();
        public static readonly DragInfo Drag_Info = new DragInfo();

        #region ==============  Examples of event handlers  ==============
        public static void DragSource_OnPreviewMouseMove(object sender, MouseEventArgs e, string dragDropFormat = null, Func<object[], bool> allowDrag = null)
        {
            if (_isDragging) return;
            if (!(sender is ItemsControl itemsControl) || e.LeftButton == MouseButtonState.Released)
            {
                StartDrag_Info.Clear(null); // (args = null) => !!! important for row selection after drop in GridView
                return;
            }

            var selectedItems = GetSelectedItems();
            var itemsHost = GetItemsHost(itemsControl);
            if (!itemsHost.IsMouseOverElement(e.GetPosition))
            {
                StartDrag_Info.Clear(e);
                return;
            }

            if (selectedItems.Select(o => (itemsControl.ItemContainerGenerator.ContainerFromItem(o) as FrameworkElement)
                    ?.IsMouseOverElement(e.GetPosition)).All(o => o != true) &&
                itemsControl.GetElementsUnderMouse(e.GetPosition).OfType<FrameworkElement>()
                    .FirstOrDefault(o => selectedItems.Contains(o.DataContext)) == null)
            {
                StartDrag_Info.Clear(e);
                return;
            }

            if (allowDrag != null && !allowDrag(selectedItems))
            {
                StartDrag_Info.Clear(e);
                return;
            }

            if (!Equals(itemsControl, StartDrag_Info.DragSource))
            {
                StartDrag_Info.Init(itemsControl, e);
                return;
            }

            var mousePosition = e.GetPosition(itemsControl);
            if (Math.Abs(mousePosition.X - StartDrag_Info.DragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(mousePosition.Y - StartDrag_Info.DragStart.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                if (itemsControl is DataGrid dataGrid)
                    dataGrid.CommitEdit();

                var dataObject = new DataObject();
                dataObject.SetData(dragDropFormat ?? sender.GetType().Name, selectedItems);
                try
                {
                    _isDragging = true;
                    var result = DragDrop.DoDragDrop(itemsControl, dataObject, DragDropEffects.Copy);
                }
                finally
                {
                    _isDragging = false;
                    StartDrag_Info.Clear(e);
                    Drag_Info.Clear();
                    ResetDragDrop(null);
                }
            }

            object[] GetSelectedItems()
            {
                if (itemsControl is MultiSelector multiSelector)
                    return multiSelector.SelectedItems.OfType<object>().ToArray();

                if (itemsControl is Selector selector)
                {
                    if (selector.SelectedItem != null)
                        return new[] { selector.SelectedItem };
                    return new object[0];
                }

                if (itemsControl is TreeView treeView)
                {
                    if (treeView.SelectedItem != null)
                        return new[] { treeView.SelectedItem };
                    return new object[0];
                }

                throw new Exception($"Trap! {itemsControl.GetType().Name} is not supported");
            }
        }

        public static void DragSource_OnPreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effects == DragDropEffects.Copy)
            {
                e.UseDefaultCursors = false;
                Mouse.SetCursor(Cursors.Arrow);
            }
            else
                e.UseDefaultCursors = true;

            e.Handled = true;
        }

        public static void DropTarget_OnPreviewDragOver(object sender, DragEventArgs e, IEnumerable<string> dragDropFormats = null,
            Action<object[], ItemsControl, DragEventArgs> afterDefiningInsertIndex = null)
        {
            Drag_Info.LastDragLeaveObject = null;
            var dragData = GetDragData(sender, e, dragDropFormats ?? new[] { sender.GetType().Name });
            if (dragData.Length == 0)
            {
                ResetDragDrop(e);
                return;
            }

            var control = (ItemsControl)sender;
            DefineInsertIndex(control, e);

            if (Drag_Info.InsertIndex.HasValue && Drag_Info.DragDropEffect != DragDropEffects.None)
                afterDefiningInsertIndex?.Invoke(dragData, control, e);

            if (!Drag_Info.InsertIndex.HasValue || Drag_Info.DragDropEffect == DragDropEffects.None)
            {
                ResetDragDrop(e);
                return;
            }

            if (afterDefiningInsertIndex == null && Drag_Info.IsBottomOrRightEdge)
            {  // default afterDefiningInsertIndex method
                Drag_Info.InsertIndex++;
                Drag_Info.IsBottomOrRightEdge = false;
            }

            if (_dropTargetAdorner == null)
                _dropTargetAdorner = new DropTargetInsertionAdorner(control);
            _dropTargetAdorner.InvalidateVisual();

            if (_dragAdorner == null)
                _dragAdorner = new DragAdorner(Window.GetWindow(control).Content as UIElement, dragData);
            _dragAdorner.UpdateUI(e, control);

            CheckScroll(control, e);
        }

        public static void DropTarget_OnPreviewDragLeave(object sender, DragEventArgs e)
        {
            Drag_Info.LastDragLeaveObject = sender;
            ((FrameworkElement)sender).Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Equals(Drag_Info.LastDragLeaveObject, sender))
                    ResetDragDrop(e);
                Drag_Info.LastDragLeaveObject = null;
            }), DispatcherPriority.Normal);
        }

        public static async Task DropTarget_OnPreviewDrop(object sender, DragEventArgs e, string[] dragDropFormats = null, Action<object[]> converter = null)
        {
            if (!Drag_Info.InsertIndex.HasValue) return;

            var sourceData = GetDragData(sender, e, dragDropFormats ?? new[] { sender.GetType().Name });
            if (sourceData.Length > 0)
            {
                converter?.Invoke(sourceData);
                var dropControl = (ItemsControl)sender;
                var insertingControl = dropControl;
                var visualItems = GetAllVisualItems(dropControl).ToArray();
                var targetList = (IList)(dropControl.ItemsSource ?? dropControl.Items);
                var insertIndex = 0;
                if (visualItems.Length > 0)
                {
                    var tempIndex = Drag_Info.InsertIndex.Value;
                    var insertItem = visualItems[Math.Min(visualItems.Length - 1, tempIndex)];
                    insertingControl = insertItem.ItemsControl;
                    targetList = (IList)(insertItem.ItemsControl.ItemsSource ?? insertItem.ItemsControl.Items);
                    insertIndex = targetList.IndexOf(insertItem.VisualElement.DataContext) + (Drag_Info.IsBottomOrRightEdge ? 1 : 0);
                    if (insertIndex == -1) // TabControl
                        insertIndex = targetList.IndexOf(insertItem.VisualElement);
                    if (insertIndex == -1)
                        throw new Exception("Trap!!! Drop method");
                    if (visualItems.Length <= tempIndex) insertIndex++;
                }

                /*var items = GetItemsHost(insertingControl).Children.OfType<FrameworkElement>().ToArray();
                var removeAnimations = new List<Task>();
                var insertingElements = new List<FrameworkElement>();
                var addAnimations = new List<Task>();
                var addElements = new List<FrameworkElement>();
                foreach (var item in sourceData)
                {
                    var indexOfOldItem = targetList.IndexOf(item);
                    if (indexOfOldItem >= 0)
                    {
                        var insertingElement = items.FirstOrDefault(o => o.DataContext == item);
                        if (insertingElement != null)
                        {
                            insertingElements.Add(insertingElement);
                            removeAnimations.AddRange(AnimationHelper.GetHeightContentAnimations(insertingElement, false));
                        }
                    }
                }

                await Task.WhenAll(removeAnimations.ToArray());
                foreach (var element in insertingElements)
                    element.SetCurrentValueSmart(FrameworkElement.HeightProperty, double.NaN);

                foreach (var item in sourceData)
                {
                    var indexOfOldItem = targetList.IndexOf(item);
                    if (indexOfOldItem >= 0)
                    {
                        if (indexOfOldItem < insertIndex) insertIndex--;
                        targetList.RemoveAt(indexOfOldItem);
                    }

                    if (item is TabItem tabItem)
                        ((TabControl)tabItem.Parent)?.Items.Remove(tabItem);
                    targetList.Insert(insertIndex++, item);
                }

                VisualHelper.DoEvents(DispatcherPriority.Background);
                foreach (var item in sourceData)
                {
                    var itemVisual = insertingControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                    itemVisual.SetCurrentValueSmart(UIElement.OpacityProperty, 1.0);
                    addAnimations.AddRange(AnimationHelper.GetHeightContentAnimations(itemVisual, true));
                    addElements.Add(itemVisual);
                }

                await Task.WhenAll(addAnimations);
                foreach (var element in addElements)
                    element.SetCurrentValueSmart(FrameworkElement.HeightProperty, double.NaN);*/

                foreach (var item in sourceData)
                {
                    var indexOfOldItem = targetList.IndexOf(item);
                    if (indexOfOldItem >= 0)
                    {
                        if (indexOfOldItem < insertIndex) insertIndex--;
                        var insertingElement = GetItemsHost(insertingControl).Children.OfType<FrameworkElement>().FirstOrDefault(o => o.DataContext == item);
                        if (insertingElement != null)
                            await Task.WhenAll(AnimationHelper.GetHeightContentAnimations(insertingElement, false));
                        targetList.RemoveAt(indexOfOldItem);
                        if (insertingElement != null)
                            insertingElement.Height = double.NaN;
                    }

                    // To prevent flicker during drop item
                    insertingControl.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;

                    if (item is TabItem tabItem)
                        ((TabControl)tabItem.Parent)?.Items.Remove(tabItem);
                    targetList.Insert(insertIndex++, item);
                    VisualHelper.DoEvents(DispatcherPriority.Render);
                    var itemVisual = insertingControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                    itemVisual.Opacity = 1.0;
                    await Task.WhenAll(AnimationHelper.GetHeightContentAnimations(itemVisual, true));
                    itemVisual.Height = double.NaN;

                    insertingControl.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
                }
            }

            // Mouse.OverrideCursor = null;
            e.Handled = true;

            void ItemContainerGenerator_StatusChanged(object sender2, EventArgs e2)
            {
                var generator = (ItemContainerGenerator)sender2;
                if (generator.Status != GeneratorStatus.ContainersGenerated) return;
                foreach (var item in generator.Items)
                    if (generator.ContainerFromItem(item) is FrameworkElement fe && fe.ActualHeight < 0.0001 && fe.ActualWidth < 0.0001)
                        fe.Opacity = 0.0;
            }
        }

        #endregion

        #region =======  Public methods  ========
        public static Panel GetItemsHost(ItemsControl itemsControl)
        {
            if (_piItemsHost == null)
                _piItemsHost = typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return (Panel)_piItemsHost.GetValue(itemsControl);
        }

        public static IEnumerable<ElementOfItemsControl> GetAllVisualItems(ItemsControl control)
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

        public static object[] GetDragData(object sender, DragEventArgs e, IEnumerable<string> dragDropFormats)
        {
            if (dragDropFormats == null)
                dragDropFormats = new[] { sender.GetType().Name };
            foreach (var format in dragDropFormats)
                if (e.Data.GetDataPresent(format))
                    return (e.Data.GetData(format) as object[]) ?? new object[0];
            return new object[0];
        }
        #endregion

        private static DropTargetInsertionAdorner _dropTargetAdorner;
        private static DragAdorner _dragAdorner;
        private static bool _isDragging;

        #region =============  Private methods  ================
        private static PropertyInfo _piItemsHost;
        private static PropertyInfo _piDataGridHeaderHost;
        private static FrameworkElement GetHeaderHost(ItemsControl itemsControl)
        {
            if (itemsControl is DataGrid)
            {
                if (_piDataGridHeaderHost == null)
                    _piDataGridHeaderHost = typeof(DataGrid).GetProperty("ColumnHeadersPresenter", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                return (DataGridColumnHeadersPresenter)_piDataGridHeaderHost.GetValue(itemsControl);
            }
            return null;
        }

        private static void CheckScroll(ItemsControl o, DragEventArgs e)
        {
            var scrollViewer = o.GetVisualChildren().OfType<ScrollViewer>().FirstOrDefault();
            if (scrollViewer != null)
            {
                const double scrollMargin = 25.0;
                var scrollStep = scrollViewer.Content is TabPanel ? 10.0 : 1.0;
                var position = e.GetPosition(scrollViewer);
                if (position.X >= scrollViewer.ActualWidth - scrollMargin && scrollViewer.HorizontalOffset <
                    scrollViewer.ExtentWidth - scrollViewer.ViewportWidth)
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + scrollStep);
                else if (position.X < scrollMargin && scrollViewer.HorizontalOffset > 0)
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - scrollStep);
                else if (position.Y >= scrollViewer.ActualHeight - scrollMargin && scrollViewer.VerticalOffset <
                         scrollViewer.ExtentHeight - scrollViewer.ViewportHeight)
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + scrollStep);
                else if (position.Y < scrollMargin && scrollViewer.VerticalOffset > 0)
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - scrollStep);
            }
        }

        internal static Orientation GetItemsPanelOrientation(ItemsControl itemsControl)
        {
            if (itemsControl is TabControl)
            {
                var tabControl = (TabControl)itemsControl;
                return tabControl.TabStripPlacement == Dock.Left || tabControl.TabStripPlacement == Dock.Right
                    ? Orientation.Vertical
                    : Orientation.Horizontal;
            }

            var panel = GetItemsHost(itemsControl);
            var orientationProperty = panel.GetType().GetProperty("Orientation", typeof(Orientation));

            if (orientationProperty != null)
                return (Orientation)orientationProperty.GetValue(panel, null);

            throw new Exception("Trap! Can't define item panel orientation");
        }

        private static void DefineInsertIndex(ItemsControl control, DragEventArgs e)
        {
            var orientation = GetItemsPanelOrientation(control);
            var panel = GetItemsHost(control);
            Drag_Info.FirstItemOffset = panel.Children.Count == 0 ? 0 : control.ItemContainerGenerator.IndexFromContainer(panel.Children[0]);
            Drag_Info.DragDropEffect = StartDrag_Info.DragSource == control ? DragDropEffects.Move : DragDropEffects.Copy;
            SetItemsRects(control);
            var mousePosition = e.GetPosition(control);
            for (var i = 0; i < Drag_Info.ItemsRects.Count; i++)
            {
                if (Drag_Info.ItemsRects[i].Contains(mousePosition))
                {
                    Drag_Info.InsertIndex = i;
                    if (orientation == Orientation.Vertical)
                        Drag_Info.IsBottomOrRightEdge = mousePosition.Y >= (Drag_Info.ItemsRects[i].Top + Drag_Info.ItemsRects[i].Height * 0.5);
                    else
                        Drag_Info.IsBottomOrRightEdge = mousePosition.X >= (Drag_Info.ItemsRects[i].Left + Drag_Info.ItemsRects[i].Width * 0.5);
                    return;
                }
            }

            var headerHost = GetHeaderHost(control);
            if (headerHost != null && headerHost.IsMouseOverElement(e.GetPosition))
            {
                Drag_Info.InsertIndex = 0;
                Drag_Info.IsBottomOrRightEdge = false;
                return;
            }

            if (Drag_Info.ItemsRects.Count == 0 && panel.IsMouseOverElement(e.GetPosition))
            {
                Drag_Info.InsertIndex = 0;
                Drag_Info.IsBottomOrRightEdge = false;
                return;
            }

            if (Drag_Info.ItemsRects.Count > 0 && panel.IsMouseOverElement(e.GetPosition))
            {
                var r = Drag_Info.ItemsRects[Drag_Info.ItemsRects.Count - 1];
                if ((orientation == Orientation.Vertical && r.Y < mousePosition.Y && r.X <= mousePosition.X &&
                     (r.X + r.Width) >= mousePosition.X) ||
                    (orientation == Orientation.Horizontal && r.X < mousePosition.X && r.Y <= mousePosition.Y &&
                     (r.Y + r.Height) >= mousePosition.Y))
                {
                    Drag_Info.InsertIndex = Drag_Info.ItemsRects.Count - 1;
                    Drag_Info.IsBottomOrRightEdge = true;
                    return;
                }
            }

            Drag_Info.InsertIndex = null;
        }

        private static void ResetDragDrop(DragEventArgs e)
        {
            if (_dropTargetAdorner != null)
            {
                _dropTargetAdorner.Detach();
                _dropTargetAdorner = null;
            }
            if (_dragAdorner != null)
            {
                _dragAdorner.Detach();
                _dragAdorner = null;
            }

            Drag_Info.DragDropEffect = DragDropEffects.None;
            Drag_Info.InsertIndex = null;

            if (e != null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private static void SetItemsRects(ItemsControl control)
        {
            var items = GetAllVisualItems(control).ToArray();
            var rects = new List<Rect>();
            foreach (var item in items)
                rects.Add(item.VisualElement.GetVisibleRect(control));

            var orientation = GetItemsPanelOrientation(control);
            for (var i = 0; i < (rects.Count - 1); i++)
            {
                var r1 = rects[i];
                var r2 = rects[i + 1];
                if (r1.IsEmpty || r2.IsEmpty) continue;
                if (orientation == Orientation.Vertical)
                {
                    var delta = r2.Y - (r1.Y + r1.Height);
                    if (delta > 0)
                        rects[i + 1] = new Rect(rects[i + 1].X, rects[i + 1].Y - delta, rects[i + 1].Width, rects[i + 1].Height + delta);
                }
                else
                {
                    var delta = r2.X - (r1.X + r1.Width);
                    if (delta > 0.0)
                        rects[i + 1] = new Rect(rects[i + 1].X - delta, rects[i + 1].Y, rects[i + 1].Width + delta, rects[i + 1].Height);
                }
            }

            Drag_Info.ItemsRects = rects;
        }
        #endregion

        #region =============  Helper classes  ===============
        public class StartDragInfo
        {
            internal Point DragStart { get; private set; }
            public ItemsControl DragSource { get; private set; }
            public void Init(ItemsControl dragSource, MouseEventArgs e)
            {
                DragSource = dragSource;
                DragStart = e.GetPosition(dragSource);
            }
            public void Clear(MouseEventArgs e)
            {
                if (e != null)
                    e.Handled = true;
                DragSource = null;
                DragStart = new Point(-100, -100);
            }
        }

        public class DragInfo
        {
            public object LastDragLeaveObject;
            public int? InsertIndex;
            public List<Rect> ItemsRects;
            public bool IsBottomOrRightEdge;
            public int FirstItemOffset;
            public DragDropEffects DragDropEffect;
            public FrameworkElement GetHoveredItem(ItemsControl control)
            {
                if (!InsertIndex.HasValue) return null;
                var items = GetAllVisualItems(control).ToArray();
                if (InsertIndex.Value < items.Length)
                    return items[InsertIndex.Value].VisualElement;
                if (InsertIndex.Value == items.Length && items.Length > 0)
                    return items[items.Length - 1].VisualElement;
                return null;
            }

            public void Clear()
            {
                LastDragLeaveObject = null;
                InsertIndex = null;
            }
        }

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
                    var aa1 = control.GetVisualChildren().OfType<ContentPresenter>().Where(o => o.ContentSource == "Header").ToArray();
                    var aa2 = aa1.Where(o => o.GetVisualParents().OfType<ItemsControl>().FirstOrDefault() == element).ToArray();
                    if (aa2.Length == 1)
                        VisualElement = aa2[0];
                }
            }
        }
        #endregion
    }
}
