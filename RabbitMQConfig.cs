using RabbitMQ.Client;

namespace Buscar.Imoveis.Venda
{
    public static class RabbitMQConfig
    {
        public static void ConfigureRabbitMqServices(this IServiceCollection services, IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"],
                AutomaticRecoveryEnabled = true,
            };

            #region Services
            try
            {
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                if (connection.IsOpen)
                {
                    services.AddSingleton(sp => connection);
                    services.AddScoped(sp => channel);
                }
            }
            catch { }

            #endregion 
        }
    }
}
