using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace HybridModelBinding
{
    public class DefaultHybridModelBinder : HybridModelBinder
    {
        public DefaultHybridModelBinder(IHttpRequestStreamReaderFactory readerFactory)
        {
            AddModelBinder(new BodyModelBinder(readerFactory))
                .AddMappedValueProviderFactory<FromFormAttribute>(new FormValueProviderFactory())
                .AddMappedValueProviderFactory<FromRouteAttribute>(new RouteValueProviderFactory())
                .AddMappedValueProviderFactory<FromQueryAttribute>(new QueryStringValueProviderFactory());
        }
    }
}
