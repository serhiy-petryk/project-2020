using System;
using System.ComponentModel;
using DGCore.DGVList;

namespace DGCore.Helpers
{
    public class PropertyDescriptorForDataGridColumn : PropertyDescriptor
    {
        private int _groupLevel = -1;
        public int _headerExcelColor;
        public Type _propertyType;
        public string _format;
        public string _displayName;

        // For common columns
        public PropertyDescriptorForDataGridColumn(PropertyDescriptor pd) : base(pd.Name, null)
        {
            _displayName = pd.DisplayName;
        }

        // For group columns
        public PropertyDescriptorForDataGridColumn(int groupLevel) : base(groupLevel.ToString(), null)
        {
            _groupLevel = groupLevel;
            _displayName = groupLevel.ToString();
        }

        // For group item count column
        public PropertyDescriptorForDataGridColumn(string name) : base(name, null)
        {
            _displayName = name;
        }

        public override string DisplayName => _displayName;
        public override bool CanResetValue(object component) => throw new NotImplementedException();

        public override object GetValue(object component)
        {
            if (component is IDGVList_GroupItem groupItem)
            {
                if (_groupLevel < 0) return groupItem.ItemCount;
                if ((_groupLevel + 1) == groupItem.Level) return groupItem.IsExpanded ? "-" : "+";
            }
            return null;
        }

        public override void ResetValue(object component) => throw new NotImplementedException();
        public override void SetValue(object component, object value) => throw new NotImplementedException();
        public override bool ShouldSerializeValue(object component) => throw new NotImplementedException();
        public override Type ComponentType => null;
        public override bool IsReadOnly => true;
        public override Type PropertyType => _propertyType;
    }
}
