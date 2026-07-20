using Dapper;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Models.Entities;
using NEO_SALES.INFRASTRUCTURE.Data;

namespace NEO_SALES.INFRASTRUCTURE.Repositories;

public class ProductRepository : IProductRepository
{
    private const string SpName = "sp_PRODUCT";
    private readonly IStoredProcedureExecutor _executor;

    public ProductRepository(IStoredProcedureExecutor executor)
    {
        _executor = executor;
    }

    public Task<List<Product>> GetAllAsync()
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET");

        return _executor.QueryAsync<Product>(SpName, parameters);
    }

    public Task<Product?> GetByIdAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET_BY_ID");
        parameters.Add("p_ID", id);

        return _executor.QuerySingleOrDefaultAsync<Product>(SpName, parameters);
    }

    public Task<List<Product>> SearchAsync(string term)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "SEARCH");
        parameters.Add("p_NAME", term);

        return _executor.QueryAsync<Product>(SpName, parameters);
    }

    public async Task<Product> InsertAsync(Product product)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "INSERT");
        parameters.Add("p_NAME", product.Name);
        parameters.Add("p_PRICE", product.Price);
        parameters.Add("p_STOCK", product.Stock);
        parameters.Add("p_STATUS", product.Status);
        parameters.Add("p_USER_CREATE", product.UserCreate);

        return await _executor.QuerySingleOrDefaultAsync<Product>(SpName, parameters)
            ?? throw new InvalidOperationException("sp_PRODUCT INSERT no devolvio ningun registro");
    }

    public Task<Product?> UpdateAsync(Product product)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "UPDATE");
        parameters.Add("p_ID", product.Id);
        parameters.Add("p_NAME", product.Name);
        parameters.Add("p_PRICE", product.Price);
        parameters.Add("p_STOCK", product.Stock);
        parameters.Add("p_STATUS", product.Status);
        parameters.Add("p_DATETIME_UPDATE", DateTime.Now);
        parameters.Add("p_USER_UPDATE", product.UserUpdate);

        return _executor.QuerySingleOrDefaultAsync<Product>(SpName, parameters);
    }

    public Task DeleteAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "DELETE");
        parameters.Add("p_ID", id);

        return _executor.ExecuteAsync(SpName, parameters);
    }
}
