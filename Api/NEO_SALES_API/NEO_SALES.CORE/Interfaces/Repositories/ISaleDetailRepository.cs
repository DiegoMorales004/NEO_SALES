using NEO_SALES.CORE.Models.Entities;

namespace NEO_SALES.CORE.Interfaces.Repositories;

public interface ISaleDetailRepository
{
    Task<List<SaleDetail>> GetAllAsync();
    Task<SaleDetail?> GetByIdAsync(Guid id);
    Task<List<SaleDetail>> GetBySaleIdAsync(Guid idSale);
    Task<List<SaleDetail>> GetByProductIdAsync(Guid idProduct);
    Task<SaleDetail> InsertAsync(SaleDetail saleDetail);
    Task<SaleDetail?> UpdateAsync(SaleDetail saleDetail);
    Task DeleteAsync(Guid id);
}
