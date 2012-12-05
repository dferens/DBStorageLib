using System;
using System.Runtime.Serialization;

namespace DBStorageLib
{
    internal class DBStorageException : Exception
    {
        public DBStorageException(string message, Exception innerException)
            : base(message, innerException) { }

        public DBStorageException(string message)
            : base(message) { }

        public DBStorageException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public DBStorageException()
            : base() { }
    }
}
