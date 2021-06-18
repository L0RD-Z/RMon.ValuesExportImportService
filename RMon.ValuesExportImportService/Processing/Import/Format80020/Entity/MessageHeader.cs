using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Import.Format80020.Entity
{
    [XmlRoot("message")]
    public class MessageHeader
    {
        [XmlAttribute("class")]
        public string Class { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("number")]
        public string Number { get; set; }
    }
}
