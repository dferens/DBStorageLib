using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using DBStorageLib.BaseMembers;

namespace DBStorageLib.SQLiteMembers
{
    public sealed class SQLiteDatabaseManager : DBDatabaseManager
    {
        private static Dictionary<Type, string> _sqliteSupportedTypeMappings = new Dictionary<Type,string>(){
            { typeof(byte), "TINYINT" },
            { typeof(short), "SMALLINT" },
            { typeof(int), "INT" },
            { typeof(long), "LONG" },
            { typeof(float), "REAL" },
            { typeof(double), "DOUBLE" },
            { typeof(string), "TEXT" },
            { typeof(DateTime), "DATETIME" },
            { typeof(decimal), "NUMERIC" },
            { typeof(bool), "BOOL" },
            { typeof(Guid), "GUID" }
        };

        public SQLiteDatabaseManager(DbConnection connection)
            : base(connection) { }

        internal override bool IsTablePresent(string tableName)
        {
            SQLiteCommand command = new SQLiteCommand((SQLiteConnection)Connection);
            command.CommandText = "SELECT * FROM sqlite_master WHERE type='table' AND name=@name";
            SQLiteParameter nameParametr = command.CreateParameter();
            nameParametr.ParameterName = "@name";
            nameParametr.Value = tableName;
            command.Parameters.Add("@name", System.Data.DbType.String);
            command.Parameters["@name"].Value = tableName;
            bool result = command.ExecuteScalar() != null;
            command.Dispose();
            return result;
        }
        internal override bool IsTypeSupported(Type type)
        {
            return _sqliteSupportedTypeMappings.ContainsKey(type);
        }
        internal override DbDataAdapter CreateDataAdapter(string tableName)
        {
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM " + tableName, (SQLiteConnection)Connection);
            DbCommandBuilder commandBuilder = new SQLiteCommandBuilder(dataAdapter);
            return dataAdapter;
        }
        internal override string GetDatabaseTypeName(Type columnType)
        {
            return _sqliteSupportedTypeMappings[columnType];
        }
    }
}
