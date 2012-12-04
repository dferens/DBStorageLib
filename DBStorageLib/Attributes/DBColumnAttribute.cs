using System;

namespace DBStorageLib.Attributes
{
    public class DBColumnAttribute : Attribute
    {
        public string CustomBindingName;

        public DBColumnAttribute(string bindingName)
        {
            this.CustomBindingName = bindingName;
        }
        public DBColumnAttribute()
            : this(null) { }
    }
}