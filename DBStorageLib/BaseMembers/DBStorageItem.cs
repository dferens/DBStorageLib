using System;
using System.Collections.Generic;
using System.Data;
using DBStorageLib.Attributes;

namespace DBStorageLib.BaseMembers
{
    public abstract class DBStorageItem : IDisposable
    {
        private static DBStorageParamsAttribute GetStorageParams(Type classType)
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
        private static DBStorage GetStorage(Type classType)
        {
            return DBStorage.GetStorage(classType);
        }
        public static readonly Dictionary<long, DBStorageItem> Items = new Dictionary<long, DBStorageItem>();
        public static DBStorageItem GetStorageItem(long id)
        {
            if (Items.ContainsKey(id))
            {
                return Items[id];
            }
            else
            {
                return null;
            }
        }

        public DBStorage Storage;
        internal DataRow _bindedRow;
        public long ID
        {
            get
            {
                return (long)_bindedRow[0];
            }
        }
        protected bool _disposed = false;

        public DBStorageItem()
        {
            SetupStorage();
            _bindedRow = Storage.CreateRow();
            Items.Add(ID, this);
        }
        public DBStorageItem(object nothing)
        {
            SetupStorage();
        }
        ~DBStorageItem()
        {
            Dispose();
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
            Items.Remove(this.ID);
            Storage.DeleteRow(this._bindedRow);
            this.Storage = null;
            this._bindedRow = null;
        }

        internal virtual DBStorage InitStorage(Type classType, DBStorageParamsAttribute attrs)
        {
            throw new NotImplementedException("You should implement this");
        }

        private void SetupStorage()
        {
            this.Storage = GetStorage(this.GetType());

            if (this.Storage == null)
            {
                Type thisRealType = this.GetType();

                this.Storage = InitStorage(thisRealType, GetStorageParams(thisRealType));
            }
        }

        #region IDisposable
        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                Save();

                if (Storage != null)
                {
                    Storage.Dispose();
                }
                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}
