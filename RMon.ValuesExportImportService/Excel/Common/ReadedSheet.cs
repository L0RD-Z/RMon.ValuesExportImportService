using RMon.ValuesExportImportService.Common;

namespace RMon.ValuesExportImportService.Excel.Common
{
    public class ReadedSheet
    {
        /// <summary>
        /// Имя листа Excel
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// считанная с листа <see cref="Name"/> таблица
        /// </summary>
        public ImportTable Table { get; set; }


        public ReadedSheet()
        {
            
        }

        public ReadedSheet(string name, ImportTable table)
        {
            Name = name;
            Table = table;
        }
    }
}
