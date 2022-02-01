using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

            /*for (var i = settings.Sorts.Count - 1; i >= 0; i--)
            {
                var sortItem = settings.Sorts[i];
                var item = PropertiesData.FirstOrDefault(o => o.Id == sortItem.Id);
                // GroupItem.AddNewGroup(item);
            }*/
        }

        #region =======  Drag/Drop event handlers ========
        private void PropertyList_OnPreviewMouseMove(object sender, MouseEventArgs e) => DragDropHelper.DragSource_OnPreviewMouseMove(sender, e);
        private void PropertyList_OnPreviewGiveFeedback(object sender, GiveFeedbackEventArgs e) => DragDropHelper.DragSource_OnPreviewGiveFeedback(sender, e);
        private void PropertyList_OnPreviewDragOver(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e);
        private void GroupTreeView_OnPreviewDragOver(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e, new[] { "TreeView", "DataGrid" });
        private void PropertyList_OnPreviewDragEnter(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e);
        private void GroupTreeView_OnPreviewDragEnter(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e, new[] { "TreeView", "DataGrid" });
        private void PropertyList_OnPreviewDragLeave(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragLeave(sender, e);
        private void PropertyList_OnDrop(object sender, DragEventArgs e)
        {
            if (!DragDropHelper.Drag_Info.InsertIndex.HasValue || e.Effects != DragDropEffects.Copy) return;

            var sourceData = ((e.Data.GetData(sender.GetType().Name) as object[]) ?? new object[0]).OfType<DGProperty_ItemModel>().ToArray();
            var insertIndex = DragDropHelper.Drag_Info.InsertIndex.Value + DragDropHelper.Drag_Info.FirstItemOffset;
            foreach (var item in sourceData)
            {
                var oldIndex = PropertyList.Items.IndexOf(item);
                if (oldIndex < insertIndex) insertIndex--;
                if (oldIndex != insertIndex)
                {
                    var offsetItem = (DGProperty_ItemModel)PropertyList.Items[insertIndex];
                    var originalNewIndex = PropertiesData.IndexOf(offsetItem);
                    var originalOldIndex = PropertiesData.IndexOf(item);
                    PropertiesData.Move(originalOldIndex, originalNewIndex);
                }

                insertIndex++;
            }

            // Update row numeration
            var itemsHost = DragDropHelper.GetItemsHost(PropertyList);
            foreach (DataGridRow item in itemsHost.Children)
                PropertyList_OnLoadingRow(PropertyList, new DataGridRowEventArgs(item));
        }

        #endregion

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            return;
            var cell = sender as DataGridCell;
            var dataGrid = cell.GetVisualParents().OfType<DataGrid>().FirstOrDefault();
            if (cell.IsReadOnly || dataGrid == null) return;

            // see  MahApps.Metro.Controls.DataGridHelper.DataGridOnPreviewMouseLeftButtonDown
            var toggleButtons = cell.GetVisualChildren().OfType<ToggleButton>().ToArray();
            if (toggleButtons.Length == 1)
            {
                dataGrid.BeginEdit();
                toggleButtons[0].SetCurrentValue(ToggleButton.IsCheckedProperty, !toggleButtons[0].IsChecked);
                dataGrid.CommitEdit();
                e.Handled = true;
            }
            var textBlocks = cell.GetVisualChildren().OfType<TextBlock>().ToArray();
            if (textBlocks.Length == 1)
            {
                cell.Focus();
                dataGrid.BeginEdit();
                e.Handled = true;
            }
        }

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
