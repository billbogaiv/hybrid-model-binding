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
                .AddMappedValueProviderFactory(new FormValueProviderFactory())
                .AddMappedValueProviderFactory(new RouteValueProviderFactory())
                .AddMappedValueProviderFactory(new QueryStringValueProviderFactory());
        }
    }
}
