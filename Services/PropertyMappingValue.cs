using System;
using System.Collections.Generic;

namespace FakeRomanHsieh.API.Services
{
    public class PropertyMappingValue
    {
        public IEnumerable<String> DestinationProperties { get; private set; }

        public PropertyMappingValue(IEnumerable<String> destinationProperties)
        {
            DestinationProperties = destinationProperties;
        }
    }
}
