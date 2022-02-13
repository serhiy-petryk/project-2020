using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using DGCore.PD;
using DGCore.UserSettings;
using DGView.ViewModels;
using WpfSpLib.Helpers;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for DGEditSettingsView.xaml
    /// </summary>
    public partial class DGEditSettingsView : UserControl
    {
        public ObservableCollection<DGProperty_ItemModel> PropertiesData { get; }
        public PropertyGroupItem GroupItem { get; } = new PropertyGroupItem();

        #region =======  Quick Filter  =========
        private string _quickFilterText;
        public string QuickFilterText
        {
            get => _quickFilterText;
            set
            {
                if (!Equals(_quickFilterText, value))
                {
                    _quickFilterText = value;
                    SetFilter();
                }
            }
        }
        private void SetFilter()
        {
            var view = CollectionViewSource.GetDefaultView(PropertiesData);
            view.Filter += Filter;
            // DataGrid.SelectedItem = DataGrid.Items.OfType<object>().FirstOrDefault();
        }
        private bool Filter(object obj) => Helpers.Misc.SetFilter(((DGProperty_ItemModel) obj).Name, QuickFilterText);
        #endregion

        public DGEditSettingsView(DGV settings, PropertyDescriptorCollection properties)
        {
            InitializeComponent();
            DataContext = this;
            PropertiesData = new ObservableCollection<DGProperty_ItemModel>(settings.AllColumns.Select(o => new DGProperty_ItemModel(this, o, settings, (IMemberDescriptor)properties[o.Id])));
            cbShowTotalRow.IsChecked = settings.ShowTotalRow;

            GroupItem.Children.Clear();
            for (var i1 = 0; i1 < settings.Groups.Count; i1++)
            {
                var groupItem = settings.Groups[i1];
                var item = PropertiesData.FirstOrDefault(o => o.Id == groupItem.Id);
                var group = GroupItem.AddNewItem(item, groupItem.SortDirection);
                for (var i2 = 0; i2 < settings.SortsOfGroup[i1].Count; i2++)
                {
                    var sortItem = settings.SortsOfGroup[i1][i2];
                    item = PropertiesData.FirstOrDefault(o => o.Id == sortItem.Id);
                    group.AddNewItem(item, sortItem.SortDirection);
                }
            }

            var detailsGroup = GroupItem.AddNewItem(null, ListSortDirection.Ascending);
            for (var i = 0; i < settings.Sorts.Count; i++)
            {
                var sortItem = settings.Sorts[i];
                var item = PropertiesData.FirstOrDefault(o => o.Id == sortItem.Id);
                detailsGroup.AddNewItem(item, sortItem.SortDirection);
            }
        }

        #region =======  Drag/Drop event handlers ========
        private void PropertyList_OnPreviewMouseMove(object sender, MouseEventArgs e) => DragDropHelper.DragSource_OnPreviewMouseMove(sender, e);
        private void GroupTreeView_OnPreviewMouseMove(object sender, MouseEventArgs e) => DragDropHelper.DragSource_OnPreviewMouseMove(sender, e, null, GroupTreeView_AllowDrag);
        private void PropertyList_OnPreviewGiveFeedback(object sender, GiveFeedbackEventArgs e) => DragDropHelper.DragSource_OnPreviewGiveFeedback(sender, e);
        private void PropertyList_OnPreviewDragOver(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e);
        private void GroupTreeView_OnPreviewDragOver(object sender, DragEventArgs e) =>
            DragDropHelper.DropTarget_OnPreviewDragOver(sender, e, new[] { "TreeView", "DataGrid" }, GroupTreeView_AfterDefiningIndex);
        private void PropertyList_OnPreviewDragEnter(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e);
        private void GroupTreeView_OnPreviewDragEnter(object sender, DragEventArgs e) =>
            DragDropHelper.DropTarget_OnPreviewDragOver(sender, e, new[] { "TreeView", "DataGrid" }, GroupTreeView_AfterDefiningIndex);
        private void PropertyList_OnPreviewDragLeave(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragLeave(sender, e);
        private async void PropertyList_OnPreviewDrop(object sender, DragEventArgs e)
        {
            await DragDropHelper.DropTarget_OnPreviewDrop(sender, e);
            ReorderFrozenItems();
        }

        private async void GroupTreeView_OnPreviewDrop(object sender, DragEventArgs e)
        {
            await DragDropHelper.DropTarget_OnPreviewDrop(sender, e, new[] {"TreeView", "DataGrid"}, ConverterForTreeView);
            foreach (var child in PropertyGroupItem.GetAllChildren(GroupItem))
                child.UpdateUI();
        }

        #endregion

        #region ==========  Helper methods  ============
        private bool GroupTreeView_AllowDrag(object[] arg) => arg.Length == 1 && arg[0] is PropertyGroupItem groupItem && groupItem.CanSort;

        private static void GroupTreeView_AfterDefiningIndex(object[] dragData, ItemsControl control, DragEventArgs e)
        {
            if (dragData.Length == 0 || dragData.OfType<DGProperty_ItemModel>().Any(o => !o.IsSortingSupport))
            {
                DragDropHelper.Drag_Info.DragDropEffect = DragDropEffects.None;
                return;
            }

            var hoveredElement = DragDropHelper.Drag_Info.GetHoveredItem(control);
            var hoveredItem = hoveredElement?.DataContext as PropertyGroupItem;
            var internalDragItem = dragData.Length == 1 && dragData[0] is PropertyGroupItem ? dragData[0] as PropertyGroupItem : null;
            if (hoveredItem == null || internalDragItem != null && hoveredItem.Parent != internalDragItem.Parent)
            {
                DragDropHelper.Drag_Info.DragDropEffect = DragDropEffects.None;
                return;
            }

            if (internalDragItem != null || dragData.OfType<DGProperty_ItemModel>().Count() == dragData.Length)
            {
                if (hoveredItem.Type == PropertyGroupItem.ItemType.Label)
                {
                    if (hoveredItem.Parent.Children.Count > 1)
                    {
                        DragDropHelper.Drag_Info.InsertIndex++;
                        DragDropHelper.Drag_Info.IsBottomOrRightEdge = false;
                    }
                    else
                        DragDropHelper.Drag_Info.IsBottomOrRightEdge = true;
                }
                else if (hoveredItem.Type == PropertyGroupItem.ItemType.Group || hoveredItem.Type == PropertyGroupItem.ItemType.Sorting)
                {
                    if (DragDropHelper.Drag_Info.IsBottomOrRightEdge && DragDropHelper.Drag_Info.InsertIndex < hoveredItem.Parent.Children.Count)
                    {
                        DragDropHelper.Drag_Info.InsertIndex = DragDropHelper.Drag_Info.InsertIndex + hoveredItem.Children.Count + 1;
                        DragDropHelper.Drag_Info.IsBottomOrRightEdge = false;
                    }
                }
                else if (hoveredItem.Type == PropertyGroupItem.ItemType.Details && hoveredItem.Children.Count > 0)
                    DragDropHelper.Drag_Info.IsBottomOrRightEdge = false;
            }
            else
                DragDropHelper.Drag_Info.DragDropEffect = DragDropEffects.None;
        }
        private void ConverterForTreeView(object[] data)
        {
            var hoveredElement = DragDropHelper.Drag_Info.GetHoveredItem(GroupTreeView);
            var targetItem = hoveredElement?.DataContext as PropertyGroupItem;
            for (var i = 0; i < data.Length; i++)
            {
                if (data[i] is DGProperty_ItemModel propertyItem)
                {
                    var newItem = new PropertyGroupItem { Parent = targetItem.Parent, Item = propertyItem };
                    if (newItem.Type == PropertyGroupItem.ItemType.Group) // Add 'Sortings:' label
                        newItem.Children.Add(new PropertyGroupItem { Parent = newItem });
                    data[i] = newItem;
                }
            }
        }
        #endregion

        private void PropertyList_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var rowHeaderText = (e.Row.GetIndex() + 1).ToString("N0", LocalizationHelper.CurrentCulture);
            if (!Equals(e.Row.Header, rowHeaderText)) e.Row.Header = rowHeaderText;
        }

        // ==================
        internal async void GroupChanged (DGProperty_ItemModel item)
        {
            var groupItem = GroupItem.Children.FirstOrDefault(o => o.Item == item);
            if (item.GroupDirection.HasValue && groupItem == null)
            {
                var propertyItem = GroupItem.AddNewItem(item, item.GroupDirection.Value);
                if (GroupTreeView.ItemContainerGenerator.ContainerFromItem(propertyItem) is TreeViewItem treeViewItem)
                {
                    VisualHelper.DoEvents(DispatcherPriority.Render);
                    var tasks = AnimationHelper.GetHeightContentAnimations(treeViewItem, true);
                    await Task.WhenAll(tasks);
                    treeViewItem.Height = double.NaN;
                    treeViewItem.Opacity = 1.0;
                }
            }
            else if (!item.GroupDirection.HasValue && groupItem != null)
            {
                if (GroupTreeView.ItemContainerGenerator.ContainerFromItem(groupItem) is TreeViewItem treeViewItem)
                {
                    var tasks = AnimationHelper.GetHeightContentAnimations(treeViewItem, false);
                    await Task.WhenAll(tasks);
                    treeViewItem.Height = double.NaN;
                    // treeViewItem.Opacity = 1.0;
                }
                GroupItem.Children.Remove(groupItem);
            }
            else if (item.GroupDirection.HasValue && groupItem != null && item.GroupDirection.Value != groupItem.SortDirection)
                groupItem.SortDirection = item.GroupDirection.Value;

            foreach(var child in PropertyGroupItem.GetAllChildren(GroupItem))
                child.UpdateUI();
        }

        public async void ReorderFrozenItems()
        {
            if (PropertiesData == null) return;

            //PropertyList.CommitEdit();
            var frozenIndex = 0;
            foreach (var item in PropertiesData.Where(o => o.IsFrozen).ToArray())
                await PropertyList.AddOrReorderItem(item, frozenIndex++);

            foreach (DataGridRow item in DragDropHelper.GetItemsHost(PropertyList).Children)
                PropertyList_OnLoadingRow(PropertyList, new DataGridRowEventArgs(item));
        }
    }
}

