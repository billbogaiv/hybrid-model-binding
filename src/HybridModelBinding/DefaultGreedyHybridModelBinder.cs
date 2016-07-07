using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Collections.Generic;

namespace HybridModelBinding
{
    public class DefaultGreedyHybridModelBinder : HybridModelBinder
    {
        public DefaultGreedyHybridModelBinder(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory)
            :base(true)
        {
            AddModelBinder(new BodyModelBinder(formatters, readerFactory))
                .AddValueProviderFactory(new FormValueProviderFactory())
                .AddValueProviderFactory(new RouteValueProviderFactory())
                .AddValueProviderFactory(new QueryStringValueProviderFactory());
        }
    }
}
