using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RMon.Context.FrontEndContext;
using RMon.Core.Base;
using RMon.Data.Provider.Common;

namespace RMon.ValuesExportImportService.Data
{
    class LanguageRepository : ILanguageRepository
    {
        private readonly ISimpleFactory<FrontEndContext> _factory;

        public LanguageRepository(ISimpleFactory<FrontEndContext> factory)
        {
            _factory = factory;
        }

        public async Task<IList<LanguageInfo>> LoadLanguages()
        {
            await using var context = _factory.Create();

            var languages = await context.Languages
                .Where(l => l.IsActive)
                .ToListAsync().ConfigureAwait(false);

            return languages.Select(l => new LanguageInfo
            {
                Id = l.Id,
                Name = l.Name,
                FolderName = l.FolderName,
                Culture = l.Culture,
                IsDefault = l.IsDefault,
                IsActive = l.IsActive
            }).ToList();
        }

        public async Task<LanguageInfo> LoadLanguage(long idLanguage)
        {
            await using var context = _factory.Create();

            var language = await context.Languages
                .FirstOrDefaultAsync(l => l.IsActive).ConfigureAwait(false);

            return new LanguageInfo
            {
                Id = language.Id,
                Name = language.Name,
                FolderName = language.FolderName,
                Culture = language.Culture,
                IsDefault = language.IsDefault,
                IsActive = language.IsActive
            };
        }

        public async Task<long?> GetUserLanguage(long idUser)
        {
            await using var context = _factory.Create();
            
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == idUser).ConfigureAwait(false);

            if (user?.IdLanguage == null)
            {
                var language = await context.Languages.FirstOrDefaultAsync(l => l.IsDefault && l.IsActive).ConfigureAwait(false);
                if (language != null)
                    return language.Id;
            }

            return user?.IdLanguage;
        }
    }
}
