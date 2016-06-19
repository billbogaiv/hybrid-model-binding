using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace HybridModelBinding
{
    public class DefaultGreedyHybridModelBinder : HybridModelBinder
    {
        public DefaultGreedyHybridModelBinder(IHttpRequestStreamReaderFactory readerFactory)
            :base(true)
        {
            AddModelBinder(new BodyModelBinder(readerFactory))
                .AddValueProviderFactory(new FormValueProviderFactory())
                .AddValueProviderFactory(new RouteValueProviderFactory())
                .AddValueProviderFactory(new QueryStringValueProviderFactory());
        }
    }
}
