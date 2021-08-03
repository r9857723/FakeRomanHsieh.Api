using System;
using System.Collections.Generic;

namespace FakeRomanHsieh.API.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool IsMappingExists<TSource, TDestination>(string fields);
        bool IsPropertyExists<T>(String fields);
    }
}