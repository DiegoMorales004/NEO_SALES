using System.Data;
using Dapper;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Models.Dtos.Sale;
using NEO_SALES.CORE.Models.Entities;
using NEO_SALES.INFRASTRUCTURE.Data;

namespace NEO_SALES.INFRASTRUCTURE.Repositories;

public class SaleRepository : ISaleRepository
{
    private const string SpName = "sp_SALE";
    private readonly IStoredProcedureExecutor _executor;
    private readonly IDbConnectionFactory _connectionFactory;

    public SaleRepository(IStoredProcedureExecutor executor, IDbConnectionFactory connectionFactory)
    {
        _executor = executor;
        _connectionFactory = connectionFactory;
    }

    public Task<List<Sale>> GetAllAsync()
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET");

        return _executor.QueryAsync<Sale>(SpName, parameters);
    }

    public Task<Sale?> GetByIdAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET_BY_ID");
        parameters.Add("p_ID", id);

        return _executor.QuerySingleOrDefaultAsync<Sale>(SpName, parameters);
    }

    public Task<List<Sale>> GetByStatusIdAsync(int statusId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET_BY_STATUS_ID");
        parameters.Add("p_STATUS_ID", statusId);

        return _executor.QueryAsync<Sale>(SpName, parameters);
    }

    public Task<List<Sale>> GetByCustomerIdAsync(Guid idCustomer)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET_BY_ID_CUSTOMER");
        parameters.Add("p_ID_CUSTOMER", idCustomer);

        return _executor.QueryAsync<Sale>(SpName, parameters);
    }

    public async Task<Sale> InsertAsync(Sale sale)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "INSERT");
        parameters.Add("p_ID_CUSTOMER", sale.IdCustomer);
        parameters.Add("p_STATUS_ID", sale.StatusId);

        return await _executor.QuerySingleOrDefaultAsync<Sale>(SpName, parameters)
            ?? throw new InvalidOperationException("sp_SALE INSERT no devolvio ningun registro");
    }

    public Task<Sale?> UpdateAsync(Sale sale)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "UPDATE");
        parameters.Add("p_ID", sale.Id);
        parameters.Add("p_ID_CUSTOMER", sale.IdCustomer);
        parameters.Add("p_STATUS_ID", sale.StatusId);
        parameters.Add("p_DATE", sale.Date);

        return _executor.QuerySingleOrDefaultAsync<Sale>(SpName, parameters);
    }

    public Task DeleteAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "DELETE");
        parameters.Add("p_ID", id);

        return _executor.ExecuteAsync(SpName, parameters);
    }

    public Task TouchUpdateDateAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "TOUCH_DATE");
        parameters.Add("p_ID", id);

        return _executor.ExecuteAsync(SpName, parameters);
    }

    public async Task<ConfirmSaleResultDto> ConfirmSaleAsync(Guid id, string? actorUser)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("p_ID_SALE", id);
        parameters.Add("p_USER_UPDATE", actorUser);

        using var grid = await connection.QueryMultipleAsync(
            "sp_CONFIRM_SALE", parameters, commandType: CommandType.StoredProcedure);

        var header = await grid.ReadSingleAsync<ConfirmSaleHeaderRow>();

        var result = new ConfirmSaleResultDto
        {
            Success = header.Success,
            Message = header.Message
        };

        if (!grid.IsConsumed)
        {
            var lines = await grid.ReadAsync<ConfirmSaleLineRow>();
            result.Lines = lines.Select(line => new ConfirmSaleLineDto
            {
                IdProduct = line.IdProduct,
                Name = line.Name,
                QuantityRequested = line.QuantityRequested ?? line.QuantitySold ?? 0,
                StockAvailable = line.StockAvailable,
                QuantityMissing = line.QuantityMissing,
                StockRemaining = line.StockRemaining
            }).ToList();
        }

        return result;
    }

    public async Task<List<Guid>> ExpireSalesAsync(int maxMinutes)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("p_MAX_MINUTES", maxMinutes);

        var rows = await connection.QueryAsync<ExpiredSaleRow>(
            "sp_EXPIRE_SALES", parameters, commandType: CommandType.StoredProcedure);

        return rows.Select(row => row.Id).ToList();
    }

    private class ConfirmSaleHeaderRow
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    private class ConfirmSaleLineRow
    {
        public Guid IdProduct { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? QuantityRequested { get; set; }
        public int? QuantitySold { get; set; }
        public int? StockAvailable { get; set; }
        public int? QuantityMissing { get; set; }
        public int? StockRemaining { get; set; }
    }

    private class ExpiredSaleRow
    {
        public Guid Id { get; set; }
    }
}
