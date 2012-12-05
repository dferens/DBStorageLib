using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using DBStorageLib.BaseMembers;

namespace DBStorageLib.SQLiteMembers
{
    internal sealed class SQLiteDatabaseManager : DBDatabaseManager
    {
        private static Dictionary<Type, string> _sqliteTypeMappings = new Dictionary<Type,string>(){
            { typeof(byte), "TINYINT" },
            { typeof(short), "SMALLINT" },
            { typeof(int), "INT" },
            { typeof(long), "LONG" },
            { typeof(float), "REAL" },
            { typeof(double), "DOUBLE" },
            { typeof(string), "TEXT" },
            { typeof(DateTime), "DATETIME" },
            { typeof(decimal), "NUMERIC" },
            { typeof(Boolean), "BOOL" },
            { typeof(Guid), "GUID" }
        };

        internal SQLiteDatabaseManager(DbConnection connection)
            : base(connection) { }

        internal override bool IsTablePresent(string tableName)
        {
            DbCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM sqlite_master WHERE type='table' AND name='" + tableName + "'";
            bool result = command.ExecuteScalar() != null;
            command.Dispose();
            return result;
        }
        internal override DbDataAdapter CreateDataAdapter(string tableName)
        {
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM " + tableName, (SQLiteConnection)Connection);
            DbCommandBuilder commandBuilder = new SQLiteCommandBuilder(dataAdapter);
            return dataAdapter;
        }
        internal override string GetDatabaseTypeName(Type columnType)
        {
            if (_sqliteTypeMappings.ContainsKey(columnType))
            {
                return _sqliteTypeMappings[columnType];
            }
            else
            {
                return "BLOB";
            }
        }
    }
}
