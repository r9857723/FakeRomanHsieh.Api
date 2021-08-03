using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace FakeRomanHsieh.API.Helper
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, String fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var expandoObjectList = new List<ExpandoObject>();

            // 避免在列表中遍歷數據 創建一個屬性信息列表
            var propertyInfoList = new List<PropertyInfo>();

            if (String.IsNullOrWhiteSpace(fields))
            {
                // 希望返回動態類型對象ExpandoObject所有的屬性
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);


            } else
            {
                var fieldsAfterSplit = fields.Split(',');
                foreach(var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();

                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"屬性 {propertyName} 找不到 {typeof(TSource)}");
                    }
                    propertyInfoList.Add(propertyInfo);
                }
            }

            foreach(TSource sourceObject in source)
            {
                // 見動態類型對象 創建數據塑形對象
                var dataShapedObject = new ExpandoObject();

                foreach(var propertyInfo in propertyInfoList)
                {
                    // 獲得對應屬性的真實數據
                    var propertyValue = propertyInfo.GetValue(sourceObject);
                    ((IDictionary<String, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }
                expandoObjectList.Add(dataShapedObject);
            }

            return expandoObjectList;
        }
    }
}
