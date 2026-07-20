using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES.CORE.Interfaces.Repositories;

public interface ISaleStatusRepository
{
    Task<List<SaleStatusDto>> GetAllAsync();
    Task<SaleStatusDto?> GetByIdAsync(int id);
}
