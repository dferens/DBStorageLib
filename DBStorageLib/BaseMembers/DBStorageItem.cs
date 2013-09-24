using System;
using System.Collections.Generic;
using System.Data;
using DBStorageLib.Attributes;

namespace DBStorageLib.BaseMembers
{
    public abstract class DBStorageItem
    {
        private static DBStorage GetStorage(Type classType)
        {
            return DBStorage.GetStorage(classType);
        }

        /// <summary>
        /// Storage item's storage instance
        /// </summary>
        public DBStorage Storage;
        internal DataRow _bindedRow;
        /// <summary>
        /// Storage item's identifier
        /// </summary>
        public Guid ID
        {
            get
            {
                return (Guid)_bindedRow[0];
            }
        }

        public DBStorageItem()
        {
            SetupStorage();
            _bindedRow = Storage.CreateRow();
            Storage.Items.Add(ID, this);
        }
        public DBStorageItem(object nothing)
        {
            SetupStorage();
        }

        /// <summary>
        /// Saves its data fields & properties to corresponding datatable
        /// </summary>
        public virtual void Save()
        {
            foreach (DBMemberInfo dbMemberInfo in Storage.ColumnBindings.Keys)
            {
                DBColumnInfo colInfo = Storage.ColumnBindings[dbMemberInfo];
                _bindedRow[colInfo.Name] = dbMemberInfo.GetValue(this);
            }
        }
        /// <summary>
        /// Fills its data fields & properties with data from corresponding datatable
        /// </summary>
        public virtual void Load()
        {
            foreach (DBMemberInfo dbMemberInfo in Storage.ColumnBindings.Keys)
            {
                DBColumnInfo colInfo = Storage.ColumnBindings[dbMemberInfo];

                try
                {
                    dbMemberInfo.SetValue(this, _bindedRow[colInfo.Name]);
                }
                catch (DBStorageException e)
                {
                    throw new DBStorageException("Exception occured while setting value to instance", e);
                }
            }
        }
        /// <summary>
        /// Deletes itself from corresponding datatable
        /// Override this method in derived class to perform specific deletion logic
        /// </summary>
        public virtual void Delete()
        {
            Storage.Delete(this);
            this.Storage = null;
            this._bindedRow = null;
        }

        internal virtual DBStorage InitStorage(Type classType)
        {
            throw new NotImplementedException("You should implement this");
        }

        private void SetupStorage()
        {
            this.Storage = GetStorage(this.GetType());

            if (this.Storage == null)
            {
                Type thisRealType = this.GetType();
                this.Storage = InitStorage(thisRealType);
            }
        }
    }
}
