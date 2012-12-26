using System;
using DBStorageLib.BaseMembers;

namespace DBStorageLib
{
    /// <summary>
    /// Use this class to initialize/stop storage service
    /// </summary>
    public static class Core
    {
        private static bool _initialized = false;
        /// <summary>
        /// Initializes storaging service.
        /// Call this method before any storage-associated operations
        /// </summary>
        public static void Init()
        {
            if (_initialized == false)
            {
                _initialized = true;
            }
            else
            {
                throw new InvalidOperationException("DBStorageLib is already initialized");
            }
        }
        /// <summary>
        /// Closes storaging service.
        /// Call this method after last storage-associated operation.
        /// </summary>
        public static void Close()
        {
            if (_initialized == true)
            {
                _initialized = false;

                // Closing storages
                DBStorage[] deleteStorages = new DBStorage[DBStorage.Storages.Count];
                DBStorage.Storages.CopyTo(deleteStorages, 0);
                foreach (var storage in deleteStorages)
                {
                    storage.Close();
                }

                // Closing database managers
                DBDatabaseManager[] deleteDatabaseManagers = new DBDatabaseManager[DBDatabaseManager.Managers.Count];
                DBDatabaseManager.Managers.CopyTo(deleteDatabaseManagers, 0);
                foreach (var dbmanager in deleteDatabaseManagers)
                {
                    dbmanager.Close();
                }
            }
            else
            {
                throw new InvalidOperationException("DBStorageLib is already closed");
            }
        }
        /// <summary>
        /// Returns object that stores object of <paramref name="classType"/> type
        /// </summary>
        /// <param name="classType">Provided type</param>
        /// <returns></returns>
        public static DBStorage GetStorage(Type classType)
        {
            return DBStorage.GetStorage(classType);
        }
    }
}
