using MassTransit;

namespace RMon.ValuesExportImportService.ServiceBus
{
    /// <summary>
    /// Resolves IBus for publisher
    /// </summary>
    interface IBusProvider
    {
        public IBus Bus { get; set; }
    }
}
