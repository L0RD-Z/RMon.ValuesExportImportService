﻿using System;
using System.Collections.Generic;

namespace RMon.ValuesExportImportService.Excel
{
    class MatrixResult
    {
        /// <summary>
        /// Значене свофства оборудования
        /// </summary>
        public string LogicDevicePropertyValue { get; set; }

        /// <summary>
        /// Список значений из матрицы
        /// </summary>
        public List<MatrixValue> Values { get; set; }

        public MatrixResult()
        {
            Values = new List<MatrixValue>();
        }

        public void AddValue(DateTime timeStamp, double value) => Values.Add(new MatrixValue(timeStamp, value));
    }
}
