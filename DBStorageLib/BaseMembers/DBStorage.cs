using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using DBStorageLib.Attributes;

namespace DBStorageLib.BaseMembers
{
    public abstract class DBStorage
	{
        internal static DBStorageParamsAttribute GetStorageParams(Type classType)
        {
            object[] classAttributes = classType.GetCustomAttributes(typeof(DBStorageParamsAttribute), false);

            if (classAttributes.Length == 1)
            {
                return (DBStorageParamsAttribute)classAttributes[0];
            }
            else if (classAttributes.Length > 1)
            {
                throw new Exception(string.Format("One <DBStorageParamsAttribute> expected, got {0}", classAttributes.Length));
            }
            else
            {
                return null;
            }
        }
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

        public Dictionary<long, DBStorageItem> Items = new Dictionary<long,DBStorageItem>();
        public Dictionary<DBMemberInfo, DBColumnInfo> ColumnBindings;
        internal DBDatabaseManager DatabaseManager  { get; set; }
        internal DataTable DataTable                { get; set; }
        internal DbDataAdapter DataAdapter          { get; set; }
        internal string TableName                   { get; set; }
        internal Type ClassType                     { get; set; }
        protected bool _closed = false;

        internal DBStorage(Type classType, DBStorageParamsAttribute attrs)
        {
            ColumnBindings = new Dictionary<DBMemberInfo, DBColumnInfo>();
            ClassType = classType;
            TableName = attrs.CustomTableName ?? classType.Name;

            try
            {
                CheckProvidedType();
                SetupDatabaseManager(attrs.ConnectionString);
                InitBindings();
                _storages.Add(classType, this);
            }
            catch (DBStorageException innerException)
            {
                throw new DBStorageException(string.Format("Exception occured while creating DBStorage object for type {0}", classType),
                                             innerException);
            }
            
            if (DatabaseManager.IsTablePresent(TableName))
            {
                DataAdapter = DatabaseManager.CreateDataAdapter(TableName);
                SetupDataTable();

                if (AreColumnsCorrect())
                {
                    LoadFromDisk();
                    return;
                }
                else
                {
                    DatabaseManager.DropTable(TableName);
                }
            }
            DatabaseManager.ConstructTable(TableName, ColumnBindings);
            DataAdapter = DatabaseManager.CreateDataAdapter(TableName);
            SetupDataTable();
            
            DatabaseManager.AddTable(DataTable);
            DataAdapter.Fill(DataTable);
        }
        internal DBStorage(Type classType)
            : this(classType, GetStorageParams(classType)) { }

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
        /// Deletes item from storage
        /// </summary>
        /// <param name="item">Provided item</param>
        internal void Delete(DBStorageItem item)
        {
            Items.Remove(item.ID);
            item._bindedRow.Delete();
        }
        /// <summary>
        /// Closes DBStorage
        /// </summary>
        internal void Close()
        {
            if (_closed == false)
            {
                _closed = true;
                SaveToDisk();
                DatabaseManager.Close();
            }
        }
        /// <summary>
        /// Loads its datatable from disk
        /// </summary>
        public virtual void LoadFromDisk()
        {
            List<long> toDeleteIDs = new List<long>();
            foreach (DBStorageItem item in Items.Values)
            {
                toDeleteIDs.Add(item.ID);
            }
            foreach (long id in toDeleteIDs)
            {
                Items.Remove(id);
            }
            DataTable.Rows.Clear();
            DataAdapter.Fill(DataTable);

            foreach (DataRow row in DataTable.Rows)
            {
                DBStorageItem newItem = (DBStorageItem)Activator.CreateInstance(ClassType);
                newItem._bindedRow = row;
                newItem.Load();
                Items.Add(newItem.ID, newItem);
            }
        }
        /// <summary>
        /// Saves its datatable to disk
        /// </summary>
        public virtual void SaveToDisk()
        {
            foreach (var item in Items.Values)
            {
                item.Save();
            }
            DataAdapter.Update(DataTable);
        }

        private void InitBindings()
        {
            ColumnBindings = new Dictionary<DBMemberInfo, DBColumnInfo>();
            List<MemberInfo> members = new List<MemberInfo>(ClassType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
            members.AddRange(ClassType.GetProperties());

            foreach (MemberInfo memberInfo in members)
            {
                object[] attributes = memberInfo.GetCustomAttributes(typeof(DBColumnAttribute), false);

                switch (attributes.Length)
                {
                    case 0:
                        break;
                    case 1:
                        {
                            DBColumnAttribute dbAttribute = (DBColumnAttribute)attributes[0];

                            string bindingName = dbAttribute.CustomBindingName ?? memberInfo.Name;
                            Type bindingType;

                            switch (memberInfo.MemberType)
                            {
                                case MemberTypes.Field:
                                    {
                                        FieldInfo fieldInfo = (FieldInfo)memberInfo;
                                        bindingType = ((FieldInfo)memberInfo).FieldType;
                                        break;
                                    }

                                case MemberTypes.Property:
                                    {
                                        PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
                                        bindingType = propertyInfo.PropertyType;
                                        break;
                                    }
                                default:
                                        throw new Exception("Can save to database only fields and properties");
                            }

                            if (DatabaseManager.IsTypeSupported(bindingType))
                            {
                                ColumnBindings.Add(new DBMemberInfo(memberInfo),
                                                   new DBColumnInfo(bindingName, bindingType));
                            }
                            else
                            {
                                throw new DBStorageException(string.Format("This storage can not store value of type {0} of '{1}",
                                                                           bindingType, memberInfo.Name));
                            }
                            break;
                        }
                    default:
                        throw new Exception(string.Format("Only one <DBColumnAttribute> attribute is allowed, got {0}", attributes.Length));
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
                        if (colInfo.Type != DataTable.Columns[colInfo.Name].DataType)
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
        private void CheckProvidedType()
        {
            ConstructorInfo defaultConstructor = this.ClassType.GetConstructor(new Type[] { });

            if (defaultConstructor == null)
            {
                throw new DBStorageException("Your class must contain public parameterless constructor, that calls 'base(null)'");
            }
        }
    }
}
