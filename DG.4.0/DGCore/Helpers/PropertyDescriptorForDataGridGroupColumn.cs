using System;
using System.ComponentModel;
using DGCore.DGVList;

namespace DGCore.Helpers
{
    public class PropertyDescriptorForDataGridGroupColumn : PropertyDescriptor
    {
        private int _groupLevel = -1;

        // For group columns
        public PropertyDescriptorForDataGridGroupColumn(int groupLevel) : base(groupLevel.ToString(), null) => _groupLevel = groupLevel;

        // For group item count column
        public PropertyDescriptorForDataGridGroupColumn(string name) : base(name, null) {}

        public override string DisplayName => base.DisplayName;
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
        public override Type PropertyType => null;
    }
}
