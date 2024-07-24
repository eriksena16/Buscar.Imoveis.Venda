//using AssetTrack.Core;
//using ConsumidorRabbit;
//using ConsumidorRabbit.Workers;
//using MassTransit;

//namespace Sgi.Notificacao.Api.Settings
//{
//    public static class BrokerConfig
//    {
//        public static IServiceCollection ConfigureBroker(this IServiceCollection services, IConfiguration configuration)
//        {
//            var appSettings = configuration.Get<AppSettings>();

//            services.AddMassTransit(configure =>
//            {
//                if (appSettings is null)
//                {
//                    throw new ArgumentNullException(nameof(appSettings));
//                }

//                var brokerConfig = appSettings.BrokerConfiguration;

//                configure.SetKebabCaseEndpointNameFormatter();

//                configure.AddConsumer<QueueSendEmailTesteConsumer>();

//                configure.UsingRabbitMq((context, cfg) =>
//                {
//                    cfg.UseRawJsonDeserializer();

//                    cfg.Host(new Uri(brokerConfig.Host));
//                    cfg.UseMessageRetry(retry => { retry.Interval(3, TimeSpan.FromSeconds(5)); });
//                    cfg.Message<SendEmailEvent>(x => x.SetEntityName("order-events-listener")); //exchange name
//                    cfg.ReceiveEndpoint("queue-send-email-teste", ep => //queue name
//                    {
//                        ep.ConfigureConsumer<QueueSendEmailTesteConsumer>(context);
//                    });
//                    cfg.ConfigureEndpoints(context);
                    
//                });

                
//            });

//            return services;
//        }
//    }
//}
