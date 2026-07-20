namespace NEO_SALES.INFRASTRUCTURE.Data;

public interface IStoredProcedureExecutor
{
    Task<List<T>> QueryAsync<T>(string spName, object? parameters = null);
    Task<T?> QuerySingleOrDefaultAsync<T>(string spName, object? parameters = null);
    Task ExecuteAsync(string spName, object? parameters = null);
}
