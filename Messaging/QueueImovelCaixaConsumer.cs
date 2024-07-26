using MassTransit;

namespace Buscar.Imoveis.Venda.Messaging
{
    public class QueueImovelCaixaConsumer : IConsumer<ImportImovel>
    {
        private readonly ILogger<QueueImovelCaixaConsumer> _logger;

        public QueueImovelCaixaConsumer(ILogger<QueueImovelCaixaConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ImportImovel> context)
        {
            var imovel = context.Message;
            _logger.LogInformation($"Imovel successfully sent: {context.Message.Cidade}");

            return Task.CompletedTask;
        }

    }
}
