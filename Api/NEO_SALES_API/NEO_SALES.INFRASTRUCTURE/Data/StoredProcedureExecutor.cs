using System.Data;
using Dapper;

namespace NEO_SALES.INFRASTRUCTURE.Data;

public class StoredProcedureExecutor : IStoredProcedureExecutor
{
    private readonly IDbConnectionFactory _connectionFactory;

    static StoredProcedureExecutor()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public StoredProcedureExecutor(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<T>> QueryAsync<T>(string spName, object? parameters = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string spName, object? parameters = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task ExecuteAsync(string spName, object? parameters = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
    }
}
