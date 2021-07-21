namespace RMon.ValuesExportImportService.Files
{
    class LocalFile
    {
        /// <summary>
        /// Имя файла
        /// </summary>
        public string Name { get; set; }
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
        /// <param name="name">Путь к файлу</param>
        /// <param name="body">Тело файла</param>
        public LocalFile(string name, byte[] body)
        {
            Name = name;
            Body = body;
        }
    }
}
