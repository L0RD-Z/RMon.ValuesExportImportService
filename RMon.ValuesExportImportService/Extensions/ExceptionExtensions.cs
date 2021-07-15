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
        public static I18nString ConcatAllExceptionMessage(this Exception ex)
        {
            var str = ex is HandledFormattedException formattedException ? formattedException.FormattedMessage.Clone() : I18nString.FromString(ex.Message);
            return ex.InnerException == null ? str : str.Append(" ").Append(ConcatAllExceptionMessage(ex.InnerException));
        }


        /// <summary>
        /// Метод склеивает сообщения из всех вложенных в <see cref="ex"/> исключений и склеивает с вышестоящим сообщением <see cref="msg"/>
        /// </summary>
        /// <param name="ex">Иерархия исключений</param>
        /// <param name="msg">Сообщение</param>
        /// <returns></returns>
        public static I18nString ConcatExceptionMessage(this Exception ex, I18nString msg) =>
            msg.Clone().Append(" ").Append(ex.ConcatAllExceptionMessage());
    }
}
