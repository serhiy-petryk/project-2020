using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DGView.ViewModels
{
    public class DGPropertyGroupItemModel
    {
        public DGPropertyItemModel GroupItem { get; set; }
        public DGPropertyItemModel NextGroupItem { get; set; }
        public ObservableCollection<DGPropertySortItemModel> SortItems = new ObservableCollection<DGPropertySortItemModel>();

        public string Name => GroupItem == null ? "Details" : GroupItem.Name;

    }

    public class DGPropertySortItemModel
    {
        public DGPropertyItemModel Item { get; set; }
        public ListSortDirection SortDirection { get; set; }
    }

}
