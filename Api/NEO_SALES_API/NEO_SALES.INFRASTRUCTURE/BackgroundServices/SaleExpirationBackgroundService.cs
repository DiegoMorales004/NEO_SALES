using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Interfaces.Repositories;
using NEO_SALES.CORE.Models.Configuration;

namespace NEO_SALES.INFRASTRUCTURE.BackgroundServices;

public class SaleExpirationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SaleExpirationBackgroundService> _logger;
    private readonly BackgroundJobsConfiguration _config;

    public SaleExpirationBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<SaleExpirationBackgroundService> logger,
        IOptions<BackgroundJobsConfiguration> config)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _config = config.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_config.CheckIntervalMinutes));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ExpirePendingSalesAsync();
        }
    }

    private async Task ExpirePendingSalesAsync()
    {
        try
        {
            _logger.LogInformation("Iniciando proceso de revision para carritos de compras vencidos. Timepo maximo permitido: " + _config.MaxTimeShoppingCartMinutes);

            using var scope = _scopeFactory.CreateScope();
            var saleRepository = scope.ServiceProvider.GetRequiredService<ISaleRepository>();

            var expiredIds = await saleRepository.ExpireSalesAsync(_config.MaxTimeShoppingCartMinutes);

            if (expiredIds.Count > 0)
            {
                _logger.LogInformation(
                    "Se marcaron {Count} ventas como EXPIRADA: {Ids}",
                    expiredIds.Count,
                    string.Join(", ", expiredIds));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fallo la revision de carritos vencidos");
        }

        _logger.LogInformation("Finalizo proceso de revision para carritos de compras vencidos. Timepo maximo permitido: " + _config.MaxTimeShoppingCartMinutes);

    }
}
