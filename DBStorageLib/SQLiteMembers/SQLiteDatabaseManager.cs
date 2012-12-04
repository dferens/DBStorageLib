using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using DBStorageLib.BaseMembers;

namespace DBStorageLib.SQLiteMembers
{
    internal sealed class SQLiteDatabaseManager : DBDatabaseManager
    {
        internal SQLiteDatabaseManager(DbConnection connection)
            : base(connection) { }

        internal override bool IsTablePresent(string tableName)
        {
            DbCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM sqlite_master WHERE type='table'";
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
            string newPart;

            if (columnType == typeof(short) ||
                columnType == typeof(int)   ||
                columnType == typeof(long))
            {
                newPart = "INTEGER";
            }
            else if (columnType == typeof(double) ||
                     columnType == typeof(float)  ||
                     columnType == typeof(decimal))
            {
                newPart = "REAL";
            }
            else if (columnType == typeof(string) ||
                     columnType == typeof(char))
            {
                newPart = "TEXT";
            }
            else
            {
                // sqlite3 uses a dynamic type system, so any type name you put
                // in your <create table> will work
                newPart = "OBJECT";
            }
            return newPart;
        }
    }
}
