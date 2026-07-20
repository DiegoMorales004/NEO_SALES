using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Models.Configuration;

namespace NEO_SALES.INFRASTRUCTURE.Data;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IOptions<DbConfiguration> dbConfiguration)
    {
        var config = dbConfiguration.Value;

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = config.Server,
            InitialCatalog = config.Database,
            UserID = config.User,
            Password = config.Password,
            TrustServerCertificate = true
        };

        _connectionString = builder.ConnectionString;
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
