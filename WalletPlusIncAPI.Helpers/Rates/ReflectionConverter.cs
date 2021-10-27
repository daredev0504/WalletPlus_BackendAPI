using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WalletPlusIncAPI.Helpers.Rates
{
    /// <summary>
    ///
    /// </summary>
    public static class ReflectionConverter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object source, string propertyName)
        {
            PropertyInfo property = source.GetType().GetProperty(propertyName);
            return property?.GetValue(source, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static string GetPropertyName(PropertyInfo propertyInfo)
        {
            return propertyInfo.Name;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetPropertyValues(object source)
        {
            PropertyInfo[] property = source.GetType().GetProperties();
            return property.ToList();
        }
    }
}