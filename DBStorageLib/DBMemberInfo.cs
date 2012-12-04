using System;
using System.Reflection;

namespace DBStorageLib
{
    internal class DBMemberInfo
    {
        internal MemberInfo MemberInfo { get; set; }
        internal string Name
        {
            get
            {
                return this.MemberInfo.Name;
            }
        }
        internal Type Type
        {
            get
            {
                if (MemberInfo.MemberType == MemberTypes.Field)
                {
                    return ((FieldInfo)MemberInfo).FieldType;
                }
                else
                {
                    return ((PropertyInfo)MemberInfo).PropertyType;
                }
            }
        }

        internal DBMemberInfo(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType != MemberTypes.Field &&
                memberInfo.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("Can save fields and properties only", "memberInfo");
            }
            else
            {
                this.MemberInfo = memberInfo;
            }
        }

        internal object GetValue(object instance)
        {
            if (MemberInfo.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)MemberInfo).GetValue(instance);
            }
            else
            {
                return ((PropertyInfo)MemberInfo).GetValue(instance, null);
            }
        }
        internal void SetValue(object instance, object value)
        {
            if (MemberInfo.MemberType == MemberTypes.Field)
            {
                ((FieldInfo)MemberInfo).SetValue(instance, value);
            }
            else
            {
                ((PropertyInfo)MemberInfo).SetValue(instance, value, null);
            }
        }
    }
}