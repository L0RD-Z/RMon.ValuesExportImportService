using System;
using System.Threading.Tasks;
using MassTransit;

namespace EsbConsumer
{
    class ConsumerT<TMessage> : IConsumer<TMessage> where TMessage : class
    {
        public Task Consume(ConsumeContext<TMessage> context)
        {
            var localPath = context.DestinationAddress.LocalPath;
            var msgName = localPath.Remove(0, localPath.IndexOf(':') + 1);
            return Console.Out.WriteLineAsync($"{DateTime.Now} \"{msgName}\" получено");
        }
    }
}
