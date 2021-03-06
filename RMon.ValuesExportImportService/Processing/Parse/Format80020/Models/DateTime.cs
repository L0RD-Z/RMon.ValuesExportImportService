using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Parse.Format80020.Models
{
    public class DateTime
    {
        [XmlElement("day")]
        public string Day { get; set; }

        [XmlElement("timestamp")]
        public string TimeStamp { get; set; }

        [XmlElement("daylightsavingtime")]
        public string DayLightSavingTime { get; set; }
    }
}
