using System.Threading;
using System.Threading.Tasks;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Common;

namespace RMon.ValuesExportImportService.Processing.Export
{
    internal interface IEntityReader
    {
        Task<ExportTable> Read(ValuesExportTaskParameters valuesExportTaskParameters, long idUser, CancellationToken cancellationToken = default);
    }
}