using System;
using RMon.Values.ExportImport.Core;

namespace RMon.ValuesExportImportService.Processing.Parse.Common
{
    static class Factory
    {
        /// <summary>
        /// Создаёт и возвращает значение <see cref="ValueInfo"/>
        /// </summary>
        /// <param name="tagId">Id тега оборудования</param>
        /// <param name="date">Дата</param>
        /// <param name="timeStamp">Таймстамп значения</param>
        /// <param name="value">Значение</param>
        /// <returns></returns>
        public static ValueInfo ValueInfoCreate(long tagId, DateTime date, DateTime timeStamp, double value) =>
            ValueInfoCreate(tagId, timeStamp.TimeOfDay == TimeSpan.Zero ? date.AddDays(1).Add(timeStamp.TimeOfDay) : date.Add(timeStamp.TimeOfDay), value);

        /// <summary>
        /// Создаёт и возвращает значение <see cref="ValueInfo"/>
        /// </summary>
        /// <param name="tagId">Id тега оборудования</param>
        /// <param name="timeStamp">Таймстамп значения</param>
        /// <param name="value">Значение</param>
        /// <returns></returns>
        public static ValueInfo ValueInfoCreate(long tagId, DateTime timeStamp, double value) =>
            new()
            {
                IdTag = tagId,
                TimeStamp = timeStamp,
                Value = new ValueUnion
                {
                    ValueFloat = value,
                    IdQuality = "Normal"
                },
            };

    }
}
