using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Parse.Format80020.Models
{
    public class Period
    {
        [XmlAttribute("start")]
        public string Start { get; set; }

        [XmlAttribute("end")]
        public string End { get; set; }

        [XmlElement("value")]
        public Value Value { get; set; }

    }
}
