using System.Threading.Tasks;
using RMon.Data.Provider.Esb.Entities.ValuesExportImport;
using RMon.ESB.Core.Common;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Processing.Common;

namespace RMon.ValuesExportImportService.Processing.Import
{
    class ImportProcessingContext : ProcessingContext<DbValuesExportImportTask>
    {
        public ImportProcessingContext(ITask task, DbValuesExportImportTask dbTask, BaseTaskLogger<DbValuesExportImportTask> taskLogger, long idUser)
            : base(task, dbTask, taskLogger, idUser)
        {
        }

        public Task LogFinished(I18nString msg) => ((ImportTaskLogger)TaskLogger).LogFinishedAsync(Task, DbTask, msg);

    }
}
