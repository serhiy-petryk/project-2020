using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DGView.ViewModels
{
    public class DGProperty_GroupItemModel
    {
        private static int cnt;
        public int Id = cnt++;
        public DGProperty_ItemModel PropertyItem { get; set; }
        public DGProperty_GroupItemModel NextGroup { get; set; }
        public ObservableCollection<DGPropertySortItemModel> SortItems = new ObservableCollection<DGPropertySortItemModel>();

        public string Name => PropertyItem == null ? "Details" : PropertyItem.Name;

        public DGProperty_GroupItemModel()
        {

        }
        public void AddNewGroup(DGProperty_ItemModel newPropertyItem)
        {
            if (NextGroup == null)
            {
                var newGroup = new DGProperty_GroupItemModel { PropertyItem = PropertyItem, NextGroup = NextGroup, SortItems = SortItems};
                PropertyItem = newPropertyItem;
                NextGroup = newGroup;
            }
            else
                NextGroup.AddNewGroup(newPropertyItem);
        }

    }

    public class DGPropertySortItemModel
    {
        public DGProperty_ItemModel Item { get; set; }
        public ListSortDirection SortDirection { get; set; }
    }

}
