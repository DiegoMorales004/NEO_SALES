using NEO_SALES.CORE.Models.Entities;

namespace NEO_SALES.CORE.Interfaces.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task<List<Product>> SearchAsync(string term);
    Task<Product> InsertAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
}
