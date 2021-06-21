using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Import.Format80020.Entity
{
    [XmlRoot("message")]
    public class Message : MessageHeader
    {
        [XmlElement("datetime")]
        public DateTime DateTime { get; set; }

        [XmlElement("sender")]
        public Sender Sender { get; set; }

        [XmlElement("area")]
        public Area[] Areas { get; set; }
    }
}
