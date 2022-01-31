using System;
using System.ComponentModel;
using System.Linq;
using DGCore.PD;
using DGCore.UserSettings;
using DGView.Views;

namespace DGView.ViewModels
{
    public class DGProperty_ItemModel: INotifyPropertyChanged
    {
        private DGEditSettingsView _host;
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Format { get; set; }
        public bool IsHidden { get; set; }
        public bool IsFrozen { get; set; }

        private ListSortDirection? _groupDirection;
        public ListSortDirection? GroupDirection
        {
            get => _groupDirection;
            set
            {
                _groupDirection = value;
                _host.GroupChanged(this);
                OnPropertiesChanged(nameof(GroupDirection));
            }
        }

        public bool IsSortingSupport => typeof(IComparable).IsAssignableFrom(DGCore.Utils.Types.GetNotNullableType(_propertyType));

        private Type _propertyType;

        public DGProperty_ItemModel(DGEditSettingsView host, Column column, DGV settings, IMemberDescriptor descriptor)
        {
            _host = host;
            Id = column.Id;
            Name = ((PropertyDescriptor)descriptor).DisplayName;
            IsHidden = column.IsHidden;
            Format = descriptor.Format;
            
            var item = settings.Groups.FirstOrDefault(o => o.Id == Id);
            if (item != null)
                GroupDirection = item.SortDirection;

            IsFrozen = settings.FrozenColumns.Contains(Id);
            Description = ((PropertyDescriptor)descriptor).Description;
            _propertyType = ((PropertyDescriptor)descriptor).PropertyType;
        }

        public override string ToString() => Name;

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
