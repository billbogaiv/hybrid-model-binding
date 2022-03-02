using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Internal;
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

            _values = values ?? throw new ArgumentNullException(nameof(values));
            Culture = culture;
        }

        public CultureInfo Culture { get; }

        private readonly IHeaderDictionary _values;
        private PrefixContainer _prefixContainer;

        protected PrefixContainer PrefixContainer
        {
            get
            {
                if (_prefixContainer == null)
                {
                    _prefixContainer = new PrefixContainer(_values.Keys);
                }

                return _prefixContainer;
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

            var values = _values[key];

            if (values.Count == 0)
            {
                return ValueProviderResult.None;
            }

            return new ValueProviderResult(values, Culture);
        }
    }
}
