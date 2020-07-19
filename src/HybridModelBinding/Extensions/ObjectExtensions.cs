using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HybridModelBinding.Extensions
{
    public static class ObjectExtensions
    {
        internal static IEnumerable<PropertyInfo> GetPropertiesNotPartOfType<T>(
            this object value,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            var tProperties = (typeof(T).IsAssignableFrom(value.GetType())
                ? typeof(T)
                    .GetProperties(bindingFlags)
                    .Select(x => x.Name)
                : Enumerable.Empty<string>())
                .ToList();

            var modelProperties = value
                .GetType()
                .GetProperties(bindingFlags)
                .Where(x => !tProperties.Contains(x.Name));

            return modelProperties;
        }
    }
}
