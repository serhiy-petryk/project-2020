using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DGView.ViewModels
{
    public class PropertyItem
    {
        private static int cnt;
        public int Id { get; } = cnt++;
        public string Name { get; set; }

        public PropertyItem(string name)
        {
            Name = name;
        }
    }

    public class PropertyGroupItem
    {
        public PropertyGroupItem Parent { get; private set; }
        public DGProperty_ItemModel Item { get; private set; }
        public ListSortDirection SortDirection { get; private set; }
        public string Name => Item == null ? "Sortings:" : Item.Name;
        public string Type => Parent == null ? "Root" : (Item == null ? "Label" : (Parent.Type == "Root" ? "Group" : "Sort"));
        public ObservableCollection<PropertyGroupItem> Children { get; } = new ObservableCollection<PropertyGroupItem>();
        public PropertyGroupItem Root => Parent == null ? this : Parent.Root;
        public System.Windows.Media.Color BaseColor
        {
            get
            {
                var groupItem = Type == "Group" ? this : Parent;
                var groupIndex = Root.Children.IndexOf(groupItem) + 1;
                var groupColor = DGCore.Helpers.ColorInfo.GroupColors[groupIndex];
                return System.Windows.Media.Color.FromArgb(255, groupColor.R, groupColor.G, groupColor.B);
            }
        }

        public PropertyGroupItem AddNewItem(DGProperty_ItemModel item, ListSortDirection sortDirection)
        {
            var newItem = new PropertyGroupItem { Parent = this, Item = item, SortDirection = sortDirection };
            Children.Add(newItem);
            if (newItem.Type == "Group")
                newItem.Children.Add(new PropertyGroupItem { Parent = newItem }); // Add "Sortings:" label
            return newItem;
        }
    }


    public class xxDGProperty_GroupItemModel
    {
        private static int cnt;
        public int Id = cnt++;
        public DGProperty_ItemModel PropertyItem { get; set; }
        public xxDGProperty_GroupItemModel NextGroup { get; set; }
        public ObservableCollection<xxDGPropertySortItemModel> SortItems = new ObservableCollection<xxDGPropertySortItemModel>();

        public string Name => PropertyItem == null ? "Details" : PropertyItem.Name;

        public xxDGProperty_GroupItemModel()
        {

        }
        public void AddNewGroup(DGProperty_ItemModel newPropertyItem)
        {
            if (NextGroup == null)
            {
                var newGroup = new xxDGProperty_GroupItemModel { PropertyItem = PropertyItem, NextGroup = NextGroup, SortItems = SortItems};
                PropertyItem = newPropertyItem;
                NextGroup = newGroup;
            }
            else
                NextGroup.AddNewGroup(newPropertyItem);
        }

    }

    public class xxDGPropertySortItemModel
    {
        public DGProperty_ItemModel Item { get; set; }
        public ListSortDirection SortDirection { get; set; }
    }

}
