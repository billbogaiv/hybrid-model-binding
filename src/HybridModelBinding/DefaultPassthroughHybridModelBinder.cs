using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Collections.Generic;
using static HybridModelBinding.Source;

namespace HybridModelBinding
{
    public class DefaultPassthroughHybridModelBinder : HybridModelBinder
    {
        public DefaultPassthroughHybridModelBinder(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory)
            : base(Strategy.Passthrough)
        {
            base
                .AddModelBinder(Body, new BodyModelBinder(formatters, readerFactory))
                .AddValueProviderFactory(Form, new FormValueProviderFactory())
                .AddValueProviderFactory(Route, new RouteValueProviderFactory())
                .AddValueProviderFactory(QueryString, new QueryStringValueProviderFactory());
        }
    }
}
