using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Exceptions;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.CORE.Models.Dtos.Sale;
using NEO_SALES.CORE.Models.Entities;

namespace NEO_SALES.CORE.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleDetailRepository _saleDetailRepository;
    private readonly ICustomerService _customerService;
    private readonly IProductService _productService;
    private readonly MessageDefaultsConfiguration _messages;

    public SaleService(
        ISaleRepository saleRepository,
        ISaleDetailRepository saleDetailRepository,
        ICustomerService customerService,
        IProductService productService,
        IOptions<MessageDefaultsConfiguration> messages)
    {
        _saleRepository = saleRepository;
        _saleDetailRepository = saleDetailRepository;
        _customerService = customerService;
        _productService = productService;
        _messages = messages.Value;
    }

    public async Task<List<SaleDto>> GetAllAsync()
    {
        var sales = await _saleRepository.GetAllAsync();
        return sales.Select(ToDto).ToList();
    }

    public async Task<SaleDto> GetByIdAsync(Guid id)
    {
        var sale = await _saleRepository.GetByIdAsync(id)
            ?? throw new NotFoundCustomException(_messages.NotFound, nameof(Sale), id);

        return ToDto(sale);
    }

    public async Task<List<SaleDto>> GetByCustomerIdAsync(Guid idCustomer)
    {
        var sales = await _saleRepository.GetByCustomerIdAsync(idCustomer);
        return sales.Select(ToDto).ToList();
    }

    public async Task<ShoppingCartDto> GetShoppingCartByCustomerAsync(Guid idCustomer)
    {
        await _customerService.GetByIdAsync(idCustomer);

        var sales = await _saleRepository.GetByCustomerIdAsync(idCustomer);
        var cartSale = sales.FirstOrDefault(sale => sale.StatusId == SaleStatusCodes.Pendiente)
            ?? throw new NotFoundCustomException(_messages.NotFound, "ShoppingCart", idCustomer);

        var details = await _saleDetailRepository.GetBySaleIdAsync(cartSale.Id);

        var items = new List<ShoppingCartItemDto>();

        foreach (var detail in details)
        {
            var product = await _productService.GetByIdAsync(detail.IdProduct);

            items.Add(new ShoppingCartItemDto
            {
                IdSaleDetail = detail.Id,
                IdProduct = product.Id,
                ProductName = product.Name,
                Stock = product.Stock,
                Quantity = detail.Quantity,
                PriceUnit = detail.PriceUnit,
                Subtotal = detail.Quantity * detail.PriceUnit
            });
        }

        return new ShoppingCartDto
        {
            IdSale = cartSale.Id,
            IdCustomer = cartSale.IdCustomer,
            Date = cartSale.Date,
            Items = items,
            Total = items.Sum(item => item.Subtotal)
        };
    }

    public async Task<SaleDto> CreateAsync(SaleCreateDto dto)
    {
        var sale = new Sale
        {
            IdCustomer = dto.IdCustomer,
            StatusId = SaleStatusCodes.Pendiente
        };

        var created = await _saleRepository.InsertAsync(sale);
        return ToDto(created);
    }

    public async Task<SaleDto> EditAsync(Guid id, SaleEditDto dto)
    {
        var existing = await GetByIdAsync(id);

        var sale = new Sale
        {
            Id = id,
            IdCustomer = dto.IdCustomer,
            StatusId = dto.StatusId,
            Date = existing.Date
        };

        var updated = await _saleRepository.UpdateAsync(sale)
            ?? throw new NotFoundCustomException(_messages.NotFound, nameof(Sale), id);

        return ToDto(updated);
    }

    public async Task RemoveAsync(Guid id)
    {
        await GetByIdAsync(id);
        await _saleRepository.DeleteAsync(id);
    }

    public async Task<ConfirmSaleResultDto> ConfirmAsync(Guid id, string actorUser)
    {
        return await _saleRepository.ConfirmSaleAsync(id, actorUser);
    }

    private static SaleDto ToDto(Sale sale) => new()
    {
        Id = sale.Id,
        IdCustomer = sale.IdCustomer,
        StatusId = sale.StatusId,
        Date = sale.Date
    };
}
