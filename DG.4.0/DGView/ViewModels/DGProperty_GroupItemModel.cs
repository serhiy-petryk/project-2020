using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WpfSpLib.Common;

namespace DGView.ViewModels
{
    public class PropertyGroupItem : INotifyPropertyChanged
    {
        public static IEnumerable<PropertyGroupItem> GetAllChildren(PropertyGroupItem current)
        {
            yield return current;
            foreach (var item in current.Children)
            foreach (var child in GetAllChildren(item))
                yield return child;
        }

        public PropertyGroupItem Parent { get; private set; }
        public DGProperty_ItemModel Item { get; private set; }
        private ListSortDirection _sortDirection;

        public ListSortDirection SortDirection
        {
            get => _sortDirection;
            set
            {
                _sortDirection = value;
                if (Type == "Group" && !Equals(Item.GroupDirection, value))
                    Item.GroupDirection = value;
                OnPropertiesChanged(nameof(SortDirection));
            }
        }

        public bool CanSort => Type == "Group" || Type == "Sort";
        public string Name => Item == null ? "Sortings:" : Item.Name;
        public string Type => Parent == null ? "Root" : (Item == null ? "Label" : (Parent.Type == "Root" ? "Group" : "Sort"));
        public ObservableCollection<PropertyGroupItem> Children { get; } = new ObservableCollection<PropertyGroupItem>();
        public PropertyGroupItem Root => Parent == null ? this : Parent.Root;
        public System.Windows.Media.Color BaseColor
        {
            get
            {
                var groupItem = Type == "Group" ? this : Parent;
                var groupIndex = Root.Children.IndexOf(groupItem) % (DGCore.Helpers.ColorInfo.GroupColors.Length-1) + 1;
                var groupColor = DGCore.Helpers.ColorInfo.GroupColors[groupIndex];
                return System.Windows.Media.Color.FromArgb(255, groupColor.R, groupColor.G, groupColor.B);
            }
        }

        public RelayCommand CmdClear { get; private set; }

        public PropertyGroupItem()
        {
            CmdClear = new RelayCommand(cmdClear);
        }

        private void cmdClear(object p)
        {
            if (Type == "Group")
                Item.GroupDirection = null;
        }

        public PropertyGroupItem AddNewItem(DGProperty_ItemModel item, ListSortDirection sortDirection)
        {
            var oldItem = Children.FirstOrDefault(o => o.Item == item);
            if (oldItem != null)
                Children.Remove(oldItem);
            var newItem = new PropertyGroupItem { Parent = this, Item = item, SortDirection = sortDirection };
            Children.Add(newItem);
            if (newItem.Type == "Group")
                newItem.Children.Add(new PropertyGroupItem { Parent = newItem }); // Add "Sortings:" label
            return newItem;
        }

        public void UpdateUI()
        {
            OnPropertiesChanged(nameof(BaseColor));
        }

        #region ===========  INotifyPropertyChanged  ==============
        public event PropertyChangedEventHandler PropertyChanged;
        internal void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    // =======  OLD CODE  ========
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
