using System;
using System.Data.Common;

namespace DBStorageLib.Attributes
{
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
