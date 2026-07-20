using Dapper;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Models.Entities;
using NEO_SALES.INFRASTRUCTURE.Data;

namespace NEO_SALES.INFRASTRUCTURE.Repositories;

public class SaleDetailRepository : ISaleDetailRepository
{
    private const string SpName = "sp_SALE_DETAIL";
    private readonly IStoredProcedureExecutor _executor;

    public SaleDetailRepository(IStoredProcedureExecutor executor)
    {
        _executor = executor;
    }

    public Task<List<SaleDetail>> GetAllAsync()
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET");

        return _executor.QueryAsync<SaleDetail>(SpName, parameters);
    }

    public Task<SaleDetail?> GetByIdAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET_BY_ID");
        parameters.Add("p_ID", id);

        return _executor.QuerySingleOrDefaultAsync<SaleDetail>(SpName, parameters);
    }

    public Task<List<SaleDetail>> GetBySaleIdAsync(Guid idSale)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET_BY_ID_SALE");
        parameters.Add("p_ID_SALE", idSale);

        return _executor.QueryAsync<SaleDetail>(SpName, parameters);
    }

    public Task<List<SaleDetail>> GetByProductIdAsync(Guid idProduct)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET_BY_ID_PRODUCT");
        parameters.Add("p_ID_PRODUCT", idProduct);

        return _executor.QueryAsync<SaleDetail>(SpName, parameters);
    }

    public async Task<SaleDetail> InsertAsync(SaleDetail saleDetail)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "INSERT");
        parameters.Add("p_ID_SALE", saleDetail.IdSale);
        parameters.Add("p_ID_PRODUCT", saleDetail.IdProduct);
        parameters.Add("p_QUANTITY", saleDetail.Quantity);
        parameters.Add("p_PRICE_UNIT", saleDetail.PriceUnit);

        return await _executor.QuerySingleOrDefaultAsync<SaleDetail>(SpName, parameters)
            ?? throw new InvalidOperationException("sp_SALE_DETAIL INSERT no devolvio ningun registro");
    }

    public Task<SaleDetail?> UpdateAsync(SaleDetail saleDetail)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "UPDATE");
        parameters.Add("p_ID", saleDetail.Id);
        parameters.Add("p_ID_SALE", saleDetail.IdSale);
        parameters.Add("p_ID_PRODUCT", saleDetail.IdProduct);
        parameters.Add("p_QUANTITY", saleDetail.Quantity);
        parameters.Add("p_PRICE_UNIT", saleDetail.PriceUnit);

        return _executor.QuerySingleOrDefaultAsync<SaleDetail>(SpName, parameters);
    }

    public Task DeleteAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "DELETE");
        parameters.Add("p_ID", id);

        return _executor.ExecuteAsync(SpName, parameters);
    }
}
