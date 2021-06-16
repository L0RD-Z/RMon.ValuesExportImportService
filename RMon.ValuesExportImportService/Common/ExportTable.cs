using System.Collections.Generic;
using RMon.Data.Provider.Units.Backend.Common;

namespace RMon.ValuesExportImportService.Common
{
    public class ExportTable
    {
        public IList<string> PropertyCodes { get; set; }

        public EntityTable EntityTable { get; set; }
    }
}
