using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Exceptions;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.CORE.Models.Dtos.Sale;

namespace NEO_SALES.CORE.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ISaleStatusRepository _saleStatusRepository;
    private readonly MessageDefaultsConfiguration _messages;

    public SaleService(
        ISaleRepository saleRepository,
        ICustomerRepository customerRepository,
        ISaleStatusRepository saleStatusRepository,
        IOptions<MessageDefaultsConfiguration> messages)
    {
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
        _saleStatusRepository = saleStatusRepository;
        _messages = messages.Value;
    }

    public async Task<List<SaleSummaryDto>> GetAllAsync()
    {
        var sales = await _saleRepository.GetAllAsync();
        var customers = await _customerRepository.GetAllAsync();
        var statuses = await _saleStatusRepository.GetAllAsync();

        return MapSummaries(sales, customers, statuses);
    }

    public async Task<SaleSummaryDto> GetByIdAsync(Guid id)
    {
        var sale = await _saleRepository.GetByIdAsync(id) ?? throw new ApiNotFoundException(_messages.NotFound);
        var customer = await _customerRepository.GetByIdAsync(sale.IdCustomer);
        var statuses = await _saleStatusRepository.GetAllAsync();

        return MapSummary(sale, customer?.Name ?? "-", statuses);
    }

    public async Task<List<SaleSummaryDto>> GetByCustomerIdAsync(Guid customerId)
    {
        var sales = await _saleRepository.GetByCustomerIdAsync(customerId);
        var customer = await _customerRepository.GetByIdAsync(customerId);
        var statuses = await _saleStatusRepository.GetAllAsync();

        return sales.Select(sale => MapSummary(sale, customer?.Name ?? "-", statuses)).ToList();
    }

    public async Task<ShoppingCartDto> GetShoppingCartByCustomerAsync(Guid customerId)
    {
        var cart = await _saleRepository.GetShoppingCartByCustomerAsync(customerId);
        return cart ?? throw new ApiNotFoundException(_messages.NotFound);
    }

    public async Task<SaleDto> CreateAsync(SaleCreateDto dto)
    {
        var created = await _saleRepository.CreateAsync(dto);
        return created ?? throw new ApiCommunicationException(_messages.InternalError);
    }

    public async Task<ConfirmSaleResultDto> ConfirmAsync(Guid id)
    {
        var result = await _saleRepository.ConfirmAsync(id);
        return result ?? throw new ApiCommunicationException(_messages.InternalError);
    }

    public Task<List<SaleStatusDto>> GetStatusesAsync() => _saleStatusRepository.GetAllAsync();

    private static List<SaleSummaryDto> MapSummaries(List<SaleDto> sales, List<CustomerDto> customers, List<SaleStatusDto> statuses)
    {
        var customerNames = customers.ToDictionary(c => c.Id, c => c.Name);
        return sales
            .Select(sale => MapSummary(sale, customerNames.GetValueOrDefault(sale.IdCustomer, "-"), statuses))
            .ToList();
    }

    private static SaleSummaryDto MapSummary(SaleDto sale, string customerName, List<SaleStatusDto> statuses)
    {
        var statusName = statuses.FirstOrDefault(s => s.Id == sale.StatusId)?.Name ?? "-";

        return new SaleSummaryDto
        {
            Id = sale.Id,
            IdCustomer = sale.IdCustomer,
            CustomerName = customerName,
            StatusId = sale.StatusId,
            StatusName = statusName,
            Date = sale.Date
        };
    }
}
