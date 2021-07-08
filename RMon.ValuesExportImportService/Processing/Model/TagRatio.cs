namespace RMon.ValuesExportImportService.Processing.Model
{
    /// <summary>
    /// Класс хранит данные для коэффициентов трансформации
    /// </summary>
    public class TagRatio
    {
        public long IdTag { get; init; }
        public string TagCode { get; init; }
        public double Ratio { get; init; }
        public double Offset { get; init; }
        /// <summary>
        /// Значение коэффициентов трансформации
        /// </summary>
        public double TransformationRatio { get; init; }
    }
}
