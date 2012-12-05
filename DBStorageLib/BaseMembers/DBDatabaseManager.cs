using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DBStorageLib.BaseMembers
{
    internal abstract class DBDatabaseManager : IDisposable
    {
        private static Dictionary<string, DBDatabaseManager> _databases = new Dictionary<string, DBDatabaseManager>();
        internal static DBDatabaseManager GetDatabase(string connectionString)
        {
            if (_databases.ContainsKey(connectionString))
            {
                return _databases[connectionString];
            }
            else
            {
                return null;
            }
        }

        internal DbConnection Connection { get; set; }
        internal DataSet DataSet { get; set; }
        protected bool _disposed = false;

        internal DBDatabaseManager(DbConnection connection)
        {
            this.Connection = connection;
            this.Connection.Open();
            this.DataSet = new DataSet();

            _databases.Add(connection.ConnectionString, this);
        }
        ~DBDatabaseManager()
        {
            Dispose();
        }

        /// <summary>
        /// Checks if sql table with provided name exists in database
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns></returns>
        internal abstract bool IsTablePresent(string tableName);
        /// <summary>
        /// Creates and returns data adapter for table with provided name
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns></returns>
        internal abstract DbDataAdapter CreateDataAdapter(string tableName);
        /// <summary>
        /// Creates "create table" query and executes it
        /// </summary>
        /// <param name="tableName">Name of the new table</param>
        /// <param name="bindings">Member-to-column's binds</param>
        internal virtual void ConstructTable(string tableName, Dictionary<DBMemberInfo, DBColumnInfo> bindings)
        {
            string commandText = string.Format("CREATE TABLE {0}(", tableName);
            List<string> parts = ConstructColumnTypes(bindings);
            commandText += string.Join(", ", parts) + ")";
            DbCommand createCommand = Connection.CreateCommand();
            createCommand.CommandText = commandText;
            createCommand.ExecuteScalar();
            createCommand.Dispose();
        }
        /// <summary>
        /// Get string, that describes database's id column
        /// </summary>
        /// <returns></returns>
        internal virtual string GetIDColumnTypeName()
        {
            return "INTEGER PRIMARY KEY";
        }
        /// <summary>
        /// Get string, that describes database's data type, which can store provided type
        /// </summary>
        /// <param name="columnType">Provided type</param>
        /// <returns></returns>
        internal abstract string GetDatabaseTypeName(Type columnType);
        /// <summary>
        /// Creates "drop table" query and executes it
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        internal virtual void DropTable(string tableName)
        {
            DbCommand dropCommand = Connection.CreateCommand();
            dropCommand.CommandText = "DROP TABLE " + tableName;
            dropCommand.ExecuteScalar();
            dropCommand.Dispose();
        }
        
        internal void AddTable(DataTable dataTable)
        {
            DataSet.Tables.Add(dataTable);
        }

        private List<string> ConstructColumnTypes(Dictionary<DBMemberInfo, DBColumnInfo> bindings)
        {
            string newPart;
            List<string> parts = new List<string>();
            parts.Add(string.Format("ID {0}", GetIDColumnTypeName()));

            foreach (var columnInfo in bindings.Values)
            {
                newPart = GetDatabaseTypeName(columnInfo.Type);
                parts.Add(string.Format("{0} {1}", columnInfo.Name, newPart));
            }
            return parts;
        }

        #region IDisposable
        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                foreach (DBStorage storage in DBStorage.Storages)
                {
                    storage.Dispose();
                }
                Connection.Dispose();

                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}
