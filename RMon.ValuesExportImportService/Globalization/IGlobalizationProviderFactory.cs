using RMon.Globalization;

namespace RMon.ValuesExportImportService.Globalization
{
    interface IGlobalizationProviderFactory
    {
        IGlobalizationProvider GetGlobalizationProvider(long idLanguage);
    }
}
