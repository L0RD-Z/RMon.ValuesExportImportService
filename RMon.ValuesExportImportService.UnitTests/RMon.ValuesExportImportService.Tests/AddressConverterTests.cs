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
            var colNumber = ExcelCellAddressConverter.ColNumberConvert("XFC");
            Assert.AreEqual(16383, colNumber, "Некорректное преобразование формата представления номера столбца Excel 1");
            colNumber = ExcelCellAddressConverter.ColNumberConvert("A");
            Assert.AreEqual(1, colNumber, "Некорректное преобразование формата представления номера столбца Excel 2");
            colNumber = ExcelCellAddressConverter.ColNumberConvert("Z");
            Assert.AreEqual(26, colNumber, "Некорректное преобразование формата представления номера столбца Excel 3");
            colNumber = ExcelCellAddressConverter.ColNumberConvert("AA");
            Assert.AreEqual(27, colNumber, "Некорректное преобразование формата представления номера столбца Excel 4");
        }

        [TestMethod]
        public void ConvertCellAddressTest()
        {
            var address = ExcelCellAddressConverter.CellAddressConvert("AB18");
            Assert.AreEqual(new ExcelCellAddress(28, 18), address, "Некорректное преобразование формата представления адреса ячейки Excel 1");
            address = ExcelCellAddressConverter.CellAddressConvert("A2");
            Assert.AreEqual(new ExcelCellAddress(1, 2), address, "Некорректное преобразование формата представления адреса ячейки Excel 2");
        }
    }
}
