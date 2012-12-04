using System;
using DBStorageLib.Attributes;
using DBStorageLib.BaseMembers;

namespace DBStorageLib.SQLiteMembers
{
    public class SQLiteStorageItem : DBStorageItem
    {
        public SQLiteStorageItem()
            : base() { }

        internal override DBStorage InitStorage(Type classType, DBStorageParamsAttribute attrs)
        {
            SQLiteStorage storageManager = new SQLiteStorage(classType, attrs);
            return storageManager;
        }
        public override void Save()
        {
            base.Save();
            this.Storage.SaveToDisk();
        }
    }
}
