namespace Buscar.Imoveis.Venda.Messaging
{
    public interface IMessageBusService
    {
        void Publish(object data, string routingKey);
    }
}