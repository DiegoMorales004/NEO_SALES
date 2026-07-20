using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Exceptions;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.CORE.Models.Entities;

namespace NEO_SALES.CORE.Services;

public class SaleDetailService : ISaleDetailService
{
    private readonly ISaleDetailRepository _saleDetailRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IProductService _productService;
    private readonly ISaleService _saleService;
    private readonly MessageDefaultsConfiguration _messages;

    public SaleDetailService(
        ISaleDetailRepository saleDetailRepository,
        ISaleRepository saleRepository,
        IOptions<MessageDefaultsConfiguration> messages,
        IProductService productService,
        ISaleService saleService)
    {
        _saleDetailRepository = saleDetailRepository;
        _saleRepository = saleRepository;
        _messages = messages.Value;
        _productService = productService;
        _saleService = saleService;
    }

    public async Task<List<SaleDetailDto>> GetAllAsync()
    {
        var details = await _saleDetailRepository.GetAllAsync();
        return details.Select(ToDto).ToList();
    }

    public async Task<SaleDetailDto> GetByIdAsync(Guid id)
    {
        var detail = await _saleDetailRepository.GetByIdAsync(id)
            ?? throw new NotFoundCustomException(_messages.NotFound, nameof(SaleDetail), id);

        return ToDto(detail);
    }

    public async Task<List<SaleDetailDto>> GetBySaleIdAsync(Guid idSale)
    {
        var details = await _saleDetailRepository.GetBySaleIdAsync(idSale);
        return details.Select(ToDto).ToList();
    }

    public async Task<SaleDetailDto> CreateAsync(SaleDetailCreateDto dto)
    {
        await ValidateSaleIsModifiableAsync(dto.IdSale);
        var product = await ValidateProductAndQuantityAsync(dto.IdProduct, dto.Quantity);

        var detail = new SaleDetail
        {
            IdSale = dto.IdSale,
            IdProduct = dto.IdProduct,
            Quantity = dto.Quantity,
            PriceUnit = product.Price
        };

        var created = await _saleDetailRepository.InsertAsync(detail);
        await _saleRepository.TouchUpdateDateAsync(created.IdSale);

        return ToDto(created);
    }

    public async Task<SaleDetailDto> EditAsync(Guid id, SaleDetailEditDto dto)
    {
        var existing = await GetByIdAsync(id);

        await ValidateSaleIsModifiableAsync(existing.IdSale);
        var product = await ValidateProductAndQuantityAsync(dto.IdProduct, dto.Quantity);

        var detail = new SaleDetail
        {
            Id = id,
            IdSale = existing.IdSale,
            IdProduct = dto.IdProduct,
            Quantity = dto.Quantity,
            PriceUnit = product.Price
        };

        var updated = await _saleDetailRepository.UpdateAsync(detail)
            ?? throw new NotFoundCustomException(_messages.NotFound, nameof(SaleDetail), id);

        await _saleRepository.TouchUpdateDateAsync(updated.IdSale);

        return ToDto(updated);
    }

    public async Task RemoveAsync(Guid id)
    {
        var existing = await GetByIdAsync(id);

        await _saleDetailRepository.DeleteAsync(id);
        await _saleRepository.TouchUpdateDateAsync(existing.IdSale);
    }

    private async Task ValidateSaleIsModifiableAsync(Guid idSale)
    {
        SaleDto sale;

        try
        {
            sale = await _saleService.GetByIdAsync(idSale);
        }
        catch (NotFoundCustomException)
        {
            throw new BadRequestCustomException($"La venta {idSale} no existe.");
        }

        if (sale.StatusId is SaleStatusCodes.Cancelada or SaleStatusCodes.Expirada)
        {
            throw new BadRequestCustomException("La venta se encuentra cancelada o expirada, no se pueden modificar sus detalles");
        }
    }

    private async Task<ProductDto> ValidateProductAndQuantityAsync(Guid idProduct, int quantity)
    {
        if (quantity <= 0)
        {
            throw new BadRequestCustomException("La cantidad debe ser mayor a cero");
        }

        ProductDto product;

        try
        {
            product = await _productService.GetByIdAsync(idProduct);
        }
        catch (NotFoundCustomException)
        {
            throw new BadRequestCustomException($"El producto {idProduct} no existe.");
        }

        if (quantity > product.Stock)
        {
            throw new BadRequestCustomException($"La cantidad {quantity} excede la cantidad disponible en stock.");
        }

        return product;
    }

    private static SaleDetailDto ToDto(SaleDetail detail) => new()
    {
        Id = detail.Id,
        IdSale = detail.IdSale,
        IdProduct = detail.IdProduct,
        Quantity = detail.Quantity,
        PriceUnit = detail.PriceUnit
    };
}
