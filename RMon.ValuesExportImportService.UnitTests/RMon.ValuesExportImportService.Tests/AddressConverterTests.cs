using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMon.ValuesExportImportService.Excel;

namespace RMon.ValuesExportImportService.Tests
{
    [TestClass]
    public class AddressConverterTests
    {
        [TestMethod]
        public void ConvertColNumberTest()
        {
            var colNumber = AddressConverter.ColNumberConvert("XFC");
            Assert.AreEqual((uint)16383, colNumber, "Некорректное преобразование формата представления номера столбца Excel 1");
            colNumber = AddressConverter.ColNumberConvert("A");
            Assert.AreEqual((uint)1, colNumber, "Некорректное преобразование формата представления номера столбца Excel 2");
            colNumber = AddressConverter.ColNumberConvert("Z");
            Assert.AreEqual((uint)26, colNumber, "Некорректное преобразование формата представления номера столбца Excel 3");
            colNumber = AddressConverter.ColNumberConvert("AA");
            Assert.AreEqual((uint)27, colNumber, "Некорректное преобразование формата представления номера столбца Excel 4");
        }

        [TestMethod]
        public void ConvertCellAddressTest()
        {
            var address = AddressConverter.CellAddressConvert("AB18");
            Assert.AreEqual(new ExcelCellAddress(28, 18), address, "Некорректное преобразование формата представления адреса ячейки Excel 1");
            address = AddressConverter.CellAddressConvert("A2");
            Assert.AreEqual(new ExcelCellAddress(1, 2), address, "Некорректное преобразование формата представления адреса ячейки Excel 2");
        }
    }
}
