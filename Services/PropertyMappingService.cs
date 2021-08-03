using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeRomanHsieh.API.Dtos;
using FakeRomanHsieh.API.Models;

namespace FakeRomanHsieh.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<String, PropertyMappingValue> _touristRoutePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase) {
                { "id", new PropertyMappingValue(new List<String>(){ "Id"})},
                { "Title", new PropertyMappingValue(new List<String>(){ "Title"})},
                { "Rating", new PropertyMappingValue(new List<String>(){ "Rating"})},
                { "OriginalPrice", new PropertyMappingValue(new List<String>(){ "OriginalPrice"})},
            };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<TouristRouteDto, TouristRoute>(_touristRoutePropertyMapping));
        }

        public Dictionary<String, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            //獲得匹配的應測對象
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance fo <{typeof(TSource)},{typeof(TDestination)}>");
        }

        public bool IsMappingExists<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();
            if (String.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');

            foreach(var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }


        public bool IsPropertyExists<T>(String fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }
            var fieldsAfterSplit = fields.Split(',');
            foreach(var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
