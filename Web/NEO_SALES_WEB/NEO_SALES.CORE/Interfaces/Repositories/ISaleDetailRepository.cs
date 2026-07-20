using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES.CORE.Interfaces.Repositories;

public interface ISaleDetailRepository
{
    Task<List<SaleDetailDto>> GetAllAsync();
    Task<SaleDetailDto?> GetByIdAsync(Guid id);
    Task<List<SaleDetailDto>> GetBySaleIdAsync(Guid saleId);
    Task<SaleDetailDto?> CreateAsync(SaleDetailCreateDto dto);
    Task<SaleDetailDto?> EditAsync(Guid id, SaleDetailEditDto dto);
    Task DeleteAsync(Guid id);
}
