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
        private static readonly Dictionary<long, DBStorageItem> _items = new Dictionary<long, DBStorageItem>();
        public static ICollection<DBStorageItem> StorageItems
        {
            get
            {
                return _items.Values;
            }
        }
        public static DBStorageItem GetStorageItem(long id)
        {
            if (_items.ContainsKey(id))
            {
                return _items[id];
            }
            else
            {
                return null;
            }
        }

        internal DBStorage Storage;
        protected DataRow _bindedRow;
        internal long ID
        {
            get
            {
                return (long)_bindedRow[0];
            }
        }
        private bool _disposed = false;

        public DBStorageItem()
        {
            SetupStorage();
            _bindedRow = Storage.CreateRow();
            _items.Add(ID, this);
            Save();
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
                dbMemberInfo.SetValue(this, _bindedRow[colInfo.Name]);
            }
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

                Storage.Dispose();

                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}
