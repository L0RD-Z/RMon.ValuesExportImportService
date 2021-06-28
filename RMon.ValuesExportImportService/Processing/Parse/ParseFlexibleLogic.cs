using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RMon.Data.Provider.Units.Backend.Interfaces;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Excel;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Common;
using RMon.ValuesExportImportService.Processing.Parse.Common;
using RMon.ValuesExportImportService.Processing.Permission;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Processing.Parse
{
    class ParseFlexibleLogic
    {
        private readonly ILogicDevicesRepository _logicDevicesRepository;
        private readonly IPermissionLogic _permissionLogic;
        private readonly ITagsRepository _tagsRepository;
        private readonly IExcelWorker _excelWorker;
        private const int StartRowNumber = 5;

        public ParseFlexibleLogic(ILogicDevicesRepository logicDevicesRepository, ITagsRepository tagsRepository, IPermissionLogic permissionLogic, IExcelWorker excelWorker)
        {
            _logicDevicesRepository = logicDevicesRepository;
            _tagsRepository = tagsRepository;
            _permissionLogic = permissionLogic;
            _excelWorker = excelWorker;
        }

        /// <summary>
        /// Асинхронно выполняет анализ файлов в формате 80020
        /// </summary>
        /// <param name="files">Список файлов</param>
        /// <param name="context">Контекст выполнения</param>
        /// <param name="ct">Токен отмены опреации</param>
        /// <returns></returns>
        public async Task<List<ValueInfo>> AnalyzeAsync(IList<LocalFile> files, ParseProcessingContext context, CancellationToken ct)
        {
            var tables = new List<(string FileName, List<ReadedSheet> Sheets)>();
            foreach (var file in files)
            {
                await context.LogInfo(TextParse.ReadingFile.With(file.Path, ValuesParseFileFormatType.Flexible.ToString())).ConfigureAwait(false);

                var table = await _excelWorker.ReadFile(file.Body, context).ConfigureAwait(false);
                tables.Add((file.Path, table));
            }

            var result = new List<ValueInfo>();
            
            var idUserGroups = await _permissionLogic.GetUserGroupIdsWithPermissionAsync(EntityTypes.Values, CrudOperations.Read, context.IdUser).ConfigureAwait(false);

            foreach (var table in tables)
                foreach (var sheet in table.Sheets)
                {
                    var rowNumber = StartRowNumber;
                    foreach (var row in sheet.Table.Entities)
                    {
                        rowNumber++;
                        try
                        {
                            if (row.Entity.Entities.TryGetValue(EntityCodes.LogicDevice, out var logicDeviceEntity)) //Поиск оборудовавние
                            {
                                var idLogicDevices = await _logicDevicesRepository.FindLogicDevices(idUserGroups, logicDeviceEntity, ct).ConfigureAwait(false);
                                if (idLogicDevices.Any())
                                {
                                    if (row.Entity.Entities.TryGetValue(EntityCodes.Tag, out var tagEntity)) //Поиск тега
                                    {
                                        var idLogicDevice = idLogicDevices.First().Id;
                                        var idTags = await _tagsRepository.FindTags(idUserGroups, tagEntity, ct).ConfigureAwait(false);
                                        idTags = idTags.Where(t => t.IdLogicDevice == idLogicDevice).ToList();
                                        if (idTags.Any()) //Todo что делать если найдено несколько устройств или тегов?
                                        {
                                            var idTag = idTags.First().Id;
                                            var valueInfo = CreateValue(row, idTag);
                                            result.Add(valueInfo);
                                        }
                                        else
                                            throw new ParseException(TextDb.FindTagForLogicDeviceNoOne.With(idLogicDevice, tagEntity.ToLogString()));
                                    }
                                    else
                                        throw new ParseException(TextParse.MissingSectionError.With(EntityCodes.Tag));
                                }
                                else
                                    throw new ParseException(TextDb.FindLogicDeviceNoOne.With(logicDeviceEntity.ToLogString()));
                            }
                            else //По Tag.Id
                            {
                                if (!row.Entity.Entities.TryGetValue(EntityCodes.Tag, out var tagEntity))
                                    throw new ParseException(TextParse.MissingSectionsError.With(EntityCodes.LogicDevice, EntityCodes.Tag));

                                if (!tagEntity.Properties.TryGetValue(PropertyCodes.Id, out var tagId))
                                    throw new ParseException(TextParse.MissingPropertyError.With($"{EntityCodes.Tag}.{PropertyCodes.Id}"));

                                if (!long.TryParse(tagId.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var idTag))
                                    throw new ParseException(TextParse.FailedConvertToLong.With(PropertyCodes.Value));
                                    
                                result.Add(CreateValue(row, idTag));
                            }
                        }
                        catch (Exception e)
                        {
                            await context.LogWarning(e.ConcatExceptionMessage(TextParse.RowParseError.With(table.FileName, sheet.Name, rowNumber))).ConfigureAwait(false);
                        }
                    }
                }

            return result;
        }

        /// <summary>
        /// Создаёт из строки <see cref="rowEntity"/> значение для тега <see cref="idTag"/>
        /// </summary>
        /// <param name="rowEntity">Строка с данными</param>
        /// <param name="idTag">id тега</param>
        /// <returns></returns>
        private static ValueInfo CreateValue(ImportEntity rowEntity, long idTag)
        {
            if (!rowEntity.Entity.Properties.TryGetValue(PropertyCodes.Timestamp, out var timestampProperty))
                throw new ParseException(TextParse.MissingPropertyError.With(PropertyCodes.Timestamp));
            if (!rowEntity.Entity.Properties.TryGetValue(PropertyCodes.Value, out var valueProperty))
                throw new ParseException(TextParse.MissingPropertyError.With(PropertyCodes.Value));

            //Todo что значит провалидированы?
            if (!DateTime.TryParseExact(timestampProperty.Value, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var timeStamp))
                throw new ParseException(TextParse.FailedConvertToDateTime.With(timestampProperty.Value));
            if (!double.TryParse(valueProperty.Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                throw new ParseException(TextParse.FailedConvertToDouble.With(valueProperty.Value));

            return Factory.ValueInfoCreate(idTag, timeStamp, value);
        }
    }
}
