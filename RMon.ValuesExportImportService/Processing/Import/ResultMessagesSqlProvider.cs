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
        private readonly IOptionsMonitor<ResultMessageSender> _resultMessageSenderOptions;
        private readonly IDataRepository _dataRepository;


        public ResultMessagesSqlProvider(IOptionsMonitor<ResultMessageSender> resultMessageSenderOptions, IDataRepository dataRepository)
        {
            _resultMessageSenderOptions = resultMessageSenderOptions;
            _dataRepository = dataRepository;
        }

        

        /// <inheritdoc />
        public async Task<long> SendPacketAsync(DAServerDataMessage packet, long idSsdList = 0, string idAnalysisService = "", CancellationToken ct = default)
        {
            var buffer = new SSDAnalizeBuf
            {
                idSSDList = idSsdList,
                rcvdTS = await _dataRepository.GetDateAsync().ConfigureAwait(false),
                IdAnalysisService = string.IsNullOrEmpty(idAnalysisService) ? null : idAnalysisService,
                data = _resultMessageSenderOptions.CurrentValue.PacketFormat switch
                {
                    ResultMessagePacketFormats.Json => packet.JsonSerialize(),
                    ResultMessagePacketFormats.Deflate => packet.DeflateSerialize(),
                    _ => packet.Serialize()
                }
            };

            return await _dataRepository.AddSsdAnalizeBufAsync(buffer, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public bool IsServiceAvailable() => true;
    }
}
