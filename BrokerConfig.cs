using Buscar.Imoveis.Venda.Messaging;
using MassTransit;

namespace Buscar.Imoveis.Venda
{
    public static class BrokerConfig
    {
        public static IServiceCollection ConfigureBroker(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.Get<AppSettings>();

            services.AddMassTransit(configure =>
            {
                if (appSettings is null)
                {
                    throw new ArgumentNullException(nameof(appSettings));
                }

                var brokerConfig = appSettings.BrokerConfiguration;

                configure.SetKebabCaseEndpointNameFormatter();
                //configure.AddConsumer<QueueImovelCaixaConsumer>();

                configure.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseRawJsonDeserializer();

                    cfg.Host(new Uri(brokerConfig.Host));
                    cfg.UseMessageRetry(retry => { retry.Interval(3, TimeSpan.FromSeconds(5)); });
                    
                    cfg.Message<ImportImovel>(x => x.SetEntityName("imovel-events-listener")); // exchange name
                    cfg.ConfigureEndpoints(context);
                });


            });

            return services;
        }
    }
}
