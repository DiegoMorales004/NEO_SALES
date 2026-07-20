using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Exceptions;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.CORE.Models.Entities;

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

    public async Task<List<CustomerDto>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(ToDto).ToList();
    }

    public async Task<CustomerDto> GetByIdAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id)
            ?? throw new NotFoundCustomException(_messages.NotFound, nameof(Customer), id);

        return ToDto(customer);
    }

    public async Task<List<CustomerDto>> SearchAsync(string term)
    {
        var customers = await _customerRepository.SearchAsync(term);
        return customers.Select(ToDto).ToList();
    }

    public async Task<CustomerDto> CreateAsync(CustomerCreateDto dto, string actorUser)
    {
        var customer = new Customer
        {
            Name = dto.Name,
            Nit = dto.Nit,
            Email = dto.Email,
            Status = true,
            UserCreate = actorUser
        };

        var created = await _customerRepository.InsertAsync(customer);
        return ToDto(created);
    }

    public async Task<CustomerDto> EditAsync(Guid id, CustomerEditDto dto, string actorUser)
    {
        await GetByIdAsync(id);

        var customer = new Customer
        {
            Id = id,
            Name = dto.Name,
            Nit = dto.Nit,
            Email = dto.Email,
            Status = dto.Status,
            UserUpdate = actorUser
        };

        var updated = await _customerRepository.UpdateAsync(customer)
            ?? throw new NotFoundCustomException(_messages.NotFound, nameof(Customer), id);

        return ToDto(updated);
    }

    public async Task RemoveAsync(Guid id)
    {
        await GetByIdAsync(id);
        await _customerRepository.DeleteAsync(id);
    }

    private static CustomerDto ToDto(Customer customer) => new()
    {
        Id = customer.Id,
        Name = customer.Name,
        Nit = customer.Nit,
        Email = customer.Email,
        Status = customer.Status,
        DatetimeCreate = customer.DatetimeCreate,
        DatetimeUpdate = customer.DatetimeUpdate
    };
}
