using RMon.ValuesExportImportService.Common;

namespace RMon.ValuesExportImportService.Excel.Common
{
    public class ReadSheet
    {
        /// <summary>
        /// Имя листа Excel
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// считанная с листа <see cref="Name"/> таблица
        /// </summary>
        public ImportTable Table { get; set; }


        public ReadSheet()
        {
            
        }

        public ReadSheet(string name, ImportTable table)
        {
            Name = name;
            Table = table;
        }
    }
}
