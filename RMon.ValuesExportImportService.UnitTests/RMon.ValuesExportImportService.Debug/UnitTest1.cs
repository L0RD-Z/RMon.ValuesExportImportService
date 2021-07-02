using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMon.Data.Provider;
using RMon.ValuesExportImportService.Excel;
using RMon.ValuesExportImportService.Excel.Matrix;

namespace RMon.ValuesExportImportService.Debug
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var timeRange = new TimeRange(new DateTime(2021, 01, 01), new DateTime(2021, 03, 04));
            
            var list = SplitTimeRange(timeRange, TimeSpan.FromDays(30));
        }


        private List<TimeRange> SplitTimeRange(TimeRange timeRange, TimeSpan timeInterval)
        {
            var result = new List<TimeRange>();
            if ((timeRange.DateEnd.Value - timeRange.DateStart.Value) > timeInterval)
            {
                var dateTimeIterator = timeRange.DateStart.Value;
                while (dateTimeIterator <= timeRange.DateEnd.Value)
                {
                    var dateEnd = dateTimeIterator + timeInterval;
                    if (dateEnd > timeRange.DateEnd.Value)
                        dateEnd = timeRange.DateEnd.Value;

                    result.Add(new TimeRange(dateTimeIterator, dateEnd));
                    dateTimeIterator = dateEnd.AddSeconds(1);
                } 
            }
            else
                result.Add(timeRange);

            return result;
        }

        [TestMethod]
        public void TestMethod2()
        {
            var num = ExcelCellAddressConverter.ColNumberConvert("XFC"); //16383

            var a = ExcelCellAddressConverter.CellAddressConvert("G14");
        }

        [TestMethod]
        public void TestMethod3()
        {
            var date = new DateTime(2021, 06, 15);
            var date1 = date.AddHours(24);
        }
    }
}
