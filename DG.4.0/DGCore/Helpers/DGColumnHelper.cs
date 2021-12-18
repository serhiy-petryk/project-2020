using System;
using System.ComponentModel;
using DGCore.PD;

namespace DGCore.Helpers
{
    public class DGColumnHelper
    {
        public string Name { get; }
        public string DisplayName { get; }
        public string Format { get; }
        public object GetValue(object component) => _getter(component);

        public Type ValueType { get; }

        private Func<object, object> _getter;
        // For common columns
        public DGColumnHelper(PropertyDescriptor pd)
        {
            Name = pd.Name;
            DisplayName = pd.DisplayName;
            ValueType = pd.PropertyType;
            Format = ((IMemberDescriptor)pd).Format;
            _getter = o => pd.GetValue(o);
        }
    }
}
