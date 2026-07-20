using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Exceptions;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;

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

    public Task<List<ProductDto>> GetAllAsync() => _productRepository.GetAllAsync();

    public async Task<ProductDto> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product ?? throw new ApiNotFoundException(_messages.NotFound);
    }

    public Task<List<ProductDto>> SearchAsync(string term) => _productRepository.SearchAsync(term);

    public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
    {
        var created = await _productRepository.CreateAsync(dto);
        return created ?? throw new ApiCommunicationException(_messages.InternalError);
    }

    public async Task<ProductDto> EditAsync(Guid id, ProductEditDto dto)
    {
        var updated = await _productRepository.EditAsync(id, dto);
        return updated ?? throw new ApiNotFoundException(_messages.NotFound);
    }
}
