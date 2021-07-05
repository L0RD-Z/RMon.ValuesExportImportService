using System.Collections.Generic;

namespace RMon.ValuesExportImportService.Excel.Common
{
    class ExcelResult
    {
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName { get; set; }
        public IList<ExcelLogicDeviceValues> LogicDeviceValues { get; set; }


        public ExcelResult()
        {
            
        }

        public ExcelResult(string fileName, IList<ExcelLogicDeviceValues> logicDeviceValues)
        {
            FileName = fileName;
            LogicDeviceValues = logicDeviceValues;
        }
    }
}
