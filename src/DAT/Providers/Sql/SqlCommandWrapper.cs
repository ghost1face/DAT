using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace DAT.Providers.Sql
{
    public class SqlCommandWrapper : DbCommandWrapper
    {
        private readonly SqlCommand sqlCommand;
        private readonly StatisticsParser parser;

        public SqlCommandWrapper(SqlConnectionWrapper connection)
            : base(connection)
        {
            parser = new StatisticsParser();
            sqlCommand = new SqlCommand
            {
                Connection = connection.sqlConnection
            };
        }

        public override CommandType CommandType
        {
            get => sqlCommand.CommandType;
            set => sqlCommand.CommandType = value;
        }

        public override string CommandText
        {
            get => sqlCommand.CommandText;
            set => sqlCommand.CommandText = value;
        }

        public override int CommandTimeout
        {
            get => sqlCommand.CommandTimeout;
            set => sqlCommand.CommandTimeout = value;
        }

        public override IEnumerable<object> RetrieveStats()
        {
            return parser.ParseStatistics(
                ((SqlConnectionWrapper)this.Connection).InfoMessages.ToString()
            );
        }

        public IEnumerable<QueryStats> RetrieveQueryStats()
        {
            return (IEnumerable<QueryStats>)RetrieveStats();
        }

        public override DbDataReader ExecuteReader()
        {
            return sqlCommand.ExecuteReader();
        }

        public override DbDataReader ExecuteReader(CommandBehavior commandBehavior)
        {
            return sqlCommand.ExecuteReader(commandBehavior);
        }

        public override async Task<DbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default)
        {
            return await sqlCommand.ExecuteReaderAsync(cancellationToken);
        }

        public override async Task<DbDataReader> ExecuteReaderAsync(CommandBehavior commandBehavior, CancellationToken cancellationToken = default)
        {
            return await sqlCommand.ExecuteReaderAsync(commandBehavior, cancellationToken);
        }
    }
}
