using HybridModelBinding.Extensions;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace HybridModelBinding.ModelBinding
{
    internal sealed class BodyValueProvider : IValueProvider
    {
        public BodyValueProvider(object model)
        {
            foreach (var property in model.GetPropertiesNotPartOfType<IHybridBoundModel>())
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
