using NEO_SALES.CORE.Models.Entities;

namespace NEO_SALES.CORE.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(Guid id);
    Task<List<Customer>> SearchAsync(string term);
    Task<Customer> InsertAsync(Customer customer);
    Task<Customer?> UpdateAsync(Customer customer);
    Task DeleteAsync(Guid id);
}
