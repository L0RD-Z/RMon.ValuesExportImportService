using System.Collections.Generic;
using System.Threading.Tasks;
using RMon.Core.Files;
using RMon.Data.Provider.Esb.Entities.ValuesExportImport;
using RMon.ESB.Core.Common;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Processing.Common;

namespace RMon.ValuesExportImportService.Processing.Export
{
    class ExportProcessingContext : ProcessingContext<DbValuesExportImportTask>
    {

        public ExportProcessingContext(ITask task, DbValuesExportImportTask dbTask, IBaseTaskLogger<DbValuesExportImportTask> taskLogger, long idUser)
            : base(task, dbTask, taskLogger, idUser)
        {
        }

        public Task LogFinished(I18nString msg, IList<FileInStorage> resultFiles) => ((IExportTaskLogger)TaskLogger).LogFinishedAsync(Task, DbTask, msg, resultFiles);

    }
}
