using System.Collections.Generic;
using System.Linq;
using RMon.Data.Provider.Units.Backend.Common;
using RMon.Globalization;

namespace RMon.ValuesExportImportService.Extensions
{
    static class EntityExtensions
    {
        private const string Splitter = ":";

        public static string ToLogString(this Entity entity)
        {
            var propertyValueDictionary = entity.GetPropertyValueDictionary();

            return string.Join("; ", propertyValueDictionary.Select(t => $"{t.Key} = '{t.Value}'"));
        }

        public static IDictionary<string, string> GetPropertyDescriptionDictionary(this EntityDescription entityDescription, IGlobalizationProvider globalizationProvider, string codePrefix = "", string namePrefix = "")
        {
            var dictionary = new Dictionary<string, string>();

            GetEntitiesDescriptionDictionary(entityDescription.EntityDescriptions, codePrefix, namePrefix, dictionary, globalizationProvider);
            GetPropertyDescriptionDictionary(entityDescription.PropertyDescriptions, codePrefix, namePrefix, dictionary, globalizationProvider);

            return dictionary;
        }

        private static void GetPropertyDescriptionDictionary(
            IList<PropertyDescription> propertyDescriptions, string codePrefix, string namePrefix, IDictionary<string, string> dictionary, IGlobalizationProvider globalizationProvider)
        {
            codePrefix = codePrefix + (string.IsNullOrEmpty(codePrefix) ? "" : Splitter);
            namePrefix = namePrefix + (string.IsNullOrEmpty(namePrefix) ? "" : Splitter);

            foreach (var propertyDescription in propertyDescriptions)
            {
                dictionary.Add($"{codePrefix}{propertyDescription.Code}", $"{namePrefix}{propertyDescription.Name.ToString(globalizationProvider)}");
            }
        }

        private static void GetEntitiesDescriptionDictionary(
            IList<EntityDescription> entityDescriptions, string codePrefix, string namePrefix, IDictionary<string, string> dictionary, IGlobalizationProvider globalizationProvider)
        {
            codePrefix = codePrefix + (string.IsNullOrEmpty(codePrefix) ? "" : Splitter);
            namePrefix = namePrefix + (string.IsNullOrEmpty(namePrefix) ? "" : Splitter);

            foreach (var entityDescription in entityDescriptions)
            {
                GetPropertyDescriptionDictionary(entityDescription.PropertyDescriptions,
                    $"{codePrefix}{entityDescription.Code}", $"{namePrefix}{entityDescription.Name.ToString(globalizationProvider)}", dictionary, globalizationProvider);

                GetEntitiesDescriptionDictionary(entityDescription.EntityDescriptions,
                    $"{codePrefix}{entityDescription.Code}", $"{namePrefix}{entityDescription.Name.ToString(globalizationProvider)}", dictionary, globalizationProvider);
            }
        }

        public static IDictionary<string, string> GetPropertyValueDictionary(this Entity entity, string codePrefix = "")
        {
            var dictionary = new Dictionary<string, string>();

            GetPropertyValueDictionary(entity.Properties.Values.ToList(), codePrefix, dictionary);

            GetEntitiesValueDictionary(entity.Entities.Values.ToList(), codePrefix, dictionary);

            return dictionary;
        }

        private static void GetPropertyValueDictionary(
            IList<PropertyValue> propertyValues, string codePrefix, IDictionary<string, string> dictionary)
        {
            codePrefix = codePrefix + (string.IsNullOrEmpty(codePrefix) ? "" : Splitter);

            foreach (var propertyValue in propertyValues)
            {
                dictionary.Add($"{codePrefix}{propertyValue.Code}", propertyValue.Value);
            }
        }
        public static bool IsEmpty(this EntityDescription entityDescription)
        {
            if (entityDescription == null)
                return true;

            return !entityDescription.EntityDescriptions.Any() && !entityDescription.PropertyDescriptions.Any();
        }

        private static void GetEntitiesValueDictionary(
            IList<Entity> entities, string codePrefix, IDictionary<string, string> dictionary)
        {
            codePrefix = codePrefix + (string.IsNullOrEmpty(codePrefix) ? "" : Splitter);

            foreach (var entity in entities)
            {
                GetPropertyValueDictionary(entity.Properties.Values.ToList(), $"{codePrefix}{entity.Code}", dictionary);

                GetEntitiesValueDictionary(entity.Entities.Values.ToList(),
                    $"{codePrefix}{entity.Code}", dictionary);
            }
        }

        public static EntityDescription ToEntityDescription(this IList<string> propertyCodes, string entityCode)
        {
            var entityDescription = new EntityDescription(entityCode);

            foreach (var propertyCode in propertyCodes)
                ParsePropertyDescription(entityDescription, propertyCode);

            return entityDescription;
        }

        public static PropertyDescription ParsePropertyDescription(this EntityDescription entityDescription, string code)
        {
            PropertyDescription propertyDescription = null;

            var currentEntityDescription = entityDescription;

            var parts = code.Split(Splitter);
            for (int i = 0; i < parts.Length; i++)
            {
                var partCode = parts[i];
                if (i == parts.Length - 1)
                {
                    propertyDescription = new PropertyDescription(partCode);
                    currentEntityDescription.PropertyDescriptions.Add(propertyDescription);
                }
                else
                {
                    var subEntityDescription = currentEntityDescription.EntityDescriptions.FirstOrDefault(t => t.Code == partCode);
                    if (subEntityDescription == null)
                    {
                        subEntityDescription = new EntityDescription(partCode);
                        currentEntityDescription.EntityDescriptions.Add(subEntityDescription);
                    }
                    currentEntityDescription = subEntityDescription;
                }
            }

            return propertyDescription;
        }

        public static Entity CreateEntity(this EntityDescription entityDescription, IDictionary<PropertyDescription, int> propertyDescriptionMap, IDictionary<int, PropertyValue> propertyValueMap)
        {
            return new Entity(entityDescription.Code, null,
                GetPropertyValues(entityDescription.PropertyDescriptions, propertyDescriptionMap, propertyValueMap),
                GetEntities(entityDescription.EntityDescriptions, propertyDescriptionMap, propertyValueMap));
        }

        private static IList<PropertyValue> GetPropertyValues(IList<PropertyDescription> propertyDescriptions, IDictionary<PropertyDescription, int> propertyDescriptionMap, IDictionary<int, PropertyValue> propertyValueMap)
        {
            var propertyValues = new List<PropertyValue>();

            foreach (var propertyDescription in propertyDescriptions)
            {
                var propertyValue = new PropertyValue(propertyDescription.Code, "");
                propertyValues.Add(propertyValue);
                propertyValueMap.Add(propertyDescriptionMap[propertyDescription], propertyValue);
            }

            return propertyValues;
        }

        private static IList<Entity> GetEntities(IList<EntityDescription> entityDescriptions, IDictionary<PropertyDescription, int> propertyDescriptionMap, IDictionary<int, PropertyValue> propertyValueMap)
        {
            var entities = new List<Entity>();

            foreach (var entityDescription in entityDescriptions)
            {
                entities.Add(CreateEntity(entityDescription, propertyDescriptionMap, propertyValueMap));
            }

            return entities;
        }
    }
}
