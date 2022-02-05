﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static void DragSource_OnPreviewMouseMove(object sender, MouseEventArgs e, string dragDropFormat = null)
        {
            if (_isDragging) return;
            if (!(sender is ItemsControl itemsControl) || e.LeftButton == MouseButtonState.Released ||
                GetSelectedItems(itemsControl, e).Count == 0)
            {
                StartDrag_Info.Clear();
                return;
            }

            var itemsHost = GetItemsHost(itemsControl);
            if (!itemsHost.IsMouseOverElement(e.GetPosition))
            {
                StartDrag_Info.Clear();
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
                dataObject.SetData(dragDropFormat ?? sender.GetType().Name, GetSelectedItems(itemsControl, e).ToArray());
                try
                {
                    _isDragging = true;
                    var result = DragDrop.DoDragDrop(itemsControl, dataObject, DragDropEffects.Copy);
                }
                finally
                {
                    _isDragging = false;
                    StartDrag_Info.Clear();
                    Drag_Info.Clear();
                    ResetDragDrop(null);
                }

                e.Handled = true;
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

        public static void DropTarget_OnPreviewDragOver(object sender, DragEventArgs e, IEnumerable<string> formats = null)
        {
            Drag_Info.LastDragLeaveObject = null;

            formats = formats ?? new[] { sender.GetType().Name };
            object dragData = null;
            foreach (var format in formats)
            {
                dragData = e.Data.GetData(format);
                if (dragData != null)
                    break;
            }
            if (dragData == null)
            {
                ResetDragDrop(e);
                return;
            }

            var control = (ItemsControl)sender;
            DefineInsertIndex(control, e);

            if (Drag_Info.IsBottomOrRightEdge)
            {
                Drag_Info.InsertIndex++;
                Drag_Info.IsBottomOrRightEdge = false;
            }

            if (!Drag_Info.InsertIndex.HasValue)
            {
                Drag_Info.DragDropEffect = DragDropEffects.None;
                ResetDragDrop(e);
                return;
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

        public static void DropTarget_OnPreviewDrop(object sender, DragEventArgs e, string[] dragDropFormats = null)
        {
            if (!Drag_Info.InsertIndex.HasValue) return;

            if (dragDropFormats == null)
                dragDropFormats = new[] { sender.GetType().Name };
            var sourceData = new object[0];
            foreach (var format in dragDropFormats)
                if (e.Data.GetDataPresent(format))
                {
                    sourceData = (e.Data.GetData(format) as object[]) ?? new object[0];
                    break;
                }

            if (sourceData.Length > 0)
            {
                var dropControl = (ItemsControl)sender;
                var items = GetAllItems(dropControl).ToArray();
                var targetList = (IList)(dropControl.ItemsSource ?? dropControl.Items);
                var insertIndex = 0;
                if (items.Length > 0)
                {
                    var insertItem = items[Math.Min(items.Length - 1, Drag_Info.InsertIndex.Value)];
                    targetList = (IList)(insertItem.ItemsControl.ItemsSource ?? insertItem.ItemsControl.Items);
                    insertIndex = targetList.IndexOf(insertItem.VisualElement.DataContext);
                    if (insertIndex == -1) // TabControl
                        insertIndex = targetList.IndexOf(insertItem.VisualElement);
                    if (insertIndex == -1)
                        throw new Exception("Trap!!! Drop method");
                    if (items.Length <= Drag_Info.InsertIndex.Value) insertIndex++;
                }

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
            }

            Mouse.OverrideCursor = null;
            e.Handled = true;
        }
        #endregion

        #region =======  Public methods  ========
        public static Panel GetItemsHost(ItemsControl itemsControl)
        {
            if (_piItemsHost == null)
                _piItemsHost = typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return (Panel)_piItemsHost.GetValue(itemsControl);
        }
        #endregion

        private static DropTargetInsertionAdorner _dropTargetAdorner;
        private static DragAdorner _dragAdorner;
        private static bool _isDragging;

        #region =============  Private methods  ================
        private static List<object> GetSelectedItems(ItemsControl control, MouseEventArgs e)
        {
            if (control is MultiSelector multiSelector)
                return new List<object>(multiSelector.SelectedItems.OfType<object>());

            if (control is Selector selector)
            {
                if (selector.SelectedItem != null)
                    return new List<object> { selector.SelectedItem };
                return new List<object>();
            }

            if (control is TreeView treeView)
            {
                if (treeView.SelectedItem != null)
                    return new List<object> { treeView.SelectedItem };
                return new List<object>();
            }

            throw new Exception($"Trap! {control.GetType().Name} is not supported");
        }

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

            if (e != null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        public static IEnumerable<ElementOfItemsControl> GetAllItems(ItemsControl control)
        {
            var panel = GetItemsHost(control);
            for (var i = 0; i < panel.Children.Count; i++)
            {
                if (panel.Children[i] is FrameworkElement element)
                {
                    yield return new ElementOfItemsControl(element, control);
                    if (element is ItemsControl itemsControl) // TreeViewItem
                    {
                        foreach (var childItem in GetAllItems(itemsControl))
                            yield return childItem;
                    }
                }
            }
        }
        private static void SetItemsRects(ItemsControl control)
        {
            var items = GetAllItems(control).ToArray();
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
            public void Clear()
            {
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
            public object GetHoveredItem(ItemsControl control)
            {
                if (!InsertIndex.HasValue) return null;
                var items = GetAllItems(control).ToArray();
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
            public ItemsControl ItemsControl;
            public FrameworkElement VisualElement;
            // internal bool IsLastElement;

            public ElementOfItemsControl(FrameworkElement element, ItemsControl itemsControl)
            {
                ItemsControl = itemsControl;
                VisualElement = element;
                // IsLastElement = itemsPanel.Children.IndexOf(element) == (itemsPanel.Children.Count - 1);
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
