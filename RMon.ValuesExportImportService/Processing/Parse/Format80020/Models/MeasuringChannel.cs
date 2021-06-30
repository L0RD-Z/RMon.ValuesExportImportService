using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Parse.Format80020.Models
{
    public class MeasuringChannel
    {
        [XmlAttribute("code")]
        public string Code { get; set; }

        [XmlAttribute("desc")]
        public string Desc { get; set; }

        [XmlElement("period")]
        public Period[] Periods { get; set; }
    }
}
