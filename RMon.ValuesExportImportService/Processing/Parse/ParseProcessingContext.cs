using System.Collections.Generic;
using System.Threading.Tasks;
using RMon.Data.Provider.Esb.Entities.ValuesExportImport;
using RMon.ESB.Core.Common;
using RMon.Globalization.String;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Processing.Common;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    public class ParseProcessingContext : ProcessingContext<DbValuesExportImportTask>
    {
        public ParseProcessingContext(ITask task, DbValuesExportImportTask dbTask, BaseTaskLogger<DbValuesExportImportTask> taskLogger, long idUser) 
            : base(task, dbTask, taskLogger, idUser)
        {
        }

        public Task LogFinished(I18nString msg, IList<ValueInfo> values) => ((ParseTaskLogger)TaskLogger).LogFinishedAsync(Task, DbTask, msg, values);
    }
}
