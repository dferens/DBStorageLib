using System;
using System.Data.SQLite;
using DBStorageLib.Attributes;
using DBStorageLib.BaseMembers;

namespace DBStorageLib.SQLiteMembers
{
    public sealed class SQLiteStorage : DBStorage
    {
        public SQLiteStorage(Type classType, DBStorageParamsAttribute attrs)
            : base(classType, attrs) { }
        public SQLiteStorage(Type classType)
            : base(classType) { }

        internal override DBDatabaseManager InitDatabaseManager(string connectionString)
        {
            return new SQLiteDatabaseManager(new SQLiteConnection(connectionString));
        }
    }
}
