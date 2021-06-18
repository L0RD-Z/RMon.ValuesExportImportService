using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options.FileStorage;
using RMon.FileStorage.Grpc;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Files
{
    public class FileStorage : IFileStorage, IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly IOptionsMonitor<ValuesExportImportFileStorage> _optionsMonitor;

        private ValuesExportImportFileStorage _options;
        private ValuesExportImportFileStorage _newOptions;

        private Channel _channel;
        private FileStorageService.FileStorageServiceClient _fileStorageService;



        public FileStorage(IOptionsMonitor<ValuesExportImportFileStorage> optionsMonitor)
        {
            _semaphore = new SemaphoreSlim(1, 1);
            _optionsMonitor = optionsMonitor;
            _optionsMonitor.OnChange(options => _newOptions = options);
            _options = _newOptions = optionsMonitor.CurrentValue;
        }

        #region IFileStorage

        /// <inheritdoc />
        public async Task StoreFileAsync(string filePath, byte[] content, CancellationToken cancellationToken = default)
        {
            try
            {
                var fileStorageService = await GetFileStorageServiceAsync().ConfigureAwait(false);

                var response = await fileStorageService.StoreFileAsync(new StoreFileRequest
                {
                    AreaName = _options.Area,
                    FilePath = filePath,
                    Content = ByteString.CopyFrom(content)
                }, Metadata.Empty, null, cancellationToken);
                try
                {
                    response.Result.ThrowIfError();
                }
                catch (RMon.FileStorage.Grpc.FileStorageException e)
                {
                    throw new FileStorageException(TextFileStorage.ErrorCodeException.With(response.Result.Code.ToString()), e);
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                    throw new OperationCanceledException();
                throw new FileStorageException(TextFileStorage.SendFileWithStatusException.With(filePath, _options.ToString(), e.Status.ToString()), e);
            }
            catch (Exception e)
            {
                throw new FileStorageException(TextFileStorage.SendFileException.With(filePath, _options.ToString()), e);
            }
        }

        /// <inheritdoc />
        public async Task<byte[]> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                var fileStorageService = await GetFileStorageServiceAsync().ConfigureAwait(false);
                var response = await fileStorageService.GetFileAsync(new GetFileRequest
                {
                    AreaName = _options.Area,
                    FilePath = filePath
                }, Metadata.Empty, null, cancellationToken);

                try
                {
                    response.Result.ThrowIfError();
                    return response.Content.ToByteArray();
                }
                catch (Exception e)
                {
                    throw new FileStorageException(TextFileStorage.ErrorCodeException.With(response.Result.Code.ToString()), e);
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                    throw new OperationCanceledException();
                throw new FileStorageException(TextFileStorage.ReceiveFileWithStatusException.With(filePath, _options.ToString(), e.Status.ToString()), e);
            }
            catch (Exception e)
            {
                throw new FileStorageException(TextFileStorage.ReceiveFileException.With(filePath, _options.ToString()), e);
            }
        }

        /// <inheritdoc />
        public async Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                var fileStorageService = await GetFileStorageServiceAsync().ConfigureAwait(false);
                var response = await fileStorageService.DeleteFileAsync(new DeleteFileRequest
                {
                    AreaName = _options.Area,
                    FilePath = filePath
                }, Metadata.Empty, null, cancellationToken);
                try
                {
                    response.Result.ThrowIfError();
                }
                catch (Exception e)
                {
                    throw new FileStorageException(TextFileStorage.ErrorCodeException.With(response.Result.Code.ToString()), e);
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                    throw new OperationCanceledException();
                throw new FileStorageException(TextFileStorage.DeleteFileWithStatusException.With(filePath, _options.ToString(), e.Status.ToString()), e);
            }
            catch (Exception e)
            {
                throw new FileStorageException(TextFileStorage.DeleteFileException.With(filePath, _options.ToString()), e);
            }
        }

        #endregion


        /// <summary>
        /// Создаёт подключение к <see cref="FileStorageService"/>
        /// </summary>
        /// <returns></returns>
        private async Task<FileStorageService.FileStorageServiceClient> GetFileStorageServiceAsync()
        {
            var fileStorageService = _fileStorageService;

            if (!_options.Equals(_newOptions) || fileStorageService == null)
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (!_options.Equals(_newOptions) || fileStorageService == null)
                    {
                        if (_options.Grpc.Equals(_newOptions.Grpc) || fileStorageService == null)
                            await CreateFileStorageServiceAsync().ConfigureAwait(false);

                        _options = _newOptions;
                        return _fileStorageService;
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return fileStorageService;
        }

        private async Task CreateFileStorageServiceAsync()
        {
            try
            {
                if (_channel != null)
                    await _channel.ShutdownAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                // ignored
            }

            _channel = new Channel($"{_newOptions.Grpc.Host}:{_newOptions.Grpc.Port}", ChannelCredentials.Insecure);
            _fileStorageService = new FileStorageService.FileStorageServiceClient(_channel);
        }



        #region IDisposable

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncWait")]
        public void Dispose()
        {
            try
            {
                _channel?.ShutdownAsync().Wait();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion
    }
}
