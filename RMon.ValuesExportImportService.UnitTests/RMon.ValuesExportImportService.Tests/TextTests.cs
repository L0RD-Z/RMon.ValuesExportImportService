using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Tests
{
    [TestClass]
    public class TextTests
    {
        [TestMethod]
        public void Validate()
        {
            var strings = StringInventory.Inventory(Assembly.GetAssembly(typeof(TextTask)));
            foreach (var s in strings)
                try
                {
                    s.Validate();
                }
                catch (Exception e)
                {
                    throw new Exception($"{s.Code}: {e.Message}");
                }
        }
    }
}
