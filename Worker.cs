using Buscar.Imoveis.Venda.Services.Interface;

namespace Buscar.Imoveis.Venda;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var imovelService = scope.ServiceProvider.GetRequiredService<IImovelCaixaService>();

                // Use o serviço dentro do escopo
                await imovelService.ProcessoImoveisCaixa();
                //_logger.LogInformation($"Received data: {result}");
            }


            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
