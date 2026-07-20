using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES.CORE.Interfaces.Services;

public interface ICustomerService
{
    Task<List<CustomerDto>> GetAllAsync();
    Task<CustomerDto> GetByIdAsync(Guid id);
    Task<List<CustomerDto>> SearchAsync(string term);
    Task<CustomerDto> CreateAsync(CustomerCreateDto dto, string actorUser);
    Task<CustomerDto> EditAsync(Guid id, CustomerEditDto dto, string actorUser);
    Task RemoveAsync(Guid id);
}
