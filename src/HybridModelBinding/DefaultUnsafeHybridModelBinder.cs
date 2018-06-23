using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using static HybridModelBinding.Source;

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
                .AddModelBinder(Body, new BodyModelBinder(formatters, readerFactory))
                .AddValueProviderFactory(Form, new FormValueProviderFactory())
                .AddValueProviderFactory(Route, new RouteValueProviderFactory())
                .AddValueProviderFactory(QueryString, new QueryStringValueProviderFactory());
        }
    }
}
