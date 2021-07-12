using System;
using RMon.Globalization;
using RMon.Globalization.String;

namespace RMon.ValuesExportImportService.Extensions
{
    static class ExceptionExtensions
    {
        /// <summary>
        /// Метод склеивает сообщения из всех вложенных в <see cref="ex"/> исключений
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static I18nString ConcatExceptionMessage(this Exception ex)
        {
            var result = I18nString.FromString(string.Empty);
            if (ex != null)
            {
                result = ex switch
                {
                    HandledFormattedException handledFormattedException => handledFormattedException.FormattedMessage,
                    { } exception => I18nString.FromString(exception.Message),
                    _ => result
                };
                if (ex.InnerException != null)
                {
                    var str = ConcatExceptionMessage(ex.InnerException);
                    if (str != null)
                        result = result.Clone().Append(I18nString.FromString(" ")).Append(str);
                }
            }

            return result;
        }


        /// <summary>
        /// Метод склеивает сообщения из всех вложенных в <see cref="ex"/> исключений и склеивает с вышестоящим сообщением <see cref="msg"/>
        /// </summary>
        /// <param name="ex">Иерархия исключений</param>
        /// <param name="msg">Сообщение</param>
        /// <returns></returns>
        public static I18nString ConcatExceptionMessage(this Exception ex, I18nString msg) =>
            msg.Clone().Append(" ").Append(ex.ConcatExceptionMessage());
    }
}
