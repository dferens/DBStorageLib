using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DBStorageLib.BaseMembers
{
    public abstract class DBDatabaseManager
    {
        private static Dictionary<string, DBDatabaseManager> _dbmanagers = new Dictionary<string, DBDatabaseManager>();
        internal static DBDatabaseManager GetDatabase(string connectionString)
        {
            if (_dbmanagers.ContainsKey(connectionString))
            {
                return _dbmanagers[connectionString];
            }
            else
            {
                return null;
            }
        }
        public static ICollection<DBDatabaseManager> Managers
        {
            get
            {
                return _dbmanagers.Values;
            }
        }

        internal DbConnection Connection    { get; set; }
        internal DataSet DataSet            { get; set; }
        protected bool _closed = false;

        public DBDatabaseManager(DbConnection connection)
        {
            this.Connection = connection;
            this.Connection.Open();
            this.DataSet = new DataSet();

            _dbmanagers.Add(connection.ConnectionString, this);
        }

        /// <summary>
        /// Checks if sql table with provided name exists in database
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns></returns>
        internal abstract bool IsTablePresent(string tableName);
        /// <summary>
        /// In derived class, checks if database can store value of provided type
        /// </summary>
        /// <param name="type">Provided type</param>
        /// <returns></returns>
        internal abstract bool IsTypeSupported(Type type);
        /// <summary>
        /// Creates and returns data adapter for table with provided name
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns></returns>
        internal abstract DbDataAdapter CreateDataAdapter(string tableName);
        /// <summary>
        /// Get string, that describes database's data type, which can store provided type
        /// </summary>
        /// <param name="columnType">Provided type</param>
        /// <returns></returns>
        internal abstract string GetDatabaseTypeName(Type columnType);
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
            return "GUID PRIMARY KEY";
        }
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
        /// <summary>
        /// Adds datatable to database
        /// </summary>
        /// <param name="dataTable">Provided datatable</param>
        internal void AddTable(DataTable dataTable)
        {
            DataSet.Tables.Add(dataTable);
        }
        /// <summary>
        /// Closes this database manager and frees all resources
        /// </summary>
        internal void Close()
        {
            if (_closed == false)
            {
                _closed = true;
                if (this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                }
                _dbmanagers.Remove(this.Connection.ConnectionString);
            }
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
    }
}
