using System;
using System.Threading;
using System.Threading.Tasks;

namespace DAT.Providers
{
    public abstract class DbConnectionWrapper : IDisposable
    {
        protected readonly string connectionString;
        private bool disposed;

        protected DbConnectionWrapper(string connString)
        {
            connectionString = connString;
        }

        public abstract DbCommandWrapper CreateCommand();
        public abstract void Open();
        public abstract Task OpenAsync(CancellationToken cancellationToken);

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

            }

            disposed = true;
        }

        public string ConnectionString => connectionString;
    }
}
