using System;
using System.Data.Common;

namespace DBStorageLib.Attributes
{
    /// <summary>
    /// Add this attribute to your class and specify connection string where it will be stored
    /// </summary>
    public class DBStorageParamsAttribute : Attribute
    {
        public string ConnectionString;
        public string CustomTableName;

        public DBStorageParamsAttribute(string connectionString, string tableName)
        {
            this.ConnectionString = connectionString;
            this.CustomTableName = tableName;
        }
        public DBStorageParamsAttribute(string connectionString)
            : this(connectionString, null) { }
    }
}
