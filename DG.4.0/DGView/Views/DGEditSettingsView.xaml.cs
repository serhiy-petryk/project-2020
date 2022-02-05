using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
        // public DGProperty_GroupItemModel GroupItem => DGPropertyGroupItemElement.ViewModel;
        public PropertyGroupItem GroupItem = new PropertyGroupItem();

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

            GroupTreeView.ItemsSource = GroupItem.Children;
        }

        #region =======  Drag/Drop event handlers ========
        private void PropertyList_OnPreviewMouseMove(object sender, MouseEventArgs e) => DragDropHelper.DragSource_OnPreviewMouseMove(sender, e);
        private void PropertyList_OnPreviewGiveFeedback(object sender, GiveFeedbackEventArgs e) => DragDropHelper.DragSource_OnPreviewGiveFeedback(sender, e);
        private void PropertyList_OnPreviewDragOver(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e);
        private void GroupTreeView_OnPreviewDragOver(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e, new[] { "TreeView", "DataGrid" });
        private void PropertyList_OnPreviewDragEnter(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e);
        private void GroupTreeView_OnPreviewDragEnter(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e, new[] { "TreeView", "DataGrid" });
        private void PropertyList_OnPreviewDragLeave(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragLeave(sender, e);
        private void PropertyList_OnPreviewDrop(object sender, DragEventArgs e)
        {
            DragDropHelper.DropTarget_OnPreviewDrop(sender, e);
            foreach (DataGridRow item in DragDropHelper.GetItemsHost(PropertyList).Children)
                PropertyList_OnLoadingRow(PropertyList, new DataGridRowEventArgs(item));
        }

        private void GroupTreeView_OnPreviewDrop(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDrop(sender, e, new[] {"TreeView", "DataGrid"});

        private void PropertyList_xxOnPreviewDrop(object sender, DragEventArgs e)
        {
            if (!DragDropHelper.Drag_Info.InsertIndex.HasValue || e.Effects != DragDropEffects.Copy) return;

            var sourceData = new object[0];
            foreach (var format in new[] {"DataGrid", "TreeView"})
                if (e.Data.GetDataPresent(format))
                {
                    sourceData = (e.Data.GetData(format) as object[]) ?? new object[0];
                    break;
                }

            if (sourceData.Length > 0)
            {
                var dropControl = (ItemsControl)sender;
                var items = DragDropHelper.GetAllItems(dropControl).ToArray();
                var targetList = (IList)dropControl.ItemsSource;
                var insertIndex = 0;
                if (items.Length > 0)
                {
                    var insertItem = items[Math.Min(items.Length - 1, DragDropHelper.Drag_Info.InsertIndex.Value)];
                    targetList = (IList) insertItem.ItemsControl.ItemsSource;
                    insertIndex = targetList.IndexOf(insertItem.VisualElement.DataContext);
                    if (insertIndex == -1)
                        throw new Exception("Trap!!! Drop method");
                    if (items.Length <= DragDropHelper.Drag_Info.InsertIndex.Value) insertIndex++;
                }

                foreach (var item in sourceData)
                {
                    var indexOfOldItem = targetList.IndexOf(item);
                    if (indexOfOldItem >= 0)
                    {
                        if (indexOfOldItem < insertIndex) insertIndex--;
                        targetList.RemoveAt(indexOfOldItem);
                    }

                    targetList.Insert(insertIndex++, item);
                }

                if (dropControl == PropertyList)
                {
                    // Update row numeration
                    foreach (DataGridRow item in DragDropHelper.GetItemsHost(PropertyList).Children)
                        PropertyList_OnLoadingRow(PropertyList, new DataGridRowEventArgs(item));
                }
            }

            Mouse.OverrideCursor = null;
            e.Handled = true;
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
                GroupItem.AddNewItem(item, item.GroupDirection.Value);
            else if (!item.GroupDirection.HasValue && groupItem != null)
            {
                var tvi = GroupTreeView.ItemContainerGenerator.ContainerFromItem(groupItem);
                if (tvi is TreeViewItem treeViewItem)
                {
                    var tasks = AnimationHelper.GetContentAnimations(treeViewItem, false).ToList();
                    tasks.Add(treeViewItem.BeginAnimationAsync(HeightProperty, treeViewItem.ActualHeight, 0.0, AnimationHelper.AnimationDurationSlow));
                    await Task.WhenAll(tasks);
                }

                GroupItem.Children.Remove(groupItem);
            }
            else if (item.GroupDirection.HasValue && groupItem != null && item.GroupDirection.Value != groupItem.SortDirection)
                groupItem.SortDirection = item.GroupDirection.Value;

            foreach(var child in PropertyGroupItem.GetAllChildren(GroupItem))
                child.UpdateUI();
        }
    }
}
