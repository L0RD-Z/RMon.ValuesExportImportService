using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Parse.Format80020.Models
{
    public class Sender
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("inn")]
        public string Inn { get; set; }
    }
}
