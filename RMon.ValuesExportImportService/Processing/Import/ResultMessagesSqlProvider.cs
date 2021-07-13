using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RMon.Context.EntityStore;
using RMon.Core.MainServerInterface;
using RMon.ValuesExportImportService.Configuration;
using RMon.ValuesExportImportService.Data;

namespace RMon.ValuesExportImportService.Processing.Import
{
    /// <inheritdoc />
    class ResultMessagesSqlProvider : IResultMessagesSender
    {
        private readonly IOptionsMonitor<ResultMessageSenderOptions> _resultMessageSenderOptions;
        private readonly IDataRepository _dataRepository;


        public ResultMessagesSqlProvider(IOptionsMonitor<ResultMessageSenderOptions> resultMessageSenderOptions, IDataRepository dataRepository)
        {
            _resultMessageSenderOptions = resultMessageSenderOptions;
            _dataRepository = dataRepository;
        }

        /// <inheritdoc />
        public async Task<long> SendPacketAsync(DAServerDataMessage packet, CancellationToken ct = default)
        {
            var buffer = new SSDAnalizeBuf
            {
                idSSDList = 0,
                rcvdTS = await _dataRepository.GetDateAsync().ConfigureAwait(false),
                data = _resultMessageSenderOptions.CurrentValue.PacketFormat switch
                {
                    ResultMessagePacketFormats.Json => packet.JsonSerialize(),
                    ResultMessagePacketFormats.Deflate => packet.DeflateSerialize(),
                    _ => packet.Serialize()
                }
            };

            return await _dataRepository.AddSsdAnalizeBufAsync(buffer, ct).ConfigureAwait(false);
        }
    }
}
