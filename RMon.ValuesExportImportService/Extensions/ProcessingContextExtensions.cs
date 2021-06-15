using System.Threading.Tasks;
using RMon.ESB.Core.Common;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Processing;

namespace RMon.ValuesExportImportService.Extensions
{
    static class ProcessingContextExtensions
    {
        public static Task LogInfo(this IProcessingContext processingContext, I18nString msg) => processingContext.Log(msg, LogLevel.Info, null);
        public static Task LogInfo(this IProcessingContext processingContext, I18nString msg, float progress) => processingContext.Log(msg, LogLevel.Info, progress);
        public static Task LogWarning(this IProcessingContext processingContext, I18nString msg) => processingContext.Log(msg, LogLevel.Warning, null);
        public static Task LogWarning(this IProcessingContext processingContext, I18nString msg, float progress) => processingContext.Log(msg, LogLevel.Warning, progress);
        public static Task LogError(this IProcessingContext processingContext, I18nString msg) => processingContext.Log(msg, LogLevel.Error, null);
    }
}
