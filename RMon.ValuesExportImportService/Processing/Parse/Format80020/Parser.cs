using System;
using System.IO;
using System.Xml.Serialization;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Processing.Parse.Format80020.Models;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Processing.Parse.Format80020
{
    public static class Parser //Todo сделать не статическим
    {
        private const string Class80020 = "80020";
        private const string SupportedVersion = "2";
        

        public static Message Parse(byte[] data)
        {
            using var stream = new MemoryStream(data);
            object message = null;
            try
            {
                var serializer = new XmlSerializer(typeof(Message));
                message = serializer.Deserialize(stream);
            }
            catch (Exception)
            {
                // ignored
            }

            var validationText = Validate(message);
            if (validationText != null)
                throw new Format800200Exception(validationText);

            return (Message)message;
        }

        private static I18nString Validate(object messageHeader)
        {
            if (messageHeader is not MessageHeader mh)
                return Text80020.IncorrectFileFormat;

            if (mh.Class != Class80020)
                return Text80020.Not80020;

            if (mh.Version != SupportedVersion)
                return Text80020.UnsupportedVersion;

            return null;
        }
    }
}
