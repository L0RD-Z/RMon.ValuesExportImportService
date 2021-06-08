using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RMon.ESB.Core.Common;
using RMon.ValuesExportImportService.ServiceBus;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class ParseTaskLogic : IParseTaskLogic
    {
        public Task StartTaskAsync(ITask receivedTask, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void AbortTask(ITask receivedTask, StateMachineInstance instance)
        {
            throw new NotImplementedException();
        }
    }
}
