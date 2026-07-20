using Dapper;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Models.Entities;
using NEO_SALES.INFRASTRUCTURE.Data;

namespace NEO_SALES.INFRASTRUCTURE.Repositories;

public class SaleStatusRepository : ISaleStatusRepository
{
    private const string SpName = "sp_SALE_STATUS";
    private readonly IStoredProcedureExecutor _executor;

    public SaleStatusRepository(IStoredProcedureExecutor executor)
    {
        _executor = executor;
    }

    public Task<List<SaleStatus>> GetAllAsync()
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET");

        return _executor.QueryAsync<SaleStatus>(SpName, parameters);
    }

    public Task<SaleStatus?> GetByIdAsync(int id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET_BY_ID");
        parameters.Add("p_ID", id);

        return _executor.QuerySingleOrDefaultAsync<SaleStatus>(SpName, parameters);
    }

    public Task<List<SaleStatus>> SearchAsync(string term)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "SEARCH");
        parameters.Add("p_NAME", term);

        return _executor.QueryAsync<SaleStatus>(SpName, parameters);
    }

    public async Task<SaleStatus> InsertAsync(SaleStatus saleStatus)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "INSERT");
        parameters.Add("p_ID", saleStatus.Id);
        parameters.Add("p_NAME", saleStatus.Name);
        parameters.Add("p_DESCRIPTION", saleStatus.Description);

        return await _executor.QuerySingleOrDefaultAsync<SaleStatus>(SpName, parameters)
            ?? throw new InvalidOperationException("sp_SALE_STATUS INSERT no devolvio ningun registro");
    }

    public Task<SaleStatus?> UpdateAsync(SaleStatus saleStatus)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "UPDATE");
        parameters.Add("p_ID", saleStatus.Id);
        parameters.Add("p_NAME", saleStatus.Name);
        parameters.Add("p_DESCRIPTION", saleStatus.Description);

        return _executor.QuerySingleOrDefaultAsync<SaleStatus>(SpName, parameters);
    }

    public Task DeleteAsync(int id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "DELETE");
        parameters.Add("p_ID", id);

        return _executor.ExecuteAsync(SpName, parameters);
    }
}
