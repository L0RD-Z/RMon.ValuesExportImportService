using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Import.Format80020.Entity
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
