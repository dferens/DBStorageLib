using System;
using System.Data;
using System.Data.Common;
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
        internal override void SetupDataTable()
        {
            base.SetupDataTable();
            //
            // SQLite package for .NET does not set autoincrement fields
            // 
            // Next "Fill" call will load columns only (0 of rows)
            DataAdapter.Fill(0, 0, DataTable);
            DataTable.Columns[0].AutoIncrement = true;
            DataTable.Columns[0].AutoIncrementStep = 1;
            DbCommand cmd = DatabaseManager.Connection.CreateCommand();
            cmd.CommandText = "select last_insert_rowid()";
            DataTable.Columns[0].AutoIncrementSeed = (long)cmd.ExecuteScalar();
        }
    }
}
