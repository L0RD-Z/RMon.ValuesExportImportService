using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RMon.Data.Provider.Units.Backend.Common;
using RMon.Data.Provider.Units.Backend.Common.FindResults;
using RMon.Data.Provider.Units.Backend.Interfaces;

namespace RMon.ValuesExportImportService.Tests.ParseFlexible
{
    class TagsRepositoryStub : ITagsRepository
    {
        private readonly List<long> _idLogicDevices;

        public TagsRepositoryStub(List<long> idLogicDevices)
        {
            _idLogicDevices = idLogicDevices;
        }

        public Task<EntityTable> GetTagsTable(IList<long> idUserGroups, IList<long> idLogicDevices, EntityDescription entityDescription, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task<IList<TagFindResult>> FindTags(IList<long> idUserGroups, Entity entityFilter, CancellationToken ct = new())
        {
            var result = _idLogicDevices.Select(t => new TagFindResult
            {
                Id = long.Parse(entityFilter.Properties["Id"].Value),
                IdLogicDevice = t
            }).ToList();

            return Task.FromResult((IList<TagFindResult>)result);
        }

        public Task<IList<long>> FindLogicDevices(IList<long> idUserGroups, Entity entityFilter, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task ValidateAddTag(IList<long> idUserGroups, long idLogicDevice, Entity entity, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task<long> AddTag(IList<long> idUserGroups, long idLogicDevice, Entity entity, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task ValidateUpdateTag(IList<long> idUserGroups, long idTag, Entity entity, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task UpdateTag(IList<long> idUserGroups, long idTag, Entity entity, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }

        public Task DeleteTag(IList<long> idUserGroups, long idTag, CancellationToken ct = new())
        {
            throw new NotImplementedException();
        }
    }
}
