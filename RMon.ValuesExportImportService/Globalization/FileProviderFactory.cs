using System.Collections.Concurrent;
using System.Collections.Generic;
using RMon.Globalization;
using RMon.ValuesExportImportService.Data;

namespace RMon.ValuesExportImportService.Globalization
{
    class FileProviderFactory : IGlobalizationProviderFactory
    {
        private readonly IDictionary<long, FileProvider> _fileProviders =
            new ConcurrentDictionary<long, FileProvider>();

        private readonly ILanguageRepository _languageRepository;

        public FileProviderFactory(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }

        public IGlobalizationProvider GetGlobalizationProvider(long idLanguage)
        {
            if (_fileProviders.ContainsKey(idLanguage))
                return _fileProviders[idLanguage];

            var languageInfo = _languageRepository.LoadLanguage(idLanguage).Result;
            if (languageInfo != null)
            {
                var fileProvider = new FileProvider(languageInfo.FolderName, languageInfo.Culture);
                fileProvider.Load();
                _fileProviders.TryAdd(idLanguage, fileProvider);
            }

            return null;
        }
    }
}
