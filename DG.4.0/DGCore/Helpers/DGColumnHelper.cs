using System;
using System.ComponentModel;
using DGCore.Common;
using DGCore.DGVList;
using DGCore.PD;

namespace DGCore.Helpers
{
    public class DGColumnHelper
    {
        public static DGColumnHelper GetColumnHelper(string propertyName, PropertyDescriptorCollection pdc)
        {
            if (propertyName == Constants.GroupItemCountColumnName)
                return new DGColumnHelper(new PropertyDescriptorForGroupItemCount());
            var propertyDescriptor = pdc[propertyName];
            return new DGColumnHelper(propertyDescriptor);
        }

        private int? _groupLevel = null;
        private Func<object, object> _getter;

        public string Name { get; }
        public string DisplayName { get; }
        public string Format { get; }
        public object GetValue(object component) => _getter(component);

        public Type ValueType { get; }

        // For common columns
        public DGColumnHelper(PropertyDescriptor pd)
        {
            Name = pd.Name;
            DisplayName = pd.DisplayName;
            ValueType = pd.PropertyType;
            Format = ((IMemberDescriptor)pd).Format;
            _getter = o => pd.GetValue(o);
        }

        // For group columns
        public DGColumnHelper(int groupLevel)
        {
            Name = groupLevel.ToString();
            _groupLevel = groupLevel;
            DisplayName = groupLevel.ToString();
            _getter = o => o is IDGVList_GroupItem groupItem && ((_groupLevel + 1) == groupItem.Level) ? (groupItem.IsExpanded ? "-" : "+") : null;
        }
    }
}
