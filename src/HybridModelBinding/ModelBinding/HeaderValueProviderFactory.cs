using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace HybridModelBinding.ModelBinding
{
    public class HeaderValueProviderFactory : IValueProviderFactory
    {
        /// <summary>
        /// We need to re-create a Header BindingSource since the default ASP.NET MVC version
        /// is greedy and that won't work as a ValueProvider.
        /// 
        /// Ref. https://github.com/aspnet/Mvc/blob/8d66f104f7f2ca42ee8b21f75b0e2b3e1abe2e00/src/Microsoft.AspNetCore.Mvc.Abstractions/ModelBinding/BindingSource.cs
        /// Ref. https://github.com/aspnet/Mvc/blob/8d66f104f7f2ca42ee8b21f75b0e2b3e1abe2e00/src/Microsoft.AspNetCore.Mvc.Core/ModelBinding/BindingSourceValueProvider.cs#L41
        /// ArgumentException: The provided binding source 'Header' is a greedy data source. 'BindingSourceValueProvider' does not support greedy data sources.
        /// Parameter name: bindingSource
        /// </summary>
        private BindingSource Header = new BindingSource(
            BindingSource.Header.Id,
            BindingSource.Header.DisplayName,
            false,
            BindingSource.Header.IsFromRequest);

        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var headers = context.ActionContext.HttpContext.Request.Headers;
            if (headers != null && headers.Count > 0)
            {
                var valueProvider = new HeaderValueProvider(
                    Header,
                    headers,
                    CultureInfo.InvariantCulture);

                context.ValueProviders.Add(valueProvider);
            }

            return Task.CompletedTask;
        }
    }
}
