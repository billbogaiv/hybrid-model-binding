using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Collections.Generic;

namespace HybridModelBinding
{
    public class DefaultUnsafeHybridModelBinder : HybridModelBinder
    {
        public DefaultUnsafeHybridModelBinder(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory)
            : base(true)
        {
            base
                .AddModelBinder(ModelBinder.Body, new BodyModelBinder(formatters, readerFactory))
                .AddValueProviderFactory(ValueProvider.Form, new FormValueProviderFactory())
                .AddValueProviderFactory(ValueProvider.Route, new RouteValueProviderFactory())
                .AddValueProviderFactory(ValueProvider.QueryString, new QueryStringValueProviderFactory());
        }
    }
}
