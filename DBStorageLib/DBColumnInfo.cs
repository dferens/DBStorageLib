using System;

namespace DBStorageLib
{
    internal class DBColumnInfo
    {
        internal string Name { get; set; }
        internal Type Type { get; set; }

        internal DBColumnInfo(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}