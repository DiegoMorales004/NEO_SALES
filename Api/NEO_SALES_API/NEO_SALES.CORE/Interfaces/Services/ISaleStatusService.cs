using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES.CORE.Interfaces.Services;

public interface ISaleStatusService
{
    Task<List<SaleStatusDto>> GetAllAsync();
    Task<SaleStatusDto> GetByIdAsync(int id);
    Task<SaleStatusDto> CreateAsync(SaleStatusDto dto);
    Task<SaleStatusDto> EditAsync(int id, SaleStatusEditDto dto);
    Task RemoveAsync(int id);
}
