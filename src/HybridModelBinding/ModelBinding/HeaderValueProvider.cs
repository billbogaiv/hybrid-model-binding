using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace HybridModelBinding.ModelBinding
{
    /// <summary>
    /// Modified from https://github.com/aspnet/Mvc/blob/8d66f104f7f2ca42ee8b21f75b0e2b3e1abe2e00/src/Microsoft.AspNetCore.Mvc.Core/ModelBinding/QueryStringValueProvider.cs.
    /// </summary>
    public class HeaderValueProvider : BindingSourceValueProvider, IEnumerableValueProvider
    {
        public HeaderValueProvider(
            BindingSource bindingSource,
            IHeaderDictionary values,
            CultureInfo culture)
            : base(bindingSource)
        {
            if (bindingSource == null)
            {
                throw new ArgumentNullException(nameof(bindingSource));
            }

            this.values = values ?? throw new ArgumentNullException(nameof(values));
            Culture = culture;
        }

        public CultureInfo Culture { get; private set; }

        private readonly IHeaderDictionary values;
        private PrefixContainer prefixContainer;

        protected PrefixContainer PrefixContainer
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

        public override bool ContainsPrefix(string prefix)
        {
            return PrefixContainer.ContainsPrefix(prefix);
        }

        public virtual IDictionary<string, string> GetKeysFromPrefix(string prefix)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }

            return PrefixContainer.GetKeysFromPrefix(prefix);
        }

        public override ValueProviderResult GetValue(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var values = this.values[key];

            if (values.Count == 0)
            {
                return ValueProviderResult.None;
            }
            else
            {
                return new ValueProviderResult(values, Culture);
            }
        }
    }
}
