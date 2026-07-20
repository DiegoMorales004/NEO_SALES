using NEO_SALES.CORE.Models.Entities;

namespace NEO_SALES.CORE.Interfaces.Repositories;

public interface ISaleStatusRepository
{
    Task<List<SaleStatus>> GetAllAsync();
    Task<SaleStatus?> GetByIdAsync(int id);
    Task<List<SaleStatus>> SearchAsync(string term);
    Task<SaleStatus> InsertAsync(SaleStatus saleStatus);
    Task<SaleStatus?> UpdateAsync(SaleStatus saleStatus);
    Task DeleteAsync(int id);
}
