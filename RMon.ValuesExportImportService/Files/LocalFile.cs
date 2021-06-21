namespace RMon.ValuesExportImportService.Files
{
    class LocalFile
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Тело файла
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>
        /// Конструктор 0
        /// </summary>
        public LocalFile()
        {

        }

        /// <summary>
        /// Конструктор 1
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="body">Тело файла</param>
        public LocalFile(string path, byte[] body)
        {
            Path = path;
            Body = body;
        }
    }
}
