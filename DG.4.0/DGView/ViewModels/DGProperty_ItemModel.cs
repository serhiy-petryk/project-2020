using System;
using System.ComponentModel;
using DGCore.PD;
using DGCore.UserSettings;

namespace DGView.ViewModels
{
    public class DGProperty_ItemModel
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Format { get; set; }
        public bool IsHidden { get; set; }
        public bool IsFrozen { get; set; }
        public ListSortDirection? IsGrouping { get; set; }
        public bool IsSortingSupport => typeof(IComparable).IsAssignableFrom(DGCore.Utils.Types.GetNotNullableType(_propertyType));

        private Type _propertyType;

        public DGProperty_ItemModel(Column column, DGV settings, IMemberDescriptor descriptor)
        {
            Id = column.Id;
            Name = ((PropertyDescriptor)descriptor).DisplayName;
            IsHidden = column.IsHidden;
            Format = descriptor.Format;
            IsFrozen = settings.FrozenColumns.Contains(Id);
            Description = ((PropertyDescriptor)descriptor).Description;
            _propertyType = ((PropertyDescriptor)descriptor).PropertyType;
        }

        public override string ToString() => Name;
    }
}
