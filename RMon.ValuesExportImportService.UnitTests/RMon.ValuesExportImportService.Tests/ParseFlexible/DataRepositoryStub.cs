using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RMon.Context.EntityStore;
using RMon.Data.Provider.Units.Backend.Common;
using RMon.ValuesExportImportService.Data;

namespace RMon.ValuesExportImportService.Tests.ParseFlexible
{
    class DataRepositoryStub : IDataRepository
    {
        private readonly List<long> _idLogicDevices;

        public DataRepositoryStub(List<long> idLogicDevices)
        {
            _idLogicDevices = idLogicDevices;
        }

        public Task<DateTime> GetDateAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<Tag>> GetTagsAsync(IList<long> idLogicDevices, IList<string> tagCodes, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<LogicDevice> GetLogicDeviceByPropertyValueAsync(string propertyCode, string propertyValue, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<long>> FindTags(IList<long> idUserGroups, long idLogicDevice, Entity entityFilter, CancellationToken ct = default)
        {
            var result = new List<long> { long.Parse(entityFilter.Properties["Id"].Value) };

            return System.Threading.Tasks.Task.FromResult((IList<long>)result);
        }
    }
}
