using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Import.Format80020.Entity
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
