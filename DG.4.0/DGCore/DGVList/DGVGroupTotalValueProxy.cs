using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace DGCore.DGVList
{
    internal class DGVGroupTotalValueProxy : CustomTypeDescriptor, Common.IGetValue
    {
        public override PropertyDescriptorCollection GetProperties() => pdc;
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) => pdc;

        internal PropertyDescriptorCollection pdc;
        internal string Prefix;
        internal Misc.TotalLine[] TotalDefinitions;
        internal decimal?[] TotalValues;

        public object GetValue(string propertyName)
        {
            Debug.Print($"Proxy.GetValue: {Prefix}, {propertyName}, {pdc[0].ComponentType}");
            var fullPropertyName = Prefix + propertyName;
            var propertyNameWithDot = Prefix + propertyName + ".";
            var nestedTotalValues = new List<decimal?>();
            var nestedTotalDefinitions = new List<Misc.TotalLine>();
            for (var i = 0; i < TotalDefinitions.Length; i++)
            {
                if (TotalDefinitions[i].Id == fullPropertyName)
                    return TotalValues[i];

                if (TotalDefinitions[i].Id.StartsWith(propertyNameWithDot))
                {
                    nestedTotalDefinitions.Add(TotalDefinitions[i]);
                    nestedTotalValues.Add(TotalValues[i]);
                }
            }

            if (nestedTotalDefinitions.Count > 0)
            {
                throw new Exception($"Trap!!! DGVGroupTotalValueProxy. Prefix+property: {Prefix},{propertyName}");
                /*var propertyType = pdc[propertyName].PropertyType;
                return new DGVGroupTotalValueProxy
                {
                    pdc = PD.MemberDescriptorUtils.GetTypeMembers(propertyType),
                    Prefix = propertyNameWithDot,
                    TotalDefinitions = nestedTotalDefinitions.ToArray(),
                    TotalValues = nestedTotalValues.ToArray()
                };*/
            }

            return null;
        }
    }
}
