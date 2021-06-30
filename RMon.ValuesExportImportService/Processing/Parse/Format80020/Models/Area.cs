using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Parse.Format80020.Models
{
    public class Area
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("inn")]
        public string Inn { get; set; }

        [XmlElement("measuringpoint")]
        public MeasuringPoint[] MeasuringPoints { get; set; }

        [XmlElement("deliverypoint")]
        public MeasuringPoint[] DeliveryPoints { get; set; }
    }
}
