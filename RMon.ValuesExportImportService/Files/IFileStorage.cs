using System.Threading;
using System.Threading.Tasks;

namespace RMon.ValuesExportImportService.Files
{
    public interface IFileStorage
    {
        /// <summary>
        /// Отправляет файл <see cref="content"/> в файловое хранилище
        /// </summary>
        /// <param name="filePath">Ссылка, по которой будет храниться файл в файловом хранилище</param>
        /// <param name="content">Тело файла</param>
        /// <param name="cancellationToken">Токен отмены данных</param>
        /// <returns>Ссылка на файл</returns>
        Task StoreFileAsync(string filePath, byte[] content, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получает файл <see cref="filePath"/> из файлового хранилища
        /// </summary>
        /// <param name="filePath">Ссылка на файл в файловом хранилище</param>
        /// <param name="cancellationToken">Токен отмены данных</param>
        /// <returns>Тело файла</returns>
        Task<byte[]> GetFileAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаляет файл <see cref="filePath"/> из файлового хранилища
        /// </summary>
        /// <param name="filePath">Ссылка на файл в файловом хранилище</param>
        /// <param name="cancellationToken">Токен отмены данных</param>
        /// <returns></returns>
        Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
    }
}
