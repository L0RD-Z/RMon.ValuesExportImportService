using System.Collections.Generic;
using RMon.ESB.Core.Common;
using RMon.ESB.Core.ValuesImportTaskDto;
using RMon.Values.ExportImport.Core;

namespace RMon.ValuesExportImportService.ServiceBus.Entity
{
    class ValuesImportTask : BaseTask, IValuesImportTask
    {
        public ValuesImportTaskParameters Parameters { get; set; }
        public string Name { get; set; }
        public long? IdUser { get; set; }


        public ValuesImportTask(ITask task, IList<ValueInfo> values, string name, long? idUser)
            :base(task)
        {
            Parameters = new ValuesImportTaskParameters(values);
            Name = name;
            IdUser = idUser;
        }

        public ValuesImportTask(ITask task, string name, long? idUser)
            : this(task, new List<ValueInfo>(), name, idUser)
        {
            
        }
        
    }
}
