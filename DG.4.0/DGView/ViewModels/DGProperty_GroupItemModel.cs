using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using WpfSpLib.Common;

namespace DGView.ViewModels
{
    public class PropertyGroupItem : INotifyPropertyChanged
    {
        public enum ItemType { Root, Details, Group, Label, Sorting}
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
                if (Type == ItemType.Group && !Equals(Item.GroupDirection, value))
                    Item.GroupDirection = value;
                OnPropertiesChanged(nameof(SortDirection));
            }
        }

        public bool CanSort => Type == ItemType.Group  || Type == ItemType.Sorting;
        public string Name => Type == ItemType.Details  ? "Sortings of detail items:" :(Item == null ? "Sortings:" : Item.Name);
        public ItemType Type => Parent == null ? ItemType.Root : (Item == null ? (Parent.Type == ItemType.Root  ? ItemType.Details : ItemType.Label) : (Parent.Type == ItemType.Root ? ItemType.Group : ItemType.Sorting));
        public ObservableCollection<PropertyGroupItem> Children { get; } = new ObservableCollection<PropertyGroupItem>();
        public PropertyGroupItem Root => Parent == null ? this : Parent.Root;
        public Color BaseColor
        {
            get
            {
//                if (Type == "Details") return Color.FromArgb(0xFF, 0xF5, 0xFA, 0xFF);
                if (Type == ItemType.Details) return Colors.White;
                if (Type == ItemType.Group)
                {
                    var groupIndex = Root.Children.IndexOf(this) % (DGCore.Helpers.ColorInfo.GroupColors.Length - 1) + 1;
                    var groupColor = DGCore.Helpers.ColorInfo.GroupColors[groupIndex];
                    return Color.FromArgb(255, groupColor.R, groupColor.G, groupColor.B);
                }
                return Parent.BaseColor;
            }
        }

        public RelayCommand CmdClear { get; private set; }

        public PropertyGroupItem()
        {
            CmdClear = new RelayCommand(cmdClear);
        }

        private void cmdClear(object p)
        {
            if (Type == ItemType.Group)
                Item.GroupDirection = null;
            else if (Type == ItemType.Sorting)
                Parent.Children.Remove(this);
        }

        public PropertyGroupItem AddNewItem(DGProperty_ItemModel item, ListSortDirection sortDirection)
        {
            var oldItem = Children.FirstOrDefault(o => o.Item == item);
            if (oldItem != null)
                Children.Remove(oldItem);
            var newItem = new PropertyGroupItem { Parent = this, Item = item, SortDirection = sortDirection };
            if (Children.Count > 0 && Children[Children.Count - 1].Type == ItemType.Details)
                Children.Insert(Children.Count - 1, newItem);
            else
                Children.Add(newItem);
            if (newItem.Type == ItemType.Group)
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
}
