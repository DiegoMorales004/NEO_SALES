using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Exceptions;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.CORE.Models.Entities;

namespace NEO_SALES.CORE.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly MessageDefaultsConfiguration _messages;

    public ProductService(IProductRepository productRepository, IOptions<MessageDefaultsConfiguration> messages)
    {
        _productRepository = productRepository;
        _messages = messages.Value;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(ToDto).ToList();
    }

    public async Task<ProductDto> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new NotFoundCustomException(_messages.NotFound, nameof(Product), id);

        return ToDto(product);
    }

    public async Task<List<ProductDto>> SearchAsync(string term)
    {
        var products = await _productRepository.SearchAsync(term);
        return products.Select(ToDto).ToList();
    }

    public async Task<ProductDto> CreateAsync(ProductCreateDto dto, string actorUser)
    {
        if (dto.Price <= 0)
        {
            throw new BadRequestCustomException("El precio del producto debe ser mayor a cero");
        }

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock,
            Status = true,
            UserCreate = actorUser
        };

        var created = await _productRepository.InsertAsync(product);
        return ToDto(created);
    }

    public async Task<ProductDto> EditAsync(Guid id, ProductEditDto dto, string actorUser)
    {
        await GetByIdAsync(id);

        if (dto.Price <= 0)
        {
            throw new BadRequestCustomException("El precio del producto debe ser mayor a cero");
        }

        var product = new Product
        {
            Id = id,
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock,
            Status = dto.Status,
            UserUpdate = actorUser
        };

        var updated = await _productRepository.UpdateAsync(product)
            ?? throw new NotFoundCustomException(_messages.NotFound, nameof(Product), id);

        return ToDto(updated);
    }

    public async Task RemoveAsync(Guid id)
    {
        await GetByIdAsync(id);
        await _productRepository.DeleteAsync(id);
    }

    private static ProductDto ToDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Stock = product.Stock,
        Status = product.Status,
        DatetimeCreate = product.DatetimeCreate,
        DatetimeUpdate = product.DatetimeUpdate
    };
}
