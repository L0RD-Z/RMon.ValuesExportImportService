using System;
using System.Threading.Tasks;
using MassTransit;
using RMon.ESB.Core.ValuesExportTaskDto;
using RMon.ESB.Core.ValuesImportTaskDto;
using RMon.ESB.Core.ValuesParseTaskDto;

namespace EsbConsumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                    h.Heartbeat(10);
                });

                cfg.ReceiveEndpoint(endpoint =>
                {
                    endpoint.Consumer<ConsumerT<IValuesExportTask>>();
                    endpoint.Consumer<ConsumerT<IValuesExportTaskStarted>>();
                    endpoint.Consumer<ConsumerT<IValuesExportTaskProgressChanged>>();
                    endpoint.Consumer<ConsumerT<IValuesExportTaskLog>>();
                    endpoint.Consumer<ConsumerT<IValuesExportTaskFinished>>();
                    endpoint.Consumer<ConsumerT<IValuesExportTaskAbort>>();

                    endpoint.Consumer<ConsumerT<IValuesParseTask>>();
                    endpoint.Consumer<ConsumerT<IValuesParseTaskStarted>>();
                    endpoint.Consumer<ConsumerT<IValuesParseTaskProgressChanged>>();
                    endpoint.Consumer<ConsumerT<IValuesParseTaskLog>>();
                    endpoint.Consumer<ConsumerT<IValuesParseTaskFinished>>();
                    endpoint.Consumer<ConsumerT<IValuesParseTaskAbort>>();

                    endpoint.Consumer<ConsumerT<IValuesImportTask>>();
                    endpoint.Consumer<ConsumerT<IValuesImportTaskStarted>>();
                    endpoint.Consumer<ConsumerT<IValuesImportTaskProgressChanged>>();
                    endpoint.Consumer<ConsumerT<IValuesImportTaskLog>>();
                    endpoint.Consumer<ConsumerT<IValuesImportTaskFinished>>();
                    endpoint.Consumer<ConsumerT<IValuesImportTaskAbort>>();
                });
            });

            await bus.StartAsync().ConfigureAwait(false);


            Console.WriteLine("Для ввыхода из программы нажмите Enter...");

            Console.ReadKey();
        }
    }
}
