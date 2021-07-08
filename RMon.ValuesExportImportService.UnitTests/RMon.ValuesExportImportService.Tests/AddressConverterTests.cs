using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Excel.Matrix;

namespace RMon.ValuesExportImportService.Tests
{
    [TestClass]
    public class AddressConverterTests
    {
        [TestMethod]
        public void ConvertColNumberTest()
        {
            var colNumber = ExcelCellAddressConverter.ExcelColumnToIndex("XFC");
            Assert.AreEqual(16382, colNumber, "Некорректное преобразование формата представления номера столбца Excel 1");
            colNumber = ExcelCellAddressConverter.ExcelColumnToIndex("A");
            Assert.AreEqual(0, colNumber, "Некорректное преобразование формата представления номера столбца Excel 2");
            colNumber = ExcelCellAddressConverter.ExcelColumnToIndex("Z");
            Assert.AreEqual(25, colNumber, "Некорректное преобразование формата представления номера столбца Excel 3");
            colNumber = ExcelCellAddressConverter.ExcelColumnToIndex("AA");
            Assert.AreEqual(26, colNumber, "Некорректное преобразование формата представления номера столбца Excel 4");
        }

        [TestMethod]
        public void ConvertCellAddressTest()
        {
            var address = ExcelCellAddressConverter.CellAddressConvert("AB18");
            Assert.AreEqual(new ExcelCellAddress(27, 17), address, "Некорректное преобразование формата представления адреса ячейки Excel 1");
            address = ExcelCellAddressConverter.CellAddressConvert("A2");
            Assert.AreEqual(new ExcelCellAddress(0, 1), address, "Некорректное преобразование формата представления адреса ячейки Excel 2");
        }
    }
}
