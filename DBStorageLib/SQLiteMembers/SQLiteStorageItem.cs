using System;
using DBStorageLib.Attributes;
using DBStorageLib.BaseMembers;

namespace DBStorageLib.SQLiteMembers
{
    public class SQLiteStorageItem : DBStorageItem
    {
        public SQLiteStorageItem()
            : base() { }
        public SQLiteStorageItem(object nothing)
            : base(null) { }

        internal override DBStorage InitStorage(Type classType)
        {
            return new SQLiteStorage(classType);
        }
    }
}
