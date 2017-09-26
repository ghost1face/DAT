using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAT.Providers.Sql
{
    public class SqlConnectionWrapper : DbConnectionWrapper
    {
        internal readonly SqlConnection sqlConnection;
        private readonly StringBuilder infoMessageBuilder;
        private bool disposed;

        public SqlConnectionWrapper(string connectionString)
            : base(connectionString)
        {
            infoMessageBuilder = new StringBuilder();
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.InfoMessage += new SqlInfoMessageEventHandler((sender, data) =>
            {
                infoMessageBuilder.AppendLine(data.Message);
            });
        }

        public StringBuilder InfoMessages
        {
            get => infoMessageBuilder;
        }

        public override DbCommandWrapper CreateCommand()
        {
            var sqlCommand = sqlConnection.CreateCommand();

            return new SqlCommandWrapper(this);
        }

        public override void Open()
        {
            this.Open();
        }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return this.OpenAsync(cancellationToken: cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                sqlConnection.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
