using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Parse.Format80020.Models
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
