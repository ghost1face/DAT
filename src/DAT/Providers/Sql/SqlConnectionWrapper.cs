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
            sqlConnection.Open();

            var command = sqlConnection.CreateCommand();
            command.CommandText = "SET STATISTICS IO ON; SET STATISTICS TIME ON;";

            command.ExecuteNonQuery();
        }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return sqlConnection.OpenAsync(cancellationToken: cancellationToken)
                .ContinueWith(async t =>
                {
                    var command = sqlConnection.CreateCommand();
                    command.CommandText = "SET STATISTICS IO ON; SET STATISTICS TIME ON;";

                    await command.ExecuteNonQueryAsync();
                }).Unwrap();
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

            disposed = true;
        }
    }
}
