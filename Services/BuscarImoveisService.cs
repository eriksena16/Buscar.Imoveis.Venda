using Buscar.Imoveis.Venda.Services.Interface;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buscar.Imoveis.Venda.Services
{
    public class BuscarImoveisService :  HostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        public BuscarImoveisService( IServiceProvider serviceProvider)
        {

            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //TimeSpan horarioDesejado = new(1, 0, 0);
            //TimeSpan intervaloInicial = CalcularIntervaloInicial(horarioDesejado);


            // Espere até o próximo horário desejado
            await Task.Delay(5000, _cts.Token);

            _timer = new(async (_) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetService<IImovelCaixaService>();

                await repository.ProcessoImoveisCaixa();

            }, null, TimeSpan.Zero, TimeSpan.FromDays(1));

            // Mantém o programa em execução
            await Task.Delay(TimeSpan.FromDays(1), _cts.Token);
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
