using System;
using System.Threading.Tasks;
using MassTransit;

namespace EsbPublisher.ServiceBus
{
    public class BusService
    {
        private readonly IBusControl _bus;
        public readonly BusPublisher Publisher;

        public BusService()
        {
            _bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                    h.Heartbeat(10);
                });
            });

            Publisher = new BusPublisher(_bus);
        }

        public Task StartAsync() => _bus.StartAsync();

        public Task StopAsync() => _bus.StopAsync();


    }
}
