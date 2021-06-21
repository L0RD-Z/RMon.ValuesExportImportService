﻿using System.Xml.Serialization;

namespace RMon.ValuesExportImportService.Processing.Import.Format80020.Entity
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
        public DeliveryPoint[] DeliveryPoints { get; set; }
    }
}
