using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RMon.Context.FrontEndContext;
using RMon.Core.Base;

namespace EsbPublisher.Data
{
    class FrontEndContextFactory : ISimpleFactory<FrontEndContext>
    {
        private readonly string _connectionString;
        protected readonly ILoggerFactory LoggerFactory;

        public FrontEndContextFactory(string connectionString)
            : this(connectionString, null)
        {
        }

        public FrontEndContextFactory(string connectionString, ILoggerFactory loggerFactory)
        {
            _connectionString = connectionString;
            LoggerFactory = loggerFactory;
        }


        /// <summary>
        /// Создаёт и возвращает <see cref="FrontEndContext"/>
        /// </summary>
        /// <returns></returns>
        protected virtual FrontEndContext FrontEndContextCreate()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder().UseSqlServer(_connectionString);
            if (LoggerFactory != null)
                dbContextOptionsBuilder = dbContextOptionsBuilder.UseLoggerFactory(LoggerFactory);

            return new FrontEndContext(dbContextOptionsBuilder.Options, _connectionString);
        }

        #region ISimpleFactory<FrontEndContext>

        public FrontEndContext Create() => FrontEndContextCreate();

        #endregion
    }
}
