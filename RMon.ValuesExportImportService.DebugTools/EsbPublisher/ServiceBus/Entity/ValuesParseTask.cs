using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMon.ESB.Core.ValuesParseTaskDto;
using RMon.Values.ExportImport.Core;

namespace EsbPublisher.ServiceBus.Entity
{
    class ValuesParseTask : BaseTask, IValuesParseTask
    {
        public ValuesParseTaskParameters Parameters { get; set; }
        public string Name { get; set; }
        public long? IdUser { get; set; }
    }
}
