using Dapper;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Models.Entities;
using NEO_SALES.INFRASTRUCTURE.Data;

namespace NEO_SALES.INFRASTRUCTURE.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private const string SpName = "sp_CUSTOMER";
    private readonly IStoredProcedureExecutor _executor;

    public CustomerRepository(IStoredProcedureExecutor executor)
    {
        _executor = executor;
    }

    public Task<List<Customer>> GetAllAsync()
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET");

        return _executor.QueryAsync<Customer>(SpName, parameters);
    }

    public Task<Customer?> GetByIdAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "GET_BY_ID");
        parameters.Add("p_ID", id);

        return _executor.QuerySingleOrDefaultAsync<Customer>(SpName, parameters);
    }

    public Task<List<Customer>> SearchAsync(string term)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "SEARCH");
        parameters.Add("p_EMAIL", term);

        return _executor.QueryAsync<Customer>(SpName, parameters);
    }

    public async Task<Customer> InsertAsync(Customer customer)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "INSERT");
        parameters.Add("p_NIT", customer.Nit);
        parameters.Add("p_NAME", customer.Name);
        parameters.Add("p_EMAIL", customer.Email);
        parameters.Add("p_STATUS", customer.Status);
        parameters.Add("p_USER_CREATE", customer.UserCreate);

        return await _executor.QuerySingleOrDefaultAsync<Customer>(SpName, parameters)
            ?? throw new InvalidOperationException("sp_CUSTOMER INSERT no devolvio ningun registro");
    }

    public Task<Customer?> UpdateAsync(Customer customer)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "UPDATE");
        parameters.Add("p_ID", customer.Id);
        parameters.Add("p_NIT", customer.Nit);
        parameters.Add("p_NAME", customer.Name);
        parameters.Add("p_EMAIL", customer.Email);
        parameters.Add("p_STATUS", customer.Status);
        parameters.Add("p_USER_UPDATE", customer.UserUpdate);

        return _executor.QuerySingleOrDefaultAsync<Customer>(SpName, parameters);
    }

    public Task DeleteAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_ACTION", "DELETE");
        parameters.Add("p_ID", id);

        return _executor.ExecuteAsync(SpName, parameters);
    }
}
