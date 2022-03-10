using System.Reflection;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace HybridModelBinding.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static string GetNameFromAttributes(this PropertyInfo propertyInfo)
        {
            //default value
            var name = propertyInfo.Name;

            //newtonsoft.json
            var jsonPropertyAttribute = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
            if (jsonPropertyAttribute != null)
            {
                return jsonPropertyAttribute.PropertyName;
            }
            //system.text.json
            var jsonPropertyNameAttribute = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (jsonPropertyNameAttribute != null)
            {
                return jsonPropertyNameAttribute.Name;
            }

            return name;
        }
    }
}