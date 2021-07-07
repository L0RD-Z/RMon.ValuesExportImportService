﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RMon.Context.EntityStore;
using RMon.Data.Provider.Units.Backend.Common;
using RMon.DriverCore;
using RMon.ValuesExportImportService.Data;
using Task = System.Threading.Tasks.Task;

namespace RMon.ValuesExportImportService.Tests.ParseMatrix24x31
{
    class DataRepositoryStub : IDataRepository
    {
        public Task<DateTime> GetDateAsync() => throw new NotImplementedException();

        public Task<IList<Tag>> GetTagsAsync(IList<long> idLogicDevices, IList<string> tagCodes, CancellationToken cancellationToken) => throw new NotImplementedException();

        public Task<LogicDevice> GetLogicDeviceByPropertyValueAsync(string propertyCode, string propertyValue, CancellationToken cancellationToken)
        {
            var logicDevice = new LogicDevice
            {
                Name = "Тестовое устройство 1",
                LogicDeviceProperties = new List<LogicDeviceProperty>
                {
                    new()
                    {
                        LogicDevicePropertyType = new LogicDevicePropertyType
                        {
                            Code = propertyCode,
                        },
                        Value = propertyValue
                    }
                },
                Tags = new List<Tag>
                {
                    TagCreate(1, "dHHA+", TimeStampTypeEnum.HalfHour),
                    TagCreate(2, "dHHA-", TimeStampTypeEnum.HalfHour),
                    TagCreate(3, "dHHR+", TimeStampTypeEnum.HalfHour),
                    TagCreate(4, "HA+", TimeStampTypeEnum.Hour),
                }
            };
            return Task.FromResult(logicDevice);
        }

        public Task<IList<long>> FindTags(IList<long> idUserGroups, long idLogicDevice, Entity entityFilter, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }


        private Tag TagCreate(long id, string tagCode, TimeStampTypeEnum idTimeStampType) =>
            new()
            {
                Id = id,
                LogicTagLink = new LogicTagLink
                {
                    LogicTagType = new LogicTagType {Code = tagCode},
                },
                DeviceTag = new DeviceTag
                {
                    IdTimeStampType = idTimeStampType.Value
                }
            };
    }
}