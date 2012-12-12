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

        public DBStorage Storage;
        internal DataRow _bindedRow;
        public Guid ID
        {
            get
            {
                return (Guid)_bindedRow[0];
            }
        }
        protected bool _disposed = false;

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

        public virtual void Save()
        {
            foreach (DBMemberInfo dbMemberInfo in Storage.ColumnBindings.Keys)
            {
                DBColumnInfo colInfo = Storage.ColumnBindings[dbMemberInfo];
                _bindedRow[colInfo.Name] = dbMemberInfo.GetValue(this);
            }
        }
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
