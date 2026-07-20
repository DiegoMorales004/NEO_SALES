using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Exceptions;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos;
using NEO_SALES.CORE.Models.Entities;

namespace NEO_SALES.CORE.Services;

public class SaleStatusService : ISaleStatusService
{
    private readonly ISaleStatusRepository _saleStatusRepository;
    private readonly MessageDefaultsConfiguration _messages;

    public SaleStatusService(ISaleStatusRepository saleStatusRepository, IOptions<MessageDefaultsConfiguration> messages)
    {
        _saleStatusRepository = saleStatusRepository;
        _messages = messages.Value;
    }

    public async Task<List<SaleStatusDto>> GetAllAsync()
    {
        var statuses = await _saleStatusRepository.GetAllAsync();
        return statuses.Select(ToDto).ToList();
    }

    public async Task<SaleStatusDto> GetByIdAsync(int id)
    {
        var status = await _saleStatusRepository.GetByIdAsync(id)
            ?? throw new NotFoundCustomException(_messages.NotFound, nameof(SaleStatus), id);

        return ToDto(status);
    }

    public async Task<SaleStatusDto> CreateAsync(SaleStatusDto dto)
    {
        var status = new SaleStatus
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description
        };

        var created = await _saleStatusRepository.InsertAsync(status);
        return ToDto(created);
    }

    public async Task<SaleStatusDto> EditAsync(int id, SaleStatusEditDto dto)
    {
        await GetByIdAsync(id);

        var status = new SaleStatus
        {
            Id = id,
            Name = dto.Name,
            Description = dto.Description
        };

        var updated = await _saleStatusRepository.UpdateAsync(status)
            ?? throw new NotFoundCustomException(_messages.NotFound, nameof(SaleStatus), id);

        return ToDto(updated);
    }

    public async Task RemoveAsync(int id)
    {
        await GetByIdAsync(id);
        await _saleStatusRepository.DeleteAsync(id);
    }

    private static SaleStatusDto ToDto(SaleStatus status) => new()
    {
        Id = status.Id,
        Name = status.Name,
        Description = status.Description
    };
}
