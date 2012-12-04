using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using DBStorageLib.Attributes;

namespace DBStorageLib.BaseMembers
{
    internal abstract class DBStorage : IDisposable
	{
        private static readonly Dictionary<Type, DBStorage> _storages = new Dictionary<Type, DBStorage>();
        public static ICollection<DBStorage> Storages
        {
            get
            {
                return _storages.Values;
            }
        }
        public static DBStorage GetStorage(Type classType)
        {
            if (_storages.ContainsKey(classType))
            {
                return _storages[classType];
            }
            else
            {
                return null;
            }
        }

        internal Dictionary<DBMemberInfo, DBColumnInfo> ColumnBindings;
        internal DBDatabaseManager DatabaseManager  { get; set; }
        internal DataTable DataTable                { get; set; }
        internal DbDataAdapter DataAdapter          { get; set; }
        internal string TableName                   { get; set; }
        internal Type ClassType                     { get; set; }
        private bool _disposed = false;

        internal DBStorage(Type classType, DBStorageParamsAttribute attrs)
        {
            ColumnBindings = new Dictionary<DBMemberInfo, DBColumnInfo>();
            ClassType = classType;
            TableName = attrs.CustomTableName ?? classType.Name;
            SetupDatabaseManager(attrs.ConnectionString);
            InitBindings();
            
            if (DatabaseManager.IsTablePresent(TableName))
            {
                DataAdapter = DatabaseManager.CreateDataAdapter(TableName);
                SetupDataTable();

                if (AreColumnsCorrect())
                {
                    LoadFromDisk();

                    _storages.Add(classType, this);
                    return;
                }
                else
                {
                    DatabaseManager.DropTable(TableName);
                }
            }

            DatabaseManager.ConstructTable(TableName, ColumnBindings);
            DatabaseManager.AddTable(DataTable);
            DataAdapter = DatabaseManager.CreateDataAdapter(TableName);
            SetupDataTable();
            DataAdapter.Fill(DataTable);

            _storages.Add(classType, this);
        }

        /// <summary>
        /// In derived class, should initialize DatabaseManager for current DBStorage
        /// </summary>
        /// <param name="connectionString">Connection string, needed to connect to database</param>
        /// <returns>Proper DBDatabaseManager</returns>
        internal abstract DBDatabaseManager InitDatabaseManager(string connectionString);
        /// <summary>
        /// In derived class, should setup internal datatable properly
        /// </summary>
        internal virtual void SetupDataTable()
        {
            DataTable = new DataTable();
        }
        /// <summary>
        /// Creates and inserts new row into its datatable
        /// </summary>
        /// <returns>New DataRow row</returns>
        internal virtual DataRow CreateRow()
        {
            DataRow newRow = DataTable.NewRow();
            DataTable.Rows.Add(newRow);
            return newRow;
        }
        /// <summary>
        /// Loads its datatable from disk
        /// </summary>
        internal virtual void LoadFromDisk()
        {
            DataAdapter.Fill(DataTable);

            foreach (DataRow row in DataTable.Rows)
            {
                DBStorageItem newItem = (DBStorageItem)Activator.CreateInstance(ClassType);
            }
        }
        /// <summary>
        /// Saves its datatable to disk
        /// </summary>
        internal virtual void SaveToDisk()
        {
            DataAdapter.Update(DataTable);
        }

        private void InitBindings()
        {
            ColumnBindings = new Dictionary<DBMemberInfo, DBColumnInfo>();

            foreach (MemberInfo memberInfo in ClassType.GetMembers())
            {
                object[] attributes = memberInfo.GetCustomAttributes(typeof(DBColumnAttribute), false);

                switch (attributes.Length)
                {
                    case 0:
                        {
                            break;
                        }
                    case 1:
                        {
                            DBColumnAttribute dbAttribute = (DBColumnAttribute)attributes[0];

                            string bindingName = dbAttribute.CustomBindingName ?? memberInfo.Name;
                            Type bindingType;

                            if (memberInfo.MemberType == MemberTypes.Field)
                            {
                                bindingType = ((FieldInfo)memberInfo).FieldType;
                            }
                            else if (memberInfo.MemberType == MemberTypes.Property)
                            {
                                bindingType = ((PropertyInfo)memberInfo).PropertyType;
                            }
                            else
                            {
                                throw new Exception("Can save to database fields and properties only");
                            }

                            ColumnBindings.Add(new DBMemberInfo(memberInfo),
                                                new DBColumnInfo(bindingName, bindingType));
                            break;
                        }
                    default:
                        {
                            throw new Exception(string.Format("One <DBColumnAttribute> expected, got {0}", attributes.Length));
                        }
                }
            }
        }
        private bool AreColumnsCorrect()
        {
            // No need to load rows, just synchronizing table columns
            DataAdapter.Fill(0, 1, DataTable);

            if ((ColumnBindings.Count + 1) != DataTable.Columns.Count)
            {
                return false;
            }
            else
            {
                foreach (DBColumnInfo colInfo in ColumnBindings.Values)
                {
                    if (DataTable.Columns.Contains(colInfo.Name))
                    {
                        if (IsCastableTo(colInfo.Type, DataTable.Columns[colInfo.Name].DataType))
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        private void SetupDatabaseManager(string connectionString)
        {
            DatabaseManager = DBDatabaseManager.GetDatabase(connectionString);

            if (DatabaseManager == null)
            {
                DatabaseManager = InitDatabaseManager(connectionString);
            }
        }
        private bool IsCastableTo(Type storedType, Type assignedType)
        {
            return storedType.IsCastableTo(assignedType);
        }

        #region IDisposable
        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                SaveToDisk();

                foreach (DBStorageItem dbItem in DBStorageItem.StorageItems)
                {
                    dbItem.Dispose();
                }
                DatabaseManager.Dispose();

                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}
