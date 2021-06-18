using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Import.Format80020.Entity
{
    public class Value
    {
        [XmlAttribute("status")]
        public string Code { get; set; }

        [XmlText]
        public string Val { get; set; }
    }
}
