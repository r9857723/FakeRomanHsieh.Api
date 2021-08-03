using System;
using System.Collections.Generic;

namespace FakeRomanHsieh.API.Services
{
    public class PropertyMapping<TSource, TDestination>: IPropertyMapping
    {
        public Dictionary<String, PropertyMappingValue> _mappingDictionary { get; set; }

        public PropertyMapping(Dictionary<String, PropertyMappingValue> mappingDictionary)
        {
            _mappingDictionary = mappingDictionary;
        }

    }
}
