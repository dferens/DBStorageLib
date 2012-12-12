using System;

namespace DBStorageLib.Attributes
{
    /// <summary>
    /// Add this attribute to any public/private field/property to save it to database
    /// </summary>
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