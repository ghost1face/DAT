using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DAT.Providers
{
    public abstract class DbCommandWrapper : IDisposable
    {
        private readonly DbConnectionWrapper connection;
        private bool disposed;

        protected DbCommandWrapper(DbConnectionWrapper dbConnection)
        {
            connection = dbConnection;
        }

        public DbConnectionWrapper Connection => connection;

        public abstract CommandType CommandType { get; set; }
        public abstract string CommandText { get; set; }
        public abstract int CommandTimeout { get; set; }
        public abstract DbDataReader ExecuteReader();
        public abstract DbDataReader ExecuteReader(CommandBehavior commandBehavior);
        public abstract Task<DbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default);
        public abstract Task<DbDataReader> ExecuteReaderAsync(CommandBehavior commandBehavior, CancellationToken cancellationToken = default);
        public abstract IEnumerable<object> RetrieveStats();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                connection.Dispose();
            }

            disposed = true;
        }
    }
}
