using MassTransit;

namespace RMon.ValuesExportImportService.ServiceBus
{
    class BusProvider : IBusProvider
    {
        public IBus Bus { get; set; }
    }
}
