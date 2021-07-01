using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RMon.Data.Provider.Units.Backend.Common;
using RMon.Data.Provider.Units.Backend.Common.FindResults;
using RMon.Data.Provider.Units.Backend.Common.ValidateAddResult;
using RMon.Data.Provider.Units.Backend.Interfaces;

namespace RMon.ValuesExportImportService.Tests.ParseFlexible
{
    class LogicDeviceRepositoryStub : ILogicDevicesRepository
    {
        private readonly List<long> _idLogicDevices;

        public LogicDeviceRepositoryStub(List<long> idLogicDevices)
        {
            _idLogicDevices = idLogicDevices;
        }

        public Task<EntityTable> GetLogicDevicesTable(IList<long> idUserGroups, IList<long> idLogicDevices, EntityDescription entityDescription, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task<IList<LogicDeviceFindResult>> FindLogicDevices(IList<long> idUserGroups, Entity entityFilter, CancellationToken ct = new())
        {
            IList<LogicDeviceFindResult> result = null;
            var logicDeviceId = long.Parse(entityFilter.Properties["Id"].Value);
            if (_idLogicDevices.Contains(logicDeviceId))
                result = new List<LogicDeviceFindResult>
                {
                    new()
                    {
                        Id = logicDeviceId
                    }
                };

            return Task.FromResult(result);
        }

        public Task<LogicDeviceValidateAddResult> ValidateAddLogicDevice(IList<long> idUserGroups, Entity entity, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task<long> AddLogicDevice(IList<long> idUserGroups, Entity entity, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task ValidateUpdateLogicDevice(IList<long> idUserGroups, long idLogicDevice, Entity entity, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task UpdateLogicDevice(IList<long> idUserGroups, long idLogicDevice, Entity entity, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task DeleteLogicDevice(IList<long> idUserGroups, long idLogicDevice, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }
    }
}
