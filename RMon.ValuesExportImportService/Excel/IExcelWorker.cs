using System.Collections.Generic;
using System.Threading;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Processing;

namespace RMon.ValuesExportImportService.Excel
{
    interface IExcelWorker
    {
        ///// <summary>
        ///// Создаёт книгу Excel
        ///// </summary>
        ///// <param name="exportContainer">Экспортируемый набор сущностей</param>
        ///// <param name="entityCodes">Код экспортируемой сущности</param>
        ///// <param name="cancellationToken">Токен отмены операции</param>
        ///// <returns></returns>
        //byte[] CreateBook(IProcessingContext processingContext, ExportContainer exportContainer, IList<EntityTypes> entityCodes, CancellationToken cancellationToken = default);

        ///// <summary>
        ///// Выполняет чтение Excel-выгрузки
        ///// </summary>
        ///// <param name="fileBody">Тело файла</param>
        ///// <param name="cancellationToken">Токен отмены операции</param>
        ///// <returns></returns>
        //ImportContainer ReadBook(byte[] fileBody, CancellationToken cancellationToken = default);

        byte[] WriteWorksheet(IProcessingContext processingContext, ExportTable exportTable);
    }
}