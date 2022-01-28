using System;
using System.ComponentModel;
using DGCore.PD;
using DGCore.UserSettings;

namespace DGView.ViewModels
{
    public class DGPropertyItemModel
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Format { get; set; }
        public bool IsHidden { get; set; }
        public bool IsFrozen { get; set; }
        public ListSortDirection? IsGrouping { get; set; }
        public bool IsSortingSupport => typeof(IComparable).IsAssignableFrom(DGCore.Utils.Types.GetNotNullableType(PropertyType));

        private Type PropertyType;

        public DGPropertyItemModel(Column column, DGV settings, IMemberDescriptor descriptor)
        {
            Id = column.Id;
            Name = ((PropertyDescriptor)descriptor).DisplayName;
            IsHidden = column.IsHidden;
            Format = descriptor.Format;
            IsFrozen = settings.FrozenColumns.Contains(Id);
            Description = ((PropertyDescriptor)descriptor).Description;
            PropertyType = ((PropertyDescriptor)descriptor).PropertyType;
        }

        public override string ToString() => Name;
    }
}
