using AssetTrack.Core;
using Buscar.Imoveis.Venda;
using Buscar.Imoveis.Venda.Services;
using Buscar.Imoveis.Venda.Services.Interface;

var builder = Host.CreateApplicationBuilder(args);

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.Configure<AppSettings>(builder.Configuration);
        //services.ConfigureBroker(builder.Configuration);
        services.AddScoped<IImovelCaixaService, ImovelCaixaService>();
    })
    .Build();

host.Run();
