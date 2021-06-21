using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RMon.Data.Provider.Configuration;

namespace EsbPublisher.Data
{
    class DataRepository
    {

        private readonly SqlLogicTagTypeRepository _logicTagTypeRepository;
        private readonly SqlDevicePropertyTypeRepository _devicePropertyTypeRepository;


        public DataRepository()
        {
            var connectionString = "data source=mule.allmonitoring.local;initial catalog=RMonLiteRo5;User Id=sa;Password=P@ssw0rd";
            var frontEndFactory = new FrontEndContextFactory(connectionString);

            _devicePropertyTypeRepository = new SqlDevicePropertyTypeRepository(frontEndFactory, null);
            _logicTagTypeRepository = new SqlLogicTagTypeRepository(frontEndFactory, null);
        }


        public Task<IEnumerable<DevicePropertyType>> GetDevicePropertyTypesAsync(CancellationToken cancellationToken)
        {
            return _devicePropertyTypeRepository.GetDevicePropertyTypes(cancellationToken);
        }

        public Task<IEnumerable<LogicTagType>> GetTagTypesAsync(CancellationToken cancellationToken)
        {
            return _logicTagTypeRepository.GetLogicTagTypes(cancellationToken);
        }
    }
}
