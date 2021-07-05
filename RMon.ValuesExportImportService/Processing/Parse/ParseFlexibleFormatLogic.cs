using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RMon.Data.Provider.Units.Backend.Common;
using RMon.Data.Provider.Units.Backend.Interfaces;
using RMon.Values.ExportImport.Core;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Data;
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
    class ParseFlexibleFormatLogic
    {
        private readonly ILogicDevicesRepository _logicDevicesRepository;
        private readonly IPermissionLogic _permissionLogic;
        private readonly IDataRepository _dataRepository;
        private readonly IExcelWorker _excelWorker;
        private const int StartRowNumber = 5;

        public ParseFlexibleFormatLogic(ILogicDevicesRepository logicDevicesRepository, IDataRepository dataRepository, IPermissionLogic permissionLogic, IExcelWorker excelWorker)
        {
            _logicDevicesRepository = logicDevicesRepository;
            _dataRepository = dataRepository;
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
                if (!table.Any())
                    throw new TaskException(TextParse.ReadFileError.With(file.Path));
                tables.Add((file.Path, table));
            }

            var result = new List<ValueInfo>();
            var logicDevicesCache = new Dictionary<long, long>();
            var tagsCache = new Dictionary<long, long>();
            var idUserGroups = await _permissionLogic.GetUserGroupIdsWithPermissionAsync(EntityTypes.Values, CrudOperations.Read, context.IdUser).ConfigureAwait(false);
            
            foreach (var table in tables)
                foreach (var sheet in table.Sheets)
                {
                    await context.LogInfo(TextParse.AnalyzeInfoFromExcelFile.With(table.FileName, sheet.Name)).ConfigureAwait(false);
                    var rowNumber = StartRowNumber;
                    foreach (var row in sheet.Table.Entities)
                    {
                        rowNumber++;
                        try
                        {
                            if (row.Entity.Entities.TryGetValue(EntityCodes.LogicDevice, out var logicDeviceEntity)) //Поиск оборудования
                            {
                                var logicDeviceHash = GetHash(logicDeviceEntity);
                                if (!logicDevicesCache.TryGetValue(logicDeviceHash, out var logicDeviceId))
                                {
                                    logicDeviceId = await FindLogicDeviceId(idUserGroups, logicDeviceEntity, ct).ConfigureAwait(false);
                                    logicDevicesCache[logicDeviceHash] = logicDeviceId;
                                }

                                if (row.Entity.Entities.TryGetValue(EntityCodes.Tag, out var tagEntity)) //Поиск тега
                                {
                                    var tagHash = HashCode.Combine(GetHash(tagEntity), logicDeviceHash);
                                    if (!tagsCache.TryGetValue(tagHash, out var tagId))
                                    {
                                        tagId = await FindTagId(idUserGroups, logicDeviceId, tagEntity, ct).ConfigureAwait(false);
                                        tagsCache[tagHash] = tagId;
                                    }
                                    result.Add(CreateValue(row, tagId));
                                }
                                else
                                    throw new ParseException(TextParse.MissingSectionError.With(EntityCodes.Tag));
                            }
                            else //По Tag.Id
                            {
                                if (!row.Entity.Entities.TryGetValue(EntityCodes.Tag, out var tagEntity))
                                    throw new ParseException(TextParse.MissingSectionsError.With(EntityCodes.LogicDevice, EntityCodes.Tag));

                                if (!tagEntity.Properties.TryGetValue(TagPropertyCodes.Id, out var tagId))
                                    throw new ParseException(TextParse.MissingPropertyError.With($"{EntityCodes.Tag}.{TagPropertyCodes.Id}"));

                                if (!long.TryParse(tagId.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var idTag))
                                    throw new ParseException(TextParse.FailedConvertToLong.With(ValuesPropertyCodes.Value));
                                    
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
        /// Вычисляет hash для <see cref="entity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public long GetHash(Entity entity)
        {
            long hash = 0;
            foreach (var property in entity.Properties)
                hash = HashCode.Combine(property.Value?.Code, property.Value?.Value);

            hash += HashCode.Combine(entity.Code, entity.Name);

            hash += entity.Entities.Sum(childEntity => GetHash(childEntity.Value));
            return hash;
        }


        /// <summary>
        /// Выполняет поиск оборудования в БД в соответствии с критериями поиска <see cref="entityFilter"/> и возвращает его Id
        /// </summary>
        /// <param name="idUserGroups">Права доступа</param>
        /// <param name="entityFilter">Набор свойство для поиска оборудования</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        private async Task<long> FindLogicDeviceId(IList<long> idUserGroups, Entity entityFilter, CancellationToken cancellationToken = default)
        {
            var logicDevices = await _logicDevicesRepository.FindLogicDevices(idUserGroups, entityFilter, cancellationToken).ConfigureAwait(false);
            return logicDevices.Count switch
            {
                0 => throw new TaskException(TextDb.FindNoOneLogicDeviceError.With(entityFilter.ToLogString())),
                1 => logicDevices.Single().Id,
                _ => throw new TaskException(TextDb.FindManyLogicDevicesError.With(entityFilter.ToLogString()))
            };
        }

        /// <summary>
        /// Выполняет поиск тега для оборудования <see cref="logicDeviceId"/> в БД в соответствии с критериями поиска <see cref="entityFilter"/> и возвращает его Id
        /// </summary>
        /// <param name="idUserGroups"></param>
        /// <param name="logicDeviceId"></param>
        /// <param name="entityFilter"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<long> FindTagId(IList<long> idUserGroups, long logicDeviceId, Entity entityFilter, CancellationToken cancellationToken = default)
        {
            var tags = await _dataRepository.FindTags(idUserGroups, logicDeviceId, entityFilter, cancellationToken).ConfigureAwait(false);
            if (tags.Any())
                return tags.SingleOrDefault();
            else
                throw new ParseException(TextDb.FindNoOneTagForLogicDevice.With(entityFilter.ToLogString(), logicDeviceId));
        }

        /// <summary>
        /// Создаёт из строки <see cref="rowEntity"/> значение для тега <see cref="idTag"/>
        /// </summary>
        /// <param name="rowEntity">Строка с данными</param>
        /// <param name="idTag">id тега</param>
        /// <returns></returns>
        private static ValueInfo CreateValue(ImportEntity rowEntity, long idTag)
        {
            if (!rowEntity.Entity.Properties.TryGetValue(ValuesPropertyCodes.Timestamp, out var timestampProperty))
                throw new ParseException(TextParse.MissingPropertyError.With(ValuesPropertyCodes.Timestamp));
            if (!rowEntity.Entity.Properties.TryGetValue(ValuesPropertyCodes.Value, out var valueProperty))
                throw new ParseException(TextParse.MissingPropertyError.With(ValuesPropertyCodes.Value));

            if (!DateTime.TryParseExact(timestampProperty.Value, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var timeStamp))
                throw new ParseException(TextParse.FailedConvertToDateTime.With(timestampProperty.Value));
            if (!double.TryParse(valueProperty.Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                throw new ParseException(TextParse.FailedConvertToDouble.With(valueProperty.Value));

            return new(idTag, timeStamp, value);
        }
    }
}
