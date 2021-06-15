using System.Collections.Generic;
using System.Threading.Tasks;
using RMon.Data.Provider.Common;

namespace RMon.ValuesExportImportService.Data
{
    interface ILanguageRepository
    {
        Task<IList<LanguageInfo>> LoadLanguages();

        Task<LanguageInfo> LoadLanguage(long idLanguage);

        Task<long?> GetUserLanguage(long idUser);
    }
}
