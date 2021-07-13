namespace RMon.ValuesExportImportService.Configuration
{
    record ValuesParseOptions
    {
        /// <summary>
        /// Суммарный размер файлов для парсинга, байт
        /// </summary>
        public int TotalParseFilesSize { get; init; }
    }
}
