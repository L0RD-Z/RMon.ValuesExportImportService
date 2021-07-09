using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RMon.ValuesExportImportService.Processing.Model;

namespace RMon.ValuesExportImportService.Processing.Common
{
    internal interface ITransformationRatioCalculator
    {
        List<TagRatio> TagsRatio { get; }

        /// <summary>
        /// Асинхронно загружает и БД список тегов <see cref="TagsRatio"/> и вычисляет значения коэффициентов трансформации
        /// </summary>
        /// <param name="idTags">Список id загружаемых тегов</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task LoadTagsRatioFromDbAsync(IList<long> idTags, CancellationToken ct = default);
    }
}