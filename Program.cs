using Buscar.Imoveis.Venda;
using Buscar.Imoveis.Venda.Messaging;
using Buscar.Imoveis.Venda.Services;
using Buscar.Imoveis.Venda.Services.Interface;

var builder = Host.CreateApplicationBuilder(args);

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<AppSettings>(builder.Configuration);
        services.ConfigureBroker(builder.Configuration);
        //services.AddHostedService<Worker>();       
        services.AddScoped<IImovelCaixaService, ImovelCaixaService>();
        services.AddScoped<IMessageBusService, RabbitMqService>();
        services.AddHostedService<BuscarImoveisService>();
        //services.ConfigureRabbitMqServices(builder.Configuration);
    })
    .Build();

host.Run();
