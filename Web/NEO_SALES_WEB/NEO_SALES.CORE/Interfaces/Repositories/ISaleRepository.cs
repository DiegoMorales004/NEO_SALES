using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.CORE.Models.Dtos.Sale;

namespace NEO_SALES.CORE.Interfaces.Repositories;

public interface ISaleRepository
{
    Task<List<SaleDto>> GetAllAsync();
    Task<SaleDto?> GetByIdAsync(Guid id);
    Task<List<SaleDto>> GetByCustomerIdAsync(Guid customerId);
    Task<ShoppingCartDto?> GetShoppingCartByCustomerAsync(Guid customerId);
    Task<SaleDto?> CreateAsync(SaleCreateDto dto);
    Task<ConfirmSaleResultDto?> ConfirmAsync(Guid id);
}
