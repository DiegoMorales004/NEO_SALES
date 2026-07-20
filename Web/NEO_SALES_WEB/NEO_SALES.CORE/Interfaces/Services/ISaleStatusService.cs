using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES.CORE.Interfaces.Services;

public interface ISaleStatusService
{
    Task<List<SaleStatusDto>> GetAllAsync();
}
