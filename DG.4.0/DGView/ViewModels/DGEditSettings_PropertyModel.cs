using System;
using System.ComponentModel;
using DGCore.Common;
using DGCore.PD;
using DGCore.UserSettings;

namespace DGView.ViewModels
{
    public class DGEditSettings_PropertyModel
    {
        [Browsable(false)]
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Format { get; set; }
        public bool IsHidden { get; set; }
        public bool IsFrozen { get; set; }
        public ListSortDirection? IsGrouping { get; set; }
        private Type PropertyType;

        public DGEditSettings_PropertyModel(Column column, DGV settings, IMemberDescriptor descriptor)
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
