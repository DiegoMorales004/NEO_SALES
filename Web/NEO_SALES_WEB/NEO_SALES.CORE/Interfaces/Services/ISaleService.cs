using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.CORE.Models.Dtos.Sale;

namespace NEO_SALES.CORE.Interfaces.Services;

public interface ISaleService
{
    Task<List<SaleSummaryDto>> GetAllAsync();
    Task<SaleSummaryDto> GetByIdAsync(Guid id);
    Task<List<SaleSummaryDto>> GetByCustomerIdAsync(Guid customerId);
    Task<ShoppingCartDto> GetShoppingCartByCustomerAsync(Guid customerId);
    Task<SaleDto> CreateAsync(SaleCreateDto dto);
    Task<ConfirmSaleResultDto> ConfirmAsync(Guid id);
    Task<List<SaleStatusDto>> GetStatusesAsync();
}
