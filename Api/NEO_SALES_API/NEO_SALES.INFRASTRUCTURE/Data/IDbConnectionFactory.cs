using System.Data;

namespace NEO_SALES.INFRASTRUCTURE.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
