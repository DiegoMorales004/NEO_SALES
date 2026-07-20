using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Exceptions;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES.CORE.Services;

public class SaleDetailService : ISaleDetailService
{
    private readonly ISaleDetailRepository _saleDetailRepository;
    private readonly MessageDefaultsConfiguration _messages;

    public SaleDetailService(ISaleDetailRepository saleDetailRepository, IOptions<MessageDefaultsConfiguration> messages)
    {
        _saleDetailRepository = saleDetailRepository;
        _messages = messages.Value;
    }

    public Task<List<SaleDetailDto>> GetBySaleIdAsync(Guid saleId) => _saleDetailRepository.GetBySaleIdAsync(saleId);

    public async Task<SaleDetailDto> CreateAsync(SaleDetailCreateDto dto)
    {
        var created = await _saleDetailRepository.CreateAsync(dto);
        return created ?? throw new ApiCommunicationException(_messages.InternalError);
    }

    public async Task<SaleDetailDto> EditAsync(Guid id, SaleDetailEditDto dto)
    {
        var updated = await _saleDetailRepository.EditAsync(id, dto);
        return updated ?? throw new ApiNotFoundException(_messages.NotFound);
    }

    public Task DeleteAsync(Guid id) => _saleDetailRepository.DeleteAsync(id);
}
