using System;
using System.Collections.Generic;

namespace RMon.ValuesExportImportService.Excel.Common
{
    class ExcelLogicDeviceValues
    {
        /// <summary>
        /// Название листа
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// Значене свойства оборудования
        /// </summary>
        public string LogicDevicePropertyValue { get; set; }

        /// <summary>
        /// Список значений из матрицы
        /// </summary>
        public List<ExcelValue> Values { get; set; }

        public ExcelLogicDeviceValues()
        {
            Values = new List<ExcelValue>();
        }

        public void AddValue(DateTime timeStamp, double value) => Values.Add(new ExcelValue(timeStamp, value));
    }
}
