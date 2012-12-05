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

        internal override DBStorage InitStorage(Type classType, DBStorageParamsAttribute attrs)
        {
            SQLiteStorage storageManager = new SQLiteStorage(classType, attrs);
            return storageManager;
        }
        public override void Save()
        {
            // SQLiteCommand instances have strange behavior : they are already disposed in any instance destructor.
            // Instead, SqlCommand, OleDbCommand, OdbcCommand are not;
            // So, we can not use SQLiteCommand, SQLiteAdapter and SQLiteDataReader in destructors.
            // We forces Storage update after every StorageItem update (yes, it is not efficient)
            // Maybe there is a better solution for this issue.
            base.Save();
            this.Storage.SaveToDisk();
        }
        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                // Removed "Save" method call
                if (Storage != null)
                {
                    Storage.Dispose();
                }
                GC.SuppressFinalize(this);
            }
        }
    }
}
