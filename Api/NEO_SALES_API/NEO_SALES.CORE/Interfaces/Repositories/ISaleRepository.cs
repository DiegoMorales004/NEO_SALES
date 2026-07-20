using NEO_SALES.CORE.Models.Dtos.Sale;
using NEO_SALES.CORE.Models.Entities;

namespace NEO_SALES.CORE.Interfaces.Repositories;

public interface ISaleRepository
{
    Task<List<Sale>> GetAllAsync();
    Task<Sale?> GetByIdAsync(Guid id);
    Task<List<Sale>> GetByStatusIdAsync(int statusId);
    Task<List<Sale>> GetByCustomerIdAsync(Guid idCustomer);
    Task<Sale> InsertAsync(Sale sale);
    Task<Sale?> UpdateAsync(Sale sale);
    Task DeleteAsync(Guid id);
    Task TouchUpdateDateAsync(Guid id);
    Task<ConfirmSaleResultDto> ConfirmSaleAsync(Guid id, string? actorUser);
    Task<List<Guid>> ExpireSalesAsync(int maxMinutes);
}
