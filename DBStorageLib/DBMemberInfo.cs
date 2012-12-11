using System;
using System.Reflection;

namespace DBStorageLib
{
    public class DBMemberInfo
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
                var fieldInfo = (FieldInfo)MemberInfo;

                if (fieldInfo.FieldType == value.GetType())
                {
                    fieldInfo.SetValue(instance, value);
                }
                else
                {
                    try
                    {
                        if (value == DBNull.Value)
                        {
                            fieldInfo.SetValue(instance, null);
                        }
                        else
                        {
                            fieldInfo.SetValue(instance, Convert.ChangeType(value, fieldInfo.FieldType));
                        }
                    }
                    catch (Exception e)
                    {
                        throw new DBStorageException(string.Format("Can not cast '{0}' type to '{1}' type",
                                                     value.GetType(),
                                                     fieldInfo.FieldType),
                                                     e);
                    }
                }
            }
            else
            {
                var propertyInfo = (PropertyInfo)MemberInfo;

                if (propertyInfo.PropertyType == value.GetType())
                {
                    propertyInfo.SetValue(instance, value, null);
                }
                else
                {
                    try
                    {
                        if (value == DBNull.Value)
                        {
                            propertyInfo.SetValue(instance, null, null);
                        }
                        else
                        {
                            propertyInfo.SetValue(instance, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new DBStorageException(string.Format("Can not cast '{0}' type to '{1}' type",
                                                     value.GetType(),
                                                     propertyInfo.PropertyType),
                                                     e);
                    }
                }
            }
        }
    }
}