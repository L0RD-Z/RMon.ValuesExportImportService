using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RMon.Configuration.Options.TagValueTransformation;
using RMon.ValuesExportImportService.Data;
using RMon.ValuesExportImportService.Processing.Model;

namespace RMon.ValuesExportImportService.Processing.Common
{
    class TransformationRatioCalculator : ITransformationRatioCalculator
    {
        public const double DefaultRatioValue = 1;

        private readonly IOptionsMonitor<TagValueTransformation> _tagValueTransformationOptions;
        private readonly IDataRepository _dataRepository;

        public List<TagRatio> TagsRatio { get; private set; }

        public TransformationRatioCalculator(IOptionsMonitor<TagValueTransformation> tagValueTransformationOptions, IDataRepository dataRepository)
        {
            _tagValueTransformationOptions = tagValueTransformationOptions;
            _dataRepository = dataRepository;
        }

        
        /// <inheritdoc />
        public async Task LoadTagsRatioFromDbAsync(IList<long> idTags, CancellationToken ct = default)
        {
            var tags = await _dataRepository.GetTagsAsync(idTags, ct).ConfigureAwait(false);
            var devicePropertyCodes = _tagValueTransformationOptions.CurrentValue.TagsGroups.SelectMany(t => t.MultiplyPropertyCodes).ToList();

            var idDevices = tags.Where(t => t.IdDevice.HasValue).Select(t => t.IdDevice.Value).ToList();
            var deviceProperties = await _dataRepository.GetDevicePropertiesAsync(idDevices, devicePropertyCodes, ct).ConfigureAwait(false);

            TagsRatio = new List<TagRatio>();
            foreach (var tag in tags)
            {
                // ReSharper disable once PossibleInvalidOperationException
                var propertyValues = deviceProperties.Where(t => t.IdDevice == tag.IdDevice.Value).ToDictionary(t => t.DevicePropertyType.KeyReport, t => t.Value);
                var item = new TagRatio
                {
                    IdTag = tag.Id,
                    Offset = tag.DeviceTag.Offset,
                    Ratio = tag.DeviceTag.Ratio,
                    TransformationRatio = GetTransformationRatio(tag.DeviceTag.Code, tag.LogicTagLink.LogicDeviceType.Code, propertyValues)
                };

                TagsRatio.Add(item);
            }
        }

        private double GetTransformationRatio(string deviceCode, string logicDeviceCode, IDictionary<string, string> propertyValues)
        {
            var devicePropertyCodes = _tagValueTransformationOptions.CurrentValue.GetMultiplyPropertyCodes(TagCodeType.Device, deviceCode);
            var logicDevicePropertyCodes = _tagValueTransformationOptions.CurrentValue.GetMultiplyPropertyCodes(TagCodeType.LogicDevice, logicDeviceCode);
            return CalculateTransformationRatio(propertyValues, devicePropertyCodes) * CalculateTransformationRatio(propertyValues, logicDevicePropertyCodes);
        }

        private static double CalculateTransformationRatio(IDictionary<string, string> propertyValues, IList<string> propertyCodes)
        {
            var kt = DefaultRatioValue;

            foreach (var propertyCode in propertyCodes)
                if (propertyValues.ContainsKey(propertyCode))
                    kt *= GetDouble(propertyValues[propertyCode]);

            return kt;
        }

        private static double GetDouble(string value)
        {
            if (value == null || !double.TryParse(value.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
                result = DefaultRatioValue;

            return result;
        }
    }
}
