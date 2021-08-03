using System;
using System.Collections.Generic;
using System.Linq;
using FakeRomanHsieh.API.Services;
using System.Linq.Dynamic.Core;

namespace FakeRomanHsieh.API.Helper
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, String orderBy, Dictionary<String, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new Exception("source");
            }
            if (mappingDictionary == null)
            {
                throw new Exception("mappingDictionary");
            }

            if (String.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            var orderByString = String.Empty;

            var orderByAfterSplit = orderBy.Split(',');

            foreach(var order in orderByAfterSplit)
            {
                var trimmedOrder = order.Trim();

                // 通過字串 " desc" 來判斷升序或是降序
                var ordeerDescending = trimmedOrder.EndsWith(" desc");

                // 刪除升序或降序字串 " asc" or "desc" 來獲得屬性名稱
                var indexOfFirstSpace = trimmedOrder.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedOrder : trimmedOrder.Remove(indexOfFirstSpace);

                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"key mappubg for {propertyName} is missing");
                }

                var propertyMappingValue = mappingDictionary[propertyName];
                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException("propertyMappingValue");
                }

                foreach(var destinationProperty in propertyMappingValue.DestinationProperties.Reverse())
                {
                    // 給 IQueryable 添加字串
                    orderByString = orderByString + (String.IsNullOrWhiteSpace(orderByString) ? String.Empty : ",")
                        + destinationProperty
                        + (ordeerDescending ? " descending" : " ascending");
                }
            }

            return source.OrderBy(orderByString);
        }
    }
}
