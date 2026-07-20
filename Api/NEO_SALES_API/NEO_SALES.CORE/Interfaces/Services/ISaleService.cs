using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.CORE.Models.Dtos.Sale;

namespace NEO_SALES.CORE.Interfaces.Services;

public interface ISaleService
{
    Task<List<SaleDto>> GetAllAsync();
    Task<SaleDto> GetByIdAsync(Guid id);
    Task<List<SaleDto>> GetByCustomerIdAsync(Guid idCustomer);
    Task<ShoppingCartDto> GetShoppingCartByCustomerAsync(Guid idCustomer);
    Task<SaleDto> CreateAsync(SaleCreateDto dto);
    Task<SaleDto> EditAsync(Guid id, SaleEditDto dto);
    Task RemoveAsync(Guid id);
    Task<ConfirmSaleResultDto> ConfirmAsync(Guid id, string actorUser);
}
