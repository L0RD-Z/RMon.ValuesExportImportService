using System.Collections.Generic;
using RMon.Data.Provider.Units.Backend.Common;

namespace RMon.ValuesExportImportService.Common
{
    public class ImportTable
    {
        public EntityDescription SelectorDescription { get; set; }

        public EntityDescription EntityDescription { get; set; }

        public IList<ImportEntity> Entities { get; set; }
    }
}
