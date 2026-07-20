using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES.CORE.Interfaces.Repositories;

public interface IProductRepository
{
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<List<ProductDto>> SearchAsync(string term);
    Task<ProductDto?> CreateAsync(ProductCreateDto dto);
    Task<ProductDto?> EditAsync(Guid id, ProductEditDto dto);
}
