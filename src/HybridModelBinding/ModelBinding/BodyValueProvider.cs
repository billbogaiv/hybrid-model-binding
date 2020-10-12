using HybridModelBinding.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HybridModelBinding.ModelBinding
{
    internal sealed class BodyValueProvider : IValueProvider
    {
        public BodyValueProvider(
            object model,
            HttpRequest request)
        {
            var bodyContent = string.Empty;

            if (request.Body.CanSeek)
            {
                using (var reader = new StreamReader(request.Body))
                {
                    request.Body.Seek(0, SeekOrigin.Begin);

                    bodyContent = reader.ReadToEnd();

                    request.Body.Seek(0, SeekOrigin.Begin);
                }
            }

            var requestKeys = Array.Empty<string>();

            if (!string.IsNullOrEmpty(bodyContent))
            {
                requestKeys = JObject
                    .Parse(bodyContent)
                    .Properties()
                    .Select(x => x.Name)
                    .ToArray();
            }

            foreach (var property in model.GetPropertiesNotPartOfType<IHybridBoundModel>()
                .Where(x => !requestKeys.Any() || requestKeys.Contains(x.Name, StringComparer.OrdinalIgnoreCase)))
            {
                values.Add(property.Name, property.GetValue(model, null));
            }
        }

        private PrefixContainer prefixContainer;
        private IDictionary<string, object> values = new Dictionary<string, object>();

        public PrefixContainer PrefixContainer
        {
            get
            {
                if (prefixContainer == null)
                {
                    prefixContainer = new PrefixContainer(values.Keys);
                }

                return prefixContainer;
            }
        }

        public bool ContainsPrefix(string prefix)
        {
            return PrefixContainer.ContainsPrefix(prefix);
        }

        public object GetObject(string key)
        {
            return values.ContainsKey(key)
                ? values[key]
                : null;
        }

        public ValueProviderResult GetValue(string key) => throw new NotImplementedException($"Use `{nameof(GetObject)}`.");
    }
}
