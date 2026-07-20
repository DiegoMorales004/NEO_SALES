using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Exceptions;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES.CORE.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly MessageDefaultsConfiguration _messages;

    public CustomerService(ICustomerRepository customerRepository, IOptions<MessageDefaultsConfiguration> messages)
    {
        _customerRepository = customerRepository;
        _messages = messages.Value;
    }

    public Task<List<CustomerDto>> GetAllAsync() => _customerRepository.GetAllAsync();

    public async Task<CustomerDto> GetByIdAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer ?? throw new ApiNotFoundException(_messages.NotFound);
    }

    public Task<List<CustomerDto>> SearchAsync(string term) => _customerRepository.SearchAsync(term);

    public async Task<CustomerDto> CreateAsync(CustomerCreateDto dto)
    {
        var created = await _customerRepository.CreateAsync(dto);
        return created ?? throw new ApiCommunicationException(_messages.InternalError);
    }

    public async Task<CustomerDto> EditAsync(Guid id, CustomerEditDto dto)
    {
        var updated = await _customerRepository.EditAsync(id, dto);
        return updated ?? throw new ApiNotFoundException(_messages.NotFound);
    }
}
