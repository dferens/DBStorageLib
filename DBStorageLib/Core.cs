using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DBStorageLib.BaseMembers;

namespace DBStorageLib
{
    public static class Core
    {
        private static bool _initialized = false;
        public static void Init()
        {
            if (_initialized == false)
            {
                _initialized = true;
               IEnumerable<Type> iterator = Assembly.GetCallingAssembly()
                                                    .GetTypes()
                                                    .Where(t => (t.IsSubclassOf(typeof(DBStorageItem))));
                foreach (var type in iterator)
                {
                    if (type.IsSubclassOf(typeof(SQLiteMembers.SQLiteStorageItem)))
                    {
                        new SQLiteMembers.SQLiteStorage(type);
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("DBStorageLib is already initialized");
            }
        }
        public static void Close()
        {
            if (_initialized == true)
            {
                _initialized = false;

                foreach (var storage in DBStorage.Storages)
                {
                    storage.Close();
                }
            }
            else
            {
                throw new InvalidOperationException("DBStorageLib is already closed");
            }
        }
        public static DBStorage GetStorage(Type classType)
        {
            return DBStorage.GetStorage(classType);
        }
    }
}
