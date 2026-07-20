using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES.CORE.Services;

public class SaleStatusService : ISaleStatusService
{
    private readonly ISaleStatusRepository _saleStatusRepository;

    public SaleStatusService(ISaleStatusRepository saleStatusRepository)
    {
        _saleStatusRepository = saleStatusRepository;
    }

    public Task<List<SaleStatusDto>> GetAllAsync() => _saleStatusRepository.GetAllAsync();
}
