﻿using System;
using System.ComponentModel;
using System.Reflection;
using DGCore.Common;
using DGCore.DGVList;

namespace DGCore.PD
{
    public class PropertyDescriptorForGroupItemCount: PropertyDescriptor, IMemberDescriptor
    {
        public PropertyDescriptorForGroupItemCount() : base("group_ItemCount", null)
        {
        }

        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object component)
        {
            if (component is IDGVList_GroupItem groupItem)
                return groupItem.ItemCount;
            return null;
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            throw new NotImplementedException();
        }

        public override Type ComponentType => typeof(object);
        public override bool IsReadOnly => true;
        public override Type PropertyType => typeof(int?);

        public MemberKind MemberKind
        {
            get
            {
                throw new Exception($"PD_GroupItem. Not ready!");
            }
        }

        public object DbNullValue => null;
        public MemberInfo ReflectedMemberInfo
        {
            get
            {
                throw new Exception($"PD_GroupItem. Not ready!");
            }
        }
        public Delegate NativeGetter
        {
            get
            {
                throw new Exception($"PD_GroupItem. Not ready!");
            }
        }

        public string Format => "N0";
        public Enums.Alignment? Alignment => Enums.Alignment.Center;
    }
}
