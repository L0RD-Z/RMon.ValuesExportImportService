using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Parse.Format80020.Entity
{
    public class MeasuringPoint
    {
        [XmlAttribute("code")]
        public string Code { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("measuringchannel")]
        public MeasuringChannel[] MeasuringChannels { get; set; }

    }
}
